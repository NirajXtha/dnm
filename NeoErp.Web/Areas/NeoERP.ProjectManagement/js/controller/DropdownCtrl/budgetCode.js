
PMModule.controller('budgetCtrl', function ($scope, $http, $routeParams, $window, $filter) {
    $scope.budgetCenterDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllBudgetCenterByFilter"
            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value,
                            accCode: accCode
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: "",
                            accCode: accCode
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: "",
                        accCode: accCode
                    };
                    return newParams;
                }
            }
        },
    }
});

