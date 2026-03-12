DTModule.controller('subLedgerMgmtCtrl', function ($scope, subLedgerMgmtService, $window, $filter) {
    console.log("controller hit");
    $scope.FormName = "Sub Ledger Management";
    $scope.selectedLedgerType = "01";
    $scope.textForLegend = "Customer";
    $scope.subLedgerTypeData = [];
    $scope.selectedAccountToMap = null;
    $scope.selectedLedgerForMapping = null;
    $scope.SubLedgerType = {
        "TypeCode": "",
        "TypeName": ""
    };
    $scope.SubLedgerTypeList = [];
    $scope.SubLedgerTypeList.push(
        {
            "TypeCode": "01",
            "TypeName": "CUSTOMER"
        },
        {
            "TypeCode": "02",
            "TypeName": "EMPLOYEE"
        },
        {
            "TypeCode": "03",
            "TypeName": "FIXED ASSETS"
        },
        {
            "TypeCode": "04",
            "TypeName": "Misc Subledger"
        },
        {
            "TypeCode": "05",
            "TypeName": "PROJECTS"
        },
        {
            "TypeCode": "06",
            "TypeName": "SUPPLIERS"
        },
        {
            "TypeCode": "07",
            "TypeName": "ChartOfItem"
        },
        {
            "TypeCode": "08",
            "TypeName": "CostCenter"
        }
    );

    $scope.selectedLedgerTypeOption = {

        dataSource: $scope.SubLedgerTypeList,
        filter: "contains",
        optionLabel: "--Select Ledger Type--",
        dataTextField: "TypeName",
        dataValueField: "TypeCode"
        //onChange: onDDLChange,

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
                return data;
            },
            model: {
                id: "MASTER_ACC_CODE",
                parentId: "PRE_ACC_CODE",
                children: "ITEMS",
                fields: {
                    ACC_CODE: { field: "ACC_CODE", type: "string" },
                    ACC_EDESC: { field: "ACC_EDESC", type: "string" },
                    parentId: { field: "PRE_ACC_CODE", type: "string", defaultValue: "00" },
                    //PRE_ACC_CODE: { field: "PRE_ACC_CODE", type: "string", defaultValue: "00" },
                }
            }
        }
    });

    $("#subLedgerAccountTree").kendoTreeView({
        loadOnDemand: false,
        //autoScroll: true,
        //autoBind: true,
        dataSource: chartOfAccountDataSource,
        dataTextField: "ACC_EDESC",
       // height: 400,
        select: onSubLedgerSelect,
        //scrollable: {
        //    virtual: true
        //}
    });


    $scope.selectedSubLedger = {
        "ACC_CODE": "",
        "ACC_EDESC": "",
        "MASTER_ACC_CODE": "",
        "PRE_ACC_CODE": "",
        "ACC_TYPE_FLAG": ""
    };

    function onSubLedgerSelect(e) {
        
        var treeview = $("#subLedgerAccountTree").data("kendoTreeView");
        var item = treeview.dataItem(e.node);
        //console.log("selected Tree Item==============>>> " + JSON.stringify(item));

        if (item) {

            $scope.selectedSubLedger = {
                "ACC_CODE": item.ACC_CODE,
                "ACC_EDESC": item.ACC_EDESC,
                "MASTER_ACC_CODE": item.MASTER_ACC_CODE,
                "PRE_ACC_CODE": item.parentId,
                "ACC_TYPE_FLAG": item.ACC_TYPE_FLAG
            };
            

        }

        $scope.selectedAccountToMap = $scope.selectedSubLedger.ACC_EDESC;
        //console.log("selectedSubLedger=======================>>" + JSON.stringify($scope.selectedSubLedger));

        var subLedgerMgmtGrid = $("#subLedgerMgmgGrid").data("kendoGrid");
        subLedgerMgmtGrid.dataSource.transport.options.read.url = window.location.protocol + "//" + window.location.host + "/api/SubLedgerMgmtApi/GetSubLedgerMappingForGrid?subCode=" + $scope.selectedSubLedger.ACC_CODE + "&masterCode=" + $scope.selectedSubLedger.MASTER_ACC_CODE + "&preCode=" + $scope.selectedSubLedger.PRE_ACC_CODE;
        subLedgerMgmtGrid.dataSource.read();

    }

    $scope.closeSubLegerModal = function () {

        var listBox = $("#selected").data("kendoListBox");
        listBox.remove(listBox.items());

        $('#selectAllListItem').prop("checked", false);

        var tree = $("#subLedgerTreeView").data("kendoTreeView");
        tree.dataSource.read();
        $scope.checkedLedgerList.length = 0;
        $('#subLedgerSetupModal').toggle();

    };

    $scope.mapFromTreeToList = function () {
        var listbox = $('#selected').data('kendoListBox');
        console.log("$scope.checkedLedger to Map: " + JSON.stringify($scope.checkedLedgerList));

        var ITEM = kendo.data.Model.define({
            id: "TYPE_CODE",
            fields: {
                "TYPE_EDESC": { type: "string" },
                "LINK_SUB_CODE": { type: "string" },
                "GROUP_SKU_FLAG": {type:"string"}
            }
        });

        listbox.add(new ITEM({
            "TYPE_CODE": $scope.checkedLedgerList[0].TYPE_CODE,
            "TYPE_EDESC": $scope.checkedLedgerList[0].TYPE_EDESC,
            "LINK_SUB_CODE": $scope.checkedLedgerList[0].LINK_SUB_CODE,
            "GROUP_SKU_FLAG": $scope.checkedLedgerList[0].GROUP_SKU_FLAG
        }
        ));

        for (i = 0; i < $scope.checkedLedgerList[0].ITEMS.length; i++) {
            listbox.add(new ITEM({
                "TYPE_CODE": $scope.checkedLedgerList[0].ITEMS[i].TYPE_CODE,
                "TYPE_EDESC": $scope.checkedLedgerList[0].ITEMS[i].TYPE_EDESC,
                "LINK_SUB_CODE": $scope.checkedLedgerList[0].ITEMS[i].LINK_SUB_CODE,
                "GROUP_SKU_FLAG": $scope.checkedLedgerList[0].ITEMS[i].GROUP_SKU_FLAG
            }));
        }

        //listbox.add($scope.checkedLedgerList[0]);
        //listbox.add($scope.checkedLedgerList[0].ITEMS);
        $scope.checkedLedgerList.length = 0;
        $scope.checkedLedgerObj = {};

        $("#selectAllListItem").attr("disabled", false);
        checkboxEventBinding();

    };

    $scope.ledgerTypeChange = function () {
        console.log("selectedLedgerType=-===============>>" + $scope.selectedLedgerType);

        if ($scope.selectedLedgerType === "01") {

            $scope.checkedLedgerList.length = 0;
            $scope.checkedLedgerObj = {};

            $scope.textForLegend = "Customer";

            var customerUrl = window.location.protocol + "//" + window.location.host + "/api/SubLedgerMgmtApi/GetCustomerSubLedger";
            var custDataSource = new kendo.data.HierarchicalDataSource({
                transport: {
                    read: {
                        url: customerUrl,
                        dataType: "json"
                    }
                },
                schema: {
                    parse: function (data) {
                        return data;
                    },
                    model: {
                        id: "MASTER_CUSTOMER_CODE",
                        parentId: "PRE_CUSTOMER_CODE",
                        children: "ITEMS",
                        items: "ITEMS",
                        fields: {
                            TYPE_CODE: { field: "TYPE_CODE", type: "string" },
                            TYPE_EDESC: { field: "TYPE_EDESC", type: "string" },
                            parentId: { field: "PRE_CUSTOMER_CODE", type: "string", defaultValue: "00" }
                        }
                    }
                }
            });

            $("#subLedgerTreeView").data("kendoTreeView").setDataSource(custDataSource);

        }
        else if ($scope.selectedLedgerType === "02") {

            $scope.textForLegend = "Employee";
            $scope.checkedLedgerList.length = 0;
            $scope.checkedLedgerObj = {};
            var employeeUrl = window.location.protocol + "//" + window.location.host + "/api/SubLedgerMgmtApi/GetEmployeeSubLedger";
            var employeeDataSource = new kendo.data.HierarchicalDataSource({
                transport: {
                    read: {
                        url: employeeUrl,
                        dataType: "json"
                    }
                },
                schema: {
                    model: {
                        id: "TYPE_CODE",
                        children: "ITEMS",
                        hasChildren: "HAS_BRANCH"
                    },
                    parse: function (response) {
                        return _.each(_.filter(response, function (x) {
                            return x.PRE_EMPLOYEE_CODE === '00';
                        }), function (y) {
                            y.ITEMS = _.filter(response, function (z) {
                                return z.PRE_EMPLOYEE_CODE === y.MASTER_EMPLOYEE_CODE;
                            });
                            y.HAS_BRANCH = y.ITEMS.length === 0 ? false : true;
                        });
                    }
                }
            });

            $("#subLedgerTreeView").data("kendoTreeView").setDataSource(employeeDataSource);


        }
        else if ($scope.selectedLedgerType === "03") {

            $scope.textForLegend = "Fixes Assets";
            $scope.checkedLedgerList.length = 0;
            $scope.checkedLedgerObj = {};
            $("#subLedgerTreeView").data("kendoTreeView").setDataSource({});

        }
        else if ($scope.selectedLedgerType === "04") {

            $scope.textForLegend = "Misc SubLedger";
            $scope.checkedLedgerList.length = 0;
            $scope.checkedLedgerObj = {};
            $("#subLedgerTreeView").data("kendoTreeView").setDataSource({});


        }
        else if ($scope.selectedLedgerType === "05") {
            $scope.textForLegend = "Projects";
            $scope.checkedLedgerList.length = 0;
            $scope.checkedLedgerObj = {};
            $("#subLedgerTreeView").data("kendoTreeView").setDataSource({});
        }
        else if ($scope.selectedLedgerType === "06") {
            $scope.textForLegend = "Supplier";
            $scope.checkedLedgerList.length = 0;
            $scope.checkedLedgerObj = {};
            var supplierUrl = window.location.protocol + "//" + window.location.host + "/api/SubLedgerMgmtApi/GetSupplierSubLedger";
            var supplierDataSource = new kendo.data.HierarchicalDataSource({
                transport: {
                    read: {
                        url: supplierUrl,
                        dataType: "json"
                    }
                },
                schema: {
                    model: {
                        id: "TYPE_CODE",
                        children: "ITEMS",
                        hasChildren: "HAS_BRANCH"
                    },
                    parse: function (response) {
                        return _.each(_.filter(response, function (x) {
                            return x.PRE_SUPPLIER_CODE === '00';
                        }), function (y) {
                            y.ITEMS = _.filter(response, function (z) {
                                return z.PRE_SUPPLIER_CODE === y.MASTER_SUPPLIER_CODE;
                            });
                            y.HAS_BRANCH = y.ITEMS.length === 0 ? false : true;
                        });
                    }
                }
            });

            $("#subLedgerTreeView").data("kendoTreeView").setDataSource(supplierDataSource);
        }
        else if ($scope.selectedLedgerType === "08") {

            $scope.textForLegend = "Cost Center";
            $scope.checkedLedgerList.length = 0;
            $scope.checkedLedgerObj = {};
            var employeeUrl = window.location.protocol + "//" + window.location.host + "/api/SubLedgerMgmtApi/GetCostCenterSubLedger";
            var employeeDataSource = new kendo.data.HierarchicalDataSource({
                transport: {
                    read: {
                        url: employeeUrl,
                        dataType: "json"
                    }
                },
                schema: {
                    parse: function (data) {
                        return data;
                    },
                    model: {
                        id: "TYPE_CODE",
                        parentId: "PRE_BUDGET_CODE",
                        children: "ITEMS",
                        items: "ITEMS",
                        fields: {
                            TYPE_CODE: { field: "TYPE_CODE", type: "string" },
                            TYPE_EDESC: { field: "TYPE_EDESC", type: "string" },
                            parentId: { field: "PRE_BUDGET_CODE", type: "string", defaultValue: "00" }
                        }
                    }
                }
            });

            $("#subLedgerTreeView").data("kendoTreeView").setDataSource(employeeDataSource);


        }
        else {
            $scope.textForLegend = "Chart Of Items";
            $scope.checkedLedgerList.length = 0;
            $scope.checkedLedgerObj = {};
            var itemUrl = window.location.protocol + "//" + window.location.host + "/api/SubLedgerMgmtApi/GetItemSubLedger";
            var itemDataSource = new kendo.data.HierarchicalDataSource({
                transport: {
                    read: {
                        url: itemUrl,
                        dataType: "json"
                    }
                },
                schema: {
                    model: {
                        id: "TYPE_CODE",
                        children: "ITEMS",
                        hasChildren: "HAS_BRANCH"
                    },
                    parse: function (response) {
                        return _.each(_.filter(response, function (x) {
                            return x.PRE_ITEM_CODE === '00';
                        }), function (y) {
                            y.ITEMS = _.filter(response, function (z) {
                                return z.PRE_ITEM_CODE === y.MASTER_ITEM_CODE;
                            });
                            y.HAS_BRANCH = y.ITEMS.length === 0 ? false : true;
                        });
                    }
                }
            });

            $("#subLedgerTreeView").data("kendoTreeView").setDataSource(itemDataSource);
        }
    };

    $scope.subLedgerMgmgGridOptions = {
        dataSource: {
            type: "json",
            transport: {
                read: "/api/SubLedgerMgmtApi/GetSubLedgerMappingForGrid",
                parameterMap: function (options, type) {
                    if (type === 'read') {
                        //return {
                        //    from: "bom"
                        //};
                    }
                }
            },
            pageSize: 20,
            serverSorting: true
        },
        selectable: "single",
        scrollable: true,
        pageable: true,
        groupable: false,
        dataBound: function (e) {

            $("#subLedgerMgmgGrid tbody tr").css("cursor", "pointer");

        },
        columns: [
            {
                field: "ACC_CODE", hidden: true
            },
            {
                // field: "SKU_FLAG",
                field: "DESCRIPTION",
                title: "Description"
            },
            {
                field: "SUB_CODE",
                title: "Shortcut"
            },
            {
                //field: "MU_CODE",
                field: "ACC_EDESC",
                title: "Acc Description"
            },
            {
                field: "SUB_LEDGER_TYPE",
                title: "Sub Ledger Type"
            }
        ]
    };

    $scope.newSubledgerMap = function () {
        if ($scope.selectedSubLedger.ACC_CODE) {
            $('#subLedgerSetupModal').toggle();
            var customerL = subLedgerMgmtService.getCustomerSubLedger();
            customerL.then(function (cRes) {
                $scope.subLedgerTypeData = cRes.data;

                //$scope.subLedgerTypeOptions = {
                //    dataSource: $scope.subLedgerTypeData,
                //    filter:"contains",
                //    connectWith: "selected",
                //    dataTextField: "TYPE_EDESC",
                //    dataValueField: "TYPE_CODE",
                //    toolbar: {
                //        tools: ["transferTo", "transferFrom", "transferAllTo", "transferAllFrom", "remove"]
                //    }

                //};
            });

        }
    };

    $scope.selectedSubLedgerLstBoxOptions = {
        template: kendo.template($("#checkBoxInListBoxTemplate").html()),
        add: function () {
            checkboxEventBinding();
            $("#selectAllListItem").attr("disabled", false);
        }
    };

    $scope.selectCheckBoxRow = function () {
        // console.log($(this).closest('.click'));
        console.log("check box is checked" + this.checked);
        if (this.checked) {
            $('.item.click').parent().addClass('k-state-selected');
        } else {
            $('.item.click').parent().removeClass('k-state-selected');
        }
    };

    function checkboxEventBinding() {
        $('#selectAllListItem').bind('click', function (e) {
            if (this.checked) {
                $('.item.click input').attr('checked', 'checked');
                $('.item.click').parent().addClass('k-state-selected');

            }
            else {
                $('.item.click input').removeAttr('checked');
                $('.item.click').parent().removeClass('k-state-selected');

            }

        });
    }

    $scope.selectedItemList = [];
    function getSelectedListBoxItem(e) {

        //var element = e.sender.select();
        //console.log("element: " + element);
        //element.each(function () {
        //    var element = $(this);
        //    var input = element.children("input");

        //    input.prop("checked", element.hasClass("k-state-selected"));
        //});
        //var dataItem = e.sender.dataItem(element[0]);

        //if (dataItem) {
        //    $scope.selectedItemList.push({
        //        "TEXT": dataItem.TYPE_EDESC,
        //        "VALUE": dataItem.TYPE_CODE
        //    });
        //} 

    }

    $scope.applyLedgerMapping = function () {

        var listBox = $("#selected").data("kendoListBox");
        var dataItem = listBox.select();
        for (var i = 0; i < dataItem.length; i++) {

            console.log("selectedItemToSave=====================>>" + JSON.stringify(listBox.dataItem(dataItem[i])));

            $scope.selectedItemList.push({
                "TEXT": JSON.stringify(listBox.dataItem(dataItem[i]).TYPE_EDESC),
                "VALUE": JSON.stringify(listBox.dataItem(dataItem[i]).TYPE_CODE),
                "LINK_SUB_CODE": JSON.stringify(listBox.dataItem(dataItem[i]).LINK_SUB_CODE),
                "GROUP_SKU_FLAG": JSON.stringify(listBox.dataItem(dataItem[i]).GROUP_SKU_FLAG)
            });

        }

        if ($scope.selectedItemList.length > 0) {

            var saveModel = {
                ACCOUNT_INFO_EDESC: $scope.selectedSubLedger.ACC_EDESC,
                ACCOUNT_INFO_CODE: $scope.selectedSubLedger.ACC_CODE,
                SUB_CODE_INFO: $scope.selectedLedgerType,
                SUB_CODE_TEXT: $scope.textForLegend,
                LIST_INFO: $scope.selectedItemList
            };

            var saveResult = subLedgerMgmtService.SaveSubLedgerMapping(saveModel);
           
            saveResult.then(function (savRes) {
                if (savRes.data === "Successfull") {
                    DisplayBarNotificationMessage("Sub ledger Mapping created successfully");
                    setTimeout(function () {
                        location.reload(true);
                    }, 3000);
                } else {
                   
                    displayPopupNotification(savRes.data, "warning");
                    setTimeout(function () {
                        location.reload(true);
                    }, 3000);
                }
            }).catch(function (error) {
                DisplayBarNotificationMessage(JSON.stringify(error.data.ExceptionMessage));
                setTimeout(function () {
                    location.reload(true);
                }, 3000);
                console.log("Exception Thrown : Error : " + JSON.stringify(error));
            });

        } else {
            displayPopupNotification("Please selected item for mapping", "error");
            return;
        }
    };

    var getCustomerForLedgerUrl = window.location.protocol + "//" + window.location.host + "/api/SubLedgerMgmtApi/GetCustomerSubLedger";
    var customerLedgerDataSource = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: getCustomerForLedgerUrl,
                dataType: "json"
            }
        },
        schema: {
            parse: function (data) {
                return data;
            },
            model: {
                id: "MASTER_CUSTOMER_CODE",
                parentId: "PRE_CUSTOMER_CODE",
                children: "ITEMS",
                items: "ITEMS",
                fields: {
                    TYPE_CODE: { field: "TYPE_CODE", type: "string" },
                    TYPE_EDESC: { field: "TYPE_EDESC", type: "string" },
                    parentId: { field: "PRE_CUSTOMER_CODE", type: "string", defaultValue: "00" }
                }
            }
        }
    });

    $("#subLedgerTreeView").kendoTreeView({
        checkboxes: {
            checkChildren: true
        },
        dragAndDrop: true,
        drop: onDrop,
        check: onLedgerCheck,
        template: kendo.template($("#treeview-template").html()),
        dataTextField: "TYPE_EDESC",
        dataSource: customerLedgerDataSource,
        dataBound: function (o) {
            var grid = o.sender;
            if (grid.dataSource.total() === 0) {
                $(o.sender.wrapper)
                    .find('ul')
                    .append('<li class="kendo-data-row" style="font-size:12px;">Sorry, no data </li>');
                displayPopupNotification("No Data Found.", "info");
            }
        }
    });

    function checkedNodeIds(nodes, checkedNodes) {
        console.log("nodes========iiiiii===========>>>" + JSON.stringify(nodes));
        if (nodes.checked) {

            $scope.checkedLedgerObj = {
                "TYPE_CODE": nodes.TYPE_CODE,
                "TYPE_EDESC": nodes.TYPE_EDESC,
                "LINK_SUB_CODE": nodes.LINK_SUB_CODE,
                "GROUP_SKU_FLAG": nodes.GROUP_SKU_FLAG,
                "ITEMS": []
            };
            if (nodes.HAS_BRANCH) {

                $scope.checkedLedgerObj.ITEMS = nodes.children.options.data.ITEMS;
            }

            $scope.checkedLedgerList.push($scope.checkedLedgerObj);

        } else {
            $scope.checkedLedgerList.pop();
        }
    }

    $scope.checkedLedgerList = [];
    $scope.checkedLedgerObj = {
        "TYPE_CODE": "",
        "TYPE_EDESC": "",
        "LINK_SUB_CODE": "",
        "GROUP_SKU_FLAG":"",
        "ITEMS": []
    };

    function onLedgerCheck(e) {
        var checkedNodes = [],
            ledgerTreeView = $("#subLedgerTreeView").data("kendoTreeView");

        // checkedNodeIds(ledgerTreeView.dataSource.view(), checkedNodes);
        checkedNodeIds(ledgerTreeView.dataItem(e.node), checkedNodes);
    }


    // Delete button behavior
    $(document).on("click", ".k-icon.k-i-close-outline", function (e) {
        e.preventDefault();
        var treeview = $("#treeview").data("kendoTreeView");
        treeview.remove($(this).closest(".k-item"));
    });

    function onDrop(e) {

        var item = e.sender.dataItem(e.sourceNode);
        var listbox = $('#selected').data('kendoListBox');
        listbox.add(item);
        checkboxEventBinding();
        $("#selectAllListItem").attr("disabled", false);
        var ledgerTreeView = $("#subLedgerTreeView").data("kendoTreeView");
        if (item.HAS_BRANCH) {
            for (var p = 0; p < item.children.options.data; p++) {
                listbox.add(item.items[p]);
            }
        }
    }


    $("#serachOnLedgerTree").on("input", function () {
        $("#subLedgerAccountTree").data("kendoTreeView").select($());
        var query = this.value.toLowerCase();
        var dataSource = $("#subLedgerAccountTree").data("kendoTreeView").dataSource;

        filter(dataSource, query);
    });

   
    function filter(dataSource, query) {
        var hasVisibleChildren = false;
        var hasVisibleChildren = false;
        var data = dataSource instanceof kendo.data.HierarchicalDataSource && dataSource.data();
        console.log("data on Filter:  " + JSON.stringify(data));
        for (var i = 0; i < data.length; i++) {
            var item = data[i];
            var text = item.ACC_EDESC.toLowerCase();
            var itemVisible =
                query === true // parent already matches
                || query === "" // query is empty
                || text.indexOf(query) >= 0; // item text matches query

            var anyVisibleChildren = filter(item.children, itemVisible || query); // pass true if parent matches

            hasVisibleChildren = hasVisibleChildren || anyVisibleChildren || itemVisible;

            item.hidden = !itemVisible && !anyVisibleChildren;
        }

        if (data) {
            // Re-apply the filter on the children.
            dataSource.filter({ field: "hidden", operator: "neq", value: true });
            //dataSource.filter({ field: "ACC_EDESC", operator: "neq", value: true });
        }

        return hasVisibleChildren;
    }


});

