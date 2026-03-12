var AsOnChart = (function (asOnChart, $) {
    "use strict";

    asOnChart.DebtorsChart = function () {
        debugger;
       // var amountFigureType = "Percentage";
        var amountFigureType = "Amount";
        var config = {
            debtorchart: "-chart",
            serverpageid: "",
            gridtochart: ".newbtnDatagrid",
            portlet: ".portlet",
            reload: ".reload",
            configbtn: ".config",
            arrowdownIcon: ".icon-arrow-down",
            fullscreenIcon: ".fullscreen",
            downloadmenu: ".dropdown-menu",
            AgeOnType: "",
        };
        var viewFunction = {
            serverpageObservable: ko.observable(),
            serverpageGird: ko.observable(),
            seriesType: ko.observable('column'),
            showLables: ko.observable(false),
            seriesTypeValue: ko.observable(),
            iconChangeChart: ko.observable("fa fa-table"),
            getCodeFilters: function (filtertype) {

                var codeIds = [];
                if (filtertype === "Customer") {
                    var multiselect = $("#customerMultiSelect_" + viewFunction.serverpageObservable()).data("kendoMultiSelect");
                    if (multiselect != undefined) {
                        var items = multiselect.dataItems();
                        for (var i = 0; i < items.length; i++) {
                            codeIds.push(items[i].CustomerCode);
                        }
                    }
                   
                }
                else if (filtertype == "Supplier") {
                    var treeview = $("#supplierTreeView_" + viewFunction.serverpageObservable()).data("kendoTreeView");
                    var items = $("#supplierTreeView .k-item input[type=checkbox]:checked").closest(".k-item");
                    $(items).each(function () {
                        codeIds.push(treeview.dataSource.getByUid($(this).attr("data-uid")).MasterSupplierCode);
                    });
                }

                return codeIds;
            },
            getAreaFilters: function () {
                var areaIds = [];

                //var multiselect = $("#areaMultiSelects_" + viewFunction.serverpageObservable()).data("kendoMultiSelect");;
                //var items = multiselect.dataItems();
                //for (var i = 0; i < items.length; i++) {
                //    areaIds.push(items[i].AREA_CODE);
                //}
                return areaIds;
            },
            GetFormatlongNumber: function(value) {
                if (value == 0) {
                    return 0;
                }
                else {
                    // for testing
                    //value = Math.floor(Math.random()*1001);

                    // hundreds
                    if (value <= 999) {
                        return value;
                    }
                    // thousands
                    else if (value >= 1000 && value <= 999999) {
                        return (value / 1000).toFixed(2) + ' K';
                    }
                    // millions
                    else if (value >= 1000000 && value <= 999999999) {
                        return (value / 1000000).toFixed(2) + ' M';
                    }
                    // billions
                    else if (value >= 1000000000 && value <= 999999999999) {
                        return (value / 1000000000).toFixed(2) + ' B';
                    }
                    else
                        return value;
                }
            },
            filters: function () {
                return {

                    AsOnDate: $('#datePickerAd_' + viewFunction.serverpageObservable()).val(),
                    FrequencyInDay: 30,
                    FixedInDay: 120,
                    BillWiseOrLedgerWise:'BillWise',
                    //Type: config.AgeOnType,
                    Codes: viewFunction.getCodeFilters('Customer'),
                    Area: viewFunction.getAreaFilters()
                }
            },
            dataSource: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: window.location.protocol + "//" + window.location.host + "/api/AgeingReport/GetAgeingChartReportBranchwise",
                        dataType: "json", // <-- The default was "jsonp".
                        type: "POST",
                        contentType: "application/json; charset=utf-8"
                    },
                    parameterMap: function (options, type) {
                        var paramMap = JSON.stringify($.extend(options, viewFunction.filters()));
                        delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                        delete paramMap.$format; // <-- remove format parameter.
                        return paramMap;
                    }
                },
                group: {
                    field: "ColumnRangeName"
                },
                serverSorting: true,
                sort: { field: "OrderBy", dir: "asc" },
                requestStart: function () {

                    kendo.ui.progress($('#' + viewFunction.serverpageObservable() + config.debtorchart), true);

                },
                requestEnd: function () {
                    kendo.ui.progress($('#' + viewFunction.serverpageObservable() + config.debtorchart), false);
                },
            }),
            createChart: function (urlData) {


                $('#' + viewFunction.serverpageObservable() + config.debtorchart).kendoChart({
                    chartArea: {
                        height: ($(config.debtorchart).hasClass("portlet-fullscreen")) ? 550 : 350,
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
                    dataSource: viewFunction.dataSource,
                    title: {
                        //text: "Monthly Sales Report"
                    },
                    legend: {
                        position: "bottom"
                    },
                    seriesColors: ["#00ff04", "#00ff61", "#007bff", "#ff003b", "#ff00f2", "#00e5ff"],
                    seriesDefaults: {
                       // stack: true,
                        type: viewFunction.seriesType(),
                        labels: {
                            visible: viewFunction.showLables(),
                            template: "#= GetFormatlongNumber(value)#",
                            rotation: '270',
                            padding: {
                                left: -5
                            },
                        },
                        style: "smooth",
                    },
                    valueAxis: {
                        //max: 100,
                        //majorUnit: 1,
                        line: {
                            visible: true
                        },
                        labels: {
                            rotation: 'auto',
                            template: "#= GetFormatlongNumber(value)#"
                        },
                        title: { text: amountFigureType }
                    },

                    series:
                    [{
                        field: "NetAmount",
                        categoryField: "Branch",
                       // colorField: 'colorCode',
                       
                        name: "#= group.value #"
                    }],
                    categoryAxis: {
                        //field: "Branch",
                        colorField: 'colorCode',
                        labels: {
                            rotation: -45,
                        }
                    },
                    tooltip: {
                        visible: true,
                        format: "{0}",
                        template: "#= series.name # :\n #= FormatLabels(value)#"
                    },
                    dataBound: function (e) {
                        var data = e.sender.dataSource.view();
                        console.log(data);
                        //e.sender.options.valueAxis.max = 150;
                        var isData = false;
                        var fieldtype = e.sender.options.series[0].field
                        var max = 0;
                        if (fieldtype === "NetAmount") {

                            jQuery.map(data, function (obj) {

                                jQuery.map(obj.items, function (test) {
                                    if (test.NetAmount > max) {
                                        isData = true;
                                        max = test.NetAmount;
                                    }
                                });
                               

                            });
                            var majorunit = Math.max(6, max / 6);
                            e.sender.options.valueAxis.majorUnit = majorunit;

                            e.sender.options.valueAxis.max = max + majorunit;
                        } else {

                            jQuery.map(data, function (obj) {
                                if (obj.Quantity > max)
                                    max = obj.Quantity;
                            });
                            var majorunit = Math.max(6, max / 6);
                            e.sender.options.valueAxis.majorUnit = majorunit;

                            e.sender.options.valueAxis.max = max + majorunit;
                        }
                       
                        if (data.length == 0)
                            $(".overlay").toggle(isData === false);


                    },
                });
            },
            relaodClick: function () {
                var urlData = "";
                viewFunction.createChart(urlData);
            },
            configClick: function () {
                $(this).closest(config.portlet).css('zIndex', 10040);
            },
            dowloadClick: function () {
                $(this).closest(config.portlet).css('zIndex', 10040);
            },
            fullScreenClick: function () {
                if ($(this).hasClass("on")) {
                    $('#' + viewFunction.serverpageObservable() + config.debtorchart).data("kendoChart").options.chartArea.height = 350;
                }
                else {
                    $(this).closest(config.portlet).css('zIndex', 15000);
                    $('#' + viewFunction.serverpageObservable() + config.debtorchart).data("kendoChart").options.chartArea.height = 550;
                }
                $('#' + viewFunction.serverpageObservable() + config.debtorchart).data("kendoChart").dataSource.read();
                $('#' + viewFunction.serverpageObservable() + config.debtorchart).data("kendoChart").refresh();
            },
            showLabelClick: function () {
                var chart = $('#' + viewFunction.serverpageObservable() + config.debtorchart).data("kendoChart");
                for (var i = 0, length = chart.options.series.length; i < length; i++) {
                    chart.options.series[i].type = viewFunction.seriesType();
                    chart.options.series[i].labels.visible = viewFunction.showLables();
                    if (viewFunction.seriesType() == "column") {
                        chart.options.series[i].labels.rotation = -90;
                    }
                    if (viewFunction.seriesType() == "bar") {
                        chart.options.series[i].labels.rotation = 0;
                    }
                };
                chart.refresh();
            },
            showSeriesClick: function () {
                var chart = $('#' + viewFunction.serverpageObservable() + config.debtorchart).data("kendoChart");
                for (var i = 0, length = chart.options.series.length; i < length; i++) {
                    chart.options.series[i].type = viewFunction.seriesType();
                    chart.options.series[i].labels.visible = viewFunction.showLables();
                    if (viewFunction.seriesType() == "column") {
                        chart.options.series[i].labels.rotation = -90;
                    }
                    if (viewFunction.seriesType() == "bar") {
                        chart.options.series[i].labels.rotation = 0;
                    }
                };

                chart.refresh();
            },
            changeGridToChart: function () {

                if (viewFunction.iconChangeChart() === "icon-bar-chart") {
                    viewFunction.iconChangeChart("fa fa-table");
                    $('#' + viewFunction.serverpageObservable() + config.debtorchart).html('');
                    viewFunction.createChart('');

                } else {
                    $('#' + viewFunction.serverpageObservable() + config.debtorchart).html('');

                    viewFunction.creatGrid();
                    viewFunction.iconChangeChart("icon-bar-chart");
                }

            },
            creatGrid: function () {
                $('#' + viewFunction.serverpageObservable() + config.debtorchart).kendoGrid({
                    dataSource: viewFunction.dataSource,
                    height: ($('#' + viewFunction.serverpageObservable() + config.debtorchart).hasClass("portlet-fullscreen")) ? 450 : 350,
                    groupable: false,
                    sortable: true,
                    selectable: "multiple",
                    columns: [{

                        field: "ColumnRangeName",
                        title: "Range"
                    },
                    {

                        field: "NetAmount",
                        title: "Amount",
                        format: "{0:n}"
                    }]
                });
            },
        };

        return {
            config: config,
            viewFunction: viewFunction,
            init: function (serverId) {
                ko.applyBindings(viewFunction);
                viewFunction.serverpageObservable(serverId);
                config.serverpageid = serverId;
                viewFunction.createChart('test');
                $('[title="Download"]').tooltip({
                    trigger: 'hover'
                });

            },
            render: function () {
                $("#apply_" + viewFunction.serverpageObservable()).on("click", function () {
                    $('#' + viewFunction.serverpageObservable() + config.debtorchart).data("kendoChart").dataSource.read();
                    //      $('#' + viewFunction.serverpageObservable() + config.debtorchart).data("kendoChart").refresh();

                });
            },

        };
    };
    asOnChart.CreditorsChartAsOn = function () {
        var config = {
            debtorchart: "-chart",
            serverpageid: "",
            gridtochart: ".newbtnDatagrid",
            portlet: ".portlet",
            reload: ".reload",
            configbtn: ".config",
            arrowdownIcon: ".icon-arrow-down",
            fullscreenIcon: ".fullscreen",
            downloadmenu: ".dropdown-menu",
            AgeOnType: "",
        };


        var viewFunction = {
            serverpageObservable: ko.observable(),
            serverpageGird: ko.observable(),
            seriesType: ko.observable('column'),
            showLables: ko.observable(false),
            seriesTypeValue: ko.observable(),
            iconChangeChart: ko.observable("fa fa-table"),
            getCodeFilters: function (filtertype) {

                var codeIds = [];
                if (filtertype === "Customer") {
                    var multiselect = $("#customerMultiSelect_" + viewFunction.serverpageObservable()).data("kendoMultiSelect");
                    if (multiselect != undefined) {
                        console.log(multiselect);
                        var items = multiselect.dataItems();
                        for (var i = 0; i < items.length; i++) {
                            codeIds.push(items[i].CustomerCode);
                        }
                    }
                   
                }
                else if (filtertype = "Supplier") {
                    var treeview = $("#supplierTreeView_" + viewFunction.serverpageObservable()).data("kendoTreeView");
                    var items = $("#supplierTreeView .k-item input[type=checkbox]:checked").closest(".k-item");
                    $(items).each(function () {
                        codeIds.push(treeview.dataSource.getByUid($(this).attr("data-uid")).MasterSupplierCode);
                    });
                }

                return codeIds;
            },
            filters: function () {
                return {

                    AsOnDate: $('#datePickerAd_' + viewFunction.serverpageObservable()).val(),
                    FrequencyInDay: 30,
                    FixedInDay: 120,
                    //Type: config.AgeOnType,
                    Codes: viewFunction.getCodeFilters('Customer'),
                }
            },
            dataSource: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: window.location.protocol + "//" + window.location.host + "/api/AgeingReport/GetAgeingChartReport",
                        dataType: "json", // <-- The default was "jsonp".
                        type: "POST",
                        contentType: "application/json; charset=utf-8"
                    },
                    parameterMap: function (options, type) {
                        var paramMap = JSON.stringify($.extend(options, viewFunction.filters()));
                        delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                        delete paramMap.$format; // <-- remove format parameter.
                        return paramMap;
                    }
                },
                requestStart: function () {

                    kendo.ui.progress($('#' + viewFunction.serverpageObservable() + config.debtorchart), true);

                },
                requestEnd: function () {
                    kendo.ui.progress($('#' + viewFunction.serverpageObservable() + config.debtorchart), false);
                },
            }),
            createChart: function (urlData) {


                $('#' + viewFunction.serverpageObservable() + config.debtorchart).kendoChart({
                    chartArea: {
                        height: ($(config.debtorchart).hasClass("portlet-fullscreen")) ? 550 : 350,
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
                    dataSource: viewFunction.dataSource,
                    title: {
                        //text: "Monthly Sales Report"
                    },
                    legend: {
                        position: "bottom"
                    },
                    //seriesColors: ["#f2b661", "#da3b36", "#e67d4a", "#5bc0de", "#5cb85c", "#f2b661"],
                    seriesDefaults: {
                        type: viewFunction.seriesType(),
                        labels: {
                            visible: viewFunction.showLables(),
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
                        field: "NetAmount",
                        name: "Amount",
                        colorField: 'colorCode',
                    }],
                    categoryAxis: {
                        field: "ColumnRangeName",
                        colorField: 'colorCode',
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
                        var data = e.sender.dataSource.view();
                        
                        var isData = false;
                        var fieldtype = e.sender.options.series[0].field
                        var max = 0;
                        if (fieldtype === "NetAmount") {

                            jQuery.map(data, function (obj) {
                                if (obj.NetAmount > max) {
                                    isData = true;
                                    max = obj.NetAmount;
                                }

                            });
                            var majorunit = Math.max(6, max / 6);
                            e.sender.options.valueAxis.majorUnit = majorunit;

                            e.sender.options.valueAxis.max = max + majorunit;
                        } else {

                            jQuery.map(data, function (obj) {
                                if (obj.Quantity > max)
                                    max = obj.Quantity;
                            });
                            var majorunit = Math.max(6, max / 6);
                            e.sender.options.valueAxis.majorUnit = majorunit;

                            e.sender.options.valueAxis.max = max + majorunit;
                        }

                        $(".overlay").toggle(isData === false);


                    },
                });
            },
            relaodClick: function () {
                var urlData = "";
                viewFunction.createChart(urlData);
            },
            configClick: function () {
                $(this).closest(config.portlet).css('zIndex', 10040);
            },
            dowloadClick: function () {
                $(this).closest(config.portlet).css('zIndex', 10040);
            },
            fullScreenClick: function () {
                if ($(this).hasClass("on")) {
                    $('#' + viewFunction.serverpageObservable() + config.debtorchart).data("kendoChart").options.chartArea.height = 350;
                }
                else {
                    $('#' + viewFunction.serverpageObservable() + config.debtorchart).data("kendoChart").options.chartArea.height = 550;
                }
                $('#' + viewFunction.serverpageObservable() + config.debtorchart).data("kendoChart").dataSource.read();
                $('#' + viewFunction.serverpageObservable() + config.debtorchart).data("kendoChart").refresh();
            },
            showLabelClick: function () {
                var chart = $('#' + viewFunction.serverpageObservable() + config.debtorchart).data("kendoChart");
                for (var i = 0, length = chart.options.series.length; i < length; i++) {
                    chart.options.series[i].type = viewFunction.seriesType();
                    chart.options.series[i].labels.visible = viewFunction.showLables();
                    if (viewFunction.seriesType() == "column") {
                        chart.options.series[i].labels.rotation = -90;
                    }
                    if (viewFunction.seriesType() == "bar") {
                        chart.options.series[i].labels.rotation = 0;
                    }
                };
                chart.refresh();
            },
            showSeriesClick: function () {
                var chart = $('#' + viewFunction.serverpageObservable() + config.debtorchart).data("kendoChart");
                for (var i = 0, length = chart.options.series.length; i < length; i++) {
                    chart.options.series[i].type = viewFunction.seriesType();
                    chart.options.series[i].labels.visible = viewFunction.showLables();
                    if (viewFunction.seriesType() == "column") {
                        chart.options.series[i].labels.rotation = -90;
                    }
                    if (viewFunction.seriesType() == "bar") {
                        chart.options.series[i].labels.rotation = 0;
                    }
                };

                chart.refresh();
            },
            changeGridToChart: function () {

                if (viewFunction.iconChangeChart() === "icon-bar-chart") {
                    viewFunction.iconChangeChart("fa fa-table");
                    $('#' + viewFunction.serverpageObservable() + config.debtorchart).html('');
                    viewFunction.createChart('');

                } else {
                    $('#' + viewFunction.serverpageObservable() + config.debtorchart).html('');

                    viewFunction.creatGrid();
                    viewFunction.iconChangeChart("icon-bar-chart");
                }

            },
            creatGrid: function () {
                $('#' + viewFunction.serverpageObservable() + config.debtorchart).kendoGrid({
                    dataSource: viewFunction.dataSource,
                    height: ($('#' + viewFunction.serverpageObservable() + config.debtorchart).hasClass("portlet-fullscreen")) ? 450 : 350,
                    groupable: false,
                    sortable: true,
                    selectable: "multiple",
                    columns: [{

                        field: "ColumnRangeName",
                        title: "Range"
                    },
                    {

                        field: "NetAmount",
                        title: "Amount",
                        format: "{0:n}"
                    }]
                });
            },
        };

        return {
            config: config,
            viewFunction: viewFunction,
            init: function (serverId, asontype) {
                ko.applyBindings(viewFunction);
                //config.AgeOnType = charttype;
                viewFunction.serverpageObservable(serverId);
                config.serverpageid = serverId;
                viewFunction.createChart('test');
                $('[title="Download"]').tooltip({
                    trigger: 'hover'
                });

            },
            render: function () {
                $("#apply_" + viewFunction.serverpageObservable()).on("click", function () {
                    $('#' + viewFunction.serverpageObservable() + config.debtorchart).data("kendoChart").dataSource.read();
                    //      $('#' + viewFunction.serverpageObservable() + config.debtorchart).data("kendoChart").refresh();

                });
            },

        };
    };

    asOnChart.CreditorsChart = function () {
        var config = {
            debtorchart: "#CredtorsMonthlychart",
            serverpageid: "",
            gridtochart: ".newbtnDatagrid",
            portlet: ".portlet",
            reload: ".reload",
            configbtn: ".config",
            arrowdownIcon: ".icon-arrow-down",
            fullscreenIcon: ".fullscreen",
            downloadmenu: ".dropdown-menu",
            AgeOnType: "",
        };
        $(config.debtorchart).closest(config.portlet).find(config.reload).click(function () {
            var urlData = "";
            alert(config.serverpageid);
            viewFunction.createChart(urlData);
            //RefreshChartFilterControl(config.serverpageid);
            $(config.gridtochart).data('valuefor', 'month');
        });
        $(config.debtorchart).closest(config.portlet).find(config.configbtn).click(function () {
            $(this).closest(config.portlet).css('zIndex', 10040);
        });
        $(config.debtorchart).closest(config.portlet).find(config.arrowdownIcon).click(function () {
            $(this).closest(config.portlet).css('zIndex', 10040);
        });
        $(config.debtorchart).closest(config.portlet).find(config.fullscreenIcon).click(function () {
            if ($(this).hasClass("on")) {
                $(config.debtorchart).data("kendoChart").options.chartArea.height = 350;
            }
            else {
                $(config.debtorchart).data("kendoChart").options.chartArea.height = 550;
            }
            $(config.debtorchart).data("kendoChart").dataSource.read();
            $(config.debtorchart).data("kendoChart").refresh();
        });

        $(config.arrowdownIcon + 'Nincustom').click(function () {
            $(config.downloadmenu + ' custom Ninslidetoggle').slideToggle(200);
        });
        $("#apply_CredtorsOutStandingChart").on("click", function () {
            viewFunction.createChart('');
        });

        var viewFunction = {
            getCodeFilters: function (filtertype) {

                var codeIds = [];
                if (filtertype == "Customer") {
                    var multiselect = $("#customerMultiSelect_CredtorsOutStandingChart").data("kendoMultiSelect");;
                    var items = multiselect.dataItems();
                    for (var i = 0; i < items.length; i++) {
                        codeIds.push(items[i].CustomerCode);
                    }
                }
                else if (filtertype = "Supplier") {
                    var treeview = $("#supplierTreeView" + config.serverpageid).data("kendoTreeView");
                    var items = $("#supplierTreeView .k-item input[type=checkbox]:checked").closest(".k-item");
                    $(items).each(function () {
                        codeIds.push(treeview.dataSource.getByUid($(this).attr("data-uid")).MasterSupplierCode);
                    });
                }

                return codeIds;
            },
            filters: function () {
                return {

                    AsOnDate: $('#datePickerAd_' + config.serverpageid).val(),
                    FrequencyInDay: 30,
                    FixedInDay: 120,
                    Type: config.AgeOnType,
                    Codes: viewFunction.getCodeFilters('Suppiler'),
                }
            },
            createChart: function (urlData) {

                var mainUrl = window.location.protocol + "//" + window.location.host + "/api/AgeingReport/GetAgeingChartReport";
                var IsDataGrid = $(config.gridtochart).data("type");
                reportFilter = viewFunction.filters();
                var dataSource = new kendo.data.DataSource({
                    transport: {
                        read: {
                            url: mainUrl,
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
                            kendo.ui.progress($(config.debtorchart), true);
                        }
                    },
                    requestEnd: function () {
                        kendo.ui.progress($(config.debtorchart), false);
                    },
                });

                if (IsDataGrid === "Grid") {
                    //LoadDataGrid_monthlySalesChart(dataSource);
                    return;
                }

                $(config.debtorchart).kendoChart({
                    chartArea: {
                        height: ($(config.debtorchart).hasClass("portlet-fullscreen")) ? 550 : 350,
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
                    seriesColors: ["#f2b661", "#da3b36", "#e67d4a", "#5bc0de", "#5cb85c", "#f2b661"],
                    seriesDefaults: {
                        type: $("input[name=seriesType]:checked").val(),
                        labels: {
                            //visible: $("#showlables_monthlyDebtors").prop("checked"),
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
                        field: "NetAmount",
                        name: "Amount"
                    }],
                    categoryAxis: {
                        field: "ColumnRangeName",
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
                        var data = e.sender.dataSource.view();
                        var isData = false;
                        var fieldtype = e.sender.options.series[0].field
                        var max = 0;
                        if (fieldtype === "NetAmount") {

                            jQuery.map(data, function (obj) {
                                if (obj.NetAmount > max) {
                                    isData = true;
                                    max = obj.NetAmount;
                                }

                            });
                            var majorunit = Math.max(6, max / 6);
                            e.sender.options.valueAxis.majorUnit = majorunit;

                            e.sender.options.valueAxis.max = max + majorunit;
                        } else {

                            jQuery.map(data, function (obj) {
                                if (obj.Quantity > max)
                                    max = obj.Quantity;
                            });
                            var majorunit = Math.max(6, max / 6);
                            e.sender.options.valueAxis.majorUnit = majorunit;

                            e.sender.options.valueAxis.max = max + majorunit;
                        }

                        $(".overlay").toggle(isData === false);


                    },
                });
            },

        };
        return {
            config: config,
            viewFunction: viewFunction,
            init: function (serverId, charttype) {
                config.serverpageid = serverId;
                config.AgeOnType = charttype;
                viewFunction.createChart('test');

                $('[title="Download"]').tooltip({
                    trigger: 'hover'
                });
            },
            render: function () {

            },

        };
    };

    asOnChart.StockValutionChart = function () {
        var config = {
            debtorchart: "-chart",
            serverpageid: "",
            gridtochart: ".newbtnDatagrid",
            portlet: ".portlet",
            reload: ".reload",
            configbtn: ".config",
            arrowdownIcon: ".icon-arrow-down",
            fullscreenIcon: ".fullscreen",
            downloadmenu: ".dropdown-menu",
            CatagoryCode: 0
        };
        var viewFunction = {

            serverpageObservableStockvalution: ko.observable(),
            serverpageGirdStockvalution: ko.observable(),
            seriesTypeStockvalution: ko.observable('column'),
            showLablesStockvalution: ko.observable(false),
            seriesTypeValueStockvalution: ko.observable(),
            iconChangeChartStockvalution: ko.observable("fa fa-table"),
            selectedSeriesCatagoryiesId: ko.observable("catagory"),
            itemWiseDatasource: ko.observableArray(),


            dataSourceStockvalution: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: window.location.protocol + "//" + window.location.host + "/api/SalesDashboard/GetStockValutions",
                        dataType: "json", // <-- The default was "jsonp".
                        type: "POST",
                        contentType: "application/json; charset=utf-8"
                    },
                    parameterMap: function (options, type) {
                        reportFilter = reportFilter == undefined ? ReportFilter.filterAdditionalData() : reportFilter;
                        var paramMap = JSON.stringify($.extend(options, reportFilter));
                        delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                        delete paramMap.$format; // <-- remove format parameter.
                        return paramMap;


                        //var paramMap = JSON.stringify($.extend(options));
                        //delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                        //delete paramMap.$format; // <-- remove format parameter.
                        //return paramMap;
                    }
                },
                requestStart: function () {

                    kendo.ui.progress($('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart), true);

                },
                requestEnd: function () {
                    kendo.ui.progress($('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart), false);
                },
            }),


            createChartStockvalution: function (urlData) {
                $('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).kendoChart({
                    chartArea: {
                        height: ($(config.debtorchart).hasClass("portlet-fullscreen")) ? 550 : 350,
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
                    dataSource: viewFunction.dataSourceStockvalution,
                    title: {
                        //text: "Monthly Sales Report"
                    },
                    legend: {
                        position: "bottom"
                    },
                    seriesColors: ["#f2b661", "#da3b36", "#e67d4a", "#5bc0de", "#5cb85c", "#f2b661"],
                    seriesDefaults: {
                        type: viewFunction.seriesTypeStockvalution(),
                        labels: {
                            visible: viewFunction.showLablesStockvalution(),
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
                        field: "CatagoryName",
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
                        var data = e.sender.dataSource.view();
                        var isData = false;
                        var fieldtype = e.sender.options.series[0].field
                        var max = 0;
                        if (fieldtype === "Amount") {

                            jQuery.map(data, function (obj) {
                                if (obj.Amount > max) {
                                    isData = true;
                                    max = obj.Amount;
                                }

                            });
                            var majorunit = Math.max(6, max / 6);
                            e.sender.options.valueAxis.majorUnit = majorunit;

                            e.sender.options.valueAxis.max = max + majorunit;
                        } else {

                            jQuery.map(data, function (obj) {
                                if (obj.Quantity > max)
                                    max = obj.Quantity;
                            });
                            var majorunit = Math.max(6, max / 6);
                            e.sender.options.valueAxis.majorUnit = majorunit;

                            e.sender.options.valueAxis.max = max + majorunit;
                        }

                        $(".overlay").toggle(isData === false);


                    },
                    seriesClick: function (e) {
                        var datasourcetest = new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: window.location.protocol + "//" + window.location.host + "/api/SalesDashboard/GetProductStockValutions?catagoryCode=" + e.dataItem.CatagoryCode,
                                    dataType: "json", // <-- The default was "jsonp".
                                    type: "POST",
                                    contentType: "application/json; charset=utf-8"
                                },
                                parameterMap: function (options, type) {
                                    reportFilter = reportFilter == undefined ? ReportFilter.filterAdditionalData() : reportFilter;
                                    var paramMap = JSON.stringify($.extend(options, reportFilter));
                                    delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                                    delete paramMap.$format; // <-- remove format parameter.
                                    return paramMap;


                                    //var paramMap = JSON.stringify($.extend(options));
                                    //delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                                    //delete paramMap.$format; // <-- remove format parameter.
                                    //return paramMap;
                                }
                            },
                            requestStart: function () {

                                kendo.ui.progress($('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart), true);

                            },
                            requestEnd: function () {
                                kendo.ui.progress($('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart), false);
                            },
                        });
                        console.log(e.dataItem.CatagoryCode)
                        viewFunction.selectedSeriesCatagoryiesId(e.dataItem.CatagoryCode);
                        config.CatagoryCode = e.dataItem.CatagoryCode;
                        viewFunction.RefreshSecondLevelChart(datasourcetest);

                    },
                });
            },
            relaodClickStockvalution: function () {
                var urlData = "";
                viewFunction.createChartStockvalution(urlData);
            },
            configClickStockvalution: function () {
                $(this).closest(config.portlet).css('zIndex', 10040);
            },
            dowloadClickStockvalution: function () {
                $(this).closest(config.portlet).css('zIndex', 10040);
            },
            fullScreenClickStockvalution: function () {
                if ($(this).hasClass("on")) {
                    $('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).data("kendoChart").options.chartArea.height = 350;
                }
                else {
                    $('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).data("kendoChart").options.chartArea.height = 550;
                }
                $('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).data("kendoChart").dataSource.read();
                $('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).data("kendoChart").refresh();
            },
            showLabelClickStockvalution: function () {
                var chart = $('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).data("kendoChart");
                for (var i = 0, length = chart.options.series.length; i < length; i++) {
                    chart.options.series[i].type = viewFunction.seriesTypeStockvalution();
                    chart.options.series[i].labels.visible = viewFunction.showLablesStockvalution();
                    if (viewFunction.seriesTypeStockvalution() == "column") {
                        chart.options.series[i].labels.rotation = -90;
                    }
                    if (viewFunction.seriesTypeStockvalution() == "bar") {
                        chart.options.series[i].labels.rotation = 0;
                    }
                };
                chart.refresh();
            },
            showSeriesClickStockvalution: function () {
                var chart = $('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).data("kendoChart");
                for (var i = 0, length = chart.options.series.length; i < length; i++) {
                    chart.options.series[i].type = viewFunction.seriesTypeStockvalution();
                    chart.options.series[i].labels.visible = viewFunction.showLablesStockvalution();
                    if (viewFunction.seriesTypeStockvalution() == "column") {
                        chart.options.series[i].labels.rotation = -90;
                    }
                    if (viewFunction.seriesTypeStockvalution() == "bar") {
                        chart.options.series[i].labels.rotation = 0;
                    }
                };

                chart.refresh();
            },
            changeGridToChartStockvalution: function () {

                if (viewFunction.iconChangeChartStockvalution() === "icon-bar-chart") {
                    viewFunction.iconChangeChartStockvalution("fa fa-table");
                    $('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).html('');
                    viewFunction.createChartStockvalution('');

                } else {
                    $('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).html('');

                    viewFunction.creatGridStockvalution();
                    viewFunction.iconChangeChartStockvalution("icon-bar-chart");
                }

            },
            creatGridStockvalution: function () {
                $('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).kendoGrid({
                    dataSource: viewFunction.dataSourceStockvalution,
                    height: ($('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).hasClass("portlet-fullscreen")) ? 450 : 350,
                    groupable: false,
                    sortable: true,
                    selectable: "multiple",
                    columns: [{

                        field: "CatagoryName",
                        title: "Range"
                    },
                    {

                        field: "Amount",
                        title: "Amount",
                        format: "{0:n}"
                    }]
                });
            },

            RefreshChartStockValution: function () {
                $('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).data("kendoChart").dataSource.read();
            },
            RefreshSecondLevelChart: function (datasource) {
                viewFunction.selectedSeriesCatagoryiesId("item");
                // $('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).data("kendoChart").dataSource = datasource;
                // $('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).data("kendoChart").dataSource.read();
                $('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).data("kendoChart").setDataSource(datasource);
                $('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).data("kendoChart").refresh();
                viewFunction.itemWiseDatasource($('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).data("kendoChart").dataSource.data());
                console.log(viewFunction.itemWiseDatasource());
                console.log($('#' + viewFunction.serverpageObservableStockvalution() + config.debtorchart).data("kendoChart").dataSource.data());
            }
        };

        return {
            config: config,
            viewFunction: viewFunction,
            init: function (serverId) {
                ko.applyBindings(viewFunction);
                viewFunction.serverpageObservableStockvalution(serverId);
                config.serverpageid = serverId;
                viewFunction.createChartStockvalution('test');
                //ChartFilter(serverId, "createChartStockvalution")
                $('[title="Download"]').tooltip({
                    trigger: 'hover'
                });

            },
            render: function () {
                $("#apply_" + viewFunction.serverpageObservableStockvalution()).on("click", function () {
                    ChartFilter(viewFunction.serverpageObservableStockvalution(), "ApplyClickEvent");

                });
            },

        };
    };

    return asOnChart;
}(AsOnChart || {}, jQuery));

