DTModule.controller('accountSetupTreeCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter, hotkeys, $timeout, $compile) {
    $scope.result = false;
    $scope.saveupdatebtn = "Save";
    $scope.accountsArr;
    $scope.savegroup = false;
    $scope.editFlag = "N";
    $scope.newrootinsertFlag = "Y";
    $scope.treenodeselected = "N";
    $scope.treeSelectedAccountCode = "";
    var dataFillDefered = $.Deferred();
    $scope.accountsetup =
    {
        TRANSACTION_TYPE: "",
        PREFIX_TEXT: "",
        ACC_CODE: "",
        ACC_EDESC: "",
        ACC_NDESC: "",
        TPB_FLAG: "",
        ACC_TYPE_FLAG: "T",
        MASTER_ACC_CODE: "",
        PRE_ACC_CODE: "",
        COMPANY_CODE: "",
        CREATED_BY: "",
        CREATED_DATE: "",
        DELETED_FLAG: "",
        SYN_ROWID: "",
        DELTA_FLAG: "",
        FREEZE_FLAG: "N",
        CURRENT_BALANCE: "",
        LIMIT: "",
        MODIFY_DATE: "",
        ACC_NATURE: "",
        BRANCH_CODE: "",
        SHARE_VALUE: "",
        MODIFY_BY: "",
        IND_VAT_FLAG: "N",
        BANK_ACCOUNT_NO: "",
        PRINTING_FLAG: "N",
        ACC_SNAME: "",
        TEL_NO: "",
        IND_TDS_FLAG: "N",
        ACC_ID: "",
        MOBILE_NO: "",
        EMAIL_ID: "",
        LINK_ID: "",
        CONTACT_PERSON: "",
        GROUP_START_CODE: "",
        GROUP_END_CODE: "",
        DEALER_FLAG: "",
        PARENT_ACC_CODE: "",
        MATURITY_DATE: "",
        ACC_OPENING_DATE: "",
        ACC_INT_SETUP: [],
        PRIOR_ALERT_DAYS: "",
        LOAN_TERMS: "M",
        BS_CODE: "",
        PL_CODE: ""
    }

    $scope.bankaccountdetail = {
        CONTACT_PERSON: "",
        DEPARTMENT: "",
        DESIGNATION: "",
        ADDRESS: "",
        MOBILE_NO: "",
        PHONE_NO: "",
        EMAIL_ID: ""
    }
    /* $scope.bankdetailArr = [];*/
    $scope.bankdetailArr = $scope.bankaccountdetail

    $scope.accountsArr = $scope.accountsetup;
    var accountCodeUrl = window.location.protocol + "//" + window.location.host + "/api/Purchase/GetAccountsList";
    //$scope.accountGroupDataSource = new kendo.data.DataSource({
    //    transport: {
    //        read: {
    //            url: accountCodeUrl,
    //        }
    //    }
    //});
    //Prem Prakash Dhakal Correction of Code
    $scope.accountGroupDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: accountCodeUrl,
                dataType: "json",
                type: "GET"
            }
        }
    });
    var accountCodeWithChildUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAccountCodeWithChild";
    $scope.accountGroupOptions = {
        dataTextField: "ACC_EDESC",
        dataValueField: "ACC_CODE",
        maxSelectedItems: 1,
        valuePrive: true,
        autoClose: true,
        //headerTemplate: '<div class="col-md-offset-3"><strong>Select Customer acc...</strong></div>',
        //placeholder: "Select Customer acc...",

        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: accountCodeWithChildUrl,
                    dataType: "json"
                }
            },
            schema: {
                data: function (response) {
                    return response;
                },
                model: {
                    id: "ACC_CODE",
                    fields: {
                        ACC_EDESC: { type: "string" },
                        ACC_CODE: { type: "string" }
                    }
                }
            }
        }
    }

    // PL/BS dropdown
    var bsCustomSetupUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetBSCustomSetupList";
    $scope.bsdropdownlist = {
        dataTextField: "BS_EDESC",
        dataValueField: "BS_CODE",
        optionLabel: "--Select--",
        dataSource: {
            transport: {
                read: {
                    url: bsCustomSetupUrl,
                    dataType: "json"
                }
            }
        }
    };
    var plCustomSetupUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetPLCustomSetupList";
    $scope.pldropdownlist = {
        dataTextField: "PL_EDESC",
        dataValueField: "PL_CODE",
        optionLabel: "--Select--",
        dataSource: {
            transport: {
                read: {
                    url: plCustomSetupUrl,
                    dataType: "json"
                }
            }
        }
    };

    $scope.onChangeGroupPLBS = function (kendoEvent) {
        debugger;
        if (kendoEvent && kendoEvent.sender) {
            var selectedItem = kendoEvent.sender.dataItem();
            if (selectedItem) {
                if ($scope.accountsArr.TPB_FLAG === 'B') {
                    $scope.accountsArr.BS_CODE = selectedItem.BS_CODE;
                    $scope.accountsArr.PL_CODE = '';
                    console.log('BS_CODE set to:', selectedItem.BS_CODE);
                } else if ($scope.accountsArr.TPB_FLAG === 'P') {
                    $scope.accountsArr.PL_CODE = selectedItem.PL_CODE;
                    $scope.accountsArr.BS_CODE = '';
                    console.log('PL_CODE set to:', selectedItem.PL_CODE);
                }
            }
        }
    };
    //show  and hide text box  
    //$scope.onCloseInit = function () {
    //    $scope.NatureAccCodeOnChange(null);
    //}
    $scope.showDefaultNatureAccTab = true;
    $scope.NatureAccCodeOnChange = function (kendoEvent) {
        debugger
        // Reset all values
        $scope.accountsArr.TPB_FLAG = '';
        //$scope.accountsArr.TRANSACTION_TYPE = '';

        // Show/hide tabs based on account nature
        $scope.BankTabResult = ['LC', 'AC'].includes($scope.accountsArr.ACC_NATURE);
        if (!$scope.BankTabResult) {
            $("#tab_1").addClass("active");
            $("#tab_2").removeClass("active");
            $("#li_tab_1").addClass("active");
            $("#li_tab_2").removeClass("active");
        }

        $scope.share = ($scope.accountsArr.ACC_NATURE === 'LA');

        //Add Code Prem Prakash Dhakal
        var selectedItem = $scope.accountsArr.ACC_NATURE;

        // Map of keys to values
        const balanceSheetKeys = ['AA', 'AB', 'AC', 'AD', 'AE', 'AF', 'SD', 'SE', 'LA', 'LB', 'LC', 'LD', 'PL', 'PV'];
        const profitLossKeys = ['EA', 'EB', 'EC', 'ED', 'SA', 'SB', 'SC', 'IA', 'IB', 'IC'];
        const creditKeys = ['IA', 'IB', 'IC', 'LA', 'LB', 'LC', 'LD', 'PL', 'PV'];
        const debitKeys = ['AA', 'AB', 'AC', 'AD', 'AE', 'AF', 'EA', 'EB', 'EC', 'ED', 'SA', 'SB', 'SC', 'SD', 'SE']
        if (balanceSheetKeys.includes(selectedItem)) {
            $scope.accountsArr.TPB_FLAG = 'B';
        }
        if (profitLossKeys.includes(selectedItem)) {
            $scope.accountsArr.TPB_FLAG = 'P';
        }
        if (creditKeys.includes(selectedItem)) {
            $scope.accountsArr.TRANSACTION_TYPE = 'CR';
        }
        if (debitKeys.includes(selectedItem)) {
            $scope.accountsArr.TRANSACTION_TYPE = 'DR';
        }
    };

    //const accNatureValidate = $scope.accountsArr.ACC_NATURE;
    //if (accNatureValidate === "LC" || accNatureValidate === "AC") {
    //    /*$scope.result = true;*/
    //    setTimeout(function () {
    //        $('a[href="#tab_2"]').tab('show');
    //    }, 0);
    //} else {
    //   /* $scope.result = false;*/
    //    setTimeout(function () {
    //        $('a[href="#tab_1"]').tab('show');
    //    }, 0);
    //}


    //$scope.NatureAccCodeOnChange = function (kendoEvent) {
    //    //var selectedItem = kendoEvent.sender.dataItem();
    //    //if (!selectedItem || !selectedItem.key) {
    //    //    console.warn("Invalid selection");
    //    //    return;
    //    //}
    //    if ($scope.accountsArr.ACC_NATURE == "LC" || $scope.accountsArr.ACC_NATURE == "AC") {
    //        $scope.BankTabResult = true;
    //    }
    //    else {
    //        $scope.BankTabResult = false;
    //    }
    //    if ($scope.accountsArr.ACC_NATURE == "LA") {
    //        $scope.share = true;
    //    }
    //    else {
    //        $scope.share = false;
    //    }
    //    //if (selectedItem.key == 'AA') {

    //    //}
    //    //if (selectedItem.key == 'AB') {

    //    //}
    //    //if (selectedItem.key == 'AC') {

    //    //}
    //    //const accNatureValidate = $scope.accountsArr.ACC_NATURE;
    //    //if (accNatureValidate === "LC" || accNatureValidate === "AC") {
    //    //    /*$scope.result = true;*/
    //    //    setTimeout(function () {
    //    //        $('a[href="#tab_2"]').tab('show');
    //    //    }, 0);
    //    //} else {
    //    //   /* $scope.result = false;*/
    //    //    setTimeout(function () {
    //    //        $('a[href="#tab_1"]').tab('show');
    //    //    }, 0);
    //    //}


    //    //Add Prem Prakash Dhakal
    //    //var selectedItem = kendoEvent.sender.dataItem();
    //    //if (selectedItem.key === "AC" || selectedItem.text === "Bank Account") {
    //    //    $scope.result = true;
    //    //} 
    //}
    //$scope.NatureAccCodeOnChange(null);
    var getAccountCodeByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAccountCode";
    $scope.acctreeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: getAccountCodeByUrl,
                type: 'GET',
                data: function (data, evt) {
                }
            },
        },
        schema: {
            parse: function (data) {
                return data;
            },
            //select: onSelect,
            model: {
                id: "MASTER_ACC_CODE",
                parentId: "PRE_ACC_CODE",
                children: "Items",
                fields: {
                    ACC_CODE: { field: "ACC_CODE", type: "string" },
                    ACC_EDESC: { field: "ACC_EDESC", type: "string" },
                    parentId: { field: "PRE_ACC_CODE", type: "string", defaultValue: "00" },
                }
            }
        }
    });

    function deepClone(obj) {
        return JSON.parse(JSON.stringify(obj));
    }

    function filterNodes(nodes, searchText) {
        const result = [];

        angular.forEach(nodes, function (node) {
            const children = node.Items ? (node.Items.data ? node.Items.data() : node.Items) : [];
            const filteredChildren = filterNodes(children, searchText);

            let isMatch = false;
            let nodeText = "";
            let newNode = {};

            newNode = deepClone(node);

            if (newNode.ACC_CODE && newNode.ACC_EDESC) {
                nodeText = (newNode.ACC_EDESC || "").toLowerCase();
                isMatch = nodeText.includes(searchText);
            } else {
                newNode.ACC_CODE = node.AccountId;
                newNode.ACC_EDESC = node.AccountName;
                nodeText = (newNode.AccountName || "").toLowerCase();
                isMatch = nodeText.includes(searchText);
            }

            if (isMatch || filteredChildren.length > 0) {
                newNode.Items = filteredChildren;
                result.push(newNode);
            }
        });

        return result;
    }

    $scope.filterTreeView = function () {
        const searchText = ($scope.txtMainGroupSearchString || '').toLowerCase();

        if (!searchText) {
            $scope.tree.setDataSource($scope.acctreeData);
            return;
        }

        const pristine = $scope.acctreeData._pristineData || $scope.acctreeData.data();
        const filteredData = filterNodes(pristine, searchText);

        const filteredDataSource = new kendo.data.HierarchicalDataSource({
            data: filteredData,
            schema: {
                model: {
                    id: "itemCode",
                    children: "Items"
                }
            }
        });

        $scope.tree.setDataSource(filteredDataSource);

        setTimeout(() => {
            $scope.tree.expand(".k-item");
        }, 100);
    };

    //Share Capital (balancesheet and credit)
    var SCNature = [
        { key: "LA", text: "Capital Account" },
        { key: "LB", text: "Current Liabilities" },
        { key: "LC", text: "Loan (Liability)" },
        { key: "LD", text: "Sundry Creditors" },
        { key: "PL", text: "Profit & Loss" },
        { key: "PV", text: "Provision" }
    ];
    //Assets(debit abd balancesheet)
    var FANature = [
        { key: "AA", text: "Fixed Assets" },
        { key: "AA", text: "Capital W.I.P" }
    ];
    //Current Assets(debit and balancesheet)
    var CANature = [
        { key: "AB", text: "Cash In Hand" },
        { key: "AC", text: "Bank Account" },
        { key: "AD", text: "Current Assets" },
        { key: "AE", text: "Sundry Debtors" },
        { key: "AF", text: "Advance" }
        //{ key: "AF", text: " Staff Advance" }
    ];
    //Salaes and Revenue(Credit and Profit & loss)
    var SRNature = [
        { key: "IA", text: "Sales Accounts" },
        { key: "IB", text: "Direct Income" },
        { key: "IC", text: "Indirect Income" }
    ];
    //Manufacturing Expences(debit and profit & loss)
    var MENature = [
        { key: "EA", text: "Manufacturing Ex" },
        { key: "EB", text: "Direct Expenses" },
        { key: "EC", text: "InDirect Expense" },
        { key: "ED", text: "Depreciation" }
    ];
    //Stock and Procurement(debit and profit & loss)
    SPNature = [
        { key: "SE", text: "Inventory" },
        { key: "SA", text: "Opening Stock" },
        { key: "SB", text: "Purchase Accoun" },
        { key: "SC", text: "Stock Transfer" },
        { key: "SD", text: "Closing Stock" }
    ];
    //Reserve & surplus
    var RStureArr = [
        { key: "AA", text: "Fixed Assets" },
        { key: "AB", text: "Cash In Hand" },
        { key: "AC", text: "Bank Account" },
        { key: "AD", text: "Current Assets" },
        { key: "AE", text: "Sundry Debtors" },
        { key: "AF", text: "Advance" },
        { key: "EA", text: "Manufacturing Ex" },
        { key: "EB", text: "Direct Expenses" },
        { key: "EC", text: "InDirect Expense" },
        { key: "ED", text: "Depreciation" },
        { key: "SA", text: "Opening Stock" },
        { key: "SB", text: "Purchase Accoun" },
        { key: "SC", text: "Stock Transfer" },
        { key: "SD", text: "Closing Stock" },
        { key: "SE", text: "Inventory" },
        { key: "LA", text: "Capital Account" },
        { key: "LB", text: "Current Liabilities" },
        { key: "LC", text: "Loan (Liability)" },
        { key: "LD", text: "Sundry Creditors" },
        { key: "PL", text: "Profit & Loss" },
        { key: "PV", text: "Provision" },
        { key: "IA", text: "Sales Accounts" },
        { key: "IB", text: "Direct Income" },
        { key: "IC", text: "Indirect Income" }
    ];
    $scope.acconDataBound = function () {
        $('#accounttree').data("kendoTreeView").expand('.k-item');
    }
    $scope.getDetailByAccCode = function (accId) {
        var getAccountdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getAccountDetailsByAccCode?accCode=" + accId;
        $http({
            method: 'GET',
            url: getAccountdetaisByUrl
        }).then(function successCallback(response) {
            debugger
            var accNature = response.data.DATA.ACC_NATURE;
            if (/*accNature == "LA" || accNature == "LB" || accNature == "LC" || */accNature == "AC" || accNature == "AB" || accNature == "AC" || accNature == "AE" || accNature == "AD") {
                //$scope.accountsArr.TRANSACTION_TYPE = $("#cr").val();
                //$scope.accountsArr.TPB_FLAG = $("#balancesheet").val();
                $scope.natureaccountOptions = {
                    dataSource: CANature,
                    dataTextField: "text",
                    dataValueField: "key",
                    optionLabel: "--Select--"
                }
            }
            else if (accNature == "PL") {
                //$scope.accountsArr.TRANSACTION_TYPE = $("#cr").val();
                //$scope.accountsArr.TPB_FLAG = $("#balancesheet").val();
                $scope.natureaccountOptions = {
                    dataSource: RStureArr,
                    dataTextField: "text",
                    dataValueField: "key",
                    optionLabel: "--Select--"
                }
            }
            else if (accNature == "AA") {
                //$scope.accountsArr.TRANSACTION_TYPE = $("#dr").val();
                //$scope.accountsArr.TPB_FLAG = $("#balancesheet").val();
                $scope.natureaccountOptions = {
                    dataSource: FANature,
                    dataTextField: "text",
                    dataValueField: "key",
                    optionLabel: "--Select--"
                }
            }
            else if (accNature == "AD") {
                //$scope.accountsArr.TRANSACTION_TYPE = $("#dr").val();
                //$scope.accountsArr.TPB_FLAG = $("#balancesheet").val();
                $scope.natureaccountOptions = {
                    dataSource: CANature,
                    dataTextField: "text",
                    dataValueField: "key",
                    optionLabel: "--Select--"
                }
            }
            else if (accNature == "IA" || accNature == "IC") {
                //$scope.accountsArr.TRANSACTION_TYPE = $("#cr").val();
                //$scope.accountsArr.TPB_FLAG = $("#profloss").val();
                $scope.natureaccountOptions = {
                    dataSource: SRNature,
                    dataTextField: "text",
                    dataValueField: "key",
                    optionLabel: "--Select--"
                }
            }
            else if (accNature == "EA" || accNature == "EC" || accNature == "EB") {
                //$scope.accountsArr.TRANSACTION_TYPE = $("#dr").val();
                //$scope.accountsArr.TPB_FLAG = $("#profloss").val();
                $scope.natureaccountOptions = {
                    dataSource: MENature,
                    dataTextField: "text",
                    dataValueField: "key",
                    optionLabel: "--Select--"
                }
            }
            else if (accNature == "SE" || accNature == "SA" || accNature == "SB") {
                //$scope.accountsArr.TRANSACTION_TYPE = $("#dr").val();
                //$scope.accountsArr.TPB_FLAG = $("#profloss").val();
                $scope.natureaccountOptions = {
                    dataSource: SPNature,
                    dataTextField: "text",
                    dataValueField: "key",
                    optionLabel: "--Select--"
                }
            }
            //var natureWidget = $("#natureaccount").data("kendoDropDownList");
            //if (natureWidget) {
            //    natureWidget.unbind("change");
            //    natureWidget.bind("change", function (e) {
            //        $scope.$apply(function () {
            //            $scope.NatureAccCodeOnChange(e);
            //        });
            //    });
            //}
            $scope.BankTabResult = false;
            if (accNature == "AC") {
                $scope.BankTabResult = true;
            }
            //setAccNature()
        }, function errorCallback(response) {
        });
    }
    $scope.fillAccSetupForms = function (accId) {
        var getAccountdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getAccountDetailsByAccCode?accCode=" + accId;
        $http({
            method: 'GET',
            url: getAccountdetaisByUrl
        }).then(function successCallback(response) {
            $scope.accountsetup = response.data.DATA;
            $scope.accountsArr = [];
            $scope.accountsArr = $scope.accountsetup;

            // Auto-set TPB_FLAG based on BS_CODE or PL_CODE immediately after assignment
            $timeout(function () {
                if ($scope.accountsArr.BS_CODE && $scope.accountsArr.BS_CODE !== '') {
                    $scope.accountsArr.TPB_FLAG = 'B';
                    $('#balancesheet').prop('checked', true);
                    $('#profloss').prop('checked', false);
                    console.log('Auto-set TPB_FLAG to B because BS_CODE exists:', $scope.accountsArr.BS_CODE);
                } else if ($scope.accountsArr.PL_CODE && $scope.accountsArr.PL_CODE !== '') {
                    $scope.accountsArr.TPB_FLAG = 'P';
                    $('#profloss').prop('checked', true);
                    $('#balancesheet').prop('checked', false);
                    console.log('Auto-set TPB_FLAG to P because PL_CODE exists:', $scope.accountsArr.PL_CODE);
                }
                console.log('Final TPB_FLAG value:', $scope.accountsArr.TPB_FLAG);
            }, 0);

            $scope.accountsArr.ACC_NATURE = $scope.accountsArr.ACC_NATURE;
            $scope.accountsArr.ACC_NATURE = $scope.accountsetup.ACC_NATURE;
            //$scope.bankdetailArr.CONTACT_PERSON = $scope.accountsArr.CONTACT_PERSON;
            //$scope.bankdetailArr.PHONE_NO = $scope.accountsArr.TEL_NO;
            //$scope.bankdetailArr.MOBILE_NO = $scope.accountsArr.MOBILE_NO;
            //$scope.bankdetailArr.EMAIL_ID = $scope.accountsArr.EMAIL_ID;
            //$scope.bankdetailArr.LINK_ID = $scope.accountsArr.LINK_ID;
            //$scope.bankdetailArr.ADDRESS = $scope.accountsArr.ADDRESS;
            //$scope.bankdetailArr.DESIGNATION = $scope.accountsArr.DESIGNATION;
            //$scope.bankdetailArr.DEPARTMENT = $scope.accountsArr.DEPARTMENT;
            $scope.accountsArr.MASTER_ACC_CODE = $scope.accountsetup.MASTER_ACC_CODE;
            $scope.accountsArr.ACC_TYPE_FLAG = "N";
            $scope.accountsArr.ACC_CODE = $scope.accountsetup.ACC_CODE;
            $scope.accountsArr.ACC_OPENING_DATE = $scope.accountsetup.ACC_OPENING_DATE;
            $scope.accountsArr.OPENING_AMOUNT = $scope.accountsetup.OPENING_AMOUNT === 0 ? "" : $scope.accountsetup.OPENING_AMOUNT;
            $scope.accountsArr.OPENING_AMOUNT_TRANSACTION_TYPE = $scope.accountsetup.OPENING_AMOUNT_TRANSACTION_TYPE;
            $scope.accountsArr.MATURITY_DATE = $scope.accountsetup.MATURITY_DATE;

            if ($scope.accountsArr.ACC_OPENING_DATE) {
                let englishOpeningDate = $scope.dateToYYYYMMDD($scope.accountsArr.ACC_OPENING_DATE)
                $scope.accountsArr.ACC_OPENING_DATE = englishOpeningDate;
                $("#nepaliDate5").val(AD2BS(englishOpeningDate));
            } else {
                $("#englishdatedocument5").val();
            }

            $scope.accountsArr.MATURITY_DATE = $scope.accountsetup.MATURITY_DATE;
            if ($scope.accountsArr.MATURITY_DATE) {
                let englishMaturityDate = $scope.dateToYYYYMMDD($scope.accountsArr.MATURITY_DATE)
                $scope.accountsArr.MATURITY_DATE = englishMaturityDate;
                $("#nepaliDate6").val(AD2BS(englishMaturityDate));
            } else {
                $("#englishdatedocument6").val();
            }

            if ($scope.accountsArr.MATURITY_DATE && $scope.accountsArr.ACC_OPENING_DATE) {
                let mDate = moment($scope.accountsArr.MATURITY_DATE)
                let oDate = moment($scope.accountsArr.ACC_OPENING_DATE)
                $scope.accountsArr.MATURITY_DAYS = mDate.diff(oDate, 'days')
            }

            $scope.accountsArr.PRIOR_ALERT_DAYS = $scope.accountsetup.PRIOR_ALERT_DAYS;
            $scope.accountsArr.LOAN_TERMS = $scope.accountsetup.LOAN_TERMS;

            $scope.bankdetailArr = $scope.accountsetup.ACC_CONTACT_DETAIL;

            $scope.accountsArr.accountInterestSetup = angular.copy($scope.accountsetup.ACC_INT_SETUP || []);
            $scope.accountsArr.accountInterestSetup.forEach(function (item, index) {
                if (item.INT_DATE) {
                    try {
                        const englishDate = $scope.dateToYYYYMMDD(item.INT_DATE);
                        const nepaliDate = AD2BS(englishDate);
                        item.NEP_INT_DATE = nepaliDate;
                        let nepDateSelector = "#nepaliCalendarAccInterest_" + index;
                        $(nepDateSelector).val(nepaliDate)
                    } catch (e) {
                        item.NEP_INT_DATE = "";
                    }
                } else {
                    item.NEP_INT_DATE = "";
                }
            });
            $scope.renderDynamicInterestTable();

            if ($scope.editFlag == "Y") {
                $scope.groupAccTypeFlag = "N";
                $scope.accountsetup.ACC_TYPE_FLAG = "T";
                $scope.accountsArr.ACC_TYPE_FLAG = "T";
            }
            dataFillDefered.resolve();
            // this callback will be called asynchronously
            // when the response is available
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
    }
    //treeview on select
    $scope.accoptions = {
        loadOnDemand: false,
        select: function (e) {
            var currentItem = e.sender.dataItem(e.node);
            $('#accountGrid').removeClass("show-displaygrid");
            $("#accountGrid").html("");
            $($(this._current).parents('ul')[$(this._current).parents('ul').length - 1]).find('span').removeClass('hasTreeCustomMenu');
            $(this._current.context).find('span').addClass('hasTreeCustomMenu');
            $scope.accountsetup.ACC_CODE = currentItem.AccountId;
            $scope.accountsetup.ACC_EDESC = currentItem.AccountName;
            $scope.accountsetup.MASTER_ACC_CODE = currentItem.masterAccountCode;
            $scope.accountsetup.PRE_ACC_CODE = currentItem.PRE_ACC_CODE;
            $scope.accountsetup.ACC_TYPE_FLAG = currentItem.accounttypeflag;
            $scope.treeSelectedAccountCode = currentItem.AccountId;
            $scope.treenodeselected = "Y";
            $scope.newrootinsertFlag = "N";
        },
    };
    $scope.movescrollbar = function () {
        var element = $(".k-in");
        for (var i = 0; i < element.length; i++) {
            var selectnode = $(element[i]).hasClass("k-state-focused");
            if (selectnode) {
                $("#accounttree").animate({
                    scrollTop: (parseInt(i * 12))
                });
                break;
            }
        }
    }
    $scope.onContextSelect = function (event) {
        if ($scope.accountsetup.ACC_CODE == "")
            return displayPopupNotification("Select account.", "error");;
        $scope.saveupdatebtn = "Save";
        if (event.item.innerText.trim() == "Delete") {
            $scope.accountsArr.ACC_CODE = $scope.accountsetup.ACC_CODE;
            $scope.delete($scope.accountsArr.ACC_CODE, "");
        }
        else if (event.item.innerText.trim() == "Update") {
            dataFillDefered = $.Deferred();
            $scope.editFlag = "N"
            $scope.saveupdatebtn = "Update";
            $scope.clearFields();
            $scope.fillAccSetupForms($scope.treeSelectedAccountCode);
            $.when(dataFillDefered).then(function () {
                $timeout(function () {
                    var natureWidget = $("#natureaccount").data("kendoDropDownList");
                    if (natureWidget) {
                        natureWidget.unbind("change");
                        natureWidget.bind("change", function (e) {
                            $scope.$apply(function () {
                                $scope.NatureAccCodeOnChange(e);
                            });
                        });
                    }
                    $scope.BankTabResult = false;
                }, 0);

                $("#accountModal").modal("toggle");
                $timeout(function () {
                    setAccNature()
                }, 100);
                if ($scope.accountsetup.PARENT_ACC_CODE == "00" || $scope.accountsetup.PARENT_ACC_CODE == null) {
                    var tree = $("#masteraccountcode").data("kendoDropDownList");
                    tree.value('')
                }
                else {
                    var tree = $("#masteraccountcode").data("kendoDropDownList");
                    tree.value($scope.accountsetup.PARENT_ACC_CODE);
                }
                $("#accountModal").modal();
            })
        }
        else if (event.item.innerText.trim() == "Add") {
            $scope.dekh = true;
            dataFillDefered = $.Deferred();
            $scope.savegroup = true;
            if ($scope.accountsetup.ACC_TYPE_FLAG == "N") {
                $scope.fillAccSetupForms($scope.treeSelectedAccountCode);
                $scope.getDetailByAccCode($scope.treeSelectedAccountCode);
                $.when(dataFillDefered).then(function () {
                    $timeout(function () {
                        var natureWidget = $("#natureaccount").data("kendoDropDownList");
                        if (natureWidget) {
                            natureWidget.unbind("change");
                            natureWidget.bind("change", function (e) {
                                $scope.$apply(function () {
                                    $scope.NatureAccCodeOnChange(e);
                                });
                            });
                        }
                        // Ensure 'Under Group' shows the correct parent description on open (Update flow)
                        setUnderGroupDropdowns($scope.accountsetup.PARENT_ACC_CODE || $scope.accountsArr.MASTER_ACC_CODE || "");
                        $scope.BankTabResult = false;
                    }, 0);

                    $("#accountModal").modal("toggle");
                    $timeout(function () {
                        setAccNature()
                    }, 100);
                    $scope.clearFields();
                    var tree = $("#masteraccountcode").data("kendoDropDownList");
                    tree.value($scope.accountsetup.ACC_CODE)
                    $("#accountModal").modal();

                })
            }
            else {
                var tree = $("#accounttree").data("kendoTreeView");
                tree.dataSource.read();
                displayPopupNotification("Please select the account head first", "warning");
            }
        }
    }
    $scope.saveNewAccount = function (isValid) {
        debugger
        if ($scope.saveupdatebtn == "Save") {
            if ($scope.accountsArr.TRANSACTION_TYPE == null || $scope.accountsArr.TRANSACTION_TYPE == '' || $scope.accountsArr.TRANSACTION_TYPE == undefined || $scope.accountsArr.TRANSACTION_TYPE == 'undefined') {
                $scope.accountsArr.TRANSACTION_TYPE = $("#dr").val();
            }
            if ($scope.accountsArr.TPB_FLAG == null || $scope.accountsArr.TPB_FLAG == '' || $scope.accountsArr.TPB_FLAG == undefined || $scope.accountsArr.TPB_FLAG == 'undefined') {
                $scope.accountsArr.TPB_FLAG = $("#balancesheet").val();
            }
            var createUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewAccountHead";
            var model = {
                ACC_CODE: $scope.accountsArr.ACC_CODE,
                ACC_EDESC: $scope.accountsArr.ACC_EDESC,
                ACC_NDESC: $scope.accountsArr.ACC_NDESC,
                TRANSACTION_TYPE: $scope.accountsArr.TRANSACTION_TYPE,
                TPB_FLAG: $scope.accountsArr.TPB_FLAG,
                MASTER_ACC_CODE: $scope.accountsArr.MASTER_ACC_CODE,
                PRE_ACC_CODE: $scope.accountsArr.PRE_ACC_CODE,
                ACC_TYPE_FLAG: $scope.accountsArr.ACC_TYPE_FLAG,
                CURRENT_BALANCE: $scope.accountsArr.CURRENT_BALANCE,
                ACC_SNAME: $scope.accountsArr.ACC_SNAME,
                LIMIT: $scope.accountsArr.LIMIT == null ? "" : $scope.accountsArr.LIMIT,
                ACC_NATURE: $scope.accountsArr.ACC_NATURE,
                //ACC_NATURE:$scope.userselected,
                //ACC_NATURE: $("#natureaccount").data("kendoDropDownList").dataItem().key,
                BANK_ACCOUNT_NO: $scope.accountsArr.BANK_ACCOUNT_NO == null ? "" : $scope.accountsArr.BANK_ACCOUNT_NO,
                FREEZE_FLAG: $scope.accountsArr.FREEZE_FLAG == null ? "" : $scope.accountsArr.FREEZE_FLAG,
                PRINTING_FLAG: $scope.accountsArr.PRINTING_FLAG == null ? "" : $scope.accountsArr.PRINTING_FLAG,
                IND_TDS_FLAG: $scope.accountsArr.IND_TDS_FLAG == null ? "" : $scope.accountsArr.IND_TDS_FLAG,
                IND_VAT_FLAG: $scope.accountsArr.IND_VAT_FLAG == null ? "" : $scope.accountsArr.IND_VAT_FLAG,
                GROUP_START_CODE: $scope.accountsArr.GROUP_START_CODE,
                GROUP_END_CODE: $scope.accountsArr.GROUP_END_CODE,
                PREFIX_TEXT: $scope.accountsArr.PREFIX_TEXT,
                ACC_ID: $scope.accountsArr.ACC_ID,
                //CONTACT_PERSON: $scope.bankdetailArr.CONTACT_PERSON,
                //TEL_NO: $scope.bankdetailArr.PHONE_NO,
                //MOBILE_NO: $scope.bankdetailArr.MOBILE_NO,
                //EMAIL_ID: $scope.bankdetailArr.EMAIL_ID,
                LINK_ID: $scope.accountsArr.LINK_ID,
                SHARE_VALUE: $scope.accountsArr.SHARE_VALUE,
                IND_MDF_FLAG: $scope.accountsArr.IND_MDF_FLAG,
                MATURITY_DAYS: $scope.accountsArr.MATURITY_DAYS,
                PRIOR_ALERT_DAYS: $scope.accountsArr.PRIOR_ALERT_DAYS,
                LOAN_TERMS: $scope.accountsArr.LOAN_TERMS,
                ACC_CONTACT_DETAIL: $scope.bankdetailArr,
                BS_CODE: $scope.accountsArr.BS_CODE,
                PL_CODE: $scope.accountsArr.PL_CODE
            }

            let nepaliMaturityDate = $("#nepaliDate6").val();
            if (nepaliMaturityDate) {
                model.MATURITY_DATE = BS2AD(nepaliMaturityDate);
                //model.MATURITY_DATE = $scope.createDateFromString(BS2AD(nepaliMaturityDate));
            }

            let nepaliOpeningDate = $("#nepaliDate5").val();
            if (nepaliOpeningDate) {
                model.ACC_OPENING_DATE = BS2AD(nepaliOpeningDate);
                //model.ACC_OPENING_DATE = $scope.createDateFromString(BS2AD(nepaliOpeningDate));
            }

            let accountInterestSetup = $scope.accountsArr.accountInterestSetup || [];
            let accountInterestSetupNew = []
            angular.forEach(accountInterestSetup, function (value, index) {
                let nepDateSelector = "#nepaliCalendarAccInterest_" + index;
                let nepDateValue = $(nepDateSelector).val();

                if (nepDateValue) {
                    value.INT_DATE = BS2AD(nepDateValue)
                    //if (typeof value.INT_DATE === 'string' && value.INT_DATE !== "") {
                    //    value.INT_DATE = $scope.createDateFromString(value.INT_DATE);
                    //}
                    delete value.NEP_INT_DATE;
                    accountInterestSetupNew.push(value);
                }
            });
            model.ACC_INT_SETUP = accountInterestSetupNew;


            if (!isValid) {
                displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
                return;
            }
            $http({
                method: 'POST',
                url: createUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "INSERTED") {
                    $scope.accountsArr = [];
                    //if ($scope.accountsetup.ACC_TYPE_FLAG !== "T") {

                    //}
                    var grid = $("#kGrid").data("kendo-grid");
                    if (grid != undefined) {
                        grid.dataSource.read();
                    }
                    var ddl = $("#masteraccountcode").data("kendoDropDownList");
                    if (ddl != undefined)
                        ddl.dataSource.read();

                    if ($scope.savegroup == false) { $("#accountModal").modal("toggle"); }
                    else { $("#accountModal").modal("toggle"); }
                    //$scope.treenodeselected = "N";
                    var tree = $("#accounttree").data("kendoTreeView");
                    tree.dataSource.read();
                    displayPopupNotification("Data succesfully saved ", "success");
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Account name already exist please try another account name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // this callback will be called asynchronously
                // when the response is available
            }, function errorCallback(response) {

                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Account name already exist please try another account name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
        }
        else {
            var updateUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateAccountByAccCode";
            var model = {
                ACC_CODE: $scope.accountsArr.ACC_CODE,
                ACC_EDESC: $scope.accountsArr.ACC_EDESC,
                ACC_NDESC: $scope.accountsArr.ACC_NDESC,
                TRANSACTION_TYPE: $scope.accountsArr.TRANSACTION_TYPE,
                TPB_FLAG: $scope.accountsArr.TPB_FLAG,
                MASTER_ACC_CODE: $scope.accountsArr.MASTER_ACC_CODE,
                PRE_ACC_CODE: $scope.accountsArr.PRE_ACC_CODE,
                ACC_TYPE_FLAG: $scope.accountsArr.ACC_TYPE_FLAG,
                CURRENT_BALANCE: $scope.accountsArr.CURRENT_BALANCE,
                ACC_SNAME: $scope.accountsArr.ACC_SNAME,
                LIMIT: $scope.accountsArr.LIMIT,
                ACC_NATURE: $scope.accountsArr.ACC_NATURE,
                //ACC_NATURE: $scope.userselected,
                //ACC_NATURE: $("#natureaccount").data("kendoDropDownList").dataItem().key,
                BANK_ACCOUNT_NO: $scope.accountsArr.BANK_ACCOUNT_NO == null ? "" : $scope.accountsArr.BANK_ACCOUNT_NO,
                FREEZE_FLAG: $scope.accountsArr.FREEZE_FLAG == null ? "" : $scope.accountsArr.FREEZE_FLAG,
                PRINTING_FLAG: $scope.accountsArr.PRINTING_FLAG == null ? "" : $scope.accountsArr.PRINTING_FLAG,
                IND_TDS_FLAG: $scope.accountsArr.IND_TDS_FLAG == null ? "" : $scope.accountsArr.IND_TDS_FLAG,
                IND_VAT_FLAG: $scope.accountsArr.IND_VAT_FLAG == null ? "" : $scope.accountsArr.IND_VAT_FLAG,
                GROUP_START_CODE: $scope.accountsArr.GROUP_START_CODE,
                GROUP_END_CODE: $scope.accountsArr.GROUP_END_CODE,
                PREFIX_TEXT: $scope.accountsArr.PREFIX_TEXT,
                ACC_ID: $scope.accountsArr.ACC_ID,
                //CONTACT_PERSON: $scope.bankdetailArr.CONTACT_PERSON,
                //TEL_NO: $scope.bankdetailArr.PHONE_NO,
                //MOBILE_NO: $scope.bankdetailArr.MOBILE_NO,
                //EMAIL_ID: $scope.bankdetailArr.EMAIL_ID,
                LINK_ID: $scope.accountsArr.LINK_ID,
                SHARE_VALUE: $scope.accountsArr.SHARE_VALUE,
                IND_MDF_FLAG: $scope.accountsArr.IND_MDF_FLAG,
                MATURITY_DAYS: $scope.accountsArr.MATURITY_DAYS,
                PRIOR_ALERT_DAYS: $scope.accountsArr.PRIOR_ALERT_DAYS,
                LOAN_TERMS: $scope.accountsArr.LOAN_TERMS,
                ACC_CONTACT_DETAIL: $scope.bankdetailArr,
                BS_CODE: $scope.accountsArr.BS_CODE,
                PL_CODE: $scope.accountsArr.PL_CODE
            }

            let nepaliMaturityDate = $("#nepaliDate6").val();
            if (nepaliMaturityDate) {
                model.MATURITY_DATE = BS2AD(nepaliMaturityDate);
                //model.MATURITY_DATE = $scope.createDateFromString(BS2AD(nepaliMaturityDate));
            }

            let nepaliOpeningDate = $("#nepaliDate5").val();
            if (nepaliOpeningDate) {
                model.ACC_OPENING_DATE = BS2AD(nepaliOpeningDate);
                //model.ACC_OPENING_DATE = $scope.createDateFromString(BS2AD(nepaliOpeningDate));
            }

            let accountInterestSetup = $scope.accountsArr.accountInterestSetup || [];
            let accountInterestSetupNew = []
            angular.forEach(accountInterestSetup, function (value, index) {
                let nepDateSelector = "#nepaliCalendarAccInterest_" + index;
                let nepDateValue = $(nepDateSelector).val();

                if (nepDateValue) {
                    value.INT_DATE = BS2AD(nepDateValue)
                    //if (typeof value.INT_DATE === 'string' && value.INT_DATE !== "") {
                    //    value.INT_DATE = $scope.createDateFromString(value.INT_DATE);
                    //}
                    delete value.NEP_INT_DATE;
                    accountInterestSetupNew.push(value);
                }
            });
            model.ACC_INT_SETUP = accountInterestSetupNew;

            if (!isValid) {
                displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
                return;
            }

            $scope.saveupdatebtn = "Update";
            $http({
                method: 'POST',
                url: updateUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "UPDATED") {
                    $scope.accountsArr = [];
                    if ($scope.accountsetup.ACC_TYPE_FLAG !== "T") {
                        var tree = $("#accounttree").data("kendoTreeView");
                        tree.dataSource.read();
                    }
                    var ddl = $("#masteraccountcode").data("kendoDropDownList");
                    if (ddl != undefined)
                        ddl.dataSource.read();
                    var grid = $("#kGrid").data("kendo-grid");
                    if (grid != undefined) {
                        grid.dataSource.read();
                    }
                    $("#accountModal").modal("toggle");
                    //$scope.treenodeselected = "N";
                    displayPopupNotification("Data succesfully updated ", "success");
                }
                if (response.data.MESSAGE == "ERROR") {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // this callback will be called asynchronously
                // when the response is available
            }, function errorCallback(response) {
                displayPopupNotification("Something went wrong.Please try again later.", "error");
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
        }
    }
    $scope.BindGrid = function (groupId) {
        debugger
        $(".topsearch").show();
        $scope.treenodeselected = "Y";
        var url = null;
        if (groupId == "All") {
            if ($('#acctxtSearchString').val() == null || $('#acctxtSearchString').val() == '' || $('#acctxtSearchString').val() == undefined || $('#acctxtSearchString').val() == 'undefined') {
                alert('Input is empty or undefined.');
                return;
            }
            url = "/api/SetupApi/GetAccountList?searchtext=" + $('#acctxtSearchString').val();
        }
        else {
            $("#acctxtSearchString").val('');
            url = "/api/SetupApi/GetChildOfAccountByGroup?groupId=" + groupId;
        }
        //grid Bind and sorting and paging Prem Prakash Dhakal
        var reportConfig = GetReportSetting("_accountSetupPartial");
        $scope.accoutnChildGridOptions = {
            dataSource: {
                transport: {
                    read: {
                        url: url,
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        type: "GET"
                    },
                    parameterMap: function (options, type) {
                        var paramMap = JSON.stringify($.extend(options, ReportFilter.filterAdditionalData()));
                        delete paramMap.$inlinecount;
                        delete paramMap.$format;
                        return paramMap;
                    }
                },
                error: function (e) {
                    displayPopupNotification("Sorry error occured while processing data", "error");
                },
                model: {
                    fields: {
                        ACC_CODE: { type: "string" },
                        ACC_EDESC: { type: "string" },
                        ACC_ID: { type: "string" },
                    }
                },
                /*pageSize: reportConfig.defaultPageSize,*/
                pageSize: 50,
            },
            height: 500,
            sortable: true,
            reorderable: true,
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
            columnShow: function (e) {
                if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                    SaveReportSetting('_accountSetupPartial', 'kGrid');
            },
            columnHide: function (e) {
                if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                    SaveReportSetting('_accountSetupPartial', 'kGrid');
            },
            pageable: {
                refresh: true,
                //pageSizes: reportConfig.itemPerPage,
                pageSize: 50,
                buttonCount: 5
            },
            pageable: true,
            dataBound: function (o) {
                var grid = o.sender;
                if (grid.dataSource.total() == 0) {
                    var colCount = grid.columns.length;
                    $(o.sender.wrapper)
                        .find('tbody')
                        .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, No data found</td></tr>');
                    /*displayPopupNotification("No Data Found.", "info");*/
                }
                else {
                    var g = $("#kGrid").data("kendoGrid");
                    for (var i = 0; i < g.columns.length; i++) {
                        g.showColumn(i);
                    }
                    $("div.k-group-indicator").each(function (i, v) {
                        g.hideColumn($(v).data("field"));
                    });
                }
                UpdateReportUsingSetting("_accountSetupPartial", "kGrid");
                $('div').removeClass('.k-header k-grid-toolbar');
            },
            columns: [
                {
                    //hidden: true,
                    title: "Code",
                    field: "ACC_CODE",
                    width: "60px",
                },
                {
                    field: "ACC_EDESC",
                    title: "Account Name",
                    width: "100px",
                },
                {
                    field: "ACC_ID",
                    title: "Account Id",
                    width: "100px",
                },
                {
                    title: "Action",
                    template: `
                                <div style="display: flex; gap: 10px; justify-content: center; align-items: center;">
                                  <a class="fa fa-pencil-square-o editAction"
                                     title="Edit"
                                     ng-click="edit(dataItem.ACC_CODE)"
                                     style="cursor: pointer;"
                                     aria-label="Edit">
                                  </a>

                                  <a class="fa fa-trash deleteAction"
                                     title="Delete"
                                     ng-click="delete(dataItem.ACC_CODE, dataItem.ACC_TYPE_FLAG)"
                                     style="cursor: pointer;"
                                     aria-label="Delete">
                                  </a>
                                </div>
                              `,
                    width: "30px"
                }

            ]
        };

        $timeout(function () {
            var $input = $("#acctxtSearchString");
            if ($input.data("bindGlobalSearch")) return;
            $input.data("bindGlobalSearch", true);

            function debounce(fn, delay) {
                var t;
                return function () {
                    var ctx = this, args = arguments;
                    clearTimeout(t);
                    t = setTimeout(function () { fn.apply(ctx, args); }, delay);
                };
            }

            var triggerGlobal = function () {
                var val = $input.val();
                if (val && val.toString().trim().length > 0) {
                    $scope.$evalAsync(function () { $scope.BindGrid('All'); });
                }
            };

            $input.on('keydown.customerGlobal', function (e) {
                if (e.key === 'Enter' || e.keyCode === 13) {
                    e.preventDefault();
                    triggerGlobal();
                }
            });

            $input.on('input.customerGlobal', debounce(triggerGlobal, 400));
        }, 0);

    }




    //    $scope.accoutnChildGridOptions = {
    //        //dataSource: {
    //        //    type: "json",
    //        //    transport: {
    //        //        read: url,
    //        //    },
    //        //    pageSize: 50,
    //        //    //serverPaging: true,
    //        //    serverSorting: true
    //        //},
    //        //Prem Prakash Dhakal Correction of Code
    //        dataSource: {
    //            transport: {
    //                read: {
    //                    url: url,
    //                    dataType: "json",

    //                },
    //            },
    //            pageSize: 50,
    //            serverPaging: true,
    //            serverSorting: true,
    //        },
    //        height: 500,
    //        scrollable: true,
    //        sortable: true,
    //        pageable: true,
    //        filterable: {
    //            extra: false,
    //            operators: {
    //                number: {
    //                    eq: "Is equal to",
    //                    neq: "Is not equal to",
    //                    gte: "is greater than or equal to	",
    //                    gt: "is greater than",
    //                    lte: "is less than or equal",
    //                    lt: "is less than",
    //                },
    //                string: {
    //                    eq: "Is equal to",
    //                    neq: "Is not equal to",
    //                    startswith: "Starts with	",
    //                    contains: "Contains",
    //                    doesnotcontain: "Does not contain",
    //                    endswith: "Ends with",
    //                },
    //                date: {
    //                    eq: "Is equal to",
    //                    neq: "Is not equal to",
    //                    gte: "Is after or equal to",
    //                    gt: "Is after",
    //                    lte: "Is before or equal to",
    //                    lt: "Is before",
    //                }
    //            }
    //        },
    //        columnMenu: true,
    //        columnMenuInit: function (e) {
    //            wordwrapmenu(e);
    //            checkboxItem = $(e.container).find('input[type="checkbox"]');
    //        },
    //        columnShow: function (e) {
    //            if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
    //                SaveReportSetting('_accountSetupPartial', 'kGrid');
    //        },
    //        columnHide: function (e) {
    //            if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
    //                SaveReportSetting('_accountSetupPartial', 'kGrid');
    //        },
    //        dataBound: function (e) {
    //            $("#kGrid tbody tr").css("cursor", "pointer");
    //            DisplayNoResultsFound($('#kGrid'));
    //            $("#kGrid tbody tr").on('dblclick', function () {
    //                var accCode = $(this).find('td span').html();
    //                $scope.edit(accCode);
    //                var tree = $("#accounttree").data("kendoTreeView");
    //                tree.dataSource.read();
    //            })
    //        },
    //        columns: [
    //            {
    //                //hidden: true,
    //                title: "Code",
    //                field: "ACC_CODE",
    //                width: "60px",
    //            },
    //            {
    //                field: "ACC_EDESC",
    //                title: "Account Name",
    //                width: "100px",
    //            },
    //            {
    //                field: "ACC_ID",
    //                title: "Account Id",
    //                width: "100px",
    //            },
    //            {
    //                title: "Action ",
    //                template: '<a class="fa fa-pencil-square-o editAction" title="Edit" ng-click="edit(dataItem.ACC_CODE)"><span class="sr-only"></span> </a><a class="fa fa-trash deleteAction" title="delete" ng-click="delete(dataItem.ACC_CODE,dataItem.ACC_TYPE_FLAG)"><span class="sr-only"></span> </a>',
    //                width: "50px",
    //            }
    //        ],
    //    };
    //}




    $scope.showModalForNew = function (event) {
        $scope.accountsArr = [];
        $scope.dekh = false;
        $scope.editFlag = "N";
        $scope.saveupdatebtn = "Save";

        var getAccountdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getAccountDetailsByAccCode?accCode=" + $scope.treeSelectedAccountCode;
        $http({
            method: 'GET',
            url: getAccountdetaisByUrl
        }).then(function successCallback(response) {
            debugger
            $scope.accountsetup = response.data.DATA;
            $scope.accountsArr = $scope.accountsetup;
            $scope.accountsArr = [];
            $scope.accountsArr.MASTER_ACC_CODE = $scope.accountsetup.MASTER_ACC_CODE;
            $scope.accountsArr.PRE_ACC_CODE = $scope.accountsetup.PRE_ACC_CODE;
            $scope.accountsArr.ACC_NATURE = $scope.accountsetup.ACC_NATURE;
            $scope.accountsArr.ACC_CODE = $scope.accountsetup.ACC_CODE;
            $scope.getDetailByAccCode($scope.accountsArr.ACC_CODE);
            $scope.groupAccTypeFlag = "N";
            $scope.accountsArr.ACC_TYPE_FLAG = "T";
            $scope.accountsArr.LOAN_TERMS = "M";
            $("#nepaliDate5").val("");
            $("#nepaliDate6").val("");
            $scope.accountsArr.ACC_OPENING_DATE = ""
            $scope.accountsArr.MATURITY_DATE = ""
            $scope.accountsArr.accountInterestSetup = $scope.accountsetup.ACC_INT_SETUP
            var tree = $("#masteraccountcode").data("kendoDropDownList");
            tree.value($scope.treeSelectedAccountCode);
            $scope.clearFieldsForNew();
            //Prem Prakash Dhakal
            $timeout(function () {
                var natureWidget = $("#natureaccount").data("kendoDropDownList");
                if (natureWidget) {
                    natureWidget.unbind("change");
                    natureWidget.bind("change", function (e) {
                        $scope.$apply(function () {
                            $scope.NatureAccCodeOnChange(e);
                        });
                    });
                }
            }, 0);

            setAccNature();

            $("#accountModal").modal("toggle");

            var getAccountCodeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetNewAccountCode";
            $http({
                method: 'GET',
                url: getAccountCodeUrl
            }).then(function successCallback(result) {
                $scope.accountsArr.ACC_CODE = result.data;
            });
        }, function errorCallback(response) {
        });
    }
    $scope.edit = function (accCode) {
        $scope.clearFields();
        dataFillDefered = $.Deferred();
        $scope.saveupdatebtn = "Update";
        $scope.editFlag = "Y";
        $scope.fillAccSetupForms(accCode);
        $.each($(".enable-input-on-edit").find("input"), function () {
            $(this).prop('disabled', false);
        })
        $.when(dataFillDefered).then(function () {
            var tree = $("#masteraccountcode").data("kendoDropDownList");
            tree.value($scope.accountsetup.PARENT_ACC_CODE);
            $timeout(function () {
                var natureWidget = $("#natureaccount").data("kendoDropDownList");
                if (natureWidget) {
                    natureWidget.unbind("change");
                    natureWidget.bind("change", function (e) {
                        $scope.$apply(function () {
                            $scope.NatureAccCodeOnChange(e);
                        });
                    });
                }
            }, 0);
            setAccNature();
            $scope.NatureAccCodeOnChange(null)
            $("#tab_1").addClass("active");
            $("#tab_2").removeClass("active");
            $("#li_tab_1").addClass("active");
            $("#li_tab_2").removeClass("active");
            $("#accountModal").modal();
        })
    }
    $scope.delete = function (code, accountTypeFlag) {
        $("#acctxtSearchString").val('');
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
                if (result == true) {
                    if (accountTypeFlag === "T")
                        $scope.accountsetup.ACC_TYPE_FLAG = "T";
                    else
                        $scope.accountsetup.ACC_TYPE_FLAG = "N";
                    var delUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteAccountSetupByAccCode?accCode=" + code;
                    $http({
                        method: 'POST',
                        url: delUrl
                    }).then(function successCallback(response) {
                        if (response.data.MESSAGE == "DELETED") {
                            if ($scope.accountsetup.ACC_TYPE_FLAG !== "T") {
                                var tree = $("#accounttree").data("kendoTreeView");
                                tree.dataSource.read();
                            }
                            $scope.treenodeselected = "N";
                            var grid = $("#kGrid").data("kendo-grid");
                            grid.dataSource.read();
                            displayPopupNotification("Data succesfully deleted ", "success");
                        }
                        if (response.data.MESSAGE == "HAS_CHILD") {
                            displayPopupNotification("Please delete the respective child first", "warning");
                        }
                        // this callback will be called asynchronously
                        // when the response is available
                    }, function errorCallback(response) {
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                        // called asynchronously if an error occurs
                        // or server returns response with an error status.
                    });
                }
            }
        });
    }


    function DisplayNoResultsFound(grid) {
        // Get the number of Columns in the grid
        //var grid = $("#kGrid").data("kendo-grid");
        var dataSource = grid.data("kendoGrid").dataSource;
        var colCount = grid.find('.k-grid-header colgroup > col').length;
        // If there are no results place an indicator row
        if (dataSource._view.length == 0) {
            grid.find('.k-grid-content tbody')
                .append('<tr class="kendo-data-row"><td colspan="' + colCount + '" style="text-align:center"><b>No Results Found!</b></td></tr>');
        }
        // Get visible row count
        var rowCount = grid.find('.k-grid-content tbody tr').length;
        // If the row count is less that the page size add in the number of missing rows
        if (rowCount < dataSource._take) {
            var addRows = dataSource._take - rowCount;
            for (var i = 0; i < addRows; i++) {
                //grid.find('.k-grid-content tbody').append('<tr class="kendo-data-row"><td>&nbsp;</td></tr>');
            }
        }
    }

    $scope.addNewAccount = function () {
        debugger
        $scope.clearFields();
        $scope.editFlag = "N";
        $scope.dekh = true;
        $scope.saveupdatebtn = "Save"
        $scope.accountsArr = [];
        $scope.groupAccTypeFlag = "Y";
        $scope.accountsetup.ACC_TYPE_FLAG = "N";
        $scope.natureaccountOptions = {
            dataSource: RStureArr,
            dataTextField: "text",
            dataValueField: "key",
            optionLabel: "--Select--"
        }
        var tree = $("#masteraccountcode").data("kendoDropDownList");
        tree.value("");
        $scope.accountsArr.ACC_TYPE_FLAG = "N";
        var tree = $("#accounttree").data("kendoTreeView");
        tree.dataSource.read();
        var grid = $("#kGrid").data("kendo-grid");
        if (grid != undefined) {
            grid.dataSource.data([]);
        }
        $timeout(function () {
            var natureWidget = $("#natureaccount").data("kendoDropDownList");
            if (natureWidget) {
                natureWidget.unbind("change");
                natureWidget.bind("change", function (e) {
                    $scope.$apply(function () {
                        $scope.NatureAccCodeOnChange(e);
                    });
                });
            }
            $scope.BankTabResult = false;
        }, 0);
        $("#accountModal").modal("toggle");
        //  $("#cr").attr('disabled', 'disabled');
        //  $("#dr").attr('disabled', 'disabled');
        //  $("#profloss").attr('disabled', 'disabled');
        //   $("#balancesheet").attr('disabled', 'disabled'); 
        var getAccountCodeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetMaxNewAccountCode";
        $http({
            method: 'GET',
            url: getAccountCodeUrl
        }).then(function successCallback(result) {
            $scope.accountsArr.ACC_CODE = result.data;
            $scope.accountsArr.ACC_TYPE_FLAG = "N";
            $("#natureaccount").data("kendoDropDownList").value("");
        });
    }
    $scope.clearFields = function () {
        $scope.accountsArr.ACC_EDESC = "";
        $scope.accountsArr.ACC_NDESC = "";
        $scope.accountsArr.BANK_ACCOUNT_NO = "";
        $scope.accountsArr.BANK_ACCOUNT_NO = "";
        $scope.accountsArr.GROUP_START_CODE = "";
        $scope.accountsArr.GROUP_END_CODE = "";
        $scope.accountsArr.TRANSACTION_TYPE = "";
        $scope.accountsArr.TPB_FLAG = "";
        $scope.accountsArr.FREEZE_FLAG = "";
        $scope.accountsArr.PRINTING_FLAG = "";
        $scope.accountsArr.IND_VAT_FLAG = "";
        $scope.accountsArr.IND_TDS_FLAG = "";
        $scope.accountsArr.DEALER_FLAG = "";
        $scope.accountsArr.ACC_ID = "";
        $scope.accountsArr.PREFIX_TEXT = "";
        $scope.accountsArr.CURRENT_BALANCE = "";
        $scope.accountsArr.ACC_SNAME = "";
        $scope.accountsArr.TOTAL_DAYS = "";
        $scope.accountsArr.SAVING_INTEREST = "";
        $scope.accountsArr.LOAN_INTEREST = "";
        $scope.accountsArr.MATURITY_DAYS = "";
        $scope.accountsArr.LINK_ID = "";
        //$scope.bankdetailArr.CONTACT_PERSON = "";
        //$scope.bankdetailArr.ADDRESS = "";
        //$scope.bankdetailArr.DESIGNATION = "";
        //$scope.bankdetailArr.DEPARTMENT = "";
        //$scope.bankdetailArr.MOBILE_NO = "";
        //$scope.bankdetailArr.PHONE_NO = "";
        //$scope.bankdetailArr.EMAIL_ID = "";
        $("#acctxtSearchString").val('');
        $scope.result = false;
        $scope.share = false;
    }
    $scope.clearFieldsForNew = function () {
        $scope.accountsArr.ACC_EDESC = "";
        $scope.accountsArr.ACC_NDESC = "";
        $scope.accountsArr.BANK_ACCOUNT_NO = "";
        $scope.accountsArr.BANK_ACCOUNT_NO = "";
        $scope.accountsArr.GROUP_START_CODE = "";
        $scope.accountsArr.GROUP_END_CODE = "";
        $scope.accountsArr.TRANSACTION_TYPE = "";
        $scope.accountsArr.TPB_FLAG = "";
        $scope.accountsArr.BS_CODE = "";
        $scope.accountsArr.PL_CODE = "";
        $scope.accountsArr.FREEZE_FLAG = "";
        $scope.accountsArr.PRINTING_FLAG = "";
        $scope.accountsArr.IND_VAT_FLAG = "";
        $scope.accountsArr.IND_TDS_FLAG = "";
        $scope.accountsArr.DEALER_FLAG = "";
        //$scope.accountsArr.ACC_ID = "";
        //$scope.accountsArr.PREFIX_TEXT = "";
        $scope.accountsArr.CURRENT_BALANCE = "";
        $scope.accountsArr.ACC_SNAME = "";
        $scope.accountsArr.TOTAL_DAYS = "";
        $scope.accountsArr.SAVING_INTEREST = "";
        $scope.accountsArr.LOAN_INTEREST = "";
        $scope.accountsArr.MATURITY_DAYS = "";
        $scope.accountsArr.LINK_ID = "";
        //$scope.bankdetailArr.CONTACT_PERSON = "";
        //$scope.bankdetailArr.ADDRESS = "";
        //$scope.bankdetailArr.DESIGNATION = "";
        //$scope.bankdetailArr.DEPARTMENT = "";
        //$scope.bankdetailArr.MOBILE_NO = "";
        //$scope.bankdetailArr.PHONE_NO = "";
        //$scope.bankdetailArr.EMAIL_ID = "";
        $("#acctxtSearchString").val('');
        $scope.result = false;
        $scope.share = false;

        //if ($scope.accountsArr.ACC_NATURE == "LC" || $scope.accountsArr.ACC_NATURE == "AC" || $scope.accountsArr.ACC_NATURE == "AD")
        //    $scope.result = true;
        //else
        //    $scope.result = false;

    }
    $scope.reset = function () {
        $scope.accountsetup =
        {
            TRANSACTION_TYPE: "",
            PREFIX_TEXT: "",
            ACC_CODE: "",
            ACC_EDESC: "",
            ACC_NDESC: "",
            TPB_FLAG: "",
            ACC_TYPE_FLAG: "T",
            MASTER_ACC_CODE: "",
            PRE_ACC_CODE: "",
            COMPANY_CODE: "",
            CREATED_BY: "",
            CREATED_DATE: "",
            DELETED_FLAG: "",
            SYN_ROWID: "",
            DELTA_FLAG: "",
            FREEZE_FLAG: "N",
            CURRENT_BALANCE: "",
            LIMIT: "",
            MODIFY_DATE: "",
            ACC_NATURE: "",
            BRANCH_CODE: "",
            SHARE_VALUE: "",
            MODIFY_BY: "",
            IND_VAT_FLAG: "N",
            BANK_ACCOUNT_NO: "",
            PRINTING_FLAG: "N",
            ACC_SNAME: "",
            TEL_NO: "",
            IND_TDS_FLAG: "N",
            ACC_ID: "",
            MOBILE_NO: "",
            EMAIL_ID: "",
            LINK_ID: "",
            CONTACT_PERSON: "",
            GROUP_START_CODE: "",
            GROUP_END_CODE: "",
            DEALER_FLAG: "",
            PARENT_ACC_CODE: "",
            IND_MDF_FLAG: "",
            MATURITY_DATE: "",
            ACC_OPENING_DATE: "",
            ACC_INT_SETUP: [],
            MATURITY_DAYS: "",
            PRIOR_ALERT_DAYS: "",
            LOAN_TERMS: "M"
        }
        $scope.newValue = function () {
            console.log(value);
        }
        $scope.bankaccountdetail = {
            CONTACT_PERSON: "",
            DEPARTMENT: "",
            DESIGNATION: "",
            ADDRESS: "",
            MOBILE_NO: "",
            PHONE_NO: "",
            EMAIL_ID: ""
        }
        $scope.bankdetailArr = [];
        $scope.bankdetailArr.push($scope.bankaccountdetail);
        $scope.accountsArr = $scope.accountsetup;
    }
    function setAccNature(isNewEntry = false) {
        debugger
        var accNature = $scope.accountsArr.ACC_NATURE;
        accNature = isNewEntry ? "" : accNature;
        if (accNature == "AC" || accNature == "AB" || accNature == "AC" || accNature == "AE" || accNature == "AD" || accNature == "AF") {
            $("#natureaccount").kendoDropDownList({
                dataTextField: "text",
                dataValueField: "key",
                dataSource: CANature,
                value: accNature
            });
        }
        else if (accNature == "LA" || accNature == "LB" || accNature == "LC" || accNature == "LD" || accNature == "PV" || accNature == "PL") {
            $("#natureaccount").kendoDropDownList({
                dataTextField: "text",
                dataValueField: "key",
                dataSource: SCNature,
                value: accNature
            });
        }
        else if (accNature == "AA") {
            $("#natureaccount").kendoDropDownList({
                dataTextField: "text",
                dataValueField: "key",
                dataSource: RStureArr,
                value: accNature
            });
        }
        else if (accNature == "IA" || accNature == "IC") {
            $("#natureaccount").kendoDropDownList({
                dataSource: SRNature,
                dataTextField: "text",
                dataValueField: "key",
                value: accNature
            });
        }
        else if (accNature == "EA" || accNature == "EC" || accNature == "EB") {
            $("#natureaccount").kendoDropDownList({
                dataSource: MENature,
                dataTextField: "text",
                dataValueField: "key",
                value: accNature
            });
        }
        else if (accNature == "SE" || accNature == "SA" || accNature == "SB" || accNature == "SC" || accNature == "SD") {
            $("#natureaccount").kendoDropDownList({
                dataSource: SPNature,
                dataTextField: "text",
                dataValueField: "key",
                value: accNature
            });
        }
        //const accNatureValid = $scope.accountsArr.ACC_NATURE;
        //if (accNatureValid === "LC" || accNatureValid === "AC") {
        //   /* $scope.result = true;*/
        //    setTimeout(function () {
        //        $('a[href="#tab_2"]').tab('show');
        //    }, 0);
        //} else {
        //  /*  $scope.result = false;*/
        //    setTimeout(function () {
        //        $('a[href="#tab_1"]').tab('show');
        //    }, 0);
        //}

        //if ($scope.accountsArr.ACC_NATURE == "LC" || $scope.accountsArr.ACC_NATURE == "AC" || $scope.accountsArr.ACC_NATURE == "AD")
        //    $scope.result = true;
        //else
        //    $scope.result = false;

        //if ($scope.accountsArr.ACC_NATURE == "LA")
        //    $scope.share = true;
        //else
        //    $scope.share = false;

        if ($scope.accountsArr.ACC_NATURE == "LC" || $scope.accountsArr.ACC_NATURE == "AC") {
            $scope.result = true;
        }
        else {
            $scope.result = false;
        }

        if ($scope.accountsArr.ACC_NATURE == "LA") {
            $scope.share = true;
        }
        else {
            $scope.share = false;
        }

        $scope.NatureAccCodeOnChange(null)

    }

    //Search For Main Group Tree Develop by Prem Prakash Dhakal
    $scope.BindMainGroupTree = function (SearchText) {
        $('.topmaingroupsearch').show();
        $scope.treenodeselected = 'Y';
        $scope.txtMainGroupSearchString = SearchText !== undefined ? SearchText : ($('#acctxtMainGroupSearchString').val() || '');
        if ($scope.txtMainGroupSearchString === '' && SearchText !== '') {
            alert('Please enter a search term.');
            $('#acctxtMainGroupSearchString').focus();
            return;
        }
        $scope.acctreeData = new kendo.data.HierarchicalDataSource({
            transport: {
                read: {
                    url: function () {
                        var searchText = $scope.txtMainGroupSearchString || '';
                        return '/api/TemplateApi/getAccountCode?searchText=' + encodeURIComponent(searchText);
                    },
                    type: 'GET',
                    error: function (e) {
                        console.error('Error fetching account codes:', e);
                        alert('Failed to load account data. Please try again.');
                    }
                }
            },
            schema: {
                parse: function (data) {
                    return data;
                },
                model: {
                    id: 'MASTER_ACC_CODE',
                    parentId: 'PRE_ACC_CODE',
                    children: 'Items',
                    fields: {
                        ACC_CODE: { type: 'string' },
                        ACC_EDESC: { type: 'string' },
                        parentId: { type: 'string', defaultValue: '00' }
                    }
                }
            }
        });
        $scope.$applyAsync();
    };

    //$scope.ConvertNepToEng = function ($event) {
    //    var date = BS2AD($("#nepaliDate5").val());
    //    $("#englishdatedocument").val($filter('date')(date, "dd-MMM-yyyy"));
    //    $('#nepaliDate5').trigger('change');
    //};
    $scope.ConvertEngToNepang = function (data, id) {
        var lastChar = id[id.length - 1];
        var ids = "#nepaliDate" + lastChar + "";
        $(ids).val(AD2BS(data));
    };

    $scope.monthSelectorOptions = {
        open: function () {

            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() + 100);
        },
        change: function () {

            var id = this.element.attr('id');
            $scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'), id)
        },
        format: "dd-MMM-yyyy",

        dateInput: true
    };

    $scope.monthSelectorOptionsSingle = {
        open: function () {
            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",

        dateInput: true
    };

    $scope.OnOpeningDateChange = function () {
        var openingDate = $('#englishdatedocument5').val();
        var maturityDate = $('#englishdatedocument6').val();
        var oDate = moment(openingDate).format("MM-DD-YYYY");
        var mDate = moment(maturityDate).format("MM-DD-YYYY");
        if (maturityDate != "") {
            if (mDate < oDate) {
                $('#englishdatedocument5').val("");
                $("#savedocumentformdata").prop("disabled", true);
                displayPopupNotification("Start date must be less than End  Date!", "warning");
                $scope.accountsArr.TOTAL_DAYS = "";
                return;
            }
            else {
                $("#savedocumentformdata").prop("disabled", false);
            }
        }
    };

    $scope.OnMaturityDateChange = function () {
        var openingDate = $('#englishdatedocument5').val();
        var maturityDate = $('#englishdatedocument6').val();

        var oDate = moment(openingDate).format("MM-DD-YYYY");
        var mDate = moment(maturityDate).format("MM-DD-YYYY");
        if (mDate < oDate) {
            $('#englishdatedocument6').val("");
            $("#savedocumentformdata").prop("disabled", true);
            displayPopupNotification("End date must be greater than Start Date!", "warning");
            $scope.accountsArr.MATURITY_DAYS = "";
            return;
        }
        else {
            $("#savedocumentformdata").prop("disabled", false);
        }

        if (maturityDate !== "" && openingDate !== "") {
            let mMomentDate = moment(maturityDate);
            let oMomentDate = moment(openingDate)
            let totalDays = mMomentDate.diff(oMomentDate, 'days');
            $scope.accountsArr.MATURITY_DAYS = totalDays;
        }
    };

    $scope.renderDynamicInterestTable = function () {
        let tableHtml = `
        <table class="document_child_table dynamic-table table-fixed dynamic_child_table">
            <thead class="child-table-head">
                <tr>
                    <th>S.N.</th>
                    <th>Date</th>
                    <th>Interest Rate</th>
                    <th class="btn-action">&nbsp;</th>
                </tr>
            </thead>
            <tbody class="table_body">`;

        const list = $scope.accountsArr.accountInterestSetup || [];

        if (list.length === 0) {

            tableHtml += `
            <tr>
                <td colspan="3" class="text-center">No interest setup entries.</td>
                <td class="btn-action">
                    <a href="" class="buttonadd" ng-click="add_acc_int_setup_row()"><i class="fa fa-plus"></i></a>
                </td>
            </tr>`;
        } else {
            for (let i = 0; i < list.length; i++) {
                const nepaliId = `nepaliCalendarAccInterest_${i}`;
                const englishId = `englishCalendarAccInterest_${i}`;

                tableHtml += `
                <tr>
                    <td>
                        <div>
                            <input type="text" class="form-control" value="${i + 1}" readonly />
                        </div>
                    </td>
                    <td>
                        <div style="position: relative">
                            <input type="hidden"
                                   id="${englishId}"
                                   name="${englishId}"
                                   style="width:100%;"
                                   class="englishdate" />

                            <div class="nepali_date">
                                <input type="text"
                                       id="${nepaliId}"
                                       data-checkfor="${englishId}"
                                       class="nepaliCalendarAccInterest nepalidate form-control"
                                       />
                                <i style="position: absolute; right: 0;" class="fa fa-calendar calendar-icon-2"></i>
                            </div>
                        </div>
                    </td>
                    <td>
                        <div>
                            <input type="text" ng-model="accountsArr.accountInterestSetup[${i}].INT_RATE" class="form-control" />
                        </div>
                    </td>
                    <td class="btn-action">
                        <a href="" class="buttonadd" ng-click="add_acc_int_setup_row(${i})"><i class="fa fa-plus"></i></a>
                        <a href="javascript:void(0)" class="buttondelete" ng-click="remove_acc_int_setup_row(${i})">
                            <i class="fa fa-trash text-danger" data-tooltip="Remove other info"></i>
                        </a>
                    </td>
                </tr>`;
            }
        }

        tableHtml += `</tbody></table>`;

        const compiled = $compile(tableHtml)($scope);
        $('#accountInterestTableContainer').html(compiled);

        setTimeout(() => {
            for (let i = 0; i < list.length; i++) {
                const nepaliId = `#nepaliCalendarAccInterest_${i}`;
                const englishId = `#englishCalendarAccInterest_${i}`;

                const storedNepaliDate = $scope.accountsArr.accountInterestSetup[i].NEP_INT_DATE;
                if (storedNepaliDate) {
                    $(nepaliId).val(storedNepaliDate);
                }

                $(nepaliId).nepaliDatePicker({
                    npdMonth: true,
                    onFocus: true,
                    npdYear: true,
                    npdYearCount: 10,
                    dateFormat: "DD-MMM-YYYY",
                    altFormat: "DD-MMM-YYYY",
                    onChange: function () {
                        const nepaliVal = $(nepaliId).val();
                        try {
                            const engDate = BS2AD(nepaliVal);
                            if (engDate) {
                                $(englishId).val(engDate);
                                $scope.accountsArr.accountInterestSetup[i].INT_DATE = engDate;
                                $scope.accountsArr.accountInterestSetup[i].NEP_INT_DATE = nepaliVal;
                            }
                        } catch (e) {
                            $scope.accountsArr.accountInterestSetup[i].INT_DATE = null;
                        }

                        $scope.$applyAsync();
                    }
                });
            }
        }, 100);

        if (!$scope.$$phase) {
            $scope.$apply();
        }

    };

    $scope.add_acc_int_setup_row = function (index) {
        $scope.accountsArr.accountInterestSetup.splice(index + 1, 0, {
            INT_DATE: "",
            INT_RATE: ""
        });
        $scope.renderDynamicInterestTable();
    }

    $scope.remove_acc_int_setup_row = function (index) {
        $scope.accountsArr.accountInterestSetup.splice(index, 1);
        $scope.renderDynamicInterestTable();
    }

    $scope.renderDynamicInterestTable();

    $scope.createDateFromString = function (dateString) {
        const regex = /^\d{4}-\d{2}-\d{2}$/;
        if (!regex.test(dateString)) {
            console.error("Invalid date format. Expected YYYY-MM-DD.");
            return null;
        }

        const parts = dateString.split('-');
        const year = parseInt(parts[0], 10);
        const month = parseInt(parts[1], 10) - 1;
        const day = parseInt(parts[2], 10);

        const date = new Date(year, month, day);

        if (date.getFullYear() !== year || date.getMonth() !== month || date.getDate() !== day) {
            console.error("Invalid date components. Date created does not match input.");
            return null;
        }

        return date;
    }

    $scope.dateToYYYYMMDD = function (date) {
        date = new Date(date);
        if (!(date instanceof Date) || isNaN(date.getTime())) {
            console.error("Invalid Date object provided.");
            return null;
        }

        const year = date.getFullYear();
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        const day = date.getDate().toString().padStart(2, '0');

        return `${year}-${month}-${day}`;
    }
});