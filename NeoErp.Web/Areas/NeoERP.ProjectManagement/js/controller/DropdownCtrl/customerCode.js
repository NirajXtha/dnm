
PMModule.controller('customerCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {


    $scope.checkNodes = [];
    $scope.tempCustomerDataSource = [];
    $scope.customerDataSource = {
        type: "json",
        serverFiltering: true,
        suggest: true,
        highlightFirst: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllCustomerSetupByFilter",

            },
            parameterMap: function (data, action) {
                var mc_defa_val = $rootScope.CUSTOMER_CODE_DEFVAL_MASTER;

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
                                filter: ""
                            };
                            return newParams;
                        }
                    }
                    else {
                        if (mc_defa_val != "") {
                            newParams = {
                                //filter: "code#" + mc_defa_val
                                filter: mc_defa_val
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
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }

                //if (data.filter != undefined) {

                //    if (data.filter.filters[0] != undefined) {                     
                //        if (data.filter.filters[0].value != "") {
                //            newParams = {
                //                filter: data.filter.filters[0].value
                //            };
                //            return newParams;
                //        }
                //        else {
                //            newParams = {
                //                filter: "!@$"
                //            };
                //            return newParams;
                //        }
                //    }
                //    else {
                //        newParams = {
                //            filter: ""
                //        };
                //        return newParams;
                //    }
                //}
                //else {
                //    newParams = {
                //        filter: ""
                //    };
                //    return newParams;
                //}
            }
        },
    };

    $scope.orientation = "horizontal";

    $scope.customerCodeOption = {
        dataSource: $scope.customerDataSource,
        template: '<span>{{dataItem.CustomerName}}</span>  --- ' +
            '<span>{{dataItem.Type}}</span>',
        dataBound: function (e) {

            $scope.tempCustomerDataSource = $("#customers").data('kendoComboBox').dataSource.data();
            if (this.element[0].attributes['customer-index'] == undefined) {
                var customer = $("#customers").data("kendoComboBox");
            }
            else {
                var index = this.element[0].attributes['customer-index'].value;
                var customerLength = ((parseInt(index) + 1) * 3) - 1;
                var customer = $($(".customer")[customerLength]).data("kendoComboBox");

            }
            if (customer != undefined) {
                customer.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(CustomerName, Type, this.text()) #"), customer)
                });
            }
            $scope.$apply();
        },
        select: function (e) {
            debugger;
            var currentCustomer = e.dataItem.CustomerCode;
            if (currentCustomer !== undefined) {
                BindDealer(currentCustomer, "");
                BindPriceList(currentCustomer, "");
            }

        }
    }

    //master validation
    $scope.customerCodeOnChange = function (kendoEvent) {
        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.customererror = "Please Enter Valid Code."
            $('#customers').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.customererror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }
    $scope.onChange = function (kendoEvent) {
        $(".divsearchoption").css({ display: 'none' });
        $("#RefrenceModel").modal('toggle');
        //$scope.bindReferenceGrid(true);
    }
    //customer popup advanced search// --start

    //$scope.CustomerOnSelect = function (kendoEvent)
    //{
    //    

    //    $scope.masterModels["CUSTOMER_EDESC"] = kendoEvent.dataItem.CustomerName;
    //}

    var getCustomersByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetCustomers";
    $scope.treeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: getCustomersByUrl,
                type: 'GET',
                data: function (data, evt) {
                }
            },

        },
        schema: {
            parse: function (data) {

                return data;
            },
            model: {
                id: "MASTER_CUSTOMER_CODE",
                parentId: "PRE_CUSTOMER_CODE",
                children: "Items",
                fields: {
                    CUSTOMER_CODE: { field: "CUSTOMER_CODE", type: "string" },
                    CUSTOMER_EDESC: { field: "CUSTOMER_EDESC", type: "string" },
                    parentId: { field: "PRE_CUSTOMER_CODE", type: "string", defaultValue: "00" },
                }
            }
        }
    });

    //treeview expand on startup
    $scope.onDataBound = function () {

        //$('#customertree').data("kendoTreeView").expand('.k-item');
    }

    //treeview on select
    $scope.options = {

        loadOnDemand: false,
        select: function (e) {

            var currentItem = e.sender.dataItem(e.node);
            $('#customerGrid').removeClass("show-displaygrid");
            $("#customerGrid").html("");
            BindCustomerGrid(currentItem.customerId, currentItem.masterCustomerCode, "");
            $scope.CustomerId;
            $scope.$apply();
        },
    };

    //search whole data on search button click
    $scope.BindSearchGrid = function () {
        debugger;
        $scope.searchText = $scope.txtSearchString;
        BindCustomerGrid("", "", $scope.searchText);
    }



    //Grid Binding main Part
    function BindCustomerGrid(customerId, customerMasterCode, searchText) {
         debugger;
        $scope.customerGridOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: "/api/TemplateApi/GetCustomerListByCustomerCode?customerId=" + customerId + '&customerMasterCode=' + customerMasterCode + '&searchText=' + searchText,
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
            toolbar: kendo.template($("#toolbar-template").html()),
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
                $("#customerGrid tbody tr").css("cursor", "pointer");
                //DisplayNoResultsFound($('#kGrid'));
                DisplayNoResultsFound($('#customerGrid'));
                $("#customerGrid tbody tr").on('dblclick', function () {

                    var customercode = $(this).find('td span').html();
                    var CustomerNameGrid = $(this).find('td span[ng-bind="dataItem.CUSTOMER_EDESC"]').html();
                    var caddress = $(this).find('td span[ng-bind="dataItem.REGD_OFFICE_EADDRESS"]').html();
                    var vatpanno = $(this).find('td span[ng-bind="dataItem.TPIN_VAT_NO"]').html();
                    var ctelepho = $(this).find('td span[ng-bind="dataItem.TEL_MOBILE_NO1"]').html();
                    var cndesc = $(this).find('td span[ng-bind="dataItem.CUSTOMER_NDESC"]').html();
                    $scope.masterModels["CUSTOMER_CODE"] = customercode;
                    //$("#customers").data('kendoComboBox').dataSource.data([{ CustomerCode: customercode, CustomerName: CustomerNameGrid, Type: "code" }]);
                    $("#customers").data('kendoComboBox').dataSource.data([{ CustomerCode: customercode, CustomerName: CustomerNameGrid, Type: "code", REGD_OFFICE_EADDRESS: caddress, TPIN_VAT_NO: vatpanno, TEL_MOBILE_NO1: ctelepho, CUSTOMER_NDESC: cndesc }]);
                    if ($("#customers").hasClass('borderRed')) {
                        $scope.customererror = "";
                        $("#customers").removeClass('borderRed');
                    }
                    BindDealer(customercode, "");
                    BindPriceList(customercode, "");
                    $('#customerModal').modal('toggle');
                    $scope.$apply();
                })
            },
            columns: [{
                field: "CUSTOMER_CODE",

                title: "Code",
                width: "80px"

            }, {
                field: "CUSTOMER_EDESC",
                title: "Customer Name",
                width: "120px"

            }, {
                field: "REGD_OFFICE_EADDRESS",
                title: "Address",
                width: "120px"

            },
            {
                field: "TPIN_VAT_NO",
                title: "Vat",
                width: "120px"

            },
            {
                field: "CUSTOMER_NDESC",
                title: "Customer",
                width: "120px"

            },
            {
                field: "TEL_MOBILE_NO1",
                title: "Phone",
                width: "120px"
            },
            {
                field: "CREATED_BY",
                title: "Created By",
                width: "120px"
            }
                ,
            {
                field: "CREATED_DATE",
                title: "Created Date",
                template: "#= kendo.toString(kendo.parseDate(CREATED_DATE, 'yyyy-MM-dd'), 'dd MMM yyyy') #",
                width: "120px"
            }
            ]
        };
    }


   

    //show modal popup
    $scope.BrowseTreeListForCustomers = function (kendoEvent) {
        if ($scope.havRefrence == 'Y' && $scope.freeze_master_ref_flag == "Y") {
            var referencenumber = $('#refrencetype').val();
            if ($scope.ModuleCode != '01' && referencenumber !== "") {
                return;
            }
        }
        if ($scope.freeze_master_ref_flag == "N") {
            $('#customerModal').modal('show');
        }
    }
    function DisplayNoResultsFound(grid) {

        // Get the number of Columns in the grid
        //var grid = $("#kGrid").data("kendo-grid");
        var dataSource = grid.data("kendoGrid").dataSource;
        var colCount = grid.find('.k-grid-header colgroup > col').length;

        // If there are no results place an indicator row
        if (dataSource._view.length == 0) {
            grid.find('.k-grid-content tbody')
                .append('<tr class="kendo-data-row"><td colspan="' + colCount + '" style="text-align:center"><b>No Results Found!</b></td></tr>');
        }

        // Get visible row count
        var rowCount = grid.find('.k-grid-content tbody tr').length;

        // If the row count is less that the page size add in the number of missing rows
        if (rowCount < dataSource._take) {
            var addRows = dataSource._take - rowCount;
            for (var i = 0; i < addRows; i++) {
                grid.find('.k-grid-content tbody').append('<tr class="kendo-data-row"><td>&nbsp;</td></tr>');
            }
        }
    }
    //customer popup advanced search// --end

    function BindDealer(CustomerCode, searchText) {

        var getdealerByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetPartyTypeByFilterAndCustomerCode?filter=" + searchText + '&customercode=' + CustomerCode;
        //$scope.DealerDataSource = {
        //    type: "json",
        //    serverFiltering: true,
        //    suggest: true,
        //    highlightFirst: true,
        //    transport: {
        //        read: {
        //            read: getdealerByUrl,

        //        },
        //        parameterMap: function (data, action) {

        //            var newParams;
        //            if (data.filter != undefined) {
        //                if (data.filter.filters[0] != undefined) {

        //                    if (data.filter.filters[0].value != "") {
        //                        newParams = {
        //                            filter: data.filter.filters[0].value
        //                        };
        //                        return newParams;
        //                    }
        //                    else {
        //                        newParams = {
        //                            filter: "!@$"
        //                        };
        //                        return newParams;
        //                    }
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
        //$scope.DealerOption = {
        //    dataSource: $scope.DealerDataSource,
        //    template: '<span>{{dataItem.PARTY_TYPE_CODE}}</span>  ' +
        //        '<span>{{dataItem.PARTY_TYPE_EDESC}}</span>',
        //    dataBound: function (e) {


        //    }
        //}

        $("#dealercode").kendoComboBox({
            optionLabel: "--Select Dealer Code--",
            filter: "contains",
            dataTextField: "PARTY_TYPE_EDESC",
            dataValueField: "PARTY_TYPE_CODE",
            dataBound: function (e) {

                if (this.select() === -1) { //check whether any item is selected
                    this.select(0);
                    this.trigger("change");
                }
            },
            autobind: true,
            suggest: true,
            dataSource: {
                type: "json",
                serverFiltering: true,
                transport: {
                    read: {

                        url: getdealerByUrl,

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
            },
            index:0,
            select: function (e) {

                $('#style-switcher').addClass('opened');
                $('#style-switcher').animate({ 'left': '-241px', 'width': '273px' });

            }
        });
    }
    function BindPriceList(CustomerCode, searchText) {

        var getpricelistByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetPriceListByFilterAndCustomerCode?filter=" + searchText + '&customercode=' + CustomerCode;
        $("#customerpriceid").kendoComboBox({
            optionLabel: "--Select Price List--",
            filter: "contains",
            dataTextField: "PRICE_LIST_NAME",
            dataValueField: "MASTER_ID",

            autobind: true,
            suggest: true,
            dataBound: function (e) {
                debugger;
                if (this.select() === -1) {
                    //check whether any item is selected
                    this.select(0);
                    this.trigger("change");
                }
            },
            dataSource: {
                type: "json",
                serverFiltering: true,
                transport: {
                    read: {

                        url: getpricelistByUrl,

                    },
                    parameterMap: function (data, action) {
                        debugger;
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
            },
            select: function (e) {
                debugger;
                $('#style-switcher').addClass('opened');
                $('#style-switcher').animate({ 'left': '-241px', 'width': '273px' });

            }

        });
    }
});