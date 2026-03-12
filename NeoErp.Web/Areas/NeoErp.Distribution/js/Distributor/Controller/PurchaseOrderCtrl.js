distributionModule.controller("mvcCRUDCtrl", function ($scope, crudAJService) {
    debugger;
    var cFactor = "";
    var salesRate = "";
    $scope.PurchaseOrder = [];
    $scope.AddPurchaseOrder = function () {
        debugger;
        var available = $scope.PurchaseOrder;
        var i = $scope.PurchaseOrder.length;
        $scope.PurchaseOrder = [];
        $scope.PurchaseOrder.push({
            SN: i + 1,
            selectedItems: '',
            reqQuantity: '',
            unitPrice: '',
            cUnit: '',
            cQuantity: '',
            totalPrice: '',
        });
        for (var i = 0; i < available.length; i++) {
            var item = available[i];
            $scope.PurchaseOrder.push(item);
        }
    }
    $scope.AddPurchaseOrder();

    $scope.distItemsSelectOptions = {
        dataTextField: "ITEM_EDESC",
        dataValueField: "ITEM_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Items...</strong></div>',
        placeholder: "Select Items...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        change: function (evt) {
            var selectedData = evt.sender.dataItems()[0];
            var row = evt.sender.element.parent().parent().parent();
            var index = $("tr", $(row).closest("table")).index(row) - 1;
            try {

                if (selectedData == undefined) {
                    $($(".reqUnit")[index]).val("").trigger("change");
                    $($(".unitPrice")[index]).val("").trigger("change");
                    $($(".cUnit")[index]).val("").trigger("change");
                    $($(".reqQuantity")[index]).val("").trigger("change");
                    $($(".cQuantity")[index]).val("").trigger("change");
                    $($(".totalPrice")[index]).val("").trigger("change");
                }
                else {
                    evt.sender.element.closest("tbody").find("select.distItemsSelect").not(row.find("select.distItemsSelect")).each(function (k, r) {
                        var sv = $(r).find("option:selected").val();
                        if (sv == selectedData.ITEM_CODE) {
                            throw "Item has been already selected";

                        }
                    });
                    $($(".reqQuantity")[index]).val(1).trigger("change");
                    $($(".reqUnit")[index]).val(evt.sender.dataItems()[0].UNIT).trigger("change");
                    $($(".reqUnita")[index]).val(evt.sender.dataItems()[0].CONVERSION_FACTOR).trigger("change");
                    $($(".unitPrice")[index]).val(evt.sender.dataItems()[0].SALES_RATE).trigger("change");
                    $($(".cUnit")[index]).val(evt.sender.dataItems()[0].CONVERSION_UNIT).trigger("change");
                    cFactor = evt.sender.dataItems()[0].CONVERSION_FACTOR;
                    salesRate = evt.sender.dataItems()[0].SALES_RATE;
                }
            }
            catch (err) {
                var index;
                // $scope.myfuncton = function (index) {
                var newIndex = (index * 2) + 1;
                var multiselect = $($('.distItemsSelect')[newIndex]).data("kendoMultiSelect");
                multiselect.value([]);
                // };
                alert(err);
            }

        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Distributor/GetDistributorItems",
                    dataType: "json"
                }
            }
        }
    };

    //var bsToDate = "2017-jul-01";
    //var bsFromDate = "2016-jul-01";
    //var report = ReportFilter.filterAdditionalData();
    //report.ReportFilters.ToDate = bsToDate;
    //report.ReportFilters.FromDate = bsFromDate
    //var dateFilter = JSON.stringify($.extend({}, report));

    var getData = crudAJService.getLoginPurchaseOrder();
    getData.then(function (response) {
        $scope.PurchaseOrderList = response.data;
    });

    $scope.UpdatePo = function () {
        var QuantityArry = $scope.PurchaseOrder;
        if ((_.filter(QuantityArry, function (x) {
            return x.reqQuantity < 0 || x.reqQuantity == "" || x.reqQuantity == null || x.reqQuantity == undefined || x.selectedItems < 0 || x.selectedItems == "" || x.selectedItems == null || x.selectedItems == undefined  
        }).length <= 0) == false) {
            displayPopupNotification("Invalid Field", "warning")
            return
        }
        else {
            debugger;
            var ITEM_CODE = $("#distItemsSelect").val()[0];
            var selectedPOArray = $scope.PurchaseOrder;
            var customerCode = $scope.customerByDealerModel;
            var customerName = $('#customerByDealer').data("kendoDropDownList").dataItem().Name;
            data = {
                ITEM_CODE: ITEM_CODE,
                CUSTOMER_CODE: customerCode,
                CUSTOMER_EDESC: customerName,
                Remarks: $("#remarks").val(),
                selectedPOArray: selectedPOArray,
            }
              var savePO = crudAJService.savePurchaseOrder(data);
            savePO.then(function (result) {
                if (result.data == "Purchase order successfully created") {
                    displayPopupNotification(result.data, "success");
                    $scope.CancelledItem();
                    $scope.RemovePurchaseOrder();
                    setTimeout(function () {
                        location.reload();
                    }, 1000);
                }
                else {
                    displayPopupNotification("Something Went Wrong", "error");
                }

            })
        };


    }
    $scope.reqChangeFunction = function (index) {
        var reqQuantity = $($(".reqQuantity")[index]).val();
        if ((reqQuantity > 0) == false) {
            displayPopupNotification("Quantity can not be 0", "warning")
        }
        else {
            var sumFactor = (reqQuantity * $($(".reqUnita")[index]).val())
            sumTotal = (salesRate * reqQuantity)
            $scope.PurchaseOrder[index].cQuantity = sumFactor;
            $scope.PurchaseOrder[index].totalPrice = sumTotal;
        }

    }

    $scope.CancelledItem = function (index) {

        $(".reqQuantity").val("");
        $(".unitPrice").val("");
        $(".cUnit").val("");
        $(".cQuantity").val("");
        $(".totalPrice").val("");
        $(".reqUnit").val("");
        //var multiSelect = $('#distItemsSelect').data("kendoMultiSelect");
        //multiSelect.value([]);
        $scope.RemovePurchaseOrder();
    }

    $scope.RemovePurchaseOrder = function (index) {
        if (index == undefined) {

            var indexVal = $scope.PurchaseOrder.length;
            $scope.PurchaseOrder.splice(1, indexVal);
            var multiSelect = $('#distItemsSelect').data("kendoMultiSelect");
            multiSelect.value([]);

        }
        else {
            for (var i = 0; i < $scope.PurchaseOrder.length; i++) {
                $scope.PurchaseOrder[i].SN = $scope.PurchaseOrder.length - i;

            }
            if ($scope.PurchaseOrder.length != 1) {
                $scope.PurchaseOrder.splice(index, 1);
            }
        }
    };

    var customerByDealerUrl = window.location.protocol + "//" + window.location.host + "/api/Setup/GetCustomerByDealer";
    $scope.customerByDealerDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: customerByDealerUrl
            }
        }
    });

    $scope.customerByDealerOptions = {

        dataSource: $scope.customerByDealerDataSource,
        filter: "contains",
        optionLabel: "Select Customer",
        dataTextField: "Name",
        dataValueField: "Code"
    };
});