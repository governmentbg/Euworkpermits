function initGridView(){
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
            return {
                ForeignerName: $('#ForeignerName').val(),
                ApplicationNumber: $('#ApplicationNumber').val(),
                Status: $('#Status').val(),
                PermitType: $('#PermitType').val(),
                FromDate: $('#FromDate').val(),
                ToDate: $('#ToDate').val(),
            };
        },
        columns: {
            rowClick: null,
            rowClickArgs: ['gid','regNumber'],
            rowCss: function (row) {
                if (row.isActive == true) {
                    return '';
                }
                return 'grid-row-unpublished';
            },
            items: [
               {
                    name: 'foreignerName',
                    title: 'Име на чужденец',
                    cssClass: 'col-md-4'
                },
                {
                    name: 'entryDate',
                    title: 'От дата',
                    cssClass: 'col-md-2',
                    render: function (el) {
                        return JsonBGDateTime(el);
                    }
                },
                {
                    name: 'permitType',
                    title: 'Разрешение',
                    cssClass:'col-md-3',
                    //render:function(el,row){
                    //    return `${el}, rendered No=${row.incommingNumber}`
                    //}
                },
                {
                    name: 'applicationId',
                    title: '',
                    cssClass: 'col-md-3',
                    render:function(el,row){
                        return `<div class="form__footer">` +
                            `<button type="button" class="btn u-btn u-btn--close u-btn--right" data-toggle="tooltip" title="Изтриване" onclick="SetStatusNone('${el}')" ></button>` +
                            `<button class="btn u-btn u-bg--c6 u-btn--arrow u-btn--right" onclick="EditApplication('${el}')">Продължи</button>` +
                            `</div>`;
                    }
                }

            ]
        }
    });
}

function EditApplication(el) {
    window.location.href = `/Application/Edit?applicationId=${el}`;
}

async function SetStatusNone(applicationId) {
    const responce = await fetch('/Application/SetStatusNone?' + new URLSearchParams({ applicationId }));
    if (await ResolveIsOkResponce(responce)) {
        gridViewLoadData('#dvMain');
    }
}


$(() => {
    initGridView();
})