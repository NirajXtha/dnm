planningModule.controller("menuCtrl", ['$scope', 'menuService', function ($scope, menuService) {
    $scope.tn = "menu test";
    $scope.menuList = null;
    $scope.moduleCode = '30';
    menuService.getMenus($scope.moduleCode).then(function (res) {
        $scope.menuList = res.data;
    });

    menuService.getFavroiteMenu($scope.moduleCode).then(function (response) {
        $('.ajax-loading').show();
        response.data = response.data == "" ? null : response.data;
        $scope.allFavroiteMenuList = JSON.parse(response.data);
        $scope.favroiteMenuList = _.filter($scope.allFavroiteMenuList, function (item) {
            if (item.Report.parentMenu == "Favourite") {
                return item;
            }
        });

        $scope.colors = [
            '#C66', '#C93', '#F66', '#36C', '#C96', '#633', '#069', '#F90', '#6C3', '#666', '#a136c7', '#2583ce', '#da2c2c',
            '#00b4ac', '#009dd8', '#25b846', '#da2c2c', '#f27022', '#6438c8', '#fbbc11'
        ];
        document.distGlobalFavourite = $scope.favroiteMenuList;
        $scope.randomColor = function (parentIndex, index) {
            var element = $($($(".erp-iconlist")[parentIndex]).find("ul").first().find("li")[index]).find(".circle");
            var span = "";
            element.bind("contextmenu", function (event) {
                event.preventDefault();
                $(".icondescription span").hide();
                span = event.target.nextElementSibling.children;
                $(span).show();
                $(span).attr("data", event.target.parentElement.nextElementSibling.textContent);
            });
        }
    });
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
    $scope.getMenuLink = function (menuName, virtualPath, fav) {
        if (fav != undefined)
            return '/' + virtualPath + "?fav=" + fav;
        else
            return '/' + virtualPath;


    }
    $scope.modelABBRColor = function (modelCode) {
        if (modelCode == 'SA')
            return "#43a12e";
        else if (modelCode == 'AC')
            return "#3c763d";
        else if (modelCode == 'AR')
            return "#4480a4";
        else if (modelCode == 'PR')
            return "#666";
        else if (modelCode == 'ST')
            return "#31708f";
        else if (modelCode == 'FA')
            return "#5aa9d7";
        else if (modelCode == 'NA')
            return "#45b6b6";
        else
            return "#b6a845";
    }
}]);

planningModule.factory('menuService', ['$http', function ($http) {
    var fac = {};
    fac.getMenus = function (moduleCode) {
        //return $http.get('/api/PlanService/GetAllPlan');
        return $http.get('/api/SalesHome/GetDynamicMenu?ModuleCode=' + moduleCode)
    }
    fac.getFavroiteMenu = function (moduleCode) {
        return $http.get(window.location.protocol + "//" + window.location.host + "/Home/GetFavroiteMenus?moduleCode=" + moduleCode)
    }
    return fac;
}])