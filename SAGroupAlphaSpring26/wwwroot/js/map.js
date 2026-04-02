/**
 * map.js
 * Now imports from PlayerMapFunctions.js for player view related logic, and MapAPI.js for tracking token data.
 */
import { repositionToken, toggleTokenInvisibility, syncBoard, removeToken } from "./PlayerMapFunctions.js";
import { saveTokenPositions } from "./MapApi.js";

// Stores session ID, gets from Razor view.
const sessionId = window.sessionId;

// Stores the IDs of tokens that need to be deleted from the database when the save button is hit.
let tokensToDelete = [];

// Sets isPlayerView Variable.
let isPlayerView = false;

// MapBoard Element & Role.
const mapBoard = document.getElementById('map-board');
const playerView = mapBoard?.dataset?.role;

// Instantiates the broadcaster object for executing functions on the playerview side.
const bc = new BroadcastChannel('map_channel');

// The Local State Dictionary - The "Source of Truth" for offline changes
let tokenData = {};

if (mapBoard) {
    mapBoard.addEventListener('contextmenu', (e) => {
        e.preventDefault();
    });
}

// PLAYER VIEW BROADCAST LOGIC
if (playerView === 'player') {
    isPlayerView = true;
    console.log('Player view set to true.');

    // Switch statement within bc.onmessage for running functions within the playerview.
    bc.addEventListener('message', (e) => {
        // console.log("Player View received broadcast:", e.data); this log clogs up the logs.
        switch (e.data.action) {
            case ("tokenMove"):
                // Repositions the token on the player view according to the data sent from the DM view. 
                // Currently runs on 'drag' for live movement, but can be moved to 'stop' if it's too intensive.
                repositionToken(e.data);
                break;
            case ("toggleIn"):
                // Toggles the invisibility of a token on the player view. Currently bugged and doesn't respect the visibility bool.
                toggleTokenInvisibility(e.data);
                break;
            case ("syncAll"):
                // Sends ALL local token data to the player view for syncing.
                syncBoard(e.data.allTokens);
                break;
            case ("reload"):
                // Reloads the player view, used after saving to prevent token duplication.
                console.log("DM Saved, reloading player view page...");
                window.location.reload();
                break;
            case ("tokenDelete"): 
            // deletes token from the player view.
                removeToken(e.data);
        }
    });

    // Runs when the player view first loads and requests the local tokenData from the DM view. Has a delay to make sure the data is ready to send.
    setTimeout(() => {
        console.log("Asking DM for unsaved token positions...");
        bc.postMessage({ action: 'requestSync' });
    }, 500);
}


