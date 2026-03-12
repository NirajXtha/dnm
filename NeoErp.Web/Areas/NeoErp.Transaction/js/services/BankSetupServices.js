// write all api get/post/put/delete code here
transactionModule.factory('BankSetupservice', function ($http) {
    var serv = {};
    serv.SaveBankSetup = function (BankObject) {
        var url = "/api/Bank/BankSetup";
        return $http.post(url,BankObject);
    }
    serv.GetBankList = function () {
        var url = "/api/Bank/GetAllBanks";
        return $http.get(url);
    }
    serv.GetBank = function (bankCode) {
        var url = "/api/Bank/GetBankByBankCode?bankCode=" + bankCode;
        return $http.get(url);
    }
    serv.DeleteBank = function (bankCode) {
        var url = "/api/Bank/DeleteBank?bankCode=" + bankCode;
        return $http.get(url);
    }
    return serv;
});