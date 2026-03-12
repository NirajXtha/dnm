/// <reference path="~/Scripts/_references.js" />
/// <reference path="../module/planningModule.js" />

planningModule.controller('EmployeeRoutePlan', function ($scope, $routeParams, $timeout, EmployeeRoutePlanService) {
    $scope.pageName = "Employee Route Plan Index";
    $scope.routeCode = $routeParams.routeCode;

    $scope.routePlanDetail = {
        plancode: "",
        planname: "",
        startdate: "",
        enddate: "",
        freequency: ""
    },
        $scope.frequencyWiseUserAssign = "yes";
    $scope.addRouteInPlan = false;
    $scope.toogleAddRouteInPlan = function () {
        if ($scope.addRouteInPlan)
            $scope.addRouteInPlan = false;
        else
            $scope.addRouteInPlan = true;
    }
    $scope.customfrequencyday = 1;
    if ($scope.routePlanDetail.freequency == "DAILY") {
        $scope.customfrequencyday = 1;
    }
    else if ($scope.routePlanDetail.freequency == "WEEK") {
        $scope.customfrequencyday = 7;
    }
    else if ($scope.routePlanDetail.freequency == "MONTH") {
        $scope.customfrequencyday = 30;
    }

    $scope.cancelSaveRouteInPlan = function () {
        $scope.addRouteInPlan = false;
    }
    
    $scope.routesDataSource = {
        type: "jsonp",
        serverFiltering: false,
        transport: {
            read: {
                url: "/api/DistributionPlaningApi/GetRouteList?plancode=",
            }
        },
    };

    $scope.empGroupsDataSource = {
        type: "jsonp",
        serverFiltering: false,
        transport: {
            read: {
                url: "/api/DistributionPlaningApi/GetEmpGroupList",
            }
        },
    };
    $scope.employeeOnChange = function (e) {

        var selectedValues = e.sender.value();
        var s = $scope.selectedEmployees;
        var element_id = e.sender.tagList[0].id.replace('_taglist', '');
        var multiselect = $("#" + element_id).data("kendoMultiSelect");

        var partElement = element_id.split('_');
        var el = moment(partElement[0]).format('YYYY-MM-DD');

        //var autoFillValue = $('input[name=frequencyWiseUserAssign]:checked').val();
        var autoFillValue = $('input#frequencyWiseUserAssign').is(':checked');
        if (autoFillValue) {
            autoFillValue = "yes";
        }
        else {
            autoFillValue = "no";
        }
        var incresedEl = null;
        var incresedElArray = [];
        $scope.isDefaultFreq = true;
        var frequency = $scope.routePlanDetail.freequency;
        var choosedDayfreq = parseInt($('#customday').val());
        $scope.customfrequencyday = choosedDayfreq;
        if ($scope.customfrequencyday == 1 || $scope.customfrequencyday == 7 || $scope.customfrequencyday == 30) {
            $scope.isDefaultFreq = true;
        }
        else {
            $scope.isDefaultFreq = false;
        }
        if (!$scope.isDefaultFreq && autoFillValue === "yes") {
            var currDate = moment(el).toArray();
            var endDate = moment($scope.routePlanDetail.enddate).toArray();
            var a = moment([currDate[0], currDate[1], currDate[2]]);
            var b = moment([endDate[0], endDate[1], endDate[2]]);
            var daysDiff = b.diff(a, 'days');
            for (i = 0; i <= daysDiff; i = i + $scope.customfrequencyday) {
                incresedEl = moment(el).add({ days: i });
                incresedElArray.push(incresedEl);
            }
        }

        if (frequency == "DAILY" && autoFillValue === "yes" && $scope.isDefaultFreq) {
            //incresedEl = moment(el).add({ days: 1 });
            var currDate = moment(el).toArray();
            var endDate = moment($scope.routePlanDetail.enddate).toArray();
            //var remainingDay = moment(currDate).add({ days: 1 });
            var a = moment([currDate[0], currDate[1], currDate[2]]);
            var b = moment([endDate[0], endDate[1], endDate[2]]);
            var daysDiff = b.diff(a, 'days');
            for (i = 1; i <= daysDiff; i++) {
                incresedEl = moment(el).add({ days: i });
                incresedElArray.push(incresedEl);
            }
        }
        else if (frequency == "WEEK" && autoFillValue === "yes" && $scope.isDefaultFreq) {
            $scope.customfrequencyday = 7;
            var currDate = moment(el).toArray();
            var endDate = moment($scope.routePlanDetail.enddate).toArray();
            //var remainingDay = moment(currDate).add({ days: 7 });
            var a = moment([currDate[0], currDate[1], currDate[2]]);
            var b = moment([endDate[0], endDate[1], endDate[2]]);
            var daysDiff = b.diff(a, 'days');
            for (i = 7; i <= daysDiff; i = i + $scope.customfrequencyday) {
                incresedEl = moment(el).add({ days: i });
                incresedElArray.push(incresedEl);
            }

            //incresedEl = moment(el).add({ days: 7 });
            //incresedElArray.push(incresedEl);
        }
        else if (frequency == "MONTH" && autoFillValue === "yes" && $scope.isDefaultFreq) {
            var currentDate = moment(el);
            //var futureMonth = moment(currentDate).add(1, 'M');
            //var futureMonthEnd = moment(futureMonth).endOf('month');

            //if (currentDate.date() != futureMonth.date() && futureMonth.isSame(futureMonthEnd.format('YYYY-MM-DD'))) {
            //    futureMonth = futureMonth.add(1, 'd');
            //}
            //incresedEl = currentDate;

            var currDate = moment(el).toArray();
            var endDate = moment($scope.routePlanDetail.enddate).toArray();
            var a = moment([currDate[0], currDate[1], currDate[2]]);
            var b = moment([endDate[0], endDate[1], endDate[2]]);
            var daysMonth = b.diff(a, 'month');
            for (i = 1; i <= daysMonth; i++) {
                var futureMonth = moment(currentDate).add(i, 'M');
                var futureMonthEnd = moment(futureMonth).endOf('month');
                if (currentDate.date() != futureMonth.date() && futureMonth.isSame(futureMonthEnd.format('YYYY-MM-DD'))) {
                    futureMonth = futureMonth.add(1, 'd');
                }
                incresedEl = futureMonth;
                incresedElArray.push(incresedEl);
            }
        }

        if (incresedElArray.length > 0) {
            $.each(incresedElArray, function (i, v) {
                var newDate = v
                if (newDate != null) {
                    newDate = moment(newDate).format('YYYY-MM-DD').toUpperCase();
                    partElement[0] = newDate;
                    var id = partElement.join('_');

                    var empData = multiselect.dataSource.data();
                    var dataSource = new kendo.data.DataSource({
                        data: empData
                    });
                    var multiSelectToSet = $("#" + id).data("kendoMultiSelect");
                    if (multiSelectToSet != undefined) {
                        multiSelectToSet.setDataSource(dataSource);
                        multiSelectToSet.value(selectedValues);
                    }
                }
            })
        }
        //if (incresedEl != null) {
        //    incresedEl = moment(incresedEl).format('DD-MMM-YY').toUpperCase();
        //    partElement[0] = incresedEl;
        //    var id = partElement.join('_');

        //    var empData = multiselect.dataSource.data();
        //    var dataSource = new kendo.data.DataSource({
        //        data: empData
        //    });
        //    var multiSelectToSet = $("#" + id).data("kendoMultiSelect");
        //    if (multiSelectToSet != undefined) {
        //        multiSelectToSet.setDataSource(dataSource);
        //        multiSelectToSet.value(selectedValues);
        //    }
        //}
    }
    $scope.selectedEmployees = [];
    $scope.routes = [];
    $scope.dates = [];
    //$scope.employeeNames = [];
    $scope.employeeData = [];

    $scope.employeeOptions = {
        dataTextField: "EMPLOYEE_EDESC",
        dataValueField: "EMPLOYEE_CODE",
        //valuePrimitive: true,
        //serverFiltering:true,
        autoBind: false,
        dataSource: {
            type: "json",
            serverFiltering: true,
            transport: {
                read: {
                    url: "/api/DistributionPlaningApi/GetEmployees",
                },
                parameterMap: function (data, action) {

                    if (data.filter != undefined) {
                        
                        if (data.filter.filters[0] != undefined) {
                            var newParams = {
                                filter: data.filter.filters[0].value,
                                empGroup: $("#groups").data("kendoDropDownList").dataItem().GROUPID
                            };
                            return newParams;
                        }
                        else {
                            var newParams = {
                                filter: "",
                                empGroup: $("#groups").data("kendoDropDownList").dataItem().GROUPID
                            };
                            return newParams;
                        }
                    }
                    else {
                        var newParams = {
                            filter: planCode,
                            empGroup: $("#groups").data("kendoDropDownList").dataItem().GROUPID
                        };
                        return newParams;
                    }
                }
            }
            //data: $scope.employeeData,
        },
        change: $scope.employeeOnChange,
        select: $scope.employeeOnSelect,
        dataBound: function () {
            
            $(".routeMultiselectElement").off('click');
            $(".routeMultiselectElement").on('click', function () {
                //$($(this).find('select')).data('kendoMultiSelect').dataSource.read();
                $("#groups").attr('readonly', true);
            })
        }
    };

    $scope.planSelected = false;
    $scope.onChange = function (e) {
        showloader();
        $("#groups").attr('readonly', false);
        var selectedValue = $("#routes").data("kendoDropDownList").value();
        SetRouteAndDetailFor(selectedValue);
    }
    function SetRouteAndDetailFor(selectedValue) {
        if (selectedValue != "" || selectedValue != undefined) {
            $scope.planSelected = true;
            EmployeeRoutePlanService.getRoutePlanInfo(selectedValue).then(function (result) {
                $scope.routePlanDetail.plancode = selectedValue;
                $scope.routePlanDetail.planname = result.data[0].PLAN_EDESC;
                $scope.routePlanDetail.startdate = moment(result.data[0].START_DATE).format('YYYY-MM-DD');
                $scope.routePlanDetail.enddate = moment(result.data[0].END_DATE).format('YYYY-MM-DD');
                $scope.routePlanDetail.freequency = result.data[0].TIME_FRAME_EDESC;
                $scope.routePlanDetail.nstartdate = AD2BS(moment(result.data[0].START_DATE).format('YYYY-MM-DD'));
                $scope.routePlanDetail.nenddate = AD2BS(moment(result.data[0].END_DATE).format('YYYY-MM-DD'));
            });
            EmployeeRoutePlanService.getRoutesFilterPlanCode(selectedValue).then(function (result) {
                $scope.routes = result.data;
                renderColumn(selectedValue);
            }, function (error) {

            });

        }
    }
    $scope.removeRouteFromPlan = function (rc) {
        $scope.routePlanDetail.plancode;
        var deleteConfirm = confirm("Are you sure to remove this route from plan?\nThis action delete record permanently.");
        if (deleteConfirm) {
            showloader();
            EmployeeRoutePlanService.removeRouteFromPlan($scope.routePlanDetail.plancode, rc).then(function (result) {
                if (result.data == 'success') {
                    $scope.onChange();
                    displayPopupNotification("Route deletion successed.", "success");
                    hideloader();
                }
                else {
                    displayPopupNotification("Route deletion failed." + result.data, "error");
                    hideloader();
                }
            }, function (err) {
                displayPopupNotification("Route deletion encountered error.", "error");
                hideloader();
            })
        }
    }
    function renderColumn(selectedValue) {
        EmployeeRoutePlanService.getDates(selectedValue).then(function (result) {
            $scope.dates = result.data;
            renderGrid();
        }, function (error) {

        });
    }
    function renderGrid() {
        var columns = [{
            field: "ROUTE_NAME",
            title: "Route Name",
            template: " <a href='javascript:void(0);' ng-click='removeRouteFromPlan(\"#=ROUTE_CODE#\")' title='Delete this route from plan.'><i class='fa fa-trash text-danger'></i></a> #= ROUTE_NAME #",
            width: "120px",
            locked: true,
        },];
        var tempYear = "", tempMonth = "";
        var dateColumnt = [];
        $scope.assignedDateValues = null;
        var d1 = $.Deferred();
        EmployeeRoutePlanService.fetchAssignedRouteEmployeeData($scope.routePlanDetail.plancode).then(function (result) {

            $scope.assignedDateValues = result.data;
            d1.resolve($scope.assignedDateValues);
        }, function (err) { });
        $.when(d1).then(function () {
            $.each($scope.dates, function (key, value) {
                var dayTitle = moment(BS2AD(value.DATES)).format('ddd');
                if (value.YEAR != tempYear && value.MONTH != tempMonth) {
                    columns.push({
                        title: value.YEAR,
                        columns: [{
                            title: value.MONTH_NAME,
                            columns: [{
                                field: value.DAY,
                                title: value.DAY + ' ' + dayTitle,
                                template: "#= renderInput('" + value.DATES + "',ROUTE_CODE) #",
                                width: 200
                            }]
                        }]
                    });
                    tempYear = value.YEAR;
                    tempMonth = value.MONTH;
                }
                else if (value.YEAR == tempYear && value.MONTH == tempMonth) {
                    var yearcolumn = columns;
                    var monthcolumn = yearcolumn[(yearcolumn.length - 1)].columns;
                    monthcolumn[(monthcolumn.length - 1)].columns.push({
                        field: value.DAY,
                        title: value.DAY + ' ' + dayTitle,
                        template: "#= renderInput('" + value.DATES + "',ROUTE_CODE) #",
                        width: 200
                    })
                }
                else if (value.YEAR == tempYear && value.MONTH != tempMonth) {
                    var yearcolumn = columns;
                    yearcolumn[(yearcolumn.length - 1)].columns.push({
                        title: value.MONTH_NAME,
                        columns: [{
                            field: value.DAY,
                            title: value.DAY + ' ' + dayTitle,
                            template: "#= renderInput('" + value.DATES + "',ROUTE_CODE) #",
                            width: 200
                        },]
                    });
                    tempMonth = value.MONTH;
                }
            });

            $scope.employeeRouteGrid = {
                dataSource: {
                    data: $scope.routes
                },
                sortable: false,
                pageable: false,
                columns: columns,
                height: 550,
                dataBound: function () {
                    $scope.assignedDateValues;
                    if ($scope.assignedDateValues != null && $scope.assignedDateValues.length > 0) {
                        var d2 = $.Deferred();
                        var employeeData = [];
                        
                        var empGroupID = "";
                        //if ($("#groups").data("kendoDropDownList").dataItem().GROUPID != undefined)
                        //    empGroupID = $("#groups").data("kendoDropDownList").dataItem().GROUPID; //$scope.empGroupsDataSource.get($scope.selectedEmpGroup);
                        EmployeeRoutePlanService.getEmployees(empGroupID).then(function (result) {
                            employeeData = result.data;
                            d2.resolve(result);
                        })

                        $.when(d2).then(function () {
                            var inputFieldList = $('select.routeMultiselectElement');
                            
                            $.each(inputFieldList, function (i, v) {
                                var id = v.id.split('_route_');
                                var date = id[0];
                                var routeCode = id[1];
                                var employees = $.grep($scope.assignedDateValues, function (v, i) {
                                    return moment(v.ASSIGN_DATE).format('YYYY-MM-DD').toUpperCase() == date && v.ROUTE_CODE == routeCode;
                                })
                                if (employees.length > 0) {
                                    var selectedEmployees = [];
                                    $.each(employees, function (k, val) {
                                        selectedEmployees.push(val.EMP_CODE);
                                    });

                                    var multiSelectToSet = $("#" + v.id).data("kendoMultiSelect");
                                    if (multiSelectToSet != undefined) {
                                        var dataSource_Selection = new kendo.data.DataSource({
                                            data: employeeData
                                        });
                                        multiSelectToSet.setDataSource(dataSource_Selection);
                                        multiSelectToSet.value(selectedEmployees);
                                    }
                                }

                            })
                            hideloader();
                        })
                    }
                    else {
                        hideloader();
                    }
                }
            };
        })
    }



    $scope.empGroupOptions = {
        optionLabel: "All Group",
        filter: "contains",
        dataTextField: "GROUP_EDESC",
        dataValueField: "GROUPID",
        dataSource: $scope.empGroupsDataSource,
        //change: function () {
        //    var first = $(".routeMultiselectElement").find('select');
        //    $.each(first, function (i, obj) {
        //        obj = $(obj);
        //        if (!_.isEmpty(obj.data('kendoMultiSelect'))) {
        //            if (obj.data('kendoMultiSelect') != undefined)
        //                if (obj.data('kendoMultiSelect').dataSource != undefined)
        //                    obj.data('kendoMultiSelect').dataSource.read();
        //        }
        //    });
        //    //if ($($(".routeMultiselectElement").find('select')).data('kendoMultiSelect') != undefined)
        
        //    //    $($(".routeMultiselectElement").find('select')).data('kendoMultiSelect').dataSource.read();
        //   // $scope.employeeOptions.DataSource.read();
        //},
        dataBound: function () {

        }
    };
    $scope.routesOptions = {
        optionLabel: "--Select Route--",
        //filter: "contains",
        filter: "startswith",
        dataTextField: "PLAN_EDESC",
        dataValueField: "PLAN_CODE",
        dataSource: $scope.routesDataSource,
        change: $scope.onChange,
        dataBound: function () {
            if ($scope.routeCode != '' && $scope.routeCode != null) {

                var routesddl = $("#routes").data("kendoDropDownList");
                routesddl.value([$scope.routeCode]);
                SetRouteAndDetailFor($scope.routeCode);
                $scope.routeCode = '';
            }
        }
    };

    $scope.selectRouteToAddInPlan = {
        placeholder: "Select Routes...",
        dataTextField: "ROUTE_NAME",
        dataValueField: "ROUTE_CODE",
        valuePrimitive: true,
        autoBind: false,
        dataSource: {
            type: "jsonp",
            serverFiltering: false,
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/DistributionPlaningApi/GetAllRouteByFilters",
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
        }
    };
    $scope.selectedRouteIdsToAddInPlan = [];

    $scope.SaveRouteInPlan = function () {

        var selectedPlanCode = $scope.routePlanDetail.plancode;
        $scope.selectedRouteIdsToAddInPlan;
        var choosedRouteToAdd = $('#selectRouteToAddInPlan').data('kendoMultiSelect').value();
        if (choosedRouteToAdd.length > 0) {
            showloader();
            EmployeeRoutePlanService.SaveNewRouteToPlan(selectedPlanCode, choosedRouteToAdd).then(function (result) {
                $scope.onChange();
                if (result.data.trim() == "success") {
                    displayPopupNotification("Route added to the plan.", "success");
                }
                if (result.data.includes("success") && result.data.trim() != "success") {
                    displayPopupNotification(result.data + " other route saved", "info");
                }
                if (!result.data.includes("success")) {
                    displayPopupNotification(result.data, "warning");
                }

                hideloader();
            }, function (err) {
                displayPopupNotification("Action encountered error." + statusText, "error");
                hideloader();
            });
        }
        else {
            $('#selectRouteToAddInPlan').focus();
            displayPopupNotification("Please select routes first.", "error");
        }

    }

    $scope.SaveEmployeeRoutePlan = function (selectedEmployees, b, c, scopes) {
        
        showloader();
        var formData = $('#employeeDataForm').serialize();
        EmployeeRoutePlanService.SaveEmployeeRoutePlanData(formData).then(function (result) {
            if (result.data === "success") {
                hideloader();
                displayPopupNotification("Employee to route assignment has done successfully.", "success");
            }
            else {
                displayPopupNotification("Error in route assignment to employee.", "error");
            }
        }, function (error) {
            displayPopupNotification("Error in route assignment to employee." + statusText, "error");
        });
        hideloader();
    }

    $scope.cancelEmployeeRoutePlan = function () {
        window.location = "/Planning/DistributionPlaning/Index#!Planning/RouteList";
    }
});

