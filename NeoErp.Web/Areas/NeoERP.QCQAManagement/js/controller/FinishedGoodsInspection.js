QCQAModule.controller('FinishedGoodsInspectionList', function ($scope, $rootScope, $http, $filter, $timeout, $routeParams) {
    $scope.voucherno = "";
    if ($routeParams.voucherno != undefined) {
        $scope.voucherno = $routeParams.voucherno;//.split(new RegExp('_', 'i')).join('/');
    }
    $("#englishdatedocument").kendoDatePicker({
        value: new Date(),
        format: "dd-MMM-yyyy"
    });
    $("#nepaliDate5").val(AD2BS(kendo.toString(new Date, 'yyyy-MM-dd')));
    $scope.PreDispatchItemsData = [];
    $scope.childModelTemplate = [];
    $scope.childModels = [];
    $scope.PreDispatchItem = [];
    $scope.QCNO = '@ViewBag.QCNO';
    $scope.DefectTypeOptions = {
        optionLabel: "Select Defect",
        dataTextField: "text",
        dataValueField: "value",
        valuePrimitive: true,
        dataSource: [{ text: "Qty Variance", value: "Qty_Variance" }, { text: "Print Mistake", value: "Print_Mistake" }, { text: "Packing Defect", value: "Packing_Defect" }, { text: "Others", value: "Others" }]
    };

    $scope.togglePackCondition = function () {
        if ($scope.PACK_CONDITION === 'Y') { $scope.PACK_CONDITION = null; $scope.PACK_CONDITION_NO = null; } else { $scope.PACK_CONDITION = 'Y'; $scope.PACK_CONDITION_NO = 'N'; }
    };

    $scope.togglePackConditionNo = function () {
        if ($scope.PACK_CONDITION_NO === 'Y') { $scope.PACK_CONDITION_NO = null; $scope.PACK_CONDITION = null; } else { $scope.PACK_CONDITION_NO = 'Y'; $scope.PACK_CONDITION = 'N'; }
    };
    $scope.toggleLabelAccuracy = function () {
        if ($scope.LABEL_ACCURACY === 'Y') { $scope.LABEL_ACCURACY = null; $scope.LABEL_ACCURACY_NO = null; } else { $scope.LABEL_ACCURACY = 'Y'; $scope.LABEL_ACCURACY_NO = 'N'; }
    };
     
     $scope.toggleLabelAccuracyNo = function () {
         if ($scope.LABEL_ACCURACY_NO === 'Y') { $scope.LABEL_ACCURACY_NO = null; $scope.LABEL_ACCURACY = null; } else { $scope.LABEL_ACCURACY_NO = 'Y'; $scope.LABEL_ACCURACY = 'N'; }
    };

    $scope.toggleProductAppearance = function () {
        if ($scope.PRODUCT_APPEARANCE === 'Y') { $scope.PRODUCT_APPEARANCE = null; $scope.PRODUCT_APPEARANCE_NO = null; } else { $scope.PRODUCT_APPEARANCE = 'Y'; $scope.PRODUCT_APPEARANCE_NO = 'N'; }
    };

    $scope.toggleProductAppearanceNo = function () {
        if ($scope.PRODUCT_APPEARANCE_NO === 'Y') { $scope.PRODUCT_APPEARANCE_NO = null; $scope.PRODUCT_APPEARANCE = null; } else { $scope.PRODUCT_APPEARANCE_NO = 'Y'; $scope.PRODUCT_APPEARANCE = 'N'; }
     };

    $scope.toggleDimensions = function () {
        if ($scope.DIMENSIONS === 'Y') { $scope.DIMENSIONS = null; $scope.DIMENSIONS_NO = null; } else { $scope.DIMENSIONS = 'Y'; $scope.DIMENSIONS_NO = 'N'; }
    };

     $scope.toggleDimensionsNo = function () {
         if ($scope.DIMENSIONS_NO === 'Y') { $scope.DIMENSIONS_NO = null; $scope.DIMENSIONS = null; } else { $scope.DIMENSIONS_NO = 'Y'; $scope.DIMENSIONS = 'N'; }
     };

    $scope.toggleComplianceCertificates = function () {
        if ($scope.COMPLIANCE_CERTIFICATES === 'Y') { $scope.COMPLIANCE_CERTIFICATES = null; $scope.COMPLIANCE_CERTIFICATES_NO = null; } else { $scope.COMPLIANCE_CERTIFICATES = 'Y'; $scope.COMPLIANCE_CERTIFICATES_NO = 'N'; }
    };

    $scope.toggleComplianceCertificatesNo = function () {
        if ($scope.COMPLIANCE_CERTIFICATES_NO === 'Y') { $scope.COMPLIANCE_CERTIFICATES_NO = null; $scope.COMPLIANCE_CERTIFICATES = null; } else { $scope.COMPLIANCE_CERTIFICATES_NO = 'Y'; $scope.COMPLIANCE_CERTIFICATES = 'N'; }
    };
        
    $scope.toggleVendorTest = function () {
        if ($scope.VENDOR_TEST === 'Y') { $scope.VENDOR_TEST = null; $scope.VENDOR_TEST_NO = null; } else { $scope.VENDOR_TEST = 'Y'; $scope.VENDOR_TEST_NO = 'N'; }
    };

    $scope.toggleVendorTestNo = function () {
        if ($scope.VENDOR_TEST_NO === 'Y') { $scope.VENDOR_TEST_NO = null; $scope.VENDOR_TEST = null; } else { $scope.VENDOR_TEST_NO = 'Y'; $scope.VENDOR_TEST = 'N'; }
    };

    $scope.toggleNumberPassed = function () {
        if ($scope.NUMBER_PASSED === 'Y') { $scope.NUMBER_PASSED = null; $scope.NUMBER_PASSED_NO = null; } else { $scope.NUMBER_PASSED = 'Y'; $scope.NUMBER_PASSED_NO = 'N'; }
    };

    $scope.toggleNumberPassedNo = function () {
        if ($scope.NUMBER_PASSED_NO === 'Y') { $scope.NUMBER_PASSED_NO = null; $scope.NUMBER_PASSED = null; } else { $scope.NUMBER_PASSED_NO = 'Y'; $scope.NUMBER_PASSED = 'N'; }
    };

    function formatDate(d) {

        // Remove the time part if present
        d = d.split("T")[0];    // "2025-12-17"

        var parts = d.split("-");  // ["2025", "12", "17"]

        var year = parts[0];
        var month = parseInt(parts[1]) - 1;  // JS month index
        var day = parts[2];

        // Construct date WITHOUT timezone shift
        var date = new Date(year, month, day);

        var dayFormatted = ("0" + date.getDate()).slice(-2);
        var monthShort = date.toLocaleString("en-US", { month: "short" });
        var yearFormatted = date.getFullYear();

        return dayFormatted + "-" + monthShort + "-" + yearFormatted;
    }
    if ($scope.voucherno != "") {
        $http.get('/api/FinishedGoodsInspectionAPI/GetEditFinishedGoodsInspection?transactionno=' + $scope.voucherno)
            .then(function (response) {
                $scope.selectedProductType = response.data.Plant_Id;
                var dropdownProductType = $("#ddlProductType").data("kendoDropDownList");
                dropdownProductType.value(response.data.Plant_Id);
                SelectProductByProductType(response.data.Plant_Id, function (items) {
                    $scope.item_code = response.data.ITEM_CODE || "";
                    var ddl = $("#ddlMaterials").data("kendoDropDownList");
                    ddl.value($scope.item_code);   // <-- now value exists
                    console.log("$scope.item_code", $scope.item_code);
                    ddl.trigger("change");
                });
               // $scope.RECEIPT_DATE = response.data.RECEIPT_DATE;

               

                $scope.RECEIPT_DATE = formatDate(response.data.RECEIPT_DATE);

                $scope.GRN_NO = response.data.GRN_NO;
                $scope.VENDOR_NAME = response.data.VENDOR_NAME;
                $scope.REFERENCE_NO = response.data.REFERENCE_NO;
                $scope.BATCH_NO = response.data.BATCH_NO;
                //$scope.MFG_DATE = response.data.MFG_DATE;
                //$scope.EXP_DATE = response.data.EXP_DATE;
                $scope.MFG_DATE = formatDate(response.data.MFG_DATE);
                $scope.EXP_DATE = formatDate(response.data.EXP_DATE);
                $scope.QUANTITY = response.data.QUANTITY;
                $scope.FINISH_GOODS_INSP_NO = response.data.FINISH_GOODS_INSP_NO;
                $scope.PACK_CONDITION = response.data.FinishedGoodsInspectionDetailsList[0].PACK_CONDITION;
                if ($scope.PACK_CONDITION == 'N')
                    $scope.PACK_CONDITION_NO = 'Y';
                $scope.PACK_COND_REMARKS = response.data.FinishedGoodsInspectionDetailsList[0].PACK_COND_REMARKS;
                $scope.LABEL_ACCURACY = response.data.FinishedGoodsInspectionDetailsList[0].LABEL_ACCURACY;
                if ($scope.LABEL_ACCURACY == 'N')
                    $scope.LABEL_ACCURACY_NO = 'Y';
                $scope.LABEL_ACC_REMARKS = response.data.FinishedGoodsInspectionDetailsList[0].LABEL_ACC_REMARKS;
                $scope.PRODUCT_APPEARANCE = response.data.FinishedGoodsInspectionDetailsList[0].PRODUCT_APPEARANCE;
                if ($scope.PRODUCT_APPEARANCE == 'N')
                    $scope.PRODUCT_APPEARANCE_NO = 'Y';
                $scope.PRODUCT_APP_REMARKS = response.data.FinishedGoodsInspectionDetailsList[0].PRODUCT_APP_REMARKS;
                $scope.DIMENSIONS = response.data.FinishedGoodsInspectionDetailsList[0].DIMENSIONS;
                if ($scope.DIMENSIONS == 'N')
                    $scope.DIMENSIONS_NO = 'Y';
                $scope.DIMENSIONS_REMARKS = response.data.FinishedGoodsInspectionDetailsList[0].DIMENSIONS_REMARKS;
                $scope.COMPLIANCE_CERTIFICATES = response.data.FinishedGoodsInspectionDetailsList[0].COMPLIANCE_CERTIFICATES;
                if ($scope.COMPLIANCE_CERTIFICATES == 'N')
                    $scope.COMPLIANCE_CERTIFICATES_NO = 'Y';
                $scope.COMP_CERT_REMARKS = response.data.FinishedGoodsInspectionDetailsList[0].COMP_CERT_REMARKS;
                $scope.VENDOR_TEST = response.data.FinishedGoodsInspectionDetailsList[0].VENDOR_TEST;
                if ($scope.VENDOR_TEST == 'N')
                    $scope.VENDOR_TEST_NO = 'Y';
                $scope.VENDOR_TEST_REMARKS = response.data.FinishedGoodsInspectionDetailsList[0].VENDOR_TEST_REMARKS;
                $scope.SAMPLING_METHOD = response.data.FinishedGoodsInspectionDetailsList[0].SAMPLING_METHOD;
                $scope.SAMP_METHOD_REMARKS = response.data.FinishedGoodsInspectionDetailsList[0].SAMP_METHOD_REMARKS;
                $scope.SAMPLE_SIZE = response.data.FinishedGoodsInspectionDetailsList[0].SAMPLE_SIZE;
                $scope.SAMP_SIZE_REMARKS = response.data.FinishedGoodsInspectionDetailsList[0].SAMP_SIZE_REMARKS;
                $scope.NUMBER_PASSED = response.data.FinishedGoodsInspectionDetailsList[0].NUMBER_PASSED;
                if ($scope.NUMBER_PASSED == 'N')
                    $scope.NUMBER_PASSED_NO = 'Y';
                $scope.NUMBER_PASSED_REMARKS = response.data.FinishedGoodsInspectionDetailsList[0].NUMBER_PASSED_REMARKS;
                $scope.DEFECT_TYPE = response.data.FinishedGoodsInspectionDetailsList[0].DEFECT_TYPE;
                $scope.DEFECT_TYPE_REMARKS = response.data.FinishedGoodsInspectionDetailsList[0].DEFECT_TYPE_REMARKS;
                $scope.ACTION_TAKEN = response.data.FinishedGoodsInspectionDetailsList[0].ACTION_TAKEN;
                $scope.ACTION_TAKEN_REMARKS = response.data.FinishedGoodsInspectionDetailsList[0].ACTION_TAKEN_REMARKS;
                $scope.REMARKS = response.data.FinishedGoodsInspectionDetailsList[0].REMARKS;
                $scope.FINAL_REMARKS = response.data.FinishedGoodsInspectionDetailsList[0].FINAL_REMARKS;

                $http.get('/api/FinishedGoodsInspectionAPI/GetFinishedGoodsInspectionField')
                    .then(function (response) {
                        var qcqa = response.data;
                        if (qcqa && qcqa.length > 0) {

                            $scope.PreDispatchItemsData = qcqa;
                            $scope.childModelTemplate.push(qcqa);
                        }
                    });
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    else {
        $http.get('/api/FinishedGoodsInspectionAPI/GetFinishedGoodsInspectionField')
            .then(function (response) {
                var qcqa = response.data;
                if (qcqa && qcqa.length > 0) {
                    $scope.PreDispatchItemsData = qcqa;
                    $scope.childModelTemplate.push(qcqa);
                }
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    $scope.add_child_element = function (e) {
        $http.get('/api/PreDispatchInspectionAPI/GetPreDispatchInspectionList')
            .then(function (response) {
                var qcqa = response.data;
                if (qcqa && qcqa.length > 0) {
                    $scope.PreDispatchItemsData.push({ element: qcqa });
                    $scope.childModelTemplate.push(qcqa);
                    $scope.childModels.push(angular.copy($scope.childModelTemplate));
                    //$scope.childModelTemplate.push(qcqa);
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
        if ($scope.PreDispatchItemsData.length > 1) {
            $scope.PreDispatchItemsData.splice(index, 1);
            $scope.childModels.splice(index, 1);
        }
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
                $scope.ParticularItemCode = data.data[0].ItemCode;
            });
        } catch (e) {
            return;
        }
    };
    $timeout(function () {
        $scope.SamplingMethodOptions = {
            optionLabel: "Select",
            dataTextField: "text",
            dataValueField: "value",
            valuePrimitive: true,
            dataSource: [
                { text: "Random", value: "R" },
                { text: "AllSamples", value: "A" }
            ]
        };
    }, 0);
    var PreDispatchItemsDataList = [];
    $scope.saveFinishedGoods = function () {
        if ($('#ddlProductType').val() == null || $("#ddlProductType").val() == "" || $("#ddlProductType").val() == undefined) {
            displayPopupNotification("Product Type is required", "warning");
            return false;
        }
        if ($('#ddlMaterials').val() == null || $('#ddlMaterials').val() == "" || $('#ddlMaterials').val() == undefined) {
            displayPopupNotification("Product Name is required", "warning");
            return false;
        }
        var wrapper = {
            FINISH_GOODS_INSP_NO: $scope.FINISH_GOODS_INSP_NO,
            Plant_Id: $('#ddlProductType').val(),
            ITEM_CODE: $('#ddlMaterials').val(),
            RECEIPT_DATE: $('#txtReceiptDate').val(),
            GRN_NO: $('#txtGrnNo').val(),
            VENDOR_NAME: $('#txtVendorName').val(),
            BATCH_NO: $('#txtBatchNo').val(),
            MFG_DATE: $('#txtMFGDate').val(),
            EXP_DATE: $('#txtExpiryDate').val(),
            REFERENCE_NO: $('#txtReferenceNo').val(),
            QUANTITY: $('#txtReceivedQty').val(),
            CREATED_DATE: $('#englishdatedocument').val(),
            PACK_CONDITION: $('.PACK_CONDITION').is(':checked') ? 'Y' : ($('.PACK_CONDITION_NO').is(':checked') ? 'N' : null),
            PACK_CONDITION_NO: ($('.PACK_CONDITION').is(':checked') ? 'Y' : 'N') === 'Y' ? 'N' : ($('.PACK_CONDITION_NO').is(':checked') ? 'Y' : null),
            PACK_COND_REMARKS: $('.PACK_COND_REMARKS').val(),
            LABEL_ACCURACY: $('.LABEL_ACCURACY').is(':checked') ? 'Y' : ($('.LABEL_ACCURACY_NO').is(':checked') ? 'N' : null),
            LABEL_ACCURACY_NO: ($('.LABEL_ACCURACY').is(':checked') ? 'Y' : 'N') === 'Y' ? 'N' : ($('.LABEL_ACCURACY_NO').is(':checked') ? 'Y' : null),
            LABEL_ACC_REMARKS: $('.LABEL_ACC_REMARKS').val(),
            PRODUCT_APPEARANCE: $('.PRODUCT_APPEARANCE').is(':checked') ? 'Y' : ($('.PRODUCT_APPEARANCE_NO').is(':checked') ? 'N' : null),
            PRODUCT_APPEARANCE_NO: ($('.PRODUCT_APPEARANCE').is(':checked') ? 'Y' : 'N') === 'Y' ? 'N' : ($('.PRODUCT_APPEARANCE_NO').is(':checked') ? 'Y' : null),
            PRODUCT_APP_REMARKS: $('.PRODUCT_APP_REMARKS').val(),
            DIMENSIONS: $('.DIMENSIONS').is(':checked') ? 'Y' : ($('.DIMENSIONS_NO').is(':checked') ? 'N' : null),
            DIMENSIONS_NO: ($('.DIMENSIONS').is(':checked') ? 'Y' : 'N') === 'Y' ? 'N' : ($('.DIMENSIONS_NO').is(':checked') ? 'Y' : null),
            DIMENSIONS_REMARKS: $('.DIMENSIONS_REMARKS').val(),
            COMPLIANCE_CERTIFICATES: $('.COMPLIANCE_CERTIFICATES').is(':checked') ? 'Y' : ($('.COMPLIANCE_CERTIFICATES_NO').is(':checked') ? 'N' : null),
            COMPLIANCE_CERTIFICATES_NO: ($('.COMPLIANCE_CERTIFICATES').is(':checked') ? 'Y' : 'N') === 'Y' ? 'N' : ($('.COMPLIANCE_CERTIFICATES_NO').is(':checked') ? 'Y' : null),
            COMP_CERT_REMARKS: $('.COMP_CERT_REMARKS').val(),
            VENDOR_TEST: $('.VENDOR_TEST').is(':checked') ? 'Y' : ($('.VENDOR_TEST_NO').is(':checked') ? 'N' : null),
            VENDOR_TEST_NO: ($('.VENDOR_TEST').is(':checked') ? 'Y' : 'N') === 'Y' ? 'N' : ($('.VENDOR_TEST_NO').is(':checked') ? 'Y' : null),
            VENDOR_TEST_REMARKS: $('.VENDOR_TEST_REMARKS').val(),
            SAMPLING_METHOD: $("#ddlSAMPLING_METHOD").val(),
            SAMP_METHOD_REMARKS: $('.SAMP_METHOD_REMARKS').val(),
            SAMPLE_SIZE: $('.Sample_Size').val(),
            SAMP_SIZE_REMARKS: $('.SAMP_SIZE_REMARKS').val(),
            NUMBER_PASSED: $('.NUMBER_PASSED').is(':checked') ? 'Y' : ($('.NUMBER_PASSED_NO').is(':checked') ? 'N' : null),
            NUMBER_PASSED_NO: ($('.NUMBER_PASSED').is(':checked') ? 'Y' : 'N') === 'Y' ? 'N' : ($('.NUMBER_PASSED_NO').is(':checked') ? 'Y' : null),
            NUMBER_PASSED_REMARKS: $('.NUMBER_PASSED_REMARKS').val(),
            DEFECT_TYPE: $('#ddlDEFECT_TYPE').val(), //$('#ddlDEFECT_TYPE').val(),
            DEFECT_TYPE_REMARKS: $('.DEFECT_TYPE_REMARKS').val(),
            ACTION_TAKEN: $('.Action_Taken').val(),
            ACTION_TAKEN_REMARKS: $('.ACTION_TAKEN_REMARKS').val(),
            REMARKS: $('.Remarks').val(),
            FINAL_REMARKS: $('.FINAL_REMARKS').val()
        };
        $http.post('/api/FinishedGoodsInspectionAPI/saveFinishedGoodsInspection', wrapper)
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
            SelectProductByProductType(selectedVal);
        },
        dataBound: function (e) {
        }
    }).data("kendoDropDownList");

    function SelectProductByProductType(productType, callback) {
        $http.get("/api/FinishedGoodsInspectionAPI/ProductWithCategoryFilter?ProductType=" + productType)
            .then(function (response) {
                var items = response.data;
                // Destroy existing KendoDropDownList if already created
                var ddl = $("#ddlMaterials").data("kendoDropDownList");
                if (ddl) {
                    ddl.destroy();
                    $("#ddlMaterials").empty();
                }
                // Create new Kendo DropDownList
                $("#ddlMaterials").kendoDropDownList({
                    dataSource: items,
                    dataTextField: "ITEM_EDESC",
                    dataValueField: "ITEM_CODE",
                    optionLabel: "-- Select Product --",
                    autoClose: true
                });

                // Callback to send items back
                if (callback) callback(items);
            });
    }

    //function SelectProductByProductType(productType) {
    //    $http.get("/api/ProductSetupAPI/ProductList?ProductType=" + productType)
    //        .then(function (response) {
    //            var items = response.data;
    //            // Destroy existing multiselect if already created
    //            var selectElement = $("#productList");
    //            if (selectElement.data("kendoMultiSelect")) {
    //                selectElement.data("kendoMultiSelect").destroy();
    //                selectElement.empty();
    //            }
    //            // Initialize MultiSelect with proper datasource
    //            $('#ddlMaterials').kendoDropDownList({
    //                optionLabel: "Select Product",
    //                dataSource: items,
    //                dataTextField: "ITEM_EDESC",
    //                dataValueField: "ITEM_CODE",
    //                autoClose: false
    //            }).data("kendoDropDownList");
    //        });
    //}
});