var app = {};

(function (exports) {
    // Code for starting/maintaining SignalR connection

    exports.connected = function () {
        // Allow users to submit code
        $('#run').prop('disabled', false).text('Run Code');
    };

    exports.disconnected = function () {
        // Attempt to reconnect
        $.connection.hub.start();
    };

    // Code for handling SignalR events

    exports.consoleOut = function (message) {
        $('#console-out').append(document.createTextNode(message));
    };

    exports.complete = function () {
        $('#run').prop('disabled', false);
    };

    // Code for handling window events

    exports.run = function () {
        $('#run').prop('disabled', true);
        $('#console-out').text('');
        try {
            hub.server.run($('#code').val()).fail(function (e) {
                var message = e.message;
                if (e.data) {
                    e.message += "\r\n\r\nThe error message is:\r\n    " + e.data.Message;
                }
                alert(e.message);
                complete();
            });
        } catch (e) {
            alert(e.message);
            complete();
        }
    };

    exports.consoleIn = function () {
        hub.server.consoleIn($('#console-in').val());
    };
})(app);




// Code that runs on startup:

$('#code').val(model.DefaultCode);

// Set up and maintain the connection
$.connection.hub.start().done(app.connected);
$.connection.hub.disconnected(app.disconnected);

// Set up the hub client callbacks
var hub = $.connection.codeRunnerHub;
hub.client.consoleOut = app.consoleOut;
hub.client.complete = app.complete;

// Set up JQuery event handlers
$('#run').click(app.run);
$('#console').click(app.consoleIn);