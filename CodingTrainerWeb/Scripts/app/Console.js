var Console = function (textAreaId, theme) {
    var self = this;
    // Set up CodeMirror
    this.codeMirror = CodeMirror.fromTextArea(document.getElementById(textAreaId), {
        mode: "text/plain",
        theme: theme,
        extraKeys: {
            Enter: function () {
                if (self.onReturn) {
                    var line = self.codeMirror.getCursor().line;
                    self.onReturn(self.codeMirror.getLine(line));
                }
                self.codeMirror.setSelection(self.codeMirror.posFromIndex(Infinity));
                self.codeMirror.replaceSelection('\n');
            }
        }
    });
};

(function () {
    // Exposed methods
    Console.prototype.setSize = function (width, height) {
        this.codeMirror.setSize(width, height);
    };

    Console.prototype.append = function (text) {
        this.codeMirror.replaceRange(text, { line: Infinity });
        this.codeMirror.markText(this.codeMirror.posFromIndex(0), this.codeMirror.posFromIndex(Infinity), { atomic: true, inclusiveLeft: true });
    };
    Console.prototype.appendWithLineLink = function (text, line, action) {
        var startOfNewText = this.codeMirror.indexFromPos({ line: Infinity, ch: 0 });
        this.codeMirror.replaceRange(text, { line: Infinity });
        this.codeMirror.markText(this.codeMirror.posFromIndex(0), this.codeMirror.posFromIndex(Infinity), { atomic: true, inclusiveLeft: true });
        this.codeMirror.markText(this.codeMirror.posFromIndex(startOfNewText), this.codeMirror.posFromIndex(Infinity), { atomic: true, inclusiveLeft: true, title: "new-link", className: 'line-link' });
        var _new = $('[title="new-link"]');
        _new.attr('data-line', line.toString()).removeAttr('title');
        _new.click(function (event) {
            action($(event.target).attr('data-line'));
        });
    };
    Console.prototype.setTheme = function (theme) {
        this.codeMirror.setOption('theme', theme);
    };
    Console.prototype.clear = function () {
        this.codeMirror.setValue("");
        this.codeMirror.clearHistory();
    };
    Console.prototype.focus = function () {
        this.codeMirror.focus();
    };
})();