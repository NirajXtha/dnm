DTModule.controller('attributeSetupTreeCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    $scope.saveupdatebtn = "Save";
    $scope.attributesArr;
    $scope.savegroup = false;
    $scope.groupschilddisabled = true;
    $scope.editFlag = "N";
    $scope.newrootinsertFlag = "Y";
    $scope.treenodeselected = "N";
    $scope.attributesetup =
    {
        ATTRIBUTE_CODE: "",
        ATTRIBUTE_EDESC: "",
        ATTRIBUTE_NDESC: "",
        REMARKS: "",
        PRE_ATTRIBUTE_CODE: "",
        GROUP_SKU_FLAG: "G",
        PARENT_ATTRIBUTE_CODE: "",
    }
    $scope.treeselectedAttributeCode = "";
    $scope.attributesArr = $scope.attributesetup;
    $scope.masterAttributeCodeDataSource = [
        { text: "<PRIMARY>", value: "" }
    ];
    $scope.treeselectedattributeCode = "";
    var attributedataFillDefered = $.Deferred();

    var attributeCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/Getattribute";

    $scope.attributeGroupDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: attributeCodeUrl,
            }
        }
    });
    $scope.MACDS = [];

    $scope.attributeGroupOptions = {
        dataSource: $scope.attributeGroupDataSource,
        optionLabel: "<PRIMARY>",
        dataTextField: "ATTRIBUTE_EDESC",
        dataValueField: "ATTRIBUTE_CODE",
        change: function (e) {

            var currentItem = e.sender.dataItem(e.node);

        },
        dataBound: function (e) {

        }
    }


    var gettoattributesByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetTreeAttribute";
    $scope.attributetreeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: gettoattributesByUrl,
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
                id: "ATTRIBUTE_CODE",
                parentId: "PRE_ATTRIBUTE_CODE",
                children: "Items",
                fields: {
                    ATTRIBUTE_CODE: { field: "ATTRIBUTE_CODE", type: "string" },
                    ATTRIBUTE_EDESC: { field: "ATTRIBUTE_EDESC", type: "string" },
                    parentId: { field: "PRE_ATTRIBUTE_CODE", type: "string", defaultValue: "00" },
                }
            }
        }
    });

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

            if (newNode.ATTRIBUTE_CODE && newNode.ATTRIBUTE_EDESC) {
                nodeText = (newNode.ATTRIBUTE_EDESC || "").toLowerCase();
                isMatch = nodeText.includes(searchText);
            } else {
                newNode.ATTRIBUTE_CODE = node.AttributeId;
                newNode.ATTRIBUTE_EDESC = node.AttributeName;
                nodeText = (newNode.AttributeName || "").toLowerCase();
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
            $scope.tree.setDataSource($scope.attributetreeData);
            return;
        }

        const pristine = $scope.attributetreeData._pristineData || $scope.attributetreeData.data();
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

    $scope.attributeOnDataBound = function (e) {

        $('#attributetree').data("kendoTreeView").expand('.k-item');
    }
    $scope.getDetailByAttributeCode = function (attributeId) {
        var getAttributedetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getAttributeDetailsByAttributeCode?attributeCode=" + attributeId;
        $http({
            method: 'GET',
            url: getAttributedetaisByUrl,

        }).then(function successCallback(response) {
            var attributeNature = response.data.DATA.ATTRIBUTE_NATURE;
            $scope.bindNatureByAttributeNature(attributeNature);

        }, function errorCallback(response) {

        });
    }

    $scope.fillAttributeSetupForms = function (attributeId) {

        var getAttributedetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getAttributeDetailsByAttributeCode?attributeCode=" + attributeId;
        $http({
            method: 'GET',
            url: getAttributedetaisByUrl,

        }).then(function successCallback(response) {

            if (response.data.DATA != null) {
                if (response.data.DATA.GROUP_SKU_FLAG == "I") {
                    var dropdownlist = $("#masterattributecode").data("kendoDropDownList");
                    dropdownlist.value($scope.treeselectedAttributeCode);
                }

                $scope.attributesetup = response.data.DATA;
                $scope.attributesArr = response.data.DATA;



                attributedataFillDefered.resolve(response);

            }


            // this callback will be called asynchronously
            // when the response is available
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
    }
    //treeview on select
    $scope.attributeoptions = {
        loadOnDemand: false,
        select: function (e) {
            var currentItem = e.sender.dataItem(e.node);
            $('#attributeGrid').removeClass("show-displaygrid");
            $("#attributeGrid").html("");
            $($(this._current).parents('ul')[$(this._current).parents('ul').length - 1]).find('span').removeClass('hasTreeCustomMenu');
            $(this._current.context).find('span').addClass('hasTreeCustomMenu');
            $scope.attributesetup.ATTRIBUTE_CODE = currentItem.AttributeId;
            $scope.attributesArr.ATTRIBUTE_CODE = currentItem.AttributeId;
            $scope.attributesetup.ATTRIBUTE_EDESC = currentItem.AttributeName;
            $scope.treenodeselected = "Y";
            $scope.treeselectedAttributeCode = currentItem.AttributeId;
            $scope.newrootinsertFlag = "N";
            $scope.groupschilddisabled = true;
            $scope.$apply();
            //$scope.movescrollbar();

        },
        editable: {
            add: false,
            update: true,
            destroy: true,
        }

    };

    $scope.movescrollbar = function () {
        var element = $(".k-in");
        for (var i = 0; i < element.length; i++) {
            var selectnode = $(element[i]).hasClass("k-state-focused");
            if (selectnode) {
                $("#attributetree").animate({
                    scrollTop: (parseInt(i * 2))
                });
                break;
            }
        }
    }
    $scope.onContextSelect = function (event) {
        if ($scope.attributesetup.ATTRIBUTE_CODE == "")
            return displayPopupNotification("Select attribute.", "error");;
        $scope.saveupdatebtn = "Save";
        if (event.item.innerText.trim() == "Delete") {
            $scope.delete($scope.attributesArr.ATTRIBUTE_CODE, "");
        }

        else if (event.item.innerText.trim() == "Update") {
            attributedataFillDefered = $.Deferred();
            $scope.saveupdatebtn = "Update";
            $scope.fillAttributeSetupForms($scope.attributesetup.ATTRIBUTE_CODE);
            $.when(attributedataFillDefered).then(function () {

                if ($scope.attributesetup.PARENT_ATTRIBUTE_CODE == null || $scope.attributesetup.PARENT_ATTRIBUTE_CODE == undefined) {
                    var popUpDropdown = $("#masterattributecode").data("kendoDropDownList");
                    popUpDropdown.value('');
                }
                else {
                    var popUpDropdown = $("#masterattributecode").data("kendoDropDownList");
                    popUpDropdown.value($scope.attributesetup.PARENT_ATTRIBUTE_CODE);
                }
                $("#attributeModal").modal();
            })
        }
        else if (event.item.innerText.trim() == "Add") {
            attributedataFillDefered = $.Deferred();
            $scope.savegroup = true;
            $scope.attributesArr = [];
            $scope.fillAttributeSetupForms($scope.attributesetup.ATTRIBUTE_CODE);
            $.when(attributedataFillDefered).then(function () {
                $scope.Cleardata();
                var dropdownlist = $("#masterattributecode").data("kendoDropDownList");
                dropdownlist.value($scope.treeselectedAttributeCode);
                $scope.attributesetup.GROUP_SKU_FLAG = "G";
                $scope.attributesArr.GROUP_SKU_FLAG = "G";
                $("#attributeModal").modal();

            });


        }
    }
    $scope.saveNewAttribute = function (form) {
            if (!form.$valid) {
                displayPopupNotification("Please Select Attribute English Name", "warning");
                return;
            } 

        if ($scope.saveupdatebtn == "Save") {
            var createUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewAttributeHead";
            var model = {
                ATTRIBUTE_CODE: $scope.attributesArr.ATTRIBUTE_CODE,
                ATTRIBUTE_EDESC: $scope.attributesArr.ATTRIBUTE_EDESC,
                ATTRIBUTE_NDESC: $scope.attributesArr.ATTRIBUTE_NDESC,
                REMARKS: $scope.attributesArr.REMARKS,
                GROUP_SKU_FLAG: $scope.attributesArr.GROUP_SKU_FLAG,
            }
            $http({
                method: 'POST',
                url: createUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "INSERTED") {
                    $scope.attributeArr = [];

                    if ($scope.attributesetup.GROUP_SKU_FLAG !== "I") {
                        var tree = $("#attributetree").data("kendoTreeView");
                        tree.dataSource.read();
                    }

                    var grid = $('#kGrid').data("kendoGrid");
                    if (grid != undefined) {
                        grid.dataSource.read();
                    }
                    //$scope.Cleardata();
                    if ($scope.savegroup == true) { $("#attributeModal").modal("toggle"); }
                    else { $("#attributeModal").modal("toggle"); }
                    $("#masterattributecode").data("kendoDropDownList").dataSource.read();
                    $scope.Cleardata();
                    displayPopupNotification("Data succesfully saved ", "success");
                }
                else {
                    displayPopupNotification(response.data.MESSAGE, "error");
                }
                // this callback will be called asynchronously
                // when the response is available
            }, function errorCallback(response) {
                displayPopupNotification("Something went wrong.Please try again later.", "error");
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
            $scope.Cleardata();


        }
        else {
            var updateUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateAttributeByAttributeCode";
            var model = {
                ATTRIBUTE_CODE: $scope.attributesArr.ATTRIBUTE_CODE,
                ATTRIBUTE_EDESC: $scope.attributesArr.ATTRIBUTE_EDESC,
                ATTRIBUTE_NDESC: $scope.attributesArr.ATTRIBUTE_NDESC,
                PRE_ATTRIBUTE_CODE: $scope.attributesArr.PRE_ATTRIBUTE_CODE,
                ATTRIBUTE_TYPE_FLAG: $scope.attributesArr.ATTRIBUTE_TYPE_FLAG,
                REMARKS: $scope.attributesArr.REMARKS,
                GROUP_SKU_FLAG: $scope.attributesArr.GROUP_SKU_FLAG
            }
            $scope.saveupdatebtn = "Update";
            $http({
                method: 'POST',
                url: updateUrl,
                data: model
            }).then(function successCallback(response) {

                if (response.data.MESSAGE == "UPDATED") {
                    $scope.attributesArr = [];
                    if ($scope.attributesetup.GROUP_SKU_FLAG !== "I") {
                        var tree = $("#attributetree").data("kendoTreeView");
                        tree.dataSource.read();
                    }

                    $scope.Cleardata();
                    var dropdownlist = $("#masterattributecode").data("kendoDropDownList");
                    dropdownlist.dataSource.read();
                    $("#kGrid").data("kendoGrid").dataSource.read();
                    $("#attributeModal").modal("toggle");
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


    $scope.MACDSOptions = {
        dataSource: $scope.MACDS,
        dataTextField: "text",
        dataValueField: "value",

    };

    $scope.masterAttributeCodeOptions = {
        dataSource: $scope.masterAttributeCodeDataSource,
        dataTextField: "text",
        dataValueField: "value",
    };


    $scope.BindGrid = function (groupId) {
        debugger
        $(".topsearch").show();
        var url = null;
        if (groupId == "All") {
            if ($('#regtxtSearchString').val() == null || $('#regtxtSearchString').val() == '' || $('#regtxtSearchString').val() == undefined || $('#regtxtSearchString').val() == 'undefined') {
                alert('Input is empty or undefined.');
                return;
            }
            url = "/api/SetupApi/GetAllAttributeList?searchtext=" + $('#regtxtSearchString').val();
        }
        else {
            $("#regtxtSearchString").val('');
            url = "/api/SetupApi/GetChildOfAttributeByGroup?groupId=" + groupId;
        }

        var reportConfig = GetReportSetting("_attributeSetupPartial");
        $scope.attributeChildGridOptions = {
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
                        ATTRIBUTE_CODE: { type: "string" },
                        ATTRIBUTE_EDESC: { type: "string" },
                        REMARKS: { type: "string" },
                        CREATED_BY: { type: "string" }
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
                    SaveReportSetting('_attributeSetupPartial', 'kGrid');
            },
            columnHide: function (e) {
                if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                    SaveReportSetting('_attributeSetupPartial', 'kGrid');
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
                UpdateReportUsingSetting("_attributeSetupPartial", "kGrid");
                $('div').removeClass('.k-header k-grid-toolbar');
            },
            columns: [
                {
                    //hidden: true,
                    title: "Code",
                    field: "ATTRIBUTE_CODE",
                    width: "80px"

                },
                {
                    field: "ATTRIBUTE_EDESC",
                    title: "Attribute Name",
                    width: "120px"
                },
                {
                    field: "REMARKS",
                    title: "Remarks",
                    width: "120px"
                },

                {
                    field: "CREATED_BY",
                    title: "Created By",
                    width: "120px"
                },

                {

                    title: "Action",
                    template: '<a class="fa fa-pencil-square-o editAction" title="Edit" ng-click="edit(dataItem.ATTRIBUTE_CODE)"><span class="sr-only"></span> </a><a class="fa fa-trash deleteAction" title="Delete" ng-click="delete(dataItem.ATTRIBUTE_CODE,dataItem.GROUP_SKU_FLAG)"><span class="sr-only"></span> </a>',
                    width: "70px"
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

    $scope.showModalForNew = function (event) {
        attributedataFillDefered = $.Deferred();
        $scope.editFlag = "N";
        $scope.saveupdatebtn = "Save";
        $scope.fillAttributeSetupForms($scope.treeselectedAttributeCode);
        $.when(attributedataFillDefered).then(function () {
            $scope.Cleardata();
            var dropdownlist = $("#masterattributecode").data("kendoDropDownList");
            dropdownlist.value($scope.treeselectedAttributeCode);
            $scope.attributesetup.GROUP_SKU_FLAG = "I";
            $scope.attributesArr.GROUP_SKU_FLAG = "I";
            $("#attributeModal").modal("toggle");
            $scope.attributesArr.ATTRIBUTE_CODE = $scope.attributesetup.ATTRIBUTE_CODE;
        }, function errorCallback(response) {
            displayPopupNotification(response.data.ATTRIBUTE_CODE, "error");

        });
    }
    $scope.edit = function (attributeCode) {
        $scope.editFlag = "Y";
        $scope.saveupdatebtn = "Update";
        $scope.fillAttributeSetupForms(attributeCode);
        var dropdownlist = $("#masterattributecode").data("kendoDropDownList");
        dropdownlist.value($scope.treeselectedAttributeCode);

        $("#attributeModal").modal();
    }
    $scope.delete = function (code, groupskuFlag) {
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
                    if (groupskuFlag === "I")
                        $scope.attributesetup.GROUP_SKU_FLAG = "I";
                    else
                        $scope.attributesetup.GROUP_SKU_FLAG = "G"
                    var delUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteAttributeSetupByAttributeCode?attributeCode=" + code;
                    $http({
                        method: 'POST',
                        url: delUrl
                    }).then(function successCallback(response) {

                        if (response.data.MESSAGE == "DELETED") {

                            var tree = $("#attributetree").data("kendoTreeView");
                            tree.dataSource.read();
                            $scope.treenodeselected = "N";
                            var grid = $('#kGrid').data("kendoGrid");
                            grid.dataSource.read();
                            $scope.Cleardata();
                            displayPopupNotification("Data succesfully deleted ", "success");
                        }
                        else if (response.data.MESSAGE == "HAS_CHILD") {
                            displayPopupNotification("You can not delete. It has child.", "warning");
                        }
                        // this callback will be called asynchronously
                        // when the response is available
                    }, function errorCallback(response) {
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                        // called asynchronously if an error occurs
                        // or server returns response with an error status.
                    });

                }

            }
        });
    }
    $scope.addNewAttribute = function () {

        $scope.editFlag = "N";
        $scope.savegroup = true;

        $scope.Cleardata();
        $scope.attributesArr = [];
        $scope.MACDS = [];
        $scope.saveupdatebtn = "Save"
        $scope.groupAttributeTypeFlag = "Y";
        $scope.attributesetup.ATTRIBUTE_TYPE_FLAG = "N";

        $scope.MACDS.push({ text: "<PRIMARY>", value: "" });
        $scope.attributesArr.GROUP_SKU_FLAG = 'G';
        $scope.attributesetup.GROUP_SKU_FLAG = 'G';
        $('#masterattributecode').data('kendoDropDownList').value("");

        $scope.attributesArr.ATTRIBUTE_TYPE_FLAG = "N";

        //$("#childmasterattributecode").data("kendoDropDownList").value("");
        $("#attributeModal").modal("toggle");
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

    $scope.Cleardata = function () {
        $scope.attributesArr.ATTRIBUTE_EDESC = "";
        $scope.attributesArr.ATTRIBUTE_NDESC = "";
        $scope.attributesArr.REMARKS = "";


    }

});

