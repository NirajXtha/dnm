distributionModule.factory('DistSetupService', function ($http) {
    var serv = {};

    // Add Route
    serv.AddRoute = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/AddRoute",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }

    // Update Route
    serv.UpdateRoute = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/UpdateRoute",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }
    serv.deleteRoute = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/deleteRoute",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    };

    serv.GetRouteDistributorList = function (codes) {
        var response = $http({
            method: 'GET',
            url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetRouteDistResellerByArea?areaCode=" + codes,
        });
        return response;
    }



    serv.getAreaByRouteCode = function (routeCode) {
        var response = $http({
            method: 'GET',
            url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetAreaByRouteCode?routeCode=" + routeCode,
        });
        return response;
    }


    serv.getSelectedCustomerByRouteCode = function (routeCode) {
        var response = $http({
            method: 'GET',
            url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetSelectedCustomerByRouteCode?routeCode=" + routeCode,
        });
        return response;
    }


    //add dealer
    serv.AddDealer = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/AddDealer",
            data: JSON.stringify(obj),
            dataType: "json",
        });
        return response;
    }

    serv.deleteDealer = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/deleteDealer",
            data: JSON.stringify(obj),
            dataType: "json",
        });
        return response;
    }

    //update dealer
    serv.UpdateDealer = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/UpdateDealer",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }

    serv.AddOutlet = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/AddOutlet",
            data: JSON.stringify(obj),
            dataType: "json"
        });
        return response;
    }

    serv.deleteItem = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/deleteItem",
            data: JSON.stringify(obj),
            dataType: "json"
        });

        return response;
    }

    serv.deleteGroup = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/deleteGroup",
            data: JSON.stringify(obj),
            dataType: "json"
        });

        return response;
    }

    serv.getSubTypeList = function (TYPE_ID) {
        var response = $http({
            method: 'GET',
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/getAllSubOutletList?TYPE_ID=" + TYPE_ID,
        });
        return response;
    }

    serv.AddResellerGroup = function (data) {
        var response = $http({
            method: 'POST',
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/AddResellerGroup",
            data: data,
            dataType: "json",
        });
        return response;
    }
    serv.AddImageCategory = function (data) {
        var response = $http({
            method: 'POST',
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/AddCategoryImage",
            data: data,
            dataType: "json",
        });
        return response;
    }
    serv.deleteImage = function (obj) {
        var response = $http({
            method: 'POST',
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/deleteImage",
            data: JSON.stringify(obj),
            dataType: "json",
        });
        return response;
    }

    serv.AddUserTree = function (data) {
        var response = $http({
            method: 'POST',
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/SaveUserTree",
            data: data,
            dataType: "json",
        });
        return response;
    }

    serv.UpdateUserTree = function (data) {
        var response = $http({
            method: 'POST',
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/UpdateUserTree",
            data: data,
            dataType: "json",
        });
        return response;
    }


    serv.UpdateUserTreeOrder = function (data) {
        var response = $http({
            method: 'POST',
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/UpdateUserTreeOrder",
            data: data,
            dataType: "json",
        });
        return response;
    }

    serv.GetVisiterList = function (dateFilter1, data) {
        debugger;
        var response = $http({
            method: "POST",
            data: dateFilter1,
            url: window.location.protocol + "//" + window.location.host + "/" + "api/Report/GetVisiterList?Distributor=" + data.distributor + "&Reseller=" + data.Reseller,
            dataType: "json",
        });
        return response;
    }
    serv.GetNotVisitedList = function (dateFilter1, data) {
        debugger;
        var response = $http({
            method: "POST",
            data: dateFilter1,
            url: window.location.protocol + "//" + window.location.host + "/" + "api/Report/GetNotVisitedList?Distributor=" + data.distributor + "&Reseller=" + data.Reseller,
            dataType: "json",
        });
        return response;
    }
    serv.GetBrandingVisiterList = function (dateFilter1, data) {
        var response = $http({
            method: "POST",
            data: dateFilter1,
            url: window.location.protocol + "//" + window.location.host + "/" + "api/Report/GetBrandingVisiterList?Distributor=" + data.distributor + "&Reseller=" + data.Reseller,
            dataType: "json",
        });
        return response;
    }
    serv.GetVisiterListGalary = function (dateFilter1, data) {
        var response = $http({
            method: "POST",
            data: dateFilter1,
            url: window.location.protocol + "//" + window.location.host + "/" + "api/Report/GetVisiterList?Distributor=" + data.distributor + "&Reseller=" + data.Reseller,
            dataType: "json",
        });
        return response;
    }
    serv.GetVisiterListCondition = function (dateFilter1, data) {
        var response = $http({
            method: "POST",
            data: dateFilter1,
            url: window.location.protocol + "//" + window.location.host + "/" + "api/Report/GetVisiterListCondition?Distributor=" + data.distributor + "&Reseller=" + data.Reseller,
            dataType: "json",
        });
        return response;
    }
    serv.AddNotification = function (data) {
        var response = $http({
            method: "POST",
            data: data,
            url: window.location.protocol + "//" + window.location.host + "/" + "api/Setup/SaveNotification",
            dataType: "json",
        });
        return response;
    }

    serv.DeleteNotification = function (id) {
        var response = $http({
            method: "GET",
            url: window.location.protocol + "//" + window.location.host + "/" + "api/Setup/DeleteNotification?Id=" + id,
        });
        return response;
    }

    serv.GetAllCompMaps = function () {
        var response = $http({
            method: "GET",
            url: window.location.protocol + "//" + window.location.host + "/" + "api/Setup/GetCompMap",
        });
        return response;
    }

    serv.SaveCompMap = function (data) {
        var response = $http({
            method: "POST",
            data: data,
            url: window.location.protocol + "//" + window.location.host + "/" + "api/Setup/SaveCompItemMap",
            dataType: "json",
        });
        return response;
    }

    serv.GetAllGroupMaps = function () {
        var response = $http({
            method: "GET",
            url: window.location.protocol + "//" + window.location.host + "/" + "api/Setup/GetGroupMap",
        });
        return response;
    }
    serv.GetAllUserMaps = function () {
        var response = $http({
            method: "GET",
            url: window.location.protocol + "//" + window.location.host + "/" + "api/Setup/GetDistUserMap",
        });
        return response;
    }

    serv.SaveGroupMap = function (data) {
        var response = $http({
            method: "POST",
            data: data,
            url: window.location.protocol + "//" + window.location.host + "/" + "api/Setup/SaveGroupMap",
            dataType: "json",
        });
        return response;
    }
    serv.SaveDistUserMap = function (data) {
        var response = $http({
            method: "POST",
            data: data,
            url: window.location.protocol + "//" + window.location.host + "/" + "api/Setup/SaveDistUserMap",
            dataType: "json",
        });
        return response;
    }

    serv.deleteUserMap = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/deleteUserMap",
            data: JSON.stringify(obj),
            dataType: "json"
        });

        return response;
    }

    serv.SaveCompField = function (data) {
        var response = $http({
            method: "POST",
            data: data,
            url: window.location.protocol + "//" + window.location.host + "/" + "api/Setup/SaveCompField",
            dataType: "json",
        });
        return response;
    }

    //For EmployeeSetup Grid
    serv.GetEmployee = function () {
        var response = $http({
            method: 'GET',
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetEmployee",
        });
        return response;
    }

    //add Employee
    serv.CreateEmployee = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/CreateEmployee",
            data: JSON.stringify(obj),
            dataType: "json",
        });
        return response;
    }
    

    //Delete Employee
    serv.DeleteEmployee = function (id) {
        var response = $http({
            method: "get",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/DeleteEmployee?Id=" + id,
            dataType: "json"
        });

        return response;
    }


    //GetCompItem
    serv.GetCompItem = function ( ) {
        var response = $http({
            method: 'GET',
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetCompItem",
        });
        return response;
    }


    //add CompItem
    serv.CreateCompItem = function (obj) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/CreateCompItem",
            data: JSON.stringify(obj),
            dataType: "json",
        });
        return response;
    }

    //Delete CompItem
    serv.DeleteCompItem = function (id) {
        var response = $http({
            method: "get",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/DeleteCompItem?Id=" + id,
            dataType: "json"
        });

        return response;
    }

    //save default fileds
    serv.SaveDefaultFields = function () {
        var response = $http({
            method: "get",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/DefaultCompFileds",
        });
        return response;
    }

    serv.SaveItem = function (item) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/SaveItem",
            data: item,
            dataType: "json",
            headers: { 'Content-Type': undefined },
        });
        return response;
    }

    serv.SaveSurvey = function (item) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/SaveSurvey",
            data: item,
            dataType: "json",
        });
        return response;
    }

    return serv;
});