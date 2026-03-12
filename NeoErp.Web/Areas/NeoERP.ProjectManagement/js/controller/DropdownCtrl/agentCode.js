
PMModule.controller('agentCtrl', function ($scope, $http, $routeParams, $window, $filter) {

    $scope.agentDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAgentListByFlter",

            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        if (data.filter.filters[0].value != "") {
                            newParams = {
                                filter: data.filter.filters[0].value
                            };
                            return newParams;
                        }
                        else {
                            newParams = {
                                filter: "!@$"
                            };
                            return newParams;
                        }
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        }
    };

    $scope.AgentCodeOption = {
        dataSource: $scope.agentDataSource,
        dataBound: function (e) {
            var agent = $("#agent").data("kendoComboBox");
            if (agent != undefined) {
                agent.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(AGENT_EDESC,AGENT_TTPE,  this.text()) #"), agent)
                });
            }
        }
    }

    $scope.agentCodeOnChange = function (kendoEvent) {

        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.agenterror = "Please Enter Valid Code."
            $('#agent').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.agentyerror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }

});