distributionModule.controller('salesPersonCtrl', function ($scope, DistSetupService, $routeParams) {
    getDateFormat = function (date) {
        var test = new Date(date);
        if (test.getFullYear() == '1970')
            return '-';
        return kendo.format("{0:" + reportConfig.dateFormat + "}", new Date(date));
    }   
    var gridUrl = window.location.protocol + "//" + window.location.host + "/api/Report/GetSalesPersonList";
    //RefreshGrid();




    function dataBind() {
        reportConfig = GetReportSetting("SalesPersonPO");
        $("#grid").kendoGrid({
            dataSource: {
                transport: {



                    read: {
                        url: gridUrl,
                        dataType: "json", // <-- The default was "jsonp".
                        type: "POST",
                        contentType: "application/json; charset=utf-8"
                    },
                    parameterMap: function (options, type) {
                        var paramMap = JSON.stringify($.extend(options, ReportFilter.filterAdditionalData()));
                        delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                        delete paramMap.$format; // <-- remove format parameter.
                        return paramMap;
                    },
                },

                pageSize: 20,
                group: {
                    field: "EMPLOYEE_EDESC", aggregates: [
                        { field: "QUANTITY", aggregate: "sum" },
                        { field: "GRAND_TOTAL_AMOUNT", aggregate: "sum" },
                        { field: "TOTAL_QUANTITY", aggregate: "sum" },
                        { field: "NET_TOTAL", aggregate: "sum" },
                        { field: "CONVERTED_QTY", aggregate: "sum" },

                    ]
                },
               // group: { field: "EMPLOYEE_EDESC" },
                pageSize: reportConfig.defaultPageSize,
                schema: {
                    parse: function (data) {
                        for (var i = 0; i < data.length; i++) {
                            if (data[i].ORDER_DATE) {
                                var dt = new Date(data[i].ORDER_DATE);
                                data[i].ORDER_TIME = kendo.toString(dt, "hh:mm tt");
                                //data[i].ORDER_DATE = kendo.toString(dt, "MM/dd/yyyy");
                            }
                        }
                        return data;
                    },
                    model: {
                        fields: {
                            ORDER_TIME: { type: "string" },
                            ORDER_NO: { type: "number" },
                            ORDER_DATE: { type: "date" },
                            CREATED_DATE: { type: "date" },
                            CREDIT_LIMIT: { type: "number" },
                            TOTAL_QUANTITY: { type: "number" },
                            GRAND_TOTAL_AMOUNT: { type: "number" },
                            GRAND_APPROVED_QUENTIITY: { type: "number" },
                            TOTAL_APPROVED_AMOUNT: { type: "number" },
                            CONVERSION_FACTOR: { type: "number" },
                            CONVERTED_QTY: { type: "number" },
                            QUANTITY: { type: "number" },
                            UNIT_PRICE: { type: "number" },
                            PRODUCT_CODE: { type: "string" },
                            NET_TOTAL: { type: "number" },
                        }
                    }
                },
                //model: {
                //    fields: {
                //        ORDER_NO: { type: "number" },
                //        ORDER_DATE: { type: "date" },
                //        CREATED_DATE: { type: "date" },
                //        CREDIT_LIMIT: { type: "number" },
                //        TOTAL_QUANTITY: { type: "number" },
                //        GRAND_TOTAL_AMOUNT: { type: "number" },
                //        GRAND_APPROVED_QUENTIITY: { type: "number" },
                //        TOTAL_APPROVED_AMOUNT: { type: "number" },
                //        CONVERSION_FACTOR: { type: "number" },
                //        CONVERTED_QTY: { type: "number" },
                //        QUANTITY: { type: "number" },
                //        UNIT_PRICE: { type: "number" },
                //        PRODUCT_CODE: {type: "string"},
                //        NET_TOTAL: { type: "number" },

                //    }
                //},
            },
            toolbar: kendo.template($("#toolbar-template").html()),
            excel: {
                fileName: "Sales Person Report",
                allPages: true,
            },
            pdf: {
                fileName: "Sales Person Report",
                allPages: true,
                avoidLinks: true,
                pageSize: "auto",
                margin: {
                    top: "2m",
                    right: "1m",
                    left: "1m",
                    buttom: "1m",
                },
                landscape: true,
                repeatHeaders: true,
                scale: 0.8,
            },
            height: window.innerHeight - 50,
            groupable: true,
            sortable: true,
            height: 500,
            selectable: false,
            resizable: true,
            filterable: {
                extra: false,
                operators: {
                    number: {
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        gte: "is greater than or equal to	",
                        gt: "is greater than",
                        lte: "is less than or equal",
                        lt: "is less than",
                    },
                    string: {

                        eq: "Is equal to",
                        neq: "Is not equal to",
                        startswith: "Starts with	",
                        contains: "Contains",
                        doesnotcontain: "Does not contain",
                        endswith: "Ends with",
                    },
                    date: {
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        gte: "Is after or equal to",
                        gt: "Is after",
                        lte: "Is before or equal to",
                        lt: "Is before",
                    }
                }
            },
           
            columnMenu: true,
            columnMenuInit: function (e) {
                wordwrapmenu(e);
                checkboxItem = $(e.container).find('input[type="checkbox"]');
            },
            dataBound: function (e) {
                var grid = $("#grid").data("kendoGrid");
                console.log(grid);

                if (grid.dataSource.total() == 0) {

                    var colCount = grid.columns.length;
                    $(e.sender.wrapper)
                        .find('tbody')
                        .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                    displayPopupNotification("No Data Found Given Date Filter.", "info");
                }
                UpdateReportUsingSetting("SalesPersonPO", "grid");
                $('div').removeClass('.k-header k-grid-toolbar');
            },
            columnShow: function (e) {
                if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                    SaveReportSetting('SalesPersonPO', 'grid');
            },
            columnHide: function (e) {
                if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                    SaveReportSetting('SalesPersonPO', 'grid');
            },
            pageable: {
                refresh: true,
                pageSizes: reportConfig.itemPerPage,
                buttonCount: 5
            },
            columns: [
                {
                    field: "ORDER_NO",
                    title: "Order No",
                },
                {
                    field: "ORDER_DATE",
                    title: "Order Date",
                    template: "#= kendo.toString(kendo.parseDate(ORDER_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
                    hidden: true,
                },
                {
                    field: "MITI",
                    title: "Miti",

                },
                {
                    title: "Order Time",
                    field: "ORDER_TIME",
                },
                {
                    field: "EMPLOYEE_EDESC",
                    title: "Sales Person",
                    hidden: true,
                },
                {
                    field: "CUSTOMER_EDESC",
                    title: "Customer",
                    width: "18%"
                }, 
                {
                    field: "RESELLER_NAME",
                    title: "Reseller",
                },
                {
                    field: "WHOLESELLER_NAME",
                    title: "Wholeseller",
                },
                {
                    field: "PRODUCT_CODE",
                    title: "Product Code",
                    width: "9%"
                },
                {
                    field: "ITEM_EDESC",
                    title: "Items",
                    width: "18%",
                  groupFooterTemplate:'Total',

                },
                //{
                //    field: "PARTY_TYPE_EDESC",
                //    title: "Party Type",
                //},
                {
                    field: "QUANTITY",
                    title: "Req. Qty.",
                    aggregates: ["sum"],
                    attributes:
                    {
                        style: "text-align:right;"
                    },
                    template: '<span style="float:right">#= QUANTITY #</span>',
                    groupFooterTemplate: "#= kendo.toString(sum, 'n')#",
                },
                {
                    field: "MU_CODE",
                    title: "Req. Unit",
                },
                //{
                //    field: "GRAND_TOTAL_AMOUNT",
                //    aggregates: ["sum"],
                //    title: "Total Amount",
                //    attributes:
                //    {
                //        style: "text-align:right;"
                //    },
                //   // template: '<span style="float:right">#= GRAND_TOTAL_AMOUNT #</span>',
                //    groupFooterTemplate: "#= kendo.toString(sum, 'n')#",
                //},

                {
                    field: "NET_TOTAL",
                    aggregates: ["sum"],
                    title: "Total Amount",
                    attributes:
                    {
                        style: "text-align:right;"
                    },
                    template: '#= kendo.toString(NET_TOTAL,"n")#',
                    groupFooterTemplate: "#= kendo.toString(sum, 'n')#",
                },
                {
                    title: "C. Qty.",
                    field: "TOTAL_QUANTITY",
                    aggregates: ["sum"],
                    attributes:
                    {
                        style: "text-align:right;"
                    },
                    groupFooterTemplate: "#= kendo.toString(sum, 'n')#",
                    // template: '<span style="float:right">#=function(converisioton_mu_code,qutnti, gratd total) #</span>'
                    // template: '<span style="float:right">#= CONVERSION_FACTOR*QUANTITY #</span>'
                    //template: function (data) {
                    //    //if (data.PO_CONVERSION_FACTOR == "Y") {

                    //        var QUANTITY = data.QUANTITY / data.CONVERSION_FACTOR
                    //        return QUANTITY;
                    //    //}



                    //},

                }, 
                {
                    field: "CONVERSION_MU_CODE",
                    title: "Alt. Unit",
                },
                {
                    field: "CONVERTED_QTY",
                    title: "Alt. QTY",
                    attributes:
                    {
                        style: "text-align:right;"
                    },
                    template: '#= kendo.toString(CONVERTED_QTY,"n")#',
                    groupFooterTemplate: "#= kendo.toString(sum, 'n')#",
                },
                {
                    field: "EntityName",
                    title: "Entity Name"
                },
                
                
            ]
        });
    }
    function RefreshGrid() {
        var gird = $("#grid").data("kendoGrid");
        $('#grid').data().kendoGrid.destroy();
        $('#grid').empty();
        dataBind();
    }
    
    DateFilter.init(function () {
        consolidate.init(function () {
            dataBind();
        });
    });

    $(".applydp").on("click", function (evt) {
        evt.preventDefault();
        RefreshGrid();
    });

    $("#advanceFilterApply").on("click", function (evt) {
        evt.preventDefault();
        var names = [];
        $("input[class='Status']:checked").each(function () {             
            names.push(this.name);
        });        
        filterStatus = names.join(',');
        reportConfig = GetReportSetting("SalesPersonPO");
        gridUrl = window.location.protocol + "//" + window.location.host + "/api/Report/GetSalesPersonList?requestStatus=" + filterStatus;
        RefreshGrid();
        });
    

    $("#RunQuery").click(function (evt) {
        evt.preventDefault();
        RefreshGrid();
    });

    $("#loadAdvancedFilters").click(function (evt) {
        evt.preventDefault();
        RefreshGrid();
    });
});

