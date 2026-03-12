
var kendoCharts = function () {
    var dataProvider = [];
    var mainUrl = window.location.protocol + "//" + window.location.host + "/api/SalesDashboard/GetSalesMonthlyReport";
    var chartDiv = "Monthlychart";
    var currentChart = "";
    var currentType = "";
    var dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: mainUrl,
                dataType: "json"
            }
        },
        //sort: {
        //    //field: "Month",
        //    field:sortAxis_monthlySales,
        //    dir: "asc"
        //}
    });
    return {
        init: function () {
            createChart("");
            monthlySalesChart
        },

        createChart: function (urlData) {
            urlData = (urlData == undefined) ? "?customerCode=&itemCode=&categoryCode=&companyCode=&branchCode=" + branchCodes + "&partyTypeCode=&formCode=" : urlData.replace('?', '&');
            dayWiseUrl_monthlySalesChart = urlData.replace('?', '&');
            var catagoryAxisName_monthlySales = "MonthYear", sortAxis_monthlySales = "MonthInt";
            if ($("input[name=DateFormat_monthlySales]:checked").val() == "BS") {
                catagoryAxisName_monthlySales = "Nepalimonth";                
            }

            var dateformat = $("input[name=DateFormat_monthlySales]:checked").val();
            var mainUrl = window.location.protocol + "//" + window.location.host + "/api/SalesDashboard/GetSalesMonthlyReport";
            var daywiseUrl = window.location.protocol + "//" + window.location.host + "/api/SalesDashboard/GetSalesDailyReport";
            var IsDataGrid = $('.newbtnDatagrid').data("type");
            reportFilter = reportFilter == undefined ? ReportFilter.filterAdditionalData() : reportFilter;
            var dataSource = new kendo.data.DataSource({
                transport: {
                    read: {
                        //url: mainUrl, // <-- Get data from here.
                        url: mainUrl + "?DateFormat=" + dateformat + urlData.replace('?', '&'),
                        dataType: "json", // <-- The default was "jsonp".
                        type: "POST",
                        contentType: "application/json; charset=utf-8"
                    },
                    parameterMap: function (options, type) {
                        var paramMap = JSON.stringify($.extend(options, reportFilter));
                        delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                        delete paramMap.$format; // <-- remove format parameter.
                        return paramMap;
                    }
                },
                requestStart: function () {
                    if (IsDataGrid != "Grid") {
                        kendo.ui.progress($("#Monthlychart"), true);
                    }
                },
                requestEnd: function () {
                    kendo.ui.progress($("#Monthlychart"), false);
                },
                schema: {
                    parse: function (items) {
                        var item,
                            result = [];
                        var showzerovalue = $("#showzerovalue_monthlySales").prop("checked");

                        if (showzerovalue) {
                            for (var i = 0; i < items.length; i++) {
                                item = items[i];
                                result.push(item);
                            }
                        }
                        else {
                            for (var i = 0; i < items.length; i++) {
                                item = items[i];
                                if (item.Amount !== 0 && item.Quantity !== 0) {
                                    result.push(item);
                                }
                            }
                        }


                        return result;
                    }
                }
            });
            // data-grid set option on ceate funciton
            $("#monthlySalesChart .DisplayFilterContent a").text("");
            ;

            if (IsDataGrid == "Grid") {
                LoadDataGrid_monthlySalesChart(dataSource);
                return;
            }

            $("#Monthlychart").kendoChart({
                chartArea: {
                    height: ($("#monthlySalesChart").hasClass("portlet-fullscreen")) ? 550 : 350,
                },
                onDrag: function (e) {
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

                pannable: {
                    lock: "y"
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
                    //text: "Monthly Sales Report"
                },
                legend: {
                    position: "bottom"
                },
                seriesColors: ["#428bca", "#da3b36", "#e67d4a", "#5bc0de", "#5cb85c", "#f2b661"],
                seriesDefaults: {
                    type: $("input[name=seriesType]:checked").val(),
                    labels: {
                        visible: $("#showlables_monthlySales").prop("checked"),
                        template: "#= FormatLabels(value)#",
                        rotation: '270',
                        padding: {
                            left: -5
                        },
                    },
                    style: "smooth",
                },
                valueAxis: {
                    line: {
                        visible: true
                    },
                    labels: {
                        rotation: 'auto',
                        template: "#= FormatLabels(value)#"
                    },
                    title: { text: amountFigureType }
                },

                series:
                [{
                    field: "Amount",
                    name: "Amount"
                }],
                categoryAxis: {
                    //field: "Month",
                    field: catagoryAxisName_monthlySales,
                    labels: {
                        rotation: -45,
                    }
                },
                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#= category #: \n #= FormatLabels(value)#"
                },
                dataBound: function (e) {
                    var maxValue = getMaxValueFromDataSource(e);
                    var majorunit = Math.max(6, maxValue / 6);
                    e.sender.options.valueAxis.majorUnit = majorunit;
                    e.sender.options.valueAxis.max = maxValue + majorunit;
                    $("#Monthlychart").next(".overlay").toggle(maxValue <= 0);

                },
                seriesClick: function (e) {
                     
                    $('.newbtnDatagrid').data('valuefor', 'day');
                    $('.newbtnDatagrid').data('category', e.category);
                    var dateformat = $("input[name=DateFormat_monthlySales]:checked").val();
                    e.dataItem.Month = dateformat == "AD" ? e.dataItem.Month : e.dataItem.Nepalimonth;
                    if (e.dataItem.MonthNo != undefined) {
                        if (e.dataItem.Month != '' && e.dataItem.MonthNo != '') {
                            return false;
                        }
                    }
                    if (e.dataItem.Month == undefined || e.dataItem.Month == null || e.dataItem.day != undefined)
                        return false;
                    var dataSourcetest = new kendo.data.DataSource({
                        transport: {
                            read: {
                                url: daywiseUrl + "?monthName=" + e.dataItem.NepaliMonthInt + urlData.replace('?', '&') + "&DateFormat=" + dateformat,
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
                        requestStart: function () {
                            kendo.ui.progress($("#Monthlychart"), true);
                        },
                        requestEnd: function () {
                            kendo.ui.progress($("#Monthlychart"), false);
                        },
                        sort: {
                            field: "day",
                            dir: "asc"
                        }
                    });
                    kendoCharts.LoadDayWiseMonthlySalesChart(dataSourcetest);
                    //LoadDayWiseMonthlySalesChart(dataSourcetest);
                },
            });
        },

        LoadDayWiseMonthlySalesChart: function (dataSourcetest, urlData) {
            var category = $('.newbtnDatagrid').data('category');
            var month = $('.newbtnDatagrid').data('month');
            var dateformat = $("input[name=DateFormat_monthlySales]:checked").val();
             
            if (dataSourcetest.data().length > 0) {
                if (!('day' in dataSourcetest.data()[0])) {
                    var daywiseUrl = window.location.protocol + "//" + window.location.host + "/api/SalesDashboard/GetSalesDailyReport";
                    urlData = (urlData == undefined) ? "?customerCode=&itemCode=&categoryCode=&companyCode=&branchCode=" + branchCodes + "&partyTypeCode=&formCode=" : urlData.replace('?', '&');
                    dataSourcetest = new kendo.data.DataSource({
                        transport: {
                            read: {
                                url: daywiseUrl + "?monthName=" + month + urlData.replace('?', '&') + "&DateFormat=" + dateformat,
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
                        requestStart: function () {
                            kendo.ui.progress($("#Monthlychart"), true);
                        },
                        requestEnd: function () {
                            kendo.ui.progress($("#Monthlychart"), false);
                        },
                        sort: {
                            field: "day",
                            dir: "asc"
                        }
                    });
                }
            }

            $("#Monthlychart").kendoChart({
                chartArea: {
                    height: ($("#Monthlychart").hasClass("portlet-fullscreen")) ? 550 : 350,
                },
                onDrag: function (e) {
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
                pannable: {
                    lock: "y"
                },
                zoomable: {
                    mousewheel: {
                        lock: "y"
                    },
                    selection: {
                        lock: "y"
                    }
                },
                dataSource: dataSourcetest,
                title: {
                },
                legend: {
                    position: "bottom"
                },
                seriesDefaults: {
                    type: $("input[name=seriesType]:checked").val(),
                    style: "smooth",
                    labels: {
                        visible: $("#showlables_monthlySales").prop("checked"),
                        template: "#= value#"
                    },
                },
                valueAxis: {
                    line: {
                        visible: true
                    },
                    labels: {
                        rotation: 'auto',
                        template: "#= FormatLabels(value)#"
                    },
                    title: { text: amountFigureType }
                },
                series: [{
                    field: "Amount",
                    //name: "#= group.value #"
                    name: category
                }],
                seriesColors: ["#428bca", "#da3b36", "#e67d4a", "#5bc0de", "#5cb85c", "#f2b661"],
                categoryAxis: {
                    field: "day",
                    labels: {
                        rotation: -45,
                    }
                },
                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "day #= category #: \n #= FormatLabels(value)#"
                },

            });
            //var chart = $("#Monthlychart").data("kendoChart");
            //type = $("input[name=seriesType]:checked").val(),
            //category = $('.newbtnDatagrid').data('category'),
            //chart.options.series[0].field = "Amount";
            ////chart.options.categoryAxis.field = "day";
            //chart.setOptions({
            //    seriesColors: ["#5cb85c", "#da3b36", "#e67d4a", "#5bc0de", "#428bca", "#f2b661"],
            //    categoryAxis: {
            //        field: "day",
            //        labels: {
            //            rotation: -45,
            //        },
            //        title: { text: category }
            //    },
            //    tooltip: {
            //        template: "day #= category #: \n #= FormatLabels(value)#"
            //    },
            //});
            //chart.setDataSource(dataSourcetest);
            //chart.refresh();
        },

        refreshChart: function () {

            var chart = $("#Monthlychart").data("kendoChart"),
            type = $("input[name=seriesType]:checked").val(),
            fieldtype = $("input[name=fieldType]:checked").val(),
            labels = $("#showlables").prop("checked");

            chart.options.series[0].type = type;
            chart.options.series[0].labels.visible = labels;
            if (type = "column") {

                for (var i = 0, length = chart.options.series.length; i < length; i++) {
                    chart.options.series[i].labels.position = "top";
                    chart.options.series[i].labels.rotation = -90;
                };


            }
            if (type == "pie") {
                chart.options.series[0].categoryField = "Month";
            }
            if (fieldtype == "Quantity") {
                chart.options.series[0].field = "Quantity";
            }
            //chart.setOptions({
            //    series: series
            //});           
            chart.refresh();
        },
        getDataProvider: function () {
            return dataProvider;
        }

    };
}();