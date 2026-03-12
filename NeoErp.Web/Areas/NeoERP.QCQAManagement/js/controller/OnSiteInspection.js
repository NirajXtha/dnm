QCQAModule.controller('OnSiteInspectionList', function ($scope, $compile, $rootScope, $http, $filter, $timeout, $routeParams) {
    $scope.voucherno = "";
    $scope.OnSiteInspectionData = [];
    if ($routeParams.voucherno != undefined) {
        $scope.voucherno = $routeParams.voucherno;//.split(new RegExp('_', 'i')).join('/');     
    }
    $("#englishdatedocument").kendoDatePicker({
        value: new Date(),
        format: "dd-MMM-yyyy"
    });
    $scope.QCNO = '@ViewBag.QCNO';
    $scope.FormType = $('.clsFormType').text();
    $("#nepaliDate5").val(AD2BS(kendo.toString(new Date, 'yyyy-MM-dd')));

    $http.get('/api/OnSiteInspectionAPI/GetOnSiteInspectionList')
        .then(function (response) {
            var qcqa = response.data;
            if (qcqa && qcqa.length > 0) {
                $scope.OnSiteInspectionData.push({ element: qcqa });
            }
        })
        .catch(function (error) {
            displayPopupNotification(error, "error");
        })
    var selectedProductType = "";
    $("#ddlProductType").kendoDropDownList({
        optionLabel: "Select Plant...",
        dataTextField: "PRODUCT_TYPE",   // <- replace with actual property
        dataValueField: "PRODUCT_TYPE",    // <- replace with actual property
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/ProductSetupAPI/ProductTypeList",
                    dataType: "json"
                }
            }
        },
        change: function (e) {
            var selectedVal = this.value();//$(this).val(); //$(this).val();//this.value();
            selectedProductType = this.value();
            $scope.childModels = [];
            MultiSelectProduct(selectedVal);

        },
        dataBound: function (e) {
        }
    }).data("kendoDropDownList");

    function MultiSelectProduct(productType, selectedItemCodes = []) {
        if ($('div.dynamicform').find('.k-widget.k-multiselect.k-header').length > 0) {
            var $multiSelects = $('.k-widget.k-multiselect.k-header');
            if ($multiSelects.length > 1) {
                // Remove only the last one
                $multiSelects.last().remove();
            }
        }
        $http.get("/api/ProductSetupAPI/ProductList?ProductType=" + productType + "&Category_Code=" + "")
            .then(function (response) {
                var items = response.data;
                var selectElement = $("#productList");
                var existingMulti = selectElement.data("kendoMultiSelect");
                if (existingMulti) {
                    existingMulti.destroy(); // destroy widget instance
                }
                var innerFloatwrap_first = $('div.dynamicform').find('.k-widget.k-multiselect.k-header .k-multiselect-wrap.k-floatwrap');
                if (innerFloatwrap_first.length != 0) {
                    $('div.dynamicform').find('.k-widget.k-multiselect.k-header').each(function () {
                        var innerFloatwrap = $(this)
                            .children('.k-widget.k-multiselect.k-header .k-multiselect-wrap.k-floatwrap');
                        innerFloatwrap.last().show();
                        if (innerFloatwrap.length > 0) {
                            innerFloatwrap.last().hide();
                        }
                    });
                }
                else {
                    console.log("No element found");
                }
                var multi = selectElement.kendoMultiSelect({
                    dataSource: items,
                    dataTextField: "ITEM_EDESC",
                    dataValueField: "ITEM_CODE",
                    autoClose: false
                }).data("kendoMultiSelect");
                if (multi) {   
                    if (selectedItemCodes) {
                        if (typeof selectedItemCodes === "string") {
                            selectedItemCodes = selectedItemCodes.split(",");
                        }
                        selectedItemCodes = selectedItemCodes.map(function (val) {
                            return val.trim();
                        });
                        multi.value(selectedItemCodes);
                    }
                    var selectedValues1 = multi.value();
                } else {
                    $('.k-multiselect-wrap.k-floatwrap').css("display", "contents");
                    $('#productList_taglist').find('li').remove();
                    $('.k-multiselect-wrap.k-floatwrap').children('.k-input').remove();
                    $("#productList").remove(); // remove old
                    var selectHtml = '<select id="productList" ' +
                        'kendo-multi-select ' +
                        'k-data-text-field="\'ITEM_EDESC\'" ' +
                        'k-data-value-field="\'ITEM_CODE\'" ' +
                        'k-data-source="items" ' +
                        'k-ng-model="productMapping" ' +
                        'k-change="onProductChange1()" ' +
                        'multiple="multiple" style="width:100%"></select>';
                    var element = angular.element(selectHtml);
                    $(".k-multiselect-wrap.k-floatwrap").append(element);
                    $compile(element)($scope);
                    var multi = element.kendoMultiSelect({
                        dataSource: items,
                        dataTextField: "ITEM_EDESC",
                        dataValueField: "ITEM_CODE",
                        autoClose: false
                    }).data("kendoMultiSelect");
                    if (multi) {  
                        if (selectedItemCodes) {
                            if (typeof selectedItemCodes === "string") {
                                selectedItemCodes = selectedItemCodes.split(",");
                            }
                            selectedItemCodes = selectedItemCodes.map(function (val) {
                                return val.trim();
                            });
                            multi.value(selectedItemCodes);
                        }
                        $("#productList").off("change").on("change", function (e) {
                            productDetails(selectedProductType, multi.value());
                        });
                    }
                }
            });
    }
    function productDetails(selectedProductType, selectedItems) {
        if (!selectedItems || selectedItems.length === 0) {
            return;
        }

        $http.get("/api/InternalInspectionAPI/GetParameterDetailsByItemCode", {
            params: {
                productType: selectedProductType,
                itemCode: selectedItems.join(","),
                formType: $scope.FormType
            }
        }).then(function (response) {
            var qcqa = response.data;
            $scope.childModels = [];
            if (qcqa && qcqa.length > 0) {
                $scope.OnSiteInspectionData = qcqa;
                $scope.childModels = qcqa.map(function (item) {
                    return {
                        PARAMETERS: item.PARAMETERS,
                        ITEM_CODE: item.ITEM_CODE,
                        PARAMETER_ID: item.PARAMETER_ID,
                        SERIAL_NO: item.SERIAL_NO,
                        PARAM_CODE: item.PARAM_CODE,
                        SPECIFICATION: item.SPECIFICATION,
                        UNIT: item.UNIT,
                        TARGET: item.TARGET,
                        TOLERENCE: item.TOLERENCE,
                        RESULTS: item.RESULTS,
                        VARIANCE: item.VARIANCE
                    };
                });
            }
        })
    }
    $('#productList').bind("change", function (e) {
        e.preventDefault;
        var selectedItems = $("#productList").data("kendoMultiSelect").value();
        if (selectedProductType == "" || selectedProductType == null)
            selectedProductType = $('#ddlProductType').val();
        if (selectedItems.length === 0) { $('[id="idOnSiteInspection"]').remove(); }
        else { productDetails(selectedProductType, selectedItems); }
    });
    $scope.add_child_element = function (e) {
        $http.get('/api/OnSiteInspectionAPI/GetOnSiteInspectionList')
            .then(function (response) {
                var qcqa = response.data;
                $scope.OnSiteInspectionData.push({ element: qcqa });
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    // remove child row.
    $scope.remove_child_element = function (index) {
        if ($scope.childModels.length > 1) {
            $scope.OnSiteInspectionData.splice(index, 1);
            $scope.childModels.splice(index, 1);
        }
    }
    var rawMateriallList = [];
    $scope.saveOnSiteInspection = function () {
        var rows = document.querySelectorAll("#idOnSiteInspection");
        if ($("#ddlProductType").val() == null || $("#ddlProductType").val() == "" || $("#ddlProductType").val() == undefined) {
            displayPopupNotification("Plant is required", "warning");
            return false;
        }
        if ($("#productList").data("kendoMultiSelect").value() == null || $("#productList").data("kendoMultiSelect").value() == "" || $("#productList").data("kendoMultiSelect").value() == undefined) {
            displayPopupNotification("Product is required", "warning");
            return false;
        }
        $scope.childModels.forEach(function (row, rowIndex) {
            var rawMaterial = {
                ITEM_CODE: row.ITEM_CODE || "",
                PARAMETERS: row.PARAMETERS || "",
                TARGET: row.TARGET || "",
                SERIAL_NO: row.SERIAL_NO || "",
                PARAM_CODE: row.PARAM_CODE || "",
                TOLERENCE: row.TOLERENCE || "",
                RESULTS: $(".RESULTS_" + rowIndex)?.val() || "",
                VARIANCE: $(".Variance_" + rowIndex).text() || ""
            };
            rawMateriallList.push(rawMaterial);
        });
        var wrapper = {
            Inspection_No: $scope.Inspection_No,
            Plant_Id: $("#ddlProductType").val(),//$scope.selectedProductType,
            ITEMSETUPS: $("#productList").data("kendoMultiSelect").value(),
            CREATED_DATE: $('#englishdatedocument').val(),
            Shift: $('#ddlShift').val(),
            BATCH_NO: $('#idBatch_No').val(),
            REFERENCE_NO: $scope.REFERENCE_NO,
            ParameterDetailsList: rawMateriallList  // lowercase 'list'
            /*ParameterDetailsList: $scope.childModels  // lowercase 'list'*/
        };
        $http.post('/api/OnSiteInspectionAPI/saveOnSiteInspection', wrapper)
            .then(function (response) {
                var message = response.data.message; // Extract message from response
                displayPopupNotification(message, "success");
                setTimeout(function () {
                    window.location.reload();
                }, 5000)
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }

    if ($scope.voucherno != "") {
        $http.get('/api/OnSiteInspectionAPI/GetEditOnSiteInspection?transactionno=' + $scope.voucherno)
            .then(function (response) {
                var innerFloatwrap_first = $('div.dynamicform').find('.k-widget.k-multiselect.k-header .k-multiselect-wrap.k-floatwrap');
                if (innerFloatwrap_first.length != 0) {
                    $('div.dynamicform').find('.k-widget.k-multiselect.k-header').each(function () {
                        var innerFloatwrap = $(this)
                            .children('.k-widget.k-multiselect.k-header .k-multiselect-wrap.k-floatwrap');
                        innerFloatwrap.last().show();
                        if (innerFloatwrap.length > 0) {
                            innerFloatwrap.last().hide();
                        }
                    });
                }
                $scope.Inspection_No = response.data.Inspection_No;
                $scope.selectedProductType = response.data.item_code;
                $scope.REFERENCE_NO = response.data.Reference_No;
                $scope.Batch_No = response.data.Batch_No;
                $scope.Shift = response.data.Shift;
                var dropdownShift = $("#ddlShift").data("kendoDropDownList");
                dropdownShift.value(response.data.Shift);
                $scope.selectedProductType = response.data.ITEM_CODE;
                var dropdownProductType = $("#ddlProductType").data("kendoDropDownList");
                dropdownProductType.value(response.data.ITEM_CODE);
                $scope.productMapping = response.data.ITEMSETUPS;
                var dropdownProductList = $("#productList").data("kendoMultiSelect");
                dropdownProductList.value(response.data.ITEMSETUPS);
                $scope.CREATED_DATE = moment(response.data.CREATED_DATE).format('DD-MMM-YYYY');
                $('#englishdatedocument').val($scope.CREATED_DATE);
                $('#nepaliDate5').val(AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD')));
                $scope.childModels = response.data.ParameterDetailsList;
                $scope.products = response.data.PRODUCTS;
                $timeout(function () {
                    var multi = $("#productList").kendoMultiSelect({
                        dataSource: $scope.products,
                        dataTextField: "ITEM_EDESC",
                        dataValueField: "ITEM_CODE",
                        autoClose: false
                    }).data("kendoMultiSelect");
                    multi.dataSource.fetch(function () {
                        multi.dataSource.data().forEach(d => d.ITEM_CODE = d.ITEM_CODE.toString());
                        multi.value(response.data.ITEMSETUPS); // selected value(s)
                        multi.trigger("change");
                    });
                }, 100);
            })
            .catch(function (error) {
                displayPopupNotification("Error fetching ID", "error");
            });
    }

    $scope.CalculateVariance = function (key) {
        let i = 0;
        $(".table_body").find('tr#idOnSiteInspection').each(function () {
            var target = parseFloat($(this).find('.Target_' + i).text()) || 0;
            var results = parseFloat($(this).find('.RESULTS_' + i).val()) || 0;
            var variance = target - results;
            variance = isNaN(variance) ? 0 : variance;
            $(this).find('.Variance_' + i).text(variance);
            i++;
        });
    };
});