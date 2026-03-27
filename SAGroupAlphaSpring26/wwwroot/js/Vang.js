// https://api.jqueryui.com/draggable/#event-create
document.addEventListener("DOMContentLoaded", () => {
    console.log("Hello! \nFrom -Mr. Vang")
    // Dont be afraid of the dollar sign
    // Just grabs all the .draggable-token DOMS like a queryselectorall
    $(".draggable-token").draggable({
        containment: "#map-board",
        scroll: false,
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

    // For every side-piece
    // My take for simplicity is to just give the user a popup
    // That they can select yes or no on.
    // It will make the piece on the map set it at a default 0x 0y and invisible to the player.
    // https://jqueryui.com/dialog/
    document.querySelectorAll(".sidebar-piece").forEach((e) => {
        e.addEventListener('click', (e) => {
            console.log(e);
            $("#dialog").dialog({
                // This will prevent the user from interacting with background (map) when popup shows.
                modal: true,
                // Adding user button choices to the popup...
                buttons: [
                    {
                        text: "Yes",
                        click: () => {
                            // Close the dialog.
                            $("#dialog").dialog("close");
                        }
                    },
                    {
                        text: "No",
                        click: () => {
                            // Close the dialog.
                            $("#dialog").dialog("close");
                        }
                    }
                ]
            });
            document.getElementById("dialog").classList.remove("hidden")
            console.log("Should show dialog...");
        })
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