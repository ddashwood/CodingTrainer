Ide.prototype.getEditor = function (run, requestCompletions) {
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
                class: "cm-btn-submit cm-btn-disable-on-run",
                label: "Submit",
                callback: function () { run(true); }
            },
            {
                hotkey: "F5",
                class: "cm-btn-run cm-btn-disable-on-run",
                label: "Connecting",
                callback: function () { run(false); }
            },
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

    $('.cm-btn-run').prop('disabled', true).css('color', 'lightgrey');

    // Configure Bootstrap tooltips on IDE buttons

    // Tooltips don't give a good experience on touchscreens - only show them
    // if we have a mouse. Assume we have a mouse if using IE, since it doesn't
    // support matchMedia('(pointer:fine)')
    var ie = false;
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) ie = true;

    if (ie || matchMedia('(pointer:fine)').matches) {
        // Device has a mouse
        $('.cm-btn-run').attr('data-toggle', 'tooltip').attr('title', 'Run (F5)').tooltip();
        $('.cm-btn-undo').attr('data-toggle', 'tooltip').attr('title', 'Undo (Ctrl-Z)').tooltip();
        $('.cm-btn-redo').attr('data-toggle', 'tooltip').attr('title', 'Redo (Ctrl-Y)').tooltip();
        $('.cm-btn-comment').attr('data-toggle', 'tooltip').attr('title', 'Toggle comment (Ctrl-/)').tooltip();
        $('.cm-btn-indent').attr('data-toggle', 'tooltip').attr('title', 'Auto-indent (Ctrl-K Ctrl-D)').tooltip();


        $('[data-toggle="tooltip"]').tooltip({ trigger: 'hover' }).on('click', function () {
            $(this).tooltip('hide');
        });
    }
    
    editor.setSize(null, '35em');

    return editor;
};