// DM VIEW LOGIC
if (!isPlayerView) {
    // switched to a bc .addEventListener because I think they are being overwritten...
    // DM VIEW BROADCAST LOGIC - Listens for the player view to 'requestSync', then sends the local tokenData to update the player view.
    bc.addEventListener('message', (e) => {
        if (e.data.action === 'requestSync') {
            console.log(`DM View received 'requestSync' Sending ${Object.keys(tokenData).length} tokens...`);

            bc.postMessage({
                action: 'syncAll',
                allTokens: tokenData
            });
                }
            });

    document.addEventListener('DOMContentLoaded', () => {
        console.log('Hello! \nFrom -Mr. Vang (and the Clean Architecture!)');

        if (!mapBoard) return console.error('Map board not found!');

        // Uses local tokenData dictionary to create the tokens.
        $('.draggable-token').each(function () {
            const token = $(this);
            const tId = token.attr('id');

            tokenData[tId] = {
                id: token.data('tokenid'),
                pieceId: token.data('pieceid'),
                src: token.attr('src'),
                x: parseFloat(token[0].style.left) || 0,
                y: parseFloat(token[0].style.top) || 0,
                isVisible: !token.hasClass('dmOpacityToggle')
            };
        });

        // makes the tokens draggable.
        $('.sidebar-piece').draggable({
            helper: 'clone',
            revert: 'invalid',
            appendTo: 'body',
            cursor: 'grabbing',
            zIndex: 999
        });

        $('.draggable-token').draggable(draggablePieceInfo);

        // Sets map-board to a droppable area that accepts .sidebar-piece class.
        $('#map-board').droppable({
            accept: '.sidebar-piece',
            drop: function (event, ui) {
                const $board = $(this);
                const boardOffset = $board.offset();

                // removes offsets.
                const dropX = ui.offset.left - boardOffset.left;
                const dropY = ui.offset.top - boardOffset.top;

                // Sets percentages for the token positions
                const leftPerc = (dropX / $board.width()) * 100;
                const topPerc = (dropY / $board.height()) * 100;

                // Build token data offline
                const localTokenData = {
                    id: 'temp-' + Date.now(),
                    pieceId: ui.draggable.data('pieceid'),
                    src: ui.draggable.attr('src'),
                    x: leftPerc,
                    y: topPerc,
                    isVisible: true
                };

                // Saves tokens to local dictionary
                tokenData[localTokenData.id] = localTokenData;

                makeNewToken(localTokenData);
            }
        });

        // sets area where tokens can be deleted by dragging and dropping them into.
        $('#delete-area').droppable({
            accept: '.draggable-token',
            over: function () { $(this).addClass('delete-hover'); },
            out: function () { $(this).removeClass('delete-hover'); },
            drop: function (event, ui) {

                // Gets the onboard token id and then the database id (if it exists, temp tokens wouldn't have one.).

                const htmlId = ui.draggable.attr('id');

                const dbId = ui.draggable.data('tokenid');

                // If it's a real token (not temp), stage it for DB deletion (temp tokens don't don't exist in the Database yet.)
                if (dbId && !htmlId.startsWith('temp-')) {
                    tokensToDelete.push(dbId);
                }

                // Remove from local data and from the DOM content.
                delete tokenData[htmlId];
                ui.draggable.remove();

                // Broadcast deletion to player view.
                bc.postMessage({
                    action: 'tokenDelete',
                    tokenId: htmlId
                });
            }
        });

        // Attach event for each .draggable-token.
        document.querySelectorAll('.vangtokendiv, .draggable-token').forEach(attachContextMenu);


        // Attaching the tokenData and Session ID to the Save button so we can save to our database when it is clicked.
        document.getElementById('btn-save')?.addEventListener('click', () => saveTokenPositions(tokenData, sessionId, tokensToDelete));
    });

    // Requires us to pass in the tokenData
    const makeNewToken = (data) => {
        const tokenImg = document.createElement('img');
        tokenImg.id = data.id;
        tokenImg.src = data.src;

        tokenImg.classList.add('draggable-token');
        tokenImg.dataset.tokenid = data.id;
        tokenImg.dataset.pieceid = data.pieceId;
        tokenImg.draggable = true;

        tokenImg.style.position = 'absolute';
        tokenImg.style.left = `${data.x}%`;
        tokenImg.style.top = `${data.y}%`;
        tokenImg.style.width = `5%`;
        tokenImg.style.height = `auto`;
        tokenImg.style.zIndex = 99;

        mapBoard.appendChild(tokenImg);

        $(`#${tokenImg.id}`).draggable(draggablePieceInfo);
        attachContextMenu(tokenImg);

        // tells playerview to create a token via 'tokenMove' (an update or add function).
        bc.postMessage({
            tokenId: data.id,
            tokenImgSrc: data.src,
            tokenLeftPerc: data.x,
            tokenTopPerc: data.y,
            action: 'tokenMove'
        });
    };

    const draggablePieceInfo = {
        containment: '#map-board',
        scroll: false,
        create: (event, ui) => {
            console.log('Token Created:', Date.now());
        },
        start: (event, ui) => {
            const tokenId = event.target.id;
            // if no token data, create it.
            if (!tokenData[tokenId]) {
                tokenData[tokenId] = {
                    id: event.target.dataset.tokenid || tokenId,
                    pieceId: event.target.dataset.pieceid,
                    src: event.target.src,
                    x: parseFloat(event.target.style.left) || 0,
                    y: parseFloat(event.target.style.top) || 0,
                    isVisible: !event.target.classList.contains('dmOpacityToggle')
                };
            }
        },
        stop: (event, ui) => {

            // Prevents the application from crashing after a token gets deleted. (After deletion the stop event tries to run)
            if (!document.getElementById(event.target.id)) return;

            const topPerc = $(`#${event.target.id}`).position().top / $(`#map-board`).height() * 100;
            const leftPerc = $(`#${event.target.id}`).position().left / $(`#map-board`).width() * 100;

            event.target.style.top = `${topPerc}%`;
            event.target.style.left = `${leftPerc}%`;

            // When drag stops, update the local tokenData.
            tokenData[event.target.id].y = topPerc;
            tokenData[event.target.id].x = leftPerc;
        },
        drag: (event, ui) => {
            const $board = $('#map-board');
            const topPerc = (ui.position.top / $board.height()) * 100;
            const leftPerc = (ui.position.left / $board.width()) * 100;

            // Tells the Playerview to update the token's position. If this is too much to handle in 'drag' we can move it to 'stop'.
            bc.postMessage({
                tokenId: event.target.id,
                tokenImgSrc: event.target.src,
                tokenTopPerc: topPerc,
                tokenLeftPerc: leftPerc,
                action: 'tokenMove',
            });
        },
    };

    // toggles opacity of tokens when right clicked (context menu)
    const attachContextMenu = (token) => {
        token.addEventListener('contextmenu', (e) => {
            e.preventDefault();
            token.classList.toggle("dmOpacityToggle");

            const tId = token.id;

            // respects the backends visibility bool.
            if (tokenData[tId]) {
                tokenData[tId].isVisible = !token.classList.contains("dmOpacityToggle");
            }

            // send the toggleIn command to the playerview so it can make the tokens invisible on player view.
            bc.postMessage({
                tokenId: tId,
                action: 'toggleIn'
            });
        });
    };




}