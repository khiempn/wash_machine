
function showMessage(msg) {
    $.gritter.add({
        // (string | mandatory) the heading of the notification
        title: 'Message',
        // (string | mandatory) the text inside the notification
        text: msg,
        class_name: 'gritter-light ', time: '3000',

    });
    return false;
}
function showTracking(msg) {
    alertA(msg);
}
function alertA(message) {
    var frame = document.getElementById('alert_frame');
    if (frame === null) {
        $(document.body).append('<div id="alert_frame" class="opacity-0_"></div>');
        frame = document.getElementById('alert_frame');
    }
    var id = Math.floor(Math.random() * 99999999);
    $(frame).append('<p class="alert-item" id="' + id + '"><span>' + message + '</span></p>');
    setTimeout(function () { $("#" + id).fadeOut(1000); }, 3000);
}