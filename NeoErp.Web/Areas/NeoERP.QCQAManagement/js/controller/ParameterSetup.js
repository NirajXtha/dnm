QCQAModule.controller('ParameterSetupList', function ($scope, $rootScope, $http, $filter, $timeout, $compile) {
    $scope.AddParameterModal = function () {
        $('#ParameterModal').modal('show');
    }
    $scope.saveParameterSetup = function () {
        var itemList = {
            item_code: $scope.item_code,
            item_edesc: $scope.item_edesc,
            ITEM_APPLY_ON: $scope.ITEM_APPLY_ON,
            BRAND_NAME: $scope.BRAND_NAME,
            PART_NUMBER: $scope.PART_NUMBER,
            ITEM_SPECIFICATION: $scope.ITEM_SPECIFICATION,
            thickness: $scope.thickness,
            INTERFACE: $scope.INTERFACE,
            COLOR: $scope.COLOR,
            LAMINATION: $scope.LAMINATION,
            GRADE: $scope.GRADE,
            TYPE: $scope.TYPE,
            GSM: $scope.GSM,
            RollDiameter: $scope.RollDiameter,
            PH: $scope.PH,
            UNPLEASANT_SMELL_ODOUR: $scope.UNPLEASANT_SMELL_ODOUR,
            Dust_Dirt: $scope.Dust_Dirt,
            Damaging_Material: $scope.Damaging_Material,
            Core_Damaging: $scope.Core_Damaging,
            Tensile_CD: $scope.Tensile_CD,
            Tensile_MD: $scope.Tensile_MD,
            Strength: $scope.Strength,
            Strength_MD: $scope.Strength_MD,
            Visual_Inspection: $scope.Visual_Inspection,
            ITEM_SIZE: $scope.ITEM_SIZE,
            SIZE_LENGHT: $scope.SIZE_LENGHT,
            SIZE_WIDTH: $scope.SIZE_WIDTH,
            REEM_WEIGHT_KG: $scope.REEM_WEIGHT_KG,
            REMARKS: $scope.REMARKS
        }
        $http.post('/api/ParameterSetupAPI/saveParameterSetup', itemList)
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

    $scope.grid = {
        change: rawMaterialChange,
        dataSource: {
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/ParameterSetupAPI/GetGroupMaterialLists"
            },
            schema: {
                model: {
                    id: "ITEM_CODE" // Unique identifier
                }
            }
        },
        height: 400,
        groupable: false,
        sortable: false,
        scrollable: true,
        columns: [
            {
                field: "item_edesc",
                title: "Groups",
                width: 200,
                template: "<a href='javascript:void(0)' class='expand-btn plus-icon-material'><b style='font-size: 13px;width: 10px;height: 13px;'>+</b></a> #= item_edesc #"
            }
        ],
        dataBound: function (e) {
            var grid = this;
            // Remove previous bindings to prevent duplication
            $(document).off('click', '.expand-btn');
            $(document).off('click', '.child-row');  // Remove previous binding for child-row

            // Bind click event using event delegation for expand/collapse buttons
            $(document).on('click', '.expand-btn', function (event) {
                var dataItem = grid.dataItem($(this).closest("tr"));
                if (!dataItem) {
                    return;
                }
                // Use $scope.$apply only if necessary
                $scope.$applyAsync(function () {
                    $scope.toggleRow(event, dataItem);
                });
            });

            // Bind click event to child rows
            $(document).on('click', '.child-row', function (event) {
                var $childRow = $(this);
                // Retrieve the data attribute
                var masterItemCode = $childRow.find('td').attr('data-child');
                if (!masterItemCode) {
                    return;
                }
                // Call function with masterItemCode
                rawMaterialChange(masterItemCode);
            });
        }
    };

    // Function to expand/collapse child rows
    $scope.toggleRow = function (event, dataItem) {
        if (!dataItem || !dataItem.master_item_code) {
            return; // Exit early if dataItem is invalid or master_item_code is missing
        }
        var parentId = dataItem.master_item_code; // Get the parent ID
        var $target = angular.element(event.target); // Use Angular's element for better binding
        var $parentRow = $target.closest("tr");
        if ($target.text() === "+") {
            $target.text("-"); // Change to "-"
        } else {
            $target.text("+"); // Change back to "+"

            // Remove all child rows related to this parent
            angular.element(".child-row[data-parent='" + parentId + "']").remove();
            return; // Exit function since we are collapsing
        }

        // Fetch child data using AJAX
        $.ajax({
            url: window.location.protocol + "//" + window.location.host + "/api/ParameterSetupAPI/GetChildItems?masterItemCode=" + parentId,
            dataType: "json",
            success: function (childData) {

                if (childData && childData.length > 0) {
                    // Loop through child items and append them as new rows
                    childData.forEach(function (child) {
                        var childRow = angular.element("<tr class='child-row' data-parent='" + parentId + "'></tr>");
                        var childCell = angular.element("<td colspan='2' data-child='" + child.master_item_code + "' class ='" + child.master_item_code + "' style='padding-left: 30px;'></td>").text(child.item_edesc);
                        childRow.append(childCell);
                        // Insert after the parent row
                        $parentRow.after(childRow);
                        $parentRow = childRow;
                    });
                } else {
                    alert("No child items found for ITEM_CODE: " + parentId);
                }
            },
            error: function (xhr, status, error) {
                alert("Error fetching child items. Please check the console.");
            }
        });
    };

    function rawMaterialChange(evt) {
        var masterItemCode = evt;
        // Example: Call API to fetch product details
        $http.get('/api/ParameterSetupAPI/GetProductDetails?masterItemCode=' + masterItemCode)
            .then(function (response) {
                $scope.Outlets = response.data;
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            });
    }

    $scope.AddParameterModal = function (itemCode, itemDesc) {
        if (itemCode) {
            // Edit Mode
            $http.get('/api/ParameterSetupAPI/GetSpecDetailsByItemID?itemCode=' + itemCode).then(function (response) {
                $scope.parameter = response.data; // Load the data into the form
                if (response.data.length > 0) {
                    var param = response.data[0];
                    $scope.item_code = itemCode;
                    $scope.item_edesc = param.item_edesc;
                    $scope.ITEM_APPLY_ON = param.ITEM_APPLY_ON;
                    $scope.BRAND_NAME = param.BRAND_NAME;
                    $scope.PART_NUMBER = param.PART_NUMBER;
                    $scope.ITEM_SPECIFICATION = param.ITEM_SPECIFICATION;
                    $scope.thickness = param.thickness;
                    $scope.INTERFACE = param.INTERFACE;
                    $scope.COLOR = param.COLOR;
                    $scope.LAMINATION = param.LAMINATION;
                    $scope.GRADE = param.GRADE;
                    $scope.TYPE = param.TYPE;
                    $scope.GSM = param.GSM;
                    $scope.RollDiameter = param.RollDiameter;
                    $scope.PH = param.PH;
                    $scope.UNPLEASANT_SMELL_ODOUR = param.UNPLEASANT_SMELL_ODOUR;
                    $scope.Dust_Dirt = param.Dust_Dirt;
                    $scope.Damaging_Material = param.Damaging_Material;
                    $scope.Core_Damaging = param.Core_Damaging;
                    $scope.Tensile_CD = param.Tensile_CD;
                    $scope.Tensile_MD = param.Tensile_MD;
                    $scope.Strength = param.Strength;
                    $scope.Strength_MD = param.Strength_MD;
                    $scope.Visual_Inspection = param.Visual_Inspection;
                    $scope.ITEM_SIZE = param.ITEM_SIZE;
                    $scope.SIZE_LENGHT = param.SIZE_LENGHT;
                    $scope.SIZE_WIDTH = param.SIZE_WIDTH;
                    $scope.REEM_WEIGHT_KG = param.REEM_WEIGHT_KG;
                    $scope.REMARKS = param.REMARKS;
                }
                else {
                    $scope.item_code = itemCode;
                    $scope.item_edesc = itemDesc;
                }
                $('#ParameterModal').modal('show'); // Open modal
            });
        } else {
            // Add Mode
            $scope.parameter = {}; // Reset the form
            $('#ParameterModal').modal('show'); // Open modal
        }
    };

    $http.get('/api/ParameterSetupAPI/MasterItemList')
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
    $scope.ItemCancel = function () {
        //window.location.reload();
    }
});
