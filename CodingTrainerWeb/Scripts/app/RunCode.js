(function () {
    // This function used when errors occure during real-time linting, and also
    // when they occur while running the program
    var handleErrors = function (errors) {
        codeConsole.clear();
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

        codeConsole.append('There were compiler errors...\n');
        codeConsole.append('Click on an error to go to the affected line\n\n');
        for (var j = 0; j < errors.length; j++) {
            codeConsole.appendWithLineLink('  ' + errors[j].Message + '\n    Line ' + (errors[i = j].line + 1) + '\n', errors[j].line, function (line) {
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

        codeConsole.append(message);
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
        codeConsole.clear();
        codeConsole.focus();
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
        codeConsole.setTheme(theme);
        // And send a request to the server to save the theme preference
        $.ajax({
            url: "/api/theme",
            type: "PUT",
            dataType: "json",
            data: '=' + theme // The '=' is needed to put the un-named string into x-www-form-urlencoded format
        }).fail(function (request, status, error) {
            codeConsole.dir(request);
            alert('Failed to save your theme preference: ' + status + " - " + error +
                '\n\nThe JavaScript console contains more details of the problem');
        });
    });

    ////////////////////
    // Set up CodeMirror
    ////////////////////

    var editor = new Editor("code", $('#Theme').val());
    editor.setSize(null, '35em');

    var codeConsole = new Console("console", $('#Theme').val());
    codeConsole.setSize(null, '35em');
    codeConsole.onReturn = function (text) {
        runnerHub.server.consoleIn(text);
    };

    ////////////////////
    // Real-time linting
    ////////////////////

    var ideHub = $.connection.ideHub;

    // Respond to callbacks from the hub
    ideHub.client.diagsCallback = function (diags, generation) {
        if (editor.isClean(generation)) {
            handleErrors(diags);
        }
    };

    ideHub.client.completionsCallback = function (completions, generation) {
        // Is just checking if clean sufficient? Need to check current token really.....
        if (editor.isClean(generation)) {
            codeConsole.clear();
            if (completions) {
                for (var i = 0; i < completions.length; i++) {
                    codeConsole.append(completions[i] + '\n');
                }
            }
        }
    };
    
    // When there's a change, send it to the hub
    editor.onChange(function () {
        if (hubConnected) {
            var generation = editor.changeGeneration(true);
            var code = editor.getValue();
            var pos = editor.getCursorIndex();
            if (model.HiddenCodeHeader) {
                code = model.HiddenCodeHeader + "\n" + code;
                pos += model.HiddenCodeHeader.length + 1;
            }
            console.log(pos);
            ideHub.server.requestDiags(code, generation);
        }
    });
})();
