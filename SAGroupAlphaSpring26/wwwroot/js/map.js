/**
 * D&D Map JS - Fixed Drag, Drop, Zoom, Pan for Pieces/Tokens
 */

const sessionId = window.sessionId;

let isPlayerView = false;
let zoomLevel = 1;
let panX = 0;
let panY = 0;
let isPanning = false;
let startX, startY;

let data = [];

// Wait for the DOM to load before initializing the map logic.
document.addEventListener('DOMContentLoaded', () => {
    const mapBoard = document.getElementById('map-board');
    const currentSessionId = window.sessionId;

    if (!mapBoard) {
        return console.error('Map board not found!');
    }

    const bc = new BroadcastChannel('map_channel');
    bc.postMessage("Hello world!");

    const playerView = mapBoard.dataset.role;

    if (playerView === 'player') {
        isPlayerView = true;
        console.log('Player view set to true.');
    }

    ////////////////////////////////////////////////////////////////////////////
    // PLAYER SPECIFIC LOGIC
    ////////////////////////////////////////////////////////////////////////////
    if (isPlayerView === true) {
        updateTokenPositions();

        bc.onmessage = function (event) {
            const tokenData = event.data;
            if (tokenData.tokenid) { // Ensure we are getting movement data, not "Hello world!"
                updateSingleTokenPosition(tokenData);
                console.log("Player View received:", event.data);
            }
        };
    }
    //////////////////////////////////////////////////////////////////////////////
    // DM SPECIFIC LOGIC
    ////////////////////////////////////////////////////////////////////////////
    else {
        console.log('Currently DM View.');

        // Handle drag start for both sidebar pieces and existing tokens on the map.
        document.addEventListener('dragstart', (e) => {
            if (e.target.classList.contains('sidebar-piece')) {
                const pieceId = e.target.dataset.pieceid;
                if (pieceId) {
                    e.dataTransfer.effectAllowed = 'copy';
                    e.dataTransfer.setData('text/plain', pieceId);
                    e.dataTransfer.setDragImage(e.target, 26, 26);
                    console.log('Dragging sidebar piece:', pieceId);
                }
            } else if (e.target.classList.contains('draggable-token')) {
                e.dataTransfer.effectAllowed = 'move';
                e.dataTransfer.setData('text/plain', `token-placed-${e.target.dataset.tokenid}`);
                e.dataTransfer.setDragImage(e.target, 26, 26);
                console.log('Dragging existing token');
            }
        });

        document.addEventListener('dragend', (e) => {
            if (e.target.classList.contains('map-piece')) {
                e.target.style.opacity = '1';
            }
        });

        mapBoard.addEventListener('dragover', (e) => {
            e.preventDefault();
            mapBoard.classList.add('drag-over');
            e.dataTransfer.dropEffect = 'all';
        });

        mapBoard.addEventListener('dragleave', (e) => {
            mapBoard.classList.remove('drag-over');
        });

        // Handle dropping both new pieces from the sidebar and moving existing tokens on the map.
        mapBoard.addEventListener('drop', async (e) => {
            e.preventDefault();
            mapBoard.classList.remove('drag-over'); // Ensure this is removed on drop
            const data = e.dataTransfer.getData('text/plain');
            const rect = mapBoard.getBoundingClientRect();
            const x = (e.clientX - rect.left) / zoomLevel - 26;
            const y = (e.clientY - rect.top) / zoomLevel - 26;

            console.log('Drop:', { data, x: x.toFixed(0), y: y.toFixed(0), currentSessionId });

            if (data.startsWith('token-placed-')) {
                console.log('Moving existing token (visual only)');
                return;
            }

            // Handle new piece dropped from sidebar
            const pieceId = parseInt(data);
            if (!isNaN(pieceId)) {
                try {
                    const response = await fetch('/Map/CreateToken', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({ pieceId, sessionId: currentSessionId, X: x, Y: y })
                    });

                    if (!response.ok) {
                        throw new Error(`HTTP ${response.status}`);
                    }

                    const result = await response.json();
                    console.log('Token created:', result);

                    if (result.id) {
                        const sidebarPiece = document.querySelector(`[data-pieceid="${pieceId}"]`);
                        if (sidebarPiece) {
                            const tokenImg = sidebarPiece.cloneNode(true);
                            tokenImg.id = `token-placed-${result.id}`;
                            tokenImg.classList.remove('sidebar-piece');
                            tokenImg.classList.add('draggable-token');
                            tokenImg.dataset.tokenid = result.id;
                            tokenImg.draggable = true;
                            tokenImg.style.position = 'absolute';
                            tokenImg.style.left = `${x}px`;
                            tokenImg.style.top = `${y}px`;
                            tokenImg.style.zIndex = (getMaxZIndex() + 1).toFixed();
                            mapBoard.appendChild(tokenImg);
                            makeDraggable(tokenImg);

                            // Send creation event to player
                            bc.postMessage({ tokenid: result.id, x: x.toFixed(0), y: y.toFixed(0) });
                        }
                    }
                } catch (error) {
                    console.error('Create token failed:', error);
                    alert('Failed to add piece. Check console.');
                }
            }
        });

        // Makes a token draggable and handles its movement logic.
        function makeDraggable(token) {
            token.draggable = true;
            token.style.cursor = 'grab';
            token.addEventListener('dragstart', () => {
                token.style.opacity = '0.5';
                token.style.zIndex = (getMaxZIndex() + 1).toString();
            });
            token.addEventListener('dragend', (e) => {
                token.style.opacity = '1';
                const rect = mapBoard.getBoundingClientRect();
                const newX = (e.clientX - rect.left) / zoomLevel - 26;
                const newY = (e.clientY - rect.top) / zoomLevel - 26;
                token.style.left = `${newX}px`;
                token.style.top = `${newY}px`;
                console.log('Token moved:', { tokenid: token.dataset.tokenid, x: newX.toFixed(0), y: newY.toFixed(0) });

                // Sends token movement to player view!
                bc.postMessage({
                    tokenid: token.dataset.tokenid, x: newX.toFixed(0), y: newY.toFixed(0)
                });
            });
        }

        // Init existing tokens for DM
        document.querySelectorAll('.draggable-token').forEach(makeDraggable);
    }

    ////////////////////////////////////////////////////////////////////////////
    // SHARED ZOOM & PAN LOGIC (Both DM and Player need this)
    ////////////////////////////////////////////////////////////////////////////
    document.getElementById('zoom-in')?.addEventListener('click', () => zoomMap(0.1));
    document.getElementById('zoom-out')?.addEventListener('click', () => zoomMap(-0.1));
    document.getElementById('zoom-reset')?.addEventListener('click', resetZoomPan);

    // Zoom with mouse wheel
    mapBoard.addEventListener('wheel', (e) => {
        e.preventDefault();
        zoomMap(e.deltaY * -0.001);
    }, { passive: false });

    // Start panning on middle mouse button down
    mapBoard.addEventListener('mousedown', (e) => {
        if (e.button === 1) { // Middle click to pan
            isPanning = true;
            startX = e.clientX - panX;
            startY = e.clientY - panY;
            mapBoard.style.cursor = 'grabbing';
            e.preventDefault();
        }
    });

    // Pan the map on mouse move if panning is active
    document.addEventListener('mousemove', (e) => {
        if (isPanning) {
            panX = e.clientX - startX;
            panY = e.clientY - startY;
            updateTransform();
        }
    });

    // Stop panning on mouse up anywhere in the document
    document.addEventListener('mouseup', () => {
        isPanning = false;
        mapBoard.style.cursor = 'grab';
    });

    document.getElementById('btn-save')?.addEventListener('click', saveTokenPositions);

    updateZoomDisplay();

});


