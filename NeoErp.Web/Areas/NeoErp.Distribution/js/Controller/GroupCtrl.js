
distributionModule.controller('GroupCtrl', function ($scope, DistSetupService) {
    $scope.saveAction = "Save";
    $scope.Group = {
        GROUP_CODE: '',
        GROUP_EDESC: '',
        GROUPID:''
    };
    $scope.GroupHeader = "Add Group";
    $scope.grid = {
        change: GroupChange,
        dataSource: {
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/Setup/getAllResellerGroups",
            },
            pageSize: 20
        },
        dataBound: function (e) {
            GetSetupSetting("GroupSetup");
        },
        height: window.innerHeight - 50,
        groupable: false,
        sortable: false,
        height: 400,
        selectable: true,
        columns: [
        {
            field: "GROUP_EDESC",
            title: "Group Name",
            width: 150
        }]

    }
    function GroupChange(evt) {
        
        selectedRow = this.select();
        var item = this.dataItem(selectedRow);

        if (item) {
            $scope.saveAction = "Update";
            $scope.GroupHeader = "Update Group";

            $scope.Group.GROUP_EDESC = item.GROUP_EDESC;
            $scope.Group.GROUP_CODE = item.GROUP_CODE;
            $scope.Group.GROUPID = item.GROUPID;
            $scope.$apply();
        }
    }

    $scope.SaveGroup = function (isValid) {
        if (!isValid) {
            displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
            return;
        }
        DistSetupService.AddResellerGroup($scope.Group).then(function (result) {
            displayPopupNotification(result.data.MESSAGE, result.data.TYPE);
            $("#grid").data("kendoGrid").dataSource.read();
            $scope.Cancel();
        }, function (result) {
            displayPopupNotification(result.data.MESSAGE, "error");
        });
    }

    $scope.deleteGroup = function () {
        
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
                    var GROUP_EDESC = $("#GroupName").val();
                    var GROUP_CODE = $("#GroupCode").val();
                    var GROUPID;
                    if ($scope.GROUPID == undefined) {
                        GROUPID = $scope.Group.GROUPID;
                    }

                    var data = {
                        GROUP_EDESC: GROUP_EDESC,
                        GROUP_CODE: GROUP_CODE,
                        GROUPID: GROUPID,
                    };

                     var deleteGroup = DistSetupService.deleteGroup(data)
                    deleteGroup.then(function (result) {
                        displayPopupNotification(result.data.MESSAGE, "success");
                        $("#grid").data("kendoGrid").dataSource.read();
                        $scope.Cancel();
                    }, function (result) {
                        displayPopupNotification(result.data.MESSAGE, "error");
                    });
                }
            }

        });
      

    }

    $scope.Cancel = function () {
        $scope.saveAction = "Save";
        $scope.GroupHeader = "Add Group";
        $scope.Group = {
            GROUP_CODE: '',
            GROUP_EDESC: '',
            GROUPID: ''
        };
        $("#grid").data("kendoGrid").clearSelection();
    }
});

//group mapping controller
distributionModule.controller('GroupMapCtrl', function ($scope, DistSetupService, $routeParams) {

    $scope.MappedGroups = [];

    $scope.$on('$routeChangeSuccess', function () {
        
        $scope.AddGroupMap();
        DistSetupService.GetAllGroupMaps().then(function (res) {
            if (res.data.length > 0) {
                res.data.forEach(function (val, key) {
                    res.data[key].GROUP_CODE = [res.data[key].GROUP_CODE];
                });
                $scope.MappedGroups = res.data;
            }
        }, function (ex) {
            displayPopupNotification("Cannot get groups", "error");
        });
    });

    $scope.distGroupSelect = {
        dataTextField: "GROUP_EDESC",
        dataValueField: "GROUPID",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Select a group...</strong></div>',
        placeholder: "Select a group...",
        autoClose: false,
        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/getAllResellerGroups",
                    dataType: "json"
                }
            }
        },
        select: function (e) {
            
            var PreviouslySelected = $scope.MappedGroups.map(a =>  a.GROUP_CODE ? a.GROUP_CODE[0] : "current");

            if (PreviouslySelected.includes(e.dataItem.GROUPID.toString())) {
                e.preventDefault();
                displayPopupNotification("Item already selected", "warning");
            }
        }
    };

    $scope.distGroupMapSelect = {
        dataTextField: "GROUP_EDESC",
        dataValueField: "GROUPID",
        height: 600,
        valuePrimitive: true,
        //maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Select groups...</strong></div>',
        placeholder: "Select groups...",
        autoClose: false,
        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/getAllResellerGroups",
                    dataType: "json"
                }
            }
        }
    };

    $scope.AddGroupMap = function () {
        $scope.MappedGroups.unshift({
            GROUP_CODE: null,
            MAPPED_GROUPS: null,
        });
    }

    $scope.RemoveGroupMap = function (idx) {
        
        $scope.MappedGroups.splice(idx, 1);
    }

    $scope.SaveGroupMap = function () {
        
        var valid = $scope.ItemMapForm.$valid;
        if (!valid) {
            var errorName = $scope.ItemMapForm.$error.required[0].$name;
            $("[name=" + errorName + "]").data("kendoMultiSelect").open(); //$("#" + errorName) can also be used since Id and name are same
            displayPopupNotification("Invalid Input fields", "warning");
            return;
        }
        $scope.MappedGroups.forEach(function (val, key) {
            $scope.MappedGroups[key].GROUP_CODE = $scope.MappedGroups[key].GROUP_CODE[0];
        });
        DistSetupService.SaveGroupMap($scope.MappedGroups).then(function (res) {
            displayPopupNotification(res.data.MESSAGE, res.data.TYPE);
        }, function (ex) {
            displayPopupNotification("Something went wrong", "error");
        });
    }

});