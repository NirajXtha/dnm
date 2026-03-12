
var PMModule = angular.module('PMModule', ['ngRoute', 'ngMessages', 'datatables', 'kendo.directives', 'cfp.hotkeys', 'ngHandsontable', 'angularjs-dropdown-multiselect']).run(['$rootScope', function ($rootScope) {

}]);
PMModule.config(function ($routeProvider, $locationProvider) {
    $routeProvider.when('/',
        {
            templateUrl: function () {
                return '/ProjectManagement/Home/Dashboard';
            },
        });

    $routeProvider.when('/Dashboard',
        {
            templateUrl: function () {
                return '/ProjectManagement/Home/Dashboard';
            },
        });
    $routeProvider.when('/ProjectSetup', {
        templateUrl: '/ProjectManagement/Home/ProjectSetup',
    });
    $routeProvider.when('/AddProject', {
        templateUrl: '/ProjectManagement/Home/AddProject',
    });
    $routeProvider.when('/Project', {
        templateUrl: '/ProjectManagement/Home/Project',
    });
    $routeProvider.when('/Dashboard', {
        templateUrl: '/ProjectManagement/Home/Dashboard',
    });
    $routeProvider.when('/RequisitionEntry', {
        templateUrl: '/ProjectManagement/Home/RequisitionEntry',
        controller: 'Requisition',

    });
    $routeProvider.when('/RequisitionEntry/:formcode/:orderno', {

        templateUrl: function (stateParams) {

            return '/ProjectManagement/Home/RequisitionEntry?formCode=' + stateParams.formcode + '&voucherNo=' + stateParams.orderno;
        },
        controller: 'Requisition',
        resolve:
        {
            formcode: function ($route) { return $route.current.params.formcode; },
            OrderNo: function ($route) { return $route.current.params.orderno; }
        }

    });
    $routeProvider.when('/MenuSplitter/',
        {
            templateUrl: '/ProjectManagement/Home/MenuSplitter',
            controller: 'consumptionMenuSplitter',

        });

    $routeProvider.when('/MenuSplitter/:module',
        {
            templateUrl: function (stateParams) {

                return '/ProjectManagement/Home/MenuSplitter?module=' + stateParams.module;
            },
            controller: 'consumptionMenuSplitter',
            resolve: {
                module: function ($route) { return $route.current.params.module; }
            }

        });
    $routeProvider.when('/ReqMenuSplitter/:module',
        {
            templateUrl: function (stateParams) {

                return '/ProjectManagement/Home/ReqMenuSplitter?module=' + stateParams.module;
            },
            controller: 'ReqMenuSplitter',
            resolve: {
                module: function ($route) { return $route.current.params.module; }
            }

        });
    $routeProvider.when('/PurMenuSplitter/:module',
        {
            templateUrl: function (stateParams) {

                return '/ProjectManagement/Home/PurMenuSplitter?module=' + stateParams.module;
            },
            controller: 'PurMenuSplitter',
            resolve: {
                module: function ($route) { return $route.current.params.module; }
            }

        });
    $routeProvider.when('/AuxMenuSplitter/:module',
        {
            templateUrl: function (stateParams) {

                return '/ProjectManagement/Home/AuxMenuSplitter?module=' + stateParams.module;
            },
            controller: 'consumptionMenuSplitter',
            resolve: {
                module: function ($route) { return $route.current.params.module; }
            }

        });
    $routeProvider.when('/ConsumptionEntry', {
        templateUrl: '/ProjectManagement/Home/ConsumptionEntry',
        controller: 'consumption',

    });
    $routeProvider.when('/ConsumptionEntry/:formcode/:orderno', {

        templateUrl: function (stateParams) {

            return '/ProjectManagement/Home/ConsumptionEntry?formCode=' + stateParams.formcode + '&voucherNo=' + stateParams.orderno;
        },
        controller: 'consumption',
        resolve:
        {
            formcode: function ($route) { return $route.current.params.formcode; },
            OrderNo: function ($route) { return $route.current.params.orderno; }
        }

    });
    $routeProvider.when('/AuxiliaryEntry', {
        templateUrl: '/ProjectManagement/Home/AuxiliaryEntry',
        controller: 'consumption',

    });
    $routeProvider.when('/PurchaseEntry', {
        templateUrl: '/ProjectManagement/Home/PurchaseEntry',
        controller: 'Purchase',

    });
    $routeProvider.when('/PurchaseEntry/:formcode/:orderno', {

        templateUrl: function (stateParams) {

            return '/ProjectManagement/Home/PurchaseEntry?formCode=' + stateParams.formcode + '&voucherNo=' + stateParams.orderno;
        },
        controller: 'Purchase',
        resolve:
        {
            formcode: function ($route) { return $route.current.params.formcode; },
            OrderNo: function ($route) { return $route.current.params.orderno; }
        }

    });
    $routeProvider.when('/RequisitionReport', {
        templateUrl: '/ProjectManagement/Home/RequisitionReport',
    });
    $routeProvider.when('/RequisitionPendingReport', {
        templateUrl: '/ProjectManagement/Home/RequisitionPendingReport',
    });
    $routeProvider.when('/ConsumptionReport', {
        templateUrl: '/ProjectManagement/Home/ConsumptionReport',
    });
    $routeProvider.when('/PurchaseReport', {
        templateUrl: '/ProjectManagement/Home/PurchaseReport',
    });
    //$routeProvider.when('/ProjectDashboard', {
    //    templateUrl: '/ProjectManagement/Home/ProjectDashboard',
    //});
    $routeProvider.otherwise({
        redirectTo: function () {
            return '/ProjectManagement/Home/Dashboard';
        }
    })
    $locationProvider.html5Mode({ enable: true }).hashPrefix('!PM');
});

PMModule.config.$inject = ['$routeProvider', '$locationProvider'];
