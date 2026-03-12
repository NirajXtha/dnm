distributionModule.controller('ItemSetupCtrl', function ($scope, DistSetupService) {
    debugger;
    $scope.showForm = false;
    reportConfig = GetReportSetting("ItemSetup");

    $scope.Item = {
        ITEM_CODE: [],
        ITEM_EDESC: '',
        CATEGORY_CODE: [{
            "CATEGORY_CODE": "FG", "CATEGORY_EDESC": "Finish Goods"
        }],
        CATEGORY_EDESC: '',
        MU_CODE: null,
        BRAND_NAME: '',
        SALES_RATE: '',
        IMAGE_FILE_NAME: '',//NS
    }

    $scope.MuOptions = {
        dataTextField: "Name",
        dataValueField: "Code",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Unit...</strong></div>',
        placeholder: "Select unit...",
        autoClose: true,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetMuList",
                    dataType: "json"
                }
            }
        }
    }


    $scope.CatOptions = {
        dataTextField: "CATEGORY_EDESC",
        dataValueField: "CATEGORY_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Category...</strong></div>',
        placeholder: "Select category...",
        autoClose: true,
        dataBound: function (e) {
            var current = this.value();

            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetCategoryList",
                    dataType: "json"
                }
            }
        }
    }
    //NS
    $scope.configureImageForUpload = function (files) {
        debugger;
        $scope.Item.Image = files;
    }
    //NS
    $scope.CreateItem = function () {
        debugger;
        if (!$scope.ItemForm.$valid) {
            displayPopupNotification("Invalid input fields", "warning");
            return;
        }

        $scope.Item.MU_CODE = $scope.Item.MU_CODE[0];
        $scope.Item.CATEGORY_CODE = $scope.Item.CATEGORY_CODE[0] + $scope.Item.CATEGORY_CODE[1];

        // MU_CODE: assume it's an array or object, extract code
        if (Array.isArray($scope.Item.MU_CODE)) {
            $scope.Item.MU_CODE = $scope.Item.MU_CODE[0];
        }

        var formData = new FormData();
        formData.append("model", JSON.stringify($scope.Item));

        if ($scope.Item.Image && $scope.Item.Image.length > 0) {
            formData.append("file", $scope.Item.Image[0]);
        }

        $.ajax({
            type: "POST",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/SaveItem",
            data: formData,
            processData: false,
            contentType: false,
            success: function (res) {
                if (res.TYPE === 'success') {
                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.Cancel(); // If this relies on Angular's digest, you might need $scope.$apply()
                } else {
                    displayPopupNotification(res.MESSAGE, res.TYPE);
                }
            },
            error: function () {
                displayPopupNotification("Error uploading item", "error");
            }
        });
    };

    /*//AA
    $scope.CreateItem = function () {
        debugger;
        if (!$scope.ItemForm.$valid) {
            displayPopupNotification("Invalid input fileds", "warning");
            return;
        }
        $scope.Item.MU_CODE = $scope.Item.MU_CODE[0];
        $scope.Item.CATEGORY_CODE = $scope.Item.CATEGORY_CODE[0] + $scope.Item.CATEGORY_CODE[1];

        DistSetupService.SaveItem($scope.Item).then(
            function (res) {
                if (res.data.TYPE == 'success') {
                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                }
                else
                    $scope.Item.MU_CODE[0] = $scope.Item.MU_CODE;
                $scope.Item.CATEGORY_CODE[0] = $scope.Item.CATEGORY_CODE;
                displayPopupNotification(res.data.MESSAGE, res.data.TYPE);
            },
            function (ex) {
                displayPopupNotification("Error", "error");
            });
    }*/

    $scope.AddClickEvent = function () {
        $scope.showForm = true;
    }

    $scope.Cancel = function () {
        $scope.Item = {
            ITEM_CODE: '',
            ITEM_EDESC: '',
            CATEGORY_CODE: [{
                "CATEGORY_CODE": "FG", "CATEGORY_EDESC": "Finish Goods"
            }],
            CATEGORY_EDESC: '',
            MU_CODE: null,
            BRAND_NAME: '',
            SALES_RATE: '',
            IMAGE_FILE_NAME: ''
        }
        //New NS
        $scope.Item.Image = null;
        document.getElementById("ItemImage").value = "";
        //New end NS
        $scope.showForm = false;
    }

    $scope.grid = {
        dataSource: {
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/Setup/GetItems",
            },
            model: {
                fields: {
                    SALES_RATE: { type: "number" },
                }
            },
            pageSize: 500,
        },
        height: window.innerHeight - 50,
        groupable: true,
        sortable: true,
        pageable: {
            refresh: true,
            pageSizes: reportConfig.itemPerPage,
            buttonCount: 5
        },
        scrollable: {
            virtual: true
        },
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
        toolbar: kendo.template($("#toolbar-template").html()),
        dataBound: function (o) {
            var grid = o.sender;
            if (grid.dataSource.total() == 0) {
                var colCount = grid.columns.length;
                $(o.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                displayPopupNotification("No Data Found.", "info");
            }
            UpdateReportUsingSetting("ItemSetup", "grid");
            $('div').removeClass('.k-header k-grid-toolbar');
        },
        columnMenu: true,
        excel: {
            fileName: "Items",
            allPages: true,
        },
        pdf: {
            fileName: "Received Schedule",
            allPages: true,
            avoidLinks: true,
            pageSize: "auto",
            margin: {
                top: "2m",
                right: "1m",
                left: "1m",
                buttom: "1m",
            },
            landscape: true,
            repeatHeaders: true,
            scale: 0.8,
        },
        columns: [{
            field: "ITEM_EDESC",
            title: "Item Name",
        }, {
            field: "CATEGORY_CODE",
            title: "Category",
        }, {
            field: "MU_CODE",
            title: "Unit",
        }, {
            field: "BRAND_NAME",
            title: "Brand",
        }, {
            field: "SALES_RATE",
            title: "Rate",
            attributes: {
                style: "text-align: right;"
            },
            format: "{0:n}",
            },{ //NS
                field: "IMAGE_FILE_NAME",
                title: "Image",
                width: "120px",
                attributes: { style: "text-align: center;" },
                template: function (dataItem) {
                    if (dataItem.IMAGE_FILE_NAME) {
                        return "<img src='/Areas/NeoErp.Distribution/Images/ItemImages/" + dataItem.IMAGE_FILE_NAME + "' style='height: 50px; width: 50px; object-fit: cover; border-radius: 4px;' />";
                    } else {
                        return "<span class='text-muted'>No Image</span>";
                    }
                }
            }, {
            title: "Actions",
            width: "10%",
            attributes: {
                style: "text-align: center;"
            },
            template: " <a class='fa fa-edit editAction' ng-click='UpdateItem(#:ITEM_CODE#)' title='Edit'>", //</a>&nbsp &nbsp<a class='fa fa-trash-o deleteAction' ng-click='deleteItem(#:ITEM_CODE#)' title='Delete'></a>  

        }]
    }

    $scope.UpdateItem = function (itemCode) {
        debugger;
        var gridDs = $("#grid").data("kendoGrid").dataSource.data();
        var items = _.filter(gridDs, function (x) { return x.ITEM_CODE == itemCode });
        items[0].MU_CODE = [items[0].MU_CODE]; //because the mu_code is multiselect and requires array value
        items[0].SALES_RATE = parseFloat(items[0].SALES_RATE); //to remove number format error in angularjs
        $scope.Item = angular.copy(items[0]);
        // New -> For Image NS
        $scope.Item.Image = null;
        document.getElementById("ItemImage").value = "";
        //End New NS
        $scope.showForm = true;
    }

});