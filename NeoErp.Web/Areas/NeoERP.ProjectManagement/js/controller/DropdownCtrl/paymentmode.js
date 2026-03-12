
PMModule.controller('paymentmodeCtrl', function ($scope, $http, $routeParams, $window, $filter) {

    $scope.paymentmodeDataSource = {

        type: "json",
        serverFiltering: true,
        transport: {
            read: {
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
