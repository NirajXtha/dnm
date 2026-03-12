var message = '';
var CustomerFilterName = [];
var ProductFilterName = [];
var SupplierFilterName = [];
var DistEmployeeFilterName = [];
var LedgerFilterName = [];
var PostedGenericFilterName = [];
var DocumentFilterName = [];
var CategoryFilterName = [];
var EmployeeFilterName = [];
var AgentFilterName = [];
var DivisionFilterName = [];
var PartyTypeFilterName = [];
var AreaTypeFilterName = [];
var SalesPersonFilterName = [];
var PurchaseDocumentFilterName = [];
var LocationFilterName = [];
var LcLocationFilterName = [];
var CompanyFilterName = [];
var BranchFilterName = [];
var CurrencyFilterName = [];
var BrandFilterName = [];
var LcStatusFilterName = [];
var LcNumberFilterName = [];
var MenuNameFilterName = [];
var ModuleNameFilterName = [];
var UserNameFilterName = [];
var ItemNameFilter = [];
var InvoiceNumberFilterName = [];
var ContractStatusTypeFilterName = [];
var DistAreaFilter = [];
var FiscalYearFilterName = [];
var AccountFilterName = [];

var Customer = '';
var Product = '';
var Supplier = '';
var DistEmployee = '';
var Ledger = '';
var PostedGeneric = '';
var Document = '';
var Category = '';
var Employee = '';
var Agent = '';
var Division = '';
var PartyType = '';
var AreaType = '';
var SalesPerson = '';
var PurchaseDocument = '';
var Location = '';
var LcLocation = '';
var Company = '';
var Branch = '';
var Currency = '';
var Brand = '';
var LcStatus = '';
var ContractStatusType = '';
var distAreaFilter = '';
var FiscalYear = '';
var AccountFilter = '';