distributionModule.controller('spDistanceCtrl', function ($scope) {
    function BindGrid() {
        $("#distanceGrid").kendoGrid({  //angular in kendo grid does not bind with DateFilter.init(). So, jQuery is used
        //$scope.distanceGrid = {
            dataSource: {
                type: "JSON",
                transport: {
                    read: {
                        url: window.location.protocol + "//" + window.location.host + "/api/Report/GetSPDistanceReport", // <-- Get data from here.
                        dataType: "json", // <-- The default was "jsonp".
                        type: "POST",
                        contentType: "application/json; charset=utf-8"
                    },
                    parameterMap: function (options, type) {
                        console.log("Options:", options);
                        var additionalData = ReportFilter.filterAdditionalData();
                        console.log("Additional Data:", additionalData);
                        var paramMap = JSON.stringify($.extend(options, additionalData));
                        //var paramMap = JSON.stringify($.extend(options, ReportFilter.filterAdditionalData()));
                        delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                        delete paramMap.$format; // <-- remove format parameter.
                        return paramMap;
                    },
                },
                pageSize: 100,
                //schema: {
                //    model: {
                //        fields: {
                //            StockDate: { type: "date" },
                //        }
                //    }
                //},
                error: function (e) {
                    displayPopupNotification("Sorry error occured while processing data", "error");
                },
            },
            toolbar: kendo.template($("#toolbar-template").html()),
            height: window.innerHeight - 100,
            sortable: true,
            reorderable: true,
            groupable: true,
            resizable: true,
            pageable: {
                refresh: true,
                pageSizes: [100, 200, 500, 1000],
                buttonCount: 2
            },
            filterable: {
                extra: false,
                operators: {
                    number: {
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        gte: "is greater than or equal to	",
                        gt: "is greater than",
                        lte: "is less than or equal",
                        lt: "is less than",
                    },
                    string: {

                        eq: "Is equal to",
                        neq: "Is not equal to",
                        startswith: "Starts with	",
                        contains: "Contains",
                        doesnotcontain: "Does not contain",
                        endswith: "Ends with",
                    },
                    date: {
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        gte: "Is after or equal to",
                        gt: "Is after",
                        lte: "Is before or equal to",
                        lt: "Is before",
                    }
                }
            },
            dataBound: function (o) {
                GetSetupSetting("spDistanceReport");
                var grid = this;
                if ($("#distanceGrid").data("kendoGrid").dataItems().length == 0) {
                    var colCount = grid.columns.length;
                    $(o.sender.wrapper)
                        .find('tbody')
                        .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                    displayPopupNotification("No Data Found.", "info");
                }
                //else {
                //    grid.tbody.find("tr").dblclick(function (e) {
                //        if (e.srcElement.className == "k-icon k-i-expand" || e.srcElement.className == "k-hierarchy-cell" || e.srcElement.className == "k-icon k-i-collapse")
                //            return;
                //        e.preventDefault();
                //        updateEvent(grid.dataItem(this));
                //    });
                //}

                $('div').removeClass('.k-header k-grid-toolbar');
            },
            columns: [{
                field: "EMPLOYEE_EDESC",
                title: "Employee Name",
                width: "60%",
            }, {
                field: "DISTANCE",
                title: "Total Distance (KM)",
                width: "40%"
            }]
        });
    }

    DateFilter.init(function () {
        BindGrid();
    });

    //$scope.$on('$routeChangeSuccess', function () {
    //    $("#RunQuery").trigger('click');
    //});
});