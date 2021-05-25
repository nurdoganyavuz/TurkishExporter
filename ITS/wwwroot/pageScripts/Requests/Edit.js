$(document).click(function () {
    $('#RequestStatus').change(function () {
        if ($('#RequestStatus').val() == 2) {
            $('#redStatus').show();
        }
        else {
            $('#redStatus').hide();
        }
    });
});