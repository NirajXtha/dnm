
/// <reference path="sammy-0.7.4.js" />
(function ($) {


    var app = $.sammy("#operationContent", function () {
        debugger;
        var loader = "<div class=''><img src='/images/ring.gif' style='margin-left: 41%; margin-top: 35%; width: 50px;'  /></div>";
        var opContainer = $("#operationContent");
        var dynamicUrl = window.location.protocol + "//" + window.location.host + "/api/MenuSettings/GetMenuRouting?moduleCode='01'";
        $.ajax({
            url: dynamicUrl,
            error: function () {

            },
            dataType: 'jsonp',
            success: function (data) {
                $.each(data, function (i, e) {
                    this.get(e.VIRTUAL_PATH, function (context) {
                        opContainer.html(loader).load(e.FULL_PATH, mobileAutoClick());
                        return false;
                    });
                })

            },
            type: 'GET'
        });

        $.get("#/", function (data, status) {
            //session check through javascript
            //true
            //if true rediect #dashboard
            //if false login

        });

        this.get("#/dashboard", function (context) {
            $("#operationContent").html(loader).load('sales/saleshome/dashboard', mobileAutoClick());
            return false;
        });
        this.get("#/Sales/FinalSalesReport", function (context) {
            //if session
            $("#operationContent").html(loader).load('Sales/SalesHome/FinalSalesReport', mobileAutoClick());
            return false;

            //else
            //redirect to login
        });
        this.get("#/Sales/Materialize", function (context) {
            //if session
            $("#operationContent").html(loader).load('Sales/SalesHome/MaterializeReport', mobileAutoClick());
            return false;

            //else
            //redirect to login
        });
        this.get("#/Sales/VatRegister", function (context) {
            $("#operationContent").html(loader).load('Sales/SalesHome/VatRegistrationReport', mobileAutoClick());
            return false;
        });

        this.get("#/Sales/SalesRegister", function (context) {
            $("#operationContent").html(loader).load('sales/SalesHome/SalesRegister', mobileAutoClick());
            return false;
        });

        this.get("#/Sales/SalesSummaryCustomerWise", function (context) {
            $("#operationContent").html(loader).load('sales/SalesHome/SalesSummaryCustomerWise', mobileAutoClick());
            return false;
        });
        this.get("#/Purchase/PurchaseVatRegister", function (context) {
            $("#operationContent").html(loader).load('sales/Purchase/PurchaseVatRegister', mobileAutoClick());
            return false;
        });
        this.get("/#/Purchase/PurchaseRegister", function (context) {
            $("#operationContent").html(loader).load('sales/Purchase/PurchaseRegister', mobileAutoClick());
            return false;
        });
        this.get("#/Purchase/PurchaseItemsSummary", function (context) {
            $("#operationContent").html(loader).load('sales/Purchase/PurchaseItemsSummary', mobileAutoClick());
            return false;
        });
        this.get("#/Purchase/PurchaseInvoiceSummary", function (context) {
            $("#operationContent").html(loader).load('sales/Purchase/PurchaseInvoiceSummary', mobileAutoClick());
            return false;
        });
        this.get("#/TrialBalance/Index", function (context) {
            $("#operationContent").html(loader).load('sales/TrialBalanceReport/Index', mobileAutoClick());
            return false;
        });
        this.get("#/Ledger/LedgerDetails", function (context) {
            $("#operationContent").html(loader).load('sales/Ledger/LedgerDetails', mobileAutoClick());
            return false;
        });
        this.get("#/Ledger/LedgerIndex", function (context) {
            $("#operationContent").html(loader).load('sales/Ledger/LedgerIndex', mobileAutoClick());
            return false;
        });

        this.get("#/TrialBalance/TreelistViewTrialBalance", function (context) {
            $("#operationContent").html(loader).load('sales/TrialBalanceReport/TreelistViewTrialBalance', mobileAutoClick());
            return false;
        });
        this.get("#/TrialBalance/TreelistViewPlBalance", function (context) {
            debugger;
            $("#operationContent").html(loader).load('sales/TrialBalanceReport/TreelistViewPlBalance', mobileAutoClick());
            return false;
        });
        this.get("#/Ageing/AgeingReport", function (context) {
            $("#operationContent").html(loader).load('sales/AgeingReport/index', mobileAutoClick());
            return false;
        });
        this.get("#/Calendar/CalendarReport", function (context) {
            $("#operationContent").html(loader).load('sales/CalendarReport/index', mobileAutoClick());
            return false;
        });
        this.get("#/Calendar/weekCalendarReport", function (context) {
            $("#operationContent").html(loader).load('sales/CalendarReport/weekIndex', mobileAutoClick());
            return false;
        });
        this.get("#/Calendar/newweekCalendarReport", function (context) {
            $("#operationContent").html(loader).load('sales/CalendarReport/FinalweekIndex', mobileAutoClick());
            return false;
        });
        this.get("#/Calendar/CalendarReportParam/:id", function (context) {
            var id = this.params['id'];
            $("#operationContent").html(loader).load('sales/CalendarReport/CalendarReportIndex?id=' + id, mobileAutoClick());
            return false;
        });
        this.get("/#/Sales/salesmoniter", function (context) {
            $("#operationContent").html(loader).load('sales/SalesHome/SalesProcessingRegisterContainer', mobileAutoClick());
            return false;
        });


    });
    $(function () {
        app.run();
    });

    function mobileAutoClick() {
        //if ($('#sidebar').parent().hasClass('slimScrollDiv')) {
        //    $("#sidebar-collapse i").trigger('click');
        //}
    }
})(jQuery)
///// <reference path="sammy-0.7.4.js" />
//(function ($) {

