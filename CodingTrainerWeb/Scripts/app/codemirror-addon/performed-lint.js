(function () {
    // Set up the linter
    var unprocessedLints = null;
    var addedSpaceMarks = [];


    // N.b. this function doesn't actually do the linting, since that is done separately on the server
    // This function simply provides the linter output to CodeMirror for displaying
    CodeMirror.registerHelper('lint', 'clike', function (text, options, editor) {
        if (unprocessedLints) {
            var temp = unprocessedLints;
            unprocessedLints = null;
            return temp;
        }
        return [];
    });

    // The linting has already been performed on the server. But now show the results
    CodeMirror.defineExtension("showPerformedLint", function (errors) {
        unprocessedLints = [];

        // Start at the end - that way, if we need to add spaces in, it won't
        // affect the index of highlighting we haven't done yet
        for (var i = errors.length - 1; i >= 0; i--) {
            var error = errors[i];

            var startIdx = error.Location.SourceSpan.Start;
            var endIdx = error.Location.SourceSpan.End;

            var startPos = this.posFromIndex(startIdx);
            var endPos = this.posFromIndex(endIdx);

            if (startIdx === endIdx) {
                // If start/end are the same, there's nothing to underline
                // Adjust the selection to include an extra space
                var nextPos = this.posFromIndex(endIdx + 1);
                var prevPos = this.posFromIndex(endIdx - 1);

                // Try to the left first, see if there's a space there that we can let the linter style
                if (this.getRange(prevPos, startPos) === " ") {
                    startIdx--;
                    startPos = this.posFromIndex(startIdx);
                }

                else {
                    // We are going to mark a space to the right - first check if
                    // there's already a space there, and if not, add one
                    var addSpace = this.getRange(startPos, nextPos) !== " ";
                    if (addSpace) {
                        // It seems like the only time the start pos and end pos are the same
                        // is if the error is at the end of the line. In this case, it's safe to
                        // insert a space.
                        this.replaceRange(" ", startPos, endPos);
                    }
                    endIdx++;
                    endPos = this.posFromIndex(endIdx);

                    // If we added a space, mark it so we can remove it later, and so the user can't click on it
                    if (addSpace) {
                        addedSpaceMarks.push(this.markText(startPos, endPos, { atomic: true, inclusiveRight: true }));
                    }
                }
            }

            error.line = startPos.line;

            var severity = error.Severity === 3 ? "error" : error.Severity === 2 ? "warning" : "";
            unprocessedLints.push({ message: error.Message, severity: severity, from: startPos, to: endPos });
        }
        this.performLint();
    });

    CodeMirror.defineExtension("clearErrors", function () {
        this.performLint();
        // Remove any spaces that we've added in for the purpose of showing the lint style
        for (var i = 0; i < addedSpaceMarks.length; i++) {
            if (addedSpaceMarks[i].find()) {
                this.replaceRange("", addedSpaceMarks[i].find().from, addedSpaceMarks[i].find().to);
            }
            addedSpaceMarks[i].clear();
        }
        addedSpaceMarks = [];
    });

    CodeMirror.defineExtension("gotoLine", function (line) {
        this.setCursor({ line: line, ch: 0 });
        this.focus();
    });
})();