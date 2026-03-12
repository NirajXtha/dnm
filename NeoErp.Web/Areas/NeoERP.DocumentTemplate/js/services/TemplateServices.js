// write all api get/post/put/delete code here
DTModule.factory('templatefactory', function ($http) {
    var serv = {};
    serv.GetEntity = function () {
        //return $http.get('/api/PlanService/GetAllPlan');
    }
    return serv;
});

// write all controller calculation part here
// you can inject 'planfactory' here if you want.
DTModule.service('templateservice', function (templatefactory) {
    this.resetPlan = function ($scope) {
        // your calculation code
        // ex: templatefactory.ser.GetEntity
    };
});