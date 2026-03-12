DTModule.controller('SalesOrderAdjustmentCtrl', function ($scope, SalesOrderAdjustmentService, $filter) {

    $scope.TableName = "SA_SALES_ORDER";

    $scope.FormName = "Sales Order Adjustment";
    $scope.IndentAdjustFrom = $filter('date')(new Date(), 'dd-MMM-yyyy');
    $scope.IndentAdjustNepFrom = ""; 
    $scope.IndentAdjustTo = $filter('date')(new Date(), 'dd-MMM-yyyy');
    $scope.IndentAdjustNepTo = "";

    $scope.salesOrderAdjustDoc = null;

    $scope.SalesOrderFilter = "CAID";

    $scope.ConvertEngToNep = function () {
        console.log(this);

        var engdate = $("#englishDate5").val();
        var nepalidate = ConvertEngDateToNep(engdate);
        $("#nepaliDate5").val(nepalidate);
        $("#nepaliDate51").val(nepalidate);
    };

    $scope.ConvertNepToEng = function ($event) {
        console.log($(this));
        var date = BS2AD($("#nepaliDate5").val());
        var date1 = BS2AD($("#nepaliDate51").val());
        $("#englishdatedocument").val($filter('date')(date, "dd-MMM-yyyy"));
        $("#englishdatedocument1").val($filter('date')(date1, "dd-MMM-yyyy"));
        $('#nepaliDate5').trigger('change')
        $('#nepaliDate51').trigger('change')
    };

    $scope.ConvertEngToNepang = function (data) {
        $("#nepaliDate5").val(AD2BS(data));

    };

    $scope.ConvertEngToNepang1 = function (data) {

        $("#nepaliDate51").val(AD2BS(data));
    }

    $scope.someDateFn = function () {

        var engdate = $filter('date')(new Date(new Date().setDate(new Date().getDate() - 1)), 'dd-MMM-yyyy');
        var a = ConvertEngDateToNep(engdate);
        $scope.Dispatch_From = engdate;
        $scope.NepaliDate = a;
        $scope.Dispatch_To = a;
        $scope.PlanningTo = ConvertEngDateToNep($filter('date')(new Date(new Date().setDate(new Date().getDate())), 'dd-MMM-yyyy'));
    };

    $scope.someDateFn();

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

    $scope.monthSelectorOptionsSingle1 = {
        open: function () {

            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {

            $scope.ConvertEngToNepang1(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",


        dateInput: true
    };

    $scope.GetIndentDocument = function () {
        var indentDocument = SalesOrderAdjustmentService.getSalesOrderDocument($scope.TableName);
        indentDocument.then(function (res) {
            $scope.SalesOrderAdjustmentDoc = res.data;
        });
    };

    $scope.GetIndentDocument();

    var dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                method: "POST",
                url: "/api/PurchaseOrderIndentAdjustmentApi/GetAllPurchaseOrderAdjustment",
                dataType: "json"
            },
            parameterMap: function (options, type) {

                if (type === 'read') {
                    return {
                        document: "",
                        fromDate: "",
                        toDate: "",
                        indentFilter: ""
                    };
                }
            }
        },
        pageSize: 10
    });

    $("#sOrderAdjustmentGrid").kendoGrid({
        dataSource: dataSource,
        scrollable: true,
        filterable: true,
        sortable: true,
        pageable: true,
        reorderable: true,
        resizable: true,
        columnMenu: true,
        dataBound: function (e) {
            $(".checkbox").on("click", selectRow);

            var view = this.dataSource.data();
            for (var j = 0; j < checkedItems.length; j++) {
                for (var i = 0; i < view.length; i++) {
                    if (checkedItems[j].VOUCHER_NO == view[i].VOUCHER_NO && checkedItems[j].SERIAL_NO == view[i].SERIAL_NO) {
                        this.tbody.find("tr[data-uid='" + view[i].uid + "']")
                            .addClass("k-state-selected")
                            .find(".checkbox")
                            .attr("checked", "checked");
                    }
                }
            }
            var grid = e.sender;
            if (grid.dataSource.total() == 0) {
                var colCount = grid.columns.length + 1;
                $(e.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, No Data Found For Given Filter. </td></tr>');
            }

        },
        columns: [
            { field: "ORDER_NO", title: "Order No.", width: 70 },
            { field: "ORDER_DATE", title: "Order Date", width: 70 },
            { field: "CUSTOMER_EDESC", title: "Customer Name", width: 120 },
            { field: "ITEM_EDESC", title: "Item Name", width: 120 },
            { field: "UNIT", title: "Unit", width: 60 },
            { field: "QUANTITY", title: "Quantity", width: 60 },
            {
                template: '<input type="text" id="txtCancelQuantity_#=ROW_NO#" style="color:black;display:none"  onkeyup="validateCAQuantity(this,true)" />',
                title: "Cancel Qty.", width: 60
            },

            {
                template: '<input type="text" id="txtAdjustQuantity_#=ROW_NO#" style="color:black;display:none"  onkeyup="validateCAQuantity(this,false)" />',
                title: "Adjust Qty.", width: 60
            },
            {
                template: "<input type='checkbox' id='intendReq#=ROW_NO#' class='checkbox row-checkbox chkDispatch' />",
                width: 60,
                title: "Confirm Change"
            },
            { field: "CREATED_BY", title: "Amended By", width: 80 },
            { field: "CREATED_DATE", title: "Amendment Date", width: 80 },
        ]
    });

    var checkedItems = [];
    var checkedIds = {};
    $scope.ADJUST_QUANTITY = 0;
    $scope.CANCEL_QUANTITY = 0;

    function selectRow() {

        var checked = this.checked,
            row = $(this).closest("tr"),
            grid = $("#sOrderAdjustmentGrid").data("kendoGrid"),
            dataItem = grid.dataItem(row);
        if (checked) {
            row.addClass("k-state-selected");
            $(this).attr('checked', true);
            checkedIds[dataItem.ORDER_NO] = checked;
            $("#txtCancelQuantity_" + dataItem.ROW_NO).show();
            $("#txtAdjustQuantity_" + dataItem.ROW_NO).show();

            checkedItems.push({
                "ORDER_NO": dataItem.ORDER_NO,
                "ORDER_DATE": dataItem.ORDER_DATE,
                "SUPPLIER_CODE": dataItem.SUPPLIER_CODE,
                "ITEM_CODE": dataItem.ITEM_CODE,
                "UNIT": dataItem.UNIT,
                "QUANTITY": dataItem.QUANTITY,
                "CANCEL_QUANTITY": document.getElementById('txtCancelQuantity_' + dataItem.ROW_NO).value,
                "ADJUST_QUANTITY": document.getElementById('txtAdjustQuantity_' + dataItem.ROW_NO).value,
                "CONFIRM_CHANGE": true,
                "COMPANY_CODE": dataItem.COMPANY_CODE,
                "BRANCH_CODE": dataItem.BRANCH_CODE,
                "FORM_CODE": dataItem.FORM_CODE
            });
            $scope.checkedItemToSave = checkedItems;

        } else {
            row.removeClass("k-state-selected");
            $("#txtCancelQuantity_" + dataItem.ROW_NO).hide();
            $("#txtAdjustQuantity_" + dataItem.ROW_NO).hide();
            for (var i = 0; i < checkedItems.length; i++) {
                if (checkedItems[i].ORDER_NO == dataItem.ORDER_NO && checkedItems[i].SUPPLIER_CODE == dataItem.SUPPLIER_CODE) {
                    checkedItems.splice(i, 1);
                }
            }
        }
    }


    $scope.findNewSalesOrderAdjustment = function () {

        console.log("scope.selected Doc=====================>>>" + $scope.salesOrderAdjustDoc);
        var fromDate = $('#FromDateVoucher').val();
        var toDate = $('#ToDateVoucher').val();
        if (!$scope.salesOrderAdjustDoc) {

            displayPopupNotification("Please select document for indent adjustment", "warning");

        } else {

            $scope.param = {
                "Document": $scope.salesOrderAdjustDoc.FORM_CODE,
                "FromDate": fromDate,
                "ToDate": toDate,
                "IndentFilter": $scope.IndentFilter
            };

            var sOrderAdjustmentGrid = $("#sOrderAdjustmentGrid").data("kendoGrid");
            sOrderAdjustmentGrid.dataSource.transport.options.read.url = "/api/SalesOrderAdjustmentApi/GetAllSalesOrderAdjustment?document=" + $scope.salesOrderAdjustDoc.FORM_CODE + "&fromDate=" + fromDate + "&toDate=" + toDate + "&indentFilter=" + $scope.IndentFilter;
            sOrderAdjustmentGrid.dataSource.read();
        }
    };

    $scope.SetIndentDataToGrid = function (data) {
        var orderAdjustment = $("#sOrderAdjustmentGrid").data("kendoGrid");
        orderAdjustment.dataSource.transport.options.read.data = data;
        orderAdjustment.dataSource.read();
    };


    $scope.FilterSalesOrder = function (value) {
        $scope.SalesOrderFilter = value;
        console.log("Indent Filter =================" + $scope.SalesOrderFilter);
    };

    $scope.saveSalesOrderAdjustment = function () {
        debugger;

        for (var i = 0; i < $scope.checkedItemToSave.length; i++) {
            $scope.checkedItemToSave[i].CANCEL_QUANTITY = $scope.CANCEL_QUANTITY;
            $scope.checkedItemToSave[i].ADJUST_QUANTITY = $scope.ADJUST_QUANTITY;
        }

        console.log("checkItemToSave=============>> " + JSON.stringify($scope.checkedItemToSave));

        var saveResponse = SalesOrderAdjustmentService.SaveSalesOrderAdjustment($scope.checkedItemToSave);
        saveResponse.then(function (res) {
            if (res.data === "Successfull") {
                DisplayBarNotificationMessage(res.data);
                setTimeout(function () {
                    location.reload(true);
                }, 3000);
            } else {
                displayPopupNotification("Error while saving sales order adjustment ", "error");
            }
        });

    };
});

DTModule.service('SalesOrderAdjustmentService', function ($http) {

    this.findNewSalesOrderAdjustment = function () {
        var newIndent = $http({
            method: "GET",
            url: "/api/SalesOrderAdjustmentApi/GetIndentDocument?tableName=" + tableName,
            dataType: "json"
        });
        return newIndent;
    };



    this.getSalesOrderDocument = function (tableName) {

        var disNo = $http({
            method: "GET",
            url: "/api/SalesOrderAdjustmentApi/GetSalesOrderDocument?tableName=" + tableName,
            dataType: "json"
        });
        return disNo;
    };


    this.SaveSalesOrderAdjustment = function (parameter) {

        var saveRes = $http({
            method: "POST",
            url: "/api/SalesOrderAdjustmentApi/SaveSalesOrderAdjustment",
            data: JSON.stringify(parameter),
            dataType: "json"
        });
        return saveRes;

    };

});