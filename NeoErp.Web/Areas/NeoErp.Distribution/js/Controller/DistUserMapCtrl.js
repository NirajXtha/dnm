distributionModule.controller('DistUserMapCtrl', function ($scope, DistSetupService, $routeParams) {

    $scope.MappedUsers = [];

    $scope.$on('$routeChangeSuccess', function () {
        $scope.AddGroupMap();
        mapedInView();
    });

    var mapedInView = function () {
        $scope.MappedUsers = [];
        DistSetupService.GetAllUserMaps().then(function (res) {
            if (res.data.length > 0) {
                res.data.forEach(function (val, key) {
                    res.data[key].LOGIN_SP_CODE = [res.data[key].LOGIN_SP_CODE];
                });
                $scope.MappedUsers = res.data;
            }
        }, function (ex) {
            displayPopupNotification("Cannot get Employees", "error");
        });
    }

    $scope.loginEmployeeSelect = {
        dataTextField: "LOGIN_SP_EDESC",
        dataValueField: "LOGIN_SP_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Select Employee...</strong></div>',
        placeholder: "Select Employee...",
        autoClose: false,
        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/getLoginEmployee",
                    dataType: "json"
                }
            }
        },
        select: function (e) {
            var PreviouslySelected = $scope.MappedUsers.map(a => a.LOGIN_SP_CODE ? a.LOGIN_SP_CODE[0] : "current");

            if (PreviouslySelected.includes(e.dataItem.LOGIN_SP_CODE.toString())) {
                e.preventDefault();
                displayPopupNotification("Employee already selected", "warning");
            }
        }
    };

    $scope.distEmployeeSelect = {
        dataTextField: "DIST_SP_EDESC",
        dataValueField: "DIST_SP_CODE",
        height: 600,
        valuePrimitive: true,
        //maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Select Employees...</strong></div>',
        placeholder: "Select Employees...",
        autoClose: false,
        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/getDistLoginEmployee",
                    dataType: "json"
                }
            }
        }
    };

    $scope.AddGroupMap = function () {
        $scope.MappedUsers.unshift({
            LOGIN_SP_CODE: null,
            MAPPED_USERS: null,
        });
    }

    $scope.deleteUser = function (idx) {
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

                    var data = $scope.MappedUsers[idx];
                    data.LOGIN_SP_CODE = data.LOGIN_SP_CODE[0];
                    var deleteUser = DistSetupService.deleteUserMap(data)
                    deleteUser.then(function (result) {
                        displayPopupNotification(result.data.MESSAGE, "success");
                        $scope.MappedUsers.splice(idx, 1);
                    }, function (result) {
                        displayPopupNotification(result.data.MESSAGE, "error");
                    });
                }
            }

        });


    }

    $scope.RemoveGroupMap = function (idx) {
        $scope.deleteUser(idx);
    }

    $scope.SaveDistUserMap = function () {
        var valid = $scope.ItemMapForm.$valid;
        if (!valid) {
            var errorName = $scope.ItemMapForm.$error.required[0].$name;
            $("[name=" + errorName + "]").data("kendoMultiSelect").open(); //$("#" + errorName) can also be used since Id and name are same
            displayPopupNotification("Invalid Input fields", "warning");
            return;
        }
        $scope.MappedUsers.forEach(function (val, key) {
            $scope.MappedUsers[key].LOGIN_SP_CODE = $scope.MappedUsers[key].LOGIN_SP_CODE[0];
        });
        DistSetupService.SaveDistUserMap($scope.MappedUsers).then(function (res) {
            mapedInView();
            displayPopupNotification(res.data.MESSAGE, res.data.TYPE);
        }, function (ex) {
            mapedInView();
            displayPopupNotification("Something went wrong", "error");
        });
    }

});