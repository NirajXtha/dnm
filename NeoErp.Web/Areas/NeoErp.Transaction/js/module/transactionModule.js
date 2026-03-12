/// <reference path="~/Scripts/_references.js" />
var transactionModule = angular.module('transactionModule', ['ngRoute', 'ngMessages', 'datatables', 'kendo.directives']);

transactionModule.config(function ($routeProvider, $locationProvider) {
    // redirect to Saleshome dashboard (for now) or Login page.
    $routeProvider.when('/',
    {
        templateUrl: '/Transaction/Home/Dashboard',
        //controller: 'transactionCtrl'
    });
    $routeProvider.when('/ConsumptionIssue',
    {
        templateUrl: '/Transaction/Home/ConsumptionIssue',
        controller: 'transactionCtrl'
    });
    $routeProvider.when('/BankSetup',
    {
        templateUrl: '/Transaction/Bank/BankSetup',
        controller: 'BankSetupCtrl'
    });
    $routeProvider.when('/TransactionSetup',
    {
        templateUrl: '/Transaction/Bank/BankLimitSetup',
        controller: 'BankLimitSetupCtrl'
    });
    $routeProvider.when('/TransactionList',
    {
        templateUrl: '/Transaction/Bank/BankLimitList',
        controller: 'KendoCtrl'
    });
    $routeProvider.when('/CategorySetup',
    {
        templateUrl: '/Transaction/Bank/LoanCategorySetup',
        controller: 'CategoryCtrl'
    });
    $routeProvider.otherwise({ redirectTo: '/transaction/Home/Index' });
    $locationProvider.html5Mode({ enable: true }).hashPrefix('!Transaction');
});

transactionModule.config.$inject = ['$routeProvider', '$locationProvider'];