$(document).click(function () {
    $('.btncreate').click(function () {

        swal("Başarılı!", "Rol eklendi.", "success");
        $("#createform").submit();

    });
});