// MapApi.js

/**
 * Converts the local tokenData dictionary into an array and saves to the C# Backend/calls the map controller.
 * @param {Object} tokenData - The local state dictionary
 * @param {number} sessionId - The current active session ID
 */
export const saveTokenPositions = (tokenData, sessionId, tokensToDelete, redirectUrl = null, broadcastSceneId = null, isSilent = false) => {

    // store save button for disabling and enabling.
    const saveBtn = document.getElementById('btn-save');

    // added promise for thread safety, was having issues with scene switching, token cloning and saves.
    if (saveBtn) {
        if (saveBtn.disabled) return $.Deferred().resolve().promise();
        saveBtn.disabled = true;
        saveBtn.innerHTML = '<i class="bi bi-save"></i> Saving...';
    }

    console.log("MapAPI: saveTokenPositions called. Tokens to delete:", tokensToDelete.length);

    // create deletePromise, to ensure we delete tokens before the page reloads. 
    // We need this so the playerview doesn't end up showing deleted tokens, as the playerview updates after the DM view saves.
    let deletePromise = $.Deferred().resolve().promise();

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

    // return the promise chain so we can wait for it if needed. (Like if we are waiting for a save before we switch scenes)
    return deletePromise.then(() => {
        const updates = Object.values(tokenData).map(token => ({
            Id: token.id ? token.id.toString() : "",
            PieceId: parseInt(token.pieceId) || 0,
            SessionID: parseInt(sessionId) || 0,
            SceneId: window.currentSceneId ? parseInt(window.currentSceneId) : null,
            X: parseFloat(token.x) || 0,
            Y: parseFloat(token.y) || 0,
            zIndex: parseInt(token.zIndex) || 1,
            Visibility: !!token.isVisible,
            Name: token.name != null ? String(token.name) : "",
            Notes: token.notes != null ? String(token.notes) : ""
        }));

        if (updates.length === 0) {
            return $.Deferred().resolve().promise();
        }

        return $.ajax({
            url: '/Map/SavePositions',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(updates)
        });
    }).then(() => {
        const notesTextArea = document.getElementById('session-notes-textarea');
        if (notesTextArea && sessionId) {
            return saveSessionNotes(sessionId, notesTextArea.value).catch(err => {
                console.warn("MapAPI: Notes failed to save, but proceeding.", err);
                return true;
            });
        }
        return $.Deferred().resolve().promise();
    }).then(() => {
        const bc = new BroadcastChannel('map_channel');
        const sceneToBroadcast = broadcastSceneId || window.currentSceneId;
        bc.postMessage({ action: 'reload', sceneId: sceneToBroadcast });

        if (redirectUrl && !isSilent) {
            window.location.href = redirectUrl;
        } else if (!isSilent) {
            window.location.reload();
        }

        return true;
    }).fail((xhr) => {
        console.error("Save/Delete error:", xhr.responseText);
        alert('Critical Error during save. Check console.');
        if (saveBtn) {
            saveBtn.disabled = false;
            saveBtn.innerHTML = '<i class="bi bi-save"></i> Save Changes';
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