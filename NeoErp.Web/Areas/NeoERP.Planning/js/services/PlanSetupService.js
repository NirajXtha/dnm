// write all api get/post/put/delete code here
planningModule.factory('plansetupfactory', function ($http) {
    var serv = {};
    serv.GetEntity = function () {
        //return $http.get('/api/PlanService/GetAllPlanSetup');
    }
    serv.GetItem = function() {
        return $http.get("/api/PlanSetupApi/getItem");
    }

    serv.getTitleValue = function(planCode) {
         var response = $http({
            method: "POST",
            url: "/api/PlanSetupApi/getTitleValues",
            params: { planCode: planCode },
            dataType: "json"
        });
        return response;
    }
    return serv;
});

// write all controller calculation part here
// you can inject 'plansetupfactory' here if you want.
planningModule.service('plansetupservice', function (plansetupfactory) {
    this.resetPlan = function ($scope) {
        // your calculation code
        // ex: plansetupfactory.ser.GetEntity
    };
    this.getItem = function() {
        return plansetupfactory.GetItem();
    }

    this.getTitleValue = function (planCode) {
         return plansetupfactory.getTitleValue(planCode);
    }
});