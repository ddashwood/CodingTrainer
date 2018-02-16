﻿(function () {
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
        //$('#console-out').append(document.createTextNode(message));

        console.append(message);
    };

    var complete = function () {
        $('#run').prop('disabled', false);
    };

    hub.client.complete = complete;

    hub.client.compilerError = function (details) {
        $('#console-out').append(document.createTextNode('There were compiler errors...\n\n'));
        for (var i = 0; i < details.length; i++) {
            $('#console-out').append(document.createTextNode('  ' + details[i].Message + '\n'));
            $('#console-out').append(document.createTextNode('    ' + details[i].LocationDescription + '\n'));
        }

        editor.showErrors(details);
    };

    //////////////////////////////////
    // Code for handling window events
    ////////////////////////////////// 

    // User clicks run button
    $('#run').click(function () {
        console.clear();
        console.focus();
        // Prevent user from running again
        editor.clearErrors();
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
                complete();
            });
        } catch (e) {
            alert(e.message);
            complete();
        }
    });

    // Change theme
    $('#Theme').change(function () {
        var theme = $(this).val();
        // Update the current theme
        editor.setTheme(theme);
        console.setTheme(theme);
        // And send a request to the server to save the theme preference
        $.ajax({
            url: "/api/theme",
            type: "PUT",
            dataType: "json",
            data: '=' + theme // The '=' is needed to put the un-named string into x-www-form-urlencoded format
        }).fail(function (request, status, error) {
            console.dir(request);
            alert('Failed to save your theme preference: ' + status + " - " + error +
                '\n\nThe JavaScript console contains more details of the problem');
        });
    });

    ////////////////////
    // Set up CodeMirror
    ////////////////////

    var editor = new Editor("code", $('#Theme').val());
    editor.setSize(null, '35em');
    editor.hideFrstCharacters(hiddenHeaderLength);

    var console = new Console("console", $('#Theme').val());
    console.setSize(null, '35em');
    console.onReturn = function (text) {
        hub.server.consoleIn(text);
    };
})();
