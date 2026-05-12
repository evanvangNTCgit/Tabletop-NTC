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

// Tracks if any changes have been made so we can ask the user if he wants to save before switching scenes.
let hasUnsavedChanges = false;

// Tracks the scene ID that the user wants to switch to, used to confirm the switch.
let pendingSceneId = null;

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
                // Use the explicitly provided sceneId (from the DM) or fallback to the current one. This allows for creating new scenes.
                if (e.data.sceneId) {
                    window.location.href = `/Map/PlayerView/${sessionId}/${e.data.sceneId}`;
                } else {
                    window.location.reload();
                }
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
    // Undo / Redo functionality
    const MAX_HISTORY = 50;
    let undoStack = [];
    let redoStack = [];

    // Updates the undo / redo buttons.
    const updateUndoRedoButtons = () => {
        const btnUndo = document.getElementById('btn-undo');
        const btnRedo = document.getElementById('btn-redo');
        // Disables the buttons if the stack is empty.
        if (btnUndo) btnUndo.disabled = undoStack.length === 0;
        if (btnRedo) btnRedo.disabled = redoStack.length === 0;
    };

    const saveStateToHistory = () => {
        undoStack.push({
            tokenData: JSON.parse(JSON.stringify(tokenData)),
            tokensToDelete: [...tokensToDelete]
        });
        if (undoStack.length > MAX_HISTORY) {
            undoStack.shift();
        }
        redoStack = [];
        updateUndoRedoButtons();
    };

    // Restores the state of the board.
    const applyState = (state) => {
        tokenData = JSON.parse(JSON.stringify(state.tokenData));
        tokensToDelete = [...state.tokensToDelete];

        // Removes all tokens from the board.
        $('.draggable-token').remove();

        for (const key in tokenData) {
            const data = tokenData[key];
            const tokenImg = document.createElement('img');
            tokenImg.id = key;
            tokenImg.src = data.src;

            // Re-creates the tokens on the board.
            tokenImg.classList.add('draggable-token', 'ui-draggable', 'ui-draggable-handle');
            tokenImg.dataset.tokenid = data.id;
            tokenImg.dataset.pieceid = data.pieceId;
            tokenImg.dataset.name = data.name || "";
            tokenImg.dataset.notes = data.notes || "";
            tokenImg.draggable = true;

            tokenImg.style.position = 'absolute';
            tokenImg.style.left = `${data.x}%`;
            tokenImg.style.top = `${data.y}%`;
            tokenImg.style.zIndex = data.zIndex || 99;

            // Hides tokens for the player view if they are not visible.
            if (!data.isVisible) {
                tokenImg.classList.add("dmOpacityToggle");
            }

            mapBoard.appendChild(tokenImg);

            $(`#${tokenImg.id}`).draggable(draggablePieceInfo);
            attachContextMenu(tokenImg);
        }

        if (selectedTokenId && !tokenData[selectedTokenId]) {
            selectedTokenId = null;
            const panel = document.getElementById('token-info-panel');
            if (panel) panel.style.display = 'none';
        } else {
            updateInfoPanel();
        }

        bc.postMessage({
            action: 'syncAll',
            allTokens: tokenData
        });

        updateUndoRedoButtons();
    };

    const undo = () => {
        if (undoStack.length === 0) return;
        redoStack.push({
            tokenData: JSON.parse(JSON.stringify(tokenData)),
            tokensToDelete: [...tokensToDelete]
        });
        applyState(undoStack.pop());
    };

    const redo = () => {
        if (redoStack.length === 0) return;
        undoStack.push({
            tokenData: JSON.parse(JSON.stringify(tokenData)),
            tokensToDelete: [...tokensToDelete]
        });
        applyState(redoStack.pop());
    };

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

    // Initializes the map, adds event listeners to the tokens.
    document.addEventListener('DOMContentLoaded', () => {

        // Controls the scaling of the map and the panels around it for different screen sizes.
        const mapRow = document.querySelector('.map-row');
        if (mapBoard && mapRow) {
            const syncScale = () => {
                const size = mapBoard.offsetHeight; // Map is square, so height = width
                if (size === 0) return;

                const root = document.documentElement;
                mapRow.style.setProperty('--map-height', `${size}px`);
                mapRow.style.setProperty('--sidebar-width', `${size * 0.12}px`);
                mapRow.style.setProperty('--panel-width', `${size * 0.35}px`);
                mapRow.style.setProperty('--piece-size', `${size * 0.08}px`);
                mapRow.style.setProperty('--panel-font-size', `${Math.max(10, size * 0.02)}px`);
                mapRow.style.setProperty('--header-font-size', `${Math.max(12, size * 0.025)}px`);
                mapRow.style.setProperty('--input-padding', `${Math.max(4, size * 0.01)}px`);
            };

            const ro = new ResizeObserver(syncScale);
            ro.observe(mapBoard);
            syncScale();

            // Also sync on window resize.
            window.addEventListener('resize', syncScale);
        }

        // --- Dropdown Menu Logic ---
        const sceneTrigger = document.getElementById('scene-trigger');
        const sceneMenu = document.getElementById('scene-menu');

        const sceneItems = document.querySelector('#scene-menu')?.querySelectorAll('.scene-item');

        const mapTrigger = document.getElementById('map-trigger');
        const mapMenu = document.getElementById('map-menu');
        const mapItems = document.querySelectorAll('.map-selection-item');

        const toolsTrigger = document.getElementById('tools-trigger');
        const toolsMenu = document.getElementById('tools-menu');
        const toolItems = document.querySelectorAll('.tool-item');

        const switchOverlay = document.getElementById('switch-confirm-overlay');

        // Toggle scene dropdown
        sceneTrigger?.addEventListener('click', (e) => {
            e.stopPropagation();
            if (mapMenu) mapMenu.style.display = 'none';
            if (toolsMenu) toolsMenu.style.display = 'none';
            const isOpen = sceneMenu.style.display === 'flex';
            sceneMenu.style.display = isOpen ? 'none' : 'flex';
        });

        // Toggle map dropdown
        mapTrigger?.addEventListener('click', (e) => {
            e.stopPropagation();
            if (sceneMenu) sceneMenu.style.display = 'none';
            if (toolsMenu) toolsMenu.style.display = 'none';
            const isOpen = mapMenu.style.display === 'flex';
            mapMenu.style.display = isOpen ? 'none' : 'flex';
        });

        // Toggle tools dropdown
        toolsTrigger?.addEventListener('click', (e) => {
            e.stopPropagation();
            if (sceneMenu) sceneMenu.style.display = 'none';
            if (mapMenu) mapMenu.style.display = 'none';
            const isOpen = toolsMenu.style.display === 'flex';
            toolsMenu.style.display = isOpen ? 'none' : 'flex';
        });

        // Close dropdowns when clicking outside
        document.addEventListener('click', () => {
            if (sceneMenu) sceneMenu.style.display = 'none';
            if (mapMenu) mapMenu.style.display = 'none';
            if (toolsMenu) toolsMenu.style.display = 'none';
        });

        // Prevent menu from closing when clicking inside
        sceneMenu?.addEventListener('click', (e) => e.stopPropagation());
        mapMenu?.addEventListener('click', (e) => e.stopPropagation());
        toolsMenu?.addEventListener('click', (e) => e.stopPropagation());

        // Handle Tool selection - Consolidated and bulletproofed
        toolItems.forEach(item => {
            item.addEventListener('click', (e) => {
                e.preventDefault();
                e.stopPropagation();

                const id = item.id;
                if (id === 'tool-token-info') {
                    // Panel is now permanent, was toggle before.
                } else if (id === 'tool-session-notes') {
                    const sessionPanel = document.getElementById('session-notes-panel');
                    if (sessionPanel) sessionPanel.style.display = (sessionPanel.style.display === 'flex') ? 'none' : 'flex';
                } else if (id === 'tool-delete-area') {
                    const deleteArea = document.getElementById('delete-area');
                    if (deleteArea) deleteArea.style.display = (deleteArea.style.display === 'block') ? 'none' : 'block';
                }

                if (toolsMenu) toolsMenu.style.display = 'none';
            });
        });

        // Handle map selection
        mapItems.forEach(item => {
            item.addEventListener('click', () => {
                const pieceId = item.getAttribute('data-id');
                if (!pieceId) return;

                // Call the map background update logic
                fetch('/Map/UpdateMapBackground', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        SessionId: sessionId,
                        PieceId: pieceId,
                        SceneId: window.currentSceneId
                    })
                })
                    .then(response => response.json())
                    .then(data => {
                        const mapImg = document.getElementById('map-bg-image');
                        if (mapImg && data.imagePath) {
                            mapImg.src = data.imagePath;
                            // Broadcast the change to other players
                            const bc = new BroadcastChannel('map_channel');
                            bc.postMessage({ action: 'reload', sceneId: window.currentSceneId });
                        }
                    })
                    .catch(error => console.error('Error updating map background:', error));

                mapMenu.style.display = 'none';
            });
        });

        // Handle scene item selection
        sceneItems.forEach(item => {
            item.addEventListener('click', () => {
                const targetSceneId = item.getAttribute('data-id');
                const currentSceneId = window.currentSceneId;

                if (targetSceneId == currentSceneId) {
                    sceneMenu.style.display = 'none';
                    return;
                }

                const newUrl = window.location.origin + `/Map/MapTest/${sessionId}/${targetSceneId}`;

                if (hasUnsavedChanges) {
                    pendingSceneId = targetSceneId;
                    if (switchOverlay) switchOverlay.style.display = 'flex';
                } else {
                    // Tell players to move to the new scene
                    bc.postMessage({ action: 'reload', sceneId: targetSceneId });
                    window.location.href = newUrl;
                }

                sceneMenu.style.display = 'none';
            });
        });

        // Handle per-scene deletion
        document.querySelectorAll('.btn-scene-delete').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.stopPropagation(); // Don't trigger the scene switch!

                const targetId = btn.getAttribute('data-id');
                const sceneName = btn.getAttribute('title').replace('Delete ', '');
                const isActive = targetId == window.currentSceneId;

                if (confirm(`Are you sure you want to delete "${sceneName}"? This will permanently remove all tokens in this scene.`)) {
                    fetch(`/Map/DeleteScene/${targetId}`, { method: 'POST' })
                        .then(res => {
                            if (res.ok) {
                                if (isActive) {
                                    // If we deleted the scene we are on, go back to the first session.
                                    window.location.href = `/Map/MapTest/${sessionId}`;
                                } else {
                                    // Otherwise just reload the window/page.
                                    window.location.reload();
                                }
                            } else {
                                alert("Failed to delete scene.");
                            }
                        })
                        .catch(err => console.error("Error deleting scene:", err));
                }

                sceneMenu.style.display = 'none';
            });
        });

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

        // makes the tokens draggable.
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

                // Saves the state to history for undo/redo functions.
                saveStateToHistory();

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
                hasUnsavedChanges = true;
            }
        });

        // sets area where tokens can be deleted by dragging and dropping them into.
        $('#delete-area').droppable({
            accept: '.draggable-token',
            over: function () { $(this).addClass('delete-hover'); },
            out: function () { $(this).removeClass('delete-hover'); },
            drop: function (event, ui) {

                // Saves the state to history for undo/redo functions.
                saveStateToHistory();

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
                hasUnsavedChanges = true;
            }
        });

        // Attach event for each .draggable-token.
        document.querySelectorAll('.vangtokendiv, .draggable-token').forEach(attachContextMenu);

        // Handles the undo / redo buttons.
        document.getElementById('btn-undo')?.addEventListener('click', undo);
        document.getElementById('btn-redo')?.addEventListener('click', redo);

        // Attaching the tokenData and Session ID to the Save button so we can save to our database when it is clicked.
        document.getElementById('btn-save')?.addEventListener('click', () => {
            saveTokenPositions(tokenData, sessionId, tokensToDelete).then(() => {
                hasUnsavedChanges = false;
            });
        });

        // Clear Board functionality
        document.getElementById('btn-clear-board')?.addEventListener('click', () => {
            if (confirm("Are you sure you want to clear all tokens from the board? This will be saved when you click the Save button.")) {

                // Saves the state to history for undo/redo functions.
                saveStateToHistory();

                // Stage all tokens for deletion
                for (const htmlId in tokenData) {
                    const dbId = tokenData[htmlId].id;
                    if (dbId && !String(htmlId).startsWith('temp-')) {
                        tokensToDelete.push(dbId);
                    }
                }

                // Clear DOM tokens
                $('.draggable-token').remove();

                // Clear local state
                tokenData = {};
                selectedTokenId = null;
                const panel = document.getElementById('token-info-panel');
                if (panel) panel.style.display = 'none';

                // Broadcast clear to player view by sending an empty syncAll
                bc.postMessage({
                    action: 'syncAll',
                    allTokens: {}
                });
                hasUnsavedChanges = true;
            }
        });

        // Change Map functionality
        document.getElementById('map-select')?.addEventListener('change', (e) => {
            const pieceId = e.target.value;
            // Checks if the piece ID is valid.
            if (!pieceId) return;

            // Calls dataservice to update the map background.
            fetch('/Map/UpdateMapBackground', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ SessionId: sessionId, PieceId: parseInt(pieceId), SceneId: window.currentSceneId })
            })
                .then(response => response.json())
                .then(data => {
                    // Checks if the response data is valid.
                    if (data && data.imagePath) {
                        // Updates the map background in the UI.
                        const bgImage = document.querySelector('.vang-map-bg-image');
                        if (bgImage) bgImage.src = data.imagePath;

                        // Tell player view to reload for the new map background.
                        bc.postMessage({ action: 'reload' });
                    } else {
                        alert("Failed to update map background.");
                    }
                    // Resets the dropdown.
                    e.target.value = "";
                })
                .catch(error => {
                    console.error("Error updating map background:", error);
                    alert("Error updating map background.");
                    // Resets the dropdown.
                    e.target.value = "";
                });
        });
    });

    // Requires us to pass in the tokenData
    const makeNewToken = (data) => {
        const tokenImg = document.createElement('img');
        tokenImg.id = data.id;
        tokenImg.src = data.src;

        tokenImg.classList.add('draggable-token');
        tokenImg.classList.add('ui-draggable');
        tokenImg.classList.add('ui-draggable-handle');
        // tokenImg.classList.add('map-piece'); // Removed to prevent overriding the 5% width constraint on board tokens
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

            // Saves the state to history if the token was moved.
            const oldX = tokenData[event.target.id].x;
            const oldY = tokenData[event.target.id].y;
            if (oldX !== leftPerc || oldY !== topPerc) {
                saveStateToHistory();
            }

            event.target.style.top = `${topPerc}%`;
            event.target.style.left = `${leftPerc}%`;

            // When drag stops, update the local tokenData.
            tokenData[event.target.id].y = topPerc;
            tokenData[event.target.id].x = leftPerc;
            hasUnsavedChanges = true;
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

            // Saves the state to history for undo/redo functions.
            saveStateToHistory();
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
            hasUnsavedChanges = true;
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

        // Saves the state to history for undo/redo functions.
        saveStateToHistory();

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

    // Clears the InfoPanel and displays default values/placeholder text and image.
    const clearInfoPanel = () => {
        const img = document.getElementById('token-info-image');
        if (img) img.src = "/images/default.png";

        document.getElementById('token-info-name').value = "";
        document.getElementById('token-info-zindex').value = "";
        document.getElementById('token-info-notes').value = "";
        document.getElementById('token-info-visibility').checked = true;
    };

    // Updates the InfoPanel based on currently selected token.
    const updateInfoPanel = () => {
        const panel = document.getElementById('token-info-panel');
        if (!panel) return;

        if (!selectedTokenId) {
            clearInfoPanel();
            return;
        }

        const data = tokenData[selectedTokenId];
        if (!data) {
            clearInfoPanel();
            return;
        }

        document.getElementById('token-info-image').src = data.src;
        document.getElementById('token-info-name').value = data.name || "";
        document.getElementById('token-info-zindex').value = data.zIndex || 1;
        document.getElementById('token-info-notes').value = data.notes || "";
        document.getElementById('token-info-visibility').checked = data.isVisible;
    };

    // Bind panel events
    document.addEventListener('DOMContentLoaded', () => {
        // Saves the state to history for undo/redo functions. (for input fields)
        document.getElementById('token-info-name')?.addEventListener('focus', (e) => {
            if (selectedTokenId && tokenData[selectedTokenId]) {
                saveStateToHistory();
            }
        });

        // Update the token data when the name is changed.
        document.getElementById('token-info-name')?.addEventListener('input', (e) => {
            if (selectedTokenId && tokenData[selectedTokenId]) {
                tokenData[selectedTokenId].name = e.target.value;
                const tokenEl = document.getElementById(selectedTokenId);
                if (tokenEl) tokenEl.dataset.name = e.target.value;
                hasUnsavedChanges = true;
            }
        });

        // Saves the state to history for undo/redo functions. (for input fields)
        document.getElementById('token-info-notes')?.addEventListener('focus', (e) => {
            if (selectedTokenId && tokenData[selectedTokenId]) {
                saveStateToHistory();
            }
        });

        document.getElementById('token-info-notes')?.addEventListener('input', (e) => {
            if (selectedTokenId && tokenData[selectedTokenId]) {
                tokenData[selectedTokenId].notes = e.target.value;
                const tokenEl = document.getElementById(selectedTokenId);
                if (tokenEl) tokenEl.dataset.notes = e.target.value;
                hasUnsavedChanges = true;
            }
        });

        document.getElementById('token-info-zindex')?.addEventListener('change', (e) => {
            if (selectedTokenId && tokenData[selectedTokenId]) {
                // Saves the state to history for undo/redo functions.
                saveStateToHistory();

                let newZ = Math.max(0, parseInt(e.target.value) || 0);

                // Ensures the z-index is at least 1.
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
                // Saves the state to history for undo/redo functions.
                saveStateToHistory();
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

                // Saves the state to history for undo/redo functions.
                saveStateToHistory();
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
                clearInfoPanel();

                // Broadcast deletion to player view.
                bc.postMessage({
                    action: 'tokenDelete',
                    tokenId: htmlId
                });
            }
        });

        // Toggles for panels are now handled in the main dropdown logic at the top of DOMContentLoaded

        // Save only session notes
        document.getElementById('btn-save-notes')?.addEventListener('click', (e) => {
            const notesTextArea = document.getElementById('session-notes-textarea');
            if (notesTextArea && sessionId) {
                const btn = e.target;
                const originalHTML = btn.innerHTML;
                btn.innerHTML = '<i class="bi bi-journal-check"></i> Saving...';
                btn.disabled = true;

                saveSessionNotes(sessionId, notesTextArea.value)
                    .done(() => {
                        btn.innerHTML = '<i class="bi bi-check-circle"></i> Saved!';
                        hasUnsavedChanges = false;
                        setTimeout(() => {
                            btn.innerHTML = originalHTML;
                            btn.disabled = false;
                        }, 2000);
                    })
                    .fail((xhr) => {
                        console.error("Save session notes error:", xhr.responseText);
                        alert('Failed to save session notes.');
                        btn.innerHTML = originalHTML;
                        btn.disabled = false;
                    });
            }
        });

        // Delete area toggle is now handled in the main dropdown logic

        // Cancel button confirmation message before leaving the page.
        document.getElementById('btn-cancel')?.addEventListener('click', (e) => {
            if (!confirm("Are you sure you wish to leave this page? Consider saving before you go!")) {
                e.preventDefault();
            }
        });

        // Dropdown used to change and manage scenes.
        const btnSwitchSave = document.getElementById('btn-switch-save');
        const btnSwitchNoSave = document.getElementById('btn-switch-nosave');
        const btnSwitchCancel = document.getElementById('btn-switch-cancel');
        const sceneSelect = document.getElementById('scene-select');

        sceneSelect?.addEventListener('change', (e) => {
            pendingSceneId = e.target.value;
            const newUrl = window.location.origin + `/Map/MapTest/${sessionId}/${pendingSceneId}`;

            if (hasUnsavedChanges) {
                // Show custom modal instead of confirm
                if (switchOverlay) switchOverlay.style.display = 'flex';
                // Reset select visually so it doesn't look changed yet
                sceneSelect.value = window.currentSceneId;
            } else {
                window.location.href = newUrl;
            }
        });

        btnSwitchCancel?.addEventListener('click', () => {
            if (switchOverlay) switchOverlay.style.display = 'none';
            pendingSceneId = null;
        });

        btnSwitchNoSave?.addEventListener('click', () => {
            if (pendingSceneId) {
                const newUrl = window.location.origin + `/Map/MapTest/${sessionId}/${pendingSceneId}`;
                // Tell players to move even though we aren't saving
                bc.postMessage({ action: 'reload', sceneId: pendingSceneId });
                window.location.href = newUrl;
            }
        });

        btnSwitchSave?.addEventListener('click', () => {
            if (pendingSceneId) {
                const newUrl = window.location.origin + `/Map/MapTest/${sessionId}/${pendingSceneId}`;
                btnSwitchSave.disabled = true;
                btnSwitchSave.innerText = "Saving & Switching...";

                // Silent Save followed by manual navigation because auto-navigation was causing problems with switching scenes.
                saveTokenPositions(tokenData, sessionId, tokensToDelete, null, null, true).then(() => {
                    hasUnsavedChanges = false;
                    window.location.href = newUrl;
                });
            }
        });

        // Button used to create a new scene (Wizard Modal).
        const wizardOverlay = document.getElementById('scene-wizard-overlay');
        const wizardCancel = document.getElementById('btn-wizard-cancel');
        const wizardCreate = document.getElementById('btn-wizard-create');

        document.getElementById('btn-add-scene')?.addEventListener('click', () => {
            if (wizardOverlay) wizardOverlay.style.display = 'flex';
        });

        // Select All Logic for Wizard
        document.getElementById('wizard-select-all')?.addEventListener('change', (e) => {
            const isChecked = e.target.checked;
            document.querySelectorAll('.token-clone-checkbox').forEach(cb => {
                cb.checked = isChecked;
            });
        });

        wizardCancel?.addEventListener('click', () => {
            if (wizardOverlay) wizardOverlay.style.display = 'none';
        });

        wizardCreate?.addEventListener('click', () => {
            const sceneName = document.getElementById('wizard-scene-name')?.value;
            const mapPieceId = document.getElementById('wizard-map-select')?.value;
            const selectedTokens = Array.from(document.querySelectorAll('.token-clone-checkbox:checked'))
                .map(cb => parseInt(cb.value));

            if (!sceneName) {
                alert("Please enter a scene name.");
                return;
            }

            wizardCreate.disabled = true;
            wizardCreate.innerText = "Saving & Creating...";

            // First, save the current state so the cloning has the latest data, because we are cloning from the Database.
            saveTokenPositions(tokenData, sessionId, tokensToDelete, null, null, true)
                .then(() => {
                    return fetch('/Map/CreateScene', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                            SessionId: sessionId,
                            Name: sceneName,
                            MapPieceId: mapPieceId ? parseInt(mapPieceId) : null,
                            TokenIdsToClone: selectedTokens
                        })
                    });
                })
                .then(res => res.json())
                .then(data => {
                    window.location.href = window.location.origin + `/Map/MapTest/${sessionId}/${data.id}`;
                })
                .catch(err => {
                    console.error("Wizard error:", err);
                    alert("Error creating scene.");
                    wizardCreate.disabled = false;
                    wizardCreate.innerText = "Create Scene";
                });
        });

        // Button used to delete the current scene.
        document.getElementById('btn-delete-scene')?.addEventListener('click', () => {
            if (confirm("Are you sure you want to delete the current scene and all its tokens?")) {
                fetch(`/Map/DeleteScene/${window.currentSceneId}`, { method: 'POST' })
                    .then(() => {
                        window.location.href = window.location.origin + `/Map/MapTest/${sessionId}`;
                    });
            }
        });
    });
}