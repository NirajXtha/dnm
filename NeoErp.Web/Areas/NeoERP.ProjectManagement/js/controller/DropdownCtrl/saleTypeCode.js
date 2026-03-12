PMModule.controller('saleTypeCtrl', function ($scope, $http, $routeParams, $window, $filter) {

    $scope.salesTypeSetupDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetsaleTypeListByFilter",
            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        //newParams = {
                        //    filter: data.filter.filters[0].value
                        //};
                        //return newParams;
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

    
    $scope.SalesTypeCodeOption = {
        dataSource: $scope.salesTypeSetupDataSource,
        dataBound: function (e) {
            var salestype = $("#salestype").data("kendoComboBox");
            if (salestype != undefined) {
                salestype.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(SALES_TYPE_EDESC, this.text()) #"), salestype)
                });
            }
        }
    }


    $scope.salestypeCodeOnChange = function (kendoEvent) {
  
        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.salestypeerror = "Please Enter Valid Code."
            $('#salestype').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.salestypeerror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }


});

