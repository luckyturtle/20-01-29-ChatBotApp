//https://stackoverflow.com/questions/1787322/htmlspecialchars-equivalent-in-javascript
function escapeHtml(text) {
    var map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };

    return text.replace(/[&<>"']/g, function (m) { return map[m]; });
}


$(document).ready(function () {

    window.drag = Draggable.create('.draggable', {
        dragClickables: true,
        onDragStart: function () {
            window.draginfo = {
                x: this.pointerX,
                y: this.pointerY
            }
        },

    })[0];
});
