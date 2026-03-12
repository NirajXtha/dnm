/// <reference path="../module/DTModule.js" />
/*/// <reference path="~/Scripts/_references.js" />*/
/// <reference path="../../scripts/_references.js" />


DTModule.controller('TemplateCtrl', function ($scope, templateservice, templatesetupfactory) {
    $scope.pageName = "Template Index";

    templatesetupfactory.getallFormDetailSetupdate().then(function (result) {
        
        //$scope.model.Name = result.Name;
    });
});

