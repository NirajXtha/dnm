//var moduleCode = "10";
//distributionModule.controller('dynamicCtrl', ['$scope', '$rootScope', 'distributorService', function ($scope, $rootScope, distributorService) {

//    //get dynamic menus



//    var Getdata = distributorService.GetDynamicMenu()
//    Getdata.then(function (response) {
//        $scope.dynamicMenu = response.data;
//        //var SN = '1';
//        //$scope.Outlets.push({SN})
//        //for(i=0;i<$scope.Outlets.length;i++)
//        //{
//        //    ("Outlets"+$scope.Outlets[i].outletName+"SN"+$scope.Outlets.SN+1)
//        //}

//    });


//    var dynamicMenu = distributorService.getData();
//    dynamicMenu.then(function (response) {
//        $scope.dynamicMenus = response.data;
//        $scope.dynamicDashboardMenus = response.data;
//        //add favourite menu to tabular report
//        if (!_.isEmpty($scope.dynamicDashboardMenus) && $scope.dynamicDashboardMenus.length > 0 && !_.isEmpty($scope.allFavroiteMenuList) && $scope.allFavroiteMenuList.length > 0) {
//            $.each($scope.dynamicDashboardMenus, function (index, obj) {
//                var temp = _.filter($scope.allFavroiteMenuList, function (item) {
//                    if (item.Report.parentMenu == obj.MENU_EDESC) {
//                        return item;
//                    }
//                });
//                if (temp.length > 0) {
//                    $.each(temp, function (i, o) {
//                        var ob = {
//                            GROUP_SKU_FLAG: 'I',
//                            ICON_PATH: o.Report.icon,
//                            MENU_EDESC: o.Report.reportName,
//                            VIRTUAL_PATH: o.Report.virtualPath + "?fav=" + o.Report.reportName,
//                            COLOR: o.Report.color,
//                            DESCRIPTION: o.Report.description,
//                            MODULE_ABBR: o.Report.modelABBR
//                        }
//                        obj.Items.unshift(ob);
//                    });
//                }
//            });
//        }
//        $scope.colors = [
//            '#C66', '#C93', '#F66', '#36C', '#C96', '#633', '#069', '#F90', '#6C3', '#666', '#a136c7', '#2583ce', '#da2c2c',
//            '#00b4ac', '#009dd8', '#25b846', '#da2c2c', '#f27022', '#6438c8', '#fbbc11'
//        ];

//        document.distParentData = $scope.dynamicDashboardMenus;

//        $scope.randomColor = function (parentIndex, index, parentLast, elementLast) {
//            //$($($(".erp-iconlist")[parentIndex]).find("ul").first().find("li")[index]).find(".circle").css('background-color', _.shuffle($scope.colors)[0]);           
//            if (parentLast && elementLast) {

//                $('.ajax-loading').hide();
//                $('[data-toggle="tooltip"]').tooltip();
//                $(".icondescription").on("click", "span", function (e) {
//                    e.preventDefault();
//                    $(".icondescription span").hide();
//                    bootbox.confirm({
//                        message: "Do you want to delete this menu? This cannot be undone.",
//                        buttons: {
//                            cancel: {
//                                label: 'Cancel'
//                            },
//                            confirm: {
//                                label: 'Confirm'
//                            }
//                        },
//                        callback: function (result) {
//                            if (result) {
//                                var value = $(e.currentTarget).attr("data");
//                                $.ajax({
//                                    type: 'POST',
//                                    dataType: "json",
//                                    url: window.location.protocol + "//" + window.location.host + "/Home/DeleteFavouriteMenu?menuName=" + value,
//                                    success: function (data) {
//                                        if (data == 200) {
//                                            $("a[data-name='" + value + "']").parent().remove();
//                                            displayPopupNotification("Menu Deleted Successfully", "success");
//                                        }
//                                        else {
//                                            displayPopupNotification("Error", "error");
//                                        }
//                                    }
//                                });
//                            }

//                        }
//                    });

//                });
//            }
//            if (parentLast && _.isEmpty(elementLast)) {
//                $('.ajax-loading').hide();
//            }
//        }


