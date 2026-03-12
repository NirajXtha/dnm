var MetricQueryBuilder = (function (metricQueryBuilder, $, ko) {
    "use strict";
    metricQueryBuilder.Create = function () {
        var config = {
            QueryCheckerButton: ".buttonQueryChecker",

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
            DisableAllDistributor: ko.observable(),
            MidenableBlink: ko.observable(),
            MaxenableBlink: ko.observable(),
            sqlHigestQuery: ko.observable(),
            sqlLowestQuery: ko.observable(),
            Order: ko.observable(0),
            FormatOptions: [
                { text: 'Color Scale', value: '1c' },
                { text: '2-color scale', value: '2c' },
                { text: '3-color scale', value: '3c' },


            ],
            LabelPostions: [
                { text: 'INSIDE', value: 'inside' },
                { text: 'OUTSIDE', value: 'OUTSIDE' },
            ],
            selectedlabelPositions: ko.observable("inside"),
            ChartOptions: [
                { text: 'Metric', value: 'M' },
                { text: 'Speedometer', value: 'S' },
                { text: 'Linear Gauge', value: 'L' },
            ],
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
                //alert(viewModel.sqlHigestQuery());
                if (typeof (viewModel.sqlHigestQuery()) === "undefined" || viewModel.sqlHigestQuery == null)
                    return false;
                var settings = {
                    url: window.virtualPath + "api/QueryBuilder/GetHighetDataFromSql?query=" + viewModel.sqlHigestQuery(),
                    type: "POST",
                    contentType: "application/json",
                    data: {
                        "query": viewModel.sqlHigestQuery(),
                    },
                    success: function (data) {
                        viewModel.highestValue(data.value);
                        viewModel.speedometerMaxValue(data.value);
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
                    url: window.virtualPath + "api/QueryBuilder/CheckQuery?query=" + viewModel.sqlQuery(),
                    type: "POST",
                    contentType: "application/json",
                    data: {
                        "query": viewModel.sqlQuery(),
                    },
                    success: function (data) {
                        debugger;

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
                   
                    url: window.virtualPath + "api/Setup/SaveWidgets",
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({
                        widgetsName: viewModel.reportname(),
                        WidgetsBGColor: viewModel.selectedBgcolorChooser(),
                        sqlQuery: viewModel.sqlQuery(),
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
                        isDistributorChecked: viewModel.DisableAllDistributor(),
                        LABELPOSITION: viewModel.selectedlabelPositions(),
                        SPEEDOMETERMAXVALUE: viewModel.speedometerMaxValue(),
                        MAXVALUEQUERY: viewModel.sqlHigestQuery(),
                        MINVALUEQUERY: viewModel.sqlLowestQuery(),
                        ChartType: viewModel.chosenChart(),
                        DISTRIBUTOR_CODE: viewModel.selectedModuleName(),

                    }),
                    success: function (data) {
                        debugger;
                        switch (data) {
                            case data.TYPE:

                        }
                        if (data.TYPE == "success")
                        {
                            toastr["success"](data.MESSAGE, "Success");
                        }

                        else if (data.TYPE == "failed")
                        {
                            toastr["error"](data.MESSAGE,"Something went wrong please try again")
                        }
                            
                        else
                        {
                            toastr["error"](data.MESSAGE, "Sql Query is failed to run.");
                        }
                            
                    },
                   
                };
                $.ajax(settings);
            },
            selectData: function (evt) {
            },
            updateData: function (evt) {
                //evt.preventDefault();
                var settings = {
                    url: window.virtualPath + "api/QueryBuilder/UpdateWidzed",
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({
                        widgetsName: viewModel.reportname(),
                        WidgetsBGColor: viewModel.selectedBgcolorChooser(),
                        sqlQuery: viewModel.sqlQuery(),
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
                        isDistributorChecked:viewModel.DisableAllDistributor(),
                        LABELPOSITION: viewModel.selectedlabelPositions(),
                        SPEEDOMETERMAXVALUE: viewModel.speedometerMaxValue(),
                        MAXVALUEQUERY: viewModel.sqlHigestQuery(),
                        MINVALUEQUERY: viewModel.sqlLowestQuery(),
                        ChartType: viewModel.chosenChart(),
                        WidgetsId: viewModel.reportId(),
                        ModuleCode: viewModel.selectedModuleName(),
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
            // alert(moment());
            var query = "select * from " + newValue + " where DELETED_FLAG=''N'";
            viewModel.sqlQuery(query);
            $.getJSON(window.virtualPath + "api/Setup/GetColumnNameList?tablesName=" + newValue, function (data) {

                viewModel.dateColumn(data.datecolumn);
                viewModel.columnsName(data);
            });

        }, viewModel);

        //function for the checkbox
        viewModel.DisableAllDistributor.subscribe(function (status) {

            if (status == true)
            {
               // var data = viewModel.moduleNames();
                $(".DisableOption option[value='null']").prop("selected", !0).change();
                $(".DisableOption option").attr('disabled', 'disabled')
               
            }
               
            else
            {
                $(".DisableOption option").attr('disabled', 'disabled').siblings().removeAttr('disabled');
            }

        }, viewModel);

        viewModel.selectedColumnsName.subscribe(function (newValue) {

            var query = "select " + newValue + " from " + viewModel.selectedTables() + " where DELETED_FLAG=''N''";
            viewModel.sqlQuery(query);


        });
        viewModel.selectedActionArray.subscribe(function (newValue) {
            var query = "select " + newValue + "(" + viewModel.selectedColumnsName() + ") from " + viewModel.selectedTables() + " where DELETED_FLAG=''N''";
            viewModel.sqlQuery(query);


        });

        viewModel.chosenChart.subscribe(function (newvalue) {
            viewModel.chosenFormat("3c");
        });
        viewModel.selectedDateRange.subscribe(function (newValue) {
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
        return {
            config: config,
            viewModel: viewModel,
            init: function () {
                ko.applyBindings(viewModel);

                viewModel.selectedIcon.extend({ notify: 'always' });
                viewModel.Order.extend({ notify: 'always' });
                viewModel.selectedBgcolorChooser.extend({ notify: 'always' });
                viewModel.highestValue.extend({ notify: 'always' });
                $.getJSON(window.virtualPath + "api/Setup/GetDistTableList", function (data) {
                    viewModel.tableNames(data);
                });
                $.getJSON(window.virtualPath + "api/Setup/GetDistributorList", function (data) {
                    data.splice(0, 0, { CUSTOMER_CODE: "null", CUSTOMER_EDESC: "Select Distributor...", DISTRIBUTOR_CODE: "null" });
                    viewModel.moduleNames(data);
                    console.log("modulename", data);
                });
            },
            render: function () {
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

    return metricQueryBuilder;
}(MetricQueryBuilder || {}, jQuery, ko));

