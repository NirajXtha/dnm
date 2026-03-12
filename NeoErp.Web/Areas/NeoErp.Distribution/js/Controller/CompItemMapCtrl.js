
distributionModule.controller('CompItemMapCtrl', function ($scope, DistSetupService, $routeParams) {
    
    $scope.MappedItems = [];

    $scope.$on('$routeChangeSuccess', function () {
        $scope.AddItemMap();
        DistSetupService.GetAllCompMaps().then(function (res) {
            if (!res.data.length < 1) {
                res.data.forEach(function (val, key) {
                    res.data[key].ITEM_CODE = [res.data[key].ITEM_CODE];
                });
                $scope.MappedItems = res.data;
            }
        }, function (ex) {
            displayPopupNotification("Cannot get items", "error");
        });
    });

    $scope.distItemSelect = {
        dataTextField: "ITEM_EDESC",
        dataValueField: "ITEM_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Select item...</strong></div>',
        placeholder: "Select item...",
        autoClose: false,
        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/DistributionPurchase/GetAllItems?type=C",
                    dataType: "json"
                }
            }
        },
        select: function (e) {
            
            var PreviouslySelected = $scope.MappedItems.map(a =>  a.ITEM_CODE ? a.ITEM_CODE[0] : "current");

            if (PreviouslySelected.includes(e.dataItem.ITEM_CODE.toString())) {
                e.preventDefault();
                displayPopupNotification("Item already selected", "warning");
            }
        }
    };

    $scope.distCompItemSelect = {
        dataTextField: "ITEM_EDESC",
        dataValueField: "ITEM_CODE",
        height: 600,
        valuePrimitive: true,
        //maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Select competitor items...</strong></div>',
        placeholder: "Select competitor items...",
        autoClose: false,
        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetCompItems",
                    dataType: "json"
                }
            }
        }
    };

    $scope.AddItemMap = function () {
        $scope.MappedItems.unshift({
            ITEM_CODE: null,
            COMP_ITEMS: null,
        });
    }

    $scope.RemoveItemMap = function (idx) {
        
        $scope.MappedItems.splice(idx, 1);
    }

    $scope.SaveItemMap = function () {
        
        var valid = $scope.ItemMapForm.$valid;
        if (!valid) {
            var errorName = $scope.ItemMapForm.$error.required[0].$name;
            $("[name=" + errorName + "]").data("kendoMultiSelect").open(); //$("#" + errorName) can also be used since Id and name are same
            displayPopupNotification("Invalid Input fields", "warning");
            return;
        }
        $scope.MappedItems.forEach(function (val, key) {
            $scope.MappedItems[key].ITEM_CODE = $scope.MappedItems[key].ITEM_CODE[0];
        });
        DistSetupService.SaveCompMap($scope.MappedItems).then(function (res) {
            displayPopupNotification(res.data.MESSAGE, res.data.TYPE);
            if (res.data.TYPE == "success")
                location.reload();
        }, function (ex) {
            displayPopupNotification("Something went wrong", "error");
        });
    }

});

