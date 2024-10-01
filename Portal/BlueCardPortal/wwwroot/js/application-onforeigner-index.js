//Резултати:
//-Наименование на чужденец
//-Дата на раждане
//-Национална идентичност
//-Подадени заявления – номер на заявлението
//- Статус на заявлението
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
                    name: 'foreignerName',
                    title: 'Име на чужденец',
                    cssClass: 'col-md-4'
                },
                {
                    name: 'foreignerBirthDate',
                    title: 'Дата на раждане',
                    cssClass: 'col-md-2',
                 },
                {
                    name: 'foreignerNationality',
                    title: 'Национална идентичност',
                    cssClass:'col-md-2',
                }, 
                {
                    name: 'applicationNumber',
                    title: 'Номер на заявление',
                    cssClass: 'col-md-2'
                },
                {
                    name: 'status',
                    title: 'Статус',
                    cssClass: 'col-md-2'
                },
            ]
        }
    });
}

async function rowClick(applicationId) {
    const responce = await fetch('/ApplicationPreview/GetApplicationRemote?' + new URLSearchParams({ applicationId }));
    const view = await responce.text();
    ShowModalDialog("Заявление", view, false, true);
}

function clearApplicationOnStatusFilter() {
    $('#ForeignerName').val('');
    $('#ApplicationNumber').val('');
    $('#Status').val('');
    $('#PermitType').val('');
    $('#FromDate').val('');
    $('#ToDate').val('');
}

$(() => {
    initGridView();
})