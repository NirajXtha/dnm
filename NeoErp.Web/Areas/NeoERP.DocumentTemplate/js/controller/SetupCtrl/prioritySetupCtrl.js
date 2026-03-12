DTModule.controller('prioritySetupCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    $scope.saveAction = "Save";
    $scope.FormName = "Priority Setup";
    $scope.btnDelete = false;
    $scope.PrioritySetup = {
        PRIORITY_CODE: "",
        PRIORITY_EDESC: "",
        PRIORITY_NDESC: "",
        REMARKS: "",

    };

    var getPriority = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getPriorityList";
    $scope.grid = {
        change: PriorityChange,
        dataSource: {
            transport: {
                read: getPriority,
            },
            pageSize: 20
        },
        dataBound: function (e) {

        },
        height: window.innerHeight - 50,
        groupable: false,
        sortable: false,
        height: 470,
        selectable: true,
        scrollable: {
            virtual: true
        },
        columns: [
        {
            field: "PRIORITY_EDESC",
            title: "Priority Name",
            width: 150
        }]

    }

    function PriorityChange(evt) {

        selectedRow = this.select();
        var item = this.dataItem(selectedRow);

        if (item) {
            $scope.saveAction = "Update";
            $scope.TransporterHeader = "Update Vehicle";

            $scope.PrioritySetup.PRIORITY_CODE = item.PRIORITY_CODE;
            $scope.PrioritySetup.PRIORITY_EDESC = item.PRIORITY_EDESC;
            $scope.PrioritySetup.PRIORITY_NDESC = item.PRIORITY_NDESC;
            $scope.PrioritySetup.REMARKS = item.REMARKS;
            $scope.btnDelete = true;
            $scope.$apply();
        }
    }

    $scope.SavePriority = function (isValid) {

        var model = {
            PRIORITY_CODE: $scope.PrioritySetup.PRIORITY_CODE,
            PRIORITY_EDESC: $scope.PrioritySetup.PRIORITY_EDESC,
            PRIORITY_NDESC: $scope.PrioritySetup.PRIORITY_NDESC,
            REMARKS: $scope.PrioritySetup.REMARKS,
        }

        if (!isValid) {
            displayPopupNotification("Please Fill the Required Fields", "warning");
            return;
        }

       
        if ($scope.saveAction == "Save") {
            var saveVehicleUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createPrioritySetup";
            $http({
                method: 'POST',
                url: saveVehicleUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "INSERTED") {
                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully saved ", "success");
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Priority name already exist please try another Priority name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            }, function errorCallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Priority name already exist please try another Priority name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            });
        }
        else if ($scope.saveAction == "Update") {
            var udpateVehicleUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateNewPrioritySetup";
            var model1 = {
                PRIORITY_CODE: $scope.PrioritySetup.PRIORITY_CODE,
                PRIORITY_EDESC: $scope.PrioritySetup.PRIORITY_EDESC,
                PRIORITY_NDESC: $scope.PrioritySetup.PRIORITY_NDESC,
                REMARKS: $scope.PrioritySetup.REMARKS,
            }
            $http({
                method: 'POST',
                url: udpateVehicleUrl,
                data: model1
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "UPDATED") {
                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully updated ", "success");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            }, function errorCallback(response) {
                displayPopupNotification("Something went wrong.Please try again later.", "error");

            });
        }
    }

    $scope.deletePriority = function () {
        bootbox.confirm({
            title: "Delete",
            message: "Are you sure?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success',
                    label: '<i class="fa fa-check"></i> Yes',
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger',
                    label: '<i class="fa fa-times"></i> No',
                }
            },
            callback: function (result) {

                if (result == true) {
                    var PRIORITY_EDESC = $("#vehiclename").val();
                    var PRIORITY_CODE = $("#vehicleCode").val();
                    var PRIORITYCODE;
                    if (PRIORITYCODE == undefined) {
                        PRIORITYCODE = $scope.PrioritySetup.PRIORITY_CODE;
                    }
                    var deleteTransporterUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/deletePrioritySetup?priorityCode=" + PRIORITYCODE;
                    $http({
                        method: 'POST',
                        url: deleteTransporterUrl,
                    }).then(function successCallback(response) {
                        if (response.data.MESSAGE == "DELETED") {
                            $("#grid").data("kendoGrid").dataSource.read();
                            $scope.Cancel();
                            displayPopupNotification("Data succesfully deleted ", "success");
                        }
                        else {
                            displayPopupNotification(response.data.STATUS_CODE, "error");
                        }
                        
                    }, function errorCallback(response) {
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                       
                    });
                }
                $("#grid").data("kendoGrid").dataSource.read();
                $scope.Cancel();
            }

        });


    }

    $scope.Cancel = function () {
        $scope.saveAction = "Save";
        $scope.TransporterHeader = "Add Priority";
        $scope.PrioritySetup = {
            PRIORITY_CODE: "",
            PRIORITY_EDESC: "",
            PRIORITY_NDESC: "",
            REMARKS: "",
        };

        $scope.btnDelete = false;
        $("#grid").data("kendoGrid").clearSelection();

    }

    $scope.getMaxPriorityCode = function () {
        var getMaxPriorityCode = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetMaxPriorityCode";
        $http({
            method: 'GET',
            url: getMaxPriorityCode,
        }).then(function successCallback(response) {
            $scope.PrioritySetup.PRIORITY_CODE = response.data;
        })
    };

    //$scope.getMaxPriorityCode();

});