var ReportFilter = function () {
    return {

        init: function (pageAlias) {
            $("#RunQuery").on("click", function () {
                ReportFilter.placeFilterInfo();
            });

            $('#reportFilterSettings').on('hidden', function () {
                ReportFilter.placeFilterInfo();
            });
            var alias = pageAlias == "" ? pageAlias : "_" + pageAlias;
            $("#applyFilters" + alias).on("click", function () {
                $("#reportFilterSettings").modal("toggle");

            });
            $("#quantity-figure-range ").ionRangeSlider({
                type: "single",
                values: ["Actual", "Hundred", "Thousand"],
                grid: true,
            });

            $("#amount-figure-range").ionRangeSlider({
                type: "single",
                values: ["Actual", "Thousand", "Lakh", "Crore"],
                grid: true,

            });

            $("#quantity-roundUp-range").ionRangeSlider({
                type: "single",
                values: ["0", "0.0", "0.00", "0.000", "0.0000"],
                from: 2,
                grid: true,
            });
            $("#amount-roundUp-range").ionRangeSlider({
                type: "single",
                values: ["0", "0.0", "0.00", "0.000", "0.0000"],
                from: 2,
                grid: true,

            });

            $(".non-negative-non-decimal").keypress(function (evt) {
                var keycode = evt.charCode || evt.keyCode;
                if (keycode == 45 || keycode == 43 || keycode == 46) { //Enter key's keycode
                    return false;
                }
            });

            if (!$().iCheck) {
                return;
            }

            $('.icheck').each(function () {
                var checkboxClass = $(this).attr('data-checkbox') ? $(this).attr('data-checkbox') : 'icheckbox_line-green';

                if (checkboxClass.indexOf('_line') > -1) {
                    $(this).iCheck({
                        checkboxClass: checkboxClass,
                        insert: '<div class="icheck_line-icon"></div>' + $(this).attr("data-label")
                    });
                } else {
                    $(this).iCheck({
                        checkboxClass: checkboxClass,
                    });
                }

                if ($(this).attr("checked") === undefined && $(this).attr("id") === "show-quantity-check") {
                    ReportFilter.disableAmountFilter();
                }

                if ($(this).attr("checked") === undefined && $(this).attr("id") === "show-amount-check") {
                    ReportFilter.disableQuantityFilter();
                }


                if ($(this).attr("checked") === undefined && $(this).attr("id") === "user-custom-frequency-days") {
                    ReportFilter.toggleFrequencyFilter(false);
                }

                if ($(this).attr("checked") === undefined && $(this).attr("id") === "user-custom-fixed-days") {
                    ReportFilter.toggleFixedDayFilter(false);
                }
            });

            $('.icheck').on("ifToggled", function (event) {

                if ($(this).attr("id") === "show-quantity-check") {
                    if ($(this).attr("checked") == undefined && $(this).attr("id") === "show-quantity-check") {

                        ReportFilter.disableQuantityFilter();
                    }
                    else {
                        ReportFilter.enableQuantityFilter();
                    }
                }
                if ($(this).attr("id") === "show-amount-check") {
                    if ($(this).attr("checked") === undefined && $(this).attr("id") === "show-amount-check") {

                        ReportFilter.disableAmountFilter();
                    }
                    else {
                        ReportFilter.enableAmountFilter();
                    }
                }

                if ($(this).attr("id") === "user-custom-fixed-days") {
                    if ($(this).attr("checked") === undefined && $(this).attr("id") === "user-custom-fixed-days") {

                        ReportFilter.toggleFixedDayFilter(false);
                    }
                    else {
                        ReportFilter.toggleFixedDayFilter(true);
                    }
                }

                if ($(this).attr("id") === "user-custom-frequency-days") {
                    if ($(this).attr("checked") === undefined && $(this).attr("id") === "user-custom-frequency-days") {

                        ReportFilter.toggleFrequencyFilter(false);
                    }
                    else {
                        ReportFilter.toggleFrequencyFilter(true);
                    }
                }
            });
        },

        disableQuantityFilter: function () {

            $("#quantity-roundUp-range").data("ionRangeSlider").update({ disable: true });
            $("#quantity-figure-range").data("ionRangeSlider").update({ disable: true });

        },
        disableAmountFilter: function () {

            $("#amount-figure-range").data("ionRangeSlider").update({ disable: true });
            $("#amount-roundUp-range").data("ionRangeSlider").update({ disable: true });
        },
        enableQuantityFilter: function () {
            $("#quantity-roundUp-range").data("ionRangeSlider").update({ disable: false });
            $("#quantity-figure-range").data("ionRangeSlider").update({ disable: false });

        },
        enableAmountFilter: function () {

            $("#amount-figure-range").data("ionRangeSlider").update({ disable: false });
            $("#amount-roundUp-range").data("ionRangeSlider").update({ disable: false });
        },
        filterAdditionalData: function () {
            var data =
            {
                ReportFilters: {
                    FromDate: $("#FromDateVoucher").val(),
                    ToDate: $("#ToDateVoucher").val(),
                    ExchangeRate: $("#exchangeRate").val(),
                    QuantityFigureFilter: $("#show-quantity-check").length > 0 && $("#show-quantity-check").prop("checked") ? ($("#quantity-figure-range").val() == "" ? "Actual" : $("#quantity-figure-range").val()) : "",
                    QuantityRoundUpFilter: $("#show-quantity-check").length > 0 && $("#show-quantity-check").prop("checked") ? ($("#quantity-roundUp-range").val() == "" ? "0.00" : $("#quantity-roundUp-range").val()) : "",
                    AmountFigureFilter: $("#show-amount-check").length > 0 && $("#show-amount-check").prop("checked") ? ($("#amount-figure-range").val() == "" ? "Actual" : $("#amount-figure-range").val()) : "",
                    AmountRoundUpFilter: $("#show-amount-check").length > 0 && $("#show-amount-check").prop("checked") ? ($("#amount-roundUp-range").val() == "" ? "0.00" : $("#amount-roundUp-range").val()) : "",
                    AmountRangeFilter: ReportFilter.getRangeFilter($("#rangeAmount-min-val").val(), $("#rangeAmount-max-val").val()),
                    QuantityRangeFilter: ReportFilter.getRangeFilter($("#rangeQuantity-min-val").val(), $("#rangeQuantity-max-val").val()),
                    RateRangeFilter: ReportFilter.getRangeFilter($("#rangeRate-min-val").val(), $("#rangeRate-max-val").val()),
                    CustomerFilter: $("#customerMultiSelect").length > 0 ? ReportFilter.getCustomerFilter() : [],
                    ProductFilter: $("#productMultiselect").length > 0 ? ReportFilter.getProductFilter() : [],
                    SupplierFilter: $("#supplierMultiSelect").length > 0 ? ReportFilter.getSupplierFilter() : [],
                    DistEmployeeFilter: $("#DistEmployeeMultiSelect").length > 0 ? ReportFilter.getDistEmployeeFilter() : [],
                    DocumentFilter: $("#documentMultiSelect").length > 0 ? ReportFilter.getDocumentFilter() : [],
                    CategoryFilter: $("#categoryMultiSelect").length > 0 ? ReportFilter.getCategoryFilter() : [],
                    EmployeeFilter: $("#employeeMultiSelect").length > 0 ? ReportFilter.getEmployeeFilter() : [],
                    AgentFilter: $("#agentMultiSelect").length > 0 ? ReportFilter.getAgentFilter() : [],
                    DivisionFilter: $("#divisionMultiSelect").length > 0 ? ReportFilter.getDivisionFilter() : [],
                    PartyTypeFilter: $("#partyTypeMultiSelects").length > 0 ? ReportFilter.getPartyTypeFilter() : [],
                    AreaTypeFilter: $("#areaMultiSelects").length > 0 ? ReportFilter.getAreaTypeFilter() : [],
                    SalesPersonFilter: $("#SalesMultiSelect").length > 0 ? ReportFilter.getSalesPersonFilter() : [],
                    PurchaseDocumentFilter: $("#purchaseDocumentMultiSelect").length > 0 ? ReportFilter.getPurchaseDocumentFilter() : [],
                    LocationFilter: $("#locationMultiSelect").length > 0 ? ReportFilter.getLocationFilter() : [],
                    LcLocationFilter: $("#lcLocationMultiSelect").length > 0 ? ReportFilter.getLcLocationFilter() : [],
                    LedgerFilter: $("#LedgerMultiSelect").length > 0 ? ReportFilter.getLedgerFilter() : [],
                    PostedGenericFilter: $("#PostedGenericMultiSelect").length > 0 ? ReportFilter.getPostedGenericFilter() : [],
                    CompanyFilter: ReportFilter.getCompanyFilter() ? ReportFilter.getCompanyFilter() : [],
                    //CompanyFilter: $("#companyMultiSelect").lenght > 0 ? ReportFilter.getCompanyFilter() : [],
                    // BranchFilter: $("#BranchMultiSelect").length > 0 ? ReportFilter.getBranchFilter() : [] ItemNameFilter
                    BranchFilter: ReportFilter.getBranchFilter() ? ReportFilter.getBranchFilter() : [],
                    AccountFilter: ReportFilter.getAccountFilter() ? ReportFilter.getAccountFilter() : [],
                    CurrencyFilter: $("#currencyTypeMultiSelect").length > 0 ? ReportFilter.getCurrencyFilter() : [],
                    BrandFilter: $("#brandTypeMultiSelect").length > 0 ? ReportFilter.getBrandFilter() : [],
                    LcStatusFilter: $("#lcStatusTypeMultiSelect").length > 0 ? ReportFilter.getLcStatusFilter() : [],
                    LcNumberFilter: $("#lcNumberTypeMultiSelect").length > 0 ? ReportFilter.getLcNumberFilter() : [],

                    ModuleNameFilter: $("#moduleNameTypeMultiSelect").length > 0 ? ReportFilter.getModuleNameFilter() : [],
                    UserNameFilter: $("#userNameTypeMultiSelect").length > 0 ? ReportFilter.UserNameFilter() : [],
                    MenuNameFilter: $("#menuNameMultiSelect").length > 0 ? ReportFilter.getMenuNameFilter() : [],

                    ItemNameFilter: $("#itemNameTypeMultiSelect").length > 0 ? ReportFilter.getItemNameFilter() : [],
                    InvoiceNumberFilter: $("#invoiceNumberTypeMultiSelect").length > 0 ? ReportFilter.getInvoiceNumberFilter() : [],
                    ContractStatusTypeFilter: $("#selectContactStatusFilter").length > 0 ? ReportFilter.getContractStatusTypeFilter() : [],
                    DistAreaFilter: $("#distAreaMultiSelect").length > 0 ? ReportFilter.getDistAreaFilter() : [],
                    FiscalYearFilter: $("#FiscalYearMultiSelect").length > 0 ? ReportFilter.getFiscalYearFilter() : [],
                    ItemBrandFilter: $("#ItemBrandItemMultiSelect").length > 0 ? ReportFilter.getItemFromItemBrandFilter() : [],
                }
            };

            return data;
        },

        parseAmountFigure: function () {
            var value = ReportFilter.filterAdditionalData().ReportFilters["AmountFigureFilter"];
            if (value == "" || value == "Actual") { return 1; }
            else if (value == "Thousand") { return 1000; }
            else if (value == "Lakh") { return 100000; }
            else if (value == "Crore") { return 10000000; }
        },
        parseDecimalPlace: function (key) {
            var value = ReportFilter.getKeyValueMap(key);
            if (value == null || value === "") { return 2; }
            var str = value.toString();
            if (str.indexOf(".") === -1 && parseInt(str, 10) === 0) { return 0; }
            var decimalPlaces = str.split(".");
            if (decimalPlaces.length >= 2) {
                return decimalPlaces[1].length;
            }
            return 2;
        },
        getRangeFilter: function (min = 0, max = 0) {
            if (min.length > 0 && max.length > 0)
                return min + ";" + max;
            else if (min.length > 0)
                return min + ";" + "2147483647";
            else if (max.length > 0)
                return "-2147483647" + ";" + max;
            else
                return "";
        },
        getKeyValueMap: function (key) {
            return ReportFilter.filterAdditionalData().ReportFilters[key];
        },
        getCustomerFilter: function () {
            var customerIds = [];
            var multiselect = $("#customerMultiSelect").data("kendoMultiSelect");
            if (multiselect && typeof multiselect.value === "function") {
                customerIds = multiselect.value();
                if (typeof multiselect.dataItems === "function") {
                    $.each(multiselect.dataItems(), function (i, item) {
                        CustomerFilterName[i] = item.CustomerName;
                    });
                }
                Customer = CustomerFilterName.join(", ");
            }
            return customerIds;
        },
        getProductFilter: function () {
            var productIds = [];
            var multiselect = $("#productMultiselect").data("kendoMultiSelect");
            productIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                ProductFilterName[i] = item.ItemName;
            });
            Product = ProductFilterName.join(", ");
            return productIds;
        },
        getSupplierFilter: function () {
            var supplierIds = [];
            var multiselect = $("#supplierMultiSelect").data("kendoMultiSelect");
            supplierIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                SupplierFilterName[i] = item.SupplierName;
            });
            Supplier = SupplierFilterName.join(", ");
            return supplierIds;
        },
        getDistEmployeeFilter: function () {
            var disEmployeeIds = [];
            var multiselect = $("#DistEmployeeMultiSelect").data("kendoMultiSelect");
            disEmployeeIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                DistEmployeeFilterName[i] = item.DistEmployeeName;
            });
            DistEmployee = DistEmployeeFilterName.join(", ");
            return disEmployeeIds;
        },
        getLedgerFilter: function () {

            var ledgerIds = [];
            var multiselect = $("#LedgerMultiSelect").data("kendoMultiSelect");
            ledgerIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                LedgerFilterName[i] = item.Name;
            });
            Ledger = LedgerFilterName.join(", ");
            return ledgerIds;
        },
        getPostedGenericFilter: function () {

            var postedGenericIds = [];
            var multiselect = $("#PostedGenericMultiSelect").data("kendoMultiSelect");
            postedGenericIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                PostedGenericFilterName[i] = item.text;
            });
            PostedGeneric = PostedGenericFilterName.join(", ");
            return postedGenericIds;
        },

        getDocumentFilter: function () {
            var documentIds = [];
            var multiselect = $("#documentMultiSelect").data("kendoMultiSelect");
            documentIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                DocumentFilterName[i] = item.VoucherName;
            });
            Document = DocumentFilterName.join(", ");
            return documentIds;
        },
        getPurchaseDocumentFilter: function () {
            var purchaseDocumentIds = [];
            var multiselect = $("#purchaseDocumentMultiSelect").data("kendoMultiSelect");
            purchaseDocumentIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                PurchaseDocumentFilterName[i] = item.VoucherName;
            });
            PurchaseDocument = PurchaseDocumentFilterName.join(", ");
            return purchaseDocumentIds;
        },
        getCategoryFilter: function () {
            var categoryIds = [];
            var multiselect = $("#categoryMultiSelect").data("kendoMultiSelect");
            categoryIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                CategoryFilterName[i] = item.CategoryName;
            });
            Category = CategoryFilterName.join(", ");
            return categoryIds;
        },
        getEmployeeFilter: function () {
            var employeeIds = [];
            var multiselect = $("#employeeMultiSelect").data("kendoMultiSelect");
            employeeIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                EmployeeFilterName[i] = item.EmployeeName;
            });
            Employee = EmployeeFilterName.join(", ");
            return employeeIds;
        },
        getAgentFilter: function () {

            var agentIds = [];
            var multiselect = $("#agentMultiSelect").data("kendoMultiSelect");
            agentIds = multiselect.value();
            //$.each(multiselect.dataItems(), function (i, item) {
            //    AgentFilterName[i] = item.AgentName;
            //});
            //Agent = AgentFilterName.join(", ");
            return agentIds;
        },
        getDivisionFilter: function () {

            var divisionIds = [];
            var multiselect = $("#divisionMultiSelect").data("kendoMultiSelect");
            divisionIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                DivisionFilterName[i] = item.DivisionName;
            });
            Division = DivisionFilterName.join(", ");
            return divisionIds;
        },
        getPartyTypeFilter: function () {

            var partyTypeIds = [];
            var multiselect = $("#partyTypeMultiSelects").data("kendoMultiSelect");
            partyTypeIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                PartyTypeFilterName[i] = item.PartyTypeName;
            });
            PartyType = PartyTypeFilterName.join(", ");
            return partyTypeIds;
        },
        getAreaTypeFilter: function () {

            var areaTypeIds = [];
            var multiselect = $("#areaMultiSelects").data("kendoMultiSelect");
            areaTypeIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                AreaTypeFilterName[i] = item.AREA_EDESC;
            });
            AreaType = AreaTypeFilterName.join(", ");
            return areaTypeIds;
        },
        getSalesPersonFilter: function () {

            var salesFilterIds = [];
            var multiselect = $("#SalesMultiSelect").data("kendoMultiSelect");
            salesFilterIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                SalesPersonFilterName[i] = item.SalesPerson;
            });
            SalesPerson = SalesPersonFilterName.join(", ");
            return salesFilterIds;
        },

        getLocationFilter: function () {
            var locationIds = [];
            var multiselect = $("#locationMultiSelect").data("kendoMultiSelect");
            locationIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                LocationFilterName[i] = item.LocationName;
            });
            Location = LocationFilterName.join(", ");
            return locationIds;
        },
        getFiscalYearFilter: function () {
            //var FiscalYearIds = [];
            //var multiselect = $("#FiscalYearMultiSelect").data("kendoMultiSelect");
            //FiscalYearIds = multiselect.value();
            //$.each(multiselect.dataItems(), function (i, item) {
            //    FiscalYearFilterName[i] = item.FiscalYear;
            //});
            //FiscalYear = FiscalYearFilterName.join(", ");
            //return FiscalYearIds;
            var multi = $("#FiscalYearMultiSelect").getKendoMultiSelect(),
                multiDataItems = multi.dataItems(),
                selectedFiscalYear = [];

            for (var i = 0; i < multiDataItems.length; i += 1) {
                var currentYear = multiDataItems[i];

                selectedFiscalYear.push({
                    FiscalYear: currentYear.FiscalYear,
                    DBName: currentYear.DBName
                })
            }
            return selectedFiscalYear;
        },
        getItemFromItemBrandFilter: function () {
            //var itemFilterCodes = [];
            //var multiselect = $("#ItemBrandItemMultiSelect").data("kendoMultiSelect");
            //itemFilterCodes = multiselect.value();
            //return itemFilterCodes;
            var itemFilterItemCodes = [];
            var itemFilterSpCodes = [];
            var itemmultiselect = $("#ItemBrandItemMultiSelect").data("kendoMultiSelect");
            itemFilterItemCodes = itemmultiselect.value();
            var datas = $("#ItemBrandSpCodeMultiSelect").data("kendoMultiSelect").dataSource.data();
            var spCodeArr = datas.filter(function (item) { return itemFilterItemCodes.indexOf(item.ITEM_CODE) > -1 });
            $.each(spCodeArr, function (i, v) {
                itemFilterSpCodes.push(v.SP_CODE);
            });
            return _.uniq(itemFilterSpCodes);
        },
        getCompanyFilter: function () {
            //debugger;
            var companyList = $("#consolidateTreeView").data("kendoTreeView");
            if ($("#companyMultiSelect").length > 0) {
                var companyIds = [];
                var multiselect = $("#companyMultiSelect").data("kendoMultiSelect");
                console.log("Company multiselect ==================>>>" + multiselect);
                companyIds = multiselect.value();
                $.each(multiselect.dataItems(), function (i, item) {
                    CompanyFilterName[i] = item.CompanyName;
                });
                Company = CompanyFilterName.join(", ");
                return companyIds;
            } else if (companyList != undefined) {
                var items = getCheckedItems_consolidate(companyList);
                $.each(items, function (i, item) {
                    CompanyFilterName[i] = item.branch_edesc;
                });
                Company = CompanyFilterName.join(", ");

                var companyCodeArray = [];
                if (items.length != 0) {
                    for (var i = 0; i < items.length; i++) {
                        var code = items[i] && items[i].branch_Code;
                        if (typeof code !== "string") { continue; }
                        if (code.indexOf('.') === -1) {
                            companyCodeArray.push(code);
                        } else {
                            companyCodeArray.push(code.substring(0, code.indexOf('.')));
                        }
                    }
                }
                return _.uniq(companyCodeArray);
            } else
                return [];

        },
        // GET COMPANY FILTER changes by aakash
        //getCompanyFilter: function () {

        //    var companyNameTypeIds = [];
        //    var CompanyNameFilterName = [];
        //    var multiselect = $("#companyMultiSelect").data("kendoMultiSelect");
        //    if (multiselect === undefined) {
        //        CompanyNameFilterName = [];
        //        return;
        //    }
        //    companyNameTypeIds = multiselect.value();
        //    $.each(multiselect.dataItems(), function (i, item) {
        //        CompanyNameFilterName[i] = item.CompanyName;
        //    });
        //    return CompanyNameFilterName;
        //},
        getBranchFilter: function () {
            var branchList = $("#consolidateTreeView").data("kendoTreeView");
            if ($("#BranchMultiSelect").length > 0) {
                var branchIds = [];
                var multiselect = $("#BranchMultiSelect").data("kendoMultiSelect");
                branchIds = multiselect.value();
                $.each(multiselect.dataItems(), function (i, item) {
                    BranchFilterName[i] = item.BranchName;
                });
                if (BranchFilterName.length > 0) {
                    Branch = BranchFilterName.join(", ");
                }

                return branchIds;
            }
            else if (branchList != undefined) {
                var kendoData = $("#consolidateTreeView").data("kendoTreeView");
                var items = getCheckedItems_consolidate(kendoData);
                $.each(items, function (i, item) {
                    BranchFilterName[i] = item.branch_edesc;
                });
                Branch = BranchFilterName.join(", ");
                var branchCodeArray = [];
                if (items.length > 0) {
                    for (var i = 0; i < items.length; i++) {
                        var code = items[i] && items[i].branch_Code;
                        if (typeof code === "string" && code.indexOf('.') !== -1) {
                            branchCodeArray.push(code);
                        }
                    }
                }
                return branchCodeArray;
            }
            else
                return [];
        },

        getAccountFilter: function () {
            var accountList = $("#consolidateTreeView").data("kendoTreeView");
            // Prefer explicit account multiselect if present
            if ($("#AccountMultiSelect").length > 0) {
                var accountIds = [];
                var multiselect = $("#AccountMultiSelect").data("kendoMultiSelect");
                if (multiselect && typeof multiselect.value === "function") {
                    accountIds = multiselect.value();
                    $.each(multiselect.dataItems ? multiselect.dataItems() : [], function (i, item) {
                        AccountFilterName[i] = item.AccountName;
                    });
                    if (AccountFilterName.length > 0) {
                        AccountFilter = AccountFilterName.join(", ");
                    }
                }
                return accountIds;
            } else if (accountList != undefined) {
                // Fallback: attempt to read from consolidate tree only if nodes contain account fields
                var kendoData = $("#consolidateTreeView").data("kendoTreeView");
                var items = getCheckedItems_consolidate(kendoData) || [];
                var accountCodeArray = [];
                var names = [];
                for (var i = 0; i < items.length; i++) {
                    var code = items[i] && items[i].account_Code;
                    var name = items[i] && (items[i].account_edesc || items[i].AccountName);
                    if (typeof name === "string") {
                        names.push(name);
                    }
                    if (typeof code === "string" && code.indexOf('.') !== -1) {
                        accountCodeArray.push(code);
                    }
                }
                if (names.length > 0) {
                    AccountFilterName = names;
                    AccountFilter = names.join(", ");
                }
                return accountCodeArray;
            } else {
                return [];
            }
        },
        getCurrencyFilter: function () {

            var currencyTypeIds = [];
            var multiselect = $("#currencyTypeMultiSelect").data("kendoMultiSelect");
            currencyTypeIds = multiselect.value();
            //$.each(multiselect.dataItems(), function (i, item) {
            //    CurrencyFilterName[i] = item.Currency;
            //});
            //Currency = CurrencyFilterName.join(", "); 
            return currencyTypeIds;
        },
        getBrandFilter: function () {

            var currencyTypeIds = [];
            var multiselect = $("#brandTypeMultiSelect").data("kendoMultiSelect");
            currencyTypeIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                BrandFilterName[i] = item.BrandName;
            });
            Brand = BrandFilterName.join(", ");
            return currencyTypeIds;
        },
        getLcStatusFilter: function () {

            var currencyTypeIds = [];
            var multiselect = $("#lcStatusTypeMultiSelect").data("kendoMultiSelect");
            currencyTypeIds = multiselect.value();
            //$.each(multiselect.dataItems(), function (i, item) {
            //    LcStatusFilterName[i] = item.LcStatus;
            //});
            //LcStatus = LcStatusFilterName.join(", ");
            return currencyTypeIds;
        },
        getLcNumberFilter: function () {

            var lcNumberTypeIds = [];
            var LcNumberFilterName = [];
            var multiselect = $("#lcNumberTypeMultiSelect").data("kendoMultiSelect");
            lcNumberTypeIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                LcNumberFilterName[i] = item.LcNumber;
            });
            //LcNumber = LcNumberFilterName.join(", "); 
            return LcNumberFilterName;
        },
        getModuleNameFilter: function () {

            var moduleNameTypeIds = [];
            var ModuleNameFilterName = [];
            var multiselect = $("#moduleNameTypeMultiSelect").data("kendoMultiSelect");
            if (multiselect === undefined) {
                ModuleNameFilterName = [];
                return;
            }
            moduleNameTypeIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                ModuleNameFilterName[i] = item.module_name;
            });
            return ModuleNameFilterName;
        },
        UserNameFilter: function () {

            var userNameTypeIds = [];
            var UserNameFilterName = [];
            var multiselect = $("#userNameTypeMultiSelect").data("kendoMultiSelect");
            if (multiselect === undefined) {
                UserNameFilterName = [];
                return;
            }
            userNameTypeIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                UserNameFilterName[i] = item.user_name;
            });
            return UserNameFilterName;
        },

        getMenuNameFilter: function () {

            var menuNameTypeIds = [];
            var MenuNameFilterName = [];
            var multiselect = $("#menuNameMultiSelect").data("kendoMultiSelect");
            if (multiselect === undefined) {
                MenuNameFilterName = [];
                return;
            }
            menuNameTypeIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {

                MenuNameFilterName[i] = item.menu_name;
            });
            return MenuNameFilterName;
        },

        getItemNameFilter: function () {

            var itemNameTypeIds = [];
            var ItemNameFilter = [];
            var multiselect = $("#itemNameTypeMultiSelect").data("kendoMultiSelect");
            itemNameTypeIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {

                ItemNameFilter[i] = item.ItemName;
            });
            //LcNumber = LcNumberFilterName.join(", ");
            return ItemNameFilter;
        },
        getInvoiceNumberFilter: function () {

            var invoiceNumberTypeIds = [];
            var InvoiceNumberFilterName = [];
            var multiselect = $("#invoiceNumberTypeMultiSelect").data("kendoMultiSelect");
            invoiceNumberTypeIds = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                InvoiceNumberFilterName[i] = item.InvoiceNumber;
            });
            //LcNumber = LcNumberFilterName.join(", ");
            return InvoiceNumberFilterName;
        },
        getLcLocationFilter: function () {

            var locations = [];
            var multiselect = $("#lcLocationMultiSelect").data("kendoMultiSelect");
            locations = multiselect.value();
            //$.each(multiselect.dataItems(), function (i, item) {
            //    LcLocationFilterName[i] = item.LcLocation;
            //});
            //LcLocation = LcLocationFilterName.join(", ");
            return locations;
        },
        placeFilterInfo: function () {

            var filterData = ReportFilter.filterAdditionalData().ReportFilters;
            var filterNote = "Your viewing with filters ";
            var hasAnyFilter = false;

            if (filterData.QuantityFigureFilter != "") {
                hasAnyFilter = true;
                filterNote += "Quantity Figure:" + filterData.QuantityFigureFilter;
            }

            if (filterData.QuantityRoundUpFilter != "") {
                if (hasAnyFilter)
                    filterNote += " <i class='fa fa-ellipsis-v'></i> ";

                hasAnyFilter = true;

                filterNote += "Quantity Roundup:" + filterData.QuantityRoundUpFilter;
            }

            if (filterData.AmountFigureFilter != "") {
                if (hasAnyFilter)
                    filterNote += " <i class='fa fa-ellipsis-v'></i> ";

                hasAnyFilter = true;

                filterNote += "Amount Figure:" + filterData.AmountFigureFilter;
            }

            if (filterData.AmountRoundUpFilter != "") {
                if (hasAnyFilter)
                    filterNote += " <i class='fa fa-ellipsis-v'></i> ";

                hasAnyFilter = true;

                filterNote += "Amount Roundup:" + filterData.AmountRoundUpFilter;
            }


            if (filterData.QuantityRangeFilter != "") {
                if (hasAnyFilter)
                    filterNote += " <i class='fa fa-ellipsis-v'></i> ";

                hasAnyFilter = true;

                filterNote += "Quantity Range:" + filterData.QuantityRangeFilter;
            }

            if (filterData.RateRangeFilter != "") {
                if (hasAnyFilter)
                    filterNote += " <i class='fa fa-ellipsis-v'></i> ";

                hasAnyFilter = true;

                filterNote += "Rate Range:" + filterData.RateRangeFilter;
            }

            if (filterData.AmountRangeFilter != "") {
                if (hasAnyFilter)
                    filterNote += " <i class='fa fa-ellipsis-v'></i> ";
                filterNote += "Amount Range:" + filterData.AmountRangeFilter;
            }
            displayBarNotification(filterNote, "info");
            //Popup filter message
            message = "<h5><br /><br /><br />";
            ;
            if (filterData.FromDate != "" && filterData.ToDate != "") {
                message = message + "<strong>" + "Date Range:</strong> <br />" + filterData.FromDate.toString() + " to " + filterData.ToDate.toString() + "<br /><br />";
            }
            if (filterData.QuantityFigureFilter != "") {
                message = message + "<strong>" + "Quantity Figure:</strong> <br />" + filterData.QuantityFigureFilter + "<br /><br />";
            }

            if (filterData.QuantityRoundUpFilter != "") {
                message = message + "<strong>" + "Quantity Roundup:</strong> <br />" + filterData.QuantityRoundUpFilter + "<br /><br />";
            }

            if (filterData.AmountFigureFilter != "") {
                message = message + "<strong>" + "Amount Figure:</strong> <br />" + filterData.AmountFigureFilter + "<br /><br />";
            }

            if (filterData.AmountRoundUpFilter != "") {
                message = message + "<strong>" + "Amount Roundup:</strong> <br />" + filterData.AmountRoundUpFilter + "<br /><br />";
            }


            if (filterData.QuantityRangeFilter != "") {
                message = message + "<strong>" + "Quantity Range:</strong> <br />" + filterData.QuantityRangeFilter + "<br /><br />";
            }

            if (filterData.RateRangeFilter != "") {
                message = message + "<strong>" + "Rate Range:</strong> <br />" + filterData.RateRangeFilter + "<br /><br />";
            }

            if (filterData.AmountRangeFilter != "") {
                message = message + "<strong>" + "Amount range:</strong> <br />" + filterData.AmountRangeFilter + "<br /><br />";
            }
            if (filterData.CustomerFilter != "") {
                message = message + "<strong>" + "Customer:</strong> <br />" + Customer + "<br /><br />";
            }
            if (filterData.ProductFilter != "") {
                message = message + "<strong>" + "Product:</strong> <br />" + Product + "<br /><br />";
            }
            if (filterData.SupplierFilter != "") {
                message = message + "<strong>" + "Supplier:</strong> <br />" + Supplier + "<br /><br />";
            }
            if (filterData.DistEmployeeFilter != "") {
                message = message + "<strong>" + "Distributor Employee:</strong> <br />" + DistEmployee + "<br /><br />";
            }
            if (filterData.DocumentFilter != "") {
                message = message + "<strong>" + "Document:</strong> <br />" + Document + "<br /><br />";
            }
            if (filterData.CategoryFilter != "") {
                message = message + "<strong>" + "Categories:</strong> <br />" + Category + "<br /><br />";
            }
            //if (filterData.EmployeeFilter != "") {
            //    message = message + "<strong>" + "Employees:</strong> <br />" + Employee + "<br /><br />";
            //}
            //if (filterData.AgentFilter != "") {
            //    message = message + "<strong>" + "Agent:</strong> <br />" + Agent + "<br /><br />";
            //}
            if (filterData.DivisionFilter != "") {
                message = message + "<strong>" + "Division:</strong> <br />" + Division + "<br /><br />";
            }
            if (filterData.PartyTypeFilter != "") {
                message = message + "<strong>" + "PartyType:</strong> <br />" + PartyType + "<br /><br />";
            }
            if (filterData.AreaTypeFilter != "") {
                message = message + "<strong>" + "AreaType:</strong> <br />" + AreaType + "<br /><br />";
            }
            if (filterData.SalesPersonFilter != "") {
                message = message + "<strong>" + "SalesPerson:</strong> <br />" + SalesPerson + "<br /><br />";
            }
            if (filterData.PurchaseDocumentFilter != "") {
                message = message + "<strong>" + "PurchaseDocument:</strong> <br />" + PurchaseDocument + "<br /><br />";
            }
            if (filterData.LocationFilter != "") {
                message = message + "<strong>" + "Location:</strong> <br />" + Location + "<br /><br />";
            }
            if (filterData.CompanyFilter != "") {
                message = message + "<strong>" + "Company:</strong> <br />" + Company + "<br /><br />";
            }
            if (filterData.BranchFilter != "") {
                message = message + "<strong>" + "Branch:</strong> <br />" + Branch + "<br /><br />";
            }
            if (filterData.AccountFilter != "") {
                message = message + "<strong>" + "Account:</strong> <br />" + Account + "<br /><br />";
            }
            if (filterData.BrandFilter != "") {
                message = message + "<strong>" + "Brand:</strong> <br />" + Brand + "<br /><br />";
            }
            if (filterData.ContractStatusTypeFilter != "") {
                message = message + "<strong>" + "ContractStatusType:</strong> <br />" + ContractStatusType + "<br /><br />";
            }
            //if (filterData.CurrencyFilter != "") {
            //    message = message + "<strong>" + "Currency:</strong> <br />" + Currency + "<br /><br />";
            //}
            message = message + "</h5>";
            //$("#filter-info").html("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'></button><p>" + filterNote + "</p>")
            if ($(message).text() != "") {
                $("#AdvanceFilter").attr('data-original-title', message);
            }


        },
        getContractStatusTypeFilter: function () {
            var ContractStatusTypes = [];
            var multiselect = $("#selectContactStatusFilter").data("kendoMultiSelect");
            ContractStatusTypes = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                ContractStatusTypeFilterName[i] = item.text;
            });
            ContractStatusType = ContractStatusTypeFilterName.join(", ");
            return ContractStatusTypes;
        },

        getDistAreaFilter: function () {
            var distAreaTypes = [];
            var multiselect = $("#distAreaMultiSelect").data("kendoMultiSelect");
            distAreaTypes = multiselect.value();
            $.each(multiselect.dataItems(), function (i, item) {
                DistAreaFilter[i] = item.text;
            });
            distAreaFilter = DistAreaFilter.join(", ");
            return distAreaTypes;
        },

        getBranchs: function () {
            var branchList = $("#consolidateTreeView").data("kendoTreeView");
            if ($("#BranchMultiSelect").length > 0) {
                var branchs = [];
                var multiselect = $("#BranchMultiSelect").data("kendoMultiSelect");
                branchs = multiselect.text();
                return branchs;
            }
            else if (branchList != undefined) {
                var kendoData = $("#consolidateTreeView").data("kendoTreeView");
                var ViewData = kendoData.dataSource.view();
                var items = getCheckedItems_consolidate(kendoData);
                var companyArray = [];
                for (var a = 0; a < ViewData.length; a++) {
                    var companyChecked = false;
                    if (ViewData[a].hasChildren) {
                        var branchArray = [];
                        var childrens = ViewData[a].children.view();
                        for (var b = 0; b < childrens.length; b++) {
                            if (childrens[b].checked) {
                                branchArray.push(childrens[b].branch_edesc);
                                companyChecked = true;
                            }
                        }
                        if (companyChecked) {
                            var newObj = {};
                            newObj['Company'] = ViewData[a].branch_edesc;
                            newObj['Branches'] = branchArray;
                            companyArray.push(newObj);
                        }
                    }
                }
                return companyArray;
            }
            else
                return [];
        },

    };
}();

