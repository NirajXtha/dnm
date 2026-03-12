var MetricQueryBuilder = (function (metricQueryBuilder, $, ko) {
    debugger;
    "use strict";

    metricQueryBuilder.Create = function () {
        debugger;
        var config = {
            QueryCheckerButton: ".buttonQueryChecker"
        };

        var viewModel = {
            reportId: ko.observable(),
            tableNames: ko.observableArray([]),
            columnsName: ko.observableArray([]),
            moduleNames: ko.observableArray([]),
            selectedModuleName: ko.observable(),
            reportname: ko.observable(),
            Active: ko.observable(),
            enableBlink: ko.observable(),
            MidenableBlink: ko.observable(),
            MaxenableBlink: ko.observable(),
            isHorizontal: ko.observable(false),
            sqlHigestQuery: ko.observable(),
            sqlLowestQuery: ko.observable(),
            Order: ko.observable(0),
            Xaxis: ko.observable(),
            Zaxis:ko.observable(),
            FormatOptions: [
                { text: 'Color Scale', value: '1c' },
                { text: '2-color scale', value: '2c' },
                { text: '3-color scale', value: '3c' },
            ],
            LabelPostions: [
                { text: 'INSIDE', value: 'inside' },
                { text: 'OUTSIDE', value: 'OUTSIDE' },
            ],
            TableSize: [
                { text: 'col-md-3', value: 'col-md-3' },
                { text: 'col-md-4', value: 'col-md-4' },
                { text: 'col-md-6', value: 'col-md-6' },
                { text: 'col-md-12', value: 'col-md-12' },
            ],
            sparklineoption: [
                { text: 'line', value: 'line' },
                { text: 'bar', value: 'bar' },
                { text: 'pie', value: 'pie' },
                { text: 'area', value: 'area' },
                
            ],
            selectedlabelPositions: ko.observable("inside"),
            ChartOptions: [
                { text: 'Metric', value: 'M' },
                { text: 'PercantageWithMetric', value: 'MP' },
                { text: 'PercantageWithPie', value: 'MPP' },
                { text: 'Speedometer', value: 'S' },
                { text: 'Linear Gauge', value: 'L' },
                { text: 'CompareMetric', value: 'C' },
                { text: 'Sparkline', value: 'P' },
                { text: 'Chart', value: 'A' },
                { text: 'Table', value: 'H' },
            ],
            sparklineSelected: ko.observable(),
            CalcualtionOption: [{ text: 'Compare', value: 'E' }, { text: 'growth percentage', value: 'P' }, { text: ' Difference Between Previous value', value: 'D' }],
            calcuationSelected: ko.observable(),
            tableSizeselected:ko.observable(),
            speedometerMaxValue: ko.observable(180),
            chosenChart: ko.observable("M"),
            chosenFormat: ko.observable("1c"),
            MaxselectedBgcolorChooser: ko.observable("#3598dc"),
            MaxselectedfontcolorChooser: ko.observable("#fff"),
            MidselectedBgcolorChooser: ko.observable("#3598dc"),
            lowestValue: ko.observable(0),
            highestValue: ko.observable(100),
            MidselectedfontcolorChooser: ko.observable("#fff"),
            selectedBgcolorChooser: ko.observable("#3598dc"),
            selectedTables: ko.observable(),
            selectedColumnsName: ko.observable(),
            selectedActionArray: ko.observable(),
            dateColumn: ko.observable(),
            sqlQuery: ko.observable(),
            widgetLink: ko.observable(),    
            SecondaryTitle: ko.observable(),
            selectedIcon: ko.observable("icon-pie-chart"),
            widgetTitle: ko.observable(),
            selectedDateRange: ko.observable(),
            sqlconditionalQuery: ko.observable(),
            selectedfontcolorChooser: ko.observable("#fff"),
            ActionArray: ko.observableArray([
                { name: "Sum" },
                { name: "Count" },
                { name: "Avg" }
            ]),
            dateRangecollection: ko.observableArray([
                { name: "Today" },
                { name: "This Week" },
                { name: "This Month" }
            ]),
            getHigestLowestValue: function () {
                debugger;
                //alert(viewModel.sqlHigestQuery());
                if (typeof (viewModel.sqlHigestQuery()) === "undefined" || viewModel.sqlHigestQuery() == null)
                    return false;
                var settings = {
                    url: window.virtualPath + "api/QueryBuilder/GetHighetDataFromSql",
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify( {
                        query: viewModel.sqlHigestQuery() ,
                    }),
                    success: function (data) {
                        if (viewModel.chosenChart() != "C") {
                            viewModel.highestValue(data.value);
                            viewModel.speedometerMaxValue(data.value);
                        }
                        toastr["success"]("Successfully Run Query", "Success");
                    },
                    statusCode: {
                        401: function () {

                        },
                        403: function () {

                        },
                        500: function (data) {

                            toastr["error"](data.responseJSON.MESSAGE, "Sql Query is failed to run.");
                        }
                    }
                };
                $.ajax(settings);
            },
            getLowestValue: function () {
                debugger;
                //alert(viewModel.sqlLowestQuery());
                if (typeof (viewModel.sqlLowestQuery()) === "undefined" || viewModel.sqlLowestQuery() == null)
                    return false;
                var settings = {
                    url: window.virtualPath + "api/QueryBuilder/GetHighetDataFromSql?query=" + viewModel.sqlLowestQuery(),
                    type: "POST",
                    contentType: "application/json",
                    data: {
                        "query": viewModel.sqlLowestQuery(),
                    },
                    success: function (data) {
                        viewModel.lowestValue(data.value);
                        toastr["success"]("Successfully Run Query", "Success");
                    },
                    statusCode: {
                        401: function () {

                        },
                        403: function () {

                        },
                        500: function (data) {

                            toastr["error"](data.responseJSON.MESSAGE, "Sql Query is failed to run.");
                        }
                    }
                };
                $.ajax(settings);
            },
            checkQuery: function () {
                debugger;
                var settings = {
                    url: window.location.protocol + "//" + window.location.host + window.virtualPath + "api/QueryBuilder/CheckQuery?query=" + viewModel.sqlQuery(),
                    type: "POST",
                    contentType: "application/json",
                    data: {
                        "query": viewModel.sqlQuery(),
                    },
                    success: function (data) {

                        toastr["success"]("Successfully Run Query", "Success");
                    },
                    statusCode: {
                        401: function () {

                        },
                        403: function () {

                        },
                        500: function (data) {

                            toastr["error"](data.responseJSON.MESSAGE, "Sql Query is failed to run.");
                        }
                    }
                };
                $.ajax(settings);
            },
           
            saveData: function (evt) {
                debugger;
                //evt.preventDefault();
                var settings = {
                    url: window.virtualPath + "api/QueryBuilder/CreateWidzed",
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({
                        widgetsName: viewModel.reportname(),
                        WidgetsBGColor: viewModel.selectedBgcolorChooser(),
                        sqlQuery: viewModel.sqlQuery(),
                        widgetLink: viewModel.widgetLink(),
                        widgetsColor: viewModel.selectedfontcolorChooser(),
                        widgetsTitle: viewModel.widgetTitle(),
                        widgetFontIcon: viewModel.selectedIcon(),
                        Isactive: viewModel.Active(),
                        OrderNo: viewModel.Order(),
                        MidBGColor: viewModel.MidselectedBgcolorChooser(),
                        MidFontColor: viewModel.MidselectedfontcolorChooser(),
                        IsMidBlink: viewModel.MidenableBlink(),
                        MaxBGColor: viewModel.MaxselectedBgcolorChooser(),
                        MaxFontColor: viewModel.MaxselectedfontcolorChooser(),
                        IsMaxBlink: viewModel.MaxenableBlink(),
                        MaxValue: viewModel.highestValue(),
                        MinValue: viewModel.lowestValue(),
                        IsBlink: viewModel.enableBlink(),
                        LABELPOSITION: viewModel.selectedlabelPositions(),
                        SPEEDOMETERMAXVALUE: viewModel.speedometerMaxValue(),
                        MAXVALUEQUERY: viewModel.sqlHigestQuery(),
                        MINVALUEQUERY: viewModel.sqlLowestQuery(),
                        ChartType: viewModel.chosenChart(),
                        ModuleCode: viewModel.selectedModuleName(),
                        CaculationMethod: viewModel.calcuationSelected(),
                        SecondaryTitle:viewModel.SecondaryTitle(),
                        TableSize: viewModel.tableSizeselected(),
                        sparklineoption: viewModel.sparklineSelected(),
                        Xaxis:viewModel.Xaxis(),
                        Zaxis:viewModel.Zaxis(),
                    }),
                    success: function (data) {
                        
                        toastr["success"]("Widget Is Created Successfully", "Success");
                    },
                    statusCode: {
                        401: function () {

                        },
                        403: function () {

                        },
                        500: function (data) {

                            toastr["error"](data.responseJSON.MESSAGE, "Sql Query is failed to run.");
                        }
                    }
                };
                $.ajax(settings);
                
            },
            selectData: function (evt) {
            },
            updateData: function (evt) {
                debugger;
                //evt.preventDefault();
                var settings = {
                    url: window.virtualPath + "api/QueryBuilder/UpdateWidzed",
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({
                        widgetsName: viewModel.reportname(),
                        WidgetsBGColor: viewModel.selectedBgcolorChooser(),
                        sqlQuery: viewModel.sqlQuery(),
                        widgetsLink: viewModel.widgetsLink(),
                        widgetsColor: viewModel.selectedfontcolorChooser(),
                        widgetsTitle: viewModel.widgetTitle(),
                        widgetFontIcon: viewModel.selectedIcon(),
                        Isactive: viewModel.Active(),
                        OrderNo: viewModel.Order(),
                        MidBGColor: viewModel.MidselectedBgcolorChooser(),
                        MidFontColor: viewModel.MidselectedfontcolorChooser(),
                        IsMidBlink: viewModel.MidenableBlink(),
                        MaxBGColor: viewModel.MaxselectedBgcolorChooser(),
                        MaxFontColor: viewModel.MaxselectedfontcolorChooser(),
                        IsMaxBlink: viewModel.MaxenableBlink(),
                        MaxValue: viewModel.highestValue(),
                        MinValue: viewModel.lowestValue(),
                        IsBlink: viewModel.enableBlink(),
                        LABELPOSITION: viewModel.selectedlabelPositions(),
                        SPEEDOMETERMAXVALUE: viewModel.speedometerMaxValue(),
                        MAXVALUEQUERY: viewModel.sqlHigestQuery(),
                        MINVALUEQUERY: viewModel.sqlLowestQuery(),
                        ChartType: viewModel.chosenChart(),
                        WidgetsId: viewModel.reportId(),
                        ModuleCode: viewModel.selectedModuleName(),
                        Horizontal: viewModel.isHorizontal(),
                    }),
                    success: function (data) {

                        toastr["success"]("Successfully Run Query", "Success");
                    },
                    statusCode: {
                        401: function () {

                        },
                        403: function () {

                        },
                        500: function (data) {

                            toastr["error"](data.responseJSON.MESSAGE, "Sql Query is failed to run.");
                        }
                    }
                };
                $.ajax(settings);
            },
        };

        viewModel.selectedIcon.subscribe(function (newValue) {

        });
        viewModel.Order.subscribe(function (newValue) {

        });
        viewModel.selectedTables.subscribe(function (newValue) {
            debugger;
            // alert(moment());
            var query = "select * from " + newValue + " where DELETED_FLAG=''N'";
            viewModel.sqlQuery(query);
            $.getJSON(window.virtualPath + "api/QueryBuilder/GetColumsListByTableName?tablesName=" + newValue, function (data) {
                debugger;
                viewModel.dateColumn(data.datecolumn);
                viewModel.columnsName(data.DATA);
            });

        }, viewModel);

        viewModel.selectedColumnsName.subscribe(function (newValue) {
            debugger;

            var query = "select " + newValue + " from " + viewModel.selectedTables() + " where DELETED_FLAG=''N''";
            viewModel.sqlQuery(query);


        });
        viewModel.selectedActionArray.subscribe(function (newValue) {
            debugger;
            var query = "select " + newValue + "(" + viewModel.selectedColumnsName() + ") from " + viewModel.selectedTables() + " where DELETED_FLAG=''N''";
            viewModel.sqlQuery(query);


        });

        viewModel.chosenChart.subscribe(function (newvalue) {
            debugger;

            viewModel.chosenFormat("3c");
        });
        viewModel.selectedDateRange.subscribe(function (newValue) {
            debugger;
            //  TO_DATE('" + fromdate + "', 'YYYY-MM-DD')
            //  alert(moment().startOf('week').format("YYYY-MM-DD")); //Returns 1
            //alert(moment.locale('en'));
            //alert(moment().startOf('month').format("MM-DD-YYYY"));
            if (newValue === "Today") {
                var query = "select " + viewModel.selectedActionArray() + "(" + viewModel.selectedColumnsName() + ") from " + viewModel.selectedTables() + " where DELETED_FLAG=''N'' and " + viewModel.dateColumn() + "=SYSDATE";
                viewModel.sqlQuery(query);

            }
            else if (newValue === "This Week") {
                var startDate = moment().startOf('week').format("YYYY-MM-DD");
                var query = "select " + viewModel.selectedActionArray() + "(" + viewModel.selectedColumnsName() + ") from " + viewModel.selectedTables() + " where DELETED_FLAG=''N'' and " + viewModel.dateColumn() + ">=" + startDate + " and " + viewModel.dateColumn() + "<=" + moment().format("YYYY-MM-DD") + "";
                viewModel.sqlQuery(query);
            }
            else if (newValue === "This Month") {
                var startDate = moment().startOf('month').format("YYYY-MM-DD");
                var query = "select " + viewModel.selectedActionArray() + "(" + viewModel.selectedColumnsName() + ") from " + viewModel.selectedTables() + " where DELETED_FLAG=''N'' and " + viewModel.dateColumn() + ">=" + startDate + " and " + viewModel.dateColumn() + "<=" + moment().format("YYYY-MM-DD") + "";
                viewModel.sqlQuery(query);
            }
            else {
                var query = "select " + viewModel.selectedActionArray() + "(" + viewModel.selectedColumnsName() + ") from " + viewModel.selectedTables() + " where DELETED_FLAG=''N''";
            }


        });

        viewModel.selectedBgcolorChooser.subscribe(function (newValue) {
            debugger;
            var gauges = $("#gauge").data("kendoRadialGauge");
            var linearGauges = $("#linearGauge").data("kendoLinearGauge");
            //  window.configuredRanges = gauges.options.scale.ranges;
            if (gauges) {
                gauges.options.scale.labels.position = viewModel.selectedlabelPositions();
                gauges.options.scale.ranges[0].color = newValue;
                gauges.options.scale.ranges[0].from = 0;
                gauges.options.scale.ranges[0].to = viewModel.lowestValue();
                gauges.options.scale.max = viewModel.speedometerMaxValue();
                gauges.redraw();
            }

            if (linearGauges) {
                linearGauges.options.scale.labels.position = viewModel.selectedlabelPositions();
                linearGauges.options.scale.ranges[0].color = newValue;
                linearGauges.options.scale.ranges[0].from = 0;
                linearGauges.options.scale.ranges[0].to = viewModel.lowestValue();
                linearGauges.options.scale.max = viewModel.speedometerMaxValue();
                linearGauges.redraw();
            }
            //gauges.options.scale.ranges[2].color = viewModel.MaxselectedBgcolorChooser();
            //gauges.options.scale.ranges[2].from = viewModel.highestValue();
            //gauges.options.scale.ranges[2].to = viewModel.speedometerMaxValue();

            //gauges.options.scale.ranges[1].color = viewModel.MidselectedBgcolorChooser();
            //gauges.options.scale.ranges[1].from = viewModel.lowestValue();
            //gauges.options.scale.ranges[1].to = viewModel.highestValue();

        });
        viewModel.speedometerMaxValue.subscribe(function (newValue) {
            debugger;
            var gauges = $("#gauge").data("kendoRadialGauge");
            var linearGauges = $("#linearGauge").data("kendoLinearGauge");

            if (linearGauges) {
                linearGauges.options.scale.ranges[2].to = newValue;
                linearGauges.options.scale.max = newValue;
                linearGauges.redraw();
            }
            //  window.configuredRanges = gauges.options.scale.ranges;
            if (gauges) {
                gauges.options.scale.ranges[0].color = viewModel.selectedBgcolorChooser();
                gauges.options.scale.labels.position = viewModel.selectedlabelPositions();
                gauges.options.scale.ranges[0].from = 0;
                gauges.options.scale.ranges[0].to = viewModel.lowestValue();

                gauges.options.scale.ranges[2].color = viewModel.MaxselectedBgcolorChooser();
                gauges.options.scale.ranges[2].from = viewModel.highestValue();
                gauges.options.scale.ranges[2].to = newValue;

                gauges.options.scale.ranges[1].color = viewModel.MidselectedBgcolorChooser();
                gauges.options.scale.ranges[1].from = viewModel.lowestValue();
                gauges.options.scale.ranges[1].to = viewModel.highestValue();
                gauges.options.scale.max = newValue;
                gauges.redraw();
            }
        });
        viewModel.highestValue.subscribe(function (newValue) {
            debugger;
            var gauges = $("#gauge").data("kendoRadialGauge");
            var linearGauges = $("#linearGauge").data("kendoLinearGauge");
            if (linearGauges) {
                linearGauges.options.scale.ranges[2].from = newValue;
                linearGauges.redraw();
            }
            //  window.configuredRanges = gauges.options.scale.ranges;
            if (gauges) {
                gauges.options.scale.ranges[0].color = viewModel.selectedBgcolorChooser();
                gauges.options.scale.labels.position = viewModel.selectedlabelPositions();
                gauges.options.scale.ranges[0].from = 0;
                gauges.options.scale.ranges[0].to = viewModel.lowestValue();

                gauges.options.scale.ranges[2].color = viewModel.MaxselectedBgcolorChooser();
                gauges.options.scale.ranges[2].from = newValue;
                gauges.options.scale.ranges[2].to = viewModel.speedometerMaxValue();

                gauges.options.scale.ranges[1].color = viewModel.MidselectedBgcolorChooser();
                gauges.options.scale.ranges[1].from = viewModel.lowestValue();
                gauges.options.scale.ranges[1].to = viewModel.highestValue();
                gauges.options.scale.max = viewModel.speedometerMaxValue();
                gauges.redraw();
            }
        });
        viewModel.lowestValue.subscribe(function (newValue) {
            debugger;
            var gauges = $("#gauge").data("kendoRadialGauge");
            var linearGauges = $("#linearGauge").data("kendoLinearGauge");
            if (linearGauges) {
                linearGauges.options.scale.ranges[0].to = newValue;
                linearGauges.redraw();
            }
            //  window.configuredRanges = gauges.options.scale.ranges;
            if (gauges) {
                gauges.options.scale.ranges[0].color = viewModel.selectedBgcolorChooser();
                gauges.options.scale.labels.position = viewModel.selectedlabelPositions();
                gauges.options.scale.max = viewModel.speedometerMaxValue();
                gauges.options.scale.ranges[0].from = 0;
                gauges.options.scale.ranges[0].to = newValue;

                gauges.options.scale.ranges[2].color = viewModel.MaxselectedBgcolorChooser();
                gauges.options.scale.ranges[2].from = viewModel.highestValue();
                gauges.options.scale.ranges[2].to = viewModel.speedometerMaxValue();

                gauges.options.scale.ranges[1].color = viewModel.MidselectedBgcolorChooser();
                gauges.options.scale.ranges[1].from = viewModel.lowestValue();
                gauges.options.scale.ranges[1].to = viewModel.highestValue();
                gauges.redraw();
            }
        });
        viewModel.selectedlabelPositions.subscribe(function (newValue) {
            debugger;
            var gauges = $("#gauge").data("kendoRadialGauge");
            gauges.options.scale.labels.position = newValue;
            //  window.configuredRanges = gauges.options.scale.ranges;

            //gauges.options.scale.ranges[0].color = viewModel.selectedBgcolorChooser();
            //gauges.options.scale.labels.position = newValue;
            //gauges.options.scale.max = viewModel.speedometerMaxValue();
            //gauges.options.scale.ranges[0].from = 0;
            //gauges.options.scale.ranges[0].to = viewModel.lowestValue;

            //gauges.options.scale.ranges[2].color = viewModel.MaxselectedBgcolorChooser();
            //gauges.options.scale.ranges[2].from = viewModel.highestValue();
            //gauges.options.scale.ranges[2].to = viewModel.speedometerMaxValue();

            //gauges.options.scale.ranges[1].color = viewModel.MidselectedBgcolorChooser();
            //gauges.options.scale.ranges[1].from = viewModel.lowestValue();
            //gauges.options.scale.ranges[1].to = viewModel.highestValue();
            gauges.redraw();
        });
        viewModel.MaxselectedBgcolorChooser.subscribe(function (newValue) {
            debugger;
            var gauges = $("#gauge").data("kendoRadialGauge");
            var linearGauges = $("#linearGauge").data("kendoLinearGauge");
            //  window.configuredRanges = gauges.options.scale.ranges;
            //gauges.options.scale.ranges[0].color = viewModel.selectedBgcolorChooser();
            //gauges.options.scale.labels.position = newValue;
            //gauges.options.scale.max = viewModel.speedometerMaxValue();
            //gauges.options.scale.ranges[0].from = 0;
            //gauges.options.scale.ranges[0].to = viewModel.lowestValue;

            gauges.options.scale.ranges[2].color = newValue;
            gauges.options.scale.ranges[2].from = viewModel.highestValue();
            gauges.options.scale.ranges[2].to = viewModel.speedometerMaxValue();

            linearGauges.options.scale.ranges[2].color = newValue;
            linearGauges.options.scale.ranges[2].from = viewModel.highestValue();
            linearGauges.options.scale.ranges[2].to = viewModel.speedometerMaxValue();
            //gauges.options.scale.ranges[1].color = viewModel.MidselectedBgcolorChooser();
            //gauges.options.scale.ranges[1].from = viewModel.lowestValue();
            //gauges.options.scale.ranges[1].to = viewModel.highestValue();
            gauges.redraw();
            linearGauges.redraw();
        });
        viewModel.MidselectedBgcolorChooser.subscribe(function (newValue) {
        debugger;

            var gauges = $("#gauge").data("kendoRadialGauge");
            var linearGauges = $("#linearGauge").data("kendoLinearGauge");
            //  window.configuredRanges = gauges.options.scale.ranges;
            //gauges.options.scale.ranges[0].color = viewModel.selectedBgcolorChooser();
            //gauges.options.scale.labels.position = newValue;
            //gauges.options.scale.max = viewModel.speedometerMaxValue();
            //gauges.options.scale.ranges[0].from = 0;
            //gauges.options.scale.ranges[0].to = viewModel.lowestValue;

            //gauges.options.scale.ranges[2].color = newValue;
            //gauges.options.scale.ranges[2].from = viewModel.highestValue();
            //gauges.options.scale.ranges[2].to = viewModel.speedometerMaxValue();


            gauges.options.scale.ranges[1].color = newValue;
            gauges.options.scale.ranges[1].from = viewModel.lowestValue();
            gauges.options.scale.ranges[1].to = viewModel.highestValue();

            linearGauges.options.scale.ranges[1].color = newValue;
            linearGauges.options.scale.ranges[1].from = viewModel.lowestValue();
            linearGauges.options.scale.ranges[1].to = viewModel.highestValue();

            gauges.redraw();
            linearGauges.redraw();
        });
        viewModel.isHorizontal.subscribe(function (newValue) {
            debugger;

            var linearGauges = $("#linearGauge").data("kendoLinearGauge");
            if (linearGauges) {
                linearGauges.options.scale.vertical = !newValue;
                $("#linearGauge-container").toggleClass("horizontal", newValue);
                linearGauges.redraw();
            }
        });
        return {

            config: config,
            viewModel: viewModel,
            init: function () {
                debugger;
                

                ko.applyBindings(viewModel);

                viewModel.selectedIcon.extend({ notify: 'always' });
                viewModel.Order.extend({ notify: 'always' });
                viewModel.selectedBgcolorChooser.extend({ notify: 'always' });
                viewModel.highestValue.extend({ notify: 'always' });
                $.getJSON(window.virtualPath + "api/QueryBuilder/GetTablesList", function (data) {
                    debugger;

                    viewModel.tableNames(data.DATA);
                });
                $.getJSON(window.virtualPath + "api/QueryBuilder/GetModuleList", function (data) {
                    debugger;

                    viewModel.moduleNames(data.data);
                    console.log("modulename", data);
                });
            },
            render: function () {
                debugger;
                $("#gauge").kendoRadialGauge({
                    //gaugeArea: {
                    //    width: "230px",
                    //    height: "130",
                    //},

                    pointer: {
                        value: 65
                    },

                    scale: {
                        minorUnit: 5,
                        startAngle: -30,
                        endAngle: 210,
                        max: 180,
                        labels: {
                            position: "inside"
                        },
                        ranges: [
                            {
                                from: 80,
                                to: 120,
                                color: '#ff7a00'
                            }, {
                                from: 120,
                                to: 150,
                                color: "#ff7a00"
                            }, {
                                from: 150,
                                to: 180,
                                color: "#c20000"
                            }
                        ]
                    }
                });

                $("#linearGauge").kendoLinearGauge({
                    pointer: {
                        value: 65,
                        shape: "arrow"
                    },
                    scale: {
                        majorUnit: 20,
                        minorUnit: 5,
                        max: 180,
                        //vertical: true,
                        ranges: [
                            {
                                from: 80,
                                to: 120,
                                color: "#ffc700"
                            }, {
                                from: 120,
                                to: 150,
                                color: "#ff7a00"
                            }, {
                                from: 150,
                                to: 180,
                                color: "#c20000"
                            }
                        ]
                    }
                });
            },

        };
    };
    metricQueryBuilder.CreateConfig = function () {
        debugger;
        var config = {
            QueryCheckerButton: ".buttonQueryChecker",

        };

        var viewModel = {
            customerCode: ko.observable(),
            itemsCode: ko.observable(),
            suppliersCode:ko.observable(),
            sqlquery: ko.observable(),
            widgetsName: ko.observable(),
            divisionCode: ko.observable(),
            ledgerCode:ko.observable(),
            saveData: function (evt) {
                debugger;

                //evt.preventDefault();
                var settings = {
                    url: window.virtualPath + "api/QueryBuilder/CreateWidzedConfig",
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({
                        widgetsName: viewModel.widgetsName(),
                        customerCode:viewModel.customerCode(),
                        sqlQuery: viewModel.sqlquery(),
                        itemsCode: viewModel.itemsCode(),
                        suppliersCode:viewModel.suppliersCode(),
                        divisionCode: viewModel.divisionCode(),
                        ledgerCode:viewModel.ledgerCode(),
                    }),
                    success: function (data) {
                        
                        toastr["success"]("Widget Is Created Successfully", "Success");
                    },
                    statusCode: {
                        401: function () {

                        },
                        403: function () {

                        },
                        500: function (data) {

                            toastr["error"](data.responseJSON.MESSAGE, "Sql Query is failed to run.");
                        }
                    }
                };
                
                $.ajax(settings);
            },
        
        };

      
        return {
            config: config,
            viewModel: viewModel,
            init: function () {
                debugger;

                ko.applyBindings(viewModel);
                

                function onSelect(e) {
                    debugger;

                    if (e.item) {
                        var dataItem = this.dataItem(e.item);
                        
                        viewModel.customerCode(dataItem.CustomerCode);

                    } else {

                    }
                };

              

                function onFiltering(e) {
                    debugger;

                    if ("kendoConsole" in window) {
                        kendoConsole.log("event :: filtering");
                    }
                }

                var autoCompleteurl = window.location.protocol + "//" + window.location.host + "/api/Scheduler/GetAllCustomers";
                var autoCompletedataSource = new kendo.data.DataSource({
                    transport: {
                        read: {
                            url: autoCompleteurl,
                            dataType: "json",
                            contentType: "application/json; charset=utf-8"
                        }
                    }
                });

                $("#CustomerSelect").kendoDropDownList({
                    dataTextField: "CustomerName",
                    dataValueField: "CustomerCode",
                    dataSource: autoCompletedataSource,
                    filter: "startswith",
                    select: onSelect,
                   
                });

                function onSelectItem(e) {
                    debugger;

                    if (e.item) {
                        var dataItem = this.dataItem(e.item);
                        
                      
                        viewModel.itemsCode(dataItem.Code);

                    } else {

                    }
                };
                var autoCompletedataSourceItem = new kendo.data.DataSource({
                    transport: {
                        read: {
                            url: window.location.protocol + "//" + window.location.host + "/api/Scheduler/GetAllItems",
                            dataType: "json",
                            contentType: "application/json; charset=utf-8"
                        }
                    }
                });

                $("#ItemsSelect").kendoComboBox({
                    dataTextField: "Name",
                    dataValueField: "Code",
                    filter: "contains",
                    autoBind: false,
                    minLength: 1,
                    dataSource: autoCompletedataSourceItem,

                    select: onSelectItem,

                });


                function onSelectsupplier(e) {
                    debugger;

                    if (e.item) {
                        var dataItem = this.dataItem(e.item);
                        
                       
                        viewModel.suppliersCode(dataItem.Code);

                    } else {

                    }
                };

                var autoCompletedataSourcesuppliers = new kendo.data.DataSource({
                    transport: {
                        read: {
                            url: window.location.protocol + "//" + window.location.host + "/api/Scheduler/GetAllSuppliers",
                            dataType: "json",
                            contentType: "application/json; charset=utf-8"
                        }
                    }
                });

                $("#supplierSelect").kendoComboBox({
                    dataTextField: "Name",
                    dataValueField: "Code",
                    filter: "contains",
                    autoBind: false,
                    minLength: 1,
                    dataSource: autoCompletedataSourcesuppliers,

                    select: onSelectsupplier,

                });


                function onSelectdivision(e) {
                    debugger;

                    if (e.item) {
                        var dataItem = this.dataItem(e.item);
                        

                        viewModel.divisionCode(dataItem.Code);

                    } else {

                    }
                };

                var autoCompletedataSourcedivision = new kendo.data.DataSource({
                    transport: {
                        read: {
                            url: window.location.protocol + "//" + window.location.host + "/api/Scheduler/GetAllDivisoin",
                            dataType: "json",
                            contentType: "application/json; charset=utf-8"
                        }
                    }
                });

                $("#DivisionSelect").kendoComboBox({
                    dataTextField: "Name",
                    dataValueField: "Code",
                    filter: "contains",
                    autoBind: false,
                    minLength: 1,
                    dataSource: autoCompletedataSourcedivision,

                    select: onSelectdivision,

                });

                function onSelectLedger(e) {
                    debugger;

                    if (e.item) {
                        var dataItem = this.dataItem(e.item);
                        

                        viewModel.ledgerCode(dataItem.Code);

                    } else {

                    }
                };

                var autoCompletedataSourceLedger = new kendo.data.DataSource({
                    transport: {
                        read: {
                            url: window.location.protocol + "//" + window.location.host + "/api/Scheduler/GetAllLEDGERS",
                            dataType: "json",
                            contentType: "application/json; charset=utf-8"
                        }
                    }
                });

                $("#LedgerSelect").kendoComboBox({
                    dataTextField: "Name",
                    dataValueField: "Code",
                    filter: "contains",
                    autoBind: false,
                    minLength: 1,
                    dataSource: autoCompletedataSourceLedger,

                    select: onSelectLedger,

                });
            },
            render: function () {
              
            },

        };
    };

    return metricQueryBuilder;
}(MetricQueryBuilder || {}, jQuery, ko));

