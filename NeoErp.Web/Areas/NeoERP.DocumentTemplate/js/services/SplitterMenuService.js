

DTModule.service('splittermenuservice', function (splittermenufactory) {

    this.getFormDetail_ByFormCode = function (formCode, d1) {
        var formDetail = inventoryfactory.getFormDetail(formCode)
            .then(function (result) {
                d1.resolve(result);
            }, function (err) { });
    };

});

DTModule.factory('splittermenufactory', function ($http) {
    var fac = {};
    fac.getFormDetail = function (formcode) {
        var req = "/api/TemplateApi/GetFormDetailSetup?formCode=";
        return $http.get(req + formcode);
    }

    return fac;
});