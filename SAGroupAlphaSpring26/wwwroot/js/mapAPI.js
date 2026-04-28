// MapApi.js

/**
 * Converts the local tokenData dictionary into an array and saves to the C# Backend/calls the map controller.
 * @param {Object} tokenData - The local state dictionary
 * @param {number} sessionId - The current active session ID
 */
export const saveTokenPositions = (tokenData, sessionId, tokensToDelete) => {

    // store save button for disabling and enabling.
    const saveBtn = document.getElementById('btn-save');

    if (saveBtn) {
        if (saveBtn.disabled) return;
        saveBtn.disabled = true;
        saveBtn.innerText = "Saving...";
    }

    // create deletePromise, to ensure we delete tokens before the page reloads. 
    // We need this so the playerview doesn't end up showing deleted tokens, as the playerview updates after the DM view saves.
    let deletePromise = $.Deferred().resolve();

    // Process the list of tokens to delete first, sending a request for each token ID that needs to be deleted.
    if (tokensToDelete.length > 0) {

        console.log("Deleting Tokens:", tokensToDelete);

        deletePromise = $.ajax({
            url: '/Map/DeleteTokens',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(tokensToDelete)
        });
    }

    deletePromise.then(() => {
        const updates = Object.values(tokenData).map(token => ({
            Id: token.id ? token.id.toString() : "",
            PieceId: parseInt(token.pieceId) || 0,
            SessionID: parseInt(sessionId) || 0,
            X: parseFloat(token.x) || 0,
            Y: parseFloat(token.y) || 0,
            zIndex: parseInt(token.zIndex) || 1,
            Visibility: !!token.isVisible,
            Name: token.name || "",
            Notes: token.notes || ""
        }));

        console.log("Sending token updates to server:", updates);

        return $.ajax({
            url: '/Map/SavePositions',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(updates)
        });
    }).then(() => {
        // Also save the session notes
        const notesTextArea = document.getElementById('session-notes-textarea');
        if (notesTextArea && sessionId) {
            return saveSessionNotes(sessionId, notesTextArea.value);
        }
        return $.Deferred().resolve();
    }).then(() => {
        const bc = new BroadcastChannel('map_channel');
        bc.postMessage({ action: 'reload' });
        console.log("Positions saved and reload message sent.");
        window.location.reload();
    }).fail((xhr) => {
        console.error("Save/Delete error:", xhr.responseText);
        alert('Critical Error during save. Check console.');
        if (saveBtn) {
            saveBtn.disabled = false;
            saveBtn.innerText = "Save Positions";
        }
    });
};

/**
 * Saves just the session notes to the C# Backend.
 * @param {number} sessionId - The current active session ID
 * @param {string} notes - The session notes string
 */
export const saveSessionNotes = (sessionId, notes) => {
    return $.ajax({
        url: '/Map/SaveSessionNotes',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            SessionId: parseInt(sessionId),
            Notes: notes
        })
    });
};