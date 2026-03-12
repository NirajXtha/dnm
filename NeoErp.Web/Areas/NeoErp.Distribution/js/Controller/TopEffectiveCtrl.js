distributionModule.controller('topEffectiveCtrl', function ($scope, distributorService, $timeout) {

    var dateFilter = "";
    $scope.topeffectiveFilters = function () {
        var dateMonth = $("#ddlDatePickerFilter_topEffectivePageId").val();
        var bsToDate = $("#datePickerBs_topEffectivePageId").val();
        var bsFromDate = bsToDate.substr(0, 8) + "01";

        if (dateMonth == " loading ... ") {
            var Month = "This Month";
            var Year = bsToDate.substr(0, 4);
            var addedDate = (Month + " " + Year);
            $scope.topEffectiveMonths = addedDate;
        }

        else {
            var Month = dateMonth;
            var Year = bsToDate.substr(0, 4);
            var addedDate = (Month + " " + Year);
            $scope.topEffectiveMonths = addedDate;
        }
        var report = ReportFilter.filterAdditionalData();
        report.ReportFilters.ToDate = moment(BS2AD(bsToDate)).format("YYYY-MMM-DD");
        report.ReportFilters.FromDate = moment(BS2AD(bsFromDate)).format("YYYY-MMM-DD");
        var dateFilter = JSON.stringify($.extend({}, report));

        var Getdata = distributorService.getTopEffectiveCalls(dateFilter)
        Getdata.then(function (response) {
            $scope.topEffectiveCalls = response.data;
            //$timeout(function () {
            //    DataTableGrid();
            //}, 10)
        });
    }
    $scope.topeffectiveFilters();

    $("#applydp").click(function () {
        var dateMonth = $("#ddlDateFilterVoucher").val();
        var bsToDate = $("#ToDateVoucher").val();
        var bsFromDate = $("#FromDateVoucher").val();

        if (dateMonth == " loading ... ") {
            var Month = "This Month";
            var Year = bsToDate.substr(0, 4);
            var addedDate = (Month + " " + Year);
            $scope.topEffectiveMonths = addedDate;
        }

        else {
            var Month = dateMonth;
            var Year = bsToDate.substr(0, 4);
            var addedDate = (Month + " " + Year);
            $scope.topEffectiveMonths = addedDate;
        }
        var report = ReportFilter.filterAdditionalData();
        report.ReportFilters.ToDate = bsToDate;
        report.ReportFilters.FromDate = bsFromDate
        var dateFilter = JSON.stringify($.extend({}, report));

        var Getdata = distributorService.getTopEffectiveCalls(dateFilter)
        Getdata.then(function (response) {
            $scope.topEffectiveCalls = response.data;
            //$timeout(function () {
            //    DataTableGrid();
            //}, 10)
        });

    })
});