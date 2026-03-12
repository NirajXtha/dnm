planningModule.controller('CollectionPlanSetupCtrl', function ($scope, plansetupservice, $routeParams, $http, $rootScope) {

    $rootScope.reservedData;

    $scope.p_code = $routeParams.plancode;
    $scope.planName = $routeParams.planName;
    $scope.ref = $routeParams.ref;

    $scope.param = $rootScope.reservedData;

    $scope.pageName = "Collection Planning setup Index";
    $scope.treelistOptions = [];
    $scope.Frequency = [];
    $scope.ITEM = [];
    $scope.Plan = [];
    $scope.startDate = [];
    $scope.endDate = [];
    $scope.employeeFlag = '';
    //$rootScope.BackEmployeeCode = '';
    //$rootScope.BackDivisionCode = '';
    getFlagFromPreferenceSetup();
    getDivisionFlagByPreferenceSetup();

    function getFlagFromPreferenceSetup() {
        var url = window.location.protocol + "//" + window.location.host + "/api/PlanApi/GetPreferenceSetupFlag";
        var req = {
            method: 'GET',
            url: url,
        }
        $http(req).then(function (data, status, headers, config) {
            $scope.employeeFlag = data.data.DATA[0].SHOW_EMPLOYEE;

            //if ($scope.employeeFlag == 'Y')
            $scope.bindEmployeeOption();
        }, function (data, status, headers, config) {
        });
    };

    function getDivisionFlagByPreferenceSetup() {
        var url = window.location.protocol + "//" + window.location.host + "/api/PlanApi/GetDivisionFlag";
        var req = {
            method: 'GET',
            url: url,
        }
        $http(req).then(function (data, status, headers, config) {
            if (data.data.DATA === "Y") {
                $scope.divisionFlag = true;
            }
            else {
                $scope.divisionFlag = false;
            }

        }, function (data, status, headers, config) {
        });
    };

    $scope.bindEmployeeOption = function () {
        $scope.employeeDataSource = {
            type: "json",
            serverFiltering: false,
            transport: {
                read: {
                    dataType: "json",
                    url: "/api/SubPlanApi/GetChiledLevelEmployee",
                },
                parameterMap: function (data, action) {
                    if (data.filter != undefined) {
                        if (data.filter.filters[0] != undefined) {
                            var newParams = {
                                filter: data.filter.filters[0].value
                            };
                            return newParams;
                        }
                        else {
                            var newParams = {
                                filter: ""
                            };
                            return newParams;
                        }
                    }
                    else {
                        var newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
            }
        };

        $scope.employeeOptions = {
            dataSource: $scope.employeeDataSource,
            dataTextField: "EMPLOYEE_EDESC",
            dataValueField: "EMPLOYEE_CODE",
            optionLabel: "All",
            filter: "contains",
            dataBound: function () {
                //var dataSource = this.dataSource;
                //var data = dataSource.data();

                //if (!this._adding) {
                //    this._adding = true;

                //    data.splice(0, 0, {
                //        "EMPLOYEE_EDESC": "All",
                //        "EMPLOYEE_CODE": ""
                //    });


                //    this._adding = false;
                //}

                //    var val = $rootScope.SelectedEmployeeCodeOnChange;
                //  //  $rootScope.BackEmployeeCode = val;
                //    if (val === "") {
                //        $("#employees").data("kendoDropDownList").value("");
                //    } else {
                //        $("#employees").data("kendoDropDownList").value(val);
                //    }

            }





        };
    }


    $scope.divisionDataSource = {
        type: "json",
        serverFiltering: false,
        transport: {
            read: {
                dataType: "json",
                url: "/api/SubPlanApi/GetChiledLevelDivision",
            },
            parameterMap: function (data, action) {
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        var newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        var newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    var newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        }
    };

    $scope.divisionOptions = {
        dataSource: $scope.divisionDataSource,
        dataTextField: "DIVISION_EDESC",
        dataValueField: "DIVISION_CODE",
        optionLabel: "All",
        filter: "contains",
        dataBound: function () {
            //var dataSource = this.dataSource;
            //var data = dataSource.data();

            //if (!this._adding) {
            //    this._adding = true;
            //    data.splice(0, 0, {
            //        "DIVISION_EDESC": "All",
            //        "DIVISION_CODE": ""
            //    });
            //    this._adding = false;
            //}
            //var val = $rootScope.SelectedDivisionCodeOnChange;
            //$rootScope.BackDivisionCode = val;
            //if (val === "") {
            //    $("#divisions").data("kendoDropDownList").value("");
            //} else {
            //    $("#divisions").data("kendoDropDownList").value(val);
            //}

        }


    };

    $scope.gotoBack = function () {
        $rootScope.reservedData;
        var fromRef = "";
        if (confirm("All input data will lost, Are you sure to go back?")) {
            if ($scope.planName != undefined && $scope.ref != undefined) {
                fromRef = "?planName=" + $scope.planName + "&ref=" + $scope.ref;
            }
            if ($routeParams.plancode != '' && $routeParams.plancode != undefined && $routeParams.plancode != 'undefined') {
                if (fromRef != "")
                    window.location = "/Planning/Home/Index#!Planning/CreateCollectionPlan/" + $routeParams.plancode + fromRef;
                else
                    window.location = "/Planning/Home/Index#!Planning/CreateCollectionPlan/" + $routeParams.plancode;
            }
            else {
                if (fromRef != "")
                    window.location = "/Planning/Home/Index#!Planning/CreateCollectionPlan" + fromRef;
                else
                    window.location = "/Planning/Home/Index#!Planning/CreateCollectionPlan";

            }
        }
    }

    $scope.cancelForm = function () {
        window.location.href = "/Planning/Home/Setup#!Planning/CollectionPlan";
    }
});
