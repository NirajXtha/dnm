distributionModule.service("crudAJService", function ($http) {
    var serv = {};
    var requestStatus = "Active"
    serv.getLoginPurchaseOrder = function () {
        var response = $http({
            method: 'POST',
            url: window.location.protocol + "//" + window.location.host + "/api/Distributor/GetDistributorMaxOrderNo ",
        });
        return response;
    }
    serv.savePurchaseOrder = function (data) {
        var result = $http({
            method: 'POST',
            data: data,
            url: window.location.protocol + "//" + window.location.host + "/api/Distributor/SavePurchaseOrder",
        });
        return result;
    };
    serv.SaveCollection = function (data) {
        var result = $http({
            method: 'POST',
            data: data,
            url: window.location.protocol + "//" + window.location.host + "/api/Distributor/SaveDistCOllection",
        });
        return result;
    };
    serv.GetCollectionDetail = function (billNo) {
        var result = $http({
            method: "GET",
            url: window.location.protocol + "//" + window.location.host + "/api/Distributor/GetCollectionDetail?billNo=" + billNo,
        });
        return result;
    };

    return serv;
});

