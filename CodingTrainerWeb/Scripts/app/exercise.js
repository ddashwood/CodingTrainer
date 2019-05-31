// Submission tab

$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    if (typeof(CodeMirror) === 'undefined') {
        $('#submissions-content').text('Page is still initialising, please return to this screen in a moment');
        return;
    }

    var target = $(e.target).attr("href");
    if (target === '#submissions') {
        $('#submissions-content').text('Loading your previous submissions...');

        var url = '/api/submissions/' + exerciseGlobals.model.ChapterNo + '/' + exerciseGlobals.model.ExerciseNo;
        $.getJSON(url, function (submissions) {
            // Data received from server

            if (!CodeMirror) {
                var wait = function () {
                    setTimeout(function () {
                        if (!CodeMirror) wait(); // Iteratively wait for CodeMirror to be loaded
                    }, 100);
                };
                wait();
            }

            $('#submissions-content').empty();

            if (submissions.length === 0) {
                // No submissions
                $('<div>').text('You have not submitted any answers for this exercise yet')
                    .appendTo('#submissions-content');
            }
            else {
                // We have submissions
                $.each(submissions, function (i, submission) {
                    var submissionDate = new Date(submission.SubmissionDateTime);
                    var row = $('<div>').addClass('row');
                    $('#submissions-content').append(row);

                    $('<div>').addClass('col-sm-3 submission-heading')
                        .append($('<div>').addClass('list-group')
                            .append($("<a>").attr('href', 'javascript:openWindow(' + submission.SubmissionId + ')').addClass('list-group-item')
                                .append(document.createTextNode(submissionDate.toLocaleDateString(undefined,
                                    { weekday: 'short', day: 'numeric', month: 'short', year: 'numeric' })))
                                .append('<br>')
                                .append(document.createTextNode(submissionDate.toLocaleTimeString(undefined, { hour24: 'true' })))
                                .append('<br>')
                                .append('Completed ' + (submission.Success ? '&#x2714;' : '&#x2716;'))))
                        .appendTo(row);
                    var cmDiv = $('<div>').addClass('col-sm-9').appendTo(row);
                    $('<div>').addClass('visible-xs top-buffer').appendTo(row); // Space between items if single column
                    var cm = CodeMirror(cmDiv.get(0), {
                        value: submission.SubmittedCode,
                        mode: "text/x-csharp",
                        theme: exerciseGlobals.theme,
                        lineNumbers: true,
                        readOnly: true
                    });
                    cm.setSize(null, '7em');
                });
            }
        }).fail(function () {
            $('#submissions-content').text('Sorry, something went wrong when loading your submissions');
        });
    }
});

exerciseGlobals.openSubmissions = [];

function openWindow(id) {
    var url = "/Submission/Show/" + id;
    w = window.open(url, "_blank");
    exerciseGlobals.openSubmissions.push(w);
}

$(function () {
    var unload = function () {
        for (var i = 0; i < exerciseGlobals.openSubmissions.length; i++) {
            if (!exerciseGlobals.openSubmissions[i].closed) {
                exerciseGlobals.openSubmissions[i].close();
            }
        }
    };
    window.addEventListener('beforeunload', unload);
    window.addEventListener('unload', unload);
});

// Model Answer tab

$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    if (typeof (CodeMirror) === 'undefined') {
        $('#submissions-content').text('Page is still initialising, please return to this screen in a moment');
        return;
    }

    var target = $(e.target).attr("href");
    if (target === '#answer') {
        var codeSegments = document.getElementsByClassName('model-answer-code');
        var i;
        for (i = 0; i < codeSegments.length; i++) {
            var cm = CodeMirror.fromTextArea(codeSegments[i], {
                lineNumbers: true,
                mode: "text/x-csharp",
                theme: exerciseGlobals.theme,
                readOnly: true
            });
            var size = codeSegments[i].getAttribute('data-height');
            if (size === null) size = '30ex';
            cm.setSize(null, size);
        }
    }
});