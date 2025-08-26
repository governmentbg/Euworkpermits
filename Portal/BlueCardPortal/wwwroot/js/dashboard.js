function GetDashBoardFilterData() {
    return {
        year: $('#Year').val(),
        yearCountry: $('#YearCountry').val(),
        yearCountryTop10: $('#YearCountryTop10').val(),
        recapFromYear: $('#recapFromYear').val(),
        recapToYear: $('#recapToYear').val(),
        countries: $("#Countries").val(),
        //  permitTypes: $("#PermitTypes").val(),

    }
}

async function ShowDashboard(showType) {
    const resuest = GetDashBoardFilterData()
    const data = await post_fetch_json_async('/Home/PermitsSeries', resuest);
    if (!showType || showType == "pie") {
        await PermitPie(data.permitData);
    }
    if (!showType || showType == "countryTop10") {
        await PermitOnCountryTop10(data.countryTop10Data);
    }
    if (!showType || showType == "year") {
        await PermitOnYear(data.yearData);
    }
    if (!showType || showType == "country") {
        await PermitOnCountry(data.countryData, data.countryMaxPermit);
    }
}

async function PermitPie(seriesData)
{
    Highcharts.chart('container', {
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            text: '',
            align: 'left'
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%({point.y} бр.)</b>'
        },
        accessibility: {
            point: {
                valueSuffix: '%'
            }
        },
        plotOptions: {
            series: {
                allowPointSelect: true,
                cursor: 'pointer',
                //dataLabels: {
                //    enabled: false
                //},
                dataLabels: [
                    {
                       enabled: false,
                    },
                    {
                    enabled: true,
                    distance: 20,
                        format: '{point.percentage:.1f}% ({point.y} бр.)',
                    style: {
                        fontSize: '0.8em',
                        textOutline: 'none',
                        opacity: 0.7
                    },
                    filter: {
                        operator: '>',
                        property: 'percentage',
                        value: 10
                    }
                 }],
                showInLegend: true

            }
        },
        series: [{
            name: 'Разрешения',
            colorByPoint: true,
            data: seriesData
        }]
    });
}
function PermitOnCountryOne(seriesData, maxValue) {
    const chart = `containerCountry${seriesData.page}`;
    $('.highcharts-on-country').append(`<div id="${chart}"></div>`);
    Highcharts.chart(chart, {
        chart: {
            type: 'column'
        },
        title: {
            text: '',
            align: 'left'
        },
        xAxis: {
            categories: seriesData.categories,
            crosshair: true,
            accessibility: {
                description: 'Countries'
            }
        },
        yAxis: {
            min: 0,
            max: maxValue, 
            title: {
                text: 'Издадени разрешения'
            }
        },
        tooltip: {
            valueSuffix: ' бр.'
        },
        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0
            }
        },
        series: seriesData.series,
        legend: {
            enabled: true
        },
    });
}

function PermitOnCountryTop10(seriesData) {
    const chart = `containerCountryTop10`;
    $('.highcharts-on-country-top10').append(`<div id="${chart}"></div>`);
    Highcharts.chart(chart, {
        chart: {
            type: 'column'
        },
        title: {
            text: '',
            align: 'left'
        },
        xAxis: {
            categories: seriesData.categories,
            crosshair: true,
            accessibility: {
                description: 'Countries'
            }
        },
        yAxis: {
            min: 0,
            title: {
                text: 'Издадени разрешения'
            }
        },
        tooltip: {
            valueSuffix: ' бр.'
        },
        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0
            },
            series: {
                dataLabels: {
                    enabled: true,
                    formatter: function () {
                        return this.y > 0 ? this.y : "";
                    }
                }
            }
        },
        series: seriesData.series,
    });
}

function PermitOnYear(seriesData) {
    const chart = `containerOnYear`;
    $('.highcharts-on-year').append(`<div id="${chart}"></div>`);
    Highcharts.chart(chart, {
        chart: {
            type: 'column'
        },
        title: {
            text: '',
            align: 'left'
        },
        xAxis: {
            categories: seriesData.categories,
            crosshair: true,
            accessibility: {
                description: 'Countries'
            }
        },
        yAxis: {
            min: 0,
            title: {
                text: 'Издадени разрешения'
            }
        },
        tooltip: {
            valueSuffix: ' бр.'
        },
        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0
            },
            series: {
                dataLabels: {
                    enabled: true,
                    formatter: function () {
                       return this.y > 0 ? this.y : "";
                    }
                }
            }
        },
        series: seriesData.series,
     });
}


async function PermitOnCountry(seriesData, maxValue) {
    $('.highcharts-on-country').html('');
    seriesData.forEach(function (value) {
        PermitOnCountryOne(value, maxValue)
    });
}

$(async function () {
    await ShowDashboard();
    $('#Year').on("change", async function () {
        await ShowDashboard("pie");
    })
    $('#YearCountryTop10').on("change", async function () {
        await ShowDashboard("countryTop10");
    })
    $('#YearCountry').on("change", async function () {
        await ShowDashboard("country");
    })
    $('#Countries').on("change", async function () {
        await ShowDashboard("country");
    })
    $('#recapFromYear').on("change", async function () {
        if ($('#recapFromYear').val() > $('#recapToYear').val()) {
            $('#recapToYear').val($('#recapToYear option:last-child').val());
        }
        await ShowDashboard("year");
    })
    $('#recapToYear').on("change", async function () {
        if ($('#recapFromYear').val() > $('#recapToYear').val()) {
            $('#recapFromYear').val($('#recapFromYear option:first-child').val());
        }
        await ShowDashboard("year");
    })
});