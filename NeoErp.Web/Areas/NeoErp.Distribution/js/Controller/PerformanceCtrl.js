distributionModule.controller('performanceCtrl', function ($scope, distributorService, $timeout) {
    debugger;
    $scope.searchOrder = '';
    $scope.searchOrdermax = '';
    $scope.searchOrderLes = '';
    $scope.searchOrderVisit = '';
    $scope.searchOrderVisitPercent = '';
    $scope.searchOrderOutlet = '';
    $scope.date = new Date();

    //var GetASM = distributorService.getPerformanceReport(dateFilter1)

    $scope.ExportToExcel = function (event) {
        debugger;
        var currentIds = event.currentTarget.id;
        switch (currentIds) {

            case "topEffective": 
                var ExportData = _.map($scope.topEffectiveCall, function (x) { return { StaffName: x.EMPLOYEE_EDESC, EffectiveCalls: kendo.format("{0:n}", x.PERCENT_EFFECTIVE_CALLS) + "%", Qty: x.PJP_TOTAL_QUANTITY + x.NPJP_TOTAL_QUANTITY, OrderAmt: x.PJP_TOTAL_AMOUNT + x.NPJP_TOTAL_AMOUNT } })
                alasql('SELECT * INTO XLSX("Top Effective Calls.xlsx",{headers:true}) FROM ?', [ExportData]);
                break;
            case "MaximumOrderList":
                var exportData = _.map($scope.maximumOrderList, function (x) { return { Name: x.EMPLOYEE_EDESC, Quantity: x.PJP_TOTAL_QUANTITY + x.NPJP_TOTAL_QUANTITY, Amount: x.PJP_TOTAL_AMOUNT + x.NPJP_TOTAL_AMOUNT } })
                alasql('SELECT * INTO XLSX("Maximum Order.xlsx",{headers:true}) FROM ?', [exportData]);
                break;
            case "LessEffective":
                debugger;
                var ExportData = _.map($scope.lessEffectiveCall, function (x) { return { StaffName: x.EMPLOYEE_EDESC, EffectiveCalls: kendo.format("{0:n}", x.PERCENT_EFFECTIVE_CALLS) + "%", Quantity: x.PJP_TOTAL_QUANTITY + x.NPJP_TOTAL_QUANTITY, OrderAmt: x.PJP_TOTAL_AMOUNT + x.NPJP_TOTAL_AMOUNT } })
                alasql('SELECT * INTO XLSX("Less Effective Calls.xlsx",{headers:true}) FROM ?', [ExportData]);
                break;
            case "VisitList":
                var ExportData = _.map($scope.maxNotVisited, function (x) { return { StaffName: x.EMPLOYEE_EDESC, Target: x.TARGET, Visited: x.VISITED, NotVisited: x.NOT_VISITED } })
                alasql('SELECT * INTO XLSX("Visit List.xlsx",{headers:true}) FROM ?', [ExportData]);
                break;
            case "VisitPercent":
                var exportData = _.map($scope.VisitedList, function (x) { return { StaffName: x.EMPLOYEE_EDESC, Visit: isNaN(x.VISITED / x.TARGET * 100) ? '0.00' + "%" : kendo.format("{0:n}", x.VISITED / x.TARGET * 100) + "%", EffectiveCalls: kendo.format("{0:n}", x.PERCENT_EFFECTIVE_CALLS) + "%", Amount: x.PJP_TOTAL_AMOUNT + x.NPJP_TOTAL_AMOUNT } });
                alasql('SELECT * INTO XLSX("Visit Percent.xlsx",{headers:true}) FROM ?', [exportData]);
                break;
            case "OutletAdded":
                var ExportData = _.map($scope.addedOutlet, function (x) { return { Area: x.zone, SumTotal: x.amount } });
                alasql('SELECT * INTO XLSX("Added OutLet Report.xlsx",{headers:true}) FROM ?', [ExportData]);
                break;
            case "globalPerformanceReport":
                var exportData = _.map($scope.VisitedList, function (x) { return { StaffName: x.EMPLOYEE_EDESC, Visit: isNaN(x.VISITED / x.TARGET * 100) ? '0.00' + "%" : kendo.format("{0:n}", x.VISITED / x.TARGET * 100) + "%", EffectiveCalls: kendo.format("{0:n}", x.PERCENT_EFFECTIVE_CALLS) + "%", PJPAmt: x.PJP_TOTAL_AMOUNT, NPJPAmt: x.NPJP_TOTAL_AMOUNT, Amount: x.PJP_TOTAL_AMOUNT + x.NPJP_TOTAL_AMOUNT } });
                alasql('SELECT * INTO XLSX("Visit List.xlsx",{headers:true}) FROM ?', [exportData]);
                break;

            default:
                alert("Setup Pending")
        }
    }
    
    $(".loader").show();

    var dateFilter = "";
    var maxListValue = 10;
    var effectiveCallThresholdValue = 75;

    function init() {
        debugger;
        var data = $("#consolidateTreeView").data("kendoTreeView").dataItems();
        //var CompanyCode = [];
        //var BranchCode = [];
        //for (i = 0; i < data.length; i++) {
        //    CompanyCode.push(data[i].branch_Code);

        //    for (x = 0; x < data[i].Items.length; x++) {

        //        BranchCode.push(data[i].Items[x].branch_Code)
        //    };
            
            
        //};

        var dateMonth = $("#ddlDateFilterVoucher").val();
        var bsToDate = $("#ToDateVoucher").val();
        var bsFromDate = $("#FromDateVoucher").val();
       

        if (dateMonth == " loading ... ") {
            var Month = "This Month";
            $scope.SelectedMonths = Month;
        }

        else {
            var Month = dateMonth;
            if (Month == "Custom") {
                $scope.SelectedMonths = bsFromDate +" To "+ bsToDate;
            }   
            else {
                $scope.SelectedMonths = Month;
            }
           
        }        
        var report = $.extend({},true, ReportFilter.filterAdditionalData());
        report.ReportFilters.ToDate = bsToDate;
        report.ReportFilters.FromDate = bsFromDate;

        
        var dateFilter= JSON.stringify(report); 
        

        var report1 = $.extend({}, true, ReportFilter.filterAdditionalData());
        report1.ReportFilters.ToDate = moment(bsToDate).format("DD-MMM-YYYY");
        report1.ReportFilters.FromDate = moment(bsFromDate).format("DD-MMM-YYYY");
        var dateFilter1 = JSON.stringify(report1);


        var Getdata = distributorService.getPerformanceReport(dateFilter1)
        Getdata.then(function (response) {
            debugger;
            $scope.totalSummary = [];
            var uniqGroup = _.reject(_.uniq(_.pluck(response.data, "GROUP_EDESC")), _.isNull);
            _.each(uniqGroup, function (x) {
                var total = _.filter(response.data, function (y) {
                    return y.GROUP_EDESC == x;
                });
                //for effectiveCall
                var avgEffectiveCall = _.pluck(total, "PERCENT_EFFECTIVE_CALLS");
                avgEffectiveCall = kendo.format("{0:n}", _.sum(avgEffectiveCall) / _.size(avgEffectiveCall)) + " %";
                //for not visited
                var notVisited = _.sum(_.pluck(total, "NOT_VISITED"));
                //for target  
                var target = _.sum(_.pluck(total, "TARGET"));
                //for order
                var order = _.sum(_.pluck(total, "PJP_TOTAL_QUANTITY")) + _.sum(_.pluck(total, "NPJP_TOTAL_QUANTITY"));
                var amount = _.sum(_.pluck(total, "PJP_TOTAL_AMOUNT")) + _.sum(_.pluck(total, "NPJP_TOTAL_AMOUNT"));
                var visited = _.sum(_.pluck(total, "VISITED"));
                $scope.totalSummary.push({
                    zone: x,
                    effectiveCall: avgEffectiveCall,
                    notVisited: notVisited,
                    target: target,
                    order: order,
                    amount: amount,
                    visited: visited,
                });

            })           
            //for maximum orderlist

            //$scope.maximumOrderList = _.filter(response.data).sortBy("TOTAL_QUANTITY").reverse());
            //$scope.maximumOrderList = _.sortBy(response.data, function (x) { return x.PJP_TOTAL_QUANTITY }).reverse();
            $scope.maximumOrderList = _.sortBy(_.sortBy(response.data, "EMPLOYEE_EDESC").reverse(), function (x) { return x.PJP_TOTAL_QUANTITY + x.NPJP_TOTAL_QUANTITY }).reverse();
            $scope.maxNotVisited = _.first(_(response.data).sortBy("NOT_VISITED").reverse(), maxListValue);
            $scope.VisitedList = _.chain(response.data).sortBy('EMPLOYEE_EDESC').reverse().sortBy("VISITED").reverse().value();
            $scope.addedOutlet = _.map($scope.totalSummary, function (x) {
                return { amount: x.amount, zone: x.zone };
            })           
            //$scope.topEffectiveCall = _.first(_(_.filter(response.data, function (x) {
            //    return x.PERCENT_EFFECTIVE_CALLS >= effectiveCallThresholdValue;
            //})).sortBy("PERCENT_EFFECTIVE_CALLS").reverse(), maxListValue);

            $scope.topEffectiveCall = _.sortBy(_.filter(response.data, function (x) { return x.PERCENT_EFFECTIVE_CALLS > effectiveCallThresholdValue }), function (y) { return y.PERCENT_EFFECTIVE_CALLS }).reverse();
            //$scope.lessEffectiveCall = _.first(_(_.filter(response.data, function (x) {
            //    return x.PERCENT_EFFECTIVE_CALLS < 75;
            //})).sortBy("PERCENT_EFFECTIVE_CALLS"), maxListValue);           
            //$scope.lessEffectiveCall = _.sortBy(_.filter(response.data, function (x) {
            //    return x.PERCENT_EFFECTIVE_CALLS < effectiveCallThresholdValue
            //}), function (y) { return -y.PERCENT_EFFECTIVE_CALLS, -y.EMPLOYEE_EDESC }).reverse();

            var sumData = _.chain(response.data).sortBy('EMPLOYEE_EDESC').reverse().sortBy("VISITED").reverse().value();
            var pjpTotal = 0;
            var npjpTotal = 0;
            $.each(sumData, function (key, value) {
                debugger;
                var TempValue = value.PJP_TOTAL_AMOUNT;
                pjpTotal += TempValue;
                var npjpValue = value.NPJP_TOTAL_AMOUNT;
                npjpTotal += npjpValue;
               
            });

            $scope.TotalPJP = pjpTotal;
            $scope.TotalNPJP = npjpTotal;
           


            $scope.lessEffectiveCall = _.chain(_.filter(response.data, function (x) {
                
                return x.PERCENT_EFFECTIVE_CALLS < effectiveCallThresholdValue
            })).sortBy('EMPLOYEE_EDESC').reverse().sortBy('PERCENT_EFFECTIVE_CALLS').reverse().value();
            $(".loader").hide();
        });     
    }
      
    //init();
    $("#applydp").click(function () {
        $(".loader").show();
        init();
    });
    $("#applyConsolidate").click(function () {
        $(".loader").show();
        init();
    });
    $("#loadAdvancedFilters").click(function () {
        $(".loader").show();
        init();
    });
    
    DateFilter.init(function () {
        consolidate.init(function () {
            $(".loader").show();
            init();
        })
       
    });
    
});



