function ApplicationTypeChangeRadio(control) {
    const selected = document.querySelector(`input[name="${control.name}"]:checked`).value;
    if (selected == "1") {
        $(".permit-type-container").show();
        $(".permit-type-container").find("[name$='.PermitType']").removeClass("ignore")

        // ToDo: Да се махнат при пълна функционалност
        $(".btn-next-step").attr("disabled", "disabled");
        $(".form-check-input").attr("disabled", function() {
            return this.id == "ApplicationType.ApplicationTypeCode_2" ? null : "disabled";
        });
        messageHelper.ShowErrorMessage("Този вид заявление все още не е наличен в системата. Моля, изберете друг вид заявление.");
    } else {
        $(".permit-type-container").hide();
        $(".permit-type-container").find("[name$='.PermitType']").addClass("ignore")

        // ToDo: Да се махнат при пълна функционалност
        $(".btn-next-step").removeAttr("disabled");
        $(".form-check-input").removeAttr("disabled");
    }
}

function ApplicantTypeChangeRadio(control) {
    const selected = document.querySelector(`input[name="${control.name}"]:checked`).value;
    if (selected == "3") {
        $(".applicant-foreigner-container").show();
        $("[name$='.ApplicantRole']").addClass("ignore");
        $(".applicant-foreigner-container").find(".list-validation").removeClass("ignore")
    } else {
        $(".applicant-foreigner-container").hide();
        $("[name$='.ApplicantRole']").removeClass("ignore");
        $(".applicant-foreigner-container").find(".list-validation").addClass("ignore")
    }
    if (selected == "1" || selected == "2") {
        $(".applicant-person-container").show();
        $(".applicant-person-container").find(".list-validation").removeClass("ignore")
    } else {
        $(".applicant-person-container").hide();
        $(".applicant-person-container").find(".list-validation").addClass("ignore")
    }
    if (selected == "2") {
        $(`input[id$="ApplicantRole_1"]`).attr('disabled', 'disabled');
        $(`input[id$="ApplicantRole_2"]`).prop('checked', true);
        $(`input[name$="ApplicantRole"]`).trigger("change");
    } else {
        $(`input[id$="ApplicantRole_1"]`).removeAttr("disabled");
    }
}

function ApplicantRoleChangeRadio(control) {
    const applicantRole = $(`input[name="${control.name}"]:checked`).val();
    const container = $(".applicant-person-container").find('.employer-container');
    if (applicantRole == "2") {
        container.show();
        container.find(".list-validation").removeClass("ignore")
        container.find("[name$='.ContactAddressIsSame']").removeClass("ignore");
    } else {
        container.hide();
        container.find(".list-validation").addClass("ignore")
        container.find("[name$='.ContactAddressIsSame']").addClass("ignore");
    }
}

function AddressIsSameChangeRadio(control) {
    const value = $(`input[name="${control.name}"]:checked`).val();
    const container = $(".address-container.address-can-be-same");
    if (value == "NO") {
        container.show();
    } else {
        container.hide();
    }
}


function ForeignerTypeIdentifierChangeRadio(control) {
    const selected = document.querySelector(`input[name="${control.name}"]:checked`).value;
    if (selected == "1") {
        $(".type-identifier-lnch").show();
    } else {
        $(".type-identifier-lnch").hide();
    }
    if (selected == "2") {
        $(".type-identifier-external").show();
    } else {
        $(".type-identifier-external").hide();
    }
}

