$.validator.addMethod('bcaddress', function (value, element) {
    const container = $(element).parents('.address-container:first');
    const street = container.find("[id$='Street']").val();
    const streetNo = container.find("[id$='StreetNo']").val();
    const quarter = container.find("[id$='Quarter']").val();
    const buildingNo = container.find("[id$='BuildingNo']").val();
    if (HaveAddressValue(street) && HaveAddressValue(streetNo))
        return true;
    if (HaveAddressValue(quarter) && HaveAddressValue(buildingNo))
        return true;
    if (container.find("[name$='.IsCompanyAddress']").val() == "True") {
        if (HaveAddressValue(street))
            return true;
        if (HaveAddressValue(quarter))
            return true;
    }
    return false;
});

$.validator.unobtrusive.adapters.add('bcaddress', function (options) {
    options.rules['bcaddress'] = true;
    options.messages['bcaddress'] = options.message;
});

$.validator.addMethod('bcaddress-list', function (value, element) {
    const container = $(element).parents('.address-list-container:first');
    if (container.find('.address-container').length <= 0) {
        return false;
    }
    return true;
});

$.validator.unobtrusive.adapters.add('bcaddress-list', function (options) {
    options.rules['bcaddress-list'] = true;
    options.messages['bcaddress-list'] = options.message;
});

function HaveAddressValue(val) {
    if (val == undefined)
        return false;
    if (val == null)
        return false;
    if (val == "")
        return false;
    return true;
}

$.validator.addMethod('bccontact-phone', function (value, element, params) {
    const type = get_bccontact_type(element);
    if (type != '1')
        return true;
    const regex = $(element).attr('data-val-regex-phone');
    return IsValidRegex(element, regex);
});

$.validator.unobtrusive.adapters.add('bccontact-phone', function (options) {
    options.rules['bccontact-phone'] = true;
    options.messages['bccontact-phone'] = options.message;
});

$.validator.addMethod('bccontact-email', function (value, element, params) {
    const type = get_bccontact_type(element);
    if (type != '2')
        return true;
    const regex = $(element).attr('data-val-regex-email');
    return IsValidRegex(element, regex);
});

$.validator.unobtrusive.adapters.add('bccontact-email', function (options) {
    options.rules['bccontact-email'] = true;
    options.messages['bccontact-email'] = options.message;
});

var bcContactMessage = "Start error";
$.validator.addMethod('bccontact-list', function (value, element, params) {
    const container = $(element).parents('.contacts-container:first');
    if (container.find('.contact-container').length <= 0) {
        bcContactMessage = $(element).attr("data-val-bccontact-list-error-count");
        return false;
    }
    var hasPrefered = false;
    container.find('.contact-is-prefered').each(function (i, btn) {
        if ($(btn).is(":checked")) {
            hasPrefered = true;
        }
    })
    if (!hasPrefered) {
        bcContactMessage = $(element).attr("data-val-bccontact-list-error-prefered");
        return false;
    }
    return true;
}, bcContactMessage);

$.validator.unobtrusive.adapters.add('bccontact-list', function (options) {
    options.rules['bccontact-list'] = true;
    options.messages['bccontact-list'] = function () { return bcContactMessage; }
});

$.validator.addMethod('bcperson-document-list', function (value, element) {
    const container = $(element).parents('.person-documents:first');
    if (container.find('.person-document').length <= 0) {
        return false;
    }
    return true;
});
$.validator.unobtrusive.adapters.add('bcperson-document-list', function (options) {
    options.rules['bcperson-document-list'] = true;
    options.messages['bcperson-document-list'] = options.message;
});

$.validator.addMethod('bcdocuments', function (value, element) {
    const containerList = $(element).parents('.documents-container:first');
    var result = true;
    containerList.find('.document-container').each(function (n, container) {
        const container$ = $(container);
        const isMandatory = container$.find("[name$='.IsMandatory']").val() == "True";
        const typeHasData = container$.find("[name$='.CmisId']").val() ? true : false;
        if (isMandatory && !typeHasData)
            result = false;
    });
    return result;
});
$.validator.unobtrusive.adapters.add('bcdocuments', function (options) {
    options.rules['bcdocuments'] = true;
    options.messages['bcdocuments'] = options.message;
});
$.validator.addMethod('bcforeigner-small-list', function (value, element) {
    const container = $(element).parents('.foreigner-small-list-container:first');
    if (container.find('.foreigner-small-container').length <= 0) {
        return false;
    }
    return true;
});
$.validator.unobtrusive.adapters.add('bcforeigner-small-list', function (options) {
    options.rules['bcforeigner-small-list'] = true;
    options.messages['bcforeigner-small-list'] = options.message;
});


