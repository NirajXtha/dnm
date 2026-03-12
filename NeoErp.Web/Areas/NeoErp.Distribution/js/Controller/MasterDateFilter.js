distributionModule.controller('masterDateFilter', function ($scope, distributorService, $timeout) {

    var dateFilter = "";
    //$("#applydp").click(function () {
    //    var dateMonth = $("#ddlDateFilterVoucher").val();
    //    var bsToDate = $("#ToDateVoucher").val();
    //    var bsFromDate = $("#FromDateVoucher").val();
        
    //    if (dateMonth == " loading ... ") {
    //        var Month = "This Month";
    //        var Year = bsToDate.substr(0, 4);
    //        var addedDate = (Month + " " + Year);
    //        $scope.outLetSelectedMonths = addedDate;
    //    }

    //    else {
    //        var Month = dateMonth;
    //        var Year = bsToDate.substr(0, 4);
    //        var addedDate = (Month + " " + Year);
    //        $scope.outLetSelectedMonths = addedDate;
    //    }
    //    var report = ReportFilter.filterAdditionalData();
    //    report.ReportFilters.ToDate = bsToDate;
    //    report.ReportFilters.FromDate = bsFromDate
    //    var dateFilter = JSON.stringify($.extend({}, report));

    //    var Getdata = distributorService.getSumOutletReport(dateFilter)
    //    Getdata.then(function (response) {
    //        $scope.sumOutletData = response.data;
    //        //$timeout(function () {
    //        //    DataTableGrid();
    //        //}, 10)
    //    });

    //})



});