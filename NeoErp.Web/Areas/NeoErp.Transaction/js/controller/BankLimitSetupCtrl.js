/// <reference path="~/Scripts/_references.js" />
/// <reference path="../module/transactionModule.js" />

transactionModule.controller('BankLimitSetupCtrl', function ($scope, $filter, $window, BankLimitSetupservice, $routeParams, $rootScope) {
    $scope.LoanInfo = {
        BankCode: '',
        TransactionNumber: '',
        Type: '',
    };
    $scope.BankDDL = [];
    $scope.LoanDetail = [];
    $scope.TypeDDL = {
        optionLabel: "--Select Type--",
        dataTextField: "TypeName",
        dataValueField: "TypeId",
        dataSource: [{ TypeName: 'Funded', TypeId: 'F' },
            { TypeName: 'Non-Funded', TypeId: 'N' }
        ]
    };
    $scope.FCatagoriesDDL = {
        optionLabel: "--Select Category--",
        dataTextField: "CategoryName",
        dataValueField: "CategoryName",
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: "/api/Bank/GetAllCategories",
                },
            },
            schema: {
                parse: function (response) {
                    debugger;
                    var result = [];
                    for (var i = 0; i < response.length; i++) {
                        if (response[i].CategoryType == 'F')
                            result.push(response[i]);
                    }
                    return result;
                }
            },
        }
    };
    $scope.NCatagoriesDDL = {
        optionLabel: "--Select Category--",
        dataTextField: "CategoryName",
        dataValueField: "CategoryName",
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: "/api/Bank/GetAllCategories",
                },
            },
            schema: {
                parse: function (response) {
                    debugger;
                    var result = [];
                    for (var i = 0; i < response.length; i++) {
                        if (response[i].CategoryType == 'N')
                            result.push(response[i]);
                    }
                    return result;
                }
            },
        }
    };
    $scope.BankDDL = {
        optionLabel: "--Select Bank--",
        dataTextField: "BankName",
        dataValueField: "BankCode",
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: "/api/Bank/GetAllBanks",
                },
            },
        },
    };
    $scope.addLoan = function () {
        debugger;
        var i = $scope.LoanDetail.length;
        var available = $scope.LoanDetail;
        $scope.LoanDetail = [];
        $scope.LoanDetail.push({
            Sn: i + 1,
            LoanCategory: '',
            LimitAmount: '',
            EffectiveDate: '',
            ExpiryDate: '',
            Remarks: '',
        });
        for (var i = 0; i < available.length; i++) {
            var item = available[i];
            $scope.LoanDetail.push(item);
        }
    };
    $scope.removeLoan = function (index) {
        $scope.LoanDetail.splice(index, 1);
        for (var i = 0; i < $scope.LoanDetail.length; i++) {
            $scope.LoanDetail[i].Sn = $scope.LoanDetail.length - i;
        }
    };
    $scope.$on('$routeChangeSuccess', function () {
        if (typeof ($routeParams.transNo) != "undefined") {
            $scope.LoanInfo.Type = $routeParams.type;
            var type = $routeParams.type == 'Funded' ? 'F' : $routeParams.type == 'Non-Funded' ? 'N' : 'B';
            $scope.GetLimit($routeParams.transNo, type);
            return;
        }
        $scope.addLoan();
    });
    $scope.ResetBankTransactionSetup = function (form) {
        debugger;
        form.$setPristine();
        form.$setUntouched();
        $scope.LoanInfo = {
            BankCode: '',
            TransactionNumber: '',
            Type: '',
        };
        $scope.LoanDetail = [];
        $scope.addLoan();
    };
    $scope.SaveBankLimit = function (form) {
        debugger;
        if (validateForm()) {
            //BankLimitSetupservice.SaveBankLimit($scope.LoanInfo, $scope.LoanDetail).then(function (status) {
            //    displayPopupNotification("CR. Limit saved.", "success");
            //    $scope.ResetBankTransactionSetup(form);
            //}, function (error) {
            //    displayPopupNotification("Error on saving.Please fix the error and Retry. \n Message:" + error, "error");
            //});
        }
    }
    validateForm = function () {
        debugger;
        var form = $scope.BankLimitSetupForm;
        if (!form.$valid) {
            displayPopupNotification("Input fileds are not valid, Please review form and Retry.", "warning");
            return false;
        }
        var loans = $scope.LoanDetail;
        if (loans.length < 1) {
            displayPopupNotification("At least one loan should be entered, Please review loan list and Retry.", "warning");
            return false;
        }
        for (var i = 0; i < loans.length; i++) {
            if (loans[i].LoanCategory == '' || loans[i].LimitAmount == '' || loans[i].EffectiveDate == '' || loans[i].ExpiryDate == '') {
                displayPopupNotification("Input fileds are not valid, Please review form and Retry.", "warning");
                return false;
            }
        }
        return true;
    }
    $scope.GetLimit = function (transNo,type) {
        BankLimitSetupservice.GetLimit(transNo,type).then(function (response) {
            $scope.LoanInfo.BankCode = response.data.BankCode;
            $scope.LoanInfo.TransactionNumber = response.data.TransactionNumber;
            $scope.LoanInfo.Type = response.data.Type;
            debugger;
            for (var i = 0; i < response.data.LoanList.length; i++) {
                response.data.LoanList[i].EffectiveDate = $filter('date')(response.data.LoanList[i].EffectiveDate, "MM/dd/yyyy");
                response.data.LoanList[i].ExpiryDate = $filter('date')(response.data.LoanList[i].ExpiryDate, "MM/dd/yyyy");
            }
            $scope.LoanDetail = response.data.LoanList;
        }, function (error) {
            displayPopupNotification("Cannot retrieve data. \n Message:" + error, "error");
        });
    }
});

