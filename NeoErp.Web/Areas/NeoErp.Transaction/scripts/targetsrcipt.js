var targetapp = angular.module('targetModule', ["kendo.directives", "datatables"]);
targetapp.controller('targetController', function ($http, $scope, targetapiservice) {
    $scope.targetList = [];
    $scope.branchList = [];
    //$scope.targetNameList = [{EndDate:"2016-11-10T16:27:39.0090256+05:45",
    //    EndDateString:"2016-11-10",
    //    RangeName:"Today",
    //    SortOrder:11,
    //    StartDate:"2016-11-10T16:27:39.0090256+05:45",
    //    StartDateString:"2016-11-10"
    //}];
    $scope.targetNameList = null;
    $scope.target = {
        target_code: '',
        name: '', /* month,quater etc name */
        year_name: '2016',
        calendar_type: 'ENG', /*('ENG','LOC') */
        range_type: 'MONTH', /*'MONTH','QUATER','HALFYEAR','BIMONTHLY' */
        start_date: '',
        end_date: '',
        item_code: '',
        item_name: '', // item display name
        category_code: '',
        category_name: '', // category display name
        customer_code: '',
        customer_name: '', // customer display name
        employee_code: '',
        employee_name: '', // employee display name
        sales_target: 0,
        collection_target: 0,
        remarks: '',
        company_code: '',
        branch_code: '',
        branchCodeList: $scope.branchList,
        start_date_np: '',
        end_date_np: '',
    };
    //$scope.itemDataSource = {
    //    type: "json",
    //    serverFiltering: false,
    //    transport: {
    //        read: {
    //            url: "/api/Transaction/GetAllProducts",
    //        }
    //    }
    //};

    //$scope.LoadItemCodeOption = {
    //    filter: "contains",
    //    dataTextField: "ItemDescription",
    //    dataValueField: "ItemCode",
    //    placeholder: "Select Item...",
    //    height: 1000,
    //    suggest: true,
    //    dataSource: $scope.itemDataSource,
    //}

    $scope.SaveAndContinueTarget = function (target, form) {
        savingTargetFunction(target, form, 'continue');
    }
    $scope.SaveTarget = function (target, form) {
        savingTargetFunction(target, form, 'cancel');
    }
    savingTargetFunction = function (target, form, option) {
        $scope.target.branch_code = $scope.branchList.length > 0 ? $scope.branchList : "0";
        $scope.target.year_name = $scope.target.start_date.split('-')[0];
        if ($scope.FormIsValid(target)) {
            targetapiservice.saveTarget(target).then(function (msg) {
                if (msg.data == "success") {
                    displayPopupNotification("Target saved.", "success");
                    init();
                    $scope.ResetTargetForm();
                    form.$setPristine();
                    form.$setUntouched();
                    if (option == 'cancel') {
                        $scope.showFormPanel = false;
                    }
                }
                else if (msg.data == "failed") {
                    displayPopupNotification("Target save failed.", "error");
                }
            });
        }
        else {
            displayPopupNotification("Please fill up all target setup field and Try again.", "warning");
        }
    }

    $scope.FormIsValid = function (target) {
        debugger;
        if (target.start_date == ''
            || target.end_date == ''
            || target.name == ''
            || target.sales_target == 0
            || target.collection_target == 0) {
            return false;
        }
        return true;
    }
    $scope.EditTarget = function (item) {
        $scope.target.target_code = item.TARGET_CODE;
        $scope.target.name = item.NAME;
        $scope.target.year_name = item.YEAR_NAME;
        $scope.target.calendar_type = item.CALENDAR_TYPE; /*('ENG';'LOC') */
        $scope.target.range_type = item.RANGE_TYPE; /*'month';'quater';'halfyear';'bimonthly' */
        $scope.target.start_date = item.START_DATE;
        $scope.target.end_date = item.END_DATE;
        $scope.target.item_code = 1008;
        $scope.target.item_name = item.Item_Name;
        $scope.target.category_code = item.CATEGORY_CODE;
        $scope.target.category_name = item.Category_Name;
        $scope.target.customer_code = item.CUSTOMER_CODE;
        $scope.target.customer_name = item.Customer_Name;
        $scope.target.employee_code = item.EMPLOYEE_CODE;
        $scope.target.employee_name = item.Employee_Name;
        $scope.target.sales_target = parseInt(item.SALES_TARGET);
        $scope.target.collection_target = parseInt(item.COLLECTION_TARGET);
        $scope.target.remarks = item.REMARKS;
        $scope.target.company_code = item.COMPANY_CODE;
        $scope.target.branch_code = item.BRANCH_CODE;
        $scope.target.created_by = item.CREATED_BY;
        $scope.target.created_date = item.CREATED_DATE;
        $scope.target.last_modified_by = item.LAST_MODIFIED_BY;
        $scope.target.last_modified_date = item.LAST_MODIFIED_DATE;
        $scope.target.approved_by = item.APPROVED_BY;
        $scope.target.approved_date = item.APPROVED_DAT;
        $scope.target.deleted_flag = item.DELETED_FLAG;
        if ($scope.target.calendar_type == "LOC") {
            $scope.target.start_date_np = AD2BS($scope.target.start_date);
            $scope.target.end_date_np = AD2BS($scope.target.end_date);
        }
        else {
            $scope.target.start_date_np = '';
            $scope.target.end_date_np = '';
        }
        //$scope.itemDataSource = [{ ItemCode: item.ITEM_CODE, ItemDescription: item.Item_Name }];
        $scope.showFormPanel=true;
    };

    $scope.RemoveTarget = function (target) {
        bootbox.confirm("Are you sure to delete target?", function (result) {
            if (result) {
                targetapiservice.removeTarget(target).then(function (msg) {
                    if (msg.data == "success") {
                        displayPopupNotification("Target removed.", "success");
                        init();
                        $scope.ResetTargetForm();
                    }
                    else if (msg.data == "failed") {
                        displayPopupNotification("Target delete failed, plz try again.", "error");
                    }
                });
            }
        });
    }

    $scope.showFormPanel = false;
    $scope.AddTarget = function () {
        $scope.showFormPanel = true;
    }
    $scope.CancelTargetForm = function () {
        $scope.showFormPanel = false;
        $scope.ResetTargetForm();
    }
    $scope.ResetTargetForm = function () {
        $scope.target ={
            target_code: '',
            name: '', /* month,quater etc name */
            year_name: '2016',
            calendar_type: 'ENG', /*('ENG','LOC') */
            range_type: 'MONTH', /*'MONTH','QUATER','HALFYEAR','BIMONTHLY' */
            start_date: '',
            end_date: '',
            item_code: '',
            category_code: '',
            customer_code: '',
            employee_code: '',
            sales_target: 0,
            collection_target: 0,
            remarks: '',
            company_code: '',
            branch_code: '',
            start_date_np: '',
            end_date_np: '',
        };
    }

    init();
    function init() {
        targetapiservice.getAllTarget()
        .then(function (d) {
            $scope.targetList = d.data;
        }, function (e) {
            alert("Generic_error");
        });

        targetapiservice.getAllTargetNameList($scope.target.calendar_type)
        .then(function (d) {
            $scope.targetNameList = d.data;
        }, function (e) {
            displayPopupNotification("Target Name retrive failed.", "error");
        })
    }
    $scope.ItemCodeSelected = function (e) {

        if (e.sender.dataItems().length > 0) {
            $scope.target.item_code = e.sender.dataSource.data()[e.sender.selectedIndex].ItemCode;
            $scope.target.item_name = e.sender.dataSource.data()[e.sender.selectedIndex].ItemDescription;
        }
        else {
            $scope.target.item_code = '';
            $scope.target.item_name = '';
        }
    }
    $scope.Calander_Type_Changed = function () {
        $scope.targetNameList = null;
        targetapiservice.getAllTargetNameList($scope.target.calendar_type)
        .then(function (d) {
            $scope.targetNameList = d.data;
        }, function (e) {
            displayPopupNotification("Target Name retrive failed.", "error");
        })
    }
});

targetapp.factory('targetapiservice', function ($http) {
    var fac = {};
    fac.getAllTargetNameList = function (calander_type) {
        var current_date = getNepaliDate();
        var date = current_date.split("-");
        var currentFiscalYear = (parseInt(date[0]) - 1).toString() + "/" + date[0].toString().substring(2, date[0].toString().length);
        if (calander_type == "ENG" || calander_type == undefined) {
            return $http.get('/api/Common/GetEnglishDateFilters?fiscalYear=' + currentFiscalYear);
        }
        else if (calander_type == "LOC") {
            return $http.get('/api/Common/GetDateFilters?fiscalYear=' + currentFiscalYear);
        }
    }
    fac.getAllTarget = function () {
        return $http.get('/api/TargetApi/GetAllTargetList');
    }

    fac.saveTarget = function (target) {
        return $http.post('/api/TargetApi/PostTarget', target);
    }

    fac.removeTarget = function (target) {
        return $http.post('/api/TargetApi/DeleteTarget', target);
    }
    return fac;
});