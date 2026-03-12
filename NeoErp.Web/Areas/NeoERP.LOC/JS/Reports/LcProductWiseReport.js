var urltest = window.location.protocol + "//" + window.location.host + "/api/LcReport/LcProductWiseReport";


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

            model: {
                fields: {
                    LC_NUMBER: { type: "string" },
                    ITEM_EDESC: { type: "string" },
                    PRE_DESC: { type: "string" },
                    OPEN_DATE: { type: "string" },
                    QUANTITY: { type: "string" },
                    TOTAL_AMOUNT: { type: "number" },
                    AMOUNT: { type: "number" },
                    CURRENCY_CODE: { type: "string" }
                }
            }
        },
        aggregate: [
            { field: "QUANTITY", aggregate: "sum" },
            { field: "AMOUNT", aggregate: "sum" },
            { field: "TOTAL_AMOUNT", aggregate: "sum" },
        ],
        pageSize: 100,
    });
    var grid = $("#grid").kendoGrid({
      
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
                        width: 50
                    });

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
                        fileName: "Lc Product Wise Report.xlsx",
                    });
                });
        },


        pdf: {
            allPages: true,
            avoidLinks: true,
            pageSize: "auto",
            //paperSize: [1100 , 1430],
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
        pageable: true,
        reorderable: true,
        groupable: true,
        resizable: true,

        pageable: {
            alwaysVisible: true,
            pageSizes: [5, 10, 20, 100]
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
        columnMenu: true,
        columnMenuInit: function (e) {
            wordwrapmenu(e);
            checkboxItem = $(e.container).find('input[type="checkbox"]');
        },
        columnShow: function (e) {
            if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                SaveReportSetting('Lc Product Wise Report Report', 'grid');
        },
        columnHide: function (e) {
            if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                SaveReportSetting('Lc Product Wise Report Report', 'grid');
        },

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
                field: "LC_NUMBER",
                title: "Lc Number",
                footerTemplate: "Total",
               
            },
            {
                field: "PRE_DESC",
                title: "Item Category",
                width: "200px"
            },
            {
                field: "ITEM_EDESC",
                title: "Item Name",
                width: "230px"
            },


            {
                field: "OPEN_DATE",
                title: "Open Date",
         

            },
            {
                field: "EXPIRY_DATE",
                title: "Expiry Date",
               

            },

            {
                field: "CURRENCY_CODE",
                title: "Currency Code",
            },
            {
                field: "QUANTITY",
                title: "Quantity",
                format: "{0:n}",
                attributes: {
                    style: "text-align: right;"

                },
                aggregates: ["sum"],
                footerTemplate: '<span style="float:right">#= kendo.toString(sum, "n")#</span>'
            },
            {
                field: "AMOUNT",
                title: "Rate",
                format: "{0:n}",
                attributes: {
                    style: "text-align: right;"
                },
                aggregates: ["sum"],
                footerTemplate: '<span style="float:right">#= kendo.toString(sum, "n")#</span>',
            },
            {
                field: "TOTAL_AMOUNT",
                title: "Total Amount",
                format: "{0:n}",
                attributes: {
                    style: "text-align: right;"
                },
                aggregates: ["sum"],
                footerTemplate: '<span style="float:right">#= kendo.toString(sum, "n")#</span>',
         
            },
        ],
    });
}



function KendoGridRefresh(readUrl) {
    $('#grid').val('');
    $('#grid').html('');
    BindGrid(readUrl);

}

