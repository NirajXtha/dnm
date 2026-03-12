planningModule.controller('masterPlanCtrl', function ($scope, $routeParams, masterplanfactory, DTOptionsBuilder) {

    $scope.pageName = "Master Plan";
    $scope.dateFormat = "BS";
    $scope.dtOptions = {
        paging: true,
        ordering: true,
        searching: true,
    };
    $scope.SalesPlan_CustomerEmployees = [];
    if ($routeParams.masterPlanCode == undefined || $routeParams.masterPlanCode == '')
        $scope.masterPlanListShow = true;
    else
        $scope.masterPlanListShow = false;
    $scope.IsCustomerProduct = true;
    $scope.planList = [];
    $scope.customer_product_option = "customer_product";
    $scope.CustomerProductOption = function () {
        if ($scope.customer_product_option == 'customer_product') {
            $scope.IsCustomerProduct = true;
            return;
        }
        $scope.IsCustomerProduct = false;
    }
    
    $scope.branchDataSource = {
        type: "json",
        serverFiltering: false,
        transport: {
            read: {
                dataType: "json",
                url: "/api/SubPlanApi/GetChiledLevelBranch",                
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
    $scope.branchOptions = {
        //dataSource: $scope.branchDataSource,
       // dataSource:{
       //     data:$scope.SalesPlan_CustomerEmployees
       // },
       // dataTextField: "BRANCH_EDESC",
       // dataValueField: "BRANCH_CODE",
       // placeholder: "Select Branch...",
       //// optionLabel: "--Select Branch--",
        // filter: "contains",
        placeholder: "Select branch...",
        dataTextField: "BRANCH_EDESC",
        dataValueField: "BRANCH_CODE",
        valuePrimitive: true,
        autoBind: false,
        dataSource: $scope.SalesPlan_CustomerEmployees,
    };
    //$scope.branchvalue = null;
    $scope.branchvalue = [];

    $scope.employeeDataSource = {
        type: "json",
        serverFiltering: false,
        transport: {
            read: {
                dataType: "json",
                url: "/api/SubPlanApi/GetChiledLevelEmployee",
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
        //dataSource: $scope.employeeDataSource,
        //dataTextField: "EMPLOYEE_EDESC",
        //dataValueField: "EMPLOYEE_CODE",
        //placeholder: "Select Employee...",
        ////optionLabel: "--Select Employee--",
        //filter: "contains",
        placeholder: "Select employee...",
        dataTextField: "EMPLOYEE_EDESC",
        dataValueField: "EMPLOYEE_CODE",
        valuePrimitive: true,
        autoBind: false,
        filter: "contains",
        dataSource: $scope.employeeDataSource,
    };
    //$scope.employeevalue = null;
    $scope.employeevalue = [];

    $scope.divisionDataSource = {
        type: "json",
        serverFiltering: false,
        transport: {
            read: {
                dataType: "json",
                url: "/api/SubPlanApi/GetChiledLevelDivision",
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

    $scope.divisionOptions = {
        //dataSource: $scope.divisionDataSource,
        //dataTextField: "DIVISION_EDESC",
        //dataValueField: "DIVISION_CODE",
        //optionLabel: "--Select Division--",
        //filter: "contains",
        placeholder: "Select divisions...",
        dataTextField: "DIVISION_EDESC",
        dataValueField: "DIVISION_CODE",
        valuePrimitive: true,
        autoBind: false,
        dataSource: $scope.divisionDataSource,
    };
    //$scope.divisionvalue = null;
    $scope.divisionvalue = [];

    $scope.customersDataSource = {
        type: "json",
        serverFiltering: false,
        transport: {
            read: {
                dataType: "json",
                url: "/api/SubPlanApi/GetChiledLevelCustomers",
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

    $scope.customerOptions = {
        //dataSource: $scope.customersDataSource,
        //dataTextField: "CUSTOMER_EDESC",
        //dataValueField: "CUSTOMER_CODE",
        //optionLabel: "--Select Customer--",
        ////placeholder: "Select Customer...",
        //filter: "contains",
        placeholder: "Select customer...",
        dataTextField: "CUSTOMER_EDESC",
        dataValueField: "CUSTOMER_CODE",
        valuePrimitive: true,
        autoBind: false,
        dataSource: $scope.customersDataSource,
    };
    //$scope.customervalue = null;
    $scope.customervalue = [];

    $scope.masterPlanList = [];

    $scope.retrieveMasterPlanList = function () {
        masterplanfactory.getMasterPlanList().then(function (result) {
            //showloader();
            $scope.masterPlanList = result.DATA;
        }, function (err) {

        });
    }

    $scope.GetMasterPlanName = function (code, startDate, endDate) {
        var link = '<a href="javascript:;" ng-click="updateMasterPlan('+code+','+startDate+','+endDate+')">{{dataItem.MASTER_PLAN_EDESC}}</a>';
         //link = link + '<small>  <a href="javascript:;" ng-click="goToMasterPlanView(' + code + ')" title="view items detail of master plan {{dataItem.MASTER_PLAN_EDESC}}"><i class="text-success">(detail)</i></a></small>';
         return link;
    }
    $scope.goToMasterPlanView = function (code) {
        window.location = "/Planning/Home/Setup#!Planning/MasterPlanView/" + code;
    }
    $scope.retrieveMasterPlanList();

    $scope.detailGridOptions = function (dataItem) {
        return {
            dataSource: {
                type: "json",
                transport: {
                    read: window.location.protocol + "//" + window.location.host + "/api/PlanApi/GetMasterSalesPlanDetailList?masterplancode=" + dataItem.MASTER_PLAN_CODE
                },
                requestStart: function () {
                    //showloader();
                },
                pageSize: 5,
            },
            scrollable: false,
            pageable:true,
            columns: [
                { field: "PLAN_CODE", title: "Plan Code", width: "100px" },
                { field: "PLAN_EDESC", title: "Plan Name" }
            ]
        };
    };

    $scope.mainGridOptions = {
        dataSource: {
            data: $scope.masterPlanList,
            pageSize: 30,
        },
        sortable: true,
        pageable: true,
        height: window.innerHeight - 200,
        groupable: true,
        toolbar: kendo.template($("#toolbar-template").html()),
        dataBound: function () {
            $('div').removeClass('.k-header k-grid-toolbar');
        },
        dataBinding: function () {
            record = (this.dataSource.page() - 1) * this.dataSource.pageSize();
        },

        columns: [{ title: "SN", template: "#= ++record #", width: "35px" },
            {
                field: "MASTER_PLAN_EDESC",
                title: "Master Plan Name",
                //template: $scope.GetMasterPlanName("#=MASTER_PLAN_CODE#")
                template: "<a href='javascript:;' ng-click='updateMasterPlan(dataItem.MASTER_PLAN_CODE,dataItem.START_DATE, dataItem.END_DATE) '>{{dataItem.MASTER_PLAN_EDESC}}</a>"
            },
            {
                field: "START_DATE",
                title: "Start Date",
            }, {
                field: "END_DATE",
                title: "End Date",
            },
            {
                title: "Action",
                width: "80px",
                //template: "<a href='javascript:;' ng-click='viewMasterPlan(dataItem.MASTER_PLAN_CODE)'><i class='fa fa-edit'></i></a>&nbsp;&nbsp; <a href='javascript:;' ng-click='updateMasterPlan(dataItem.MASTER_PLAN_CODE)'><i class='fa fa-edit'></i></a>&nbsp;&nbsp; <a href='javascript:;' ng-click='deleteMasterPlan(dataItem.MASTER_PLAN_CODE)'><i class='fa fa-trash text-danger'></i></a>",
                template: "<a href='javascript:;' ng-click='updateMasterPlan(dataItem.MASTER_PLAN_CODE,dataItem.START_DATE,dataItem.END_DATE)'><i class='fa fa-edit'></i></a>&nbsp;&nbsp; <a href='javascript:;' ng-click='deleteMasterPlan(dataItem.MASTER_PLAN_CODE)'><i class='fa fa-trash text-danger'></i></a>",
            }]
    };
    function getIndexOf(arr, val, prop) {
        var l = arr.length,
          k = 0;
        for (k = 0; k < l; k = k + 1) {
            if (arr[k][prop] === val) {
                return k;
            }
        }
        return false;
    }
    $scope.viewMasterPlan = function (code) {
        $scope.masterPlanListShow = false;
        window.location = "/Planning/Home/Setup#!Planning/MasterPlanSetup/" + code;
    }
    $scope.deleteMasterPlan = function (code) {
        var sureDelete = confirm("Are you sure to delete this master plan.");
        if (sureDelete) {
            masterplanfactory.DeleteMasterPlan(code).then(function () {

                displayPopupNotification("Successfully deleted the plan", "success");
                var index = getIndexOf($scope.masterPlanList, code, 'MASTER_PLAN_CODE');
                $scope.masterPlanList.splice(index, 1);
            }, function (err) {
                displayPopupNotification("Error happened while delete, please try later.", "error");
            });
        }
        else {
            return false;
        }
    }
    $scope.updateMasterPlan = function (code,startDate,endDate) {
        $scope.masterPlanListShow = false;
        //showloader();
        window.location = "/Planning/Home/Setup#!Planning/MasterPlan/" + code + "?startDate=" + startDate + "&endDate=" + endDate;
    }
    $scope.cancelMasterPlan = function () {
        $scope.masterPlanListShow = true;
        window.location = "/Planning/Home/Setup#!Planning/MasterPlan";
    }
    function getCustomers(customerUrl, dateObj) {
        $.ajax({
            url: customerUrl,
            data: { startdate: dateObj.startDate, enddate: dateObj.endDate },
            async: false
        }).done(function (result) {
            var branchDDL = $('#customers').data('kendoMultiSelect');
            if (branchDDL != undefined) {
                var dataSource_branchDDL = new kendo.data.DataSource({
                    data: result
                });
                branchDDL.setDataSource(dataSource_branchDDL);
            }
            $scope.$apply();
        });
    }
    function getEmployees(employeeUrl, dateObj) {
        $.ajax({
            url: employeeUrl,
            data: { startdate: dateObj.startDate, enddate: dateObj.endDate },
            async: false
        }).done(function (result) {
            var branchDDL = $('#employees').data('kendoMultiSelect');
            if (branchDDL != undefined) {
                var dataSource_branchDDL = new kendo.data.DataSource({
                    data: result
                });
                branchDDL.setDataSource(dataSource_branchDDL);
            }
            $scope.$apply();
        });
    }
    function getDivisions(divisionUrl, dateObj) {
        $.ajax({
            url: divisionUrl,
            data: { startdate: dateObj.startDate, enddate: dateObj.endDate },
            async: false
        }).done(function (result) {
            var branchDDL = $('#divisions').data('kendoMultiSelect');
            if (branchDDL != undefined) {
                var dataSource_branchDDL = new kendo.data.DataSource({
                    data: result
                });
                branchDDL.setDataSource(dataSource_branchDDL);
            }
            $scope.$apply();
        });
    }
    function getBranches(branchUrl, dateObj) {
        $.ajax({
            url: branchUrl,
            data: { startdate: dateObj.startDate, enddate: dateObj.endDate },
            async: false
        }).done(function (result) {
            var branchDDL = $('#branchs').data('kendoMultiSelect');
            if (branchDDL != undefined) {
                var dataSource_branchDDL = new kendo.data.DataSource({
                    data: result
                });
                branchDDL.setDataSource(dataSource_branchDDL);
            }
            $scope.$apply();
        });
    }
    $('#ddlDateFilterVoucher').on('change', function () {
        
        var dateObj = $('select#ddlDateFilterVoucher option:selected').data();
        var branchUrl = window.location.protocol + "//" + window.location.host + "/api/PlanApi/GetDateWise_SalesPlan_Branch";
        var divisionUrl = window.location.protocol + "//" + window.location.host + "/api/PlanApi/GetDateWise_SalesPlan_Division";
        var employeeUrl = window.location.protocol + "//" + window.location.host + "/api/PlanApi/GetDateWise_SalesPlan_Employee";
        var customerUrl = window.location.protocol + "//" + window.location.host + "/api/PlanApi/GetDateWise_SalesPlan_Customer";
        getBranches(branchUrl,dateObj);
        getDivisions(divisionUrl,dateObj);
        getEmployees(employeeUrl,dateObj);
        getCustomers(customerUrl,dateObj);
    });

})
    

    
planningModule.factory('masterplanfactory', function ($http) {
    var serv = {};
    serv.getPlanList = function () {
        var planUrl = window.location.protocol + "//" + window.location.host + "/api/PlanApi/GetSalesPlanList";
        var data = { customercode: '', employeecode: '', divisioncode: '', branchcode: '', startdate: '', enddate: '' };
        return $.ajax({
            url: planUrl,
            type: 'GET',
            data: data,
            dataType: "json",
            async: false
        });
    }
    serv.getMasterPlanList = function () {
        var planUrl = window.location.protocol + "//" + window.location.host + "/api/PlanApi/GetMasterSalesPlanList";
        var data = { customercode: '', employeecode: '', divisioncode: '', branchcode: '', startdate: '', enddate: '' };
        return $.ajax({
            url: planUrl,
            type: 'GET',
            //data: data,
            dataType: "json",
            async: false
        });
    }
    serv.DeleteMasterPlan = function (master_plan_code) {
        var planUrl = window.location.protocol + "//" + window.location.host + "/api/PlanApi/DeleteMasterSalesPlan";
        var data = { code: master_plan_code };
        return $.ajax({
            url: planUrl,
            type: 'GET',
            data: data,
            dataType: "json",
            async: false
        });
    }
    return serv;
});