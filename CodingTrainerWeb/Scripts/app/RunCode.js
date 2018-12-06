(function () {
    ///////////////////////////////////////
    // Functions for completion suggestions
    ///////////////////////////////////////


    ////////////////////
    // Set up CodeMirror
    ////////////////////

    // Editor

    var editor = CodeMirror.fromTextArea(document.getElementById('code'), {
        lineNumbers: true,
        matchBrackets: true,
        mode: "text/x-csharp",
        indentUnit: 4,
        dragDrop: false,
        autoCloseBrackets: true,
        theme: $('#Theme').val(),
        lint: { lintOnChange: false },
        loadHints: loadHints,
        gutters: ["CodeMirror-lint-markers"],
        buttons: [
            {
                hotkey: "Ctrl-Z",
                class: "cm-btn-undo",
                label: "&#x21BA;",
                callback: function (cm) {
                    cm.execCommand("undo");
                }
            },
            {
                hotkey: "Ctrl-Y",
                class: "cm-btn-redo",
                label: "&#x21BB;",
                callback: function (cm) {
                    cm.execCommand("redo");
                }
            },
            {
                hotkey: "Ctrl-/",
                class: "cm-btn-comment",
                label: "//",
                callback: function (cm) {
                    var sel = cm.listSelections();
                    for (var i = 0; i < sel.length; i++) {
                        cm.toggleComment(sel[i].anchor, sel[i].head);
                    }
                }
            },
            {
                hotkey: 'Ctrl-K Ctrl-D',
                class: "cm-btn-indent",
                label: "{ }",
                callback: function (cm) {
                    for (var i = 0; i < cm.lineCount(); i++) {
                        cm.indentLine(i);
                    }
                }
            }
        ],
        extraKeys: {
            Tab: function (cm) {
                var spaces = Array(cm.getOption("indentUnit") + 1).join(" ");
                cm.replaceSelection(spaces);
            }
        }
    });

    $('.cm-btn-undo').attr('data-toggle', 'tooltip').attr('title', 'Undo (Ctrl-Z)').tooltip();
    $('.cm-btn-redo').attr('data-toggle', 'tooltip').attr('title', 'Redo (Ctrl-Y)').tooltip();
    $('.cm-btn-comment').attr('data-toggle', 'tooltip').attr('title', 'Toggle comment (Ctrl-/)').tooltip();
    $('.cm-btn-indent').attr('data-toggle', 'tooltip').attr('title', 'Auto-indent (Ctrl-K Ctrl-D)').tooltip();

    editor.setSize(null, '35em');

    // Console

    var codeConsole = CodeMirror.fromTextArea(document.getElementById("console"), {
        mode: "text/plain",
        theme: $('#Theme').val()
    });
    codeConsole.setSize(null, '35em');
    codeConsole.submitOnReturn(function (text) {
        hubsContainer.runnerHub.server.consoleIn(text);
    });


    // This function used when errors occure during real-time linting, and also
    // when they occur while running the program
    var handleErrors = function (errors) {
        codeConsole.clearAll();
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

        editor.showPerformedLint(errors);

        codeConsole.consoleAppend('There were compiler errors...\n');
        codeConsole.consoleAppend('Click on an error to go to the affected line\n\n');
        for (var j = 0; j < errors.length; j++) {
            codeConsole.consoleAppendWithLineLink('  ' + errors[j].Message + '\n    Line ' + (errors[i = j].line + 1) + '\n', errors[j].line, function (line) {
                editor.setCursor({ line: line, ch: 0 });
                editor.focus();
            });
        }
    };


    // Hubs

    var hubsContainer = new hubs(editor, codeConsole, handleErrors);


    ///////////////////////////////////////////////////
    // Code for starting/maintaining SignalR connection
    ///////////////////////////////////////////////////

    //var hubConnected = false;

    //// Set up and maintain the connection
    //$.connection.hub.start().done(function () {
    //    hubConnected = true;
    //    // Once connected, allow users to submit code
    //    $('#run').prop('disabled', false).text('Run Code');
    //});
    //$.connection.hub.disconnected(function () {
    //    // Attempt to reconnect
    //    $.connection.hub.start();
    //});

    //////////////////////////////////////////////////////
    //// Code for handling SignalR events for running code
    //////////////////////////////////////////////////////

    //var runnerHub = $.connection.codeRunnerHub;

    //runnerHub.client.consoleOut = function (message) {
    //    codeConsole.consoleAppend(message);
    //};

    //var complete = function () {
    //    $('#run').prop('disabled', false);
    //};

    //runnerHub.client.complete = complete;

    //runnerHub.client.compilerError = handleErrors;

    //////////////////////////////////
    // Code for handling window events
    ////////////////////////////////// 

    // User clicks run button
    $('#run').click(function () {
        codeConsole.clearAll();
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
            hubsContainer.runnerHub.server.run(code).fail(function (e) {
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
        editor.setOption('theme', theme);
        codeConsole.setOption('theme', theme);
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
    // Real-time linting
    ////////////////////

    var ideHub = $.connection['ideHub'];

    // Respond to callbacks from the hub
    ideHub.client.diagsCallback = function (diags, generation) {
        if (editor.isClean(generation)) {
            handleErrors(diags);
        }
    };

    // When there's a change, send it to the hub to get diagnostics
    editor.on('change', function () {
        if (hubsContainer.hubConnected) {
            var generation = editor.changeGeneration(true);
            var code = editor.getValue();
            //var pos = editor.indexFromPos(this.getCursor());
            if (model.HiddenCodeHeader) {
                code = model.HiddenCodeHeader + "\n" + code;
                //pos += model.HiddenCodeHeader.length + 1;
            }
            ideHub.server.requestDiags(code, generation);
        }
    });

    /////////////////////////
    // Completion Suggestions
    /////////////////////////

    function loadHints(cm, tokenStart) {
        // Send a SignalR request to get the hints
        var code = cm.getValue();
        var pos = cm.indexFromPos(cm.getCursor());
        if (model.HiddenCodeHeader) {
            code = model.HiddenCodeHeader + "\n" + code;
            pos += model.HiddenCodeHeader.length + 1;
        }

        ideHub.server.requestCompletions(code, pos, tokenStart);
    }

    ideHub.client.completionsCallback = function (completions, tokenStart) {
        editor.showHints(completions, tokenStart);
    };

})();