//    var app = $.sammy("#operationContent", function () {

//        $.get("#/", function (data, status) {
//            //session check through javascript
//            //true
//            //if true rediect #dashboard
//            //if false login

//        });

//        var loader = "<div class=''><img src='/images/ring.gif' style='margin-left: 41%; margin-top: 35%; width: 50px;'  /></div>";

//        this.get("#/dashboard", function (context) {
//            $("#operationContent").html(loader).load('sales/saleshome/dashboard', mobileAutoClick());
//            return false;
//        });
//        this.get("#/Sales/FinalSalesReport", function (context) {
//            //if session
//            $("#operationContent").html(loader).load('Sales/SalesHome/FinalSalesReport', mobileAutoClick());
//            return false;

//            //else
//            //redirect to login
//        });

//        this.get("#/Sales/VatRegister", function (context) {
//            $("#operationContent").html(loader).load('Sales/SalesHome/VatRegistrationReport', mobileAutoClick());
//            return false;
//        });

//        this.get("#/Sales/SalesRegister", function (context) {
//            $("#operationContent").html(loader).load('sales/SalesHome/SalesRegister', mobileAutoClick());
//            return false;
//        });

//        this.get("#/Sales/SalesSummaryCustomerWise", function (context) {
//            $("#operationContent").html(loader).load('sales/SalesHome/SalesSummaryCustomerWise', mobileAutoClick());
//            return false;
//        });
//        this.get("#/Purchase/PurchaseVatRegister", function (context) {
//            $("#operationContent").html(loader).load('sales/Purchase/PurchaseVatRegister', mobileAutoClick());
//            return false;
//        });
//        this.get("/#/Purchase/PurchaseRegister", function (context) {
//            $("#operationContent").html(loader).load('sales/Purchase/PurchaseRegister', mobileAutoClick());
//            return false;
//        });
//        this.get("#/Purchase/PurchaseItemsSummary", function (context) {
//            $("#operationContent").html(loader).load('sales/Purchase/PurchaseItemsSummary', mobileAutoClick());
//            return false;
//        });
//        this.get("#/Purchase/PurchaseInvoiceSummary", function (context) {
//            $("#operationContent").html(loader).load('sales/Purchase/PurchaseInvoiceSummary', mobileAutoClick());
//            return false;
//        });
//        this.get("#/TrialBalance/Index", function (context) {
//            $("#operationContent").html(loader).load('sales/TrialBalanceReport/Index', mobileAutoClick());
//            return false;
//        });
//        this.get("#/Ledger/LedgerDetails", function (context) {
//            $("#operationContent").html(loader).load('sales/Ledger/LedgerDetails', mobileAutoClick());
//            return false;
//        });
//        this.get("#/Ledger/LedgerIndex", function (context) {
//            $("#operationContent").html(loader).load('sales/Ledger/LedgerIndex', mobileAutoClick());
//            return false;
//        });

//        this.get("#/TrialBalance/TreelistViewTrialBalance", function (context) {
//            $("#operationContent").html(loader).load('sales/TrialBalanceReport/TreelistViewTrialBalance', mobileAutoClick());
//            return false;
//        });
//        this.get("#/Ageing/AgeingReport", function (context) {
//            $("#operationContent").html(loader).load('sales/AgeingReport/index', mobileAutoClick());
//            return false;
//        });
//        this.get("#/Calendar/CalendarReport", function (context) {
//            $("#operationContent").html(loader).load('sales/CalendarReport/index', mobileAutoClick());
//            return false;
//        });
//        this.get("#/Calendar/weekCalendarReport", function (context) {
//            $("#operationContent").html(loader).load('sales/CalendarReport/weekIndex', mobileAutoClick());
//            return false;
//        });
//        this.get("#/Calendar/newweekCalendarReport", function (context) {
//            $("#operationContent").html(loader).load('sales/CalendarReport/FinalweekIndex', mobileAutoClick());
//            return false;
//        });
//        this.get("#/Calendar/CalendarReportParam/:id", function (context) {
//            var id = this.params['id'];
//            $("#operationContent").html(loader).load('sales/CalendarReport/CalendarReportIndex?id=' + id, mobileAutoClick());
//            return false;
//        });
//        this.get("/#/Sales/salesmoniter", function (context) {
//            $("#operationContent").html(loader).load('sales/SalesHome/SalesProcessingRegisterContainer', mobileAutoClick());
//            return false;
//        });
        
       
//    });
//    $(function () {
//        app.run();
//    });

//    function mobileAutoClick() {
//        //if ($('#sidebar').parent().hasClass('slimScrollDiv')) {
//        //    $("#sidebar-collapse i").trigger('click');
//        //}
//    }
//})(jQuery)