QCQAModule.controller('FinishedGoodsSetupList', function ($scope, $rootScope, $http, $filter, $timeout, $routeParams) {
    $scope.voucherno = "";
    $scope.childParameterInspectionItemModels = [];
    //$scope.ParameterInspectionSetup = [];
    //$scope.ParameterInspectionSetup = [];
    $scope.AddQCQAFinishedGoodsSetup = function () {
        $http.get('/api/FinishedGoodsSetupAPI/GetFinishedItemCheckList')
            .then(function (response) {
                var product = response.data;
                //$scope.ParameterInspectionSetup = [];
                //$scope.childParameterInspectionItemModels = [];
                $scope.FinishedGoodsSetup = response.data;
                $scope.childParameterInspectionItemModels = response.data;
                $('#QCQAParameterInspectionModal').modal('show');
            }).catch(function (error) {
                var message = 'Error in displaying no!!';
                displayPopupNotification(message, "error");
            })
    };
    $http.get('/api/FinishedGoodsSetupAPI/GetFinishedItemCheckListDetails')
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
    $("#FinishedGoodsSetupGrid").kendoGrid({
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
                    url: window.location.protocol + "//" + window.location.host + "/api/FinishedGoodsSetupAPI/GetFinishedInspectionList",
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
        $http.get('/api/FinishedGoodsSetupAPI/GetFinishedItemCheckList')
            .then(function (response) {
                var qcqa = response.data;
                $scope.FinishedGoodsSetup.push(qcqa);
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    // remove child row.
    $scope.remove_child_element = function (index) {
        if ($scope.FinishedGoodsSetup.length > 1) {
            $scope.FinishedGoodsSetup.splice(index, 1);
            //$scope.childModels.splice(index, 1);
        }
    }
    $("#FinishedGoodsSetupGrid").on("click", ".edit-btn", function () {
        var editBtn = $(this);
        var id = $(this).data("id");

        $http.get('/api/FinishedGoodsSetupAPI/GetFinishedGoodsById?id=' + id)
            .then(function (response) {
                var parameter = response.data;
                console.log("parameter", parameter);
                var comboBox = $("#ddlInspectionParameter").data("kendoDropDownList");
                $scope.INS_PARAM_DETAIL_NO = id;
                var selectedParameter = parameter.PARAMETER_ID;
                comboBox.value(selectedParameter); // select the type
                //MultiSelectProduct(selectedProductType, selectedItemCodes); //
                var parameterDetails = response.data.ParameterItemDetailsList;
                // $scope.ParameterItems = [];
                $scope.FinishedGoodsSetup = [];
                parameterDetails.forEach(function (row, rowIndex) {
                    var parameterDetailsRow = [
                        { COLUMN_NAME: "ITEMS", VALUE: row.ITEMS }
                    ];
                    $scope.FinishedGoodsSetup.push(parameterDetailsRow);
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
    $scope.saveFinishedGoods = function () {
        /*var rows = document.querySelectorAll("#idIntenalInspection");*/
        var rows = document.querySelectorAll("#FinishedGoods");
        var isValid = true;
        // var rows = $scope.childModels;
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
        $http.post('/api/FinishedGoodsSetupAPI/saveFinishedGoods', wrapper)
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