distributionModule.controller('CompFieldCtrl', function ($scope, DistSetupService, $routeParams) {

    $scope.Item = {
        ITEM_CODE: null,
        FIELDS: [],
    };

    $scope.$on('$routeChangeSuccess', function () {
        $scope.AddField();
    });

    $scope.AddField = function () {
        var uid = $scope.Item.uid;
        $scope.Item.FIELDS.unshift({
            COL_NAME: null,
            COL_DATA_TYPE: null,
        });
        $("#grid").data("kendoGrid").tbody.find("tr[data-uid='" + uid + "']").addClass("k-state-selected");
    }

    $scope.distItemSelect = {
        dataTextField: "ITEM_EDESC",
        dataValueField: "ITEM_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Select item...</strong></div>',
        placeholder: "Select item...",
        autoClose: false,
        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/DistributionPurchase/GetAllItems?type=C",
                    dataType: "json"
                }
            }
        },
    };

    $scope.DataTypeOptions = {
        dataTextField: "TEXT",
        dataValueField: "VALUE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Select type...</strong></div>',
        placeholder: "Select type...",
        autoClose: false,
        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: [
            { TEXT: "Text Field", VALUE: "TXT" },
            { TEXT: "Numeric Field", VALUE: "NUM" },
            { TEXT: "YES/NO", VALUE: "BOL" },
        ],
    };

    $scope.SaveFields = function () {
        var valid = $scope.CompFieldForm.$valid;
        if (!valid) {
            var errorName = $scope.CompFieldForm.$error.required[0].$name;
            try {
                $("[name=" + errorName + "]").data("kendoMultiSelect").open(); //$("#" + errorName) can also be used since Id and name are same
            }
            catch (e) {
                $("[name=" + errorName + "]").focus();
            }
            displayPopupNotification("Invalid Input fields", "warning");
            return;
        }

        $scope.Item.ITEM_CODE = $scope.Item.ITEM_CODE[0];
        $scope.Item.FIELDS.forEach(function (val, key) {
            $scope.Item.FIELDS[key].COL_DATA_TYPE = $scope.Item.FIELDS[key].COL_DATA_TYPE[0];
        });

        DistSetupService.SaveCompField($scope.Item).then(function (res) {
            displayPopupNotification(res.data.MESSAGE, res.data.TYPE);
            if (res.data.TYPE == "success")
                $scope.Cancel();
            $("#grid").data("kendoGrid").dataSource.read();
        }, function (ex) {
            displayPopupNotification("Something went wrong", "error");
        });
    }

    $scope.Cancel = function () {
        $scope.Item = {
            ITEM_CODE: null,
            FIELDS: [],
        };
        $("#grid .k-state-selected").removeClass("k-state-selected");
        $scope.AddField();
    }

    $scope.RemoveField = function (idx) {
        var uid = $scope.Item.uid;
        $scope.Item.FIELDS.splice(idx, 1);
        $("#grid").data("kendoGrid").tbody.find("tr[data-uid='" + uid + "']").addClass("k-state-selected");
    }

    $scope.grid = {
        dataSource: {
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/Setup/GetCompFields",
            },
            //  pageSize: 20
        },
        dataBound: function (e) {
            GetSetupSetting("GetCompFields");
        },
        height: window.innerHeight - 50,
        groupable: false,
        sortable: false,
        height: 400,
        selectable: true,
        columns: [
        {
            field: "ITEM_EDESC",
            title: "Item",
            width: "100%",
        }],
        change: function (evt) {
            
            selectedRow = this.select();
            var item = angular.copy(this.dataItem(selectedRow));
            item.ITEM_CODE = [item.ITEM_CODE];
            item.FIELDS.forEach(function (val, key) {
                item.FIELDS[key].COL_DATA_TYPE = [item.FIELDS[key].COL_DATA_TYPE];
            });
            $scope.Item = item;
            //angular.copy(item, $scope.Item);
            $scope.$apply();
        }
    }

    $scope.onItemSelect = function (evt) {
        
        var itemCode = evt.dataItem.ITEM_CODE;
        var grid = $("#grid").data("kendoGrid");
        var dataSource = grid.dataSource.data();

        //select the grid row
        for (var i = 0; i < dataSource.length; i++) {
            var val = dataSource[i].ITEM_CODE;
            if (val.toString() == itemCode) {
                $scope.Cancel();
                grid.tbody.find("tr[data-uid='" + dataSource[i].uid + "']").addClass("k-state-selected");
                var item = angular.copy(dataSource[i]);
                item.ITEM_CODE = [item.ITEM_CODE];
                item.FIELDS.forEach(function (val, key) {
                    item.FIELDS[key].COL_DATA_TYPE = [item.FIELDS[key].COL_DATA_TYPE];
                });
                $scope.Item = item;
                evt.preventDefault();
                break;
            }
        }
    }

    $scope.onItemDeselect = function (evt) {
        $("#grid .k-state-selected").removeClass("k-state-selected");
    }

    $scope.onFieldNameChange = function (txt, idx) {
        txt = txt.trim().toLowerCase();
        if (txt == 'available' || txt == 'rate') {
            displayPopupNotification("'" + txt + "' Cannot be used again...", "warning");
            $scope.Item.FIELDS[idx].COL_NAME = "";
        }
    }

    $scope.SaveDefaults = function () {
        
        DistSetupService.SaveDefaultFields().then(function (res) {
            displayPopupNotification(res.data.MESSAGE, res.data.TYPE);
            if (res.data.TYPE == "success")
                $("#grid").data("kendoGrid").dataSource.read();
        }, function (ex) {
            displayPopupNotification("Something went wrong", "error");
        });
    }
});