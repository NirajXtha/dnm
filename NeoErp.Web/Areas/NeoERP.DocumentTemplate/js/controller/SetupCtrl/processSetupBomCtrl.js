DTModule.controller('processSetupBomCtrl', function ($scope, $timeout, $http, $filter, $route, processSetupBomService) {

    console.log("controller hit");

    $scope.FormName = "Process Setup (Bill of Materials)";

    $scope.txtSearchString = null;
    $scope.selectedProcessType = null;
    $scope.selectedLocation = null;

    $scope.IsCategoryProcessRoutineEdit = false;
    $scope.isCopyMode = false;
    $scope.selectedProcessForDDL = null;

    $scope.selectedProcessFlag = "CATEGORY";

    $scope.isRoutineDetail = false;
    $scope.allCategoryTreeData = [];
    $scope.isResourceEdit = false;

    $scope.searchText = "";

    $scope.processCategoryRoutineObj = {
        "ROOT_UNDER": "",
        "IN_ENGLISH": "",
        "IN_NEPALI": "",
        "PRIORITY_NUMBER": "",
        "PROCESS_CODE": "",
        "PRIMARY_PROCESS_NAME": "",
        "REMARK": ""
    };

    $scope.routineDetailsSetupObj = {
        "ROUTINE_NAME": "",
        "ROUTINE_CODE": "",
        "BELONGS": "",
        "BELONGS_CODE": "",
        "INPUT_INDEX_ITEM": "",
        "OUTPUT_INDEX_ITEM": "",
        "INPUT_CAPACITY": "",
        "INPUT_UNIT": "",
        "INPUT_UNIT_CODE": "",
        "INPUT_IN_PERIOD": "",
        "OUTPUT_CAPACITY": "",
        "OUTPUT_UNIT": "",
        "OUTPUT_UNIT_CODE": "",
        "OUTPUT_IN_PERIOD": ""
    };
    $scope.gridData = [];

    $scope.inputDialogAction = "Finish";
    $scope.outPutDialogAction = "Finish";

    $scope.inputMaterialSetupObj = {
        "ID": 1,
        "PROCESS": "",
        "PROCESS_CODE": "",
        "ITEM_NAME": "",
        "ITEM_CODE": "",
        "ITEM_EDESC": "",
        "QUANTITY": "",
        "UNIT": "",
        "UNIT_CODE": "",
        "MU_CODE": "",
        "REMARKS": "",
        "MODAL_INFO": "INPUT"
    };

    $scope.outPutMaterialSetupObj = {
        "PROCESS": "",
        "PROCESS_CODE": "",
        "ITEM_NAME": "",
        "ITEM_CODE": "",
        "ITEM_EDESC": "",
        "VALUATION_FLAG": "",
        "QUANTITY": "",
        "OUTPUT": "",
        "OUT_PUT": "",
        "UNIT": "",
        "UNIT_CODE": "",
        "MU_CODE": "",
        "REMARKS": "",
        "MODAL_INFO": "OUTPUT"
    };
    $("#selectedLocation").prop("disabled", false);
    $("#cprradio *").prop('disabled', false);

    $scope.currentOutPutItem = {};








    //Added by mahesh Inilitilize part

    $scope.AssignedResourceList = [];

    $scope.SelectedCategoryType = "";

    $scope.selectedAssignedResource = {};

    $scope.AllResourceList = [];
    $scope.filteredResources = [];
    $scope.selectedResource = {};


    $scope.AllUnitList = [];
    $scope.filterUnitList = [];
    $scope.selectedUnit = {};
    // Initialize the resource object for ng-model binding
    // This object will hold the data from your form fields.
    $scope.resourceAssignObj = {};

    $scope.resetResourceAssignModel = function () {
        $scope.resourceAssignObj = {
            processCode: '',
            processName: '', // Initial value from the image
            resourceName: '',
            resourceCode: '',
            quantity: null, // Use null for number fields initially
            unit: '',
            unitCode: '',
            remarks: '',
            categoryType: '',
            StandardInputQty: null,
            StandardOutputQty: null
        };
    }; $scope.resetResourceAssignModel();

    //$scope.resetProcessCategoryRoutineObj = function () {
    //    $scope.processCategoryRoutineObj = {
    //        "ROOT_UNDER": "",
    //        "IN_ENGLISH": "",
    //        "IN_NEPALI": "",
    //        "PRIORITY_NUMBER": "",
    //        "PRIMARY_PROCESS_NAME": "",
    //        "REMARK": ""
    //    };
    //}


    //End by mahesh

    /* Process Category Main Page Start */

    // All available process type for process setup
    $scope.getProcessType = function () {
        var processType = processSetupBomService.getProcessType();
        processType.then(function (processRes) {
            $scope.ProcessType = processRes.data;
        });
    };

    // all mu code (unit code) for selected item
    $scope.getProcessMuCode = function () {
        var allMuUnit = processSetupBomService.getProcessMuCode();
        allMuUnit.then(function (muRes) {
            $scope.AvailabelMuUnit = muRes.data;
        });
    };

    // item list for input output
    $scope.getAllItemForInputOutput = function () {
        var allItem = processSetupBomService.GetAllItemForInputOutput();
        allItem.then(function (itemRes) {
            $scope.AllItemForInputOutput = itemRes.data;
        });
    };

    //get all location for process
    $scope.getAllLocation = function () {
        var allLocation = processSetupBomService.GetAllLocation();
        allLocation.then(function (locRes) {
            $scope.AllAvailableLocation = locRes.data;
        });
    };

    $scope.BrowseInputUnitTreeList = function (tag) {
        $('#inputUnitModal').toggle();
        $scope.CalledFrom = tag;
    };

    $scope.BrowseOutputUnitTreeList = function (tag) {
        $('#outputUnitModal').modal('show');
        $scope.CalledFrom = tag;
    };

    // open input index item dialog
    $scope.BrowseInputIndexItem = function () {
        $('#inputIndexItemModal').toggle();
    };

    //configure and open input index grid
    $scope.inputIndexItemGridOptions = {
        dataSource: {
            data: [{}]
        },
        selectable: "single",
        scrollable: true,
        pageable: true,
        groupable: false,
        dataBound: function (e) {
            $("#inputIndexItemGrid tbody tr").css("cursor", "pointer");
            $("#inputIndexItemGrid tbody tr").on('dblclick', function () {
                var grid = $("#inputIndexItemGrid").data("kendoGrid");
                var dataItem = grid.dataItem(this);
                $scope.routineDetailsSetupObj.INPUT_INDEX_ITEM = dataItem.ITEM_CODE;
                $("#input_index_item").data("kendoComboBox").value(dataItem.ITEM_CODE);
                $("#inputIndexItemModal").toggle();
                $scope.$apply();
            });
        },
        columns: [
            {
                field: "ITEM_CODE",
                title: "Id"
            },
            {
                field: "ITEM_NAME",
                title: "Item Description"
            }
        ]
    };

    //configure production period grid
    $scope.productionInPeriodGridOptions = {
        //dataSource: {
        //    transport: {
        //        read: {
        //            url: window.location.protocol + "//" + window.location.host + "/api/ProcessSetupBomApi/GetProcessPeriod",
        //            dataType: "json"
        //        }
        //    }
        //},

        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/ProcessSetupBomApi/GetProcessPeriod",
                    dataType: "json"
                }
            },
            schema: {
                parse: function (response) {



                    // 🔹 ALSO UPDATE COMBOBOX DATASOURCE
                    var combo = $("#product_in_period").data("kendoComboBox");
                    if (combo) {
                        combo.setDataSource(new kendo.data.DataSource({
                            data: response
                        }));
                    }

                    return response;
                }
            }
        },




        selectable: "single",
        scrollable: true,
        pageable: true,
        groupable: false,
        dataBound: function (e) {
            $("#productionInPeriodGrid tbody tr").css("cursor", "pointer");
            $("#productionInPeriodGrid tbody tr").on('dblclick', function () {
                var grid = $("#productionInPeriodGrid").data("kendoGrid");
                var dataItem = grid.dataItem(this);

                console.log(dataItem);

                $scope.routineDetailsSetupObj.INPUT_IN_PERIOD = dataItem.PERIOD_CODE;
                //  $scope.routineDetailsSetupObj.INPUT_IN_PERIOD = dataItem.PERIOD_CODE;

                $("#product_in_period").data("kendoComboBox").value(dataItem.PERIOD_CODE);
                $("#productionInPeriodModal").toggle();
                $scope.$apply();
            });
        },
        columns: [
            {
                field: "PERIOD_CODE",
                title: "Id"
            },
            {
                field: "PERIOD_EDESC",
                title: "Period Description"
            }
        ]
    };

    //configure time take period grid
    $scope.timeTakenInPeriodGridOptions = {
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/ProcessSetupBomApi/GetProcessPeriod",
                    dataType: "json"
                }
            },
            schema: {
                parse: function (response) {
                    var combo = $("#time_taken_in_period").data("kendoComboBox");
                    if (combo) {
                        combo.setDataSource(new kendo.data.DataSource({
                            data: response
                        }));
                    }
                    return response;
                }
            }
        },
        selectable: "single",
        scrollable: true,
        pageable: true,
        groupable: false,
        dataBound: function (e) {
            $("#timeTakenInPeriodGrid tbody tr").css("cursor", "pointer");
            $("#timeTakenInPeriodGrid tbody tr").on('dblclick', function () {
                var grid = $("#timeTakenInPeriodGrid").data("kendoGrid");
                var dataItem = grid.dataItem(this);

                $scope.routineDetailsSetupObj.OUTPUT_IN_PERIOD = dataItem.PERIOD_CODE;
                $("#time_taken_in_period").data("kendoComboBox").value(dataItem.PERIOD_CODE);
                $("#timeTakenInPeriodModal").toggle();
                $scope.$apply();
            });
        },
        columns: [
            {
                field: "PERIOD_CODE",
                title: "Id"
            },
            {
                field: "PERIOD_EDESC",
                title: "Period Description"
            }
        ]
    };

    //production unit grid
    $scope.productionUnitGridOptions = {
        dataSource: {
            data: [{}]
        },
        selectable: "single",
        scrollable: true,
        pageable: true,
        groupable: false,
        dataBound: function (e) {
            $("#productionUnitGrid tbody tr").css("cursor", "pointer");
            $("#productionUnitGrid tbody tr").on('dblclick', function () {
                var grid = $("#productionUnitGrid").data("kendoGrid");
                var dataItem = grid.dataItem(this);
                $scope.routineDetailsSetupObj.INPUT_UNIT = dataItem.UNIT;
                $("#production_unit").data("kendoComboBox").value(dataItem.UNIT);
                $("#productionUnitModal").toggle();
                $scope.$apply();
            });

        },
        columns: [
            {
                field: "UNIT",
                title: "UNIT"
            }
        ]
    };

    $scope.timeTakenUnitGridOptions = {
        dataSource: {
            data: [{}]
        },
        selectable: "single",
        scrollable: true,
        pageable: true,
        groupable: false,
        dataBound: function (e) {
            $("#timeTakenUnitGrid tbody tr").css("cursor", "pointer");
            $("#timeTakenUnitGrid tbody tr").on('dblclick', function () {
                var grid = $("#timeTakenUnitGrid").data("kendoGrid");
                var dataItem = grid.dataItem(this);
                $scope.routineDetailsSetupObj.OUTPUT_UNIT = dataItem.UNIT;
                $("#time_taken_unit").data("kendoComboBox").value(dataItem.UNIT);
                $("#timeTakenUnitModal").toggle();
                $scope.$apply();
            });

        },
        columns: [
            {
                field: "UNIT",
                title: "UNIT"
            }
        ]
    };

    // close input index item dialog
    $scope.closeInputIndexItemModal = function () {
        $('#inputIndexItemModal').toggle();
    };

    // open output index item dialog
    $scope.BrowseOutputIndexItem = function () {
        $('#outPutIndexItemModal').toggle();
    };

    $scope.outPutIndexItemGridOptions = {
        dataSource: {
            data: [{}]
        },
        selectable: "single",
        scrollable: true,
        pageable: true,
        groupable: false,
        dataBound: function (e) {
            $("#outPutIndexItemGrid tbody tr").css("cursor", "pointer");
            $("#outPutIndexItemGrid tbody tr").on('dblclick', function () {
                var grid = $("#outPutIndexItemGrid").data("kendoGrid");
                var dataItem = grid.dataItem(this);

                var validationModel = {
                    RoutineDetail: {
                        OUTPUT_INDEX_ITEM: dataItem.ITEM_CODE,
                        ROUTINE_CODE: $scope.processCategoryRoutineObj.PROCESS_CODE
                    }
                };


                processSetupBomService.ValidationIndexItemCode(validationModel).then(function (res) {

                    $scope.routineDetailsSetupObj.OUTPUT_INDEX_ITEM = dataItem.ITEM_CODE;
                    $("#output_index_item").data("kendoComboBox").value(dataItem.ITEM_CODE);
                    $("#outPutIndexItemModal").toggle();
                    $scope.$apply();

                }, function (error) {
                    var msg = "Error validating index item code.";
                    if (error.data) {
                        if (typeof error.data === 'string') {
                            msg = error.data;
                        } else if (error.data.ExceptionMessage) {
                            msg = error.data.ExceptionMessage;
                        } else if (error.data.Message) {
                            msg = error.data.Message;
                        }
                    }

                    if (msg && typeof msg === 'string') {
                        msg = msg.replace(/\n/g, '<br/>');
                    }

                    if (error.status === 400 || error.status === 500) {
                        bootbox.confirm({
                            message: msg,
                            buttons: {
                                confirm: {
                                    label: 'Yes',
                                    className: 'btn-success'
                                },
                                cancel: {
                                    label: 'No',
                                    className: 'btn-danger'
                                }
                            },
                            callback: function (result) {
                                if (result) {
                                    //alert('ok called');
                                    $scope.routineDetailsSetupObj.OUTPUT_INDEX_ITEM = dataItem.ITEM_CODE; // dataItem.ITEM_NAME;
                                    $("#output_index_item").data("kendoComboBox").value(dataItem.ITEM_CODE);
                                    $("#outPutIndexItemModal").toggle();
                                    $scope.$apply();
                                } else {
                                    //alert('cancelled called');
                                    $scope.routineDetailsSetupObj.OUTPUT_INDEX_ITEM = "";
                                }
                            }
                        });
                    } else {
                        bootbox.alert(msg);
                    }
                });

            });

        },
        columns: [
            {
                field: "ITEM_CODE",
                title: "Id"
            },
            {
                field: "ITEM_NAME",
                title: "Item Description"
            }
        ]
    };

    // close out put index item dialog
    $scope.closeOutPutIndexItemModal = function () {
        $('#outPutIndexItemModal').toggle();
    };

    $scope.BrowseProductionUnit = function () {
        $("#productionUnitModal").toggle();
    };

    $scope.closeProductionUnitModal = function () {
        $("#productionUnitModal").toggle();
    };

    $scope.BrowseProductionInPeriod = function () {
        $("#productionInPeriodModal").toggle();
    };

    $scope.closeProductionInPeriodModal = function () {
        $("#productionInPeriodModal").toggle();
    };

    $scope.BrowseTimeTakenUnit = function () {
        $("#timeTakenUnitModal").toggle();
    };

    $scope.closeTimeTakenUnitModal = function () {
        $("#timeTakenUnitModal").toggle();
    };

    $scope.BrowseTimeTakenInPeriod = function () {

        $("#timeTakenInPeriodModal").toggle();
    };

    $scope.closeTimeTakenInPeriodModal = function () {
        $("#timeTakenInPeriodModal").toggle();
    };


    //--------------------------------END----------------------------------------

    var processTypeUrl = window.location.protocol + "//" + window.location.host + "/api/ProcessSetupBomApi/GetAllProcessTypeCode";
    $scope.processTypeDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: processTypeUrl
            }
        }
    });

    $scope.processTypeOptions = {
        dataSource: $scope.processTypeDataSource,
        filter: "contains",
        optionLabel: "--Select Process Type--",
        dataTextField: "PROCESS_TYPE_EDESC",
        dataValueField: "PROCESS_TYPE_CODE"

    };

    var itemUrl = window.location.protocol + "//" + window.location.host + "/api/ProcessSetupBomApi/GetAllItemForInputOutput";
    $scope.itemDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: itemUrl
            }
        }
    });

    $scope.InputItemUnitList = [];
    $("#inputMaterialsItem").kendoAutoComplete({
        minLength: 1,
        dataTextField: "ITEM_EDESC",
        dataSource: $scope.itemDataSource,
        filter: "contains",
        placeholder: "Select Item...",
        select: function (e) {

            $("#inputMaterialsUnit").data("kendoComboBox").value("");
            var dataItem = this.dataItem(e.item.index());

            $scope.inputMaterialSetupObj.ITEM_NAME = dataItem.ITEM_EDESC;
            $scope.inputMaterialSetupObj.ITEM_CODE = dataItem.ITEM_CODE;

            $scope.inputMaterialSetupObj.UNIT = dataItem.MU_EDESC;
            $scope.inputMaterialSetupObj.UNIT_CODE = dataItem.MU_CODE;
            //set unit as per item

            $scope.InputItemUnitList.push({
                "MU_CODE": dataItem.MU_CODE,
                "MU_EDESC": dataItem.MU_EDESC
            });

            var inputUnitPerItem = $("#inputMaterialsUnit").data("kendoComboBox");
            inputUnitPerItem.dataSource.data($scope.InputItemUnitList);
            inputUnitPerItem.dataSource.query();
            inputUnitPerItem.select(function (dataItems) {
                return dataItems.MU_EDESC = dataItem.MU_EDESC;
            });
        }
        //separator: ","
    });

    $scope.OutputItemUnitList = [];
    $("#outPutMaterialsItem").kendoAutoComplete({
        minLength: 1,
        dataTextField: "ITEM_EDESC",
        dataSource: $scope.itemDataSource,
        filter: "contains",
        placeholder: "Select Item...",
        select: function (e) {

            $("#outputMaterialsUnit").data("kendoComboBox").value("");
            var dataItem = this.dataItem(e.item.index());
            $scope.outPutMaterialSetupObj.ITEM_NAME = dataItem.ITEM_EDESC;
            $scope.outPutMaterialSetupObj.ITEM_CODE = dataItem.ITEM_CODE;

            $scope.outPutMaterialSetupObj.UNIT = dataItem.MU_EDESC;
            $scope.outPutMaterialSetupObj.UNIT_CODE = dataItem.MU_CODE;

            $scope.OutputItemUnitList.push({
                "MU_CODE": dataItem.MU_CODE,
                "MU_EDESC": dataItem.MU_EDESC
            });
            var outputUnitPerItem = $("#outputMaterialsUnit").data("kendoComboBox");
            outputUnitPerItem.dataSource.data($scope.OutputItemUnitList);
            outputUnitPerItem.dataSource.query();
            //outputUnitPerItem.select(0);
            outputUnitPerItem.select(function (dataItems) {
                return dataItems.MU_EDESC = dataItem.MU_EDESC;
            });

        }
        //separator: ","
    });

    var locationUrl = window.location.protocol + "//" + window.location.host + "/api/ProcessSetupBomApi/GetAllLocation";
    $scope.locationDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: locationUrl
            }
        }
    });

    $scope.locationOptions = {
        dataSource: $scope.locationDataSource,
        filter: "contains",
        optionLabel: "--Select Location--",
        dataTextField: "LOCATION_EDESC",
        dataValueField: "LOCATION_CODE"
    };

    $scope.inputMuGridOptions = {
        dataSource: {
            type: "json",
            transport: {
                read: "/api/ProcessSetupBomApi/GetProcessMuCodeList",
                parameterMap: function (options, type) {
                    //if (type === 'read') {
                    //    return {
                    //        from: "bom"
                    //    };
                    //}
                }
            },
            pageSize: 20,
            serverSorting: true
        },
        selectable: "single",
        scrollable: true,
        pageable: true,
        groupable: false,
        dataBound: function (e) {
            $("#inputMuGrid tbody tr").css("cursor", "pointer");
            $("#inputMuGrid tbody tr").on('dblclick', function () {
                var grid = $("#inputMuGrid").data("kendoGrid");
                var dataItem = grid.dataItem(this);
                $scope.routineDetailsSetupObj.INPUT_UNIT = dataItem.MU_EDESC;
                if ($scope.CalledFrom === 'input') $scope.inputMaterialSetupObj.UNIT = dataItem.MU_EDESC; $scope.inputMaterialSetupObj.UNIT_CODE = dataItem.MU_CODE;
                if ($scope.CalledFrom === 'output') $scope.outPutMaterialSetupObj.UNIT = dataItem.MU_EDESC; $scope.outPutMaterialSetupObj.UNIT_CODE = dataItem.MU_CODE;
                //$scope.routineDetailsSetupObj.INPUT_UNIT_CODE = dataItem.MU_CODE;
                $("#inputUnitModal").toggle();
                $scope.$apply();
            });

        },
        columns: [
            {
                field: "MU_CODE",
                title: "Codes"
            },
            {
                field: "MU_EDESC",
                title: "Description"
            }
        ]
    };

    $scope.outPutMuGridOptions = {
        dataSource: {
            type: "json",
            transport: {
                read: "/api/ProcessSetupBomApi/GetProcessMuCodeList",
                parameterMap: function (options, type) {
                    //if (type === 'read') {
                    //    return {
                    //        from: "bom"
                    //    };
                    //}
                }
            },
            pageSize: 20,
            serverSorting: true
        },
        selectable: "single",
        scrollable: true,
        pageable: true,
        groupable: false,
        dataBound: function (e) {
            $("#outPutMuGrid tbody tr").css("cursor", "pointer");
            $("#outPutMuGrid tbody tr").on('dblclick', function () {
                var grid = $("#outPutMuGrid").data("kendoGrid");
                var dataItem = grid.dataItem(this);
                $scope.routineDetailsSetupObj.OUTPUT_UNIT = dataItem.MU_EDESC;
                if ($scope.CalledFrom === 'output') $scope.outPutMaterialSetupObj.UNIT = dataItem.MU_EDESC; $scope.outPutMaterialSetupObj.UNIT_CODE = dataItem.MU_CODE;
                if ($scope.CalledFrom === 'input') $scope.inputMaterialSetupObj.UNIT = dataItem.MU_EDESC; $scope.inputMaterialSetupObj.UNIT_CODE = dataItem.MU_CODE;
                //$scope.routineDetailsSetupObj.INPUT_UNIT_CODE = dataItem.MU_CODE;
                $("#outputUnitModal").toggle();
                $scope.$apply();
            });

        },
        columns: [
            {
                field: "MU_CODE",
                title: "Codes"
            },
            {
                field: "MU_EDESC",
                title: "Description"
            }
        ]
    };


    var processTypeDDLUrl = window.location.protocol + "//" + window.location.host + "/api/ProcessSetupBomApi/GetAllProcessForDDL";
    $scope.processTypeDDLDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: processTypeDDLUrl
            }
        }
    });

    $scope.processTypeDDLOptions = {

        dataSource: $scope.processTypeDDLDataSource,
        filter: "contains",
        optionLabel: "--Select Root Type--",
        dataTextField: "PROCESS_EDESC",
        dataValueField: "PROCESS_CODE"
        //onChange: onDDLChange,

    };

    $scope.fillPrimaryProcessName = false;
    $scope.onDDLChange = function (e) {

        var selectedText = $("#processType").data("kendoDropDownList").text();

        if (selectedText === "Primary") {

            $scope.processCategoryRoutineObj.PROCESS_CODE = $scope.selectedProcessForDDL;
            $scope.fillPrimaryProcessName = true;


        } else {
            $scope.fillPrimaryProcessName = false;
            var childProcessCode = processSetupBomService.GetChildProcessCode($scope.selectedProcessForDDL);
            childProcessCode.then(function (cpRes) {
                $scope.processCategoryRoutineObj.PROCESS_CODE = cpRes.data;
            });
        }
    };


    $scope.getAllCategoryProcessRoutineTree = function () {
        var allCategoryProcessRoutine = processSetupBomService.getAllCategoryProcessRoutineTree();
        allCategoryProcessRoutine.then(function (catRes) {
            $scope.AllCategoryProcessRoutine = catRes.data;
        });
    };

    $scope.removeNodeSelection = function () {
        var treeview = $("#categoryTreeGrid").data("kendoTreeView");
        treeview.select($());
        $scope.BindRoutineResultGrid();
    };

    var getCategoryTree = window.location.protocol + "//" + window.location.host + "/api/ProcessSetupBomApi/GetAllProcessCategoryRoutine";
    var categoryTreeDataSource = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: getCategoryTree,
                dataType: "json"
            }
        },
        schema: {
            model: {
                id: "PROCESS_CODE",
                children: "ITEMS",
                hasChildren: "HAS_BRANCH"
            },
            parse: function (response) {
                $scope.allCategoryTreeData = response;
                return _.each(_.filter(response, function (x) {
                    return x.PRE_PROCESS_CODE === '00';
                }), function (y) {
                    y.ITEMS = _.filter(response, function (z) {
                        return z.PRE_PROCESS_CODE === y.PROCESS_CODE;
                    });
                    y.HAS_BRANCH = y.ITEMS.length === 0 ? false : true;
                });
            }
        }
    });

    $("#dropdowntree").kendoDropDownList({
        dataTextField: "PROCESS_EDESC",
        dataValueField: "PROCESS_CODE",
        dataSource: categoryTreeDataSource,
        //index: 0,
        change: onChange
    });


    $scope.processCategoryRoutineObj.PROCESS_CODE = $("#dropdowntree").val();

    function onChange(e) {

    }


    $("#categoryTreeGrid").kendoTreeView({
        loadOnDemand: false,
        // autoScroll: true,
        // autoBind: true,
        dataSource: categoryTreeDataSource,
        dataTextField: "PROCESS_EDESC",
        // height: 400,
        select: onCategorySelect,
        //scrollable: {
        //    virtual: true
        //},
        //dataBound: function () {

        //}

    });


    //Search on TREEE

    $("#processSearchString").on("input", function () {
        var query = this.value.toLowerCase();
        var dataSource = $("#categoryTreeGrid").data("kendoTreeView").dataSource;

        filter(dataSource, query);
    });

    function filter(dataSource, query) {
        var hasVisibleChildren = false;
        var data = dataSource instanceof kendo.data.HierarchicalDataSource && dataSource.data();

        for (var i = 0; i < data.length; i++) {
            var item = data[i];
            var text = item.PROCESS_EDESC.toLowerCase();
            var itemVisible =
                query === true // parent already matches
                || query === "" // query is empty
                || text.indexOf(query) >= 0; // item text matches query

            var anyVisibleChildren = filter(item.children, itemVisible || query); // pass true if parent matches

            hasVisibleChildren = hasVisibleChildren || anyVisibleChildren || itemVisible;

            item.hidden = !itemVisible && !anyVisibleChildren;
        }

        if (data) {
            // Re-apply the filter on the children.
            dataSource.filter({ field: "hidden", operator: "neq", value: true });
        }

        return hasVisibleChildren;
    }


    var selectedItemList = [];

    var selectedItemObj = {
        "PROCESS_CODE": "",
        "PROCESS_EDESC": ""
    };
    $scope.selectedParentProcessCode;

    $scope.isSelectedCatIsProcess = false;
    function onCategorySelect(e) {
        debugger;
        $('#newProcessSetupBtn').attr("disabled", false);
        $('#editRoutine').attr("disabled", true);
        $('#copyRoutine').attr("disabled", true);
        var treeview = $("#categoryTreeGrid").data("kendoTreeView");
        var item = treeview.dataItem(e.node);
        console.log("selected category : " + JSON.stringify(item));

        if (item) {

            if (item.PROCESS_FLAG === "P") $scope.isSelectedCatIsProcess = true;

            selectedItemObj = {
                "PROCESS_CODE": item.PROCESS_CODE,
                "PROCESS_EDESC": item.PROCESS_EDESC
            };
            $scope.selectedParentProcessCode = item.PROCESS_CODE;
            $scope.selectedParentProcessDesc = item.PROCESS_EDESC;


            $scope.selectedProcessForDDL = item.PROCESS_CODE;
            $scope.routineDetailsSetupObj.BELONGS = selectedItemObj.PROCESS_EDESC;
            var childProcess = processSetupBomService.GetChildProcessCode($scope.selectedProcessForDDL);
            childProcess.then(function (cpRes) {
                $scope.processCategoryRoutineObj.PROCESS_CODE = cpRes.data;
                $scope.routineDetailsSetupObj.BELONGS = cpRes.data;
            });

            var routineResultGrid = $("#routineResultGrid").data("kendoGrid");
            routineResultGrid.dataSource.transport.options.read.url = window.location.protocol + "//" + window.location.host + "/api/ProcessSetupBomApi/GetRoutineBasedOnProcessCode?processCode=" + selectedItemObj.PROCESS_CODE;
            routineResultGrid.dataSource.read();

        } else {
            selectedItem.pop();
        }

    }

    $scope.BindRoutineResultGrid = function () {

        $scope.routineResultOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: "/api/ProcessSetupBomApi/GetRoutineBasedOnProcessCode",
                    parameterMap: function (options, type) {
                        if (type === 'read') {
                            return {
                                processCode: $scope.selectedParentProcessCode || "",  //selectedItemObj.PROCESS_CODE,
                                searchText: $scope.searchText
                            };
                        }
                    }
                },
                pageSize: 20,
                serverSorting: true
            },
            selectable: "single",
            resizable: true,
            scrollable: true,
            pageable: true,
            groupable: false,
            change: routineRowSelected,
            dataBound: function (e) {

                $("#routineResultGrid tbody tr").css("cursor", "pointer");

            },
            columns: [
                {
                    field: "SHORT_CUT",
                    title: "Short Cut"
                },
                {
                    field: "PROCESS_DESCRIPTION",
                    title: "Process Description"
                },
                {
                    field: "ITEM_DESCRIPTION",
                    title: "Item Description"
                },
                {
                    field: "CAPACITY",
                    title: "Capacity"
                },
                {
                    field: "MU_CODE",
                    title: "MU"
                },
                {
                    field: "LOCATION_EDESC",
                    title: "Location"
                }
            ],
        };
    };

    function routineRowSelected(e) {
        debugger;
        var grid = $("#routineResultGrid").data("kendoGrid"),
            selectedItem = grid.dataItem(grid.select());

        if (selectedItem) {
            $('#editRoutine').attr("disabled", false);
            $('#copyRoutine').attr("disabled", false);
            $('#deleteRoutine').attr("disabled", false);
            $('#newProcessSetupBtn').attr("disabled", true);

            selectedItemObj = {
                "PROCESS_CODE": selectedItem.SHORT_CUT,
                "PROCESS_EDESC": selectedItem.PROCESS_DESCRIPTION
            };

            // $scope.selectedProcessForDDL = selectedItem.SHORT_CUT;
            $scope.selectedProcessForDDL = $scope.selectedParentProcessCode;
            var childProcess = processSetupBomService.GetChildProcessDetail(selectedItemObj.PROCESS_CODE);
            childProcess.then(function (cpRes) {
                debugger;
                console.log("CP RES Detail: " + JSON.stringify(cpRes));
                $scope.processCategoryRoutineObj.PROCESS_CODE = cpRes.data.PROCESS_CODE;
                $scope.routineDetailsSetupObj.ROUTINE_NAME = cpRes.data.PROCESS_EDESC;
                $scope.processCategoryRoutineObj.IN_ENGLISH = cpRes.data.PROCESS_EDESC;
                $scope.processCategoryRoutineObj.IN_NEPALI = cpRes.data.PROCESS_EDESC;
                $scope.selectedProcessType = cpRes.data.PROCESS_TYPE_CODE;
                $scope.selectedLocation = cpRes.data.PRE_LOCATION_CODE;
                $scope.processCategoryRoutineObj.PRIORITY_NUMBER = cpRes.data.PRIORITY_ORDER_NO;
                $scope.processCategoryRoutineObj.REMARKS = cpRes.data.REMARKS;

                //---


            });



            //childProcess.then(function (cpRes) {
            //    $scope.processCategoryRoutineObj.PROCESS_CODE = cpRes.data.PROCESS_CODE;
            //    $scope.routineDetailsSetupObj.BELONGS = cpRes.data;
            //});

            //var routineResultGrid = $("#routineResultGrid").data("kendoGrid");
            //routineResultGrid.dataSource.transport.options.read.url = window.location.protocol + "//" + window.location.host + "/api/ProcessSetupBomApi/GetRoutineBasedOnProcessCode?processCode=" + selectedItemObj.PROCESS_CODE;
            //routineResultGrid.dataSource.read();

        } else {
            selectedItem.pop();
        }

    }

    $scope.BindRoutineResultGrid();
    $scope.onContextAdd = function (event) {
        debugger;
        setTimeout(function () {
            debugger;
            if (event.PROCESS_FLAG == "C") {
                $scope.selectedProcessFlag = "PROCESS";
                $("input:radio[value='PROCESS']").prop('checked', true);
                $('#addNewProcessModel').toggle();


            }
            if (event.PROCESS_FLAG == "P") {
                $scope.selectedProcessFlag = "ROUTINE";
                $("input:radio[value='ROUTINE']").prop('checked', true);
                //$('#routineRadio').attr('checked', true);
                $('#addNewProcessModel').toggle();
            }
            //$('#newProcessSetupBtn').trigger('click');
        }, 0);
    }
    $scope.onContextUpdate = function (event) {
        debugger;
        var childProcess = processSetupBomService.GetChildProcessDetail(event.PROCESS_CODE);
        childProcess.then(function (cpRes) {
            debugger;
            console.log("CP RES Detail: " + JSON.stringify(cpRes));
            $scope.processCategoryRoutineObj.PROCESS_CODE = cpRes.data.PROCESS_CODE;
            $scope.routineDetailsSetupObj.ROUTINE_NAME = cpRes.data.PROCESS_EDESC;
            $scope.processCategoryRoutineObj.IN_ENGLISH = cpRes.data.PROCESS_EDESC;
            $scope.processCategoryRoutineObj.IN_NEPALI = cpRes.data.PROCESS_EDESC;
            $scope.selectedProcessType = cpRes.data.PROCESS_TYPE_CODE;
            $scope.selectedLocation = cpRes.data.PRE_LOCATION_CODE;
            $scope.processCategoryRoutineObj.PRIORITY_NUMBER = cpRes.data.PRIORITY_ORDER_NO;
            $scope.processCategoryRoutineObj.REMARK = cpRes.data.REMARKS;
            $('#processType').data("kendoDropDownList").value(event.PROCESS_TYPE_CODE);
            $('#selectedLocation').data("kendoDropDownList").value(event.LOCATION_CODE);
            $scope.IsCategoryProcessRoutineEdit = true;
            if (event.PROCESS_FLAG == "R") {
                $scope.selectedProcessFlag = "ROUTINE";
                $('#addNewProcessModel').toggle();
            } else if (event.PROCESS_FLAG == "P") {
                $scope.selectedProcessFlag = "PROCESS";
                $("input:radio[value='PROCESS']").prop('checked', true);
                $('#addNewProcessModel').toggle();
            } else {
                $scope.selectedProcessFlag = "CATEGORY";
                $('#catRadio').attr('checked', true);
                $('#addNewProcessCategoryModel').toggle();
            }

        });
        //$scope.processCategoryRoutineObj.PROCESS_CODE = event.PROCESS_CODE;
        //$scope.processCategoryRoutineObj.IN_ENGLISH = event.PROCESS_EDESC;
        //$scope.selectedProcessType = event.PROCESS_TYPE_CODE;       
        //$scope.selectedLocation = event.LOCATION_CODE;
        //$scope.processCategoryRoutineObj.PRIORITY_NUMBER = event.PRIORITY_ORDER_NO;             
    }
    $scope.onContextDelete = function (event) {
        debugger;
        bootbox.confirm({
            title: "Delete",
            message: "Are you sure?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success',
                    label: '<i class="fa fa-check"></i> Yes',
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger',
                    label: '<i class="fa fa-times"></i> No',
                }
            },
            callback: function (result) {

                if (result == true) {

                    var delUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteProcessSetupByProcessCode?processCode=" + event.PROCESS_CODE;
                    $http({
                        method: 'POST',
                        url: delUrl
                    }).then(function successCallback(response) {

                        if (response.data.MESSAGE == "DELETED") {
                            var tree = $("#customertree").data("kendoTreeView");
                            tree.dataSource.read();
                            $scope.treenodeselected = "N";
                            displayPopupNotification("Data succesfully deleted ", "success");
                        }
                        if (response.data.MESSAGE == "HAS_CHILD") {
                            displayPopupNotification("Please delete child first", "warning");
                        }
                    }, function errorCallback(response) {
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                        // called asynchronously if an error occurs
                        // or server returns response with an error status.
                    });

                }

                else {
                    bootbox.hideAll();
                    //displayPopupNotification(response.data.message, "error");

                }
            }
        });
    }

    $scope.reloadRoutineGridOnSearchText = function () {
        var routineResultGrid = $("#routineResultGrid").data("kendoGrid");
        // routineResultGrid.dataSource.transport.options.read.url = window.location.protocol + "//" + window.location.host + "/api/ProcessSetupBomApi/GetRoutineBasedOnProcessCode?processCode=" + selectedItemObj.PROCESS_CODE + "&searchText=" + $scope.searchText;
        routineResultGrid.dataSource.read();
    };

    $scope.SaveProcessCategorySetup = function () {
        // debugger;
        var saveModel = {
            "IN_ENGLISH": $scope.processCategoryRoutineObj.IN_ENGLISH,
            "IN_NEPALI": $scope.processCategoryRoutineObj.IN_NEPALI,
            "PROCESS_CODE": $scope.processCategoryRoutineObj.PROCESS_CODE,
            "REMARK": $scope.processCategoryRoutineObj.REMARK,
            "PROCESS_FLAG": $scope.selectedProcessFlag,
            "PRIORITY_NUMBER": $scope.processCategoryRoutineObj.PRIORITY_NUMBER,
            "PROCESS_TYPE": $scope.selectedProcessType,
            "ROOT_UNDER": $scope.selectedProcessForDDL,
            "LOCATION": $scope.selectedLocation,
            "IS_EDIT": $scope.IsCategoryProcessRoutineEdit

        };

        if ($scope.isCopyMode) {
            // Logic for copying: Skip the master save API and go to next popup
            // We need to fetch the details of the SOURCE routine to populate the second popup
            $scope.isRoutineDetail = true;
            $('#addNewProcessModel').hide();
            $('#addNewProcessCategoryModel').hide();

            $scope.routineDetailsSetupObj.ROUTINE_NAME = $scope.processCategoryRoutineObj.IN_ENGLISH;
            $scope.routineDetailsSetupObj.ROUTINE_CODE = $scope.processCategoryRoutineObj.PROCESS_CODE;
            $scope.routineDetailsSetupObj.BELONGS = $scope.selectedParentProcessDesc;
            $scope.routineDetailsSetupObj.BELONGS_CODE = $scope.processCategoryRoutineObj.ROOT_UNDER;

            // Reset all lists
            $scope.InputIndexItemList = [];
            $scope.ProductionUnitList = [];
            $scope.InputPeriodList = [];
            $scope.OutputIndexItemList = [];
            $scope.TimeTakenUnitList = [];
            $scope.OutputPeriodList = [];
            $scope.totalQuantity = 0;

            var sourceProcessCode = $scope.sourceCodeToCopy;

            // Fetch Input Details
            processSetupBomService.GetInputRoutineDetail(JSON.stringify(sourceProcessCode)).then(function (iRes) {
                if (iRes.data.length > 0) {
                    $scope.IsInputCreated = true;
                    $("#btnSaveRoutineDetail").show();
                    var billOfMatGrid = $("#billOfMatGrid").data("kendoGrid");
                    billOfMatGrid.dataSource.data(iRes.data);

                    var totalQty = 0;
                    for (var i = 0; i < iRes.data.length; i++) {
                        totalQty += parseFloat(iRes.data[i].QUANTITY || 0);
                        $scope.InputIndexItemList.push({
                            "ITEM_CODE": iRes.data[i].ITEM_CODE,
                            "ITEM_NAME": iRes.data[i].ITEM_EDESC
                        });
                        $scope.ProductionUnitList.push({ "UNIT": iRes.data[i].MU_CODE });
                        $scope.InputPeriodList.push({
                            "PERIOD_CODE": iRes.data[i].PERIOD_CODE,
                            "PERIOD_EDESC": iRes.data[i].PERIOD_EDESC
                        });
                    }
                    $scope.totalQuantity = totalQty;

                    // Refresh Input Controls
                    var inputIdxCombo = $("#input_index_item").data('kendoComboBox');
                    if (inputIdxCombo) {
                        inputIdxCombo.dataSource.data($scope.InputIndexItemList);
                        inputIdxCombo.value(iRes.data[0].ITEM_CODE);
                    }
                    $scope.routineDetailsSetupObj.INPUT_INDEX_ITEM = iRes.data[0].ITEM_CODE;

                    var inputIdxGrid = $("#inputIndexItemGrid").data("kendoGrid");
                    if (inputIdxGrid) inputIdxGrid.dataSource.data($scope.InputIndexItemList);

                    var productPeriodCombo = $("#product_in_period").data("kendoComboBox");
                    if (productPeriodCombo) {
                        productPeriodCombo.dataSource.data($scope.InputPeriodList);
                        productPeriodCombo.value(iRes.data[0].PERIOD_CODE);
                    }
                    $scope.routineDetailsSetupObj.INPUT_IN_PERIOD = iRes.data[0].PERIOD_CODE;

                    var prodUnitCombo = $("#production_unit").data("kendoComboBox");
                    if (prodUnitCombo) {
                        prodUnitCombo.dataSource.data($scope.ProductionUnitList);
                        prodUnitCombo.value(iRes.data[0].MU_CODE);
                    }
                    var prodUnitGrid = $("#productionUnitGrid").data("kendoGrid");
                    if (prodUnitGrid) prodUnitGrid.dataSource.data($scope.ProductionUnitList);

                    $scope.routineDetailsSetupObj.INPUT_CAPACITY = iRes.data[0].INDEX_CAPACITY;

                }
            });

            // Fetch Output Details
            var getOutputItemOfGivenRoutine = processSetupBomService.GetOutputRoutineDetail(JSON.stringify(sourceProcessCode));
            getOutputItemOfGivenRoutine.then(function (oRes) {
                debugger;
                if (oRes.data.length > 0) {
                    debugger;
                    console.log("o.Res.data===============>>" + JSON.stringify(oRes.data));

                    if (oRes.data && oRes.data.length == 1) {
                        $scope.currentOutPutItem = oRes.data[0];
                    }

                    //reinitialize the output material setup grid
                    var outputMatGrid = $("#outputMatGrid").data("kendoGrid");
                    outputMatGrid.dataSource.data(oRes.data);
                    outputMatGrid.dataSource.query();

                    //debugger;
                    for (var o = 0; o < oRes.data.length; o++) {
                        // $scope.routineDetailsSetupObj.OUTPUT_INDEX_ITEM = oRes.data[o].ITEM_CODE;
                        $scope.routineDetailsSetupObj.OUTPUT_UNIT = oRes.data[o].MU_CODE;

                        //input side data binding
                        $scope.routineDetailsSetupObj.INPUT_UNIT = oRes.data[o].MU_CODE;



                        $scope.OutputIndexItemList.push({
                            "ITEM_CODE": oRes.data[o].ITEM_CODE,
                            "ITEM_NAME": oRes.data[o].ITEM_EDESC
                        });





                        //reinitialize the out put index item kendocombox
                        var odex = $("#output_index_item").data('kendoComboBox');
                        odex.dataSource.data($scope.OutputIndexItemList);
                        odex.dataSource.query();

                        //odex.select(0);
                        //$scope.routineDetailsSetupObj.OUTPUT_INDEX_ITEM = oRes.data.ITEM_EDESC;
                        //$("#output_index_item").data("kendoComboBox").value(oRes.data.ITEM_CODE);

                        //reinitialize the out put index grid
                        var outputGrid = $("#outPutIndexItemGrid").data("kendoGrid");
                        outputGrid.dataSource.data($scope.OutputIndexItemList);
                        outputGrid.dataSource.query();

                        if (oRes.data[o].OUTPUT_INDEX_ITEM_CODE) {
                            $scope.routineDetailsSetupObj.OUTPUT_INDEX_ITEM = oRes.data[o].OUTPUT_INDEX_ITEM_CODE;
                            $("#output_index_item").data("kendoComboBox").value(oRes.data[o].OUTPUT_INDEX_ITEM_CODE);
                        }


                        $scope.TimeTakenUnitList.push({
                            "UNIT": oRes.data[o].MU_CODE
                        });


                        //reinitialize the out put unit box
                        var timeTakenUnit = $("#time_taken_unit").data("kendoComboBox");
                        timeTakenUnit.dataSource.data($scope.TimeTakenUnitList);
                        timeTakenUnit.dataSource.query();
                        timeTakenUnit.select(0);

                        //$scope.outPutMaterialSetupObj.UNIT = oRes.data[o].UNIT;
                        //$scope.outPutMaterialSetupObj.UNIT_CODE = oRes.data[o].UNIT_CODE;

                        var timeTakenUnitGrid = $("#timeTakenUnitGrid").data("kendoGrid");
                        timeTakenUnitGrid.dataSource.data($scope.TimeTakenUnitList);
                        timeTakenUnitGrid.dataSource.query();

                        $scope.OutputPeriodList.push({
                            "PERIOD_CODE": oRes.data[o].PERIOD_CODE,
                            "PERIOD_EDESC": oRes.data[o].PERIOD_EDESC
                        });


                        //reinitialize the out put unit box
                        var timeTakenPeriodcBox = $("#time_taken_in_period").data("kendoComboBox");
                        timeTakenPeriodcBox.dataSource.data($scope.OutputPeriodList);
                        timeTakenPeriodcBox.dataSource.query();
                        timeTakenPeriodcBox.select(0);
                        $scope.routineDetailsSetupObj.OUTPUT_CAPACITY = oRes.data[o].INDEX_TIME_REQUIRED;
                        $scope.routineDetailsSetupObj.OUTPUT_IN_PERIOD = oRes.data[o].PERIOD_CODE;

                    }

                    $timeout(function () {
                        var grid = $("#productionUnitGrid").data("kendoGrid");
                        grid.dataSource.data($scope.TimeTakenUnitList);
                        grid.refresh();

                        var prodUnit = $("#production_unit").data("kendoComboBox");
                        prodUnit.dataSource.data($scope.TimeTakenUnitList);
                        prodUnit.dataSource.query();
                        prodUnit.select(0);
                    });


                    $('#routineDetailSetup').toggle();
                    // end--

                } else {
                    alert("no data");
                }
            });

            return; // Exit function, don't call API
        }


        if (!$scope.IsCategoryProcessRoutineEdit) {


            $scope.isRoutineDetail = true;
            //  DisplayBarNotificationMessage("Process created successfully");
            // $('#addNewProcessCategoryModel').hide();
            $scope.routineDetailsSetupObj.ROUTINE_NAME = $scope.processCategoryRoutineObj.IN_ENGLISH;
            $scope.routineDetailsSetupObj.ROUTINE_CODE = $scope.processCategoryRoutineObj.PROCESS_CODE;
            $scope.routineDetailsSetupObj.BELONGS = $scope.selectedParentProcessDesc;
            $scope.routineDetailsSetupObj.BELONGS_CODE = $scope.processCategoryRoutineObj.ROOT_UNDER;
            $('#routineDetailSetup').toggle();
            return;
        }


        var saveResult = processSetupBomService.SaveProcessCategorySetup(saveModel);
        saveResult.then(function (res) {
            debugger;
            if (res.data.MESSAGE === "Successful") {
                if (res.data.PROCESS_FLAG === "ROUTINE" || res.data.PROCESS_FLAG === 'R') {
                    debugger;
                    $scope.IsCategoryProcessRoutineEdit = false;
                    $scope.isRoutineDetail = true;
                    //  DisplayBarNotificationMessage("Process created successfully");
                    $('#addNewProcessCategoryModel').hide();
                    $scope.routineDetailsSetupObj.ROUTINE_NAME = $scope.processCategoryRoutineObj.IN_ENGLISH;
                    $scope.routineDetailsSetupObj.ROUTINE_CODE = $scope.processCategoryRoutineObj.PROCESS_CODE;
                    $scope.routineDetailsSetupObj.BELONGS = $scope.selectedParentProcessDesc;
                    $scope.routineDetailsSetupObj.BELONGS_CODE = $scope.processCategoryRoutineObj.ROOT_UNDER;


                    if (res.data.SAVED_MODAL.IS_EDIT) {
                        debugger;
                        var getInputItemOfGivenRoutine = processSetupBomService.GetInputRoutineDetail(JSON.stringify($scope.processCategoryRoutineObj.PROCESS_CODE));
                        getInputItemOfGivenRoutine.then(function (iRes) {
                            if (iRes.data.length > 0) {

                                $("#btnSaveRoutineDetail").show();

                                var billOfMatGrid = $("#billOfMatGrid").data("kendoGrid");
                                billOfMatGrid.dataSource.data(iRes.data);
                                billOfMatGrid.dataSource.query();
                                var inputItemList = iRes.data;

                                for (var i = 0; i < iRes.data.length; i++) {

                                    //$scope.routineDetailsSetupObj.INPUT_INDEX_ITEM = iRes.data[i].ITEM_CODE;

                                    //  $scope.routineDetailsSetupObj.INPUT_UNIT = iRes.data[i].MU_CODE;

                                    $scope.InputIndexItemList.push({
                                        "ITEM_CODE": iRes.data[i].ITEM_CODE,
                                        "ITEM_NAME": iRes.data[i].ITEM_EDESC
                                    });

                                    //reinitialize the input index item comboBox
                                    var idex = $("#input_index_item").data('kendoComboBox');
                                    idex.dataSource.data($scope.InputIndexItemList);
                                    idex.dataSource.query();
                                    idex.select(0);


                                    //reinitialize the input index item grid
                                    var inputGrid = $("#inputIndexItemGrid").data("kendoGrid");
                                    inputGrid.dataSource.data($scope.InputIndexItemList);
                                    inputGrid.dataSource.query();



                                    $scope.ProductionUnitList.push({
                                        "UNIT": iRes.data[i].MU_CODE
                                    });


                                    //reinitialize the input unit comboBox
                                    //var prodUnit = $("#production_unit").data("kendoComboBox");
                                    //prodUnit.dataSource.data($scope.ProductionUnitList);
                                    //prodUnit.dataSource.query();
                                    //prodUnit.select(0);

                                    //$scope.inputMaterialSetupObj.UNIT =iRes.data[i].UNIT;
                                    //$scope.inputMaterialSetupObj.UNIT_CODE = iRes.data[i].UNIT_CODE;

                                    //reinitialize the input unit grid
                                    //var prodUnitGrid = $("#productionUnitGrid").data("kendoGrid");
                                    //prodUnitGrid.dataSource.data($scope.ProductionUnitList);
                                    //prodUnitGrid.dataSource.query();

                                    $scope.InputPeriodList.push({
                                        "PERIOD_CODE": iRes.data[i].PERIOD_CODE,
                                        "PERIOD_EDESC": iRes.data[i].PERIOD_EDESC
                                    });

                                    // commented by mahesh
                                    var inputPeriodCBox = $("#product_in_period").data("kendoComboBox");
                                    inputPeriodCBox.dataSource.data($scope.InputPeriodList);
                                    inputPeriodCBox.dataSource.query();
                                    inputPeriodCBox.select(0);



                                    // alert(iRes.data[i].PERIOD_CODE);

                                    console.log('here we are--');
                                    console.log(iRes.data[i]);

                                    $scope.routineDetailsSetupObj.INPUT_CAPACITY = iRes.data[i].INDEX_CAPACITY;
                                    $scope.routineDetailsSetupObj.INPUT_IN_PERIOD = iRes.data[i].PERIOD_CODE;
                                    console.log("res.data[] tttt======================>>> " + JSON.stringify(iRes.data));
                                    //reinitialize the input unit grid
                                    //var inputPeriodGrid = $("#productionUnitGrid").data("kendoGrid");
                                    //inputPeriodGrid.dataSource.data($scope.ProductionUnitList);
                                    //inputPeriodGrid.dataSource.query();


                                }


                                if (inputItemList && inputItemList.length) {
                                    var itemData = inputItemList.filter(item =>
                                        item.ITEM_CODE == inputItemList[0].INPUT_INDEX_ITEM_CODE
                                    );
                                    //--alert(itemData[0].ITEM_CODE + "--" + itemData[0].ITEM_EDESC);
                                    $scope.routineDetailsSetupObj.INPUT_INDEX_ITEM = itemData[0].ITEM_CODE;
                                    $("#input_index_item").data("kendoComboBox").value(itemData[0].ITEM_CODE);

                                    $scope.IsInputCreated = true;

                                    // $scope.$apply();
                                }

                            } else {
                                alert("no data");
                            }
                        });


                        var getOutputItemOfGivenRoutine = processSetupBomService.GetOutputRoutineDetail(JSON.stringify($scope.processCategoryRoutineObj.PROCESS_CODE));
                        getOutputItemOfGivenRoutine.then(function (oRes) {
                            debugger;
                            if (oRes.data.length > 0) {
                                debugger;
                                console.log("o.Res.data===============>>" + JSON.stringify(oRes.data));

                                if (oRes.data && oRes.data.length == 1) {
                                    $scope.currentOutPutItem = oRes.data[0];
                                }

                                //reinitialize the output material setup grid
                                var outputMatGrid = $("#outputMatGrid").data("kendoGrid");
                                outputMatGrid.dataSource.data(oRes.data);
                                outputMatGrid.dataSource.query();

                                //debugger;
                                for (var o = 0; o < oRes.data.length; o++) {
                                    // $scope.routineDetailsSetupObj.OUTPUT_INDEX_ITEM = oRes.data[o].ITEM_CODE;
                                    $scope.routineDetailsSetupObj.OUTPUT_UNIT = oRes.data[o].MU_CODE;

                                    //input side data binding
                                    $scope.routineDetailsSetupObj.INPUT_UNIT = oRes.data[o].MU_CODE;



                                    $scope.OutputIndexItemList.push({
                                        "ITEM_CODE": oRes.data[o].ITEM_CODE,
                                        "ITEM_NAME": oRes.data[o].ITEM_EDESC
                                    });





                                    //reinitialize the out put index item kendocombox
                                    var odex = $("#output_index_item").data('kendoComboBox');
                                    odex.dataSource.data($scope.OutputIndexItemList);
                                    odex.dataSource.query();

                                    //odex.select(0);
                                    //$scope.routineDetailsSetupObj.OUTPUT_INDEX_ITEM = oRes.data.ITEM_EDESC;
                                    //$("#output_index_item").data("kendoComboBox").value(oRes.data.ITEM_CODE);

                                    //reinitialize the out put index grid
                                    var outputGrid = $("#outPutIndexItemGrid").data("kendoGrid");
                                    outputGrid.dataSource.data($scope.OutputIndexItemList);
                                    outputGrid.dataSource.query();

                                    if (oRes.data[o].OUTPUT_INDEX_ITEM_CODE) {
                                        $scope.routineDetailsSetupObj.OUTPUT_INDEX_ITEM = oRes.data[o].OUTPUT_INDEX_ITEM_CODE;
                                        $("#output_index_item").data("kendoComboBox").value(oRes.data[o].OUTPUT_INDEX_ITEM_CODE);
                                    }


                                    $scope.TimeTakenUnitList.push({
                                        "UNIT": oRes.data[o].MU_CODE
                                    });


                                    //reinitialize the out put unit box
                                    var timeTakenUnit = $("#time_taken_unit").data("kendoComboBox");
                                    timeTakenUnit.dataSource.data($scope.TimeTakenUnitList);
                                    timeTakenUnit.dataSource.query();
                                    timeTakenUnit.select(0);

                                    //$scope.outPutMaterialSetupObj.UNIT = oRes.data[o].UNIT;
                                    //$scope.outPutMaterialSetupObj.UNIT_CODE = oRes.data[o].UNIT_CODE;

                                    var timeTakenUnitGrid = $("#timeTakenUnitGrid").data("kendoGrid");
                                    timeTakenUnitGrid.dataSource.data($scope.TimeTakenUnitList);
                                    timeTakenUnitGrid.dataSource.query();

                                    $scope.OutputPeriodList.push({
                                        "PERIOD_CODE": oRes.data[o].PERIOD_CODE,
                                        "PERIOD_EDESC": oRes.data[o].PERIOD_EDESC
                                    });


                                    //reinitialize the out put unit box
                                    var timeTakenPeriodcBox = $("#time_taken_in_period").data("kendoComboBox");
                                    timeTakenPeriodcBox.dataSource.data($scope.OutputPeriodList);
                                    timeTakenPeriodcBox.dataSource.query();
                                    timeTakenPeriodcBox.select(0);
                                    $scope.routineDetailsSetupObj.OUTPUT_CAPACITY = oRes.data[o].INDEX_TIME_REQUIRED;
                                    $scope.routineDetailsSetupObj.OUTPUT_IN_PERIOD = oRes.data[o].PERIOD_CODE;

                                }

                                $timeout(function () {
                                    var grid = $("#productionUnitGrid").data("kendoGrid");
                                    grid.dataSource.data($scope.TimeTakenUnitList);
                                    grid.refresh();

                                    var prodUnit = $("#production_unit").data("kendoComboBox");
                                    prodUnit.dataSource.data($scope.TimeTakenUnitList);
                                    prodUnit.dataSource.query();
                                    prodUnit.select(0);
                                });

                                // end--

                            } else {
                                alert("no data");
                            }
                        });

                    }
                    $('#routineDetailSetup').toggle();
                } else {
                    // DisplayBarNotificationMessage("Process created successfully");
                    $scope.IsCategoryProcessRoutineEdit = false;
                    setTimeout(function () {
                        //location.reload(true);
                        $route.reload();
                    }, 500);
                }
            }
        });

    };

    $scope.SaveProcessSetup = function () {

        var saveModel = {
            "IN_ENGLISH": $scope.processCategoryRoutineObj.IN_ENGLISH,
            "IN_NEPALI": $scope.processCategoryRoutineObj.IN_NEPALI,
            "PROCESS_CODE": $scope.processCategoryRoutineObj.PROCESS_CODE,
            "REMARK": $scope.processCategoryRoutineObj.REMARK,
            "PROCESS_FLAG": $scope.selectedProcessFlag,
            "PRIORITY_NUMBER": $scope.processCategoryRoutineObj.PRIORITY_NUMBER,
            "LOCATION": $scope.selectedLocation,
            "PROCESS_TYPE": $scope.selectedProcessType,
            "ROOT_UNDER": $scope.selectedProcessForDDL

        };

        var saveResult = processSetupBomService.SaveProcessSetup(saveModel);
        saveResult.then(function (res) {

            if (res.data.MESSAGE === "Successful") {

                if (res.data.PROCESS_FLAG === "ROUTINE") {
                    $scope.isRoutineDetail = true;
                    $scope.routineDetailsSetupObj.BELONGS = $scope.selectedParentProcessDesc;
                    $scope.routineDetailsSetupObj.BELONGS_CODE = $scope.processCategoryRoutineObj.ROOT_UNDER;
                    $scope.routineDetailsSetupObj.ROUTINE_NAME = $scope.processCategoryRoutineObj.IN_ENGLISH;
                    $scope.routineDetailsSetupObj.ROUTINE_CODE = $scope.processCategoryRoutineObj.PROCESS_CODE;
                    //  DisplayBarNotificationMessage("Process created successfully");

                    var billOfMatGrid = $("#billOfMatGrid").data('kendoGrid');
                    billOfMatGrid.dataSource.transport.options.read.url = window.location.protocol + "//" + window.location.host + "/ProcessSetupBomApi/GetBillOfMaterialList?processCode=" + $scope.processCategoryRoutineObj.PROCESS_CODE;
                    billOfMatGrid.dataSource.read();

                    var billOfOutMatGrid = $("#outputMatGrid").data('kendoGrid');
                    billOfOutMatGrid.dataSource.transport.options.read.url = window.location.protocol + "//" + window.location.host + "/ProcessSetupBomApi/GetOutputMaterialList?processCode=" + $scope.processCategoryRoutineObj.PROCESS_CODE;
                    billOfOutMatGrid.dataSource.read();

                    $('#addNewProcessCategoryModel').toggle();
                    $('#routineDetailSetup').toggle();
                } else {
                    //   DisplayBarNotificationMessage("Process created successfully");
                    setTimeout(function () {
                        // location.reload(true);
                        $route.reload();
                    }, 500);
                }
            }
        });

    };

    /* Process Category Main Page End */

    //------------------------------------------------------------------------------------------------------------------------------------------------------

    /* Routine Detail Setup Page Start */


    $scope.getDefaultLocation = function (processCode) {

        var underLocation = $http({
            method: "GET",
            url: "/api/ProcessSetupBomApi/GetUnderLocationDetail?process_code=" + processCode,
            dataType: "json"
        });
        return underLocation;
    };





    $scope.newProcessSetup = function () {


        var processCode = $scope.selectedParentProcessCode;

        $scope.getDefaultLocation(processCode)
            .then(function (response) {


                $scope.selectedLocation = response && response.data;
                debugger;
                $scope.IsCategoryProcessRoutineEdit = false;
                if ($scope.isSelectedCatIsProcess) {
                    $scope.selectedProcessFlag = "ROUTINE";
                    $('#addNewProcessModel').toggle();

                } else if (selectedItemObj.PROCESS_CODE) {
                    $scope.selectedProcessFlag = "PROCESS";
                    $('#addNewProcessModel').toggle();
                } else {
                    $scope.selectedProcessFlag = "CATEGORY";
                    $('#addNewProcessCategoryModel').toggle();
                    $('#catRadio').attr('checked', true);

                    // $('#routineDetailSetup').toggle();
                }

                //$('#routineDetailSetup').toggle();


            }, function (error) {
                // error
                //  console.log("Error:", error);
                //  alert("Failed to load default location");
            });
    };

    $scope.editProcessRoutineSetup = function () {

        if (selectedItemObj.PROCESS_CODE) {

            var treeView = $("#categoryTreeGrid").data("kendoTreeView");
            var hierarchicalData = treeView.dataSource.data();

            var dataFiltered = $scope.allCategoryTreeData.filter(x => x.PROCESS_CODE == selectedItemObj.PROCESS_CODE && x.PROCESS_FLAG == "R");
            if (dataFiltered.length && dataFiltered.length == 1) {
                var processData = $scope.allCategoryTreeData.filter(x => x.PROCESS_CODE == dataFiltered[0].PRE_PROCESS_CODE && x.PROCESS_FLAG == "P");
                if (processData && processData.length && processData.length == 1) {
                    $scope.selectedProcessForDDL = processData[0].PROCESS_CODE;
                }
            }

            $scope.selectedProcessFlag = "ROUTINE";
            $scope.IsCategoryProcessRoutineEdit = true;
            var tree = $("#selectedLocation").data("kendoDropDownList");
            tree.value($scope.selectedLocation);
            $("#cprradio *").prop('disabled', true);
            $("#selectedLocation").prop("disabled", true);

            $('#addNewProcessModel').toggle();
        }
    };

    $scope.copyProcessRoutineSetup = function () {


        debugger;

        if (selectedItemObj.PROCESS_CODE) {
            $scope.isCopyMode = true;
            $scope.sourceCodeToCopy = selectedItemObj.PROCESS_CODE;
            // Similar logic to edit but IsCategoryProcessRoutineEdit = false
            var dataFiltered = $scope.allCategoryTreeData.filter(x => x.PROCESS_CODE == selectedItemObj.PROCESS_CODE && x.PROCESS_FLAG == "R");
            if (dataFiltered.length && dataFiltered.length == 1) {
                var processData = $scope.allCategoryTreeData.filter(x => x.PROCESS_CODE == dataFiltered[0].PRE_PROCESS_CODE && x.PROCESS_FLAG == "P");
                if (processData && processData.length && processData.length == 1) {
                    $scope.selectedProcessForDDL = processData[0].PROCESS_CODE;
                }
            }

            $scope.selectedProcessFlag = "ROUTINE";
            $scope.IsCategoryProcessRoutineEdit = false; // It's a copy, so treat as new save

            // Re-fetch current detail to ensure we have all values
            var childProcess = processSetupBomService.GetChildProcessDetail(selectedItemObj.PROCESS_CODE);
            childProcess.then(function (cpRes) {
                $scope.processCategoryRoutineObj.IN_ENGLISH = cpRes.data.PROCESS_EDESC + " - Copy";
                $scope.processCategoryRoutineObj.IN_NEPALI = cpRes.data.PROCESS_EDESC;
                $scope.selectedProcessType = cpRes.data.PROCESS_TYPE_CODE;
                $scope.selectedLocation = cpRes.data.PRE_LOCATION_CODE;
                $scope.processCategoryRoutineObj.PRIORITY_NUMBER = cpRes.data.PRIORITY_ORDER_NO;
                $scope.processCategoryRoutineObj.REMARKS = cpRes.data.REMARKS;

                // Get a NEW process code for the copy
                var childProcessCodeTmp = processSetupBomService.GetChildProcessCode($scope.selectedProcessForDDL);
                childProcessCodeTmp.then(function (newCodeRes) {
                    $scope.processCategoryRoutineObj.PROCESS_CODE = newCodeRes.data;

                    var tree = $("#selectedLocation").data("kendoDropDownList");
                    tree.value($scope.selectedLocation);
                    $("#cprradio *").prop('disabled', true);
                    $("#selectedLocation").prop("disabled", true);
                    $('#addNewProcessModel').toggle();
                });
            });
        }
    };

    $scope.deleteProcessRoutineSetup = function () {
        if (selectedItemObj.PROCESS_CODE) {
            bootbox.confirm("Are you sure you want to delete this routine?", function (result) {
                if (result) {
                    var deleteProcess = processSetupBomService.DeleteProcessSetup(selectedItemObj.PROCESS_CODE);
                    deleteProcess.then(function (res) {
                        if (res.data === "Successful") {
                            displayPopupNotification("Routine deleted successfully.", "success");
                            $scope.reloadRoutineGridOnSearchText();
                            // Refresh Tree as well
                            var treeView = $("#categoryTreeGrid").data("kendoTreeView");
                            treeView.dataSource.read();

                            // Clear selection and disable buttons
                            $('#editRoutine').attr("disabled", true);
                            $('#copyRoutine').attr("disabled", true);
                            $('#deleteRoutine').attr("disabled", true);
                            $('#newProcessSetupBtn').attr("disabled", false);
                            selectedItemObj = { "PROCESS_CODE": "", "PROCESS_EDESC": "" };
                        } else {
                            displayPopupNotification("Error while deleting routine.", "error");
                        }
                    }, function (err) {
                        displayPopupNotification("Status: " + err.status + " " + err.statusText, "error");
                    });
                }
            });
        }
    };

    $scope.close = function () {
        $('#addNewProcessCategoryModel').toggle();
        //$('#routineDetailSetup').toggle();
    };

    $scope.closeProcessModel = function () {
        $scope.isCopyMode = false;
        $('#newProcessSetupBtn').attr("disabled", false);
        $('#editRoutine').attr("disabled", true);
        $('#copyRoutine').attr("disabled", true);
        $('#addNewProcessModel').toggle();


        // Reset with Current Process data
        $scope.processCategoryRoutineObj = {
            "IN_ENGLISH": "",
            "IN_NEPALI": "",
            "REMARK": "",
            PROCESS_CODE: $scope.selectedProcessForDDL
        };
        $scope.selectedProcessType = "";
        var childProcessCodeTmp = processSetupBomService.GetChildProcessCode($scope.selectedProcessForDDL);
        childProcessCodeTmp.then(function (cpRes) {
            $scope.processCategoryRoutineObj.PROCESS_CODE = cpRes.data;
        });


        setTimeout(function () {
            // location.reload(true);
            $route.reload();

            //  var grid = $("#routineResultGrid").data("kendoGrid");
            //  grid.dataSource.read();
            //  grid.refresh();

        }, 100);
    };

    $scope.newRoutineDetailSetup = function () {

        $('#routineDetailSetup').toggle();
    };

    $scope.closeRoutineDetailSetup = function () {
        $('#routineDetailSetup').toggle();
    };

    $scope.closeProcessMuUnit = function () {
        $('#inputUnitModal').hide();
        $('#outputUnitModal').modal('hide');
    };
    $scope.ChangeFlag = function (value) {
        $scope.selectedProcessFlag = value;
    };


    //Input masterial grid Options
    $scope.billOfMatGridOptions = {
        dataSource: {
            type: "json",
            transport: {
                read: "/api/ProcessSetupBomApi/GetBillOfMaterialList?processCode=" + 0
            },
            pageSize: 20
        },
        selectable: "single",
        resizable: true,
        scrollable: true,
        pageable: true,
        groupable: false,
        change: inputMatSelected,
        editable: "popup",
        dataBound: function (e) {
            $("#billOfMatGrid tbody tr").css("cursor", "pointer");

            var grid = e.sender;
            var dataSource = grid.dataSource;
            var allData = dataSource.data();

            var totalQuantity = 0;
            allData.forEach(function (item) {
                totalQuantity += parseFloat(item.QUANTITY || 0);
            });

            $scope.totalQuantity = totalQuantity;

        },
        columns: [
            {
                field: "MODEL_INFO", hidden: true
            },
            {
                title: "SKU ITEMS",
                field: "ITEM_EDESC"
            },
            {
                field: "QUANTITY",
                title: "Quantity"
            },
            {
                field: "MU_CODE",
                title: "Unit"
            },
            {
                field: "ITEM_CODE",
                title: "Item Code"
            },
            {
                field: "REMARKS",
                title: "Remark"
            }
        ]
    };

    $scope.outputMatGridOptions = {
        dataSource: {
            type: "json",
            transport: {
                read: "/api/ProcessSetupBomApi/GetOutputMaterialList?processCode=" + 0
            },
            pageSize: 20
        },
        selectable: "single",
        resizable: true,
        scrollable: true,
        pageable: true,
        groupable: false,
        change: outputMatSelected,
        dataBound: function (e) {
            //var insertedRow = this.dataSource.view();
            //for (var or = 0; or < insertedRow.length; or++) {
            //    $scope.outPutMaterialSetupObj.UNIT = insertedRow[or].UNIT_EDESC;
            //    $scope.outPutMaterialSetupObj.UNIT_CODE = insertedRow[or].UNIT_CODE;
            //}
            $("#outputMatGrid tbody tr").css("cursor", "pointer");

        },
        columns: [
            {
                field: "MODEL_INFO", hidden: true
            },
            //{
            //    field: "PROCESS_CODE", hidden: true
            //},
            {
                // field: "SKU_FLAG",
                field: "ITEM_EDESC",
                title: "SKU ITEMS"
            },
            {
                field: "QUANTITY",
                title: "Quantity"
            },
            {
                //field: "MU_CODE",
                field: "MU_CODE",
                title: "Unit"
            },
            {
                field: "OUT_PUT",
                title: "Output"
            },
            {
                field: "VALUATION_FLAG",
                title: "Valuation Flag"
            },
            {
                // field: "ITEM_CODE",
                field: "ITEM_CODE",
                title: "Item Code"
            },
            {
                // field: "REMARKS",
                field: "REMARKS",
                title: "Remark"
            }
        ]
    };

    function outputMatSelected() {
        $('#outPutMatEdit').attr("disabled", false);
        $('#outputMatDelete').attr("disabled", false);
    }

    $("#input_index_item").kendoComboBox({
        dataTextField: "ITEM_NAME",
        dataValueField: "ITEM_CODE",
        dataSource: $scope.inputEntry,
        filter: "contains",
        suggest: true,
        index: 2,
        select: function (e) {
            //debugger;
            //console.log("input item index: " + JSON.stringify(e.dataItem));

            //setTimeout(function () {
            $scope.routineDetailsSetupObj.INPUT_INDEX_ITEM = e.dataItem.ITEM_CODE;
            $("#input_index_item").data("kendoComboBox").value(e.dataItem.ITEM_CODE);
            $scope.$apply();
            //}, 300);


        }

    });

    $("#output_index_item").kendoComboBox({
        dataTextField: "ITEM_NAME",
        dataValueField: "ITEM_CODE",
        dataSource: $scope.inputEntry,
        filter: "contains",
        suggest: true,
        index: 2,
        select: function (e) {
            // console.log("Output item index: " + JSON.stringify(e.dataItem));
            $scope.routineDetailsSetupObj.OUTPUT_INDEX_ITEM = e.dataItem.ITEM_CODE;
        }
    });

    $("#production_unit").kendoComboBox({
        dataTextField: "UNIT",
        dataValueField: "UNIT",
        dataSource: $scope.inputEntry,
        filter: "contains",
        suggest: true,
        index: 2,
        select: function (e) {
            // console.log("Output item index: " + JSON.stringify(e.dataItem));
            $scope.routineDetailsSetupObj.INPUT_UNIT_CODE = e.dataItem.UNIT;
        }
    });

    $("#product_in_period").kendoComboBox({
        dataTextField: "PERIOD_EDESC",
        dataValueField: "PERIOD_CODE",
        dataSource: $scope.inputEntry,
        filter: "contains",
        suggest: true,
        index: 2,
        select: function (e) {
            // console.log("Output item index: " + JSON.stringify(e.dataItem));
            $scope.routineDetailsSetupObj.INPUT_IN_PERIOD = e.dataItem.PERIOD_CODE;
        }
    });

    $("#time_taken_unit").kendoComboBox({
        dataTextField: "UNIT",
        dataValueField: "UNIT",
        dataSource: $scope.inputEntry,
        filter: "contains",
        suggest: true,
        index: 2,
        select: function (e) {
            // console.log("Output item index: " + JSON.stringify(e.dataItem));
            $scope.routineDetailsSetupObj.OUTPUT_UNIT_CODE = e.dataItem.UNIT;
        }
    });

    $("#time_taken_in_period").kendoComboBox({
        dataTextField: "PERIOD_EDESC",
        dataValueField: "PERIOD_CODE",
        dataSource: $scope.inputEntry,
        filter: "contains",
        suggest: true,
        index: 2,
        select: function (e) {
            // console.log("Output item index: " + JSON.stringify(e.dataItem));
            $scope.routineDetailsSetupObj.OUTPUT_IN_PERIOD = e.dataItem.PERIOD_CODE;
        }
    });

    $("#inputMaterialsUnit").kendoComboBox({
        dataTextField: "MU_EDESC",
        dataValueField: "MU_CODE",
        dataSource: $scope.inputEntry,
        filter: "contains",
        suggest: true,
        index: 2
    });

    $("#outputMaterialsUnit").kendoComboBox({
        dataTextField: "MU_EDESC",
        dataValueField: "MU_CODE",
        dataSource: $scope.inputEntry,
        filter: "contains",
        suggest: true,
        index: 2
    });



    /* Routine Detail Setup Page End */

    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    /* Input Material Setup Page Start */
    $scope.openNewInputMaterialSetup = function () {
        $scope.inputDialogAction = "Finish";
        $scope.inputMaterialSetupObj.PROCESS = $scope.routineDetailsSetupObj.ROUTINE_NAME;    //$scope.processCategoryRoutineObj.IN_ENGLISH;
        $scope.inputMaterialSetupObj.PROCESS_CODE = $scope.processCategoryRoutineObj.PROCESS_CODE;
        $('#inputMaterialSetupModel').toggle();
    };

    $scope.inputEntry = {};
    function inputMatSelected(e) {
        $('#inputMatEdit').attr("disabled", false);
        $('#inputMatDelete').attr("disabled", false);
        //var rows = e.sender.select();
        //rows.each(function (e) {
        //    var grid = $("#billOfMatGrid").data("kendoGrid");
        //    var dataItem = grid.dataItem(this);
        //    $scope.inputEntry = dataItem;
        //    var idex = $("#input_index_item").data('kendoComboBox');
        //    idex.dataSource.data([$scope.inputEntry]);
        //    idex.dataSource.query();
        //});

        //console.log("$scope.inputEntry: " + JSON.stringify($scope.inputEntry));
    }

    $scope.deleteInputMaterialSetup = function () {
        var grid = $("#billOfMatGrid").data("kendoGrid");

        // finding all the selected rows
        var rows = grid.select();
        rows.each(function (index, row) {
            // reading each selected item
            var selectedItem = grid.dataItem(row);

            console.log("selected item to delete: " + JSON.stringify(selectedItem));
            for (var i = 0; i < $scope.InputIndexItemList.length; i++) {
                if ($scope.InputIndexItemList[i].ITEM_CODE === selectedItem.ITEM_CODE) {
                    $scope.InputIndexItemList.pop($scope.InputIndexItemList[i]);
                }
            }

            for (var j = 0; j < $scope.ProductionUnitList.length; j++) {
                if ($scope.ProductionUnitList[j].UNIT === selectedItem.UNIT) {
                    $scope.ProductionUnitList.pop($scope.ProductionUnitList[j]);
                }
            }

            // finally removing selected item from grid data source
            grid.dataSource.remove(selectedItem);

            //remove item from item grid
            var inputGrid = $("#inputIndexItemGrid").data("kendoGrid");
            inputGrid.dataSource.data($scope.InputIndexItemList);
            inputGrid.dataSource.query();


            var idex = $("#input_index_item").data('kendoComboBox');
            //idex.dataSource.data([$scope.inputMaterialSetupObj]);
            idex.dataSource.data($scope.InputIndexItemList);
            idex.dataSource.query();

            var prodUnit = $("#production_unit").data("kendoComboBox");
            prodUnit.dataSource.data($scope.ProductionUnitList);
            prodUnit.dataSource.query();

            var prodUnitGrid = $("#productionUnitGrid").data("kendoGrid");
            prodUnitGrid.dataSource.data($scope.ProductionUnitList);
            prodUnitGrid.dataSource.query();

        });
    };

    $scope.deleteOutputMaterialSetup = function () {
        var grid = $("#outputMatGrid").data("kendoGrid");
        var rows = grid.select();
        rows.each(function (index, row) {
            var selectedItem = grid.dataItem(row);

            for (var i = 0; i < $scope.OutputIndexItemList.length; i++) {
                if ($scope.OutputIndexItemList[i].ITEM_CODE === selectedItem.ITEM_CODE) {
                    $scope.OutputIndexItemList.pop($scope.OutputIndexItemList[i]);
                }
            }

            for (var j = 0; j < $scope.TimeTakenUnitList.length; j++) {
                if ($scope.TimeTakenUnitList[j].UNIT === selectedItem.UNIT) {
                    $scope.TimeTakenUnitList.pop($scope.TimeTakenUnitList[j]);
                }
            }

            grid.dataSource.remove(selectedItem);

            var odex = $("#output_index_item").data('kendoComboBox');
            odex.dataSource.data($scope.OutputIndexItemList);
            odex.dataSource.query();

            var inputGrid = $("#outPutIndexItemGrid").data("kendoGrid");
            inputGrid.dataSource.data($scope.OutputIndexItemList);
            inputGrid.dataSource.query();


            var timeTakenUnit = $("#time_taken_unit").data("kendoComboBox");
            timeTakenUnit.dataSource.data($scope.TimeTakenUnitList);
            timeTakenUnit.dataSource.query();

            var timeTakenUnitGrid = $("#timeTakenUnitGrid").data("kendoGrid");
            timeTakenUnitGrid.dataSource.data($scope.TimeTakenUnitList);
            timeTakenUnitGrid.dataSource.query();
        });
    };

    $scope.editInputMaterialSetup = function () {
        var inputMatGrid = $("#billOfMatGrid").data("kendoGrid");
        var dataRows = inputMatGrid.items();
        var selected = inputMatGrid.select();
        $scope.InputRowIndex = dataRows.index(selected);
        if (selected.length === 0) {
            alert('No record selected');
        } else {

            for (var i = 0; i < selected.length; i++) {
                var dataItem = inputMatGrid.dataItem(selected[i]);
                console.log("Selected Input Grid: ================>>", JSON.stringify(dataItem));

                var muCode = dataItem.MU_CODE;
                var muDesc = dataItem.MU_EDESC || muCode; // Fallback to Code if Description is missing or undefined

                $scope.inputMaterialSetupObj.PROCESS = $scope.routineDetailsSetupObj.ROUTINE_NAME;          //dataItem.PROCESS_EDESC;
                $scope.inputMaterialSetupObj.PROCESS_CODE = $scope.processCategoryRoutineObj.PROCESS_CODE;                                               //dataItem.PROCESS_CODE;
                $scope.inputMaterialSetupObj.ITEM_NAME = dataItem.ITEM_NAME;
                $scope.inputMaterialSetupObj.ITEM_CODE = dataItem.ITEM_CODE;
                $scope.inputMaterialSetupObj.ITEM_EDESC = dataItem.ITEM_EDESC;
                $scope.inputMaterialSetupObj.QUANTITY = dataItem.QUANTITY;
                $scope.inputMaterialSetupObj.UNIT = muDesc;
                $scope.inputMaterialSetupObj.UNIT_CODE = dataItem.UNIT_CODE || muCode;
                $scope.inputMaterialSetupObj.MU_CODE = muCode;
                $scope.inputMaterialSetupObj.REMARKS = dataItem.REMARKS;


                var inputMatItemAutoComplete = $("#inputMaterialsItem").data("kendoAutoComplete");
                if (inputMatItemAutoComplete) {
                    inputMatItemAutoComplete.value(dataItem.ITEM_EDESC);
                    inputMatItemAutoComplete.trigger("change");
                }

                $scope.InputItemUnitList.push({
                    "MU_CODE": muCode,
                    "MU_EDESC": muDesc
                });

                var inputUnitPerItem = $("#inputMaterialsUnit").data("kendoComboBox");
                if (inputUnitPerItem) {
                    inputUnitPerItem.dataSource.data($scope.InputItemUnitList);
                    inputUnitPerItem.dataSource.query();
                    inputUnitPerItem.value(muCode); // Set value by Code
                }
            }

        }
        $scope.inputDialogAction = "Update";
        $('#inputMaterialSetupModel').toggle();
    };

    $scope.editOutputMaterialSetup = function () {
        debugger;
        var outPutMatGrid = $("#outputMatGrid").data("kendoGrid");
        var selectedOutPut = outPutMatGrid.select();
        var dataRows = outPutMatGrid.items();
        $scope.OutputRowIndex = dataRows.index(selectedOutPut);
        if (selectedOutPut.length === 0) {
            alert('No record selected');
        } else {

            for (var i = 0; i < selectedOutPut.length; i++) {
                var dataItem = outPutMatGrid.dataItem(selectedOutPut[i]);
                var muCode = dataItem.MU_CODE;
                var muDesc = dataItem.MU_EDESC || muCode; // Fallback to Code if Description is missing

                $scope.outPutMaterialSetupObj.PROCESS = $scope.routineDetailsSetupObj.ROUTINE_NAME;    //dataItem.PROCESS_EDESC;
                $scope.outPutMaterialSetupObj.PROCESS_CODE = $scope.processCategoryRoutineObj.PROCESS_CODE;  //dataItem.PROCESS_CODE;
                $scope.outPutMaterialSetupObj.ITEM_NAME = dataItem.ITEM_NAME;
                $scope.outPutMaterialSetupObj.ITEM_CODE = dataItem.ITEM_CODE;
                $scope.outPutMaterialSetupObj.ITEM_EDESC = dataItem.ITEM_EDESC;
                $scope.outPutMaterialSetupObj.QUANTITY = dataItem.QUANTITY;
                $scope.outPutMaterialSetupObj.UNIT = muDesc;
                $scope.outPutMaterialSetupObj.UNIT_CODE = muCode;
                $scope.outPutMaterialSetupObj.MU_CODE = muCode;
                $scope.outPutMaterialSetupObj.VALUATION_FLAG = dataItem.VALUATION_FLAG === "Y" ? true : false;
                $scope.outPutMaterialSetupObj.OUTPUT = dataItem.OUT_PUT;
                $scope.outPutMaterialSetupObj.OUT_PUT = dataItem.OUT_PUT;
                $scope.outPutMaterialSetupObj.REMARKS = dataItem.REMARKS;

                var outputMatItemAutoComplete = $("#outPutMaterialsItem").data("kendoAutoComplete");
                if (outputMatItemAutoComplete) {
                    outputMatItemAutoComplete.value(dataItem.ITEM_EDESC);
                    outputMatItemAutoComplete.trigger("change");
                }

                $scope.OutputItemUnitList.push({
                    "MU_CODE": muCode,
                    "MU_EDESC": muDesc
                });

                var outputUnitPerItem = $("#outputMaterialsUnit").data("kendoComboBox");
                if (outputUnitPerItem) {
                    outputUnitPerItem.dataSource.data($scope.OutputItemUnitList);
                    outputUnitPerItem.dataSource.query();
                    outputUnitPerItem.value(muCode); // Set value explicitly using Code
                }
            }
        }
        $scope.outPutDialogAction = "Update";
        $('#outputMaterialSetupModel').toggle();
    };

    $scope.openNewOutputMaterialSetup = function () {
        debugger;
        $scope.outPutDialogAction = "Finish";
        var inputCount = $("#inputIndexItemGrid").data("kendoGrid").dataSource.total();
        if ($scope.IsInputCreated && inputCount > 0) {
            $scope.outPutMaterialSetupObj.PROCESS = $scope.processCategoryRoutineObj.IN_ENGLISH;
            $scope.outPutMaterialSetupObj.PROCESS_CODE = $scope.processCategoryRoutineObj.PROCESS_CODE;
            $('#outputMaterialSetupModel').toggle();
        } else {
            displayPopupNotification("No input materials are available to create output", "error");
            return;
        }

    };

    $scope.closeInputMaterialSetupModel = function () {
        $('#inputMaterialSetupModel').toggle();
    };

    $scope.closeOutputMaterialSetupModel = function () {
        $('#outputMaterialSetupModel').toggle();
    };

    $scope.InputIndexItemList = [];
    $scope.ProductionUnitList = [];
    $scope.TimeTakenUnitList = [];
    $scope.OutputIndexItemList = [];
    $scope.InputPeriodList = [];
    $scope.OutputPeriodList = [];
    $scope.IsInputCreated = false;
    $scope.SaveInputMaterialSetup = function (valueString) {

        debugger;
        $scope.inputMaterialSetupObj.MODAL_INFO = "INPUT";

        if ($scope.inputMaterialSetupObj.ITEM_CODE === "" || $scope.inputMaterialSetupObj.ITEM_CODE === undefined) {
            bootbox.alert("Please select an item.");
            return;
        }

        var billOfMatGrid = $("#billOfMatGrid").data("kendoGrid");
        if ($scope.inputDialogAction === "Finish") {
            var gridData = billOfMatGrid.dataSource.data();
            for (var i = 0; i < gridData.length; i++) {
                if (gridData[i].ITEM_CODE === $scope.inputMaterialSetupObj.ITEM_CODE) {
                    bootbox.alert("Item already exists in the list.");
                    return;
                }
            }
            var tempObj = {
                "MODAL_INFO": $scope.inputMaterialSetupObj.MODAL_INFO,
                "PROCESS_CODE": $scope.inputMaterialSetupObj.PROCESS_CODE,
                "PROCESS_EDESC": $scope.inputMaterialSetupObj.PROCESS,
                "QUANTITY": $scope.inputMaterialSetupObj.QUANTITY,
                "ITEM_CODE": $scope.inputMaterialSetupObj.ITEM_CODE,
                "ITEM_EDESC": $scope.inputMaterialSetupObj.ITEM_NAME,
                "MU_EDESC": $scope.inputMaterialSetupObj.UNIT,
                "MU_CODE": $scope.inputMaterialSetupObj.UNIT_CODE,
                "UNIT_CODE": $scope.inputMaterialSetupObj.UNIT_CODE,
                "REMARKS": $scope.inputMaterialSetupObj.REMARKS
            };
            billOfMatGrid.dataSource.add(tempObj);
            $("#btnSaveRoutineDetail").show();
        }
        else {
            var gridData = billOfMatGrid.dataSource.data();
            for (var i = 0; i < gridData.length; i++) {
                if (i !== $scope.InputRowIndex && gridData[i].ITEM_CODE === $scope.inputMaterialSetupObj.ITEM_CODE) {
                    bootbox.alert("Item already exists in the list.");
                    return;
                }
            }
            var inputMatItemAutoComplete = $("#inputMaterialsItem").data("kendoAutoComplete");
            var rowToUpdate = billOfMatGrid.dataSource.at($scope.InputRowIndex);
            rowToUpdate.set("MODAL_INFO", $scope.inputMaterialSetupObj.MODAL_INFO);
            rowToUpdate.set("PROCESS_CODE", $scope.inputMaterialSetupObj.PROCESS_CODE);
            rowToUpdate.set("PROCESS_EDESC", $scope.inputMaterialSetupObj.PROCESS);
            rowToUpdate.set("QUANTITY", $scope.inputMaterialSetupObj.QUANTITY);
            rowToUpdate.set("ITEM_EDESC", inputMatItemAutoComplete.value());
            rowToUpdate.set("ITEM_NAME", $scope.inputMaterialSetupObj.ITEM_NAME);
            rowToUpdate.set("ITEM_CODE", $scope.inputMaterialSetupObj.ITEM_CODE);
            rowToUpdate.set("MU_EDESC", $scope.inputMaterialSetupObj.UNIT);
            rowToUpdate.set("MU_CODE", $scope.inputMaterialSetupObj.UNIT_CODE);
            rowToUpdate.set("UNIT_CODE", $scope.inputMaterialSetupObj.UNIT_CODE);
            rowToUpdate.set("REMARKS", $scope.inputMaterialSetupObj.REMARKS);
        }

        var idex = $("#input_index_item").data('kendoComboBox');
        var prodUnit = $("#production_unit").data("kendoComboBox");

        if (idex.ITEM_CODE !== $scope.inputMaterialSetupObj.ITEM_CODE) {

            $scope.InputIndexItemList.push({
                "ITEM_CODE": $scope.inputMaterialSetupObj.ITEM_CODE,
                "ITEM_NAME": $scope.inputMaterialSetupObj.ITEM_NAME,
                "ITEM_EDESC": $scope.inputMaterialSetupObj.ITEM_EDESC
            });

            console.log("inserted Item======================>>>" + JSON.stringify($scope.InputIndexItemList));

            idex.dataSource.data($scope.InputIndexItemList);
            idex.dataSource.query();
            idex.select(0);
            $scope.routineDetailsSetupObj.INPUT_INDEX_ITEM = $scope.inputMaterialSetupObj.ITEM_CODE;

            var inputGrid = $("#inputIndexItemGrid").data("kendoGrid");
            inputGrid.dataSource.data($scope.InputIndexItemList);
            inputGrid.dataSource.query();
        }

        debugger;
        if (prodUnit.UNIT_CODE !== $scope.inputMaterialSetupObj.UNIT_CODE) {

            $scope.ProductionUnitList.push({
                "UNIT": $scope.inputMaterialSetupObj.UNIT
            });

            prodUnit.dataSource.data($scope.ProductionUnitList);
            prodUnit.dataSource.query();
            prodUnit.select(0);

            if ($scope.inputMaterialSetupObj.UNIT_CODE) {
                $scope.routineDetailsSetupObj.INPUT_UNIT = $scope.inputMaterialSetupObj.UNIT_CODE;
            }

            var prodUnitGrid = $("#productionUnitGrid").data("kendoGrid");
            prodUnitGrid.dataSource.data($scope.ProductionUnitList);
            prodUnitGrid.dataSource.query();
        }

        if (!valueString) {
            $('#inputMaterialSetupModel').toggle();
        }


        $scope.inputMaterialSetupObj = {};

        $("#inputMaterialsItem").data("kendoAutoComplete").value("");
        $("#inputMaterialsUnit").data("kendoComboBox").value("");

        $scope.IsInputCreated = true;

    };

    $scope.clearInputMaterialDialog = function () {

    };

    $scope.SaveOutputMaterialSetup = function () {

        $scope.outPutMaterialSetupObj.MODAL_INFO = "OUTPUT";

        if ($scope.outPutMaterialSetupObj.ITEM_CODE === "" || $scope.outPutMaterialSetupObj.ITEM_CODE === undefined) {
            bootbox.alert("Please select an item.");
            return;
        }

        var outputMatGrid = $("#outputMatGrid").data("kendoGrid");

        if ($scope.outPutDialogAction === 'Finish') {
            var gridData = outputMatGrid.dataSource.data();
            for (var i = 0; i < gridData.length; i++) {
                if (gridData[i].ITEM_CODE === $scope.outPutMaterialSetupObj.ITEM_CODE) {
                    bootbox.alert("Item already exists in the list.");
                    return;
                }
            }

            var tempObj = {
                "MODAL_INFO": $scope.outPutMaterialSetupObj.MODAL_INFO,
                "PROCESS_CODE": $scope.outPutMaterialSetupObj.PROCESS_CODE,
                "PROCESS_EDESC": $scope.outPutMaterialSetupObj.PROCESS,
                "QUANTITY": $scope.outPutMaterialSetupObj.QUANTITY,
                "ITEM_EDESC": $scope.outPutMaterialSetupObj.ITEM_NAME,
                "ITEM_CODE": $scope.outPutMaterialSetupObj.ITEM_CODE,
                "MU_EDESC": $scope.outPutMaterialSetupObj.UNIT,
                "MU_CODE": $scope.outPutMaterialSetupObj.UNIT_CODE,
                "UNIT_CODE": $scope.outPutMaterialSetupObj.UNIT_CODE,
                "REMARKS": $scope.outPutMaterialSetupObj.REMARKS,
                "OUT_PUT": $scope.outPutMaterialSetupObj.OUTPUT,
                "OUTPUT": $scope.outPutMaterialSetupObj.OUTPUT,
                "VALUATION_FLAG": $scope.outPutMaterialSetupObj.VALUATION_FLAG ? "Y" : "N"
            };
            outputMatGrid.dataSource.add(tempObj);

        }
        else {
            var gridData = outputMatGrid.dataSource.data();
            for (var i = 0; i < gridData.length; i++) {
                if (i !== $scope.OutputRowIndex && gridData[i].ITEM_CODE === $scope.outPutMaterialSetupObj.ITEM_CODE) {
                    bootbox.alert("Item already exists in the list.");
                    return;
                }
            }

            var outputItemAutoComplete = $("#outPutMaterialsItem").data("kendoAutoComplete");

            var rowToEdit = outputMatGrid.dataSource.at($scope.OutputRowIndex);
            //console.log("rowToEdit============>>" + JSON.stringify(rowToEdit));
            rowToEdit.set("MODAL_INFO", $scope.outPutMaterialSetupObj.MODAL_INFO);
            rowToEdit.set("PROCESS_CODE", $scope.outPutMaterialSetupObj.PROCESS_CODE);
            rowToEdit.set("PROCESS_EDESC", $scope.outPutMaterialSetupObj.PROCESS);
            rowToEdit.set("QUANTITY", $scope.outPutMaterialSetupObj.QUANTITY);
            rowToEdit.set("MU_EDESC", $scope.outPutMaterialSetupObj.UNIT);
            rowToEdit.set("MU_CODE", $scope.outPutMaterialSetupObj.UNIT_CODE);
            rowToEdit.set("UNIT_CODE", $scope.outPutMaterialSetupObj.UNIT_CODE);
            rowToEdit.set("OUT_PUT", $scope.outPutMaterialSetupObj.OUT_PUT);
            rowToEdit.set("OUT_PUT", $scope.outPutMaterialSetupObj.OUTPUT);
            rowToEdit.set("OUTPUT", $scope.outPutMaterialSetupObj.OUTPUT);
            rowToEdit.set("VALUATION_FLAG", $scope.outPutMaterialSetupObj.VALUATION_FLAG ? "Y" : "N");
            rowToEdit.set("ITEM_NAME", $scope.outPutMaterialSetupObj.ITEM_NAME);
            rowToEdit.set("ITEM_EDESC", outputItemAutoComplete.value());
            rowToEdit.set("ITEM_CODE", $scope.outPutMaterialSetupObj.ITEM_CODE);
            rowToEdit.set("REMARKS", $scope.outPutMaterialSetupObj.REMARKS);
        }

        var odex = $("#output_index_item").data('kendoComboBox');
        var timeTakenUnit = $("#time_taken_unit").data("kendoComboBox");


        if (odex.ITEM_CODE !== $scope.outPutMaterialSetupObj.ITEM_CODE) {
            console.log("odexItemCode====>>" + odex.ITEM_CODE);
            console.log("$scope.outPutMaterialSetupObj.ITEM_CODE====>>" + $scope.outPutMaterialSetupObj.ITEM_CODE);
            $scope.OutputIndexItemList.push({
                "ITEM_CODE": $scope.outPutMaterialSetupObj.ITEM_CODE,
                "ITEM_NAME": $scope.outPutMaterialSetupObj.ITEM_NAME,
                "ITEM_EDESC": $scope.outPutMaterialSetupObj.ITEM_EDESC
            });

            //Item comboBox
            odex.dataSource.data($scope.OutputIndexItemList);
            odex.dataSource.query();
            odex.select(0);
            $scope.routineDetailsSetupObj.OUTPUT_INDEX_ITEM = $scope.outPutMaterialSetupObj.ITEM_CODE;
            //Item Grid
            var inputGrid = $("#outPutIndexItemGrid").data("kendoGrid");
            inputGrid.dataSource.data($scope.OutputIndexItemList);
            inputGrid.dataSource.query();
        }

        if (timeTakenUnit.UNIT_CODE !== $scope.outPutMaterialSetupObj.UNIT_CODE) {

            $scope.TimeTakenUnitList.push({
                "UNIT": $scope.outPutMaterialSetupObj.UNIT,
                "UNIT_CODE": $scope.outPutMaterialSetupObj.UNIT_CODE
            });

            timeTakenUnit.dataSource.data($scope.TimeTakenUnitList);
            timeTakenUnit.dataSource.query();
            timeTakenUnit.select(0);

            $scope.routineDetailsSetupObj.OUTPUT_UNIT = $scope.outPutMaterialSetupObj.UNIT_CODE;

            var timeTakenUnitGrid = $("#timeTakenUnitGrid").data("kendoGrid");
            timeTakenUnitGrid.dataSource.data($scope.TimeTakenUnitList);
            timeTakenUnitGrid.dataSource.query();

        }

        $('#outputMaterialSetupModel').toggle();

        $scope.outPutMaterialSetupObj = {};


        $("#outPutMaterialsItem").data("kendoAutoComplete").value("");
        $("#outputMaterialsUnit").data("kendoComboBox").value("");

    };

    $scope.SaveRoutineDetailSetup = function () {
        debugger;

        var performDetailSave = function () {
            var saveModal = {
                "RoutineDetail": $scope.routineDetailsSetupObj,
                "InputModel": $("#billOfMatGrid").data().kendoGrid.dataSource.view(),
                "OutputModel": $("#outputMatGrid").data().kendoGrid.dataSource.view(),
                "PROCESS_FLAG": $scope.selectedProcessFlag
            };

            if (saveModal && saveModal.InputModel.length) {
                saveModal.InputModel.forEach(function (item, index, array) {
                    item.UNIT_CODE = item.MU_CODE;
                    item.MODAL_INFO = "INPUT";
                });
            }

            if (saveModal && saveModal.OutputModel.length) {
                saveModal.OutputModel.forEach(function (item, index, array) {
                    item.UNIT_CODE = item.MU_CODE;
                });
            }

            if ($scope.routineDetailsSetupObj.INPUT_IN_PERIOD == null || $scope.routineDetailsSetupObj.INPUT_IN_PERIOD == 'undefined' || $scope.routineDetailsSetupObj.INPUT_IN_PERIOD == undefined || $scope.routineDetailsSetupObj.INPUT_IN_PERIOD == "") {
                displayPopupNotification("Input period must be selected.", "warning");
                return;
            }
            if ($scope.routineDetailsSetupObj.OUTPUT_IN_PERIOD == null || $scope.routineDetailsSetupObj.OUTPUT_IN_PERIOD == 'undefined' || $scope.routineDetailsSetupObj.OUTPUT_IN_PERIOD == undefined || $scope.routineDetailsSetupObj.OUTPUT_IN_PERIOD == "") {
                displayPopupNotification("Output period must be selected.", "warning");
                return;
            }

            var routineResult = processSetupBomService.SaveRoutineDetailSetup(saveModal);
            routineResult.then(function (res) {
                if (res.data === "Successful") {
                    $scope.isCopyMode = false;
                    displayPopupNotification("Routine detail saved successfully ", "success");
                    setTimeout(function () {
                        $route.reload();
                    }, 3000);
                } else {
                    bootbox.alert(res.data);
                }
            }, function (error) {
                console.log('error', error);
                var errorMessage = "Error in saving routine detail";
                if (error.data) {
                    if (error.data.ExceptionMessage) {
                        errorMessage = error.data.ExceptionMessage;
                    } else if (error.data.Message) {
                        errorMessage = error.data.Message;
                    }
                } else if (error.statusText) {
                    errorMessage = error.statusText;
                }

                if (errorMessage && typeof errorMessage === 'string') {
                    errorMessage = errorMessage.replace(/\n/g, '<br/>');
                }
                bootbox.alert(errorMessage);
            });
        };

        if ($scope.isCopyMode || !$scope.IsCategoryProcessRoutineEdit) {

            var validationModel = {
                RoutineDetail: {
                    OUTPUT_INDEX_ITEM: $scope.routineDetailsSetupObj.OUTPUT_INDEX_ITEM,
                    ROUTINE_CODE: $scope.processCategoryRoutineObj.PROCESS_CODE
                },
                IsShowMsgOnly: true
            };
            processSetupBomService.ValidationIndexItemCode(validationModel).then(function (res) {


                // Save Master first for Copy
                var masterModel = {
                    "IN_ENGLISH": $scope.processCategoryRoutineObj.IN_ENGLISH,
                    "IN_NEPALI": $scope.processCategoryRoutineObj.IN_NEPALI,
                    "PROCESS_CODE": $scope.processCategoryRoutineObj.PROCESS_CODE,
                    "REMARK": $scope.processCategoryRoutineObj.REMARK,
                    "PROCESS_FLAG": $scope.selectedProcessFlag,
                    "PRIORITY_NUMBER": $scope.processCategoryRoutineObj.PRIORITY_NUMBER,
                    "PROCESS_TYPE": $scope.selectedProcessType,
                    "ROOT_UNDER": $scope.selectedProcessForDDL,
                    "LOCATION": $scope.selectedLocation,
                    "IS_EDIT": false
                };

                var saveMasterResult = processSetupBomService.SaveProcessCategorySetup(masterModel);
                saveMasterResult.then(function (res) {
                    if (res.data.MESSAGE === "Successful") {
                        // Update routine code in case it was changed by backend (though we set it)
                        $scope.routineDetailsSetupObj.ROUTINE_CODE = $scope.processCategoryRoutineObj.PROCESS_CODE;
                        performDetailSave();
                    } else {
                        bootbox.alert("Error saving master record for copy: " + res.data.MESSAGE);
                    }
                }, function (err) {
                    bootbox.alert("Failed to save master record for copy.");
                });




            }, function (error) {
                var msg = "Error validating index item code.";
                if (error.data) {
                    if (typeof error.data === 'string') {
                        msg = error.data;
                    } else if (error.data.ExceptionMessage) {
                        msg = error.data.ExceptionMessage;
                    } else if (error.data.Message) {
                        msg = error.data.Message;
                    }
                }

                if (msg && typeof msg === 'string') {
                    msg = msg.replace(/\n/g, '<br/>');
                }

                if (error.status === 400 || error.status === 500) {
                    bootbox.confirm({
                        message: msg,
                        buttons: {
                            confirm: {
                                label: 'Ok',
                                className: 'btn-success'
                            }
                            //cancel: {
                            //    label: 'Cancle',
                            //    className: 'btn-danger'
                            //}
                        },
                        callback: function (result) {
                            if (result) {
                                //alert('ok called');

                            } else {
                                //alert('cancelled called');

                            }
                        }
                    });
                } else {
                    bootbox.alert(msg);
                }
            });















        } else {
            performDetailSave();
        }
    };

    /* Input Material Setup Page End */

    //--------------------------------------------------------------------------------------------------------------------------------------------------------

    $scope.GetAssignedResourceList = function (type) {
        $scope.SelectedCategoryType = type;
        const promiseObj = processSetupBomService.GetAssignedResourceListWithParticularBOM($scope.processCategoryRoutineObj.PROCESS_CODE, type);
        promiseObj.then(function (res) {
            console.log(res);
            console.log("res:::: " + JSON.stringify(res));
            if (res && res['data']) {
                $scope.AssignedResourceList = res['data'];
            }
        });
    };


    $scope.newResourceMapping = function (value) {

        $scope.resetResourceAssignModel();
        if (value === 'edit') {
            console.log($scope.selectedAssignedResource);
            $scope.resourceAssignObj = {
                processCode: $scope.processCategoryRoutineObj.PROCESS_CODE,
                processName: $scope.routineDetailsSetupObj.ROUTINE_NAME, // Initial value from the image
                resourceName: $scope.selectedAssignedResource.RESOURCE_EDESC,
                resourceCode: $scope.selectedAssignedResource.RESOURCE_CODE,
                quantity: parseFloat($scope.selectedAssignedResource.QUANTITY) || 0, // Use null for number fields initially
                unit: $scope.selectedAssignedResource.MU_CODE,
                unitCode: $scope.selectedAssignedResource.MU_CODE,
                remarks: $scope.selectedAssignedResource.REMARKS,
                categoryType: $scope.SelectedCategoryType,

                StandardOutputQty: $scope.currentOutPutItem.QUANTITY, // Output item QTY
                StandardInputQty: $scope.selectedAssignedResource.STANDARD_INPUT_QTY,
            };
            $scope.isResourceEdit = true;
            $('#resourceAssignSetupModal').modal('show');

        } else {
            $scope.resourceAssignObj.processCode = $scope.processCategoryRoutineObj.PROCESS_CODE;
            $scope.resourceAssignObj.processName = $scope.routineDetailsSetupObj.ROUTINE_NAME;
            $scope.resourceAssignObj.StandardOutputQty = $scope.currentOutPutItem.QUANTITY;
            console.log($scope.resourceAssignObj.processName);
        }
    };

    $scope.isShowResourcesList = false;
    $scope.isShowUnitList = false;

    $scope.openResourceLookup = function () {
        $scope.filterUnitList = [];
        const promiseObj = processSetupBomService.GetAllResourceList($scope.SelectedCategoryType);
        promiseObj.then(function (res) {
            $scope.isShowResourcesList = true;
            $scope.isShowUnitList = false;
            if (res && res['data']) {
                $scope.AllResourceList = res['data'];
                $scope.filteredResources = $scope.AllResourceList;
                $('#resourceLookupModal').modal('show');
            }
        });
    };

    // Function to filter resources based on search text
    $scope.filterResources = function () {

        console.log($scope.filteredResources);


        if ($scope.isShowResourcesList) {
            $scope.filteredResources = $filter('filter')($scope.AllResourceList, function (resource) {
                // Case-insensitive search across description and remarks
                var lowerSearchText = $scope.searchText.toLowerCase();
                return (resource.RESOURCE_EDESC && resource.RESOURCE_EDESC.toLowerCase().includes(lowerSearchText)) ||
                    (resource.REMARKS && resource.REMARKS.toLowerCase().includes(lowerSearchText));
            });
        }
        if ($scope.isShowUnitList) {
            console.log('unit here');
            $scope.filterUnitList = $filter('filter')($scope.AllUnitList, function (resource) {
                // Case-insensitive search across description and remarks
                var lowerSearchText = $scope.searchText.toLowerCase();
                return (resource.MU_EDESC && resource.MU_EDESC.toLowerCase().includes(lowerSearchText)) ||
                    (resource.REMARKS && resource.REMARKS.toLowerCase().includes(lowerSearchText));
            });
        }
    };

    $scope.resourceOk = function () {
        if ($scope.selectedResource) {
            console.log('OK clicked. Selected resource:', $scope.selectedResource);
            // TODO: Pass the selected resource back to the calling component/parent scope
            // e.g., using a callback function or $emit/$broadcast
            // Example: $scope.$emit('resourceSelected', $scope.selectedResource);
            $('#resourceLookupModal').modal('hide'); // Close the Bootstrap modal
        } else {
            alert('Please select a resource first.');
        }
    };

    // Function to handle double-click row selection
    $scope.doubleClickSelectResource = function (item) {

        if ($scope.filterUnitList && $scope.filterUnitList.length) {
            $scope.selectedUnit = item; // Ensure it's selected on double-click too
            $scope.resourceAssignObj.unit = $scope.selectedUnit.MU_EDESC;
            $scope.resourceAssignObj.unitCode = $scope.selectedUnit.MU_CODE;
        }

        if ($scope.filteredResources && $scope.filteredResources.length) {
            $scope.selectedResource = item; // Ensure it's selected on double-click too
            $scope.resourceAssignObj.resourceName = $scope.selectedResource.RESOURCE_EDESC;
            $scope.resourceAssignObj.resourceCode = $scope.selectedResource.RESOURCE_CODE;
        }

        console.log('Selected resource (double click):', item);
        // Automatically trigger the OK action on double-click
        $scope.resourceOk();
    };


    $scope.openUnitLookup = function () {
        $scope.filteredResources = [];
        const promiseObj = processSetupBomService.GetAllUnitList();
        promiseObj.then(function (res) {

            $scope.isShowResourcesList = false;
            $scope.isShowUnitList = true;

            if (res && res['data']) {
                $scope.AllUnitList = res['data'];
                $scope.filterUnitList = $scope.AllUnitList;
                $('#resourceLookupModal').modal('show');
            }
        });
    };


    $scope.saveResourceMapping = function (form, value) {

        if (form.$invalid) {
            // Touch all fields to show validation messages
            angular.forEach(form.$error.required, function (field) {
                field.$setTouched();
            });
            //alert('called');
            return; // Block submission
        }


        $scope.resourceAssignObj.categoryType = $scope.SelectedCategoryType;
        const saveToObj = $scope.resourceAssignObj;

        const resultPromise = processSetupBomService.SaveResourceMapping(saveToObj);
        resultPromise.then(function (res) {
            $scope.GetAssignedResourceList($scope.resourceAssignObj.categoryType);
            if (value && value == 'closeModel' || $scope.isResourceEdit) {
                $('#resourceAssignSetupModal').modal('hide');
            }
        });
    };

    $scope.selectAssignedResource = function (assignedResource) {
        $scope.selectedAssignedResource = assignedResource;
    };
});

