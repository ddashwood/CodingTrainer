function Ide(serviceFactory) {
    // Nested class: CodeSubmitter
    
    // Constructor parameter: submitFunction - a callback that takes {code,  data}
    var CodeSubmitter = function (submitFunction) {
        this.currentCode = new Rx.Subject();
        this.currentCode.pipe(
            Rx.operators.debounceTime(300),
            Rx.operators.distinctUntilChanged(o => o.text)
        ).subscribe(submitFunction);
    };
    // Methood: submit - call submit with code and data - it will call submitFunction if code has not changed
    CodeSubmitter.prototype.submit = function (code, data) {
        this.currentCode.next({code:code, data:data});
    };

    var self = this;



    // Make the service objects
    var codeRunner = serviceFactory.getCodeRunner(this);
    var ideServices = serviceFactory.getIdeServices(this);

    // Callbacks that are needed for the CodeMirror editors
    var completionsCodeSubmitter = new CodeSubmitter(function (codeData) {
        // Send a SignalR request to get the hints
        ideServices.requestCompletions(codeData.code, codeData.data.pos, codeData.data.tokenStart);
    });
    var requestCompletions = function (cm, tokenStart) {
        completionsCodeSubmitter.submit(cm.getValue(), { pos: cm.indexFromPos(cm.getCursor()), tokenStart: tokenStart });
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
        $('#theme-div').css('visibility', 'visible');
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

    var changedCodeSubmitter = new CodeSubmitter(function (codeData) {
        ideServices.requestDiagnostics(codeData.code, codeData.data /* generation */ );
    });


    var changed = function () {
        changedCodeSubmitter.submit(self.editor.getValue(), self.editor.changeGeneration(true));
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
            catch (e){
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