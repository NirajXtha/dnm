DTModule.controller('currencySetupCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    $scope.saveAction = "Save";
    $scope.FormName = "Currency Setup";
    $scope.btnDelete = false;
    $scope.CurrencySetup = {
        CURRENCY_CODE: "",
        CURRENCY_SYMBOL: "",
        ENG_CURRENCY: "",
        NEP_CURRENCY: "",
        COUNTRY: "",
        CREATED_DATE: "",
        MODIFY_DATE: "",
        MODIFY_BY: "",
        REMARKS: ""
    };
    var getCurrencyUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getCurrencyList";
    $scope.grid = {
        change: CurrencyChange,
        dataSource: {
            transport: {
                read: getCurrencyUrl,
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
                field: "CURRENCY_EDESC",
                title: "Currency Name",
                width: 150
            }
        ]
    }


    function CurrencyChange(evt) {

        selectedRow = this.select();
        var item = this.dataItem(selectedRow);
        $('#currencyCode').attr("disabled", true);
        if (item) {
            $scope.saveAction = "Update";
            $scope.TransporterHeader = "Update Currency";

            $scope.CurrencySetup.REMARKS = item.REMARKS;
            $scope.CurrencySetup.CURRENCY_CODE = item.CURRENCY_CODE;
            $scope.CurrencySetup.CURRENCY_SYMBOL = item.CURRENCY_SYMBOL;
            $scope.CurrencySetup.ENG_CURRENCY = item.CURRENCY_EDESC;
            $scope.CurrencySetup.NEP_CURRENCY = item.CURRENCY_NDESC;
            $scope.CurrencySetup.COUNTRY = item.COUNTRY;
            $scope.btnDelete = true;
            $scope.$apply();
        }
    }

    $scope.SaveTransporter = function (isValid) {
        debugger;
        if (!$scope.CurrencySetup.CURRENCY_CODE) {
            displayPopupNotification("Invlaid Currency Code", "error");
        }
        else if (!$scope.CurrencySetup.ENG_CURRENCY) {
            displayPopupNotification("Invlaid in English", "error");
        }
        else{
        if ($scope.saveAction == "Save") {
                var model = {
                    REMARKS: $scope.CurrencySetup.REMARKS,
                    CURRENCY_CODE: $scope.CurrencySetup.CURRENCY_CODE,
                    CURRENCY_SYMBOL: $scope.CurrencySetup.CURRENCY_SYMBOL,
                    CURRENCY_EDESC: $scope.CurrencySetup.ENG_CURRENCY,
                    CURRENCY_NDESC: $scope.CurrencySetup.NEP_CURRENCY,
                    COUNTRY: $scope.CurrencySetup.COUNTRY
                }
                var saveVehicleUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewCurrencySetup";
                $http({
                    method: 'POST',
                    url: saveVehicleUrl,
                    data: model
                }).then(function successCallback(response) {
                    debugger;
                    if (response.data.MESSAGE == "INSERTED") {
                        $("#grid").data("kendoGrid").dataSource.read();
                        $scope.Cancel();
                        displayPopupNotification("Data succesfully saved ", "success");
                    }
                    else if (response.data.MESSAGE == "INVALIDINSERTED") {
                        displayPopupNotification("Currency name already exist please try another Currency name.", "error");
                    }
                    else {
                        displayPopupNotification("Something went wrong.Please try again later.", "error");
                    }

                }, function errorCallback(response) {
                    if (response.data.MESSAGE == "INVALIDINSERTED") {
                        displayPopupNotification("Currency name already exist please try another Currency name.", "error");
                    }
                    else {
                        displayPopupNotification("Something went wrong.Please try again later.", "error");
                    }

                });
             }
            else if ($scope.saveAction == "Update") {
                debugger;
                var udpateCurrencyUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateNewCurrencySetup";
                var model1 = {
                    REMARKS: $scope.CurrencySetup.REMARKS,
                    CURRENCY_CODE: $scope.CurrencySetup.CURRENCY_CODE,
                    CURRENCY_SYMBOL: $scope.CurrencySetup.CURRENCY_SYMBOL,
                    CURRENCY_EDESC: $scope.CurrencySetup.ENG_CURRENCY,
                    CURRENCY_NDESC: $scope.CurrencySetup.NEP_CURRENCY,
                    COUNTRY: $scope.CurrencySetup.COUNTRY
                }
                $http({
                    method: 'POST',
                    url: udpateCurrencyUrl,
                    data: model1
                }).then(function successCallback(response) {
                    if (response.data.MESSAGE == "UPDATED") {
                        debugger;
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
                debugger;
                if (result == true) {
                    var CURRENCY_EDESC = $("#engCurrency").val();
                    var CURRENCY_CODE = $("#currencyCode").val();
                    if (CURRENCY_CODE == undefined) {
                        CURRENCY_CODE = $scope.CurrencySetup.CURRENCY_CODE;
                    }
                    var deleteTransporterUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/deleteCurrencySetup?currencyCode=" + CURRENCY_CODE;
                    $http({
                        method: 'POST',
                        url: deleteTransporterUrl,
                    }).then(function successCallback(response) {
                        debugger;
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
                else {
                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                }
            }
        });
    }

    $scope.Cancel = function () {
        $scope.saveAction = "Save";
        $scope.CurrencySetup = {
            CURRENCY_CODE: "",
            CURRENCY_SYMBOL: "",
            IN_ENGLISH: "",
            IN_NEPALI: "",
            COUNTRY: "",
            REMARKS: ""
        };

        $scope.btnDelete = false;
        $("#grid").data("kendoGrid").clearSelection();
        setTimeout(function () {
            $('#currencyCode').attr("disabled", false);
        }, 100);
    }
});

