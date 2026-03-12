PMModule.controller('documentTypeCtrl', function ($scope, $http, $routeParams, $window, $filter) {

    $scope.documentTypeDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllDocumentTypeListByFilter",

            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
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



    $scope.documentTypeOption = {
        dataSource: $scope.documentTypeDataSource,
        template: '<span>{{dataItem.TYPE_EDESC}}</span>  --- ' +
                  '<span>{{dataItem.Type}}</span>',
        dataBound: function (e) {
            var division = $("#documenttype").data("kendoComboBox");
            if (division != undefined) {
                division.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(TYPE_EDESC, this.text()) #"), division)
                });
            }
        }
    }


    $scope.documentTypeCodeOnChange = function (kendoEvent) {
        
        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.documenttypeerror = "Please Enter Valid Code."
            $('#documenttype').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.documenttypeerror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }

});