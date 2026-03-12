//var transactionApp = angular.module('transactionModule', ["kendo.directives", "ngMessages"]);

transactionModule.controller('transactionCtrl', function ($scope, $http, TransactionService, issueService) {
    debugger;
    $scope.selectedEmployeeId = {};
    $scope.fromLocationSetup = null;
    $scope.products = null;
    $scope.costCenterSetup = null;

    $scope.consumption = {
        date: '',
        miti: '',
        issueId: '',
        mannualNo: '',
        userId: '',
        departmentId: '',
        employeeId: '',
        grandtotal: '',
        issues: [{
            sn: 1,
            locationtemp: '',
            fromLocation: '',
            producttemp: '',
            productDescription: '',
            unit: '',
            quantity: '',
            rate: '',
            amount: '',
            costcentertemp: '',
            costCenter: '',
            remark: '',
        }]
    };

    $scope.selectedEmployeeOptions = {
        filter: "contains",
        height: 1000,
        highlightFirst: false,
        dataTextField: "EMPLOYEE_EDESC",
        dataValueField: "EMPLOYEE_CODE",
        placeholder: "Select Employee...",
        noDataTemplate: 'No Data!',
        suggest: true,
        headerTemplate: '<div class="dropdown-header k-widget k-header customeHeading">' +
                              '<span class="width10">Employee Code</span>' +
                              '<span class="width20">Employee Name</span>' +
                                 '<span class="width15">Email</span>' +
                                    '<span class="width10">Mobile No</span>' +
                                '<span class="width10">Citizen  No</span>' +
                                 '<span class="width20">Permanent Address</span>' +
                                  '<span class="width10">Temporary Address</span>' +
                          '</div>',
        footerTemplate: 'Total #: instance.dataSource.total() # items found',
        template: '<div class="customedata"><span class="width10"> #:data.EMPLOYEE_CODE#</span>' +
                  '<span class="width20">#: data.EMPLOYEE_EDESC #</span>' +
                  '<span class="width15"> #:data.EMAIL#</span>' +
                  '<span class="width10">#: data.MOBILE #</span>' +
                  '<span class="width10">#: data.CITIZENSHIP_NO #</span>' +
                  '<span class="width20">#: data.EPERMANENT_ADDRESS1 #</span>' +
                  '<span class="width15"> #:data.ETEMPORARY_ADDRESS1#</span></div>',
        //template: kendo.template($("#employeetemplate").html()),
        dataSource: {
            type: "json",
            serverFiltering: true,
            transport: {
                read: {
                    url: "/api/Transaction/GetAllEmployeeSetup",
                    //dataType: "jsonp"
                },
                parameterMap: function (data, action) {
                    debugger;
                    var newParams = {
                        //filter: data.filter.filters[0].value
                    };
                    return newParams;
                }
            }
        },
        dataBound: function (obj) {
            var userTxt = obj.sender._prev;
            var listItems = obj.sender.ul.children();
            if (listItems.length > 0 && userTxt.length > 1) {
                $(listItems).each(function (i, e) {
                    var liTxt = $(e).html();
                    //console.log(liTxt);
                    var highlightedTxt = "<strong style='color:blue;'>" + userTxt + "</strong>";
                    e.innerHTML = liTxt.replace(new RegExp(userTxt, "gi"), highlightedTxt);
                });
            }
        }
    };
    $scope.EmployeChanged = function (e, index) {
        if (e.sender.dataItems().length > 0) {
            $scope.consumption.employeeId = e.sender.dataItems()[e.item.index()].EMPLOYEE_CODE;
            $scope.employeetemp = e.sender.dataItems()[e.item.index()].EMPLOYEE_EDESC;
        }
        else {
            $scope.consumption.employeeId = '';
        }
        $("#search").removeClass('open');
        angular.element('#txtEmployee').trigger('focus');
    }

    $scope.selectedCustomerOptions = {
        filter: "contains",
        dataTextField: "CustomerName",
        dataValueField: "CustomerCode",
        placeholder: "Select User...",
        height: 1000,
        minLength: 3,
        enforceMinLength: true,
        suggest: true,
        headerTemplate: '<div class="dropdown-header k-widget k-header customeHeading">' +
                              '<span class="width10">User Id</span>' +
                              '<span class="width20">User Name</span>' +
                              '<span class="width15">Reg. Office address</span>' +
                              '<span class="width10">Tel/Mobile No</span>' +
                              '<span class="width10">VAT No</span>' +
                              '<span class="width10">Region</span>' +
                              '<span class="width10">Zone</span>' +
                              '<span class="width15">Dealing Person</span>' +
                          '</div>',
        footerTemplate: 'Total #: instance.dataSource.total() # items found',
        template: '<div class="customedata">' +
                  '<span class="width10"> #:data.CustomerCode#</span>' +
                  '<span class="width20">#: data.CustomerName #</span>' +
                  '<span class="width15"> #:data.REGD_OFFICE_EADDRESS#</span>' +
                  '<span class="width10">#: data.TEL_MOBILE_NO1 #</span>' +
                  '<span class="width10">#: data.TPIN_VAT_NO #</span>' +
                  '<span class="width10">#: data.REGION_CODE #</span>' +
                  '<span class="width10"> #:data.ZONE_CODE#</span>' +
                  '<span class="width15"> #:data.DEALING_PERSON#</span>' +
                  '</div>',
        dataSource: {
            type: "json",
            serverFiltering: true,
            transport: {
                read: {
                    url: "/api/Transaction/GetAllCustomerSetup",
                },
                parameterMap: function (data, action) {
                    debugger;
                    var newParams = {
                        filter: '',//data.filter.filters[0].value
                    };
                    return newParams;
                }
            }
        },
        dataBound: function (obj) {
            var userTxt = obj.sender._prev;
            var listItems = obj.sender.ul.children();
            if (listItems.length > 0 && userTxt.length > 1) {
                $(listItems).each(function (i, e) {
                    var liTxt = $(e).html();
                    //console.log(liTxt);
                    var highlightedTxt = "<strong style='color:blue;'>" + userTxt + "</strong>";
                    e.innerHTML = liTxt.replace(new RegExp(userTxt, "gi"), highlightedTxt);
                });
            }
        }
    };
    $scope.CustomerChanged = function (e, index) {
        if (e.sender.dataItems().length > 0) {
            $scope.consumption.userId = e.sender.dataItems()[e.item.index()].CustomerCode;
            $scope.usertemp = e.sender.dataItems()[e.item.index()].CustomerName;
        }
        else {
            $scope.consumption.userId = '';
        }
        $("#searchUser").removeClass('open');
    }

    $scope.selectedDepartmentOptions = {
        filter: "contains",
        dataTextField: "DepartmentName",
        dataValueField: "DepartmentCode",
        placeholder: "Select Department...",
        height: 1000,
        suggest: true,
        headerTemplate: '<div class="dropdown-header k-widget k-header customeHeading">' +
                              '<span class="width25">Department Code</span>' +
                              '<span class="width25">Department Name</span>' +
                          '</div>',
        footerTemplate: 'Total #: instance.dataSource.total() # items found',
        template: '<div class="customedata">' +
                  '<span class="width25"> #:data.DepartmentCode#</span>' +
                  '<span class="width25">#: data.DepartmentName #</span>' +
                  '</div>',
        dataSource: {
            type: "json",
            serverFiltering: true,
            transport: {
                read: {
                    url: "/api/Transaction/GetAllDepartmentSetup",
                },
                parameterMap: function (data, action) {
                    var newParams = {
                        filter: data.filter.filters[0].value
                    };
                    return newParams;
                }

            }
        },
        dataBound: function (obj) {
            var userTxt = obj.sender._prev;
            var listItems = obj.sender.ul.children();
            if (listItems.length > 0 && userTxt.length > 1) {
                $(listItems).each(function (i, e) {
                    var liTxt = $(e).html();
                    //console.log("ap"+liTxt);
                    var highlightedTxt = "<strong style='color:blue;'>" + userTxt + "</strong>";
                    e.innerHTML = liTxt.replace(new RegExp(userTxt, "gi"), highlightedTxt);
                });
            }
        }
    };
    $scope.DepartmentChanged = function (e, index) {
        if (e.sender.dataItems().length > 0) {
            $scope.consumption.departmentId = e.sender.dataItems()[e.item.index()].DepartmentCode;
            $scope.departmenttemp = e.sender.dataItems()[e.item.index()].DepartmentName;
        }
        else {
            $scope.consumption.departmentId = '';
        }
        $("#searchDepartment").removeClass('open');
    }

    $scope.selectedLocationOptions = {
        filter: "contains",
        dataTextField: "locationName",
        dataValueField: "LocationCode",
        placeholder: "Select Location...",
        height: 1000,
        suggest: true,
        headerTemplate: '<div class="dropdown-header k-widget k-header customeHeading">' +
                              '<span class="width10">Location Code</span>' +
                              '<span class="width20">Location Name</span>' +
                              '<span class="width20">Contact Person</span>' +
                              '<span class="width20">Telephone/Mobile</span>' +
                              '<span class="width20">Email</span>' +
                              '<span class="width20">Fax</span>' +
                          '</div>',
        footerTemplate: 'Total #: instance.dataSource.total() # items found',
        template: '<div class="customedata">' +
                  '<span class="width10"> #:data.LocationCode#</span>' +
                  '<span class="width20">#: data.locationName #</span>' +
                  '<span class="width20">#: data.Auth_Contact_Person #</span>' +
                  '<span class="width20">#: data.Telephone_Mobile_No #</span>' +
                  '<span class="width20">#: data.Email #</span>' +
                  '<span class="width20">#: data.Fax #</span>' +
                  '</div>',
        dataSource: {
            type: "json",
            serverFiltering: true,
            transport: {
                read: {
                    url: "/api/Transaction/GetAllLocationSetup",
                },
                parameterMap: function (data, action) {
                    var newParams = {
                        filter: data.filter.filters[0].value
                    };
                    return newParams;
                }
            }
        },
        dataBound: function (obj) {
            var userTxt = obj.sender._prev;
            var listItems = obj.sender.ul.children();
            if (listItems.length > 0 && userTxt.length > 1) {
                $(listItems).each(function (i, e) {
                    var liTxt = $(e).html();
                    //console.log(liTxt);
                    var highlightedTxt = "<strong style='color:blue;'>" + userTxt + "</strong>";
                    e.innerHTML = liTxt.replace(new RegExp(userTxt, "gi"), highlightedTxt);
                });
            }
        }
    };
    $scope.LocationChanged = function (e, index) {
        if (e.sender.dataItems().length > 0) {
            $scope.consumption.issues[index].fromLocation = e.sender.dataItems()[e.item.index()].LocationCode;
            $scope.consumption.issues[index].locationtemp = e.sender.dataItems()[e.item.index()].locationName;
        }
        else {
            $scope.consumption.issues[index].fromLocation = '';
            $scope.consumption.issues[index].locationtemp = '';
        }
        $("#searchLocation").removeClass('open');
    }

    $scope.selectedProductOptions = {
        filter: "contains",
        dataTextField: "ItemDescription",
        dataValueField: "ItemCode",
        placeholder: "Select Product...",
        height: 1000,
        suggest: true,
        headerTemplate: '<div class="dropdown-header k-widget k-header customeHeading">' +
                              '<span class="width15">Product code</span>' +
                              '<span class="width25">Product Name</span>' +
                              '<span class="">Unit</span>' +
                          '</div>',
        footerTemplate: 'Total #: instance.dataSource.total() # items found',
        template: '<div class="customedata">' +
                  '<span class="width15"> #:data.ItemCode#</span>' +
                  '<span class="width25">#: data.ItemDescription #</span>' +
                  '<span class="">#: data.ItemUnit #</span>' +
                  '</div>',
        dataSource: {
            type: "json",
            serverFiltering: true,
            transport: {
                read: {
                    url: "/api/Transaction/GetAllProducts",
                },
                parameterMap: function (data, action) {
                    var newParams = {
                        filter: data.filter.filters[0].value
                    };
                    return newParams;
                }
            }
        },
        dataBound: function (obj) {
            var userTxt = obj.sender._prev;
            var listItems = obj.sender.ul.children();
            if (listItems.length > 0 && userTxt.length > 1) {
                $(listItems).each(function (i, e) {
                    var liTxt = $(e).html();
                    //console.log(liTxt);
                    var highlightedTxt = "<strong style='color:blue;'>" + userTxt + "</strong>";
                    e.innerHTML = liTxt.replace(new RegExp(userTxt, "gi"), highlightedTxt);
                });
            }
        }
    };
    $scope.ProductsChanged = function (e, index) {
        if (e.sender.dataItems().length > 0) {
            $scope.consumption.issues[index].productDescription = e.sender.dataItems()[e.item.index()].ItemCode;
            $scope.consumption.issues[index].producttemp = e.sender.dataItems()[e.item.index()].ItemDescription;
            $scope.consumption.issues[index].unit = e.sender.dataItems()[e.item.index()].ItemUnit;
        }
        else {
            $scope.consumption.issues[index].productDescription = '';
            $scope.consumption.issues[index].producttemp = '';
        }
        $("#searchProduct").removeClass('open');
    }

    $scope.selectedCostCenterOptions = {
        filter: "contains",
        dataTextField: "BudgetName",
        dataValueField: "BudgetCode",
        placeholder: "Select CostCenter...",
        height: 1000,
        suggest: true,
        headerTemplate: '<div class="dropdown-header k-widget k-header customeHeading">' +
                              '<span class="width15">Center Code</span>' +
                              '<span class="width25">Center Name</span>' +
                          '</div>',
        footerTemplate: 'Total #: instance.dataSource.total() # items found',
        template: '<div class="customedata">' +
                  '<span class="width15"> #:data.BudgetCode#</span>' +
                  '<span class="width25">#: data.BudgetName #</span>' +
                  //'<span class="">#: data.ItemUnit #</span>' +
                  '</div>',
        dataSource: {
            type: "json",
            serverFiltering: true,
            transport: {
                read: {
                    url: "/api/Transaction/GetAllCostCenter",
                },
                parameterMap: function (data, action) {
                    var newParams = {
                        filter: data.filter.filters[0].value
                    };
                    return newParams;
                }
            }
        },
        dataBound: function (obj) {
            var userTxt = obj.sender._prev;
            var listItems = obj.sender.ul.children();
            if (listItems.length > 0 && userTxt.length > 1) {
                $(listItems).each(function (i, e) {
                    var liTxt = $(e).html();
                    //console.log(liTxt);
                    var highlightedTxt = "<strong style='color:blue;'>" + userTxt + "</strong>";
                    e.innerHTML = liTxt.replace(new RegExp(userTxt, "gi"), highlightedTxt);
                });
            }
        }
    };
    $scope.CostCenterChanged = function (e, index) {
        if (e.sender.dataItems().length > 0) {
            $scope.consumption.issues[index].costCenter = e.sender.dataItems()[e.item.index()].BudgetCode;
            $scope.consumption.issues[index].costcentertemp = e.sender.dataItems()[e.item.index()].BudgetName;
        }
        else {
            $scope.consumption.issues[index].costCenter = '';
            $scope.consumption.issues[index].costcentertemp = '';
        }
        $("#searchCostcenter").removeClass('open');
    }
    init();
    $scope.refreshDate = function () {
        //$scope.consumption.miti = $scope.consumption.date;
    },

    $scope.addIssue = function () {
        var i = $scope.consumption.issues.length;
        var available = $scope.consumption.issues;
        $scope.consumption.issues = [];
        $scope.consumption.issues.push({
            sn: i + 1,
            locationtemp: '',
            fromLocation: '',
            productDescription: '',
            unit: '',
            quantity: '',
            rate: '',
            amount: '',
            costCenter: '',
            remark: '',
        });
        for (var i = 0; i < available.length; i++) {
            var item = available[i];
            //item.sn = 2 + i;
            $scope.consumption.issues.push(item);
        }
    },
    $scope.removeIssue = function (index) {
        $scope.consumption.issues.splice(index, 1);
        for (var i = 0; i < $scope.consumption.issues.length; i++) {
            $scope.consumption.issues[i].sn = $scope.consumption.issues.length - i;//  $scope.consumption.issues[i].sn - 1;
        }
    },
    $scope.setUnit = function (e, index) {
        var unit = '';
        if (e.sender.dataItems().length > 0) {
            unit = e.sender.dataItems()[e.item.index()].ItemUnit;
        }
        $scope.consumption.issues[index].unit = unit;
    }
    $scope.getTotalPCS = function () {
        var pcs = 0;
        var issueList = $scope.consumption.issues;
        for (var i = 0; i < issueList.length; i++) {
            if (issueList[i].unit.toUpperCase() == 'PCS') {
                pcs += parseInt(issueList[i].quantity);
            }
        }
        return pcs;
    }
    $scope.getTotalKGS = function () {
        var kgs = 0;
        var issueList = $scope.consumption.issues;
        for (var i = 0; i < issueList.length; i++) {
            if (issueList[i].unit.toUpperCase() == 'KGS') {
                kgs += parseInt(issueList[i].quantity);
            }
        }
        return kgs;
    }
    var additional = 0;
    var deduction = 0;
    $scope.getAdditional = function () {
        return additional;
    }
    $scope.getDeduction = function () {
        return deduction;
    }
    $scope.granTotal = function () {
        var grantotal = 0;
        var issueList = $scope.consumption.issues;
        for (var i = 0; i < issueList.length; i++) {
            grantotal += issueList[i].amount;
        }
        $scope.consumption.grandtotal = grantotal;
        return grantotal;
    }
    $scope.SaveConsumptionIssue = function (form,IsValid) {
        if (validateConsumptionIssue(IsValid)) { 
            TransactionService.SaveConsumptionIssue($scope.consumption).then(function (status) {
                displayPopupNotification("Consumption Issue saved.", "success");
                $scope.ResetConsumptionIssue(form);
            }, function (error) {
                displayPopupNotification("Error on saving.Please fix the error and Retry. \n Message:"+error, "error");                
            });
        }
    };

    $scope.ResetConsumptionIssue = function (form) {
        debugger;
        issueService.resetConsumptionIssue($scope);
        form.$setPristine();
        form.$setUntouched();
        init();
    }

    // validation
    validateConsumptionIssue = function (IsValid) {
        if (!IsValid) {
            displayPopupNotification("Input fileds are not valid, Please review form and Retry.", "warning");
            return false;
        }
        var consumption = $scope.consumption;
        if (consumption.issues.length < 1) {
            displayPopupNotification("At least one issue should be entered, Please review Issue list and Retry.", "warning");
            return false;
        }
        return true;
    }

    $scope.validateMannualNo = function () {
        var str = $scope.consumption.mannualNo;
        s = String(str || "");
        var count = 0, stringLength = s.length, i;

        for (i = 0 ; i < stringLength ; i++) {
            var partCount = encodeURI(s[i]).split("%").length;
            count += partCount == 1 ? 1 : partCount - 1;
        }
        if (count > 50) {
            return false;
        }
    }

    function init() {
        TransactionService.GetAllSetup()
        .then(function (d) {
            $data = d.data;
            $consumption = $scope.consumption;
            $consumption.issueId = $data.IssueNo;
            $consumption.date = $data.Date;
            //$consumption.miti = $data.Miti; //AD2BS($($data.Date).val());// 

        }, function () {
            alert("Generic_error");
        });
    }

});

