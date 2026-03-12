distributionModule.controller("SchemeWiseReportCtrl", function ($scope, $http) {
    debugger;
    var previouselement;
    $scope.showRow = true;
    $scope.showDetail = true;
    $http({
            method: "get",
        url: window.location.protocol + "//" + window.location.host + "/api/Report/GetSchemeandDetails",
            dataType: "json"
        }).then(function (response) {
            debugger;
            $scope.allSchemes =  response.data;
        });


    $scope.showRule = function (schemeID, e) {
        debugger;
        if (previouselement != undefined) {
            document.getElementById(previouselement).classList.remove("active");

        }
        previouselement = "scheme" + schemeID;
        document.getElementById("scheme" + schemeID).classList.add("active");

        $scope.showRow = false;
       var rule = _.filter($scope.allSchemes, function (x) {

           return x.SchemeID == schemeID;
        });

        $scope.schemeRule = rule[0];
    }

    $scope.showDetails = function (id, min, max, frmDate, toDate) {
        debugger;
        //showloader();
        $scope.showDetail = false;
        $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Report/GetSchemeReport?Id=" + id + "&MinVal=" + min + "&MaxVal=" + max + "&fromDate=" + frmDate + "&toDate=" + toDate,
            dataType: "json"
        }).then(function (response) {
            debugger;
            $scope.details = response.data;
        });
    }
});