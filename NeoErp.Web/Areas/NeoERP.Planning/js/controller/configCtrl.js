planningModule.controller("configCtrl", ['$scope', 'menuService', function ($scope, menuService) {
    $scope.tn = "menu test";
    $scope.menuList = null;
    $scope.moduleCode = '30';
    $("#nepaliDate5").val(AD2BS(moment().format('YYYY-MM-DD')));
    menuService.getMenus($scope.moduleCode).then(function (res) {
        $scope.menuList = res.data;
    });

    $scope.userDataSource = {
        type: "json",
        serverFiltering: false,
        transport: {
            read: {
                dataType: "json",
                url: window.location.protocol + "//" + window.location.host + '/api/PlanApi/GetAllUsers',
            },
            parameterMap: function (data, action) {
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        var newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        var newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    var newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        }
    };

    $scope.userOptions = {
        //optionLabel: "-- Select User --",
        placeholder: "-- Select User --",
        dataTextField: "LOGIN_EDESC",
        dataValueField: "USER_NO",
        valuePrimitive: true,
        autoBind: false,
        filter: "contains",
        autoClose: false,
        dataSource: $scope.userDataSource,
    };

    $scope.planTypeDataSource = {
        type: "json",
        serverFiltering: false,
        transport: {
            read: {
                dataType: "json",
                url: window.location.protocol + "//" + window.location.host + '/api/PlanApi/GetAggregatePlanType',
            },
            parameterMap: function (data, action) {
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        var newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        var newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    var newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        }
    };

    $scope.planTypeOptions = {
        //optionLabel: "-- Select Type --",
        placeholder: "Select Type...",
        dataTextField: "PLAN_TYPE",
        dataValueField: "INITIALS",
        valuePrimitive: true,
        autoBind: false,
        filter: "contains",
        autoClose: false,
        dataSource: $scope.planTypeDataSource,
    };

    $scope.categoryDataSource = {
        type: "json",
        serverFiltering: false,
        transport: {
            read: {
                dataType: "json",
                url: window.location.protocol + "//" + window.location.host + '/api/PlanApi/GetAllCategory',
            },
            parameterMap: function (data, action) {
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        var newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        var newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    var newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        }
    };

    $scope.categoryOptions = {
        //optionLabel: "-- Select Category --",
        placeholder: "-- Select Category --",
        dataTextField: "CATEGORY_EDESC",
        dataValueField: "CATEGORY_CODE",
        valuePrimitive: true,
        autoBind: false,
        filter: "contains",
        autoClose:false,
        dataSource: $scope.categoryDataSource,
    };

    var getCodeByName = function (pName) {
        var planCode = "";
        var planName = "";
        switch (pName) {
            case "SALES":
                planName = "Sales";
                planCode = "01"
                break;
            case "PRODUCTION":
                planName = "Production";
                planCode = "02"
                break;
            case "PROCUREMENT":
                planName = "Procurement";
                planCode = "03"
                break;
            case "MATERIAL":
                planName = "Material";
                planCode = "04"
                break;
            case "LEDGER":
                planName = "Ledger";
                planCode = "05"
                break;
            case "BUDGET":
                planName = "Budget";
                planCode = "06"
                break;
            default:
        }
        return planCode + "," + planName;
    }
    var saveConfigSetup = function () {
        showloader();
        var categoryList = $("#category").data("kendoMultiSelect").value().join("','");
        var planNameList = $("#planType").data("kendoMultiSelect").value().join(",");
        var userList = "";//$("#users").data("kendoMultiSelect").value().join(",");
        var param = {
            USER_NO:userList,
            PLAN_NAME: planNameList,
            //PLAN_CODE: planCode,
            CATEGORY_CODE:categoryList,
        }
        var data = { model:param};
        var url = window.location.protocol + "//" + window.location.host + "/api/PlanApi/SaveConfigSetup";
        $.ajax({
            url: url,
            type: "POST",
            data: param,
            dataType: 'JSON',
            //async: false,
            //contentType: "application/json; charset=utf-8",
            success: function (data) {
                displayPopupNotification("Successfully Saved.", "success");
                hideloader();
            },
            error: function (jqxhr) {
                displayPopupNotification("There is an Server Error", "error");
                hideloader();
            }
        });
    }

    $scope.saveCategoryInXml = function () {
        saveConfigSetup();
    }

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
    var grid,
        createGrid = function () {
            grid = $("#grid").kendoGrid({
                dataSource: {
                    type: "json",
                    transport: {
                        read: window.location.protocol + "//" + window.location.host + '/api/PlanApi/GetCategoryPlanFromXML',
                    },
                    pageSize: 20,
                    requestEnd: function () {
                        hideloader();
                    },
                },
                //toolbar: ["excel"],
                //excel: {
                //    fileName: "Referential.xlsx"
                //},
                height: window.innerHeight - 330,
                groupable: false,
                resizable: true,
                filterable: true,
                sortable: true,
                pageable: true,
                columnMenu: false,
                dataBinding: function () {
                    record = (this.dataSource.page() - 1) * this.dataSource.pageSize();
                },
                dataBound: function (o) {
                    var grid = o.sender;
                    if (grid.dataSource.total() == 0) {
                        var colCount = grid.columns.length;
                        $(o.sender.wrapper)
                            .find('tbody')
                            .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                        displayPopupNotification("No Data Found.", "info");
                        $scope.visibleSavebtn = false;
                        $scope.visibleGenerateBtn = true;
                    }
                },
                columns: [
                    { title: "SN", template: "#= ++record #", width: "15px" },
                    {
                        field: "PLAN_CODE",
                        title: "Plan Code",
                        width: "30px",
                    },
                    {
                        field: "PLAN_EDESC",
                        title: "Plan Name",
                        width: "120px",
                    },
                    {
                        field: "PLAN_DATE",
                        title: "Start Date",
                        //format: "{0:yyyy-MMM-dd}",
                        width: "35px",
                    },
                    //{
                    //    field: "END_DATE",
                    //    title: "End Date",
                    //    format: "{0:yyyy-MMM-dd}",
                    //    width: "35px",
                    //},
                    {
                        field: "INITIALS",
                        title: "Type",
                        width: "100px",
                    },
                    {
                        field: "PLAN_TYPE",
                        title: "Plan Type",
                        width: "40px",
                    },
                    {
                        field: "REF_FLAG",
                        title: "Option Type",
                        width: "40px",
                    },
                    {
                        field: "PLAN_CODE", title: "Action", sortable: false, filterable: false, width: "90px",
                        template: "#= setAction(PLAN_CODE,INITIALS,PLAN_EDESC,REF_FLAG) #",
                        //template: "{{ setAction(dataItem.PLAN_CODE,dataItem.INITIALS) }}",
                        groupable: false,
                        width: "40px",
                    }
                    //{
                    //    field: "ID", title: "Action", sortable: false, filterable: false, width: "60px",
                    //    template: '<a class="edit glyphicon glyphicon-edit" style="color:orange;"><span class="sr-only"></span> </a> <a style="color:red;" class="delete glyphicon glyphicon-trash "><span class="sr-only"></span> </a> <a style="color:green;" title="go to Plan setup" class="goto_planSetup icon icon-share-alt "><span class="sr-only"></span> </a>'
                    //},
                ],
            }).data("kendoGrid");

        };

    createGrid();
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