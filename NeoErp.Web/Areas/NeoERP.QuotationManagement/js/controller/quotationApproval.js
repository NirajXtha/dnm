QMModule.controller('quotationApproval', function ($scope, $rootScope, $http, $filter, $timeout, $location, $window) {
    $scope.pageName = "Quotation Approval";
    $scope.saveAction = "Save";
    $scope.createPanel = false;
    $scope.tablePanel = true;
    $scope.createLink = false;
    $scope.viewPanel = false;
    $scope.createEdit = false;
    $scope.idShowHide = false;

    $scope.approvalRemarks = "";
    $scope.pendingApprovalQuote = null;
    $scope.remarksAction = "";
    $scope.remarksActionTitle = "";

    $scope.AddButtonClickEvent = function () {
        window.location.href = "/QuotationManagement/Home/Index#!QM/QuotationApproval"
    }

    $scope.productFormList = [];
    $scope.counterProduct = 1;

    $scope.clear = function () {
        $scope.pageName = "Quotation Approval";
        $scope.saveAction = "Save";
    }
    $scope.setInitialWidth = function () {
        $(".table-container").css("width", "98%");
    };
    $scope.TENDER_NO = "";
    /*$http.get('/api/QuotationApi/getTenderNo',)
        .then(function (response) {
            $scope.TENDER_NO = response.data[0].TENDER_NO;
        })
        .catch(function (error) {
            console.error('Error fetching ID:', error);
        });*/
    $scope.selectedItem = null;
    $scope.generatedUrl = '';

    $scope.generateLink = function () {
        $http.get('/api/QuotationApi/getTenderId?tenderNo=' + $scope.TENDER_NO)
            .then(function (response) {
                $scope.ID = response.data[0].ID;
                var linkeUrl = window.location.protocol + "//" + window.location.host + "/Quotation/Index?qo=" + $scope.ID;
                $scope.generatedUrl = linkeUrl;
            })
            .catch(function (error) {
                displayPopupNotification("Error fetching ID", "error");
            });
    };
    $http.post('/api/QuotationApi/ListAllPendingTenders')
        .then(function (response) {
            var tenders = response.data;
            if (tenders && tenders.length > 0) {
                $scope.dataSource.data(tenders);
            } else {
            }
        })
    $scope.QuotationApproval = false;
    $http.get('/api/QuotationApi/ApprovalProceeding?amount=0')
        .then(function (response) {
            $scope.QuotationApproval = response.data.success;
        }).finally(function () {
            if ($("#kGrid").data("kendoGrid")) {
                $("#kGrid").data("kendoGrid").refresh();
            }
        });
    $scope.dataSource = new kendo.data.DataSource({
        data: [], // Initially empty
        pageSize: 10// Optionally, set page size
    });
    $("#kGrid").kendoGrid({
        dataSource: $scope.dataSource,
        height: 400,
        sortable: true,
        pageable: {
            refresh: true,
            pageSizes: true
        },
        toolbar: ["excel"],
        excel: {
            fileName: "Quotation.xlsx",
            allPages: true
        },
        columns: [
            { field: "ID", title: "ID", type: "string", hidden: true },
            { field: "TENDER_NO", title: "Quote No", type: "string" },
            {
                field: "ISSUE_DATE", title: "Issue Date", type: "string",
                template: "#=kendo.toString(kendo.parseDate(ISSUE_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(ISSUE_DATE),'dd MMM yyyy') #",
            },
            {
                field: "VALID_DATE", title: "Delivery Date", type: "string",
                template: "#=kendo.toString(kendo.parseDate(VALID_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(VALID_DATE),'dd MMM yyyy') #",
            },
            {
                field: "CREATED_DATE", title: "Created Date", type: "string",
                template: "#=kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy') #",
            },
            { field: "APPROVED_STATUS", title: "Approved Status", type: "string" },
            {
                title: "Actions",
                template: function (dataItem) {
                    var disabled = !$scope.QuotationApproval ? "disabled" : "";
                    return `
                        <a class='btn btn-sm btn-success approve-btn' data-item='${dataItem.ID}' data-id='${dataItem.TENDER_NO}' ${disabled} title='Approve' style="color: white;">
                            <i class='fa fa-check'></i>
                        </a>
                        &nbsp;
                        <a class='btn btn-sm btn-danger disapprove-btn' data-item='${dataItem.ID}' data-id='${dataItem.TENDER_NO}' ${disabled} title='Reject' style="color: white;">
                            <i class='fa fa-times'></i>
                        </a>
                        &nbsp;
                        <a class='btn btn-sm btn-info view-btn' data-item='${dataItem.ID}' data-id='${dataItem.TENDER_NO}' title='View' style="color: white;">
                            <i class='fa fa-info'></i>
                        </a>
                    `;
                }
            }

        ]
    });
    var id;
    var quoteNo;
    $scope.product = {};

    $('#kGrid').on('click', '.approve-btn', function () {
        openRemarksPopup($(this).data('id'), $(this).data('item'), 'APPROVE');
    });

    $('#kGrid').on('click', '.disapprove-btn', function () {
        openRemarksPopup($(this).data('id'), $(this).data('item'), 'REJECT');
    });

    function openRemarksPopup(tenderNo, tenderId, action) {
        $scope.$apply(function () {
            $scope.pendingApprovalQuote = {
                TENDER_NO: tenderNo,
                ID: tenderId
            };

            $scope.remarksAction = action;
            $scope.remarksActionTitle =
                action === 'APPROVE'
                    ? 'Approval Remarks'
                    : 'Rejection Remarks';

            $scope.approvalRemarks = "";
        });

        $('#approvalRemarksModal').modal('show');
    }


    //$('#kGrid').on('click', '.approve-btn', function () {
    //    var tenderNo = $(this).data('id');
    //    var q_no = $(this).data('item');
    //    $scope.$apply(function () {
    //        $scope.pendingApprovalQuote = {
    //            TENDER_NO: tenderNo,
    //            ID: tenderId
    //        };
    //        $scope.approvalRemarks = "";
    //    });

    //    $('#approvalRemarksModal').modal('show');
    //    //$http.get('/api/QuotationApi/getTenderId?tenderNo=' + q_no)
    //    //    .then(function (response) {
    //    //        var id = response.data[0].ID;
    //    //        var formData = {
    //    //            TENDER_NO: tenderNo,
    //    //            ID: id
    //    //        };
    //    //        $http.post('/api/QuotationApi/QuotationApproval', formData)
    //    //            .then(function (res) {
    //    //                displayPopupNotification(res.data.message, "success");
    //    //                $timeout(function () {
    //    //                    window.location.reload();
    //    //                }, 3000);

    //    //            })
    //    //            .catch(function (err) {
    //    //                displayPopupNotification("Approval failed.", "error");
    //    //            });
    //    //    })
    //    //    .catch(function () {
    //    //        displayPopupNotification("Error fetching ID.", "error");
    //    //    });
    //});

    $scope.confirmApprove = function () {

        if (!$scope.approvalRemarks || !$scope.approvalRemarks.trim()) {
            displayPopupNotification("Remarks is required.", "warning");
            return;
        }

        var payload = {
            TENDER_NO: $scope.pendingApprovalQuote.TENDER_NO,
            ID: $scope.pendingApprovalQuote.ID,
            REMARKS: $scope.approvalRemarks
        };

        $http.post('/api/QuotationApi/QuotationApproval', payload)
            .then(function (res) {
                displayPopupNotification(res.data.message, "success");
                $('#approvalRemarksModal').modal('hide');

                $timeout(function () {
                    window.location.reload();
                }, 2000);
            })
            .catch(function () {
                displayPopupNotification("Approval failed.", "error");
            });
    };

    $scope.openActionPopup = function (quote, action) {
        $scope.pendingApprovalQuote = {
            TENDER_NO: quote.TENDER_NO,
            ID: quote.ID
        };

        $scope.remarksAction = action;
        $scope.remarksActionTitle =
            action === 'APPROVE'
                ? 'Approval Remarks'
                : 'Rejection Remarks';

        $scope.approvalRemarks = "";
        $('#approvalRemarksModal').modal('show');
    };

    $scope.submitRemarksAction = function () {

        if (!$scope.approvalRemarks || !$scope.approvalRemarks.trim()) {
            displayPopupNotification("Remarks is required.", "warning");
            return;
        }

        var payload = {
            TENDER_NO: $scope.pendingApprovalQuote.TENDER_NO,
            ID: $scope.pendingApprovalQuote.ID,
            REMARKS: $scope.approvalRemarks
        };

        var apiUrl = $scope.remarksAction === 'APPROVE'
            ? '/api/QuotationApi/QuotationApproval'
            : '/api/QuotationApi/deleteQuotationId';

        $http.post(apiUrl, payload)
            .then(function (res) {
                displayPopupNotification(res.data.message || "Success", "success");
                $('#approvalRemarksModal').modal('hide');

                $timeout(function () {
                    window.location.reload();
                }, 2000);
            })
            .catch(function () {
                displayPopupNotification("Action failed.", "error");
            });
    };

    //$('#kGrid').on('click', '.disapprove-btn', function (e) {
    //    e.stopPropagation(); // prevent event bubbling

    //    // Close any open popovers
    //    $('.disapprove-btn').not(this).popover('hide');

    //    var tenderNo = $(this).data('id');
    //    var q_no = $(this).data('item');
    //    var deleteButton = $(this);
    //    var popoverContent = `
    //    <div class="popover-delete-confirm">
    //        <p>Reject?</p>
    //        <div class="popover-buttons">
    //            <button type="button" class="btn btn-danger confirm-delete">Yes</button>
    //            <button type="button" class="btn btn-secondary cancel-delete">No</button>
    //        </div>
    //    </div>
    //`;

    //    deleteButton.popover({
    //        container: 'body',
    //        placement: 'bottom',
    //        html: true,
    //        content: popoverContent,
    //        trigger: 'manual'
    //    }).popover('show');

    //    // Handle outside click to close popover
    //    $(document).on('click.dismissPopover', function (e) {
    //        if (!$(e.target).closest('.popover, .disapprove-btn').length) {
    //            deleteButton.popover('hide');
    //            $(document).off('click.dismissPopover');
    //        }
    //    });

    //    $(document).off('click.confirmDelete').on('click.confirmDelete', '.confirm-delete', function () {
    //        $http.post('/api/QuotationApi/deleteQuotationId?tenderNo=' + q_no)
    //            .then(function (response) {
    //                var message = response.data.MESSAGE;
    //                displayPopupNotification(message, "success");
    //                window.location.reload();
    //            }).catch(function () {
    //                displayPopupNotification("Error in rejecting quotation.", "error");
    //            });
    //        deleteButton.popover('hide');
    //        $(document).off('click.dismissPopover');
    //    });

    //    $(document).off('click.cancelDelete').on('click.cancelDelete', '.cancel-delete', function () {
    //        deleteButton.popover('hide');
    //        $(document).off('click.dismissPopover');
    //    });
    //});
    $('#kGrid').on('click', '.view-btn', function () {
        var tenderNo = $(this).data('id');
        var q_no = $(this).data('item');
        $timeout(function () {
            $http.get('/api/QuotationApi/GetQuotationById?tenderNo=' + q_no)
                .then(function (response) {
                    var quote = response.data[0];
                    var items = quote.Items;

                    // Now fetch item details using a separate API
                    $http.get('/api/QuotationApi/ItemDetails', items)
                        .then(function (res) {
                            var enrichedItems = items.map(function (item) {
                                // Find the corresponding item in ItemDetails using ItemCode
                                var details = res.data.find(d => d.ItemCode === item.ITEM_CODE);

                                return Object.assign({}, item, {
                                    // If details exist, merge fields; otherwise, retain original item data
                                    ItemDescription: details?.ItemDescription || item.SPECIFICATION,
                                    SPECIFICATION: item.SPECIFICATION,
                                    CATEGORY: item.CATEGORY,
                                    BRAND_NAME: item.BRAND_NAME,
                                    INTERFACE: item.INTERFACE,
                                    TYPE: item.TYPE,
                                    LAMINATION: item.LAMINATION,
                                    ITEM_SIZE: item.ITEM_SIZE,
                                    THICKNESS: item.THICKNESS,
                                    COLOR: item.COLOR,
                                    GRADE: item.GRADE,
                                    SIZE_LENGTH: item.SIZE_LENGTH,
                                    SIZE_WIDTH: item.SIZE_WIDTH,
                                    IMAGE: item.IMAGE,
                                    PRICE: item.PRICE,
                                    UNIT: item.UNIT
                                });
                            });
                            quote.Items = enrichedItems;
                            $scope.selectedItem = quote;
                            $('#quoteViewModal').modal('show');
                        })
                        .catch(function () {
                            displayPopupNotification("Failed to load item details.", "error");
                        });
                })
                .catch(function (error) {
                    displayPopupNotification("Failed to load quotation details.", "error");
                });
        });
    });
    $scope.showSpecificationDetail = false;

    $scope.toggleDetails = function () {
        $scope.showSpecificationDetail = !$scope.showSpecificationDetail;
    };
    $scope.approveQuotation = function (quote) {
        var tenderNo = quote.TENDER_NO;
        var tenderId = quote.ID;

        $http.get('/api/QuotationApi/getTenderId?tenderNo=' + tenderId)
            .then(function (response) {
                var id = response.data[0].ID;
                var formData = {
                    TENDER_NO: tenderNo,
                    ID: tenderId
                };
                $http.post('/api/QuotationApi/QuotationApproval', formData)
                    .then(function (res) {
                        displayPopupNotification(res.data.message, "success");
                        $timeout(function () {
                            window.location.reload();
                        }, 3000);
                    })
                    .catch(function () {
                        displayPopupNotification("Approval failed.", "error");
                    });
            })
            .catch(function () {
                displayPopupNotification("Error fetching ID.", "error");
            });
    };

    $scope.rejectQuotation = function (quote) {
        var tenderNo = quote.ID;

        if (confirm("Reject this quotation?")) {
            $http.post('/api/QuotationApi/deleteQuotationId?tenderNo=' + tenderNo)
                .then(function (response) {
                    displayPopupNotification(response.data.MESSAGE, "success");
                    window.location.reload();
                })
                .catch(function () {
                    displayPopupNotification("Error in rejecting quotation.", "error");
                });
        }
    };

    $("#itemtxtSearchString").keyup(function () {
        var val = $(this).val().toLowerCase(); // Get the search input value

        var filters = [];

        // Retrieve columns from the Kendo UI Grid configuration
        var columns = $("#kGrid").data("kendoGrid").columns;

        // Loop through each column in the grid configuration
        for (var i = 0; i < columns.length; i++) {
            var column = columns[i];
            var field = column.field;

            // Determine the type of data in the column and construct the filter accordingly
            if (column.type === "string") {
                filters.push({
                    field: field,
                    operator: "contains",
                    value: val
                });
            } else if (column.type === "number") {
                // Assuming the input value can be parsed into a number
                filters.push({
                    field: field,
                    operator: "eq",
                    value: parseFloat(val) || null
                });
            } else if (column.type === "date") {
                if (parsedDate) {
                    filters.push({
                        field: field,
                        operator: "eq",
                        value: new Date(val) || null
                    });
                }
            }
        }

        // Apply the filters to the Kendo UI Grid data source
        $scope.dataSource.filter({
            logic: "or",
            filters: filters
        });
    });
});