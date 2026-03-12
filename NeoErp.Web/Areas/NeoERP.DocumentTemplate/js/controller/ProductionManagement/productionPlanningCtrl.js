DTModule.controller('LocationMappingCtrl', function ($scope, $http, $routeParams, $window, $filter) {

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

    $scope.locationType = '';

    $scope.$on("callChildEvent", function (event, locationType) {
        $scope.locationType = locationType;
        $scope.BrowseTreeListFortolocation();
    });


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
    };


    //treeview on select
    $scope.options = {
        loadOnDemand: false,
        //select: function (e) {
        //    debugger;
        //    var currentItem = e.sender.dataItem(e.node);
        //    if ($scope.childRowIndex == undefined) {
        //        $('#tolocationGrid').removeClass("show-displaygrid");
        //        $("#tolocationGrid").html("");
        //    }
        //    else {
        //        $('#tolocationGrid_' + $scope.childRowIndex).removeClass("show-displaygrid");
        //        $("#tolocationGrid_" + $scope.childRowIndex).html("");
        //    }
        //    BindtolocationGrid(currentItem.LocationId, currentItem.LocationId, "");
        //    // Unselect so the next click on same node triggers again
        //    e.sender.select($());
        //    $scope.$apply();
        //},
    };


    //$scope.onTreeClick = function (dataItem, e) {
    //    debugger;
    //    if ($scope.childRowIndex === undefined) {
    //        $('#tolocationGrid').removeClass("show-displaygrid").html("");
    //    } else {
    //        $('#tolocationGrid_' + $scope.childRowIndex).removeClass("show-displaygrid").html("");
    //    }

    //    BindtolocationGrid(dataItem.LocationId, dataItem.LocationId, "");

    //    // Keep node highlighted (optional)
    //    var treeview = $("#tolocationtree").data("kendoTreeView");
    //    treeview.select(e.node);
    //};


    // called when the tree is bound/rendered
    $scope.onTreeDataBound = function (e) {
        debugger;
        var tree = $("#tolocationtree").data("kendoTreeView");

        // remove any previous handler to avoid duplicates
        $("#tolocationtree").off('click', '.k-in');

        // attach click handler on the label element inside each node (.k-in)
        $("#tolocationtree").on('click', '.k-in', function (evt) {
            var $node = $(this).closest('.k-item');          // the <li> .k-item
            var dataItem = tree.dataItem($node);            // Kendo dataItem for the node

            // keep node visually selected
            tree.select($node);

            // clear previous grid(s) same as your code
            if ($scope.childRowIndex === undefined) {
                $('#tolocationGrid').removeClass("show-displaygrid").html("");
            } else {
                $('#tolocationGrid_' + $scope.childRowIndex)
                    .removeClass("show-displaygrid").html("");
            }

            // call your bind function inside $applyAsync to avoid $digest errors
            $scope.$applyAsync(function () {
                BindtolocationGrid(dataItem.LocationId, dataItem.LocationId, "");
            });
        });
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

                        var locationcode = $(this).find('td span').html();

                        if ($scope.locationType == 'reqFromLocation') {
                            $scope.$parent.selectedFormDetails.RequisitionFromLocationValue = locationcode;
                            $scope.$apply();
                        }
                        if ($scope.locationType == 'reqToLocation') {
                            $scope.$parent.selectedFormDetails.RequisitionToLocationValue = locationcode;
                            $scope.$apply();
                        }

                        if ($scope.locationType == 'indentFromLocation') {
                            $scope.$parent.selectedFormDetails.IndentFromLocationValue = locationcode;
                            $scope.$apply();
                        }
                        if ($scope.locationType == 'indentToLocation') {
                            $scope.$parent.selectedFormDetails.IndentToLocationValue = locationcode;
                            $scope.$apply();
                        }

                        $('#tolocationModal').modal('toggle');

                        //debugger;
                        ////  alert('here');
                        //var locationcode = $(this).find('td span').html();
                        //$scope.masterModels["TO_LOCATION_CODE"] = locationcode;
                        //if ($("#tolocation").hasClass('borderRed')) {
                        //    $scope.loactionerror = "";
                        //    $("#tolocation").removeClass('borderRed');
                        //}
                        //$('#tolocationModal').modal('toggle');
                        //if ($scope.$parent.invCtrlObject && $scope.$parent.invCtrlObject.SelectPlanDtl && $scope.$parent.invCtrlObject.SelectPlanDtl.PLAN_NO) {
                        //    $scope.GetSelectedItemQty(locationcode);
                        //} else {
                        //    $scope.$parent.getVoucherDetailForReferenceProduct();
                        //}
                        //$scope.$apply();
                    })
                } else {
                    //$("#tolocationGrid_" + $scope.childRowIndex + " tbody tr").css("cursor", "pointer");
                    //$("#tolocationGrid_" + $scope.childRowIndex + " tbody tr").on('dblclick', function () {
                    //    var locationcode = $(this).find('td span').html();
                    //    $scope.childModels[$scope.childRowIndex]["TO_LOCATION_CODE"] = locationcode;

                    //    if ($($(".ctolocation_" + $scope.childRowIndex)[0]).closest('div').parent().hasClass('borderRed')) {
                    //        $($(".ctolocation_" + $scope.childRowIndex)[0]).closest('div').parent().removeClass('borderRed')
                    //    }

                    //    $('#tolocationModal_' + $scope.childRowIndex).modal('toggle');
                    //    $scope.$apply();
                    //})
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

        // BrowseTreeListFortolocation

        //$("#tolocationtree")
        //    .find(".k-state-selected, .k-selected")
        //    .removeClass("k-state-selected k-selected");

        //if ($scope.havRefrence == 'Y' && $scope.freeze_master_ref_flag == "Y") {
        //    var referencenumber = $('#refrencetype').val();
        //    if ($scope.ModuleCode != '01' && referencenumber !== "") {
        //        return;
        //    }
        //}
        //if ($scope.freeze_master_ref_flag == "N") {
        //    $scope.childRowIndex = index;
        //    document.popupindex = index;
        //    if (index == undefined) {
        //        $('#tolocationModal').modal('show');
        //    }
        //    else {
        //        $('#tolocationModal_' + index).modal('show');
        //        if ($('#tolocationtree_' + $scope.childRowIndex).data("kendoTreeView") != undefined)
        //            $('#tolocationtree_' + $scope.childRowIndex).data("kendoTreeView").expand('.k-item');
        //    }
        //}
        $('#tolocationModal').modal('toggle');
        BindtolocationGrid("", "", "");
    }

});


