PMModule.controller('consumption', function ($scope, $rootScope, $http, $routeParams, inventoryservice, inventoryfactory, $window, $filter, hotkeys) {
    const ACC_CODE = "ACC_CODE";
    const BUDGET_FLAG = "BUDGET_FLAG";
    const BUDGET_CODE = "BUDGET_CODE";
    const TRANSACTION_TYPE = "TRANSACTION_TYPE";
    const AMOUNT = "AMOUNT";
    $scope.units = [];
    $scope.BUDGET_CENTER = [];
    $scope.ChargeList = [];
    $scope.tempCode = "";
    $scope.showRefTab = false;
    $scope.formBackDays = "";
    $scope.BUDGET_CENTER.push({
        BUDGET_VAL: "",
        AMOUNT: "",
        NARRATION: ""
    });
    $scope.dynamicModalData = [{
        LOCATION_CODE: "",
        BUDGET_FLAG: "",
        BUDGET: [{
            SERIAL_NO: 1,
            BUDGET_CODE: "",
            QUANTITY: "",
            ACC_CODE: "",
        }]

    }];
    $scope.dynamicBatchTrackingModalData = [{
        ITEM_CODE: "",
        ITEM_EDESC: "",
        MU_CODE: "",
        LOCATION_CODE: "",
        QUANTITY: "",
        BATCH_NO: "",
        EXPIRY_DATE: "",
        SERIAL_NO: 1
    }];

    $scope.dynamicSerialTrackingModalData = [{
        ITEM_CODE: "",
        ITEM_EDESC: "",
        MU_CODE: "",
        LOCATION_CODE: "",
        QUANTITY: "",
        TRACK: [{
            SERIAL_NO: 1,
            TRACKING_SERIAL_NO: "",
        }]

    }];
    $scope.referenceDataDisplay = [{
        REFERENCE_NO: "",
        ITEM_EDESC: "",
        REFERENCE_QUANTITY: "",
        REFERENCE_MU_CODE: "",
        REFERENCE_UNIT_PRICE: "",
        REFERENCE_TOTAL_PRICE: "",
        REFERENCE_CALC_UNIT_PRICE: "",
        REFERENCE_CALC_TOTAL_PRICE: "",
        REFERENCE_REMARKS: ""
    }];
    $scope.SDModel = {
        VEHICLE_CODE: "",
        VEHICLE_OWNER_NAME: "",
        VEHICLE_OWNER_NO: "",
        DRIVER_NAME: "",
        DRIVER_LICENCE_NO: "",
        DRIVER_MOBILE_NO: "",
        TRANSPORTER_CODE: "",
        SHIPPING_TERMS: "",
        FREIGHT_RATE: "",
        FREGHT_AMOUNT: "",
        START_FORM: "",
        DESTINATION: "",
        CN_NO: "",
        TRANSPORT_INVOICE_NO: "",
        TRANSPORT_INVOICE_DATE: null,
        DELIVERY_INVOICE_DATE: null,
        WB_WEIGHT: "",
        WB_NO: "",
        WB_DATE: null,
        GATE_ENTRY_NO: "",
        GATE_ENTRY_DATE: null,
        LOADING_SLIP_NO: ""
    };
    $scope.shortCut = "N";
    $scope.youFromReference = false;
    hotkeys.add({
        combo: 'ctrl+0',
        allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            if ($scope.shortCut == "N") {
                $scope.shortCut = "Y";
                setTimeout(function () {
                    $(".refrencetype").focus();
                }, 100);
            }
            else
                $scope.shortCut = "N";
        }
    });
    //Enable Master Field
    hotkeys.add({
        combo: 'shift+e',
        callback: function (event, hotkey) {
            $scope.freeze_master_ref_flag = 'N';
            $scope.freeze_manual_entry_flag = 'N';
            alert("Enable Master Field.");
        }
    });
    //// save and continuee
    //hotkeys.add({
    //    combo: 'ctrl+shift+enter',
    //    allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
    //    callback: function (event, hotkey) {
    //        // save and continuee
    //    }
    //});
    // save
    hotkeys.add({
        combo: 'ctrl+enter',
        allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {

            //event.preventDefault();
            var param = "";
            $scope.SaveDocumentFormData(0);
        }
    });
    // focus on first element of child field last row
    hotkeys.add({
        combo: 'shift+enter',
        allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            //$($(".childDiv :input").not(':button,:hidden')[0]).focus();
            $($($($(".document_child_table")[0]).find('tr')[$($(".document_child_table")[0]).find('tr').length - 1]).find('input').not(':button,:hidden,:disabled')[0]).focus();
        }
    });

    hotkeys.add({
        combo: 'ctrl+alt+t',
        callback: function (event, hotkey) {
            // $scope.toogleToolTips();
            $('i.font-green').toggle();
        }
    });
    // focus on first element of masters
    hotkeys.add({
        combo: 'ctrl+shift+backspace',
        allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            $($(".masterDiv :input").not(':button,:hidden,:disabled')[0]).focus();
        }
    });

    // navigate to transaction tab
    hotkeys.add({
        combo: 'shift+t',
        //allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            $('#myTab a[href="#tab_15_1"]').trigger('click');
        }
    });

    // navigate to Refrence tab
    hotkeys.add({
        combo: 'shift+r',
        //allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            $('#myTab a[href="#tab_15_2"]').trigger('click');
        }
    });

    // navigate to Custom tab
    hotkeys.add({
        combo: 'shift+c',
        //allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            $('#myTab a[href="#tab_15_3"]').trigger('click');
        }
    });

    // navigate to Document tab
    hotkeys.add({
        combo: 'shift+d',
        //allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            $('#myTab a[href="#tab_15_4"]').trigger('click');
        }
    });

    // navigate to reset tab
    hotkeys.add({
        combo: 'alt+r',
        //allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            $scope.reset();
        }
    });

    // navigate to save and continue tab
    hotkeys.add({
        combo: 'alt+c',
        //allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            $scope.SaveDocumentFormData(1);
        }
    });

    //navigate to save and print tab
    hotkeys.add({
        combo: 'alt+p',
        //allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            $scope.SaveDocumentFormData(2);
        }
    });

    // navigate to draftlist tab
    hotkeys.add({
        combo: 'alt+d',
        //allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            $scope.showDraftModal();
        }
    });

    // navigate to save as draftlist tab
    hotkeys.add({
        combo: 'alt+s',
        //allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            $scope.SaveAsDraft(1);
        }
    });

    // navigate to refernce
    hotkeys.add({
        combo: 'shift+alt+r',
        allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            $('#RefrenceModelShow').modal('toggle');
        }
    });

    // update and print
    hotkeys.add({
        combo: 'alt+shift+p',
        callback: function (event, hotkey) {
            $scope.SaveDocumentFormData(4);
        }
    });
    //Enable Master Field
    hotkeys.add({
        combo: 'shift+e',
        callback: function (event, hotkey) {
            $scope.freeze_master_ref_flag = 'N';
            alert("Enable Master Field.");
        }
    });
    $scope.dynamicSubLedgerModalData = [{
        ACC_CODE: 0,
        TRANSACTION_TYPE: "",
        VOUCHER_AMOUNT: "",
        SUBLEDGER_AMOUNT: "",
        REMAINING_AMOUNT: "",
        SUBLEDGER: [{
            SUB_CODE: "",
            SUB_EDESC: "",
            AMOUNT: "",
            PARTICULARS: "",
            REFRENCE: ""
        }]
    }];


    $scope.saveAsDraft = {
        TEMPLATE_NO: "",
        TEMPLATE_EDESC: "",
        TEMPLATE_NDESC: ""
    }
    if ($routeParams.orderno != undefined) {
        $scope.OrderNo = $routeParams.orderno.split(new RegExp('_', 'i')).join('/');

    }
    else { $scope.OrderNo = "undefined"; }
    //draft 

    if ($routeParams.tempCode != undefined) {

        $scope.tempCode = $routeParams.tempCode;
    }

    $scope.formcode = $routeParams.formcode;

    $scope.FormCode = $routeParams.formcode;
    document.formCode = $scope.FormCode;
    $rootScope.refCheckedItem = [];


    var d1 = $.Deferred();
    var d2 = $.Deferred();
    var d3 = $.Deferred();
    var d4 = $.Deferred();
    var d5 = $.Deferred();
    var d6 = $.Deferred();
    $scope.productDescription = '';
    $scope.producttemp = '';
    $scope.NepaliDate = '';
    $rootScope.suppliervalidation = "";
    $scope.FormName = '';
    var suppliercodeforupdate = "";
    $scope.VoucherCount = '';
    $scope.DocumentName = ""; // document display name at top

    $scope.RefTableName = "";
    $scope.subProjectList = "";

    $scope.MasterFormElement = []; // for elements having master_child_flag='M'
    $scope.ChildFormElement = [{ element: [], additionalElements: $scope.BUDGET_CENTER }]; // initial child element

    $scope.accsummary = { 'drTotal': 0.00, 'crTotal': 0.00, 'diffAmount': 0.00 }

    $scope.aditionalChildFormElement = []; // a blank child element model while add button press.
    $scope.formDetail = ""; // all form_dtl elements retrived from service (contains master_child 'M' && 'C' ).
    $scope.formSetup = "";
    $scope.formCustomSetup = "";
    $scope.CustomFormElement = [];
    $scope.freeze_manual_entry_flag = false;

    $scope.ModuleCode = "";
    //$scope.ModuleCode = '01';
    $scope.MasterFormElementValue = [];
    $scope.SalesOrderformDetail = "";
    $scope.datePattern = /^[01][0-9]-(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)-\d{4}$/
    $scope.companycode = "";
    //^[01][0-9]-(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)-\d{4}$

    $scope.BUDGET_MODAL = [];
    $scope.BUDGET_CHILD_MODAL = [];

    $scope.BATCH_MODAL = [];
    $scope.BATCH_CHILD_MODAL = [];

    $scope.BATCH_TRACKING_MODEL = [];


    $scope.todaydate = "";
    $scope.after_delemeter_value = "NO";
    $scope.masterModels = {}; // for master model
    $scope.childModels = []; // for dynamic
    $scope.customModels = {};
    $scope.ItemInfo = [];
    var newordernumber = "";

    $scope.save = "Save";
    $scope.savecontinue = "Save & Continue";

    $scope.draftsave = "Save";

    $scope.ref_form_code = "";
    //masterModelTemplate
    $scope.masterModelTemplate = {};
    $scope.childModelTemplate = {};

    $scope.masterChildData = null;
    $scope.onreferenceclickshow = false;

    $scope.selectedProductOptions = [];
    $scope.ProductsChanged = [];
    //summary Object
    $scope.dzvouchernumber = "";
    $scope.dzvoucherdate = "";
    $scope.dzformcode = "";
    $scope.summary = { 'grandTotal': 0 }
    $scope.newgenorderno = "";
    $scope.childelementwidth = "";
    $scope.havRefrence = 'N';
    $scope.ref_form_code = "";
    $scope.freeze_master_ref_flag = 'N';
    $scope.ref_fix_qty = 'N';
    $scope.ref_fix_price = 'N';
    $scope.NEGETIVE_STOCK_FLAG = 'N';
    $rootScope.ITEM_CODE_DEFAULTVAL = "";
    $scope.havRefrence = 'Y';
    $scope.RT_CONTROL_FLAG = 'N';
    $scope.formControlModelData = [{
        USER_NO: "",
        FORM_CODE: "",
        CREATE_FLAG: "",
        READ_FLAG: "",
        UPDATE_FLAG: "",

        DELETE_FLAG: "",
        POST_FLAG: "",
        UNPOST_FLAG: "",
        CHECK_FLAG: "",
        VERIFY_FLAG: "",

        MORE_FLAG: "",
        COMPANY_CODE: "",
        CREATED_BY: "",
        CREATED_DATE: "",
        DELETED_FLAG: "",

        UNCHECK_FLAG: "",
        UNVERIFY_FLAG: "",
        BRANCH_CODE: "",
        SYN_ROWID: "",
        MODIFY_DATE: "",

        MODIFY_BY: "",
        CHECK_VALUE: "",
        VERIFY_VALUE: "",
        POST_VALUE: ""

    }];

    //vstart

    var formUrl = "/api/SetupApi/GetFormControlByFormCode?formcode=" + $scope.formcode;
    $http.get(formUrl).then(function (response) {
        if (response.data.length != 0) {
            $scope.formControlModelData = response.data;
        }
    });
    // vend

    //Sub Project List for Consumption Entry
    var formUrl = "/api/EntryApi/GetSubProjectList";
    $http.get(formUrl).then(function (response) {
        if (response.data.length != 0) {
            $scope.subProjectList = response.data;
        }
    });
    console.log($scope.subProjectList);
    var selectElement = angular.element(document.getElementById('subProjectSelect')); // Get the select element by its ID

    //End Sub Project List for Consumption Entry

    $scope.checkRefrences = function () {
        var req = "/api/TemplateApi/GetRefrenceFlag?formCode=" + $scope.formcode;
        $http.get(req).then(function (results) {

            var response = results.data;
            //$scope.havRefrence = response[0].REFERENCE_FLAG;
            //$scope.RefTableName = response[0].REF_TABLE_NAME;
            //$scope.ref_form_code = response[0].REF_FORM_CODE;
            //$scope.freeze_master_ref_flag = response[0].FREEZE_MASTER_REF_FLAG;
            //$scope.ref_fix_qty = response[0].REF_FIX_QUANTITY;
            //$scope.ref_fix_price = response[0].REF_FIX_PRICE;

            $scope.havRefrence = response.FormSetupRefrence[0].REFERENCE_FLAG;
            $scope.RefTableName = response.FormSetupRefrence[0].REF_TABLE_NAME;
            $scope.ref_form_code = response.FormSetupRefrence[0].REF_FORM_CODE;
            $scope.freeze_master_ref_flag = response.FormSetupRefrence[0].FREEZE_MASTER_REF_FLAG;
            $scope.ref_fix_qty = response.FormSetupRefrence[0].REF_FIX_QUANTITY;
            $scope.ref_fix_price = response.FormSetupRefrence[0].REF_FIX_PRICE;
            $scope.freeze_manual_entry_flag = response.FormSetupRefrence[0].FREEZE_MANUAL_ENTRY_FLAG;
            $scope.formBackDays = response.FormSetupRefrence[0].FREEZE_BACK_DAYS;
            $scope.RT_CONTROL_FLAG = response.FormSetupRefrence[0].RT_CONTROL_FLAG;
            $scope.serial_tracking_flag = response.FormSetupRefrence[0].SERIAL_TRACKING_FLAG;
            $scope.batch_tracking_flag = response.FormSetupRefrence[0].BATCH_TRACKING_FLAG;
        });
    }
    $scope.checkRefrences();
    $scope.printCompanyInfo = {
        companyName: '',
        address: '',
        formName: '',
        phoneNo: '',
        email: '',
        tPinVatNo: '',
        formType: '',
    }

    $scope.initialMasterVal = [];
    $scope.initialChildVal = [{ element: [], additionalElements: $scope.BUDGET_CENTER }];

    function PrimaryColumnForTable(tablename) {

        var primarycolumn = "";
        if (tablename == "FA_SINGLE_VOUCHER" || tablename == "FA_DOUBLE_VOUCHER") {
            primarycolumn = "VOUCHER_NO";
        }
        else if (tablename == "IP_PURCHASE_MRR" || tablename == "IP_ADVICE_MRR" || tablename == "IP_PRODUCTION_MRR") {
            primarycolumn = "MRR_NO";
        }
        else if (tablename == "IP_PURCHASE_REQUEST") {
            primarycolumn = "REQUEST_NO";
        }
        else if (tablename == "IP_PURCHASE_INVOICE") {
            primarycolumn = "INVOICE_NO";
        }
        else if (tablename == "IP_PURCHASE_RETURN" || tablename == "SA_SALES_RETURN") {
            primarycolumn = "RETURN_NO";
        }
        else if (tablename == "IP_GOODS_REQUISITION") {
            primarycolumn = "REQUISITION_NO";
        }
        else if (tablename == "IP_QUOTATION_INQUIRY") {
            primarycolumn = "QUOTE_NO";
        }
        else if (tablename == "IP_TRANSFER_ISSUE" || tablename == "IP_GOODS_ISSUE" || tablename == "IP_GATE_PASS_ENTRY" || tablename == "IP_PRODUCTION_ISSUE") {
            primarycolumn = "ISSUE_NO";
        }
        else if (tablename == "IP_PURCHASE_ORDER" || tablename == "SA_SALES_ORDER") {
            primarycolumn = "ORDER_NO";
        }
        else if (tablename == "SA_SALES_CHALAN") {
            primarycolumn = "CHALAN_NO";
        }
        else if (tablename == "SA_SALES_INVOICE") {
            primarycolumn = "SALES_NO";
        }
        return primarycolumn;
    }

    checkRefrences();

    function checkRefrences() {
        var req = "/api/TemplateApi/GetRefrenceFlag?formCode=" + $scope.FormCode;
        $http.get(req).then(function (results) {
            var response = results.data;
            //$scope.havRefrence = response[0].REFERENCE_FLAG;
            //$scope.RefTableName = response[0].REF_TABLE_NAME;
            //$scope.ref_form_code = response[0].REF_FORM_CODE;

            $scope.havRefrence = response.FormSetupRefrence[0].REFERENCE_FLAG;
            $scope.RefTableName = response.FormSetupRefrence[0].REF_TABLE_NAME;
            $scope.ref_form_code = response.FormSetupRefrence[0].REF_FORM_CODE;
        });
    }
    $scope.inventoryRefrenceFn = function (response, callback) {
        debugger;
        var primarycolumnname = PrimaryColumnForTable($scope.RefTableName);
        $scope.youFromReference = true;
        var rows = response.data;
        $scope.BUDGET_MODAL.push({ ACC_CODE: "", BUDGET_FLAG: "", BUDGET: [] });
        $scope.BUDGET_CHILD_MODAL.push({ BUDGET_VAL: "", AMOUNT: "", NARRATION: "" });


        //$scope.BATCH_MODAL.push({ ITEM_CODE: "", ITEM_EDESC: "", MU_CODE: "", LOCATION_CODE: "", QUANTITY: "", TRACK: [] });
        //$scope.BATCH_CHILD_MODAL.push({ SERIAL_NO: 1, TRACKING_SERIAL_NO: ""});

        if (rows.length > 0) {

            var imageurl = [];
            var imageslistcount = "";
            if (rows[0].IMAGES_LIST != null || rows[0].IMAGES_LIST != undefined) {
                imageslistcount = rows[0].IMAGES_LIST.length;

                $.each(rows[0].IMAGES_LIST, function (key, value) {
                    var filepath = value.DOCUMENT_FILE_NAME;
                    var path = filepath.replace(/[/]/g, '_');
                    imageurl.push(path);
                });
                if (imageurl.length > 0) {
                    for (var i = 0; i < imageurl.length; i++) {
                        var mockFile = {
                            name: rows[0].IMAGES_LIST[i].DOCUMENT_NAME,
                            size: 12345,
                            type: 'image/jpeg',
                            url: imageurl[i],
                            accepted: true,
                        };
                        if (i == 0) {
                            myInventoryDropzone.on("addedfile", function (file) {
                                caption = file.caption == undefined ? "" : file.caption;
                                file._captionLabel = Dropzone.createElement("<a class='fa fa-download dropzone-download' href='" + imageurl[i] + "' name='Download' class='dropzone_caption' download></a>");
                                file.previewElement.appendChild(file._captionLabel);
                            });
                        }
                        myInventoryDropzone.emit("addedfile", mockFile);
                        myInventoryDropzone.emit("thumbnail", mockFile, imageurl[i]);
                        myInventoryDropzone.emit('complete', mockFile);
                        myInventoryDropzone.files.push(mockFile);
                        $('.dz-details').find('img').addClass('sr-only');
                        $('.dz-remove').css("display", "none");
                    }
                }
            }
            $scope.masterModels = {};
            var masterModel = angular.copy($scope.masterModelTemplate);
            var savedData = $scope.getObjWithKeysFromOtherObj(masterModel, response.data[0]);
            $scope.masterModels = savedData;

            //to solve problem in suppliercode binding for update purpose
            suppliercodeforupdate = $scope.masterModels.SUPPLIER_CODE;

            //for mrr_date in goods receipt note -store and spare
            //if ($scope.RefTableName === "IP_PURCHASE_MRR" && $scope.DocumentName==="IP_PURCHASE_INVOICE") {
            //    if ($scope.masterModels.hasOwnProperty("INVOICE_DATE")) {
            //        $scope.masterModels.INVOICE_DATE = response.data[0].MRR_DATE;
            //    }
            //}
            //if ($scope.RefTableName === "IP_PURCHASE_INVOICE" && $scope.DocumentName === "IP_PURCHASE_RETURN") {
            //    if ($scope.masterModels.hasOwnProperty("RETURN_DATE")) {
            //        $scope.masterModels.RETURN_DATE = response.data[0].INVOICE_DATE;
            //    }
            //}
            //if ($scope.RefTableName === "IP_GOODS_REQUISITION" && $scope.DocumentName === "IP_GOODS_ISSUE") {
            //    if ($scope.masterModels.hasOwnProperty("ISSUE_DATE")) {
            //        $scope.masterModels.ISSUE_DATE = response.data[0].REQUISITION_DATE;
            //    }
            //}
            //if ($scope.RefTableName === "IP_GOODS_ISSUE" && $scope.DocumentName === "IP_GOODS_ISSUE_RETURN") {
            //    if ($scope.masterModels.hasOwnProperty("RETURN_DATE")) {
            //        $scope.masterModels.RETURN_DATE = response.data[0].ISSUE_DATE;
            //    }
            //}
            //$scope.masterModels.MRR_DATE = response.data[0].ORDER_DATE;

            $scope.ChildFormElement = [];
            $scope.childModels = [];
            $scope.referenceDataDisplay1 = [];
            $scope.dynamicSerialTrackingModalData = [];
            $scope.dynamicBatchTrackingModalData = [];
            for (var i = 0; i < rows.length; i++) {
                setDataOnModal(rows, i);
                var tempCopy111 = angular.copy($scope.referenceDataDisplay);
                $scope.referenceDataDisplay1.push(tempCopy111);
            }
            //for setting today date for primary date column(master)
            if ($scope.masterModels.hasOwnProperty(GetPrimaryDateByTableName($scope.DocumentName))) {
                var primarydatecolname = GetPrimaryDateByTableName($scope.DocumentName);
                $scope.masterModels[primarydatecolname] = $filter('date')(new Date(), 'dd-MMM-yyyy');
            }
            angular.forEach(rows, function (refrencerow, refrenkey) {
                if ($scope.RefTableName === "IP_PURCHASE_ORDER") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ORDER_NO;
                }
                if ($scope.RefTableName === "IP_GOODS_REQUISITION") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.REQUISITION_NO;
                }
                if ($scope.RefTableName === "IP_TRANSFER_ISSUE") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ISSUE_NO;
                }
                if ($scope.RefTableName === "IP_GATE_PASS_ENTRY") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ISSUE_NO;
                }
                if ($scope.RefTableName === "IP_PURCHASE_RETURN") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.RETURN_NO;
                }
                if ($scope.RefTableName === "IP_RETURNABLE_GOODS_RETURN") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ISSUE_NO;
                }
                if ($scope.RefTableName === "IP_GOODS_ISSUE") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ISSUE_NO;
                }
                if ($scope.RefTableName === "IP_PURCHASE_MRR") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.MRR_NO;
                }
                if ($scope.RefTableName === "IP_PURCHASE_INVOICE") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.INVOICE_NO;
                }
                if ($scope.RefTableName === "IP_GOODS_ISSUE_RETURN") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.RETURN_NO;
                }
                if ($scope.RefTableName === "IP_PURCHASE_REQUEST") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.REQUEST_NO;
                }
                //var switch_on = $scope.DocumentName;
                //switch (switch_on) {
                //    case 'IP_PURCHASE_ORDER':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ORDER_NO;
                //        break;
                //    case 'IP_GOODS_REQUISITION':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.REQUISITION_NO;
                //        break;
                //    case 'IP_TRANSFER_ISSUE':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ISSUE_NO;      
                //        break;
                //    case 'IP_GATE_PASS_ENTRY':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ISSUE_NO;
                //        break;
                //    case 'IP_PURCHASE_RETURN':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.RETURN_NO;
                //        break;
                //    case 'IP_RETURNABLE_GOODS_RETURN':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ISSUE_NO;
                //        break;
                //    case 'IP_GOODS_ISSUE':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ISSUE_NO;
                //        break;
                //    case 'IP_PURCHASE_MRR':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.MRR_NO;
                //        break;
                //    case 'IP_PURCHASE_INVOICE':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.INVOICE_NO;
                //        break;
                //    case 'IP_GOODS_ISSUE_RETURN':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.RETURN_NO;
                //        break;
                //    case 'IP_PURCHASE_REQUEST':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.REQUEST_NO;
                //        break;
                //    default:
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.VOUCHER_NO;
                //}

                $scope.referenceDataDisplay1[refrenkey].ITEM_EDESC = refrencerow.ITEM_EDESC;
                $scope.referenceDataDisplay1[refrenkey].REFERENCE_QUANTITY = refrencerow.QUANTITY;
                $scope.referenceDataDisplay1[refrenkey].REFERENCE_MU_CODE = refrencerow.MU_CODE;
                $scope.referenceDataDisplay1[refrenkey].REFERENCE_UNIT_PRICE = refrencerow.UNIT_PRICE;
                $scope.referenceDataDisplay1[refrenkey].REFERENCE_TOTAL_PRICE = refrencerow.TOTAL_PRICE;
                $scope.referenceDataDisplay1[refrenkey].REFERENCE_CALC_UNIT_PRICE = refrencerow.CALC_UNIT_PRICE;
                $scope.referenceDataDisplay1[refrenkey].CALC_TOTAL_PRICE = refrencerow.TOTAL_PRICE;
                $scope.referenceDataDisplay1[refrenkey].REFERENCE_REMARKS = refrencerow.REMARKS;
            });
            $scope.refBGridOptions = {
                dataSource: {
                    type: "json",
                    //transport: {
                    //    read: $scope.referenceDataDisplay1,
                    //},
                    data: $scope.referenceDataDisplay1,
                    pageSize: 5,
                    serverPaging: true,
                    serverSorting: true
                },
                sortable: true,
                pageable: true,
                dataBound: function () {
                    this.expandRow(this.tbody.find("tr.k-master-row").first());
                },
                columns: [{
                    field: "REFERENCE_NO",
                    title: "Document No",
                    width: "120px"
                }, {
                    field: "ITEM_EDESC",
                    title: "Item",
                    width: "120px"
                }, {
                    field: "REFERENCE_QUANTITY",
                    title: "Quantity",
                    width: "120px"
                }, {
                    field: "REFERENCE_MU_CODE",
                    title: "Unit",
                    width: "120px"
                },
                {
                    field: "REFERENCE_UNIT_PRICE",
                    title: "Unit Price",
                    width: "120px"
                },
                {
                    field: "REFERENCE_TOTAL_PRICE",
                    title: "Total Price",
                    width: "120px"
                },
                {
                    field: "REFERENCE_CALC_UNIT_PRICE",
                    title: " Calc Unit Price",
                    width: "120px"
                },
                {
                    field: "REFERENCE_CALC_TOTAL_PRICE",
                    title: " Calc Total Price",
                    width: "120px"
                },
                {
                    field: "REFERENCE_REMARKS",
                    title: "Remarks",
                    width: "120px"
                }
                ]
            };
            $scope.ChildSumOperations(0);
            if ($rootScope.IncludeCharge === "True") {
                var reforderno = response.data[0][primarycolumnname];
                var chargeUrlForEdit = "/api/TemplateApi/GetChargeDataForEdit?formCode=" + rows[0].FORM_CODE + "&&voucherNo=" + reforderno;
                $http.get(chargeUrlForEdit).then(function (res) {

                    if (reforderno != "undefined") {
                        setTimeout(function () {
                            if (res.data.length > 0) {

                                $scope.data = res.data;

                                $.each(res.data, function (it, val) {

                                    $.each($scope.ChargeList, function (i, v) {

                                        if (val.CHARGE_CODE === v.CHARGE_CODE) {
                                            v.CHARGE_AMOUNT = val.CHARGE_AMOUNT;

                                            if (val.VALUE_PERCENT_FLAG == "P") {
                                                val.VALUE_PERCENT_AMOUNT = (val.CHARGE_AMOUNT * 100) / $scope.summary.grandTotal;

                                            }
                                            else {

                                                //v.VALUE_PERCENT_AMOUNT = val.CHARGE_AMOUNT;
                                            }
                                            v.ACC_CODE = val.ACC_CODE;
                                        }
                                    });
                                });

                                $scope.calculateChargeAmountrefrence($scope.data, true);


                            }
                            $scope.$apply();
                        }, 1000);
                    }

                });
            }
            $scope.showRefTab = true;

        }

        function setDataOnModal(rows, i) {

            var tempCopy = angular.copy($scope.childModelTemplate);
            $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
            $scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));
            setTimeout(function () {
                $("#products_" + i).data('kendoComboBox').dataSource.data([{ ItemCode: rows[i].ITEM_CODE, ItemDescription: rows[i].ITEM_EDESC, Type: "code" }]);
                console.log("value of productCode from kendo===================>>>>" + JSON.stringify($("#products_" + i).data('kendoComboBox').dataSource.data()[0].ItemCode));
                $scope.childModels[i].ITEM_CODE = $("#products_" + i).data('kendoComboBox').dataSource.data()[0].ItemCode;
            }, 0);
            var rowsObj = rows[i];

            var budgetModel = angular.copy($scope.BUDGET_MODAL[0]);
            var budgetChildModel = angular.copy($scope.BUDGET_CHILD_MODAL[0]);

            var batchModel = angular.copy($scope.BATCH_MODAL[0]);
            var batchChildModel = angular.copy($scope.BATCH_CHILD_MODAL[0]);

            $scope.dynamicModalData.push($scope.getObjWithKeysFromOtherObj(budgetModel, rowsObj));
            if ($scope.dynamicModalData[i].BUDGET == undefined) {
                $scope.dynamicModalData[i].BUDGET = [];
                $scope.dynamicModalData[i].BUDGET.push(budgetChildModel);
            }

            $scope.dynamicSerialTrackingModalData.push($scope.getObjWithKeysFromOtherObj(batchModel, rowsObj));
            if ($scope.dynamicSerialTrackingModalData[i].TRACK == undefined) {
                $scope.dynamicSerialTrackingModalData[i].TRACK = [];
                $scope.dynamicSerialTrackingModalData[i].TRACK.push(batchChildModel);
            }
            var batchtrackingmodel = angular.copy($scope.BATCH_TRACKING_MODEL[0]);
            $scope.dynamicBatchTrackingModalData.push($scope.getObjWithKeysFromOtherObj(batchtrackingmodel, rowsObj));
            var config = {
                async: false
            };
            var voucherNo = PrimaryColumnForTable($scope.DocumentName);
            var response = $http.get('/api/InventoryApi/GetDataForBudgetModal?VOUCHER_NO=' + rows[i][voucherNo], config);

            response.then(function (budgetResult) {

                if (budgetResult.data.length > 0) {


                    for (var a = 0; a < budgetResult.data.length; a++) {

                        for (var b = 0; b < $scope.dynamicModalData.length; b++) {

                            if (budgetResult.data[a].ACC_CODE == $scope.dynamicModalData[b].ACC_CODE) {

                                var serialno = budgetResult.data[a].SERIAL_NO - 1;

                                $scope.dynamicModalData[b].BUDGET[a] = $scope.getObjWithKeysFromOtherObj(budgetChildModel, budgetResult.data[a]);

                            }
                        }
                    }

                }

            });
            //for serial transaction data
            var serialresponse = $http.get('/api/InventoryApi/GetDataForBatchModal?VOUCHER_NO=' + rows[i][voucherNo], config);

            serialresponse.then(function (serialResult) {

                if (serialResult.data.length > 0) {


                    //for (var a = 0; a < serialResult.data.length; a++) {

                    //    for (var b = 0; b < $scope.dynamicSerialTrackingModalData.length; b++) {

                    //        if (serialResult.data[a].ITEM_CODE == $scope.dynamicSerialTrackingModalData[b].ITEM_CODE) {

                    //            var serialno = batchResult.data[a].SERIAL_NO - 1;

                    //            $scope.dynamicSerialTrackingModalData[b].TRACK[a] = $scope.getObjWithKeysFromOtherObj(batchChildModel, serialResult.data[a]);

                    //        }
                    //    }
                    //}


                    for (var b = 0; b < $scope.dynamicSerialTrackingModalData.length; b++) {

                        var iii = 0;
                        for (var a = 0; a < serialResult.data.length; a++) {

                            if (serialResult.data[a].ITEM_CODE == $scope.dynamicSerialTrackingModalData[b].ITEM_CODE) {

                                //var serialno = batchResult.data[a].SERIAL_NO - 1;

                                $scope.dynamicSerialTrackingModalData[b].TRACK[iii] = $scope.getObjWithKeysFromOtherObj(batchChildModel, serialResult.data[a]);
                                iii = iii + 1;
                            }
                        }
                    }

                }


            });
            //for batch tracking data
            var batchtrackingresponse = $http.get('/api/InventoryApi/GetDataForBatchTrackingModal?VOUCHER_NO=' + rows[i][voucherNo], config);
            batchtrackingresponse.then(function (batchTrackingResult) {

                if (batchTrackingResult.data.length > 0) {

                    for (var a = 0; a < batchTrackingResult.data.length; a++) {

                        $scope.dynamicBatchTrackingModalData[a] = $scope.getObjWithKeysFromOtherObj(batchtrackingmodel, batchTrackingResult.data[a]);
                    }
                }

            });

        }
        callback();
    };
    function hideloader() {
        $(".busy-loader").css("display", "none");
    }
    $scope.inventoryProductCalc = function (response, callback) {
        debugger;


        var rows = response.data;

        if (rows.length > 0) {

            var imageurl = [];
            var imageslistcount = "";


            $scope.ChildFormElement = [];
            $scope.childModels = [];
            $scope.referenceDataDisplay1 = [];
            for (var i = 0; i < rows.length; i++) {
                setDataOnModal(rows, i);
                var tempCopy111 = angular.copy($scope.referenceDataDisplay);
                $scope.referenceDataDisplay1.push(tempCopy111);
            }
            //for setting today date for primary date column(master)
            if ($scope.masterModels.hasOwnProperty(GetPrimaryDateByTableName($scope.DocumentName))) {
                var primarydatecolname = GetPrimaryDateByTableName($scope.DocumentName);
                $scope.masterModels[primarydatecolname] = $filter('date')(new Date(), 'dd-MMM-yyyy');
            }
            angular.forEach(rows, function (refrencerow, refrenkey) {
                if ($scope.RefTableName === "IP_PURCHASE_ORDER") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ORDER_NO;
                }
                if ($scope.RefTableName === "IP_GOODS_REQUISITION") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.REQUISITION_NO;
                }
                if ($scope.RefTableName === "IP_TRANSFER_ISSUE") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ISSUE_NO;
                }
                if ($scope.RefTableName === "IP_GATE_PASS_ENTRY") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ISSUE_NO;
                }
                if ($scope.RefTableName === "IP_PURCHASE_RETURN") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.RETURN_NO;
                }
                if ($scope.RefTableName === "IP_RETURNABLE_GOODS_RETURN") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ISSUE_NO;
                }
                if ($scope.RefTableName === "IP_GOODS_ISSUE") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ISSUE_NO;
                }
                if ($scope.RefTableName === "IP_PURCHASE_MRR") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.MRR_NO;
                }
                if ($scope.RefTableName === "IP_PURCHASE_INVOICE") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.INVOICE_NO;
                }
                if ($scope.RefTableName === "IP_GOODS_ISSUE_RETURN") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.RETURN_NO;
                }
                if ($scope.RefTableName === "IP_PURCHASE_REQUEST") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.REQUEST_NO;
                }
                //var switch_on = $scope.DocumentName;
                //switch (switch_on) {
                //    case 'IP_PURCHASE_ORDER':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ORDER_NO;
                //        break;
                //    case 'IP_GOODS_REQUISITION':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.REQUISITION_NO;
                //        break;
                //    case 'IP_TRANSFER_ISSUE':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ISSUE_NO;      
                //        break;
                //    case 'IP_GATE_PASS_ENTRY':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ISSUE_NO;
                //        break;
                //    case 'IP_PURCHASE_RETURN':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.RETURN_NO;
                //        break;
                //    case 'IP_RETURNABLE_GOODS_RETURN':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ISSUE_NO;
                //        break;
                //    case 'IP_GOODS_ISSUE':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ISSUE_NO;
                //        break;
                //    case 'IP_PURCHASE_MRR':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.MRR_NO;
                //        break;
                //    case 'IP_PURCHASE_INVOICE':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.INVOICE_NO;
                //        break;
                //    case 'IP_GOODS_ISSUE_RETURN':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.RETURN_NO;
                //        break;
                //    case 'IP_PURCHASE_REQUEST':
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.REQUEST_NO;
                //        break;
                //    default:
                //        $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.VOUCHER_NO;
                //}

                $scope.referenceDataDisplay1[refrenkey].ITEM_EDESC = refrencerow.ITEM_EDESC;
                $scope.referenceDataDisplay1[refrenkey].REFERENCE_QUANTITY = refrencerow.QUANTITY;
                $scope.referenceDataDisplay1[refrenkey].REFERENCE_MU_CODE = refrencerow.MU_CODE;
                $scope.referenceDataDisplay1[refrenkey].REFERENCE_UNIT_PRICE = refrencerow.UNIT_PRICE;
                $scope.referenceDataDisplay1[refrenkey].REFERENCE_TOTAL_PRICE = refrencerow.TOTAL_PRICE;
                $scope.referenceDataDisplay1[refrenkey].REFERENCE_CALC_UNIT_PRICE = refrencerow.CALC_UNIT_PRICE;
                $scope.referenceDataDisplay1[refrenkey].CALC_TOTAL_PRICE = refrencerow.TOTAL_PRICE;
                $scope.referenceDataDisplay1[refrenkey].REFERENCE_REMARKS = refrencerow.REMARKS;
            });

            $scope.ChildSumOperations(0);

            $scope.showRefTab = true;

        }

        function setDataOnModal(rows, i) {
            debugger;
            var tempCopy = angular.copy($scope.childModelTemplate);
            $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
            $scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));
            setTimeout(function () {
                $("#products_" + i).data('kendoComboBox').dataSource.data([{ ItemCode: rows[i].ITEM_CODE, ItemDescription: rows[i].ITEM_EDESC, Type: "code" }]);
                console.log("value of productCode from kendo===================>>>>" + JSON.stringify($("#products_" + i).data('kendoComboBox').dataSource.data()[0].ItemCode));
                $scope.childModels[i].ITEM_CODE = $("#products_" + i).data('kendoComboBox').dataSource.data()[0].ItemCode;
            }, 0);
            var rowsObj = rows[i];

            var budgetModel = angular.copy($scope.BUDGET_MODAL[0]);
            var budgetChildModel = angular.copy($scope.BUDGET_CHILD_MODAL[0]);

            $scope.dynamicModalData.push($scope.getObjWithKeysFromOtherObj(budgetModel, rowsObj));
            if ($scope.dynamicModalData[i].BUDGET == undefined) {
                $scope.dynamicModalData[i].BUDGET = [];
                $scope.dynamicModalData[i].BUDGET.push(budgetChildModel);
            }

            var batchModel = angular.copy($scope.BATCH_MODAL[0]);
            var batchChildModel = angular.copy($scope.BATCH_CHILD_MODAL[0]);

            $scope.dynamicSerialTrackingModalData.push($scope.getObjWithKeysFromOtherObj(batchModel, rowsObj));
            if ($scope.dynamicSerialTrackingModalData[i].TRACK == undefined) {
                $scope.dynamicSerialTrackingModalData[i].TRACK = [];
                $scope.dynamicSerialTrackingModalData[i].TRACK.push(batchChildModel);
            }

            var batchtrackingmodel = angular.copy($scope.BATCH_TRACKING_MODEL[0]);
            $scope.dynamicBatchTrackingModalData.push($scope.getObjWithKeysFromOtherObj(batchtrackingmodel, rowsObj));

            var config = {
                async: false
            };

            var voucherNo = PrimaryColumnForTable($scope.DocumentName);

            var response = $http.get('/api/InventoryApi/GetDataForBudgetModal?VOUCHER_NO=' + rows[i][voucherNo], config);
            response.then(function (budgetResult) {
                if (budgetResult.data != "") {
                    for (var a = 0; a < budgetResult.data.length; a++) {
                        for (var b = 0; b < $scope.dynamicModalData.length; b++) {
                            var serialno = budgetResult.data[a].SERIAL_NO - 1;
                            $scope.dynamicModalData[b].BUDGET_FLAG = budgetResult.data[a].BUDGET_FLAG;
                            $scope.dynamicModalData[b].BUDGET[a] = $scope.getObjWithKeysFromOtherObj(budgetChildModel, budgetResult.data[a]);
                        }
                    }

                }

            });
            //for serial transaction data
            var serialresponse = $http.get('/api/InventoryApi/GetDataForBatchModal?VOUCHER_NO=' + rows[i][voucherNo], config);
            serialresponse.then(function (serialResult) {

                if (serialResult.data.length > 0) {
                    //for (var a = 0; a < serialResult.data.length; a++) {   
                    //    debugger;
                    //    for (var b = 0; b < $scope.dynamicSerialTrackingModalData.length; b++) {          
                    //        debugger;
                    //        if (serialResult.data[a].ITEM_CODE == $scope.dynamicSerialTrackingModalData[b].ITEM_CODE) {      
                    //            debugger;
                    //            var serialno = serialResult.data[a].SERIAL_NO - 1;
                    //            $scope.dynamicSerialTrackingModalData[b].TRACK[a] = $scope.getObjWithKeysFromOtherObj(batchChildModel, serialResult.data[a]);
                    //        }
                    //    }
                    //}
                    for (var b = 0; b < $scope.dynamicSerialTrackingModalData.length; b++) {

                        var iii = 0;
                        for (var a = 0; a < serialResult.data.length; a++) {

                            if (serialResult.data[a].ITEM_CODE == $scope.dynamicSerialTrackingModalData[b].ITEM_CODE) {

                                //var serialno = batchResult.data[a].SERIAL_NO - 1;

                                $scope.dynamicSerialTrackingModalData[b].TRACK[iii] = $scope.getObjWithKeysFromOtherObj(batchChildModel, serialResult.data[a]);
                                iii = iii + 1;
                            }
                        }
                    }
                }

            });
            //for batch tracking data
            var batchtrackingresponse = $http.get('/api/InventoryApi/GetDataForBatchTrackingModal?VOUCHER_NO=' + rows[i][voucherNo], config);
            batchtrackingresponse.then(function (batchTrackingResult) {

                if (batchTrackingResult.data.length > 0) {
                    for (var a = 0; a < batchTrackingResult.data.length; a++) {
                        $scope.dynamicBatchTrackingModalData[a] = $scope.getObjWithKeysFromOtherObj(batchtrackingmodel, batchTrackingResult.data[a]);
                    }
                }
            });
        }
        callback();
    };

    var formDetail = inventoryservice.getFormDetail_ByFormCode($scope.formcode, d1);
    $.when(d1).done(function (result) {
        $scope.formDetail = result.data;
        if ($scope.formDetail.length > 0) {
            debugger;
            $scope.DocumentName = $scope.formDetail[0].TABLE_NAME;
            $scope.companycode = $scope.formDetail[0].COMPANY_CODE;
            $scope.printCompanyInfo.companyName = $scope.formDetail[0].COMPANY_EDESC;
            $scope.printCompanyInfo.address = $scope.formDetail[0].ADDRESS;
            $scope.printCompanyInfo.formName = $scope.formDetail[0].FORM_EDESC;
            $scope.printCompanyInfo.phoneNo = $scope.formDetail[0].TELEPHONE;
            $scope.printCompanyInfo.email = $scope.formDetail[0].EMAIL;
            $scope.printCompanyInfo.tPinVatNo = $scope.formDetail[0].TPIN_VAT_NO;
            $scope.printCompanyInfo.formType = $scope.formDetail[0].FORM_TYPE;
        }
        var values = $scope.formDetail;
        //collection of Master elements
        angular.forEach(values,
            function (value, key) {
                if (value.MASTER_CHILD_FLAG == 'M' && value.DISPLAY_FLAG == 'Y') {
                    $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                    $scope.masterModelTemplate[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                }
                if (value.MASTER_CHILD_FLAG == 'M' && value.DISPLAY_FLAG == 'Y') {

                    this.push(value);
                    if (value['COLUMN_NAME'].indexOf('DATE') > -1) {
                        $scope.masterModelTemplate[value['COLUMN_NAME']] = $filter('date')(new Date(), 'dd-MMM-yyyy');
                    } else {
                        $scope.masterModelTemplate[value['COLUMN_NAME']] = "";
                    }
                    $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE;
                }
            },
            $scope.MasterFormElement);

        //collection of child elements.
        angular.forEach(values,
            function (value, key) {
                if (value.MASTER_CHILD_FLAG == 'C' && value.DISPLAY_FLAG == 'Y') {
                    this.push(value);
                    if (value['COLUMN_NAME'] === "ITEM_CODE") {
                        $rootScope.ITEM_CODE_DEFAULTVAL = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                    }
                }
            },
            $scope.ChildFormElement[0].element);

        //additional child element reservation.


        angular.forEach(values,
            function (value, key) {
                if (value.MASTER_CHILD_FLAG == 'C' && value.DISPLAY_FLAG == 'Y') {
                    this.push(value);
                    $scope.childModelTemplate[value['COLUMN_NAME']] = "";
                }
            },
            $scope.aditionalChildFormElement);

        var tempFn = function (result) {

            $scope.BUDGET_MODAL.push({ ACC_CODE: "", BUDGET_FLAG: "", BUDGET: [] });
            $scope.BUDGET_CHILD_MODAL.push({ SERIAL_NO: "", BUDGET_CODE: "", AMOUNT: "", NARRATION: "", ACC_CODE: "", QUANTITY: "" });

            $scope.BATCH_MODAL.push({ ITEM_CODE: "", ITEM_EDESC: "", MU_CODE: "", LOCATION_CODE: "", QUANTITY: "", TRACK: [] });
            $scope.BATCH_CHILD_MODAL.push({ SERIAL_NO: 1, TRACKING_SERIAL_NO: "" });


            $scope.BATCH_TRACKING_MODEL.push({ ITEM_CODE: "", ITEM_EDESC: "", MU_CODE: "", LOCATION_CODE: "", QUANTITY: "", BATCH_NO: "", EXPIRY_DATE: "", SERIAL_NO: 1 });

            var rows = result.data;
            if (rows.length > 0) {

                var imageurl = [];
                var imageslistcount = rows[0].IMAGES_LIST.length;
                $.each(rows[0].IMAGES_LIST, function (key, value) {
                    var filepath = value.DOCUMENT_FILE_NAME;
                    var path = filepath.replace(/[/]/g, '_');
                    imageurl.push(path);
                });
                if (imageurl.length > 0) {
                    for (var i = 0; i < imageurl.length; i++) {
                        var mockFile = {
                            name: rows[0].IMAGES_LIST[i].DOCUMENT_NAME,
                            size: 12345,
                            type: 'image/jpeg',
                            url: imageurl[i],
                            accepted: true,
                        };
                        if (i == 0) {
                            myInventoryDropzone.on("addedfile", function (file) {
                                caption = file.caption == undefined ? "" : file.caption;
                                file._captionLabel = Dropzone.createElement("<a class='fa fa-download dropzone-download' href='" + imageurl[i] + "' name='Download' class='dropzone_caption' download></a>");
                                file.previewElement.appendChild(file._captionLabel);
                            });
                        }
                        myInventoryDropzone.emit("addedfile", mockFile);
                        myInventoryDropzone.emit("thumbnail", mockFile, imageurl[i]);
                        myInventoryDropzone.emit('complete', mockFile);
                        myInventoryDropzone.files.push(mockFile);
                        $('.dz-details').find('img').addClass('sr-only');
                        $('.dz-remove').css("display", "block");
                    }
                }
                $scope.masterModels = {};
                var masterModel = angular.copy($scope.masterModelTemplate);
                var savedData = $scope.getObjWithKeysFromOtherObj(masterModel, result.data[0]);
                $scope.masterModels = savedData;


                //to solve problem in suppliercode binding for update purpose
                suppliercodeforupdate = $scope.masterModels.SUPPLIER_CODE;
                //

                if ($scope.OrderNo != undefined && $scope.OrderNo != "undefined") {
                    $scope.ChildFormElement = [];
                    $scope.childModels = [];
                    $scope.dynamicModalData = [];
                    $scope.dynamicSerialTrackingModalData = [];
                    $scope.dynamicBatchTrackingModalData = [];
                    $scope.newgenorderno = "";
                    $scope.save = "Update";
                    $scope.savecontinue = "Update & Continue";
                }
                for (var i = 0; i < rows.length; i++) {
                    setDataOnModal(rows, i);
                }

                $scope.ChildSumOperations(0);
                $scope.calculateChargeAmount1($scope.data, true);

            }
            else {
            }

            function setDataOnModal(rows, i) {

                var tempCopy = angular.copy($scope.childModelTemplate);
                $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
                $scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));
                setTimeout(function () {
                    //console.log("setDataModal=================>>>" + JSON.stringify(rows));
                    if ($scope.childModels[0].hasOwnProperty("ITEM_CODE")) {
                        $("#products_" + i).data('kendoComboBox').dataSource.data([{ ItemCode: rows[i].ITEM_CODE, ItemDescription: rows[i].ITEM_EDESC, Type: "code" }]);
                    }
                    if ($scope.masterModels.hasOwnProperty("CUSTOMER_CODE")) {
                        $("#customers").data('kendoComboBox').dataSource.data([{ CustomerCode: rows[0].CUSTOMER_CODE, CustomerName: rows[0].CUSTOMER_EDESC, Type: "code", REGD_OFFICE_EADDRESS: rows[0].REGD_OFFICE_EADDRESS, TPIN_VAT_NO: rows[0].TPIN_VAT_NO, TEL_MOBILE_NO1: rows[0].TEL_MOBILE_NO1, CUSTOMER_NDESC: rows[0].CUSTOMER_NDESC }]);
                    }
                    //if ($scope.masterModels.hasOwnProperty("FROM_LOCATION_CODE")) {
                    //    $("#location").data('kendoComboBox').dataSource.data([{ LocationCode: rows[0].FROM_LOCATION_CODE, LocationName: rows[0].FROM_LOCATION_EDESC, Type: "code" }]);
                    //}
                    //if ($scope.masterModels.hasOwnProperty("TO_LOCATION_CODE")) {
                    //    $("#tolocation").data('kendoComboBox').dataSource.data([{ LocationCode: rows[0].TO_LOCATION_CODE, LocationName: rows[0].TO_LOCATION_EDESC, Type: "code" }]);
                    //}
                }, 0);

                angular.forEach($scope.masterModels,
                    function (mvalue, mkey) {

                        if (mkey === "TO_LOCATION_CODE") {

                            var req = "/api/TemplateApi/GetLoactionNameByCode?locationcode=" + mvalue;
                            $http.get(req).then(function (results) {
                                debugger;
                                setTimeout(function () {
                                    debugger;
                                    $("#tolocation").data('kendoComboBox').dataSource.data([{ LocationCode: rows[0].TO_LOCATION_CODE, LocationName: results.data, Type: "code" }]);
                                }, 50);
                            });
                        }
                    });

                var rowsObj = rows[i];

                var budgetModel = angular.copy($scope.BUDGET_MODAL[0]);
                var budgetChildModel = angular.copy($scope.BUDGET_CHILD_MODAL[0]);

                $scope.dynamicModalData.push($scope.getObjWithKeysFromOtherObj(budgetModel, rowsObj));
                if ($scope.dynamicModalData[i].BUDGET == undefined) {
                    $scope.dynamicModalData[i].BUDGET = [];
                    $scope.dynamicModalData[i].BUDGET.push(budgetChildModel);
                }

                var batchModel = angular.copy($scope.BATCH_MODAL[0]);
                var batchChildModel = angular.copy($scope.BATCH_CHILD_MODAL[0]);

                $scope.dynamicSerialTrackingModalData.push($scope.getObjWithKeysFromOtherObj(batchModel, rowsObj));
                if ($scope.dynamicSerialTrackingModalData[i].TRACK == undefined) {
                    $scope.dynamicSerialTrackingModalData[i].TRACK = [];
                    $scope.dynamicSerialTrackingModalData[i].TRACK.push(batchChildModel);
                }

                var batchtrackingmodel = angular.copy($scope.BATCH_TRACKING_MODEL[0]);
                $scope.dynamicBatchTrackingModalData.push($scope.getObjWithKeysFromOtherObj(batchtrackingmodel, rowsObj));

                var config = {
                    async: false
                };

                var voucherNo = PrimaryColumnForTable($scope.DocumentName);

                var response = $http.get('/api/InventoryApi/GetDataForBudgetModal?VOUCHER_NO=' + rows[i][voucherNo], config);
                response.then(function (budgetResult) {
                    if (budgetResult.data.length > 0) {
                        for (var a = 0; a < budgetResult.data.length; a++) {
                            for (var b = 0; b < $scope.dynamicModalData.length; b++) {
                                var serialno = budgetResult.data[a].SERIAL_NO - 1;
                                $scope.dynamicModalData[b].BUDGET_FLAG = budgetResult.data[a].BUDGET_FLAG;
                                $scope.dynamicModalData[b].BUDGET[a] = $scope.getObjWithKeysFromOtherObj(budgetChildModel, budgetResult.data[a]);
                            }
                        }

                    }

                });
                //for serial transaction data
                var serialresponse = $http.get('/api/InventoryApi/GetDataForBatchModal?VOUCHER_NO=' + rows[i][voucherNo], config);
                serialresponse.then(function (serialResult) {

                    if (serialResult.data.length > 0) {
                        //for (var a = 0; a < serialResult.data.length; a++) {   
                        //    debugger;
                        //    for (var b = 0; b < $scope.dynamicSerialTrackingModalData.length; b++) {          
                        //        debugger;
                        //        if (serialResult.data[a].ITEM_CODE == $scope.dynamicSerialTrackingModalData[b].ITEM_CODE) {      
                        //            debugger;
                        //            var serialno = serialResult.data[a].SERIAL_NO - 1;
                        //            $scope.dynamicSerialTrackingModalData[b].TRACK[a] = $scope.getObjWithKeysFromOtherObj(batchChildModel, serialResult.data[a]);
                        //        }
                        //    }
                        //}
                        for (var b = 0; b < $scope.dynamicSerialTrackingModalData.length; b++) {

                            var iii = 0;
                            for (var a = 0; a < serialResult.data.length; a++) {

                                if (serialResult.data[a].ITEM_CODE == $scope.dynamicSerialTrackingModalData[b].ITEM_CODE) {

                                    //var serialno = batchResult.data[a].SERIAL_NO - 1;

                                    $scope.dynamicSerialTrackingModalData[b].TRACK[iii] = $scope.getObjWithKeysFromOtherObj(batchChildModel, serialResult.data[a]);
                                    iii = iii + 1;
                                }
                            }
                        }
                    }

                });
                //for batch tracking data
                var batchtrackingresponse = $http.get('/api/InventoryApi/GetDataForBatchTrackingModal?VOUCHER_NO=' + rows[i][voucherNo], config);
                batchtrackingresponse.then(function (batchTrackingResult) {

                    if (batchTrackingResult.data.length > 0) {
                        for (var a = 0; a < batchTrackingResult.data.length; a++) {
                            $scope.dynamicBatchTrackingModalData[a] = $scope.getObjWithKeysFromOtherObj(batchtrackingmodel, batchTrackingResult.data[a]);
                        }
                    }
                });
            }
            //$scope.muwiseQty();
            //$scope.someDateFn();
        };
        if ($scope.masterChildData === null) {
            $scope.masterModelDataFn(tempFn);
        } else {
            tempFn($scope.masterChildData);
        }

        // check master transaction type .. if cr/dr disable child element
        $scope.check = function () {
            var MasterFormlen = $scope.MasterFormElement.length;
            $.each($scope.MasterFormElement, function (key, value) {
                var mastercolname = $scope.MasterFormElement[key].COLUMN_NAME;
                if (mastercolname == "MASTER_TRANSACTION_TYPE") {
                    if ($scope.MasterFormElement[key].DEFA_VALUE != null);
                    $scope.checktransaction = true;
                    return;
                };
            });

        }


        $scope.check();


        var date = new Date();
        $scope.todaydate = $filter('date')(new Date(), 'dd-MMM-yyyy');
        $scope.someFn();
        elements = $scope.MasterFormElement;

        $scope.masterModels;
        $scope.childModels;
        //dt.resolve($scope.MasterFormElement);
        var dt = new Promise(function (resolve, reject) {
            // Resolve the promise with $scope.MasterFormElement or any other data
            resolve($scope.MasterFormElement);
        });

    });
    $scope.GrandtotalCalution = function () {

        var sum = 0;
        angular.forEach($scope.childModels, function (value, key) {

            if ($scope.childModels[key].hasOwnProperty("TOTAL_PRICE")) {
                if (typeof value.TOTAL_PRICE !== 'undefined' && value.TOTAL_PRICE !== null && value.TOTAL_PRICE !== "") {
                    sum = parseFloat(sum) + (parseFloat(value.TOTAL_PRICE));
                }
            }

            else {
                if ($scope.childModels[key].hasOwnProperty("CALC_TOTAL_PRICE")) {
                    if (typeof value.CALC_TOTAL_PRICE !== 'undefined' && value.CALC_TOTAL_PRICE !== null && value.CALC_TOTAL_PRICE !== "") {
                        sum = parseFloat(sum) + (parseFloat(value.CALC_TOTAL_PRICE));
                    }
                }
            }

        });

        $scope.summary.grandTotal = (parseFloat(sum)).toFixed(2);
        if ($scope.data.length == 0 || $scope.data.length == "undefined") {
            $scope.adtotal = $scope.summary.grandTotal;
        }
        $scope.setTotal();
    }
    $scope.cityDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllCityDtlsByFilter"
            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        },
    };
    $scope.cityOption = {
        dataSource: $scope.cityDataSource,
        dataTextField: 'CITY_EDESC',
        dataValueField: 'CITY_CODE',
        filter: "contains",
        select: function (e) {

            if (e.dataItem !== undefined) {

            }

        },
        dataBound: function (e) {

        },
        change: function (e) {


        }
    };
    $scope.monthSOshipingdetails = {
        open: function () {

            //var calendar = this.dateView.calendar;

            //calendar.wrapper.width(this.wrapper.width() + 10);
        },
        change: function () {

            //$scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        dataBound: function (e) {

        },
        format: "dd-MMM-yyyy",

        // specifies that DateInput is used for masking the input element
        dateInput: true
    };
    $scope.vechDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllVechileDtlsByFilter"
            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        },
    };
    $scope.vechOption = {
        dataSource: $scope.vechDataSource,
        dataTextField: 'VEHICLE_EDESC',
        dataValueField: 'VEHICLE_CODE',
        filter: "contains",
        select: function (e) {

            if (e.dataItem !== undefined) {
                $scope.SDModel.VEHICLE_OWNER_NAME = e.dataItem.VEHICLE_OWNER_NAME;
                $scope.SDModel.VEHICLE_OWNER_NO = e.dataItem.VEHICLE_OWNER_NO;
                $scope.SDModel.DRIVER_NAME = e.dataItem.DRIVER_NAME;
                $scope.SDModel.DRIVER_LICENCE_NO = e.dataItem.DRIVER_LICENCE_NO;
                $scope.SDModel.DRIVER_MOBILE_NO = e.dataItem.DRIVER_MOBILE_NO;
            }

        },
        dataBound: function (e) {

        },
        change: function (e) {


        }
    };
    $scope.transporterDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllTransporterDtlsByFilter"
            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        },
    };
    $scope.transporterOption = {
        dataSource: $scope.transporterDataSource,
        dataTextField: 'TRANSPORTER_EDESC',
        dataValueField: 'TRANSPORTER_CODE',
        filter: "contains",
        select: function (e) {

            if (e.dataItem !== undefined) {

            }

        },
        dataBound: function (e) {

        },
        change: function (e) {


        }
    };
    $scope.ChildCalcSum = function (key) {

        $scope.Childcolqtyratevalidation(key);
        var calc_quantity = $scope.childModels[key].CALC_QUANTITY;
        var calc_rate = $scope.childModels[key].CALC_UNIT_PRICE;

        if (calc_rate !== null && calc_rate !== "" && calc_rate !== undefined) {
            $scope.childModels[key].CALC_UNIT_PRICE = parseFloat(calc_rate.toFixed(2));
        }


        if (calc_rate === undefined || calc_quantity === undefined || calc_rate === 0 || calc_rate === "" || calc_quantity === 0 || calc_quantity === "" || calc_rate === null || calc_quantity === null) {
            $scope.childModels[key].CALC_TOTAL_PRICE = 0;
        }
        else {
            var total_price = calc_rate * calc_quantity;
            $scope.childModels[key].CALC_TOTAL_PRICE = parseFloat(total_price.toFixed(2));
        }

        $scope.GrandtotalCalution();

    }

    $scope.ChildSumOperations = function (keys) {

        $scope.Childcolqtyratevalidation(keys);
        var child_rate = $scope.childModels[keys].UNIT_PRICE;
        var quantity = $scope.childModels[keys].QUANTITY;
        if (child_rate != null && child_rate != "" && child_rate !== undefined) {
            $scope.childModels[keys].UNIT_PRICE = parseFloat(child_rate.toFixed(2));
        }



        if (quantity !== undefined || quantity !== null || quantity !== "") {
            $scope.childModels[keys].CALC_QUANTITY = quantity;
        }
        if (child_rate !== undefined && child_rate !== null && child_rate !== "") {
            $scope.childModels[keys].CALC_UNIT_PRICE = child_rate;
        }
        if (child_rate === undefined || quantity === undefined) {

        }
        if (child_rate === 0 || quantity === 0) {

            $scope.childModels[keys].TOTAL_PRICE = 0;
        }
        if (child_rate === "" || quantity === "") {


        }
        if (child_rate === null || quantity === null) {


        }
        else {
            if (child_rate != null && child_rate !== "" && child_rate !== undefined) {
                if (quantity != null && quantity != "" && quantity !== undefined) {
                    var total_price = child_rate * quantity;

                    $scope.childModels[keys].TOTAL_PRICE = parseFloat(total_price.toFixed(2));
                    $scope.childModels[keys].CALC_TOTAL_PRICE = $scope.childModels[keys].TOTAL_PRICE;
                }
            }




        }


        $scope.GrandtotalCalution();
    }
    $scope.Childcolqtyratevalidation = function (keys) {

        var child_rate = $scope.childModels[keys].UNIT_PRICE;
        var quantity = $scope.childModels[keys].QUANTITY;
        var calc_quantity = $scope.childModels[keys].CALC_QUANTITY;
        var calc_rate = $scope.childModels[keys].CALC_UNIT_PRICE;
        //rate validation start
        if (child_rate === undefined) {
            $(".UNIT_PRICE_" + keys).parent().css({ "border": "solid 1px red" });
        }
        else {
            $(".UNIT_PRICE_" + keys).parent().css({ "border": "none" });
            $(".CALC_UNIT_PRICE_" + keys).parent().css({ "border": "none" });
        }
        //validation end

        //quantity validation start
        if (quantity === undefined) {
            $(".QUANTITY_" + keys).parent().css({ "border": "solid 1px red" });
        }
        else {
            $(".QUANTITY_" + keys).parent().css({ "border": "none" });
            $(".CALC_QUANTITY_" + keys).parent().css({ "border": "none" });
        }

        if (child_rate === undefined && quantity === undefined) {
            $scope.childModels[keys].CALC_UNIT_PRICE = "";
            $scope.childModels[keys].CALC_QUANTITY = "";
            return;
        }
        //validation end
        if (calc_rate === undefined) {

            $(".CALC_UNIT_PRICE_" + keys).parent().css({ "border": "solid 1px red" });
            return;
        }
        else {
            $(".CALC_UNIT_PRICE_" + keys).parent().css({ "border": "none" });
        }
        if (calc_quantity === undefined) {
            $(".CALC_QUANTITY_" + keys).parent().css({ "border": "solid 1px red" });
            return;
        }
        else {
            $(".CALC_QUANTITY_" + keys).parent().css({ "border": "none" });
        }
    };

    $scope.someFn = function () {

        angular.forEach($scope.MasterFormElement, function (value, key) {
            if (value.MASTER_CHILD_FLAG == 'M' && value.DISPLAY_FLAG == 'Y') {
                $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                if (value['COLUMN_NAME'].indexOf('DATE') > -1) {
                    $scope.masterModels[value['COLUMN_NAME']] = $filter('date')(new Date(), 'dd-MMM-yyyy');
                }
            }
        });
        var cml = $scope.childModels.length;
        var sl = parseFloat(cml) - 1;
        $scope.ChildFormElement.splice(0, sl);
        $scope.childModels.splice(0, sl);
        angular.forEach($scope.ChildFormElement[0].element, function (value, key) {

            if ($scope.childModels.length == 0) {
                $scope.childModels.push({});
            }
            if (value.MASTER_CHILD_FLAG == 'C' && value.DISPLAY_FLAG == 'Y') {

                $scope.childModels[0][value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
            }
        });
        var response = $http.get('/api/TemplateApi/GetNewOrderNo?companycode=' + $scope.companycode + '&formcode=' + $scope.formcode + '&currentdate=' + $scope.todaydate + '&tablename=' + $scope.DocumentName);
        response.then(function (res) {
            if (res.data != "0") {
                if ($scope.save !== "Update") {
                    $scope.newgenorderno = res.data;
                }
                var primarycolumn = PrimaryColumnForTable($scope.DocumentName);
                //$scope.masterModels[primarycolumn] = res.data;
                $.each($scope.MasterFormElement, function (key, value) {

                    if (value['COLUMN_NAME'].indexOf('DATE') > -1) {

                        $scope.masterModels[value.COLUMN_NAME] = $filter('date')(new Date(), 'dd-MMM-yyyy');
                        if (value['COLUMN_HEADER'].indexOf('Miti') > -1) {

                            if (value.DEFA_VALUE == null) {

                                var englishdate = $filter('date')(new Date(), 'yyyy-MM-dd');
                                var nepalidate = AD2BS(englishdate);
                                $("#nepaliDate5").val(nepalidate);
                            }
                            else {

                                var englishdate = value.DEFA_VALUE;
                                var nepalidate = AD2BS(englishdate);
                                $("#nepaliDate5").val(nepalidate);
                            }
                        }
                    }
                });

            }
        });
    }
    $scope.setTotal = function () {

        $scope.units = [];
        $scope.totalQty = 0;
        var totalQty = 0;
        $scope.childModels.forEach(function (item) {
            var qtySum = 0;
            $scope.childModels.forEach(function (it) {
                if (item.MU_CODE == it.MU_CODE) {
                    if (it.QUANTITY !== undefined) {
                        qtySum += it.QUANTITY;
                    }
                }
            });

            $scope.units.push({ mu_name: item.MU_CODE, mu_code_value: (parseFloat(qtySum)).toFixed(4) });
            if (item.QUANTITY !== undefined) {
                totalQty += item.QUANTITY;
                $scope.totalQty = (parseFloat(totalQty)).toFixed(4);
            }
        });
        $scope.units = _.uniq($scope.units, JSON.stringify);
    }
    $scope.SetMuCode = function (key, kendoEvent) {

        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.childModels[key]["MU_CODE"] = "";
            //$(kendoEvent.sender.element[0]).parent().parent().parent().css({ "border": "solid 1px red" });
            $(kendoEvent.sender.element[0]).parent().parent().parent().addClass('borderRed');
            $(kendoEvent.sender.input[0]).val("");
        }
        else {
            //$(kendoEvent.sender.element[0]).parent().parent().parent().css({ "border": "none" });
            $(kendoEvent.sender.element[0]).parent().parent().parent().removeClass('borderRed');
            var mucode = kendoEvent.sender.dataItem().ItemUnit;
            $scope.childModels[key]["MU_CODE"] = mucode;

            if ($scope.serial_tracking_flag == "Y") {

                var icrreq = "/api/TemplateApi/getItemCountResult?code=" + kendoEvent.sender.dataItem().ItemCode;
                $http.get(icrreq).then(function (icrreqresults) {
                    debugger
                    if (icrreqresults.data == true) {

                        $scope.dynamicSerialTrackingModalData[key].ITEM_CODE = kendoEvent.sender.dataItem().ItemCode;
                        $scope.dynamicSerialTrackingModalData[key].ITEM_EDESC = kendoEvent.sender.dataItem().ItemDescription;
                        $scope.dynamicSerialTrackingModalData[key].MU_CODE = mucode;
                        if ($scope.childModels[0].hasOwnProperty("FROM_LOCATION_CODE")) {
                            $scope.dynamicSerialTrackingModalData[key].LOCATION_CODE = $scope.childModels[key]["FROM_LOCATION_CODE"];
                        }
                        if ($scope.childModels[0].hasOwnProperty("TO_LOCATION_CODE")) {
                            $scope.dynamicSerialTrackingModalData[key].LOCATION_CODE = $scope.childModels[key]["TO_LOCATION_CODE"];
                        }
                        $(".serialtrackFlag_" + key).modal('toggle');
                    }

                });

            }
            if ($scope.batch_tracking_flag == "Y") {

                var icrreq = "/api/TemplateApi/getBatchItemCountResult?code=" + kendoEvent.sender.dataItem().ItemCode;
                $http.get(icrreq).then(function (icrreqresults) {
                    debugger
                    if (icrreqresults.data == true) {

                        $scope.dynamicBatchTrackingModalData[key].ITEM_CODE = kendoEvent.sender.dataItem().ItemCode;
                        $scope.dynamicBatchTrackingModalData[key].ITEM_EDESC = kendoEvent.sender.dataItem().ItemDescription;
                        $scope.dynamicBatchTrackingModalData[key].MU_CODE = mucode;
                        if ($scope.childModels[0].hasOwnProperty("FROM_LOCATION_CODE")) {
                            $scope.dynamicBatchTrackingModalData[key].LOCATION_CODE = $scope.childModels[key]["FROM_LOCATION_CODE"];
                        }

                        $(".batchtrackflag_" + key).modal('toggle');
                    }

                });


            }



        }
        //$scope.muwiseQty();
    }
    $scope.loadchildelements = function (index, itqty) {

        $scope.dynamicSerialTrackingModalData[index].TRACK = [{
            SERIAL_NO: 1,
            TRACKING_SERIAL_NO: "",
        }];

        for (var t = 1; t < itqty; t++) {

            $scope.dynamicSerialTrackingModalData[index].TRACK.push({
                SERIAL_NO: index + 1,
                TRACKING_SERIAL_NO: ""
            });
        }

    };
    // check validation
    $scope.checkValidation = function (kendoEvent) {
        if (kendoEvent.sender.dataItem() == undefined) {
            //$(kendoEvent.sender.element[0]).parent().parent().parent().css({ "border": "solid 1px red" });
            $(kendoEvent.sender.element[0]).parent().parent().parent().addClass('borderRed');
            $(kendoEvent.sender.input[0]).val("");
            displayPopupNotification("Code is required", "warning");

        }
        else {
            //$(kendoEvent.sender.element[0]).parent().parent().parent().css({ "border": "none" });
            $(kendoEvent.sender.element[0]).parent().parent().parent().removeClass('borderRed');
        }
    }


    $scope.setOnFlagFocus = function () {

        $(".budgetFlag_" + index).on('hidden.bs.modal', function () {
            $($(".flag")[index]).focus();
        })
    }
    $scope.setNextFlagFocus = function () {
        $(".budgetFlag_" + index).on('hidden.bs.modal', function () {
            $(".flag").closest('td').next().find('input').focus();
        })
    }

    $scope.ConvertEngToNep = function () {

        var englishdateId = $('.englishdate').attr('id');
        var englishdate = $("#" + englishdateId).val();
        var FormattedEngDate = moment(englishdate).format('YYYY-MM-DD');
        var nepalidate = AD2BS(FormattedEngDate);
        var splitteddate = englishdateId.split(/_(.+)/)[1];
        $("#nepaliDate5_" + splitteddate).val(nepalidate);

    };



    $scope.ConvertNepToEng = function ($event) {
        var date = BS2AD($("#nepaliDate5").val());
        $("#englishdatedocument").val($filter('date')(date, "dd-MMM-yyyy"));
        $('#nepaliDate5').trigger('change')
    };

    $scope.ConvertEngToNepang = function (data) {
        $("#nepaliDate5").val(AD2BS(data));
    };

    var index = 0;
    var accCode = "";
    var popupAccess = false;

    $scope.add_child_element = function (e) {

        //$scope.BUDGET_CENTER.BUDGET.ID = $scope.dynamicModalData.length;
        var budgetserialno = $scope.dynamicModalData.length + 1;
        $scope.dynamicModalData.push({
            LOCATION_CODE: "",
            BUDGET_FLAG: "",
            BUDGET: [{
                SERIAL_NO: budgetserialno,
                BUDGET_CODE: "",
                QUANTITY: "",
                ACC_CODE: ""
            }]
        });

        $scope.dynamicSubLedgerModalData.push({
            ACC_CODE: 0,
            TRANSACTION_TYPE: "",
            VOUCHER_AMOUNT: "",
            SUBLEDGER_AMOUNT: "",
            REMAINING_AMOUNT: "",
            SUBLEDGER: [{
                SUB_CODE: "",
                SUB_EDESC: "",
                AMOUNT: "",
                PARTICULARS: "",
                REFRENCE: ""
            }]
        });
        $scope.dynamicSerialTrackingModalData.push({
            ITEM_CODE: "",
            ITEM_EDESC: "",
            MU_CODE: "",
            LOCATION_CODE: "",
            QUANTITY: "",
            TRACK: [{
                SERIAL_NO: $scope.dynamicSerialTrackingModalData.length + 1,
                TRACKING_SERIAL_NO: ""
            }]
        });
        $scope.dynamicBatchTrackingModalData.push({
            ITEM_CODE: "",
            ITEM_EDESC: "",
            MU_CODE: "",
            LOCATION_CODE: "",
            QUANTITY: "",
            BATCH_NO: "",
            EXPIRY_DATE: "",
            SERIAL_NO: $scope.dynamicBatchTrackingModalData.length + 1

        });
        $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement, additionalElements: $scope.BUDGET_CENTER });
        $scope.childModels.push(angular.copy($scope.childModelTemplate));

        var childLen = $scope.childModels.length - 1;

        $.each($scope.ChildFormElement[0].element, function (childkey, childelementvalue) {
            if (childelementvalue.DEFA_VALUE != null)
                $scope.childModels[childLen][childelementvalue.COLUMN_NAME] = childelementvalue.DEFA_VALUE;
        });

    }

    $scope.add_childSubledger_element = function (e) {

        $scope.dynamicSubLedgerModalData[index].SUBLEDGER.push({
            SUB_CODE: "",
            SUB_EDESC: "",
            AMOUNT: "",
            PARTICULARS: "",
            REFRENCE: ""
        });

    }
    $scope.add_childSerialTracking_element = function (e) {

        $scope.dynamicSerialTrackingModalData[index].TRACK.push({
            SERIAL_NO: index + 1,
            TRACKING_SERIAL_NO: ""
        });
        $scope.dynamicSerialTrackingModalData[index].QUANTITY = e + 2;
    }
    $scope.add_childbudgetflag_element = function (e) {
        var totalQtyOFBudgetFlag = 0;
        $.each($scope.dynamicModalData[index].BUDGET, function (it, val) {
            totalQtyOFBudgetFlag += val.QUANTITY;
        })
        if (totalQtyOFBudgetFlag < $scope.childModels[index].CALC_QUANTITY) {
            $scope.dynamicModalData[index].BUDGET.push({
                SERIAL_NO: index + 1,
                BUDGET_CODE: "",
                QUANTITY: $scope.childModels[index].CALC_QUANTITY - totalQtyOFBudgetFlag,
                ACC_CODE: ""
            });
        }

    }

    $scope.calQtyBudgetFlag = function ($this, $index) {

        var totalQtyOFBudgetFlag = 0;
        $.each($scope.dynamicModalData[index].BUDGET, function (it, val) {
            totalQtyOFBudgetFlag += val.QUANTITY;
        })
        //if ($this.QUANTITY == null || $this.QUANTITY === 0)
        //{
        //    $this.QUANTITY = $scope.childModels[index].CALC_QUANTITY - totalQtyOFBudgetFlag;
        //    return;
        //}

        if (totalQtyOFBudgetFlag > $scope.childModels[index].CALC_QUANTITY) {
            $scope.dynamicModalData[index].BUDGET[$index].QUANTITY = "";
        }
        //else if (totalQtyOFBudgetFlag < $scope.childModels[index].CALC_QUANTITY) {
        //    $scope.add_childbudgetflag_element("");
        //}
    }

    $scope.remove_childSubledger_element = function (key, index, VOUCHER_AMOUNT) {
        if ($scope.dynamicSubLedgerModalData[key].SUBLEDGER.length > 1) {
            $scope.dynamicSubLedgerModalData[key].SUBLEDGER.splice(index, 1);
            $scope.Change(index, VOUCHER_AMOUNT);
        }
    }

    //On Voucher Amount Change
    $scope.ChangeVoucherAmount = function (VOUCHER_AMOUNT) {
        $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT = $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT;
        $scope.dynamicSubLedgerModalData[index].REMAINING_AMOUNT = VOUCHER_AMOUNT - $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT;
    }

    //On SubLedger Amount Change
    $scope.Change = function (Index, VOUCHER_AMOUNT) {

        var SubLength = $scope.dynamicSubLedgerModalData[index].SUBLEDGER.length;
        var subledgeramount = 0;
        if (VOUCHER_AMOUNT == undefined) {
            VOUCHER_AMOUNT = 0;
            $scope.dynamicSubLedgerModalData[index].VOUCHER_AMOUNT = 0;
        }
        else {

            $scope.dynamicSubLedgerModalData[index].VOUCHER_AMOUNT = VOUCHER_AMOUNT;
        }
        for (var i = 0; i < SubLength; i++) {

            var amt = $scope.dynamicSubLedgerModalData[index].SUBLEDGER[i].AMOUNT;
            if (amt != "") {
                var sa = parseFloat($scope.dynamicSubLedgerModalData[index].SUBLEDGER[i].AMOUNT);
                if (isNaN(sa)) {
                    sa = 0;
                }
                subledgeramount = subledgeramount + sa;
            }
            else {
                subledgeramount = subledgeramount;
            }

        }
        $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT = subledgeramount;
        $scope.dynamicSubLedgerModalData[index].REMAINING_AMOUNT = VOUCHER_AMOUNT - subledgeramount;

        if ($scope.dynamicSubLedgerModalData[index].REMAINING_AMOUNT < 0) {
            $('.subledgeramounts').addClass("borderred");
        }
        else {
            $scope.errorshow = "";
            $('.subledgeramounts').removeClass("borderred");
        }
    }

    //SubLedger Ok 
    $scope.SubLedger_Ok = function (index, e) {

        if ($scope.dynamicSubLedgerModalData[index].REMAINING_AMOUNT < 0) {
            $scope.errorshow = "Remaining Amount Cannot Be Negative.";
            e.preventDefault();
            e.stopPropagation();
            return;
        }
        $scope.accsum(index);
        $(".drcr").focus();
    }

    //SubLedger Cancel 
    $scope.SubLedger_Cancel = function () {
        $scope.errorshow = "";
        $scope.dynamicSubLedgerModalData[index] = [];
        //$scope.remove_child_element(index);
        $scope.childModels[index].AMOUNT = "";
        $scope.accsum(index);
        $(".accCodeAutoComplete").focus();
    }

    $scope.remove_childbudgetflag_element = function (key, index) {

        if ($scope.dynamicModalData[key].BUDGET.length > 1) {
            $scope.dynamicModalData[key].BUDGET.splice(index, 1);
        }
    }
    $scope.remove_childSerialTracking_element = function (key, index) {

        if ($scope.dynamicSerialTrackingModalData[key].TRACK.length > 1) {
            $scope.dynamicSerialTrackingModalData[key].TRACK.splice(index, 1);
            $scope.dynamicSerialTrackingModalData[key].QUANTITY = index;
        }
    }
    $scope.SerialTrackingOK = function (key) {

        $scope.childModels[key].QUANTITY = $scope.dynamicSerialTrackingModalData[key].QUANTITY;

    }
    // remove child row.
    $scope.remove_child_element = function (index) {

        if ($scope.ChildFormElement.length > 1) {
            $scope.ChildFormElement.splice(index, 1);
            $scope.childModels.splice(index, 1); // }
            $scope.dynamicModalData.splice(index, 1);
            $scope.dynamicSubLedgerModalData.splice(index, 1);
            $scope.dynamicSerialTrackingModalData.splice(index, 1);
            $scope.GrandtotalCalution();
            $scope.setTotal();
            //var sum = 0;
            //$.each($scope.childModels, function (key, value) {
            //    value.TOTAL_PRICE = parseFloat((value.QUANTITY * value.UNIT_PRICE).toFixed(2));
            //    sum += value.TOTAL_PRICE;
            //    $scope.summary.grandTotal = parseFloat(sum.toFixed(2));
            //    $scope.setTotal();

            //});
        }
    }

    var formSetup = inventoryservice.getFormSetup_ByFormCode($scope.formcode, d2);

    $.when(d2).done(function (result) {

        $scope.formSetup = result.data;
        $scope.FormName = $scope.formSetup[0].FORM_EDESC;

        if ($scope.formSetup.length > 0) {
            $scope.ModuleCode = $scope.formSetup[0].MODULE_CODE;
        }
    });

    //Division
    $scope.FaDivisionSetupDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetdivisionListByFilter",
            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        },
    };
    $scope.divisionCodeAutocomplete = {
        dataSource: $scope.FaDivisionSetupDataSource,
        dataTextField: 'DIVISION_EDESC',
        dataValueField: 'DIVISION_CODE',
    };
    //get customer
    //var purl;
    //if ($scope.RefTableName == 'SA_SALES_ORDER') {
    //    purl = "GetAllCustomerSetupByFilter";
    //}
    //else if ($scope.RefTableName == '') {
    //    purl = "GetAllSupplierForReferenceByFilter";
    //}
    $scope.customerDataSource = {

        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllSupplierForReferenceByFilter",
                //url: "/api/TemplateApi/" + purl,

            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        }
    };
    $scope.customerCodeOption = {
        dataSource: $scope.customerDataSource,
        dataTextField: 'CustomerName',
        dataValueField: 'CustomerCode',
        select: function (e) {

        },
        dataBound: function (e) {

        },
        change: function (e) {


        }
    }
    $scope.employeeDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllEmployeeListByFilter",

            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        }
    };

    $scope.monthSelectorOptions = {
        open: function () {
            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() + 100);
        },
        change: function () {
            var date = new Date();
            date.setDate(date.getDate() - $scope.formBackDays);
            var minDate = dateSet(date);
            var maxDate = dateSet(new Date());
            var selecteddate = dateSet(this.value());
            if ((selecteddate > maxDate) || (selecteddate < minDate)) {
                alert("Selected date not available");
                $("#englishdatedocument").focus();
                var months = ["jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec"];
                var curDate = new Date();
                curDate = curDate.getDate() + "-" + months[curDate.getMonth()] + "-" + curDate.getFullYear();
                $("#englishdatedocument").val(curDate);
            }
            $scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",

        // specifies that DateInput is used for masking the input element
        dateInput: true
    };
    $scope.monthSelectorOptionsSingle = {
        open: function () {
            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",

        // specifies that DateInput is used for masking the input element
        dateInput: true
    };




    $scope.supplierDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllSupplierListByFilter",

            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        }
    };
    $scope.supplierCodeOption = {
        dataSource: $scope.supplierDataSource,
        dataTextField: 'SUPPLIER_EDESC',
        dataValueField: 'SUPPLIER_CODE',

    }
    //$scope.CountVoucherTotal = function () {

    //    var tablename = $scope.MasterFormElement[0].TABLE_NAME;
    //    var response = inventoryservice.GetVouchersCount($routeParams.formcode, tablename);
    //    
    //    response.then(function (res) {

    //        if (res.data != "0") {

    //            $scope.VoucherCount = res.data;
    //        }
    //    });

    //}

    $scope.currencyDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetCurrencyListByFlter",

            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        }
    };
    $scope.employeeCodeOption = {
        dataSource: $scope.employeeDataSource,
        dataTextField: 'EMPLOYEE_EDESC',
        dataValueField: 'EMPLOYEE_CODE',

    }
    $scope.currencyCodeOption = {
        dataSource: $scope.currencyDataSource,
        dataTextField: 'CURRENCY_EDESC',
        dataValueField: 'CURRENCY_CODE',

    }

    $scope.accountCodeDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllAccountSetupByFilter",
            },
            parameterMap: function (data, action) {

                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        //newParams = {
                        //    filter: data.filter.filters[0].value
                        //};
                        //return newParams;
                        if (data.filter.filters[0].value != "") {
                            newParams = {
                                filter: data.filter.filters[0].value
                            };
                            return newParams;
                        }
                        else {
                            newParams = {
                                filter: "!@$"
                            };
                            return newParams;
                        }
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        },
    };
    $scope.accountCodeDataSourceForBud = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllAccountForBud",
            },
            parameterMap: function (data, action) {

                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        //newParams = {
                        //    filter: data.filter.filters[0].value
                        //};
                        //return newParams;
                        if (data.filter.filters[0].value != "") {
                            newParams = {
                                filter: data.filter.filters[0].value
                            };
                            return newParams;
                        }
                        else {
                            newParams = {
                                filter: "!@$"
                            };
                            return newParams;
                        }
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        },
    };

    //function locationOnSelect(e) {
    //    
    //    var dataItem = this.dataItem(e.item.index());
    //    $scope.locationName = dataItem.LocationName;
    //    $scope.locationCode = dataItem.LocationCode;
    //    locationCode = dataItem.LocationCode;
    //    index = parseInt(this.element.closest('td').find('input')[0].value);
    //    if (locationCode === $scope.dynamicSubLedgerModalData[index].LocationCode) {
    //        $scope.dynamicModalData[index] = $scope.dynamicModalData[index];
    //    } else {
    //        $scope.dynamicModalData[index].BUDGET = [{
    //            BUDGET_VAL: "",
    //            QUANTITY: "",
    //            ACC_CODE: ""
    //        }];
    //    }

    //    $scope.childModels[index][TRANSACTION_TYPE] = $scope.childModels[index].TRANSACTION_TYPE;
    //    $scope.transactiontype = $scope.childModels[index].TRANSACTION_TYPE;

    //    var response = $http.get('/api/TemplateApi/checkBudgetFlagByLocationCode?locationCode=' + locationCode);
    //    response.then(function (res) {
    //        if (res.data != "0") {
    //            popupAccess = true;
    //            $scope.popUp(index);
    //        }
    //    });
    //}



    //$scope.accountCodeAutocomplete = {
    //    dataSource: $scope.accountCodeDataSource,
    //    dataTextField: 'ACC_EDESC',
    //    dataValueField: 'ACC_CODE',

    //    select: function (e) {
    //        var dataItem = this.dataItem(e.item.index());
    //        $scope.accName = dataItem.ACC_EDESC;
    //        $scope.accCode = dataItem.ACC_CODE;
    //        accCode = dataItem.ACC_CODE;
    //        index = parseInt(this.element.closest('td').find('input')[0].value);
    //        if (accCode === $scope.dynamicSubLedgerModalData[index].ACC_CODE) {
    //            $scope.dynamicSubLedgerModalData[index] = $scope.dynamicSubLedgerModalData[index];
    //            $scope.dynamicModalData[index] = $scope.dynamicModalData[index];
    //        } else {

    //            $scope.dynamicSubLedgerModalData[index].SUBLEDGER = [{
    //                SUB_CODE: "",
    //                SUB_EDESC: "",
    //                AMOUNT: "",
    //                PARTICULARS: "",
    //                REFRENCE: ""
    //            }];
    //            $scope.dynamicModalData[index].BUDGET = [{
    //                BUDGET_VAL: "",
    //                QUANTITY: "",
    //                ACC_CODE: ""
    //            }];
    //        }

    //        $scope.childModels[index][TRANSACTION_TYPE] = $scope.childModels[index].TRANSACTION_TYPE;
    //        $scope.transactiontype = $scope.childModels[index].TRANSACTION_TYPE;
    //        $scope.dynamicSubLedgerModalData[index].ACC_CODE = accCode;

    //        var response = $http.get('/api/TemplateApi/getSubledgerCodeByAccCode?accCode=' + accCode);
    //        response.then(function (res) {
    //            if (res.data != "0") {
    //                popupAccess = true;
    //                $scope.popUp(index);
    //            }
    //        });
    //        $($(".subledgersecond")[2]).data('kendoComboBox').dataSource.read();
    //        $($(".subledgerfirst")[2]).data('kendoComboBox').dataSource.read();
    //        $scope.$apply();
    //    },
    //    dataBound: function (e) {

    //    },

    //}
    $scope.accountCodeAutocomplete = {
        dataSource: $scope.accountCodeDataSource,
        dataTextField: 'ACC_EDESC',
        dataValueField: 'ACC_CODE',
        suggest: true,
        autoBind: true,

        filter: "contains",
        highlightFirst: true,
        close: function (e) {

            if (e.sender.dataItem()) {
                var dataItem = e.sender.dataItem();
                if (dataItem == undefined)
                    return;
                $scope.accName = dataItem.ACC_EDESC;
                $scope.accCode = dataItem.ACC_CODE;
                window.accCode = dataItem.ACC_CODE;
                var accCode = window.accCode;
                index = parseInt(this.element.closest('td').find('input')[0].value);
                window.globalIndex = index;
                if ($scope.childModels[index].AMOUNT == $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT) {
                    $scope.dynamicSubLedgerModalData[index].REMAINING_AMOUNT = 0;
                    $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT = $scope.childModels[index].AMOUNT;
                    $('.remainingamt').removeClass("borderred");
                }
                if (accCode === $scope.dynamicSubLedgerModalData[index].ACC_CODE) {
                    $scope.dynamicSubLedgerModalData[index] = $scope.dynamicSubLedgerModalData[index];
                    $scope.dynamicModalData[index] = $scope.dynamicModalData[index];
                } else {

                    $scope.dynamicSubLedgerModalData[index].SUBLEDGER = [{
                        SERIAL_NO: index + 1,
                        SUB_CODE: "",
                        SUB_EDESC: "",
                        AMOUNT: "",
                        PARTICULARS: "",
                        REFRENCE: ""
                    }];
                    $scope.dynamicModalData[index].BUDGET = [{
                        SERIAL_NO: index + 1,
                        BUDGET_CODE: "",
                        AMOUNT: "",
                        NARRATION: ""
                    }];
                }

                $scope.childModels[index][TRANSACTION_TYPE] = $scope.childModels[index].TRANSACTION_TYPE;
                $scope.transactiontype = $scope.childModels[index].TRANSACTION_TYPE;
                //$scope.dynamicSubLedgerModalData[index].ACC_CODE = accCode;


                var response = $http.get('/api/TemplateApi/getSubledgerCodeByAccCode?accCode=' + accCode);
                response.then(function (res) {
                    if (res.data != "0") {
                        $scope.dynamicSubLedgerModalData[index].ACC_CODE = accCode;
                        popupAccess = true;
                        $scope.popUp(index);

                    }
                });

                var first = $(".subledgerfirst:input");
                $.each(first, function (i, obj) {
                    obj = $(obj);
                    if (!_.isEmpty(obj.data('kendoComboBox'))) {
                        obj.data('kendoComboBox').dataSource.read();
                    }
                });
                var second = $(".subledgersecond:input");
                $.each(second, function (i, obj) {
                    obj = $(obj);
                    if (!_.isEmpty(obj.data('kendoComboBox'))) {
                        obj.data('kendoComboBox').dataSource.read();
                    }
                });

                //var len = (parseInt(index) * 2) + 1;
                //$($(".subledgerfirst:input")[len]).data('kendoComboBox').dataSource.read();
                //$($(".subledgersecond:input")[len]).data('kendoComboBox').dataSource.read();

                $scope.$apply();

            }
        },

        change: function (e) {
            //
            //var dataItem = this.dataItem(this.select());
            //if (dataItem == undefined)
            //    return;
            //$scope.accName = dataItem.ACC_EDESC;
            //$scope.accCode = dataItem.ACC_CODE;
            //window.accCode = dataItem.ACC_CODE;
            //var accCode = window.accCode;
            //index = parseInt(this.element.closest('td').find('input')[0].value);
            //window.globalIndex = index;
            //if ($scope.childModels[index].AMOUNT == $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT) {
            //    $scope.dynamicSubLedgerModalData[index].REMAINING_AMOUNT = 0;
            //    $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT = $scope.childModels[index].AMOUNT;
            //    $('.remainingamt').removeClass("borderred");
            //}
            //if (accCode === $scope.dynamicSubLedgerModalData[index].ACC_CODE) {
            //    $scope.dynamicSubLedgerModalData[index] = $scope.dynamicSubLedgerModalData[index];
            //    $scope.dynamicModalData[index] = $scope.dynamicModalData[index];
            //} else {

            //    $scope.dynamicSubLedgerModalData[index].SUBLEDGER = [{
            //        SERIAL_NO: index + 1,
            //        SUB_CODE: "",
            //        SUB_EDESC: "",
            //        AMOUNT: "",
            //        PARTICULARS: "",
            //        REFRENCE: ""
            //    }];
            //    $scope.dynamicModalData[index].BUDGET = [{
            //        SERIAL_NO: index + 1,
            //        BUDGET_VAL: "",
            //        AMOUNT: "",
            //        NARRATION: ""
            //    }];
            //}

            //$scope.childModels[index][TRANSACTION_TYPE] = $scope.childModels[index].TRANSACTION_TYPE;
            //$scope.transactiontype = $scope.childModels[index].TRANSACTION_TYPE;
            //$scope.dynamicSubLedgerModalData[index].ACC_CODE = accCode;

            //var response = $http.get('/api/TemplateApi/getSubledgerCodeByAccCode?accCode=' + accCode);
            //response.then(function (res) {
            //    if (res.data != "0") {

            //        popupAccess = true;
            //        $scope.popUp(index);

            //    }
            //});

            //var first = $(".subledgerfirst:input");
            //$.each(first, function (i, obj) {
            //    obj = $(obj);
            //    if (!_.isEmpty(obj.data('kendoComboBox'))) {
            //        obj.data('kendoComboBox').dataSource.read();
            //    }
            //});
            //var second = $(".subledgersecond:input");
            //$.each(second, function (i, obj) {
            //    obj = $(obj);
            //    if (!_.isEmpty(obj.data('kendoComboBox'))) {
            //        obj.data('kendoComboBox').dataSource.read();
            //    }
            //});

            ////var len = (parseInt(index) * 2) + 1;
            ////$($(".subledgerfirst:input")[len]).data('kendoComboBox').dataSource.read();
            ////$($(".subledgersecond:input")[len]).data('kendoComboBox').dataSource.read();

            //$scope.$apply();

            if (e.sender.dataItem() == undefined) {
                $(this.element.closest('td').find('input')[1]).parent().parent().parent().addClass('borderRed');
                $(this.element.closest('td').find('input')[1]).val("");
            }
            else {
                $(this.element.closest('td').find('input')[1]).parent().parent().parent().removeClass('borderRed');
            }
        },
        dataBound: function (e) {
            var index = this.element[0].attributes['acc-data-index'].value;
            var accountLength = ((parseInt(index) + 1) * 3) - 1;
            var account = $($(".cacccode")[accountLength]).data("kendoComboBox");
            if (account != undefined) {
                account.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(ACC_EDESC,Type, this.text()) #"), account)
                });
            }
        }

    };


    $scope.accountMasterCodeAutocomplete = {
        dataSource: $scope.accountCodeDataSource,
        dataTextField: 'ACC_EDESC',
        dataValueField: 'ACC_CODE',

        dataBound: function (e) {

        },
    }
    $scope.accountMasterCodeAutocompleteForBud = {
        dataSource: $scope.accountCodeDataSourceForBud,
        dataTextField: 'ACC_EDESC',
        dataValueField: 'ACC_CODE',

        dataBound: function (e) {

        },
    }
    $scope.budgetCenterDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllBudgetCenterForLocationByFilter"
            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        },
    }
    var budgetCode = "";

    $scope.budgetCenterChildDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllBudgetCenterChildByFilter"
            },
            parameterMap: function (data, action) {

                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value,
                            accCode: budgetCode
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: "",
                            accCode: budgetCode
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: "",
                        accCode: budgetCode
                    };
                    return newParams;
                }
            }
        },
    }
    $scope.budgetCenterOption = {
        dataSource: $scope.budgetCenterDataSource,
        dataTextField: 'BUDGET_EDESC',
        dataValueField: 'BUDGET_CODE',
        filter: "contains",
        select: function (e) {
        },
        dataBound: function (e) {

        },
        change: function (e) {

            budgetCode = this.value();
        }
    }
    $scope.subLedgerDataSource = {
        type: "json",
        serverFiltering: true,
        autoBind: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllSubLedgerByFilter"
            },
            parameterMap: function (data, action) {

                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value,
                            accCode: accCode
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: "",
                            accCode: accCode
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: "",
                        accCode: accCode
                    };
                    return newParams;
                }
            }
        },
    }
    $scope.subledgerAutocomplete =
    {
        dataSource: $scope.subLedgerDataSource,
        dataTextField: 'SUB_EDESC',
        dataValueField: 'SUB_CODE',

        select: function (e) {

        },
        dataBound: function (e) {

        },
    }
    $scope.budgetCenterChildOption = {
        dataSource: $scope.budgetCenterChildDataSource,
        dataTextField: 'BUDGET_EDESC',
        dataValueField: 'BUDGET_CODE',
        dataBound: function (e) {

        }
    }
    $scope.flagKyeDown = function ($index, event) {
        debugger;
        if (event.keyCode == 13) {
            accCode = $scope.childModels[$index][ACC_CODE];
            //$scope.dynamicModalData[$index].BUDGET_FLAG = $scope.childModels[$index].TO_BUDGET_FLAG == undefined ? "" : $scope.childModels[$index].TO_BUDGET_FLAG;
            index = $index;
            $('.budgetFlag_' + $index).modal('toggle');
            $(".budgetFlag_" + index).on('shown.bs.modal', function () {
                $($(".budgetCenterAutoComplete input")[0]).focus();
            });

            var first = $(".budgetCenterAutoComplete:input");
            $.each(first, function (i, obj) {
                obj = $(obj);
                if (!_.isEmpty(obj.data('kendoComboBox'))) {
                    obj.data('kendoComboBox').dataSource.read();
                }
            });
            $scope.childModels[$index].TO_BUDGET_FLAG = $scope.childModels[$index].TO_BUDGET_FLAG == 'L' ? 'L' : 'E';
            //$scope.dynamicModalData[$index].BUDGET_FLAG = $scope.childModels[$index].TO_BUDGET_FLAG;
        }



    }
    $scope.popUp = function ($index) {
        if (popupAccess === true)
            $(".dynamicSubLedgerModal_" + $index).modal('toggle');
    }

    $scope.loadingBtn = function () {
        $("#savedocumentformdata").button('loading');
        $(".portlet-title .btn").attr("disabled", "disabled");

    }
    $scope.loadingBtnReset = function () {
        $("#savedocumentformdata").button('reset');
        $(".portlet-title .btn").attr("disabled", false);
    }
    //---------------------- Draft start ----------------------------
    $scope.showDraftModal = function () {
        var draftUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetDraftList?moduleCode=" + $scope.ModuleCode + "&formCode=" + $scope.formcode;
        $scope.drafDataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: draftUrl,
                }
            }
        });
        $scope.drafOptions = {
            dataSource: $scope.drafDataSource,
            dataTextField: 'TEMPLATE_EDESC',
            dataValueField: 'TEMPLATE_CODE',
            filter: 'contains',
            dataBound: function (e) {

            },
            select: function (e) {

                var dataItem = this.dataItem(e.item.index());
                $scope.saveAsDraft.TEMPLATE_NO = dataItem.TEMPLATE_CODE;
                $scope.saveAsDraft.TEMPLATE_EDESC = dataItem.TEMPLATE_EDESC;
                $scope.saveAsDraft.TEMPLATE_NDESC = dataItem.TEMPLATE_EDESC;
                $scope.draftsave = "Update";
            },
        }
        $("#getDraftModal").modal("toggle");
    }

    var param1 = $routeParams.menu;

    if (param1 == "draft") {
        $.when(d2).done(function (result) {
            $scope.showDraftModal();
        });
    }

    $scope.refreshDraft = function () {
        $scope.draftsave = "Save";
        $scope.saveAsDraft = {
            TEMPLATE_NO: "",
            TEMPLATE_EDESC: "",
            TEMPLATE_NDESC: ""
        }
    }
    $scope.getDraftDetails = function () {
        var d7 = $.Deferred();
        var templateCode = $("#formDraftTemplate").data("kendoComboBox").value();
        if (templateCode == "" || templateCode == null)
            return displayPopupNotification("Please select the template.", "warning");
        var formDetail = inventoryservice.getDraftFormDetail_ByFormCode(templateCode, d7);
        $.when(d7).done(function (result) {

            $scope.formDetail = result.data;

            if ($scope.formDetail.length > 0) {
                $scope.DocumentName = $scope.formDetail[0].TABLE_NAME;
                $scope.companycode = $scope.formDetail[0].COMPANY_CODE;
            }
            var values = $scope.formDetail;

            angular.forEach(values,
                function (value, key) {
                    if (parseInt(value.SERIAL_NO) == 0 && value.DELETED_FLAG == 'N') {
                        var primaryCol = PrimaryColumnForTable(value.TABLE_NAME);
                        if (value['COLUMN_NAME'].indexOf('DATE') > -1) {
                            $scope.masterModelTemplate[value['COLUMN_NAME']] = $filter('date')(new Date(), 'dd-MMM-yyyy');
                        }
                        else if (value['COLUMN_NAME'].indexOf('AMOUNT') > -1) {
                            $scope.masterModels[value.COLUMN_NAME] = parseFloat(value.COLUMN_VALUE);
                        }
                        else if (value['COLUMN_NAME'].indexOf(primaryCol) > -1) {

                        }
                        else {
                            $scope.masterModels[value.COLUMN_NAME] = value.COLUMN_VALUE;
                        }
                    }
                });


            var uniqLen = _.uniq(values, 'SERIAL_NO');
            if (uniqLen.length > 1) {
                $scope.ChildFormElement = [];
                $scope.childModels = [];
            }
            for (var i = 1; i < uniqLen.length; i++) {
                var result = {};
                angular.forEach(values, function (val, key) {
                    var serialNo = val.SERIAL_NO;
                    if (parseInt(val.SERIAL_NO) != 0) {
                        if (i == parseInt(serialNo)) {
                            if (val['COLUMN_NAME'].indexOf('AMOUNT') > -1) {
                                result[val.COLUMN_NAME] = parseFloat(val.COLUMN_VALUE);
                            }
                            else if (val['COLUMN_NAME'].indexOf('PRICE') > -1) {
                                result[val.COLUMN_NAME] = parseFloat(val.COLUMN_VALUE);
                            } else if (val['COLUMN_NAME'].indexOf('QUANTITY') > -1) {
                                result[val.COLUMN_NAME] = parseFloat(val.COLUMN_VALUE);
                            } else {
                                result[val.COLUMN_NAME] = val.COLUMN_VALUE;
                            }

                        }
                    }
                });

                var tempCopy = angular.copy($scope.childModelTemplate);
                $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
                $scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, result));

            }
            $("#getDraftModal").modal("toggle");
        })
    }
    $scope.SaveAsDraft = function () {
        $("#saveAsDraftModal").modal("toggle");
    }




    $scope.saveTemplateInDraft = function () {

        var saveflag;
        if ($scope.draftsave == "Save")
            saveflag = "0";
        else
            saveflag = "1";
        var formcode = $scope.formcode;
        var tablename = $scope.DocumentName;
        var orderno = $scope.OrderNo;
        var masterelementvalues = $scope.masterModels;
        var grandtotal = $scope.summary.grandTotal;
        var masterElementJson = {};

        $.each($scope.MasterFormElement, function (key, value) {
            if (value['COLUMN_NAME'].indexOf('DATE') > -1) {

                var date = $scope.masterModels[value.COLUMN_NAME];
                $scope.masterModels[value.COLUMN_NAME] = moment(date).format('DD-MMM-YYYY');
            }
        });
        var childcolumnkeys = "";
        for (key in $scope.childModels[0]) {


            childcolumnkeys += key + ",";

        }
        $.each($scope.MasterFormElement, function (key, value) {
            if ($scope.masterModels[value.COLUMN_NAME] == undefined || $scope.masterModels[value.COLUMN_NAME] == null) {
                $scope.masterModels[value.COLUMN_NAME] = "";
            }
        });
        $.each($scope.ChildFormElement, function (key, value) {
            $.each($scope.childModels, function (i, val) {
                if (value.COLUMN_NAME in $scope.childModels[i]) {
                    if ($scope.childModels[i].value.COLUMN_NAME.toString() == "NaN" || $scope.childModels[i].value.COLUMN_NAME == undefined || $scope.childModels[i].value.COLUMN_NAME == null) {
                        $scope.childModels[i].value.COLUMN_NAME = "";
                    }
                }
            });

        });
        var model = {
            Save_Flag: saveflag,
            Table_Name: tablename,
            Form_Code: formcode,
            FORM_TEMPLATE: $scope.saveAsDraft,
            Master_COLUMN_VALUE: JSON.stringify($scope.masterModels),
            Child_COLUMNS: childcolumnkeys,
            Child_COLUMN_VALUE: JSON.stringify($scope.childModels),
            Grand_Total: grandtotal,
            Custom_COLUMN_VALUE: JSON.stringify($scope.customModels),
            Order_No: orderno,
            BUDGET_TRANS_VALUE: angular.toJson($scope.dynamicModalData),

        };

        var staturl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/SaveAsDraftFormData";
        var response = $http({
            method: "POST",
            data: model,
            url: staturl,
            contentType: "application/json",
            dataType: "json"
        });
        return response.then(function (data) {
            if (data.data.MESSAGE == "INSERTED") {
                $("#saveAsDraftModal").modal("toggle");
                $scope.saveAsDraft = {
                    TEMPLATE_NO: "",
                    TEMPLATE_EDESC: "",
                    TEMPLATE_NDESC: ""
                }
                displayPopupNotification("Saved Successfully", "Success");
            }

            $scope.loadingBtnReset();
        },
            function errorCallback(response) {
                displayPopupNotification(response.data.MESSAGE, "error");
            });
    }
    //----------------------Draft end-----------------------------------

    //-------------------- Quick Setup --------------------------------

    $scope.quickSetupModal = function (items) {

        if (items == "Customer") {
            $scope.customermodelopened = true;
            $scope.clearfeilds();
        }
        else if (items == "Supplier") {
            $scope.suppliermodelopened = true;
            $scope.clearfeilds();

        }
        else {
            $scope.itemmodelopened = true;
            $scope.clearfeilds();
        }
        $("#" + items + "QuickSetupModal").modal("toggle");
    }
    $scope.quickSetup = [];
    $scope.quickSetupArr = $scope.quickSetup;
    $scope.CUSTOMER = {
        CUSTOMER_EDESC: "",
        CUSTOMER_NDESC: "",
        REGD_OFFICE_EADDRESS: "",
        TEL_MOBILE_NO1: "",
        EMAIL: "",
        MASTER_CUSTOMER_CODE: "",
        PRE_CUSTOMER_CODE: "",
        GROUP_SKU_FLAG: "",
        REMARKS: "",
        PARENT_CODE: "",
    };

    $scope.CUSTOMERArr = $scope.CUSTOMER;
    $scope.ITEM = {
        ITEM_EDESC: "",
        ITEM_NDESC: "",
        MASTER_ITEM_CODE: "",
        PRE_ITEM_CODE: "",
        GROUP_SKU_FLAG: "",
        REMARKS: "",
        PARENT_CODE: "",
    };
    $scope.ITEMArr = $scope.ITEM;
    $scope.SUPPLIER = {
        SUPPLIER_EDESC: "",
        SUPPLIER_NDESC: "",
        REGD_OFFICE_EADDRESS: "",
        TEL_MOBILE_NO1: "",
        EMAIL: "",
        GROUP_SKU_FLAG: "",
        MASTER_SUPPLIER_CODE: "",
        PRE_SUPPLIER_CODE: "",
        REMARKS: "",
        PARENT_CODE: "",

    };
    $scope.SUPPLIERArr = $scope.SUPPLIER;

    $scope.quickSave = function (param) {

        var url = window.location.protocol + "//" + window.location.host + "/api/SetupApi/insertQuickSetup";

        if (param == "customer") {

            var model = {
                PARENT_CODE: $scope.CUSTOMERArr.PARENT_CODE,
                ENG_NAME: $scope.CUSTOMERArr.CUSTOMER_EDESC,
                NEP_NAME: $scope.CUSTOMERArr.CUSTOMER_NDESC,
                REGD_OFFICE_EADDRESS: $scope.CUSTOMERArr.REGD_OFFICE_EADDRESS,
                TEL_MOBILE_NO1: $scope.CUSTOMERArr.TEL_MOBILE_NO1,
                EMAIL: $scope.CUSTOMERArr.EMAIL,
                REMARKS: $scope.CUSTOMERArr.REMARKS,
                MASTER_CODE: $rootScope.quickmastercustomercode,
                FLAG: "C",
            }
        }
        else if (param == "item") {

            var model = {
                PARENT_CODE: $scope.ITEMArr.PARENT_CODE,
                ENG_NAME: $scope.ITEMArr.ITEM_EDESC,
                NEP_NAME: $scope.ITEMArr.ITEM_NDESC,
                CATEGORY_CODE: $scope.ITEMArr.CATEGORY_CODE,
                INDEX_MU_CODE: $scope.ITEMArr.INDEX_MU_CODE,
                REMARKS: $scope.ITEMArr.REMARKS,
                MASTER_CODE: $rootScope.quickmasteritemcode,
                FLAG: "I",
            }
        }
        else if (param == "supplier") {

            var model = {
                PARENT_CODE: $scope.SUPPLIERArr.PARENT_CODE,
                ENG_NAME: $scope.SUPPLIERArr.SUPPLIER_EDESC,
                NEP_NAME: $scope.SUPPLIERArr.SUPPLIER_NDESC,
                REGD_OFFICE_EADDRESS: $scope.SUPPLIERArr.REGD_OFFICE_EADDRESS,
                TEL_MOBILE_NO1: $scope.SUPPLIERArr.TEL_MOBILE_NO1,
                EMAIL: $scope.SUPPLIERArr.EMAIL,
                REMARKS: $scope.SUPPLIERArr.REMARKS,
                MASTER_CODE: $rootScope.quickmastersuppliercode,
                FLAG: "S",
            }
        }
        $http({
            method: 'POST',
            url: url,
            data: model

        }).then(function successCallback(response) {


            var switch_on = response.data.MESSAGE;
            switch (switch_on) {
                case 'C_SUCCESS':
                    displayPopupNotification("Data succesfully saved ", "success");
                    $scope.refreshquick();

                    //$scope.quickSetupModal('Customer');
                    //$scope.CUSTOMERArr = '';
                    break;
                case 'I_SUCCESS':
                    displayPopupNotification("Data succesfully saved ", "success");
                    $scope.refreshquick();

                    //$scope.quickSetupModal('Item');
                    //$scope.ITEMArr = '';
                    break;
                case 'S_SUCCESS':
                    displayPopupNotification("Data succesfully saved ", "success");
                    $scope.refreshquick();

                    //$scope.quickSetupModal('Supplier');
                    //$scope.SUPPLIERAr = '';
                    break;
                case 'ERROR':
                    displayPopupNotification(response.data.STATUS_CODE, "error");
                default:
                    displayPopupNotification("Data succesfully saved ", "success");
            }

        }, function errorCallback(response) {
            $scope.refresh();
            displayPopupNotification(response.data.STATUS_CODE, "error");
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

    };


    //$scope.refresh = function () {
    //    location.reload();
    //};

    //$scope.close = function () {
    //    $('#class').modal('hide');
    //};

    $scope.clearfeilds = function () {
        $scope.CUSTOMERArr = {
            CUSTOMER_EDESC: "",
            CUSTOMER_NDESC: "",
            REGD_OFFICE_EADDRESS: "",
            TEL_MOBILE_NO1: "",
            EMAIL: "",
            MASTER_CUSTOMER_CODE: "",
            PRE_CUSTOMER_CODE: "",
            GROUP_SKU_FLAG: "",
            REMARKS: "",
            PARENT_CODE: "",
        };
        $scope.ITEMArr = {
            ITEM_EDESC: "",
            ITEM_NDESC: "",
            CATEGORY_CODE: "",
            INDEX_MU_CODE: "",
            MASTER_ITEM_CODE: "",
            PRE_ITEM_CODE: "",
            GROUP_SKU_FLAG: "",
            REMARKS: "",
            PARENT_CODE: "",
        };
        $scope.SUPPLIERArr = {
            SUPPLIER_EDESC: "",
            SUPPLIER_NDESC: "",
            REGD_OFFICE_EADDRESS: "",
            TEL_MOBILE_NO1: "",
            EMAIL: "",
            GROUP_SKU_FLAG: "",
            MASTER_SUPPLIER_CODE: "",
            PRE_SUPPLIER_CODE: "",
            REMARKS: "",
            PARENT_CODE: "",

        };
    }
    $scope.refreshquick = function () {
        $scope.clearfeilds();
        if ($scope.customermodelopened == true) {
            $scope.quickSetupModal('Customer');
            $scope.customermodelopened = false;

        }
        else if ($scope.suppliermodelopened == true) {

            $scope.quickSetupModal('Supplier');
            $scope.suppliermodelopened = false;
        }
        else {
            $scope.quickSetupModal('Item');
            $scope.itemmodelopened = false;
        }
    };

    //-------------------- Quick Setup End----------------------------



    $scope.print_header;
    $scope.print_body_col;

    $scope.TodayDate = AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD'));

    $scope.getNepaliDate = function (date) {
        return AD2BS(moment(date).format('YYYY-MM-DD'));
    }

    $scope.print = function (event) {
        if (event != "1") {
            var printContents = document.getElementById('printTemplateForm').innerHTML;

            var popupWin = window.open('', '_blank', 'width=800,height=800', 'orientation = portrait');
            popupWin.ScreenOrientation = "Portrait";
            popupWin.document.open();
            popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
            popupWin.document.close();
            //var divToPrint = document.getElementById('printTemplateForm');
            //var newWin = window.open('', 'Print-Window');
            //newWin.document.open();
            //newWin.document.write('<html><head><link rel="stylesheet" type="text/css" media="print" href="~/Areas/NeoERP.ProjectManagement/Content/Site.css" /></head><body onload="window.print()">' + divToPrint.innerHTML + '</body></html>');
            ////newWin.document.write('<html><body onload="window.print()">' + divToPrint.innerHTML + '</body></html>');
            //newWin.document.close();
            //setTimeout(function () { newWin.close(); }, 10);
        }
        $scope.someFn();
        $scope.dynamicModalData = [{
            LOCATION_CODE: "",
            BUDGET_FLAG: "",
            BUDGET: [{
                SERIAL_NO: 1,
                BUDGET_CODE: "",
                QUANTITY: "",
                ACC_CODE: "",
            }]

        }];
        $scope.summary.grandTotal = 0;
        $scope.summary = { 'grandTotal': 0 };
        $scope.units = [];
        $scope.totalQty = 0;
        myInventoryDropzone.processQueue();
        $("#printTemplateModal").toggle('modal');
    }

    $scope.SaveDocumentFormData = function (param) {
        debugger;
        //check master model validation       
        $scope.loadingBtn();
        //check master customer validation
        if ($scope.masterModels.hasOwnProperty("CUSTOMER_CODE")) {
            var master_customer_code = $scope.masterModels.CUSTOMER_CODE;
            if ($(".mcustomer").hasClass("borderRed") || master_customer_code == null || master_customer_code == "" || master_customer_code == undefined) {
                var dataForm = _.findWhere($scope.formDetail, { COLUMN_NAME: "CUSTOMER_CODE" })
                if (dataForm.DEFA_VALUE === "") {
                    displayPopupNotification("Customer Code is required", "warning");
                    return $scope.loadingBtnReset();
                } else {
                    $scope.masterModels.CUSTOMER_CODE = dataForm.DEFA_VALUE;
                }

            }
        };
        if ($scope.masterModels.hasOwnProperty("ISSUE_TYPE_CODE")) {
            var master_issue_type_code = $scope.masterModels.ISSUE_TYPE_CODE;
            if ($(".missuetype").hasClass("borderRed") || master_issue_type_code == null || master_issue_type_code == "" || master_issue_type_code == undefined) {
                displayPopupNotification("Issue Type Code is required", "warning");
                return $scope.loadingBtnReset();
            }
        };
        //check master From Location validation

        if ($scope.masterModels.hasOwnProperty("FROM_LOCATION_CODE")) {
            var master_From_location_code = $scope.masterModels.FROM_LOCATION_CODE;
            if ($(".mlocation").hasClass("borderRed") || master_From_location_code == null || master_From_location_code == "" || master_From_location_code == undefined) {
                displayPopupNotification("Location Code is required", "warning");
                return $scope.loadingBtnReset();
            }
        };


        //check master To Location validation

        if ($scope.masterModels.hasOwnProperty("TO_LOCATION_CODE")) {
            var master_to_location_code = $scope.masterModels.TO_LOCATION_CODE;
            if ($(".mtolocation").hasClass("borderRed") || master_to_location_code == null || master_to_location_code == "" || master_to_location_code == undefined) {
                displayPopupNotification("Location Code is required", "warning");
                return $scope.loadingBtnReset();
            }
        };

        //check master Currency validation

        if ($scope.masterModels.hasOwnProperty("CURRENCY_CODE")) {
            var master_currency_code = $scope.masterModels.CURRENCY_CODE;
            if (master_currency_code == null || master_currency_code == "" || master_currency_code == undefined) {
                var result = _.find($scope.formDetail, function (item) {
                    return item.COLUMN_NAME == "CURRENCY_CODE";
                });
                if (result != null) {
                    master_currency_code = result.DEFA_VALUE;
                }

                var resultExcange = _.find($scope.formDetail, function (item) {
                    return item.COLUMN_NAME == "EXCHANGE_RATE";
                });
                if (resultExcange != null) {
                    $scope.masterModels["EXCHANGE_RATE"] = resultExcange.DEFA_VALUE;
                }
            }

            if ($(".mcurrency").hasClass("borderRed") || master_currency_code == null || master_currency_code == "" || master_currency_code == undefined) {
                displayPopupNotification("Currency Code is required", "warning");
                return $scope.loadingBtnReset();
            }
        };

        //check master supplier code validation
        if ($scope.masterModels.hasOwnProperty("SUPPLIER_CODE")) {

            //   //to solve problem in mastercode binding for update
            if ($scope.orderno !== "undefined") {
                if ($scope.masterModels.SUPPLIER_CODE === '' || $scope.masterModels.SUPPLIER_CODE === null || $scope.masterModels.SUPPLIER_CODE === undefined) {
                    if ($rootScope.suppliervalidation === "") {
                        $scope.masterModels.SUPPLIER_CODE = suppliercodeforupdate;
                    }

                }
            }
            var master_supplier_code = $scope.masterModels.SUPPLIER_CODE;
            if ($(".msupplier").hasClass("borderRed") || master_supplier_code == null || master_supplier_code == "" || master_supplier_code == undefined) {
                displayPopupNotification("Supplier Code is required", "warning");
                return $scope.loadingBtnReset();
            }
        };


        //check master priority code validation
        if ($scope.masterModels.hasOwnProperty("PRIORITY_CODE")) {
            var master_priotrity_code = $scope.masterModels.PRIORITY_CODE;
            if ($(".mprority").hasClass("borderRed") || master_priotrity_code == null || master_priotrity_code == "" || master_priotrity_code == undefined) {
                displayPopupNotification("Priority Code is required", "warning");
                return $scope.loadingBtnReset();
            }
        };


        //check master division code validation
        if ($scope.masterModels.hasOwnProperty("DIVISION_CODE")) {
            var master_division_code = $scope.masterModels.DIVISION_CODE;
            if ($(".mdivision").hasClass("borderRed") || master_division_code == null || master_division_code == "" || master_division_code == undefined) {
                var dataForm = _.findWhere($scope.formDetail, { COLUMN_NAME: "DIVISION_CODE" })
                if (dataForm.DISPLAY_FLAG == "Y") {
                    displayPopupNotification("Division Code is required", "warning");
                    return $scope.loadingBtnReset();
                }
                else {
                    $scope.masterModels.DIVISION_CODE = dataForm.DEFA_VALUE;
                }
            }
        };

        //check master employee code / marketing person validation
        if ($scope.masterModels.hasOwnProperty("EMPLOYEE_CODE")) {
            var master_employee_code = $scope.masterModels.EMPLOYEE_CODE;
            if ($(".memployee").hasClass("borderRed") || master_employee_code == null || master_employee_code == "" || master_employee_code == undefined) {

                var dataForm = _.findWhere($scope.formDetail, { COLUMN_NAME: "EMPLOYEE_CODE" })
                if (dataForm.DISPLAY_FLAG == "Y") {
                    displayPopupNotification("Employee Code is required", "warning");
                    return $scope.loadingBtnReset();
                }
                else {
                    $scope.masterModels.EMPLOYEE_CODE = dataForm.DEFA_VALUE;
                }
            }
        };

        //check master sales type validation
        if ($scope.masterModels.hasOwnProperty("SALES_TYPE_CODE")) {
            var master_salestype = $scope.masterModels.SALES_TYPE_CODE;
            if ($(".msalestype").hasClass("borderRed") || master_salestype == null || master_salestype == "" || master_salestype == undefined) {
                var dataForm = _.findWhere($scope.formDetail, { COLUMN_NAME: "SALES_TYPE_CODE" })
                if (dataForm.DISPLAY_FLAG == "Y") {
                    displayPopupNotification("Sales Type Code is required", "warning");
                    return $scope.loadingBtnReset();
                }
                else {
                    $scope.masterModels.SALES_TYPE_CODE = dataForm.DEFA_VALUE;
                }

            }
        };
        //check master terms days

        if ($scope.masterModels.hasOwnProperty("TERMS_DAY")) {
            var TERMS_DAY_VAL = $scope.masterModels.TERMS_DAY;
            if ($(".termsday").hasClass("borderRed")) {
                displayPopupNotification("Terms of day must be less than 100", "warning");
                return $scope.loadingBtnReset();
            }
        };

        //check child validation
        var childlen = $scope.childModels.length;
        for (var i = 0; i < childlen; i++) {


            //check child  to location validation
            if ($scope.childModels[0].hasOwnProperty("TO_LOCATION_CODE")) {

                var child_from_location = $scope.childModels[i].TO_LOCATION_CODE;
                if ($(".clocation").parent().parent().hasClass("borderRed") || child_from_location == null || child_from_location == "" || child_from_location == undefined) {
                    displayPopupNotification("Location Code is required", "warning");
                    return $scope.loadingBtnReset();
                }
            };
            //check child  from location validation
            if ($scope.childModels[0].hasOwnProperty("FROM_LOCATION_CODE")) {

                var child_from_location = $scope.childModels[i].FROM_LOCATION_CODE;
                if ($(".clocation").parent().parent().hasClass("borderRed") || child_from_location == null || child_from_location == "" || child_from_location == undefined) {
                    displayPopupNotification("Location Code is required", "warning");
                    return $scope.loadingBtnReset();
                }
            };
            //check child product validation
            if ($scope.childModels[0].hasOwnProperty("ITEM_CODE")) {
                ;
                var child_item = $scope.childModels[i].ITEM_CODE;
                if ($(".cproducts").parent().parent().hasClass("borderRed") || child_item == null || child_item == "" || child_item == undefined) {
                    displayPopupNotification("Product Code is required", "warning");
                    return $scope.loadingBtnReset();
                }
            }
            //check child quantity validation
            if ($scope.childModels[0].hasOwnProperty("QUANTITY")) {
                var child_quanity = $scope.childModels[i].QUANTITY;
                if (child_quanity == null || child_quanity == "" || child_quanity == undefined) {
                    displayPopupNotification("Quantity is required", "warning");
                    return $scope.loadingBtnReset();
                };
            }
            //check child cal quantity validation
            if ($scope.childModels[0].hasOwnProperty("CALC_QUANTITY")) {
                var calc_quanity = $scope.childModels[i].CALC_QUANTITY;
                if (calc_quanity === null || calc_quanity === "") {
                    displayPopupNotification("Calculated Quantity is required", "warning");
                    return $scope.loadingBtnReset();
                }
                if (calc_quanity === undefined) {
                    displayPopupNotification("Enter Calculated Quantity Correctly", "error");
                    return $scope.loadingBtnReset();
                }
            }
            //check rate/unit validation
            if ($scope.childModels[0].hasOwnProperty("UNIT_PRICE")) {
                var child_rate = $scope.childModels[i].UNIT_PRICE;
                //if (child_rate == null)
                //{
                //    $scope.childModels[i].UNIT_PRICE = 0;
                //    child_rate = 0;
                //}
                if (child_rate == null || child_rate === "") {
                    displayPopupNotification("Rate/Unit Price is required", "warning");
                    return $scope.loadingBtnReset();
                }
                if (child_rate === undefined) {
                    displayPopupNotification("Enter Amount Correctly", "error");
                    return $scope.loadingBtnReset();
                }
            };

            //check cal_rate/unit validation
            if ($scope.childModels[0].hasOwnProperty("CALC_UNIT_PRICE")) {
                var calc_rate = $scope.childModels[i].CALC_UNIT_PRICE;

                //if (calc_rate == null) {
                //    $scope.childModels[i].CALC_UNIT_PRICE = 0;
                //    calc_rate = 0;
                //}
                if (calc_rate == null || calc_rate === "") {
                    displayPopupNotification("Calcutaled Rate/Unit Price is required", "warning");
                    return $scope.loadingBtnReset();
                }
                if (calc_rate === undefined) {
                    displayPopupNotification("Enter Calcutaled Amount Correctly", "error");
                    return $scope.loadingBtnReset();
                }
            };

            if ($scope.childModels[0].hasOwnProperty("COMPLETED_QUANTITY")) {

                var completed_quantity = $scope.childModels[i].COMPLETED_QUANTITY;
                if (completed_quantity === null || completed_quantity === "") {
                    displayPopupNotification("Calcutaled Completed Quantity  is required", "warning");
                    return $scope.loadingBtnReset();
                }
                if (completed_quantity === undefined) {
                    displayPopupNotification("Enter Completed Quantity Correctly", "error");
                    return $scope.loadingBtnReset();
                }
            };

            //Sub Project Code 
            if ($scope.childModels[0].hasOwnProperty("SUB_PROJECT_CODE")) {
                var subProjectCode = $scope.childModels[i].SUB_PROJECT_CODE;
                if (subProjectCode === null || subProjectCode === "") {
                    displayPopupNotification("Sub Project Code is required", "warning");
                    return $scope.loadingBtnReset();
                }
                if (subProjectCode === undefined) {
                    displayPopupNotification("Select Sub Project Correctly", "error");
                    return $scope.loadingBtnReset();
                }
            }
            //End of Sub Project Code 
        };

        //showloader();
        var saveflag = param;
        var formcode = $scope.formcode;
        var tablename = $scope.DocumentName;
        var orderno = $scope.OrderNo;
        var masterelementvalues = $scope.masterModels;
        //var grandtotal = $scope.summary.grandTotal;
        var grandtotal = $scope.adtotal;
        var masterElementJson = {};

        $.each($scope.MasterFormElement, function (key, value) {
            if (value['COLUMN_NAME'].indexOf('DATE') > -1) {

                var date = $scope.masterModels[value.COLUMN_NAME];
                $scope.masterModels[value.COLUMN_NAME] = moment(date).format('DD-MMM-YYYY');
            }
        });
        var childcolumnkeys = "";
        for (key in $scope.childModels[0]) {


            childcolumnkeys += key + ",";

        }
        $.each($scope.MasterFormElement, function (key, value) {
            if ($scope.masterModels[value.COLUMN_NAME] == undefined || $scope.masterModels[value.COLUMN_NAME] == null) {
                $scope.masterModels[value.COLUMN_NAME] = "";
            }
        });
        $.each($scope.ChildFormElement, function (key, value) {
            $.each($scope.childModels, function (i, val) {
                if (value.COLUMN_NAME in $scope.childModels[i]) {
                    if ($scope.childModels[i].value.COLUMN_NAME.toString() == "NaN" || $scope.childModels[i].value.COLUMN_NAME == undefined || $scope.childModels[i].value.COLUMN_NAME == null) {
                        $scope.childModels[i].value.COLUMN_NAME = "";
                    }
                }
            });

        });
        $scope.BUDGET_TRANS_VALUE_ARRAY = [];
        var budgettransactiondata = $.grep($scope.dynamicModalData, function (e) {

            return e.ACC_CODE != 0;
        });
        if (budgettransactiondata.length > 0) {
            $scope.BUDGET_TRANS_VALUE_ARRAY = budgettransactiondata;
        }
        else {

            $scope.BUDGET_TRANS_VALUE_ARRAY = [];
        }
        if ($scope.childModels[0].hasOwnProperty("TO_BUDGET_FLAG") || $scope.childModels[0].hasOwnProperty("FROM_BUDGET_FLAG") || $scope.childModels[0].hasOwnProperty("BUDGET_FLAG")) {

            if ($scope.BUDGET_TRANS_VALUE_ARRAY[0].BUDGET[0].BUDGET_CODE == "" || $scope.BUDGET_TRANS_VALUE_ARRAY[0].BUDGET[0].QUANTITY == "") {

                displayPopupNotification("Budget Transaction Value is required", "error");
                $scope.loadingBtnReset();
                return;
            }
        }


        $scope.SERIAL_TRACKING_VALUE = [];
        $scope.BATCH_TRACKING_VALUE = [];
        if ($scope.youFromReference == true) {

            var serialtrackingdata = $.grep($scope.dynamicSerialTrackingModalData, function (e) {
                return e.LOCATION_CODE != undefined;
            });
            if (serialtrackingdata.length > 0) {
                $scope.SERIAL_TRACKING_VALUE = serialtrackingdata;
            }
            else {
                $scope.SERIAL_TRACKING_VALUE = [];
            }


            var batchtrackingdata = $.grep($scope.dynamicBatchTrackingModalData, function (e) {
                return e.LOCATION_CODE != undefined;
            });
            if (batchtrackingdata.length > 0) {
                $scope.BATCH_TRACKING_VALUE = batchtrackingdata;
            }
            else {
                $scope.BATCH_TRACKING_VALUE = [];
            }
        }
        else {

            var serialtrackingdata = $.grep($scope.dynamicSerialTrackingModalData, function (e) {
                return e.LOCATION_CODE != "";
            });
            if (serialtrackingdata.length > 0) {
                $scope.SERIAL_TRACKING_VALUE = serialtrackingdata;
            }
            else {
                $scope.SERIAL_TRACKING_VALUE = [];
            }


            var batchtrackingdata = $.grep($scope.dynamicBatchTrackingModalData, function (e) {
                return e.LOCATION_CODE != "";
            });
            if (batchtrackingdata.length > 0) {
                $scope.BATCH_TRACKING_VALUE = batchtrackingdata;
            }
            else {
                $scope.BATCH_TRACKING_VALUE = [];
            }
        }
        //$scope.BUDGET_TRANS_VALUE_ARRAY = [];
        //try {
        //    if ($scope.dynamicModalData.length > 0) {
        //        angular.forEach($scope.dynamicModalData, function (val1, key1) {

        //            if ($scope.dynamicModalData[key1].BUDGET === undefined) {

        //            }
        //            else {
        //                if (($scope.dynamicModalData[key1].BUDGET[0].BUDGET_VAL) == "") {
        //                    $scope.dynamicModalData.splice(key1, 1);
        //                }
        //            }
        //        });
        //    }
        //}
        //catch
        //{
        //}
        //try {
        //    if ($scope.dynamicModalData.length > 0) {
        //        if ($scope.dynamicModalData[0].BUDGET !== null || $scope.dynamicModalData[0].BUDGET !== undefined) {
        //            if ($scope.dynamicModalData[0].BUDGET.length > 0) {
        //                if (($scope.dynamicModalData[0].BUDGET[0].BUDGET_VAL) !== "") {

        //                    $scope.BUDGET_TRANS_VALUE_ARRAY = angular.toJson($scope.dynamicModalData);
        //                }
        //            }
        //        }
        //    }

        //    else {

        //        $scope.BUDGET_TRANS_VALUE_ARRAY = [];
        //    }
        //}
        //catch 
        //{
        //    $scope.BUDGET_TRANS_VALUE_ARRAY = [];
        //}


        var custommodelscheck = $("#hdncustommodels").val();
        if (custommodelscheck < 1) {

            $scope.custommodels = [];
        }
        var TempCode = $scope.tempCode;
        if ($scope.OrderNo != "undefined" && $scope.OrderNo != "" && $scope.OrderNo != null) {
            if ($scope.SDModel.TRANSPORT_INVOICE_DATE != "undefined" && $scope.SDModel.TRANSPORT_INVOICE_DATE != "" && $scope.SDModel.TRANSPORT_INVOICE_DATE != null && $scope.SDModel.TRANSPORT_INVOICE_DATE != "Invalid date") {
                $scope.SDModel.TRANSPORT_INVOICE_DATE = moment($("#TransportInvoiceDate").val()).format('DD-MMM-YYYY');
            }
            if ($scope.SDModel.DELIVERY_INVOICE_DATE != "undefined" && $scope.SDModel.DELIVERY_INVOICE_DATE != "" && $scope.SDModel.DELIVERY_INVOICE_DATE != null && $scope.SDModel.DELIVERY_INVOICE_DATE != "Invalid date") {
                $scope.SDModel.DELIVERY_INVOICE_DATE = moment($("#DeliveryDate").val()).format('DD-MMM-YYYY');
            }
            if ($scope.SDModel.GATE_ENTRY_DATE != "undefined" && $scope.SDModel.GATE_ENTRY_DATE != "" && $scope.SDModel.GATE_ENTRY_DATE != null && $scope.SDModel.GATE_ENTRY_DATE != "Invalid date") {
                $scope.SDModel.GATE_ENTRY_DATE = moment($("#GateEntryDate").val()).format('DD-MMM-YYYY');
            }
            if ($scope.SDModel.WB_DATE != "undefined" && $scope.SDModel.WB_DATE != "" && $scope.SDModel.WB_DATE != null && $scope.SDModel.WB_DATE != "Invalid date") {
                $scope.SDModel.WB_DATE = moment($("#WeighBridgeDate").val()).format('DD-MMM-YYYY');
            }
        }
        var model = {
            Save_Flag: saveflag,
            Table_Name: tablename,
            Form_Code: formcode,
            Master_COLUMN_VALUE: JSON.stringify($scope.masterModels),
            Child_COLUMNS: childcolumnkeys,
            Child_COLUMN_VALUE: JSON.stringify($scope.childModels),
            Grand_Total: grandtotal,
            Custom_COLUMN_VALUE: JSON.stringify($scope.customModels),
            FROM_REF: $scope.showRefTab,
            REF_MODEL: $rootScope.refCheckedItem,
            //BUDGET_TRANS_VALUE: angular.toJson($scope.dynamicModalData),
            Order_No: orderno,
            TempCode: TempCode,
            //BUDGET_TRANS_VALUE: $scope.BUDGET_TRANS_VALUE_ARRAY,
            BUDGET_TRANS_VALUE: JSON.stringify($scope.BUDGET_TRANS_VALUE_ARRAY),
            CHARGES: JSON.stringify($scope.data),
            //SUB_LEDGER_VALUE: JSON.stringify($scope.dynamicSubLedgerModalData),
            //SUB_LEDGER_VALUE: angular.toJson($scope.dynamicSubLedgerModalData),
            //DR_TOTAL_VALUE: angular.toJson($scope.accsummary.drTotal),
            //CR_TOTAL_VALUE: angular.toJson($scope.accsummary.crTotal),
            SHIPPING_DETAILS_VALUE: JSON.stringify($scope.SDModel),
            SERIAL_TRACKING_VALUE: JSON.stringify($scope.SERIAL_TRACKING_VALUE),
            BATCH_TRACKING_VALUE: JSON.stringify($scope.BATCH_TRACKING_VALUE),
            MODULE_CODE: $scope.ModuleCode

        };
        // Function to hide the loader
        function hideloader() {
        }
        setTimeout(hideloader, 3000);

        var staturl = window.location.protocol + "//" + window.location.host + "/api/InventoryApi/SaveInventoryFormData";
        var response = $http({
            method: "POST",
            data: model,
            url: staturl,
            contentType: "application/json",
            dataType: "json"
        });
        return response.then(function (data) {
            if (data.data.MESSAGE == "INSERTED") {

                DisplayBarNotificationMessage("Voucher Saved Successfully! </br> Voucher no: " + data.data.VoucherNo);
                var moduleCode = $scope.ModuleCode;
                hideloader();
                $scope.dzvouchernumber = data.data.VoucherNo;
                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;
                $scope.showRefTab = false;
                //myInventoryDropzone.processQueue();
                var host = $window.location.host;
                //var landingUrl = window.location.protocol + "//" + window.location.host + "/ProjectManagement/Home/Index#!PM/MenuSplitter/" + $scope.ModuleCode; /*done previously*/

                $window.location.href = window.location.protocol + "//" + window.location.host + "/ProjectManagement/Home/Index#!PM/MenuSplitter/" + $scope.ModuleCode; /*done previously*/
                //setTimeout(function () {
                //    $window.location.href = landingUrl;
                //}, 800);
            }
            else if (data.data.MESSAGE == "SAVEANDPRINT") {

                if ($scope.ModuleCode == "03") {
                    DisplayBarNotificationMessage("Voucher Saved Successfully! </br> Voucher no: " + data.data.VoucherNo);
                    $scope.someFn();
                    $scope.dynamicModalData = [{
                        LOCATION_CODE: "",
                        BUDGET_FLAG: "",
                        BUDGET: [{
                            SERIAL_NO: 1,
                            BUDGET_CODE: "",
                            QUANTITY: "",
                            ACC_CODE: "",
                        }]

                    }];
                    $scope.summary.grandTotal = 0;
                    $scope.summary = { 'grandTotal': 0 };
                    $scope.units = [];
                    $scope.totalQty = 0;
                    $scope.dzvouchernumber = data.data.VoucherNo;
                    $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                    $scope.dzformcode = data.data.FormCode;
                    $scope.showRefTab = false;
                    myInventoryDropzone.processQueue();
                    return;
                    return;
                }
                var masterelem = $scope.MasterFormElement;

                $.each($scope.MasterFormElement, function (key, value) {

                    if (value['COLUMN_NAME'].indexOf('CODE') > -1) {
                        var switched;
                        switched = value['COLUMN_NAME'];

                        switch (switched) {
                            case 'SUPPLIER_CODE':
                                $scope.masterModels["SUPPLIER_CODE"] = $('#supplier').data("kendoComboBox").dataItem().SUPPLIER_EDESC;
                                $scope.masterModels["REGD_OFFICE_EADDRESS"] = $('#supplier').data("kendoComboBox").dataItem().REGD_OFFICE_EADDRESS;
                                $scope.masterModels["TPIN_VAT_NO"] = $('#supplier').data("kendoComboBox").dataItem().TPIN_VAT_NO;
                                $scope.masterModels["TEL_MOBILE_NO1"] = $('#supplier').data("kendoComboBox").dataItem().TEL_MOBILE_NO1;
                                break;
                            case 'ISSUE_TYPE_CODE':
                                $scope.masterModels["ISSUE_TYPE_CODE"] = $('#issuetype').data("kendoComboBox").dataItem().ISSUE_TYPE_EDESC;
                                break;
                            case 'TO_BRANCH_CODE':
                                $scope.masterModels["TO_BRANCH_CODE"] = $('#branchcode').data("kendoComboBox").dataItem().BRANCH_EDESC;
                                break;

                            case "TO_LOCATION_CODE":
                                if ($('#tolocation').data("kendoComboBox") == null) {
                                    break;
                                }
                                $scope.masterModels["TO_LOCATION_CODE"] = $('#tolocation').data("kendoComboBox").dataItem().LocationName;
                                break;
                            case "FROM_LOCATION_CODE":
                                if ($('#location').data("kendoComboBox") == null) {
                                    break;
                                }
                                //if ($($(".ctolocation_" + ind)[$(".ctolocation_" + ind).length - 1]).data("kendoComboBox") == null) {
                                //    break;
                                //}
                                $scope.masterModels["FROM_LOCATION_CODE"] = $('#location').data("kendoComboBox").dataItem().LocationName;
                                break;
                            case "MASTER_ACC_CODE":
                                $scope.masterModels["MASTER_ACC_CODE"] = $('#masteracccode').data("kendoComboBox").dataItem().ACC_EDESC;
                                break;
                            case "CUSTOMER_CODE":
                                $scope.masterModels["CUSTOMER_CODE"] = $('#customers').data("kendoComboBox").dataItem().CustomerName;
                                break;
                            default:
                        }

                    }
                });
                var masterArr = $scope.ChildFormElement[0].element;
                var print_master = $.grep(masterArr, function (e) {
                    return (e['COLUMN_NAME'].indexOf("CALC") === -1 && e['COLUMN_NAME'].indexOf("REMARKS") === -1);
                });
                var print_child = [];
                $.each($scope.ChildFormElement, function (ind, it) {
                    print_child.push({
                        element: $.grep(it.element, function (e) {
                            var switch_on;
                            switch_on = e['COLUMN_NAME'];
                            switch (switch_on) {
                                case 'ITEM_CODE':
                                    $scope.childModels[ind]["ITEM_CODE"] = $($(".cproduct_" + ind)[$(".cproduct_" + ind).length - 1]).data("kendoComboBox").dataItem().ItemDescription;
                                    break;
                                case 'PRODUCT_CODE':
                                    $scope.childModels[ind]["ITEM_CODE"] = $($(".cproduct_" + ind)[$(".cproduct_" + ind).length - 1]).data("kendoComboBox").dataItem().ItemDescription;
                                    break;
                                case 'ACC_CODE':
                                    $scope.childModels[ind]["ACC_CODE"] = $($(".caccount_" + ind)[$(".caccount_" + ind).length - 1]).data("kendoComboBox").dataItem().ACC_EDESC;
                                    break;

                                case "TO_LOCATION_CODE":
                                    if ($($(".ctolocation_" + ind)[$(".ctolocation_" + ind).length - 1]).data("kendoComboBox") == null) {
                                        break;
                                    }
                                    $scope.childModels[ind]["TO_LOCATION_CODE"] = $($(".ctolocation_" + ind)[$(".ctolocation_" + ind).length - 1]).data("kendoComboBox").dataItem().LocationName;
                                    break;
                                case "FROM_LOCATION_CODE":
                                    if ($($(".ctolocation_" + ind)[$(".ctolocation_" + ind).length - 1]).data("kendoComboBox") == null) {
                                        break;
                                    }
                                    $scope.childModels[ind]["FROM_LOCATION_CODE"] = $($(".ctolocation_" + ind)[$(".ctolocation_" + ind).length - 1]).data("kendoComboBox").dataItem().LocationName;
                                    break;
                                default:
                            }
                            return (e['COLUMN_NAME'].indexOf("CALC") === -1 && e['COLUMN_NAME'].indexOf("REMARKS") === -1);
                        }),
                        additionalElements: ''
                    });
                });
                $scope.print_header = print_master;
                $scope.print_body_col = print_child;
                $scope.dzvouchernumber = data.data.VoucherNo;
                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;
                $scope.showRefTab = false;
                displayPopupNotification("Data succesfully Saved.", "success");
                $scope.TodayDate = AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD'));
                $scope.amountinword = convertNumberToWords($scope.summary.grandTotal);
                $("#printTemplateModal").toggle('modal');
            }

            else if (data.data.MESSAGE == "INSERTEDANDCONTINUE") {
                // OLD Save and Continue Logic Start

                //hideloader();
                //$scope.dzvouchernumber = data.data.VoucherNo;
                //$scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                //$scope.dzformcode = data.data.FormCode;
                //myInventoryDropzone.processQueue();
                //var host = $window.location.host;
                //var landingUrl = window.location.protocol + "//" + window.location.host + "/ProjectManagement/Template/SplitterIndex#!DT/MenuSplitter/" + $scope.ModuleCode;
                //setTimeout(function () {
                //    $window.location.href = landingUrl;
                //}, 1000);

                // OLD Save and Continue Logic End

                DisplayBarNotificationMessage("Voucher Saved Successfully! </br> Voucher no: " + data.data.VoucherNo);
                //displayPopupNotification(generateMsg,"success");
                $scope.masterChildData = [];
                $scope.dzvouchernumber = data.data.VoucherNo;
                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;

                myInventoryDropzone.processQueue();
                angular.forEach($scope.MasterFormElement, function (value, key) {
                    if (value.MASTER_CHILD_FLAG == 'M' && value.DISPLAY_FLAG == 'Y') {
                        $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                        $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                        if (value['COLUMN_NAME'].indexOf('DATE') > -1) {
                            $scope.masterModels[value['COLUMN_NAME']] = $filter('date')(new Date(), 'dd-MMM-yyyy');
                        }
                    }
                });

                var cml = $scope.childModels.length;
                var sl = parseFloat(cml) - 1;
                $scope.ChildFormElement.splice(0, sl);
                $scope.childModels.splice(0, sl);
                angular.forEach($scope.ChildFormElement[0].element, function (value, key) {
                    if (value.MASTER_CHILD_FLAG == 'C' && value.DISPLAY_FLAG == 'Y') {
                        $scope.childModels[0][value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                    }
                });
                var req = "/api/TemplateApi/GetNewOrderNo?companycode=" + $scope.companycode + "&formcode=" + $scope.FormCode + "&currentdate=" + $scope.todaydate + "&tablename=" + $scope.DocumentName + '&isSequence=false';
                $http.get(req).then(function (results) {

                    $scope.newgenorderno = results.data;
                    var primarycolumnname = PrimaryColumnForTable($scope.DocumentName);
                    $scope.someFn();
                    $scope.masterModels[primarycolumnname] = results.data;
                    $scope.masterModelTemplate[primarycolumnname] = results.data;

                });
                $scope.summary.grandTotal = 0;
                $scope.showRefTab = false;
                $scope.summary = { 'grandTotal': 0 };
                $scope.units = [];
                $scope.totalQty = 0;
                $scope.adtotal = 0;
            }
            else if (data.data.MESSAGE == "UPDATEDANDPRINT") {

                if ($scope.ModuleCode == "03") {
                    DisplayBarNotificationMessage("Voucher Saved Successfully! </br> Voucher no: " + data.data.VoucherNo);
                    $scope.someFn();
                    $scope.dynamicModalData = [{
                        LOCATION_CODE: "",
                        BUDGET_FLAG: "",
                        BUDGET: [{
                            SERIAL_NO: 1,
                            BUDGET_CODE: "",
                            QUANTITY: "",
                            ACC_CODE: "",
                        }]

                    }];
                    $scope.summary.grandTotal = 0;
                    $scope.summary = { 'grandTotal': 0 };
                    $scope.units = [];
                    $scope.totalQty = 0;
                    $scope.dzvouchernumber = data.data.VoucherNo;
                    $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                    $scope.dzformcode = data.data.FormCode;
                    $scope.showRefTab = false;
                    myInventoryDropzone.processQueue();
                    return;
                }
                var masterelem = $scope.MasterFormElement;

                $.each($scope.MasterFormElement, function (key, value) {

                    if (value['COLUMN_NAME'].indexOf('CODE') > -1) {

                        var switched;
                        switched = value['COLUMN_NAME'];
                        switch (switched) {
                            case 'SUPPLIER_CODE':
                                $scope.masterModels["SUPPLIER_CODE"] = $('#supplier').data("kendoComboBox").dataItem().SUPPLIER_EDESC;
                                $scope.masterModels["REGD_OFFICE_EADDRESS"] = $('#supplier').data("kendoComboBox").dataItem().REGD_OFFICE_EADDRESS;
                                $scope.masterModels["TPIN_VAT_NO"] = $('#supplier').data("kendoComboBox").dataItem().TPIN_VAT_NO;
                                $scope.masterModels["TEL_MOBILE_NO1"] = $('#supplier').data("kendoComboBox").dataItem().TEL_MOBILE_NO1;
                                break;
                            case 'ISSUE_TYPE_CODE':
                                $scope.masterModels["ISSUE_TYPE_CODE"] = $('#issuetype').data("kendoComboBox").dataItem().ISSUE_TYPE_EDESC;
                                break;
                            case 'TO_BRANCH_CODE':
                                $scope.masterModels["TO_BRANCH_CODE"] = $('#branchcode').data("kendoComboBox").dataItem().BRANCH_EDESC;
                                break;

                            case "TO_LOCATION_CODE":

                                $scope.masterModels["TO_LOCATION_CODE"] = $('#tolocation').data("kendoComboBox").dataItem().LocationName;
                                break;
                            case "FROM_LOCATION_CODE":
                                $scope.masterModels["FROM_LOCATION_CODE"] = $('#location').data("kendoComboBox").dataItem().LocationName;
                                break;
                            case "MASTER_ACC_CODE":
                                $scope.masterModels["MASTER_ACC_CODE"] = $('#masteracccode').data("kendoComboBox").dataItem().ACC_EDESC;
                                break;
                            case "CUSTOMER_CODE":
                                $scope.masterModels["CUSTOMER_CODE"] = $('#customers').data("kendoComboBox").dataItem().CustomerName;
                                break;
                            default:
                        }

                    }
                });
                var masterArr = $scope.ChildFormElement[0].element;
                var print_master = $.grep(masterArr, function (e) {
                    return (e['COLUMN_NAME'].indexOf("CALC") === -1 && e['COLUMN_NAME'].indexOf("REMARKS") === -1);
                });
                var print_child = [];
                $.each($scope.ChildFormElement, function (ind, it) {

                    print_child.push({
                        element: $.grep(it.element, function (e) {

                            var switch_on;
                            switch_on = e['COLUMN_NAME'];
                            switch (switch_on) {
                                case 'ITEM_CODE':
                                    $scope.childModels[ind]["ITEM_CODE"] = $($(".cproduct_" + ind)[$(".cproduct_" + ind).length - 1]).data("kendoComboBox").dataItem().ItemDescription;
                                    break;
                                case 'PRODUCT_CODE':
                                    $scope.childModels[ind]["ITEM_CODE"] = $($(".cproduct_" + ind)[$(".cproduct_" + ind).length - 1]).data("kendoComboBox").dataItem().ItemDescription;
                                    break;
                                case 'ACC_CODE':
                                    $scope.childModels[ind]["ACC_CODE"] = $($(".caccount_" + ind)[$(".caccount_" + ind).length - 1]).data("kendoComboBox").dataItem().ACC_EDESC;
                                    break;

                                case "TO_LOCATION_CODE":
                                    //$scope.childModels[ind]["TO_LOCATION_CODE"] = $($(".ctolocation_" + ind)[$(".ctolocation_" + ind).length - 1]).data("kendoComboBox").dataItem().LocationName;
                                    break;
                                case "FROM_LOCATION_CODE":
                                    //$scope.childModels[ind]["FROM_LOCATION_CODE"] = $($(".ctolocation_" + ind)[$(".ctolocation_" + ind).length - 1]).data("kendoComboBox").dataItem().LocationName;
                                    break;
                                default:
                            }
                            return (e['COLUMN_NAME'].indexOf("CALC") === -1 && e['COLUMN_NAME'].indexOf("REMARKS") === -1);
                        }),
                        additionalElements: ''
                    });
                });
                $scope.print_header = print_master;
                $scope.print_body_col = print_child;
                $scope.dzvouchernumber = data.data.VoucherNo;
                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;
                $scope.showRefTab = false;
                displayPopupNotification("Data succesfully Updated.", "success");
                $scope.TodayDate = AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD'))
                $scope.amountinword = convertNumberToWords($scope.summary.grandTotal);
                $("#printTemplateModal").toggle('modal');
            }
            else if (data.data.MESSAGE == "SAVEANDCONTINUE") {
                displayPopupNotification("Data succesfully Saved.", "success");
                $scope.someFn();
                $scope.dynamicModalData = [{
                    LOCATION_CODE: "",
                    BUDGET_FLAG: "",
                    BUDGET: [{
                        SERIAL_NO: 1,
                        BUDGET_CODE: "",
                        QUANTITY: "",
                        ACC_CODE: "",
                    }]

                }];
                $scope.summary.grandTotal = 0;
                $scope.summary = { 'grandTotal': 0 };
                $scope.units = [];
                $scope.totalQty = 0;
                $scope.dzvouchernumber = data.data.VoucherNo;
                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;
                $scope.showRefTab = false;
                myInventoryDropzone.processQueue();

            }
            else if (data.data.MESSAGE == "UPDATED") {

                DisplayBarNotificationMessage("Data succesfully updated.", "success");
                $scope.dzvouchernumber = data.data.VoucherNo;
                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;
                //myInventoryDropzone.processQueue();
                $scope.showRefTab = false;
                //var landingUrl
                $window.location.href= window.location.protocol + "//" + window.location.host + "/ProjectManagement/Home/Index#!PM/MenuSplitter/" + $scope.ModuleCode; /*done previously*/
                //setTimeout(function () {
                //    $window.location.href = landingUrl;
                //}, 1000);
            }
            else {
                hideloader();
                displayPopupNotification("Something went wrong!.Please try again later.", "error");
                //   console.log(response.data.MESSAGE);
            }
            $scope.loadingBtnReset();
        },
            function errorCallback(response) {
                hideloader();
                $scope.loadingBtnReset();
                displayPopupNotification("Something went wrong!.Please try again later.", "error");
                // console.log(response.data.MESSAGE);

            });


    };
    $scope.DeleteInvDocument = function () {
        debugger;
        bootbox.confirm({
            title: "Delete",
            message: "Are you sure?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {

                if (result === true) {

                    var delRes = inventoryservice.DeleteInvVoucher($scope.OrderNo, $scope.formcode);
                    delRes.then(function (dRes) {
                        debugger;
                        if (dRes.data.MESSAGE === "DELETED") {

                            displayPopupNotification("Data succesfully deleted ", "success");
                            $window.location.href = window.location.protocol + "//" + window.location.host + "/ProjectManagement/Home/Index#!PM/MenuSplitter/" + $scope.ModuleCode; /*done previously*/
                          
                        }
                        else if (dRes.data.MESSAGE === "POSTED") {

                            displayPopupNotification("Unpost the voucher first", "warning");
                        }
                        else if (dRes.data.MESSAGE === "REFERENCED") {

                            displayPopupNotification("Voucher is in reference.Please delete the referenced voucher:" + dRes.data.VoucherNo, "warning");
                        }
                        else {
                            displayPopupNotification("Something went wrong!.Please try again.", "error");
                        }

                    },
                        function errorCallback(response) {
                            displayPopupNotification("Something went wrong!.Please try again.", "error");
                            $scope.loadingBtnReset();
                        }
                    );

                }

            }

        });


    };
    $scope.resetSave = function () {
        // $scope.ResetDocument();
        $('#printTemplateModal').modal('toggle');
    }

    $scope.reset = function () {
        // if (orderno) {
        //     location.reload
        // }
        //// $scope.ResetDocument();
        // $('#printTemplateModal').modal('toggle');
        location.reload();
    };
    var ShippingDetailUrlForEdit = "/api/TemplateApi/GetAllShippingDtlsByFilter?FormCode=" + $scope.FormCode + "&&VoucherNo=" + $scope.OrderNo;
    $http.get(ShippingDetailUrlForEdit).then(function (res) {
        if ($scope.OrderNo != "undefined") {

            if (res.data.length > 0) {


                //$scope.SDModel = res.data[0];
                $scope.SDModel.VEHICLE_CODE = res.data[0].VEHICLE_CODE;
                $scope.SDModel.VEHICLE_OWNER_NAME = res.data[0].VEHICLE_VEHICLE_OWNER_NAME;
                $scope.SDModel.VEHICLE_OWNER_NO = res.data[0].VEHICLE_OWNER_NO;
                $scope.SDModel.DRIVER_NAME = res.data[0].DRIVER_NAME;
                $scope.SDModel.DRIVER_LICENCE_NO = res.data[0].DRIVER_LICENSE_NO;
                $scope.SDModel.DRIVER_MOBILE_NO = res.data[0].DRIVER_MOBILE_NO;
                $scope.SDModel.TRANSPORTER_CODE = res.data[0].TRANSPORTER_CODE;
                $scope.SDModel.FREIGHT_RATE = res.data[0].FREIGHT_RATE;
                $scope.SDModel.FREGHT_AMOUNT = res.data[0].FREGHT_AMOUNT;
                $scope.SDModel.START_FORM = res.data[0].START_FORM;
                $scope.SDModel.DESTINATION = res.data[0].DESTINATION;
                $scope.SDModel.CN_NO = res.data[0].CN_NO;
                $scope.SDModel.TRANSPORT_INVOICE_NO = res.data[0].TRANSPORT_INVOICE_NO;
                $scope.SDModel.TRANSPORT_INVOICE_DATE = res.data[0].TRANSPORT_INVOICE_DATE;
                $scope.SDModel.DELIVERY_INVOICE_DATE = res.data[0].DELIVERY_INVOICE_DATE;
                $scope.SDModel.WB_WEIGHT = res.data[0].WB_WEIGHT;
                $scope.SDModel.WB_NO = res.data[0].WB_NO;
                $scope.SDModel.WB_DATE = res.data[0].WB_DATE;
                $scope.SDModel.GATE_ENTRY_NO = res.data[0].GATE_ENTRY_NO;
                $scope.SDModel.GATE_ENTRY_DATE = res.data[0].GATE_ENTRY_DATE;
                $scope.SDModel.LOADING_SLIP_NO = res.data[0].LOADING_SLIP_NO;
                $scope.SDModel.SHIPPING_TERMS = res.data[0].SHIPPING_TERMS;
                //$("#mydropdownlist").val("thevalue");


            }

        }

    });
    $scope.ResetDocument = function () {

        var cml = $scope.childModels.length;
        var sl = parseFloat(cml) - 1;
        $scope.ChildFormElement.splice(0, sl);
        $scope.masterModels = {};
        $scope.childModels.splice(0, sl); // }
        $scope.dynamicModalData.splice(0, sl);
        $scope.dynamicSubLedgerModalData.splice(0, sl);
        //$scope.masterModels["MASTER_TRANSACTION_TYPE"] = "DR";
        $scope.childModels = [];
        $scope.accsummary = { 'drTotal': 0.00, 'crTotal': 0.00, 'diffAmount': 0.00 };
        //$scope.childModels.push(
        //{ ACC_CODE: "", AMOUNT: "", BUDGET_FLAG: "", TRANSACTION_TYPE: "DR", VOUCHER_DATE: "" });
        $scope.accountMasterCodeAutocomplete = [];
        $scope.BUDGET_CENTER = [];
        $scope.BUDGET_CENTER.push({
            BUDGET_VAL: "",
            AMOUNT: "",
            NARRATION: ""
        });
        $scope.dynamicModalData = [{
            LOCATION_CODE: "",
            BUDGET_FLAG: "",
            BUDGET: [{
                SERIAL_NO: 1,
                BUDGET_CODE: "",
                QUANTITY: "",
                ACC_CODE: "",
            }]

        }];

        $scope.dynamicSubLedgerModalData = [{
            ACC_CODE: 0,
            TRANSACTION_TYPE: "",
            VOUCHER_AMOUNT: "",
            SUBLEDGER_AMOUNT: "",
            REMAINING_AMOUNT: "",
            SUBLEDGER: [{
                SUB_CODE: "",
                SUB_EDESC: "",
                AMOUNT: "",
                PARTICULARS: "",
                REFRENCE: ""
            }]
        }];
        $scope.someFn();
        $scope.refreshDraft();
    }

    $scope.accsum = function () {

        //if (typeof $scope.childModels[index][TRANSACTION_TYPE] !== "undefined" && $scope.childModels[index][TRANSACTION_TYPE] != null) {

        var drsum = 0;
        var crsum = 0;


        var maslen = $scope.MasterFormElement.length;
        for (var i = 0; i < maslen; i++) {
            var masterelementname = $scope.MasterFormElement[i].COLUMN_NAME;
            if (masterelementname == "MASTER_TRANSACTION_TYPE") {
                angular.forEach($scope.childModels, function (value, key) {
                    if (typeof value[AMOUNT] !== 'undefined' && value[AMOUNT] != null) {
                        if (value[TRANSACTION_TYPE] == "DR" && (value[AMOUNT] != "" || value[AMOUNT] != 0)) {
                            drsum = drsum + value[AMOUNT];
                            $scope.accsummary.drTotal = drsum;
                            $scope.accsummary.crTotal = $scope.accsummary.drTotal;
                        }
                        if (value[TRANSACTION_TYPE] == "CR" && (value[AMOUNT] != "" || value[AMOUNT] != 0)) {
                            crsum = crsum + value[AMOUNT];
                            $scope.accsummary.crTotal = crsum;
                            $scope.accsummary.drTotal = crsum;
                        }

                    }
                });

                $scope.accsummary.diffAmount = $scope.accsummary.drTotal - $scope.accsummary.crTotal;
                $scope.masterModels["MASTER_AMOUNT"] = $scope.accsummary.drTotal;

            }
        };
        if (masterelementname != "MASTER_TRANSACTION_TYPE") {
            //if ($scope.childModels[index][TRANSACTION_TYPE] == "DR") {
            angular.forEach($scope.childModels, function (value, key) {
                if (typeof value[AMOUNT] !== 'undefined' && value[AMOUNT] != null) {
                    //console.log('value', value);
                    if (value[TRANSACTION_TYPE] == "DR" && (value[AMOUNT] != "" || value[AMOUNT] != 0)) {
                        drsum = drsum + value[AMOUNT];
                    }
                }
            });
            $scope.accsummary.drTotal = drsum;
            // }
            // if ($scope.childModels[index][TRANSACTION_TYPE] == "CR") {
            angular.forEach($scope.childModels, function (value, key) {

                if (typeof value[AMOUNT] !== 'undefined' && value[AMOUNT] != null) {
                    //console.log('value', value);
                    if (value[TRANSACTION_TYPE] == "CR" && (value[AMOUNT] != "" || value[AMOUNT] != 0)) {
                        crsum = crsum + value[AMOUNT];
                    }
                }
            });
            $scope.accsummary.crTotal = crsum;

            if ($scope.masterModels["MASTER_TRANSACTION_TYPE"] == "DR") {
                $scope.masterModels["MASTER_AMOUNT"] = $scope.accsummary.crTotal;
            }
            if ($scope.masterModels["MASTER_TRANSACTION_TYPE"] == "CR") {
                $scope.masterModels["MASTER_AMOUNT"] = $scope.accsummary.drTotal;
            }
            $scope.accsummary.diffAmount = $scope.accsummary.drTotal - $scope.accsummary.crTotal;
            if ($("#ledgermodal").data()['bs.modal'] != undefined) {
                if ($("#ledgermodal").data()['bs.modal'].isShown) {
                    $('#ledgermodal').modal('toggle');
                }

            }
        }
    };

    $scope.getmucode = function (index, productId) {

        try {
            var pId = $.isNumeric(parseInt(productId));
            if (pId === false) {
                throw "";
            }
            var staturl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetMUCodeByProductId";
            var response = $http({
                method: "GET",
                url: "/api/TemplateApi/GetMUCodeByProductId",
                params: { productId: productId },
                contentType: "application/json",
                dataType: "json"
            });
            return response.then(function (data) {
                //
                if (!data == "") {
                    $scope.childModels[index].MU_CODE = data.data[0].ItemUnit;
                }

            });
        } catch (e) {
            return;
        }

    };

    $scope.Changesubledgercode = function (key, index) {
        $scope.isPresent = false;
        var data = $($(".subledgerfirst")[2]).data("kendoComboBox").dataSource._data;
        if (data.length == 0) {
            displayPopupNotification("Please Enter Valid Code.", "warning");
            $scope.dynamicSubLedgerModalData[key].SUBLEDGER[index].SUB_CODE = "";
        }
        else {
            if ($scope.dynamicSubLedgerModalData[key].SUBLEDGER[index].SUB_CODE != "") {
                for (var i = 0; i < data.length - 1; i++) {
                    if (data[i].SUB_CODE == $scope.dynamicSubLedgerModalData[key].SUBLEDGER[index].SUB_CODE) {
                        $scope.isPresent = true;
                        return;
                    }
                    else {
                        $scope.isPresent = false;
                    }
                }
                if ($scope.isPresent == false) {
                    $scope.dynamicSubLedgerModalData[key].SUBLEDGER[index].SUB_CODE = "";
                    displayPopupNotification("Please Enter Valid Code.", "warning");
                }
            }
        }

    }
    var customUrl = "/api/TemplateApi/GetFormCustomSetup?formcode=" + $scope.formcode + "&&voucherNo=" + $scope.OrderNo;
    $http.get(customUrl).then(function (res) {
        if (res.data != null) {
            $scope.formCustomSetup = res.data;
            var customvalues = $scope.formCustomSetup;
            var count = 0;
            angular.forEach(customvalues, function (value, key) {

                if (count == 0) {
                    count++;
                    $("#hdnCustomModels").val(count);
                }
                this.push(value);
                if ($scope.OrderNo == "undefined")
                    $scope.customModels[value['FIELD_NAME']] = value['DEFA_FIELD_VALUE'];
                else
                    $scope.customModels[value['FIELD_NAME']] = value['FIELD_VALUE'];
            }, $scope.CustomFormElement);
        }
    })

    $scope.masterModelDataFn = function (fn) {
        var saleOrderformDetail = inventoryservice.getSalesOrderDetail_ByFormCodeAndOrderNo($scope.formcode, $scope.OrderNo, d4);
        $.when(d4).done(function (result) {
            fn(result);
            if ($scope.tempCode != undefined && $scope.tempCode != "" && $scope.tempCode != null) {
                var formcode = $scope.formcode;
                var tempCode = $scope.tempCode;
                inventoryservice.getDraftData_ByFormCodeAndTempCode(formcode, tempCode).then(function (result) {

                    result = result.data;
                    var uniq = _.uniq(result, 'SERIAL_NO');
                    var len = uniq.length;
                    for (var i = 0; i < len; i++) {
                        if (i === 0 || i === 1) { continue; }
                        $scope.add_child_element();
                    }
                    for (var i = 0; i < result.length; i++) {
                        if (result[i].SERIAL_NO === 0) {
                            var COL_NAME = result[i].COLUMN_NAME;
                            if (COL_NAME === "MASTER_AMOUNT" || COL_NAME === "AMOUNT") {
                                { $scope.masterModels[COL_NAME] = parseFloat(result[i].COLUMN_VALUE); }
                            } else {
                                { $scope.masterModels[COL_NAME] = result[i].COLUMN_VALUE; }
                            }

                        } else {

                            var ArrayIndex = result[i].SERIAL_NO - 1;
                            var COL_NAME = result[i].COLUMN_NAME;
                            var column_value = result[i].COLUMN_VALUE;
                            if (!isNaN(column_value)) {
                                column_value = parseInt(column_value);
                            }
                            $scope.childModels[ArrayIndex][COL_NAME] = column_value;



                        }

                    }
                    $scope.GrandtotalCalution();
                    $scope.setTotal();

                });

            }
        });

    };

    $scope.getObjWithKeysFromOtherObj = function (objKeys, objKeyswithData) {

        var keys = Object.keys(objKeys);
        var result = {};
        for (var i = 0; i < keys.length; i++) {
            result[keys[i]] = objKeyswithData[keys[i]];
        }
        return result;
    };


    $scope.ReferenceList = [];
    var req = "/api/InventoryApi/GetReferenceList?formcode=" + $scope.formcode;
    $http.get(req).then(function (response) {
        $scope.ReferenceList = response.data;
    });

    //$scope.ShowInventoryRefrence = function () {
    $scope.ShowInventoryRef = function () {
        if ($scope.havRefrence == 'Y') {
            $('#RefrenceModel').modal('show');

        }
    }
    $scope.termsdayChange = function (e) {

        if (e.length > 2) {
            $scope.tdayserror = "Value must be less than 100."
            $scope.masterModels.TERMS_DAY = "";
            $('.termsday').addClass("borderred");

        }
        else {
            $scope.tdayserror = "";
            $('.termsday').removeClass('borderRed');
        }
    };
    function convertNumberToWords(amount) {
        var words = new Array();
        words[0] = '';
        words[1] = 'One';
        words[2] = 'Two';
        words[3] = 'Three';
        words[4] = 'Four';
        words[5] = 'Five';
        words[6] = 'Six';
        words[7] = 'Seven';
        words[8] = 'Eight';
        words[9] = 'Nine';
        words[10] = 'Ten';
        words[11] = 'Eleven';
        words[12] = 'Twelve';
        words[13] = 'Thirteen';
        words[14] = 'Fourteen';
        words[15] = 'Fifteen';
        words[16] = 'Sixteen';
        words[17] = 'Seventeen';
        words[18] = 'Eighteen';
        words[19] = 'Nineteen';
        words[20] = 'Twenty';
        words[30] = 'Thirty';
        words[40] = 'Forty';
        words[50] = 'Fifty';
        words[60] = 'Sixty';
        words[70] = 'Seventy';
        words[80] = 'Eighty';
        words[90] = 'Ninety';
        amount = amount.toString();
        var atemp = amount.split(".");
        var number = atemp[0].split(",").join("");
        var n_length = number.length;
        var words_string = "";
        if (n_length <= 9) {
            var n_array = new Array(0, 0, 0, 0, 0, 0, 0, 0, 0);
            var received_n_array = new Array();
            for (var i = 0; i < n_length; i++) {
                received_n_array[i] = number.substr(i, 1);
            }
            for (var i = 9 - n_length, j = 0; i < 9; i++, j++) {
                n_array[i] = received_n_array[j];
            }
            for (var i = 0, j = 1; i < 9; i++, j++) {
                if (i == 0 || i == 2 || i == 4 || i == 7) {
                    if (n_array[i] == 1) {
                        n_array[j] = 10 + parseInt(n_array[j]);
                        n_array[i] = 0;
                    }
                }
            }
            value = "";
            for (var i = 0; i < 9; i++) {
                if (i == 0 || i == 2 || i == 4 || i == 7) {
                    value = n_array[i] * 10;
                } else {
                    value = n_array[i];
                }
                if (value != 0) {
                    words_string += words[value] + " ";
                }
                if ((i == 1 && value != 0) || (i == 0 && value != 0 && n_array[i + 1] == 0)) {
                    words_string += "Crores ";
                }
                if ((i == 3 && value != 0) || (i == 2 && value != 0 && n_array[i + 1] == 0)) {
                    words_string += "Lakhs ";
                }
                if ((i == 5 && value != 0) || (i == 4 && value != 0 && n_array[i + 1] == 0)) {
                    words_string += "Thousand ";
                }
                if (i == 6 && value != 0 && (n_array[i + 1] != 0 && n_array[i + 2] != 0)) {
                    words_string += "Hundred and ";
                } else if (i == 6 && value != 0) {
                    words_string += "Hundred ";
                }
            }
            words_string = words_string.split("  ").join(" ");
        }
        return words_string;
    }
    if ($scope.OrderNo != undefined && $scope.OrderNo != "undefined") {

        $scope.refBGridOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: "/api/TemplateApi/getRefDetails?VoucherNo=" + $scope.OrderNo + '&formcode=' + $scope.formcode,
                },
                pageSize: 5,
                serverPaging: true,
                serverSorting: true
            },
            sortable: true,
            pageable: true,
            dataBound: function () {
                this.expandRow(this.tbody.find("tr.k-master-row").first());
            },
            columns: [{
                field: "REFERENCE_NO",
                title: "Document No",
                width: "120px"
            }, {
                field: "ITEM_EDESC",
                title: "Item",
                width: "120px"
            }, {
                field: "REFERENCE_QUANTITY",
                title: "Quantity",
                width: "120px"
            }, {
                field: "REFERENCE_MU_CODE",
                title: "Unit",
                width: "120px"
            },
            {
                field: "REFERENCE_UNIT_PRICE",
                title: "Unit Price",
                width: "120px"
            },
            {
                field: "REFERENCE_TOTAL_PRICE",
                title: "Total Price",
                width: "120px"
            },
            {
                field: "REFERENCE_CALC_UNIT_PRICE",
                title: " Calc Unit Price",
                width: "120px"
            },
            {
                field: "REFERENCE_CALC_TOTAL_PRICE",
                title: " Calc Total Price",
                width: "120px"
            },
            {
                field: "REFERENCE_REMARKS",
                title: "Remarks",
                width: "120px"
            }
            ]
        };
    }

    function GetPrimaryDateByTableName(tablename) {
        var primaryDateCol = "";
        //if (tablename == "FA_SINGLE_VOUCHER" || tablename == "FA_DOUBLE_VOUCHER") {
        //    primaryDateCol = "VOUCHER_DATE";
        //}
        //else if (tablename == "IP_PURCHASE_MRR" || tablename == "IP_ADVICE_MRR" || tablename == "IP_PRODUCTION_MRR") {
        //    primaryDateCol = "MRR_DATE";
        //}
        //else if (tablename == "IP_PURCHASE_REQUEST") {
        //    primaryDateCol = "REQUEST_DATE";
        //}
        if (tablename == "IP_PURCHASE_ORDER") {
            primaryDateCol = "ORDER_DATE";
        }
        else if (tablename == "IP_GOODS_REQUISITION") {
            primaryDateCol = "REQUISITION_DATE";
        }
        else if (tablename == "IP_GOODS_ISSUE" || tablename == "IP_RETURNABLE_GOODS_RETURN" || tablename == "IP_RETURNABLE_GOODS_ISSUE" || tablename == "IP_GATE_PASS_ENTRY" || tablename == "IP_TRANSFER_ISSUE") {
            primaryDateCol = "ISSUE_DATE";
        }
        else if (tablename == "IP_PURCHASE_MRR" || tablename == "IP_ADVICE_MRR") {
            primaryDateCol = "MRR_DATE";
        }
        else if (tablename == "IP_PURCHASE_INVOICE") {
            primaryDateCol = "INVOICE_DATE";
        }
        else if (tablename == "IP_GOODS_ISSUE_RETURN" || tablename == "IP_PURCHASE_RETURN") {
            primaryDateCol = "RETURN_DATE";
        }
        else if (tablename == "IP_PURCHASE_REQUEST") {
            primaryDateCol = "REQUEST_DATE";
        }
        else if (tablename == "IP_QUOTATION_INQUIRY") {
            primaryDateCol = "QUOTE_DATE";
        }
        return primaryDateCol;
    }
    //Get party Type
    $scope.PartyTypeDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetPartyTypes",

            },
            parameterMap: function (data, action) {

                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        }
    };

    $scope.PartyTypeCodeOption = {
        dataSource: $scope.PartyTypeDataSource,
        dataTextField: 'PARTY_TYPE_EDESC',
        dataValueField: 'PARTY_TYPE_CODE'
    };
    //charge implementation subin
    var chargeUrl = "/api/TemplateApi/GetChargeData?formCode=" + $scope.FormCode;
    $http.get(chargeUrl).then(function (response) {
        $scope.ChargeList = response.data;
        $scope.data = [];
        setTimeout(function () {
            if ($scope.ChargeList.length > 0) {

                for (var m = 0; m < $scope.ChargeList.length; m++) {


                    $('#chargacccode_' + m).data('kendoComboBox').dataSource.data([{ ACC_CODE: $scope.ChargeList[m].ACC_CODE, ACC_EDESC: $scope.ChargeList[m].ACC_EDESC, Type: "code" }]);

                }
            }
        }, 0);
    });

    $scope.checkAll = function (bool) {

        if (bool)
            $scope.data = $scope.ChargeList;
        else
            $scope.data = [];
        $scope.calculateChargeAmount($scope.data, true);
    }
    $scope.isChecked = function (CHARGE_CODE) {

        var match = false;
        if ($scope.data.length > 0) {

            for (var i = 0; i < $scope.data.length; i++) {

                if ($scope.data[i].CHARGE_CODE == CHARGE_CODE) {
                    match = true;
                }
            }
            return match;
        }
        else {

            $scope.adtotal = $scope.summary.grandTotal;
            $scope.deduction = 0.00;
            $scope.addition = 0.00;
        }

    };
    $scope.sync = function (bool, item) {

        if ($scope.OrderNo === "undefined" && $scope.youFromReference == false) {

            for (var i = 0; i < $scope.data.length; i++) {

                if ($scope.data[i].CHARGE_CODE == item.CHARGE_CODE) {


                    $scope.data.splice(i, 1);
                }
            }
            if (bool)
                $scope.data.push(item);

            $scope.calculateChargeAmount($scope.data, bool);
        }
        if ($scope.OrderNo === "undefined" && $scope.youFromReference == true) {

            $scope.rsync(bool, item);
        }
        if ($scope.OrderNo != "undefined" && $scope.youFromReference == false) {
            $scope.msync(bool, item);
        }
    };
    $scope.calculateChargeAmount = function (dataList, bool) {

        //var lista = dataList.sort((a, b) => (a.PRIORITY_INDEX_NO > b.PRIORITY_INDEX_NO) ? 1 : -1);
        var sortedObjs = _.sortBy(dataList, 'PRIORITY_INDEX_NO');
        var totalAddition = 0;
        var totalDeduction = 0;
        var netTotal = 0;
        $.each(sortedObjs, function (i, val) {

            var percent_amount = val.VALUE_PERCENT_AMOUNT;
            var grand_total = $scope.summary.grandTotal;
            var charge_amount = val.CHARGE_AMOUNT;



            if (percent_amount === null || percent_amount === "" || percent_amount === NaN || percent_amount === undefined) {
                percent_amount = 0;
            }
            if (grand_total === null || grand_total === "" || grand_total === NaN || grand_total === undefined) {
                grand_total = 0;
            }
            if (val.VALUE_PERCENT_FLAG === 'P') {

                if (val.CHARGE_TYPE_FLAG == "D") {
                    grand_total = parseFloat($scope.summary.grandTotal) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                    var Deduction = parseFloat(((percent_amount * grand_total) / 100).toFixed(2));

                    val.CHARGE_AMOUNT = Deduction;
                    totalDeduction += Deduction;

                }
                else {
                    grand_total = parseFloat($scope.summary.grandTotal) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                    var Addition = parseFloat(((percent_amount * grand_total) / 100).toFixed(2));

                    val.CHARGE_AMOUNT = Addition;
                    totalAddition += Addition;
                }
            }
            else if (val.VALUE_PERCENT_FLAG === 'Q') {

                if (val.CHARGE_TYPE_FLAG == "D") {
                    var tiqty = 0;
                    $.each($scope.childModels, function (int, itn) {
                        tiqty = tiqty + itn.QUANTITY


                    });
                    var Deduction = parseFloat(percent_amount) * parseFloat(tiqty);

                    val.CHARGE_AMOUNT = Deduction;
                    totalDeduction += Deduction;

                }
                else {
                    var tiqty = 0;
                    $.each($scope.childModels, function (int, itn) {
                        tiqty = tiqty + itn.QUANTITY


                    });
                    var Addition = parseFloat(percent_amount) * parseFloat(tiqty);

                    val.CHARGE_AMOUNT = Addition;
                    totalAddition += Addition;
                }
            }
            else {

                if (val.CHARGE_TYPE_FLAG == "D") {
                    var Deduction = parseFloat(percent_amount);
                    val.CHARGE_AMOUNT = Deduction;
                    totalDeduction += Deduction;
                    if (charge_amount > 0) {
                        $scope.PrintDiscount = charge_amount;
                    }
                }
                else {

                    var Addition = parseFloat(percent_amount);
                    val.CHARGE_AMOUNT = Addition;
                    totalAddition += Addition;
                }
            }
        });

        $scope.deduction = totalDeduction.toFixed(2);
        $scope.addition = totalAddition.toFixed(2);
        $scope.deductionCalc = totalDeduction.toFixed(2);
        $scope.additionCalc = totalAddition.toFixed(2);
        netTotal = parseFloat(parseFloat($scope.summary.grandTotal) + parseFloat($scope.additionCalc) - parseFloat($scope.deductionCalc)).toFixed(2);

        $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;
    }


    var chargeUrlForEdit = "/api/TemplateApi/GetChargeDataForEdit?formCode=" + $scope.formcode + "&&voucherNo=" + $scope.OrderNo;
    $http.get(chargeUrlForEdit).then(function (res) {
        $scope.data = [];

        if ($scope.OrderNo != "undefined") {

            setTimeout(function () {
                if (res.data.length > 0) {

                    $scope.data = res.data;
                    //$scope.ChargeList = $scope.data;
                    $.each(res.data, function (it, val) {


                        $.each($scope.ChargeList, function (i, v) {

                            if (val.CHARGE_CODE === v.CHARGE_CODE) {
                                v.CHARGE_AMOUNT = val.CHARGE_AMOUNT;
                                //val.VALUE_PERCENT_AMOUNT = v.VALUE_PERCENT_AMOUNT;
                                if (val.VALUE_PERCENT_FLAG == "P") {
                                    if ($scope.summary.grandTotal !== 0) {
                                        val.VALUE_PERCENT_AMOUNT = (val.CHARGE_AMOUNT * 100) / $scope.summary.grandTotal;
                                    }


                                }
                                else {
                                    if (val.VALUE_PERCENT_FLAG == "V") {
                                        v.VALUE_PERCENT_AMOUNT = val.CHARGE_AMOUNT;
                                    }

                                    //val.VALUE_PERCENT_AMOUNT = val.CHARGE_AMOUNT;

                                }
                                v.ACC_CODE = val.ACC_CODE;
                            }
                        });
                    });

                    //$scope.calculateChargeAmount1($scope.data, true);
                }
            }, 0);
        }

    });
    if ($scope.OrderNo != "undefined") {
        $scope.calculateChargeAmount1 = function (dataList, bool) {

            var totalAddition = 0;
            var totalDeduction = 0;
            var netTotal = 0;

            $.each(dataList, function (i, val) {

                if (val.CHARGE_TYPE_FLAG == "D") {

                    var Deduction = parseFloat(val.CHARGE_AMOUNT);
                    totalDeduction += Deduction;
                }
                else {

                    var Addition = parseFloat(val.CHARGE_AMOUNT);
                    totalAddition += Addition;
                    //$scope.VALUE_PERCENT_AMOUNT[i] = (val.CHARGE_AMOUNT * 100) / $scope.summary.grandTotal;
                }
            });

            //$scope.deduction = isNaN(totalDeduction) ? 0 : totalDeduction.toFixed(2);
            //$scope.addition = isNaN(totalAddition) ? 0 : totalAddition.toFixed(2);
            var tded = isNaN(totalDeduction) ? 0 : totalDeduction.toFixed(2);
            var tadd = isNaN(totalAddition) ? 0 : totalAddition.toFixed(2);
            $scope.deduction = isNaN(totalDeduction) ? 0 : totalDeduction.toFixed(2);
            $scope.addition = isNaN(totalAddition) ? 0 : totalAddition.toFixed(2);
            //netTotal = parseFloat(parseFloat($scope.summary.grandTotal) + parseFloat($scope.addition) - parseFloat($scope.deduction)).toFixed(2);

            netTotal = parseFloat(parseFloat($scope.summary.grandTotal) + parseFloat(tadd) - parseFloat(tded)).toFixed(2);

            $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;

            if (tded > $scope.tadd) {

                displayPopupNotification("Deduction amount is greater than total amount.", "warning");
            }
            else {


                $scope.deduction = isNaN(totalDeduction) ? 0 : totalDeduction.toFixed(2);
                $scope.addition = isNaN(totalAddition) ? 0 : totalAddition.toFixed(2);
            }

        }
    }
    $scope.msync = function (bool, item) {
        if ($scope.OrderNo != "undefined") {
            for (var i = 0; i < $scope.data.length; i++) {
                if ($scope.data[i].CHARGE_CODE == item.CHARGE_CODE) {
                    $scope.data.splice(i, 1);
                }
            }
            if (bool)

                $scope.data.push(item);
            $scope.calculateChargeAmounteditedit($scope.data, bool);
        }
    };
    $scope.calculateChargeAmounteditedit = function (dataList, bool) {

        if ($scope.OrderNo != "undefined" /*&& $scope.youFromReference == false*/) {

            var sortedObjs = _.sortBy(dataList, 'PRIORITY_INDEX_NO');
            var totalAddition = 0;
            var totalDeduction = 0;
            var netTotal = 0;
            $.each(sortedObjs, function (i, val) {

                var percent_amount = val.VALUE_PERCENT_AMOUNT;
                var grand_total = $scope.summary.grandTotal;
                var charge_amount = val.CHARGE_AMOUNT;



                if (percent_amount === null || percent_amount === "" || percent_amount === NaN || percent_amount === undefined) {
                    percent_amount = 0;
                }
                if (grand_total === null || grand_total === "" || grand_total === NaN || grand_total === undefined) {
                    grand_total = 0;
                }
                if (val.VALUE_PERCENT_FLAG === 'P') {

                    if (val.CHARGE_TYPE_FLAG == "D") {
                        grand_total = parseFloat($scope.summary.grandTotal) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                        var Deduction = parseFloat(((percent_amount * grand_total) / 100).toFixed(2));

                        val.CHARGE_AMOUNT = Deduction;
                        totalDeduction += Deduction;

                    }
                    else {
                        if (percent_amount !== 0) {
                            grand_total = parseFloat($scope.summary.grandTotal) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                            var Addition = parseFloat(((percent_amount * grand_total) / 100).toFixed(2));

                            val.CHARGE_AMOUNT = Addition;

                        }
                        else {
                            Addition = val.CHARGE_AMOUNT;

                        }
                        totalAddition += Addition;
                    }
                }
                else if (val.VALUE_PERCENT_FLAG === 'Q') {

                    if (val.CHARGE_TYPE_FLAG == "D") {
                        var tiqty = 0;
                        $.each($scope.childModels, function (int, itn) {
                            tiqty = tiqty + itn.QUANTITY


                        });
                        if (percent_amount != 0) {
                            var Deduction = parseFloat(percent_amount) * parseFloat(tiqty);

                            val.CHARGE_AMOUNT = Deduction;
                        }
                        else {
                            Deduction = val.CHARGE_AMOUNT;
                        }
                        totalDeduction += Deduction;

                    }
                    else {
                        var tiqty = 0;
                        $.each($scope.childModels, function (int, itn) {
                            tiqty = tiqty + itn.QUANTITY


                        });
                        if (percent_amount != 0) {
                            var Addition = parseFloat(percent_amount) * parseFloat(tiqty);

                            val.CHARGE_AMOUNT = Addition;
                        }
                        else { Addition = val.CHARGE_AMOUNT; }

                        totalAddition += Addition;
                    }
                }
                else {

                    if (val.CHARGE_TYPE_FLAG == "D") {
                        var Deduction = parseFloat(charge_amount) == 0 ? parseFloat(percent_amount) : parseFloat(charge_amount);
                        //var Deduction = parseFloat(percent_amount);
                        val.CHARGE_AMOUNT = Deduction;
                        totalDeduction += Deduction;
                        if (charge_amount > 0) {
                            $scope.PrintDiscount = charge_amount;
                        }
                    }
                    else {

                        //var Addition = parseFloat(percent_amount);
                        var Addition = parseFloat(charge_amount) == 0 ? parseFloat(percent_amount) : parseFloat(charge_amount);
                        charge_amount
                        val.CHARGE_AMOUNT = Addition;
                        totalAddition += Addition;
                    }
                }
            });

            $scope.deduction = totalDeduction.toFixed(2);
            $scope.addition = totalAddition.toFixed(2);
            $scope.deductionCalc = totalDeduction.toFixed(2);
            $scope.additionCalc = totalAddition.toFixed(2);
            netTotal = parseFloat(parseFloat($scope.summary.grandTotal) + parseFloat($scope.additionCalc) - parseFloat($scope.deductionCalc)).toFixed(2);

            $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;
        }
    }
    $scope.rsync = function (bool, item) {

        if ($scope.OrderNo === "undefined" && $scope.youFromReference == true) {
            for (var i = 0; i < $scope.data.length; i++) {
                if ($scope.data[i].CHARGE_CODE == item.CHARGE_CODE) {
                    $scope.data.splice(i, 1);
                }
            }
            if (bool)

                $scope.data.push(item);
            $scope.calculateChargeAmountrefrenceedit($scope.data, bool);
        }
    };
    $scope.calculateChargeAmountrefrence = function (dataList, bool) {

        var totalAddition = 0;
        var totalDeduction = 0;
        var netTotal = 0;

        $.each(dataList, function (i, val) {

            if (val.CHARGE_TYPE_FLAG == "D") {

                var Deduction = parseFloat(val.CHARGE_AMOUNT);
                totalDeduction += Deduction;
            }
            else {

                var Addition = parseFloat(val.CHARGE_AMOUNT);
                totalAddition += Addition;
                //$scope.VALUE_PERCENT_AMOUNT[i] = (val.CHARGE_AMOUNT * 100) / $scope.summary.grandTotal;
            }
        });

        //$scope.deduction = isNaN(totalDeduction) ? 0 : totalDeduction.toFixed(2);
        //$scope.addition = isNaN(totalAddition) ? 0 : totalAddition.toFixed(2);
        var tded = isNaN(totalDeduction) ? 0 : totalDeduction.toFixed(2);
        var tadd = isNaN(totalAddition) ? 0 : totalAddition.toFixed(2);
        netTotal = parseFloat(parseFloat($scope.summary.grandTotal) + parseFloat(tadd) - parseFloat(tded)).toFixed(2);

        $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;

        if (tded > $scope.tadd) {

            displayPopupNotification("Deduction amount is greater than total amount.", "warning");
        }
        else {


            $scope.deduction = isNaN(totalDeduction) ? 0 : totalDeduction.toFixed(2);
            $scope.addition = isNaN(totalAddition) ? 0 : totalAddition.toFixed(2);
        }

    }
    $scope.calculateChargeAmountrefrenceedit = function (dataList, bool) {

        if ($scope.OrderNo === "undefined" && $scope.youFromReference == true) {

            var sortedObjs = _.sortBy(dataList, 'PRIORITY_INDEX_NO');
            var totalAddition = 0;
            var totalDeduction = 0;
            var netTotal = 0;
            $.each(sortedObjs, function (i, val) {

                var percent_amount = val.VALUE_PERCENT_AMOUNT;
                var grand_total = $scope.summary.grandTotal;
                var charge_amount = val.CHARGE_AMOUNT;



                if (percent_amount === null || percent_amount === "" || percent_amount === NaN || percent_amount === undefined) {
                    percent_amount = 0;
                }
                if (grand_total === null || grand_total === "" || grand_total === NaN || grand_total === undefined) {
                    grand_total = 0;
                }
                if (val.VALUE_PERCENT_FLAG === 'P') {

                    if (val.CHARGE_TYPE_FLAG == "D") {
                        grand_total = parseFloat($scope.summary.grandTotal) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                        var Deduction = parseFloat(((percent_amount * grand_total) / 100).toFixed(2));

                        val.CHARGE_AMOUNT = Deduction;
                        totalDeduction += Deduction;

                    }
                    else {
                        grand_total = parseFloat($scope.summary.grandTotal) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                        var Addition = parseFloat(((percent_amount * grand_total) / 100).toFixed(2));

                        val.CHARGE_AMOUNT = Addition;
                        totalAddition += Addition;
                    }
                }
                else if (val.VALUE_PERCENT_FLAG === 'Q') {

                    if (val.CHARGE_TYPE_FLAG == "D") {
                        var tiqty = 0;
                        $.each($scope.childModels, function (int, itn) {
                            tiqty = tiqty + itn.QUANTITY


                        });
                        var Deduction = parseFloat(percent_amount) * parseFloat(tiqty);

                        val.CHARGE_AMOUNT = Deduction;
                        totalDeduction += Deduction;

                    }
                    else {
                        var tiqty = 0;
                        $.each($scope.childModels, function (int, itn) {
                            tiqty = tiqty + itn.QUANTITY


                        });
                        var Addition = parseFloat(percent_amount) * parseFloat(tiqty);

                        val.CHARGE_AMOUNT = Addition;
                        totalAddition += Addition;
                    }
                }
                else {

                    if (val.CHARGE_TYPE_FLAG == "D") {
                        var Deduction = parseFloat(percent_amount);
                        val.CHARGE_AMOUNT = Deduction;
                        totalDeduction += Deduction;
                        if (charge_amount > 0) {
                            $scope.PrintDiscount = charge_amount;
                        }
                    }
                    else {

                        var Addition = parseFloat(percent_amount);
                        val.CHARGE_AMOUNT = Addition;
                        totalAddition += Addition;
                    }
                }
            });

            $scope.deduction = totalDeduction.toFixed(2);
            $scope.addition = totalAddition.toFixed(2);
            $scope.deductionCalc = totalDeduction.toFixed(2);
            $scope.additionCalc = totalAddition.toFixed(2);
            netTotal = parseFloat(parseFloat($scope.summary.grandTotal) + parseFloat($scope.additionCalc) - parseFloat($scope.deductionCalc)).toFixed(2);

            $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;
        }
    }

    $scope.getVoucherDetailForReferenceProduct = function () {
        debugger;
        var locationMRR = $scope.masterModels.TO_LOCATION_CODE;
        if ($scope.DocumentName == "IP_PRODUCTION_MRR") {

            if ($scope.masterModels.hasOwnProperty("FROM_LOCATION_CODE")) {
                var master_to_location_code = $scope.masterModels.FROM_LOCATION_CODE;
                locationMRR = $scope.masterModels.FROM_LOCATION_CODE;
                if ($(".mtolocation").hasClass("borderRed") || master_to_location_code == null || master_to_location_code == "" || master_to_location_code == undefined) {
                    displayPopupNotification("Routine Code is required", "warning");
                    return $scope.loadingBtnReset();
                }
            };

            if ($scope.masterModels.hasOwnProperty("PRODUCTION_QTY")) {
                var master_to_location_code = $scope.masterModels.PRODUCTION_QTY;
                if ($(".mtolocation").hasClass("borderRed") || master_to_location_code == null || master_to_location_code == "" || master_to_location_code == undefined) {
                    displayPopupNotification("Production Qty is required", "warning");
                    return $scope.loadingBtnReset();
                }
            };
        }
        else {
            if ($scope.masterModels.hasOwnProperty("TO_LOCATION_CODE")) {
                var master_to_location_code = $scope.masterModels.TO_LOCATION_CODE;
                locationMRR = $scope.masterModels.TO_LOCATION_CODE;
                if ($(".mtolocation").hasClass("borderRed") || master_to_location_code == null || master_to_location_code == "" || master_to_location_code == undefined) {
                    displayPopupNotification("Routine Code is required", "warning");
                    return $scope.loadingBtnReset();
                }
            };

            if ($scope.masterModels.hasOwnProperty("PRODUCTION_QTY")) {
                var master_to_location_code = $scope.masterModels.PRODUCTION_QTY;
                if ($(".mtolocation").hasClass("borderRed") || master_to_location_code == null || master_to_location_code == "" || master_to_location_code == undefined) {
                    displayPopupNotification("Production Qty is required", "warning");
                    return $scope.loadingBtnReset();
                }
            };
            if (locationMRR == null) {
                displayPopupNotification("Route  is required", "warning");
                return $scope.loadingBtnReset();
            }
        }

        var model = {

            FormCode: $scope.FormCode,
            TableName: $scope.DocumentName,
            RoutingName: locationMRR,
            //if include charge is set true if also multiple voucher no is selected, single voucher no's transaction with its charge is shown.
            Production_Qty: $scope.masterModels.PRODUCTION_QTY
            //ModuleCode: $scope.ModuleCode,
            //voucherNo: voucherNo,
            //serialNo: serialNo,
            //formCode: $scope.FormCode,
            //tableName: $("#refrenceTypeMultiSelect").val().toString()
        }
        var url = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetVoucherDetailForPRoduction";

        var response = $http({
            method: "POST",
            data: model,
            url: url,
            contentType: "application/json",
            dataType: "json"
        });
        return response.then(function (data) {
            debugger;
            response = data;

            var modulecode = $scope.ModuleCode;
            if (response.data.length <= 0) {
                hideloader();
                DisplayBarNotificationMessage(data.ExceptionMessage);
                return;
            }
            if (modulecode == undefined || modulecode == "" || modulecode == null) {
                hideloader();
                return;

            }
            else {
                if (modulecode == "04") {

                    $scope.refrenceFn(response, function () {
                        //var rowwss=$rootScope.refrencedata;

                        angular.forEach($scope.masterModels,
                            function (mvalue, mkey) {

                                if (mkey === "CUSTOMER_CODE") {
                                    var req = "/api/TemplateApi/getCustEdesc?code=" + mvalue;
                                    $http.get(req).then(function (results) {
                                        setTimeout(function () {

                                            $("#customers").data('kendoComboBox').dataSource.data([{ CustomerCode: mvalue, CustomerName: results.data, Type: "code" }]);
                                        }, 0);
                                    });
                                }
                            });
                        angular.forEach($scope.childModels, function (cval, ckey) {

                            if (cval.hasOwnProperty("ITEM_CODE")) {
                                var ireq = "/api/TemplateApi/getItemEdesc?code=" + cval.ITEM_CODE;
                                $http.get(ireq).then(function (results) {
                                    setTimeout(function () {

                                        $("#products_" + ckey).data('kendoComboBox').dataSource.data([{ ItemCode: cval.ITEM_CODE, ItemDescription: results.data, Type: "code" }]);
                                    }, 0);
                                });
                            }


                        });
                        hideloader();
                        if ($scope.freeze_master_ref_flag == "Y") {
                            $(".btn-action a").css('display', 'none')
                        }
                        if ($scope.freeze_master_ref_flag == "N") {
                            $(".btn-action a").css('display', 'inline')
                        }
                    });
                }
                else if (modulecode == "03") {

                    $scope.inventoryProductCalc(response, function () {
                        hideloader();
                        $(".btn-action a").css('display', 'none')
                    });


                }
            }

        });


        //$http.post(url).then(function (result) {
        //    
        //    response = result;
        //    $scope.refrenceFn(response);
        //});
    };
    $scope.ShowBatchTranDetail = function (index) {
        $(".serialtrackFlag_" + index).modal('toggle');
    }
    $scope.ShowBatchTrackingTranDetail = function (index) {
        $(".batchtrackflag_" + index).modal('toggle');
    }
});
//}]);

