/**
 * D&D Map JS - Fixed Drag, Drop, Zoom, Pan for Pieces/Tokens
 */

const sessionId = window.sessionId; // Set by Razor, stores session ID.

let isDev = true; // Set to false for player view.
let zoomLevel = 1;
let panX = 0;
let panY = 0;
let isPanning = false;
let startX, startY;

let data = []; // For saving token positions


const bc = new BroadcastChannel('map_channel');

bc.onmessage = (event) => {
    console.log("Test");
    console.table(event.data);
};

bc.postMessage("Hello world!");

document.addEventListener('DOMContentLoaded', () => {
    const mapBoard = document.getElementById('map-board');
    const sessionId = window.sessionId;

    if (!mapBoard) {
        console.error('Map board not found!');
        return;
    }
    if (!sessionId) {
        console.error('No session ID!');
        return;
    }

    console.log('Map initialized:', { sessionId });

    // Drag start from sidebar pieces (add new token)
    document.addEventListener('dragstart', (e) => {
        if (e.target.classList.contains('sidebar-piece')) {
            const pieceId = e.target.dataset.pieceid;
            if (pieceId) {
                e.dataTransfer.effectAllowed = 'copy';
                e.dataTransfer.setData('text/plain', pieceId);
                e.dataTransfer.setDragImage(e.target, 26, 26); // Custom ghost
                console.log('Dragging sidebar piece:', pieceId);
            }
        } else if (e.target.classList.contains('draggable-token')) {
            e.dataTransfer.effectAllowed = 'move';
            e.dataTransfer.setData('text/plain', `token-placed-${e.target.dataset.tokenid}`);
            e.dataTransfer.setDragImage(e.target, 26, 26); // Custom ghost
            console.log('Dragging existing token');
        }
    });

    // Drag end (cleanup)
    document.addEventListener('dragend', (e) => {
        if (e.target.classList.contains('map-piece')) {
            e.target.style.opacity = '1';
        }
    });

    // Board dragover
    mapBoard.addEventListener('dragover', (e) => {
        e.preventDefault();
        mapBoard.classList.add('drag-over');
        e.dataTransfer.dropEffect = 'all';
    });

    mapBoard.addEventListener('dragleave', (e) => {
        mapBoard.classList.remove('drag-over');
    });

    // Drop on board (add new OR move existing)
    mapBoard.addEventListener('drop', async (e) => {
        e.preventDefault();
        const data = e.dataTransfer.getData('text/plain');
        const rect = mapBoard.getBoundingClientRect();
        const x = (e.clientX - rect.left) / zoomLevel - 26;
        const y = (e.clientY - rect.top) / zoomLevel - 26;

        console.log('Drop:', { data, x: x.toFixed(0), y: y.toFixed(0), sessionId });

        bc.postMessage("A piece was dropped!");

        // Skip if dragging existing token (handled in dragend)
        if (data.startsWith('token-placed-')) {
            console.log('Moving existing token (visual only)');
            return;
        }

        // New piece from sidebar
        const pieceId = parseInt(data);
        if (!isNaN(pieceId)) {
            try {
                const response = await fetch('/Map/CreateToken', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ pieceId, sessionId, X: x, Y: y })
                });

                if (!response.ok) {
                    throw new Error(`HTTP ${response.status}`);
                }

                const result = await response.json();
                console.log('Token created:', result);

                if (result.id) {
                    // Clone sidebar img as new token
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
                        makeDraggable(tokenImg); // Enable drag immediately
                    }
                }
            } catch (error) {
                console.error('Create token failed:', error);
                alert('Failed to add piece. Check console.');
            }
        }
    });

    // Make single token draggable
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
        });
    }

    // Init existing
    document.querySelectorAll('.draggable-token').forEach(makeDraggable);

    // Zoom controls
    document.getElementById('zoom-in')?.addEventListener('click', () => zoomMap(0.1));
    document.getElementById('zoom-out')?.addEventListener('click', () => zoomMap(-0.1));
    document.getElementById('zoom-reset')?.addEventListener('click', resetZoomPan);

    // Wheel zoom
    mapBoard.addEventListener('wheel', (e) => {
        e.preventDefault();
        zoomMap(e.deltaY * -0.001);
    }, { passive: false });

    // Pan (middle mouse)
    mapBoard.addEventListener('mousedown', (e) => {
        if (e.button === 1) {
            isPanning = true;
            startX = e.clientX - panX;
            startY = e.clientY - panY;
            mapBoard.style.cursor = 'grabbing';
            e.preventDefault();
        }
    });
    document.addEventListener('mousemove', (e) => {
        if (isPanning) {
            panX = e.clientX - startX;
            panY = e.clientY - startY;
            updateTransform();
        }
    });
    document.addEventListener('mouseup', () => {
        isPanning = false;
        mapBoard.style.cursor = 'grab';
    });

    // Save button
    document.getElementById('btn-save')?.addEventListener('click', saveTokenPositions);

    updateZoomDisplay();
});

function getMaxZIndex() {
    const max = [...document.querySelectorAll('#map-board .draggable-token')]
        .reduce((max, t) => Math.max(max, parseInt(t.style.zIndex) || 0), 10);
    return max;
}

function zoomMap(delta) {
    zoomLevel = Math.max(0.3, Math.min(3, zoomLevel + delta));
    updateTransform();
    updateZoomDisplay();
}

function resetZoomPan() {
    zoomLevel = 1;
    panX = 0;
    panY = 0;
    updateTransform();
    updateZoomDisplay();
}

function updateZoomDisplay() {
    document.getElementById('zoom-level').textContent = `${zoomLevel.toFixed(1)}x`;
}

function updateTransform() {
    const mapBoard = document.getElementById('map-board');
    mapBoard.style.transform = `scale(${zoomLevel}) translate(${panX}px, ${panY}px)`;
}

// Calls the update token positions and then posts Token Positions.
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

// Local Save Token Positions.
async function updateTokenPositions() {

    tokenPositions = document.querySelectorAll('#map-board .draggable-token[data-tokenid]');


    data = Array.from(tokens).map(t => ({
        Id: parseInt(t.dataset.tokenid),
        X: parseFloat(t.style.left) || 0,
        Y: parseFloat(t.style.top) || 0,
        zIndex: parseInt(t.style.zIndex) || 1,
        SessionID: window.sessionId
    }));
}

