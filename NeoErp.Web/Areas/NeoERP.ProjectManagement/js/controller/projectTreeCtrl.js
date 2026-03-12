PMModule.controller('projectTreeCtrl', function ($scope, ProjectSetupService, $rootScope, $route, $http, $filter, $timeout) {
    var dataSource;
    $scope.itemList = [];
    $scope.counterMaterial = 0;
    $scope.counterLabour = 0;
    $scope.employeeList = [];
    $scope.contractorList = [];

    //Project 
    $scope.projectFormList = [];
    $scope.counterProject = 0;
    $scope.showProjectModal = function () {
        $("#projectModal").modal('show');
        if ($scope.projectFormList.length === 0) {
            $scope.addProject();
        }
        //window.location.href = "/ProjectManagement/Home/Index#!PM/AddProject"

    };
    //Material Model Form
    $scope.materialPlanningFormList = [];
    $scope.labourPlanningFormList = [];

    $scope.projectFormTemplate = {
        subProjectId: 0,
        SUB_PROJECTNAME: "",
        IMAGE_NAME: "",
        AREA: "",
        BUDGET_PLANNING: "",
        PROJECT_MANAGER: "",
        CONTRACTOR: "",
        START_DT: "",
        END_DT: "",
        STATUS: 'S',
        checkbox: "checkboxr0",
        checked: false
    };

    $scope.addProject = function () {
        var req = "/api/InventoryApi/GetAllEmployees"
        $http.get(req).then(function (results) {
            $scope.employeeList = results.data;
        })
        var contractorListPromise = ProjectSetupService.getAllSupplier();
        contractorListPromise.then(function (response) {
            $scope.contractorList = response.data;
        }, function () {
            $scope.contractorMessage = "Error while getting item list";
        });
        $scope.projectFormList.push(angular.copy({
            subProjectId: $scope.counterProject,
            SUB_PROJECTNAME: "",
            IMAGE_NAME: "",
            AREA: "",
            BUDGET_PLANNING: "",
            PROJECT_MANAGER: $scope.employeeList[0],
            CONTRACTOR: $scope.contractorList[0],
            START_DT: "",
            END_DT: "",
            STATUS: 'S',
            checkbox: "checkboxr" + $scope.counterProject,
            checked: false
        }));

        $scope.materialPlanningFormList.push(angular.copy({
            modelId: $scope.counterProject
        }));

        $scope['materialFormList' + $scope.counterProject] = [
            {
                Id: 0,
                DESCRIPTION: $scope.itemList[0],
                QUANTITY: 0,
                RATE: 0,
                AMOUNT: 0,
                checkbox: "checkboxr0",
                checked: false
            }
        ];
        $scope['counterMaterial' + $scope.counterProject] = 1;

        $scope.labourPlanningFormList.push(angular.copy({
            modelId: $scope.counterProject
        }));
        $scope['labourFormList' + $scope.counterProject] = [
            {
                Id: 0,
                description: "",
                QUANTITY: 0,
                RATE: 0,
                AMOUNT: 0,
                checkbox: "checkboxr0",
                checked: false
            }
        ];
        $scope['counterLabour' + $scope.counterProject] = 1;

        $scope.counterProject++;
    };

    $scope.deleteProject = function () {
        var tempE = 0;
        var lengthE = $scope.projectFormList.length;
        for (var i = 0; i < lengthE; i++) {
            if ($scope.projectFormList[i - tempE].checked) {
                var id = $scope.projectFormList[i - tempE].eRId;
                //if (id != 0) {
                //    window.app.pullDataById(document.deleteRelationDtlLink, {
                //        "id": id
                //    }).then(function (success) {
                //        $scope.$apply(function () {
                //            console.log(success.data);
                //        });
                //    }, function (failure) {
                //        console.log(failure);
                //    });
                //}
                $scope.projectFormList.splice(i - tempE, 1);
                tempE++;
            }
        }
    };
    //Imag


    //End of project
    //Material Planning
    $scope.openMaterialPlanningModal = function (subProjectId) {
        $("#materialPlanningModal_" + subProjectId).modal('show');
    //        $scope.addMaterial(subProjectId);
    };


    $scope.getMaterialFormList = function (modelId) {
        // Here, you can return the corresponding materialFormList based on modelId
        return $scope['materialFormList' + modelId];
    };
    $scope.addMaterial = function (idAy) {
        var itemListPromise = ProjectSetupService.GetAllItemsName();
        itemListPromise.then(function (response) {
            $scope.itemList = response.data;

            // Push new material into materialFormList after item list is fetched
            $scope['materialFormList' + idAy].push(angular.copy({
                id: $scope['counterMaterial' + idAy],
                itemId: $scope.itemList[0],
                QUANTITY: 0,
                RATE: 0,
                AMOUNT: 0,
                checkbox: "checkboxr" + $scope.counterMaterial,
                checked: false
            }));
            $scope['counterMaterial' + idAy]++;
        }, function () {
            $scope.ItemMessage = "Error while getting item list";
        });
    };

    $scope.materialFormList = [];
    $scope.materialFormTemplate = {
        id: 0,
        DESCRIPTION: $scope.itemList[0],
        QUANTITY: 0,
        RATE: 0,
        AMOUNT: 0,
        checkbox: "checkboxr0",
        checked: false
    };

    $scope.deleteMaterial = function (idAy) {
        var tempE = 0;
        var lengthE = $scope['materialFormList' + idAy].length;
        for (var i = 0; i < lengthE; i++) {
            if ($scope['materialFormList' + idAy][i - tempE].checked) {
                var id = $scope['materialFormList' + idAy][i - tempE].eRId;
                // If you want to perform some action before deleting, uncomment the following lines
                // if (id != 0) {
                //     window.app.pullDataById(document.deleteRelationDtlLink, {
                //         "id": id
                //     }).then(function (success) {
                //         $scope.$apply(function () {
                //             console.log(success.data);
                //         });
                //     }, function (failure) {
                //         console.log(failure);
                //     });
                // }
                $scope['materialFormList' + idAy].splice(i - tempE, 1);
                tempE++;
            }
        }
    };
    $scope.calculateAmount = function (material) {
        material.AMOUNT = (material.QUANTITY || 0) * (material.RATE || 0);
    };
    $scope.materialFormLists = [];
    $scope.submitMaterialPlanning = function (modelId) {
        $scope.materialPlanningData = angular.copy($scope['materialFormList' + modelId]);
        $scope.materialFormLists[modelId] = $scope.materialPlanningData;
        $("#materialPlanningModal_" + modelId).modal('hide');
    };
    //End of material planning

    //Labour Planning
    $scope.openLabourPlanningModal = function (subProjectId) {
        //$scope['labourFormList' + subProjectId] = [];
        //$scope.addLabour(subProjectId);
        $("#labourPlanningModal_" + subProjectId).modal('show');
    };
    $scope.getLabourFormList = function (modelId) {
        // Here, you can return the corresponding materialFormList based on modelId
        return $scope['labourFormList' + modelId];
    };
    $scope.labourFormList = [];
    $scope.labourFormTemplate = {
        Id: 0,
        description: "",
        QUANTITY: 0,
        RATE: 0,
        AMOUNT: 0,
        checkbox: "checkboxr0",
        checked: false
    };


    $scope.addLabour = function (idAy) {
        // Push new material into materialFormList after item list is fetched
        $scope['labourFormList' + idAy].push(angular.copy({
            Id: $scope['counterLabour' + idAy],
            description: "",
            QUANTITY: 0,
            RATE: 0,
            AMOUNT: 0,
            checkbox: "checkboxr" + $scope.counterLabour,
            checked: false
        }));
        $scope['counterLabour' + idAy]++;

    };

    $scope.deleteLabour = function (idAy) {
        var tempE = 0;
        var lengthE = $scope['labourFormList' + idAy].length;
        for (var i = 0; i < lengthE; i++) {
            if ($scope['labourFormList' + idAy][i - tempE].checked) {
                var id = $scope['labourFormList' + idAy][i - tempE].eRId;
                // If you want to perform some action before deleting, uncomment the following lines
                // if (id != 0) {
                //     window.app.pullDataById(document.deleteRelationDtlLink, {
                //         "id": id
                //     }).then(function (success) {
                //         $scope.$apply(function () {
                //             console.log(success.data);
                //         });
                //     }, function (failure) {
                //         console.log(failure);
                //     });
                // }
                $scope['labourFormList' + idAy].splice(i - tempE, 1);
                tempE++;
            }
        }
    };
    $scope.updateAmount = function (labour) {
        // Calculate the amount based on the rate and quantity
        labour.AMOUNT = (labour.RATE || 0) * (labour.QUANTITY || 0);
    };
    $scope.labourFormLists = [];
    $scope.submitLabourPlanning = function (modelId) {
        $scope.labourPlanningData = angular.copy($scope['labourFormList' + modelId]);
        $scope.labourFormLists[modelId] = $scope.labourPlanningData;
        $("#labourPlanningModal_" + modelId).modal('hide');
    };

    $scope.saveNewitem = function () {
        var projectName = $scope.project.PROJECT_EDESC;
        var formData = {
            PROJECT_NAME: projectName,
            subProjects: []
        };

        angular.forEach($scope.projectFormList, function (subProject) {
            var subProjectId = subProject.subProjectId;
            var fullPath = $('#image_' + subProjectId).val();
            var filename = fullPath.split('\\').pop();
            var subProjectData = {
                SubProjectId: subProject.subProjectId,
                SUB_PROJECTNAME: subProject.SUB_PROJECTNAME,
                IMAGE_NAME: filename,
                //IMAGE_PATH: $('#image_0')[0].files,
                AREA: subProject.AREA,
                BUDGET_PLANNING: subProject.BUDGET_PLANNING,
                STATUS: subProject.STATUS,
                PROJECT_MANAGER: (subProject.PROJECT_MANAGER == undefined ? null : subProject.PROJECT_MANAGER['EMPLOYEE_CODE']),
                CONTRACTOR: (subProject.CONTRACTOR == undefined ? null : subProject.CONTRACTOR['SUPPLIER_CODE']),
                START_DT: formatDate(subProject.START_DT),
                END_DT: formatDate(subProject.END_DT),
                checkbox: subProject.checkbox,
                checked: subProject.checked,
                materialPlanningData: transformMaterialPlanningData(subProjectId) || [],
                labourPlanningData: $scope.labourFormLists[subProjectId] || []
            };
            formData.subProjects.push(subProjectData);
        });
        $http.post('/api/ProjectApi/SaveProjectFormData', formData)
            .then(function (response) {
                $scope.projectFormList.splice(0, $scope.projectFormList.length);
                window.location.reload();
                $scope.ItemCancel();
                $("#projectModal").modal('hide');
                var message = response.data.MESSAGE;
                displayPopupNotification(message, 'success');
            })
            .catch(function (error) {
                var message = error;
                displayPopupNotification(message, 'error');
            });
    };
    function transformMaterialPlanningData(subProjectId) {
        var materialPlanningData = [];
        angular.forEach($scope.materialFormLists[subProjectId] || [], function (materialItem) {
            var transformedItem = {
                DESCRIPTION: (materialItem.DESCRIPTION == undefined ? null : materialItem.DESCRIPTION['ITEM_CODE']),
                QUANTITY: materialItem.QUANTITY,
                RATE: materialItem.RATE,
                AMOUNT: materialItem.AMOUNT
            };
            materialPlanningData.push(transformedItem);
        });
        return materialPlanningData;
    }
    function formatDate(dateString) {
        var date = new Date(dateString);
        return $filter('date')(date, 'dd-MMM-yyyy');
    }

    $http.post('/api/ProjectApi/ListAllProjects')
        .then(function (response) {
            var projects = response.data; // Assuming the response contains an array of project objects
            projects.forEach(function (project) {
                project.CREATED_DT = formatDate(project.CREATED_DT);
                project.TOTAL_BUDGET = (project.TOTAL_BUDGET).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                project.START_DATE = formatDate(project.START_DATE);
                project.END_DATE = formatDate(project.END_DATE);
            });
            $scope.dataSource.data(projects); // Set the data to the dataSource
        })
        .catch(function (error) {
            var message = error.data.message || 'Error loading projects!!!'; // Assuming the error response contains a message
            displayPopupNotification(message, 'error');
        });

    // Define the dataSource for the Kendo Grid
    $scope.dataSource = new kendo.data.DataSource({
        data: [], // Initially empty
        pageSize: 10// Optionally, set page size
    });

    // Configure the Kendo Grid with the dataSource
    $("#kGrid").kendoGrid({
        dataSource: $scope.dataSource,
        height: 400,
        sortable: true,
        pageable: {
            refresh: true,
            pageSizes: true
        },
        resizable: true, // Enable column resizing
        toolbar: ["excel"/*, "pdf"*/],
        excel: {
            fileName: "Projects.xlsx",
            allPages: true
        },
        //pdf: {
        //    fileName: "Projects.pdf",
        //    allPages: true
        //},
        columns: [
            { field: "PROJECT_NAME", title: "Project Name", type: "string" },
            { field: "SUB_PROJECT_COUNT", title: "Total SubProject", style: "text-align:right", template: "<div style='text-align:right;'>#= SUB_PROJECT_COUNT #</div>" },
            { field: "TOTAL_AREA", title: "Total Area", style: "text-align:right", type: "number", template: "<div style='text-align:right;'>#= TOTAL_AREA #</div>" },
            { field: "TOTAL_BUDGET", title: "Total Budget", style: "text-align:right", type: "number", template: "<div style='text-align:right;'>#= kendo.toString(TOTAL_BUDGET, 'n2') #</div>" },
            { field: "LABOUR_COUNT", title: "Total Labour Planning", style: "text-align:right", type: "number", template: "<div style='text-align:right;'>#= LABOUR_COUNT #</div>" },
            { field: "MATERIAL_COUNT", title: "Total Material Planning", style: "text-align:right", type: "number", template: "<div style='text-align:right;'>#= MATERIAL_COUNT #</div>" },
            { field: "START_DATE", title: "Start Date", type: "string" },
            { field: "END_DATE", title: "End Date", type: "string" },
            { field: "CREATED_DT", title: "Created Date", type: "string" },
            {
                title: "Actions",
                width: 120,
                template: "<a class='btn btn-sm btn-info view-btn' data-id='#= ID #'><i class='fa fa-eye'></i></a>&nbsp;<a class='btn btn-sm btn-warning edit-btn' data-id='#= ID #'><i class='fa fa-edit'></i></a>&nbsp;<a class='btn btn-sm btn-danger delete-btn' data-id='#= ID #'><i class='fa fa-trash'></i></a>"
            }
        ]

    });
    // Handle click event for the edit button
    $("#kGrid").on("click", ".edit-btn", function () {
        var id = $(this).data("id");
        var req = "/api/ProjectApi/GetAllEmployees"
        $http.get(req).then(function (results) {
            $scope.employeeList = results.data;
        })
        var contractorListPromise = ProjectSetupService.getAllSupplier();
        contractorListPromise.then(function (response) {
            $scope.contractorList = response.data;
        }, function () {
            $scope.contractorMessage = "Error while getting item list";
        });
        var responseList = ProjectSetupService.GetProjectById(id);
        responseList.then(function (response) {
            var project = response.data[0];
            $scope.viewMaterialData = project;

            $("#projectDesc").val(project.PROJECT_NAME);
            $scope.projectFormList = [];
            for (var i = 0; i < project.SubProjects.length; i++) {
                var subProject = project.SubProjects[i];
                subProject.START_DT = formatDate(subProject.START_DT);
                subProject.END_DT = formatDate(subProject.END_DT);
                subProject.MATERIAL_PLANNING = subProject.MaterialPlanningData.length ? 'Planned' : 'N/A';
                subProject.LABOUR_PLANNING = subProject.LabourPlanningData.length ? 'Planned' : 'N/A';
                $scope.projectFormList.push(angular.copy({
                    project: subProject
                }));

            }
            $scope.selectedProjectManager = project.PROJECT_MANAGER;
            $scope.selectedContractor = project.CONTRACTOR;
            // Use $timeout to run the code in the next digest cycle
            $timeout(function () {
                $("#editProjectModal").modal('show');
            });
            $scope.materialPlanningFormList = [];
            $scope.viewMaterialPlanningModal = function (subProjectId) {
                for (var i = 0; i < project.SubProjects.length; i++) {
                    var subProject = project.SubProjects.find(function (subProj) {
                        return subProj.SubProjectId === subProjectId;
                    });
                }
                $scope.materialPlanningFormList = [];
                if (subProject) {
                    for (var i = 0; i < subProject.MaterialPlanningData.length; i++) {
                        var materialData = subProject.MaterialPlanningData[i];
                        $scope.materialPlanningFormList.push(angular.copy({ data: materialData }));
                    }
                }
                var itemListPromise = ProjectSetupService.GetAllItemsName();
                itemListPromise.then(function (response) {
                    $scope.itemList = response.data;
                }.then(function (results) {
                }));
                $("#editMaterialModal").modal('show');
            };
            $scope.labourPlanningFormList = [];
            $scope.viewLabourPlanningModal = function (subProjectId) {
                for (var i = 0; i < project.SubProjects.length; i++) {
                    var subProject = project.SubProjects.find(function (subProj) {
                        return subProj.SubProjectId === subProjectId;
                    });
                }
                $scope.labourPlanningFormList = [];
                if (subProject) {
                    for (var i = 0; i < subProject.LabourPlanningData.length; i++) {
                        var labourData = subProject.LabourPlanningData[i];
                        $scope.labourPlanningFormList.push(angular.copy({ data: labourData }));
                    }
                }
                $("#editLabourModal").modal('show');
            };
        }).catch(function (error) {
            var message = 'Error in displaying project!!'; // Extract message from response
            displayPopupNotification(message, 'error');
        });
    });

    // Handle click event for the view button
    $("#kGrid").on("click", ".view-btn", function () {
        var id = $(this).data("id");
        var responseList = ProjectSetupService.GetProjectById(id);
        responseList.then(function (response) {
            var project = response.data[0];
            $scope.viewMaterialData = project;

            $("#projectName").val(project.PROJECT_NAME);
            $scope.projectFormList = [];
            for (var i = 0; i < project.SubProjects.length; i++) {
                var subProject = project.SubProjects[i];
                console.log(subProject);
                subProject.START_DT = formatDate(subProject.START_DT);
                subProject.END_DT = formatDate(subProject.END_DT);
                subProject.MATERIAL_PLANNING = subProject.MaterialPlanningData.length ? 'Planned' : 'N/A';
                subProject.LABOUR_PLANNING = subProject.LabourPlanningData.length ? 'Planned' : 'N/A';
                if (subProject.STATUS === 'R') {
                    subProject.STATUS = 'Running';
                } else if (subProject.STATUS === 'C') {
                    subProject.STATUS = 'Completed';
                } else if (subProject.STATUS === 'H') {
                    subProject.STATUS = 'on Hold';
                } else {
                    subProject.STATUS = 'Select';
                }
                $scope.projectFormList.push(angular.copy({
                    project: subProject
                }));

            }

            // Use $timeout to run the code in the next digest cycle
            $timeout(function () {
                $("#viewProjectModal").modal('show');
            });
            $scope.materialPlanningFormList = [];
            $scope.viewMaterialPlanningModal = function (subProjectId) {
                for (var i = 0; i < project.SubProjects.length; i++) {
                    var subProject = project.SubProjects.find(function (subProj) {
                        return subProj.SubProjectId === subProjectId;
                    });
                }
                $scope.materialPlanningFormList = [];
                if (subProject) {
                    for (var i = 0; i < subProject.MaterialPlanningData.length; i++) {
                        var materialData = subProject.MaterialPlanningData[i];
                        $scope.materialPlanningFormList.push(angular.copy({ data: materialData }));
                    }
                }
                $("#viewMaterialModal").modal('show');
            };
            $scope.labourPlanningFormList = [];
            $scope.viewLabourPlanningModal = function (subProjectId) {
                for (var i = 0; i < project.SubProjects.length; i++) {
                    var subProject = project.SubProjects.find(function (subProj) {
                        return subProj.SubProjectId === subProjectId;
                    });
                }
                $scope.labourPlanningFormList = [];
                if (subProject) {
                    for (var i = 0; i < subProject.LabourPlanningData.length; i++) {
                        var labourData = subProject.LabourPlanningData[i];
                        $scope.labourPlanningFormList.push(angular.copy({ data: labourData }));
                    }
                }
                $("#viewLabourModal").modal('show');
            };
        }).catch(function (error) {
            var message = 'Error in displaying project!!'; // Extract message from response
            displayPopupNotification(message, 'error');
        });
    });

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
            var responseList = ProjectSetupService.DeleteProjectById(id);
            responseList.then(function (response) {
                var message = response.data.MESSAGE; // Extract message from response
                displayPopupNotification(message, 'success');
                window.location.reload(); // Reload the page
            }).catch(function (error) {
                var message = 'Error in displaying project!!'; // Extract message from response
                //$('#popoverMessage').text(message);
                displayPopupNotification(message, 'error');
            });
            deleteButton.popover('hide');
        });

        // Handle click event on the "No" button
        $(document).on('click', '.cancel-delete', function () {
            // Hide the popover
            deleteButton.popover('hide');
        });

    });

    
    $("#itemtxtSearchString").keyup(function () {
        var val = $(this).val().toLowerCase(); // Get the search input value
        var filters = [];
        var columns = $("#kGrid").data("kendoGrid").columns;
        for (var i = 0; i < columns.length; i++) {
            var column = columns[i];
            var field = column.field;
            if (column.type === "string") {
                filters.push({
                    field: field,
                    operator: "contains",
                    value: val
                });
            } else if (column.type === "number") {
                filters.push({
                    field: field,
                    operator: "eq",
                    value: parseFloat(val) || null
                });
            } else if (column.type === "date") {
                // Assuming you have a parsedDate variable defined somewhere in your code
                if (parsedDate) {
                    filters.push({
                        field: field,
                        operator: "eq",
                        value: new Date(val) || null
                    });
                }
            }
        }
        $scope.dataSource.filter({
            logic: "or",
            filters: filters
        });
    });
    $scope.ItemCancel = function () {
        $scope.project = {}; // Clear the main project object
        $scope.projectFormList = []; // Clear the subprojects list
        $scope.materialFormLists = {}; // Clear the material planning data
        $scope.labourFormLists = {}; // Clear the labour planning data
        // You can reset other variables here as needed
    };


});

PMModule.service('ProjectSetupService', function ($http) {

    this.GetAllItemsName = function () {

        var itemResponse = $http({
            method: "GET",
            url: "/api/PriceSetupApi/GetAllItemWithName",
            dataType: "json"
        });
        return itemResponse;
    }
    this.GetProjectById = function (id) {

        var itemResponse = $http({
            method: "GET",
            url: "/api/ProjectApi/GetProjectById?id=" + id,
            dataType: "json"
        });
        return itemResponse;
    }
    this.DeleteProjectById = function (id) {

        var itemResponse = $http({
            method: "POST",
            url: "/api/ProjectApi/DeleteProjectById?id=" + id,
            dataType: "json"
        });
        return itemResponse;
    }
    this.getAllSupplier = function () {
        var allS = $http({
            method: "GET",
            url: "/api/CustomFormApi/GetAllSupplier",
            dataType: "JSON"
        });
        return allS;
    };
});
