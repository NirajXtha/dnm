distributionModule.controller("BranchWiseSalesvsTarget", function ($scope, $filter, $timeout, crudAJService) {

    getDateFormat = function (date) {
        var test = new Date(date);
        if (test.getFullYear() == '1970')
            return '-';
        return kendo.format("{0:" + reportConfig.dateFormat + "}", new Date(date));
    }
   // var gridUrl = window.location.protocol + "//" + window.location.host + "/api/Distributor/GetAllSalesVsTarget";
    var gridUrl = window.location.protocol + "//" + window.location.host + "/api/Distributor/GetAllSalesVsTarget";




    function dataBind() {
        reportConfig = GetReportSetting("BranchWiseSalesVsTarget");
        $("#grid").kendoGrid({
            dataSource: {
                transport: {
                    read: {
                        url: gridUrl,
                        dataType: "json", // <-- The default was "jsonp".
                        //type: "POST",
                        contentType: "application/json; charset=utf-8"
                    },
                    parameterMap: function (options, type) {
                        debugger;
                        var paramMap = JSON.stringify($.extend(options, ReportFilter.filterAdditionalData()));
                        delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                        delete paramMap.$format; // <-- remove format parameter.
                        return paramMap;
                    },
                },
                schema: {
                    model: {
                        fields: {
                            TARGET_QUANTITY: { type: "number" },
                            TARGET_VALUE: { type: "number" },
                            QUANTITY_ACHIVE: { type: "number" },
                            ACHIVE_VALUE: { type: "number" },
                        }
                    }
                },
                pageSize: 20,
                group: {
                    field: "NEPALI_MONTHINT", aggregates: [
                        { field: "TARGET_QUANTITY", aggregate: "sum" },
                        { field: "TARGET_VALUE", aggregate: "sum" },
                        { field: "QUANTITY_ACHIVE", aggregate: "sum" },
                        { field: "ACHIVE_VALUE", aggregate: "sum" }
                    ]
                },

                aggregate: [{ field: "TARGET_QUANTITY", aggregate: "sum" },
                    { field: "TARGET_VALUE", aggregate: "sum" },
                    { field: "QUANTITY_ACHIVE", aggregate: "sum" },
                    { field: "ACHIVE_VALUE", aggregate: "sum" }],
                pageSize: reportConfig.defaultPageSize,
               
            },
            toolbar: kendo.template($("#toolbar-template").html()),
            excel: {
                fileName: "Branch Wise sales vs target",
                allPages: true,
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



                UpdateReportUsingSetting("SchemeReport", "grid");
                $('div').removeClass('.k-header k-grid-toolbar');
            },
            columnShow: function (e) {
                if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                    SaveReportSetting('SchemeReport', 'grid');
            },
            columnHide: function (e) {
                if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                    SaveReportSetting('SchemeReport', 'grid');
            },
            pageable: {
                refresh: true,
                pageSizes: reportConfig.itemPerPage,
                buttonCount: 5
            },
            columns: [

                {
                    field: "ITEM_EDESC",
                    title: "Item Name"
                },
                {
                    field: "BRAND_NAME",
                    title: "Brand Name",
                },
                {
                    field: "NEPALI_MONTHINT",
                    title: "Month Name",
                    footerTemplate: "Total: ",
                    groupFooterTemplate: "Total: ",
                },
                {
                    field: "TARGET_QUANTITY",
                    title: "Target Quantity",
                    aggregates: ["sum"],
                    footerTemplate: "#=sum.toFixed(2)#",
                    groupFooterTemplate: "#=sum.toFixed(2)#"
                },
                {
                    field: "TARGET_VALUE",
                    title: "Target Amount",
                    aggregates: ["sum"],
                    footerTemplate: "#=sum.toFixed(2)#",
                    groupFooterTemplate: "#=sum.toFixed(2)#"
                },
                {
                    field: "QUANTITY_ACHIVE",
                    title: "Purchase Quantity",
                    aggregates: ["sum"],
                    footerTemplate: "#=sum.toFixed(2)#",
                    groupFooterTemplate: "#=sum.toFixed(2)#",
                    width: "18%"
                },
                {
                    field: "ACHIVE_VALUE",
                    title: "Purchase Amount",
                    footerTemplate: "#=sum.toFixed(2)#",
                    groupFooterTemplate: "#=sum.toFixed(2)#",
                    aggregates: ["sum"],
                },
                {
                    title: "Achievement %",
                    attributes: {
                        style: "text-align: right;"
                    },
                    template: '#= TARGET_QUANTITY == 0 ? 0: ((QUANTITY_ACHIVE/TARGET_QUANTITY) * 100).toFixed(2) #%',
                    footerTemplate: function (e) {
                        per = e.TARGET_QUANTITY.sum == 0 ? 0 : ((e.QUANTITY_ACHIVE.sum / e.TARGET_QUANTITY.sum) * 100).toFixed(2);
                        return per + '%';
                    },
                    groupFooterTemplate: function (e) {
                        per = e.TARGET_QUANTITY.sum == 0 ? 0 : ((e.QUANTITY_ACHIVE.sum / e.TARGET_QUANTITY.sum) * 100).toFixed(2);
                        return per + '%';
                    },
                },
                {
                    field: "NEPALI_MONTH",
                    title: "Month",
                    hidden: true,
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
    dataBind();
   

    $(".applydp").on("click", function (evt) {
        evt.preventDefault();
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