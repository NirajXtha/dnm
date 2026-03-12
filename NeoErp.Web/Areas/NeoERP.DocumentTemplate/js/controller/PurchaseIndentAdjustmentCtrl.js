DTModule.controller('PurchaseIndentAdjustmentCtrl', function ($scope, PurchaseIndentAdjustmentService, $filter) {

    $scope.TableName = "IP_PURCHASE_REQUEST";

    $scope.FormName = "Purchase Indent Adjustment";
    $scope.IndentAdjustFrom = $filter('date')(new Date(), 'dd-MMM-yyyy');
    $scope.IndentAdjustNepFrom = ""; 
    $scope.IndentAdjustTo = $filter('date')(new Date(), 'dd-MMM-yyyy');
    $scope.IndentAdjustNepTo = "";

    $scope.selectedIndentDoc = null;

    $scope.IndentFilter = "CAID";

    $scope.param = {

        "Document": "",
        "FromDate": "",
        "ToDate": "",
        "IndentFilter": ""
    };

    $scope.ConvertEngToNep = function () {
        console.log(this);

        var engdate = $("#englishDate5").val();
        var nepalidate = ConvertEngDateToNep(engdate);
        $("#nepaliDate5").val(nepalidate);
        $("#nepaliDate51").val(nepalidate);
    };

    $scope.ConvertNepToEng = function ($event) {

        //$event.stopPropagation();
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
        //var engdate1 = $filter('date')(new Date(new Date().setDate(new Date().getDate() - 2)), 'dd-MMM-yyyy');
        var a = ConvertEngDateToNep(engdate);
        //var a1 = ConvertEngDateToNep(engdate1);
        $scope.IndentAdjustFrom = engdate;
        $scope.IndentAdjustNepFrom = a;
        $scope.IndentAdjustNepTo = a;
        $scope.IndentAdjustTo = ConvertEngDateToNep($filter('date')(new Date(new Date().setDate(new Date().getDate())), 'dd-MMM-yyyy'));
        //  $scope.PlanningDate = a;

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
        var indentDocument = PurchaseIndentAdjustmentService.getIndentDocument($scope.TableName);
        indentDocument.then(function (res) {
            $scope.indentDocument = res.data;
        });
    };

    $scope.GetIndentDocument();

    var dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                method:"POST",
                url: "/api/PurchaseOrderIndentAdjustmentApi/GetAllPurchaseIndentAdjustment",
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

    var checkedItems = [];
    var checkedIds = {};
    $scope.ADJUST_QUANTITY = 0;
    $scope.CANCEL_QUANTITY = 0;

    function selectRow() {

        var checked = this.checked,
            row = $(this).closest("tr"),
            grid = $("#pIndentAdjustmentGrid").data("kendoGrid"),
            dataItem = grid.dataItem(row);
        if (checked) {
            row.addClass("k-state-selected");
            $(this).attr('checked', true);
            checkedIds[dataItem.ORDER_NO] = checked;
            $("#txtCancelQuantity_" + dataItem.ROW_NO).show();
            $("#txtAdjustQuantity_" + dataItem.ROW_NO).show();

            checkedItems.push({
                "REQUEST_NO": dataItem.REQUEST_NO,
                "REQUEST_DATE": dataItem.REQUEST_DATE,
                "FROM_LOCATION_CODE": dataItem.FROM_LOCATION_CODE,
                "ITEM_CODE": dataItem.ITEM_CODE,
                "UNIT": dataItem.UNIT,
                "QUANTITY": dataItem.QUANTITY,
                "CANCEL_QUANTITY":  document.getElementById('txtCancelQuantity_' + dataItem.ROW_NO).value,  
                "ADJUST_QUANTITY":  document.getElementById('txtAdjustQuantity_' + dataItem.ROW_NO).value,
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
                if (checkedItems[i].REQUEST_NO == dataItem.REQUEST_NO && checkedItems[i].FROM_LOCATION_CODE == dataItem.FROM_LOCATION_CODE) {
                    checkedItems.splice(i, 1);
                }
            }
        }
    }

    $scope.findNewIndentAdjustment = function () {

        console.log("scope.selected Doc=====================>>>" + $scope.selectedIndentDoc);
        var fromDate = $('#FromDateVoucher').val();
        var toDate = $('#ToDateVoucher').val();
        if (!$scope.selectedIndentDoc) {

            displayPopupNotification("Please select document for indent adjustment", "warning");

        } else {

            $scope.param = {
                "Document": $scope.selectedIndentDoc.FORM_CODE,
                "FromDate": fromDate,
                "ToDate": toDate,
                "IndentFilter": $scope.IndentFilter
            };

            var pIndentAdjustmentGrid= $("#pIndentAdjustmentGrid").data("kendoGrid");
            pIndentAdjustmentGrid.dataSource.transport.options.read.url = "/api/PurchaseOrderIndentAdjustmentApi/GetAllPurchaseIndentAdjustment?document=" + $scope.selectedIndentDoc.FORM_CODE + "&fromDate=" + fromDate + "&toDate=" + toDate + "&indentFilter=" + $scope.IndentFilter;
            pIndentAdjustmentGrid.dataSource.read();
        }
    };

    $scope.SetIndentDataToGrid = function (data) {
        var indentAdjustment = $("#pIndentAdjustmentGrid").data("kendoGrid");
        indentAdjustment.dataSource.transport.options.read.data = data;
        indentAdjustment.dataSource.read();
    };

    $("#pIndentAdjustmentGrid").kendoGrid({
            dataSource:dataSource,
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
                        if (checkedItems[j].VOUCHER_NO === view[i].VOUCHER_NO && checkedItems[j].SERIAL_NO === view[i].SERIAL_NO) {
                            this.tbody.find("tr[data-uid='" + view[i].uid + "']")
                                .addClass("k-state-selected")
                                .find(".checkbox")
                                .attr("checked", "checked");
                        }
                    }
                }
                var grid = e.sender;
                if (grid.dataSource.total() === 0) {
                    var colCount = grid.columns.length + 1;
                    $(e.sender.wrapper)
                        .find('tbody')
                        .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, No Data Found For Given Filter. </td></tr>');
                }

            },
            columns: [
                { field: "REQUEST_NO", title: "Indent No.", width: 70 },
                { field: "REQUEST_DATE", title: "Indent Date", width: 70 },
                { field: "FROM_LOCATION", title: "From Department", width: 120 },
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
                { field: "CREATED_DATE", title: "Amendment Date", width: 80}
            ]
        });
   
    $scope.FilterIndent = function (value) {
        console.log("val=============================>>>" + value);
        $scope.IndentFilter = value;
        console.log("Indent Filter =================" + $scope.IndentFilter);
    };


    $scope.saveIndenAdjustment = function () {

        for (var i = 0; i < $scope.checkedItemToSave.length; i++) {
            $scope.checkedItemToSave[i].CANCEL_QUANTITY = $scope.CANCEL_QUANTITY;
            $scope.checkedItemToSave[i].ADJUST_QUANTITY = $scope.ADJUST_QUANTITY;
        }
      
        var saveResponse = PurchaseIndentAdjustmentService.SaveIndentAdjustment($scope.checkedItemToSave);
        saveResponse.then(function (res) {
            if (res.data === "Successfull") {
                DisplayBarNotificationMessage(res.data);
                setTimeout(function () {
                    location.reload(true);
                }, 3000);
            } else {
                displayPopupNotification("Error while saving indent adjustment ", "error");
            }
        });

    };
});

DTModule.service('PurchaseIndentAdjustmentService', function ($http) {

    this.findNewIndentAdjustment = function () {
        var newIndent = $http({
            method: "GET",
            url: "/api/PurchaseOrderIndentAdjustmentApi/GetIndentDocument?tableName=" + tableName,
            dataType: "json",
        });
        return newIndent;
    };


    this.findAllIndentAdjustment = function (params) {
        var indentAdData = $http({
            method: "POST",
            url: "/api/PurchaseOrderIndentAdjustmentApi/GetAllPurchaseIndentAdjustment",
            data: JSON.stringify(params),
            dataType: "json",
        });

        return indentAdData;
    };

    this.getIndentDocument = function (tableName) {

        var disNo = $http({
            method: "GET",
            url: "/api/PurchaseOrderIndentAdjustmentApi/GetIndentAdjustmentDoc?tableName=" +tableName,
            dataType: "json",
        });
        return disNo;
    }

    this.SaveIndentAdjustment = function (parameter) {

        var saveRes = $http({
            method: "POST",
            url: "/api/PurchaseOrderIndentAdjustmentApi/SaveIndentAdjustment",
            data: JSON.stringify(parameter),
            dataType: "json",

        });
        return saveRes;
    };

});