/// <reference path="~/Scripts/_references.js" />
/// <reference path="../module/planningModule.js" />

planningModule.controller('LedgerBudgetPlanSetupCtrl', function ($scope, $routeParams, $rootScope) {
     
    //$rootScope.reservedBudgetData;    
    $scope.p_code = $routeParams.plancode;
    $scope.param = $rootScope.reservedBudgetData;
    isParamLoaded.resolve($scope.param);
    $scope.pageName = "Ledger Budget plan setup";
    $scope.treelistOptions = [];
    $scope.Frequency = [];
    $scope.ITEM = [];
    $scope.Plan = [];
    $scope.startDate = [];
    $scope.endDate = [];

    $scope.gotoBack = function () {
        
        $scope.param;
        $rootScope.reservedBudgetData;
        var plandetail_temp = $scope.param;
        //var elem = angular.element(document.querySelector('[ng-app]'));
        //var injector = elem.injector();
        //var $rootScope = injector.get('$rootScope');
        //$rootScope.$apply(function () {
        //    $rootScope.reservedBudgetData = plandetail_temp;
        //    nextDiffer.resolve(plandetail_temp);
        //});
        //$rootScope.reservedBudgetData= $scope.param;        
        if (confirm("All input data will lost, Are you sure to go back?")) {
            if ($routeParams.plancode != '' && $routeParams.plancode != undefined && $routeParams.plancode != 'undefined') {
                window.location = "/Planning/Home/Index#!Planning/CreateLedgerBudgetPlan/" + $routeParams.plancode;
            }
            else {
                window.location = "/Planning/Home/Index#!Planning/CreateLedgerBudgetPlan";
            }
        }
    }

    $scope.cancelForm = function () {
        window.location.href = "/Planning/Home/Setup#!Planning/LedgerBudgetPlan";
    }
});
