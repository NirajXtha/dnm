//templatesetupservice
// write all api get/post/put/delete code here
DTModule.factory('templatesetupfactory', function ($http) {
    var serv = {};
    serv.GetEntity = function () {
        //return $http.get('/api/PlanService/GetAllPlan');
    }

    serv.getallTemplatedate = function () {
        return $http.get('/api/TemplateApi/GetAllTemplateList');
    }

    serv.getallFormDetailSetupdate = function () {
        return $http.get('/api/TemplateApi/GetAllFormDetailSetup');
    }
    

    return serv;
});

// write all controller calculation part here
// you can inject 'frequencysetupfactory' here if you want.
DTModule.service('templatesetupservice', function (templatesetupfactory) {
    this.resetPlan = function ($scope) {
        // your calculation code
        // ex: templatesetupfactory.ser.GetEntity
    };

    this.GetAllTemplateDate = function () {
        var result = null;
        templatesetupfactory.getallTemplatedate().then(function (d) {
            
            result = d.data;
        });
        
    }
   
});