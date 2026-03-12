QCQAModule.controller('GlobalAgroProductsList', function ($scope, $rootScope, $http, $filter, $timeout, $routeParams) {
    $scope.voucherno = "";
    if ($routeParams.voucherno != undefined) {
        $scope.voucherno = $routeParams.voucherno;//.split(new RegExp('_', 'i')).join('/');     
    }
    $("#englishdatedocument").kendoDatePicker({
        value: new Date(),
        format: "dd-MMM-yyyy"
    });
    $("#nepaliDate5").val(AD2BS(kendo.toString(new Date, 'yyyy-MM-dd')));
    $scope.WeightDetailsData = [];
    $scope.UNLOADEDCHHALLIList = [];
    $scope.DAKHILADETAILSList = [];
    $scope.aditionalRawMaterialData = [];
    $scope.childModelTemplate = [];
    $scope.childWeightDetailsModels = [];
    $scope.childUnloadedChalliModels = [];
    $scope.childDakhilaDetailsModels = [];
    $scope.QCNO = '@ViewBag.QCNO';
    $scope.ActualTransaction_no = "";
    //window.sessionStorage.clear();
    //sessionStorage.clear();
    $scope.formCode = $('#idFormCode').text();
    
    if ($scope.voucherno != "") {
        window.sessionStorage.clear();
        sessionStorage.clear();
        window.sessionStorage.setItem("TRANSACTION_NO", $scope.voucherno);
        $scope.ActualTransaction_no = $scope.voucherno;
        $http.get('/api/GlobalAgroProductsAPI/GetEditGlobalAgroProductLists?transactionno=' + $scope.voucherno)
            .then(function (response) {
                var qcqa = response.data;
                $scope.TRANSACTION_NO = response.data.TRANSACTION_NO;               
                $scope.VEHICLE_NO = response.data.VEHICLE_NO;
                $scope.PARTY_NAME = response.data.PARTY_NAME;
                $scope.ADDRESS = response.data.ADDRESS;
                $scope.BILL_NO = response.data.BILL_NO;
                $scope.GATE_OR_GRN_NO = response.data.GATE_OR_GRN_NO;
                $scope.ITEM_CODE = response.data.ITEM_CODE;
                var dropdown = $("#ddlMaterials").data("kendoDropDownList");
                dropdown.value($scope.ITEM_CODE);
                $scope.SERIAL_NO = response.data.SERIAL_NO;
                //$scope.CREATED_DATE = moment(response.data.CREATED_DATE).format('dd-MMM-yyyy');
                $scope.CREATED_DATE = moment(response.data.CREATED_DATE).format('DD-MMM-YYYY');
                $('#englishdatedocument').val($scope.CREATED_DATE);
                $('#nepaliDate5').val(AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD')));
                $scope.PRODUCT_REMARKS = response.data.PRODUCT_REMARKS;
                $scope.REMARKS = response.data.REMARKS;
                $scope.PHYSICAL_TEST_RAWMATERIAL = response.data.PHYSICAL_TEST_RAWMATERIAL;
                window.sessionStorage.setItem("PHYSICAL_TEST_RAWMATERIAL", response.data.PHYSICAL_TEST_RAWMATERIAL);
                var physicalTestRawMaterial = window.sessionStorage.getItem("PHYSICAL_TEST_RAWMATERIAL");
                $scope.WEIGHT = response.data.WEIGHT;
                $scope.MOISTURE = response.data.MOISTURE;
                $scope.TEMPERATURE = response.data.TEMPERATURE;
                $scope.WET = response.data.WET;
                $scope.FUNGUS = response.data.FUNGUS;
                $scope.DUST = response.data.DUST;
                $scope.GRADING = response.data.GRADING;
                $scope.SMELL = response.data.SMELL;
                $scope.COLOR = response.data.COLOR;
                $scope.PIECES = response.data.PIECES;
                $scope.IMMATURITY_OF_GRAINS = response.data.IMMATURITY_OF_GRAINS;
                $scope.OTHER_ITEMS = response.data.OTHER_ITEMS;
                $scope.ROTTEN_HOLED = response.data.ROTTEN_HOLED;
                $scope.DAMAGED = response.data.DAMAGED;
                $scope.BROKEN = response.data.BROKEN;
                $scope.HUSK = response.data.HUSK;
                $scope.OVERTOASTED = response.data.OVERTOASTED;
                $scope.USEABLE = response.data.USEABLE;
                $scope.UNUSEABLE = response.data.UNUSEABLE;
                $scope.FAT = response.data.FAT;
                $scope.QUALITY_OF_GOODS = response.data.QUALITY_OF_GOODS;
                $scope.EXCELLENT = response.data.EXCELLENT;
                $scope.GREAT = response.data.GREAT;
                $scope.GOODS_NORMAL = response.data.GOODS_NORMAL;
                $scope.WAREHOUSE = response.data.WAREHOUSE;
                $scope.SILO = response.data.SILO;
                $scope.GHAN = response.data.GHAN;
                $scope.PROTEIN = response.data.PROTEIN;
                $scope.QUALITY_OF_FIREWOOD = response.data.QUALITY_OF_FIREWOOD;
                $scope.PRODUCT_SIZE = response.data.PRODUCT_SIZE;
                $scope.PRODUCT_TYPE = response.data.PRODUCT_TYPE;
                $scope.DEDUCT_IN_BAG = response.data.DEDUCT_IN_BAG;
                $scope.DEDUCT_IN_WT = response.data.DEDUCT_IN_WT;
                $scope.NET_WEIGHT = response.data.NET_WEIGHT;
                $scope.ISPLASTIC_BAG = response.data.ISPLASTIC_BAG;
                $scope.ISJUTE_BAG = response.data.ISJUTE_BAG;
                $scope.ISPLASTIC_WEIGHT = response.data.ISPLASTIC_WEIGHT;
                $scope.ISJUTE_WEIGHT = response.data.ISJUTE_WEIGHT;
                $scope.SLIDER_VALUE = response.data.SLIDER_VALUE;
                $scope.PRODUCT_REMARKS = response.data.PRODUCT_REMARKS;
                $scope.REMARKS = response.data.REMARKS;
                $scope.UNLOAD_UNIT = response.data.UNLOAD_UNIT;
                $scope.ISPLASTIC_BAG = response.data.ISPLASTIC_BAG;
                $scope.ISUNLOAD = response.data.ISUNLOAD;
                $scope.ISPROTEIN = response.data.ISPROTEIN;
                $scope.ISPT = response.data.ISPT;
                $scope.ISOUT = response.data.ISOUT;
                $scope.DEDUCT_IN_PLASTIC = response.data.DEDUCT_IN_PLASTIC;
                $scope.DEDUCT_IN_JUTE = response.data.DEDUCT_IN_JUTE;
                var weightDetails = response.data.WEIGHTDETAILSList;
                var unloadedChhalli = response.data.UNLOADEDCHHALLIList;
                var dakhilaDetails = response.data.DAKHILADETAILSList;

                weightDetails.forEach(function (row, rowIndex) {
                    var weightDetailRow = [
                        { COLUMN_NAME: "FIRST_WEIGHT", VALUE: row.FIRST_WEIGHT },
                        { COLUMN_NAME: "SECOND_WEIGHT", VALUE: row.SECOND_WEIGHT },
                        { COLUMN_NAME: "NET_WEIGHT", VALUE: row.NET_WEIGHT },
                        { COLUMN_NAME: "CHALLAN_WEIGHT", VALUE: row.CHALLAN_WEIGHT },
                        { COLUMN_NAME: "WEIGHT_DIFFERENCE", VALUE: row.WEIGHT_DIFFERENCE },
                        { COLUMN_NAME: "REMARKS", VALUE: row.REMARKS }
                    ];
                    $scope.WeightDetailsData.push({ element: weightDetailRow });
                    $scope.childWeightDetailsModels = $scope.childWeightDetailsModels || [];
                    $scope.childWeightDetailsModels[rowIndex] = {};
                    weightDetailRow.forEach(function (item) {
                        $scope.childWeightDetailsModels[rowIndex][item.COLUMN_NAME] = item.VALUE;
                    });
                });

                unloadedChhalli.forEach(function (row, rowIndex) {
                    var unloadedDetailRow = [
                        { COLUMN_NAME: "FIRST_CHHALLI", VALUE: row.FIRST_CHHALLI },
                        { COLUMN_NAME: "SECOND_CHHALLI", VALUE: row.SECOND_CHHALLI },
                        { COLUMN_NAME: "THIRD_CHHALLI", VALUE: row.THIRD_CHHALLI },
                        { COLUMN_NAME: "FOURTH_CHHALLI", VALUE: row.FOURTH_CHHALLI },
                        { COLUMN_NAME: "FIFTH_CHHALLI", VALUE: row.FIFTH_CHHALLI },
                        { COLUMN_NAME: "SIXTH_CHHALLI", VALUE: row.SIXTH_CHHALLI },
                        { COLUMN_NAME: "SEVENTH_CHHALLI", VALUE: row.SEVENTH_CHHALLI },
                        { COLUMN_NAME: "EIGHTH_CHHALLI", VALUE: row.EIGHTH_CHHALLI },
                        { COLUMN_NAME: "NINETH_CHHALLI", VALUE: row.NINETH_CHHALLI },
                        { COLUMN_NAME: "TENTH_CHHALLI", VALUE: row.TENTH_CHHALLI },
                        { COLUMN_NAME: "ELEVEN_CHHALLI", VALUE: row.ELEVEN_CHHALLI },
                        { COLUMN_NAME: "TWELVE_CHHALLI", VALUE: row.TWELVE_CHHALLI },
                        { COLUMN_NAME: "TOTAL", VALUE: row.TOTAL },
                        { COLUMN_NAME: "REMARKS", VALUE: row.REMARKS }
                    ];
                    $scope.UNLOADEDCHHALLIList.push({ element: unloadedDetailRow });
                    $scope.childUnloadedChalliModels = $scope.childUnloadedChalliModels || [];
                    $scope.childUnloadedChalliModels[rowIndex] = {};
                    unloadedDetailRow.forEach(function (item) {
                        $scope.childUnloadedChalliModels[rowIndex][item.COLUMN_NAME] = item.VALUE;
                    });
                });

                dakhilaDetails.forEach(function (row, rowIndex) {
                    var dakhilaDetailRow = [
                        { COLUMN_NAME: "ENTRY_NO", VALUE: row.ENTRY_NO },
                        { COLUMN_NAME: "BILL_NO", VALUE: row.BILL_NO },
                        { COLUMN_NAME: "CHALAN_NO", VALUE: row.CHALAN_NO },
                        { COLUMN_NAME: "ITEM", VALUE: row.ITEM },
                        { COLUMN_NAME: "TOTAL_BAG", VALUE: row.TOTAL_BAG },
                        { COLUMN_NAME: "WEIGHT", VALUE: row.WEIGHT },
                        { COLUMN_NAME: "REMARKS", VALUE: row.REMARKS }
                    ];
                    $scope.DAKHILADETAILSList.push({ element: dakhilaDetailRow });
                    $scope.childDakhilaDetailsModels = $scope.childDakhilaDetailsModels || [];
                    $scope.childDakhilaDetailsModels[rowIndex] = {};
                    dakhilaDetailRow.forEach(function (item) {
                        $scope.childDakhilaDetailsModels[rowIndex][item.COLUMN_NAME] = item.VALUE;
                    });
                });
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })


        //var rows = responsesod.data;
        //window.sessionStorage.clear();
        //window.sessionStorage.setItem("CREATED_DATE", rows.CREATED_DATE);
        //window.sessionStorage.setItem("VEHICLE_NO", rows.VEHICLE_NO);
        //window.sessionStorage.setItem("ITEM_EDESC", rows.ITEM_EDESC);
        //window.sessionStorage.setItem("PARTY_NAME", rows.PARTY_NAME);
        //window.sessionStorage.setItem("ADDRESS", rows.ADDRESS);
        //window.sessionStorage.setItem("TELEPHONE", rows.TELEPHONE);
        //window.sessionStorage.setItem("BILL_NO", rows.BILL_NO);
        //window.sessionStorage.setItem("GATE_OR_GRN_NO", rows.GATE_OR_GRN_NO);
        //window.sessionStorage.setItem("PHYSICAL_TEST_RAWMATERIAL", rows.PHYSICAL_TEST_RAWMATERIAL);
        //var physicalTestRawMaterial = window.sessionStorage.getItem("PHYSICAL_TEST_RAWMATERIAL");
        //alert(physicalTestRawMaterial);
        //window.sessionStorage.setItem("WEIGHT", rows.WEIGHT);
        //window.sessionStorage.setItem("MOISTURE", rows.MOISTURE);
        //window.sessionStorage.setItem("TEMPERATURE", rows.TEMPERATURE);
        //window.sessionStorage.setItem("WET", rows.WET);
        //window.sessionStorage.setItem("FUNGUS", rows.FUNGUS);
        //window.sessionStorage.setItem("DUST", rows.DUST);
        //window.sessionStorage.setItem("GRADING", rows.GRADING);
        //window.sessionStorage.setItem("SMELL", rows.SMELL);
        //window.sessionStorage.setItem("COLOR", rows.COLOR);
        //window.sessionStorage.setItem("PIECES", rows.PIECES);
        //window.sessionStorage.setItem("IMMATURITY_OF_GRAINS", rows.IMMATURITY_OF_GRAINS);
        //window.sessionStorage.setItem("OTHER_ITEMS", rows.OTHER_ITEMS);
        //window.sessionStorage.setItem("ROTTEN_HOLED", rows.ROTTEN_HOLED);
        //window.sessionStorage.setItem("DAMAGED", rows.DAMAGED);
        //window.sessionStorage.setItem("BROKEN", rows.BROKEN);
        //window.sessionStorage.setItem("HUSK", rows.HUSK);
        //window.sessionStorage.setItem("OVERTOASTED", rows.OVERTOASTED);
        //window.sessionStorage.setItem("USEABLE", rows.USEABLE);
        //window.sessionStorage.setItem("UNUSEABLE", rows.UNUSEABLE);
        //window.sessionStorage.setItem("FAT", rows.FAT);
        //window.sessionStorage.setItem("QUALITY_OF_GOODS", rows.QUALITY_OF_GOODS);
        //window.sessionStorage.setItem("EXCELLENT", rows.EXCELLENT);
        //window.sessionStorage.setItem("GREAT", rows.GREAT);
        //window.sessionStorage.setItem("GOODS_NORMAL", rows.GOODS_NORMAL);
        //window.sessionStorage.setItem("WAREHOUSE", rows.WAREHOUSE);
        //window.sessionStorage.setItem("SILO", rows.SILO);
        //window.sessionStorage.setItem("GHAN", rows.GHAN);
        //window.sessionStorage.setItem("PROTEIN", rows.PROTEIN);
        //window.sessionStorage.setItem("QUALITY_OF_FIREWOOD", rows.QUALITY_OF_FIREWOOD);
        //window.sessionStorage.setItem("PRODUCT_SIZE", rows.PRODUCT_SIZE);
        //window.sessionStorage.setItem("PRODUCT_TYPE", rows.PRODUCT_TYPE);
        //window.sessionStorage.setItem("ISPLASTIC_BAG", rows.ISPLASTIC_BAG);
        //window.sessionStorage.setItem("ISPLASTIC_WEIGHT", rows.ISPLASTIC_WEIGHT);
        //window.sessionStorage.setItem("ISJUTE_BAG", rows.ISJUTE_BAG);
        //window.sessionStorage.setItem("ISJUTE_WEIGHT", rows.ISJUTE_WEIGHT);
        //window.sessionStorage.setItem("NET_WEIGHT", rows.NET_WEIGHT);
        //window.sessionStorage.setItem("REMARKS", rows.REMARKS);
        //var weightDetails = responsesod.data.WEIGHTDETAILSList;
        //var unloadedChhalli = responsesod.data.UNLOADEDCHHALLIList;
        //var dakhilaDetails = responsesod.data.DAKHILADETAILSList;
        //$scope.WeightDetailsData = [];
        //weightDetails.forEach(function (row, index) {
        //    window.sessionStorage.setItem("FIRST_WEIGHT_" + index, row.FIRST_WEIGHT);
        //    window.sessionStorage.setItem("SECOND_WEIGHT_" + index, row.SECOND_WEIGHT);
        //    window.sessionStorage.setItem("NET_WEIGHT_" + index, row.NET_WEIGHT);
        //    window.sessionStorage.setItem("CHALLAN_WEIGHT_" + index, row.CHALLAN_WEIGHT);
        //    window.sessionStorage.setItem("WEIGHT_DIFFERENCE_" + index, row.WEIGHT_DIFFERENCE);
        //    window.sessionStorage.setItem("REMARKS_" + index, row.REMARKS);
        //});
        //window.sessionStorage.setItem("WEIGHT_ROWS_COUNT", weightDetails.length);
        //window.sessionStorage.setItem("weightDetails", $scope.WeightDetailsData);

        //unloadedChhalli.forEach(function (row, index) {
        //    window.sessionStorage.setItem("FIRST_CHHALLI_" + index, row.FIRST_CHHALLI);
        //    window.sessionStorage.setItem("SECOND_CHHALLI_" + index, row.SECOND_CHHALLI);
        //    window.sessionStorage.setItem("THIRD_CHHALLI_" + index, row.THIRD_CHHALLI);
        //    window.sessionStorage.setItem("FOURTH_CHHALLI_" + index, row.FOURTH_CHHALLI);
        //    window.sessionStorage.setItem("FIFTH_CHHALLI_" + index, row.FIFTH_CHHALLI);
        //    window.sessionStorage.setItem("SIXTH_CHHALLI_" + index, row.SIXTH_CHHALLI);
        //    window.sessionStorage.setItem("SEVENTH_CHHALLI_" + index, row.SEVENTH_CHHALLI);
        //    window.sessionStorage.setItem("EIGHTH_CHHALLI_" + index, row.EIGHTH_CHHALLI);
        //    window.sessionStorage.setItem("NINETH_CHHALLI_" + index, row.NINETH_CHHALLI);
        //    window.sessionStorage.setItem("TENTH_CHHALLI_" + index, row.TENTH_CHHALLI);
        //    window.sessionStorage.setItem("ELEVEN_CHHALLI_" + index, row.ELEVEN_CHHALLI);
        //    window.sessionStorage.setItem("TWELVE_CHHALLI_" + index, row.TWELVE_CHHALLI);
        //    window.sessionStorage.setItem("TOTAL_" + index, row.TOTAL);
        //    window.sessionStorage.setItem("REMARKS_" + index, row.REMARKS);
        //});
        //window.sessionStorage.setItem("UNLOADEDCHHALLI_ROWS_COUNT", unloadedChhalli.length);

        //dakhilaDetails.forEach(function (row, index) {
        //    window.sessionStorage.setItem("ENTRY_NO_" + index, row.ENTRY_NO);
        //    window.sessionStorage.setItem("BILL_NO_" + index, row.BILL_NO);
        //    window.sessionStorage.setItem("CHALAN_NO_" + index, row.CHALAN_NO);
        //    window.sessionStorage.setItem("ITEM_" + index, row.ITEM);
        //    window.sessionStorage.setItem("TOTAL_BAG_" + index, row.TOTAL_BAG);
        //    window.sessionStorage.setItem("WEIGHT_" + index, row.WEIGHT);
        //    window.sessionStorage.setItem("REMARKS_" + index, row.REMARKS);
        //});
        //window.sessionStorage.setItem("DAKHILA_ROWS_COUNT", dakhilaDetails.length);
        //$scope.DocumentName == "GLOBAL_PRODUCTS_TRANSACTION"
        ////$("#saveAndPrintGlobalAgroProductsModal").html($scope.TRANSACTION_NO);
        //$("#saveAndPrintGlobalAgroProductsModal").modal("toggle");

    }
    else {
        //alert("i" + $(".hidTRANSACTION_NO").text());
        window.sessionStorage.clear();
        sessionStorage.clear();
        $scope.ActualTransaction_no = $(".hidTRANSACTION_NO").text();
        window.sessionStorage.setItem("TRANSACTION_NO", $(".hidTRANSACTION_NO").text());
        $http.get('/api/GlobalAgroProductsAPI/GetGlobalAgroProductLists')
            .then(function (response) {
                var qcqa = response.data;
                $scope.SLIDER_VALUE = 0;
                $scope.WeightDetailsData.push({ element: qcqa.WEIGHTDETAILSList });
                $scope.UNLOADEDCHHALLIList.push({ element: qcqa.UNLOADEDCHHALLIList });
                $scope.DAKHILADETAILSList.push({ element: qcqa.DAKHILADETAILSList });
                if (qcqa && qcqa.length > 0) {
                    $scope.WeightDetailsData.push({ element: qcqa });
                    $scope.childModelTemplate.push(qcqa);
                }
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    $scope.add_child_element_weightDetails = function (e) {
        $http.get('/api/GlobalAgroProductsAPI/GetGlobalAgroProductLists')
            .then(function (response) {
                var qcqa = response.data;
                $scope.WeightDetailsData.push({ element: qcqa });
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    // remove child row.
    $scope.remove_child_element_weightDetails = function (index) {
        if ($scope.WeightDetailsData.length > 1) {
            $scope.WeightDetailsData.splice(index, 1);
            $scope.childModels.splice(index, 1);
        }
    }

    $scope.add_child_element_unloadedChhalli = function (e) {
        $http.get('/api/GlobalAgroProductsAPI/GetGlobalAgroProductLists')
            .then(function (response) {
                var qcqa = response.data;
                $scope.UNLOADEDCHHALLIList.push({ element: qcqa.UNLOADEDCHHALLIList });
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    // remove child row.
    $scope.remove_child_element_unloadedChhalli = function (index) {
        if ($scope.UNLOADEDCHHALLIList.length > 1) {
            $scope.UNLOADEDCHHALLIList.splice(index, 1);
            $scope.childModels.splice(index, 1);
        }
    }

    $scope.add_child_element_dakhilaDetails = function (e) {
        $http.get('/api/GlobalAgroProductsAPI/GetGlobalAgroProductLists')
            .then(function (response) {
                var qcqa = response.data;
                $scope.DAKHILADETAILSList.push({ element: qcqa.DAKHILADETAILSList });
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    // remove child row.
    $scope.remove_child_element_dakhilaDetails = function (index) {
        if ($scope.DAKHILADETAILSList.length > 1) {
            $scope.DAKHILADETAILSList.splice(index, 1);
            $scope.childModels.splice(index, 1);
        }
    }
    var WeightDetailsList = [];
    var UnloadedChhalliList = [];
    var DakhilaDetailsList = [];
    $scope.saveGlobalAgroProducts = function () {
        var rows = document.querySelectorAll("#idGlobalProducts");
        var rows_chahalli = document.querySelectorAll("#idLoadedChhalli");
        var rows_dakhila = document.querySelectorAll("#idDakhilaDetail");

        //$("#weightDetailsBody tr").each(function (rowIndex, row) {
        rows.forEach(function (row, rowIndex) {
            var weightDetail = {
                FIRST_WEIGHT: $(".FIRST_WEIGHT_" + rowIndex)?.val() || "",
                SECOND_WEIGHT: $(".SECOND_WEIGHT_" + rowIndex)?.val() || "",
                NET_WEIGHT: $(".NET_WEIGHT_" + rowIndex)?.val() || "",
                CHALLAN_WEIGHT: $(".CHALLAN_WEIGHT_" + rowIndex)?.val() || "",
                WEIGHT_DIFFERENCE: $(".WEIGHT_DIFFERENCE_" + rowIndex)?.val() || "",
                REMARKS: row.querySelector(".REMARKS_" + rowIndex)?.value || ""
            };
            WeightDetailsList.push(weightDetail);
        });
        rows_chahalli.forEach(function (row, rowIndex) {
            var unloadedChhalli = {
                FIRST_CHHALLI: $(".FIRST_CHHALLI_" + rowIndex)?.val() || "",
                SECOND_CHHALLI: $(".SECOND_CHHALLI_" + rowIndex)?.val() || "",
                THIRD_CHHALLI: row.querySelector(".THIRD_CHHALLI_" + rowIndex)?.value || "",
                FOURTH_CHHALLI: row.querySelector(".FOURTH_CHHALLI_" + rowIndex)?.value || "",
                FIFTH_CHHALLI: row.querySelector(".FIFTH_CHHALLI_" + rowIndex)?.value || "",
                SIXTH_CHHALLI: row.querySelector(".SIXTH_CHHALLI_" + rowIndex)?.value || "",
                SEVENTH_CHHALLI: row.querySelector(".SEVENTH_CHHALLI_" + rowIndex)?.value || "",
                EIGHTH_CHHALLI: row.querySelector(".EIGHTH_CHHALLI_" + rowIndex)?.value || "",
                NINETH_CHHALLI: row.querySelector(".NINETH_CHHALLI_" + rowIndex)?.value || "",
                TENTH_CHHALLI: row.querySelector(".TENTH_CHHALLI_" + rowIndex)?.value || "",
                ELEVEN_CHHALLI: row.querySelector(".ELEVEN_CHHALLI_" + rowIndex)?.value || "",
                TWELVE_CHHALLI: row.querySelector(".TWELVE_CHHALLI_" + rowIndex)?.value || "",
                TOTAL: row.querySelector(".TOTAL_" + rowIndex)?.value || ""
            };
            UnloadedChhalliList.push(unloadedChhalli);
        });
        rows_dakhila.forEach(function (row, rowIndex) {
            var dakhilaDetails = {
                ENTRY_NO: $(".ENTRY_NO_" + rowIndex)?.val() || "",
                BILL_NO: $(".BILL_NO_" + rowIndex)?.val() || "",
                CHALAN_NO: row.querySelector(".CHALAN_NO_" + rowIndex)?.value || "",
                ITEM: row.querySelector(".ITEM_" + rowIndex)?.value || "",
                TOTAL_BAG: row.querySelector(".TOTAL_BAG_" + rowIndex)?.value || "",
                WEIGHT: row.querySelector(".WEIGHT_" + rowIndex)?.value || "",
                REMARKS: row.querySelector(".REMARKS_" + rowIndex)?.value || ""
            };
            DakhilaDetailsList.push(dakhilaDetails);
        });
        var wrapper = {
            TRANSACTION_NO: $scope.TRANSACTION_NO,
            ITEM_CODE: $('#ddlMaterials').val(),
            CREATED_DATE: $scope.CREATED_DATE,
            VEHICLE_NO: $('#idVehicleNo').val(),
            PARTY_NAME: $('#idPartyName').val(),
            ADDRESS: $('#idAddress').val(),
            BILL_NO: $('#idBillNo').val(),
            GATE_OR_GRN_NO: $('#idGateOrGrnNo').val(),
            PHYSICAL_TEST_RAWMATERIAL: $scope.PHYSICAL_TEST_RAWMATERIAL,
            WEIGHT: $scope.WEIGHT,
            MOISTURE: $scope.MOISTURE,
            TEMPERATURE: $scope.TEMPERATURE,
            WET: $scope.WET,
            FUNGUS: $scope.FUNGUS,
            DUST: $scope.DUST,
            GRADING: $scope.GRADING,
            SMELL: $scope.SMELL,
            COLOR: $scope.COLOR,
            PIECES: $scope.PIECES,
            IMMATURITY_OF_GRAINS: $scope.IMMATURITY_OF_GRAINS,
            OTHER_ITEMS: $scope.OTHER_ITEMS,
            ROTTEN_HOLED: $scope.ROTTEN_HOLED,
            DAMAGED: $scope.DAMAGED,
            BROKEN: $scope.BROKEN,
            HUSK: $scope.HUSK,
            OVERTOASTED: $scope.OVERTOASTED,
            USEABLE: $scope.USEABLE,
            UNUSEABLE: $scope.UNUSEABLE,
            FAT: $scope.FAT,
            QUALITY_OF_GOODS: $scope.QUALITY_OF_GOODS,
            EXCELLENT: $scope.EXCELLENT,
            GREAT: $scope.GREAT,
            GOODS_NORMAL: $scope.GOODS_NORMAL,
            WAREHOUSE: $scope.WAREHOUSE,
            SILO: $scope.SILO,
            GHAN: $scope.GHAN,
            PROTEIN: $scope.PROTEIN,
            QUALITY_OF_FIREWOOD: $scope.QUALITY_OF_FIREWOOD,
            PRODUCT_SIZE: $scope.PRODUCT_SIZE,
            PRODUCT_TYPE: $scope.PRODUCT_TYPE,
            ISPLASTIC_BAG: $scope.ISPLASTIC_BAG,
            ISJUTE_BAG: $scope.ISJUTE_BAG,
            ISPLASTIC_WEIGHT: $scope.ISPLASTIC_WEIGHT,
            ISJUTE_WEIGHT: $scope.ISJUTE_WEIGHT,
            DEDUCT_IN_BAG: $scope.DEDUCT_IN_BAG,
            DEDUCT_IN_WT: $scope.DEDUCT_IN_WT,
            NET_WEIGHT: $scope.NET_WEIGHT,
            ISUNLOAD: $scope.ISUNLOAD,
            ISPROTEIN: $scope.ISPROTEIN,
            ISPT: $scope.ISPT,
            ISOUT: $scope.ISOUT,
            DEDUCT_IN_PLASTIC: $scope.DEDUCT_IN_PLASTIC,
            DEDUCT_IN_JUTE: $scope.DEDUCT_IN_JUTE,
            SLIDER_VALUE: $scope.SLIDER_VALUE,
            PRODUCT_REMARKS: $('#idPRODUCT_REMARKS').val(),
            REMARKS: $('#idREMARKS').val(),
            WEIGHTDETAILSList: WeightDetailsList,
            UNLOADEDCHHALLIList: UnloadedChhalliList,
            DAKHILADETAILSList: DakhilaDetailsList  // lowercase 'list'
        };
        $http.post('/api/GlobalAgroProductsAPI/saveGlobalAgroProducts', wrapper)
            .then(function (response) {
                angular.element('#showModel').modal('show');
                
                //var message = response.data.message; // Extract message from response
                
                //displayPopupNotification(message, "success");
                //setTimeout(function () {
                //    window.location.reload();
                //}, 5000)
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    $('.clsOK').click(function () {
        angular.element('#printShowModel').modal('show');

    });
    //$('.clsYes').click(function () {
    $(document).on("click", ".clsYes", function (e) {
        var fdetails = "/api/TemplateApi/GetFormDetailSetup?formCode=" + $scope.formCode;
        $http.get(fdetails).then(function (responsefdetails) {

            $scope.formDetails = responsefdetails.data;
            if ($scope.formDetails.length > 0) {
                $scope.DocumentName = $scope.formDetails[0].TABLE_NAME;
                $scope.subDocumentName = $scope.formDetails[0].FORM_EDESC;
                $('#printCompanyInfo').text($scope.formDetails[0].COMPANY_EDESC);
                window.sessionStorage.setItem("printCompanyInfo", $scope.formDetails[0].COMPANY_EDESC);
                window.sessionStorage.setItem("FORM_EDESC", $scope.formDetails[0].FORM_EDESC);
                $('#companyaddress').text($scope.formDetails[0].ADDRESS);
                window.sessionStorage.setItem("COMPANY_ADDRESS", $scope.formDetails[0].ADDRESS);
                //$('#companytelephone').text("Ph.No. :" + $scope.formDetails[0].TELEPHONE);
                //$('#companytelephone').text(
                //    "Ph.No. : " + ($scope.formDetails[0].TELEPHONE ? $scope.formDetails[0].TELEPHONE : "")
                //);
                //$('#companypanNo').text(
                //    "Pan No. : " + ($scope.formDetails[0].TPIN_VAT_NO ? $scope.formDetails[0].TPIN_VAT_NO : "")
                //);
                $('#companytelephone').text(
                     ($scope.formDetails[0].TELEPHONE ? $scope.formDetails[0].TELEPHONE : "")
                );
                $('#companypanNo').text(
                     ($scope.formDetails[0].TPIN_VAT_NO ? $scope.formDetails[0].TPIN_VAT_NO : "")
                );
                window.sessionStorage.setItem("COMPANY_TELEPHONE", $scope.formDetails[0].TELEPHONE);
            }
        });
        //var reqst = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetGlobalAgroProductsReport?transactionno=" + $scope.TRANSACTION_NO;
        var reqst = window.location.protocol + "//" + window.location.host + "/api/GlobalAgroProductsAPI/GetGlobalAgroProductsReport?transactionno=" + $scope.ActualTransaction_no ;
        //var reqst = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetGlobalAgroProductsReport?transactionno=" + $scope.ActualTransaction_no;
        /////*var reqst = window.location.protocol + "//" + window.location.host + "/Home/GetGlobalAgroProductsReport?transactionno=" + $scope.TRANSACTION_NO;*/
        $http.get(reqst, { cache: false }).then(function (responsesod) {
            var rows = responsesod.data;
            var weightDetails = responsesod.data.WEIGHTDETAILSList;
            var unloadedChhalli = responsesod.data.UNLOADEDCHHALLIList;
            var dakhilaDetails = responsesod.data.DAKHILADETAILSList;
            var tbody_weightDetailsBody = $("#weightDetailsBody");
            tbody_weightDetailsBody.empty();
            WeightDetailsList = [];
            UnloadedChhalliList = [];
            DakhilaDetailsList = [];
            weightDetails.forEach(function (rows, index) {
    var tr = `
        <tr>
            <td id="SNO_${index}" style="border:1px solid #000;">${index + 1}</td>
            <td id="FIRST_WEIGHT_${index}" style="border:1px solid #000;">${rows.FIRST_WEIGHT ?  rows.FIRST_WEIGHT : ""}</td>
            <td id="SECOND_WEIGHT_${index}" style="border:1px solid #000;">${rows.SECOND_WEIGHT ? rows.SECOND_WEIGHT : ""}</td>
            <td id="NET_WEIGHT_${index}" style="border:1px solid #000;">${rows.NET_WEIGHT ? rows.NET_WEIGHT : ""}</td>
            <td id="CHALLAN_WEIGHT_${index}" style="border:1px solid #000;">${rows.CHALLAN_WEIGHT ? rows.CHALLAN_WEIGHT : ""}</td>
            <td id="WEIGHT_DIFFERENCE_${index}" style="border:1px solid #000;">${rows.WEIGHT_DIFFERENCE ? rows.WEIGHT_DIFFERENCE : ""}</td>
            <td id="REMARKS_${index}" style="border:1px solid #000;">${rows.REMARKS ? rows.REMARKS : ""}</td>
        </tr>
    `;
                tbody_weightDetailsBody.append(tr);
            });

            var tbody_unloadedChhalliBody = $("#unloadedChhalliBody");
            tbody_unloadedChhalliBody.empty();
            unloadedChhalli.forEach(function (row, index) {
                var tr = `
            <tr>
                <td id="SNO_${index}" style="border:1px solid #000;">${index + 1}</td>
                <td id="FIRST_CHHALLI_${index}" style="border:1px solid #000;">${row.FIRST_CHHALLI ? row.FIRST_CHHALLI : ""}</td>
                <td id="SECOND_CHHALLI_${index}" style="border:1px solid #000;">${row.SECOND_CHHALLI ? row.SECOND_CHHALLI : ""}</td>
                <td id="THIRD_CHHALLI_${index}" style="border:1px solid #000;">${row.THIRD_CHHALLI ? row.THIRD_CHHALLI : ""}</td>
                <td id="FOURTH_CHHALLI_${index}" style="border:1px solid #000;">${row.FOURTH_CHHALLI ? row.FOURTH_CHHALLI : ""}</td>
                <td id="FIFTH_CHHALLI_${index}" style="border:1px solid #000;">${row.FIFTH_CHHALLI ? row.FIFTH_CHHALLI : ""}</td>
                <td id="SIXTH_CHHALLI_${index}" style="border:1px solid #000;">${row.SIXTH_CHHALLI ? row.SIXTH_CHHALLI : ""}</td>
                <td id="SEVENTH_CHHALLI_${index}" style="border:1px solid #000;">${row.SEVENTH_CHHALLI ? row.SEVENTH_CHHALLI : ""}</td>
                <td id="EIGHTH_CHHALLI_${index}" style="border:1px solid #000;">${row.EIGHTH_CHHALLI ? row.EIGHTH_CHHALLI : ""}</td>
                <td id="NINETH_CHHALLI_${index}" style="border:1px solid #000;">${row.NINETH_CHHALLI ? row.NINETH_CHHALLI : ""}</td>
                <td id="TENTH_CHHALLI_${index}" style="border:1px solid #000;">${row.TENTH_CHHALLI ? row.TENTH_CHHALLI : ""}</td>
                <td id="ELEVEN_CHHALLI_${index}" style="border:1px solid #000;">${row.ELEVEN_CHHALLI ? row.ELEVEN_CHHALLI : ""}</td>
                <td id="TWELVE_CHHALLI_${index}" style="border:1px solid #000;">${row.TWELVE_CHHALLI ? row.TWELVE_CHHALLI : ""}</td>
                <td id="TOTAL_${index}" style="border:1px solid #000;">${row.TOTAL ? row.TOTAL : ""}</td>
            </tr>
        `;
                tbody_unloadedChhalliBody.append(tr);
            });
            var tbody_dakhilaDetailsBody = $("#dakhilaDetailsBody");
            tbody_dakhilaDetailsBody.empty();

            dakhilaDetails.forEach(function (row, index) {
                var tr = `
            <tr>
                <td id="SNO_${index}" style="border:1px solid #000;">${index + 1}</td>
                <td id="ENTRY_NO_${index}" style="border:1px solid #000;">${row.ENTRY_NO ? row.ENTRY_NO : ""}</td>
                <td id="BILL_NO_${index}" style="border:1px solid #000;">${row.BILL_NO ? row.BILL_NO : ""}</td>
                <td id="CHALAN_NO_${index}" style="border:1px solid #000;">${row.CHALAN_NO ? row.CHALAN_NO : ""}</td>
                <td id="ITEM_${index}" style="border:1px solid #000;">${row.ITEM ? row.ITEM : ""}</td>
                <td id="TOTAL_BAG_${index}" style="border:1px solid #000;">${row.TOTAL_BAG ? row.TOTAL_BAG : ""}</td>
                <td id="WEIGHT_${index}" style="border:1px solid #000;">${row.WEIGHT ? row.WEIGHT : ""}</td>
                <td id="REMARKS_${index}" style="border:1px solid #000;">${row.REMARKS ? row.REMARKS : ""}</td>
            </tr>
        `;
                tbody_dakhilaDetailsBody.append(tr);
            });
            $scope.DocumentName == "GLOBAL_PRODUCTS_TRANSACTION"
            $('#transactionNo').text(($scope.ActualTransaction_no.replace(/^'|'$/g, "") ? ": " + $scope.ActualTransaction_no.replace(/^'|'$/g, "") : ""));
            $('#createdDate').text((rows.CREATED_DATE_STR ? ": " + rows.CREATED_DATE_STR : ""));
            $('#partyName').text((rows.PARTY_NAME ? ": " + rows.PARTY_NAME : ""));
            $('#vehicleNo').text((rows.VEHICLE_NO ? ": " + rows.VEHICLE_NO : ""));
            $('#product_item_edesc').text((rows.ITEM_EDESC ? ": " + rows.ITEM_EDESC : ""));
            $('#address').text((rows.ADDRESS ? ": " + rows.ADDRESS : ""));
            $('#gateOrgrNo').text((rows.GATE_OR_GRN_NO ? ": " + rows.GATE_OR_GRN_NO : ""));
            $('#billNo').text((rows.BILL_NO ? ": " + rows.BILL_NO : ""));
            $('#physicalTestRawMaterial').text((rows.PHYSICAL_TEST_RAWMATERIAL ? rows.PHYSICAL_TEST_RAWMATERIAL : ""));
            $('#weight').text((rows.WEIGHT ? rows.WEIGHT : ""));
            $('#moisture').text((rows.MOISTURE ? rows.MOISTURE : ""));
            $('#wet').text((rows.WET ? rows.WET : ""));
            $('#temperature').text((rows.TEMPERATURE ? rows.TEMPERATURE : ""));
            $('#dust').text((rows.DUST ? rows.DUST : ""));
            $('#smell').text((rows.SMELL ? rows.SMELL : ""));
            $('#fungus').text((rows.FUNGUS ? rows.FUNGUS : ""));
            $('#pieces').text((rows.PIECES ? rows.PIECES : ""));
            $('#otheritems').text((rows.OTHER_ITEMS ? rows.OTHER_ITEMS : ""));
            $('#grading').text((rows.GRADING ? rows.GRADING : ""));
            $('#damaged').text((rows.DAMAGED ? rows.DAMAGED : ""));
            $('#husk').text((rows.HUSK ? rows.HUSK : ""));
            $('#color').text((rows.COLOR ? rows.COLOR : ""));
            $('#useable').text((rows.USEABLE ? rows.USEABLE : ""));
            $('#fat').text((rows.FAT ? rows.FAT : ""));
            $('#imaturityofgrains').text((rows.IMMATURITY_OF_GRAINS ? rows.IMMATURITY_OF_GRAINS : ""));
            $('#excellent').text((rows.EXCELLENT ? rows.EXCELLENT : ""));
            $('#goodsnormal').text((rows.GOODS_NORMAL ? rows.GOODS_NORMAL : ""));
            $('#rottenholed').text((rows.ROTTEN_HOLED ? rows.ROTTEN_HOLED : ""));
            $('#silo').text((rows.SILO ? rows.SILO : ""));
            $('#productsize').text((rows.PRODUCT_SIZE ? rows.PRODUCT_SIZE : ""));
            $('#broken').text((rows.BROKEN ? rows.BROKEN : ""));
            $('#overtoasted').text((rows.OVERTOASTED ? rows.OVERTOASTED : ""));
            $('#unuseable').text((rows.UNUSEABLE ? rows.UNUSEABLE : ""));
            $('#qualityofgoods').text((rows.QUALITY_OF_GOODS ? rows.QUALITY_OF_GOODS : ""));
            $('#great').text((rows.GREAT ? rows.GREAT : ""));
            $('#warehouse').text((rows.WAREHOUSE ? rows.WAREHOUSE : ""));
            $('#ghan').text((rows.GHAN ? rows.GHAN : ""));
            $('#qualityoffirewood').text((rows.QUALITY_OF_FIREWOOD ? rows.QUALITY_OF_FIREWOOD : ""));
            $('#producttype').text((rows.PRODUCT_TYPE ? rows.PRODUCT_TYPE : ""));
            $('#protein').text((rows.PROTEIN ? rows.PROTEIN : ""));
            var text_bag = (rows.ISPLASTIC_BAG === 'Y' ? "Plastic" : "")
                + (rows.ISJUTE_BAG === 'Y' ? " " + "Jute" : "");
            $("#BAG_TYPE").text(text_bag.trim());
            var text_weight = (rows.ISPLASTIC_WEIGHT === 'Y' ? "Plastic" : "")
                + (rows.ISJUTE_WEIGHT === 'Y' ? " " + "Jute" : "");
            $("#WEIGHT_TYPE").text(text_weight.trim());
            $("#netweight").text(rows.NET_WEIGHT);
            $("#DEDUCT_IN_PLASTIC").text(rows.DEDUCT_IN_PLASTIC);
            $("#DEDUCT_IN_JUTE").text(rows.DEDUCT_IN_JUTE);
            var text_unload = (rows.ISUNLOAD === 'Y' ? "Yes" : "No");
            $("#ISUNLOAD").text(text_unload.trim());
            var text_protein = (rows.ISPROTEIN === 'Y' ? "Yes" : "No");
            $("#ISPROTEIN").text(text_protein.trim());
            var text_ispt = (rows.ISPT === 'Y' ? "Yes" : "No");
            $("#ISPT").text(text_ispt.trim());
            var text_isout = (rows.ISOUT === 'Y' ? "Yes" : "No");
            $("#ISOUT").text(text_isout.trim());
            $('#DEDUCT_IN_BAG').text((rows.DEDUCT_IN_BAG ? rows.DEDUCT_IN_BAG : ""));
            $('#DEDUCT_IN_WT').text((rows.DEDUCT_IN_WT ? rows.DEDUCT_IN_WT : ""));
            $('#remarks').text((rows.REMARKS ? rows.REMARKS : ""));
            $('span#prodRemarks').text((rows.PRODUCT_REMARKS ? rows.PRODUCT_REMARKS : ""));
            $("#saveAndPrintGlobalAgroProductsModal").modal("toggle");
        });
    });
    $scope.updateTotal = function (key) {
        let first = parseFloat($scope.childUnloadedChalliModels[key]['FIRST_CHHALLI']) || 0;
        let second = parseFloat($scope.childUnloadedChalliModels[key]['SECOND_CHHALLI']) || 0;
        let third = parseFloat($scope.childUnloadedChalliModels[key]['THIRD_CHHALLI']) || 0;
        let fourth = parseFloat($scope.childUnloadedChalliModels[key]['FOURTH_CHHALLI']) || 0;
        let fifth = parseFloat($scope.childUnloadedChalliModels[key]['FIFTH_CHHALLI']) || 0;
        let sixth = parseFloat($scope.childUnloadedChalliModels[key]['SIXTH_CHHALLI']) || 0;
        let seventh = parseFloat($scope.childUnloadedChalliModels[key]['SEVENTH_CHHALLI']) || 0;
        let eighth = parseFloat($scope.childUnloadedChalliModels[key]['EIGHTH_CHHALLI']) || 0;
        let nineth = parseFloat($scope.childUnloadedChalliModels[key]['NINETH_CHHALLI']) || 0;
        let tenth = parseFloat($scope.childUnloadedChalliModels[key]['TENTH_CHHALLI']) || 0;
        let eleventh = parseFloat($scope.childUnloadedChalliModels[key]['ELEVEN_CHHALLI']) || 0;
        let twelveth = parseFloat($scope.childUnloadedChalliModels[key]['TWELVE_CHHALLI']) || 0;
        $scope.childUnloadedChalliModels[key]['TOTAL'] = first + second + third + fourth + fifth + sixth + seventh + eighth + nineth + tenth + eleventh + twelveth;
    };

    $scope.updateNetTotal = function (key) {
        let sum = 0;
        $(".table_body").find('tr#idDakhilaDetail').each(function () {
            var wt = $(this).find('#id_WEIGHT').val();
            let val = parseFloat(wt) || 0;
            sum += val;
        });
        let weight = parseFloat(sum) || 0;
        let deductInPlastic = parseFloat($scope.DEDUCT_IN_PLASTIC) || 0;
        let deductInJute = parseFloat($scope.DEDUCT_IN_JUTE) || 0;
        let deductInWt = parseFloat($scope.DEDUCT_IN_WT) || 0;
        $scope.NET_WEIGHT = weight - deductInPlastic - deductInJute - deductInWt;
    };

    $scope.updateWeightNetTotal = function (key) {
        let firstWeight = parseFloat($scope.childWeightDetailsModels[key]['FIRST_WEIGHT']) || 0;
        let secondWeight = parseFloat($scope.childWeightDetailsModels[key]['SECOND_WEIGHT']) || 0;
        $scope.childWeightDetailsModels[key]['NET_WEIGHT'] = firstWeight - secondWeight;
    };

    $scope.updateWeightDifference = function (key) {
        let netWeight = parseFloat($scope.childWeightDetailsModels[key]['NET_WEIGHT']) || 0;
        let challanWeight = parseFloat($scope.childWeightDetailsModels[key]['CHALLAN_WEIGHT']) || 0;
        $scope.childWeightDetailsModels[key]['WEIGHT_DIFFERENCE'] = netWeight - challanWeight;
    };
    $('.clsIncomingPrint').click(function () {
        var printContents = document.getElementById('saveAndPrintGlobalAgroProductsModalFor').innerHTML;
        var popupWin = window.open('', '_blank', 'width=800,height=1122');
        popupWin.document.open();
        popupWin.document.write(`
  <html>
    <head>
      <style>
        @media print {
            body {
                margin: 0;
                padding: 0;
                font-family: Arial, sans-serif;
                font-size: 9px;
            }

            @page {
                size: A4;
                margin: 10mm;
            }

            /* Ensure outer table has no borders */
            .printable-modal>table,
            .printable-modal>table th,
            .printable-modal>table td {
                border: none !important;
                border-collapse: collapse !important;
            }

            /* Reinforce borders for inner tables */
            table.print_table,
            table.noPageBreakTable,
            table.print_table_details {
                border: 1px solid #000 !important;
                border-collapse: collapse !important;
                page-break-inside: avoid;
            }

            table.print_table th,
            table.print_table td,
            table.noPageBreakTable th,
            table.noPageBreakTable td,
            table.print_table_details th,
            table.print_table_details td {
                border: 1px solid #000 !important;
            }

            .print_table.consignee,
            .print_table.consignee td {
                border: 1px solid #000 !important;
            }

            .compLogo img {
                max-height: 70px !important;
            }

            /* Ensure stamp box has border */
            div[style*="border: 1px solid black; margin-left: 445px; height: 85px"] {
                border: 1px solid #000 !important;
            }

            /* Typography for print */
            table.print_table .grid-tr td,
            table.print_table .grid-tr th {
                font-family: Arial, sans-serif;
                font-size: 10px;
                font-weight: normal;
                line-height: 1.2;
                color: #000;
            }

            .pon_table {
                border: none !important;
            }

            .pon_table tbody tr td {
                border: none !important;
                font-size: 10px !important;
            }

            .pon_table tbody tr td:empty {
                border: none !important;
                background-color: transparent;
            }
        }
      </style>
    </head>
    <body onload="window.print()">
      ${printContents}
    </body>
  </html>
`);


        //AA
        $scope.CheckPrintCount = function () {
            //var vouch_no = $scope.dzvouchernumber.replace(/_/g, '/');
            var vouch_no = orderNo;
            var getprintcounturl = "/api/TemplateApi/GetPrintCountByVoucherNo?voucherno=" + vouch_no + "&formcode=" + formcode;
            $http.get(getprintcounturl).then(function (responseee) {


                if ($scope.DocumentName === "SA_SALES_RETURN") {
                    $scope.taxinvoice = "Credit Note";
                }
                else {
                    if (responseee.data > 0) {
                        $scope.printcounttext = "Copy of Original" + "(" + (responseee.data) + ")";
                        $scope.taxinvoice = "INVOICE";
                        $scope.taxinvoice1 = "";
                    }
                    else {
                        $scope.taxinvoice = "TAX INVOICE";
                        $scope.printcounttext = "";
                        $scope.taxinvoice1 = "";
                    }
                }
                d2.resolve(responseee);
            });

        };
        $scope.getNepaliDate = function (date) {
            return AD2BS(moment(date).format('YYYY-MM-DD'));
        };
        $scope.cnlPrint = function () {

        };
    });
});