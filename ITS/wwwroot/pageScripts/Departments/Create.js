$(document).click(function () {
    $('.btncreate').click(function () {

        swal("Başarılı!", "Departman eklendi.", "success");
        $("#createform").submit();

    });
});