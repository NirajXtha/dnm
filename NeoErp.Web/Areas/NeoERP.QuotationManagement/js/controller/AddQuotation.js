QMModule.controller('AddQuotation', function ($scope, $rootScope, $http, $filter, $timeout, $location, $httpParamSerializer) {
    $scope.pageName = "Add Quotation";
    $scope.saveAction = "Save";

    $scope.idShowHide = false;
    var checkedItems = [];
    var checkedItemsCount = [];
    var checkedCutomer = [];
    var checkedIds = {};
    $scope.FROM_DATE = '';
    $scope.TO_DATE = '';
    $scope.startdate = '';
    $scope.enddate = '';
    $scope.itemDescriptionList = [];
    $("#englishdatedocument").kendoDatePicker();
    $("#validDt").kendoDatePicker();
    $("#deliveryDt").kendoDatePicker();
    $scope.productFormList = [];
    $scope.refList = [];
    $scope.counterProduct = 1;
    $scope.showCustomerDetails = false;
    $scope.showSpecificationDetail = false;
    $scope.selectedCurrency = "";
    $scope.LOCAL_FLAG = 'Y';
    $scope.ROW = 'nonreference';

    $scope.localFlagOptions = {
        dataSource: [
            { text: "Local", value: "Y" },
            { text: "Import", value: "N" }
        ],
        dataTextField: "text",
        dataValueField: "value",
        valuePrimitive: true,
        change: function (e) {
            debugger;
            var selectedItem = this.dataItem();
            console.log("flag", $scope.LOCAL_FLAG);
        }
    };

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
    $scope.FORM_CODE = "";
    $scope.getTender = function (tender) {
        $http.get('/api/QuotationApi/getTenderNo?tender=' + tender,)
            .then(function (response) {
                if (!response.data[0].TENDER_NO || response.data[0].TENDER_NO == "") {
                    $scope.TENDER_NO = "";
                    $scope.selectedTender = "";
                    $scope.FORM_CODE = "";
                } else {
                    $scope.TENDER_NO = response.data[0].TENDER_NO;
                }
            })
            .catch(function (error) {
                console.error('Error fetching ID:', error);
            });
    };
    $scope.selectedTender = "";
    $scope.tenderSelect = {
        autoBind: false,
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/QuotationApi/getSelectQuotationOptions",
                    dataType: "json"
                }
            }
        },
        dataTextField: "FORM_EDESC",
        dataValueField: "ID",
        autoClose: true,
        optionLabel: "--Select Quotation--",
        dataBound: function () {
            this.wrapper.find("select option[value='']").prop("disabled", true);
        },
        change: function (e) {
            var selectedItem = this.dataItem();
            $scope.selectedTender = $scope.FORM_CODE = selectedItem.ID;
            $scope.getTender($scope.selectedTender);
        }
    };
    $scope.selectedItem = null;
    $http.get('/api/QuotationApi/ItemDetails').then(function (response) {
        $scope.itemDescriptionList = response.data || [];
    });

    $scope.ItemSelect = {
        autoBind: true,
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/QuotationApi/ItemDetails",
                    dataType: "json"
                }
            }
        },
        dataTextField: "ItemDescription",
        dataValueField: "ItemCode",
        filter: "contains",
        //autoClose: true,
        change: function (e) {
            var selectedItem = this.dataItem();
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

    $scope.addProduct = function () {
        $http.get("/api/QuotationApi/ItemDetails")
            .then(function (response) {
                // Assuming response.data is an array of objects with 'ItemDescription' and 'ItemCode' properties
                $scope.ItemSelect.dataSource.data = response.data;
                $scope.productFormList.push({
                    ID: $scope.counterProduct,
                    ItemCode: "",
                    UNIT: "",
                    IMAGE: "",
                    QUANTITY: "",
                    UNIT_PRICE: "",
                    AMOUNT: "",
                    REMARKS: ""
                });
            })
            .catch(function (error) {
                //console.error('Error fetching item details:', error);
                displayPopupNotification("Error fetching item details", "error");
            });
    };
    function formatDate(dateString) {
        var date = new Date(dateString);
        return $filter('date')(date, 'dd-MMM-yyyy');
    }
    $scope.addProduct();
    $scope.updateUnit = function (product) {
        if (product && product.ItemCode) {
            // Find the item with the matching ItemCode
            var selectedItem = $scope.ItemSelect.dataSource.data.find(function (item) {
                console.log(product.ItemCode);
                return item.ItemCode === product.ItemCode;
            });

            if (selectedItem) {
                product.UNIT = selectedItem.ItemUnit;
                product.SPECIFICATION = selectedItem.SPECIFICATION;

            }
        } else {
            product.UNIT = ""; // Set UNIT to empty string if product or ItemCode is not provided
        }
    };

    $scope.updateAmount = function (prod) {
        if (prod) {
            const ref = $scope.refList.find(r =>
                r.REFERENCE_NO === prod.TENDER_NO &&
                r.SERIAL_NO === prod.SERIAL_NO
            );
            if (ref && ref.REF_QTY !== prod.QUANTITY) {
                ref.REF_QTY = prod.QUANTITY;
            }
        }
        console.log($scope.refList);
    };
    $scope.addRow = function () {
        if ($scope.refList && $scope.refList.length > 0) {
            return;
        }
        var maxId = Math.max(...$scope.productFormList.map(product => product.ID));
        $scope.counterProduct = maxId !== -Infinity ? maxId + 1 : 1;
        $scope.productFormList.push({
            ID: $scope.counterProduct,
            ItemCode: "",
            UNIT: "",
            QUANTITY: "",
            UNIT_PRICE: "",
            AMOUNT: "",
            IMAGE: "",
            REMARKS: ""
        });
    };


    $scope.deleteRow = function (index) {
        debugger;
        if ($scope.refList && $scope.refList.length > 0) {
            $scope.refList.pop(index);
            $scope.productFormList.pop(index);
            if (index === 0) {
                $scope.addProduct();
            }
            console.log("reference List after Delete", $scope.refList);
            console.log("Product List after Delete", $scope.productFormList);
            return;
        }
        
        var itemId = $scope.productFormList[index].TID;
        var tenderNo = $scope.TENDER_NO;
        var action = $scope.saveAction;

        if (itemId) {
            if (action == 'Update') {
                $http.post('/api/QuotationApi/updateItemsById?tenderNo=' + tenderNo + '&id=' + itemId)
                    .then(function (response) {
                        var message = response.data.MESSAGE;
                        var deletedProduct = $scope.productFormList.splice(index, 1)[0];
                        delete deletedProduct.ID;
                        //$scope.productFormList.splice(index, 1);
                        displayPopupNotification(message, "success");
                    }).catch(function (error) {
                        var message = 'Error in displaying project!!';
                        displayPopupNotification(message, "error");
                    });
            }
        } else {
            var deletedProduct = $scope.productFormList.splice(index, 1)[0];
            delete deletedProduct.ID;
            $scope.productFormList.splice(index, 1); // Remove the row at the specified index
            if (index === 0) {
                $scope.addProduct(); // Reload the Add Product page if the first row is deleted
            }
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

    $scope.isSaving = false;

    $scope.saveData = function () {
        if ($scope.isSaving) return;
        $scope.isSaving = true;
        debugger;
        console.log($scope.productFormList);
        if (!$('#validDt').val()) {
            displayPopupNotification("DeliveryDate is required", "warning");
            $scope.isSaving = false;
            return;
        }
        if (!$scope.TENDER_NO || $scope.TENDER_NO == "") {
            displayPopupNotification("Please select a Quotation", "warning");
            $scope.isSaving = false;
            return;
        }
        for (var i = 0; i < $scope.productFormList.length; i++) {
            if ($scope.productFormList[i].QUANTITY <= 0 || $scope.productFormList[i].QUANTITY == null) {
                displayPopupNotification("Please specify the quantity", "warning");
                $scope.isSaving = false;
                return;
            }
        }
        var formData = {
            ID: $scope.ID || 0,
            FORM_CODE: $scope.FORM_CODE,
            TENDER_NO: $scope.TENDER_NO,
            ISSUE_DATE: $('#englishdatedocument').val(),
            VALID_DATE: $('#validDt').val(),
            REMARKS: $('#remarks').val(),
            LOG_REMARKS: '',
            Items: [],
            References: $scope.refList || [],
            LOCAL_FLAG: $scope.LOCAL_FLAG,
        };

        var count = 0;
        var ProductEmpty = false;
        // Loop over the productFormList if it's not empty
        if ($scope.productFormList && $scope.productFormList.length > 0) {
            var totalFiles = $scope.productFormList.length;

            angular.forEach($scope.productFormList, function (itemList) {
                var fileInput = document.getElementById('image_' + itemList.ID);
                var file = fileInput.files[0];
                if (itemList.ItemCode == "" || typeof itemList.ItemCode === "undefined" || itemList.ItemCode === null) {
                    displayPopupNotification("Product Name is required", "warning");
                    ProductEmpty = true;
                    $scope.isSaving = false;
                    return;// Set flag to true if any rate is empty
                }
                if (!ProductEmpty) {
                    if (file) {
                        var reader = new FileReader();
                        reader.onload = function () {
                            var itemListData = {
                                ID: itemList.TID,
                                ITEM_CODE: itemList.ItemCode,
                                SPECIFICATION: itemList.SPECIFICATION,
                                IMAGE: reader.result.split(',')[1],
                                UNIT: itemList.UNIT,
                                IMAGE_NAME: itemList.IMAGE_NAME,
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
                            };
                            formData.Items.push(itemListData);
                            count++;

                            if (count === totalFiles) {
                                //saveFormData(formData);
                                $scope.tempFormData = formData;
                                $scope.popupRemarks = "";
                                $('#remarksModal').modal('show');
                            }
                        };
                        reader.onerror = function (error) {
                            displayPopupNotification("Error reading file!!", "error");
                        };

                        reader.readAsDataURL(file); // Convert file to base64
                    } else {
                        var itemListData = {
                            ID: itemList.TID,
                            ITEM_CODE: itemList.ItemCode,
                            SPECIFICATION: itemList.SPECIFICATION,
                            IMAGE: null,
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
                        };
                        formData.Items.push(itemListData);
                        count++;
                        console.log("FormData",formData);
                        if (count === totalFiles) {
                            //saveFormData(formData);
                            $scope.tempFormData = formData;
                            $scope.popupRemarks = "";
                            $('#remarksModal').modal('show');
                        }
                    }
                }
            });
        } else {
            $scope.isSaving = false;
        }
    };
    $scope.confirmSave = function () {

        if (!$scope.popupRemarks || $scope.popupRemarks.trim() === "") {
            displayPopupNotification("Remarks are required", "warning");
            $scope.isSaving = false;
            return;
        }

        $scope.tempFormData.LOG_REMARKS = $scope.popupRemarks;
        $('#remarksModal').modal('hide');
        saveFormData($scope.tempFormData);
    };

    function saveFormData(formData) {
        $http.post('/api/QuotationApi/SaveItemData', formData)
            .then(function (response) {
                var message = response.data.message;
                $scope.createPanel = false;
                $scope.tablePanel = true;
                displayPopupNotification(message, "success");
                window.location.href = "/QuotationManagement/Home/Index#!QM/QuotationSetup"
            })
            .catch(function (error) {
                console.log(error);
                var message = error.data?.ExceptionMessage ?? error.data?.Message;
                displayPopupNotification(message, "error");
            })
            .finally(function () {
                $scope.isSaving = false;
            });
    }

    $scope.Cancel = function () {
        $("#englishdatedocument").data("kendoDatePicker").value(null);
        $("#validDt").data("kendoDatePicker").value(null);
        $("#nepaliDate").val('');
        $("#issueNep").val('');

        // Clear the content of productFormList
        $scope.productFormList.forEach(function (product) {
            product.ItemCode = '';
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
    //Kendo table 
    $http.post('/api/QuotationApi/ListAllTenders')
        .then(function (response) {
            var tenders = response.data;
            if (tenders && tenders.length > 0) {
                $scope.dataSource.data(tenders); // Set the data to the dataSource
            } else {
                console.log("No tenders found.");
            }
        })

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
            { field: "TENDER_NO", title: "Tender No", type: "string" },
            {
                field: "ISSUE_DATE", title: "Issue Date", type: "string",
                template: "#=kendo.toString(kendo.parseDate(ISSUE_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(ISSUE_DATE),'dd MMM yyyy') #",
            },
            {
                field: "VALID_DATE", title: "To be Delivered Date", type: "string",
                template: "#=kendo.toString(kendo.parseDate(VALID_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(VALID_DATE),'dd MMM yyyy') #",
            },
            {
                field: "CREATED_DATE", title: "Created Date", type: "string",
                template: "#=kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy') #",
            },
            { field: "APPROVED_STATUS", title: "Approved Status", type: "string" },
            {
                title: "Actions",
                width: 120,
                template: "<a class='btn btn-sm btn-info view-btn' data-id='#= TENDER_NO #'><i class='fa fa-eye'></i></a>&nbsp;<a class='btn btn-sm btn-warning edit-btn' data-id='#= TENDER_NO #'><i class='fa fa-edit'></i></a>&nbsp;<a class='btn btn-sm btn-danger delete-btn' data-id='#= TENDER_NO #'><i class='fa fa-trash'></i></a>"
            }
        ]
    });

    // Handle click event for the delete button
    $("#kGrid").on("click", ".delete-btn", function () {
        var deleteButton = $(this);
        var id = $(this).data("id");

        // Create the popover element with custom HTML content
        var popoverContent = `
        <div class="popover-delete-confirm">
            <p>Delete?</p>
            <div class="popover-buttons">
                <button type="button" class="btn btn-danger confirm-delete">Yes</button>
                <button type="button" class="btn btn-secondary cancel-delete">No</button>
            </div>
        </div>
    `;
        deleteButton.popover({
            container: 'body',
            placement: 'bottom',
            html: true,
            content: popoverContent
        });

        // Show popover
        deleteButton.popover('show');

        // Handle click event on the "Yes" button
        $(document).on('click', '.confirm-delete', function () {
            $http.post('/api/QuotationApi/deleteQuotationId?tenderNo=' + id)
                .then(function (response) {
                    var message = response.data.MESSAGE; // Extract message from response
                    displayPopupNotification(message, "success");
                    setTimeout(function () {
                        window.location.reload();
                    }, 5000)
                }).catch(function (error) {
                    var message = 'Error in displaying project!!'; // Extract message from response
                    displayPopupNotification(message, "error");
                });
            deleteButton.popover('hide');
        });

        // Handle click event on the "No" button
        $(document).on('click', '.cancel-delete', function () {
            // Hide the popover
            deleteButton.popover('hide');
        });

    });
    // Handle click event for the view button
    $scope.product = {};
    $("#kGrid").on("click", ".view-btn", function () {
        var id = $(this).data("id");
        $http.get('/api/QuotationApi/GetQuotationById?tenderNo=' + id)
            .then(function (response) {
                var quotation = response.data[0];
                console.log(quotation);
                $scope.TENDER_NO = quotation.TENDER_NO;
                $scope.ISSUE_DATE = formatDate(quotation.ISSUE_DATE);
                $scope.VALID_DATE = formatDate(quotation.VALID_DATE);
                $scope.NEPALI_DATE = quotation.NEPALI_DATE;
                $scope.TXT_REMARKS = quotation.REMARKS;
                $scope.APPROVED_STATUS = quotation.APPROVED_STATUS;
                var id = 1;
                $scope.panelMode = 'view';
                $scope.productFormList = [];
                for (var i = 0; i < quotation.Items.length; i++) {
                    var itemList = quotation.Items[i];
                    var imageUrl = window.location.protocol + "//" + window.location.host + "/Areas/NeoERP.QuotationManagement/Image/Items/" + itemList.IMAGE;
                    $scope.productFormList.push({
                        TID: itemList.ID,
                        ID: id,
                        ItemCode: itemList.ITEM_CODE,
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
    });
    // Handle click event for the edit button
    $("#kGrid").on("click", ".edit-btn", function () {
        var id = $(this).data("id");
        $http.get('/api/QuotationApi/GetQuotationById?tenderNo=' + id)
            .then(function (response) {
                var quotation = response.data[0];
                $scope.ID = quotation.ID;
                $scope.TENDER_NO = quotation.TENDER_NO;
                var issueDate = new Date(quotation.ISSUE_DATE);
                var validDate = new Date(quotation.VALID_DATE);
                var issueDate = $filter('date')(new Date(quotation.ISSUE_DATE), 'dd-MMM-yyyy');
                var validDate = $filter('date')(new Date(quotation.VALID_DATE), 'dd-MMM-yyyy');

                // Set values for input fields with specific IDs
                $('#englishdatedocument').val(issueDate);
                $('#nepaliDate').val(quotation.NEPALI_DATE);
                $("#validDt").val(validDate);

                $scope.TXT_REMARKS = quotation.REMARKS;
                var id = 1;
                $scope.panelMode = 'edit';
                $scope.saveAction = "Update";
                $scope.createEdit = true; // Corrected typo here
                $scope.productFormList = [];
                if (quotation.Items.length === 0) {
                    // If there are no items, call addProduct directly
                    $scope.addProduct();
                } else {
                    for (var i = 0; i < quotation.Items.length; i++) {
                        var itemList = quotation.Items[i];
                        var imageUrl = window.location.protocol + "//" + window.location.host + "/Areas/NeoERP.QuotationManagement/Image/Items/" + itemList.IMAGE;
                        $scope.productFormList.push({
                            TID: itemList.ID,
                            ID: id,
                            ItemCode: itemList.ITEM_CODE,
                            SPECIFICATION: itemList.SPECIFICATION,
                            IMAGE: itemList.IMAGE,
                            IMAGE_NAME: itemList.IMAGE,
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
    });

    $scope.openImage = function (imageUrl) {
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

    // Reference Code from here
    // Voucher ComboBox
    $scope.refrenceCodeDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: function () {
                    return "/api/QuotationApi/VoucherList";
                },
                dataType: "json",
                data: function () {
                    return {
                        code: $scope.selectedTemplateCode || '',
                        row: $scope.ROW
                    };
                }
            },
            parameterMap: function (data, type) {
                return data;
            }
        },
        autoClose: true,
        autoBind: true,
        optionLabel: "--Select Voucher--"
    });

    // Item ComboBox
    $scope.ItemCodeOption = {
        dataTextField: 'ItemDescription',
        dataValueField: 'ItemCode',
        placeholder: 'Select Product Code',
        filter: 'contains',
        suggest: false,
        minLength: 3,
        dataSource: {
            transport: {
                read: {
                    url: '/api/QuotationApi/ItemDetails',
                    dataType: 'json'
                }
            }
        }
    };
    $http.get('/api/QuotationApi/TemplateOptions')
        .then(function (response) {
            let allData = [];

            if (Array.isArray(response.data)) {
                allData = response.data;
            } else if (response.data && typeof response.data === 'object') {
                allData = [response.data];
            }

            $scope.quotationOptionData = allData;
            console.log("data", allData);

            // ========== DOCUMENT DROPDOWN ==========
            const quotationDoc = allData.find(item =>
                item.FORM_EDESC && item.FORM_EDESC.toLowerCase()
            );

            const documentOptions = [{
                FORM_CODE: quotationDoc?.ID || 'default',
                FORM_EDESC: 'Purchase Indent'
            }];

            const $documentDropdown = $("#refrenceTypeMultiSelect");

            if ($documentDropdown.length) {
                if ($documentDropdown.data("kendoDropDownList")) {
                    $documentDropdown.data("kendoDropDownList").destroy();
                    $documentDropdown.empty();
                }

                $documentDropdown.kendoDropDownList({
                    dataSource: documentOptions,
                    optionLabel: '-- Select Document --',
                    dataTextField: "FORM_EDESC",
                    dataValueField: "FORM_CODE",
                    autoBind: true
                });
            } else {
                console.warn("#refrenceTypeMultiSelect not found in DOM.");
            }

            // ========== TEMPLATE DROPDOWN ==========
            const parsedTemplates = allData.map(item => ({
                FORM_CODE: `${item.PREFIX || ''};${item.SUFFIX || ''}`,
                FORM_EDESC: item.FORM_EDESC || '(No Name)'
            }));

            // Add "ALL DOCUMENTS" at the top
            parsedTemplates.unshift({
                FORM_CODE: 'all',
                FORM_EDESC: 'ALL DOCUMENTS'
            });

            console.log("Parsed Templates for Dropdown:", parsedTemplates);

            const $templateDropdown = $("#TemplateTypeMultiSelect");

            if ($templateDropdown.length) {
                if ($templateDropdown.data("kendoDropDownList")) {
                    $templateDropdown.data("kendoDropDownList").destroy();
                    $templateDropdown.empty();
                }

                $templateDropdown.kendoDropDownList({
                    dataSource: parsedTemplates,
                    optionLabel: '-- Select Template --',
                    dataTextField: "FORM_EDESC",
                    dataValueField: "FORM_CODE",
                    autoBind: true,
                    change: function () {
                        const selectedTemplate = this.value();
                        $scope.selectedTemplateCode = selectedTemplate;

                        const docDropDown = $documentDropdown.data("kendoDropDownList");
                        if (docDropDown) {
                            docDropDown.value(quotationDoc?.ID || 'default');
                        }
                        $scope.voucher_no = "";
                        $scope.refrenceCodeDataSource.read();
                        const voucherDropDown = $("input[ng-model='voucher_no']").data("kendoDropDownList");
                        if (voucherDropDown) {
                            voucherDropDown.value("");
                            voucherDropDown.open();
                        }
                    }
                });
            } else {
                console.warn("#TemplateTypeMultiSelect not found in DOM.");
            }
        })
        .catch(function (error) {
            console.error('Error fetching template options:', error);
        });
    
    // Reference grid
    function refFunc() {
        debugger;
        $scope.referenceGridOptions = {
            dataSource: {
                transport: {
                    read: {
                        type: "POST",
                        url: "/api/QuotationApi/getTemplateData",
                        contentType: "application/json; charset=utf-8",
                        dataType: 'json'
                    },
                    parameterMap: function (options, type) {
                        const prefixSuffix = $("#TemplateTypeMultiSelect").val() || 'all';
                        const [prefix, suffix] = prefixSuffix.split(";");

                        const payload = {
                            prefix: prefixSuffix || null,
                            Row: $scope.ROW || null,
                            Reference: null,
                            Voucher_no: $scope.voucher_no || null,
                            Item: $scope.DocumentReference?.ITEM_DESC || null,
                            Name: $scope.DocumentReference?.TEMPLATE || null,
                            fromDate: $scope.FROM_DATE || null,
                            toDate: $scope.TO_DATE || null
                        };

                        return JSON.stringify(payload);
                    }

                },
                pageSize: 50,
                serverPaging: false,
                serverSorting: false,
                schema: {
                    model: {
                        fields: {
                            TENDER_NO: { type: "string", defaultValue: "" },
                            FORM_CODE: { type: "string", defaultValue: "" },
                            ITEM_EDESC: { type: "string", defaultValue: "" },
                            MU_CODE: { type: "string", defaultValue: "" },
                            QUANTITY: { type: "number", defaultValue: 0 },
                            UNIT_PRICE: { type: "number", defaultValue: 0 },
                            TOTAL_PRICE: { type: "number", defaultValue: 0 },
                            REMARKS: { type: "string", defaultValue: "" },
                            SERIAL_NO: { type: "string", defaultValue: "" },
                            SPECIFICATION: { type: "string", defaultValue: "" },
                            ITEM_CODE: { type: "string", defaultValue: "" }
                        }
                    },
                    parse: function (data) {
                        console.log("Parse Data: ", data);
                        let flatten = [];
                        const itemLookup = {};
                        ($scope.itemDescriptionList || []).forEach(item => {
                            itemLookup[item.ItemCode] = item.ItemDescription;
                        });
                        const values = Array.isArray(data) ? data : Object.values(data);
                        values.forEach(parent => {
                            if (parent.Items && parent.Items.length) {
                                parent.Items.forEach(item => {
                                    const itemCode = item.ITEM_CODE || '';
                                    const itemEDESC = itemLookup[itemCode] || item.SPECIFICATION || '';

                                    flatten.push({
                                        FORM_CODE: parent.FORM_CODE || '',
                                        TENDER_NO: parent.TENDER_NO || '',
                                        ITEM_EDESC: itemEDESC,
                                        MU_CODE: item.UNIT || '',
                                        QUANTITY: item.QUANTITY || 0,
                                        UNIT_PRICE: item.PRICE || 0,
                                        TOTAL_PRICE: (item.QUANTITY || 0) * (item.PRICE || 0),
                                        REMARKS: parent.REMARKS || '',
                                        SERIAL_NO: item.ID || '',
                                        ITEM_CODE: itemCode,
                                        SPECIFICATION: item.SPECIFICATION || '',
                                        IMAGE_NAME: item.IMAGE_NAME || '',
                                        CATEGORY: item.CATEGORY || '',
                                        BRAND_NAME: item.BRAND_NAME || '',
                                        INTERFACE: item.INTERFACE || '',
                                        TYPE: item.TYPE || '',
                                        LAMINATION: item.LAMINATION || '',
                                        ITEM_SIZE: item.ITEM_SIZE || '',
                                        THICKNESS: item.THICKNESS || '',
                                        COLOR: item.COLOR || '',
                                        GRADE: item.GRADE || '',
                                        SIZE_LENGTH: item.SIZE_LENGTH || 0,
                                        SIZE_WIDTH: item.SIZE_WIDTH || 0,
                                        PRICE: item.PRICE || 0
                                    });
                                });
                            }
                        });
                        const seen = new Set();
                        const unique = [];
                        flatten.forEach(row => {
                            const key = row.TENDER_NO + "_" + row.SERIAL_NO;
                            if (!seen.has(key)) {
                                seen.add(key);
                                unique.push(row);
                            }
                        });
                        console.log(unique);
                        return unique;
                    }
                },
                group: {
                    field: "TENDER_NO",
                    dir: "asc",
                    headerTemplate: "#= value #"
                },
            },
            sortable: true,
            pageable: true,
            reorderable: true,
            groupable: true,
            resizable: true,
            filterable: {
                extra: false,
                operators: {
                    number: { eq: "Is equal to", neq: "Is not equal to", gte: "Is greater than or equal to", gt: "Is greater than", lte: "Is less than or equal", lt: "Is less than" },
                    string: { eq: "Is equal to", neq: "Is not equal to", startswith: "Starts with", contains: "Contains", doesnotcontain: "Does not contain", endswith: "Ends with" },
                    date: { eq: "Is equal to", neq: "Is not equal to", gte: "Is after or equal to", gt: "Is after", lte: "Is before or equal to", lt: "Is before" }
                }
            },
            columnMenu: true,
            persistSelection: true,
            scrollable: { virtual: true },
            dataBound: function (e) {
                var grid = this;
                var view = grid.dataSource.view();

                view.forEach(function (item) {
                    var key = item.TENDER_NO + "_" + item.SERIAL_NO;

                    if (checkedItems.some(function (x) { return x.key === key; })) {
                        var uid = item.uid;
                        grid.tbody
                            .find("tr[data-uid='" + uid + "']")
                            .addClass("k-state-selected")
                            .find(".checkbox")
                            .prop("checked", true);
                    }
                });
                $(".checkbox").on("click", selectRow);

                if (grid.dataSource.total() === 0) {
                    var colCount = grid.columns.length + 1;
                    grid.wrapper
                        .find('tbody')
                        .append(
                            '<tr class="kendo-data-row" style="font-size:12px;">' +
                            '<td colspan="' + colCount + '" class="alert alert-danger">' +
                            'Sorry, No Data Found For Given Filter.' +
                            '</td></tr>'
                        );
                }
            }
            ,
            columns: [
                {
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.TENDER_NO}' class='checkbox row-checkbox_" + dataItem.ITEM_CODE + "'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                { field: "SERIAL_NO", title: "S_No" , width: 70},
                { field: "TENDER_NO", title: "Voucher No" },
                { field: "ITEM_EDESC", title: "Item" },
                { field: "SPECIFICATION", title: "Specification" },
                { field: "MU_CODE", title: "Unit" },
                { field: "QUANTITY", title: "Quantity", attributes: { style: "text-align:right;" } },
                { field: "REMARKS", title: "Remark" },
                {
                    field: "IMAGE_NAME",
                    hidden: true
                },{
                    field: "FORM_CODE",
                    hidden: true
                },
                {
                    field: "CATEGORY",
                    hidden: true
                },
                {
                    field: "BRAND_NAME",
                    hidden: true
                },
                {
                    field: "INTERFACE",
                    hidden: true
                },
                {
                    field: "TYPE",
                    hidden: true
                },
                {
                    field: "LAMINATION",
                    hidden: true
                },
                {
                    field: "ITEM_SIZE",
                    hidden: true
                },
                {
                    field: "THICKNESS",
                    hidden: true
                },
                {
                    field: "COLOR",
                    hidden: true
                },
                {
                    field: "GRADE",
                    hidden: true
                },
                {
                    field: "SIZE_LENGTH",
                    hidden: true
                },
                {
                    field: "SIZE_WIDTH",
                    hidden: true
                },
                {
                    field: "PRICE",
                    hidden: true
                }
            ]
        };
    }

    //Function to selct row
    function selectRow() {
        var checked = this.checked,
            row = $(this).closest("tr"),
            grid = $("#referenceGrid").data("kendoGrid"),
            dataItem = grid.dataItem(row);

        // build the unique key
        var key = dataItem.TENDER_NO + "_" + dataItem.SERIAL_NO;

        if (checked) {
            // skip if already in checkedItems
            if (!checkedItems.some(function (x) { return x.key === key; })) {
                checkedItems.push({
                    key: key,
                    REF_FORM_CODE: dataItem.FORM_CODE || '',
                    TENDER_NO: dataItem.TENDER_NO || '',
                    SERIAL_NO: dataItem.SERIAL_NO || '',
                    ITEM_CODE: dataItem.ITEM_CODE || '',
                    SPECIFICATION: dataItem.SPECIFICATION || '',
                    IMAGE: dataItem.IMAGE_NAME || '',
                    IMAGE_LINK: dataItem.IMAGE
                        ? window.location.protocol + "//" + window.location.host +
                        "/Areas/NeoERP.QuotationManagement/Image/Items/" + dataItem.IMAGE
                        : '',
                    UNIT: dataItem.MU_CODE || '',
                    QUANTITY: dataItem.QUANTITY || 0,
                    CATEGORY: dataItem.CATEGORY || '',
                    BRAND_NAME: dataItem.BRAND_NAME || '',
                    INTERFACE: dataItem.INTERFACE || '',
                    TYPE: dataItem.TYPE || '',
                    LAMINATION: dataItem.LAMINATION || '',
                    ITEM_SIZE: dataItem.ITEM_SIZE || '',
                    THICKNESS: dataItem.THICKNESS || '',
                    COLOR: dataItem.COLOR || '',
                    GRADE: dataItem.GRADE || '',
                    SIZE_LENGTH: dataItem.SIZE_LENGTH || 0,
                    SIZE_WIDTH: dataItem.SIZE_WIDTH || 0,
                    PRICE: dataItem.PRICE || 0
                });
            }
            row.addClass("k-state-selected");
        } else {
            // remove from checkedItems by key
            checkedItems = checkedItems.filter(function (x) { return x.key !== key; });
            row.removeClass("k-state-selected");
        }

        console.log("Items checked:", checkedItems);
    }

    /*function selectRow() {
        var checked = this.checked,
            row = $(this).closest("tr"),
            grid = $("#referenceGrid").data("kendoGrid"),
            dataItem = grid.dataItem(row);
        if (checked) {
            if (checkedItems.length > 0) {
                if (jQuery.inArray(dataItem.ITEM_CODE, checkedCutomer) === -1) {
                    checkedCutomer.push(dataItem.ITEM_CODE);
                    return false;
                }
                else {

                }
            }
            else {
                checkedCutomer.push(dataItem.ITEM_CODE);
            }
            row.addClass("k-state-selected");
            $(this).attr('checked', true);
            var imageUrl = dataItem.IMAGE_NAME ? window.location.protocol + "//" + window.location.host + "/Areas/NeoERP.QuotationManagement/Image/Items/" + dataItem.IMAGE_NAME : '';
            checkedIds[dataItem.ORDER_NO] = checked;
            console.log("Data Items",dataItem);
            checkedItems.push({
                "TENDER_NO": dataItem.TENDER_NO || '',
                "SERIAL_NO": dataItem.ID || '',
                "ITEM_CODE": dataItem.ITEM_CODE || '',
                "SPECIFICATION": dataItem.SPECIFICATION || '',
                "IMAGE": dataItem.IMAGE_NAME || '',
                "IMAGE_LINK": imageUrl || '',
                "UNIT": dataItem.MU_CODE || '',
                "QUANTITY": dataItem.QUANTITY || 0,
                "CATEGORY": dataItem.CATEGORY || '',
                "BRAND_NAME": dataItem.BRAND_NAME || '',
                "INTERFACE": dataItem.INTERFACE || '',
                "TYPE": dataItem.TYPE || '',
                "LAMINATION": dataItem.LAMINATION || '',
                "ITEM_SIZE": dataItem.ITEM_SIZE || '',
                "THICKNESS": dataItem.THICKNESS || '',
                "COLOR": dataItem.COLOR || '',
                "GRADE": dataItem.GRADE || '',
                "SIZE_LENGTH": dataItem.SIZE_LENGTH || 0,
                "SIZE_WIDTH": dataItem.SIZE_WIDTH || 0,
                "PRICE": dataItem.PRICE || 0
            });
            console.log("Items checked",checkedItems);
        } else {
            //-remove selection
            for (var i = 0; i < checkedItems.length; i++) {
                if (checkedItems[i].VOUCHER_NO == dataItem.VOUCHER_NO && checkedItems[i].SERIAL_NO == dataItem.SERIAL_NO) {
                    checkedItems.splice(i, 1);
                }
            }
            row.removeClass("k-state-selected");
        }
    }*/
    $scope.ReSetCheck = function () {
        checkedItems = [];
        checkedCutomer = [];
    }
    // Take Reference part
    $scope.bindRefrenceDataToTemplate = function () {
        checkedItems;
        $rootScope.refCheckedItem = checkedItems;

        if (checkedItems.length <= 0)
            return displayPopupNotification("Please choose the item.", "warning");
        showloader();
        getselectedReference(checkedItems);
        $("#RefrenceModel").modal('hide');
        checkedItems = [];
        checkedCutomer = [];
    }


    function getselectedReference(checkedItems) {
        if (Array.isArray($scope.productFormList)) {
            $scope.productFormList = $scope.productFormList.filter(function (p) {
                return (p.TENDER_NO && p.SERIAL_NO) ||
                    (p.TENDER_NO && !p.SERIAL_NO) ||
                    (!p.TENDER_NO && p.SERIAL_NO);
            });
        }

        console.log("Checked Items: ", checkedItems);

        checkedItems.forEach(function (dataItem) {
            const imageLink = dataItem.IMAGE_LINK || '';

            const exists = $scope.productFormList.some(function (p) {
                return p.TENDER_NO === dataItem.TENDER_NO &&
                    p.SERIAL_NO === dataItem.SERIAL_NO;
            });
            if (exists) return;

            const maxId = Math.max(0, ...$scope.productFormList.map(p => p.ID || 0));
            $scope.counterProduct = maxId + 1;
            debugger;
            const productData = {
                ID: $scope.counterProduct,
                TENDER_NO: dataItem.TENDER_NO || '',
                SERIAL_NO: dataItem.SERIAL_NO || '',
                ItemCode: dataItem.ITEM_CODE || '',
                SPECIFICATION: dataItem.SPECIFICATION || '',
                IMAGE: dataItem.IMAGE || '',
                IMAGE_LINK: imageLink,
                UNIT: dataItem.UNIT || '',
                QUANTITY: dataItem.QUANTITY || 0,
                CATEGORY: dataItem.CATEGORY || '',
                BRAND_NAME: dataItem.BRAND_NAME || '',
                INTERFACE: dataItem.INTERFACE || '',
                TYPE: dataItem.TYPE || '',
                LAMINATION: dataItem.LAMINATION || '',
                ITEM_SIZE: dataItem.ITEM_SIZE || '',
                THICKNESS: dataItem.THICKNESS || '',
                COLOR: dataItem.COLOR || '',
                GRADE: dataItem.GRADE || '',
                SIZE_LENGTH: dataItem.SIZE_LENGTH || 0,
                SIZE_WIDTH: dataItem.SIZE_WIDTH || 0,
                PRICE: dataItem.PRICE || 0
            };
            const refData = {
                REF_FORM_CODE: dataItem.REF_FORM_CODE,
                TENDER_NO: $scope.TENDER_NO,
                REFERENCE_NO: dataItem.TENDER_NO || '',
                SERIAL_NO: dataItem.SERIAL_NO || '',
                ITEM_CODE: dataItem.ITEM_CODE || '',
                SPECIFICATION: dataItem.SPECIFICATION || '',
                IMAGE: dataItem.IMAGE || '',
                IMAGE_NAME: imageLink,
                UNIT: dataItem.UNIT || '',
                QUANTITY: dataItem.QUANTITY || 0,
                REF_QTY: dataItem.QUANTITY || 0,
                CATEGORY: dataItem.CATEGORY || '',
                BRAND_NAME: dataItem.BRAND_NAME || '',
                INTERFACE: dataItem.INTERFACE || '',
                TYPE: dataItem.TYPE || '',
                LAMINATION: dataItem.LAMINATION || '',
                ITEM_SIZE: dataItem.ITEM_SIZE || '',
                THICKNESS: dataItem.THICKNESS || '',
                COLOR: dataItem.COLOR || '',
                GRADE: dataItem.GRADE || '',
                SIZE_LENGTH: dataItem.SIZE_LENGTH || 0,
                SIZE_WIDTH: dataItem.SIZE_WIDTH || 0,
                PRICE: dataItem.PRICE || 0
            };
            console.log("Pushing-------------", productData);


            //$scope.productFormList.push(angular.copy(productData));

            $scope.productFormList.push(productData);

            $scope.refList.push(refData);

        });
        console.log("productFormLIst-----------------------", $scope.productFormList);

        console.log("Final List:", $scope.productFormList);
        console.log("Ref List: ", $scope.refList);
        hideloader();

    }

    $scope.dateFilterOptions = [
        { value: 'today', label: 'Today' },
        { value: 'yesterday', label: 'Yesterday' },
        { value: 'this_week', label: 'This Week' },
        { value: 'this_month', label: 'This Month' },
        { value: 'this_year', label: 'This Year' }
    ];
    $scope.dateFilter = 'today';
    function generateNepaliMonths() {
        const nepaliMonths = ['Baishakh', 'Jestha', 'Ashadh', 'Shrawan', 'Bhadra', 'Ashwin', 'Kartik', 'Mangsir', 'Poush', 'Magh', 'Falgun', 'Chaitra'];

        try {
            const todayAD = new Date();
            const adString = todayAD.getFullYear() + '-' +
                String(todayAD.getMonth() + 1).padStart(2, '0') + '-' +
                String(todayAD.getDate()).padStart(2, '0');

            const todayBS = AD2BS(adString);
            const bsMonth = parseInt(todayBS.split('-')[1], 10);

            const nepaliMonthOptions = [];
            for (let i = 0; i < bsMonth; i++) {
                nepaliMonthOptions.push({
                    value: 'nepali_month_' + i,
                    label: nepaliMonths[i]
                });
            }

            $scope.$applyAsync(() => {
                $scope.dateFilterOptions = $scope.dateFilterOptions.concat(nepaliMonthOptions);
                $scope.dateFilterOptions.push({ value: 'custom', label: 'Custom' });
            });
        } catch (e) {
            console.error('Nepali date conversion failed:', e);
        }
    }
    generateNepaliMonths();
    
    
    $scope.setDateFilter = function (isCustom = false) {
        let fromDate = null, toDate = null;
        let fromDateAD = null, toDateAD = null;
        if (isCustom) {
            fromDate = $("#FromDateVoucher").val();
            toDate = $("#ToDateVoucher").val();
            fromDateAD = $("#fromInputDateVoucher").val();
            toDateAD = $("#toInputDateVoucher").val();

            $scope.dateFilter = 'custom';
        } else {
            const today = new Date();
            const formatDate = date => $filter('date')(date, 'yyyy-MM-dd');
            const parseDate = date => !isNaN(new Date(date).getTime()) ? formatDate(new Date(date)) : null;

            switch ($scope.dateFilter) {
                case 'today': {
                    const dateAD = formatDate(today);
                    const dateBS = AD2BS(dateAD);
                    fromDate = toDate = dateAD;
                    fromDateAD = toDateAD = dateBS;
                    $scope.FROM_DATE = fromDate;
                    $scope.TO_DATE = toDate;
                    $scope.startdate = fromDateAD;
                    $scope.enddate = toDateAD;
                    break;
                }

                case 'yesterday': {
                    const yesterday = new Date(today);
                    yesterday.setDate(today.getDate() - 1);
                    const dateAD = formatDate(yesterday);
                    const dateBS = AD2BS(dateAD);
                    fromDate = toDate = dateAD;
                    fromDateAD = toDateAD = dateBS;
                    $scope.FROM_DATE = fromDate;
                    $scope.TO_DATE = toDate;
                    $scope.startdate = fromDateAD;
                    $scope.enddate = toDateAD;
                    break;
                }

                case 'this_week': {
                    const firstDayOfWeek = new Date(today);
                    firstDayOfWeek.setDate(today.getDate() - today.getDay());
                    fromDate = formatDate(firstDayOfWeek);
                    toDate = formatDate(today);
                    fromDateAD = AD2BS(fromDate);
                    toDateAD = AD2BS(toDate);
                    $scope.FROM_DATE = fromDate;
                    $scope.TO_DATE = toDate;
                    $scope.startdate = fromDateAD;
                    $scope.enddate = toDateAD;
                    break;
                }

                case 'last_week': {
                    const lastWeekStart = new Date(today);
                    lastWeekStart.setDate(today.getDate() - 7 - today.getDay());
                    const lastWeekEnd = new Date(lastWeekStart);
                    lastWeekEnd.setDate(lastWeekStart.getDate() + 6);
                    fromDate = formatDate(lastWeekStart);
                    toDate = formatDate(lastWeekEnd);
                    fromDateAD = AD2BS(fromDate);
                    toDateAD = AD2BS(toDate);
                    $scope.FROM_DATE = fromDate;
                    $scope.TO_DATE = toDate;
                    $scope.startdate = fromDateAD;
                    $scope.enddate = toDateAD;
                    break;
                }

                case 'this_month': {
                    const todayAD = new Date();
                    const todayADStr = $filter('date')(todayAD, 'yyyy-MM-dd');
                    const todayBSStr = AD2BS(todayADStr);
                    const [bsYear, bsMonth] = todayBSStr.split('-').map(Number);

                    const bsFromDate = `${bsYear}-${String(bsMonth).padStart(2, '0')}-01`;
                    fromDateAD = bsFromDate;
                    fromDate = BS2AD(bsFromDate); // Convert to AD

                    // Calculate last date of the BS month by testing backwards
                    for (let d = 32; d > 27; d--) {
                        const potentialBSDate = `${bsYear}-${String(bsMonth).padStart(2, '0')}-${String(d).padStart(2, '0')}`;
                        try {
                            const adToDate = BS2AD(potentialBSDate);
                            if (adToDate) {
                                toDate = $filter('date')(new Date(adToDate), 'yyyy-MM-dd');
                                toDateAD = potentialBSDate;
                                break;
                            }
                        } catch (e) {
                            continue;
                        }
                    }

                    $scope.FROM_DATE = fromDate;
                    $scope.TO_DATE = toDate;
                    $scope.startdate = fromDateAD;
                    $scope.enddate = toDateAD;
                    break;
                }

                case 'this_year': {
                    const todayAD = new Date();
                    const todayADStr = $filter('date')(todayAD, 'yyyy-MM-dd');
                    const todayBSStr = AD2BS(todayADStr);
                    const [bsYear] = todayBSStr.split('-').map(Number);

                    const bsFromDate = `${bsYear}-01-01`;
                    const bsToDate = `${bsYear}-12-30`; // max expected date, fallback

                    fromDateAD = bsFromDate;
                    fromDate = BS2AD(bsFromDate);

                    // Find actual last day of Chaitra
                    for (let d = 32; d > 27; d--) {
                        const tryDate = `${bsYear}-12-${String(d).padStart(2, '0')}`;
                        try {
                            const adToDate = BS2AD(tryDate);
                            if (adToDate) {
                                toDate = $filter('date')(new Date(adToDate), 'yyyy-MM-dd');
                                toDateAD = tryDate;
                                break;
                            }
                        } catch (e) {
                            continue;
                        }
                    }

                    $scope.FROM_DATE = fromDate;
                    $scope.TO_DATE = toDate;
                    $scope.startdate = fromDateAD;
                    $scope.enddate = toDateAD;
                    break;
                }

                default: {
                    if ($scope.dateFilter && $scope.dateFilter.startsWith('nepali_month_')) {
                        const monthIndex = parseInt($scope.dateFilter.split('_')[2]); // 0-based index
                        const adToday = formatDate(today);
                        const bsToday = AD2BS(adToday);
                        const [bsYear] = bsToday.split('-').map(Number);
                        const bsMonth = monthIndex + 1;
                        const bsFromDate = `${bsYear}-${String(bsMonth).padStart(2, '0')}-01`;
                        const adFromDate = BS2AD(bsFromDate);
                        fromDate = formatDate(new Date(adFromDate));
                        fromDateAD = bsFromDate;

                        for (let d = 32; d > 27; d--) {
                            const bsToDate = `${bsYear}-${String(bsMonth).padStart(2, '0')}-${String(d).padStart(2, '0')}`;
                            try {
                                const adToDate = BS2AD(bsToDate);
                                if (adToDate) {
                                    toDate = formatDate(new Date(adToDate));
                                    toDateAD = bsToDate;
                                    break;
                                }
                            } catch (e) {
                                continue;
                            }
                        }
                    } else {
                        console.warn('Unknown dateFilter:', $scope.dateFilter);
                    }
                }
            }
        }
        // Final assignments
        $scope.FROM_DATE = fromDate;
        $scope.TO_DATE = toDate;
        $scope.startdate = fromDateAD;
        $scope.enddate = toDateAD;
        console.log("from", $scope.FROM_DATE);
        console.log("To", $scope.TO_DATE);
    };

    $scope.init = function () {
        if (!$scope.dateFilter) {
            $scope.dateFilter = 'today'; // Default value if not set
        }
        $scope.setDateFilter($scope.dateFilter === 'custom');
    };

    // Call it at the end of controller
    $scope.init();
    $(document).ready(function () {
        $("#fromInputDateVoucher").nepaliDatePicker({
            npdMonth: false,
            npdYear: false,
            onFocus: true,
            npdYearCount: 10,
            altFormat: "dd-MMM-YYYY",
            dateFormat: "dd-MMM-YYYY",
            onChange: function (e) {
                const nep = $("#fromInputDateVoucher").val();
                try {
                    const eng = BS2AD(nep);
                    console.log("FROM", eng);
                    $("#FromDateVoucher").val(moment(eng).format("YYYY-MM-DD"));
                    $scope.setDateFilter(true);
                } catch (e) {
                    console.warn("Invalid BS date format:", nep);
                }
            }
        });

        $("#toInputDateVoucher").nepaliDatePicker({
            npdMonth: false,
            npdYear: false,
            onFocus: true,
            npdYearCount: 10,
            altFormat: "dd-MMM-YYYY",
            dateFormat: "dd-MMM-YYYY",
            onChange: function (e) {
                const nep = $("#toInputDateVoucher").val();
                try {
                    const eng = BS2AD(nep);
                    console.log("FROM", eng);
                    $("#ToDateVoucher").val(moment(eng).format("YYYY-MM-DD"));
                    $scope.setDateFilter(true);
                } catch (e) {
                    console.warn("Invalid BS date format:", nep);
                }
            }
        });

        $("#FromDateVoucher").on("change", function () {
            const eng = $(this).val();
            try {
                const nep = AD2BS(moment(eng).format("YYYY-MM-DD"));
                $("#fromInputDateVoucher").val(moment(nep).format("DD-MMM-YYYY"));
                $("#fromInputDateVoucher").trigger("change");
            } catch (e) {
                console.warn("Invalid AD date format:", eng);
            }
        });

        $("#ToDateVoucher").on("change", function () {
            const eng = $(this).val();
            try {
                const nep = AD2BS(moment(eng).format("YYYY-MM-DD"));
                $("#toInputDateVoucher").val(moment(nep).format("DD-MMM-YYYY"));
                $("#toInputDateVoucher").trigger("change");
            } catch (e) {
                console.warn("Invalid AD date format:", eng);
            }
        });
    });

    // Search function
    $scope.bindReferenceGrid = function (isValid) {
        debugger;
        checkedItems = [];
        if (!isValid) {
            console.log('Form is invalid');
            return;
        }
        refFunc();
    };

    $scope.buildSearchParams = function () {
        return {
            prefix: $scope.selectedTemplate ? $scope.selectedTemplate.ID || $scope.selectedTemplate.id : null,
            Reference: $scope.referenceType,
            Row: $scope.rowType,
            Voucher_no: $scope.voucherNo,
            Item: $scope.itemDesc,
            Name: $scope.consigneeName,
            fromDate: $scope.dateFilter,
            toDate: $scope.adDateStart ? $filter('date')($scope.adDateStart, 'yyyy-MM-dd') : null
        };
    };
});