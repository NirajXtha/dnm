// write all api get/post/put/delete code here
transactionModule.factory('BankLimitSetupservice', function ($http) {
    var serv = {};
    serv.SaveBankLimit = function (BankObject, Loan) {
        var url = "/api/Bank/BankLimitSetup";
        BankObject.LoanList = Loan;
        debugger;
        return $http.post(url, BankObject);
    }
    serv.FundedList = function () {
        var url = "/api/Bank/GetAllFunded";
        return $http.get(url);
    }
    serv.NonFundedList = function () {
        var url = "/api/Bank/GetAllNonFunded";
        return $http.get(url);
    }
    serv.GetBankList = function () {
        var url = "/api/Bank/GetAllBanks";
        return $http.get(url);
    }
    serv.GetLimit = function (transNo,type) {
        var url = "/api/Bank/GetFundedByTranNo?transNo=" + transNo + "&type=" + type;
        return $http.get(url);
    }
    serv.AllCategories = function () {
        var url = "/api/Bank/GetAllCategories";
        return $http.get(url);
    }
    return serv;
});

transactionModule.factory('KendoCtrlservice', function ($http) {
    var serv = {};
    serv.UpdateLimit = function (Id, ExpiryDate) {
        var url = "/api/Bank/UpdateLimit?id=" + Id + "&date=" + ExpiryDate;
        return $http.get(url);
    }
    return serv;
});

transactionModule.factory('CategoryCtrlservice', function ($http) {
    var serv = {};
    serv.Save = function (obj) {
        var url = "/api/Bank/CategorySetup";
        return $http.post(url, obj);
    }
    serv.AllCategories = function () {
        var url = "/api/Bank/GetAllCategories";
        return $http.get(url);
    }
    serv.GetCategory = function (id) {
        var url = "/api/Bank/GetLoanCategory?Id=" + id;
        return $http.get(url);
    }
    serv.Delete = function (id) {
        var url = "/api/Bank/DeleteLoanCategory?Id=" + id;
        return $http.get(url);
    }
    return serv;
});