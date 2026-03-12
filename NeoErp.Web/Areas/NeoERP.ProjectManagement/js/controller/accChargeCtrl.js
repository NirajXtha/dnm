(function ($) {
    'use strict';
    var accChargeCtrl = angular.module("accChargeCtrl", [])
    accChargeCtrl.controller('accChargeCtrl', ['$scope', '$http', '$routeParams', '$window', '$filter', function ($scope, $http, $routeParams, $window, $filter) {
    $scope.changeIndex;

    $scope.chargeAccountCodeDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllAccountSetupByFilterCharge",
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
        },
    };
    $scope.chargeAccountCodeOption = {
        dataSource: $scope.chargeAccountCodeDataSource,
        dataTextField: 'ACC_EDESC',
        dataValueField: 'ACC_CODE',
        filter: 'contains',
        select: function (e) {
        },
        dataBound: function (e) {
            if (this.element[0].attributes['chargeAcc-index'] == undefined) {
                var acccode = $(".accCodeAutoComplete").data("kendoComboBox");
            }
            else {
                var index = this.element[0].attributes['chargeAcc-index'].value;
                var accLength = ((parseInt(index) + 1) * 3) - 1;
                var acccode = $($(".cacccode")[accLength]).data("kendoComboBox");

            }
            if (acccode != undefined) {
                //acccode.setOptions({
                //    template: $.proxy(kendo.template("#= formatValue(ACC_EDESC, Type, this.text()) #"), acccode)
                //});
            }
        },
        change: function (e) {


        }
    }
    //show modal popup
    $scope.BrowseTreeListForChargeAccountCode = function (index) {
     
        //if ($scope.havRefrence == 'Y' && $scope.freeze_master_ref_flag == "N") {
            $scope.changeIndex = index;
            document.chargeAccIndex = index;
            $('#chargeAccountModel_' + index).modal('show');
            if ($('#chargeAccountTree_' + $scope.changeIndex).data("kendoTreeView") != undefined)
                $('#chargeAccountTree_' + $scope.changeIndex).data("kendoTreeView").expand('.k-item');
        //}
    }

    var getAccountCodeByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAccountCode";
    $scope.chargeAcctreeData = new kendo.data.HierarchicalDataSource({

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
        //if ($('#chargeAccountTree_' + $scope.changeIndex).data("kendoTreeView")!=undefined)
        //    $('#chargeAccountTree_'+$scope.changeIndex).data("kendoTreeView").expand('.k-item');
    }

    //treeview on select
    $scope.chargeAccoptions = {
        loadOnDemand: false,
        select: function (e) {
            var currentItem = e.sender.dataItem(e.node);
            $('#chargeAccountGrid_' + $scope.changeIndex).removeClass("show-displaygrid");
            $("#chargeAccountGrid_" + $scope.changeIndex).html("");
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
                $("#chargeAccountGrid_" + $scope.changeIndex + " tbody tr").css("cursor", "pointer");
                $("#chargeAccountGrid_" + $scope.changeIndex + " tbody tr").off('click');
                $("#chargeAccountGrid_" + $scope.changeIndex + " tbody tr").on('click', function () {
                    var acccode = $(this).find('td span').html();
                    $scope.ChargeList[$scope.changeIndex]["ACC_CODE"] = acccode;
                    $('#chargeAccountModel_' + $scope.changeIndex).modal('toggle');
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
}]);
})(jQuery);

