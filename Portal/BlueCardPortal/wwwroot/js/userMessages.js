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
