


DTModule.service('salesInvoiceJewelleryService', function (salesInvoiceJewelleryfactory) {
    debugger;
    this.getFormDetail_ByFormCode = function (d1) {
        var formDetail = salesInvoiceJewelleryfactory.getFormDetail()
            .then(function (result) {
                debugger;
                d1.resolve(result);
            }, function (err) { });
    };
    this.getDraftFormDetail_ByFormCode = function (formCode, d7) {
        var formDetail = salesInvoiceJewelleryfactory.getDraftFormDetail(formCode)
            .then(function (result) {
                d7.resolve(result);
            }, function (err) { });
    };
    this.getFormCustomSetup_ByFormCode = function (formCode, voucherNo, d3) {

        var formDetail = salesInvoiceJewelleryfactory.GetFormCustomSetup(formCode, voucherNo)
            .then(function (result) {
                d3.resolve(result);
            }, function (err) { });
    };
    this.getFormSetup_ByFormCode = function (formCode, d1) {
        var formSetup = salesInvoiceJewelleryfactory.getFormSetup(formCode)
            .then(function (result) {
                d1.resolve(result);
            }, function (err) { });
    };
    this.getSalesOrderDetail_ByFormCodeAndOrderNo = function (formCode, orderno, d4) {

        var salesorderformDetail = salesInvoiceJewelleryfactory.getSalesOrderFormDetail(formCode, orderno)
            .then(function (result) {
                d4.resolve(result);
            }, function (err) { });
    };
    this.getGrandTotalSalesOrder_ByFormCodeAndOrderNo_Ref = function (orderno, d9) {

        var grandtotalsalesorder = salesInvoiceJewelleryfactory.getGrandTotalSalesOrderRef(orderno)
            .then(function (result) {
                d9.resolve(result);
            }, function (err) { });
    };
    this.getGrandTotalSalesOrder_ByFormCodeAndOrderNo = function (orderno, formCode, d5) {

        var grandtotalsalesorder = salesInvoiceJewelleryfactory.getGrandTotalSalesOrder(orderno, formCode)
            .then(function (result) {
                d5.resolve(result);
            }, function (err) { });
    };
    this.getnewlygeneratedvoucherno = function (companycode, fromcode, currentdate, tablename, d6) {

        var newvoucherno = salesInvoiceJewelleryfactory.getNewOrederNumber(companycode, fromcode, currentdate, tablename)
            .then(function (result) {
                d6.resolve(result);
            }, function (err) { });
    };
    this.GetVouchersCount = function (FORM_CODE, TABLE_NAME) {
        var budgetCode = salesInvoiceJewelleryfactory.GetVoucherCount(FORM_CODE, TABLE_NAME);
        return budgetCode;
    };
    //draft
    this.getDraftData_ByFormCodeAndTempCode = function (formCode, tempcode) {
        return salesInvoiceJewelleryfactory.getDraftDataByFormCodeAndTempCode(formCode, tempcode);

    };
});

DTModule.factory('salesInvoiceJewelleryfactory', function ($http) {
    debugger;
    var fac = {};
    fac.getFormDetail = function (formcode) {
        var req = "/api/TemplateApi/GetFormDetailSalesInvoiceJewellerySetup";
        return $http.get(req);
    }
    fac.getDraftFormDetail = function (formcode) {
        var req = "/api/TemplateApi/GetDraftFormDetail?formCode=";
        return $http.get(req + formcode);
    }
    fac.GetFormCustomSetup = function (formcode, voucherNo) {
        var req = "/api/TemplateApi/GetFormCustomSetup?formCode=" + formcode + "&&voucherNo=" + voucherNo;
        return $http.get(req);
    }
    fac.getFormSetup = function (formcode) {
        var req = "/api/TemplateApi/GetFormSetupByFormCode?formCode=";
        return $http.get(req + formcode);
    }
    fac.getSalesOrderFormDetail = function (formcode, orderno) {

        var req = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetSalesOrderDetailFormDetailByFormCodeAndOrderNo?formCode=" + formcode + "&orderno=" + orderno;
        //return $http.get(req + formcode);
        return $http.get(req);
    }
    fac.getGrandTotalSalesOrder = function (orderno, formcode) {

        var req = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetGrandTotalByVoucherNo?voucherno=" + orderno + "&formcode=" + formcode;
        //return $http.get(req + formcode);
        return $http.get(req);
    }
    fac.getGrandTotalSalesOrderRef = function (orderno) {

        var req = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetREFGrandTotalByVoucherNo?voucherno=" + orderno;
        //return $http.get(req + formcode);
        return $http.get(req);
    }
    fac.getNewOrederNumber = function (companycode, formcode, currentdate, tablename) {
        formcode = "496";
        var req = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetNewOrderNo?companycode=" + companycode + "&formcode=" + formcode + "&currentdate=" + currentdate + "&tablename=" + tablename + "&isSequence=false";
        //return $http.get(req + formcode);
        return $http.get(req);
    }
    fac.GetVoucherCount = function (FORM_CODE, TABLE_NAME) {
        var req = "/api/ContraVoucherApi/GetVouchersCount?FORM_CODE=" + FORM_CODE + "&TABLE_NAME=" + TABLE_NAME;
        return $http.get(req);
    }
    fac.getDraftDataByFormCodeAndTempCode = function (formcode, tempCode) {
        var req = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getDraftDataByFormCodeAndTempCode?formCode=" + formcode + "&TempCode=" + tempCode;
        return $http.get(req);
    }

    return fac;
});