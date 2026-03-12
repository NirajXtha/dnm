/// <reference path="~/Scripts/_references.js" />
/// <reference path="../module/planningModule.js" />

planningModule.controller('editPlanCtrl', function ($scope, planservice, $routeParams, $rootScope) {
    
    $scope.param = $routeParams.plancode;
    $rootScope.reservedData;
    tempValue = $rootScope.reservedData;
    DateFilter.init();
    $scope.pageName = "Edit Plan";
    $scope.planName = '';
    $rootScope.reservedData = null;

    $scope.IsCustomerProduct = true;
    $scope.customer_product_option = "customer_product";
    $scope.CustomerProductOption = function () {
        
        if ($scope.customer_product_option == 'customer_product') {
            $scope.IsCustomerProduct = true;
            return;
        }
        $scope.IsCustomerProduct = false;
    }

    $scope.branchDataSource = {
        type: "json",
        serverFiltering: false,
        transport: {
            read: {
                dataType: "json",
                url: "/api/SubPlanApi/GetChiledLevelBranch",
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
    $scope.branchOptions = {
        dataSource: $scope.branchDataSource,
        dataTextField: "BRANCH_EDESC",
        dataValueField: "BRANCH_CODE",
        optionLabel: "--Select Branch--",
        filter: "contains",
    };
    $scope.branchvalue = null;

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
        optionLabel: "--Select Employee--",
        filter: "contains",
    };
    $scope.employeevalue = null;


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
        optionLabel: "--Select Division--",
        filter: "contains",
    };
    $scope.divisionvalue = null;

    $scope.customersDataSource = {
        type: "json",
        serverFiltering: false,
        transport: {
            read: {
                dataType: "json",
                url: "/api/SubPlanApi/GetChiledLevelCustomers",
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

    $scope.customerOptions = {
        dataSource: $scope.customersDataSource,
        dataTextField: "CUSTOMER_EDESC",
        dataValueField: "CUSTOMER_CODE",
        optionLabel: "--Select Customer--",
        filter: "contains",
    };
    $scope.customervalue = null;

    $scope.dateFormat = 'AD';

    
    if (tempValue != undefined) {
        
        $scope.plancode = tempValue.PLAN_CODE;
        $scope.planName = tempValue.PLAN_EDESC;
        $scope.frequency = tempValue.TIME_FRAME_CODE;
        $scope.plantype = tempValue.PLAN_TYPE;
        $scope.planfor = tempValue.PLAN_FOR;
        $("#frequency").data("kendoDropDownList").value(tempValue.TIME_FRAME_CODE);
        $("#plantype").data("kendoDropDownList").value(tempValue.PLAN_TYPE);
        $("#planfor").data("kendoDropDownList").value(tempValue.PLAN_FOR);
        setTimeout(function () {
            $("#ToDateVoucher").val(moment(tempValue.END_DATE).format("YYYY-MMM-DD"));
            $("#FromDateVoucher").val(moment(tempValue.START_DATE).format("YYYY-MMM-DD"));
            $("#FromDateVoucher").trigger("change");
            $("#ToDateVoucher").trigger("change");
            setTimeout(function () {
                if (tempValue.dateFilter != "")
                    $(".dateFilterSelect").val(tempValue.dateFilter);
            }, 1000)
        }, 1000);
        $scope.next = true;
        $scope.saveContinue = false;
        $scope.customervalue = tempValue.customerCode;
        $scope.branchvalue = tempValue.branchcode;
        $scope.employeevalue = tempValue.employeecode;
        $scope.divisionvalue = tempValue.divisioncode;
        $scope.dateFormat = tempValue.dateFormat;
        //
        setTimeout(function () {
            var productTreeView = $('#productTreeview').data('kendoTreeView');
            $.each(tempValue.selectedItemsList, function (i, v) {
                productTreeView.expandPath([v.ITEM_CODE]);
            })
            $.each(tempValue.selectedItemsList, function (i, v) {
                
                var itemName = v.ITEM_EDESC;
                var item = productTreeView.findByText(itemName);
                if (item.length > 0) {
                    if (v.GROUP_SKU_FLAG == 'G') {
                        productTreeView.dataItem(item).set("dirty", true);
                    }
                    else if (v.GROUP_SKU_FLAG == 'I') {
                        productTreeView.dataItem(item).set("checked", true);
                    }
                }
            });
            hideloader();
        }, 2000);
        //$("#customers").data('kendoDropDownList').value(tempValue.customerCode);
    }
    if ($scope.customer_product_option == 'product_only') {
        $scope.IsCustomerProduct = false;
    }
    
});