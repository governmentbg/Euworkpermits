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
            return {};
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
                    name: 'applicationNumber',
                    title: 'Към заявление',
                    cssClass: 'col-md-2'
                },
                {
                    name: 'complaintName',
                    title: 'Име на подател',
                    cssClass: 'col-md-3'
                },
                {
                    name: 'status',
                    title: 'Статус',
                    cssClass: 'col-md-3'
                },
                {
                    name: 'statusDate',
                    title: 'От дата',
                    cssClass: 'col-md-2',
                    render: function (el) {
                        return JsonBGdate(el);
                    }
                },

            ]
        }
    });
}

$(() => {
    initGridView();
})