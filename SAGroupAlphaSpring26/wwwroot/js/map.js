/**
 * map.js
 * Now imports from PlayerMapFunctions.js for player view related logic, and MapAPI.js for tracking token data.
 */
import { repositionToken, toggleTokenInvisibility, syncBoard, removeToken, updateZIndex, setTokenVisibility } from "./PlayerMapFunctions.js?v=999";
import { saveTokenPositions, saveSessionNotes } from "./mapAPI.js?v=1001";

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
let selectedTokenId = null;

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
                // Toggles the invisibility of a token on the player view.
                toggleTokenInvisibility(e.data);
                break;
            // Sets the visibility of a token on the playerview, this should replace toggleIn, but I'm keeping both for now just in case.
            case ("setVisibility"):
                setTokenVisibility(e.data);
                break;
            // Updates the z index of a token for the playerview.
            case ("updateZIndex"):
                updateZIndex(e.data);
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
        console.log('Hello! \nFrom -Mr. Vang');

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
                zIndex: parseInt(token[0].style.zIndex) || 1,
                isVisible: !token.hasClass('dmOpacityToggle'),
                name: token.data('name') || token.data('piecename') || "",
                notes: token.data('notes') || ""
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
                    zIndex: 99,
                    isVisible: true,
                    name: ui.draggable.data('piecename') || "",
                    notes: ""
                };

                // Saves tokens to local dictionary
                tokenData[localTokenData.id] = localTokenData;

                makeNewToken(localTokenData);
                selectedTokenId = localTokenData.id;
                bringToFront(localTokenData.id);
                updateInfoPanel();
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

                if (selectedTokenId === htmlId) {
                    selectedTokenId = null;
                    const panel = document.getElementById('token-info-panel');
                    if (panel) panel.style.display = 'none';
                }

                // Broadcast deletion to player view.
                bc.postMessage({
                    action: 'tokenDelete',
                    tokenId: htmlId
                });
                $(this).removeClass('delete-hover');
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
        tokenImg.style.zIndex = data.zIndex || 99;

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
                    zIndex: parseInt(event.target.style.zIndex) || 1,
                    isVisible: !event.target.classList.contains('dmOpacityToggle'),
                    name: event.target.dataset.name || event.target.dataset.piecename || "",
                    notes: event.target.dataset.notes || ""
                };
            }
            selectedTokenId = tokenId;
            bringToFront(tokenId);
            updateInfoPanel();
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

            // Tells the Playerview to update the token's position. If this is too much to handle in 'drag' we can move it to 'stop' later.
            bc.postMessage({
                tokenId: event.target.id,
                tokenImgSrc: event.target.src,
                tokenTopPerc: topPerc,
                tokenLeftPerc: leftPerc,
                action: 'tokenMove',
            });
        },
    };

    // Right Click Function for tokens.
    const attachContextMenu = (token) => {
        token.addEventListener('contextmenu', (e) => {
            e.preventDefault();
            token.classList.toggle("dmOpacityToggle");

            const tId = token.id;

            // respect the backends visibility bool.
            if (tokenData[tId]) {
                tokenData[tId].isVisible = !token.classList.contains("dmOpacityToggle");
            }

            // send the toggleIn command to the playerview.
            bc.postMessage({
                tokenId: tId,
                action: 'toggleIn'
            });

            // Syncs checkbox if the token is right clicked whilst selected.
            if (selectedTokenId === tId) {
                updateInfoPanel();
            }
        });

        // Left click selects the token and updates the info panel.
        token.addEventListener('click', (e) => {
            selectedTokenId = token.id;
            updateInfoPanel();
        });
    };

    // Function to bring a token to the front of the stack. (ontop of the other tokens)
    const bringToFront = (tokenId) => {
        if (!tokenData[tokenId]) return;

        // Sort all active tokens by their current Z-Indexes
        const tokensArray = Object.entries(tokenData).map(([key, val]) => {
            val.htmlId = key;
            return val;
        }).sort((a, b) => {
            // zA = z index of token a, zB = z index of token b.
            const zA = parseInt(a.zIndex) || 1;
            const zB = parseInt(b.zIndex) || 1;

            // Failsafe for tokens with the same z-index...
            if (zA === zB) {
                // convert to strings.
                const zAString = String(a.id || "");
                const zBString = String(b.id || "");
                return zAString.localeCompare(zBString);
            }

            return zA - zB;
        });

        const targetToken = tokenData[tokenId];
        let currentZ = 1;

        // Iterates through sorted tokens, updating and ordering by z index.
        for (const token of tokensArray) {
            if (token.htmlId !== tokenId) {
                if (token.zIndex !== currentZ) {
                    token.zIndex = currentZ;
                    const el = document.getElementById(token.htmlId);
                    if (el) el.style.zIndex = currentZ;
                    bc.postMessage({ tokenId: token.htmlId, zIndex: currentZ, action: 'updateZIndex' });
                }
                currentZ++;
            }
        }

        // Z index set to the highest value (based on how many tokens exist)
        if (targetToken.zIndex !== currentZ) {
            targetToken.zIndex = currentZ;
            const targetEl = document.getElementById(tokenId);
            if (targetEl) targetEl.style.zIndex = currentZ;
            bc.postMessage({ tokenId: tokenId, zIndex: currentZ, action: 'updateZIndex' });
        }
    };

    // Updates the InfoPanel based on currently selected token. Panel is hidden if no token is selected.
    const updateInfoPanel = () => {
        const panel = document.getElementById('token-info-panel');
        if (!panel || !selectedTokenId) return;

        const data = tokenData[selectedTokenId];
        if (!data) return;

        panel.style.display = 'block';
        document.getElementById('token-info-image').src = data.src;
        document.getElementById('token-info-name').value = data.name || "";
        document.getElementById('token-info-zindex').value = data.zIndex || 1;
        document.getElementById('token-info-notes').value = data.notes || "";
        document.getElementById('token-info-visibility').checked = data.isVisible;
    };

    // Bind panel events
    document.addEventListener('DOMContentLoaded', () => {
        document.getElementById('token-info-name')?.addEventListener('input', (e) => {
            if (selectedTokenId && tokenData[selectedTokenId]) {
                tokenData[selectedTokenId].name = e.target.value;
                const tokenEl = document.getElementById(selectedTokenId);
                if (tokenEl) tokenEl.dataset.name = e.target.value;
            }
        });

        document.getElementById('token-info-notes')?.addEventListener('input', (e) => {
            if (selectedTokenId && tokenData[selectedTokenId]) {
                tokenData[selectedTokenId].notes = e.target.value;
                const tokenEl = document.getElementById(selectedTokenId);
                if (tokenEl) tokenEl.dataset.notes = e.target.value;
            }
        });

        document.getElementById('token-info-zindex')?.addEventListener('change', (e) => {
            if (selectedTokenId && tokenData[selectedTokenId]) {
                let newZ = Math.max(0, parseInt(e.target.value) || 0);

                if (newZ <= 0) {
                    newZ = 1;
                }

                e.target.value = newZ;

                tokenData[selectedTokenId].zIndex = newZ;
                const tokenEl = document.getElementById(selectedTokenId);
                if (tokenEl) tokenEl.style.zIndex = newZ;

                bc.postMessage({
                    tokenId: selectedTokenId,
                    zIndex: newZ,
                    action: 'updateZIndex'
                });
            }
        });

        document.getElementById('token-info-visibility')?.addEventListener('change', (e) => {
            if (selectedTokenId && tokenData[selectedTokenId]) {
                let isVis = e.target.checked;
                tokenData[selectedTokenId].isVisible = isVis;
                const tokenEl = document.getElementById(selectedTokenId);
                if (tokenEl) {
                    if (!isVis) tokenEl.classList.add("dmOpacityToggle");
                    else tokenEl.classList.remove("dmOpacityToggle");
                }

                bc.postMessage({
                    tokenId: selectedTokenId,
                    isVisible: isVis,
                    action: 'setVisibility'
                });
            }
        });

        document.getElementById('token-info-delete')?.addEventListener('click', (e) => {
            if (selectedTokenId && tokenData[selectedTokenId]) {
                const htmlId = selectedTokenId;
                const dbId = tokenData[selectedTokenId].id;

                // If it's a real token (not temp), stage it for DB deletion
                if (dbId && !String(htmlId).startsWith('temp-')) {
                    tokensToDelete.push(dbId);
                }

                // Remove from local data
                delete tokenData[htmlId];
                const tokenEl = document.getElementById(htmlId);
                if (tokenEl) tokenEl.remove();

                selectedTokenId = null;
                const panel = document.getElementById('token-info-panel');
                if (panel) panel.style.display = 'none';

                // Broadcast deletion to player view.
                bc.postMessage({
                    action: 'tokenDelete',
                    tokenId: htmlId
                });
            }
        });

        // Toggle Session Notes Panel
        document.getElementById('btn-toggle-notes')?.addEventListener('click', () => {
            const notesPanel = document.getElementById('session-notes-panel');
            if (notesPanel) {
                notesPanel.style.display = (notesPanel.style.display === 'flex') ? 'none' : 'flex';
            }
        });

        // Toggle Token Info Panel
        document.getElementById('btn-toggle-token-info')?.addEventListener('click', () => {
            const tokenPanel = document.getElementById('token-info-panel');
            if (tokenPanel) {
                tokenPanel.style.display = (tokenPanel.style.display === 'block') ? 'none' : 'block';
            }
        });

        // Save only session notes
        document.getElementById('btn-save-notes')?.addEventListener('click', (e) => {
            const notesTextArea = document.getElementById('session-notes-textarea');
            if (notesTextArea && sessionId) {
                const btn = e.target;
                const originalText = btn.innerText;
                btn.innerText = "Saving...";
                btn.disabled = true;

                saveSessionNotes(sessionId, notesTextArea.value)
                    .done(() => {
                        btn.innerText = "Saved!";
                        setTimeout(() => {
                            btn.innerText = originalText;
                            btn.disabled = false;
                        }, 2000);
                    })
                    .fail((xhr) => {
                        console.error("Save session notes error:", xhr.responseText);
                        alert('Failed to save session notes.');
                        btn.innerText = originalText;
                        btn.disabled = false;
                    });
            }
        });
    });
}