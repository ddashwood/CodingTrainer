$.getJSON('/api/chapter', function (chapters) {
    $('#chapter-list').html('');
    $.each(chapters, function (i, chapter) {
        $("<h4 class='list-group-item-heading'>").text('Chapter ' + chapter.ChapterNo + ' - ' + chapter.ChapterName).appendTo($('#chapter-list'));
        $.each(chapter.Exercises, function (j, exercise) {
            var url = urlTemplate.replace("phld-chapter", chapter.ChapterNo).replace("phld-exercise", exercise.ExerciseNo);
            $("<a href='" + url + "' class='list-group-item'>").text(exercise.ExerciseName).appendTo($('#chapter-list'));
        });
    });
}).fail(function (request, status, error) {
    console.dir(request);
    $('#chapter-list').html('Failed to load chapter list: ' + status + " - " + error +
        '<br/><em>&emsp;The JavaScript console contains more details of the problem</em>');
    });