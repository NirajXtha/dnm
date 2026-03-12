// write all api get/post/put/delete code here
planningModule.factory('planservice', function ($http) {
    var serv = {};
    serv.getPlanInfo = function (plancode) {
        //return $http.get('/api/PlanService/GetAllPlan');
        var url = window.location.protocol + "//" + window.location.host + "/api/PlanSetupApi/GetAllPlanNames?filter="+plancode;
        return $http.get(url);
    }
    return serv;
});

// write all controller calculation part here
// you can inject 'planfactory' here if you want.
//planningModule.service('planservice', function (planfactory) {
//    this.resetPlan = function ($scope) {
//        // your calculation code
//        // ex: planfactory.ser.GetEntity
//    };
//});