DTModule.service('processSetupBomService', function ($http) {

    this.getProcessType = function () {

        var processRes = $http({
            method: "GET",
            url: "/api/SetupApi/getProcessType",
            dataType: "json"
        });
        return processRes;
    };

    this.getAllCategoryProcessRoutineTree = function () {

        var cprTree = $http({
            method: "GET",
            url: "/api/ProcessSetupBomApi/GetAllProcessCategoryRoutine",
            dataType: "json"
        });
        return cprTree;
    };

    this.SaveProcessCategorySetup = function (parameter) {

        var processCatSetup = $http({
            method: "POST",
            url: "/api/ProcessSetupBomApi/SaveProcessCategoryRoutine",
            data: JSON.stringify(parameter),
            dataType: "json"
        });

        return processCatSetup;
    };

    this.SaveProcessSetup = function (saveParam) {
        var allProcessSetup = $http({
            method: "POST",
            url: "/api/ProcessSetupBomApi/SaveProcessSetup",
            data: JSON.stringify(saveParam),
            dataType: "JSON"
        });
        return allProcessSetup;
    };

    this.SaveInputOutMaterialSetup = function (inputOutputParam) {

        var materialResponse = $http({
            method: "POST",
            url: "/api/ProcessSetupBomApi/SaveInputOutMaterial",
            data: JSON.stringify(inputOutputParam),
            dataType: "JSON"
        });

        return materialResponse;

    };

    this.SaveRoutineDetailSetup = function (routineParam) {

        var routineResponse = $http({
            method: "POST",
            url: "/api/ProcessSetupBomApi/SaveRoutineDetailSetup",
            data: JSON.stringify(routineParam),
            dataType: "JSON"

        });

        return routineResponse;
    };

    this.ValidationIndexItemCode = function (routineParam) {
        var routineResponse = $http({
            method: "POST",
            url: "/api/ProcessSetupBomApi/ValidationIndexItemCode",
            data: JSON.stringify(routineParam),
            dataType: "JSON"
        });
        return routineResponse;
    };

    this.GetAllLocation = function () {
        var allLoc = $http({
            method: "GET",
            url: "/api/ProcessSetupBomApi/GetAllLocation",
            dataType: "JSON"
        });

        return allLoc;
    };

    this.GetAllItemForInputOutput = function () {
        var allItem = $http({
            method: "GET",
            url: "/api/ProcessSetupBomApi/GetAllItemForInputOutput",
            dataType: "JSON"
        });

        return allItem;
    };

    this.getProcessMuCode = function () {

    };

    this.GetChildProcessCode = function (processCode) {
        var cpCode = $http({
            method: "GET",
            url: "/api/ProcessSetupBomApi/GetChildProcessCode?processCode=" + processCode,
            dataType: "JSON"
        });

        return cpCode;
    };


    this.GetChildProcessDetail = function (processCode) {
        var cpDetail = $http({
            method: "GET",
            url: "/api/ProcessSetupBomApi/GetChildProcessDetail?processCode=" + processCode,
            dataType: "JSON"
        });
        return cpDetail;
    };

    this.GetInputRoutineDetail = function (processCode) {
        var inputRoutineDetail = $http({
            method: "GET",
            url: "/api/ProcessSetupBomApi/GetBillOfMaterialList?processCode=" + processCode,
            dataType: "JSON"
        });
        return inputRoutineDetail;
    };

    this.GetOutputRoutineDetail = function (processCode) {
        var outputRoutineDetail = $http({
            method: "GET",
            url: "/api/ProcessSetupBomApi/GetOutputMaterialList?processCode=" + processCode,
            dataType: "JSON"
        });
        return outputRoutineDetail;
    };





    // added by mahesh
    this.GetAssignedResourceListWithParticularBOM = function (processCode, type) {
        var assignResourceListPromise = $http({
            method: "GET",
            url: "/api/ProcessSetupBomApi/GetAssignedResounceWithParticularRoutine?processCode=" + processCode + '&categoryType=' + type,
            dataType: "JSON"
        });
        return assignResourceListPromise;
    };

    this.GetAllResourceList = function (type) {
        var resourceListPromise = $http({
            method: "GET",
            url: "/api/ProcessSetupBomApi/GetResourceList?resourceType=" + type,
            dataType: "JSON"
        });
        return resourceListPromise;
    };

    this.GetAllUnitList = function () {
        var unitListPromise = $http({
            method: "GET",
            url: "/api/ProcessSetupBomApi/GetUnitList",
            dataType: "JSON"
        });
        return unitListPromise;
    };

    this.SaveResourceMapping = function (parameter) {
        var promise = $http({
            method: "POST",
            url: "/api/ProcessSetupBomApi/SaveRoutineAndResourceMapping",
            data: JSON.stringify(parameter),
            dataType: "json"
        });
        return promise;
    };

    this.DeleteProcessSetup = function (processCode) {
        var promise = $http({
            method: "POST",
            url: "/api/ProcessSetupBomApi/DeleteProcessSetup?processCode=" + processCode,
            dataType: "json"
        });
        return promise;
    };

});