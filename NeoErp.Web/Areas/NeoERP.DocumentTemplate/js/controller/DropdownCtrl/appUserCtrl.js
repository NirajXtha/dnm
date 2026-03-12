DTModule.controller('appuserCtrl', function ($scope, $http, $routeParams, $window, $filter) {

    $scope.userDataSource = {
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetUserListByFlter",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                type: "GET",
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