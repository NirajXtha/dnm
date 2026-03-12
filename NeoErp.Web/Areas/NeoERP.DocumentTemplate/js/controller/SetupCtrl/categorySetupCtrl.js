DTModule.controller('categorySetupCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    $scope.saveAction = "Save";
    $scope.FormName = "Category Setup";
    $scope.btnDelete = false;
    $scope.CategorySetup = {
        CATEGORY_CODE: "",
        ENG_CATEGORY: "",
        NEP_CATEGORY: "",
        CREATED_DATE: "",
        MODIFY_DATE: "",
        MODIFY_BY: "",
        REMARKS: "",
        CATEGORY_TYPE: "",
        PREFIX_TEXT: ""
    };
    var getCategoryUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getCategoryList";
    $scope.grid = {
        change: CategoryChange,
        dataSource: {
            transport: {
                read: getCategoryUrl,
            },
            pageSize: 20
        },
        dataBound: function (e) {
        },
        height: window.innerHeight - 50,
        groupable: false,
        sortable: false,
        height: 470,
        selectable: true,
        scrollable: {
            virtual: true
        },
        columns: [
            {
                field: "CATEGORY_EDESC",
                title: "Category Name",
                width: 150
            }
        ]
    }
    function CategoryChange(evt) {
        debugger;
        selectedRow = this.select();
        var item = this.dataItem(selectedRow);
        if (item) {
            $scope.saveAction = "Update";
            $scope.TransporterHeader = "Update Category";
            $scope.CategorySetup.REMARKS = item.REMARKS;  
            $scope.CategorySetup.CATEGORY_CODE = item.CATEGORY_CODE;
            $scope.CategorySetup.ENG_CATEGORY = item.CATEGORY_EDESC;
            $scope.CategorySetup.NEP_CATEGORY = item.CATEGORY_NDESC;
            $scope.CategorySetup.CATEGORY_TYPE = item.CATEGORY_TYPE;
            $scope.CategorySetup.PREFIX_TEXT = item.PREFIX_TEXT;
            $scope.btnDelete = true;
            $scope.$apply();
        }
    }

    $scope.SaveTransporter = function (isValid) {
        debugger;
        if (!$scope.CategorySetup.CATEGORY_CODE) {
            displayPopupNotification("Invlaid Category Code", "error");
        }
        else if (!$scope.CategorySetup.ENG_CATEGORY) {
            displayPopupNotification("Invlaid in English", "error");
        }
        else if (! /^[a-z]{2}$/i.test($scope.CategorySetup.CATEGORY_TYPE))
        { displayPopupNotification("Only Two Character in catagory type allowed", "error"); }
        else {
                if ($scope.saveAction == "Save") {
                    var model = {
                        REMARKS: $scope.CategorySetup.REMARKS,
                        CATEGORY_CODE: $scope.CategorySetup.CATEGORY_CODE,
                        CATEGORY_EDESC: $scope.CategorySetup.ENG_CATEGORY,
                        CATEGORY_NDESC: $scope.CategorySetup.NEP_CATEGORY,
                        CATEGORY_TYPE: $scope.CategorySetup.CATEGORY_TYPE,
                        PREFIX_TEXT: $scope.CategorySetup.PREFIX_TEXT
                    }
                    var saveCategoryUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewCategorySetup";
                    $http({
                        method: 'POST',
                        url: saveCategoryUrl,
                        data: model
                    }).then(function successCallback(response) {
                        debugger;
                        if (response.data.MESSAGE == "INSERTED") {
                            $("#grid").data("kendoGrid").dataSource.read();
                            $scope.Cancel();
                            displayPopupNotification("Data succesfully saved ", "success");
                        }
                      else if (response.data.MESSAGE == "INVALIDINSERTED") {
                            displayPopupNotification("Category name already exist please try another Category name.", "error");
                        }
                        else {
                            displayPopupNotification("Something went wrong.Please try again later.", "error");
                        }

                    }, function errorCallback(response) {
                        if (response.data.MESSAGE == "INVALIDINSERTED") {
                            displayPopupNotification("Category name already exist please try another Category name.", "error");
                        }
                        else {
                            displayPopupNotification("Something went wrong.Please try again later.", "error");
                        }

                    });
                }
                 else if ($scope.saveAction == "Update") {
                    debugger;
                    var udpateCategoryUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateNewCategorySetup";
                    var model1 = {
                        REMARKS: $scope.CategorySetup.REMARKS,
                        CATEGORY_CODE: $scope.CategorySetup.CATEGORY_CODE,
                        CATEGORY_EDESC: $scope.CategorySetup.ENG_CATEGORY,
                        CATEGORY_NDESC: $scope.CategorySetup.NEP_CATEGORY,
                        CATEGORY_TYPE: $scope.CategorySetup.CATEGORY_TYPE,
                        PREFIX_TEXT: $scope.CategorySetup.PREFIX_TEXT
                    }
                    $http({
                        method: 'POST',
                        url: udpateCategoryUrl,
                        data: model1
                    }).then(function successCallback(response) {
                        if (response.data.MESSAGE == "UPDATED") {
                            debugger;
                            $("#grid").data("kendoGrid").dataSource.read();
                            $scope.Cancel();
                            displayPopupNotification("Data succesfully updated ", "success");
                        }
                        else {
                            displayPopupNotification("Something went wrong.Please try again later.", "error");
                        }

                    }, function errorCallback(response) {
                        displayPopupNotification("Something went wrong.Please try again later.", "error");

                    });
                 }
             }
    }
    $scope.deleteTransporter = function () {
        bootbox.confirm({
            title: "Delete",
            message: "Are you sure?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success',
                    label: '<i class="fa fa-check"></i> Yes',
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger',
                    label: '<i class="fa fa-times"></i> No',
                }
            },
            callback: function (result) {
                debugger;
                if (result == true) {
                    var  CATEGORY_CODE = $("#categoryCode").val();
                    if ( CATEGORY_CODE == undefined) {
                        CATEGORY_CODE = $scope.CurrencySetup. CATEGORY_CODE;
                    }
                    var deleteTransporterUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/deleteCategorySetup?categoryCode=" +  CATEGORY_CODE;
                    $http({
                        method: 'POST',
                        url: deleteTransporterUrl,
                    }).then(function successCallback(response) {
                        debugger;
                        if (response.data.MESSAGE == "DELETED") {
                            $("#grid").data("kendoGrid").dataSource.read();
                            $scope.Cancel();
                            displayPopupNotification("Data succesfully deleted ", "success");
                        }
                        else {
                            displayPopupNotification(response.data.STATUS_CODE, "error");
                        }
                    }, function errorCallback(response) {
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                    });
                }
                else {
                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                }
            }
        });
    }
    $scope.Cancel = function () {
        $scope.saveAction = "Save";
        $scope.CategorySetup = {
            CATEGORY_CODE: "",
            ENG_CATEGORY: "",
            NEP_CATEGORY: "",
            REMARKS: "",
            CATEGORY_TYPE: "",
            PREFIX_TEXT: ""
        };
        $scope.btnDelete = false;
        $("#grid").data("kendoGrid").clearSelection();
    }
    var categoryUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetCategoryCode";
    $scope.categoryDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: categoryUrl,
            }
        }
    });
    $scope.categoryOptions = {
        dataSource: $scope.categoryDataSource,
        dataTextField: "CATEGORY_EDESC",
        dataValueField: "CATEGORY_CODE",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {
        }
    }
    $scope.getMaxCategoryCode = function () {
        var getMaxCategoryCodeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetMaxCategoryCode";
        $http({
            method: 'GET',
            url: getMaxCategoryCodeUrl,
        }).then(function successCallback(response) {
            $scope.CategorySetup.CATEGORY_CODE = response.data;
        })
    };

    //$scope.getMaxCategoryCode();

});