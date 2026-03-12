
//var urltest = window.location.protocol + "//" + window.location.host + "/api/LcReport/GetExchangeGainLossReports";
var urltest = window.location.protocol + "//" + window.location.host + "/api/LcReport/GetExchangeGLReports";


$(document).ready(function () {

    // init function for the data load
    DateFilter.init(function () {
        //consolidate.init(function () {

        BindGrid(urltest);
        //to bind checkbox

        //});
    });


    //fileMailSender.init();


});


$(".applydp").on("click", function (evt) {

    evt.preventDefault();
    $("#RunQuery").trigger("click");
});
$("#loadAdvancedFilters").on("click", function (evt) {
    evt.preventDefault();
    $("#RunQuery").trigger("click");
});


$("#RunQuery").click(function (evt) {
    evt.preventDefault();
    KendoGridRefresh(urltest);
});
function BindGrid(urltest) {

    var dataSource = new kendo.data.DataSource({

        type: "json",
        transport: {
            read: {
                url: urltest, // <-- Get data from here.
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
        error: function (e) {
            displayPopupNotification("Sorry error occured while processing data", "error");
        },
        serverFiltering: true,
        serverGrouping: false,
        serverAggregates: false,
        serverPaging: true,
        schema: {
            aggregates: "AggregationResult",
            model: {
                fields: {
                    INVOICE_DATE: { type: "string" },
                    INVOICE_NUMBER: { type: "string" },
                    LC_NUMBER: { type: "string" },
                    ITEM_EDESC: { type: "string" },
                    INVOICE_CURRENCY: { type: "string" },
                    AMT: { type: "number" },
                    PAYMENT_EXCHANGE_RATE: { type: "number" },
                    SELLING_EXCHANGE_RATE: { type: "number" },
                    //PAYMENT_DATE: { type: "string" },
                    EXNG_GAIN_LOSS: { type: "number" },
                    
                }
            }
        },
        aggregate: [

            { field: "AMT", aggregate: "sum" },
            { field: "EXNG_GAIN_LOSS", aggregate: "sum" },
        ],
        //aggregate: [
        //    { field: "PaymentRate", aggregate: "sum" },
        //    { field: "SalesRate", aggregate: "sum" },
        //    { field: "ExchangeGainLoss", aggregate: "sum" }
        //],
        pageSize: 100
    });
    //var grid = $("#grid").kendoGrid({
    //    dataSource: dataSource,
    //    toolbar: kendo.template($("#toolbar-template").html()),
    //    excel: {
    //        fileName: "Due Invoice Report",
    //        allPages: true
    //    },
    //    excelExport: function (e) {
    //        var sheet = e.workbook.sheets[0];
    //        var datasource = $("#grid").data("kendoGrid");
    //        var data = e.data;
    //        var heardervalue = [];

    //        heardervalue = sheet.rows[0].cells;
    //        for (var i = 0; i < sheet.rows.length; i++) {
    //            // sheet.rows[i].cells.reverse();

    //            for (var ci = 0; ci < sheet.rows[i].cells.length; ci++) {

    //                //  if (sheet.rows[i].cells[ci].value==)
    //                if (typeof (sheet.rows[i].cells[ci].value) == "undefined") {
    //                    var index = i - 1;
    //                    var headername = heardervalue[ci];

    //                    if (typeof (data[index]) != "undefined") {
    //                        var charges = data[index].charges;
    //                        var test = myCustomFunctionWithTitle(charges, headername);
    //                        sheet.rows[i].cells[ci].value = test;
    //                    }

    //                }
    //                if (i == sheet.rows.length - 1) {

    //                    sheet.rows[i].cells[ci].value = $(sheet.rows[i].cells[ci].value).text();
    //                }



    //            }
    //        }
    //        var rows = e.workbook.sheets[0].rows;
    //        rows.unshift({
    //            height: 20,
    //            cells: [{
    //                value: "Due Invoice Report",
    //                background: "#3fd5c0",
    //                textAlign: "center",
    //                colSpan: 10,
    //                fontSize: 15,

    //            }]
    //        });
    //    },

    //    pdf: {
    //        allPages: true,
    //        avoidLinks: true,
    //        pageSize: "auto",
    //        margin: {
    //            top: "2m",
    //            left: "1cm",
    //            right: "1cm",
    //            bottom: "1cm"
    //        },
    //        landscape: true,
    //        repeatHeaders: true,
    //        scale: 0.8
    //    },
    //    allowCopy: true,
    //    //height: 600,
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
    //            SaveReportSetting('Due Invoice Report', 'grid');
    //    },
    //    columnHide: function (e) {
    //        if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
    //            SaveReportSetting('Due Invoice Report', 'grid');
    //    },
    //    pageable: true,
    //    //scrollable: {
    //    //    virtual: true
    //    //},
    //    dataBound: function (o) {
    //        var grid = o.sender;
    //        if (grid.dataSource.total() == 0) {
    //            var colCount = grid.columns.length + 1;
    //            $(o.sender.wrapper)
    //                .find('tbody')
    //                .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
    //            displayPopupNotification("No Data Found Given Date Filter.", "info");
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
    //        $('div').removeClass('.k-header k-grid-toolbar');


    //        UpdateReportUsingSetting("FinalSalesReport", "grid");
    //    },

    //    columns: [

    //        {
    //            field: "BillDate",
    //            title: "Bill Date",
    //            //template: "#=SUPPLIER_EDESC == null ? LC_NUMBER:SUPPLIER_EDESC#",

    //        },
    //        {
    //            field: "CommercialInvoiceAmount",
    //            title: "CI Amount",
    //            //footerTemplate: '<span style="float:right">#= "Total" #</span>',
    //        },
    //        {
    //            field: "PaymentRate",
    //            title: "Payment Rate",
    //            attributes: {
    //                style: "text-align: right;"
    //            },
    //            //format: "{0:n" + ReportFilter.parseDecimalPlace('AmountRoundUpFilter').toString() + "}",
    //            //aggregates: ["sum"],
    //            //footerTemplate: '<span style="float:right">#= kendo.toString(sum, "n" + ReportFilter.parseDecimalPlace("AmountRoundUpFilter").toString()) #</span>',
    //            //groupFooterTemplate: '<span style="float:right">#= kendo.toString(sum,"n" + ReportFilter.parseDecimalPlace("AmountRoundUpFilter").toString()) #</span>',
    //        },
    //        {
    //            field: "SalesRate",
    //            title: "Sales Rate",

    //        },
    //        {
    //            field: "ExchangeGainLoss",
    //            title: "Exchange Gain Loss",

    //            attributes: {
    //                style: "text-align: right;"
    //            },
    //            //format: "{0:n" + ReportFilter.parseDecimalPlace('AmountRoundUpFilter').toString() + "}",
    //            //aggregates: ["sum"],
    //            //footerTemplate: '<span style="float:right">#= kendo.toString(sum, "n" + ReportFilter.parseDecimalPlace("AmountRoundUpFilter").toString()) #</span>',
    //            //groupFooterTemplate: '<span style="float:right">#= kendo.toString(sum,"n" + ReportFilter.parseDecimalPlace("AmountRoundUpFilter").toString()) #</span>',
    //        },
    //        //{
    //        //    field: "BalanceQuantity",
    //        //    title: "Balance Quantity",
    //        //    attributes: {
    //        //        style: "text-align: right;"
    //        //    },
    //        //    format: "{0:n" + ReportFilter.parseDecimalPlace('AmountRoundUpFilter').toString() + "}",
    //        //    aggregates: ["sum"],
    //        //    footerTemplate: '<span style="float:right">#= kendo.toString(sum, "n" + ReportFilter.parseDecimalPlace("AmountRoundUpFilter").toString()) #</span>',
    //        //    groupFooterTemplate: '<span style="float:right">#= kendo.toString(sum,"n" + ReportFilter.parseDecimalPlace("AmountRoundUpFilter").toString()) #</span>',
    //        //},
    //        //{
    //        //    field: "BalanceAmount",
    //        //    title: "Balance Amount",
    //        //    attributes: {
    //        //        style: "text-align: right;"
    //        //    },
    //        //    format: "{0:n" + ReportFilter.parseDecimalPlace('AmountRoundUpFilter').toString() + "}",
    //        //    aggregates: ["sum"],
    //        //    footerTemplate: '<span style="float:right">#= kendo.toString(sum, "n" + ReportFilter.parseDecimalPlace("AmountRoundUpFilter").toString()) #</span>',
    //        //    groupFooterTemplate: '<span style="float:right">#= kendo.toString(sum,"n" + ReportFilter.parseDecimalPlace("AmountRoundUpFilter").toString()) #</span>',
    //        //},


    //    ]
    //});
    var grid = $("#grid").kendoGrid({
        dataSource: dataSource,

        toolbar: kendo.template($("#toolbar-template").html()),
        excel: {
            fileName: "Exchange Gain Report",
            allPages: true
        },
        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];
            var datasource = $("#grid").data("kendoGrid");
            var data = e.data;
            var heardervalue = [];

            heardervalue = sheet.rows[0].cells;
            //for (var i = 0; i < sheet.rows.length; i++) {
            //    // sheet.rows[i].cells.reverse();

            //    for (var ci = 0; ci < sheet.rows[i].cells.length; ci++) {

            //        //  if (sheet.rows[i].cells[ci].value==)
            //        if (typeof (sheet.rows[i].cells[ci].value) == "undefined") {
            //            var index = i - 1;
            //            var headername = heardervalue[ci];

            //            if (typeof (data[index]) != "undefined") {
            //                var charges = data[index].charges;
            //                var test = myCustomFunctionWithTitle(charges, headername);
            //                sheet.rows[i].cells[ci].value = test;
            //            }

            //        }
            //        if (i == sheet.rows.length - 1) {

            //            sheet.rows[i].cells[ci].value = $(sheet.rows[i].cells[ci].value).text();
            //        }



            //    }
            //}
            var rows = e.workbook.sheets[0].rows;
            rows.unshift({
                height: 20,
                cells: [{
                    value: "Exchange Gain Report",
                    background: "#3fd5c0",
                    textAlign: "center",
                    colSpan: 10,
                    fontSize: 15,

                }]
            });
        },

        pdf: {
            allPages: true,
            avoidLinks: true,
            pageSize: "auto",
            margin: {
                top: "2m",
                left: "1cm",
                right: "1cm",
                bottom: "1cm"
            },
            landscape: true,
            repeatHeaders: true,
            scale: 0.8
        },
        allowCopy: true,

        //height: 600,
        sortable: true,
        reorderable: true,
        groupable: true,
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
        columnShow: function (e) {
            if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                SaveReportSetting('Due Invoice Report', 'grid');
        },
        columnHide: function (e) {
            if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                SaveReportSetting('Due Invoice Report', 'grid');
        },
        pageable: true,
        //scrollable: {
        //    virtual: true
        //},
        dataBound: function (o) {
            var grid = o.sender;
            if (grid.dataSource.total() == 0) {
                var colCount = grid.columns.length + 1;
                $(o.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                displayPopupNotification("No Data Found Given Date Filter.", "info");
            }
            else {
                var g = $("#grid").data("kendoGrid");
                for (var i = 0; i < g.columns.length; i++) {
                    g.showColumn(i);
                }
                $("div.k-group-indicator").each(function (i, v) {
                    g.hideColumn($(v).data("field"));
                });
            }
            $('div').removeClass('.k-header k-grid-toolbar');


            UpdateReportUsingSetting("FinalSalesReport", "grid");
        },

       
        columns: [

            {
                field: "INVOICE_DATE",
                title: "Bill Date",
                template: "#= kendo.toString(kendo.parseDate(INVOICE_DATE),'dd MMM yyyy') #",
                

            },
            {
                field: "INVOICE_NUMBER",
                title: "Invoice Number",
               
            },
            {
                field: "LC_NUMBER",
                title: "Lc Number",

            },
            
            {
                field: "ITEM_EDESC",
                title: "Item",
                attributes: {
                    style: "text-align: right;"
                },
               
            },
            {
                field: "AMT",
                title: "Amount",
                format: "{0:n}",
                aggregates: ["sum"],
                footerTemplate: '#= kendo.toString(sum, "n")#',
             
            },
            {
                field: "INVOICE_CURRENCY",
                title: "Currency",

                attributes: {
                    style: "text-align: right;"
                },
                
            },
            {
                field: "PAYMENT_EXCHANGE_RATE",
                title: "Payment Exchange Rate",

            },
            {
                field: "SELLING_EXCHANGE_RATE",
                title: "Sales Exchange Rate",


            },
            {
                field: "PAYMENT_DATE",
                title: "Payment Date",

            },
            {
                field: "EXNG_GAIN_LOSS",
                title: "Exchange Gain Loss",
                format: "{0:n}",
                aggregates: ["sum"],
                footerTemplate: '#= kendo.toString(sum, "n")#',
               

            },


        ]
    });
}



function KendoGridRefresh(readUrl) {
    $('#grid').val('');
    $('#grid').html('');
    BindGrid(readUrl);

}

