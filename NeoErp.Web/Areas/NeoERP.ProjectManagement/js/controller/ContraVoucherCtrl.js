
function specialTab() {

    $('.cacccode, .acccode, .issuetype, .location, .currency, .cust, .supplier, .prority, .products, .budget').keydown(function (e) {
        if (e.keyCode == 9) {  //tab pressed
            e.preventDefault();
            var pageElems = document.querySelectorAll('input, select, textarea, button'),
                elem = e.srcElement
            focusNext = false,
                len = pageElems.length;
            for (var i = 0; i < len; i++) {
                var pe = pageElems[i];
                if (focusNext) {
                    if (pe.style.display !== 'none' && pe.type != 'hidden' && pe.disabled == false && pe.readOnly == false && pe.name != 'search') {
                        pe.focus();
                        //$(this).select();
                        break;
                    }
                } else if (pe === e.srcElement) {
                    focusNext = true;
                }
            }
        }
        if (e.keyCode == 9 && e.shiftKey) {  //tab pressed
            e.preventDefault();
            var pageElems = [].slice.call(document.querySelectorAll('input, select, textarea'), 0).reverse(),
                elem = e.srcElement
            focusNext = false,
                len = pageElems.length;
            for (var i = 0; i < len; i++) {
                var pe = pageElems[i];
                if (focusNext) {
                    if (pe.style.display !== 'none' && pe.type != 'hidden' && pe.disabled == false && pe.readOnly == false && pe.name != 'search') {
                        pe.focus();
                        break;
                    }
                } else if (pe === e.srcElement) {
                    focusNext = true;
                }
            }
        }
    });
}


PMModule.directive('nextOnTab',
    function () {
        return {
            restrict: 'A',
            link: function ($scope, selem, attrs) {
                var count = 0;

                //selem.bind('focus',
                //    function (e) {
                //        $("input").bind("focus", function () {
                //            var input = $(this);
                //            input.select();
                //        });
                //    });

                selem.bind('keydown',
                    function (e) {

                        if (count < 1) {
                            specialTab();
                            count++;
                        }
                        var code = e.keyCode || e.which;
                        if (code === 9 && !e.shiftKey) {
                            e.preventDefault();
                            var pageElems = document.querySelectorAll('input, select, textarea'),
                                elem = e.srcElement
                            focusNext = false,
                                len = pageElems.length;
                            for (var i = 0; i < len; i++) {
                                var pe = pageElems[i];
                                if (focusNext) {
                                    if (pe.style.display !== 'none' && pe.type != 'hidden' && pe.disabled == false && pe.readOnly == false) {
                                        pe.focus();
                                        break;
                                    }
                                } else if (pe === e.srcElement) {
                                    focusNext = true;

                                    if (pe.id == "nepaliDate5")
                                        if ($("#ndp-nepali-box")[0].style.display == 'block' || $("#ndp-nepali-box")[0].style.display == "")
                                            showNdpCalendarBox('nepaliDate5');//$("#nepaliDate5").trigger('onfocus')
                                }
                            }
                        }
                        if (code === 9 && e.shiftKey) {
                            e.preventDefault();
                            var pageElems = [].slice.call(document.querySelectorAll('input, select, textarea'), 0).reverse(),
                                elem = e.srcElement
                            focusNext = false,
                                len = pageElems.length;
                            for (var i = 0; i < len; i++) {
                                var pe = pageElems[i];
                                if (focusNext) {
                                    if (pe.style.display !== 'none' && pe.type != 'hidden' && pe.disabled == false && pe.readOnly == false) {
                                        pe.focus();
                                        //
                                        //if (pe.id == "englishdatedocument")
                                        //    $("#nepaliDate5").trigger('onfocus')
                                        break;
                                    }
                                } else if (pe === e.srcElement) {
                                    focusNext = true;

                                    if (pe.id == "nepaliDate5")
                                        if ($("#ndp-nepali-box")[0].style.display == 'block' || $("#ndp-nepali-box")[0].style.display == "")
                                            showNdpCalendarBox('nepaliDate5');//$("#nepaliDate5").trigger('onfocus')
                                }
                            }
                        }
                    });
            }
        }
    });


