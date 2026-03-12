DTModule.controller('indexMuCtrl', function ($scope, $http, $routeParams, $window, $filter) {
    $scope.indexMuDataSource = {
        serverFiltering: true,
        transport: {
            read: {
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                type: "GET",
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