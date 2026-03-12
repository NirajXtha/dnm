DTModule.controller('ConsumptionVoucherGenerateCtrl', function ($scope, $filter, $window, $timeout, $http, $q) {
    $scope.FormName = "Consumption Voucher Generate";
    $scope.FROM_DATE = "";
    $scope.TO_DATE = "";
    $scope.PRODUCT_FILTER = "";

    $scope.ConvertEngToNep = function () {
        console.log(this);

        var engdate = $("#englishDate5").val();
        var nepalidate = ConvertEngDateToNep(engdate);
        $("#nepaliDate5").val(nepalidate);
        $("#nepaliDate51").val(nepalidate);
    };

    $scope.ConvertNepToEng = function ($event) {

        //$event.stopPropagation();
        console.log($(this));
        var date = BS2AD($("#nepaliDate5").val());
        var date1 = BS2AD($("#nepaliDate51").val());
        $("#englishdatedocument").val($filter('date')(date, "dd-MMM-yyyy"));
        $("#englishdatedocument1").val($filter('date')(date1, "dd-MMM-yyyy"));
        $('#nepaliDate5').trigger('change')
        $('#nepaliDate51').trigger('change')
    };

    $scope.ConvertEngToNepang = function (data) {
        $("#nepaliDate5").val(AD2BS(data));
    };
    $scope.ConvertEngToNepang1 = function (data) {

        $("#nepaliDate51").val(AD2BS(data));
    }
    $scope.ConvertGateEngToNepang = function (data) {
        $("#gateNepaliDate").val(AD2BS(data))
    }
    $scope.ConvertBillEngToNepang = function (data) {
        $("#billNepaliDate").val(AD2BS(data))
    }

    $scope.someDateFn = function () {

        var engdate = $filter('date')(new Date(new Date().setDate(new Date().getDate() - 1)), 'dd-MMM-yyyy');
        //var engdate1 = $filter('date')(new Date(new Date().setDate(new Date().getDate() - 2)), 'dd-MMM-yyyy');
        var a = ConvertEngDateToNep(engdate);
        //var a1 = ConvertEngDateToNep(engdate1);
        $scope.Dispatch_From = engdate;
        $scope.NepaliDate = a;
        $scope.Dispatch_To = a;
        $scope.PlanningTo = ConvertEngDateToNep($filter('date')(new Date(new Date().setDate(new Date().getDate())), 'dd-MMM-yyyy'));
        //  $scope.PlanningDate = a;

    };

    function getCurrentTimeAsDate() {
        var now = new Date();
        return new Date(1970, 0, 1, now.getHours(), now.getMinutes());
    }


    $scope.someDateFn();
    $scope.monthSelectorOptionsSingle = {
        value: new Date(),
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {

            $scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",


        dateInput: true
    };

    $scope.monthSelectorOptionsSingle1 = {
        open: function () {

            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {

            $scope.ConvertEngToNepang1(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",


        dateInput: true
    };

    $scope.monthGateSelectorOptionsSingle = {
        open: function () {

            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {

            $scope.ConvertGateEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",


        dateInput: true
    };

    $scope.monthBillSelectorOptionsSingle = {
        open: function () {

            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {

            $scope.ConvertBillEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",


        dateInput: true
    };


    $scope.BindConsumptionVoucherGenerateGrid = function () {
        debugger
        var grid = $("#CategoryBaseGrid").data("kendoGrid");

        if (grid) {
            grid.dataSource.read();
        }
    };

    $timeout(function () {
        debugger
        $scope.BindConsumptionVoucherGenerateGrid();
    })

    $scope.CategoryBaseGridOptions = {
        dataSource: new kendo.data.DataSource({
            transport: {
                read: {
                    url: "/api/SetupApi/GetConsumptionVoucherCategoryBaseData",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json"
                },
                parameterMap: function (options, type) {
                    debugger
                    if (type === "read") {
                        let fromDate = $(".fromDate input").val();
                        let toDate = $(".toDate input").val();
                        let productFilter = $(".productFilter").val();
                        var combinedOptions = angular.extend({}, options, {
                            FROM_DATE: moment(fromDate).toISOString(),
                            TO_DATE: moment(toDate).toISOString(),
                            PRODUCT_FILTER: productFilter
                        });
                        return JSON.stringify(combinedOptions);
                    }
                    return options;
                }
            },
            autoBind: false,
            schema: {
                data: "DATA",
                model: {
                    fields: {
                        ISSUE_TYPE_EDESC: { type: "string" },
                        ISSUE_TYPE_CODE: { type: "string" },
                        CATEGORY_EDESC: { type: "string" }, 
                        CATEGORY_CODE: { type: "string" }, 
                        QUANTITY: { type: "number" },
                        TOTAL_VALUE: { type: "number" }
                    }
                }
            },
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            pageSize: 10,
            sort: {
                field: "CATEGORY_EDESC",
                dir: "asc"
            },
            group: {
                field: "CATEGORY_CODE",
                dir: "asc",
                aggregates: [
                    { field: "QUANTITY", aggregate: "sum" },
                    { field: "TOTAL_VALUE", aggregate: "sum" }
                ]
            },
            aggregate: [
                { field: "QUANTITY", aggregate: "sum" },
                { field: "TOTAL_VALUE", aggregate: "sum" }
            ]
        }),
        autoBind: false,
        selectable: "single",
        scrollable: true,
        height: 350,
        sortable: true,
        pageable: true,
        groupable: true,
        resizable: true,
        columns: [
            {
                field: "CATEGORY_CODE",
                title: "Category",
                hidden: true,
                groupHeaderTemplate: function (data) {
                    debugger
                    return "Category: " + data.items[0].CATEGORY_EDESC + " (" + data.value + ")";
                },
                footerTemplate: "<strong>Grand Total:</strong>" 
            },
            {
                field: "ISSUE_TYPE_EDESC",
                title: "Issue Type",
                width: "150px",
                footerTemplate: "Grand Total:"
            },
            {
                field: "QUANTITY",
                title: "Quantity",
                width: "100px",
                format: "{0:n2}",
                footerTemplate: "#= kendo.format('{0:n2}', sum) #" 
            },
            {
                field: "TOTAL_VALUE",
                title: "Amount",
                width: "150px",
                format: "{0:n2}",
                footerTemplate: "#= kendo.format('{0:n2}', sum) #" 
            }
        ]
    };

    $scope.initializeDates = function () {
        let today = moment();
        endDate = today.toDate();

        $scope.ConsumptionVoucherGenerate.GATE_DATE = moment(endDate).format("DD-MMM-YYYY");
        $scope.ConsumptionVoucherGenerate.BILL_DATE = moment(endDate).format("DD-MMM-YYYY");
        $("#gateEnglishDate").data("kendoDatePicker").value(moment(endDate).format("DD-MMM-YYYY"));
        $("#billEnglishDate").data("kendoDatePicker").value(moment(endDate).format("DD-MMM-YYYY"));

        let nepaliToDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));

        $("#gateNepaliDate").val(nepaliToDate);
        $("#billNepaliDate").val(nepaliToDate);
    }

});
