/// <reference path="~/Scripts/_references.js" />
/// <reference path="../module/planningModule.js" />

planningModule.controller('frequencySetupCtrl', function ($scope, $http, $filter, FrequencySetupService) {
    var vm = this;
    
    //vm.dtOptions = DTOptionsBuilder.newOptions().withPaginationType('simple').withDisplayLength(1);
    //$scope.dtOptions = { paging: false, searching: false };
    $scope.dtOptions = {
        paging:   true,
        ordering: true,
        searching:false,
    };
    $scope.pageName = "plan frequency setup";
    $scope.frequency = function () {
        FrequencySetupService.getFrequency().then(function (d) {
            $scope.frequencies = d.data;
        }, function () { alert("Error at: Get All Frequency") });
        function isEmpty(obj) {
            return Object.keys(obj).length === 0;
        }
        
    }
    $scope.frequency();


    $scope.addfreq = function (isValid) {

        if (isValid) {
            
            var frequencies = window.location.protocol + "//" + window.location.host + "/api/FrequencyApi/CreateFreq";

            var frequency = {
                TIME_FRAME_CODE: $scope.TIME_FRAME_CODE,
                TIME_FRAME_EDESC: $scope.TIME_FRAME_EDESC
            };
            var response = $http({
                method: "post",
                url: frequencies,
                data: frequency,
                dataType: "json"
            });

            return response.then(function (data) {
                if (data.data.MESSAGE == "Success") {
                    $scope.TIME_FRAME_CODE = 0;
                    $scope.TIME_FRAME_EDESC = '';
                    displayPopupNotification("Succesfully Saved Frequency", "success");
                    $scope.frequency();

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
    $scope.getfreqByCode = function (freq) {

        $scope.TIME_FRAME_CODE = freq.TIME_FRAME_CODE;
        $scope.TIME_FRAME_EDESC = freq.TIME_FRAME_EDESC;

    }
    $scope.deletefreqByCode = function (freq) {
        bootbox.confirm("Are you sure you want to delete?", function (result) {
            if (result) {
                FrequencySetupService.deleteByFrequency(freq).then(function (data) {
                    displayPopupNotification("Succesfully Deleted Frequency", "success");
                    $scope.frequency();

                }, function (e) {

                });
            }
        });

    }
    $scope.cancel = function () {
        $scope.TIME_FRAME_CODE = 0;
        $scope.TIME_FRAME_EDESC = "";
    }

});

planningModule.factory('FrequencySetupService', function ($http) {
    var fac = {};
    fac.getFrequency = function () {
        return $http.get('/api/FrequencyApi/GetAllFrequencies');
    }
    fac.deleteByFrequency = function (freqcode) {
        return $http({
            url: '/api/FrequencyApi/DeleteFrequency',
            method: "GET",
            params: { TIME_FRAME_CODE: freqcode }
        });
    }


    //getFrequency = function () {
    //    return $http.get('/api/FrequencyApi/GetAllFrequencies');
    //}

    return fac;

    //var getFrequency = function () {

    //    var freq = $http({
    //        method: "GET",
    //        url: /api/FrequencyApi / GetAllFrequencies
    //    });

    //};

});