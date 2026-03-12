var distributionModule = angular.module('distributionModule', ["ngRoute", "kendo.directives", "ngMessages", "datatables"]);
distributionModule.config.$inject = ['$routeProvider', '$locationProvider'];
distributionModule.config(function ($routeProvider, $locationProvider) {
    // redirect to Saleshome dashboard (for now) or Login page.
    $routeProvider.when('/Dashboard',
        {
            templateUrl: '/distribution/Home/Index',
            //  controller: 'distributionCtrl'
        });
    $routeProvider.when('/VisitSummary',
        {
            templateUrl: '/distribution/Home/VisitSummaryReport'
        });
    $routeProvider.when('/QuestionnaireReport',
        {
            templateUrl: '/distribution/Report/QuestionaireReport'
        });
    $routeProvider.when('/collectionreport',
        {
            templateUrl: '/distribution/Report/CollectionsReport'
        });

    $routeProvider.when('/performanceReport',
        {
            templateUrl: '/distribution/Report/performanceReport'
        });
    $routeProvider.when('/MrVisitTracking',
        {
            templateUrl: '/distribution/Report/MrVisitReportMap'
        });
    $routeProvider.when('/SalesPersonPO',
       {
           templateUrl: '/distribution/Report/SalesPersonPO'
       });
    $routeProvider.when('/ItemCumulativeReport',
     {
         templateUrl: '/distribution/Report/ItemCumulativeReport'
     });
    $routeProvider.when('/resellerordersummary',
        {
            templateUrl: '/distribution/Report/ResellerOrderSummaryReport'
        });
    $routeProvider.when('/getSalesOrderDetails', {
        templateUrl: '/distribution/Report/getSalesOrderDetails'
    });
    //  /Distribution/Home/Dashboard#!Distribution/collectionreport
    $routeProvider.when('/Purchase/POIndex',
        {
            templateUrl: '/Distribution/Purchase/POIndex'
        });

    $routeProvider.when('/Report/DistSalesReturn', {
        templateUrl: '/Distribution/Purchase/DistSalesReturn',
      //  controller : 'DistSalesReturnCtrl'
        
    });
    $routeProvider.when('/Purchase/POEdit',
        {
            templateUrl: '/Distribution/Purchase/EditPO',
            controller: 'POEditCtrl'
        });
    $routeProvider.when('/PurchaseReport',
       {
           templateUrl: '/distribution/Distributor/ResellerPurchaseOrder'
        });
    
    $routeProvider.when('/ApprovePurchaseReport',
        {
            templateUrl: '/distribution/Distributor/ResellerPOStatus'
        });
    $routeProvider.when('/ResellerPOSummaryStatus',
        {
            templateUrl: '/distribution/Distributor/ResellerPOSummaryStatus'
        });
    $routeProvider.when('/Purchase/CancelledSalesOrder',
        {
            templateUrl: '/distribution/Purchase/CancelledSalesOrder',
        });
    $routeProvider.when('/Purchase/ApprovedSalesOrder',
        {
            templateUrl: '/distribution/Purchase/ApprovedSalesOrder',
        });
    $routeProvider.when('/UserSetup',
        {
            templateUrl: '/distribution/Home/UserSetup'
        });
    $routeProvider.when('/DistributorSetup',
        {
            templateUrl: '/distribution/Setup/DistributorSetup',
        });
    $routeProvider.when('/ResellerSetup',
        {
            templateUrl: '/distribution/Setup/ResellerSetup',
            controller: 'ResellerCtrl'
        });
    $routeProvider.when('/QuestionSetup',
        {
            templateUrl: '/distribution/Setup/QuestionSetup',
            controller: 'QuestionCtrl'
        });
    $routeProvider.when('/QuestionList',
        {
            templateUrl: '/distribution/Setup/QuestionList',
            controller: 'QuestionListCtrl'
        });
    $routeProvider.when('/PreferenceSetup',
        {
            templateUrl: '/distribution/Setup/PreferenceSetup',
        });
    $routeProvider.when('/RouteSetup',
        {
            templateUrl: '/distribution/Setup/RouteSetup',
            
        });

    $routeProvider.when('/CustomerSetup',
        {
            templateUrl: '/distribution/Setup/CustomerSetup',
            // controller: 'DistributorCtrl'
        });
    $routeProvider.when('/DealerSetup',
        {
            templateUrl: '/distribution/Setup/DealerSetup',
            // controller: 'DistributorCtrl'
        });

    $routeProvider.when('/AreaSetup',
        {
            templateUrl: '/distribution/Setup/AreaSetup',
            // controller: 'DistributorCtrl'
        });
    $routeProvider.when('/GroupSetup',
        {
            templateUrl: '/distribution/Setup/GroupSetup',
            controller: 'GroupCtrl'
        });
    $routeProvider.when('/ImageCategorySetup',
        {
            templateUrl: '/distribution/Setup/ImageCategorySetup',
            controller: 'ImageCategoryCtrl'
        });
    $routeProvider.when('/UserSetupTree',
        {
            templateUrl: '/distribution/Setup/UserSetupTree',           
        });

    $routeProvider.when('/getOutLet', {
        templateUrl: '/distribution/Setup/OutLetSetup'
    });


    $routeProvider.when('/EOD', {
        templateUrl: '/distribution/Report/EOD'
    });

    $routeProvider.when('/AttendanceReport',
        {
            templateUrl: '/distribution/Report/AttendanceReport'
        });

    $routeProvider.when('/MRVisitTrackingMap',
        {
            templateUrl: '/distribution/Report/MRVisitTrackingMap'
        });
    $routeProvider.when('/EmployeeWisePerformance',
        {
            templateUrl: '/distribution/Report/EmployeeWisePerformance'
        });
    $routeProvider.when('/DailyActivityReport',
        {
            templateUrl: '/distribution/Report/DailyActivityReport'
        });


    $routeProvider.otherwise({ redirectTo: '/' });
    $locationProvider.html5Mode({ enable: false }).hashPrefix('!Distribution');
});

