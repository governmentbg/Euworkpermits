// Async get request
async function get_async(url, data) {
    return new Promise((resolve, reject) => {
        $.ajax({
            type: 'GET',
            async: true,
            cache: false,
            contentType: "application/json;charset=utf-8",
            dataType: 'json',
            url: url,
            data: data,
            success: function (result) {
                resolve(result);
            },
            error: function (err) {
                reject(err);
            },
        });
    });
}

// Async post request
async function post_async(url, data) {
    return new Promise((resolve, reject) => {
        $.ajax({
            type: 'POST',
            async: true,
            cache: false,
            dataType: 'json',
            url: url,
            data: data,
            headers: {
                "X-CSRF-TOKEN": getRequestVerificationToken()
            },
            success: function (result) {
                resolve(result);
            },
            error: function (err) {
                reject(err);
            },
        });
    });
}

// Async get request за dataType: 'string'
async function get_string_async(url, data) {
    return new Promise((resolve, reject) => {
        $.ajax({
            type: 'GET',
            async: true,
            cache: false,
            contentType: "application/json;charset=utf-8",
            dataType: 'text',
            url: url,
            data: data,
            success: function (result) {
                resolve(result);
            },
            error: function (err) {
                reject(err);
            },
        });
    });
}

async function post_string_async(url, data) {
    return new Promise((resolve, reject) => {
        $.ajax({
            type: 'POST',
            async: true,
            cache: false,
            url: url,
            data: {
                json: JSON.stringify(data)
            },
            headers: {
                "X-CSRF-TOKEN": getRequestVerificationToken()
            },
            success: function (result) {
                resolve(result);
            },
            error: function (err) {
                reject(err);
            },
        });
    });
}

function refreshTable(dataTableID) {
    $(dataTableID).DataTable().ajax.reload(null, true);
    return true;
}

function deleteItemWithConfirmation(deleteUrl, callback = null) {
    $('.confirm-delete')
        .modal({
            centered: true,
            closable: false,
            onApprove: function () {
                let url = deleteUrl;
                get_async(url)
                    .then((result) => {
                        if (callback !== null) {
                            callback();
                        }
                        else if (result.redirectUrl) {
                            window.location.href = result.redirectUrl;
                        }
                        else {
                            window.location.reload();
                        }
                    })
                    .catch((error) => {
                        console.error('Грешка при изтриване от URL ' + deleteUrl + " : " + error.statusText);
                    });
            }
        })
        .modal('show');
};


// Показва съобщения от JS
var messageHelper = (function () {
    function ShowErrorMessage(message) {
        toastr.error(message);
    }

    function ShowSuccessMessage(message) {
        toastr.success(message);
    }

    function ShowWarning(message) {
        toastr.warning(message);
    }

    return {
        ShowErrorMessage: ShowErrorMessage,
        ShowSuccessMessage: ShowSuccessMessage,
        ShowWarning: ShowWarning
    };
})();


async function swalConfirmAsync(text) {
    const dialogResult = await Swal.fire({
        title: 'Потвърди',
        text: text,
        showCancelButton: true,
        confirmButtonText: "Потвърди",
        cancelButtonText: "Отказ"
    });
    return !!dialogResult.isConfirmed;
}

function initDynamicForms(addCallback) {
    async function PerformAddItem(addItem) {
        const container = $(addItem).parents('.dynamic-form-container:first');
        const index = container.data('index');
        const prefix = container.data('prefix');
        const beforebtn = addItem.data('beforebtn');
        let data = {
            index,
            prefix
        };
        const html = await get_string_async(addItem.data('url'), data);
        container.data('index', index + 1);
        if (beforebtn) {
            $(html).hide().insertBefore(addItem.parent()).slideDown();
        } else {
            $(html).hide().appendTo(container).slideDown();
        }

        let form = container.parents('form:first');
        form.removeData("validator")    // Added by jQuery Validation
            .removeData("unobtrusiveValidation");   // Added by jQuery Unobtrusive Validation
        $.validator.unobtrusive.parse(form);
        if (addCallback) {
            addCallback();
        }
    }
    $('button.add-item').each(function (i, btn) {
        if ($(btn).data("is-set-click") !== undefined)
            return;
        $(btn).click(async function () {
            const addItem = $(this);
            await PerformAddItem(addItem);
            return false;
        });
        $(btn).data("is-set-click", true);
    });

    async function PerformRemoveItem(removeLink) {
        if (removeLink.data('alert')) {
            const confirm = await swalConfirmAsync(removeLink.data('alert'));
            if (confirm) {
                removeLink.parents('.item-template:first').hide('normal').remove();
            }
            return false;
        } else {
            removeLink.parents('.item-template:first').hide('normal').remove();
            return false;
        }
    }
    $('button.remove-item').each(function (i, btn) {
        if ($(btn).data("is-set-click") !== undefined) {
            return;
        }
        $(btn).click(async function () {
            const removeLink = $(this);
            await PerformRemoveItem(removeLink);
        });
        $(btn).data("is-set-click", true);
    });
}


