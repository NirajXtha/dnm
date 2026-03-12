DTModule.controller('transporterSetupTreeCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    $scope.saveAction = "Save";
    $scope.btnDelete = false;
    $scope.phoneNumbr = /^\+?(\d+(\.(\d+)?)?|\.\d+)$/;
    $scope.Transporter = {
        TRANSPORTER_CODE: "",
        TRANSPORTER_EDESC: "",
        PAN_NO: "",
        DEPOSIT_AMOUNT: "",
        PROPRITER_NAME: "",
        PHONE_NO: "",
        ADDRESS: "",
        REMARKS: "",
        COMPANY_CODE: "",
        DELETED_FLAG: "",
        CREATED_BY: "",
        CREATED_DATE: "",
        MODIFY_DATE: "",
        SYN_ROWID: "",
        MODIFY_BY: "",
        PRIORITY: "",
        TRANSPORTER_NDESC: "",
    };
    $scope.pan1 = /[a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}/;
    $scope.TransporterHeader = "Add Transporter";
    $scope.grid = {
        change: TransporterChange,
        dataSource: {
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getTransporterCodeWithChild",
            },
            pageSize: 20
        },
        dataBound: function (e) {
            //GetSetupSetting("TransporterSetup");
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
                field: "TRANSPORTER_EDESC",
                title: "Transporter Name",
                width: 150
            }]

    }
    function TransporterChange(evt) {
        selectedRow = this.select();
        var item = this.dataItem(selectedRow);

        if (item) {
            $scope.saveAction = "Update";
            $scope.TransporterHeader = "Update Transporter";

            $scope.Transporter.TRANSPORTER_CODE = item.TRANSPORTER_CODE;
            $scope.Transporter.TRANSPORTER_EDESC = item.TRANSPORTER_EDESC;
            $scope.Transporter.PAN_NO = item.PAN_NO;
            $scope.Transporter.DEPOSIT_AMOUNT = item.DEPOSIT_AMOUNT;
            $scope.Transporter.PROPRITER_NAME = item.PROPRITER_NAME;
            $scope.Transporter.PHONE_NO = item.PHONE_NO;
            $scope.Transporter.ADDRESS = item.ADDRESS;
            $scope.Transporter.REMARKS = item.REMARKS;
            $scope.Transporter.PRIORITY = item.PRIORITY;
            $scope.Transporter.TRANSPORTER_NDESC = item.TRANSPORTER_NDESC;
            $scope.btnDelete = true;
            $scope.$apply();
        }
    }
    $scope.getMaxTransporterCode = function () {
        var getMaxCodeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getMaxTransporterCode";
        $http({
            method: 'Get',
            url: getMaxCodeUrl,
        }).then(function successCallback(response) {
            $scope.Transporter.TRANSPORTER_CODE = response.data.DATA;
        }, function errorCallback(response) {
            displayPopupNotification(response.data.STATUS_CODE, "error");
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
    }
    $scope.getMaxTransporterCode();
    $scope.SaveTransporter = function (isValid) {
        var model = {
            TRANSPORTER_CODE: $scope.Transporter.TRANSPORTER_CODE,
            TRANSPORTER_EDESC: $scope.Transporter.TRANSPORTER_EDESC,
            PAN_NO: $scope.Transporter.PAN_NO,
            DEPOSIT_AMOUNT: $scope.Transporter.DEPOSIT_AMOUNT,
            PROPRITER_NAME: $scope.Transporter.PROPRITER_NAME,
            PHONE_NO: $scope.Transporter.PHONE_NO,
            ADDRESS: $scope.Transporter.ADDRESS,
            REMARKS: $scope.Transporter.REMARKS,
            PRIORITY: $scope.Transporter.PRIORITY,
            TRANSPORTER_NDESC: $scope.Transporter.TRANSPORTER_NDESC,
        }
        if (!isValid) {
            displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
            return;
        }
        if ($scope.saveAction == "Save") {
            var saveTransporterUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewTransporterSetup";
            $http({
                method: 'POST',
                url: saveTransporterUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "INSERTED") {
                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully saved ", "success");
                }
               else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Transporter name already exist please try another transporter name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // this callback will be called asynchronously
                // when the response is available
            }, function errorCallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Transporter name already exist please try another transporter name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
        }
        else {
            var udpateTransporterUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateTransporterSetup";
            $http({
                method: 'POST',
                url: udpateTransporterUrl,
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
    }

    $scope.deleteTransporter = function () {
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
                    var TRANSPORTER_EDESC = $("#TransporterName").val();
                    var TRANSPORTER_CODE = $("#TransporterCode").val();
                    var TRANSPORTERID;
                    if (TRANSPORTERID == undefined) {
                        TRANSPORTERID = $scope.Transporter.TRANSPORTER_CODE;
                    }
                    var deleteTransporterUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/deleteTransporterSetup?transporterCode=" + TRANSPORTERID;
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
                        // this callback will be called asynchronously
                        // when the response is available
                    }, function errorCallback(response) {
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                        // called asynchronously if an error occurs
                        // or server returns response with an error status.
                    });
                }
                $("#grid").data("kendoGrid").dataSource.read();
                $scope.Cancel();
            }

        });


    }

    $scope.Cancel = function () {
        $scope.saveAction = "Save";
        $scope.TransporterHeader = "Add Transporter";
        $scope.Transporter = {
            TRANSPORTER_CODE: "",
            TRANSPORTER_EDESC: "",
            PAN_NO: "",
            DEPOSIT_AMOUNT: "",
            PROPRITER_NAME: "",
            PHONE_NO: "",
            ADDRESS: "",
            REMARKS: "",
            PRIORITY: "",
            TRANSPORTER_NDESC: "",
        };
        $scope.getMaxTransporterCode();
        $scope.btnDelete = false;
        $("#grid").data("kendoGrid").clearSelection();
        
    }


});

