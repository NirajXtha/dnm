/*eslint-disable*/
planningModule.controller('MaterialFromProductionCtrl', function ($scope, planservice, $routeParams, $rootScope, $http) {
    $scope.pageName = "Refrential Material Plan";
    $scope.visibleSavebtn = false;
    $scope.visibleGenerateBtn = true;
    $scope.saveUpdateBtn = "Save";
    $scope.PLAN_CODE = $routeParams.plancode;
    var planName = $routeParams.planName;
    $scope.manualOrReferential = "";

    var ref = $routeParams.ref;
    if (ref == "Refrential") {
        $('#fromMRPRadio').attr('checked', true);
        $('input[name=SALES_PRODUCTION]').trigger("click");
        $(".ManualDiv").hide();
        $scope.manualOrReferential = "Refrential";
        $(".custom_col").show();
    } else if (ref == "Manual") {
        $(".ReferenceDiv").hide();
        $(".ManualDiv").hide();
        $(".custom_col").hide();
        $scope.pageName = "Material Plan";
        $scope.manualOrReferential = "Manual";
    }
    else {
        $(".ReferenceDiv").show();
        $(".ManualDiv").show();
        $(".custom_col").show();
        $scope.manualOrReferential = "Refrential";
    }

    $scope.fgproductvalue = [];
    $scope.materialPlan = {
        PLAN_EDESC:planName,
        PLAN_DATE: "",
        PLAN_NDATE: AD2BS(moment().format('YYYY-MM-DD'))
    }
    $scope.GoodsQty = [{
        FG_ITEM_CODE: "",
        QTY: 1
    }];

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
    $scope.commonFn = function () {
        $scope.visibleSavebtn = false;
        $scope.visibleGenerateBtn = true;
        $("#grid").empty();
        $scope.gbClickInfo();
        $scope.$apply();
    }

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

    $scope.gbClickInfo = function () {
        if ($("#grid").html() == "") {
            $(".helptext").html("");
            $("#grid").removeClass('k-grid k-widget k-display-block k-reorderable');
            if ($("#grid").data("kendoGrid") != undefined)
                $(".helptext").html('<span class="grid-info" style="color:#327ad5;">* Please click the "generate" button to generate the material plan according to selected product and quantity.</span>');
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
        var salesOrProd = $('input[name=SALES_PRODUCTION]:checked').val();
        if (salesOrProd == "Manual") {
            var productValidation = $.grep($scope.GoodsQty, function (b) {
                return b.FG_ITEM_CODE === "" || b.FG_ITEM_CODE == null;
            });
            if (productValidation.length > 0) {
                displayPopupNotification("Please choose the product", "error");
                valid = false;
                hideloader();
            }
            if ($("#nepaliDate5").val() == "") {
                displayPopupNotification("Please Choose the date", "error");
                valid = false;
                hideloader();
            }
        }
        else if (salesOrProd == "Sales") {
            var multiSelectVal = $("#SalesPlanList").data("kendoMultiSelect").value();
            if (multiSelectVal.length <= 0) {
                displayPopupNotification("Please choose the sales plan", "error");
                valid = false;
                hideloader();
            }

        }
        else if (salesOrProd == "Production") {
            var multiSelectVal = $("#ProductionPlanList").data("kendoMultiSelect").value();
            if (multiSelectVal.length <= 0) {
                displayPopupNotification("Please choose the production plan", "error");
                valid = false;
                hideloader();
            }
        }
        //else {
        //    var multiSelectVal = $("#customerList").data("kendoMultiSelect").value();
        //    if (multiSelectVal.length <= 0) {
        //        displayPopupNotification("Please choose the customer plan", "error");
        //        valid = false;
        //        hideloader();
        //    }
        //}
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
        var salesOrProd = $('input[name=SALES_PRODUCTION]:checked').val();
        //if (salesOrProd == "Sales") {
        //    $("#SalesPlanList").data("kendoMultiSelect").value([]);
        //} else {
        //    $("#ProductionPlanList").data("kendoMultiSelect").value([]);
        //}
        $("#SalesPlanList").data("kendoMultiSelect").value([]);
        $("#ProductionPlanList").data("kendoMultiSelect").value([]);
        $("#customerList").data("kendoMultiSelect").value([]);
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
        debugger;
        showloader();
        var valid = false;
        if (isValid) {
            valid = $scope.validation();
        }
        else if (!isValid) {
            hideloader();
            displayPopupNotification("There is an invalid field.", "error");
        }
        else {
            hideloader();
            return displayPopupNotification("There is an invalid data", "error");
        }
        if (valid) {
            $scope.bindInInputField();
            showloader();
            var param = {
                mpList: $scope.GoodsQty
            }
            var salesOrProd = $('input[name=SALES_PRODUCTION]:checked').val();
            if (salesOrProd == "Manual")
                createGrid1(param);
            else
                createGrid(param);
            $scope.visibleSavebtn = true;
            $scope.visibleGenerateBtn = false;
            $scope.gbClickInfo();
        }
    }
    $scope.bindInInputField = function () {
        debugger;
        var filterdata = ReportFilter.filterAdditionalData();
        $("#END_DATE").val(filterdata.ReportFilters.ToDate);
        $("#PLAN_CODE").val();
        $("#PLAN_EDESC").val($("#planName").val());
        $("#REMARKS").val($("#remarks").val());
        $("#START_DATE").val(filterdata.ReportFilters.FromDate);
        $("#TIME_FRAME_CODE").val("203");
        $("#TIME_FRAME_EDESC").val("LOC");
        $("#customerCode").val("");
        $("#branchCode").val("");
        $("#divisionCode").val("");
        $("#employeeCode").val("");
        $("#dateFormat").val("BS");
    }

    $scope.showHide = function ($this) {
        if ($this == "PROD") {
            $("#isFromMRP").show();
            $("#isManualProcmt").hide();
            $("#manualDiv").hide();
            $("#isFromSalesOrder").hide();
        }
        else if ($this == "SALES") {
            $("#isFromMRP").hide();
            $("#isManualProcmt").show();
            $("#manualDiv").hide();
            $("#isFromSalesOrder").hide();
        }
        else if ($this == "ORDER") {
            $("#isFromMRP").hide();
            $("#isManualProcmt").hide();
            $("#manualDiv").hide();
            $("#isFromSalesOrder").show();
        }
        else {
            $("#isFromMRP").hide();
            $("#isManualProcmt").hide();
            $("#manualDiv").show();
            $("#isFromSalesOrder").hide();
        }
    };
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
                }, 10);
            },
            success: function (data) {
                $scope.GoodsQty = [];
                $scope.materialPlan.PLAN_EDESC = data[0].PLAN_EDESC;
                $scope.materialPlan.PLAN_NDATE = AD2BS(moment(data[0].PLAN_DATE).format('YYYY-MM-DD'));
                var refCodeArr = [];
                var refCode = data[0].REFERENCE_CODE;
                if (refCode != undefined) {
                    if (refCode.indexOf(",") > -1) {
                        refCode = refCode.split(",");
                        refCodeArr = refCode;
                    }
                    else {
                        refCodeArr.push(refCode);
                    }
                }
                $('#ManualRadio').prop("checked", false);
                $('#fromMRPRadio').prop("checked", false);
                $('#ManualOnlyDiv').prop("checked", false);
                $('#ProductionRadio').prop("checked", false);
                $('#OrderRadio').prop("checked", false);
                $scope.showHide(data[0].REFERENCE_FLAG);
                if (data[0].REFERENCE_FLAG == "SALES") {
                    $('#fromMRPRadio').prop("checked", true);
                    $("#SalesPlanList").data("kendoMultiSelect").value(refCodeArr);
                }
                else if (data[0].REFERENCE_FLAG == "PROD") {
                    $('#ProductionRadio').prop("checked", true);
                    $("#ProductionPlanList").data("kendoMultiSelect").value(refCodeArr);
                }
                else if (data[0].REFERENCE_FLAG == "ORDER") {
                    $('#OrderRadio').prop("checked", true);
                    $("#customerList").data("kendoMultiSelect").value(refCodeArr);
                }
                else {
                    $('#ManualOnlyDiv').prop("checked", true);
                    angular.forEach(data, function (i, v) {
                        $scope.GoodsWithQty = {
                            FG_ITEM_CODE: i.FINISHED_ITEM_CODE,
                            QTY: i.MATERIAL_QUANTITY
                        }
                        $scope.GoodsQty.push($scope.GoodsWithQty);
                    });
                }
            },
            error: function (error) {
                hideloader();
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

    var frequencyTitleObject;

    function BindFrequencyTitle() {
        debugger;
        var startDate = $("#FromDateVoucher").val();
        var endDate = $("#ToDateVoucher").val();
        startDate = moment($("#ddlDateFilterVoucher option[value='This Year']").attr('data-start-date')).format('MM-DD-YYYY');
        endDate = moment($("#ddlDateFilterVoucher option[value='This Year']").attr('data-end-date')).format('MM-DD-YYYY')
        var param = {
            startDate: startDate,
            endDate: endDate,
            timeFrameCode: "",
            timeFrameName: "month",
            datetype: "BS"
        };

        $.ajax({
            type: 'GET',
            url: window.location.protocol + "//" + window.location.host + "/Planning/Plan/getFrequencyTitle",
            data: param,
            dataType: 'html',
            async: false,
            success: function (result) {
                frequencyTitleObject = JSON.parse(result);
            }
        });
    }
    var monthWiseResponseData = [];
    var totalSum = 0;

    $('input[name=SALES_PRODUCTION]:radio').on('change', function () {
        $scope.clearAllField();
    });

    function BindMonthWiseData() {
        //debugger;
        var pCode = "";
        var pUrl = "";
        var startDate = $("#FromDateVoucher").val();
        var endDate = $("#ToDateVoucher").val();
        startDate = moment(startDate).format('MM-DD-YYYY');
        endDate = moment(endDate).format('MM-DD-YYYY')
        var salesOrProd = $('input[name=SALES_PRODUCTION]:checked').val();
        if (salesOrProd == "Sales") {
            //pCode = $("#SalesPlanList").val();
            pCode = $.grep($("#SalesPlanList").data("kendoMultiSelect").value(), function (v) { return (v) }).toString();
            pUrl = window.location.protocol + "//" + window.location.host + "/api/MaterialPlanApi/GetAllRawMaterialBySalesPlanCode?pCode=" + pCode;
        }
        else if (salesOrProd == "Production") {
            //pCode = $("#ProductionPlanList").val();
            pCode = $.grep($("#ProductionPlanList").data("kendoMultiSelect").value(), function (v) { return (v) }).toString();
            pUrl = window.location.protocol + "//" + window.location.host + "/api/MaterialPlanApi/GetAllRawMaterialByProductionPlanCode?pCode=" + pCode;
        }
        else {
            pCode = $.grep($("#customerList").data("kendoMultiSelect").value(), function (v) { return (v) }).toString();
            pUrl = window.location.protocol + "//" + window.location.host + "/api/MaterialPlanApi/GetAllRawMaterialBySalesOrderCustomerCode?pCode=" + pCode + "&startDate=" + startDate + "&endDate=" + endDate;
        }
        $.ajax({
            type: 'POST',
            url: pUrl,
            //data: data,
            dataType: 'html',
            async: false,
            success: function (result) {
                result = JSON.parse(result);
                var data = result;
                var yearVal = 0.0;
                var finalData = [];
                var findFinalData = function (item) {
                    for (var i in finalData) {
                        if (finalData[i]['ITEM_CODE'] == item['ITEM_CODE'] && finalData[i]['FINISHED_ITEM_CODE'] == item['FINISHED_ITEM_CODE']) {
                            return i;
                        }
                    }
                    return null;
                }

                for (var i in data) {
                    var found = findFinalData(data[i]);
                    if (found == null) {
                        //debugger;
                        if (data[i]['MONTH']!==null)
                        data[i][data[i]['MONTH'].toUpperCase()] = parseFloat(data[i]['REQUIRED_QUANTITY']).toFixed(2);
                        finalData.push(data[i]);
                    } else {
                        finalData[found][data[i]['MONTH'].toUpperCase()] = parseFloat(data[i]['REQUIRED_QUANTITY']).toFixed(2);
                    }
                }
                $.each(finalData, function (i, v) {
                    var yearVal = 0.0;
                    $.each(frequencyTitleObject, function (fi, fv) {
                        //debugger;
                        if (parseFloat(finalData[i][fv.PropertyName]) != null)
                            //debugger;
                        if (!isNaN(parseFloat(finalData[i][fv.PropertyName])))
                            //debugger;
                                yearVal += parseFloat(finalData[i][fv.PropertyName]);
                    });
                    finalData[i]["REQ_QTY"] = yearVal;
                    finalData[i]["PO"] = (yearVal - (v.STOCK + v.PO_PENDING)) > 0 ? (yearVal - (v.STOCK + v.PO_PENDING)):"-";
                });
                monthWiseResponseData = finalData;
            }
        });
    }
    var grid,
        createGrid = function (param) {
            debugger;
            BindFrequencyTitle();
            BindMonthWiseData();
            var dynamicHeader = [];
            var yearWiseArr = [];
            dynamicHeader.push({
                field: "ITEM_EDESC",
                title: "Item",
                groupHeaderTemplate: "#= value #",
                locked: true,
                width: headerWidth_ScreenWise(15.4),
            });
            dynamicHeader.push({
                field: "INDEX_MU_CODE",
                title: "Unit",
                width: 80,
                locked: true,
            });
            //dynamicHeader.push({
            //    field: "MAX_LEVEL",
            //    title: "Max",
            //    locked: true,
            //    width: headerWidth_ScreenWise(7),
            //    attributes: {
            //        style: "text-align: right;"
            //    },
            //});
            //dynamicHeader.push({
            //    field: "MIN_LEVEL",
            //    title: "Min",
            //    locked: true,
            //    filterable: true,
            //    width: headerWidth_ScreenWise(7),
            //    attributes: {
            //        style: "text-align: right;"
            //    },
            //});
            dynamicHeader.push({
                field: "FINISHED_ITEM_EDESC",
                title: "Finished Item",
                groupHeaderTemplate: "#= data.value #",
                hidden: true,
                //width: 50,
            });
            dynamicHeader.push({
                field: "CATEGORY_EDESC",
                title: "Category",
                groupHeaderTemplate: "#= data.value #",
                hidden: true,
                //width: 50,
            });
            dynamicHeader.push({
                field: "REQ_QTY",
                title: "Required Qty",
                locked: true,
                width: 100,
                format: "{0:n2}",
                attributes: {
                    style: "text-align: right;"
                },
            });
            dynamicHeader.push({
                field: "STOCK",
                title: "Available Stock",
                locked: true,
                format: "{0:n2}",
                width: 100,
                attributes: {
                    style: "text-align: right;"
                },
            });

            dynamicHeader.push({
                field: "PO_PENDING",
                title: "Po In Transit",
                locked: true,
                format: "{0:n2}",
                width: 100,
                attributes: {
                    style: "text-align: right;"
                },
            });
            dynamicHeader.push({
                field: "PO",
                title: "New PO",
                locked: true,
                format: "{0:n2}",
                width: 100,
                attributes: {
                    style: "text-align: right;"
                },
            });
            $.each(frequencyTitleObject, function (id, it) {
                yearWiseArr.push(it.getPeriod[0].YEAR);
            });
            yearWiseArr = _.uniq(yearWiseArr);
            var aggregate = [];
            $.each(yearWiseArr, function (e, i) {
                var monthArr = {};
                var test = $.grep(frequencyTitleObject, function (e) { return e.getPeriod[0].YEAR == i; });
                var column = [];
                $.each(test, function (idx, item) {
                    if (item.PropertyName != null || item.Title != null) {
                        column.push({
                            field: item.PropertyName,
                            title: item.Title,
                            //editable: true,
                            attributes: {
                                style: "text-align: right;"
                            },
                            format: "{0:n2}",
                            //footerTemplate: "<span style='display: block; text-align: right;'> #=kendo.toString(sum, '\\#\\#,\\#.00')# </span>",
                            aggregates: ["sum"],
                            //width: "80px",
                            template: function (dataItem) {
                                var value = dataItem[item.PropertyName];
                                return "<input readonly class='freqItemNum_" + dataItem.ITEM_CODE + "_" + item.PropertyName + "_" + item.getPeriod[0].YEAR + "' name='freqItemNum_" + dataItem.ITEM_CODE + "_" + item.PropertyName + "_" + item.getPeriod[0].YEAR + "' ng-model='freqItemNum_" + dataItem.ITEM_CODE + "' type='number' min='0' value='" + value + "'/>"
                            },
                            width: headerWidth_ScreenWise(6.87),
                        });
                        aggregate.push({
                            field: item.PropertyName,
                            aggregate: "sum"
                        });
                    }
                })
                monthArr['title'] = i;
                monthArr['headerAttributes'] = { style: "text-align: center" };
                monthArr['columns'] = column;
                dynamicHeader.push(monthArr);
            });
            grid = $("#grid").kendoGrid({
                dataSource: {
                    type: "json",
                    data: monthWiseResponseData,
                    aggregate: aggregate,
                    //pageSize: 20,
                    group: [{
                        field: "FINISHED_ITEM_EDESC", groupHeaderTemplate: "#= data.value #",
                    }, {
                        field: "CATEGORY_EDESC", groupHeaderTemplate: "#= data.value #",
                    }],
                    requestEnd: function () {
                        hideloader();
                    },
                },
                toolbar: ["excel"],
                excel: {
                    fileName: "Referential.xlsx"
                },
                columnMenu: true,
                height: window.innerHeight - 200,
                reorderable: true,
                groupable: false,
                resizable: true,
                columnMenu: false,
                dataBinding: function () {
                    record = (this.dataSource.page() - 1) * this.dataSource.pageSize();
                },
                dataBound: function (o) {
                    debugger;
                    var grid = o.sender;
                    if (grid.dataSource.total() == 0) {
                        var colCount = grid.columns.length;
                        $(o.sender.wrapper)
                            .find('tbody')
                            .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                        displayPopupNotification("No Data Found.", "info");
                        $scope.visibleSavebtn = false;
                        $scope.visibleGenerateBtn = true;
                        //$scope.$apply();
                    }
                },
                columns: dynamicHeader
            }).data("kendoGrid");
        };
    var createGrid1 = function (param) {
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
                group: [
                //    {
                //    field: "FINISHED_ITEM_EDESC",
                //},
                    {
                    field: "CATEGORY_EDESC"
                    }
                ],
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
    $scope.saveMaterialPlanAll = function (isValid) {
        var salesOrProd = $('input[name=SALES_PRODUCTION]:checked').val();
        if (salesOrProd == "Manual")
            $scope.saveMaterialPlan(isValid);
        else
            $scope.saveMaterialFromProduction(isValid);
    };

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
                PLAN_EDESC: $("#planName").val(),
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

    $scope.saveMaterialFromProduction = function (isValid) {
        if ($("#grid").html() == "") {
            displayPopupNotification("Please generate the plan first", "error");
            return false;
        }
        var valid = true;
        //if (isValid) {
        //    valid = $scope.validation();
        //}
        if (valid) {
            showloader();
            var savePlanUrl = window.location.protocol + "//" + window.location.host + "/MaterialPlan/SaveMaterialPlanReference";//window.location.protocol + "//" + window.location.host + "/MaterialPlan/SaveMaterialPlans";
            var data = $("#grid").data("kendoGrid").dataItems();
            var finalData = [];
            var findFinalData = function (item) {
                for (var i in finalData) {
                    if (finalData[i]['ITEM_CODE'] == item['ITEM_CODE'] && finalData[i]['FINISHED_ITEM_CODE'] == item['FINISHED_ITEM_CODE']) {
                        return i;
                    }
                }
                return null;
            }
            for (var i in data) {
                var found = findFinalData(data[i]);
                if (found == null) {
                    finalData.push(data[i]);
                }
            }
            var prodList = "";
            var refFlag = "";
            var salesOrProd = $('input[name=SALES_PRODUCTION]:checked').val();
            if (salesOrProd == "Sales") {
                prodList = $.grep($("#SalesPlanList").data("kendoMultiSelect").value(), function (v) { return (v) }).toString();
                refFlag = "SALES";
            }
            else if (salesOrProd == "Production") {
                prodList = $.grep($("#ProductionPlanList").data("kendoMultiSelect").value(), function (v) { return (v) }).toString();
                refFlag = "PROD";
            }
            else if (salesOrProd == "Order") {
                prodList = $.grep($("#customerList").data("kendoMultiSelect").value(), function (v) { return (v) }).toString();
                refFlag = "ORDER";
            }
            var startDate = $("#FromDateVoucher").val();
            var endDate = $("#ToDateVoucher").val();
            startDate = moment(startDate).format('MM-DD-YYYY');
            endDate = moment(endDate).format('MM-DD-YYYY')
            var params = {
                START_DATE: startDate,
                END_DATE: endDate,
                PLAN_CODE: $scope.PLAN_CODE,
                PLAN_EDESC: $("#planName").val(),
                PLAN_DATE: moment(BS2AD($("#nepaliDate5").val())).format('MM/DD/YYYY'),
                REFERENCE_CODE: prodList,
                REFERENCE_FLAG: refFlag,
                rawItemList: JSON.parse(JSON.stringify(finalData))
            }
            var dataList = { model: params }
            setTimeout(function () {
                $.ajax({
                    type: 'POST',
                    url: savePlanUrl,
                    data: dataList,
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

    $scope.saveProcureFromMaterial = function (isValid) {
        if ($("#grid").html() == "") {
            displayPopupNotification("Please generate the plan first", "error");
            return false;
        }
        var valid = true;
        //if (isValid) {
        //    valid = $scope.validation();
        //}
        if (valid) {
            showloader();
            var savePlanUrl = window.location.protocol + "//" + window.location.host + "/MaterialPlan/SaveMaterialPlanReference";//window.location.protocol + "//" + window.location.host + "/MaterialPlan/SaveMaterialPlans";
            //var params = {
            //    PLAN_CODE: $scope.PLAN_CODE,
            //    PLAN_EDESC: $("#PLAN_EDESC").val(),
            //    PLAN_DATE: moment(BS2AD($("#nepaliDate5").val())).format('MM/DD/YYYY'),
            //    finishedItemList: $scope.GoodsQty,
            //    rawItemList: JSON.parse(JSON.stringify($("#grid").data("kendoGrid").dataItems()))
            //}
            var dataList = $('#pmGridForm').serialize();
            var data = JSON.stringify(dataList)
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

