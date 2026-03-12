QMModule.controller('ViewQuotation', function ($scope, $rootScope, $http, $filter, $timeout, $routeParams) {
    $scope.pageName = "Add Quotation";
    $scope.saveAction = "Save";
    $scope.createPanel = false;
    $scope.id = "";
    $scope.QUOTATION_NO = "";
    if ($routeParams.id != undefined) {
        //$scope.id = $routeParams.id.split(new RegExp('_', 'i')).join('/');
        $scope.id = $routeParams.id;
    }
    else { $scope.id = "undefined"; }
    $scope.productFormList = [];
    $scope.counterProduct = 1;
    $scope.showCustomerDetails = false;
    $scope.showSpecificationDetail = false;
    $scope.selectedCurrency = "";
    $scope.LOCAL_FLAG = 'Y';

    $scope.clear = function () {
        $scope.pageName = "Add Quotation";
        $scope.saveAction = "Save";
    }
    $scope.setInitialWidth = function () {
        $(".table-container").css("width", "98%");
    };
    $scope.toggleDetails = function () {
        $scope.showSpecificationDetail = !$scope.showSpecificationDetail;
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
    $scope.currencySelect = {
        dataTextField: "name",
        dataValueField: "name",
        dataSource: {
            transport: {
                read: {
                    url: "https://gist.githubusercontent.com/aaronhayes/5fef481815ac75f771d37b16d16d35c9/raw/edbec8eea5cc9ace57a79409cc390b7b9bcf24f6/currencies.json",
                    dataType: "json"
                }
            }
        },
        optionLabel: "Currency" // Optional: Add a default option label
    };

    function formatDate(dateString) {
        var date = new Date(dateString);
        return $filter('date')(date, 'dd-MMM-yyyy');
    }

    $scope.Cancel = function () {
        $scope.ISSUE_DATE = '';
        $scope.VALID_DATE = '';
        $scope.APPROVED_STATUS = '';
        $scope.TENDER_NO = '';
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
        });
        $scope.showSpecificationDetail = false;
        // Clear the file input value (if supported, this might not work in all browsers)
        var fileInputs = document.querySelectorAll('input[type="file"]');
        fileInputs.forEach(function (input) {
            input.value = '';
        });

        window.location.href = "/QuotationManagement/Home/Index#!QM/QuotationSetup"
    };


    $scope.generatedUrl = '';

    $scope.generateLink = function () {
        $http.get('/api/QuotationApi/getTenderId?tenderNo=' + $scope.id)
            .then(function (response) {
                $scope.ID = response.data[0].ID;
                var linkeUrl = window.location.protocol + "//" + window.location.host + "/Quotation/Index?qo=" + $scope.ID;
                $scope.generatedUrl = linkeUrl;
            })
            .catch(function (error) {
                displayPopupNotification("Error fetching ID", "error");
            });
    };


    // Handle click event for the view button

    $http.get('/api/QuotationApi/GetQuotationById?tenderNo=' + $scope.id)
            .then(function (response) {
                var quotation = response.data[0];
                $scope.QUOTATION_NO = quotation.ID;
                $scope.TENDER_NO = quotation.TENDER_NO;
                $scope.ISSUE_DATE = formatDate(quotation.ISSUE_DATE);
                $scope.VALID_DATE = formatDate(quotation.VALID_DATE);
                $scope.NEPALI_DATE = quotation.NEPALI_DATE;
                $scope.DELIVERY_DT_BS = quotation.DELIVERY_DT_BS;
                $scope.TXT_REMARKS = quotation.REMARKS;
                $scope.APPROVED_STATUS = quotation.APPROVED_STATUS;
                $scope.LOCAL_FLAG = quotation.LOCAL_FLAG;
                var id = 1;
                $scope.productFormList = [];
                for (var i = 0; i < quotation.Items.length; i++) {
                    var itemList = quotation.Items[i];
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
                    });
                    id++;
                }
                $scope.createPanel = true;
                $scope.tablePanel = false;

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
    // Handle click event for the edit button

    $scope.openImage = function (imageUrl) {
        if (imageUrl == null) {
            return;
        }
        window.open(imageUrl, '_blank');
    };
    $scope.getItemByCode = function (itemCode, product) {
        var filteredItems = $filter('filter')($scope.ItemSelect.dataSource.data, { ItemCode: itemCode });
        if (filteredItems.length > 0) {
            var selectedItem = filteredItems[0]; // Get the first matching item
            product.ItemDescription = selectedItem.ItemDescription;
            product.Unit = selectedItem.ItemUnit;
        } else {
            // If no item found, you may want to clear the properties
            product.ItemDescription = null;
            product.Unit = null;
        }
    };

});



