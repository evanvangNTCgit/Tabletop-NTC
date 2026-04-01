// MapApi.js

/**
 * Converts the local tokenData dictionary into an array and saves to the C# Backend/calls the map controller.
 * @param {Object} tokenData - The local state dictionary
 * @param {number} sessionId - The current active session ID
 */
export const saveTokenPositions = (tokenData, sessionId) => {

    const saveBtn = document.getElementById('btn-save');

    if (saveBtn) {
        if (saveBtn.disabled) return;
        saveBtn.disabled = true;
        saveBtn.innerText = "Saving...";
    }

    // Converts the tokenData diction into an array to match model.
    const updates = Object.values(tokenData).map(token => ({
        Id: token.id ? token.id.toString() : "", // failsafe for new tokens missing an id.
        PieceId: parseInt(token.pieceId) || 0, // failsafe for new tokens missing a pieceId, applied to the below aswell.
        SessionID: parseInt(sessionId) || 0,
        X: parseFloat(token.x) || 0,
        Y: parseFloat(token.y) || 0,
        zIndex: token.zIndex,
        Visibility: token.isVisible,
    }));

    if (updates.length === 0) {
        alert('No tokens to save.');

        // since it didn't save, re-enable the button.
        if (saveBtn) {
            saveBtn.disabled = false;
            saveBtn.innerText = "Save";
        }

        return;
    }

    // THE AJAX CALL, instead of using the old fetch API. This is built into jQuery.
    $.ajax({
        url: '/Map/SavePositions',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(updates),
        success: function (response) {
            // need a broadcast channel because the map.js bc is out of scope...
            const bc = new BroadcastChannel('map_channel');
            // Was duplicating new tokens on save... So now we reload the page (and the player page) so that the temperary token ids get replaced with new real ids by the backend.
            alert('Positions saved successfully, reloading page!');
            window.location.reload();
            bc.postMessage({ action: 'reload' });
        },
        error: function (xhr, status, error) {
            console.error("Status: " + status);
            console.error("Error: " + xhr.responseText);
            alert('Save failed: ' + xhr.responseText);

            if (saveBtn) {
                saveBtn.disabled = false;
                saveBtn.innerText = "Save Positions";
            }
        }
    });
};