
DTModule.controller('bankReconcilationCtrl', function ($scope, bankReconcilationService) {
    console.log("bank Reconcilation controller : ");

    $scope.FormName = "Bank Reconcilation";

    $scope.gridHeaderContent = {
        OFFICE_NAME: "",
        OFFICE_ADDRESS: "",
        OFFICE_ADDRESS1: "",
        FORM_NAME: $scope.FormName,
        FORM_CYCLE: ""

    };

    //$scope.getCurrentLoginInfo = function () {
    //    var currrentInfo = bankReconcilationService.getCurrentLoggedInfo();
    //    currrentInfo.then(function (cInfo) {
    //        $scope.gridHeaderContent = cInfo.data;
    //    });
    //};

    //$scope.getCurrentLoginInfo();

   
    $scope.bankReconcilationObj = {
        "BANK_ACCOUNT_FILTER": [],
        "GENERIC_FILTER":""
    };
    $scope.GENERIC_FILTER = [];
    $scope.GENERIC_FILTER.push({
        ACC_CODE:"1",
        ACC_EDESC:"Generic"
    }, { ACC_CODE: "2", ACC_EDESC: "Posted" });

    $scope.bankFilterControl = {
        smartButtonMaxItems: 1,
        enableSearch: true,
        smartButtonTextProvider(selectionArray) {
            if (selectionArray.length === 1) {
                return selectionArray[0].label;
            } else {
                return selectionArray.length + ' Selected';
            }
        }
    };

    //$scope.bankReconcilationObj.GENERIC_FILTER.push

    $scope.AllBankDetail = null;
    $scope.getAllBankDetails = function () {
        var allBank = bankReconcilationService.getAllActiveBank();
        allBank.then(function (bankRes) {
            $scope.AllBankDetail = bankRes.data;
        });
    };

    $scope.getAllBankDetails();

    $scope.dataToReconcile =[];
    $scope.voucherNoToReconcile =[];

    $("#bankReconcilatonGrid").kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/CustomFormApi/GetDataForReconcilation"
                    //dataType: "json"
                }
            },
            pageSize: 20,
            schema: {
                model: {
                    fields: {
                        QUANTITY: { type: "number" },
                        UNIT_PRICE: { type: "number" },
                        DUE_QTY: { type: "number" }
                    }
                },
            },
            group: {
                field: "VOUCHER_DATE", aggregates: [
                    { field: "CR_AMOUNT", aggregate: "sum" },
                ]
            },
        },
        //toolbar: kendo.template($("#toolbar-template").html()),
        width: 700,
        pageable: true,
        dataBound: function (e) {
          //  $(".checkbox").on("click", selectRow);
            var view = this.dataSource.data();
           
            var grid = e.sender;
            if (grid.dataSource.total() === 0) {
                var colCount = grid.columns.length + 1;
                $(e.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, No Data Found For Given Filter. </td></tr>');
            }

        },
        columns: [

            {
                field: "VOUCHER_DATE", title: "VR. DATE", width: 40,
                template: "#= kendo.toString(kendo.parseDate(VOUCHER_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #"
            },
            {
                field: "MITI", title: "BS Date", width: 25,
                template: "#= kendo.toString(kendo.parseDate(MITI, 'yyyy-MM-dd'), 'MM/dd/yyyy') #"
            },
            { field: "VOUCHER_NO", title: "VR. NO", width: 50 },
            { field: "CHEQUE_NO", title: "Cheque NO", width: 50 },
            { field: "PARTICULAR", title: "PARTICULAR", width: 30 },
            { field: "DR_AMOUNT", title: "DR. Amount", width: 40 },
            { field: "CR_AMOUNT", title: "CR. Amount", width: 25 },
            { field: "BALANCE_AMOUNT", title: "Balance", width: 20 },
            {
                template: "<input type='checkbox' id='isReconcile_#=VOUCHER_NO#' class='checkbox row-checkbox chkReconcile'/>",
                width: 30,
                title: "Check"
            },
            {
                field: "RECONCILE_DATE", title: "Recon. Date", width: 40,
                template: "#= kendo.toString(kendo.parseDate(RECONCILE_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #"
            },
            {
                field: "CLEARING_DATE", title: "Clearing Date", width: 40,
                template: "#= kendo.toString(kendo.parseDate(CLEARING_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #"
            }
        ]
    });

    $("#bankReconcilatonGrid").data('kendoGrid').table.on("click", ".checkbox" , selectRow);

    //on click of the checkbox:
    function selectRow() {
        var checked = this.checked,
        row = $(this).closest("tr"),
        grid = $("#bankReconcilatonGrid").data("kendoGrid"),
        dataItem = grid.dataItem(row);

       // checkedIds[dataItem.id] = checked;
        if (checked) {
            //-select the row
            row.addClass("k-state-selected");
            $scope.dataToReconcile.push(dataItem);
            $scope.voucherNoToReconcile.push(dataItem.VOUCHER_NO)
            } else {
            //-remove selection
            row.removeClass("k-state-selected");
            $scope.dataToReconcile.pop();
            $scope.voucherNoToReconcile.pop();
        }
    }

    $scope.UpdateReconsileGrid = function(){
      console.log("voucherNO To rco===>>",$scope.voucherNoToReconcile);
       if($scope.voucherNoToReconcile.length > 0){
            
            var saveResponse = bankReconcilationService.updateBankReconcilation($scope.voucherNoToReconcile);
            saveResponse.then(function (savRes) {
            if (savRes.data === "Successful") {
                 DisplayBarNotificationMessage("Reconcile done.");
                setTimeout(function () {
                    location.reload(true);
                },2000);
            }
        }); 

       }else{
             displayPopupNotification("No data to reconcile , Please select !", "error");
       }
     
    };

});
DTModule.service('bankReconcilationService', function ($http) {

    this.getAllActiveBank = function () {
        var allBank = $http({
            method: "GET",
            url: "/api/CustomFormApi/GetBankDetailForFilter",
            dataType: "json"
        });
        return allBank;
    };

    this.updateBankReconcilation = function (voucherNo) {
        var upInfo = $http({
            method: "GET",
            url: "/api/CustomFormApi/UpdateBankReconcilation?voucherNo=" + voucherNo,
            dataType: "json"
        });
        return upInfo;
    };

    this.getCurrentLoggedInfo = function () {

        var currntInfo = $http({
            method: "GET",
            url: "/api/CustomFormApi/GetLoggedInInfo",
            dataType: "json"
        });
        return currntInfo;
    };


});