$("#btnClose").click(function () {

    $("#menuNameMultiSelect").data("kendoMultiSelect").value("");
    $("#userNameTypeMultiSelect").data("kendoMultiSelect").value("");
    $("#moduleNameTypeMultiSelect").data("kendoMultiSelect").value("");


});

function SaveReportSetting(reportName, gridName) {
    var columnsNames = "";
    var columns = $("#" + gridName).data("kendoGrid").columns;
    if (columns.length > 0) {
        for (var i = 0; i < columns.length; i++) {
            var col = columns[i];
            if (col.field != undefined) {
                if (col.hidden) {
                    if (columnsNames.length < 1)
                        columnsNames = col.field;
                    else
                        columnsNames += "," + col.field;
                }
            }
        }
    }
    var value = {
        reportName: reportName,
        columnName: columnsNames
    };

    $.ajax({
        type: 'POST',
        data: value,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/Home/SaveReportSetting",
        success: function (data) {
            //displayPopupNotification("Setting Saved", "success");
        }
    });



}


function SaveReportConfigSetting(obj) {
    //get all hidden column fields
    var columnsNames = "";
    var columns = $("#" + obj.gridName).data("kendoGrid").columns;
    if (columns.length > 0) {
        for (var i = 0; i < columns.length; i++) {
            var col = columns[i];
            if (col.field != undefined) {
                if (col.hidden) {
                    if (columnsNames.length < 1)
                        columnsNames = col.field;
                    else
                        columnsNames += "," + col.field;
                }
            }
        }
    }
    //get all group fields
    var group = $("#" + obj.gridName).data("kendoGrid").dataSource.group();
    var groupNames = [], groupDir = [];
    if (group.length > 0) {
        for (var i = 0; i < group.length; i++) {
            var groupArr = group[i];
            if (groupArr.field != undefined) {
                groupNames.push(groupArr.field);
                groupDir.push(groupArr.dir);
            }
        }
    }

    obj.hiddenColumnName = columnsNames;
    obj.groupField = groupNames.join(',');
    obj.groupDir = groupDir.join(',');
    obj.dateTitle = $("#ddlDateFilterVoucher :selected").val();
    obj.modelABBR = $("a[href='/" + obj.virtualPath + "']").last().attr("data-abbr");
    obj = $.extend(obj, ReportFilter.filterAdditionalData().ReportFilters)
    console.log("reportfilter", obj);
    $.ajax({
        type: 'POST',
        data: obj,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/Home/SaveReportConfigSetting",
        success: function (data) {
            displayPopupNotification("Setting Saved", "success");
            $("#favModal").modal("hide");
        }
    });
}

