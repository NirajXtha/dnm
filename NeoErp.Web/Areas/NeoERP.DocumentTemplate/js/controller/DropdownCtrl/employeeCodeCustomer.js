
DTModule.controller('employeeCtrlCustomer', function ($scope, $http, $routeParams, $window, $filter) {

    $scope.employeesDataSourceCustomer = {
       
        serverFiltering: true,
        suggest: true,
        highlightFirst: true,
        transport: {
            read: {
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                type: "GET",
                url: "/api/TemplateApi/GetAllEmployeeCodeByFilters",
            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        //newParams = {
                        //    filter: data.filter.filters[0].value
                        //};
                        //return newParams;
                        if (data.filter.filters[0].value != "") {
                            newParams = {
                                filter: data.filter.filters[0].value
                            };
                            return newParams;
                        }
                        else {
                            newParams = {
                                filter: "!@$"
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
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        },
    };
    //$scope.EmployeeSetupDataSource = {
    //    type: "json",
    //    serverFiltering: true,
    //    transport: {
    //        read: {
    //            url: "/api/TemplateApi/GetAllEmployeeCodeByFilters",
    //        },
    //        parameterMap: function (data, action) {
    //            
    //            var newParams;
    //            if (data.filter != undefined) {
    //                if (data.filter.filters[0] != undefined) {
    //                    newParams = {
    //                        filter: data.filter.filters[0].value
    //                    };
    //                    return newParams;
    //                }
    //                else {
    //                    newParams = {
    //                        filter: ""
    //                    };
    //                    return newParams;
    //                }
    //            }
    //            else {
    //                newParams = {
    //                    filter: ""
    //                };
    //                return newParams;
    //            }
    //        }
    //    },

    //};




    $scope.employeeCodeOptionCustomer = {

        dataSource: $scope.employeesDataSourceCustomer,
        template: '<span>{{dataItem.EMPLOYEE_EDESC}}</span>  ' +
            '<span>{{dataItem.Type}}</span>',
        dataBound: function (e) {
            debugger;
            //var employee = $("#employeeCustomer").data("kendoComboBox");
            //if (employee != undefined) {
            //    employee.setOptions({
            //        template: $.proxy(kendo.template("#= formatValue(EMPLOYEE_EDESC,Type, this.text()) #"), employee)
            //    });
            //}
            if (this.element[0].attributes['emp-index'] == undefined) {
                var empcode = $("#employeeCustomer").data("kendoComboBox");
            }
            else {
                var index = this.element[0].attributes['emp-index'].value;
                var empcodeLength = ((parseInt(index) + 1) * 3) - 1;
                var empcode = $($(".employeemaster")[empcodeLength]).data("kendoComboBox");

            }
            if (empcode != undefined) {
                empcode.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(EMPLOYEE_EDESC,Type, this.text()) #"), empcode)
                });
            }
        }
    }


    $scope.employeeCodeOnChangeCustomer = function (kendoEvent) {
        debugger;
        if (kendoEvent.sender.dataItem() == undefined) {

            $scope.employee_error = "Please Enter Valid Code."
            $('#employeeCustomer').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {

            $scope.employee_error = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }


    //employee popup advanced search// --start
    var getemployeesByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getEmployeeCode";
    $scope.employeetreeDataCustomer = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                type: "GET",
                url: getemployeesByUrl,
                data: function (data, evt) {
                }
            },

        },
        schema: {
            parse: function (data) {

                return data;
            },
            model: {
                id: "MASTER_EMPLOYEE_CODE",
                parentId: "PRE_EMPLOYEE_CODE",
                children: "Items",
                fields: {
                    CUSTOMER_CODE: { field: "EMPLOYEE_CODE", type: "string" },
                    CUSTOMER_EDESC: { field: "EMPLOYEE_EDESC", type: "string" },
                    parentId: { field: "PRE_EMPLOYEE_CODE", type: "string", defaultValue: "00" },
                }
            }
        }
    });

    //treeview expand on startup
    $scope.onDataBound = function () {
        $('#employeetreeCustomer').data("kendoTreeView").expand('.k-item');
    }

    //treeview on select
    $scope.employeeoptionsCustomer = {
        loadOnDemand: false,
        select: function (e) {

            var currentItem = e.sender.dataItem(e.node);
            $('#employeeGridCustomer').removeClass("show-displaygrid");
            $("#employeeGridCustomer").html("");
            BindemployeeGridCustomer(currentItem.employeeId, currentItem.masterEmployeeCode, "");
            $scope.$apply();
        },
    };
    //search whole data on search button click
    $scope.BindSearchGrid = function () {

        $scope.searchText = $scope.txtSearchString;
        BindemployeeGridCustomer("", "", $scope.searchText);
    }


    //Grid Binding main Part
    function BindemployeeGridCustomer(employeeId, employeeMasterCode, searchText) {
        $scope.employeeCodeGridOptionsCustomer = {
            dataSource: {
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                transport: {
                    read: "/api/TemplateApi/GetemployeeListByemployeeCode?employeeId=" + employeeId + '&employeeMasterCode=' + employeeMasterCode + '&searchText=' + searchText,
                },

                schema: {
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    model: {
                        fields: {
                            EMPLOYEE_CODE: { type: "string" },
                            EMPLOYEE_EDESC: { type: "string" }
                        }
                    }
                },
                pageSize: 30,

            },
            scrollable: true,
            sortable: true,
            resizable: true,
            pageable: true,
            filterable: {
                extra: false,
                operators: {
                    number: {
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        gte: "is greater than or equal to	",
                        gt: "is greater than",
                        lte: "is less than or equal",
                        lt: "is less than",
                    },
                    string: {
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        startswith: "Starts with",
                        contains: "Contains",
                        doesnotcontain: "Does not contain",
                        endswith: "Ends with",
                    },
                    date: {
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        gte: "Is after or equal to",
                        gt: "Is after",
                        lte: "Is before or equal to",
                        lt: "Is before",
                    }

                }
            },
            columnMenu: true,
            columnMenuInit: function (e) {
                wordwrapmenu(e);
            },
            dataBound: function (e) {
                debugger;
                $("#employeeGridCustomer tbody tr").css("cursor", "pointer");
                $("#employeeGridCustomer tbody tr").on('dblclick', function () {
                    debugger;
                    var employeecode = $(this).find('td span').html();
                    $scope.customersMasterChildArr.DEALING_PERSON = employeecode;
                    if ($("#employeeCustomer").hasClass('borderRed')) {
                        $scope.employee_error = "";
                        $("#employeeCustomer").removeClass('borderRed');
                    }
                    $('#employeeModalCustomer').modal('toggle');
                    $scope.$apply();
                })
            },
            columns: [{
                field: "EMPLOYEE_CODE",
                hidden: true,
                title: "Employee Code",

            }, {
                field: "EMPLOYEE_EDESC",
                title: "Employee Name",

            }, {
                field: " EPERMANENT_ADDRESS",
                title: "Address",

            }, {
                field: "MOBILE",
                title: "Phone",

            }]
        };
    }


    $scope.BrowseTreeListForEmployeeCustomerInSIJ = function (kendoEvent) {
        if ($scope.havRefrence == 'Y' && $scope.freeze_master_ref_flag == "Y") {
            var referencenumber = $('#refrencetype').val();
            if ($scope.ModuleCode != '01' && referencenumber !== "") {
                return;
            }
        }
        if ($scope.freeze_master_ref_flag == "N") {
            $('#employeeModalCustomer').modal({
                backdrop: false, // prevents backdrop conflict
                keyboard: false
            }).modal('show');
        }
    }

    //employee popup advanced search// --end
});

