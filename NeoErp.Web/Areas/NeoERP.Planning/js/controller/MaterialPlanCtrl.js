planningModule.controller('MaterialPlanCtrl', function ($scope, planservice, $routeParams, $rootScope, $http) {
    $scope.pageName = "Material Plan";

    $scope.visibleSavebtn = false;
    $scope.visibleGenerateBtn = true;
    $scope.saveUpdateBtn = "Save";

    $scope.PLAN_CODE = $routeParams.plancode;

    $scope.fgproductvalue = [];
    $scope.materialPlan = {
        PLAN_EDESC: "",
        PLAN_DATE: "",
        PLAN_NDATE: AD2BS(moment().format('YYYY-MM-DD'))
    }
    $scope.GoodsQty = [{
        FG_ITEM_CODE: "",
        QTY: 1
    }];

    $scope.checkUniqFIs = function () {
        if ($scope.GoodsQty.length > 1) {
            var gqLen = $scope.GoodsQty.length;
            var unqLen = _.uniq($scope.GoodsQty, "FG_ITEM_CODE");
            if (gqLen != unqLen) {
                angular.forEach($scope.GoodsQty, function (i, v) {
                    if (i > 0) {
                        v.FG_ITEM_CODE = "";
                        v.QTY = 1;
                    }
                });
            }
        }
    }
    $scope.AddGoodsNQty = function (index) {
        $scope.GoodsWithQty = {
            FG_ITEM_CODE: "",
            QTY: 1
        }
        $scope.GoodsQty.push($scope.GoodsWithQty);
        $scope.visibleSavebtn = false;
        $scope.visibleGenerateBtn = true;
        $("#grid").empty();
        $scope.gbClickInfo();
    };
    $scope.RemoveGoodsNQty = function (index) {
        if ($scope.GoodsQty.length > 1) {
            $scope.GoodsQty.splice(index, 1);
            $scope.visibleSavebtn = false;
            $scope.visibleGenerateBtn = true;
            $("#grid").empty();
            $scope.gbClickInfo();
        }
    }
    $scope.gbClickInfo = function () {
        if ($("#grid").html() == "") {
            $(".helptext").html("");
            $("#grid").removeClass('k-grid k-widget k-display-block k-reorderable');
            if ($("#grid").data("kendoGrid") != undefined)
                $(".helptext").html('<span class="grid-info" style="color:#327ad5;">* Please click the generate button to generate the material plan according to selected product and quantity.</span>');
        }
        else {
            $("#grid").addClass('k-grid k-widget k-display-block k-reorderable');
            $(".grid-info").css({ display: "none" });
        }
    }

    $scope.removeSelectedItem = function (index) {
        var comboBox = $("#fgProducts_" + index).data("kendoComboBox");
        comboBox.value([]);
    }
    $scope.validation = function () {
        var valid = true;
        var productValidation = $.grep($scope.GoodsQty, function (b) {
            return b.FG_ITEM_CODE === "" || b.FG_ITEM_CODE == null;
        });
        if (productValidation.length > 0) {
            displayPopupNotification("Please choose the product", "error");
            valid = false;
        }
        if ($("#nepaliDate5").val() == "") {
            displayPopupNotification("Please Choose the date", "error");
            valid = false;
        }
        return valid;
    }
    $scope.clearAllField = function () {
        $scope.GoodsQty = [{
            FG_ITEM_CODE: "",
            QTY: 1
        }];
        $("#grid").empty();
        $scope.gbClickInfo();
        $("#nepaliDate5").val(AD2BS(moment().format('YYYY-MM-DD')));
        $scope.visibleSavebtn = false;
        $scope.visibleGenerateBtn = true;
    }

    $scope.qtyChangeEvent = function () {
        $("#grid").empty();
        $scope.gbClickInfo();
        $scope.visibleSavebtn = false;
        $scope.visibleGenerateBtn = true;
    }
    $scope.generatePlan = function (isValid) {
        var valid = false;
        if (isValid) {
            valid = $scope.validation();
        }
        else if (!isValid) {
            displayPopupNotification("There is an invalid field.", "error");
        }
        else {
            return displayPopupNotification("There is an invalid data", "error");
        }
        if (valid) {
            showloader();
            var param = {
                mpList: $scope.GoodsQty
            }
            createGrid(param);

            $scope.visibleSavebtn = true;
            $scope.visibleGenerateBtn = false;
            $scope.gbClickInfo();
        }
    }


    $scope.editMaterialPlan = function () {
        var editUrl = window.location.protocol + "//" + window.location.host + "/Api/MaterialPlanApi/GetMaterialPlanDetailByPlanCode?planCode=" + $scope.PLAN_CODE;
        $.ajax({
            type: 'POST',
            url: editUrl,
            async: false,
            dataType: 'json',
            beforeSend: function () {
                showloader();
            },
            complete: function () {
                setTimeout(function () {
                    $scope.generatePlan(true);
                    //hideloader();
                }, 10);
            },
            success: function (data) {
                $scope.GoodsQty = [];
                $scope.materialPlan.PLAN_EDESC = data[0].PLAN_EDESC;
                $scope.materialPlan.PLAN_NDATE = AD2BS(moment(data[0].PLAN_DATE).format('YYYY-MM-DD'));
                angular.forEach(data, function (i, v) {
                    $scope.GoodsWithQty = {
                        FG_ITEM_CODE: i.FINISHED_ITEM_CODE,
                        QTY: i.MATERIAL_QUANTITY
                    }
                    $scope.GoodsQty.push($scope.GoodsWithQty);
                });
            },
            error: function (error) {
                if (error.responseText == "constraint") {
                    displayPopupNotification("Plan already created.", "warning");
                }
                else {
                    displayPopupNotification(error, "error");
                }
            }
        });
    }
    if ($scope.PLAN_CODE != undefined) {
        $scope.saveUpdateBtn = "Update";
        $scope.editMaterialPlan();
    }

    var grid,
        createGrid = function (param) {
            grid = $("#grid").kendoGrid({
                dataSource: {
                    type: "json",
                    transport: {
                        read: {
                            url: window.location.protocol + "//" + window.location.host + "/api/MaterialPlanApi/GetAllRawMaterialByFinishGood",
                            dataType: "json", // <-- The default was "jsonp".
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                        },
                        parameterMap: function (options, type) {
                            var paramMap = JSON.stringify($.extend(options, param));
                            delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                            delete paramMap.$format; // <-- remove format parameter.
                            return paramMap;
                        }
                    },
                    error: function (e) {
                        displayPopupNotification(e.errorThrown, "error");
                        $scope.visibleSavebtn = false;
                        $scope.visibleGenerateBtn = true;
                        $scope.$apply();
                    },
                    group: [{
                        field: "FINISHED_ITEM_EDESC",
                    }, {
                        field: "CATEGORY_EDESC"
                    }],
                    requestEnd: function () {
                        hideloader();
                    },
                    schema: {
                        model: {
                            fields: {
                                CATEGORY_EDESC: { type: "string" },
                            }
                        }
                    },
                },
                height: window.innerHeight - 300,
                reorderable: true,
                groupable: false,
                resizable: true,
                //filterable: {    // filter for the null and is not null etc
                //    extra: false,// extra false means there is 2 different filter inside the filter
                //    operators: {   // the number is data type for the net sales column , and string for the MITI
                //        number: {

                //            eq: "Is equal to",
                //            neq: "Is not equal to",
                //            gte: "is greater than or equal to	",
                //            gt: "is greater than",
                //            lte: "is less than or equal",
                //            lt: "is less than",

                //        },
                //        string: {

                //            eq: "Is equal to",
                //            neq: "Is not equal to",
                //            startswith: "Starts with	",
                //            contains: "Contains",
                //            doesnotcontain: "Does not contain",
                //            endswith: "Ends with",
                //        },
                //        date: {

                //            eq: "Is equal to",
                //            neq: "Is not equal to",
                //            gte: "Is after or equal to",
                //            gt: "Is after",
                //            lte: "Is before or equal to",
                //            lt: "Is before",
                //        }
                //    }
                //},
                columnMenu: false,
                dataBinding: function () {
                    record = (this.dataSource.page() - 1) * this.dataSource.pageSize();
                },
                dataBound: function (o) {
                    var grid = o.sender;
                    if (grid.dataSource.total() == 0) {
                        var colCount = grid.columns.length;
                        $(o.sender.wrapper)
                            .find('tbody')
                            .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                        displayPopupNotification("No Data Found.", "info");
                        $scope.visibleSavebtn = false;
                        $scope.visibleGenerateBtn = true;
                        $scope.$apply();
                    }
                    //else {
                    //    var g = $("#grid").data("kendoGrid");
                    //    for (var i = 0; i < g.columns.length; i++) {
                    //        g.showColumn(i);
                    //    }
                    //    $("div.k-group-indicator").each(function (i, v) {
                    //        g.hideColumn($(v).data("field"));
                    //    });
                    //}
                },
                columns: [
                    //{ title: "SN", template: "#= ++record #", width: "15px" },
                    //{
                    //    field: "ITEM_CODE",
                    //    title: "Item",
                    //    width: "70px",
                    //    hidden:true
                    //},
                    {
                        field: "ITEM_EDESC",
                        title: "Item",
                        width: "200px"
                    },
                    {
                        field: "CATEGORY_EDESC",
                        title: "Category",
                        hidden: true,
                        groupHeaderTemplate: "#=value#"
                    },
                    {
                        field: "FINISHED_ITEM_EDESC",
                        title: "Product",
                        hidden: true,
                        groupHeaderTemplate: "#=value#"
                    },
                    {
                        field: "INDEX_MU_CODE",
                        title: "Unit",
                        width: "100px",
                    },
                    //{
                    //    field: "FINISHED_ITEM_EDESC",
                    //    title: "Index Item",
                    //    hidden: true
                    //},
                    //{
                    //    field: "ACTUAL_REQUIRED_QUANTITY",
                    //    title: "Single Item RQ",
                    //    format: '{0:0.00}',
                    //    template: "<span class='ACTUAL_REQUIRED_QUANTITY' style='text-align:right;margin-right: 7px;'>#=kendo.toString(ACTUAL_REQUIRED_QUANTITY,'n2')#</span>",
                    //    //width: "135px",
                    //    attributes: {
                    //        style: "text-align: right;"
                    //    },
                    //},
                    //{
                    //    field: "REQUIRED_QUANTITY",
                    //    title: "RQ per item",
                    //    format: '{0:0.00}',
                    //    template: "<span class='REQUIRED_QUANTITY' style='text-align:right;margin-right: 7px;'>#=kendo.toString(REQUIRED_QUANTITY,'n2')#</span>",
                    //    //width: "135px",
                    //    attributes: {
                    //        style: "text-align: right;"
                    //    },
                    //},
                    {
                        field: "CALC_QTY",
                        title: "Required Quantity",
                        width: "145px",
                        format: '{0:0.00}',
                        template: "<span class='CALC_QTY' style='text-align:right;margin-right: 7px;'>#=kendo.toString(CALC_QTY,'n2')#</span>",
                        attributes: {
                            style: "text-align: right;"
                        },
                    },
                    {
                        field: "STOCK",
                        title: "Available Stock",
                        width: "145px",
                        format: '{0:0.00}',
                        template: "<span class='STOCK' style='text-align:right;margin-right: 7px;'>#=kendo.toString(STOCK,'n2')#</span>",
                        attributes: {
                            style: "text-align: right;"
                        },
                    },
                    {
                        field: "PO_PENDING",
                        title: "PO In Transit",
                        width: "145px",
                        format: '{0:0.00}',
                        template: "<span class='PO_PENDING' style='text-align:right;margin-right: 7px;'>#=kendo.toString(PO_PENDING,'n2')#</span>",
                        attributes: {
                            style: "text-align: right;"
                        },
                    }, {
                        field: "REMAINING_QTY",
                        title: "PO",
                        format: '{0:0.00}',
                        attributes: {
                            style: "text-align: center;"
                        },
                        template: '#= RemainingField(ITEM_CODE,REQUIRED_QUANTITY,CALC_QTY,REMAINING_QTY,STOCK,PO_PENDING) #',
                        width: "150px",
                    }]
            }).data("kendoGrid");
            //grid.hideColumn("CATEGORY_EDESC");
            //grid.hideColumn("FINISHED_ITEM_EDESC");
        };

    function monthDiff(start, end) {
        var tempDate = new Date(start);
        var monthCount = 0;
        while ((tempDate.getMonth() + '' + tempDate.getFullYear()) != (end.getMonth() + '' + end.getFullYear())) {
            monthCount++;
            tempDate.setMonth(tempDate.getMonth() + 1);
        }
        return monthCount;
    }
    $scope.saveMaterialPlan = function (isValid) {
        if ($("#grid").html() == "") {
            displayPopupNotification("Please generate the plan first", "error");
            return false;
        }
        var valid = false;
        if (isValid) {
            valid = $scope.validation();
        }
        if (valid) {
            showloader();
            var savePlanUrl = window.location.protocol + "//" + window.location.host + "/MaterialPlan/SaveMaterialPlans";
            var params = {
                PLAN_CODE: $scope.PLAN_CODE,
                PLAN_EDESC: $("#PLAN_EDESC").val(),
                PLAN_DATE: moment(BS2AD($("#nepaliDate5").val())).format('MM/DD/YYYY'),
                START_DATE: moment($("#FromDateVoucher").val()).format('MM/DD/YYYY'),
                END_DATE: moment($("#ToDateVoucher").val()).format('MM/DD/YYYY'),
                MONTH_DIFF: monthDiff(new Date($("#FromDateVoucher").val()), new Date($("#ToDateVoucher").val())),
                finishedItemList: $scope.GoodsQty,
                rawItemList: JSON.parse(JSON.stringify($("#grid").data("kendoGrid").dataItems()))
            }
            var data = { model: params }
            setTimeout(function () {
                $.ajax({
                    type: 'POST',
                    url: savePlanUrl,
                    data: data,
                    async: false,
                    dataType: 'json',
                    beforeSend: function () {
                        showloader();
                    },
                    complete: function () {
                        setTimeout(function () {
                            hideloader();
                        }, 100);
                    },
                    success: function (data) {
                        if (data == "error") {
                            displayPopupNotification(data, "error");
                        }
                        else {
                            displayPopupNotification("Succesfully Saved.", "success");
                            window.location = "/Planning/Home/Setup#!Planning/MaterialPlan";
                        }
                    },
                    error: function (error) {
                        if (error.responseText == "constraint") {
                            displayPopupNotification("Plan already created.", "warning");
                        }
                        else {
                            displayPopupNotification(error, "error");
                        }
                    }
                });
            }, 1)
        }
    }

    $scope.productsDataSource = {
        transport: {
            read: {
                dataType: "json",
                url: "/api/MaterialPlanApi/GetAllFGProducts",
            },
            parameterMap: function (data, action) {
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        var newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        var newParams = {
                            filter: $("#customers").val()
                        };
                        return newParams;
                    }
                }
                else {
                    var newParams = {
                        filter: $("#customers").val()
                    };
                    return newParams;
                }
            }
        }
    };

    $scope.productOptions = {
        dataSource: $scope.productsDataSource,
        dataTextField: "ITEM_EDESC",
        dataValueField: "ITEM_CODE",
        placeholder: "Select products...",
        //optionLabel: "Select products...",
        valuePrimitive: true,
        autoBind: false,
        filter: "contains",
        change: function () {
            $scope.qtyChangeEvent();
            $scope.$apply();
        },
        close: function (e) {
            var dataItem = e.sender.dataItem();
            if (dataItem != undefined) {
                var itemCode = dataItem.ITEM_CODE;
                var ls = $.grep($scope.GoodsQty, function (n) { return n.FG_ITEM_CODE == itemCode; });
                if (ls.length > 1) {
                    displayPopupNotification("Same product not allowed.", "warning");
                    var indx = e.sender.element[0].getAttribute("prod-data-index");
                    //$("#fgProducts_" + indx).data('kendoComboBox').value([]);
                    $scope.GoodsQty[indx].FG_ITEM_CODE = "";
                    $scope.$apply();
                }
            }
        }
    };

});