transactionModule.controller("KendoCtrl", function ($scope, $filter, KendoCtrlservice, $routeParams) {
    $scope.OpenWindow = function () {
        var DlgOptions = {
            width: 500,
            pinned: true,
            resizable: false,
            position: {
                left: "35%",
                top: "10%",
            },
            //height: 90,
            visible: false,
            title: "Select",
            draggable: false,
            actions: ["Close"]
        };
        $scope.dlgWindow.setOptions(DlgOptions);
        $scope.dlgWindow.open();
    }
    $scope.$on('$routeChangeSuccess', function () {
        $scope.OpenWindow();
        var DlgRenewOptions = {
            width: 500,
            pinned: true,
            resizable: false,
            position: {
                left: "35%",
                top: "10%",
            },
            //height: 133,
            visible: false,
            title: "Select",
            draggable: false,
            actions: ["Close"]
        };
        $scope.dlgRenewWindow.setOptions(DlgRenewOptions);
    });
    $scope.DrawMainGrid = function () {
        var type = $("input[name='radioLimitType']:checked").val();
        if (typeof ($scope.mainGridOptions) != "undefined") {
            $scope.dlgWindow.close();
            $("#mainGrid").data("kendoGrid").dataSource.transport.options.read.url = "/api/Bank/GetAllBankLimit?type=" + type;
            $("#mainGrid").data("kendoGrid").dataSource.read();
            return;
        }
        $scope.dlgWindow.close();
        Draw(type);
    }
    function Draw(type) {
        $scope.mainGridOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: "/api/Bank/GetAllBankLimit?type=" + type
                },
                pageSize: 100,
                serverPaging: false,
                serverSorting: false,
                schema: {
                    parse: function (items) {
                        debugger;
                        var NewDataSource = [];
                        for (var i = 0; i < items.length; i++) {
                            for (j = 0; j < items[i].LoanList.length; j++) {
                                var newItem = {
                                    Type: items[i].Type == 'F' ? 'Funded' : 'Non-Funded',
                                    BankCode: items[i].BankCode,
                                    BankName: items[i].BankName,
                                    TransactionNumber: items[i].TransactionNumber,
                                    Id: items[i].LoanList[j].Id,
                                    Sn: items[i].LoanList[j].Sn,
                                    LoanCategory: items[i].LoanList[j].LoanCategory,
                                    LimitAmount: items[i].LoanList[j].LimitAmount,
                                    EffectiveDate: $filter('date')(items[i].LoanList[j].EffectiveDate, "MM/dd/yyyy"),
                                    ExpiryDate: $filter('date')(items[i].LoanList[j].ExpiryDate, "MM/dd/yyyy"),
                                    Remarks: items[i].LoanList[j].Remarks
                                }
                                NewDataSource.push(newItem);
                            }
                        }
                        return NewDataSource;
                    }
                }
            },
            sortable: true,
            //selectable: true,
            height: window.innerHeight - 150,
            pageable: {
                refresh: true,
                pageSizes: [50, 100, 200, 500, 1000],
                buttonCount: 5
            },
            groupable: true,
            resizable: true,
            filterable: {
                extra: false,
                operators: {
                    number: {
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        gte: "is greater than or equal to	",
                        gt: "is greater than",
                        lte: "is less than or equal",
                        lt: "is less than",
                    },
                    string: {

                        eq: "Is equal to",
                        neq: "Is not equal to",
                        startswith: "Starts with	",
                        contains: "Contains",
                        doesnotcontain: "Does not contain",
                        endswith: "Ends with",
                    },
                    date: {
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        gte: "Is after or equal to",
                        gt: "Is after",
                        lte: "Is before or equal to",
                        lt: "Is before",
                    }
                }
            },
            columnMenu: true,
            columnMenuInit: function (e) {
                wordwrapmenu(e);
                checkboxItem = $(e.container).find('input[type="checkbox"]');
            },
            columns: [{
                //field: "Sn",
                title: "S.N.",
                width: '40px',
                template: "#:Sn#"
            }, {
                template: "#:Type#",
                title: "Type",
                //field: "Type",
                width: '80px',
            }, {
                field: "TransactionNumber",
                title: "Transaction No.",
                width: '100px',
            }, {
                field: "BankName",
                title: "Bank Name",
                width: "300px"
            }, {
                field: "LoanCategory",
                title: "Loan Category",
            }, {
                field: "LimitAmount",
                title: "Limit Amount",
                attributes: { style: "text-align:right;" }
            }, {
                field: "EffectiveDate",
                title: "Effective Date",
            }, {
                field: "ExpiryDate",
                title: "Expiry Date",
            }, {
                field: "Remarks",
                title: "Remarks",
            }, {
                //field: "TransactionNumber",
                title: "Actions",
                template: function (dataItem) {
                    return kendo.template($("#action-template").html())(dataItem);
                },
                width: "120px",
            }]
        };
    }
    $scope.Renew = function (Id, date) {
        debugger;
        if (typeof (date) == "undefined") {
            displayPopupNotification("Date must be selected", "warning");
            return;
        }
        KendoCtrlservice.UpdateLimit(Id, date).then(function (result) {
            debugger;
            displayPopupNotification("Updated", "success");
            angular.element('#renewModal').modal('hide');
            $("#mainGrid").data("kendoGrid").dataSource.read();
            //$(".k-i-refresh").trigger('click');
        }, function (ex) { });
        $scope.dlgRenewWindow.close();
    }
    $scope.DetailGrid = function (sn, transactionNo, type) {
        type = type == 'Funded' ? 'F' : type == 'Non-Funded' ? 'N' : 'B';
        if (typeof ($scope.DetailGridOptions) != "undefined") {
            $("#detailGrid").data("kendoGrid").dataSource.transport.options.read.url = "/api/Bank/GetHistory?transNo=" + transactionNo + "&sn=" + sn + "&type=" + type;
            $("#detailGrid").data("kendoGrid").dataSource.read();
            return;
        }
        $scope.DetailGridOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: "/api/Bank/GetHistory?transNo=" + transactionNo + "&sn=" + sn + "&type=" + type,
                    dataType: "json", // "jsonp" is required for cross-domain requests; use "json" for same-domain requests
                },
                pageSize: 100,
                serverPaging: false,
                serverSorting: false,
                schema: {
                    parse: function (items) {
                        debugger;
                        var NewDataSource = [];
                        for (var i = 0; i < items.length; i++) {
                            for (j = 0; j < items[i].LoanList.length; j++) {
                                var newItem = {
                                    Type: items[i].Type == 'F' ? 'Funded' : 'Non-Funded',
                                    BankCode: items[i].BankCode,
                                    BankName: items[i].BankName,
                                    TransactionNumber: items[i].TransactionNumber,
                                    TransactionDate: $filter('date')(items[i].TransactionDate, "MM/dd/yyyy"),
                                    Id: items[i].LoanList[j].Id,
                                    Sn: items[i].LoanList[j].Sn,
                                    LoanCategory: items[i].LoanList[j].LoanCategory,
                                    LimitAmount: items[i].LoanList[j].LimitAmount,
                                    EffectiveDate: $filter('date')(items[i].LoanList[j].EffectiveDate, "MM/dd/yyyy"),
                                    ExpiryDate: $filter('date')(items[i].LoanList[j].ExpiryDate, "MM/dd/yyyy"),
                                    Remarks: items[i].LoanList[j].Remarks
                                }
                                NewDataSource.push(newItem);
                            }
                        }
                        return NewDataSource;
                    }
                }
            },
            sortable: true,
            //selectable: true,
            height: window.innerHeight - 150,
            pageable: {
                refresh: true,
                pageSizes: [50, 100, 200, 500, 1000],
                buttonCount: 5
            },
            groupable: true,
            resizable: true,
            filterable: {
                extra: false,
                operators: {
                    number: {
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        gte: "is greater than or equal to	",
                        gt: "is greater than",
                        lte: "is less than or equal",
                        lt: "is less than",
                    },
                    string: {

                        eq: "Is equal to",
                        neq: "Is not equal to",
                        startswith: "Starts with	",
                        contains: "Contains",
                        doesnotcontain: "Does not contain",
                        endswith: "Ends with",
                    },
                    date: {
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        gte: "Is after or equal to",
                        gt: "Is after",
                        lte: "Is before or equal to",
                        lt: "Is before",
                    }
                }
            },
            columnMenu: true,
            columnMenuInit: function (e) {
                wordwrapmenu(e);
                checkboxItem = $(e.container).find('input[type="checkbox"]');
            },
            columns: [{
                //field: "Sn",
                title: "S.N.",
                width: '40px',
                template: "#:Sn#"
            }, {
                template: "#:Type#",
                title: "Type",
                width: '80px',
            }, {
                field: "TransactionNumber",
                title: "Transaction No.",
                width: '100px',
            }, {
                field: "BankName",
                title: "Bank Name",
                width: "300px"
            }, {
                field: "LoanCategory",
                title: "Loan Category",
            }, {
                field: "LimitAmount",
                title: "Limit Amount",
                attributes: { style: "text-align:right;" }
            }, {
                field: "EffectiveDate",
                title: "Effective Date",
            }, {
                field: "ExpiryDate",
                title: "Expiry Date",
            }, {
                field: "Remarks",
                title: "Remarks"
            }]
        };
    }
});

