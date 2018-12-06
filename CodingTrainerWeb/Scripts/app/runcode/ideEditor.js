function getEditor(requestCompletions) {
    var editor = CodeMirror.fromTextArea(document.getElementById('code'), {
        lineNumbers: true,
        matchBrackets: true,
        mode: "text/x-csharp",
        indentUnit: 4,
        dragDrop: false,
        autoCloseBrackets: true,
        theme: $('#Theme').val(),
        lint: { lintOnChange: false },
        loadHints: requestCompletions,
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
            },
            {
                hotkey: 'Ctrl-K Ctrl-D',
                class: "cm-btn-indent",
                label: "{ }",
                callback: function (cm) {
                    for (var i = 0; i < cm.lineCount(); i++) {
                        cm.indentLine(i);
                    }
                }
            }
        ],
        extraKeys: {
            Tab: function (cm) {
                var spaces = Array(cm.getOption("indentUnit") + 1).join(" ");
                cm.replaceSelection(spaces);
            }
        }
    });

    // Configure Bootstrap tooltips on IDE buttons
    $('.cm-btn-undo').attr('data-toggle', 'tooltip').attr('title', 'Undo (Ctrl-Z)').tooltip();
    $('.cm-btn-redo').attr('data-toggle', 'tooltip').attr('title', 'Redo (Ctrl-Y)').tooltip();
    $('.cm-btn-comment').attr('data-toggle', 'tooltip').attr('title', 'Toggle comment (Ctrl-/)').tooltip();
    $('.cm-btn-indent').attr('data-toggle', 'tooltip').attr('title', 'Auto-indent (Ctrl-K Ctrl-D)').tooltip();

    editor.setSize(null, '35em');

    return editor;
}