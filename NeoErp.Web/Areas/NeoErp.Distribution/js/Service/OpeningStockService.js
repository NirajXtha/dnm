distributionModule.factory('OpeningStockService', function ($http) {
    var serv = {};

    //added for opening stock setup
    serv.SaveOpeningStock = function (list, DistCode, StockId) {
        var result = $http({
            method: 'post',
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/SaveOpeningStock?DistributerCode=" + DistCode,
            data: {
                OpeningStockId: StockId,
                DistributerCode: DistCode,
                OpeningList: list
            },
            dataType: "json"
        });
        return result;
    }

    return serv;
});