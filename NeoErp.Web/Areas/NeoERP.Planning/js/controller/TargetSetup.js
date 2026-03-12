planningModule.controller('TargetSetup', function ($scope, $http, $timeout, $compile, $route) {

    var date = new Date();
    var d = date.getDate();
    var m = date.getMonth();
    var y = date.getFullYear();
    $scope.bsFromDate = '';
    $scope.bsToDate = '';

    $scope.startdate = "";
    $scope.enddate = "";
    $scope.endDateToEdit = Date.now();
    $scope.holidayList = [];
    $scope.GroupOptions = {};
    $scope.selectedEmpGroup = {};
    $scope.IsEdit = false;
    $scope.targetType = null;
    $scope.Type = null;
    $scope.add_edit_option = "Edit";
    $scope.itemGroup = [];
    $scope.shouldShowSubTargetType = false;
    $scope.showIndividualType = false;
    $scope.showCustomerType = false;
    $scope.selectedSubTarget = null;
    $scope.selectedCustomerss = [];
    $scope.selectedEmployeeObjects = []; 
    $scope.targetTypes = [
        { text: "Sales", value: "SAL" },
        { text: "Collection", value: "COL" }
    ];
    $scope.Types = [
        { text: "Synergy", value: "SNG" },
        { text: "DNM", value: "DNM" }
    ];
    $scope.subTargetTypeOptions = {
        dataTextField: "text",
        dataValueField: "value",
        dataSource: [{ text: "Sales Target", value: "SALE" }, { text: "Customer", value: "CUS" }, { text: "Product", value: "PRO" }, {text: "Area", value: "AREA"}],
        optionLabel: "Select Sub Target"
    };
    $scope.onSubTargetChange = function () {

        if (!$scope.selectedSubTarget) return;

        switch ($scope.selectedSubTarget.value) {

            case "SALE":
                $scope.showIndividualType = false;
                $scope.showCustomerType = false;
                $scope.showIndividualType = false;
                $scope.showAreaType = false;
                $scope.showEmpType = true;
                $scope.subTargetChange();
                break;

            case "CUS":
                $scope.showIndividualType = false;
                $scope.showCustomerType = true;
                $scope.showIndividualType = false;
                $scope.showAreaType = false;
                $scope.showEmpType = true;
                $scope.subTargetChange();
                break;

            case "PRO": 
                $scope.showIndividualType = false;
                $scope.showCustomerType = false;
                $scope.showIndividualType = true;
                $scope.showAreaType = false;
                $scope.showEmpType = true; 
                $scope.subTargetChange();
                break;

            case "AREA": 
                $scope.showIndividualType = false;
                $scope.showCustomerType = false;
                $scope.showIndividualType = false;
                $scope.showAreaType = true;
                $scope.showEmpType = true; 
                $scope.subTargetChange();
                break;
        }
    };
    $scope.onTypeChange = function (selectedType) {
        if (selectedType && selectedType.value) {
            $scope.Type = selectedType.value;
            
            $scope.customerGroupOptions.dataSource.read();
            $scope.employeeGroupOptions.dataSource.read();
        }
    };
    //$scope.employeeGroupOptions = {
    //    optionLabel: "-- Select Group --",
    //    filter: "contains",
    //    dataTextField: "GROUP_EDESC",
    //    dataValueField: "GROUP_ID",
    //    autoBind: false,

    //    dataSource: new kendo.data.DataSource({
    //        type: "json",
    //        serverFiltering: true,
    //        transport: {
    //            read: {
    //                url: function () {
    //                    // Dynamically generate URL based on $scope.Type
    //                    return $scope.Type === "SNG"
    //                        ? "/api/DistributionPlaningApi/GetGroupEmployees"
    //                        : "/api/DistributionPlaningApi/GetDistributionGroups";
    //                },
    //                type: "GET",
    //                dataType: "json"
    //            },
    //            parameterMap: function (data, type) {
    //                var filterValue = (data.filter && data.filter.filters && data.filter.filters.length > 0)
    //                    ? data.filter.filters[0].value
    //                    : "";
    //                return { filter: filterValue };
    //                debugger;
    //            }
    //        }
    //    })
    //};

    $scope.employeeGroupOptions = {
        optionLabel: "-- Select Group --",
        filter: "contains",
        dataTextField: "GROUP_EDESC",
        dataValueField: "GROUPID",
        autoBind: false,          // don't fetch data immediately
        valuePrimitive: true,    
        dataSource: new kendo.data.DataSource({
            type: "json",
            serverFiltering: true,
            transport: {
                read: {
                    url: function () {
                        return $scope.Type === "SNG"
                            ? "/api/DistributionPlaningApi/GetGroupEmployees"
                            : "/api/DistributionPlaningApi/GetDistributionGroups";
                    },
                    type: "GET",
                    dataType: "json"
                },
                parameterMap: function (data, type) {
                    var filterValue = (data.filter && data.filter.filters && data.filter.filters.length > 0)
                        ? data.filter.filters[0].value
                        : "";
                    return { filter: filterValue };
                }
            }
        }),

        // Header with Select All button
        headerTemplate: `
        <div class="k-header d-flex align-items-center" style="justify-content:space-between;">
            <strong>Groups</strong>
            <button type="button" id="groupSelectAll"
                    style="font-size:9px; cursor:pointer; border:none; background:none; padding:0;">
                Select All
            </button>
        </div>
    `,

        dataBound: function (e) {
            const btn = $("#groupSelectAll");
            btn.off("click");
            btn.on("click", function (ev) {
                ev.preventDefault();
                selectAllGroups();
            });

            // Apply the selected values after DataSource is ready
            if ($scope.selectedEmpGroup && $scope.selectedEmpGroup.length) {
                const ms = e.sender;
                ms.value($scope.selectedEmpGroup);
            }
        }
    };

    function selectAllGroups() {
        const ms = $("#empGroup").data("kendoMultiSelect");
        if (!ms) return;

        $scope.updatingGroups = true;

        ms.dataSource.fetch().then(function () {
            const allValues = ms.dataSource.data().map(item => item.GROUPID);
            const selectedValues = ms.value() || [];

            const isAllSelected =
                allValues.length > 0 &&
                selectedValues.length === allValues.length;

            if (isAllSelected) {
                ms.value([]);
                $scope.selectedEmpGroup = [];
            } else {
                ms.value(allValues);
                $scope.selectedEmpGroup = allValues;
            }

            if (!$scope.$$phase) {
                $scope.$apply();
            }

            $scope.updatingGroups = false;
        });
    }



    $scope.itemGroupOptions = {
        optionLabel: "-- Select Group --",
        filter: "contains",
        dataTextField: "ITEM_EDESC",
        dataValueField: "ITEM_CODE",
        autoBind: false,
        dataSource: new kendo.data.DataSource({  // Ensure this is a Kendo UI DataSource
            type: "json",
            serverFiltering: true,
            transport: {
                read: {
                    url: "/api/DistributionPlaningApi/GetItemGroup",
                    type: "GET",
                    dataType: "json"
                },
                parameterMap: function (data, type) {
                    var filterValue = (data.filter && data.filter.filters && data.filter.filters.length > 0) ? data.filter.filters[0].value : "";
                    return {
                        filter: filterValue,  // Pass the filter value
                    };
                }
            }
        }),
    };
    $scope.customerGroupOptions = {
        optionLabel: "-- Select Group --",
        filter: "contains",
        dataTextField: "GROUP_EDESC",
        dataValueField: "GROUP_ID",
        autoBind: false,
        dataSource: new kendo.data.DataSource({  // Ensure this is a Kendo UI DataSource
            type: "json",
            serverFiltering: true,
            transport: {
                //    read: {
                //        url: "/api/DistributionPlaningApi/GetCustomerGroup",
                //        type: "GET",
                //        dataType: "json"
                //    },
                read: {
                    url: function () {
                        // Dynamically generate the URL based on $scope.Type
                        return $scope.Type === "SNG"
                            ? "/api/DistributionPlaningApi/GetCustomerSNGGroup"
                            : "/api/DistributionPlaningApi/GetCustomerGroup";
                    },
                    type: "GET",
                    dataType: "json"
                },
                parameterMap: function (data, type) {
                    var filterValue = (data.filter && data.filter.filters && data.filter.filters.length > 0) ? data.filter.filters[0].value : "";
                    return {
                        filter: filterValue,  // Pass the filter value
                    };
                }
            }
        }),
    };
    // Ensure the function is called when selectedSubTarget changes
    $scope.$watch('selectedSubTarget', function (newVal, oldVal) {
        if (newVal !== oldVal) {
            $scope.onSubTargetChange();
        }
    });
    $scope.onTargetTypeChange = function (type) {
        if (type && type.value === 'SAL') {
            $scope.shouldShowSubTargetType = true;
            $scope.targetType = type.value;
            $scope.cancelClick();
        } else {
            $scope.showEmpType = true;
            $scope.showCustomerType = false;
            $scope.shouldShowSubTargetType = false;
            $scope.showIndividualType = false;
            $scope.targetType = type.value;
            $scope.cancelClick();
        }
    };
    $scope.getMasterEmployeeCodes = function () {
        if ($scope.selectedEmpGroup && $scope.selectedEmpGroup.length > 0) {
            if ($scope.Type == "SNG") {
                return $scope.selectedEmpGroup.map(function (emp) {
                    return "'" + emp.MASTER_CUSTOMER_CODE + "'";
                }).join(', ');
            } else {
                return $scope.selectedEmpGroup.map(function (emp) {
                    return "'" + emp.GROUP_ID + "'";
                }).join(', ');
            }
        }
        return '';
    };
    $scope.getItemCodes = function () {
        if ($scope.selectedItmGroup && $scope.selectedItmGroup.length > 0) {
            return $scope.selectedItmGroup.map(function (emp) {
                return "'" + emp.MASTER_ITEM_CODE + "'";
            }).join(', ');
        }
        return '';
    };
    $scope.getCustomerCodes = function () {
        if ($scope.selectedCusGroup && $scope.selectedCusGroup.length > 0) {
            if ($scope.Type == "SNG") {
                return $scope.selectedCusGroup.map(function (cus) {
                    return "'" + cus.MASTER_CUSTOMER_CODE + "'";
                }).join(', ');
            } else {
                return $scope.selectedCusGroup.map(function (cus) {
                    return "'" + cus.GROUP_ID + "'";
                }).join(', ');
            }
        }
        return '';
    };

    $scope.employeeOptions = {
        optionLabel: " ",
        filter: "contains",
        autoBind: false,
        valuePrimitive: false,
        autoClose: false,
        placeholder: "",

        dataTextField: $scope.Type === "SNG" ? "EMPLOYEE_EDESC" : "FULL_NAME",
        dataValueField: $scope.Type === "SNG" ? "EMPLOYEE_CODE": "USERID" ,

        // DataSource with dynamic API URL
        dataSource: new kendo.data.DataSource({
            serverFiltering: false,
            transport: {
                read: {
                    url: function () {
                        return $scope.Type === "SNG"
                            ? "/api/DistributionPlaningApi/getSNGEmployees"
                            : "/api/DistributionPlaningApi/GetDistributionEmployee";
                    },
                    type: "GET",
                    dataType: "json"
                },
                parameterMap: function () {
                    return ($scope.selectedEmpGroup && $scope.selectedEmpGroup.length)
                        ? { filter: $scope.selectedEmpGroup.join(',') }
                        : {};
                }
            }
        }),

        // HeaderTemplate with Select All button
        headerTemplate: `
        <div class="k-header d-flex align-items-center" style="justify-content:space-between;">
            <strong>Employees</strong>
            <button type="button" id="employeeSelectAll"
                    style="font-size:9px; cursor:pointer; border:none; background:none; padding:0;">
                Select All
            </button>
        </div>
    `,

       
        dataBound: function (e) {
            const btn = $("#employeeSelectAll");

            btn.off("click");
            btn.on("click", function (ev) {
                ev.preventDefault();
                selectAllEmployees();
            });
        }
    };

    function selectAllEmployees() {
        const multiSelect = $("#employees").data("kendoMultiSelect");
        if (!multiSelect) return;

        $scope.updatingEmployees = true;

        // Prevent recursive databound
        multiSelect.unbind("dataBound");

        multiSelect.dataSource.fetch().then(function () {

            const valueField = $scope.Type === "SNG"
                ? "EMPLOYEE_CODE"
                : "USERID";

            const allValues = multiSelect.dataSource.data().map(item => item[valueField]);
            const selectedValues = multiSelect.value() || [];

            const isAllSelected =
                allValues.length > 0 &&
                selectedValues.length === allValues.length;

            if (isAllSelected) {
                // 🔁 UNSELECT ALL
                multiSelect.value([]);
                $scope.selectedEmployees = [];
            } else {
                // ✅ SELECT ALL
                multiSelect.value(allValues);
                $scope.selectedEmployees = allValues;
            }

            if (!$scope.$$phase) {
                $scope.$apply();
            }

            // Restore databound handler
            multiSelect.bind("dataBound", $scope.employeeOptions.dataBound);

            $scope.updatingEmployees = false;
        });
    }




    //$scope.reloadEmployees = function () {
    //    const empMultiSelect = $("#employeeEmployees").data("kendoMultiSelect");
    //    if (empMultiSelect) {
    //        $scope.selectedEmployees = [];
    //        empMultiSelect.dataSource.read();
    //    }
    //};

    angular.element(document).ready(function () {
        if ($scope.selectedEmpGroup && $scope.selectedEmpGroup.length > 0) {
            $scope.reloadEmployees();
        }
    });

    $scope.individualOptions = {
        optionLabel: "-- Select Item --",
        filter: "contains",
        dataTextField: "ITEM_EDESC",
        dataValueField: "ITEM_CODE",
        autoBind: false,
        dataSource: new kendo.data.DataSource({  // Ensure this is a Kendo UI DataSource
            type: "json",
            serverFiltering: true,
            transport: {
                read: {
                    url: "/api/DistributionPlaningApi/GetItemLists",
                    type: "GET",
                    dataType: "json"
                },
                parameterMap: function (data, type) {
                    var itmGroupCodes = $scope.getItemCodes();
                    var filterValue = (data.filter && data.filter.filters && data.filter.filters.length > 0) ? data.filter.filters[0].value : "";
                    return {
                        filter: filterValue,
                        itmGroup: itmGroupCodes
                    };
                }
            }
        }),
    };
    //$scope.customerOptions = {
    //    optionLabel: " ",
    //    filter: "contains",
    //    dataTextField: "CUSTOMER_EDESC",
    //    dataValueField: "CUSTOMER_CODE",
    //    autoBind: false,
    //    dataSource: new kendo.data.DataSource({
    //        type: "json",
    //        serverFiltering: true,
    //        transport: {
    //            read: function (options) {
    //                if ($scope.Type === "SNG") {
    //                    $.get("/api/DistributionPlaningApi/GetCustomerSNGLists", function (result) {
    //                        options.success(result);
    //                    });
    //                } else {
    //                    var postData = {
    //                        CustomerGroupIds: ($scope.selectedCusGroup || []).map(g => g.GROUP_ID).join(","),
    //                        EmployeeIds: $scope.selectedEmployees ? $scope.selectedEmployees.map(e => e.USERID).join(",") : "",
    //                        SelectAll: false
    //                    };

    //                    $.ajax({
    //                        url: "/api/DistributionPlaningApi/GetDistributionCustomerPaged",
    //                        type: "POST",
    //                        contentType: "application/json",
    //                        data: JSON.stringify(postData),
    //                        dataType: "json",
    //                        success: function (result) {
    //                            options.success(result);
    //                        },
    //                        error: function (err) {
    //                            console.error("Customer fetch failed:", err);
    //                            options.error(err);
    //                        }
    //                    });
    //                }
    //            }
    //        }
    //    })
    //};

    // Initialize variables
    $scope.selectedCustomer = [];
    $scope.selectedCustomerText = "";
    $scope.customerPopupSearch = "";
    $scope.customerSelectAll = false;

    $scope.selectedEmployees = [];
    $scope.selectedEmployeesText = "";

    // ------------------------- CUSTOMER POPUP -------------------------

    // Open customer popup
    $scope.openCustomerPopup = function () {
        $scope.customerPopup.center().open();
        $scope.reloadCustomerPopup();
    };

    // Reload customer grid
    $scope.reloadCustomerPopup = function () {
        var grid = $("#customerPopupGrid").data("kendoGrid");
        if (grid) grid.dataSource.read();
    };

    // Select all customers (current page)
    $scope.selectAllCustomers = function () {
        var grid = $("#customerPopupGrid").data("kendoGrid");
        grid.tbody.find("tr").each(function () {
            grid.select(this);
        });

        $scope.selectedCustomer = grid.dataSource.view().slice();
        $scope.customerSelectAll = true;
    };
   
    // Unselect all customers
    $scope.unselectAllCustomers = function () {
        var grid = $("#customerPopupGrid").data("kendoGrid");
        grid.clearSelection();
        $scope.selectedCustomer = [];
        $scope.selectedCustomerText = "";
        
        $scope.customerSelectAll = false;
    };

    $scope.applyCustomerSelection = function () {
        var grid = $("#customerPopupGrid").data("kendoGrid");
        var selectedRows = grid.select().toArray();

        // Get selected customer objects
        const selectedCustomers = selectedRows.map(row => grid.dataItem(row));

        // Use $timeout to update Angular scope safely
        $timeout(function () {
            $scope.selectedCustomer = selectedCustomers;

            // Update textarea content
            $scope.selectedCustomerText = $scope.selectedCustomer
                .map(c => `${c.CUSTOMER_CODE} - ${c.CUSTOMER_EDESC}`)
                .join("\n");
            $scope.selectedCustomerss = $scope.selectedCustomer
            console.log("$scope.selectedCustomerss", $scope.selectedCustomerss);
        });

        $scope.customerPopup.close();
    };



    // Customer Grid Options
    $scope.customerPopupGridOptions = {
        selectable: "multiple, row",
        pageable: { refresh: true, pageSizes: ["50", "100", "500", "1000", "All"], buttonCount: 5 },
        filterable: true,
        columns: [
            { field: "CUSTOMER_CODE", title: "Code", width: 100 },
            { field: "CUSTOMER_EDESC", title: "Customer Name" }
        ],
        dataSource: new kendo.data.DataSource({
            type: "json",
            serverPaging: true,
            serverFiltering: true,
            pageSize: 50,
            transport: {
                read: function (options) {
                    if ($scope.Type === "SNG") {
                        $.get("/api/DistributionPlaningApi/GetCustomerSNGLists", function (result) {
                            options.success({ data: result, total: result.length });
                        }).fail(function (err) {
                            console.error("SNG Customer fetch failed:", err);
                            options.error(err);
                        });
                    } else {
                        var postData = {
                            CustomerGroupIds: ($scope.selectedCusGroup || []).map(g => g.GROUP_ID).join(","),
                            EmployeeIds: $scope.selectedEmployees ? $scope.selectedEmployees.map(e => e.USERID).join(",") : "",
                            SelectAll: $scope.customerSelectAll,
                            SearchText: $scope.customerPopupSearch || "",
                            Skip: options.data.skip || 0,
                            Take: options.data.take || 50
                        };

                        $.ajax({
                            url: "/api/DistributionPlaningApi/GetDistributionCustomerPaged",
                            type: "POST",
                            contentType: "application/json",
                            data: JSON.stringify(postData),
                            success: function (result) {
                                options.success({
                                    data: result.items || result,
                                    total: result.totalCount || (result.items ? result.items.length : result.length)
                                });
                            },
                            error: function (err) {
                                console.error("DNM Customer fetch failed:", err);
                                options.error(err);
                            }
                        });
                    }
                }
            },
            schema: { data: "data", total: "total" }
        }),
        dataBound: function (e) {
            var grid = e.sender;

            // Re-select previously selected customers
            grid.tbody.find("tr").each(function () {
                var dataItem = grid.dataItem(this);
                if ($scope.selectedCustomer.some(c => c.CUSTOMER_CODE === dataItem.CUSTOMER_CODE)) {
                    grid.select(this);
                }
            });

            // Select all if checkbox is true
            if ($scope.customerSelectAll) {
                grid.tbody.find("tr").each(function () {
                    grid.select(this);
                });
            }
        },
        change: function () {
            var grid = this;
            $scope.selectedCustomer = grid.select().map(function (idx, row) {
                return grid.dataItem(row);
            }).get();
        }
    };

    // ------------------------- EMPLOYEE MULTISELECT -------------------------

    // Reload employees
    $scope.reloadEmployees = function () {
        const empMultiSelect = $("#employees").data("kendoMultiSelect");
        if (!empMultiSelect) return;

        // Clear previous selection
        $scope.selectedEmployees = [];

        // Refresh the dataSource
        empMultiSelect.dataSource.read().then(function () {
            const selectedData = empMultiSelect.dataItems(); // full objects
            $scope.selectedEmployees = selectedData;

            if (!$scope.$$phase) $scope.$apply();
        });
    };

    // Watch for selectedEmployees changes to update textarea
    $scope.$watch('selectedEmployees', function (newVal) {
        if (newVal && newVal.length > 0) {
            $scope.selectedEmployeesText = newVal
                .map(e => `${e.USERID} - ${e.EMPLOYEE_EDESC || e.USER_NAME || ''}`)
                .join("\n");
        } else {
            $scope.selectedEmployeesText = "";
        }
    }, true);

 


    // Watch selectedEmpGroup to trigger reload
    $scope.$watch('selectedEmpGroup', function (newVal) {
        if (!$scope.selectedEmpGroup || !$scope.selectedEmpGroup.length) return;

        $scope.reloadEmployees();
    }, true);

    //$scope.$watch('selectedEmpGroup', function (newVal) {
    //    if (!$scope.selectedEmpGroup || !$scope.selectedEmpGroup.length) return;

    //    // Trigger employee reload whenever groups change
    //    $scope.reloadEmployees(); // assumes you have this function from previous step
    //}, true);

    //$scope.reloadEmployeesByGroups = function (groupIds) {
    //    const ms = $("#employees").data("kendoMultiSelect");
    //    if (!ms) return;

    //    // Update the read data function to pass selected GroupIDs
    //    ms.dataSource.transport.options.read.data = function () {
    //        return { groupIds: (groupIds || []).join(',') }; // Pass as comma-separated string
    //    };

    //    ms.dataSource.read(); // Reload employees
    //};

    $scope.$watch('selectedCusGroup', function (newVal, oldVal) {
        // Refresh the employee MultiSelect to fetch new data based on the updated group selection
        var customerMultiSelect = $("#customerList").data("kendoMultiSelect");
        if (customerMultiSelect) {
            customerMultiSelect.dataSource.read();
        }
    }, true);
    $scope.remove = function (index) {
        $scope.events.splice(index, 1);
    };
    $scope.calculateTreeTotals = function () {
        if (!$scope.employeeTreeListOptions || !$scope.employeeTreeListOptions.dataSource) return;

        const allNodes = $scope.employeeTreeListOptions.dataSource.data();

        if (!allNodes || allNodes.length === 0) return;

        // Recursive function to calculate totals
        function calculateTotals(node) {
            let totalQty = 0;
            let totalAmt = 0;

            if (node.Type === 'Employee') {
                // Children: sum dynamic fields
                for (let i = 1; i <= $scope.totalDynamicFields; i++) {
                    totalQty += parseFloat(node[`quantity_${i}`]) || 0;
                    totalAmt += parseFloat(node[`amount_${i}`]) || 0;
                }
                node.Quantity = Number(totalQty.toFixed(2));
                node.Amount = Number(totalAmt.toFixed(2));
            } else if (node.Type === 'Group') {
                // Parent: sum all immediate children totals
                const children = allNodes.filter(c => c.ParentId === node.Id);
                children.forEach(child => {
                    const childTotals = calculateTotals(child);
                    totalQty += childTotals.qty;
                    totalAmt += childTotals.amt;
                });

                node.Quantity = Number(totalQty.toFixed(2));
                node.Amount = Number(totalAmt.toFixed(2));

                // Remove dynamic fields from parent
                for (let i = 1; i <= $scope.totalDynamicFields; i++) {
                    delete node[`quantity_${i}`];
                    delete node[`amount_${i}`];
                }
            }

            return { qty: totalQty, amt: totalAmt };
        }

        // Only calculate totals starting from parent nodes
        allNodes.forEach(node => {
            if (!node.ParentId) calculateTotals(node);
        });
        console.log(allNodes);
        // Refresh Kendo TreeList
        const tree = $("#employeeTreeList").data("kendoTreeList");
        if (tree) tree.refresh();
    };

    $scope.updateQuantity = function (input, counter, id) {
        const value = parseFloat(input.value) || 0;
        const treeData = $scope.employeeTreeListOptions.dataSource.data();
        const item = treeData.find(x => x.Id === id);
        if (item) {
            item[`quantity_${counter}`] = value;
        }
        $scope.calculateTreeTotals(); // recalc totals if needed
    };

    $scope.updateAmount = function (input, counter, id) {
        const value = parseFloat(input.value) || 0;
        const treeData = $scope.employeeTreeListOptions.dataSource.data();
        const item = treeData.find(x => x.Id === id);
        if (item) {
            item[`amount_${counter}`] = value;
        }
        $scope.calculateTreeTotals(); // recalc totals if needed
    };
    //var treeData = [];
    //$scope.searchClick = function () {

    //    // --- Validations ---
    //    if (!$scope.Type) { displayPopupNotification("Select Type!", "error"); return; }
    //    if (!$scope.targetType) { displayPopupNotification("Select Target type!", "error"); return; }
    //    if (!$scope.targetName) { displayPopupNotification("Target Name is required!", "error"); return; }
    //    if (!$scope.selectedSubTarget) {
    //        displayPopupNotification("Select Sub Target!", "error"); return;
    //    }

    //    const subTarget = $scope.selectedSubTarget?.value; // "SALE" or "CUS"
    //    const isSaleSubTarget = subTarget === "SALE";
    //    const isCustomerSubTarget = subTarget === "CUS";

    //    // --- Determine date range ---
    //    const filter = $("#ddlDateFilterVoucher").val();
    //    const today = new Date();
    //    let bsFromDate, bsToDate;

    //    switch (filter) {
    //        case 'monthly':
    //            bsFromDate = new Date(today.getFullYear(), today.getMonth(), 1);
    //            bsToDate = new Date(today.getFullYear(), today.getMonth() + 1, 0);
    //            break;
    //        case 'thisyear':
    //            bsFromDate = new Date(today.getFullYear(), 0, 1);
    //            bsToDate = new Date(today.getFullYear(), 11, 31);
    //            break;
    //        case 'q1':
    //            bsFromDate = new Date(today.getFullYear(), 0, 1);
    //            bsToDate = new Date(today.getFullYear(), 2, 31);
    //            break;
    //        case 'q2':
    //            bsFromDate = new Date(today.getFullYear(), 3, 1);
    //            bsToDate = new Date(today.getFullYear(), 5, 30);
    //            break;
    //        case 'q3':
    //            bsFromDate = new Date(today.getFullYear(), 6, 1);
    //            bsToDate = new Date(today.getFullYear(), 8, 30);
    //            break;
    //        case 'q4':
    //            bsFromDate = new Date(today.getFullYear(), 9, 1);
    //            bsToDate = new Date(today.getFullYear(), 11, 31);
    //            break;
    //        default:
    //            bsFromDate = $('#FromDateVoucher').val() ? new Date($('#FromDateVoucher').val()) : today;
    //            bsToDate = $('#ToDateVoucher').val() ? new Date($('#ToDateVoucher').val()) : today;
    //    }

    //    const formattedFromDate = moment(bsFromDate).format('DD-MMM-YYYY');
    //    const formattedToDate = moment(bsToDate).format('DD-MMM-YYYY');

    //    // --- Fetch holidays ---
    //    $http.get(`/api/DistributionPlaningApi/HolidayDetails?fromDate=${formattedFromDate}&toDate=${formattedToDate}`)
    //        .then(function (response) {
    //            $scope.holidayList = response.data.map(h => moment(h.HOLIDAY_DATE).format("YYYY-MM-DD"));

    //            // --- Prepare payload for GetEmployeeTree ---
    //            const payload = {
    //                Type: isCustomerSubTarget ? "CUS" : "SAL",          // CUS for customer, SAL for sale
    //                SubTarget: isSaleSubTarget ? subTarget : null,      // only send subTarget if SALE
    //                SelectedEmployees: ($scope.selectedEmployees || []).map(e => ({
    //                    UserId: e.USERID || e.EMPLOYEE_CODE,
    //                    UserName: e.USER_NAME || e.EMPLOYEE_EDESC
    //                }))
    //            };

    //            return $http.post("/api/DistributionPlaningApi/GetEmployeeTree", JSON.stringify(payload), {
    //                headers: { 'Content-Type': 'application/json' }
    //            });

    //        })
    //        .then(function (res) {

    //            // --- Map employees for TreeList ---
    //            let treeData = res.data.map(node => ({
    //                Id: node.Id,
    //                Name: node.Name,
    //                Type: node.Type,
    //                Quantity: 0,
    //                Amount: 0,
    //                hasChildren: isCustomerSubTarget // only customer target has expandable children
    //            }));

    //            // --- Calculate working days ---
    //            function getWorkingDays(from, to) {
    //                let start = new Date(from);
    //                const end = new Date(to);
    //                let days = 0;
    //                while (start <= end) {
    //                    const dateStr = moment(start).format("YYYY-MM-DD");
    //                    if (start.getDay() !== 6 && !$scope.holidayList.includes(dateStr)) days++;
    //                    start.setDate(start.getDate() + 1);
    //                }
    //                return days;
    //            }
    //            const totalWorkingDays = getWorkingDays(bsFromDate, bsToDate);

    //            // --- Generate dynamic columns per day ---
    //            let dynamicColumns = [];
    //            let counter = 1;
    //            let start = new Date(bsFromDate);
    //            while (start <= new Date(bsToDate)) {
    //                const dateStr = moment(start).format("YYYY-MM-DD");
    //                if (start.getDay() !== 6 && !$scope.holidayList.includes(dateStr)) {
    //                    const bsDate = AD2BS(dateStr);

    //                    dynamicColumns.push({
    //                        field: `quantity_${counter}`,
    //                        title: `${bsDate} Qty`,
    //                        width: 80,
    //                        attributes: { style: "text-align:right" },
    //                        headerAttributes: { style: "text-align:center" },
    //                        template: `<input type="number" ng-model="dataItem.quantity_${counter}"
    //                        ng-change="updateTotalFromDaily(dataItem)"
    //                        class="form-control" style="text-align:right"/>`
    //                    });

    //                    dynamicColumns.push({
    //                        field: `amount_${counter}`,
    //                        title: `${bsDate} Amt`,
    //                        width: 100,
    //                        attributes: { style: "text-align:right" },
    //                        headerAttributes: { style: "text-align:center" },
    //                        template: `<input type="number" ng-model="dataItem.amount_${counter}"
    //                        ng-change="updateTotalFromDaily(dataItem)"
    //                        class="form-control" style="text-align:right"/>`
    //                    });

    //                    counter++;
    //                }
    //                start.setDate(start.getDate() + 1);
    //            }

    //            // --- Initialize totals for employees ---
    //            const totalQtyFromForm = parseFloat($scope.customQty) || 0;
    //            const totalAmtFromForm = parseFloat($scope.customAmt) || 0;

    //            treeData.forEach(emp => {
    //                emp.Quantity = totalQtyFromForm;
    //                emp.Amount = totalAmtFromForm;

    //                for (let i = 1; i < counter; i++) {
    //                    emp[`quantity_${i}`] = parseFloat((emp.Quantity / totalWorkingDays).toFixed(2));
    //                    emp[`amount_${i}`] = parseFloat((emp.Amount / totalWorkingDays).toFixed(2));
    //                }
    //            });

    //            // --- Functions to update totals / daily values ---
    //            $scope.updateTotalFromDaily = function (emp) {
    //                let totalQty = 0, totalAmt = 0;
    //                for (let i = 1; i < counter; i++) {
    //                    totalQty += parseFloat(emp[`quantity_${i}`]) || 0;
    //                    totalAmt += parseFloat(emp[`amount_${i}`]) || 0;
    //                }
    //                emp.Quantity = parseFloat(totalQty.toFixed(2));
    //                emp.Amount = parseFloat(totalAmt.toFixed(2));
    //            };

    //            $scope.updateDailyFromTotal = function (emp) {
    //                for (let i = 1; i < counter; i++) {
    //                    emp[`quantity_${i}`] = parseFloat((emp.Quantity / totalWorkingDays).toFixed(2));
    //                    emp[`amount_${i}`] = parseFloat((emp.Amount / totalWorkingDays).toFixed(2));
    //                }
    //            };

    //            let treeDataSource;

    //            if (isCustomerSubTarget) {
    //                console.log("TreeData fully prepared:", treeData);
    //                treeDataSource = new kendo.data.TreeListDataSource({
    //                    transport: {
    //                        read: function (e) {
    //                            // 1️⃣ Root nodes: employees
    //                            if (!e.data) {
    //                                console.log("Loading root employees", JSON.parse(JSON.stringify(treeData)));
    //                                e.success(treeData.map(emp => ({
    //                                    Id: emp.Id,
    //                                    Name: emp.Name,
    //                                    Type: emp.Type,
    //                                    ParentId: null,
    //                                    Quantity: emp.Quantity,
    //                                    Amount: emp.Amount,
    //                                    hasChildren: true
    //                                })));
    //                                return;
    //                            }

    //                            // 2️⃣ Child nodes: customers
    //                            if (e.data.Type === "Employee") {
    //                                const employeeId = e.data.Id;
    //                                console.log("Fetching customers for employee:", employeeId);
    //                                $http.post("/api/DistributionPlaningApi/GetCustomersByEmployee", {
    //                                    employeeIds: [employeeId],
    //                                    selectedCustomerCodes: $scope.selectedCustomerss || []
    //                                }).then(function (res) {
    //                                    const customers = res.data.map(c => ({
    //                                        Id: c.Id,
    //                                        Name: c.Name,
    //                                        Type: "Customer",
    //                                        ParentId: employeeId,
    //                                        Quantity: 0,
    //                                        Amount: 0,
    //                                        hasChildren: false
    //                                    }));
    //                                    console.log("Customers loaded:", customers);
    //                                    e.success(customers);
    //                                }, function (err) {
    //                                    e.error();
    //                                });
    //                                return;
    //                            }

    //                            // fallback
    //                            e.success([]);
    //                        }
    //                    },
    //                    schema: {
    //                        model: {
    //                            id: "Id",
    //                            parentId: "ParentId",
    //                            hasChildren: "hasChildren"
    //                        }
    //                    }
    //                });

    //            } else {

    //                treeDataSource = new kendo.data.TreeListDataSource({
    //                    data: treeData,
    //                    schema: {
    //                        model: {
    //                            id: "Id",
    //                            parentId: "ParentId"
    //                        }
    //                    }
    //                });
    //            }


    //            // --- Bind TreeList ---
    //            $timeout(function () {
    //                $scope.employeeTreeListOptions = {
    //                    dataSource: treeDataSource,
    //                    height: 500,
    //                    sortable: true,
    //                    filterable: true,
    //                    selectable: "multiple, row",
    //                    scrollable: true,
    //                    columns: [
    //                        { field: "Name", title: "Employee / Customer", width: 250 },
    //                        {
    //                            field: "Quantity",
    //                            title: "Total Qty",
    //                            width: 120,
    //                            attributes: { style: "text-align:right" },
    //                            headerAttributes: { style: "text-align:right" },
    //                            template: `<input type="number" ng-model="dataItem.Quantity" step="0.01" min="0"
    //                            ng-change="updateDailyFromTotal(dataItem)" class="form-control" style="text-align:right"/>`
    //                        },
    //                        {
    //                            field: "Amount",
    //                            title: "Total Amt",
    //                            width: 120,
    //                            attributes: { style: "text-align:right" },
    //                            headerAttributes: { style: "text-align:right" },
    //                            template: `<input type="number" ng-model="dataItem.Amount" step="0.01" min="0"
    //                            ng-change="updateDailyFromTotal(dataItem)" class="form-control" style="text-align:right"/>`
    //                        },
    //                        ...dynamicColumns
    //                    ],
    //                    dataBound: function (e) {
    //                        $scope.treeListWidget = e.sender;
    //                    }
    //                };
    //                $compile($("#employeeTreeList"))($scope);
    //            }, 500);

    //        })
    //        .catch(function (err) {
    //            console.error(err);
    //            displayPopupNotification("Error fetching employee tree", "error");
    //        });
    //};

    $scope.searchClick = function () {
        // --- Validations ---
        if (!$scope.Type) { displayPopupNotification("Select Type!", "error"); return; }
        if (!$scope.targetType) { displayPopupNotification("Select Target type!", "error"); return; }
        if (!$scope.targetName) { displayPopupNotification("Target Name is required!", "error"); return; }
        if (!$scope.selectedSubTarget && $scope.targetType === 'SAL') {
            displayPopupNotification("Select Sub Target Type!", "error"); return;
        }

        // --- Determine date range ---
        const filter = $("#ddlDateFilterVoucher").val();
        const today = new Date();
        let bsFromDate, bsToDate;

        switch (filter) {
            case 'monthly':
                bsFromDate = new Date(today.getFullYear(), today.getMonth(), 1);
                bsToDate = new Date(today.getFullYear(), today.getMonth() + 1, 0);
                break;
            case 'thisyear':
                bsFromDate = new Date(today.getFullYear(), 0, 1);
                bsToDate = new Date(today.getFullYear(), 11, 31);
                break;
            case 'q1':
                bsFromDate = new Date(today.getFullYear(), 0, 1);
                bsToDate = new Date(today.getFullYear(), 2, 31);
                break;
            case 'q2':
                bsFromDate = new Date(today.getFullYear(), 3, 1);
                bsToDate = new Date(today.getFullYear(), 5, 30);
                break;
            case 'q3':
                bsFromDate = new Date(today.getFullYear(), 6, 1);
                bsToDate = new Date(today.getFullYear(), 8, 30);
                break;
            case 'q4':
                bsFromDate = new Date(today.getFullYear(), 9, 1);
                bsToDate = new Date(today.getFullYear(), 11, 31);
                break;
            default:
                bsFromDate = $('#FromDateVoucher').val() ? new Date($('#FromDateVoucher').val()) : today;
                bsToDate = $('#ToDateVoucher').val() ? new Date($('#ToDateVoucher').val()) : today;
        }

        const formattedFromDate = moment(bsFromDate).format('DD-MMM-YYYY');
        const formattedToDate = moment(bsToDate).format('DD-MMM-YYYY');

        // --- Fetch holidays ---
        $http.get(`/api/DistributionPlaningApi/HolidayDetails?fromDate=${formattedFromDate}&toDate=${formattedToDate}`)
            .then(function (response) {
                $scope.holidayList = response.data.map(h => moment(h.HOLIDAY_DATE).format("YYYY-MM-DD"));
                const ms = $("#employees").data("kendoMultiSelect");

                const selectedEmployeeObjects = ms ? ms.dataItems() : [];
                const payload = {
                    Type: $scope.Type,
                    SubTarget: $scope.selectedSubTarget?.value || null,
                    SelectedEmployees: selectedEmployeeObjects.map(e => ({
                        UserId: e.USERID || e.EMPLOYEE_CODE,
                        UserName: e.FULL_NAME || e.EMPLOYEE_EDESC
                    }))
                };
                debugger;
                return $http.post("/api/DistributionPlaningApi/GetEmployeeTree", JSON.stringify(payload), {
                    headers: { 'Content-Type': 'application/json' }
                });

            })
            .then(function (res) {
                let treeData = res.data.map(emp => ({
                    Id: emp.Id,
                    Name: emp.Name,
                    Type: emp.Type,
                    Quantity: 0,
                    Amount: 0
                }));

                function getWorkingDays(from, to) {
                    let start = new Date(from);
                    const end = new Date(to);
                    let days = 0;
                    while (start <= end) {
                        const dateStr = moment(start).format("YYYY-MM-DD");
                        if (start.getDay() !== 6 && !$scope.holidayList.includes(dateStr)) days++;
                        start.setDate(start.getDate() + 1);
                    }
                    return days;
                }
                const totalWorkingDays = getWorkingDays(bsFromDate, bsToDate);

                let dynamicColumns = [];
                let counter = 1;
                let start = new Date(bsFromDate);
                while (start <= new Date(bsToDate)) {
                    const dateStr = moment(start).format("YYYY-MM-DD");
                    if (start.getDay() !== 6 && !$scope.holidayList.includes(dateStr)) {
                        const bsDate = AD2BS(dateStr);

                        dynamicColumns.push({
                            field: `quantity_${counter}`,
                            title: `${bsDate} Qty`,
                            width: 180,
                            attributes: { style: "text-align:right" },
                            headerAttributes: { style: "text-align:center" },
                            template: `<input type="number" ng-model="dataItem.quantity_${counter}" 
                             ng-change="updateTotalFromDaily(dataItem)" 
                             class="form-control" style="text-align:right"/>`
                        });

                        dynamicColumns.push({
                            field: `amount_${counter}`,
                            title: `${bsDate} Amt`,
                            width: 180,
                            attributes: { style: "text-align:right" },
                            headerAttributes: { style: "text-align:center" },
                            template: `<input type="number" ng-model="dataItem.amount_${counter}" 
                             ng-change="updateTotalFromDaily(dataItem)" 
                             class="form-control" style="text-align:right"/>`
                        });

                        counter++;
                    }
                    start.setDate(start.getDate() + 1);
                }

                // --- Bind total Quantity & Amount from form ---
                const totalQtyFromForm = parseFloat($scope.customQty) || 0;
                const totalAmtFromForm = parseFloat($scope.customAmt) || 0;

                treeData.forEach(emp => {
                    emp.Quantity = totalQtyFromForm;
                    emp.Amount = totalAmtFromForm;

                    for (let i = 1; i < counter; i++) {
                        emp[`quantity_${i}`] = parseFloat((emp.Quantity / totalWorkingDays).toFixed(2));
                        emp[`amount_${i}`] = parseFloat((emp.Amount / totalWorkingDays).toFixed(2));
                    }
                });

                // --- Update totals from daily changes ---
                $scope.updateTotalFromDaily = function (emp) {
                    let totalQty = 0, totalAmt = 0;
                    for (let i = 1; i < counter; i++) {
                        totalQty += parseFloat(emp[`quantity_${i}`]) || 0;
                        totalAmt += parseFloat(emp[`amount_${i}`]) || 0;
                    }
                    emp.Quantity = parseFloat(totalQty.toFixed(2));
                    emp.Amount = parseFloat(totalAmt.toFixed(2));
                };

                // --- Update daily values from totals ---
                $scope.updateDailyFromTotal = function (emp) {
                    for (let i = 1; i < counter; i++) {
                        emp[`quantity_${i}`] = parseFloat((emp.Quantity / totalWorkingDays).toFixed(2));
                        emp[`amount_${i}`] = parseFloat((emp.Amount / totalWorkingDays).toFixed(2));
                    }
                };

                // --- Initialize TreeList ---
                const treeDataSource = new kendo.data.TreeListDataSource({
                    data: treeData,
                    schema: {
                        model: { id: "Id", parentId: null, fields: { Name: { type: "string" }, Quantity: { type: "number" }, Amount: { type: "number" } } }
                    }
                });

                $timeout(function () {
                    $scope.employeeTreeListOptions = {
                        dataSource: treeDataSource,
                        height: 500,
                        sortable: true,
                        filterable: true,
                        selectable: "multiple, row",
                        scrollable: true,
                        columns: [
                            { field: "Name", title: "Employee", width: 250, locked: true },
                            {
                                field: "Quantity",
                                title: "Total Qty",
                                width: 120,
                                attributes: { style: "text-align:right" },
                                headerAttributes: { style: "text-align:right" },
                                template: `<input type="number" ng-model="dataItem.Quantity" step="0.01" min="0"
                                 ng-change="updateDailyFromTotal(dataItem)" class="form-control" style="text-align:right"/>`
                            },
                            {
                                field: "Amount",
                                title: "Total Amt",
                                width: 120,
                                attributes: { style: "text-align:right" },
                                headerAttributes: { style: "text-align:right" },
                                template: `<input type="number" ng-model="dataItem.Amount" step="0.01" min="0"
                                 ng-change="updateDailyFromTotal(dataItem)" class="form-control" style="text-align:right"/>`
                            },
                            ...dynamicColumns
                        ],
                        dataBound: function (e) { $scope.treeListWidget = e.sender; }
                    };
                    $compile($("#employeeTreeList"))($scope);
                }, 500);
            })
            .catch(function (err) {
                console.error(err);
                displayPopupNotification("Error fetching employee tree", "error");
            });
    };







    $scope.calculateRowTotals = function (data, dynamicColumns) {
        data.forEach(function (item) {
            var totalQuantity = 0;
            var totalAmount = 0;
            dynamicColumns.forEach(function (col) {
                col.columns.forEach(function (subCol) {
                    if (subCol.field.startsWith('quantity_')) {
                        var quantityValue = parseFloat(item[subCol.field]) || 0;
                        totalQuantity += quantityValue;
                    } else if (subCol.field.startsWith('amount_')) {
                        var amountValue = parseFloat(item[subCol.field]) || 0;
                        totalAmount += amountValue;
                    }
                });
            });
            item.totalQuantity = totalQuantity.toFixed(2);
            item.totalAmount = totalAmount.toFixed(2);
        });
    }
    //vertically calculation s
    $scope.addTotalRow = function (data, columns, selectedType) {
        var totals = {
            totalQuantity: 0,
            totalAmount: 0
        };

        // Initialize totals object with zeros
        columns.forEach(function (col) {
            col.columns.forEach(function (subCol) {
                if (subCol.field.startsWith('quantity_') || subCol.field.startsWith('amount_')) {
                    totals[subCol.field] = 0;
                }
            });
        });

        // Accumulate totals for each column
        data.forEach(function (item) {
            columns.forEach(function (col) {
                col.columns.forEach(function (subCol) {
                    if (subCol.field.startsWith('quantity_')) {
                        var quantityValue = parseFloat(item[subCol.field]) || 0;
                        totals[subCol.field] += quantityValue;
                    } else if (subCol.field.startsWith('amount_')) {
                        var amountValue = parseFloat(item[subCol.field]) || 0;
                        totals[subCol.field] += amountValue;
                    }
                });
            });
        });
        // Accumulate totals
        data.forEach(function (item) {
            totals.totalQuantity += parseFloat(item.totalQuantity) || 0;
            totals.totalAmount += parseFloat(item.totalAmount) || 0;
        });
        // Create total row with formatted totals for display
        var key;
        if (selectedType && selectedType.value == "ITM") {
            key = 'ITEM_EDESC';
        } else if (selectedType && selectedType.value == "CUS") {
            key = 'CUSTOMER_EDESC';
        }
        else {
            key = 'EMPLOYEE_EDESC';
        }
        var totalRow = {
            [key]: 'Total',
            totalQuantity: totals.totalQuantity.toFixed(2),
            totalAmount: totals.totalAmount.toFixed(2)
        };

        columns.forEach(function (col) {
            col.columns.forEach(function (subCol) {
                if (subCol.field.startsWith('quantity_') || subCol.field.startsWith('amount_')) {
                    totalRow[subCol.field] = parseFloat(parseFloat(totals[subCol.field]).toFixed(2));
                }
            });
        });
        data.push(totalRow);
    }
    $scope.updateTotals = function (datas) {
        var selectedType = $scope.selectedSubTarget;
        var dataSource = $scope.gridOptions.dataSource;
        var data = datas;
        var columns = dataSource.fields;
        // Filter date columns
        var dateColumns = columns.filter(function (col) {
            if (col.columns) {
                return col.columns.some(function (subCol) {
                    return subCol.field && (subCol.field.startsWith('quantity_') || subCol.field.startsWith('amount_'));
                });
            }
            return false;
        });
        var filteredData = data.filter(function (item) {
            return !(item.EMPLOYEE_EDESC === "Total" || item.ITEM_EDESC === "Total" || item.CUSTOMER_EDESC === "Total");
        });

        $scope.calculateRowTotals(filteredData, dateColumns);
        $scope.addTotalRow(filteredData, dateColumns, selectedType);
        $scope.gridOptions.dataSource.data = filteredData;
        $scope.gridOptions.columns = columns;
        $("#grid").data("kendoGrid").setOptions($scope.gridOptions);
        $("#grid").data("kendoGrid").refresh();
    };
    $scope.gridOptions = {
        dataSource: {
            data: [], // Initially empty, will be filled after search
            schema: {
                model: {
                    fields: {
                        EMPLOYEE_EDESC: { type: "string", editable: false },
                        ITEM_EDESC: { type: "string", editable: false },
                        totalAmount: { type: "number", editable: false },
                        totalQuantity: { type: "number", editable: false } // Field for total column
                    }
                }
            },
            pageSize: 50
        },
        scrollable: true,
        height: 400,
        editable: true,
        pageable: true,
        columns: [], // Columns will be set dynamically
        dataBound: function (e) {
            var grid = this;
            // Add a 'change' event listener to input elements within the grid's tbody
            grid.tbody.find('input').on('change', function () {
                var dataItem = grid.dataItem($(this).closest('tr')); // Get the data item (row) that was edited
                var newValue = parseFloat($(this).val()) || 0; // Get the new value from the input, default to 0 if NaN
                var $input = $(this);
                var ngModel = $input.attr('ng-model');
                var field = ngModel.split('.').pop();
                // Update the data item with the new value
                dataItem.set(field, newValue);
                // Update the totals across all rows
                var dataSource = grid.dataSource.data();
                $scope.updateTotals(dataSource);
                // Refresh the grid
                grid.refresh();
            });
        }

    };
    $scope.cancelClick = function () {
        // Reset the form fields
        $scope.targetName = '';
        $scope.customQty = '';
        $scope.customAmt = '';
        $scope.selectedTarget = null;
        $scope.selectedSubTarget = null;
        $scope.selectedItmGroup = [];
        $scope.selectedEmpGroup = [];
        $scope.selectedIndividual = [];
        $scope.selectedEmployees = [];
        $scope.customfrequencyday = null;
        $scope.frequencyWiseRouteAssign = false;

        // Reset Kendo UI widgets
        var kendoWidgets = [
            $("#subTargetType").data("kendoDropDownList"),
            $("#itemGroup").data("kendoMultiSelect"),
            $("#empGroup").data("kendoMultiSelect"),
            $("#individual").data("kendoMultiSelect"),
            $("#employees").data("kendoMultiSelect")
        ];

        kendoWidgets.forEach(function (widget) {
            if (widget) {
                widget.value(""); // Clear value for DropDownList and MultiSelect
                widget.dataSource.read(); // Refresh the data source if necessary
            }
        });

        // Reset the form validation state
        if ($scope.targetForm) {
            $scope.targetForm.$setPristine();
            $scope.targetForm.$setUntouched();
        }
    };
    $scope.subTargetChange = function () {
        // Reset the form fields
        $scope.selectedItmGroup = [];
        $scope.selectedEmpGroup = [];
        $scope.selectedIndividual = [];
        $scope.selectedEmployees = [];
        $scope.customfrequencyday = null;
        $scope.frequencyWiseRouteAssign = false;

        // Reset Kendo UI widgets
        var kendoWidgets = [
            $("#itemGroup").data("kendoMultiSelect"),
            $("#empGroup").data("kendoMultiSelect"),
            $("#individual").data("kendoMultiSelect"),
            $("#employees").data("kendoMultiSelect")
        ];

        kendoWidgets.forEach(function (widget) {
            if (widget) {
                widget.value(""); // Clear value for DropDownList and MultiSelect
                widget.dataSource.read(); // Refresh the data source if necessary
            }
        });

        // Reset the form validation state
        if ($scope.targetForm) {
            $scope.targetForm.$setPristine();
            $scope.targetForm.$setUntouched();
        }
    };
    //$scope.assignTargets = function () {
    //    var dataToAssign = $scope.collectDataForAssign();
    //    console.log(dataToAssign);
    //    $http.post('/api/DistributionPlaningApi/SaveTargetData', dataToAssign)
    //        .then(function (response) {
    //            window.location = "/Planning/DistributionPlaning/Index#!Planning/TargetSetup";
    //            displayPopupNotification(response.data, "success");
    //        }, function (error) {
    //            displayPopupNotification(response.data, "error");
    //        });
    //};

    //$scope.assignTargets = function () {
    //    // Choose correct data collector based on subTargetType
    //    var dataToAssign = $scope.selectedSubTarget?.value === "SALE"
    //        ? $scope.collectDataForSalesTarget()
    //        : $scope.collectDataForAssign();
    //    debugger;

    //    console.log(dataToAssign);

    //    $http.post('/api/DistributionPlaningApi/SaveTargetData', dataToAssign)
    //        .then(function (response) {
    //            window.location = "/Planning/DistributionPlaning/Index#!Planning/TargetSetup";
    //            displayPopupNotification(response.data, "success");
    //        }, function (error) {
    //            displayPopupNotification(error.data, "error");
    //        });
    //};

    $scope.assignTargets = function () {
        let dataToAssign;

        if ($scope.selectedSubTarget?.value === "SALE") {
            // Prepare data for Sales Target
            dataToAssign = $scope.collectDataForSalesTarget();
            $http.post('/api/DistributionPlaningApi/SaveDNMSalesTarget', dataToAssign)
                .then(function (response) {
                    displayPopupNotification(response.data.message, "success");
                    window.location = "/Planning/DistributionPlaning/Index#!Planning/TargetSetup";
                }, function (error) {
                    displayPopupNotification(error.data?.message || "Error saving Sales Target", "error");
                });
        } else {
            // Prepare data for regular DNM Target
            dataToAssign = $scope.collectDataForAssign();
            $http.post('/api/DistributionPlaningApi/SaveDNMTarget', dataToAssign)
                .then(function (response) {
                    displayPopupNotification(response.data.message, "success");
                    window.location = "/Planning/DistributionPlaning/Index#!Planning/TargetSetup";
                }, function (error) {
                    displayPopupNotification(error.data?.message || "Error saving DNM Target", "error");
                });
        }
    };
    $scope.collectDataForAssign = function () {
        // Store as arrays or null
        var itmGroup = $scope.selectedItmGroup && $scope.selectedItmGroup.length > 0
            ? $scope.selectedItmGroup.map(function (item) {
                return item.ITEM_CODE;
            }).join(',')
            : '';
        var cusGroup = $scope.selectedCusGroup && $scope.selectedCusGroup.length > 0
            ? $scope.selectedCusGroup.map(function (item) {
                return item.GROUP_ID;
            }).join(',')
            : '';
        var empGroup = $scope.selectedEmpGroup && $scope.selectedEmpGroup.length > 0
            ? $scope.selectedEmpGroup.map(function (emp) {
                return emp.GROUP_ID;
            }).join(',')
            : '';
        var itmMasterGroup = $scope.selectedItmGroup && $scope.selectedItmGroup.length > 0
            ? $scope.selectedItmGroup.map(function (item) {
                return item.MASTER_ITEM_CODE;
            }).join(',')
            : '';
        var cusMasterGroup = $scope.selectedCusGroup && $scope.selectedCusGroup.length > 0
            ? $scope.selectedCusGroup.map(function (item) {
                return item.MASTER_CUSTOMER_CODE;
            }).join(',')
            : '';
        if ($scope.Type == 'SNG') {
            var empMasterGroup = $scope.selectedEmpGroup && $scope.selectedEmpGroup.length > 0
                ? $scope.selectedEmpGroup.map(function (emp) {
                    return emp.MASTER_CUSTOMER_CODE;
                }).join(',')
                : '';
        } else {
            var empMasterGroup = $scope.selectedEmpGroup && $scope.selectedEmpGroup.length > 0
                ? $scope.selectedEmpGroup.map(function (emp) {
                    return emp.GROUP_ID;
                }).join(',')
                : '';
        }
        var individual = $scope.selectedIndividual && $scope.selectedIndividual.length > 0 ? $scope.selectedIndividual.map(function (itm) {
            return {
                item_code: itm.ITEM_CODE,
                mu_code: itm.MU_CODE
            };
        }) : [];

        var employees = $scope.selectedEmployees && $scope.selectedEmployees.length > 0 ? $scope.selectedEmployees.map(function (emp) {
            return emp.EMPLOYEE_CODE;
        }) : [];
        var customers = $scope.selectedCustomer && $scope.selectedCustomer.length > 0 ? $scope.selectedCustomer.map(function (cus) {
            return cus.CUSTOMER_CODE;
        }) : [];

        var gridData = $scope.getGridData() || []; 

        var dataToAssign = {
            targetId: $scope.targetId || 0,
            targetType: $scope.targetType || null,
            DateFilter: $("#ddlDateFilterVoucher").val() || null,
            subTargetType: $scope.selectedSubTarget ? $scope.selectedSubTarget.value : null,
            targetName: $scope.targetName || null,
            itemGroup: itmGroup,
            employeeGroup: empGroup,
            customerGroup: cusGroup,
            itemMasterGroup: itmMasterGroup,
            employeeMasterGroup: empMasterGroup,
            customerMasterGroup: cusMasterGroup,
            flag: $scope.Type,
            items: individual,
            employees: employees,
            customers: customers,
            gridData: gridData
        };

        return dataToAssign;
    };

    $scope.collectDataForSalesTarget = function () {
        debugger;
        var empGroup = ($scope.selectedEmpGroup || [])
            .map(function (emp) {
                return emp.GROUPID; 
            })
            .filter(e => e);

        var employees = ($scope.selectedEmployees || []).map(function (emp) {
            return $scope.Type === "SNG" ? emp : emp;
            //return $scope.Type === "SNG" ? emp.EMPLOYEE_CODE : emp.USERID;
        }).filter(e => e);

        var gridData = employees.map(empId => ({
            Quantity: parseFloat($scope.customQty || 0), // total quantity from form
            Amount: parseFloat($scope.customAmt || 0),   // total amount from form
        }));
        var dataToAssign = {
            targetId: $scope.targetId || 0,
            targetType: $scope.targetType || null,
            DateFilter: $("#ddlDateFilterVoucher").val() || null,
            subTargetType: $scope.selectedSubTarget ? $scope.selectedSubTarget.value : null,
            targetName: String($scope.targetName || ""),
            employeeGroup: $scope.selectedEmpGroup || [], 
            flag: $scope.Type,
            //employees: $scope.selectedEmployees || [], 
            //employees: ($scope.selectedEmployees || []).map(e => e.USERID) ,
            employees: ($scope.selectedEmployees || []).map(e => {
                if (typeof e === "object" && e !== null && e.USERID !== undefined) {
                    return e.USERID;
                }
                return e;
            }) ,
            gridData: $scope.getGridData()
        };
        debugger;
        return dataToAssign;
    };

   

    $scope.getGridData = function () {
        var grid = $("#employeeTreeList").data("kendoTreeList");
        if (!grid) return [];

        var data = grid.dataSource.data();

        return data.map(function (rowData) {
            return {
                Quantity: parseInt(rowData.Quantity) || 0,
                Amount: parseInt(rowData.Amount) || 0
            };
        });
    };

});
