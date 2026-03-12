planningModule.controller('CollectionFromMaterialCtrl', function ($scope, planservice, $routeParams, $rootScope, $http) {
    $scope.pageName = "Collection from material";
    $scope.visibleSavebtn = false;
    $scope.visibleGenerateBtn = true;
    $scope.saveUpdateBtn = "Save";
    $scope.planName = $routeParams.planName;
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
        //var valid = false;
        //if (isValid) {
        //    valid = $scope.validation();
        //}
        //else if (!isValid) {
        //    displayPopupNotification("There is an invalid field.", "error");
        //}
        //else {
        //    return displayPopupNotification("There is an invalid data", "error");
        //}
        //if (valid) {
        $scope.bindInInputField();
        showloader();
        var param = {
            mpList: $scope.GoodsQty
        }
        createGrid(param);
        $scope.visibleSavebtn = true;
        $scope.visibleGenerateBtn = false;
        $scope.gbClickInfo();
        //}
    }
    $scope.bindInInputField = function () {
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

    var frequencyTitleObject;

    function BindFrequencyTitle() {
        var startDate = $("#FromDateVoucher").val();
        var endDate = $("#ToDateVoucher").val();
        startDate = moment(startDate).format('MM-DD-YYYY');
        endDate = moment(endDate).format('MM-DD-YYYY')
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

    function BindMonthWiseData() {
        var param = {
            PLAN_CODE: $("#MaterialPlanList").val(),
        }
        var data = { model: param }
        var pCode = $("#MaterialPlanList").val();
        $.ajax({
            type: 'POST',
            url: window.location.protocol + "//" + window.location.host + "/api/ProcurementPlanApi/GetAllRawMaterialByMaterialPlanCode?pCode=" + pCode,
            //data: data,
            dataType: 'html',
            async: false,
            success: function (result) {
                result = JSON.parse(result);
                var itemList = _.uniq(result, "ITEM_CODE");
                var changeArr = [];
                var data = result;
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
                        data[i][data[i]['MONTH'].toUpperCase()] = parseFloat(data[i]['REQUIRED_QUANTITY']).toFixed(2);
                        finalData.push(data[i]);
                    } else {
                        finalData[found][data[i]['MONTH'].toUpperCase()] = parseFloat(data[i]['REQUIRED_QUANTITY']).toFixed(2);
                    }
                }

                //$.each(result, function (e, v) {
                //    //v.MAX_LEVEL = 2;
                //    var obj = {
                //        ITEM_CODE: v.ITEM_CODE,
                //        ITEM_EDESC: v.ITEM_EDESC,
                //        FINISHED_ITEM_EDESC: v.FINISHED_ITEM_EDESC,
                //        FINISHED_ITEM_CODE: v.FINISHED_ITEM_CODE,
                //        MIN_LEVEL: v.MIN_LEVEL,
                //        MAX_LEVEL: v.MAX_LEVEL,
                //    };
                //    var remainingAmt = 0;
                //    var monthInt = "";
                //    //if (v.MAX_LEVEl > 0) {
                //    //    $.each(frequencyTitleObject, function (fi, fv) {
                //    //        var monthAmt = 0.0;
                //    //        if (v.MONTH.toUpperCase() == fv.PropertyName) {
                //    //            monthAmt = monthAmt + v.REQUIRED_QUANTITY;
                //    //            monthInt = parseInt(fv.MONTHINT) + 1;
                //    //            if (parseInt(monthInt) < 9)
                //    //                monthInt = "0" + monthInt;
                //    //            if (parseInt(monthInt) > 12)
                //    //                monthInt = "01";

                //    //            if (remainingAmt > 0) {
                //    //                monthAmt = monthAmt + remainingAmt;
                //    //            }
                //    //            if (monthAmt > v.MAX_LEVEL) {
                //    //                remainingAmt = monthAmt - v.MAX_LEVEL;
                //    //                monthAmt = v.MAX_LEVEL;
                //    //            }
                //    //        }
                //    //        if (remainingAmt > 0 && monthInt == fv.MONTHINT && parseInt(monthInt) < 13) {
                //    //            if (remainingAmt > v.MAX_LEVEL) {
                //    //                remainingAmt = remainingAmt - v.MAX_LEVEL;
                //    //                if (remainingAmt > v.MAX_LEVEL) {
                //    //                    monthAmt = v.MAX_LEVEL;
                //    //                }
                //    //                else {
                //    //                    monthAmt = remainingAmt;
                //    //                }
                //    //                monthInt = parseInt(fv.MONTHINT) + 1;
                //    //                if (parseInt(monthInt) < 9)
                //    //                    monthInt = "0" + monthInt
                //    //            }
                //    //        }
                //    //        obj[fv.PropertyName] = monthAmt;
                //    //    });
                //    //} else {
                //        //$.each(frequencyTitleObject, function (fi, fv) {
                //        //    var monthAmt = 0.0;
                //        //    if (v.MONTH.toUpperCase() == fv.PropertyName) {
                //        //        monthAmt = monthAmt + v.REQUIRED_QUANTITY;
                //        //    }
                //        //    obj[fv.PropertyName] = monthAmt;
                //        //});
                //    //}

                //    changeArr.push(obj);
                //});
                monthWiseResponseData = finalData;
            }
        });
    }
    var grid,
        createGrid = function (param) {
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
                field: "MAX_LEVEL",
                title: "Max",
                locked: true,
                width: headerWidth_ScreenWise(7),
                attributes: {
                    style: "text-align: right;"
                },
            });
            dynamicHeader.push({
                field: "MIN_LEVEL",
                title: "Min",
                locked: true,
                filterable: true,
                width: headerWidth_ScreenWise(7),
                attributes: {
                    style: "text-align: right;"
                },
            });
            dynamicHeader.push({
                field: "FINISHED_ITEM_EDESC",
                title: "Finished Item",
                groupHeaderTemplate: "#= data.value #",
                hidden: true,
                width: 50,
            });
            dynamicHeader.push({
                field: "CATEGORY_EDESC",
                title: "Category",
                groupHeaderTemplate: "#= data.value #",
                hidden: true,
                width: 50,
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
                    pageSize: 20,
                    group: [{
                        field: "FINISHED_ITEM_EDESC", groupHeaderTemplate: "#= data.value #",
                    }, {
                        field: "CATEGORY_EDESC", groupHeaderTemplate: "#= data.value #",
                    }],
                    requestEnd: function () {
                        hideloader();
                    },
                },
                columnMenu: true,
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
                        //$scope.$apply();
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
                columns: dynamicHeader
            }).data("kendoGrid");
        };

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
            var savePlanUrl = window.location.protocol + "//" + window.location.host + "/CollectionPlan/SaveCollectionPlan";//window.location.protocol + "//" + window.location.host + "/MaterialPlan/SaveMaterialPlans";
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

