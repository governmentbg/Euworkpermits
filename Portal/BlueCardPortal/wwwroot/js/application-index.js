function initGridView() {
    const loadUrl = $('#dvMain').data("url");
    if ($('#dvMain').hasClass('gridview-container')) {
        gridViewLoadData('#dvMain');
        return;
    }

    let mainTable = new GridView({
        container: '#dvMain',
        grid_title: 'Заявления',
        url: loadUrl,
        data: function () {
            return GetApplicationFilter();
        },
        columns: {
            rowClick: 'showApplicationModal',
            rowClickArgs: ['applicationId'],
            rowCss: function (row) {
                if (row.isActive == true) {
                    return '';
                }
                return 'grid-row-unpublished';
            },
            items: [

                {
                    name: 'applicationNumber',
                    title: 'Номер на заявление',
                    cssClass: 'col-md-2'
                },
                {
                    name: 'foreignerName',
                    title: 'Име на чужденец',
                    cssClass: 'col-md-2'
                },
                {
                    name: 'status',
                    title: 'Статус',
                    cssClass:'col-md-2'
                },
                {
                    name: 'entryDate',
                    title: 'Дата на входиране',
                    cssClass: 'col-md-2',
                    render: function (el) {
                        return JsonBGdate(el);
                    }
                },
                {
                    name: 'permitType',
                    title: 'Разрешение',
                    cssClass:'col-md-2',
                },
                {
                    name: 'applicationNumber',
                    title: '',
                    cssClass: 'col-md-2',
                    render: function (el, row) {
                        if (row.forComplaint) {
                            return `<button type="button" class="btn u-btn u-bg-sm u-btn--right u-btn--arrow"  onClick="AddComplaint('${el}')">Подаване <br>на жалба</button>`
                        }
                        if (row.forUpdate) {
                            return `<button type="button" class="btn u-btn u-bg-sm u-btn--right u-btn--arrow"  onClick="ApplicationUpdate('${el}')">Промяна</button>`
                        }
                        if (row.forSelfDenial) {
                            return `<button type="button" class="btn u-btn u-bg-sm u-btn--right u-btn--arrow"  onClick="AddSelfDenial('${el}')">Самоотказ</button>`
                        }
                        return "";
                    }
                }
            ]
        }
    });
}
function AddComplaint(el) {
    event.stopImmediatePropagation();
    window.location.href = `/Application/AddComplaint?applicationNumber=${el}`;;
}

function AddSelfDenial(el) {
    event.stopImmediatePropagation();
    window.location.href = `/Application/AddSelfDenial?applicationNumber=${el}`;;
}
function ApplicationUpdate(el) {
    event.stopImmediatePropagation();
    window.location.href = `/Application/ApplicationUpdate?applicationNumber=${el}`;;
}

$(() => {
    initGridView();
})