QCQAModule.controller('DailyWastageList', function ($scope, $compile, $rootScope, $http, $filter, $timeout, $routeParams) {
    $scope.voucherno = "";
    if ($routeParams.voucherno != undefined) {
        $scope.voucherno = $routeParams.voucherno;//.split(new RegExp('_', 'i')).join('/');
    }
    $("#englishdatedocument").kendoDatePicker({
        value: new Date(),
        format: "dd-MMM-yyyy"
    });
    $("#nepaliDate5").val(AD2BS(kendo.toString(new Date, 'yyyy-MM-dd')));
    
    $scope.DaysInMonth = 0;
    $scope.DailyWastageItems = [];
    $scope.DailyWastageItemLists = [];
    $scope.DailyWastageDetails = [];
    $scope.childModelTemplate = [];
    $scope.childModels = [];
    $scope.rawMaterialRowItem = [];
    $scope.QCNO = '@ViewBag.QCNO';
    $scope.FormType = $('.clsFormType').text();

    var selectedProductType = "";
    $("#ddlProductType").kendoDropDownList({
        optionLabel: "Select Product...",
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
            var selectedVal = this.value();
            selectedProductType = this.value();
            MultiSelectProduct(selectedVal);

        },
        dataBound: function (e) {
        }
    }).data("kendoDropDownList");
    function MultiSelectProductForEdit(productType, selectedItemCodes = []) {
        if (typeof selectedItemCodes === "string" && selectedItemCodes !== "") {
            selectedItemCodes = selectedItemCodes.split(",").map(x => x.trim());
        }

        if (!Array.isArray(selectedItemCodes)) {
            selectedItemCodes = [];
        }
        $http.get("/api/ProductSetupAPI/ProductList?ProductType=" + productType)
            .then(function (response) {

                var items = response.data;  // <-- MultiSelect datasource
                var selectElement = $("#productList");
                var existing = selectElement.data("kendoMultiSelect");
                if (existing) {
                    existing.destroy();
                    selectElement.data("kendoMultiSelect", null);
                }
                var multi = selectElement.kendoMultiSelect({
                    dataSource: items,
                    dataTextField: "ITEM_EDESC",
                    dataValueField: "ITEM_CODE",
                    autoClose: false
                }).data("kendoMultiSelect");

                if (selectedItemCodes.length > 0) {
                    multi.value(selectedItemCodes);
                }
            });
    }


    function MultiSelectProduct(productType, selectedItemCodes = []) {
        if ($('div.dynamicform').find('.k-widget.k-multiselect.k-header').length > 0) {
            var $multiSelects = $('.k-widget.k-multiselect.k-header');
            if ($multiSelects.length > 1) {
                // Remove only the last one
                $multiSelects.last().remove();
            }
        }
        $http.get("/api/ProductSetupAPI/ProductList?ProductType=" + productType)
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
                if (multi) {   // <-- ✅ only run if initialized successfully
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
                $scope.DailyWastageItems = qcqa;
                $scope.childModels = qcqa.map(function (item) {
                    return {
                        PARAMETERS: item.PARAMETERS,
                        ITEM_CODE: item.ITEM_CODE,
                        SERIAL_NO: item.SERIAL_NO,
                        PARAM_CODE: item.PARAM_CODE,
                        PARAMETER_ID: item.PARAMETER_ID,
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
        console.log("selectedProductType", selectedProductType);
        console.log("selectedItems", selectedItems);
        productDetails(selectedProductType, selectedItems);
    });
   
    if ($scope.voucherno != "") {
        $scope.DailyWastageItems = [];       
        $http.get('/api/DailyWastageAPI/GetEditDailyWastage?transactionno=' + $scope.voucherno)
            .then(function (response) {
                var qcqa = response.data;
                $scope.DAILYWASTAGE_NO = response.data.DAILYWASTAGE_NO;
                $scope.SERIAL_NO = response.data.SERIAL_NO;
                $scope.BATCH_NO = response.data.BATCH_NO;
                $scope.CREATED_DATE = moment(response.data.CREATED_DATE).format('DD-MMM-YYYY');
                $('#englishdatedocument').val($scope.CREATED_DATE);
                $('#nepaliDate5').val(AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD')));
                $scope.selectedProductType = response.data.ITEM_CODE;
                var dropdownProductType = $("#ddlProductType").data("kendoDropDownList");
                dropdownProductType.value(response.data.ITEM_CODE);
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
                $scope.childModels = response.data.DailyWastageList;             
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    else {
        $http.get('/api/DailyWastageAPI/GetDailyWastageList')
            .then(function (response) {
                var qcqa = response.data;
                if (qcqa && qcqa.length > 0) {
                    $scope.DailyWastageItems.push({ element: qcqa });
                }
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
	}
    $scope.add_child_element = function (e) {
        $http.get('/api/DailyWastageAPI/GetDailyWastageList')
            .then(function (response) {
                var qcqa = response.data;
                if (qcqa && qcqa.length > 0) {
                    $scope.DailyWastageItems.push({ element: qcqa });
                }
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    $scope.remove_child_element = function (index) {
        if ($scope.DailyWastageItems.length > 1 || $scope.childModels.length > 1) {
            $scope.DailyWastageItems.splice(index, 1);
            $scope.childModels.splice(index, 1);
        }
    }
    var rawMateriallList = [];
    
    $scope.saveDailyWastage = function () {
        var rows = document.querySelectorAll("#idDailyWastage");
        if ($('#ddlProductType').val() == null || $("#ddlProductType").val() == "" || $("#ddlProductType").val() == undefined) {
            displayPopupNotification("Product Type is required", "warning");
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
                UNIT: row.UNIT || "",
                SERIAL_NO: row.SERIAL_NO || "",
                PARAM_CODE: row.PARAM_CODE || "",
                QTY: $(".Qty_" + rowIndex).val() || ""
            };
            rawMateriallList.push(rawMaterial);
        });
        var wrapper = {

            DAILYWASTAGE_NO: $scope.DAILYWASTAGE_NO,
            ITEM_CODE: $("#ddlProductType").val(),
            CREATED_DATE: $('#englishdatedocument').val(),
            ITEMSETUPS: $("#productList").data("kendoMultiSelect").value(),
            BATCH_NO: $('#txtBatch_No').val(),
            DailyWastageList: rawMateriallList  // lowercase 'list
        };
        $http.post('/api/DailyWastageAPI/saveDailyRawMaterial', wrapper)
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


    $scope.getmucode = function (index, productId) {

        try {
            var pId = $.isNumeric(parseInt(productId));
            if (pId === false) {
                throw "";
            }
            var staturl = window.location.protocol + "//" + window.location.host + "/api/RawMaterialAPI/GetMUCodeByRawMaterialId";
            var response = $http({
                method: "GET",
                url: "/api/RawMaterialAPI/GetMUCodeByRawMaterialId",
                params: { productId: productId },
                contentType: "application/json",
                dataType: "json"
            });
            return response.then(function (data) {
                $('.cproduct_' + index).val(data.data[0].ItemDescription);
                $('.ItemCode_' + index).val(data.data[0].ItemCode);
                $('#vendor_' + index).val('');
                $('.GSM_' + index).val(data.data[0].GSM);
                $('.SIZE_WIDTH_' + index).val(data.data[0].SIZE_WIDTH);
                $('.STRENGTH_' + index).val(data.data[0].STRENGTH);
                $('.THICKNESS_' + index).val(data.data[0].THICKNESS);
                $scope.ParticularItemCode = data.data[0].ItemCode;
            });
        } catch (e) {
            return;
        }
    };

});