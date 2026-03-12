var ReportFilter = function () {
    var globalactionPageid;
    return {
       
        init: function (actionPageId) {           
            globalactionPageid = actionPageId;
           
            $("#RunQuery_"+actionPageId).on("click", function () {
                ReportFilter.placeFilterInfo();
            });

            $('#reportFilterSettings_'+actionPageId).on('hidden', function () {
                ReportFilter.placeFilterInfo();
            });

            $("#applyFilters_" + actionPageId).on("click", function () {
                // $("#reportFilterSettings_"+actionPageId).modal("toggle");
                ReportFilter.placeFilterInfo();

            });
            $("#quantity-figure-range_"+actionPageId).ionRangeSlider({
                type: "single",
                values: ["Actual", "Hundred", "Thousand"],
                grid: true,
            });

            $("#amount-figure-range_" + actionPageId).ionRangeSlider({
                type: "single",
                values: ["Actual", "Thousand", "Lakh", "Crore"],
                grid: true,

            });

            $("#quantity-roundUp-range_" + actionPageId).ionRangeSlider({
                type: "single",
                values: ["0", "0.0", "0.00", "0.000", "0.0000"],
                from: 2,
                grid: true,

            });

            $("#amount-roundUp-range_" + actionPageId).ionRangeSlider({
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

                if ($(this).attr("checked") == undefined && $(this).attr("id") == "show-quantity-check") {
                    ReportFilter.disableAmountFilter();
                }

                if ($(this).attr("checked") == undefined && $(this).attr("id") == "show-amount-check") {
                    ReportFilter.disableQuantityFilter();
                }


                if ($(this).attr("checked") == undefined && $(this).attr("id") == "user-custom-frequency-days") {
                    ReportFilter.toggleFrequencyFilter(false);
                }

                if ($(this).attr("checked") == undefined && $(this).attr("id") == "user-custom-fixed-days") {
                    ReportFilter.toggleFixedDayFilter(false);
                }
            });

            $('.icheck').on("ifToggled", function (event) {

                if ($(this).attr("id") == "show-quantity-check") {
                    if ($(this).attr("checked") == undefined && $(this).attr("id") == "show-quantity-check") {

                        ReportFilter.disableQuantityFilter();
                    }
                    else {
                        ReportFilter.enableQuantityFilter();
                    }
                }
                if ($(this).attr("id") == "show-amount-check") {
                    if ($(this).attr("checked") == undefined && $(this).attr("id") == "show-amount-check") {

                        ReportFilter.disableAmountFilter();
                    }
                    else {
                        ReportFilter.enableAmountFilter();
                    }
                }

                if ($(this).attr("id") == "user-custom-fixed-days") {
                    if ($(this).attr("checked") == undefined && $(this).attr("id") == "user-custom-fixed-days") {

                        ReportFilter.toggleFixedDayFilter(false);
                    }
                    else {
                        ReportFilter.toggleFixedDayFilter(true);
                    }
                }

                if ($(this).attr("id") == "user-custom-frequency-days") {
                    if ($(this).attr("checked") == undefined && $(this).attr("id") == "user-custom-frequency-days") {

                        ReportFilter.toggleFrequencyFilter(false);
                    }
                    else {
                        ReportFilter.toggleFrequencyFilter(true);
                    }
                }
            });
        },
        disableQuantityFilter: function () {

            $("#quantity-roundUp-range_" + globalactionPageid).data("ionRangeSlider").update({ disable: true });
            $("#quantity-figure-range_"+globalactionPageid).data("ionRangeSlider").update({ disable: true });

        },
        disableAmountFilter: function () {

            $("#amount-figure-range_"+globalactionPageid).data("ionRangeSlider").update({ disable: true });
            $("#amount-roundUp-range_"+globalactionPageid).data("ionRangeSlider").update({ disable: true });
        },
        enableQuantityFilter: function () {
            $("#quantity-roundUp-range_"+globalactionPageid).data("ionRangeSlider").update({ disable: false });
            $("#quantity-figure-range_"+globalactionPageid).data("ionRangeSlider").update({ disable: false });

        },
        enableAmountFilter: function () {

            $("#amount-figure-range_" + globalactionPageid).data("ionRangeSlider").update({ disable: false });
            $("#amount-roundUp-range_" + globalactionPageid).data("ionRangeSlider").update({ disable: false });
        },
        filterAdditionalData: function () { //abp test
            
            var actionPageId = globalactionPageid;           
            var data =
                {
                    ReportFilters: {
                        //datePickerAd_
                        FromDate: $("#FromDateVoucher_" + actionPageId).length>0?$("#FromDateVoucher_" + actionPageId).val():$("#datePickerAd_" + actionPageId).val(),
                        ToDate: $("#ToDateVoucher_" + actionPageId).val(),
                        QuantityFigureFilter: $("#show-quantity-check_" + actionPageId).length > 0 && $("#show-quantity-check_" + actionPageId).prop("checked") ? ($("#quantity-figure-range_" + actionPageId).val() == "" ? "Actual" : $("#quantity-figure-range_" + actionPageId).val()) : "",
                        QuantityRoundUpFilter: $("#show-quantity-check_" + actionPageId).length > 0 && $("#show-quantity-check_" + actionPageId).prop("checked") ? ($("#quantity-roundUp-range_" + actionPageId).val() == "" ? "0.00" : $("#quantity-roundUp-range_" + actionPageId).val()) : "",
                        AmountFigureFilter: $("#show-amount-check_" + actionPageId).length > 0 && $("#show-amount-check_" + actionPageId).prop("checked") ? ($("#amount-figure-range_" + actionPageId).val() == "" ? "Actual" : $("#amount-figure-range_" + actionPageId).val()) : "",
                        AmountRoundUpFilter: $("#show-amount-check_" + actionPageId).length > 0 && $("#show-amount-check_" + actionPageId).prop("checked") ? ($("#amount-roundUp-range_" + actionPageId).val() == "" ? "0.00" : $("#amount-roundUp-range_" + actionPageId).val()) : "",
                        AmountRangeFilter: $("#rangeAmount-min-val_" + actionPageId).length > 0 ? $("#rangeAmount-min-val_" + actionPageId).val() + ";" + $("#rangeAmount-max-val_" + actionPageId).val() : "",
                        QuantityRangeFilter: $("#rangeQuantity-min-val_" + actionPageId).length > 0 ? $("#rangeQuantity-min-val_" + actionPageId).val() + ";" + $("#rangeQuantity-max-val_" + actionPageId).val() : "",
                        RateRangeFilter: $("#rangeRate-min-val_" + actionPageId).length > 0 ? $("#rangeRate-min-val_" + actionPageId).val() + ";" + $("#rangeRate-max-val_" + actionPageId).val() : "",
                        CustomerFilter: $("#customerMultiSelect_"+actionPageId).length > 0 ? ReportFilter.getCustomerFilter() : [],
                        ProductFilter: $("#productMultiselect_"+actionPageId).length > 0 ? ReportFilter.getProductFilter() : [],
                        SupplierFilter: $("#supplierMultiSelect_"+actionPageId).length > 0 ? ReportFilter.getSupplierFilter() : [],
                        DocumentFilter: $("#documentMultiSelect_"+actionPageId).length > 0 ? ReportFilter.getDocumentFilter() : [],
                        CategoryFilter: $("#categoryMultiSelect_" + actionPageId).length > 0 ? ReportFilter.getCategoryFilter() : [],
                        EmployeeFilter: $("#employeeMultiSelect_" + actionPageId).length > 0 ? ReportFilter.getEmployeeFilter() : [],
                        AgentFilter: $("#agentMultiSelect_" + actionPageId).length > 0 ? ReportFilter.getAgentFilter() : [],
                        DivisionFilter: $("#divisionMultiSelect_" + actionPageId).length > 0 ? ReportFilter.getDivisionFilter() : [],
                        PartyTypeFilter: $("#partyTypeMultiSelect_"+actionPageId).length > 0 ? ReportFilter.getPartyTypeFilter() : [],
                        PurchaseDocumentFilter: $("#purchaseDocumentMultiSelect_"+actionPageId).length > 0 ? ReportFilter.getPurchaseDocumentFilter() : [],
                        LocationFilter: $("#locationMultiSelect_" + actionPageId).length > 0 ? ReportFilter.getLocationFilter() : [],
                        DistAreaFilter: $("#AreaFilterMultiSelect_" + actionPageId).length > 0 ? ReportFilter.getDistAreaWiseFilter() : [],
                        // CompanyFilter: $("#companyMultiSelect_"+actionPageId).length > 0 ? ReportFilter.getCompanyFilter() : [],
                        CompanyFilter:  ReportFilter.getCompanyFilter(),
                        // BranchFilter: $("#BranchMultiSelect_"+actionPageId).length > 0 ? ReportFilter.getBranchFilter() : []
                        BranchFilter: ReportFilter.getBranchFilter(),
                        FiscalYearFilter : $("#FiscalYearFilter_"+actionPageId).length > 0 ? ReportFilter.getFiscalYearFilter() : [],
                    }
                };

            return data;
        },
        parseDecimalPlace: function (key) {
            var value = ReportFilter.getKeyValueMap(key);
            if (value == "") { return 2; }

            if (value.indexOf(".") == -1 && parseInt(value) == 0) { return 0; }

            var decimalPlaces = value.toString().split(".");
            if (decimalPlaces.length >= 2) {
                return decimalPlaces[1].length;
            }
            return 2;
        },
        getKeyValueMap: function (key) {
            return ReportFilter.filterAdditionalData().ReportFilters[key];
        },
        getCustomerFilter: function () {
            var customerIds = [];
            var multiselect = $("#customerMultiSelect_" + globalactionPageid).data("kendoMultiSelect");
            customerIds = multiselect.value();
            return customerIds;
        },
        getEmployeeFilter: function () {           
            var employeeIds = [];
            var multiselect = $("#employeeMultiSelect_" + globalactionPageid).data("kendoMultiSelect");
            employeeIds = multiselect.value();
            return employeeIds;
        },
        getAgentFilter: function () {           
            var agentIds = [];
            var multiselect = $("#agentMultiSelect_" + globalactionPageid).data("kendoMultiSelect");
            agentIds = multiselect.value();
            return agentIds;
        },
        getDivisionFilter: function () {            
            var divisionIds = [];
            var multiselect = $("#divisionMultiSelect_" + globalactionPageid).data("kendoMultiSelect");
            divisionIds = multiselect.value();
            return divisionIds;
        },
        getProductFilter: function () {
            var productIds = [];
            var multiselect = $("#productMultiselect_" + globalactionPageid).data("kendoMultiSelect");
            productIds = multiselect.value();
            return productIds;
        },
        getSupplierFilter: function () {
            var supplierIds = [];
            var multiselect = $("#supplierMultiSelect_" + globalactionPageid).data("kendoMultiSelect");
            supplierIds = multiselect.value();
            return supplierIds;
        },
        getDocumentFilter: function () {
            var documentIds = [];
            var multiselect = $("#documentMultiSelect_" + globalactionPageid).data("kendoMultiSelect");
            documentIds = multiselect.value();
            return documentIds;
        },
        getPurchaseDocumentFilter: function () {
            var purchaseDocumentIds = [];
            var multiselect = $("#purchaseDocumentMultiSelect_" + globalactionPageid).data("kendoMultiSelect");
            purchaseDocumentIds = multiselect.value();
            return purchaseDocumentIds;
        },
        getCategoryFilter: function () {
            var categoryIds = [];
            var multiselect = $("#categoryMultiSelect_" + globalactionPageid).data("kendoMultiSelect");
            categoryIds = multiselect.value();
            return categoryIds;
        },
        getPartyTypeFilter: function () {
            var partyTypeIds = [];
            var multiselect = $("#partyTypeMultiSelect_" + globalactionPageid).data("kendoMultiSelect");
            partyTypeIds = multiselect.value();
            return partyTypeIds;
        },
        getLocationFilter: function() {
            var locationIds = [];
            var multiselect = $("#locationMultiSelect_" + globalactionPageid).data("kendoMultiSelect");
            locationIds = multiselect.value();
            return locationIds;
        },
        getDistAreaWiseFilter: function () {
            var locationIds = [];
            var multiselect = $("#AreaFilterMultiSelect_" + globalactionPageid).data("kendoMultiSelect");
            distAreasIds = multiselect.value();
            return distAreasIds;
        },
        getLocationFilter: function() {
            var FiscalYearIds = [];
            var multiselect = $("#FiscalYearMultiSelect_" + globalactionPageid).data("kendoMultiSelect");
            FiscalYearIds = multiselect.value();
            return FiscalYearIds;
        },
        getFiscalYearFilter: function () {
            var FiscalYearIds = [];
            var multiselect = $("#FiscalYearMultiSelect_" + globalactionPageid).data("kendoMultiSelect");
            FiscalYearIds = multiselect.value();
            return FiscalYearIds;
        },
        getCompanyFilter: function () {          
            var companyList = $("#consolidateTreeView_" + globalactionPageid).data("kendoTreeView");
            if ($("#companyMultiSelect_" + globalactionPageid).length > 0) {
                var companyIds = [];
                var multiselect = $("#companyMultiSelect_"+ globalactionPageid).data("kendoMultiSelect");
                companyIds = multiselect.value();
                return companyIds;
            } else if (companyList != undefined) {                
                var items = getCheckedItemList(companyList);
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
                return _.uniq(companyCodeArray);
            } else
                return [];
        },
        getBranchFilter: function () {            
            var branchList = $("#consolidateTreeView_" + globalactionPageid).data("kendoTreeView");
          
            if ($("#BranchMultiSelect_" + globalactionPageid).length > 0) {
                var branchIds = [];
                var multiselect = $("#BranchMultiSelect_" + globalactionPageid).data("kendoMultiSelect");
                branchIds = multiselect.value();
                return branchIds;
            }
            else if (branchList != undefined) {
                var kendoData = $("#consolidateTreeView_" + globalactionPageid).data("kendoTreeView");
                var items = getCheckedItemList(kendoData);
                var branchCodeArray = [];
                if (items.length != 0) {
                    var j = 0;
                    for (var i = 0; i < items.length; i++) {
                        if (items[i].branch_Code.indexOf('.') != -1) {
                            branchCodeArray[j] = items[i].branch_Code;
                            j++;
                        }
                    }
                }
                return branchCodeArray;
            }
            else
                return [];

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

            //$("#filter-info").html("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'></button><p>" + filterNote + "</p>")
           
            displayBarNotification(filterNote, "info");

        },
        getCheckedItemList: function (treeview) {
            if (treeview != undefined) {
                var nodes = treeview.dataSource.view();
                return getCheckedNode(nodes);
            } else {
                return [];
            }
        },

    };
}();
/* this is bikalp change */
function getCheckedItemList(treeview) {
    
    if (treeview != undefined) {
        var nodes = treeview.dataSource.view();
        return getCheckedNode(nodes);
    } else {
        return [];
    }
}

function getCheckedNode(nodes) {
    var node, childCheckedNodes;
    var checkedNodes = [];

    for (var i = 0; i < nodes.length; i++) {
        node = nodes[i];
        if (node.checked) {
            checkedNodes.push(node);
        }

        // to understand recursion, first
        // you must understand recursion
        if (node.hasChildren) {
            childCheckedNodes = getCheckedNode(node.children.view());
            if (childCheckedNodes.length > 0) {
                checkedNodes = checkedNodes.concat(childCheckedNodes);
            }
        }

    }

    return checkedNodes;
}


/* this is bikalp change End */