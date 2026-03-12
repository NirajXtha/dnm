DTModule.controller('purchaseTypeCtrl', function ($scope, $http, $routeParams, $window, $filter) {

    $scope.purchaseTypeDataSource = {
        serverFiltering: true,
        transport: {
            read: {
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                type: "GET",
                url: "/api/TemplateApi/GetPurchaseTypeList",
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

    $scope.PurchaseTypeOnChange = function (kendoEvent) {
        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.ptypeerror = "Please Enter Valid Purchase Type.";
            $('#ptype').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.ptypeerror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    };

    $scope.PurchaseTypeOnSelect = function (kendoEvent) {
        // Store previous value if needed
    };
});
