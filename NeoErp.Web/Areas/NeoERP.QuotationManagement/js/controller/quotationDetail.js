QMModule.controller('quotationDetail', function ($scope, $rootScope, $http, $filter, $routeParams, $window) {

    // Initialize scope variables
    $scope.productFormList = [];
    $scope.termList = [];
    $scope.counterProduct = 1;
    $scope.quotationDetails = false;
    $scope.showSpecificationDetail = false;
    var uniqueVendors = [];

    $scope.toggleDetails = function () {
        $scope.showSpecificationDetail = !$scope.showSpecificationDetail;
    };
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


    $scope.openImage = function (imageUrl) {
        window.open(imageUrl, '_blank');
    };
    // Initialize data source for view grid
    $scope.viewGridDataSource = new kendo.data.DataSource({
        data: [], // Initially empty
        pageSize: 10 // Optionally, set page size
    });

    $scope.id = "";

    if ($routeParams.id != undefined) {
        //$scope.id = $routeParams.id.split(new RegExp('_', 'i')).join('/');
        $scope.id = $routeParams.id;
    }
    else { $scope.id = "undefined"; }

    $http.get('/api/QuotationApi/ItemDetailsByTender?tenderNo=' + $scope.id)
        .then(function (response) {

            console.log("check my id", $scope.id);

                var quotation = response.data[0];
                var itemDetails = quotation.Items;
                var partyDetails = quotation.PartDetails;
                var quotationNo = quotation.QUOTATION_NO; // Extract quotation_no from quotation object
            console.log("Quotation", quotation);
                $scope.TENDER_NO = quotation.TENDER_NO;
                $scope.ISSUE_DATE = formatDate(quotation.ISSUE_DATE);
                $scope.VALID_DATE = formatDate(quotation.VALID_DATE);
                $scope.NEPALI_DATE = quotation.NEPALI_DATE;
                $scope.DELIVERY_DT_BS = quotation.DELIVERY_DT_BS;
                $scope.TXT_REMARKS = quotation.REMARKS;
                var id = 1;

                if (itemDetails && itemDetails.length > 0) {
                    if (partyDetails && partyDetails.length > 0) {
                        var ratesByItemCode = {};
                        var uniqueVendors = [];

                        //partyDetails.forEach(function (party) {
                        //    if (!ratesByItemCode[party.ITEM_CODE]) {
                        //        ratesByItemCode[party.ITEM_CODE] = {};
                        //    }
                        //    ratesByItemCode[party.ITEM_CODE][party.QUOTATION_NO] = {
                        //        rate: party.ACTUAL_PRICE,
                        //        status: party.STATUS
                        //    };

                        //    var vendorExists = uniqueVendors.some(function (item) {
                        //        return item.quotationNo === party.QUOTATION_NO;
                        //    });

                        //    // If the vendor and quotation number do not exist, add them to uniqueVendors
                        //    if (!vendorExists) {
                        //        uniqueVendors.push({
                        //            name: party.PARTY_NAME, quotationNo: party.QUOTATION_NO, status: party.STATUS,revise:party.REVISE
                        //        });
                        //    }
                        //});

                        partyDetails.forEach(function (party) {
                            if (!ratesByItemCode[party.ITEM_CODE]) {
                                ratesByItemCode[party.ITEM_CODE] = {};
                            }
                            if (!ratesByItemCode[party.ITEM_CODE][party.QUOTATION_NO]) {
                                ratesByItemCode[party.ITEM_CODE][party.QUOTATION_NO] = [];
                            }

                            // Push into array instead of overwriting
                            //ratesByItemCode[party.ITEM_CODE][party.QUOTATION_NO].push({
                            //    rate: party.ACTUAL_PRICE,
                            //    status: party.STATUS
                            //});
                            ratesByItemCode[party.ITEM_CODE][party.QUOTATION_NO].push({
                                rate: party.ACTUAL_PRICE,
                                status: party.STATUS,
                                checked_by: party.CHECKED_BY,
                                verify_by: party.VERIFY_BY,
                                recommended1_by: party.RECOMMENDED1_BY,
                                recommended2_by: party.RECOMMENDED2_BY,
                                recommended3_by: party.RECOMMENDED3_BY,
                                recommended4_by: party.RECOMMENDED4_BY,
                                approved_by: party.APPROVED_BY
                            });
                            // Build unique vendor list
                            if (!uniqueVendors.some(v => v.quotationNo === party.QUOTATION_NO)) {
                                uniqueVendors.push({
                                    name: party.PARTY_NAME,
                                    quotationNo: party.QUOTATION_NO,
                                    status: party.STATUS,
                                    revise: party.REVISE
                                });
                            }
                        });
                        var itemVendorMatchCount = {};

                        itemDetails.forEach(function (item) {
                            
                            uniqueVendors.forEach(function (vendor) {
                                var key = item.ITEM_CODE + "|" + vendor.quotationNo;
                                itemVendorMatchCount[key] = itemVendorMatchCount[key] || 0;

                                var rateArray = (ratesByItemCode[item.ITEM_CODE] && ratesByItemCode[item.ITEM_CODE][vendor.quotationNo]) || [];
                                var rateObj = rateArray[itemVendorMatchCount[key]] || null;

                                item[vendor.quotationNo] = rateObj ? rateObj : '';
                                item[vendor.quotationNo + "_status"] = rateObj ? rateObj.status : '';

                                itemVendorMatchCount[key]++;
                            });

                            item.QUOTATION_NO = $scope.TENDER_NO;
                        });


                        //itemDetails.forEach(function (item) {
                        //    debugger;
                        //    uniqueVendors.forEach(function (vendor) {
                        //        var itemRates = ratesByItemCode[item.ITEM_CODE] || {};
                        //        item[vendor.quotationNo] = itemRates[vendor.quotationNo] || '';
                        //    });
                        //    item.QUOTATION_NO = $scope.TENDER_NO;
                        //});

                        /*itemDetails.forEach(function (item) {
                            debugger;
                            uniqueVendors.forEach(function (vendor) {
                                item[vendor.quotationNo] = ratesByItemCode[item.ITEM_CODE][vendor.quotationNo] || '';
                            });
                            item.QUOTATION_NO = $scope.TENDER_NO; // Add quotation number to each item
                        });*/

                        var dynamicColumns = generateColumns(uniqueVendors);
                        var staticColumns = [
                            { field: "SN", title: "S.No", width: 40, type: "number" },
                            { field: "ITEM_DESC", title: "Product Name", width: 250, type: "string" },
                            { field: "SPECIFICATION", title: "Specification", width: 150, type: "string" },
                            { field: "UNIT", title: "Unit", width: 50, type: "string" },
                            { field: "QUANTITY", title: "Quantity", width: 90, type: "number" },
                            { field: "LAST_VENDOR", title: "Last Vendor", width: 90, type: "string" },
                            { field: "LAST_PRICE", title: "Last Price", width: 90, type: "number", style: "text-align:right" }

                        ];

                        var id = 1; // Initialize ID counter
                        itemDetails.forEach(function (item) {
                            item.ID = id++; // Assign the current ID and increment for the next item
                        });

                        var combinedColumns = staticColumns.concat(dynamicColumns);

                        // Create the viewGrid with the combined columns
                        $("#viewGrid").kendoGrid({
                            dataSource: $scope.viewGridDataSource,
                            height: 400,
                            sortable: true,
                            pageable: {
                                refresh: true,
                                pageSizes: true
                            },
                            resizable: true,
                            columns: combinedColumns
                        });

                        $scope.viewGridDataSource.data(itemDetails); // Populate viewGrid with the combined data

                        // Attach event listeners to the parent element for event delegation
                        $("#viewGrid").on("click", ".vendor-title", function () {
                            var quotationNo = $(this).attr('data-quotation-no');
                            handleTitleClick(quotationNo);
                        });

                        $("#viewGrid").on("click", ".vendor-search", function () {
                            var quotationNo = $(this).siblings('.vendor-title').attr('data-quotation-no');
                            handleTitleClick(quotationNo);
                        });
                    } else {
                        var noPartyDetailsMessage = 'Party Details are not set for this quotation.';
                        $scope.noPartyDetailsMessage = noPartyDetailsMessage;
                        $("#viewGrid").kendoGrid({
                            dataSource: [],
                            columns: [
                                { field: "Message", title: noPartyDetailsMessage, width: 500, type: "string" }
                            ]
                        });
                    }
                } else {
                    console.log("No tenders found.");
                }

            })
            .catch(function (error) {
                var message = 'Error in displaying quotation!!';
                displayPopupNotification(message, "error");
            });

    function generateColumns(uniqueVendors) {
        var vendorColumns = [];
        // Generate vendor columns
        uniqueVendors.forEach(function (vendor) {
            /*var backgroundColor = vendor.status === 'AP' ? '#9afa84' : 'transparent';*/
            let backgroundColor = 'transparent';

            switch (vendor.status) {
                case 'C': // Checked
                    backgroundColor = '#fef3c7'; // soft yellow
                    break;
                case 'RE': // Recommended
                    backgroundColor = '#bfdbfe'; // light blue
                    break;
                case 'AP': // Approved
                    backgroundColor = '#86efac'; // green
                    break;
                case 'V': // Verified
                    backgroundColor = '#a5b4fc'; // purple
                    break;
                case 'P': // Posted
                    backgroundColor = '#d1fae5'; // soft mint green
                    break;
                case 'R': // Red status
                    backgroundColor = '#f87171'; // soft red
                    break;
                default:
                    backgroundColor = 'transparent';
            }
            var displayRevise = vendor.revise ? ` (${vendor.revise})` : '';

            var headerTemplate = `
            <div style="display: flex; align-items: center; white-space: normal; word-break: break-word;">
                <span class="k-link vendor-title" data-quotation-no="${vendor.quotationNo}">${vendor.name}${displayRevise}</span>
                <i class="fa fa-edit vendor-search" style="margin-left: 1rem; cursor: pointer;"></i>
            </div>`;

            var column = {
                field: vendor.quotationNo, // Use quotationNo as the field
                headerTemplate: headerTemplate,
                width: 200,
                template: function (dataItem) {
                    var rateInfo = dataItem[vendor.quotationNo];
                    if (!rateInfo) return '';

                    let bgColor = 'transparent';

                    if (rateInfo.approved_by) {
                        bgColor = '#86efac';   // green for approved
                    } else if (
                        rateInfo.recommended1_by || rateInfo.recommended2_by ||
                        rateInfo.recommended3_by || rateInfo.recommended4_by
                    ) {
                        bgColor = '#bfdbfe';   // blue for recommended
                    } else if (rateInfo.verify_by) {
                        bgColor = '#a5b4fc';   // purple for verified
                    } else if (rateInfo.checked_by) {
                        bgColor = '#fef3c7';   // yellow for checked
                    }

                    return `<div style="display:flex;justify-content:right;cursor:pointer;
                        background-color:${bgColor};padding:2px 4px;
                        border-radius:4px;">${rateInfo.rate}</div>`;
                },
                //template: function (dataItem) {
                //    debugger;
                //    var rateInfo = dataItem[vendor.quotationNo]; // Access rate info using quotationNo
                //    return rateInfo ? `
                //    <div style="display: flex; justify-content: right; cursor: pointer;">
                //        ${rateInfo}
                //    </div>` : '';
                //},
                headerAttributes: { style: `background-color: ${backgroundColor}` }
            };

            vendorColumns.push(column);
        });

        return vendorColumns;
    }

    function handleTitleClick(quotationNo) {
        var tenderNo = $scope.TENDER_NO.split(new RegExp('/', 'i')).join('_');
        $window.location.href = "/QuotationManagement/Home/Index#!QM/QuotationDetailItemwise/" + quotationNo + "/" + $scope.id;
    }


    function formatDate(dateString) {
        var date = new Date(dateString);
        return $filter('date')(date, 'dd-MMM-yyyy');
    }
});
