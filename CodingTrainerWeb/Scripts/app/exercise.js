// Submission tab

$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    var target = $(e.target).attr("href");
    if (target === '#submissions') {
        $.getJSON('/api/submissions/2/2', function (submissions) {
            //$('#').html('');

            $.each(submissions, function (i, submission) {
                //console.log(submission);
                //alert(submission.SubmittedCode);
            });
        }).fail(function (request, status, error) {
            alert('Error');
        });
    }
});
