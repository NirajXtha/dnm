DTModule.controller('resourceSetupTreeCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter, $timeout) {
    $scope.saveupdatebtn = "Save";
    $scope.resourcesArr;
    $scope.savegroup = false;
    $scope.editFlag = "N";
    $scope.newrootinsertFlag = "Y";
    $scope.treenodeselected = "N";
    $scope.groupchilddisabled = true;
    $scope.masterresourcedisabled = false;

    $scope.treeselectedresourceCode = "";
    //$scope.groupResourceTypeFlag = "Y";
    $scope.resourcesetup =
    {
        RESOURCE_TYPE: "",
        RESOURCE_CODE: "",
        RESOURCE_EDESC: "",
        RESOURCE_NDESC: "",
        RESOURCE_FLAG: "G",
        GROUP_SKU_FLAG: "",
        PRE_RESOURCE_CODE: "",
        REMARKS: "",
        PARENT_RESOURCE_CODE: "",
    }
    $scope.resourceDetails = [];
    $scope.unitList = [];



    $scope.fillResourceSetupForms = function () {
        var getUnilList = window.location.protocol + "//" + window.location.host + "/api/ProcessSetupBomApi/GetUnitList";
        $http({
            method: 'GET',
            url: getUnilList,
        }).then(function successCallback(response) {

            console.log(response);
            $scope.unitList = response.data;
        }, function errorCallback(response) { });
    }; $scope.fillResourceSetupForms();


    $scope.resourcesArr = $scope.resourcesetup;
    var resourcedataFillDefered = $.Deferred();
    var resourceCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getResourceCodeWithChild";

    $scope.resourceGroupDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: resourceCodeUrl,
            }
        }
    });

    $scope.resourceGroupOptions = {
        dataSource: $scope.resourceGroupDataSource,
        optionLabel: "<PRIMARY>",
        dataTextField: "RESOURCE_EDESC",
        dataValueField: "RESOURCE_CODE",
        change: function (e) {

            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {

        }
    };


    $scope.initFunction = function () {
        // $scope.resourcesArr.EFFECTIVE_DATE = new Date();
        //var purchaseDate = new Date($scope.resourcesArr.PURCHASE_DATE);
        //var effectiveDate = new Date($scope.resourcesArr.EFFECTIVE_DATE);
        //$scope.resourcesArr.PURCHASE_DATE_BS = ConvertEngDateToNep(
        //    $filter('date')(new Date(purchaseDate.setDate(purchaseDate.getDate())), 'dd-MMM-yyyy')
        //);
        //$scope.resourcesArr.EFFECTIVE_DATE_BS = ConvertEngDateToNep(
        //    $filter('date')(new Date(effectiveDate.setDate(purchaseDate.getDate())), 'dd-MMM-yyyy')
        //);
    }; $scope.initFunction();


    var getResourceCodeByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetResource";
    $scope.resourcetreeData = new kendo.data.HierarchicalDataSource({

        transport: {
            read: {
                url: getResourceCodeByUrl,
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
                id: "RESOURCE_CODE",
                parentId: "PRE_RESOURCE_CODE",
                children: "Items",
                fields: {
                    RESOURCE_CODE: { field: "RESOURCE_CODE", type: "string" },
                    RESOURCE_EDESC: { field: "RESOURCE_EDESC", type: "string" },
                    parentId: { field: "PRE_RESOURCE_CODE", type: "string", defaultValue: "00" },
                }
            }
        }
    });




    $scope.monthSelectorOptionsSingle1 = {
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

    $scope.monthSelectorOptionsSingle12 = {
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepang2(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };

    $scope.ConvertEngToNepang = function (data) {
        $("#nepaliDate51").val(AD2BS(data));
    };

    $scope.ConvertEngToNepang2 = function (data) {
        $("#nepaliDate512").val(AD2BS(data));
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

            if (newNode.RESOURCE_CODE && newNode.RESOURCE_EDESC) {
                nodeText = (newNode.RESOURCE_EDESC || "").toLowerCase();
                isMatch = nodeText.includes(searchText);
            } else {
                newNode.RESOURCE_CODE = node.ResourceId;
                newNode.RESOURCE_EDESC = node.ResourceName;
                nodeText = (newNode.ResourceName || "").toLowerCase();
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
            $scope.tree.setDataSource($scope.resourcetreeData);
            return;
        }

        const pristine = $scope.resourcetreeData._pristineData || $scope.resourcetreeData.data();
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
        }, 100);
    };

    //Grid Binding main Part

    $scope.resourceOnDataBound = function () {
        $('#resourcetree').data("kendoTreeView").expand('.k-item');
    }

    $scope.fillResourceSetupForms = function (resourceId) {
        var getResourcedetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getResourceDetailsByResourceCode?resourceCode=" + resourceId;
        $http({
            method: 'GET',
            url: getResourcedetaisByUrl,

        }).then(function successCallback(response) {

            if (response.data.DATA != null) {

                $scope.resourcesetup = response.data.DATA;
                $scope.resourcesArr = response.data.DATA;

                $scope.resourcesArr.PURCHASE_DATE = $scope.resourcesArr.PURCHASE_DATE;
                $scope.resourcesArr.EFFECTIVE_DATE = $scope.resourcesArr.EFFECTIVE_DATE;

                var purchaseDate = new Date($scope.resourcesArr.PURCHASE_DATE);
                var effectiveDate = new Date($scope.resourcesArr.EFFECTIVE_DATE);


                $scope.resourcesArr.PURCHASE_DATE_BS = ConvertEngDateToNep(
                    $filter('date')(new Date(purchaseDate.setDate(purchaseDate.getDate())), 'dd-MMM-yyyy')
                );
                $scope.resourcesArr.EFFECTIVE_DATE_BS = ConvertEngDateToNep(
                    $filter('date')(new Date(effectiveDate.setDate(purchaseDate.getDate())), 'dd-MMM-yyyy')
                );


                if ($scope.resourcesetup.RESOURCE_DETAIL_LIST && $scope.resourcesetup.RESOURCE_DETAIL_LIST.length) {
                    $scope.resourcesArr.IS_INDIVIDUAL_OR_SERIAL_ITEM = true;
                    $scope.resourceDetails = [];
                    $scope.resourcesetup.RESOURCE_DETAIL_LIST.forEach(item => {
                        $scope.addDetailRow(item);
                    });
                }
                resourcedataFillDefered.resolve(response);
            }

        }, function errorCallback(response) {

        });
    }
    //treeview on select
    $scope.resourceOptions = {
        loadOnDemand: false,
        select: function (e) {

            var currentItem = e.sender.dataItem(e.node);
            $('#resourceGrid').removeClass("show-displaygrid");
            $("#resourceGrid").html("");
            $($(this._current).parents('ul')[$(this._current).parents('ul').length - 1]).find('span').removeClass('hasTreeCustomMenu');
            $(this._current.context).find('span').addClass('hasTreeCustomMenu');
            $scope.resourcesetup.RESOURCE_CODE = currentItem.ResourceId;
            $scope.resourcesArr.RESOURCE_CODE = currentItem.ResourceId;
            $scope.treeselectedresourceCode = currentItem.ResourceId;
            $scope.resourcesetup.GROUP_SKU_FLAG = currentItem.groupSkuFlag;
            $scope.resourcesArr.GROUP_SKU_FLAG = currentItem.groupSkuFlag;
            //$scope.resourcesetup.RESOURCE_EDESC = currentItem.ResourceName;
            $scope.resourcesetup.MASTER_RESOURCE_CODE = currentItem.masterResourceCode;
            $scope.treenodeselected = "Y";
            $scope.newrootinsertFlag = "N";
            $scope.$apply();
            //$scope.movescrollbar();
        },

    };

    $scope.movescrollbar = function () {
        var element = $(".k-in");
        for (var i = 0; i < element.length; i++) {
            var selectnode = $(element[i]).hasClass("k-state-focused");
            if (selectnode) {
                $("#resourcetree").animate({
                    scrollTop: (parseInt(i * 2))
                });
                break;
            }
        }
    }
    $scope.onContextSelect = function (event) {

        if ($scope.resourcesetup.RESOURCE_CODE == "")
            return displayPopupNotification("Select resource.", "error");;
        $scope.saveupdatebtn = "Save";
        if (event.item.innerText.trim() == "Delete") {

            $scope.delete($scope.resourcesArr.RESOURCE_CODE, "");
        }
        else if (event.item.innerText.trim() == "Update") {

            resourcedataFillDefered = $.Deferred();
            $scope.saveupdatebtn = "Update";
            $scope.fillResourceSetupForms($scope.resourcesetup.RESOURCE_CODE);
            $.when(resourcedataFillDefered).then(function () {

                $scope.masterresourcedisabled = true;
                if ($scope.resourcesetup.PARENT_RESOURCE_CODE == null || $scope.resourcesetup.PARENT_RESOURCE_CODE == undefined) {
                    var popUpDropdown = $("#masterresourcecode").data("kendoDropDownList");
                    popUpDropdown.value('');
                }
                else {
                    var popUpDropdown = $("#masterresourcecode").data("kendoDropDownList");
                    popUpDropdown.value($scope.resourcesetup.PARENT_RESOURCE_CODE);
                }
                $("#resourceModal").modal();
            });

        }
        else if (event.item.innerText.trim() == "Add") {
            resourcedataFillDefered = $.Deferred();
            $scope.savegroup = true;
            $scope.resourcesArr = [];
            $scope.fillResourceSetupForms($scope.resourcesetup.RESOURCE_CODE);
            $.when(resourcedataFillDefered).then(function () {

                var tree = $("#masterresourcecode").data("kendoDropDownList");
                tree.value($scope.treeselectedresourceCode);
                $scope.resourcesArr.RESOURCE_CODE = $scope.resourcesetup.RESOURCE_CODE;
                $scope.resourcesArr.GROUP_SKU_FLAG = "G";
                $scope.ClearFields();
                $("#resourceModal").modal();

            });

        }
    }
    $scope.saveNewResource = function (isValid) {

        var preresourcecode = $("#masterresourcecode").data("kendoDropDownList").value();
        var model = {
            RESOURCE_CODE: $scope.resourcesArr.RESOURCE_CODE,
            RESOURCE_EDESC: $scope.resourcesArr.RESOURCE_EDESC,
            RESOURCE_NDESC: $scope.resourcesArr.RESOURCE_NDESC,
            PRE_RESOURCE_CODE: preresourcecode,
            GROUP_SKU_FLAG: $scope.resourcesArr.GROUP_SKU_FLAG,
            RESOURCE_TYPE: $scope.resourcesArr.RESOURCE_TYPE,
            REMARKS: $scope.resourcesArr.REMARKS,

            RESOURCE_DETAIL_LIST: $scope.resourceDetails,
            IS_INDIVIDUAL_OR_SERIAL_ITEM: $scope.resourcesArr.IS_INDIVIDUAL_OR_SERIAL_ITEM,

            PURCHASE_DATE: $scope.resourcesArr.PURCHASE_DATE,
            EFFECTIVE_DATE: $scope.resourcesArr.EFFECTIVE_DATE,
            OUTPUT_CAPACITY: $scope.resourcesArr.OUTPUT_CAPACITY,
            UNIT: $scope.resourcesArr.UNIT
        }
        if (!isValid) {
            displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
            return;
        }
        if (model.GROUP_SKU_FLAG == "I" && preresourcecode == "") {
            return displayPopupNotification("Plese Select", "error");
        }
        if ($scope.saveupdatebtn == "Save") {
            var createUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewResourceHead";
            $http({
                method: 'POST',
                url: createUrl,
                data: model
            }).then(function successCallback(response) {

                if (response.data.MESSAGE == "INSERTED") {
                    $scope.resourcesArr = [];
                    if ($scope.resourcesetup.GROUP_SKU_FLAG !== "I") {
                        var tree = $("#resourcetree").data("kendoTreeView");
                        tree.dataSource.read();
                    }
                    if ($scope.savegroup == false) { $("#resourceModal").modal("toggle"); }
                    else { $("#resourceModal").modal("toggle"); }
                    //$("#kGrid").data("kendoGrid").dataSource.read();
                    var grid = $("#kGrid").data("kendo-grid");
                    if (grid != undefined) {
                        grid.dataSource.read();
                    }
                    $("#masterresourcecode").data("kendoDropDownList").dataSource.read();
                    displayPopupNotification("Data succesfully saved ", "success");
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Resource name already exist please try another resource name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // this callback will be called asynchronously
                // when the response is available
            }, function errorCallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Resource name already exist please try another resource name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
        }
        else {
            var updateUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateResourceByResourceCode";

            $scope.saveupdatebtn = "Update";
            $http({
                method: 'POST',
                url: updateUrl,
                data: model
            }).then(function successCallback(response) {

                if (response.data.MESSAGE == "UPDATED") {
                    $scope.resourcesArr = [];
                    if ($scope.resourcesetup.GROUP_SKU_FLAG !== "I") {
                        var tree = $("#resourcetree").data("kendoTreeView");
                        tree.dataSource.read();
                    }
                    //$("#kGrid").data("kendoGrid").dataSource.read();
                    var grid = $("#kGrid").data("kendo-grid");
                    if (grid != undefined) {
                        grid.dataSource.read();
                    }
                    $("#masterresourcecode").data("kendoDropDownList").dataSource.read();
                    $("#resourceModal").modal("toggle");
                    displayPopupNotification("Data succesfully updated ", "success");

                }
                if (response.data.MESSAGE == "ERROR") {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // this callback will be called asynchronously
                // when the response is available
            }, function errorCallback(response) {
                displayPopupNotification("Something went wrong.Please try again later.", "error");
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
        }
    }

    $scope.BindGrid = function (groupId) {
        $(".topsearch").show();
        var url = null;
        if (groupId == "All") {
            if ($('#restxtSearchString').val() == null || $('#restxtSearchString').val() == '' || $('#restxtSearchString').val() == undefined || $('#restxtSearchString').val() == 'undefined') {
                alert('Input is empty or undefined.');
                return;
            }
            url = "/api/SetupApi/GetAllResourceList?searchtext=" + $('#restxtSearchString').val();
        }
        else {
            $("#restxtSearchString").val('');
            url = "/api/SetupApi/GetChildOfResourceByGroup?groupId=" + groupId;
        }

        $timeout(function () {
            var $input = $("#restxtSearchString");
            if ($input.data("bindGlobalSearch")) return;
            $input.data("bindGlobalSearch", true);

            function debounce(fn, delay) {
                var t;
                return function () {
                    var ctx = this, args = arguments;
                    clearTimeout(t);
                    t = setTimeout(function () { fn.apply(ctx, args); }, delay);
                };
            }

            var triggerGlobal = function () {
                var val = $input.val();
                if (val && val.toString().trim().length > 0) {
                    $scope.$evalAsync(function () { $scope.BindGrid('All'); });
                }
            };

            $input.on('keydown.customerGlobal', function (e) {
                if (e.key === 'Enter' || e.keyCode === 13) {
                    e.preventDefault();
                    triggerGlobal();
                }
            });

            $input.on('input.customerGlobal', debounce(triggerGlobal, 400));
        }, 0);

        var reportConfig = GetReportSetting("_resourceSetupPartial");
        $scope.resourceChildGridOptions = {
            dataSource: {
                transport: {
                    read: {
                        url: url,
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        type: "GET"
                    },
                    parameterMap: function (options, type) {
                        var paramMap = JSON.stringify($.extend(options, ReportFilter.filterAdditionalData()));
                        delete paramMap.$inlinecount;
                        delete paramMap.$format;
                        return paramMap;
                    }
                },
                error: function (e) {
                    displayPopupNotification("Sorry error occured while processing data", "error");
                },
                model: {
                    fields: {
                        RESOURCE_EDESC: { type: "string" },
                        REMARKS: { type: "string" },
                        CREATED_BY: { type: "string" },
                        CREATED_DATE: { type: "string" }
                    }
                },
                /*pageSize: reportConfig.defaultPageSize,*/
                pageSize: 50,
            },
            height: 500,
            sortable: true,
            reorderable: true,
            groupable: true,
            resizable: true,
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
                        startswith: "Starts with	",
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
                checkboxItem = $(e.container).find('input[type="checkbox"]');
            },
            columnShow: function (e) {
                if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                    SaveReportSetting('_resourceSetupPartial', 'kGrid');
            },
            columnHide: function (e) {
                if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                    SaveReportSetting('_resourceSetupPartial', 'kGrid');
            },
            pageable: {
                refresh: true,
                //pageSizes: reportConfig.itemPerPage,
                pageSize: 50,
                buttonCount: 5
            },
            pageable: true,
            dataBound: function (o) {
                var grid = o.sender;
                if (grid.dataSource.total() == 0) {
                    var colCount = grid.columns.length;
                    $(o.sender.wrapper)
                        .find('tbody')
                        .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, No data found</td></tr>');
                    /*displayPopupNotification("No Data Found.", "info");*/
                }
                else {
                    var g = $("#kGrid").data("kendoGrid");
                    for (var i = 0; i < g.columns.length; i++) {
                        g.showColumn(i);
                    }
                    $("div.k-group-indicator").each(function (i, v) {
                        g.hideColumn($(v).data("field"));
                    });
                }
                UpdateReportUsingSetting("_resourceSetupPartial", "kGrid");
                $('div').removeClass('.k-header k-grid-toolbar');
            },
            columns: [
                //{
                //    //hidden: true,
                //    title: "Resource ID",
                //    field: "RESOURCE_CODE",
                //    width: "80px"
                //},
                {
                    field: "RESOURCE_EDESC",
                    title: "Resource Name",
                    width: "120px"
                },

                {
                    field: "REMARKS",
                    title: "Remarks",
                    width: "120px"
                },
                {
                    field: "CREATED_BY",
                    title: "CREATED BY",
                    width: "120px"
                }, {
                    field: "CREATED_DATE",
                    title: "CREATED DATE",
                    template: "#= kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy') #",
                    width: "120px"
                },

                {
                    title: "Action ",
                    template: '<a class="fa fa-pencil-square-o editAction" title="Edit" ng-click="edit(dataItem.RESOURCE_CODE)"><span class="sr-only"></span> </a><a class="fa fa-trash deleteAction" title="Delete" ng-click="delete(dataItem.RESOURCE_CODE,dataItem.GROUP_SKU_FLAG)"><span class="sr-only"></span> </a>',
                    width: "60px"
                }
            ],
        };


        $scope.onsiteSearch = function ($this) {

            var q = $("#txtSearchString").val();
            var grid = $("#kGrid").data("kendo-grid");
            grid.dataSource.query({
                page: 1,
                pageSize: 50,
                filter: {
                    logic: "or",
                    filters: [
                        { field: "ORDER_NO", operator: "contains", value: q },
                        { field: "ORDER_DATE", operator: "contains", value: q },
                        { field: "CREATED_BY", operator: "contains", value: q }
                    ]
                }
            });
        }
    }

    $scope.showModalForNew = function (event, value = "") {



        resourcedataFillDefered = $.Deferred();
        $scope.editFlag = "N";
        $scope.fillResourceSetupForms($scope.treeselectedresourceCode);
        $.when(resourcedataFillDefered).then(function () {
            $scope.ClearFields();
            $scope.groupResourceTypeFlag = "N";
            $scope.saveupdatebtn = "Save"
            $scope.masterresourcedisabled = false;
            $scope.resourcesetup.GROUP_SKU_FLAG = "I";
            $scope.resourcesArr.GROUP_SKU_FLAG = "I";
            var tree = $("#masterresourcecode").data("kendoDropDownList");
            if (tree != null && tree != undefined) {
                tree.value($scope.treeselectedresourceCode);
            }
            $("#resourceModal").modal("toggle");
        }, function errorCallback(response) {
            displayPopupNotification(response.data.RESOURCE_CODE, "error");

        });



        if (value == 'N') {
            setTimeout(function () {

                $('#englishdatedocument1').val($filter('date')(new Date(), 'dd-MMM-yyyy'));
                $scope.resourcesArr.PURCHASE_DATE = $filter('date')(new Date(), 'dd-MMM-yyyy');
                $('#englishdatedocument12').val($filter('date')(new Date(), 'dd-MMM-yyyy'));
                $scope.resourcesArr.EFFECTIVE_DATE = $filter('date')(new Date(), 'dd-MMM-yyyy');



                var purchaseDate = new Date($scope.resourcesArr.PURCHASE_DATE);
                var effectiveDate = new Date($scope.resourcesArr.EFFECTIVE_DATE);


                $('#nepaliDate51').val(ConvertEngDateToNep($filter('date')(new Date(purchaseDate.setDate(purchaseDate.getDate())), 'dd-MMM-yyyy')));
                $scope.resourcesArr.PURCHASE_DATE_BS = ConvertEngDateToNep(
                    $filter('date')(new Date(purchaseDate.setDate(purchaseDate.getDate())), 'dd-MMM-yyyy')
                );

                $('#nepaliDate512').val(ConvertEngDateToNep(
                    $filter('date')(new Date(effectiveDate.setDate(effectiveDate.getDate())), 'dd-MMM-yyyy')
                ));
                $scope.resourcesArr.EFFECTIVE_DATE_BS = ConvertEngDateToNep(
                    $filter('date')(new Date(effectiveDate.setDate(effectiveDate.getDate())), 'dd-MMM-yyyy')
                );

            }, 200);
        }

    }

    $scope.edit = function (resourceCode) {

        console.log(resourceCode);

        $scope.editFlag = "Y";
        $scope.saveupdatebtn = "Update"
        $scope.masterresourcedisabled = true;
        $scope.fillResourceSetupForms(resourceCode);
        $scope.groupResourceTypeFlag = "N";
        $scope.resourcesetup.GROUP_SKU_FLAG = "I";
        $scope.resourcesArr.GROUP_SKU_FLAG = "I";
        var tree = $("#masterresourcecode").data("kendoDropDownList");
        tree.value($scope.treeselectedresourceCode);
        $("#resourceModal").modal();
    }
    $scope.delete = function (code, skf) {

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

                    if (skf === "I")
                        $scope.resourcesetup.GROUP_SKU_FLAG = "I";
                    else
                        $scope.resourcesetup.GROUP_SKU_FLAG = "G";

                    var delUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteResourceSetupByResourceCode?resourceCode=" + code;
                    $http({
                        method: 'POST',
                        url: delUrl
                    }).then(function successCallback(response) {

                        if (response.data.MESSAGE == "DELETED") {
                            if ($scope.resourcesetup.GROUP_SKU_FLAG !== "I") {
                                var tree = $("#resourcetree").data("kendoTreeView");
                                tree.dataSource.read();
                            }
                            $("#kGrid").data("kendoGrid").dataSource.read();
                            displayPopupNotification("Data succesfully deleted ", "success");
                        }
                        else if (response.data.MESSAGE == "HAS_CHILD") {
                            displayPopupNotification("You can not delete. It has child.", "warning");
                        }
                    }, function errorCallback(response) {
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                    });

                }

            }
        });
    }
    $scope.addNewResource = function () {
        $scope.editFlag = "N";
        $scope.saveupdatebtn = "Save"
        $scope.masterresourcedisabled = false;
        $scope.groupResourceTypeFlag = "Y";
        $scope.resourcesetup.GROUP_SKU_FLAG = "G";
        $scope.resourcesArr.GROUP_SKU_FLAG = "G";
        var tree = $("#masterresourcecode").data("kendoDropDownList");
        $scope.ClearFields();
        tree.value("");
        $('#resourcetree').data("kendoTreeView").dataSource.read();
        $("#resourceModal").modal("toggle");
    }
    $scope.Cancel = function () {
        $scope.saveupdatebtn = "Save";
        $scope.resourcesArr = [];
        //$("#kGrid").data("kendoGrid").clearSelection();
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
                //grid.find('.k-grid-content tbody').append('<tr class="kendo-data-row"><td>&nbsp;</td></tr>');
            }
        }
    }

    $scope.ClearFields = function () {
        $scope.resourcesArr.RESOURCE_EDESC = "";
        $scope.resourcesArr.RESOURCE_NDESC = "";
        $scope.resourcesArr.RESOURCE_TYPE = "";
        $scope.resourcesArr.REMARKS = "";

    }

    // added by mahesh
    $scope.onSerialItemToggle = function (value) {
        console.log(value);
        if (value) {
            $scope.resourceDetails = [];
            $scope.addDetailRow();
        }
    };

    $scope.addDetailRow = function (item = {}) {

        $scope.resourceDetails.push({
            RESOURCE_UNIQUE_NAME: item.RESOURCE_UNIQUE_NAME || '',
            SERIAL_NO: item.SERIAL_NO || '',
            PURCHASE_DATE: item.PURCHASE_DATE ? item.PURCHASE_DATE : '',
            EFFECTIVE_DATE: item.EFFECTIVE_DATE ? item.EFFECTIVE_DATE : '',
            OUTPUT_CAPACITY: item.OUTPUT_CAPACITY || '',
            UNIT: item.UNIT || ''
        });
    };



    $scope.removeDetailRow = function (index) {
        $scope.resourceDetails.splice(index, 1);
    };

    //get resource list

    //ProcessSetupBomApi / GetUnitList




});

