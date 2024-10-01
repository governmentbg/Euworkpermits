const stepButtons = document.querySelectorAll(".step-button");
const progress = document.querySelector("#progress");


function getStepButtonsLength() {
    var stepButtonLength = 0;
    stepButtons.forEach((item, secindex) => {
        if (!$(item).is(":hidden")) {
            stepButtonLength += 1;
        }
    });
    return stepButtonLength;
}
Array.from(stepButtons).forEach((button, index) => {
    button.addEventListener("click", () => {
        stepButtons.forEach((item, secindex) => {
            if (index > secindex) {
                item.classList.add("done");
            }
            if (index < secindex) {
                item.classList.remove("done");
            }
        });
        progress.setAttribute(
            "value",
            ((button.getAttribute('data-number') - 1) * 100) / (getStepButtonsLength() - 1)
        );
    });
});
function setStepItemText(id, text) {
    const btn = $(`#step${id}item`).find('button');
    btn.text(text)
}
async function StepItemClick(control) {
    var step = $(control)
    const number = step.data('number');
    var applicationType = $("[name$='.ApplicationTypeCode']:checked").val();
    if (applicationType == "1" && number == 2) {
        $('#step3item').show();
        $('#step5item').show();
        $('#step9item').hide();
        setStepItemText(6, '6');
        setStepItemText(7, '7');
        setStepItemText(8, '8');
    }
    if (applicationType == "2" && number == 2) {
        $('#step3item').hide();
        $('#step5item').hide();
        $('#step9item').show();
        setStepItemText(6, '5');
        setStepItemText(7, '6');
        setStepItemText(8, '7');
    }

    const containernext = GetNextStep(number, true);
    if (containernext.length > 0) {
        $('.btn-next-step').show();
    }
    else {
        $('.btn-next-step').hide();
    }
    const containerprev = GetNextStep(number, false);
    if (containerprev.length > 0) {
        $('.btn-prev-step').show();
    }
    else {
        $('.btn-prev-step').hide();
    }

    if (step.data("bs-target") == "#step7") {
        const form = $(step.data("bs-target")).find('#formDocuments');
        if (form.length == 1) {
            const documentPermitType = form.find('.document-permit-type');
            const documentApplicantType = form.find('.document-aplicant-type');
            if (documentPermitType.length == 1 && documentApplicantType.length == 1) {
                const permitType = GetPermitType();
                const applicantType = $(".applicant-container").find("[name$='.ApplicantType']:checked").val();
                const applicationId = $('#ApplicationId').val();
                if (documentPermitType.val() !== permitType || documentApplicantType.val() !== applicantType) {
                    const data = {
                        applicationId,
                        permitType,
                        applicantType
                    }
                    const view = await get_string_async('/Application/AddDocuments', data);
                    form.html(view);
                }
            }
        }
    }
    if (step.data("bs-target") == "#step3") {
        const form = $(step.data("bs-target")).find('#formForeigner');
        const permitType = GetPermitType();
        const applicantType = $(".applicant-container").find("[name$='.ApplicantType']:checked").val();
        if (form.length == 1 && permitType != 5 && applicantType == 3) {
            const applicationId = $('#ApplicationId').val();
            const view = await get_string_async('/Application/AddForeigner', { applicationId });
            form.html(view);
        }
    }
    if (step.data("bs-target") == "#step8") {
        const form = $(step.data("bs-target")).find('#formApplicationPreview');
        if (form.length == 1) {
            const applicationId = $('#ApplicationId').val();
            const view = await get_string_async('/ApplicationPreview/PreviewLocal', { applicationId });
            form.find('.application-preview').html(view);
        }
    }
}

function GetNextStep(number, up) {
    let next = up ? number + 1 : number - 1;
    let step = $(`[data-number="${next}"]`).not(":hidden");
    if (step.length == 0) {
        next = up ? next + 1 : next - 1;
        step = $(`[data-number="${next}"]`).not(":hidden");
    }
    return step;
}
function SetStep(number, up) {
    const step = GetNextStep(number, up);
    step.prop('disabled', false);
    step.trigger("click");
    step.focus().blur();
    step.prop('disabled', true);
    var num = step.data("number");
    $('.step-item').each(function (n, item) {
        const btn = $(item).find("button");
        if (btn.data("number") > num)
            btn.prop('disabled', true);
        if (btn.data("number") < num)
            btn.prop('disabled', false);
    })

}


async function SetNextStep() {
    const currStep = $('[aria-expanded="true"]');
    let number = currStep.data('number') || 0;
    let id = currStep.data('step-id');
    const container = $(`#step${id}`);
    const form = container.find('form');

    form.validate().settings.ignore = ":hidden:not(.bc-radio-button),.ignore";

    if (!form.valid()) {
        return false;
    }
    const data = new FormData(form[0]);
    data.append('applicationId', $('#ApplicationId').val());
    url = form.attr("action");
    const responce = await fetch(url, {
        method: 'post',
        body: data,
    })
    if (await ResolveIsOkResponce(responce)) {
        SetStep(number, true);
    }
}

function SetPrevStep() {
    const currStep = $('[aria-expanded="true"]');
    let number = currStep.data('number') || 2;
    SetStep(number, false);
}