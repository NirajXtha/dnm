DTModule.controller('issueTypeSetupCtrl', function ($scope, $http, $window) {
   // alert("hello");

    $scope.FormName = "Issue Type";
    $scope.saveAction = "Save";
    $scope.btnDelete = false;

    $scope.IssueTypeSetupObj = {
        SHORT_CUT: "",
        IN_NEPALI: "",
        IN_ENGLISH: "",
        REMARKS: "",

    };



    var getSavedIssueType = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetSavedIssueType";
    $scope.iGrid = {
        change: IssueTypeChange,
        dataSource: {
            transport: {
                read: getSavedIssueType,
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
                field: "ISSUE_TYPE_EDESC",
                title: "Issue Type",
                width: 150
            }]

    }


    function IssueTypeChange(evt) {

        selectedRow = this.select();
        var item = this.dataItem(selectedRow);
        console.log("Item=======================<<<<<" + JSON.stringify(item));
        if (item) {
            $scope.saveAction = "Update";
           // $scope.TransporterHeader = "Update Vehicle";

            $scope.IssueTypeSetupObj.SHORT_CUT = item.ISSUE_TYPE_CODE;
            $scope.IssueTypeSetupObj.IN_ENGLISH = item.ISSUE_TYPE_EDESC;
            $scope.IssueTypeSetupObj.IN_NEPALI = item.ISSUE_TYPE_NDESC;
            $scope.IssueTypeSetupObj.REMARKS = item.REMARKS;
            $scope.btnDelete = true;
            $scope.$apply();
        }
    }


    $scope.SaveIssueType = function (isValid) {
        var model = {
            ISSUE_TYPE_CODE: $scope.IssueTypeSetupObj.SHORT_CUT,
            ISSUE_TYPE_EDESC: $scope.IssueTypeSetupObj.IN_ENGLISH,
            ISSUE_TYPE_NDESC: $scope.IssueTypeSetupObj.IN_NEPALI,
            REMARKS: $scope.IssueTypeSetupObj.REMARKS,
        }



        var validation = [
            { short_cut: $scope.IssueTypeForm.short_cut.$invalid },
            { in_english: $scope.IssueTypeForm.in_english.$invalid },
        ]

        if (validation[0].short_cut == true) {

            displayPopupNotification("Please provide type shortcut", "warning");
            return
        }
        if (validation[1].in_english == true) {

            displayPopupNotification(" Please provide name in english", "warning");

            return
        }

        if ($scope.saveAction == "Save") {
            var saveTypeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/SaveIssueType";
            $http({
                method: 'POST',
                url: saveTypeUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "INSERTED") {
                    $("#iGrid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully saved ", "success");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            }, function errorCallback(response) {
                displayPopupNotification("Something went wrong.Please try again later.", "error");

            });
        }
        else if ($scope.saveAction == "Update") {
            var udpateVehicleUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/UpdateIssueTypeSetup";
            var model1 = {
                ISSUE_TYPE_CODE: $scope.IssueTypeSetupObj.SHORT_CUT,
                ISSUE_TYPE_EDESC: $scope.IssueTypeSetupObj.IN_ENGLISH,
                ISSUE_TYPE_NDESC: $scope.IssueTypeSetupObj.IN_NEPALI,
                REMARKS: $scope.IssueTypeSetupObj.REMARKS,
            }
            $http({
                method: 'POST',
                url: udpateVehicleUrl,
                data: model1
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "UPDATED") {
                    $("#iGrid").data("kendoGrid").dataSource.read();
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


    $scope.deleteIssueType = function () {
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
                    var ISSUE_TYPE_EDESC = $("#in_english").val();
                    var ISSUE_TYPE_CODE = $("#short_cut").val();
                    if (ISSUE_TYPE_CODE == undefined) {
                        ISSUE_TYPE_CODE = $scope.IssueTypeSetupObj.SHORT_CUT;
                    }
                    var deleteTypeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteIssueTypeSetup?issueTypeCode=" + ISSUE_TYPE_CODE;
                    $http({
                        method: 'POST',
                        url: deleteTypeUrl,
                    }).then(function successCallback(response) {
                        if (response.data.MESSAGE == "DELETED") {
                            $("#iGrid").data("kendoGrid").dataSource.read();
                            $scope.Cancel();
                            displayPopupNotification("Data succesfully deleted ", "success");
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
                $("#iGrid").data("kendoGrid").dataSource.read();
                $scope.Cancel();
            }

        });


    }


    $scope.Cancel = function () {
        $scope.saveAction = "Save";
        $scope.IssueTypeSetupObj = {
            SHORT_CUT: "",
            IN_ENGLISH: "",
            IN_NEPALI: "",
            REMARKS: "",

        };

        $scope.btnDelete = false;
        $("#iGrid").data("kendoGrid").clearSelection();

    }

});