function GetReportSetting(reportName) {
    var fav = Arg("fav") == undefined ? reportName : Arg("fav");
    var result = "";

    $.ajax({
        type: 'POST',
        data: { 'reportName': fav },
        async: false,
        url: window.location.protocol + "//" + window.location.host + "/Home/GetReportSetting",
        success: function (data) {

            result = data;
            var topTenReportList = ["TopSalesDealer", "TopSalesEmployee"];
            if ($.inArray(fav, topTenReportList) > -1) {
                result.defaultPageSize = result.defaultPageSize == undefined ? 10 : parseInt(result.defaultPageSize);
                $($("#itemsPerPage").parents()[1]).addClass("sr-only");
                $($("#dateFormat").parents()[1]).addClass("sr-only");
            }
            else
                result.defaultPageSize = result.defaultPageSize == undefined ? 100 : parseInt(result.defaultPageSize);

            if (result.itemPerPage != null && result.itemPerPage != "") {
                result.itemPerPage = JSON.parse("[" + result.itemPerPage + "]");
                result.itemPerPage.push('ALL');

            }
            else {
                result.itemPerPage = JSON.parse("[100,500,1000,5000,10000]");
                result.itemPerPage.push('ALL');
            }

            result.dateFormat = _.isEmpty(result.dateFormat) ? "dd-MMM-yyyy" : result.dateFormat;

            if (!_.isEmpty(result.theme)) {
                var selectedTheme = '<link href="' + window.location.protocol + "//" + window.location.host + '/Content/Kendo/styles/' + result.theme + '" rel="stylesheet" tag="kendoTheme" />';
                $("#operationContent").find("link[tag='kendoTheme']").remove();
                $("#operationContent").append(selectedTheme);
            }


            //for tooltip
            $('body').tooltip({
                selector: '.favourites'
            });
            //add fav menu
            var node = $("#operationContent").find(".page-toolbar > .btn-group > .actions");
            if (node[0] != undefined) {
                var favnode = node.find("#Savefavriote");
                if (favnode[0] == undefined) {
                    var favColor = location.hash.indexOf('?') != -1 ? "style = 'color:#ffbf00'" : "";
                    //var html = "<a class='btn btn-circle btn-icon-only btn-default' id= 'Savefavriote' onclick= 'loadFavouriteMenu()' href= 'javascript:;'><i class='fa fa-star' " + favColor + "></i></a>";
                    var html = "<a class='btn btn-circle btn-icon-only btn-default favourites' id= 'Savefavriote' onclick= 'loadFavouriteMenu()' data-toggle='tooltip' data-placement='left' data-html='true' title='Save Favourite' href= 'javascript:;'><i class='fa fa-star'" + favColor + "></i></a>";
                    node.prepend(html);
                    isFirstLoad.fav = true;
                    isFirstLoad.favGroup = true;
                    isFirstLoad.favCount = 0;


                    $("#RunQuery").click(function (evt) {
                        isFirstLoad.favGroup = true;
                    });


                }

                if (location.hash.indexOf('?') != -1 && Arg("fav") != undefined && isFirstLoad.fav) {
                    if (isFirstLoad.favCount == 1) {
                        isFirstLoad.fav = false;
                        favouriteReportSetting.init(result);
                    } else
                        isFirstLoad.favCount++;
                }



            }
        }
    });
    return result;
}


