distributionModule.controller('outletSummaryCtrl', function ($scope, distributorService, $timeout) {

    var dateFilter = "";
    $scope.outletSummaryFilter = function () {
        var dateMonth = $("#ddlDatePickerFilter_outletSummary").val();
        var bsToDate = $("#datePickerBs_outletSummary").val();
        var bsFromDate = bsToDate.substr(0, 8) + "01";
        if (dateMonth == " loading ... ") {
            var Month = "This Month";
            var Year = bsToDate.substr(0, 4);
            var addedDate = (Month + " " + Year);
            $scope.outletSummaryMonths = addedDate;
        }

        else {
            var Month = dateMonth;
            var Year = bsToDate.substr(0, 4);
            var addedDate = (Month + " " + Year);
            $scope.outletSummaryMonths = addedDate;
        }
        var report = ReportFilter.filterAdditionalData();
        report.ReportFilters.ToDate = moment(BS2AD(bsToDate)).format("YYYY-MMM-DD");
        report.ReportFilters.FromDate = moment(BS2AD(bsFromDate)).format("YYYY-MMM-DD");
        var dateFilter = JSON.stringify($.extend({}, report));

        var Getdata = distributorService.GetOutletSummaryReport(dateFilter)
        Getdata.then(function (response) {
            $scope.outletSummaryData = response.data;
            //$timeout(function () {
            //    DataTableGrid();
            //}, 10)
        });
    }
    $scope.outletSummaryFilter();

    $("#applydp").click(function () {
        var dateMonth = $("#ddlDateFilterVoucher").val();
        var bsToDate = $("#ToDateVoucher").val();
        var bsFromDate = $("#FromDateVoucher").val();

        if (dateMonth == " loading ... ") {
            var Month = "This Month";
            var Year = bsToDate.substr(0, 4);
            var addedDate = (Month + " " + Year);
            $scope.outletSummaryMonths = addedDate;
        }

        else {
            var Month = dateMonth;
            var Year = bsToDate.substr(0, 4);
            var addedDate = (Month + " " + Year);
            $scope.outletSummaryMonths = addedDate;
        }
        var report = ReportFilter.filterAdditionalData();
        report.ReportFilters.ToDate = bsToDate;
        report.ReportFilters.FromDate = bsFromDate
        var dateFilter = JSON.stringify($.extend({}, report));

        var Getdata = distributorService.GetOutletSummaryReport(dateFilter)
        Getdata.then(function (response) {
            $scope.outletSummaryData = response.data;
            //$timeout(function () {
            //    DataTableGrid();
            //}, 10)
        });

    })
});