transactionModule.controller("CategoryCtrl", function ($scope, $filter, CategoryCtrlservice, $routeParams) {
    $scope.CategorySetup = {};
    $scope.AllCategories = [];
    $scope.$on('$routeChangeSuccess', function () {
        $scope.GetAllCategories();
        var DlgOptions = {
            width: 400,
            pinned: true,
            resizable: false,
            position: {
                left: "35%",
                top: "10%",
            },
            //height: 80,
            visible: false,
            title: "Confirm",
            draggable: false,
            actions: ["Close"]
        };
        $scope.dlgWindow.setOptions(DlgOptions);
    });
    $scope.ResetCategory = function (form) {
        debugger;
        $scope.CategorySetup = [];
        $scope.CategorySetup.CategoryType = '';
        form.$setPristine();
        form.$setUntouched();
    }
    $scope.GetAllCategories = function () {
        CategoryCtrlservice.AllCategories().then(function (obj) {
            debugger;
            $scope.AllCategories = obj.data;
        }, function (ex) { });
    }
    $scope.GetCategory = function (cat) {
        $scope.CategorySetup = cat;
        //CategoryCtrlservice.GetCategory(id).then(function (obj) {
        //    $scope.CategorySetup = obj.data;
        //}, function (ex) { });
    }
    $scope.DeleteCategory = function (id, form) {
        $scope.dlgWindow.open();
        $("#btn-okay").on("click", function () {
            debugger;
            CategoryCtrlservice.Delete(id).then(function (obj) {
                displayPopupNotification("Category deleted.", "success");
                $scope.ResetCategory(form);
            }, function (ex) {
                displayPopupNotification("Error on deleting.Please fix the error and Retry. \n Message:" + error, "error");
            });
            $scope.dlgWindow.close();
        });
        $("#btn-cancel").on("click", function () {
            $scope.dlgWindow.close();
        });
    }
    $scope.SaveCategory = function (form, valid) {
        debugger;
        if (valid) {
            CategoryCtrlservice.Save($scope.CategorySetup).then(function (obj) {
                displayPopupNotification("Category added.", "success");
                $scope.ResetCategory(form);
            }, function (ex) {
                displayPopupNotification("Error on deleting.Please fix the error and Retry. \n Message:" + error, "error");
            });
        }
        else
            displayPopupNotification("Input fields are not valid. Please review form and try again.", "error");
    }
});