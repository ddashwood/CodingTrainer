// Ensure entire page shows even when nav bar takes up more than one line

$(window).resize(function () {
    $('body').css('padding-top', parseInt($('#main-navbar').css("height")) + 1);
});

$(function () {
    $('body').css('padding-top', parseInt($('#main-navbar').css("height")) + 1);
});
