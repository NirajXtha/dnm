/// <reference path="~/Scripts/_references.js" />
/// <reference path="../module/planningModule.js" />
planningModule.controller('planningCopyCtrl', function ($scope, planservice, $routeParams, $rootScope) {
        $scope.param = $routeParams.plancode;
    $rootScope.reservedData;
    tempValue = $rootScope.reservedData;;
    $scope.next = true;
    $scope.saveContinue = true;
    DateFilter.init();
    

    $scope.pageName = "Copy Plan";
    $scope.planName = "plan name copied";
    $scope.progress = 'style="5%"';
    $scope.startdate = [];
    function IsFormValid() {
        var message = "";
        var pnameSelect = $('#planName').val();
        var frequencySelected = $('#frequency').val();
        var plantypeSelected = $('#plantype').val();
        var planforSelected = $('#planfor').val();
        var IsCustomerSelected = $('input[name=customer_product_option]:checked').val();
        var customerSelected = $('#customers').val();
        var productMultiselect = $("#productMultiselect").data("kendoMultiSelect");
        var count = productMultiselect.value().length;

        if (pnameSelect == "") {
            return false;
        }
        else if (frequencySelected == "") {
            return false;
        }
        else if (planforSelected == "") {
            return false;
        }
        else if (IsCustomerSelected == "customer_product" && customerSelected == "") {
            return false;
        }
        else if (message != "") {
            return false;
        }
        else if (count < 1) {
            return false;
        }
        else {
            return true;
        }
    }
    $scope.gotoPlanSetup = function () {
        var Isvalid = IsFormValid();
        if (Isvalid) {
            //window.location.href = "/Planning/Home/Index#!Planning/PlanSetup/" + $scope.param;
            window.location.href = "/Planning/Home/Index#!Planning/PlanSetup";
        }
        //window.location.href = "/Planning/Home/Index#!Planning/";
    }
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
    $scope.DateFormatChange = function () {

    }

    if (tempValue != undefined) {
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
            setTimeout(function () {
                if (tempValue.multiselectValue != undefined) {
                   var productMultiselect =  $("#productMultiselect").data("kendoMultiSelect");
                    productMultiselect.value(tempValue.multiselectValue);
                }
                var productTreeView = $('#productTreeView').data('kendoTreeView');
                $.each(tempValue.checkeOnlyDataTemp, function (i, v) {
                    var item = productTreeView.findByText(v.ITEM_EDESC);
                    
                    if (item.length > 0) {
                        productTreeView.dataItem(item).set("checked", true);
                    }
                });
                _.each(tempValue.selectedItemsList, function (i, v) {
                     productTreeView.expandPath([i.ITEM_CODE]);
                })
            }, 3000);

        }, 1000);
        $scope.next = true;
        $scope.saveContinue = false;
        $scope.customervalue = tempValue.customerCode;
        $scope.branchvalue = tempValue.branchcode;
        $scope.employeevalue = tempValue.employeecode;
        $scope.divisionvalue = tempValue.divisioncode;
        $scope.dateFormat = tempValue.dateFormat;

        //$("#customers").data('kendoDropDownList').value(tempValue.customerCode);
    }

    $scope.$on('$viewcontentloaded', function () {
        $('#submit_form').bootstrapwizard({
            'ontabshow': function (tab, navigation, index) {
                
                var $total = navigation.find('li').length;
                var $current = index + 1;
                var $percent = ($current / $total) * 100;
                $('#submit_form').find('.bar').css({ width: $percent + '%' });
                $scope.progress = $percent + '%';
            }
        });

    });
    $rootScope.reservedData = null;
});

