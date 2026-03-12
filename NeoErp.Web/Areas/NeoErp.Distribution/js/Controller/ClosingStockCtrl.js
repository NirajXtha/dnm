distributionModule.controller('ClosingStockCtrl', function ($scope, distributorService, $routeParams, $http) {
    $scope.AllMonths = ["Shrawan", "Bhadra", "Ashoj", "Kartik", "Mangsir", "Poush", "Magh", "Chaitra", "Baisakh", "Jestha", "Ashadh", "Falgun"];
    $scope.pageName = "Add Closing Stock";
    $scope.saveAction = "Save";
    $scope.createPanel = false;
    var sn = 2;
    $scope.disable = true;
    $scope.ClosingStocks = [];
    $scope.additems = true;
    $scope.removeitems = true;
    $scope.itemlist = [];
    $scope.showFirstRow = true;
    $scope.isUpdate = false;
    $scope.actions = [{ text: 'Confirm', primary: true }, { text: 'Cancel' }];

    $scope.AddClickEvent = function () {
        $scope.createPanel = true;
    }

    $scope.AddClosingStock = function (itemCode, item, mucode) {
        if (!$scope.selecteddistCustomer || $scope.selecteddistCustomer.length < 1)
            bootbox.alert("Please select distributor first");
        else {
            if (typeof item == 'undefined')
                $scope.itemlist.unshift({
                    ITEM_CODE: typeof itemCode !== 'undefined' ? [itemCode] : '',
                    //Current_Stock: typeof cuStock !== 'undefined' ? cuStock : '',
                    MU_CODE: typeof mucode !== 'undefined' ? [mucode] : '',
                    MuOptions: {
                        dataTextField: "TEXT",
                        dataValueField: "UNITID",
                        height: 200,
                        valuePrimitive: true,
                        maxSelectedItems: 1,
                        headerTemplate: '<div class="col-md-offset-3"><strong>Unit...</strong></div>',
                        placeholder: "Select Unit...",
                        autoClose: false,
                        dataSource: [],
                    }
                });
            else {
                item.ITEM_CODE = [item.ITEM_CODE];
                item.MU_CODE = [item.MU_CODE];
                item.MuOptions = {
                    dataTextField: "TEXT",
                    dataValueField: "UNITID",
                    height: 200,
                    valuePrimitive: true,
                    maxSelectedItems: 1,
                    headerTemplate: '<div class="col-md-offset-3"><strong>Unit...</strong></div>',
                    placeholder: "Select Unit...",
                    autoClose: false,
                    dataSource: [],
                };
                $scope.itemlist.unshift(item);
            }
        }
    }

    $scope.RemoveClosingStock = function (index) {
        var itemLength = $scope.itemlist.length;
        if (itemLength > 0) {
            var currentIndex = index > itemLength ? index - itemLength : itemLength - index;
            $scope.itemlist.splice(currentIndex, 1);
        }
        else {
            $scope.itemlist.splice(index, 1);
        }
        //$scope.itemlist.splice(index, 1);

    }

    $scope.cancelClickEvent = function (disNo) {

        $scope.isUpdate = false;
        $scope.stockId = '';
        $("#ClosingStockCreateForm")[0].reset();
        $scope.createPanel = false;
        $scope.itemlist = [];
        disNo = typeof (disNo) == "undefined" ? "" : disNo;
        $scope.selecteddistCustomer = _.isEmpty(disNo.toString()) ? [] : [disNo];
    }

    $scope.distItemsSelectOptions = {
        dataTextField: "ITEM_EDESC",
        dataValueField: "ITEM_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Items...</strong></div>',
        placeholder: "Select Item...",
        autoClose: false,
        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/DistributionPurchase/GetItems",
                    dataType: "json"
                }
            }
        }
    };

    $scope.distCustomerSelectOptions = {
        dataTextField: "CUSTOMER_EDESC",
        dataValueField: "DISTRIBUTOR_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Distributor...</strong></div>',
        placeholder: "Select Distributor...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetIndividualCustomer",
                    dataType: "json"
                }
            }
        }
    }

    $scope.onDistCustomerSelect = function (e) {
        var distCode = e.dataItem.DISTRIBUTOR_CODE;
        $scope.distItemsSelectOptions.dataSource.transport.read.url = window.location.protocol + "//" + window.location.host + "/api/DistributionPurchase/GetItems?distributorCode=" + distCode;
    }

    $scope.onDistCustomerDeselect = function (e) {
        if ($scope.itemlist.length > 0) {
            e.preventDefault();
            bootbox.confirm("<strong>All selected items will be lost.</strong><br /> Are you sure want to remove?", function (result) {

                if (result) {
                    $scope.itemlist = [];
                    $scope.selecteddistCustomer = [];
                    $scope.$apply();
                }
            });
        }
    }

    $scope.onItemsChange = function (e, index) {

        var selected = e.dataItem;
        if (!e.update) {
            var PreviouslySelected = [];
            for (var i = 0; i < $scope.itemlist.length; i++) {
                var val = $scope.itemlist[i].ITEM_CODE[0];
                PreviouslySelected.push(val);
            }
            if (PreviouslySelected.includes(selected.ITEM_CODE)) {
                e.preventDefault();
                displayPopupNotification("Item already selected", "warning");
                return;
            }
            $scope.itemlist[index].MU_CODE = [selected.UNIT];
        }
        //unit multiselect
        var selectDS = [];
        selectDS.push({ UNITID: e.dataItem.UNIT, TEXT: e.dataItem.UNIT });
        if (e.dataItem.CONVERSION_FACTOR != '')
            selectDS.push({ UNITID: e.dataItem.CONVERSION_UNIT, TEXT: e.dataItem.CONVERSION_UNIT });
        $scope.itemlist[index].MuOptions.dataSource = selectDS;
    }

    $scope.ItemRemove = function (e, index) {
        $scope.itemlist[index].MU_CODE = '';
        $scope.itemlist[index].MuOptions.dataSource = [];
    }

    $scope.mainGridOptions = {
        dataSource: {
            type: 'JSON',
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/Setup/GetClosingStock",
            },
            pageSize: 100,
            schema: {
                model: {
                    fields: {
                        StockDate: { type: "date" },
                    }
                }
            },
            error: function (e) {
                displayPopupNotification("Sorry error occured while processing data", "error");
            },
        },

        toolbar: kendo.template($("#toolbar-template").html()),
        height: window.innerHeight - 100,
        sortable: true,
        reorderable: true,
        groupable: true,
        resizable: true,
        pageable: {
            refresh: true,
            pageSizes: [100, 200, 300, 500],
            buttonCount: 2
        },
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
        dataBound: function (o) {

            GetSetupSetting("ClosingStockSetup");
            var grid = this;
            if ($("#grid").data("kendoGrid").dataItems().length == 0) {
                var colCount = grid.columns.length;
                $(o.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                displayPopupNotification("No Data Found.", "info");
            }
            else {
                grid.tbody.find("tr").dblclick(function (e) {

                    if (e.srcElement.className == "k-icon k-i-expand" || e.srcElement.className == "k-hierarchy-cell" || e.srcElement.className == "k-icon k-i-collapse")
                        return;
                    e.preventDefault();
                    updateEvent(grid.dataItem(this), true);
                });
            }
            $('div').removeClass('.k-header k-grid-toolbar');
            $("#allItems").parent().hide();
        },
        columns: [{
            field: "DistributerName",
            title: "Distributor",
            width: "30%",
        }, {
            field: "DistributerAddress",
            title: "Address",
            width: "30%"
            //}, {
            //    field: "StockDate",
            //    title: "Date",
            //    format: "{0:dd-MMM-yyyy}",
            //    width: "30%",
            //}, {
            //    title: "Action",
            //    template: " <a class='fa fa-edit editAction' ng-click='EditStock($event)' title='Edit'></a>",
            //    width: "10%",
        }]
    };

    var tempdata = [];

    $scope.detailGridOptions = function (dataItem) {

        return {
            dataSource: {
                transport: {
                    read: {
                        url: window.location.protocol + "//" + window.location.host + "/" + "api/Setup/GetDistChildItems?distCode=" + dataItem.DistCustomer,
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                    }
                },

                schema: {
                    model: {
                        fields: {
                            CREATED_DATE: { type: "date" },
                        }
                    }
                },
                serverPaging: true,
                serverSorting: true,
                serverFiltering: true,
                pageSize: 5,

            },
            dataBound: function (e) {

                tempdata = e.sender._data;
            },
            scrollable: false,
            sortable: true,
            pageable: true,
            columns: [
                { field: "ITEM_EDESC", title: "Item", width: "30%" },
                { field: "MU_CODE", title: "MU Code", width: "20" },
                { field: "Current_STOCK", title: "Quantity", width: "20%" },
                {
                    field: "CREATED_DATE",
                    title: "Date",
                    format: "{0:dd-MMM-yyyy}",
                    width: "20%",
                },
            ]
        };
    };

    $scope.EditStock = function (evt) {

        var grid = $("#grid").data("kendoGrid");
        var row = evt.target.closest("tr");
        var item = grid.dataItem(row);
        updateEvent(grid.dataItem(row));
    }

    $scope.closingStockCreate = function (isValid) {

        if ($scope.selecteddistCustomer == undefined) {
            displayPopupNotification("Invalid Input fields.", "warning");
            return;
        }
        if ($scope.itemlist.length == 0) {
            displayPopupNotification("Please select at least one item", "warning");
            return;
        }
        angular.forEach($scope.itemlist, function (value, key) {

            if ($scope.itemlist[key].ITEM_CODE == '' || $scope.itemlist[key].mu_code == '') {
                displayPopupNotification("Invalid Input fields.", "warning");
                return;
            }
            $scope.itemlist[key].ITEM_CODE = $scope.itemlist[key].ITEM_CODE[0];
            $scope.itemlist[key].MU_CODE = $scope.itemlist[key].MU_CODE[0];
        });

        var details = [];
        for (var i = 0; i < $scope.itemlist.length; i++) {
            for (var j = 0; j < $scope.AllMonths.length; j++) {
                details.push({
                    ITEM_CODE: $scope.itemlist[i].ITEM_CODE,
                    Current_STOCK: $scope.itemlist[i][$scope.AllMonths[j]],
                    CREATED_DATE_STRING: _.filter($("#ddlDateFilterVoucher option"), function (x) { return x.value == $scope.AllMonths[j]; })[0].dataset.endDate,
                    MU_CODE: $scope.itemlist[i].MU_CODE,
                });
            }
        }

        var osDetail = {
            OS_Details: details,//$scope.itemlist,
            STOCK_ID: $scope.stockId,
            DistCustomer: $scope.selecteddistCustomer.toString()
        };
        var osentryurl = "";
        if (!$scope.isUpdate) {
            osentryurl = window.location.protocol + "//" + window.location.host + "/api/Setup/CreateClosingStock";
        }
        else {
            osentryurl = window.location.protocol + "//" + window.location.host + "/api/Setup/UpdateClosingStock";
        }
        var response = $http({
            method: "post",
            url: osentryurl,
            data: osDetail,
            dataType: "json"
        });

        return response.then(function (data) {
            displayPopupNotification("Closing stock created/updated Successfully.", "success");
            $("#ClosingStockCreateForm")[0].reset();
            $scope.itemlist = [];
            $scope.selecteddistCustomer = [];
            $("#grid").data("kendoGrid").dataSource.read();
            $scope.createPanel = false;
        }, function errorCallback(response) {
            displayPopupNotification("Error Occured", "error");
        });
    }

    var updateEvent = function (item, dblclick) {

        var DistId = item.DistCustomer;
        var Getdata = distributorService.GetClosingStock(DistId)
        Getdata.then(function (response) {

            var item = response.data[0];
            //$timeout(function () {
            //    DataTableGrid();
            //}, 10)
            $scope.stockId = item.STOCK_ID;
            $scope.selecteddistCustomer = [item.DistCustomer];
            $scope.itemlist = [];
            var grouped = _.groupBy(item.OS_Details, "ITEM_CODE");
            var details = [];
            _.each(grouped, function (x) {
                var detailObj = { ITEM_CODE: x[0].ITEM_CODE, MU_CODE: x[0].MU_CODE };
                for (var j = 0; j < x.length; j++) {
                    var month = _.filter($("#ddlDateFilterVoucher option"), function (y) { return (y.dataset.endDate == x[j].CREATED_DATE_STRING && $scope.AllMonths.includes(y.value)); })[0].value;
                    detailObj[month] = x[j].Current_STOCK;
                }
                details.push(detailObj);
            });
            //$scope.itemlist = details;
            item.OS_Details = details;
            for (let i = 0; i < item.OS_Details.length; i++) {
                $scope.AddClosingStock(item.OS_Details[i].ITEM_CODE, item.OS_Details[i], item.OS_Details[i].MU_CODE);
            }
            $scope.isUpdate = true;
            $scope.createPanel = true;
            $scope.disable = false;
            if (dblclick)
                $scope.$apply();
            //trigger select event for updating the unit selection datasource update
            setTimeout(function () {
                var allItems = $("#allItems").data("kendoMultiSelect").dataSource.data();
                var totallen = item.OS_Details.length;
                for (let i = 0; i < totallen; i++) {

                    var currentItem = $.grep(allItems, function (e) { return e.ITEM_CODE == item.OS_Details[totallen - 1 - i].ITEM_CODE; });
                    $("#ItemSelect_" + i).data("kendoMultiSelect").trigger("select", { dataItem: currentItem[0], update: true });
                }
            }, 200);
        });


    }
});