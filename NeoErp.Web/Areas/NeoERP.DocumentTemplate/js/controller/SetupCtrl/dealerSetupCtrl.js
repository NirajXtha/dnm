
DTModule.controller('dealerSetupCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter, hotkeys, $timeout, $compile) {
    $scope.result = false;
    $scope.phoneNumbr = /^\+?(\d+(\.(\d+)?)?|\.\d+)$/;
    $scope.saveupdatebtn = "save";
    $scope.dealerArr = [];
    $scope.savegroup = false;
    $scope.editFlag = "N";
    $scope.treeSelectedDealer = "";
    $scope.newrootinsertFlag = "Y";
    $scope.treenodeselected = "N";
    $scope.treeselectedmastercode = "";
    var DealerdataFillDefered = $.Deferred();
    $scope.dealersetup = {
        PARTY_TYPE_CODE: "",
        PARTY_TYPE_EDESC: "",
        GROUP_SKU_FLAG: "",
        PARTY_TYPE_NDESC: "",
        ACC_CODE: "",
        REMARKS: "",
        TEL_NO: "",
        CREDIT_LIMIT: "",
        CREDIT_DAYS: "",
        PAN_NO: "",
        TEL_NO2: "",
        ADDRESS: "",
        OWNER_NAME: "",
        LINK_BRANCH_CODE: "",
        AREA_CODE: "",
        ZONE_CODE: "",
        BG_AMOUNT: "",
        TERMS_CONDITIONS: "",
        APPROVED_FLAG: "",
        EXCEED_LIMIT_PERCENTAGE: "",
        TRADE_DISCOUNT: "",
        ANNUAL_BONUS: "",
        BG_PER_UNIT: "",
        CD_PER_UNIT: "",
        PDC_CHEQUE_AMT: "",
        SALES_TARGET: "",
        EMAIL: "",
        PRE_PARTY_CODE: "",
        MASTER_PARTY_CODE: "",
        PARTY_TYPE_FLAG: true,
    }
    $scope.dealerArr = $scope.dealersetup;
    $scope.l = function () {
        $http({
            method: 'GET',
            url: window.location.protocol + "//" + window.location.host + "/api/setupapi/GetPrefffromload",
        }).then(function successCallback(response) {
            debugger;
            $scope.items = response.data;
        }
        )
    }
    var getCustomerCodeByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetDealer";
    $scope.dealertreeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: getCustomerCodeByUrl,
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
                id: "masterDealerCode",
                parentId: "PRE_PARTY_CODE",
                children: "Items",
                fields: {
                    CUSTOMER_CODE: { field: "PARTY_TYPE_CODE", type: "string" },
                    CUSTOMER_EDESC: { field: "PARTY_TYPE_EDESC", type: "string" },
                    parentId: { field: "PRE_PARTY_CODE", type: "string", defaultValue: "00" },
                }
            }
        }
    });


    $scope.dealerOptions = {
        select: function (e) {
            debugger;
            var currentItem = e.sender.dataItem(e.node);
            $('#dealerGrid').removeClass("show-displaygrid");
            $("#dealerGrid").html("");
            $($(this._current).parents('ul')[$(this._current).parents('ul').length - 1]).find('span').removeClass('hasTreeCustomMenu');
            $(this._current.context).find('span').addClass('hasTreeCustomMenu');
            $scope.dealersetup.PARTY_TYPE_CODE = currentItem.dealerId;
            $scope.dealerArr.PARTY_TYPE_CODE = currentItem.dealerId;
            $scope.dealerArr.PARTY_TYPE_EDESC = currentItem.dealerName;
            $scope.dealersetup.MASTER_PARTY_CODE = currentItem.MASTER_PARTY_CODE;
            $scope.dealerArr.MASTER_PARTY_CODE = currentItem.MASTER_PARTY_CODE;
            $scope.treeSelectedDealer = currentItem.dealerId;
            $scope.dealersetup.PRE_PARTY_CODE = currentItem.preDealerCode;
            $scope.dealersetup.GROUP_SKU_FLAG = currentItem.groupSkuFlag;
            $scope.dealerArr.GROUP_SKU_FLAG = currentItem.groupSkuFlag;
            $scope.treeselectedmastercode = currentItem.masterDealerCode;
            $scope.dealerArr.masterDealerCode = currentItem.masterDealerCode;
            $scope.treenodeselected = "Y";
            $scope.newrootinsertFlag = "N";
            $scope.refresh();
            var grid = $("#kGrid2").data("kendo-grid");
            if (grid != undefined) {
                grid.dataSource.read();
            }
            //masterbranchcode=under group dropdown wala
            var tree = $("#dealerparentundergroup").data("kendoDropDownList");
            tree.value($scope.dealerArr.PARTY_TYPE_CODE);
        },
    };

    $scope.movescrollbar = function () {
        var element = $(".k-in");
        for (var i = 0; i < element.length; i++) {
            var selectnode = $(element[i]).hasClass("k-state-focused");
            if (selectnode) {
                $("#dealertree").animate({
                    scrollTop: (parseInt(i * 12))
                });
                break;
            }
        }
    }
    $scope.dealerDataBound = function () {
        $('#dealertree').data("kendoTreeView").expand('.k-item');
    }

    $scope.refresh = function () {

        var tree = $("#accounttree").data("kendoTreeView");
        tree.dataSource.read();
        var grid = $("#kGrid").data("kendo-grid");
        if (grid != undefined) {
            grid.dataSource.read();
        }
    }

    //Child grid
    $scope.BindGridDealer = function (groupCode) {
        debugger
        $scope.dealerChildGridOptions = {
            dataSource: {
                /*type: "json",*/
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                transport: {
                    read: "/api/SetupApi/GetChildOfDealerByGroup?groupCode=" + groupCode,
                },
                pageSize: 50,
                //serverPaging: true,
                serverSorting: true
            },
            scrollable: true,
            height: 450,
            sortable: true,
            pageable: true,
            //resizable: true,
            dataBound: function (e) {
                $("#kGrid tbody tr").css("cursor", "pointer");
                //DisplayNoResultsFound($('#kGrid'));
                $("#kGrid tbody tr").on('dblclick', function () {
                    var bcCode = $(this).find('td span').html()
                    $scope.edit(bcCode);

                })
            },
            columns: [
                //{
                //    //hidden: true,
                //    field: "BRANCH_CODE",
                //    title: "Code",
                //    width: "80px"

                //},
                {
                    field: "PARTY_TYPE_EDESC",
                    title: "Dealer Name",
                    width: "100px"
                },
                {
                    field: "CREDIT_DAYS",
                    title: "Credit Days",
                    width: "100px"
                },
                {
                    field: "REMARKS",
                    title: "Remarks",
                    width: "80px"
                },

                {

                    title: "Action ",
                    template: '<a class="fa fa-pencil-square-o editAction" title="Edit" ng-click="edit(dataItem.PARTY_TYPE_CODE)"><span class="sr-only"></span> </a><a class="fa fa-trash deleteAction" title="delete" ng-click="delete(dataItem.PARTY_TYPE_CODE)"><span class="sr-only"></span> </a>',
                    width: "40px"
                }
            ],


        };


    }
    //Add new Dealer in Root tree
    $scope.addGroupDealer = function () {
        $scope.dealersetup.MASTER_PARTY_CODE = "";
        $scope.editFlag = "N";
        $scope.clearFields();
        $("#createbtncustomer").attr('disabled', 'disabled');
        $scope.saveupdatebtn = "Save"
        $scope.savegroup = true;
        $scope.dealerArr = [];
        $scope.groupDealerTypeFlag = "Y";
        $scope.dealersetup.GROUP_SKU_FLAG = "G";
        $scope.dealerArr.GROUP_SKU_FLAG = "G";
        $scope.treeselectedmastercode = "";
        var tree = $("#dealerparentundergroup").data("kendoDropDownList");
        tree.value("");
        var tree = $("#dealertree").data("kendoTreeView");
        tree.dataSource.read();
        var grid = $("#kGrid").data("kendo-grid");
        if (grid != undefined) {
            grid.dataSource.data([]);
        }
        var listBox1 = $("#selectedselectOptions1").data("kendoListBox");
        if (listBox1 != undefined && listBox1 != "undefined") {
            listBox1.clearSelection();
        }
        var listBox2 = $("#selected").data("kendoListBox");
        if (listBox2 != undefined && listBox2 != "undefined") {
            listBox2.remove(listBox2.items());
        }
        $("#dealerModal").modal("toggle");

    }

    //undergroup dropdown 
    $scope.dealerArr.allCustomers = [];
    $scope.dealerArr.selectedCustomers = [];

    $scope.dealerGroupDataSource = [
        { text: "<PRIMARY>", value: "" }
    ];

    var dealerCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getDealerParent";

    $scope.dealerGroupDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: dealerCodeUrl,
            }
        }
    });

    $scope.dealerGroupOptions = {
        dataSource: $scope.dealerGroupDataSource,
        optionLabel: "<PRIMARY>",
        dataTextField: "PARTY_TYPE_EDESC",
        dataValueField: "PARTY_TYPE_CODE",
        change: function (e) {

            var currentItem = e.sender.dataItem(e.node);

        },
        dataBound: function (e) {
            $scope.branchGroupDataSource;

        }
    }

    //account
    var accountUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetAllAccountCode";
    $scope.accountsDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: accountUrl,
            }
        }
    });
    $scope.accountsOptions = {
        dataSource: $scope.accountsDataSource,
        optionLabel: "--Select Account--",
        dataTextField: "ACC_EDESC",
        dataValueField: "ACC_CODE",
        filter: "contains",
        change: function (e) {



        },
        dataBound: function () {
            ////
            //$scope.Bind();
        }
    };

    //dealer
    var branchUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getAllBranchs";
    $scope.branchDataSource = new kendo.data.DataSource({
        serverFiltering: true,
        transport: {
            read: {
                url: branchUrl,
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
    });
    $scope.branchOptions = {
        dataSource: $scope.branchDataSource,
        filter: "contains",
        optionLabel: "--Select Branch--",
        dataTextField: "BRANCH_EDESC",
        dataValueField: "BRANCH_CODE",
    }

    //Area
    var regionUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAreaCodeWithChild";
    $scope.regionDataSource = new kendo.data.DataSource({
        serverFiltering: true,
        transport: {
            read: {
                url: regionUrl,
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
    });
    $scope.regionOptions = {
        dataSource: $scope.regionDataSource,
        filter: "contains",
        optionLabel: "--Select Area--",
        dataTextField: "AREA_EDESC",
        dataValueField: "AREA_CODE",

    }
    //Zone
    var zoneUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getAllZones";
    $scope.zoneDataSource = new kendo.data.DataSource({
        serverFiltering: true,
        transport: {
            read: {
                url: zoneUrl,
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
    });
    $scope.zoneOptions = {
        dataSource: $scope.zoneDataSource,
        filter: "contains",
        optionLabel: "--Select Zone--",
        dataTextField: "ZONE_EDESC",
        dataValueField: "ZONE_CODE",
    }

    //Add new child
    $scope.AddChildDealer = function (event) {
        $scope.saveupdatebtn = "Save"
        $scope.editFlag = "N";
        $scope.dealerArr = [];
        $scope.clearFields();
        $scope.savegroup = false;
        $("#createbtncustomer").attr('disabled', 'disabled');
        $scope.groupDealerTypeFlag = "N";
        var grid2 = $("#kGrid2").data("kendo-grid");
        if (grid2 != undefined) {
            grid2.dataSource.data([]);
        }
        var returnMaxCustomerUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getDealerData?dealerCode=" + $scope.treeSelectedDealer;
        $http({
            method: 'GET',
            url: returnMaxCustomerUrl,
        }).then(function successCallback(response) {
            debugger;
            $scope.dealersetup = response.data.DATA;
            //$scope.dealerArr = response.data.DATA;
            $scope.dealerArr.masterDealerCode = $scope.dealersetup.MASTER_PARTY_CODE;
            $scope.dealerArr.PARTY_TYPE_CODE = $scope.dealersetup.PARTY_TYPE_CODE;
            $scope.dealerArr.MASTER_PARTY_CODE = $scope.dealersetup.MASTER_PARTY_CODE;
            $scope.dealerArr.PARTY_TYPE_EDESC = "";
            var tree = $("#dealerparentundergroup").data("kendoDropDownList");
            tree.value($scope.dealerArr.PARTY_TYPE_CODE);
            $scope.dealersetup.GROUP_SKU_FLAG = "I";
            $scope.dealerArr.GROUP_SKU_FLAG = "I";

            var listBox1 = $("#selectedselectOptions1").data("kendoListBox");
            if (listBox1 != undefined && listBox1 != "undefined") {
                listBox1.clearSelection();
            }
            var listBox2 = $("#selected").data("kendoListBox");
            if (listBox2 != undefined && listBox2 != "undefined") {
                listBox2.remove(listBox2.items());
            }

            $("#dealerModal").modal();


        }, function errorCallback(response) {
            displayPopupNotification(response.data.STATUS_CODE, "error");

        });
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

    //Context Menu here
    $scope.onContextSelect = function (event) {
        var grid2 = $("#kGrid2").data("kendo-grid");
        if (grid2 != undefined) {
            grid2.dataSource.data([]);
        }
        if ($scope.dealersetup.PARTY_TYPE_CODE == "")
            return displayPopupNotification("Select dealer.", "error");;
        $scope.saveupdatebtn = "Save";
        if (event.item.innerText.trim() == "Delete") {
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
                        var delUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteDealer?dealerCode=" + $scope.dealersetup.PARTY_TYPE_CODE;
                        $http({
                            method: 'POST',
                            url: delUrl
                        }).then(function successCallback(response) {
                            if (response.data.MESSAGE == "DELETED") {
                                $("#dealerModal").modal("hide");
                                $scope.treenodeselected = "N";
                                $scope.refresh();
                                bootbox.hideAll();
                                displayPopupNotification("Data succesfully deleted ", "success");
                            }
                            if (response.data.MESSAGE == "HAS_CHILD") {
                                displayPopupNotification("Please delete the respective child first", "warning");
                            }
                        }, function errorCallback(response) {
                            $scope.refresh();
                            displayPopupNotification(response.data.STATUS_CODE, "error");

                        });
                    }
                    else if (result == false) {

                        $scope.refresh();
                        $("#dealerModal").modal("hide");
                        bootbox.hideAll();
                    }
                }
            });
        }
        else if (event.item.innerText.trim() == "Update") {
            debugger;
            DealerdataFillDefered = $.Deferred();
            $scope.saveupdatebtn = "Update";
            $scope.fillDealerSetupForms($scope.dealersetup.PARTY_TYPE_CODE);
            $.when(DealerdataFillDefered).then(function () {
                if ($scope.dealersetup.PARENT_DEALER_CODE == null || $scope.dealersetup.PARENT_DEALER_CODE == undefined) {
                    var popUpDropdown = $("#dealerparentundergroup").data("kendoDropDownList");
                    popUpDropdown.value('');
                }
                else {
                    var popUpDropdown = $("#dealerparentundergroup").data("kendoDropDownList");
                    popUpDropdown.value($scope.dealersetup.PARENT_DEALER_CODE);
                }
                $("#dealerModal").modal("toggle");
            });

        }
        else if (event.item.innerText.trim() == "Add") {
            $("#createbtncustomer").attr('disabled', 'disabled');
            DealerdataFillDefered = $.Deferred();
            $scope.savegroup = true;
            $scope.dealerArr = [];
            $scope.fillDealerSetupForms($scope.dealersetup.PARTY_TYPE_CODE);
            $.when(DealerdataFillDefered).then(function () {
                //$scope.clearFields();
                var tree = $("#dealerparentundergroup").data("kendoDropDownList");
                tree.value($scope.dealerArr.PARTY_TYPE_CODE);
                $scope.dealersetup.GROUP_SKU_FLAG = "G";
                $scope.dealerArr.GROUP_SKU_FLAG = "G";
                $scope.dealerArr.PARTY_TYPE_EDESC = "";
                //$('#divisionname').val("");
                //$scope.clearFields();
                // $scope.clearFields();
                $("#dealerModal").modal("toggle");
            });
        }
    }

    //Get data
    $scope.fillDealerSetupForms = function (dealerCode) {
        var getDealerdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getDealerData?dealerCode=" + dealerCode;
        $http({
            method: 'GET',
            url: getDealerdetaisByUrl,

        }).then(function successCallback(response) {
            debugger;
            $scope.dealersetup = response.data.DATA;
            $scope.dealerArr = response.data.DATA;
            $scope.dealerArr.masterDealerCode = $scope.dealersetup.MASTER_PARTY_CODE;
            $scope.dealerArr.PARTY_TYPE_CODE = $scope.dealersetup.PARTY_TYPE_CODE;
            $scope.dealerArr.MASTER_PARTY_CODE = $scope.dealersetup.MASTER_PARTY_CODE;

            if ($scope.dealerArr.PARTY_TYPE_FLAG == "P") {
                $scope.dealerArr.PARTY_TYPE_FLAG = true;
            }
            else {
                $scope.dealerArr.PARTY_TYPE_FLAG = false;
            }
            if ($scope.dealerArr.GROUP_SKU_FLAG == "I") {
                $scope.dealerArr.GROUP_SKU_FLAG = "I";
            }
            else {
                $scope.dealerArr.GROUP_SKU_FLAG = "G";
            }

            if ($scope.dealerArr.INACTIVE_FLAG == "Y") {
                $scope.dealerArr.IS_ACTIVE = true;
            }
            else {
                $scope.dealerArr.IS_ACTIVE = false;
            }


            //if ($scope.dealersetup.PARTY_TYPE_FLAG == 'P') {
            //    $scope.dealerArr.PARTY_TYPE_FLAG = true;
            //    $scope.dealersetup.IS_CASH = true;
            //    $scope.dealersetup.IS_ACTIVE = true;
            //}
            //if ($scope.dealersetup.PARTY_TYPE_FLAG == 'D') {
            //    $scope.dealerArr.PARTY_TYPE_FLAG = false;
            //    $scope.dealersetup.IS_CASH = false;
            //    $scope.dealersetup.IS_ACTIVE = false;
            //}
            //if ($scope.dealersetup.GROUP_SKU_FLAG == 'I') {
            //    $scope.dealerArr.GROUP_SKU_FLAG = true;
            //}
            //else {
            //    $scope.dealerArr.GROUP_SKU_FLAG = false;
            //}

            //if ($scope.dealersetup.INACTIVE_FLAG == 'N') {
            //    $scope.dealerArr.INACTIVE_FLAG = true;
            //    $scope.dealersetup.IS_ACTIVE = true;
            //}
            //else {
            //    $scope.dealerArr.INACTIVE_FLAG = false;
            //    $scope.dealersetup.IS_ACTIVE = false;
            //}



            //$scope.dealerArr.IS_CASH = convertToBoolean($scope.dealersetup.IS_CASH);
            //$scope.dealerArr.IS_ACTIVE = convertToBoolean($scope.dealersetup.IS_ACTIVE);
            //To open the context menu and bind data in modal
            DealerdataFillDefered.resolve(response);
        }, function errorCallback(response) {

        });
    }

    //Edit Function
    $scope.edit = function (dealerCode) {
        debugger;
        var dealerlocalCode = dealerCode;
        $scope.editFlag = "Y";
        var grid = $("#kGrid2").data("kendo-grid");
        if (grid != undefined) {
            grid.dataSource.read();
        }
        $scope.savegroup = false;
        $("#createbtncustomer").removeAttr('disabled');
        $scope.groupDealerTypeFlag = "N";
        $scope.saveupdatebtn = "Update";
        DealerdataFillDefered = $.Deferred();
        $scope.fillDealerSetupForms(dealerCode);
        $.when(DealerdataFillDefered).done(function () {
            $scope.groupDealerTypeFlag = "N";
            $("#dealerModal").modal("toggle");
        });
        $scope.dealerChildGridOptions2 = {};
        $('#grid2').html('');
        var urlmapping = "/api/SetupApi/MappedDealerData?dealerCode=" + dealerlocalCode;
        //Grid 2
        $scope.dealerChildGridOptions2 = {
            dataSource: {
                /* type: "json",*/
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                transport: {
                    read: "/api/SetupApi/MappedDealerData?dealerCode=" + dealerCode,
                },
                pageSize: 7,
                serverSorting: true
            },
            scrollable: true,
            resizable: true,
            //height: auto,
            sortable: true,
            pageable: true,
            dataBound: function (e) {
                debugger;
                $("#kGrid2 tbody tr").css("cursor", "pointer");
                //DisplayNoResultsFound($('#kGrid'));
                $("#kGrid2 tbody tr").on('dblclick', function () {
                    var resourceCode = $(this).find('td span').html()
                    $scope.edit(resourceCode);
                    //var tree = $("#divisionRoottree").data("kendoTreeView");
                    //tree.dataSource.read();
                })
            },
            columns: [
                {
                    field: "CUSTOMER_CODE",
                    title: "Customer code",
                    width: "80px"
                },
                {
                    field: "CUSTOMER_EDESC",
                    title: "Customer Name",
                    width: "120px"
                },
                {
                    field: "TPIN_VAT_NO",
                    title: "Vat No.",
                    width: "120px"
                },
                {
                    field: "REGD_OFFICE_EADDRESS",
                    title: "Address",
                    width: "120px"
                },
            ],
        };
        var gridre = $("#Grid2").data("kendo-grid");
        if (gridre != undefined) {
            gridre.dataSource.read();
        }
    }

    //Delete function for child
    $scope.delete = function (code) {
        debugger
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
                    var delUrl =
                        window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteDealer?dealerCode=" + code;
                    $http({
                        method: 'POST',
                        url: delUrl
                    }).then(function successCallback(response) {
                        if (response.data.MESSAGE == "DELETED") {
                            debugger;
                            // $scope.budgetcenterArr = [];
                            if ($scope.dealersetup.GROUP_SKU_FLAG !== "I") {
                                var tree = $("#dealertree").data("kendoTreeView");
                                if (tree != undefined) {
                                    tree.dataSource.read();
                                }
                            }
                            $("#dealerModal").modal("hide");
                            var grid = $('#kGrid').data("kendoGrid");
                            grid.dataSource.read();
                            bootbox.hideAll();
                            $scope.treenodeselected = "N";
                            $scope.refresh();
                            $("#dealertree").data("kendoTreeView").dataSource.read();
                            $("#kGrid").data("kendoGrid").dataSource.read();
                            displayPopupNotification("Data succesfully deleted ", "success");
                        }
                        if (response.data.MESSAGE == "HAS_CHILD") {
                            displayPopupNotification("Please delete the respective child first", "warning");
                        }
                    }, function errorCallback(response) {
                        $scope.refresh();
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                    });
                }
                else if (result == false) {
                    $scope.refresh();
                    $("#dealerModal").modal("hide");
                    bootbox.hideAll();
                }
            }
        });
    }
    $scope.refresh = function () {
        $("#dealertree").data("kendoTreeView").dataSource.read();
    }



    $scope.resetFormValidation = function () {
        if ($scope.dealerform) {
            $scope.dealerform.$setPristine();
            $scope.dealerform.$setUntouched();
        }
    };

    //Save and Update Function
    $scope.saveNewDealer = function () {
        /* debugger;*/
        var masterdealervalue = $scope.treeselectedmastercode;

            if ($scope.dealerform.$invalid) {
            angular.forEach($scope.dealerform.$error.required, function (field) {
                field.$setTouched();
            });
            return;
        }
        //if (!isValid) {
        //    displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
        //    return;
        //}
        if ($scope.saveupdatebtn == "Save") {
            //var predivisioncode = $("#masterdivisioncode").data("kendoDropDownList").value();
            var createurl = window.location.protocol + "//" + window.location.host + "/api/setupapi/createNewDealer";
            var model = {
                PARTY_TYPE_CODE: $scope.dealerArr.PARTY_TYPE_CODE,
                PARTY_TYPE_EDESC: $scope.dealerArr.PARTY_TYPE_EDESC,
                PARTY_TYPE_NDESC: $scope.dealerArr.PARTY_TYPE_NDESC,
                ACC_CODE: $scope.dealerArr.ACC_CODE,
                REMARKS: $scope.dealerArr.REMARKS,
                TEL_NO: $scope.dealerArr.TEL_NO,
                GROUP_SKU_FLAG: $scope.dealerArr.GROUP_SKU_FLAG,
                PRE_PARTY_CODE: $scope.dealerArr.PRE_PARTY_CODE,
                MASTER_PARTY_CODE: $scope.dealerArr.MASTER_PARTY_CODE,
                CREDIT_LIMIT: $scope.dealerArr.CREDIT_LIMIT,
                CREDIT_DAYS: $scope.dealerArr.CREDIT_DAYS,
                PAN_NO: $scope.dealerArr.PAN_NO,
                TEL_NO2: $scope.dealerArr.TEL_NO2,
                ADDRESS: $scope.dealerArr.ADDRESS,
                OWNER_NAME: $scope.dealerArr.OWNER_NAME,
                LINK_BRANCH_CODE: $scope.dealerArr.LINK_BRANCH_CODE,
                AREA_CODE: $scope.dealerArr.AREA_CODE,
                ZONE_CODE: $scope.dealerArr.ZONE_CODE,
                BG_AMOUNT: $scope.dealerArr.BG_AMOUNT,
                TERMS_CONDITIONS: $scope.dealerArr.TERMS_CONDITIONS,
                APPROVED_FLAG: $scope.dealerArr.APPROVED_FLAG,
                EXCEED_LIMIT_PERCENTAGE: $scope.dealerArr.EXCEED_LIMIT_PERCENTAGE,
                TRADE_DISCOUNT: $scope.dealerArr.TRADE_DISCOUNT,
                ANNUAL_BONUS: $scope.dealerArr.ANNUAL_BONUS,
                BG_PER_UNIT: $scope.dealerArr.BG_PER_UNIT,
                CD_PER_UNIT: $scope.dealerArr.CD_PER_UNIT,
                PDC_CHEQUE_AMT: $scope.dealerArr.PDC_CHEQUE_AMT,
                SALES_TARGET: $scope.dealerArr.SALES_TARGET,
                PARTY_TYPE_FLAG: $scope.dealerArr.PARTY_TYPE_FLAG,
                EMAIL: $scope.dealerArr.EMAIL,
                INACTIVE_FLAG: $scope.dealerArr.IS_ACTIVE,
            }
            $http({
                method: 'post',
                url: createurl,
                data: model
            }).then(function successcallback(response) {
                if (response.data.MESSAGE == "INSERTED") {
                    $scope.clearFields();
                    $scope.dealerArr = [];
                    $scope.dealerArr.PARTY_TYPE_EDESC = "";
                    if ($scope.dealersetup.GROUP_SKU_FLAG !== "I") {
                        $scope.treeselectedmastercode = "";
                        $("#dealertree").data("kendoTreeView").dataSource.read();
                    }
                    if ($scope.savegroup == true) { $("#dealerModal").modal("toggle"); }
                    else { $("#dealerModal").modal("toggle"); }
                    var grid = $("#kGrid").data("kendoGrid");
                    if (grid != undefined) {
                        grid.dataSource.read();
                    }
                    $("#dealerparentundergroup").data("kendoDropDownList").dataSource.read();
                    var listBox1 = $("#selectedselectOptions1").data("kendoListBox");
                    if (listBox1 != undefined && listBox1 != "undefined") {
                        listBox1.clearSelection();
                    }
                    var listBox2 = $("#selected").data("kendoListBox");
                    if (listBox2 != undefined && listBox2 != "undefined") {
                        listBox2.remove(listBox2.items());
                    }
                    displayPopupNotification("data succesfully saved ", "success");
                    debugger;
                    $scope.treenodeselected = "N";
                    $scope.refresh();
                    var grid2 = $("#kGrid2").data("kendo-grid");
                    if (grid2 != undefined) {
                        grid2.dataSource.data([]);
                    }
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Dealer name already exist please try another Dealer name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            }, function errorcallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Dealer name already exist please try another Dealer name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            });
        }
        else {

            var updateurl = window.location.protocol + "//" + window.location.host + "/api/setupapi/updateDealerCode";
            $scope.saveupdatebtn = "Update";
            var model = {
                PARTY_TYPE_CODE: $scope.dealerArr.PARTY_TYPE_CODE,
                PARTY_TYPE_EDESC: $scope.dealerArr.PARTY_TYPE_EDESC,
                PARTY_TYPE_NDESC: $scope.dealerArr.PARTY_TYPE_NDESC,
                ACC_CODE: $scope.dealerArr.ACC_CODE,
                REMARKS: $scope.dealerArr.REMARKS,
                TEL_NO: $scope.dealerArr.TEL_NO,
                GROUP_SKU_FLAG: $scope.dealerArr.GROUP_SKU_FLAG,
                PRE_PARTY_CODE: $scope.dealerArr.PRE_PARTY_CODE,
                MASTER_PARTY_CODE: $scope.dealerArr.MASTER_PARTY_CODE,
                CREDIT_LIMIT: $scope.dealerArr.CREDIT_LIMIT,
                CREDIT_DAYS: $scope.dealerArr.CREDIT_DAYS,
                PAN_NO: $scope.dealerArr.PAN_NO,
                TEL_NO2: $scope.dealerArr.TEL_NO2,
                ADDRESS: $scope.dealerArr.ADDRESS,
                OWNER_NAME: $scope.dealerArr.OWNER_NAME,
                LINK_BRANCH_CODE: $scope.dealerArr.LINK_BRANCH_CODE,
                AREA_CODE: $scope.dealerArr.AREA_CODE,
                ZONE_CODE: $scope.dealerArr.ZONE_CODE,
                BG_AMOUNT: $scope.dealerArr.BG_AMOUNT,
                TERMS_CONDITIONS: $scope.dealerArr.TERMS_CONDITIONS,
                APPROVED_FLAG: $scope.dealerArr.APPROVED_FLAG,
                EXCEED_LIMIT_PERCENTAGE: $scope.dealerArr.EXCEED_LIMIT_PERCENTAGE,
                TRADE_DISCOUNT: $scope.dealerArr.TRADE_DISCOUNT,
                ANNUAL_BONUS: $scope.dealerArr.ANNUAL_BONUS,
                BG_PER_UNIT: $scope.dealerArr.BG_PER_UNIT,
                CD_PER_UNIT: $scope.dealerArr.CD_PER_UNIT,
                PDC_CHEQUE_AMT: $scope.dealerArr.PDC_CHEQUE_AMT,
                SALES_TARGET: $scope.dealerArr.SALES_TARGET,
                EMAIL: $scope.dealerArr.EMAIL,
                PARTY_TYPE_FLAG: $scope.dealerArr.PARTY_TYPE_FLAG,
                INACTIVE_FLAG: $scope.dealerArr.IS_ACTIVE,
            }
            $http({
                method: 'post',
                url: updateurl,
                data: model
            }).then(function successcallback(response) {
                if (response.data.MESSAGE == "UPDATED") {
                    $scope.clearFields();
                    $scope.dealerArr = [];
                    if ($scope.dealersetup.GROUP_SKU_FLAG !== "I") {
                        $scope.treeselectedmastercode = "";
                        $("#dealertree").data("kendoTreeView").dataSource.read();
                    }
                    var grid = $("#kGrid").data("kendoGrid");
                    if (grid != undefined) {
                        grid.dataSource.read();
                    }
                    $("#dealerparentundergroup").data("kendoDropDownList").dataSource.read();
                    $("#dealerModal").modal("toggle");
                    $scope.refresh();
                    var listBox1 = $("#selectedselectOptions1").data("kendoListBox");
                    if (listBox1 != undefined && listBox1 != "undefined") {
                        listBox1.clearSelection();
                    }
                    var listBox2 = $("#selected").data("kendoListBox");
                    if (listBox2 != undefined && listBox2 != "undefined") {
                        listBox2.remove(listBox2.items());
                    }
                    displayPopupNotification("data succesfully updated ", "success");
                    $scope.treenodeselected = "N";
                }
                if (response.data.MESSAGE == "error") {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            }, function errorcallback(response) {
                displayPopupNotification("Something went wrong.Please try again later.", "error");
            });
        }
        $scope.refresh();
    }
    $scope.searchtest = "";
    $scope.searchcustomerlist = function () {
        debugger;
        var listBox = $("#selectedselectOptions1").getKendoListBox();
        var sarchString = $scope.searchtest;
        listBox.dataSource.filter({ field: "CUSTOMER_EDESC", operator: "contains", value: sarchString });
    }

    $scope.toggleTreeDropdown = function () {
        debugger
        $scope.showTreeDropdown = !$scope.showTreeDropdown;
    };
    //For customer Mpping
    $scope.showModalForCustomer = function (event) {
        $scope.saveupdatebtn = "Save"
        var grid = $("#kGrid2").data("kendo-grid");
        if (grid != undefined) {
            grid.dataSource.read();
        }
        $("#CustomermapModal").modal("toggle");
        debugger;
        $("#CustomerGrid").data("kendoGrid").dataSource.data([]);
        $("#SelectedCustomerGrid").data("kendoGrid").dataSource.data([]);
    }
    $scope.selectOptions1 = {
        dataSource: $scope.regionDataSource123,
        filter: "contains",
        dataTextField: "CUSTOMER_EDESC",
        dataValueField: "CUSTOMER_CODE",
        connectWith: "selected",
        filterable: true,
        toolbar: {
            position: "right",
            tools: ["transferTo", "transferFrom", "transferAllTo", "transferAllFrom"]
        },
        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetChildOfCustomerByGroup1?dealercode=" + $scope.dealerArr.PARTY_TYPE_CODE,
                    /*dataType: "json"*/
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                }
            }
        }
    };
    $scope.selectOptions2 = {
        dataTextField: "CUSTOMER_EDESC",
        dataValueField: "CUSTOMER_CODE",
        filterable: true,
    };

    var getCustomerGroupMappingByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetCustomers";

    $scope.showTreeDropdown = false;

    // Close dropdown on outside click
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.dropdown-tree-container').length) {
            $scope.$apply(function () {
                $scope.showTreeDropdown = false;
            });
        }
    });

    // Tree Data Source
    $scope.customertreeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: getCustomerGroupMappingByUrl,
                type: 'GET',
                data: function () { }
            },
        },
        schema: {
            parse: function (data) {
                $scope.customertreeData._pristineData = deepClone(data);
                return data;
            },
            model: {
                id: "masterCustomerCode",
                parentId: "preCustomerCode",
                children: "Items",
                hasChildren: function (item) {
                    return item.Items && item.Items.length > 0;
                },
                fields: {
                    CUSTOMER_CODE: { field: "CUSTOMER_CODE", type: "string" },
                    CUSTOMER_EDESC: { field: "CUSTOMER_EDESC", type: "string" },
                    parentId: { field: "preCustomerCode", type: "string", defaultValue: "00" },
                }
            }
        }
    });

    // TreeView Options
    $scope.customerOptions = {
        loadOnDemand: true,
        select: function (e) {
            debugger
            var currentItem = e.sender.dataItem(e.node);
            $scope.txtMainGroupSearchString = currentItem.CUSTOMER_EDESC || currentItem.customerName
            $scope.treenodeselected = "Y";
            /*  $('#accountGrid').removeClass("show-displaygrid").html("");*/

            $($(this._current).parents('ul')[$(this._current).parents('ul').length - 1]).find('span').removeClass('hasTreeCustomMenu');
            $(this._current.context).find('span').addClass('hasTreeCustomMenu');

            $scope.customersetup = [];
            $scope.customersetup.CUSTOMER_CODE = currentItem.customerId;
            $scope.customersetup.CUSTOMER_EDESC = currentItem.customerName;
            $scope.customersetup.MASTER_CUSTOMER_CODE = currentItem.masterCustomerCode;
            $scope.customersetup.PRE_CUSTOMER_CODE = currentItem.preCustomerCode;
            $scope.customersetup.GROUP_SKU_FLAG = currentItem.groupSkuFlag;
            $scope.customersetup.CUSTOMER_ACCOUNT = currentItem.ACC_CODE;
            $scope.customersetup.CUSTOMER_TYPE = currentItem.CUSTOMER_FLAG;
            $scope.customersetup.CUSTOMER_NDESC = currentItem.CUSTOMER_NDESC;
            $scope.customersetup.CUSTOMER_PREFIX = currentItem.CUSTOMER_PREFIX;
            $scope.customersetup.CUSTOMER_STARTID = currentItem.CUSTOMER_STARTID;
            $scope.customersetup.REMARKS = currentItem.REMARKS;
            $scope.customersetup.PARENT_CUSTOMER_CODE = currentItem.PARENT_CUSTOMER_CODE;

            $scope.treeSelectedCustomerCode = currentItem.customerId;
            $scope.treeSelectedCustomerMasterCode = currentItem.masterCustomerCode || currentItem.MASTER_CUSTOMER_CODE;
            $scope.newbuttondisabled = false;

            $scope.showTreeDropdown = false;
            $scope.BindCustomerGrid();
            $scope.$apply();
        }
    };

    $scope.customeronDataBound = function () {
        // $('#customertree').data("kendoTreeView").expand('.k-item');
    };

    $scope.loadPreviouslySelectedCustomers = function () {
        var url = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getCustomerMapped?partyTypeCode=" + $scope.dealerArr.PARTY_TYPE_CODE;

        $http.get(url).then(function (response) {
            debugger
            if (response.data && Array.isArray(response.data)) {
                $scope.dealerArr.selectedCustomers = angular.copy(response.data);

                $("#SelectedCustomerGrid").data("kendoGrid").setDataSource(
                    new kendo.data.DataSource({
                        data: $scope.dealerArr.selectedCustomers,
                        schema: {
                            model: {
                                id: "CUSTOMER_CODE",
                                fields: {
                                    CUSTOMER_EDESC: { type: "string" },
                                    PAN_NO: { type: "string" },
                                    CUSTOMER_ID: { type: "string" }
                                }
                            }
                        },
                        pageSize: 50
                    })
                );
            }
        });
    };

    $scope.loadPreviouslySelectedCustomers();
    
    $scope.BindCustomerGrid = function (isInitialLoad = false) {
        debugger
        let filter = isInitialLoad ? "_initial_load" : $scope.searchQuery;
        let url = "/api/SetupApi/GetCustomerMappingForDealer?preCustomerCode=" + ($scope.treeSelectedCustomerMasterCode || "") + "&partyTypeCode=" + ($scope.dealerArr.PARTY_TYPE_CODE || "") + "&filter=" + (filter || "");

        $.getJSON(url, function (data) {
            $scope.dealerArr.allCustomers = data;
            $scope.allCustomers = $scope.dealerArr.allCustomers;
            $scope.updateCustomerGrids();
            $scope.$apply();
        });
    };

  /*  $scope.BindCustomerGrid();*/

    $scope.updateCustomerGrids = function () {
        const selectedIds = $scope.dealerArr.selectedCustomers.map(c => c.CUSTOMER_CODE);

        const unselectedCustomers = $scope.dealerArr.allCustomers.filter(c => !selectedIds.includes(c.CUSTOMER_CODE));

        $("#CustomerGrid").data("kendoGrid").setDataSource(new kendo.data.DataSource({
            data: unselectedCustomers,
            schema: {
                model: {
                    id: "CUSTOMER_CODE",
                    fields: {
                        CUSTOMER_EDESC: { type: "string" },
                        PAN_NO: { type: "string" },
                        CUSTOMER_ID: { type: "string" }
                    }
                }
            },
            pageSize: 50
        }));

        $("#SelectedCustomerGrid").data("kendoGrid").setDataSource(new kendo.data.DataSource({
            data: $scope.dealerArr.selectedCustomers,
            schema: {
                model: {
                    id: "CUSTOMER_CODE",
                    fields: {
                        CUSTOMER_EDESC: { type: "string" },
                        PAN_NO: { type: "string" },
                        CUSTOMER_ID: { type: "string" }
                    }
                }
            },
            pageSize: 50
        }));
    };

    $scope.selectionMap = {
        main: {},
        selected: {}
    };

    $scope.addSelectedCustomers = function () {
        const selectedCodes = Object.keys($scope.selectionMap.main).filter(k => $scope.selectionMap.main[k]);

        const selectedGridData = getGridData("SelectedCustomerGrid");
        const mainGridData = getGridData("CustomerGrid");

        $scope.dealerArr.selectedCustomers = selectedGridData;

        let allCustomers = [...mainGridData, ...selectedGridData];
        $scope.dealerArr.allCustomers = allCustomers;

        $scope.dealerArr.allCustomers.forEach(item => {
            if (selectedCodes.includes(item.CUSTOMER_CODE) &&
                !$scope.dealerArr.selectedCustomers.some(c => c.CUSTOMER_CODE === item.CUSTOMER_CODE)) {
                $scope.dealerArr.selectedCustomers.push(angular.copy(item));
            }
        });

        $scope.selectionMap.main = {};
        $scope.updateCustomerGrids();
        $('#CustomerGrid thead input[type="checkbox"]').prop('checked', false);
        $('#SelectedCustomerGrid thead input[type="checkbox"]').prop('checked', false);
    };

    $scope.removeSelectedCustomers = function () {
        const selectedCodes = Object.keys($scope.selectionMap.selected).filter(k => $scope.selectionMap.selected[k]);

        let unselectedCustomers = $scope.dealerArr.selectedCustomers.filter(c => selectedCodes.includes(c.CUSTOMER_CODE));
        $scope.dealerArr.selectedCustomers = $scope.dealerArr.selectedCustomers.filter(c => !selectedCodes.includes(c.CUSTOMER_CODE));
        $scope.selectionMap.selected = {};
        updateCustomerGridsOnRemove(unselectedCustomers);
        $('#CustomerGrid thead input[type="checkbox"]').prop('checked', false);
        $('#SelectedCustomerGrid thead input[type="checkbox"]').prop('checked', false);
    };

    function getGridData(gridId) {
        const grid = $(`#${gridId}`).data("kendoGrid");
        return grid ? grid.dataSource.data().toJSON() : [];
    }

    function updateCustomerGridsOnRemove(unselectedCustomers) {
        //const selectedGrid = $("#SelectedCustomerGrid").data("kendoGrid");
        //const mainGrid = $("#CustomerGrid").data("kendoGrid");

        //const checkedCustomerCodes = [];
        //$("#SelectedCustomerGrid tbody input[type='checkbox']:checked").each(function () {
        //    const row = $(this).closest("tr");
        //    const dataItem = selectedGrid.dataItem(row);
        //    if (dataItem) {
        //        checkedCustomerCodes.push(dataItem.CUSTOMER_CODE);
        //    }
        //});

        //const selectedGridData = selectedGrid
        //    ? selectedGrid.dataSource.data().filter(item => checkedCustomerCodes.includes(item.CUSTOMER_CODE))
        //    : [];

        //const mainGridData = mainGrid ? mainGrid.dataSource.data() : [];

        //const allCustomers = [...mainGridData, ...selectedGridData];

        //const unselectedCustomers = allCustomers.filter(c => selectionMap.includes(c.CUSTOMER_CODE));

        $("#CustomerGrid").data("kendoGrid").setDataSource(new kendo.data.DataSource({
            data: unselectedCustomers,
            schema: {
                model: {
                    id: "CUSTOMER_CODE",
                    fields: {
                        CUSTOMER_EDESC: { type: "string" },
                        PAN_NO: { type: "string" },
                        CUSTOMER_ID: { type: "string" }
                    }
                }
            },
            pageSize: 50
        }));

        $("#SelectedCustomerGrid").data("kendoGrid").setDataSource(new kendo.data.DataSource({
            data: $scope.dealerArr.selectedCustomers,
            schema: {
                model: {
                    id: "CUSTOMER_CODE",
                    fields: {
                        CUSTOMER_EDESC: { type: "string" },
                        PAN_NO: { type: "string" },
                        CUSTOMER_ID: { type: "string" }
                    }
                }
            },
            pageSize: 50
        }));
    }

    function refreshGrids() {
        const selectedGridData = getGridData("SelectedCustomerGrid");
        const mainGridData = getGridData("CustomerGrid");

        $scope.dealerArr.selectedCustomers = selectedGridData;

        const allCustomers = [...mainGridData, ...selectedGridData];
        const selectedIds = new Set(selectedGridData.map(c => c.CUSTOMER_CODE));

        const unselectedCustomers = allCustomers.filter(c => !selectedIds.has(c.CUSTOMER_CODE));

        $("#CustomerGrid").data("kendoGrid").setDataSource(new kendo.data.DataSource({
            data: unselectedCustomers,
            schema: {
                model: {
                    id: "CUSTOMER_CODE",
                    fields: {
                        CUSTOMER_EDESC: { type: "string" },
                        PAN_NO: { type: "string" },
                        CUSTOMER_ID: { type: "string" }
                    }
                }
            },
            pageSize: 50
        }));

        $("#SelectedCustomerGrid").data("kendoGrid").setDataSource(new kendo.data.DataSource({
            data: selectedGridData,
            schema: {
                model: {
                    id: "CUSTOMER_CODE",
                    fields: {
                        CUSTOMER_EDESC: { type: "string" },
                        PAN_NO: { type: "string" },
                        CUSTOMER_ID: { type: "string" }
                    }
                }
            },
            pageSize: 50
        }));
    }

    $("#CustomerGrid").on("dblclick", "tbody > tr", function () {
        debugger
        const grid = $("#CustomerGrid").data("kendoGrid");
        const dataItem = grid.dataItem(this);
        if (!dataItem) return;

        const selectedGridData = getGridData("SelectedCustomerGrid");
        const exists = selectedGridData.some(c => c.CUSTOMER_CODE === dataItem.CUSTOMER_CODE);
        if (!exists) selectedGridData.push(angular.copy(dataItem));

        $scope.selectionMap.main[dataItem.CUSTOMER_CODE] = false;

        $("#SelectedCustomerGrid").data("kendoGrid").dataSource.data(selectedGridData);
        refreshGrids();
        $scope.$applyAsync();
    });

    $("#SelectedCustomerGrid").on("dblclick", "tbody > tr", function () {
        const selectedGrid = $("#SelectedCustomerGrid").data("kendoGrid");
        const dataItem = selectedGrid.dataItem(this);
        if (!dataItem) return;

        const selectedGridData = getGridData("SelectedCustomerGrid").filter(
            c => c.CUSTOMER_CODE !== dataItem.CUSTOMER_CODE
        );

        const mainGridData = getGridData("CustomerGrid");
        const alreadyInMain = mainGridData.some(c => c.CUSTOMER_CODE === dataItem.CUSTOMER_CODE);

        if (!alreadyInMain) {
            mainGridData.unshift(angular.copy(dataItem));
        }

        $scope.selectionMap.selected[dataItem.CUSTOMER_CODE] = false;

        $("#SelectedCustomerGrid").data("kendoGrid").dataSource.data(selectedGridData);
        $("#CustomerGrid").data("kendoGrid").dataSource.data(mainGridData);

        refreshGrids();
        $scope.$applyAsync();
    });


    $scope.onRowCheckboxChange = function (gridKey) {
        const grid = gridKey === 'main' ? $("#CustomerGrid").data("kendoGrid") : $("#SelectedCustomerGrid").data("kendoGrid");
        const map = $scope.selectionMap[gridKey];
        const data = grid.dataSource.data();

        const allChecked = data.length && data.every(item => map[item.CUSTOMER_CODE]);
        const someChecked = data.some(item => map[item.CUSTOMER_CODE]);

        const headerCheckboxId = gridKey === 'main' ? '#mainGridHeaderChk' : '#selectedGridHeaderChk';
        const headerCheckbox = $(headerCheckboxId)[0];

        if (headerCheckbox) {
            headerCheckbox.checked = allChecked;
            headerCheckbox.indeterminate = !allChecked && someChecked;
        }
    };

    $scope.toggleRowSelection = function (item, gridKey) {
        const map = $scope.selectionMap[gridKey];   
        map[item.CUSTOMER_CODE] = !map[item.CUSTOMER_CODE];
    };

    $scope.toggleAll = function ($event, gridKey) {
        const checked = $event.target.checked;
        const grid = gridKey === 'main' ? $("#CustomerGrid").data("kendoGrid") : $("#SelectedCustomerGrid").data("kendoGrid");
        if (!grid) return;

        const map = $scope.selectionMap[gridKey];
        grid.dataSource.data().forEach(item => {
            map[item.CUSTOMER_CODE] = checked;
        });

        //$scope.updateHeaderCheckboxState(gridKey);
       
        $scope.$applyAsync();
    };

    $scope.customerChildGridOptions = {
        sortable: true,
        pageable: true,
        filterable: true,
        resizable: true,
        scrollable: true,
        height: 500,
        selectable: "multiple, row",
        columns: [
            {
                headerTemplate: `<input type="checkbox" ng-click="toggleAll($event, 'main')" id="mainGridHeaderChk"/>`,
                template: `<input type="checkbox" ng-checked="selectionMap['main'][dataItem.CUSTOMER_CODE]"
                               ng-click="selectionMap['main'][dataItem.CUSTOMER_CODE] = !selectionMap['main'][dataItem.CUSTOMER_CODE]; onRowCheckboxChange('main')" />`,
                width: 50
            },
            { field: "CUSTOMER_EDESC", title: "Name", width: "250px" },
            { field: "PAN_NO", title: "PAN No", width: "100px" },
            { field: "CUSTOMER_ID", title: "User ID", width: "100px" }
        ]
    };


    $scope.selectedCustomerGridOptions = {
        sortable: true,
        pageable: true,
        filterable: true,
        resizable: true,
        scrollable: true,
        height: 500,
        selectable: "multiple, row",
        columns: [
            {
                headerTemplate: `<input type="checkbox" ng-click="toggleAll($event, 'selected')" id="selectedGridHeaderChk"/>`,
                template: `<input type="checkbox" ng-checked="selectionMap['selected'][dataItem.CUSTOMER_CODE]"
                               ng-click="selectionMap['selected'][dataItem.CUSTOMER_CODE] = !selectionMap['selected'][dataItem.CUSTOMER_CODE]; onRowCheckboxChange('selected')" />`,
                width: 50
            },
            { field: "CUSTOMER_EDESC", title: "Name", width: "250px" },
            { field: "PAN_NO", title: "PAN No", width: "100px" },
            { field: "CUSTOMER_ID", title: "User ID", width: "100px" }
        ]
    };


    $scope.searchQuery = "";

    $scope.filterGrid = function () {
        $scope.filterCustomerGrid();
    };

    $scope.searchSelectedCustomerQuery = "";

    $scope.filterSelectedCustomerGrid = function () {
        let grid = $("#SelectedCustomerGrid").data("kendoGrid");
        if (grid) {
            let dataSource = grid.dataSource;
            if ($scope.searchQuery) {
                dataSource.filter({
                    field: "CUSTOMER_EDESC",
                    operator: "contains",
                    value: $scope.searchQuery
                });
            } else {
                dataSource.filter({});
            }
        }
    };


    $scope.searchSelectedCustomerQuery = "";

    $scope.filterSelectedCustomerGrid = function () {
        let grid = $("#SelectedCustomerGrid").data("kendoGrid");
        if (grid) {
            let dataSource = grid.dataSource;
            if ($scope.searchSelectedCustomerQuery) {
                dataSource.filter({
                    field: "CUSTOMER_EDESC",
                    operator: "contains",
                    value: $scope.searchSelectedCustomerQuery
                });
            } else {
                dataSource.filter({});
            }
        }
    };

    $scope.onRowCheckboxChange = function () {
        const grid = $("#CustomerGrid").data("kendoGrid");
        const allSelected = grid.dataSource.data().every(item => item.selected);
        const headerCheckbox = $('#CustomerGrid thead input[type="checkbox"]')[0];
        if (headerCheckbox) {
            headerCheckbox.checked = allSelected;
        }
    };
    $scope.onSelectedCustomerRowCheckboxChange = function () {
        const grid = $("#SelectedCustomerGrid").data("kendoGrid");
       
        const allSelected = grid.dataSource.data().every(item => item.selected);
        const headerCheckbox = $('#SelectedCustomerGrid thead input[type="checkbox"]')[0];
        if (headerCheckbox) {
            headerCheckbox.checked = allSelected;
        }
    };

    $scope.toggleAllSelectedCustomer = function ($event) {
        const checked = $event.target.checked;
        const grid = $("#SelectedCustomerGrid").data("kendoGrid");
        if (!grid) return;

        const view = grid.dataSource.view();

        view.forEach(item => {
            item.selected = checked;
        });

        $scope.$applyAsync();
    };
    function deepClone(obj) {
        return JSON.parse(JSON.stringify(obj));
    }

    function filterNodes(nodes, searchText) {
        const result = [];

        angular.forEach(nodes, function (node) {
            const children = node.Items ? (node.Items.data ? node.Items.data() : node.Items) : [];
            const filteredChildren = filterNodes(children, searchText);

            let isMatch = false;
            let nodeText = "";
            let newNode = {};

            newNode = deepClone(node);


            if (newNode.CUSTOMER_CODE && newNode.CUSTOMER_EDESC) {
                nodeText = (newNode.CUSTOMER_EDESC || "").toLowerCase();
                isMatch = nodeText.includes(searchText);
            } else {
                newNode.CUSTOMER_CODE = node.customerCode;
                newNode.CUSTOMER_EDESC = node.customerName;
                nodeText = (newNode.customerName || "").toLowerCase();
                isMatch = nodeText.includes(searchText);
            }

            if (isMatch || filteredChildren.length > 0) {
                newNode.Items = filteredChildren;
                result.push(newNode);
            }
        });

        return result;
    }

    $scope.filterTreeView = function () {
        const searchText = ($scope.txtMainGroupSearchString || '').toLowerCase();

        if (!searchText) {
            $scope.tree.setDataSource($scope.customertreeData);
            return;
        }

        const pristine = $scope.customertreeData._pristineData || $scope.customertreeData.data();
        const filteredData = filterNodes(pristine, searchText);

        const filteredDataSource = new kendo.data.HierarchicalDataSource({
            data: filteredData,
            schema: {
                model: {
                    id: "itemCode",
                    children: "Items"
                }
            }
        });

        $scope.tree.setDataSource(filteredDataSource);

        setTimeout(() => {
            $scope.tree.expand(".k-item");
        }, 200);
    };

    $scope.SubLedger_Cancel = function () {
        $scope.resetFormValidation();
        $scope.refresh();
        var grid2 = $("#kGrid2").data("kendo-grid");
        if (grid2 != undefined) {
            grid2.dataSource.data([]);
        }
        var listBox1 = $("#selectedselectOptions1").data("kendoListBox");
        if (listBox1 != undefined && listBox1 != "undefined") {
            listBox1.clearSelection();
        }
        var listBox2 = $("#selected").data("kendoListBox");
        if (listBox2 != undefined && listBox2 != "undefined") {
            listBox2.remove(listBox2.items());
        }
    }
    $scope.clearFields = function () {
        $scope.dealerArr.PARTY_TYPE_EDESC = "";
        $scope.dealerArr.PARTY_TYPE_NDESC = "";
        $scope.dealerArr.ACC_CODE = "";
        $scope.dealerArr.REMARKS = "";
        $scope.dealerArr.TEL_NO = "";
        $scope.dealerArr.GROUP_SKU_FLAG = "";
        $scope.dealerArr.PRE_PARTY_CODE = "";
        $scope.dealerArr.MASTER_PARTY_CODE = "";
        $scope.dealerArr.CREDIT_LIMIT = "";
        $scope.dealerArr.CREDIT_DAYS = "";
        $scope.dealerArr.PAN_NO = "";
        $scope.dealerArr.TEL_NO2 = "";
        $scope.dealerArr.ADDRESS = "";
        $scope.dealerArr.OWNER_NAME = "";
        $scope.dealerArr.LINK_BRANCH_CODE = "";
        $scope.dealerArr.AREA_CODE = "";
        $scope.dealerArr.ZONE_CODE = "";
        $scope.dealerArr.BG_AMOUNT = "";
        $scope.dealerArr.TERMS_CONDITIONS = "";
        $scope.dealerArr.APPROVED_FLAG = "";
        $scope.dealerArr.EXCEED_LIMIT_PERCENTAGE = "";
        $scope.dealerArr.TRADE_DISCOUNT = "";
        $scope.dealerArr.ANNUAL_BONUS = "";
        $scope.dealerArr.BG_PER_UNIT = "";
        $scope.dealerArr.CD_PER_UNIT = "";
        $scope.dealerArr.PDC_CHEQUE_AMT = "";
        $scope.dealerArr.SALES_TARGET = "";
        $scope.dealerArr.EMAIL = "";
    }

    //save Customers
    $scope.selectedItemList = [];
    $scope.saveNewCustomers = function (isValid) {
        debugger
        $scope.selectedItemList = [];

        var selectedGrid = $("#SelectedCustomerGrid").data("kendoGrid");
        if (!selectedGrid) return;

        var selectedData = selectedGrid.dataSource.data();

        for (var i = 0; i < selectedData.length; i++) {
            $scope.selectedItemList.push({
                CUSTOMER_CODE: selectedData[i].CUSTOMER_CODE
            });
        }

        var customerMappedurl = window.location.protocol + "//" + window.location.host + "/api/setupapi/createCustomerMapped";

        var model = {
            CUSTOMER_CODE: $("#customerChildAutoGenerated").val(),
            PARTY_TYPE_CODE: $scope.dealerArr.PARTY_TYPE_CODE,
            CustSubList: $scope.selectedItemList
        };

        $http({
            method: 'POST',
            url: customerMappedurl,
            data: model
        }).then(function successcallback(response) {
            if (response.data.MESSAGE === "INSERTED") {
                $scope.dealerArr.selectedCustomers = [];
                selectedGrid.setDataSource(new kendo.data.DataSource({ data: [] }));
                $("#CustomermapModal").modal("toggle");
                displayPopupNotification("Data successfully saved", "success");

                var grid = $("#kGrid2").data("kendoGrid");
                if (grid) {
                    grid.dataSource.read();
                }
            } else {
                displayPopupNotification("Something went wrong. Please try again later.", "error");
            }
        }, function errorcallback() {
            displayPopupNotification("Something went wrong. Please try again later.", "error");
        });
    };

    $scope.filterCustomerGrid = function (isInitialLoad = false) {
        debugger;
        const grid = $("#CustomerGrid").data("kendoGrid");
        if (!grid) return;

        $scope.BindCustomerGrid(isInitialLoad);
    };


    $scope.onCustomerSearchKeypress = function ($event) {
        debugger
        if ($event.which === 13 && !$scope.searchQuery) { 
            $scope.filterCustomerGrid(true);
        }
    };

    $scope.showTreeDropdown = false;

});

