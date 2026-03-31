// MapApi.js

/**
 * Converts the local tokenData dictionary into an array and saves to the C# Backend/calls the map controller.
 * @param {Object} tokenData - The local state dictionary
 * @param {number} sessionId - The current active session ID
 */
export const saveTokenPositions = async (tokenData, sessionId) => {
    const updates = Object.values(tokenData).map(token => ({
        Id: token.id.toString(),
        PieceId: parseInt(token.pieceId),
        SessionID: sessionId,
        X: token.x,
        Y: token.y,
        zIndex: 99,
        Visibility: token.isVisible
    }));

    if (updates.length === 0) return alert('No tokens to save.');

    try {
        const res = await fetch('/Map/SavePositions', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(updates)
        });

        if (res.ok) {
            alert('Positions saved!');
            window.location.reload();
        } else {
            throw new Error(await res.text());
        }
    } catch (e) {
        console.error('Save failed:', e);
        alert('Save failed.');
    }
};