function InitForm() {
    $('.select2-drop-down').select2({
        theme: 'bootstrap-5'
    });
    InitDatePicker();
    initDynamicForms(function () {
        InitForm();
    })
}
$(function () {
    initDynamicForms(function () {
        InitForm();
    });
});
function GetFileIndex(documentTypeCode) {
    var index = 0;
    var indexForType = 0;
    var typeHasData = true;
    $('.document-container').each(function (n, container) {
        const container$ = $(container);
        let i = parseInt(container$.find("[name$='.Index']").val());
        if (i > index) {
            index = i;
        }
        let typeCode = container$.find("[name$='.DocumentTypeCode']").val();
        if (typeCode == documentTypeCode && i > indexForType) {
            indexForType = i;
            typeHasData = container$.find("[name$='.CmisId']").val() ? true : false;
        }
    });
    return {
        index,
        indexForType,
        typeHasData
    }
}
async function AppUploadFile(btn) {
    const container = $(btn).parents('.document-container:first');
    let applicationId = $('#ApplicationId').val();
    let url = $(btn).data('urlview');
    let index = container.find("[name$='.Index']").val();
    let id = container.find("[name$='.Id']").val() || null;
    let documentType = container.find("[name$='.DocumentType']").val() || null;
    let documentTypeCode = container.find("[name$='.DocumentTypeCode']").val() || null;
    let documentCategoryCode = container.find("[name$='.DocumentCategoryCode']").val() || null;
    let cmisId = container.find("[name$='.CmisId']").val() || null;
    let portalId = container.find("[name$='.PortalId']").val();
    let isMandatory = container.find("[name$='.IsMandatory']").val() == "True";
    let isOriginal = container.find("[name$='.IsOriginal']").is(":checked");
    let hasMultipleFile = container.find("[name$='.HasMultipleFile']").val() == "True";
    let hasTitle = container.find("[name$='.HasTitle']").val() == "True";
    let title = container.find("[name$='.Title']").val();
    const indexObj = GetFileIndex(documentTypeCode);
    let indexAll = indexObj.index + 1;
    let data = {
        index,
        indexAll,
        applicationId,
        id,
        documentType,
        documentTypeCode,
        documentCategoryCode,
        cmisId,
        portalId,
        isMandatory,
        isOriginal,
        hasMultipleFile,
        hasTitle,
        title
    };
    try {
        let view = await post_fetch_string_async(url, data);
        ShowModalDialog("Прикачане на документ", view, false, true);
    } catch (e) {
        messageHelper.ShowErrorMessage(e.message);
    }
}

function removeFile(control) {
    const container = $(control).parents('.document-container:first');
    let hasMultipleFile = container.find("[name$='.HasMultipleFile']").val() == "True";
    if (hasMultipleFile) {
        container.remove();
        return;
    }
    container.find("[name$='.Id']").val("");
    container.find("[name$='.CmisId']").val("");
    container.find(".document-file-name").html("");
    container.find("[name$='.FileUrl']").val("");
    container.find("[name$='.FileName']").val("");
    container.find("[name$='.Title']").val("");
    container.removeClass('list-documents-body-uploaded')
    container.addClass('list-documents-body')
    container.find('.u-btn--attach').show();
    container.find('.u-btn--close').hide();
    container.find('.u-btn--view').hide();
}


async function RegionChange(control) {
    const container = $(control).parents('.address-container:first');
    const region = $(control).select2('data')[0].text;
    const city_control = container.find('.address-city');
    await get_drop_down_async(
        '/Application/GetCities',
        { region },
        city_control,
    );
    city_control.trigger("change");
}

function IsPreferedContract(control) {
    const container = $(control).parents('.contacts-container:first');
    container.find('.contact-is-prefered').each(function (i, btn) {
        if (btn != control && $(btn).is(":checked")) {
            $(btn).prop('checked', false);
        }
    })
}

async function SendApplication() {
    const applicationId = $('#ApplicationId').val();
    //const responce = await get_async('/Application/SendApplication', { applicationId })
    const responce = await fetch('/Application/SendApplication?' + new URLSearchParams({ applicationId }));
    if (await ResolveIsOkResponce(responce)) {
        window.location.href = "/Home";
    }
}

async function GetEmployer(control) {
    const container = $(control).parents('.employer-container:first');
    const uic = container.find("[name$='.Identifier']").val();
    const prefix = container.find("[name$='.Identifier']")[0].name.replace('.Identifier', '');
    var responce = await get_string_async('/Application/GetEmployer', { uic, prefix });
    container.parents(':first').html(responce);
    InitForm();
}

async function SetStatusDraft() {
    const applicationId = $('#ApplicationId').val();
    //const responce = await get_async('/Application/SetStatusDraft', { applicationId })
    const responce = await fetch('/Application/SetStatusDraft?' + new URLSearchParams({ applicationId }));
    if (await ResolveIsOkResponce(responce)) {
        window.location.href = "/Home";
    }
}

