

var reportFilter = ReportFilter.filterAdditionalData();
var _oldData = '',
    message = '',
    Data = '';
function ChartFilter(actionPageId, createChartMethod, IsDateChanged) {
    //get Chart Filter


    //for customer Filter
    var customerIds = [];
    var customerName = [];
    var customer = "";
    // Data === '' ? actionPageId : Data;
    try {
        var multiselect = $("#customerMultiSelect_" + actionPageId).data("kendoMultiSelect");
        var dataItem = multiselect.dataItems();
        reportFilter.ReportFilters.CustomerFilter = multiselect.value();
        customerIds = multiselect.value().join(',');
        $.each(dataItem, function (i, item) {
            customerName[i] = item.CustomerName;
        });
        customer = customerName.join(",<br />");
        Data += customer;
    } catch (e) { }

    //for location Filter
    var locationCodes = [];
    var locationName = [];
    var location = "";
    // Data === '' ? actionPageCode : Data;
    try {
        var multiselect = $("#locationMultiSelect_" + actionPageId).data("kendoMultiSelect");
        var dataItem = multiselect.dataItems();
        reportFilter.ReportFilters.LocationFilter = multiselect.value();
        locationCodes = multiselect.value().join(',');
        $.each(dataItem, function (i, item) {
            locationName[i] = item.LocationName;
        });
        location = locationName.join(",<br />");
        Data += location;
    } catch (e) { }


    //for Product Filter
    var productIds = [];
    var productName = [];
    var product = "";
    try {
        var multiselect = $("#productMultiselect_" + actionPageId).data("kendoMultiSelect");
        var dataItem = multiselect.dataItems();
        reportFilter.ReportFilters.ProductFilter = multiselect.value();
        productIds = multiselect.value().join(',');
        $.each(dataItem, function (i, item) {
            productName[i] = item.ItemName;
        });
        product = productName.join(",<br />");
        Data += product;
    } catch (e) { }


    //for category Filter
    var categoryIds = [];
    var categoryName = [];
    var category = "";
    try {
        var multiselect = $("#categoryMultiSelect_" + actionPageId).data("kendoMultiSelect");
        var dataItem = multiselect.dataItems();
        reportFilter.ReportFilters.CategoryFilter = multiselect.value();
        categoryIds = multiselect.value().join(',');
        $.each(dataItem, function (i, item) {
            categoryName[i] = item.CategoryName;
        });
        category = categoryName.join(",<br />");
        Data += category;
    } catch (e) { }




    //for company filter1
    var companyIds = [];
    var companyCodes = "";
    try {
        var companyList = $("#consolidateTreeView_" + actionPageId).data("kendoTreeView");
        if ($("#companyMultiSelect_" + actionPageId).length > 0) {
            var companyIds = [];
            var multiselect = $("#companyMultiSelect_" + actionPageId).data("kendoMultiSelect");
            companyIds = multiselect.value();
            //return companyIds;
        } else if (companyList != undefined) {
            var items = getCheckedItems(companyList);
            var companyCodeArray = [];

            if (items.length != 0) {
                for (var i = 0; i < items.length; i++) {
                    if (items[i].branch_Code.indexOf('.') == -1) {
                        companyCodeArray[i] = items[i].branch_Code;
                    } else {
                        companyCodeArray[i] = items[i].branch_Code.substring(0, items[i].branch_Code.indexOf('.'));
                    }
                }
            }
            companyIds = _.uniq(companyCodeArray);
        } else {
            companyIds = [];
        }

        reportFilter.ReportFilters.CompanyFilter = companyIds;
        reportFilterExpense = reportFilter.ReportFilters.CompanyFilter;
    } catch (e) { }




    //for branchTree filter
    var branchIds = [];
    var branchName = [];
    var branchNamePreItemCode = [];
    var branchCodes = "";
    var branch = "";
    try {
        var kendoData = $("#consolidateTreeView_" + actionPageId).data("kendoTreeView");
        
        var items = getCheckedItems(kendoData);
        var branchCodeArray = [];
        if (items.length != 0) {
            var j = 0;
            for (var i = 0; i < items.length; i++) {
                if (items[i].branch_Code.indexOf('.') != -1) {
                    branchCodeArray[j] = items[i].branch_Code;
                    branchName[j] = items[i].branch_edesc;
                    branchNamePreItemCode[j] = items[i].branch_edesc + items[i].pre_branch_code;
                    j++;
                }
            }
        }
        branchName = _.uniq(branchName);
        if (branchCodeArray.length == 1 && branchCodeArray[0].indexOf('.') > -1) {
            branchCodes = branchCodeArray.join(',');
            branch = branchName.join(",<br />");
        } else if (branchCodeArray.length > 1) {
            branchCodes = branchCodeArray.join(',');
            branch = branchName.join(",<br />");
        } else {
            branchCodes = "";
            branch = "";
        }
        Data += branchNamePreItemCode.join(",<br />");
        reportFilter.ReportFilters.BranchFilter = branchCodeArray;
        reportFilterExpense = reportFilter.ReportFilters.BranchFilter;
    } catch (e) { }



    //for party Type Filter
    var partyTypeIds = [];
    var partyTypeName = [];
    var partyType = "";
    try {
        
        var multiselect = $("#partyTypeMultiSelect_" + actionPageId).data("kendoMultiSelect");
        var dataItem = multiselect.dataItems();
        reportFilter.ReportFilters.PartyTypeFilter = multiselect.value();
        partyTypeIds = multiselect.value().join(",");
        $.each(dataItem, function (i, item) {
            partyTypeName[i] = item.PartyTypeName; //update this
        });
        partyType = partyTypeName.join(",<br />");
        Data += partyType;
    } catch (e) { }

    //for Area Filter
    
    var areaIds = [];
    var areaName = [];
    var area = "";
    try {
        var multiselect = $("#areaMultiSelects_" + actionPageId).data("kendoMultiSelect");
        var dataItem = multiselect.dataItems();
        reportFilter.ReportFilters.AreaTypeFilter = multiselect.value();
        areaIds = multiselect.value().join(",");
        $.each(dataItem, function (i, item) {
            areaName[i] = item.AREA_EDESC; //update this
        });
        area = areaName.join(",<br />");
        Data += area;
    } catch (e) { }


    //for form Code Filter
    var documentIds = [];
    var documentName = [];
    var document = "";
    try {
        var multiselect = $("#documentMultiSelect_" + actionPageId).data("kendoMultiSelect");
        var dataItem = multiselect.dataItems();
        reportFilter.ReportFilters.DocumentFilter = multiselect.value();
        documentIds = multiselect.value().join(',');
        $.each(dataItem, function (i, item) {
            documentName[i] = item.CategoryName; //update this
        });
        document = documentName.join(",<br />");
        Data += document;
    } catch (e) { }

    //for employeeCode Filter
    var employeeIds = [];
    var employeeName = [];
    var employee = "";
    try {
        var multiselect = $("#employeeMultiSelect_" + actionPageId).data("kendoMultiSelect");
        var dataItem = multiselect.dataItems();
        reportFilter.ReportFilters.EmployeeFilter = multiselect.value();
        employeeIds = multiselect.value().join(',');
        $.each(dataItem, function (i, item) {
            employeeName[i] = item.EmployeeName;
        });
        employee = employeeName.join(",<br />");
        Data += employee;
    } catch (e) { }


    //for agentCode Filter
    var agentIds = [];
    var agentName = [];
    var agent = "";
    try {

        var multiselect = $("#agentMultiSelect_" + actionPageId).data("kendoMultiSelect");
        var dataItem = multiselect.dataItems();
        reportFilter.ReportFilters.AgentFilter = multiselect.value();
        agentIds = multiselect.value().join(',');
        $.each(dataItem, function (i, item) {
            agentName[i] = item.AgentName;
        });
        agent = agentName.join(",<br />");
        Data += agent;
    } catch (e) { }

    //for divisionCode Filter
    var divisionIds = [];
    var divisionName = [];
    var division = "";
    try {

        var multiselect = $("#divisionMultiSelect_" + actionPageId).data("kendoMultiSelect");
        var dataItem = multiselect.dataItems();
        reportFilter.ReportFilters.DivisionFilter = multiselect.value();
        divisionIds = multiselect.value().join(',');
        $.each(dataItem, function (i, item) {
            divisionName[i] = item.DivisionName;
        });
        division = divisionName.join(",<br />");
        Data += division;
    } catch (e) { }

    //for account Filter
    var accountIds = [];
    var accountName = [];
    var account = "";
    try {

        var multiselect = $("#accountMultiSelect_" + actionPageId).data("kendoMultiSelect");
        var dataItem = multiselect.dataItems();
        accountIds = multiselect.value().join(',');
        $.each(dataItem, function (i, item) {
            accountName[i] = item.acc_group;
        });
        account = accountName.join(",<br />");
        Data += account;
    } catch (e) { }


    //for FiscalYear Filter
    var fiscalYearIds = [];
    var fiscalYearName = [];
    var fiscalYear = "";
    try {
        var multiselect = $("#FiscalYearMultiSelect_" + actionPageId).data("kendoMultiSelect");
        var dataItem = multiselect.dataItems();
        reportFilter.ReportFilters.FiscalYearFilter = dataItem;
        fiscalYearIds = multiselect.value().join(',');
        $.each(dataItem, function (i, item) {
            fiscalYearName[i] = item.FiscalYear;
        });
        fiscalYear = fiscalYearName.join(",<br />");
        Data += fiscalYear;
    } catch (e) { }






    var urlData = "";
    if (actionPageId == "EmployeeWiseSalesChartPartial")
        urlData = "?customerCode=" + customerIds + "&itemCode=" + productIds + "&categoryCode=" + categoryIds + "&companyCode=" + companyIds + "&branchCode=" + branchCodes + "&partyTypeCode=" + partyTypeIds + "&formCode=" + documentIds + "&employeeCode=" + employeeIds;
    else if (actionPageId == "ExpensesTrendChartPartial")
        urlData = "?customerCode=" + customerIds + "&itemCode=" + productIds + "&categoryCode=" + categoryIds + "&companyCode=" + companyIds + "&branchCode=" + branchCodes + "&partyTypeCode=" + partyTypeIds + "&formCode=" + documentIds + "&accountCode=" + accountIds;
    else
        urlData = "?customerCode=" + customerIds + "&itemCode=" + productIds + "&categoryCode=" + categoryIds + "&companyCode=" + companyIds + "&branchCode=" + branchCodes + "&partyTypeCode=" + partyTypeIds + "&formCode=" + documentIds;




    if (customerIds != "" || divisionIds != "" || locationCodes != "" || productIds != "" || categoryIds != "" || partyTypeIds != "" || documentIds != "" || employeeIds != "" || agentIds != "" || accountIds != "" || branchCodes != "" || fiscalYearIds != "" || areaIds!="") {
        message = "<h5 style='white-space: nowrap;'>";
        if (customerIds != "")
            message = message + "<strong>" + (customerIds.indexOf(',') > 0 ? "Customers" : "Customer") + ":</strong> <br />" + customer + "<br /><br />";
        if (productIds != "")
            message = message + "<strong>" + (productIds.indexOf(',') > 0 ? "Items" : "Item") + ":</strong><br /> " + product + "<br /><br />";
        if (locationCodes != "")
            message = message + "<strong>" + (locationCodes.indexOf(',') > 0 ? "Locations" : "Location") + ":</strong> <br />" + location + "<br /><br />";
        if (categoryIds != "")
            message = message + "<strong>Category: </strong><br />" + category + "<br /><br />";
        if (branchIds != "")
            message = message + "<strong>Branch: </strong><br />" + branch + "<br /><br />";
        if (partyTypeIds != "")
            message = message + "<strong>Party Type: </strong><br />" + partyType + "<br /><br />";
        if (areaIds != "")
            message = message + "<strong>Area: </strong><br />" + area + "<br /><br />";
        if (documentIds != "")
            message = message + "<strong>Form: </strong><br />" + document + "<br /><br />";
        if (employeeIds != "")
            message = message + "<strong>Employee: </strong><br />" + employee + "<br /><br />";
        if (divisionIds != "")
            message = message + "<strong>Division: </strong><br />" + division + "<br /><br />";
        if (agentIds != "")
            message = message + "<strong>Agent: </strong><br />" + agent + "<br /><br />";
        if (accountIds != "")
            message = message + "<strong>Account: </strong><br />" + account + "<br /><br />";
        if (branchCodes != "")
            message = message + "<strong>Branch: </strong><br />" + branch + "<br /><br />";
        if (fiscalYearIds != "")
            message = message + "<strong>FiscalYear: </strong><br />" + fiscalYear + "<br /><br />";

        message = message + "</h5>";


        // $("#" + actionPageId + " .DisplayFilterContent a").attr("data-original-title", message);       
        //$("#" + actionPageId + " .DisplayFilterContent a").text("Filtered With...");
    }
    else {
        message = "";
        $("#" + actionPageId + " .DisplayFilterContent a").text("");
    }

    if (_oldData != Data || IsDateChanged) {
        if (jQuery.isFunction(createChartMethod))
            createChartMethod();
        else
            window[createChartMethod](urlData);      
        var setting = $('[href="#Model_' + actionPageId + '"]');
        if (setting[0] == undefined)
            setting = $('[data-target="#Model_' + actionPageId + '"]');
        if (message != "") {
            setting
                .attr("data-toggle", "tooltip")
                .attr("data-original-title", message)
                .tooltip({ html: true, placement: "bottom" })
                .bind("click", function (evt) {
                    $("#Model_" + actionPageId).modal('show');
                    evt.preventDefault();
                });
        }
        else {
            setting
                .attr("data-toggle", "tooltip")
                .attr("data-original-title", "Setting")
                .tooltip({ html: true, placement: "bottom" })
                .bind("click", function (evt) {
                    $("#Model_" + actionPageId).modal('show');
                    evt.preventDefault();
                });
        }

        //_oldData = Data;
    }
}