function UpdateReportUsingSetting(reportName, gridName) {

    var fav = Arg("fav") == undefined ? reportName : Arg("fav");
    var grid = $("#" + gridName).data("kendoGrid");
    try {
        $.ajax({
            type: 'POST',
            data: { 'reportName': fav },
            async: false,
            url: window.location.protocol + "//" + window.location.host + "/Home/GetReportSetting",
            success: function (data) {
                var hiddenColumns = data.hiddenColumnName.split(',');
                $.each(hiddenColumns, function (key, hiddenColumn) {
                    grid.hideColumn(hiddenColumn);
                });
                //
                var group = grid.dataSource.group();
                var groupNames = [];
                if (group.length > 0) {
                    for (var i = 0; i < group.length; i++) {
                        var groupArr = group[i];
                        if (groupArr.field != undefined) {
                            groupNames.push(groupArr.field);
                        }
                    }
                }
                var condition = location.hash.indexOf('?') != -1 && Arg("fav") != undefined && isFirstLoad.favGroup && !_.isEmpty(data.groupField) && groupNames.join(',') != data.groupField;
                if (condition) {
                    //this will load in first time only
                    var group = [];
                    var groupDir = data.groupDir.split(',');
                    $.each(data.groupField.split(','), function (key, groupColumn) {
                        group.push({ field: groupColumn, dir: groupDir[key], aggregates: grid.dataSource.options.aggregate });
                    });
                    grid.dataSource.group(group);
                    isFirstLoad.favGroup = false;


                }

            }
        });

    }
    catch (e) { }

}


