function Ide(serviceFactory, isSubmittable) {
    var self = this;
    this.isSubmittable = isSubmittable;

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
        $('.cm-btn-run').prop('disabled', true).css('color', 'lightgrey');
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
        self.enableRun();

        // Apply linting to the default code
        changed();
    });

    // Change theme
    $('#Theme').change(function () {
        var theme = $(this).val();
        // Update the current theme
        self.editor.setOption('theme', theme);
        self.codeConsole.setOption('theme', theme);
    });

    // Real-time linting

    var changed = function () {
        var generation = self.editor.changeGeneration(true);
        var code = self.editor.getValue();
        ideServices.requestDiagnostics(code, generation);
    };

    this.editor.on('change', function () {
        changed();
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
    // After user has tested their code, allow them to submit it
    if (this.isSubmittable) {
        $('.cm-btn-submit').addClass('cm-btn-submit-enabled');
    }
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
            self.editor.focus();
            self.editor.setCursor({ line: line, ch: 0 });

            // Check that the top of the editor is not hidden behind the nav bar
            try {
                var cursor = $('.CodeMirror-cursor');
                var cursorY;
                if (cursor.length > 0) {
                    cursorY = cursor.get(0).getBoundingClientRect().top;
                } else {
                    // CodeMirror-cursor class is not always present, especially on mobile
                    // CodeMirrow-scroll is not as accurate, but still works
                    cursorY = $('.CodeMirror-scroll').get(0).getBoundingClientRect().top;
                }
                var navBarHeight = parseInt($('#main-navbar').css("height"));
                if (cursorY < navBarHeight) {
                    var currentScroll = $(window).scrollTop();
                    $(window).scrollTop(currentScroll - (navBarHeight - cursorY));
                }
            }
            catch (e) {
                // Exceptions might occur if the classes we are looking for are not present.
                // In this case, ignore it
            }
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