PMModule.controller('indexMuCtrl', function ($scope, $http, $routeParams, $window, $filter) {
    $scope.indexMuDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllIndexMuFilter",
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
    $scope.indexMuOptions = {
        dataSource: $scope.indexMuDataSource,
        filter: "contains",
        dataTextField: 'MU_EDESC',
        dataValueField: 'MU_CODE',
        optionLabel:'--Select MU--',
        dataBound: function (e) {

        }
    }
});