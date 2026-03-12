var app = angular.module("dashboardApp", ["ngRoute", "kendo.directives", "ngMessages", "datatables"]);
app.config.$inject = ["$routeProvider", "$locationProvider"];

app.config(function ($routeProvider, $locationProvider) {
    var path = "@myAreaName";
    $routeProvider

        .when('/LcSetup',
        {
            templateUrl: '/Loc/Home/LcSetup',
            //controller: 'LcSetupCtrl'
        })
        .when('/',
        {
            templateUrl: '/Loc/Home/Dashboard',
            //controller: 'LCEntryController'
        })
        .when('/Dashboard',
        {
            templateUrl: '/Loc/Dashboard/Index',
            //controller: 'PerformaInvoiceCtrl'
        })
        .when('/ProformaInvoice',
        {
            templateUrl: '/Loc/ProformaInvoice/ProformaInvoice',
            //controller: 'PerformaInvoiceCtrl'
        })
        .when('/PurchaseOrder',
        {
            templateUrl: '/Loc/PurchaseOrder/PurchaseOrder',
            //controller: 'PurchaseOrderController'
        })
        .when('/LcEntry',
        {
            templateUrl: '/Loc/Home/LcEntry',
            //controller: 'LCEntryController'
        })
        .when('/LogisticPlan',
        {
            templateUrl: '/Loc/LogisticPlan/Index',

        })
        .when('/CommercialInvoice',
        {
            templateUrl: '/Loc/CommercialInvoice/Index',
            //controller: 'CommercialInvoiceCtrl'
        })
        .when('/PaymentSettlement',
        {
            templateUrl: '/Loc/CIPaymentSettlement/Index',

        })
        .when('/Logistics',
        {
            templateUrl: '/Loc/Logistics/Logistics',

        })
        .when('/GRN',
        {
            templateUrl: '/Loc/GRN/Index',

        })
       
        .when('/PendingLcReport',
        {
            templateUrl: '/Loc/LcReport/PendingLcReport',

        })

        .when('/PoPendingReport',
        {
            templateUrl: '/Loc/LcReport/PoPendingReport',

        })
        .when('/OpenLcReport',
        {
            templateUrl: '/Loc/LcReport/OpenLcReport',

        })
        .when('/DueInvoiceReport',
        {
            templateUrl: '/Loc/LcReport/DueInvoiceReport',

        })
        .when('/LcStatusReport',
        {
            templateUrl: '/Loc/LcReport/LcStatusReport',

        })
        .when('/VMovReport',
        {
            templateUrl: '/Loc/LcReport/VMovReport',

        })
         .when('/URVMovReport',
        {
            templateUrl: '/Loc/LcReport/URVMovReport',

        })
        .when('/LcProductWiseReport',
        {
            templateUrl: '/Loc/LcReport/LcProductWiseReport',

        })
        .when('/PendingCommercialInvoiceReport',
        {
            templateUrl: '/Loc/LcReport/PendingCommercialInvoiceReport',

            })
        .when('/PendingCIReport',
        {
            templateUrl: '/Loc/LcReport/PendingCIReport',

        })
        .when('/LcPendingReport',
        {
            templateUrl: '/Loc/LcReport/LcPendingReport',

        })
        .when('/ExchangeGainLossReport',
        {
            templateUrl: '/Loc/LcReport/ExchangeGainLossReport',

            })
        .when('/MITReport',
            {
                templateUrl: '/Loc/LcReport/MITReport',

            })
        .otherwise({ redirectTo: "/" });
    $locationProvider.html5Mode({
        enabled: true,
        requireBase: false
    });
    $locationProvider.html5Mode(false).hashPrefix('!Loc');
});
