QCQAModule.controller('HandOverInspectionList', function ($scope, $rootScope, $http, $filter, $timeout, $routeParams) {
    $scope.voucherno = "";
    if ($routeParams.voucherno != undefined) {
        $scope.voucherno = $routeParams.voucherno;//.split(new RegExp('_', 'i')).join('/');
    }
    $("#englishdatedocument").kendoDatePicker({
        value: new Date(),
        format: "dd-MMM-yyyy"
    });
    $("#nepaliDate5").val(AD2BS(kendo.toString(new Date, 'yyyy-MM-dd')));
    $scope.HandOverItemsData = [];
    $scope.childModelTemplate = [];
    $scope.childModels = [];
    $scope.HandOverItem = [];
    $scope.QCNO = '@ViewBag.QCNO';

    $scope.selectedProductType = null;  // ng-model
    if ($scope.voucherno != "") {
        $http.get('/api/HandOverInspectionAPI/GetEditHandOverInspection?transactionno=' + $scope.voucherno)
            .then(function (response) {
                var qcqa = response.data;
                $scope.DISPATCH_NO = response.data.DISPATCH_NO;
                $scope.PACKING_UNIT = response.data.PACKING_UNIT;
                var dropdown = $("#ddlPackingUnit").data("kendoDropDownList");
                dropdown.value($scope.PACKING_UNIT);
                $scope.SERIAL_NO = response.data.SERIAL_NO;
                $scope.CREATED_DATE = moment(response.data.CREATED_DATE).format('DD-MMM-YYYY');
                $('#englishdatedocument').val($scope.CREATED_DATE);
                $('#nepaliDate5').val(AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD')));
                $scope.REMARKS = response.data.REMARKS;
                $scope.OVERALL_REMARKS = response.data.OVERALL_REMARKS;
                $scope.MANUAL_NO = response.data.MANUAL_NO;
                $('.LooseProduct_SampleTotal').text(parseFloat(response.data.LooseProduct_SampleTotal) || 0);
                $('.LooseProduct_DefectTotal').text(parseFloat(response.data.LooseProduct_DefectTotal) || 0);
                $('.UnsealedPacket_SampleTotal').text(parseFloat(response.data.UnsealedPacket_SampleTotal) || 0);
                $('.UnsealedPacket_DefectTotal').text(parseFloat(response.data.UnsealedPacket_DefectTotal) || 0);
                $('.SealedPacket_SampleTotal').text(parseFloat(response.data.SealedPacket_SampleTotal) || 0);
                $('.SealedPacket_DefectTotal').text(parseFloat(response.data.SealedPacket_DefectTotal) || 0);
                $('.CartonBag_SampleTotal').text(parseFloat(response.data.CartonBag_SampleTotal) || 0);
                $('.CartonBag_DefectTotal').text(parseFloat(response.data.CartonBag_DefectTotal) || 0);
                $('.WrapperInPrinter_SampleTotal').text(parseFloat(response.data.WrapperInPrinter_SampleTotal) || 0);
                $('.WrapperInPrinter_DefectTotal').text(parseFloat(response.data.WrapperInPrinter_DefectTotal) || 0);
                $('.LooseProductTotal').text((parseFloat(response.data.LooseProduct_DefectTotal / response.data.LooseProduct_SampleTotal) || 0).toFixed(2));
                $('.UnsealedPacketTotal').text((parseFloat(response.data.UnsealedPacket_DefectTotal / response.data.UnsealedPacket_SampleTotal) || 0).toFixed(2));
                $('.SealedPacketTotal').text((parseFloat(response.data.SealedPacket_DefectTotal / response.data.SealedPacket_SampleTotal) || 0).toFixed(2));
                $('.CartonBagTotal').text((parseFloat(response.data.CartonBag_DefectTotal / response.data.CartonBag_SampleTotal) || 0).toFixed(2));
                $('.WrapperInPrinterTotal').text((parseFloat(response.data.WrapperInPrinter_DefectTotal / response.data.WrapperInPrinter_SampleTotal) || 0).toFixed(2));
                var editRawMaterials = response.data.HandOverInspectionDetailsList;
                editRawMaterials.forEach(function (row, rowIndex) {
                    $http.get('/api/ProductSetupAPI/ProductTypeList')
                        .then(function (response) {
                            var qcqa = response.data;
                            if (qcqa && qcqa.length > 0) {
                                $scope.ItemCodeOption1 = row.PRODUCT_TYPE;
                                $timeout(function () {
                                    var dropdown = $("#productType_" + rowIndex).data("kendoComboBox");
                                    if (dropdown) {
                                        dropdown.value(row.PRODUCT_TYPE);
                                    }
                                }, 100);
                            }
                        })
                        .catch(function (error) {
                            displayPopupNotification(error, "error");
                        })
                    $http.get('/api/ProductSetupAPI/ProductList?ProductType=' + row.PRODUCT_TYPE + "&Category_Code=" + "")
                        .then(function (response) {
                            var qcqa1 = response.data;
                            if (qcqa1 && qcqa1.length > 0) {
                                $scope.ItemCodeOption = row.ITEM_CODE;
                                $timeout(function () {
                                    var dropdown_productName = $("#productName_" + rowIndex).data("kendoComboBox");
                                    if (dropdown_productName) {
                                        dropdown_productName.value(row.ITEM_EDESC);
                                        $('.hidProductNameCode_' + rowIndex).val(row.ITEM_CODE);
                                    }
                                }, 100);
                            }
                        })
                        .catch(function (error) {
                            displayPopupNotification(error, "error");
                        })
                    $http.get('/api/RawMaterialAPI/GetBatchNoByTransactionNo?TransactionNo=' + row.BATCH_EDESC)
                        .then(function (response) {
                            var qcqa1 = response.data;
                            if (qcqa1 && qcqa1.length > 0) {
                                $scope.ItemCodeBatchOption = row.BATCH_NO;
                                $timeout(function () {
                                    var dropdown_batchno = $("#batch_" + rowIndex).data("kendoComboBox");
                                    if (dropdown_batchno) {
                                        dropdown_batchno.value(row.BATCH_NO);
                                    }
                                }, 100);
                            }
                        })
                        .catch(function (error) {
                            displayPopupNotification(error, "error");
                        })
                    var dropdown_vendor = $("#batch_" + rowIndex).data("kendoComboBox");
                    $("#batch_" + rowIndex).val(row.BATCH_NO);
                    if (dropdown_vendor) {
                        dropdown_vendor.value(row.BATCH_NO); // Set the selected value
                        dropdown_vendor.trigger("change");    // Optional: trigger change if you need dependent behavior
                    }
                    var rawMaterialRow = [
                        { COLUMN_NAME: "TIME_PERIOD", VALUE: row.TIME_PERIOD },
                        { COLUMN_NAME: "PRODUCT_TYPE", VALUE: row.PRODUCT_TYPE },
                        { COLUMN_NAME: "ITEM_EDESC", VALUE: row.ITEM_EDESC },
                        { COLUMN_NAME: "BATCH_NO", VALUE: row.BATCH_NO },
                        { COLUMN_NAME: "LOOSE_PRODUCT_SAMPLE", VALUE: row.LOOSE_PRODUCT_SAMPLE },
                        { COLUMN_NAME: "LOOSE_PRODUCT_DEFECT", VALUE: row.LOOSE_PRODUCT_DEFECT },
                        { COLUMN_NAME: "UNSEALED_PACKET_SAMPLE", VALUE: row.UNSEALED_PACKET_SAMPLE },
                        { COLUMN_NAME: "UNSEALED_PACKET_DEFECT", VALUE: row.UNSEALED_PACKET_DEFECT },
                        { COLUMN_NAME: "SEALED_PACKET_SAMPLE", VALUE: row.SEALED_PACKET_SAMPLE },
                        { COLUMN_NAME: "SEALED_PACKET_DEFECT", VALUE: row.SEALED_PACKET_DEFECT },
                        { COLUMN_NAME: "BAG_SAMPLE", VALUE: row.BAG_SAMPLE },
                        { COLUMN_NAME: "BAG_DEFECT", VALUE: row.BAG_DEFECT },
                        { COLUMN_NAME: "WRAPPER_SAMPLE", VALUE: row.WRAPPER_SAMPLE },
                        { COLUMN_NAME: "WRAPPER_DEFECT", VALUE: row.WRAPPER_DEFECT },
                        { COLUMN_NAME: "REMARKS", VALUE: row.REMARKS }
                    ];

                    var rawMaterialRow_Item = [
                        { COLUMN_NAME: "ITEM_CODE", VALUE: row.ITEM_CODE }
                    ];
                    $scope.HandOverItemsData.push({ element: rawMaterialRow });
                    $scope.childModels = $scope.childModels || [];
                    $scope.childModels[rowIndex] = {};
                    rawMaterialRow.forEach(function (item) {
                        $scope.childModels[rowIndex][item.COLUMN_NAME] = item.VALUE;
                    });
                    $scope.HandOverItem = $scope.HandOverItem || [];
                    $scope.HandOverItem[rowIndex] = {};
                    rawMaterialRow_Item.forEach(function (item) {
                        $scope.HandOverItem[rowIndex][item.COLUMN_NAME] = item.VALUE;
                    });
                   /* $scope.CalculateTotal();*/
                });
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    else {
        $http.get('/api/HandOverInspectionAPI/GetHandOverInspectionList')
            .then(function (response) {
                var qcqa = response.data;
                if (qcqa && qcqa.length > 0) {
                    $scope.HandOverItemsData.push({ element: qcqa });                    
                }
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    $('#ddlProductType').change(function (e) {
        e.preventDefault;
    });
    $scope.add_child_element = function (e) {
        $http.get('/api/HandOverInspectionAPI/GetHandOverInspectionList')
            .then(function (response) {
                var qcqa = response.data;
                if (qcqa && qcqa.length > 0) {
                    $scope.HandOverItemsData.push({ element: qcqa });
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
    // remove child row.
    $scope.remove_child_element = function (index) {
        if ($scope.HandOverItemsData.length > 1) {
            $scope.HandOverItemsData.splice(index, 1);
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
    var HandOverItemsDataList = [];
    $scope.saveDispatchInspection = function () {
        var rows = document.querySelectorAll("#idDispatchItems");
        for (var rowIndex = 0; rowIndex < rows.length; rowIndex++) {
            var row = rows[rowIndex];
            if ($(".TIME_PERIOD_" + rowIndex).val() == null || $(".TIME_PERIOD_" + rowIndex).val() == "" || $(".TIME_PERIOD_" + rowIndex).val() == undefined) {
                displayPopupNotification("Time Period is required", "warning");
                return false; 
            }
            if (row.querySelector("#ProductType_" + rowIndex).value == null || row.querySelector("#ProductType_" + rowIndex).value == "" || row.querySelector("#ProductType_" + rowIndex).value == undefined) {
                displayPopupNotification("Product Type is required", "warning");
                return false;
            }
            if (row.querySelector(".hidProductNameCode_" + rowIndex).value == null || row.querySelector(".hidProductNameCode_" + rowIndex).value == "" || row.querySelector(".hidProductNameCode_" + rowIndex).value == undefined) {
                displayPopupNotification("Product Name is required", "warning");
                return false;
            }
            var dispatchDetail = {
                TIME_PERIOD: $(".TIME_PERIOD_" + rowIndex)?.val() || "",
                PRODUCT_TYPE: row.querySelector("#ProductType_" + rowIndex)?.value || "",
                ITEM_CODE: row.querySelector(".hidProductNameCode_" + rowIndex)?.value || "",
                BATCH_NO: row.querySelector("#batch_" + rowIndex)?.value || "",
                LOOSE_PRODUCT_SAMPLE: row.querySelector(".LOOSE_PRODUCT_SAMPLE_" + rowIndex)?.value || "",
                LOOSE_PRODUCT_DEFECT: row.querySelector(".LOOSE_PRODUCT_DEFECT_" + rowIndex)?.value || "",
                UNSEALED_PACKET_SAMPLE: row.querySelector(".UNSEALED_PACKET_SAMPLE_" + rowIndex)?.value || "",
                UNSEALED_PACKET_DEFECT: row.querySelector(".UNSEALED_PACKET_DEFECT_" + rowIndex)?.value || "",
                SEALED_PACKET_SAMPLE: row.querySelector(".SEALED_PACKET_SAMPLE_" + rowIndex)?.value || "",
                SEALED_PACKET_DEFECT: row.querySelector(".SEALED_PACKET_DEFECT_" + rowIndex)?.value || "",
                BAG_SAMPLE: row.querySelector(".BAG_SAMPLE_" + rowIndex)?.value || "",
                BAG_DEFECT: row.querySelector(".BAG_DEFECT_" + rowIndex)?.value || "",
                WRAPPER_SAMPLE: row.querySelector(".WRAPPER_SAMPLE_" + rowIndex)?.value || "",
                WRAPPER_DEFECT: row.querySelector(".WRAPPER_DEFECT_" + rowIndex)?.value || "",
                REMARKS: row.querySelector(".REMARKS_" + rowIndex)?.value || ""
            };
            HandOverItemsDataList.push(dispatchDetail);
        }
        var wrapper = {
            DISPATCH_NO: $scope.DISPATCH_NO,
            MANUAL_NO: $('#txtManualNo').val(),
            PACKING_UNIT: $('#ddlPackingUnit').val(),
            REMARKS: $('#txtRemarks').val(),
            OVERALL_REMARKS: $('#txtOverAllRemarks').val(),
            CREATED_DATE: $('#englishdatedocument').val(),
            HandOverInspectionDetailsList: HandOverItemsDataList,
        };
        $http.post('/api/HandOverInspectionAPI/saveHandOverInspection', wrapper)
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

    function showCustomAlert(msg) {
        var $alert = $('.custom-alert');
        $alert.text(msg).fadeIn(200);

        setTimeout(function () {
            $alert.fadeOut(300);
        }, 2000);
    }

    $scope.CalculateTotal = function (key) {
        let looseProductSum = 0;
        let looseProductDefectSum = 0;
        let unSealedPacketSum = 0;
        let unSealedPacketDefectSum = 0;
        let sealedPacketSampleSum = 0;
        let sealedPacketDefectSum = 0;
        let bagSampleSum = 0;
        let bagDefectSum = 0;
        let wrapperSampleSum = 0;
        let wrapperDefectSum = 0;
        let i = 0;
        $(".table_body").find('tr#idDispatchItems').each(function () {
            var looseProduct = $(this).find('.LOOSE_PRODUCT_SAMPLE_' + i).val();
            let looseProductVal = parseFloat(looseProduct) || 0;
            looseProductSum += looseProductVal;
            var looseProductDefect = $(this).find('.LOOSE_PRODUCT_DEFECT_' + i).val();
            let looseProductDefectVal = parseFloat(looseProductDefect) || 0;
            if (looseProductDefectVal > looseProductVal){$timeout(function () {showCustomAlert('Defect cannot be greater than Sample - Loose Product.');}, 0); $('.LOOSE_PRODUCT_DEFECT_' + i).val(0); looseProductDefectVal = 0;}
            looseProductDefectSum += looseProductDefectVal;          
            var unSealedPacket = $(this).find('.UNSEALED_PACKET_SAMPLE_' + i).val();
            let unSealedPacketVal = parseFloat(unSealedPacket) || 0;
            unSealedPacketSum += unSealedPacketVal;
            var unSealedPacketDefect = $(this).find('.UNSEALED_PACKET_DEFECT_' + i).val();
            let unSealedPacketDefectVal = parseFloat(unSealedPacketDefect) || 0;
            if (unSealedPacketDefectVal > unSealedPacketVal) { $timeout(function () {showCustomAlert('Defect cannot be greater than Sample - Unsealed Packet.'); }, 0); $('.UNSEALED_PACKET_DEFECT_' + i).val(0); unSealedPacketDefectVal = 0; }
            unSealedPacketDefectSum += unSealedPacketDefectVal;
            var sealedPacketSample = $(this).find('.SEALED_PACKET_SAMPLE_' + i).val();
            let sealedPacketSampleVal = parseFloat(sealedPacketSample) || 0;
            sealedPacketSampleSum += sealedPacketSampleVal;
            var sealedPacketDefect = $(this).find('.SEALED_PACKET_DEFECT_' + i).val();
            let sealedPacketDefectVal = parseFloat(sealedPacketDefect) || 0;
            if (sealedPacketDefectVal > sealedPacketSampleVal) { $timeout(function () { showCustomAlert('Defect cannot be greater than Sample - Sealed Packet.'); }, 0); $('.SEALED_PACKET_DEFECT_' + i).val(0); sealedPacketDefectVal = 0; }
            sealedPacketDefectSum += sealedPacketDefectVal;
            var bagSample = $(this).find('.BAG_SAMPLE_' + i).val();
            let bagSampleVal = parseFloat(bagSample) || 0;
            bagSampleSum += bagSampleVal;
            var bagDefect = $(this).find('.BAG_DEFECT_' + i).val();
            let bagDefectVal = parseFloat(bagDefect) || 0;
            if (bagDefectVal > bagSampleVal) { $timeout(function () { showCustomAlert('Defect cannot be greater than Sample - Carton/Bag.'); }, 0); $('.BAG_DEFECT_' + i).val(0); bagDefectVal = 0; }
            bagDefectSum += bagDefectVal;
            var wrapperSample = $(this).find('.WRAPPER_SAMPLE_' + i).val();
            let wrapperSampleVal = parseFloat(wrapperSample) || 0;
            wrapperSampleSum += wrapperSampleVal;
            var wrapperDefect = $(this).find('.WRAPPER_DEFECT_' + i).val();
            let wrapperDefectVal = parseFloat(wrapperDefect) || 0;
            if (wrapperDefectVal > wrapperSampleVal) { $timeout(function () { showCustomAlert('Defect cannot be greater than Sample - Wrapper in printer.'); }, 0); $('.WRAPPER_DEFECT_' + i).val(0); wrapperDefectVal = 0; }
            wrapperDefectSum += wrapperDefectVal;
            i++;
        });
        
        $('.LooseProduct_SampleTotal').text(parseFloat(looseProductSum) || 0);
        $('.LooseProduct_DefectTotal').text(parseFloat(looseProductDefectSum) || 0);
        $('.UnsealedPacket_SampleTotal').text(parseFloat(unSealedPacketSum) || 0);
        $('.UnsealedPacket_DefectTotal').text(parseFloat(unSealedPacketDefectSum) || 0);
        $('.SealedPacket_SampleTotal').text(parseFloat(sealedPacketSampleSum) || 0);
        $('.SealedPacket_DefectTotal').text(parseFloat(sealedPacketDefectSum) || 0);
        $('.CartonBag_SampleTotal').text(parseFloat(bagSampleSum) || 0);
        $('.CartonBag_DefectTotal').text(parseFloat(bagDefectSum) || 0);
        $('.WrapperInPrinter_SampleTotal').text(parseFloat(wrapperSampleSum) || 0);
        $('.WrapperInPrinter_DefectTotal').text(parseFloat(wrapperDefectSum) || 0);
        $('.LooseProductTotal').text((parseFloat((looseProductDefectSum * 100) / looseProductSum) || 0).toFixed(2) + " %");
        $('.UnsealedPacketTotal').text((parseFloat((unSealedPacketDefectSum * 100) / unSealedPacketSum) || 0).toFixed(2) + " %");
        $('.SealedPacketTotal').text((parseFloat((sealedPacketDefectSum * 100) / sealedPacketSampleSum) || 0).toFixed(2) + " %");
        $('.CartonBagTotal').text((parseFloat((bagDefectSum * 100) / bagSampleSum) || 0).toFixed(2) + " %");
        $('.WrapperInPrinterTotal').text((parseFloat((wrapperDefectSum * 100) / wrapperSampleSum) || 0).toFixed(2) + " %");
    };

    //$(document).on('keyup keypress keydown', '.LOOSE_PRODUCT_DEFECT', function () {

    //    var $input = $(this);
    //    var i = $input.data('index');

    //    setTimeout(function () {

    //        var looseProductDefectVal = parseInt($input.val()) || 0;
    //        var looseProductVal = parseInt($('.LOOSE_PRODUCT_' + i).val()) || 0;

    //        if (looseProductDefectVal > looseProductVal) {

    //            $input.val(0);

    //            $input.popover('dispose');
    //            $input.popover({
    //                container: 'body',
    //                placement: 'bottom',
    //                trigger: 'manual',
    //                html: true,
    //                content: 'Defect cannot be greater than Sample.'
    //            });

    //            $input.popover('show');

    //            // Auto hide after 2 seconds
    //            setTimeout(function () {
    //                $input.popover('hide');
    //            }, 2000);
    //        }

    //    }, 0);
    //});


});