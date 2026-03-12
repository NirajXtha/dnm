distributionModule.controller('OpeningStockCtrl', function ($scope, $routeParams, OpeningStockService) {
    $scope.DistributerCode;
    $scope.StockId = 0;
    $scope.FormVisibility = false;
    $scope.ViewForm = function () {
        $scope.FormVisibility = true;
    }

    $scope.distributorDetails = [];

    $scope.AddItem = function (itemCode, cuStock, mucode) {
        if ($scope.DistributerCode.length < 1)
            bootbox.alert("Please select distributor first");
        else {
            var tarr = {
                ITEM_CODE: typeof itemCode !== 'undefined' ? itemCode : '',
                CURRENT_STOCK: typeof cuStock !== 'undefined' ? cuStock : '',
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
            };
            $scope.distributorDetails.unshift(tarr);
        }
    }

    $scope.RemoveItem = function (index) {
        $scope.distributorDetails.splice(index, 1);
    }

    $scope.CancelForm = function (Pro) {
        $scope.selecteddistCustomer = [];
        $scope.StockId = 0;
        $scope.FormVisibility = false;
        $scope.distributorDetails = [];
        Pro = typeof (Pro) == "undefined" ? "" : Pro;
        $scope.DistributerCode = _.isEmpty(Pro.toString()) ? [] : [Pro];
        
    }

    $scope.SaveOpeningStock = function () {
        if (!$scope.OpeningForm.$valid) {
            displayPopupNotification("Invalid Input fields.", "warning");
            return;
        }
        if ($scope.distributorDetails.length == 0) {
            displayPopupNotification("Please select at least one item", "warning");
            return;
        }
        for (let i = 0; i < $scope.distributorDetails.length; i++) {
            if ($scope.distributorDetails[i].ITEM_CODE == '' || $scope.distributorDetails[i].CURRENT_STOCK == '' || $scope.distributorDetails[i].MU_CODE == '') {
                displayPopupNotification("Invalid Input fields.", "warning");
                return;
            }
        }
        var NewData = $.extend(true, {}, $scope.distributorDetails);
        for (let i = 0; i < $scope.distributorDetails.length; i++) {
            $scope.distributorDetails[i].ITEM_CODE = NewData[i].ITEM_CODE[0];
            $scope.distributorDetails[i].MU_CODE = NewData[i].MU_CODE[0];
        }
        OpeningStockService.SaveOpeningStock($scope.distributorDetails, $scope.DistributerCode[0], $scope.StockId).then(function (result) {
            displayPopupNotification(result.data.MESSAGE, result.data.TYPE);
            if (result.data.TYPE == "success")
                $scope.CancelForm();
            $("#grid").data("kendoGrid").dataSource.read();
        }, function (ex) {
            displayPopupNotification("Something went wrong. Please try again.", "error");
        });
    };

    //GetDistributorList
    $scope.distributerSelectOptions = {
        dataTextField: "DISTRIBUTOR_NAME",
        dataValueField: "DISTRIBUTOR_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Distributors...</strong></div>',
        placeholder: "Select Distributors...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetDistributorList",
                    dataType: "json",
                    type: "POST",
                },
            }
        },
    };

    //Get Items for Distributors    
    $scope.ItemSelectOptions = {
        dataTextField: "ITEM_EDESC",
        dataValueField: "ITEM_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Items...</strong></div>',
        placeholder: "Select item...",
        autoClose: false,
        dataBound: function (e) {
            //for single element
            //var current = this.value();
            //this._savedOld = current.slice(0);
            //$("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });

            //for multiple elements
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/DistributionPurchase/GetItems",
                    dataType: "json",
                   
                },
            }
        }
    };

    $scope.ItemChange = function (e, index) {
        var selected = e.dataItem;
        if (!e.update) {
            var PreviouslySelected = [];
            for (var i = 0; i < $scope.distributorDetails.length; i++) {
                var val = $scope.distributorDetails[i].ITEM_CODE[0];
                PreviouslySelected.push(val);
            }
            if (PreviouslySelected.includes(selected.ITEM_CODE)) {
                e.preventDefault();
                displayPopupNotification("Item already selected", "warning");
            }
            $scope.distributorDetails[index].MU_CODE = [selected.UNIT];
        }
        var selectDS = [];
        selectDS.push({ UNITID: selected.UNIT, TEXT: selected.UNIT });
        if (selected.CONVERSION_FACTOR)
            selectDS.push({ UNITID: selected.CONVERSION_UNIT, TEXT: selected.CONVERSION_UNIT });
        $scope.distributorDetails[index].MuOptions.dataSource = selectDS;
    }

    $scope.ItemRemove = function (e, index) {
        $scope.distributorDetails[index].MU_CODE = '';
        $scope.distributorDetails[index].MuOptions.dataSource = [];
    }

    $scope.mainGridOptions = {
        dataSource: {
            type: "JSON",
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/Setup/GetOpeningStock",
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
            pageSizes: [100, 200, 500, 1000],
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
        scrollable: {
            virtual: true
        },
        dataBound: function (o) {
            GetSetupSetting("OpeningStockSetup");
            var grid = this;
            if ($("#grid").data("kendoGrid").dataItems().length==0) {
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
                    updateEvent(grid.dataItem(this));
                });
            }

            $('div').removeClass('.k-header k-grid-toolbar');
            $("#allItems").parent().hide();
        },
        columns: [{
            field: "DistributerName",
            title: "Distributer",
            width: "30%",
        }, {
            field: "DistributerAddress",
            title: "Address",
            width: "30%"
        }, {
            field: "StockDate",
            title: "Date",
            format: "{0:dd-MMM-yyyy}",
            width: "30%",
        }, {
            title: "Action",
            template: " <a class='fa fa-edit editAction' ng-click='EditStock($event)' title='Edit'></a>",
            width: "10%",
        }]
    };

    $scope.detailGridOptions = function (dataItem) {
        return {
            dataSource: {
                data: dataItem.OpeningList,
            },
            scrollable: false,
            sortable: true,
            columns: [
                {
                    field: "DISTRIBUTOR_EDESC",
                    title: "Distributer",
                    width: "30%",
                }, {
                    field: "ITEM_EDESC",
                    title: "Item",
                    width: "30%",
                }, {
                    field: "MU_CODE",
                    title: "Mu Code",
                    width: "20%",
                }, {
                    field: "CURRENT_STOCK",
                    title: "Quantity",
                    width: "20%",
                },]
        };
    };

    $scope.EditStock = function (evt) {
        var grid = $("#grid").data("kendoGrid");
        var row = evt.target.closest("tr");
        var item = grid.dataItem(row);
        updateEvent(item);
    }

    var updateEvent = function (item) {
        $scope.DistributerCode = [item.DistributerCode];
        $scope.StockId = item.OpeningStockId;
        $scope.distributorDetails = [];
        for (let i = 0; i < item.OpeningList.length; i++) {
            $scope.AddItem([item.OpeningList[i].ITEM_CODE], item.OpeningList[i].CURRENT_STOCK, item.OpeningList[i].MU_CODE);
        }
        
        $scope.FormVisibility = true;
        //trigger select event for updating the unit selection datasource update
        setTimeout(function () {
            var allItems = $("#allItems").data("kendoMultiSelect").dataSource.data();
            var totallen = item.OpeningList.length;
            for (let i = 0; i < totallen; i++) {
                var currentItem = $.grep(allItems, function (e) { return e.ITEM_CODE == item.OpeningList[totallen - 1 - i].ITEM_CODE; });
                $("#ItemSelect_" + i).data("kendoMultiSelect").trigger("select", { dataItem: currentItem[0], update: true });
            }
        }, 200);
    }

    $scope.onDistCustomerSelect = function (e) {
        var distCode = e.dataItem.DISTRIBUTOR_CODE;
        $scope.ItemSelectOptions.dataSource.transport.read.url = window.location.protocol + "//" + window.location.host + "/api/DistributionPurchase/GetItems?distributorCode=" + distCode;
    }

    $scope.onDistCustomerDeselect = function (e) {
        if ($scope.distributorDetails.length > 0) {
            e.preventDefault();
            bootbox.confirm("<strong>All selected items will be lost.</strong><br /> Are you sure want to remove?", function (result) {
                if (result) {
                    $scope.distributorDetails = [];
                    $scope.DistributerCode = [];
                    $scope.$apply();
                }
            });
        }
    }
});