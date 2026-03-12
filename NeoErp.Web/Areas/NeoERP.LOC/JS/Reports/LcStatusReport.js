
var urltest = window.location.protocol + "//" + window.location.host + "/api/LcReport/LcStatusReport";
var urlchildtemplate = window.location.protocol + "//" + window.location.host + "/api/LcReport/LcChildStatus?lctrackno=";

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

        serverGrouping: false,
        serverAggregates: false,
        serverPaging: true,
        schema: {
            aggregates: "AggregationResult",
            model: {
                fields: {
                    LC_TRACK_NO: { type: "string" },
                    LC_NUMBER: { type: "string" },
                    TOTAL_RECEIVED_AMOUNT: { type: "number" },
                    LC_AMOUNT: { type: "number" },
                    BALANCE_AMOUNT: { type: "number" },
                }
            }
        },
        aggregate: [
            { field: "TOTAL_RECEIVED_AMOUNT", aggregate: "sum" },
            { field: "LC_AMOUNT", aggregate: "sum" },
            { field: "BALANCE_AMOUNT", aggregate: "sum" },
            
        ],
        pageSize: 100,
    });
    var grid = $("#grid").kendoGrid({
        toolbar: ["excel"],
        dataSource: dataSource,
        toolbar: kendo.template($("#toolbar-template").html()),
        excel: {
            allPages: true
        },
        excelExport: function (e) {
            e.preventDefault();
            var workbook = e.workbook;
            var detailExportPromises = [];
            var masterData = e.data;
            for (var rowIndex = 0; rowIndex < masterData.length; rowIndex++) {
                var itemgrid = $($('.orders')[rowIndex]).data("kendoGrid")
                if (itemgrid != undefined) {
                    if (masterData[rowIndex].LC_TRACK_NO != itemgrid.dataSource.data()[0].LC_TRACK_NO) {
                        isEqual = false;

                    }
                    else {
                        exportChildData(masterData[rowIndex].LC_TRACK_NO, rowIndex, detailExportPromises);

                    }
                }
            }
            $.when.apply(null, detailExportPromises)
                .then(function () {
                    // get the export results
                    var detailExports = $.makeArray(arguments);

                    // sort by masterRowIndex
                    detailExports.sort(function (a, b) {
                        return a.masterRowIndex - b.masterRowIndex;
                    });

                    // add an empty column
                    workbook.sheets[0].columns.unshift({
                        width: 30
                    });

                    // prepend an empty cell to each row
                    var rows = workbook.sheets[0].rows;

                    // prepend an empty cell to each row
                    for (var i = 0; i < rows.length; i++) {
                        //workbook.sheets[0].rows[i].cells.unshift({});
                        var row = rows[i];
                        if (row.type == "group-footer" || row.type == "footer") {
                            for (var ci = 0; ci < row.cells.length; ci++) {
                                var cell = row.cells[ci];
                                if (cell.value) {
                                    // Use jQuery.fn.text to remove the HTML and get only the text
                                    cell.value = $(cell.value).text();
                                    // Set the alignment
                                    cell.hAlign = "right";
                                }
                            }
                        }

                    }

                    // merge the detail export sheet rows with the master sheet rows
                    // loop backwards so the masterRowIndex doesn't need to be updated
                    for (var i = detailExports.length - 1; i >= 0; i--) {
                        var masterRowIndex = detailExports[i].masterRowIndex + 1; // compensate for the header row

                        var sheet = detailExports[i].sheet;

                        // prepend an empty cell to each row
                        for (var ci = 0; ci < sheet.rows.length; ci++) {
                            if (sheet.rows[ci].cells[0].value) {
                                sheet.rows[ci].cells.unshift({});
                            }
                            var row = sheet.rows[i];
                            if (row.type == "group-footer" || row.type == "footer") {
                                for (var ci = 0; ci < row.cells.length; ci++) {
                                    var cell = row.cells[ci];
                                    if (cell.value) {
                                        // Use jQuery.fn.text to remove the HTML and get only the text
                                        cell.value = $(cell.value).text();
                                        // Set the alignment
                                        cell.hAlign = "right";
                                    }
                                }
                            }
                        }

                        // insert the detail sheet rows after the master row
                        [].splice.apply(workbook.sheets[0].rows, [masterRowIndex + 1, 0].concat(sheet.rows));
                    }

                    // save the workbook
                    kendo.saveAs({
                        dataURI: new kendo.ooxml.Workbook(workbook).toDataURL(),
                        fileName: "LC Status Report.xlsx"
                    });
                });
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
     
        pageable: true,
     
        detailTemplate: kendo.template($("#template").html()),
        detailInit: detailInit,
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


         
        },
        columns: [
            {
                field: "LC_TRACK_NO",
                title: "Track No",
                width: "50px",
                footerTemplate: "Total"

            },
            {
                field: "LC_NUMBER",
                title: "Lc Number",
                width: "200px"
            },
            {
                field: "OPEN_DATE",
                title: "Open Date",
                format: "{0:dd-MMM-yyyy}",

            },
            {
                field: "EXPIRY_DATE",
                title: "Expiry Date",
                format: "{0:dd-MMM-yyyy}",

            },

            {
                field: "LAST_SHIPMENT_DATE",
                title: "Last Shipment Date",
                format: "{0:dd-MMM-yyyy}",

            },
            {
                field: "STATUS_EDESC",
                title: "Status",
            },
            {
                field: "LC_CURRENCY",
                title: "Currency",

            },
            {
                field: "TOTAL_RECEIVED_AMOUNT",
                title: "Total Received Amount",
                attributes: {
                    style: "text-align: right;"

                },
                format: "{0:n}",            
                aggregates: ["sum"],
                footerTemplate: '<span style="float:right">#= kendo.toString(sum, "n")#</span>',
            },
            {
                field: "LC_AMOUNT",
                title: "Lc Amount",
                attributes: {
                    style: "text-align: right;"

                },
                format: "{0:n}",           
                aggregates: ["sum"],
                footerTemplate: '<span style="float:right">#= kendo.toString(sum, "n")#</span>',
            },
            {
                field: "BALANCE_AMOUNT",
                title: "Balance Amount",
                attributes: {
                    style: "text-align: right;"

                },
                format: "{0:n}",             
                aggregates: ["sum"],
                footerTemplate: '<span style="float:right">#= kendo.toString(sum, "n")#</span>',
            },
           
        ]
    });
}

