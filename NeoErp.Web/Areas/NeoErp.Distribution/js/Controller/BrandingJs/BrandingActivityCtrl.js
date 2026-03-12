distributionModule.controller('BrandingActivityCtrl', function ($scope, BrandingService, $routeParams) {

    GetAllActivity();
    var ActivityArr = [];
    var currentRow = "";
    var rowIndex = "";
    $scope.makeItDisabled = false

    var config = {
        getSelectData: "",
        url: window.location.protocol + "//" + window.location.host + "/api/Branding/getBrandingActivityList",
    }

    //angular.element('#treelist').kendoTreeList({
    //    dataSource: new kendo.data.TreeListDataSource({
    //        transport: {
    //            read: {
    //                url: config.url,
    //                dataType: "json"
    //            }
    //        },
    //        schema: {
    //            model: {
    //                id: "MASTER_ACTIVITY_CODE",
    //                parentId: "PARENT_ACTIVITY_CODE",
    //                fields: {
    //                    MASTER_ACTIVITY_CODE: { field: 'MASTER_ACTIVITY_CODE', type: "number", nullable: false, defaultvalue: "0" },
    //                    PARENT_ACTIVITY_CODE: { field: "PARENT_ACTIVITY_CODE", type: "number" }
    //                }
    //            }
    //        }
    //    }),
    //    dataBound: function (evt) {
    //        GetSetupSetting("BrandingActivity");
    //    },
    //    resizable: true,
    //    sortable: true,
    //    filterable: {
    //        extra: false,
    //        operators: {
    //            number: {
    //                eq: "Is equal to",
    //                neq: "Is not equal to",
    //                gte: "is greater than or equal to	",
    //                gt: "is greater than",
    //                lte: "is less than or equal",
    //                lt: "is less than",
    //            },
    //            string: {

    //                eq: "Is equal to",
    //                neq: "Is not equal to",
    //                startswith: "Starts with	",
    //                contains: "Contains",
    //                doesnotcontain: "Does not contain",
    //                endswith: "Ends with",
    //            },
    //            date: {
    //                eq: "Is equal to",
    //                neq: "Is not equal to",
    //                gte: "Is after or equal to",
    //                gt: "Is after",
    //                lte: "Is before or equal to",
    //                lt: "Is before",
    //            }
    //        }
    //    },
    //    columns: [
    //        { field: "ACTIVITY_EDESC", expandable: true, title: "Name" },
    //        { field: "ACTIVITY_TYPE", title: "Activity Type" },
    //        { field: "REMARKS", title: "Remarks" },
    //    ],
    //});

    angular.element('#treelist').kendoTreeList({
        dataSource: new kendo.data.TreeListDataSource({
            transport: {
                read: {
                    url: config.url,
                    dataType: "json"
                }
            },
            schema: {
                model: {
                    id: "ACTIVITY_CODE",
                    parentId: "MASTER_ACTIVITY_CODE",
                    fields: {
                        ACTIVITY_CODE: { type: "number", nullable: false },
                        MASTER_ACTIVITY_CODE: { field: "MASTER_ACTIVITY_CODE", type: "number", nullable: true }
                    }
                }
            }
        }),

        dataBound: function (e) {
            GetSetupSetting("UserSetUpTree");
        },
        editable: {
            move: true
        },
        dragstart: function (e) {
            if (e.source.IS_GROUP == 'Y')
                e.preventDefault();
        },
        drop: function (e) {
            $scope.UpdateUserTreeOrder(e);
        },
        columns: [
            { field: "ACTIVITY_EDESC", expandable: true, title: "Name" },
            { field: "ACTIVITY_TYPE", title: "Activity Type" },
            { field: "REMARKS", title: "Remarks" },
        ],
    });


    $scope.preMenuItem = [];
    angular.element('#menu').kendoContextMenu({
        target: "#treelist",
        filter: "tbody > tr",
        select: function (e) {
            var button = $(e.item);
            var row = $(e.target);
            var dataItem = $("#treelist").data("kendoTreeList").dataItem(row);
            ActivityArr = dataItem;
            if (button.text() == "Update") {
                debugger;
                $scope.pageName = "Update Branding Activity";
                $scope.saveButtonText = "Update";

                var acitvityCode = dataItem.MASTER_ACTIVITY_CODE;
                $scope.Activity = dataItem.ACTIVITY_EDESC;
                $("#barndingActivityType").val(dataItem.ACTIVITY_TYPE).trigger('change');
                $("#groupActivityFlag").val(dataItem.GROUP_ACTIVITY_FLAG).trigger('change');
                $('#ParentMultiSelect').val(acitvityCode);


                //$("#ParentMultiSelect option[value='" + dataItem.PARENT_ACTIVITY_CODE+"']").prop("selected", !0).change();
                //$('#ParentMultiSelect').val(dataItem.MASTER_ACTIVITY_CODE).hide();
                //setTimeout(function () {
                //    $("#ParentMultiSelect option[value='" + dataItem.PARENT_ACTIVITY_CODE + "']").prop("selected", !0).change();
                //}, 1500);
                selectRoots(dataItem.MASTER_ACTIVITY_CODE);
                var data = $("#ParentMultiSelect option");
                $.each(data, function (key, val) {
                    if (val.value === dataItem.MASTER_ACTIVITY_CODE)
                        $(this).css({ display: 'none' });
                })
                // $($("#ParentMultiSelect option")[$("#ParentMultiSelect option").length - 1]).css({ display: 'none' });
                $scope.Remarks = dataItem.REMARKS;
                angular.element('#barndingActivityType').data("kendoMultiSelect").value(dataItem.ACTIVITY_TYPE);
                angular.element('#groupActivityFlag').data("kendoMultiSelect").value(dataItem.GROUP_ACTIVITY_FLAG);
                angular.element('#brandingActivityCreateModel').modal('show');
                $scope.preMenuItem = _.reject(_.reject(config.getSelectData, function (evt) { return evt.PARENT_ACTIVITY_CODE == _.filter(config.getSelectData, function (x) { return x.ACTIVITY_EDESC == $("#Activity").val() })[0].MASTER_ACTIVITY_CODE }), function (y) { return y.ACTIVITY_EDESC == $("#Activity").val() })
                $scope.makeItDisabled = true;

            }

            else if (button.text() == "Add") {
                Clear();
                var GROUP_ACTIVITY_FLAG = $('#groupActivityFlag').val();
                if (dataItem.GROUP_ACTIVITY_FLAG == 'N') {

                    displayPopupNotification("You can not add child to this node", "warning");
                }
                else {


                 
                    $scope.pageName = "Add Branding Activity";
                    $scope.saveButtonText = "Save";
                    angular.element('#brandingActivityCreateModel').modal('show');
                    $('#ParentMultiSelect').val(acitvityCode);
                    $scope.preMenuItem = _.filter(config.getSelectData, function (evt) { return evt.MASTER_ACTIVITY_CODE == dataItem.MASTER_ACTIVITY_CODE })
                    $scope.makeItDisabled = true;
                    //selectRoots(dataItem.MASTER_ACTIVITY_CODE

                    var acitvityCode = dataItem.ACTIVITY_CODE;
                    selectRoots(dataItem.ACTIVITY_CODE);
                    var data = $("#ParentMultiSelect option");
                    $.each(data, function (key, val) {
                        if (val.value === dataItem.ACTIVITY_CODE)
                            $(this).css({ display: 'none' });
                    })

                }


            }


            else if (button.text() == "Delete") {
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
                            var TYPE_ID;
                            ActivityArr;
                            var ACTIVITY_CODE = ActivityArr.ACTIVITY_CODE;
                            var PRE_ACTIVITY_CODE = ActivityArr.parentId;

                            var data = {
                                ACTIVITY_CODE: ACTIVITY_CODE,
                                PRE_ACTIVITY_CODE: PRE_ACTIVITY_CODE
                            };

                            var deleteData = BrandingService.deleteActivity(data);
                            deleteData.then(function (result) {
                                if (result.data === 'deleted') {
                                    displayPopupNotification("Deleted Successfully", "success");
                                    angular.element('#brandingActivityCreateModel').modal('hide');
                                    angular.element('#treelist').data("kendoTreeList").dataSource.read();
                                    GetAllActivity();
                                }
                                else {
                                    displayPopupNotification("Something went wrong", "warning")
                                }
                            });
                        }
                    }
                });
            }
            function selectRoots(currentData) {
                setTimeout(function () {
                    $("#ParentMultiSelect option[value='" + currentData + "']").prop("selected", !0).change();
                }, 500);
            }
            $scope.$apply();
        }
    });

    $scope.openModal = function () {
        Clear();
        $('#ParentMultiSelect').val($('#ParentMultiSelect option')[0].value).trigger('change');
        $scope.saveButtonText = "Save";
        $scope.pageName = "Add Branding Activity"
        $scope.preMenuItem = config.getSelectData;
        angular.element('#brandingActivityCreateModel').modal('show');
        $scope.makeItDisabled = true;
    }

    $scope.saveActivity = function (isValid) {
        debugger;
        if (!isValid) {
            displayPopupNotification("Invalid Field", "warning");
            return;
        }
        var ACTIVITY_EDESC = $scope.Activity;
        var ACTIVITY_TYPE = $('#barndingActivityType').val()[0];
        var GROUP_ACTIVITY_FLAG = $('#groupActivityFlag').val()[0];
        var MASTER_ACTIVITY_CODE = $('#ParentMultiSelect').val();
        var REMARKS = $scope.Remarks;
        var ACTIVITY_CODE = ActivityArr.ACTIVITY_CODE;

        var data = {
            ACTIVITY_EDESC: ACTIVITY_EDESC,
            ACTIVITY_TYPE: ACTIVITY_TYPE,
            GROUP_ACTIVITY_FLAG: GROUP_ACTIVITY_FLAG,
            MASTER_ACTIVITY_CODE: MASTER_ACTIVITY_CODE,
            REMARKS: REMARKS,
            ACTIVITY_CODE: ACTIVITY_CODE,
        }
        if ($scope.saveButtonText == "Update") {
            debugger;
            var updateData = BrandingService.updateBrandingActivity(data);
            updateData.then(function (response) {
                if (response.data == "update") {
                    displayPopupNotification("update Successfully", "success");
                    angular.element('#brandingActivityCreateModel').modal('hide');
                    GetSelectedNodeElement();
                    angular.element('#treelist').data("kendoTreeList").dataSource.read();
                    ExpandAllNode();
                    GetAllActivity();
                }
                else if (response.data == "denied") {
                    displayPopupNotification("Access Denied !", "warning")
                }
                else {
                    displayPopupNotification("Something Went wrong", "error");
                }
            });
        }
        else {
            var Getdata = BrandingService.saveBrandingActivity(data)
            Getdata.then(function (response) {
                if (response.data == "success") {
                    displayPopupNotification("Insert Successfully", "success");
                    angular.element('#brandingActivityCreateModel').modal('hide');
                    GetSelectedNodeElement();
                    angular.element('#treelist').data("kendoTreeList").dataSource.read();
                    ExpandAllNode();
                    GetAllActivity();
                }
                else if (response.data == "denied") {
                    displayPopupNotification("Access Denied !", 'warning');
                }
                else {
                    displayPopupNotification("Something Went wrong", "error");
                }
            });
        }
    }

    function GetAllActivity() {
        var GetAllActivity = BrandingService.GetAllActivity();
        GetAllActivity.then(function (response) {
            config.getSelectData = response.data;
        });
    }

    function Clear() {
        $scope.Activity = "";
        $scope.barndingActivityType = "";
        $scope.groupActivityFlag = "";
        angular.element('#groupActivityFlag').data("kendoMultiSelect").value([]);
        angular.element('#barndingActivityType').data("kendoMultiSelect").value([]);
        $scope.Remarks = "";
    }

    $("#treelist").on("mousedown", "tr[role='row']", function (e) {
        if (e.which === 3) {
            $('tr.k-state-selected', '#treelist').removeClass('k-state-selected');
            $(this).addClass("k-state-selected");
           
        }
    });

    $("#barndingActivityType").kendoMultiSelect({
        animation: {
            open: {
                effects: "zoom:in",
                duration: 300
            }
        },
        placeholder: "Please Select..."
    });
    $("#groupActivityFlag").kendoMultiSelect({
        animation: {
            open: {
                effects: "zoom:in",
                duration: 300
            }
        },
        placeholder: "Please Select..."
    });
    $("#barndingActivityType").data("kendoMultiSelect").options.maxSelectedItems = 1;
    $("#groupActivityFlag").data("kendoMultiSelect").options.maxSelectedItems = 1;

    function ExpandAllNode() {

        setTimeout(function () {
            var treeList = $("#treelist").data("kendoTreeList");
            var crow = currentRow;
            var rIndex = rowIndex;
            $(crow[rIndex]).addClass('').css('background-color', 'hsl(22, 100%, 48%)')
            // 'M' for selected node index
            // 'd' for above selected node and search for the super parent, expand it and continue with the M index nodes 
            //Confused? connect skype dushant.kunwar
            for (i = 0; i < crow.length; i++) {

                if (i == rIndex) {

                    for (x = rIndex - 1; x < crow.length; x--) {

                        if (x >= 0) {

                            if (crow[x].className.includes('treelist-group')) {
                                var expandParent = crow[x];
                                treeList.expand(expandParent);
                            }
                            else {
                                for (z = rIndex; z < crow.length; z++) {
                                    if (crow[z].className.includes('treelist-group')) {
                                        var expandChildNode = crow[z];
                                        treeList.expand(expandChildNode);
                                    }
                                    else {
                                        for (d = rIndex - 1; d < crow.length; d--) {
                                            if (d >= 0) {
                                                if ($(crow[d]).prop('style').cssText != "display: none;" && crow[d].className.includes('k-treelist-group')) {
                                                    var superParent = crow[d];
                                                    treeList.expand(superParent);
                                                    for (m = d + 1; m < crow.length; m++) {

                                                        if (crow[m].className == "") {
                                                            $.noop()
                                                        }
                                                        else if (crow[m].className.includes('treelist-group')) {
                                                            var expandChildNode = crow[m];
                                                            treeList.expand(expandChildNode);
                                                        }
                                                        else {
                                                            return false;
                                                        }
                                                    }
                                                }
                                            }
                                            //if inedx is less than 0
                                            else {
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else {
                            for (z = rIndex; z < crow.length; z++) {

                                if (crow[z].className.includes('treelist-group')) {
                                    var expandChildNode = crow[z];
                                    treeList.expand(expandChildNode);
                                }
                                else {
                                    return false;
                                }

                            }
                        }
                    }
                }
            }

        }, 500);
    }

    function GetSelectedNodeElement() {
        var crow = currentRow;
        var rIndex = rowIndex
        $(crow[rIndex]).addClass('').css('background-color', '')
        var treeList = $("#treelist").data("kendoTreeList");
        currentRow = treeList.tbody[0].childNodes;
        for (i = 0; i < currentRow.length; i++) {
            if (currentRow[i].className.includes("state-selected")) {
                rowIndex = currentRow[i].rowIndex;
                return false;
            };
        };
    }
});