planningModule.factory('EmployeeRoutePlanService', function ($http) {
    var fac = {};
    fac.getDates = function (plancode) {
        return $http.get("/api/DistributionPlaningApi/GetPlanDates?plancode=" + plancode);
    }
    fac.getRoutesFilterPlanCode = function (plancode) {
        return $http.get("/api/DistributionPlaningApi/GetRouteByRouteCode?PLAN_ROUTE_CODE=" + plancode);
    }
    fac.SaveEmployeeRoutePlanData = function (requestData) {
        return $http.post("/Planning/DistributionPlaning/SaveEmployeeRoutePlanData", requestData, {
            headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset-UTF-8' },
        })
    }
    fac.getRoutePlanInfo = function (plancode) {
        return $http.get("/api/DistributionPlaningApi/GetRouteList?plancode=" + plancode);
    }
    fac.removeRouteFromPlan = function (plancode, routecode) {
        return $http.get("/api/DistributionPlaningApi/removeRouteFromPlan?plancode=" + plancode + "&routecode=" + routecode);
    }
    fac.getEmployees = function (empGroupId) {
        return $http.get("/api/DistributionPlaningApi/GetEmployees?filter=&&empGroup=" + empGroupId);
    }
    fac.fetchAssignedRouteEmployeeData = function (plancode) {
        return $http.get("/api/DistributionPlaningApi/GetAssignedEmployeesOfRoute?plancode=" + plancode);
    }
    fac.SaveNewRouteToPlan = function (plancode, selectedRoute) {
        var data = { plancode: plancode, routecode: selectedRoute };
        return $http.get("/api/DistributionPlaningApi/addRoutesToPlan?plancode=" + plancode + "&routecode=" + selectedRoute.join(','));
        //return $http({
        //    url: "/api/DistributionPlaningApi/addRoutesToPlan",
        //    method: 'GET',
        //    data: JSON.stringify(data)
        //});
    }
    return fac;
});