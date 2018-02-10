(function () {
    ///////////////////////////////////////////////////
    // Code for starting/maintaining SignalR connection
    ///////////////////////////////////////////////////

    // Set up and maintain the connection
    $.connection.hub.start().done(function () {
        // Once connected, allow users to submit code
        $('#run').prop('disabled', false).text('Run Code');
    });
    $.connection.hub.disconnected(function () {
        // Attempt to reconnect
        $.connection.hub.start();
    });

    ///////////////////////////////////
    // Code for handling SignalR events
    ///////////////////////////////////

    var hub = $.connection.codeRunnerHub;

    hub.client.consoleOut = function (message) {
        // Display stdout data
        $('#console-out').append(document.createTextNode(message));
    };

    hub.client.complete = function () {
        $('#run').prop('disabled', false);
    };

    //////////////////////////////////
    // Code for handling window events
    ////////////////////////////////// 

    // User clicks run button
    $('#run').click(function () {
        // Prevent user from running again
        $('#run').prop('disabled', true);
        $('#console-out').text('');
        try {
            // Send the code to the server to run
            hub.server.run(editor.getValue()).fail(function (e) {
                var message = e.message;
                if (e.data) {
                    e.message += "\r\n\r\nThe error message is:\r\n    " + e.data.Message;
                }
                alert(e.message);
                exports.complete();
            });
        } catch (e) {
            alert(e.message);
            exports.complete();
        }
    });

    // User sending data to stdin
    $('#console').click(function () {
        hub.server.consoleIn($('#console-in').val());
    });

    ////////////////////
    // Set up CodeMirror
    ////////////////////

    var editor = CodeMirror.fromTextArea(document.getElementById("code"), {
        lineNumbers: true,
        matchBrackets: true,
        mode: "text/x-csharp"
    });
    editor.setSize(null, '35em');
})();
