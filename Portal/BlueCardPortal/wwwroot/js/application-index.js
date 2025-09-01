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
                    title: '№ заявление',
                    cssClass: 'col'
                },
                {
                    name: 'employerName',
                    title: 'Работодател',
                    cssClass: 'col'
                },
                {
                    name: 'foreignerName',
                    title: 'Име на чужденец',
                    cssClass: 'col'
                },
                {
                    name: 'status',
                    title: 'Статус',
                    cssClass:'col'
                },
                {
                    name: 'entryDate',
                    title: 'Дата на входиране',
                    cssClass: 'col',
                    render: function (el) {
                        return JsonBGdate(el);
                    }
                },
                {
                    name: 'permitType',
                    title: 'Разрешение',
                    cssClass:'col-2 text-wrap',
                },
                {
                    name: 'applicationNumber',
                    title: '',
                    cssClass: 'col',
                    render: function (el, row) {
                        let btn = row.remark ? `<p>${row.remark}</p>`: "";
                        if (row.paymentAccessCode && row.paymentAccessCode.length > 0) {
                            btn += `<button onclick="toPayment('${row.paymentAccessCode}')" target="_blank" type="button" class="btn u-btn u-btn--sm app-list-btn u-btn--right u-btn--arrow" >Плащане</button>`
                        }
                        if (row.forComplaint) {
                            btn += `<button type="button" class="btn u-btn u-btn--sm  app-list-btn u-btn--right u-btn--arrow"  onClick="AddComplaint('${el}')">Подаване <br>на жалба</button>`
                        }
                        if (row.forUpdate) {
                            btn +=  `<button type="button" class="btn u-btn u-btn--sm  app-list-btn u-btn--right u-btn--arrow"  onClick="ApplicationUpdate('${el}')">Промяна</button>`
                        }
                        if (row.forSelfDenial) {
                            btn +=  `<button type="button" class="btn u-btn u-btn--sm  app-list-btn u-btn--right u-btn--arrow"  onClick="AddSelfDenial('${el}')">Самоотказ</button>`
                        }
                        return btn;
                    }
                }
            ]
        }
    });
}
function AddComplaint(el) {
    event.stopImmediatePropagation();
    window.location.href = `/Application/AddComplaint?applicationNumber=${el}`;
}

function AddSelfDenial(el) {
    event.stopImmediatePropagation();
    window.location.href = `/Application/AddSelfDenial?applicationNumber=${el}`;
}
function ApplicationUpdate(el) {
    event.stopImmediatePropagation();
    window.location.href = `/Application/ApplicationUpdate?applicationNumber=${el}`;
}

function toPayment(url) {
    event.stopImmediatePropagation();
    window.open(url, "_blank");
}
$(() => {
    initGridView();
})