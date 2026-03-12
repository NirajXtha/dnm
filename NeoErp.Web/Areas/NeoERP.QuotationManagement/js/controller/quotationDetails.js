QMModule.controller('quotationDetails', function ($scope, $rootScope, $http, $filter, $timeout, $location, $httpParamSerializer) {
    $("#englishdatedocument").kendoDatePicker();
    $("#validDt").kendoDatePicker();
    $("#deliveryDt").kendoDatePicker();
    $scope.productFormList = [];
    $scope.counterProduct = 1;
    $scope.quotationDetails = false;
    $scope.showSpecificationDetail = false;
    $scope.tableData = true;

    $http.post('/api/QuotationApi/ListQuotationDetails')
        .then(function (response) {
            var tenderDetails = response.data; 
            if (tenderDetails && tenderDetails.length > 0) {
                tenderDetails.forEach(function (tender) {
                    tender.DELIVERY_DATE = formatDate(tender.DELIVERY_DATE);
                });
                $scope.dataSource.data(tenderDetails); // Set the data to the dataSource
            } else {
                console.log("No tenders found.");
            }
        })
    function formatDate(dateString) {
        var date = new Date(dateString);
        return $filter('date')(date, 'dd-MMM-yyyy');
    }
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
        toolbar: ["excel"/*, "pdf"*/],
        excel: {
            fileName: "Quotation.xlsx",
            allPages: true
        },
        //pdf: {
        //    fileName: "Projects.pdf",
        //    allPages: true
        //},
        columns: [
            { field: "TENDER_NO", title: "Quote No", locked: true, width: 120 },
            { field: "PARTY_NAME", title: "Party Name", locked: true, width: 170 },
            { field: "PAN_NO", title: "PAN No", width: 90 },
            { field: "ADDRESS", title: "Address", width: 150 },
            { field: "CONTACT_NO", title: "Contact No", width: 100 },
            { field: "EMAIL", title: "Email", width: 120 },
            { field: "CURRENCY", title: "Currency", width: 100 },
            { field: "CURRENCY_RATE", title: "Currency Rate", width: 100 },
            { field: "DELIVERY_DATE", title: "Delivery Date", width: 90 },
            { field: "TERM_CONDITION", title: "Terms & Condition", width: 150 },
            { field: "TOTAL_AMOUNT", title: "Total Amount", width: 120, template: "#= kendo.toString(TOTAL_AMOUNT, 'n2') #" },
            { field: "TOTAL_DISCOUNT", title: "Total Discount", width: 120, template: "#= kendo.toString(TOTAL_DISCOUNT, 'n2') #" },
            { field: "TOTAL_EXCISE", title: "Total Excise", width: 120, template: "#= kendo.toString(TOTAL_EXCISE, 'n2') #" },
            { field: "TOTAL_TAXABLE_AMOUNT", title: "Total Taxable Amount", width: 130, template: "#= kendo.toString(TOTAL_TAXABLE_AMOUNT, 'n2') #" },
            { field: "TOTAL_VAT", title: "Total VAT", width: 120, template: "#= kendo.toString(TOTAL_VAT, 'n2') #" },
            { field: "TOTAL_NET_AMOUNT", title: "Total Net Amount", width: 130, template: "#= kendo.toString(TOTAL_NET_AMOUNT, 'n2') #" },
            { field: "STATUS", title: "Status", width: 100 },
            {
                title: "Actions",
                width: 120,
                template: "<a class='btn btn-sm btn-info view-btn' data-id='#= QUOTATION_NO #'><i class='fa fa-eye'></i></a>&nbsp;&nbsp;<a class='btn btn-sm btn-danger delete-btn' data-id='#= QUOTATION_NO #'><i class='fa fa-trash'></i></a>"
            }
        ]

    });
    $("#kGrid").on("click", ".view-btn", function () {
        var id = $(this).data("id");
        $scope.tableData = false;
        $scope.quotationDetails = true;
        $http.get('/api/QuotationApi/QuotationDetailsById?quotationNo=' + id)
            .then(function (response) {
                var quotation = response.data[0];
                console.log(quotation);
                $scope.TENDER_NO = quotation.TENDER_NO;
                $scope.ISSUE_DATE = formatDate(quotation.ISSUE_DATE);
                $scope.VALID_DATE = formatDate(quotation.VALID_DATE);
                $scope.NEPALI_DATE = quotation.NEPALI_DATE;
                $scope.TXT_REMARKS = quotation.REMARKS;
                var id = 1;
                $scope.panelMode = 'view';
                $scope.productFormList = [];
                for (var i = 0; i < quotation.Items.length; i++) {
                    var itemList = quotation.Items[i];
                    var imageUrl = window.location.protocol + "//" + window.location.host + "/Areas/NeoERP.QuotationManagement/Image/Items/" + itemList.IMAGE;
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
                    });
                    id++;
                }
                $scope.tableData = false;
                $scope.quotationDetails = true;

                // After populating data, trigger select events
                setTimeout(function () {
                    for (let i = 0; i < quotation.Items.length; i++) {
                        var currentItem = quotation.Items[i];
                        var currentItemCode = currentItem.ITEM_CODE;
                        // Check if the element exists before attempting to trigger the select event
                        var dropdownElement = $("#item_" + id).data("kendoDropDownList");
                        if (dropdownElement) {
                            dropdownElement.value(currentItemCode);
                        }
                        id++;
                    }
                }, 200);
            })
            .catch(function (error) {
                var message = 'Error in displaying quotation!!';
                displayPopupNotification(message, "error");
            });
    });
});



