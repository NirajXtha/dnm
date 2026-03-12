// write all api get/post/put/delete code here
planningModule.factory('planlistfactory', function ($http) {
    var serv = {};
    serv.GetEntity = function () {
        //return $http.get('/api/PlanService/GetAllPlan');
    }
    return serv;
});

// write all controller calculation part here
// you can inject 'planfactory' here if you want.
planningModule.service('planlistservice', function (planlistfactory) {
    this.resetPlan = function ($scope) {
        // your calculation code
        // ex: planfactory.ser.GetEntity
    };
});