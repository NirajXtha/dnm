planningModule.controller('MaterialPlanSetupCtrl', function ($scope, plansetupservice, $routeParams, $rootScope) {
    $rootScope.reservedData;
    
    $scope.p_code = $routeParams.plancode;
    $scope.param = $rootScope.reservedData;

    $scope.pageName = "Material Planning setup Index";
    $scope.treelistOptions = [];
    $scope.Frequency = [];
    $scope.ITEM = [];
    $scope.Plan = [];
    $scope.startDate = [];
    $scope.endDate = [];

    $scope.gotoBack = function () {
        
        $rootScope.reservedData;
        if (confirm("All input data will lost, Are you sure to go back?")) {
            if ($routeParams.plancode != '' && $routeParams.plancode != undefined && $routeParams.plancode != 'undefined') {
                 window.location = "/Planning/Home/Index#!Planning/CreateMaterialPlanReference/" + $routeParams.plancode;
            }
            else {
                window.location = "/Planning/Home/Index#!Planning/CreateMaterialPlanReference";
            }
        }
    }
        
    $scope.cancelForm = function () {
        window.location.href = "/Planning/Home/Setup#!Planning/MaterialPlan";
    }
});
