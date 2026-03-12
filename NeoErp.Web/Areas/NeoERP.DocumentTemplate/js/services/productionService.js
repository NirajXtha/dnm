DTModule.service('productionservice', function (productionfactory) {

    this.getFormDetail_ByFormCode = function (formCode, d1) {
        var formDetail = productionfactory.getFormDetail(formCode)
            .then(function (result) {
                d1.resolve(result);
            }, function (err) { });
    };
    this.getFormSetup_ByFormCode = function (formCode, d2) {
        var formSetup = productionfactory.getFormSetup(formCode)
            .then(function (result) {
                d2.resolve(result);
            }, function (err) { });
    };
    this.getDraftFormDetail_ByFormCode = function (formCode, d7) {
        var formDetail = productionfactory.getDraftFormDetail(formCode)
            .then(function (result) {
                d7.resolve(result);
            }, function (err) { });
    };
    this.getSalesOrderDetail_ByFormCodeAndOrderNo = function (formCode, orderno, d4) {

        var salesorderformDetail = productionfactory.getSalesOrderFormDetail(formCode, orderno)
            .then(function (result) {
                d4.resolve(result);
            }, function (err) { });
    };
    this.getnewlygeneratedvoucherno = function (companycode, fromcode, currentdate, tablename, d6) {

        var newvoucherno = productionfactory.getNewOrederNumber(companycode, fromcode, currentdate, tablename)
            .then(function (result) {
                d6.resolve(result);
            }, function (err) { });
    };
});

DTModule.factory('productionfactory', function ($http) {
    var fac = {};
    fac.getFormDetail = function (formcode) {
        var req = "/api/TemplateApi/GetFormDetailSetup?formCode=";
        return $http.get(req + formcode);
    }
    fac.getFormSetup = function (formcode) {
        var req = "/api/TemplateApi/GetFormSetupByFormCode?formCode=";
        return $http.get(req + formcode);
    }
    fac.getDraftFormDetail = function (formcode) {
        var req = "/api/TemplateApi/GetDraftFormDetail?formCode=";
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
    return fac;
});
