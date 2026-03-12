/// <reference path="../../Scripts/_references.js" />
// DT stands for DocumentTemplate
//var DTModule = angular.module('DTModule', ['ngRoute', 'ngMessages', 'datatables', 'kendo.directives', 'cfp.hotkeys','ngHandsontable']);
var DTModule = angular.module('DTModule', ['ngRoute', 'ngMessages', 'datatables', 'kendo.directives', 'cfp.hotkeys', 'ngHandsontable', 'angularjs-dropdown-multiselect']).run(['$rootScope', function ($rootScope) {
    //var DTModule = angular.module('DTModule', ['ngRoute', 'ngMessages', 'datatables', 'kendo.directives', 'cfp.hotkeys', 'ngHandsontable']).run(['$rootScope', function ($rootScope) {
    $rootScope.CUSTOMER_CODE_DEFVAL_MASTER = "";
    $rootScope.ITEM_CODE_DEFAULTVAL = "";
    $rootScope.M_ACC_CODE_DEFAULTVAL = "";
    $rootScope.Contra_C_ACC_CODE_DEFAULTVAL = "";
    //$rootScope.Charge_ACC_CODE_DEFAULTVAL = "";

    $rootScope.$on('$routeChangeSuccess', function () {
        setTimeout(function () {
            $("#sidebarToggler").trigger("click");
        }, 100)
    });

}]);




//var DTModule = angular.module('DTModule', []);
DTModule.filter('range', function () {
    return function (input, min, max) {
        min = parseInt(min);
        max = parseInt(max);
        for (var i = min; i <= max; i++) {
            input.push(i);
        }
        return input;
    };
});

// added by mahesh ngEnter directive
DTModule.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) { // 13 = Enter key
                scope.$apply(function () {
                    scope.$eval(attrs.ngEnter);
                });

                event.preventDefault();
            }
        });
    };
});