function RefreshChartFilterControl(actionPageId) {

    //reset tooltip message
    $('[href="#Model_' + actionPageId + '"]')
                .attr("data-original-title", "Setting")
                .tooltip({ html: true, placement: "top" });

    try {
        $("#customerMultiSelect_" + actionPageId).data("kendoMultiSelect").value([]);
        reportFilter.ReportFilters.CustomerFilter = [];
        $("#customerSlidebox_" + actionPageId).slideUp();
    } catch (e) { }
    try {
        $("#locationMultiSelect_" + actionPageId).data("kendoMultiSelect").value([]);
        reportFilter.ReportFilters.LocationFilter = [];
        $("#locationSlidebox_" + actionPageId).slideUp();
    } catch (e) { }
    try {
        $("#productMultiselect_" + actionPageId).data("kendoMultiSelect").value([]);
        reportFilter.ReportFilters.ProductFilter = [];
        $("#productSlidebox_" + actionPageId).slideUp();
    } catch (e) { }
    try {
        $("#categoryMultiSelect_" + actionPageId).data("kendoMultiSelect").value([]);
        reportFilter.ReportFilters.CategoryFilter = [];
    } catch (e) { }
    try { $("#BranchMultiSelect_" + actionPageId).data("kendoMultiSelect").value([]); } catch (e) { }
    try {
        $("#employeeMultiSelect_" + actionPageId).data("kendoMultiSelect").value([]);
        reportFilter.ReportFilters.EmployeeFilter = [];
    } catch (e) { }
    try {
        $("#divisionMultiSelect_" + actionPageId).data("kendoMultiSelect").value([]);
        reportFilter.ReportFilters.DivisionFilter = [];
    } catch (e) { }
    try {
        $("#agentMultiSelect_" + actionPageId).data("kendoMultiSelect").value([]);
        reportFilter.ReportFilters.AgentFilter = [];
    } catch (e) { }
    try {
        $("#consolidateTreeView_" + actionPageId).data("kendoTreeView").collapse(".k-item");
    } catch (e) { }
    try {
        $("#accountMultiSelect_" + actionPageId).data("kendoMultiSelect").value([]);
    } catch (e) { }
    try {
        $("#FiscalYearMultiSelect_" + actionPageId).data("kendoMultiSelect").value([]);
        reportFilter.ReportFilters.FiscalYearFilter = [];
    } catch (e) { }
}


