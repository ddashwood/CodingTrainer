$(function () {
    var popoutUnloadHandled = false;
    var w = null;
    if (window.opener) {
        // We are a popout window - when we close, tell the main window
        var unload = function () {
            if (popoutUnloadHandled) return;
            popoutUnloadHandled = true;

            try {
                w = null;
                opener.ideGlobals.popoutClosing(ideGlobals.ide.getValue());
            }
            catch (e) { console.log(e); }
        };
        window.addEventListener('beforeunload', unload);
        window.addEventListener('unload', unload);
    }
    else {
        // We are the main window - when we close, close the popout window too
        window.onbeforeunload = window.onunload = function () {
            if (w !== null && !w.closed) {
                w.close();
                w = null;
            }
        };
    }

    ideGlobals.popoutClosing = function (code) {
        ideGlobals.ide.setValue(code);
        $('#ide').show();
        ideGlobals.ide.refresh();
    };

    $('#popoutLink').click(function () {
        try {
            $('#chapterInput').val(ideGlobals.model.ChapterNo);
            $('#exerciseInput').val(ideGlobals.model.ExerciseNo);
            $('#codeInput').val(ideGlobals.ide.getValue());

            w = window.open("about:blank", "formresult", "left=0,top=0,width=800,height=640,menubar=no,toolbar=no,location=no,personalbar=no,status=no,scrollbars=yes,resizable=yes");
            
            $('form#newWindow').submit();
            $('#ide').hide();
            return false;
        }
        catch (e) {
            console.log(e);
            alert('Something went wrong opening the new window');
            return false;
        }
   });
});
