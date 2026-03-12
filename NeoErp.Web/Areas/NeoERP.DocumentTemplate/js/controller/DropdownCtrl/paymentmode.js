
DTModule.controller('paymentmodeCtrl', function ($scope, $http, $routeParams, $window, $filter) {

    $scope.paymentmodeDataSource = {
        serverFiltering: true,
        transport: {
            read: {
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                type: "GET",
                url: "/api/TemplateApi/GetPaymentModeListByFlter",
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
  




});
