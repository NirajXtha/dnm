QCQAModule.controller('PreDispatchInspectionList', function ($scope, $rootScope, $http, $filter, $timeout, $routeParams) {
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
    $scope.YesNoOptions = {
        optionLabel: "Select Yes/No",
        dataTextField: "text",
        dataValueField: "value",
        valuePrimitive: true,
        dataSource: [
            { text: "Yes", value: "Y" },
            { text: "No", value: "N" }
        ]
    };
    var ddlUrl = window.location.protocol + "//" + window.location.host + "/api/PreDispatchInspectionAPI/GetInvoiceNoList";
    $scope.ddlCustomerInvoiceNo = {
        optionLabel: "-- Select Invoice No --",
        dataTextField: "item_edesc",
        dataValueField: "item_code",
        autoBind: true,
        filter: "contains",
        dataSource: {
            transport: {
                read: {
                    url: ddlUrl,
                    dataType: "json"
                }
            }
        },
        dataBound: function (e) {
            if ($scope.CUSTOMER_INVOICE_NO) {
                this.value($scope.CUSTOMER_INVOICE_NO);  // Set value in edit mode
            }
        },
        change: function (e) {
            var selectedVal = e.sender.value();
            if (!selectedVal) return;

            $.get("/api/PreDispatchInspectionAPI/GetDispatchDetails?InvoiceNo=" + selectedVal,
                function (response) {
                    $('#txtCustomerName').val(response.CUSTOMER_NAME);
                    $('#txtDriverName').val(response.DRIVER_NAME);
                    $('#txtDriverContactNo').val(response.DRIVER_CONTACT_NO);
                    $('#txtVehicleNo').val(response.VEHICLE_NO);
                });
        }
    };
    if ($scope.voucherno != "") {
        $http.get('/api/PreDispatchInspectionAPI/GetEditPreDispatchInspection?transactionno=' + $scope.voucherno)
            .then(function (response) {
                $scope.DISPATCH_NO = response.data.DISPATCH_NO;
                $scope.ddlCustomerInvoiceNo = response.data.CUSTOMER_INVOICE_NO;
                var dropdownProductType = $("#ddlCustomerInvoiceNo").data("kendoDropDownList");
                dropdownProductType.value(response.data.CUSTOMER_INVOICE_NO);
                $scope.CUSTOMER_NAME = response.data.CUSTOMER_NAME;
                $scope.CUSTOMER_INVOICE_NO = response.data.CUSTOMER_INVOICE_NO;
                $scope.TRANSPORT_DETAIL = response.data.TRANSPORT_DETAIL;
                $scope.DRIVER_NAME = response.data.DRIVER_NAME;
                $scope.DRIVER_CONTACT_NO = response.data.DRIVER_CONTACT_NO;
                $scope.VEHICLE_NO = response.data.VEHICLE_NO;
                $scope.DISPATCH_PERSON = response.data.DISPATCH_PERSON;
                $scope.QC_INSPECTOR = response.data.QC_INSPECTOR;
                $scope.ISDUST_VEHICLE = response.data.PreDispatchInspectionDetailsList[0].ISDUST_VEHICLE;
                $scope.VEHICLE_DUST_REMARKS = response.data.PreDispatchInspectionDetailsList[0].VEHICLE_DUST_REMARKS;
                $scope.ISWATERSPILL_VEHICLE = response.data.PreDispatchInspectionDetailsList[0].ISWATERSPILL_VEHICLE;
                $scope.VEHICLE_WATERSPILL_REMARKS = response.data.PreDispatchInspectionDetailsList[0].VEHICLE_WATERSPILL_REMARKS;
                $scope.ISCRACKSHOLES_VEHICLE = response.data.PreDispatchInspectionDetailsList[0].ISCRACKSHOLES_VEHICLE;
                $scope.VEHICLE_CRACKSHOLES_REMARKS = response.data.PreDispatchInspectionDetailsList[0].VEHICLE_CRACKSHOLES_REMARKS;
                $scope.ISNAILS_VEHICLE = response.data.PreDispatchInspectionDetailsList[0].ISNAILS_VEHICLE;
                $scope.VEHICLE_NAILS_REMARKS = response.data.PreDispatchInspectionDetailsList[0].VEHICLE_NAILS_REMARKS;
                $scope.ISLEAKWALL_VEHICLE = response.data.PreDispatchInspectionDetailsList[0].ISLEAKWALL_VEHICLE;
                $scope.VEHICLE_WALL_REMARKS = response.data.PreDispatchInspectionDetailsList[0].VEHICLE_WALL_REMARKS;
                $scope.ISVISUALDEFECT_PRODUCT = response.data.PreDispatchInspectionDetailsList[0].ISVISUALDEFECT_PRODUCT;
                $scope.PRODUCT_DEFECT_REMARKS = response.data.PreDispatchInspectionDetailsList[0].PRODUCT_DEFECT_REMARKS;
                $scope.ISDIMENSIONS_PRODUCT = response.data.PreDispatchInspectionDetailsList[0].ISDIMENSIONS_PRODUCT;
                $scope.PRODUCT_DIMENSIONS_REMARKS = response.data.PreDispatchInspectionDetailsList[0].PRODUCT_DIMENSIONS_REMARKS;
                $scope.ISWEIGHTCHECK_PRODUCT = response.data.PreDispatchInspectionDetailsList[0].ISWEIGHTCHECK_PRODUCT;
                $scope.PRODUCT_WEIGHT_REMARKS = response.data.PreDispatchInspectionDetailsList[0].PRODUCT_WEIGHT_REMARKS;
                $scope.ISCORRECT_PACKAGING = response.data.PreDispatchInspectionDetailsList[0].ISCORRECT_PACKAGING;
                $scope.PACKAGING_CORRECT_REMARKS = response.data.PreDispatchInspectionDetailsList[0].PACKAGING_CORRECT_REMARKS;
                $scope.ISSEALED_PACKAGING = response.data.PreDispatchInspectionDetailsList[0].ISSEALED_PACKAGING;
                $scope.PACKAGING_SEALED_REMARKS = response.data.PreDispatchInspectionDetailsList[0].PACKAGING_SEALED_REMARKS;
                $scope.ISPERBOX_PACKAGING = response.data.PreDispatchInspectionDetailsList[0].ISPERBOX_PACKAGING;
                $scope.PACKAGING_PERBOX_REMARKS = response.data.PreDispatchInspectionDetailsList[0].PACKAGING_PERBOX_REMARKS;
                $scope.ISSTACKING_PACKAGING = response.data.PreDispatchInspectionDetailsList[0].ISSTACKING_PACKAGING;
                $scope.PACKAGING_STACKING_REMARKS = response.data.PreDispatchInspectionDetailsList[0].PACKAGING_STACKING_REMARKS;
                $scope.ISINVOICE_DOCUMENTATION = response.data.PreDispatchInspectionDetailsList[0].ISINVOICE_DOCUMENTATION;
                $scope.DOCU_INVOICE_REMARKS = response.data.PreDispatchInspectionDetailsList[0].DOCU_INVOICE_REMARKS;
                $scope.ISQUALITY_DOCUMENTATION = response.data.PreDispatchInspectionDetailsList[0].ISQUALITY_DOCUMENTATION;
                $scope.DOCU_QUALITY_REMARKS = response.data.PreDispatchInspectionDetailsList[0].DOCU_QUALITY_REMARKS;
                $scope.ISCOMPLIANCE_DOCUMENTATION = response.data.PreDispatchInspectionDetailsList[0].ISCOMPLIANCE_DOCUMENTATION;
                $scope.DOCU_COMP_REMARKS = response.data.PreDispatchInspectionDetailsList[0].DOCU_COMP_REMARKS;
                $http.get('/api/PreDispatchInspectionAPI/GetPreDispatchInspectionCheckList')
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
        $http.get('/api/PreDispatchInspectionAPI/GetPreDispatchInspectionCheckList')
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
    var PreDispatchItemsDataList = [];
    $scope.saveDispatchInspection = function () {
        var saveData = [];
        var HandOverItemsDataList122 = [];
        var rows = document.querySelectorAll("#idPreDispatchItemsData");
        //rows.forEach(function (row, rowIndex) {
        //});
        var dispatchDetail = {
            ISDUST_VEHICLE: $("#ddlISDUST_VEHICLE").val(),// $('.ISDUST_VEHICLE').val(),
            VEHICLE_DUST_REMARKS: $('.Dust_dirt_accumulation_REMARKS').val(),
            ISWATERSPILL_VEHICLE: $('#ddlWATERSPILL_VEHICLE').val(),
            VEHICLE_WATERSPILL_REMARKS: $('.water_spill_REMARKS').val(),
            ISCRACKSHOLES_VEHICLE: $('#ddlCRACKSHOLES_VEHICLE').val(),
            VEHICLE_CRACKSHOLES_REMARKS: $('.Cracks_and_holes_REMARKS').val(),
            ISNAILS_VEHICLE: $('#ddlNAILS_VEHICLE').val(),
            VEHICLE_NAILS_REMARKS: $('.protruding_nails_or_pointed_object_REMARKS').val(),
            ISLEAKWALL_VEHICLE: $('#ddlLEAKWALL_VEHICLE').val(),
            VEHICLE_WALL_REMARKS: $('.leaking_roofs_or_wall_REMARKS').val(),
            ISVISUALDEFECT_PRODUCT: $('#ddlVISUALDEFECT_PRODUCT').val(),
            PRODUCT_DEFECT_REMARKS: $('.Visual_defects_scratches_dents_cracks_REMARKS').val(),
            ISDIMENSIONS_PRODUCT: $('#ddlDIMENSIONS_PRODUCT').val(),
            PRODUCT_DIMENSIONS_REMARKS: $('.Dimensions_and_specifications_REMARKS').val(),
            ISWEIGHTCHECK_PRODUCT: $('#ddlWEIGHTCHECK_PRODUCT').val(),
            PRODUCT_WEIGHT_REMARKS: $('.Weight_and_tolerance_checks_REMARKS').val(),
            ISCORRECT_PACKAGING: $('#ddlCORRECT_PACKAGING').val(),
            PACKAGING_CORRECT_REMARKS: $('.Correct_labeling_and_barcodes_REMARKS').val(),
            ISSEALED_PACKAGING: $('#ddlSEALED_PACKAGING').val(),
            PACKAGING_SEALED_REMARKS: $('.Packaging_integrity_no_damage_sealed_REMARKS').val(),
            ISPERBOX_PACKAGING: $('#ddlPERBOX_PACKAGING').val(),
            PACKAGING_PERBOX_REMARKS: $('.Quantity_per_box_carton_matches_REMARKS').val(),
            ISSTACKING_PACKAGING: $('#ddlSTACKING_PACKAGING').val(),
            PACKAGING_STACKING_REMARKS: $('.Stacking_REMARKS').val(),
            ISINVOICE_DOCUMENTATION: $('#ddlINVOICE_DOCUMENTATION').val(),
            DOCU_INVOICE_REMARKS: $('.Invoice_and_packing_list_REMARKS').val(),
            ISQUALITY_DOCUMENTATION: $('#ddlQUALITY_DOCUMENTATION').val(),
            DOCU_QUALITY_REMARKS: $('.Quality_assurance_certificates_REMARKS').val(),
            ISCOMPLIANCE_DOCUMENTATION: $('#ddlCOMPLIANCE_DOCUMENTATION').val(),
            DOCU_COMP_REMARKS: $('.Compliance_certificates_ISO_NS_etc_REMARKS').val()
        };
        PreDispatchItemsDataList.push(dispatchDetail);
        var wrapper = {
            DISPATCH_NO: $scope.DISPATCH_NO,
            CUSTOMER_INVOICE_NO: $('#ddlCustomerInvoiceNo').val(),
            CUSTOMER_NAME: $('#txtCustomerName').val(),
            CUSTOMER_NAME: $('#txtCustomerName').val(),
            TRANSPORT_DETAIL: $('#txtTransportDetail').val(),
            DRIVER_NAME: $('#txtDriverName').val(),
            DRIVER_CONTACT_NO: $('#txtDriverContactNo').val(),
            VEHICLE_NO: $('#txtVehicleNo').val(),
            DISPATCH_PERSON: $('#txtDispatchPerson').val(),
            QC_INSPECTOR: $('#txtQCInpector').val(),
            PreDispatchInspectionDetailsList: PreDispatchItemsDataList,
        };
        $http.post('/api/PreDispatchInspectionAPI/savePreDispatchInspection', wrapper)
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
});