var getFilteredCompareChart = function (pageId) {
    
    //for branchTree filter       
    var filteredCompareChart = [];
    //two chart is static so add manually
    filteredCompareChart.push("SalesChartFiscalYear");
    filteredCompareChart.push("SalesChartBranchWiseFiscalYear");
    try {
        var kendoData = $("#consolidateTreeView_" + pageId).data("kendoTreeView");
        var items = getCheckedItems(kendoData);
        if (items.length != 0) {
            for (var i = 0; i < items.length; i++) {
                if (items[i].branch_Code.indexOf('.') != -1) {
                    var company = "_" + items[i].Abbr_Code;
                    filteredCompareChart.push("SalesChartBranchWiseFiscalYear" + items[i].branch_edesc.replace(/[-,\s]/g, '') + company);
                }
            }
            
            //for only him
            /// $("#consolidateTreeView_" + pageId).hide();
            // filteredCompareChart = filteredCompareChart.slice(2, 11);
        }
        return filteredCompareChart;
    } catch (e) { }
}


var getFilteredCompareChartDivision = function () {

    var filteredCompareChart = [];
    //chart is static so add manually
    filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearRoyalTraserDivision_JGI.cshtml");
    filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearRuslan_Division_JGI.cshtml");
    filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearGoldenOakDivision_JGI.cshtml");
    filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearBlueDiamondDivision_JGI.cshtml");
    filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearUltimateGinDivision_JGI.cshtml");
    filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearWarsteinerBeerDivision_JGI.cshtml");
    
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearENTERPRISEBUSINESS_NNPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearDISTRIBUTIONDIVISION_NNPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearSCIENTIFICBUSINESSDIVISION_NNPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearCORPORATEDIVISION_NNPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearSERVICECENTREDIVISION_NNPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearNEODIVISION_NNPL.cshtml");
    

    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearSamsungMobileDivision_HEPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearSamsungCEDivision_HEPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearAlternativeEnergyDivision_HEPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearFactoryDivision_HEPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearWatchDivision_HEPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearElectricalDivision_HEPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearSmallApplianceDivision_HEPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearServiceCenterDivision_HEPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_SalesChartBranchWiseFiscalYearHimstarDivision_HEPL.cshtml");
    return filteredCompareChart;
}




