PMModule.controller('projectSetupTreeCtrl', function ($scope, ProjectSetupService, $rootScope, $route, $http, $filter, $timeout) {
    $scope.showProjectModal = function () {
        $('#projectModal').modal('show');
    };
    $scope.saveNewitem = function () {
            if (!$scope.PROJECT_NAME) {
            alert("Project Name is required.");
            return;
            } else if
             (!$scope.START_DATE) {
                alert("Start date is required.");
                return;
            } else if
                ($scope.END_DATE && new Date($scope.START_DATE) >= new Date($scope.END_DATE)) {
                alert("Start date should be less than end date.");
                return;
            }
        var formData = {
                ID: $scope.ID,
                PROJECT_NAME: $scope.PROJECT_NAME,
                START_DATE:formatDate($scope.START_DATE),
                END_DATE: $scope.END_DATE ? formatDate($scope.END_DATE) : null // Handle null end date
            };
            $http.post('/api/ProjectApi/SaveProjectFormData', formData)
                .then(function (response) {
                    window.location.reload(); // Reload the page
                    $("#projectModal").modal('hide');
                    var message = response.data.MESSAGE;
                    $scope.setPopoverContent(message, 'success');
                })
                .catch(function (error) {
                    var message = error;
                    $scope.setPopoverContent(message, 'error');
                });
        };
        function formatDate(dateString) {
            var date = new Date(dateString);
            return $filter('date')(date, 'dd-MMM-yyyy');
        }

        $scope.setPopoverContent = function (message, type) {
            // Set the message
            var popMessage = $('.popMessage');
            popMessage.html('<h5>' + message + '</h5>');

            // Set the background color based on the type
            if (type === 'success') {
                popMessage.css('background-color', 'green');
            } else if (type === 'error') {
                popMessage.css('background-color', 'red');
            }

            setTimeout(function () {
                $('.popContainer').hide();
            }, 4000);
        };

        $http.post('/api/ProjectApi/ListAllProjects')
            .then(function (response) {
                var projects = response.data; // Assuming the response contains an array of project objects
                projects.forEach(function (project) {
                    project.CREATED_DT = formatDate(project.CREATED_DT);
                    project.START_DATE = formatDate(project.START_DATE);
                    project.END_DATE = project.END_DATE ? formatDate(project.END_DATE) : null; // Handle null end date

                });
                $scope.dataSource.data(projects); // Set the data to the dataSource
            })
            .catch(function (error) {
                var message = error.data.message || 'Error loading projects!!!'; // Assuming the error response contains a message
                $scope.setPopoverContent(message, 'error');
            });

        // Define the dataSource for the Kendo Grid
        $scope.dataSource = new kendo.data.DataSource({
            data: [], // Initially empty
            pageSize:10// Optionally, set page size
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
                { field: "PROJECT_NAME", title: "Project Name" },
                { field: "START_DATE", title: "Start Date" },
                { field: "END_DATE", title: "End Date" },
                { field: "CREATED_DT", title: "Created Date" },
                {
                    title: "Actions",
                    width: 120,
                    template: "<a class='btn btn-sm btn-info view-btn' data-id='#= ID #'><i class='fa fa-eye'></i></a>&nbsp;<a class='btn btn-sm btn-warning edit-btn' data-id='#= ID #'><i class='fa fa-edit'></i></a>&nbsp;<a class='btn btn-sm btn-danger delete-btn' data-id='#= ID #'><i class='fa fa-trash'></i></a>"
                }
            ]
        });
        $("#kGrid").on("click", ".edit-btn", function () {
            var id = $(this).data("id");
            var responseList = ProjectSetupService.GetProjectById(id);
            responseList.then(function (response) {
                var project = response.data[0];
                $scope.ID = project.ID;
                $scope.PROJECT_NAME = project.PROJECT_NAME;
                $scope.START_DATE = new Date(project.START_DATE);
                //$scope.END_DATE = new Date(project.END_DATE);
                $scope.END_DATE = project.END_DATE ? new Date(project.END_DATE) : null; // Handle null end date

                // Use $timeout to run the code in the next digest cycle
                $timeout(function () {
                    $("#editProjectModal").modal('show');
                });
            }).catch(function (error) {
                var message = 'Error in displaying project!!'; // Extract message from response
                $scope.setPopoverContent(message, 'error');
            });
        });

        // Handle click event for the view button
        $("#kGrid").on("click", ".view-btn", function () {
            var id = $(this).data("id");
            var responseList = ProjectSetupService.GetProjectById(id);
            responseList.then(function (response) {
                var project = response.data[0];
                $scope.PROJECT_NAME = project.PROJECT_NAME;
                $scope.START_DATE = formatDate(project.START_DATE); // Convert string to Date object
                $scope.END_DATE = project.END_DATE ? formatDate(project.END_DATE) : null; // Handle null end date
                // Use $timeout to run the code in the next digest cycle
                $timeout(function () {
                    $("#viewProjectModal").modal('show');
                });
            }).catch(function (error) {
                var message = 'Error in displaying project!!'; // Extract message from response
                $scope.setPopoverContent(message, 'error');
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
                    $scope.setPopoverContent(message, 'success');
                    window.location.reload(); // Reload the page
                }).catch(function (error) {
                    var message = 'Error in displaying project!!'; // Extract message from response
                    //$('#popoverMessage').text(message);
                    $scope.setPopoverContent(message, 'error');
                });
                deleteButton.popover('hide');
            });

            // Handle click event on the "No" button
            $(document).on('click', '.cancel-delete', function () {
                // Hide the popover
                deleteButton.popover('hide');
            });

        });

        // Function to filter data
        $("#itemtxtSearchString").keyup(function () {
            var val = $(this).val().toLowerCase(); // Get the search input value
            $scope.dataSource.filter({
                logic: "or",
                filters: [
                    {
                        field: "PROJECT_NAME",
                        operator: "contains",
                        value: val
                    },
                    {
                        field: "CREATED_DT",
                        operator: "eq",
                        value: new Date(val) || null // Assuming the input value is in date format
                    }
                ]
            });
        });


    });

PMModule.service('ProjectSetupService', function ($http) {
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
});
