/// <reference path="~/Scripts/_references.js" />
var distributionModule = angular.module('distributorModule', ["ngRoute", "kendo.directives", "ngMessages", "datatables"]);
distributionModule.config.$inject = ['$routeProvider', '$locationProvider'];
distributionModule.config(function ($routeProvider, $locationProvider) {
    // redirect to Saleshome dashboard (for now) or Login page.
    $routeProvider.when('/Dashboard',
        {
            templateUrl: '/distribution/Distributor/DistributorDashboard',
        });
    $routeProvider.when('/PurchaseReport',
        {
            templateUrl: '/distribution/Distributor/ResellerPurchaseOrder'
       });
    $routeProvider.when('/WholeSellerPurchaseReport',
        {
            templateUrl: '/distribution/Distributor/WholeSellerPurchaseReport'
        });
    $routeProvider.when('/ApprovePurchaseReport',
        {
            templateUrl: '/distribution/Distributor/ResellerPOStatus'
        });
    $routeProvider.when('/ResellerPOSummaryStatus',
        {
            templateUrl: '/distribution/Distributor/ResellerPOSummaryStatus'
        });
    $routeProvider.when('/PurchaseOrderSummary', {
        templateUrl: '/distribution/Distributor/PurchaseOrderSummary'
    });
    $routeProvider.when('/PurchaseOrder',
        {
            templateUrl: '/distribution/Distributor/PurchaseOrder'
        });
    $routeProvider.when('/CollectionReport',
        {
            templateUrl: '/distribution/Distributor/CollectionReport'
        });
    $routeProvider.when('/AccountStatement',
        {
            templateUrl: '/distribution/Distributor/AccountStatement'
        });
    $routeProvider.when('/ClosingStock',
        {
            templateUrl: '/distribution/Distributor/ClosingReportNew'
       });
    $routeProvider.when('/OpeningStockSetup',
        {
            templateUrl: '/distribution/Setup/OpeningStockSetup'
        });
    $routeProvider.when('/ClosingStockSetup',
        {
            
            templateUrl: '/distribution/Setup/ClosingStock'
        });
    $routeProvider.when('/SalesRegister',
        {
            templateUrl: '/distribution/Report/SalesRegister'
        });
    $routeProvider.when('/OutletClosingReport',
        {
            templateUrl: '/distribution/Report/OutletClosingStock'
        });
    $routeProvider.when('/BranchWiseSalesVsTarget',
        {
            templateUrl: '/distribution/Distributor/BranchWiseSalesVsTarget'
        });
    $routeProvider.when('/DistWidgets',
        {
            templateUrl: '/distribution/Distributor/DistWidgets'
        });
    $routeProvider.when('/DivisionWiseCreditLismit',
        {
            templateUrl: '/distribution/Distributor/DivisionWiseCreditLismit'
        });
    $routeProvider.when('/AgeingReport',
        {
            templateUrl: '/distribution/Distributor/AgingReport'
        });
    $routeProvider.when('/TargetAchievement',
        {
            templateUrl: '/distribution/Distributor/SalesTargetvsAchievementReport'
        });
    $routeProvider.when('/CustomerAchievement',
        {
            templateUrl: '/distribution/Distributor/SalesTargetvsCustomerAchievementReport'
        });
    $routeProvider.when('/SchemeReportNew',
        {
            templateUrl: '/distribution/Distributor/SchemeReport'
        });

    $routeProvider.otherwise({ redirectTo: '/Dashboard' });
    $locationProvider.html5Mode({ enable: false }).hashPrefix('!Distributor');
});