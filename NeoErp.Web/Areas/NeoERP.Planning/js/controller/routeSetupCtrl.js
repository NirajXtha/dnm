/// <reference path="~/Scripts/_references.js" />
/// <reference path="../module/planningModule.js" />
planningModule.controller('routeSetupCtrl', function ($scope, $http, $filter, $q, setupservice, DTOptionsBuilder) {
    var $this = this;
    
    $this.dtOptions = DTOptionsBuilder.newOptions().withPaginationType('simple').withDisplayLength(1);
    $scope.pageName = "Route Area setup";
    $scope.route = function () {
        setupservice.getRoute().then(function (d) {
            $scope.routes = d.data;
        }
        , function () {
            alert("Error at: Get All Route")
        }
        );
        function isEmpty(obj) {
            return Object.keys(obj).length === 0;
        }
    }
    $scope.route();

    $scope.addRoute = function (isValid) {
        
        if (isValid) {
            var url = window.location.protocol + "//" + window.location.host + "/api/DistributionPlaningApi/CreateRoute";

            var route = {
                AREA_CODE: $scope.AREA_CODE,
                AREA_NAME: $scope.AREA_NAME
            };
            var response = $http({
                method: "post",
                url: url,
                data: route,
                dataType: "json"
            });

            return response.then(function (data) {
                if (data.data.MESSAGE == "Success") {
                    $scope.AREA_CODE = 0;
                    $scope.AREA_NAME = '';
                    displayPopupNotification("Succesfully Saved Area", "success");
                    $scope.route();

                }
                else if (data.data.MESSAGE == "Alreadyexists") {
                    displayPopupNotification("The data already exits", "error");

                }
                else if (data.data.MESSAGE == "ExistsButDeleted") {
                    displayPopupNotification("Cannot update to the existing name! Use another name!", "error");

                }
                else {
                    displayPopupNotification("Error Occured.", "error");

                }

            });

        }
        else {
            toastr.error("Fields cannot be empty.");
        }

    };
    $scope.getRouteByCode = function (route) {

        $scope.AREA_CODE = route.AREA_CODE;
        $scope.AREA_NAME = route.AREA_NAME;

    }
    $scope.deleteRouteByCode = function (route) {
        bootbox.confirm("Are you sure you want to delete?", function (result) {
            if (result) {
                setupservice.deleteByRoute(route).then(function (data) {
                    displayPopupNotification("Succesfully Deleted Area", "success");
                    $scope.route();

                }, function (e) {

                });
            }
        });

    }
    $scope.cancel = function () {
        $scope.AREA_CODE = 0;
        $scope.AREA_NAME = "";
    }

});

planningModule.service('setupservice', function ($http, $q) {
    var fac = {};
    fac.getRoute = function () {
        return $http.get('/api/DistributionPlaningApi/GetAllRoutes');
    }
    fac.deleteByRoute = function (code) {
        return $http({
            url: '/api/DistributionPlaningApi/DeleteRoute',
            method: "GET",
            params: { ROUTE_CODE: code }
        });
    }
    return fac;

});