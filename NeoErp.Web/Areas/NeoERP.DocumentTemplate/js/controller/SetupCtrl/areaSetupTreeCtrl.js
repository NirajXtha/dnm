DTModule.controller('areaSetupTreeCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    $scope.saveAction = "Save";
    $scope.btnDelete = false;
    $scope.Area = {
        AREA_CODE: '',
        AREA_EDESC: '',
        REMARKS: '',
        AREAID: ''
    };
    $scope.AreaHeader = "Add Area";
    $scope.grid = {
        change: AreaChange,
        dataSource: {
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAreaCodeWithChild",
            },
            pageSize: 20
        },
        dataBound: function (e) {
            //GetSetupSetting("AreaSetup");
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
                field: "AREA_EDESC",
                title: "Area Name",
                width: 150,
            }]

    }
    function AreaChange(evt) {

        selectedRow = this.select();
        var item = this.dataItem(selectedRow);

        if (item) {
            $scope.saveAction = "Update";
            $scope.AreaHeader = "Update Area";

            $scope.Area.AREA_EDESC = item.AREA_EDESC;
            $scope.Area.AREA_CODE = item.AREA_CODE;
            $scope.Area.REMARKS = item.REMARKS;
            $scope.Area.AREAID = item.AREAID;
            $scope.btnDelete = true;
            $scope.$apply();
        }

    }

    $scope.getMaxAreaCode = function () {
        var getMaxCodeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getMaxAreaCode";
        $http({
            method: 'Get',
            url: getMaxCodeUrl,
        }).then(function successCallback(response) {
            $scope.Area.AREA_CODE = response.data.DATA;
        }, function errorCallback(response) {
            displayPopupNotification(response.data.STATUS_CODE, "error");
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
    }

    $scope.getMaxAreaCode();

    $scope.SaveArea = function (isValid) {
        var model = {
            AREA_CODE: $scope.Area.AREA_CODE,
            AREA_EDESC: $scope.Area.AREA_EDESC,
            REMARKS: $scope.Area.REMARKS
        }
        if (!isValid) {
            displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
            return;
        }
        if ($scope.saveAction == "Save") {
            var saveAreaUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewAreaSetup";
            $http({
                method: 'POST',
                url: saveAreaUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "INSERTED") {
                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully saved ", "success");
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Area name already exist please try another area name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

                // this callback will be called asynchronously
                // when the response is available
            }, function errorCallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Area name already exist please try another area name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            }
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            )
        }
        else {
            var udpateAreaUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateAreaSetup";
            $http({
                method: 'POST',
                url: udpateAreaUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "UPDATED") {
                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully updated ", "success");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // this callback will be called asynchronously
                // when the response is available
            }, function errorCallback(response) {
                displayPopupNotification("Something went wrong.Please try again later.", "error");
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
        }

        $("#grid").data("kendoGrid").dataSource.read();

    }

    $scope.filterTreeView = function () {
        const searchText = ($scope.txtMainGroupSearchString || '').toLowerCase();

        var dataSource = $("#grid").data("kendoGrid").dataSource;

        if (searchText) {
            dataSource.filter({
                field: "AREA_EDESC",
                operator: "contains",
                value: searchText
            });
        } else {
            dataSource.filter({});
        }
    };

    $scope.deleteArea = function (isValid) {

        if (!isValid) {
            displayPopupNotification("There is no any area selected.", "warning");
            return;
        }
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

                if (result == true) {
                    var AREA_EDESC = $("#AreaName").val();
                    var AREA_CODE = $("#AreaCode").val();
                    var AREAID;
                    if (AREAID == undefined) {
                        AREAID = $scope.Area.AREA_CODE;
                    }

                    var deleteAreaUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/deleteAreaSetup?areaCode=" + AREAID;
                    $http({
                        method: 'POST',
                        url: deleteAreaUrl,
                    }).then(function successCallback(response) {
                        if (response.data.MESSAGE == "DELETED") {
                            displayPopupNotification("Data succesfully deleted ", "success");
                            $("#grid").data("kendoGrid").dataSource.read();
                            $scope.Cancel();
                        }
                        else {
                            displayPopupNotification(response.data.STATUS_CODE, "error");
                        }
                        // this callback will be called asynchronously
                        // when the response is available
                    }, function errorCallback(response) {
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                        // called asynchronously if an error occurs
                        // or server returns response with an error status.
                    });
                }

                //$("#grid").data("kendoGrid").dataSource.read();
                $scope.Cancel();
            }

        });


    }

    $scope.Cancel = function () {
        $scope.saveAction = "Save";
        $scope.AreaHeader = "Add Area";
        $scope.Area = {
            AREA_CODE: '',
            AREA_EDESC: '',
            AREAID: ''
        };
        $("#grid").data("kendoGrid").clearSelection();
        $scope.getMaxAreaCode();
        $scope.btnDelete = false;
    }

});

