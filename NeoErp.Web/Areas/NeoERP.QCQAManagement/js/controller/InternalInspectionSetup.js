QCQAModule.controller('InternalInspectionSetupList', function ($scope, $rootScope, $http, $filter, $timeout) {


    $scope.ParameterItems1 = [];
    $scope.childParameterItemModels = [];
    $scope.AddQCQAInternalInspectionModal = function () {
        $('#QCQAInternalInspectionModal').modal('show');
    };
    MultiSelectProduct();

    function MultiSelectProduct(selectedItemCodes = []) {
        $http.get("/api/InternalInspectionSetupAPI/ItemList")
            .then(function (response) {
                var items = response.data;

                // Destroy existing multiselect if already created
                var selectElement = $("#productList");
                if (selectElement.data("kendoMultiSelect")) {
                    selectElement.data("kendoMultiSelect").destroy();
                    selectElement.empty();
                }

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
    $scope.saveInternalInspectionSetup = function () {
        if ($("#txtProduct").val() == null || $("#txtProduct").val() == "" || $("#txtProduct").val() == undefined) {
            displayPopupNotification("Name is required", "warning");
            return false;
        }
        if ($("#productList").data("kendoMultiSelect").value() == null || $("#productList").data("kendoMultiSelect").value() == "" || $("#productList").data("kendoMultiSelect").value() == undefined) {
            displayPopupNotification("Item is required", "warning");
            return false;
        }
        var rows = document.querySelectorAll("#InternalInspectionSetupGridData");

        for (let rowIndex = 0; rowIndex < rows.length; rowIndex++) {
            let row = rows[rowIndex];
            let parameterValue = row.querySelector(".parameter_" + rowIndex);
            if (!parameterValue || parameterValue.value.trim() === null || parameterValue.value.trim() === "" || parameterValue.value.trim() === undefined) {
                displayPopupNotification("Parameter is required", "warning");
                return false;  
            }
            var rawMaterial = {
                PARAM_CODE: row.querySelector(".paramCode_" + rowIndex)?.value || "",
                PARAMETERS: parameterValue.value || "",
                SPECIFICATION: row.querySelector(".specification_" + rowIndex)?.value || "",
                UNIT: row.querySelector(".unit_" + rowIndex)?.value || "",
                TARGET: row.querySelector(".target_" + rowIndex)?.value || "",
                TOLERENCE: row.querySelector(".tolerence_" + rowIndex)?.value || ""
            };
            rawMateriallList.push(rawMaterial);
        }

        var wrapper = {
            PARAM_CODE: $('.paramCode').val(),
            PRODUCT_NAME: $('#txtProduct').val(),//$scope.selectedProductType,
            ITEMSETUPS: $("#productList").data("kendoMultiSelect").value(),//$('#productList').val(),
            ParameterDetailsList: rawMateriallList  // lowercase 'list'
        };
        $http.post('/api/InternalInspectionSetupAPI/saveInternalInspectionDetails', wrapper)
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
    $scope.add_child_element = function (e) {
        $http.get('/api/ProductSetupAPI/GetParameterList')
            .then(function (response) {
                var qcqa = response.data;
                if (qcqa && qcqa.length > 0) {
                    $scope.ParameterItems1.push({ element: qcqa });
                }
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    // remove child row.
    $scope.remove_child_element = function (index) {
        if ($scope.ParameterItems1.length > 1) {
            $scope.ParameterItems1.splice(index, 1);
        }
    }

    $http.get('/api/ProductSetupAPI/GetParameterList')
        .then(function (response) {
            var qcqa = response.data;
            if (qcqa && qcqa.length > 0) {
                $scope.ParameterItems1.push({ element: qcqa });
            }
        })
        .catch(function (error) {
            displayPopupNotification(error, "error");
        })

    $http.get('/api/InternalInspectionSetupAPI/GetProductDetails')
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
    $("#InternalInspectionSetupGrid").kendoGrid({
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
            { field: "PARAM_CODE", title: "ID", width: 100, type: "string" },
            { field: "PRODUCT_NAME", title: "PRODUCT", width: 200, type: "string" },
            {
                title: "Actions",
                width: 120,
                template: "<a class='btn btn-sm btn-info view-btn' data-id='#= PARAM_CODE #'><i class='fa fa-eye'></i></a>&nbsp;<a class='btn btn-sm btn-warning edit-btn' data-id='#= PARAM_CODE #'><i class='fa fa-edit'></i></a>&nbsp;<a class='btn btn-sm btn-danger delete-btn' data-id='#= PARAM_CODE #'><i class='fa fa-trash'></i></a>"
            }
        ]
    });
    // Handle click event for the delete button
    $("#InternalInspectionSetupGrid").on("click", ".delete-btn", function () {
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
            $http.post('/api/InternalInspectionSetupAPI/DeleteInternalInspectionSetupById?id=' + id)
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
                MultiSelectProduct(selectedItemCodes); //
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

    $("#InternalInspectionSetupGrid").on("click", ".edit-btn", function () {
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
        $http.get('/api/InternalInspectionSetupAPI/GetInternalInspectionSetupById?id=' + id)
            .then(function (response) {
                var product = response.data;
                $('.paramCode').val(product.PARAM_CODE);
                $('#txtProduct').val(product.PRODUCT_NAME);
                var selectedItemCodes = product.ITEM_CODE.split(","); // if saved as comma
                MultiSelectProduct(selectedItemCodes); //
                var parameterDetails = response.data.ParameterDetailsList;
                $scope.ParameterItems1 = [];
                parameterDetails.forEach(function (row, rowIndex) {
                    var parameterDetailsRow = [
                        { COLUMN_NAME: "PARAM_CODE", VALUE: row.PARAM_CODE },
                        { COLUMN_NAME: "PARAMETERS", VALUE: row.PARAMETERS },
                        { COLUMN_NAME: "SPECIFICATION", VALUE: row.SPECIFICATION },
                        { COLUMN_NAME: "UNIT", VALUE: row.UNIT },
                        { COLUMN_NAME: "TARGET", VALUE: row.TARGET },
                        { COLUMN_NAME: "TOLERENCE", VALUE: row.TOLERENCE }
                    ];
                    $scope.ParameterItems1.push({ element: parameterDetailsRow });
                    $scope.childParameterItemModels = $scope.childParameterItemModels || [];
                    $scope.childParameterItemModels[rowIndex] = {};
                    parameterDetailsRow.forEach(function (item) {
                        $scope.childParameterItemModels[rowIndex][item.COLUMN_NAME] = item.VALUE;
                    });
                });
                $('#QCQAInternalInspectionModal').modal('show');
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
