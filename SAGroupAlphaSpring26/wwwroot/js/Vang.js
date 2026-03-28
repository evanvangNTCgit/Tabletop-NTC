import { repositionToken, toggleTokenInvisibility } from "../js/PlayerMapFunctions.js";

const mapBoard = document.getElementById('map-board');
mapBoard.addEventListener('contextmenu', (e) => {
    e.preventDefault();
});

let isPlayerView = false;

// Instantiates the broadcaster object for executing functions on the playerview side.
const bc = new BroadcastChannel('map_channel');

// sets playerView to 'dm' or 'player' and then assigns isPlayerView to true if set to 'player'
const playerView = mapBoard.dataset.role;

//assigns isPlayerView to true if set to 'player'
if (playerView === 'player') {
    isPlayerView = true;
    console.log('Player view set to true.');

    bc.onmessage = (e) => {
        console.log(e);
        switch (e.data.action) {
            case ("tokenMove"):
                repositionToken(e.data);
                break;
            case ("toggleIn"):
                toggleTokenInvisibility(e.data);
                break;
        }
    }
}

if (!isPlayerView) {
    // https://api.jqueryui.com/draggable/#event-create
    document.addEventListener('DOMContentLoaded', () => {
        console.log('Hello! \nFrom -Mr. Vang');
        // Dont be afraid of the dollar sign
        // JQuery selector.
        // Just grabs all the .draggable-token DOMS like a queryselectorall
        $('.draggable-token').draggable(draggablePieceInfo);

        // For every side-piece
        // My take for simplicity is to just give the user a popup
        // That they can select yes or no on.
        // It will make the piece on the map set it at a default 0x 0y and invisible to the player.
        // https://jqueryui.com/dialog/
        document.querySelectorAll('.sidebar-piece').forEach(async (e) => {
            e.addEventListener('click', async (e) => {
                // E returns a pointer event so I need to get necessary data to work with the create token POST.

                const pieceId = e.srcElement.dataset.pieceid;
                // Gets the session ID form the uri
                const currentSessionId = window.sessionId;

                const x = 0;
                const y = 0;
                console.log(e);
                $('#dialog').dialog({
                    // This will prevent the user from interacting with background (map) when popup shows.
                    modal: true,
                    // Adding user button choices to the popup...
                    buttons: [
                        {
                            text: 'Yes',
                            click: async () => {
                                // Close the dialog.
                                $('#dialog').dialog('close');

                                const response = await fetch('/Map/CreateToken', {
                                    method: 'POST',
                                    headers: { 'Content-Type': 'application/json' },
                                    body: JSON.stringify({
                                        pieceId,
                                        sessionId: currentSessionId,
                                        X: x,
                                        Y: y,
                                    }),
                                });

                                if (!response.ok) {
                                    throw new Error(`HTTP ${response.status}`);
                                }

                                const result = await response.json();
                                console.log('Token created:', result);
                                makeNewToken(result);
                            },
                        },
                        {
                            text: 'No',
                            click: () => {
                                // Close the dialog.
                                $('#dialog').dialog('close');
                            },
                        },
                    ],
                });
                document.getElementById('dialog').classList.remove('hidden');
                console.log('Should show dialog...');
            });
        });

        // For all the tokens give the right click event.
        document.querySelectorAll('.vangtokendiv').forEach((e) => {
            e.addEventListener('contextmenu', (e) => {
                e.preventDefault();
                event.target.classList.toggle('dmOpacityToggle');
                bc.postMessage({
                    tokenId: `${e.target.id}`,
                    action: 'toggleIn',
                });
            });
        });
    });

    // Requires id of the token.
    const makeNewToken = (data) => {


        // Get the img element DOM in the sidebar.
        const sidebarPiece = document.querySelector(
            `[data-pieceid="${data.pieceImageID}"]`,
        );

        // if we got it clone it and make it a normal draggable piece.
        if (sidebarPiece) {
            const tokenImg = sidebarPiece.cloneNode(true);
            tokenImg.id = `token-placed-${data.id}`;
            tokenImg.classList.remove('sidebar-piece');
            tokenImg.classList.add('draggable-token');
            tokenImg.dataset.tokenid = data.id;
            tokenImg.draggable = true;
            tokenImg.style.position = 'absolute';
            tokenImg.style.left = `100px`;
            tokenImg.style.top = `100px`;
            tokenImg.style.zIndex = 99;
            mapBoard.appendChild(tokenImg);

            $(`#${tokenImg.id}`).draggable(draggablePieceInfo);

            // Send creation event to player
            // bc.postMessage({ tokenid: result.id, x: x.toFixed(0), y: y.toFixed(0), isVisible: true, action: 'tokenMoved' });
        } else {
            console.log('Could not make the token!');
        }
    };

    const draggablePieceInfo = {
        containment: '#map-board',
        scroll: false,
        create: (event, ui) => {
            console.log('Token Created:', Date.now());
        },
        stop: (event, ui) => {
        },
        drag: (event, ui) => {
            console.log(event);
            // Get the Token x and y; 
            const X = event.originalEvent.target.x;
            const Y = event.originalEvent.target.y;

            // Then the ID: 
            const tokenId = event.originalEvent.target.id;

            // Send over the img source:
            const imgSrc = event.originalEvent.target.src;
            // console.log(event);

            bc.postMessage({
                tokenId: `${tokenId}`,
                tokenImgSrc: `${imgSrc}`,
                tokenX: X,
                tokenY: Y,
                action: 'tokenMove',
            });
        },
    };
}