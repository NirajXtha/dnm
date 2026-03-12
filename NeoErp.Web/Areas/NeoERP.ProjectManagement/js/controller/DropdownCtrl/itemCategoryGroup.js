PMModule.controller('itemCategoryGroup', function ($scope, $http, $routeParams, $window, $filter) {
    $scope.itemCategoryDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllItemCategoryFilter",
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
    $scope.categoryGroupOptions = {
        dataSource: $scope.itemCategoryDataSource,
        dataTextField: 'CATEGORY_EDESC',
        dataValueField: 'CATEGORY_CODE',
        filter: "contains",
        optionLabel: '--Select Category--',
        dataBound: function (e) {
           
        }
    }
});