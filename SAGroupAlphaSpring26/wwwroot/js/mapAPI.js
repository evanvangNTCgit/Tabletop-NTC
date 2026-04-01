// MapApi.js

/**
 * Converts the local tokenData dictionary into an array and saves to the C# Backend/calls the map controller.
 * @param {Object} tokenData - The local state dictionary
 * @param {number} sessionId - The current active session ID
 */
export const saveTokenPositions = (tokenData, sessionId) => {
    // Converts the tokenData diction into an array to match model.
    const updates = Object.values(tokenData).map(token => ({
        Id: token.id.toString(),
        PieceId: parseInt(token.pieceId),
        SessionID: parseInt(sessionId),
        X: parseFloat(token.x),
        Y: parseFloat(token.y),
        zIndex: 99,
        Visibility: token.isVisible
    }));

    if (updates.length === 0) {
        alert('No tokens to save.');
        return;
    }

    // THE AJAX CALL
    $.ajax({
        url: '/Map/SavePositions',
        type: 'POST',
        contentType: 'application/json', // Tells C# to use [FromBody]
        data: JSON.stringify(updates),   // Sends the array
        success: function (response) {
            alert('Positions saved successfully!');
            window.location.reload();    // Reloads to show the new saved spots
        },
        error: function (xhr, status, error) {
            console.error("Status: " + status);
            console.error("Error: " + xhr.responseText);
            alert('Save failed: ' + xhr.responseText);
        }
    });
};