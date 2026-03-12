QMModule.controller('EditQuotation', function ($scope, $rootScope, $http, $filter, $timeout, $routeParams) {
    $scope.pageName = "Add Quotation";
    $scope.saveAction = "Save";
    $scope.id = "";
    $scope.QUOTATION_NO = "";
    if ($routeParams.id != undefined) {
        //$scope.id = $routeParams.id.split(new RegExp('_', 'i')).join('/');
        $scope.id = $routeParams.id;
    }
   
    else { $scope.id = "undefined"; }

    $scope.APPROVED_STATUS = 'Pending';
    $scope.idShowHide = false;

    $("#englishdatedocument").kendoDatePicker();
    $("#validDt").kendoDatePicker();
    $scope.productFormList = [];
    $scope.counterProduct = 1;
    $scope.showCustomerDetails = false;
    $scope.showSpecificationDetail = false;
    $scope.selectedCurrency = "";

    $scope.LOCAL_FLAG = 'Y';

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
                $scope.LOCAL_FLAG = selectedItem.value;
                console.log("flag (via ng-model)", $scope.LOCAL_FLAG);
                $scope.$apply();
            }
    };

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
    let itemDataCache = null; // Cache for API data

    // DropDownList configuration
    const itemSelectConfig = {
        dataTextField: "ItemDescription",
        dataValueField: "ItemCode",
        height: 300,
        autoBind: false,
        maxSelectedItems: 1,
        enable: true,
        openOnFocus: true,
        scrollable: true,
        /*virtual: {
            itemHeight: 26,
            valueMapper: function (options) {
                if (!options.value) {
                    options.success(null);
                    return;
                }
                if (itemDataCache) {
                    var index = itemDataCache.findIndex(item => item.ItemCode === options.value);
                    options.success(index === -1 ? null : index);

                    // Force rebind after short delay
                    setTimeout(function () {
                        const comboBox = $("#" + dropdownId).data("kendoDropDownList");
                        if (comboBox) {
                            comboBox.value(options.value);
                            comboBox.trigger("change");
                        }
                    }, 100);
                } else {
                    $http.get(window.location.protocol + "//" + window.location.host + "/api/QuotationApi/ItemDetails")
                        .then(function (response) {
                            itemDataCache = response.data;
                            var index = itemDataCache.findIndex(item => item.ItemCode === options.value);
                            options.success(index === -1 ? null : index);

                            // Force rebind after data is available
                            setTimeout(function () {
                                const comboBox = $("#" + dropdownId).data("kendoDropDownList");
                                if (comboBox) {
                                    comboBox.value(options.value);
                                    comboBox.trigger("change");
                                }
                            }, 100);
                        })
                        .catch(function () {
                            options.success(null);
                        });
                }
            }

        },*/
        dataSource: new kendo.data.DataSource({
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/QuotationApi/ItemDetails",
                    dataType: "json"
                }
            },
            schema: {
                parse: function (data) {
                    const uniqueData = [];
                    const seenCodes = new Set();
                    data.forEach(item => {
                        if (!seenCodes.has(item.ItemCode)) {
                            seenCodes.add(item.ItemCode);
                            uniqueData.push(item);
                        }
                    });
                    itemDataCache = uniqueData;
                    return uniqueData;
                },
                total: function (data) {
                    return data.length;
                }
            }
        }),
        filter: "contains",
        autoClose: true,
        open: function (e) {
            console.log('Dropdown opened:', e.sender.element.attr('id'));
            setTimeout(function () {
                e.sender.element.focus();
            }, 100);
        },
        select: function (e) {
            if (e.dataItem) {
                console.log('Item selected:', e.dataItem.ItemCode, e.dataItem.ItemDescription);
            } else {
                console.log('No item selected');
            }
        },
        change: function (e) {
            console.log('Value changed to:', e.sender.value());
            var selectedItem = e.sender.dataItem(); // ✅ use e.sender
            if (selectedItem) {
                console.log('Selected item:', selectedItem.ItemCode, selectedItem.ItemDescription);
                var dropdownId = e.sender.element.attr('id');
                var productId = dropdownId.replace('itemSelected_', '');
                var product = $scope.productFormList.find(p => p.ID == Number(productId)); // ✅ cast to number
                if (product) {
                    console.log('Matched product:', product);
                    console.log('Before:', product.ItemCode);
                    product.ItemCode = selectedItem.ItemCode;
                    console.log('After:', product.ItemCode);
                    product.ItemDescription = selectedItem.ItemDescription;
                    product.UNIT = selectedItem.ItemUnit || '';
                    product.SPECIFICATION = selectedItem.SPECIFICATION || '';

                    if (!$scope.$$phase) $scope.$apply(); // ✅ safer apply
                }
            }
        }

        /*change: function (e) {
            console.log('Value changed to:', e.sender.value());
            debugger;
            var selectedItem = this.dataItem();
            if (selectedItem) {
                console.log('Selected item:', selectedItem.ItemCode, selectedItem.ItemDescription);
                // Update product in AngularJS scope
                var dropdownId = e.sender.element.attr('id');
                var productId = dropdownId.replace('itemSelected_', '');
                var product = $scope.productFormList.find(p => p.ID == productId);
                if (product) {
                    console.log('Matched product:', product);
                    console.log('Before:', product.ItemCode);
                    product.ItemCode = selectedItem.ItemCode;
                    console.log('After:', product.ItemCode);
                    product.ItemDescription = selectedItem.ItemDescription;
                    product.UNIT = selectedItem.ItemUnit || '';
                    product.SPECIFICATION = selectedItem.SPECIFICATION || '';
                    $scope.$apply();
                }
            }
        }*/,
        dataBound: function (e) {
            var dropdown = e.sender.element;
            dropdown.on('click', function () {
                console.log('Dropdown clicked:', dropdown.attr('id'));
                setTimeout(function () {
                    dropdown.focus();
                }, 100);
            });
            dropdown.on('keydown', function (event) {
                console.log('Keydown:', event.keyCode, 'Focused:', document.activeElement === dropdown[0]);
                if (event.keyCode === 38 || event.keyCode === 40) {
                    event.stopPropagation();
                    var widget = e.sender;
                    var list = widget.ul;
                    var selected = list.find('.k-state-selected');
                    if (selected.length) {
                        var listContainer = list.parent();
                        var itemHeight = selected.outerHeight();
                        var listHeight = listContainer.height();
                        var selectedTop = selected.position().top;
                        var currentScroll = listContainer.scrollTop();

                        if (selectedTop < 0) {
                            listContainer.scrollTop(currentScroll + selectedTop);
                        } else if (selectedTop + itemHeight > listHeight) {
                            listContainer.scrollTop(currentScroll + selectedTop - listHeight + itemHeight);
                        }
                    }
                }
            });
            dropdown.on('focus', function () {
                console.log('Dropdown focused:', dropdown.attr('id'));
            });
        }
    };

    // Initialize dropdowns for existing products
    function initializeDropdowns() {
        $scope.productFormList.forEach(function (product) {
            var selector = `#itemSelected_${product.ID}`;
            var $element = $(selector);
            if ($element.length && !$element.data('kendoDropDownList')) {
                $element.kendoDropDownList(itemSelectConfig);
                // Set initial value if exists
                if (product.ItemCode) {
                    var dropdown = $element.data('kendoDropDownList');
                    dropdown.value(product.ItemCode);
                    dropdown.trigger('change');
                }
            }
        });
    }

    // Call initializeDropdowns when productFormList changes
    $scope.$watch('productFormList', function (newVal, oldVal) {
        if (newVal !== oldVal) {
            $timeout(function () {
                initializeDropdowns();
            }, 100);
        }
    }, true);

    // Initialize on controller load
    $scope.addProduct = function () {
        $scope.ItemSelect.dataSource.read().then(function () {
            $scope.productFormList.push({
                ID: $scope.counterProduct,
                ItemCode: "",
                ItemDescription: "",
                UNIT: "",
                IMAGE: "",
                QUANTITY: "",
                UNIT_PRICE: "",
                AMOUNT: "",
                REMARKS: ""
            });
            $scope.counterProduct++;
            $timeout(function () {
                initializeDropdowns();
            }, 100);
        });
    };

    // Update unit and specification
    $scope.updateUnit = function (product) {
        debugger;
        if (product && product.ItemCode) {
            itemSelectConfig.dataSource.fetch(function () {
                var data = itemSelectConfig.dataSource.data();
                var selectedItem = data.find(function (item) {
                    return item.ItemCode === product.ItemCode;
                });
                if (selectedItem) {
                    product.UNIT = selectedItem.ItemUnit || '';
                    product.SPECIFICATION = selectedItem.SPECIFICATION || '';
                    product.ItemDescription = selectedItem.ItemDescription;
                    $scope.$apply();
                } else {
                    product.UNIT = "";
                    product.SPECIFICATION = "";
                    product.ItemDescription = "";
                }
            });
        } else {
            product.UNIT = "";
            product.SPECIFICATION = "";
            product.ItemDescription = "";
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
    /*$scope.addProduct = function () {
        $scope.ItemSelect.dataSource.read().then(function () {
            const data = $scope.ItemSelect.dataSource.data();

            $scope.productFormList.push({
                ID: $scope.counterProduct,
                ItemCode: "",
                ItemDescription: "",
                UNIT: "",
                IMAGE: "",
                QUANTITY: "",
                UNIT_PRICE: "",
                AMOUNT: "",
                REMARKS: ""
            });

            $scope.counterProduct++;
        });
    };*/

    /*$scope.addProduct = function () {
        $http.get("/api/QuotationApi/ItemDetails")
            .then(function (response) {
                // Assuming response.data is an array of objects with 'ItemDescription' and 'ItemCode' properties
                $scope.ItemSelect.dataSource.data = response.data;
                $scope.productFormList.push({
                    ID: $scope.counterProduct,
                    ItemCode: "",
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
                //console.error('Error fetching item details:', error);
                displayPopupNotification("Error fetching item details", "error");
            });
    };*/
    /*$scope.updateUnit = function (product) {
        if (product && product.ItemDescription) {
            $scope.ItemSelect.dataSource.fetch(function () {
                var data = $scope.ItemSelect.dataSource.data();

                var selectedItem = data.find(function (item) {
                    return item.ItemCode === product.ItemDescription;
                });

                if (selectedItem) {
                    product.UNIT = selectedItem.ItemUnit;
                    product.SPECIFICATION = selectedItem.SPECIFICATION;
                } else {
                    product.UNIT = ""; 
                }
            });
        } else {
            product.UNIT = "";
        }
    };*/
    /*$scope.updateUnit = function (product) {
        if (product && product.ItemCode) {
            $scope.ItemSelect.dataSource.fetch(function () {
                var data = $scope.ItemSelect.dataSource.data();

                var selectedItem = data.find(function (item) {
                    return item.ItemCode === product.ItemCode;
                });

                if (selectedItem) {
                    product.UNIT = selectedItem.ItemUnit;
                    product.SPECIFICATION = selectedItem.SPECIFICATION;
                } else {
                    product.UNIT = "";
                }
            });
        } else {
            product.UNIT = "";
        }
    };*/

    $scope.updateAmount = function (product) {
        if (product.UNIT_PRICE && product.QUANTITY) {
            product.AMOUNT = product.UNIT_PRICE * product.QUANTITY;
        } else {
            product.AMOUNT = null;
        }
    };
    $scope.addRow = function () {
        debugger;
        if ($scope.APPROVED_STATUS == 'Approved') { return; }
        var maxId = Math.max(...$scope.productFormList.map(product => product.ID));
        $scope.counterProduct = maxId !== -Infinity ? maxId + 1 : 1;
        $scope.productFormList.push({
            ID: $scope.counterProduct,
            ItemDescription: "",
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
        if ($scope.APPROVED_STATUS == 'Approved') { return; }
        var itemId = $scope.productFormList[index].TID;
        var tenderNo = $scope.TENDER_NO;
        var action = $scope.saveAction;

        if (itemId) {
            if (action == 'Update') {
                $http.post('/api/QuotationApi/updateItemsById?tenderNo=' + tenderNo + '&id=' + itemId + '&q_no=' + $scope.id)
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

    

    $scope.saveData = function () {
        var formData = {
            ID: $scope.ID,
            QUOTATION_NO: $scope.id,
            LOCAL_FLAG: $scope.LOCAL_FLAG,
            TENDER_NO: $scope.TENDER_NO,
            ISSUE_DATE: $('#englishdatedocument').val(),
            VALID_DATE: $('#validDt').val(),
            REMARKS: $('#remarks').val(),
            LOG_REMARKS: '',
            Items: []
        };
        debugger;
        var count = 0;
        var ProductEmpty = false;
        // Loop over the productFormList if it's not empty
        if ($scope.productFormList && $scope.productFormList.length > 0) {
            var totalFiles = $scope.productFormList.length;

            angular.forEach($scope.productFormList, function (itemList) {
                var fileInput = document.getElementById('image_' + itemList.ID);
                var file = fileInput.files[0];
                if (itemList.ItemDescription == "" || typeof itemList.ItemDescription === "undefined" || itemList.ItemDescription === null) {
                    displayPopupNotification("Product Name is required", "warning");
                    ProductEmpty = true;
                    return;// Set flag to true if any rate is empty
                }
                debugger;
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
                    }
                }
            });
        }
    };

    $scope.confirmSave = function () {

        if (!$scope.popupRemarks || $scope.popupRemarks.trim() === "") {
            displayPopupNotification("Remarks are required", "warning");
            return;
        }

        $scope.tempFormData.LOG_REMARKS = $scope.popupRemarks;
        $('#remarksModal').modal('hide');
        saveFormData($scope.tempFormData);
    };

    function saveFormData(formData) {
        debugger;
        $http.post('/api/QuotationApi/SaveItemData', formData)
            .then(function (response) {
                var message = response.data.message;
                $scope.createPanel = false;
                $scope.tablePanel = true;
                displayPopupNotification(message, "success");
                    window.location.href = "/QuotationManagement/Home/Index#!QM/QuotationSetup"
            })
            .catch(function (error) {
                var message = error.data.ExceptionMessage ?? error.data.Message;
                displayPopupNotification(message, "error");
            });
    }

    $scope.Cancel = function () {
        $("#englishdatedocument").data("kendoDatePicker").value(null);
        $("#validDt").data("kendoDatePicker").value(null);
        $("#nepaliDate").val('');
        $("#issueNep").val('');

        // Clear the content of productFormList
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

    // Handle click event for the edit button
    $http.get('/api/QuotationApi/GetQuotationById?tenderNo=' + $scope.id)
        .then(function (response) {
            var quotation = response.data[0];
            $scope.ID = quotation.ID;
            $scope.TENDER_NO = quotation.TENDER_NO;
            $timeout(function () {
                $scope.LOCAL_FLAG = quotation.LOCAL_FLAG;
            });
            $scope.APPROVED_STATUS = quotation.APPROVED_STATUS || 'Pending';
            var issueDate = $filter('date')(new Date(quotation.ISSUE_DATE), 'dd-MMM-yyyy');
            var validDate = $filter('date')(new Date(quotation.VALID_DATE), 'dd-MMM-yyyy');
            
            // Set values for input fields
            $('#englishdatedocument').val(issueDate);
            $('#issueNep').val(quotation.NEPALI_DATE);
            $('#nepaliDate').val(quotation.DELIVERY_DT_BS);
            $("#validDt").val(validDate);

            $scope.TXT_REMARKS = quotation.REMARKS;
            $scope.panelMode = 'edit';
            $scope.saveAction = "Update";
            $scope.createEdit = true;
            $scope.productFormList = [];

            // Populate productFormList
            if (quotation.Items.length === 0) {
                $scope.addProduct();
            } else {
                var id = 1;
                for (var i = 0; i < quotation.Items.length; i++) {
                    var itemList = quotation.Items[i];
                    var imageUrl = null;
                    if (itemList.IMAGE != null) {
                        imageUrl = window.location.protocol + "//" + window.location.host + "/Areas/NeoERP.QuotationManagement/Image/Items/" + itemList.IMAGE;
                    }

                    $scope.productFormList.push({
                        TID: itemList.ID,
                        ID: id,
                        ItemCode: itemList.ITEM_CODE,
                        ItemDescription: itemList.ITEM_CODE, // Adjust if ItemDescription should be different
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
                        SIZE_WIDTH: itemList.SIZE_WIDTH
                    });
                    id++;
                }

               /* // Ensure ItemSelect dataSource is loaded before setting values
                $scope.ItemSelect.dataSource.read().then(function () {
                    $timeout(function () {
                        $scope.productFormList.forEach(function (product) {
                            var dropdown = $("#itemSelected_" + product.ID).data("kendoDropDownList");
                            if (dropdown && product.ItemCode) {
                                dropdown.value(product.ItemCode); // Set the value to ItemCode
                                dropdown.trigger("change"); // Trigger change to update bindings
                            }
                        });
                    }, 500); // Delay to ensure Kendo widgets are initialized
                });*/
            }
        })
        .catch(function (error) {
            var message = 'Error in displaying quotation!!';
            displayPopupNotification(message, "error");
        });
    $scope.openImage = function (imageUrl) {
        if (imageUrl == null) {
            return;
        }
        window.open(imageUrl, '_blank');
    };
    $scope.getItemByCode = function (itemCode, product) {
        var filteredItems = $filter('filter')($scope.ItemSelect.dataSource.data, { ItemCode: itemCode });
        if (filteredItems.length > 0) {
            var selectedItem = filteredItems[0];
            product.ItemCode = selectedItem.ItemCode;
            product.UNIT = selectedItem.ItemUnit;
        } else {
            product.ItemCode = null;
            product.UNIT = null;
        }
    };

});

