/**
 * Takes the broadcast data and positions tokens on player map accordingly.
 * @param {any} broadCastData the data received from broadcast channel post.
 */
export const repositionToken = (broadCastData) => {
    const token = document.getElementById(broadCastData.tokenId);
    if (token) {
        // Force percentage-based positioning
        token.style.left = `${broadCastData.tokenLeftPerc}%`;
        token.style.top = `${broadCastData.tokenTopPerc}%`;

        // Ensure width/height stays consistent relative to the map
        token.style.width = '5%';
        token.style.height = 'auto';
    } else {
        console.log("that token not found.");
    }
};

export const toggleTokenInvisibility = (broadCastData) => {
    console.log(broadCastData);
    const tokenGettingToggled = document.getElementById(broadCastData.tokenId);
    tokenGettingToggled.classList.toggle('hidden');
}