PMModule.controller('ContraVoucherCtrl', function ($scope, $rootScope, $http, $routeParams, contravoucherservice, formtemplateservice, contravoucherfactory, $window, $filter, hotkeys) {

    var param2 = $routeParams.param2;
    $scope.tempCode = "";
    const ACC_CODE = "ACC_CODE";
    const BUDGET_FLAG = "BUDGET_FLAG";
    const BUDGET_CODE = "BUDGET_CODE";
    const TRANSACTION_TYPE = "TRANSACTION_TYPE";
    const AMOUNT = "AMOUNT";
    window.globalIndex = 0;
    $scope.freeze_manual_entry_flag = false;
    $scope.BUDGET_CENTER = [];
    $scope.BUDGET_CENTER.push({
        BUDGET_CODE: "",
        AMOUNT: "",
        NARRATION: ""
    });
    $scope.ChildAccCode = [];
    $scope.dynamicExtraItemChargeModalData = [{}];
    $scope.dynamicModalData = [{
        ACC_CODE: "",
        BUDGET_FLAG: "",
        BUDGET: [{
            SERIAL_NO: 1,
            BUDGET_CODE: "",
            AMOUNT: "",
            NARRATION: "",
        }]

    }];
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
    //hotkeys.add({
    //    combo: 'ctrl+alt+p',
    //    allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
    //    callback: function (event, hotkey) {
    //        $("#PurchaseExpSheet").modal('toggle');
    //    }
    //});
    hotkeys.add({
        combo: 'ctrl+enter',
        allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        description: 'To Save Document.',
        callback: function (event, hotkey) {
            //event.preventDefault();
            var param = "";
            $scope.SaveDocumentFormData(param);
        }
    });
    // navigate to save and print tab
    hotkeys.add({
        combo: 'alt+p',
        callback: function (event, hotkey) {
            $scope.SaveDocumentFormData(3);
        }
    });
    hotkeys.add({
        combo: 'ctrl+alt+t',
        callback: function (event, hotkey) {
            // $scope.toogleToolTips();
            $('i.font-green').toggle();
        }
    });
    hotkeys.add({
        combo: 'shift+enter',
        allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        description: 'Focus on Child First Element.',
        callback: function (event, hotkey) {
            $($($($(".dynamic_child_table6")[0]).find('tr')[$($(".dynamic_child_table6")[0]).find('tr').length - 1]).find('input').not(':button,:hidden,:disabled')[0]).focus();
        }
    });
    hotkeys.add({
        combo: 'ctrl+shift+backspace',
        allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        description: 'Focus on Master First Element.',
        callback: function (event, hotkey) {

            $($(".masterDiv :input").not(':button,:hidden,:disabled')[0]).focus();
        }
    });
    hotkeys.add({
        combo: 'shift+t',
        description: 'Focus on First Tab.',
        callback: function (event, hotkey) {
            $('#myTab a[href="#tab_15_1"]').trigger('click');
        }
    });
    hotkeys.add({
        combo: 'shift+r',
        description: 'Focus on Refrence Tab.',
        callback: function (event, hotkey) {
            $('#myTab a[href="#tab_15_2"]').trigger('click');
        }
    });
    hotkeys.add({
        combo: 'shift+c',
        description: 'Focus on Custom Tab.',
        callback: function (event, hotkey) {
            $('#myTab a[href="#tab_15_3"]').trigger('click');
        }
    });
    hotkeys.add({
        combo: 'shift+d',
        description: 'Focus on Document Tab.',
        callback: function (event, hotkey) {
            $('#myTab a[href="#tab_15_4"]').trigger('click');
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
    // navigate to reset tab
    hotkeys.add({
        combo: 'alt+r',
        //allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            $scope.reset();
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
            $('#RefrenceModel').modal('toggle');
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
            SERIAL_NO: 1,
            SUB_CODE: "",
            SUB_EDESC: "",
            PARTY_TYPE_CODE: "",
            AMOUNT: "",
            PARTICULARS: "",
            REFRENCE: "",
            Descriptiontest: "",
        }]
    }];
    $scope.dynamicTDSModalData = [{
        ACC_CODE: 0,
        VOUCHER_NO: "",
        INVOICE_DATE: "",
        REMARKS: "",
        TOTAL_TDS_AMOUNT: "",
        SUPPLIER_CODE: "",
        CHILDTDS: [{
            SERIAL_NO: 1,
            SUPPLIER_CODE: "",
            ACC_CODE: "",
            TDS_TYPE_CODE: "",
            MEETING_TYPE_CODE: "",
            NET_AMOUNT: "",
            TDS_PERCENTAGE: "",
            TDS_AMOUNT: ""
        }]
    }];
    $scope.dynamicVATModalData = [{
        ACC_CODE: 0,
        INVOICE_NO: "",
        INVOICE_DATE: "",
        REMARKS: "",
        REFRENCE_NO: "",
        DOC_TYPE: "",
        TYPE: "",
        TOTAL_VAT_AMOUNT: "",
        Enable_DirectEntry: false,
        CHILDVAT: [{
            SERIAL_NO: 1,
            SUPPLIER_CODE: "",
            TAXABLE_AMOUNT: "",
            VAT_AMOUNT: ""
        }]

    }];
    $scope.dynamic_TDS_Modal_Data = [{
        TDS_ACC_CODE: 0,
        TDS_INVOICE_NO: "",
        TDS_INVOICE_DATE: "",
        TDS_REMARKS: "",
        TDS_REFRENCE_NO: "",
        TDS_DOC_TYPE: "",
        TDS_TYPE: "",
        TDS_TOTAL_VAT_AMOUNT: "",
        TDS_CHILDVAT: [{
            TDS_SERIAL_NO: 1,
            TDS_SUPPLIER_CODE: "",
            TDS_TAXABLE_AMOUNT: "",
            TDS_VAT_AMOUNT: ""
        }]

    }];

    $scope.saveAsDraft = {
        TEMPLATE_NO: "",
        TEMPLATE_EDESC: "",
        TEMPLATE_NDESC: "",
        TEMPLATE_ASSIGNEE: "",
        ASSIGNED_DATE: ""
    };

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

    $scope.formBackDays = "";


    //$scope.OrderNo = $routeParams.orderNo1 + "/" + $routeParams.orderNo2 + "/" + $routeParams.orderNo3;
    if ($routeParams.orderno != undefined) {
        $scope.OrderNo = $routeParams.orderno.split(new RegExp('_', 'i')).join('/');

    }
    else { $scope.OrderNo = "undefined"; }

    //draft 

    if ($routeParams.tempCode != undefined) {

        $scope.tempCode = $routeParams.tempCode;
    }

    $scope.formcode = $routeParams.formcode;

    document.formCode = $scope.formcode;
    var d1 = $.Deferred();
    var d2 = $.Deferred();
    var d3 = $.Deferred();
    var d4 = $.Deferred();
    var d5 = $.Deferred();
    var d6 = $.Deferred();

    $scope.productDescription = '';
    $scope.producttemp = '';
    $scope.NepaliDate = '';
    $scope.FormName = '';
    $scope.sum = '';
    $rootScope.mastervalidation = "";
    var masteracccodeforupdate = '';
    $scope.budgetCodeValidation = '';
    $scope.LedgerAmountValidation = '';
    $scope.budgetcount = '';
    $scope.VoucherCount = '';
    $scope.havRefrence = 'N';
    $scope.havRefrencedata = 'N';
    $scope.ref_form_code = "";
    $scope.freeze_master_ref_flag = 'N';
    $scope.ref_fix_qty = 'N';
    $scope.ref_fix_price = 'N';
    $scope.NEGETIVE_STOCK_FLAG = 'N';
    $scope.CodeValidation = '';//subledger code validation error message
    $scope.TDSCodeValidation = '';//TDS code validation error message
    $scope.VATCodeValidation = '';//TDS code validation error message
    $scope.subledgercount = '';
    $scope.tdsacccount = '';
    $scope.tdstype = '';
    $scope.tdscount = '';
    $scope.vatcount = '';
    $scope.DocumentName = ""; // document display name at top
    $scope.MasterFormElement = []; // for elements having master_child_flag='M'
    $scope.ChildFormElement = [{ element: [], additionalElements: $scope.BUDGET_CENTER }]; // initial child element

    $scope.accsummary = { 'drTotal': 0.00, 'crTotal': 0.00, 'diffAmount': 0.00 }

    $scope.aditionalChildFormElement = []; // a blank child element model while add button press.
    $scope.formDetail = ""; // all form_dtl elements retrived from service (contains master_child 'M' && 'C' ).
    $scope.formSetup = "";
    $scope.formCustomSetup = "";
    $scope.CustomFormElement = [];
    $scope.ModuleCode = "";
    $scope.checksubledgerforserialno = "";
    //$scope.ModuleCode = '01';
    $scope.MasterFormElementValue = [];
    $scope.SalesOrderformDetail = "";
    $scope.datePattern = /^[01][0-9]-(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)-\d{4}$/
    $scope.companycode = "";
    //^[01][0-9]-(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)-\d{4}$

    $scope.todaydate = "";
    $scope.after_delemeter_value = "NO";
    $scope.masterModels = {}; // for master model
    $scope.childModels = []; // for dynamic
    $scope.customModels = {};
    $scope.dzvouchernumber = "";
    $scope.dzvoucherdate = "";
    $scope.dzformcode = "";
    $scope.dzSessionCode = "";


    $scope.BUDGET_MODAL = [];
    $scope.BUDGET_CHILD_MODAL = [];
    $scope.SUBLEDGER_MODAL = [];
    $scope.SUBLEDGER_CHILD_MODAL = [];
    $scope.TDS_MODAL = [];
    $scope.VAT_MODAL = [];
    $scope.TDS_CHILD_MODAL = [];
    $scope.VAT_CHILD_MODAL = [];





    $scope.units = [];
    $scope.ItemInfo = [];
    var newordernumber = "";
    //masterModelTemplate
    $scope.masterModelTemplate = {};
    $scope.childModelTemplate = {};

    $scope.masterChildData = null;
    $scope.onreferenceclickshow = false;
    //$scope.Enable_DirectEntry=
    $scope.selectedProductOptions = [];
    $scope.ProductsChanged = [];
    //summary Object

    $scope.summary = { 'grandTotal': 0 }
    $scope.newgenorderno = "";
    $scope.childelementwidth = "";

    $scope.initialMasterVal = [];
    $scope.initialChildVal = [{ element: [], additionalElements: $scope.BUDGET_CENTER }];
    $scope.save = "Save";
    $scope.savecontinue = "Save & Continue";

    $scope.draftsave = "Save";
    $scope.print_header;
    $scope.print_body_col;
    $rootScope.M_ACC_CODE_DEFAULTVAL = "";
    $rootScope.Contra_C_ACC_CODE_DEFAULTVAL = "";
    $scope.printCompanyInfo = {
        companyName: '',
        address: '',
        formName: '',
        phoneNo: '',
        email: '',
        tPinVatNo: '',
        formType: '',
    }
    $scope.refpurexp = [{
        FORM_EDESC: '',
        REFERENCE_NO: ''
    }];



    $scope.checkRefrences = function () {
        var req = "/api/TemplateApi/GetRefrenceFlag?formCode=" + $scope.formcode;
        $http.get(req).then(function (results) {

            var response = results.data.FormSetupRefrence;
            $scope.havRefrence = response[0].REFERENCE_FLAG;
            $scope.RefTableName = response[0].REF_TABLE_NAME;
            $scope.ref_form_code = response[0].REF_FORM_CODE;
            $scope.freeze_master_ref_flag = response[0].FREEZE_MASTER_REF_FLAG;
            $scope.ref_fix_qty = response[0].REF_FIX_QUANTITY;
            $scope.ref_fix_price = response[0].REF_FIX_PRICE;
            $scope.freeze_manual_entry_flag = response[0].FREEZE_MANUAL_ENTRY_FLAG;
            $scope.formBackDays = response[0].FREEZE_BACK_DAYS;

        });
    }
    $scope.checkRefrences();
    $scope.refrencefinanceFn = function (response, callback) {
        debugger;
        var rows = response.data;
        var reforderno = response.data[0].ORDER_NO;
        if (rows.length > 0) {
            $scope.havRefrencedata = 'Y';
            var imageurl = [];
            if (rows[0].IMAGES_LIST != null) {
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
                            mySalesDropzone.on("addedfile", function (file) {

                                if (file.url != undefined) {
                                    file._captionLabel = Dropzone.createElement("<a class='fa fa-download dropzone-download' href='" + imageurl[i] + "' name='Download' class='dropzone_caption' return false; download></a>");
                                    file.previewElement.appendChild(file._captionLabel);
                                }
                            });
                        }
                        mySalesDropzone.emit("addedfile", mockFile);
                        mySalesDropzone.emit("thumbnail", mockFile, imageurl[i]);
                        mySalesDropzone.emit('complete', mockFile);
                        mySalesDropzone.files.push(mockFile);
                        $('.dz-details').find('img').addClass('sr-only')
                        $('.dz-remove').css("display", "none");
                    }
                }
            }


            $scope.ChildFormElement = [];
            $scope.childModels = [];
            for (var i = 0; i < rows.length; i++) {
                var tempCopy = angular.copy($scope.childModelTemplate);
                $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
                $scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));
                var mastertempCopy = angular.copy($scope.masterModelTemplate);
                var mastercopy = $scope.getObjWithKeysFromOtherObj(mastertempCopy, rows[i]);
                //$scope.masterModels[mastercopy];
                $scope.masterModels = angular.copy(mastercopy);
            }
            //$scope.muwiseQty();
            var deffered = $.Deferred();
            var grandtotalsaleOrder = formtemplateservice.getGrandTotalSalesOrder_ByFormCodeAndOrderNo(reforderno, $scope.FormCode, deffered);
            $.when(deffered).done(function (result) {

                $scope.summary.grandTotal = result.data;

                $scope.masterModels;
            });

            //$scope.someDateFn();


        } else {
            $scope.masterModels = angular.copy($scope.masterModelTemplate);
        }




        //    var tempCopy = angular.copy($scope.childModelTemplate);
        //    $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
        //$scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[0]));
        //$scope.childModels = drData;
        //debugger;
        //for (var i = 0; i < $scope.childModels.length; i++) {
        //    debugger;
        //    if ($scope.childModels[i].hasOwnProperty("ACC_CODE")) {
        //        debugger;
        //        $scope.ChildAccCode[i] = $scope.childModels[i].ACC_CODE;
        //        var ireq = "/api/TemplateApi/getACCEdesc?code=" + $scope.childModels[i].ACC_CODE;
        //        $http.get(ireq).then(function (results) {
        //            setTimeout(function () {

        //                $("#idaccount_" + i).data('kendoComboBox').dataSource.data([{ ACC_CODE: $scope.childModels[i].ACC_CODE, ACC_EDESC: results.data, Type: "code" }]);
        //                $scope.childModels[i].ACC_CODE = $("#idaccount_" + i).data('kendoComboBox').dataSource.data()[0].ACC_CODE;
        //            },1500);
        //        });
        //    }
        //}


        //}
        //$scope.BUDGET_MODAL.push({ ACC_CODE: "", BUDGET_FLAG: "", BUDGET: [] });
        //$scope.BUDGET_CHILD_MODAL.push({ SERIAL_NO: "", BUDGET_CODE: "", AMOUNT: "", NARRATION: "" });

        //$scope.SUBLEDGER_MODAL.push({ ACC_CODE: "", TRANSACTION_TYPE: "", VOUCHER_AMOUNT: "", SUBLEDGER_AMOUNT: "", REMAINING_AMOUNT: "", SUBLEDGER: [] });
        //$scope.SUBLEDGER_CHILD_MODAL.push({ SERIAL_NO: "", SUB_CODE: "", SUB_EDESC: "", AMOUNT: "", PARTICULARS: "", REFRENCE: "" });

        //$scope.TDS_MODAL.push({ ACC_CODE: "", VOUCHER_NO: "", INVOICE_DATE: "", REMARKS: "", TOTAL_TDS_AMOUNT: "", CHILDTDS: [] });
        //$scope.TDS_CHILD_MODAL.push({ SERIAL_NO: "", SUPPLIER_CODE: "", ACC_CODE: "", TDS_TYPE_CODE: "", MEETING_TYPE_CODE: "", NET_AMOUNT: "", TDS_PERCENTAGE: "", TDS_AMOUNT: "" });
        //$scope.VAT_MODAL.push({ ACC_CODE: "", INVOICE_NO: "", INVOICE_DATE: "", REMARKS: "", REFRENCE_NO: "", DOC_TYPE: "", TYPE: "", TOTAL_VAT_AMOUNT: "", CHILDVAT: [] });
        //$scope.VAT_CHILD_MODAL.push({ SERIAL_NO: "", SUPPLIER_CODE: "", TAXABLE_AMOUNT: "", VAT_AMOUNT: "" });


        $scope.dynamicModalData = [];
        $scope.dynamicSubLedgerModalData = [];
        $scope.dynamicTDSModalData = [];
        $scope.dynamicVATModalData = [];


        var subledgerModel = angular.copy($scope.SUBLEDGER_MODAL[0]);
        var subledgerChildModel = angular.copy($scope.SUBLEDGER_CHILD_MODAL[0]);

        var bubgetransModel = angular.copy($scope.BUDGET_MODAL[0]);
        var bubgetranschildModel = angular.copy($scope.BUDGET_CHILD_MODAL[0]);

        var tdsModel = angular.copy($scope.TDS_MODAL[0]);
        var subtdsChildModel = angular.copy($scope.TDS_CHILD_MODAL[0]);
        var vatModel = angular.copy($scope.VAT_MODAL[0]);
        var subvatchildModel = angular.copy($scope.VAT_CHILD_MODAL[0]);

        var config = {
            async: false
        };
        for (var i = 0; i < rows.length; i++) {

            var rowsObj = rows[i];
            //$scope.dynamicTDSModalData.push($scope.getObjWithKeysFromOtherObj(tdsModel, rowsObj));

            //$scope.dynamicVATModalData.push($scope.getObjWithKeysFromOtherObj(vatModel, rowsObj));

            $scope.dynamicSubLedgerModalData.push($scope.getObjWithKeysFromOtherObj(subledgerModel, rowsObj));

            var response = $http.get('/api/ContraVoucherApi/GetDataForSubledgerModal?VOUCHER_NO=' + rows[i].VOUCHER_NO + '&accCode=' + rows[i].ACC_CODE, config);
            response.then(function (res) {

                if (res.data.length > 0) {


                    for (var b = 0; b < $scope.dynamicSubLedgerModalData.length; b++) {

                        var voucherAmt = 0;
                        var snoo = b + 1;
                        var iii = 0;
                        if ($scope.dynamicSubLedgerModalData[b].SUBLEDGER == undefined) {
                            $scope.dynamicSubLedgerModalData[b].SUBLEDGER = [];
                        }
                        for (var a = 0; a < res.data.length; a++) {

                            //if (res.data[a].ACC_CODE == $scope.dynamicSubLedgerModalData[b].ACC_CODE && res.data[a].TRANSACTION_TYPE == $scope.dynamicSubLedgerModalData[b].TRANSACTION_TYPE && res.data[a].SERIAL_NO == snoo) {
                            if (res.data[a].ACC_CODE == $scope.dynamicSubLedgerModalData[b].ACC_CODE && res.data[a].TRANSACTION_TYPE == $scope.dynamicSubLedgerModalData[b].TRANSACTION_TYPE) {
                                voucherAmt += res.data[a].BALANCE_AMOUNT;
                                $scope.dynamicSubLedgerModalData[b].SUBLEDGER.push(subledgerChildModel);
                                $scope.dynamicSubLedgerModalData[b].SUBLEDGER[iii] = $scope.getObjWithKeysFromOtherObj(subledgerChildModel, res.data[a]);
                                $scope.dynamicSubLedgerModalData[b].SUBLEDGER_AMOUNT = voucherAmt;
                                $scope.dynamicSubLedgerModalData[b].REMAINING_AMOUNT = 0;
                                iii = iii + 1;
                            }
                        }
                    }

                }

            });


            //var response = $http.get('/api/ContraVoucherApi/GetDataForBudgetModal?VOUCHER_NO=' + rows[i].VOUCHER_NO + '&accCode=' + rows[i].ACC_CODE, config);
            //response.then(function (budgetResult) {
            //    debugger;
            //    if (budgetResult.data != "") {
            //        for (var b = 0; b < $scope.dynamicModalData.length; b++) {
            //            var bsno = b + 1;
            //            var bi = 0;
            //            if ($scope.dynamicModalData[b].BUDGET == undefined) {
            //                $scope.dynamicModalData[b].BUDGET = [];
            //            }
            //            for (var a = 0; a < budgetResult.data.length; a++) {
            //                if (budgetResult.data[a].ACC_CODE == $scope.dynamicModalData[b].ACC_CODE && budgetResult.data[a].SERIAL_NO == bsno) {
            //                    $scope.dynamicModalData[b].BUDGET.push(budgetChildModel);
            //                    $scope.dynamicModalData[b].BUDGET[bi] = $scope.getObjWithKeysFromOtherObj(budgetChildModel, budgetResult.data[a]);
            //                    bi = bi + 1;
            //                }
            //            }
            //        }
            //    }
            //});

            //var tdsresponse = $http.get('/api/ContraVoucherApi/GetDataForTDSModal?VOUCHER_NO=' + rows[i].VOUCHER_NO + '&accCode=' + rows[i].ACC_CODE, config);
            //tdsresponse.then(function (tdsres) {
            //    debugger;
            //    if (tdsres.data != "") {
            //        for (var b = 0; b < $scope.dynamicTDSModalData.length; b++) {
            //            var bsno = b + 1;
            //            var bi = 0;
            //            if ($scope.dynamicTDSModalData[b].CHILDTDS == undefined) {
            //                $scope.dynamicTDSModalData[b].CHILDTDS = [];
            //            }
            //            for (var a = 0; a < tdsres.data.length; a++) {
            //                if (tdsres.data[a].ACC_CODE == $scope.dynamicTDSModalData[b].ACC_CODE && tdsres.data[a].SERIAL_NO == bsno) {
            //                    $scope.dynamicTDSModalData[b].CHILDTDS.push(subtdsChildModel);
            //                    $scope.dynamicTDSModalData[b].CHILDTDS[bi] = $scope.getObjWithKeysFromOtherObj(subtdsChildModel, tdsres.data[a]);
            //                    bi = bi + 1;
            //                }
            //            }
            //        }
            //    }
            //});
            //var vatresponse = $http.get('/api/ContraVoucherApi/GetDataForVATModal?VOUCHER_NO=' + rows[i].VOUCHER_NO + '&accCode=' + rows[i].ACC_CODE, config);
            //vatresponse.then(function (vatres) {
            //    debugger;
            //    if (vatres.data != "") {
            //        for (var b = 0; b < $scope.dynamicVATModalData.length; b++) {
            //            var bsno = b + 1;
            //            var bi = 0;
            //            if ($scope.dynamicVATModalData[b].CHILDVAT == undefined) {
            //                $scope.dynamicVATModalData[b].CHILDVAT = [];
            //            }
            //            for (var a = 0; a < vatres.data.length; a++) {
            //                if (vatres.data[a].ACC_CODE == $scope.dynamicVATModalData[b].ACC_CODE && vatres.data[a].SERIAL_NO == bsno) {
            //                    $scope.dynamicVATModalData[b].CHILDVAT.push(subvatchildModel);
            //                    $scope.dynamicVATModalData[b].CHILDVAT[bi] = $scope.getObjWithKeysFromOtherObj(subvatchildModel, vatres.data[a]);
            //                    bi = bi + 1;
            //                }
            //            }
            //        }
            //    }

            //});
            setTimeout(function () {

                angular.forEach($scope.ChildFormElement[0].element, function (value) {

                    if (value.COLUMN_NAME != "QUANTITY" && value.COLUMN_NAME != "UNIT_PRICE" && value.COLUMN_NAME != "CHEQUE_NO") {
                        $("." + value.COLUMN_NAME + "_div input").attr('readonly', true);
                    }
                });
                $.each($scope.MasterFormElement, function (key, value) {
                    if (value['COLUMN_NAME'].indexOf('CODE') > -1) {
                        $("." + value.COLUMN_NAME + " input").attr('readonly', true);
                    }

                });
                //callback();
            }, 500);
            //subin confused
            $scope.accsum(i);


        }
        if ($scope.FormName.indexOf('Bank Payment') !== -1) {
            debugger;
            $scope.masterModels.MASTER_TRANSACTION_TYPE = "CR";
            var crData = $.grep($scope.childModels, function (e) {

                return e.TRANSACTION_TYPE != "DR";
            });
            $scope.masterModels.MASTER_ACC_CODE = crData[0].ACC_CODE;
            var drData = $.grep($scope.childModels, function (e) {

                return e.TRANSACTION_TYPE != "CR";
            });
            if ($scope.ChildFormElement.length > 1) {
                $scope.ChildFormElement.splice(0, 1);
            }
            $scope.childModels = [];
            $scope.childModels = drData;
            if ($scope.masterModels.hasOwnProperty("MASTER_ACC_CODE")) {
                var ireqq = "/api/TemplateApi/getACCEdesc?code=" + $scope.masterModels.MASTER_ACC_CODE;
                $http.get(ireqq).then(function (resultss) {
                    setTimeout(function () {

                        $("#masteracccode").data('kendoComboBox').dataSource.data([{ ACC_CODE: $scope.masterModels.MASTER_ACC_CODE, ACC_EDESC: resultss.data, Type: "code" }]);
                        $scope.masterModels.MASTER_ACC_CODE = $("#masteracccode").data('kendoComboBox').dataSource.data()[0].ACC_CODE;
                    }, 0);
                });
            }
            for (var i = 0; i < $scope.ChildFormElement.length; i++) {
                debugger;
                if ($scope.childModels[i].hasOwnProperty("ACC_CODE")) {

                    $scope.ChildAccCode[i] = $scope.childModels[i].ACC_CODE;
                    var ireq = "/api/TemplateApi/getACCEdesc?code=" + $scope.childModels[i].ACC_CODE;
                    $http.get(ireq).then(function (results) {
                        debugger;
                        setTimeout(function () {
                            debugger;
                            $("#idaccount_" + 0).data('kendoComboBox').dataSource.data([{ ACC_CODE: $scope.childModels[0].ACC_CODE, ACC_EDESC: results.data, Type: "code" }]);
                            $scope.childModels[0].ACC_CODE = $("#idaccount_" + 0).data('kendoComboBox').dataSource.data()[0].ACC_CODE;
                        }, 110);
                    });
                }
            }


        }
        hideloader();
    };
    var formDetail = contravoucherservice.getFormDetail_ByFormCode($scope.formcode, d1);
    $.when(d1).done(function (result) {

        $scope.formDetail = result.data;

        if ($scope.formDetail.length > 0) {
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

                    this.push(value);
                    if (value['COLUMN_NAME'].indexOf('DATE') > -1) {
                        $scope.masterModelTemplate[value['COLUMN_NAME']] = $filter('date')(new Date(), 'dd-MMM-yyyy');
                    } else {
                        $scope.masterModelTemplate[value['COLUMN_NAME']] = "";
                    }
                    $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE;
                    if (value['COLUMN_NAME'] === "MASTER_ACC_CODE") {


                        $rootScope.M_ACC_CODE_DEFAULTVAL = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                    }
                }
            }, $scope.MasterFormElement);

        //$.each($scope.MasterFormElement, function (key, value) {


        //});

        //collection of child elements.
        angular.forEach(values,
            function (value, key) {
                if (value.MASTER_CHILD_FLAG == 'C' && value.DISPLAY_FLAG == 'Y') {
                    this.push(value);

                    if ($scope.childModels.length == 0) {
                        $scope.childModels.push({});
                    }
                    if (value['COLUMN_NAME'].indexOf('PARTICULARS') > -1) {
                        if (value.DEFA_VALUE == null) {
                            value.DEFA_VALUE = "";
                        }
                        $scope.childModels[0]["PARTICULARS"] = value.DEFA_VALUE;
                    }
                    else if (value['COLUMN_NAME'].indexOf('TRANSACTION_TYPE') > -1) {
                        if ($scope.masterModels['MASTER_TRANSACTION_TYPE'] == "DR") {
                            $scope.childModels[0]["TRANSACTION_TYPE"] = value.DEFA_VALUE;
                        }
                        else if ($scope.masterModels['MASTER_TRANSACTION_TYPE'] == "CR") {
                            $scope.childModels[0]["TRANSACTION_TYPE"] = value.DEFA_VALUE;
                        }
                        else if ($scope.masterModels['MASTER_TRANSACTION_TYPE'] == undefined || $scope.masterModels[0]["MASTER_TRANSACTION_TYPE"] == null) {
                            value.DEFA_VALUE = "DR";
                            $scope.childModels[0]["TRANSACTION_TYPE"] = value.DEFA_VALUE;
                        }

                    }
                    if (value['COLUMN_NAME'] === "ACC_CODE") {

                        //value.DEFA_VALUE == null ? "" : $scope.childModels[0]["ITEM_CODE"] = value.DEFA_VALUE;
                        $rootScope.Contra_C_ACC_CODE_DEFAULTVAL = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                    }
                    else {
                        //$scope.childModels[0][value.COLUMN_NAME] = value.DEFA_VALUE;
                        $scope.childModelTemplate[value['COLUMN_NAME']] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                    }


                }

            },
            $scope.ChildFormElement[0].element);

        //$scope.childModelTemplate = _.sortBy($scope.childModelTemplate, 'SERIAL_NO');
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

            //if ($scope.OrderNo != undefined && $scope.OrderNo != "undefined")
            //    $scope.ChildFormElement = [];
            //$scope.childModelTemplate["SUBLEDGER"] = $scope.dynamicSubLedgerModalData[0];
            //$scope.childModelTemplate["BUDGET"] = $scope.dynamicModalData[0];

            $scope.BUDGET_MODAL.push({ ACC_CODE: "", BUDGET_FLAG: "", BUDGET: [] });
            $scope.BUDGET_CHILD_MODAL.push({ SERIAL_NO: "", BUDGET_CODE: "", AMOUNT: "", NARRATION: "" });

            $scope.SUBLEDGER_MODAL.push({ ACC_CODE: "", TRANSACTION_TYPE: "", VOUCHER_AMOUNT: "", SUBLEDGER_AMOUNT: "", REMAINING_AMOUNT: "", SUBLEDGER: [] });
            $scope.SUBLEDGER_CHILD_MODAL.push({ SERIAL_NO: "", SUB_CODE: "", SUB_EDESC: "", PARTY_TYPE_CODE: "", AMOUNT: "", PARTICULARS: "", REFRENCE: "" });

            $scope.TDS_MODAL.push({ ACC_CODE: "", VOUCHER_NO: "", INVOICE_DATE: "", REMARKS: "", TOTAL_TDS_AMOUNT: "", CHILDTDS: [] });
            $scope.TDS_CHILD_MODAL.push({ SERIAL_NO: "", SUPPLIER_CODE: "", ACC_CODE: "", TDS_TYPE_CODE: "", MEETING_TYPE_CODE: "", NET_AMOUNT: "", TDS_PERCENTAGE: "", TDS_AMOUNT: "" });
            $scope.VAT_MODAL.push({ ACC_CODE: "", INVOICE_NO: "", INVOICE_DATE: "", REMARKS: "", REFRENCE_NO: "", DOC_TYPE: "", TYPE: "", TOTAL_VAT_AMOUNT: "", CHILDVAT: [] });
            $scope.VAT_CHILD_MODAL.push({ SERIAL_NO: "", SUPPLIER_CODE: "", TAXABLE_AMOUNT: "", VAT_AMOUNT: "" });
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
                            myFinanceDropzone.on("addedfile", function (file) {
                                caption = file.caption == undefined ? "" : file.caption;
                                file._captionLabel = Dropzone.createElement("<a class='fa fa-download dropzone-download' href='" + imageurl[i] + "' name='Download' class='dropzone_caption' download></a>");
                                file.previewElement.appendChild(file._captionLabel);
                            });
                        }
                        myFinanceDropzone.emit("addedfile", mockFile);
                        myFinanceDropzone.emit("thumbnail", mockFile, imageurl[i]);
                        myFinanceDropzone.emit('complete', mockFile);
                        myFinanceDropzone.files.push(mockFile);
                        $('.dz-details').find('img').addClass('sr-only')
                        $('.dz-remove').css("display", "block");
                    }
                }
                $scope.masterModels = {};
                var masterModel = angular.copy($scope.masterModelTemplate);
                var savedData = $scope.getObjWithKeysFromOtherObj(masterModel, result.data[0]);

                $scope.masterModels = savedData;

                //to solve problem in mastercode binding for update purpose
                masteracccodeforupdate = $scope.masterModels.MASTER_ACC_CODE;
                //

                if ($scope.OrderNo != undefined && $scope.OrderNo != "undefined") {
                    $scope.ChildFormElement = [];
                    $scope.childModels = [];
                    $scope.dynamicModalData = [];
                    $scope.dynamicSubLedgerModalData = [];
                    $scope.dynamicTDSModalData = [];
                    $scope.dynamicVATModalData = [];
                    $scope.newgenorderno = "";
                    $scope.save = "Update";
                    $scope.savecontinue = "Update & Continue";
                }
                ////$scope.childModelTemplate.SERIAL_NO = "";
                for (var i = 0; i < rows.length; i++) {
                    setDataOnModal(rows, i);
                }

                $scope.accsum(0);
                hideloader();
            }
            else {
                //$scope.childModels.push(angular.copy($scope.childModelTemplate));
            }

            function setDataOnModal(rows, i) {
                debugger;
                var tempCopy = angular.copy($scope.childModelTemplate);
                ////$scope.childModelTemplate.SERIAL_NO = result.data[i].SERIAL_NO;
                $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
                $scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));
                if ($scope.masterModels.hasOwnProperty("MASTER_ACC_CODE")) {
                    var ireqq = "/api/TemplateApi/getACCEdesc?code=" + $scope.masterModels.MASTER_ACC_CODE;
                    $http.get(ireqq).then(function (resultss) {
                        setTimeout(function () {

                            $("#masteracccode").data('kendoComboBox').dataSource.data([{ ACC_CODE: $scope.masterModels.MASTER_ACC_CODE, ACC_EDESC: resultss.data, Type: "code" }]);
                            $scope.masterModels.MASTER_ACC_CODE = $("#masteracccode").data('kendoComboBox').dataSource.data()[0].ACC_CODE;
                        }, 0);
                    });
                }
                if ($scope.childModels[i].hasOwnProperty("ACC_CODE")) {

                    $scope.ChildAccCode[i] = $scope.childModels[i].ACC_CODE;
                    var ireq = "/api/TemplateApi/getACCEdesc?code=" + $scope.childModels[i].ACC_CODE;
                    $http.get(ireq).then(function (results) {
                        setTimeout(function () {

                            $("#idaccount_" + i).data('kendoComboBox').dataSource.data([{ ACC_CODE: $scope.childModels[i].ACC_CODE, ACC_EDESC: results.data, Type: "code" }]);
                            $scope.childModels[i].ACC_CODE = $("#idaccount_" + i).data('kendoComboBox').dataSource.data()[0].ACC_CODE;
                        }, 800);
                    });
                }
                var subledgerModel = angular.copy($scope.SUBLEDGER_MODAL[0]);
                var subledgerChildModel = angular.copy($scope.SUBLEDGER_CHILD_MODAL[0]);
                var tdsModel = angular.copy($scope.TDS_MODAL[0]);
                var subtdsChildModel = angular.copy($scope.TDS_CHILD_MODAL[0]);
                var vatModel = angular.copy($scope.VAT_MODAL[0]);
                var subvatchildModel = angular.copy($scope.VAT_CHILD_MODAL[0]);

                var rowsObj = rows[i];
                $scope.dynamicTDSModalData.push($scope.getObjWithKeysFromOtherObj(tdsModel, rowsObj));
                //if ($scope.dynamicTDSModalData[i].CHILDTDS == undefined) {
                //    $scope.dynamicTDSModalData[i].CHILDTDS = [];
                //    $scope.dynamicTDSModalData[i].CHILDTDS.push(subtdsChildModel);
                //}
                $scope.dynamicVATModalData.push($scope.getObjWithKeysFromOtherObj(vatModel, rowsObj));
                //if ($scope.dynamicVATModalData[i].CHILDVAT == undefined) {
                //    $scope.dynamicVATModalData[i].CHILDVAT = [];
                //    $scope.dynamicVATModalData[i].CHILDVAT.push(subvatchildModel);
                //}



                $scope.dynamicSubLedgerModalData.push($scope.getObjWithKeysFromOtherObj(subledgerModel, rowsObj));
                //if ($scope.dynamicSubLedgerModalData[i].SUBLEDGER == undefined) {
                //    $scope.dynamicSubLedgerModalData[i].SUBLEDGER = [];
                //    $scope.dynamicSubLedgerModalData[i].SUBLEDGER.push(subledgerChildModel);
                //}



                var budgetModel = angular.copy($scope.BUDGET_MODAL[0]);
                var budgetChildModel = angular.copy($scope.BUDGET_CHILD_MODAL[0]);

                $scope.dynamicModalData.push($scope.getObjWithKeysFromOtherObj(budgetModel, rowsObj));
                //if ($scope.dynamicModalData[i].BUDGET == undefined) {
                //    $scope.dynamicModalData[i].BUDGET = [];
                //    $scope.dynamicModalData[i].BUDGET.push(budgetChildModel);
                //}
                var config = {
                    async: false
                };
                var response = $http.get('/api/ContraVoucherApi/GetDataForSubledgerModal?VOUCHER_NO=' + rows[i].VOUCHER_NO + '&accCode=' + rows[i].ACC_CODE, config);
                response.then(function (res) {
                    debugger;
                    if (res.data != "") {

                        debugger;

                        //for (var a = 0; a < res.data.length; a++) {
                        //    debugger;
                        //    voucherAmt += res.data[a].BALANCE_AMOUNT;
                        //    for (var b = 0; b < $scope.dynamicSubLedgerModalData.length; b++) {
                        //        debugger;
                        //        if (res.data[a].ACC_CODE == $scope.dynamicSubLedgerModalData[b].ACC_CODE && res.data[a].TRANSACTION_TYPE == $scope.dynamicSubLedgerModalData[b].TRANSACTION_TYPE) {
                        //            debugger;
                        //            //var serialno = res.data[a].SERIAL_NO - 1;
                        //            //if (serialno == b) {
                        //            debugger;
                        //            if ($scope.dynamicSubLedgerModalData[b].SUBLEDGER == undefined) {
                        //                $scope.dynamicSubLedgerModalData[b].SUBLEDGER = [];
                        //            }

                        //            $scope.dynamicSubLedgerModalData[b].SUBLEDGER.push(subledgerChildModel);
                        //            //$scope.dynamicSubLedgerModalData[b].SUBLEDGER[a] = $scope.getObjWithKeysFromOtherObj(subledgerChildModel, res.data[a]);
                        //            $scope.dynamicSubLedgerModalData[b].SUBLEDGER[a] = $scope.getObjWithKeysFromOtherObj(subledgerChildModel, res.data[a]);
                        //            $scope.dynamicSubLedgerModalData[b].SUBLEDGER_AMOUNT = voucherAmt;
                        //            $scope.dynamicSubLedgerModalData[b].REMAINING_AMOUNT = 0;

                        //            //$scope.dynamicSubLedgerModalData[b].SERIAL_NO = b + 1;
                        //            //}
                        //        }

                        //    }
                        //}

                        for (var b = 0; b < $scope.dynamicSubLedgerModalData.length; b++) {
                            var voucherAmt = 0;
                            var snoo = b + 1;

                            var iii = 0;
                            if ($scope.dynamicSubLedgerModalData[b].SUBLEDGER == undefined) {
                                $scope.dynamicSubLedgerModalData[b].SUBLEDGER = [];
                            }
                            for (var a = 0; a < res.data.length; a++) {

                                debugger;
                                //if (res.data[a].ACC_CODE == $scope.dynamicSubLedgerModalData[b].ACC_CODE && res.data[a].TRANSACTION_TYPE == $scope.dynamicSubLedgerModalData[b].TRANSACTION_TYPE && res.data[a].SERIAL_NO == snoo) {
                                if (res.data[a].ACC_CODE == $scope.dynamicSubLedgerModalData[b].ACC_CODE && res.data[a].TRANSACTION_TYPE == $scope.dynamicSubLedgerModalData[b].TRANSACTION_TYPE) {
                                    debugger;
                                    //var iii = 0;
                                    voucherAmt += res.data[a].BALANCE_AMOUNT;
                                    $scope.dynamicSubLedgerModalData[b].SUBLEDGER.push(subledgerChildModel);
                                    $scope.dynamicSubLedgerModalData[b].SUBLEDGER[iii] = $scope.getObjWithKeysFromOtherObj(subledgerChildModel, res.data[a]);
                                    $scope.dynamicSubLedgerModalData[b].SUBLEDGER_AMOUNT = voucherAmt;
                                    $scope.dynamicSubLedgerModalData[b].REMAINING_AMOUNT = 0;
                                    iii = iii + 1;
                                }
                            }
                        }
                        //for (var u = 0; u < $scope.dynamicSubLedgerModalData[i].SUBLEDGER.length; u++) {
                        //    for (var f = 0; f < res.data.length; f++) {
                        //        $scope.dynamicSubLedgerModalData[i].SUBLEDGER[u].PARTY_TYPE_CODE = res.data[f].PARTY_TYPE_EDESC;
                        //    }
                        //}
                        //BindDealerBySubCode(res.data[0].SUB_CODE, res.data[0].SUB_EDESC, 0);

                        //for (var f = 0; f < res.data.length; f++) {
                        //    for (var u = 0; u < $scope.dynamicSubLedgerModalData[i].SUBLEDGER.length; u++) {


                        //        debugger;
                        //        setTimeout(function () {
                        //            BindDealerBySubCode(res.data[0].SUB_CODE, res.data[0].SUB_EDESC, f, u);
                        //            if ($("#subcodedealercode_"+f+"_"+u).data('kendoComboBox') != null) {
                        //            debugger;
                        //                $("#subcodedealercode_" + f + "_" + u).data('kendoComboBox').dataSource.data([{ PARTY_TYPE_CODE: res.data[f].PARTY_TYPE_CODE, PARTY_TYPE_EDESC: res.data[f].PARTY_TYPE_EDESC, Type: "code" }]);
                        //        }
                        //    }, 100);
                        //    }
                        //}
                    }

                });
                var response = $http.get('/api/ContraVoucherApi/GetDataForBudgetModal?VOUCHER_NO=' + rows[i].VOUCHER_NO + '&accCode=' + rows[i].ACC_CODE, config);
                response.then(function (budgetResult) {
                    if (budgetResult.data != "") {
                        //for (var a = 0; a < budgetResult.data.length; a++) {
                        //    for (var b = 0; b < $scope.dynamicModalData.length; b++) {
                        //        if (budgetResult.data[a].ACC_CODE == $scope.dynamicModalData[b].ACC_CODE) {
                        //            var serialno = budgetResult.data[a].SERIAL_NO - 1;
                        //            $scope.dynamicModalData[b].BUDGET[a] = $scope.getObjWithKeysFromOtherObj(budgetChildModel, budgetResult.data[a]);                                                                }
                        //    }
                        //}
                        for (var b = 0; b < $scope.dynamicModalData.length; b++) {
                            var bsno = b + 1;

                            var bi = 0;
                            if ($scope.dynamicModalData[b].BUDGET == undefined) {
                                $scope.dynamicModalData[b].BUDGET = [];
                            }
                            for (var a = 0; a < budgetResult.data.length; a++) {
                                if (budgetResult.data[a].ACC_CODE == $scope.dynamicModalData[b].ACC_CODE && budgetResult.data[a].SERIAL_NO == bsno) {
                                    $scope.dynamicModalData[b].BUDGET.push(budgetChildModel);
                                    $scope.dynamicModalData[b].BUDGET[bi] = $scope.getObjWithKeysFromOtherObj(budgetChildModel, budgetResult.data[a]);
                                    bi = bi + 1;
                                }
                            }
                        }
                    }
                });
                var tdsresponse = $http.get('/api/ContraVoucherApi/GetDataForTDSModal?VOUCHER_NO=' + rows[i].VOUCHER_NO + '&accCode=' + rows[i].ACC_CODE, config);
                tdsresponse.then(function (tdsres) {
                    if (tdsres.data != "") {
                        //for (var a = 0; a < tdsres.data.length; a++) {
                        //    for (var b = 0; b < $scope.dynamicTDSModalData.length; b++) {
                        //        if (tdsres.data[a].ACC_CODE == $scope.dynamicTDSModalData[b].ACC_CODE) {
                        //            var serialno = tdsres.data[a].SERIAL_NO - 1;
                        //            $scope.dynamicTDSModalData[b].CHILDTDS[a] = $scope.getObjWithKeysFromOtherObj(subtdsChildModel, tdsres.data[a]);
                        //        }
                        //    }
                        //}
                        for (var b = 0; b < $scope.dynamicTDSModalData.length; b++) {
                            var bsno = b + 1;

                            var bi = 0;
                            if ($scope.dynamicTDSModalData[b].CHILDTDS == undefined) {
                                $scope.dynamicTDSModalData[b].CHILDTDS = [];
                            }
                            for (var a = 0; a < tdsres.data.length; a++) {
                                if (tdsres.data[a].ACC_CODE == $scope.dynamicTDSModalData[b].ACC_CODE && tdsres.data[a].SERIAL_NO == bsno) {
                                    $scope.dynamicTDSModalData[b].CHILDTDS.push(subtdsChildModel);
                                    $scope.dynamicTDSModalData[b].CHILDTDS[bi] = $scope.getObjWithKeysFromOtherObj(subtdsChildModel, tdsres.data[a]);
                                    bi = bi + 1;
                                }
                            }
                        }
                    }
                });
                var vatresponse = $http.get('/api/ContraVoucherApi/GetDataForVATModal?VOUCHER_NO=' + rows[i].VOUCHER_NO + '&accCode=' + rows[i].ACC_CODE, config);
                vatresponse.then(function (vatres) {
                    if (vatres.data != "") {
                        //for (var a = 0; a < vatres.data.length; a++) {
                        //    for (var b = 0; b < $scope.dynamicVATModalData.length; b++) {
                        //        if (vatres.data[a].ACC_CODE == $scope.dynamicVATModalData[b].ACC_CODE) {
                        //            var serialno = vatres.data[a].SERIAL_NO - 1;
                        //            $scope.dynamicVATModalData[b].DOC_TYPE = vatres.data[a].DOC_TYPE;
                        //            $scope.dynamicVATModalData[b].TYPE = vatres.data[a].TYPE;
                        //            $scope.dynamicVATModalData[b].CHILDVAT[a] = $scope.getObjWithKeysFromOtherObj(subvatchildModel, vatres.data[a]);
                        //        }
                        //    }
                        //}
                        for (var b = 0; b < $scope.dynamicVATModalData.length; b++) {
                            var bsno = b + 1;

                            var bi = 0;
                            if ($scope.dynamicVATModalData[b].CHILDVAT == undefined) {
                                $scope.dynamicVATModalData[b].CHILDVAT = [];
                            }
                            for (var a = 0; a < vatres.data.length; a++) {
                                if (vatres.data[a].ACC_CODE == $scope.dynamicVATModalData[b].ACC_CODE && vatres.data[a].SERIAL_NO == bsno) {
                                    $scope.dynamicVATModalData[b].CHILDVAT.push(subvatchildModel);
                                    $scope.dynamicVATModalData[b].CHILDVAT[bi] = $scope.getObjWithKeysFromOtherObj(subvatchildModel, vatres.data[a]);
                                    bi = bi + 1;
                                }
                            }
                        }
                    }
                });
                if ($scope.OrderNo != undefined) {
                    var chargeResponse = $http.get(window.location.protocol + "//" + window.location.host + '/api/ContraVoucherApi/GetChargeExpList?voucherNo=' + $scope.OrderNo + '&accCode=' + rows[i].ACC_CODE);
                    chargeResponse.then(function (result) {

                        $scope.dynamicExtraItemChargeModalData[i] = result.data;
                        if ($scope.dynamicExtraItemChargeModalData[i] == undefined) {
                            $scope.dynamicExtraItemChargeModalData[i] = "";
                        }
                    });
                }
                if ($scope.OrderNo != "undefined") {

                    $scope.refPESGridOptions = {
                        dataSource: {
                            type: "json",

                            transport: {
                                read: "/api/ContraVoucherApi/GetReferenceListPES?voucherno=" + $scope.OrderNo,
                            },
                            pageSize: 5,
                            serverPaging: false,
                            serverSorting: false
                        },
                        sortable: false,
                        pageable: false,
                        dataBound: function () {
                            this.expandRow(this.tbody.find("tr.k-master-row").first());
                        },
                        columns: [{
                            field: "FORM_EDESC",
                            title: "Document",
                            width: "60px"
                        }, {
                            field: "REFERENCE_NO",
                            title: "Voucher No",
                            width: "60px"
                        }
                        ]
                    };
                }
            }
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
        dt.resolve($scope.MasterFormElement);

    });

    $scope.someFn = function () {

        angular.forEach($scope.MasterFormElement, function (value, key) {
            if (value.MASTER_CHILD_FLAG == 'M' && value.DISPLAY_FLAG == 'Y') {
                $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                if (value['COLUMN_NAME'].indexOf('DATE') > -1) {
                    $scope.masterModels[value['COLUMN_NAME']] = $filter('date')(new Date(), 'dd-MMM-yyyy');
                }

            }
        })
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

        })
        //if ($scope.OrderNo != "undefined") {
        //    angular.forEach($scope.childModels, function (valuec, keyc) {

        //        if ($scope.childModels[keyc].hasOwnProperty("ACC_CODE")) {
        //            var ireq = "/api/TemplateApi/getACCEdesc?code=" + $scope.childModels[keyc].ACC_CODE;
        //            $http.get(ireq).then(function (results11) {
        //                setTimeout(function () {
        //                    if (results11.data != "") {
        //                        $("#idaccount_" + keyc).data('kendoComboBox').dataSource.data([{ ACC_CODE: $scope.childModels[keyc].ACC_CODE, ACC_EDESC: results11.data, Type: "code" }]);
        //                    }

        //                }, 50);
        //            });
        //        }
        //    });
        //}

        var response = $http.get('/api/TemplateApi/GetNewOrderNo?companycode=' + $scope.companycode + '&formcode=' + $scope.formcode + '&currentdate=' + $scope.todaydate + '&tablename=' + $scope.DocumentName + '&isSequence=true');

        response.then(function (res) {

            if (res.data != "0") {

                //$scope.masterModels["VOUCHER_NO"] = res.data;
                $scope.masterModels["VOUCHER_NO"] = $scope.masterModels["VOUCHER_NO"] == "" ? res.data : $scope.masterModels["VOUCHER_NO"];
                $scope.masterModels["VOUCHER_DATE"] = $scope.masterModels["VOUCHER_DATE"] == "" ? $filter('date')(new Date(), 'dd-MMM-yyyy') : $scope.masterModels["VOUCHER_DATE"];
                var engdate = $scope.masterModels["VOUCHER_DATE"];

                var formatedengdate = moment(engdate).format('DD-MMM-YYYY');
                var a = ConvertEngDateToNep(formatedengdate);
                $scope.NepaliDate = a;
                $("#nepaliDate5").val(a);
                //$scope.CountVoucherTotal();

            }

        });
        //if ($scope.masterModels.hasOwnProperty("MASTER_ACC_CODE")) {
        //    var ireqqq = "/api/TemplateApi/getACCEdesc?code=" + $scope.masterModels.MASTER_ACC_CODE;
        //    $http.get(ireqqq).then(function (resultsss) {
        //        setTimeout(function () {

        //            $("#masteracccode").data('kendoComboBox').dataSource.data([{ ACC_CODE: $scope.masterModels.MASTER_ACC_CODE, ACC_EDESC: resultsss.data, Type: "code" }]);
        //            $scope.masterModels.MASTER_ACC_CODE = $("#masteracccode").data('kendoComboBox').dataSource.data()[0].ACC_CODE;
        //        }, 0);
        //    });
        //}
        //angular.forEach($scope.childModels, function (value, key) {
        //    if ($scope.childModels[key].hasOwnProperty("ACC_CODE")) {
        //        var ireqqs = "/api/TemplateApi/getACCEdesc?code=" + $scope.childModels[key].ACC_CODE;
        //        $http.get(ireqqs).then(function (resultsqs) {
        //            setTimeout(function () {

        //                $("#idaccount_" + key).data('kendoComboBox').dataSource.data([{ ACC_CODE: $scope.childModels[key].ACC_CODE, ACC_EDESC: resultsqs.data, Type: "code" }]);
        //                $scope.childModels[key].ACC_CODE = $("#idaccount_" + key).data('kendoComboBox').dataSource.data()[key].ACC_CODE;
        //            }, 0);
        //        });
        //    }
        //});
    }
    $scope.someFnCP = function () {

        angular.forEach($scope.MasterFormElement, function (value, key) {
            if (value.MASTER_CHILD_FLAG == 'M' && value.DISPLAY_FLAG == 'Y') {
                $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                if (value['COLUMN_NAME'].indexOf('DATE') > -1) {
                    $scope.masterModels[value['COLUMN_NAME']] = $filter('date')(new Date(), 'dd-MMM-yyyy');
                }

            }
        })
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

        })
        //if ($scope.OrderNo != "undefined") {
        //    angular.forEach($scope.childModels, function (valuec, keyc) {

        //        if ($scope.childModels[keyc].hasOwnProperty("ACC_CODE")) {
        //            var ireq = "/api/TemplateApi/getACCEdesc?code=" + $scope.childModels[keyc].ACC_CODE;
        //            $http.get(ireq).then(function (results11) {
        //                setTimeout(function () {
        //                    if (results11.data != "") {
        //                        $("#idaccount_" + keyc).data('kendoComboBox').dataSource.data([{ ACC_CODE: $scope.childModels[keyc].ACC_CODE, ACC_EDESC: results11.data, Type: "code" }]);
        //                    }

        //                }, 50);
        //            });
        //        }
        //    });
        //}

        var response = $http.get('/api/TemplateApi/GetNewOrderNo?companycode=' + $scope.companycode + '&formcode=' + $scope.formcode + '&currentdate=' + $scope.todaydate + '&tablename=' + $scope.DocumentName + '&isSequence=true');

        response.then(function (res) {

            if (res.data != "0") {

                //$scope.masterModels["VOUCHER_NO"] = res.data;
                $scope.masterModels["VOUCHER_NO"] = $scope.masterModels["VOUCHER_NO"] == "" ? res.data : $scope.masterModels["VOUCHER_NO"];
                $scope.masterModels["VOUCHER_DATE"] = $scope.masterModels["VOUCHER_DATE"] == "" ? $filter('date')(new Date(), 'dd-MMM-yyyy') : $scope.masterModels["VOUCHER_DATE"];
                var engdate = $scope.masterModels["VOUCHER_DATE"];

                var formatedengdate = moment(engdate).format('DD-MMM-YYYY');
                var a = ConvertEngDateToNep(formatedengdate);
                $scope.NepaliDate = a;
                $("#nepaliDate5").val(a);
                //$scope.CountVoucherTotal();

            }

        });
        if ($scope.masterModels.hasOwnProperty("MASTER_ACC_CODE")) {
            var ireqqq = "/api/TemplateApi/getACCEdesc?code=" + $scope.masterModels.MASTER_ACC_CODE;
            $http.get(ireqqq).then(function (resultsss) {
                setTimeout(function () {

                    $("#masteracccode").data('kendoComboBox').dataSource.data([{ ACC_CODE: $scope.masterModels.MASTER_ACC_CODE, ACC_EDESC: resultsss.data, Type: "code" }]);
                    $scope.masterModels.MASTER_ACC_CODE = $("#masteracccode").data('kendoComboBox').dataSource.data()[0].ACC_CODE;
                }, 0);
            });
        }
        angular.forEach($scope.childModels, function (value, key) {
            if ($scope.childModels[key].hasOwnProperty("ACC_CODE")) {
                var ireqqs = "/api/TemplateApi/getACCEdesc?code=" + $scope.childModels[key].ACC_CODE;
                $http.get(ireqqs).then(function (resultsqs) {
                    setTimeout(function () {

                        $("#idaccount_" + key).data('kendoComboBox').dataSource.data([{ ACC_CODE: $scope.childModels[key].ACC_CODE, ACC_EDESC: resultsqs.data, Type: "code" }]);
                        $scope.childModels[key].ACC_CODE = $("#idaccount_" + key).data('kendoComboBox').dataSource.data()[0].ACC_CODE;
                    }, 0);
                });
            }
        });
    }

    $scope.SetDefaulValueForMasters = function () {
        $.each($scope.MasterFormElement, function (key, value) {

            $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE;
        });
    }
    $scope.refrencePES = function (response, callback) {

        $scope.refpurexp = response;
        if (response.length > 0) {
            $scope.refPESGridOptions = {
                dataSource: {
                    type: "json",

                    data: response,
                    pageSize: 5,
                    serverPaging: false,
                    serverSorting: false
                },
                sortable: false,
                pageable: false,
                dataBound: function () {
                    this.expandRow(this.tbody.find("tr.k-master-row").first());
                },
                columns: [{
                    field: "FORM_EDESC",
                    title: "Document",
                    width: "60px"
                }, {
                    field: "REFERENCE_NO",
                    title: "Voucher No",
                    width: "60px"
                }
                ]
            };

        }


    };

    //change VoucherDate Eng to Nepali
    $scope.ConvertEngToNep = function () {
        console.log(this);

        var engdate = $("#englishDate5").val();
        var nepalidate = ConvertEngDateToNep(engdate);
        $("#nepaliDate5").val(nepalidate);
    };

    $scope.ConvertNepToEng = function (keyCode) {
        //
        var date = BS2AD($("#nepaliDate5").val());
        $("#englishdatedocument").val($filter('date')(date, "dd-MMM-yyyy"));
        //if (keyCode == 9 )
        //    showNdpCalendarBox('nepaliDate5');
        //$('#nepaliDate5').trigger('change')
    };

    $scope.ConvertEngToNepang = function (data) {
        console.log(data);
        $("#nepaliDate5").val(AD2BS(data));
    };
    //change VoucherDate Nepali To English


    var index = 0;
    var accCode = "";
    window.accCode = "";
    var popupAccess = false;
    var popupAccessTds = false;
    var popupAccessVAT = false;
    var popupAccessBudget = false;

    $scope.add_child_element = function (e) {

        if ($scope.childModels[e].hasOwnProperty("Display_BFlag")) {
            if ($scope.childModels[e].Display_BFlag === "T") {
                var csum = 0;
                var cbudgetLength = $scope.dynamicModalData[e].BUDGET.length;
                for (var i = 0; i < cbudgetLength; i++) {
                    var camt = $scope.dynamicModalData[index].BUDGET[i].AMOUNT;

                    if (camt != null && camt != "" && camt !== undefined) {
                        $scope.dynamicModalData[index].BUDGET[i].AMOUNT = parseFloat(camt.toFixed(2));
                        csum += parseFloat(camt.toFixed(2));
                    }
                }
                if (csum !== $scope.childModels[e].AMOUNT) {
                    displayPopupNotification("Value in Amount and Ledger Budget Amount is not  same", "warning");
                    $(".AMOUNT_" + index).parent().css({ "border": "solid 1px red" });
                    e.preventDefault();
                    e.stopPropagation();
                    return false;
                }
            }
        }

        //$scope.BUDGET_CENTER.BUDGET.ID = $scope.dynamicModalData.length;
        var budgetserialno = $scope.dynamicModalData.length + 1;
        $scope.dynamicModalData.push({
            ACC_CODE: "",
            BUDGET_FLAG: "",
            BUDGET: [{
                SERIAL_NO: budgetserialno,
                BUDGET_CODE: "",
                AMOUNT: "",
                NARRATION: ""
            }]
        });
        var serialno = $scope.dynamicSubLedgerModalData.length + 1;
        $scope.dynamicSubLedgerModalData.push({
            ACC_CODE: 0,
            TRANSACTION_TYPE: "",
            VOUCHER_AMOUNT: "",
            SUBLEDGER_AMOUNT: "",
            REMAINING_AMOUNT: "",
            SUBLEDGER: [{
                SERIAL_NO: serialno,
                SUB_CODE: "",
                SUB_EDESC: "",
                PARTY_TYPE_CODE: "",
                AMOUNT: "",
                PARTICULARS: "",
                REFRENCE: ""
            }]
        });
        var serialno = $scope.dynamicTDSModalData.length + 1;
        $scope.dynamicTDSModalData.push({
            ACC_CODE: 0,
            VOUCHER_NO: "",
            INVOICE_DATE: "",
            REMARKS: "",
            TOTAL_TDS_AMOUNT: "",
            SUPPLIER_CODE: "",
            CHILDTDS: [{
                SERIAL_NO: 1,
                SUPPLIER_CODE: "",
                ACC_CODE: "",
                TDS_TYPE_CODE: "",
                MEETING_TYPE_CODE: "",
                NET_AMOUNT: "",
                TDS_PERCENTAGE: "",
                TDS_AMOUNT: ""
            }]
        });
        var serialno = $scope.dynamicVATModalData.length + 1;
        $scope.dynamicVATModalData.push({
            ACC_CODE: 0,
            INVOICE_NO: "",
            INVOICE_DATE: "",
            REMARKS: "",
            REFRENCE_NO: "",
            DOC_TYPE: "",
            TYPE: "",
            Enable_DirectEntry: false,
            TOTAL_VAT_AMOUNT: "",
            CHILDVAT: [{
                SERIAL_NO: serialno,
                SUPPLIER_CODE: "",
                TAXABLE_AMOUNT: "",
                VAT_AMOUNT: "",
            }]
        });

        $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement, additionalElements: $scope.BUDGET_CENTER });
        $scope.childModels.push(angular.copy($scope.childModelTemplate));
        var childLen = $scope.childModels.length - 1;

        $.each($scope.ChildFormElement[0].element, function (childkey, childelementvalue) {
            if (childelementvalue.DEFA_VALUE != null)
                $scope.childModels[childLen][childelementvalue.COLUMN_NAME] = childelementvalue.DEFA_VALUE;
        });


        ////$.each($scope.childModels, function (key, value) {
        //$.each($scope.ChildFormElement[0].element, function (childkey, childelementvalue) {
        //    
        //        if (childelementvalue.DEFA_VALUE != null) //{
        //            $scope.childModels[childLen][childelementvalue.COLUMN_NAME] = childelementvalue.DEFA_VALUE;
        //            //$scope.childModels[key][childelementvalue.COLUMN_NAME] = "";
        //        //}
        //        //else {

        //        //    $scope.childModels[childLen][childelementvalue.COLUMN_NAME] = childelementvalue.DEFA_VALUE;
        //        //    //var Particularvalue = $scope.childModels[key].PARTICULARS;
        //        //    //if (Particularvalue == null || Particularvalue == undefined || Particularvalue == "") {
        //        //    //    $scope.childModels[key][childelementvalue.COLUMN_NAME] = childelementvalue.DEFA_VALUE;
        //        //    //}
        //        //}


        //    });
        ////});


    }
    $scope.ShowRefrenceFinance = function () {

        if ($scope.havRefrence == 'Y') {
            //$('#IncludeCharge').val('').prop('disabled', false);
            $('#RefrenceModel').modal('show');
            //$('#refrenceTypeMultiSelect').data("kendoDropDownList").value($scope.ref_form_code);
            //$('#TemplateTypeMultiSelect').data("kendoDropDownList").value($scope.ref_form_code);
        }


    }
    $scope.add_childSubledger_element = function (e) {

        if ($scope.subledgercount === true) {
            displayPopupNotification("Same Code Or Subledger cannot be selected", "warning");
            e.preventDefault();
            e.stopPropagation();
            return false;
        }
        var subledgerindex = window.globalIndex;
        $scope.dynamicSubLedgerModalData[subledgerindex].SUBLEDGER.push({
            SERIAL_NO: subledgerindex + 1,
            SUB_CODE: "",
            SUB_EDESC: "",
            PARTY_TYPE_CODE: "",
            AMOUNT: "",
            PARTICULARS: "",
            REFRENCE: ""
        });

    }
    $scope.add_childtds_element = function (e) {

        if ($scope.tdscount === true) {
            displayPopupNotification("Same Code Or tds cannot be selected", "warning");
            e.preventDefault();
            e.stopPropagation();
            return false;
        }
        var tdsindex = window.globalIndex;
        $scope.dynamicTDSModalData[tdsindex].CHILDTDS.push({
            SERIAL_NO: tdsindex + 1,
            SUPPLIER_CODE: "",
            ACC_CODE: "",
            TDS_TYPE_CODE: "",
            MEETING_TYPE_CODE: "",
            NET_AMOUNT: "",
            TDS_PERCENTAGE: "",
            TDS_AMOUNT: ""
        });

    }

    $scope.add_childvat_element = function (e) {

        if ($scope.vatcount === true) {
            displayPopupNotification("Same Code Or vat cannot be selected", "warning");
            e.preventDefault();
            e.stopPropagation();
            return false;
        }
        var vatindex = window.globalIndex;
        $scope.dynamicVATModalData[vatindex].CHILDVAT.push({
            SERIAL_NO: vatindex + 1,
            SUPPLIER_CODE: "",
            TAXABLE_AMOUNT: "",
            VAT_AMOUNT: ""
        });

    }


    $scope.add_childbudgetflag_element = function (e) {
        if ($scope.budgetcount === true) {
            displayPopupNotification("You cannot select same Budget Center", "warning");
            e.preventDefault();
            e.stopPropagation();
            return false;
        }

        $scope.dynamicModalData[index].BUDGET.push({
            SERIAL_NO: index + 1,
            BUDGET_CODE: "",
            AMOUNT: "",
            NARRATION: ""
        });

    }

    $scope.remove_childSubledger_element = function (key, index, VOUCHER_AMOUNT) {
        if ($scope.dynamicSubLedgerModalData[key].SUBLEDGER.length > 1) {
            $scope.dynamicSubLedgerModalData[key].SUBLEDGER.splice(index, 1);
            $scope.Change(key, VOUCHER_AMOUNT);
            $scope.CodeValidation = '';
            $scope.subledgercount = false;
        }
    }
    $scope.remove_childtds_element = function (key, index) {

        if ($scope.dynamicTDSModalData[key].CHILDTDS.length > 1) {
            $scope.dynamicTDSModalData[key].CHILDTDS.splice(index, 1);
            //$scope.Change(key, VOUCHER_AMOUNT);
            $scope.TDSCodeValidation = '';
            $scope.tdscount = false;
        }
    }
    $scope.remove_childvat_element = function (key, index) {

        if ($scope.dynamicVATModalData[key].CHILDVAT.length > 1) {
            $scope.dynamicVATModalData[key].CHILDVAT.splice(index, 1);
            //$scope.Change(key, VOUCHER_AMOUNT);
            $scope.CodeValidation = '';
            $scope.vatcount = false;
        }
    }
    //On Voucher Amount Change
    $scope.ChangeVoucherAmount = function (VOUCHER_AMOUNT) {
        $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT = $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT;
        $scope.dynamicSubLedgerModalData[index].REMAINING_AMOUNT = VOUCHER_AMOUNT - $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT;
    }


    //on budget Amount Change
    $scope.BudgetChange = function (index, BUDGET_AMOUNT) {

        var sum = 0;
        var budgetLength = $scope.dynamicModalData[index].BUDGET.length;
        for (var i = 0; i < budgetLength; i++) {
            var amt = $scope.dynamicModalData[index].BUDGET[i].AMOUNT;

            if (amt != null && amt != "" && amt !== undefined) {
                $scope.dynamicModalData[index].BUDGET[i].AMOUNT = parseFloat(amt.toFixed(2));
                sum += parseFloat(amt.toFixed(2));
            }


            if (amt === undefined) {
                $(".BUDGETAMOUNT_" + i).addClass("borderRed");
            }
            else {
                $(".BUDGETAMOUNT_" + i).removeClass("borderRed");

            }

            if ($("#BudgetAmount").hasClass("borderRed")) {
                $("#BudgetOk").addClass("disabled");
            }
            else {

                if ($scope.LedgerAmountValidation === "") {
                    $("#BudgetOk").removeClass("disabled");
                }
            }

        };
        $scope.sum = sum;
        //if (sum !== $scope.childModels[index].AMOUNT) {
        //    $("#BudgetOk").addClass("disabled");
        //    }
        //else {
        //    $("#BudgetOk").removeClass("disabled");
        //}
    }

    $scope.someDateFn = function () {
        var engdate = $filter('date')(new Date(), 'dd-MMM-yyyy');
        var a = ConvertEngDateToNep(engdate);
        $scope.NepaliDate = a;
        $("#nepaliDate5").val(a);

    };

    //On SubLedger Amount Change
    $scope.Change = function (index, VOUCHER_AMOUNT) {
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
            if (amt != null && amt != "" && amt !== undefined) {
                $scope.dynamicSubLedgerModalData[index].SUBLEDGER[i].AMOUNT = parseFloat(amt.toFixed(2));
            }


            if (amt === undefined) {
                $(".SUBLEDGERAMOUNT_" + i).addClass("borderRed");
            }
            else {
                $(".SUBLEDGERAMOUNT_" + i).removeClass("borderRed");

            }

            if ($(".subledgeramounts").hasClass("borderRed")) {
                $(".subledgerok").addClass("disabled");
            }
            else {
                $(".subledgerok").removeClass("disabled");
            }

            if (amt != "") {
                var sa = parseFloat($scope.dynamicSubLedgerModalData[index].SUBLEDGER[i].AMOUNT);
                if (isNaN(sa)) {
                    sa = 0;
                }
                subledgeramount = parseFloat((subledgeramount + sa).toFixed(2));
            }
            else {
                subledgeramount = subledgeramount;
            }
            $scope.apply;
        }
        $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT = (subledgeramount);
        $scope.dynamicSubLedgerModalData[index].REMAINING_AMOUNT = parseFloat((VOUCHER_AMOUNT - subledgeramount).toFixed(2));

        if ($scope.dynamicSubLedgerModalData[index].REMAINING_AMOUNT < 0) {
            $('.remainingamt').addClass("borderred");
        }
        else {
            $scope.errorshow = "";
            $('.remainingamt').removeClass("borderred");
        }

    }
    $scope.calculatetdsamount = function (key, index) {

        var tds_netamount = 0;
        var tds_percentage = 0;
        if (typeof $scope.dynamicTDSModalData[key].CHILDTDS[index].NET_AMOUNT !== 'undefined' && $scope.dynamicTDSModalData[key].CHILDTDS[index].NET_AMOUNT !== null && $scope.dynamicTDSModalData[key].CHILDTDS[index].NET_AMOUNT !== "") {

            tds_netamount = $scope.dynamicTDSModalData[key].CHILDTDS[index].NET_AMOUNT;
        }
        if (typeof $scope.dynamicTDSModalData[key].CHILDTDS[index].TDS_PERCENTAGE !== 'undefined' && $scope.dynamicTDSModalData[key].CHILDTDS[index].TDS_PERCENTAGE !== null && $scope.dynamicTDSModalData[key].CHILDTDS[index].TDS_PERCENTAGE !== "") {

            tds_percentage = $scope.dynamicTDSModalData[key].CHILDTDS[index].TDS_PERCENTAGE;
        }
        var tds_amount = (parseFloat(tds_netamount) / 100) * parseFloat(tds_percentage);
        $scope.dynamicTDSModalData[key].CHILDTDS[index].TDS_AMOUNT = (parseFloat(tds_amount)).toFixed(2);
        var totaltdssum = 0;

        $.each($scope.dynamicTDSModalData[key].CHILDTDS, function (key, value) {

            totaltdssum += parseFloat(value.TDS_AMOUNT);
        });
        $scope.dynamicTDSModalData[key].TOTAL_TDS_AMOUNT = parseFloat(totaltdssum).toFixed(2);
    };

    $scope.calculatevatamount = function (key, index) {

        //var tds_netamount = 0;
        //var tds_percentage = 0;
        //if (typeof $scope.dynamicTDSModalData[key].CHILDTDS[index].NET_AMOUNT !== 'undefined' && $scope.dynamicTDSModalData[key].CHILDTDS[index].NET_AMOUNT !== null && $scope.dynamicTDSModalData[key].CHILDTDS[index].NET_AMOUNT !== "") {

        //    tds_netamount = $scope.dynamicTDSModalData[key].CHILDTDS[index].NET_AMOUNT;
        //}
        //if (typeof $scope.dynamicTDSModalData[key].CHILDTDS[index].TDS_PERCENTAGE !== 'undefined' && $scope.dynamicTDSModalData[key].CHILDTDS[index].TDS_PERCENTAGE !== null && $scope.dynamicTDSModalData[key].CHILDTDS[index].TDS_PERCENTAGE !== "") {

        //    tds_percentage = $scope.dynamicTDSModalData[key].CHILDTDS[index].TDS_PERCENTAGE;
        //}
        //var tds_amount = (parseFloat(tds_netamount) / 100) * parseFloat(tds_percentage);
        //$scope.dynamicTDSModalData[key].CHILDTDS[index].TDS_AMOUNT = (parseFloat(tds_amount)).toFixed(2);
        //var totaltdssum = 0;

        //$.each($scope.dynamicTDSModalData[key].CHILDTDS, function (key, value) {

        //    totaltdssum += parseFloat(value.TDS_AMOUNT);
        //});
        //$scope.dynamicTDSModalData[key].TOTAL_TDS_AMOUNT = parseFloat(totaltdssum).toFixed(2);
        var taxable_amount = 0;
        if (typeof $scope.dynamicVATModalData[key].CHILDVAT[index].TAXABLE_AMOUNT !== 'undefined' && $scope.dynamicVATModalData[key].CHILDVAT[index].TAXABLE_AMOUNT !== null && $scope.dynamicVATModalData[key].CHILDVAT[index].TAXABLE_AMOUNT !== "") {

            taxable_amount = $scope.dynamicVATModalData[key].CHILDVAT[index].TAXABLE_AMOUNT;
        }
        var vat_amount = (parseFloat(taxable_amount) / 100) * 13;
        $scope.dynamicVATModalData[key].CHILDVAT[index].VAT_AMOUNT = (parseFloat(vat_amount)).toFixed(2);
        var totalvatsum = 0;

        $.each($scope.dynamicVATModalData[key].CHILDVAT, function (key, value) {

            totalvatsum += parseFloat(value.VAT_AMOUNT);
        });
        $scope.dynamicVATModalData[key].TOTAL_VAT_AMOUNT = parseFloat(totalvatsum).toFixed(2);
    };
    $scope.calculateBaseamount = function (key, index) {

        //var tds_netamount = 0;
        //var tds_percentage = 0;
        //if (typeof $scope.dynamicTDSModalData[key].CHILDTDS[index].NET_AMOUNT !== 'undefined' && $scope.dynamicTDSModalData[key].CHILDTDS[index].NET_AMOUNT !== null && $scope.dynamicTDSModalData[key].CHILDTDS[index].NET_AMOUNT !== "") {

        //    tds_netamount = $scope.dynamicTDSModalData[key].CHILDTDS[index].NET_AMOUNT;
        //}
        //if (typeof $scope.dynamicTDSModalData[key].CHILDTDS[index].TDS_PERCENTAGE !== 'undefined' && $scope.dynamicTDSModalData[key].CHILDTDS[index].TDS_PERCENTAGE !== null && $scope.dynamicTDSModalData[key].CHILDTDS[index].TDS_PERCENTAGE !== "") {

        //    tds_percentage = $scope.dynamicTDSModalData[key].CHILDTDS[index].TDS_PERCENTAGE;
        //}
        //var tds_amount = (parseFloat(tds_netamount) / 100) * parseFloat(tds_percentage);
        //$scope.dynamicTDSModalData[key].CHILDTDS[index].TDS_AMOUNT = (parseFloat(tds_amount)).toFixed(2);
        //var totaltdssum = 0;

        //$.each($scope.dynamicTDSModalData[key].CHILDTDS, function (key, value) {

        //    totaltdssum += parseFloat(value.TDS_AMOUNT);
        //});
        //$scope.dynamicTDSModalData[key].TOTAL_TDS_AMOUNT = parseFloat(totaltdssum).toFixed(2);
        var taxable_amount = 0;

        if (typeof $scope.dynamicVATModalData[key].CHILDVAT[index].VAT_AMOUNT !== 'undefined' && $scope.dynamicVATModalData[key].CHILDVAT[index].VAT_AMOUNT !== null && $scope.dynamicVATModalData[key].CHILDVAT[index].VAT_AMOUNT !== "") {

            taxable_amount = $scope.dynamicVATModalData[key].CHILDVAT[index].VAT_AMOUNT;
        }
        var vat_amount = (parseFloat(taxable_amount) * 7.6923);
        $scope.dynamicVATModalData[key].CHILDVAT[index].TAXABLE_AMOUNT = (parseFloat(vat_amount)).toFixed(2);
        var totalvatsum = 0;

        $.each($scope.dynamicVATModalData[key].CHILDVAT, function (key, value) {

            totalvatsum += parseFloat(value.VAT_AMOUNT);
        });
        $scope.dynamicVATModalData[key].TOTAL_VAT_AMOUNT = parseFloat(totalvatsum).toFixed(2);
    };
    //SubLedger Ok 
    $scope.SubLedger_Ok = function (index, e) {
        //check child validation
        debugger;
        var SubLength = $scope.dynamicSubLedgerModalData[index].SUBLEDGER.length;
        var check = false;
        for (var i = 0; i < SubLength; i++) {

            //subledger code
            var child_subcode = $scope.dynamicSubLedgerModalData[index].SUBLEDGER[i].SUB_CODE;
            $scope.dynamicSubLedgerModalData[index].SUBLEDGER[i].PARTY_TYPE_CODE = $('#subcodedealercode_' + index + '_' + i)[0].value;
            if (child_subcode === null || child_subcode === "") {
                displayPopupNotification("Sub Ledger Code Should Not Be Empty.", "warning");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }

            //subledger amount
            var child_amount = $scope.dynamicSubLedgerModalData[index].SUBLEDGER[i].AMOUNT;
            if (child_amount === null || child_amount === "") {
                displayPopupNotification("Sub Ledger Amount Should Not Be Empty.", "warning");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            if (child_amount === undefined) {
                displayPopupNotification("Sub Ledger Amount Should Be Correct.", "error");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }


            for (var j = 0; j < SubLength; j++) {
                var newsubcode = $scope.dynamicSubLedgerModalData[index].SUBLEDGER[j].SUB_CODE;
                if (i != j) {
                    if (newsubcode === child_subcode) {
                        check = true;
                        break;
                    }
                }

            }

            if (check == true) {
                check = false;
                displayPopupNotification("Same Code Or Subledger cannot be selected", "warning");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
        };
        if ($scope.CodeValidation !== '') {
            displayPopupNotification("Same Code Or Subledger cannot be selected", "warning");
            e.preventDefault();
            e.stopPropagation();
            return false;
        }

        if ($scope.dynamicSubLedgerModalData[index].VOUCHER_AMOUNT == 0 || $scope.dynamicSubLedgerModalData[index].VOUCHER_AMOUNT == undefined) {
            $scope.childModels[index].AMOUNT = $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT;
            $scope.childModels[index].AMOUNT = $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT;
            var sum = 0;
            $.each($scope.childModels, function (key, value) {
                sum += $scope.childModels[key].AMOUNT;
            });
            if ($scope.masterModels.MASTER_AMOUNT !== undefined)
                $scope.masterModels.MASTER_AMOUNT = sum;
            $('.dynamicSubLedgerModal_' + index).modal('toggle');
            if ($scope.masterModels.hasOwnProperty("MASTER_TRANSACTION_TYPE")) {
                if ($scope.masterModels.MASTER_TRANSACTION_TYPE == "CR" || $scope.masterModels.MASTER_TRANSACTION_TYPE == "DR") {
                    $scope.accsummary.drTotal = $scope.masterModels.MASTER_AMOUNT;
                    $scope.accsummary.crTotal = $scope.masterModels.MASTER_AMOUNT;
                }
            }
            else {
                $scope.accsum(index);
            }
        }
        else if ($scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT > $scope.childModels[index].AMOUNT || $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT < $scope.childModels[index].AMOUNT) {
            e.preventDefault();
            e.stopPropagation();
            bootbox.confirm("Your main Amount is " + $scope.childModels[index].AMOUNT + " and your subledger total is " + $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT + ".. Do you want to continue?",
                function (result) {

                    if (result) {

                        $scope.childModels[index].AMOUNT = $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT;
                        var sum = 0;
                        $.each($scope.childModels, function (key, value) {

                            sum += $scope.childModels[key].AMOUNT;
                        });
                        if ($scope.masterModels.MASTER_AMOUNT !== undefined)
                            $scope.masterModels.MASTER_AMOUNT = sum;
                        $('.dynamicSubLedgerModal_' + index).modal('toggle');
                        if ($scope.masterModels.hasOwnProperty("MASTER_TRANSACTION_TYPE")) {
                            if ($scope.masterModels.MASTER_TRANSACTION_TYPE == "CR" || $scope.masterModels.MASTER_TRANSACTION_TYPE == "DR") {
                                $scope.accsummary.drTotal = $scope.masterModels.MASTER_AMOUNT;
                                $scope.accsummary.crTotal = $scope.masterModels.MASTER_AMOUNT;
                            }
                        }
                        else {
                            $scope.accsum(index);
                        }
                        $scope.$apply();
                    }
                    else {

                    }
                });
        }
        else {
            $scope.childModels[index].AMOUNT = $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT;
            $scope.childModels[index].AMOUNT = $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT;
            var sum = 0;
            $.each($scope.childModels, function (key, value) {
                sum += $scope.childModels[key].AMOUNT;
            });
            if ($scope.masterModels.MASTER_AMOUNT !== undefined)
                $scope.masterModels.MASTER_AMOUNT = sum;
            $('.dynamicSubLedgerModal_' + index).modal('toggle');
            if ($scope.masterModels.hasOwnProperty("MASTER_TRANSACTION_TYPE")) {
                if ($scope.masterModels.MASTER_TRANSACTION_TYPE == "CR" || $scope.masterModels.MASTER_TRANSACTION_TYPE == "DR") {
                    $scope.accsummary.drTotal = $scope.masterModels.MASTER_AMOUNT;
                    $scope.accsummary.crTotal = $scope.masterModels.MASTER_AMOUNT;
                }
            }
            else {
                $scope.accsum(index);
            }
        }

        $(".dynamicSubLedgerModal_" + index).on('hidden.bs.modal', function () {

            //$($(".childaccautoComplete input")[0]).focus();
            //$($(".childDiv :input").not(':button,:hidden')[0]).focus();
            $($($($(".dynamic_child_table")[0]).find('tr')[$($(".dynamic_child_table")[0]).find('tr').length - 1]).find('input').not(':button,:hidden,:disabled')[0]).focus();
        })

    };


    //TDS Ok 
    $scope.TDS_Ok = function (index, e) {

        $scope.dynamicTDSModalData[index].ACC_CODE = $scope.childModels[index].ACC_CODE;
        var primary_date = GetPrimaryDateByTableName($scope.DocumentName);

        var tdschildLength = $scope.dynamicTDSModalData[index].CHILDTDS.length;
        var check = false;
        for (var i = 0; i < tdschildLength; i++) {
            //supplier code

            var child_suppliercode = $scope.dynamicTDSModalData[index].CHILDTDS[i].SUPPLIER_CODE;
            if (child_suppliercode === null || child_suppliercode === "") {
                displayPopupNotification("Supplier Code Should Not Be Empty.", "warning");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            //acc code
            var child_acccode = $scope.dynamicTDSModalData[index].CHILDTDS[i].ACC_CODE;
            if (child_acccode === null || child_acccode === "") {
                displayPopupNotification("Account Code Should Not Be Empty.", "warning");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            //tds type code
            var child_tdstypecode = $scope.dynamicTDSModalData[index].CHILDTDS[i].TDS_TYPE_CODE;
            if (child_tdstypecode === null || child_tdstypecode === "") {
                displayPopupNotification("TDS Type Code Should Not Be Empty.", "warning");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            //tds Net Amount
            var child_netamount = $scope.dynamicTDSModalData[index].CHILDTDS[i].NET_AMOUNT;
            if (child_netamount === null || child_netamount === "") {
                displayPopupNotification("Net Amount Should Not Be Empty.", "warning");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            //tds percentage
            var child_per = $scope.dynamicTDSModalData[index].CHILDTDS[i].TDS_PERCENTAGE;
            if (child_per === null || child_per === "") {
                displayPopupNotification("Percentage Should Not Be Empty.", "warning");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
        }
        $scope.dynamicTDSModalData[index].INVOICE_DATE = $scope.masterModels[primary_date];
        $scope.childModels[index].AMOUNT = parseFloat($scope.dynamicTDSModalData[index].TOTAL_TDS_AMOUNT, 10);
        var sum = 0;
        $.each($scope.childModels, function (key, value) {

            sum += $scope.childModels[key].AMOUNT;
        });
        if ($scope.masterModels.MASTER_AMOUNT !== undefined)
            $scope.masterModels.MASTER_AMOUNT = sum;
        if ($scope.masterModels.hasOwnProperty("MASTER_TRANSACTION_TYPE")) {
            if ($scope.masterModels.MASTER_TRANSACTION_TYPE == "CR" || $scope.masterModels.MASTER_TRANSACTION_TYPE == "DR") {
                $scope.accsummary.drTotal = $scope.masterModels.MASTER_AMOUNT;
                $scope.accsummary.crTotal = $scope.masterModels.MASTER_AMOUNT;
            }
        }
        else {
            $scope.accsum(index);
        }
        //$scope.validaccsum(index);


    }
    //vt Ok 
    $scope.VAT_Ok = function (index, e) {

        var vatchildLength = $scope.dynamicVATModalData[index].CHILDVAT.length;
        var check = false;
        for (var i = 0; i < vatchildLength; i++) {

            //supplier code
            var child_suppliercode = $scope.dynamicVATModalData[index].CHILDVAT[i].SUPPLIER_CODE;
            console.log($scope.dynamicVATModalData[index].Enable_DirectEntry);
            if ($scope.dynamicVATModalData[index].Enable_DirectEntry == false) {

                if (child_suppliercode === null || child_suppliercode === "") {
                    displayPopupNotification("Supplier Code Should Not Be Empty.", "warning");
                    e.preventDefault();
                    e.stopPropagation();
                    return false;
                }
            }
            //acc code
            var child_acccode = $scope.dynamicVATModalData[index].CHILDVAT[i].ACC_CODE;
            var primary_date = GetPrimaryDateByTableName($scope.DocumentName);

            if (child_acccode === null || child_acccode === "") {
                displayPopupNotification("Account Code Should Not Be Empty.", "warning");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            //tds type code
            //var child_tdstypecode = $scope.dynamicTDSModalData[index].CHILDTDS[i].TDS_TYPE_CODE;
            //if (child_tdstypecode === null || child_tdstypecode === "") {
            //    displayPopupNotification("TDS Type Code Should Not Be Empty.", "warning");
            //    e.preventDefault();
            //    e.stopPropagation();
            //    return false;
            //}
            //tds Net Amount
            var child_vatamount = $scope.dynamicVATModalData[index].CHILDVAT[i].VAT_AMOUNT;
            if (child_vatamount === null || child_vatamount === "") {
                displayPopupNotification("Vat Amount Should Not Be Empty.", "warning");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            //tds percentage
            var child_per = $scope.dynamicVATModalData[index].CHILDVAT[i].TAXABLE_AMOUNT;
            if (child_per === null || child_per === "") {
                displayPopupNotification("Percentage Should Not Be Empty.", "warning");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
        }
        $scope.dynamicVATModalData[index].ACC_CODE = $scope.childModels[index].ACC_CODE;
        $scope.dynamicVATModalData[index].INVOICE_DATE = $scope.masterModels[primary_date];
        $scope.childModels[index].AMOUNT = parseFloat($scope.dynamicVATModalData[index].TOTAL_VAT_AMOUNT, 10);
        var sum = 0;
        $.each($scope.childModels, function (key, value) {

            sum += $scope.childModels[key].AMOUNT;
        });
        if ($scope.masterModels.MASTER_AMOUNT !== undefined)
            $scope.masterModels.MASTER_AMOUNT = sum;
        if ($scope.masterModels.hasOwnProperty("MASTER_TRANSACTION_TYPE")) {
            if ($scope.masterModels.MASTER_TRANSACTION_TYPE == "CR" || $scope.masterModels.MASTER_TRANSACTION_TYPE == "DR") {
                $scope.accsummary.drTotal = $scope.masterModels.MASTER_AMOUNT;
                $scope.accsummary.crTotal = $scope.masterModels.MASTER_AMOUNT;
            }
        }
        else {
            $scope.accsum(index);
        }
        //$scope.validaccsum(index);


    }

    //SubLedger Cancel 
    $scope.SubLedger_Cancel = function (index) {

        $scope.errorshow = "";
        //$scope.dynamicSubLedgerModalData[index] = [];
        //$scope.dynamicSubLedgerModalData[index] = [{
        //    ACC_CODE: 0,
        //    TRANSACTION_TYPE: "",
        //    VOUCHER_AMOUNT: "",
        //    SUBLEDGER_AMOUNT: "",
        //    REMAINING_AMOUNT: "",
        //    SUBLEDGER: [{
        //        SERIAL_NO: "",
        //        SUB_CODE: "",
        //        SUB_EDESC: "",
        //        AMOUNT: "",
        //        PARTICULARS: "",
        //        REFRENCE: ""
        //    }]
        //}];
        $scope.dynamicSubLedgerModalData[index].ACC_CODE = 0;
        $scope.dynamicSubLedgerModalData[index].TRANSACTION_TYPE = "";
        $scope.dynamicSubLedgerModalData[index].VOUCHER_AMOUNT = "";
        $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT = "";
        $scope.dynamicSubLedgerModalData[index].REMAINING_AMOUNT = "";
        $scope.dynamicSubLedgerModalData[index].SUBLEDGER = [{
            SERIAL_NO: index + 1,
            SUB_CODE: "",
            SUB_EDESC: "",
            PARTY_TYPE_CODE: "",
            AMOUNT: "",
            PARTICULARS: "",
            REFRENCE: ""
        }];


        //$scope.childModels[index].AMOUNT = 0;
        $scope.CodeValidation = '';

        if ($scope.masterModels.hasOwnProperty("MASTER_TRANSACTION_TYPE")) {
            var sum = 0;
            $.each($scope.childModels, function (key, value) {
                sum += value.AMOUNT;
            })
            $scope.masterModels.MASTER_AMOUNT = sum;
            $scope.accsummary.crTotal = sum;
            $scope.accsummary.drTotal = sum;
        }
        else {
            $scope.accsum(index);
        }
        $('.dynamicSubLedgerModal_' + index).modal('toggle');

        setTimeout(function () {

            //$($($($(".dynamic_child_table")[0]).find('tr')[$($(".dynamic_child_table")[0]).find('tr').length - 1]).find('input').not(':button,:hidden,:disabled')[0]).focus();
        }, 500);
    }
    $scope.vat_Cancel = function (e) {

        //$scope.errorshow = "";
        //$scope.dynamicVATModalData[e] = [];
        //$scope.childModels[index].AMOUNT = 0;
        //$scope.CodeValidation = '';
        $scope.dynamicVATModalData[e].ACC_CODE = 0;
        $scope.dynamicVATModalData[e].INVOICE_NO = "";
        $scope.dynamicVATModalData[e].INVOICE_DATE = "";
        $scope.dynamicVATModalData[e].REMARKS = "";
        $scope.dynamicVATModalData[e].REFRENCE_NO = "";
        $scope.dynamicVATModalData[e].TYPE = "";
        $scope.dynamicVATModalData[e].TOTAL_VAT_AMOUNT = "";
        $scope.dynamicVATModalData[e].CHILDVAT = [{
            SERIAL_NO: 1,
            SUPPLIER_CODE: "",
            TAXABLE_AMOUNT: "",
            VAT_AMOUNT: ""
        }];

        if ($scope.masterModels.hasOwnProperty("MASTER_TRANSACTION_TYPE")) {
            var sum = 0;
            $.each($scope.childModels, function (key, value) {
                sum += value.AMOUNT;
            })
            $scope.masterModels.MASTER_AMOUNT = sum;
            $scope.accsummary.crTotal = sum;
            $scope.accsummary.drTotal = sum;
        }
        else {
            $scope.accsum(index);
        }
        //$('.dynamicSubLedgerModal_' + index).modal('toggle');
    };
    $scope.tds_Cancel = function (e) {

        //$scope.errorshow = "";
        //$scope.dynamicTDSModalData[e] = [];
        //$scope.childModels[index].AMOUNT = 0;
        //$scope.CodeValidation = '';
        $scope.dynamicTDSModalData[e].ACC_CODE = 0;
        $scope.dynamicTDSModalData[e].VOUCHER_NO = "";
        $scope.dynamicTDSModalData[e].INVOICE_DATE = "";
        $scope.dynamicTDSModalData[e].REMARKS = "";
        $scope.dynamicTDSModalData[e].TOTAL_TDS_AMOUNT = "";
        $scope.dynamicTDSModalData[e].CHILDTDS = [{
            SERIAL_NO: 1,
            SUPPLIER_CODE: "",
            ACC_CODE: "",
            TDS_TYPE_CODE: "",
            MEETING_TYPE_CODE: "",
            NET_AMOUNT: "",
            TDS_PERCENTAGE: "",
            TDS_AMOUNT: ""
        }]


        if ($scope.masterModels.hasOwnProperty("MASTER_TRANSACTION_TYPE")) {
            var sum = 0;
            $.each($scope.childModels, function (key, value) {
                sum += value.AMOUNT;
            })
            $scope.masterModels.MASTER_AMOUNT = sum;
            $scope.accsummary.crTotal = sum;
            $scope.accsummary.drTotal = sum;
        }
        else {
            $scope.accsum(index);
        }
        //$('.dynamicSubLedgerModal_' + index).modal('toggle');
    };
    $scope.remove_childbudgetflag_element = function (key, index) {
        $scope.budgetCodeValidation = '';
        $scope.budgetcount = false;
        if ($scope.dynamicModalData[key].BUDGET.length > 1) {
            $scope.dynamicModalData[key].BUDGET.splice(index, 1);
        }
    }

    $scope.Budget_Ok = function (e) {



        //check child validation
        var BudgetLength = $scope.dynamicModalData[index].BUDGET.length;
        var check = false;
        for (var i = 0; i < BudgetLength; i++) {

            //budget amount
            var child_amount = $scope.dynamicModalData[index].BUDGET[i].AMOUNT;
            if (child_amount === null || child_amount === "") {
                displayPopupNotification("Budget Amount Should Not Be Empty.", "warning");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }

            //budget code
            var child_budgetcode = $scope.dynamicModalData[index].BUDGET[i].BUDGET_CODE;
            if (child_budgetcode === null || child_budgetcode === "") {
                displayPopupNotification("Budget Center Should Not Be Empty.", "warning");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }


            if (child_amount === undefined) {
                displayPopupNotification("Budget Amount Should Be Correct.", "error");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }


            for (var j = 0; j < BudgetLength; j++) {
                var newbudgetcode = $scope.dynamicModalData[index].BUDGET[j].BUDGET_CODE;
                if (i != j) {
                    if (newbudgetcode === child_budgetcode) {
                        check = true;
                        break;
                    }
                }

            }

            if (check == true) {
                check = false;
                displayPopupNotification("Same Budget Center cannot be selected.", "warning");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }

        };

        if ($scope.budgetCodeValidation !== '') {
            e.preventDefault();
            e.stopPropagation();
            return false;
        }
        if ($scope.childModels[index].AMOUNT == 0 || $scope.childModels[index].AMOUNT == "") {
            $scope.childModels[index].AMOUNT = $scope.sum;
        }

        var csum = 0;
        angular.forEach($scope.childModels, function (value, key) {

            if ($scope.childModels[key].hasOwnProperty("AMOUNT")) {
                if (typeof value.AMOUNT !== 'undefined' && value.AMOUNT !== null && value.AMOUNT !== "") {
                    csum = parseFloat(csum) + (parseFloat(value.AMOUNT));
                }
            }
        })
        $scope.masterModels.MASTER_AMOUNT = csum;
        //if ( $scope.sum !== $scope.childModels[index].AMOUNT) {
        //    displayPopupNotification("Ledger Amount should be equal to sum of Amount.", "warning");
        //    e.preventDefault();
        //    e.stopPropagation();
        //    return false;
        //}

    }


    $scope.Budget_Cancel = function (e) {

        $scope.budgetCodeValidation = '';
        $scope.budgetcount = false;
        $(".subledgerModal_" + index).on('hidden.bs.modal', function () {
            $($(".flag")[index]).focus();
            //$scope.dynamicModalData[e] = [];
            //$scope.dynamicModalData[e].BUDGET = [];
            $scope.dynamicModalData[e].ACC_CODE = "",
                $scope.dynamicModalData[e].BUDGET_FLAG = "",
                $scope.dynamicModalData[e].BUDGET = [{
                    SERIAL_NO: 1,
                    BUDGET_CODE: "",
                    AMOUNT: "",
                    NARRATION: "",
                }];

            $scope.childModels[e].AMOUNT = 0;
            if ($scope.masterModels.hasOwnProperty("MASTER_TRANSACTION_TYPE")) {
                var sum = 0;
                $.each($scope.childModels, function (key, value) {
                    sum += value.AMOUNT;
                })
                $scope.masterModels.MASTER_AMOUNT = sum;
                $scope.accsummary.crTotal = sum;
                $scope.accsummary.drTotal = sum;
            }
            else {
                $scope.accsum(e);
            }
        })
    }
    $scope.setNextFlagFocus = function () {
        $(".subledgerModal_" + index).on('hidden.bs.modal', function () {
            $(".flag").closest('td').next().find('input').focus();
        });
    }

    // remove child row.
    $scope.remove_child_element = function (index) {

        if ($scope.ChildFormElement.length > 1) {
            $scope.ChildFormElement.splice(index, 1);
            $scope.childModels.splice(index, 1); // }
            $scope.dynamicModalData.splice(index, 1);
            $scope.dynamicSubLedgerModalData.splice(index, 1);
            $scope.accsum(index);
        }
        var subledgerserialno = 1;
        var budgetserialno = 1;
        $.each($scope.dynamicSubLedgerModalData, function (key, value) {
            $.each(value.SUBLEDGER, function (key, values) {
                values.SERIAL_NO = subledgerserialno;

            });
            subledgerserialno++;
        })
        $.each($scope.dynamicModalData, function (key, value) {
            $.each(value.BUDGET, function (key, values) {
                values.SERIAL_NO = budgetserialno;

            });
            budgetserialno++;
        })



    }

    var formSetup = contravoucherservice.getFormSetup_ByFormCode($scope.formcode, d2);

    $.when(d2).done(function (result) {

        $scope.formSetup = result.data;
        $scope.FormName = $scope.formSetup[0].FORM_EDESC;
        if ($scope.formSetup.length > 0) {
            $scope.ModuleCode = $scope.formSetup[0].MODULE_CODE;
        }
    });
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

    function dateSet(date) {
        var month = date.getMonth() + 1;
        var day = date.getDate();
        date = date.getFullYear() + '/' +
            (('' + month).length < 2 ? '0' : '') + month + '/' +
            (('' + day).length < 2 ? '0' : '') + day;
        return date;
    }

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
    //Division
    $scope.FaDivisionSetupDataSource = {
        type: "json",
        serverFiltering: true,
        suggest: true,
        highlightFirst: true,
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
        suggest: true,
        highlightFirst: true,
    };
    //Customer
    $scope.customerDataSource = {
        type: "json",
        serverFiltering: true,
        suggest: true,
        highlightFirst: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllCustomerSetupByFilter",

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


    $scope.CountVoucherTotal = function () {

        var tablename = $scope.MasterFormElement[0].TABLE_NAME;
        var response = contravoucherservice.GetVouchersCount($routeParams.formcode, tablename);

        response.then(function (res) {

            if (res.data != "0") {

                $scope.VoucherCount = res.data;
            }
        });

    }

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
        suggest: true,
        highlightFirst: true,

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
                var cacccodevalue = $rootScope.Contra_C_ACC_CODE_DEFAULTVAL;

                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {

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
                        //newParams = {
                        //    filter: ""
                        //};
                        //return newParams;
                        if (cacccodevalue != "" && cacccodevalue != undefined) {
                            newParams = {
                                filter: cacccodevalue
                            };
                            return newParams;
                        }
                        else {
                            //newParams = {
                            //    filter: ""
                            //};
                            //return newParams;
                            if (cacccodevalue != "" && cacccodevalue != undefined) {
                                newParams = {
                                    filter: cacccodevalue
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

    $scope.accountCodeAutocomplete = {
        dataSource: $scope.accountCodeDataSource,
        dataTextField: 'ACC_EDESC',
        dataValueField: 'ACC_CODE',
        suggest: true,
        autoBind: true,

        filter: "contains",
        highlightFirst: true,
        close: function (e) {
            debugger;
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
                        PARTY_TYPE_CODE: "",
                        AMOUNT: "",
                        PARTICULARS: "",
                        REFRENCE: ""
                    }];

                }
                if (accCode === $scope.dynamicModalData[index].ACC_CODE) {
                    $scope.dynamicModalData[index] = $scope.dynamicModalData[index];
                } else {

                    $scope.dynamicModalData[index].BUDGET = [{
                        SERIAL_NO: index + 1,
                        BUDGET_CODE: "",
                        AMOUNT: "",
                        NARRATION: ""
                    }];

                }
                if (accCode === $scope.dynamicTDSModalData[index].ACC_CODE) {
                    $scope.dynamicTDSModalData[index] = $scope.dynamicTDSModalData[index];

                } else {

                    $scope.dynamicTDSModalData[index].CHILDTDS = [{
                        SERIAL_NO: index + 1,
                        SUPPLIER_CODE: "",
                        ACC_CODE: "",
                        TDS_TYPE_CODE: "",
                        MEETING_TYPE_CODE: "",
                        NET_AMOUNT: "",
                        TDS_PERCENTAGE: "",
                        TDS_AMOUNT: ""
                    }];
                }
                if (accCode === $scope.dynamicModalData[index].ACC_CODE) {
                    $scope.dynamicModalData[index] = $scope.dynamicModalData[index];

                } else {

                    $scope.dynamicModalData[index].BUDGET = [{
                        SERIAL_NO: index + 1,
                        BUDGET_CODE: "",
                        AMOUNT: "",
                        NARRATION: ""
                    }];
                }
                if (accCode === $scope.dynamicVATModalData[index].ACC_CODE) {
                    $scope.dynamicVATModalData[index] = $scope.dynamicVATModalData[index];

                } else {

                    $scope.dynamicVATModalData[index].CHILDVAT = [{
                        SERIAL_NO: index + 1,
                        SUPPLIER_CODE: "",
                        TAXABLE_AMOUNT: "",
                        VAT_AMOUNT: "",
                    }];
                }
                $scope.childModels[index][TRANSACTION_TYPE] = $scope.childModels[index].TRANSACTION_TYPE;
                //$scope.transactiontype = $scope.childModels[index].TRANSACTION_TYPE;
                $scope.dynamicSubLedgerModalData[index].TRANSACTION_TYPE = $scope.childModels[index][TRANSACTION_TYPE];


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
                var response = $http.get('/api/TemplateApi/IfIsTdsByAccCode?accCode=' + accCode);
                response.then(function (res) {

                    if (res.data != "0") {

                        $scope.dynamicTDSModalData[index].ACC_CODE = accCode;
                        popupAccessTds = true;
                        $scope.popUpTds(index);

                    }
                });
                var response = $http.get('/api/TemplateApi/IfIsVATByAccCode?accCode=' + accCode);

                response.then(function (res) {

                    if (res.data != "0") {

                        $scope.dynamicVATModalData[index].ACC_CODE = accCode;
                        popupAccessVAT = true;
                        $scope.popUpVAT(index);

                    }
                });
                var response = $http.get('/api/TemplateApi/getBudgetCodeCountCodeByAccCode?accCode=' + accCode);
                response.then(function (res) {

                    if (res.data != "0") {

                        $scope.dynamicModalData[index].ACC_CODE = accCode;
                        popupAccessBudget = true;

                        $scope.popUpBudget(index);

                    }
                });

                //var response = $http.get('/api/ContraVoucherApi/GetPurchaseExpensesFlag?formCode=' + $scope.formcode);
                //response.then(function (res1) {
                //    debugger;
                //    var response = null;
                //    if (res1.data[0] == 'Y') {
                //        debugger;
                //        response = $http.get('/api/TemplateApi/GetAccountListByAccountCode?accId=null&accMastercode=niraj&searchText=' + dataItem.ACC_EDESC);
                //    }
                //    else {
                //        response = $http.get('/api/TemplateApi/GetAccountListByAccountCode?accId=null&accMastercode=&searchText=' + dataItem.ACC_EDESC);
                //    }
                //    response.then(function (res2) {
                //        debugger;
                //        if (res2.data.length > 0) {
                //            if (res2.data[0].ACC_NATURE == 'SB') {
                //                $("#PurchaseExpSheet").modal('toggle');
                //            }
                //        }
                //    });
                //});

                //for Budget Center Pupop

                //var response = $http.get('/api/TemplateApi/getBudgetCodeCountCodeByAccCode?accCode=' + window.accCode);
                //response.then(function (res) {

                //    if (res.data != "0") {
                //        $scope.childModels[index].Display_BFlag = 'T';
                //        $scope.dynamicModalData[index].ACC_CODE = accCode;
                //        $scope.popupAccessBudget = true;
                //        $scope.popUpBudget(index);
                //        setTimeout(function () {
                //            $(".budgetCenterAutoComplete").focus();

                //        }, 500);
                //    }
                //});
                //var len = (parseInt(index) * 2) + 1;
                //$($(".subledgerfirst:input")[len]).data('kendoComboBox').dataSource.read();
                //$($(".subledgersecond:input")[len]).data('kendoComboBox').dataSource.read();

                $scope.$apply();

            }
        },

        change: function (e) {

            var ono = $scope.OrderNo;

            //var dataItem = this.dataItem(this.select());
            //if (dataItem == undefined)
            //    return;
            //$scope.accName = dataItem.ACC_EDESC;
            //$scope.accCode = dataItem.ACC_CODE;
            //window.accCode = dataItem.ACC_CODE;
            //var accCode = e.sender.dataItem();
            //var index = parseInt(this.element.closest('td').find('input')[0].value);
            //var accCode = $scope.dynamicSubLedgerModalData[index].ACC_CODE;
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
            //        BUDGET_CODE: "",
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
            //if (ono == "undefined") {
            //    if (e.sender.dataItem() == undefined) {
            //        $(this.element.closest('td').find('input')[1]).parent().parent().parent().addClass('borderRed');
            //        $(this.element.closest('td').find('input')[1]).val("");
            //    }
            //    else {
            //        $(this.element.closest('td').find('input')[1]).parent().parent().parent().removeClass('borderRed');
            //    }
            //}
            //else {
            //    if ($scope.dynamicSubLedgerModalData[index].SUBLEDGER[0].SUB_CODE!= "") {


            //        var response = $http.get('/api/TemplateApi/getSubledgerCodeByAccCode?accCode=' + accCode);
            //        response.then(function (res) {

            //            if (res.data != "0") {

            //                $scope.dynamicSubLedgerModalData[index].ACC_CODE = accCode;
            //                popupAccess = true;
            //                $scope.popUp(index);

            //            }
            //        });

            //        //



            //    }
            //}


        },
        dataBound: function (e) {
            var index = this.element[0].attributes['acc-data-index'].value;
            var accountLength = ((parseInt(index) + 1) * 3) - 1;
            var account = $($(".cacccode")[accountLength]).data("kendoComboBox");
            //if (account != undefined) {

            //    account.setOptions({

            //        template: $.proxy(kendo.template("#= formatValue(ACC_EDESC,Type, this.text()) #"), account)
            //    });
            //}

        }
    };


    $scope.budgetCenterDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllBudgetCenterByFilter"
            },
            parameterMap: function (data, action) {

                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value,
                            accCode: window.accCode
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: "",
                            accCode: window.accCode
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
    var budgetCode = "";

    //$scope.budgetCenterChildDataSource = {
    //    type: "json",
    //    serverFiltering: true,
    //    transport: {
    //        read: {
    //            url: "/api/TemplateApi/GetAllBudgetCenterChildByFilter"
    //        },
    //        parameterMap: function (data, action) {
    //         
    //            var newParams;
    //            if (data.filter != undefined) {
    //                if (data.filter.filters[0] != undefined) {
    //                    newParams = {
    //                        filter: data.filter.filters[0].value,
    //                        accCode: budgetCode
    //                    };
    //                    return newParams;
    //                }
    //                else {
    //                    newParams = {
    //                        filter: "",
    //                        accCode: budgetCode
    //                    };
    //                    return newParams;
    //                }
    //            }
    //            else {
    //                newParams = {
    //                    filter: "",
    //                    accCode: budgetCode
    //                };
    //                return newParams;
    //            }
    //        }
    //    },
    //}

    $scope.budgetCenterOption = {
        dataSource: $scope.budgetCenterDataSource,
        dataTextField: 'BUDGET_EDESC',
        dataValueField: 'BUDGET_CODE',
        suggest: true,
        highlightFirst: true,
        select: function (e) {

            var Code = e.dataItem.BUDGET_CODE;
            var key = this.element[0].attributes['budget-key'].value;
            var index = this.element[0].attributes['budget-index'].value;

            var budgetlen = $scope.dynamicModalData[key].BUDGET.length;
            //check valildation after select new code  start
            for (var j = 0; j < budgetlen; j++) {
                var budgetcode = $scope.dynamicModalData[key].BUDGET[j].BUDGET_CODE;
                if (index != j) {
                    if (budgetcode === Code) {
                        $scope.budgetCodeValidation = "You cannot select same Budget Center";
                        $($(".BUDGETCODE_" + index)[1]).addClass("borderRed");
                        $scope.budgetcount = true;
                        return;

                    }
                    else {
                        $($(".BUDGETCODE_" + index)[1]).removeClass("borderRed");
                        if (!$(".budgetCenterAutoComplete").hasClass("borderRed")) {
                            $scope.budgetCodeValidation = '';
                        }

                        $scope.budgetcount = false;
                    };

                }
            }
            //end
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
                            accCode: window.accCode
                        };
                        return newParams;

                    }
                    else {
                        newParams = {
                            filter: "",
                            accCode: window.accCode
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: "",
                        accCode: window.accCode
                    };
                    return newParams;
                }
            }
        },
    }
    $scope.TdsSupplierDataSource = {
        type: "json",
        serverFiltering: true,
        autoBind: true,

        transport: {
            read: {
                url: "/api/TemplateApi/GetAllSupplierListByFilter"
            },
            parameterMap: function (data, action) {

                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value,
                            accCode: window.accCode
                        };
                        return newParams;

                    }
                    else {
                        newParams = {
                            filter: "",
                            accCode: window.accCode
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: "",
                        accCode: window.accCode
                    };
                    return newParams;
                }
            }
        },
    }
    $scope.tdsSupplierCodeAutocomplete =
    {
        dataSource: $scope.TdsSupplierDataSource,
        suggest: false,
        highlightFirst: true,
        select: function (e) {
            var description = e.dataItem.LINK_SUB_CODE;
            var Code = e.dataItem.LINK_SUB_CODE;
            var key = this.element[0].attributes['tds-key'].value;

            if (this.element[0].attributes['tds-index'] !== undefined) {
                var index = this.element[0].attributes['tds-index'].value;
                $scope.dynamicTDSModalData[key].CHILDTDS[index].SUPPLIER_CODE = description;


                var sublen = $scope.dynamicTDSModalData[key].CHILDTDS.length;
                //check valildation after select new code  start
                for (var j = 0; j < sublen; j++) {

                    var subcode = $scope.dynamicTDSModalData[key].CHILDTDS[j].SUPPLIER_CODE;
                    if (index != j) {
                        if (subcode === Code) {
                            $scope.TDSCodeValidation = "You cannot select same Code.";
                            //$($(".SUBLEDGERCODE_" + index)[1]).addClass("borderRed");
                            $($(".tdssupplierDESCRIPTION_" + index)[1]).addClass("borderRed");
                            $scope.tdscount = true;
                            return;

                        }
                        else {
                            //$($(".SUBLEDGERCODE_" + index)[1]).removeClass("borderRed");
                            $($(".tdssupplierDESCRIPTION_" + index)[1]).addClass("borderRed");
                            if (!$(".tdssuppliersecond").hasClass("borderRed")) {
                                $scope.TDSCodeValidation = '';
                            }

                            $scope.tdscount = false;
                        };

                    }
                }
                //end
            }


        },
        dataBound: function (e) {


            if (this.element[0].attributes['tds-index'] !== undefined) {
                var index = this.element[0].attributes['tds-index'].value;
                var tdsCustomerLength = ((parseInt(index) + 1) * 3) - 1;
                var tdssupplier = $($(".tdssuppliersecond")[tdsCustomerLength]).data("kendoComboBox");
                if (tdssupplier != undefined) {
                    tdssupplier.setOptions({
                        template: $.proxy(kendo.template("#= formatValue(SUPPLIER_EDESC,Type,this.text()) #"), tdssupplier)
                    });
                }
            }

        },
    }

    $scope.TdsTypeDataSource = {
        type: "json",
        serverFiltering: true,
        autoBind: true,

        transport: {
            read: {
                url: "/api/TemplateApi/GetAllTDSByFilter"
            },
            parameterMap: function (data, action) {

                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value,
                            //accCode: window.accCode
                        };
                        return newParams;

                    }
                    else {
                        newParams = {
                            filter: "",
                            //accCode: window.accCode
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: "",
                        //accCode: window.accCode
                    };
                    return newParams;
                }
            }
        },
    }
    $scope.tdsTypeAutocomplete =
    {
        dataSource: $scope.TdsTypeDataSource,
        suggest: false,
        highlightFirst: true,
        select: function (e) {

            var description = e.dataItem.TDS_CODE;
            var Code = e.dataItem.TDS_CODE;
            var key = this.element[0].attributes['tdstype-key'].value;
            var index = this.element[0].attributes['tdstype-index'].value;

            $scope.dynamicTDSModalData[key].CHILDTDS[index].TDS_CODE = description;


            var sublen = $scope.dynamicTDSModalData[key].CHILDTDS.length;
            //check valildation after select new code  start
            for (var j = 0; j < sublen; j++) {
                var subcode = $scope.dynamicTDSModalData[key].CHILDTDS[j].TDS_CODE;
                if (index != j) {
                    if (subcode === Code) {
                        $scope.TDSCodeValidation = "You cannot select same Code.";
                        //$($(".SUBLEDGERCODE_" + index)[1]).addClass("borderRed");
                        $($(".tdstypeDESCRIPTION_" + index)[1]).addClass("borderRed");
                        $scope.tdstype = true;
                        return;

                    }
                    else {
                        //$($(".SUBLEDGERCODE_" + index)[1]).removeClass("borderRed");
                        $($(".tdstypeDESCRIPTION_" + index)[1]).addClass("borderRed");
                        if (!$(".tdstypesecond").hasClass("borderRed")) {
                            $scope.TDSCodeValidation = '';
                        }

                        $scope.tdstype = false;
                    };

                }
            }
            //end

        },
        dataBound: function (e) {

            var index = this.element[0].attributes['tdstype-index'].value;
            var tdsCustomerLength = ((parseInt(index) + 1) * 3) - 1;
            var tdssupplier = $($(".tdssuppliersecond")[tdsCustomerLength]).data("kendoComboBox");
            if (tdssupplier != undefined) {
                tdssupplier.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(SUPPLIER_EDESC,Type,this.text()) #"), tdssupplier)
                });
            }
        },
    }

    $scope.TdsAccountDataSource = {
        type: "json",
        serverFiltering: true,
        autoBind: true,

        transport: {
            read: {
                url: "/api/TemplateApi/GetAllAccountSetupByFilter"
            },
            parameterMap: function (data, action) {

                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value,
                            accCode: window.accCode
                        };
                        return newParams;

                    }
                    else {
                        newParams = {
                            filter: "",
                            accCode: window.accCode
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: "",
                        accCode: window.accCode
                    };
                    return newParams;
                }
            }
        },
    }
    $scope.tdsAccCodeAutocomplete =
    {
        dataSource: $scope.TdsAccountDataSource,
        suggest: false,
        highlightFirst: true,
        select: function (e) {

            var description = e.dataItem.ACC_CODE;
            var Code = e.dataItem.SUPPLIER_CODE;
            var key = this.element[0].attributes['tdsacc-key'].value;
            var index = this.element[0].attributes['tdsacc-index'].value;

            $scope.dynamicTDSModalData[key].CHILDTDS[index].ACC_CODE = description;


            var sublen = $scope.dynamicTDSModalData[key].CHILDTDS.length;
            //check valildation after select new code  start
            for (var j = 0; j < sublen; j++) {
                var subcode = $scope.dynamicTDSModalData[key].CHILDTDS[j].ACC_CODE;
                if (index != j) {
                    if (subcode === Code) {
                        $scope.TDSCodeValidation = "You cannot select same Code.";
                        //$($(".SUBLEDGERCODE_" + index)[1]).addClass("borderRed");
                        $($(".tdsaccDESCRIPTION_" + index)[1]).addClass("borderRed");
                        $scope.tdsacccount = true;
                        return;

                    }
                    else {
                        //$($(".SUBLEDGERCODE_" + index)[1]).removeClass("borderRed");
                        $($(".tdsaccDESCRIPTION_" + index)[1]).addClass("borderRed");
                        if (!$(".tdsaccsecond").hasClass("borderRed")) {
                            $scope.TDSCodeValidation = '';
                        }

                        $scope.tdsacccount = false;
                    };

                }
            }
            //end

        },
        dataBound: function (e) {

            var index = this.element[0].attributes['tdsacc-index'].value;
            var tdsCustomerLength = ((parseInt(index) + 1) * 3) - 1;
            var tdssupplier = $($(".tdssuppliersecond")[tdsCustomerLength]).data("kendoComboBox");
            if (tdssupplier != undefined) {
                tdssupplier.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(SUPPLIER_EDESC,Type,this.text()) #"), tdssupplier)
                });
            }
        },
    }
    $scope.VATSupplierDataSource = {
        type: "json",
        serverFiltering: true,
        autoBind: true,

        transport: {
            read: {
                url: "/api/TemplateApi/GetAllSupplierListByFilter"
            },
            parameterMap: function (data, action) {

                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value,
                            accCode: window.accCode
                        };
                        return newParams;

                    }
                    else {
                        newParams = {
                            filter: "",
                            accCode: window.accCode
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: "",
                        accCode: window.accCode
                    };
                    return newParams;
                }
            }
        },
    }
    $scope.VATSupplierCodeAutocomplete =
    {
        dataSource: $scope.VATSupplierDataSource,
        suggest: false,
        highlightFirst: true,
        select: function (e) {

            var description = e.dataItem.SUPPLIER_CODE;
            var Code = e.dataItem.SUPPLIER_CODE;
            var key = this.element[0].attributes['vat-key'].value;

            if (this.element[0].attributes['vat-index'] !== undefined) {
                var index = this.element[0].attributes['vat-index'].value;
                $scope.dynamicVATModalData[key].CHILDVAT[index].SUPPLIER_CODE = description;


                var sublen = $scope.dynamicVATModalData[key].CHILDVAT.length;
                //check valildation after select new code  start
                for (var j = 0; j < sublen; j++) {
                    var subcode = $scope.dynamicVATModalData[key].CHILDVAT[j].SUPPLIER_CODE;
                    if (index != j) {
                        if (subcode === Code) {
                            $scope.VATCodeValidation = "You cannot select same Code.";
                            //$($(".SUBLEDGERCODE_" + index)[1]).addClass("borderRed");
                            $($(".vatsupplierDESCRIPTION_" + index)[1]).addClass("borderRed");
                            $scope.vatcount = true;
                            return;

                        }
                        else {
                            //$($(".SUBLEDGERCODE_" + index)[1]).removeClass("borderRed");
                            $($(".vatsupplierDESCRIPTION_" + index)[1]).addClass("borderRed");
                            if (!$(".vatsuppliersecond").hasClass("borderRed")) {
                                $scope.VATCodeValidation = '';
                            }

                            $scope.vatcount = false;
                        };

                    }
                }
                //end
            }


        },
        dataBound: function (e) {


            if (this.element[0].attributes['vat-index'] !== undefined) {
                var index = this.element[0].attributes['vat-index'].value;
                var vatsupplierLength = ((parseInt(index) + 1) * 3) - 1;
                var vatsupplier = $($(".vatsuppliersecond")[vatsupplierLength]).data("kendoComboBox");
                if (vatsupplier != undefined) {
                    vatsupplier.setOptions({
                        template: $.proxy(kendo.template("#= formatValue(SUPPLIER_EDESC,Type,this.text()) #"), vatsupplier)
                    });
                }
            }

        },
    }




    //$scope.subledgerAutocomplete =
    //    {
    //        dataSource: $scope.subLedgerDataSource,
    //        dataTextField: 'SUB_EDESC',
    //        dataValueField: 'SUB_CODE',
    //        suggest: true,
    //        highlightFirst: true,
    //        select: function (e) {

    //        },
    //        dataBound: function (e) {
    //          
    //            var index = this.element[0].attributes['subledger-index'].value;
    //            var subledgerLength = ((parseInt(index) + 1) * 3) - 1;
    //            var subledger = $($(".subledgersecond")[subledgerLength]).data("kendoComboBox");
    //            if (subledger != undefined) {
    //                subledger.setOptions({
    //                    template: $.proxy(kendo.template("#= formatValue(SUB_EDESC,Type,this.text()) #"), subledger)
    //                });
    //            }
    //        },
    //    }




    $scope.subledgerCodeAutocomplete =
    {
        dataSource: $scope.subLedgerDataSource,
        suggest: false,
        highlightFirst: true,
        select: function (e) {

            var description = e.dataItem.SUB_EDESC;
            var Code = e.dataItem.SUB_CODE;
            var key = this.element[0].attributes['subledger-key'].value;
            var index = this.element[0].attributes['subledger-index'].value;
            console.log("description", description);

            $scope.dynamicSubLedgerModalData[key].SUBLEDGER[index].SUB_EDESC = description;


            var sublen = $scope.dynamicSubLedgerModalData[key].SUBLEDGER.length;
            //check valildation after select new code  start
            for (var j = 0; j < sublen; j++) {
                var subcode = $scope.dynamicSubLedgerModalData[key].SUBLEDGER[j].SUB_CODE;
                if (index != j) {
                    if (subcode === Code) {
                        $scope.CodeValidation = "You cannot select same Code and Subledger";
                        $($(".SUBLEDGERCODE_" + index)[1]).addClass("borderRed");
                        $($(".SUBLEDGERDESCRIPTION_" + index)[1]).addClass("borderRed");
                        $scope.subledgercount = true;
                        return;

                    }
                    else {
                        $($(".SUBLEDGERCODE_" + index)[1]).removeClass("borderRed");
                        $($(".SUBLEDGERDESCRIPTION_" + index)[1]).removeClass("borderRed");
                        if (!$(".subledgerfirst").hasClass("borderRed")) {
                            $scope.CodeValidation = '';
                        }

                        $scope.subledgercount = false;
                    };

                }
            }
            //end

        },
        dataBound: function (e) {

            var index = this.element[0].attributes['subledger-index'].value;
            var subledgerLength = ((parseInt(index) + 1) * 3) - 1;
            var subledger = $($(".subledgersecond")[subledgerLength]).data("kendoComboBox");
            if (subledger != undefined) {
                subledger.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(SUB_EDESC,Type,this.text()) #"), subledger)
                });
            }
        },
    }


    $scope.subledgerDescAutocomplete =
    {
        dataSource: $scope.subLedgerDataSource,
        suggest: false,
        highlightFirst: true,
        select: function (e) {

            var Code = e.dataItem.SUB_CODE;
            var key = this.element[0].attributes['subledger-key'].value;
            var index = this.element[0].attributes['subledger-index'].value;

            $scope.dynamicSubLedgerModalData[key].SUBLEDGER[index].SUB_CODE = Code;


            var sublen = $scope.dynamicSubLedgerModalData[key].SUBLEDGER.length;
            //check valildation after select new code  start
            for (var j = 0; j < sublen; j++) {
                var subcode = $scope.dynamicSubLedgerModalData[key].SUBLEDGER[j].SUB_CODE;
                if (index != j) {
                    if (subcode === Code) {
                        $scope.CodeValidation = "You cannot select same Code and Subledger";
                        $($(".SUBLEDGERCODE_" + index)[1]).addClass("borderRed");
                        $($(".SUBLEDGERDESCRIPTION_" + index)[1]).addClass("borderRed");
                        $scope.subledgercount = true;
                        return;

                    }
                    else {
                        $($(".SUBLEDGERCODE_" + index)[1]).removeClass("borderRed");
                        $($(".SUBLEDGERDESCRIPTION_" + index)[1]).removeClass("borderRed");
                        if (!$(".subledgerfirst").hasClass("borderRed")) {
                            $scope.CodeValidation = '';
                        }
                        $scope.subledgercount = false;
                    };

                }
            }
            //end
        },
        dataBound: function (e) {

            var index = this.element[0].attributes['subledger-index'].value;
            var subledgerLength = ((parseInt(index) + 1) * 3) - 1;
            var subledger = $($(".subledgersecond")[subledgerLength]).data("kendoComboBox");
            if (subledger != undefined) {
                subledger.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(SUB_EDESC,Type,this.text()) #"), subledger)
                });
            }
        },
    };

    $scope.budgetCenterChildOption = {
        dataSource: $scope.budgetCenterChildDataSource,
        dataTextField: 'BUDGET_EDESC',
        dataValueField: 'BUDGET_CODE',
        dataBound: function (e) {

        }
    }

    $scope.flagKyeDown = function ($index, event) {

        if (event.keyCode == 13) {
            window.accCode = $scope.childModels[$index][ACC_CODE];
            $scope.dynamicModalData[$index].ACC_CODE = window.accCode;
            $scope.dynamicModalData[$index].BUDGET_FLAG = $scope.childModels[$index][BUDGET_FLAG] == undefined ? "" : $scope.childModels[$index][BUDGET_FLAG];
            index = $index;
            var response = $http.get('/api/TemplateApi/getBudgetCodeCountCodeByAccCode?accCode=' + window.accCode);
            response.then(function (res) {

                if (res.data != "0") {
                    $('.subledgerModal_' + $index).modal('toggle');
                    $scope.childModels[index].Display_BFlag = 'T';
                    setTimeout(function () {
                        $(".budgetCenterAutoComplete").focus();

                    }, 500);
                }
            });

            var first = $(".budgetCenterAutoComplete:input");
            if ($scope.childModels[index].AMOUNT === undefined || $scope.childModels[index].AMOUNT === null) {
                $("#BudgetOk").addClass("disabled");
                $scope.LedgerAmountValidation = "* Ledger Amount cannot be empty.*";
            }
            else {
                $("#BudgetOk").removeClass("disabled");
                $scope.LedgerAmountValidation = "";
            }
            $.each(first, function (i, obj) {
                obj = $(obj);
                if (!_.isEmpty(obj.data('kendoComboBox'))) {
                    obj.data('kendoComboBox').dataSource.read();
                }
            });
        }
        $scope.childModels[$index].BUDGET_FLAG = $scope.childModels[$index].BUDGET_FLAG == 'L' ? 'L' : 'E';

    }

    $scope.popUp = function ($index) {

        if (popupAccess === true) {
            if ($scope.dynamicSubLedgerModalData[$index].ACC_CODE !== 0) {

                window.accCode = $scope.dynamicSubLedgerModalData[$index].ACC_CODE;
            }
            //$(".dynamicSubLedgerModal_" + $index).modal('toggle');
            $(".dynamicSubLedgerModal_" + $index).modal({
                show: 'true'
            });

            $(".dynamicSubLedgerModal_" + $index).on('shown.bs.modal', function () {
                var $this = this;
                setTimeout(function () {

                    $($($($(".subledger-table")[$index]).find('tr')[1]).find('td :input')[0]).focus();
                }, 500);
            })
        }

    }
    $scope.popUpTds = function ($index) {

        if (popupAccessTds === true) {
            if ($scope.dynamicTDSModalData[$index].ACC_CODE !== "") {
                window.accCode = $scope.dynamicTDSModalData[$index].ACC_CODE;
            }
            var totaltaxableamount = 0;
            angular.forEach($scope.dynamicTDSModalData[$index].CHILDTDS, function (value, key) {

                totaltaxableamount = totaltaxableamount + value.TDS_AMOUNT;
            });
            $scope.dynamicTDSModalData[$index].TOTAL_TDS_AMOUNT = totaltaxableamount;
            $(".dynamictdsModal_" + $index).modal('toggle');

            $(".dynamictdsModal_" + $index).on('shown.bs.modal', function () {
                var $this = this;
                setTimeout(function () {

                    $($($($(".subledger-table")[$index]).find('tr')[1]).find('td :input')[0]).focus();
                }, 500);
            })
        }

    }
    $scope.popUpVAT = function ($index) {

        if (popupAccessVAT === true) {
            if ($scope.dynamicVATModalData[$index].ACC_CODE !== "") {
                window.accCode = $scope.dynamicVATModalData[$index].ACC_CODE;
            }
            var totalvatamount = 0;
            angular.forEach($scope.dynamicVATModalData[$index].CHILDVAT, function (value, key) {
                totalvatamount = totalvatamount + value.VAT_AMOUNT;


            });
            setTimeout(function () {

                //$("#doctype_" + $index).html($scope.dynamicVATModalData[$index].DOC_TYPE); 
                //    $("#type_" + $index).html($scope.dynamicVATModalData[$index].TYPE); 

                //$("#doctype_" + $index).append($('<option>',
                //    {
                //        value: $scope.dynamicVATModalData[$index].DOC_TYPE,
                //        text: "Sales"
                //    }));
                //$("#type_" + $index).html($scope.dynamicVATModalData[$index].TYPE); 
            }, 500);

            $scope.dynamicVATModalData[$index].TOTAL_VAT_AMOUNT = totalvatamount;
            $(".dynamicVATModal_" + $index).modal('toggle');

            $(".dynamicVATModal_" + $index).on('shown.bs.modal', function () {
                var $this = this;
                setTimeout(function () {

                    $($($($(".subledger-table")[$index]).find('tr')[1]).find('td :input')[0]).focus();
                }, 500);
            })
        }

    }
    $scope.popUpBudget = function ($index) {

        if (popupAccessBudget === true) {
            if ($scope.dynamicModalData[$index].ACC_CODE !== "") {
                window.accCode = $scope.dynamicModalData[$index].ACC_CODE;
            }
            angular.forEach($scope.dynamicModalData[$index].BUDGET, function (value, key) {

                if (value.BUDGET_CODE !== "") {
                    var budreqq = "/api/TemplateApi/GetBudgetNameByBCode?code=" + value.BUDGET_CODE;
                    $http.get(budreqq).then(function (budresultss) {
                        setTimeout(function () {

                            $("#BUDGETCODE_" + key).data('kendoComboBox').dataSource.data([{ BUDGET_CODE: value.BUDGET_CODE, BUDGET_EDESC: budresultss.data, Type: "code" }]);
                        }, 100);
                    });
                }
            });
            $('.subledgerModal_' + $index).modal('toggle');

            $(".subledgerModal_" + $index).on('shown.bs.modal', function () {
                var $this = this;
                setTimeout(function () {

                    $($($($(".subledger-table")[$index]).find('tr')[1]).find('td :input')[0]).focus();
                }, 500);
            })
            var first = $(".budgetCenterAutoComplete:input");
            if ($scope.childModels[$index].AMOUNT === undefined || $scope.childModels[$index].AMOUNT === null) {
                $("#BudgetOk").addClass("disabled");
                $scope.LedgerAmountValidation = "* Ledger Amount cannot be empty.*";
            }
            else {
                $("#BudgetOk").removeClass("disabled");
                $scope.LedgerAmountValidation = "";
            }
            $.each(first, function (i, obj) {
                obj = $(obj);
                if (!_.isEmpty(obj.data('kendoComboBox'))) {
                    obj.data('kendoComboBox').dataSource.read();
                }
            });

        }

    }
    $scope.loadingBtn = function () {
        $("#F").button('loading');
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
            TEMPLATE_NDESC: "",
            TEMPLATE_ASSIGNEE: "",
            ASSIGNED_DATE: ""
        }
    }
    $scope.getDraftDetails = function () {
        var d7 = $.Deferred();
        var templateCode = $("#formDraftTemplate").data("kendoComboBox").value();
        if (templateCode == "" || templateCode == null)
            return displayPopupNotification("Please select the template.", "warning");
        var formDetail = contravoucherservice.getDraftFormDetail_ByFormCode(templateCode, d7);
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
                            } else {
                                result[val.COLUMN_NAME] = val.COLUMN_VALUE;
                            }

                        }
                    }
                });

                $scope.BUDGET_MODAL.push({ ACC_CODE: "", BUDGET_FLAG: "", BUDGET: [] });
                $scope.BUDGET_CHILD_MODAL.push({ SERIAL_NO: "", BUDGET_CODE: "", AMOUNT: "", NARRATION: "" });
                $scope.SUBLEDGER_MODAL.push({ ACC_CODE: "", TRANSACTION_TYPE: "", VOUCHER_AMOUNT: "", SUBLEDGER_AMOUNT: "", REMAINING_AMOUNT: "", SUBLEDGER: [] });
                $scope.SUBLEDGER_CHILD_MODAL.push({ SERIAL_NO: "", SUB_CODE: "", SUB_EDESC: "", PARTY_TYPE_CODE: "", AMOUNT: "", PARTICULARS: "", REFRENCE: "" });

                var subledgerModel = angular.copy($scope.SUBLEDGER_MODAL[0]);
                var subledgerChildModel = angular.copy($scope.SUBLEDGER_CHILD_MODAL[0]);

                $scope.dynamicSubLedgerModalData.push(subledgerModel);
                if ($scope.dynamicSubLedgerModalData[i].SUBLEDGER == undefined) {
                    $scope.dynamicSubLedgerModalData[i].SUBLEDGER = [];
                    $scope.dynamicSubLedgerModalData[i].SUBLEDGER.push(subledgerChildModel);
                }

                var budgetModel = angular.copy($scope.BUDGET_MODAL[0]);
                var budgetChildModel = angular.copy($scope.BUDGET_CHILD_MODAL[0]);

                $scope.dynamicModalData.push(budgetModel);
                if ($scope.dynamicModalData[i].BUDGET == undefined) {
                    $scope.dynamicModalData[i].BUDGET = [];
                    $scope.dynamicModalData[i].BUDGET.push(budgetChildModel);
                }

                var tempCopy = angular.copy($scope.childModelTemplate);
                $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
                $scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, result));

            }
            angular.forEach($scope.childModels, function (valuec, keyc) {

                if ($scope.childModels[keyc].hasOwnProperty("ACC_CODE")) {
                    if ($scope.childModels[keyc].ACC_CODE != "") {
                        var ireq = "/api/TemplateApi/getACCEdesc?code=" + $scope.childModels[keyc].ACC_CODE;
                        $http.get(ireq).then(function (results11) {
                            setTimeout(function () {
                                if (results11.data != "") {
                                    $("#idaccount_" + keyc).data('kendoComboBox').dataSource.data([{ ACC_CODE: $scope.childModels[keyc].ACC_CODE, ACC_EDESC: results11.data, Type: "code" }]);
                                }

                            }, 50);
                        });
                    }

                }
            });
            $("#getDraftModal").modal("toggle");

        });


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

        for (var i = 0; i < $scope.childModels.length; i++) {
            if ("AMOUNT" in $scope.childModels[i]) {

            }
            else {
                $scope.childModels[i]["AMOUNT"] = "";
            }

            if ("BUDGET_FLAG" in $scope.childModels[i]) {

            }
            else {
                $scope.childModels[i]["BUDGET_FLAG"] = "";
            }
        }

        var childcolumnkeys = "";
        for (key in $scope.childModels[0]) {


            childcolumnkeys += key + ",";

        }
        $scope.masterModels;
        var TempCode = $scope.tempCode;
        var model = {
            Save_Flag: saveflag,
            FORM_TEMPLATE: $scope.saveAsDraft,
            Table_Name: tablename,
            Form_Code: formcode,
            Order_No: orderno,
            TempCode: TempCode,
            Master_COLUMN_VALUE: JSON.stringify($scope.masterModels),
            Child_COLUMNS: childcolumnkeys,
            Child_COLUMN_VALUE: JSON.stringify($scope.childModels),
            Custom_COLUMN_VALUE: JSON.stringify($scope.customModels),
            BUDGET_TRANS_VALUE: angular.toJson($scope.dynamicModalData),
            SUB_LEDGER_VALUE: angular.toJson($scope.dynamicSubLedgerModalData),
            DR_TOTAL_VALUE: angular.toJson($scope.accsummary.drTotal),
            CR_TOTAL_VALUE: angular.toJson($scope.accsummary.crTotal),
        };

        var staturl = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/SaveAsDraftFormData";
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
                    TEMPLATE_NDESC: "",
                    TEMPLATE_ASSIGNEE: "",
                    ASSIGNED_DATE: ""
                }
                displayPopupNotification("Saved Successfully", "Success");
            }
            $scope.loadingBtnReset();
        },
            function errorCallback(response) {
                displayPopupNotification("Something went wrong.Please try again later.", "error");
            });
    }

    $scope.printDiv = function (divName) {
        var printContents = document.getElementById(divName).innerHTML;

        var popupWin = window.open('', '_blank', 'width=800,height=800', 'orientation = portrait');
        //popupWin.ScreenOrientation = "Portrait";
        popupWin.document.open();

        popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
        popupWin.document.close();
    }
    //subin changes

    //----------------------Draft end-----------------------------------
    $scope.SaveDocumentFormData = function (param) {
        debugger;
        //check master dr and  cr amount validation
        $scope.loadingBtn();
        var maslen = $scope.MasterFormElement.length;
        for (var i = 0; i < maslen; i++) {
            var masterelementname = $scope.MasterFormElement[i].COLUMN_NAME;
            if (masterelementname != "MASTER_TRANSACTION_TYPE") {


                if ($scope.accsummary.diffAmount == 0) {
                    $scope.accsummary.diffAmount = "0.00";
                }

                if ($scope.accsummary.diffAmount !== "0.00") {
                    displayPopupNotification("Dr Amount & Cr Amount must be equal", "warning");
                    return $scope.loadingBtnReset();
                }
            }
        };

        //check master acc_code validation
        if ($scope.masterModels.hasOwnProperty("MASTER_ACC_CODE")) {

            //   //to solve problem in mastercode binding for update
            if ($scope.orderno !== "undefined") {
                if ($scope.masterModels.MASTER_ACC_CODE === '' || $scope.masterModels.MASTER_ACC_CODE === null || $scope.masterModels.MASTER_ACC_CODE === undefined) {
                    if ($rootScope.mastervalidation === "") {
                        $scope.masterModels.MASTER_ACC_CODE = masteracccodeforupdate;
                    }

                }
            }

            var master_acc_code = $scope.masterModels.MASTER_ACC_CODE;
            if ($(".macccode").hasClass("borderRed") || master_acc_code == null || master_acc_code == "" || master_acc_code == undefined) {
                displayPopupNotification("Main Ledger Account Code is required", "warning");
                return $scope.loadingBtnReset();
            }
        };

        if ($scope.masterModels.hasOwnProperty("MASTER_AMOUNT")) {
            if ($scope.masterModels.MASTER_AMOUNT <= 0) {
                displayPopupNotification("Master Amount must be greater than zero", "warning");
                return $scope.loadingBtnReset();
            }
        }
        if ($scope.masterModels.hasOwnProperty("CHEQUE_NO")) {
            if ($scope.masterModels.CHEQUE_NO === null || $scope.masterModels.CHEQUE_NO === "") {
                displayPopupNotification("Cheque no is required", "warning");
                return $scope.loadingBtnReset();
            }
        }




        //check child validation
        var childlen = $scope.childModels.length;
        for (var i = 0; i < childlen; i++) {

            var child_acc_code = $scope.childModels[i].ACC_CODE;
            var child_amount = $scope.childModels[i].AMOUNT;

            //subledger validation
            if ($scope.childModels[0].hasOwnProperty("ACC_CODE")) {
                if ($(".cacccode").parent().parent().hasClass("borderRed") || child_acc_code == null || child_acc_code == "" || child_acc_code == undefined) {
                    displayPopupNotification("Sub Ledger Account Code is required", "warning");
                    return $scope.loadingBtnReset();
                }
            }
            //amount validation
            if ($scope.childModels[0].hasOwnProperty("AMOUNT")) {
                if (child_amount === null || child_amount === "" || child_amount === 0 || child_amount === 0.00) {
                    displayPopupNotification("Amount is required", "warning");
                    return $scope.loadingBtnReset();
                }
                if ($scope.childModels[0].hasOwnProperty("CHEQUE_NO")) {
                    if ($scope.childModels[i].CHEQUE_NO === null || $scope.childModels[i].CHEQUE_NO === "") {
                        displayPopupNotification("Cheque no is required", "warning");
                        return $scope.loadingBtnReset();
                    }
                }
                if (child_amount === undefined) {
                    displayPopupNotification("Enter Amount Correctly", "error");
                    return $scope.loadingBtnReset();
                }
            }
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
            if ($scope.childModels[0].hasOwnProperty("CALC_UNIT_PRICE")) {
                var calc_rate = $scope.childModels[i].UNIT_PRICE;
                if (calc_rate == null || calc_rate == "") {
                    displayPopupNotification("Calcutaled Rate/Unit is required", "warning");
                    return $scope.loadingBtnReset();
                }
                if (calc_rate === undefined) {
                    displayPopupNotification("Enter Calcutaled Amount Correctly", "error");
                    return $scope.loadingBtnReset();
                }
            };
            if ($scope.childModels[i].hasOwnProperty("TRANSACTION_TYPE")) {
                $scope.dynamicSubLedgerModalData[0].TRANSACTION_TYPE = $scope.childModels[i].TRANSACTION_TYPE;
            }

        };


        showloader();
        var saveflag = param;
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

        for (var i = 0; i < $scope.childModels.length; i++) {
            if ("AMOUNT" in $scope.childModels[i]) {

            }
            else {
                $scope.childModels[i]["AMOUNT"] = "";
            }

            if ("BUDGET_FLAG" in $scope.childModels[i]) {

            }
            else {
                $scope.childModels[i]["BUDGET_FLAG"] = "";
            }
        }

        var childcolumnkeys = "";
        for (key in $scope.childModels[0]) {


            childcolumnkeys += key + ",";

        }

        $scope.SUB_LEDGER_VALUE_ARRAY = [];
        var SubLedgerModaldata = $.grep($scope.dynamicSubLedgerModalData, function (e) {

            return e.ACC_CODE != 0;
        });
        if (SubLedgerModaldata.length > 0) {
            $scope.SUB_LEDGER_VALUE_ARRAY = SubLedgerModaldata;
        }
        else {
            $scope.SUB_LEDGER_VALUE_ARRAY = [];
        }

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

        $scope.TDS_VALUE_ARRAY = [];
        var TDSModaldata = $.grep($scope.dynamicTDSModalData, function (e) {

            return e.ACC_CODE != 0;
        });
        if (TDSModaldata.length > 0) {
            $scope.TDS_VALUE_ARRAY = TDSModaldata;
        }
        else {
            $scope.TDS_VALUE_ARRAY = [];
        }

        $scope.VAT_VALUE_ARRAY = [];
        var VATModaldata = $.grep($scope.dynamicVATModalData, function (e) {

            return e.ACC_CODE != 0;
        });
        if (VATModaldata.length > 0) {
            $scope.VAT_VALUE_ARRAY = VATModaldata;
        }
        else {
            $scope.VAT_VALUE_ARRAY = [];
        }
        var extraChargeElement = $rootScope.rootdynamicExtraItemChargeModalData;
        $scope.masterModels;
        var model = {
            Save_Flag: saveflag,
            Table_Name: tablename,
            Form_Code: formcode,
            Order_No: orderno,
            Master_COLUMN_VALUE: JSON.stringify($scope.masterModels),
            Child_COLUMNS: childcolumnkeys,
            Child_COLUMN_VALUE: JSON.stringify($scope.childModels),
            Custom_COLUMN_VALUE: JSON.stringify($scope.customModels),
            BUDGET_TRANS_VALUE: JSON.stringify($scope.BUDGET_TRANS_VALUE_ARRAY),
            //BUDGET_TRANS_VALUE: angular.toJson($scope.dynamicModalData),
            //BUDGET_TRANS_VALUE: $scope.BUDGET_TRANS_VALUE_ARRAY,
            SUB_LEDGER_VALUE: JSON.stringify($scope.SUB_LEDGER_VALUE_ARRAY),
            //SUB_LEDGER_VALUE: $scope.SUB_LEDGER_VALUE_ARRAY,
            //TDS_VALUE: angular.toJson($scope.dynamicTDSModalData),
            //VAT_VALUE: angular.toJson($scope.dynamicVATModalData),
            //TDS_VALUE: $scope.TDS_VALUE_ARRAY,
            TDS_VALUE: JSON.stringify($scope.TDS_VALUE_ARRAY),
            //VAT_VALUE: $scope.VAT_VALUE_ARRAY,
            VAT_VALUE: JSON.stringify($scope.VAT_VALUE_ARRAY),
            DR_TOTAL_VALUE: angular.toJson($scope.accsummary.drTotal),
            CR_TOTAL_VALUE: angular.toJson($scope.accsummary.crTotal),
            charge_tran_value: JSON.stringify(extraChargeElement),
        };

        var staturl = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/SaveFinancialFormData";
        var response = $http({
            method: "POST",
            data: model,
            url: staturl,
            contentType: "application/json",
            dataType: "json"
        });
        return response.then(function (data) {

            if (data.data.MESSAGE == "INSERTED") {

                hideloader();
                var generateMsg = "Voucher Saved Successfully! </br> Voucher no: " + data.data.VoucherNo;
                $scope.dzvouchernumber = data.data.VoucherNo;
                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;
                $scope.dzSessionCode = data.data.SessionNo;
                myFinanceDropzone.processQueue();
                DisplayBarNotificationMessage(generateMsg);
                //$scope.reset();
                //$scope.someFn();
                var host = $window.location.host;
                var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Template/SplitterIndex#!DT/MenuSplitter/01";
                setTimeout(function () {
                    $window.location.href = landingUrl;
                }, 1000);
                //$scope.$apply();
            }
            //else if (data.data.MESSAGE == "INSERTEDANDCONTINUE") {
            //    
            //    hideloader();
            //    var generateMsg = "Voucher Saved Successfully! </br> Voucher no: " + data.data.VoucherNo;
            //    $scope.dzvouchernumber = data.data.VoucherNo;
            //    $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
            //    $scope.dzformcode = data.data.FormCode;
            //    $scope.dzSessionCode = data.data.SessionNo;
            //    myFinanceDropzone.processQueue();
            //    DisplayBarNotificationMessage(generateMsg);

            //    $scope.reset();
            //    $scope.masterChildData = [];

            //    angular.forEach($scope.MasterFormElement, function (value, key) {
            //        if (value.MASTER_CHILD_FLAG == 'M' && value.DISPLAY_FLAG == 'Y') {
            //            $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
            //            $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
            //            if (value['COLUMN_NAME'].indexOf('DATE') > -1) {
            //                $scope.masterModels[value['COLUMN_NAME']] = $filter('date')(new Date(), 'dd-MMM-yyyy');
            //            }
            //        }
            //    });
            //    angular.forEach($scope.ChildFormElement[0].element, function (value, key) {
            //        if (value.MASTER_CHILD_FLAG == 'C' && value.DISPLAY_FLAG == 'Y') {
            //            $scope.childModels[0][value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
            //        }
            //    })
            //    $scope.someFn();


            //}
            else if (data.data.MESSAGE == "SAVEANDPRINT") {
                hideloader();
                $scope.PrintDocumentVouhcer(data);

            }
            else if (data.data.MESSAGE == "UPDATEDANDPRINT") {
                hideloader();

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
                                case 'ACC_CODE':
                                    //$scope.childModels[ind]["ACC_CODE"] = $($(".caccount_" + ind)[$(".caccount_" + ind).length - 1]).data("kendoComboBox").dataItem().ACC_EDESC;
                                    //subin changes
                                    $scope.childModels[ind]["ACC_EDESC"] = $($(".caccount_" + ind)[$(".caccount_" + ind).length - 1]).data("kendoComboBox").dataItem().ACC_EDESC;
                                    break;
                                case "TO_LOCATION_CODE":
                                    $scope.childModels[ind]["TO_LOCATION_CODE"] = $($(".ctolocation_" + ind)[$(".ctolocation_" + ind).length - 1]).data("kendoComboBox").dataItem().LocationName;
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
                $scope.print_body_col1 = print_child;
                $scope.print_body_col2 = print_child;
                for (var i = 0; i < $scope.print_body_col1.length; i++) {

                    $scope.print_body_col1[i].element = $.grep($scope.print_body_col1[i].element, function (v) { return (v) });
                    for (var j = 0; j < $scope.print_body_col1[i].element.length; j++) {

                        if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "PARTICULARS") {
                            $scope.print_body_col1[i].element[j] = undefined;
                            continue;
                        }
                        if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "CALC_UNIT_PRICE") {
                            $scope.print_body_col1[i].element[j] = undefined;
                            continue;
                        }
                        if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "TOTAL_PRICE") {
                            $scope.print_body_col1[i].element[j] = undefined;
                            continue;
                        }
                        if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "QUANTITY") {
                            $scope.print_body_col1[i].element[j] = undefined;
                            continue;
                        }
                        if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "UNIT_PRICE") {
                            $scope.print_body_col1[i].element[j] = undefined;
                        }
                        if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "CALC_TOTAL_PRICE") {
                            $scope.print_body_col1[i].element[j] = undefined;
                            continue;
                        }
                        if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "COMPLETED_QUANTITY") {
                            $scope.print_body_col1[i].element[j] = undefined;
                            continue;
                        }
                        if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "TRANSACTION_TYPE") {
                            $scope.print_body_col1[i].element[j] = undefined;
                            continue;
                        }
                        if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "BUDGET_FLAG") {
                            $scope.print_body_col1[i].element[j] = undefined;
                            continue;
                        }
                        if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "MU_CODE") {
                            $scope.print_body_col1[i].element[j] = undefined;
                            continue;
                        }
                        if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "CALC_QUANTITY") {
                            $scope.print_body_col1[i].element[j] = undefined;
                            continue;
                        }

                    }
                }
                for (var i = 0; i < $scope.print_body_col1.length; i++) {
                    $scope.print_body_col1[i].element = $.grep($scope.print_body_col1[i].element, function (v) { return (v) });
                }
                $scope.print_body_col1 = JSON.parse(JSON.stringify($scope.print_body_col1));
                for (var i = 0; i < $scope.print_body_col2.length; i++) {

                    $scope.print_body_col2[i].element = $.grep($scope.print_body_col2[i].element, function (v) { return (v) });
                    for (var j = 0; j < $scope.print_body_col2[i].element.length; j++) {


                        if ($scope.print_body_col2[i].element[j].COLUMN_NAME === "CALC_UNIT_PRICE") {
                            $scope.print_body_col2[i].element[j] = undefined;
                            continue;
                        }
                        if ($scope.print_body_col2[i].element[j].COLUMN_NAME === "TOTAL_PRICE") {
                            $scope.print_body_col2[i].element[j] = undefined;
                            continue;
                        }
                        if ($scope.print_body_col2[i].element[j].COLUMN_NAME === "QUANTITY") {
                            $scope.print_body_col2[i].element[j] = undefined;
                            continue;
                        }
                        if ($scope.print_body_col2[i].element[j].COLUMN_NAME === "UNIT_PRICE") {
                            $scope.print_body_col2[i].element[j] = undefined;
                            continue;
                        }
                        if ($scope.print_body_col2[i].element[j].COLUMN_NAME === "CALC_TOTAL_PRICE") {
                            $scope.print_body_col2[i].element[j] = undefined;
                            continue;
                        }
                        if ($scope.print_body_col2[i].element[j].COLUMN_NAME === "COMPLETED_QUANTITY") {
                            $scope.print_body_col2[i].element[j] = undefined;
                            continue;
                        }


                        if ($scope.print_body_col2[i].element[j].COLUMN_NAME === "MU_CODE") {
                            $scope.print_body_col2[i].element[j] = undefined;
                            continue;
                        }
                        if ($scope.print_body_col2[i].element[j].COLUMN_NAME === "CALC_QUANTITY") {
                            $scope.print_body_col2[i].element[j] = undefined;
                            continue;
                        }

                    }
                }
                for (var i = 0; i < $scope.print_body_col2.length; i++) {
                    $scope.print_body_col2[i].element = $.grep($scope.print_body_col2[i].element, function (v) { return (v) });
                }
                $scope.print_body_col2 = JSON.parse(JSON.stringify($scope.print_body_col2));

                $.each($scope.childModels, function (key, val) {

                    if (val.hasOwnProperty("ACC_CODE")) {

                        if ($scope.dynamicModalData != 'undefined') {
                            if ($scope.dynamicModalData.length > 0) {
                                if ($scope.childModels[key].ACC_CODE == $scope.dynamicModalData[key].ACC_CODE) {
                                    if ($scope.dynamicModalData[key].BUDGET != undefined) {
                                        $.each($scope.dynamicModalData[key].BUDGET, function (keys, vals) {

                                            if ($($(".BUDGETCODE_" + keys)[$(".BUDGETCODE_" + keys).length - 1]).data("kendoComboBox").dataItem() !== undefined) {
                                                $scope.dynamicModalData[key].BUDGET[keys].BUDGET_CODE =
                                                    $($(".BUDGETCODE_" + keys)[$(".BUDGETCODE_" + keys).length - 1]).data("kendoComboBox").dataItem().BUDGET_EDESC;
                                            }
                                        })
                                    }
                                }
                            }
                        }

                    }

                });


                $scope.dzvouchernumber = data.data.VoucherNo;
                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;
                displayPopupNotification("Data succesfully Saved.", "success");
                $scope.TodayDate = AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD'));

                $scope.amountinword = convertNumberToWords($scope.masterModels["MASTER_AMOUNT"]);

                $("#saveAndPrintModal").modal("toggle");
            }
            else if (data.data.MESSAGE == "INSERTEDANDCONTINUE") {
                hideloader();
                var generateMsg = "Voucher Saved Successfully! </br> Voucher no: " + data.data.VoucherNo;
                $scope.dzvouchernumber = data.data.VoucherNo;
                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;
                $scope.dzSessionCode = data.data.SessionNo;
                myFinanceDropzone.processQueue();
                DisplayBarNotificationMessage(generateMsg);
                $scope.reset();


            }
            else if (data.data.MESSAGE == "UPDATED") {

                DisplayBarNotificationMessage("Data succesfully updated.", "success");
                $scope.dzvouchernumber = data.data.VoucherNo;
                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;
                $scope.dzSessionCode = data.data.SessionNo;
                myFinanceDropzone.processQueue();
                var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Template/SplitterIndex#!DT/MenuSplitter/01";
                //$window.location.href = landingUrl;
                setTimeout(function () {
                    $window.location.href = landingUrl;
                }, 1000);
            }

            else {
                hideloader();
                displayPopupNotification("Something went wrong.Please try again later.", "error");

            }
            $scope.loadingBtnReset();

        },
            function errorCallback(response) {
                hideloader();
                displayPopupNotification("Something went wrong.Please try again later.", "error");
                $scope.loadingBtnReset();
            });
    };
    $scope.DeleteFinanceDocument = function () {
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

                    var delRes = contravoucherservice.deleteFinanceVoucher($scope.OrderNo, $scope.formcode);
                    delRes.then(function (dRes) {
                        debugger;
                        if (dRes.data.MESSAGE === "DELETED") {

                            displayPopupNotification("Data succesfully deleted ", "success");
                            var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Template/SplitterIndex#!DT/MenuSplitter/01";
                            //$window.location.href = landingUrl;
                            setTimeout(function () {
                                $window.location.href = landingUrl;
                            }, 1000);
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
    $scope.getNepaliDate = function (date) {
        return AD2BS(moment(date).format('YYYY-MM-DD'));
    };
    $scope.reset = function () {

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
            BUDGET_CODE: "",
            AMOUNT: "",
            NARRATION: ""
        });
        $scope.dynamicModalData = [{
            ACC_CODE: "",
            BUDGET_FLAG: "",
            BUDGET: [{
                SERIAL_NO: "",
                BUDGET_CODE: "",
                AMOUNT: "",
                NARRATION: "",
            }]

        }];

        $scope.dynamicSubLedgerModalData = [{
            ACC_CODE: 0,
            TRANSACTION_TYPE: "",
            VOUCHER_AMOUNT: "",
            SUBLEDGER_AMOUNT: "",
            REMAINING_AMOUNT: "",
            SUBLEDGER: [{
                SERIAL_NO: "",
                SUB_CODE: "",
                SUB_EDESC: "",
                PARTY_TYPE_CODE: "",
                AMOUNT: "",
                PARTICULARS: "",
                REFRENCE: ""
            }]
        }];
        $scope.someFn();
        $scope.refreshDraft();
    };
    $scope.resetCP = function () {

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
            BUDGET_CODE: "",
            AMOUNT: "",
            NARRATION: ""
        });
        $scope.dynamicModalData = [{
            ACC_CODE: "",
            BUDGET_FLAG: "",
            BUDGET: [{
                SERIAL_NO: "",
                BUDGET_CODE: "",
                AMOUNT: "",
                NARRATION: "",
            }]

        }];

        $scope.dynamicSubLedgerModalData = [{
            ACC_CODE: 0,
            TRANSACTION_TYPE: "",
            VOUCHER_AMOUNT: "",
            SUBLEDGER_AMOUNT: "",
            REMAINING_AMOUNT: "",
            SUBLEDGER: [{
                SERIAL_NO: "",
                SUB_CODE: "",
                SUB_EDESC: "",
                PARTY_TYPE_CODE: "",
                AMOUNT: "",
                PARTICULARS: "",
                REFRENCE: ""
            }]
        }];
        $scope.someFnCP();
        $scope.refreshDraft();
    };

    //amount check validation and sum amount
    $scope.validaccsum = function (index) {
        $scope.childModels[index];
        var child_amount = $scope.childModels[index].AMOUNT;
        if (child_amount != null && child_amount != "" && child_amount !== undefined) {
            $scope.childModels[index].AMOUNT = parseFloat(child_amount.toFixed(2));
        }
        if (child_amount === undefined) {
            $(".AMOUNT_" + index).parent().css({ "border": "solid 1px red" });
            return;
        }
        else {
            $(".AMOUNT_" + index).parent().css({ "border": "none" });
        }
        $scope.accsum(index);
    }

    $scope.ChangeTransactionType = function (index) {
        debugger;
        console.log("Globeltest", window.globalIndex)
        $scope.childModels[window.globalIndex][TRANSACTION_TYPE] = $scope.dynamicSubLedgerModalData[index].TRANSACTION_TYPE;
        //  $scope.dynamicSubLedgerModalData[index].TRANSACTION_TYPE = $scope.childModels[window.globalIndex][TRANSACTION_TYPE];
    };
    $scope.ChangeDocTypeVat = function (index) {
        debugger;
        var selectedDocType = $scope.dynamicVATModalData[index].DOC_TYPE;

    };
    $scope.accsum = function (index) {

        var amount = $("AMOUNT_" + index).val();
        if ($scope.dynamicSubLedgerModalData[index] != undefined) {
            if ($scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT != null && $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT != undefined && $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT != "") {
                if ($scope.childModels[index].AMOUNT > $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT) {
                    $scope.dynamicSubLedgerModalData[index].REMAINING_AMOUNT = $scope.childModels[index].AMOUNT - $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT;
                    $scope.dynamicSubLedgerModalData[index].VOUCHER_AMOUNT = $scope.childModels[index].AMOUNT;
                    $('.remainingamt').removeClass("borderred");
                }
                else {
                    $scope.dynamicSubLedgerModalData[index].REMAINING_AMOUNT = $scope.childModels[index].AMOUNT - $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT;
                    $('.remainingamt').addClass("borderred");
                }
            }
        }
        //if (typeof $scope.childModels[index][TRANSACTION_TYPE] !== "undefined" && $scope.childModels[index][TRANSACTION_TYPE] != null) {
        var drsum = 0;
        var crsum = 0;
        var maslen = $scope.MasterFormElement.length;
        for (var i = 0; i < maslen; i++) {
            var masterelementname = $scope.MasterFormElement[i].COLUMN_NAME;
            if (masterelementname == "MASTER_TRANSACTION_TYPE") {

                angular.forEach($scope.childModels, function (value, key) {
                    if (typeof value[AMOUNT] !== 'undefined' && value[AMOUNT] !== null) {

                        if (value[TRANSACTION_TYPE] === "DR" && (value[AMOUNT] !== "")) {
                            drsum = drsum + value[AMOUNT];
                            //$scope.$apply(function () {
                            $scope.accsummary.drTotal = parseFloat(drsum.toFixed(2));
                            $scope.accsummary.crTotal = parseFloat(drsum.toFixed(2));
                            //});
                        }
                        if (value[TRANSACTION_TYPE] === "CR" && (value[AMOUNT] !== "")) {
                            crsum = crsum + value[AMOUNT];
                            //$scope.accsummary.crTotal = crsum;
                            //$scope.accsummary.drTotal = crsum;
                            $scope.accsummary.crTotal = parseFloat(crsum.toFixed(2));
                            $scope.accsummary.drTotal = parseFloat(crsum.toFixed(2));
                        }
                    }
                    else {
                        if (value[TRANSACTION_TYPE] === "DR" && value[AMOUNT] === null) {
                            value[AMOUNT] = 0;
                            drsum = drsum + value[AMOUNT];
                            $scope.accsummary.drTotal = parseFloat(drsum.toFixed(2));
                            $scope.accsummary.crTotal = parseFloat(drsum.toFixed(2));
                            $scope.childModels[key].AMOUNT = "";
                        }
                        if (value[TRANSACTION_TYPE] === "CR" && value[AMOUNT] === null) {
                            value[AMOUNT] = 0;
                            drsum = drsum + value[AMOUNT];
                            $scope.accsummary.crTotal = parseFloat(crsum.toFixed(2));
                            $scope.accsummary.drTotal = parseFloat(crsum.toFixed(2));
                            $scope.childModels[key].AMOUNT = "";
                        }
                    }
                });
                $scope.accsummary.diffAmount = ($scope.accsummary.drTotal - $scope.accsummary.crTotal).toFixed(2);
                $scope.masterModels["MASTER_AMOUNT"] = parseFloat($scope.accsummary.drTotal.toFixed(2));
                break;
            }
        };
        //old

        //if (masterelementname != "MASTER_TRANSACTION_TYPE") {
        //    //if ($scope.childModels[index][TRANSACTION_TYPE] == "DR") {
        //    angular.forEach($scope.childModels, function (value, key) {
        //        if (typeof value[AMOUNT] !== 'undefined' && value[AMOUNT] != null) {
        //            //console.log('value', value);
        //            if (value[TRANSACTION_TYPE] == "DR" && (value[AMOUNT] != "" || value[AMOUNT] != 0)) {
        //                drsum = drsum + value[AMOUNT];
        //            }
        //        }
        //        if (value[AMOUNT] == null || value[AMOUNT] == "" || value[AMOUNT] == undefined || value[AMOUNT] == 0) {
        //            if (value[TRANSACTION_TYPE] == "DR") {
        //                //old
        //                //value[AMOUNT] = 0;
        //                //crsum = crsum + value[AMOUNT];
        //                //$scope.childModels[key].AMOUNT = "";

        //               //new
        //                if ($scope.accsummary.diffAmount == 0) {
        //                    $scope.childModels[key].AMOUNT = 0;
        //                }
        //                else {
        //                    if (crsum > drsum) {
        //                        value[AMOUNT] = crsum -drsum;
        //                        drsum = value[AMOUNT];
        //                        $scope.childModels[key].AMOUNT = drsum;
        //                    }
        //                }

        //            }
        //        }
        //        $scope.accsummary.drTotal = drsum.toFixed(2);
        //    });

        //    // }
        //    // if ($scope.childModels[index][TRANSACTION_TYPE] == "CR") {
        //    angular.forEach($scope.childModels, function (value, key) {

        //        if (typeof value[AMOUNT] !== 'undefined' && value[AMOUNT] != null) {
        //            //console.log('value', value);
        //            if (value[TRANSACTION_TYPE] == "CR" && (value[AMOUNT] != "" || value[AMOUNT] != 0)) {
        //                crsum = crsum + value[AMOUNT];
        //            }
        //        }
        //        if (value[AMOUNT] == null || value[AMOUNT] == "" || value[AMOUNT] == undefined || value[AMOUNT] == 0) {
        //            if (value[TRANSACTION_TYPE] == "CR") {
        //                //old
        //                //value[AMOUNT] = 0;
        //                //crsum = crsum + value[AMOUNT];
        //                //$scope.childModels[key].AMOUNT = "";

        //                //new                        
        //                if ($scope.accsummary.diffAmount == 0) {
        //                    $scope.childModels[key].AMOUNT = 0;
        //                }
        //                else {                            
        //                    if (drsum > crsum) {
        //                        value[AMOUNT] = drsum - crsum;
        //                        crsum = value[AMOUNT];
        //                        $scope.childModels[key].AMOUNT = crsum;
        //                    }
        //                }
        //            }
        //        }
        //        $scope.accsummary.crTotal = crsum.toFixed(2);
        //    });

        //    if ($scope.masterModels["MASTER_TRANSACTION_TYPE"] == "DR") {
        //        $scope.masterModels["MASTER_AMOUNT"] = parseFloat($scope.accsummary.crTotal.toFixed(2));
        //    }
        //    if ($scope.masterModels["MASTER_TRANSACTION_TYPE"] == "CR") {
        //        $scope.masterModels["MASTER_AMOUNT"] = parseFloat($scope.accsummary.drTotal.toFixed(2));
        //    }
        //    $scope.accsummary.diffAmount = ($scope.accsummary.drTotal - $scope.accsummary.crTotal).toFixed(2);
        //    //if ($("#ledgermodal").data()['bs.modal'].isShown) {
        //    //    $('#ledgermodal').modal('toggle');
        //    //}

        //}
        ////$scope.$apply();

        //new

        if (masterelementname != "MASTER_TRANSACTION_TYPE") {
            angular.forEach($scope.childModels, function (value, key) {
                if (typeof value[AMOUNT] !== 'undefined' && value[AMOUNT] != null) {
                    //console.log('value', value);
                    if (value[TRANSACTION_TYPE] == "DR" && (value[AMOUNT] != "" || value[AMOUNT] != 0)) {
                        drsum = drsum + value[AMOUNT];
                        $scope.childModels[key].BUDGET_FLAG = "E";
                    }
                }
                if (typeof value[AMOUNT] !== 'undefined' && value[AMOUNT] != null) {
                    //console.log('value', value);
                    if (value[TRANSACTION_TYPE] == "CR" && (value[AMOUNT] != "" || value[AMOUNT] != 0)) {
                        crsum = crsum + value[AMOUNT];
                        $scope.childModels[key].BUDGET_FLAG = "L";
                    }
                }
                if (value[AMOUNT] == null || value[AMOUNT] == "" || value[AMOUNT] == undefined || value[AMOUNT] == 0) {
                    if (value[TRANSACTION_TYPE] == "DR") {
                        $scope.childModels[key].BUDGET_FLAG = "E";
                        if ($scope.accsummary.diffAmount == 0) {
                            $scope.childModels[key].AMOUNT = 0;
                        }
                        else {
                            if (crsum > drsum) {
                                value[AMOUNT] = crsum - drsum;
                                drsum = drsum + value[AMOUNT];
                                $scope.childModels[key].AMOUNT = value[AMOUNT];
                            }
                        }
                    }
                    if (value[TRANSACTION_TYPE] == "CR") {
                        $scope.childModels[key].BUDGET_FLAG = "L";
                        if ($scope.accsummary.diffAmount == 0) {
                            $scope.childModels[key].AMOUNT = 0;
                        }
                        else {
                            if (drsum > crsum) {
                                value[AMOUNT] = drsum - crsum;
                                crsum = crsum + value[AMOUNT];
                                $scope.childModels[key].AMOUNT = value[AMOUNT];
                            }
                        }
                    }
                }
                $scope.accsummary.drTotal = drsum.toFixed(2);
                $scope.accsummary.crTotal = crsum.toFixed(2);
            });
            if ($scope.masterModels["MASTER_TRANSACTION_TYPE"] == "DR") {
                $scope.masterModels["MASTER_AMOUNT"] = parseFloat($scope.accsummary.crTotal.toFixed(2));
            }
            if ($scope.masterModels["MASTER_TRANSACTION_TYPE"] == "CR") {
                $scope.masterModels["MASTER_AMOUNT"] = parseFloat($scope.accsummary.drTotal.toFixed(2));
            }
            $scope.accsummary.diffAmount = ($scope.accsummary.drTotal - $scope.accsummary.crTotal).toFixed(2);
            //if ($("#ledgermodal").data()['bs.modal'].isShown) {
            //    $('#ledgermodal').modal('toggle');
            //}
        }
    };


    $scope.PrintDocumentVouhcer = function (data) {

        debugger;
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
                        case 'ACC_CODE':
                            //$scope.childModels[ind]["ACC_CODE"] = $($(".caccount_" + ind)[$(".caccount_" + ind).length - 1]).data("kendoComboBox").dataItem().ACC_EDESC;
                            //subin changes
                            $scope.childModels[ind]["ACC_EDESC"] = $($(".caccount_" + ind)[$(".caccount_" + ind).length - 1]).data("kendoComboBox").dataItem().ACC_EDESC;
                            break;
                        case "TO_LOCATION_CODE":
                            $scope.childModels[ind]["TO_LOCATION_CODE"] = $($(".ctolocation_" + ind)[$(".ctolocation_" + ind).length - 1]).data("kendoComboBox").dataItem().LocationName;
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
        $scope.print_body_col1 = print_child;
        $scope.print_body_col2 = print_child;
        for (var i = 0; i < $scope.print_body_col1.length; i++) {

            $scope.print_body_col1[i].element = $.grep($scope.print_body_col1[i].element, function (v) { return (v) });
            for (var j = 0; j < $scope.print_body_col1[i].element.length; j++) {

                if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "PARTICULARS") {
                    $scope.print_body_col1[i].element[j] = undefined;
                    continue;
                }
                if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "CALC_UNIT_PRICE") {
                    $scope.print_body_col1[i].element[j] = undefined;
                    continue;
                }
                if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "TOTAL_PRICE") {
                    $scope.print_body_col1[i].element[j] = undefined;
                    continue;
                }
                if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "QUANTITY") {
                    $scope.print_body_col1[i].element[j] = undefined;
                    continue;
                }
                if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "UNIT_PRICE") {
                    $scope.print_body_col1[i].element[j] = undefined;
                }
                if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "CALC_TOTAL_PRICE") {
                    $scope.print_body_col1[i].element[j] = undefined;
                    continue;
                }
                if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "COMPLETED_QUANTITY") {
                    $scope.print_body_col1[i].element[j] = undefined;
                    continue;
                }
                if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "TRANSACTION_TYPE") {
                    $scope.print_body_col1[i].element[j] = undefined;
                    continue;
                }
                if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "BUDGET_FLAG") {
                    $scope.print_body_col1[i].element[j] = undefined;
                    continue;
                }
                if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "MU_CODE") {
                    $scope.print_body_col1[i].element[j] = undefined;
                    continue;
                }
                if ($scope.print_body_col1[i].element[j].COLUMN_NAME === "CALC_QUANTITY") {
                    $scope.print_body_col1[i].element[j] = undefined;
                    continue;
                }

            }
        }
        for (var i = 0; i < $scope.print_body_col1.length; i++) {
            $scope.print_body_col1[i].element = $.grep($scope.print_body_col1[i].element, function (v) { return (v) });
        }
        $scope.print_body_col1 = JSON.parse(JSON.stringify($scope.print_body_col1));

        for (var i = 0; i < $scope.print_body_col2.length; i++) {

            $scope.print_body_col2[i].element = $.grep($scope.print_body_col2[i].element, function (v) { return (v) });
            for (var j = 0; j < $scope.print_body_col2[i].element.length; j++) {


                if ($scope.print_body_col2[i].element[j].COLUMN_NAME === "CALC_UNIT_PRICE") {
                    $scope.print_body_col2[i].element[j] = undefined;
                    continue;
                }
                if ($scope.print_body_col2[i].element[j].COLUMN_NAME === "TOTAL_PRICE") {
                    $scope.print_body_col2[i].element[j] = undefined;
                    continue;
                }
                if ($scope.print_body_col2[i].element[j].COLUMN_NAME === "QUANTITY") {
                    $scope.print_body_col2[i].element[j] = undefined;
                    continue;
                }
                if ($scope.print_body_col2[i].element[j].COLUMN_NAME === "UNIT_PRICE") {
                    $scope.print_body_col2[i].element[j] = undefined;
                    continue;
                }
                if ($scope.print_body_col2[i].element[j].COLUMN_NAME === "CALC_TOTAL_PRICE") {
                    $scope.print_body_col2[i].element[j] = undefined;
                    continue;
                }
                if ($scope.print_body_col2[i].element[j].COLUMN_NAME === "COMPLETED_QUANTITY") {
                    $scope.print_body_col2[i].element[j] = undefined;
                    continue;
                }


                if ($scope.print_body_col2[i].element[j].COLUMN_NAME === "MU_CODE") {
                    $scope.print_body_col2[i].element[j] = undefined;
                    continue;
                }
                if ($scope.print_body_col2[i].element[j].COLUMN_NAME === "CALC_QUANTITY") {
                    $scope.print_body_col2[i].element[j] = undefined;
                    continue;
                }

            }
        }
        for (var i = 0; i < $scope.print_body_col2.length; i++) {
            $scope.print_body_col2[i].element = $.grep($scope.print_body_col2[i].element, function (v) { return (v) });
        }
        $scope.print_body_col2 = JSON.parse(JSON.stringify($scope.print_body_col2));

        $.each($scope.childModels, function (key, val) {
            if ($scope.dynamicModalData.length > 0) {
                if (val.hasOwnProperty("ACC_CODE")) {
                    if ($scope.dynamicModalData.length > 0) {
                        if ($scope.childModels[key].ACC_CODE == $scope.dynamicModalData[key].ACC_CODE) {
                            $.each($scope.dynamicModalData[key].BUDGET, function (keys, vals) {

                                $scope.dynamicModalData[key].BUDGET[keys].BUDGET_CODE =
                                    $($(".BUDGETCODE_" + keys)[$(".BUDGETCODE_" + keys).length - 1]).data("kendoComboBox").dataItem().BUDGET_EDESC;
                            })
                        }
                    }

                }
            }


        });


        $scope.dzvouchernumber = data.data.VoucherNo;
        $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
        $scope.dzformcode = data.data.FormCode;
        displayPopupNotification("Data succesfully Saved.", "success");
        $scope.TodayDate = AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD'));
        var master_amount = $scope.masterModels["MASTER_AMOUNT"];

        if (master_amount === "null" || master_amount === undefined || master_amount === "") {
            $scope.amountinword = convertNumberToWords($scope.accsummary.drTotal);
        }
        else {
            $scope.amountinword = convertNumberToWords($scope.masterModels["MASTER_AMOUNT"]);

        }

        if ($scope.masterModels.hasOwnProperty("MASTER_ACC_CODE")) {

            //   //to solve problem in mastercode binding for update

            if ($scope.masterModels.MASTER_ACC_CODE != '' || $scope.masterModels.MASTER_ACC_CODE != null || $scope.masterModels.MASTER_ACC_CODE != undefined) {
                if ($rootScope.mastervalidation != "") {
                    $scope.masteraccName = $("#masteracccode").data("kendoComboBox").dataItem().ACC_EDESC;
                }

            }



        };
        $("#saveAndPrintModal").modal("toggle");
    }


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
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                default:
                    displayPopupNotification("Data succesfully saved ", "success");
            }

        }, function errorCallback(response) {
            $scope.refresh();
            //displayPopupNotification(response.data.STATUS_CODE, "error");
            displayPopupNotification("Something went wrong.Please try again later.", "error");
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

    };


    $scope.refresh = function () {
        location.reload();
    };

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



    //vstart

    var formUrl = "/api/SetupApi/GetFormControlByFormCode?formcode=" + $scope.formcode;
    $http.get(formUrl).then(function (response) {
        if (response.data.length != 0) {
            $scope.formControlModelData = response.data;

        }

    });
    // vend
    //-------------------- Quick Setup End----------------------------


    $scope.Changesubledgercode = function (key, index, e) {
        debugger;

        $scope.isPresent = false;
        var data = $($(".subledgerfirst")[2]).data("kendoComboBox").dataSource.data();
        if ($scope.dynamicSubLedgerModalData[key].SUBLEDGER[index].SUB_CODE != "") {
            BindDealerBySubCode($scope.dynamicSubLedgerModalData[key].SUBLEDGER[index].SUB_CODE, $scope.dynamicSubLedgerModalData[key].SUBLEDGER[index].SUB_EDESC, key, index);
            //for (var i = 0; i < data.length; i++) {
            //    var SubCode = data[i].SUB_CODE;
            //    var SubEdesc = data[i].SUB_EDESC;

            //    if (data[i].SUB_CODE == $scope.dynamicSubLedgerModalData[key].SUBLEDGER[index].SUB_CODE) {
            //        $scope.isPresent = true;

            //        return;
            //    }
            //    else {
            //        $scope.isPresent = false;
            //    }
            //}
            //if ($scope.isPresent == false) {
            //    $scope.dynamicSubLedgerModalData[key].SUBLEDGER[index].SUB_CODE = "";
            //    displayPopupNotification("Please Enter Valid Code.", "warning");
            //}
        }


    }
    function BindDealerBySubCode(SubCode, searchText, key, index) {
        debugger;
        var getdealerByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetPartyTypeByFilterAndSubCode?filter=" + searchText + '&subCode=' + SubCode;

        $("#subcodedealercode_" + key + "_" + index).kendoComboBox({
            optionLabel: "--Select Dealer Code--",
            filter: "contains",
            dataTextField: "PARTY_TYPE_EDESC",
            dataValueField: "PARTY_TYPE_CODE",
            dataBound: function (e) {

                if (this.select() === -1) { //check whether any item is selected
                    this.select(0);
                    this.trigger("change");
                }
            },
            autobind: true,
            suggest: true,
            dataSource: {
                type: "json",
                serverFiltering: true,
                transport: {
                    read: {

                        url: getdealerByUrl,

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
            },
            index: 0,
            select: function (e) {

                $('#style-switcher').addClass('opened');
                $('#style-switcher').animate({ 'left': '-241px', 'width': '273px' });

            }
        });
    }
    $scope.masterModelDataFn = function (fn) {

        var saleOrderformDetail = contravoucherservice.getSalesOrderDetail_ByFormCodeAndOrderNo($scope.formcode, $scope.OrderNo, d4);

        $.when(d4).done(function (result) {
            fn(result);
            if ($scope.tempCode != undefined) {
                var formcode = $scope.formcode;
                var tempCode = $scope.tempCode;

                contravoucherservice.getDraftData_ByFormCodeAndTempCode(formcode, tempCode).then(function (result) {

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
                        }

                        else {

                            var ArrayIndex = result[i].SERIAL_NO - 1;
                            var COL_NAME = result[i].COLUMN_NAME;
                            var column_value = result[i].COLUMN_VALUE;
                            if (!isNaN(column_value)) {
                                column_value = parseInt(column_value);
                            }
                            $scope.childModels[ArrayIndex][COL_NAME] = column_value;
                        }

                    }
                    $scope.accsum();
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
    $scope.ShowSubledgerDetail = function (index) {
        if ($scope.dynamicSubLedgerModalData[index].SUBLEDGER !== undefined) {
            $scope.dynamicSubLedgerModalData[index].SUBLEDGER = $.grep($scope.dynamicSubLedgerModalData[index].SUBLEDGER, function (e) {

                return e.SERIAL_NO != "";
            });
            for (var u = 0; u < $scope.dynamicSubLedgerModalData[index].SUBLEDGER.length; u++) {
                BindDealerBySubCode($scope.dynamicSubLedgerModalData[index].SUBLEDGER[u].SUB_CODE, $scope.dynamicSubLedgerModalData[index].SUBLEDGER[u].SUB_EDESC, index, u);
            }

        }


        popupAccess = true;
        $scope.popUp(index);
    }
    $scope.ShowBudgetTranDetail = function (index) {

        if ($scope.dynamicModalData[index].BUDGET !== undefined) {
            $scope.dynamicModalData[index].BUDGET = $.grep($scope.dynamicModalData[index].BUDGET, function (e) {

                return e.SERIAL_NO != "";
            });
        }
        popupAccessBudget = true;
        $scope.popUpBudget(index);
    }
    $scope.ShowTdsTranDetail = function (index) {

        if ($scope.dynamicTDSModalData[index].CHILDTDS !== undefined) {
            $scope.dynamicTDSModalData[index].CHILDTDS = $.grep($scope.dynamicTDSModalData[index].CHILDTDS, function (e) {

                return e.SERIAL_NO != "";
            });
        }
        popupAccessTds = true;
        $scope.popUpTds(index);
    }
    $scope.ShowVatTranDetail = function (index) {

        if ($scope.dynamicVATModalData[index].CHILDVAT !== undefined) {
            $scope.dynamicVATModalData[index].CHILDVAT = $.grep($scope.dynamicVATModalData[index].CHILDVAT, function (e) {

                return e.SERIAL_NO != "";
            });
        }
        popupAccessVAT = true;
        $scope.popUpVAT(index);
    }
    $scope.DatabindForEdit = function (index) {
        $scope.$broadcast('DatabindForEdit', { index: index });
    }
    $scope.ReferenceList = [];
    var req = "/api/ContraVoucherApi/GetReferenceList?formcode=" + $scope.formcode;
    return $http.get(req).then(function (response) {

        $scope.ReferenceList = response.data;
    });

    var formCustomSetup = $http.get('/api/TemplateApi/GetFormCustomSetup?formCode=' + $scope.formcode);
    formCustomSetup.then(function (res) {

        if (res.data != null) {

            $scope.formCustomSetup = res.data;
            var customvalues = $scope.formCustomSetup;

            angular.forEach(customvalues, function (value, key) {

                this.push(value);
                $scope.customModels[value['FIELD_NAME']] = null;
            }, $scope.CustomFormElement);
        }
    });
    function GetPrimaryDateByTableName(tablename) {
        var primaryDateCol = "";
        if (tablename == "FA_SINGLE_VOUCHER" || tablename == "FA_DOUBLE_VOUCHER") {
            primaryDateCol = "VOUCHER_DATE";
        }
        else if (tablename == "IP_PURCHASE_MRR" || tablename == "IP_ADVICE_MRR" || tablename == "IP_PRODUCTION_MRR") {
            primaryDateCol = "MRR_DATE";
        }
        else if (tablename == "IP_PURCHASE_REQUEST") {
            primaryDateCol = "REQUEST_DATE";
        }
        else if (tablename == "IP_PURCHASE_INVOICE") {
            primaryDateCol = "INVOICE_DATE";
        }
        else if (tablename == "IP_PURCHASE_RETURN" || tablename == "SA_SALES_RETURN") {
            primaryDateCol = "RETURN_DATE";
        }
        else if (tablename == "IP_GOODS_REQUISITION") {
            primaryDateCol = "REQUISITION_DATE";
        }
        else if (tablename == "IP_QUOTATION_INQUIRY") {
            primaryDateCol = "QUOTE_DATE";
        }
        else if (tablename == "IP_TRANSFER_ISSUE" || tablename == "IP_GOODS_ISSUE" || tablename == "IP_GATE_PASS_ENTRY") {
            primaryDateCol = "ISSUE_DATE";
        }
        else if (tablename == "IP_PURCHASE_ORDER" || tablename == "SA_SALES_ORDER") {
            primaryDateCol = "ORDER_DATE";
        }
        else if (tablename == "SA_SALES_CHALAN") {
            primaryDateCol = "CHALAN_DATE";
        }
        else if (tablename == "SA_SALES_INVOICE") {
            primaryDateCol = "SALES_DATE";
        }
        return primaryDateCol;
    }

    function PrimaryColumnForTable(tablename) {

        var primarycolumn = "";
        if (tablename == "FA_SINGLE_VOUCHER" || tablename == "FA_DOUBLE_VOUCHER" || tablename == "FA_PAY_ORDER") {
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
        else if (tablename == "IP_TRANSFER_ISSUE" || tablename == "IP_GOODS_ISSUE" || tablename == "IP_GATE_PASS_ENTRY" || tablename == "IP_TRANSFER_ISSUE" || tablename == "IP_PRODUCTION_ISSUE") {
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





    function generateColumn(tableName) {
        var colName;
        if (tableName == "IP_PURCHASE_INVOICE") {
            colName = [
                {
                    field: "ITEM_EDESC",
                    title: "Product"
                },
                {
                    field: "MU_CODE",
                    title: "Unit"
                },
                {
                    field: "QUANTITY",
                    title: "Quantity"
                }
                ,
                {
                    field: "UNIT_PRICE",
                    title: "Rate"
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Amount"
                }

                ,
                {
                    field: "CALC_QUANTITY",
                    title: "Cal Qty"
                }
                ,
                {
                    field: "CALC_UNIT_PRICE",
                    title: "Cal Rate"
                }
                ,

                {
                    field: "CALC_TOTAL_PRICE",
                    title: "Cal Amount"
                }

            ];
        }
        return colName;
    }



    //$scope.init = function () {
    //    $scope.toptions = new Array();
    //    for (var i = 0; i < 10; i++) {
    //        $scope.toptions.push({
    //            name: " display name" + i,
    //            value: "value" + i,
    //            index: i
    //        });
    //    }
    //    $scope.tselected = [$scope.toptions[4], $scope.toptions[5]];
    //    $scope.isSaving = false;
    //}

    //$scope.init();
});
PMModule.service('contravoucherservice', function (contravoucherfactory, $http) {

    this.deleteFinanceVoucher = function (voucherno, formcode) {
        var delRes = $http({
            method: 'POST',
            url: "/api/ContraVoucherApi/DeleteFinanceVoucher?voucherno=" + voucherno + "&formcode=" + formcode,
            dataType: "JSON"
        });
        return delRes;
    };

    this.getFormDetail_ByFormCode = function (formCode, d1) {
        var formDetail = contravoucherfactory.getFormDetail(formCode)
            .then(function (result) {
                d1.resolve(result);
            }, function (err) { });
    };
    this.getDraftFormDetail_ByFormCode = function (formCode, d7) {
        var formDetail = contravoucherfactory.getDraftFormDetail(formCode)
            .then(function (result) {
                d7.resolve(result);
            }, function (err) { });
    };

    //draft
    this.getDraftData_ByFormCodeAndTempCode = function (formCode, tempcode) {
        return contravoucherfactory.getDraftDataByFormCodeAndTempCode(formCode, tempcode);

    };
    this.getFormSetup_ByFormCode = function (formCode, d2) {
        var formSetup = contravoucherfactory.getFormSetup(formCode)
            .then(function (result) {
                d2.resolve(result);
            }, function (err) { });
    };
    this.getnewlygeneratedvoucherno = function (companycode, fromcode, currentdate, tablename, d6) {

        var newvoucherno = contravoucherfactory.getNewOrederNumber(companycode, fromcode, currentdate, tablename)
            .then(function (result) {
                d6.resolve(result);
            }, function (err) { });
    };
    this.getSalesOrderDetail_ByFormCodeAndOrderNo = function (formCode, orderno, d4) {

        var salesorderformDetail = contravoucherfactory.getSalesOrderFormDetail(formCode, orderno)
            .then(function (result) {
                d4.resolve(result);
            }, function (err) { });
    };

    this.getnewlygeneratedvouchernoafterinsert = function () {

        return Ccontravoucherfactory.getNewOrederNumber(companycode, fromcode, currentdate, tablename);
    }
    this.getBudgetCodeByAccCode = function (accCode) {
        var budgetCode = contravoucherfactory.getBudgetCodeByAccCode(accCode);
    }
    this.GetVouchersCount = function (FORM_CODE, TABLE_NAME) {
        var budgetCode = contravoucherfactory.GetVoucherCount(FORM_CODE, TABLE_NAME);
        return budgetCode;
    }

    this.GetReferenceList = function () {
        return contravoucherfactory.getReferenceList();
    }

    this.getSubledgerByAccCode = function (accCode, d5) {

        var newvoucherno = contravoucherfactory.getSubledgerByAccCode(accCode, d5)
            .then(function (result) {
                d5.resolve(result);
            }, function (err) { });
    };
    this.getFormCustomSetup_ByFormCode = function (formCode, d3) {

        var formDetail = contravoucherfactory.GetFormCustomSetup(formCode)
            .then(function (result) {
                d3.resolve(result);
            }, function (err) { });
    };

});

PMModule.factory('contravoucherfactory', function ($http) {
    var fac = {};
    fac.getFormDetail = function (formcode) {
        var req = "/api/TemplateApi/GetFormDetailSetup?formCode=";
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
    fac.getNewOrederNumber = function (companycode, formcode, currentdate, tablename) {
        var req = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetNewOrderNo?companycode=" + companycode + "&formcode=" + formcode + "&currentdate=" + currentdate + "&tablename=" + tablename;
        //return $http.get(req + formcode);
        return $http.get(req);
    }
    fac.getSalesOrderFormDetail = function (formcode, orderno) {

        var req = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetSalesOrderDetailFormDetailByFormCodeAndOrderNo?formCode=" + formcode + "&orderno=" + orderno;
        //return $http.get(req + formcode);
        return $http.get(req);
    }
    fac.getDraftDataByFormCodeAndTempCode = function (formcode, tempCode) {
        var req = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getDraftDataByFormCodeAndTempCode?formCode=" + formcode + "&TempCode=" + tempCode;
        //return $http.get(req + formcode);
        return $http.get(req);
    }

    fac.getBudgetCodeByAccCode = function (accCode) {
        var req = "/api/TemplateApi/getBudgetCodeByAccCode?accCode=";
        return $http.get(req + accCode);
    }

    fac.getReferenceList = function () {
        var req = "/api/ContraVoucherApi/GetReferenceList";
        return $http.get(req);

    };
    fac.GetVoucherCount = function (FORM_CODE, TABLE_NAME) {
        var req = "/api/ContraVoucherApi/GetVouchersCount?FORM_CODE=" + FORM_CODE + "&TABLE_NAME=" + TABLE_NAME;
        return $http.get(req);
    }


    fac.GetFormCustomSetup = function (formcode) {
        var req = "/api/TemplateApi/GetFormCustomSetup?formCode=";
        return $http.get(req + formcode);
    }


    return fac;
});

