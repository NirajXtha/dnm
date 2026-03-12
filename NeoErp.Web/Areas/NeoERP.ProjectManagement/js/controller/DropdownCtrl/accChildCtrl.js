PMModule.controller('accChildCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {    
    $scope.childRowIndex;
    //show modal popup

    var getChildAccountCodeByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAccountCode";
    $scope.childAccounttreeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: getChildAccountCodeByUrl,
                type: 'GET',
                data: function (data, evt) {
                }
            },

        },
        schema: {
            parse: function (data) {
                return data;
            },
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

    ////treeview expand on startup
    //$scope.onChildAccountDataBound = function () {
    //    if ($scope.childRowIndex == undefined)
    //        $('#fromlocationtree').data("kendoTreeView").expand('.k-item');
    //}

    //treeview on select
    $scope.subaccountoptions = {
        loadOnDemand: false,
        select: function (e) {
            $('.topsearch').show();
            var currentItem = e.sender.dataItem(e.node);
            $('#accountGrid_' + $scope.childRowIndex).removeClass("show-displaygrid");
            $("#accountGrid_" + $scope.childRowIndex).html("");
            BindaccountchildGrid(currentItem.AccountId, currentItem.masterAccountCode, "");
            $scope.$apply();
        },
    };

    //search whole data on search button click
    $scope.BindSearchGrid = function () {
        $scope.searchText = $scope.SubLedgertxtSearchString;
        BindaccountchildGrid("", "", $scope.searchText);
    }

    //Grid Binding main Part
    function BindaccountchildGrid(accId, accMasterCode, searchText) {
        $scope.accountchildGridOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: "/api/TemplateApi/GetAccountListByAccountCode?accId=" + accId + '&accMastercode=' + accMasterCode + '&searchText=' + searchText,
                },
                schema: {
                    type: "json",
                    model: {
                        fields: {
                            ACC_CODE: { type: "string" },
                            ACC_EDESC: { type: "string" }
                        }
                    }
                },
                pageSize: 30,

            },
            scrollable: true,
            sortable: true,
            resizable: true,
            pageable: true,
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
                        startswith: "Starts with",
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
            },
            dataBound: function (e) {
             
                $("#accountGrid_" + $scope.childRowIndex + " tbody tr").css("cursor", "pointer");
                $("#accountGrid_" + $scope.childRowIndex + " tbody tr").on('dblclick', function () {
                    debugger;
                    var subaccountcode = $(this).find('td span').html();
                    //var subaccName = $(this).find('td span[ng-bind="dataItem.ACC_EDESC"]').html();
                    var subaccName = $(this).find('td span[ng-bind="dataItem.ACC_EDESC"]')[0].innerText;
                    //var accName = $(this).find('td span[ng-bind="dataItem.ACC_EDESC"]')[0].innerText;
                    $("#idaccount_" + $scope.childRowIndex).data('kendoComboBox').dataSource.data([{ ACC_CODE: subaccountcode, ACC_EDESC: subaccName, Type: "code" }]);   
                    $scope.childModels[$scope.childRowIndex]["ACC_CODE"] = subaccountcode;
                    if ($($(".caccount_" + $scope.childRowIndex)[0]).closest('div').hasClass('borderRed')) {
                        $($(".caccount_" + $scope.childRowIndex)[0]).closest('div').removeClass('borderRed')
                    }
                    $('#AccountModal_' + $scope.childRowIndex).modal('toggle');

                    var index = $scope.childRowIndex;
                    $rootScope.childRowIndexacc = $scope.childRowIndex;
                    var accCode = subaccountcode;
                    window.accCode = accCode;
                    window.globalIndex = index;
                    //$scope.validaccsum($scope.childRowIndex);
                    //---------------------------------------------------------------------------------------------------------------------
                    if ($scope.childModels[index].AMOUNT == $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT) {
                        $scope.dynamicSubLedgerModalData[index].REMAINING_AMOUNT = 0;
                        $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT = $scope.childModels[index].AMOUNT;
                        $('.remainingamt').removeClass("borderred");
                    }
                    if (accCode === $scope.dynamicSubLedgerModalData[index].ACC_CODE) {
                        $scope.dynamicSubLedgerModalData[index] = $scope.dynamicSubLedgerModalData[index];
                        $scope.dynamicModalData[index] = $scope.dynamicModalData[index];
                    } else {

                        $scope.dynamicSubLedgerModalData[index].SUBLEDGER = [{
                            SERIAL_NO: index + 1,
                            SUB_CODE: "",
                            SUB_EDESC: "",
                            AMOUNT: "",
                            PARTICULARS: "",
                            REFRENCE: ""
                        }];
                        $scope.dynamicModalData[index].BUDGET = [{
                            SERIAL_NO: index + 1,
                            BUDGET_CODE: "",
                            AMOUNT: "",
                            NARRATION: ""
                        }];
                    }

                    $scope.childModels[index]["TRANSACTION_TYPE"] = $scope.childModels[index].TRANSACTION_TYPE;
                    $scope.transactiontype = $scope.childModels[index].TRANSACTION_TYPE.toString();
                    $scope.dynamicSubLedgerModalData[index].ACC_CODE = accCode;

                    var response = $http.get('/api/TemplateApi/getSubledgerCodeByAccCode?accCode=' + accCode);
                    response.then(function (res) {
                        if (res.data != "0") {
                            $(".dynamicSubLedgerModal_" + index).modal('toggle');
                            $(".subledger_transaction_type").val($scope.transactiontype);
                        }
                    });
                    var response = $http.get('/api/TemplateApi/IfIsTdsByAccCode?accCode=' + accCode);
                    response.then(function (res) {

                        if (res.data != "0") {
                            $(".dynamictdsModal_" + index).modal('toggle');
                            //$scope.dynamicTDSModalData[index].ACC_CODE = accCode;
                            //popupAccessTds = true;
                            //$scope.popUpTds(index);

                        }
                    });
                    var response = $http.get('/api/TemplateApi/IfIsVATByAccCode?accCode=' + accCode);

                    response.then(function (res) {
                        if (res.data != "0") {
                            $(".dynamicVATModal_" + index).modal('toggle');
                            //$scope.dynamicVATModalData[index].ACC_CODE = accCode;
                            //popupAccessVAT = true;
                            //$scope.popUpVAT(index);

                        }
                    });
                    var response = $http.get('/api/TemplateApi/getBudgetCodeCountCodeByAccCode?accCode=' + accCode);
                    response.then(function (res) {
                        if (res.data != "0") {
                            $(".subledgerModal_" + index).modal('toggle');
                            //$scope.dynamicModalData[index].ACC_CODE = accCode;
                            //popupAccessBudget = true;
                            //$scope.popUpBudget(index);

                        }
                    });

                    //---------------------------------------------------------------------------------------------------------------------------
                    $scope.$apply();
                    
                    //var len = (parseInt(index) * 2) + 1;
                    //$($(".subledgerfirst:input")[len]).data('kendoComboBox').dataSource.read();
                    //$($(".subledgersecond:input")[len]).data('kendoComboBox').dataSource.read();

                    var first = $(".subledgerfirst:input");
                    $.each(first, function (i, obj) {
                        obj = $(obj);
                        if (!_.isEmpty(obj.data('kendoComboBox'))) {
                            obj.data('kendoComboBox').dataSource.read();
                        }
                    });
                    var second = $(".subledgersecond:input");
                    $.each(second, function (i, obj) {
                        obj = $(obj);
                        if (!_.isEmpty(obj.data('kendoComboBox'))) {
                            obj.data('kendoComboBox').dataSource.read();
                        }
                    });
                    var a = $(this).find('td span[ng-bind="dataItem.ACC_NATURE"]').html();
                    var response = $http.get('/api/ContraVoucherApi/GetPurchaseExpensesFlag?formCode=' + $scope.formcode);
                    response.then(function (res) {
                        debugger;
                        if (res.data[0] == 'Y') {                            
                            if (a == 'SB') {
                                $("#PurchaseExpSheet").modal('toggle');
                                $rootScope.ACC_CODE = accCode;
                            }
                        }
                    });
                })
            },

            columns: [{
                field: "ACC_CODE",
                //hidden: true,
                title: "Code",

            }, {
                field: "ACC_EDESC",
                title: "Account Name",

            },
            {
                field: "TRANSACTION_TYPE",
                title: "Transaction Type",

            },
            {
                field: "ACC_NATURE",
                title: "Account Nature",

            },
            {
                field: "CREATED_BY",
                title: "Created By",
                width: "120px"
            }, {
                field: "CREATED_DATE",
                title: "Created Date",
                template: "#= kendo.toString(CREATED_DATE,'dd MMM yyyy') #",
                //template: "#= kendo.toString(kendo.parseDate(CREATED_DATE, 'yyyy-MM-dd'), 'dd MMM yyyy') #",

            },


            ]
        };
    }


    //show modal popup
    $scope.BrowseTreeListForChildAccountCode = function (index) {        
        if ($scope.havRefrence == 'Y' && $scope.freeze_master_ref_flag == "Y") {
            var referencenumber = $('#refrencetype').val();
            if ($scope.ModuleCode != '01' && referencenumber !== "") {
                return;
            }
        }
        if ($scope.freeze_master_ref_flag == "N") {
            $scope.childRowIndex = index;
            document.popupindex = index;
            $('#AccountModal_' + index).modal('show');
            if ($('#accounttree_' + $scope.childRowIndex).data("kendoTreeView") != undefined)
                $('#accounttree_' + $scope.childRowIndex).data("kendoTreeView").expand('.k-item');
        }
        
    }

    $scope.acccode_Cancel = function (e) {
        $('#AccountModal_' + e).modal('hide');
    }
    $scope.show = function (e) {
        debugger;
        $rootScope.ACC_CODE = e.dataItem.ACC_CODE;
        $rootScope.childRowIndexacc = this.$index;
        var response = $http.get('/api/ContraVoucherApi/GetPurchaseExpensesFlag?formCode=' + $scope.formcode);
        //old code
            //response.then(function (res1) {
           
            //    var response = null;
            //    if (res1.data[0] == 'Y') {
            //        response = $http.get('/api/TemplateApi/GetAccountListByAccountCode?accId=null&accMastercode=niraj&searchText=' + e.dataItem.ACC_EDESC);                    
            //    }
            //    else {
            //        response = $http.get('/api/TemplateApi/GetAccountListByAccountCode?accId=null&accMastercode=&searchText=' + e.dataItem.ACC_EDESC);
            //    }
            //    response.then(function (res2) {
            //        if (res2.data.length > 0) {
            //            if (res2.data[0].ACC_NATURE == 'SB') {
            //                $("#PurchaseExpSheet").modal('toggle');
            //            }
            //        }
            //    });
            //});
        //subin change
        response.then(function (res1) {

            var response = null;
            if (res1.data[0] == 'Y') {
                response = $http.get('/api/TemplateApi/GetAccountListByAccountCode?accId=null&accMastercode=niraj&searchText=' + e.dataItem.ACC_EDESC);
            }
            else {
                response = $http.get('/api/TemplateApi/GetAccountListByAccountCode?accId=null&accMastercode=&searchText=' + e.dataItem.ACC_EDESC);
            }
            response.then(function (res2) {
                if (res2.data.length > 0) {
                    if (res2.data[0].ACC_NATURE == 'SB') {
                        $("#PurchaseExpSheet").modal('toggle');
                    }
                }
            });
        });
    }
});