function InitDatePicker() {
    //tempusDominus.extend(tempusDominus.plugins.customDateFormat);
    tempusDominus.loadLocale(tempusDominus.locales.bg);
     //globally
    tempusDominus.locale(tempusDominus.locales.bg.name);
    $(".date-picker").each(function (i, element) {
        if ($(element).data('loaded')) {
            return;
        }
        let selectedValue = undefined;
        if ($(element).data("value").length > 0) {
            selectedValue = $(element).data("value");
        }
        let currentTimezone = Intl.DateTimeFormat().resolvedOptions().timeZone;
        const _picker = new tempusDominus.TempusDominus(element, {
            defaultDate:  selectedValue,
            display: {
                viewMode: "calendar",
                components: {
                    decades: false,
                    year: true,
                    month: true,
                    date: true,
                    hours: false,
                    minutes: false,
                    seconds: false
                },
                icons: {
                    type: 'icons',
                    time: 'fa-solid fa-clock',
                    date: 'fa-solid fa-calendar',
                    up: 'fa-solid fa-arrow-up',
                    down: 'fa-solid fa-arrow-down',
                    previous: 'fa fa-chevron-left',
                    next: 'fa fa-chevron-right',
                    today: 'fa-solid fa-calendar-check',
                    clear: 'fa-solid fa-trash',
                    close: 'fa-solid fa-xmark'
                }, 
            }
        });
        _picker.dates.setFromInput(selectedValue);

        $(element).data('loaded', 'loaded')
    });
    $(".month-picker").each(function (i, element) {
        if ($(element).data('loaded')) {
            return;
        }
        let selectedValue = undefined;
        if ($(element).data("value").length > 0) {
            selectedValue = $(element).data("value");
        }
        const _picker = new tempusDominus.TempusDominus(element, {
            defaultDate: selectedValue,
            display: {
                viewMode: "months",
                components: {
                    decades: false,
                    year: true,
                    month: true,
                    date: false,
                    hours: false,
                    minutes: false,
                    seconds: false
                },
                icons: {
                    type: 'icons',
                    time: 'fa-solid fa-clock',
                    date: 'fa-solid fa-calendar',
                    up: 'fa-solid fa-arrow-up',
                    down: 'fa-solid fa-arrow-down',
                    previous: 'fa fa-chevron-left',
                    next: 'fa fa-chevron-right',
                    today: 'fa-solid fa-calendar-check',
                    clear: 'fa-solid fa-trash',
                    close: 'fa-solid fa-xmark'
                },
            },
            localization: {
                locale: 'bg-BG',
                format: 'MM.yyyy',
            }
        });
        $(element).data('loaded', 'loaded');
    });
    
}


function getValueLongNullable(el) {
    return $(el).val() || null;
}

function getValueDateTimeNullable(el) {
    moment.locale('bg');
    return moment($(el).val(), 'DD.MM.YYYY').toDate() || null;
}

$(function () {
    InitDatePicker();
});

function getFormData($form) {
    var unindexed_array = $form.serializeArray();
    var indexed_array = {};

    $.map(unindexed_array, function (n, i) {
        indexed_array[n['name']] = n['value'];
    });

    return indexed_array;
}

async function get_drop_down_async(url, data, dropdown, selected) {
    const items = await get_fetch_json_async(url, data)
    fill_drop_down(items, dropdown, selected)
}
function fill_drop_down(items, dropdown, selected) {
    const ddl = items.reduce(
        (accumulator, currentValue) => {
            const isSelected = (currentValue.value == selected ? "selected" : "");
            return accumulator + (currentValue.value == null ?
                `<option value ${isSelected}>${currentValue.text}</option>` :
                `<option value="${currentValue.value}" ${isSelected}>${currentValue.text}</option>`);
        }
        , '');
    dropdown.html(ddl);
}


