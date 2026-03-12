QCQAModule.controller('QCQANumberSetupList', function ($scope, $rootScope, $http, $filter, $timeout) {
    $scope.AddQCQASetup = function () {
        $('#QCQANumberModal').modal('show');
    }
    $scope.saveQCSetup = function () {
        var formData = {
            FORM_CODE: $scope.FORM_CODE,
            CUSTOM_PREFIX_TEXT: $scope.CUSTOM_PREFIX_TEXT,
            PREFIX_LENGTH: $('.clsPrefixLength').val(),
            CUSTOM_SUFFIX_TEXT: $scope.CUSTOM_SUFFIX_TEXT,
            SUFFIX_LENGTH: $('.clsSuffixLength').val(),
            BODY_LENGTH: $scope.BODY_LENGTH,
            START_NO: $scope.START_NO,
            LAST_NO: $scope.LAST_NO,
            BODY_LENGTH: $scope.BODY_LENGTH,
            START_DATE: $('#idStartDate').val(),
            LAST_DATE: $('#idEndDate').val()
        }
        $http.post('/api/QCQANumberSetupAPI/saveQCSetup', formData)
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

    $http.get('/api/QCQANumberSetupAPI/QCQANumberDetails')
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
    $("#NumberSetupGrid").kendoGrid({
        dataSource: $scope.dataSource,
        height: 400,
        sortable: true, // Enable sorting
        pageable: {
            refresh: true,
            pageSizes: true
        },
        resizable: true, // Enable column resizing
        columns: [
            { field: "FORM_CODE", title: "S.N", width: 90, type: "string" },
            { field: "CUSTOM_PREFIX_TEXT", title: "Prefix Text", width: 200, type: "string" },
            { field: "PREFIX_LENGTH", title: "Prefix Length", width: 100, type: "string" },
            {
                field: "CUSTOM_SUFFIX_TEXT", title: "Suffix Text", width: 200, type: "string",
            },
            { field: "SUFFIX_LENGTH", title: "Suffix Length", width: 100, type: "string" },
            {
                field: "BODY_LENGTH", title: "Body Length", width: 150, type: "string",
            },
            {
                field: "CREATED_DATE", title: "Created Date", width: 150, type: "string",
                template: "#=kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy') #",
            },
            {
                title: "Actions",
                width: 120,
                template: "<a class='btn btn-sm btn-info view-btn' data-id='#= FORM_CODE #'><i class='fa fa-eye'></i></a>&nbsp;<a class='btn btn-sm btn-warning edit-btn' data-id='#= FORM_CODE #'><i class='fa fa-edit'></i></a>&nbsp;<a class='btn btn-sm btn-danger delete-btn' data-id='#= FORM_CODE #'><i class='fa fa-trash'></i></a>"
            }
        ]
    });
    // Handle click event for the delete button
    $("#NumberSetupGrid").on("click", ".delete-btn", function () {
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
            $http.post('/api/QCQANumberSetupAPI/DeleteQCQAById?formCode=' + id)
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
    $("#NumberSetupGrid").on("click", ".view-btn", function () {
        var viewBtn = $(this);
        var id = $(this).data("id");
        $http.get('/api/QCQANumberSetupAPI/GetQCQAById?formCode=' + id)
            .then(function (response) {
                $scope.panelMode = 'view';              
                var qcqa = response.data[0];
                $scope.CUSTOM_PREFIX_TEXT = qcqa.CUSTOM_PREFIX_TEXT;
                $scope.PREFIX_LENGTH = qcqa.PREFIX_LENGTH;
                $scope.CUSTOM_SUFFIX_TEXT = qcqa.CUSTOM_SUFFIX_TEXT;
                $scope.SUFFIX_LENGTH = qcqa.SUFFIX_LENGTH;
                $scope.BODY_LENGTH = qcqa.BODY_LENGTH;
                $scope.START_NO = qcqa.START_NO;
                $scope.LAST_NO = qcqa.LAST_NO;
                $scope.START_DATE = qcqa.START_DATE;
                $scope.LAST_DATE = qcqa.LAST_DATE;
                $('#QCQANumberModal').modal('show');
            }).catch(function (error) {
                var message = 'Error in displaying no!!';
                displayPopupNotification(message, "error");
            })
    })


    $("#NumberSetupGrid").on("click", ".edit-btn", function () {
        var editBtn = $(this);
        var id = $(this).data("id");
        $http.get('/api/QCQANumberSetupAPI/GetQCQAById?formCode=' + id)
            .then(function (response) {
                var qcqa = response.data[0];
                $scope.CUSTOM_PREFIX_TEXT = qcqa.CUSTOM_PREFIX_TEXT;
                $scope.PREFIX_LENGTH = qcqa.PREFIX_LENGTH;
                $scope.CUSTOM_SUFFIX_TEXT = qcqa.CUSTOM_SUFFIX_TEXT;
                $scope.SUFFIX_LENGTH = qcqa.SUFFIX_LENGTH;
                $scope.BODY_LENGTH = qcqa.BODY_LENGTH;
                $scope.START_NO = qcqa.START_NO;
                $scope.LAST_NO = qcqa.LAST_NO;
                $scope.START_DATE = qcqa.START_DATE;
                $scope.LAST_DATE = qcqa.LAST_DATE;
                $('#QCQANumberModal').modal('show');
            }).catch(function (error) {
                var message = 'Error in displaying no!!';
                displayPopupNotification(message, "error");
            })
    })
    //edit button 

    $scope.start_datePickerOptions = {
        format: "M/d/yyyy", // Ensure matches the expected display format
        parseFormats: ["yyyy-MM-dd", "MM/dd/yyyy", "M/d/yyyy"],
        value: $scope.START_DATE
    };

    $scope.last_datePickerOptions = {
        format: "M/d/yyyy", // Ensure matches the expected display format
        parseFormats: ["yyyy-MM-dd", "MM/dd/yyyy", "M/d/yyyy"],
        value: $scope.LAST_DATE
    };
    $('.clsPrefixText').keyup(function (e) {
        e.preventDefault;
        $('.clsPrefixLength').val($(this).val().length);
    });
    $('.clsSuffixText').keyup(function (e) {
        e.preventDefault;
        $('.clsSuffixLength').val($(this).val().length);
    });

    $(".clsStartNo").on("input", function () {
        $(this).val($(this).val().replace(/[^0-9]/g, '')); // Remove non-numeric characters
    });

    $(".clsLastNo").on("input", function () {
        $(this).val($(this).val().replace(/[^0-9]/g, '')); // Remove non-numeric characters
    });
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
});
