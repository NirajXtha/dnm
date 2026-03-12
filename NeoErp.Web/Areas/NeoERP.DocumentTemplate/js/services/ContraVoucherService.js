


DTModule.service('contravoucherservice', function (contravoucherfactory, $http) {

    this.deleteFinanceVoucher = function (voucherno,formcode) {
        var delRes = $http({
            method: 'POST',
            url: "/api/ContraVoucherApi/DeleteFinanceVoucher?voucherno=" + voucherno + "&formcode=" + formcode,
            dataType: "JSON"
        });
        return delRes;
    };

    this.getFormDetail_ByFormCode = function (formCode, d1) {
        var formDetail = contravoucherfactory.getFormDetail(formCode)
            .then(function (result) {
                d1.resolve(result);
            }, function (err) { });
    };
    this.getDraftFormDetail_ByFormCode = function (formCode, d7) {
        var formDetail = contravoucherfactory.getDraftFormDetail(formCode)
            .then(function (result) {
                d7.resolve(result);
            }, function (err) { });
    };

    //draft
    this.getDraftData_ByFormCodeAndTempCode = function (formCode, tempcode) {
        return contravoucherfactory.getDraftDataByFormCodeAndTempCode(formCode, tempcode);

    };
    this.getFormSetup_ByFormCode = function (formCode, d2) {
        var formSetup = contravoucherfactory.getFormSetup(formCode)
            .then(function (result) {
                d2.resolve(result);
            }, function (err) { });
    };
    this.getnewlygeneratedvoucherno = function (companycode, fromcode, currentdate, tablename, d6) {

        var newvoucherno = contravoucherfactory.getNewOrederNumber(companycode, fromcode, currentdate, tablename)
            .then(function (result) {
                d6.resolve(result);
            }, function (err) { });
    };
     this.getSalesOrderDetail_ByFormCodeAndOrderNo = function (formCode,orderno, d4) {
      
        var salesorderformDetail = contravoucherfactory.getSalesOrderFormDetail(formCode, orderno)
            .then(function (result) {
                d4.resolve(result);
            }, function (err) { });
     };
  
    this.getnewlygeneratedvouchernoafterinsert = function () {
        
        return Ccontravoucherfactory.getNewOrederNumber(companycode, fromcode, currentdate, tablename);
    }
    this.getBudgetCodeByAccCode = function(accCode) {
        var budgetCode = contravoucherfactory.getBudgetCodeByAccCode(accCode);
    }
    this.GetVouchersCount = function (FORM_CODE,TABLE_NAME) {
        var budgetCode = contravoucherfactory.GetVoucherCount(FORM_CODE, TABLE_NAME);
        return budgetCode;
    }

    this.GetReferenceList = function () {
        return contravoucherfactory.getReferenceList();
    }

     this.getSubledgerByAccCode = function (accCode, d5) {
        
        var newvoucherno = contravoucherfactory.getSubledgerByAccCode( accCode, d5)
            .then(function (result) {
                d5.resolve(result);
            }, function (err) { });
     };
    this.getFormCustomSetup_ByFormCode = function (formCode, d3) {
        
        var formDetail = contravoucherfactory.GetFormCustomSetup(formCode)
            .then(function (result) {
                d3.resolve(result);
            }, function (err) { });
    };

});

DTModule.factory('contravoucherfactory', function ($http) {
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
    fac.getNewOrederNumber = function (companycode, formcode, currentdate, tablename) {
        var req = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetNewOrderNo?companycode=" + companycode + "&formcode=" + formcode + "&currentdate=" + currentdate + "&tablename=" + tablename;
        //return $http.get(req + formcode);
        return $http.get(req);
    }
   fac.getSalesOrderFormDetail = function (formcode, orderno) {
        
        var req = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetSalesOrderDetailFormDetailByFormCodeAndOrderNo?formCode=" + formcode + "&orderno=" + orderno;
        //return $http.get(req + formcode);
        return $http.get(req);
   }
   fac.getDraftDataByFormCodeAndTempCode = function (formcode, tempCode) {
       var req = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getDraftDataByFormCodeAndTempCode?formCode=" + formcode + "&TempCode=" + tempCode;
       //return $http.get(req + formcode);
       return $http.get(req);
   }
  
    fac.getBudgetCodeByAccCode= function(accCode) {
        var req = "/api/TemplateApi/getBudgetCodeByAccCode?accCode=";
        return $http.get(req + accCode);
    }

    fac.getReferenceList = function () {
        var req = "/api/ContraVoucherApi/GetReferenceList";
       return $http.get(req);
       
    };
    fac.GetVoucherCount = function (FORM_CODE,TABLE_NAME) {
        var req = "/api/ContraVoucherApi/GetVouchersCount?FORM_CODE=" + FORM_CODE + "&TABLE_NAME=" + TABLE_NAME;
        return $http.get(req);
    }
        
    
    fac.GetFormCustomSetup = function (formcode) {
        var req = "/api/TemplateApi/GetFormCustomSetup?formCode=";
        return $http.get(req + formcode);
    }

    
    return fac;
});