PMModule.controller('TolocationCtrl', function ($scope, $http, $routeParams, $window, $filter) {

    $scope.childRowIndex;

    $scope.TolocationDataSource = {
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


    //$scope.tolocationCodeOption = {
    //    dataSource: $scope.TolocationDataSource,
    //    dataBound: function (e) {
    //        
    //        var location = $("#tolocation").data("kendoComboBox");
    //        if (location != undefined) {
    //            location.setOptions({
    //                template: $.proxy(kendo.template("#= formatValue(LocationName,Type, this.text()) #"), location)
    //            });
    //        }
    //    }
    //}





    $scope.tolocationCodeOption = {
     
        dataSource: $scope.TolocationDataSource,
        template: '<span>{{dataItem.LocationName}}</span>  --- ' +
                  '<span>{{dataItem.Type}}</span>',
        dataBound: function (e) {
            if (this.element[0].attributes['location-index'] == undefined) {
                var location = $("#tolocation").data("kendoComboBox");
            }
            else {
                var index = this.element[0].attributes['location-index'].value;
                var locationLength = ((parseInt(index) + 1) * 3) - 1;
                var location = $($(".ctolocation")[locationLength]).data("kendoComboBox");
            }
        //    if (location != undefined) {
        //        location.setOptions({
        //            template: $.proxy(kendo.template("#= formatValue(LocationName,Type, this.text()) #"), location)
        //        });
        //    }
        }
    }



    $scope.MasterTolocationCodeOnChange = function (kendoEvent) {

        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.loactionerror = "Please Enter Valid Code."
            $('#tolocation').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.loactionerror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }



    //tolocation popup advanced search// --start

    var gettolocationsByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/Getlocation";
    $scope.treeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: gettolocationsByUrl,
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
        //if ($scope.childRowIndex == undefined) {
        //    
        //    $('#tolocationtree').data("kendoTreeView").expand('.k-item');
        //}
   if ($('#tolocationtree').data("kendoTreeView") != undefined) 
            $('#tolocationtree').data("kendoTreeView").expand('.k-item');
   }
 

    //treeview on select
    $scope.options = {
        loadOnDemand: false,
        select: function (e) {
            
            var currentItem = e.sender.dataItem(e.node);
            if ($scope.childRowIndex == undefined) {
                $('#tolocationGrid').removeClass("show-displaygrid");
                $("#tolocationGrid").html("");
            }
            else {
                $('#tolocationGrid_' + $scope.childRowIndex).removeClass("show-displaygrid");
                $("#tolocationGrid_" + $scope.childRowIndex).html("");
            }
            BindtolocationGrid(currentItem.LocationId, currentItem.LocationId, "");
            $scope.$apply();
        },
    };




    //search whole data on search button click
    $scope.BindSearchGrid = function () {
        $scope.searchText = $scope.locationtxtSearchString;
        BindtolocationGrid("", "", $scope.searchText);
    }


    //Grid Binding main Part
    function BindtolocationGrid(locationId, locationCode, searchText) {
       $scope.tolocationGridOptions = {
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
                    $("#tolocationGrid tbody tr").css("cursor", "pointer");
                    $("#tolocationGrid tbody tr").on('dblclick', function () {
                        
                        var locationcode = $(this).find('td span').html();
                        $scope.masterModels["TO_LOCATION_CODE"] = locationcode;
                        if ($("#tolocation").hasClass('borderRed')) {
                            $scope.loactionerror = "";
                            $("#tolocation").removeClass('borderRed');
                        }
                        $('#tolocationModal').modal('toggle');
                        $scope.$apply();
                    })
                } else {
                    $("#tolocationGrid_" + $scope.childRowIndex + " tbody tr").css("cursor", "pointer");
                    $("#tolocationGrid_" + $scope.childRowIndex + " tbody tr").on('dblclick', function () {
                        
                        var locationcode = $(this).find('td span').html();
                        $scope.childModels[$scope.childRowIndex]["TO_LOCATION_CODE"] = locationcode;
                       
                        if($($(".ctolocation_"+$scope.childRowIndex)[0]).closest('div').parent().hasClass('borderRed')){
                            $($(".ctolocation_" + $scope.childRowIndex)[0]).closest('div').parent().removeClass('borderRed')
                        }
                       
                        $('#tolocationModal_' + $scope.childRowIndex).modal('toggle');
                        $scope.$apply();
                    })
                }
            },
            columns: [{
                field: "LOCATION_CODE",
                //hidden: true,
                title: "Code",
                width: "80px"
            }, {
                field: "LOCATION_EDESC",
                title: "Location Name",
                width: "120px"

            }, {
                field: "ADDRESS",
                title: "Address",
                width: "120px"
            }, {
                field: "TELEPHONE_MOBILE_NO",
                title: "Phone",
                width: "120px"
            }
              , {
                field: "CREATED_BY",
                title: "Created By",
                width: "120px"
            }, {
                field: "CREATED_DATE",
                title: "Created Date",
                //template: "#= kendo.toString(CREATED_DATE,'dd MMM yyyy') #",
                template: "#= kendo.toString(kendo.parseDate(CREATED_DATE, 'yyyy-MM-dd'), 'dd MMM yyyy') #",
                width: "120px"
            },
            ]
        };
    }


    //show modal popup
    $scope.BrowseTreeListFortolocation = function (index) {
     
        if ($scope.havRefrence == 'Y' && $scope.freeze_master_ref_flag == "Y") {
            var referencenumber = $('#refrencetype').val();
            if ($scope.ModuleCode != '01' && referencenumber !== "") {
                return;
            }
        }
        if ($scope.freeze_master_ref_flag == "N") {
            $scope.childRowIndex = index;
            document.popupindex = index;
            if (index == undefined) {
                $('#tolocationModal').modal('show');
            }
            else {
                $('#tolocationModal_' + index).modal('show');
                if ($('#tolocationtree_' + $scope.childRowIndex).data("kendoTreeView") != undefined)
                    $('#tolocationtree_' + $scope.childRowIndex).data("kendoTreeView").expand('.k-item');
            }
        }
    }

    
  




    //tolocation popup advanced search// --end

});
