//-Номер на заявлението
//-Статус на заявлението
//-Наименование на чужденец
//-Дата на раждане на чужденец
//-ЛНЧ
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
                    name: 'status',
                    title: 'Статус',
                    cssClass: 'col-md-2'
                },
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
                    name: 'foreignerLNCH',
                    title: 'ЛНЧ',
                    cssClass:'col-md-2',
                } 
            ]
        }
    });
}

$(() => {
    initGridView();
})