var favouriteReportSetting = function () {
    return {
        init: function (reportSetting) {
            reportConfig = reportSetting;
            try {
                //
                if (reportConfig.HideOption == "Hide") {
                    $('*[data-target="#filterModal"]').hide();
                }
                else {
                    $('*[data-target="#filterModal"]').show();
                }

                //****************************** Advance Filter Start here ********************************//

                //customer
                if (reportConfig.CustomerFilter.length > 0)
                    $('#customerMultiSelect').data("kendoMultiSelect").value(reportConfig.CustomerFilter);

                //Product
                if (reportConfig.ProductFilter.length > 0)
                    $("#productMultiselect").data("kendoMultiSelect").value(reportConfig.ProductFilter);

                //DocumentFilter
                if (reportConfig.DocumentFilter.length > 0)
                    $("#documentMultiSelect").data("kendoMultiSelect").value(reportConfig.DocumentFilter);

                //PartyTypeFilter
                if (reportConfig.PartyTypeFilter.length > 0)
                    $("#partyTypeMultiSelect").data("kendoMultiSelect").value(reportConfig.PartyTypeFilter);

                //PartyTypeFilter
                if (reportConfig.AreaTypeFilter.length > 0)
                    $("#areaMultiSelects").data("kendoMultiSelect").value(reportConfig.AreaTypeFilter);

                //CategoryFilter
                if (reportConfig.CategoryFilter.length > 0)
                    $("#categoryMultiSelect").data("kendoMultiSelect").value(reportConfig.CategoryFilter);


                //LocationFilter
                if (reportConfig.LocationFilter.length > 0)
                    $("#locationMultiSelect").data("kendoMultiSelect").value(reportConfig.LocationFilter);

                //EmployeeFilter
                if (reportConfig.EmployeeFilter.length > 0)
                    $("#employeeMultiSelect").data("kendoMultiSelect").value(reportConfig.EmployeeFilter);

                //AgentFilter
                if (reportConfig.AgentFilter.length > 0)
                    $("#agentMultiSelect").data("kendoMultiSelect").value(reportConfig.AgentFilter);

                //DivisionFilter
                if (reportConfig.DivisionFilter.length > 0)
                    $("#divisionMultiSelect").data("kendoMultiSelect").value(reportConfig.DivisionFilter);


                //SalesPersonFilter
                if (reportConfig.SalesPersonFilter.length > 0)
                    $("#SalesMultiSelect").data("kendoMultiSelect").value(reportConfig.SalesPersonFilter);

                //CurrencyFilter
                if (reportConfig.CurrencyFilter.length > 0)
                    $("#currencyTypeMultiSelect").data("kendoMultiSelect").value(reportConfig.CurrencyFilter);

                //SupplierFilter
                if (reportConfig.SupplierFilter.length > 0)
                    $("#supplierMultiSelect").data("kendoMultiSelect").value(reportConfig.SupplierFilter);


                //****************************** Advance Filter End here ********************************//


                //****************************** Other Filter Start here ********************************//

                //Date
                if (reportConfig.dateTitle.length > 0 && reportConfig.dateTitle != "Custom") {
                    $("#ddlDateFilterVoucher").val(reportConfig.dateTitle);
                    $("#ddlDateFilterVoucher").trigger("change");
                } else {
                    if (reportConfig.FromDate.length > 0) {
                        $('#FromDateVoucher').val(moment(reportConfig.FromDate).format("YYYY-MMM-DD"));
                        $('#FromDateVoucher').trigger("change");
                    }
                    if (reportConfig.ToDate.length > 0) {
                        $('#ToDateVoucher').val(moment(reportConfig.ToDate).format("YYYY-MMM-DD"));
                        $('#ToDateVoucher').trigger("change");
                    }
                }



                //consolidate
                if (reportConfig.BranchFilter.length > 0) {
                    var consolidate = $("#consolidateTreeView").data("kendoTreeView");
                    $("#consolidateTreeView input").prop("checked", false).trigger("change");;
                    $.each(reportConfig.BranchFilter, function (index, obj) {
                        var branch = consolidate.findByUid(consolidate.dataSource.get(obj).uid);
                        consolidate.dataItem(branch).set("checked", true);
                    });
                    //Preference Filter
                    if (reportConfig.QuantityFigureFilter.length > 0) {
                        var item = $("#quantity-figure-range").data("ionRangeSlider");
                        if (item != undefined) {
                            var index = item.options.values.findIndex(x => x == reportConfig.QuantityFigureFilter);
                            item.update({
                                from: index,
                            });
                        }
                    }
                }

                if (reportConfig.QuantityRoundUpFilter.length > 0) {
                    var item = $("#quantity-roundUp-range").data("ionRangeSlider");
                    if (item != undefined) {
                        var index = item.options.values.findIndex(x => x == reportConfig.QuantityRoundUpFilter);
                        item.update({
                            from: index,
                        });
                    }
                }
                if (reportConfig.QuantityRangeFilter.length > 1) {
                    var value = reportConfig.QuantityRangeFilter.spit(';');
                    $("#rangeQuantity-min-val").val(value[0]);
                    $("#rangeQuantity-max-val").val(value[1]);
                }
                if (reportConfig.AmountFigureFilter.length > 0) {
                    var item = $("#amount-figure-range").data("ionRangeSlider");
                    if (item != undefined) {
                        var index = item.options.values.findIndex(x => x == reportConfig.AmountFigureFilter);
                        item.update({
                            from: index,
                        });
                    }
                }
                if (reportConfig.AmountRoundUpFilter.length > 0) {
                    var item = $("#amount-roundUp-range").data("ionRangeSlider");
                    if (item != undefined) {
                        var index = item.options.values.findIndex(x => x == reportConfig.AmountRoundUpFilter);
                        item.update({
                            from: index,
                        });
                    }
                }
                if (reportConfig.AmountRangeFilter.length > 1) {
                    var value = reportConfig.AmountRangeFilter.split(';');
                    $("#rangeAmount-min-val").val(value[0]);
                    $("#rangeAmount-max-val").val(value[1]);
                }
                if (reportConfig.RateRangeFilter.length > 1) {
                    var value = reportConfig.RateRangeFilter.split(';');
                    $("#rangeRate-min-val").val(value[0]);
                    $("#rangeRate-max-val").val(value[1]);
                }





                //****************************** Other Filter End here ********************************//


            } catch (e) {
                console.log("Error:Favourite Setting Not Loaded!!");
            }
        }
    }
}();



