// The data these functions take...
/**
 *             bc.postMessage({
                tokenId: `${tokenId}`,
                tokenImgSrc: `${imgSrc}`,
                tokenX: X,
                tokenY: Y,
                action: 'tokenMove',
            });
 */

/**
 * Takes the broadcast data and positions tokens on player map accordingly.
 * @param {any} broadCastData the data received from broadcast channel post.
 */
export const repositionToken = (broadCastData) => {
    try {
        console.log(broadCastData);
        const tokenGettingPositioned = document.getElementById(broadCastData.tokenId);
        tokenGettingPositioned.style.left = `${broadCastData.tokenX}px`;
        tokenGettingPositioned.style.top = `${broadCastData.tokenY}px`;
    }
    catch {
        // Sometimes if you drag off screen it shows a style error.
        // So just do nothing in the catch.
    }
}

export const toggleTokenInvisibility = (broadCastData) => {
    console.log(broadCastData);
    const tokenGettingToggled = document.getElementById(broadCastData.tokenId);
    tokenGettingToggled.classList.toggle('hidden');
}