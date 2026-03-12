DTModule.controller('citySetupCtrl', function ($scope, $http, $window) {

    $scope.FormName = "City Setup";
    $scope.saveAction = "Save";
    $scope.btnDelete = false;

    $scope.CitySetupObj = {
        SHORT_CUT: "",
        IN_NEPALI: "",
        IN_ENGLISH: "",
        DISTRICT: "",
        VILLAGE_CITY:"",
        REMARKS: "",

    };



    var getCities= window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetCities";
    $scope.cGrid = {
        change: CityChange,
        dataSource: {
            transport: {
                read: getCities,
            },
            pageSize: 20
        },
        dataBound: function (e) {

        },
        height: window.innerHeight - 50,
        groupable: false,
        sortable: false,
        // height: 470,
        selectable: true,
        scrollable: {
            virtual: true
        },
        columns: [
            {
                field: "CITY_EDESC",
                title: "CITIES/VILLAGE",
                width: 150
            }]

    };


    function CityChange(evt) {

        selectedRow = this.select();
        var item = this.dataItem(selectedRow);

        console.log("Item=================>>>>" + JSON.stringify(item));
        
        if (item) {
            $scope.saveAction = "Update";
          

            $scope.CitySetupObj.SHORT_CUT = item.CITY_CODE;
            $scope.CitySetupObj.IN_ENGLISH = item.CITY_EDESC;
            $scope.CitySetupObj.IN_NEPALI = item.CITY_NDESC;
            $scope.CitySetupObj.DISTRICT = item.DISTRICT_CODE;
            $scope.CitySetupObj.VILLAGE_CITY = item.CITY_VDC_FLAG;
            $scope.CitySetupObj.REMARKS = item.REMARKS;
            $scope.btnDelete = true;
            $scope.$apply();
        }
    }


    $scope.SaveCitySetup = function (isValid) {
        debugger;
        var model = {
            CITY_CODE: $scope.CitySetupObj.SHORT_CUT,
            CITY_EDESC: $scope.CitySetupObj.IN_ENGLISH,
            CITY_NDESC: $scope.CitySetupObj.IN_NEPALI,
            CITY_VDC_FLAG: $scope.CitySetupObj.VILLAGE_CITY,
            DISTRICT_CODE: $scope.CitySetupObj.DISTRICT,
            REMARKS: $scope.CitySetupObj.REMARKS
        };



        var validation = [
            { short_cut: $scope.CitySetupForm.short_cut.$invalid },
            { in_english: $scope.CitySetupForm.in_english.$invalid },
            { district: $scope.CitySetupForm.district.$invalid }
        ];

        if (validation[0].short_cut === true) {

            displayPopupNotification("Please provide type shortcut", "warning");
            return;
        }
        if (validation[1].in_english === true) {

            displayPopupNotification(" Please provide name in english", "warning");

            return;
        }
        if ($scope.CitySetupObj.VILLAGE_CITY == null || $scope.CitySetupObj.VILLAGE_CITY == "" || $scope.CitySetupObj.VILLAGE_CITY == undefined) {

            displayPopupNotification(" Please choose City/Village", "warning");

            return;
        }

        if ($scope.CitySetupObj.DISTRICT == null || $scope.CitySetupObj.DISTRICT == "" || $scope.CitySetupObj.DISTRICT == undefined) {

            displayPopupNotification(" Please provide district", "warning");

            return;
        }

        if ($scope.saveAction === "Save") {
            var saveTypeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/SaveCitySetup";
            $http({
                method: 'POST',
                url: saveTypeUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE === "INSERTED") {
                    $("#cGrid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully saved ", "success");
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("City name already exist please try another City name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            }, function errorCallback(response) {
                if(response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("City name already exist please try another city name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            });
        }
        else if ($scope.saveAction === "Update") {
            var udpateVehicleUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/UpdateCitySetup";
            var model1 = {
                CITY_CODE: $scope.CitySetupObj.SHORT_CUT,
                CITY_EDESC: $scope.CitySetupObj.IN_ENGLISH,
                CITY_NDESC: $scope.CitySetupObj.IN_NEPALI,
                CITY_VDC_FLAG: $scope.CitySetupObj.VILLAGE_CITY,
                DISTRICT_CODE: $scope.CitySetupObj.DISTRICT,
                REMARKS: $scope.CitySetupObj.REMARKS
            };
            $http({
                method: 'POST',
                url: udpateVehicleUrl,
                data: model1
            }).then(function successCallback(response) {
                if (response.data.MESSAGE === "UPDATED") {
                    $("#cGrid").data("kendoGrid").dataSource.read();
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


    $scope.DeleteCity = function () {
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

                if (result === true) {
                    var CITY_EDESC = $("#in_english").val();
                    var CITY_CODE = $("#short_cut").val();
                    if (CITY_CODE === undefined) {
                        CITY_CODE = $scope.CitySetupObj.SHORT_CUT;
                    }
                    var deleteTypeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteCitySetup?cityCode=" + CITY_CODE;
                    $http({
                        method: 'POST',
                        url: deleteTypeUrl
                    }).then(function successCallback(response) {
                        if (response.data.MESSAGE === "DELETED") {
                            $("#cGrid").data("kendoGrid").dataSource.read();
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
                $("#cGrid").data("kendoGrid").dataSource.read();
                $scope.Cancel();
            }

        });


    };


    var districtUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetDistricts";
    $scope.districtDataSource = new kendo.data.DataSource({
       // serverFiltering: true,
        transport: {
            read: {
                url: districtUrl
            },
            //parameterMap: function (data, action) {
            //    var newParams;
            //    if (data.filter != undefined) {
            //        if (data.filter.filters[0] != undefined) {
            //            newParams = {
            //                filter: data.filter.filters[0].value
            //            };
            //            return newParams;
            //        }
            //        else {
            //            newParams = {
            //                filter: ""
            //            };
            //            return newParams;
            //        }
            //    }
            //    else {
            //        newParams = {
            //            filter: ""
            //        };
            //        return newParams;
            //    }
            //}
        }
    });
    $scope.districtOptions = {
        dataSource: $scope.districtDataSource,
        filter: "contains",
        optionLabel: "--Select District--",
        dataTextField: "DISTRICT_EDESC",
        dataValueField: "DISTRICT_CODE"

    };


    $scope.Cancel = function () {
        $scope.getMaxCityCode();
        $scope.saveAction = "Save";
        $scope.CitySetupObj = {
            SHORT_CUT: "",
            IN_ENGLISH: "",
            IN_NEPALI: "",
            DISTRICT: "",
            VILLAGE_CITY: "",
            REMARKS: ""

        };

        $scope.btnDelete = false;
        $("#cGrid").data("kendoGrid").clearSelection();

    };


    $scope.getMaxCityCode = function () {
        var getMaxCityCode = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetMaxCityCode";
        $http({
            method: 'GET',
            url: getMaxCityCode,
        }).then(function successCallback(response) {
            debugger
            $scope.CitySetupObj.SHORT_CUT = response.data;
        })
    };

    $scope.init = function () {
        $scope.getMaxCityCode();
    };

    $scope.init();
});