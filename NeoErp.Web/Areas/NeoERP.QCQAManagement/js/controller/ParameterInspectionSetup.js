QCQAModule.controller('ParameterInspectionSetupList', function ($scope, $rootScope, $http, $filter, $timeout, $routeParams) {
    $scope.voucherno = "";
    $scope.childParameterInspectionItemModels = [];
    $scope.AddQCQAParameterInspectionSetup = function () {
        $http.get('/api/ParameterInspectionSetupAPI/GetItemCheckList')
            .then(function (response) {
                var product = response.data;
                $scope.ParameterInspectionSetup = response.data;
                $scope.childParameterInspectionItemModels = response.data;
                $('#QCQAParameterInspectionModal').modal('show');
            }).catch(function (error) {
                var message = 'Error in displaying no!!';
                displayPopupNotification(message, "error");
            })
    };

    $http.get('/api/ParameterInspectionSetupAPI/GetItemCheckListDetails')
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
    $("#ParameterInspectionSetupGrid").kendoGrid({
        dataSource: $scope.dataSource,
        height: 400,
        sortable: true, // Enable sorting
        pageable: {
            refresh: true,
            pageSizes: true
        },
        resizable: true, // Enable column resizing
        columns: [
            { field: "INS_PARAM_DETAIL_NO", title: "ID", width: 100, type: "string" },
            { field: "PARAMETERS", title: "PARAMETERS", width: 200, type: "string" },
            {
                title: "Actions",
                width: 120,
                template: "<a class='btn btn-sm btn-info view-btn' data-id='#= INS_PARAM_DETAIL_NO #'><i class='fa fa-eye'></i></a>&nbsp;<a class='btn btn-sm btn-warning edit-btn' data-id='#= INS_PARAM_DETAIL_NO #'><i class='fa fa-edit'></i></a>&nbsp;<a class='btn btn-sm btn-danger delete-btn' data-id='#= INS_PARAM_DETAIL_NO #'><i class='fa fa-trash'></i></a>"
            }
        ]
    });
    $("#ddlInspectionParameter").kendoDropDownList({
        optionLabel: "Select Parameter...",
        dataTextField: "PARAMETERS",   // <- replace with actual property
        dataValueField: "INSPECTION_PARAM_NO",    // <- replace with actual property
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/ParameterInspectionSetupAPI/GetParameterInspectionList",
                    dataType: "json"
                }
            }
        }
    }).data("kendoDropDownList");
   

    if ($routeParams.voucherno != undefined) {
        $scope.voucherno = $routeParams.voucherno;//.split(new RegExp('_', 'i')).join('/');     
    }
    $("#englishdatedocument").kendoDatePicker({
        value: new Date(),
        format: "dd-MMM-yyyy"
    });

    $scope.add_child_element = function (e) {
        $http.get('/api/ParameterInspectionSetupAPI/GetItemCheckList')
            .then(function (response) {
                var qcqa = response.data;
                $scope.ParameterInspectionSetup.push(qcqa);
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    // remove child row.
    $scope.remove_child_element = function (index) {
        if ($scope.ParameterInspectionSetup.length > 1) {
            $scope.ParameterInspectionSetup.splice(index, 1);
            //$scope.childModels.splice(index, 1);
        }
    }
    $("#ParameterInspectionSetupGrid").on("click", ".edit-btn", function () {
        var editBtn = $(this);
        var id = $(this).data("id");
       
        $http.get('/api/ParameterInspectionSetupAPI/GetParameterInspectionById?id=' + id)
            .then(function (response) {
                var parameter = response.data;
                console.log("parameter", parameter);
                var comboBox = $("#ddlInspectionParameter").data("kendoDropDownList");
                $scope.INS_PARAM_DETAIL_NO = id;

                /* var selectedParameter = parameter.INSPECTION_PARAM_NO;*/
                /*var selectedParameter = parameter.INSPECTION_PARAM_NO;*/
                var selectedParameter = parameter.PARAMETER_ID;
                //var selectedItemCodes = product.ITEM_CODE.split(","); // if saved as comma

                comboBox.value(selectedParameter); // select the type
                //MultiSelectProduct(selectedProductType, selectedItemCodes); //
                var parameterDetails = response.data.ParameterItemDetailsList;
               // $scope.ParameterItems = [];
                $scope.ParameterInspectionSetup = [];
                parameterDetails.forEach(function (row, rowIndex) {
                    var parameterDetailsRow = [
                        { COLUMN_NAME: "ITEMS", VALUE: row.ITEMS }
                    ];
                    /*$scope.ParameterInspectionSetup.push({ element: parameterDetailsRow });*/
                    $scope.ParameterInspectionSetup.push(parameterDetailsRow);
                    $scope.childParameterInspectionItemModels = $scope.childParameterInspectionItemModels || [];
                    $scope.childParameterInspectionItemModels[rowIndex] = {};
                    parameterDetailsRow.forEach(function (item) {
                        $scope.childParameterInspectionItemModels[rowIndex][item.COLUMN_NAME] = item.VALUE;
                    });
                });
                $('#QCQAParameterInspectionModal').modal('show');
            }).catch(function (error) {
                var message = 'Error in displaying no!!';
                displayPopupNotification(message, "error");
            })
    })
    var rawMateriallList = [];
    $scope.saveParameterInspection = function () {
        /*var rows = document.querySelectorAll("#idIntenalInspection");*/
        var rows = document.querySelectorAll("#ParameterInspection");
        var isValid = true;
        if ($("#ddlInspectionParameter").val() == null || $("#ddlInspectionParameter").val() == "" || $("#ddlInspectionParameter").val() == undefined) {
            displayPopupNotification("Parameter is required", "warning");
            return false;
        }
        rows.forEach(function (row, rowIndex) {
            if (row.querySelector(".ITEMS_" + rowIndex).value == null || row.querySelector(".ITEMS_" + rowIndex).value == "" || row.querySelector(".ITEMS_" + rowIndex).value == undefined) {
                displayPopupNotification("Item is required", "warning");
                isValid = false;
            }
        });
        // var rows = $scope.childModels;
        if (!isValid) {
            return false;   // STOP here before save
        }
        rows.forEach(function (row, rowIndex) {
            var rawMaterial = {
                ITEMS: $(".ITEMS_" + rowIndex).val() //row.querySelector(".clsDepartmentCode_" + rowIndex)?.value || "", //
            };
            rawMateriallList.push(rawMaterial);
        });
        var wrapper = {
            PARAMETER_ID: $('#ddlInspectionParameter').val(),
            CREATED_DATE: $('#englishdatedocument').val(),
            ParameterItemDetailsList: rawMateriallList  // lowercase 'list'
        };
        $http.post('/api/ParameterInspectionSetupAPI/saveParameterInspection', wrapper)
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