PMModule.service('inventoryservice', function (inventoryfactory, $http) {

    this.DeleteInvVoucher = function (voucherno, formcode) {
        var delRes = $http({
            method: 'POST',
            url: "/api/InventoryApi/DeleteInvVoucher?voucherno=" + voucherno + "&formcode=" + formcode,
            dataType: "JSON"
        });
        return delRes;
    };
    this.getFormDetail_ByFormCode = function (formCode, d1) {
        var formDetail = inventoryfactory.getFormDetail(formCode)
            .then(function (result) {

                d1.resolve(result);
            }, function (err) { });
    };
    this.getDraftFormDetail_ByFormCode = function (formCode, d7) {
        var formDetail = inventoryfactory.getDraftFormDetail(formCode)
            .then(function (result) {
                d7.resolve(result);
            }, function (err) { });
    };
    this.getFormSetup_ByFormCode = function (formCode, d2) {
        var formSetup = inventoryfactory.getFormSetup(formCode)
            .then(function (result) {
                d2.resolve(result);
            }, function (err) { });
    };
    this.getSalesOrderDetail_ByFormCodeAndOrderNo = function (formCode, orderno, d4) {

        var salesorderformDetail = inventoryfactory.getSalesOrderFormDetail(formCode, orderno)
            .then(function (result) {
                d4.resolve(result);
            }, function (err) { });
    };

    //draft
    this.getDraftData_ByFormCodeAndTempCode = function (formCode, tempcode) {
        return inventoryfactory.getDraftDataByFormCodeAndTempCode(formCode, tempcode);

    };

    this.getnewlygeneratedvoucherno = function (companycode, fromcode, currentdate, tablename, d6) {

        var newvoucherno = inventoryfactory.getNewOrederNumber(companycode, fromcode, currentdate, tablename)
            .then(function (result) {
                d6.resolve(result);
            }, function (err) { });
    };
    this.getnewlygeneratedvouchernoafterinsert = function () {

        return Cinventoryfactory.getNewOrederNumber(companycode, fromcode, currentdate, tablename);
    }
    this.getBudgetCodeByAccCode = function (accCode) {
        var budgetCode = inventoryfactory.getBudgetCodeByAccCode(accCode);
    }
    this.GetVouchersCount = function (FORM_CODE, TABLE_NAME) {
        var budgetCode = inventoryfactory.GetVoucherCount(FORM_CODE, TABLE_NAME);
        return budgetCode;
    }

    this.GetReferenceList = function () {
        return inventoryfactory.getReferenceList();
    }

    this.getSubledgerByAccCode = function (accCode, d5) {

        var newvoucherno = inventoryfactory.getSubledgerByAccCode(accCode, d5)
            .then(function (result) {
                d5.resolve(result);
            }, function (err) { });
    };
    this.getFormCustomSetup_ByFormCode = function (formCode, d3) {

        var formDetail = inventoryfactory.GetFormCustomSetup(formCode)
            .then(function (result) {
                d3.resolve(result);
            }, function (err) { });
    };

});

