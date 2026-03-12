distributionModule.factory('distributorService', function ($http) {
    var serv = {};
    var self = this;
    self.addedDate = '';
    // Add Distributor
    serv.AddDistributor = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/AddDistributor",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }


    //UPDATE DISTRIBUTOR ORDER
    serv.UpdateOrder = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/UpdateDistributorOrder",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }
    serv.deleteDistributor = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/deleteDistributor",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }


    // Update Distributor
    serv.UpdateDistributor = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/UpdateDistributor",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }


    serv.getDynamicMenu = function (moduleCode) {
        var response = $http({
            method: 'GET',
            url: window.location.protocol + "//" + window.location.host + "/api/SalesHome/GetDynamicMenu?ModuleCode=" + moduleCode,
        });
        return response;
    }
    serv.GetClosingStock = function (DistId) {
        var response = $http({
            method: 'GET',
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetClosingStock?DistId=" + DistId,
        });
        return response;
    }

    serv.getFavroiteMenu = function () {
        var response = $http({
            method: 'GET',
            url: window.location.protocol + "//" + window.location.host + "/Home/GetFavroiteMenus?moduleCode=10"
        });
        return response;
    }
    serv.GetPerformanceData = function (dateFilter) {
        var response = $http({
            method: 'POST',
            data: dateFilter,
            url: window.location.protocol + "//" + window.location.host + "/api/Report/GetPerformanceReport",
        });
        return response;
    }
    serv.getSumOutletReport = function (dateFilter) {
        var response = $http({
            method: 'POST',
            data: dateFilter,
            url: window.location.protocol + "//" + window.location.host + "/api/Report/GetSumOutletReport"
        });
        return response;
    }
    serv.GetOutletSummaryReport = function (dateFilter) {
        var response = $http({
            method: 'POST',
            data: dateFilter,
            url: window.location.protocol + "//" + window.location.host + "/api/Report/GetOutletSummaryReport"
        });
        return response;
    }
    serv.getTopEffectiveCalls = function (dateFilter) {
        var percentEffectiveCalls = ">74";
        var response = $http({
            method: 'POST',
            data: dateFilter,
            url: window.location.protocol + "//" + window.location.host + "/api/Report/GetTopEffectiveCallsReport?percentEffectiveCalls=" + percentEffectiveCalls + ""
        });
        return response;
    }
    serv.lessTopEffectiveCalls = function (dateFilter) {
        var percentEffectiveCalls = "<75";
        var response = $http({
            method: 'POST',
            data: dateFilter,
            url: window.location.protocol + "//" + window.location.host + "/api/Report/GetTopEffectiveCallsReport?percentEffectiveCalls=" + percentEffectiveCalls + ""
        });
        return response;
    }


    serv.getPerformanceReport = function (data) {
        var response = $http({
            method: 'POST',
            data: data,
            url: window.location.protocol + "//" + window.location.host + "/api/Report/GetALLPerformanceReport",
        });
        return response;
    }

    serv.getPresentASMBeat = function (data) {
        debugger;
        var response = $http({
            method: 'POST',
            data: data,
            url: window.location.protocol + "//" + window.location.host + "/api/Report/GetPresentASMBeat",
        });
        return response;
    }

    serv.getASMBeat = function (data) {
        debugger;
        var response = $http({
            method: 'POST',
            data: data,
            url: window.location.protocol + "//" + window.location.host + "/api/Report/GetASMBeat",
        });
        return response;
    }

    

    //serv.GetDynamicMenu= function () {
    //    //// This could be $http or any other promise returning service.
    //    //// Use a deferred and $timeout to simulate a network request.
    //    var deferred = $q.defer()
    //    $timeout(function () {
    //        $http.get('/api/SalesHome/GetDynamicMenu?ModuleCode=' + moduleCode)
    //            .success(function (result) {
    //                defer.resolve(result);
    //            });
    //    }, 2000);
    //    return deferred.promise;
    //}

    serv.getData= function () {
        //var deferred = $q.defer();
        var results = $http.get('/api/SalesHome/GetDynamicMenu?ModuleCode=' + moduleCode)
            .success(function (data) {
                return data.data;
            });
        //deferred.resolve(results);
        return results;
    }


    //serv.getFavroiteMenu= function () {
    //    var deferred = $q.defer()
    //    var results = $http.get(window.location.protocol + "//" + window.location.host + "/Home/GetFavroiteMenus?moduleCode=10")
    //        .success(function (data) {
    //            return data.data;
    //        });
    //    deferred.resolve(results);
    //    return deferred.promise;
    //}



    return serv;
});

