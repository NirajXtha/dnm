PMModule.controller('branchCtrl', function ($scope, $http, $routeParams, $window, $filter) {
    $scope.branchDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/InventoryApi/GetAllBranchCodeByFilter"
            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        //newParams = {
                        //    filter: data.filter.filters[0].value,

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
                            filter: "",

                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: "",

                    };
                    return newParams;
                }
            }
        },
    };

    $scope.branchCodeOption = {
        dataSource: $scope.branchDataSource,
        template: '<span>{{dataItem.AccountName}}</span>  ' +
        '<span>{{dataItem.Type}}</span>',
        dataBound: function (e) {
            
                var branch = $("#branchcode").data("kendoComboBox");
                if (branch != undefined) {
                branch.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(BRANCH_EDESC,Type, this.text()) #"), branch)
                });
            }
        }
    };
    $scope.BranchCodeOnChange = function (kendoEvent) {

        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.branchcodeerror = "Please Enter Valid Code."
            $('#branchcode').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.branchcodeerror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    };

    //fromlocation popup advanced search// --start

    var getbranchByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetBranch";
    $scope.treeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: getbranchByUrl,
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
                id: "BRANCH_CODE",
                parentId: "PRE_BRANCH_CODE",
                children: "Items",
                fields: {
                    BRANCH_CODE: { field: "BRANCH_CODE", type: "string" },
                    BRANCH_EDESC: { field: "BRANCH_EDESC", type: "string" },
                    parentId: { field: "PRE_BRANCH_CODE", type: "string", defaultValue: "00" },
                }
            }
        }
    });

    //treeview expand on startup
    $scope.onDataBound = function () {
        $('#Branchtree').data("kendoTreeView").expand('.k-item');
    }

    //treeview on select
    $scope.options = {
        loadOnDemand: false,
        select: function (e) {
            
            var currentItem = e.sender.dataItem(e.node);
            $('#BranchGrid').removeClass("show-displaygrid");
            $("#BranchGrid").html("");
            BindBranchGrid(currentItem.BranchId, currentItem.BranchCode, "");
            $scope.$apply();
        },
    };

    //search whole data on search button click
    $scope.BindSearchGrid = function () {
        
        $scope.searchText = $scope.branchtxtSearchString;
        BindBranchGrid("", "", $scope.searchText);
    }


    //Grid Binding main Part
    function BindBranchGrid(branchId, branchCode, searchText) {
        
        $scope.BranchGridOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: "/api/TemplateApi/GetbranchListBybranchCode?branchId=" + branchId + '&branchCode=' + branchCode + '&searchText=' + searchText,
                },

                schema: {
                    type: "json",
                    model: {
                        fields: {
                            BRANCH_CODE: { type: "string" },
                            BRANCH_EDESC: { type: "string" }
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
                $("#BranchGrid tbody tr").css("cursor", "pointer");
                $("#BranchGrid tbody tr").on('dblclick', function () {
                    var branchCode = $(this).find('td span').html();
                    $scope.masterModels["TO_BRANCH_CODE"] = branchCode;
                    if ($("#branchcode").hasClass('borderRed')) {
                        $scope.branchcodeerror = "";
                        $("#branchcode").removeClass('borderRed');
                    }
                    $('#BranchModal').modal('toggle');
                    $scope.$apply();
                })
            },
            columns: [{
                field: "BRANCH_CODE",
                hidden: true,
                title: "Code",

            }, {
                field: "BRANCH_EDESC",
                title: "Location Name",

            }, {
                field: "ADDRESS",
                title: "Address",

            }, {
                field: "EMAIL",
                title: "Email",

            }, {
                field: "TELEPHONE_NO",
                title: "Phone",

            }]
        };
    }


    //show modal popup
    $scope.BrowseTreeListForBranches = function (kendoEvent) {
        if ($scope.havRefrence == 'Y' && $scope.freeze_master_ref_flag == "Y") {
            var referencenumber = $('#refrencetype').val();
            if ($scope.ModuleCode != '01' && referencenumber !== "") {
                return;
            }
        }
        if ($scope.freeze_master_ref_flag == "N") {
            $('#BranchModal').modal('show');
        }
       
    }

    //fromlocation popup advanced search// --end


});