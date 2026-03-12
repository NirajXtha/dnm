DTModule.controller('unitOfMeasurementCtrl', function ($scope, $http, $window) {
    $scope.FormName = "Unit Of Measurement";
    $scope.saveAction = "Save";
    $scope.btnDelete = false;
    $scope.MeasurementUnitObj = {
        SHORT_CUT: "",
        IN_NEPALI: "",
        IN_ENGLISH: "",
        REMARKS: "",
    };
    var getMeasurementUnit = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetAllMeasurementUnit";
    $scope.uGrid = {
        change: MeasurementUnitChange,
        dataSource: {
            transport: {
                read: getMeasurementUnit,
            },
            pageSize: 20
        },
        dataBound: function (e) {
        },
        height: window.innerHeight - 50,
        groupable: false,
        sortable: false,
        //height: 470,
        selectable: true,
        scrollable: {
            virtual: true
        },
        columns: [
            {
                field: "MU_EDESC",
                title: "Measurement Unit",
                width: 150
            }]
    };
    function MeasurementUnitChange(evt) {
        selectedRow = this.select();
        var item = this.dataItem(selectedRow);
        console.log("Item=======================<<<<<" + JSON.stringify(item));
        if (item) {
            $scope.saveAction = "Update";
            // $scope.TransporterHeader = "Update Vehicle";
            $('#short_cut').attr('readonly', true);
            $scope.MeasurementUnitObj.SHORT_CUT = item.MU_CODE;
            $scope.MeasurementUnitObj.IN_ENGLISH = item.MU_EDESC;
            $scope.MeasurementUnitObj.IN_NEPALI = item.MU_NDESC;
            $scope.MeasurementUnitObj.REMARKS = item.REMARKS;
            $scope.btnDelete = true;
            $scope.$apply();
        }
    }
    $scope.SaveMeasurementUnit = function (isValid) {
        var model = {
            MU_CODE: $scope.MeasurementUnitObj.SHORT_CUT,
            MU_EDESC: $scope.MeasurementUnitObj.IN_ENGLISH,
            MU_NDESC: $scope.MeasurementUnitObj.IN_NEPALI,
            REMARKS: $scope.MeasurementUnitObj.REMARKS,
        };
        var validation = [
            { short_cut: $scope.MeasurementForm.short_cut.$invalid },
            { in_english: $scope.MeasurementForm.in_english.$invalid }
        ];

        if (validation[0].short_cut === true) {
            displayPopupNotification("Please provide type shortcut", "warning");
            return;
        }
        if (validation[1].in_english === true) {
            displayPopupNotification(" Please provide name in english", "warning");
            return;
        }
        if ($scope.saveAction === "Save") {
            var saveUnitUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/SaveMeasurementUnit";
            $http({
                method: 'POST',
                url: saveUnitUrl,
                data: model
            }).then(function successCallback(response) {
                //debugger;
                if (response.data.MESSAGE === "INSERTED") {
                    $("#uGrid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully saved ", "success");
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Unit name already exist please try another unit name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            }, function errorCallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Unit name already exist please try another unit name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            });
        }
        else if ($scope.saveAction === "Update") {
            var udpateUnitUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/UpdateMeasurementUnit";
            var model1 = {
                MU_CODE: $scope.MeasurementUnitObj.SHORT_CUT,
                MU_EDESC: $scope.MeasurementUnitObj.IN_ENGLISH,
                MU_NDESC: $scope.MeasurementUnitObj.IN_NEPALI,
                REMARKS: $scope.MeasurementUnitObj.REMARKS
            };
            $http({
                method: 'POST',
                url: udpateUnitUrl,
                data: model1
            }).then(function successCallback(response) {
                //debugger
                if (response.data.MESSAGE === "UPDATED") {
                    $("#uGrid").data("kendoGrid").dataSource.read();
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
    };
    $scope.deleteMeasurementUnit = function () {
        bootbox.confirm({
            title: "Delete",
            message: "Are you sure?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success',
                    label: '<i class="fa fa-check"></i> Yes'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger',
                    label: '<i class="fa fa-times"></i> No'
                }
            },
            callback: function (result) {

                if (result === true) {
                    var MU_EDESC = $("#in_english").val();
                    var MU_CODE = $("#short_cut").val();
                    if (MU_CODE === undefined) {
                        MU_CODE = $scope.MeasurementUnitObj.SHORT_CUT;
                    }
                    var deleteUnitUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteMeasurementUnit?unitCode=" + MU_CODE;
                    $http({
                        method: 'POST',
                        url: deleteUnitUrl
                    }).then(function successCallback(response) {
                        if (response.data.MESSAGE === "DELETED") {
                            $("#uGrid").data("kendoGrid").dataSource.read();
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
                $("#uGrid").data("kendoGrid").dataSource.read();
                $scope.Cancel();
            }
        });
    };
    $scope.Cancel = function () {
        $scope.saveAction = "Save";
        $('#short_cut').attr('readonly', false);
        $scope.MeasurementUnitObj = {
            SHORT_CUT: "",
            IN_ENGLISH: "",
            IN_NEPALI: "",
            REMARKS: ""
        };
        $scope.btnDelete = false;
        $("#uGrid").data("kendoGrid").clearSelection();
    };
});