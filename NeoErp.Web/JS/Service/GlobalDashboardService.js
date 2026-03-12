app.service("crudAJService", function ($http) {
    // Add Distributor
    var serv = {};
    serv.getGlobalDashboardMenu = function (obj) {
        var Module_Code = obj;
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/SalesDashboard/GetUserWiseMenuPermission?Module_Code=" + Module_Code,
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }
    serv.getInstalledPlugInData = function (obj) {
        var response = $http({
            method: "GET",
            url: window.location.protocol + "//" + window.location.host + "/api/Common/InstalledPuginListForGlobalMenu",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }

    serv.GetDistributionMneu = function () {
        var moduleCode = "10";
        var response = $http({
            method: "GET",
            url: window.location.protocol + "//" + window.location.host + "/api/SalesHome/GetDynamicMenu?ModuleCode=" + moduleCode,
            //data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }

    return serv;
});