var lctrackno = "";
var totalAmount = "";
var totalQty = "";


function detailInit(e) {
    var detailRow = e.detailRow;
    lctrackno = e.data.LC_TRACK_NO;
    totalAmount = e.data.PFI_AMOUNT;
    totalQty = e.data.TOTAL_QTY;
    detailRow.find(".tabstrip").kendoTabStrip({
        animation: {
            open: { effects: "fadeIn" }
        }
    });

    detailRow.find(".orders").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: urlchildtemplate + lctrackno
            },
            serverPaging: true,
            serverSorting: true,

            pageSize: 7,
            schema: {
                aggregates: "AggregationResult",
                model: {
                    fields: {
                        INVOICE_DATE: { type: "date" },
                        DUE_DATE: { type: "date" },
                        INVOICE_AMOUNT: { type: "number" },
                        INVOICE_NUMBER: { type: "string" },
                        LC_TRACK_NO: { type: "number" }
                    }
                }
            },
            group: {
                field: "INVOICE_NUMBER", aggregates: [
                    { field: "INVOICE_AMOUNT", aggregate: "sum" },
                    { field: "QUANTITY", aggregate: "sum" },
                ]
            },
            aggregate: [
                { field: "INVOICE_AMOUNT", aggregate: "sum" },
                { field: "QUANTITY", aggregate: "sum" },
            ],
            filter: {
                field: "LC_TRACK_NO",
                operator: "eq",
                value: e.data.LC_TRACK_NO
            },

        },

        scrollable: false,
        sortable: true,
        pageable: true,
        resizable: true,
        excelExport: function (e) {
            // prevent saving the file
            e.preventDefault();

            // resolve the deferred
            //deferred.resolve({
            //  masterRowIndex: masterRowIndex,
            //  sheet: e.workbook.sheets[0]
            //});
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
        columns: [
      
            { field: "ITEM_EDESC", title: "Item Name" },

            { field: "INVOICE_DATE", title: "Invoice Date", format: "{0:dd-MMM-yyyy}", },

            { field: "INVOICE_NUMBER", title: "Invoice Number", footerTemplate: "<span>Balance</span>", groupFooterTemplate: "<span>Total</span>" },

            { field: "INVOICE_AMOUNT", title: "CI Amount", format: "{0:n}", attributes: { style: "text-align: right;" }, footerTemplate: '<span style="float: right">#= kendo.toString(calculateBalanceAmount(sum,' + totalAmount + '),"n") #</span>', groupFooterTemplate: '<span style="float:right">#= kendo.toString(sum, "n")#</span>' },

            { field: "QUANTITY", title: "CI Quantity", format: "{0:n}", attributes: { style: "text-align: right;" }, footerTemplate: '<span style="float:right">#= kendo.toString(calculateBalanceQuantity(sum,' + totalQty + '),"n") #</span>', groupFooterTemplate: '<span style="float:right">#= kendo.toString(sum, "n")#</span>' },

       
            { field: "CREDIT_DAYS", title: "Credit Days" },

            { field: "DUE_DATE", title: "Due Date", format: "{0:dd-MMM-yyyy}", },

            { field: "PARENT_NAME", title: "Product Group" },
        ]
    });
}

