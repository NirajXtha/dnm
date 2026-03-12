planningModule.controller('BrandingPlanSetupCtrl', function ($scope, $routeParams, $rootScope) {

        //$rootScope.reservedBudgetData;    
        $scope.p_code = $routeParams.plancode;
        $scope.param = $rootScope.reservedBudgetData;
        isParamLoaded.resolve($scope.param);
        $scope.pageName = "Branding Plan Setup";
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
           
            if (confirm("All input data will lost, Are you sure to go back?")) {
                if ($routeParams.plancode != '' && $routeParams.plancode != undefined && $routeParams.plancode != 'undefined') {
                    window.location = "/Planning/Home/Index#!Planning/CreateBrandingPlan/" + $routeParams.plancode;
                }
                else {
                    window.location = "/Planning/Home/Index#!Planning/CreateBrandingPlan";
                }
            }
        }

        $scope.cancelForm = function () {
            window.location.href = "/Planning/Home/Setup#!Planning/BrandingPlan";
        }
    });
