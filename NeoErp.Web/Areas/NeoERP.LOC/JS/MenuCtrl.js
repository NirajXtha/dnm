app.controller("dynamicCtrl", ["$scope", "menuService", function ($scope, menuService) {
    $scope.dynamicMenus = [];
    $scope.masterDynamicMenus = [];
    $scope.lcDynamicMenus = [];
    $scope.lcReportsDynamicMenus = [];

    var dynamicMenu = menuService.getData();
    dynamicMenu.then(function (response) {
        $scope.dynamicMenus = response.data;
    });

    var masterDynamicMenu = menuService.getmasterDynamicMenuData();
    masterDynamicMenu.then(function (response) {
        $scope.masterDynamicMenus = response.data;
    });

    var lcDynamicMenu = menuService.getlcDynamicMenuData();
    lcDynamicMenu.then(function (response) {
        $scope.lcDynamicMenus = response.data;
    });

    var lcReportsDynamicMenu = menuService.getlcReportsDynamicMenuData();
    lcReportsDynamicMenu.then(function (response) {
        $scope.lcReportsDynamicMenus = response.data;
    });

    $scope.colors = [
        '#C66', '#C93', '#F66', '#36C', '#C96', '#633', '#069', '#F90', '#6C3', '#666', '#a136c7', '#2583ce', '#da2c2c',
        '#00b4ac', '#009dd8', '#25b846', '#da2c2c', '#f27022', '#6438c8', '#fbbc11'
    ];

    $scope.randomColor = function (parentIndex, index, parentLast, elementLast) {
           if (parentLast && elementLast) {

            $('.ajax-loading').hide();
            $('[data-toggle="tooltip"]').tooltip();
            $(".icondescription").on("click", "span", function (e) {
                e.preventDefault();
                $(".icondescription span").hide();
                bootbox.confirm({
                    message: "Do you want to delete this menu? This cannot be undone.",
                    buttons: {
                        cancel: {
                            label: 'Cancel'
                        },
                        confirm: {
                            label: 'Confirm'
                        }
                    },
                    callback: function (result) {
                        if (result) {
                            var value = $(e.currentTarget).attr("data");
                            $.ajax({
                                type: 'POST',
                                dataType: "json",
                                url: window.location.protocol + "//" + window.location.host + "/Home/DeleteFavouriteMenu?menuName=" + value,
                                success: function (data) {
                                    if (data == 200) {
                                        $("a[data-name='" + value + "']").parent().remove();
                                        displayPopupNotification("Menu Deleted Successfully", "success");
                                    }
                                    else {
                                        displayPopupNotification("Error", "error");
                                    }
                                }
                            });
                        }

                    }
                });

            });
        }
        if (parentLast && _.isEmpty(elementLast)) {
            $('.ajax-loading').hide();
        }
    }
}]);
app.service("menuService", ["$http", "$q", "$timeout", function ($http, $q, $timeout) {
    return {
        getData: function () {
            // This could be $http or any other promise returning service.
            // Use a deferred and $timeout to simulate a network request.
            var deferred = $q.defer();
            var results = $http.get("/api/LocHome/GetDynamicMenu")
                .success(function (data) {
                    return data.data;
                });
            deferred.resolve(results);
            return deferred.promise;
        },
        getmasterDynamicMenuData: function () {
            var deferred = $q.defer();
            var results = $http.get("/api/LocHome/getmasterDynamicMenuData")
                .success(function (data) {
                    return data.data;
                });
            deferred.resolve(results);
            return deferred.promise;
        },
        getlcDynamicMenuData: function () {
            var deferred = $q.defer();
            var results = $http.get("/api/LocHome/getlcDynamicMenuData")
                .success(function (data) {
                    return data.data;
                });
            deferred.resolve(results);
            return deferred.promise;
        },
        getlcReportsDynamicMenuData: function () {
            var deferred = $q.defer();
            var results = $http.get("/api/LocHome/getlcReportsDynamicMenuData")
                .success(function (data) {
                    return data.data;
                });
            deferred.resolve(results);
            return deferred.promise;
        }
    };
}]);