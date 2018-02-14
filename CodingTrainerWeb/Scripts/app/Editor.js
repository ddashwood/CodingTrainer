var Editor = function (textAreaId, theme) {
    // Set up CodeMirror
    this.codeMirror = CodeMirror.fromTextArea(document.getElementById(textAreaId), {
        lineNumbers: true,
        matchBrackets: true,
        mode: "text/x-csharp",
        indentUnit: 4,
        dragDrop: false,
        autoCloseBrackets: true,
        theme: theme,
        lint: { lintOnChange: false },
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
            }
        ]
    });

    $('.cm-btn-undo').attr('data-toggle', 'tooltip').attr('title', 'Undo (Ctrl-Z)').tooltip();
    $('.cm-btn-redo').attr('data-toggle', 'tooltip').attr('title', 'Redo (Ctrl-Y)').tooltip();
    $('.cm-btn-comment').attr('data-toggle', 'tooltip').attr('title', 'Toggle comment (Ctrl-/)').tooltip();
};

(function () {
    // Set up the linter
    var unprocessedLints = null;

    CodeMirror.registerHelper('lint', 'clike', function (text, options, editor) {
        if (unprocessedLints) {
            var temp = unprocessedLints;
            unprocessedLints = null;
            return temp;
        }
        return [];
    });

    // Exposed methods
    Editor.prototype.setSize = function (width, height) {
        this.codeMirror.setSize(width, height);
    };
    Editor.prototype.getValue = function () {
        return this.codeMirror.getValue();
    };
    Editor.prototype.showErrors = function (errors) {
        unprocessedLints = [];

        // Start at the end - that way, if we need to add spaces in, it won't
        // affect the index of highlighting we haven't done yet
        for (var i = errors.length - 1; i >= 0; i--) {
            var error = errors[i];

            var startIdx = error.Location.SourceSpan.Start;
            var endIdx = error.Location.SourceSpan.End;

            var startPos = this.codeMirror.posFromIndex(startIdx);
            var endPos = this.codeMirror.posFromIndex(endIdx);

            if (startIdx === endIdx) {
                // If start/end are the same, there's nothing to underline
                // Adjust the selection to include an extra space
                var nextPos = this.codeMirror.posFromIndex(endIdx + 1);
                var prevPos = this.codeMirror.posFromIndex(endIdx - 1);

                // Try to the left first, see if there's a space there that we can highlight
                if (this.codeMirror.getRange(prevPos, startPos) === " ") {
                    startIdx--;
                    startPos = this.codeMirror.posFromIndex(startIdx);
                }

                else {
                    // We are going to mark a space to the right - first check if
                    // there's already a space there, and if not, add one
                    if (this.codeMirror.getRange(startPos, nextPos) !== " ") {
                        // It seems like the only time the start pos and end pos are the same
                        // is if the error is at the end of the line. In this case, it's safe to
                        // insert a space.
                        this.codeMirror.replaceRange(" ", startPos, endPos);
                    }
                    endIdx++;
                    endPos = this.codeMirror.posFromIndex(endIdx);
                }
            }

            var severity = error.Severity === 3 ? "error" : error.Severity === 2 ? "warning" : "";
            unprocessedLints.push({ message: error.Message, severity: severity, from: startPos, to: endPos });
        }
        this.codeMirror.performLint();
    };
    Editor.prototype.clearErrors = function () {
        this.codeMirror.performLint();
    };
    Editor.prototype.hideFrstCharacters = function (howMany) {
        if (howMany > 0) {
            var startPos = this.codeMirror.posFromIndex(0);
            var endPos = this.codeMirror.posFromIndex(howMany);
            this.codeMirror.markText(startPos, endPos, { collapsed: true });
        }
    };
    Editor.prototype.setTheme = function(theme) {
        this.codeMirror.setOption('theme', theme);
    };
})();