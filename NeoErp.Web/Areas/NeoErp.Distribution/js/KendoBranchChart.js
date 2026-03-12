var kendoBranchCharts = function () {
    var dataProvider = [];
    var mainUrl = window.location.protocol + "//" + window.location.host + "/api/SalesDashboard/GetSalesMonthlyReport";
    var chartDiv = "MonthlyBranchchart";
    var currentChart = "";
    var currentType = "";
 
    return {
        init: function () {

          //  createChart();

        },
        createChart: function (charttype) {
            //console.log(ReportFilter.filterAdditionalData());
            var mainUrl = window.location.protocol + "//" + window.location.host + "/api/SalesDashboard/GetSalesBranchMonthlyReport";
            var daywiseUrl = window.location.protocol + "//" + window.location.host + "/api/SalesDashboard/GetSalesBranchDailyReport";
            var dataSource = new kendo.data.DataSource({
                transport: {
                    read: {
                        url: mainUrl, // <-- Get data from here.
                        dataType: "json", // <-- The default was "jsonp".
                        type: "POST",
                        contentType: "application/json; charset=utf-8"
                    },
                    parameterMap: function (options, type) {
                        var paramMap = JSON.stringify($.extend(options, ReportFilter.filterAdditionalData()));
                        delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                        delete paramMap.$format; // <-- remove format parameter.
                        return paramMap;
                    }
                },
                sort: {
                    field: "Month",
                    dir: "asc"
                },
                group: {
                    field: "BranchName"
                },
            });
         
            $("#MonthlyBranchchart").kendoChart({
                chartArea: {
                    // width: 200,
                    height: 300
                },
                onDrag: function (e) {
                    //
                    var chart = e.sender;
                    var ds = chart.dataSource;
                    var options = chart.options;
    
                    var categoryRange = e.axisRanges.CategoryAxis;
    
                    if (categoryRange) {
                        var xMin = categoryRange.min;
                        var xMax = categoryRange.max;
      
                        options.categoryAxis.min = xMin;
                        options.categoryAxis.max = xMax;
      
                        ds.filter(viewModel.getFilter(xMin, xMax));
                    }
                },
                zoomable: {
                    mousewheel: {
                        lock: "y"
                    },
                    selection: {
                        lock: "y"
                    }
                },
                dataSource: dataSource,
                title: {
                    //text: "Monthly Branch Sales Report"
                },
                legend: {
                    position: "bottom"
                },
                seriesDefaults: {
                    type: "line",
                    labels: {
                        visible: false,
                        template: "#= category #: \n #= value#"
                    },                    
                },
                valueAxis: {
                    line: {
                        visible: true
                    },
                    labels: {
                        rotation: 'auto',
                        template: "#= FormatLabels(value)#"
                    }
                },
                series: [{
                    field: "Amount",
                    name: "#= group.value #"
                }],
                seriesColors: ["#428bca", "#da3b36", "#e67d4a", "#5bc0de", "#5cb85c", "#f2b661"],
                categoryAxis: {
                    field: "Month",
                    labels: {
                        rotation: -45,
                    }
                },
                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#= FormatLabels(value)#"
                },
                dataBound: function (e) {
                    var data = e.sender.dataSource.view();
                    var fieldtype = e.sender.options.series[0].field
                    var max = 0;
                    //
                    if (fieldtype == "Amount") {

                        jQuery.map(data, function (obj) {
                            jQuery.map(obj.items, function (objitem) {
                                if (objitem.Amount > max)
                                    max = objitem.Amount;
                            })
                            e.sender.options.valueAxis.majorUnit = 20000000;
                        });
                    } else {

                        jQuery.map(data, function (obj) {
                            jQuery.map(obj.items, function (objitem) {
                                if (objitem.Quantity > max)
                                    max = objitem.Quantity;
                            })

                        });
                      //var  maxAxis = max + (max / 100 * 10);
                      // var axisDivision = maxAxis / 10;
                       e.sender.options.valueAxis.majorUnit = 5000;
                    }


                    e.sender.options.valueAxis.max = max + e.sender.options.valueAxis.majorUnit;
                 
                },
                seriesClick: function (e) {
                    var boolvalue = !isNaN(parseFloat(e.category)) && isFinite(e.category);
                    if (boolvalue)
                        return false;
                    var dataSourcetest = new kendo.data.DataSource({
                        transport: {
                            read: {
                                url: daywiseUrl + "?monthName=" + e.category+ "&branchName="+e.series.name,
                                dataType: "json"
                            }
                        },
                        sort: {
                            field: "day",
                            dir: "asc"
                        },
                        group: {
                            field: "BranchName"
                        },
                    });
                    var chart = $("#MonthlyBranchchart").data("kendoChart");
                    chart.setDataSource(dataSourcetest);
                    type = $("input[name=seriesType]:checked").val(),
                     chart.options.series[0].field = "Quantity";
                    chart.options.categoryAxis.field = "day";
                    chart.dataSource.read();
                    chart.refresh();
                    //kendoConsole.log(kendo.format("Series click :: {0} ({1}): {2}",
                    //    e.series.name, e.category, e.value));
                },
            });
        },

       
        getDataProvider: function () {
            return dataProvider;
        }

    };
}();