transactionModule.factory('TransactionService', function ($http) {
    var fac = {};
    fac.GetAllSetup = function () {
        return $http.get('/api/Transaction/GetAllTransactionSetup');
    }
    //fac.GetAllLocation = function () {
    //    return $http.get("/api/Transaction/GetAllLocationSetup");
    //}
    fac.GetAllProducts = function () {
        return $http.get("/api/Transaction/GetAllProducts");
    }
    fac.GetAllCostCenter = function () {
        return $http.get("/api/Transaction/GetAllCostCenter");
    }
    fac.SaveConsumptionIssue = function (data) {
        return $http.post("/api/Transaction/PostConsumptionIssue", data);
    }
    return fac;
});


transactionModule.service('issueService', function () {
    this.resetConsumptionIssue = function ($scope) {
        var consumption = $scope.consumption;
        consumption.issues = [{
            sn: 1,
            locationtemp: '',
            fromLocation: '',
            producttemp: '',
            productDescription: '',
            unit: '',
            quantity: '',
            rate: '',
            amount: '',
            costcentertemp: '',
            costCenter: '',
            remark: '',
        }];
        consumption.mannualNo = '';
        consumption.userId = '';
        consumption.departmentId = '';
        consumption.employeeId = '';
        $scope.usertemp = '';
        $scope.departmenttemp = '';
        $scope.employeetemp = '';
    };

    //validation on save
    this.ValidateConsumptionIssue = function ($scope) {
        debugger;
        var IsValid = true;

        return IsValid;
    }
})