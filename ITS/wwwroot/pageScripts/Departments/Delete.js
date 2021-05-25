$(document).ready(function () {
    $('.btndelete').click(function () {
        var url = $(this).attr('data-url');
        console.log(url);
        swal({
            title: "Silme Onayı",
            text: "Silmek istediğinizden emin misiniz?",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "Evet",
            cancelButtonText: "İptal",
            closeOnConfirm: false
        }, function () {
            window.location.href = url;
        });
    });
});