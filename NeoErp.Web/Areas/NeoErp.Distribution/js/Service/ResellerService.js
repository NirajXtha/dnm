distributionModule.factory('ResellerService', function ($http) {
    var serv = {};   

    // Add Reseller
    serv.AddReseller = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/AddReseller",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }

    serv.AddOtherEntity = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/AddOtherEntity",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }

    // Update Reseller
    serv.UpdateReseller = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/UpdateReseller",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }
    serv.UpdateEntity = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/UpdateEntity",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }

    
    serv.deleteReseller = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/deleteReseller",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }
    serv.DeleteOtherEntity = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/DeleteOtherEntity",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }

    serv.AddArea = function (data) {
        var response = $http({
            method: "POST",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/AddArea",
            data: JSON.stringify(data),
            dataType:"json"
        })
        return response;

    }

    serv.updateArea = function (data) {
        var response = $http({
            method: "POST",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/UpdateArea",
            data: JSON.stringify(data),
            dataType:"json"
        })
        return response;
    }
    serv.GetReseller = function (id) {
        var response = $http({
            method: "GET",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetReseller?ResellerId=" + id,
            dataType: "json",
        })
        return response;
    }
    serv.GetIndividualEntity = function (Code) {
        var response = $http({
            method: "GET",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetIndividualEntity?Code=" + Code,
            dataType: "json",
        })
        return response;
    }

    serv.GetPreference = function () {
        var response = $http({
            method: "GET",
            url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetDistPreferences",
            dataType: "json",
        })
        return response;
    }

    serv.deleteArea = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/deleteArea",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }

    return serv;
});

