QCQAModule.controller('ProductSetupList', function ($scope, $rootScope, $http, $filter, $timeout) {


    $scope.ParameterItems = [];
    $scope.childParameterItemModels = [];
    $scope.AddQCQAProductSetup = function () {
        $('#QCQAProductModal').modal('show');
    };
    $scope.ddlType = {
        optionLabel: "Select",
        dataTextField: "text",
        dataValueField: "value",
        valuePrimitive: true,
        dataSource: [{ text: "OnSite Inspection", value: "OI" }, { text: "Daily Wastage", value: "DW" }]
    };
    $("#ddlProductType").kendoDropDownList({
        optionLabel: "Select Product Type...",
        dataTextField: "PRODUCT_TYPE",   // <- replace with actual property
        dataValueField: "PRODUCT_TYPE",    // <- replace with actual property
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/ProductSetupAPI/ProductTypeList",
                    dataType: "json"
                }
            }
        },
        change: function (e) {
            var selectedVal = this.value();//$(this).val(); //$(this).val();//this.value();
            MultiSelectProduct(selectedVal);
        },
        dataBound: function (e) {
            // Called after DropDownList is loaded
            var selectedVal = this.value(); // get current value on edit
            if (selectedVal) {
                MultiSelectProduct(selectedVal); // call for initial load
            }
        }
    }).data("kendoDropDownList");


    function MultiSelectProduct(productType, selectedItemCodes = []) {
        var selectElement = $("#productList");

        if (selectElement.data("kendoMultiSelect")) {
            selectElement.data("kendoMultiSelect").destroy();
            selectElement.closest(".k-widget").find('.k-multiselect-wrap').remove();
        }
        selectElement.empty();

        $http.get("/api/ProductSetupAPI/ProductList?ProductType=" + productType + "&Category_Code=" + "")
            .then(function (response) {
                var items = response.data;

                // Destroy existing multiselect if already created
                var selectElement = $("#productList");
                if (selectElement.data("kendoMultiSelect")) {
                    selectElement.data("kendoMultiSelect").destroy();
                    selectElement.empty();
                }

                // Initialize MultiSelect with proper datasource
                var multi = selectElement.kendoMultiSelect({
                    dataSource: items,
                    dataTextField: "ITEM_EDESC",
                    dataValueField: "ITEM_CODE",
                    autoClose: false
                }).data("kendoMultiSelect");
                if (selectedItemCodes) {
                    if (typeof selectedItemCodes === "string") {
                        selectedItemCodes = selectedItemCodes.split(",");
                    }
                    selectedItemCodes = selectedItemCodes.map(function (val) {
                        return val.trim();
                    });
                    multi.value(selectedItemCodes);
                }
            });
    }

    var rawMateriallList = [];
    $scope.saveProductSetup = function () {
        if ($("#ddlProductType").val() == null || $("#ddlProductType").val() == "" || $("#ddlProductType").val() == undefined) {
            displayPopupNotification("Product Type is required", "warning");
            return false;
        }
        if ($("#ddlType").val() == null || $("#ddlType").val() == "" || $("#ddlType").val() == undefined) {
            displayPopupNotification("Form Type is required", "warning");
            return false;
        }
        if ($("#productList").data("kendoMultiSelect").value() == null || $("#productList").data("kendoMultiSelect").value() == "" || $("#productList").data("kendoMultiSelect").value() == undefined) {
            displayPopupNotification("Product is required", "warning");
            return false;
        }
        var rows = document.querySelectorAll("#ItemGrid");
        rows.forEach(function (row, rowIndex) {
            if (row.querySelector(".parameter_" + rowIndex).value == null || row.querySelector(".parameter_" + rowIndex).value == "" || row.querySelector(".parameter_" + rowIndex).value == undefined) {
                displayPopupNotification("Parameter is required", "warning");
                return false;
            }
            var rawMaterial = {
                PARAM_CODE: row.querySelector(".paramCode_" + rowIndex)?.value || "",
                PARAMETERS: row.querySelector(".parameter_" + rowIndex)?.value || "",
                SPECIFICATION: row.querySelector(".specification_" + rowIndex)?.value || "",
                TARGET: row.querySelector(".target_" + rowIndex)?.value || "",
                TOLERENCE: row.querySelector(".tolerence_" + rowIndex)?.value || ""
            };
            rawMateriallList.push(rawMaterial);
        });
        var wrapper = {
            PRODUCT_TYPE: $('#ddlProductType').val(),//$scope.selectedProductType,
            TYPE: $('#ddlType').val(),
            ITEMSETUPS: $("#productList").data("kendoMultiSelect").value(),//$('#productList').val(),
            ParameterDetailsList: rawMateriallList  // lowercase 'list'
        };
        $http.post('/api/ProductSetupAPI/saveProductDetails', wrapper)
            .then(function (response) {
                var message = response.data.message; // Extract message from response
                displayPopupNotification(message, "success");
                setTimeout(function () {
                    window.location.reload();
                }, 5000)
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    $http.get('/api/ProductSetupAPI/GetParameterList')
        .then(function (response) {
            var qcqa = response.data;
            if (qcqa && qcqa.length > 0) {
                $scope.ParameterItems.push({ element: qcqa });
            }
        })
        .catch(function (error) {
            displayPopupNotification(error, "error");
        })
    $scope.add_child_element = function (e) {
        $http.get('/api/ProductSetupAPI/GetParameterList')
            .then(function (response) {
                var qcqa = response.data;
                if (qcqa && qcqa.length > 0) {
                    $scope.ParameterItems.push({ element: qcqa });
                }
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    // remove child row.
    $scope.remove_child_element = function (index) {
        if ($scope.ParameterItems.length > 1) {
            $scope.ParameterItems.splice(index, 1);
        }
    }


    $http.get('/api/ProductSetupAPI/GetItemDetails')
        .then(function (response) {
            var qcqa = response.data;
            if (qcqa && qcqa.length > 0) {
                $scope.dataSource.data(qcqa);
            }
        })
        .catch(function (error) {
            displayPopupNotification(error, "error");
        })
    $scope.dataSource = new kendo.data.DataSource({
        data: [], // Initially empty
    });
    $("#ProductSetupGrid").kendoGrid({
        dataSource: $scope.dataSource,
        height: 400,
        sortable: true, // Enable sorting
        pageable: {
            refresh: true,
            pageSizes: true
        },
        resizable: true, // Enable column resizing
        columns: [
            //{ field: "FORM_CODE", title: "S.N", width: 90, type: "string" },
            { field: "PARAM_CODE", title: "Code", width: 100, type: "string" },
            { field: "PRODUCT_TYPE", title: "Product Type", width: 200, type: "string" },
            { field: "TYPE", title: "Type", width: 200, type: "string" },
            { field: "ITEM_EDESC", title: "Product", width: 100, type: "string" },

            //{
            //    field: "CUSTOM_SUFFIX_TEXT", title: "Suffix Text", width: 200, type: "string",
            //},
            //{ field: "SUFFIX_LENGTH", title: "Suffix Length", width: 100, type: "string" },
            //{
            //    field: "BODY_LENGTH", title: "Body Length", width: 150, type: "string",
            //},
            //{
            //    field: "CREATED_DATE", title: "Created Date", width: 150, type: "string",
            //    template: "#=kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy') #",
            //},
            {
                title: "Actions",
                width: 120,
                template: "<a class='btn btn-sm btn-info view-btn' data-id='#= PARAM_CODE #'><i class='fa fa-eye'></i></a>&nbsp;<a class='btn btn-sm btn-warning edit-btn' data-id='#= PARAM_CODE #'><i class='fa fa-edit'></i></a>&nbsp;<a class='btn btn-sm btn-danger delete-btn' data-id='#= PARAM_CODE #'><i class='fa fa-trash'></i></a>"
            }
            //{
            //    title: "Actions",
            //    width: 120,
            //    template: "<a class='btn btn-sm btn-info view-btn' data-id='#= FORM_CODE #'><i class='fa fa-eye'></i></a>&nbsp;<a class='btn btn-sm btn-warning edit-btn' data-id='#= FORM_CODE #'><i class='fa fa-edit'></i></a>&nbsp;<a class='btn btn-sm btn-danger delete-btn' data-id='#= FORM_CODE #'><i class='fa fa-trash'></i></a>"
            //}
        ]
    });
    // Handle click event for the delete button
    $("#ProductSetupGrid").on("click", ".delete-btn", function () {
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
            $http.post('/api/ProductSetupAPI/DeleteProductSetupById?id=' + id)
                .then(function (response) {
                    $scope.Outlets = response.data;
                    setTimeout(function () {
                        window.location.reload();
                    }, 5000)
                }).catch(function (error) {
                    var message = 'Error in displaying no!!'; // Extract message from response
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

    //view button
    $("#ProductSetupGrid").on("click", ".view-btn", function () {
        var editBtn = $(this);
        var id = $(this).data("id");
        $http.get('/api/ProductSetupAPI/GetProductById?id=' + id)
            .then(function (response) {
                var product = response.data;
                var comboBox = $("#ddlProductType").data("kendoDropDownList");

                var selectedProductType = product.PRODUCT_TYPE;
                var selectedItemCodes = product.ITEM_CODE.split(","); // if saved as comma

                comboBox.value(selectedProductType); // select the type
                MultiSelectProduct(selectedProductType, selectedItemCodes); //
                var parameterDetails = response.data.ParameterDetailsList;
                $scope.ParameterItems = [];
                parameterDetails.forEach(function (row, rowIndex) {
                    var parameterDetailsRow = [
                        { COLUMN_NAME: "PARAM_CODE", VALUE: row.PARAM_CODE },
                        { COLUMN_NAME: "PARAMETERS", VALUE: row.PARAMETERS },
                        { COLUMN_NAME: "SPECIFICATION", VALUE: row.SPECIFICATION },
                        { COLUMN_NAME: "TARGET", VALUE: row.TARGET },
                        { COLUMN_NAME: "TOLERENCE", VALUE: row.TOLERENCE }
                    ];
                    $scope.ParameterItems.push({ element: parameterDetailsRow });
                    $scope.childParameterItemModels = $scope.childParameterItemModels || [];
                    $scope.childParameterItemModels[rowIndex] = {};
                    parameterDetailsRow.forEach(function (item) {
                        $scope.childParameterItemModels[rowIndex][item.COLUMN_NAME] = item.VALUE;
                    });
                });
                $('#QCQAProductModal').modal('show');
            }).catch(function (error) {
                var message = 'Error in displaying no!!';
                displayPopupNotification(message, "error");
            })
    })

    $("#ProductSetupGrid").on("click", ".edit-btn", function () {
        var editBtn = $(this);
        var id = $(this).data("id");
        setTimeout(function () {
            $('.dynamicform .k-widget.k-multiselect.k-header').each(function () {
                var $directFloatwrap = $(this)
                    .children('.k-multiselect-wrap.k-floatwrap');
                var innercheck = false;
                $(this)
                    .children('.k-widget.k-multiselect.k-header').each(function () {
                        var $innerFloatwrap = $(this)
                            .children('.k-multiselect-wrap.k-floatwrap');
                        if ($innerFloatwrap.length == 1) {
                            innercheck = true;
                        }
                    });
                if (innercheck == true) { $directFloatwrap.remove(); }
            });
        }, 300);
        $http.get('/api/ProductSetupAPI/GetProductById?id=' + id)
            .then(function (response) {
                var product = response.data;
                var comboBox = $("#ddlProductType").data("kendoDropDownList");
                var comboBox_Type = $("#ddlType").data("kendoDropDownList");
                var selectedProductType = product.PRODUCT_TYPE;
                var selectedItemCodes = product.ITEM_CODE.split(","); // if saved as comma
                comboBox.value(selectedProductType); // select the type
                MultiSelectProduct(selectedProductType, selectedItemCodes); //
                var selectedType = product.TYPE;
                comboBox_Type.value(selectedType); // select the type
                var parameterDetails = response.data.ParameterDetailsList;
                $scope.ParameterItems = [];
                parameterDetails.forEach(function (row, rowIndex) {
                    var parameterDetailsRow = [
                        { COLUMN_NAME: "PARAM_CODE", VALUE: row.PARAM_CODE },
                        { COLUMN_NAME: "PARAMETERS", VALUE: row.PARAMETERS },
                        { COLUMN_NAME: "SPECIFICATION", VALUE: row.SPECIFICATION },
                        { COLUMN_NAME: "TARGET", VALUE: row.TARGET },
                        { COLUMN_NAME: "TOLERENCE", VALUE: row.TOLERENCE }
                    ];
                    $scope.ParameterItems.push({ element: parameterDetailsRow });
                    $scope.childParameterItemModels = $scope.childParameterItemModels || [];
                    $scope.childParameterItemModels[rowIndex] = {};
                    parameterDetailsRow.forEach(function (item) {
                        $scope.childParameterItemModels[rowIndex][item.COLUMN_NAME] = item.VALUE;
                    });
                });
                $('#QCQAProductModal').modal('show');
            }).catch(function (error) {
                var message = 'Error in displaying no!!';
                displayPopupNotification(message, "error");
            })
    })

    $("#itemtxtSearchString").keyup(function () {
        var val = $(this).val().toLowerCase(); // Get the search input value

        if (!val) {
            // If input is empty, clear filters
            $scope.dataSource.filter({});
            return;
        }

        var filters = [];
        var grid = $("#NumberSetupGrid").data("kendoGrid");

        if (!grid) return; // If grid not found, exit

        var columns = grid.columns;

        for (var i = 0; i < columns.length; i++) {
            var column = columns[i];
            var field = column.field;

            if (!field) continue; // Skip if no field name

            filters.push({
                field: field,
                operator: function (itemValue) {
                    itemValue = (itemValue || "").toString().toLowerCase();
                    return itemValue.indexOf(val) >= 0;
                }
            });
        }

        grid.dataSource.filter({
            logic: "or",
            filters: filters
        });
    });
    $scope.ItemCancel = function () {
        window.location.reload();
    }

    $scope.BrowseTreeListForItems = function (index) {
        $('#productModal_' + index).modal('show');
        $scope.childRowIndex = index;
        document.popupindex = index;
        if ($('#producttree_' + $scope.childRowIndex).data("kendoTreeView") != undefined)
            $('#producttree_' + $scope.childRowIndex).data("kendoTreeView").expand('.k-item');
    }
});
