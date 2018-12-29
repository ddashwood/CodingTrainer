﻿function Ide(serviceFactory) {
    var self = this;

    // Make the service objects
    var codeRunner = serviceFactory.getCodeRunner(this);
    var ideServices = serviceFactory.getIdeServices(this);

    // Callbacks that are needed for the CodeMirror editors
    var requestCompletions = function (cm, tokenStart) {
        // Send a SignalR request to get the hints
        var code = cm.getValue();
        var pos = cm.indexFromPos(cm.getCursor());

        ideServices.requestCompletions(code, pos, tokenStart);
    };

    var consoleIn = function (message) {
        codeRunner.consoleIn(message);
    };

    var run = function () {
        self.codeConsole.clearAll();
        self.codeConsole.focus();
        // Prevent user from running again
        self.editor.clearErrors();
        $('.cm-btn-run').prop('disabled', true).css('color','lightgrey');
        $('#console-out').text('');

        var code = self.editor.getValue();
        codeRunner.run(code);
    };


    // Make the CodeMirror editors
    this.editor = this.getEditor(run, requestCompletions);
    this.codeConsole = this.getConsole(consoleIn);

    // Connect to the server
    // When done, enable the Run button, etc
    serviceFactory.connect(function () {
        $('.cm-btn-run').html('&#x25B6;&nbsp;Run');
        $('#theme-div').css('visibility', 'visible');
        self.enableRun();
    });

    // Change theme
    $('#Theme').change(function () {
        var theme = $(this).val();
        // Update the current theme
        self.editor.setOption('theme', theme);
        self.codeConsole.setOption('theme', theme);
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

    // Real-time linting
    this.editor.on('change', function () {
        var generation = self.editor.changeGeneration(true);
        var code = self.editor.getValue();
        ideServices.requestDiagnostics(code, generation);
    });
}

Ide.prototype.consoleOut = function (message) {
    this.codeConsole.consoleAppend(message);
};

Ide.prototype.enableRun = function () {
    $('.cm-btn-run').prop('disabled', false).css('color', 'black');
};

Ide.prototype.runComplete = function () {
    this.enableRun();
};

// This method may be used when errors occur during real-time linting,
// as well as when they occur while running the program
Ide.prototype.showErrors = function (errors) {
    this.codeConsole.clearAll();
    if (!errors) {
        this.editor.clearErrors();
        return;
    }

    this.editor.showPerformedLint(errors);

    this.codeConsole.consoleAppend('There were compiler errors...\n');
    this.codeConsole.consoleAppend('Click on an error to go to the affected line\n\n');
    var self = this;
    for (var j = 0; j < errors.length; j++) {
        this.codeConsole.consoleAppendWithLineLink('  ' + errors[j].Message + '\n    Line ' + (errors[i = j].line + 1) + '\n', errors[j].line, function (line) {
            self.editor.setCursor({ line: line, ch: 0 });
            self.editor.focus();
        });
    }
};

Ide.prototype.showErrorsForGeneration = function (errors, generation) {
    if (this.editor.isClean(generation)) {
        this.showErrors(errors);
    }
};

Ide.prototype.showAutoCompletions = function (completions, tokenStart) {
    this.editor.showHints(completions, tokenStart);
};

Ide.prototype.showParameters = function () { }; // Not yet implemented