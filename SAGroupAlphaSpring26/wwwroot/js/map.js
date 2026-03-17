/**
 * D&D Map JS - Drag, Drop, Zoom, Pan
 */

let zoomLevel = 1;
let panX = 0;
let panY = 0;
let isPanning = false;
let startX, startY;
let sessionId = 0; // Will be set from inline script

document.addEventListener('DOMContentLoaded', () => {
    const mapBoard = document.getElementById('map-board');
    if (!mapBoard) return;

    // Set sessionId from inline script
    sessionId = window.sessionId || 0;

    // Drag & Drop for sidebar pieces to map
    document.addEventListener('dragstart', (e) => {
        if (e.target.classList.contains('sidebar-piece')) {
            e.dataTransfer.effectAllowed = 'copy';
            e.dataTransfer.setData('text/plain', e.target.dataset.pieceid || 'new');
            e.target.style.opacity = '0.5';
        }
    });

    document.addEventListener('dragend', (e) => {
        if (e.target.classList.contains('map-piece')) {
            e.target.style.opacity = '1';
        }
    });

    mapBoard.addEventListener('dragover', (e) => {
        e.preventDefault();
    });

    mapBoard.addEventListener('drop', async (e) => {
        e.preventDefault();
        const data = e.dataTransfer.getData('text/plain');
        
        // If data is a token ID (existing token drag), ignore server call
        if (data.startsWith('token-placed-') || data.match(/^[0-9]+$/)) {
            return;
        }
        
        const rect = mapBoard.getBoundingClientRect();
        const x = (e.clientX - rect.left) / zoomLevel - 26;
        const y = (e.clientY - rect.top) / zoomLevel - 26;
        
        // Check if new piece from sidebar
        const sidebarPiece = document.querySelector(`[data-pieceid="${data}"]`);
        if (!sidebarPiece || !sessionId) return;

        try {
            const response = await fetch('/Map/CreateToken', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ 
                    pieceId: parseInt(data), 
                    sessionId: sessionId, 
                    X: x, 
                    Y: y 
                })
            });
            
            if (!response.ok) throw new Error(`HTTP ${response.status}`);
            
            const result = await response.json();
            if (result.id) {
                // Clone and position new token
                const draggedImg = sidebarPiece;
                if (draggedImg) {
                    const tokenElement = draggedImg.cloneNode(true);
                    tokenElement.id = `token-placed-${result.id}`;
                    tokenElement.classList.remove('sidebar-piece');
                    tokenElement.classList.add('draggable-token');
                    tokenElement.dataset.tokenid = result.id;
                    tokenElement.style.left = `${x}px`;
                    tokenElement.style.top = `${y}px`;
                    tokenElement.style.zIndex = (getMaxZIndex() + 1).toString();
                    mapBoard.appendChild(tokenElement);
                }
                console.log(`New token ${result.id} added at (${x.toFixed(0)}, ${y.toFixed(0)})`);
            }
        } catch (error) {
            console.error('Failed to create token:', error);
            alert('Failed to add token. Check console for details.');
        }
    });

    // Drag existing tokens - make them draggable
    mapBoard.addEventListener('dragstart', (e) => {
        if (e.target.classList.contains('draggable-token')) {
            e.dataTransfer.effectAllowed = 'move';
        }
    });

    mapBoard.addEventListener('dragend', (e) => {
        if (e.target.classList.contains('draggable-token')) {
            // Update position after drag
            const rect = mapBoard.getBoundingClientRect();
            const x = (e.clientX - rect.left) / zoomLevel - 26;
            const y = (e.clientY - rect.top) / zoomLevel - 26;
            e.target.style.left = `${x}px`;
            e.target.style.top = `${y}px`;
            e.target.style.zIndex = (getMaxZIndex() + 1).toString();
        }
    });

    // Make existing tokens draggable on load
    const existingTokens = mapBoard.querySelectorAll('.draggable-token');
    existingTokens.forEach(token => {
        token.draggable = true;
        token.style.cursor = 'grab';
        token.addEventListener('dragstart', () => {
            token.style.opacity = '0.5';
        });
        token.addEventListener('dragend', () => {
            token.style.opacity = '1';
        });
    });

    // Zoom controls
    const zoomInBtn = document.getElementById('zoom-in');
    const zoomOutBtn = document.getElementById('zoom-out');
    const zoomResetBtn = document.getElementById('zoom-reset');
    const zoomLevelSpan = document.getElementById('zoom-level');

    if (zoomInBtn) zoomInBtn.addEventListener('click', () => zoomMap(0.1));
    if (zoomOutBtn) zoomOutBtn.addEventListener('click', () => zoomMap(-0.1));
    if (zoomResetBtn) zoomResetBtn.addEventListener('click', resetZoomPan);

    // Mouse wheel zoom
    mapBoard.addEventListener('wheel', (e) => {
        e.preventDefault();
        const delta = -(e.deltaY * 0.001); // Natural scroll direction
        zoomMap(delta);
        updateZoomDisplay();
    }, { passive: false });

    // Update zoom display
    updateZoomDisplay();

    // Pan with middle mouse
    mapBoard.addEventListener('mousedown', (e) => {
        if (e.button === 1) { // middle button
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
        if (mapBoard) mapBoard.style.cursor = 'grab';
    });

    // Save positions
    const saveBtn = document.getElementById('btn-save');
    if (saveBtn) {
        saveBtn.addEventListener('click', saveTokenPositions);
    }
});

function getMaxZIndex() {
    const tokens = document.querySelectorAll('#map-board .draggable-token');
    return Math.max(...Array.from(tokens).map(t => parseInt(t.style.zIndex) || 0), 10);
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
    const zoomSpan = document.getElementById('zoom-level');
    if (zoomSpan) {
        zoomSpan.textContent = `${zoomLevel.toFixed(1)}x`;
    }
}

function updateTransform() {
    const mapBoard = document.getElementById('map-board');
    if (mapBoard) {
        mapBoard.style.setProperty('--zoom', zoomLevel);
        mapBoard.style.setProperty('--pan-x', `${panX}px`);
        mapBoard.style.setProperty('--pan-y', `${panY}px`);
    }
}

async function saveTokenPositions() {
    const mapBoard = document.getElementById('map-board');
    const tokens = mapBoard.querySelectorAll('.draggable-token[data-tokenid]');
    const tokenData = Array.from(tokens).map(t => ({
        Id: parseInt(t.dataset.tokenid),
        X: parseFloat(t.style.left) || 0,
        Y: parseFloat(t.style.top) || 0,
        zIndex: parseInt(t.style.zIndex) || 1,
        SessionID: sessionId
    })).filter(d => d.Id > 0);

    try {
        const response = await fetch('/Map/SavePositions', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(tokenData)
        });
        if (response.ok) {
            alert('Positions saved!');
        } else {
            throw new Error('Save failed');
        }
    } catch (error) {
        console.error('Save error:', error);
        alert('Failed to save positions.');
    }
}

