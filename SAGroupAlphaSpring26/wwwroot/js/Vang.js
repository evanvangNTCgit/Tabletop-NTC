// https://api.jqueryui.com/draggable/#event-create
document.addEventListener("DOMContentLoaded", () => {
    console.log("Hello! \nFrom -Mr. Vang")
    $(".draggable-token").draggable({
        create: (event, ui) => {
            console.log("Token Created:", Date.now());
        },
        stop: (event, ui) => {
            console.log("Broadcast a message to the player view!");
            console.log("Dropped at:", event.target.x,"X");
            console.log("Dropped at:", event.target.y,"Y");
        },
        drag: (event, ui) => {
            console.log("A piece is dragging!");
            // Put whatever functions here.
        }
    });

    // For all the tokens give the right click event.
    document.querySelectorAll('.vangtokendiv').forEach((e) => {
        e.addEventListener('contextmenu', (e) => {
            e.preventDefault();
            // console.log(e);
            // Get the source element and add my DM Toggle opacity.
            event.target.classList.toggle('dmOpacityToggle');
        })
    })
});