var getFilteredCompareChartDivisionCollectionwise = function () {

    var filteredCompareChart = [];
    //chart is static so add manually
    filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_CollectionChartBranchWiseFiscalYearRuslanDivision_JGI.cshtml");
    filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_CollectionChartBranchWiseFiscalYearRoyalTraserDivision_JGI.cshtml");
    filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_CollectionChartBranchWiseFiscalYearGoldenOakDivision_JGI.cshtml");
    filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_CollectionChartBranchWiseFiscalYearBlueDiamondDivision_JGI.cshtml");
    filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_CollectionChartBranchWiseFiscalYearUltimateGinDivision_JGI.cshtml");
    filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_CollectionChartBranchWiseFiscalYearWarsteinerBeerDivision_JGI.cshtml");


    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_CollectionChartBranchWiseFiscalYearSamsungMobileDivision_HEPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_CollectionChartBranchWiseFiscalYearSamsungCEDivision_HEPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_CollectionChartBranchWiseFiscalYearAlternativeEnergyDivision_HEPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_CollectionChartBranchWiseFiscalYearFactoryDivision_HEPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_CollectionChartBranchWiseFiscalYearWatchDivision_HEPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_CollectionChartBranchWiseFiscalYearElectricalDivision_HEPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_CollectionChartBranchWiseFiscalYearSmallApplianceDivision_HEPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_CollectionChartBranchWiseFiscalYearServiceCenterDivision_HEPL.cshtml");
    //filteredCompareChart.push("~/Areas/NeoErp.sales.Module/Views/SalesHome/_CollectionChartBranchWiseFiscalYearHimstarDivision_HEPL.cshtml");
    return filteredCompareChart;
}

