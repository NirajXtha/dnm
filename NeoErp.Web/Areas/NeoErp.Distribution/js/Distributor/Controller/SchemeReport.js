distributionModule.controller("SchemeReport", function ($scope, $filter, $timeout, crudAJService) {

    var getgiftItems = function (id) {
        debugger;
        var response = $http({
            method: "get",
            url: window.location.protocol + "//" + window.location.host + "/api/Distributor/GetSchemeData",
            dataType: "json"
        }).then(function (response) {
            debugger;
            $scope.giftItems = response.data.DATA;
        });
        debugger;
        return response;
    }

});