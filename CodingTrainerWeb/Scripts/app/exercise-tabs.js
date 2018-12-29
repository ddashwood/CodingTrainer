$("#both-button").click(function () {
    $("#exercise-display")
        .css("display", "block")
        .removeClass()
        .addClass("col-md-3")
        .addClass("col-sm-12")
        .addClass("col-md-push-6");
    $("#code-display")
        .css("display", "block")
        .removeClass()
        .addClass("col-md-6")
        .addClass("col-sm-12")
        .addClass("col-md-pull-3");
});

$("#code-button").click(function () {
    $("#exercise-display").css("display", "none");
    $("#code-display")
        .css("display", "block")
        .removeClass()
        .addClass("col-md-9")
        .addClass("col-sm-12");
    $("#submissions").css("display", "none");
});

$("#exercise-button").click(function () {
    $("#exercise-display")
        .css("display", "block")
        .removeClass()
        .addClass("col-md-9")
        .addClass("col-sm-12");
    $("#code-display").css("display", "none");
    $("#submissions").css("display", "none");
});

$("#submissions-button").click(function () {
    $("#exercise-display").css("display", "none");
    $("#code-display").css("display", "none");
    $("#submissions").css("display", "block");
});