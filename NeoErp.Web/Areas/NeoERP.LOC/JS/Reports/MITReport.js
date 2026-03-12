
//var urltest = window.location.protocol + "//" + window.location.host + "/api/LcReport/GetExchangeGainLossReports";
var urltest = window.location.protocol + "//" + window.location.host + "/api/LcReport/GetMITReports";


$(document).ready(function () {
  

    // init function for the data load
    DateFilter.init(function () {
        //consolidate.init(function () {
        debugger;

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
                    ORDER_NO: { type: "string" },
                    ORDER_DATE: { type: "date" },
                    ITEM_CODE: {type:"number"},
                    LC_TRACK_NO: { type: "string" },
                    WEEK_LC_NO: { type: "string" },
                    DO_NO: { type: "string" },
                    CI_NO: { type: "string" },
                    ITEM_EDESC: { type: "string" },                    
                    ORDER_QTY: { type: "number" },
                    IN_QUANTITY: {type: "number"},
                    PENDING_PO_QTY: { type: "number" },
                    AT_PORT: { type: "number" },
                    LC_WEEK: { type: "number" },
                    CURRENT_WEEK: { type: "number" },
                    DUE: { type: "number" },
                    ATA: { type: "number" },

                }
            }
        },
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
            fileName: "MIT Report",
            allPages: true
        },
        excelExport: function (e) {
            debugger;
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
            //                //var test = myCustomFunctionWithTitle(charges, headername);
            //                //sheet.rows[i].cells[ci].value = test;
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
                    value: "MIT Report",
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
            //mergeGridRows("grid", ["ORDER_NO", "ITEM_EDESC"], "ORDER_QTY");
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
                field: "ORDER_NO",
                title: "Order No",
                width:"150px"


            },
            {
                field: "ORDER_DATE",
                title: "Order Date",
                template: "#= kendo.toString(kendo.parseDate(ORDER_DATE),'dd MMM yyyy') #",
                width: "150px"



            },
            {
                field: "ITEM_CODE",
                title: "Code",
                width: "150px"
            },
            
            {
                field: "LC_TRACK_NO",
                title: "Track No",
                width: "150px"

            },
            {
                field: "WEEK_LC_NO",
                title: "Lc No",
                width: "150px"
               

            },
            {
                field: "DO_NO",
                title: "Do No",
                width: "150px"

            },
            {
                field: "CI_NO",
                title: "Ci No",
                width: "150px"

            },
            
            {
                field: "ITEM_EDESC",
                title: "Item",
                width: "250px"

            },
            {
                field: "ORDER_QTY",
                title: "Order Quantity",
                width: "150px"

            },
            {
                field: "IN_QUANTITY",
                title: "In Quantity",
                width: "150px"

            },
            {
                field: "PENDING_PO_QTY",
                title: "Pending Po Quantity",
                width: "150px"

            },
            {
                field: "AT_PORT",
                title: "At Port",
                width: "150px"

            },
            {
                field: "LC_WEEK",
                title: "Lc Week",
                width: "150px"

            },
            {
                field: "CURRENT_WEEK",
                title: "Current Week",
                width: "150px"

            },
            {
                field: "DUE",
                title: "Due",
                width: "150px"

            },
            {
                field: "ATA",
                title: "Ata",
                width: "150px"

            },
            //{
            //    field: "AWB_DATE",
            //    title: "Awb Date",
            //    template: "#= kendo.toString(kendo.parseDate(AWB_DATE),'dd MMM yyyy') #",

            //},


        ]
    });
}

function mergeGridRows(gridId, distinctMergeColumnField, otherMergeColumnField) {
    debugger;
    $('#' + gridId + '>.k-grid-content').find('table[role="grid"]').each(function (index, item) {
        debugger;
        var dimension_col = 1;
        // First, scan first row of headers for the "Dimensions" column.
        var columnDefinition = $('#' + gridId + '>.k-grid-header>.k-grid-header-wrap>table');//get whole data as table
        var columnIndex1 = $(columnDefinition).find("th[data-field='" + distinctMergeColumnField[0] + "']").index() + 1;
        var columnIndex2 = $(columnDefinition).find("th[data-field='" + distinctMergeColumnField[1] + "']").index() + 1;
        var columnIndex3 = $(columnDefinition).find("th[data-field='" + otherMergeColumnField + "']").index() + 1;
        
        if (columnIndex1 - 1 != -1) {
            // first_instance holds the first instance of identical td
            var first_instance = null;
            var second_instance = null;
            var third_instance = null;

            $(item).find('tr').each(function (index, trItem) {
                debugger;
                // find the td of the correct column (determined by the colTitle)
                var dimension1_td = $(trItem).find('td:nth-child(' + columnIndex1 + ')');
                var dimension2_td = $(trItem).find('td:nth-child(' + columnIndex2 + ')');
                var dimension3_td = $(trItem).find('td:nth-child(' + columnIndex3 + ')');

                if (first_instance == null) {
                    first_instance = dimension1_td;
                    second_instance = dimension2_td;
                    third_instance = dimension3_td;
                } else if (dimension1_td.text() == first_instance.text()) {
                    if (dimension2_td.text() == second_instance.text()) {
                        if (dimension3_td.text() == third_instance.text()) {
                            if (otherMergeColumnField != undefined && otherMergeColumnField.length != undefined && otherMergeColumnField.length > 0) {


                                var otherColumnIndex = $(columnDefinition).find("th[data-field='" + otherMergeColumnField + "']").index() + 1;
                                if (otherColumnIndex - 1 != -1) {
                                    $(trItem).find('td:nth-child(' + otherColumnIndex + ')').text("");
                                }
                            }
                            dimension1_td.text("");
                            dimension2_td.text("");
                            dimension3_td.text("");

                        
                    }

                        
                    }
                    // if current td is identical to the previous
                    // then remove the current td
                    // increment the rowspan attribute of the first instance
                    // first_instance.attr('rowspan', typeof first_instance.attr('rowspan') == "undefined" ? 2 : first_instance.attr('rowspan') + 1);
                } else {
                    // this cell is different from the last
                    first_instance = dimension1_td;
                    second_instance = dimension2_td;
                    third_instance = dimension3_td;
                }
            });
            return;
        }
    });
}


function KendoGridRefresh(readUrl) {
    $('#grid').val('');
    $('#grid').html('');
    BindGrid(readUrl);

}