function exportChildData(lctrackno, rowIndex, detailExportPromises) {

    var deferred = $.Deferred();

    detailExportPromises.push(deferred);

    var rows = [{
        cells: [
            // First cell
            //{ value: "LC_TRACK_NO" },
            { value: "INVOICE_NUMBER" },
            { value: "ITEM_EDESC" },
            // Second cell
            { value: "INVOICE_DATE" },
            // Third cell
            { value: "INVOICE_AMOUNT" },
            // Fourth cell
            { value: "QUANTITY" },
            // Fifth cell
            { value: "CREDIT_DAYS" },
            { value: "DUE_DATE" },
            { value: "PARENT_NAME" }
        ]
    }];


    $($('.orders')[rowIndex]).data("kendoGrid").dataSource.filter({ field: "LC_TRACK_NO", operator: "eq", value: lctrackno });

    var exporter = new kendo.ExcelExporter({
        columns: [
            {
                field: "INVOICE_NUMBER", title: "Invoice Number"
            },
            {
                field: "ITEM_EDESC", title: "Item Name",
            }, {
                field: "INVOICE_DATE", title: "Invoice Date", footerTemplate: "<span>Balance</span>", groupFooterTemplate: "<span>Total</span>"
            }, {
                field: "INVOICE_AMOUNT", title: "Invoice Amount"
            }, {
                field: "QUANTITY", title: "Quantity"
            }, {
                field: "CREDIT_DAYS", title: "Credit Days",
            }, {
                field: "DUE_DATE", title: "Due Date",
            }, {
                field: "PARENT_NAME", title: "Product Group",
            }],
        dataSource: $($('.orders')[rowIndex]).data("kendoGrid").dataSource.data(),
    });
    exporter.workbook().then(function (book, data) {
        deferred.resolve({

            masterRowIndex: rowIndex,
            sheet: book.sheets[0]
        });
    });
};



function KendoGridRefresh(readUrl) {
    $('#grid').val('');
    $('#grid').html('');
    BindGrid(readUrl);

}

