DTModule.controller('EmployeeSetupCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter, $timeout) {


    $scope.isUpdate = false;

    $scope.employeeModel = {
        EMPLOYEE_CODE: "",
        EMPLOYEE_EDESC: "",
        EMAIL: "",
        SEX: "",
        MOBILE: "",
        PAN_NO:""
    };

    
    // 1. Grid Configuration
    $scope.EmployeeGridOptions = {
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetEmployees",
                    dataType: "json",
                    type: "GET",
                    contentType: "application/json; charset=utf-8"
                }
            },
            // CLIENT-SIDE SETTINGS
            pageSize: 50,          // How many rows per page
            serverPaging: false,    // DO NOT ask server for pages
            serverFiltering: false, // Handle search locally
            serverSorting: false,   // Handle sorting locally

            schema: {
                data: function (response) {
                    // Fail-safe for different API return structures
                    if (response.hasOwnProperty("DATA")) return response.DATA;
                    if (response.hasOwnProperty("Data")) return response.Data;
                    return response;
                },
                model: {
                    id: "EMPLOYEE_CODE",
                    fields: {
                        EMPLOYEE_CODE: { type: "string" },
                        EMPLOYEE_EDESC: { type: "string" },
                        DEPARTMENT_EDESC: { type: "string" },
                        DESIGNATION_EDESC: { type: "string" },
                        MOBILE: { type: "string" },
                        EMAIL: { type: "string" },
                        EMPLOYEE_STATUS: { type: "string" }
                    }
                }
            },
            error: function (e) {
                if (window.displayPopupNotification)
                    displayPopupNotification("Error loading employee data", "error");
            }
        },
        // GRID UI SETTINGS
        sortable: true,
        reorderable: true,
        scrollable: true,
        resizable: true,
        pageable: {
            refresh: true,
            pageSizes: [ 10, 20,30,50, 100,"all"], // Optional: let user choose page size
            buttonCount: 5
        },
        columns: [
            { field: "EMPLOYEE_CODE", title: "Id", width: "70px" },
            { field: "EMPLOYEE_EDESC", title: "Employee Name", width: "150px" },
            { field: "DEPARTMENT_EDESC", title: "Department", width: "120px" },
            { field: "DESIGNATION_EDESC", title: "Designation", width: "120px" },
            { field: "MOBILE", title: "Mobile", width: "100px" },
            { field: "EMAIL", title: "Email", width: "120px" },
            { field: "EMPLOYEE_STATUS", title: "Status", width: "80px" },
            {
                title: "Action",
                width: "80px",
                headerAttributes: { style: "text-align: center;" },
                attributes: { style: "text-align: center; overflow: visible;" },
                template: `
                        <div style="display: flex; justify-content: center; gap: 15px;">
                            <a class='fa fa-edit' 
                               ng-click='editEmployee(dataItem)' 
                               title='Edit' 
                               style='cursor:pointer; color: \\#4da539; font-size: 14px;'
                               onmouseover='this.style.opacity="0.7"' 
                               onmouseout='this.style.opacity="1"'></a>
                            <a class='fa fa-trash' 
                               ng-click='DeleteEmployee(dataItem)'
                               title='Delete' 
                               style='cursor:pointer; color: \\#ed6b75; font-size: 14px;'
                               onmouseover='this.style.opacity="0.7"' 
                               onmouseout='this.style.opacity="1"'></a>
                        </div>`
            }
        ]
    };

    // 2. Search Logic
    // Using $scope.$apply is safer if called from outside events, 
    // but since this is ng-change, standard logic applies.
    $scope.onItemSearchChange = function () {
        var q = $scope.txtSearchString;
        var grid = $("#kGrid").data("kendoGrid");

        if (grid && grid.dataSource) {
            if (q && q.length > 0) {
                grid.dataSource.filter({
                    logic: "or",
                    filters: [
                        { field: "EMPLOYEE_EDESC", operator: "contains", value: q },
                        { field: "MOBILE", operator: "contains", value: q },
                        { field: "EMAIL", operator: "contains", value: q },
                        { field: "EMPLOYEE_STATUS", operator: "contains", value: q },
                        { field: "DEPARTMENT_EDESC", operator: "contains", value: q },
                        { field: "DESIGNATION_EDESC", operator: "contains", value: q },
                    ]
                });
            } else {
                grid.dataSource.filter({}); // Clear filters if search is empty
            }
        }
    };

    // 3. BindGrid Method (For the Search Icon)
    $scope.BindGrid = function () {
        var grid = $("#kGrid").data("kendoGrid");
        if (grid) {
            grid.dataSource.read(); // Re-fetches data from API
        }
    };



    $scope.DeleteEmployee = function (data) {
        if (!data) return;

        showUniversalPopup({
            title: "Delete Employee",
            message: `Are you sure you want to delete this employee? (<strong>${data.EMPLOYEE_EDESC}</strong>)`,
            confirmText: "Delete",
            confirmColor: "",
            onConfirm: function () {

                var employeeCode = data.EMPLOYEE_CODE;

                // Close modal first
                $scope.closeDeleteModal();

                // Call API
                $http({
                    method: 'POST',
                    url: window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteEmployee",
                    params: { employeeCode: employeeCode }
                }).then(function successCallback(response) {
                    if (response.data.DATA === "DELETED") {
                        displayPopupNotification("Employee deleted successfully", "success");
                        $scope.BindGrid();
                    } else {
                        alert("Error: " + response.data.MESSAGE);
                    }
                }, function errorCallback(response) {
                    console.error(response);
                    displayPopupNotification("Failed to delete Employee. Please try again.","error");
                });
            }
        });
    };

    
    $scope.activeTab = 'personalDetails';
    $scope.setActiveTab = function (tab) {
        $scope.activeTab = tab;
    };

    $scope.openModal = function () {
        $scope.activeTab = 'personalDetails';
        $scope.resetForm();
        $scope.isUpdate = false;
        $('#employeeModal').modal({
            backdrop: 'static',
            keyboard: false
        });

        $('#employeeModal').modal('show');
    }; 

    
    $scope.save = function () {
        if ($scope.employeeForm.$valid) {
            
            var apiUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/SaveEmployee";

            $http.post(apiUrl, $scope.employeeModel)
                .then(function (response) {
                    displayPopupNotification("Employee saved successfully!", "success");
                    $scope.resetForm();
                    $('#employeeModal').modal('hide');
                    $scope.BindGrid();
                })
                .catch(function (error) {
                    displayPopupNotification("Failed to save data. Please try again.", "error");
                });

        } else {           
            angular.forEach($scope.employeeForm, function (value, key) {
                if (key.indexOf('$') !== 0 && value.$setTouched) {
                    value.$setTouched();
                }
            });
        }
    };

    $scope.resetForm = function () {
        $scope.employee = {};
        if ($scope.employeeForm) {
            $scope.employeeForm.$setPristine();  
            $scope.employeeForm.$setUntouched(); 
        }
        $scope.employeeModel = {
            EMPLOYEE_EDESC: "",
            EMAIL: "",
            SEX: "",
            MOBILE: "",
            PAN_NO: ""
        };
    };

    $scope.closeModal = function () {
        $scope.resetForm();
        $('#employeeModal').modal('hide');

    }


    $scope.editEmployee = function (data) {
       
        $scope.employeeModel.EMPLOYEE_EDESC = data.EMPLOYEE_EDESC;
        $scope.employeeModel.SEX = data.SEX;
        $scope.employeeModel.EMAIL= data.EMAIL;
        $scope.employeeModel.MOBILE = data.MOBILE;
        $scope.employeeModel.PAN_NO = data.PAN_NO;
        $scope.employeeModel.EMPLOYEE_CODE = data.EMPLOYEE_CODE;
        $scope.isUpdate = true;
        $('#employeeModal').modal('show');
    }


    $scope.update = function () {
        if ($scope.employeeForm.$valid) {          
            var apiUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/SaveEmployee";
            $http.post(apiUrl, $scope.employeeModel)
                .then(function (response) {
                    displayPopupNotification("Employee Updated successfully!", "success");
                    $scope.resetForm();
                    $('#employeeModal').modal('hide');
                    $scope.BindGrid();
                })
                .catch(function (error) {
                    displayPopupNotification("Failed to update data. Please try again.", "error");
                });

        } else {
            angular.forEach($scope.employeeForm, function (value, key) {
                if (key.indexOf('$') !== 0 && value.$setTouched) {
                    value.$setTouched();
                }
            });
        }
    };
});