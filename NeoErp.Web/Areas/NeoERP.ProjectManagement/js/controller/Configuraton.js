///*DTModule.controller('Configuraton', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
(function ($) {
    'use strict';
    var configuration = angular.module("configuration", [])
    configuration.controller('Configuraton', ['$scope', '$http',' $window', '$filter', ', $routeParams,', '$timeout', function ($scope, $http, $window, $filter, $routeParams, $timeout) {

   $scope.saveAction = "Save";
    $scope.btnDelete = false;
    $scope.Configuration = {
        UserId: '',
        ShowAdvanceSearch: '',
        ShowAdvanceAutoComplete: '',
    };
    $scope.ConfigurationHeader = "Add Configuration";


    $scope.SaveConfiguration = function (isValid) {
        var model = {
            Userid: $scope.Configuration.UserId,
            ShowAdvanceSearch: $scope.Configuration.ShowAdvanceSearch,
            ShowAdvanceAutoComplete: $scope.Configuration.ShowAdvanceAutoComplete
        }
        //if (!isValid) {
        //    displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
        //    return;
        //}
        if ($scope.saveAction == "Save") {
            var saveAreaUrl = window.location.protocol + "//" + window.location.host + "/api/EntryApi/SaveWebPrefrence";
            $http({
                method: 'POST',
                url: saveAreaUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.Success == "true") {

                    displayPopupNotification("Data succesfully saved ", "success");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // this callback will be called asynchronously
                // when the response is available
            }, function errorCallback(response) {
                displayPopupNotification("Something went wrong.Please try again later.", "error");
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
        }
        else {
            var udpateAreaUrl = window.location.protocol + "//" + window.location.host + "/api/EntryApi/updateAreaSetup";
            $http({
                method: 'POST',
                url: udpateAreaUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "UPDATED") {
                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully updated ", "success");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // this callback will be called asynchronously
                // when the response is available
            }, function errorCallback(response) {
                displayPopupNotification("Something went wrong.Please try again later.", "error");
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
        }

        $("#grid").data("kendoGrid").dataSource.read();

    }
    }]);
})(jQuery);