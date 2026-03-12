/// <reference path="~/Scripts/_references.js" />
/// <reference path="../module/planningModule.js" />

planningModule.controller('planningCtrl', function ($scope, planservice, $routeParams, $rootScope, $http) {
    
    $scope.param = $routeParams.plancode;
    $scope.planName = $routeParams.planName;
    $rootScope.reservedData;
    tempValue = $rootScope.reservedData;
    $scope.next = true;
    $scope.divisionFlag = false;
    $scope.branchFlag = '';
    $scope.employeeFlag = '';
    $scope.customerFlag = '';
    $scope.partytypeFlag = '';

    $scope.itemflag = '';
    $scope.divisionFlag = 'Y';
    $scope.agentFlag = '';

    $scope.saveContinue = true;
    
    function getDivisionFlagByPreferenceSetup() {
        var url = window.location.protocol + "//" + window.location.host + "/api/PlanApi/GetDivisionFlag";
        var req = {
            method: 'GET',
            url: url,
        }
        $http(req).then(function (data, status, headers, config) {
            if (data.data.DATA === 'Y') {
                $scope.divisionFlag = 'Y';
            }
            else {
                $scope.divisionFlag = 'N';
            }

        }, function (data, status, headers, config) {
        });
    };
    function getFlagFromPreferenceSetup() {
        var url = window.location.protocol + "//" + window.location.host + "/api/PlanApi/GetPlanPreferenceSetupFlag";
        var req = {
            method: 'GET',
            url: url,
        }
        $http(req).then(function (data, status, headers, config) {
            if (data.data.DATA.length == 0)
                return;
            $scope.branchFlag = data.data.DATA[0].SHOW_BRANCH;
            $scope.employeeFlag = data.data.DATA[0].SHOW_EMPLOYEE;
            $scope.customerFlag = data.data.DATA[0].SHOW_CUSTOMER;
            $scope.itemflag = data.data.DATA[0].SHOW_ITEM;
            $scope.partytypeFlag = data.data.DATA[0].SHOW_PARTY_TYPE;
            $scope.agentFlag = data.data.DATA[0].SHOW_AGENT;

            if ($scope.agentFlag !== 'N')
                $scope.bindAgentOptions();
            if ($scope.divisionFlag!=='N')
                $scope.divisionFlag = data.data.DATA[0].SHOW_DIVISION;
            if ($scope.branchFlag == 'Y')
                $scope.bindBranchOption();
            if ($scope.employeeFlag == 'Y')
                $scope.bindEmployeeOption();
            if ($scope.partytypeFlag == 'Y')
                $scope.bindPartyTypeOption();
            if ($scope.divisionFlag == 'Y')
                $scope.bindDivisionOption();
            if ($scope.itemflag != "Y")
            {
                hideloader();
            }
        }, function (data, status, headers, config) {
        });
    };

    getDivisionFlagByPreferenceSetup();
    getFlagFromPreferenceSetup();

    $scope.pageName = "Sales Plan";
    $scope.progress = 'style="5%"';
    $scope.startdate = [];
    function IsFormValid() {
        var message = "";
        var pnameSelect = $('#planName').val();
        var frequencySelected = $('#frequency').val();
        var plantypeSelected = $('#plantype').val();
        var planforSelected = $('#planfor').val();
        var remark = $('#remarks').val();
        var IsCustomerSelected = $('input[name=customer_product_option]:checked').val();
        var customerSelected = $('#customers').val();
        var employeeSelected = $('#employees').val();
        var productMultiselect = $("#productMultiselect").data("kendoMultiSelect");
        var count = 1;
        if ($scope.itemflag == 'Y')
            count = productMultiselect.value().length;
        if (pnameSelect == "" || pnameSelect.length > 200) {
            return false;
        }
        if (pnameSelect.includes("\"") || pnameSelect.includes("'")) {
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
        else if ((employeeSelected == "" || employeeSelected == null) && loggedInUsers_employeeCode == "" && $scope.employeeFlag =="Y") {
            return false;
        }
        else if (count < 1) {
            return false;
        }
        else if (remark.length > 400) {
            return false;
        }
        else if (remark.includes("\"") || remark.includes("'")) {
            return false;
        }
        else if (IsCustomerSelected != "customer_product" && loggedInUsers_employeeCode == '' && employeeSelected == "") {
            return false;
        }
        else {
            return true;
        }
    }
    $scope.gotoPlanSetup = function () {
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
            //if (itemLength > parseInt(productSelectionLimit)) { // productSelectionLimit could set on web.config
            //    //displayPopupNotification("You have selected more than " + productSelectionLimit + " (i.e " + itemLength + ") item. Please limit your item selection below " + productSelectionLimit + ".", "warning");
            //    hideloader();
            //    var isReadyToContinue = confirm("You have selected large number of Products( " + itemLength + " >" + productSelectionLimit + "),\n processing may take long time. Are you sure to continue?");
            //    if (isReadyToContinue) {
            //        displayPopupNotification("Processing time takes longer for more products... Please keep patience", "info");
            //        if ($scope.param != '' && $scope.param != undefined && $scope.param != 'undefined') {
            //            window.location.href = "/Planning/Home/Index#!Planning/PlanSetup/" + $scope.param;
            //        } else {
            //            window.location.href = "/Planning/Home/Index#!Planning/PlanSetup";
            //        }
            //    }
            //    else {
            //        hideloader();
            //    }                
            //}
            //else {
            //    if ($scope.param != '' && $scope.param != undefined && $scope.param != 'undefined') {
            //        window.location.href = "/Planning/Home/Index#!Planning/PlanSetup/" + $scope.param;
            //    } else {
            //        window.location.href = "/Planning/Home/Index#!Planning/PlanSetup";
            //    }
            //}
            //productSelectionLimit = '';
            //productSelectionLimit = (productSelectionLimit == '' ? 100 : productSelectionLimit);
            //if (itemLength < parseInt(productSelectionLimit)) {
                if ($scope.param != '' && $scope.param != undefined && $scope.param != 'undefined') {
                    window.location.href = "/Planning/Home/Index#!Planning/PlanSetup/" + $scope.param;
                } else {
                    window.location.href = "/Planning/Home/Index#!Planning/PlanSetup";
                }
            //}
            //else {
            //    displayPopupNotification("You have selected more than " + productSelectionLimit + " (i.e " + itemLength + ") item. Please limit your item selection below " + productSelectionLimit + ".", "warning");
            //    hideloader();
            //}
        }
    }
    $scope.IsCustomerProduct = true;
    $scope.customer_product_option = "customer_product";
    $scope.CustomerProductOption = function () {

        if ($scope.customer_product_option == 'customer_product') {
            $scope.IsCustomerProduct = true;
            return;
        }
        $scope.IsCustomerProduct = true;
    }

    

    $scope.bindAgentOptions = function () {
        $scope.agentDataSource = {
            type: "json",
            serverFiltering: false,
            transport: {
                read: {
                    dataType: "json",
                    url: "/api/SubPlanApi/GetChiledLevelAgent",
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

        $scope.agentOptions = {
            dataSource: $scope.agentDataSource,
            dataTextField: "AGENT_EDESC",
            dataValueField: "AGENT_CODE",
            optionLabel: "--Select Agent--",
            filter: "contains",
        };
    }
    $scope.agentvalue = null;

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
            optionLabel: "--Select Employee--",
            filter: "contains",
        };
    }
    $scope.employeevalue = null;

    $scope.bindPartyTypeOption = function () {
        $scope.partyTypeDataSource = {
            type: "json",
            serverFiltering: false,
            transport: {
                read: {
                    dataType: "json",
                    url: "/api/SubPlanApi/GetChiledLevelPartyType",
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

        $scope.partyTypeOptions = {
            dataSource: $scope.partyTypeDataSource,
            dataTextField: "PARTY_TYPE_EDESC",
            dataValueField: "PARTY_TYPE_CODE",
            optionLabel: "--Select Party Type--",
            filter: "contains",
        };
    }
    $scope.partytypevalue = null;


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

    //$scope.divisionOptions = {
    //    dataSource: $scope.divisionDataSource,
    //    dataTextField: "DIVISION_EDESC",
    //    dataValueField: "DIVISION_CODE",
    //    optionLabel: "--Select Division--",
    //    filter: "contains",
    //};
  

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
    $scope.customersDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                dataType: "json",
                url: "/api/SubPlanApi/GetCustomersForPlan",
                //url:"/api/SubPlanApi/GetChiledLevelCustomers"
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
                            filter: $("#customers").val()
                        };
                        return newParams;
                    }
                }
                else {
                    var newParams = {
                        filter: $("#customers").val()
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
        minLength: 3,
        //dataBound: function (e) {
        //    if (this.element[0].attributes['chargeAcc-index'] == undefined) {
        //        var acccode = $(".accCodeAutoComplete").data("kendoComboBox");
        //    }
        //    else {
        //        var index = this.element[0].attributes['chargeAcc-index'].value;
        //        var accLength = ((parseInt(index) + 1) * 3) - 1;
        //        var acccode = $($(".cacccode")[accLength]).data("kendoComboBox");

        //    }
        //    if (acccode != undefined) {
        //        acccode.setOptions({
        //            template: $.proxy(kendo.template("#= formatValue(ACC_EDESC, Type, this.text()) #"), acccode)
        //        });
        //    }
        //},
    };

    $scope.customerCodeOnChange = function (kendoEvent) {
        if (kendoEvent.sender.dataItem() == undefined) {
            $('#customers').data("kendoComboBox").value([]);
        }
    };

    $scope.customervalue = null;

    //$scope.dateFormat = 'AD';
    $scope.dateFormat = 'BS';
    $scope.DateFormatChange = function () {

    }

    $scope.GetPlanDetailValueByPlanCode_returnResult = null;
    var tempValue_deffer = $.Deferred();
    var paramPreset_deffer = $.Deferred();
    if (tempValue != undefined) {
        $scope.planName = tempValue.PLAN_EDESC;
        $scope.frequency = tempValue.TIME_FRAME_CODE;
        $scope.plantype = tempValue.PLAN_TYPE;
        $scope.planfor = tempValue.PLAN_FOR;
        $("#frequency").data("kendoDropDownList").value(tempValue.TIME_FRAME_CODE);
        $("#plantype").data("kendoDropDownList").value(tempValue.PLAN_TYPE);
        $("#planfor").data("kendoDropDownList").value(tempValue.PLAN_FOR);
        setTimeout(function () {
            $(".salesRateType").val(tempValue.salesRateType);
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
                $("#customers").data('kendoComboBox').dataSource.read();
            }, 1500);
            tempValue_deffer.resolve();
        }, 1000);
        $scope.next = true;
        $scope.saveContinue = false;
        $scope.customervalue = tempValue.customerCode;
        $scope.branchvalue = tempValue.branchcode;
        $scope.employeevalue = tempValue.employeecode;
        $scope.partytypevalue = tempValue.partytypecode;
        $scope.agentvalue = tempValue.agentcode;
        $scope.divisionvalue = tempValue.divisioncode;
        $scope.dateFormat = tempValue.dateFormat;
    }
    else if (($scope.param != '' && $scope.param != undefined) && (tempValue == undefined || tempValue == 'undefined')) {

        showloader();
        $.ajax({
            url: '/api/PlanSetupApi/GetPlanDetailValueByPlanCode',
            data: { plancode: p_code }
        }).done(function (ret) {
            
            $('#plancode').val(ret.PLAN_CODE);
            var $scope = angular.element($("#planName")).scope();
            $scope.plancode = ret.PLAN_CODE;
            $scope.planName = ret.PLAN_EDESC;
            $scope.frequency = ret.TIME_FRAME_CODE;
            $scope.plantype = ret.PLAN_TYPE;
            $scope.planfor = ret.SALES_AMOUNT != null ? 'AMOUNT' : 'QUANTITY';
            $scope.customer_product_option = (ret.customerCode == null || ret.customerCode == '') ? 'customer_product' : 'customer_product';
            if (ret.customerCode == null || ret.customerCode == '') {
                $scope.IsCustomerProduct = true;
            }
            $scope.remarks = ret.REMARKS;
            $("#frequency").data("kendoDropDownList").value(ret.TIME_FRAME_CODE);
            $("#planfor").data("kendoDropDownList").value($scope.planfor);

            $(".salesRateType").val(ret.salesRateType);
            $scope.dateFormat = ret.CALENDAR_TYPE == 'ENG' ? 'AD' : 'BS';
            setTimeout(function () {
                if ($scope.dateFormat != "") {
                    DateFilter.init($scope.dateFormat, function () {
                        $("#ToDateVoucher").val(moment(ret.END_DATE).format("YYYY-MMM-DD"));
                        $("#FromDateVoucher").val(moment(ret.START_DATE).format("YYYY-MMM-DD"));
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
                $("#customers").data('kendoComboBox').dataSource.read();
            }, 1500);
            $scope.customervalue = ret.customerCode;
            $scope.branchvalue = ret.salesPlanDetail[0].BRANCH_CODE;
            $scope.employeevalue = ret.salesPlanDetail[0].EMPLOYEE_CODE;
            $scope.divisionvalue = ret.salesPlanDetail[0].DIVISION_CODE;
            $scope.partytypevalue = ret.salesPlanDetail[0].PARTY_TYPE_CODE;
            $scope.agentvalue = ret.salesPlanDetail[0].AGENT_CODE;

            $scope.branchcode = ret.salesPlanDetail[0].BRANCH_CODE;
            $scope.branchName = ret.salesPlanDetail[0].branchName;
            $scope.divisioncode = ret.salesPlanDetail[0].DIVISION_CODE;
            $scope.divisionName = ret.salesPlanDetail[0].divisionName;
            $scope.employeecode = ret.salesPlanDetail[0].EMPLOYEE_CODE;
            $scope.employeeName = ret.salesPlanDetail[0].employeeName;
            $scope.GetPlanDetailValueByPlanCode_returnResult = ret;
            paramPreset_deffer.resolve();
            $scope.$apply();
        }).error(function (err) {
        });
    }

    $rootScope.reservedData = null;
    $.when(productDeffer).then(function () {
        if (tempValue == undefined && ($scope.param == '' || $scope.param == undefined)) {
            if ($scope.itemflag == "Y") {
                hideloader();
            }
            return false;
        }
    });
    $.when(productDeffer, tempValue_deffer).then(function () {
        if (tempValue != undefined) {
            if ($scope.itemflag == "Y") {
                setTimeout(function () {
                    if (tempValue.multiselectValue != undefined) {
                        var productMultiselect = $("#productMultiselect").data("kendoMultiSelect");
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
                    hideloader();
                }, 1000);
            }
        }
        return false;
    });

    $.when(productDeffer, paramPreset_deffer).then(function () {
        if ($scope.param != '' && (tempValue == undefined || tempValue == 'undefined')) {
            if ($scope.itemflag == "Y") {
                setTimeout(function () {
                    var productTreeView = $('#productTreeView').data('kendoTreeView');
                    var ret = $scope.GetPlanDetailValueByPlanCode_returnResult;
                    for (var i = 0; i < ret.selectedItemsList.length; i++) {
                        var v = ret.selectedItemsList[i];
                        productTreeView.expandPath([v.ITEM_CODE]);

                        if (i == (ret.selectedItemsList.length - 1)) {
                            $.each(ret.selectedItemsList, function (i, v) {
                                var itemName = v.ITEM_EDESC;
                                var itemCode = v.ITEM_CODE;
                                var item = productTreeView.findByText(itemName);
                                if (item.length > 0) {
                                    if (v.GROUP_SKU_FLAG == 'G') {
                                        productTreeView.dataItem(item).set("dirty", true);
                                        var listOfChilds = $.grep(ret.selectedItemsList, function (it, vl) {
                                            return v.MASTER_ITEM_CODE == it.PRE_ITEM_CODE;//&& vl.ITEM_CODE==item.ITEM_CODE;
                                        });
                                        if (listOfChilds.length == 0) {
                                            productTreeView.dataItem(item).set("checked", true);
                                        }
                                    }
                                    else if (v.GROUP_SKU_FLAG == 'I') {
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
        }
        return false;
    })
});

