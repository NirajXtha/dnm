DTModule.controller('vehicleSetupCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    $scope.saveAction = "Save";
    $scope.btnDelete = false;
    $scope.VehicleSetup = {
        VEHICLE_CODE: "",
        VEHICLE_TYPE: "",
        VEHICLE_EDESC: "",
        VEHICLE_ID: "",
        OWNER_NAME: "",
        OWNER_MOBILE_NO: "",
        DRIVER_NAME: "",
        DRIVER_LICENCE_NO: "",
        DRIVER_MOBILE_NO: "",
        GROUP_EDESC: "",
        ACC_CODE: "",
        CREATED_DATE: "",
        MODIFY_DATE: "",
        VEHICLE_LABOUR_EDESC: "",
        MODIFY_BY: "",
        REMARKS: "",

    };

    var accTypeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAllAccountCodeForVeh";
    $scope.accountTypeDataSource = new kendo.data.DataSource({
        serverFiltering: true,
        transport: {
            read: {
                url: accTypeUrl,
            },
            parameterMap: function (data, action) {
                debugger;
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        }
    });
    $scope.accountpeOptions = {
        dataSource: $scope.accountTypeDataSource,
        filter: "contains",
        optionLabel: "--Select Account--",
        dataTextField: "ACC_EDESC",
        dataValueField: "ACC_CODE",

    }

    var getVehicle = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getVehicleList";
    $scope.grid = {
        change: VehicleChange,
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
                field: "VEHICLE_EDESC",
                title: "Vehicle Name",
                width: 150
        }]

    }


    
    $scope.VehicleType = {
        dataSource: $scope.VehicleSetup.VEHICLE_TYPE,
        dataTextField: 'VEHICLE_TYPE',
        dataValueField: 'value',
        filter: "contains",
        //maxSelectedItems: 1,
        select: function (e) {
            debugger;
            if (e.dataItem !== undefined) {
                $http({
                    method: 'GET',
                    url: window.location.protocol + "//" + window.location.host + "/api/setupapi/GetVehicleCode?vehicletype=" + e.dataItem.value,

                }).then(function successCallback(response) {
                    debugger;
                  
                    $scope.VehicleSetup.VEHICLE_CODE = response.data;
                    $scope.VehicleSetup.VEHICLE_ID = e.dataItem.value + '001';


                });
               

               
            }

        },


        dataBound: function (e) {

        },
        change: function (e) {


        }
    }



    function VehicleChange(evt) {
        debugger;
        selectedRow = this.select();
        var item = this.dataItem(selectedRow);

        if (item) {
            $scope.saveAction = "Update";
            $scope.TransporterHeader = "Update Vehicle";
          
            $scope.VehicleSetup.VEHICLE_CODE = item.VEHICLE_CODE;
            $scope.VehicleSetup.VEHICLE_TYPE = item.VEHICLE_TYPE;
            $scope.VehicleSetup.VEHICLE_EDESC = item.VEHICLE_EDESC;
            $scope.VehicleSetup.VEHICLE_ID = item.VEHICLE_ID;
            $scope.VehicleSetup.OWNER_NAME = item.OWNER_NAME;
            $scope.VehicleSetup.OWNER_MOBILE_NO = item.OWNER_MOBILE_NO;
            $scope.VehicleSetup.DRIVER_NAME = item.DRIVER_NAME;
            $scope.VehicleSetup.DRIVER_LICENCE_NO = item.DRIVER_LICENCE_NO;
            $scope.VehicleSetup.DRIVER_MOBILE_NO = item.DRIVER_MOBILE_NO;
            $scope.VehicleSetup.GROUP_EDESC = item.GROUP_EDESC
            $scope.VehicleSetup.ACC_CODE = item.ACC_CODE;
            $scope.VehicleSetup.VEHICLE_LABOUR_EDESC = item.VEHICLE_LABOUR_EDESC;
            $scope.VehicleSetup.REMARKS = item.REMARKS;
            $scope.btnDelete = true;
            $scope.$apply();
        }
        $('#vehicletype').attr("disabled", true);
    }

   

    $scope.SaveTransporter = function (isValid) {

        var model = {
            VEHICLE_CODE: $scope.VehicleSetup.VEHICLE_CODE,
            VEHICLE_TYPE: $scope.VehicleSetup.VEHICLE_TYPE,
            VEHICLE_EDESC: $scope.VehicleSetup.VEHICLE_EDESC,
            VEHICLE_ID: $scope.VehicleSetup.VEHICLE_ID,
            OWNER_NAME: $scope.VehicleSetup.OWNER_NAME,
            OWNER_MOBILE_NO: $scope.VehicleSetup.OWNER_MOBILE_NO,
            DRIVER_NAME: $scope.VehicleSetup.DRIVER_NAME,
            DRIVER_LICENCE_NO: $scope.VehicleSetup.DRIVER_LICENCE_NO,
            DRIVER_MOBILE_NO: $scope.VehicleSetup.DRIVER_MOBILE_NO,
            GROUP_EDESC: $scope.VehicleSetup.GROUP_EDESC,
            ACC_CODE: $scope.VehicleSetup.ACC_CODE,
            VEHICLE_LABOUR_EDESC: $scope.VehicleSetup.VEHICLE_LABOUR_EDESC,
            REMARKS: $scope.VehicleSetup.REMARKS,
        }
      


        var validation = [
                { vehicletype: $scope.VehicleForm.vehicletype.$invalid },
                { drivercontact: $scope.VehicleForm.drivercontact.$invalid },
                { vehiclename: $scope.VehicleForm.vehiclename.$invalid },
                { ownercontact: $scope.VehicleForm.ownercontact.$invalid },
              
        ]

        if (validation[0].vehicletype == true) {

            displayPopupNotification("Enter Vehicle Type", "warning");
            return
        }
        if (validation[1].drivercontact == true) {

            displayPopupNotification(" Driver Contact NO should Be 10 Digits", "warning");
        
            return
        }

        if (validation[2].vehiclename == true) {

            displayPopupNotification(" Enter Vehicle Name", "warning");

            return
        }
        if (validation[3].ownercontact == true) {

            displayPopupNotification("Owner Contact NO should Be 10 Digits ", "warning");

            return
        }
        if ($scope.saveAction == "Save") {
            var saveVehicleUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewVehicleSetup";
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
                    displayPopupNotification("Vehicle name already exist please try another vehicle name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
               
            }, function errorCallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Vehicle name already exist please try another vehicle name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
               
            });
        }
        else if ($scope.saveAction == "Update")
        {
            var udpateVehicleUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateNewVehicleSetup";
            var model1 = {
                VEHICLE_CODE: $scope.VehicleSetup.VEHICLE_CODE,
                VEHICLE_TYPE: $scope.VehicleSetup.VEHICLE_TYPE,
                VEHICLE_EDESC: $scope.VehicleSetup.VEHICLE_EDESC,
                VEHICLE_ID: $scope.VehicleSetup.VEHICLE_ID,
                OWNER_NAME: $scope.VehicleSetup.OWNER_NAME,
                OWNER_MOBILE_NO: $scope.VehicleSetup.OWNER_MOBILE_NO,
                DRIVER_NAME: $scope.VehicleSetup.DRIVER_NAME,
                DRIVER_LICENCE_NO: $scope.VehicleSetup.DRIVER_LICENCE_NO,
                DRIVER_MOBILE_NO: $scope.VehicleSetup.DRIVER_MOBILE_NO,
                GROUP_EDESC: $scope.VehicleSetup.GROUP_EDESC,
                ACC_CODE: $scope.VehicleSetup.ACC_CODE,
                VEHICLE_LABOUR_EDESC: $scope.VehicleSetup.VEHICLE_LABOUR_EDESC,
                REMARKS: $scope.VehicleSetup.REMARKS,
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
                    var VEHICLE_EDESC = $("#vehiclename").val();
                    var VEHICLE_CODE = $("#vehicleCode").val();
                    var VEHICLECODE;
                    if (VEHICLECODE == undefined) {
                        VEHICLECODE = $scope.VehicleSetup.VEHICLE_CODE;
                    }
                    var deleteTransporterUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/deleteVehicleSetup?vehicleCode=" + VEHICLECODE;
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
        debugger;
       
        $scope.saveAction = "Save";
        $scope.TransporterHeader = "Add Vehicle";
        $scope.VehicleSetup = {
            VEHICLE_CODE: "",
            VEHICLE_TYPE: "",
            VEHICLE_EDESC: "",
            VEHICLE_ID: "",
            OWNER_NAME: "",
            OWNER_MOBILE_NO: "",
            DRIVER_NAME: "",
            DRIVER_LICENCE_NO: "",
            DRIVER_MOBILE_NO: "",
            GROUP_EDESC: "",
            ACC_CODE: "",
            CREATED_DATE: "",
            MODIFY_DATE: "",
            VEHICLE_LABOUR_EDESC: "",
            MODIFY_BY: "",
            REMARKS: "",

        };
       
        $scope.btnDelete = false;
        $("#grid").data("kendoGrid").clearSelection();
        setTimeout(function () {
            $('#vehicletype').attr("disabled", false);
        }, 100);

    }


});

