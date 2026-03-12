var distributionModule = angular.module('distributionModule', ["ngRoute", "kendo.directives", "ngMessages", "datatables", 'ui.calendar']);
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
    $routeProvider.when('/TargetSetup',
        {
            templateUrl: '/distribution/Report/TargetSetup'
        });
    $routeProvider.when('/VisitSummaryAll',
        {
            templateUrl: '/distribution/Home/VisitSummaryReportAll'
        });
    $routeProvider.when('/QuestionnaireReport',
        {
            templateUrl: '/distribution/Report/QuestionaireReport'
        });

    $routeProvider.when('/CalendarRouteReport',
        {
            templateUrl: '/distribution/Report/CalendarRouteReport',
            controller: 'calendarReport'
        });

    $routeProvider.when('/collectionreport',
        {
            templateUrl: '/distribution/Report/CollectionsReport'
        });

    $routeProvider.when('/performanceReport',
        {
            templateUrl: '/distribution/Report/performanceReport'
        });
    $routeProvider.when('/performanceReport_Global',
        {
            templateUrl: '/distribution/Report/performanceReport_Global'
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
    $routeProvider.when('/Purchase/POEdit',
        {
            templateUrl: '/Distribution/Purchase/EditPO',
            controller: 'POEditCtrl'
        });
    $routeProvider.when('/VehicleApprovalSetup',
        {
            templateUrl: '/Distribution/Purchase/VehicleApprovalSetup'
        });
    $routeProvider.when('/ClaimReport',
        {
            templateUrl: '/distribution/Report/ClaimReport'
        });

    //
    $routeProvider.when('/Purchase/DistSalesReturn',
        {
            templateUrl: '/Distribution/Purchase/DistSalesReturn',
           // controller: 'DistSalesReturnCtrl'
        });
    $routeProvider.when('/PurchaseReport',
       {
           templateUrl: '/distribution/Distributor/ResellerPurchaseOrder'
       });
    $routeProvider.when('/TimeSummaryReport',
        {
            templateUrl: '/distribution/Report/VisitsummaryTimeReport'
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
    $routeProvider.when('/Purchase/DealerSalesOrder',
        {
            templateUrl: '/distribution/Purchase/DealerSalesOrder',
        });
    $routeProvider.when('/UserSetup',
        {
            templateUrl: '/distribution/Home/UserSetup'
        });
    $routeProvider.when('/FreeQuantitySetup',
        {
            templateUrl: '/distribution/Setup/FreeQty',
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
    $routeProvider.when('/FarmSetup',
        {
            templateUrl: '/distribution/Setup/FarmSetup',
            controller: 'FarmCtrl'
        });
    $routeProvider.when('/FarmerProblemRecommendation',
        {
            templateUrl: '/distribution/Report/FarmerProblemRecommendation'
        });
    $routeProvider.when('/ResellerSetup/:status',
        {
            templateUrl: function (stateParams) {

                return '/distribution/Setup/ResellerSetup?status=' + stateParams.status;
            },
            controller: 'ResellerCtrl',
            resolve: {
                status: function ($route) { return $route.current.params.status; }
            }
        });
    
    $routeProvider.when('/QuestionSetup',
        {
            templateUrl: '/distribution/Setup/QuestionSetup',
            controller: 'QuestionCtrl'
        });
    $routeProvider.when('/webQuestionnaireReport',
        {
           
            templateUrl: '/distribution/Report/webQuestionnaireReport',
            controller: 'QuestionListCtrl'
        });
    $routeProvider.when('/SurveySetup',
        {
            templateUrl: '/distribution/Setup/SurveySetup',
        });
    $routeProvider.when('/BrandingActivity',
        {
            templateUrl: '/distribution/Branding/BrandingActivity',
           // controller: 'QuestionCtrl'
        });
    $routeProvider.when('/ContractSetup',
        {
            templateUrl: '/distribution/Branding/ContractSetup',
            // controller: 'QuestionCtrl'
        });
    $routeProvider.when('/EventContractSetup',
        {
            templateUrl: '/distribution/Branding/EventContractSetup',
            // controller: 'QuestionCtrl'
        });
    $routeProvider.when('/OtherContractSetup',
        {
            templateUrl: '/distribution/Branding/OtherContractSetup',
            // controller: 'QuestionCtrl'
        });
    $routeProvider.when('/DistQueryBuilder',
        {
            templateUrl: '/distribution/Setup/DistQueryBuilder',
            // controller: 'QuestionCtrl'
        });
    
    $routeProvider.when('/SchemeReport',
        {
            templateUrl: '/distribution/Branding/SchemeReport',
            // controller: 'QuestionCtrl'
        });
    $routeProvider.when('/ContractSummary',
        {
            templateUrl: '/distribution/Branding/ContractSummary',
            // controller: 'QuestionCtrl'
        });
    $routeProvider.when('/ContractAnsReport',
    {
        templateUrl: '/distribution/Branding/ContractReport',
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
    $routeProvider.when('/BrandingRouteSetup',
        {
            templateUrl: '/distribution/Setup/RouteSetupBranding',

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
    $routeProvider.when('/ClosingStockSetup',
        {
            templateUrl: '/distribution/Setup/ClosingStock'
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
    $routeProvider.when('/OtherEntitySetup', {
        templateUrl: '/distribution/Setup/OtherEntitySetup'
    });

    $routeProvider.when('/EOD', {
        templateUrl: '/distribution/Report/EOD'
    });

    $routeProvider.when('/AttendanceReport',
        {
            templateUrl: '/distribution/Report/AttendanceReport'
        });
    $routeProvider.when('/MRVisitMap',
        {
            templateUrl: '/distribution/Report/MRMap'
        });
    $routeProvider.when('/MRVisitTrackingMap',
        {
            templateUrl: '/distribution/Report/MRVisitTrackingMap'
        });
    $routeProvider.when('/EmployeeWisePerformance',
        {
            templateUrl: '/distribution/Report/EmployeeWisePerformanceNew'
        });
    $routeProvider.when('/EmployeeDetailReport',
        {
            templateUrl: '/distribution/Report/SalesPersonDetailReport'
        });
    $routeProvider.when('/ItemsMinMax',
        {
            templateUrl: '/distribution/Report/ItemsMinMax'
        });
    $routeProvider.when('/DailyActivityReport',
        {
            templateUrl: '/distribution/Report/DailyActivityReport'
        });    
    $routeProvider.when('/ClosingStock',
        {
            templateUrl: '/distribution/Distributor/ClosingReport'
        });
    $routeProvider.when('/OpeningStockSetup',
        {
            templateUrl: '/distribution/Setup/OpeningStockSetup'
        });
    $routeProvider.when('/SalesRegister',
        {
            templateUrl: '/distribution/Report/SalesRegister'
        });
    $routeProvider.when('/OutletClosingReport',
        {
            templateUrl: '/distribution/Report/OutletClosingStock'
        });
    $routeProvider.when('/ResellerClosingStockReport',
        {
            templateUrl: '/distribution/Report/ResellerClosingStockReport'
        });
    $routeProvider.when('/DeviceRegistration',
        {
            templateUrl: '/distribution/Setup/MobileRegistration'
        });
    $routeProvider.when('/VisitImageGallery',
        {
            templateUrl: '/distribution/Report/VisitImageGallery'
        });
    $routeProvider.when('/VisitImageGalleryCondition',
        {
            templateUrl: '/distribution/Report/VisitImageGalleryCondition'
        });
    $routeProvider.when('/SchemeReport',
        {
            templateUrl: '/distribution/Report/SchemeReport'
        });
    $routeProvider.when('/SchemeWiseReport',
        {
            templateUrl: '/distribution/Report/SchemeWiseReport'
        });
    $routeProvider.when('/SchemeChecker',
        {
            templateUrl: '/distribution/Report/SchemeChecker'
        });
    $routeProvider.when('/Notifications',
        {
            templateUrl: '/distribution/Setup/NotificationSetup'
        });
    $routeProvider.when('/CompItemSetup',
        {
            templateUrl: '/distribution/Setup/CompItemSetup',
        });
    $routeProvider.when('/CompItemMap',
        {
            templateUrl: '/distribution/Setup/CompItemMap',
        });
    $routeProvider.when('/CompFieldSetup',
        {
            templateUrl: '/distribution/Setup/CompFieldSetup',
        });
    $routeProvider.when('/GroupMapping',
        {
            templateUrl: '/distribution/Setup/GroupMapping',
        });
    $routeProvider.when('/DistUserMapping',
        {
            templateUrl: '/distribution/Setup/DistUserMapping',
        });
    $routeProvider.when('/CompetitorReport',
        {
            templateUrl: '/distribution/Report/CompItemReport',
        });
    $routeProvider.when('/CompetitorMonthlyReport',
        {
            templateUrl: '/distribution/Report/CompItemReportMonthly',
        });
    $routeProvider.when('/DistanceReport',
        {
            templateUrl: '/distribution/Report/SPDistanceReport',
        });
    $routeProvider.when('/RouteReport',
        {
            templateUrl: '/distribution/Report/SPRouteReport',
        });
    $routeProvider.when('/ResellerDetailReport',
        {
            templateUrl: '/distribution/Report/ResellerDetailReport',
        });
    $routeProvider.when('/QuickSetup',
        {
            templateUrl: '/distribution/Setup/QuickSetup',
        });
    $routeProvider.when('/ItemSetup',
        {
            templateUrl: '/distribution/Setup/ItemSetup',
        });
    $routeProvider.when('/OutletItemRateSetup',
        {
            templateUrl: '/distribution/Setup/OutletItemRate',
        });
    $routeProvider.when('/EmployeeSetup',
        {
            templateUrl: '/distribution/Setup/EmployeeSetup',
        });
    $routeProvider.when('/SchemeSetup',
        {
            templateUrl: '/distribution/Setup/SchemeSetup'
        });
    $routeProvider.when('/SchemeApprovalSetup',
        {
            templateUrl: '/distribution/Setup/SchemeApprovalSetup'
        });
    $routeProvider.when('/DeviceLog',
        {
            templateUrl: '/distribution/Report/DeviceLog', 
        });
    $routeProvider.when('/SurveyReport',
        {
            templateUrl: '/distribution/Report/SurveyReport'
        });
    $routeProvider.when('/SurveyReportAata',
        {
            templateUrl: '/distribution/Report/SurveyReportAata'
        });
    $routeProvider.when('/SurveyReportAataTabular',
        {
            templateUrl: '/distribution/Report/SurveyReportAataTabular'
        });
    $routeProvider.when('/SummaryReport',
        {
            templateUrl: '/distribution/Report/SummaryReport'
        });
    $routeProvider.when('/EmpAttendanceReport',
        {
            templateUrl: '/distribution/Report/EmpAttendanceCalendarReport'
        });
    $routeProvider.when('/SurveyReportTabular',
        {
            templateUrl: '/distribution/Report/SurveyReportTabularDynamic'
        });
    $routeProvider.when('/SurveyReportJGI',
        {
            templateUrl: '/distribution/Report/SurveyReportTabularJGI'
        });
    $routeProvider.when('/EmployeeActivityReport',
        {
            templateUrl: '/distribution/Report/EmployeeActivityReport'
        });
    $routeProvider.when('/DistResellerStockReport',
        {
            templateUrl: '/distribution/Report/DistResellerStockReport'
        });
    $routeProvider.when('/DistResellerBrandItemStockReport',
        {
            templateUrl: '/distribution/Report/DistResellerBrandItemStockReport'
        });
    $routeProvider.when('/DistDistributorBrandItemStockReport',
        {
            templateUrl: '/distribution/Report/DistDistributorBrandItemStockReport'
        });
    $routeProvider.when('/WebSurveyReport',
        {
            templateUrl: '/distribution/Report/WebSurveyReportDynamic'
        });
    // Branding 
    $routeProvider.when('/BrandingSurveyReport',
        {
            templateUrl: '/distribution/Report/SurveyReportBrandingTabular'
        });
    $routeProvider.when('/MerchandisingStockReport',
        {
            templateUrl: '/distribution/Report/MerchandisingStockReport'
        });
    $routeProvider.when('/BrandingVisitSummary',
        {
            templateUrl: '/distribution/Home/VisitSummaryBrandingReport'
        });
    $routeProvider.when('/BrandingAttendanceReport',
        {
            templateUrl: '/distribution/Report/BrandingAttendanceReport'
        });
    $routeProvider.when('/BrandingEmployeeDetailReport',
        {
            templateUrl: '/distribution/Report/BrandingSalesPersonDetailReport'
        });
    $routeProvider.when('/BrandingVisitImageGallery',
        {
            templateUrl: '/distribution/Report/BrandingVisitImageGallery'
        });
    $routeProvider.when('/IndisualEmployee',
        {
            templateUrl: '/distribution/Report/SalesPersonDetailIndivisual'
        });
    $routeProvider.when('/AttendanceSummary',
        {
            templateUrl: '/distribution/Report/AttendanceSummary'
        });
    $routeProvider.when('/EmployeeWisePerformancetest',
        {
            templateUrl: '/distribution/Report/EmployeeWisePerformance'
        });
    $routeProvider.when('/EmployeeWisePerformancetest1',
        {
            templateUrl: '/distribution/Report/EmployeePerformanceNewDetail'
        });
    $routeProvider.when('/StockSummary',
        {
            templateUrl: '/distribution/Report/StockSummary'
        });

    $routeProvider.when('/LedgerImport',
        {
            templateUrl: '/Distribution/Report/LedgerImport'
        });

    $routeProvider.when('/DeliveryReport',
        {
            templateUrl: '/Distribution/Report/DeliveryReport'
        });

    $routeProvider.when('/LedgerAccess',
        {
            templateUrl: '/Distribution/Report/ledgerAccess'
        });

    $routeProvider.when('/UserAccessSetup',
        {
            templateUrl: '/Distribution/Setup/UserAccessSetup',
        });

    /// user scheme
    $routeProvider.when('/SchemeUser',
        {
            templateUrl: '/distribution/UserScheme/SchemeUser',
        });
    $routeProvider.when('/SchemePoints',
        {
            templateUrl: '/distribution/SchemePoints/Index',
        });
    $routeProvider.when('/ClaimHistory',
        {
            templateUrl: '/distribution/SchemePoints/ClaimHistory',
        });


    $routeProvider.when('/QrGenerator',
        {
            templateUrl: '/distribution/UserScheme/QrGenerator',
        });



    $routeProvider.when('/CreateOffers',
        {
            templateUrl: '/distribution/SchemePoints/CreateOffers',
        });
    $routeProvider.when('/UpdateQr',
        {
            templateUrl: '/distribution/UserScheme/UpdateQr',
        });
    /// User scheme end

    $routeProvider.otherwise({ redirectTo: '/' });
    $locationProvider.html5Mode({ enable: false }).hashPrefix('!Distribution');
});



