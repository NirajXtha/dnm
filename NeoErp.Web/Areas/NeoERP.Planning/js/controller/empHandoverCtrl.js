planningModule.controller("empHandoverCtrl", ['$scope', 'menuService', function ($scope, menuService) {
    $scope.tn = "menu test";
    $scope.menuList = null;
    $scope.moduleCode = '30';
    $("#nepaliDate5").val(AD2BS(moment().format('YYYY-MM-DD')));
    menuService.getMenus($scope.moduleCode).then(function (res) {
        $scope.menuList = res.data;
    });

    $scope.employeeDataSource = {
        type: "json",
        serverFiltering: false,
        transport: {
            read: {
                dataType: "json",
                url: window.location.protocol + "//" + window.location.host + '/api/SubPlanApi/GetChiledLevelEmployee',
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

    $scope.employeeOptions = {
        optionLabel: "-- Select Employee --",
        //placeholder: "Select Type...",
        dataTextField: "EMPLOYEE_EDESC",
        dataValueField: "EMPLOYEE_CODE",
        valuePrimitive: true,
        autoBind: false,
        filter: "contains",
        dataSource: $scope.employeeDataSource,
    };
    $scope.employeeOptions1 = {
        optionLabel: "-- Select Employee --",
        //placeholder: "Select Type...",
        dataTextField: "EMPLOYEE_EDESC",
        dataValueField: "EMPLOYEE_CODE",
        valuePrimitive: true,
        autoBind: false,
        filter: "contains",
        dataSource: $scope.employeeDataSource,
    };
    $scope.validation = function (date, fromEmp, ToEmp)
    {
        var check = true
        if (!(fromEmp && ToEmp)) {
            displayPopupNotification("Choose Employee.", "error");
            return check = false;
        }
        if (fromEmp == "" && ToEmp == "") {
            displayPopupNotification("Choose Employee.", "error");
            return check = false;
        }
        if (fromEmp == ToEmp) {
            displayPopupNotification("You can not migrate between same employee.", "error");
            return check = false;
        }
        return check;
    }
    $scope.saveHandoverEmployee = function () {
        var url = window.location.protocol + "//" + window.location.host + "/api/PlanSetupApi/SaveEmployeeHandover";
        var fDate = $("#nepaliDate5").val();
        var splitedData = fDate.split('-');
        var from_date = splitedData.splice(0, 2).join('-')
        var fromDateInEnglish = moment(BS2AD(from_date + "-01")).format("YYYY-MM-DD")
        var from_employee = $("#fromEmployee").val();
        var to_employee = $("#toEmployee").val()
        var valid = $scope.validation(from_date, from_employee, to_employee);
        if (!valid)
            return;
        showloader();
        var param = {
            FROM_DATE: fromDateInEnglish,
            FROM_EMPLOYEE_CODE: from_employee,
            TO_EMPLOYEE_CODE: to_employee
        };
        menuService.saveHandoverEmployee(url, param).then(function (response, status, headers, config) {
            var message = response.data.MESSAGE;
            if (message == "Success") {
                displayPopupNotification("Save Successfully.", "success");
                createGrid();
                hideloader();
            }
            else {
                displayPopupNotification(message, "warning");
                hideloader();
            }
        }, function (error, status, headers, config) {
            displayPopupNotification(error.data.ExceptionMessage, "error");
            hideloader();
        });
    }

    var grid,
        createGrid = function () {
            grid = $("#grid").kendoGrid({
                dataSource: {
                    type: "json",
                    transport: {
                        read: window.location.protocol + "//" + window.location.host + '/api/PlanApi/getEmployeeHandoverList',
                    },
                    pageSize: 20,
                    requestEnd: function () {
                        hideloader();
                    },
                },
                height: window.innerHeight - 330,
                groupable: false,
                resizable: true,
                filterable: true,
                sortable: true,
                pageable: true,
                columnMenu: false,
                detailInit: detailInit,
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
                    { title: "SN", template: "#= ++record #", width: "50px" },
                    {
                        field: "FROM_DATE",
                        title: "From Date",
                        width: "80",
                    },
                    {
                        field: "FROM_EMPLOYEE_EDESC",
                        title: "(From) Employee Name",
                        width: "110",
                    },
                    {
                        field: "TO_EMPLOYEE_EDESC",
                        title: "(To) Employee Name",
                        //format: "{0:yyyy-MMM-dd}",
                        //width: "350px",
                    },
                    //{
                    //    field: "INITIALS",
                    //    title: "Type",
                    //    width: "100px",
                    //},
                    //{
                    //    field: "PLAN_TYPE",
                    //    title: "Plan Type",
                    //    width: "40px",
                    //},
                    //{
                    //    field: "REF_FLAG",
                    //    title: "Option Type",
                    //    width: "40px",
                    //},
                    //{
                    //    field: "PLAN_CODE", title: "Action", sortable: false, filterable: false, width: "90px",
                    //    template: "#= setAction(PLAN_CODE,INITIALS,PLAN_EDESC,REF_FLAG) #",
                    //    //template: "{{ setAction(dataItem.PLAN_CODE,dataItem.INITIALS) }}",
                    //    groupable: false,
                    //    width: "40px",
                    //}
                    //{
                    //    field: "ID", title: "Action", sortable: false, filterable: false, width: "60px",
                    //    template: '<a class="edit glyphicon glyphicon-edit" style="color:orange;"><span class="sr-only"></span> </a> <a style="color:red;" class="delete glyphicon glyphicon-trash "><span class="sr-only"></span> </a> <a style="color:green;" title="go to Plan setup" class="goto_planSetup icon icon-share-alt "><span class="sr-only"></span> </a>'
                    //},
                ],
            }).data("kendoGrid");

        };
    function detailInit(e) {
        $("<div/>").appendTo(e.detailCell).kendoGrid({
            dataSource: {
                type: "json",
                transport: {
                    read: window.location.protocol + "//" + window.location.host + '/api/PlanApi/getEmployeeHandoverListWithPlan?fromEmpCode=' + e.data.FROM_EMPLOYEE_CODE + "&toEmpCode=" + e.data.TO_EMPLOYEE_CODE,
                },
                serverPaging: true,
                serverSorting: true,
                serverFiltering: true,
                pageSize: 10,
                filter: { field: "PLAN_EDESC", operator: "eq", value: e.data.PLAN_CODE }
            },
            scrollable: false,
            sortable: true,
            pageable: true,
            columns: [
                { field: "PLAN_CODE", width: "80" },
                { field: "PLAN_EDESC", title: "Plan Name", width: "110" },
                { field: "START_DATE", title: "Start Date", width: "100", template: '#= kendo.toString(kendo.parseDate(START_DATE), "MM/dd/yyyy")#' },
                { field: "END_DATE", title: "End Date", width: "100", template: '#= kendo.toString(kendo.parseDate(END_DATE), "MM/dd/yyyy")#' }
            ]
        });
    }

     createGrid();

}]);

planningModule.factory('menuService', ['$http', function ($http) {
    var fac = {};
    fac.getMenus = function (moduleCode) {
        return $http.get('/api/SalesHome/GetDynamicMenu?ModuleCode=' + moduleCode)
    }
    fac.getFavroiteMenu = function (moduleCode) {
        return $http.get(window.location.protocol + "//" + window.location.host + "/Home/GetFavroiteMenus?moduleCode=" + moduleCode)
    }
    fac.saveHandoverEmployee = function(url, param) {
        return $http.post(url, param);
    }
    return fac;
}])