DTModule.controller('dealerSetupCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter, hotkeys) {
    $scope.result = false;
    $scope.saveupdatebtn = "save";
    $scope.dealerArr =[];
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
        PRE_PARTY_CODE: "",
        MASTER_PARTY_CODE: "",
        PARTY_TYPE_FLAG:true,
    }
    $scope.dealerArr = $scope.dealersetup;
    $scope.l = function () {
        $http({
            method: 'GET',
            url: window.location.protocol + "//" + window.location.host + "/api/setupapi/GetPrefffromload",
        }).then(function successCallback(response) {
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
            $scope.dealersetup.MASTER_PARTY_CODE = currentItem.masterDealerCode;
            $scope.dealerArr.MASTER_PARTY_CODE = currentItem.masterDealerCode;
            $scope.treeSelectedDealer = currentItem.dealerId;
            $scope.dealersetup.PRE_PARTY_CODE = currentItem.preDealerCode;
            $scope.dealersetup.GROUP_SKU_FLAG = currentItem.groupSkuFlag;
            $scope.dealerArr.GROUP_SKU_FLAG = currentItem.groupSkuFlag;
            $scope.treeselectedmastercode = currentItem.masterDealerCode;
            $scope.dealerArr.masterDealerCode = currentItem.masterDealerCode;
            $scope.treenodeselected = "Y";
            $scope.newrootinsertFlag = "N";
            //$scope.refresh();
            var grid = $("#kGrid2").data("kendo-grid");
            if (grid != undefined) {
                grid.dataSource.read();
            }
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
        //$('#dealertree').data("kendoTreeView").expand('.k-item');
    }
    $scope.refresh = function () {
        debugger;
        var tree = $("#dealertree").data("kendoTreeView");
        tree.dataSource.read();
        var grid = $("#kGrid").data("kendo-grid");
        if (grid != undefined) {
            grid.dataSource.read();
        }
    }

    //Child grid
    $scope.BinddealerGrid = function (groupId) {
        debugger;
        var getdealerByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetChildOfDealerByGroup?groupId=" + groupId;
        $scope.dealerChildGridOptions = {

            dataSource: {
                /* type: "json",*/
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                transport: {
                    read: getdealerByUrl,
                },
                //pageSize: 50,
                //serverPaging: true,
                //serverSorting: true
                
            },
            scrollable: true,
            height: 450,
            sortable: true,
            pageable: true,
            dataBound: function (e) {
                $("#kGrid tbody tr").css("cursor", "pointer");
                DisplayNoResultsFound($('#kGrid'));
                $("#kGrid tbody tr").on('dblclick', function () {
                    var bcCode = $(this).find('td span').html();
                    $scope.edit(bcCode);
                    //var tree = $("#customertree").data("kendoTreeView");
                    //tree.dataSource.read();
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
        $("#dealerModal").modal("toggle");
    }

    //undergroup dropdown 
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
        var returnMaxCustomerUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getDealerData?dealerCode=" + $scope.treeSelectedDealer;
        $http({
            method: 'GET',
            url: returnMaxCustomerUrl,
        }).then(function successCallback(response) {
            $scope.dealersetup = response.data.DATA;
            $scope.dealerArr = response.data.DATA;
            $scope.dealerArr.masterDealerCode = $scope.dealersetup.MASTER_PARTY_CODE;
            $scope.dealerArr.PARTY_TYPE_CODE = $scope.dealersetup.PARTY_TYPE_CODE;
            $scope.dealerArr.MASTER_PARTY_CODE = $scope.dealersetup.MASTER_PARTY_CODE;
            $scope.dealerArr.PARTY_TYPE_EDESC = "";
            var tree = $("#dealerparentundergroup").data("kendoDropDownList");
            tree.value($scope.dealerArr.PARTY_TYPE_CODE);
            $scope.dealersetup.GROUP_SKU_FLAG = "I";
            $scope.dealerArr.GROUP_SKU_FLAG = "I";
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
                $scope.clearFields();
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

      
            $scope.dealersetup = response.data.DATA;
            $scope.dealerArr = response.data.DATA;
            $scope.dealerArr.masterDealerCode = $scope.dealersetup.MASTER_PARTY_CODE;
            $scope.dealerArr.PARTY_TYPE_CODE = $scope.dealersetup.PARTY_TYPE_CODE;
            $scope.dealerArr.MASTER_PARTY_CODE = $scope.dealersetup.PARTY_TYPE_CODE;
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
                /*type: "json",*/
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                transport: {
                    read: urlmapping,
                },
                pageSize: 50,
                serverSorting: true
            },
            scrollable: true,
            height: 450,
            sortable: true,
            pageable: true,
            dataBound: function (e) {
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
                //{
                //    field: "REGD_OFFICE_EADDRESS",
                //    title: "Address",
                //    width: "120px"
                //},

            ],
        };
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
                    $scope.divisionsetup.GROUP_SKU_FLAG = "I";
                    var delUrl =
			  window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteDealer?dealerCode=" + code;
                    $http({
                        method: 'POST',
                        url: delUrl
                    }).then(function successCallback(response) {
                        if (response.data.MESSAGE == "DELETED") {
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

    //Save and Update Function
    $scope.saveNewDealer = function (isValid) {
        var masterdealervalue = $scope.treeselectedmastercode;
        if (!isValid) {
            displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
            return;
        }

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
                MASTER_PARTY_CODE: $scope.dealerArr.masterDealerCode,
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
                    displayPopupNotification("data succesfully saved ", "success");
                    $scope.treenodeselected = "N";
                    $scope.refresh();
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            }, function errorcallback(response) {
                displayPopupNotification("Something went wrong.Please try again later.", "error");

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
                MASTER_PARTY_CODE:$scope.dealerArr.masterDealerCode,
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
    //For customer Mpping

    $scope.showModalForCustomer = function (event) {
        $scope.saveupdatebtn = "Save"
        var grid = $("#kGrid2").data("kendo-grid");
        if (grid != undefined) {
            grid.dataSource.read();
        }
        $("#CustomermapModal").modal("toggle");
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
                        dataType: "json"
                    }
                }
            }
        };
        $scope.selectOptions2 = {
            dataTextField: "CUSTOMER_EDESC",
            dataValueField: "CUSTOMER_CODE",
            filterable: true,
        };
    }
    $scope.SubLedger_Cancel = function () {
        $scope.refresh();
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
       
    }
    
    //save Customers
    $scope.selectedItemList = [];
    $scope.saveNewCustomers = function (isValid) {
        //var listBox = $("#selected").val();
        //var listBox = $("#selected").data("kendoListBox");
        $scope.selectedItemList = [];
        var dataItem = $("#selected").data("kendoListBox").dataItems();
        for (var i = 0; i < dataItem.length; i++) {
            $scope.selectedItemList.push({
                CUSTOMER_CODE:dataItem[i].CUSTOMER_CODE,
            });
        }
        var customerMappedurl = window.location.protocol + "//" + window.location.host + "/api/setupapi/createCustomerMapped";
        var model = {
            PARTY_TYPE_CODE: $scope.dealerArr.PARTY_TYPE_CODE,
            CustSubList: $scope.selectedItemList
        }
            $http({
                method: 'post',
                url: customerMappedurl,
                data: model
            }).then(function successcallback(response) {

                if (response.data.MESSAGE == "INSERTED") {
                    $("#CustomermapModal").modal("toggle");
                    displayPopupNotification("data succesfully saved ", "success");
                    var grid = $("#kGrid2").data("kendo-grid");
                    if (grid != undefined) {
                        grid.dataSource.read();
                    }
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            }, function errorcallback(response) {
                displayPopupNotification("Something went wrong.Please try again later.", "error");
            });
        }
});

