function Ide(serviceFactory, isSubmittable, fixedSize, model) {
    var self = this;
    this.isSubmittable = isSubmittable;
    this.fixedSize = fixedSize;
    this.model = model;

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

    this.running = false;
    var run = function (forAssessment) {
        self.running = true;
        self.codeConsole.clearAll();
        self.codeConsole.focus();
        // Prevent user from running again
        self.editor.clearErrors();
        $('.cm-btn-disable-on-run').prop('disabled', true).css('color', 'lightgrey');
        $('#console-out').text('');

        var code = self.editor.getValue();
        codeRunner.run(code, forAssessment);
    };


    // Make the CodeMirror editors
    $('#ide').show();  // Keep this hidden until now so user can't click on link before the JavaScript is ready
    this.editor = this.getEditor(run, requestCompletions, model);
    this.codeConsole = this.getConsole(consoleIn);
    $('#ide-loading').hide();

    // Resise the editors based on the screen size
    if (this.fixedSize) {
        var resizeIde = function () {
            var screenHeight = $(window).height();
            var editorTop = $('.CodeMirror').first().offset().top;
            var buttonsHeight = $('.CodeMirror-buttonsPanel').first().height();
            var ideHeight = screenHeight - editorTop - buttonsHeight - 50;
            self.editor.setSize(null, (ideHeight * 0.6 + buttonsHeight) + "px");
            self.codeConsole.setSize(null, (ideHeight * 0.4) + "px");
        };
        var resizeIdeDebounceTimer = null;
        var resizeIdeDebounce = function () {
            clearTimeout(resizeIdeDebounceTimer);
            resizeIdeDebounceTimer = setTimeout(resizeIde, 100);
        };
        $(window).on('resize', resizeIdeDebounce);
        resizeIde();
    }

    // Connect to the server
    // When done, enable the Run button, etc
    serviceFactory.connect(function () {
        $('.cm-btn-run').html('&#x25B6;&nbsp;Run');
        if (self.isSubmittable) {
            $('.cm-btn-submit').addClass('cm-btn-submit-enabled');
        }
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
        if (!self.running)
            changed();
    });

    // These functions respond to the user's next action when they pass an assessment
    $('#successModal').on('hidden.bs.modal', function () {
        var w = window.opener;
        if (w === null) w = window;

        w.jQuery('#exercise-sidebar').load("/Exercise/ExerciseSidebarRefresh?" + $.param({
            chapter: model.ChapterNo,
            exercise: model.ExerciseNo
        }));
        w.jQuery('#answer').load("/Exercise/ModelAnswer?" + $.param({
            chapter: model.ChapterNo,
            exercise: model.ExerciseNo
        }));
    });
    $('.next-page').click(function () {
        $('#successModal').off('hidden.bs.modal');
        var w = window.opener;
        if (w === null) w = window;
        w.location.href = "/Exercise/Next?advance=false";
    });
}

Ide.prototype.getValue = function (separator) {
    return this.editor.getValue(separator);
};

Ide.prototype.setValue = function (content) {
    this.editor.setValue(content);
};

Ide.prototype.refresh = function () {
    this.editor.refresh();
    this.codeConsole.refresh();
}

Ide.prototype.consoleOut = function (message, colour) {
    this.codeConsole.consoleAppend(message, undefined, colour);
};

Ide.prototype.enableRun = function () {
    $('.cm-btn-disable-on-run').prop('disabled', false).css('color', 'black');
    this.running = false;
};

Ide.prototype.runComplete = function () {
    this.enableRun();
};

Ide.prototype.assessmentComplete = function (success) {
    if (success) {
        $("#successModal").modal();
    } else {
        $("#failModal").modal();
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

    this.codeConsole.consoleAppend('There were compiler errors...\n', true);
    this.codeConsole.consoleAppend('Click on an error to go to the affected line\n\n', true);
    var self = this;
    for (var i = 0; i < errors.length; i++) {
        if (errors[i].Location.SourceSpan.End < 0) {
            this.codeConsole.consoleAppend('  ' + errors[i].Message + '\n    In the test code\n', true);
        } else {
            this.codeConsole.consoleAppendWithLineLink('  ' + errors[i].Message + '\n    Line ' + (errors[i].line + 1) + '\n', errors[i].line, function (line) {
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
    }
};

Ide.prototype.showErrorsForGeneration = function (errors, generation) {
    if (this.editor.isClean(generation) && !this.running) {
        this.showErrors(errors);
    }
};

Ide.prototype.showAutoCompletions = function (completions, tokenStart) {
    this.editor.showHints(completions, tokenStart);
};

Ide.prototype.showParameters = function () { }; // Not yet implemented