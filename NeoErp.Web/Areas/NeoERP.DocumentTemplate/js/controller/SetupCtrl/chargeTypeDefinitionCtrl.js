DTModule.controller('chargeTypeDefinitionCtrl', function ($scope, $http) {

    $scope.FormName = "Charge Setup";
    $scope.saveAction = "Save";
    $scope.btnDelete = false;

    $scope.ChargeSetupObj = {
        SHORT_CUT: "",
        IN_NEPALI: "",
        IN_ENGLISH: "",
        SPECIFIC_CHARGE: "",
        REMARKS: "",

    };



    var getCharges = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetCharges";
    $scope.cRGrid = {
        change: ChargeChange,
        dataSource: {
            transport: {
                read: getCharges,
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
                field: "CHARGE_EDESC",
                title: "Charge List",
                width: 150
            }]

    }


    function ChargeChange(evt) {

        selectedRow = this.select();
        var item = this.dataItem(selectedRow);

        console.log("Item=================>>>>" + JSON.stringify(item));

        if (item) {
            $scope.saveAction = "Update";


            $scope.ChargeSetupObj.SHORT_CUT = item.CHARGE_CODE;
            $scope.ChargeSetupObj.IN_ENGLISH = item.CHARGE_EDESC;
            $scope.ChargeSetupObj.IN_NEPALI = item.CHARGE_NDESC;
            $scope.ChargeSetupObj.SPECIFIC_CHARGE = item.SPECIFIC_CHARGE_FLAG;
            $scope.ChargeSetupObj.REMARKS = item.REMARKS;
            $scope.btnDelete = true;
            $scope.$apply();
        }
    }


    $scope.SaveChargeSetup = function (isValid) {
        debugger;
        var model = {
            CHARGE_CODE: $scope.ChargeSetupObj.SHORT_CUT,
            CHARGE_EDESC: $scope.ChargeSetupObj.IN_ENGLISH,
            CHARGE_NDESC: $scope.ChargeSetupObj.IN_NEPALI,
            SPECIFIC_CHARGE_FLAG: $scope.ChargeSetupObj.SPECIFIC_CHARGE,
            REMARKS: $scope.ChargeSetupObj.REMARKS,
        }



        var validation = [
            { short_cut: $scope.ChargeSetupForm.short_cut.$invalid },
            { in_english: $scope.ChargeSetupForm.in_english.$invalid },
            { district: $scope.ChargeSetupForm.charge_flag.$invalid },
        ]

        if (validation[0].short_cut == true) {

            displayPopupNotification("Please provide type shortcut", "warning");
            return
        }
        if (validation[1].in_english == true) {

            displayPopupNotification(" Please provide name in english", "warning");

            return
        }

        if (validation[2].district == true) {

            displayPopupNotification(" Please provide Specific Charge", "warning");

            return
        }

        if ($scope.saveAction == "Save") {
            var saveChargeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/SaveChargeSetup";
            $http({
                method: 'POST',
                url: saveChargeUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "INSERTED") {
                    $("#cRGrid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully saved ", "success");
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Charge Type name already exist please try another Charge Type name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            }, function errorCallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Charge Type name already exist please try another Charge Type name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            });
        }
        else if ($scope.saveAction == "Update") {
            var udpateChargeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/UpdateChargeSetup";
            var model1 = {
                CHARGE_CODE: $scope.ChargeSetupObj.SHORT_CUT,
                CHARGE_EDESC: $scope.ChargeSetupObj.IN_ENGLISH,
                CHARGE_NDESC: $scope.ChargeSetupObj.IN_NEPALI,
                SPECIFIC_CHARGE_FLAG: $scope.ChargeSetupObj.SPECIFIC_CHARGE,
                REMARKS: $scope.ChargeSetupObj.REMARKS,
            }
            $http({
                method: 'POST',
                url: udpateChargeUrl,
                data: model1
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "UPDATED") {
                    $("#cRGrid").data("kendoGrid").dataSource.read();
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


    $scope.DeleteChargeSetup = function () {
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
                    var CHARGE_EDESC = $("#in_english").val();
                    var CHARGE_CODE = $("#short_cut").val();
                    if (CHARGE_CODE == undefined) {
                        CHARGE_CODE = $scope.ChargeSetupObj.SHORT_CUT;
                    }
                    var deleteChargeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteChargeSetup?chargeCode=" + CHARGE_CODE;
                    $http({
                        method: 'POST',
                        url: deleteChargeUrl,
                    }).then(function successCallback(response) {
                        if (response.data.MESSAGE == "DELETED") {
                            $("#cRGrid").data("kendoGrid").dataSource.read();
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
                $("#cRGrid").data("kendoGrid").dataSource.read();
                $scope.Cancel();
            }

        });


    }


    //var districtUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetDistricts";
    //$scope.districtDataSource = new kendo.data.DataSource({
    //    transport: {
    //        read: {
    //            url: districtUrl,
    //        },
    //    }
    //});
    //$scope.districtOptions = {
    //    dataSource: $scope.districtDataSource,
    //    filter: "contains",
    //    optionLabel: "--Select District--",
    //    dataTextField: "DISTRICT_EDESC",
    //    dataValueField: "DISTRICT_CODE",

    //}


    $scope.Cancel = function () {
        $scope.saveAction = "Save";
        $scope.ChargeSetupObj = {
            SHORT_CUT: "",
            IN_ENGLISH: "",
            IN_NEPALI: "",
            SPECIFIC_CHARGE: "",
            REMARKS: "",

        };

        $scope.btnDelete = false;
        $("#cRGrid").data("kendoGrid").clearSelection();

    }

});