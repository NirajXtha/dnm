
var urltest = window.location.protocol + "//" + window.location.host + "/api/LcReport/PoPendingReport";
//var urlchildtemplate = window.location.protocol + "//" + window.location.host + "/api/LcReport/LcChildPendingLc?lctrackno=";


$(document).ready(function () {
    // init function for the data load
    DateFilter.init(function () {
        BindGrid(urltest);
    });
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
       
            model: {
                fields: {
                    LC_NUMBER: { type: "string" },
                    ITEM_DESC: { type: "string" },
               
                    

                }
            }
        },
      
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
                        fileName: "Po Pending Report.xlsx"
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
      
        columns: [
            {
                field: "LC_NUMBER",
                title: "LC NUMBER",
            },
            {
                field: "ITEM_DESC",
                title: "ITEM NUMBER",

            },
            {
                field: "TOTAL_ITEM",
                title: "TOTAL ITEM",

            },
            {
                field: "PROCESS_1",
                title: "NO CI",

            },
            {
                field: "PROCESS_2",
                title: "TRANSIT",

            },
            {
                field: "PROCESS_3",
                title: "CI WITH ATA NO PP NO",
                title: "ITEM TO BE CLEAR",
            },
            {
                field: "PROCESS_4",
                title: "CI WITH ATA AND PP NO AND GRN",
                title: "ITEM RECEIEVED",
            }
          
        ]
    });
}

function detailInit(e) {

    var detailRow = e.detailRow;
    var lctrackno = e.data.LC_TRACK_NO;
    var brandname = e.data.BRAND_NAME;
    detailRow.find(".tabstrip").kendoTabStrip({
        animation: {
            open: { effects: "fadeIn" }
        }
    });

    detailRow.find(".orders").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: urlchildtemplate + lctrackno + "&brandname=" + brandname
            },
            serverPaging: false,
            serverSorting: false,

            pageSize: 50,
            group: { field: "BRAND_NAME", title: "Shipment Type" },
            schema: {
                aggregates: "AggregationResult",
                model: {
                    fields: {

                        AMOUNT: { type: "number" },
                        QUANTITY: { type: "number" }
                    }
                }
            },
            aggregate: [
                { field: "AMOUNT", aggregate: "sum" },
                { field: "QUANTITY", aggregate: "sum" },
            ],
            filter: {
                field: "LC_TRACK_NO",
                operator: "eq",
                value: e.data.LC_TRACK_NO
            }
        },

        scrollable: false,
        sortable: true,
        pageable: true,
        resizable: true,
        excelExport: function (e) {
            // prevent saving the file
            e.preventDefault();

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
           
            { field: "SNO", title: "SN", },
            { field: "ITEM_EDESC", title: "Item Name" },
            { field: "MU_CODE", title: "Mu Code", footerTemplate: "<span>Total</span>", },
            {
                field: "QUANTITY", title: "Quantity", format: "{0:n}", attributes: { style: "text-align: right;" }, aggregates: ["sum"],
                footerTemplate: '<span style="float:right">#= kendo.toString(sum, "n")#</span>',
            },
            {
                field: "AMOUNT", title: "Amount", format: "{0:n}", attributes: { style: "text-align: right;" }, aggregates: ["sum"],
                footerTemplate: '<span style="float:right">#= kendo.toString(sum, "n")#</span>',
            },
          
            { field: "HS_CODE", title: "Hs Codes" },
            { field: "COUNTRY_EDESC", title: "Country" },
        ]
    });

}

function exportChildData(lctrackno, rowIndex, detailExportPromises) {

    var deferred = $.Deferred();

    detailExportPromises.push(deferred);

    var rows = [{
        cells: [
            // First cell
          
            { value: "BRAND_NAME" },
            { value: "ITEM_EDESC" },
            // Second cell
            { value: "MU_CODE" },
            // Third cell
            { value: "QUANTITY" },
            // Fourth cell
            { value: "AMOUNT" },
            // Fifth cell
            { value: "HS_CODE" },
            { value: "COUNTRY_EDESC" }
        ]
    }];


    $($('.orders')[rowIndex]).data("kendoGrid").dataSource.filter({ field: "LC_TRACK_NO", operator: "eq", value: lctrackno });

    var exporter = new kendo.ExcelExporter({
        columns: [
            {
                field: "BRAND_NAME", title: "Brand Name"
            },
            {
                field: "ITEM_EDESC", title: "Item Name",
            }, {
                field: "MU_CODE", title: "Master Unit",
            }, {
                field: "QUANTITY", title: "Quantity",
            }, {
                field: "AMOUNT", title: "Amount",
            }, {
                field: "HS_CODE", title: "Hs Code",
            }, {
                field: "COUNTRY_EDESC", title: "Country Name",
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

