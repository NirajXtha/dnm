//region double event on amchart start
AmCharts.clickTimeout = 0; // this will hold setTimeout reference
AmCharts.lastClick = 0; // last click timestamp
AmCharts.doubleClickDuration = 300; // distance between clicks in ms - if it's less than that - it's a doubleckick
// let's define functions to actually do something on clicks/doubleclicks
// you will want to replace the insides of these with your own code
AmCharts.doSingleClick = function (event) {
    //var div = document.getElementById("events");
    //div.innerHTML = "Click<br />" + div.innerHTML;
    //alert(event.item.dataContext.CategoryCode);
}
AmCharts.doDoubleClick = function (event) {
    //var div = document.getElementById("events");
    //div.innerHTML = "Double Click<br />" + div.innerHTML;
    //alert(event.item.dataContext.CategoryCode);
}
// create click handler
AmCharts.myClickHandler = function (event) {
    var ts = (new Date()).getTime();
    if ((ts - AmCharts.lastClick) < AmCharts.doubleClickDuration) {
        // it's double click!
        // let's clear the timeout so the "click" event does not fire
        if (AmCharts.clickTimeout) {
            clearTimeout(AmCharts.clickTimeout);
        }

        // reset last click
        AmCharts.lastClick = 0;

        // now let's do whatever we want to do on double-click
        event.chart.doDoubleClick(event);
    }
    else {
        // single click!
        // let's delay it to see if a second click will come through
        AmCharts.clickTimeout = setTimeout(function () {
            // let's do whatever we want to do on single click
            event.chart.doSingleClick(event);
        }, AmCharts.doubleClickDuration);
    }
    AmCharts.lastClick = ts;
}
//region double event on amchart end
var charts = function () {
    var dataProvider = [],
    chartDiv = "chart_1",
    currentChart = "",
    currentType = "",
    chartHeight = "chart-portlet-height",//css class
    charFullHeight = "chart-full-height",//css class
    currentChartObject = null,
    chartImages = Metronic.getGlobalPluginsAbsolutePath() + "amcharts/images/",
    toggleFullWidth = function () {
        if ($('#' + chartDiv).hasClass(chartHeight)) {
            $("#" + chartDiv).removeClass(chartHeight);
            $("#" + chartDiv).addClass(charFullHeight);
        }
        else if ($('#' + chartDiv).hasClass(charFullHeight)) {
            $("#" + chartDiv).removeClass(charFullHeight);
            $("#" + chartDiv).addClass(chartHeight);
        }
    },
    loadCategorySalesChart = function () {
        var mainUrl = Metronic.getGlobalUrl() + "api/" + "saleshome/GetCategorySales";
        var el = $("#" + chartDiv).parent();
        Metronic.blockUI({
            target: el,
            animate: true,
            overlayColor: 'none'
        });
        $.ajax({
            url: mainUrl,
            dataType: "Json",
            type: "get",
            async:true,
            error: function (error) {
                Metronic.unblockUI(el);
            },
            success: function (data) {
                dataProvider = data;

                if (currentChart == "")
                {
                    loadCategoryChart();
                }
                else
                {
                    eval("charts." + currentChart + "()");
                }

                Metronic.unblockUI(el);
            }
        });
    },
    loadProductSalesData = function (categoryCode, callback) {
        var mainUrl = Metronic.getGlobalUrl() + "api/" + "saleshome/GetProductSalesByCategory";
        var el = $("#" + chartDiv).parent();
        Metronic.blockUI({
            target: el,
            animate: true,
            overlayColor: 'none'
        });
        $.ajax({
            url: mainUrl,
            dataType: "Json",
            type: "get",
            data: { "categoryCode": categoryCode },
            async: true,
            error: function (error) {
                Metronic.unblockUI(el);
            },
            success: function (data) {
                if (callback !== undefined && data.length > 0)
                    callback(data);
                Metronic.unblockUI(el);
            }
        });

    },
    addChildProductData = function (categoryCode, data) {
        //
        var index = dataProvider.findIndex(x => x.Code === categoryCode);
        if(index > -1)
        dataProvider[index].data = data;
    },
    loadCategoryChart = function () {
        var chart = AmCharts.makeChart(chartDiv, {
            "type": "serial",
            "theme": "light",
            "pathToImages": chartImages,
            "autoMargins": true,
            "fontFamily": 'Open Sans',
            "color": '#888',
            "titles": [{
                "text": "Category"
            }],
            "chartScrollbar": {
                "enabled": true
            },
            "dataProvider": dataProvider,
            "valueAxes": [{
                "axisAlpha": 0,
                "position": "left"
            }],
            "startDuration": 1,
            "graphs": [{
                "alphaField": "alpha",
                "balloonText": "<span style='font-size:13px;'>[[title]] in [[category]]:<b>[[value]]</b> [[additional]]</span>",
                "dashLengthField": "dashLengthColumn",
                "fillAlphas": 1,
                "title": currentType,
                "type": "column",
                "valueField": currentType
            }],
            "legend": {
                "enabled": true,
                "useGraphSettings": true
            },
            "categoryField": "Description",
            "categoryAxis": {
                "gridPosition": "start",
                "axisAlpha": 0,
                "tickLength": 10,
                "labelRotation": -90,
                "fontSize": 10,
                "inside": true,
            }
        });

        chart.drillLevels = [{
            "title": "Category",
            "data": dataProvider
        }];

        chart.doSingleClick = function (event) {
        }
        chart.doDoubleClick = function (event) {
            var dataContext = event.item.dataContext;
            if (dataContext.data == undefined) {
                loadProductSalesData(dataContext.Code,
                    function (data) {
                        addChildProductData(dataContext.Code, data);
                        drillDown(event);
                    });
            }
            else {
                drillDown(event);
            }
        }
        chart.addListener("clickGraphItem", AmCharts.myClickHandler);
        currentChart = "columnChart";
        currentChartObject = chart;
        var drillDown = function (event) {
            chart.drillLevels.push(event.item.dataContext);
            // replace data
            chart.dataProvider = event.item.dataContext.data;

            // replace title
            chart.titles[0].text = event.item.dataContext.Description;

            // add back link
            // let's add a label to go back to yearly data
            event.chart.addLabel(
              0, 25,
              "< Go back",
              undefined,
              undefined,
              undefined,
              undefined,
              undefined,
              undefined,
              'javascript:charts.columnChartDrillUp();');

            // take in data and animate
            chart.validateData();
            chart.animateAgain();

        }
        return {

            drillUp: function () {

                // get level
                chart.drillLevels.pop();
                var level = chart.drillLevels[chart.drillLevels.length - 1];

                // replace data
                chart.dataProvider = level.data;

                // replace title
                chart.titles[0].text = level.title;

                // remove labels
                if (chart.drillLevels.length === 1)
                    chart.clearLabels();

                // take in data and animate
                chart.validateData();
                chart.animateAgain();
            }
        }
    },
    loadPieCategoryChart = function () {
        var chart = AmCharts.makeChart(chartDiv, {
            "autoMargins": true,
            "type": "pie",
            "theme": "light",
            "dataProvider": dataProvider,
            "valueField": currentType,
            "titleField": "Description",
            "outlineAlpha": 0.4,
            "depth3D": 15,
            "balloonText": "[[title]]<br><span style='font-size:14px'><b>[[value]]</b> ([[percents]]%)</span>",
            "angle": 30,
            "export": {
                "enabled": true
            },
            "titles": [{
                "text": "Category"
            }]
        });

        chart.drillLevels = [{
            "title": "Category",
            "data": dataProvider
        }];

        chart.doSingleClick = function (event) {
        }
        chart.doDoubleClick = function (event) {
            var dataContext = event.dataItem.dataContext;
            if (dataContext.data == undefined) {
                loadProductSalesData(dataContext.Code,
                    function (data) {
                        addChildProductData(dataContext.Code, data);
                        drillDown(event);
                    });
            }
            else {
                drillDown(event);
            }
        }

        var drillDown = function (event) {
            chart.drillLevels.push(event.dataItem.dataContext);
            // replace data
            chart.dataProvider = event.dataItem.dataContext.data;

            // replace title
            chart.titles[0].text = event.dataItem.dataContext.Description;

            // add back link
            // let's add a label to go back to yearly data
            event.chart.addLabel(
              0, 25,
              "< Go back",
              undefined,
              undefined,
              undefined,
              undefined,
              undefined,
              undefined,
              'javascript:charts.pieChartDrillUp();');

            // take in data and animate
            chart.validateData();
            chart.animateAgain();

        }

        chart.addListener("clickSlice", AmCharts.myClickHandler);
        currentChart = "pieChart";
        currentChartObject = chart;

        return {

            drillUp: function () {

                // get level
                chart.drillLevels.pop();
                var level = chart.drillLevels[chart.drillLevels.length - 1];

                // replace data
                chart.dataProvider = level.data;

                // replace title
                chart.titles[0].text = level.title;

                // remove labels
                if (chart.drillLevels.length === 1)
                    chart.clearLabels();

                // take in data and animate
                chart.validateData();
                chart.animateAgain();
            }
        }
    },
    loadBarCategoryChart = function () {
        var chart = AmCharts.makeChart(chartDiv,
				{
				    "type": "serial",
				    "theme": "light",
				    "fontFamily": 'Open Sans',
				    "color": '#888',
				    "pathToImages": chartImages,
				    "categoryField": "Description",
				    "rotate": true,
				    "startDuration": 1,
				    "categoryAxis": {
				        "gridPosition": "start",
				        "inside": true,
				    },
				    "chartCursor": {
				        "enabled": true
				    },
				    "chartScrollbar": {
				        "enabled": true
				    }, "legend": {
				        "enabled": true,
				        "useGraphSettings": true
				    },
				    "trendLines": [],
				    "graphs": [
						{
						    //"balloonText": "<span style='font-size:13px;'>[[title]] in [[category]]:<b>[[value]]</b> [[additional]]</span>",
						    "fillAlphas": 1,
						    "title": currentType,
						    "type": "column",
						    "valueField": currentType
						}
				    ],
				    "guides": [],
				    "valueAxes": [{ "id": "ValueAxis-1", "title": "" }
				    ],
				    "allLabels": [],
				    "balloon": {},
				    "titles": [{
				        "text": "Category"
				    }],
				    "dataProvider": dataProvider
				}
			);
        currentChart = "barChart";
        currentChartObject = chart;
        chart.drillLevels = [{
            "title": "Category",
            "data": dataProvider
        }];

        chart.doSingleClick = function (event) {
        }
        chart.doDoubleClick = function (event) {
            var dataContext = event.item.dataContext;
            if (dataContext.data == undefined) {
                loadProductSalesData(dataContext.Code,
                    function (data) {
                        addChildProductData(dataContext.Code, data);
                        drillDown(event);
                    });
            }
            else {
                drillDown(event);
            }
        }
        chart.addListener("clickGraphItem", AmCharts.myClickHandler);
        var drillDown = function (event) {
            chart.drillLevels.push(event.item.dataContext);
            // replace data
            chart.dataProvider = event.item.dataContext.data;

            // replace title
            chart.titles[0].text = event.item.dataContext.Description;

            // add back link
            // let's add a label to go back to yearly data
            event.chart.addLabel(
              0, 25,
              "< Go back",
              undefined,
              undefined,
              undefined,
              undefined,
              undefined,
              undefined,
              'javascript:charts.barChartDrillUp();');

            // take in data and animate
            chart.validateData();
            chart.animateAgain();

        }
        return {

            drillUp: function () {

                // get level
                chart.drillLevels.pop();
                var level = chart.drillLevels[chart.drillLevels.length - 1];

                // replace data
                chart.dataProvider = level.data;

                // replace title
                chart.titles[0].text = level.title;

                // remove labels
                if (chart.drillLevels.length === 1)
                    chart.clearLabels();

                // take in data and animate
                chart.validateData();
                chart.animateAgain();
            }
        }
    },
    loadAreaCategoryChart = function () {
        var chart = AmCharts.makeChart(chartDiv, {
            "theme":"light",
            "type": "serial",
            "fontFamily": 'Open Sans',
            "color": '#888',
            "pathToImages": chartImages,
            "categoryField": "Description",
            "startDuration": 1,
            "labelsEnabled": false,
            "categoryAxis": {
                "gridPosition": "start",
                "inside": true,
                "labelRotation": -90,

            },
            "chartScrollbar": {
                "enabled": true
            },
            "trendLines": [],
            "graphs": [
                {
                    "balloonText": "[[title]] of [[category]]:[[value]]",
                    "fillAlphas": 0.7,
                    "lineAlpha": 0,
                    "title": currentType,
                    "valueField": currentType,
                    "bullet": "round",

                }
            ],
            "guides": [],
            "valueAxes": [
                {
                }
            ],
            "allLabels": [],
            "balloon": {},
            "legend": {
                "enabled": true
            },
            "titles": [
                {
                    "id": "Title-1",
                    "size": 15,
                    "text": "Category"
                }
            ],
            "dataProvider":dataProvider
        });
        chart.drillLevels = [{
            "title": "Category",
            "data": dataProvider
        }];

        chart.doSingleClick = function (event) {
        }
        chart.doDoubleClick = function (event) {
            var dataContext = event.item.dataContext;
            if (dataContext.data == undefined) {
                loadProductSalesData(dataContext.Code,
                    function (data) {
                        addChildProductData(dataContext.Code, data);
                        drillDown(event);
                    });
            }
            else {
                drillDown(event);
            }
        }
        chart.addListener("clickGraphItem", AmCharts.myClickHandler);
        var drillDown = function (event) {
            chart.drillLevels.push(event.item.dataContext);
            // replace data
            chart.dataProvider = event.item.dataContext.data;

            // replace title
            chart.titles[0].text = event.item.dataContext.Description;

            // add back link
            // let's add a label to go back to yearly data
            event.chart.addLabel(
              0, 25,
              "< Go back",
              undefined,
              undefined,
              undefined,
              undefined,
              undefined,
              undefined,
              'javascript:charts.areaChartDrillUp();');

            // take in data and animate
            chart.validateData();
            chart.animateAgain();

        }
        currentChart = "areaChart";
        currentChartObject = chart;
        return {

            drillUp: function () {

                // get level
                chart.drillLevels.pop();
                var level = chart.drillLevels[chart.drillLevels.length - 1];

                // replace data
                chart.dataProvider = level.data;

                // replace title
                chart.titles[0].text = level.title;

                // remove labels
                if (chart.drillLevels.length === 1)
                    chart.clearLabels();

                // take in data and animate
                chart.validateData();
                chart.animateAgain();
            }
        }
        //clickGraph
        };
    return {


        init: function () {
            
            $('#' + chartDiv).closest('.portlet').find('.fullscreen').click(function () {
                //
                toggleFullWidth();
                 currentChartObject.invalidateSize();
            });

            $('#' + chartDiv).closest('.portlet').find('.reload').click(function () {
                loadCategorySalesChart();
            });

            $("#" + chartDiv).addClass(chartHeight);
            currentType = $("#chart-type option:selected").val();
            $("#chart-type").on("change", function () {
                currentType = $(this).val();
                eval("charts." + currentChart + "()");
            });

            loadCategorySalesChart();

            $('input[type=radio][name=optionsRadios]').change(function () {
                eval($(this).val());
            });
        },
        pieChart: function () {
            loadPieCategoryChart();
        },
        columnChart:function(){
            loadCategoryChart();
        },
        barChart: function () {
            loadBarCategoryChart();
        },
        areaChart: function () {
            loadAreaCategoryChart();
        },
        getDataProvider: function () {
            return dataProvider;
        },
        pieChartDrillUp:function()
        {
            loadPieCategoryChart().drillUp();
        },
        columnChartDrillUp:function()
        {
            loadCategoryChart().drillUp();
        },
        barChartDrillUp:function()
        {
            loadBarCategoryChart().drillUp();
        },
        areaChartDrillUp:function(){
            loadAreaCategoryChart().drillUp();
        }
        
    };
}();