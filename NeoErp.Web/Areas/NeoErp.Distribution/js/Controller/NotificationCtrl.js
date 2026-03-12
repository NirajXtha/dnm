distributionModule.controller('NotificationCtrl', function ($scope, DistSetupService) {

    $scope.HeadText = "New Notification";

    $scope.Notification = {
        NOTIFICATION_ID: '',
        NOTIFICATION_TITLE: '',
        NOTIFICATION_TEXT: '',
        NOTIFICATION_TYPE: '',
        SP_CODES: [],
    }

    $scope.grid = {
        dataSource: {
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/Setup/GetNotifications",
            },
            //  pageSize: 20
        },
        dataBound: function (e) {
            GetSetupSetting("GetNotification");
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
            field: "NOTIFICATION_TITLE",
            title: "Notifications",
            width: 150,
        }],
        change: function (evt) {
            selectedRow = this.select();
            var item = this.dataItem(selectedRow);
            $scope.Notification = {
                NOTIFICATION_ID: item.NOTIFICATION_ID,
                NOTIFICATION_TITLE: item.NOTIFICATION_TITLE,
                NOTIFICATION_TEXT: item.NOTIFICATION_TEXT,
                NOTIFICATION_TYPE: item.NOTIFICATION_TYPE,
                SP_CODES: item.SP_CODE ? item.SP_CODE.split(",") : [""],
            }
            $scope.HeadText = "Update Notification";
            $scope.$apply();
        }
    }

    $scope.EmpSelectOptions = {
        dataTextField: "EMPLOYEE_EDESC",
        dataValueField: "EMPLOYEE_CODE",
        height: 300,
        valuePrimitive: true,
        //maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-4"><strong>..Employee..</strong></div>',
        placeholder: "Select Employee...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/UserSetup/GetSalesPersonList",
                    dataType: "json"
                }
            },
            schema: {
                parse: function (data) {
                    data.unshift({
                        COMPANY_CODE: null,
                        EMPLOYEE_CODE: "",
                        EMPLOYEE_EDESC: "All Employees",
                        EMPLOYEE_NDESC: null
                    });
                    return data;
                }
            }
        },
    };

    $scope.Cancel = function () {
        $scope.Notification = {
            NOTIFICATION_ID: '',
            NOTIFICATION_TITLE: '',
            NOTIFICATION_TEXT: '',
            NOTIFICATION_TYPE: '',
            SP_CODES: null,
        }
        $scope.HeadText = "New Notification";
        //clear selected row
        $("#grid .k-state-selected").removeClass("k-state-selected");
    }

    $scope.Save = function (valid) {
        if (valid) {
            var a = $scope.Notification;
            DistSetupService.AddNotification($scope.Notification).then(function (res) {
                displayPopupNotification(res.data.MESSAGE, res.data.TYPE);
                if (res.data.TYPE == "success") {
                    $scope.Cancel();
                    $("#grid").data("kendoGrid").dataSource.read();
                }
            }, function (ex) {
                displayPopupNotification("Sorry error occured while processing data", "error");
            });
        }
        else
            displayPopupNotification("Invalid input fileds", "warning");
    }

    $scope.Delete = function (id) {
        if (id) {
            bootbox.confirm("<strong>This notification will be removed</strong><br /> Are you sure want to remove?", function (result) {
                if (result) {
                    DistSetupService.DeleteNotification(id).then(function (res) {
                        displayPopupNotification(res.data.MESSAGE, res.data.TYPE);
                        if (res.data.TYPE == "success") {
                            $scope.Cancel();
                            $("#grid").data("kendoGrid").dataSource.read();
                        }
                    }, function (ex) {
                        displayPopupNotification("Error processing request", "error");
                    });
                }
            });
        }
    }
});