distributionModule.controller('UserSetupCtrl', function ($scope, DistSetupService, $routeParams, $timeout) {
    'use strict';

    // ========================================
    // INITIALIZATION
    // ========================================

    const API_BASE = `${window.location.protocol}//${window.location.host}/api`;

    $scope.param = $routeParams.param;
    $scope.pageName = "AddUser";
    $scope.EmployeeMultiSelectName = "Employee";
    $scope.createPanel = true;
    $scope.saveButtonText = "Save";
    $scope._old = {};
    $scope.selectedArea = null;
    $scope.preservedCustomerValues = null;
    $scope.pendingCustomerRestore = null;
    $scope.userSetupTree = {};

    // ========================================
    // UTILITY FUNCTIONS
    // ========================================

    function clearForm() {
        $scope.userSetupTree = {};
        $scope.preservedCustomerValues = null;
        $scope.pendingCustomerRestore = null;

        const multiSelects = [
            'EmployeeMultiSelect',
            'RoleMultiSelect',
            'AreaMultiSelect',
            'distItemsSelect',
            'distCustomerSelect'
        ];

        multiSelects.forEach(id => {
            const element = angular.element(`#${id}`).data("kendoMultiSelect");
            if (element) {
                element.value([]);
            }
        });

        const empSelect = angular.element('#EmployeeMultiSelect').data("kendoMultiSelect");
        if (empSelect) {
            empSelect.enable(true);
        }
    }

    function getApiUrl(endpoint) {
        return `${API_BASE}${endpoint}`;
    }

    function showNotification(message, type) {
        if (type === 'popup') {
            displayPopupNotification(message, 'success');
        } else {
            displayPopupNotification(message, type);
        }
    }

    function applySlimScroll(elementId, height = '200px') {
        angular.element(`#${elementId}_listbox`).slimScroll({ height });
    }

    // ========================================
    // DATA SOURCE CONFIGURATIONS
    // ========================================

    function createDataSource(endpoint) {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: getApiUrl(endpoint),
                    dataType: "json",
                    contentType: "application/json;charset=utf-8"
                }
            }
        });
    }

    const productsDataSource = createDataSource('/DistributionPurchase/GetAllItems');

    // ========================================
    // MULTISELECT CONFIGURATIONS
    // ========================================

    $scope.EmployeeMultiSelect = {
        dataSource: createDataSource('/UserSetup/getUserEmployee'),
        dataTextField: "EMPLOYEE_EDESC",
        dataValueField: "EMPLOYEE_CODE",
        maxSelectedItems: 1,
        enable: false,
        filter: "contains",
        height: 600,
        headerTemplate: '<div class="col-md-offset-4"><strong>Employee...</strong></div>',
        placeholder: "Select Employee...",
        autoClose: true,
        dataBound: function () {
            const current = this.value();
            this._savedOld = current.slice(0);
            applySlimScroll('EmployeeMultiSelect', '200px');
        },
        change: function (evt) {
            const dataItem = evt.sender.dataItem();
            if (dataItem) {
                $scope.userSetupTree.FullName = dataItem.EMPLOYEE_EDESC;
            }
        }
    };

    $scope.RoleMultiSelect = {
        dataSource: createDataSource('/Setup/GetDistUserRole'),
        dataTextField: "ROLE_NAME",
        dataValueField: "ROLE_CODE",
        maxSelectedItems: 1,
        filter: "contains",
        height: 600,
        headerTemplate: '<div class="col-md-offset-4"><strong>Role...</strong></div>',
        placeholder: "Select Role...",
        autoClose: true,
        dataBound: function () {
            const current = this.value();
            this._savedOld = current.slice(0);
            applySlimScroll('RoleMultiSelect', '100px');
        },
        change: function (evt) {
            const dataItem = evt.sender.dataItem();
            if (!dataItem) return false;

            if (dataItem.ROLE_CODE === 2) {
                setDistributorDataSource();
            } else if ($scope._old.EmployeeMultiSelectData) {
                restoreEmployeeDataSource();
            }
        }
    };

    $scope.AreaMultiSelect = {
        dataTextField: "AREA_NAME",
        dataValueField: "AREA_CODE",
        height: 600,
        valuePrimitive: true,
        filter: "contains",
        placeholder: "Select Area...",
        autoClose: false,
        headerTemplate: `
      <div class="k-header d-flex align-items-center ng-scope" 
           style="display:flex;flex-direction:row;justify-content:space-between;align-items:center;">
        <strong>Area</strong>
        <div style="font-size:9px;cursor:pointer;" 
             onmouseover="this.style.textDecoration='underline'" 
             onmouseout="this.style.textDecoration='none'" 
             onclick="angular.element(this).scope().selectAllAreas()">
          Select All
        </div>
      </div>`,
        dataBound: function (e) {
            $(`#${e.sender.element[0].id}_listbox`).slimScroll({
                'height': '179px',
                'scroll': 'scroll'
            });
        },
        dataSource: {
            transport: {
                read: {
                    url: getApiUrl('/Distribution/GetDistArea'),
                    dataType: "json"
                }
            }
        }
    };

    $scope.distCustomerSelectOptions = {
        dataTextField: "CUSTOMER_EDESC",
        dataValueField: "CUSTOMER_CODE",
        height: 600,
        valuePrimitive: true,
        filter: "contains",
        placeholder: "Select Customer...",
        autoClose: false,
        headerTemplate: `
      <div class="k-header d-flex align-items-center ng-scope" 
           style="display:flex;flex-direction:row;justify-content:space-between;align-items:center;">
        <strong>Customer</strong>
        <div style="font-size:9px;cursor:pointer;" 
             onmouseover="this.style.textDecoration='underline'" 
             onmouseout="this.style.textDecoration='none'" 
             onclick="angular.element(this).scope().selectAllCustomers()">
          Select All
        </div>
      </div>`,
        dataBound: function (e) {
            $(`#${e.sender.element[0].id}_listbox`).slimScroll({
                'height': '179px',
                'scroll': 'scroll'
            });

            // Check if we have values to restore
            const valuesToRestore = $scope.preservedCustomerValues || $scope.pendingCustomerRestore;

            if (valuesToRestore && valuesToRestore.length > 0) {
                const self = this;

                $timeout(() => {
                    const availableData = self.dataSource.data();
                    const availableValues = availableData.map(item => item.CUSTOMER_CODE);

                    const validValues = valuesToRestore.filter(value =>
                        availableValues.indexOf(value) !== -1
                    );

                    if (validValues.length > 0) {
                        self.value(validValues);
                        $scope.userSetupTree.CustomerMultiSelect = validValues;
                    }

                    // Clear both restoration flags
                    $scope.preservedCustomerValues = null;
                    $scope.pendingCustomerRestore = null;
                }, 150);
            }
        },
        change: function (e) {
            const selectedValues = this.value();
            $scope.userSetupTree.CustomerMultiSelect = selectedValues;
            $scope.$apply();
        },
        dataSource: new kendo.data.DataSource({
            type: "json",
            transport: {
                read: {
                    url: getApiUrl('/Distribution/GetCustomersByArea'),
                    type: "GET",
                    dataType: "json",
                    data: function () {
                        // Deduplicate area codes to avoid massive URLs
                        const uniqueAreaCodes = $scope.selectedArea
                            ? _.uniq($scope.selectedArea)
                            : [];
                        const areaCodes = uniqueAreaCodes.length > 0
                            ? uniqueAreaCodes.map(area => `'${area}'`).join(',')
                            : '';
                        return { areaCode: areaCodes };
                    }
                }
            }
        })
    };

    // ========================================
    // BRAND AND ITEMS MULTISELECT
    // ========================================

    productsDataSource.fetch(function () {
        $scope.distBrandSelectOptions = {
            dataTextField: "BRAND_NAME",
            dataValueField: "ITEM_CODE",
            height: 600,
            valuePrimitive: true,
            placeholder: "Select Brands...",
            autoClose: false,
            headerTemplate: `
        <div class="k-header d-flex align-items-center ng-scope" 
             style="display:flex;flex-direction:row;justify-content:space-between;align-items:center;">
          <strong>Brands</strong>
          <div style="font-size:9px;cursor:pointer;" 
               onmouseover="this.style.textDecoration='underline'" 
               onmouseout="this.style.textDecoration='none'" 
               onclick="angular.element(this).scope().selectAllBrands()">
            Select All
          </div>
        </div>`,
            dataBound: function (e) {
                const current = this.value();
                this._savedOld = current.slice(0);
                $(`#${e.sender.element[0].id}_listbox`).slimScroll({
                    'height': '179px',
                    'scroll': 'scroll'
                });
            },
            dataSource: new kendo.data.DataSource({
                data: _.uniq(this.data(), "BRAND_NAME")
            }),
            change: function () {
                buildItemFilters(this.dataItems());
            }
        };
    });

    $scope.distItemsSelectOptions = {
        dataTextField: "ITEM_EDESC",
        dataValueField: "ITEM_CODE",
        height: 600,
        valuePrimitive: true,
        headerTemplate: '<div class="col-md-offset-3"><strong>Items...</strong></div>',
        placeholder: "Select Items...",
        autoClose: false,
        dataBound: function (e) {
            const current = this.value();
            this._savedOld = current.slice(0);
            $(`#${e.sender.element[0].id}_listbox`).slimScroll({
                'height': '179px',
                'scroll': 'scroll'
            });
        },
        dataSource: productsDataSource
    };

    // ========================================
    // HELPER FUNCTIONS FOR MULTISELECT
    // ========================================

    function buildItemFilters(dataItems) {
        const filters = [];
        const itemSelect = $("#distItemsSelect").data("kendoMultiSelect");

        if (!itemSelect || dataItems.length === 0) {
            if (itemSelect) itemSelect.value("");
            return;
        }

        const allData = itemSelect.dataSource.data();

        dataItems.forEach(dataItem => {
            const filtered = _.filter(allData, item => item.BRAND_NAME === dataItem.BRAND_NAME);
            filtered.forEach(item => filters.push(item.ITEM_CODE));
        });

        itemSelect.value(filters);
    }

    $scope.selectAllAreas = function () {
        selectAllInMultiSelect('AreaMultiSelect', values => {
            $scope.userSetupTree.AreaMultiSelect = values;
        });
    };

    $scope.selectAllCustomers = function () {
        selectAllInMultiSelect('distCustomerSelect', values => {
            $scope.userSetupTree.CustomerMultiSelect = values;
        });
    };

    $scope.selectAllBrands = function () {
        const multiselect = $("#distBrandSelect").data("kendoMultiSelect");
        if (!multiselect) return;

        multiselect.dataSource.fetch().then(() => {
            const allValues = multiselect.dataSource.data()
                .map(item => item[multiselect.options.dataValueField]);
            multiselect.value(allValues);
            $scope.userSetupTree.BrandMultiSelect = allValues;
            buildItemFilters(multiselect.dataItems());
            $scope.$apply();
        });
    };

    function selectAllInMultiSelect(elementId, callback) {
        const multiselect = $(`#${elementId}`).data("kendoMultiSelect");
        if (!multiselect) {
            console.error("MultiSelect not initialized yet.");
            return;
        }

        multiselect.dataSource.fetch().then(() => {
            const allValues = multiselect.dataSource.data()
                .map(item => item[multiselect.options.dataValueField]);
            multiselect.value(allValues);
            if (callback) callback(allValues);
            $scope.$apply();
        });
    }

    $scope.restoreCustomerValues = function (valuesToRestore) {
        const customerMultiSelect = $("#distCustomerSelect").data("kendoMultiSelect");
        if (!customerMultiSelect || !valuesToRestore || valuesToRestore.length === 0) {
            return;
        }

        // Wait for dataSource to be populated
        customerMultiSelect.dataSource.fetch().then(() => {
            const availableData = customerMultiSelect.dataSource.data();
            const availableValues = availableData.map(item => item.CUSTOMER_CODE);

            // Filter to only include values that exist in the new data
            const validValues = valuesToRestore.filter(value =>
                availableValues.indexOf(value) !== -1
            );

            if (validValues.length > 0) {
                $timeout(() => {
                    customerMultiSelect.value(validValues);
                    $scope.userSetupTree.CustomerMultiSelect = validValues;
                }, 50);
            } else {
                // If none of the previous values are valid, clear the selection
                customerMultiSelect.value([]);
                $scope.userSetupTree.CustomerMultiSelect = [];
            }
        });
    };

    // ========================================
    // DATA SOURCE SWITCHING
    // ========================================

    function setDistributorDataSource() {
        const dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: getApiUrl('/UserSetup/GetDistributor'),
                    dataType: "json",
                    contentType: "application/json;charset=utf-8"
                }
            },
            schema: {
                parse: function (response) {
                    _.each(response, item => {
                        item.EMPLOYEE_CODE = item.DISTRIBUTOR_CODE;
                        item.EMPLOYEE_EDESC = item.CUSTOMER_EDESC;
                    });
                    return response;
                }
            }
        });

        const empSelect = angular.element('#EmployeeMultiSelect').data("kendoMultiSelect");
        $scope._old.EmployeeMultiSelectData = $.extend(true, {}, empSelect.dataSource.data());
        empSelect.setDataSource(dataSource);
        $scope.EmployeeMultiSelectName = "Distributor";
    }

    function restoreEmployeeDataSource() {
        const empSelect = angular.element('#EmployeeMultiSelect').data("kendoMultiSelect");
        empSelect.setDataSource(new kendo.data.DataSource({
            data: $scope._old.EmployeeMultiSelectData
        }));
        $scope.EmployeeMultiSelectName = "Employee";
    }

    function setSalesPersonDataSource() {
        const salesDataSource = createDataSource('/UserSetup/getUserEmployee');
        const empSelect = angular.element('#EmployeeMultiSelect').data("kendoMultiSelect");
        $scope._old.EmployeeMultiSelectData = $.extend(true, {}, empSelect.dataSource.data());
        empSelect.setDataSource(salesDataSource);
        $scope.EmployeeMultiSelectName = "Employee";
    }

    // ========================================
    // WATCHERS
    // ========================================

    $scope.$watch('userSetupTree.AreaMultiSelect', function (newVal, oldVal) {
        if (newVal === oldVal) return;

        $scope.selectedArea = newVal;
        const customerMultiSelect = $("#distCustomerSelect").data("kendoMultiSelect");

        if (customerMultiSelect) {
            // Check if we're in an update scenario with pending customer restore
            const hasPendingRestore = $scope.pendingCustomerRestore &&
                $scope.pendingCustomerRestore.length > 0;

            if (!hasPendingRestore) {
                // Normal area change - save current selected values
                const currentSelectedValues = customerMultiSelect.value() || [];
                $scope.preservedCustomerValues = currentSelectedValues.slice(0);
            } else {
                // Update scenario - use pending customer values
                $scope.preservedCustomerValues = $scope.pendingCustomerRestore.slice(0);
                $scope.pendingCustomerRestore = null;
            }

            // Read new data based on selected areas
            customerMultiSelect.dataSource.read();
        }
    });

    // ========================================
    // TREELIST INITIALIZATION
    // ========================================

    angular.element('#treelist').kendoTreeList({
        dataSource: new kendo.data.TreeListDataSource({
            transport: {
                read: {
                    url: getApiUrl('/Setup/GetUserSetupTreeList'),
                    dataType: "json"
                }
            },
            schema: {
                model: {
                    id: "CODE",
                    parentId: "MASTER_CODE",
                    fields: {
                        CODE: { type: "number", nullable: false },
                        MASTER_CODE: { field: "MASTER_CODE", type: "number", nullable: true }
                    }
                }
            }
        }),
        dataBound: function (e) {
            GetSetupSetting("UserSetUpTree");
        },
        editable: { move: true },
        dragstart: function (e) {
            if (e.source.IS_GROUP === 'Y') {
                e.preventDefault();
            }
        },
        drop: function (e) {
            $scope.UpdateUserTreeOrder(e);
        },
        columns: [
            { field: "NAME", expandable: true, title: "Name" },
            { field: "FULLNAME", title: "Full Name" },
            { field: "ROLE_NAME", title: "Role" },
            { field: "AREA_NAME", title: "Area" },
            { field: "EMAIL", title: "Email" },
            { field: "CONTACT_NO", title: "Contact No" }
        ]
    });

    // ========================================
    // CONTEXT MENU
    // ========================================

    angular.element('#menu').kendoContextMenu({
        target: "#treelist",
        filter: "tbody > tr",
        select: function (e) {
            const button = $(e.item);
            const row = $(e.target);
            const dataItem = $("#treelist").data("kendoTreeList").dataItem(row);
            const areaMultiSelect = angular.element('#AreaMultiSelect').data("kendoMultiSelect");

            // Store original area data
            $scope._old.areaMultiSelectData = $scope._old.areaMultiSelectData ||
                areaMultiSelect.dataSource.data();

            // Filter area data based on manager status
            if (dataItem.MGR_USER === 'Y') {
                areaMultiSelect.setDataSource(new kendo.data.DataSource({
                    data: $scope._old.areaMultiSelectData
                }));
            } else {
                areaMultiSelect.setDataSource(new kendo.data.DataSource({
                    data: _.filter($scope._old.areaMultiSelectData, x =>
                        x.GROUPID === dataItem.GROUPID
                    )
                }));
            }

            const action = button.text();

            if (action === "Update") {
                handleUpdate(dataItem, areaMultiSelect);
            } else if (action === "Add") {
                handleAdd(dataItem);
            } else if (action === "Delete") {
                handleDelete(dataItem);
            }

            // Set master codes
            if (dataItem.parentId == null) {
                $scope.userSetupTree.MASTER_CODE = dataItem.CODE;
            } else {
                $scope.userSetupTree.MASTER_CODE = dataItem.GROUPID;
                $scope.userSetupTree.MASTER_CUSTOMER_CODE = dataItem.CODE;
            }

            $scope.$apply();
        }
    });

    function handleUpdate(dataItem, areaMultiSelect) {
        clearForm();

        // Clear any preserved customer values from previous operations
        $scope.preservedCustomerValues = null;

        $scope.pageName = "UpdateUser";
        $scope.saveButtonText = "Update";

        const roleMultiSelect = angular.element('#RoleMultiSelect').data("kendoMultiSelect");
        $scope._old.roleMultiSelect = $scope._old.roleMultiSelect ||
            roleMultiSelect.dataSource.data();

        if (dataItem.ROLE_CODE !== undefined) {
            if (dataItem.ROLE_CODE === '2') {
                setDistributorDataSource();
                roleMultiSelect.setDataSource(new kendo.data.DataSource({
                    data: _.filter($scope._old.roleMultiSelect, x => x.ROLE_CODE === 2)
                }));
            } else {
                setSalesPersonDataSource();
                roleMultiSelect.setDataSource(new kendo.data.DataSource({
                    data: _.filter($scope._old.roleMultiSelect, x => x.ROLE_CODE !== 2)
                }));
            }
        }

        const customerCodes = dataItem.CUSTOMER_CODE ? dataItem.CUSTOMER_CODE.split(',') : [];
        const areaCodes = dataItem.AREA_CODE ? dataItem.AREA_CODE.split(',') : [];

        // Deduplicate area and customer codes
        const uniqueAreaCodes = _.uniq(areaCodes);
        const uniqueCustomerCodes = _.uniq(customerCodes);

        $scope.userSetupTree = {
            attendanceCheckbox: dataItem.ATTENDENCE === 'Y',
            mobileCheckbox: dataItem.MOBILE === 'Y',
            activeCheckbox: dataItem.ACTIVE === 'Y',
            brandingCheckbox: dataItem.BRANDING === 'Y',
            allVisitPlan: dataItem.ALL_PLAN === 'Y',
            superCheckbox: dataItem.SUPER_USER === 'Y',
            managerCheckbox: dataItem.MGR_USER === 'Y',
            CODE: dataItem.CODE,
            FullName: dataItem.FULLNAME,
            NAME: dataItem.NAME,
            EMAIL: dataItem.EMAIL,
            Password: dataItem.PASSWORD,
            CONTACT_NO: dataItem.CONTACT_NO,
            EmployeeMultiSelect: [dataItem.EMPLOYEE_CODE],
            RoleMultiSelect: [dataItem.ROLE_CODE],
            AreaMultiSelect1: uniqueAreaCodes,
            ItemCodeMultiSelect: dataItem.ITEM_CODE ? dataItem.ITEM_CODE.split(',') : [],
            CustomerMultiSelect: uniqueCustomerCodes,
            BrandMultiSelect: dataItem.ITEM_CODE ? dataItem.ITEM_CODE.split(',') : []
        };

        const employeeMultiSelect = angular.element('#EmployeeMultiSelect').data("kendoMultiSelect");
        employeeMultiSelect.value($scope.userSetupTree.EmployeeMultiSelect);
        employeeMultiSelect.enable(false);

        // Store customer values that need to be restored after area data loads
        if (uniqueCustomerCodes.length > 0) {
            $scope.pendingCustomerRestore = uniqueCustomerCodes.slice(0);
        }

        // Set area - this will trigger the watcher which will load customers
        areaMultiSelect.value(uniqueAreaCodes);
        $scope.userSetupTree.AreaMultiSelect = uniqueAreaCodes;

        angular.element('#RoleMultiSelect').data("kendoMultiSelect")
            .value($scope.userSetupTree.RoleMultiSelect);

        angular.element('#userSetupTreeCreateModal').modal('show');
    }

    function handleAdd(dataItem) {
        clearForm();
        $scope.pageName = "SaveUser";
        $scope.saveButtonText = "Save";
        angular.element('#EmployeeMultiSelect').data("kendoMultiSelect").enable(true);
        angular.element('#userSetupTreeCreateModal').modal('show');
        $scope.userSetupTree.GROUPID = dataItem.GROUPID;
    }

    function handleDelete(dataItem) {
        bootbox.confirm({
            message: "Do you want to delete this User? This cannot be undone.",
            buttons: {
                cancel: { label: 'Cancel' },
                confirm: { label: 'Confirm' }
            },
            callback: function (result) {
                if (!result) return;

                $.ajax({
                    type: 'GET',
                    contentType: "application/json;charset=utf-8",
                    dataType: "json",
                    url: getApiUrl(`/Setup/DeleteUserTree?Code=${dataItem.CODE}`),
                    success: function (result) {
                        if (result === "200") {
                            showNotification("Deleted Successfully", "success");
                            angular.element('#treelist').data("kendoTreeList").dataSource.read();
                        } else {
                            showNotification("Error", "error");
                        }
                    },
                    error: function () {
                        showNotification("Error", "error");
                    }
                });
            }
        });
    }

    // ========================================
    // MODAL EVENT HANDLERS
    // ========================================

    $('#userSetupTreeCreateModal').on('shown.bs.modal', function () {
        const customerMultiSelect = $("#distCustomerSelect").data("kendoMultiSelect");

        // Check if there are pending customer values to restore
        if (customerMultiSelect && $scope.pendingCustomerRestore &&
            $scope.pendingCustomerRestore.length > 0) {

            customerMultiSelect.dataSource.fetch().then(() => {
                $scope.restoreCustomerValues($scope.pendingCustomerRestore);
                $scope.pendingCustomerRestore = null;
            });
        }
    });

    $('#userSetupTreeCreateModal').on('hidden.bs.modal', function () {
        // Clean up when modal is closed
        $scope.preservedCustomerValues = null;
        $scope.pendingCustomerRestore = null;
    });

    // ========================================
    // SAVE/UPDATE FUNCTIONS
    // ========================================

    $scope.saveUser = function (isValid) {
        if (!isValid) {
            showNotification("Invalid Field", "warning");
            return;
        }

        const customerMultiSelect = $("#distCustomerSelect").data("kendoMultiSelect");
        if (customerMultiSelect) {
            $scope.userSetupTree.CustomerMultiSelect = customerMultiSelect.value();
        }

        const obj = prepareUserData();

        if ($scope.saveButtonText === "Save") {
            saveNewUser(obj);
        } else {
            updateExistingUser(obj);
        }
    };

    function prepareUserData() {
        const obj = $scope.userSetupTree;

        obj.EMPLOYEE_CODE = $scope.userSetupTree.EmployeeMultiSelect[0];
        obj.ROLE_CODE = $scope.userSetupTree.RoleMultiSelect[0];
        obj.ATTENDENCE = $scope.userSetupTree.attendanceCheckbox ? 'Y' : 'N';
        obj.MOBILE = $scope.userSetupTree.mobileCheckbox ? 'Y' : 'N';
        obj.ACTIVE = $scope.userSetupTree.activeCheckbox ? 'Y' : 'N';
        obj.BRANDING = $scope.userSetupTree.brandingCheckbox ? 'Y' : 'N';
        obj.ALL_PLAN = $scope.userSetupTree.allVisitPlan ? 'Y' : 'N';
        obj.SUPER_USER = $scope.userSetupTree.superCheckbox ? 'Y' : 'N';
        obj.MGR_USER = $scope.userSetupTree.managerCheckbox ? 'Y' : 'N';
        obj.AREA = $scope.userSetupTree.AreaMultiSelect || [];
        obj.CUSTOMER = $scope.userSetupTree.CustomerMultiSelect || [];
        obj.GROUPID = $scope.userSetupTree.GROUPID;
        obj.BRAND = $scope.userSetupTree.BrandMultiSelect || [];
        obj.ITEMS = $("#distItemsSelect").data("kendoMultiSelect").value();

        return obj;
    }

    function saveNewUser(obj) {
        DistSetupService.AddUserTree(obj).then(
            function (result) {
                console.log("usertree", result);
                if (result.data.STATUS_CODE === 200) {
                    showNotification("Saved Successfully", "success");
                    refreshTreeList();
                    closeModal();
                    clearForm();
                } else if (result.data.STATUS_CODE === 300) {
                    showNotification(result.data.MESSAGE, "warning");
                } else {
                    displayBarNotification(result.data.MESSAGE, "error");
                }
            },
            function () {
                showNotification("Error", "error");
            }
        );
    }

    function updateExistingUser(obj) {
        DistSetupService.UpdateUserTree(obj).then(
            function (result) {
                if (result.data === "200") {
                    showNotification("Update Successfully", "success");
                    refreshTreeList();
                    closeModal();
                    clearForm();
                } else {
                    showNotification("Error", "error");
                    closeModal();
                }
            },
            function () {
                showNotification("Error", "error");
            }
        );
    }

    function refreshTreeList() {
        angular.element('#treelist').data("kendoTreeList").dataSource.read();
    }

    function closeModal() {
        angular.element('#userSetupTreeCreateModal').modal('hide');
    }

    // ========================================
    // TREE ORDER UPDATE
    // ========================================

    $scope.UpdateUserTreeOrder = function (e) {
        if (!e.valid) return;

        if (e.destination !== undefined) {
            e.source.GROUPID = e.destination.GROUPID;
        } else {
            e.source.GROUPID = null;
        }

        DistSetupService.UpdateUserTreeOrder(e.source);
    };

    // ========================================
    // EVENT HANDLERS
    // ========================================

    $("#treelist").on("mousedown", "tr[role='row']", function (e) {
        if (e.which === 3) { // Right click
            $('tr.k-state-selected', '#treelist').removeClass('k-state-selected');
            $(this).addClass("k-state-selected");
        }
    });

});