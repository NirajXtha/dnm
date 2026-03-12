
PMModule.controller('locationCtrl', function ($scope, $http, $routeParams, $window, $filter) {

    $scope.childRowIndex;

    $scope.locationDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllLocationListByFilter",

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

    $scope.MasterlocationCodeOnChange = function (kendoEvent) {

        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.loactionerror = "Please Enter Valid Code."
            $('#location').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.loactionerror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }


    $scope.locationCodeOption = {
        dataSource: $scope.locationDataSource,
        template: '<span>{{dataItem.LocationName}}</span>  --- ' +
            '<span>{{dataItem.Type}}</span>',
        //minLength: 3,
        dataBound: function (e) {
            if (this.element[0].attributes['location-index'] == undefined) {
                var location = $("#location").data("kendoComboBox");
            }
            else {
                var index = this.element[0].attributes['location-index'].value;
                var locationLength = ((parseInt(index) + 1) * 3) - 1;
                var location = $($(".location")[locationLength]).data("kendoComboBox");
            }
            //if (location != undefined) {
            //    location.setOptions({
            //        template: $.proxy(kendo.template("#= formatValue(LocationName,Type, this.text()) #"), location)
            //    });
            //}
        }
    }


    //fromlocation popup advanced search// --start

    var getfromlocationsByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/Getlocation";
    $scope.treeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: getfromlocationsByUrl,
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
                id: "LOCATION_CODE",
                parentId: "PRE_LOCATION_CODE",
                children: "Items",
                fields: {
                    LOCATION_CODE: { field: "LOCATION_CODE", type: "string" },
                    LOCATION_EDESC: { field: "LOCATION_EDESC", type: "string" },
                    parentId: { field: "PRE_LOCATION_CODE", type: "string", defaultValue: "00" },
                }
            }
        }
    });

    //treeview expand on startup
    $scope.onDataBound = function () {
        if ($scope.childRowIndex == undefined)
            if ($('#fromlocationtree').data("kendoTreeView") != undefined)
                $('#fromlocationtree').data("kendoTreeView").expand('.k-item');
    }

    //treeview on select
    $scope.options = {
        loadOnDemand: false,
        select: function (e) {
            var currentItem = e.sender.dataItem(e.node);
            if ($scope.childRowIndex == undefined) {
                $('#locationGrid').removeClass("show-displaygrid");
                $("#locationGrid").html("");
            }
            else {
                $('#locationGrid_' + $scope.childRowIndex).removeClass("show-displaygrid");
                $("#locationGrid_" + $scope.childRowIndex).html("");
            }
            BindlocationGrid(currentItem.LocationId, currentItem.LocationId, "");
            $scope.$apply();
        },
    };

    //search whole data on search button click
    $scope.BindSearchGrid = function () {

        $scope.searchText = $scope.locationtxtSearchString;
        BindlocationGrid("", "", $scope.searchText);
    }


    //Grid Binding main Part
    function BindlocationGrid(locationId, locationCode, searchText) {

        $scope.locationGridOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: "/api/TemplateApi/GetlocationListBylocationCode?locationId=" + locationId + '&locationCode=' + locationCode + '&searchText=' + searchText,
                },

                schema: {
                    type: "json",
                    model: {
                        fields: {
                            LOCATION_CODE: { type: "string" },
                            LOCATION_EDESC: { type: "string" }
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

                if ($scope.childRowIndex == undefined) {
                    $("#locationGrid tbody tr").css("cursor", "pointer");
                    DisplayNoResultsFound($('#locationGrid'));
                    $("#locationGrid tbody tr").on('dblclick', function () {

                        var locationcode = $(this).find('td span').html();
                        $scope.masterModels["FROM_LOCATION_CODE"] = locationcode;
                        if ($("#location").hasClass('borderRed')) {
                            $scope.loactionerror = "";
                            $("#location").removeClass('borderRed');
                        }
                        $('#locationModal').modal('toggle');
                        $scope.$apply();
                    })
                }
                else {
                    $("#locationGrid_" + $scope.childRowIndex + " tbody tr").css("cursor", "pointer");
                    $("#locationGrid_" + $scope.childRowIndex + " tbody tr").on('dblclick', function () {

                        var locationcode = $(this).find('td span').html();
                        $scope.childModels[$scope.childRowIndex]["FROM_LOCATION_CODE"] = locationcode;

                        if ($($(".clocation_" + $scope.childRowIndex)[0]).closest('div').parent().hasClass('borderRed')) {
                            $($(".clocation_" + $scope.childRowIndex)[0]).closest('div').parent().removeClass('borderRed')
                        }
                        $('#locationModal_' + $scope.childRowIndex).modal('toggle');
                        $scope.$apply();
                    })
                }
            },
            columns: [{
                field: "LOCATION_CODE",
                title: "Code",


            }, {
                field: "LOCATION_EDESC",
                title: "Location Name",


            }, {
                field: "ADDRESS",
                title: "Address",


            },
            {
                field: "TELEPHONE_MOBILE_NO",
                title: "Phone",


            },
            {
                field: "CREATED_BY",
                title: "Created By",

            }, {
                field: "CREATED_DATE",
                title: "Created Date",
                //template: "#= kendo.toString(CREATED_DATE,'dd MMM yyyy') #",
                template: "#= kendo.toString(kendo.parseDate(CREATED_DATE, 'yyyy-MM-dd'), 'dd MMM yyyy') #",

            },
            ]
        };
    }


    //show modal popup
    $scope.BrowseTreeListForlocation = function (index) {

        if ($scope.havRefrence == 'Y' && $scope.freeze_master_ref_flag == "Y") {
            var referencenumber = $('#refrencetype').val();
            var custref = $(".referenceCustomer").val();
            var btnref = $("#refrenceTypeMultiSelect").data('kendoDropDownList').dataItem().REF_CODE;
            if ($scope.ModuleCode != '01' && (referencenumber !== "" || custref !== "" || btnref !== "")) {
                return;
            }
        }
        if ($scope.freeze_master_ref_flag == "N") {
            $scope.childRowIndex = index;
            document.popupindex = index;
            if (index == undefined) {
                $('#locationModal').modal('show');
            }
            else {
                $('#locationModal_' + index).modal('show');
                if ($('#fromlocationtree_' + $scope.childRowIndex).data("kendoTreeView") != undefined)
                    $('#fromlocationtree_' + $scope.childRowIndex).data("kendoTreeView").expand('.k-item');
            }
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
    //fromlocation popup advanced search// --end

    //    }]);
});
//})(jQuery);
