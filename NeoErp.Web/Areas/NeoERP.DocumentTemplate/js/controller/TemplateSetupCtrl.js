/// <reference path="../module/DTModule.js" />
/*/// <reference path="~/Scripts/_references.js" />*/
/// <reference path="../../scripts/_references.js" />


DTModule.controller('TemplateSetupCtrl', function ($scope, $http, $filter, $q, templatesetupservice, templatesetupfactory) {
    $scope.pageName = "template setup controller";
    $scope.resultDate = [];
    $scope.test = "subin";
    $scope.GetTemplateDate = function () {
        
        $scope.resultDate = templatesetupfactory.getallTemplatedate().then(function (d) {
            
            $scope.resultDate = d.data;
            console.log("subin " + $scope.resultDate[0].name);
            $scope.test = $scope.resultDate[0].name;
        });
    }
    
});
