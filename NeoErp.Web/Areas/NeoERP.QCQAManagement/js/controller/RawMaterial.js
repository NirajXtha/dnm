QCQAModule.controller('RawMaterialList', function ($scope, $rootScope, $http, $filter, $timeout, $routeParams) {
    $scope.voucherno = "";
    if ($routeParams.voucherno != undefined) {
        $scope.voucherno = $routeParams.voucherno;//.split(new RegExp('_', 'i')).join('/');
    }
    else { $scope.voucherno = "undefined"; }

    $scope.RawMaterialData = [];
    $scope.aditionalRawMaterialData = [];
    $scope.childModelTemplate = [];
    $scope.childModels = [];
    $scope.vendorOptions = [];
    $scope.batchOptions = [];
    $scope.QCNO = '@ViewBag.QCNO';
    $("#englishdatedocument").kendoDatePicker({
        value: new Date(),
        format: "dd-MMM-yyyy"
    });
    $("#nepaliDate5").val(AD2BS(kendo.toString(new Date, 'yyyy-MM-dd')));
    $("#ddlProductType").kendoDropDownList({
        optionLabel: "Select Product Type...",
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
            MultiSelectProduct(selectedVal);
        },
        dataBound: function (e) {
            // Called after DropDownList is loaded
            var selectedVal = this.value(); // get current value on edit
            if (selectedVal) {
                MultiSelectProduct(selectedVal); // call for initial load
            }
        }
    }).data("kendoDropDownList");
    $scope.childModels = [];
    function MultiSelectProduct(productType, selectedItemCodes = []) {
        var selectElement = $("#productList");

        if (selectElement.data("kendoMultiSelect")) {
            selectElement.data("kendoMultiSelect").destroy();
            selectElement.closest(".k-widget").find('.k-multiselect-wrap').remove();
        }
        selectElement.empty();

        $http.get("/api/ProductSetupAPI/ProductList?ProductType=" + productType + "&Category_Code=" + "RM")
            .then(function (response) {
                var items = response.data;
                var selectElement = $("#productList");
                if (selectElement.data("kendoMultiSelect")) {
                    selectElement.data("kendoMultiSelect").destroy();
                    selectElement.empty();
                }
                var multi = selectElement.kendoMultiSelect({
                    dataSource: items,
                    dataTextField: "ITEM_EDESC",
                    dataValueField: "ITEM_CODE",
                    autoClose: false
                }).data("kendoMultiSelect");
                if (selectedItemCodes) {
                    if (typeof selectedItemCodes === "string") {
                        selectedItemCodes = selectedItemCodes.split(",");
                    }
                    selectedItemCodes = selectedItemCodes.map(function (val) {
                        return val.trim();
                    });
                    multi.value(selectedItemCodes);
                }
            });
        $http.get("/api/RawMaterialAPI/GetMaterialDetailByProductType?ProductType=" + productType).then(function (response) {
            var qcqa = response.data;
            $scope.childModels = [];
            if (qcqa && qcqa.length > 0) {
                $scope.childModels = qcqa.map(function (item) {
                    return {
                        ITEM_EDESC: item.ITEM_EDESC,
                        ITEM_CODE: item.ITEM_CODE,
                        GSM: item.GSM,
                        WIDTH: item.SIZE_WIDTH,
                        STRENGTH_MD: item.STRENGTH_MD,
                        STRENGTH: item.STRENGTH,
                        THICKNESS: item.THICKNESS
                    };
                });
            }
        })
    }

    if ($scope.voucherno != "undefined") {
        var rawMateriallList = [];
        $http.get('/api/RawMaterialAPI/GetRawMaterialList')
            .then(function (response) {
                var qcqa = response.data;
                if (qcqa && qcqa.length > 0) {
                    $scope.childModelTemplate.push(qcqa);
                    $scope.RawMaterialData.push({ element: qcqa });
                }
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
        $http.get('/api/RawMaterialAPI/GetEditDailyRawMaterial?transactionno=' + $scope.voucherno)
            .then(function (response) {
                var qcqa = response.data;
                console.log("row", qcqa);
                $scope.QC_NO = response.data.QC_NO;
                $scope.ITEM_CODE = response.data.ITEM_CODE;
                var dropdown = $("#ddlProductType").data("kendoDropDownList");
                dropdown.value(response.data.ITEM_CODE);
                $scope.SERIAL_NO = response.data.SERIAL_NO;
                $scope.MANUAL_NO = response.data.MANUAL_NO;
                $scope.BATCH_NO = response.data.BATCH_NO;
                $scope.CREATED_DATE = moment(response.data.CREATED_DATE).format('DD-MMM-YYYY');
                $('#englishdatedocument').val($scope.CREATED_DATE);
                $('#nepaliDate5').val(AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD')));
                $scope.REMARKS = response.data.REMARKS;
                console.log("$scope.REMARKS", $scope.REMARKS);
                $scope.RawMaterialList = response.data.RawMaterialList;
                var editRawMaterials = response.data.RawMaterialList;
                editRawMaterials.forEach(function (row, rowIndex) {

                    // Ensure the saved supplier exists in the list
                    row.RawMaterialList = row.RawMaterialList || [];
                    if (!row.RawMaterialList.find(s => s.SUPPLIER_CODE == row.SUPPLIER_CODE)) {
                        row.RawMaterialList.push({
                            SUPPLIER_CODE: row.SUPPLIER_CODE,
                            SUPPLIER_EDESC: row.SUPPLIER_EDESC
                        });
                    }

                    // Force type match (string vs number)
                    row.SUPPLIER_CODE = String(row.SUPPLIER_CODE);
                    row.RawMaterialList.forEach(s => s.SUPPLIER_CODE = String(s.SUPPLIER_CODE));

                    // After DOM rendered, create the ComboBox manually
                    $timeout(function () {
                        var dropdown = $("#vendor_" + rowIndex).data("kendoComboBox");
                        $('#supplierCode_' + rowIndex).val(row.SUPPLIER_CODE);
                        if (!dropdown) {
                            $("#vendor_" + rowIndex).kendoComboBox({
                                dataTextField: "SUPPLIER_EDESC",
                                dataValueField: "SUPPLIER_CODE",
                                dataSource: row.RawMaterialList,
                                value: row.SUPPLIER_CODE,
                                autoBind: true,
                                filter: "contains"
                            });
                        } else {
                            dropdown.setDataSource(new kendo.data.DataSource({ data: row.RawMaterialList }));
                            dropdown.value(row.SUPPLIER_CODE);
                            $('#supplierCode_' + rowIndex).val(row.SUPPLIER_CODE);
                        }

                    }, 0); // wait for row to be rendered


                    
                    //$http.get('/api/RawMaterialAPI/GetPending_RawMaterialsByItemId?ItemCode=' + row.ITEM_CODE)
                    //        .then(function (response) {
                    //            var qcqa = response.data;
                    //            console.log("ad", qcqa);
                    //        if (qcqa && qcqa.length > 0) {
                    //            $scope.ItemCodeOption = row.SUPPLIER_CODE;
                                
                    //            $timeout(function () {
                    //                var dropdown = $("#vendor_" + rowIndex).data("kendoComboBox");
                    //                if (dropdown) {
                    //                    dropdown.value(row.SUPPLIER_EDESC);
                    //                    $('#supplierCode_' + rowIndex).val(row.SUPPLIER_CODE);
                    //                }
                    //            }, 100);
                    //        }
                    //    })
                    //    .catch(function (error) {
                    //        displayPopupNotification(error, "error");
                    //    })

                    $http.get('/api/RawMaterialAPI/GetBatchNoByTransactionNo?TransactionNo=' + row.TRANSACTION_NO)
                        .then(function (response) {
                            var qcqa = response.data;
                            if (qcqa && qcqa.length > 0) {
                                $scope.BatchOptions = row.BATCH_NO;
                                $timeout(function () {
                                    var dropdown_batch = $("#batch_" + rowIndex).data("kendoComboBox");
                                    if (dropdown_batch) {
                                        dropdown_batch.value(row.BATCH_NO);
                                        $('#transactionNo_' + rowIndex).val(row.TRANSACTION_NO);
                                    }
                                }, 100);
                            }
                        })
                        .catch(function (error) {
                            displayPopupNotification(error, "error");
                        })
                    var rawMaterialRow = [
                        { COLUMN_NAME: "ITEM_EDESC", VALUE: row.ITEM_EDESC },
                        { COLUMN_NAME: "ITEM_CODE", VALUE: row.ITEM_CODE },
                        { COLUMN_NAME: "SUPPLIER_EDESC", VALUE: row.SUPPLIER_EDESC },
                        { COLUMN_NAME: "SUPPLIER_CODE", VALUE: row.SUPPLIER_CODE },
                        { COLUMN_NAME: "BATCH_NO", VALUE: row.TRANSACTION_NO },
                        { COLUMN_NAME: "GSM", VALUE: row.GSM },
                        { COLUMN_NAME: "WIDTH", VALUE: row.WIDTH },
                        { COLUMN_NAME: "ACTUAL_WIDTH", VALUE: row.ACTUAL_WIDTH },
                        { COLUMN_NAME: "ROLL_NO", VALUE: row.ROLL_NO },
                        { COLUMN_NAME: "STRENGTH", VALUE: row.STRENGTH },
                        { COLUMN_NAME: "STRENGTH_MD", VALUE: row.STRENGTH_MD },
                        { COLUMN_NAME: "THICKNESS", VALUE: row.THICKNESS },
                        { COLUMN_NAME: "ACTUAL_GSM", VALUE: row.ACTUAL_GSM },
                        { COLUMN_NAME: "ACTUAL_SIZE_WIDTH", VALUE: row.ACTUAL_SIZE_WIDTH },
                        { COLUMN_NAME: "ACTUAL_STRENGTH", VALUE: row.ACTUAL_STRENGTH },
                        { COLUMN_NAME: "ACTUAL_STRENGTH_MD", VALUE: row.ACTUAL_STRENGTH_MD },
                        { COLUMN_NAME: "ACTUAL_THICKNESS", VALUE: row.ACTUAL_THICKNESS },
                        { COLUMN_NAME: "REMARKS", VALUE: row.REMARKS }
                    ];

                    var rawMaterialRow_Item = [
                        { COLUMN_NAME: "ITEM_CODE", VALUE: row.ITEM_CODE }
                    ];

                    $scope.RawMaterialData.push({ element: rawMaterialRow });
                    $scope.childModels = $scope.childModels || [];
                    $scope.childModels[rowIndex] = {};
                    rawMaterialRow.forEach(function (item) {
                        $scope.childModels[rowIndex][item.COLUMN_NAME] = item.VALUE;
                    });
                });
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    else {
        $http.get('/api/RawMaterialAPI/GetRawMaterialList')
            .then(function (response) {
                var qcqa = response.data;
                if (qcqa && qcqa.length > 0) {
                    $scope.RawMaterialData.push({ element: qcqa });
                    $scope.childModelTemplate.push(qcqa);
                }
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }

    $scope.add_child_element = function (e) {
        $scope.aditionalRawMaterialData.push($scope.RawMaterialData);
        $http.get('/api/RawMaterialAPI/GetRawMaterialList')
            .then(function (response) {
                var qcqa = response.data;
                if (qcqa && qcqa.length > 0) {
                    $scope.RawMaterialData.push({ element: qcqa });
                    $scope.childModelTemplate.push(qcqa);
                    $scope.childModels.push(angular.copy($scope.childModelTemplate));
                }
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
        $scope.childModels.push(angular.copy($scope.childModelTemplate));
        var childLen = $scope.childModels.length - 1;
        $.each($scope.RawMaterialData[0].element, function (childkey, childelementvalue) {
            if (childelementvalue.DEFA_VALUE != null)
                $scope.childModels[childLen][childelementvalue.COLUMN_NAME] = childelementvalue.DEFA_VALUE;
        });
    }
    $scope.remove_child_element = function (index) {
        if ($scope.childModels.length > 1) {
            $scope.childModels.splice(index, 1);
        }
    }
    var rawMateriallList = [];
    $scope.saveRawMaterial = function () {
        var rows = document.querySelectorAll("#idRawMaterial");
        if ($('#ddlProductType').val() == null || $("#ddlProductType").val() == "" || $("#ddlProductType").val() == undefined) {
            displayPopupNotification("Product Type is required", "warning");
            return false;
        }
        rows.forEach(function (row, rowIndex) {
            var scope = angular.element(row).scope();
            var gsm = (scope && scope.childModels && scope.childModels[rowIndex])
                ? scope.childModels[rowIndex]['ACTUAL_GSM']
                : (row.querySelector(".ACTUAL_GSM_" + rowIndex)?.value || "");

            var rawMaterial = {
                ITEM_CODE: row.querySelector(".itemCode_" + rowIndex)?.value || "",
                SUPPLIER_CODE: $("#supplierCode_" + rowIndex)?.val() || "",
                BATCH_NO: $("#transactionNo_" + rowIndex)?.val() || "",
                ROLL_NO: (scope && scope.childModels && scope.childModels[rowIndex])
                    ? scope.childModels[rowIndex]['ROLL_NO']
                    : (row.querySelector(".ROLL_NO_" + rowIndex)?.value || ""),
                GSM: $(".GSM_" + rowIndex)?.text() || "",
                WIDTH: $(".SIZE_WIDTH_" + rowIndex)?.text() || "",
                STRENGTH_MD: $(".STRENGTH_MD_" + rowIndex)?.text() || "",
                STRENGTH: $(".STRENGTH_" + rowIndex)?.text() || "",
                THICKNESS: $(".THICKNESS_" + rowIndex)?.text() || "",
                ACTUAL_GSM: (scope && scope.childModels && scope.childModels[rowIndex])
                    ? scope.childModels[rowIndex]['ACTUAL_GSM']
                    : (row.querySelector(".ACTUAL_GSM_" + rowIndex)?.value || ""),
                /*ACTUAL_GSM: row.querySelector(".ACTUAL_GSM_" + rowIndex)?.value || "",*/
                ACTUAL_WIDTH: (scope && scope.childModels && scope.childModels[rowIndex])
                    ? scope.childModels[rowIndex]['ACTUAL_WIDTH']
                    : (row.querySelector(".ACTUAL_WIDTH" + rowIndex)?.value || ""),
                ACTUAL_STRENGTH_MD: (scope && scope.childModels && scope.childModels[rowIndex])
                    ? scope.childModels[rowIndex]['ACTUAL_STRENGTH_MD']
                    : (row.querySelector(".ACTUAL_STRENGTH_MD_" + rowIndex)?.value || ""),
                ACTUAL_STRENGTH: (scope && scope.childModels && scope.childModels[rowIndex])
                    ? scope.childModels[rowIndex]['ACTUAL_STRENGTH']
                    : (row.querySelector(".ACTUAL_STRENGTH_" + rowIndex)?.value || ""),
                ACTUAL_THICKNESS: (scope && scope.childModels && scope.childModels[rowIndex])
                    ? scope.childModels[rowIndex]['ACTUAL_THICKNESS']
                    : (row.querySelector(".ACTUAL_THICKNESS_" + rowIndex)?.value || ""),
                REMARKS: (scope && scope.childModels && scope.childModels[rowIndex])
                    ? scope.childModels[rowIndex]['REMARKS']
                    : (row.querySelector(".REMARKS_" + rowIndex)?.value || "")
            };
            rawMateriallList.push(rawMaterial);
        });
        var wrapper = {
            ITEM_CODE: $('#ddlProductType').val(),
            QC_NO: $scope.QC_NO,
            MANUAL_NO: $('#txtManualNo').val(),
            BATCH_NO: $('#txtBatchNo').val(),
            REMARKS: $('#txtRemarks').val(),
            CREATED_DATE: $('#englishdatedocument').val(),
            RawMaterialList: rawMateriallList  // lowercase 'list'
        };
        $http.post('/api/RawMaterialAPI/saveDailyRawMaterial', wrapper)
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