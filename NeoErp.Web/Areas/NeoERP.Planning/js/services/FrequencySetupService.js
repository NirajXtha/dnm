//frequencysetupservice
// write all api get/post/put/delete code here
planningModule.factory('frequencysetupfactory', function ($http) {
    var serv = {};
    serv.GetEntity = function () {
        //return $http.get('/api/PlanService/GetAllPlan');
    }
    return serv;
});

// write all controller calculation part here
// you can inject 'frequencysetupfactory' here if you want.
planningModule.service('frequencysetupservice', function (frequencysetupfactory) {
    this.resetPlan = function ($scope) {
        // your calculation code
        // ex: frequencysetupfactory.ser.GetEntity
    };
});