DTModule.controller('productionPlanningCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter, $q) {

    var baseUrl = '/api/ProductionManagementApi/'; //
    $scope.FormName = "Production Planning";


    //FromLocationValue: ""
    //IndendFormCodeValue: ""

    //IndentFromLocationValue: ""
    //IndentToLocationValue: ""

    //RequisitionFormCodeValue: ""
    //RequisitionToLocationValue: ""

    //ToLocationValue: ""



    $scope.ProductionPreferencesDetails = {};

    $scope.selectedFormDetails = {
        RequisitionFormCodeValue: '',
        IndendFormCodeValue: '',

        RequisitionFromLocationValue: '',
        RequisitionToLocationValue: '',

        IndentFromLocationValue: '',
        IndentToLocationValue: ''
    };

    $scope.from_location = '';
    $scope.to_location = '';


    $scope.groupedData = [];
    $scope.formData = {
        PlanNo: '',
        PlanName: '',
        ProductName: '',
        ProductItemCode: '',
        planDateAd: '',
        planDateBs: '',
        PlanDate: '',
        ItemCode: '',
        PlanQuantity: '',
        PlanBaseOn: '',
        BaseFlag: '',
        ResourceCode: '',
        ResourceName: '',
        OrderQuantity: '',
        PlanDate: '',
        OrderNo: '',
        BatchQtyList: [],
        OrderDetailList: []
    };
    $scope.formData.planDateAd = $filter('date')(new Date(), 'dd-MMM-yyyy');
    $scope.formData.PlanDate = $filter('date')(new Date(), 'dd-MMM-yyyy');
    $scope.planOptions = [{ Value: "O", Name: "Order" }, { Value: "D", Name: "Direct" }];

    $scope.formsMappingList = [];
    $scope.formsIndentMappingList = [];

    $scope.dataObject = {
        treeDataList: [],
        treeObj: {},
        productItemList: [],
        productItemDetails: {},
        groupedItems: [],
        RelatedItemOrderList: [],
        ProductionPipeLineDataList: [],
        ResourceList: [],
        SelectedResource: {},
        IsUpdateMode: "",
        ItemSearchText: "",
        OrderList: [],
        SelectedOrder: {},
        BatchInfoList: [],
        FilterStatusList: [],
        SelectedFilterStatus: "",
        FilterListSearchText: "",
        RawMaterialMainList: [],
        SelectProductItem: {},
        selectAllRequired: false,
        selectAllVariance: false  // NEW: Add this property
    };

    $scope.InitFormModel = function () {
        $scope.groupedData = [];
        $scope.dataObject = {
            treeDataList: [],
            treeObj: {},
            productItemList: [],
            productItemDetails: {},
            groupedItems: [],
            RelatedItemOrderList: [],
            ProductionPipeLineDataList: [],
            ResourceList: [],
            SelectedResource: {},
            IsUpdateMode: "",
            ItemSearchText: "",
            OrderList: [],
            SelectedOrder: {},
            BatchInfoList: [],
            FilterStatusList: [],
            SelectedFilterStatus: "",
            FilterListSearchText: "",
            RawMaterialMainList: [],
            SelectProductItem: {},
            selectAllRequired: false
        };

        $scope.formData = {
            PlanNo: '',
            PlanName: '',
            ProductName: '',
            ProductItemCode: '',
            planDateAd: '',
            planDateBs: '',
            PlanDate: '',
            ItemCode: '',
            PlanQuantity: '',
            PlanBaseOn: '',
            BaseFlag: '',
            ResourceCode: '',
            ResourceName: '',
            OrderQuantity: '',
            PlanDate: '',
            OrderNo: '',
            BatchQtyList: [],
            OrderDetailList: []
        };
        $scope.formData.planDateAd = $filter('date')(new Date(), 'dd-MMM-yyyy');
        $scope.formData.PlanDate = $filter('date')(new Date(), 'dd-MMM-yyyy');
        $scope.planOptions = [{ Value: "O", Name: "Order" }, { Value: "D", Name: "Direct" }];

        $scope.formData.PlanBaseOn = "D";
        $scope.formData.BaseFlag = "I";

        $scope.dataObject.FilterStatusList = [
            { Name: "All", Value: "all" },
            { Name: "Incomplete", Value: "incomplete" },
            { Name: "Completed", Value: "completed" },
        ];

        $scope.dataObject.SelectedFilterStatus = 'all';
    };

    $scope.InitFormModel();

    $scope.onBaseFlagChange = function (value) {
        $scope.formData.BaseFlag = value;
        if (value == 'O') {
            $scope.formData.PlanBaseOn = 'O';
        } else {
            $scope.formData.PlanBaseOn = 'D';
        }
    };

    // Kendo TreeView Options
    $scope.treeOptions = {
        dataSource: $scope.treeData,
        dataTextField: "ItemEDesc",  // What text field to display
        template: "<span>{{dataItem.ItemEDesc}} ({{dataItem.MasterItemCode}})</span>", // Custom template
        select: function (e) { // On click/select
            var treeView = e.sender;
            var selectedNode = treeView.dataItem(e.node);
            $scope.selectedItem = selectedNode;  // Save clicked item
            $scope.$apply(); // Update Angular scope manually because Kendo event is outside digest
        }
    };

    $scope.dataSourceTreeListData = [];


    $scope.getProductTreeStructureList = function () {
        return $http({
            method: 'GET',
            url: baseUrl + 'GetProductTreeStructureListAsync'
        }).then(function (response) {

            $scope.dataObject.treeDataList = response.data;
            $scope.onTreeSelect(""); // call for particular display initially
            //setTimeout(function () {
            //    expandAllNodes();
            //}, 100);

        }).catch(function (error) {
            console.error('Error while fetching product tree structure:', error);
            throw error; // rethrow error to caller
        });
    };


    // Tree item search // Search Text
    $scope.searchProductItems = function () {
        let searchText = $scope.dataObject.ItemSearchText;
        $http.get(baseUrl + 'GetParticularProductListUsingSearchText'
            , { params: { search_text: searchText } }
        ).then(function (response) {
            if (response && response.data) {
                $scope.dataObject.productItemList = response.data;
                if ($scope.dataObject.productItemList.length) {
                    $scope.GetProductItemDetails($scope.dataObject.productItemList[0], $scope.dataObject.productItemList[0].item_code, false); // Getting Item Code details
                }
            }
        }, function (error) {
            // Error callback
            console.error('Error occurred:', error);
        });
    };

    // Tree options
    $scope.treeOptions = {
        dataTextField: "text",
        loadOnDemand: false,
        select: function (e) {
            var dataItem = this.dataItem(e.node);
        }
    };

    // Tree selection handler
    $scope.onTreeSelect = function (dataItem) {
        $http({
            method: 'GET',
            url: baseUrl + 'GetParticularProductItemsAsync?itemCode=' + (dataItem && dataItem.id ? dataItem.id : "")
        }).then(function (response) {

            if (response && response.data) {
                $scope.dataObject.productItemList = response.data;
                if ($scope.dataObject.productItemList.length) {
                    $scope.GetProductItemDetails($scope.dataObject.productItemList[0], $scope.dataObject.productItemList[0].item_code, false); // Getting Item Code details
                }
            }

        }).catch(function (error) {
            console.error('Error while fetching product tree structure:', error);
            throw error; // rethrow error to caller
        });

        // Handle selection here
    };


    $scope.submitPlan = function () {
        if ($scope.dataObject.IsUpdateMode == "Edit") {
            $scope.updatePlan();
        } else {
            $scope.addPlan();
        }
    };


    $scope.addPlan = function () {

        if ($scope.formData.BaseFlag == "I" && ($scope.formData.ItemCode == null || $scope.formData.ItemCode == "")) {
            displayPopupNotification("Product is required", "error");
            return;
        }
        if ($scope.formData.BaseFlag == "O" && ($scope.formData.OrderNo == null || $scope.formData.OrderNo == "")) {
            displayPopupNotification("Order no is required", "error");
            return;
        }

        $scope.formData.PlanDate = $scope.formData.planDateAd;
        $scope.formData.BatchQtyList = $scope.dataObject.BatchInfoList;
        $scope.formData.OrderDetailList = $scope.dataObject.RelatedItemOrderList.filter(item => item.selected == true);

        $http.post(baseUrl + 'InsertOrderPlanProcess', $scope.formData)
            .then(function (response) {
                alert('Plan Added successfully.');
                displayPopupNotification("Plan Added successfully.", "success");
                $('#productionPlanningNewAddModel').modal('hide');
                $scope.GridReCall();
            }, function (error) {

                console.log(JSON.stringify(error));

                if (error && error.data && error.data.Errors && error.status == 400) {
                    displayPopupNotification(error.data.Errors, "error");
                }

                if (error && error.data && error.data.Errors && error.data.Errors.length) {
                    error.data.Errors.forEach(item => {
                        displayPopupNotification(item.ErrorMessage, "warning");
                    })
                }

                if (error && error.data.ExceptionMessage) {
                    displayPopupNotification(error.data.ExceptionMessage, "error");
                }

            });
    };



    $scope.updatePlan = function () {
        $scope.formData.PlanDate = $scope.formData.planDateAd;
        $scope.formData.BatchQtyList = $scope.dataObject.BatchInfoList;
        $scope.formData.OrderDetailList = $scope.dataObject.RelatedItemOrderList.filter(item => item.selected == true);
        console.log(JSON.stringify($scope.formData));
        // return;
        $http.put(baseUrl + 'UpateOrderPlanProcess', $scope.formData)
            .then(function (response) {
                alert('Plan Updated successfully.');
                displayPopupNotification("Plan Update successfully.", "success");
                $('#productionPlanningNewAddModel').modal('hide');
                $scope.GridReCall();
            }, function (error) {
                console.error('Error occurred:', error);
            });
    };


    $scope.GetProductItemDetails = function (item, itemCode, isClick = false) {


        if (isClick) {
            $scope.dataObject.SelectProductItem = item;
        }


        $http({
            method: 'GET',
            url: baseUrl + 'GetParticularProductItemsDetailsAsync?itemCode=' + itemCode
        }).then(function (response) {

            if (response && response.data) {
                $scope.dataObject.productItemDetails = response.data;
            }

        }).catch(function (error) {
            console.error('Error while fetching product tree structure:', error);
            throw error; // rethrow error to caller
        });
    };

    // Tree View Open // Order View Open
    $scope.OpenProductTreeView = function () {



        $http({
            method: 'GET',
            url: baseUrl + 'GetProductTreeStructureListAsync'
        }).then(function (response) {

            $scope.dataObject.treeDataList = response.data;
            $scope.onTreeSelect(""); // call for particular display initially
            //setTimeout(function () {
            //    expandAllNodes();
            //}, 100);


            // debugger;
            if ($scope.formData.BaseFlag == 'I') {
                $scope.treeData = transformToKendoTree($scope.dataObject.treeDataList);
                setTimeout(function () {
                    $('#TreeViewPopupModel').find('.modal-dialog').addClass('large1-popup');
                    $('#TreeViewPopupModel').modal('show'); // Show Bootstrap modal
                }, 100);
            } else {

                const searchText = $scope.dataObject.SearchText || "";
                $http.get(baseUrl + 'GetOrderListData?searchText=' + searchText
                ).then(function (response) {
                    if (response && response.data) {
                       // debugger;
                        $scope.dataObject.OrderList = response.data;

                        // Order Model open
                        setTimeout(function () {
                            $('#OrderSearchViewPopupModel').find('.modal-dialog').addClass('large1-popup');
                            $('#OrderSearchViewPopupModel').modal('show'); // Show Bootstrap modal
                        }, 100);
                    }
                }, function (error) {
                });
            }





        }).catch(function (error) {
            console.error('Error while fetching product tree structure:', error);
            throw error; // rethrow error to caller
        });


    };

    $scope.searchOrder = function (searchValue) {

        $scope.dataObject.SearchText = searchValue;
        $scope.OpenProductTreeView();
    };


    var dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                method: "GET",
                url: "/api/ProductionManagementApi/GetProductionPlanningList", // Make sure this matches your controller route
                dataType: "json"
            },
            parameterMap: function (options, type) {
                if (type === "read") {
                    return {
                        StartDate: $('#FromDateVoucher').val(),
                        EndDate: $('#ToDateVoucher').val(),
                        Status: $scope.dataObject.SelectedFilterStatus,
                        FilterListSearchText: $scope.dataObject.FilterListSearchText,
                    };
                }
            }
        },
        pageSize: 10
    });


    // double click // select item from tree view
    $scope.SelectAndCloseProductPopup = function () {
        $scope.formData.ProductName = $scope.dataObject.productItemDetails.NAME;
        $scope.formData.ProductItemCode = $scope.dataObject.productItemDetails.CODE;
        $scope.formData.ItemCode = $scope.dataObject.productItemDetails.CODE;
        $scope.GetOrderList();
        $scope.GetProductionPileList();
        $('#TreeViewPopupModel').modal('hide');
    };

    // Get Order list of related item(selected item)
    $scope.GetOrderList = function () {


        if ($scope.dataObject.IsUpdateMode == "Edit") {

            $http.get(baseUrl + 'GetOrderListByPlanDetailForEdit'
                , { params: { planNo: $scope.formData.PlanNo } }
            ).then(function (response) {
                if (response && response.data) {
                    angular.forEach(response.data, function (item) {
                        item.selected = true; // or true, depending on your default
                    });
                    $scope.dataObject.RelatedItemOrderList = response.data;

                    if ($scope.dataObject.RelatedItemOrderList.length == 1) {
                        $scope.formData.ItemCode = $scope.dataObject.RelatedItemOrderList[0].ITEM_CODE;
                    }

                    // Load material // 
                    $scope.GetProductionPileList();
                    $scope.getRawMaterialList();
                    $scope.getBatchTransectionInfo();
                }
            }, function (error) {
                // Error callback
                console.error('Error occurred:', error);
            });
        } else {

            $http.get(baseUrl + 'GetOrderListOfRelatedItem'
                , { params: { itemCode: $scope.formData.ItemCode, orderNo: $scope.formData.OrderNo } }
            ).then(function (response) {
                if (response && response.data) {
                    $scope.dataObject.RelatedItemOrderList = response.data;
                }
            }, function (error) {
                // Error callback
                console.error('Error occurred:', error);
            });
        }


    }

    // Get Production Pile line List
    $scope.GetProductionPileList = function () {

        $http.get(baseUrl + 'GetProductionPipeLineDataList'
            , {
                params:
                {
                    itemCode: $scope.formData.ItemCode,
                    planNo: $scope.formData.PlanNo,
                    orderNo: $scope.formData.OrderNo
                }
            }
        ).then(function (response) {
            if (response && response.data) {
                $scope.dataObject.ProductionPipeLineDataList = response.data;
            }
        }, function (error) {
            // Error callback
            console.error('Error occurred:', error);
        });


    }

    $("#productionPlanningListGrid").kendoGrid({
        dataSource: dataSource,
        scrollable: true,
        filterable: true,
        sortable: true,
        pageable: true,
        reorderable: true,
        resizable: true,
        columnMenu: true,
        dataBound: function (e) {

            // Use jQuery to bind double-click event to grid rows
            $("#productionPlanningListGrid").find("tbody tr").on("dblclick", function () {
                var grid = $("#productionPlanningListGrid").data("kendoGrid");
                var dataItem = grid.dataItem(this);
                $scope.editProductionPlan(dataItem);
            });

        },
        columns: [
            { field: "PLAN_NO", title: "Plan No", width: 60 },
            { field: "PLAN_DATE", title: "Plan Date", width: 60, template: "#= kendo.toString(kendo.parseDate(PLAN_DATE), 'yyyy-MM-dd') #" },
            { field: "MITI", title: "Plan Miti", width: 60 },
            { field: "PLAN_NAME", title: "Plan Name", width: 120 },
            { field: "ITEM_EDESC", title: "Product Name / Order No", width: 85 },
            { field: "INDEX_MU_CODE", title: "Unit", width: 60 },
            {
                field: "PLAN_BASE_ON",
                title: "Plan Type",
                width: 100,
                template: function (dataItem) {
                    return dataItem.PLAN_BASE_ON === 'D' ? 'Direct' :
                        dataItem.PLAN_BASE_ON === 'O' ? 'Order' : dataItem.PLAN_BASE_ON;
                }
            },
            { field: "ORDER_QUANTITY", title: "Order Quantity", width: 60 },
            { field: "PLAN_QUANTITY", title: "Planned Quantity", width: 60 },
            { field: "PROD_ISS_QTY", title: "Prod. Iss. Quantity", width: 60 },
            { field: "RESOURCE_EDESC", title: "Resource Name", width: 120 }
        ]
    });


    $scope.editProductionPlan = function (item) {

        $scope.dataObject.IsUpdateMode = "Edit";

        $http.get(baseUrl + 'GetPlanDetailsFoEdit'
            , { params: { plan_no: item.PLAN_NO } }
        ).then(function (response) {
            if (response && response.data) {
                $scope.dataObject.ResourceList = response.data;

                let data = response.data;


                $scope.formData = {
                    PlanNo: data.PLAN_NO || '',
                    PlanName: data.PLAN_NAME || '',
                    ProductName: data.ITEM_EDESC || '',
                    ProductItemCode: data.ITEM_CODE || '',
                    planDateAd: data.PLAN_DATE || '',
                    planDateBs: data.MITI || '',
                    PlanDate: data.PLAN_DATE || '',
                    ItemCode: data.ITEM_CODE || '',
                    PlanQuantity: data.PLAN_QUANTITY || '',
                    PlanBaseOn: data.PLAN_BASE_ON || '',
                    BaseFlag: data.BASE_FLAG || '', // Assuming BaseFlag is same as PLAN_BASE_ON
                    ResourceCode: data.RESOURCE_CODE || '',
                    ResourceName: data.RESOURCE_EDESC, // You'll need to map this separately if available
                    OrderQuantity: data.ORDER_QUANTITY || '',
                    OrderNo: data.ORDER_NO || ''
                };

                $scope.GetOrderList();
                $scope.GetProductionPileList();



                //$scope.formData.PlanNo = data.
                setTimeout(function () {
                    $('#productionPlanningNewAddModel').find('.modal-dialog').addClass('large-popup');
                    $('#productionPlanningNewAddModel').modal('show'); // Show Bootstrap modal
                }, 100);
            }
        }, function (error) {
            // Error callback
            console.error('Error occurred:', error);
        });
    };



    $scope.GridReCall = function () {

        $("#productionPlanningListGrid").data("kendoGrid").dataSource.read();
        $("#productionPlanningListGrid").data("kendoGrid").refresh();
    };

    $scope.ShowAddNewPlanningPopup = function () {

        $scope.InitFormModel();
        $scope.someDateFn();

        $scope.popupMessage = "Add new Planning";
        $scope.getProductTreeStructureList();
        var item_code = "";
        $http({
            method: 'GET',
            url: baseUrl + 'GetNewPlanCode'
        }).then(function (response) {

            if (response && response.data) {
                $scope.formData.PlanNo = response.data;
            }
            setTimeout(function () {
                $('#productionPlanningNewAddModel').find('.modal-dialog').addClass('large-popup');
                $('#productionPlanningNewAddModel').modal('show'); // Show Bootstrap modal
            }, 100);

        }).catch(function (error) {
            console.error('Error while fetching product tree structure:', error);
            throw error; // rethrow error to caller
        });

    };


    function transformToKendoTree(data) {
        return data.map(function (item) {
            return {
                id: item.MasterItemCode,
                text: item.ItemEDesc,
                items: item.Items && item.Items.length > 0 ? transformToKendoTree(item.Items) : []
            };
        });
    }

    // When your tree data is loaded or in the $scope.$watch for treeData
    function expandAllNodes() {
        if ($scope.tree) { // check if tree widget is available
            var treeView = $scope.tree;
            // Expand all nodes
            treeView.expand(".k-item");
        }
    }


    $scope.getRawMaterialList = function () {
        let PostToObj = {
            ItemWithQtyList: [],
            RequestedQty: 0,
            PlanNo: ""
        };

        const itemCode = $scope.formData.ItemCode;
        //if ($scope.dataObject.IsUpdateMode == "Edit") {
        //    if ($scope.dataObject.RelatedItemOrderList && $scope.dataObject.RelatedItemOrderList.length == 1) {
        //        $scope.formData.ItemCode = $scope.dataObject.RelatedItemOrderList[0].ITEM_CODE;
        //    }
        //} else {
        //}

        const FilterItemCodeAndQty = $scope.dataObject.RelatedItemOrderList
            .filter(function (item) {
                return item.selected === true;
            })
            .map(function (item) {
                return {
                    Qty: item.QUANTITY,
                    ItemCode: item.ITEM_CODE
                };
            });

        if (FilterItemCodeAndQty && FilterItemCodeAndQty.length) {
            PostToObj.ItemWithQtyList = FilterItemCodeAndQty;
        } else {
            PostToObj.ItemWithQtyList = [{
                Qty: $scope.formData.PlanQuantity,
                ItemCode: itemCode
            }]
        }
        PostToObj.RequestedQty = $scope.formData.PlanQuantity;
        PostToObj.PlanNo = $scope.formData.PlanNo;

        $http.post(baseUrl + 'PrepareRowMaterialPackingBasedOnCalcAsync', PostToObj).then(function (response) {

            if (response && response.data) {
                var tempListData = response.data;

                $scope.dataObject.RawMaterialMainList = tempListData;


                const grouped = [];
                const typeGroups = {};
                $scope.dataObject.RawMaterialMainList.forEach(item => {
                    const typeKey = item.PROCESS_TYPE_EDESC;
                    const processKey = item.PROCESS_EDESC;

                    if (!typeGroups[typeKey]) typeGroups[typeKey] = {};
                    if (!typeGroups[typeKey][processKey]) typeGroups[typeKey][processKey] = [];
                    typeGroups[typeKey][processKey].push(item);
                });

                for (const type in typeGroups) {
                    const processes = typeGroups[type];
                    const totalTypeRows = Object.values(processes).reduce((sum, arr) => sum + arr.length, 0);
                    let typeRowAdded = false;

                    for (const process in processes) {
                        const rows = processes[process];
                        let processRowAdded = false;

                        rows.forEach((row, index) => {
                            const newRow = angular.copy(row);
                            if (!typeRowAdded) {
                                newRow._typeRowspan = totalTypeRows;
                                typeRowAdded = true;
                            }
                            if (!processRowAdded) {
                                newRow._processRowspan = rows.length;
                                processRowAdded = true;
                            }
                            grouped.push(newRow);
                        });
                    }
                }

                // initialize selection state per row
                grouped.forEach(function (row) {
                    if (typeof row.isRequired === 'undefined') {
                        row.isRequired = false;
                    }


                });







                $scope.groupedData = grouped;






                //// Nested Grouping: ProcessTypeEDesc -> ProcessEDesc
                //$scope.dataObject.groupedItems = tempListData.reduce((acc, item) => {
                //    const processType = item.PROCESS_TYPE_EDESC;
                //    const routine = item.PROCESS_EDESC;
                //    acc[processType] = acc[processType] || {};
                //    acc[processType][routine] = acc[processType][routine] || [];
                //    acc[processType][routine].push(item);
                //    return acc;
                //}, {});


            }

        }).catch(function (error) {
            console.error('Error while fetching product tree structure:', error);
            throw error; // rethrow error to caller
        });
        $scope.getBatchTransectionInfo();
    };

    // Toggle header checkbox to select/deselect all rows
    $scope.toggleSelectAllRequired = function () {
        var selectAll = $scope.dataObject.selectAllRequired === true;
        ($scope.groupedData || []).forEach(function (row) {
            row.isRequired = selectAll;
        });
    };

    // When a single row checkbox changes, update header checkbox if needed
    $scope.onRequiredRowChange = function (row) {
        if (!$scope.groupedData || !$scope.groupedData.length) return;
        var allSelected = $scope.groupedData.every(function (r) { return r.isRequired === true; });
        var noneSelected = $scope.groupedData.every(function (r) { return r.isRequired !== true; });
        // Keep header checkbox in sync (no indeterminate state supported natively here)
        $scope.dataObject.selectAllRequired = allSelected && !noneSelected;
    };

    // Show selected rows as JSON in modal
    $scope.showSelectionJson = function () {
        var selected = ($scope.groupedData || []).filter(function (r) { return r.isRequired === true; });
        var payload = selected.map(function (r) {
            return {
                PROCESS_TYPE_EDESC: r.PROCESS_TYPE_EDESC,
                PROCESS_EDESC: r.PROCESS_EDESC,
                PROCESS_CODE: (r.PROCESS_CODE || ''),
                ITEM_EDESC: r.ITEM_EDESC,
                ITEM_CODE: r.ITEM_CODE,
                INDEX_MU_CODE: r.INDEX_MU_CODE,
                STOCK: r.STOCK,
                PLAN_QUANTITY: r.PLAN_QUANTITY,
                REQUIRED_QUANTITY: r.REQUIRED_QUANTITY,
                VARIANCE: (r.STOCK - (r.PLAN_QUANTITY + r.REQUIRED_QUANTITY))
            };
        });
        $scope.jsonPreview = JSON.stringify(payload, null, 2);
        setTimeout(function () { $('#jsonOutputModal').modal('show'); }, 50);
    };

    $scope.copyJsonToClipboard = function () {
        try {
            var temp = document.createElement('textarea');
            temp.value = $scope.jsonPreview || '';
            document.body.appendChild(temp);
            temp.select();
            document.execCommand('copy');
            document.body.removeChild(temp);
            displayPopupNotification('JSON copied to clipboard', 'success');
        } catch (e) {
            displayPopupNotification('Unable to copy', 'error');
        }
    };

    // Requisition
    // Generate requisition by sending selected rows to server
    $scope.generateRequisition = function () {
        debugger;
        var selected = ($scope.groupedData || []).filter(function (r) { return r.isRequired === true; });
        if (!selected.length) {
            displayPopupNotification('Please select at least one row', 'warning');
            return;
        }

        // Validate FROM_LOCATION_CODE and TO_LOCATION_CODE
        if (!$scope.ProductionPreferencesDetails.REQ_FROM_LOCATION_CODE ||
            $scope.ProductionPreferencesDetails.REQ_FROM_LOCATION_CODE.trim() === '') {
            displayPopupNotification('Please set the From Location before generating requisition.', 'warning');
            return;
        }

        if (!$scope.ProductionPreferencesDetails.REQ_TO_LOCATION_CODE ||
            $scope.ProductionPreferencesDetails.REQ_TO_LOCATION_CODE.trim() === '') {
            displayPopupNotification('Please set the To Location before generating requisition.', 'warning');
            return;
        }

        var payload = selected.map(function (r) {
            return {
                processTypeName: r.PROCESS_TYPE_EDESC,
                processName: r.PROCESS_EDESC,
                processCode: (r.PROCESS_CODE || ''),
                itemCode: r.ITEM_CODE,
                itemName: r.ITEM_EDESC,
                unit: r.INDEX_MU_CODE,
                stock: r.STOCK,
                planQuantity: r.PLAN_QUANTITY,
                requiredQuantity: r.REQUIRED_QUANTITY,
                variance: (r.STOCK - (r.PLAN_QUANTITY + r.REQUIRED_QUANTITY))
            };
        });

        var request = {
            planNo: $scope.formData.PlanNo,
            baseFlag: $scope.formData.BaseFlag,
            itemCode: $scope.formData.ItemCode,
            orderNo: $scope.formData.OrderNo,
            items: payload
        };


        var Child_COLUMN_VALUE_LIST = [];


        var selectedItems = selected.map(function (r) {
            return {
                "COMPLETED_QUANTITY": 0,
                "CALC_TOTAL_PRICE": 0,
                "CALC_UNIT_PRICE": 0,
                "CALC_QUANTITY": r.REQUIRED_QUANTITY,
                "REMARKS": "",
                "MU_CODE": r.INDEX_MU_CODE,
                "QUANTITY": r.REQUIRED_QUANTITY,
                "ITEM_CODE": r.ITEM_CODE,
            };
        });

        var Child_COLUMN_VALUE_LIST_OBJ = selectedItems;

        var Master_COLUMN_VALUE_OBJ = {
            "FROM_LOCATION_CODE": $scope.ProductionPreferencesDetails.REQ_FROM_LOCATION_CODE,  //   "02.01",
            "TO_LOCATION_CODE": $scope.ProductionPreferencesDetails.REQ_TO_LOCATION_CODE,      //  
            "MANUAL_NO": "",
            "REQUISITION_DATE": $filter('date')(new Date(), 'dd-MMM-yyyy'),
            "REQUISITION_NO": ""
        };
        var grand_total = selectedItems.reduce(function (sum, item) {
            return sum + (item.QUANTITY || 0);
        }, 0);

        var sendToObj = {
            "Save_Flag": 3,
            "Table_Name": $scope.ProductionPreferencesDetails.TABLE_NAME,
            "Form_Code": $scope.ProductionPreferencesDetails.REQUISITION_FORM_CODE,
            "Master_COLUMN_VALUE": JSON.stringify(Master_COLUMN_VALUE_OBJ),
            //"Child_COLUMNS": "COMPLETED_QUANTITY,CALC_TOTAL_PRICE,CALC_UNIT_PRICE,CALC_QUANTITY,REMARKS,MU_CODE,QUANTITY,ITEM_CODE,",
            "Child_COLUMN_VALUE": JSON.stringify(Child_COLUMN_VALUE_LIST_OBJ),
            "Grand_Total": grand_total,
            "Custom_COLUMN_VALUE": "{}",
            "FROM_REF": false,
            "REF_MODEL": [],
            "Order_No": "undefined",
            "TempCode": "",
            "BUDGET_TRANS_VALUE": "[]",
            "CHARGES": "[]",
            "SHIPPING_DETAILS_VALUE": "{}",
            "SERIAL_TRACKING_VALUE": "[]",
            "BATCH_TRACKING_VALUE": "[]",
            "MODULE_CODE": $scope.ProductionPreferencesDetails.MODULE_CODE_REQUISITION,
            "RESOURCE_LIST": [],
            "MANPOWER_RESOURCE_LIST": [],
            "CONSUMEABLE_POWER_RESOURCE_LIST": []


        };
        debugger;
        console.log(sendToObj);
        JSON.stringify(sendToObj);
        // return;

        var requisitionRequestPostURL = window.location.protocol + "//" + window.location.host + "/api/InventoryApi/SaveInventoryFormData";

        $http({
            method: "POST",
            url: requisitionRequestPostURL,
            data: sendToObj,
            headers: { "Content-Type": "application/json" }
        }).success(function (data, status, headers, config) {
            $scope.groupedData.forEach(function (row) {
                row.isVarianceSelected = false;
            });
            $scope.dataObject.selectAllRequired = false;

            $scope.showSimplePopup("Requisition Saved Successfully! </br> Voucher No: " + data.VoucherNo);
            displayPopupNotification(
                'Requisition generated successfully! Requisition No: ' + data.VoucherNo,
                'success'
            );

        }).error(function (data, status, headers, config) {
            console.error("Error:", data);
        });

    };


    $scope.getBatchTransectionInfo = function () {

        const orderData = $scope.dataObject.RelatedItemOrderList[0];

        var selectedOrderNos = $scope.dataObject.RelatedItemOrderList
            .filter(function (order) { return order.selected === true; })
            .map(function (order) { return order.ORDER_NO; })
            .join(',');

        if (selectedOrderNos) {
            //Get Batch Info List
            $http.get(baseUrl + 'GetBatchTransectionInfo'
                , { params: { itemCode: orderData.ITEM_CODE, orderNo: selectedOrderNos, planNo: $scope.formData.PlanNo } }
            ).then(function (response) {
                if (response && response.data) {
                    $scope.dataObject.BatchInfoList = response.data
                }
            }, function (error) {
                // Error callback
                console.error('Error occurred:', error);
            });
        }

    };


    // Helper to count total rows in a process group
    $scope.getTotalItemCount = function (processGroup) {
        let total = 0;
        angular.forEach(processGroup, function (items) {
            total += items.length;
        });
        return total;
    };

    // Resource Model
    $scope.OpenResourceSearchPopupModel = function () {
        $http.get(baseUrl + 'GetResourceDataList'
            // , { params: { id: someId } }
        ).then(function (response) {
            if (response && response.data) {
                $scope.dataObject.ResourceList = response.data;

                setTimeout(function () {
                    $('#ResourceViewPopupModel').find('.modal-dialog').addClass('large1-popup');
                    $('#ResourceViewPopupModel').modal('show'); // Show Bootstrap modal
                }, 100);
            }
        }, function (error) {
            // Error callback
            console.error('Error occurred:', error);
        });
    };

    $scope.confirmResourceSelection = function (item, action) {

        if (Object.keys(item).length > 0) {
            $scope.dataObject.SelectedResource = item;
        } else {
            setTimeout(function () {
                $('#ResourceViewPopupModel').modal('hide'); // Show Bootstrap modal
            }, 80);
        }

        if ($scope.dataObject.SelectedResource && $scope.dataObject.SelectedResource.RESOURCE_CODE) {
            $scope.formData.ResourceCode = $scope.dataObject.SelectedResource.RESOURCE_CODE;
            $scope.formData.ResourceName = $scope.dataObject.SelectedResource.RESOURCE_EDESC;
        }

        if (action == "cancle") {
            $scope.formData.ResourceCode = "";
            $scope.dataObject.SelectedResource = {};
            $scope.formData.ResourceCode = "";
            $scope.formData.ResourceName = "";
        }

    };



    $scope.confirmOrderSelection = function (item, action) {

        if (Object.keys(item).length > 0) {
            $scope.dataObject.SelectedOrder = item;
        } else {

            setTimeout(function () {
                $('#OrderSearchViewPopupModel').modal('hide'); // Show Bootstrap modal
            }, 80);
        }

        if ($scope.dataObject.SelectedOrder && $scope.dataObject.SelectedOrder.ORDER_NO) {
            $scope.formData.OrderNo = $scope.dataObject.SelectedOrder.ORDER_NO;

            $scope.GetOrderList();
            $scope.GetProductionPileList();
        }

        if (action == "cancle") {
            $scope.dataObject.SelectedOrder = {};
            $scope.formData.OrderNo = "";
        }
    };









    /*Date picker calendar related*/
    $scope.ConvertNepToEng = function ($event) {
        //$event.stopPropagation();
        var date1 = BS2AD($("#nepaliDate51").val());
        $("#englishdatedocument1").val($filter('date')(date1, "dd-MMM-yyyy"));
        $('#nepaliDate51').trigger('change')
    };

    $scope.ConvertEngToNepang = function (data) {
        $("#nepaliDate51").val(AD2BS(data));
    };

    $scope.someDateFn = function () {
        var engdate = $filter('date')(new Date(new Date().setDate(new Date().getDate() - 1)), 'dd-MMM-yyyy');
        //var engdate1 = $filter('date')(new Date(new Date().setDate(new Date().getDate() - 2)), 'dd-MMM-yyyy');
        var a = ConvertEngDateToNep(engdate);
        //var a1 = ConvertEngDateToNep(engdate1);
        $scope.Dispatch_From = engdate;
        $scope.NepaliDate = a;
        $scope.Dispatch_To = a;
        $scope.formData.planDateBs = ConvertEngDateToNep($filter('date')(new Date(new Date().setDate(new Date().getDate())), 'dd-MMM-yyyy'));
        //  $scope.PlanningDate = a;
    };
    $scope.someDateFn();

    $scope.monthSelectorOptionsSingle = {
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };

    $scope.monthSelectorOptionsSingle1 = {

        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            debugger;
            $scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };

    $scope.onOrderChecked = function (orderDetails) {
        $scope.formData.PlanQuantity = $scope.dataObject.RelatedItemOrderList
            .filter(item => item.selected)
            .reduce((sum, item) => sum + (item.QUANTITY || 0), 0);

        $scope.formData.OrderQuantity = $scope.formData.PlanQuantity;
        $scope.formData.ItemCode = orderDetails.ITEM_CODE;
    };

    $scope.changeFilterStatus = function () {
        $scope.GridReCall();
    };


    $scope.PlanFormClose = function (val) {
        if (val === 'Add') {
            $http.delete(baseUrl + 'DeleteVarianceInfo', {
                params: { planNo: $scope.formData.PlanNo }
            }).then(function (response) {
                if (response && response.data) {
                    console.log('Delete successful:', response.data);
                }
            }, function (error) {
                console.error('Error occurred:', error);
            });
        }
    };


    $scope.checkEnterItemSearch = function (event) {
        if (event.which === 13) { // 13 is the Enter key
            $scope.searchProductItems(); // or any function you want to call
        }
    };

    $scope.checkEnterOrderSearch = function (event) {
        if (event.which === 13) { // 13 is the Enter key
            $scope.searchOrder($scope.dataObject.SearchText);
        }
    };





    $scope.OpenProductionPreferencesPopup = function () {
        $http.get(baseUrl + 'GetProductionPreferences')
            .then(function (response) {
                if (response && response.data) {
                    $scope.ProductionPreferencesDetails = response.data;

                    $scope.selectedFormDetails.RequisitionFormCodeValue = $scope.ProductionPreferencesDetails.REQUISITION_FORM_CODE;
                    $scope.selectedFormDetails.IndendFormCodeValue = $scope.ProductionPreferencesDetails.INDENT_FORM_CODE;


                    $scope.selectedFormDetails.RequisitionFromLocationValue = $scope.ProductionPreferencesDetails.REQ_FROM_LOCATION_CODE;
                    $scope.selectedFormDetails.RequisitionToLocationValue = $scope.ProductionPreferencesDetails.REQ_TO_LOCATION_CODE;

                    $scope.selectedFormDetails.IndentFromLocationValue = $scope.ProductionPreferencesDetails.IND_FROM_LOCATION_CODE;
                    $scope.selectedFormDetails.IndentToLocationValue = $scope.ProductionPreferencesDetails.IND_TO_LOCATION_CODE;

                }
            }, function (error) {
                // Error callback
                console.error('Error occurred while fetching production preferences:', error);
            });
    }; $scope.OpenProductionPreferencesPopup();




    //// Inside your AngularJS controller
    //$scope.OpenMappingFormPopupModel = function () {
    //    // optional: reset object values before opening popup
    //    $scope.dataObject.RequisitionText = "";
    //    $scope.dataObject.LevelsText = "";
    //    $scope.dataObject.RequisitionList = [];
    //    $scope.dataObject.SelectedRequisition = null;

    //    $http.get(baseUrl + 'GetFormMappingListForPreferencesSetup')
    //        .then(function (response) {
    //            if (response && response.data) {
    //                $scope.formsMappingList = response.data;
    //            }
    //        }, function (error) {
    //            // Error callback
    //            //console.error('Error occurred while fetching production preferences:', error);
    //        });

    //    $http.get(baseUrl + 'GetIndentFormMappingListForPreferencesSetup')
    //        .then(function (response) {
    //            if (response && response.data) {
    //                $scope.formsIndentMappingList = response.data;
    //            }
    //        }, function (error) {
    //            // Error callback
    //            //console.error('Error occurred while fetching production preferences:', error);
    //        });


    //    setTimeout(function () {
    //        // show modal
    //        $('#MappingFormViewPopupModel').modal({
    //            backdrop: 'static',   // prevent closing when clicking outside
    //            keyboard: false       // disable ESC key close
    //        });
    //        $('#MappingFormViewPopupModel').modal('show');
    //    }, 400);

    //};


    // Inside your AngularJS controller
    $scope.OpenMappingFormPopupModel = function () {
        // optional: reset object values before opening popup
        $scope.dataObject.RequisitionText = "";
        $scope.dataObject.LevelsText = "";
        $scope.dataObject.RequisitionList = [];
        $scope.dataObject.SelectedRequisition = null;

        // Create array of promises for both HTTP requests
        var promises = [
            $http.get(baseUrl + 'GetFormMappingListForPreferencesSetup'),
            $http.get(baseUrl + 'GetIndentFormMappingListForPreferencesSetup')
        ];

        // Wait for both requests to complete
        $q.all(promises)
            .then(function (responses) {
                // Both requests successful
                var formMappingResponse = responses[0];
                var indentMappingResponse = responses[1];

                // Process first response
                if (formMappingResponse && formMappingResponse.data) {
                    $scope.formsMappingList = formMappingResponse.data;
                }

                // Process second response
                if (indentMappingResponse && indentMappingResponse.data) {
                    $scope.formsIndentMappingList = indentMappingResponse.data;
                }

                // Open popup after both requests are successful
                openPopup();
            })
            .catch(function (error) {
                // Handle errors from either request
                console.error('Error occurred while fetching data:', error);
                // Optionally still open popup even if requests fail
                // openPopup();
            });

        function openPopup() {
            setTimeout(function () {
                $('#MappingFormViewPopupModel').modal({
                    backdrop: 'static',   // prevent closing when clicking outside
                    keyboard: false       // disable ESC key close
                });

                $('#MappingFormViewPopupModel').find('.modal-dialog').addClass('large1-popup');
                $('#MappingFormViewPopupModel').modal('show');

            }, 0);
        }
    };




    // Function to close modal (optional helper)
    $scope.SaveUpdateCloseMappingFormPopupModel = function (data, value) {
        if (value == 'cancle') {
            $('#MappingFormViewPopupModel').modal('hide');
            return;
        }
        console.log('ready to call api');

        $http.post(baseUrl + 'FormCodeMappingSave', $scope.selectedFormDetails, {
            headers: { 'Content-Type': 'application/json' }
        })
            .then(function (response) {
                // success
                $scope.OpenProductionPreferencesPopup();
                displayPopupNotification("Saved successfully.", "success");
                $('#MappingFormViewPopupModel').modal('hide');
            })
            .catch(function (error) {
                // error handling
                console.error("Error saving form code mapping:", error);
                displayPopupNotification("Error while saving. Please try again.", "error");
            });



    };

    // NEW: Toggle header checkbox to select/deselect all variance rows
    $scope.toggleSelectAllVariance = function () {
        var selectAll = $scope.dataObject.selectAllVariance === true;
        ($scope.groupedData || []).forEach(function (row) {
            row.isVarianceSelected = selectAll;
        });
    };

    // NEW: When a single variance row checkbox changes, update header checkbox if needed
    $scope.onVarianceRowChange = function (row) {
        if (!$scope.groupedData || !$scope.groupedData.length) return;
        var allSelected = $scope.groupedData.every(function (r) { return r.isVarianceSelected === true; });
        var noneSelected = $scope.groupedData.every(function (r) { return r.isVarianceSelected !== true; });
        // Keep header checkbox in sync
        $scope.dataObject.selectAllVariance = allSelected && !noneSelected;
    };

    // NEW: Show selected variance rows as JSON in modal
    $scope.showVarianceSelectionJson = function () {
        var selected = ($scope.groupedData || []).filter(function (r) { return r.isVarianceSelected === true; });
        var payload = selected.map(function (r) {
            return {
                PROCESS_TYPE_EDESC: r.PROCESS_TYPE_EDESC,
                PROCESS_EDESC: r.PROCESS_EDESC,
                PROCESS_CODE: (r.PROCESS_CODE || ''),
                ITEM_EDESC: r.ITEM_EDESC,
                ITEM_CODE: r.ITEM_CODE,
                INDEX_MU_CODE: r.INDEX_MU_CODE,
                STOCK: r.STOCK,
                PLAN_QUANTITY: r.PLAN_QUANTITY,
                REQUIRED_QUANTITY: r.REQUIRED_QUANTITY,
                VARIANCE: (r.STOCK - (r.PLAN_QUANTITY + r.REQUIRED_QUANTITY))
            };
        });
        $scope.varianceJsonPreview = JSON.stringify(payload, null, 2);
        setTimeout(function () { $('#varianceOutputModal').modal('show'); }, 50);
    };


    // NEW: Copy variance JSON to clipboard
    $scope.copyVarianceJsonToClipboard = function () {
        try {
            var temp = document.createElement('textarea');
            temp.value = $scope.varianceJsonPreview || '';
            document.body.appendChild(temp);
            temp.select();
            document.execCommand('copy');
            document.body.removeChild(temp);
            displayPopupNotification('Variance JSON copied to clipboard', 'success');
        } catch (e) {
            displayPopupNotification('Unable to copy', 'error');
        }
    };


    // Indent
    // NEW: Generate Indent report by sending selected variance rows to server
    $scope.generateVarianceReport = function () {
        debugger;
        var selected = ($scope.groupedData || []).filter(function (r) { return r.isVarianceSelected === true; });
        if (!selected.length) {
            displayPopupNotification('Please select at least one variance row', 'warning');
            return;
        }



        // Validate FROM_LOCATION_CODE and TO_LOCATION_CODE
        if (!$scope.ProductionPreferencesDetails.IND_FROM_LOCATION_CODE ||
            $scope.ProductionPreferencesDetails.IND_FROM_LOCATION_CODE.trim() === '') {
            displayPopupNotification('Please set the From Location before generating requisition.', 'warning');
            return;
        }

        if (!$scope.ProductionPreferencesDetails.IND_TO_LOCATION_CODE ||
            $scope.ProductionPreferencesDetails.IND_TO_LOCATION_CODE.trim() === '') {
            displayPopupNotification('Please set the To Location before generating requisition.', 'warning');
            return;
        }

        //var selectedItems = selected.map(function (r) {
        //    return {
        //        "COMPLETED_QUANTITY": 0,
        //        "CALC_TOTAL_PRICE": 0,
        //        "CALC_UNIT_PRICE": 0,
        //        "CALC_QUANTITY": (r.STOCK - (r.PLAN_QUANTITY + r.REQUIRED_QUANTITY)),
        //        "QUANTITY": (r.STOCK - (r.PLAN_QUANTITY + r.REQUIRED_QUANTITY)),
        //        "MU_CODE": r.INDEX_MU_CODE,
        //        "SPECIFICATION": "",
        //        "ITEM_CODE": r.ITEM_CODE
        //    };
        //});

        var selectedItems = selected.map(function (r) {
            var calcValue = r.STOCK - (r.PLAN_QUANTITY + r.REQUIRED_QUANTITY);
            return {
                "COMPLETED_QUANTITY": 0,
                "CALC_TOTAL_PRICE": 0,
                "CALC_UNIT_PRICE": 0,
                "CALC_QUANTITY": Math.abs(calcValue),
                "QUANTITY": Math.abs(calcValue),
                "MU_CODE": r.INDEX_MU_CODE,
                "SPECIFICATION": "",
                "ITEM_CODE": r.ITEM_CODE
            };
        });

        var Child_COLUMN_VALUE_LIST_OBJ = selectedItems;

        var Master_COLUMN_VALUE_OBJ = {
            "REMARKS": "",
            "FROM_LOCATION_CODE": $scope.ProductionPreferencesDetails.IND_FROM_LOCATION_CODE,  //   "02.01",
            "TO_LOCATION_CODE": $scope.ProductionPreferencesDetails.IND_TO_LOCATION_CODE,      //  
            "MANUAL_NO": "",
            "REQUEST_DATE": $filter('date')(new Date(), 'dd-MMM-yyyy'),
            "REQUEST_NO": ""
        }

        var total_variance_qty = selectedItems.reduce(function (sum, item) {
            return sum + (item.QUANTITY || 0);
        }, 0);

        //$scope.ProductionPreferencesDetails
        //$scope.selectedFormDetails = {
        //    RequisitionFormCodeValue: '',
        //    IndendFormCodeValue: '',
        //    MODULE_CODE_REQUISITION: '',
        //    MODULE_CODE_INDENT: ''
        //};

        var sendToObj = {
            "Save_Flag": 2,
            "Table_Name": $scope.ProductionPreferencesDetails.TABLE_NAME_INDENT,
            "Form_Code": $scope.ProductionPreferencesDetails.INDENT_FORM_CODE,
            "Master_COLUMN_VALUE": JSON.stringify(Master_COLUMN_VALUE_OBJ),
            "Child_COLUMN_VALUE": JSON.stringify(Child_COLUMN_VALUE_LIST_OBJ),
            "Grand_Total": total_variance_qty,
            "Custom_COLUMN_VALUE": "{}",
            "FROM_REF": false,
            "REF_MODEL": [],
            "Order_No": "undefined",
            "TempCode": "",
            "BUDGET_TRANS_VALUE": "[]",
            "CHARGES": "[]",
            "SHIPPING_DETAILS_VALUE": "{}",
            "SERIAL_TRACKING_VALUE": "[]",
            "BATCH_TRACKING_VALUE": "[]",
            "RESOURCE_LIST": [],
            "MANPOWER_RESOURCE_LIST": [],
            "CONSUMEABLE_POWER_RESOURCE_LIST": [],
            "MODULE_CODE": $scope.ProductionPreferencesDetails.MODULE_CODE_INDENT
        };






        debugger;
        console.log("Variance Report Data:", sendToObj);
        JSON.stringify(sendToObj);
        //return;

        // API endpoint for variance report (you may need to adjust this URL)
        var indentPostURL = window.location.protocol + "//" + window.location.host + "/api/InventoryApi/SaveInventoryFormData";

        $http({
            method: "POST",
            url: indentPostURL,
            data: sendToObj,
            headers: { "Content-Type": "application/json" }
        }).success(function (data, status, headers, config) {

            $scope.groupedData.forEach(function (row) {
                row.isVarianceSelected = false;
            });
            $scope.dataObject.selectAllVariance = false;

            console.log("Variance Report Success:", data);
            $scope.showSimplePopup("Indent Generated Successfully! </br> Report No: " + data.VoucherNo);
            //displayPopupNotification(
            //    'Variance report generated successfully! Report No: ' + data.VoucherNo,
            //    'success'
            //);

        }).error(function (data, status, headers, config) {
            console.error("Variance Report Error:", data);
            displayPopupNotification('Failed to generate variance report', 'error');
        });
    };




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

    $scope.locationCodeOption = {

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



    $scope.BrowseTreeListFortolocation = function (valueLocationType) {
        $scope.$broadcast("callChildEvent", valueLocationType);
    };








    $scope.showSimplePopup = function (message) {
        // Inject CSS only once
        if (!document.getElementById('simple-popup-style')) {
            var style = document.createElement('style');
            style.id = 'simple-popup-style';
            style.innerHTML = `
            .custom-success-popup {
                position: fixed;
                top: 0; left: 0;
                width: 100%; height: 100%;
                background: rgba(0, 0, 0, 0.4);
                display: flex;
                align-items: center;
                justify-content: center;
            }
            .custom-success-popup .popup-box {
                background: #fff;
                padding: 20px 30px;
                border-radius: 10px;
                min-width: 320px;
                text-align: center;
                box-shadow: 0 4px 15px rgba(0,0,0,0.3);
                border-top: 5px solid #4CAF50;
                animation: fadeIn 0.25s ease-in-out;
            }
            .custom-success-popup h4 { margin:0 0 10px; color:#4CAF50; font-size:20px; font-weight:bold; }
            .custom-success-popup p { margin:10px 0 15px; font-size:15px; color:#333; }
            .custom-success-popup button {
                margin-top:5px; padding:8px 18px; background:#4CAF50; color:white; border:none; border-radius:4px; cursor:pointer; font-size:14px; transition: background 0.2s;
            }
            .custom-success-popup button:hover { background:#45a049; }
            @keyframes fadeIn { from{opacity:0; transform:scale(0.9);} to{opacity:1; transform:scale(1);} }
        `;
            document.head.appendChild(style);
        }

        // Find highest z-index among open modals and backdrops
        var modals = document.querySelectorAll('.modal.in, .modal-backdrop.in');
        var highestZ = 1050; // default Bootstrap modal z-index
        modals.forEach(function (el) {
            var z = parseInt(window.getComputedStyle(el).zIndex) || 0;
            if (z >= highestZ) highestZ = z + 1; // increment so popup is above
        });

        // Create popup
        var popup = document.createElement('div');
        popup.className = 'custom-success-popup';
        popup.style.zIndex = highestZ; // dynamically above modal
        popup.innerHTML = `
                            <div class="popup-box">
                                <h4>Success</h4>
                                <p>${message}</p>
                                <button onclick="this.closest('.custom-success-popup').remove()">OK</button>
                            </div>
                        `;
        document.body.appendChild(popup);
    };



    angular.element(document).ready(function () {

        $('#productionPlanningNewAddModel').on('hidden.bs.modal', function (e) {
            // Modal has been closed (by any method: close button, backdrop click, or ESC)
            // You can call your AngularJS function or handle logic here
            $scope.$apply(function () {
                $scope.PlanFormClose('Add');
            });
        });

        setTimeout(function () {
            $('#ddlDateFilterVoucher').val('This Month').trigger('change');
        });

        setTimeout(function () {
            $scope.GridReCall();
            var treeviewPP = $("#treeviewPPlan").data("kendoTreeView");
            treeviewPP.expand(".k-item");
        }, 500);

        $("#ddlDateFilterVoucher").on("change", function () {
            var selectedValue = $(this).val();
            $scope.GridReCall();
        });
    });


    //setTimeout(function () {
    //    $scope.from_location = '02.03';
    //}, 500);


});





