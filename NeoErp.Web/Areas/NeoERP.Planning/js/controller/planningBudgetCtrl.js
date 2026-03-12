/// <reference path="~/Scripts/_references.js" />
/// <reference path="../module/planningModule.js" />

planningModule.controller('planningBudgetCtrl', function ($scope, planservice, $routeParams, $rootScope, $http) {

    $scope.param = $routeParams.plancode;
    $scope.planName = $routeParams.planName;
    //$rootScope.reservedBudgetData;
    tempValue = $rootScope.reservedBudgetData;
    $rootScope.reservedBudgetData;
    $scope.next = true;
    $scope.divisionFlag = false;
    $scope.branchFlag = '';
    $scope.employeeFlag = '';
    $scope.customerFlag = '';
    $scope.divisionFlag = '';
    $scope.saveContinue = true;
    getDivisionFlagByPreferenceSetup();
    getFlagFromPreferenceSetup();

    function getDivisionFlagByPreferenceSetup() {
        var url = window.location.protocol + "//" + window.location.host + "/api/PlanApi/GetDivisionFlag";
        var req = {
            method: 'GET',
            url: url,
        }
        $http(req).then(function (data, status, headers, config) {
            if (data.data.DATA == "Y") {
                $scope.divisionFlag = true;
            }
            else {
                $scope.divisionFlag = false;
            }
        }, function (data, status, headers, config) {
        });
    };
    function getFlagFromPreferenceSetup() {
        var url = window.location.protocol + "//" + window.location.host + "/api/PlanApi/GetbudgetPreferenceSetupFlag";
        var req = {
            method: 'GET',
            url: url,
        }
        $http(req).then(function (data, status, headers, config) {
            $scope.branchFlag = data.data.DATA[0].SHOW_BRANCH;
            $scope.employeeFlag = data.data.DATA[0].SHOW_EMPLOYEE;
            $scope.customerFlag = data.data.DATA[0].SHOW_CUSTOMER;
            $scope.itemflag = data.data.DATA[0].SHOW_ITEM;
            $scope.divisionFlag = data.data.DATA[0].SHOW_DIVISION;
            
            if ($scope.branchFlag == 'Y')
                $scope.bindBranchOption();
            if ($scope.employeeFlag == 'Y')
                $scope.bindEmployeeOption();
            if ($scope.divisionFlag == 'Y')
                $scope.bindDivisionOption();
            if ($scope.itemflag != "Y") {
                hideloader();
            }


        }, function (data, status, headers, config) {
        });
    };
    $scope.pageName = "New Ledger Plan";
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
        //var count = productMultiselect.value().length;

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
            //else if (count < 1) {
            //    return false;
            //}
        else {
            return true;
        }
    }
    $.when(nextDiffer).then(function (resolved) {
        var Isvalid = IsFormValid();
        if (Isvalid) {
            var itemLength = 0
            if (tempValue != undefined) {
                tempValue.checkeOnlyDataTemp = _.uniq(tempValue.checkeOnlyDataTemp, function (d) { return d.ITEM_CODE });
                itemLength = tempValue.checkeOnlyDataTemp.length;
                if (checkedNodeOnly.length > 0) {
                    checkedNodeOnly = _.uniq(checkedNodeOnly, function (d) { return d.ITEM_CODE });
                    itemLength = checkedNodeOnly.length;
                }
            }
            else if ($scope.param != '' && (tempValue == undefined || tempValue == 'undefined')) {
                checkedNodeOnly = _.uniq(checkedNodeOnly, function (d) { return d.ITEM_CODE });
                itemLength = checkedNodeOnly.length;
            }

            //productSelectionLimit = (productSelectionLimit == '' ? 100 : productSelectionLimit);
            //productSelectionLimit = '';
            productSelectionLimit = (productSelectionLimit == '' ? 100 : productSelectionLimit);
            if (itemLength < parseInt(productSelectionLimit)) {
                if ($scope.param != '' && $scope.param != undefined && $scope.param != 'undefined') {
                    window.location.href = "/Planning/Home/Index#!Planning/BudgetPlanSetup/" + $scope.param;
                } else {
                    window.location.href = "/Planning/Home/Index#!Planning/BudgetPlanSetup";
                }
            }
            else {                
                displayPopupNotification("You have selected more than " + productSelectionLimit + " (i.e " + itemLength + ") item. Please limit your item selection below " + productSelectionLimit + ".", "warning");
                hideloader();
            }
        }
    });
    $scope.gotoBudgetPlanSetup = function () {

        //tempValue = $rootScope.reservedBudgetData;
        //var Isvalid = IsFormValid();

        //if (Isvalid) {
        //    var itemLength = 0
        //    if (tempValue != undefined) {
        //        tempValue.checkeOnlyDataTemp = _.uniq(tempValue.checkeOnlyDataTemp, function (d) { return d.ITEM_CODE });
        //        itemLength = tempValue.checkeOnlyDataTemp.length;
        //        if (checkedNodeOnly.length > 0) {
        //            checkedNodeOnly = _.uniq(checkedNodeOnly, function (d) { return d.ITEM_CODE });
        //            itemLength = checkedNodeOnly.length;
        //        }
        //    }
        //    else if ($scope.param != '' && (tempValue == undefined || tempValue == 'undefined')) {
        //        checkedNodeOnly = _.uniq(checkedNodeOnly, function (d) { return d.ITEM_CODE });
        //        itemLength = checkedNodeOnly.length;
        //    }
        //    productSelectionLimit = (productSelectionLimit == '' ? 100 : productSelectionLimit);

        //    if (itemLength < parseInt(productSelectionLimit)) {
        //        if ($scope.param != '' && $scope.param != undefined && $scope.param != 'undefined') {
        //            window.location.href = "/Planning/Home/Index#!Planning/BudgetPlanSetup/" + $scope.param;
        //        } else {
        //            window.location.href = "/Planning/Home/Index#!Planning/BudgetPlanSetup";
        //        }
        //    }
        //    else {
        //        displayPopupNotification("You have selected more than " + productSelectionLimit + " (i.e " + itemLength + ") item. Please limit your item selection below " + productSelectionLimit + ".", "warning");
        //        hideloader();
        //    }
        //}
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
    $scope.bindBranchOption = function () {
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
    }
    $scope.branchvalue = null;

    
    $scope.bindDivisionOption = function () {
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
            //dataBound: function () {
            //    if ($rootScope.BackDivisionCode !== "" || $rootScope.BackDivisionCode !== undefined) {
            //        var val = $rootScope.BackDivisionCode;
            //        $("#divisions").data("kendoDropDownList").value(val);
            //    }

            //}
        };
    }
    $scope.divisionvalue = null;

    
    $scope.dateFormat = 'BS';
    $scope.DateFormatChange = function () {

    }

    $scope.GetPlanDetailValueByPlanCode_returnResult = null;
    var tempValue_deffer = $.Deferred();
    var paramPreset_deffer = $.Deferred();
    if (tempValue != undefined) {

        $scope.planName = tempValue.PLAN_EDESC;
        $scope.frequency = tempValue.TIME_FRAME_CODE;
        //$scope.plantype = tempValue.PLAN_TYPE;
        //$scope.planfor = tempValue.PLAN_FOR;
        $("#frequency").data("kendoDropDownList").value(tempValue.TIME_FRAME_CODE);
        //$("#plantype").data("kendoDropDownList").value(tempValue.PLAN_TYPE);
        //$("#planfor").data("kendoDropDownList").value(tempValue.PLAN_FOR);
        setTimeout(function () {
            //$(".salesRateType").val(tempValue.salesRateType);
            setTimeout(function () {
                if (tempValue.dateFilter != "") {
                    DateFilter.init(tempValue.dateFormat, function () {
                        $("#ToDateVoucher").val(moment(tempValue.END_DATE).format("YYYY-MMM-DD"));
                        $("#FromDateVoucher").val(moment(tempValue.START_DATE).format("YYYY-MMM-DD"));
                        $("#FromDateVoucher").trigger("change");
                        $("#ToDateVoucher").trigger("change");
                    });
                }
            }, 1000);
            setTimeout(function () {
                var dateOptions = $('#ddlDateFilterVoucher option');
                var check = $.grep(dateOptions, function (n, i) {
                    return n.value == 'Next Year';
                });
                if (check.length == 0) {
                    appendNextYear();
                }
            }, 1500);
            tempValue_deffer.resolve();
        }, 1000);
        $scope.next = true;
        $scope.saveContinue = false;
        //$scope.customervalue = tempValue.customerCode;
        $scope.branchvalue = tempValue.branchcode;
        //$scope.employeevalue = tempValue.employeecode;
        $scope.divisionvalue = tempValue.divisioncode;
        $scope.dateFormat = tempValue.dateFormat;
    }
    else if ($scope.param != '' && $scope.param != undefined && (tempValue == undefined || tempValue == 'undefined')) {

        showloader();
        $.ajax({
            //url: '/api/PlanSetupApi/GetPlanDetailValueByPlanCode',
            url: '/api/BudgetPlanApi/GetPlanDetailValueByPlanCode',
            data: { plancode: p_code }
        }).done(function (ret) {

            $('#plancode').val(ret.CoaPlan.PLAN_CODE);
            var $scope = angular.element($("#planName")).scope();
            $scope.plancode = ret.CoaPlan.PLAN_CODE;
            $scope.planName = ret.CoaPlan.PLAN_EDESC;
            $scope.frequency = ret.CoaPlan.TIME_FRAME_CODE;
            //$scope.plantype = ret.PLAN_TYPE;
            //$scope.planfor = ret.SALES_AMOUNT != null ? 'AMOUNT' : 'QUANTITY';
            //$scope.customer_product_option = (ret.customerCode == null || ret.customerCode == '') ? 'product_only' : 'customer_product';
            //if (ret.customerCode == null || ret.customerCode == '') {
            //    $scope.IsCustomerProduct = false;
            //}
            $scope.remarks = ret.CoaPlan.REMARKS;
            $("#frequency").data("kendoDropDownList").value(ret.CoaPlan.TIME_FRAME_CODE);
            //$("#planfor").data("kendoDropDownList").value($scope.planfor);

            //$(".salesRateType").val(ret.salesRateType);
            $scope.dateFormat = ret.CoaPlan.CALENDAR_TYPE == 'ENG' ? 'AD' : 'BS';
            setTimeout(function () {
                if ($scope.dateFormat != "") {
                    DateFilter.init($scope.dateFormat, function () {
                        $("#ToDateVoucher").val(moment(ret.CoaPlan.END_DATE).format("YYYY-MMM-DD"));
                        $("#FromDateVoucher").val(moment(ret.CoaPlan.START_DATE).format("YYYY-MMM-DD"));
                        $("#FromDateVoucher").trigger("change");
                        $("#ToDateVoucher").trigger("change");
                    });
                }
            }, 1000);
            setTimeout(function () {
                var dateOptions = $('#ddlDateFilterVoucher option');
                var check = $.grep(dateOptions, function (n, i) {
                    return n.value == 'Next Year';
                });
                if (check.length == 0) {
                    appendNextYear();
                }
            }, 1500);
            $scope.branchvalue = ret.CoaPlanDtlList[0].BRANCH_CODE;
            $scope.divisionvalue = ret.CoaPlanDtlList[0].DIVISION_CODE;
            $scope.branchcode = ret.CoaPlanDtlList[0].BRANCH_CODE;
            $scope.branchName = ret.CoaPlanDtlList[0].branchName;
            $scope.divisioncode = ret.CoaPlanDtlList[0].DIVISION_CODE;
            $scope.divisionName = ret.CoaPlanDtlList[0].divisionName;
            $scope.GetPlanDetailValueByPlanCode_returnResult = ret;
            paramPreset_deffer.resolve();
            $scope.$apply();
        }).error(function (err) {
        });
        hideloader();
    }

    //$rootScope.reservedBudgetData = null;
    $.when(productDeffer).then(function () {
        if (tempValue == undefined && ($scope.param == '' || $scope.param == undefined)) {
            hideloader();
            return false;
        }
    });
    $.when(productDeffer, tempValue_deffer).then(function () {

        if (tempValue != undefined) {
            setTimeout(function () {
                if (tempValue.multiselectValue != undefined) {
                    var productMultiselect = $("#productMultiselect").data("kendoMultiSelect");
                    productMultiselect.value(tempValue.multiselectValue);
                }
                var productTreeView = $('#COATreeview').data('kendoTreeView');
                $.each(tempValue.checkeOnlyDataTemp, function (i, v) {

                    var item = productTreeView.findByText(v.ITEM_EDESC);

                    if (item.length > 0) {
                        productTreeView.dataItem(item).set("checked", true);
                    }
                });
                _.each(tempValue.selectedItemsList, function (i, v) {

                    productTreeView.expandPath([i.ITEM_CODE]);
                })
                hideloader();
            }, 1000);
        }
        return false;
    });

    $.when(productDeffer, paramPreset_deffer).then(function () {
        if ($scope.param != '' && (tempValue == undefined || tempValue == 'undefined')) {
            setTimeout(function () {

                var productTreeView = $('#COATreeview').data('kendoTreeView');
                var result = $scope.GetPlanDetailValueByPlanCode_returnResult;
                var ret = { selectedItemsList: null };
                ret.selectedItemsList = result.CoaPlanItems;
                for (var i = 0; i < ret.selectedItemsList.length; i++) {
                    var v = ret.selectedItemsList[i];
                    productTreeView.expandPath([v.ITEM_CODE]);
                    if (i == (ret.selectedItemsList.length - 1)) {
                        $.each(ret.selectedItemsList, function (i, v) {
                            var itemName = v.ITEM_EDESC;
                            var itemCode = v.ITEM_CODE;
                            var item = productTreeView.findByText(itemName);
                            if (item.length > 0) {
                                if (v.GROUP_SKU_FLAG == 'N') {
                                    productTreeView.dataItem(item).set("dirty", true);
                                    var listOfChilds = $.grep(ret.selectedItemsList, function (it, vl) {
                                        return v.MASTER_ITEM_CODE == it.PRE_ITEM_CODE;//&& vl.ITEM_CODE==item.ITEM_CODE;
                                    });
                                    if (listOfChilds.length == 0) {
                                        productTreeView.dataItem(item).set("checked", true);
                                    }
                                }
                                else if (v.GROUP_SKU_FLAG == 'T') {
                                    productTreeView.dataItem(item).set("checked", true);
                                }
                            }
                        });
                    }
                }
                tree_check();
                getGroupOfItem();
                getCheckedItems();
                if (checkedNodeOnly.length > 0) {
                    var productMultiselect = $("#productMultiselect").data("kendoMultiSelect");
                    productMultiselect.value(checkedNodeOnly[0].ITEM_CODE);
                }
                hideloader();
            }, 1000);
        }
        return false;
    })
});

