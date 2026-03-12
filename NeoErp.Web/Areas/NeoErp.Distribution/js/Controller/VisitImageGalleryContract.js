distributionModule.controller('VisitImageGalleryContract', function ($scope, DistSetupService, $routeParams) {
    
    $scope.applyAdvanceFilter = function () {
        var salesPerson = $("#SalesMultiSelect").val();
        if (!salesPerson) {
            displayPopupNotification("Please select Sales Person", "warning");
            return;
        }
        var dateMonth = $("#ddlDateFilterVoucher").val();
        var bsToDate = $("#ToDateVoucher").val();
        var bsFromDate = $("#FromDateVoucher").val();
        if (" loading ... " == dateMonth)
        { var Month = "This Month"; $scope.SelectedMonths = Month }
        else
        {
            var Month = dateMonth; "Custom" == Month ? $scope.SelectedMonths = bsFromDate + " To " + bsToDate : $scope.SelectedMonths = Month
        }
        var Reseller=  $("#ResellerMultiSelect").val() == null ? "" : $("#ResellerMultiSelect").val()[0];
        var distributor=  $("#DistributorMultiSelect").val() == null ? "" : $("#DistributorMultiSelect").val()[0]
        var data = {
            Reseller: Reseller,
            distributor: distributor,
        }
        var report = $.extend({}, true, ReportFilter.filterAdditionalData());
        report.ReportFilters.ToDate = bsToDate;
        report.ReportFilters.FromDate = bsFromDate;
        var dateFilter = JSON.stringify(report);
        var report1 = $.extend({}, true, ReportFilter.filterAdditionalData());
        report1.ReportFilters.ToDate = moment(bsToDate).format("DD-MMM-YYYY");
        report1.ReportFilters.FromDate = moment(bsFromDate).format("DD-MMM-YYYY");
        var dateFilter1 = JSON.stringify(report1);

        var getdata = DistSetupService.GetVisiterList(dateFilter1,data);
        getdata.then(function (response) {
            for (var i = 0; i < response.data.length; i++) {
                response.data[i].Date = moment(response.data[i].UPLOAD_DATE).format('DD-MMM-YYYY');
                response.data[i].Time = moment(response.data[i].UPLOAD_DATE).format('hh:mm:ss A');
            }
            $scope.url = window.location.protocol + "//" + window.location.host + "/Areas/NeoErp.Distribution/Images/EntityImages/";
            $scope.imageLists = _.groupBy(response.data, function (x) { return x.ENTITY_NAME })

            $('#exampleModal').modal('hide');
        })
    }

    $(document).ready(function () {
        DateFilter.init();
        $('#exampleModal').modal('show');
    })
});