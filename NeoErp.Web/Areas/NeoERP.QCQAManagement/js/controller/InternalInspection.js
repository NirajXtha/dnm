QCQAModule.controller('InternalInspectionList', function ($scope, $compile, $rootScope, $http, $filter, $timeout, $routeParams) {
    $scope.voucherno = "";
    $scope.InternalInspectionData = [];
    $scope.childModels = [];
    $scope.vendorchildModels = [];

    $scope.products = [];         
    $scope.productMapping = [];  

    $timeout(function () {
        var totalWidth = 0;
        var headersWidth = 0;
        var classes = [
            '.SNo',
            '.Parameters',
            '.Specification',
            '.Unit',
            '.Target',
            '.Tolerence',
            '.Results',
            '.Variance',
            '.btn-action'
        ];

        var headers = [
            '.SNo',
            '.Parameters'
        ];

        headers.forEach(function (cls) {
            var el = document.querySelector(cls);
            if (el) {
                headersWidth += el.getBoundingClientRect().width;
            }
        });

        classes.forEach(function (cls) {
            var el = document.querySelector(cls);
            if (el) {
                totalWidth += el.getBoundingClientRect().width;
            }
        });
        $('.th-headers').css('width', headersWidth + 'px');
        $('.remarks-row').css('width', totalWidth + 'px');
    }, 0); // runs after digest/render
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

    $http.get('/api/InternalInspectionAPI/GetInternalInspectionList')
        .then(function (response) {
            var qcqa = response.data;
            if (qcqa && qcqa.length > 0) {
                $scope.InternalInspectionData.push({ element: qcqa });
            }
        })
        .catch(function (error) {
            displayPopupNotification(error, "error");
        })
    function applySideBySideLayout() {
        $("#productList")
            .data("kendoMultiSelect")
            .list
            .find(".k-list-ul")
            .css({
                display: "grid",
                gridTemplateColumns: "repeat(2, 1fr)",
                gap: "8px"
            });
    }

    var selectedProductType = "";
    $("#ddlProductType").kendoDropDownList({
        optionLabel: "Select Plant...",
        dataTextField: "PRODUCT_NAME",   // <- replace with actual property
        dataValueField: "PARAM_CODE",    // <- replace with actual property
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/InternalInspectionSetupAPI/GetProductDetails",
                    dataType: "json"
                }
            }
        },
        change: function (e) {
            var selectedVal = this.value();//$(this).val(); //$(this).val();//this.value();
            selectedProductType = this.value();
            productDetails(selectedVal);
            productDetails1(selectedVal);
        },
        dataBound: function (e) {
        }
    }).data("kendoDropDownList");

    function productDetails1(selectedProductType) {
        $http.get("/api/InternalInspectionAPI/VendorDetailsList", {
            params: {
                Product: selectedProductType
            }
        }).then(function (response) {
            var qcqa = response.data;
            $scope.vendorchildModels = [];
            if (qcqa && qcqa.length > 0) {
                $scope.vendorchildModels = qcqa.map(function (item) {
                    return {
                        ITEM_EDESC: item.ITEM_EDESC,
                        ITEM_CODE: item.ITEM_CODE
                    };
                });
            }
        })
    }
    $scope.remove_vendor_child_element = function (index) {
        if ($scope.vendorchildModels.length > 1) {
            $scope.vendorchildModels.splice(index, 1);
            //$scope.vendorchildModels.splice(index, 1);
        }
    }
    $('#productList').bind("change", function (e) {
        e.preventDefault;
        var selectedItems = $("#productList").data("kendoMultiSelect").value();
        if (selectedProductType == "" || selectedProductType == null)
            selectedProductType = $('#ddlProductType').val();
        if (selectedItems.length === 0) { $('[id="idOnSiteInspection"]').remove(); }
        else { productDetails(selectedProductType, selectedItems); }
    });


    function MultiSelectProduct(productType, selectedItemCodes = []) {
        if ($('div.dynamicform').find('.k-widget.k-multiselect.k-header').length > 0) {
            var $multiSelects = $('.k-widget.k-multiselect.k-header');
            if ($multiSelects.length > 1) {
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
                    $("#productList").remove();
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

    function productDetails(selectedItems) {
        if (!selectedItems || selectedItems.length === 0) {
            return;
        }

        $http.get("/api/InternalInspectionAPI/GetParameterDetailsByItemCode", {
            params: {
                ProductId: selectedItems
            }
        }).then(function (response) {
            var qcqa = response.data;
            $scope.childModels = [];
            if (qcqa && qcqa.length > 0) {
                // Assign to scope for binding
                $scope.InternalInspectionData = qcqa;
                // Transform into childModels
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
        if (selectedItems.length === 0) { $('[id="idIntenalInspection"]').remove(); }
        else { productDetails(selectedProductType, selectedItems); } 
    });
    $scope.add_child_element = function (e) {
        $http.get('/api/InternalInspectionAPI/GetInternalInspectionList')
            .then(function (response) {
                var qcqa = response.data;
                $scope.InternalInspectionData.push({ element: qcqa });
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    $scope.remove_child_element = function (index) {
        if ($scope.InternalInspectionData.length > 1) {
            $scope.InternalInspectionData.splice(index, 1);
            $scope.childModels.splice(index, 1);
        }
    }
    var internalItemSetupList = [];
    var rawMateriallList = [];
    $scope.saveInternalInspection = function () {
        internalItemSetupList = [];
        rawMateriallList = [];
        if ($("#ddlProductType").val() == null || $("#ddlProductType").val() == "" || $("#ddlProductType").val() == undefined) {
            displayPopupNotification("Product Type is required", "warning");
            return false;
        }
        var internalitemrows = $scope.vendorchildModels;
        internalitemrows.forEach(function (row, rowIndex) {
            var internalItem = {
                ITEM_CODE: $(".itemCode_" + rowIndex)?.text().trim() || "",
                QUANTITY: $(".Quantity_" + rowIndex)?.val() || ""
            };
            internalItemSetupList.push(internalItem);
        });
        var rows = $scope.childModels;
        rows.forEach(function (row, rowIndex) {
            var rawMaterial = {
                PARAMETER_ID: $(".ParamID_" + rowIndex)?.text() || "",
                ITEM_CODE: $(".Item_" + rowIndex)?.text() || "",
                PARAMETERS: $(".Parameters_" + rowIndex)?.text() || "",
                SPECIFICATION: $(".Specification_" + rowIndex)?.text() || "",
                UNIT: $(".Unit_" + rowIndex)?.text() || "",
                TARGET: $(".Target_" + rowIndex)?.text() || "",
                TOLERENCE: $(".Tolerence_" + rowIndex)?.text() || "",
                RESULTS: $(".RESULTS_" + rowIndex)?.val() || "",
                VARIANCE: $(".Variance_" + rowIndex)?.text() || "",
                SERIAL_NO: $(".Serial_" + rowIndex)?.text() || "",
                PARAM_CODE: $(".Param_" + rowIndex)?.text() || "",
            };
            rawMateriallList.push(rawMaterial);
        });      
        var wrapper = {
            Inspection_No: $scope.Inspection_No,
            DISPATCH_PERSON: $("#txtVendor").length ? $("#txtVendor").val() : null,
            Plant_Id: $("#ddlProductType").length ? $("#ddlProductType").val() : null,
            //ITEMSETUPS: $("#productList").data("kendoMultiSelect").value(),
            CREATED_DATE: $('#englishdatedocument').length ? $('#englishdatedocument').val() : null,
            Shift: $('#ddlShift').length ? $('#ddlShift').val() : null,
            BATCH_NO: $('#idBatch_No').val(),
            REFERENCE_NO: $scope.REFERENCE_NO,
            REMARKS: $('#txtRemarks').val(),
            InternalItemSetupList: internalItemSetupList,
            ParameterDetailsList: rawMateriallList
        };
        $http.post('/api/InternalInspectionAPI/saveInternalInspection', wrapper)
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
        $http.get('/api/InternalInspectionAPI/GetEditInternalInspection?transactionno=' + $scope.voucherno)
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
                $scope.REMARKS = response.data.REMARKS;
                var dropdownShift = $("#ddlShift").data("kendoDropDownList");
                dropdownShift.value(response.data.Shift);
                $scope.selectedProductType = response.data.ITEM_CODE;
                var dropdownProductType = $("#ddlProductType").data("kendoDropDownList");
                dropdownProductType.value(response.data.ITEM_CODE);
                /* $scope.productMapping = response.data.ITEMSETUPS;*/
                $scope.vendorchildModels = response.data.InternalItemSetupList;
                $scope.DISPATCH_PERSON = response.data.DISPATCH_PERSON;
                $scope.childModels = response.data.ParameterDetailsList;
                $scope.products = response.data.PRODUCTS;
                $timeout(function () {
                    $scope.vendorchildModels.forEach(function (row, rowIndex) {
                        $(".Quantity_" + rowIndex).val(row.QUANTITY);
					})
                }, 0);
                $scope.childModels = response.data.ParameterDetailsList;
            })
            .catch(function (error) {
                displayPopupNotification("Error fetching ID", "error");
            });
    }
    $scope.CalculateVariance = function (key) {
        let i = 0;
        $(".table_body").find('tr#idIntenalInspection').each(function () {
            var target = parseFloat($(this).find('.Target_' + i).text()) || 0;
            var results = parseFloat($(this).find('.RESULTS_' + i).val()) || 0;
            var variance = target - results;
            variance = isNaN(variance) ? 0 : variance;
            $(this).find('.Variance_' + i).text(variance);
            i++;
        });
    };
});