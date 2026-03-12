
distributionModule.controller('OutletCtrl', function ($scope, DistSetupService) {
    $scope.saveAction = "Save";
    $scope.outletHeader = "ADD OUTLET";

    $scope.grid = {
        change: outletChange,
        dataSource: {
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/Setup/getAllOutletList",
            },
            //  pageSize: 20
        },
        dataBound: function (e) {
            GetSetupSetting("GetOutlet");
        },
        height: window.innerHeight - 50,
        groupable: false,
        sortable: false,
        scrollable: {
            virtual: true
        },
        height: 400,
        selectable: true,
        columns: [
            {
                field: "TYPE_EDESC",
                title: "Outlet Name",
                width: 150,
            }]
    }
    function outletChange(evt) {
        // var DISTRICT_CODE = evt.sender.value()[0];
        $scope.saveAction = "Update";
        $scope.outletHeader = "UPDATE OUTLET";
        selectedRow = this.select();
        var item = this.dataItem(selectedRow);
        var TYPE_ID = item.TYPE_ID;

        $scope.outletName = [item.TYPE_EDESC];
        $scope.outletCode = [item.TYPE_CODE];
        $scope.typeId = [item.TYPE_ID];

        $scope.OutletsForm.outletName.$invalid = false;
        $scope.OutletsForm.outletName.$error.required = false;
        $scope.OutletsForm.outletCode.$invalid = false;
        $scope.OutletsForm.outletCode.$error.required = false;


        var Getdata = DistSetupService.getSubTypeList(TYPE_ID)
        Getdata.then(function (response) {
            $scope.Outlets = response.data;
            //var SN = '1';
            //$scope.Outlets.push({SN})
            //for(i=0;i<$scope.Outlets.length;i++)
            //{
            //    ("Outlets"+$scope.Outlets[i].outletName+"SN"+$scope.Outlets.SN+1)
            //}

        })


    }

    //$scope.Outlets = [{SN:'1', SUBTYPE_CODE: '', SUBTYPE_EDESC: '' }];
    $scope.Outlets = [];
    $scope.AddOutlet = function () {
        var available = $scope.Outlets;
        var i = $scope.Outlets.length;
        $scope.Outlets = [];
        $scope.Outlets.push({
            SN: i + 1,
            SUBTYPE_CODE: '',
            SUBTYPE_EDESC: ''
        });
        for (var i = 0; i < available.length; i++) {
            var item = available[i];
            $scope.Outlets.push(item);
        }
    }

    $scope.RemoveOutlet = function (index) {

        for (var i = 0; i < $scope.Outlets.length; i++) {
            $scope.Outlets[i].SN = $scope.Outlets.length - i;

        }
        if ($scope.Outlets.length != 0) {
            $scope.Outlets.splice(index, 1);
        }

    };

    $scope.addItem = function (isValid) {
        if (!isValid) {
            displayPopupNotification("Invalid Field", "warning");
            return;
        }
        var TYPE_EDESC = $("#outletName").val();
        var TYPE_CODE = $("#outletCode").val();
        var TYPE_ID;
        if ($scope.typeId != undefined) {
            TYPE_ID = $scope.typeId[0];
        }

        var subtypeArr = $scope.Outlets

        var data = {
            TYPE_EDESC: TYPE_EDESC,
            TYPE_CODE: TYPE_CODE,
            TYPE_ID: TYPE_ID,
            subtypeArr: subtypeArr,

        }
        var getData = DistSetupService.AddOutlet(data);
        getData.then(function (result) {
            if (result.data.STATUS_CODE === 200) {
                displayPopupNotification(result.data.MESSAGE, "success");
                $("#grid").data("kendoGrid").dataSource.read();
                // $scope.Outlets = [{ SN: '1', SUBTYPE_CODE: '', SUBTYPE_EDESC: '' }];
                $scope.CancelledItem();
            }
        }, function () {
            displayPopupNotification(result.data.MESSAGE, "error");
        });
    }

    $scope.deleteItem = function () {
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
                    var TYPE_ID;
                    if ($scope.typeId != undefined) {
                        TYPE_ID = $scope.typeId[0];
                    }
                    var TYPE_EDESC = $("#outletName").val();
                    var TYPE_CODE = $("#outletCode").val();

                    var data = {
                        TYPE_ID: TYPE_ID,
                        TYPE_EDESC: TYPE_EDESC,
                        TYPE_CODE: TYPE_CODE,

                    };

                    var updateData = DistSetupService.deleteItem(data);
                    updateData.then(function (result) {
                        if (result.data.STATUS_CODE === 200) {
                            displayPopupNotification(result.data.MESSAGE, "Success");
                            $("#grid").data("kendoGrid").dataSource.read();
                            $scope.CancelledItem();

                        };


                    }, function () {
                        displayPopupNotification(result.data.MESSAGE, "error");
                    });
                }

            }
        });


    }

    $scope.CancelledItem = function () {
        $scope.saveAction = "Save";
        $scope.outletHeader = "ADD OUTLET";
        $scope.typeId = undefined;
        $scope.outletName = [];
        $scope.outletCode = [];
        $scope.Outlets = [];
    }


});