//        $scope.getMenuLink = function (menuName, virtualPath, fav) {
//            //if (virtualPath.indexOf("_chart") >=0) {
//            //    return  "/Sales/Dashboard/index/"+ virtualPath;
//            //}
//            //else {
//            //    if (fav != undefined)
//            //        return '/' + virtualPath + "?fav=" + fav;
//            //    else
//            //        return '/' + virtualPath;
//            //}
//            if (fav != undefined)
//                return '/' + virtualPath + "?fav=" + fav;
//            else
//                return '/' + virtualPath;


//        }


//        $scope.generateFormat = function (str) {
//            return str.match(/\b(\w)/g).join('');
//        }


//    });


//    var dynamicFavroiteMenu = distributorService.getFavroiteMenu();
//    dynamicFavroiteMenu.then(function (response) {
//        $('.ajax-loading').show();
//        response.data = response.data == "" ? null : response.data;
//        $scope.allFavroiteMenuList = JSON.parse(response.data);

//        $scope.favroiteMenuList = _.filter($scope.allFavroiteMenuList, function (item) {
//            if (item.Report.parentMenu == "Favourite") {
//                return item;
//            }
//        });
//        //if ($scope.favroiteMenuList.length == 0)
//        //    $(".favourite-portlet").hide();

//        $scope.colors = [
//            '#C66', '#C93', '#F66', '#36C', '#C96', '#633', '#069', '#F90', '#6C3', '#666', '#a136c7', '#2583ce', '#da2c2c',
//            '#00b4ac', '#009dd8', '#25b846', '#da2c2c', '#f27022', '#6438c8', '#fbbc11'
//        ];
//        document.distGlobalFavourite = $scope.favroiteMenuList;
//        $scope.randomColor = function (parentIndex, index) {
//            var element = $($($(".erp-iconlist")[parentIndex]).find("ul").first().find("li")[index]).find(".circle");
//            //element.css('background-color', _.shuffle($scope.colors)[0]);  
//            var span = "";
//            element.bind("contextmenu", function (event) {
//                event.preventDefault();
//                $(".icondescription span").hide();
//                span = event.target.nextElementSibling.children;
//                $(span).show();
//                $(span).attr("data", event.target.parentElement.nextElementSibling.textContent);
//            });
//        }

//    });


//    $scope.modelABBRColor = function (modelCode) {
//        if (modelCode == 'SA')
//            return "#43a12e";
//        else if (modelCode == 'AC')
//            return "#3c763d";
//        else if (modelCode == 'AR')
//            return "#4480a4";
//        else if (modelCode == 'PR')
//            return "#666";
//        else if (modelCode == 'ST')
//            return "#31708f";
//        else if (modelCode == 'FA')
//            return "#5aa9d7";
//        else if (modelCode == 'NA')
//            return "#45b6b6";
//        else
//            return "#b6a845";
//    }


//}]);
//distributionModule.service('distributorService', ['$http', '$q', '$timeout', function ($http, $q, $timeout) {
//    return {
//        GetDynamicMenu: function () {
//            //// This could be $http or any other promise returning service.
//            //// Use a deferred and $timeout to simulate a network request.
//            var deferred = $q.defer()
//            $timeout(function () {
//                $http.get('/api/SalesHome/GetDynamicMenu?ModuleCode=' + moduleCode)
//                    .success(function (result) {
//                        deferred.resolve(result);
//                    });
//            }, 2000);
//            return deferred.promise;
//        },
//        getData: function () {
//            var deferred = $q.defer();
//            var results = $http.get('/api/SalesHome/GetDynamicMenu?ModuleCode=' + moduleCode)
//                .success(function (data) {
//                    return data.data;
//                });
//            deferred.resolve(results);
//            return deferred.promise;
//        },

//        getFavroiteMenu: function () {

//            var deferred = $q.defer()
//            var results = $http.get(window.location.protocol + "//" + window.location.host + "/Home/GetFavroiteMenus?moduleCode=10")
//                .success(function (data) {
//                    return data.data;
//                });
//            deferred.resolve(results);
//            return deferred.promise;
//        }
//    };
//}]);