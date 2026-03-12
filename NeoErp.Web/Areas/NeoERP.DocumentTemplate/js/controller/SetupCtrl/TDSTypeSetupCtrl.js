DTModule.controller('TDSTypeSetupCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    $scope.saveAction = "Save";
    $scope.FormName = "TDS Type Setup";
    $scope.btnDelete = false;
    $scope.TdsSetup = {
        TDS_CODE: "",
        TDS_EDESC: "",
        TDS_TYPE_CODE: "",
        REMARKS: "",

    }

    var getTDS = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getTDSList";
    $scope.grid = {
        change: TDSChange,
        dataSource: {
            transport: {
                read: getTDS,
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
            field: "TDS_EDESC",
            title: "Tds Name",
            width: 150
        }]

    }

    function TDSChange(evt) {
        debugger;
        selectedRow = this.select();
        var item = this.dataItem(selectedRow);
        if (item) {
            $scope.saveAction = "Update";
            $scope.TdsSetup.TDS_CODE = item.TDS_CODE;
            $scope.TdsSetup.TDS_EDESC = item.TDS_EDESC;
            $scope.TdsSetup.TDS_TYPE_CODE = item.TDS_TYPE_CODE;
            $scope.TdsSetup.REMARKS = item.REMARKS;
            $scope.btnDelete = true;
            $scope.$apply();
        }
    }

    $scope.SaveNewTds = function (isValid) {
        var model = {
            TDS_CODE: $scope.TdsSetup.TDS_CODE,
            TDS_EDESC: $scope.TdsSetup.TDS_EDESC,
            TDS_TYPE_CODE: $scope.TdsSetup.TDS_TYPE_CODE,
            REMARKS: $scope.TdsSetup.REMARKS,
        }
        if (!isValid) {
            displayPopupNotification("Please Fill the Required Fields", "warning");
            return;
        }
        if ($scope.saveAction == "Save") {
            var saveTDSUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createTDSSetup";
            $http({
                method: 'POST',
                url: saveTDSUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "INSERTED") {
                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully saved ", "success");
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("TDS Type name already exist please try another TDS Type name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            }, function errorCallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("TDS Type name already exist please try another TDS Type name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            });
        }
        else if ($scope.saveAction == "Update") {
            var udpateVehicleUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateTdsSetup";
            var model1 = {
                TDS_CODE: $scope.TdsSetup.TDS_CODE,
                TDS_EDESC: $scope.TdsSetup.TDS_EDESC,
                TDS_TYPE_CODE: $scope.TdsSetup.TDS_TYPE_CODE,
                REMARKS: $scope.TdsSetup.REMARKS,
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

    $scope.deleteDds = function () {
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
   
                    var TDSCODE;
                    if (TDSCODE == undefined) {
                        TDSCODE = $scope.TdsSetup.TDS_CODE;
                    }
                    var deleteTransporterUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/deleteTDsSetup?tdsCode=" + TDSCODE;
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
        $scope.TdsSetup = {
            TDS_EDESC: "",
            TDS_TYPE_CODE: "",
            REMARKS: "",

        };
        $scope.btnDelete = false;
        $("#grid").data("kendoGrid").clearSelection();

    }


    $scope.getTransactionCode = function () {
        var gettrancsactionCodeByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getMaxTdsCode";
        $http({
            method: 'GET',
            url: gettrancsactionCodeByUrl,

        }).then(function successCallback(response) {

            $scope.TdsSetup.TDS_CODE = response.data.DATA;
        }, function errorCallback(response) {
        });
    }

    $scope.getTransactionCode();

});

