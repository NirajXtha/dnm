
DTModule.controller('TolocationCtrl', function ($scope, $http, $routeParams, $window, $filter) {

    $scope.parentController = $scope.$parent.controllerName;


    $scope.childRowIndex;

    $scope.routineSearchText = "";

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


    $scope.routineSearchTextFun = function () {
        BindtolocationGrid("", "", $scope.searchText);
    };



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
            if (location != undefined) {
                location.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(LocationName,Type, this.text()) #"), location)
                });
            }
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

        if (searchText == undefined || searchText == 'undefined' || searchText == null || searchText == 'null') {
            searchText = "";
        }

        console.log('here we check parent scope data');
        //console.log($scope.$parent.invCtrlObject.SelectPlanDtl);
        let apiUrlLink = "/api/TemplateApi/GetlocationListBylocationCode?locationId=" + locationId + '&locationCode=' + locationCode + '&searchText=' + searchText;
        if ($scope.routineSearchText) {
            apiUrlLink = apiUrlLink + '&routineSearchText=' + $scope.routineSearchText;
        }

        if ($scope.$parent.controllerName == 'InventoryCtrl') {
            if ($scope.$parent.masterModels['REFERENCE_NO']) {
                apiUrlLink = apiUrlLink + '&referenceNo=' + $scope.$parent.masterModels['REFERENCE_NO'];
            }
        }

        if ($scope.$parent.invCtrlObject && $scope.$parent.invCtrlObject.SelectPlanDtl && $scope.$parent.invCtrlObject.SelectPlanDtl.PLAN_NO) {
            const planCode = $scope.$parent.invCtrlObject.SelectPlanDtl.PLAN_NO;
            apiUrlLink = "/api/TemplateApi/GetRoutineListByPlanCode?locationId=" + locationId + '&planCode=' + planCode + '&searchText=' + searchText;
        }

        $scope.tolocationGridOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: apiUrlLink
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
                debugger;
                if ($scope.childRowIndex == undefined) {
                    $("#tolocationGrid tbody tr").css("cursor", "pointer");
                    $("#tolocationGrid tbody tr").on('dblclick', function () {

                        debugger;
                        //  alert('here');
                        var locationcode = $(this).find('td span').html();
                        $scope.masterModels["TO_LOCATION_CODE"] = locationcode;
                        if ($("#tolocation").hasClass('borderRed')) {
                            $scope.loactionerror = "";
                            $("#tolocation").removeClass('borderRed');
                        }
                        $('#tolocationModal').modal('toggle');




                        // added code while working with production module
                        if ($scope.$parent && $scope.$parent.invCtrlObject && $scope.$parent.invCtrlObject.SelectPlanDtl && $scope.$parent.invCtrlObject.SelectPlanDtl.PLAN_NO) {
                            $scope.GetSelectedItemQty(locationcode);
                        } else if ($scope.$parent && $scope.$parent.getVoucherDetailForReferenceProduct) {
                            $scope.$parent.getVoucherDetailForReferenceProduct();
                        }


                        $scope.$apply();
                    })
                } else {
                    $("#tolocationGrid_" + $scope.childRowIndex + " tbody tr").css("cursor", "pointer");
                    $("#tolocationGrid_" + $scope.childRowIndex + " tbody tr").on('dblclick', function () {
                        var locationcode = $(this).find('td span').html();
                        $scope.childModels[$scope.childRowIndex]["TO_LOCATION_CODE"] = locationcode;

                        if ($($(".ctolocation_" + $scope.childRowIndex)[0]).closest('div').parent().hasClass('borderRed')) {
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


    $scope.GetSelectedItemQty = function (locationcode) {

        console.log(locationcode);
        //  alert(locationcode);

        $http.get("/api/TemplateApi/" + 'GetQuantityByPlanCodeAndLocationCode'
            , { params: { planCode: $scope.$parent.invCtrlObject.SelectPlanDtl.PLAN_NO, lcode: locationcode } }
        ).then(function (response) {
            if (response && response.data) {
                $scope.$parent.invCtrlObject.QtyFromPlanCode = response.data;

                //if there is plan with reference number and already select REFERENCE_NO
                if ($scope.$parent.masterModels["REFERENCE_NO"]) {
                    $scope.$parent.masterModels["PRODUCTION_QTY"] = $scope.$parent.masterModels["PRODUCTION_QTY"];
                } else {
                    $scope.$parent.masterModels["PRODUCTION_QTY"] = $scope.$parent.invCtrlObject.QtyFromPlanCode;
                }

                setTimeout(function () {
                    $scope.$parent.getVoucherDetailForReferenceProduct();
                }, 50);
            }
        }, function (error) {
            // Error callback
            console.error('Error occurred:', error);
        });
    };



    //show modal popup
    $scope.BrowseTreeListFortolocation = function (index) {

        debugger;
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

        //if ($scope.$parent.invCtrlObject && $scope.$parent.invCtrlObject.SelectPlanDtl && $scope.$parent.invCtrlObject.SelectPlanDtl.PLAN_NO > 0) {
        //    console.log($scope.treeData);
        //    BindtolocationGrid(04, 04, "");
        //}
        //BindtolocationGrid(04, 04, "");

        BindtolocationGrid("", "", "");



        //if ($scope.$parent.controllerName !== 'InventoryCtrl') {
        //    alert('InventoryCtrl');
        //    //alert($scope.$parent.controllerName);
        //    //✅ Apply CSS to.k - grid table after render
        //    setTimeout(function () {
        //        $(".k-grid table").css({
        //            "display": "flex"   // 👈 Added flex
        //        });
        //    }, 50);
        //}






    }







    //tolocation popup advanced search// --end

});



DTModule.controller('TolocationCtrlScrn2', function ($scope, $http, $routeParams, $window, $filter) {

    $scope.parentControllerName = $scope.$parent.controllerName;

    $scope.childRowIndex;

    $scope.routineSearchTextScrn2 = "";

    $scope.TolocationDataSourceScrn2 = {
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


    $scope.routineSearchTextFun = function () {
        BindtolocationGrid("", "", $scope.searchText);
    };



    $scope.tolocationCodeOptionScrn2 = {

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
            if (location != undefined) {
                location.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(LocationName,Type, this.text()) #"), location)
                });
            }
        }
    }



    $scope.MasterTolocationCodeOnChange = function (kendoEvent) {
        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.loactionerror = "Please Enter Valid Code."
            $('#tolocationScrn2').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.loactionerror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }



    //tolocation popup advanced search// --start

    var gettolocationsByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/Getlocation";
    $scope.treeDataScrn2 = new kendo.data.HierarchicalDataSource({
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
        if ($('#tolocationtreeScrn2').data("kendoTreeView") != undefined)
            $('#tolocationtreeScrn2').data("kendoTreeView").expand('.k-item');
    }


    //treeview on select
    $scope.options = {
        loadOnDemand: false,
        select: function (e) {

            var currentItem = e.sender.dataItem(e.node);

            if ($scope.childRowIndex == undefined) {
                $('#tolocationGridScrn2').removeClass("show-displaygrid");
                $("#tolocationGridScrn2").html("");
            }
            else {
                $('#tolocationGridScrn2_' + $scope.childRowIndex).removeClass("show-displaygrid");
                $("#tolocationGridScrn2_" + $scope.childRowIndex).html("");
            }
            BindtolocationGrid(currentItem.LocationId, currentItem.LocationId, "");
            $scope.$apply();
        },
    };




    //search whole data on search button click
    $scope.BindSearchGrid = function () {

        $scope.searchText = $scope.locationtxtSearchStringScrn2;
        BindtolocationGrid("", "", $scope.searchText);
    }


    //Grid Binding main Part
    function BindtolocationGrid(locationId, locationCode, searchText) {

        if (searchText == undefined || searchText == 'undefined' || searchText == null || searchText == 'null') {
            searchText = "";
        }

        console.log('here we check parent scope data');
        //console.log($scope.$parent.invCtrlObject.SelectPlanDtl);
        let apiUrlLink = "/api/TemplateApi/GetlocationListBylocationCode?locationId=" + locationId + '&locationCode=' + locationCode + '&searchText=' + searchText;
        if ($scope.routineSearchText) {
            apiUrlLink = apiUrlLink + '&routineSearchText=' + $scope.routineSearchText;
        }

        if ($scope.$parent.controllerName == 'InventoryCtrl') {
            if ($scope.$parent.masterModels['REFERENCE_NO']) {
                apiUrlLink = apiUrlLink + '&referenceNo=' + $scope.$parent.masterModels['REFERENCE_NO'];
            }
        }

        if ($scope.$parent.invCtrlObject && $scope.$parent.invCtrlObject.SelectPlanDtl && $scope.$parent.invCtrlObject.SelectPlanDtl.PLAN_NO) {
            const planCode = $scope.$parent.invCtrlObject.SelectPlanDtl.PLAN_NO;
            apiUrlLink = "/api/TemplateApi/GetRoutineListByPlanCode?locationId=" + locationId + '&planCode=' + planCode + '&searchText=' + searchText;
        }

        $scope.tolocationGridOptionsScrn2 = {
            dataSource: {
                type: "json",
                transport: {
                    read: apiUrlLink
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
                debugger;
                if ($scope.childRowIndex == undefined) {
                    $("#tolocationGridScrn2 tbody tr").css("cursor", "pointer");
                    $("#tolocationGridScrn2 tbody tr").on('dblclick', function () {

                        debugger;

                        //  alert('here');
                        var locationcode = $(this).find('td span').html();
                        $scope.masterModels["TO_LOCATION_CODE"] = locationcode;

                        if ($scope.masterModels.hasOwnProperty("TO_LOCATION_CODE")) {
                            var req = "/api/TemplateApi/GetLoactionNameByCode?locationcode=" + locationcode;
                            $http.get(req).then(function (results) {
                                setTimeout(function () {
                                    $("#tolocationScrn2").data('kendoComboBox').dataSource.data([{ LocationCode: locationcode, LocationName: results.data, Type: "code" }]);
                                }, 10);
                            });
                        }



                        if ($("#tolocationScrn2").hasClass('borderRed')) {
                            $scope.loactionerror = "";
                            $("#tolocationScrn2").removeClass('borderRed');
                        }
                        $('#tolocationModalScrn2').modal('toggle');



                        if ($scope.$parent.invCtrlObject && $scope.$parent.invCtrlObject.SelectPlanDtl && $scope.$parent.invCtrlObject.SelectPlanDtl.PLAN_NO) {
                            $scope.GetSelectedItemQty(locationcode);
                        } else {

                            $scope.$parent.getVoucherDetailForReferenceProduct();
                        }


                        $scope.$apply();
                    })
                } else {
                    $("#tolocationGridScrn2_" + $scope.childRowIndex + " tbody tr").css("cursor", "pointer");
                    $("#tolocationGridScrn2_" + $scope.childRowIndex + " tbody tr").on('dblclick', function () {
                        var locationcode = $(this).find('td span').html();
                        $scope.childModels[$scope.childRowIndex]["TO_LOCATION_CODE"] = locationcode;

                        if ($($(".ctolocation_" + $scope.childRowIndex)[0]).closest('div').parent().hasClass('borderRed')) {
                            $($(".ctolocation_" + $scope.childRowIndex)[0]).closest('div').parent().removeClass('borderRed')
                        }

                        $('#tolocationModalScrn2_' + $scope.childRowIndex).modal('toggle');
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


    $scope.GetSelectedItemQty = function (locationcode) {

        console.log(locationcode);
        //  alert(locationcode);

        $http.get("/api/TemplateApi/" + 'GetQuantityByPlanCodeAndLocationCode'
            , { params: { planCode: $scope.$parent.invCtrlObject.SelectPlanDtl.PLAN_NO, lcode: locationcode } }
        ).then(function (response) {
            if (response && response.data) {
                $scope.$parent.invCtrlObject.QtyFromPlanCode = response.data;

                //if there is plan with reference number and already select REFERENCE_NO
                if ($scope.$parent.masterModels["REFERENCE_NO"]) {
                    $scope.$parent.masterModels["PRODUCTION_QTY"] = $scope.$parent.masterModels["PRODUCTION_QTY"];
                } else {
                    $scope.$parent.masterModels["PRODUCTION_QTY"] = $scope.$parent.invCtrlObject.QtyFromPlanCode;
                }

                setTimeout(function () {
                    $scope.$parent.getVoucherDetailForReferenceProduct();
                }, 50);
            }
        }, function (error) {
            // Error callback
            console.error('Error occurred:', error);
        });
    };



    //show modal popup
    $scope.BrowseTreeListFortolocation = function (index) {

        debugger;
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
                $('#tolocationModalScrn2').modal('show');
            }
            else {
                $('#tolocationModalScrn2_' + index).modal('show');
                if ($('#tolocationtreeScrn2_' + $scope.childRowIndex).data("kendoTreeView") != undefined)
                    $('#tolocationtreeScrn2_' + $scope.childRowIndex).data("kendoTreeView").expand('.k-item');
            }
        }

        //if ($scope.$parent.invCtrlObject && $scope.$parent.invCtrlObject.SelectPlanDtl && $scope.$parent.invCtrlObject.SelectPlanDtl.PLAN_NO > 0) {
        //    console.log($scope.treeData);
        //    BindtolocationGrid(04, 04, "");
        //}
        //BindtolocationGrid(04, 04, "");

        BindtolocationGrid("", "", "");



        //if ($scope.$parent.controllerName !== 'InventoryCtrl') {
        //    alert('InventoryCtrl');
        //    //alert($scope.$parent.controllerName);
        //    //✅ Apply CSS to.k - grid table after render
        //    setTimeout(function () {
        //        $(".k-grid table").css({
        //            "display": "flex"   // 👈 Added flex
        //        });
        //    }, 50);
        //}
    }

    //tolocation popup advanced search// --end

});

