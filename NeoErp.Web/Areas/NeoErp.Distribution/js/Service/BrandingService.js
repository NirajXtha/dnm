distributionModule.factory('BrandingService', function ($http) {
    var serv = {};
    var self = this;
    self.addedDate = '';
    // Add Distributor
    serv.saveBrandingActivity = function (data) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Branding/saveBrandingActivity",
            data: JSON.stringify(data),
            dataType: "json"
        });
        return response;
    }

    serv.GetAllActivity = function () {
        var response = $http({
            method: "GET",
            url: window.location.protocol + "//" + window.location.host + "/api/Branding/getBrandingActivityList",
        });
        return response;
    }

    serv.updateBrandingActivity = function (data) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Branding/UpdateBrandingActivity",
            data: JSON.stringify(data),
            dataType: "json"
        });
        return response;
    }

    serv.deleteActivity = function (data) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Branding/deleteActivity",
            data: JSON.stringify(data),
            dataType: "json"
        });
        return response;
    }

    serv.saveContract = function (obj) {
        var response = $http({
            method: "Post",
            url: window.location.protocol + "//" + window.location.host + "/api/Branding/saveContract",
            data: JSON.stringify(obj),
            dataType: "json",
        })
        return response;
    };

    serv.deleteContract = function (delOnject) {
        var response = $http({
            method: "Post",
            url: window.location.protocol + "//" + window.location.host + "/api/Branding/deleteContract",
            data: JSON.stringify(delOnject),
            dataType: "json",
        })
        return response;
    };

    serv.updateContract = function (obj) {
        var response = $http({
            method: "Post",
            url: window.location.protocol + "//" + window.location.host + "/api/Branding/updateContract",
            data: JSON.stringify(obj),
            dataType: "json",
        })
        return response;
    };

    serv.getContractSummary = function () {
        var response = $http({
            method: "GET",
            url: window.location.protocol + "//" + window.location.host + "/api/Branding/getContractSummary",
        });
        return response;
    }

    serv.saveContractAns = function (obj) {
        var response = $http({
            method: "Post",
            url: window.location.protocol + "//" + window.location.host + "/api/Branding/SaveContractAns",
            data: JSON.stringify(obj),
            dataType: "json",
        })
        return response;
    };

    return serv;

});