DTModule.config(function ($routeProvider, $locationProvider) {

    $routeProvider.when('/',
        {

            templateUrl: function () {
                return '/DocumentTemplate/Template/Dashboard';
            },
        });

    $routeProvider.when('/Dashboard',
        {
            templateUrl: function () {
                return '/DocumentTemplate/Template/Dashboard';
            },
            //templateUrl: '/DocumentTemplate/Template/Dashboard',
        });


    //route For LogViewer 
    $routeProvider.when('/LogView',
        {
            templateUrl: function () {
                return '/DocumentTemplate/Home/LogView'
            }
        }
    )


    $routeProvider.when('/formtemplate/:formCode',
        {
            templateUrl: function (stateParams) {

                //console.log(stateParams.formCode);

                return '/DocumentTemplate/Template/Formtemplate?formCode=' + stateParams.formCode;
            },
            controller: 'FormTemplateCtrl',
            resolve: {
                FormCode: function ($route) { return $route.current.params.formCode; }
            }
        });

    $routeProvider.when('/formtemplates/:formCode/:orderno', {

        templateUrl: function (stateParams) {

            return '/DocumentTemplate/Template/Formtemplates?formCode=' + stateParams.formCode + '&voucherNo=' + stateParams.orderno;
        },
        controller: 'FormTemplateCtrl',
        resolve:

        {
            FormCode: function ($route) { return $route.current.params.formCode; },
            OrderNo: function ($route) { return $route.current.params.orderno; }

        }

    })

    $routeProvider.when('/formtemplates/:formCode/:orderno/:printstatus', {

        templateUrl: function (stateParams) {

            return '/DocumentTemplate/Template/Formtemplateprint?formCode=' + stateParams.formCode + '&orderNo=' + stateParams.orderno + '&printstatus=' + stateParams.printstatus;
        },
        controller: 'FormTemplateCtrl',
        resolve:

        {
            FormCode: function ($route) { return $route.current.params.formCode; },
            OrderNo: function ($route) { return $route.current.params.orderno; },
            PrintStatus: function ($route) { return $route.current.params.printstatus; }

        }

    })

    $routeProvider.when('/FinanceVoucher/:formcode/:orderno', {

        templateUrl: function (stateParams) {

            return '/DocumentTemplate/FinanceVoucher/FinanceVoucher?formCode=' + stateParams.formcode + '&voucherNo=' + stateParams.orderno;
        },
        controller: 'ContraVoucherCtrl',
        resolve:

        {
            formcode: function ($route) { return $route.current.params.formcode; },
            OrderNo: function ($route) { return $route.current.params.orderno; }

        }

    });

    //$routeProvider.when('/Inventory/:formcode/:orderno', {
    //    templateUrl: function (stateParams) {
    //        return '/DocumentTemplate/Inventory/Inventory?formCode=' + stateParams.formcode + '&voucherNo=' + stateParams.orderno;
    //    },
    //    controller: 'InventoryCtrl',
    //    resolve:
    //    {
    //        formcode: function ($route) { return $route.current.params.formcode; },
    //        OrderNo: function ($route) { return $route.current.params.orderno; }
    //    }
    //});

    $routeProvider.when('/Inventory/:formcode/:orderno', {
        templateUrl: function (stateParams) {
            return '/DocumentTemplate/Inventory/Inventory?formCode=' + stateParams.formcode + '&voucherNo=' + stateParams.orderno;
        },
        controller: 'InventoryCtrl',
        resolve:
        {
            formcode: function ($route) { return $route.current.params.formcode; },
            OrderNo: function ($route) { return $route.current.params.orderno; }
        }
    });


    //Draft
    $routeProvider.when('/Draft/:formcode/:tempCode', {
        templateUrl: function (stateParams) {
            return '/DocumentTemplate/Inventory/Inventory?formCode=' + stateParams.formcode + '&tempCode=' + stateParams.tempCode;
        },

        controller: 'InventoryCtrl',
        resolve:
        {
            formcode: function ($route) { return $route.current.params.formcode; },
            tempCode: function ($route) { return $route.current.params.tempCode; }
        }
    });



    //Inventory draft
    $routeProvider.when('/DraftInventory/:formcode/:tempCode', {
        templateUrl: function (stateParams) {
            return '/DocumentTemplate/Inventory/Inventory?formCode=' + stateParams.formcode + '&tempCode=' + stateParams.tempCode;
        },
        controller: 'InventoryCtrl',
        resolve:
        {
            formcode: function ($route) { return $route.current.params.formcode; },
            tempCode: function ($route) { return $route.current.params.tempCode; }
        }
    });

    //FinanceVoucher draft
    $routeProvider.when('/DraftFinanceVoucher/:formcode/:tempCode', {
        templateUrl: function (stateParams) {
            return '/DocumentTemplate/FinanceVoucher/FinanceVoucher?formCode=' + stateParams.formcode + '&tempCode=' + stateParams.tempCode;
        },
        controller: 'ContraVoucherCtrl',
        resolve:
        {
            formcode: function ($route) { return $route.current.params.formcode; },
            tempCode: function ($route) { return $route.current.params.tempCode; }
        }
    });

    //Sales draft
    $routeProvider.when('/DraftFormtemplate/:formCode/:tempCode', {
        templateUrl: function (stateParams) {
            return '/DocumentTemplate/Template/Formtemplates?formCode=' + stateParams.formCode + '&tempCode=' + stateParams.tempCode;
        },
        controller: 'FormTemplateCtrl',
        resolve:
        {
            formCode: function ($route) { return $route.current.params.formCode; },
            tempCode: function ($route) { return $route.current.params.tempCode; }
        }
    });

    $routeProvider.when('/ProdManagement/:formcode/:orderno', {

        templateUrl: function (stateParams) {

            return '/DocumentTemplate/Production/Production?formCode=' + stateParams.formcode + '&voucherNo=' + stateParams.orderno;
        },
        controller: 'productionCtrl',
        resolve:

        {
            formcode: function ($route) { return $route.current.params.formcode; },
            OrderNo: function ($route) { return $route.current.params.orderno; }

        }

    })
    $routeProvider.when('/ThirdParty',
        {
            templateUrl: '/DocumentTemplate/Home/Upload',
            controller: 'ThirdPartyApiCtrl'
        });
    $routeProvider.when('/ThirdPartyPreferenceSetup',
        {
            templateUrl: '/DocumentTemplate/Home/ThirdPartyPreferenceSetup',
            controller: 'ThirdPartyPreferenceSetupCtrl'
        });
    $routeProvider.when('/PreferenceSetup',
        {
            templateUrl: '/DocumentTemplate/Home/PreferenceSetup',
            controller: 'PreferenceSetupCtrl'
        });


    //Route Provider For price setup 
    $routeProvider.when('/PriceSetup',
        {
            templateUrl: '/DocumentTemplate/Home/PriceSetupCTRL',
            controller: 'PriceSetupCtrl'
        }
    );

    $routeProvider.when('/Setup',
        {
            templateUrl: '/DocumentTemplate/Template/Setup',
            controller: 'TemplateSetupCtrl'
        });
    $routeProvider.when('/SalesOrderDetail',
        {
            templateUrl: '/DocumentTemplate/Template/SalesOrderDetail',
            controller: 'salesorderdetailCtrl'
        });

    $routeProvider.when('/FinanceVoucher',
        {
            templateUrl: '/DocumentTemplate/FinanceVoucher/FinanceVoucher',
            controller: 'ContraVoucherCtrl'
        });

    $routeProvider.when('/Inventory',
        {
            templateUrl: '/DocumentTemplate/Inventory/Inventory',
            controller: 'InventoryCtrl'
        });


    $routeProvider.when('/MenuSplitter/',
        {
            templateUrl: '/DocumentTemplate/Template/MenuSplitter',
            controller: 'SplitterMenuCtrl',

        });

    $routeProvider.when('/MenuSplitter/:module',
        {
            templateUrl: function (stateParams) {

                return '/DocumentTemplate/Template/MenuSplitter?module=' + stateParams.module;
            },
            controller: 'SplitterMenuCtrl',
            resolve: {
                module: function ($route) { return $route.current.params.module; }
            }

        });
    $routeProvider.when('/VerifySplitter/:module',
        {
            templateUrl: function (stateParams) {

                return '/DocumentTemplate/Template/VerifySplitter?module=' + stateParams.module;
            },
            controller: 'DocumentVerificationCtrl',
            resolve: {
                module: function ($route) { return $route.current.params.module; }
            }

        });
    $routeProvider.when('/MenuSplitter/AccountSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/AccountSetup',

        });
    $routeProvider.when('/SchemeSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/SchemeSetup',

        });
    $routeProvider.when('/SchemeTransaction',
        {
            templateUrl: '/DocumentTemplate/Setup/SchemeTransaction',

        });
    $routeProvider.when('/SchemeRun',
        {
            templateUrl: '/DocumentTemplate/Setup/SchemeRun',

        });
    $routeProvider.when('/SchemeRun',
        {
            templateUrl: '/DocumentTemplate/Setup/SchemeRun',

        });
    $routeProvider.when('/SchemeImplement',
        {
            templateUrl: '/DocumentTemplate/Setup/SchemeImplement',

        });
    $routeProvider.when('/AccountSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/AccountSetupIndex',
        });

    $routeProvider.when('/ItemSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/ItemSetupIndex',
        });
    $routeProvider.when('/CustomerSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/CustomerSetup',
        });

    $routeProvider.when('/TermsAndConditionSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/TermsAndConditionSetup',
        });

    $routeProvider.when('/BudgetCenterSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/BudgetCenterSetup',
        });
    $routeProvider.when('/AgentSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/AgentSetup',
        });
    $routeProvider.when('/AreaSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/AreaSetup',
        });
    $routeProvider.when('/WebPrefrence',
        {
            templateUrl: '/DocumentTemplate/Setup/WebErpConfig',
        });
    $routeProvider.when('/LocationSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/LocationSetup',
        });
    //$routeProvider.when('/ProcessSetup',
    //    {
    //        templateUrl: '/DocumentTemplate/Setup/ProcessSetup',
    //    });
    $routeProvider.when('/SubLedgerMgmt',
        {
            templateUrl: '/DocumentTemplate/Setup/SubLedgerMgmt',
            controller: 'subLedgerMgmtCtrl'
        });
    $routeProvider.when('/ResourceSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/ResourceSetup',
        });
    $routeProvider.when('/SupplierSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/SupplierSetupIndex',

        })

    $routeProvider.when('/DealerSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/DealerSetup',

        })
    $routeProvider.when('/TransporterSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/TransporterSetup',
        });
    $routeProvider.when('/RegionalSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/RegionalSetup',
        });
    //BranchSetup
    $routeProvider.when('/BranchSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/BranchSetup',
            controller: 'BranchSetupCtrl'
        });
    //Division Setup
    $routeProvider.when('/DivisionSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/DivisionSetup',
            controller: 'divisionSetupCtrl'
        });
    //CompanySetup
    $routeProvider.when('/CompanySetup',
        {
            templateUrl: '/DocumentTemplate/Setup/CompanySetup',
            controller: 'CompanySetupCtrl'
        });


    //VehicleSetup
    $routeProvider.when('/VehicleSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/VehicleSetup',
            controller: 'vehicleSetupCtrl'
        });

    //PartyTypeSetup
    $routeProvider.when('/PartyTypeSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/PartyTypeSetup',
            controller: 'PartyTypeSetupCtrl'
        });

    //AttributeSetup
    $routeProvider.when('/AttributeSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/AttributeSetup',
            controller: 'attributeSetupTreeCtrl'
        });
    //Miscellaneous Sub Ledger Setup
    $routeProvider.when('/MiscellaneousSubLedgerSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/MiscellaneousSubLedgerSetup',
        });
    //CurrencySetup
    $routeProvider.when('/CurrencySetup',
        {
            templateUrl: '/DocumentTemplate/Setup/CurrencySetup',
            controller: 'currencySetupCtrl'
        });
    //CategorySetup
    $routeProvider.when('/CategorySetup',
        {
            templateUrl: '/DocumentTemplate/Setup/CategorySetup',
            controller: 'categorySetupCtrl'
        });
    //EmployeeSetup
    $routeProvider.when('/EmployeeSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/EmployeeSetup',
            controller: 'EmployeeSetupCtrl'
        });
    //delear setup
    $routeProvider.when('/DealerSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/DealerSetup',
            controller: 'dealerSetupCtrl'
        });
    //sales invoice jewellery


    $routeProvider.when('/SalesInvoiceJewellery',
        {
            templateUrl: '/DocumentTemplate/Setup/SalesInvoiceJewellery',
            controller: 'SalesInvoiceJewelleryCtrl'
        });


    $routeProvider.when('/SalesInvoiceJewellery',

        {
            templateUrl: function (stateParams) {

                console.log(stateParams.formCode);

                return '/DocumentTemplate/Setup/SalesInvoiceJewellery';
            },

            controller: 'SalesInvoiceJewelleryCtrl',
            resolve: {
                FormCode: function ($route) { return "496" }
            }
        });


    //prioritySetup
    $routeProvider.when('/PrioritySetup',
        {
            templateUrl: '/DocumentTemplate/Setup/PrioritySetup',
            controller: 'prioritySetupCtrl'
        });
    $routeProvider.when('/TDSTypeSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/TDSTypeSetup',
            controller: 'TDSTypeSetupCtrl'
        });

    //RejectableSetup
    $routeProvider.when('/RejectableItemSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/RejectableItemSetup',
            controller: 'rejectableItemSetupCtrl'
        });



    //Vehicle Registration
    $routeProvider.when('/VehicleRegistration',
        {
            templateUrl: '/DocumentTemplate/Setup/VehicleRegistration',
            controller: 'vehicleRegistrationCtrl'
        });


    //OrderDispatch
    $routeProvider.when('/OrderDispatchmanagement',
        {
            templateUrl: '/DocumentTemplate/Setup/OrderDispatchmanagement',
            controller: 'OrderDispatchCtrl'
        });

    //GateEntry
    $routeProvider.when('/GateEntry',
        {
            templateUrl: '/DocumentTemplate/Setup/GateEntry',
            controller: 'GateEntryCtrl'
        })

    //PostDateCheque
    $routeProvider.when('/PostDateCheque',
        {
            templateUrl: '/DocumentTemplate/Setup/PostDateCheque',
            controller: 'PostDateChequeCtrl'
        })

    //BankGuarantee
    $routeProvider.when('/BankGuarantee',
        {
            templateUrl: '/DocumentTemplate/Setup/BankGuarantee',
            controller: 'BankGuaranteeCtrl'
        })

    //RateSchedule
    $routeProvider.when('/RateSchedule',
        {
            templateUrl: '/DocumentTemplate/Setup/RateSchedule',
            controller: 'RateScheduleCtrl'
        })

    //DiscountSchedule
    $routeProvider.when('/DiscountSchedule',
        {
            templateUrl: '/DocumentTemplate/Setup/DiscountSchedule',
            controller: 'DiscountScheduleCtrl'
        })

    //Consumption Voucher Generate
    $routeProvider.when('/ConsumptionVoucherGenerate',
        {
            templateUrl: '/DocumentTemplate/Setup/ConsumptionVoucherGenerate',
            controller: 'ConsumptionVoucherGenerateCtrl'
        })

    $routeProvider.when('/PurchaseIndentAdjustment',
        {
            templateUrl: '/DocumentTemplate/PurchaseOrderIndentAdjustment/IndentIndex',
            controller: 'PurchaseIndentAdjustmentCtrl'
        });

    $routeProvider.when('/PurchaseOrderAdjustment',
        {
            templateUrl: '/DocumentTemplate/PurchaseOrderIndentAdjustment/OrderIndex',
            controller: 'PurchaseOrderAdjustmentCtrl'
        });

    //PDC Form - Custom Form
    $routeProvider.when('/postDatedCheque', {
        templateUrl: '/DocumentTemplate/CustomForm/PostDatedCheque',
        controller: 'postDatedChequeCtrl'
    });

    //Column Settings - Custom Form
    $routeProvider.when('/columnSettings', {
        templateUrl: '/DocumentTemplate/CustomForm/ColumnSettingsCshtml',
        controller: 'columnSettingsCtrl'
    });

    //Cash Bank Setups - Custom Form
    $routeProvider.when('/cashBankSetup', {
        templateUrl: '/DocumentTemplate/CustomForm/CashBankSetupHtml',
        controller: 'cashBankSetupCtrl'
    });

    //Bank Reconcilation - Custom Form
    $routeProvider.when('/bankReconcilation', {
        templateUrl: '/DocumentTemplate/CustomForm/BankReconcilationHtml',
        controller: 'bankReconcilationCtrl'
    });

    //Bank Gurantee - Custom Form
    $routeProvider.when('/bankGurantee', {
        templateUrl: '/DocumentTemplate/CustomForm/BankGurantee',
        controller: 'bankGuranteeCtrl'
    });

    $routeProvider.when('/projectManagement', {
        templateUrl: '/DocumentTemplate/CustomForm/ProjectManagement',
        controller: 'ProjectManagementCtrl'
    });


    $routeProvider.when('/SalesOrderAdjustment',
        {
            templateUrl: '/DocumentTemplate/SalesOrderAdjustment/Index',
            controller: 'SalesOrderAdjustmentCtrl'
        });

    $routeProvider.when('/LoadingSlipGenerator',
        {
            templateUrl: '/DocumentTemplate/Home/LoadingSlipGenerator',
            controller: 'LoadingSlipCtrl'
        }
    );

    $routeProvider.when('/LoadingSlipPrinter',
        {
            templateUrl: '/DocumentTemplate/Home/LoadingSlipPrinterPage',
            controller: 'LoadingSlipPrinterCtrl'
        }
    );

    $routeProvider.when('/issueTypeSetup',
        {
            templateUrl: '/DocumentTemplate/Setup/IssueTypeSetup',
            controller: 'issueTypeSetupCtrl'
        }
    );

    $routeProvider.when('/citySetup',
        {
            templateUrl: '/DocumentTemplate/Setup/CitySetup',
            controller: 'citySetupCtrl'
        }
    );

    $routeProvider.when('/unitOfMeasurement',
        {
            templateUrl: '/DocumentTemplate/Setup/UnitOfMeasurement',
            controller: 'unitOfMeasurementCtrl'
        }
    );

    $routeProvider.when('/chargeTypeDefinition',
        {
            templateUrl: '/DocumentTemplate/Setup/ChargeTypeDefinition',
            controller: 'chargeTypeDefinitionCtrl'
        }
    );

    $routeProvider.when('/processSetupBom', {
        templateUrl: '/DocumentTemplate/Setup/ProcessSetupBom',
        controller: 'processSetupBomCtrl'
    });

    $routeProvider.when('/SubLedgerMgmt', {
        templateUrl: '/DocumentTemplate/Setup/SubLedgerMgmt',
        controller: 'subLedgerMgmtCtrl'
    });

    $routeProvider.when('/formSetup/:formCode',
        {

            templateUrl: function (stateParams) {
                return '/DocumentTemplate/Template/FormSetuptemplate?formCode=' + stateParams.formCode;
            },
            controller: 'FormTemplateCtrl',
            resolve: {
                FormCode: function ($route) { return $route.current.params.formCode; }
            }
        });
    $routeProvider.when('/ProdManagement',
        {
            templateUrl: '/DocumentTemplate/Production/Production',
            controller: 'productionCtrl'
        });

    $routeProvider.when('/CostCenterMgmt', {
        templateUrl: '/DocumentTemplate/Setup/CostCenterMgmt',
        controller: 'costCenterMgmtCtrl'
    });
    //InterestCalcForm 
    $routeProvider.when('/InterestCalc', {
        templateUrl: '/DocumentTemplate/InterestCalc/Index',
        controller: 'interestCalcCtrl'
    });
    $routeProvider.when('/InterestLog', {
        templateUrl: '/DocumentTemplate/InterestCalc/InterestLog',
        controller: 'interestCalcCtrl'
    });
    $routeProvider.when('/ProjectSetup', {
        templateUrl: '/DocumentTemplate/Home/ProjectSetup',
        controller: 'Home'
    });

    /*added by mahesh*/
    $routeProvider.when('/ProductionPlanning',
        {
            templateUrl: '/DocumentTemplate/ProductionManagement/ProductionPlanning',
            controller: 'productionPlanningCtrl'
        });

    $routeProvider.when('/IdrDataSync',
        {
            templateUrl: '/DocumentTemplate/IRDDataSync/IdrDataSync',
            controller: 'idrDataSyncCtrl'
        });
    /*end of mahesh*/


    $routeProvider.otherwise({
        redirectTo: function () {
            return '/DocumentTemplate/Template/Dashboard';
        }
    })
    $locationProvider.html5Mode({ enable: true }).hashPrefix('!DT');
});

DTModule.config.$inject = ['$routeProvider', '$locationProvider'];
