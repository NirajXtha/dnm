distributionModule.controller("SchemeReportCtr", function ($scope, BrandingService, $routeParams) {
    
    getDateFormat = function (date) {
        var test = new Date(date);
        if (test.getFullYear() == '1970')
            return '-';
        return kendo.format("{0:" + reportConfig.dateFormat + "}", new Date(date));
    }
    var gridUrl = window.location.protocol + "//" + window.location.host + "/api/Branding/GetSchemeList";
    




    function dataBind() {
        reportConfig = GetReportSetting("SchemeReport");
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
                group: { field: "ContractName" },
                pageSize: reportConfig.defaultPageSize,
                model: {
                    fields: {
                        HandOverDate: { type: "date" },

                    }
                },
            },
            toolbar: kendo.template($("#toolbar-template").html()),
            excel: {
                fileName: "Scheme report",
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
                    field: "ContractName",
                    title: "Contract Name",
                    hidden: true,
                },
                {
                    field: "EmployeeName",
                    title: "Employee Name",
                },
            
             
                {
                    field: "ItemName",
                    title: "Item",
                },
                {
                    field: "ResellerName",
                    title: "Reseller",
                },
                {
                    field: "Quantity",
                    title: "Quantity",
                    width: "18%"
                },
                {
                    field: "MuName",
                    title: "Mu Name",
                },
                {
                    field: "HandOverDate",
                    title: "Hand over Date",
                    template: "#= kendo.toString(kendo.parseDate(HandOverDate, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
                  
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


    $("#RunQuery").click(function (evt) {
        evt.preventDefault();
        RefreshGrid();
    });

    $("#loadAdvancedFilters").click(function (evt) {
        evt.preventDefault();
        RefreshGrid();
    });



});

