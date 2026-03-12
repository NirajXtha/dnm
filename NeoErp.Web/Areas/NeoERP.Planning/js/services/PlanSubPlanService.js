// write all api get/post/put/delete code here
planningModule.factory('plansubplanfactory', function ($http) {
    var serv = {};
    serv.GetEntity = function () {
        return $http.get('/api/SubPlanApi/TestMe');
    }
    serv.getFrequencyCoulum = function (plancode) {
        var url = window.location.protocol + "//" + window.location.host + "/api/SubPlanApi/GetFrequencyColumnByPlanCode?plancode="+plancode;
        return $http.get(url);
    }
    return serv;
});

// write all controller calculation part here
// you can inject 'plansubplanfactory' here if you want.
planningModule.service('plansubplanservice', function (plansubplanfactory) {
    this.getPlan = function ($scope) {
        // your calculation code
        // ex: plansubplanfactory.ser.GetEntity
        return plansubplanfactory.GetEntity();
    };
    this.getFrequencyForCoulum = function (plancode) {
        return plansubplanfactory.getFrequencyCoulum(plancode);
    }
});