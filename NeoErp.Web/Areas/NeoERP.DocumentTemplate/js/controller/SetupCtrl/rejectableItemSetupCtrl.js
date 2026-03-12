DTModule.controller('rejectableItemSetupCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    $scope.saveAction = "Save";
    $scope.FormName = "Rejectable Setup";
    $scope.btnDelete = false;   
    $scope.RejectableItemSetup = {
        ITEM_ID: "",
        ITEM_CODE: "",
        ITEM_NAME: "",
        REMARKS: "",
        CREATED_BY: "",
        CREATED_DATE: "",
        COMPANY_CODE: "",
    };
    $scope.TransporterHeader = "Add RejectableItem";
    var getVehicle = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getRejetableItem";
    $scope.grid = {
        change: RejectableChange,
        dataSource: {
            transport: {
                read: getVehicle,
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
            field: "ITEM_NAME",
            title: "Item Name",
            width: 150
        }]

    }

    $scope.Cancel = function () {
        $scope.saveAction = "Save";
        $scope.TransporterHeader = "Add RejectableItem";
        $scope.RejectableItemSetup = {
            ITEM_CODE: "",
            ITEM_NAME: "",
            REMARKS: "",
        };
        $scope.btnDelete = false;
        $("#grid").data("kendoGrid").clearSelection();

    }

    $scope.SaveRejectable = function (isValid) {

        var model = {
            ITEM_ID: $scope.RejectableItemSetup.ITEM_ID,
            ITEM_CODE: $scope.RejectableItemSetup.ITEM_CODE,
            ITEM_NAME: $scope.RejectableItemSetup.ITEM_NAME,
            REMARKS: $scope.RejectableItemSetup.REMARKS,
        }
        if (!isValid) {
            displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
            return;
        }
        if ($scope.saveAction == "Save") {
            var saveRejectItemUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewRejectableSetup";
            $http({
                method: 'POST',
                url: saveRejectItemUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "INSERTED") {
                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully saved ", "success");
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Rejectable item name already exist please try another Rejectable item.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            }, function errorCallback(response) {
                 if (response.data.MESSAGE == "INVALIDINSERTED") {
                     displayPopupNotification("Rejectable item name already exist please try another Rejectable item.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            });
        }
        else if ($scope.saveAction == "Update") {
            var udpateRejectyableUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateRejectableSetup";
            var model1 = {
                ITEM_ID: $scope.RejectableItemSetup.ITEM_ID,
                ITEM_CODE: $scope.RejectableItemSetup.ITEM_CODE,
                ITEM_NAME: $scope.RejectableItemSetup.ITEM_NAME,
                REMARKS: $scope.RejectableItemSetup.REMARKS,
            }
            $http({
                method: 'POST',
                url: udpateRejectyableUrl,
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

    function RejectableChange(evt) {

        selectedRow = this.select();
        var item = this.dataItem(selectedRow);

        if (item) {
            $scope.saveAction = "Update";
            $scope.TransporterHeader = "Update Vehicle";
            $scope.RejectableItemSetup.ITEM_ID = item.ITEM_ID;
            $scope.RejectableItemSetup.ITEM_CODE = item.ITEM_CODE;
            $scope.RejectableItemSetup.ITEM_NAME = item.ITEM_NAME;
            $scope.RejectableItemSetup.REMARKS = item.REMARKS;
            $scope.btnDelete = true;
            $scope.$apply();
        }
    }

    $scope.deleteRejectableItem = function () {
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
                    var ITEM_NAME = $("#itemname").val();
                    var ITEMID;
                    if (ITEMID == undefined) {
                        ITEMID = $scope.RejectableItemSetup.ITEM_ID;
                    }
                    var deleteTransporterUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/deleteRejectableItemSetup?itemId=" + ITEMID;
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

    $scope.filterTreeView = function () {
        const searchText = ($scope.txtMainGroupSearchString || '').toLowerCase();

        var dataSource = $("#grid").data("kendoGrid").dataSource;

        if (searchText) {
            dataSource.filter({
                field: "ITEM_NAME",
                operator: "contains",
                value: searchText
            });
        } else {
            dataSource.filter({});
        }
    };

});

