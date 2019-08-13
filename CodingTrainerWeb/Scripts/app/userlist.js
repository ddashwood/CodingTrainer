$('#unprocessed-only-check').change(function () {
    if (this.checked) {
        $('.user-processed').hide();
    }
    else {
        $('.user-processed').show();
    }
});