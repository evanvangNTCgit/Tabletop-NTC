// https://api.jqueryui.com/draggable/#event-create
document.addEventListener("DOMContentLoaded", () => {
    console.log("Hello! \nFrom -Mr. Vang")
    $(".draggable-token").draggable({
        create: (event, ui) => {
            event.target.x
        }
    });
});