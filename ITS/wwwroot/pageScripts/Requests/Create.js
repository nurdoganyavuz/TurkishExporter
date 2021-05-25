$(document).click(function () {
    $('.btncreate').click(function () {

        swal("Başarılı!", "Talep eklendi.", "success");
        $("#createform").submit();

    });
});