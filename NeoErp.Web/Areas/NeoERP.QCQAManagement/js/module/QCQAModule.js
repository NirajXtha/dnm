var QCQAModule = angular.module('QCQAModule', ['ngRoute', 'ngMessages', 'datatables', 'kendo.directives', 'cfp.hotkeys', 'ngHandsontable', 'angularjs-dropdown-multiselect']).run(['$rootScope', function ($rootScope) {
    //console.log("hi", $rootScope);
    //$rootScope.formCode = "";
    //alert(12);
}]);
QCQAModule.config(function ($routeProvider, $locationProvider) {
    //$routeProvider.when('/Dashboard',
    //    {
    //        templateUrl: function () {
    //            return '/QCQAManagement/Home/Dashboard';
    //        },
    //    });
    $routeProvider.when('/AddQCQA', {
        templateUrl: '/QCQAManagement/Home/AddQCQA',
    });

    $routeProvider.when('/QCQADetail/:id',
        {
            templateUrl: function (stateParams) {

                return '/QCQAManagement/Home/QCQADetail?id=' + stateParams.id;
            },
            controller: 'quotationDetail',
            resolve: {
                module: function ($route) { return $route.current.params.id; }
            }

        });   
    $routeProvider.when('/EditQCQA/:id',
        {
            templateUrl: function (stateParams) {
                return '/QCQAManagement/Home/EditQCQA?id=' + stateParams.id;
            },
            controller: 'EditQCQA',
            resolve: {
                module: function ($route) { return $route.current.params.id; }
            }
        });
    //$routeProvider.when('/SummaryReport', {
    //    templateUrl: '/QCQAManagement/Home/SummaryReport',
    //});
    $routeProvider.when('/QCQASetup', {
        templateUrl: '/QCQAManagement/Home/QCQAInspectionSetup',
    });
    $routeProvider.when('/IncomingMaterialDirect', {
        templateUrl: '/QCQAManagement/Home/IncomingMaterialDirectList',
    });
    $routeProvider.when('/IncomingMaterial', {
        templateUrl: '/QCQAManagement/Home/IncomingMaterialList',
    });
    $routeProvider.when('/RawMaterial', {
        templateUrl: '/QCQAManagement/Home/RawMaterialList',
    });
    $routeProvider.when('/DailyWastage', {
        templateUrl: '/QCQAManagement/Home/DailyWastageList',
    });
    $routeProvider.when('/PreDispatchInspection', {
        templateUrl: '/QCQAManagement/Home/PreDispatchInspectionList',
    });
    $routeProvider.when('/HandOverInspection', {
        templateUrl: '/QCQAManagement/Home/HandOverInspectionList',
    });
    $routeProvider.when('/LabTesting', {
        templateUrl: '/QCQAManagement/Home/LabTestingList',
    });
    $routeProvider.when('/GlobalAgroProducts', {
        templateUrl: '/QCQAManagement/Home/GlobalAgroProductsList',
    });
    $routeProvider.when('/QCQANumberSetup', {
        templateUrl: '/QCQAManagement/Home/QCQANumberSetupList',
    });
    $routeProvider.when('/ProductSetup', {
        templateUrl: '/QCQAManagement/Home/ProductSetupList',
    });
    $routeProvider.when('/ParameterSetup', {
        templateUrl: '/QCQAManagement/Home/ParameterSetupList',
    });
    $routeProvider.when('/ParameterInspectionSetup', {
        templateUrl: '/QCQAManagement/Home/ParameterInspectionSetupList',
    });
    $routeProvider.when('/FinishedGoodsSetup', {
        templateUrl: '/QCQAManagement/Home/FinishedGoodsSetupList',
    });
    $routeProvider.when('/InternalInspectionSetup', {
        templateUrl: '/QCQAManagement/Home/InternalInspectionSetupList',
    });
    $routeProvider.when('/FinishedGoodsInspection', {
        templateUrl: '/QCQAManagement/Home/FinishedGoodsInspectionList',
    });
    $routeProvider.when('/OnSiteInspection', {
        templateUrl: '/QCQAManagement/Home/OnSiteInspectionList',
    });
    $routeProvider.when('/InternalInspection', {
        templateUrl: '/QCQAManagement/Home/InternalInspectionList',
    });
    $routeProvider.when('/SanitationHygiene', {
        templateUrl: '/QCQAManagement/Home/SanitationHygieneList',
    });
    $routeProvider.when('/SanitationHygieneReport', {
        templateUrl: '/QCQAManagement/Home/SanitationHygieneReportList',
    });
    $routeProvider.when('/QCQADocumentFinder', {
        templateUrl: '/QCQAManagement/Home/QCQADocumentFinderList',
    });



    $routeProvider.otherwise({
        redirectTo: function () {
            return '/QCQAManagement/Home/Dashboard';
        }
    })
    $locationProvider.html5Mode({ enable: true }).hashPrefix('!QCQA');

});

QCQAModule.config.$inject = ['$routeProvider', '$locationProvider'];
