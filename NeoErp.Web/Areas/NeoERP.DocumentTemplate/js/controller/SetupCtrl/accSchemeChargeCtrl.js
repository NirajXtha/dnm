DTModule.controller('accSchemeChargeCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    $scope.accountCodeDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllAccountSetupByFilter",
            },
            parameterMap: function (data, action) {
                var macccodevalue = $rootScope.M_ACC_CODE_DEFAULTVAL;
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
                        //newParams = {
                        //    filter: ""
                        //};
                        //return newParams;

                        if (macccodevalue != "" && macccodevalue != undefined) {
                            newParams = {
                                filter: macccodevalue
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
                    //newParams = {
                    //    filter: ""
                    //};
                    //return newParams;
                    if (macccodevalue != "" && macccodevalue != undefined) {
                        newParams = {
                            filter: macccodevalue
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
        },
    };

    $scope.masteraccountCodeOption = {
        dataSource: $scope.accountCodeDataSource,
        template: '<span>{{dataItem.AccountName}}</span>  ' +
            '<span>{{dataItem.Type}}</span>',
        dataBound: function (e) {
            if (this.element[0].attributes['accmaster-index'] == undefined) {
                var acccode = $("#masteracccode").data("kendoComboBox");
            }
            else {
                var index = this.element[0].attributes['accmaster-index'].value;
                var acccodeLength = ((parseInt(index) + 1) * 3) - 1;
                var acccode = $($(".accmaster")[acccodeLength]).data("kendoComboBox");

            }
            if (acccode != undefined) {
                acccode.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(ACC_EDESC,Type, this.text()) #"), acccode)
                });
            }
        }
        //dataBound: function (e) {
        //    var Account = $("#masteracccode").data("kendoComboBox");
        //    if (Account != undefined) {
        //        Account.setOptions({
        //            template: $.proxy(kendo.template("#= formatValue(ACC_EDESC, this.text()) #"), Account)
        //        });
        //    }
        //}
    }


    $scope.MasterAccCodeOnChange = function (kendoEvent) {

        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.mainledgererror = "Please Enter Valid Code."
            $rootScope.mastervalidation = $scope.mainledgererror;
            $('#masteracccode').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.mainledgererror = "";
            $rootScope.mastervalidation = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    };

    //show modal popup
    $scope.BrowseTreeListForAccountCode = function () {
        if ($scope.havRefrence == 'Y') {
            var referencenumber = $('#refrencetype').val();
            if ($scope.ModuleCode != '01' && referencenumber !== "") {
                return;
            }
        }

        $('#AccountModel').modal('show');
    }

    //acccode popup advanced search// --start

    var getAccountCodeByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAccountCode";
    $scope.acctreeData = new kendo.data.HierarchicalDataSource({

        transport: {
            read: {
                url: getAccountCodeByUrl,
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
                id: "MASTER_ACC_CODE",
                parentId: "PRE_ACC_CODE",
                children: "Items",
                fields: {
                    ACC_CODE: { field: "ACC_CODE", type: "string" },
                    ACC_EDESC: { field: "ACC_EDESC", type: "string" },
                    parentId: { field: "PRE_ACC_CODE", type: "string", defaultValue: "00" },
                }
            }
        }
    });

    //Grid Binding main Part

    $scope.acconDataBound = function () {
        $('#accounttree').data("kendoTreeView").expand('.k-item');
    }

    //treeview on select
    $scope.accoptions = {
        loadOnDemand: false,
        select: function (e) {
            var currentItem = e.sender.dataItem(e.node);
            $('#accountGrid').removeClass("show-displaygrid");
            $("#accountGrid").html("");
            BindAccountCodeGrid(currentItem.AccountId, currentItem.masterAccountCode, "");
            $scope.$apply();
        },
    };
    $scope.BindSearchGrid = function () {
        $scope.searchText = $scope.txtSearchString;
        BindAccountCodeGrid("", "", $scope.searchText);
    }
    function BindAccountCodeGrid(accId, accMasterCode, searchText) {
        $scope.AccountCodeGridOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: "/api/TemplateApi/GetAccountListByAccountCode?accId=" + accId + '&accMastercode=' + accMasterCode + '&searchText=' + searchText,
                },

                schema: {
                    type: "json",
                    model: {
                        fields: {
                            ACC_CODE: { type: "string" },
                            ACC_EDESC: { type: "string" }
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
                $("#accountGrid tbody tr").css("cursor", "pointer");
                $("#accountGrid tbody tr").on('dblclick', function () {
                    debugger;
                    var acccode = $(this).find('td span').html();
                    //var accName = $(this).find('td span[ng-bind="dataItem.ACC_EDESC"]').html();
                    var accName = $(this).find('td span[ng-bind="dataItem.ACC_EDESC"]')[0].innerText;
                    console.log("AccountName==================>>>" + accName);
                    $("#masteracccode").data('kendoComboBox').dataSource.data([{ ACC_CODE: acccode, ACC_EDESC: accName, Type: "code" }]);
                    $scope.Scheme.CHARGE_ACCOUNT_CODE = acccode;
                    if ($("#masteracccode").hasClass('borderRed')) {
                        $scope.mainledgererror = "";
                        $("#masteracccode").removeClass('borderRed');
                    }
                    $('#AccountModel').modal('toggle');
                    $scope.$apply();
                })
            },
            columns: [{
                field: "ACC_CODE",
                //hidden: true,
                title: "Code",

            }, {
                field: "ACC_EDESC",
                title: "Account Name",

            },
            {
                field: "TRANSACTION_TYPE",
                title: "Transaction Type",

            },
            {
                field: "ACC_NATURE",
                title: "Account Nature",

            },
            {
                field: "CREATED_BY",
                title: "Created By",
                width: "120px"
            }, {
                field: "CREATED_DATE",
                title: "Created Date",
                template: "#= kendo.toString(CREATED_DATE,'dd MMM yyyy') #",
                //template: "#= kendo.toString(kendo.parseDate(CREATED_DATE, 'yyyy-MM-dd'), 'dd MMM yyyy') #",

            },


            ]
        };
    }
});

