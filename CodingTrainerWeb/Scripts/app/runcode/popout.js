$(function () {
    if (window.opener !== null) {
        var popoutUnloadHandled = false;
        window.onbeforeunload = window.onunload = function () {
            if (popoutUnloadHandled) return;
            popoutUnloadHandled = true;

            try {
                opener.ideGlobals.popoutClosing(ideGlobals.ide.getValue());
            }
            catch (e) { console.log(e); }
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

            window.open("about:blank", "formresult", "left=0,top=0,width=800,height=640,menubar=no,toolbar=no,location=no,personalbar=no,status=no,scrollbars=yes,resizable=yes");
            
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
