(function () {
    var cmCode = CodeMirror.fromTextArea(document.getElementById('code'), {
        lineNumbers: true,
        mode: "text/x-csharp",
        theme: submissionsGlobals.theme,
        readOnly: true
    });

    var cmFeedback = CodeMirror.fromTextArea(document.getElementById('feedback'), {
        mode: "text/plain",
        theme: submissionsGlobals.theme,
        readOnly: true
    });

    // Resise the editors based on the screen size
    var resizeIde = function () {
        var screenHeight = $(window).height();
        var editorTop = $('.CodeMirror').first().offset().top;
        var ideHeight = screenHeight - editorTop - 100;
        cmCode.setSize(null, (ideHeight * 0.6) + "px");
        cmFeedback.setSize(null, (ideHeight * 0.4) + "px");
    };
    var resizeIdeDebounceTimer = null;
    var resizeIdeDebounce = function () {
        clearTimeout(resizeIdeDebounceTimer);
        resizeIdeDebounceTimer = setTimeout(resizeIde, 100);
    };
    $(window).on('resize', resizeIdeDebounce);
    resizeIde();
})();