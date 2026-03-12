PMModule.controller('divisionCtrl', function ($scope, $http, $routeParams, $window, $filter) {

    $scope.FaDivisionSetupDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetdivisionListByFilter",
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
        }
    };

   

    $scope.divisionCodeOption = {
        dataSource: $scope.FaDivisionSetupDataSource,
        template: '<span>{{dataItem.DIVISION_EDESC}}</span>  ' +
        '<span>{{dataItem.Type}}</span>',
        dataBound: function (e) {
            if (this.element[0].attributes['division-index'] == undefined) {
                var division = $("#division").data("kendoComboBox");
            }
            else {
                var index = this.element[0].attributes['division-index'].value;
                var divisionLength = ((parseInt(index) + 1) * 3) - 1;
                var division = $($(".division")[divisionLength]).data("kendoComboBox");
            }
            if (division != undefined) {
                division.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(DIVISION_EDESC,Type, this.text()) #"), division)
                });
            }
        }
           
    }


    $scope.DivisionCodeOnChange = function (kendoEvent) {

        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.divisionerror = "Please Enter Valid Code."
            $('#division').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.divisionerror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }
    //division popup advanced search// --start
    $scope.onDataBound = function () {
        //$('#divisiontree').data("kendoTreeView").expand('.k-item');
          //$('#producttree_' + $scope.childRowIndex).data("kendoTreeView").expand('.k-item');
    }

    //treeview on select
    $scope.divisionoptions = {
        loadOnDemand: false,
        //select: function (e) {

        //    var currentItem = e.sender.dataItem(e.node);
        //    $('#divisionGrid').removeClass("show-displaygrid");
        //    $("#divisionGrid").html("");
        //    BindDivisionGrid(currentItem.divisionId, currentItem.divisionId, "");
        //    $scope.$apply();
        //},
        select: function (e) {
            var currentItem = e.sender.dataItem(e.node);
            if ($scope.childRowIndex == undefined) {
                $('#divisionGrid').removeClass("show-displaygrid");
                $("#divisionGrid").html("");
            }
            else {
                $('#divisionGrid_' + $scope.childRowIndex).removeClass("show-displaygrid");
                $("#divisionGrid_" + $scope.childRowIndex).html("");
            }
            BindDivisionGrid(currentItem.divisionId, currentItem.divisionId, "");
            $scope.$apply();
        },
    };
    //search whole data on search button click
    $scope.BindSearchGrid = function () {

        $scope.searchText = $scope.txtSearchString;
        BindDivisionGrid("", "", $scope.searchText);
    }


    //Grid Binding main Part
    function BindDivisionGrid(divisionId, divisionMasterCode, searchText) {
        $scope.divisionCodeGridOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: "/api/TemplateApi/GetdivisionListBydivisionCode?divisionId=" + divisionId + '&divisionMasterCode=' + divisionMasterCode + '&searchText=' + searchText,
                },

                schema: {
                    type: "json",
                    model: {
                        fields: {
                            DIVISION_CODE: { type: "string" },
                            DIVISION_EDESC: { type: "string" }
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
            //dataBound: function (e) {

            //    $("#divisionGrid tbody tr").css("cursor", "pointer");
            //    $("#divisionGrid tbody tr").on('dblclick', function () {

            //        var divisioncode = $(this).find('td span').html();
            //        $scope.masterModels["DIVISION_CODE"] = divisioncode;
            //        $('#divisionModal').modal('toggle');
            //        $scope.$apply();
            //    })
            //},
            dataBound: function (e) {
                
                if ($scope.childRowIndex == undefined) {
                    $("#divisionGrid tbody tr").css("cursor", "pointer");
                    $("#divisionGrid tbody tr").on('dblclick', function () {
                        var divisioncode = $(this).find('td span').html();
                        $scope.masterModels["DIVISION_CODE"] = divisioncode;
                        if ($("#division").hasClass('borderRed')) {
                            $scope.divisionerror = "";
                            $("#division").removeClass('borderRed');
                        }
                        $('#divisionModal').modal('toggle');
                        $scope.$apply();
                    })
                }
                else {
                    
                    $("#divisionGrid_" + $scope.childRowIndex + " tbody tr").css("cursor", "pointer");
                    $("#divisionGrid_" + $scope.childRowIndex + " tbody tr").on('dblclick', function () {
                        
                        var divisioncode = $(this).find('td span').html();
                        $scope.childModels[$scope.childRowIndex]["DIVISION_CODE"] = divisioncode;
                        if ($($(".cdivision_" + $scope.childRowIndex)[0]).closest('div').parent().hasClass('borderRed')) {
                            $($(".cdivision_" + $scope.childRowIndex)[0]).closest('div').parent().removeClass('borderRed')
                        }
                        $('#divisionModal_' + $scope.childRowIndex).modal('toggle');
                        $scope.$apply();
                    })
                }
            },
            columns: [{
                field: "DIVISION_CODE",
                hidden: true,
                title: "Division Code",

            }, {
               field: "DIVISION_EDESC",
               title: "Division Name",

            }, {
                    field: "ADDRESS",
                title: "Address",

            }, {
               field: "TELEPHONE_NO",
                title: "Phone",

            }]
        };
    }

    var getdivisionByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetDivision";
    $scope.divisiontreeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: getdivisionByUrl,
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
                id: "DIVISION_CODE",
                parentId: "PRE_DIVISION_CODE",
                children: "Items",
                fields: {
                    DIVISION_CODE: { field: "DIVISION_CODE", type: "string" },
                    DIVISION_EDESC: { field: "DIVISION_EDESC", type: "string" },
                    parentId: { field: "PRE_DIVISION_CODE", type: "string", defaultValue: "00" },
                }
            }
        }
    });
    $scope.BrowseTreeListForDivision = function (index) {
        
        if ($scope.havRefrence == 'Y' && $scope.freeze_master_ref_flag == "Y") {
            var referencenumber = $('#refrencetype').val();
            if ($scope.ModuleCode != '01' && referencenumber !== "") {
                return;
            }
        }
        //$('#divisionModal').modal('show');
        if ($scope.freeze_master_ref_flag == "N") {
            $scope.childRowIndex = index;
            document.popupindex = index;
            if (index == undefined) {
                $('#divisionModal').modal('show');
            }
            else {
                $('#divisionModal_' + index).modal('show');
                if ($('#divisiontree_' + $scope.childRowIndex).data("kendoTreeView") != undefined)
                    $('#divisiontree_' + $scope.childRowIndex).data("kendoTreeView").expand('.k-item');
            }
        }
    }

    //division popup advanced search// --end
});

