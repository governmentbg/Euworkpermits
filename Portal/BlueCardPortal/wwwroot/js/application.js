function isStartPermament() {
    return $("#IsStartPermanent").val() == "YES";
}

function ApplicationTypeChangeRadio(control) {
    const selected = document.querySelector(`input[name="${control.name}"]:checked`).value;
    if (selected == "1") {
        $(".permit-type-container").show();
        $(".permit-type-container").find("[name$='.PermitType']").removeClass("ignore")

        if (!isStartPermament()) {
            $(".btn-next-step").attr("disabled", "disabled");
            $(".form-check-input").attr("disabled", function () {
                return this.id == "ApplicationType.ApplicationTypeCode_2" ? null : "disabled";
            });
            messageHelper.ShowErrorMessage($("#PermanentMessage").text());
        }
    } else {
        $(".permit-type-container").hide();
        $(".permit-type-container").find("[name$='.PermitType']").addClass("ignore")

        if (!isStartPermament()) {
            $(".btn-next-step").removeAttr("disabled");
            $(".form-check-input").removeAttr("disabled");
        }
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
        $(`input[name$="ApplicantRole"]`).trigger("change");
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

function UicTypeChangeRadio(control) {
    const selected = document.querySelector(`input[name="${control.name}"]:checked`).value;
    if (selected == "EGN") {
        $(".applicant-egn-container").show();
    } else {
        $(".applicant-egn-container").hide();
    }
    if (selected == "LNCH") {
        $(".applicant-lnch-container").show();
    } else {
        $(".applicant-lnch-container").hide();
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
    const containerIsSame = $(".applicant-person-container").find('.applicant-contact-address-is-same');
    if (applicantRole == "2") {
        containerIsSame.show();
        containerIsSame.find("[name$='.ApplicantContactAddressIsSame']").trigger("change");
    } else {
        containerIsSame.hide();
        containerIsSame.find(`input[id$=".ApplicantContactAddressIsSame_NO"]`).prop('checked', true);
        containerIsSame.find("[name$='.ApplicantContactAddressIsSame']").trigger("change");
    }
}

function AddressIsSameChangeRadio(control) {
    const form = $(control).parents('form:first')
    const value = $(`input[name="${control.name}"]:checked`).val();
    const container = form.find(".address-container.address-can-be-same");
    if (value == "NO") {
        container.show();
    } else {
        container.hide();
    }
}

function ApplicantAddressIsSameChangeRadio(control) {
    const form = $(control).parents('form:first')
    const value = $(`input[name="${control.name}"]:checked`).val();
    const container = form.find(".applicant-address-can-be-same");
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
    $(".contact-type").trigger("change");
    $('.select2-drop-down').select2({
        theme: 'bootstrap-5'
    });
    InitDatePicker();
    initDynamicForms(function () {
        InitForm();
    })
}
$(function () {
    $(".contact-type").trigger("change");
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
    let foreignerLabel = container.find("[name$='.ForeignerLabel']").val();
    let foreignerSmallId = container.find("[name$='.ForeignerSmallId']").val() || null;
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
        title,
        foreignerLabel,
        foreignerSmallId
    };

    try {
        let view = await post_fetch_string_async(url, data);
        ShowModalDialog("Прикачане на документ", view, false, true);
    } catch (e) {
        messageHelper.ShowErrorMessage(e.message);
    }
}

async function removeFile(control) {
    const container = $(control).parents('.document-container:first');
    let hasMultipleFile = container.find("[name$='.HasMultipleFile']").val() == "True";
    let portalId = container.find("[name$='.PortalId']").val();
    const applicationId = $('#ApplicationId').val();

    await get_fetch_json_async('/Files/RemoveFile', { applicationId, portalId, remove: hasMultipleFile })
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

async function SendApplication(btn) {
    StartButtonAction(btn)
    const applicationId = $('#ApplicationId').val();
    const responce = await fetch('/Application/SendApplication?' + new URLSearchParams({ applicationId }));
    let result = await ResolveResponceJson(responce)
    if (result.state == "OK") {

        await Swal.fire({
            title: 'Резултат',
            text: result.message,
            confirmButtonText: "OK",
        });
        window.location.href = "/Home";
    } else {
        await Swal.fire(
            "Грешка",
            result.message,
            'error',
        );
        EndButtonAction(btn);
    }
}

async function GetEmployer(control) {
    $(control).attr("disabled", "disabled");
    const container = $(control).parents('.employer-container:first');
    const uic = container.find("[name$='.Identifier']").val();
    const prefix = container.find("[name$='.Identifier']")[0].name.replace('.Identifier', '');
    var responce = await get_fetch_string_async('/Application/GetEmployer', { uic, prefix })
    var applicationType = $("[name$='.ApplicationTypeCode']:checked").val();
    const containerParent = container.parents(':first');
    containerParent.html(responce);
    if (applicationType == "2") {
        containerParent.find('.employee-count').hide();
    }
    resetValidation(containerParent);
    InitForm();
}

async function SetStatusDraft() {
    const applicationId = $('#ApplicationId').val();
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
        let view = await get_fetch_string_async('/Application/AddForeignerSmall', data);
        container.data('index', index + 1);
        ShowModalDialog("Добавяне данни за чужденец", view, false, true);
    } catch (e) {
        messageHelper.ShowErrorMessage(e.message);
    }
}

async function ForeignerSmallEdit(btn) {
    StartButtonAction(btn)
    const form = $(btn).parents('form:first');
    resetValidation(form);

    if (!form.valid()) {
        EndButtonAction(btn)
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
        let view = await get_fetch_string_async('/Application/EditForeignerSmall', data);
        container.data('index', index + 1);
        ShowModalDialog("Добавяне данни за чужденец", view, false, true);
    } catch (e) {
        messageHelper.ShowErrorMessage(e.message);
    }
}
function GetPermitType() {
    
    const selected = $("[name$='.ApplicationTypeCode']:checked").val();
    let permitType = "5";
    if (selected == "1") {
        permitType = $(".permit-type-container").find("[name$='.PermitType']:checked").val();
    }
    $("[name$='.EmploymentPermitType']").val(permitType);
    const control = document.querySelector(`input[name="Employment.Type"]`);
    EmploymentTypeChangeRadio(control);
        
    return permitType;
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
function ContactTypeChange(control) {
    const container = $(control).parents('.contact-container:first');
    const description = container.find('.contact-description').find('input');
    const descriptionLabel = container.find('.contact-description').find('label');
    if ($(control).val() == '1' || $(control).val() == '2' || $(control).val() == '4') {
        description.show();
        descriptionLabel.show();
        if ($(control).val() == '4') {
            if (control.name.includes("Applicant.Person.Contacts.")) {
                description.val(GetApplicantContactAddress(control));
            }
            if (control.name.includes("Foreigner.")) {
                description.val(GetForeignerContactAddress(control))
            }
            if (control.name.includes("Employer.")) {
                description.val(GetEmployerContactAddress(control))
            }
            description.prop("readonly", true);
        } else {
            description.prop("readonly", false);
        }
    } else {
        description.hide();
        descriptionLabel.hide();
    }
    if ($(control).val() == '1') {
        container.find('.contact-is-prefered').each(function (i, btn) {
            $(btn).prop('checked', false);
            $(btn).attr("disabled", "disabled");
        });
    } else {
        container.find('.contact-is-prefered').each(function (i, btn) {
            $(btn).removeAttr("disabled");
        });
    }
}

function AddAddressText(container, result, itemName, prefix) {
    const item = container.find(`[id$='${itemName}']`).val();
    if (HaveAddressValue(item))
        result += prefix + item + " ";
    return result;
}
function AddressText(container) {
    let result = "";
    result = AddAddressText(container, result, 'Street', 'ул. ')
    result = AddAddressText(container, result, 'StreetNo', '№ ')

    result = AddAddressText(container, result, 'Quarter', 'кв. ')
    result = AddAddressText(container, result, 'BuildingNo', 'бл. ')

    result = AddAddressText(container, result, 'Entrance', 'вх. ')
    result = AddAddressText(container, result, 'Floor', 'ет. ')
    result = AddAddressText(container, result, 'Apartment', 'ап. ')

    // result = AddAddressText(container, result, 'Region', 'обл. ')
    var data = container.find(`[id$='Region']`).select2('data');
    if (HaveAddressValue(data[0].text))
        result += 'обл. ' + data[0].text + " ";
    result = AddAddressText(container, result, 'City', 'гр. ')
    result = AddAddressText(container, result, 'PostalCode', 'п.к. ')

    return result;
}

function GetEmployerContactAddress(control) {
    const form = $(control).parents('form:first')
    const value = form.find(`input[name$=".ContactAddressIsSame"]:checked`).val();
    if (value == "NO") {
        const container = form.find(".address-container.address-can-be-same");
        return AddressText(container);
    } else {
        const containerHead = form.find(".address-container.head-address");
        return AddressText(containerHead);

    }
}

function GetForeignerContactAddress(control) {
    const form = $(control).parents('form:first')
    var text = ""
    //form.find('.address-list-container').each(function (i, container) {
    //    if ($(container).find('.address-kind').val() == "3") {
    //        text = AddressText($(container));
    //    }
    //})    
    const container = form.find(".address-container.current-address");
    return AddressText(container);
    return text;
}
function GetApplicantContactAddress(control) {
    const form = $(control).parents('form:first')
    const applicantType = form.find("[name$='.ApplicantType']:checked").val();
    if (applicantType == "3") { 
        return GetForeignerContactAddress(control);
    }
    const value = form.find(`input[name$="ApplicantContactAddressIsSame"]:checked`).val();
    if (value == "NO") {
        const container = form.find(".applicant-address-can-be-same");
        return AddressText(container);
    } else {
        return GetEmployerContactAddress(control);
    }
}
function resetValidation(form) {
    form.removeData("validator")    // Added by jQuery Validation
        .removeData("unobtrusiveValidation");   // Added by jQuery Unobtrusive Validation
    $.validator.unobtrusive.parse(form);

    form.validate().settings.ignore = ":hidden:not(.bc-radio-button),.ignore";
}
function ApplicationClose() {
    window.location.href = "/Home/Index"
}

function birthDateTypeInputChange(control) {
    const form = $(control).parents('form:first');
    var typeInput = $(control).val();

    if (typeInput == "1") {
        form.find('.birth-date').show();
        form.find('.birth-month').hide();
    } else {
        form.find('.birth-date').hide();
        form.find('.birth-month').show();
    }
}

function EmploymentTypeChangeRadio(control) {
    debugger
    const selected_control = document.querySelector(`input[name="${control.name}"]:checked`);
    const selected = selected_control == null ? "NO" : selected_control.value;
    const permitType = $("[name$='.EmploymentPermitType']").val();
    if (permitType == "1" || permitType == "3") {
        $("[name$='Employment.EmployerChange']").removeClass("ignore")
        $(".employer-change-container").show();
        if (selected == "NO") {
            $("[id$='.EmployerChange_YES']").removeAttr("disabled");
        } else { 
            $("[id$='.EmployerChange_YES']").attr('disabled', 'disabled');
            $(`input[id$=".EmployerChange_NO"]`).prop('checked', true);
        }
    } else {
        $(".employer-change-container").hide();
        $("[name$='Employment.EmployerChange']").addClass("ignore")
        $("[id$='.EmployerChange_NO']").removeAttr("disabled");
        $(`input[id$=".EmployerChange_NO"]`).prop('checked', true);
    }
}
