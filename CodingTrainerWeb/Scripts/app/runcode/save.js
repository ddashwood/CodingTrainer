$(function () {
    function saveIfVisible(code, unloading) {
        if ($('#ide').is(':visible')) {
            try {
                $('#ide-save').css('visibility', 'visible');

                var url = "/api/SavedWork/" + ideGlobals.model.ChapterNo + "/" + ideGlobals.model.ExerciseNo;
                $.ajax({
                    type: "PUT",
                    url: url,
                    dataType: "application/json",
                    data: "=" + encodeURIComponent(code)
                }).done(function () {
                    $('#ide-save-error').hide();
                }).fail(function () {
                    if (!unloading)
                        $('#ide-save-error').show();
                });
            }
            catch (e) {
                if (!unloading)
                    $('#ide-save-error').show();
            }
            finally {
                // Slight delay so user has time to see it
                setTimeout(function () {
                    $('#ide-save').css('visibility', 'hidden');
                }, 300);
            }
        }
    }

    // Save on exit
    // N.b. Mobile iOS does not support beforeunload
    //   Recommended alternatives are unload, or pagehide
    //   but testing shows that neither of these work properly
    //   either. No known resolution at present.
    $(window).on('beforeunload', function () {
        saveIfVisible(ideGlobals.ide.getValue(), true);
    });



    // Autosave
    var lastSavedCode = ideGlobals.ide.getValue();

    function autoSave() {
        var currentCode = ideGlobals.ide.getValue();
        if (lastSavedCode !== currentCode) {
            lastSavedCode = currentCode;
            saveIfVisible(currentCode, false);
        }
        setTimeout(autoSave, 3000);
    }
    autoSave();
});