distributionModule.controller('itemCumulativeCtrl', function ($scope, DistSetupService, $routeParams) {
    getDateFormat = function (date) {
        var test = new Date(date);
        if (test.getFullYear() == '1970')
            return '-';
        return kendo.format("{0:" + reportConfig.dateFormat + "}", new Date(date));
    };
    function getTimeFormat(date) {
        var test = new Date(date);
        if (test.getFullYear() == '1970')
            return '-';
        return kendo.format("{0:hh:mm tt}", moment(date)._d)
    }
    var dateFlag = false;
    var gridUrl = window.location.protocol + "//" + window.location.host + "/api/Report/GetItemCumulativeReport?dateFlag=" + dateFlag;
    function bindGrid() {
        console.log("please mann");
        reportConfig = GetReportSetting("itemCumulativeReport");
        $("#grid").kendoGrid({
            dataSource: {
                transport: {
                    read: {
                        url: gridUrl,
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8"
                    },
                    parameterMap: function (options, type) {
                        var paramMap = JSON.stringify($.extend(options, ReportFilter.filterAdditionalData()));
                        delete paramMap.$inlinecount;
                        delete paramMap.$format;
                        return paramMap;

                    },
                },

                pageSize: 20,
                //  group: { field: "MITI" },
                pageSize: reportConfig.defaultPageSize,
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
                        TOTAL_QUANTITY: { type: "number" },
                        TOTAL_AMOUNT: { type: "number" },

                    }
                },
                group: [{
                    field: "EMPLOYEE_EDESC", aggregates: [
                        { field: "TOTAL_QUANTITY", aggregate: "sum" },
                        { field: "TOTAL_AMOUNT", aggregate: "sum" },

                    ]
                }, {
                    field: "MITI", aggregates: [
                        { field: "TOTAL_QUANTITY", aggregate: "sum" },
                        { field: "TOTAL_AMOUNT", aggregate: "sum" },

                    ]
                }],
                sort: {
                    field: "ITEM_EDESC",
                    dir: "asc",
                }
                //aggregate: [
                //    { field: "TOTAL_QUANTITY", aggregate: "sum" },
                //    { field: "TOTAL_AMOUNT", aggregate: "sum" },
                //],
            },

            toolbar: kendo.template($("#toolbar-template").html()),
            excel: {
                fileName: "Item Cumulative Report",
                allPages: true,
            },
            pdf: {
                fileName: "Item Cumulative Report",
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
            pageable: true,
            dataBound: function (e) {
                var grid = $("#grid").data("kendoGrid");

                if (dateFlag) {
                    //   grid.dataSource.options.fields[0].menu = true;
                    //grid.dataSource.options.fields[1].menu = true;
                    dateFlag = false;
                    var group = grid.dataSource.group();
                    if (group.length != "2") {
                        group.push({ field: "MITI", dir: "desc", aggregates: [{ field: "TOTAL_QUANTITY", aggregate: "sum" }, { field: "TOTAL_AMOUNT", aggregate: "sum" }] });
                        grid.dataSource.group(group);
                    }
                    else {
                        false;
                    }

                }

                UpdateReportUsingSetting("itemCumulativeReport", "grid");
                $('div').removeClass('.k-header k-grid-toolbar');

                var grid = e.sender;
                if (grid.dataSource.total() == 0) {
                    var colCount = grid.columns.length;
                    $(e.sender.wrapper)
                        .find('tbody')
                        .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                    displayPopupNotification("No Data Found Given Date Filter.", "info");
                }
                else {
                    //grid.dataSource.options.fields[0].menu = true;
                    //grid.dataSource.options.fields[1].menu = true;
                }

            },
            columnShow: function (e) {
                if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                    SaveReportSetting('itemCumulativeReport', 'grid');
            },
            columnHide: function (e) {
                if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                    SaveReportSetting('itemCumulativeReport', 'grid');
            },
            pageable: {
                refresh: true,
                pageSizes: reportConfig.itemPerPage,
                buttonCount: 5
            },
            columns:
            [
                //{
                //    field: "ORDER_NO",
                //    title: "Order No",
                //    hidden: true,
                //},
                {
                    field: "ORDER_DATE",
                    title: "Date",
                    template: "#= kendo.toString(kendo.parseDate(ORDER_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
                    groupHeaderTemplate: "#= kendo.toString(kendo.parseDate(value, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
                    hidden: true,
                    //menu:false,
                },
                {
                    field: "MITI",
                    title: "Miti",
                    hidden: true,
                    // menu: false,
                    template: "#=(MITI) #",
                    groupHeaderTemplate: "#=(value) #",

                },
                {
                    field: "EMPLOYEE_EDESC",
                    title: "Sales Person",
                    hidden: true,
                    template: "#=(EMPLOYEE_EDESC) #",
                    groupHeaderTemplate: "#=(value) #",
                },
                {
                    field: "ITEM_EDESC",
                    title: "Items",
                    width: "18%",
                    groupFooterTemplate: "Total"

                },
                {
                    field: "TOTAL_QUANTITY",
                    title: "Total Quantity",
                    aggregates: ["sum"],
                    attributes:
                    {
                        style: "text-align:right;"
                    },
                    // footerTemplate: '<span style="float:right">#= kendo.toString(sum)#</span>',
                    groupFooterTemplate: "#= kendo.toString(sum)#",
                    template: "#= (TOTAL_QUANTITY) #",
                    width: "8%"

                },
                {
                    field: "TOTAL_AMOUNT",
                    title: "Total Amount",
                    aggregates: ["sum"],
                    attributes:
                    {
                        style: "text-align:right;"
                    },
                    groupFooterTemplate: "#= kendo.toString(sum, 'n')#",
                    // footerTemplate: '<span style="float:right">#= kendo.toString(sum, "n")#</span>',
                    template: "#= kendo.toString(TOTAL_AMOUNT, 'n') #",
                    width: "8%",
                    format: "{0:n0}",
                },
            ]
        })

    }

    DateFilter.init(function () {
        consolidate.init(function () {
            ChangeDateFilter();
            bindGrid();
        });
    });

    $("#applydp").on("click", function (evt) {
        dateFlag = true;
        evt.preventDefault();
        var grid = $("#grid").data("kendoGrid");
        gridUrl = window.location.protocol + "//" + window.location.host + "/api/Report/GetItemCumulativeReport?dateFlag=" + dateFlag,
            kendoGridRefresh();
    });

    $("#applyConsolidate").on('click', function (evt) {
        evt.preventDefault();
        kendoGridRefresh();
    })

    $("#RunQuery").click(function (evt) {
        evt.preventDefault();
        ChangeDateFilter();
        kendoGridRefresh();
    });

    $("#loadAdvancedFilters").click(function (evt) {
        evt.preventDefault();
        reportConfig = GetReportSetting("itemCumulativeReport");
        $("#grid").data("kendoGrid").dataSource.read();
    });

    function kendoGridRefresh() {

        $("#grid").data().kendoGrid.destroy();
        $("#grid").empty();
        bindGrid();
    }

    function ChangeDateFilter() {
        $("#ddlDateFilterVoucher option[value='Today']").prop("selected", !0).change();
    }

  

});

