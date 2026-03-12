distributionModule.controller('VisitImageGalleryCtrl', function ($scope, DistSetupService, $routeParams) {

    
    
    $scope.currentView = "Visited";       
    $scope.dateFilter1 = null;            
    $scope.filterData = {};                
    $scope.imageLists = [];
    $scope.notVisitedList = [];
    $scope.imageListsFlat = [];           
    $scope.url = window.location.protocol + "//" + window.location.host + "/Areas/NeoErp.Distribution/Images/EntityImages/";

    $scope.applyAdvanceFilter = function () {
        try {
            debugger;
            var salesPersonArr = $("#SalesMultiSelect").val();
            if (!salesPersonArr || salesPersonArr.length === 0) {
                displayPopupNotification("Please select Sales Person", "warning");
                return;
            }
            var salesPerson = salesPersonArr;


            var dateMonth = $("#ddlDateFilterVoucher").val();
            var bsToDate = $("#ToDateVoucher").val();
            var bsFromDate = $("#FromDateVoucher").val();

            var Month;
            if (dateMonth === " loading ... ") {
                Month = "This Month";
            } else if (dateMonth === "Custom") {
                Month = bsFromDate + " To " + bsToDate;
            } else {
                Month = dateMonth;
            }
            $scope.SelectedMonths = Month;


            var distributorArr = $("#DistributorMultiSelect").val();
            var ResellerArr = $("#ResellerMultiSelect").val();


            var distributor = (Array.isArray(distributorArr) && distributorArr.length > 0) ? distributorArr[0] : null;
            var Reseller = (Array.isArray(ResellerArr) && ResellerArr.length > 0) ? ResellerArr[0] : null;

            console.log("Distributor:", distributor, "Reseller:", Reseller);


            var report1 = $.extend(true, {}, ReportFilter.filterAdditionalData() || {});
            report1.ReportFilters.ToDate = moment(bsToDate).format("DD-MMM-YYYY");
            report1.ReportFilters.FromDate = moment(bsFromDate).format("DD-MMM-YYYY");
            report1.ReportFilters.SalesPersonFilter = salesPerson;
            report1.ReportFilters.DistributorFilter = distributor ? [distributor] : [];
            report1.ReportFilters.ResellerFilter = Reseller ? [Reseller] : [];


            $scope.dateFilter1 = JSON.stringify(report1);
            $scope.filterData = { Distributor: distributor, Reseller: Reseller };

            console.log("Filter JSON:", $scope.dateFilter1);

            displayPopupNotification("Filter applied successfully", "success");

            $('#exampleModal').modal('hide');


            if ($scope.currentView === "Visited") $scope.loadVisited();
            else $scope.loadNotVisited();

        } catch (err) {
            console.error("Apply Filter error:", err);
            displayPopupNotification("Error applying filter", "error");
        }
    };

    $scope.loadVisited = function () {
        if (!$scope.dateFilter1) {
            displayPopupNotification("Please apply filter first", "warning");
            $('#exampleModal').modal('show');
            return;
        }

        $scope.currentView = "Visited";

        var parsedFilter = JSON.parse($scope.dateFilter1);
        var distributor = $scope.filterData.Distributor || "";
        var reseller = $scope.filterData.Reseller || "";

        console.log("Calling Visited API:", parsedFilter, distributor, reseller);

        DistSetupService.GetVisiterList(parsedFilter, { distributor: distributor, Reseller: reseller })
            .then(function (response) {
                processResponse(response.data);
            }).catch(function (error) {
                console.error("Visited API error:", error);
                displayPopupNotification("Failed to load Visited data", "error");
            });
    };


    $scope.loadNotVisited = function () {
        if (!$scope.dateFilter1) {
            displayPopupNotification("Please apply filter first", "warning");
            $('#exampleModal').modal('show');
            return;
        }

        $scope.currentView = "NotVisited";

        var parsedFilter = JSON.parse($scope.dateFilter1);
        var distributor = $scope.filterData.Distributor || "";
        var reseller = $scope.filterData.Reseller || "";

        DistSetupService.GetNotVisitedList(parsedFilter, distributor, reseller)
            .then(function (response) {

                $scope.notVisitedList = response.data || [];

                console.log("NotVisited List:", $scope.notVisitedList);

                if (!$scope.$$phase) $scope.$apply();
            })
            .catch(function (error) {
                console.error("NotVisited API error:", error);
                displayPopupNotification("Failed to load Not Visited data", "error");
            });
    };

    $scope.exportNotVisitedToExcel = function () {
        if (!$scope.notVisitedList || $scope.notVisitedList.length === 0) {
            alert("No data to export!");
            return;
        }

        var ws_data = $scope.notVisitedList.map(function (customer) {
            return {
                "Customer Code": customer.CUSTOMER_CODE,
                "Customer Name": customer.CUSTOMER_NAME,
                "Customer Type": customer.CUSTOMER_TYPE === 'D' ? 'Distributor' :
                    customer.CUSTOMER_TYPE === 'R' ? 'Reseller' :
                        customer.CUSTOMER_TYPE
            };
        });

        var ws = XLSX.utils.json_to_sheet(ws_data);

        var wb = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(wb, ws, "NotVisited");

        XLSX.writeFile(wb, "NotVisited_Customers.xlsx");
    };





    function processResponse(data) {
        for (var i = 0; i < data.length; i++) {
            data[i].Date = moment(data[i].UPLOAD_DATE).format('DD-MMM-YYYY');
            data[i].Time = moment(data[i].UPLOAD_DATE).format('hh:mm:ss A');
        }

        $scope.imageLists = _.groupBy(data, 'ENTITY_NAME');
        $scope.imageListsFlat = _.flatten(_.values($scope.imageLists));
    }

    
    $(document).ready(function () {
        DateFilter.init();
        $('#exampleModal').modal('show'); 
    });

});
