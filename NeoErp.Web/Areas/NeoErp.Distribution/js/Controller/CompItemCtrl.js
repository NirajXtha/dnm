
distributionModule.controller('CompItemCtrl', function ($scope, DistSetupService, $http, $routeParams) {

    $scope.pageName = "Add Item";
    $scope.saveAction = "Save";
    $scope.CategoryAutoComplete = [];

    $scope.$on('$routeChangeSuccess', function () {
        DistSetupService.GetCategories().then(function (response) {
            $scope.CategoryAutoComplete = response.data;
        }, function (response) {
            displayPopupNotification("Something went wrong", "error");
        });
    });

    //create CompItem
    $scope.CompItemSetup = {
        ITEM_ID: null,
        ITEM_EDESC: null,
        RATE: null,
        UNIT: null,
    };
    
    $scope.AddButtonClickEvent = function () {
        $scope.Cancel();
        $scope.createPanel = true;
    }
  
    $scope.SaveItem = function (isValid) {
        if (isValid == false) {
            displayPopupNotification("Invalid Input fields", "warning");
            return;
        }
       
        DistSetupService.CreateCompItem($scope.CompItemSetup).then(function (response) {
            displayPopupNotification(response.data.MESSAGE, response.data.TYPE);
            if (response.data.TYPE == "success") {
                $("#grid").data("kendoGrid").dataSource.read();
                $scope.Cancel();
            }
        }, function (response) {
            displayPopupNotification("Something went wrong", "error");
        });
    }

    $scope.createPanel = false;
    
    //grid
    var reportConfig = GetReportSetting("CompItemSetup");
    $scope.grid = {
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetCompItem",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8"
                },
            },
            error: function (e) {
                displayPopupNotification("Sorry error occured while processing data", "error");
            },
            pageSize: reportConfig.defaultPageSize,
        },
        toolbar: kendo.template($("#toolbar-template").html()),
        excel: {
            fileName: "CompItem Setup",
            allPages: true,
        },
        height: window.innerHeight - 50,
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
        //columnMenuInit: function (e) {
        //    wordwrapmenu(e);
        //    checkboxItem = $(e.container).find('input[type="checkbox"]');
        //},
        //columnShow: function (e) {
        //    if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
        //        SaveReportSetting('CompItemSetup', 'grid');
        //},
        //columnHide: function (e) {
        //    if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
        //        SaveReportSetting('CompItemSetup', 'grid');
        //},
        pageable: {
            refresh: true,
            pageSizes: reportConfig.itemPerPage,
            buttonCount: 5
        },
        dataBound: function (o) {
            GetSetupSetting("CompItemSetup");
            var grid = o.sender;
            if (grid.dataSource.total() == 0) {
                var colCount = grid.columns.length;
                $(o.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                displayPopupNotification("No Data Found.", "info");
            }
            else {
                var g = $("#grid").data("kendoGrid");
                for (var i = 0; i < g.columns.length; i++) {
                    g.showColumn(i);
                }
                $("div.k-group-indicator").each(function (i, v) {
                    g.hideColumn($(v).data("field"));
                });
            }

            UpdateReportUsingSetting("CompItemSetup", "grid");
            $('div').removeClass('.k-header k-grid-toolbar');
        },
        columns: [
            {
                field: "ITEM_EDESC",
                title: "Item Name",
                width: "50%",
            },
            {
                field: "RATE",
                title: "Rate",
                width: "10%",
                attributes: {
                    style: "text-align:right"
                }
            },
            {
                field: "UNIT",
                title: "Unit",
                width: "10%",
            },
            {
                field: "CATEGORY",
                title: "Category",
                width: "10%",
            },
            {
                title: "Actions",
                width: "10%",
                template: " <a class='fa fa-edit editAction' ng-click='UpdateItem(#:ITEM_ID#)' title='Edit'></a>&nbsp &nbsp<a class='fa fa-trash-o deleteAction' ng-click='deleteArea(#:ITEM_ID#)' title='Delete'></a>  ",

            }
        ]
    };

    $scope.UpdateItem = function (id) {
        var gridDs = $("#grid").data("kendoGrid").dataSource.data();
        var items = _.filter(gridDs, function (x) { return x.ITEM_ID == id });
        $scope.CompItemSetup = angular.copy(items[0]);
        $scope.pageName = "Update Item";
        $scope.saveAction = "Update";
        $scope.createPanel = true;
    }

    $scope.deleteArea = function (id) {
        if (id) {
            bootbox.confirm("<strong>This item will be deleted</strong><br /> Are you sure want to delete?", function (result) {
                if (result) {
                    DistSetupService.DeleteCompItem(id).then(function (res) {
                        displayPopupNotification(res.data.MESSAGE, res.data.TYPE);
                        if (res.data.TYPE == "success") {
                            $scope.Cancel();
                            $("#grid").data("kendoGrid").dataSource.read();
                        }
                    }, function (ex) {
                        displayPopupNotification("Error processing request", "error");
                    });
                }
            });
        }
    }
    
    $scope.Cancel = function () {
        $scope.createPanel = false;
        $scope.pageName = "Add Item";
        $scope.saveAction = "Save";
        $scope.CompItemSetup = {
            ITEM_ID: null,
            ITEM_EDESC: null,
            RATE: null,
            UNIT: null,
        };
    }

});
