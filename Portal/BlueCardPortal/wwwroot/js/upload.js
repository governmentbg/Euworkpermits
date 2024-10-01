$(function () {
    $('form#cdnUploadFile').ajaxForm({
        beforeSend: function () {
            $('#cdnUploadButton').hide();
            $('#uploadProgress').text('');
        },

        uploadProgress: function (event, position, total, percentComplete) {
            var percentVal = 'Моля, изчакайте... ' + percentComplete + '%';
            $('#uploadProgress').text(percentVal);
        },
        complete: function (xhr) {
            $('#cdnUploadButton').show();
            $('#uploadProgress').text('');

            const form = $('form#cdnUploadFile');
            var result = JSON.parse(xhr.responseText);
            if (result.isOk) {
                //const indexObj = GetFileIndex(documentTypeCode);
                //if (indexObj.indexForType > 0 && !indexObj.typeHasData)
                //{
                //}
                const control = $(`[value=${result.portalId}]`).not('.is-modal');
                const container = $(control).parents('.document-container:first');
                container.replaceWith(result.view)
                $('#cdnUploadButton').parents('div.modal:first').modal("hide");
            } else {
                Swal.fire(
                    result.error,
                    '',
                    'error'
                )
            }
        }
    });
});