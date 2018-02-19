(function () {
    // This function used when errors occure during real-time linting, and also
    // when they occur while running the program
    var handleErrors = function (errors) {
        console.clear();
        if (!errors) {
            editor.clearErrors();
            return;
        }

        if (model.HiddenCodeHeader) {
            adjustment = model.HiddenCodeHeader.length + 1;
            for (var i = errors.length - 1; i >= 0; i--) {
                errors[i].Location.SourceSpan.Start -= adjustment;
                errors[i].Location.SourceSpan.End -= adjustment;
            }
        }

        editor.showErrors(errors);

        console.append('There were compiler errors...\n');
        console.append('Click on an error to go to the affected line\n\n');
        for (var j = 0; j < errors.length; j++) {
            console.appendWithLineLink('  ' + errors[j].Message + '\n    Line ' + (errors[i = j].line + 1) + '\n', errors[j].line, function (line) {
                editor.gotoLine(line);
            });
        }

    };

    ///////////////////////////////////////////////////
    // Code for starting/maintaining SignalR connection
    ///////////////////////////////////////////////////

    var hubConnected = false;

    // Set up and maintain the connection
    $.connection.hub.start().done(function () {
        hubConnected = true;
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

    var runnerHub = $.connection.codeRunnerHub;

    runnerHub.client.consoleOut = function (message) {
        // Display stdout data
        //$('#console-out').append(document.createTextNode(message));

        console.append(message);
    };

    var complete = function () {
        $('#run').prop('disabled', false);
    };

    runnerHub.client.complete = complete;

    runnerHub.client.compilerError = handleErrors;

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
            var code = editor.getValue();
            if (model.HiddenCodeHeader) {
                code = model.HiddenCodeHeader + "\n" + code;
            }
            runnerHub.server.run(code).fail(function (e) {
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

    var console = new Console("console", $('#Theme').val());
    console.setSize(null, '35em');
    console.onReturn = function (text) {
        runnerHub.server.consoleIn(text);
    };

    ////////////////////
    // Real-time linting
    ////////////////////

    var ideHub = $.connection.ideHub;

    // Respond to callback from the hub
    ideHub.client.compilerError = function (errors, generation) {
        if (editor.isClean(generation)) {
            handleErrors(errors);
        }
    };

    // When there's a change, send it to the hub
    editor.onChange(function () {
        if (hubConnected) {
            var generation = editor.changeGeneration(true);
            var code = editor.getValue();
            if (model.HiddenCodeHeader) {
                code = model.HiddenCodeHeader + "\n" + code;
            }
            ideHub.server.validate(code, generation);
        }
    });
})();