function getMonthFromString(mon) {
    var value = new Date(Date.parse(mon + " 1, 2012")).getMonth() + 1;
    if (value < 7)
        value = value + 13;
    return value;
}


var sort_by = function (field, reverse, primer) {

    var key = primer ?
        function (x) { return primer(x[field]) } :
        function (x) { return x[field] };

    reverse = !reverse ? 1 : -1;

    return function (a, b) {
        return a = key(a), b = key(b), reverse * ((a > b) - (b > a));
    }
}


var getMaxValueFromDataSource = function (e) {
    var data = e.sender.dataSource.view();
    var max = 0;
    var min = 0;

    for (var i = 0, length = e.sender.options.series.length; i < length; i++) {
        var fieldtype = e.sender.options.series[i].field;

        //calculate max value for group
        try {
            jQuery.map(data, function (obj) {
                jQuery.map(obj.items, function (objitem) {
                    if (objitem[fieldtype] > max)
                        max = objitem[fieldtype];
                })
            });
        } catch (e) { }

        //calculate max value for non-group
        try {
            jQuery.map(data, function (obj) {
                if (obj[fieldtype] > max)
                    max = obj[fieldtype];
            });
        } catch (e) { }





        //calculate min value for group
        try {
            jQuery.map(data, function (obj) {
                jQuery.map(obj.items, function (objitem) {
                    if (objitem[fieldtype] < min)
                        min = objitem[fieldtype];
                });
            });
        } catch (e) { }

        //calculate min value for non-group
        try {
            jQuery.map(data, function (obj) {
                if (obj[fieldtype] < min)
                    min = obj[fieldtype];
            });
        } catch (e) { }



    };
    var maxValue = -(min) > max ? -(min) : max;
    return maxValue;

}
var GetFormatlongNumber=function FormatLongNumber(value) {
    if (value == 0) {
        return 0;
    }
    else {
        // for testing
        //value = Math.floor(Math.random()*1001);

        // hundreds
        if (value <= 999) {
            return value;
        }
        // thousands
        else if (value >= 1000 && value <= 999999) {
            return (value / 1000).toFixed(2)+ ' K';
        }
        // millions
        else if (value >= 1000000 && value <= 999999999) {
            return (value / 1000000).toFixed(2) + ' M';
        }
        // billions
        else if (value >= 1000000000 && value <= 999999999999) {
            return (value / 1000000000).toFixed(2) + ' B';
        }
        else
            return value;
    }
}

var getCategoryFromDataSource = function (e) {
    var data = e.sender.dataSource.view();
    var max = 0;
    var categoryAxisField = e.sender.options.categoryAxis.field;
    var category = [];
    $.each(data, function (i, obj) {
        if (obj.items != undefined) {            
            $.grep(obj.items, function (item) {
                if (item[categoryAxisField] != undefined)
                    category.push(item[categoryAxisField])
            });           
        }
    });
    category = _.uniq(category);
    //$.each(data, function (i, obj) {
    //    if (obj.items != undefined && obj.items.length > max) {
    //        category = [];
    //        $.grep(obj.items, function (item) {
    //            if (item[categoryAxisField] != undefined)
    //                category.push(item[categoryAxisField])
    //        });
    //        max = obj.items.length;
    //    }
    //});
    return category;



}



