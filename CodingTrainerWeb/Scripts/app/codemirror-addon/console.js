CodeMirror.defineExtension("submitOnReturn", function (onReturn) {
    var self = this;

    self.onReturn = onReturn;
    self.addKeyMap({
        Enter: function () {
            if (self.onReturn) {
                // We can't return the whole of this line, because some of it
                // might have been put there as output

                // But the output should be read only, so if we select "everything",
                // we should get just the bits the user typed
                self.setSelection(self.posFromIndex(Infinity), self.posFromIndex(0));
                var input = self.getSelection();
                self.onReturn(input);
            }
            self.setSelection(self.posFromIndex(Infinity));
            self.replaceSelection('\n');
        }
    });
});

CodeMirror.defineExtension("consoleAppend", function (text, preventScroll) {
    this.replaceRange(text, { line: Infinity });
    if (preventScroll !== true) {
        this.focus();
        this.setCursor({ line: Infinity, ch: 0 });
    }
    this.markText(this.posFromIndex(0), this.posFromIndex(Infinity), { atomic: true, inclusiveLeft: true });
});

CodeMirror.defineExtension("consoleAppendWithLineLink", function (text, line, action) {
    var startOfNewText = this.indexFromPos({ line: Infinity, ch: 0 });
    this.consoleAppend(text, true);

    var el = $("<span>").text(text)
        .attr('data-line', line.toString())
        .addClass('line-link')
        .click(function (event) {
            event.preventDefault();
            action($(event.target).attr('data-line'));
        });

    this.markText(this.posFromIndex(startOfNewText), this.posFromIndex(Infinity), { replacedWith: el.get(0) });
});

CodeMirror.defineExtension("clearAll", function () {
    this.setValue("");
    this.clearHistory();
});
