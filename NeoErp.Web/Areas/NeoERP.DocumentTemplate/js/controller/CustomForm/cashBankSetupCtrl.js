DTModule.controller('cashBankSetupCtrl', function ($scope, cashBankSetupService) {

    console.log("cash bank setup controller");
    $scope.FormName = "Cash Bank Setup";
    $scope.saveAction = "Save";

   
    $scope.btnDelete = false;

    $scope.CashBankSetupObj = {
        "SHORT_CUT": "",
        "IN_ENGLISH": "",
        "IN_NEPALI": "",
        "ACC_TYPE_FLAG":"",
        "IS_UPDATE":false
    };

    $("#selectedAccountGrid").kendoGrid({
        dataSource: {
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            type: "GET",
            transport: {
                read: "/api/CustomFormApi/GetCashBankAccountDetail?cb_code=01"
            },
            pageSize: 50
        },
        scrollable: true,
        pageable: true,
        groupable: false,
        columns: [
            {
                field: "ACC_EDESC",
                title: "ASSOCIATED ACCOUNTS",
                width: "100px"
            }
        ]
    });

    $scope.cashBankGridOptions = {
        change: cashBankGridChange,
        dataSource: {
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            type: "GET",
            transport: {
                read: "/api/CustomFormApi/GetCashBankRootDetail"
            },
            pageSize: 20
        },
        groupable: false,
        sortable: false,
        selectable: true,
        scrollable: {
            virtual: true
        },
        dataBound: function (e) {
            var grid = this;
            var rows = grid.items();
            grid.select(rows[0]);
        },
        columns: [
            {
                field: "CB_EDESC",
                title: "Detail",
                width: "100px"
            }]

    };

    function cashBankGridChange() {
        selectedRow = this.select();
        var item = this.dataItem(selectedRow);
        console.log("Item=================>>>>" + JSON.stringify(item));

        if (item) {
            $scope.saveAction = "Update";
            $scope.CashBankSetupObj.SHORT_CUT = item.CB_CODE;
            $scope.CashBankSetupObj.IN_ENGLISH = item.CB_EDESC;
            $scope.CashBankSetupObj.IN_NEPALI = item.CB_NDESC;
            $scope.btnDelete = true;
            $scope.$apply();


            var selectedAccountGrid = $("#selectedAccountGrid").data("kendoGrid");
            selectedAccountGrid.dataSource.transport.options.read.url = window.location.protocol + "//" + window.location.host + "/api/CustomFormApi/GetCashBankAccountDetail?cb_code="+$scope.CashBankSetupObj.SHORT_CUT;
            selectedAccountGrid.dataSource.read();
        }
    }


    $scope.Cancel = function () {

        $scope.getCashBankDetailId();

        $scope.saveAction = "Save";
        $scope.CashBankSetupObj = {
            SHORT_CUT: "",
            IN_ENGLISH: "",
            IN_NEPALI: ""
        };

        $scope.btnDelete = false;
        $("#cashBankGrid").data("kendoGrid").clearSelection();
        var selectedAccountGrid = $("#selectedAccountGrid").data("kendoGrid");
        selectedAccountGrid.dataSource.transport.options.read.url = window.location.protocol + "//" + window.location.host + "/api/CustomFormApi/GetCashBankAccountDetail?cb_code=" + $scope.CashBankSetupObj.SHORT_CUT;
        selectedAccountGrid.dataSource.read();
        $('#btnAddNewCB').prop('disabled', false);

    };

    $scope.DeleteCashBank = function () {
        bootbox.confirm({
            title: "Delete",
            message: "Are you sure?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {

                if (result === true) {
                    var CB_EDESC = $("#in_english").val();
                    var CB_CODE = $("#short_cut").val();
                    if (CB_CODE === undefined) {
                        CB_CODE = $scope.CashBankSetupObj.SHORT_CUT;
                    }
                    var deleteCBUrl = window.location.protocol + "//" + window.location.host + "/api/CustomFormApi/DeleteCashBankSetup?cbCode=" + CB_CODE;
                    var delRes = cashBankSetupService.deleteCashBankSetup(CB_CODE);
                    delRes.then(function (dRes) {
                        if (dRes.data.message === "DELETED") {
                            $("#cashBankGrid").data("kendoGrid").dataSource.read();
                            $scope.Cancel();
                            displayPopupNotification("Data succesfully deleted ", "success");
                        } else {
                            displayPopupNotification(response.data.STATUS_CODE, "error");
                        }
                    });
                    //$http({
                    //    method: 'POST',
                    //    url: deleteCBUrl
                    //}).then(function successCallback(response) {
                    //    if (response.data.MESSAGE === "DELETED") {
                    //        $("#cashBankGrid").data("kendoGrid").dataSource.read();
                    //        $scope.Cancel();
                    //        displayPopupNotification("Data succesfully deleted ", "success");
                    //    }
                    //    else {
                    //        displayPopupNotification(response.data.STATUS_CODE, "error");
                    //    }
                        
                    //}, function errorCallback(response) {
                    //    displayPopupNotification(response.data.STATUS_CODE, "error");
                       
                    //});
                }
                $("#cashBankGrid").data("kendoGrid").dataSource.read();
                $scope.Cancel();
            }

        });


    };

    $scope.SaveCashBankSetup = function (isValid) {
        if ($scope.saveAction === "Update") $scope.CashBankSetupObj.IS_UPDATE = true;
        else $scope.CashBankSetupObj.IS_UPDATE = false;

        var model = {
            CashBankDetail: $scope.CashBankSetupObj,
            ListInfo: $scope.selectedAccountDetail
        };

        var validation = [
            { short_cut: $scope.CashBankSetupForm.short_cut.$invalid },
            { in_english: $scope.CashBankSetupForm.in_english.$invalid }
        ];

        if (validation[0].short_cut === true) {

            displayPopupNotification("Please provide type shortcut", "warning");
            return;
        }
        if (validation[1].in_english === true) {

            displayPopupNotification(" Please provide name in english", "warning");

            return;
        }

        if ($scope.saveAction === "Save") {
            var saveResponse = cashBankSetupService.SaveCashBankAccountDetail(model);
            saveResponse.then(function (savRes) {
                if (savRes.data === "Successfull") {
                    $("#cashBankGrid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully saved ", "success");
                } else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            });
            
        }
        else if ($scope.saveAction === "Update") {
            var model1 = {
                CashBankDetail: $scope.CashBankSetupObj,
                ListInfo: $scope.selectedAccountDetail
            };
            var updateResponse = cashBankSetupService.SaveCashBankAccountDetail(model1);
            updateResponse.then(function (upRes) {
                if (upRes.data === "Successfull") {
                    $("#cashBankGrid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully updated ", "success");
                } else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            });
        }
    };

    $scope.getCashBankDetailId = function () {
        var cbId = cashBankSetupService.generateCBId();
        cbId.then(function (cbRes) {
            $scope.CashBankSetupObj.SHORT_CUT = cbRes.data;
        });
    };

    $scope.addCashBankAccountDetail = function () {
        if ($scope.CashBankSetupObj.IN_ENGLISH) {
            $("#addCashBankAccountModal").toggle();
        } else {
            displayPopupNotification("Please insert cash bank name beforing adding", "error");
            return;
        }
        
    };

    $scope.closeCashBankAccoutModal = function () {
        $("#addCashBankAccountModal").toggle();
    };

    function checkboxEventBinding() {

        $('#selectAllListItem').bind('click', function (e) {
            if (this.checked) {
                $('.item.click input').attr('checked', 'checked');
                $('.item.click').parent().addClass('k-state-selected');
                $('.item.click').parent().select();

            }
            else {
                $('.item.click input').removeAttr('checked');
                $('.item.click').parent().removeClass('k-state-selected');

            }

        });
    }

    $scope.checkedAccountList = [];
    $scope.checkedAccountObj = {
        "TYPE_CODE": "",
        "TYPE_EDESC": "",
        "LINK_SUB_CODE": "",
        "ITEMS": []
    };
    var getCharofAccountUrl = window.location.protocol + "//" + window.location.host + "/api/SubLedgerMgmtApi/GetCharOfAccountTree";
    var chartOfAccountDataSource = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: getCharofAccountUrl,
                dataType: "json"
            }
        },
        schema: {
            parse: function (data) {
                console.log("dataToParse: " + JSON.stringify(data));
                return data;
            },
            model: {
                id: "MASTER_ACC_CODE",
                parentId: "PRE_ACC_CODE",
                children: "ITEMS",
                hasChildren: "HAS_BRANCH",
                //items:"ITEMS",
                fields: {
                    ACC_CODE: { field: "ACC_CODE", type: "string" },
                    ACC_EDESC: { field: "ACC_EDESC", type: "string" },
                    parentId: { field: "PRE_ACC_CODE", type: "string", defaultValue: "00" }
                }
            }
        }
    });

    $("#accountTreeBox").kendoTreeView({
        checkboxes: {
            checkChildren: true
        },
        autoScroll: true,
        autoBind: true,
        dragAndDrop: true,
        drop: onDrop,
        check: onLedgerCheck,
        dataSource: chartOfAccountDataSource,
        dataTextField: "ACC_EDESC",
        //height: 400,
        scrollable: {
            virtual: true
        }
    });

    function onLedgerCheck(e) {
        var checkedNodes = [],
            ledgerTreeView = $("#accountTreeBox").data("kendoTreeView");
        checkedNodeIds(ledgerTreeView.dataItem(e.node), checkedNodes);
    }

    function checkedNodeIds(nodes, checkedNodes) {
        $scope.checkedAccountObj = {
            "ACC_CODE": nodes.ACC_CODE,
            "ACC_EDESC": nodes.ACC_EDESC,
            "ACC_TYPE_FLAG": nodes.ACC_TYPE_FLAG,
            "ITEMS": []
        };
        if (nodes.HAS_BRANCH) {

            $scope.checkedAccountObj.ITEMS = nodes.children.options.data.ITEMS;
        }

        $scope.checkedAccountList.push($scope.checkedAccountObj);
    }  

    function onDrop(e) {

        var item = e.sender.dataItem(e.sourceNode);
        var listbox = $('#accountListBox').data('kendoListBox');
        console.log("item on drop==============>>>" + JSON.stringify(item));
        console.log("item on drop=====Item=========>>>" + JSON.stringify(item.items));
        listbox.add(item);
        checkboxEventBinding();
        $("#selectAllListItem").attr("disabled", false);
        var ledgerTreeView = $("#accountTreeBox").data("kendoTreeView");
        var item1 = ledgerTreeView.dataItem(e.node);
        if (item.HAS_BRANCH) {
            var ITEM = kendo.data.Model.define({
                id: "ACC_CODE",
                fields: {
                    "ACC_EDESC": { type: "string" },
                    "ACC_TYPE_FLAG": { type: "string" }
                }
            });

            for (var p = 0; p < item.children.options.data.ITEMS.length; p++) {

                console.log("childrenView=============>>>" + JSON.stringify(item.children.options.data.ITEMS[p]));
                var tempObj = {
                    "ACC_CODE": item.children.options.data.ITEMS[p].ACC_CODE,
                    "ACC_EDESC": item.children.options.data.ITEMS[p].ACC_EDESC,
                    "ACC_TYPE_FLAG": item.children.options.data.ITEMS[p].ACC_TYPE_FLAG,
                    "ITEMS": [],
                    "index": p + 1,
                    "HAS_BRANCH": false,
                    "MASTER_ACC_CODE": item.children.options.data.ITEMS[p].MASTER_ACC_CODE,
                    "PRE_ACC_CODE": item.children.options.data.ITEMS[p].PRE_ACC_CODE,
                    "parentId": item.parentId
                };
                console.log("childrenView=====Obj========>>>" + JSON.stringify(tempObj));
                listbox.add(new ITEM({
                    "ACC_CODE": item.children.options.data.ITEMS[p].ACC_CODE,
                    "ACC_EDESC": item.children.options.data.ITEMS[p].ACC_EDESC,
                    "ACC_TYPE_FLAG": item.children.options.data.ITEMS[p].ACC_TYPE_FLAG
                }));
               // listbox.read();
                //listbox.add(item.children.options.data.ITEMS);
            }
            //console.log("item:::: " + JSON.stringify(item.children.view()));
            //listbox.add(item.children.view());
            //listbox.add(item.children.options.data.ITEMS);
            //for (var p = 0; p < item.children.options.data.ITEMS.length; p++) {
            //    listbox.add(item.children.options.data.ITEMS[p]);
            //}
        }
    }

    $scope.MapAccountDetailTreeToList = function () {
        var listbox = $('#accountListBox').data('kendoListBox');
        console.log("$scope.checkedLedger to Map: " + JSON.stringify($scope.checkedAccoutList));
        var ITEM = kendo.data.Model.define({
            id: "ACC_CODE",
            fields: {
                "ACC_EDESC": { type: "string" },
                "ACC_TYPE_FLAG": { type: "string" }
            }
        });
       
        listbox.add(new ITEM({
            "ACC_CODE": $scope.checkedAccountList[0].ACC_CODE,
            "ACC_EDESC": $scope.checkedAccountList[0].ACC_EDESC,
            "ACC_TYPE_FLAG": $scope.checkedAccountList[0].ACC_TYPE_FLAG
        }
        ));

        for (i = 0; i < $scope.checkedAccountList[0].ITEMS.length; i++) {
            listbox.add(new ITEM({
                "ACC_CODE": $scope.checkedAccountList[0].ITEMS[i].ACC_CODE,
                "ACC_EDESC": $scope.checkedAccountList[0].ITEMS[i].ACC_EDESC,
                "ACC_TYPE_FLAG": $scope.checkedAccountList[0].ITEMS[i].ACC_TYPE_FLAG
            }));
        }
        $scope.checkedAccountList.length = 0;
        $scope.checkedAccountObj = {};

        $("#selectAllListItem").attr("disabled", false);
        checkboxEventBinding();

    };
 
    $scope.accountListBoxOptions = {
        template: kendo.template($("#checkBoxInListBoxTemplate").html()),
        add: function (e) {
            addItems(this.dataSource, e.dataItems);
           // checkboxEventBinding();
           // $("#selectAllListItem").attr("disabled", false);
        }
    };

    function addItems(dataSource, items) {
        items.forEach(function (item) {
            dataSource.data().push(item);
        });
    }

    $scope.selectedAccountDetail = [];
    $scope.SaveCashBankAccountDetail = function () {

        var listBox = $("#accountListBox").data("kendoListBox");
        var dataItems = listBox.select();
        for (var i = 0; i < dataItems.length; i++) {

            console.log("selectedItemToSave=====================>>" + JSON.stringify(listBox.dataItem(dataItems[i])));

            $scope.selectedAccountDetail.push({
                "ACC_CODE": JSON.stringify(listBox.dataItem(dataItems[i]).ACC_CODE),
                "ACC_EDESC": JSON.stringify(listBox.dataItem(dataItems[i]).ACC_EDESC),
                "ACC_TYPE_FLAG": $scope.CashBankSetupObj.ACC_TYPE_FLAG
            });

        }

        if ($scope.selectedAccountDetail.length > 0) {

            var saveModel = {
                CashBankDetail: $scope.CashBankSetupObj,
                ListInfo: $scope.selectedAccountDetail
            };

            var saveResult = cashBankSetupService.SaveCashBankAccountDetail(saveModel);
            saveResult.then(function (savRes) {
                if (savRes.data === "Successfull") {
                    DisplayBarNotificationMessage("Cash Bank setup created successfully");
                    setTimeout(function () {
                        location.reload(true);
                    }, 3000);
                } else {
                    DisplayBarNotificationMessage("Error while setting cash and bank setup");
                }
            });

        } else {
            displayPopupNotification("Please selected accout or item for setup", "error");
            return;
        }
    };

});

DTModule.service('cashBankSetupService', function ($http) {

    this.SaveCashBankAccountDetail = function (parameter) {
        var saveResponse = $http({
            method: "POST",
            url: "/api/CustomFormApi/SaveCashBankAccountDetail",
            data: JSON.stringify(parameter),
            dataType: "JSON"
        });
        return saveResponse;
    };

    this.generateCBId = function () {
        var rootDetail = $http({
            method: 'GET',
            url: "/api/CustomFormApi/GenerateCBID",
            dataType: "JSON"
        });
        return rootDetail;
    };

    this.getCashBankRootDetail = function (acc_code) {
        var rootDetail = $http({
            method: 'GET',
            url: "/api/CustomFormApi/GetCashBankRootDetail?acc_code=" + acc_code,
            dataType:"JSON"
        });
        return rootDetail;
    };

    this.deleteCashBankSetup = function (cbCode) {
        var delRes = $http({
            method: 'GET',
            url: "/api/CustomFormApi/DeleteCashBankSetup?cbCode=" + cbCode,
            dataType: "JSON"
        });
        return delRes;
    };

});