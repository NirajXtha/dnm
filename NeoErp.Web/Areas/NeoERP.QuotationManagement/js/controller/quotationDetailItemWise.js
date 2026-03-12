QMModule.controller('quotationDetailItemWise', function ($scope, $rootScope, $http, $filter, $routeParams, $window) {

    // Initialize scope variables
    $scope.productFormList = [];
    $scope.selectedProducts = [];
    $scope.productList = [];
    $scope.currencyData = []; // Declare currencyData on $scope
    $scope.currencyOptions = {};
    $scope.termList = [];
    $scope.counterProduct = 1;
    $scope.STATUS = 'Pending';
    $scope.quotationDetails = false;
    $scope.showSpecificationDetail = false;
    $scope.CURRENCY = '';
    var uniqueVendors = [];
    $scope.amountinword = "";
    $scope.COMPANY_NAME = "";
    $scope.COMPANY_ADDRESS = "";
    $scope.IS_APPROVED = "N";
    $scope.IS_ALL_APPROVED = "N";
    $scope.IS_FULL_RECOMMENDATION = "N";
    $scope.IS_SELF_RECOMMEND = "N";
    $scope.itemId = "";
    $scope.net_amt = 0;
    $scope.RECYCLE = false;
    $scope.isProcessing = false;
    $scope.toggleDetails = function () {
        $scope.showSpecificationDetail = !$scope.showSpecificationDetail;
    };
    $scope.MultiPartyPrint = false;
    $scope.BtnClicked = false;
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

    $scope.openRemarksPrompt = function (title, callback) {
        $scope.remarksTitle = title;
        $scope.remarksText = "";
        $scope._remarksCallback = callback;

        $("#remarksModal").modal("show");
    };

    $scope.confirmWithRemarks = function () {
        $("#remarksModal").modal("hide");

        if (typeof $scope._remarksCallback === "function") {
            $scope._remarksCallback($scope.remarksText);
        }
    };

    $scope.openImage = function (imageUrl) {
        if (imageUrl == null) {
            return;
        }
        window.open(imageUrl, '_blank');
    };

    $scope.changePrintTemplate = function () {
        $http.get('/api/QuotationApi/QuotationDetailsId', {
            params: {
                quotationNo: '',
                tenderNo: $scope.tenderNo
            }
        }).then(function (response) {
            if (response.data && response.data.length > 0) {
                $scope.master = response.data[0];
                $scope.vendors = response.data[0].Vendors || [];

                $scope.MultiPartyPrint = !$scope.MultiPartyPrint;
            }

        }).catch(function (err) {
            console.error('Print load failed', err);
        });
    };

    $scope.quotationNo = "";
    $scope.tenderNo = "";
    if ($routeParams.quotationNo != undefined) {
        $scope.quotationNo = $routeParams.quotationNo;

    }
    else { $scope.quotationNo = "undefined"; }

    if ($routeParams.tenderNo != undefined) {
        //$scope.tenderNo = $routeParams.tenderNo.split(new RegExp('_', 'i')).join('/');
        $scope.tenderNo = $routeParams.tenderNo;
    }
    else { $scope.tenderNo = "undefined"; }

    $http.get('/api/QuotationApi/QuotationDetailsById?quotationNo=' + $scope.quotationNo + '&tenderNo=' + $scope.tenderNo)
        .then(function (response) {
            var quotation = response.data[0];
            $scope.QUOTATION_NO = quotation.QUOTATION_NO;
            $scope.PAN_NO = quotation.PAN_NO == 0 ? "" : quotation.PAN_NO;
            $scope.PARTY_NAME = quotation.PARTY_NAME;
            $scope.ADDRESS = quotation.ADDRESS;
            $scope.CONTACT_NO = quotation.CONTACT_NO;
            $scope.EMAIL = quotation.EMAIL;
            $scope.CURRENCY_RATE = quotation.CURRENCY_RATE;
            $scope.CURRENCY = quotation.CURRENCY;
            $scope.DELIVERY_DATE = formatDate(quotation.DELIVERY_DATE);
            $scope.TENDER_NO = quotation.TENDER_NO;
            $scope.ISSUE_DATE = formatDate(quotation.ISSUE_DATE);
            $scope.VALID_DATE = formatDate(quotation.VALID_DATE);
            $scope.NEPALI_DATE = quotation.NEPALI_DATE;
            $scope.DELIVERY_DT_BS = quotation.DELIVERY_DT_BS;
            $scope.DELIVERY_DT_NEP = quotation.DELIVERY_DT_NEP;
            $scope.TXT_REMARKS = quotation.REMARKS;
            $scope.STATUS = quotation.STATUS;
            $scope.DISCOUNT_TYPE = quotation.DISCOUNT_TYPE;
            $scope.CREATED_BY = quotation.CREATED_BY;
            $scope.APPROVED_BY = quotation.APPROVED_BY;
            $scope.IS_APPROVED = quotation.IS_APPROVED;
            $scope.IS_ALL_APPROVED = quotation.IS_ALL_APPROVED;
            $scope.IS_FULL_RECOMMENDATION = quotation.IS_FULL_RECOMMENDATION;
            $scope.IS_SELF_RECOMMEND = quotation.IS_SELF_RECOMMEND;
            $scope.QUOTATION_APPROVAL_AMOUNT = Math.floor(quotation.TOTAL_AMOUNT);
            $http.get('/api/QuotationApi/UserAccess?amount=' + $scope.QUOTATION_APPROVAL_AMOUNT)
                .then(function (res) {
                    $scope.APPROVALBYAMOUNT = res.data[0].APPROVE_FLAG;
                    $scope.CHECK = res.data[0].CHECK_FLAG;
                    $scope.VERIFY = res.data[0].VERIFY_FLAG;
                    $scope.RECOMMEND = res.data[0].RECOMMEND_FLAG;
                    $scope.POST = res.data[0].POST_FLAG;
                    $scope.RECYCLE = res.data[0].RECYCLE;
                });
            // Format amounts
            $scope.TOTAL_AMOUNT = quotation.TOTAL_AMOUNT.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            $scope.TOTAL_DISCOUNT = quotation.TOTAL_DISCOUNT.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            $scope.TOTAL_EXCISE = quotation.TOTAL_EXCISE.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            $scope.TOTAL_TAXABLE_AMOUNT = quotation.TOTAL_TAXABLE_AMOUNT.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            $scope.TOTAL_VAT = quotation.TOTAL_VAT.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            $scope.TOTAL_NET_AMOUNT = quotation.TOTAL_NET_AMOUNT.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            $scope.net_amt = quotation.TOTAL_NET_AMOUNT;
            $scope.amountinword = convertNumberToWords(quotation.TOTAL_NET_AMOUNT);

            var id = 1;
            var idTerm = 1;
            var quantity = 0;

            $scope.productFormList = [];
            $scope.termList = [];
            for (var i = 0; i < quotation.Item_Detail.length; i++) {
                var itemList = quotation.Item_Detail[i];
                var imageUrl = null;
                if (itemList.IMAGE != null) {
                    imageUrl = window.location.protocol + "//" + window.location.host + "/Areas/NeoERP.QuotationManagement/Image/Items/" + itemList.IMAGE;
                }

                $scope.productFormList.push({
                    TID: itemList.ID,
                    ID: id,
                    ItemDescription: itemList.ITEM_CODE,
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
                    SIZE_LENGTH: itemList.SIZE_LENGTH,
                    SIZE_WIDTH: itemList.SIZE_WIDTH,
                    RATE: itemList.RATE.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
                    AMOUNT: itemList.AMOUNT.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
                    DISCOUNT: itemList.DISCOUNT,
                    DISCOUNT_AMOUNT: itemList.DISCOUNT_AMOUNT.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
                    EXCISE: itemList.EXCISE.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
                    TAXABLE_AMOUNT: itemList.TAXABLE_AMOUNT.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
                    ACTUAL_PRICE: ((itemList.TAXABLE_AMOUNT - itemList.EXCISE) / itemList.QUANTITY).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
                    VAT_AMOUNT: itemList.VAT_AMOUNT.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
                    NET_AMOUNT: itemList.NET_AMOUNT.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
                    isSelected: (itemList.ISSELECTED === "TRUE"),
                    isApproved: (itemList.IS_APPROVED === "TRUE"),
                    isItemApproved: (itemList.IS_ITEM_APPROVED === "TRUE"),
                    isRecommended: (itemList.IS_RECOMMENDED === "TRUE"),
                });
                quantity += itemList.QUANTITY;
                id++;
            }
            for (var i = 0; i < quotation.TermsCondition.length; i++) {
                var termList = quotation.TermsCondition[i];
                $scope.termList.push({
                    ID: idTerm,
                    TERM_CONDITION: termList.TERM_CONDITION,
                });
                idTerm++;
            }
            $scope.TOTAL_QUANTITY = quantity;

            setTimeout(function () {
                for (let i = 0; i < quotation.Item_Detail.length; i++) {
                    var currentItem = quotation.Item_Detail[i];
                    var currentItemCode = currentItem.ITEM_CODE;
                    // Check if the element exists before attempting to trigger the select event
                    var dropdownElement = $("#item_" + id).data("kendoDropDownList");
                    if (dropdownElement) {
                        dropdownElement.value(currentItemCode);
                    }
                    id++;
                }
            }, 200);

            var imageurl = [];
            var imageslistcount = "";
            if (quotation.IMAGES_LIST != null || quotation.IMAGES_LIST != undefined) {
                imageslistcount = quotation.IMAGES_LIST.length;

                $.each(quotation.IMAGES_LIST, function (key, value) {
                    var filepath = value.DOCUMENT_FILE_NAME;
                    var path = filepath.replace(/[/]/g, '_');
                    imageurl.push(path);
                });
                if (imageurl.length > 0) {
                    for (var i = 0; i < imageurl.length; i++) {
                        var mockFile = {
                            name: quotation.IMAGES_LIST[i].DOCUMENT_NAME,
                            size: 12345,
                            type: 'image/jpeg',
                            url: imageurl[i],
                            accepted: true,
                        };
                        if (i == 0) {
                            myInventoryDropzone.on("addedfile", function (file) {
                                caption = file.caption == undefined ? "" : file.caption;
                                file._captionLabel = Dropzone.createElement("<a class='fa fa-download dropzone-download' href='" + imageurl[i] + "' name='Download' class='dropzone_caption' download></a>");
                                file.previewElement.appendChild(file._captionLabel);
                            });
                        }
                        myInventoryDropzone.emit("addedfile", mockFile);
                        myInventoryDropzone.emit("thumbnail", mockFile, imageurl[i]);
                        myInventoryDropzone.emit('complete', mockFile);
                        myInventoryDropzone.files.push(mockFile);
                        $('.dz-details').find('img').addClass('sr-only');
                        $('.dz-remove').css("display", "none");
                    }
                }
            }

            /*return $http.get("/api/QuotationApi/getCurrency");*/
        })
    /*.then(function (response) {
        $scope.currencyData = response.data;

        // Find and set the currency name
        var currency = $scope.currencyData.find(function (item) {
            return item.CURRENCY_CODE === $scope.CURRENCY;
        });

        if (currency) {
            $scope.CURRENCY = currency.CURRENCY_EDESC;
        } else {
            $scope.CURRENCY = "Currency not found";
        }
    });*/
    $scope.updateSelected = function () {
        $scope.selectedProducts = $scope.productFormList.filter(p => p.isSelected);
    };
    $scope.toggleAll = function () {
        angular.forEach($scope.productFormList, function (p) {
            p.isSelected = $scope.selectAll;
        });
    };

    $scope.canShowRecommend = function () {
        if ($scope.RECOMMEND !== 'Y') return false;

        if ($scope.net_amt >= 50000) {
            if ($scope.IS_SELF_RECOMMEND === 'Y')
                return false;
            return $scope.IS_FULL_RECOMMENDATION === 'N' &&
                ['Pending', 'Checked', 'Verified', 'Recommended'].includes($scope.STATUS);
        } else {
            return ['Pending', 'Checked', 'Verified'].includes($scope.STATUS);
        }
    };
    function formatDate(dateString) {
        var date = new Date(dateString);
        return $filter('date')(date, 'dd-MMM-yyyy');
    }
    $scope.AcceptEvent = function (flag) {
        let selectedItems = $scope.productFormList.filter(p => p.isSelected);
        if (flag == 'Approved') {
            if ($scope.net_amt >= 50000) {
                let allRecommended = selectedItems.every(p => p.isRecommended === true);
                if (!allRecommended) {
                    displayPopupNotification("All selected items must be recommended before approval!", "error");
                    return;
                }
                if ($scope.IS_FULL_RECOMMENDATION == 'N') {
                    displayPopupNotification("Requires 4 recommendations before approval.", "error");
                    return;
                }
            }
            let allApproved = selectedItems.some(p => p.isItemApproved === true);
            if (allApproved) {
                displayPopupNotification("Some selected items are already approved!", "error");
                return;
            }
        }
        $scope.openRemarksPrompt(flag + " Quotation", function (remarks) {
            $scope._acceptEventInternal(flag, remarks, selectedItems);
        });
    };
    $scope._acceptEventInternal = function (flag, remarks, selectedItems) {
        if ($scope.isProcessing) return;
        $scope.isProcessing = true;
        let quotationNo = $scope.quotationNo;
        let status = '';

        switch (flag) {
            case 'Approved': status = 'AP'; break;
            case 'Verify': status = 'V'; break;
            case 'Checked': status = 'C'; break;
            case 'Recommended': status = 'RE'; break;
            case 'Posted': status = 'P'; break;
        }

        let itemCodes = selectedItems.map(p => p.ItemDescription).join(",");
        $scope.itemId = selectedItems.map(i => i.TID).join(",");

        if (!itemCodes) {
            displayPopupNotification("Select at least 1 item!", "error");
            return;
        }

        $http.post('/api/QuotationApi/updateQuotationStatus', {
                quotationNo: quotationNo,
                status: status,
                type: flag,
                items: itemCodes,
                itemId: $scope.itemId,
                REMARKS: remarks
        }).then(function () {
            displayPopupNotification(flag + " successful!", "success");
            $window.location.href = "/QuotationManagement/Home/Index#!QM/SummaryReport";
        }).catch(function () {
            displayPopupNotification("Action failed!", "error");
            $scope.isProcessing = false;
        });
    };
    $scope.RejectEvent = function () {
        $scope.openRemarksPrompt("Reject Quotation", function (remarks) {
            console.log(remarks);
            $http.post('/api/QuotationApi/updateQuotationStatus', {
                quotationNo: $scope.QUOTATION_NO,
                status: 'R',
                type: '',
                items: '',
                itemId: '',
                REMARKS: remarks
            }).then(function () {
                displayPopupNotification("Quotation Rejected!", "success");
                $window.location.href = "/QuotationManagement/Home/Index#!QM/SummaryReport";
            });
        });
    };
    $scope.Recycle = function () {
        $scope.openRemarksPrompt("Recycle Quotation", function (remarks) {

            let selectedItems = $scope.productFormList.filter(p => p.isSelected);
            let itemCodes = selectedItems.map(p => p.ItemDescription).join(",");
            $scope.itemId = selectedItems.map(i => i.TID).join(",");

            if (!itemCodes) {
                displayPopupNotification("Select at least 1 item!", "error");
                return;
            }

            $http.post('/api/QuotationApi/updateQuotationStatus', {
                    quotationNo: $scope.QUOTATION_NO,
                    status: 'RR',
                    items: itemCodes,
                    itemId: $scope.itemId,
                    REMARKS: remarks
            }).then(function () {
                displayPopupNotification("Quotation Recycled!", "success");
                $window.location.href = "/QuotationManagement/Home/Index#!QM/SummaryReport";
            });
        });
    };

    //$scope.AcceptEvent = function (flag) {
    //    let selectedItems = $scope.productFormList.filter(p => p.isSelected);
    //    var quotationNo = $scope.quotationNo;
    //    var status = '';
    //    switch (flag) {
    //        case 'Approved':
    //            status = 'AP';
    //            if ($scope.net_amt >= 50000) {
    //                let allRecommended = selectedItems.every(p => p.isRecommended === true);
    //                if (!allRecommended) {
    //                    displayPopupNotification("All selected items must be recommended before approval!", "error");
    //                    return;
    //                }
    //                if ($scope.IS_FULL_RECOMMENDATION == 'N') {
    //                    displayPopupNotification("Requires 4 recommendations before approval.", "error");
    //                    return;
    //                }
    //            }
    //            let allApproved = selectedItems.some(p => p.isItemApproved === true);
    //            if (allApproved) {
    //                displayPopupNotification("Some selected items are already approved!", "error");
    //                return;
    //            }
    //            break;
    //        case 'Verify':
    //            status = 'V';
    //            break;
    //        case 'Rejected':
    //            status = 'R';
    //            break;
    //        case 'Checked':
    //            status = 'C';
    //            break;
    //        case 'Recommended':
    //            status = 'RE';
    //            break;
    //        case 'Posted':
    //            status = 'P';
    //            break;
    //        default:
    //            status = 'AP';
    //            break;
    //    }

    //    // join item codes with a comma
    //    let itemCodes = selectedItems.map(p => p.ItemDescription).join(",");
    //    $scope.itemId = selectedItems.map(i => i.TID).join(",");

    //    console.log("Selected Item Codes:", itemCodes);
    //    if (!itemCodes || itemCodes == null || itemCodes == "") {
    //        displayPopupNotification("Select atleast 1 item to proceed!", "error");
    //        return;
    //    }
    //    setTimeout(function () {
    //        if ($scope.BtnClicked == false) {
    //            $scope.BtnClicked = true;
    //            $http.post('/api/QuotationApi/updateQuotationStatus?quotationNo=' + quotationNo + '&status=' + status + '&type=' + flag + '&items=' + itemCodes + '&itemId=' + $scope.itemId + '&Remarks=')
    //                .then(function (response) {
    //                    displayPopupNotification("Quotation Accepted!!", "success");
    //                    var landingUrl = window.location.protocol + "//" + window.location.host + "/QuotationManagement/Home/Index#!QM/SummaryReport";
    //                    setTimeout(function () {
    //                        $window.location.href = landingUrl;
    //                    }, 1000);
    //                }).catch(function (error) {
    //                    var err = error.message;
    //                    var message = 'Failed to accept Quotation!!'; // Extract message from response
    //                    displayPopupNotification(message, "error");
    //                    setTimeout(function () {
    //                        window.location.reload();
    //                    }, 2000);
    //                });
    //        }
    //    }, 5);
    //}
    //$scope.RejectEvent = function () {
    //    var quotationNo = $scope.QUOTATION_NO;
    //    var status = 'R';
    //    $http.post('/api/QuotationApi/updateQuotationStatus?quotationNo=' + quotationNo + '&status=' + status + "&type=&items=&itemId=&Remarks=")
    //        .then(function (response) {
    //            displayPopupNotification("Quotation Rejected!!", "success");
    //            var landingUrl = window.location.protocol + "//" + window.location.host + "/QuotationManagement/Home/Index#!QM/SummaryReport";
    //            //$window.location.href = landingUrl;
    //            setTimeout(function () {
    //                $window.location.href = landingUrl;
    //            }, 1000);
    //        }).catch(function (error) {
    //            var message = 'Failed to reject Quotation!!';
    //            displayPopupNotification(message, "error");
    //            setTimeout(function () {
    //                window.location.reload();
    //            }, 2000);
    //        });
    //}
    //$scope.Recycle = function () {
    //    var quotationNo = $scope.QUOTATION_NO;
    //    var status = 'RR';
    //    let selectedItems = $scope.productFormList.filter(p => p.isSelected);
    //    let itemCodes = selectedItems.map(p => p.ItemDescription).join(",");
    //    $scope.itemId = selectedItems.map(i => i.TID).join(",");

    //    console.log("Selected Item Codes:", itemCodes);
    //    if (!itemCodes || itemCodes == null || itemCodes == "") {
    //        displayPopupNotification("Select atleast 1 item to proceed!", "error");
    //        return;
    //    }
    //    console.log("Quotation No: ", quotationNo);
    //    $http.post('/api/QuotationApi/updateQuotationStatus?quotationNo=' + quotationNo + '&status=' + status + "&type=&items=" + itemCodes + '&itemId=' + $scope.itemId + '&Remarks=')
    //        .then(function (response) {
    //            displayPopupNotification("Quotation Recycled!!", "success");
    //            var landingUrl = window.location.protocol + "//" + window.location.host + "/QuotationManagement/Home/Index#!QM/SummaryReport";
    //            //$window.location.href = landingUrl;
    //            setTimeout(function () {
    //                $window.location.href = landingUrl;
    //            }, 1000);
    //        }).catch(function (error) {
    //            var message = 'Failed to reject Quotation!!';
    //            displayPopupNotification(error.data.MESSAGE, "error");
    //            setTimeout(function () {
    //                window.location.reload();
    //            }, 2000);
    //        });
    //}
    $scope.chunkedProducts = [];
    let chunkSize = 10;
    $scope.isPrint = false;
    $scope.printPage = function () {
        setTimeout(function () {
            $http.get('/api/QuotationApi/QuotationDetailsId?quotationNo=' + $scope.quotationNo + '&tenderNo=' + $scope.tenderNo)
                .then(function (response) {
                    var quotation = response.data[0];
                    $scope.QUOTATION_NO = quotation.TENDER_NO;
                    $scope.PAN_NO = quotation.PAN_NO == '0' ? '' : quotation.PAN_NO;
                    $scope.PARTY_NAME = quotation.PARTY_NAME;
                    $scope.ADDRESS = quotation.ADDRESS;
                    $scope.CONTACT_NO = quotation.CONTACT_NO;
                    $scope.EMAIL = quotation.EMAIL;
                    $scope.CURRENCY_RATE = quotation.CURRENCY_RATE;
                    $scope.CURRENCY = quotation.CURRENCY;
                    $scope.DELIVERY_DATE = formatDate(quotation.DELIVERY_DATE);
                    $scope.TENDER_NO = quotation.TENDER_NO;
                    $scope.ISSUE_DATE = formatDate(quotation.ISSUE_DATE);
                    $scope.VALID_DATE = formatDate(quotation.VALID_DATE);
                    $scope.NEPALI_DATE = quotation.NEPALI_DATE;
                    $scope.TXT_REMARKS = quotation.REMARKS;
                    $scope.STATUS = quotation.STATUS;
                    $scope.DISCOUNT_TYPE = quotation.DISCOUNT_TYPE;
                    $scope.CREATED_BY = quotation.CREATED_BY;
                    $scope.APPROVED_BY = quotation.APPROVED_BY;
                    $scope.CHECKED_BY = quotation.CHECKED_BY;
                    $scope.RECOMMEND_BY = quotation.RECOMMEND_BY;
                    $scope.VERIFIED_BY = quotation.VERIFIED_BY;
                    $scope.POSTED_BY = quotation.POSTED_BY;
                    $scope.TOTAL_AMOUNT = (quotation.TOTAL_AMOUNT).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    $scope.TOTAL_DISCOUNT = (quotation.TOTAL_DISCOUNT).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    $scope.TOTAL_EXCISE = (quotation.TOTAL_EXCISE).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    $scope.TOTAL_TAXABLE_AMOUNT = (quotation.TOTAL_TAXABLE_AMOUNT).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    $scope.TOTAL_VAT = (quotation.TOTAL_VAT).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    $scope.TOTAL_NET_AMOUNT = (quotation.TOTAL_NET_AMOUNT).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    $scope.amountinword = convertNumberToWords(quotation.TOTAL_NET_AMOUNT);
                    $http.get('/api/QuotationApi/GetCompany')
                        .then(function (res) {
                            $scope.COMPANY_NAME = res.data[0].COMPANY_EDESC;
                            $scope.COMPANY_ADDRESS = res.data[0].ADDRESS;
                        });
                    var id = 1;
                    var idTerm = 1;
                    var quantity = 0;
                    $scope.TOTAL_AMOUNT = 0;
                    $scope.TOTAL_DISCOUNT = 0;
                    $scope.TOTAL_EXCISE = 0;
                    $scope.TOTAL_TAXABLE_AMOUNT = 0;
                    $scope.TOTAL_VAT = 0;
                    $scope.TOTAL_NET_AMOUNT = 0;

                    $scope.productList = [];
                    $scope.termList = [];
                    for (var i = 0; i < quotation.Item_Detail.length; i++) {
                        var itemList = quotation.Item_Detail[i];
                        var imageUrl = window.location.protocol + "//" + window.location.host + "/Areas/NeoERP.QuotationManagement/Image/Items/" + itemList.IMAGE;

                        $scope.TOTAL_AMOUNT += itemList.AMOUNT;
                        $scope.TOTAL_DISCOUNT += itemList.DISCOUNT_AMOUNT;
                        $scope.TOTAL_EXCISE += itemList.EXCISE;
                        $scope.TOTAL_TAXABLE_AMOUNT += itemList.TAXABLE_AMOUNT;
                        $scope.TOTAL_VAT += itemList.VAT_AMOUNT;
                        $scope.TOTAL_NET_AMOUNT += itemList.NET_AMOUNT;

                        $scope.productList.push({
                            TID: itemList.ID,
                            ID: id,
                            ItemDescription: itemList.ITEM_CODE,
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
                            SIZE_LENGTH: itemList.SIZE_LENGTH,
                            SIZE_WIDTH: itemList.SIZE_WIDTH,
                            RATE: (itemList.RATE).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
                            AMOUNT: (itemList.AMOUNT).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
                            DISCOUNT: itemList.DISCOUNT,
                            DISCOUNT_AMOUNT: (itemList.DISCOUNT_AMOUNT).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
                            EXCISE: (itemList.EXCISE).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
                            TAXABLE_AMOUNT: (itemList.TAXABLE_AMOUNT).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
                            ACTUAL_PRICE: ((itemList.TAXABLE_AMOUNT - itemList.EXCISE) / itemList.QUANTITY).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
                            VAT_AMOUNT: (itemList.VAT_AMOUNT).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
                            NET_AMOUNT: (itemList.NET_AMOUNT).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
                        });
                        quantity += itemList.QUANTITY;
                        id++;
                    }
                    $scope.TOTAL_AMOUNT = $scope.TOTAL_AMOUNT.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    $scope.TOTAL_DISCOUNT = $scope.TOTAL_DISCOUNT.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    $scope.TOTAL_EXCISE = $scope.TOTAL_EXCISE.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    $scope.TOTAL_TAXABLE_AMOUNT = $scope.TOTAL_TAXABLE_AMOUNT.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    $scope.TOTAL_VAT = $scope.TOTAL_VAT.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    $scope.amountinword = convertNumberToWords(quotation.TOTAL_NET_AMOUNT);
                    $scope.TOTAL_NET_AMOUNT = $scope.TOTAL_NET_AMOUNT.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

                    for (var i = 0; i < quotation.TermsCondition.length; i++) {
                        var termList = quotation.TermsCondition[i];
                        $scope.termList.push({
                            ID: idTerm,
                            TERM_CONDITION: termList.TERM_CONDITION,
                        });
                        idTerm++;
                    }
                    $scope.TOTAL_QUANTITY = quantity;
                    $scope.chunkedProducts = [];
                    for (let i = 0; i < $scope.productList.length; i += chunkSize) {
                        let chunk = $scope.productList.slice(i, i + chunkSize);
                        $scope.chunkedProducts.push(chunk);
                    }

                })
                .catch(function (error) {
                    var message = 'Error in displaying quotation!!';
                    displayPopupNotification(message, "error");
                });
            $("#saveAndPrintQuotationModal").modal("toggle");
        }, 50);
    }
    $scope.printTodayDateTime = new Date();

    $scope.cnlPrint = function () {
        $("#saveAndPrintQuotationModal").modal("hide");
    };
    $scope.printcounttext = "Copy of Original";
    function convertNumberToWords(amount) {

        var finalWord1 = test_value(amount);
        var finalWord2 = "";

        var val = amount.toString();
        var actual_val = amount;
        amount = val;

        if (val.indexOf('.') != -1) {
            val = val.substring(val.indexOf('.') + 1, val.length);
            if (val.length == 0 || val == '00') {
                finalWord2 = "paisa zero";
            }
            else {
                amount = val;
                finalWord2 = "paisa" + test_value(amount);
            }
            return finalWord1 + " Rupees and " + finalWord2;
        }
        else {
            //finalWord2 =  " Zero paisa only";
            return finalWord1 + " Rupees";
        }
        amount = actual_val;
    }
    function test_value(amount) {

        var junkVal = amount;
        junkVal = Math.floor(junkVal);
        var obStr = new String(junkVal);
        numReversed = obStr.split("");
        actnumber = numReversed.reverse();


        if (Number(junkVal) == 0) {
            return obStr + '' + 'Rupees Zero';

        }


        var iWords = ["Zero", " One", " Two", " Three", " Four", " Five", " Six", " Seven", " Eight", " Nine"];
        var ePlace = ['Ten', ' Eleven', ' Twelve', ' Thirteen', ' Fourteen', ' Fifteen', ' Sixteen', ' Seventeen', ' Eighteen', 'Nineteen'];
        var tensPlace = ['dummy', ' Ten', ' Twenty', ' Thirty', ' Forty', ' Fifty', ' Sixty', ' Seventy', ' Eighty', ' Ninety'];

        var iWordsLength = numReversed.length;
        var totalWords = "";
        var inWords = new Array();
        var finalWord = "";
        j = 0;
        for (i = 0; i < iWordsLength; i++) {
            switch (i) {
                case 0:
                    if (actnumber[i] == 0 || actnumber[i + 1] == 1) {
                        inWords[j] = '';
                    }
                    else {
                        inWords[j] = iWords[actnumber[i]];
                    }
                    inWords[j] = inWords[j];
                    break;
                case 1:
                    tens_complication();
                    break;
                case 2:
                    if (actnumber[i] == 0) {
                        inWords[j] = '';
                    }
                    else if (actnumber[i - 1] != 0 && actnumber[i - 2] != 0) {
                        inWords[j] = iWords[actnumber[i]] + ' Hundred and';
                    }
                    else {
                        inWords[j] = iWords[actnumber[i]] + ' Hundred';
                    }
                    break;
                case 3:
                    if (actnumber[i] == 0 || actnumber[i + 1] == 1) {
                        inWords[j] = '';
                    }
                    else {
                        inWords[j] = iWords[actnumber[i]];
                    }
                    if (actnumber[i + 1] != 0 || actnumber[i] > 0) { //here
                        inWords[j] = inWords[j] + " Thousand";
                    }
                    break;
                case 4:
                    tens_complication();
                    break;
                case 5:
                    if (actnumber[i] == "0" || actnumber[i + 1] == 1) {
                        inWords[j] = '';
                    }
                    else {
                        inWords[j] = iWords[actnumber[i]];
                    }
                    if (actnumber[i + 1] != 0 || actnumber[i] > 0) {   //here 
                        inWords[j] = inWords[j] + " Lakh";
                    }

                    break;
                case 6:
                    tens_complication();
                    break;
                case 7:
                    if (actnumber[i] == "0" || actnumber[i + 1] == 1) {
                        inWords[j] = '';
                    }
                    else {
                        inWords[j] = iWords[actnumber[i]];
                    }
                    if (actnumber[i + 1] != 0 || actnumber[i] > 0) { // changed here
                        inWords[j] = inWords[j] + " Crore";
                    }
                    break;
                case 8:
                    tens_complication();
                    break;
                default:
                    break;
            }
            j++;
        }
        function tens_complication() {
            if (actnumber[i] == 0) {
                inWords[j] = '';
            }
            else if (actnumber[i] == 1) {
                inWords[j] = ePlace[actnumber[i - 1]];
            }
            else {
                inWords[j] = tensPlace[actnumber[i]];
            }
        }
        inWords.reverse();
        for (i = 0; i < inWords.length; i++) {
            finalWord += inWords[i];
        }
        return finalWord;
    }



    $scope.printDiv1 = function () {
        $scope.isPrint = true;
        $scope.tempChunk = $scope.chunkedProducts;
        $scope.chunkedProducts = [];
        for (let i = 0; i < $scope.productList.length; i += chunkSize) {
            let chunk = $scope.productList.slice(i, i + chunkSize);
            while (chunk.length < chunkSize) {
                chunk.push({ ItemDescription: '', UNIT: '', QUANTITY: '', RATE: '', AMOUNT: '' });
            }
            $scope.chunkedProducts.push(chunk);
        }
        setTimeout(function () {
            var printContents = document.getElementById('saveAndPrintQuotationModalFor').innerHTML;
            var styles = `
            <style>
        @media print {
            .info {
                text-align: center;
            }

            .right-side {
                width: 50%;
                float: left;
            }

            .left-side {
                width: 50%;
                float: right;
            }

            table {
                border-collapse: collapse;
            }

            .fnce>thead {
                border-top: 1px solid #333;
                border-bottom: 1px solid #333;
            }

            p {
                margin: 8px 0;
                font-size: smaller;
            }

            td {
                padding: 4px 8px;
            }

            .footer-space {
                margin-top: 20px;
            }

            .fixed-table-body {
                max-height: 300px;
                overflow: hidden;
                border-bottom: 1px solid #333;
            }

            .print_item_table {
                min-height: 80mm;
                border-bottom: 1px solid #333;
                width: 100%;
            }

            .print_item_table td {
                vertical-align: top;
                padding: 6px;
                font-size: 12px;
            }

            .items_table {
                vertical-align: text-top;
            }


            .print-only {
                display: block !important;
            }

            .print-header {
                background-color: white;
                z-index: 1000;
                border-bottom: 1px solid #ccc;
                
            }

            .item_table_print {
                page-break-before: always;
            }
            .no-page-break{
                page-break-before: avoid !important;
                break-inside: avoid !important;
            }
            .print-page.no-break {
                break-inside: avoid;
            }

            @page {
                size: A4;
                margin: 20mm;
            }
        }

        .print-only {
            display: none;
        }
    </style>
        `;
            if ($scope.MultiPartyPrint) {
                styles = `
                    <style>
@media print {
    @page {
        size: A4 landscape;
        margin: 8mm;
    }

    body {
        font-size: 8.5px;
        line-height: 1.2;
    }

    .print-page {
        page-break-after: auto;
        page-break-inside: avoid;
    }

    .no-break {
        page-break-after: avoid;
    }

    table, tr, td, th {
        page-break-inside: avoid;
    }
}

.print_table {
    width: 100%;
    border-collapse: collapse;
    font-size: 8.5px;
}

.print_table th,
.print_table td {
    border: 1px solid #000;
    padding: 2px 4px;     
    vertical-align: top;
    white-space: nowrap; 
}

.vendor-section {
    border: 1px solid #000;
    padding: 5px; 
    margin-bottom: 8px; 
}

.vendor-title {
    font-weight: 700;
    font-size: 9.5px; 
    margin-bottom: 3px;
}

.fnce td div {
    font-size: 7.8px;
    line-height: 1.15;
    margin: 0;
}
</style>
                `;
            }
            var popupWin = window.open('', '_blank', 'width=800,height=1000');
            popupWin.document.open();
            popupWin.document.write(`
            <html>
            <head>
                <title>Quotation Details</title>
                ${styles}
            </head>
            <body onload="window.print();window.close();">
                <div class="container">
                    ${printContents}
                </div>
            </body>
            </html>
        `);
            popupWin.document.close();
            $scope.isPrint = false;
            $scope.chunkedProducts = $scope.tempChunk;
        }, 5);
        $scope.cnlPrint();
    };


});