function get_bccontact_type(element) {
    const container = $(element).parents('.contact-container:first');
    return container.find("[name$='.Type']").val();
}
function IsValidRegex(element, params) {
    var match;
    //if (this.optional(element)) {
    //    return true;
    //}
    let value = $(element).val();
    match = new RegExp(params).exec(value);
    return (match && (match.index === 0) && (match[0].length === value.length));
}

$.validator.addMethod('bc-range-date-today', function (value, element) {
    const fromYear = $(element).data("val-from-year");
    const toYear = $(element).data("val-to-year");
    const dateFrom = moment().add(fromYear, "year");
    const dateTo = moment().add(toYear, "year");
    if (moment(value, 'DD.MM.YYYY') < dateFrom)
        return false;
    if (moment(value, 'DD.MM.YYYY') > dateTo)
        return false;
    return (value != null && value !== "" && value !== "-1");
});

$.validator.unobtrusive.adapters.add('bc-range-date-today', function (options) {
    options.rules['bc-range-date-today'] = true;
    options.messages['bc-range-date-today'] = options.message;
});

var bcDurationOfEmploymentMessage;
$.validator.addMethod('bc-duration-of-employment', function (value, element) {
    const period = $(element).data("val-period-days");
    const dateFrom = moment($("#DurationOfEmploymentFrom").val(), 'DD.MM.YYYY');
    const dateTo = moment($("#DurationOfEmploymentTo").val(), 'DD.MM.YYYY');
    if (dateFrom > dateTo && element.name == "DurationOfEmploymentTo") {
        bcDurationOfEmploymentMessage = "Началото на периода трябва да е преди края на периода";
        return false;
    }
    if (dateFrom < moment() && element.name == "DurationOfEmploymentFrom") {
        bcDurationOfEmploymentMessage = "Началото на периода трябва да е след " + moment().format('DD.MM.YYYY');
        return false;
    }
    let days = dateTo.diff(dateFrom, 'days');
    days = days + 1;
    if (days > period && element.name == "DurationOfEmploymentTo") {
        bcDurationOfEmploymentMessage = `Периодът не може да бъде повече от ${period} дни`;
        return false;
    }
    return (value != null && value !== "" && value !== "-1");
});

$.validator.unobtrusive.adapters.add('bc-duration-of-employment', function (options) {
    options.rules['bc-duration-of-employment'] = true;
    options.messages['bc-duration-of-employment'] = function () { return bcDurationOfEmploymentMessage; };
});

$.validator.addMethod('bc-identifier', function (value, element) {
    const uic_type = $(element).data("val-uic-type");
    if (!value) {
        return true;
    }
    switch (uic_type) {
        case 1: return CheckEGN(value);
        case 2: return CheckLNCH(value);
        case 3: return CheckEIK(value);
        default: return false;
    }
});

$.validator.unobtrusive.adapters.add('bc-identifier', function (options) {
    options.rules['bc-identifier'] = true;
    options.messages['bc-identifier'] = options.message;
});

$.validator.addMethod('bc-employee-count', function (value, element) {
    const container = $(element).parents('form:first');
    let employeeCount = container.find("[id$='EmployeeCount']").val();
    employeeCount = parseInt(employeeCount);
    let valueInt = parseInt(value);
    if (valueInt > employeeCount)
        return false;
    return true;
});
$.validator.unobtrusive.adapters.add('bc-employee-count', function (options) {
    options.rules['bc-employee-count'] = true;
    options.messages['bc-employee-count'] = options.message;
});

var bcEmploymentDurationMonthMessage;
var bcEmploymentDurationMonthMessageInit;
$.validator.addMethod('bc-employment-duration-month', function (value, element) {
    const permitType = $("[name$='.PermitType']:checked").val();
    $("[name$='.EmploymentPermitType']").val(permitType);
    const param = {
       "1" : {from: 24, to: 60},
       "2" : {from: 1, to: 36 },
       "3" : {from: 3, to: 9},
       "4" : {from: 1, to: 36},
    }[permitType];
    let month = parseInt(value)
    if (param.from > month || month > param.to) {
        bcEmploymentDurationMonthMessage = bcEmploymentDurationMonthMessageInit.replace("{0}", param.from);
        bcEmploymentDurationMonthMessage = bcEmploymentDurationMonthMessage.replace("{1}", param.to);
        return false;
    }
    return (value != null && value !== "" && value !== "-1");
});

$.validator.unobtrusive.adapters.add('bc-employment-duration-month', function (options) {
    bcEmploymentDurationMonthMessageInit = options.message;
    options.rules['bc-employment-duration-month'] = true;
    options.messages['bc-employment-duration-month'] = function () { return bcEmploymentDurationMonthMessage; };
});

