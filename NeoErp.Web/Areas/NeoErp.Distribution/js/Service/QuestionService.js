distributionModule.factory('QuestionService', function ($http) {
    var serv = {};
    serv.AddGeneral = function (general) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/AddGeneralQuestions",
            data: general,
            dataType: "json"
        });
        return response;
    }
    serv.AddTabular = function (tabular) {
        var response = $http({
            method: "post",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/AddTabularQuestions",
            data: tabular,
            dataType: "json"
        });
        return response;
    }
    serv.GetGeneral = function (setId) {
        var response = $http({
            method: "get",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/GeneralBySetId?setId=" + setId,
            dataType: "json"
        });
        return response;
    }
    serv.GetTabular = function (setId) {
        var response = $http({
            method: "get",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/TabularBySetId?setId=" + setId,
            dataType: "json"
        });
        return response;
    }
    return serv;
});

distributionModule.factory('QuestionListService', function ($http) {
    var serv = {};

    return serv;
});