async function downloadFile(url, request) {
    let fileName = 'report.xlsx';
    $('#ajaxLoader').hide();
    // $('#ajaxLoader').show();
    try {
        const res = await fetch(
            url,
            {
                method: "POST",
                body: JSON.stringify(request),
                headers: {
                    'Cache-Control': 'no-cache',
                    'Content-Type': 'application/json',
                    "X-CSRF-TOKEN": getRequestVerificationToken()
                }
            });
        const header = res.headers.get('Content-Disposition');
        const parts = header.split(';');
        fileName = parts[1].split('=')[1];
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
    //$('#ajaxLoader').hide();
    $("body").css("cursor", "default");
}

function replaceAll(str, find, replace) {
    return str.replace(new RegExp(find, 'g'), replace);
}

async function post_fetch_async(url, request) {
    try {
        const res = await fetch(
            url,
            {
                method: "POST",
                body: JSON.stringify(request),
                headers: {
                    'Cache-Control': 'no-cache',
                    'Content-Type': 'application/json',
                    "X-CSRF-TOKEN": getRequestVerificationToken()
                }
            });
        return await res.json();
    } catch (e) {
        console.error(e);
    }
}

function getRequestVerificationToken() {
    return document.getElementsByName("__RequestVerificationToken")[0].value;
}

function htmlEncode(input) {
    return input
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
}

function JsonBGdate(value) {
    if (!value) {
        return '';
    }
    try {
        let date = Date.parse(value);
        return new Intl.DateTimeFormat('bg-BG').format(date);
    }
    catch (e) {
        console.log(value);
        return '';
    }
}

function JsonBGDateTime(value) {
    if (!value) {
        return '';
    }
    try {
        let date = Date.parse(value);
        return new Intl.DateTimeFormat('bg-BG', {
            year: "numeric",
            month: "2-digit",
            day: "2-digit",
            hour: "2-digit",
            minute: "2-digit"
        }).format(date);
    }
    catch (e) {
        console.log(value);
        return '';
    }
}

function logValidationError(form) {
    var formerrorList = $(form).data("validator").errorList;
    $.each(formerrorList, function (key, value) {
        console.log(formerrorList[key].element.id);
    });
}

async function ResolveIsOkResponce(responce) {
    let result = await ResolveResponceJson(responce)
    if (result.state == "OK") {
        return true;
    } else {
        messageHelper.ShowErrorMessage(result.message)
    }
}

async function get_fetch_json_async(url, data) {
    const responce = await fetch(url + "?" + new URLSearchParams(data));
    return ResolveResponceJson(responce);
}

async function get_fetch_string_async(url, data) {
    const responce = await fetch(url + "?" + new URLSearchParams(data));
    return ResolveResponceString(responce)
}
async function post_fetch_string_async(url, data) {
    const responce = await fetch(url,
        {
            method: "POST",
            body: JSON.stringify(data),
            headers: {
                'Cache-Control': 'no-cache',
                'Content-Type': 'application/json',
                "X-CSRF-TOKEN": getRequestVerificationToken()
            }
        });
    return ResolveResponceString(responce)
}
async function post_fetch_json_async(url, data) {
    const responce = await fetch(url,
        {
            method: "POST",
            body: JSON.stringify(data),
            headers: {
                'Cache-Control': 'no-cache',
                'Content-Type': 'application/json',
                "X-CSRF-TOKEN": getRequestVerificationToken()
            }
        });
    return ResolveResponceJson(responce);
}

async function ResolveResponceString(responce) {
    if (responce.redirected) {
        await Swal.fire(
            "Изтекла е потребителската сесия",
            '',
            'error'
        )
        window.location.href = window.location.href
        return null;
    }
    let text = await responce.text();
    return text;
}
async function ResolveResponceJson(responce) {
    let text = "Възникна непредвидена грешка";
    const contentType = responce.headers.get('content-type');
    if (responce.status == 200 && contentType.startsWith('application/json;')) {
        try {
            return await responce.json();
        }
        catch (e) {
            messageHelper.ShowErrorMessage(text);
        }
    } else {
        if (responce.redirected) {
            text = "Изтекла е потребителската сесия";
            await Swal.fire(
                text,
                '',
                'error'
            )
            window.location.href = window.location.href
        }
        else {
            text = await responce.text();
            await Swal.fire(
                text,
                '',
                'error'
            )
        }

    }
}
function StartButtonAction(btn) {
    $(btn).prop('disabled', true);
}
function EndButtonAction(btn) {
    $(btn).prop('disabled', false);
}