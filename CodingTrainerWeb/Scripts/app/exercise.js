$(function () {
    var url = courseUrl + chapter + "/" + exercise + ".html";
    $('#exercise-content').load(url, function (response, status, xhr) {
        if (status == 'error') {
            if (xhr.status == 404) {
                $('#exercise-content').text('There is no course material for this exercise');
            } else {
                $('#exercise-content').text('An error occured loading the course material');
            }
        }
    });
});