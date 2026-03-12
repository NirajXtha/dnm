

DTModule.controller('agentSetupTreeCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    $scope.saveAction = "Save";
    $scope.btnDelete = false;

    $scope.Agent = {
        AGENT_CODE: "",
        AGENT_EDESC: "",
        AGENT_NDESC: "",
        AGENT_TYPE: "",
        REMARKS: "",
        CREDIT_LIMIT: "",
        CREDIT_DAYS: "",
        CREATED_BY: "",
        CREATED_DATE: "",
        COMPANY_CODE: "",
        BRANCH_CODE: "",
        DELETED_FLAG: "",
        MODIFY_DATE: "",
        MODIFY_BY: "",
        AGENT_ID: "",
        PAN_NO: "",
        ADDRESS: "",
        AGENTID: ""
    };
    $scope.AgentHeader = "Add Agent";
    $scope.grid = {
        change: AgentChange,
        dataSource: {
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAgentCodeWithChild",
            },
            pageSize: 20
        },
        dataBound: function (e) {
            //GetSetupSetting("AgentSetup");
        },
        //height: window.innerHeight - 50,
        groupable: false,
        sortable: false,
        height: 470,
        selectable: true,
        scrollable: {
            virtual: true
        },
        columns: [
            {
                field: "AGENT_EDESC",
                title: "Agent Name",
                width: 150
            }]

    }
    function AgentChange(evt) {
        debugger
        selectedRow = this.select();
        var item = this.dataItem(selectedRow);
        if (item) {
            $scope.saveAction = "Update";
            $scope.AgentHeader = "Update Agent";
            $scope.Agent.AGENT_EDESC = item.AGENT_EDESC;
            $scope.Agent.AGENT_NDESC = item.AGENT_NDESC;
            $scope.Agent.AGENT_CODE = item.AGENT_CODE;
            $scope.Agent.REMARKS = item.REMARKS;
            $scope.Agent.AGENTID = item.AGENTID;
            $scope.Agent.AGENT_TYPE = item.AGENT_TYPE;
            $scope.Agent.CREDIT_LIMIT = item.CREDIT_LIMIT;
            $scope.Agent.CREDIT_DAYS = item.CREDIT_DAYS;
            $scope.Agent.AGENT_ID = item.AGENT_ID
            $scope.Agent.PAN_NO = item.PAN_NO;
            $scope.Agent.ADDRESS = item.ADDRESS;
            $scope.btnDelete = true;
            $scope.$apply();
        }
    }

    $scope.getMaxAgentCode = function () {
        var getMaxCodeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getMaxAgentCode";
        $http({
            method: 'Get',
            url: getMaxCodeUrl,
        }).then(function successCallback(response) {
            $scope.Agent.AGENT_CODE = response.data.DATA;
        }, function errorCallback(response) {
            displayPopupNotification(response.data.STATUS_CODE, "error");
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
    }

    $scope.getMaxAgentCode();

    $scope.SaveAgent = function (isValid) {

        var model = {
            AGENT_CODE: $scope.Agent.AGENT_CODE,
            AGENT_EDESC: $scope.Agent.AGENT_EDESC,
            AGENT_NDESC: $scope.Agent.AGENT_NDESC,
            REMARKS: $scope.Agent.REMARKS,
            AGENT_TYPE: $scope.Agent.AGENT_TYPE,
            CREDIT_LIMIT: $scope.Agent.CREDIT_LIMIT,
            CREDIT_DAYS: $scope.Agent.CREDIT_DAYS,
            AGENT_ID: $scope.Agent.AGENT_ID,
            PAN_NO: $scope.Agent.PAN_NO,
            ADDRESS: $scope.Agent.ADDRESS
        }
        debugger
        if (!isValid) {
            displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
            return;
        }
        // $scope.creditLimitChecker();
        if ($scope.saveAction == "Save") {
            var saveAgentUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewAgentSetup";
            $http({
                method: 'POST',
                url: saveAgentUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "INSERTED") {
                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully saved ", "success");
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Agent name already exist please try another agent name.", "error");
                }

                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // this callback will be called asynchronously
                // when the response is available
            }, function errorCallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Agent name already exist please try another agent name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
        }
        else {
            //  $scope.creditLimitChecker();
            var udpateAgentUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateAgentSetup";
            $http({
                method: 'POST',
                url: udpateAgentUrl,
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
        debugger
        const searchText = ($scope.txtMainGroupSearchString || '').toLowerCase();

        var dataSource = $("#grid").data("kendoGrid").dataSource;

        if (searchText) {
            dataSource.filter({
                field: "AGENT_EDESC",
                operator: "contains",
                value: searchText
            });
        } else {
            dataSource.filter({});
        }
    };


    $scope.deleteAgent = function (isValid) {
        if (!isValid) {
            displayPopupNotification("There is no any agent selected.", "warning");
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
                    var AGENT_EDESC = $("#AgentName").val();
                    var AGENT_CODE = $("#AgentCode").val();
                    var AGENTID;
                    if (AGENTID == undefined) {
                        AGENTID = $scope.Agent.AGENT_CODE;
                    }
                    var deleteAgentUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/deleteAgentSetup?agentCode=" + AGENTID;
                    $http({
                        method: 'POST',
                        url: deleteAgentUrl,
                    }).then(function successCallback(response) {
                        if (response.data.MESSAGE == "DELETED") {
                            $("#grid").data("kendoGrid").dataSource.read();
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

                $("#grid").data("kendoGrid").dataSource.read();
                $scope.Cancel();
            }

        });


    }

    $scope.creditLimitChecker = function () {
      
        var creditLimit = $scope.Agent.CREDIT_LIMIT;
        var creditDays = $scope.Agent.CREDIT_DAYS;
        var stringCreditDays = creditDays.toString();
        var limitCreditDaysToFiveDigit = stringCreditDays.substring(0, 5);
        var creditDaysLen = parseInt(limitCreditDaysToFiveDigit);
        if (creditDays > creditLimit) {
            $scope.Agent.CREDIT_DAYS = "";
            // $scope.Agent.CREDIT_LIMIT = "";
            displayPopupNotification("Credit days can not be greater than credit limit.", "warning");
            return;
        }
        if (stringCreditDays.length > 5) {
            $scope.Agent.CREDIT_DAYS = creditDaysLen;
            displayPopupNotification("Credit Days can not greater then 5 digit.", "warning");
            return;
        }

    }

    $scope.Cancel = function () {
        $scope.saveAction = "Save";
        $scope.AgentHeader = "Add Agent";
        $scope.Agent = {
            AGENT_CODE: '',
            AGENT_EDESC: '',
            AGENT_NDESC: '',
            AGENTID: ''
        };
        $("#grid").data("kendoGrid").clearSelection();
        $scope.getMaxAgentCode();
        $scope.btnDelete = false;
    }


});

