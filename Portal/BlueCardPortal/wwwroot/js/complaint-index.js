function initGridView(){
    const loadUrl = $('#dvMain').data("url");
    if ($('#dvMain').hasClass('gridview-container')) {
        gridViewLoadData('#dvMain');
        return;
    }

    let mainTable = new GridView({
        container: '#dvMain',
        grid_title: 'Жалби',
        url: loadUrl,
        data: function () {
            return GetComplaintFilter();
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
                    name: 'complaintNumber',
                    title: 'Номер',
                    cssClass: 'col-md-2'
                },
                {
                    name: 'status',
                    title: 'Статус на жалба',
                    cssClass: 'col-md-2'
                },
                {
                    name: 'applicationNumber',
                    title: 'Към заявление',
                    cssClass: 'col-md-2'
                },
                {
                    name: 'foreignerName',
                    title: 'Име на чужденец',
                    cssClass: 'col-md-2'
                },
                {
                    name: 'foreignerBirthDate',
                    title: 'Дата на раждане',
                    cssClass: 'col-md-2',
                },
                {
                    name: 'foreignerLNCH',
                    title: 'ЛНЧ',
                    cssClass: 'col-md-2',
                } 

            ]
        }
    });
}

$(() => {
    initGridView();
})



function clearComplaintFilter() {
    $('#ComplaintNumber').val('');
    $('#FromDate').val('');
    $('#ToDate').val('');
    searchApplication();
}

function GetComplaintFilter() {
    return {
        ComplaintNumber: $('#ComplaintNumber').val(),
        FromDate: $('#FromDate').val(),
        ToDate: $('#ToDate').val(),
    };
}
function searchComplaint() {
    gridViewLoadData('#dvMain')
}
