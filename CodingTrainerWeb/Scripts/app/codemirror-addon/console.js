CodeMirror.defineExtension("submitOnReturn", function (onReturn) {
    var self = this;

    self.onReturn = onReturn;
    self.addKeyMap({
        Enter: function () {
            if (self.onReturn) {
                var line = self.getCursor().line;
                self.onReturn(self.getLine(line));
            }
            self.setSelection(self.posFromIndex(Infinity));
            self.replaceSelection('\n');
        }
    });
});

CodeMirror.defineExtension("consoleAppend", function (text) {
    this.replaceRange(text, { line: Infinity });
    this.markText(this.posFromIndex(0), this.posFromIndex(Infinity), { atomic: true, inclusiveLeft: true });

});

CodeMirror.defineExtension("consoleAppendWithLineLink", function (text, line, action) {
    var startOfNewText = this.indexFromPos({ line: Infinity, ch: 0 });
    this.consoleAppend(text);
    this.markText(this.posFromIndex(startOfNewText), this.posFromIndex(Infinity), { title: "new-link", className: 'line-link' });
    $('[title="new-link"]')
        .attr('data-line', line.toString())
        .removeAttr('title')
        .click(function (event) {
            action($(event.target).attr('data-line'));
        });

});

CodeMirror.defineExtension("clearAll", function () {
    this.setValue("");
    this.clearHistory();
});
