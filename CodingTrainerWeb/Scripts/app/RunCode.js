﻿$(function () {
    // Default data
    $('#code').val(model.DefaultCode);

    // Set up and maintain the connection
    $.connection.hub.start().done(connected);
    $.connection.hub.disconnected(disconnected);

    // Set up the hub client callbacks
    var hub = $.connection.codeRunnerHub;
    hub.client.consoleOut = consoleOut;
    hub.client.complete = complete;

    // Set up JQuery event handlers
    $('#run').click(run);
    $('#console').click(consoleIn);




    // Code for starting/maintaining SignalR connection

    function connected() {
        // Allow users to submit code
        $('#run').prop('disabled', false).text('Run Code');
    }

    function disconnected() {
        // Attempt to reconnect
        $.connection.hub.start();
    }



    // Code for handling SignalR events

    function consoleOut(message) {
        $('#console-out').append(document.createTextNode(message));
    }

    function complete() {
        $('#run').prop('disabled', false);
    }


    // Code for handling window events

    function run() {
        $('#run').prop('disabled', true);
        $('#console-out').text('');
        try {
            hub.server.run($('#code').val());
        } catch (e) {
            // In case it's been disconnected
            $.connection.hub.start().done(function () {
                hub.server.run($('#code').val());
            });
        }
    }

    function consoleIn() {
        hub.server.consoleIn($('#console-in').val());
    }
});