async function AddForeignerSmall(btn) {
    const container = $(btn).parents('.foreigner-small-list-container:first');
    let index = container.data('index');
    let data = {
        index,
    };
    try {
        let view = await get_string_async('/Application/AddForeignerSmall', data);
        container.data('index', index + 1);
        ShowModalDialog("Добавяне данни за чужденец", view, false, true);
    } catch (e) {
        messageHelper.ShowErrorMessage(e.message);
    }
}

async function ForeignerSmallEdit(btn) {
    const form = $(btn).parents('form:first');
    form.removeData("validator")    // Added by jQuery Validation
        .removeData("unobtrusiveValidation");   // Added by jQuery Unobtrusive Validation
    $.validator.unobtrusive.parse(form);

    form.validate().settings.ignore = ":hidden:not(.bc-radio-button),.ignore";

    if (!form.valid()) {
        return false;
    }
    const index = form.find('#Index').val();
    const data = new FormData(form[0]);
    url = '/Application/ForeignerSmallEdit';
    const responce = await fetch(url, {
        method: 'post',
        body: data,
    })
    const view = await responce.text();

    HideModal();
    if ($(btn).data("isedit") && $(btn).data("isedit") != 'False') {
        const passportNumber = $(`[name='ForeignerSmallList.Items[${index}].PassportNumber']`);
        $(passportNumber).parents('.foreigner-small-container:first').hide().html(view).slideDown();
    } else {
        const control = $('.foreigner-small-list-container').find('div.list-validation');
        $(view).hide().insertBefore(control).slideDown();
    }
}

function PerformRemoveForeignerSmall(btn) {
    $(btn).parents('.foreigner-small-container:first').hide('normal').remove();
        return false;
}

async function PerformEditForeignerSmall(btn) {
    const container = $(btn).parents('.foreigner-small-container:first');
    const index = container.find("[name$='.Index']").val();
    const name = container.find("[name$='.Name']").val();
    const nameCyrilic = container.find("[name$='.NameCyrilic']").val();
    const birthDate = container.find("[name$='.BirthDate']").val();
    const passportNumber = container.find("[name$='.PassportNumber']").val();
    const nationality = container.find("[name$='.Nationality']").val();
    const nationalityCode = container.find("[name$='.NationalityCode']").val();
    const position = container.find("[name$='.Position']").val();
    const durationOfEmploymentFrom = container.find("[name$='.DurationOfEmploymentFrom']").val();
    const durationOfEmploymentTo = container.find("[name$='.DurationOfEmploymentTo']").val();
 
    let data = {
        index,
        name,
        nameCyrilic,
        birthDate,
        passportNumber,
        nationality,
        nationalityCode,
        position,
        durationOfEmploymentFrom,
        durationOfEmploymentTo
    };
    try {
        let view = await get_string_async('/Application/EditForeignerSmall', data);
        container.data('index', index + 1);
        ShowModalDialog("Добавяне данни за чужденец", view, false, true);
    } catch (e) {
        messageHelper.ShowErrorMessage(e.message);
    }
}
function GetPermitType() {
    
    const selected = $("[name$='.ApplicationTypeCode']:checked").val();
    if (selected == "1") {
        const permitType = $(".permit-type-container").find("[name$='.PermitType']:checked").val();
        return permitType;
    } else {
        return "5";
    }
}

$('#frmComplaintEdit').on('submit', async function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    const form = $(this);
    form.validate().settings.ignore = ":hidden:not(.bc-radio-button),.ignore";

    if (!form.valid()) {
        var formerrorList = $(this).data("validator").errorList;
        $.each(formerrorList, function (key, value) {
            console.log(formerrorList[key].element.id);
        });
        return false;
    }
    this.submit();
})

function addressStreetNoChange(element) {
    const container = $(element).parents('.address-container:first');
    container.find("[id$='Street']").valid()
    container.find("[id$='Quarter']").valid()
}

function addressBuildingNoChange(element) {
    const container = $(element).parents('.address-container:first');
    container.find("[id$='Street']").valid()
    container.find("[id$='Quarter']").valid()
}