////////////////////////////////////////////////////////////////////////////
// HELPER FUNCTIONS
////////////////////////////////////////////////////////////////////////////

// Gets the highest z-index among tokens to ensure new tokens are on top.
function getMaxZIndex() {
    const max = [...document.querySelectorAll('#map-board .draggable-token')]
        .reduce((max, t) => Math.max(max, parseInt(t.style.zIndex) || 0), 10);
    return max;
}

// Adjusts zoom level by a delta and updates the map transform and display.
function zoomMap(delta) {
    zoomLevel = Math.max(0.3, Math.min(3, zoomLevel + delta));
    updateTransform();
    updateZoomDisplay();
}

// Resets zoom and pan to default values.
function resetZoomPan() {
    zoomLevel = 1;
    panX = 0;
    panY = 0;
    updateTransform();
    updateZoomDisplay();
}

// Updates the zoom level display in the UI.
function updateZoomDisplay() {
    const display = document.getElementById('zoom-level');
    if (display) {
        display.textContent = `${zoomLevel.toFixed(1)}x`;
    }
}

// Applies the current zoom and pan to the map board.
function updateTransform() {
    const mapBoard = document.getElementById('map-board');
    if (mapBoard) {
        mapBoard.style.transform = `scale(${zoomLevel}) translate(${panX}px, ${panY}px)`;
    }
}

// Saves token positions to the server.
async function saveTokenPositions() {
    updateTokenPositions();
    try {
        const res = await fetch('/Map/SavePositions', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        if (res.ok) alert('Positions saved!');
        else throw new Error(await res.text());
    } catch (e) {
        console.error('Save failed:', e);
        alert('Save failed.');
    }
}

// updates token positions.
function updateTokenPositions() {
    const tokens = document.querySelectorAll('#map-board .draggable-token[data-tokenid]');
    data = Array.from(tokens).map(t => ({
        Id: parseInt(t.dataset.tokenid),
        X: parseFloat(t.style.left) || 0,
        Y: parseFloat(t.style.top) || 0,
        zIndex: parseInt(t.style.zIndex) || 1,
        SessionID: window.sessionId
    }));
}

// updates a single token position on the player view when moved on the DM view.
function updateSingleTokenPosition(token) {
    const existingToken = document.getElementById(`token-placed-${token.tokenid}`);
    if (existingToken) {
        existingToken.style.left = `${token.x}px`;
        existingToken.style.top = `${token.y}px`;
    } else {
        console.warn(`Token ${token.tokenid} not found on this view.`);
    }
}