PMModule.factory('inventoryfactory', function ($http) {
    var fac = {};
    fac.getFormDetail = function (formcode) {
        var req = "/api/EntryApi/GetFormDetailSetup?formCode=";
        return $http.get(req + formcode);
    }
    fac.getDraftFormDetail = function (formcode) {
        var req = "/api/TemplateApi/GetDraftFormDetail?formCode=";
        return $http.get(req + formcode);
    }
    fac.getSubledgerByAccCode = function (accCode) {
        var req = "/api/TemplateApi/getSubledgerCodeByAccCode?accCode=";
        return $http.get(req + accCode);
    }
    fac.getFormSetup = function (formcode) {
        var req = "/api/TemplateApi/GetFormSetupByFormCode?formCode=";
        return $http.get(req + formcode);
    }
    fac.getSalesOrderFormDetail = function (formcode, orderno) {

        var req = window.location.protocol + "//" + window.location.host + "/api/EntryApi/GetSalesOrderDetailFormDetailByFormCodeAndOrderNo?formCode=" + formcode + "&orderno=" + orderno;
        //return $http.get(req + formcode);
        return $http.get(req);
    }
    fac.getNewOrederNumber = function (companycode, formcode, currentdate, tablename) {
        var req = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetNewOrderNo?companycode=" + companycode + "&formcode=" + formcode + "&currentdate=" + currentdate + "&tablename=" + tablename;
        //return $http.get(req + formcode);
        return $http.get(req);
    }

    fac.getBudgetCodeByAccCode = function (accCode) {
        var req = "/api/TemplateApi/getBudgetCodeByAccCode?accCode=";
        return $http.get(req + accCode);
    }

    fac.getReferenceList = function () {
        var req = "/api/InventoryApi/GetReferenceList";
        return $http.get(req);

    };
    fac.GetVoucherCount = function (FORM_CODE, TABLE_NAME) {
        var req = "/api/InventoryApi/GetVouchersCount?FORM_CODE=" + FORM_CODE + "&TABLE_NAME=" + TABLE_NAME;
        return $http.get(req);
    }


    fac.GetFormCustomSetup = function (formcode) {
        var req = "/api/TemplateApi/GetFormCustomSetup?formCode=";
        return $http.get(req + formcode);
    }

    fac.getDraftDataByFormCodeAndTempCode = function (formCode, tempcode) {

        var req = "/api/TemplateApi/getDraftDataByFormCodeAndTempCode?formCode=" + formCode + "&tempCode=" + tempcode;
        return $http.get(req);
    }
    return fac;
});



