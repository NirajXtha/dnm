TMModule.controller('quotationDetails', function ($scope, $rootScope, $http, $routeParams, $http, $filter, $timeout) {
    $("#englishdatedocument").kendoDatePicker();
    $scope.productFormList = [];
    $scope.termList = [];
    $scope.counterProduct = 1;
    $scope.counterTerm = 1;
    $scope.quotationNo = "";
    $scope.tenderNo = "";
    $scope.companyCode = "";
    $scope.branchCode = "";
    $scope.customerCode = "";
    $scope.createdBy = "";
    $scope.CURRENCY_RATE = 1;
    $scope.LOCAL_FLAG = 'N';
    $scope.VAT_INCLUDE_FLAG = 'Y';
    $scope.IS_POSTED = 'N';

    $scope.selectedCurrency = "";
    $scope.company = {};
    $scope.selectedDiscountType = "Percentage";
    $scope.toggleDetails = function () {
        $scope.showSpecificationDetail = !$scope.showSpecificationDetail;
    };
    $scope.showDiscountType = false;

    $scope.toggleDiscountTypePopup = function () {
        $scope.showDiscountType = !$scope.showDiscountType;
    };

    /*document.getElementById("customerName").addEventListener("change", function () {
        $scope.customerCode = null;
    });*/

    $scope.updateSelectedDiscountType = function (discountType) {
        $scope.selectedDiscountType = discountType;
        $scope.showDiscountType = false; // Hide the popup after selecting an option
    };

    var url = new URL(window.location.href);
    $scope.quotationNo = url.searchParams.get("qo");
    $http.get("/api/ApiQuotation/GetCompanyDetails?id=" + $scope.quotationNo)
        .then(function (response) {
            var company = response.data[0];
            company.LOGO_FILE_NAME = window.location.protocol + "//" + window.location.host + "/Pictures/Login/" + company.LOGO_FILE_NAME;
            $scope.company = company;
            console.table($scope.company);
        })
        .catch(function (error) {
            console.error('Error fetching company details:', error);
        });

    $scope.openImage = function (imageUrl) {
        if (imageUrl == null) {
            return;
        }
        window.open(imageUrl, '_blank');
    };
    $scope.TOTAL_QUANTITY = 0;

    /*$http.get("/api/ApiQuotation/partyDetails?id=" + $scope.quotationNo)
        .then(function (response) {
            if (!response.data || response.data.length === 0) {
                return;
            }
            var party = response.data[0];
            $scope.customerCode = party.SUPPLIER_CODE;
            $scope.selectedCurrency = party.CURRENCY;
            $scope.CURRENCY_RATE = party.CURRENCY_RATE;
            $scope.DELIVERY_DATE = new Date(party.DELIVERY_DATE);
            $scope.selectedDiscountType = party.DISCOUNT_TYPE;
            $scope.PAN_NO = party.PAN_NO;

            $scope.fetchEmployeeDetails();
        })
        .catch(function (error) {
            console.error('Error fetching Party details:', error);
        });*/

    //$http.get("/api/ApiQuotation/termsAndConditions?id=" + $scope.quotationNo + "&supCode=")
    //    .then(function (response) {
    //        if (!response.data || response.data.length === 0) {
    //            return;
    //        }
    //        var termsLists = response.data;
    //        $scope.termList = [];
    //        var termId = 1;
    //        for (var i = 0; i < termsLists.length; i++) {
    //            var term = {
    //                ID: termId,
    //                TERM_CONDITION: termsLists[i].TERM_CONDITION
    //            };
    //            termId++;
    //            $scope.termList.push(term);
    //        }
    //        console.log($scope.termList);
            
    //    })
    //    .catch(function (error) {
    //        console.error('Error fetching Terms and Conditions:', error);
    //    });
    $scope.CURRENCY = '';
    $http.get("/api/ApiQuotation/GetQuotationDetails?id=" + $scope.quotationNo)
        .then(function (response) {
            debugger;
            console.table(response);
            var quotation = response.data[0];
            $scope.LOCAL_FLAG = quotation.LOCAL_FLAG;
            $scope.TENDER_NO = quotation.TENDER_NO;
            $scope.ISSUE_DATE = formatDate(quotation.ISSUE_DATE);
            $scope.VALID_DATE = formatDate(quotation.VALID_DATE);
            $scope.NEPALI_DATE = quotation.NEPALI_DATE;
            $scope.DELIVERY_DT_BS = quotation.DELIVERY_DT_BS;
            $scope.TXT_REMARKS = quotation.REMARKS;
            $scope.companyCode = quotation.COMPANY_CODE;
            $scope.createdBy = quotation.CREATED_BY;
            $scope.branchCode = quotation.BRANCH_CODE;
                
            $scope.CURRENCY = quotation.CURRENCY;
            $scope.VAT_INCLUDE_FLAG = quotation.VAT_INCLUDE_FLAG || 'Y';
            $scope.IS_POSTED = quotation.IS_POSTED || 'N';

                var id = 1;
                var quantity = 0;
                $scope.productFormList = [];

                for (var i = 0; i < quotation.Items.length; i++) {
                    debugger;
                    var itemList = quotation.Items[i];
                    var imageUrl = null;
                    if (itemList.IMAGE && itemList.IMAGE != null) {
                        imageUrl = window.location.protocol + "//" + window.location.host + "/Areas/NeoERP.QuotationManagement/Image/Items/" + itemList.IMAGE;
                    }

                    $scope.productFormList.push(angular.copy({
                        ID: id,
                        ITEM_CODE: itemList.ITEM_DESC,
                        ITEM: itemList.ITEM_CODE,
                        SPECIFICATION: itemList.SPECIFICATION,
                        IMAGE: itemList.IMAGE,
                        IMAGE_LINK: imageUrl,
                        UNIT: itemList.UNIT,
                        QUANTITY: itemList.QUANTITY,
                        CATEGORY: itemList.CATEGORY,
                        BRAND_NAME: itemList.BRAND_NAME,
                        INTERFACE: itemList.INTERFACE,
                        TYPE: itemList.TYPE,
                        LAMINATION: itemList.LAMINATION,
                        ITEM_SIZE: itemList.ITEM_SIZE,
                        THICKNESS: itemList.THICKNESS,
                        COLOR: itemList.COLOR,
                        GRADE: itemList.GRADE,
                        SIZE_LENGHT: itemList.SIZE_LENGTH,
                        SIZE_WIDTH: itemList.SIZE_WIDTH,
                        RATE: 0 // Initialize rate
                    }));

                    quantity += itemList.QUANTITY;
                    id++;
                }

                $scope.TOTAL_QUANTITY = quantity.toFixed(2);

                return $http.get("/api/ApiQuotation/partyDetailsItems?id=" + $scope.quotationNo + "&supCode=" + $scope.customerCode + "&company=" + $scope.company.COMPANY_CODE);
            
        })
        .then(function (response) {
            if ($scope.LOCAL_FLAG == 'N' && (!$scope.customerCode || $scope.customerCode == "")) { return; }
            else {
                var rateData = response.data;

                // Match rateData by ID, ITEM_CODE and update RATE
                for (var i = 0; i < rateData.length; i++) {
                    var rateItem = rateData[i];
                    for (var j = 0; j < $scope.productFormList.length; j++) {
                        var product = $scope.productFormList[j];

                        if (product.ITEM === rateItem.ITEM_CODE) {
                            product.RATE = rateItem.RATE || 0;
                            product.DISCOUNT = rateItem.DISCOUNT || 0;
                            product.DISCOUNT_AMOUNT = rateItem.DISCOUNT_AMOUNT || 0;
                            product.EXCISE = rateItem.EXCISE || 0;

                            $scope.updateAmount(product);
                            if ($scope.updateTaxableAmt) {
                                $scope.updateTaxableAmt(product);
                            }
                            break;
                        }
                    }
                }
            }
        })

        .catch(function (error) {
            console.error('Error fetching quotation or rate details:', error);
        });



    function formatDate(dateString) {
        var date = new Date(dateString);
        return $filter('date')(date, 'dd-MMM-yyyy');
    }
    $scope.ItemSelect = {
        dataTextField: "ItemDescription",
        dataValueField: "ItemCode",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Group...</strong></div>',
        placeholder: "Select Item...",
        autoClose: true,
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/QuotationApi/ItemDetails",
                    dataType: "json"
                }
            }
        }
    };
    $scope.currencyRate = 1;
    // Fetch the currency data
    /*$http.get("https://gist.githubusercontent.com/aaronhayes/5fef481815ac75f771d37b16d16d35c9/raw/edbec8eea5cc9ace57a79409cc390b7b9bcf24f6/currencies.json")*/
    setTimeout(function () {
        $http.get("/api/ApiQuotation/getCurrency?company=" + $scope.company.COMPANY_CODE)
            .then(function (response) {
                $scope.currencyData = response.data.map(function (currency) {
                    return {
                        code: currency.CURRENCY_CODE,
                        name: currency.CURRENCY_EDESC + ` (${currency.CURRENCY_CODE})`,
                        country: currency.COUNTRY
                    };
                });

                // Initialize the Kendo DropDownList options
                $scope.currencyOptions = {
                    dataSource: $scope.currencyData,
                    dataTextField: "name",
                    dataValueField: "code",
                    height: 600,
                    optionLabel: "Select Currency",
                    filter: "contains",
                    autoClose: true,
                    dataBound: function (e) {
                        var current = this.value();
                        this._savedOld = current.slice(0);
                        $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '190px', 'scroll': 'scroll' });

                        var nepaliRupee = $scope.currencyData.find(currency => currency.code === 'NRS');
                        if (nepaliRupee) {
                            this.value(nepaliRupee.code);
                            $scope.selectedCurrency = nepaliRupee.code;
                            $scope.CURRENCY_RATE = 1;
                        }
                    },
                    change: function (e) {
                        debugger;
                        var selectedCurrency = this.value();
                        if (selectedCurrency === 'NRS') {
                            $scope.CURRENCY_RATE = 1;
                        } else {
                            $scope.CURRENCY_RATE = "";
                        }
                    }
                };
            })
            .catch(function (error) {
                console.error("Error fetching currency data:", error);
            });
    }, 500);
    

    $scope.addProduct = function () {
        $http.get("/api/QuotationApi/ItemDetails")
            .then(function (response) {
                // Assuming response.data is an array of objects with 'ItemDescription' and 'ItemCode' properties
                $scope.ItemSelect.dataSource.data = response.data;
                $scope.productFormList.push({
                    ID: $scope.counterProduct,
                    ItemDescription: "",
                    UNIT: "",
                    IMAGE: "",
                    QUANTITY: "",
                    UNIT_PRICE: "",
                    AMOUNT: "",
                    REMARKS: ""
                });
            })
            .catch(function (error) {
                console.error('Error fetching item details:', error);
            });
    };

    //$scope.addProduct();
    $scope.updateUnit = function (product) {
        if (product && product.ItemDescription) {
            // Find the item with the matching ItemCode
            var selectedItem = $scope.ItemSelect.dataSource.data.find(function (item) {
                return item.ItemCode === product.ItemDescription;
            });

            // If a matching item is found, set the UNIT to the ItemUnit of the selected item
            if (selectedItem) {
                product.UNIT = selectedItem.ItemUnit;
                product.SPECIFICATION = selectedItem.SPECIFICATION;
            }
        } else {
            product.UNIT = "";
        }
    };


    $scope.updateDiscountAmt = function (product) {
        product.DISCOUNT_AMOUNT = 0;
        if (product.DISCOUNT) {
            if ($scope.selectedDiscountType === 'Percentage') {
                product.DISCOUNT_AMOUNT = (product.DISCOUNT / 100) * product.AMOUNT;
            } else if ($scope.selectedDiscountType === 'Flat') {
                product.DISCOUNT_AMOUNT = product.DISCOUNT;
            } else if ($scope.selectedDiscountType === 'Quantity') {
                product.DISCOUNT_AMOUNT = (product.DISCOUNT * product.QUANTITY);
            }
        } else {
            product.DISCOUNT_AMOUNT = null;
        }
    };

    $scope.updateAmount = function (product) {
        if (product.RATE) {
            product.AMOUNT = (product.RATE ? product.RATE : 0) * (product.QUANTITY ? product.QUANTITY : 0);
        } else {
            product.AMOUNT = null;
        }
    };

    $scope.onVatIncludeChanged = function () {
        angular.forEach($scope.productFormList, function (product) {
            $scope.updateTaxableAmt(product);
        });
    };

    $scope.updateTaxableAmt = function (product) {
        if (product.EXCISE || product.DISCOUNT || product.AMOUNT) {
            var taxableAMt = (product.AMOUNT ? product.AMOUNT : 0) - (product.DISCOUNT_AMOUNT ? product.DISCOUNT_AMOUNT : 0) + (product.EXCISE ? product.EXCISE : 0);
            product.TAXABLE_AMOUNT = parseFloat(taxableAMt.toFixed(2));
            if ($scope.VAT_INCLUDE_FLAG == 'Y') {
                var vatAMt = (13 / 100) * product.TAXABLE_AMOUNT;
            } else {
                vatAMt = 0.00;
            }
            //var vatAMt = (13 / 100) * product.TAXABLE_AMOUNT;
            product.VAT_AMOUNT = parseFloat(vatAMt.toFixed(2));
            var netAmt = vatAMt + taxableAMt;
            product.NET_AMOUNT = parseFloat(netAmt.toFixed(2));
        } else {
            product.TAXABLE_AMOUNT = null;
        }
    };
    $scope.addRow = function () {
        var maxId = Math.max(...$scope.termList.map(term => term.ID));
        $scope.counterTerm = maxId !== -Infinity ? maxId + 1 : 1;
        if ($scope.counterTerm <= 30) {
            $scope.termList.push({
                ID: $scope.counterTerm,
                TERM_CONDITION: "",
            });
        } else {
            displayPopupNotification("Terms and Condition cannot be more than 30!!", "error")
        }

    };

    $scope.updateQuantity = function () {
        var totalQty = 0;
        angular.forEach($scope.productFormList, function (item) {
            totalQty += item.QUANTITY ?? 0;
        });
        $scope.totalQty = totalQty ?? totalQty.toFixed(2);
    };
    $scope.updateQuantity();

    $scope.PAN_NO = '';
    $scope.employee = {};

    function fetchTermsAndConditions() {
        return $http.get("/api/ApiQuotation/termsAndConditions", {
            params: {
                id: $scope.quotationNo,
                supCode: $scope.customerCode ?? ''
            }
        })
            .then(function (response) {
                if (!response.data || response.data.length === 0) {
                    return;
                }
                var termsLists = response.data;
                $scope.termList = [];
                var termId = 1;
                for (var i = 0; i < termsLists.length; i++) {
                    var term = {
                        ID: termId,
                        TERM_CONDITION: termsLists[i].TERM_CONDITION
                    };
                    termId++;
                    $scope.termList.push(term);
                }
                console.log($scope.termList);
            })
            .catch(function (error) {
                console.error('Error fetching Terms and Conditions:', error);
            });
    }
    
    $scope.fetchEmployeeDetails = function () {
            $http.get('/api/ApiQuotation/getSupplierDetails', { params: { panNo: $scope.PAN_NO ?? '', supCode: $scope.customerCode ?? '', company: $scope.company.COMPANY_CODE } })
                .then(function (response) {
                    debugger;
                    if (response.data.length > 0 && $scope.LOCAL_FLAG == 'Y') {
                    var employeeDetails = response.data[0];
                    $scope.customerCode = employeeDetails.SUPPLIER_CODE;
                    $scope.PARTY_NAME = employeeDetails.SUPPLIER_EDESC;
                    $scope.ADDRESS = employeeDetails.ADDRESS;
                    $scope.CONTACT_NO = employeeDetails.CONTACT_NO;
                    document.getElementById("contactNo").value = employeeDetails.CONTACT_NO;
                    $scope.EMAIL = employeeDetails.EMAIL;
                } else {
                    $scope.customerCode = '';
                    $scope.PARTY_NAME = "";
                    $scope.ADDRESS = "";
                    $scope.CONTACT_NO = "";
                    $scope.EMAIL = "";
                    }
                    return $http.get("/api/ApiQuotation/partyDetailsItems?id=" + $scope.quotationNo +
                        "&supCode=" + $scope.customerCode + "&company=" + $scope.company.COMPANY_CODE);
                })
                .then(function (response) {
                    debugger;
                    var rateData = response.data;
                    //var id = 1;
                    var itemMatchCount = {};

                    for (var i = 0; i < rateData.length; i++) {
                        var rateItem = rateData[i];
                        itemMatchCount[rateItem.ITEM_CODE] = itemMatchCount[rateItem.ITEM_CODE] || 0;

                        var matchCount = 0;
                        for (var j = 0; j < $scope.productFormList.length; j++) {
                            var product = $scope.productFormList[j];

                            if (product.ITEM === rateItem.ITEM_CODE) {
                                if (matchCount === itemMatchCount[rateItem.ITEM_CODE]) {
                                    debugger;
                                    product.RATE = rateItem.RATE || 0;
                                    product.DISCOUNT = rateItem.DISCOUNT || 0;
                                    product.DISCOUNT_AMOUNT = rateItem.DISCOUNT_AMOUNT || 0;
                                    product.EXCISE = rateItem.EXCISE || 0;

                                    $scope.updateAmount(product);
                                    if ($scope.updateTaxableAmt) {
                                        $scope.updateTaxableAmt(product);
                                    }
                                    itemMatchCount[rateItem.ITEM_CODE]++;
                                    break; // Move to next rateData item
                                }
                                matchCount++;
                            }
                        }
                    }
                    return fetchTermsAndConditions();

                    //for (var i = 0; i < rateData.length; i++) {
                    //    var rateItem = rateData[i];
                    //    for (var j = 0; j < $scope.productFormList.length; j++) {
                    //        var product = $scope.productFormList[j];

                    //        if (product.ITEM === rateItem.ITEM_CODE) {
                    //            product.RATE = rateItem.RATE || 0;
                    //            product.DISCOUNT = rateItem.DISCOUNT || 0;
                    //            product.DISCOUNT_AMOUNT = rateItem.DISCOUNT_AMOUNT || 0;
                    //            product.EXCISE = rateItem.EXCISE || 0;

                    //            $scope.updateAmount(product);
                    //            if ($scope.updateTaxableAmt) {
                    //                $scope.updateTaxableAmt(product);
                    //            }
                    //            id++;
                    //            break;
                    //        }
                    //    }
                    //}
                })
            .catch(function (error) {
                console.error('Error fetching Employee details:', error);
            });
    };

    $scope.fetchVendorDetails = function () {
        if ($scope.LOCAL_FLAG == 'N') {
            $http.get('/api/ApiQuotation/getSupplierDetails', { params: { panNo: $scope.PARTY_NAME ?? '', supCode: 'IMPORT', company: $scope.company.COMPANY_CODE } })
                .then(function (response) {
                    debugger;
                    if (response.data.length > 0) {
                        var employeeDetails = response.data[0];
                        $scope.customerCode = employeeDetails.SUPPLIER_CODE;
                        $scope.ADDRESS = employeeDetails.ADDRESS;
                        $scope.CONTACT_NO = employeeDetails.CONTACT_NO;
                        document.getElementById("contactNo").value = employeeDetails.CONTACT_NO;
                        $scope.EMAIL = employeeDetails.EMAIL;
                    } else {
                        $scope.customerCode = '';
                        $scope.ADDRESS = "";
                        $scope.CONTACT_NO = "";
                        $scope.EMAIL = "";
                    }
                    return $http.get("/api/ApiQuotation/partyDetailsItems?id=" + $scope.quotationNo +
                        "&supCode=" + $scope.customerCode + "&company=" + $scope.company.COMPANY_CODE);
                })
                .then(function (response) {
                    debugger;
                    var rateData = response.data;
                    var itemMatchCount = {};

                    for (var i = 0; i < rateData.length; i++) {
                        var rateItem = rateData[i];
                        itemMatchCount[rateItem.ITEM_CODE] = itemMatchCount[rateItem.ITEM_CODE] || 0;

                        var matchCount = 0;
                        for (var j = 0; j < $scope.productFormList.length; j++) {
                            var product = $scope.productFormList[j];

                            if (product.ITEM === rateItem.ITEM_CODE) {
                                if (matchCount === itemMatchCount[rateItem.ITEM_CODE]) {
                                    product.RATE = rateItem.RATE || 0;
                                    product.DISCOUNT = rateItem.DISCOUNT || 0;
                                    product.DISCOUNT_AMOUNT = rateItem.DISCOUNT_AMOUNT || 0;
                                    product.EXCISE = rateItem.EXCISE || 0;

                                    $scope.updateAmount(product);
                                    if ($scope.updateTaxableAmt) {
                                        $scope.updateTaxableAmt(product);
                                    }
                                    itemMatchCount[rateItem.ITEM_CODE]++;
                                    break; // go to next rateData
                                }
                                matchCount++;
                            }
                        }
                    }
                    return fetchTermsAndConditions();
                })
                .catch(function (error) {
                    console.error('Error fetching Employee details:', error);
                });
        }
    };

    //To show sum in summary table 
    $scope.TOTAL_AMOUNT = 0;
    $scope.TOTAL_DISCOUNT = 0;
    $scope.TOTAL_EXCISE = 0;
    $scope.TOTAL_TAXABLE_AMOUNT = 0;
    $scope.TOTAL_VAT = 0;
    $scope.TOTAL_NET_AMOUNT = 0;
    $scope.isSaving = false;

    $scope.$watch('productFormList', function (newVal, oldVal) {
        // Reset sums
        $scope.TOTAL_AMOUNT = 0;
        $scope.TOTAL_DISCOUNT = 0;
        $scope.TOTAL_EXCISE = 0;
        $scope.TOTAL_TAXABLE_AMOUNT = 0;
        $scope.TOTAL_VAT = 0;
        $scope.TOTAL_NET_AMOUNT = 0;

        // Update sums for each product
        angular.forEach(newVal, function (product) {
            $scope.TOTAL_AMOUNT += (product.AMOUNT || 0.00);
            $scope.TOTAL_DISCOUNT += (product.DISCOUNT_AMOUNT || 0.00);
            $scope.TOTAL_EXCISE += (product.EXCISE || 0.00);
            $scope.TOTAL_TAXABLE_AMOUNT += (product.TAXABLE_AMOUNT || 0.00);
        });

        // Convert sums to have two decimal places
        $scope.TOTAL_AMOUNT = ($scope.TOTAL_AMOUNT).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        $scope.TOTAL_DISCOUNT = ($scope.TOTAL_DISCOUNT).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        $scope.TOTAL_EXCISE = ($scope.TOTAL_EXCISE).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var totalTaxableAmt = $scope.TOTAL_TAXABLE_AMOUNT;
        $scope.TOTAL_TAXABLE_AMOUNT = ($scope.TOTAL_TAXABLE_AMOUNT).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

        if ($scope.VAT_INCLUDE_FLAG == 'Y') {
            var totalVatAmt = (totalTaxableAmt || 0.00) * 0.13;
        } else {
            totalVatAmt = 0.00;
        }
        //var totalVatAmt = (totalTaxableAmt || 0.00) * 0.13;
        $scope.TOTAL_VAT = (totalVatAmt).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

        // Calculate total net amount
        $scope.TOTAL_NET_AMOUNT = (totalVatAmt + totalTaxableAmt).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }, true);

    //$scope.saveData = function () {
    //    // Validate files first
    //    var files = myInventoryDropzone.getAcceptedFiles();
    //    var maxFiles = 2;
    //    var maxSizeMB = 4;
    //    var hasOversized = files.some(file => (file.size / 1024 / 1024) > maxSizeMB);

    //    if (files.length > maxFiles) {
    //        displayPopupNotification("You can only upload up to " + maxFiles + " files.", "warning");
    //        return;
    //    }
    //    if (hasOversized) {
    //        displayPopupNotification("One or more files exceed " + maxSizeMB + " MB size limit.", "warning");
    //        return;
    //    }

    //    // Process files first
    //    myInventoryDropzone.on("queuecomplete", function () {
    //        // after files finish uploading, run the quotation save logic
    //        proceedSaveData();
    //    });

    //    if (files.length > 0) {
    //        myInventoryDropzone.processQueue();
    //    } else {
    //        // no files to upload, proceed directly
    //        proceedSaveData();
    //    }
    //};

    //function proceedSaveData() {
    //    if ($scope.PAN_NO == "" && $scope.LOCAL_FLAG == 'Y') {
    //        displayPopupNotification("PAN No is required", "warning");
    //    }
    //    else if ($scope.PARTY_NAME == "") {
    //        displayPopupNotification("Party Name is required", "warning");
    //    }
    //    else if ($scope.ADDRESS == "") {
    //        displayPopupNotification("Address is required", "warning");
    //    }
    //    else if ($scope.CONTACT_NO == "") {
    //        displayPopupNotification("Contact No is required", "warning");
    //    }
    //    else if ($scope.CURRENCY_RATE == "") {
    //        displayPopupNotification("Currency Rate is required", "warning");
    //    }
    //    else if ($scope.selectedCurrency == "") {
    //        displayPopupNotification("Currency is required", "warning");
    //    }
    //    else if ($('#deliveryDt').val() == "") {
    //        displayPopupNotification("Delivery Date is required", "warning");
    //    }
    //    else {
    //        $http.get('/api/ApiQuotation/checkPAN?panNo=' + $scope.PAN_NO + '&tenderNo=' + $scope.TENDER_NO + "&company=" + $scope.company.COMPANY_CODE)
    //            .then(function (response) {
    //                var revise = response.data > 0 ? "Revised " + response.data : "";

    //                var formData = {
    //                    ID: $scope.quotationNo,
    //                    PAN_NO: $scope.PAN_NO == "" ? 000 : $scope.PAN_NO,
    //                    SUPPLIER_CODE: $scope.customerCode ?? null,
    //                    TENDER_NO: $scope.TENDER_NO,
    //                    PARTY_NAME: $scope.PARTY_NAME,
    //                    ADDRESS: $scope.ADDRESS,
    //                    CONTACT_NO: $scope.CONTACT_NO ?? document.getElementById("contactNo").value,
    //                    CUSTOMER_CODE: $scope.customerCode,
    //                    EMAIL: $scope.EMAIL || null,
    //                    CURRENCY: $scope.selectedCurrency,
    //                    CURRENCY_RATE: $scope.CURRENCY_RATE ?? 1,
    //                    DELIVERY_DATE: $('#englishdatedocument').val(),
    //                    TOTAL_AMOUNT: $scope.TOTAL_AMOUNT,
    //                    TOTAL_DISCOUNT: $scope.TOTAL_DISCOUNT,
    //                    TOTAL_EXCISE: $scope.TOTAL_EXCISE,
    //                    TOTAL_TAXABLE_AMOUNT: $scope.TOTAL_TAXABLE_AMOUNT,
    //                    TOTAL_VAT: $scope.LOCAL_FLAG == 'N' ? 0 : $scope.TOTAL_VAT,
    //                    REVISE: revise,
    //                    TOTAL_NET_AMOUNT: $scope.LOCAL_FLAG == 'N' ? $scope.TOTAL_AMOUNT : $scope.TOTAL_NET_AMOUNT,
    //                    DISCOUNT_TYPE: $scope.selectedDiscountType,
    //                    COMPANY_CODE: $scope.companyCode,
    //                    BRANCH_CODE: $scope.branchCode,
    //                    CREATED_BY: $scope.createdBy,
    //                    VAT_INCLUDE_FLAG: $scope.VAT_INCLUDE_FLAG,
    //                    Item_Detail: [],
    //                    TermsCondition: []
    //                };

    //                var rateEmpty = false;
    //                angular.forEach($scope.productFormList, function (itemDetails) {
    //                    if (itemDetails.RATE == "" || typeof itemDetails.RATE === "undefined") {
    //                        displayPopupNotification("Rate is required", "warning");
    //                        rateEmpty = true;
    //                    } else {
    //                        var itemDetail = {
    //                            ITEM_CODE: itemDetails.ITEM,
    //                            RATE: itemDetails.RATE,
    //                            AMOUNT: itemDetails.AMOUNT,
    //                            DISCOUNT: itemDetails.DISCOUNT,
    //                            DISCOUNT_AMOUNT: itemDetails.DISCOUNT_AMOUNT,
    //                            EXCISE: itemDetails.EXCISE,
    //                            TAXABLE_AMOUNT: itemDetails.TAXABLE_AMOUNT,
    //                            VAT_AMOUNT: $scope.LOCAL_FLAG == 'N' ? 0 : itemDetails.VAT_AMOUNT,
    //                            NET_AMOUNT: $scope.LOCAL_FLAG == 'N' ? (itemDetails.NET_AMOUNT - itemDetails.VAT_AMOUNT) : itemDetails.NET_AMOUNT
    //                        };
    //                        formData.Item_Detail.push(itemDetail);
    //                    }
    //                });

    //                if (!rateEmpty) {
    //                    angular.forEach($scope.termList, function (termCondition) {
    //                        if (termCondition.TERM_CONDITION.trim() !== "") {
    //                            formData.TermsCondition.push({
    //                                TERM_CONDITION: termCondition.TERM_CONDITION
    //                            });
    //                        }
    //                    });

    //                    $http.post('/api/ApiQuotation/SaveFormData', formData)
    //                        .then(function (response) {
    //                            $scope.tenderNo = response.data.data.tenderNo;
    //                            $scope.quotationNo = response.data.data.quotationNo;
    //                            $scope.Cancel();
    //                            $scope.formSubmitted = true;
    //                            var message = response.data.MESSAGE;
    //                            const urlParams = new URLSearchParams(window.location.search);
    //                            const paramValue = urlParams.get('qo');
    //                            window.location.href = '/Quotation/Message?qo=' + paramValue;
    //                        })
    //                        .catch(function (error) {
    //                            displayPopupNotification(error, "error");
    //                        });
    //                }
    //            });
    //    }
    //};

    $scope.saveData = function () {
        if ($scope.isSaving) return; // Prevent if already saving
        $scope.isSaving = true;
        if ($scope.PAN_NO == "" && $scope.LOCAL_FLAG == 'Y') {
            displayPopupNotification("PAN No is required", "warning");
        }
        else if ($scope.PARTY_NAME == "") {
            displayPopupNotification("Party Name is required", "warning");
        }
        else if ($scope.ADDRESS == "") {
            displayPopupNotification("Address is required", "warning");
        }
        else if ($scope.CONTACT_NO == "") {
            displayPopupNotification("Contact No is required", "warning");
        }
        else if ($scope.CURRENCY_RATE == "") {
            displayPopupNotification("Currency Rate is required", "warning");
        }
        else if ($scope.selectedCurrency == "") {
            displayPopupNotification("Currency is required", "warning");
        }
        else if ($('#deliveryDt').val() == "") {
            displayPopupNotification("Delivery Date is required", "warning");
        }
        else {
            $http.get('/api/ApiQuotation/checkPAN?panNo=' + $scope.PAN_NO + '&tenderNo=' + $scope.TENDER_NO + "&company=" + $scope.company.COMPANY_CODE + "&supCode=" + $scope.customerCode ?? '')
                .then(function (response) {
                    debugger;
                    if (response.data > 0) {
                        if ($scope.customerCode == null) { var revise = ""; }
                        else { var revise = "Revised " + response.data; }
                    } else {
                        var revise = "";
                    }
                    debugger;
                    var formData = {
                        ID: $scope.quotationNo,
                        PAN_NO: $scope.PAN_NO == "" ? 0 : $scope.PAN_NO,
                        SUPPLIER_CODE: $scope.customerCode ?? null,
                        TENDER_NO: $scope.TENDER_NO,
                        PARTY_NAME: $scope.PARTY_NAME,
                        ADDRESS: $scope.ADDRESS,
                        CONTACT_NO: $scope.CONTACT_NO ?? document.getElementById("contactNo").value,
                        CUSTOMER_CODE: $scope.customerCode,
                        EMAIL: $scope.EMAIL ?? null,
                        CURRENCY: $scope.selectedCurrency,
                        CURRENCY_RATE: $scope.CURRENCY_RATE ?? 1,
                        DELIVERY_DATE: $('#englishdatedocument').val(),
                        TOTAL_AMOUNT: $scope.TOTAL_AMOUNT,
                        TOTAL_DISCOUNT: $scope.TOTAL_DISCOUNT,
                        TOTAL_EXCISE: $scope.TOTAL_EXCISE,
                        TOTAL_TAXABLE_AMOUNT: $scope.TOTAL_TAXABLE_AMOUNT,
                        TOTAL_VAT: $scope.LOCAL_FLAG == 'N'? 0 : $scope.TOTAL_VAT,
                        REVISE: revise,
                        TOTAL_NET_AMOUNT: $scope.LOCAL_FLAG == 'N' ? $scope.TOTAL_AMOUNT : $scope.TOTAL_NET_AMOUNT,
                        DISCOUNT_TYPE: $scope.selectedDiscountType,
                        COMPANY_CODE: $scope.companyCode,
                        BRANCH_CODE: $scope.branchCode,
                        CREATED_BY: $scope.createdBy,
                        VAT_INCLUDE_FLAG: $scope.VAT_INCLUDE_FLAG,
                        Item_Detail: [],
                        TermsCondition: []
                    };
                    var rateEmpty = false; // Flag to check if any rate is empty

                    angular.forEach($scope.productFormList, function (itemDetails) {
                        if (itemDetails.RATE == "" || typeof itemDetails.RATE === "undefined") {
                            displayPopupNotification("Rate is required", "warning");
                            rateEmpty = true;
                        } else {
                            var itemDetail = {
                                ITEM_CODE: itemDetails.ITEM,
                                RATE: itemDetails.RATE,
                                AMOUNT: itemDetails.AMOUNT,
                                DISCOUNT: itemDetails.DISCOUNT,
                                DISCOUNT_AMOUNT: itemDetails.DISCOUNT_AMOUNT,
                                EXCISE: itemDetails.EXCISE,
                                TAXABLE_AMOUNT: itemDetails.TAXABLE_AMOUNT,
                                VAT_AMOUNT: $scope.LOCAL_FLAG == 'N' ? 0 : itemDetails.VAT_AMOUNT,
                                NET_AMOUNT: $scope.LOCAL_FLAG == 'N' ? (itemDetails.NET_AMOUNT - itemDetails.VAT_AMOUNT) : itemDetails.NET_AMOUNT
                            };
                            formData.Item_Detail.push(itemDetail);
                        }
                    });
                    if (!rateEmpty) { // Proceed with HTTP POST only if all rates are filled
                        angular.forEach($scope.termList, function (termCondition) {
                            // Check if the term and condition is not empty
                            if (termCondition.TERM_CONDITION.trim() !== "") {
                                var termConditions = {
                                    TERM_CONDITION: termCondition.TERM_CONDITION
                                };
                                formData.TermsCondition.push(termConditions);
                            }
                        });

                        $http.post('/api/ApiQuotation/SaveFormData', formData)
                            .then(function (response) {
                                $scope.tenderNo = response.data.data.tenderNo; // Access the TENDER_NO from the response
                                $scope.quotationNo = response.data.data.quotationNo;
                                $scope.companyCode = $scope.companyCode;
                                myInventoryDropzone.processQueue();
                                $scope.Cancel();
                                $scope.formSubmitted = true;
                                window.location.reload();
                                var message = response.data.MESSAGE;
                                const urlParams = new URLSearchParams(window.location.search);
                                const paramValue = urlParams.get('qo');
                                window.location.href = '/Quotation/Message?qo=' + paramValue;
                            })
                            .catch(function (error) {
                                var message = error;
                                displayPopupNotification(message, "error");
                            }).finally(function () {
                                $scope.isSaving = false; // Reset flag after request finishes
                            });
                    } else {
                        $scope.isSaving = false;
                    }
                }).catch(function () {
                    $scope.isSaving = false;
                });
        }
    };

    $scope.addTerm = function () {
        $scope.termList.push({
            ID: $scope.counterTerm,
            TERM_CONDITION: ""
        })
    };
    $scope.addTerm();
    $scope.deleteRow = function (index) {
        var deletedTerm = $scope.termList.splice(index, 1)[0];
        delete deletedTerm.ID;
        $scope.termList.splice(index, 1); // Remove the row at the specified index
        if (index === 0) {
            $scope.addTerm();
        }
    };
    $scope.Cancel = function () {
        $("#englishdatedocument").data("kendoDatePicker").value(null);
        $scope.PAN_NO = "";
        $scope.TENDER_NO = "";
        $scope.PARTY_NAME = "";
        $scope.ADDRESS = "";
        $scope.CONTACT_NO = "";
        $scope.EMAIL = "";
        $scope.ISSUE_DATE = "",
            $scope.VALID_DATE = "",
            $scope.selectedCurrency = "";
        $scope.CURRENCY_RATE = "";
        $scope.TOTAL_AMOUNT = "";
        $scope.TOTAL_DISCOUNT = "";
        $scope.TOTAL_EXCISE = "";
        $scope.TOTAL_TAXABLE_AMOUNT = "";
        $scope.TOTAL_VAT = "";
        $scope.TOTAL_NET_AMOUNT = "";
        $scope.TOTAL_QUANTITY = "";
        $scope.productFormList.forEach(function (product) {
            product.ItemDescription = '';
            product.SPECIFICATION = '';
            product.CATEGORY = '';
            product.BRAND_NAME = '';
            product.INTERFACE = '';
            product.TYPE = '';
            product.LAMINATION = '';
            product.ITEM_SIZE = '';
            product.THICKNESS = '';
            product.COLOR = '';
            product.GRADE = '';
            product.SIZE_LENGHT = '';
            product.SIZE_WIDTH = '';
            product.IMAGE = ''; // Clear the image field
            product.UNIT = '';
            product.QUANTITY = '';
            product.AMOUNT = '',
                product.DISCOUNT = '',
                product.DISCOUNT_AMOUNT = '',
                product.EXCISE = '',
                product.TAXABLE_AMOUNT = '',
                product.VAT_AMOUNT = '',
                product.NET_AMOUNT = ''
        });
        $scope.termList.forEach(function (term) {
            term.ID = '';
            term.TERM_CONDITION = '';
        });
        var fileInputs = document.querySelectorAll('input[type="file"]');
        fileInputs.forEach(function (input) {
            input.value = '';
        })
    };
    $scope.isValidEmail = function (email) {
        var emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        return emailPattern.test(email) ? 'valid' : 'invalid';
    };

});


