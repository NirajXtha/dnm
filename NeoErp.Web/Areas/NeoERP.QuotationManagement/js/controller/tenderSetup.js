QMModule.controller('tenderSetup', function ($scope, $rootScope, $http, $filter, $timeout) {

    $scope.tenderData = [];
    $scope.AddTender = function () {
        $scope.panelMode = 'new';
        $scope.ID = null;
        $scope.FORM_EDESC = '';
        $scope.FORM_NDESC = '';
        $scope.PREFIX = '';
        $scope.SUFFIX = '';
        $scope.BODY_LENGTH = '';
        $('#tenderModal').modal('show');
    }
    $scope.ORIGINAL_PREFIX = "";
    $scope.ORIGINAL_SUFFIX = "";
    $scope.saveTender = function () {
        var duplicate = $scope.tenderData.some(function (tender) {
            if ($scope.PREFIX !== $scope.ORIGINAL_PREFIX || $scope.SUFFIX !== $scope.ORIGINAL_SUFFIX) {
                if (tender.PREFIX === $scope.PREFIX &&
                    tender.SUFFIX === $scope.SUFFIX) {
                    return true;
                }
            }
                
        });

        if (duplicate) {
            displayPopupNotification("Quotation with similar prefix and suffix exists", "warning");
            return;
        }

        if (!$scope.FORM_EDESC || $scope.FORM_EDESC == '' ||
            !$scope.FORM_NDESC || $scope.FORM_NDESC == '' ||
        !$scope.PREFIX || $scope.PREFIX == '' ||
        !$scope.SUFFIX || $scope.SUFFIX == '' ||
        !$scope.BODY_LENGTH || $scope.BODY_LENGTH == ''
        ) {
            displayPopupNotification("Please enter all the fields!", "warning");
            return;
        }
        var formData = {
            ID: $scope.ID,
            FORM_EDESC: $scope.FORM_EDESC,
            FORM_NDESC: $scope.FORM_NDESC,
            PREFIX: $scope.PREFIX,
            SUFFIX: $scope.SUFFIX,
            BODY_LENGTH: $scope.BODY_LENGTH
        }
        $http.post('/api/QuotationApi/saveTender', formData)
            .then(function (response) {
                var message = response.data.message;
                displayPopupNotification(message, "success");
                setTimeout(function () {
                    window.location.reload();
                },5000)
            })
            .catch(function (error) {
                displayPopupNotification(error,"error");
            })
    }

    $http.get('/api/QuotationApi/TenderDetails')
        .then(function (response) {
            var tenders = response.data;
            $scope.tendersExist = tenders && tenders.length > 0;
            if ($scope.tendersExist) {
                $scope.dataSource.data(tenders);
                $scope.tenderData = tenders;
                console.log($scope.tenderData);
            }
        })
        .catch(function (error) {
            displayPopupNotification(error, "error");
        })
    $scope.dataSource = new kendo.data.DataSource({
        data: [], // Initially empty
    });
    $("#kGrid").kendoGrid({
        dataSource: $scope.dataSource,
        height: 400,
        sortable: true, // Enable sorting
        pageable: {
            refresh: true,
            pageSizes: true
        },
        toolbar: ["excel"/*, "pdf"*/],
        excel: {
            fileName: "Tender Details.xlsx",
            allPages: true
        },
        resizable: true, // Enable column resizing
        columns: [
            { field: "ID", title: "S.N", width: 50, type: "string" },
            { field: "FORM_EDESC", title: "Quotation Name", width: 250, type: "string" },
            { field: "PREFIX", title: "Prefix Text", width: 200, type: "string" },
            {
                field: "SUFFIX", title: "Suffix Text", width: 200, type: "string",
            },
            {
                field: "BODY_LENGTH", title: "Body Length", width: 80, type: "string",
            },
            {
                field: "CREATED_DATE", title: "Created Date", width: 150, type: "string",
                template: "#=kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy') #",

            },
            {
                title: "Actions",
                width: 120,
                template: "<a class='btn btn-sm btn-info view-btn' data-id='#= ID #'><i class='fa fa-eye'></i></a>&nbsp;<a class='btn btn-sm btn-warning edit-btn' data-id='#= ID #'><i class='fa fa-edit'></i></a>&nbsp;<a class='btn btn-sm btn-danger delete-btn' data-id='#= ID #'><i class='fa fa-trash'></i></a>"
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
            $http.post('/api/QuotationApi/deleteTenderId?tenderNo=' + id)
                .then(function (response) {
                    var message = response.data.MESSAGE;
                    displayPopupNotification(message, "success");
                    setTimeout(function () {
                        window.location.reload();
                    }, 5000)
                }).catch(function (error) {
                    var message = 'Error in displaying tender no!!'; // Extract message from response
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
    $("#kGrid").on("click", ".view-btn", function () {
        var viewBtn = $(this);
        var id = $(this).data("id");
        $http.get('/api/QuotationApi/getTenderById?tenderNo=' + id)
            .then(function (response) {
                $scope.panelMode = 'view';
                var tenders = response.data[0];
                $scope.FORM_EDESC = tenders.FORM_EDESC;
                $scope.FORM_NDESC = tenders.FORM_NDESC;
                $scope.PREFIX = tenders.PREFIX;
                $scope.SUFFIX = tenders.SUFFIX;
                $scope.BODY_LENGTH = tenders.BODY_LENGTH;
                $('#tenderModal').modal('show');
            }).catch(function (error) {
                var message = 'Error in displaying tender no!!'; 
                displayPopupNotification(message, "error");
            })
    })
    $("#kGrid").on("click", ".edit-btn", function () {
        var editBtn = $(this);
        var id = $(this).data("id");
        $http.get('/api/QuotationApi/getTenderById?tenderNo=' + id)
            .then(function (response) {
                var tenders = response.data[0];
                $scope.panelMode = 'edit';
                $scope.ID = tenders.ID;
                $scope.FORM_EDESC = tenders.FORM_EDESC;
                $scope.FORM_NDESC = tenders.FORM_NDESC;
                $scope.PREFIX = tenders.PREFIX;
                $scope.SUFFIX = tenders.SUFFIX;
                $scope.ORIGINAL_PREFIX = tenders.PREFIX;
                $scope.ORIGINAL_SUFFIX = tenders.SUFFIX;
                $scope.BODY_LENGTH = tenders.BODY_LENGTH;
                $('#tenderModal').modal('show');
            }).catch(function (error) {
                var message = 'Error in displaying tender no!!';
                displayPopupNotification(message, "error");
            })
    })
    //edit button 

    $scope.ItemCancel = function () {
        window.location.reload(); 
    }
});

QMModule.controller('valueSetup', function ($scope, $rootScope, $http, $filter, $timeout) {

    $scope.initGrid = function (isEditable) {
        var grid = $("#kGrid").kendoGrid({
            dataSource: {
                transport: {
                    read: {
                        url: "/api/QuotationApi/getUserValue",
                        dataType: "json",
                        error: function (e) {
                            console.error("DataSource read error:", e);
                            alert("Failed to load data.");
                        }
                    },
                    update: {
                        url: "/api/QuotationApi/setUserValue",
                        type: "POST",
                        dataType: "json",
                        complete: function (xhr) {
                            let response = {};
                            try {
                                response = JSON.parse(xhr.responseText);
                            } catch (e) {
                                console.error("Failed to parse response JSON:", e);
                            }

                            if (response.success) {
                                displayPopupNotification("Set Approval Limit Successfully", "success");
                            } else {
                                displayPopupNotification("Failed To Set The Approval Limit", "error");
                            }
                        },
                        error: function (xhr, status, e) {
                            console.error("Update error:", e);
                            displayPopupNotification("Failed To Set The Approval Limit", "error");
                        }
                    }
                },
                parameterMap: function (data, type) {
                    if (type === "update") {
                        var limit = parseFloat(data.QUOTATION_APPROVAL_LIMIT);
                        return {
                            ID: data.ID,
                            Employee_Name: data.Employee_Name,
                            QUOTATION_APPROVAL_LIMIT: isNaN(limit) ? null : limit
                        };
                    }
                    return data;
                },
                schema: {
                    parse: function (response) {
                        const items = Array.isArray(response) ? response : response.data || [];
                        return items.map(function (item) {
                            let limit = parseFloat(item.QUOTATION_APPROVAL_LIMIT);
                            item.QUOTATION_APPROVAL_LIMIT = isNaN(limit) ? null : limit;
                            return item;
                        });
                    },
                    model: {
                        id: "ID",
                        fields: {
                            ID: { type: "number", editable: false },
                            Employee_Name: { type: "string", editable: false },
                            EMPLOYEE_EDESC: { type: "string", editable: false },
                            QUOTATION_APPROVAL_LIMIT: { type: "number", editable: true, nullable: true }
                        }
                    }
                }
            },
            height: 400,
            editable: isEditable ? "inline" : false,
            sortable: true,
            pageable: false,
            columns: [
                {
                    title: "S.No",
                    width: "60px"
                },
                {
                    field: "ID",
                    hidden: true
                },
                {
                    field: "EMPLOYEE_EDESC",
                    title: "Employee Name",
                    width: "250px"
                },
                {
                    field: "QUOTATION_APPROVAL_LIMIT",
                    title: "Amount Limit",
                    width: "150px",
                    editor: numericEditor,
                    template: "#= QUOTATION_APPROVAL_LIMIT != null ? QUOTATION_APPROVAL_LIMIT.toFixed(2) : '0' #"
                },
                {
                    command: isEditable ? ["edit"] : [],
                    title: "Action",
                    width: "80px"
                }
            ],
            dataBound: function () {
                var grid = this;
                var items = grid.items();
                var rowNumber = 1;

                $(items).each(function () {
                    var row = $(this);
                    row.find("td:first").html(rowNumber);
                    rowNumber++;
                });
            }
        }).data("kendoGrid");

        $scope.$watch('txtSearchString', function (newVal) {
            if (grid) {
                grid.dataSource.filter({
                    field: "EMPLOYEE_EDESC",
                    operator: "contains",
                    value: newVal || ""
                });
            }
        });
    };

    function numericEditor(container, options) {
        $('<input name="' + options.field + '" autocomplete="off"/>')
            .appendTo(container)
            .kendoNumericTextBox({
                format: "n2",
                min: 0
            });
    }

    $http.get("/api/quotationapi/UserTypeToSetValue")
        .then(function (response) {
            var canEdit = response.data && response.data.success === true;
            $scope.initGrid(canEdit);
        })
        .catch(function (error) {
            console.error("Failed to check user type permission:", error);
            $scope.initGrid(false);
        });
});

QMModule.controller('userAccess', function ($scope, $rootScope, $http, $filter, $timeout) {

    $scope.initGrid = function (isEditable) {
        var grid = $("#kGrid").kendoGrid({
            dataSource: {
                transport: {
                    read: {
                        url: "/api/QuotationApi/getUserValue",
                        dataType: "json",
                        error: function (e) {
                            console.error("DataSource read error:", e);
                            alert("Failed to load data.");
                        }
                    },
                    update: {
                        url: "/api/QuotationApi/setUserAccess",
                        type: "POST",
                        dataType: "json",
                        complete: function (xhr) {
                            let response = {};
                            try {
                                response = JSON.parse(xhr.responseText);
                            } catch (e) {
                                console.error("Failed to parse response JSON:", e);
                            }

                            if (response.success) {
                                displayPopupNotification("Set Approval Limit Successfully", "success");
                            } else {
                                displayPopupNotification("Failed To Set The Approval Limit", "error");
                            }
                        },
                        error: function (xhr, status, e) {
                            console.error("Update error:", e);
                            displayPopupNotification("Failed To Set The User Access", "error");
                        }
                    }
                },
                parameterMap: function (data, type) {
                    if (type === "update") {
                        var limit = parseFloat(data.QUOTATION_APPROVAL_LIMIT);
                        return {
                            ID: data.ID,
                            Employee_Name: data.Employee_Name
                        };
                    }
                    return data;
                },
                schema: {
                    parse: function (response) {
                        const items = Array.isArray(response) ? response : response.data || [];
                        return items.map(function (item) {
                            let limit = parseFloat(item.QUOTATION_APPROVAL_LIMIT);
                            item.QUOTATION_APPROVAL_LIMIT = isNaN(limit) ? null : limit;
                            return item;
                        });
                    },
                    model: {
                        id: "ID",
                        fields: {
                            ID: { type: "number", editable: false },
                            Employee_Name: { type: "string", editable: false },
                            EMPLOYEE_EDESC: { type: "string", editable: false },
                            QUOTATION_APPROVAL_LIMIT: { type: "number", editable: true, nullable: true }
                        }
                    }
                }
            },
            height: 400,
            editable: isEditable ? "inline" : false,
            sortable: true,
            pageable: false,
            columns: [
                {
                    title: "S.No",
                    width: "60px"
                },
                {
                    field: "ID",
                    hidden: true
                },
                {
                    field: "EMPLOYEE_EDESC",
                    title: "Employee Name",
                    width: "250px"
                },
                {
                    field: "CHECK_FLAG",
                    title: "Can Check",
                    width: "150px",
                    template: function (dataItem) {
                        return toggleSwitchTemplate(dataItem.ID, "CHECK_FLAG", dataItem.CHECK_FLAG);
                    }
                },
                {
                    field: "VERIFY_FLAG",
                    title: "Can Verify",
                    width: "150px",
                    template: function (dataItem) {
                        return toggleSwitchTemplate(dataItem.ID, "VERIFY_FLAG", dataItem.VERIFY_FLAG);
                    }
                },
                {
                    field: "RECOMMEND_FLAG",
                    title: "Can Recommend",
                    width: "150px",
                    template: function (dataItem) {
                        return toggleSwitchTemplate(dataItem.ID, "RECOMMEND_FLAG", dataItem.RECOMMEND_FLAG);
                    }
                },
                {
                    field: "APPROVE_FLAG",
                    title: "Can Approve",
                    width: "150px",
                    template: function (dataItem) {
                        return toggleSwitchTemplate(dataItem.ID, "APPROVE_FLAG", dataItem.APPROVE_FLAG);
                    }
                }
                //,{
                //    field: "POST_FLAG",
                //    title: "Can Post",
                //    width: "150px",
                //    template: function (dataItem) {
                //        return toggleSwitchTemplate(dataItem.ID, "POST_FLAG", dataItem.POST_FLAG);
                //    }
                //}
            ],
            dataBound: function () {
                var grid = this;
                var items = grid.items();
                var rowNumber = 1;

                $(items).each(function () {
                    var row = $(this);
                    row.find("td:first").html(rowNumber);
                    rowNumber++;
                });
            }
        }).data("kendoGrid");

        $scope.$watch('txtSearchString', function (newVal) {
            if (grid) {
                grid.dataSource.filter({
                    field: "EMPLOYEE_EDESC",
                    operator: "contains",
                    value: newVal || ""
                });
            }
        });
    };

    function toggleSwitchTemplate(id, field, flagValue) {
        var isChecked = flagValue === 'Y' ? 'checked' : '';
        return `
        <label class="switch">
            <input type="checkbox" ${isChecked} onchange="angular.element(this).scope().toggleFlag(${id}, '${field}', this.checked)">
            <span class="slider round"></span>
        </label>
    `;
    }

    $scope.toggleFlag = function (id, field, isChecked) {
        var grid = $("#kGrid").data("kendoGrid");
        var dataItem = grid.dataSource.get(id);

        if (!dataItem) return;

        // Update the toggled flag
        dataItem.set(field, isChecked ? 'Y' : 'N');

        // Clone the entire dataItem and prepare plain object
        var payload = angular.copy(dataItem.toJSON());

        $http.post("/api/QuotationApi/setUserAccess", payload)
            .then(function (response) {
                if (response.data && response.data.success) {
                    displayPopupNotification("Updated successfully", "success");
                } else {
                    displayPopupNotification("Failed to update", "error");
                }
            })
            .catch(function (error) {
                console.error("Update error:", error);
                displayPopupNotification("Error updating", "error");
            });
    };



    $http.get("/api/quotationapi/UserTypeToSetValue")
        .then(function (response) {
            var canEdit = response.data && response.data.success === true;
            $scope.initGrid(canEdit);
        })
        .catch(function (error) {
            console.error("Failed to check user type permission:", error);
            $scope.initGrid(false);
        });
});