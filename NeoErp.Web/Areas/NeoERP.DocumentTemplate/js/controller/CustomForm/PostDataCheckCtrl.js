DTModule.controller('postDataCheckCtrl', function ($scope,postDataChequeService) {
    $scope.FormName = "Post Data Check";

    $scope.PDCFormObject = {
        "RECEIPT_NO": "",
        "RECEIPT_DATE": "",
        "CHEQUE_DATE": "",
        "BOUNCE_DATE": "",
        "CUSTOMER_CODE_DDL": "",
        "CUSTOMER_CODE":"",
        "PARTY_TYPE_DDL": "",
        "PARTY_TYPE":"",
        "PDC_AMOUNT": "",
        "PDC_DETAIL": "",
        "PARTY_BANK_NAME": "",
        "CHEQUE_NO": "",
        "REMARK":"",
        "REMINDER_PRIOR_DAYS": "",
        "MONEY_RECEIPT_ISSUED_BY": "",
        "MONEY_RECEIPT_NO": "",
        "CREATED_BY": "",
        "BOUNCE_BY": "",
        "IN_TRANSIT_BY": "",
        "CREATED_DATE": "",
        "IN_TRANSIT_DATE": "",
        "CHEQUE_IN_HAND": false,
        "CHEQUE_IN_TRANSIT": false,
        "DIRECT_BOUNCE": false,
        "CHECK_RETURN": false,
        "CHECK_RETURN_DATE": "",
        "SELECTED_ACCOUNT": "",
        "ACCOUNT_CONFIRM":false
    };

    $scope.UpdateEncashObj = {
        "ENCASH_DATE": "",
        "DAYS": "",
        "ENCASH_REMARK":""
    };



    var dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                method: "GET",
                url: "/api/CustomFormApi/GetAllPDCFormDetail",
                dataType: "json"
            },
            parameterMap: function (options, type) {
                if (type === 'read') {
                    return {
                       
                    };
                }
            }
        },
        pageSize: 10
    });
    var checkedItems = [];

    $("#postDataCheckGrid").kendoGrid({
        dataSource: dataSource,
        scrollable: true,
       // filterable: true,
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
            { field: "RECEIPT_NO", title: "Receipt No.", width: 70 },
            { field: "RECEIPT_DATE", title: "Receipt Date", width: 70 },
            { field: "CHEQUE_NO", title: "Cheque No", width: 120 },
            { field: "CHEQUE_DATE", title: "Cheque Date", width: 120 },
            { field: "ENCASH_DATE", title: "Encash Date", width: 60 },
            { field: "CUSTOMER", title: "Customer", width: 60 },
            { field: "DEALER", title: "Dealer", width: 80 },
            { field: "PDC_AMOUNT", title: "PDC Amount", width: 80 },
            { field: "PDC_DETAILS", title: "PDC Detail", width: 80 },
            { field: "BANK_NAME", title: "Bank Name", width: 80 },
            { field: "REMARKS", title: "Remarks", width: 80 }
        ]
    });

    function selectRow() {

    }


    $scope.createNewPDCForm = function () {
        $("#postDataCheckModal").toggle();
    };

    $scope.closeNewPDCFormDialog = function () {
        $("#postDataCheckModal").toggle();
    };

    $scope.getAllCustomer = function () {
        var allCust = postDataChequeService.getAllCustomer();
        allCust.then(function (custRes) {
            $scope.AllCustomer = custRes.data;
        });
    };
    $scope.getAllCustomer();

    $scope.getAllPartyType = function () {
        var allPartyType = postDataChequeService.getAllPartyType();
        allPartyType.then(function (partyRes) {
            $scope.AllPartyType = partyRes.data;
        });
    };
    $scope.getAllPartyType();


    $scope.getAllChartOfAccount = function () {
        var allAcccount = postDataChequeService.getAllChartOfAccount();
        allAcccount.then(function (accountRes) {
            $scope.AllChartOfAccount = accountRes.data;
        });
    };
    $scope.getAllChartOfAccount();

    $scope.saveNewPDCForm = function () {
        var saveResponse = postDataChequeService.saveNewPDCForm();
        saveResponse.then(function (savRes) {
            if (savRes.data === "Successfull") {
                DisplayBarNotificationMessage("PDC entry saved successfully");
                setTimeout(function () {
                    location.reload(true);
                },3000);
            }
        });
    };


    $scope.searchPDFToView = function () {
        var detailToEdit = $("#columnSettingsGrid").data("kendoGrid");
        var modal = $scope.PDCFormObject;
        console.log("Search Modal=======================>>> " + JSON.stringify(modal));
        var searchResult = postDataChequeService.searchPDCDetail(modal);
        searchResult.then(function (sRes) {
            //detailToEdit.dataSource = new kendo.data.DataSource({
            //    data: sRes.data
            //});
            dataSource.read({ data: sRes.data });
        });

        
        //detailToEdit.dataSource.transport.options.read.url = window.location.protocol + "//" + window.location.host + "/api/CustomFormApi/GetAllFormDetailToEdit?formCode=" + $scope.columnSettingsObj.FORM_NAME.FORM_CODE + "&tableName=" + $scope.columnSettingsObj.TABLE_NAME.TABLE_NAME;
        //detailToEdit.dataSource.read();
    };
});
DTModule.service('postDataChequeService', function ($http) {
    this.getAllCustomer = function () {
        var allCust = $http({
            method: "GET",
            url: "/api/SubLedgerMgmtApi/GetCustomerSubLedger",
            dataType: "json"
        });
        return allCust;
    };

    this.getAllPartyType = function () {
        var allPT = $http({
            method: "GET",
            url: "/api/CustomFormApi/GetAllPartyType",
            dataType: "JSON"
        });
        return allPT;
    };

    this.getAllChartOfAccount = function () {
        var allCOA = $http({
            method: "GET",
            url: "/api/SubLedgerMgmtApi/GetCharOfAccountTree",
            dataType: "JSON"
        });
        return allCOA;
    };

    this.saveNewPDCForm = function (saveParam) {
        var saveRes = $http({
            method: "POST",
            url: "/api/CustomFormApi/SaveNewPDCForm",
            data: JSON.stringify(saveParam),
            dataType: "JSON"
        });
        return saveRes;
    };

    this.searchPDCDetail = function (searchParam) {
        var searchResult = $http({
            method: "POST",
            url: "/api/CustomFormApi/SearchPDCDetail",
            data: JSON.stringify(searchParam),
            dataType: "JSON"

        });
        return searchResult;
    };
});