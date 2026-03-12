

DTModule.service('inventoryservice', function (inventoryfactory, $http) {

    this.DeleteInvVoucher = function (voucherno, formcode) {
        var delRes = $http({
            method: 'POST',
            url: "/api/InventoryApi/DeleteInvVoucher?voucherno=" + voucherno + "&formcode=" + formcode,
            dataType: "JSON"
        });
        return delRes;
    };
    this.getFormDetail_ByFormCode = function (formCode, d1) {
        var formDetail = inventoryfactory.getFormDetail(formCode)
            .then(function (result) {

                d1.resolve(result);
            }, function (err) { });
    };
    this.getDraftFormDetail_ByFormCode = function (formCode, d7) {
        var formDetail = inventoryfactory.getDraftFormDetail(formCode)
            .then(function (result) {
                d7.resolve(result);
            }, function (err) { });
    };
    this.getFormSetup_ByFormCode = function (formCode, d2) {
        var formSetup = inventoryfactory.getFormSetup(formCode)
            .then(function (result) {
                d2.resolve(result);
            }, function (err) { });
    };
    this.getSalesOrderDetail_ByFormCodeAndOrderNo = function (formCode, orderno, d4) {

        var salesorderformDetail = inventoryfactory.getSalesOrderFormDetail(formCode, orderno)
            .then(function (result) {
                d4.resolve(result);
            }, function (err) { });
    };

    //draft
    this.getDraftData_ByFormCodeAndTempCode = function (formCode, tempcode) {
        return inventoryfactory.getDraftDataByFormCodeAndTempCode(formCode, tempcode);

    };

    this.getnewlygeneratedvoucherno = function (companycode, fromcode, currentdate, tablename, d6) {

        var newvoucherno = inventoryfactory.getNewOrederNumber(companycode, fromcode, currentdate, tablename)
            .then(function (result) {
                d6.resolve(result);
            }, function (err) { });
    };
    this.getnewlygeneratedvouchernoafterinsert = function () {

        return Cinventoryfactory.getNewOrederNumber(companycode, fromcode, currentdate, tablename);
    }
    this.getBudgetCodeByAccCode = function (accCode) {
        var budgetCode = inventoryfactory.getBudgetCodeByAccCode(accCode);
    }
    this.GetVouchersCount = function (FORM_CODE, TABLE_NAME) {
        var budgetCode = inventoryfactory.GetVoucherCount(FORM_CODE, TABLE_NAME);
        return budgetCode;
    }

    this.GetReferenceList = function () {
        return inventoryfactory.getReferenceList();
    }

    this.getSubledgerByAccCode = function (accCode, d5) {

        var newvoucherno = inventoryfactory.getSubledgerByAccCode(accCode, d5)
            .then(function (result) {
                d5.resolve(result);
            }, function (err) { });
    };
    this.getFormCustomSetup_ByFormCode = function (formCode, d3) {

        var formDetail = inventoryfactory.GetFormCustomSetup(formCode)
            .then(function (result) {
                d3.resolve(result);
            }, function (err) { });
    };

});

DTModule.factory('inventoryfactory', function ($http) {
    var fac = {};
    fac.getFormDetail = function (formcode) {
        var req = "/api/TemplateApi/GetFormDetailSetup?formCode=";
        return $http.get(req + formcode);
    }
    fac.getDraftFormDetail = function (formcode) {
        var req = "/api/TemplateApi/GetDraftFormDetail?formCode=";
        return $http.get(req + formcode);
    }
    fac.getSubledgerByAccCode = function (accCode) {
        var req = "/api/TemplateApi/getSubledgerCodeByAccCode?accCode=";
        return $http.get(req + accCode);
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
    fac.getNewOrederNumber = function (companycode, formcode, currentdate, tablename) {
        var req = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetNewOrderNo?companycode=" + companycode + "&formcode=" + formcode + "&currentdate=" + currentdate + "&tablename=" + tablename;
        //return $http.get(req + formcode);
        return $http.get(req);
    }

    fac.getBudgetCodeByAccCode = function (accCode) {
        var req = "/api/TemplateApi/getBudgetCodeByAccCode?accCode=";
        return $http.get(req + accCode);
    }

    fac.getReferenceList = function () {
        var req = "/api/InventoryApi/GetReferenceList";
        return $http.get(req);

    };
    fac.GetVoucherCount = function (FORM_CODE, TABLE_NAME) {
        var req = "/api/InventoryApi/GetVouchersCount?FORM_CODE=" + FORM_CODE + "&TABLE_NAME=" + TABLE_NAME;
        return $http.get(req);
    }


    fac.GetFormCustomSetup = function (formcode) {
        var req = "/api/TemplateApi/GetFormCustomSetup?formCode=";
        return $http.get(req + formcode);
    }

    fac.getDraftDataByFormCodeAndTempCode = function (formCode, tempcode) {

        var req = "/api/TemplateApi/getDraftDataByFormCodeAndTempCode?formCode=" + formCode + "&tempCode=" + tempcode;
        return $http.get(req);
    }
    return fac;
});