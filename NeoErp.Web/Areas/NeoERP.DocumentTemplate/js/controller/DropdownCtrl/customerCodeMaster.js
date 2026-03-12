
DTModule.controller('customerCtrlMasterCode', function ($scope, $http, $routeParams, $window, $filter, $timeout) {

    $scope.customerDataSource = {
        type: "json",
        serverFiltering: true,
        suggest: true,
        highlightFirst: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllMasterCustomerCodeByFilter",

            },
            parameterMap: function (data, action) {
                debugger;
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




    $scope.customerCodeOption = {
        dataSource: $scope.customerDataSource,
        template: '<span>{{dataItem.CUSTOMER_EDESC}}</span>  ' +
            '<span>{{dataItem.Type}}</span>',
        dataBound: function (e) {
            debugger;
            //var employee = $("#employee").data("kendoComboBox");
            //if (employee != undefined) {
            //    employee.setOptions({
            //        template: $.proxy(kendo.template("#= formatValue(EMPLOYEE_EDESC,Type, this.text()) #"), employee)
            //    });
            //}
            if (this.element[0].attributes['cus-index'] == undefined) {
                var cuscode = $("#customer").data("kendoComboBox");
            }
            else {
                var index = this.element[0].attributes['cus-index'].value;
                var cuscodeLength = ((parseInt(index) + 1) * 3) - 1;
                var cuscode = $($(".customermaster")[cuscodeLength]).data("kendoComboBox");

            }
            if (cuscode != undefined) {
                cuscode.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(CUSTOMER_EDESC,Type, this.text()) #"), cuscode)
                });
            }
        }
    }


    $scope.customerCodeOnChange = function (kendoEvent) {
        debugger;
        if (kendoEvent.sender.dataItem() == undefined) {

            $scope.customer_error = "Please Enter Valid Code."
            $('#customer').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {

            $scope.customer_error = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }


    //employee popup advanced search// --start
    //var getcustomerByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getCustomerCodeMaster";
    //$scope.customertreeData = new kendo.data.HierarchicalDataSource({
    //    transport: {
    //        read: {
    //            url: getcustomerByUrl,
    //            type: 'GET',
    //            data: function (data, evt) {
    //                debugger;
    //            }
    //        },

    //    },
    //    schema: {
    //        parse: function (data) {
    //            debugger;
    //            return data;
    //        },
    //        model: {
    //            id: "MASTER_CUSTOMER_CODE",
    //            parentId: "PRE_CUSTOMER_CODE",
    //            children: "Items",
    //            fields: {
    //                CUSTOMER_CODE: { field: "CUSTOMER_CODE", type: "string" },
    //                CUSTOMER_EDESC: { field: "CUSTOMER_EDESC", type: "string" },
    //                parentId: { field: "PRE_CUSTOMER_CODE", type: "string", defaultValue: "00" },
    //            }
    //        }
    //    }
    //});

    //treeview expand on startup
    $scope.onDataBound = function () {
        debugger;
        $('#customertree').data("kendoTreeView").expand('.k-item');
    }

    //treeview on select
    $scope.customeroptions = {
        loadOnDemand: false,
        select: function (e) {
            debugger;
            var currentItem = e.sender.dataItem(e.node);
            $('#customerGrid').removeClass("show-displaygrid");
            $("#customerGrid").html("");
            BindCustomerGrid(currentItem.masterCustomerCode, "");
            $scope.$apply();
        },
    };

    $timeout(function () {
        var mockCustomerCode = "DEFAULT123"; // Replace with a real one from your dataset
        $('#customerGrid').removeClass("show-displaygrid");
        $("#customerGrid").html("");
        BindCustomerGrid("", "");
    }, 0);

    //search whole data on search button click
    $scope.BindSearchGridMaster = function () {

        $scope.searchText = $scope.txtSearchString;
        BindCustomerGrid("", "", $scope.searchText);
    }


    //Grid Binding main Part
    function BindCustomerGrid(masterCustomerCode, searchText) {
        debugger;
        $scope.customerCodeGridOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: "/api/TemplateApi/GetCustomerListByCustomerCodeMaster?customerMasterCode=" + masterCustomerCode + '&searchText=' + searchText,
                },

                schema: {
                    type: "json",
                    model: {
                        fields: {
                            CUSTOMER_CODE: { type: "string" },
                            CUSTOMER_EDESC: { type: "string" }
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
                $("#customerGrid tbody tr").css("cursor", "pointer");
                $("#customerGrid tbody tr").on('dblclick', function () {
                    debugger;
                    var customercode = $(this).find('td span').html();
                    $scope.customersMasterChildArr.PARENT_TYPE = customercode;
                    if ($("#customer").hasClass('borderRed')) {
                        $scope.employee_error = "";
                        $("#customer").removeClass('borderRed');
                    }
                    $('#customerModalMasterCode').modal('toggle');
                    $scope.$apply();
                })
            },
            columns: [{
                field: "CUSTOMER_CODE",
                hidden: true,
                title: "Customer Code",

            }, {
                field: "MASTER_CUSTOMER_CODE",
                hidden: true,
                title: "Master Customer Code",

            }, {
                field: "CUSTOMER_EDESC",
                title: "Parent Group",

            }]
        };
    }


    $scope.BrowseTreeListForCustomerMasterCode = function (kendoEvent) {
        if ($scope.havRefrence == 'Y' && $scope.freeze_master_ref_flag == "Y") {
            var referencenumber = $('#refrencetype').val();
            if ($scope.ModuleCode != '01' && referencenumber !== "") {
                return;
            }
        }
        if ($scope.freeze_master_ref_flag == "N") {
            $('#customerModalMasterCode').modal('show');
        }
    }

    //employee popup advanced search// --end
});

