distributionModule.controller('SchemeReportCtrl', function ($scope, $http, $routeParams) {

    $scope.showGrid = true;

    $scope.distSchemeSelectOptions = {

        dataTextField: "SchemeName",
        dataValueField: "SchemeID",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Scheme...</strong></div>',
        placeholder: "Select Scheme...",
        autoClose: false,
        dataBound: function (e) {
          
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Report/GetSchemeName",
                    dataType: "json"
                }
            }
        },
        change: function (e) {
            debugger;
            var selectedSchemeID = $scope.selecteddistScheme[0];
            
            if (selectedSchemeID != undefined) {
                dataBind(selectedSchemeID);

            }

        }

    };

    //var gridUrl = window.location.protocol + "//" + window.location.host + "/api/Report/GetSalesPersonList";
    var gridUrl = window.location.protocol + "//" + window.location.host + "/api/Report/GetSchemeSalesPersonList?SchemeID";
    function dataBind(SchemeId) {
        debugger;
    $scope.showGrid = false;

        //reportConfig = GetReportSetting("SalesPersonPO");
        $("#grid").kendoGrid({
            dataSource: {
                transport: {



                    read: {
                        url: window.location.protocol + "//" + window.location.host + "/api/Report/GetSchemeSalesPersonList?SchemeID=" + SchemeId,
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
                //group: {
                //    field: "EMPLOYEE_EDESC", aggregates: [
                //        { field: "QUANTITY", aggregate: "sum" },
                //        { field: "GRAND_TOTAL_AMOUNT", aggregate: "sum" },
                //        { field: "TOTAL_QUANTITY", aggregate: "sum" },
                //        { field: "NET_TOTAL", aggregate: "sum" }

                //    ]
                //},
                // group: { field: "EMPLOYEE_EDESC" },
                //pageSize: reportConfig.defaultPageSize,
                model: {
                    fields: {
                        ORDER_NO: { type: "number" },
                        ORDER_DATE: { type: "date" },
                        CREATED_DATE: { type: "date" },
                        CREDIT_LIMIT: { type: "number" },
                        TOTAL_QUANTITY: { type: "number" },
                        GRAND_TOTAL_AMOUNT: { type: "number" },
                        GRAND_APPROVED_QUENTIITY: { type: "number" },
                        TOTAL_APPROVED_AMOUNT: { type: "number" },
                        CONVERSION_FACTOR: { type: "number" },
                        QUANTITY: { type: "number" },
                        UNIT_PRICE: { type: "number" },
                        NET_TOTAL: { type: "number" },

                    }
                },
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
                if (grid.dataSource.total() == 0) {

                    var colCount = grid.columns.length;
                    $(e.sender.wrapper)
                        .find('tbody')
                        .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                    displayPopupNotification("No Data Found Given Date Filter.", "info");
                }
                else {
                    if (grid.dataSource.data()[0].PO_CONVERSION_FACTOR == "N") {

                        grid.hideColumn("TOTAL_QUANTITY");

                    };
                    if (grid.dataSource.data()[0].PO_CONVERSION_UNIT == "N") {
                        grid.hideColumn("CONVERSION_MU_CODE");
                    }
                }
                //else {
                //    var g = $("#grid").data("kendoGrid");
                //    for (var i = 0; i < g.columns.length; i++) {
                //        g.showColumn(i);
                //    }
                //    $("div.k-group-indicator").each(function (i, v) {
                //        g.hideColumn($(v).data("field"));
                //    });
                //}

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
                //pageSizes: reportConfig.itemPerPage,
                buttonCount: 5
            },
            columns: [
                {
                    field: "ORDER_NO",
                    title: "Order No",
                    width: "100px",
                },
                {
                    field: "ORDER_DATE",
                    title: "Order Date",
                    template: "#= kendo.toString(kendo.parseDate(ORDER_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
                    hidden: true,
                    width: "200px",
                },
                {
                    field: "MITI",
                    title: "Miti",
                    width: "200px",

                },
                {
                    field: "EMPLOYEE_EDESC",
                    title: "Sales Person",
                    width: "250px",
                },
                {
                    field: "CUSTOMER_EDESC",
                    title: "Customer",
                    width: "300px",
                },
                {
                    field: "RESELLER_NAME",
                    title: "Reseller",
                    width: "300px",
                },
                {
                    field: "ITEM_EDESC",
                    title: "Items",
                    width: "300px",
                    groupFooterTemplate: 'Total',

                },
                {
                    field: "UNIT_PRICE",
                    title: "Rate",
                    width: "100px",
                },
                {
                    field: "QUANTITY",
                    title: "Req. Qty.",
                    aggregates: ["sum"],
                    width: "100px",
                    attributes:
                    {
                        style: "text-align:right;"
                    },
                    template: '<span style="float:right">#= QUANTITY #</span>',
                    groupFooterTemplate: "#= kendo.toString(sum, 'n')#",
                },
                //{
                //    field: "MU_CODE",
                //    title: "Req. Unit",
                //},
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
                    width: "150px",
                    attributes:
                    {
                        style: "text-align:right;"
                    },
                    template: '#= kendo.toString(NET_TOTAL,"n")#',
                    groupFooterTemplate: "#= kendo.toString(sum, 'n')#",
                },
                {
                    field: "SchemeName",
                    title: "Current Scheme",
                    width: "150px",
                },
                {
                    field: "CurrentSlot",
                    title: "Current Slot",
                    width: "250px",
                },
                {
                    field: "SchemeDiscount_Qty",
                    title: "Scheme Discount Quantity",
                    width: "250px",
                },
                {
                    field: "DiscountType",
                    title: "Scheme Discount Type",
                    width: "250px",
                },
                {
                    field: "ActualDiscount",
                    title: "Actual Discount Amount",
                    width: "250px",
                },
                {
                    field: "ActualAmount",
                    title: "Actual Amount",
                    width: "250px",
                },
                {
                    field: "NextScheme",
                    title: "Next Scheme",
                    width: "250px",
                },
                {
                    field: "NextSchemeTarget",
                    title: "Next Scheme Target",
                    width: "250px",
                },
                {
                    field: "NextDiscount",
                    title: "Next Discount",
                    width: "250px",
                },
                {
                    field: "NextDiscountType",
                    title: "Next Discount Type",
                    width: "250px",
                },
                //{

                //    title: "C. Qty.",
                //    field: "TOTAL_QUANTITY",
                //    aggregates: ["sum"],
                //    attributes:
                //    {
                //        style: "text-align:right;"
                //    },
                //    groupFooterTemplate: "#= kendo.toString(sum, 'n')#",
                //    // template: '<span style="float:right">#=function(converisioton_mu_code,qutnti, gratd total) #</span>'
                //    // template: '<span style="float:right">#= CONVERSION_FACTOR*QUANTITY #</span>'
                //    template: function (data) {
                //        if (data.PO_CONVERSION_FACTOR == "Y") {

                //            var QUANTITY = data.CONVERSION_FACTOR * data.QUANTITY
                //            return QUANTITY;
                //        }



                //    },

                    // template: '<span style="float:right">#= QUANTITY #</span>'


                //},
                //{
                //    field: "EntityName",
                //    title: "Entity Name"
                //},
                //{
                //    field: "CONVERSION_MU_CODE",
                //    title: "C. Unit(KG)",
                //},

            ]
        });
    }

    //$scope.grid = {
    //    dataSource: {
    //        type: "json",
    //        transport: {
    //            read: {
    //                url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetAllScheme",
    //                dataType: "json",
    //                type: "GET",
    //                contentType: "application/json; charset=utf-8"
    //            },
    //            parameterMap: function (options, type) {
    //                var paramMap = JSON.stringify($.extend(options, ReportFilter.filterAdditionalData()));
    //                delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
    //                delete paramMap.$format; // <-- remove format parameter.
    //                return paramMap;
    //            }
    //        },
    //        schema: {
    //            //data: "data",
    //            parse: function (data) {
    //                debugger;
    //                //for (let i = 0; i < data.data.length; i++) {
    //                //    data.data[i].Location = (data.data[i].LATITUDE && data.data[i].LONGITUDE) ? (data.data[i].LATITUDE + "," + data.data[i].LONGITUDE) : "Missing"
    //                //}
    //                //if (data.inActive > 0) {
    //                //    Metronic.alert({
    //                //        container: $('.content').val(), // alerts parent container
    //                //        place: 'append', // append or prepent in container
    //                //        type: 'success', // alert's type
    //                //        message: data.inActive + " new outlets are waiting for activation.", // alert's message
    //                //        close: true, // make alert closable
    //                //        reset: true, // close all previouse alerts first
    //                //        focus: true, // auto scroll to the alert after shown
    //                //        closeInSeconds: 10, // auto close after defined seconds
    //                //        icon: 'fa fa-user' // put icon class before the message
    //                //    });
    //                //}
    //                return data;
    //            },
    //            model: {
    //                fields: {
    //                    StartDate: { type: "date" },
    //                    EndDate: { type: "date" },
    //                }
    //            },
    //        },
    //        error: function (e) {
    //            displayPopupNotification("Sorry error occured while processing data", "error");
    //        },
    //        //pageSize: reportConfig.defaultPageSize,
    //    },
    //    toolbar: kendo.template($("#toolbar-template").html()),
    //    //excel: {
    //    //    fileName: "Reseller Setup",
    //    //    allPages: true,
    //    //},
    //    //excelExport: function (e) {
    //    //    ExportToExcel(e);
    //    //    e.preventDefault();
    //    //},
    //    //pdf: {
    //    //    fileName: "Received Schedule",
    //    //    allPages: true,
    //    //    avoidLinks: true,
    //    //    pageSize: "auto",
    //    //    margin: {
    //    //        top: "2m",
    //    //        right: "1m",
    //    //        left: "1m",
    //    //        buttom: "1m",
    //    //    },
    //    //    landscape: true,
    //    //    repeatHeaders: true,
    //    //    scale: 0.8,
    //    //},
    //    height: window.innerHeight - 50,
    //    sortable: true,
    //    reorderable: true,
    //    groupable: true,
    //    resizable: true,
    //    filterable: {
    //        extra: false,
    //        operators: {
    //            number: {
    //                eq: "Is equal to",
    //                neq: "Is not equal to",
    //                gte: "is greater than or equal to	",
    //                gt: "is greater than",
    //                lte: "is less than or equal",
    //                lt: "is less than",
    //            },
    //            string: {

    //                eq: "Is equal to",
    //                neq: "Is not equal to",
    //                startswith: "Starts with	",
    //                contains: "Contains",
    //                doesnotcontain: "Does not contain",
    //                endswith: "Ends with",
    //            },
    //            date: {
    //                eq: "Is equal to",
    //                neq: "Is not equal to",
    //                gte: "Is after or equal to",
    //                gt: "Is after",
    //                lte: "Is before or equal to",
    //                lt: "Is before",
    //            }
    //        }
    //    },
    //    columnMenu: true,
    //    columnMenuInit: function (e) {
    //        wordwrapmenu(e);
    //        checkboxItem = $(e.container).find('input[type="checkbox"]');
    //    },
    //    columnShow: function (e) {
    //        if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
    //            SaveReportSetting('ResellerSetup', 'grid');
    //    },
    //    columnHide: function (e) {
    //        if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
    //            SaveReportSetting('ResellerSetup', 'grid');
    //    },
    //    pageable: {
    //        refresh: true,
    //        //pageSizes: reportConfig.itemPerPage,
    //        buttonCount: 5
    //    },
    //    //scrollable: {
    //    //    virtual: true
    //    //},
    //    pageable: true,
    //    dataBound: function (o) {
    //        debugger;
    //        //GetSetupSetting("ResellerSetup");
    //        var grid = o.sender;
    //        if (grid.dataSource.data().length == 0) {
    //            var colCount = grid.columns.length;
    //            $(o.sender.wrapper)
    //                .find('tbody')
    //                .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
    //            displayPopupNotification("No Data Found.", "info");
    //        }
    //        else {
    //            var g = $("#grid").data("kendoGrid");
    //            for (var i = 0; i < g.columns.length; i++) {
    //                g.showColumn(i);
    //            }
    //            $("div.k-group-indicator").each(function (i, v) {
    //                g.hideColumn($(v).data("field"));
    //            });
    //        }

    //        //UpdateReportUsingSetting("ResellerSetup", "grid");
    //        if (grid.dataSource.data().length == 0) {
    //            var colCount = grid.columns.length;
    //            $(o.sender.wrapper)
    //                .find('tbody')
    //                .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
    //            displayPopupNotification("No Data Found.", "info");
    //        }
    //        else {
    //            var g = $("#grid").data("kendoGrid");
    //            for (var i = 0; i < g.columns.length; i++) {
    //                g.showColumn(i);
    //            }
    //            $("div.k-group-indicator").each(function (i, v) {
    //                g.hideColumn($(v).data("field"));
    //            });
    //        }

    //        //UpdateReportUsingSetting("ResellerSetup", "grid");
    //        $('div').removeClass('.k-header k-grid-toolbar');
    //    },
    //    columns: [{
    //        field: "SchemeName",
    //        title: "Scheme Name",
    //        width: "120px"
    //    },
    //    {
    //        field: "StartDate",
    //        title: "Start Date",
    //        format: "{0:MM/dd/yyyy}",
    //        width: "120px"
    //    },
    //    {
    //        field: "EndDate",
    //        title: "End Date",
    //        format: "{0:MM/dd/yyyy}",
    //        width: "120px"
    //    },
    //    {
    //        field: "AreaName",
    //        title: "Area",
    //        width: "120px"
    //    },
    //    {
    //        field: "OfferType",
    //        title: "Scheme Type",
    //        width: "120px"
    //    },

    //    {
    //        title: "Action",
    //        template: " <a class='fa fa-edit editAction' ng-click='updateScheme($event)' title='Edit'></a>&nbsp &nbsp<a class='fa fa-trash-o deleteAction' ng-click='deleteScheme($event)' title='delete'></a> ",
    //        width: "8%"
    //    }
    //    ]
    //};

    $scope.schemeItems = function (dataItem) {
        debugger;
        if (dataItem.OfferType == "GIFT") {
            $scope.showOthers = false;
        }
        else {
            $scope.showOthers = true;

        }
        var id = dataItem.SchemeID;
        return {
            dataSource: {
                type: "json",
                transport: {
                    read: {
                        url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetSchemeItem?SchemeID=" + id,
                        dataType: "json",
                        type: "GET",
                        contentType: "application/json; charset=utf-8"
                    },
                },
                schema: {
                    parse: function (data) {
                        debugger;
                        //getgiftItems(id);
                        return data.DATA;
                    }
                },
                error: function (e) {
                    displayPopupNotification("Sorry error occured while processing data", "error");
                },
            },
            scrollable: false,
            sortable: false,
            pageable: false,
            columns: [
                { field: "ITEM_EDESC", title: "Items", width: "100%" },
            ]
        };
    };


    //var getgiftItems = function (id) {
    //    debugger;
    //    var response = $http({
    //        method: "get",
    //        url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetGiftItem?SchemeID=" + id,
    //        dataType: "json"
    //    }).then(function (response) {
    //        debugger;
    //        $scope.giftItems = response.data.DATA;
    //    });
    //    debugger;
    //    return response;
    //}

    var getGiftItems = function (dataItem) {

        debugger;
        var schemeid = dataItem.SchemeID;

    };


    $scope.others = function (dataItem) {
        debugger;
        var id = dataItem.SchemeID;
        return {
            dataSource: {
                type: "json",
                transport: {
                    read: {
                        url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetOtherItem?SchemeID=" + id,
                        dataType: "json",
                        type: "GET",
                        contentType: "application/json; charset=utf-8"
                    },
                },
                schema: {
                    parse: function (data) {
                        debugger;
                        return data.DATA;
                    }
                },
                error: function (e) {
                    displayPopupNotification("Sorry error occured while processing data", "error");
                },
            },
            scrollable: false,
            sortable: false,
            pageable: false,
            columns: [
                { field: "Max_Value", title: "Max Quantity", width: "100%" },
                { field: "Min_Value", title: "Min Quantity", width: "100%" },
                { field: "Discount", title: "Discount", width: "100%" },
                { field: "DiscountType", title: "Discount Type", width: "100%" },
            ]
        };
    };

});