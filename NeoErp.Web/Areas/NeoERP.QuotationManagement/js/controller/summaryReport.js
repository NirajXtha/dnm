QMModule.controller('summaryReport', function ($scope, $rootScope, $http, $filter, $timeout, $window) {

    // Initialize scope variables
    $scope.productFormList = [];
    $scope.termList = [];
    $scope.counterProduct = 1;
    $scope.quotationDetails = false;
    $scope.showSpecificationDetail = false;
    $scope.tableData = true;
    $scope.tenderItemDetails = false;

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
    reportConfig = GetReportSetting("SummaryReport");
    // Fetch tender details
    /*$http.post('/api/QuotationApi/TendersItemWise')
        .then(function (response) {
            var tenderDetails = response.data;
            if (tenderDetails && tenderDetails.length > 0) {
                $scope.dataSource.data(tenderDetails); // Set the data to the dataSource
            } else {
                console.log("No tenders found.");
            }
        });*/
    //DateFilter.init(function () {
    //    BindGrid();
    //});
    BindGrid();
    document.querySelector(".date-tooltip").addEventListener("click", function () {
        console.log("Date button clicked");
        DateFilter.init();
    });
    document.getElementById("applydp").addEventListener("click", function () {
        console.log("Apply button clicked");
        BindGrid();
    });
    function BindGrid() {
        payload = angular.extend({}, ReportFilter?.filterAdditionalData());
        $http.post('/api/QuotationApi/TendersItemWise', payload?.ReportFilters)
            .then(function (response) {
                var allItems = response.data;

                // Group by composite key: ID + TENDER_NO
                const grouped = {};
                allItems.forEach(item => {
                    const groupKey = `${item.ID}_${item.TENDER_NO}`; // Composite key

                    if (!grouped[groupKey]) {
                        grouped[groupKey] = {
                            ID: item.ID,
                            TENDER_NO: item.TENDER_NO,
                            CREATED_DATE: item.CREATED_DATE,
                            VALID_DATE: item.VALID_DATE,
                            STATUS: item.STATUS,
                            CHECKED_BY: item.CHECKED_BY,
                            VERIFIED_BY: item.VERIFIED_BY,
                            RECOMMENDED_BY: item.RECOMMENDED_BY,
                            APPROVED_BY: item.APPROVED_BY,
                            ITEMS: []
                        };
                    }
                    grouped[groupKey].ITEMS.push(item);
                });

                // Convert grouped object to array
                const tenderSummaries = Object.values(grouped);

                $scope.dataSource.data(tenderSummaries); // Set to main grid data
            });
    }

    // Initialize data source for main grid
    $scope.dataSource = new kendo.data.DataSource({
        data: [], // Initially empty
    });

    var grid = $("#kGrid").kendoGrid({
        dataSource: $scope.dataSource,
        height: 400,
        sortable: true,
        pageable: {
            refresh: true,
            pageSizes: true
        },
        toolbar: ["excel"],
        excel: {
            fileName: "Tender Details.xlsx",
            allPages: true
        },
        resizable: true,
        detailTemplate: kendo.template($("#detail-template").html()),
        detailInit: function (e) {
            const gridContainer = e.detailCell.find(".tender-detail-grid");

            if (gridContainer.length && !gridContainer.data("kendoGrid")) {
                gridContainer.kendoGrid({
                    dataSource: {
                        data: e.data.ITEMS || []
                    },
                    scrollable: false,
                    sortable: true,
                    pageable: false,
                    columns: [
                        { field: "ITEM_DESC", title: "Product Name", width: 300 },
                        { field: "SPECIFICATION", title: "Specification", width: 150 },
                        { field: "UNIT", title: "Unit", width: 50 },
                        { field: "QUANTITY", title: "Quantity", width: 80 },
                    ]
                });
            }
        },
        columns: [
            { field: "ID", hidden: true },
            { field: "TENDER_NO", title: "Quote No", width: 120 },
            {
                field: "CREATED_DATE", title: "Date", width: 100,
                template: "#=kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy')#",
            },
            {
                field: "VALID_DATE", title: "Delivery Date", width: 100,
                template: "#=kendo.toString(kendo.parseDate(VALID_DATE),'dd MMM yyyy')#",
            },
            { field: "STATUS", title: "Status", width: 90 },
            { field: "CHECKED_BY", title: "Checked By", width: 90 },
            { field: "VERIFIED_BY", title: "Verified By", width: 90 },
            { field: "RECOMMENDED_BY", title: "Recommended By", width: 90 },
            { field: "APPROVED_BY", title: "Approved By", width: 90 },
            {
                title: "Actions",
                width: 120,
                template: "<a class='btn btn-sm btn-info view-btn' data-id='#= ID #'><i class='fa fa-eye'></i></a>"
            }
        ]
    }).data("kendoGrid");
    $scope.dataSource.bind("change", function () {
        var grid = $("#kGrid").data("kendoGrid");
        if (grid) {
            grid.refresh();
        }
    });

    $scope.openImage = function (imageUrl) {
        window.open(imageUrl, '_blank');
    };
    // Initialize data source for view grid
    $scope.viewGridDataSource = new kendo.data.DataSource({
        data: [], // Initially empty
        pageSize: 10 // Optionally, set page size
    });

    // Handle click event on view button
    $("#kGrid").on("click", ".view-btn", function () {
        var quoteNo = $(this).data("id");
        var id = quoteNo;
        //var id = quoteNo.split(new RegExp('/', 'i')).join('_');
        window.location.href = "/QuotationManagement/Home/Index#!QM/QuotationDetail/" + id;
    });
    $scope.$watch('txtSearchString', function (newVal) {
        if (!grid || !grid.dataSource) return;

        // If empty -> remove filters on the TENDER_NO field
        if (!newVal) {
            // remove any TENDER_NO filter but keep other filters (if any)
            const current = grid.dataSource.filter();
            if (!current) return; // nothing to remove

            // If filter is a composite (filters array) handle both cases
            if (current.filters && current.filters.length) {
                const remaining = current.filters.filter(f => f.field !== "TENDER_NO");
                if (remaining.length) {
                    grid.dataSource.filter({ logic: current.logic || "and", filters: remaining });
                } else {
                    grid.dataSource.filter([]); // clear all
                }
            } else if (current.field === "TENDER_NO") {
                grid.dataSource.filter([]); // was a single filter -> clear
            }
            return;
        }

        // Apply single-field filter (TENDER_NO only)
        grid.dataSource.filter({
            field: "TENDER_NO",
            operator: "contains",
            value: newVal
        });
    });
    function formatDate(dateString) {
        var date = new Date(dateString);
        return $filter('date')(date, 'dd-MMM-yyyy');
    }

});