distributionModule.controller('performanceGlobalCtrl', function ($scope, distributorService, $timeout) {
    debugger;
    $scope.searchOrder = '';
    $scope.searchOrdermax = '';
    $scope.searchOrderLes = '';
    $scope.searchOrderVisit = '';
    $scope.searchOrderVisitPercent = '';
    $scope.searchOrderOutlet = '';
    $scope.date = new Date();

    //var GetASM = distributorService.getPerformanceReport(dateFilter1)

    $scope.ExportToExcel = function (event) {
        debugger;
        var currentIds = event.currentTarget.id;
        switch (currentIds) {

            case "topEffective":
                var ExportData = _.map($scope.topEffectiveCall, function (x) { return { StaffName: x.EMPLOYEE_EDESC, EffectiveCalls: kendo.format("{0:n}", x.PERCENT_EFFECTIVE_CALLS) + "%", Qty: x.PJP_TOTAL_QUANTITY + x.NPJP_TOTAL_QUANTITY, OrderAmt: x.PJP_TOTAL_AMOUNT + x.NPJP_TOTAL_AMOUNT } })
                alasql('SELECT * INTO XLSX("Top Effective Calls.xlsx",{headers:true}) FROM ?', [ExportData]);
                break;
            case "MaximumOrderList":
                var exportData = _.map($scope.maximumOrderList, function (x) { return { Name: x.EMPLOYEE_EDESC, Quantity: x.PJP_TOTAL_QUANTITY + x.NPJP_TOTAL_QUANTITY, Amount: x.PJP_TOTAL_AMOUNT + x.NPJP_TOTAL_AMOUNT } })
                alasql('SELECT * INTO XLSX("Maximum Order.xlsx",{headers:true}) FROM ?', [exportData]);
                break;
            case "LessEffective":
                debugger;
                var ExportData = _.map($scope.lessEffectiveCall, function (x) { return { StaffName: x.EMPLOYEE_EDESC, EffectiveCalls: kendo.format("{0:n}", x.PERCENT_EFFECTIVE_CALLS) + "%", Quantity: x.PJP_TOTAL_QUANTITY + x.NPJP_TOTAL_QUANTITY, OrderAmt: x.PJP_TOTAL_AMOUNT + x.NPJP_TOTAL_AMOUNT } })
                alasql('SELECT * INTO XLSX("Less Effective Calls.xlsx",{headers:true}) FROM ?', [ExportData]);
                break;
            case "VisitList":
                var ExportData = _.map($scope.maxNotVisited, function (x) { return { StaffName: x.EMPLOYEE_EDESC, Target: x.TARGET, Visited: x.VISITED, NotVisited: x.NOT_VISITED } })
                alasql('SELECT * INTO XLSX("Visit List.xlsx",{headers:true}) FROM ?', [ExportData]);
                break;
            case "VisitPercent":
                var exportData = _.map($scope.VisitedList, function (x) { return { StaffName: x.EMPLOYEE_EDESC, Visit: isNaN(x.VISITED / x.TARGET * 100) ? '0.00' + "%" : kendo.format("{0:n}", x.VISITED / x.TARGET * 100) + "%", EffectiveCalls: kendo.format("{0:n}", x.PERCENT_EFFECTIVE_CALLS) + "%", Amount: x.PJP_TOTAL_AMOUNT + x.NPJP_TOTAL_AMOUNT } });
                alasql('SELECT * INTO XLSX("Visit Percent.xlsx",{headers:true}) FROM ?', [exportData]);
                break;
            case "OutletAdded":
                var ExportData = _.map($scope.addedOutlet, function (x) { return { Area: x.zone, SumTotal: x.amount } });
                alasql('SELECT * INTO XLSX("Added OutLet Report.xlsx",{headers:true}) FROM ?', [ExportData]);
                break;
            case "globalPerformanceReport":
                var exportData = _.map($scope.VisitedList, function (x) { return { StaffName: x.EMPLOYEE_EDESC, Visit: isNaN(x.VISITED / x.TARGET * 100) ? '0.00' + "%" : kendo.format("{0:n}", x.VISITED / x.TARGET * 100) + "%", EffectiveCalls: kendo.format("{0:n}", x.PERCENT_EFFECTIVE_CALLS) + "%", PJPAmt: x.PJP_TOTAL_AMOUNT, NPJPAmt: x.NPJP_TOTAL_AMOUNT, Amount: x.PJP_TOTAL_AMOUNT + x.NPJP_TOTAL_AMOUNT } });
                alasql('SELECT * INTO XLSX("Visit List.xlsx",{headers:true}) FROM ?', [exportData]);
                break;

            default:
                alert("Setup Pending")
        }
    }

    $(".loader").show();

    var dateFilter = "";
    var maxListValue = 10;
    var effectiveCallThresholdValue = 75;

    function init() {
        debugger;
        var data = $("#consolidateTreeView").data("kendoTreeView").dataItems();
        //var CompanyCode = [];
        //var BranchCode = [];
        //for (i = 0; i < data.length; i++) {
        //    CompanyCode.push(data[i].branch_Code);

        //    for (x = 0; x < data[i].Items.length; x++) {

        //        BranchCode.push(data[i].Items[x].branch_Code)
        //    };


        //};

        var dateMonth = $("#ddlDateFilterVoucher").val();
        var bsToDate = $("#ToDateVoucher").val();
        var bsFromDate = $("#FromDateVoucher").val();


        if (dateMonth == " loading ... ") {
            var Month = "This Month";
            $scope.SelectedMonths = Month;
        }

        else {
            var Month = dateMonth;
            if (Month == "Custom") {
                $scope.SelectedMonths = bsFromDate + " To " + bsToDate;
            }
            else {
                $scope.SelectedMonths = Month;
            }

        }
        var report = $.extend({}, true, ReportFilter.filterAdditionalData());
        report.ReportFilters.ToDate = bsToDate;
        report.ReportFilters.FromDate = bsFromDate;


        var dateFilter = JSON.stringify(report);


        var report1 = $.extend({}, true, ReportFilter.filterAdditionalData());
        report1.ReportFilters.ToDate = moment(bsToDate).format("YYYY-MMM-DD");
        report1.ReportFilters.FromDate = moment(bsFromDate).format("YYYY-MMM-DD");
        var dateFilter1 = JSON.stringify(report1);

        var GetInidiviualAsm = distributorService.getPresentASMBeat(dateFilter1)
        GetInidiviualAsm.then(function (response) {
            debugger;

            var data = response.data;
            $scope.StaffName = data[0]["EMPLOYEE_EDESC"];
            $scope.AttendanceTime = data[0]["ATN_TIME"];
            $scope.EODTime = data[0]["EOD_TIME"];
            $scope.TotalTarget = data[0]["TARGET"];
            $scope.TargetVisited = data[0]["VISITED"];
            $scope.AchievedTarget = data[0]["PERCENT_ACHIEVED_TARGET"];
            $scope.ProductiveCall = data[0]["PERCENT_PRODUCTIVE_CALLS"];
            $scope.EffectiveCall = data[0]["PERCENT_EFFECTIVE_CALLS"];
            $scope.OutletMade = data[0]["OUTLET_ADDED"];
        });
        var GetAsm = distributorService.getASMBeat(dateFilter1)
        GetAsm.then(function (response) {
            debugger;
           $scope.mondata=response.data;
            angular.forEach(mondata, function (value, key) {
                $scope.workhrs = workhrs + value.WORKING_HOURS;
            });
            $(".loader").hide();
        });
    }

    //init();
    $("#applydp").click(function () {
        $(".loader").show();
        init();
    });
    $("#applyConsolidate").click(function () {
        $(".loader").show();
        init();
    });
    $("#loadAdvancedFilters").click(function () {
        $(".loader").show();
        init();
    });

    DateFilter.init(function () {
        consolidate.init(function () {
            $(".loader").show();
            init();
        })

    });

});


