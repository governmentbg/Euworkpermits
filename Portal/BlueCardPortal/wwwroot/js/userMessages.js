$(function () {
    if ($('#ServerErrorMessage').val()) {
        toastr.error($('#ServerErrorMessage').val());
        $('#ServerErrorMessage').val('');
    }
});

$(function () {
    if ($('#ServerWarningMessage').val()) {
        toastr.error($('#ServerWarningMessage').val());
        $('#ServerWarningMessage').val('');
    }
});

$(function () {
    if ($('#ServerSuccessMessage').val()) {
        toastr.success($('#ServerSuccessMessage').val());
        $('#ServerSuccessMessage').val('');
    }
});


$(function () {
    if ($('#SwalServerErrorMessage').val()) {
        Swal.fire(
            "Грешка",
            $('#SwalServerErrorMessage').val(),
            'error',
        )
        $('#SwalServerErrorMessage').val('');
    }
});

$(function () {
    if ($('#SwalServerWarningMessage').val()) {
        Swal.fire(
            "Предупреждение",
            $('#SwalServerWarningMessage').val(),
            'warning',
        )
        $('#SwalServerWarningMessage').val('');
    }
});

$(function () {
    if ($('#SwalServerSuccessMessage').val()) {
        Swal.fire({
            title: 'Резултат',
            text: $('#SwalServerSuccessMessage').val(),
            confirmButtonText: "OK",
        });
        $('#SwalServerSuccessMessage').val('')
    }
});

