async function showApplicationModal(applicationId) {
    const responce = await fetch('/ApplicationPreview/GetApplicationRemote?' + new URLSearchParams({ applicationId }));
    const view = await responce.text();
    ShowModalDialog("Заявление", view, false, true);
}

function clearApplicationFilter() {
    $('#ForeignerName').val('');
    $('#ApplicationNumber').val('');
    $('#Status').val('');
    $('#PermitType').val('');
    $('#FromDate').val('');
    $('#ToDate').val('');
    $('#BirthDate').val('');
    $('#LNCH').val('');
    $('#Country').val('');
    searchApplication();
}

function GetApplicationFilter() {
    return {
        ForeignerName: $('#ForeignerName').val(),
        ApplicationNumber: $('#ApplicationNumber').val(),
        Status: $('#Status').val(),
        PermitType: $('#PermitType').val(),
        FromDate: $('#FromDate').val(),
        ToDate: $('#ToDate').val(),
        BirthDate: $('#BirthDate').val(),
        LNCH: $('#LNCH').val(),
        Country: $('#Country').val(),
    };
}
function searchApplication() {
    gridViewLoadData('#dvMain')
}

async function previewFile(control) {
    const container = $(control).parents('.document-container:first');
    let cmisId = container.find("[name$='.CmisId']").val() || null;
    let fileName = "file.pdf";
    try {
        const res = await fetch(
            '/Application/GetDocument?' + new URLSearchParams({ cmisId, fileName }),
            {
                headers: {
                    'Cache-Control': 'no-cache',
                    'Content-Type': 'application/json',
                    "X-CSRF-TOKEN": getRequestVerificationToken()
                }
            });
        const header = res.headers.get('Content-Disposition');
        const parts = header.split(';');
        fileName = decodeURI(parts[1].split('=')[1]);

        const blob = await res.blob();
        var url = window.URL.createObjectURL(blob);
        var a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a); // append the element to the dom
        a.click();
        a.remove(); // afterwards, remove the element  
    } catch (e) {
        console.error(e);
    }
    $("body").css("cursor", "default");
}

function ApplicationClose() {
    window.location.href = "/Home/Index"
}