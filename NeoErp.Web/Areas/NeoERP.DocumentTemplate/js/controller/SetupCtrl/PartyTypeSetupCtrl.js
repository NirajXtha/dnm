DTModule.controller('PartyTypeSetupCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    $scope.phoneNumbr = /^\+?(\d+(\.(\d+)?)?|\.\d+)$/;
    $scope.saveAction = "Save";
    $scope.btnDelete = false;
    $scope.PartyTypeSetup = {
        PARTY_TYPE_CODE: "",
        PARTY_TYPE_EDESC: "",
        ADDRESS: "",
        REMARKS: "",
        ACC_CODE: "",
        TEL_NO: "",
        CREDIT_LIMIT: "",
        CREDIT_DAYS: ""
    }
    var getpartyType = window.location.protocol + "//" + window.location.host + "/api/SetupApi/partyTypeList";
    $scope.grid = {
        change: PartyChange,
        dataSource: {
            transport: {
                read: getpartyType,
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
            field: "PARTY_TYPE_EDESC",
            title: " Party Name",
            width: 150
        }]

    }

    //account dd
    var accTypeUrl123 = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getAccountCodPrtyType";
    $scope.custaccountOptions = {
        dataTextField: "ACC_EDESC",
        dataValueField: "ACC_CODE",
        valuePrimitive: true,
        autoClose: true,
        headerTemplate: '<div class="col-md-offset-3"><strong>Select  acc...</strong></div>',
       
        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: accTypeUrl123,
                    dataType: "json"
                }
            }
        }
    };

    function PartyChange(evt) {
        debugger;
        selectedRow = this.select();
        var item = this.dataItem(selectedRow);
        if (item) {
            $scope.saveAction = "Update";
            $scope.PartyTypeSetup.PARTY_TYPE_CODE = item.PARTY_TYPE_CODE;
            $scope.PartyTypeSetup.PARTY_TYPE_EDESC = item.PARTY_TYPE_EDESC;
            $scope.PartyTypeSetup.ADDRESS = item.ADDRESS;
            $scope.PartyTypeSetup.ACC_CODE = item.ACC_CODE;
            $scope.PartyTypeSetup.REMARKS = item.REMARKS;
            $scope.PartyTypeSetup.TEL_NO = item.TEL_NO;
            $scope.PartyTypeSetup.CREDIT_LIMIT = item.CREDIT_LIMIT;
            $scope.PartyTypeSetup.CREDIT_DAYS = item.CREDIT_DAYS;
            $scope.btnDelete = true;
            $scope.$apply();
        }
    }

    $scope.SaveNewPartyType = function (isValid) {
        var model = {
            PARTY_TYPE_CODE: $scope.PartyTypeSetup.PARTY_TYPE_CODE,
            PARTY_TYPE_EDESC: $scope.PartyTypeSetup.PARTY_TYPE_EDESC,
            ADDRESS: $scope.PartyTypeSetup.ADDRESS,
            ACC_CODE: $scope.PartyTypeSetup.ACC_CODE,
            TEL_NO: $scope.PartyTypeSetup.TEL_NO,
            CREDIT_LIMIT: $scope.PartyTypeSetup.CREDIT_LIMIT,
            CREDIT_DAYS: $scope.PartyTypeSetup.CREDIT_DAYS,
            REMARKS: $scope.PartyTypeSetup.REMARKS,
        }

        if (!isValid) {
            displayPopupNotification("Please Fill the Required Fields", "warning");
            return;
        }
        if ($scope.saveAction == "Save") {
            var saveTDSUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewPaetyType";
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
                    displayPopupNotification("Party type name already exist please try another Party type name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            }, function errorCallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Party type name already exist please try another Party type name.", "error");
                }
                else { 
                displayPopupNotification("Something went wrong.Please try again later.", "error");
            }

            });
        }
        else if ($scope.saveAction == "Update") {
            var udpateVehicleUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updatePartyType";
            
            $http({
                method: 'POST',
                url: udpateVehicleUrl,
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

            }, function errorCallback(response) {
                displayPopupNotification("Something went wrong.Please try again later.", "error");

            });
        }
    }

    $scope.deleteDds = function () {
        debugger;
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
   
                    var PARTYCODE;
                    if (PARTYCODE == undefined) {
                        PARTYCODE = $scope.PartyTypeSetup.PARTY_TYPE_CODE;
                    }
                    var deleteTransporterUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/deletePartySetup?partyCode=" + PARTYCODE;
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
        debugger
        $scope.saveAction = "Save";
        $scope.PartyTypeSetup = {
            PARTY_TYPE_CODE: "",
            PARTY_TYPE_EDESC: "",
            ADDRESS: "",
            REMARKS: "",
            ACC_CODE: "",
            TEL_NO: "",
            CREDIT_LIMIT: "",
            CREDIT_DAYS: ""
        };

        $scope.getMaxPartyTypeCode()

        $scope.btnDelete = false;
    }


    $scope.getMaxPartyTypeCode = function () {
        var getMaxPartyTypeCodeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetMaxPartyTypeCode";
        $http({
            method: 'GET',
            url: getMaxPartyTypeCodeUrl,

        }).then(function successCallback(response) {
            $scope.PartyTypeSetup.PARTY_TYPE_CODE = response.data;
        }, function errorCallback(response) {
        });
    }

    //$scope.getTransactionCode();

    $scope.init = function () {
        $scope.getMaxPartyTypeCode();
    };

    $scope.init();

});

