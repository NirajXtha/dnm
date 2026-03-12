
PMModule.controller('prorityCtrl', function ($scope, $http, $routeParams, $window, $filter) {

    $scope.prorityDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetPriorityListByFlter",

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

    $scope.PriorityCodeOption = {
        dataSource: $scope.prorityDataSource,
        dataBound: function (e) {
            var priority = $("#prority").data("kendoComboBox");
            if (priority != undefined) {
                priority.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(PRIORITY_EDESC,Type,  this.text()) #"), priority)
                });
            }
        }
    }

    $scope.prorityCodeOnChange = function (kendoEvent) {
        
        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.prorityerror = "Please Enter Valid Code."
            $('#prority').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.prorityerror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }

});