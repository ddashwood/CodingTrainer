var Editor = function (textAreaId) {
    this.codeMirror = CodeMirror.fromTextArea(document.getElementById(textAreaId), {
        lineNumbers: true,
        matchBrackets: true,
        mode: "text/x-csharp"
    });
};
Editor.prototype.setSize = function (width, height) {
    this.codeMirror.setSize(width, height);
};
Editor.prototype.getValue = function () {
    return this.codeMirror.getValue();
};
Editor.prototype.showErrors = function (errors) {
    // Start at the end - that way, if we need to add spaces in, it won't
    // affect the index of highlighting we haven't done yet
    for (var i = errors.length - 1; i >= 0; i--) {
        var error = errors[i];
        var startIdx = error.Location.SourceSpan.Start;
        var endIdx = error.Location.SourceSpan.End;

        var startPos = this.codeMirror.posFromIndex(startIdx);
        var endPos = this.codeMirror.posFromIndex(endIdx);

        if (startIdx === endIdx) {
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

        this.codeMirror.markText(startPos, endPos, { className: 'error' });


    }
};
Editor.prototype.clearErrors = function () {
    var marks = this.codeMirror.getAllMarks();
    for (var i = 0; i < marks.length; i++) {
        marks[i].clear();
    }
};