DTModule.service('subLedgerMgmtService', function ($http) {

    this.getCustomerSubLedger = function () {
        var cLedger = $http({
            method: "GET",
            url: "/api/SubLedgerMgmtApi/GetCustomerSubLedger",
            dataType: "json"
        });
        return cLedger;
    };

    this.getEmployeeSubLedger = function () {
        var eLedger = $http({
            method: "GET",
            url: "/api/SubLedgerMgmtApi/GetEmployeeSubLedger",
            dataType: "JSON"
        });

        return eLedger;
    };

    this.getMiscSubLedger = function () {
        var mLedger = $http({
            method: "GET",
            url: "/api/SubLedgerMgmtApi/GetMiscSubLedger",
            dataType: "JSON"
        });
        return mLedger;
    };

    this.getSupplierSubLedger = function () {
        var sLedger = $http({
            method: "GET",
            url: "/api/SubLedgerMgmtApi/GetSupplierSubLedger",
            dataType: "JSON"
        });
        return sLedger;
    };

    this.getChartOfItemSubLedger = function () {
        var cItemSubLedger = $http({
            method: "GET",
            url: "/api/SubLedgerMgmtApi/GetItemSubLedger",
            dataType: "JSON"
        });
        return cItemSubLedger;
    };

    this.SaveSubLedgerMapping = function (dataToSave) {
        var saveRes = $http({
            method: "POST",
            url: "/api/SubLedgerMgmtApi/SaveSubLedgerMapping",
            data: JSON.stringify(dataToSave),
            dataType: "JSON"
        });
        return saveRes;
    };

});