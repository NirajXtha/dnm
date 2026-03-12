QCQAModule.controller('QCQADocumentFinderList', function ($scope, $rootScope, $http, $filter, $timeout, $location) {
    //var formCode = $location.search().formcode;
    var formCode = ($location.search().formcode || '').trim().toUpperCase();
    $scope.voucherno = "";
    $scope.printCompanyInfo = {
        companyName: '',
        address: '',
        formName: '',
        phoneNo: '',
        email: '',
        tPinVatNo: '',
    }

    var fdetails = "/api/QCQAAPI/GetQCFormDetailSetup?formCode=" + formCode;
    $http.get(fdetails).then(function (responsefdetails) {
        $scope.formDetails = responsefdetails.data;
        if ($scope.formDetails.length > 0) {
            $scope.DocumentName = $scope.formDetails[0].TABLE_NAME;
            $scope.subDocumentName = $scope.formDetails[0].FORM_EDESC;
            $scope.companycode = $scope.formDetails[0].COMPANY_CODE;
            $scope.NEGETIVE_STOCK_FLAG = $scope.formDetails[0].NEGATIVE_STOCK_FLAG;
            $scope.printCompanyInfo.companyName = $scope.formDetails[0].COMPANY_EDESC;
            $scope.printCompanyInfo.address = $scope.formDetails[0].ADDRESS;
            $scope.printCompanyInfo.formName = $scope.formDetails[0].FORM_EDESC;
            $scope.printCompanyInfo.phoneNo = $scope.formDetails[0].TELEPHONE;
            $scope.printCompanyInfo.email = $scope.formDetails[0].EMAIL;
            $scope.printCompanyInfo.tPinVatNo = $scope.formDetails[0].TPIN_VAT_NO;
        }
    });
    $http.get("/api/QCQADocumentFinderAPI/GetQCQADetailByFormCode?formCode=" + formCode + "&docVer=" + "")
        .then(function (response) {
            var qcqa = response.data;
            if (qcqa && qcqa.length > 0) {
                $scope.voucherno = qcqa[0].VOUCHER_NO;
                console.log("voucher34", $scope.voucherno);
                $scope.dataSource.data(qcqa);
            }
        })
        .catch(function (error) {
            displayPopupNotification(error, "error");
        })
    console.log("voucher", $scope.VOUCHER_NO);
    $scope.dataSource = new kendo.data.DataSource({
        data: [], // Initially empty
    });
    //var qcTitle = "QC No";

    //// Get first record safely
    //var data = $scope.dataSource;
    ////console.log("voucher123", data);
    
    //if (data.length > 0) {

    //    var voucherNo = data[0].VOUCHER_NO || "";
        
    //    if (voucherNo.includes('RM')) qcTitle = "QC No (RawMaterial)";
    //    else if (voucherNo.includes('DW')) qcTitle = "QC No (DailyWastage)";
    //    else if (voucherNo.includes('PD')) qcTitle = "QC No (PreDispatchInspection)";
    //    else if (voucherNo.includes('OI')) qcTitle = "QC No (OnSiteInspection)";
    //    else if (voucherNo.includes('II')) qcTitle = "QC No (InternalInspection)";
    //    else if (voucherNo.includes('SH')) qcTitle = "QC No (SanitationHygiene)";
    //    else if (voucherNo.includes('HO')) qcTitle = "QC No (HandOverInspection)";
    //    else if (voucherNo.includes('FI')) qcTitle = "QC No (FinishedGoodsInspection)";
    //    else if (voucherNo.includes('IS')) qcTitle = "QC No (IncomingMaterialDirect)";
    //}
    //console.log("hello", $location.search());
    $("#QCQADocumentFinderGrid").kendoGrid({
        dataSource: $scope.dataSource,
        height: 400,
        sortable: true, // Enable sorting
        pageable: {
            refresh: true,
            pageSizes: true
        },
        resizable: true, // Enable column resizing
        dataBound: function () {

            var grid = this;
            var data = grid.dataSource.view();   // current page data

            if (data.length > 0) {

                var voucherNo = data[0].VOUCHER_NO || "";
                var qcTitle = "DocumentName";

                if (voucherNo.includes('RM')) qcTitle = "QC No (RawMaterial)";
                else if (voucherNo.includes('IN')) qcTitle = "QC No (Incoming Material)";
                else if (voucherNo.includes('DW')) qcTitle = "QC No (Daily Wastage)";
                else if (voucherNo.includes('PD')) qcTitle = "QC No (Pre Dispatch Inspection)";
                else if (voucherNo.includes('OI')) qcTitle = "QC No (On Site Inspection)";
                else if (voucherNo.includes('II')) qcTitle = "QC No (Internal Inspection)";
                else if (voucherNo.includes('SH')) qcTitle = "QC No (Sanitation Hygiene)";
                else if (voucherNo.includes('HO')) qcTitle = "QC No (HandOver Inspection)";
                else if (voucherNo.includes('FI')) qcTitle = "QC No (Finished Goods Inspection)";
                else if (voucherNo.includes('IS')) qcTitle = "QC No (Incoming Material(Sample))";

                // Change header text
                grid.thead.find("th[data-field='VOUCHER_NO']").text(qcTitle);
            }
        },
        columns: [
            { field: "VOUCHER_NO", title: "DocumentName", width: 100 },
            { field: "VOUCHER_DATE", title: "Date", width: 200, type: "string" },
            {
                title: "Actions",
                width: 120,
                template: function (dataItem) {
                    var pageName = "IncomingMaterial";
                    var methodName = "";

                    if (dataItem.VOUCHER_NO && dataItem.VOUCHER_NO.includes('RM')) { pageName = "RawMaterial"; methodName = "GetDailyRawMaterialReport"; }
                    else if (dataItem.VOUCHER_NO && dataItem.VOUCHER_NO.includes('DW')) { pageName = "DailyWastage"; methodName = "GetDailyWastageReport"; }
                    else if (dataItem.VOUCHER_NO && dataItem.VOUCHER_NO.includes('PD')) { pageName = "PreDispatchInspection"; methodName = "GetDailyRawMaterialReport"; }
                    else if (dataItem.VOUCHER_NO && dataItem.VOUCHER_NO.includes('OI')) { pageName = "OnSiteInspection"; methodName = "GetOnSiteInspectionReport"; }
                    else if (dataItem.VOUCHER_NO && dataItem.VOUCHER_NO.includes('II')) { pageName = "InternalInspection"; methodName = "GetInternalInspectionReport"; }
                    else if (dataItem.VOUCHER_NO && dataItem.VOUCHER_NO.includes('SH')) { pageName = "SanitationHygiene"; methodName = "GetSanitationHygieneReport"; }
                    else if (dataItem.VOUCHER_NO && dataItem.VOUCHER_NO.includes('HO')) { pageName = "HandOverInspection"; methodName = "GetHandOverInspectionReport"; }
                    else if (dataItem.VOUCHER_NO && dataItem.VOUCHER_NO.includes('FI')) { pageName = "FinishedGoodsInspection"; methodName = "GetFinishedGoodsInspectionReport"; }
                    else if (dataItem.VOUCHER_NO && dataItem.VOUCHER_NO.includes('IS')) { pageName = "IncomingMaterialDirect"; methodName = "GetIncomingMaterialSampleDetailReport"; }
                    else { pageName = "IncomingMaterial"; }

                    var editBtn =
                        "<a class='btn btn-xs btn-warning' " +
                        "href='/QCQAManagement/Home/Index#!QCQA/" + pageName + "?voucherno=" +
                        dataItem.VOUCHER_NO +
                        "' title='Edit'>" +
                        "<i class='fa fa-edit'></i>" +
                        "</a>";

                    var deleteBtn =
                        "<a href='javascript:void(0);' " +
                        "class='btn btn-xs btn-danger delete-btn' " +
                        "data-id='" + dataItem.VOUCHER_NO + "' " +
                        "title='Delete'>" +
                        "<i class='fa fa-trash'></i>" +
                        "</a>";

                    var printBtn =
                        "<a href='javascript:void(0);' " +
                        "class='btn btn-xs btn-default print-btn' " +
                        "data-url='/API/" + pageName + "/" + methodName + "/?voucherno=" + dataItem.VOUCHER_NO + "' " +
                        "data-print-type='" + pageName + "' " + // e.g., "DailyWastage" or "RawMaterial"
                        "data-voucherno='" + dataItem.VOUCHER_NO + "' " +
                        "title='Print'>" +
                        "<i class='glyphicon glyphicon-print'></i>" +
                        "</a>";
                    return editBtn + "&nbsp;" + deleteBtn + "&nbsp;" + printBtn;
                }
            }
        ]
    });

    $(document).on("click", ".print-btn", function () {

        var printUrl = $(this).data("url");
        var printType = $(this).data("print-type");

        var voucherno = $(this).data("voucherno");    // e.g., RM123
        var url = $(this).data("url");
        if (printType && printType === 'IncomingMaterial') {
            var reqst = window.location.protocol + "//" + window.location.host + "/api/IncomingMaterialApi/GetIncomingMaterialsDetailReport?transactionno=" + voucherno;

            $http.get(reqst).then(function (responsesod) {
                var rows = responsesod.data;
                $scope.TRANSACTION_NO = rows.TRANSACTION_NO;
                $scope.QC_DATE = rows.QC_DATE;
                $scope.GRN_DATE = rows.GRN_DATE;
                $scope.supplier_edesc = rows.supplier_edesc;
                $scope.to_location_code = rows.to_location_code;
                $scope.invoice_no = rows.invoice_no;
                $scope.GRN_NO = rows.GRN_NO;
                $scope.RECEIPT_DATE = rows.RECEIPT_DATE;
                $scope.quantity = rows.quantity;
                $scope.item_edesc = rows.item_edesc;
                $scope.MANUAL_NO = rows.MANUAL_NO;
                $scope.REMARKS = rows.REMARKS;
                $scope.Thickness = rows.thickness;
                $scope.item_code = rows.item_code;
                $scope.Thickness1 = rows.Thickness1;
                $scope.Thickness2 = rows.Thickness2;
                $scope.Thickness3 = rows.Thickness3
                $scope.RollDiameter0 = rows.RollDiameter0;
                $scope.RollDiameter = rows.RollDiameter;
                $scope.RollDiameter1 = rows.RollDiameter1;
                $scope.RollDiameter2 = rows.RollDiameter2;
                $scope.RollDiameter3 = rows.RollDiameter3;
                $scope.PH = rows.PH;
                $scope.PH1 = rows.PH1;
                $scope.PH2 = rows.PH2;
                $scope.PH3 = rows.PH3;
                $scope.UnpleasantSmell1 = rows.UnpleasantSmell1;
                $scope.UnpleasantSmell2 = rows.UnpleasantSmell2;
                $scope.UnpleasantSmell3 = rows.UnpleasantSmell3;
                $scope.DustDirt1 = rows.DustDirt1;
                $scope.DustDirt2 = rows.DustDirt2;
                $scope.DustDirt3 = rows.DustDirt3;
                $scope.DamagingMaterial1 = rows.DamagingMaterial1;
                $scope.DamagingMaterial2 = rows.DamagingMaterial2;
                $scope.DamagingMaterial3 = rows.DamagingMaterial3;
                $scope.CoreDamaging1 = rows.CoreDamaging1;
                $scope.CoreDamaging2 = rows.CoreDamaging2;
                $scope.CoreDamaging3 = rows.CoreDamaging3;
                $scope.SIZE_WIDTH = rows.SIZE_WIDTH;
                $scope.Width1 = rows.Width1;
                $scope.Width2 = rows.Width2;
                $scope.Width3 = rows.Width3;
                $scope.GSM = rows.GSM;
                $scope.GSM1 = rows.GSM1;
                $scope.GSM2 = rows.GSM2;
                $scope.GSM3 = rows.GSM3;
                $scope.Tensile_CD = rows.Tensile_CD;
                $scope.TensileCD1 = rows.TensileCD1;
                $scope.TensileCD2 = rows.TensileCD2;
                $scope.TensileCD3 = rows.TensileCD3;
                $scope.Tensile_MD = rows.Tensile_MD;
                $scope.TensileMD1 = rows.TensileMD1;
                $scope.TensileMD2 = rows.TensileMD2;
                $scope.TensileMD3 = rows.TensileMD3;
                $scope.VisualInspection = rows.VisualInspection;
                $scope.VisualInspection1 = rows.VisualInspection1;
                $scope.VisualInspection2 = rows.VisualInspection2;
                $scope.VisualInspection3 = rows.VisualInspection3;
                $scope.Remarks1 = rows.Remarks1;
                $scope.Remarks2 = rows.Remarks2;
                $scope.Remarks3 = rows.Remarks3;
                $scope.Remarks4 = rows.Remarks4;
                $scope.Remarks5 = rows.Remarks5;
                $scope.Remarks6 = rows.Remarks6;
                $scope.Remarks7 = rows.Remarks7;
                $scope.Remarks8 = rows.Remarks8;
                $scope.Remarks9 = rows.Remarks9;
                $scope.Remarks10 = rows.Remarks10;
                $scope.Remarks11 = rows.Remarks11;
                $scope.Remarks12 = rows.Remarks12;

                $("#saveAndPrintQCModal").modal("show");
                //    if ($scope.DocumentName == "IP_ITEM_QC_SETUP") {
                //        $("#saveAndPrintQCModal").modal("toggle");
                //    }
            });
            //$("#saveAndPrintQCModal").data("print-url", printUrl).data("print-type", printType).modal("show");
        }
        else if (printType && printType === 'IncomingMaterialDirect') {

            var reqst = window.location.protocol + "//" + window.location.host + "/api/IncomingMaterialDirectApi/GetIncomingMaterialSampleDetailReport?transactionno=" + voucherno;

            $http.get(reqst).then(function (responsesod) {
                var rows = responsesod.data;
                $scope.TRANSACTION_NO = rows.TRANSACTION_NO;
                $scope.QC_DATE = rows.QC_DATE;
                $scope.supplier_edesc = rows.supplier_edesc;
                $scope.to_location_code = rows.to_location_code;
                $scope.invoice_no = rows.invoice_no;
                $scope.GRN_NO = rows.GRN_NO;
                $scope.RECEIPT_DATE = rows.RECEIPT_DATE;
                $scope.quantity = rows.quantity;
                $scope.item_edesc = rows.item_edesc;
                $scope.MANUAL_NO = rows.MANUAL_NO;
                $scope.REMARKS = rows.REMARKS;
                $scope.Thickness = rows.thickness;
                $scope.item_code = rows.item_code;
                $scope.Thickness1 = rows.Thickness1;
                $scope.Thickness2 = rows.Thickness2;
                $scope.Thickness3 = rows.Thickness3
                $scope.RollDiameter = rows.RollDiameter;
                $scope.RollDiameter1 = rows.RollDiameter1;
                $scope.RollDiameter2 = rows.RollDiameter2;
                $scope.RollDiameter3 = rows.RollDiameter3;
                $scope.PH = rows.PH;
                $scope.PH1 = rows.PH1;
                $scope.PH2 = rows.PH2;
                $scope.PH3 = rows.PH3;
                $scope.UnpleasantSmell1 = rows.UnpleasantSmell1;
                $scope.UnpleasantSmell2 = rows.UnpleasantSmell2;
                $scope.UnpleasantSmell3 = rows.UnpleasantSmell3;
                $scope.DustDirt1 = rows.DustDirt1;
                $scope.DustDirt2 = rows.DustDirt2;
                $scope.DustDirt3 = rows.DustDirt3;
                $scope.DamagingMaterial1 = rows.DamagingMaterial1;
                $scope.DamagingMaterial2 = rows.DamagingMaterial2;
                $scope.DamagingMaterial3 = rows.DamagingMaterial3;
                $scope.CoreDamaging1 = rows.CoreDamaging1;
                $scope.CoreDamaging2 = rows.CoreDamaging2;
                $scope.CoreDamaging3 = rows.CoreDamaging3;
                $scope.SIZE_WIDTH = rows.SIZE_WIDTH;
                $scope.Width1 = rows.Width1;
                $scope.Width2 = rows.Width2;
                $scope.Width3 = rows.Width3;
                $scope.GSM = rows.GSM;
                $scope.GSM1 = rows.GSM1;
                $scope.GSM2 = rows.GSM2;
                $scope.GSM3 = rows.GSM3;
                $scope.Tensile_CD = rows.Tensile_CD;
                $scope.TensileCD1 = rows.TensileCD1;
                $scope.TensileCD2 = rows.TensileCD2;
                $scope.TensileCD3 = rows.TensileCD3;
                $scope.Tensile_MD = rows.Tensile_MD;
                $scope.TensileMD1 = rows.TensileMD1;
                $scope.TensileMD2 = rows.TensileMD2;
                $scope.TensileMD3 = rows.TensileMD3;
                $scope.VisualInspection = rows.VisualInspection;
                $scope.VisualInspection1 = rows.VisualInspection1;
                $scope.VisualInspection2 = rows.VisualInspection2;
                $scope.VisualInspection3 = rows.VisualInspection3;
                $scope.Remarks1 = rows.Remarks1;
                $scope.Remarks2 = rows.Remarks2;
                $scope.Remarks3 = rows.Remarks3;
                $scope.Remarks4 = rows.Remarks4;
                $scope.Remarks5 = rows.Remarks5;
                $scope.Remarks6 = rows.Remarks6;
                $scope.Remarks7 = rows.Remarks7;
                $scope.Remarks8 = rows.Remarks8;
                $scope.Remarks9 = rows.Remarks9;
                $scope.Remarks10 = rows.Remarks10;
                $scope.Remarks11 = rows.Remarks11;
                $scope.Remarks12 = rows.Remarks12;

                $("#saveAndPrintQCDirectModal").modal("show");
                //if ($scope.DocumentName == "IP_ITEM_QC_SETUP") {
                //    $("#saveAndPrintQCDirectModal").modal("toggle");
                //}
                //$("#saveAndPrintQCDirectModal").data("print-url", printUrl).data("print-type", printType).modal("show");
            });
        }
        else if (printType && printType.includes('RawMaterial')) {
            var reqst = window.location.protocol + "//" + window.location.host +
                "/api/RawMaterialAPI/GetDailyRawMaterialReport?transactionno=" + voucherno;

            $http.get(reqst).then(function (responsesod) {
                var rows = responsesod.data;
                $scope.ITEM_CODE = rows.ITEM_CODE;
                $scope.QC_NO = rows.QC_NO;
                $scope.CREATED_DATE = rows.CREATED_DATE;
                $scope.BATCH_NO = rows.BATCH_NO;
                $scope.MANUAL_NO = rows.MANUAL_NO;
                $scope.REMARKS = rows.REMARKS;

                var editRawMaterials = responsesod.data.RawMaterialList;
                $scope.RawMaterialList = []; // reset
                $scope.childModels = [];

                editRawMaterials.forEach(function (row, rowIndex) {
                    var rawMaterialRow = [
                        { COLUMN_NAME: "ITEM_EDESC", VALUE: row.ITEM_EDESC },
                        { COLUMN_NAME: "ITEM_CODE", VALUE: row.ITEM_CODE },
                        { COLUMN_NAME: "SUPPLIER_EDESC", VALUE: row.SUPPLIER_EDESC },
                        { COLUMN_NAME: "SUPPLIER_CODE", VALUE: row.SUPPLIER_CODE },
                        { COLUMN_NAME: "BATCH_NO", VALUE: row.BATCH_NO },
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

                    $scope.RawMaterialList.push({ element: rawMaterialRow });
                    $scope.childModels[rowIndex] = {};
                    rawMaterialRow.forEach(function (item) {
                        $scope.childModels[rowIndex][item.COLUMN_NAME] = item.VALUE;
                    });
                });
                $("#saveAndPrintRawMaterialModal").modal("show");
                //$("#saveAndPrintRawMaterialModal").data("print-url", printUrl).data("print-type", printType).modal("show");
            });
        }
        else if (printType && printType.includes('DailyWastage')) {
            var reqst = window.location.protocol + "//" + window.location.host + "/api/DailyWastageApi/GetDailyWastageReport?transactionno=" + voucherno;
            $http.get(reqst).then(function (responsesod) {
                var rows = responsesod.data;
                $scope.DAILYWASTAGE_NO = responsesod.data.DAILYWASTAGE_NO;
                $scope.BATCH_NO = responsesod.data.BATCH_NO;
                $scope.CREATED_DATE = moment(responsesod.data.CREATED_DATE).format('DD-MMM-YYYY');
                $scope.ITEM_CODE = rows.ITEM_CODE;
                $scope.products = responsesod.data.PRODUCTS;
                $scope.ITEM_EDESC = [...new Set(
                    $scope.products.map(d => d.ITEM_EDESC)
                )].join(", ");
                $scope.childModels = responsesod.data.DailyWastageList;
            });
            $scope.DocumentName == "QC_PARAMETER_TRANSACTION"
            $("#saveAndPrintDailyWastageModal").modal("show");
        }
        else if (printType && printType.includes('HandOverInspection')) {
            var reqst = window.location.protocol + "//" + window.location.host + "/api/HandOverInspectionApi/GetHandOverInspectionReport?transactionno=" + voucherno;
            $http.get(reqst).then(function (responsesod) {
                var rows = responsesod.data;
                $scope.DISPATCH_NO = responsesod.data.DISPATCH_NO;
                $scope.PACKING_UNIT = responsesod.data.PACKING_UNIT;
                $scope.CREATED_DATE = moment(responsesod.data.CREATED_DATE).format('DD-MMM-YYYY');
                $('#englishdatedocument').val($scope.CREATED_DATE);
                $('#nepaliDate5').val(AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD')));
                $scope.REMARKS = responsesod.data.REMARKS;
                $scope.MANUAL_NO = responsesod.data.MANUAL_NO;
                $scope.childModels = responsesod.data.HandOverInspectionDetailsList;
            });
            $scope.DocumentName == "QC_PARAMETER_TRANSACTION";
            $("#saveAndPrintHandOverModal").modal("show");
            // $("#saveAndPrintHandOverModal").data("print-url", printUrl).data("print-type", printType).modal("show");
        }
        else if (printType && printType.includes('OnSiteInspection')) {
            var reqst = window.location.protocol + "//" + window.location.host + "/api/OnSiteInspectionApi/GetOnSiteInspectionReport?transactionno=" + voucherno;
            $http.get(reqst).then(function (responsesod) {
                var rows = responsesod.data;
                $scope.Inspection_No = responsesod.data.Inspection_No;
                $scope.selectedProductType = responsesod.data.item_code;
                $scope.REFERENCE_NO = responsesod.data.Reference_No;
                $scope.Batch_No = responsesod.data.Batch_No;
                $scope.Shift = responsesod.data.Shift;
                $scope.ITEM_CODE = responsesod.data.ITEM_CODE;
                $scope.products = responsesod.data.PRODUCTS;
                $scope.CREATED_DATE = moment(responsesod.data.CREATED_DATE).format('DD-MMM-YYYY');
                $scope.ITEM_EDESC = [...new Set(
                    $scope.products.map(d => d.ITEM_EDESC)
                )].join(", ");
                $scope.childModels = responsesod.data.ParameterDetailsList;
            });
            $scope.DocumentName == "QC_PARAMETER_TRANSACTION";
            $("#saveAndPrintOnSiteModal").modal("show");
            //$("#saveAndPrintOnSiteModal").data("print-url", printUrl).data("print-type", printType).modal("show");
        }
        else if (printType && printType.includes('InternalInspection')) {
            var reqst = window.location.protocol + "//" + window.location.host + "/api/InternalInspectionApi/GetInternalInspectionReport?transactionno=" + voucherno;
            $http.get(reqst).then(function (responsesod) {
                var rows = responsesod.data;
                $scope.Inspection_No = responsesod.data.Inspection_No;
                $scope.selectedProductType = responsesod.data.item_code;
                $scope.REFERENCE_NO = responsesod.data.Reference_No;
                $scope.Batch_No = responsesod.data.Batch_No;
                $scope.Shift = responsesod.data.Shift;
                $scope.DISPATCH_PERSON = responsesod.data.DISPATCH_PERSON;
                $scope.CREATED_DATE = moment(responsesod.data.CREATED_DATE).format('DD-MMM-YYYY');
                $scope.ITEM_CODE = responsesod.data.ITEM_CODE;
                $scope.products = responsesod.data.PRODUCTS;
                $scope.DISPATCH_PERSON = responsesod.data.DISPATCH_PERSON;
                $scope.ITEM_EDESC = [...new Set(
                    $scope.products.map(d => d.ITEM_EDESC)
                )].join(", ");
                $scope.childModels = responsesod.data.ParameterDetailsList;
            });
            $scope.DocumentName == "QC_PARAMETER_TRANSACTION";
            $("#saveAndPrintInternalModal").modal("show");
            //$("#saveAndPrintInternalModal").data("print-url", printUrl).data("print-type", printType).modal("show");
        }
        else if (printType && printType.includes('SanitationHygiene')) {
            var reqst = window.location.protocol + "//" + window.location.host + "/api/SanitationHygieneApi/GetSanitationHygieneReport?transactionno=" + voucherno;
            $http.get(reqst).then(function (responsesod) {
                var rows = responsesod.data;
                $scope.SANITATION_NO = responsesod.data.SANITATION_NO;
                $scope.CREATED_DATE = moment(responsesod.data.CREATED_DATE).format('DD-MMM-YYYY');
                $scope.childModels = responsesod.data.SanitationHygieneList;
            });
            $scope.DocumentName == "QC_PARAMETER_TRANSACTION";
            $("#saveAndPrintSanitationHygieneModal").modal("show");
            //$("#saveAndPrintSanitationHygieneModal").data("print-url", printUrl).data("print-type", printType).modal("show");
        }
        else if (printType && printType.includes('PreDispatchInspection')) {
            var reqst = window.location.protocol + "//" + window.location.host + "/api/PreDispatchInspectionApi/GetPreDispatchInspectionReport?transactionno=" + voucherno;
            $http.get(reqst).then(function (responsesod) {
                var rows = responsesod.data;
                $scope.DISPATCH_NO = responsesod.data.DISPATCH_NO;
                $scope.CREATED_DATE = moment(responsesod.data.CREATED_DATE).format('DD-MMM-YYYY');
                $scope.CUSTOMER_INVOICE_NO = responsesod.data.CUSTOMER_INVOICE_NO;
                $scope.DRIVER_CONTACT_NO = responsesod.data.DRIVER_CONTACT_NO;
                $scope.QC_INSPECTOR = responsesod.data.QC_INSPECTOR;
                $scope.TRANSPORT_DETAIL = responsesod.data.TRANSPORT_DETAIL;
                $scope.VEHICLE_NO = responsesod.data.VEHICLE_NO;
                $scope.CUSTOMER_NAME = responsesod.data.CUSTOMER_NAME;
                $scope.DRIVER_NAME = responsesod.data.DRIVER_NAME;
                $scope.DISPATCH_PERSON = responsesod.data.DISPATCH_PERSON;
                $scope.ISDUST_VEHICLE = responsesod.data.PreDispatchInspectionDetailsList[0].ISDUST_VEHICLE;
                $scope.VEHICLE_DUST_REMARKS = responsesod.data.PreDispatchInspectionDetailsList[0].VEHICLE_DUST_REMARKS;
                $scope.ISWATERSPILL_VEHICLE = responsesod.data.PreDispatchInspectionDetailsList[0].ISWATERSPILL_VEHICLE;
                $scope.VEHICLE_WATERSPILL_REMARKS = responsesod.data.PreDispatchInspectionDetailsList[0].VEHICLE_WATERSPILL_REMARKS;
                $scope.ISCRACKSHOLES_VEHICLE = responsesod.data.PreDispatchInspectionDetailsList[0].ISCRACKSHOLES_VEHICLE;
                $scope.VEHICLE_CRACKSHOLES_REMARKS = responsesod.data.PreDispatchInspectionDetailsList[0].VEHICLE_CRACKSHOLES_REMARKS;
                $scope.ISNAILS_VEHICLE = responsesod.data.PreDispatchInspectionDetailsList[0].ISNAILS_VEHICLE;
                $scope.VEHICLE_NAILS_REMARKS = responsesod.data.PreDispatchInspectionDetailsList[0].VEHICLE_NAILS_REMARKS;
                $scope.ISLEAKWALL_VEHICLE = responsesod.data.PreDispatchInspectionDetailsList[0].ISLEAKWALL_VEHICLE;
                $scope.VEHICLE_WALL_REMARKS = responsesod.data.PreDispatchInspectionDetailsList[0].VEHICLE_WALL_REMARKS;
                $scope.ISVISUALDEFECT_PRODUCT = responsesod.data.PreDispatchInspectionDetailsList[0].ISVISUALDEFECT_PRODUCT;
                $scope.PRODUCT_DEFECT_REMARKS = responsesod.data.PreDispatchInspectionDetailsList[0].PRODUCT_DEFECT_REMARKS;
                $scope.ISDIMENSIONS_PRODUCT = responsesod.data.PreDispatchInspectionDetailsList[0].ISDIMENSIONS_PRODUCT;
                $scope.PRODUCT_DIMENSIONS_REMARKS = responsesod.data.PreDispatchInspectionDetailsList[0].PRODUCT_DIMENSIONS_REMARKS;
                $scope.ISWEIGHTCHECK_PRODUCT = responsesod.data.PreDispatchInspectionDetailsList[0].ISWEIGHTCHECK_PRODUCT;
                $scope.PRODUCT_WEIGHT_REMARKS = responsesod.data.PreDispatchInspectionDetailsList[0].PRODUCT_WEIGHT_REMARKS;
                $scope.ISCORRECT_PACKAGING = responsesod.data.PreDispatchInspectionDetailsList[0].ISCORRECT_PACKAGING;
                $scope.PACKAGING_CORRECT_REMARKS = responsesod.data.PreDispatchInspectionDetailsList[0].PACKAGING_CORRECT_REMARKS;
                $scope.ISSEALED_PACKAGING = responsesod.data.PreDispatchInspectionDetailsList[0].ISSEALED_PACKAGING;
                $scope.PACKAGING_SEALED_REMARKS = responsesod.data.PreDispatchInspectionDetailsList[0].PACKAGING_SEALED_REMARKS;
                $scope.ISPERBOX_PACKAGING = responsesod.data.PreDispatchInspectionDetailsList[0].ISPERBOX_PACKAGING;
                $scope.PACKAGING_PERBOX_REMARKS = responsesod.data.PreDispatchInspectionDetailsList[0].PACKAGING_PERBOX_REMARKS;
                $scope.ISSTACKING_PACKAGING = responsesod.data.PreDispatchInspectionDetailsList[0].ISSTACKING_PACKAGING;
                $scope.PACKAGING_STACKING_REMARKS = responsesod.data.PreDispatchInspectionDetailsList[0].PACKAGING_STACKING_REMARKS;
                $scope.ISINVOICE_DOCUMENTATION = responsesod.data.PreDispatchInspectionDetailsList[0].ISINVOICE_DOCUMENTATION;
                $scope.DOCU_INVOICE_REMARKS = responsesod.data.PreDispatchInspectionDetailsList[0].DOCU_INVOICE_REMARKS;
                $scope.ISQUALITY_DOCUMENTATION = responsesod.data.PreDispatchInspectionDetailsList[0].ISQUALITY_DOCUMENTATION;
                $scope.DOCU_QUALITY_REMARKS = responsesod.data.PreDispatchInspectionDetailsList[0].DOCU_QUALITY_REMARKS;
                $scope.ISCOMPLIANCE_DOCUMENTATION = responsesod.data.PreDispatchInspectionDetailsList[0].ISCOMPLIANCE_DOCUMENTATION;
                $scope.DOCU_COMP_REMARKS = responsesod.data.PreDispatchInspectionDetailsList[0].DOCU_COMP_REMARKS;
                $http.get('/api/PreDispatchInspectionAPI/GetPreDispatchInspectionCheckList')
                    .then(function (responsesod) {
                        var qcqa = responsesod.data;
                        if (qcqa && qcqa.length > 0) {

                            $scope.PreDispatchItemsData = qcqa;
                            //$scope.childModelTemplate.push(qcqa);
                        }
                    });
            });
            $scope.DocumentName == "QC_PARAMETER_TRANSACTION";
            $("#saveAndPrintPreDispatchModal").modal("show");
            // $("#saveAndPrintPreDispatchModal").data("print-url", printUrl).data("print-type", printType).modal("show");
        }
        else {
            var reqst = window.location.protocol + "//" + window.location.host + "/api/FinishedGoodsInspectionApi/GetFinishedGoodsInspectionReport?transactionno=" + voucherno;
            $http.get(reqst).then(function (responsesod) {
                var rows = responsesod.data;
                //$scope.RECEIPT_DATE = formatDate(responsesod.data.RECEIPT_DATE);
                $scope.GRN_NO = responsesod.data.GRN_NO;
                $scope.VENDOR_NAME = responsesod.data.VENDOR_NAME;
                $scope.REFERENCE_NO = responsesod.data.REFERENCE_NO;
                $scope.BATCH_NO = responsesod.data.BATCH_NO;
                $scope.CREATED_DATE = moment(responsesod.data.CREATED_DATE).format('DD-MMM-YYYY');
                $scope.MFG_DATE = moment(responsesod.data.MFG_DATE).format('DD-MMM-YYYY');
                $scope.RECEIPT_DATE = moment(responsesod.data.RECEIPT_DATE).format('DD-MMM-YYYY');
                $scope.EXP_DATE = moment(responsesod.data.EXP_DATE).format('DD-MMM-YYYY');
                $scope.PLANT_ID = responsesod.data.Plant_Id;
                $scope.ITEM_EDESC = responsesod.data.ITEM_EDESC;
                $scope.QUANTITY = responsesod.data.QUANTITY;
                $scope.CHECKED_BY = responsesod.data.CHECKED_BY;
                $scope.AUTHORISED_BY = responsesod.data.AUTHORISED_BY;
                $scope.POSTED_BY = responsesod.data.POSTED_BY;
                $scope.DISPATCH_NO = responsesod.data.FINISH_GOODS_INSP_NO;
                $scope.PACK_CONDITION = responsesod.data.FinishedGoodsInspectionDetailsList[0].PACK_CONDITION;
                if ($scope.PACK_CONDITION == 'N')
                    $scope.PACK_CONDITION_NO = 'Y';
                $scope.PACK_COND_REMARKS = responsesod.data.FinishedGoodsInspectionDetailsList[0].PACK_COND_REMARKS;
                $scope.LABEL_ACCURACY = responsesod.data.FinishedGoodsInspectionDetailsList[0].LABEL_ACCURACY;
                if ($scope.LABEL_ACCURACY == 'N')
                    $scope.LABEL_ACCURACY_NO = 'Y';
                $scope.LABEL_ACC_REMARKS = responsesod.data.FinishedGoodsInspectionDetailsList[0].LABEL_ACC_REMARKS;
                $scope.PRODUCT_APPEARANCE = responsesod.data.FinishedGoodsInspectionDetailsList[0].PRODUCT_APPEARANCE;
                if ($scope.PRODUCT_APPEARANCE == 'N')
                    $scope.PRODUCT_APPEARANCE_NO = 'Y';
                $scope.PRODUCT_APP_REMARKS = responsesod.data.FinishedGoodsInspectionDetailsList[0].PRODUCT_APP_REMARKS;
                $scope.DIMENSIONS = responsesod.data.FinishedGoodsInspectionDetailsList[0].DIMENSIONS;
                if ($scope.DIMENSIONS == 'N')
                    $scope.DIMENSIONS_NO = 'Y';
                $scope.DIMENSIONS_REMARKS = responsesod.data.FinishedGoodsInspectionDetailsList[0].DIMENSIONS_REMARKS;
                $scope.COMPLIANCE_CERTIFICATES = responsesod.data.FinishedGoodsInspectionDetailsList[0].COMPLIANCE_CERTIFICATES;
                if ($scope.COMPLIANCE_CERTIFICATES == 'N')
                    $scope.COMPLIANCE_CERTIFICATES_NO = 'Y';
                $scope.COMP_CERT_REMARKS = responsesod.data.FinishedGoodsInspectionDetailsList[0].COMP_CERT_REMARKS;
                $scope.VENDOR_TEST = responsesod.data.FinishedGoodsInspectionDetailsList[0].VENDOR_TEST;
                if ($scope.VENDOR_TEST == 'N')
                    $scope.VENDOR_TEST_NO = 'Y';
                $scope.VENDOR_TEST_REMARKS = responsesod.data.FinishedGoodsInspectionDetailsList[0].VENDOR_TEST_REMARKS;
                $scope.SAMPLING_METHOD = responsesod.data.FinishedGoodsInspectionDetailsList[0].SAMPLING_METHOD;
                $scope.SAMP_METHOD_REMARKS = responsesod.data.FinishedGoodsInspectionDetailsList[0].SAMP_METHOD_REMARKS;
                $scope.SAMPLE_SIZE = responsesod.data.FinishedGoodsInspectionDetailsList[0].SAMPLE_SIZE;
                $scope.SAMP_SIZE_REMARKS = responsesod.data.FinishedGoodsInspectionDetailsList[0].SAMP_SIZE_REMARKS;
                $scope.NUMBER_PASSED = responsesod.data.FinishedGoodsInspectionDetailsList[0].NUMBER_PASSED;
                if ($scope.NUMBER_PASSED == 'N')
                    $scope.NUMBER_PASSED_NO = 'Y';
                $scope.NUMBER_PASSED_REMARKS = responsesod.data.FinishedGoodsInspectionDetailsList[0].NUMBER_PASSED_REMARKS;
                $scope.DEFECT_TYPE = responsesod.data.FinishedGoodsInspectionDetailsList[0].DEFECT_TYPE;
                $scope.DEFECT_TYPE_REMARKS = responsesod.data.FinishedGoodsInspectionDetailsList[0].DEFECT_TYPE_REMARKS;
                $scope.ACTION_TAKEN = responsesod.data.FinishedGoodsInspectionDetailsList[0].ACTION_TAKEN;
                $scope.ACTION_TAKEN_REMARKS = responsesod.data.FinishedGoodsInspectionDetailsList[0].ACTION_TAKEN_REMARKS;
                $scope.REMARKS = responsesod.data.FinishedGoodsInspectionDetailsList[0].REMARKS;
                $scope.FINAL_REMARKS = responsesod.data.FinishedGoodsInspectionDetailsList[0].FINAL_REMARKS;
                $http.get('/api/FinishedGoodsInspectionAPI/GetFinishedGoodsInspectionField')
                    .then(function (response) {
                        var qcqa = response.data;
                        if (qcqa && qcqa.length > 0) {

                            $scope.FinishedGoodsInspectionDetailsData = qcqa;
                            //$scope.childModelTemplate.push(qcqa);
                        }
                    });
            });
            $scope.DocumentName == "QC_PARAMETER_TRANSACTION";
            $("#saveAndPrintFinishedGoodsModal").modal("show");
            //$("#saveAndPrintFinishedGoodsModal").data("print-url", printUrl).data("print-type", printType).modal("show");
        }


    });

    $scope.printIncomingMaterial = function (divName) {
        var printContents = document.getElementById(divName).innerHTML;
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
    }

    $("#QCQADocumentFinderGrid").on("click", ".delete-btn", function () {
        var deleteButton = $(this);
        var id = $(this).data("id");
        // Create the popover element with custom HTML content
        var popoverContent = `
        <div class="popover-delete-confirm">
            <p>Delete?</p>
            <div class="popover-buttons">
                <button type="button" class="btn btn-danger confirm-delete">Yes</button>
                <button type="button" class="btn btn-secondary cancel-delete">No</button>
            </div>
        </div>
    `;
        deleteButton.popover({
            container: 'body',
            placement: 'bottom',
            html: true,
            content: popoverContent
        });

        // Show popover
        deleteButton.popover('show');

        // Handle click event on the "Yes" button
        $(document).on('click', '.confirm-delete', function () {
            $http.post('/api/QCQADocumentFinderAPI/DeleteQCByTransaction?transactionNo=' + id)
                .then(function (response) {
                    $scope.Outlets = response.data;
                    setTimeout(function () {
                        window.location.reload();
                    }, 5000)
                }).catch(function (error) {
                    var message = 'Error in displaying no!!'; // Extract message from response
                    displayPopupNotification(message, "error");
                });
            deleteButton.popover('hide');
        });

        // Handle click event on the "No" button
        $(document).on('click', '.cancel-delete', function () {
            // Hide the popover
            deleteButton.popover('hide');
        });

    });

});
