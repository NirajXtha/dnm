QCQAModule.controller('QCQAInspectionSetup', function ($scope, $rootScope, $http, $filter, $timeout) {
    $scope.saveAction = "Save";
    $scope.AddQCQA = function () {
        $('#qcqainspectionModal').modal('show');
    }
   
     $scope.dataSource = new kendo.data.DataSource({
        data: [], // Initially empty 
    });


    $scope.tableName = "QC_Master_Setup";

    var autoCompletedataSource = window.location.protocol + "//" + window.location.host + "/api/QCQAAPI/GetFormCode?tableName=" + $scope.tableName;
    var formCode = "";
    $http.get(autoCompletedataSource).then(function (response) {
        
        if (response.data && response.data.length > 0) {
            formCode = response.data[0].FORM_CODE;
        }
    })
        .catch(function (error) {
            displayPopupNotification(error, "error");
        })

    var req = '/api/QCQAAPI/GetQCQADetails?tableName=' + $scope.tableName;
        $http.get(req).then(function (response) {
            var qc = response.data;
            if (qc && qc.length > 0) {
                $scope.dataSource.data(qc);
            }
        })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })


    $scope.TableSelect = {
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/QCQAApi/TableLists",
                    dataType: "json"
                }
            }
        },
        dataTextField: "TABLE_DESC",
        dataValueField: "TABLE_NAME",
        filter: "contains",
        autoClose: true,
        change: function (e) {
            // Handle change event if needed
            var selectedItem = this.dataItem();
        }
    };

    var ddlUrl = window.location.protocol + "//" + window.location.host + "/api/QCQAAPI/TableLists"
    var defaultValue = "QC_Master_Setup";
    $("#ddlTableName").kendoDropDownList({
        optionLabel: "-- Select Table --",
        dataTextField: "TABLE_DESC",
        dataValueField: "TABLE_NAME",
        autoBind: true,
        filter: "contains",
        dataSource: {
            type: "json",
            serverFiltering: false,
            transport: {
                read: {
                    url: ddlUrl,
                },
            }
        },
        dataBound: function () {
            var dropdown = this;
            var items = dropdown.dataSource.data();

            // Check if the default value exists in the data
            var exists = items.some(function (item) {
                return item.TABLE_NAME === defaultValue;
            });

            if (exists) {
                dropdown.value(defaultValue);  // Set default value
                callNeewFieldAPI(defaultValue);
            }
        },
        change: function (e) {
            var dropdown = e.sender;
            var selectedValue = dropdown.value();

            // Update AngularJS scope
            var scope = angular.element($("#ddlTableName")).scope();
            scope.$apply(function () {
                scope.tableName = selectedValue;
            });

            // Call API
            callAPI(scope.tableName);
            callNeewFieldAPI(scope.tableName)
        }
    });

    function callAPI(tableName) {
        var req = '/api/QCQAAPI/GetQCQADetails?tableName=' + tableName;
        $http.get(req).then(function (response) {
            var qc = response.data;
            if (qc && qc.length > 0) {
                $scope.dataSource.data(qc);
                $scope.$applyAsync();
            } else {
                $scope.dataSource.data([]);  // Clear data when no records
            }
        })
            .catch(function (error) {
                $scope.dataSource.data([]);
                displayPopupNotification(error, "error");
            })
    }

    function callNeewFieldAPI(tableName) {
        var req = '/api/QCQAAPI/GetQCQADetailsByTableName?tableName=' + tableName;
        $http.get(req).then(function (response) {
            var qcNewField = response.data;

            // Get reference to the New Field dropdown
            var dropdown = $("#ddlNewField").data("kendoDropDownList");

            // If dropdown isn't initialized, initialize it
            if (!dropdown) {
                $("#ddlNewField").kendoDropDownList({
                    dataTextField: "COLUMN_NAME",  // Update with correct field name
                    dataValueField: "COLUMN_NAME",   // Update with correct field name
                    optionLabel: "-- Select New Field --", // Optional label
                    enable: true, // Enable by default
                    dataSource: {
                        data: qcNewField || []  // Set data if available
                    }
                });
            } else {
                // If dropdown is already initialized, just update its data source
                dropdown.setDataSource(new kendo.data.DataSource({
                    data: qcNewField || []
                }));

                // Enable the dropdown if data is available
                if (qcNewField && qcNewField.length > 0) {
                    dropdown.enable();
                } else {
                    dropdown.enable(false);  // Disable if no data
                }
            }
        })
            .catch(function (error) {
                // Handle error and disable dropdown if needed
                var dropdown = $("#ddlNewField").data("kendoDropDownList");
                if (dropdown) {
                    dropdown.setDataSource(new kendo.data.DataSource({ data: [] }));
                    dropdown.enable(false);
                }
                displayPopupNotification(error, "error");
            });
    }

    $scope.QCQACreate = function () {
        var combinedData = [];

        $("#kQCQAGrid tbody tr").each(function () {
            var model = {
                COLUMN_NAME: $(this).find('.clsColumnName').text(),
                COLUMN_HEADER: $(this).find('.clsHeader').val(),
                COLUMN_WIDTH: $(this).find('.clsWidth').val(),
                TOP_POSITION: $(this).find('.clsTopPosition').val(),
                LEFT_POSITION: $(this).find('.clsLeftPosition').val(),
                MASTER_CHILD_FLAG: $(this).find('.clsMasterChildFlag').val(),
                IS_DESC_FLAG: $(this).find('.clsIsDescFlag').val(),
                DEFA_VALUE: $(this).find('.clsDefaValue').val(),
                FILTER_VALUE: $(this).find('.clsFilterValue').val(),
                FORM_CODE: formCode,
                DISPLAY_FLAG: $(this).find('.clsDisplayFlag').is(":checked") ? "Y" : "N"
            };

            combinedData.push(model);
        });
        var staturl = window.location.protocol + "//" + window.location.host + "/api/QCQAAPI/AddColumnsToTable?tableName=" + encodeURIComponent($scope.tableName);
        var response = $http({
            method: "POST",
            data: combinedData,
            url: staturl,
            contentType: "application/json",
            dataType: "json"
        });
        return response.then(function (data) {
            if (data) {
                displayPopupNotification("Saved Successfully", "Success");
            }
            else { displayPopupNotification("Failed To Save", "Error");}
        });
    }
            
    $("#kQCQAGrid").on("change", ".clsDisplayFlag", function () {
        updateDisplayFlag(this);
    });
    function updateDisplayFlag(element) {
        var isChecked = $(element).is(":checked");
        var columnName = $(element).data("id");

        var grid = $("#kQCQAGrid").data("kendoGrid");
        var dataItems = grid.dataSource.data();

        var dataItem = dataItems.find(item => item.COLUMN_NAME === columnName);

        if (dataItem) {
            var newValue = isChecked ? "Y" : "N";
            $(this).find('.clsDisplayFlag').val(newValue);
            dataItem.set("DISPLAY_FLAG", newValue);           
        }
    }
});

