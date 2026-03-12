
/* eslint-disable */
function focusNextFunction(e, pageElements) {
    debugger

    var pageElems = pageElements,
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

function specialTab() {

    $('.cacccode, .acccode, .issuetype, .location, .currency, .cust, .supplier, .prority, .products, .budget').keydown(function (e) {
        if (e.keyCode == 9) {  //tab pressed
            e.preventDefault();
            var pageElements = document.querySelectorAll('input, select, textarea, button');
            focusNextFunction(e, pageElements);
        }
        if (e.keyCode == 9 && e.shiftKey) {  //tab pressed
            e.preventDefault();
            var pageElements = [].slice.call(document.querySelectorAll('input, select, textarea, button'), 0).reverse();
            focusNextFunction(e, pageElements);
        }
    });
}

DTModule.directive('nextOnTab',
    function () {
        return {
            restrict: 'A',
            link: function ($scope, selem, attrs) {
                var count = 0;
                selem.bind('keydown',
                    function (e) {
                        if (count < 1) {
                            specialTab();
                            count++;
                        }
                        var code = e.keyCode || e.which;
                        if (code === 9) {
                            e.preventDefault();
                            var pageElements = document.querySelectorAll('input, select, textarea, button');
                            focusNextFunction(e, pageElements);
                        }
                        if (e.shiftKey && code == 9) {
                            var pageElements = [].slice.call(document.querySelectorAll('input, select, textarea, button'), 0).reverse();
                            focusNextFunction(e, pageElements);
                        }

                    });
            }
        }
    });

DTModule.directive('financialform', function () {

    return {
        templateUrl: '/DocumentTemplate/Template/ModuleFormTab'
    };
});


DTModule.controller('FormTemplateCtrl', function ($scope, $rootScope, $http, $routeParams, formtemplateservice, $window, $filter, hotkeys, $timeout) {

    $scope.isNextScreenRender = false;
    //$scope.controllerName = "FormTemplateCtrl";
    const CALC_QUANTITY = "CALC_QUANTITY";
    const CALC_TOTAL_PRICE = "CALC_TOTAL_PRICE";
    const CALC_UNIT_PRICE = "CALC_UNIT_PRICE";
    const ITEM_CODE = "ITEM_CODE";
    const ATTRIBUTE_CODE = "ATTRIBUTE_CODE";
    const SERIAL_NO = "SERIAL_NO";
    const REFERENCE_SERIAL_NO = "REFERENCE_SERIAL_NO";
    const REFERENCE_VOUCHER_NO = "REFERENCE_VOUCHER_NO";
    const BLANKET_NO = "BLANKET_NO";
    const MU_CODE = "MU_CODE";
    const QUANTITY = "QUANTITY";
    const SECOND_QUANTITY = "SECOND_QUANTITY";
    const STOCK_BLOCK_FLAG = "STOCK_BLOCK_FLAG";
    const PRICE = "PRICE";
    const TOTAL_PRICE = "TOTAL_PRICE";
    const NET_AMOUNT = "NET_AMOUNT";
    const TOTAL_ROW_PRICE = "TOTAL_ROW_PRICE";
    const UNIT_PRICE = "UNIT_PRICE";
    const VAT_AMOUNT = "VAT_AMOUNT";
    const LUXURY_TAX_AMOUNT = "LUXURY_TAX_AMOUNT";
    const DISCOUNT_AMOUNT = "DISCOUNT_AMOUNT";
    const DISCOUNT_CLASS = "DISCOUNT_CLASS";
    const PURITY = "PURITY";
    const NON_VAT_FLAG = "NON_VAT_FLAG";
    const TOTAL_WEIGHT = "TOTAL_WEIGHT";
    const AMOUNT = "AMOUNT";
    const MAKING = "MAKING";
    const STONE_AMOUNT = "STONE_AMOUNT";
    const DIAMOND_AMOUNT = "DIAMOND_AMOUNT";
    const SHIPPING_ADDRESS = "SHIPPING_ADDRESS";
    const SHIPPING_CONTACT_NO = "SHIPPING_CONTACT_NO";
    const EMPLOYEE_CODE = "EMPLOYEE_CODE";
    const EMPLOYEE_NAME = "EMPLOYEE_NAME";
    const GROSS_WEIGHT = "GROSS_WEIGHT";
    const LESS_STONE = "LESS_STONE";
    const NET_WEIGHT = "NET_WEIGHT";
    const WASTAGE = "WASTAGE";
    const DESCRIPTION = "DESCRIPTION";
    $scope.save = "Save";
    $scope.saveupdatebtncustomer = "Save";
    $scope.savecontinue = "Save & Continue";
    $scope.tempCode = "";
    $scope.draftsave = "Save";
    $scope.Opera = false;
    $scope.shymphony = false;
    $scope.PrintDiscount = "";
    $scope.PrintDiscountShow = false;
    $scope.decimal_place = 2;
    $scope.freeze_manual_entry_flag = false;
    $scope.Dealer_system_flag = "N";
    $scope.discount_schedule_flag = "";
    $scope.salesInvoiceJewelleryDateTime = "";
    $scope.price_control_flag = "";
    $scope.rate_diff_flag = "";

    $scope.accsummary = {
        drTotal: null,
        crTotal: null,
        diffAmount: null,
    }

    $scope.FormCode = $routeParams.formCode;
    document.formCode = $scope.FormCode;
    var checkedItems = [];
    var checkedIds = {};
    $scope.checkedItemstore = [];
    $scope.checkedresult = [];


    $scope.havRefrence = 'N';
    $scope.ref_form_code = "";
    $scope.freeze_master_ref_flag = 'N';
    $scope.ref_fix_qty = 'N';
    $scope.ref_fix_price = 'N';
    $scope.NEGETIVE_STOCK_FLAG = 'N';
    if ($routeParams.orderno != undefined) {
        $scope.OrderNo = $routeParams.orderno.split(new RegExp('_', 'i')).join('/');
    }
    else { $scope.OrderNo = "undefined"; }
    if ($routeParams.tempCode != undefined) {
        $scope.tempCode = $routeParams.tempCode;
    }
    if ($routeParams.printstatus != undefined) {
        $scope.printStatus = $routeParams.printstatus;
    }
    else { $scope.printstatus = "undefined"; }
    var d1 = $.Deferred();
    var d2 = $.Deferred();
    var d3 = $.Deferred();
    var d4 = $.Deferred();
    var d5 = $.Deferred();
    var d6 = $.Deferred();
    var d10 = $.Deferred();
    var d11 = $.Deferred();
    var d95 = $.Deferred();
    var luxuryTaxAddition = true;
    $scope.isPrintAble = false;
    $scope.SN_No = 0;
    $scope.dzvouchernumber = "";
    $scope.dzvoucherdate = "";
    $scope.dzformcode = "";
    $scope.productDescription = '';
    $scope.producttemp = '';
    $scope.NepaliDate = '';
    $scope.HeaderNepaliDate = '';
    $scope.FormName = '';
    $scope.VoucherCount = '';
    $scope.DocumentName = ""; // document display name at top
    $scope.youFromReference = false;
    $scope.refernceForBatch = false;
    $scope.refernceForSerial = false;
    $scope.RefTableName = "";

    $scope.MasterFormElement = []; // for elements having master_child_flag='M'
    $scope.ChildFormElement = [{ element: [] }]; // initial child element
    $scope.aditionalChildFormElement = []; // a blank child element model while add button press.
    $scope.formDetail = ""; // all form_dtl elements retrived from service (contains master_child 'M' && 'C' ).
    $scope.formSetup = "";
    $scope.formCustomSetup = "";
    $scope.CustomFormElement = [];
    $scope.customersMasterChildArr = [];
    $scope.customersArrSIJ = [];
    $scope.ModuleCode = "";
    $scope.MasterFormElementValue = [];
    $scope.CheckReferenceQuantity = [];
    $scope.SalesOrderformDetail = "";
    $scope.datePattern = /^[01][0-9]-(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)-\d{4}$/
    $scope.companycode = "";
    $scope.ChargeList = [];
    $scope.lineWiseChargeList = [];
    $scope.newChargeListForJewellery = [];
    ///subin declare for storing initial charge data
    $scope.Initialchargedata = [];
    $scope.todaydate = "";
    $scope.after_delemeter_value = "NO";
    $scope.masterModels = {}; // for master model
    $scope.childModels = []; // for dynamic
    $scope.companyAndBranchDataModel = {};
    $scope.customModels = {};
    $scope.units = [];
    $scope.ItemInfo = [];
    $scope.masterModelTemplate = {};
    $scope.childModelTemplate = {};
    $scope.iamFromReference = false;
    $scope.showRefTab = false;
    $scope.masterChildData = null;
    $rootScope.refCheckedItem = [];
    $scope.selectedProductOptions = [];
    $scope.ProductsChanged = [];
    $rootScope.quickmastercustomercode = "";
    $rootScope.quickmasteritemcode = "";
    $rootScope.quickmastersuppliercode = "";
    $scope.itemmodelopened = false;
    $scope.customermodelopened = false;
    $scope.suppliermodelopened = false;
    $scope.InvChargeList = [];
    $scope.buttonflag = "";
    $scope.pricelistid = "";
    $scope.printcount = 1;
    $scope.printcountdiv = "Original";
    $scope.sim_flag = "";
    $scope.TPIN_VAT_NO_customer = "";
    $scope.checkedBatchTranResult = [];
    $scope.voucherNoFrmRefernce = [];
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
    $scope.dynamicBatchTrackingModalData = [{
        ITEM_CODE: "",
        ITEM_EDESC: "",
        MU_CODE: "",
        LOCATION_CODE: "",
        QUANTITY: ""
    }];
    //$scope.dynamicSerialTrackingModalData = [];
    //$scope.checkedresult = [];
    $scope.dynamicInvItenChargeModalData = [{
        ITEM_CODE: 0,
        QUANTITY: "",
        UNIT_PRICE: "",
        TOTAL_PRICE: "",
        CALC_QUANTITY: "",
        CALC_UNIT_PRICE: "",
        CALC_TOTAL_PRICE: "",
        INV_ITEM_CHARGE_AMOUNT_WISE: [{
            CHARGE_CODE: "",
            CHARGE_EDESC: "",
            CHARGE_TYPE: "",
            IMPACT_ON: "",
            APPLY_QUANTITY: "",
            VALUE_PERCENT_AMOUNT: "",
            CHARGE_AMOUNT: "",
            VALUE_PERCENT_FLAG: "",
            ACC_CODE: "",
            SUB_CODE: "",
            BUDGET_CODE: "",
            GL: "",
            APPORTION_FLAG: "",
            CALC: "",
            APPLY_NO: "",
        }],
        INV_ITEM_CHARGE_QUANTITY_WISE: [{
            CHARGE_CODE: "",
            CHARGE_EDESC: "",
            CHARGE_TYPE: "",
            IMPACT_ON: "",
            APPLY_QUANTITY: "",
            VALUE_PERCENT_AMOUNT: "",
            CHARGE_AMOUNT: "",
            VALUE_PERCENT_FLAG: "",
            ACC_CODE: "",
            SUB_CODE: "",
            BUDGET_CODE: "",
            GL: "",
            APPORTION_FLAG: "",
            CALC: "",
            APPLY_NO: "",
        }]

    }];

    $scope.chunkedProducts = [];
    let chunkSize = 10;

    $scope.cashVoucherData = {
        SALES_NO: '',
        CUSTOMER_NAME: '',
        CUSTOMER_NAME_DISPLAY: '',
        SALES_AMOUNT: 0,
        TENDER_AMOUNT: 0,
        BALANCE_AMOUNT: 0,
        FORM_CODE: '',
        RECEIPT_LIST: []
    };

    // Store receipt message from SaveFormData response for use in confirmReceiptEntry
    $scope.receiptMessage = null;

    $scope.dynamicInvItenChargeModalData_OK = [{
        ITEM_CODE: 0,
        QUANTITY: "",
        UNIT_PRICE: "",
        TOTAL_PRICE: "",
        CALC_QUANTITY: "",
        CALC_UNIT_PRICE: "",
        CALC_TOTAL_PRICE: "",
        INV_ITEM_CHARGE_AMOUNT_WISE: [{
            CHARGE_CODE: "",
            CHARGE_TYPE: "",
            CHARGE_EDESC: "",
            IMPACT_ON: "",
            APPLY_QUANTITY: "",
            VALUE_PERCENT_AMOUNT: "",
            CHARGE_AMOUNT: "",
            VALUE_PERCENT_FLAG: "",
            ACC_CODE: "",
            SUB_CODE: "",
            BUDGET_CODE: "",
            GL: "",
            APPORTION_FLAG: "",
            CALC: "",
            APPLY_NO: "",
        }],
        INV_ITEM_CHARGE_QUANTITY_WISE: [{
            CHARGE_CODE: "",
            CHARGE_TYPE: "",
            CHARGE_EDESC: "",
            IMPACT_ON: "",
            APPLY_QUANTITY: "",
            VALUE_PERCENT_AMOUNT: "",
            CHARGE_AMOUNT: "",
            VALUE_PERCENT_FLAG: "",
            ACC_CODE: "",
            SUB_CODE: "",
            BUDGET_CODE: "",
            GL: "",
            APPORTION_FLAG: "",
            CALC: "",
            APPLY_NO: "",
        }]

    }];

    $scope.INDIV_ITEM_CHARGE_MODAL = [];
    $scope.INDIV_ITEM_CHARGE_MODAL_AMOUNT = [];
    $scope.INDIV_ITEM_CHARGE_MODAL_QUANTITY = [];
    $scope.formControlsInfo = [];
    $scope.formBackDays = "";

    $scope.saveAsDraft = {
        TEMPLATE_NO: "",
        TEMPLATE_EDESC: "",
        TEMPLATE_NDESC: ""
    }

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
        START_FORM: "Tilottama-16",
        DESTINATION: "",
        CN_NO: "",
        TRANSPORT_INVOICE_NO: "",
        TRANSPORT_INVOICE_DATE: "",
        DELIVERY_INVOICE_DATE: "",
        WB_WEIGHT: "",
        WB_NO: "",
        WB_DATE: "",
        GATE_ENTRY_NO: "",
        GATE_ENTRY_DATE: "",
        LOADING_SLIP_NO: "",
        TRANSPORTER_EDESC: "",
        VEHICLE_EDESC: "",

    }
    $scope.BATCH_MODAL = [];
    $scope.BATCH_CHILD_MODAL = [];
    //rootscope variables for storing deault values (master)
    $rootScope.CUSTOMER_CODE_DEFVAL_MASTER = "";
    $rootScope.ITEM_CODE_DEFAULTVAL = "";

    $scope.fromCustomer = false;
    //summary Object
    $scope.summary = { 'grandTotal': 0 }
    $scope.summary = { 'grandTotalAmount': 0 }
    $scope.newgenorderno = "";
    $scope.newFormLocationCode = "";
    $scope.newFormLocationName = "";
    $scope.childelementwidth = "";
    $scope.shortCut = "N";
    $scope.batchTranIcon = [];
    $scope.linkcustomer = [];
    $scope.linkitem = [];
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
    //    combo: 'ctrl+shift+enter',
    //    allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
    //    callback: function (event, hotkey) {
    //    }
    //});

    hotkeys.add({
        combo: 'ctrl+enter',
        allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            var param = 0;
            $scope.SaveDocumentFormData(param);
        }
    });

    hotkeys.add({
        combo: 'shift+enter',
        allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            $($($($(".document_child_table")[0]).find('tr')[$($(".document_child_table")[0]).find('tr').length - 1]).find('input').not(':button,:hidden,:disabled')[0]).focus();
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
        callback: function (event, hotkey) {
            $('#myTab a[href="#tab_15_1"]').trigger('click');
        }
    });

    // navigate to reset tab
    //hotkeys.add({
    //    combo: 'shift+alt+r',
    //    callback: function (event, hotkey) {
    //        //$scope.ResetDocument();
    //        $scope.refresh();
    //    }
    //});

    hotkeys.add({
        combo: 'alt+r',
        callback: function (event, hotkey) {
            $scope.ResetDocument();
        }
    })

    hotkeys.add({
        //combo: 'ctrl+a',
        //callback: function (event, hotkey) {
        //    event.preventDefault();
        //    $scope.AddNewChildCustomer();
        //}

        combo: 'ctrl+a',
        allowIn: ['INPUT'], // optional: allows it in input fields
        callback: function (event, hotkey) {
            debugger;
            const active = document.activeElement;
            if (active && document.activeElement.name === 'customer_input') {
                event.preventDefault();
                $scope.AddNewChildCustomer();
            }
        }

    })

    hotkeys.add({
        combo: 'ctrl+alt+t',
        callback: function (event, hotkey) {
            // $scope.toogleToolTips();
            $('i.font-green').toggle();
        }
    })

    // navigate to save and continue tab
    hotkeys.add({
        combo: 'alt+c',
        callback: function (event, hotkey) {
            $scope.SaveDocumentFormData(1);
        }
    });

    // navigate to save and print tab
    hotkeys.add({
        combo: 'alt+p',
        callback: function (event, hotkey) {
            $scope.SaveDocumentFormData(3);
        }
    });
    // update and print
    hotkeys.add({
        combo: 'alt+shift+p',
        callback: function (event, hotkey) {
            $scope.SaveDocumentFormData(4);
        }
    });
    // navigate to draftlist tab
    hotkeys.add({
        combo: 'alt+d',
        callback: function (event, hotkey) {
            $scope.showDraftModal();
        }
    });

    // navigate to save as draftlist tab
    hotkeys.add({
        combo: 'alt+s',
        callback: function (event, hotkey) {
            $scope.SaveAsDraft(1);
        }
    });

    // navigate to Refrence tab
    hotkeys.add({
        combo: 'shift+r',
        callback: function (event, hotkey) {
            $('#myTab a[href="#tab_15_2"]').trigger('click');
        }
    });

    // navigate to Custom tab
    hotkeys.add({
        combo: 'shift+c',
        callback: function (event, hotkey) {
            $('#myTab a[href="#tab_15_3"]').trigger('click');
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
    // navigate to Document tab
    hotkeys.add({
        combo: 'shift+d',
        //allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function (event, hotkey) {
            $('#myTab a[href="#tab_15_4"]').trigger('click');
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

    $scope.parseFloat = function (s) {
        return parseFloat(s, 10).toFixed(2);
    };

    $scope.printCompanyInfo = {
        companyName: '',
        address: '',
        formName: '',
        phoneNo: '',
        email: '',
        tPinVatNo: '',
        ComPanNo: '',
    }


    $scope.toogleToolTips = function () {
        var ie = document.getElementsByClassName('font-green');
        ie.style.display = "none";
        return;
    }

    $scope.changeFreghtAmount = function () {

        var totalFreghtAmount = $scope.SDModel.FREIGHT_RATE * $scope.totalQty;
        console.log(totalFreghtAmount);
        $scope.SDModel.FREGHT_AMOUNT = totalFreghtAmount.toFixed(2);

        //SDModel.FREGHT_AMOUNT

    }
    $scope.bindlinkcustomer = function (data) {
        debugger;

        //$('#itemCustomerModel').show();
        setTimeout(function () {
            //$scope.linkcustomer = data;
            debugger;
            $scope.linkcustomer = data.split('/s')[0].split('/n');
            $scope.linkitem = data.split('/s')[1].split('/n');
            $('#itemCustomerModel').toggle();
            $("#btnCustItemClose").trigger("click");
        }, 30);
        debugger;
    }

    //var frominfourl = "/api/SetupApi/GetFormControlByFormCode?formcode=" + $scope.FormCode;
    //$http.get(frominfourl).then(function (response) {
    //    $scope.formControlsInfo = response.data;

    //});
    //var frombackdaysurl = "/api/SetupApi/Backdays?formcode=" + $scope.FormCode;
    //$http.get(frombackdaysurl).then(function (response) {

    //    $scope.formBackDays = response.data;

    //});

    ///subin  set initial charge data
    //var initchargeUrl = "/api/TemplateApi/GetChargeData?formCode=" + $scope.FormCode;
    //$http.get(initchargeUrl).then(function (initresponse) {
    //    $scope.Initialchargedata = initresponse.data;
    //});

    $scope.checkRefrences = function () {
        var req = "/api/TemplateApi/GetRefrenceFlag?formCode=" + $scope.FormCode;
        $http.get(req).then(function (results) {

            var response = results.data.FormSetupRefrence;
            $scope.formControlsInfo = results.data.FormControlModels;
            $scope.havRefrence = response[0].REFERENCE_FLAG;
            $scope.RefTableName = response[0].REF_TABLE_NAME;
            document.RefTN = response[0].REF_TABLE_NAME;
            $scope.ref_form_code = response[0].REF_FORM_CODE;
            $scope.freeze_master_ref_flag = response[0].FREEZE_MASTER_REF_FLAG;
            $scope.ref_fix_qty = response[0].REF_FIX_QUANTITY;
            $scope.ref_fix_price = response[0].REF_FIX_PRICE;
            $scope.formBackDays = response[0].FREEZE_BACK_DAYS;
            $scope.decimal_place = response[0].DECIMAL_PLACE;
            $scope.freeze_manual_entry_flag = response[0].FREEZE_MANUAL_ENTRY_FLAG;
            $scope.discount_schedule_flag = response[0].DISCOUNT_SCHEDULE_FLAG;
            $scope.price_control_flag = response[0].PRICE_CONTROL_FLAG;
            $scope.rate_diff_flag = response[0].RATE_DIFF_FLAG;
            $scope.rate_diff_flag = response[0].RATE_DIFF_FLAG;
            $scope.sim_flag = response[0].SIM_FLAG == null ? 'N' : response[0].SIM_FLAG;
            $scope.serial_tracking_flag = response[0].SERIAL_TRACKING_FLAG;
            $scope.batch_tracking_flag = response[0].BATCH_TRACKING_FLAG;
            $scope.Dealer_system_flag = response[0].Dealer_system_flag;
            //  $scope.NEGATIVE_STOCK_FLAG = response[0].NEGATIVE_STOCK_FLAG//;

        });
    }
    $scope.checkRefrences();


    //get issue type
    //$scope.IssueTypeDataSource = {
    //    type: "json",
    //    serverFiltering: true,
    //    transport: {
    //        read: {
    //            url: "/api/TemplateApi/GetAllIssueTypeListByFilter",

    //        },
    //        parameterMap: function (data, action) {
    //            var newParams;
    //            if (data.filter != undefined) {
    //                if (data.filter.filters[0] != undefined) {
    //                    newParams = {
    //                        filter: data.filter.filters[0].value
    //                    };
    //                    return newParams;
    //                }
    //                else {
    //                    newParams = {
    //                        filter: ""
    //                    };
    //                    return newParams;
    //                }
    //            }
    //            else {
    //                newParams = {
    //                    filter: ""
    //                };
    //                return newParams;
    //            }
    //        }
    //    }
    //};

    //$scope.IssueTypeCodeOption = {
    //    dataSource: $scope.IssueTypeDataSource,
    //    dataTextField: 'ISSUE_TYPE_EDESC',
    //    dataValueField: 'ISSUE_TYPE_CODE'
    //};

    //get supplier
    //$scope.supplierDataSource = {
    //    type: "json",
    //    serverFiltering: true,
    //    transport: {
    //        read: {
    //            url: "/api/TemplateApi/GetAllSupplierListByFilter",

    //        },
    //        parameterMap: function (data, action) {
    //            var newParams;
    //            if (data.filter != undefined) {
    //                if (data.filter.filters[0] != undefined) {
    //                    newParams = {
    //                        filter: data.filter.filters[0].value
    //                    };
    //                    return newParams;
    //                }
    //                else {
    //                    newParams = {
    //                        filter: ""
    //                    };
    //                    return newParams;
    //                }
    //            }
    //            else {
    //                newParams = {
    //                    filter: ""
    //                };
    //                return newParams;
    //            }
    //        }
    //    }
    //};

    //$scope.supplierCodeOption = {
    //    dataSource: $scope.supplierDataSource,
    //    dataTextField: 'SUPPLIER_EDESC',
    //    dataValueField: 'SUPPLIER_CODE'

    //}

    //get currency code
    //$scope.currencyDataSource = {
    //    type: "json",
    //    serverFiltering: true,
    //    transport: {
    //        read: {
    //            url: "/api/TemplateApi/GetCurrencyListByFlter",

    //        },
    //        parameterMap: function (data, action) {
    //            var newParams;
    //            if (data.filter != undefined) {
    //                if (data.filter.filters[0] != undefined) {
    //                    newParams = {
    //                        filter: data.filter.filters[0].value
    //                    };
    //                    return newParams;
    //                }
    //                else {
    //                    newParams = {
    //                        filter: ""
    //                    };
    //                    return newParams;
    //                }
    //            }
    //            else {
    //                newParams = {
    //                    filter: ""
    //                };
    //                return newParams;
    //            }
    //        }
    //    }
    //};

    //$scope.currencyCodeOption = {
    //    dataSource: $scope.currencyDataSource,
    //    dataTextField: 'CURRENCY_EDESC',
    //    dataValueField: 'CURRENCY_CODE',
    //    select: function (e) {


    //    },
    //    dataBound: function (e) {

    //    },
    //    change: function (e) {


    //    }
    //}

    //Account code autocomplete
    //$scope.accountCodeDataSource = {
    //    type: "json",
    //    serverFiltering: true,
    //    transport: {
    //        read: {
    //            url: "/api/TemplateApi/GetAllAccountSetupByFilter",
    //        },
    //        parameterMap: function (data, action) {

    //            var newParams;
    //            if (data.filter != undefined) {
    //                if (data.filter.filters[0] != undefined) {
    //                    newParams = {
    //                        filter: data.filter.filters[0].value
    //                    };
    //                    return newParams;
    //                }
    //                else {
    //                    newParams = {
    //                        filter: ""
    //                    };
    //                    return newParams;
    //                }
    //            }
    //            else {
    //                newParams = {
    //                    filter: ""
    //                };
    //                return newParams;
    //            }
    //        }
    //    },
    //};

    //$scope.accountCodeOption = {
    //    dataSource: $scope.accountCodeDataSource,
    //    dataTextField: 'ACC_EDESC',
    //    dataValueField: 'ACC_CODE',
    //    filter: 'contains',
    //    select: function (e) {
    //    },
    //    dataBound: function (e) {

    //    },
    //    change: function (e) {


    //    }
    //}


    //budget center for indv item charge
    $scope.budgetCenterinvchargeDataSource = {
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
    $scope.budgetCenterinvchargeOption = {
        dataSource: $scope.budgetCenterinvchargeDataSource,
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


    //sub code for inv item charge
    $scope.invchargesubledgerCodeDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllSubLedgerCodeByFilter",
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
    $scope.invchargeSubledgerCodeCodeOption = {
        dataSource: $scope.invchargesubledgerCodeDataSource,
        dataTextField: 'SUB_EDESC',
        dataValueField: 'SUB_CODE',
        filter: 'contains',
        select: function (e) {
        },
        dataBound: function (e) {

        },
        change: function (e) {


        }
    }

    var chargeUrl = "/api/TemplateApi/GetChargeData?formCode=" + $scope.FormCode;
    $http.get(chargeUrl).then(function (response) {
        debugger;
        setTimeout(function () {
            debugger;
            $scope.ChargeList = response.data;

            // Make sure lineWiseChargeList is initialized
            $scope.lineWiseChargeList = $scope.lineWiseChargeList || [];

            // Store a deep copy at index 0 (so future edits don't mess with original)
            $scope.lineWiseChargeList[0] = angular.copy($scope.ChargeList);

            if ($scope.DocumentName == 'SA_SALES_INVOICE_JEWEL' || $scope.DocumentName == 'SA_SALES_RETURN_JEWEL') {
                $scope.newChargeListForJewellery = angular.copy($scope.ChargeList).sort(function (a, b) {
                    return a.PRIORITY_INDEX_NO - b.PRIORITY_INDEX_NO;
                });
            }

            // Auto-tick checkboxes for charges where CHARGE_ACTIVE_FLAG == 'Y'
            // Initialize $scope.data if it doesn't exist
            if (!$scope.data) {
                $scope.data = [];
            }

            // Add charges with CHARGE_ACTIVE_FLAG == 'Y' to $scope.data
            angular.forEach($scope.ChargeList, function (charge) {
                if (charge.CHARGE_ACTIVE_FLAG === 'Y') {
                    // Check if this charge is not already in $scope.data
                    var alreadyExists = false;
                    for (var i = 0; i < $scope.data.length; i++) {
                        if ($scope.data[i].CHARGE_CODE === charge.CHARGE_CODE) {
                            alreadyExists = true;
                            break;
                        }
                    }

                    // Only add if it doesn't already exist
                    if (!alreadyExists) {
                        $scope.data.push(angular.copy(charge));
                    }
                }
            });

            // Trigger calculation for auto-checked charges
            //if ($scope.data.length > 0) {
            //    if ($scope.DocumentName == 'SA_SALES_INVOICE_JEWEL' || $scope.DocumentName == 'SA_SALES_RETURN_JEWEL') {
            //        $scope.calculateChargeAmountSalesInvoiceJewellery($scope.data, true);
            //    } else {
            //        $scope.calculateChargeAmount($scope.data, true);
            //    }
            //}
        }, 0/*300*/);
        //$scope.data = response.data;

    });

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
    }
    $scope.vechOption = {
        dataSource: $scope.vechDataSource,
        dataTextField: 'VEHICLE_EDESC',
        dataValueField: 'VEHICLE_EDESC',
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
    }

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
    }
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
    }
    $scope.cityOption = {
        dataSource: $scope.cityDataSource,
        dataTextField: 'CITY_EDESC',
        dataValueField: 'CITY_EDESC',
        filter: "contains",
        select: function (e) {

            if (e.dataItem !== undefined) {

            }

        },
        dataBound: function (e) {

        },
        change: function (e) {


        }
    }

    $scope.refrenceFn = function (response, callback) {
        debugger;
        $scope.dynamicSerialTrackingModalData = [];
        $scope.checkedresult = [];
        var rows = response.data;
        $scope.CheckReferenceQuantity = response.data;
        //$rootScope.refrencedata = rows;
        var primarycolumnname = PrimaryColumnForTable($scope.RefTableName);
        $scope.voucherNoFrmRefernce = response.data;
        $scope.youFromReference = true;
        $scope.refernceForBatch = true;
        $scope.refernceForSerial = true;
        $scope.checkedBatchTranResult = [];
        if (rows.length > 0) {
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
            $scope.referenceDataDisplay1 = [];
            for (var i = 0; i < rows.length; i++) {
                var tempCopy111 = angular.copy($scope.referenceDataDisplay);
                $scope.referenceDataDisplay1.push(tempCopy111);

                if ($scope.DocumentName == 'SA_SALES_ORDER') {
                    $scope.childModelTemplate.REFERENCE_SERIAL_NO = '';
                    $scope.childModelTemplate.REFERENCE_VOUCHER_NO = '';
                }

                $scope.childModelTemplate.NON_VAT_FLAG = '';

                var tempCopy = angular.copy($scope.childModelTemplate);
                $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
                $scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));
                //subin changes for default value while setting refrence data
                $.each($scope.ChildFormElement[0].element, function (childkey, childelementvalue) {
                    if (childelementvalue.DEFA_VALUE != null) {
                        if ($scope.childModels[i][childelementvalue.COLUMN_NAME] == null) {
                            $scope.childModels[i][childelementvalue.COLUMN_NAME] = childelementvalue.DEFA_VALUE;
                        }
                    }
                });
                //setTimeout(function () {
                //    $("#products_" + i).data('kendoComboBox').dataSource.data([{ ItemCode: rows[i].ITEM_CODE, ItemDescription: rows[i].ITEM_EDESC, Type: "code" }]);
                //    $("#customers").data('kendoComboBox').dataSource.data([{ CustomerCode: rows[0].CUSTOMER_CODE, CustomerName: rows[0].CUSTOMER_EDESC, Type: "code" }]);
                //}, 1000);
                //var mastertempCopy = angular.copy($scope.masterModelTemplate);
                //var mastercopy = $scope.getObjWithKeysFromOtherObj(mastertempCopy, rows[i]);
                ////$scope.masterModels[mastercopy];
                //$scope.masterModels = angular.copy(mastercopy);

                if ($scope.masterModels.hasOwnProperty("CUSTOMER_CODE")) {
                    if ($scope.masterModels["CUSTOMER_CODE"] != undefined) {
                        $scope.masterModels["CUSTOMER_CODE"] = rows[i].CUSTOMER_CODE;
                    }
                }
                if ($scope.masterModels.hasOwnProperty("MANUAL_NO")) {
                    if ($scope.masterModels["MANUAL_NO"] != undefined) {
                        $scope.masterModels["MANUAL_NO"] = rows[i].MANUAL_NO;
                    }
                }
                if ($scope.masterModels.hasOwnProperty("DESCRIPTION")) {
                    if ($scope.masterModels["DESCRIPTION"] != undefined) {
                        $scope.masterModels["DESCRIPTION"] = rows[i].DESCRIPTION;
                    }
                }
                if ($scope.masterModels.hasOwnProperty("PARTY_TYPE_CODE")) {
                    if ($scope.masterModels["PARTY_TYPE_CODE"] != undefined) {
                        $scope.masterModels["PARTY_TYPE_CODE"] = rows[i].PARTY_TYPE_CODE;
                    }
                }
                if ($scope.masterModels.hasOwnProperty("EMPLOYEE_CODE")) {
                    if ($scope.masterModels["EMPLOYEE_CODE"] != undefined) {
                        $scope.masterModels["EMPLOYEE_CODE"] = rows[i].EMPLOYEE_CODE;
                    }
                }
                if ($scope.masterModels.hasOwnProperty("AGENT_CODE")) {
                    if ($scope.masterModels["AGENT_CODE"] != undefined) {
                        $scope.masterModels["AGENT_CODE"] = rows[i].AGENT_CODE;
                    }
                }
                if ($scope.masterModels.hasOwnProperty("SHIPPING_ADDRESS")) {
                    if ($scope.masterModels["SHIPPING_ADDRESS"] != undefined) {
                        $scope.masterModels["SHIPPING_ADDRESS"] = rows[i].SHIPPING_ADDRESS;
                    }
                }
                if ($scope.masterModels.hasOwnProperty("SHIPPING_CONTACT_NO")) {
                    if ($scope.masterModels["SHIPPING_CONTACT_NO"] != undefined) {
                        $scope.masterModels["SHIPPING_CONTACT_NO"] = rows[i].SHIPPING_CONTACT_NO;
                    }
                }
                if ($scope.masterModels.hasOwnProperty("SALES_TYPE_CODE")) {
                    if ($scope.masterModels["SALES_TYPE_CODE"] != undefined) {
                        $scope.masterModels["SALES_TYPE_CODE"] = rows[i].SALES_TYPE_CODE;
                    }
                }
                if ($scope.masterModels.hasOwnProperty("PAYMENT_MODE")) {
                    if ($scope.masterModels["PAYMENT_MODE"] != undefined) {
                        $scope.masterModels["PAYMENT_MODE"] = rows[i].PAYMENT_MODE;
                    }
                }
                if ($scope.masterModels.hasOwnProperty("PRIORITY_CODE")) {
                    if ($scope.masterModels["PRIORITY_CODE"] != undefined) {
                        $scope.masterModels["PRIORITY_CODE"] = rows[i].PRIORITY_CODE;
                    }
                }
                if ($scope.masterModels.hasOwnProperty("AREA_CODE")) {
                    if ($scope.masterModels["AREA_CODE"] != undefined) {
                        $scope.masterModels["AREA_CODE"] = rows[i].AREA_CODE;
                    }
                }

            }


            angular.forEach(rows, function (refrencerow, refrenkey) {
                debugger;
                if ($scope.DocumentName === "SA_SALES_CHALAN") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.ORDER_NO;
                }
                if ($scope.DocumentName === "SA_SALES_INVOICE") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.CHALAN_NO;
                }
                if ($scope.DocumentName === "SA_SALES_RETURN_JEWEL") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.SALES_NO;
                }
                if ($scope.DocumentName === "SA_SALES_RETURN") {
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NO = refrencerow.SALES_NO;
                }
                if ($scope.DocumentName != "SA_SALES_RETURN_JEWEL") {
                    $scope.referenceDataDisplay1[refrenkey].ITEM_EDESC = refrencerow.ITEM_EDESC;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_QUANTITY = refrencerow.QUANTITY;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_MU_CODE = refrencerow.MU_CODE;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_UNIT_PRICE = refrencerow.UNIT_PRICE;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_TOTAL_PRICE = refrencerow.TOTAL_PRICE;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_CALC_UNIT_PRICE = refrencerow.CALC_UNIT_PRICE;
                    $scope.referenceDataDisplay1[refrenkey].CALC_TOTAL_PRICE = refrencerow.TOTAL_PRICE;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_REMARKS = refrencerow.REMARKS;
                }
                else {
                    $scope.referenceDataDisplay1[refrenkey].ITEM_EDESC = refrencerow.ITEM_EDESC;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_QUANTITY = refrencerow.QUANTITY;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_MU_CODE = refrencerow.MU_CODE;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_UNIT_PRICE = refrencerow.UNIT_PRICE;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_TOTAL_PRICE = refrencerow.TOTAL_PRICE;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NET_AMOUNT = refrencerow.NET_AMOUNT;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_GROSS_WEIGHT = refrencerow.GROSS_WEIGHT;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_LESS_STONE = refrencerow.LESS_STONE;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_NET_WEIGHT = refrencerow.NET_WEIGHT;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_WASTAGE = refrencerow.WASTAGE;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_TOTAL_WEIGHT = refrencerow.TOTAL_WEIGHT;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_MAKING = refrencerow.MAKING;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_STONE_WEIGHT = refrencerow.STONE_WEIGHT;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_STONE_AMOUNT = refrencerow.STONE_AMOUNT;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_DIAMOND_CARAT = refrencerow.DIAMOND_CARAT;
                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_DIAMOND_AMOUNT = refrencerow.DIAMOND_AMOUNT;

                    $scope.referenceDataDisplay1[refrenkey].REFERENCE_PARTICULAR = refrencerow.REMARKS;
                }
            });

            if ($scope.DocumentName != "SA_SALES_RETURN_JEWEL") {
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
            }
            else {
                $scope.refBGridOptionsJewel = {
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
                        field: "REFERENCE_NET_AMOUNT",
                        title: "Total Amount",
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
                        field: "REFERENCE_PARTICULAR",
                        title: "Particular",
                        width: "120px"
                    },
                    {
                        field: "REFERENCE_GROSS_WEIGHT",
                        title: "Gross Wt.",
                        width: "120px"
                    }, {
                        field: "REFERENCE_LESS_STONE",
                        title: "Less Stone",
                        width: "120px"
                    }, {
                        field: "REFERENCE_NET_WEIGHT",
                        title: "Net Wt.",
                        width: "120px"
                    }, {
                        field: "REFERENCE_WASTAGE",
                        title: "Wastage",
                        width: "120px"
                    }, {
                        field: "REFERENCE_TOTAL_WEIGHT",
                        title: "Total Wt.",
                        width: "120px"
                    }, {
                        field: "REFERENCE_MAKING",
                        title: "Making",
                        width: "120px"
                    }, {
                        field: "REFERENCE_STONE_WEIGHT",
                        title: "Stone Wt.",
                        width: "120px"
                    }, {
                        field: "REFERENCE_STONE_AMOUNT",
                        title: "Stone Amount",
                        width: "120px"
                    }, {
                        field: "REFERENCE_DIAMOND_CARAT",
                        title: "Diamond Ct.",
                        width: "120px"
                    }, {
                        field: "REFERENCE_DIAMOND_AMOUNT",
                        title: "Diamond Amount",
                        width: "120px"
                    }
                    ]
                };
            }

            $scope.muwiseQty();
            var sum = 0;

            //async function processChildModels() {
            //    for (let index = 0; index < $scope.childModels.length; index++) {
            //        const itemCode = $scope.childModels[index].ITEM_CODE;
            //        const url = "/api/TemplateApi/GetItemNonVatFlagByItemCode?itemCode=" + itemCode;

            //        try {
            //            const response = await new Promise((resolve, reject) => {
            //                $http.get(url).then(resolve).catch(reject);
            //            });
            //            $scope.childModels[index]["NON_VAT_FLAG"] = response.data;
            //        } catch (err) {
            //            console.error("Error at index", index, err);
            //        }

            //        // Wait 200ms before next call
            //        await new Promise(resolve => setTimeout(resolve, 200));
            //    }
            //}

            //// Usage
            //(async function () {
            //    await processChildModels();
            //    console.log("✅ All done, running outside code");
            //})();

            //async function processChildModels() {
            //    for (let index = 0; index < $scope.childModels.length; index++) {
            //        const itemCode = $scope.childModels[index].ITEM_CODE;
            //        const url = "/api/TemplateApi/GetItemNonVatFlagByItemCode?itemCode=" + itemCode;

            //        try {
            //            const response = await $http.get(url);
            //            $scope.childModels[index]["NON_VAT_FLAG"] = response.data;
            //        } catch (err) {
            //            console.error("Error at index", index, err);
            //        }

            //        // Wait 200ms before next call
            //        await new Promise(resolve => setTimeout(resolve, 200));
            //    }
            //}

            //// Usage
            //(async function () {
            //    await processChildModels();
            //    // Your outside code here
            //})();

            //async function processChildModels() {
            //    // Map each item into a promise
            //    const promises = $scope.childModels.map(async (child) => {
            //        const url = "/api/TemplateApi/GetItemNonVatFlagByItemCode?itemCode=" + child.ITEM_CODE;

            //        try {
            //            debugger;
            //            const response = await $http.get(url);
            //            child["NON_VAT_FLAG"] = response.data;
            //            $scope.$applyAsync(); // forces Angular to notice
            //        } catch (err) {
            //            console.error("Error with item", child.ITEM_CODE, err);
            //        }
            //    });

            //    // Wait for ALL promises to finish
            //    await Promise.all(promises);
            //}

            //// Usage
            //(async function () {
            //    await processChildModels();
            //    // Your outside code here
            //})();

            angular.forEach($scope.childModels, function (value, key) {
                debugger;
                if (typeof value[TOTAL_PRICE] !== 'undefined' && value[TOTAL_PRICE] !== null && value[TOTAL_PRICE] !== "") {

                    //console.log('value', value);
                    sum = parseFloat(sum) + (parseFloat(value[TOTAL_PRICE]));
                }
                else
                    return;
            });
            debugger;
            $scope.summary.grandTotal = (parseFloat(sum)).toFixed(2);
            //$scope.adtotal = $scope.summary.grandTotal;

            //var deffered = $.Deferred();
            //var grandtotalsaleOrder = formtemplateservice.getGrandTotalSalesOrder_ByFormCodeAndOrderNo_Ref(reforderno,deffered);
            //$.when(deffered).done(function (result) {

            //    //$scope.adtotal = result.data;

            //    $scope.masterModels;

            //});
            //if ($scope.DocumentName === "SA_SALES_RETURN") {



            if ($rootScope.IncludeCharge === "True") {
                var reforderno = response.data[0][primarycolumnname];
                var chargeUrlForEdit = "/api/TemplateApi/GetChargeDataForEdit?formCode=" + rows[0].FORM_CODE + "&&voucherNo=" + reforderno;
                $http.get(chargeUrlForEdit).then(function (res) {
                    debugger;
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
                        }, 0);
                    }

                });


                //subin change for automatic charge calculation
                // This Code automatically tries to calculate charge even if the charge is not included STARTS/////

                //else {
                //    angular.forEach($scope.ChargeList,
                //        function (chvalue, chkey) {

                //            $scope.sync(true, chvalue);

                //        });
                //}
                // This Code automatically tries to calculate charge even if the charge is not included ENDS///////
                ////
            }

            if ($scope.DocumentName == 'SA_SALES_INVOICE_JEWEL' || $scope.DocumentName == 'SA_SALES_RETURN_JEWEL') {
                if ($scope.childModels.length > 0) {
                    var chargeUrl = "/api/TemplateApi/GetChargeData?formCode=" + $scope.FormCode;

                    $http.get(chargeUrl).then(function (response) {
                        setTimeout(function () {
                            $scope.ChargeList = response.data;

                            $scope.newChargeListForJewellery = angular.copy($scope.ChargeList).sort(function (a, b) {
                                return a.PRIORITY_INDEX_NO - b.PRIORITY_INDEX_NO;
                            });

                            //for (var i = 0; i < $scope.childModels.length; i++) {
                            //    var nonVatFlagUrl = "/api/TemplateApi/GetItemNonVatFlagByItemCode?itemCode=" + $scope.childModels[i].ITEM_CODE;
                            //    $http.get(nonVatFlagUrl).then(function (response) {
                            //        debugger;

                            //        setTimeout(function () {
                            //            debugger;
                            //            $scope.childModels[i]["NON_VAT_FLAG"] = response.data;

                            //            $scope.calculateItemWiseChargeReference(i);
                            //        },0);

                            //    });
                            //   /* $scope.GrandtotalCalculationJewellery();*/
                            //}

                            for (var i = 0; i < $scope.childModels.length; i++) {
                                debugger;
                                (function (index) {
                                    debugger;
                                    var itemCode = $scope.childModels[index].ITEM_CODE;
                                    var nonVatFlagUrl = "/api/TemplateApi/GetItemNonVatFlagByItemCode?itemCode=" + itemCode;

                                    $http.get(nonVatFlagUrl).then(function (response) {
                                        setTimeout(function () {
                                            debugger;
                                            if ($scope.childModels[index]) {
                                                $scope.childModels[index]["NON_VAT_FLAG"] = response.data;
                                                $scope.calculateItemWiseChargeReference(index);
                                            }
                                        }, 0);
                                    });
                                })(i);
                            }

                            $scope.$apply();
                        }, 0);
                    });
                }

            }


            //}

        } else {

            $scope.masterModels = angular.copy($scope.masterModelTemplate);
        }
        setTimeout(function () {
            angular.forEach($scope.masterModels,
                function (mvalue, mkey) {

                    if (mkey === "PARTY_TYPE_CODE") {
                        var req = "/api/TemplateApi/getPartyTypeEdesc?partytypecode=" + mvalue;
                        $http.get(req).then(function (results) {
                            setTimeout(function () {
                                var searchText = "";

                                CustomerCode = $scope.masterModels.CUSTOMER_CODE;
                                var getdealerByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetPartyTypeByFilterAndCustomerCode?filter=" + searchText + '&customercode=' + CustomerCode;
                                $("#dealercode").kendoComboBox({
                                    optionLabel: "--Select Dealer Code--",
                                    filter: "contains",
                                    dataTextField: "PARTY_TYPE_EDESC",
                                    dataValueField: "PARTY_TYPE_CODE",

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
                                    select: function (e) {

                                        $('#style-switcher').addClass('opened');
                                        $('#style-switcher').animate({ 'left': '-241px', 'width': '273px' });

                                    }
                                });
                                if ($("#dealercode").data('kendoComboBox') != null) {
                                    $("#dealercode").data('kendoComboBox').dataSource.data([{ PARTY_TYPE_CODE: mvalue, PARTY_TYPE_EDESC: results.data, Type: "code" }]);
                                }
                            }, 0);
                        });
                    }
                });
        }, 1000);

        setTimeout(function () {
            debugger;
            //if ($scope.freeze_master_ref_flag == "Y") {
            //    angular.forEach($scope.ChildFormElement[0].element, function (value) {

            //        $("." + value.COLUMN_NAME + "_div input").attr('readonly', true);
            //        //if (value.COLUMN_NAME != "QUANTITY" && value.COLUMN_NAME != "UNIT_PRICE") {
            //        //    $("." + value.COLUMN_NAME + "_div input").attr('readonly', true);
            //        //}
            //    });
            //    $.each($scope.MasterFormElement, function (key, value) {

            //        //if (value['COLUMN_NAME'].indexOf('CODE') > -1) {
            //        //    $("." + value.COLUMN_NAME + " input").attr('readonly', true);
            //        //}
            //        $("." + value.COLUMN_NAME + " input").attr('readonly', true);
            //    });
            //}
            //if ($scope.freeze_master_ref_flag == "N") {
            //    angular.forEach($scope.ChildFormElement[0].element, function (value) {

            //        $("." + value.COLUMN_NAME + "_div input").attr('readonly', false);
            //        //if (value.COLUMN_NAME != "QUANTITY" && value.COLUMN_NAME != "UNIT_PRICE") {
            //        //    $("." + value.COLUMN_NAME + "_div input").attr('readonly', false);
            //        //}
            //    });
            //    $.each($scope.MasterFormElement, function (key, value) {

            //        //if (value['COLUMN_NAME'].indexOf('CODE') > -1) {
            //        //    $("." + value.COLUMN_NAME + " input").attr('readonly', false);
            //        //}
            //        $("." + value.COLUMN_NAME + " input").attr('readonly', false);
            //    });

            //}
            //if ($scope.ref_fix_qty == "Y") {
            //    angular.forEach($scope.ChildFormElement[0].element, function (value) {
            //        if (value.COLUMN_NAME == "QUANTITY") {
            //            $("." + value.COLUMN_NAME + "_div input").attr('readonly', true);
            //        }
            //    });
            //}
            //if ($scope.ref_fix_qty == "N") {
            //    angular.forEach($scope.ChildFormElement[0].element, function (value) {
            //        if (value.COLUMN_NAME == "QUANTITY") {
            //            $("." + value.COLUMN_NAME + "_div input").attr('readonly', false);
            //        }
            //    });
            //}

            //if ($scope.ref_fix_price == "Y") {
            //    angular.forEach($scope.ChildFormElement[0].element, function (value) {
            //        if (value.COLUMN_NAME == "UNIT_PRICE") {
            //            $("." + value.COLUMN_NAME + "_div input").attr('readonly', true);
            //        }
            //    });
            //}
            //if ($scope.ref_fix_price == "N") {
            //    angular.forEach($scope.ChildFormElement[0].element, function (value) {
            //        if (value.COLUMN_NAME == "UNIT_PRICE") {
            //            $("." + value.COLUMN_NAME + "_div input").attr('readonly', false);
            //        }
            //    });
            //}
            $.each($scope.MasterFormElement, function (key, value) {

                if (value['COLUMN_NAME'].indexOf('DATE') > -1) {

                    $("." + value.COLUMN_NAME + " input").attr("disabled", "disabled");
                    $($("." + value.COLUMN_NAME + " input").parent()[0]).css("height", "20px");
                    ;
                }

            });
            debugger;
            $scope.iamFromReference = true;
            $scope.showRefTab = true;
            if ($scope.freeze_master_ref_flag == "Y") {
                var prevValue = "0";
                $(".table_body input[type=number]").on('change', function () {
                    var $this = $(this);
                    var colName = $this[0].className.match(/[\w-]*QUANTITY[\w-]*/g) == null ? $this[0].className.match(/[\w-]*UNIT_PRICE[\w-]*/g) : $this[0].className.match(/[\w-]*QUANTITY[\w-]*/g);
                    var index = colName[0].split("_")[colName[0].split("_").length - 1];
                    prevValue = colName == "QUANTITY_" + index ? response.data[index].QUANTITY : response.data[index].UNIT_PRICE;
                    if (parseFloat(prevValue) <= parseFloat($this.val())) {
                        $this.val(prevValue);
                        var columnName = colName == "QUANTITY_" + index ? "QUANTITY" : "UNIT_PRICE";
                        $scope.childModels[index][columnName] = prevValue;
                    }
                    $scope.iamFromReference = false;
                    $scope.sum(index);
                    $scope.iamFromReference = true;
                    $scope.$apply();
                });
                setTimeout(function () {
                    $(".QUANTITY_0").trigger("change");
                }, 10);
            }

            if ($scope.iamFromReference) {
                debugger;
                var chargeData = angular.copy($scope.ChargeList);
                for (var i = 0; i < $scope.childModels.length; i++) {
                    debugger;
                    $scope.lineWiseChargeList[i] = angular.copy(chargeData);
                    $scope.chargeSumForReference(i);
                }

                //$scope.GrandtotalLineWiseCalculationForReference();

                $scope.GrandtotalCalution();

            }

            callback();
            $scope.$apply();
        }, 2000);


        if ($scope.RefTableName == "SA_LOADING_SLIP_DETAIL") {
            var ShippingDetailUrlForreference = "/api/TemplateApi/GetLoadingSlipListByReferenceno?VoucherNo=" + $scope.referenceDataDisplay1[0].REFERENCE_NO;
            $http.get(ShippingDetailUrlForreference).then(function (res) {



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
                    $scope.SDModel.TRANSPORTER_EDESC = res.data[0].TRANSPORTER_EDESC;
                    $scope.SDModel.VEHICLE_EDESC = res.data[0].VEHICLE_EDESC
                    //$("#mydropdownlist").val("thevalue");


                }



            });
        }
        else {
            var ShippingDetailUrlForreference = "/api/TemplateApi/GetAllShippingDtlsByVno?VoucherNo=" + $scope.referenceDataDisplay1[0].REFERENCE_NO;
            $http.get(ShippingDetailUrlForreference).then(function (res) {



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
                    $scope.SDModel.TRANSPORTER_EDESC = res.data[0].TRANSPORTER_EDESC;
                    $scope.SDModel.VEHICLE_EDESC = res.data[0].VEHICLE_EDESC
                    //$("#mydropdownlist").val("thevalue");


                }



            });
        }

        if ($scope.serial_tracking_flag == "Y") {

            var config = {
                async: false
            };
            $scope.BATCH_MODAL.push({ ITEM_CODE: "", ITEM_EDESC: "", MU_CODE: "", LOCATION_CODE: "", QUANTITY: "", TRACK: [] });
            $scope.BATCH_CHILD_MODAL.push({ SERIAL_NO: 1, TRACKING_SERIAL_NO: "" });
            for (var itt = 0; itt < $scope.childModels.length; itt++) {

                var responsevaluewise = $http.get('/api/TemplateApi/getItemCountResult?code=' + $scope.childModels[itt].ITEM_CODE, config);
                responsevaluewise.then(function (resvaluewise) {

                    if (resvaluewise.data == true) {


                        for (var it = 0; it < $scope.childModels.length; it++) {

                            if ($scope.childModels[it].hasOwnProperty("FROM_LOCATION_CODE")) {
                                if ($scope.childModels[it].FROM_LOCATION_CODE != 'undefined') {
                                    locationcode = $scope.childModels[it].FROM_LOCATION_CODE;


                                    var ibdrreq = "/api/TemplateApi/GetDataForBatchModalsales?itemcode=" + $scope.childModels[it].ITEM_CODE + "&loactioncode=" + $scope.childModels[it].FROM_LOCATION_CODE;
                                    $http.get(ibdrreq).then(function (ibdrreqresults) {

                                        if (ibdrreqresults.data.length > 0) {
                                            var rows = ibdrreqresults.data;

                                            for (var itn = 0; itn < $scope.childModels.length; itn++) {

                                                setbatchDataOnModal(rows, itn);
                                            }
                                        }
                                    })
                                }
                            }
                        }
                    }

                });

            }
        }
        //batchTransaction
        if ($scope.batch_tracking_flag == "Y" && $scope.serial_tracking_flag == "N") {
            $scope.BATCH_MODAL.push({ ITEM_CODE: "", ITEM_EDESC: "", MU_CODE: "", LOCATION_CODE: "", QUANTITY: "" });
            for (var itt = 0; itt < $scope.childModels.length; itt++) {
                $scope.batchTranIcon[itt] = true;
                var responsevaluewise = $http.get('/api/TemplateApi/BatchWiseItemCheck?code=' + $scope.childModels[itt].ITEM_CODE);
                responsevaluewise.then(function (resvaluewise) {
                    if (resvaluewise.data == true) {
                        for (var it = 0; it < $scope.childModels.length; it++) {
                            $scope.dynamicBatchTrackingModalData[it] = angular.copy($scope.BATCH_MODAL[0]);
                            $scope.childModels[it].QUANTITY = 0;
                        }
                    }
                });
            }
        }
    };

    function setbatchDataOnModal(rows, i) {

        //$scope.dynamicSerialTrackingModalData = [];
        var batchModel = angular.copy($scope.BATCH_MODAL[0]);
        var batchChildModel = angular.copy($scope.BATCH_CHILD_MODAL[0]);
        var rowsObj = rows;
        //if ($scope.dynamicSerialTrackingModalData.length==0) {
        //    $scope.dynamicSerialTrackingModalData.push(batchModel);
        //}

        //$scope.dynamicSerialTrackingModalData.push(batchModel);
        //$scope.dynamicSerialTrackingModalData.push($scope.getObjWithKeysFromOtherObj(batchModel, rowsObj));
        //if ($scope.dynamicSerialTrackingModalData[i].TRACK == undefined) {
        //    $scope.dynamicSerialTrackingModalData[i].TRACK = [];
        //    $scope.dynamicSerialTrackingModalData[i].TRACK.push(batchChildModel);
        //}
        $scope.dynamicSerialTrackingModalData[i] = $scope.getObjWithKeysFromOtherObj(batchModel, rowsObj);
        $scope.dynamicSerialTrackingModalData[i].TRACK = [];
        $scope.dynamicSerialTrackingModalData[i].TRACK.push(batchChildModel);

        for (var a = 0; a < rows.length; a++) {

            for (var b = 0; b < $scope.dynamicSerialTrackingModalData.length; b++) {

                if (rows[a].ITEM_CODE == $scope.dynamicSerialTrackingModalData[b].ITEM_CODE) {

                    $scope.dynamicSerialTrackingModalData[b].TRACK[a] = $scope.getObjWithKeysFromOtherObj(batchChildModel, rowsObj);
                }
            }
        }

    }
    function setbatchTran(rows, i) {
        var batchModel = angular.copy($scope.BATCH_MODAL[0]);
        $scope.dynamicBatchTrackingModalData[i] = $scope.getObjWithKeysFromOtherObj(batchModel, rows[i]);
    }

    var formDetail = formtemplateservice.getFormDetail_ByFormCode($scope.FormCode, d1);
    $.when(d1).done(function (result) {
        debugger;
        $scope.formDetail = result.data;
        if ($scope.formDetail.length > 0) {

            $scope.DocumentName = $scope.formDetail[0].TABLE_NAME;
            $scope.companycode = $scope.formDetail[0].COMPANY_CODE;
            $scope.NEGETIVE_STOCK_FLAG = $scope.formDetail[0].NEGATIVE_STOCK_FLAG;
            $scope.printCompanyInfo.companyName = $scope.formDetail[0].COMPANY_EDESC;
            $scope.printCompanyInfo.address = $scope.formDetail[0].ADDRESS;
            $scope.printCompanyInfo.formName = $scope.formDetail[0].FORM_EDESC;
            $scope.printCompanyInfo.phoneNo = $scope.formDetail[0].TELEPHONE;
            $scope.printCompanyInfo.email = $scope.formDetail[0].EMAIL;
            $scope.printCompanyInfo.tPinVatNo = $scope.formDetail[0].TPIN_VAT_NO;

        }
        var values = $scope.formDetail;
        //collection of Master elements
        angular.forEach(values, function (value, key) {
            if (value.MASTER_CHILD_FLAG == 'M' && value.DISPLAY_FLAG == 'Y') {
                $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                $scope.masterModelTemplate[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
            }

            if (value.MASTER_CHILD_FLAG == 'M' && value.DISPLAY_FLAG == 'Y') {
                this.push(value);
                //$scope.masterModelTemplate[value['COLUMN_NAME']] = null;

                if (value['COLUMN_NAME'].indexOf('DATE') > -1) {
                    $scope.masterModelTemplate[value['COLUMN_NAME']] = $filter('date')(new Date(), 'dd-MMM-yyyy');
                }
                if (value['COLUMN_NAME'] === 'CUSTOMER_CODE') {

                    $rootScope.CUSTOMER_CODE_DEFVAL_MASTER = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                }

            }
        }, $scope.MasterFormElement);

        var date = new Date();
        $scope.todaydate = $filter('date')(new Date(), 'dd-MMM-yyyy');
        debugger;
        if ($scope.OrderNo == undefined || $scope.OrderNo == "undefined") {
            var newordernumber = formtemplateservice.getnewlygeneratedvoucherno($scope.companycode, $scope.FormCode, $scope.todaydate, $scope.DocumentName, d6);

            $.when(d6).done(function (results) {
                debugger;
                $scope.newgenorderno = results.data;
                var primarycolumnname = PrimaryColumnForTable($scope.DocumentName);
                $scope.someDateFn();
                $scope.masterModels[primarycolumnname] = results.data;
                $scope.masterModelTemplate[primarycolumnname] = results.data;
            });

            if ($scope.DocumentName == 'SA_SALES_INVOICE_JEWEL') {
                var req = "/api/TemplateApi/GetFromLocationCode?companycode=" + $scope.companycode + "&formcode=" + $scope.FormCode;
                $http.get(req).then(function (results) {
                    debugger;


                    $scope.newFormLocationCode = results.data.DEFAULT_FROM_LOCATION;
                    $scope.newFormLocationName = results.data.DEFAULT_FROM_LOCATION_EDESC;
                    $scope.childModels[0].FROM_LOCATION_CODE = results.data.DEFAULT_FROM_LOCATION;
                    $scope.childModels[0]["FROM_LOCATION_CODE"] = results.data.DEFAULT_FROM_LOCATION;

                    var comboBox = $(`input[location-index='0']`).data("kendoComboBox");
                    var ds = comboBox.dataSource;

                    ds.fetch(function () {
                        var item = ds.data().find(i =>
                            String(i.LocationCode).trim().toLowerCase() ===
                            String(results.data.DEFAULT_FROM_LOCATION).trim().toLowerCase()
                        );

                        if (item) {
                            item.set("LocationName", results.data.DEFAULT_FROM_LOCATION_EDESC);
                        } else {
                            ds.add({
                                LocationCode: results.data.DEFAULT_FROM_LOCATION,
                                LocationName: results.data.DEFAULT_FROM_LOCATION_EDESC
                            });
                        }

                        // Set value and trigger change to sync display
                        comboBox.value(results.data.DEFAULT_FROM_LOCATION);
                        comboBox.trigger("change");
                    });

                });
            }
        }
        debugger;
        if ($scope.OrderNo != "undefined" && ($scope.DocumentName == 'SA_SALES_INVOICE_JEWEL' || $scope.DocumentName == 'SA_SALES_RETURN_JEWEL')) {
            debugger;

            var grandtotalAmountJewellery = formtemplateservice.getGrandTotalAmountJewellery_ByFormCodeAndOrderNo($scope.OrderNo, $scope.FormCode, $scope.DocumentName, d10);
            $.when(d10).done(function (result) {

                $scope.summary.grandTotalAmount = result.data;
                $scope.summary.grandTotal = $scope.summary.grandTotalAmount;
            });




        }

        if ($scope.OrderNo != "undefined") {
            debugger;
            var grandtotalPriceSaleOrder = formtemplateservice.getTotalPriceSalesOrder_ByFormCodeAndOrderNo($scope.OrderNo, $scope.FormCode, $scope.DocumentName, d11);
            $.when(d11).done(function (result) {
                debugger;
                $scope.summary.grandTotal = result.data;

                $scope.masterModels;

            });

        }

        var count = 0;
        //AA
        var tempFn = function (result) {
            debugger;
            $scope.INDIV_ITEM_CHARGE_MODAL.push({ ITEM_CODE: "", QUANTITY: "", UNIT_PRICE: "", TOTAL_PRICE: "", CALC_QUANTITY: "", CALC_UNIT_PRICE: "", CALC_TOTAL_PRICE: "", INV_ITEM_CHARGE_AMOUNT_WISE: [], INV_ITEM_CHARGE_QUANTITY_WISE: [] });
            $scope.INDIV_ITEM_CHARGE_MODAL_AMOUNT.push({ CHARGE_CODE: "", CHARGE_TYPE: "", CHARGE_EDESC: "", IMPACT_ON: "", APPLY_QUANTITY: "", VALUE_PERCENT_AMOUNT: "", CHARGE_AMOUNT: "", VALUE_PERCENT_FLAG: "", ACC_CODE: "", SUB_CODE: "", BUDGET_CODE: "", GL: "", APPORTION_FLAG: "", CALC: "", APPLY_ON: "" });
            $scope.INDIV_ITEM_CHARGE_MODAL_QUANTITY.push({ CHARGE_CODE: "", CHARGE_TYPE: "", CHARGE_EDESC: "", IMPACT_ON: "", APPLY_QUANTITY: "", VALUE_PERCENT_AMOUNT: "", CHARGE_AMOUNT: "", VALUE_PERCENT_FLAG: "", ACC_CODE: "", SUB_CODE: "", BUDGET_CODE: "", GL: "", APPORTION_FLAG: "", CALC: "", APPLY_ON: "" });

            var rows = result.data;
            if (count == 0) {
                if (rows.length > 0) {
                    count++;
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
                            $('.dz-remove').css("display", "block");
                        }
                    }

                    if ($scope.OrderNo != undefined && $scope.OrderNo != "undefined") {
                        debugger;
                        $scope.ChildFormElement = [];
                        $scope.childModels = [];
                        $scope.dynamicInvItenChargeModalData = [];
                        $scope.dynamicInvItenChargeModalData_OK = [];
                        $scope.dynamicSerialTrackingModalData = [];
                        $scope.dynamicBatchTrackingModalData = [];
                        $scope.newgenorderno = "";
                        $scope.save = "Update";
                        $scope.savecontinue = "Update & Continue";
                        //var mastertempCopy = angular.copy($scope.masterModelTemplate);
                        //var mastercopy = $scope.getObjWithKeysFromOtherObj(mastertempCopy, rows[0]);
                        //setTimeout(function () {
                        //    $scope.masterModels = {};
                        //    $scope.masterModels = angular.copy(mastercopy);
                        //    $scope.muwiseQty();

                        //})
                    }
                    for (var i = 0; i < rows.length; i++) {

                        //var tempCopy = angular.copy($scope.childModelTemplate);

                        //$scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
                        //$scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));
                        //var mastertempCopy = angular.copy($scope.masterModelTemplate);
                        //var mastercopy = $scope.getObjWithKeysFromOtherObj(mastertempCopy, rows[i]);

                        //$scope.masterModels = angular.copy(mastercopy);

                        //AA
                        setDataOnModal(rows, i);

                    }


                    ///---------Nila Changes for Attribute Setup Commented Out Start---------////

                    //if ($scope.OrderNo != undefined && $scope.OrderNo != "undefined") {
                    //    setItemWiseAttribute();
                    //}

                    ///---------Nila Changes for Attribute Setup Commented Out End---------////

                    $scope.muwiseQty();
                    $scope.someDateFn();

                    if ($scope.DocumentName == "SA_SALES_INVOICE") {
                        if ($scope.OrderNo != undefined && $scope.OrderNo != "undefined") {
                            setTimeout(function () {
                                $(".btn-action a").css('display', 'none');
                            }, 0);
                        }

                    }
                    if ($scope.DocumentName == "SA_SALES_RETURN") {
                        if ($scope.OrderNo != undefined && $scope.OrderNo != "undefined") {
                            setTimeout(function () {

                                $(".btn-action a").css('display', 'none');
                                var childlen = $scope.childModels.length;
                                for (var i = 0; i < childlen; i++) {

                                    //if ($scope.childModels[0].hasOwnProperty("QUANTITY")) {
                                    //    $("." + "QUANTITY_" + i).attr("disabled", "disabled");
                                    //}
                                    if ($scope.childModels[0].hasOwnProperty("UNIT_PRICE")) {
                                        $("." + "UNIT_PRICE_" + i).attr("disabled", "disabled");
                                    }
                                    //if ($scope.childModels[0].hasOwnProperty("TOTAL_PRICE")) {
                                    //    $("." + "TOTAL_PRICE" + i).attr("disabled", "disabled");
                                    //}
                                    if ($scope.childModels[0].hasOwnProperty("CALC_QUANTITY")) {
                                        $("." + "CALC_QUANTITY_" + i).attr("disabled", "disabled");
                                    }
                                    if ($scope.childModels[0].hasOwnProperty("CALC_UNIT_PRICE")) {
                                        $("." + "CALC_UNIT_PRICE_" + i).attr("disabled", "disabled");
                                    }
                                    //if ($scope.childModels[0].hasOwnProperty("CALC_TOTAL_PRICE")) {
                                    //    $("." + "CALC_TOTAL_PRICE" + i).attr("disabled", "disabled");
                                    //}
                                }
                            }, 0);
                        }

                    }



                    //$("#refrencetype").val();
                } else {
                    $scope.masterModels = angular.copy($scope.masterModelTemplate);
                }

                function setItemWiseAttribute() {
                    debugger;

                    var childModalVal = $scope.childModels;

                    var mappedData = childModalVal.map(function (item, index) {
                        return {
                            ITEM_CODE: item.ITEM_CODE,
                            SERIAL_NO: index + 1
                        };
                    });

                    // Start processing sequentially
                    processItem(0);

                    function processItem(i) {
                        if (i >= mappedData.length) return; // finished all items

                        var row = mappedData[i];

                        var itemWiseAttributeUrl =
                            "/api/TemplateApi/GetItemWiseAttribute"
                            + "?itemCode=" + encodeURIComponent(row.ITEM_CODE)
                            + "&serialNo=" + encodeURIComponent(row.SERIAL_NO)
                            + "&voucherNo=" + encodeURIComponent($scope.OrderNo)
                            + "&tableName=" + encodeURIComponent($scope.DocumentName);

                        $http.get(itemWiseAttributeUrl).then(function (response) {
                            debugger;

                            var extra = response.data;
                            if (extra == null) {
                                extra = "";
                            }

                            // Update dropdown or model
                            updateDropdownDescription(row.ITEM_CODE, extra, i);

                            // Move to the next item
                            processItem(i + 1);
                        });
                    }
                }

                function updateDropdownDescription(itemCode, extraValue, index) {
                    debugger;
                    // Get the Kendo ComboBox widget
                    var combo = $("#products_" + index).data("kendoComboBox");
                    if (!combo) {
                        console.warn("ComboBox not ready for index:", index);
                        return;
                    }

                    var ds = combo.dataSource.data();

                    var item = ds.find(x => x.ItemCode == itemCode);

                    if (item) {
                        if (!item.OriginalDescription) {
                            item.OriginalDescription = item.ItemDescription;
                        }

                        item.ItemDescription = item.OriginalDescription + " " + extraValue;

                        // Refresh UI
                        combo.refresh();
                    }
                }

                function setDataOnModal(rows, i) {
                    debugger;

                    console.log("Row data ==============================>>>>" + JSON.stringify(rows));

                    var tempCopy = angular.copy($scope.childModelTemplate);

                    $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
                    $scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));
                    //subin
                    setTimeout(function () {

                        $("#products_" + i).data('kendoComboBox').dataSource.data([{ ItemCode: rows[i].ITEM_CODE, ItemDescription: rows[i].ITEM_EDESC, Type: "code" }]);
                        console.log("value of productCode from kendo===================>>>>" + JSON.stringify($("#products_" + i).data('kendoComboBox').dataSource.data()[0].ItemCode));
                        $scope.childModels[i].ITEM_CODE = $("#products_" + i).data('kendoComboBox').dataSource.data()[0].ItemCode;



                        $("#customers").data('kendoComboBox').dataSource.data([{ CustomerCode: rows[i].CUSTOMER_CODE, CustomerName: rows[i].CUSTOMER_EDESC, Type: "code", REGD_OFFICE_EADDRESS: rows[i].REGD_OFFICE_EADDRESS, TPIN_VAT_NO: rows[i].TPIN_VAT_NO, TEL_MOBILE_NO1: rows[i].TEL_MOBILE_NO1, CUSTOMER_NDESC: rows[i].CUSTOMER_NDESC }]);
                        console.log("value of customer from kendo===================>>>>" + JSON.stringify($('#customers').data('kendoComboBox').dataSource.data()[0].CustomerCode));
                        $scope.masterModels.CUSTOMER_CODE = $('#customers').data('kendoComboBox').dataSource.data()[0].CustomerCode;

                        var searchText = "";

                        CustomerCode = $scope.masterModels.CUSTOMER_CODE;
                        var getdealerByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetPartyTypeByFilterAndCustomerCode?filter=" + searchText + '&customercode=' + CustomerCode;
                        $("#dealercode").kendoComboBox({
                            optionLabel: "--Select Dealer Code--",
                            filter: "contains",
                            dataTextField: "PARTY_TYPE_EDESC",
                            dataValueField: "PARTY_TYPE_CODE",

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
                            select: function (e) {

                                $('#style-switcher').addClass('opened');
                                $('#style-switcher').animate({ 'left': '-241px', 'width': '273px' });

                            }
                        });

                        if ($("#dealercode").data('kendoComboBox') != null) {
                            $("#dealercode").data('kendoComboBox').dataSource.data([{ PARTY_TYPE_CODE: rows[i].PARTY_TYPE_CODE, PARTY_TYPE_EDESC: rows[i].PARTY_TYPE_EDESC, Type: "code" }]);
                        }


                    }, 0);


                    var mastertempCopy = angular.copy($scope.masterModelTemplate);
                    var mastercopy = $scope.getObjWithKeysFromOtherObj(mastertempCopy, rows[i]);
                    $scope.masterModels = angular.copy(mastercopy);
                    console.log("$scope.masterModels======================>>>>" + JSON.stringify($scope.masterModels));
                    var itemchargeModel = angular.copy($scope.INDIV_ITEM_CHARGE_MODAL[0]);
                    var itemchargeamountwiseModel = angular.copy($scope.INDIV_ITEM_CHARGE_MODAL_AMOUNT[0]);
                    var itemchargequantitywiseModel = angular.copy($scope.INDIV_ITEM_CHARGE_MODAL_QUANTITY[0]);

                    var rowsObj = rows[i];
                    $scope.dynamicInvItenChargeModalData.push($scope.getObjWithKeysFromOtherObj(itemchargeModel, rowsObj));
                    if ($scope.dynamicInvItenChargeModalData[i].INV_ITEM_CHARGE_AMOUNT_WISE == undefined) {
                        $scope.dynamicInvItenChargeModalData[i].INV_ITEM_CHARGE_AMOUNT_WISE = [];
                        $scope.dynamicInvItenChargeModalData[i].INV_ITEM_CHARGE_AMOUNT_WISE.push(itemchargeamountwiseModel);
                    }
                    if ($scope.dynamicInvItenChargeModalData[i].INV_ITEM_CHARGE_QUANTITY_WISE == undefined) {
                        $scope.dynamicInvItenChargeModalData[i].INV_ITEM_CHARGE_QUANTITY_WISE = [];
                        $scope.dynamicInvItenChargeModalData[i].INV_ITEM_CHARGE_QUANTITY_WISE.push(itemchargequantitywiseModel);
                    }


                    $scope.dynamicInvItenChargeModalData_OK.push($scope.getObjWithKeysFromOtherObj(itemchargeModel, rowsObj));
                    if ($scope.dynamicInvItenChargeModalData_OK[i].INV_ITEM_CHARGE_AMOUNT_WISE == undefined) {
                        $scope.dynamicInvItenChargeModalData_OK[i].INV_ITEM_CHARGE_AMOUNT_WISE = [];
                        $scope.dynamicInvItenChargeModalData_OK[i].INV_ITEM_CHARGE_AMOUNT_WISE.push(itemchargeamountwiseModel);
                    }
                    if ($scope.dynamicInvItenChargeModalData_OK[i].INV_ITEM_CHARGE_QUANTITY_WISE == undefined) {
                        $scope.dynamicInvItenChargeModalData_OK[i].INV_ITEM_CHARGE_QUANTITY_WISE = [];
                        $scope.dynamicInvItenChargeModalData_OK[i].INV_ITEM_CHARGE_QUANTITY_WISE.push(itemchargequantitywiseModel);
                    }


                    var config = {
                        async: false
                    };


                    //for (var b = 0; b < $scope.dynamicInvItenChargeModalData.length; b++) {
                    angular.forEach($scope.dynamicInvItenChargeModalData, function (value, key) {

                        var responsevaluewise = $http.get('/api/TemplateApi/GetItemChargeDataSavedValueWise?voucherNo=' + rows[i].SALES_NO + '&itemcode=' + rows[i].ITEM_CODE, config);

                        responsevaluewise.then(function (resvaluewise) {
                            if (resvaluewise.data != "") {

                                for (var a = 0; a < resvaluewise.data.length; a++) {

                                    $scope.dynamicInvItenChargeModalData[key].INV_ITEM_CHARGE_AMOUNT_WISE[a] = $scope.getObjWithKeysFromOtherObjInvCharge(itemchargeamountwiseModel, resvaluewise.data[a]);
                                    $scope.dynamicInvItenChargeModalData_OK[key].INV_ITEM_CHARGE_AMOUNT_WISE[a] = $scope.getObjWithKeysFromOtherObjInvCharge(itemchargeamountwiseModel, resvaluewise.data[a]);
                                }

                            }

                        });

                    });
                    if ($scope.masterModels.hasOwnProperty("CUSTOMER_CODE")) {

                        searchText = "";
                        var getpricelistidByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetPriceListByFilterAndCustomerCode?filter=" + searchText + '&customercode=' + $scope.masterModels.CUSTOMER_CODE;
                        BindPriceList($scope.masterModels.CUSTOMER_CODE, "");


                    }

                    if ($scope.serial_tracking_flag == "Y") {

                        var config = {
                            async: false
                        };
                        $scope.BATCH_MODAL.push({ ITEM_CODE: "", ITEM_EDESC: "", MU_CODE: "", LOCATION_CODE: "", QUANTITY: "", TRACK: [] });
                        $scope.BATCH_CHILD_MODAL.push({ SERIAL_NO: 1, TRACKING_SERIAL_NO: "" });
                        for (var itt = 0; itt < $scope.childModels.length; itt++) {

                            var responsevaluewise = $http.get('/api/TemplateApi/getItemCountResult?code=' + $scope.childModels[itt].ITEM_CODE, config);
                            responsevaluewise.then(function (resvaluewise) {

                                if (resvaluewise.data == true) {


                                    for (var it = 0; it < $scope.childModels.length; it++) {

                                        if ($scope.childModels[it].hasOwnProperty("FROM_LOCATION_CODE")) {
                                            if ($scope.childModels[it].FROM_LOCATION_CODE != 'undefined') {
                                                locationcode = $scope.childModels[it].FROM_LOCATION_CODE;


                                                var ibdrreq = "/api/TemplateApi/GetDataForBatchModalsalesforedit?itemcode=" + $scope.childModels[it].ITEM_CODE + "&loactioncode=" + $scope.childModels[it].FROM_LOCATION_CODE + "&voucherno=" + $scope.OrderNo;
                                                $http.get(ibdrreq).then(function (ibdrreqresults) {

                                                    if (ibdrreqresults.data.length > 0) {
                                                        var rows = ibdrreqresults.data;

                                                        for (var itn = 0; itn < $scope.childModels.length; itn++) {

                                                            setbatchDataOnModal(rows, itn);
                                                        }
                                                    }
                                                })
                                            }
                                        }
                                    }
                                }

                            });

                        }
                    }
                    //for batch Transaction
                    if ($scope.batch_tracking_flag == "Y" && $scope.serial_tracking_flag == "N") {
                        $scope.BATCH_MODAL.push({ ITEM_CODE: "", ITEM_EDESC: "", MU_CODE: "", LOCATION_CODE: "", QUANTITY: "" });
                        for (var itt = 0; itt < $scope.childModels.length; itt++) {
                            $scope.batchTranIcon[itt] = true;
                            var responsevaluewise = $http.get('/api/TemplateApi/BatchWiseItemCheck?code=' + $scope.childModels[itt].ITEM_CODE);
                            responsevaluewise.then(function (resvaluewise) {
                                if (resvaluewise.data == true) {
                                    for (var it = 0; it < $scope.childModels.length; it++) {
                                        if ($scope.childModels[it].hasOwnProperty("FROM_LOCATION_CODE")) {
                                            if ($scope.childModels[it].FROM_LOCATION_CODE != 'undefined') {
                                                locationcode = $scope.childModels[it].FROM_LOCATION_CODE;
                                                var ibdrreq = "/api/TemplateApi/GetbatchTranDataByItemCodeAndLocCode?itemcode=" + $scope.childModels[it].ITEM_CODE + "&loactioncode=" + $scope.childModels[it].FROM_LOCATION_CODE + "&refernceNo=" + $scope.OrderNo;
                                                $http.get(ibdrreq).then(function (ibdrreqresults) {
                                                    if (ibdrreqresults.data.length > 0) {
                                                        angular.forEach(ibdrreqresults.data, function (value, key) {
                                                            if (!$scope.checkedBatchTranResult.find(obj => obj["BATCH_NO"] === value.BATCH_NO)) {
                                                                $scope.checkedBatchTranResult.push({
                                                                    "ITEM_CODE": value.ITEM_CODE,
                                                                    "MU_CODE": value.MU_CODE,
                                                                    "LOCATION_CODE": value.LOCATION_CODE,
                                                                    "SERIAL_NO": value.SERIAL_NO,
                                                                    "SERIAL_TRACKING_FLAG": $scope.serial_tracking_flag,
                                                                    "BATCH_TRACKING_FLAG": $scope.batch_tracking_flag,
                                                                    "QUANTITY": value.QUANTITY,
                                                                    "BATCH_NO": value.BATCH_NO,
                                                                    "EXPIRY_DATE": value.EXPIRY_DATE
                                                                });
                                                                $scope.quantitySum = $scope.quantitySum + value.QUANTITY;
                                                            }
                                                        });
                                                        var rows = ibdrreqresults.data;
                                                        for (var itn = 0; itn < $scope.childModels.length; itn++) {
                                                            setbatchTran(rows, itn);
                                                        }
                                                    }
                                                })
                                            }
                                        }
                                    }
                                }
                            });
                        }
                    }
                }
            }
        };
        //if ($scope.masterChildData === null) {
        //    $scope.masterModelDataFn(tempFn);
        //} else {
        //    tempFn($scope.masterChildData);
        //}
        document.test = $scope.MasterFormElement;
        //collection of child elements.
        angular.forEach(values, function (value, key) {
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
                if (value['COLUMN_NAME'].indexOf('FLAG') > -1) {
                    if (value.DEFA_VALUE == null) {
                        value.DEFA_VALUE = "";
                    }
                    $scope.childModels[0]["STOCK_BLOCK_FLAG"] = value.DEFA_VALUE;
                }
                if (value['COLUMN_NAME'] === "COMPLETED_QUANTITY") {
                    if (value.DEFA_VALUE == null) {
                        value.DEFA_VALUE = "";
                    }
                    $scope.childModels[0]["COMPLETED_QUANTITY"] = value.DEFA_VALUE;
                }
                if (value['COLUMN_NAME'] === "ITEM_CODE") {


                    //value.DEFA_VALUE == null ? "" : $scope.childModels[0]["ITEM_CODE"] = value.DEFA_VALUE;
                    $rootScope.ITEM_CODE_DEFAULTVAL = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
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
                else {
                    $scope.childModels[0][value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                }
            }
        }, $scope.ChildFormElement[0].element);

        document.childtest = $scope.ChildFormElement[0].element;
        elements = $scope.MasterFormElement;
        //additional child element reservation.

        angular.forEach(values, function (value, key) {
            ///////////--- Before getting Total price in childModal in Jewellery -----//////////////
            if (value.MASTER_CHILD_FLAG == 'C' && value.DISPLAY_FLAG == 'Y') {
                this.push(value);
                if (value['COLUMN_NAME'].indexOf('FLAG') > -1) {
                    $scope.childModelTemplate[value['COLUMN_NAME']] = "N";
                    value['CHILD_ELEMENT_WIDTH'] = "50";
                }
                else if (value['COLUMN_NAME'].indexOf('MU') > -1) {
                    $scope.childModelTemplate[value['COLUMN_NAME']] = "";
                    value['CHILD_ELEMENT_WIDTH'] = "50";
                }
                else if (value['COLUMN_NAME'].indexOf('CODE') > -1) {
                    $scope.childModelTemplate[value['COLUMN_NAME']] = "";
                    value['CHILD_ELEMENT_WIDTH'] = "175";
                }
                else if (value['COLUMN_NAME'].indexOf('QUANTITY') > -1) {
                    $scope.childModelTemplate[value['COLUMN_NAME']] = "";
                    value['CHILD_ELEMENT_WIDTH'] = "50";
                }
                else if (value['COLUMN_NAME'].indexOf('PRICE') > -1) {
                    $scope.childModelTemplate[value['COLUMN_NAME']] = "";
                    value['CHILD_ELEMENT_WIDTH'] = "75";
                }
                else {
                    $scope.childModelTemplate[value['COLUMN_NAME']] = null;
                }
                //$scope.childModelTemplate.SERIAL_NO = null;
            }


            ///////////--- After getting Total price in childModal in Jewellery -----//////////////
            //if (value.MASTER_CHILD_FLAG == 'C' ) {
            //    this.push(value);
            //    if (value.DISPLAY_FLAG == 'Y') {
            //        if (value['COLUMN_NAME'].indexOf('FLAG') > -1) {
            //            $scope.childModelTemplate[value['COLUMN_NAME']] = "N";
            //            value['CHILD_ELEMENT_WIDTH'] = "50";
            //        }
            //        else if (value['COLUMN_NAME'].indexOf('MU') > -1) {
            //            $scope.childModelTemplate[value['COLUMN_NAME']] = "";
            //            value['CHILD_ELEMENT_WIDTH'] = "50";
            //        }
            //        else if (value['COLUMN_NAME'].indexOf('CODE') > -1) {
            //            $scope.childModelTemplate[value['COLUMN_NAME']] = "";
            //            value['CHILD_ELEMENT_WIDTH'] = "175";
            //        }
            //        else if (value['COLUMN_NAME'].indexOf('QUANTITY') > -1) {
            //            $scope.childModelTemplate[value['COLUMN_NAME']] = "";
            //            value['CHILD_ELEMENT_WIDTH'] = "50";
            //        }
            //        else if (value['COLUMN_NAME'].indexOf('PRICE') > -1) {
            //            $scope.childModelTemplate[value['COLUMN_NAME']] = "";
            //            value['CHILD_ELEMENT_WIDTH'] = "75";
            //        }
            //        else {
            //            $scope.childModelTemplate[value['COLUMN_NAME']] = null;
            //        }
            //    }
            //    else {
            //        if (value['COLUMN_NAME'].indexOf('TOTAL_PRICE') > -1) {
            //            $scope.childModelTemplate[value['COLUMN_NAME']] = ;
            //            value['CHILD_ELEMENT_WIDTH'] = "75";
            //        }
            //    }
            //    //$scope.childModelTemplate.SERIAL_NO = null;
            //}
        }, $scope.aditionalChildFormElement);

        if (typeof $scope.OrderNo != "undefined") {
            $scope.childModels[0]["NON_VAT_FLAG"] = '';
            $scope.childModelTemplate['NON_VAT_FLAG'] = '';
        }

        debugger;
        if ($scope.masterChildData === null) {
            $scope.masterModelDataFn(tempFn);
        } else {

            tempFn($scope.masterChildData);
        }
    })


    //subin changes
    if (typeof $scope.OrderNo != "undefined") {
        debugger;
        var grandtotalsaleOrder = formtemplateservice.getGrandTotalSalesOrder_ByFormCodeAndOrderNo($scope.OrderNo, $scope.FormCode, d5);
        $.when(d5).done(function (result) {
            debugger;
            $scope.summary.grandTotal = result.data;
            $scope.adtotal = $scope.summary.grandTotal;
            $scope.masterModels;

        });

    }

    $scope.add_child_element = function (e) {
        debugger;

        $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
        $scope.childModels.push(angular.copy($scope.childModelTemplate));

        var childLen = $scope.childModels.length - 1;


        $scope.lineWiseChargeList[childLen] = angular.copy($scope.ChargeList);

        if ($scope.DocumentName == 'SA_SALES_INVOICE_JEWEL') {
            $scope.childModels[childLen].FROM_LOCATION_CODE = $scope.newFormLocationCode;
            $scope.childModels[childLen]["FROM_LOCATION_CODE"] = $scope.newFormLocationCode;

            $timeout(function () {
                var comboBoxChild = $(`input[location-index='${childLen}']`).data("kendoComboBox");
                if (!comboBoxChild) {
                    console.warn("ComboBox not found at index", childLen);
                    return;
                }

                var ds = comboBoxChild.dataSource;
                // Update item logic here...


                ds.fetch(function () {
                    var item = ds.data().find(i =>
                        String(i.LocationCode).trim().toLowerCase() ===
                        String($scope.newFormLocationCode).trim().toLowerCase()
                    );

                    if (item) {
                        item.set("LocationName", $scope.newFormLocationName);
                    } else {
                        ds.add({
                            LocationCode: $scope.newFormLocationCode,
                            LocationName: $scope.newFormLocationName
                        });
                    }

                    // Set value and trigger change to sync display
                    comboBoxChild.value($scope.newFormLocationCode);
                    comboBoxChild.trigger("change");
                });
            }, 0);



        }



        $.each($scope.ChildFormElement[0].element, function (childkey, childelementvalue) {
            if (childelementvalue.DEFA_VALUE != null)
                $scope.childModels[childLen][childelementvalue.COLUMN_NAME] = childelementvalue.DEFA_VALUE;
        });

        $scope.childRowIndex = childLen;

        $scope.dynamicInvItenChargeModalData.push({
            ITEM_CODE: 0,
            QUANTITY: "",
            UNIT_PRICE: "",
            TOTAL_PRICE: "",
            CALC_QUANTITY: "",
            CALC_UNIT_PRICE: "",
            CALC_TOTAL_PRICE: "",
            INV_ITEM_CHARGE_AMOUNT_WISE: [{
                CHARGE_CODE: "",
                CHARGE_TYPE: "",
                CHARGE_EDESC: "",
                IMPACT_ON: "",
                APPLY_QUANTITY: "",
                VALUE_PERCENT_AMOUNT: "",
                CHARGE_AMOUNT: "",
                VALUE_PERCENT_FLAG: "",
                ACC_CODE: "",
                SUB_CODE: "",
                BUDGET_CODE: "",
                GL: "",
                APPORTION_FLAG: "",
                CALC: "",
                APPLY_NO: "",
            }],
            INV_ITEM_CHARGE_QUANTITY_WISE: [{
                CHARGE_CODE: "",
                CHARGE_TYPE: "",
                CHARGE_EDESC: "",
                IMPACT_ON: "",
                APPLY_QUANTITY: "",
                VALUE_PERCENT_AMOUNT: "",
                CHARGE_AMOUNT: "",
                VALUE_PERCENT_FLAG: "",
                ACC_CODE: "",
                SUB_CODE: "",
                BUDGET_CODE: "",
                GL: "",
                APPORTION_FLAG: "",
                CALC: "",
                APPLY_NO: "",
            }]
        });
        $scope.dynamicInvItenChargeModalData_OK.push({
            ITEM_CODE: 0,
            QUANTITY: "",
            UNIT_PRICE: "",
            TOTAL_PRICE: "",
            CALC_QUANTITY: "",
            CALC_UNIT_PRICE: "",
            CALC_TOTAL_PRICE: "",
            INV_ITEM_CHARGE_AMOUNT_WISE: [{
                CHARGE_CODE: "",
                CHARGE_TYPE: "",
                IMPACT_ON: "",
                APPLY_QUANTITY: "",
                VALUE_PERCENT_AMOUNT: "",
                CHARGE_AMOUNT: "",
                VALUE_PERCENT_FLAG: "",
                ACC_CODE: "",
                SUB_CODE: "",
                BUDGET_CODE: "",
                GL: "",
                APPORTION_FLAG: "",
                CALC: "",
                APPLY_NO: "",
            }],
            INV_ITEM_CHARGE_QUANTITY_WISE: [{
                CHARGE_CODE: "",
                CHARGE_TYPE: "",
                IMPACT_ON: "",
                APPLY_QUANTITY: "",
                VALUE_PERCENT_AMOUNT: "",
                CHARGE_AMOUNT: "",
                VALUE_PERCENT_FLAG: "",
                ACC_CODE: "",
                SUB_CODE: "",
                BUDGET_CODE: "",
                GL: "",
                APPORTION_FLAG: "",
                CALC: "",
                APPLY_NO: "",
            }]
        });
        //$scope.dynamicSerialTrackingModalData.push[{
        //ITEM_CODE: "",
        //ITEM_EDESC: "",
        //MU_CODE: "",
        //LOCATION_CODE: "",
        //QUANTITY: "",
        //TRACK: [{
        //    SERIAL_NO: 1,
        //    TRACKING_SERIAL_NO: "",
        //}]

        //}];
        $scope.dynamicSerialTrackingModalData.push({
            ITEM_CODE: "",
            ITEM_EDESC: "",
            MU_CODE: "",
            LOCATION_CODE: "",
            QUANTITY: "",
            TRACK: [{
                SERIAL_NO: 1,
                TRACKING_SERIAL_NO: "",
            }]
        });
        //$scope.dynamicBatchTrackingModalData.push({
        //    ITEM_CODE: "",
        //    ITEM_EDESC: "",
        //    MU_CODE: "",
        //    LOCATION_CODE: "",
        //    QUANTITY: ""
        //});
        //$.each($scope.childModels, function (key, value) {

        //    $.each($scope.ChildFormElement[0].element, function (childkey, childelementvalue) {


        //        if (childelementvalue.DEFA_VALUE != null) {
        //            var lcvalue = $scope.childModels[key].FROM_LOCATION_CODE
        //            if (lcvalue == null || lcvalue == undefined || lcvalue == "") {
        //                $scope.childModels[key][childelementvalue.COLUMN_NAME] = childelementvalue.DEFA_VALUE;
        //            }
        //        }
        //    });
        //});

        $scope.batchTranIcon[$scope.childRowIndex] = false;
    }

    // remove child row.
    $scope.remove_child_element = function (index) {
        debugger;
        //if ($scope.ChildFormElement.length > 1) {
        //    //if (index > 0) {
        //    if (typeof $scope.childModels[index][TOTAL_PRICE] !== "undefined" && $scope.childModels[index][TOTAL_PRICE] != null) {
        //        $scope.summary.grandTotal = $scope.summary.grandTotal - $scope.childModels[index][TOTAL_PRICE];
        //    }

        if ($scope.ChildFormElement.length > 1) {
            $scope.ChildFormElement.splice(index, 1);
            $scope.childModels.splice(index, 1);

            // Clean up tracked account tooltip entry for the deleted row and re-index
            if (typeof window.removeTrackedAccount === 'function') {
                window.removeTrackedAccount(index);
            }
        }
        if ($scope.lineWiseChargeList.length > 1) {
            $scope.lineWiseChargeList.splice(index, 1);

        }
        if ($scope.DocumentName == 'SA_SALES_INVOICE_JEWEL' || $scope.DocumentName == 'SA_SALES_RETURN_JEWEL') {
            debugger;
            var childLength = $scope.ChildFormElement.length;
            $scope.GrandtotalCalculationJewellery();
        } else {
            $scope.GrandtotalCalution();
        }
        $scope.muwiseQty();

        // }

    }

    var formSetup = formtemplateservice.getFormSetup_ByFormCode($scope.FormCode, d2);
    $.when(d2).done(function (result) {
        $scope.formSetup = result.data;
        $scope.FormName = $scope.formSetup[0].FORM_EDESC;
        if ($scope.formSetup.length > 0) {
            $scope.ModuleCode = $scope.formSetup[0].MODULE_CODE;
        }

    });

    // ========== CASH VOUCHER MODAL IMPLEMENTATION ==========
    // Check if RECEIPT_FLAG is enabled
    var checkReceiptFlag = function (message) {
        if ($scope.formSetup && $scope.formSetup[0] && $scope.formSetup[0].RECEIPT_FLAG === 'Y') {
            // Initialize Cash Voucher Data
            var cashVoucherReceiptUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetReceiptForCashVoucher";
            $http({
                method: 'GET',
                url: cashVoucherReceiptUrl,
            }).then(function successCallback(response) {
                $scope.receiptMessage = message;
                // Populate receipt list from API
                $scope.cashVoucherData.RECEIPT_LIST = response.data;

                // Set Sales No from masterModels
                $scope.cashVoucherData.SALES_NO = $scope.masterModels["SALES_NO"] || '';

                // Get Customer Code from masterModels
                var customerCode = $scope.masterModels["CUSTOMER_CODE"] || '';
                $scope.cashVoucherData.CUSTOMER_CODE = customerCode;

                // Get Customer Name from Kendo ComboBox dataSource (already loaded, no API call needed)
                var customerComboBox = $("#customers").data("kendoComboBox");
                var customerName = '';

                if (customerComboBox && customerComboBox.dataItem()) {
                    // Get the selected customer's name from the ComboBox
                    customerName = customerComboBox.dataItem().CustomerName || customerCode;
                } else if (customerCode && customerComboBox) {
                    // Fallback: try to find customer in the dataSource
                    var dataSource = customerComboBox.dataSource.data();
                    var customerData = dataSource.find(function (item) {
                        return item.CustomerCode === customerCode;
                    });
                    customerName = customerData ? customerData.CustomerName : customerCode;
                } else {
                    customerName = customerCode;
                }

                // Set Customer Name for display only (not sent to backend)
                $scope.cashVoucherData.CUSTOMER_NAME_DISPLAY = customerName;

                // Set Sales Amount from adtotal
                $scope.cashVoucherData.SALES_AMOUNT = Number($scope.adtotal) || 0;

                // Initialize Tender and Balance amounts
                $scope.cashVoucherData.TENDER_AMOUNT = 0;
                $scope.cashVoucherData.BALANCE_AMOUNT = Number($scope.cashVoucherData.SALES_AMOUNT);

                // Initialize all receipt amounts to 0 (as numbers, not strings)
                angular.forEach($scope.cashVoucherData.RECEIPT_LIST, function (receipt) {
                    receipt.AMOUNT = 0;
                });

                // Open the modal (for testing)
                $('#CashVoucherModal').modal('show');

            }, function errorCallback(response) {
                console.error("Error fetching cash voucher receipts:", response);
            });
        }
    };

    // Call after formSetup is loaded
    //$.when(d2).done(function () {
    //    checkReceiptFlag();
    //});

    // Cash Voucher: Calculate Balance Amount with validation
    $scope.calculateCashVoucherBalance = function () {

        var tenderAmount = 0;

        angular.forEach($scope.cashVoucherData.RECEIPT_LIST, function (receipt) {
            // Parse the amount, handling both string and number inputs
            var amt = parseFloat(receipt.AMOUNT);
            if (!isNaN(amt) && amt !== null && amt !== undefined) {
                tenderAmount += amt;
            }
        });

        $scope.cashVoucherData.TENDER_AMOUNT = Number(tenderAmount.toFixed(2));

        var salesAmount = Number($scope.cashVoucherData.SALES_AMOUNT) || 0;
        var balanceAmount = salesAmount - tenderAmount;

        balanceAmount = balanceAmount < 0 ? 0 : balanceAmount;

        $scope.cashVoucherData.BALANCE_AMOUNT = Number(balanceAmount.toFixed(2));

        if (tenderAmount > salesAmount) {
            displayPopupNotification("Tender Amount cannot be more than Sales Amount", "warning");
        } else if (salesAmount - tenderAmount < 0) {
            displayPopupNotification("Balance amount shouldn't be negative", "warning");
        }
    };

    // Cash Voucher: Auto-fill balance amount on F1 key press
    $scope.fillBalanceAmount = function (event, receipt) {
        // Check if F1 key is pressed (keyCode 112)
        if (event.keyCode === 112) {
            event.preventDefault(); // Prevent browser F1 help

            // Set the current receipt amount to the balance amount
            // Explicitly convert to Number to prevent ngModel:numfmt error
            var balanceAmount = Number($scope.cashVoucherData.BALANCE_AMOUNT) || 0;
            receipt.AMOUNT = balanceAmount;

            // Use $timeout to ensure Angular digest cycle processes the change
            $timeout(function () {
                $scope.calculateCashVoucherBalance();
            }, 0);
        }
    };

    // Cash Voucher: Save Receipt Data
    $scope.saveCashVoucher = function () {
        debugger;

        // Validate that balance is 0 or close to 0
        if ($scope.cashVoucherData.BALANCE_AMOUNT > 0.01) {
            displayPopupNotification("Balance amount must be zero before saving", "warning");
            return;
        } else if ($scope.cashVoucherData.TENDER_AMOUNT > $scope.cashVoucherData.SALES_AMOUNT) {
            displayPopupNotification("Tender Amount cannot be more than Sales Amount", "warning");
            return;
        }

        $('#CashVoucherModal').modal('hide');
        $('#receiptConfirmationModal').modal('show');


        // TODO: Implement save logic here
        //console.log("Saving Cash Voucher:", $scope.cashVoucherData);

        //// For now, just close the modal
        //$('#CashVoucherModal').modal('hide');
        //displayPopupNotification("Cash voucher will be saved (functionality to be implemented)", "info");
    };
    // ========== END CASH VOUCHER IMPLEMENTATION ==========

    $scope.cancelCashVoucher = function () {


        $('#CashVoucherModal').modal('hide');
        $('#receiptCancellationModal').modal('show');

    };

    $scope.confirmReceiptEntry = function () {

        debugger;
        var message = $scope.receiptMessage;
        // Filter RECEIPT_LIST to only include items with AMOUNT > 0
        var filteredReceiptList = $scope.cashVoucherData.RECEIPT_LIST.filter(function (receipt) {
            return receipt.AMOUNT && Number(receipt.AMOUNT) > 0;
        });

        //// Create model with filtered data
        $scope.cashVoucherData.FORM_CODE = $scope.FormCode;
        var model = angular.copy($scope.cashVoucherData);
        model.RECEIPT_LIST = filteredReceiptList;

        var staturl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/SaveCashVoucherData";
        var response = $http({
            method: "POST",
            data: model,
            url: staturl,
            contentType: "application/json",
            dataType: "json"
        });
        return response.then(function (data) {
            debugger;

            // Close the CashVoucherModal
            $('#CashVoucherModal').modal('hide');

            $('#receiptConfirmationModal').modal('hide');

            //$scope.showSimplePopup(data.data.MESSAGE);

            // Handle different MESSAGE types with appropriate navigation logic
            if (message == "INSERTED") {
                $scope.showSimplePopup(data.data.MESSAGE, function () {
                    location.reload();
                    var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Template/SplitterIndex#!DT/MenuSplitter/04";
                    $window.location.href = landingUrl;
                });
            }
            else if (message == "SAVEANDPRINT") {
                //var url = '/Print/Home/PreviewPattern?formCode=' + $scope.dzformcode + '&companyCode=' + $scope.companycode + '&mainFieldValue=' + encodeURIComponent($scope.dzvouchernumber);
                //window.open(url, '_blank');

                $scope.showSimplePopup(data.data.MESSAGE, function () {
                    location.reload();
                    var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Home/Index#!DT/formtemplates/" + $scope.dzformcode;
                    $window.location.href = landingUrl;
                });
            }
            else if (message == "UPDATEDANDPRINT") {
                //var url = '/Print/Home/PreviewPattern?formCode=' + $scope.dzformcode + '&companyCode=' + $scope.companycode + '&mainFieldValue=' + encodeURIComponent(data.data.VoucherNo);
                //window.open(url, '_blank');

                $scope.showSimplePopup(data.data.MESSAGE, function () {
                    location.reload();
                    var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Home/Index#!DT/formtemplates/" + $scope.dzformcode;
                    $window.location.href = landingUrl;
                });
            }
            else if (message == "INSERTEDANDCONTINUE") {
                $scope.showSimplePopup(data.data.MESSAGE);
                $scope.resetFormAfterSave();
            }
            else if (message == "UPDATED") {
                $scope.showSimplePopup(data.data.MESSAGE, function () {
                    location.reload();
                    var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Template/SplitterIndex#!DT/MenuSplitter/04";
                    $window.location.href = landingUrl;
                });
            }
            $scope.receiptMessage = null;
        });
    };

    $scope.confirmReceiptEntryCancellation = function () {

        var message = $scope.receiptMessage;
        $('#CashVoucherModal').modal('hide');
        $('#receiptCancellationModal').modal('hide');
        if (message == "INSERTED") {
            location.reload();
            var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Template/SplitterIndex#!DT/MenuSplitter/04";
            $window.location.href = landingUrl;

        }
        else if (message == "SAVEANDPRINT") {
            //var url = '/Print/Home/PreviewPattern?formCode=' + $scope.dzformcode + '&companyCode=' + $scope.companycode + '&mainFieldValue=' + encodeURIComponent($scope.dzvouchernumber);
            //window.open(url, '_blank');

            location.reload();
            var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Home/Index#!DT/formtemplates/" + $scope.dzformcode;
            $window.location.href = landingUrl;

        }
        else if (message == "UPDATEDANDPRINT") {
            //var url = '/Print/Home/PreviewPattern?formCode=' + $scope.dzformcode + '&companyCode=' + $scope.companycode + '&mainFieldValue=' + encodeURIComponent(data.data.VoucherNo);
            //window.open(url, '_blank');

            location.reload();
            var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Home/Index#!DT/formtemplates/" + $scope.dzformcode;
            $window.location.href = landingUrl;

        }
        else if (message == "INSERTEDANDCONTINUE") {
            $scope.resetFormAfterSave();
        }
        else if (message == "UPDATED") {
            location.reload();
            var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Template/SplitterIndex#!DT/MenuSplitter/04";
            $window.location.href = landingUrl
        }
        $scope.receiptMessage = null;
    };

    $scope.cancelReceiptEntry = function () {

        $('#CashVoucherModal').modal('show');
        $('#receiptConfirmationModal').modal('hide');
    };

    $scope.cancelReceiptEntryCancellation = function () {

        $('#CashVoucherModal').modal('show');
        $('#receiptCancellationModal').modal('hide');
    };

    var formCustomSetup = formtemplateservice.getFormCustomSetup_ByFormCode($scope.FormCode, $scope.OrderNo, d3)
    $.when(d3).done(function (result) {
        $scope.formCustomSetup = result.data;
        var customvalues = $scope.formCustomSetup;
        angular.forEach(customvalues, function (value, key) {
            this.push(value);
            if ($scope.OrderNo == "undefined")
                $scope.customModels[value['FIELD_NAME']] = value['DEFA_FIELD_VALUE'];
            else
                $scope.customModels[value['FIELD_NAME']] = value['FIELD_VALUE'];
        }, $scope.CustomFormElement);

        //console.log($scope.customModels);


    });
    //$(".c").css("border:0px solid blue");
    $scope.loadingBtn = function () {
        $("#savedocumentformdata").button('loading');
        $(".portlet-title .btn").attr("disabled", "disabled");

    }

    $scope.loadingBtnReset = function () {
        $("#savedocumentformdata").button('reset')
        $(".portlet-title .btn").attr("disabled", false);
    }

    //---------------------- Draft start ----------------------------
    $scope.showDraftModal = function () {

        var draftUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetDraftList?moduleCode=" + $scope.ModuleCode + "&formCode=" + $scope.FormCode;
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

                if (e.item == null)
                    return clearField(e), displayPopupNotification("Invalid data", "warning");
                else
                    $(e.sender.element[0]).removeClass('borderRed');
                var dataItem = this.dataItem(e.item.index());
                $scope.saveAsDraft.TEMPLATE_NO = dataItem.TEMPLATE_CODE;
                $scope.saveAsDraft.TEMPLATE_EDESC = dataItem.TEMPLATE_EDESC;
                $scope.saveAsDraft.TEMPLATE_NDESC = dataItem.TEMPLATE_EDESC;
                $scope.draftsave = "Update";
            },
        }
        function clearField(kendoEvent) {
            if (kendoEvent.sender.dataItem() == undefined) {
                $('#formDraftTemplate').data("kendoComboBox").value([]);
                $(kendoEvent.sender.element[0]).addClass('borderRed');
            }

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

    $scope.AddNewChildCustomer = function () {
        debugger;
        $scope.customersMasterChildArr = [];
        var returnMaxCustomerUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/MaxCustomerChild";
        $http({
            method: 'GET',
            url: returnMaxCustomerUrl,
        }).then(function successCallback(response) {
            debugger;
            $scope.customersMasterChildArr.CHILD_AUTOGENERATED = response.data;
        }, function errorCallback(response) {
            displayPopupNotification(response.data.STATUS_CODE, "error");

        });

        var customerMasterMapUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetCustomerMasterMap";
        $http({
            method: 'GET',
            url: customerMasterMapUrl,
        }).then(function successCallback(response) {
            debugger;
            // $scope.customersMasterChildArr.CHILD_AUTOGENERATED = response.data;//

            //$scope.customersMasterChildArr.PARTY_TYPE = response.data.PARTY_TYPE_CODE;
            //$scope.customersMasterChildArr.ACCOUNTMAP = response.data.ACC_CODE;

            $scope.customersMasterChildArr.PARENT_TYPE = response.data.PARENT_GROUP;

        }, function errorCallback(response) {
            displayPopupNotification(response.data.STATUS_CODE, "error");

        });

        $("#customerModalInsideSalesInvoiceJewellery").modal("toggle");
        /*$('#myTab a[href="#tab-1"]').tab('show');*/

    }
    $scope.saveNewCustomerChildInSIJ = function (isValid) {
        debugger;
        //var validation = [
        //    { childCustomerEdesc: $scope.customersMasterChildArrform.childCustomerEdesc.$invalid },
        //    { childPermanentAddress: $scope.customersMasterChildArrform.childPermanentAddress.$invalid },
        //    { childPanVat: $scope.customersMasterChildArrform.childPanVat.$invalid },
        //    { childPartyType: $scope.customersMasterChildArrform.childPartyType.$invalid },

        //]

        //if (validation[0].childCustomerEdesc == true) {

        //    displayPopupNotification("Enter English Nmae", "warning");
        //    return
        //}
        //if (validation[1].childPermanentAddress == true) {

        //    displayPopupNotification(" Enter Permanent Address", "warning");

        //    return
        //}

        //if (validation[2].childPanVat == true) {

        //    displayPopupNotification("PAN Number Should be 9 Letter only", "warning");

        //    return
        //}
        //if (validation[3].childPartyType == true) {

        //    displayPopupNotification("Enter Party Type ", "warning");

        //    return
        //}






        //for (var i = 0; i < $scope.divisionList.length; i++) {
        //    if ($scope.divisionList[i].CREDIT_LIMIT == "" || $scope.divisionList[i].DIVISION_CODE == "") {
        //        $scope.divisionList.splice(i, 1);
        //        i--;
        //    }
        //    else {
        //        $scope.divisionList[i].BLOCK_FLAG = $scope.divisionList[i].BLOCK_FLAG == true ? "Y" : "N";
        //    }
        //};
        //for (var i = 0; i < $scope.ownerList.length; i++) {
        //    if ($scope.ownerList[i].OWNER_NAME == "") {
        //        $scope.ownerList.splice(i, 1);
        //        i--;
        //    }
        //};
        //for (var i = 0; i < $scope.invoiceWiseList.length; i++) {
        //    if ($scope.invoiceWiseList[i].ACC_CODE == "" || $scope.invoiceWiseList[i].BALANCE_AMOUNT == "" || $scope.invoiceWiseList[i].INVOICE_DATE == "" || $scope.invoiceWiseList[i].REFERENCE_NO == "") {
        //        $scope.invoiceWiseList.splice(i, 1);
        //        i--;
        //    }
        //};
        //for (var i = 0; i < $scope.otherInfoList.length; i++) {
        //    if ($scope.otherInfoList[i].FIELD_NAME == "" || $scope.otherInfoList[i].FIELD_VALUE == "") {
        //        $scope.otherInfoList.splice(i, 1);
        //        i--;
        //    }
        //};
        //for (var i = 0; i < $scope.alternativeLocationInfoList.length; i++) {
        //    if ($scope.alternativeLocationInfoList[i].OFFICE_EDESC == "") {
        //        $scope.alternativeLocationInfoList.splice(i, 1);
        //        i--;
        //    }
        //};
        //for (var i = 0; i < $scope.budgetCenterList.length; i++) {
        //    if ($scope.budgetCenterList[i].BUDGET_CODE == "") {
        //        $scope.budgetCenterList.splice(i, 1);
        //        i--;
        //    }
        //};
        //for (var i = 0; i < $scope.sisterConcernsList.length; i++) {
        //    if ($scope.sisterConcernsList[i].SISTER_CONCERN_EDESC == "") {
        //        $scope.sisterConcernsList.splice(i, 1);
        //        i--;
        //    }
        //};
        //for (var i = 0; i < $scope.otherTermsConditionsList.length; i++) {
        //    if ($scope.otherTermsConditionsList[i].FIELD_NAME == "" || $scope.otherTermsConditionsList[i].FIELD_VALUE == "") {
        //        $scope.otherTermsConditionsList.splice(i, 1);
        //        i--;
        //    }
        //};
        //for (var i = 0; i < $scope.customerStockStatusList.length; i++) {
        //    if ($scope.customerStockStatusList[i].ITEM_CODE == "" || $scope.customerStockStatusList[i].QUANTITY == "" || $scope.customerStockStatusList[i].STOCK_DATE == "") {
        //        $scope.customerStockStatusList.splice(i, 1);
        //        i--;
        //    }
        //};
        debugger;
        $scope.customersMasterChildArr.IS_ACTIVE = $scope.customersMasterChildArr.IS_ACTIVE;
        $scope.customersMasterChildArr.IS_CASH = $scope.customersMasterChildArr.IS_CASH == true ? "Y" : "N";
        /*  $scope.customersMasterChildArr.CASH_CUSTOMER_FLAG = $scope.customersMasterChildArr.CASH_CUSTOMER_FLAG == "true" ? "Y" : "N";*/
        $scope.customersMasterChildArr.EXCLUSIVE = $scope.customersMasterChildArr.EXCLUSIVE == true ? "Y" : "N";
        //if (!isValid) {
        //    displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
        //    return;
        //}

        //onchnge of checkbox
        //$scope.change = function (evt) {
        //    debugger;
        //    if($scope.customersMasterChildArr.CASH_CUSTOMER_FLAG==="Y")
        //    {
        //        $scope.partyTypeOptions = {
        //            dataSource: CANature,
        //            dataTextField: "text",
        //            dataValueField: "key",
        //        }
        //    }
        //    else
        //    {
        //        $scope.partyTypeOptions = {
        //            dataSource: SDNature,
        //            dataTextField: "text",
        //            dataValueField: "key",
        //        }

        //    }
        //};

        var master_customer_code = $scope.customersMasterChildArr.PARENT_TYPE;
        var telephone = $scope.customersMasterChildArr.TELEPHONE;
        var permanent_address = $scope.customersMasterChildArr.PERMANENT_ADDRESS;
        var party_type = $scope.customersMasterChildArr.PARTY_TYPE;
        var account_map = $scope.customersMasterChildArr.ACCOUNTMAP;
        var customer_name = $scope.customersMasterChildArr.CHILD_CUSTOMER_EDESC;
        var city = $scope.customersMasterChildArr.CITY;

        if (customer_name == null || customer_name == "" || customer_name == undefined) {
            displayPopupNotification("Name In English is required", "warning");
            return $scope.loadingBtnReset();
        }

        if (master_customer_code == null || master_customer_code == "" || master_customer_code == undefined) {
            displayPopupNotification("Parent Group is required", "warning");
            return $scope.loadingBtnReset();
        }
        if (telephone == null || telephone == "" || telephone == undefined) {
            displayPopupNotification("Telephone/Mobile is required", "warning");
            return $scope.loadingBtnReset();
        }
        if (permanent_address == null || permanent_address == "" || permanent_address == undefined) {
            displayPopupNotification("Permanent Address is required", "warning");
            return $scope.loadingBtnReset();
        }
        if (party_type == null || party_type == "" || party_type == undefined) {
            displayPopupNotification("Party Type is required", "warning");
            return $scope.loadingBtnReset();
        }
        if (account_map == null || account_map == "" || account_map == undefined) {
            displayPopupNotification("Account Map is required", "warning");
            return $scope.loadingBtnReset();
        }
        if (city == null || city == "" || city == undefined) {
            displayPopupNotification("City is required", "warning");
            return $scope.loadingBtnReset();
        }

        var createCustomerChildUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/createChildCustomerInSIJ";
        var model = {


            CUSTOMER_EDESC: $scope.customersMasterChildArr.CHILD_CUSTOMER_EDESC,
            GROUP_SKU_FLAG: "I",
            MASTER_CUSTOMER_CODE: $scope.customersMasterChildArr.PARENT_TYPE,
            REGD_OFFICE_EADDRESS: $scope.customersMasterChildArr.PERMANENT_ADDRESS,
            TEL_MOBILE_NO1: $scope.customersMasterChildArr.TELEPHONE,
            PAN_VAT: $scope.customersMasterChildArr.PAN_VAT,
            PARTY_TYPE_CODE: $scope.customersMasterChildArr.PARTY_TYPE,
            ACC_CODE: $scope.customersMasterChildArr.ACCOUNTMAP,
            CHILD_AUTOGENERATED: $scope.customersMasterChildArr.CHILD_AUTOGENERATED,
            CASH_CUSTOMER_FLAG: $scope.customersMasterChildArr.CASH_CUSTOMER_FLAG,
            EXCLUSIVE_FLAG: $scope.customersMasterChildArr.EXCLUSIVE,
            DEALING_PERSON: $scope.customersMasterChildArr.DEALING_PERSON,
            REGION_CODE: $scope.customersMasterChildArr.REGION_CODE,
            CITY_CODE: $scope.customersMasterChildArr.CITY
        }
        $http({
            method: 'POST',
            url: createCustomerChildUrl,
            data: model,
        }).then(function successCallback(response) {
            debugger;
            //if (response.data.MESSAGE == "INSERTED") {
            if (isValid == "saveAndContinuee") {
                $scope.AfterSaveCustomerCode = $scope.customersMasterChildArr.CHILD_AUTOGENERATED;
            }
            else {
                $scope.clearCustomersMasterChildArr();
                //$scope.AfterSaveCustomerCode = "";
                //$scope.divisionList = [];
                //$scope.ownerList = [];
                //$scope.invoiceWiseList = [];
                //$scope.otherTermsConditionsList = [];
                //$scope.otherInfoList = [];
                //$scope.alternativeLocationInfoList = [];
                //$scope.budgetCenterList = [];
                //$scope.sisterConcernsList = [];
                //$scope.customerStockStatusList = [];
            }
            //}
            //else {

            //}
            /*  reinitialize();*/
            //$scope.treenodeselected = "N";
            displayPopupNotification("Data succesfully saved ", "success");
            $("#customerModalInsideSalesInvoiceJewellery").modal("toggle");
            $("#customerModalInsideSalesInvoiceJewellery").modal("hide");



            //else {


            //    displayPopupNotification(response.data.STATUS_CODE, "error");
            //}
            // this callback will be called asynchronously
            // when the response is available
        }
            //, function errorCallback(response) {
            //var message = response.data.MESSAGE.replace(/ *\([^)]*\) */g, " ");
            //displayPopupNotification(message, "error");
            //// called asynchronously if an error occurs
            //// or server returns response with an error status.
            //}
        );
    }


    $scope.clearCustomersMasterChildArr = function () {

        $scope.customersMasterChildArr.PARENT_TYPE = "";
        $scope.customersMasterChildArr.PERMANENT_ADDRESS = "";
        $scope.customersMasterChildArr.TELEPHONE = "";
        $scope.customersMasterChildArr.PAN_VAT = "";
        $scope.customersMasterChildArr.PARTY_TYPE = "";
        $scope.customersMasterChildArr.ACCOUNTMAP = "";
        $scope.customersMasterChildArr.CHILD_AUTOGENERATED = "";
        $scope.customersMasterChildArr.CASH_CUSTOMER_FLAG = "";
        $scope.customersMasterChildArr.EXCLUSIVE = "";
        $scope.customersMasterChildArr.DEALING_PERSON = "";
        $scope.customersMasterChildArr.REGION_CODE = "";
        $scope.customersMasterChildArr.CHILD_CUSTOMER_EDESC = "";
    }


    var regionUrlSIJ = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getAllRegions";
    $scope.regionDataSourceSIJ = new kendo.data.DataSource({
        serverFiltering: true,
        transport: {
            read: {
                url: regionUrlSIJ,
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
    });
    $scope.regionOptionsSIJ = {
        dataSource: $scope.regionDataSourceSIJ,
        optionLabel: "--Select Region--",
        dataTextField: "REGION_EDESC",
        dataValueField: "REGION_CODE",

    }


    var cityUrlSIJ = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getAllCities";
    $scope.cityDataSourceSIJ = new kendo.data.DataSource({
        serverFiltering: true,
        transport: {
            read: {
                url: cityUrlSIJ,
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
    });
    $scope.cityOptionsSIJ = {
        dataSource: $scope.cityDataSourceSIJ,

        optionLabel: "--Select City--",
        dataTextField: "CITY_EDESC",
        dataValueField: "CITY_CODE",
    }



    $scope.openCustomerSetupModal = function () {
        debugger;
        bootbox.confirm({
            title: "M/s Party Name",
            message: "This M/s Party Name doesn't exist do you want to add this?",
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
                debugger;
                if (result === true) {
                    $scope.AddNewChildCustomer();

                }

            }

        });


    };


    var partyTypeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getAllPartyTypes";

    //$scope.partyTypeOptionsSIJ = {
    //    dataSource: new kendo.data.DataSource({
    //        type: "json",

    //    serverFiltering: true,
    //        transport: {
    //            read: {
    //                url: partyTypeUrl,

    //            },
    //            parameterMap: function (data, action) {
    //                var newParams;

    //                if (data.filter != undefined) {
    //                    if (data.filter.filters[0] != undefined) {
    //                        newParams = {
    //                            filter: data.filter.filters[0].value
    //                        };
    //                        return newParams;
    //                    }
    //                    else {
    //                        newParams = {
    //                            filter: ""
    //                        };
    //                        return newParams;
    //                    }
    //                }
    //                else {
    //                    newParams = {
    //                        filter: ""
    //                    };
    //                    return newParams;
    //                }
    //            }
    //        }
    //    }),
    //    dataTextField: "PARTY_TYPE_EDESC",
    //    dataValueField: "PARTY_TYPE_CODE",
    //    optionLabel: "--Select Party Type--"

    //};

    $scope.partyTypeOptionsSIJ = {
        dataSource: new kendo.data.DataSource({
            type: "json",
            transport: {
                read: {
                    url: partyTypeUrl,
                },
                parameterMap: function (data, action) {
                    var newParams = {
                        filter: data.filter?.filters[0]?.value || ""
                    };
                    return newParams;
                }
            }
        }),
        dataTextField: "PARTY_TYPE_EDESC",
        dataValueField: "PARTY_TYPE_CODE",
        optionLabel: "--Select Party Type--"
    };

    $scope.onChangePartyTypeCustomerSetup = function (e) {
        $timeout(function () {
            // Your change handling logic here
        }, 100);
    };


    var accMapUrlSIJ = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetAllAccountCodesupp";
    $scope.accMapDataSourceSIJ = new kendo.data.DataSource({

        transport: {
            read: {
                url: accMapUrlSIJ,
            },
            parameterMap: function (data, action) {
                debugger;
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
    });
    $scope.accMapOptionsSIJ = {
        dataSource: $scope.accMapDataSourceSIJ,
        optionLabel: "--Select Account--",
        dataTextField: "ACC_EDESC",
        dataValueField: "ACC_CODE",

    }

    $scope.getDraftDetails = function () {
        debugger;
        var d7 = $.Deferred();
        var templateCode = $("#formDraftTemplate").data("kendoComboBox").value();
        if (templateCode == "" || templateCode == null)
            return displayPopupNotification("Please select the template.", "warning");
        var formDetail = formtemplateservice.getDraftFormDetail_ByFormCode(templateCode, d7);
        $.when(d7).done(function (result) {
            debugger;
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
            debugger;
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
                debugger;
                var tempCopy = angular.copy($scope.childModelTemplate);
                $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
                $scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, result));


            }
            debugger;
            angular.forEach($scope.masterModels,
                function (mvalue, mkey) {
                    debugger;
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

            $("#getDraftModal").modal("toggle");
        })
    }

    $scope.SaveAsDraft = function () {

        if ($scope.masterModels.DIVISION_CODE != "" && $scope.childModels.ITEM_CODE != "") {
            $("#saveAsDraftModal").modal("toggle");
        } else {
            displayPopupNotification("Please insert details before saving as draft", "warning");
        }
    }

    $scope.saveTemplateInDraft = function () {
        var saveflag;
        if ($scope.draftsave == "Save")
            saveflag = "0";
        else
            saveflag = "1";
        var formcode = $scope.FormCode;
        var tablename = $scope.DocumentName;
        var orderno = $scope.OrderNo;
        var masterelementvalues = $scope.masterModels;
        var grandtotal = $scope.summary.grandTotal;

        var masterElementJson = {};
        var primarycolumnname = PrimaryColumnForTable(tablename);
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

        var model = {
            Save_Flag: saveflag,
            Table_Name: tablename,
            Form_Code: formcode,
            Order_No: orderno,
            Grand_Total: grandtotal,
            FORM_TEMPLATE: $scope.saveAsDraft,
            Master_COLUMN_VALUE: JSON.stringify($scope.masterModels),
            Child_COLUMNS: childcolumnkeys,
            Child_COLUMN_VALUE: JSON.stringify($scope.childModels),
            PRIMARY_COL_NAME: primarycolumnname,
            Custom_COLUMN_VALUE: JSON.stringify($scope.customModels),
            CHARGES: JSON.stringify($scope.data),
        };

        var staturl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/SaveAsDraftFormDataOld";
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
            $scope.masterChildData = [];
            $scope.dzvouchernumber = data.data.VoucherNo;
            $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
            $scope.dzformcode = data.data.FormCode;
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
            //var req = "/api/TemplateApi/GetNewOrderNo?companycode=" + $scope.companycode + "&formcode=" + $scope.FormCode + "&currentdate=" + $scope.todaydate + "&tablename=" + $scope.DocumentName + '&isSequence=false';
            //$http.get(req).then(function (results) {

            //    $scope.newgenorderno = results.data;
            //    var primarycolumnname = PrimaryColumnForTable($scope.DocumentName);
            //    $scope.someDateFn();
            //    $scope.masterModels[primarycolumnname] = results.data;
            //    $scope.masterModelTemplate[primarycolumnname] = results.data;
            //});
            $scope.summary.grandTotal = 0;

            $scope.summary = { 'grandTotal': 0 };
            $scope.units = [];
            $scope.totalQty = 0;

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

    $scope.getNepaliDate = function (date) {
        return AD2BS(moment(date).format('YYYY-MM-DD'));
    }

    $scope.getNepaliDateJewellery = function (date) {
        return AD2BS(moment(date).format('YYYY-MM-DD'));
    }

    $scope.refresh = function () {

        //location.reload();
        $scope.ResetDocument();

    };

    $scope.cnlPrint = function () {

        var ss = $scope.buttonflag;
        location.reload();

        var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Home/Index#!DT/formtemplates/" + formcode + "/" + orderno;
        $window.location.href = landingUrl;



        //var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Home/Index#!DT/formtemplate/" + $scope.FormCode; //commented by aakash, uncomment if problem rise
        //$window.location.href = landingUrl; //commented by aakash  uncomment if problem rise

        // $scope.ResetDocumentPrint();  //commmented by aakash   uncomment if problem rise
        //  $("#saveAndPrintModal").modal("toggle");
        //$scope.OrderNo = "undefined";
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


    function setFirstFormLocationCode() {
        debugger;
        if ($scope.childModels[0].hasOwnProperty("FROM_LOCATION_CODE")) {
            child_from_location = $scope.childModels[0].FROM_LOCATION_CODE;
            if (child_from_location == "") {
                if (($scope.DocumentName == "SA_SALES_INVOICE_JEWEL" || $scope.DocumentName == "SA_SALES_RETURN_JEWEL") && ($scope.OrderNo == undefined || $scope.OrderNo == "undefined")) {
                    $scope.childModels[0]["FROM_LOCATION_CODE"] = $scope.newFormLocationCode;

                }
            }
        }
    }


    //-------------------- Quick Setup End----------------------------



    // save data
    $scope.SaveDocumentFormData = function (param) {
        debugger;
        $scope.loadingBtn();
        setFirstFormLocationCode();
        //Validations Start
        if ($scope.masterModels.hasOwnProperty("CUSTOMER_CODE")) {
            var master_customer_code = $scope.masterModels.CUSTOMER_CODE;

            if (master_customer_code == null || master_customer_code == "" || master_customer_code == undefined) {

                $(".mcustomer").addClass("borderRed")
                displayPopupNotification("Customer Code is required", "warning");
                return $scope.loadingBtnReset();
            }
            else {
                $(".mcustomer").removeClass("borderRed");
            }
        };
        if ($scope.Dealer_system_flag == "Y") {
            debugger;
            var mpartytype_code = $scope.masterModels.PARTY_TYPE_CODE;
            if (mpartytype_code === "") {
                //subin change
                if ($('#dealercode').data("kendoComboBox").dataItem() == null || $('#dealercode').data("kendoComboBox").dataItem() == "" || $('#dealercode').data("kendoComboBox").dataItem() == undefined) {
                    displayPopupNotification("Dealer Requried Because of Dealer system Enable in this Company", "warning");
                    return $scope.loadingBtnReset();
                }
                else {
                    var dataSelected = $('#dealercode').data("kendoComboBox").dataItem().PARTY_TYPE_CODE;
                    mpartytype_code = dataSelected;
                    $scope.masterModels.PARTY_TYPE_CODE = dataSelected;
                }

            }
            if (mpartytype_code == null || mpartytype_code == "" || mpartytype_code == undefined) {

                $(".mpartytype").addClass("borderRedwithBbackground");
                displayPopupNotification("Dealer Requried Because of Dealer system Enable in this Company", "warning");
                return $scope.loadingBtnReset();
            }
            else {
                var mpartytype_code = $scope.masterModels.PARTY_TYPE_CODE;
                if (mpartytype_code === "") {
                    var dataSelected = $('#dealercode').data("kendoComboBox").dataItem().PARTY_TYPE_CODE;
                    mpartytype_code = dataSelected;
                    $scope.masterModels.PARTY_TYPE_CODE = dataSelected;
                }
                $(".mpartytype").removeClass("borderRedwithBbackground");
            }

        };
        if ($scope.masterModels.hasOwnProperty("MANUAL_NO")) {
            var manual_no = $scope.masterModels.MANUAL_NO;
            if (/^[a-zA-Z0-9- ]*$/.test(manual_no) == false) {
                displayPopupNotification("Manual No shouldnot contain special characters", "warning");
                return $scope.loadingBtnReset();
            }
        };
        if ($scope.masterModels.hasOwnProperty("REMARKS")) {
            var manual_no = $scope.masterModels.MANUAL_NO;
            if (/^[a-zA-Z0-9- ]*$/.test(manual_no) == false) {
                displayPopupNotification("Remarks shouldnot contain special characters", "warning");
                return $scope.loadingBtnReset();
            }
        };
        if ($scope.masterModels.hasOwnProperty("FROM_LOCATION_CODE")) {
            var master_From_location_code = $scope.masterModels.FROM_LOCATION_CODE;
            if (master_From_location_code == null || master_From_location_code == "" || master_From_location_code == undefined) {
                displayPopupNotification("Location Code is required", "warning");
                return $scope.loadingBtnReset();
            }
            else {
                $(".mlocation").removeClass("borderRed");
            }

        };
        if ($scope.masterModels.hasOwnProperty("TO_LOCATION_CODE")) {
            var master_to_location_code = $scope.masterModels.TO_LOCATION_CODE;
            if (master_to_location_code == null || master_to_location_code == "" || master_to_location_code == undefined) {
                displayPopupNotification("Location Code is required", "warning");
                return $scope.loadingBtnReset();
            }
            else {
                $(".mtolocation").removeClass("borderRed");
            }
        };
        if ($scope.masterModels.hasOwnProperty("CURRENCY_CODE")) {
            var master_currency_code = $scope.masterModels.CURRENCY_CODE;
            if (master_currency_code == null || master_currency_code == "" || master_currency_code == undefined) {
                displayPopupNotification("Currency Code is required", "warning");
                return $scope.loadingBtnReset();
            }
            else {
                $(".mcurrency").removeClass("borderRed");
            }
        };
        if ($scope.masterModels.hasOwnProperty("SUPPLIER_CODE")) {
            var master_supplier_code = $scope.masterModels.SUPPLIER_CODE;
            if (master_supplier_code == null || master_supplier_code == "" || master_supplier_code == undefined) {
                displayPopupNotification("Supplier Code is required", "warning");
                return $scope.loadingBtnReset();
            }
            else {
                $(".msupplier").removeClass("borderRed");
            }
        };
        if ($scope.masterModels.hasOwnProperty("PRIORITY_CODE")) {
            var master_priotrity_code = $scope.masterModels.PRIORITY_CODE;
            if (master_priotrity_code == null || master_priotrity_code == "" || master_priotrity_code == undefined) {
                displayPopupNotification("Priority Code is required", "warning");
                return $scope.loadingBtnReset();
            }

            else {
                $(".mprority").removeClass("borderRed");
            }
        };
        if ($scope.masterModels.hasOwnProperty("DIVISION_CODE")) {
            var master_division_code = $scope.masterModels.DIVISION_CODE;
            if (master_division_code == null || master_division_code == "" || master_division_code == undefined) {
                displayPopupNotification("Division Code is required", "warning");
                return $scope.loadingBtnReset();
            }

            else {
                $(".mdivision").removeClass("borderRed");
            }
        };
        if ($scope.masterModels.hasOwnProperty("EMPLOYEE_CODE")) {
            var master_employee_code = $scope.masterModels.EMPLOYEE_CODE;
            if (master_employee_code == null || master_employee_code == "" || master_employee_code == undefined) {
                displayPopupNotification("Employee Code is required", "warning");
                return $scope.loadingBtnReset();
            }
            else {
                $(".memployee").removeClass("borderRed");
            }
        };
        if ($scope.masterModels.hasOwnProperty("SALES_TYPE_CODE")) {
            var master_salestype = $scope.masterModels.SALES_TYPE_CODE;
            if (master_salestype == null || master_salestype == "" || master_salestype == undefined) {
                displayPopupNotification("Sales Type Code is required", "warning");
                return $scope.loadingBtnReset();
            }
            else {
                $(".msalestype").removeClass("borderRed");
            }
        };
        //if ($scope.masterModels.hasOwnProperty(GetPrimaryDateByTableName($scope.DocumentName))) {

        //    var bd = $scope.formBackDays;
        //    var primarydatecolname = GetPrimaryDateByTableName($scope.DocumentName);
        //    var master_orderdate = $scope.masterModels[primarydatecolname];
        //    master_orderdate = new Date(master_orderdate);
        //    var CurrentDate = new Date();
        //    var today = new Date();

        //    var days_ago = new Date().setDate(today.getDate() - $scope.formBackDays);
        //    var nd = new Date(days_ago);
        //    if (nd >= master_orderdate) {
        //        displayPopupNotification("Date should not be less than" + bd+ " from today date", "warning");
        //        return $scope.loadingBtnReset();
        //    }
        //};
        if ($scope.masterModels.hasOwnProperty("DELIVERY_DATE")) {
            var master_deliverydate = $scope.masterModels.DELIVERY_DATE;
            master_deliverydate = new Date(master_deliverydate);
            if ($scope.masterModels.hasOwnProperty(GetPrimaryDateByTableName($scope.DocumentName))) {

                var bd = $scope.formBackDays;
                var primarydatecolname = GetPrimaryDateByTableName($scope.DocumentName);
                var master_orderdate = $scope.masterModels[primarydatecolname];
                master_orderdate = new Date(master_orderdate);
            }
            if (master_deliverydate < master_orderdate) {
                displayPopupNotification("Delivery Date should not be less than today date", "warning");
                return $scope.loadingBtnReset();
            }
        };
        if ($scope.masterModels.hasOwnProperty("SHIPPING_CONTACT_NO")) {
            var manual_no = $scope.masterModels.MANUAL_NO;
            if (/^[a-zA-Z0-9- ]*$/.test(manual_no) == false) {
                displayPopupNotification("Shipping Contact No shouldnot contain special characters", "warning");
                return $scope.loadingBtnReset();
            }
        };
        var childlen = $scope.childModels.length;
        for (var i = 0; i < childlen; i++) {
            if ($scope.childModels[0].hasOwnProperty("FROM_LOCATION_CODE")) {
                var child_from_location = $scope.childModels[i].FROM_LOCATION_CODE;
                if ($(".clocation").parent().parent().hasClass("borderRed") || child_from_location == null || child_from_location == "" || child_from_location == undefined) {
                    displayPopupNotification("Location Code is required", "warning");
                    return $scope.loadingBtnReset();
                }
            };
            if ($scope.childModels[0].hasOwnProperty("ITEM_CODE")) {
                var child_item = $scope.childModels[i].ITEM_CODE;
                if (child_item == null || child_item == "" || child_item == undefined) {
                    $(".cproducts").parent().parent().hasClass("borderRed")
                    displayPopupNotification("Product Code is required", "warning");
                    return $scope.loadingBtnReset();
                }
                else {
                    $(".cproducts").parent().parent().addClass("borderRed")
                }
            };

            if ($scope.childModels[0].hasOwnProperty("QUANTITY")) {
                var child_quanity = $scope.childModels[i].QUANTITY;
                if (child_quanity === null || child_quanity === "" || child_quanity === 0) {
                    displayPopupNotification("Quantity is required", "warning");
                    return $scope.loadingBtnReset();
                }
                if (child_quanity === undefined) {
                    displayPopupNotification("Enter Quantity Correctly", "error");
                    return $scope.loadingBtnReset();
                }
            }
            //if ($scope.childModels[0].hasOwnProperty("PURITY")) {
            //    var child_purity = $scope.childModels[i].PURITY;
            //    if (child_purity === null || child_purity === "" || child_purity === 0) {
            //        displayPopupNotification("Purity is required", "warning");
            //        return $scope.loadingBtnReset();
            //    }
            //    if (child_quanity === undefined) {
            //        displayPopupNotification("Enter Purity Correctly", "error");
            //        return $scope.loadingBtnReset();
            //    }
            //}
            if ($scope.childModels[0].hasOwnProperty("CALC_QUANTITY")) {
                var calc_quanity = $scope.childModels[i].CALC_QUANTITY;
                if (calc_quanity === null || calc_quanity === "" || calc_quanity === 0) {
                    displayPopupNotification("Calculated Quantity is required", "warning");
                    return $scope.loadingBtnReset();
                }
                if (calc_quanity === undefined) {
                    displayPopupNotification("Enter Calculated Quantity Correctly", "error");
                    return $scope.loadingBtnReset();
                }
            }


            if ($scope.childModels[0].hasOwnProperty("TOTAL_WEIGHT")) {
                var totalWeight = $scope.childModels[i].TOTAL_WEIGHT;
                if (totalWeight === null || totalWeight === "" || totalWeight === 0) {
                    displayPopupNotification("Total Wt. is required", "warning");
                    return $scope.loadingBtnReset();
                }
                if (totalWeight === undefined) {
                    displayPopupNotification("Enter Total Wt. Correctly", "error");
                    return $scope.loadingBtnReset();
                }
            }


            if ($scope.childModels[0].hasOwnProperty("UNIT_PRICE")) {
                var child_rate = $scope.childModels[i].UNIT_PRICE;
                if (child_rate === null || child_rate === "") {
                    displayPopupNotification("Rate/Unit is required", "warning");
                    return $scope.loadingBtnReset();
                }
                if (child_rate === undefined) {
                    displayPopupNotification("Enter Amount Correctly", "error");
                    return $scope.loadingBtnReset();
                }
            };
            if ($scope.childModels[0].hasOwnProperty("CALC_UNIT_PRICE")) {
                var calc_rate = $scope.childModels[i].UNIT_PRICE;
                if (calc_rate === null || calc_rate === "") {
                    displayPopupNotification("Calcutaled Rate/Unit is required", "warning");
                    return $scope.loadingBtnReset();
                }
                if (calc_rate === undefined) {
                    displayPopupNotification("Enter Calcutaled Amount Correctly", "error");
                    return $scope.loadingBtnReset();
                }
            };
        };


        //check if Grand Total Is 0.00

        if ($scope.summary.grandTotal == 0) {
            displayPopupNotification("Grand Total cannot be 0.00", "warning");
            return $scope.loadingBtnReset();
        }

        $scope.SERIAL_TRACKING_VALUE = [];
        var serialtrackingdata = $.grep($scope.checkedresult, function (e) {

            return e.ITEM_CODE != 0;
        });
        if (serialtrackingdata.length > 0) {
            $scope.SERIAL_TRACKING_VALUE = serialtrackingdata;
        }
        else {
            $scope.SERIAL_TRACKING_VALUE = [];
        }
        var batchTranData = $.grep($scope.checkedBatchTranResult, function (e) {
            return e.ITEM_CODE != 0;
        });
        if (batchTranData.length > 0) {
            $scope.SERIAL_TRACKING_VALUE = batchTranData;
        }
        else {
            $scope.SERIAL_TRACKING_VALUE = [];
        }
        //Validation Ends

        var saveflag = param;
        $scope.buttonflag = param;
        var formcode = $scope.FormCode;
        var tablename = $scope.DocumentName;
        var orderno = $scope.OrderNo;
        var masterelementvalues = $scope.masterModels;
        //var grandtotal = $scope.summary.grandTotal;
        var grandtotal = $scope.adtotal;

        var masterElementJson = {};
        var primarycolumnname = PrimaryColumnForTable(tablename);
        var fromReference = $scope.showRefTab;
        $.each($scope.MasterFormElement, function (key, value) {
            if (value['COLUMN_NAME'].indexOf('DATE') > -1) {
                var date = $scope.masterModels[value.COLUMN_NAME];
                $scope.masterModels[value.COLUMN_NAME] = moment(date).format('DD-MMM-YYYY');
            }
        });
        var childcolumnkeys = "";
        $scope.childModels = JSON.parse(angular.toJson($scope.childModels));
        for (key in $scope.childModels[0]) {
            childcolumnkeys += key + ",";
        }
        //$rootScope.refCheckedItem["REF_FORM_CODE"] = $scope.ref_form_code;
        $scope.INV_ITEM_CHARGE_VALUE_ARRAY = [];
        if (($scope.dynamicInvItenChargeModalData[0].ITEM_CODE) !== 0 && $scope.dynamicInvItenChargeModalData[0].INV_ITEM_CHARGE_AMOUNT_WISE[0].VALUE_PERCENT_FLAG !== "") {

            $scope.INV_ITEM_CHARGE_VALUE_ARRAY = angular.toJson($scope.dynamicInvItenChargeModalData);
        }

        else {

            $scope.INV_ITEM_CHARGE_VALUE_ARRAY = [];
        }
        $scope.SDModel.TRANSPORT_INVOICE_DATE = moment($("#TransportInvoiceDate").val()).format('DD-MMM-YYYY');
        $scope.SDModel.DELIVERY_INVOICE_DATE = moment($("#DeliveryDate").val()).format('DD-MMM-YYYY');
        $scope.SDModel.GATE_ENTRY_DATE = moment($("#GateEntryDate").val()).format('DD-MMM-YYYY');
        $scope.SDModel.WB_DATE = moment($("#WeighBridgeDate").val()).format('DD-MMM-YYYY');



        var model = {
            Save_Flag: saveflag,
            Table_Name: tablename,
            Form_Code: formcode,
            Order_No: orderno,
            Grand_Total: grandtotal,
            FROM_REF: fromReference,
            REF_MODEL: $rootScope.refCheckedItem,
            //Master_COLUMN_VALUE: JSON.stringify(masterElementJson),
            Master_COLUMN_VALUE: JSON.stringify($scope.masterModels),
            Child_COLUMNS: childcolumnkeys,
            Child_COLUMN_VALUE: JSON.stringify($scope.childModels),
            PRIMARY_COL_NAME: primarycolumnname,
            Custom_COLUMN_VALUE: JSON.stringify($scope.customModels),
            CHARGES: JSON.stringify($scope.data),
            //INV_ITEM_CHARGE_VALUE: angular.toJson($scope.dynamicInvItenChargeModalData),
            INV_ITEM_CHARGE_VALUE: $scope.INV_ITEM_CHARGE_VALUE_ARRAY,
            SHIPPING_DETAILS_VALUE: JSON.stringify($scope.SDModel),
            SERIAL_TRACKING_VALUE: JSON.stringify($scope.SERIAL_TRACKING_VALUE)
        };
        debugger;
        var staturl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/SaveFormData";
        var response = $http({
            method: "POST",
            data: model,
            url: staturl,
            contentType: "application/json",
            dataType: "json"
        });
        return response.then(function (data) {
            debugger;
            // or it might be: response.data.data.MESSAGE
            var message = data.data.MESSAGE;

            // ADD THIS LINE: Store the message in scope
            $scope.receiptMessage = message;
            if (data.data.MESSAGE == "INSERTED") {

                //displayPopupNotification("Data succesfully saved ", "success");
                //var generateMsg = "Voucher Saved Successfully! </br> Voucher no: " + data.data.VoucherNo;
                //$scope.showSimplePopup("Voucher Saved Successfully! </br> Voucher No: " + data.data.VoucherNo);

                $scope.dzvouchernumber = data.data.VoucherNo;
                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;
                if ($scope.formSetup[0].RECEIPT_FLAG === 'Y') {
                    $scope.showSimplePopup("Voucher Saved Successfully! </br> Voucher No: " + data.data.VoucherNo, function () {
                        // Close popup and open CashVoucherModal
                        /*     $('#CashVoucherModal').modal('show');*/
                        checkReceiptFlag(message);
                    });
                } else {
                    $scope.showSimplePopup("Voucher Saved Successfully! </br> Voucher No: " + data.data.VoucherNo, function () {
                        location.reload();

                        var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Template/SplitterIndex#!DT/MenuSplitter/04";
                        $window.location.href = landingUrl;
                    });
                }

                //DisplayBarNotificationMessage(generateMsg);
                mySalesDropzone.processQueue();
                var host = $window.location.host;
                //var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Template/SplitterIndex#!DT/MenuSplitter/04";
                //setTimeout(function () {
                //    $window.location.href = landingUrl;
                //}, 1000);

            }
            else if (data.data.MESSAGE == "SAVEANDPRINT") {
                debugger;
                if ($scope.OrderNo === "undefined") {
                    $scope.OrderNo = data.data.VoucherNo;
                }
                if ($scope.DocumentName === "SA_SALES_INVOICE_JEWEL" || $scope.DocumentName === "SA_SALES_RETURN_JEWEL") {


                    var generateMsg = "Voucher Saved Successfully! </br> Voucher no: " + data.data.VoucherNo;
                    /*DisplayBarNotificationMessage(generateMsg);*/

                    $.notifyBar({ html: generateMsg, close: true, waitingForClose: true, closeOnClick: true, cssClass: "success" });

                    setTimeout(function () {
                        $(".notify-bar-close").trigger('click');
                    }, 2000);


                    $scope.PrintDocument();

                    $scope.childModels = JSON.parse(JSON.stringify($scope.childModels));

                }
                //$scope.PrintDocument();
                else {
                    var masterelem = $scope.MasterFormElement;
                    //$.each($scope.MasterFormElement, function (key, value) {

                    //    if (value['COLUMN_NAME'].indexOf('CODE') > -1) {

                    //        var switched;
                    //        switched = value['COLUMN_NAME'];
                    //        switch (switched) {
                    //            case 'SUPPLIER_CODE':
                    //                $scope.masterModels["SUPPLIER_CODE"] = $('#supplier').data("kendoComboBox").dataItem().SUPPLIER_EDESC;
                    //                break;
                    //            case 'ISSUE_TYPE_CODE':
                    //                $scope.masterModels["ISSUE_TYPE_CODE"] = $('#issuetype').data("kendoComboBox").dataItem().ISSUE_TYPE_EDESC;
                    //                break;
                    //            case 'TO_BRANCH_CODE':
                    //                $scope.masterModels["TO_BRANCH_CODE"] = $('#branchcode').data("kendoComboBox").dataItem().BRANCH_EDESC;
                    //                break;

                    //            case "TO_LOCATION_CODE":
                    //                $scope.masterModels["TO_LOCATION_CODE"] = $('#tolocation').data("kendoComboBox").dataItem().LocationName;
                    //                break;
                    //            case "FROM_LOCATION_CODE":
                    //                $scope.masterModels["FROM_LOCATION_CODE"] = $('#location').data("kendoComboBox").dataItem().LocationName;
                    //                break;
                    //            case "MASTER_ACC_CODE":
                    //                $scope.masterModels["MASTER_ACC_CODE"] = $('#masteracccode').data("kendoComboBox").dataItem().ACC_EDESC;
                    //                break;
                    //            case "CUSTOMER_CODE":
                    //                $scope.masterModels["CUSTOMER_CODE"] = $('#customers').data("kendoComboBox").dataItem().CustomerName;
                    //                $scope.masterModels["REGD_OFFICE_EADDRESS"] = $('#customers').data("kendoComboBox").dataItem().REGD_OFFICE_EADDRESS;
                    //                $scope.masterModels["TPIN_VAT_NO"] = $('#customers').data("kendoComboBox").dataItem().TPIN_VAT_NO;
                    //                $scope.masterModels["TEL_MOBILE_NO1"] = $('#customers').data("kendoComboBox").dataItem().TEL_MOBILE_NO1;
                    //                break;
                    //            default:
                    //        }

                    //    }
                    //});
                    //var masterArr = $scope.ChildFormElement[0].element;
                    //var print_master = $.grep(masterArr, function (e) {
                    //    return (e['COLUMN_NAME'].indexOf("CALC") === -1 && e['COLUMN_NAME'].indexOf("REMARKS") === -1);
                    //});
                    //var print_child = [];
                    //$.each($scope.ChildFormElement, function (ind, it) {
                    //    print_child.push({
                    //        element: $.grep(it.element, function (e) {
                    //            var switch_on;
                    //            switch_on = e['COLUMN_NAME'];
                    //            switch (switch_on) {
                    //                case 'ITEM_CODE':
                    //                    $scope.childModels[ind]["ITEM_CODE"] = $($(".cproduct_" + ind)[$(".cproduct_" + ind).length - 1]).data("kendoComboBox").dataItem().ItemDescription;
                    //                    break;
                    //                case 'PRODUCT_CODE':
                    //                    $scope.childModels[ind]["ITEM_CODE"] = $($(".cproduct_" + ind)[$(".cproduct_" + ind).length - 1]).data("kendoComboBox").dataItem().ItemDescription;
                    //                    break;
                    //                case 'ACC_CODE':
                    //                    $scope.childModels[ind]["ACC_CODE"] = $($(".caccount_" + ind)[$(".caccount_" + ind).length - 1]).data("kendoComboBox").dataItem().ACC_EDESC;
                    //                    break;

                    //                case "TO_LOCATION_CODE":
                    //                    $scope.childModels[ind]["TO_LOCATION_CODE"] = $($(".ctolocation_" + ind)[$(".ctolocation_" + ind).length - 1]).data("kendoComboBox").dataItem().LocationNames;
                    //                    break;
                    //                case "FROM_LOCATION_CODE":
                    //                    $scope.childModels[ind]["FROM_LOCATION_CODE"] = $($(".clocation_" + ind)[$(".clocation_" + ind).length - 1]).data("kendoComboBox").dataItem().LocationName;
                    //                    break;
                    //                default:
                    //            }
                    //            return (e['COLUMN_NAME'].indexOf("CALC") === -1 && e['COLUMN_NAME'].indexOf("REMARKS") === -1);
                    //        }),
                    //        additionalElements: ''
                    //    });
                    //});

                    //for (var i = 0; i < $scope.childModels.length; i++) {
                    //    if ("FROM_LOCATION_CODE" in $scope.childModels[i]) {
                    //        $scope.childModels[i]["FROM_LOCATION_CODE"] = undefined;
                    //    }
                    //    if ("CALC_QUANTITY" in $scope.childModels[i]) {
                    //        $scope.childModels[i]["CALC_QUANTITY"] = undefined;
                    //    }
                    //    if ("CALC_TOTAL_PRICE" in $scope.childModels[i]) {
                    //        $scope.childModels[i]["CALC_TOTAL_PRICE"] = undefined;
                    //    }
                    //    if ("CALC_UNIT_PRICE" in $scope.childModels[i]) {
                    //        $scope.childModels[i]["CALC_UNIT_PRICE"] = undefined;
                    //    }
                    //    if ("STOCK_BLOCK_FLAG" in $scope.childModels[i]) {
                    //        $scope.childModels[i]["STOCK_BLOCK_FLAG"] = undefined;
                    //    }
                    //    if ("COMPLETED_QUANTITY" in $scope.childModels[i]) {
                    //        $scope.childModels[i]["COMPLETED_QUANTITY"] = undefined;
                    //    }

                    //};

                    //$scope.childModels = JSON.parse(JSON.stringify($scope.childModels));

                    //$scope.print_header = print_master;
                    //$scope.print_body_col = print_child;
                    //for (var i = 0; i < $scope.print_body_col.length; i++) {

                    //    $scope.print_body_col[i].element = $.grep($scope.print_body_col[i].element, function (v) { return ($.inArray(v.COLUMN_NAME, ["FROM_LOCATION_CODE", "STOCK_BLOCK_FLAG", "COMPLETED_QUANTITY"]) == -1) });

                    //}
                    ////for (var i = 0; i < $scope.print_body_col.length; i++) {
                    ////    $scope.print_body_col[i].element = $.grep($scope.print_body_col[i].element, function (v) { return (v) });
                    ////}
                    //$scope.print_body_col = JSON.parse(JSON.stringify($scope.print_body_col));

                    $scope.dzvouchernumber = data.data.VoucherNo;
                    $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                    $scope.dzformcode = data.data.FormCode;

                    //displayPopupNotification("Data succesfully Saved.", "success");



                    if ($scope.formSetup[0].RECEIPT_FLAG === 'Y') {
                        $scope.showSimplePopup("Voucher Saved Successfully! </br> Voucher No: " + data.data.VoucherNo, function () {
                            // Close popup and open CashVoucherModal
                            /*     $('#CashVoucherModal').modal('show');*/

                            var url = '/Print/Home/PreviewPattern?formCode=' + $scope.dzformcode + '&companyCode=' + $scope.companycode + '&mainFieldValue=' + encodeURIComponent(data.data.VoucherNo);
                            window.open(url, '_blank');
                            checkReceiptFlag(message);
                        });
                    } else {

                        $scope.showSimplePopup("Voucher Saved Successfully! </br> Voucher No: " + data.data.VoucherNo, function () {

                            var url = '/Print/Home/PreviewPattern?formCode=' + $scope.dzformcode + '&companyCode=' + $scope.companycode + '&mainFieldValue=' + encodeURIComponent(data.data.VoucherNo);
                            window.open(url, '_blank');
                            location.reload();

                            var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Home/Index#!DT/formtemplates/" + $scope.dzformcode;
                            $window.location.href = landingUrl;
                        });
                    }

                    $scope.TodayDate = AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD'))
                    //$scope.amountinword = convertNumberToWords($scope.summary.grandTotal);
                    $scope.amountinword = convertNumberToWords($scope.adtotal);
                    $scope.printcount = 1;
                    $scope.printcountdiv = "Original";
                    //if ($scope.PrintPathUl == "" || $scope.PrintPathUl == undefined || $scope.PrintPathUl == "undefined") {
                    //    displayPopupNotification("Please perform print teplate setup first.", "warning");
                    //}
                    //else {
                    //    $("#saveAndPrintModal").modal("toggle");
                    //}


                }

            }
            else if (data.data.MESSAGE == "UPDATEDANDPRINT") {
                if ($scope.OrderNo === "undefined") {
                    $scope.OrderNo = data.data.VoucherNo;
                }
                //$scope.PrintDocument();


                //var masterelem = $scope.MasterFormElement;

                //$.each($scope.MasterFormElement, function (key, value) {

                //    if (value['COLUMN_NAME'].indexOf('CODE') > -1) {

                //        var switched;
                //        switched = value['COLUMN_NAME'];
                //        switch (switched) {
                //            case 'SUPPLIER_CODE':
                //                $scope.masterModels["SUPPLIER_CODE"] = $('#supplier').data("kendoComboBox").dataItem().SUPPLIER_EDESC;
                //                break;
                //            case 'ISSUE_TYPE_CODE':
                //                $scope.masterModels["ISSUE_TYPE_CODE"] = $('#issuetype').data("kendoComboBox").dataItem().ISSUE_TYPE_EDESC;
                //                break;
                //            case 'TO_BRANCH_CODE':
                //                $scope.masterModels["TO_BRANCH_CODE"] = $('#branchcode').data("kendoComboBox").dataItem().BRANCH_EDESC;
                //                break;

                //            case "TO_LOCATION_CODE":
                //                $scope.masterModels["TO_LOCATION_CODE"] = $('#tolocation').data("kendoComboBox").dataItem().LocationName;
                //                break;
                //            case "FROM_LOCATION_CODE":
                //                $scope.masterModels["FROM_LOCATION_CODE"] = $('#location').data("kendoComboBox").dataItem().LocationName;
                //                break;
                //            case "MASTER_ACC_CODE":
                //                $scope.masterModels["MASTER_ACC_CODE"] = $('#masteracccode').data("kendoComboBox").dataItem().ACC_EDESC;
                //                break;
                //            case "CUSTOMER_CODE":
                //                $scope.masterModels["CUSTOMER_CODE"] = $('#customers').data("kendoComboBox").dataItem().CustomerName;
                //                $scope.masterModels["REGD_OFFICE_EADDRESS"] = $('#customers').data("kendoComboBox").dataItem().REGD_OFFICE_EADDRESS;
                //                $scope.masterModels["TPIN_VAT_NO"] = $('#customers').data("kendoComboBox").dataItem().TPIN_VAT_NO;
                //                $scope.masterModels["TEL_MOBILE_NO1"] = $('#customers').data("kendoComboBox").dataItem().TEL_MOBILE_NO1;
                //                break;
                //            default:
                //        }

                //    }
                //});
                //var masterArr = $scope.ChildFormElement[0].element;
                //var print_master = $.grep(masterArr, function (e) {
                //    return (e['COLUMN_NAME'].indexOf("CALC") === -1 && e['COLUMN_NAME'].indexOf("REMARKS") === -1);
                //});
                //var print_child = [];
                //$.each($scope.ChildFormElement, function (ind, it) {

                //    print_child.push({
                //        element: $.grep(it.element, function (e) {
                //            var switch_on;
                //            switch_on = e['COLUMN_NAME'];
                //            switch (switch_on) {
                //                case 'ITEM_CODE':
                //                    $scope.childModels[ind]["ITEM_CODE"] = $($(".cproduct_" + ind)[$(".cproduct_" + ind).length - 1]).data("kendoComboBox").dataItem().ItemDescription;
                //                    break;
                //                case 'PRODUCT_CODE':
                //                    $scope.childModels[ind]["ITEM_CODE"] = $($(".cproduct_" + ind)[$(".cproduct_" + ind).length - 1]).data("kendoComboBox").dataItem().ItemDescription;
                //                    break;
                //                case 'ACC_CODE':
                //                    $scope.childModels[ind]["ACC_CODE"] = $($(".caccount_" + ind)[$(".caccount_" + ind).length - 1]).data("kendoComboBox").dataItem().ACC_EDESC;
                //                    break;

                //                case "TO_LOCATION_CODE":
                //                    $scope.childModels[ind]["TO_LOCATION_CODE"] = $($(".ctolocation_" + ind)[$(".ctolocation_" + ind).length - 1]).data("kendoComboBox").dataItem().LocationNames;
                //                    break;
                //                case "FROM_LOCATION_CODE":
                //                    $scope.childModels[ind]["FROM_LOCATION_CODE"] = $($(".clocation_" + ind)[$(".clocation_" + ind).length - 1]).data("kendoComboBox").dataItem().LocationName;
                //                    break;
                //                default:
                //            }
                //            return (e['COLUMN_NAME'].indexOf("CALC") === -1 && e['COLUMN_NAME'].indexOf("REMARKS") === -1);
                //        }),
                //        additionalElements: ''
                //    });
                //});

                //for (var i = 0; i < $scope.childModels.length; i++) {

                //    if ("FROM_LOCATION_CODE" in $scope.childModels[i]) {
                //        $scope.childModels[i]["FROM_LOCATION_CODE"] = undefined;
                //    }
                //    if ("CALC_QUANTITY" in $scope.childModels[i]) {
                //        $scope.childModels[i]["CALC_QUANTITY"] = undefined;
                //    }
                //    if ("CALC_TOTAL_PRICE" in $scope.childModels[i]) {
                //        $scope.childModels[i]["CALC_TOTAL_PRICE"] = undefined;
                //    }
                //    if ("CALC_UNIT_PRICE" in $scope.childModels[i]) {
                //        $scope.childModels[i]["CALC_UNIT_PRICE"] = undefined;
                //    }
                //    if ("STOCK_BLOCK_FLAG" in $scope.childModels[i]) {
                //        $scope.childModels[i]["STOCK_BLOCK_FLAG"] = undefined;
                //    }
                //    if ("COMPLETED_QUANTITY" in $scope.childModels[i]) {
                //        $scope.childModels[i]["COMPLETED_QUANTITY"] = undefined;
                //    }

                //};

                //$scope.childModels = JSON.parse(JSON.stringify($scope.childModels));

                //$scope.print_header = print_master;
                //$scope.print_body_col = print_child;

                //for (var i = 0; i < $scope.print_body_col.length; i++) {

                //    $scope.print_body_col[i].element = $.grep($scope.print_body_col[i].element, function (v) { return ($.inArray(v.COLUMN_NAME, ["FROM_LOCATION_CODE", "STOCK_BLOCK_FLAG", "COMPLETED_QUANTITY"]) == -1) });

                //}

                ////for (var i = 0; i < $scope.print_body_col.length; i++) {
                ////    $scope.print_body_col[i].element = $.grep($scope.print_body_col[i].element, function (v) { return (v) });
                ////}
                //$scope.print_body_col = JSON.parse(JSON.stringify($scope.print_body_col));

                $scope.dzvouchernumber = data.data.VoucherNo;
                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;
                /*  displayPopupNotification("Data succesfully Updated.", "success");*/

                //var url = '/Print/Home/PreviewPattern?formCode=' + $scope.dzformcode + '&companyCode=' + $scope.companycode + '&mainFieldValue=' + encodeURIComponent(data.data.VoucherNo);
                //window.open(url, '_blank');

                if ($scope.formSetup[0].RECEIPT_FLAG === 'Y') {
                    $scope.showSimplePopup("Voucher Updated Successfully! </br> Voucher No: " + data.data.VoucherNo, function () {
                        // Close popup and open CashVoucherModal
                        /*     $('#CashVoucherModal').modal('show');*/

                        var url = '/Print/Home/PreviewPattern?formCode=' + $scope.dzformcode + '&companyCode=' + $scope.companycode + '&mainFieldValue=' + encodeURIComponent(data.data.VoucherNo);
                        window.open(url, '_blank');
                        checkReceiptFlag(message);
                    });
                } else {
                    $scope.showSimplePopup("Voucher Updated Successfully! </br> Voucher No: " + data.data.VoucherNo, function () {

                        var url = '/Print/Home/PreviewPattern?formCode=' + $scope.dzformcode + '&companyCode=' + $scope.companycode + '&mainFieldValue=' + encodeURIComponent(data.data.VoucherNo);
                        window.open(url, '_blank');
                        location.reload();

                        var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Home/Index#!DT/formtemplates/" + $scope.dzformcode;
                        $window.location.href = landingUrl;
                    });
                }

                $scope.TodayDate = AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD'));
                $scope.amountinword = convertNumberToWords($scope.summary.grandTotal);
                $scope.printcountdiv = "Copy Of Original";
                //if ($scope.PrintPathUl == "" || $scope.PrintPathUl == undefined || $scope.PrintPathUl == "undefined") {
                //    displayPopupNotification("Please perform print teplate setup first.", "warning");
                //}
                //else {
                //    $("#saveAndPrintModal").modal("toggle");
                //}


            }
            else if (data.data.MESSAGE == "INSERTEDANDCONTINUE") {

                //var generateMsg = "Voucher Saved Successfully! </br> Voucher no: " + data.data.VoucherNo;
                //DisplayBarNotificationMessage(generateMsg);


                if ($scope.formSetup[0].RECEIPT_FLAG === 'Y') {
                    $scope.showSimplePopup("Voucher Saved Successfully! </br> Voucher No: " + data.data.VoucherNo, function () {
                        // Close popup and open CashVoucherModal
                        /*     $('#CashVoucherModal').modal('show');*/

                        //Call Cash Voucher
                        checkReceiptFlag(message);
                        //$scope.resetFormAfterSave();
                    });
                } else {
                    $scope.showSimplePopup("Voucher Saved Successfully! </br> Voucher No: " + data.data.VoucherNo);
                    $scope.resetFormAfterSave();
                }

                //    $scope.ChargeList = [];

                $scope.dzvouchernumber = data.data.VoucherNo;
                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;

                //$scope.masterChildData = [];


                //mySalesDropzone.processQueue();
                //angular.forEach($scope.MasterFormElement, function (value, key) {
                //    if (value.MASTER_CHILD_FLAG == 'M' && value.DISPLAY_FLAG == 'Y') {
                //        $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                //        $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                //        if (value['COLUMN_NAME'].indexOf('DATE') > -1) {
                //            $scope.masterModels[value['COLUMN_NAME']] = $filter('date')(new Date(), 'dd-MMM-yyyy');
                //        }
                //    }
                //});

                //var cml = $scope.childModels.length;
                //var sl = parseFloat(cml) - 1;
                //$scope.ChildFormElement.splice(0, sl);
                //$scope.childModels.splice(0, sl);
                //angular.forEach($scope.ChildFormElement[0].element, function (value, key) {
                //    if (value.MASTER_CHILD_FLAG == 'C' && value.DISPLAY_FLAG == 'Y') {
                //        $scope.childModels[0][value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
                //    }
                //});
                //var req = "/api/TemplateApi/GetNewOrderNo?companycode=" + $scope.companycode + "&formcode=" + $scope.FormCode + "&currentdate=" + $scope.todaydate + "&tablename=" + $scope.DocumentName + '&isSequence=false';
                //$http.get(req).then(function (results) {

                //    $scope.newgenorderno = results.data;
                //    var primarycolumnname = PrimaryColumnForTable($scope.DocumentName);
                //    $scope.someDateFn();
                //    $scope.masterModels[primarycolumnname] = results.data;
                //    $scope.masterModelTemplate[primarycolumnname] = results.data;
                //});
                //$scope.summary.grandTotal = 0;

                //$scope.summary = { 'grandTotal': 0 };
                //$scope.units = [];
                //$scope.totalQty = 0;
                /////reset initial charge data
                ////$scope.ChargeList = $scope.Initialchargedata;

                //// Reset $scope.data and re-add active charges BEFORE setTimeout
                //$scope.data = [];
                //angular.forEach($scope.ChargeList, function (charge) {
                //    if (charge.CHARGE_ACTIVE_FLAG === 'Y') {
                //        $scope.data.push(angular.copy(charge));
                //    }
                //    charge.CHARGE_AMOUNT = 0.0;
                //});

                //setTimeout(function () {

                //    // Uncheck all checkboxes EXCEPT charge checkboxes
                //    $('input[type="checkbox"]').not('[ng-click*="sync"]').prop('checked', false);

                //    if ($('#dealercode').data("kendoComboBox") != undefined) {
                //        if ($('#dealercode').data("kendoComboBox").dataItem() != undefined) {
                //            $('#dealercode').data("kendoComboBox").value([]);
                //        }
                //    }

                //    // Trigger Angular digest to update charge checkboxes
                //    $scope.$applyAsync();

                //}, 100);


                //$scope.adtotal = 0.00;
                //$scope.addition = 0;
                //$scope.deduction = 0;
            }
            else if (data.data.MESSAGE == "UPDATED") {
                debugger;
                //DisplayBarNotificationMessage("Data succesfully updated.", "success");
                //$scope.dzvouchernumber = data.data.VoucherNo;


                if ($scope.formSetup[0].RECEIPT_FLAG === 'Y') {
                    $scope.showSimplePopup("Voucher Updated Successfully! </br> Voucher No: " + data.data.VoucherNo, function () {
                        // Close popup and open CashVoucherModal
                        /*     $('#CashVoucherModal').modal('show');*/
                        checkReceiptFlag(message);
                    });
                } else {
                    $scope.showSimplePopup("Voucher Updated Successfully! </br> Voucher No: " + data.data.VoucherNo);
                }

                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;
                mySalesDropzone.processQueue();
                var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Template/SplitterIndex#!DT/MenuSplitter/04";
                //$window.location.href = landingUrl;
                setTimeout(function () {
                    $window.location.href = landingUrl;
                }, 1000);
            }
            else {
                displayPopupNotification("Something went wrong!.Please try again.", "error");
            }

            $("#refrenceTypeMultiSelect").data('kendoDropDownList').value('');
            $('#referenceGrid').empty();

            $scope.loadingBtnReset();

        },
            function errorCallback(response) {
                displayPopupNotification("Something went wrong!.Please try again.", "error");
                $scope.loadingBtnReset();
            });

    };


    $scope.showSimplePopup = function (message, callback) {
        // Remove if already exists
        document.getElementById('simple-popup')?.remove();

        // Create popup container
        var popup = document.createElement('div');
        popup.id = 'simple-popup';
        popup.innerHTML = `
        <div style="
            position: fixed; top: 0; left: 0; width: 100%; height: 100%;
            background: rgba(0, 0, 0, 0.4); display: flex; align-items: center; justify-content: center;
            z-index: 9999;">
            <div style="
                background: white; padding: 20px 30px; border-radius: 8px; min-width: 300px;
                text-align: center; box-shadow: 0 4px 10px rgba(0,0,0,0.2);">
                <h4 style="margin-top:0">Success</h4>
                <p>${message}</p>
                <button id="simple-popup-ok-btn" style="
                    margin-top: 15px; padding: 6px 16px; background: #4CAF50; color: white;
                    border: none; border-radius: 4px; cursor: pointer;">OK</button>
            </div>
        </div>
    `;
        document.body.appendChild(popup);

        // Add click event listener to OK button
        document.getElementById('simple-popup-ok-btn').addEventListener('click', function () {
            document.getElementById('simple-popup').remove();
            if (callback && typeof callback === 'function') {
                callback();
            }
        });
    };

    $scope.resetFormAfterSave = function () {

        $scope.masterChildData = [];

        mySalesDropzone.processQueue();

        angular.forEach($scope.MasterFormElement, function (value) {
            if (value.MASTER_CHILD_FLAG == 'M' && value.DISPLAY_FLAG == 'Y') {

                $scope.masterModels[value.COLUMN_NAME] =
                    value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;

                if (value.COLUMN_NAME.indexOf('DATE') > -1) {
                    $scope.masterModels[value.COLUMN_NAME] =
                        $filter('date')(new Date(), 'dd-MMM-yyyy');
                }
            }
        });

        var cml = $scope.childModels.length;
        var sl = parseFloat(cml) - 1;

        $scope.ChildFormElement.splice(0, sl);
        $scope.childModels.splice(0, sl);

        angular.forEach($scope.ChildFormElement[0].element, function (value) {
            if (value.MASTER_CHILD_FLAG == 'C' && value.DISPLAY_FLAG == 'Y') {
                $scope.childModels[0][value.COLUMN_NAME] =
                    value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
            }
        });

        var req = "/api/TemplateApi/GetNewOrderNo?companycode=" +
            $scope.companycode +
            "&formcode=" + $scope.FormCode +
            "&currentdate=" + $scope.todaydate +
            "&tablename=" + $scope.DocumentName +
            "&isSequence=false";

        $http.get(req).then(function (results) {

            var primarycolumnname = PrimaryColumnForTable($scope.DocumentName);

            $scope.newgenorderno = results.data;
            $scope.someDateFn();

            $scope.masterModels[primarycolumnname] = results.data;
            $scope.masterModelTemplate[primarycolumnname] = results.data;
        });

        $scope.summary = { grandTotal: 0 };
        $scope.units = [];
        $scope.totalQty = 0;

        $scope.data = [];

        angular.forEach($scope.ChargeList, function (charge) {
            if (charge.CHARGE_ACTIVE_FLAG === 'Y') {
                $scope.data.push(angular.copy(charge));
            }
            charge.CHARGE_AMOUNT = 0.0;
        });

        setTimeout(function () {

            $('input[type="checkbox"]')
                .not('[ng-click*="sync"]')
                .prop('checked', false);

            var dealerCombo = $('#dealercode').data("kendoComboBox");

            if (dealerCombo && dealerCombo.dataItem()) {
                dealerCombo.value([]);
            }

            $scope.$applyAsync();

        }, 100);

        $scope.adtotal = 0.00;
        $scope.addition = 0;
        $scope.deduction = 0;
    };

    $scope.printDiv = function (divName) {


        var printContents = document.getElementById(divName).innerHTML;

        var popupWin = window.open('', '_blank', 'width=800,height=800', 'orientation = portrait');
        //popupWin.ScreenOrientation = "Portrait";
        popupWin.document.open();
        popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
        popupWin.document.close();

        $scope.masterChildData = [];
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

        $scope.summary.grandTotal = 0;

        $scope.summary = { 'grandTotal': 0 };
        $scope.units = [];
        $scope.totalQty = 0;
        //var pCUrl = "/api/TemplateApi/GetPrintCountByVC?voucherno=" + $scope.OrderNo;
        //$http.get(pCUrl).then(function (response) {

        //    
        //    $scope.printcount = response.data;
        //    if ($scope.printcount > 1) {
        //        $scope.printcountdiv = "Copy of original";
        //    }
        //    else { $scope.printcountdiv ="Original"}
        //});

    }

    //$scope.cancelPrint = function (event) {

    //    $scope.masterChildData = [];
    //    $scope.masterModels = {}; // for master model
    //    //$scope.childModels = []; // for dynamic
    //    angular.forEach($scope.MasterFormElement, function (value, key) {

    //        if (value.MASTER_CHILD_FLAG == 'M' && value.DISPLAY_FLAG == 'Y') {

    //            $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
    //            //$scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
    //            if (value['COLUMN_NAME'].indexOf('DATE') > -1) {
    //                $scope.masterModels[value['COLUMN_NAME']] = $filter('date')(new Date(), 'dd-MMM-yyyy');
    //            }
    //        }

    //    });

    //    var cml = $scope.childModels.length;
    //    var sl = parseFloat(cml) - 1;
    //    $scope.ChildFormElement.splice(0, sl);
    //    $scope.childModels.splice(0, sl);
    //    angular.forEach($scope.ChildFormElement[0].element, function (value, key) {
    //        if (value.MASTER_CHILD_FLAG == 'C' && value.DISPLAY_FLAG == 'Y') {
    //            $scope.childModels[0][value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
    //        }
    //    });

    //    $scope.summary.grandTotal = 0;

    //    $scope.summary = { 'grandTotal': 0 };
    //    $scope.units = [];
    //    $scope.totalQty = 0;
    //    var newdeffered = $.Deferred();
    //    var date = new Date();
    //    $scope.todaydate = $filter('date')(new Date(), 'dd-MMM-yyyy');
    //    var newordernumber = formtemplateservice.getnewlygeneratedvoucherno($scope.companycode, $scope.FormCode, $scope.todaydate, $scope.DocumentName, newdeffered);

    //    $.when(newdeffered).done(function (results) {
    //        $scope.newgenorderno = results.data;


    //    });

    //}

    $scope.ResetDocument = function () {
        debugger;
        if ($scope.OrderNo === "undefined") {
            $scope.masterModels = {}
            $scope.masterChildData = [];
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
            $scope.data = [];
            $scope.deduction = 0;
            $scope.addition = 0;
            $scope.adtotal = 0;


            $scope.summary.grandTotal = 0;

            $scope.summary = { 'grandTotal': 0 };
            $scope.units = [];
            $scope.totalQty = 0;
            mySalesDropzone.processQueue();

            $scope.refreshDraft();

            $(".table_body input[type=number]").off('change');
            $scope.iamFromReference = false;
            var newdeffered = $.Deferred();
            var date = new Date();
            $scope.todaydate = $filter('date')(new Date(), 'dd-MMM-yyyy');
            var newordernumber = formtemplateservice.getnewlygeneratedvoucherno($scope.companycode, $scope.FormCode, $scope.todaydate, $scope.DocumentName, newdeffered);

            $.when(newdeffered).done(function (results) {

                $scope.newgenorderno = results.data;
            });
        }
        else {
            debugger;
            var saleOrderformDetail = formtemplateservice.getSalesOrderDetail_ByFormCodeAndOrderNo($scope.FormCode, $scope.OrderNo, d4);
            $.when(d4).done(function (result) {
                debugger;

                //var tempFn1 = function (result) {
                debugger;
                $scope.INDIV_ITEM_CHARGE_MODAL.push({ ITEM_CODE: "", QUANTITY: "", UNIT_PRICE: "", TOTAL_PRICE: "", CALC_QUANTITY: "", CALC_UNIT_PRICE: "", CALC_TOTAL_PRICE: "", INV_ITEM_CHARGE_AMOUNT_WISE: [], INV_ITEM_CHARGE_QUANTITY_WISE: [] });
                $scope.INDIV_ITEM_CHARGE_MODAL_AMOUNT.push({ CHARGE_CODE: "", CHARGE_TYPE: "", CHARGE_EDESC: "", IMPACT_ON: "", APPLY_QUANTITY: "", VALUE_PERCENT_AMOUNT: "", CHARGE_AMOUNT: "", VALUE_PERCENT_FLAG: "", ACC_CODE: "", SUB_CODE: "", BUDGET_CODE: "", GL: "", APPORTION_FLAG: "", CALC: "", APPLY_ON: "" });
                $scope.INDIV_ITEM_CHARGE_MODAL_QUANTITY.push({ CHARGE_CODE: "", CHARGE_TYPE: "", CHARGE_EDESC: "", IMPACT_ON: "", APPLY_QUANTITY: "", VALUE_PERCENT_AMOUNT: "", CHARGE_AMOUNT: "", VALUE_PERCENT_FLAG: "", ACC_CODE: "", SUB_CODE: "", BUDGET_CODE: "", GL: "", APPORTION_FLAG: "", CALC: "", APPLY_ON: "" });

                var rows = result.data;
                //if (count == 0) {
                if (rows.length > 0) {
                    //count++;
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
                            $('.dz-remove').css("display", "block");
                        }
                    }

                    if ($scope.OrderNo != undefined && $scope.OrderNo != "undefined") {

                        $scope.ChildFormElement = [];
                        $scope.childModels = [];
                        $scope.dynamicInvItenChargeModalData = [];
                        $scope.dynamicInvItenChargeModalData_OK = [];
                        $scope.dynamicSerialTrackingModalData = [];
                        $scope.dynamicBatchTrackingModalData = [];
                        $scope.newgenorderno = "";
                        $scope.save = "Update";
                        $scope.savecontinue = "Update & Continue";
                        //var mastertempCopy = angular.copy($scope.masterModelTemplate);
                        //var mastercopy = $scope.getObjWithKeysFromOtherObj(mastertempCopy, rows[0]);
                        //setTimeout(function () {
                        //    $scope.masterModels = {};
                        //    $scope.masterModels = angular.copy(mastercopy);
                        //    $scope.muwiseQty();

                        //})
                    }
                    for (var i = 0; i < rows.length; i++) {

                        //var tempCopy = angular.copy($scope.childModelTemplate);

                        //$scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
                        //$scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));
                        //var mastertempCopy = angular.copy($scope.masterModelTemplate);
                        //var mastercopy = $scope.getObjWithKeysFromOtherObj(mastertempCopy, rows[i]);

                        //$scope.masterModels = angular.copy(mastercopy);
                        setDataOnModal(rows, i);

                    }

                    $scope.muwiseQty();
                    $scope.someDateFn();

                    if ($scope.DocumentName == "SA_SALES_INVOICE") {
                        if ($scope.OrderNo != undefined && $scope.OrderNo != "undefined") {
                            setTimeout(function () {
                                $(".btn-action a").css('display', 'none');
                            }, 0);
                        }

                    }
                    if ($scope.DocumentName == "SA_SALES_RETURN") {
                        if ($scope.OrderNo != undefined && $scope.OrderNo != "undefined") {
                            setTimeout(function () {

                                $(".btn-action a").css('display', 'none');
                                var childlen = $scope.childModels.length;
                                for (var i = 0; i < childlen; i++) {

                                    //if ($scope.childModels[0].hasOwnProperty("QUANTITY")) {
                                    //    $("." + "QUANTITY_" + i).attr("disabled", "disabled");
                                    //}
                                    if ($scope.childModels[0].hasOwnProperty("UNIT_PRICE")) {
                                        $("." + "UNIT_PRICE_" + i).attr("disabled", "disabled");
                                    }
                                    //if ($scope.childModels[0].hasOwnProperty("TOTAL_PRICE")) {
                                    //    $("." + "TOTAL_PRICE" + i).attr("disabled", "disabled");
                                    //}
                                    if ($scope.childModels[0].hasOwnProperty("CALC_QUANTITY")) {
                                        $("." + "CALC_QUANTITY_" + i).attr("disabled", "disabled");
                                    }
                                    if ($scope.childModels[0].hasOwnProperty("CALC_UNIT_PRICE")) {
                                        $("." + "CALC_UNIT_PRICE_" + i).attr("disabled", "disabled");
                                    }
                                    //if ($scope.childModels[0].hasOwnProperty("CALC_TOTAL_PRICE")) {
                                    //    $("." + "CALC_TOTAL_PRICE" + i).attr("disabled", "disabled");
                                    //}
                                }
                            }, 0);
                        }

                    }



                    //$("#refrencetype").val();
                } else {
                    $scope.masterModels = angular.copy($scope.masterModelTemplate);
                }

                function setDataOnModal(rows, i) {
                    debugger;
                    console.log("Row data ==============================>>>>" + JSON.stringify(rows));

                    var tempCopy = angular.copy($scope.childModelTemplate);

                    $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
                    $scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));
                    //subin
                    setTimeout(function () {

                        $("#products_" + i).data('kendoComboBox').dataSource.data([{ ItemCode: rows[i].ITEM_CODE, ItemDescription: rows[i].ITEM_EDESC, Type: "code" }]);
                        console.log("value of productCode from kendo===================>>>>" + JSON.stringify($("#products_" + i).data('kendoComboBox').dataSource.data()[0].ItemCode));
                        $scope.childModels[i].ITEM_CODE = $("#products_" + i).data('kendoComboBox').dataSource.data()[0].ItemCode;



                        $("#customers").data('kendoComboBox').dataSource.data([{ CustomerCode: rows[i].CUSTOMER_CODE, CustomerName: rows[i].CUSTOMER_EDESC, Type: "code", REGD_OFFICE_EADDRESS: rows[i].REGD_OFFICE_EADDRESS, TPIN_VAT_NO: rows[i].TPIN_VAT_NO, TEL_MOBILE_NO1: rows[i].TEL_MOBILE_NO1, CUSTOMER_NDESC: rows[i].CUSTOMER_NDESC }]);
                        console.log("value of customer from kendo===================>>>>" + JSON.stringify($('#customers').data('kendoComboBox').dataSource.data()[0].CustomerCode));
                        $scope.masterModels.CUSTOMER_CODE = $('#customers').data('kendoComboBox').dataSource.data()[0].CustomerCode;

                        var searchText = "";

                        CustomerCode = $scope.masterModels.CUSTOMER_CODE;
                        var getdealerByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetPartyTypeByFilterAndCustomerCode?filter=" + searchText + '&customercode=' + CustomerCode;
                        $("#dealercode").kendoComboBox({
                            optionLabel: "--Select Dealer Code--",
                            filter: "contains",
                            dataTextField: "PARTY_TYPE_EDESC",
                            dataValueField: "PARTY_TYPE_CODE",

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
                            select: function (e) {

                                $('#style-switcher').addClass('opened');
                                $('#style-switcher').animate({ 'left': '-241px', 'width': '273px' });

                            }
                        });

                        if ($("#dealercode").data('kendoComboBox') != null) {
                            $("#dealercode").data('kendoComboBox').dataSource.data([{ PARTY_TYPE_CODE: rows[i].PARTY_TYPE_CODE, PARTY_TYPE_EDESC: rows[i].PARTY_TYPE_EDESC, Type: "code" }]);
                        }


                    }, 0);


                    var mastertempCopy = angular.copy($scope.masterModelTemplate);
                    var mastercopy = $scope.getObjWithKeysFromOtherObj(mastertempCopy, rows[i]);
                    $scope.masterModels = angular.copy(mastercopy);
                    console.log("$scope.masterModels======================>>>>" + JSON.stringify($scope.masterModels));
                    var itemchargeModel = angular.copy($scope.INDIV_ITEM_CHARGE_MODAL[0]);
                    var itemchargeamountwiseModel = angular.copy($scope.INDIV_ITEM_CHARGE_MODAL_AMOUNT[0]);
                    var itemchargequantitywiseModel = angular.copy($scope.INDIV_ITEM_CHARGE_MODAL_QUANTITY[0]);

                    var rowsObj = rows[i];
                    $scope.dynamicInvItenChargeModalData.push($scope.getObjWithKeysFromOtherObj(itemchargeModel, rowsObj));
                    if ($scope.dynamicInvItenChargeModalData[i].INV_ITEM_CHARGE_AMOUNT_WISE == undefined) {
                        $scope.dynamicInvItenChargeModalData[i].INV_ITEM_CHARGE_AMOUNT_WISE = [];
                        $scope.dynamicInvItenChargeModalData[i].INV_ITEM_CHARGE_AMOUNT_WISE.push(itemchargeamountwiseModel);
                    }
                    if ($scope.dynamicInvItenChargeModalData[i].INV_ITEM_CHARGE_QUANTITY_WISE == undefined) {
                        $scope.dynamicInvItenChargeModalData[i].INV_ITEM_CHARGE_QUANTITY_WISE = [];
                        $scope.dynamicInvItenChargeModalData[i].INV_ITEM_CHARGE_QUANTITY_WISE.push(itemchargequantitywiseModel);
                    }


                    $scope.dynamicInvItenChargeModalData_OK.push($scope.getObjWithKeysFromOtherObj(itemchargeModel, rowsObj));
                    if ($scope.dynamicInvItenChargeModalData_OK[i].INV_ITEM_CHARGE_AMOUNT_WISE == undefined) {
                        $scope.dynamicInvItenChargeModalData_OK[i].INV_ITEM_CHARGE_AMOUNT_WISE = [];
                        $scope.dynamicInvItenChargeModalData_OK[i].INV_ITEM_CHARGE_AMOUNT_WISE.push(itemchargeamountwiseModel);
                    }
                    if ($scope.dynamicInvItenChargeModalData_OK[i].INV_ITEM_CHARGE_QUANTITY_WISE == undefined) {
                        $scope.dynamicInvItenChargeModalData_OK[i].INV_ITEM_CHARGE_QUANTITY_WISE = [];
                        $scope.dynamicInvItenChargeModalData_OK[i].INV_ITEM_CHARGE_QUANTITY_WISE.push(itemchargequantitywiseModel);
                    }


                    var config = {
                        async: false
                    };


                    //for (var b = 0; b < $scope.dynamicInvItenChargeModalData.length; b++) {
                    angular.forEach($scope.dynamicInvItenChargeModalData, function (value, key) {

                        var responsevaluewise = $http.get('/api/TemplateApi/GetItemChargeDataSavedValueWise?voucherNo=' + rows[i].SALES_NO + '&itemcode=' + rows[i].ITEM_CODE, config);

                        responsevaluewise.then(function (resvaluewise) {
                            if (resvaluewise.data != "") {

                                for (var a = 0; a < resvaluewise.data.length; a++) {

                                    $scope.dynamicInvItenChargeModalData[key].INV_ITEM_CHARGE_AMOUNT_WISE[a] = $scope.getObjWithKeysFromOtherObjInvCharge(itemchargeamountwiseModel, resvaluewise.data[a]);
                                    $scope.dynamicInvItenChargeModalData_OK[key].INV_ITEM_CHARGE_AMOUNT_WISE[a] = $scope.getObjWithKeysFromOtherObjInvCharge(itemchargeamountwiseModel, resvaluewise.data[a]);
                                }

                            }

                        });

                    });
                    if ($scope.masterModels.hasOwnProperty("CUSTOMER_CODE")) {

                        searchText = "";
                        var getpricelistidByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetPriceListByFilterAndCustomerCode?filter=" + searchText + '&customercode=' + $scope.masterModels.CUSTOMER_CODE;
                        BindPriceList($scope.masterModels.CUSTOMER_CODE, "");


                    }

                    if ($scope.serial_tracking_flag == "Y") {

                        var config = {
                            async: false
                        };
                        $scope.BATCH_MODAL.push({ ITEM_CODE: "", ITEM_EDESC: "", MU_CODE: "", LOCATION_CODE: "", QUANTITY: "", TRACK: [] });
                        $scope.BATCH_CHILD_MODAL.push({ SERIAL_NO: 1, TRACKING_SERIAL_NO: "" });
                        for (var itt = 0; itt < $scope.childModels.length; itt++) {

                            var responsevaluewise = $http.get('/api/TemplateApi/getItemCountResult?code=' + $scope.childModels[itt].ITEM_CODE, config);
                            responsevaluewise.then(function (resvaluewise) {

                                if (resvaluewise.data == true) {


                                    for (var it = 0; it < $scope.childModels.length; it++) {

                                        if ($scope.childModels[it].hasOwnProperty("FROM_LOCATION_CODE")) {
                                            if ($scope.childModels[it].FROM_LOCATION_CODE != 'undefined') {
                                                locationcode = $scope.childModels[it].FROM_LOCATION_CODE;


                                                var ibdrreq = "/api/TemplateApi/GetDataForBatchModalsalesforedit?itemcode=" + $scope.childModels[it].ITEM_CODE + "&loactioncode=" + $scope.childModels[it].FROM_LOCATION_CODE + "&voucherno=" + $scope.OrderNo;
                                                $http.get(ibdrreq).then(function (ibdrreqresults) {

                                                    if (ibdrreqresults.data.length > 0) {
                                                        var rows = ibdrreqresults.data;

                                                        for (var itn = 0; itn < $scope.childModels.length; itn++) {

                                                            setbatchDataOnModal(rows, itn);
                                                        }
                                                    }
                                                })
                                            }
                                        }
                                    }
                                }

                            });

                        }
                    }
                    //for batch Transaction
                    if ($scope.batch_tracking_flag == "Y" && $scope.serial_tracking_flag == "N") {
                        $scope.BATCH_MODAL.push({ ITEM_CODE: "", ITEM_EDESC: "", MU_CODE: "", LOCATION_CODE: "", QUANTITY: "" });
                        for (var itt = 0; itt < $scope.childModels.length; itt++) {
                            $scope.batchTranIcon[itt] = true;
                            var responsevaluewise = $http.get('/api/TemplateApi/BatchWiseItemCheck?code=' + $scope.childModels[itt].ITEM_CODE);
                            responsevaluewise.then(function (resvaluewise) {
                                if (resvaluewise.data == true) {
                                    for (var it = 0; it < $scope.childModels.length; it++) {
                                        if ($scope.childModels[it].hasOwnProperty("FROM_LOCATION_CODE")) {
                                            if ($scope.childModels[it].FROM_LOCATION_CODE != 'undefined') {
                                                locationcode = $scope.childModels[it].FROM_LOCATION_CODE;
                                                var ibdrreq = "/api/TemplateApi/GetbatchTranDataByItemCodeAndLocCode?itemcode=" + $scope.childModels[it].ITEM_CODE + "&loactioncode=" + $scope.childModels[it].FROM_LOCATION_CODE + "&refernceNo=" + $scope.OrderNo;
                                                $http.get(ibdrreq).then(function (ibdrreqresults) {
                                                    if (ibdrreqresults.data.length > 0) {
                                                        angular.forEach(ibdrreqresults.data, function (value, key) {
                                                            if (!$scope.checkedBatchTranResult.find(obj => obj["BATCH_NO"] === value.BATCH_NO)) {
                                                                $scope.checkedBatchTranResult.push({
                                                                    "ITEM_CODE": value.ITEM_CODE,
                                                                    "MU_CODE": value.MU_CODE,
                                                                    "LOCATION_CODE": value.LOCATION_CODE,
                                                                    "SERIAL_NO": value.SERIAL_NO,
                                                                    "SERIAL_TRACKING_FLAG": $scope.serial_tracking_flag,
                                                                    "BATCH_TRACKING_FLAG": $scope.batch_tracking_flag,
                                                                    "QUANTITY": value.QUANTITY,
                                                                    "BATCH_NO": value.BATCH_NO,
                                                                    "EXPIRY_DATE": value.EXPIRY_DATE
                                                                });
                                                                $scope.quantitySum = $scope.quantitySum + value.QUANTITY;
                                                            }
                                                        });
                                                        var rows = ibdrreqresults.data;
                                                        for (var itn = 0; itn < $scope.childModels.length; itn++) {
                                                            setbatchTran(rows, itn);
                                                        }
                                                    }
                                                })
                                            }
                                        }
                                    }
                                }
                            });
                        }
                    }
                }
                //}
                //};         
            });

        }
    }

    $scope.ResetDocumentPrint = function () {


        $scope.masterModels = {}
        $scope.masterChildData = [];
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
        $scope.data = [];
        $scope.deduction = 0;
        $scope.addition = 0;
        $scope.adtotal = 0;


        $scope.summary.grandTotal = 0;

        $scope.summary = { 'grandTotal': 0 };
        $scope.units = [];
        $scope.totalQty = 0;
        mySalesDropzone.processQueue();

        $scope.refreshDraft();

        $(".table_body input[type=number]").off('change');
        $scope.iamFromReference = false;
        var newdeffered = $.Deferred();
        var date = new Date();
        $scope.todaydate = $filter('date')(new Date(), 'dd-MMM-yyyy');
        var newordernumber = formtemplateservice.getnewlygeneratedvoucherno($scope.companycode, $scope.FormCode, $scope.todaydate, $scope.DocumentName, newdeffered);

        $.when(newdeffered).done(function (results) {
            debugger;
            $scope.newgenorderno = results.data;
            var primarycolname = PrimaryColumnForTable($scope.DocumentName);
            $scope.masterModels[primarycolname] = $scope.newgenorderno;
        });
    }

    $scope.SetMuCode = function (key, kendoEvent) {
        debugger;
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
            var purity = kendoEvent.sender.dataItem().ItemGrade;
            var nonVatFlag = kendoEvent.sender.dataItem().NON_VAT_FLAG;
            $scope.childModels[key]["MU_CODE"] = mucode;
            $scope.childModels[key]["PURITY"] = purity;
            $scope.childModels[key]["NON_VAT_FLAG"] = nonVatFlag;
            if ($scope.serial_tracking_flag == "Y") {

                var icrreq = "/api/TemplateApi/getItemCountResult?code=" + kendoEvent.sender.dataItem().ItemCode;
                $http.get(icrreq).then(function (icrreqresults) {

                    if (icrreqresults.data == true) {

                        if ($scope.childModels[0].hasOwnProperty("FROM_LOCATION_CODE")) {
                            if ($scope.childModels[key].FROM_LOCATION_CODE != 'undefined') {
                                locationcode = $scope.childModels[key].FROM_LOCATION_CODE;
                            }
                        }
                        var ibdrreq = "/api/TemplateApi/GetDataForBatchModalsales?itemcode=" + kendoEvent.sender.dataItem().ItemCode + "&loactioncode=" + locationcode;
                        $http.get(ibdrreq).then(function (ibdrreqresults) {

                            if (ibdrreqresults.data.length > 0) {

                                //$scope.dynamicSerialTrackingModalData[key] = [];
                                $scope.BATCH_MODAL.push({ ITEM_CODE: "", ITEM_EDESC: "", MU_CODE: "", LOCATION_CODE: "", QUANTITY: "", TRACK: [] });
                                $scope.BATCH_CHILD_MODAL.push({ SERIAL_NO: 1, TRACKING_SERIAL_NO: "" });
                                var batchModel = angular.copy($scope.BATCH_MODAL[0]);
                                var batchChildModel = angular.copy($scope.BATCH_CHILD_MODAL[0]);
                                //$scope.dynamicSerialTrackingModalData.push(batchModel);
                                $scope.dynamicSerialTrackingModalData[key] = $scope.getObjWithKeysFromOtherObj(batchModel, ibdrreqresults.data[0]);
                                $scope.dynamicSerialTrackingModalData[key].TRACK = [];
                                for (var bm = 0; bm < ibdrreqresults.data.length; bm++) {


                                    $scope.dynamicSerialTrackingModalData[key].TRACK.push(batchChildModel);
                                }
                                //for (var bm = 0; bm < ibdrreqresults.data.length; bm++) {
                                //      
                                //    $scope.dynamicSerialTrackingModalData.push($scope.getObjWithKeysFromOtherObj(batchModel, ibdrreqresults.data[bm]));
                                //    if ($scope.dynamicSerialTrackingModalData[bm].TRACK == undefined || $scope.dynamicSerialTrackingModalData[bm].TRACK == null) {
                                //        $scope.dynamicSerialTrackingModalData[bm].TRACK = [];
                                //        $scope.dynamicSerialTrackingModalData[bm].TRACK.push(batchChildModel);

                                //    }
                                //  }


                                var config = {
                                    async: false
                                };
                                if (ibdrreqresults.data != "") {


                                    for (var a = 0; a < ibdrreqresults.data.length; a++) {

                                        for (var b = 0; b < $scope.dynamicSerialTrackingModalData.length; b++) {

                                            if (ibdrreqresults.data[a].ITEM_CODE == $scope.dynamicSerialTrackingModalData[b].ITEM_CODE) {

                                                //var serialno = batchResult.data[a].SERIAL_NO - 1;

                                                $scope.dynamicSerialTrackingModalData[b].TRACK[a] = $scope.getObjWithKeysFromOtherObj(batchChildModel, ibdrreqresults.data[a]);

                                            }
                                        }
                                    }


                                }

                            }
                        });
                        checkedItems = [];
                        var checkedIds = {};
                        $scope.batchGridOptions = {
                            dataSource: {
                                type: "json",

                                transport: {
                                    read: "/api/TemplateApi/GetDataForBatchModalsales?itemcode=" + kendoEvent.sender.dataItem().ItemCode + "&loactioncode=" + locationcode,
                                },
                                //pageSize: 5,
                                serverPaging: false,
                                serverSorting: false
                            },
                            sortable: false,
                            pageable: false,
                            dataBound: function (e) {

                                //$scope.detailExportPromises = [];
                                $('div').removeClass('.k-header k-grid-toolbar');

                                $(".checkbox1").on("click", selectRow1);

                                var view = this.dataSource.data();
                                for (var j = 0; j < checkedItems.length; j++) {

                                    for (var i = 0; i < view.length; i++) {

                                        if (checkedItems[j].TRANSACTION_NO == view[i].TRANSACTION_NO) {
                                            this.tbody.find("tr[data-uid='" + view[i].uid + "']")
                                                .addClass("k-state-selected")
                                                .find(".checkbox")
                                                .attr("checked", "checked");
                                        }
                                    }

                                }

                                var grid = e.sender;
                                if (grid.dataSource.total() == 0) {
                                    var colCount = grid.columns.length + 1;
                                    $(e.sender.wrapper)
                                        .find('tbody')
                                        .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, No Data Found For Given Filter. </td></tr>');

                                }

                            },
                            columns: [
                                {
                                    template: function (dataItem) {
                                        return "<input type='checkbox' id='${dataItem.TRANSACTION_NO}' class='checkbox1 row-checkbox'><label class='k-checkbox-label' for='${dataItem.TRANSACTION_NO}'></label>"
                                    },
                                    width: 20
                                },
                                {
                                    field: "TRACKING_SERIAL_NO",
                                    title: "Serial NO",
                                    width: "40px"
                                }
                            ]
                        };
                        function selectRow1() {

                            var checked = this.checked,
                                row = $(this).closest("tr"),
                                grid = $("#batchGrid_" + key).data("kendoGrid"),
                                dataItem = grid.dataItem(row);

                            if (checked) {
                                if (row.hasClass("k-state-selected")) {
                                    return;

                                }
                                else {
                                    row.addClass("k-state-selected");
                                    $(this).attr('checked', true);
                                    checkedIds[dataItem.TRANSACTION_NO] = checked;
                                    checkedItems.push({
                                        //"TRACKING_SERIAL_NO": dataItem.TRACKING_SERIAL_NO,
                                        "ITEM_CODE": dataItem.ITEM_CODE,
                                        "MU_CODE": dataItem.MU_CODE,
                                        "LOCATION_CODE": dataItem.LOCATION_CODE,
                                        "SERIAL_NO": dataItem.SERIAL_NO,
                                        "TRACKING_SERIAL_NO": dataItem.TRACKING_SERIAL_NO
                                    });

                                }
                            }
                            else {
                                for (var i = 0; i < checkedItems.length; i++) {
                                    if (checkedItems[i].TRANSACTION_NO == dataItem.TRANSACTION_NO && checkedItems[i].TRACKING_SERIAL_NO == dataItem.TRACKING_SERIAL_NO) {
                                        checkedItems.splice(i, 1);
                                    }
                                }
                                row.removeClass("k-state-selected");
                            }

                            //for (var f = 0; f < checkedItems.length; f++) {
                            //    
                            //    $scope.checkedresult.push({
                            //        //"TRACKING_SERIAL_NO": dataItem.TRACKING_SERIAL_NO,
                            //        "ITEM_CODE": checkedItems[f].ITEM_CODE,
                            //        "MU_CODE": checkedItems[f].MU_CODE,
                            //        "LOCATION_CODE": checkedItems[f].LOCATION_CODE,
                            //        "SERIAL_NO": checkedItems[f].SERIAL_NO,
                            //        "TRACKING_SERIAL_NO": checkedItems[f].TRACKING_SERIAL_NO
                            //    });
                            //}
                            $scope.childModels[key].QUANTITY = Object.keys(checkedIds).length;

                        }

                    }
                });

                setTimeout(function () {
                    $(".serialtrackFlag_" + key).modal('toggle');
                }, 1500);

            }

            /////for batch transaction/////
            if ($scope.batch_tracking_flag == "Y" && $scope.serial_tracking_flag == "N") {
                var responsevaluewise = $http.get('/api/TemplateApi/BatchWiseItemCheck?code=' + $scope.childModels[key].ITEM_CODE);
                responsevaluewise.then(function (resvaluewise) {
                    if (resvaluewise.data == true) {
                        if ($scope.childModels[key].hasOwnProperty("FROM_LOCATION_CODE")) {
                            if ($scope.childModels[key].FROM_LOCATION_CODE != 'undefined') {
                                locationcode = $scope.childModels[key].FROM_LOCATION_CODE;
                            }
                        }
                        var ibdrreq = "/api/TemplateApi/GetbatchTranDataByItemCodeAndLocCode?itemcode=" + kendoEvent.sender.dataItem().ItemCode + "&loactioncode=" + locationcode;
                        $http.get(ibdrreq).then(function (ibdrreqresults) {
                            if (ibdrreqresults.data.length > 0) {
                                $scope.BATCH_MODAL.push({ ITEM_CODE: "", ITEM_EDESC: "", MU_CODE: "", LOCATION_CODE: "", QUANTITY: "" });
                                var batchModel = angular.copy($scope.BATCH_MODAL[0]);
                                $scope.dynamicBatchTrackingModalData[key] = $scope.getObjWithKeysFromOtherObj(batchModel, ibdrreqresults.data[0]);
                            }
                        });
                        $scope.quantitySum = 0;
                        setTimeout(function () {
                            $(".batchTran_" + key).modal('toggle');
                        }, 1500);
                        $scope.batchDataGridOptions = {
                            dataSource: {
                                type: "json",

                                transport: {
                                    read: "/api/TemplateApi/GetbatchTranDataByItemCodeAndLocCode?itemcode=" + $scope.childModels[key].ITEM_CODE + "&loactioncode=" + locationcode,
                                },
                                serverPaging: false,
                                serverSorting: false
                            },
                            sortable: false,
                            pageable: false,
                            dataBound: function (e) {
                                e.preventDefault();
                                if (!e.node) {
                                    if ($scope.checkedBatchTranResult.length > 0) {
                                        angular.forEach($scope.checkedBatchTranResult, function (value, key) {
                                            angular.forEach(e.sender.dataSource._data, function (val, k) {
                                                if (value.BATCH_NO == val.BATCH_NO) {

                                                    $('#' + value.BATCH_NO).prop('checked', true);
                                                    $('#Qty_' + value.BATCH_NO).val(value.QUANTITY);
                                                    $('#Qty_' + value.BATCH_NO).attr('readonly', true);
                                                }
                                            });
                                        });
                                    }
                                    $('div').removeClass('.k-header k-grid-toolbar');
                                    $(".chkBatchTran").on("change", chkRow);
                                    var grid = e.sender;
                                    if (grid.dataSource.total() == 0) {
                                        var colCount = grid.columns.length + 1;
                                        $(e.sender.wrapper)
                                            .find('tbody')
                                            .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, No Data Found For Given Filter. </td></tr>');
                                    }
                                }
                            },
                            columns: [
                                {
                                    template: function (dataItem) {
                                        return "<input type='checkbox' id='" + dataItem.BATCH_NO + "' class='chkBatchTran row-checkbox'>"
                                    },
                                    width: 20
                                },
                                {
                                    field: "BATCH_NO",
                                    title: "Batch NO",
                                    width: "40px"
                                },
                                {
                                    field: "EXPIRY_DATE",
                                    title: "Expiry Date",
                                    template: "#= kendo.toString(kendo.parseDate(EXPIRY_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
                                    width: "40px"
                                },
                                {
                                    field: "QUANTITY",
                                    title: "Quantity",
                                    width: "40px"
                                },
                                {
                                    title: "Quantity To Out",
                                    template: function (dataItem) {
                                        return "<input type='number' id='Qty_" + dataItem.BATCH_NO + "' class='row-checkbox form-control' style='height: auto;width: auto;'>"
                                    },
                                    width: "40px"
                                }
                            ]
                        };
                        function chkRow() {
                            var checked = this.checked,
                                row = $(this).closest("tr"),
                                grid = $("#batchTranGrid_" + key).data("kendoGrid"),
                                dataItem = grid.dataItem(row);
                            var outQty = parseInt($(this).closest('tr').find('#Qty_' + dataItem.BATCH_NO).val());
                            if (checked) {
                                var chkMultipleHit = false;
                                if ($scope.checkedBatchTranResult.length > 0) {
                                    angular.forEach($scope.checkedBatchTranResult, function (value, key) {
                                        if (value.BATCH_NO == dataItem.BATCH_NO) {
                                            chkMultipleHit = true;
                                        }
                                    });
                                }
                                if (chkMultipleHit == false) {
                                    if (outQty == "" || outQty == "undefined" || isNaN(outQty)) {
                                        alert('Out Quantity not defined.');
                                        $(this).closest('tr').find('#Qty_' + dataItem.BATCH_NO).attr('readonly', false);
                                        $(this).closest('tr').find('#Qty_' + dataItem.BATCH_NO).focus();
                                        $(this).closest('tr').find('#' + dataItem.BATCH_NO).attr('checked', false);
                                    }
                                    else if (dataItem.QUANTITY < outQty) {
                                        alert('Out Quantity greater than provided quantity.');
                                        $(this).closest('tr').find('#Qty_' + dataItem.BATCH_NO).attr('readonly', false);
                                        $(this).closest('tr').find('#Qty_' + dataItem.BATCH_NO).focus();
                                        $(this).closest('tr').find('#' + dataItem.BATCH_NO).attr('checked', false);
                                    }
                                    else {
                                        $scope.checkedBatchTranResult.push({
                                            "ITEM_CODE": dataItem.ITEM_CODE,
                                            "MU_CODE": dataItem.MU_CODE,
                                            "LOCATION_CODE": dataItem.LOCATION_CODE,
                                            "SERIAL_NO": dataItem.SERIAL_NO,
                                            "SERIAL_TRACKING_FLAG": $scope.serial_tracking_flag,
                                            "BATCH_TRACKING_FLAG": $scope.batch_tracking_flag,
                                            "QUANTITY": outQty,
                                            "BATCH_NO": dataItem.BATCH_NO,
                                            "EXPIRY_DATE": dataItem.EXPIRY_DATE.split('T')[0]
                                        });
                                        $(this).closest('tr').find('#Qty_' + dataItem.BATCH_NO).attr('readonly', true);
                                        $('.QUANTITY_' + key).attr('readonly', true);
                                        if ($scope.childModels[key].QUANTITY == "" || isNaN($scope.childModels[key].QUANTITY)) {
                                            $scope.childModels[key].QUANTITY = 0;
                                        }
                                        $scope.childModels[key].QUANTITY = parseInt($scope.childModels[key].QUANTITY) + outQty;
                                    }
                                }
                            }
                            else {
                                if ($scope.checkedBatchTranResult.find(obj => obj["BATCH_NO"] === dataItem.BATCH_NO)) {
                                    $(this).closest('tr').find('#Qty_' + dataItem.BATCH_NO).attr('readonly', false);
                                    $(this).closest('tr').find('#Qty_' + dataItem.BATCH_NO).val("");
                                    var data = $.grep($scope.checkedBatchTranResult, function (e) {
                                        return e.BATCH_NO != dataItem.BATCH_NO;
                                    });
                                    if (data.length > 0) {
                                        $scope.checkedBatchTranResult = [];
                                        $scope.checkedBatchTranResult = data;
                                    }
                                    $scope.childModels[key].QUANTITY = parseInt($scope.childModels[key].QUANTITY) - outQty;
                                }
                            }
                        }
                    }
                });
            }
        }
        $scope.muwiseQty();
        //var cs_code = "";
        //if ($scope.masterModels["CUSTOMER_CODE"] != undefined) {
        //    cs_code = $scope.masterModels["CUSTOMER_CODE"];
        //}
        var ps_code = ""
        if ($('#customerpriceid').data("kendoComboBox") != undefined) {
            if ($('#customerpriceid').data("kendoComboBox").dataItem() != undefined) {
                ps_code = $('#customerpriceid').data("kendoComboBox").dataItem().MASTER_ID;
            }
        }
        //var area_code = "";
        //if ($scope.masterModels["AREA_CODE"] != undefined) {
        //    area_code = $scope.masterModels["AREA_CODE"];
        //}
        var item_code = "";
        if (kendoEvent.sender.dataItem() != undefined) {
            item_code = kendoEvent.sender.dataItem().ItemCode;
        }
        //if (ps_code != "" && item_code != "") {
        //    $scope.PricelistItemRate(ps_code, item_code, key);
        //}



        //$scope.StandardItemRate(cs_code, $scope.FormCode, area_code, item_code, key);

    }
    // check  child validation
    $scope.checkValidation = function (kendoEvent) {
        if (kendoEvent.sender.dataItem() == undefined) {
            //$(kendoEvent.sender.element[0]).parent().parent().parent().css({ "border": "solid 1px red" });
            $(kendoEvent.sender.element[0]).parent().parent().parent().addClass('borderRed');
            $(kendoEvent.sender.input[0]).val("");


        }
        else {
            //$(kendoEvent.sender.element[0]).parent().parent().parent().css({ "border": "none" });
            $(kendoEvent.sender.element[0]).parent().parent().parent().removeClass('borderRed');
        }
    }


    $scope.onChangePartyTypeCustomerSetup = function (e) {
        debugger;
        $scope.customersMasterChildArr.ACCOUNTMAP = e.sender.dataItem().ACC_CODE;

        var tree = $("#childAccountMap").data("kendoDropDownList");
        tree.value(e.sender.dataItem().ACC_CODE);
    }


    $scope.printDivJewellery = function (divName) {
        debugger;
        $scope.isPrintAble = true;
        $scope.tempChunk = $scope.chunkedProducts;
        setTimeout(function () {
            debugger;
            var printContents = document.getElementById(divName).innerHTML;

            var popupWin = window.open('', '_blank', 'width=1500,height=800', 'orientation = landscape');
            //popupWin.ScreenOrientation = "Portrait";
            popupWin.document.open();

            popupWin.document.write(`
       <html>
<head>
    <style>
        @media print {
            @page {
                size: A4 landscape;
                margin: 10mm;
            }

            .print-container {
            max-height: 170mm;
                width: 101%;
                height: 100%;
                padding: 10px;
                box-sizing: border-box;
            }

            .content {
                width: 100%;
                height: 100%;
                transform: rotate(0deg);
                transform-origin: center center;
            }

            table {
                font-size: 11px;
                width: 100%;
                border-collapse: collapse;
            }

            td, th {
                padding: 6px;
                border: 1px solid #ccc;
                text-align: left;
            }

            .info-section {
                width: 100%; /* Adjusted to fit reduced height */
                display: flex;
                justify-content: space-between;
                gap: 20px; /* Reduced gap */
                page-break-before: avoid;
            }
                .heade{
                 page-break-after: avoid !important;
                }

            .left-info, .center-info, .right-info {
                flex: 1; /* Equal width sections */
            }

            #main-table {
                font-size: 12px;
            }

            .right-info {
                justify-content: space-between;
                max-width: 100%;
                width: 100%;
                margin-right: 30px; /* Reduced margin */
                text-align: left;
                flex: 1;
            }
                .item_table_print {
                page-break-before: always;
            }
            .no-page-break{
                page-break-before: avoid !important;
                break-inside: avoid !important;
            }
            .print-page {
                break-inside: avoid;
            }
        }
    </style>
   
</head>
<body onload="window.print()">
    <div class="print-container">
        <div class="content">
            ${printContents}
        </div>
    </div>
</body>
</html>
    `);
            popupWin.document.close();

            $scope.isPrintAble = false;

            $scope.chunkedProducts = $scope.tempChunk;
            $scope.PrintDocument();
        }, 5);
    }

    $scope.ConvertNepToEng = function ($event) {

        //$event.stopPropagation();
        console.log($(this));
        var date = BS2AD($("#nepaliDate5").val());
        $("#englishdatedocument").val($filter('date')(date, "dd-MMM-yyyy"));
        $('#nepaliDate5').trigger('change')
    };

    $scope.ConvertEngToNepang = function (data) {
        $("#nepaliDate5").val(AD2BS(data));
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

    function dateSet(date) {
        var month = date.getMonth() + 1;
        var day = date.getDate();
        date = date.getFullYear() + '/' +
            (('' + month).length < 2 ? '0' : '') + month + '/' +
            (('' + day).length < 2 ? '0' : '') + day;
        return date;
    }

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

    $scope.monthSelectorOptionsSingle = {

        open: function () {

            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {

            // $scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",

        // specifies that DateInput is used for masking the input element
        dateInput: true
    };

    $scope.child_rate = {};
    $scope.deduction = 0.00;
    $scope.addition = 0.00;
    $scope.adtotal = 0.00;


    $scope.sum = function (index) {
        debugger;
        $scope.iamFromReference = false;
        if (!$scope.iamFromReference) {
            var child_rate = $scope.childModels[index].UNIT_PRICE;
            var quantity = $scope.childModels[index].QUANTITY;
            if (child_rate != null && child_rate != "" && child_rate !== undefined) {
                $scope.childModels[index].UNIT_PRICE = parseFloat(child_rate.toFixed(2));
            }
            //quantity validation start
            if (quantity === undefined) {
                $(".QUANTITY_" + index).parent().css({ "border": "solid 1px red" });

            }
            else {
                $(".QUANTITY_" + index).parent().css({ "border": "none" });
                $(".CALC_QUANTITY_" + index).parent().css({ "border": "none" });
            }

            //price validation start
            if (child_rate === undefined) {
                $(".UNIT_PRICE_" + index).parent().css({ "border": "solid 1px red" });

            }
            else {
                $(".UNIT_PRICE_" + index).parent().css({ "border": "none" });
                $(".CALC_UNIT_PRICE_" + index).parent().css({ "border": "none" });
            }

            if (child_rate === undefined && quantity === undefined) {
                $scope.childModels[index].CALC_UNIT_PRICE = "";
                $scope.childModels[index].CALC_QUANTITY = "";
                return;
            }
            //for stock quantity
            $scope.checkitemstock(index);



            //validation end


            if (typeof $scope.childModels[index][UNIT_PRICE] !== "undefined" && $scope.childModels[index][UNIT_PRICE] != null && $scope.childModels[index][UNIT_PRICE] != "") {
                $scope.childModels[index][CALC_UNIT_PRICE] = parseFloat($scope.childModels[index][UNIT_PRICE].toFixed(2));
            }
            else {
                $scope.childModels[index][CALC_UNIT_PRICE] = $scope.childModels[index][UNIT_PRICE];
            }
            $scope.childModels[index][CALC_QUANTITY] = $scope.childModels[index][QUANTITY];

            if ($scope.DocumentName == 'SA_SALES_ORDER') {
                var referenceItemCode = "";
                var referenceSerialNo = "";
                var referenceQuantity = "";
                var referenceVoucherNo = "";
                var childModelItemCode = "";
                var childModelSerialNo = "";
                var childModelQuantity = "";
                var childModelVoucherNo = "";

                if ($scope.CheckReferenceQuantity.length >= 1) {
                    for (var i = 0; i < $scope.CheckReferenceQuantity.length; i++) {
                        referenceItemCode = $scope.CheckReferenceQuantity[i][ITEM_CODE];
                        referenceSerialNo = $scope.CheckReferenceQuantity[i][SERIAL_NO];
                        referenceQuantity = $scope.CheckReferenceQuantity[i][QUANTITY];
                        referenceVoucherNo = $scope.CheckReferenceQuantity[i][BLANKET_NO];

                        for (var j = 0; j < $scope.childModels.length; j++) {
                            childModelItemCode = $scope.childModels[j][ITEM_CODE];
                            childModelSerialNo = $scope.childModels[j][REFERENCE_SERIAL_NO];
                            childModelQuantity = $scope.childModels[j][QUANTITY];
                            childModelVoucherNo = $scope.childModels[j][REFERENCE_VOUCHER_NO];
                            if (referenceItemCode == childModelItemCode && referenceSerialNo == childModelSerialNo && referenceVoucherNo == childModelVoucherNo) {
                                debugger;
                                if (childModelQuantity > referenceQuantity) {
                                    displayPopupNotification("This is Referenced Item so its quantity cannot be bigger then Referenced Quantity", "warning");
                                    $scope.childModels[j][QUANTITY] = "";
                                }
                            }
                        }
                    }
                }
            }

            if ($scope.childModels[index][UNIT_PRICE] === 0 || $scope.childModels[index][UNIT_PRICE] === "" || $scope.childModels[index][QUANTITY] === 0 || $scope.childModels[index][QUANTITY] === "" || $scope.childModels[index][UNIT_PRICE] === null || $scope.childModels[index][QUANTITY] === null) {
                if ($scope.childModels[index][UNIT_PRICE] === 0 || $scope.childModels[index][UNIT_PRICE] === "" || $scope.childModels[index][UNIT_PRICE] === null) {
                    $scope.childModels[index][TOTAL_PRICE] = "";
                }
                if ($scope.childModels[index][QUANTITY] === 0 || $scope.childModels[index][QUANTITY] === "" || $scope.childModels[index][QUANTITY] === null) {

                }
                $scope.childModels[index][TOTAL_PRICE] = 0;
                $scope.childModels[index][CALC_TOTAL_PRICE] = 0;
            }

            var unit_price = $scope.childModels[index][UNIT_PRICE];
            var quantity = $scope.childModels[index][QUANTITY];
            debugger;
            if (unit_price !== undefined && quantity !== undefined && unit_price !== null && quantity !== null && unit_price !== "" && quantity !== "") {
                var total_price = $scope.childModels[index][UNIT_PRICE] * $scope.childModels[index][QUANTITY];
                $scope.childModels[index][PRICE] = parseFloat(total_price.toFixed(2));
                $scope.childModels[index][TOTAL_PRICE] = parseFloat(total_price.toFixed(2));
                $scope.childModels[index][CALC_UNIT_PRICE] = $scope.childModels[index][UNIT_PRICE];
            }
            $scope.GrandtotalCalution();

            if (typeof $scope.childModels[index][CALC_UNIT_PRICE] !== "undefined" && typeof $scope.childModels[index][CALC_QUANTITY] !== "undefined" && $scope.childModels[index][CALC_UNIT_PRICE] != null && $scope.childModels[index][CALC_QUANTITY] != null) {
                var cal_price = $scope.childModels[index][CALC_UNIT_PRICE] * $scope.childModels[index][CALC_QUANTITY];
                $scope.childModels[index][CALC_TOTAL_PRICE] = parseFloat(cal_price.toFixed(2));

                var calcsum = 0;
                angular.forEach($scope.childModels, function (value, key) {
                    debugger;
                    if (typeof value[CALC_TOTAL_PRICE] !== 'undefined' && value[CALC_TOTAL_PRICE] != null) {
                        //console.log('value', value);
                        calcsum = calcsum + parseFloat(value[CALC_TOTAL_PRICE].toFixed(2));
                    }
                });
            }
            $scope.muwiseQty();

            /* $scope.chargeSum(index);*/
        }
    };

    // Cache object: key = itemCode, value = API result
    $scope.unitCache = {};

    // Column -> index mapping
    var quantityMap = {
        "SECOND_QUANTITY": 0,
        "THIRD_QUANTITY": 1,
        "FOURTH_QUANTITY": 2
    };

    // Generic function to fetch API with cache
    function getUnitData(itemCode) {
        if ($scope.unitCache[itemCode]) {
            // Already cached
            return Promise.resolve($scope.unitCache[itemCode]);
        }
        return $http.get("/api/TemplateApi/GetItemUnit?itemCode=" + itemCode)
            .then(function (res) {
                $scope.unitCache[itemCode] = res.data; // Store in cache
                return res.data;
            });
    }

    $scope.calculateQuantity = function (index, columnName) {
        var itemCode = $scope.childModels[index].ITEM_CODE;
        if (!itemCode) return;

        getUnitData(itemCode).then(function (unitData) {
            var itemSerialNo = quantityMap[columnName];
            var data = unitData[itemSerialNo];
            if (!data) return;

            var columnQuantityData = $scope.childModels[index][columnName];

            // Update base QUANTITY
            $scope.childModels[index].QUANTITY =
                columnQuantityData * data.CONVERSION_FACTOR * data.FRACTION;

            // Now recalc other quantities (fetches from cache itself)
            $scope.calculateChargeSum(index);
        });
    };


    //$scope.calculateQuantity = function (index) {
    //    var itemCode = $scope.childModels[index].ITEM_CODE;
    //    if (!itemCode) return;

    //    var req = "/api/TemplateApi/GetItemUnit?itemCode=" + itemCode;
    //    $http.get(req).then(function (results) {
    //        var itemSerialNo = quantityMap[columnName];
    //        if (itemSerialNo === undefined) return; // invalid columnName

    //        var data = results.data[itemSerialNo];
    //        if (!data) return;

    //        var columnQuantityData = $scope.childModels[index][columnName];
    //        $scope.childModels[index].QUANTITY =
    //            columnQuantityData * data.CONVERSION_FACTOR * data.FRACTION;

    //        $scope.calculateChargeSum(index); // pass results for reuse
    //    });
    //};

    //$scope.calculateQuantity = function (index, columnName) {
    //    debugger;
    //    var itemCode = $scope.childModels[index].ITEM_CODE;
    //    if (itemCode) {
    //        var req = "/api/TemplateApi/GetItemUnit?itemCode=" + itemCode;
    //        $http.get(req).then(function (results) {
    //            var columnQuantityData = $scope.childModels[index][columnName];

    //            var itemSerialNo = 0;

    //            if (columnName === "SECOND_QUANTITY") {
    //                itemSerialNo = 0;
    //            } else if (columnName === "THIRD_QUANTITY") {
    //                itemSerialNo = 1;
    //            } else if (columnName === "FOURTH_QUANTITY") {
    //                itemSerialNo = 2;
    //            }

    //            var serialNo = results.data[itemSerialNo].SERIAL_NO;
    //            var fraction = results.data[itemSerialNo].FRACTION;
    //            var conversionFactor = results.data[itemSerialNo].CONVERSION_FACTOR;


    //            if ($scope.childModels[0].hasOwnProperty("SECOND_QUANTITY") && itemSerialNo === 0) {
    //                if (results.data.length > 0) {
    //                    $scope.childModels[index].QUANTITY = columnQuantityData * conversionFactor * fraction;
    //                }
    //            }
    //            $scope.calculateChargeSum(index);
    //        });
    //    }
    //}

    $scope.calculateChargeSum = function (index) {
        debugger;
        var itemCode = $scope.childModels[index].ITEM_CODE;

        if (itemCode) {
            getUnitData(itemCode).then(function (unitData) {
                var quantity = $scope.childModels[index].QUANTITY;

                Object.keys(quantityMap).forEach(function (columnName) {
                    var itemSerialNo = quantityMap[columnName];
                    var data = unitData[itemSerialNo];

                    if (data && $scope.childModels[index].hasOwnProperty(columnName)) {
                        $scope.childModels[index][columnName] =
                            (quantity / data.FRACTION) / data.CONVERSION_FACTOR;
                    }
                });
            });
        }

        //if (itemCode) {
        //    var req = "/api/TemplateApi/GetItemUnit?itemCode=" + itemCode;
        //    $http.get(req).then(function (results) {
        //        debugger;
        //        var quantity = $scope.childModels[index].QUANTITY;
        //        if ($scope.childModels[0].hasOwnProperty("SECOND_QUANTITY")) {
        //            if (results.data.length > 0) {
        //                var serialNo = results.data[0].SERIAL_NO;
        //                var fraction = results.data[0].FRACTION;
        //                var conversionFactor = results.data[0].CONVERSION_FACTOR;
        //                if (serialNo == 1) {
        //                    $scope.childModels[index].SECOND_QUANTITY = (quantity / fraction) / conversionFactor;
        //                }
        //            }
        //        }
        //        if ($scope.childModels[0].hasOwnProperty("THIRD_QUANTITY")) {
        //            if (results.data.length > 1) {
        //                var serialNo = results.data[1].SERIAL_NO;
        //                var fraction = results.data[1].FRACTION;
        //                var conversionFactor = results.data[1].CONVERSION_FACTOR;
        //                if (serialNo == 2) {
        //                    $scope.childModels[index].THIRD_QUANTITY = (quantity / fraction) / conversionFactor;
        //                }
        //            }
        //        }
        //        if ($scope.childModels[0].hasOwnProperty("FOURTH_QUANTITY")) {
        //            if (results.data.length > 2) {
        //                var serialNo = results.data[2].SERIAL_NO;
        //                var fraction = results.data[2].FRACTION;
        //                var conversionFactor = results.data[1].CONVERSION_FACTOR;
        //                if (serialNo == 3) {
        //                    $scope.childModels[index].FOURTH_QUANTITY = (quantity / fraction) / conversionFactor;
        //                }
        //            }
        //        }
        //    });
        //}
        $scope.iamFromReference = false;
        if (!$scope.iamFromReference) {
            var child_rate = $scope.childModels[index].UNIT_PRICE;
            var quantity = $scope.childModels[index].QUANTITY;
            if (child_rate != null && child_rate != "" && child_rate !== undefined) {
                $scope.childModels[index].UNIT_PRICE = parseFloat(child_rate.toFixed(2));
            }
            //quantity validation start
            if (quantity === undefined) {
                $(".QUANTITY_" + index).parent().css({ "border": "solid 1px red" });

            }
            else {
                $(".QUANTITY_" + index).parent().css({ "border": "none" });
                $(".CALC_QUANTITY_" + index).parent().css({ "border": "none" });
            }

            //price validation start
            if (child_rate === undefined) {
                $(".UNIT_PRICE_" + index).parent().css({ "border": "solid 1px red" });

            }
            else {
                $(".UNIT_PRICE_" + index).parent().css({ "border": "none" });
                $(".CALC_UNIT_PRICE_" + index).parent().css({ "border": "none" });
            }

            if (child_rate === undefined && quantity === undefined) {
                $scope.childModels[index].CALC_UNIT_PRICE = "";
                $scope.childModels[index].CALC_QUANTITY = "";
                return;
            }
            //for stock quantity
            $scope.checkitemstock(index);



            //validation end


            if (typeof $scope.childModels[index][UNIT_PRICE] !== "undefined" && $scope.childModels[index][UNIT_PRICE] != null && $scope.childModels[index][UNIT_PRICE] != "") {
                $scope.childModels[index][CALC_UNIT_PRICE] = parseFloat($scope.childModels[index][UNIT_PRICE].toFixed(2));
            }
            else {
                $scope.childModels[index][CALC_UNIT_PRICE] = $scope.childModels[index][UNIT_PRICE];
            }
            $scope.childModels[index][CALC_QUANTITY] = $scope.childModels[index][QUANTITY];

            if ($scope.DocumentName == 'SA_SALES_ORDER') {
                var referenceItemCode = "";
                var referenceSerialNo = "";
                var referenceQuantity = "";
                var referenceVoucherNo = "";
                var childModelItemCode = "";
                var childModelSerialNo = "";
                var childModelQuantity = "";
                var childModelVoucherNo = "";

                if ($scope.CheckReferenceQuantity.length >= 1) {
                    for (var i = 0; i < $scope.CheckReferenceQuantity.length; i++) {
                        referenceItemCode = $scope.CheckReferenceQuantity[i][ITEM_CODE];
                        referenceSerialNo = $scope.CheckReferenceQuantity[i][SERIAL_NO];
                        referenceQuantity = $scope.CheckReferenceQuantity[i][QUANTITY];
                        referenceVoucherNo = $scope.CheckReferenceQuantity[i][BLANKET_NO];

                        for (var j = 0; j < $scope.childModels.length; j++) {
                            childModelItemCode = $scope.childModels[j][ITEM_CODE];
                            childModelSerialNo = $scope.childModels[j][REFERENCE_SERIAL_NO];
                            childModelQuantity = $scope.childModels[j][QUANTITY];
                            childModelVoucherNo = $scope.childModels[j][REFERENCE_VOUCHER_NO];
                            if (referenceItemCode == childModelItemCode && referenceSerialNo == childModelSerialNo && referenceVoucherNo == childModelVoucherNo) {
                                debugger;
                                if (childModelQuantity > referenceQuantity) {
                                    displayPopupNotification("This is Referenced Item so its quantity cannot be bigger then Referenced Quantity", "warning");
                                    $scope.childModels[j][QUANTITY] = "";
                                }
                            }
                        }
                    }
                }
            }

            if ($scope.childModels[index][UNIT_PRICE] === 0 || $scope.childModels[index][UNIT_PRICE] === "" || $scope.childModels[index][QUANTITY] === 0 || $scope.childModels[index][QUANTITY] === "" || $scope.childModels[index][UNIT_PRICE] === null || $scope.childModels[index][QUANTITY] === null) {
                if ($scope.childModels[index][UNIT_PRICE] === 0 || $scope.childModels[index][UNIT_PRICE] === "" || $scope.childModels[index][UNIT_PRICE] === null) {
                    $scope.childModels[index][TOTAL_PRICE] = "";
                }
                if ($scope.childModels[index][QUANTITY] === 0 || $scope.childModels[index][QUANTITY] === "" || $scope.childModels[index][QUANTITY] === null) {

                }
                $scope.childModels[index][TOTAL_PRICE] = 0;
                $scope.childModels[index][CALC_TOTAL_PRICE] = 0;
            }

            var unit_price = $scope.childModels[index][UNIT_PRICE];
            var quantity = $scope.childModels[index][QUANTITY];
            debugger;
            if (unit_price !== undefined && quantity !== undefined && unit_price !== null && quantity !== null && unit_price !== "" && quantity !== "") {
                var total_price = $scope.childModels[index][UNIT_PRICE] * $scope.childModels[index][QUANTITY];
                $scope.childModels[index][PRICE] = parseFloat(total_price.toFixed(2));
                $scope.childModels[index][TOTAL_PRICE] = parseFloat(total_price.toFixed(2));
                $scope.childModels[index][CALC_UNIT_PRICE] = $scope.childModels[index][UNIT_PRICE];


                if (typeof $scope.childModels[index][CALC_UNIT_PRICE] !== "undefined" && typeof $scope.childModels[index][CALC_QUANTITY] !== "undefined" && $scope.childModels[index][CALC_UNIT_PRICE] != null && $scope.childModels[index][CALC_QUANTITY] != null) {
                    var cal_price = $scope.childModels[index][CALC_UNIT_PRICE] * $scope.childModels[index][CALC_QUANTITY];
                    $scope.childModels[index][CALC_TOTAL_PRICE] = parseFloat(cal_price.toFixed(2));

                    var calcsum = 0;
                    angular.forEach($scope.childModels, function (value, key) {
                        debugger;
                        if (typeof value[CALC_TOTAL_PRICE] !== 'undefined' && value[CALC_TOTAL_PRICE] != null) {
                            //console.log('value', value);
                            calcsum = calcsum + parseFloat(value[CALC_TOTAL_PRICE].toFixed(2));
                        }
                    });
                }

                $scope.chargeSum(index);
            }

            //$scope.GrandtotalCalution();

            $scope.muwiseQty();

        }
    };

    ///----Nila changes to calculate Line Wise Calculation Starts-------------///////////////

    $scope.chargeSum = function (index) {
        debugger;
        var quantity = $scope.childModels[index][QUANTITY];
        var unit_price = $scope.childModels[index][UNIT_PRICE];
        var itemCode = $scope.childModels[index][ITEM_CODE];
        var chargeList = _.sortBy($scope.ChargeList, 'PRIORITY_INDEX_NO');
        var lineWiseChargeList = $scope.lineWiseChargeList;
        let minPriority = chargeList.length
            ? Math.min(...chargeList.map(c => c.PRIORITY_INDEX_NO))
            : null;
        $scope.data = chargeList.filter(item => item.APPLY_ON === 'C');
        $scope.lineWiseData = lineWiseChargeList;
        if (quantity > 0 && unit_price > 0) {
            $.each(chargeList, function (i, val) {
                debugger;
                /////----------If the column have type for Charge then variable columnName will be that type else ""-------------///////////
                if (val.CHARGE_CODE == 'SD') {
                    columnName = "DISCOUNT_CLASS";

                    columnAmount = "DISCOUNT_AMOUNT";
                }
                else if (val.CHARGE_CODE == 'ED') {
                    columnName = "";
                    columnAmount = "EXCISE_DUTY_AMOUNT";
                }
                else if (val.CHARGE_CODE == 'LT') {
                    columnName = "";
                    columnAmount = "LUXURY_TAX_AMOUNT";
                }
                else if (val.CHARGE_CODE == 'VT') {
                    columnName = "";
                    columnAmount = "VAT_AMOUNT";
                }
                else {
                    columnName = "";
                    columnAmount = "";
                }

                var vpa = val.VALUE_PERCENT_AMOUNT;
                var total_price = $scope.childModels[index][TOTAL_PRICE];
                var columnValue = $scope.childModels[index][columnName];
                var nonVatFlag = $scope.childModels[index][NON_VAT_FLAG];

                if (minPriority == val.PRIORITY_INDEX_NO) {
                    total_price = $scope.childModels[index][UNIT_PRICE] * $scope.childModels[index][QUANTITY];
                }

                if (columnValue != "" && columnValue != undefined) {
                    vpa = $scope.childModels[index][columnName];
                }
                else if (vpa <= 0 && columnValue != "") {
                    vpa = $scope.childModels[index][columnName];
                }
                $scope.lineWiseChargeList[index].find(c => c.CHARGE_CODE === val.CHARGE_CODE).VALUE_PERCENT_AMOUNT = vpa;

                const isVatApplicable = (val.CHARGE_CODE !== 'VT' && val.APPLY_ON === 'C') || (val.CHARGE_CODE === 'VT' && nonVatFlag === 'N' && val.APPLY_ON === 'C');

                if (isVatApplicable) {
                    lineWiseCalculation(index, columnName, columnAmount, val.CHARGE_CODE, val.VALUE_PERCENT_FLAG, vpa, val.CHARGE_TYPE_FLAG, val.CHARGE_AMOUNT, total_price, nonVatFlag);
                }
                //chargeAmountCalculation(columnName, columnAmount, val, val.CHARGE_CODE, val.VALUE_PERCENT_FLAG, vpa, val.CHARGE_TYPE_FLAG, val.CHARGE_AMOUNT, total_price);

            });

            $scope.GrandtotalLineWiseCalculation();
        }

    }


    $scope.chargeSumForReference = function (index) {
        debugger;
        var quantity = $scope.childModels[index][QUANTITY];
        var unit_price = $scope.childModels[index][UNIT_PRICE];
        var chargeList = _.sortBy($scope.ChargeList, 'PRIORITY_INDEX_NO');
        var lineWiseChargeList = $scope.lineWiseChargeList;
        let minPriority = chargeList.length
            ? Math.min(...chargeList.map(c => c.PRIORITY_INDEX_NO))
            : null;
        $scope.data = chargeList.filter(item => item.APPLY_ON === 'C');
        $scope.lineWiseData = lineWiseChargeList;
        if (quantity > 0 && unit_price > 0) {
            $.each(chargeList, function (i, val) {
                /////----------If the column have type for Charge then variable columnName will be that type else ""-------------///////////
                if (val.CHARGE_CODE == 'SD') {
                    columnName = "DISCOUNT_CLASS";

                    columnAmount = "DISCOUNT_AMOUNT";
                }
                else if (val.CHARGE_CODE == 'ED') {
                    columnName = "";
                    columnAmount = "EXCISE_DUTY_AMOUNT";
                }
                else if (val.CHARGE_CODE == 'LT') {
                    columnName = "";
                    columnAmount = "LUXURY_TAX_AMOUNT";
                }
                else if (val.CHARGE_CODE == 'VT') {
                    columnName = "";
                    columnAmount = "VAT_AMOUNT";
                }
                else {
                    columnName = "";
                    columnAmount = "";
                }

                var vpa = val.VALUE_PERCENT_AMOUNT;
                var total_price = $scope.childModels[index][TOTAL_PRICE];
                var columnValue = $scope.childModels[index][columnName];
                var nonVatFlag = $scope.childModels[index][NON_VAT_FLAG];

                if (minPriority == val.PRIORITY_INDEX_NO) {
                    total_price = $scope.childModels[index][UNIT_PRICE] * $scope.childModels[index][QUANTITY];
                }

                if (columnValue != "" && columnValue != undefined) {
                    vpa = $scope.childModels[index][columnName];
                }
                else if (vpa <= 0 && columnValue != "") {
                    vpa = $scope.childModels[index][columnName];
                }
                $scope.lineWiseChargeList[index].find(c => c.CHARGE_CODE === val.CHARGE_CODE).VALUE_PERCENT_AMOUNT = vpa;


                const isVatApplicable = (val.CHARGE_CODE !== 'VT' && val.APPLY_ON === 'C') || (val.CHARGE_CODE === 'VT' && nonVatFlag === 'N' && val.APPLY_ON === 'C');

                if (isVatApplicable) {
                    lineWiseCalculation(index, columnName, columnAmount, val.CHARGE_CODE, val.VALUE_PERCENT_FLAG, vpa, val.CHARGE_TYPE_FLAG, val.CHARGE_AMOUNT, total_price, nonVatFlag);
                }
            });


        }

    }

    function lineWiseCalculation(index, columnName, columnAmount, chargeCode, valuePerncentFlag, vpa, ctf, chargeAmount, totalPrice, nonVatFlag) {
        if (valuePerncentFlag == 'P') {
            lineWiseCalculationByVPFByPercentage(index, columnName, columnAmount, chargeCode, vpa, ctf, chargeAmount, totalPrice, nonVatFlag);
        }
        else if (valuePerncentFlag == 'V') {
            lineWiseCalculationByVPFByValue(index, columnName, columnAmount, chargeCode, vpa, ctf, chargeAmount, totalPrice, nonVatFlag);
        }
        else if (valuePerncentFlag == 'Q') {
            lineWiseCalculationByVPFByQuantity(index, columnName, columnAmount, chargeCode, vpa, ctf, chargeAmount, totalPrice, nonVatFlag);
        }
    }

    function lineWiseCalculationByVPFByPercentage(index, columnName, columnAmount, chargeCode, vpa, ctf, chargeAmount, totalPrice, nonVatFlag) {
        var typeAmount = $scope.childModels[index][columnName];
        var quantity = $scope.childModels[index][QUANTITY];
        var unit_price = $scope.childModels[index][UNIT_PRICE];
        if (unit_price !== undefined && quantity !== undefined && unit_price !== null && quantity !== null && unit_price !== "" && quantity !== "") {
            var total_price = 0;
            var amount = parseFloat((totalPrice / 100) * vpa);
            $scope.childModels[index][columnAmount] = parseFloat(amount.toFixed(2));
            if (ctf == 'D') {
                total_price = parseFloat(totalPrice - amount);
            }
            else {
                total_price = parseFloat(totalPrice + amount);
            }
            setTotalPrice(index, total_price);
        }
    }

    function lineWiseCalculationByVPFByValue(index, columnName, columnAmount, chargeCode, vpa, ctf, chargeAmount, totalPrice, nonVatFlag) {

        var typeAmount = $scope.childModels[index][columnName];
        var quantity = $scope.childModels[index][QUANTITY];
        var unit_price = $scope.childModels[index][UNIT_PRICE];
        $scope.childModels[index][columnAmount] = vpa;

        if (unit_price !== undefined && quantity !== undefined && unit_price !== null && quantity !== null && unit_price !== "" && quantity !== "") {
            var total_price = 0;
            if (ctf == 'D') {
                total_price = totalPrice - vpa;
            }
            else {
                total_price = totalPrice + vpa;
            }
            setTotalPrice(index, total_price);

        }

    }

    function lineWiseCalculationByVPFByQuantity(index, columnName, columnAmount, chargeCode, vpa, ctf, chargeAmount, totalPrice, nonVatFlag) {

        var typeAmount = $scope.childModels[index][columnName];
        var quantity = $scope.childModels[index][QUANTITY];
        var unit_price = $scope.childModels[index][UNIT_PRICE];

        if (unit_price !== undefined && quantity !== undefined && unit_price !== null && quantity !== null && unit_price !== "" && quantity !== "") {
            var amount = quantity * vpa;
            $scope.childModels[index][columnAmount] = parseFloat(amount.toFixed(2));
            var total_price = 0;
            if (ctf == 'D') {
                total_price = totalPrice - amount;
            }
            else {
                var total_price = totalPrice + amount;
            }
            setTotalPrice(index, total_price);
        }
    }

    function setTotalPrice(index, total_price) {

        if (typeof $scope.childModels[index][CALC_UNIT_PRICE] !== "undefined" && typeof $scope.childModels[index][CALC_QUANTITY] !== "undefined" && $scope.childModels[index][CALC_UNIT_PRICE] != null && $scope.childModels[index][CALC_QUANTITY] != null) {

            $scope.childModels[index][CALC_TOTAL_PRICE] = parseFloat(total_price.toFixed(2));
        }
        $scope.childModels[index][TOTAL_PRICE] = parseFloat(total_price.toFixed(2));
    }

    function chargeAmountCalculation(columnName, columnAmount, chargeValue, chargeCode, valuePerncentFlag, vpa, ctf, chargeAmount, totalPrice) {
        debugger;
        var totalAddition = 0;
        var totalDeduction = 0;
        var netTotal = 0;
        var childModelsChargeAmountTotal = 0;
        $.each($scope.childModels, function (i, val) {
            debugger;
            var childModelsChargeAmount = $scope.childModels[i][columnAmount];
            var quantity = $scope.childModels[i][QUANTITY];
            var unit_price = $scope.childModels[i][UNIT_PRICE];
            if (unit_price !== undefined && quantity !== undefined && unit_price !== null && quantity !== null && unit_price !== "" && quantity !== "") {
                childModelsChargeAmountTotal += childModelsChargeAmount;
            }

        });
        chargeValue.CHARGE_AMOUNT = childModelsChargeAmountTotal;
    }

    ///----Nila changes to calculate Line Wise Calculation Ends-------------///////////////

    $scope.sumWeightCalculation = function (index) {
        debugger;
        var item_code = $scope.childModels[index].ITEM_CODE;
        var gross_weight = $scope.childModels[index].GROSS_WEIGHT;
        var less_stone = $scope.childModels[index].LESS_STONE;

        //if (gross_weight == "" || less_stone == "" || gross_weight == undefined || less_stone == undefined || gross_weight == null || less_stone == null) {
        //    if (gross_weight == "" || gross_weight == undefined || gross_weight == null) {
        //        $scope.childModels[index][GROSS_WEIGHT] = 0;
        //    }
        //    if (less_stone == "" || less_stone == undefined || less_stone == null) {
        //        $scope.childModels[index][LESS_STONE] = 0;
        //    }

        //    $scope.childModels[index][NET_WEIGHT] = 0;
        //}


        if (gross_weight == null || gross_weight == undefined || gross_weight == "") {

            $scope.childModels[index][NET_WEIGHT] = null;
            $scope.childModels[index][TOTAL_WEIGHT] = null;
            $scope.childModels[index][LESS_STONE] = null;
            $scope.childModels[index][WASTAGE] = null;
            $scope.sumTotalWeightCalculation(index);
            $scope.sumSalesInvoiceJewlleryCalculation(index);



        }

        else if (gross_weight !== undefined && gross_weight !== null && gross_weight !== "") {

            if (gross_weight < less_stone) {
                displayPopupNotification("Less Stone cannot be more then Gross Wt.", "warning");
                $scope.childModels[index][LESS_STONE] = null;
            }
            else {
                var net_weight = $scope.childModels[index][GROSS_WEIGHT] - $scope.childModels[index][LESS_STONE];
                $scope.childModels[index][NET_WEIGHT] = parseFloat(net_weight.toFixed(3));
            }
            $scope.sumTotalWeightCalculation(index);
            $scope.sumSalesInvoiceJewlleryCalculation(index);
        }


    }


    $scope.sumTotalWeightCalculation = function (index) {
        debugger;
        var net_weight = $scope.childModels[index].NET_WEIGHT;
        var wastage = $scope.childModels[index].WASTAGE;
        var isString = false;
        //if (net_weight == "" || wastage == "" || net_weight == undefined || wastage == undefined || net_weight == null || wastage == null) {
        //    if (gross_weight == "" || gross_weight == undefined || gross_weight == null) {
        //        $scope.childModels[index][GROSS_WEIGHT] = 0;
        //    }
        //    if (less_stone == "" || less_stone == undefined || less_stone == null) {
        //        $scope.childModels[index][LESS_STONE] = 0;
        //    }

        //    $scope.childModels[index][TOTAL_WEIGHT] = 0;
        //}
        if (net_weight == null || net_weight == undefined || net_weight == "") {
            $scope.childModels[index][TOTAL_WEIGHT] = null;
            $scope.childModels[index][WASTAGE] = null;
            $scope.sumSalesInvoiceJewlleryCalculation(index);
        }
        if (net_weight !== undefined && net_weight !== null && net_weight !== "") {

            //if (net_weight < wastage) {
            //    displayPopupNotification("Wastage cannot be more then Net Wt.", "warning");
            //    $scope.childModels[index][TOTAL_WEIGHT] = null;
            //    $scope.childModels[index][WASTAGE] = null;

            //}
            //else {
            //    var total_weight = $scope.childModels[index][NET_WEIGHT] - $scope.childModels[index][WASTAGE];
            //    $scope.childModels[index][TOTAL_WEIGHT] = parseFloat(total_weight.toFixed(2));
            //}

            if ($scope.childModels[index].WASTAGE == "") {
                isString = true;
                $scope.childModels[index].WASTAGE = 0;
            }

            var total_weight = $scope.childModels[index][NET_WEIGHT] + $scope.childModels[index][WASTAGE];
            $scope.childModels[index][TOTAL_WEIGHT] = parseFloat(total_weight.toFixed(3));
            $scope.sumSalesInvoiceJewlleryCalculation(index);

            if (isString) {
                $scope.childModels[index].WASTAGE = "";
            }

        }


    }

    $scope.sumSalesInvoiceJewlleryCalculation = function (index) {
        debugger;
        $scope.iamFromReference = false;
        if (!$scope.iamFromReference) {
            var child_rate = $scope.childModels[index].UNIT_PRICE;
            var total_weight = $scope.childModels[index].TOTAL_WEIGHT;

            if (child_rate != null && child_rate != "" && child_rate !== undefined) {
                $scope.childModels[index].UNIT_PRICE = parseFloat(child_rate.toFixed(2));
            }
            //total_weight validation start
            if (total_weight === undefined) {
                $(".TOTAL_WEIGHT_" + index).parent().css({ "border": "solid 1px red" });

            }
            else {
                $(".TOTAL_WEIGHT_" + index).parent().css({ "border": "none" });

            }

            //price validation start
            if (child_rate === undefined) {
                $(".UNIT_PRICE_" + index).parent().css({ "border": "solid 1px red" });

            }
            else {
                $(".UNIT_PRICE_" + index).parent().css({ "border": "none" });

            }

            //if (child_rate === undefined && quantity === undefined) {
            //    $scope.childModels[index].CALC_UNIT_PRICE = "";
            //    $scope.childModels[index].CALC_QUANTITY = "";
            //    return;
            //}
            //for stock quantity
            $scope.checkitemstock(index);



            //validation end


            //if (typeof $scope.childModels[index][UNIT_PRICE] !== "undefined" && $scope.childModels[index][UNIT_PRICE] != null && $scope.childModels[index][UNIT_PRICE] != "") {
            //    $scope.childModels[index][CALC_UNIT_PRICE] = parseFloat($scope.childModels[index][UNIT_PRICE].toFixed(2));
            //}
            //else {
            //    $scope.childModels[index][CALC_UNIT_PRICE] = $scope.childModels[index][UNIT_PRICE];
            //}
            //$scope.childModels[index][CALC_QUANTITY] = $scope.childModels[index][QUANTITY];
            debugger;

            if ($scope.childModels[index][UNIT_PRICE] === 0 || $scope.childModels[index][UNIT_PRICE] === "" || $scope.childModels[index][TOTAL_WEIGHT] === 0 || $scope.childModels[index][TOTAL_WEIGHT] === "" || $scope.childModels[index][UNIT_PRICE] === null || $scope.childModels[index][TOTAL_WEIGHT] === null) {
                if ($scope.childModels[index][UNIT_PRICE] === 0 || $scope.childModels[index][UNIT_PRICE] === "" || $scope.childModels[index][UNIT_PRICE] === null) {
                    $scope.childModels[index][TOTAL_PRICE] = "";
                }
                if ($scope.childModels[index][TOTAL_WEIGHT] === 0 || $scope.childModels[index][TOTAL_WEIGHT] === "" || $scope.childModels[index][TOTAL_WEIGHT] === null) {

                }
                $scope.childModels[index][TOTAL_PRICE] = null;
                $scope.childModels[index][NET_AMOUNT] = null;
                $scope.childModels[index][AMOUNT] = null;
            }

            //var making = $scope.childModels[index].MAKING;
            //var stone_amount = $scope.childModels[index].STONE_AMOUNT;
            //var diamond_amount = $scope.childModels[index].DIAMOND_AMOUNT;
            debugger;
            var unit_price = $scope.childModels[index][UNIT_PRICE];
            var weight = $scope.childModels[index][TOTAL_WEIGHT];
            var making = $scope.childModels[index][MAKING];
            var stone_amount = $scope.childModels[index][STONE_AMOUNT];
            var diamond_amount = $scope.childModels[index][DIAMOND_AMOUNT];


            ////---- checking if MAKING STONE_AMOUNT AND DIAMOND_AMOUNT IS GIVING A STRING VALUE "" IF YES THEN MAKE THEIR VALUE 0-----------////////////
            if (making == "" || stone_amount == "" || diamond_amount == "" || making == undefined || stone_amount == undefined || diamond_amount == undefined || making == null || stone_amount == null || diamond_amount == null) {
                if (making == "" || making == undefined || making == null) {
                    $scope.childModels[index][MAKING] = null;
                }
                if (stone_amount == "" || stone_amount == undefined || stone_amount == null) {
                    $scope.childModels[index][STONE_AMOUNT] = null;
                }
                if (diamond_amount == "" || diamond_amount == undefined || diamond_amount == null) {
                    $scope.childModels[index][DIAMOND_AMOUNT] = null;
                }
                $scope.childModels[index][TOTAL_PRICE] = null;
                $scope.childModels[index][NET_AMOUNT] = null;
                $scope.childModels[index][AMOUNT] = null;

            }

            if (unit_price !== undefined && weight !== undefined && unit_price !== null && weight !== null && unit_price !== "" && weight !== "") {

                var total_price = $scope.childModels[index][UNIT_PRICE] * $scope.childModels[index][TOTAL_WEIGHT] + $scope.childModels[index][MAKING] + $scope.childModels[index][STONE_AMOUNT] + $scope.childModels[index][DIAMOND_AMOUNT];
                /*                $scope.childModels[index][TOTAL_PRICE] = parseFloat(total_price.toFixed(2));*/

                $scope.childModels[index][TOTAL_PRICE] = Math.round(total_price);
                $scope.childModels[index][NET_AMOUNT] = Math.round(total_price);
                $scope.childModels[index][TOTAL_ROW_PRICE] = Math.round(total_price);

                //if (making == "" || stone_amount == "" || diamond_amount == "") {
                //    var total_price_amount = $scope.childModels[index][UNIT_PRICE] * $scope.childModels[index][TOTAL_WEIGHT];
                //    $scope.childModels[index][AMOUNT] = parseFloat(total_price_amount.toFixed(2));
                //}


                var total_price_amount = $scope.childModels[index][UNIT_PRICE] * $scope.childModels[index][TOTAL_WEIGHT];

                /*                $scope.childModels[index][AMOUNT] = parseFloat(total_price_amount.toFixed(2));*/

                $scope.childModels[index][AMOUNT] = Math.round(total_price_amount);

                /*$scope.childModels[index][CALC_UNIT_PRICE] = $scope.childModels[index][UNIT_PRICE];*/
            }


            //$scope.GrandtotalCalution();


            if (unit_price !== undefined && unit_price !== null && unit_price !== "") {
                $scope.calculateItemWiseCharge(index);


                $scope.GrandtotalCalculationJewellery();
            }
            //if (typeof $scope.childModels[index][CALC_UNIT_PRICE] !== "undefined" && typeof $scope.childModels[index][CALC_QUANTITY] !== "undefined" && $scope.childModels[index][CALC_UNIT_PRICE] != null && $scope.childModels[index][CALC_QUANTITY] != null) {
            //    var cal_price = $scope.childModels[index][CALC_UNIT_PRICE] * $scope.childModels[index][CALC_QUANTITY];
            //    $scope.childModels[index][CALC_TOTAL_PRICE] = parseFloat(cal_price.toFixed(2));

            //    var calcsum = 0;
            //    angular.forEach($scope.childModels, function (value, key) {

            //        if (typeof value[CALC_TOTAL_PRICE] !== 'undefined' && value[CALC_TOTAL_PRICE] != null) {
            //            //console.log('value', value);
            //            calcsum = calcsum + parseFloat(value[CALC_TOTAL_PRICE].toFixed(2));
            //        }
            //    });
            //}

            //$scope.muwiseQty();
        }
    };
    $scope.calculateItemWiseCharge = function (index) {
        debugger;
        $scope.childModels[index][TOTAL_ROW_PRICE] = $scope.childModels[index][TOTAL_PRICE];

        $.each($scope.newChargeListForJewellery, function (i, val) {
            debugger;
            var totalAddition = 0;
            var totalDeduction = 0;
            var percent_amount = val.VALUE_PERCENT_AMOUNT;
            var total_price = $scope.childModels[index][TOTAL_PRICE];
            var total_amount = $scope.childModels[index][NET_AMOUNT];
            var nonVatFlag = $scope.childModels[index][NON_VAT_FLAG];
            var charge_amount = val.CHARGE_AMOUNT;


            if (percent_amount === null || percent_amount === "" || percent_amount === NaN || percent_amount === undefined) {
                percent_amount = 0;
            }
            if (total_amount === null || total_amount === "" || total_amount === NaN || total_amount === undefined) {
                total_amount = 0;
            }
            if (val.CHARGE_CODE == 'LT') {

                if (val.VALUE_PERCENT_FLAG === 'P') {

                    if (val.CHARGE_TYPE_FLAG == "D") {
                        total_amount = parseFloat($scope.childModels[index][NET_AMOUNT]) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                        var Deduction = parseFloat(((percent_amount * total_amount) / 100).toFixed(2));

                        val.CHARGE_AMOUNT = Deduction;
                        totalDeduction += Deduction;

                    }
                    else {
                        total_amount = parseFloat($scope.childModels[index][NET_AMOUNT]) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                        var Addition = parseFloat(((percent_amount * total_amount) / 100).toFixed(2));

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
                        if (charge_amount > 0) {
                            $scope.PrintDiscount = charge_amount;
                        }


                    }
                }
                if (val.CHARGE_TYPE_FLAG == "D") {
                    $scope.childModels[index][LUXURY_TAX_AMOUNT] = totalDeduction;
                }
                else {
                    $scope.childModels[index][LUXURY_TAX_AMOUNT] = totalAddition;
                }

                //if (nonVatFlag != 'undefined' && nonVatFlag == 'N') {
                //    $scope.childModels[index][TOTAL_PRICE] += Math.round(totalAddition);
                //}

                $scope.childModels[index][NET_AMOUNT] += Math.round(totalAddition);
                //$scope.childModels[index][TOTAL_PRICE] += Math.round(totalAddition);
                $scope.itsync(true, val);
            }
            else if (val.CHARGE_CODE == 'VT' && nonVatFlag != 'undefined' && nonVatFlag == 'N') {

                if (val.VALUE_PERCENT_FLAG === 'P') {

                    if (val.CHARGE_TYPE_FLAG == "D") {
                        total_amount = parseFloat($scope.childModels[index][NET_AMOUNT]) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                        var Deduction = parseFloat(((percent_amount * total_amount) / 100).toFixed(2));

                        val.CHARGE_AMOUNT = Deduction;
                        totalDeduction += Deduction;

                    }
                    else {
                        total_amount = parseFloat($scope.childModels[index][NET_AMOUNT]) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                        var Addition = parseFloat(((percent_amount * total_amount) / 100).toFixed(2));

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
                        if (charge_amount > 0) {
                            $scope.PrintDiscount = charge_amount;
                        }


                    }
                }
                if (val.CHARGE_TYPE_FLAG == "D") {
                    $scope.childModels[index][VAT_AMOUNT] = totalDeduction;
                }
                else {
                    $scope.childModels[index][VAT_AMOUNT] = totalAddition;
                }

                //$scope.childModels[index][TOTAL_PRICE] += Math.round(totalAddition);

                $scope.childModels[index][NET_AMOUNT] += Math.round(totalAddition);
                $scope.itsync(true, val);
            }
            else if (val.CHARGE_CODE == 'SD') {

                if (val.VALUE_PERCENT_FLAG === 'P') {

                    if (val.CHARGE_TYPE_FLAG == "D") {
                        total_amount = parseFloat($scope.childModels[index][NET_AMOUNT]) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                        var Deduction = parseFloat(((percent_amount * total_amount) / 100).toFixed(2));

                        val.CHARGE_AMOUNT = Deduction;
                        totalDeduction += Deduction;

                    }
                    else {
                        total_amount = parseFloat($scope.childModels[index][NET_AMOUNT]) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                        var Addition = parseFloat(((percent_amount * total_amount) / 100).toFixed(2));

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
                        if (charge_amount > 0) {
                            $scope.PrintDiscount = charge_amount;
                        }


                    }
                }

                if (val.CHARGE_TYPE_FLAG == "D") {
                    $scope.childModels[index][DISCOUNT_AMOUNT] = totalDeduction;
                }
                else {
                    $scope.childModels[index][DISCOUNT_AMOUNT] = totalAddition;
                }
                //$scope.childModels[index][TOTAL_PRICE] -= Math.round(totalDeduction);

                $scope.childModels[index][NET_AMOUNT] += Math.round(totalAddition);

                $scope.itsync(true, val);
            }

        });

    }

    $scope.calculateItemWiseChargeReference = function (index) {
        debugger;
        $scope.childModels[index][TOTAL_ROW_PRICE] = $scope.childModels[index][TOTAL_PRICE];

        $.each($scope.ChargeList, function (i, val) {
            debugger;
            var totalAddition = 0;
            var totalDeduction = 0;
            var percent_amount = val.VALUE_PERCENT_AMOUNT;
            var total_price = $scope.childModels[index][TOTAL_PRICE];
            var total_amount = $scope.childModels[index][NET_AMOUNT];
            var nonVatFlag = $scope.childModels[index][NON_VAT_FLAG];
            var charge_amount = val.CHARGE_AMOUNT;


            if (percent_amount === null || percent_amount === "" || percent_amount === NaN || percent_amount === undefined) {
                percent_amount = 0;
            }
            if (total_amount === null || total_amount === "" || total_amount === NaN || total_amount === undefined) {
                total_amount = 0;
            }
            if (total_price === null || total_price === "" || total_price === NaN || total_price === undefined) {
                total_price = 0;
            }
            if (val.CHARGE_CODE == 'LT') {

                if (val.VALUE_PERCENT_FLAG === 'P') {

                    if (val.CHARGE_TYPE_FLAG == "D") {
                        total_amount = parseFloat($scope.childModels[index][TOTAL_PRICE]) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                        var Deduction = parseFloat(((percent_amount * total_amount) / 100).toFixed(2));

                        val.CHARGE_AMOUNT = Deduction;
                        totalDeduction += Deduction;

                    }
                    else {
                        total_amount = parseFloat($scope.childModels[index][TOTAL_PRICE]) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                        var Addition = parseFloat(((percent_amount * total_amount) / 100).toFixed(2));

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
                        if (charge_amount > 0) {
                            $scope.PrintDiscount = charge_amount;
                        }


                    }
                }
                //if (val.CHARGE_TYPE_FLAG == "D") {
                //    $scope.childModels[index][LUXURY_TAX_AMOUNT] = totalDeduction;
                //}
                //else {
                //    $scope.childModels[index][LUXURY_TAX_AMOUNT] = totalAddition;
                //}

                //if (nonVatFlag != 'undefined' && nonVatFlag == 'N') {
                //    $scope.childModels[index][TOTAL_PRICE] += Math.round(totalAddition);
                //}

                //$scope.childModels[index][NET_AMOUNT] += Math.round(totalAddition);
                //$scope.childModels[index][TOTAL_PRICE] += Math.round(totalAddition);
                $scope.ritsync(true, val);
            }
            else if (val.CHARGE_CODE == 'VT' && nonVatFlag != 'undefined' && nonVatFlag == 'N') {

                if (val.VALUE_PERCENT_FLAG === 'P') {

                    if (val.CHARGE_TYPE_FLAG == "D") {
                        total_amount = parseFloat($scope.childModels[index][TOTAL_PRICE]) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                        var Deduction = parseFloat(((percent_amount * total_amount) / 100).toFixed(2));

                        val.CHARGE_AMOUNT = Deduction;
                        totalDeduction += Deduction;

                    }
                    else {
                        total_amount = parseFloat($scope.childModels[index][TOTAL_PRICE]) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                        var Addition = parseFloat(((percent_amount * total_amount) / 100).toFixed(2));

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
                        if (charge_amount > 0) {
                            $scope.PrintDiscount = charge_amount;
                        }


                    }
                }
                //if (val.CHARGE_TYPE_FLAG == "D") {
                //    $scope.childModels[index][VAT_AMOUNT] = totalDeduction;
                //}
                //else {
                //    $scope.childModels[index][VAT_AMOUNT] = totalAddition;
                //}

                //$scope.childModels[index][TOTAL_PRICE] += Math.round(totalAddition);

                //$scope.childModels[index][NET_AMOUNT] += Math.round(totalAddition);
                $scope.ritsync(true, val);
            }
            else if (val.CHARGE_CODE == 'SD') {

                if (val.VALUE_PERCENT_FLAG === 'P') {

                    if (val.CHARGE_TYPE_FLAG == "D") {
                        total_amount = parseFloat($scope.childModels[index][TOTAL_PRICE]) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                        var Deduction = parseFloat(((percent_amount * total_amount) / 100).toFixed(2));

                        val.CHARGE_AMOUNT = Deduction;
                        totalDeduction += Deduction;

                    }
                    else {
                        total_amount = parseFloat($scope.childModels[index][TOTAL_PRICE]) + parseFloat(totalAddition) - parseFloat(totalDeduction);
                        var Addition = parseFloat(((percent_amount * total_amount) / 100).toFixed(2));

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
                        if (charge_amount > 0) {
                            $scope.PrintDiscount = charge_amount;
                        }


                    }
                }

                //if (val.CHARGE_TYPE_FLAG == "D") {
                //    $scope.childModels[index][DISCOUNT_AMOUNT] = totalDeduction;
                //}
                //else {
                //    $scope.childModels[index][DISCOUNT_AMOUNT] = totalAddition;
                //}
                //$scope.childModels[index][TOTAL_PRICE] -= Math.round(totalDeduction);

                //$scope.childModels[index][NET_AMOUNT] += Math.round(totalAddition);

                $scope.ritsync(true, val);
            }

        });

    }

    $scope.checkitemstock = function (index) {

        if ($scope.NEGETIVE_STOCK_FLAG == "N") {

            var itemcode = "";
            var master_orderdate = "";
            var locationcode = "";
            if ($scope.childModels[index][ITEM_CODE] !== "undefined") {
                itemcode = $scope.childModels[index][ITEM_CODE];
            }
            if ($scope.childModels[0].hasOwnProperty("LOCATION_CODE")) {
                if ($scope.childModels[index][LOCATION_CODE] != 'undefined') {
                    locationcode = $scope.childModels[index][LOCATION_CODE];
                }
            }

            if ($scope.masterModels.hasOwnProperty(GetPrimaryDateByTableName($scope.DocumentName))) {


                var primarydatecolname = GetPrimaryDateByTableName($scope.DocumentName);
                if ($scope.masterModels[primarydatecolname] !== "undefined") {
                    master_orderdate = $scope.masterModels[primarydatecolname];
                }
            }

            var itemstk = "/api/TemplateApi/GetStockQuantityOfItem?itemcodecode=" + itemcode + "&voucherdate=" + master_orderdate + "&locationcode=" + locationcode;
            $http.get(itemstk).then(function (response) {

                $scope.itemstockqty = response.data;

                if ($scope.itemstockqty !== "undefined") {
                    if ($scope.childModels[index].QUANTITY > $scope.itemstockqty) {

                        $(".UNIT_PRICE_" + index).parent().css({ "border": "solid 1px red" });
                        return;

                    }
                }

            });
        }
    };

    $scope.totalQty = 0;

    $scope.muwiseQty = function () {
        debugger;
        $scope.units = [];
        $scope.totalQty = 0;
        var totalQty = 0;
        $scope.childModels.forEach(function (item) {
            debugger;
            var qtySum = 0;
            $scope.childModels.forEach(function (it) {
                debugger;
                if (item.MU_CODE == it.MU_CODE) {
                    if (it.QUANTITY !== undefined) {
                        qtySum += it.QUANTITY;
                    }
                }
            });
            var sumquatity = qtySum;

            $scope.units.push({ mu_name: item.MU_CODE, mu_code_value: parseFloat(qtySum).toFixed(4) });
            if (item.QUANTITY !== undefined) {
                debugger;
                totalQty += item.QUANTITY;
                $scope.totalQty = parseFloat(totalQty).toFixed(4);
            }

        });
        $scope.units = _.uniq($scope.units, JSON.stringify);

    }


    $scope.getInfo = function (index, itemCode) {
        $scope.ItemInfo = [];
        $scope.getmuinfo(index, itemCode);
    }

    $scope.getmuinfo = function (index, productId) {
        //console.log(index);

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
                if (!data == "") {
                    $scope.ItemInfo = [];
                    $scope.ItemInfo.push(data.data[0]);
                    $scope.muwiseQty();
                }
                else {

                }
            });
        } catch (e) {
            return;
        }

    };

    $scope.msync = function (bool, item) {
        debugger;
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
    $scope.rsync = function (bool, item) {
        debugger;
        if ($scope.OrderNo === "undefined" && $scope.youFromReference == true) {
            for (var i = 0; i < $scope.data.length; i++) {

                if ($scope.data[i].CHARGE_CODE == item.CHARGE_CODE) {
                    $scope.data[i].CHARGE_AMOUNT = 0;
                    $scope.data.splice(i, 1);

                }
            }
            if (bool)

                $scope.data.push(item);
            $scope.calculateChargeAmountrefrenceedit($scope.data, bool);
        }
    };
    $scope.eesync = function (bool, item) {
        debugger;
        if ($scope.OrderNo != "undefined") {

            for (var i = 0; i < $scope.data.length; i++) {

                if ($scope.data[i].CHARGE_CODE == item.CHARGE_CODE) {


                    $scope.data.splice(i, 1);
                }
            }
            if (bool)
                $scope.data.push(item);

            ///////////------Start---- Issue The Addition such as Luxury Tax is not being added in the Total Amount -------------/////////////

            //$scope.calculateChargeAmounteditedit($scope.data, bool);

            ///////////------End---- Issue The Addition such as Luxury Tax is not being added in the Total Amount -------------/////////////


            ///////////------Start---- Fixing Issue The Addition such as Luxury Tax is not being added in the Total Amount -------------/////////////

            $scope.calculateChargeAmountediteditNew($scope.data, bool);

            ///////////------End---- FixingIssue The Addition such as Luxury Tax is not being added in the Total Amount -------------/////////////


        }
    };
    $scope.eelwsync = function (bool, item) {
        debugger;
        if ($scope.OrderNo !== "undefined") {

            $.each($scope.lineWiseData, function (k, dataChargeArray) {
                debugger;
                let wasRemoved = false;
                let chargeData = "";
                for (var l = dataChargeArray.length - 1; l >= 0; l--) {
                    if (dataChargeArray[l].CHARGE_CODE === item.CHARGE_CODE) {
                        chargeData = dataChargeArray[l];
                        dataChargeArray.splice(l, 1); // Remove the matching item
                        wasRemoved = true;
                    }
                }

                if (wasRemoved && bool) {
                    dataChargeArray.push(chargeData); // Push it back if needed
                }
            });

            $scope.calculateTotalLineWiseChargeAmountEdit($scope.lineWiseData, bool);

        }

    };
    $scope.sync = function (bool, item) {
        debugger;
        if ($scope.OrderNo === "undefined" && $scope.youFromReference == false) {

            for (var i = 0; i < $scope.data.length; i++) {

                if ($scope.data[i].CHARGE_CODE == item.CHARGE_CODE) {
                    debugger;

                    $scope.data.splice(i, 1);

                    //////-----------------Discount and luxury tax after 10L-----------------////////////////////

                    //if (bool == false && item.CHARGE_CODE == "SD" && ($scope.DocumentName == "SA_SALES_INVOICE_JEWEL" || $scope.DocumentName == "SA_SALES_RETURN_JEWEL")) {
                    //    debugger;
                    //    for (let i = $scope.data.length - 1; i >= 0; i--) {

                    //            $scope.data.splice(i, 1);

                    //    }
                    //    $scope.data.splice(i, 1);
                    //    luxuryTaxAddition = true;
                    //}

                    //////-----------------Discount and luxury tax after 10L-----------------////////////////////
                }
            }
            if (bool)
                $scope.data.push(item);
            if ($scope.DocumentName == "SA_SALES_INVOICE_JEWEL" || $scope.DocumentName == "SA_SALES_RETURN_JEWEL") {

                $scope.calculateChargeAmountSalesInvoiceJewellery($scope.data, bool);
            }
            else {
                $scope.calculateChargeAmount($scope.data, bool);
            }
        }
        if ($scope.OrderNo === "undefined" && $scope.youFromReference == true) {
            $scope.rsync(bool, item);
        }
        if ($scope.OrderNo != "undefined" && $scope.youFromReference == false) {
            $scope.msync(bool, item);
        }
    };

    $scope.lwsync = function (bool, item) {
        debugger;
        if ($scope.OrderNo === "undefined" && $scope.youFromReference == false) {


            $.each($scope.lineWiseData, function (k, dataChargeArray) {
                debugger;
                let wasRemoved = false;
                let chargeData = "";
                for (var l = dataChargeArray.length - 1; l >= 0; l--) {
                    if (dataChargeArray[l].CHARGE_CODE === item.CHARGE_CODE) {
                        chargeData = dataChargeArray[l];
                        dataChargeArray.splice(l, 1); // Remove the matching item
                        wasRemoved = true;
                    }
                }

                if (wasRemoved && bool) {
                    dataChargeArray.push(chargeData); // Push it back if needed
                }
            });

            $scope.calculateTotalLineWiseChargeAmount($scope.lineWiseData, bool);

        }
        if ($scope.OrderNo === "undefined" && $scope.youFromReference == true) {
            $scope.lwrsync(bool, item);
        }
        if ($scope.OrderNo != "undefined" && $scope.youFromReference == false) {
            $scope.msync(bool, item);
        }
    };

    $scope.lwrsync = function (bool, item) {
        debugger;
        if ($scope.OrderNo === "undefined" && $scope.youFromReference == true) {

            $.each($scope.lineWiseData, function (k, dataChargeArray) {
                debugger;
                let wasRemoved = false;
                let chargeData = "";
                for (var l = dataChargeArray.length - 1; l >= 0; l--) {
                    if (dataChargeArray[l].CHARGE_CODE === item.CHARGE_CODE) {
                        chargeData = dataChargeArray[l];
                        dataChargeArray.splice(l, 1); // Remove the matching item
                        wasRemoved = true;
                    }
                }

                if (wasRemoved && bool) {
                    dataChargeArray.push(chargeData); // Push it back if needed
                }
            });

            $scope.calculateTotalLineWiseChargeAmountReferenceEdit($scope.lineWiseData, bool);

        }
    };

    $scope.itsync = function (bool, item) {
        debugger;
        if ($scope.OrderNo === "undefined" && $scope.youFromReference == false) {

            for (var i = 0; i < $scope.data.length; i++) {

                if ($scope.data[i].CHARGE_CODE == item.CHARGE_CODE) {
                    debugger;

                    $scope.data.splice(i, 1);

                }
            }
            if (bool)
                $scope.data.push(item);
            if ($scope.DocumentName == "SA_SALES_INVOICE_JEWEL" || $scope.DocumentName == "SA_SALES_RETURN_JEWEL") {

                $scope.newCalculateChargeAmountSalesInvoiceJewellery($scope.data, bool);
            }
        }
    };

    $scope.ritsync = function (bool, item) {
        debugger;
        if ($scope.OrderNo === "undefined" && $scope.youFromReference == true) {

            for (var i = 0; i < $scope.data.length; i++) {

                if ($scope.data[i].CHARGE_CODE == item.CHARGE_CODE) {
                    debugger;

                    $scope.data.splice(i, 1);

                }
            }
            if (bool)
                $scope.data.push(item);
            if ($scope.DocumentName == "SA_SALES_INVOICE_JEWEL" || $scope.DocumentName == "SA_SALES_RETURN_JEWEL") {

                $scope.newCalculateChargeAmountSalesInvoiceJewelleryEditReference($scope.data, bool);
            }
        }
    };

    $scope.eitsync = function (bool, item) {
        debugger;
        if ($scope.OrderNo != "undefined") {
            debugger;
            for (var i = 0; i < $scope.data.length; i++) {
                debugger;
                if ($scope.data[i].CHARGE_CODE == item.CHARGE_CODE) {
                    debugger;

                    $scope.data.splice(i, 1);

                }
            }
            if (bool)
                $scope.data.push(item);
            console.log(typeof $scope.newCalculateChargeAmountSalesInvoiceJewelleryEdit);
            $scope.newCalculateChargeAmountSalesInvoiceJewelleryEdit($scope.data, bool);

        }
    };

    $scope.invsync = function (bool, item) {


        //for (var i = 0; i < $scope.data.length; i++) {
        //    
        //    if ($scope.data[i].CHARGE_CODE == item.CHARGE_CODE) {


        //        $scope.data.splice(i, 1);
        //    }
        //}
        //if (bool)
        //    $scope.data.push(item);

        //$scope.calculateChargeAmount($scope.data, bool);
    };

    //$scope.ChildCalcSum = function (key) {

    //    var sum = 0;
    //    $.each($scope.childModels, function (key, value) {

    //        value.CALC_TOTAL_PRICE = value.CALC_UNIT_PRICE * value.CALC_QUANTITY;
    //        sum += value.TOTAL_PRICE;
    //    });
    //}

    $scope.ChildCalcSum = function (key) {

        var sum = 0;
        var calc_quantity = $scope.childModels[key].CALC_QUANTITY;
        var calc_rate = $scope.childModels[key].CALC_UNIT_PRICE;

        if (calc_rate !== null && calc_rate !== "" && calc_rate !== undefined) {
            $scope.childModels[key].CALC_UNIT_PRICE = parseFloat(calc_rate.toFixed(2));
        }

        if (calc_rate === undefined) {
            $(".CALC_UNIT_PRICE_" + key).parent().css({ "border": "solid 1px red" });

            return;
        }
        else {
            $(".CALC_UNIT_PRICE_" + key).parent().css({ "border": "none" });
        }
        if (calc_quantity === undefined) {

            $(".CALC_QUANTITY_" + key).parent().css({ "border": "solid 1px red" });
            return;
        }
        else {
            $(".CALC_QUANTITY_" + key).parent().css({ "border": "none" });
        }

        if (calc_rate === 0 || calc_rate === "" || calc_quantity === 0 || calc_quantity === "" || calc_rate === null || calc_quantity === null) {
            $scope.childModels[key][CALC_TOTAL_PRICE] = 0;
        }
        else {
            var total_price = calc_rate * calc_quantity;
            $scope.childModels;
            $scope.childModels[key][CALC_TOTAL_PRICE] = parseFloat(total_price.toFixed(2));
        }


        //$.each($scope.childModels, function (key, value) {
        //    ;
        //    var total_cal_value = value.CALC_UNIT_PRICE * value.CALC_QUANTITY;
        //    if (total_cal_value === 0) {
        //        value.CALC_TOTAL_PRICE = value.total_cal_value;
        //    }
        //    else
        //    {
        //        if (total_cal_value !== undefined) {
        //            value.CALC_TOTAL_PRICE = parseFloat(value.total_cal_value.toFixed(2));
        //        }
        //    }
        //   sum += parseFloat(value.CALC_TOTAL_PRICE.toFixed(2));
        //});
    }
    //Sales Grandtotal Calculation
    $scope.GrandtotalCalution = function () {
        debugger;
        var sum = 0;
        angular.forEach($scope.childModels, function (value, key) {

            if (typeof value[TOTAL_PRICE] !== 'undefined' && value[TOTAL_PRICE] !== null && value[TOTAL_PRICE] !== "") {

                //console.log('value', value);
                sum = parseFloat(sum) + (parseFloat(value[TOTAL_PRICE]));
            }
            else
                return;
        });

        $scope.summary.grandTotal = (parseFloat(sum)).toFixed(2);
        if ($scope.data.length < 0 || $scope.data.length == "undefined") {
            $scope.adtotal = $scope.summary.grandTotal;
        }
        else {
            $scope.adtotal = $scope.summary.grandTotal;
        }
        //$scope.adtotal = $scope.summary.grandTotal;

        $scope.data;
        $.each($scope.ChargeList, function (i, v) {
            debugger;
            $.each($scope.data, function (it, val) {
                debugger;
                if (typeof (val) != "undefined") {

                    if (val.CHARGE_CODE == v.CHARGE_CODE) {


                        //if (val.VALUE_PERCENT_FLAG == "P") {
                        //    $scope.ChargeList[i].VALUE_PERCENT_AMOUNT = parseFloat((val.CHARGE_AMOUNT * 100) / $scope.adtotal).toFixed(2);
                        //    $scope.ChargeList[i].CHARGE_AMOUNT = (parseFloat(sum.toFixed(2)) * $scope.ChargeList[i].VALUE_PERCENT_AMOUNT / 100);
                        //}
                        //else {
                        //    $scope.ChargeList[i].VALUE_PERCENT_AMOUNT = v.VALUE_PERCENT_AMOUNT;
                        //    $scope.ChargeList[i].CHARGE_AMOUNT = $scope.ChargeList[i].VALUE_PERCENT_AMOUNT;
                        //}
                        if ($scope.OrderNo != "undefined") {
                            $scope.eesync(true, v);
                        }
                        else {
                            $scope.sync(true, v);
                        }
                    }
                }
            });
        });
    }

    $scope.GrandtotalLineWiseCalculation = function () {
        debugger;
        var sum = 0;
        angular.forEach($scope.childModels, function (value, key) {

            if (typeof value[PRICE] !== 'undefined' && value[PRICE] !== null && value[PRICE] !== "") {

                sum = parseFloat(sum) + (parseFloat(value[PRICE]));
            }
            else
                return;
        });

        $scope.summary.grandTotal = (parseFloat(sum)).toFixed(2);

        $scope.adtotal = $scope.summary.grandTotal;

        $scope.data;

        $.each($scope.lineWiseChargeList, function (i, lineCharges) {
            $.each(lineCharges, function (j, charge) {
                $.each($scope.lineWiseData, function (k, dataCharge) {
                    $.each(dataCharge, function (l, chargeData) {
                        debugger;
                        if (charge && chargeData && charge.CHARGE_CODE === chargeData.CHARGE_CODE) {
                            if ($scope.OrderNo !== "undefined") {
                                $scope.eelwsync(true, charge);
                            } else {
                                $scope.lwsync(true, charge);
                            }
                        }
                    });
                });
            });
        });

    }

    $scope.GrandtotalLineWiseCalculationForReference = function () {
        debugger;
        var sum = 0;
        angular.forEach($scope.childModels, function (value, key) {

            if (typeof value[PRICE] !== 'undefined' && value[PRICE] !== null && value[PRICE] !== "") {

                sum = parseFloat(sum) + (parseFloat(value[PRICE]));
            }
            else
                return;
        });

        $scope.summary.grandTotal = (parseFloat(sum)).toFixed(2);

        $scope.adtotal = $scope.summary.grandTotal;

        $scope.data;

        $.each($scope.lineWiseChargeList, function (i, lineCharges) {
            $.each(lineCharges, function (j, charge) {
                $.each($scope.lineWiseData, function (k, dataCharge) {
                    $.each(dataCharge, function (l, chargeData) {
                        debugger;
                        if (charge && chargeData && charge.CHARGE_CODE === chargeData.CHARGE_CODE) {
                            if ($scope.OrderNo !== "undefined") {
                                $scope.eesync(true, charge);
                            } else {
                                $scope.lwsync(true, charge);
                            }
                        }
                    });
                });
            });
        });

    }

    $scope.GrandtotalCalculationJewellery = function () {
        debugger;
        var sum = 0;
        angular.forEach($scope.childModels, function (value, key) {

            //if (typeof value[TOTAL_ROW_PRICE] !== 'undefined' && value[TOTAL_ROW_PRICE] !== null && value[TOTAL_ROW_PRICE] !== "") {

            //    sum = parseFloat(sum) + (parseFloat(value[TOTAL_ROW_PRICE]));
            //}
            //else
            //    return;

            if (typeof value[TOTAL_PRICE] !== 'undefined' && value[TOTAL_PRICE] !== null && value[TOTAL_PRICE] !== "") {

                sum = parseFloat(sum) + (parseFloat(value[TOTAL_PRICE]));
            }
            else
                return;
        });

        $scope.summary.grandTotal = (parseFloat(sum)).toFixed(2);
        if ($scope.data.length < 0 || $scope.data.length == "undefined") {
            $scope.adtotal = $scope.summary.grandTotal;
        }
        else {
            $scope.adtotal = $scope.summary.grandTotal;
        }

        $scope.data;
        $.each($scope.ChargeList, function (i, v) {
            debugger;
            $.each($scope.data, function (it, val) {
                debugger;
                if (typeof (val) != "undefined") {

                    if (val.CHARGE_CODE == v.CHARGE_CODE) {

                        if ($scope.OrderNo != "undefined") {
                            $scope.eitsync(true, v);
                        }
                        else {
                            $scope.itsync(true, v);
                        }
                    }
                }
            });
        });
    }
    $scope.getCustomerDetail = function (customerId) {
        debugger;
        try {
            var cId = $.isNumeric(parseInt(customerId));
            if (cId === false) {
                throw "";
            }
            // var staturl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetMUCodeByProductId";
            var response = $http({
                method: "GET",
                url: "/api/TemplateApi/GetCustomerDetailByCustomerId",
                params: { customerId: customerId },
                contentType: "application/json",
                dataType: "json"
            });
            return response.then(function (data) {
                debugger;
                if (!data == "") {

                    if (typeof $scope.masterModels[SHIPPING_ADDRESS] !== "undefined") {
                        debugger;
                        $scope.masterModels[SHIPPING_ADDRESS] = data.data[0].SHIPPING_ADDRESS;

                    }
                    if (typeof $scope.masterModels[SHIPPING_CONTACT_NO] !== "undefined") {
                        $scope.masterModels[SHIPPING_CONTACT_NO] = data.data[0].SHIPPING_CONTACT_NO;

                    }
                    if (typeof $scope.masterModels[EMPLOYEE_CODE] !== "undefined") {
                        $scope.masterModels[EMPLOYEE_CODE] = data.data[0].EMPLOYEE_CODE;

                    }
                    console.log($scope.masterModels[SHIPPING_ADDRESS]);
                }
                else {

                }
            });
        } catch (e) {
            return;
        }

    };
    $scope.getmucode = function (index, productId) {
        debugger;
        try {
            var pId = $.isNumeric(parseInt(productId));
            if (pId === false) {
                throw "";
            }
            // var staturl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetMUCodeByProductId";
            var response = $http({
                method: "GET",
                url: "/api/TemplateApi/GetMUCodeByProductId",
                params: { productId: productId },
                contentType: "application/json",
                dataType: "json"
            });
            return response.then(function (data) {
                debugger;
                if (!data == "") {

                    if (typeof $scope.childModels[index][MU_CODE] !== "undefined") {
                        $scope.childModels[index][MU_CODE] = data.data[0].ItemUnit;
                        if ($scope.childModels[index].hasOwnProperty("ALT1_MU_CODE")) {
                            $scope.childModels[index].ALT1_MU_CODE = data.data[0].MultiItemUnit;
                        }
                    }
                    if (typeof $scope.childModels[index][PURITY] !== "undefined") {
                        $scope.childModels[index][PURITY] = data.data[0].ItemGrade;
                        //if ($scope.childModels[index].hasOwnProperty("ALT1_MU_CODE")) {
                        //    $scope.childModels[index].ALT1_MU_CODE = data.data[0].MultiItemUnit;
                        //}
                    }
                    //if (data.data[0].NON_VAT_FLAG !== "undefined") {
                    //    $scope.childModels[index] = {
                    //        NON_VAT_FLAG: data.data[0]?.NON_VAT_FLAG || 'Y'
                    //    };
                    //}
                    if (typeof $scope.childModels[index][NON_VAT_FLAG] !== "undefined") {
                        $scope.childModels[index][NON_VAT_FLAG] = data.data[0].NON_VAT_FLAG;
                    }
                    if (data.data[0].NON_VAT_FLAG != "undefined") {
                        $scope.childModels[index].NON_VAT_FLAG = data.data[0]?.NON_VAT_FLAG || 'Y';
                    }
                    //if ($scope.childModels[index].hasOwnProperty("ALT1_MU_CODE")) {
                    //    $scope.childModels[index].ALT1_MU_CODE = data.data[0].MultiItemUnit;
                    //}
                    $scope.ItemInfo = [];
                    $scope.ItemInfo.push(data.data[0]);
                    $scope.muwiseQty();
                    //var cs_code = "";
                    //if ($scope.masterModels["CUSTOMER_CODE"] != undefined) {
                    //    cs_code = $scope.masterModels["CUSTOMER_CODE"];
                    //}
                    //var area_code = "";
                    //if ($scope.masterModels["AREA_CODE"] != undefined) {
                    //    area_code = $scope.masterModels["AREA_CODE"];
                    //}
                    var ps_code = ""
                    if ($('#customerpriceid').data("kendoComboBox") != undefined) {
                        if ($('#customerpriceid').data("kendoComboBox").dataItem() != undefined) {
                            ps_code = $('#customerpriceid').data("kendoComboBox").dataItem().MASTER_ID;
                        }

                    }
                    var item_code = data.data[0].ItemCode;
                    //if (kendoEvent.sender.dataItem() != undefined) {
                    //    item_code = kendoEvent.sender.dataItem().ItemCode;
                    //}

                    //$scope.StandardItemRate(cs_code, $scope.FormCode, area_code, item_code, index);
                    //if (ps_code != "" && item_code != "") {
                    //    $scope.PricelistItemRate(ps_code, item_code, index);
                    //}
                    //BindFocus();
                }
                else {

                }
            });
        } catch (e) {
            return;
        }

    };
    var one = 0;
    $scope.masterModelDataFn = function (fn) {
        debugger;

        var saleOrderformDetail = formtemplateservice.getSalesOrderDetail_ByFormCodeAndOrderNo($scope.FormCode, $scope.OrderNo, d4);

        $.when(d4).done(function (result) {


            fn(result);
            if ($scope.tempCode != undefined) {
                var formcode = $scope.FormCode;
                var tempCode = $scope.tempCode;
                ++one;

                if (one === 1) {
                    debugger;
                    formtemplateservice.getDraftData_ByFormCodeAndTempCode(formcode, tempCode).then(function (result) {

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
                        debugger;
                        //if ($scope.DocumentName != "SA_SALES_INVOICE_JEWEL" && $scope.DocumentName != "SA_SALES_RETURN_JEWEL") {
                        //    $scope.GrandtotalCalution();
                        //}
                        //else {
                        //    $scope.GrandtotalCalculationJewellery();
                        //}
                        $scope.muwiseQty();
                    });
                }


            }
            $scope.masterModels;
            console.log("Scope.masterModel in masterModelDataFn==========================>>>>" + JSON.stringify($scope.masterModels));
        });
    };

    $scope.getObjWithKeysFromOtherObj = function (objKeys, objKeyswithData) {
        debugger;
        var keys = Object.keys(objKeys);
        var result = {};
        for (var i = 0; i < keys.length; i++) {
            result[keys[i]] = objKeyswithData[keys[i]];

            if (keys[i] == "REFERENCE_SERIAL_NO") {
                result[keys[i]] = objKeyswithData["SERIAL_NO"];
            }
            if (keys[i] == "REFERENCE_VOUCHER_NO") {
                result[keys[i]] = objKeyswithData["BLANKET_NO"];
            }
        }
        return result;
    };
    $scope.getObjWithKeysFromOtherObjInvCharge = function (objKeys, objKeyswithData) {

        var keys = Object.keys(objKeys);
        var result = {};
        for (var i = 0; i < keys.length; i++) {
            result[keys[i]] = objKeyswithData[keys[i]];
        }
        return result;
    };
    $scope.someDateFn = function () {

        var engdate = $filter('date')(new Date(), 'dd-MMM-yyyy');
        var a = ConvertEngDateToNep(engdate);
        $scope.NepaliDate = a;
        $("#nepaliDate5").val(a);

    };


    //change VoucherDate Eng to Nepali
    $scope.ConvertEngToNep = function () {
        console.log(this);

        var engdate = $("#englishDate5").val();
        var nepalidate = ConvertEngDateToNep(engdate);
        $("#nepaliDate5").val(nepalidate);
    };

    $scope.ReferenceList = [];
    var req = "/api/InventoryApi/GetReferenceList?formcode=" + $scope.FormCode;
    $http.get(req).then(function (response) {

        $scope.ReferenceList = response.data;
    });

    $scope.chargesChecked = false;

    //if ($scope.OrderNo === "undefined") {



    //}

    $scope.checkAll = function (bool) {

        if (bool)
            $scope.data = $scope.ChargeList;
        else
            $scope.data = [];
        $scope.calculateChargeAmount($scope.data, true);
    }

    $scope.data = [];
    $scope.lineWiseData = [];

    var chargeUrlForEdit = "/api/TemplateApi/GetChargeDataForEdit?formCode=" + $scope.FormCode + "&&voucherNo=" + $scope.OrderNo;
    $http.get(chargeUrlForEdit).then(function (res) {
        debugger;
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
                                    val.VALUE_PERCENT_AMOUNT = (val.CHARGE_AMOUNT * 100) / $scope.summary.grandTotal;

                                }
                                else {
                                    //  val.VALUE_PERCENT_AMOUNT = val.CHARGE_AMOUNT;
                                    //v.VALUE_PERCENT_AMOUNT = val.CHARGE_AMOUNT;
                                }
                                v.ACC_CODE = val.ACC_CODE;
                            }
                        });
                    });
                    debugger;
                    if ($scope.DocumentName != "SA_SALES_INVOICE_JEWEL" && $scope.DocumentName != "SA_SALES_RETURN_JEWEL") {
                        $scope.calculateChargeAmount1($scope.data, true);
                    }
                    else {
                        $scope.calculateChargeAmountJewellery($scope.data, true);
                    }
                }
            }, 0/*1500*/);
        }

    });

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
                $scope.SDModel.TRANSPORTER_EDESC = res.data[0].TRANSPORTER_EDESC;
                $scope.SDModel.VEHICLE_EDESC = res.data[0].VEHICLE_EDESC
                //$("#mydropdownlist").val("thevalue");


            }

        }

    });

    //for vijaya
    //$scope.calculateChargeAmount = function (dataList, bool) {

    //    var totalAddition = 0;
    //    var totalDeduction = 0;
    //    var netTotal = 0;
    //    $.each(dataList, function (i, val) {

    //        var percent_amount = val.VALUE_PERCENT_AMOUNT;
    //        var grand_total = $scope.summary.grandTotal;
    //        var charge_amount = val.CHARGE_AMOUNT;



    //        if (percent_amount === null || percent_amount === "" || percent_amount === NaN || percent_amount === undefined) {
    //            percent_amount = 0;
    //        }
    //        if (grand_total === null || grand_total === "" || grand_total === NaN || grand_total === undefined) {
    //            grand_total = 0;
    //        }
    //        if (val.VALUE_PERCENT_FLAG === 'P') {

    //            if (val.CHARGE_TYPE_FLAG == "D") {

    //                var Deduction = parseFloat(((percent_amount * grand_total) / 100).toFixed(2));

    //                val.CHARGE_AMOUNT = Deduction;
    //                totalDeduction += Deduction;

    //            }
    //            else {

    //                var Addition = parseFloat(((percent_amount * grand_total) / 100).toFixed(2));

    //                val.CHARGE_AMOUNT = Addition;
    //                totalAddition += Addition;

    //            }
    //        } else {

    //            if (val.CHARGE_TYPE_FLAG == "D") {

    //                //CHARGE_AMOUNT
    //                //var Deduction = parseFloat(percent_amount);
    //                var Deduction = parseFloat(charge_amount);

    //                val.CHARGE_AMOUNT = Deduction;
    //                totalDeduction += Deduction;
    //                if (charge_amount > 0) {
    //                    $scope.PrintDiscount = charge_amount;
    //                }
    //            }
    //            else {

    //                var Addition = parseFloat(percent_amount);

    //                val.CHARGE_AMOUNT = Addition;
    //                totalAddition += Addition;

    //            }
    //        }


    //    });

    //    $scope.deduction = totalDeduction.toFixed(2);
    //    $scope.addition = totalAddition.toFixed(2);
    //    netTotal = parseFloat(parseFloat($scope.summary.grandTotal) + parseFloat($scope.addition) - parseFloat($scope.deduction)).toFixed(2);

    //    $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;
    //    // $scope.ITEM.VALUE_PERCENT_AMOUNT = $scope.deduction;

    //}

    //for nila
    $scope.newCalculateChargeAmountSalesInvoiceJewelleryEdit = function (dataList, bool) {
        debugger;
        //var lista = dataList.sort((a, b) => (a.PRIORITY_INDEX_NO > b.PRIORITY_INDEX_NO) ? 1 : -1);
        var sortedObjs = _.sortBy(dataList, 'PRIORITY_INDEX_NO');
        var totalAddition = 0;
        var totalDeduction = 0;
        var netTotal = 0;
        $.each(sortedObjs, function (i, val) {
            debugger;
            var percent_amount = val.VALUE_PERCENT_AMOUNT;
            var grand_total = $scope.summary.grandTotal;
            var charge_amount = val.CHARGE_AMOUNT;



            if (percent_amount === null || percent_amount === "" || percent_amount === NaN || percent_amount === undefined) {
                percent_amount = 0;
            }
            if (grand_total === null || grand_total === "" || grand_total === NaN || grand_total === undefined) {
                grand_total = 0;
            }

            if (val.CHARGE_CODE == 'VT') {
                var chargeAmountTotal = 0;
                $.each($scope.childModels, function (key, value) {
                    debugger;
                    var nonVatFlag = value.NON_VAT_FLAG;
                    var totalRowPrice = value.TOTAL_ROW_PRICE;
                    var totalPrice = value.TOTAL_PRICE;
                    var luxuryTaxAmount = value.LUXURY_TAX_AMOUNT;
                    var vatAmount = value.VAT_AMOUNT;
                    var discountAmount = value.DISCOUNT_AMOUNT;
                    if (nonVatFlag != undefined && nonVatFlag == 'N') {

                        if (val.VALUE_PERCENT_FLAG === 'P') {

                            if (val.CHARGE_TYPE_FLAG == "D") {
                                //grand_total = parseFloat(totalRowPrice) + parseFloat(luxuryTaxAmount) - parseFloat(discountAmount);
                                //var Deduction = parseFloat(((percent_amount * grand_total) / 100).toFixed(2));

                                var Deduction = vatAmount;
                                chargeAmountTotal += vatAmount;
                                val.CHARGE_AMOUNT = chargeAmountTotal;
                                totalDeduction += Deduction;

                            }
                            else {
                                //grand_total = parseFloat(totalRowPrice) + parseFloat(luxuryTaxAmount) - parseFloat(discountAmount);
                                //var Addition = parseFloat(((percent_amount * grand_total) / 100).toFixed(2));

                                var Addition = vatAmount;
                                chargeAmountTotal += vatAmount;
                                val.CHARGE_AMOUNT = chargeAmountTotal;
                                totalAddition += Addition;
                            }
                        }
                        else if (val.VALUE_PERCENT_FLAG === 'Q') {

                            if (val.CHARGE_TYPE_FLAG == "D") {
                                var tiqty = 0;
                                $.each($scope.childModels, function (int, itn) {
                                    tiqty = tiqty + itn.QUANTITY


                                });
                                //var Deduction = parseFloat(percent_amount) * parseFloat(tiqty);
                                var Deduction = parseFloat(vatAmount) * parseFloat(tiqty);

                                //val.CHARGE_AMOUNT = Deduction;
                                chargeAmountTotal += vatAmount;
                                val.CHARGE_AMOUNT = chargeAmountTotal;
                                totalDeduction += Deduction;

                            }
                            else {
                                var tiqty = 0;
                                $.each($scope.childModels, function (int, itn) {
                                    tiqty = tiqty + itn.QUANTITY


                                });
                                //var Addition = parseFloat(percent_amount) * parseFloat(tiqty);
                                var Addition = parseFloat(vatAmount) * parseFloat(tiqty);

                                //val.CHARGE_AMOUNT = Addition;

                                chargeAmountTotal += vatAmount;
                                val.CHARGE_AMOUNT = chargeAmountTotal;

                                totalAddition += Addition;
                            }
                        }
                        else {

                            if (val.CHARGE_TYPE_FLAG == "D") {
                                var Deduction = parseFloat(vatAmount);
                                //val.CHARGE_AMOUNT = Deduction;

                                chargeAmountTotal += vatAmount;
                                val.CHARGE_AMOUNT = chargeAmountTotal;

                                totalDeduction += Deduction;
                                if (charge_amount > 0) {
                                    $scope.PrintDiscount = charge_amount;
                                }
                            }
                            else {


                            }
                        }
                    }
                });
            }
            else {
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


                    }
                }
            }
        });

        //$scope.deduction = totalDeduction.toFixed(2);
        //$scope.addition = totalAddition.toFixed(2);
        //$scope.deductionCalc = totalDeduction.toFixed(2);
        //$scope.additionCalc = totalAddition.toFixed(2);


        $scope.deduction = totalDeduction.toFixed(2);
        $scope.addition = totalAddition.toFixed(2);
        $scope.deductionCalc = totalDeduction.toFixed(2);
        $scope.deductionCalc = Math.round($scope.deductionCalc);
        $scope.additionCalc = totalAddition.toFixed(2);
        $scope.additionCalc = Math.round($scope.additionCalc);

        netTotal = parseFloat(parseFloat($scope.summary.grandTotal) + parseFloat($scope.additionCalc) - parseFloat($scope.deductionCalc)).toFixed(2);

        $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;
    }

    $scope.newCalculateChargeAmountSalesInvoiceJewelleryEditReference = function (dataList, bool) {
        debugger;
        //var lista = dataList.sort((a, b) => (a.PRIORITY_INDEX_NO > b.PRIORITY_INDEX_NO) ? 1 : -1);
        var sortedObjs = _.sortBy(dataList, 'PRIORITY_INDEX_NO');
        var totalAddition = 0;
        var totalDeduction = 0;
        var netTotal = 0;
        $.each(sortedObjs, function (i, val) {
            debugger;
            var percent_amount = val.VALUE_PERCENT_AMOUNT;
            var grand_total = $scope.summary.grandTotal;
            var charge_amount = val.CHARGE_AMOUNT;



            if (percent_amount === null || percent_amount === "" || percent_amount === NaN || percent_amount === undefined) {
                percent_amount = 0;
            }
            if (grand_total === null || grand_total === "" || grand_total === NaN || grand_total === undefined) {
                grand_total = 0;
            }

            if (val.CHARGE_CODE == 'VT') {
                var chargeAmountTotal = 0;
                $.each($scope.childModels, function (key, value) {
                    debugger;
                    var nonVatFlag = value.NON_VAT_FLAG;
                    var totalRowPrice = value.TOTAL_ROW_PRICE;
                    var totalPrice = value.TOTAL_PRICE;
                    var luxuryTaxAmount = value.LUXURY_TAX_AMOUNT;
                    var vatAmount = value.VAT_AMOUNT;
                    var discountAmount = value.DISCOUNT_AMOUNT;
                    if (nonVatFlag != undefined && nonVatFlag == 'N') {

                        if (val.VALUE_PERCENT_FLAG === 'P') {

                            if (val.CHARGE_TYPE_FLAG == "D") {
                                //grand_total = parseFloat(totalRowPrice) + parseFloat(luxuryTaxAmount) - parseFloat(discountAmount);
                                //var Deduction = parseFloat(((percent_amount * grand_total) / 100).toFixed(2));

                                var Deduction = vatAmount;
                                chargeAmountTotal += vatAmount;
                                val.CHARGE_AMOUNT = chargeAmountTotal;
                                totalDeduction += Deduction;

                            }
                            else {
                                //grand_total = parseFloat(totalRowPrice) + parseFloat(luxuryTaxAmount) - parseFloat(discountAmount);
                                //var Addition = parseFloat(((percent_amount * grand_total) / 100).toFixed(2));

                                var Addition = vatAmount;
                                chargeAmountTotal += vatAmount;
                                val.CHARGE_AMOUNT = chargeAmountTotal;
                                totalAddition += Addition;
                            }
                        }
                        else if (val.VALUE_PERCENT_FLAG === 'Q') {

                            if (val.CHARGE_TYPE_FLAG == "D") {
                                var tiqty = 0;
                                $.each($scope.childModels, function (int, itn) {
                                    tiqty = tiqty + itn.QUANTITY


                                });
                                //var Deduction = parseFloat(percent_amount) * parseFloat(tiqty);
                                var Deduction = parseFloat(vatAmount) * parseFloat(tiqty);

                                //val.CHARGE_AMOUNT = Deduction;
                                chargeAmountTotal += vatAmount;
                                val.CHARGE_AMOUNT = chargeAmountTotal;
                                totalDeduction += Deduction;

                            }
                            else {
                                var tiqty = 0;
                                $.each($scope.childModels, function (int, itn) {
                                    tiqty = tiqty + itn.QUANTITY


                                });
                                //var Addition = parseFloat(percent_amount) * parseFloat(tiqty);
                                var Addition = parseFloat(vatAmount) * parseFloat(tiqty);

                                //val.CHARGE_AMOUNT = Addition;

                                chargeAmountTotal += vatAmount;
                                val.CHARGE_AMOUNT = chargeAmountTotal;

                                totalAddition += Addition;
                            }
                        }
                        else {

                            if (val.CHARGE_TYPE_FLAG == "D") {
                                var Deduction = parseFloat(vatAmount);
                                //val.CHARGE_AMOUNT = Deduction;

                                chargeAmountTotal += vatAmount;
                                val.CHARGE_AMOUNT = chargeAmountTotal;

                                totalDeduction += Deduction;
                                if (charge_amount > 0) {
                                    $scope.PrintDiscount = charge_amount;
                                }
                            }
                            else {


                            }
                        }
                    }
                });
            }
            else {
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


                    }
                }
            }
        });

        //$scope.deduction = totalDeduction.toFixed(2);
        //$scope.addition = totalAddition.toFixed(2);
        //$scope.deductionCalc = totalDeduction.toFixed(2);
        //$scope.additionCalc = totalAddition.toFixed(2);


        $scope.deduction = totalDeduction.toFixed(2);
        $scope.addition = totalAddition.toFixed(2);
        $scope.deductionCalc = totalDeduction.toFixed(2);
        $scope.deductionCalc = Math.round($scope.deductionCalc);
        $scope.additionCalc = totalAddition.toFixed(2);
        $scope.additionCalc = Math.round($scope.additionCalc);

        netTotal = parseFloat(parseFloat($scope.summary.grandTotal) + parseFloat($scope.additionCalc) - parseFloat($scope.deductionCalc)).toFixed(2);

        $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;
    }

    //for subin
    if ($scope.OrderNo === "undefined") {
        //$scope.calculateChargeAmount = function (dataList, bool) {
        //    
        //    var totalAddition = 0;
        //    var totalDeduction = 0;
        //    var netTotal = 0;
        //    $.each(dataList, function (i, val) {
        //        
        //        var percent_amount = val.VALUE_PERCENT_AMOUNT;
        //        var grand_total = $scope.summary.grandTotal;
        //        var charge_amount = val.CHARGE_AMOUNT;
        //        if (percent_amount === null || percent_amount === "" || percent_amount === NaN || percent_amount === undefined) {
        //            percent_amount = 0;
        //        }
        //        if (grand_total === null || grand_total === "" || grand_total === NaN || grand_total === undefined) {
        //            grand_total = 0;
        //        }

        //         if (val.VALUE_PERCENT_FLAG === 'Q') {

        //            if (val.CHARGE_TYPE_FLAG == "D") {
        //                var tiqty = 0;
        //                $.each($scope.childModels, function (int, itn) {
        //                    tiqty = tiqty + itn.QUANTITY


        //                });
        //                var Deduction = parseFloat(percent_amount) * parseFloat(tiqty);

        //                val.CHARGE_AMOUNT = Deduction;
        //                totalDeduction += Deduction;

        //            }
        //            else {
        //                var tiqty = 0;
        //                $.each($scope.childModels, function (int, itn) {
        //                    tiqty = tiqty + itn.QUANTITY


        //                });
        //                var Addition = parseFloat(percent_amount) * parseFloat(tiqty);

        //                val.CHARGE_AMOUNT = Addition;
        //                totalAddition += Addition;
        //            }
        //        } 
        //        if (val.VALUE_PERCENT_FLAG === 'V') {

        //            if (val.CHARGE_TYPE_FLAG == "D") {
        //                var Deduction = parseFloat(percent_amount);
        //                val.CHARGE_AMOUNT = Deduction;
        //                totalDeduction += Deduction;
        //                if (charge_amount > 0) {
        //                    $scope.PrintDiscount = charge_amount;
        //                }
        //            }
        //            else {

        //                var Addition = parseFloat(percent_amount);
        //                val.CHARGE_AMOUNT = Addition;
        //                totalAddition += Addition;
        //            }
        //        }
        //    });

        //    $scope.deduction = totalDeduction.toFixed(2);
        //    $scope.addition = totalAddition.toFixed(2);
        //    netTotal = parseFloat(parseFloat($scope.summary.grandTotal) + parseFloat($scope.addition) - parseFloat($scope.deduction)).toFixed(2);

        //    $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;

        //    $scope.calculateChargeAmountPercentage(dataList, bool);

        //}
        $scope.calculateChargeAmount = function (dataList, bool) {
            debugger;
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

        $scope.calculateLineWiseChargeAmount = function (dataList, bool) {
            debugger;
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


        $scope.calculateTotalLineWiseChargeAmount = function (dataList, bool) {
            //var lista = dataList.sort((a, b) => (a.PRIORITY_INDEX_NO > b.PRIORITY_INDEX_NO) ? 1 : -1);

            dataList.forEach(function (chargeArray) {
                chargeArray.sort(function (a, b) {
                    return a.PRIORITY_INDEX_NO - b.PRIORITY_INDEX_NO;
                });
            });

            //var sortedObjs = _.sortBy(dataList, 'PRIORITY_INDEX_NO');
            var totalAddition = 0;
            var totalDeduction = 0;
            var netTotal = 0;
            var total_row_price = $scope.summary.grandTotal;
            var totalPrice = 0;


            $.each($scope.childModels, function (i, childModels) {
                var nonVatFlag = childModels.NON_VAT_FLAG;
                var price = childModels.PRICE;
                var chargeAddition = 0;
                var chargeDeduciton = 0;
                $.each(dataList, function (k, dataCharge) {
                    $.each(dataCharge, function (l, chargeData) {
                        debugger;
                        var priceVal = 0;
                        var percent_amount = chargeData.VALUE_PERCENT_AMOUNT;
                        var percent_flag = chargeData.VALUE_PERCENT_FLAG;
                        var charge_amount = chargeData.CHARGE_AMOUNT;
                        var charge_flag = chargeData.CHARGE_TYPE_FLAG;
                        if (i === k) {

                            const isVatApplicable = (chargeData.CHARGE_CODE !== "VT" && chargeData.APPLY_ON === "C") ||
                                (chargeData.CHARGE_CODE === "VT" && childModels.NON_VAT_FLAG === 'N' && chargeData.APPLY_ON === "C");

                            if (isVatApplicable) {
                                if (percent_flag === 'P') {
                                    if (charge_flag === 'D') {
                                        priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                        var Deduction = parseFloat(((percent_amount * priceVal) / 100).toFixed(2));

                                        chargeData.CHARGE_AMOUNT = Deduction;
                                        chargeDeduciton += Deduction;
                                    }
                                    else {

                                        priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                        var Addition = parseFloat(((percent_amount * priceVal) / 100).toFixed(2));

                                        chargeData.CHARGE_AMOUNT = Addition;
                                        chargeAddition += Addition;
                                    }
                                }
                                else if (percent_flag === 'V') {
                                    if (charge_flag === 'D') {

                                        priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                        var Deduction = parseFloat(((percent_amount * priceVal - percent_amount) / 100).toFixed(2));

                                        chargeData.CHARGE_AMOUNT = Deduction;
                                        chargeDeduciton += Deduction;
                                    }
                                    else {

                                        priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                        var Addition = parseFloat(((percent_amount * priceVal) / 100).toFixed(2));

                                        chargeData.CHARGE_AMOUNT = Addition;
                                        chargeAddition += Addition;
                                    }
                                }
                                else if (percent_flag === 'Q') {
                                    if (charge_flag === 'D') {

                                        priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                        var Deduction = parseFloat(((percent_amount * priceVal - percent_amount) / 100).toFixed(2));

                                        chargeData.CHARGE_AMOUNT = Deduction;
                                        chargeDeduciton += Deduction;
                                    }
                                    else {

                                        priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                        var Addition = parseFloat(((percent_amount * priceVal) / 100).toFixed(2));

                                        chargeData.CHARGE_AMOUNT = Addition;
                                        chargeAddition += Addition;
                                    }
                                }
                            }


                        }
                    });
                });
                totalPrice += price;
                totalAddition += chargeAddition;
                totalDeduction += chargeDeduciton;
            });


            $.each($scope.ChargeList, function (i, chargeListData) {
                chargeListData.CHARGE_AMOUNT = 0;
                $.each(dataList, function (k, dataCharge) {
                    $.each(dataCharge, function (l, chargeData) {
                        debugger;
                        if (chargeListData.CHARGE_CODE === chargeData.CHARGE_CODE) {
                            chargeListData.CHARGE_AMOUNT += parseFloat((chargeData.CHARGE_AMOUNT).toFixed(2));
                        }
                    });
                });
            });

            $scope.deduction = totalDeduction.toFixed(2);
            $scope.addition = totalAddition.toFixed(2);
            $scope.deductionCalc = totalDeduction.toFixed(2);
            $scope.additionCalc = totalAddition.toFixed(2);
            netTotal = parseFloat(parseFloat(totalPrice) + parseFloat($scope.additionCalc) - parseFloat($scope.deductionCalc)).toFixed(2);

            $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;
        }

        $scope.calculateTotalLineWiseChargeAmountReferenceEdit = function (dataList, bool) {
            debugger;
            //var lista = dataList.sort((a, b) => (a.PRIORITY_INDEX_NO > b.PRIORITY_INDEX_NO) ? 1 : -1);

            if ($scope.OrderNo === "undefined" && $scope.youFromReference == true) {
                dataList.forEach(function (chargeArray) {
                    chargeArray.sort(function (a, b) {
                        return a.PRIORITY_INDEX_NO - b.PRIORITY_INDEX_NO;
                    });
                });

                //var sortedObjs = _.sortBy(dataList, 'PRIORITY_INDEX_NO');
                var totalAddition = 0;
                var totalDeduction = 0;
                var netTotal = 0;
                var total_row_price = $scope.summary.grandTotal;
                var totalPrice = 0;


                $.each($scope.childModels, function (i, childModels) {
                    var price = childModels.QUANTITY * childModels.UNIT_PRICE;
                    var chargeAddition = 0;
                    var chargeDeduciton = 0;
                    $.each(dataList, function (k, dataCharge) {
                        $.each(dataCharge, function (l, chargeData) {
                            var priceVal = 0;
                            var percent_amount = chargeData.VALUE_PERCENT_AMOUNT;
                            var percent_flag = chargeData.VALUE_PERCENT_FLAG;
                            var charge_amount = chargeData.CHARGE_AMOUNT;
                            var charge_flag = chargeData.CHARGE_TYPE_FLAG;
                            if (i === k) {

                                const isVatApplicable = (chargeData.CHARGE_CODE !== "VT" && chargeData.APPLY_ON === "C") ||
                                    (chargeData.CHARGE_CODE === "VT" && childModels.NON_VAT_FLAG === 'N' && chargeData.APPLY_ON === "C");

                                if (isVatApplicable) {
                                    if (percent_flag === 'P') {
                                        if (charge_flag === 'D') {
                                            priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                            var Deduction = parseFloat(((percent_amount * priceVal) / 100).toFixed(2));

                                            chargeData.CHARGE_AMOUNT = Deduction;
                                            chargeDeduciton += Deduction;
                                        }
                                        else {

                                            priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                            var Addition = parseFloat(((percent_amount * priceVal) / 100).toFixed(2));

                                            chargeData.CHARGE_AMOUNT = Addition;
                                            chargeAddition += Addition;
                                        }
                                    }
                                    else if (percent_flag === 'V') {
                                        if (charge_flag === 'D') {

                                            priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                            var Deduction = parseFloat(((percent_amount * priceVal - percent_amount) / 100).toFixed(2));

                                            chargeData.CHARGE_AMOUNT = Deduction;
                                            chargeDeduciton += Deduction;
                                        }
                                        else {

                                            priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                            var Addition = parseFloat(((percent_amount * priceVal) / 100).toFixed(2));

                                            chargeData.CHARGE_AMOUNT = Addition;
                                            chargeAddition += Addition;
                                        }
                                    }
                                    else if (percent_flag === 'Q') {
                                        if (charge_flag === 'D') {

                                            priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                            var Deduction = parseFloat(((percent_amount * priceVal - percent_amount) / 100).toFixed(2));

                                            chargeData.CHARGE_AMOUNT = Deduction;
                                            chargeDeduciton += Deduction;
                                        }
                                        else {

                                            priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                            var Addition = parseFloat(((percent_amount * priceVal) / 100).toFixed(2));

                                            chargeData.CHARGE_AMOUNT = Addition;
                                            chargeAddition += Addition;
                                        }
                                    }
                                }


                            }
                        });
                    });
                    totalPrice += price;
                    totalAddition += chargeAddition;
                    totalDeduction += chargeDeduciton;
                });


                $.each($scope.ChargeList, function (i, chargeListData) {
                    debugger;
                    chargeListData.CHARGE_AMOUNT = 0;
                    $.each(dataList, function (k, dataCharge) {
                        $.each(dataCharge, function (l, chargeData) {
                            debugger;
                            if (chargeListData.CHARGE_CODE === chargeData.CHARGE_CODE) {
                                chargeListData.CHARGE_AMOUNT += parseFloat((chargeData.CHARGE_AMOUNT).toFixed(2));
                            }
                        });
                    });
                });

                $scope.deduction = totalDeduction.toFixed(2);
                $scope.addition = totalAddition.toFixed(2);
                $scope.deductionCalc = totalDeduction.toFixed(2);
                $scope.additionCalc = totalAddition.toFixed(2);
                netTotal = parseFloat(parseFloat(totalPrice) + parseFloat($scope.additionCalc) - parseFloat($scope.deductionCalc)).toFixed(2);

                $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;
            }
        }



        $scope.calculateChargeAmountSalesInvoiceJewellery = function (dataList, bool) {
            debugger;
            //var lista = dataList.sort((a, b) => (a.PRIORITY_INDEX_NO > b.PRIORITY_INDEX_NO) ? 1 : -1);
            var sortedObjs = _.sortBy(dataList, 'PRIORITY_INDEX_NO');
            var totalAddition = 0;
            var totalDeduction = 0;
            var netTotal = 0;
            $.each(sortedObjs, function (i, val) {
                debugger;
                var percent_amount = val.VALUE_PERCENT_AMOUNT;
                var grand_total = $scope.summary.grandTotal;
                var charge_amount = val.CHARGE_AMOUNT;



                if (percent_amount === null || percent_amount === "" || percent_amount === NaN || percent_amount === undefined) {
                    percent_amount = 0;
                }
                if (grand_total === null || grand_total === "" || grand_total === NaN || grand_total === undefined) {
                    grand_total = 0;
                }

                //$.each($scope.childModels, function (i, val) {
                //    debugger;
                //    var vatAmount = val.VAT_AMOUNT;
                //});

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
                        //////-----------------Discount and luxury tax after 10L-----------------////////////////////

                        //if (parseFloat($scope.summary.grandTotal) >= 1000000 && totalDeduction != 0 && totalDeduction != null && totalDeduction != undefined && luxuryTaxAddition == true) {


                        //    totalAddition = (parseFloat($scope.summary.grandTotal) / 100) * 2;
                        //    /*totalAddition = parseFloat($scope.summary.grandTotal) * 1.02;*/
                        //    if (totalAddition > 0) {
                        //        debugger;
                        //        if (luxuryTaxAddition == true) {
                        //            luxuryTaxAddition = false;
                        //            angular.forEach($scope.ChargeList,
                        //                function (chvalue, chkey) {
                        //                    debugger;
                        //                    if (chvalue.CHARGE_CODE == "LT") {
                        //                        chvalue.CHARGE_AMOUNT = totalAddition;
                        //                        chvalue.VALUE_PERCENT_AMOUNT = totalAddition;
                        //                        $scope.sync(true, chvalue);
                        //                    }
                        //                });

                        //        }
                        //    }
                        //}

                        //////-----------------Discount and luxury tax after 10L-----------------////////////////////
                        //if (totalDeduction <= 0) {
                        //    debugger;
                        //    for (var i = 0; i < $scope.data.length; i++) {

                        //        if ($scope.data[i].CHARGE_CODE == val.CHARGE_CODE) {
                        //            debugger;

                        //            $scope.data.splice(i, 1);

                        //            if (val.CHARGE_CODE == "SD" && ($scope.DocumentName == "SA_SALES_INVOICE_JEWEL" || $scope.DocumentName == "SA_SALES_RETURN_JEWEL")) {
                        //                debugger;
                        //                for (let i = $scope.data.length - 1; i >= 0; i--) {

                        //                    $scope.data.splice(i, 1);

                        //                }
                        //                /*$scope.data.splice(i, 1);*/
                        //                $scope.bool = false;
                        //                //isChecked('SD');
                        //                //isChecked('LT');

                        //                //luxuryTaxAddition = true;

                        //            }
                        //        }
                        //    }
                        //}

                        //////-----------------Discount and luxury tax after 10L-----------------////////////////////
                    }
                    else {
                        //////-----------------Discount and luxury tax after 10L-----------------////////////////////

                        //if (parseFloat($scope.summary.grandTotal) >= 1000000 && totalDeduction != 0 && totalDeduction != null && totalDeduction != undefined) {
                        //    var Addition = (parseFloat($scope.summary.grandTotal) / 100) * 2;
                        //    val.CHARGE_AMOUNT = Addition;
                        //    val.VALUE_PERCENT_AMOUNT = Addition;
                        //    totalAddition += Addition;
                        //}

                        //////-----------------Discount and luxury tax after 10L-----------------////////////////////

                    }
                }
            });

            //$scope.deduction = totalDeduction.toFixed(2);
            //$scope.addition = totalAddition.toFixed(2);
            //$scope.deductionCalc = totalDeduction.toFixed(2);
            //$scope.additionCalc = totalAddition.toFixed(2);

            $scope.deduction = totalDeduction.toFixed(2);
            $scope.addition = totalAddition.toFixed(2);
            $scope.deductionCalc = totalDeduction.toFixed(2);
            $scope.deductionCalc = Math.round($scope.deductionCalc);
            $scope.additionCalc = totalAddition.toFixed(2);
            $scope.additionCalc = Math.round($scope.additionCalc);

            netTotal = parseFloat(parseFloat($scope.summary.grandTotal) + parseFloat($scope.additionCalc) - parseFloat($scope.deductionCalc)).toFixed(2);

            $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;
        }


        $scope.newCalculateChargeAmountSalesInvoiceJewellery = function (dataList, bool) {
            debugger;
            //var lista = dataList.sort((a, b) => (a.PRIORITY_INDEX_NO > b.PRIORITY_INDEX_NO) ? 1 : -1);
            var sortedObjs = _.sortBy(dataList, 'PRIORITY_INDEX_NO');
            var totalAddition = 0;
            var totalDeduction = 0;
            var netTotal = 0;
            $.each(sortedObjs, function (i, val) {
                debugger;
                var percent_amount = val.VALUE_PERCENT_AMOUNT;
                var grand_total = $scope.summary.grandTotal;
                var charge_amount = val.CHARGE_AMOUNT;



                if (percent_amount === null || percent_amount === "" || percent_amount === NaN || percent_amount === undefined) {
                    percent_amount = 0;
                }
                if (grand_total === null || grand_total === "" || grand_total === NaN || grand_total === undefined) {
                    grand_total = 0;
                }

                if (val.CHARGE_CODE == 'VT') {
                    var chargeAmountTotal = 0;
                    $.each($scope.childModels, function (key, value) {
                        debugger;
                        var nonVatFlag = value.NON_VAT_FLAG;
                        var totalRowPrice = value.TOTAL_ROW_PRICE;
                        var totalPrice = value.TOTAL_PRICE;
                        var luxuryTaxAmount = value.LUXURY_TAX_AMOUNT;
                        var vatAmount = value.VAT_AMOUNT;
                        var discountAmount = value.DISCOUNT_AMOUNT;
                        if (nonVatFlag != undefined && nonVatFlag == 'N') {

                            if (val.VALUE_PERCENT_FLAG === 'P') {

                                if (val.CHARGE_TYPE_FLAG == "D") {
                                    //grand_total = parseFloat(totalRowPrice) + parseFloat(luxuryTaxAmount) - parseFloat(discountAmount);
                                    //var Deduction = parseFloat(((percent_amount * grand_total) / 100).toFixed(2));

                                    var Deduction = vatAmount;
                                    chargeAmountTotal += vatAmount;
                                    val.CHARGE_AMOUNT = chargeAmountTotal;
                                    totalDeduction += Deduction;

                                }
                                else {
                                    //grand_total = parseFloat(totalRowPrice) + parseFloat(luxuryTaxAmount) - parseFloat(discountAmount);
                                    //var Addition = parseFloat(((percent_amount * grand_total) / 100).toFixed(2));

                                    var Addition = vatAmount;
                                    chargeAmountTotal += vatAmount;
                                    val.CHARGE_AMOUNT = chargeAmountTotal;
                                    totalAddition += Addition;
                                }
                            }
                            else if (val.VALUE_PERCENT_FLAG === 'Q') {

                                if (val.CHARGE_TYPE_FLAG == "D") {
                                    var tiqty = 0;
                                    $.each($scope.childModels, function (int, itn) {
                                        tiqty = tiqty + itn.QUANTITY


                                    });
                                    //var Deduction = parseFloat(percent_amount) * parseFloat(tiqty);
                                    var Deduction = parseFloat(vatAmount) * parseFloat(tiqty);

                                    //val.CHARGE_AMOUNT = Deduction;
                                    chargeAmountTotal += vatAmount;
                                    val.CHARGE_AMOUNT = chargeAmountTotal;
                                    totalDeduction += Deduction;

                                }
                                else {
                                    var tiqty = 0;
                                    $.each($scope.childModels, function (int, itn) {
                                        tiqty = tiqty + itn.QUANTITY


                                    });
                                    //var Addition = parseFloat(percent_amount) * parseFloat(tiqty);
                                    var Addition = parseFloat(vatAmount) * parseFloat(tiqty);

                                    //val.CHARGE_AMOUNT = Addition;

                                    chargeAmountTotal += vatAmount;
                                    val.CHARGE_AMOUNT = chargeAmountTotal;

                                    totalAddition += Addition;
                                }
                            }
                            else {

                                if (val.CHARGE_TYPE_FLAG == "D") {
                                    var Deduction = parseFloat(vatAmount);
                                    //val.CHARGE_AMOUNT = Deduction;

                                    chargeAmountTotal += vatAmount;
                                    val.CHARGE_AMOUNT = chargeAmountTotal;

                                    totalDeduction += Deduction;
                                    if (charge_amount > 0) {
                                        $scope.PrintDiscount = charge_amount;
                                    }
                                }
                                else {


                                }
                            }
                        }
                        else {
                            val.CHARGE_AMOUNT = chargeAmountTotal;
                        }
                    });
                }
                else {
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


                        }
                    }
                }
            });

            $scope.deduction = totalDeduction.toFixed(2);
            $scope.addition = totalAddition.toFixed(2);
            $scope.deductionCalc = totalDeduction.toFixed(2);
            $scope.deductionCalc = Math.round($scope.deductionCalc);
            $scope.additionCalc = totalAddition.toFixed(2);
            $scope.additionCalc = Math.round($scope.additionCalc);
            netTotal = parseFloat(parseFloat($scope.summary.grandTotal) + parseFloat($scope.additionCalc) - parseFloat($scope.deductionCalc)).toFixed(2);

            $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;
        }


        $scope.calculateChargeAmountPercentage = function (dataList, bool) {
            debugger;
            var totalAddition = 0;
            var totalDeduction = 0;
            var netTotal = 0;
            $scope.PercentageFlag = "False";
            $.each(dataList, function (i, val) {

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
                    $scope.PercentageFlag = "True";
                    $scope.ChargeCodeFlag = val.CHARGE_CODE;

                    if (val.CHARGE_TYPE_FLAG == "D") {
                        grand_total = parseFloat($scope.summary.grandTotal) + parseFloat($scope.addition) - parseFloat($scope.deduction);
                        var Deduction = parseFloat(((percent_amount * grand_total) / 100).toFixed(2));

                        val.CHARGE_AMOUNT = Deduction;

                        totalDeduction += Deduction;

                    }
                    else {
                        grand_total = parseFloat($scope.summary.grandTotal) + parseFloat($scope.addition) - parseFloat($scope.deduction);
                        var Addition = parseFloat(((percent_amount * grand_total) / 100).toFixed(2));

                        val.CHARGE_AMOUNT = Addition;
                        totalAddition += Addition;
                    }
                }

            });
            if ($scope.PercentageFlag === "True" && $scope.ChargeCodeFlag == "VT") {
                var prevdeduction = $scope.deduction;
                var prevaddition = $scope.addition;
                //$scope.deduction = totalDeduction.toFixed(2);
                $scope.deduction = parseFloat(parseFloat(totalDeduction) + parseFloat($scope.deduction)).toFixed(2);
                $scope.addition = parseFloat(parseFloat(totalAddition) + parseFloat($scope.addition)).toFixed(2);
                netTotal = parseFloat(parseFloat($scope.summary.grandTotal) + parseFloat($scope.addition) - parseFloat($scope.deduction)).toFixed(2);

                $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;
            }
            if ($scope.PercentageFlag === "True" && $scope.ChargeCodeFlag != "VT") {
                var prevdeduction = $scope.deduction;
                var prevaddition = $scope.addition;
                //$scope.deduction = totalDeduction.toFixed(2);
                $scope.deduction = parseFloat(totalDeduction).toFixed(2);
                $scope.addition = parseFloat(totalDeduction).toFixed(2);
                netTotal = parseFloat(parseFloat($scope.summary.grandTotal) + parseFloat($scope.addition) - parseFloat($scope.deduction)).toFixed(2);

                $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;
            }




        }
    }

    $scope.calculateTotalLineWiseChargeAmountEdit = function (dataList, bool) {
        debugger;
        //var lista = dataList.sort((a, b) => (a.PRIORITY_INDEX_NO > b.PRIORITY_INDEX_NO) ? 1 : -1);

        if ($scope.OrderNo !== "undefined") {
            dataList.forEach(function (chargeArray) {
                chargeArray.sort(function (a, b) {
                    return a.PRIORITY_INDEX_NO - b.PRIORITY_INDEX_NO;
                });
            });

            //var sortedObjs = _.sortBy(dataList, 'PRIORITY_INDEX_NO');
            var totalAddition = 0;
            var totalDeduction = 0;
            var netTotal = 0;
            var total_row_price = $scope.summary.grandTotal;
            var totalPrice = 0;


            $.each($scope.childModels, function (i, childModels) {
                var price = childModels.QUANTITY * childModels.UNIT_PRICE;
                var chargeAddition = 0;
                var chargeDeduciton = 0;
                $.each(dataList, function (k, dataCharge) {
                    $.each(dataCharge, function (l, chargeData) {
                        var priceVal = 0;
                        var percent_amount = chargeData.VALUE_PERCENT_AMOUNT;
                        var percent_flag = chargeData.VALUE_PERCENT_FLAG;
                        var charge_amount = chargeData.CHARGE_AMOUNT;
                        var charge_flag = chargeData.CHARGE_TYPE_FLAG;
                        if (i === k) {

                            const isVatApplicable = (chargeData.CHARGE_CODE !== "VT" && chargeData.APPLY_ON === "C") ||
                                (chargeData.CHARGE_CODE === "VT" && childModels.NON_VAT_FLAG === 'N' && chargeData.APPLY_ON === "C");

                            if (isVatApplicable) {
                                if (percent_flag === 'P') {
                                    if (charge_flag === 'D') {
                                        priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                        var Deduction = parseFloat(((percent_amount * priceVal) / 100).toFixed(2));

                                        chargeData.CHARGE_AMOUNT = Deduction;
                                        chargeDeduciton += Deduction;
                                    }
                                    else {

                                        priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                        var Addition = parseFloat(((percent_amount * priceVal) / 100).toFixed(2));

                                        chargeData.CHARGE_AMOUNT = Addition;
                                        chargeAddition += Addition;
                                    }
                                }
                                else if (percent_flag === 'V') {
                                    if (charge_flag === 'D') {

                                        priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                        var Deduction = parseFloat(((percent_amount * priceVal - percent_amount) / 100).toFixed(2));

                                        chargeData.CHARGE_AMOUNT = Deduction;
                                        chargeDeduciton += Deduction;
                                    }
                                    else {

                                        priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                        var Addition = parseFloat(((percent_amount * priceVal) / 100).toFixed(2));

                                        chargeData.CHARGE_AMOUNT = Addition;
                                        chargeAddition += Addition;
                                    }
                                }
                                else if (percent_flag === 'Q') {
                                    if (charge_flag === 'D') {

                                        priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                        var Deduction = parseFloat(((percent_amount * priceVal - percent_amount) / 100).toFixed(2));

                                        chargeData.CHARGE_AMOUNT = Deduction;
                                        chargeDeduciton += Deduction;
                                    }
                                    else {

                                        priceVal = parseFloat(price) + parseFloat(chargeAddition) - parseFloat(chargeDeduciton);
                                        var Addition = parseFloat(((percent_amount * priceVal) / 100).toFixed(2));

                                        chargeData.CHARGE_AMOUNT = Addition;
                                        chargeAddition += Addition;
                                    }
                                }
                            }


                        }
                    });
                });
                totalPrice += price;
                totalAddition += chargeAddition;
                totalDeduction += chargeDeduciton;
            });


            $.each($scope.ChargeList, function (i, chargeListData) {
                debugger;
                chargeListData.CHARGE_AMOUNT = 0;
                $.each(dataList, function (k, dataCharge) {
                    $.each(dataCharge, function (l, chargeData) {
                        debugger;
                        if (chargeListData.CHARGE_CODE === chargeData.CHARGE_CODE) {
                            chargeListData.CHARGE_AMOUNT += parseFloat((chargeData.CHARGE_AMOUNT).toFixed(2));
                        }
                    });
                });
            });

            $scope.deduction = totalDeduction.toFixed(2);
            $scope.addition = totalAddition.toFixed(2);
            $scope.deductionCalc = totalDeduction.toFixed(2);
            $scope.additionCalc = totalAddition.toFixed(2);
            netTotal = parseFloat(parseFloat(totalPrice) + parseFloat($scope.additionCalc) - parseFloat($scope.deductionCalc)).toFixed(2);

            $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;
        }
    }

    $scope.calculateChargeAmountJewellery = function (dataList, bool) {
        debugger;
        var totalAddition = 0;
        var totalDeduction = 0;
        var netTotal = 0;

        $.each(dataList, function (i, val) {
            debugger;
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

        tded = Math.round(tded);
        tadd = Math.round(tadd);
        //netTotal = parseFloat(parseFloat($scope.summary.grandTotal) + parseFloat($scope.addition) - parseFloat($scope.deduction)).toFixed(2);
        netTotal = parseFloat(parseFloat($scope.summary.grandTotalAmount) + parseFloat(tadd) - parseFloat(tded)).toFixed(2);

        $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;

        if (tded > $scope.tadd) {

            displayPopupNotification("Deduction amount is greater than total amount.", "warning");
        }
        else {


            $scope.deduction = isNaN(totalDeduction) ? 0 : totalDeduction.toFixed(2);
            $scope.addition = isNaN(totalAddition) ? 0 : totalAddition.toFixed(2);
        }

    }

    if ($scope.OrderNo != "undefined") {
        if ($scope.DocumentName != "SA_SALES_INVOICE_JEWEL" && $scope.DocumentName != "SA_SALES_RETURN_JEWEL") {
            $scope.calculateChargeAmount1 = function (dataList, bool) {
                debugger;
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
                tded = Math.round(tded);
                tadd = Math.round(tadd);
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

    }
    //if ($scope.youFromReference==true) {
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

        tded = Math.round(tded);
        tadd = Math.round(tadd);
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
        debugger;
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
    //}

    $scope.calculateChargeAmounteditedit = function (dataList, bool) {
        debugger;
        if ($scope.OrderNo != "undefined" && $scope.youFromReference == false) {
            var sortedObjs = _.sortBy(dataList, 'PRIORITY_INDEX_NO');
            var totalAddition = 0;
            var totalDeduction = 0;
            var netTotal = 0;
            $.each(sortedObjs, function (i, val) {
                debugger;
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
                        var Deduction = parseFloat(charge_amount);
                        //var Deduction = parseFloat(percent_amount);
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


    $scope.calculateChargeAmountediteditNew = function (dataList, bool) {
        debugger;
        if ($scope.OrderNo != "undefined" && $scope.youFromReference == false) {
            var sortedObjs = _.sortBy(dataList, 'PRIORITY_INDEX_NO');
            var totalAddition = 0;
            var totalDeduction = 0;
            var netTotal = 0;
            $.each(sortedObjs, function (i, val) {
                debugger;
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
                        var Deduction = parseFloat(charge_amount);
                        //var Deduction = parseFloat(percent_amount);
                        val.CHARGE_AMOUNT = Deduction;
                        totalDeduction += Deduction;
                        if (charge_amount > 0) {
                            $scope.PrintDiscount = charge_amount;
                        }
                    }
                    else {

                        var Addition = parseFloat(charge_amount);
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
    //}
    //}
    //$scope.OnChargeTypeChecked = function (bool, index, key) {
    //    
    //    var calcTotalAmount = $scope.dynamicInvItenChargeModalData[key].TOTAL_PRICE;
    //    var amountPercent = $('#onamountchange_' + index).val();
    //    calc = ((calcTotalAmount * amountPercent) / 100);
    //    if (bool) {

    //        $scope.dynamicInvItenChargeModalData[key].INV_ITEM_CHARGE_AMOUNT_WISE[index].CALC = calc;
    //    }
    //    else {
    //        $scope.dynamicInvItenChargeModalData[key].INV_ITEM_CHARGE_AMOUNT_WISE[index].CALC = "";
    //    }      
    //};


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

    $scope.isCheckedinvitem = function (CHARGE_CODE) {

        var match = false;
        //if ($scope.data.length > 0) {

        //    for (var i = 0; i < $scope.data.length; i++) {

        //        if ($scope.data[i].CHARGE_CODE == CHARGE_CODE) {
        //            match = true;
        //        }
        //    }
        //    return match;
        //}
        //else {

        //    $scope.adtotal = $scope.summary.grandTotal;
        //    $scope.deduction = 0.00;
        //    $scope.addition = 0.00;
        //}

    };

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
        else if (tablename == "SA_SALES_BLANKET") {
            primarycolumn = "BLANKET_NO";
        }
        else if (tablename == "SA_SALES_CHALAN") {
            primaryDateCol = "CHALAN_DATE";
        }
        else if (tablename == "SA_SALES_INVOICE") {
            primaryDateCol = "SALES_DATE";
        }
        else if (tablename == "SA_SALES_INVOICE_JEWEL") {
            primaryDateCol = "SALES_DATE";
        }
        else if (tablename == "SA_SALES_RETURN_JEWEL") {
            primaryDateCol = "RETURN_DATE";
        }
        return primaryDateCol;
    }

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
        else if (tablename == "IP_TRANSFER_ISSUE" || tablename == "IP_GOODS_ISSUE" || tablename == "IP_GATE_PASS_ENTRY") {
            primarycolumn = "ISSUE_NO";
        }
        else if (tablename == "IP_PURCHASE_ORDER" || tablename == "SA_SALES_ORDER") {
            primarycolumn = "ORDER_NO";
        }
        else if (tablename == "SA_SALES_BLANKET") {
            primarycolumn = "BLANKET_NO";
        }
        else if (tablename == "SA_SALES_CHALAN") {
            primarycolumn = "CHALAN_NO";
        }
        else if (tablename == "SA_SALES_INVOICE") {
            primarycolumn = "SALES_NO";
        }
        else if (tablename == "SA_SALES_INVOICE_JEWEL") {
            primarycolumn = "SALES_NO";
        }
        else if (tablename == "SA_SALES_RETURN_JEWEL") {
            primarycolumn = "RETURN_NO";
        }
        return primarycolumn;
    }

    $scope.ShowRefrence = function () {
        debugger;
        if ($scope.havRefrence == 'Y') {
            //$('#IncludeCharge').val('').prop('disabled', false);
            $('#RefrenceModel').modal('show');
            $('#refrenceTypeMultiSelect').data("kendoDropDownList").value($scope.ref_form_code);
            //$('#TemplateTypeMultiSelect').data("kendoDropDownList").value($scope.ref_form_code);
        }


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

    $scope.add_inv_item_chage = function ($index, event) {

        $scope.ItemName = $($(".cproduct_" + $index)[$(".cproduct_" + $index).length - 1]).data("kendoComboBox").dataItem().ItemDescription;
        window.citemCode = $scope.childModels[$index][ITEM_CODE];
        if ($scope.dynamicInvItenChargeModalData_OK[$index].INV_ITEM_CHARGE_AMOUNT_WISE[0].CALC == "") {
            var InvItemchargeUrl = "/api/TemplateApi/GetItemChargeData?formCode=" + $scope.FormCode + "&itemcode=" + window.citemCode;
            $http.get(InvItemchargeUrl).then(function (response) {

                if (response.data.ChargeOnItemAmountWiseList.length > 0 || response.data.ChargeOnItemQuantityWiseList.length > 0) {
                    $scope.dynamicInvItenChargeModalData[$index].ITEM_CODE = $scope.childModels[$index][ITEM_CODE];
                    $scope.dynamicInvItenChargeModalData[$index].QUANTITY = $scope.childModels[$index].QUANTITY;
                    $scope.dynamicInvItenChargeModalData[$index].UNIT_PRICE = $scope.childModels[$index].UNIT_PRICE;
                    $scope.dynamicInvItenChargeModalData[$index].TOTAL_PRICE = $scope.childModels[$index].TOTAL_PRICE;
                    $scope.dynamicInvItenChargeModalData[$index].CALC_QUANTITY = $scope.childModels[$index].CALC_QUANTITY;
                    $scope.dynamicInvItenChargeModalData[$index].CALC_UNIT_PRICE = $scope.childModels[$index].CALC_UNIT_PRICE;
                    $scope.dynamicInvItenChargeModalData[$index].CALC_TOTAL_PRICE = $scope.childModels[$index].CALC_TOTAL_PRICE;
                    $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_AMOUNT_WISE = [];
                    $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_QUANTITY_WISE = [];
                    $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_AMOUNT_WISE = response.data.ChargeOnItemAmountWiseList;
                    $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_QUANTITY_WISE = response.data.ChargeOnItemQuantityWiseList;
                    if ($scope.dynamicInvItenChargeModalData[$index].QUANTITY != null && $scope.dynamicInvItenChargeModalData[$index].QUANTITY != undefined && $scope.dynamicInvItenChargeModalData[$index].QUANTITY != "" && $scope.dynamicInvItenChargeModalData[$index].UNIT_PRICE != null && $scope.dynamicInvItenChargeModalData[$index].UNIT_PRICE != undefined && $scope.dynamicInvItenChargeModalData[$index].UNIT_PRICE != "") {
                        $('.invitemchargeModal_' + $index).modal('toggle');
                        for (var i = 0; i < response.data.ChargeOnItemAmountWiseList.length; i++) {
                            $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_AMOUNT_WISE[i].GL = "N";
                            var calcTotalAmount = $scope.dynamicInvItenChargeModalData[$index].TOTAL_PRICE;
                            var amountPercent = $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_AMOUNT_WISE[i].VALUE_PERCENT_AMOUNT;
                            var vpa = $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_AMOUNT_WISE[i].VALUE_PERCENT_AMOUNT;
                            var vpf = $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_AMOUNT_WISE[i].VALUE_PERCENT_FLAG;
                            if (vpa != null || vpa != undefined || vpf != null || vpf != undefined) {


                                if ($scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_AMOUNT_WISE[i].VALUE_PERCENT_FLAG === "P") {

                                    calc = ((calcTotalAmount * amountPercent) / 100);
                                    $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_AMOUNT_WISE[i].CALC = calc;
                                }
                                else {
                                    $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_AMOUNT_WISE[i].CALC = amountPercent;
                                }
                            }
                        }
                    }

                }
            });


        }
        else {

            $scope.dynamicInvItenChargeModalData[$index].ITEM_CODE = $scope.childModels[$index][ITEM_CODE];
            $scope.dynamicInvItenChargeModalData[$index].QUANTITY = $scope.childModels[$index].QUANTITY;
            $scope.dynamicInvItenChargeModalData[$index].UNIT_PRICE = $scope.childModels[$index].UNIT_PRICE;
            $scope.dynamicInvItenChargeModalData[$index].TOTAL_PRICE = $scope.childModels[$index].TOTAL_PRICE;
            $scope.dynamicInvItenChargeModalData[$index].CALC_QUANTITY = $scope.childModels[$index].CALC_QUANTITY;
            $scope.dynamicInvItenChargeModalData[$index].CALC_UNIT_PRICE = $scope.childModels[$index].CALC_UNIT_PRICE;
            $scope.dynamicInvItenChargeModalData[$index].CALC_TOTAL_PRICE = $scope.childModels[$index].CALC_TOTAL_PRICE;
            $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_AMOUNT_WISE = [];
            $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_QUANTITY_WISE = [];
            $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_AMOUNT_WISE =
                $scope.dynamicInvItenChargeModalData_OK[$index].INV_ITEM_CHARGE_AMOUNT_WISE;
            $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_QUANTITY_WISE =
                $scope.dynamicInvItenChargeModalData_OK[$index].INV_ITEM_CHARGE_QUANTITY_WISE;
            $('.invitemchargeModal_' + $index).modal('toggle');
        }


    };

    $scope.OnAmountPercentChange = function ($index, key) {

        var unitPrice = $scope.dynamicInvItenChargeModalData[key].UNIT_PRICE;
        var quantity = $scope.dynamicInvItenChargeModalData[key].QUANTITY;
        var calcTotalAmount = unitPrice * quantity;
        //var calcTotalAmount = $scope.dynamicInvItenChargeModalData[key].TOTAL_PRICE;
        var amountPercent = $scope.dynamicInvItenChargeModalData[key].INV_ITEM_CHARGE_AMOUNT_WISE[$index].VALUE_PERCENT_AMOUNT;
        calc = ((calcTotalAmount * amountPercent) / 100);
        $scope.dynamicInvItenChargeModalData[key].INV_ITEM_CHARGE_AMOUNT_WISE[$index].CALC = calc;
    };

    $scope.invitemCharge_Ok = function (index, e) {
        debugger;
        var totalCalcAmount = 0;
        var vpa = 0;
        //angular.forEach($scope.dynamicInvItenChargeModalData, function (value, index) {
        var unitPrice = $scope.dynamicInvItenChargeModalData[index].UNIT_PRICE;
        var quantity = $scope.dynamicInvItenChargeModalData[index].QUANTITY;
        totalCalcAmount = unitPrice * quantity;
        //totalCalcAmount = $scope.dynamicInvItenChargeModalData[index].TOTAL_PRICE;
        angular.forEach($scope.dynamicInvItenChargeModalData[index].INV_ITEM_CHARGE_AMOUNT_WISE, function (val, i) {

            vpa = val.CALC;
            if (val.VALUE_PERCENT_FLAG === 'P') {
                if (val.CHARGE_TYPE == "D") {
                    totalCalcAmount -= vpa;
                }
                else {
                    totalCalcAmount += vpa;
                }
            } else {
                if (val.CHARGE_TYPE == "D") {


                }
                else {


                }
            }

        });
        $scope.dynamicInvItenChargeModalData_OK[index].INV_ITEM_CHARGE_AMOUNT_WISE = $scope.dynamicInvItenChargeModalData[index].INV_ITEM_CHARGE_AMOUNT_WISE;
        $scope.childModels[index].CALC_TOTAL_PRICE = totalCalcAmount;
        $scope.childModels[index].TOTAL_PRICE = totalCalcAmount;

        $scope.GrandtotalCalution();
        //});

    };

    $scope.invitemCharge_Cancel = function ($index, $event) {
        debugger;
        if ($scope.OrderNo === "undefined") {
            var InvItemchargeUrl = "/api/TemplateApi/GetItemChargeData?formCode=" + $scope.FormCode + "&itemcode=" + window.citemCode;
            $http.get(InvItemchargeUrl).then(function (response) {
                debugger;

                if (response.data.ChargeOnItemAmountWiseList.length > 0) {

                    var unitPrice = $scope.dynamicInvItenChargeModalData[$index].UNIT_PRICE;
                    var quantity = $scope.dynamicInvItenChargeModalData[$index].QUANTITY;
                    var totalCalcAmount = unitPrice * quantity;
                    for (var i = 0; i < response.data.ChargeOnItemAmountWiseList.length; i++) {

                        $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_AMOUNT_WISE[i].GL = "N";

                        var amountPercent = response.data.ChargeOnItemAmountWiseList[i].VALUE_PERCENT_AMOUNT;
                        var calc = ((totalCalcAmount * amountPercent) / 100);
                        $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_AMOUNT_WISE[i].CALC = calc;
                        $scope.dynamicInvItenChargeModalData[$index].INV_ITEM_CHARGE_AMOUNT_WISE[i].VALUE_PERCENT_AMOUNT = amountPercent;


                    }
                    $scope.childModels[$index].CALC_TOTAL_PRICE = totalCalcAmount;
                    $scope.childModels[$index].TOTAL_PRICE = totalCalcAmount;
                    $scope.GrandtotalCalution();
                }

            });
        }

        //$('.invitemchargeModal_' + $index).modal('toggle');
    };

    //opera//
    $scope.GetPreferenceSetup = function () {

        var url = window.location.protocol + "//" + window.location.host + "/api/OSApi/GetPreferenceSetup";

        var response = $http({
            method: "get",
            url: url,
        });
        return response.then(function (result) {

            if (result != null) {
                if (result.data.length > 0) {
                    for (var i = 0; i < result.data.length; i++) {
                        if (result.data[i].TYPE == "O" && result.data[i].DISPLAY_FLAG == "Y") {
                            $scope.Opera = true;
                        }
                        if (result.data[i].TYPE == "S" && result.data[i].DISPLAY_FLAG == "Y") {
                            $scope.shymphony = true;
                        }
                    }
                }
            }

        });

    };

    $scope.GetPreferenceSetup();

    $scope.Shymphonydata = function () {

        var queryString = location.href;
        var formCode = queryString.split('/')[7];
        var ShymphonyApi = "/api/ShymphonyApi/Read?formCode=" + formCode;
        $http.get(ShymphonyApi).then(function (res) {

            //res.Sales_No !== "0" && res.Sales_No !== "AE"
            if (res.data.Sales_No !== "0" && res.data.Sales_No !== "AE") {
                var Url = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Home/Index#!DT/formtemplates/" + formCode + "/" + res.data.Sales_No.split(new RegExp('/', 'i')).join('_');
                $window.location.href = Url;
            } else if (res.Sales_No === "AE") {

                displayPopupNotification("No Item Existed", "warning");
            }
            else {
                displayPopupNotification("Same Bill No Already Existed", "warning");
            }
        })
    };


    //$scope.PosPrint = function () {

    //    var vouch_no = $scope.dzvouchernumber.replace(/_/g, '/');
    //    var vouch_no = $scope.OrderNo;
    //    var updateprinturl = "/api/TemplateApi/UpdatePrintCount?VoucherNo=" + vouch_no + "&formcode=" + $scope.FormCode;
    //    var response = $http({
    //        method: "POST",
    //        url: updateprinturl,
    //        contentType: "application/json",
    //        dataType: "json"
    //    });
    //    return response.then(function (data) {
    //        
    //        if (data.data.MESSAGE == "SUCCESS") {
    //            
    //            var getprintcounturl = "/api/TemplateApi/GetPrintCountByVC?voucherno=" + vouch_no;
    //            $http.get(getprintcounturl).then(function (response) {
    //                
    //                if (response.data > 1) {
    //                    $scope.printcounttext = "Copy of Original"
    //                }
    //                else {
    //                    $scope.printcounttext = ""
    //                }
    //            });
    //        }
    //    });

    //};


    //$scope.CheckPrintCount = function () {

    //    var vouch_no = $scope.dzvouchernumber.replace(/_/g, '/');
    //    var vouch_no = $scope.OrderNo;
    //    var updateprinturl = "/api/TemplateApi/UpdatePrintCount?VoucherNo=" + vouch_no + "&formcode=" + $scope.FormCode;
    //    var response = $http({
    //        method: "POST",
    //        url: updateprinturl,
    //        contentType: "application/json",
    //        dataType: "json"
    //    });
    //    return response.then(function (data) {

    //        if (data.data.MESSAGE == "SUCCESS") {

    //            var getprintcounturl = "/api/TemplateApi/GetPrintCountByVoucherNo?voucherno=" + vouch_no + "&formcode=" + $scope.FormCode;;
    //            $http.get(getprintcounturl).then(function (response) {

    //                if (response.data > 1) {
    //                    $scope.printcounttext = "Copy of Original"
    //                }
    //                else {
    //                    $scope.printcounttext = ""
    //                }
    //            });
    //        }
    //    });

    //};

    //$scope.CheckPrintCount = function () {

    //    var vouch_no = $scope.dzvouchernumber.replace(/_/g, '/');
    //    var vouch_no = $scope.OrderNo;
    //    var updateprinturl = "/api/TemplateApi/UpdatePrintCount?VoucherNo=" + vouch_no + "&formcode=" + $scope.FormCode;
    //    var response = $http({
    //        method: "POST",
    //        url: updateprinturl,
    //        contentType: "application/json",
    //        dataType: "json"
    //    });
    //    return response.then(function (data) {

    //        if (data.data.MESSAGE == "SUCCESS") {

    //            var getprintcounturl = "/api/TemplateApi/GetPrintCountByVoucherNo?voucherno=" + vouch_no + "&formcode=" + $scope.FormCode;;
    //            $http.get(getprintcounturl).then(function (response) {

    //                if (response.data > 1) {
    //                    $scope.printcounttext = "Copy of Original"
    //                }
    //                else {
    //                    $scope.printcounttext = ""
    //                }
    //            });
    //        }
    //    });

    //};

    //$scope.CheckPrintCount = function () {

    //    var vouch_no = $scope.dzvouchernumber.replace(/_/g, '/');
    //    var vouch_no = $scope.OrderNo;
    //    var updateprinturl = "/api/TemplateApi/UpdatePrintCount?VoucherNo=" + vouch_no + "&formcode=" + $scope.FormCode;
    //    var response = $http({
    //        method: "POST",
    //        url: updateprinturl,
    //        contentType: "application/json",
    //        dataType: "json"
    //    });
    //    return response.then(function (data) {

    //        if (data.data.MESSAGE == "SUCCESS") {

    //            var getprintcounturl = "/api/TemplateApi/GetPrintCountByVoucherNo?voucherno=" + vouch_no + "&formcode=" + $scope.FormCode;;
    //            $http.get(getprintcounturl).then(function (response) {

    //                if (response.data > 1) {
    //                    $scope.printcounttext = "Copy of Original" + "(" + (response.data - 1) + ")";
    //                    $scope.taxinvoice = "INVOICE";
    //                }
    //                else {
    //                    $scope.taxinvoice = "TAX INVOICE";
    //                    $scope.printcounttext = "";
    //                    $scope.taxinvoice1 = "";
    //                }
    //            });
    //        }
    //    });

    //};
    //AA
    $scope.CheckPrintCount = function () {
        debugger;

        //var vouch_no = $scope.dzvouchernumber.replace(/_/g, '/');
        var vouch_no = $scope.OrderNo;
        var getprintcounturl = "/api/TemplateApi/GetPrintCountByVoucherNo?voucherno=" + vouch_no + "&formcode=" + $scope.FormCode + "&updateCount=false";
        $http.get(getprintcounturl).then(function (response) {
            if ($scope.DocumentName === "SA_SALES_RETURN") {
                $scope.taxinvoice = "Credit Note";
            }
            else {
                if (response.data > 1) {
                    $scope.printcounttext = "Copy of Original" + "(" + (response.data - 1) + ")";
                    $scope.taxinvoice = "INVOICE";
                }
                else {
                    $scope.taxinvoice = "TAX INVOICE";
                    $scope.printcounttext = "Customer Copy";
                    $scope.taxinvoice1 = "";
                }
            }

        });

    };
    $scope.CheckPrintCountAndUpdate = function () {

        var vouch_no = $scope.dzvouchernumber.replace(/_/g, '/');
        var vouch_no = $scope.OrderNo;
        var updateprinturl = "/api/TemplateApi/UpdatePrintCount?VoucherNo=" + vouch_no + "&formcode=" + $scope.FormCode;
        var response = $http({
            method: "POST",
            url: updateprinturl,
            contentType: "application/json",
            dataType: "json"
        });
        return response.then(function (data) {

            if (data.data.MESSAGE == "SUCCESS") {

                var getprintcounturl = "/api/TemplateApi/GetPrintCountByVoucherNo?voucherno=" + vouch_no + "&formcode=" + $scope.FormCode;
                $http.get(getprintcounturl).then(function (response) {
                    if ($scope.DocumentName === "SA_SALES_RETURN") {
                        $scope.taxinvoice = "Credit Note";
                    }
                    else {
                        if (response.data > 1) {
                            $scope.printcounttext = "Copy of Original" + "(" + (response.data - 1) + ")";
                            $scope.taxinvoice = "INVOICE";
                        }
                        else {
                            $scope.taxinvoice = "TAX INVOICE";
                            $scope.printcounttext = "Customer Copy";
                            $scope.taxinvoice1 = "";
                        }
                    }

                });
            }
        });

    };
    //$scope.PrintDocument = function () {





    //    $scope.CheckPrintCount();

    //    $scope.todayDateOpera = $filter('date')(new Date(), 'yyyy-MM-dd');
    //    $scope.printTodayDateTime = $filter('date')(new Date(), 'yyyy-MM-dd HH:mm:ss');

    //    var masterelem = $scope.MasterFormElement;
    //    $.each($scope.MasterFormElement, function (key, value) {
    //        if (value['COLUMN_NAME'].indexOf('CODE') > -1) {

    //            var switched;
    //            switched = value['COLUMN_NAME'];
    //            switch (switched) {
    //                case 'SUPPLIER_CODE':
    //                    $scope.masterModels["SUPPLIER_CODE"] = $('#supplier').data("kendoComboBox").dataItem().SUPPLIER_EDESC;
    //                    break;
    //                case 'ISSUE_TYPE_CODE':
    //                    $scope.masterModels["ISSUE_TYPE_CODE"] = $('#issuetype').data("kendoComboBox").dataItem().ISSUE_TYPE_EDESC;
    //                    break;
    //                case 'TO_BRANCH_CODE':
    //                    $scope.masterModels["TO_BRANCH_CODE"] = $('#branchcode').data("kendoComboBox").dataItem().BRANCH_EDESC;
    //                    break;

    //                case "TO_LOCATION_CODE":
    //                    $scope.masterModels["TO_LOCATION_CODE"] = $('#tolocation').data("kendoComboBox").dataItem().LocationName;
    //                    break;
    //                case "FROM_LOCATION_CODE":
    //                    $scope.masterModels["FROM_LOCATION_CODE"] = $('#location').data("kendoComboBox").dataItem().LocationName;
    //                    break;
    //                case "MASTER_ACC_CODE":
    //                    $scope.masterModels["MASTER_ACC_CODE"] = $('#masteracccode').data("kendoComboBox").dataItem().ACC_EDESC;
    //                    break;
    //                case "CUSTOMER_CODE":
    //                    $scope.masterModels["CUSTOMER_CODE"] = $('#customers').data("kendoComboBox").dataItem().CustomerName;
    //                    $scope.masterModels["REGD_OFFICE_EADDRESS"] = $('#customers').data("kendoComboBox").dataItem().REGD_OFFICE_EADDRESS;
    //                    $scope.masterModels["TPIN_VAT_NO"] = $('#customers').data("kendoComboBox").dataItem().TPIN_VAT_NO;
    //                    $scope.masterModels["TEL_MOBILE_NO1"] = $('#customers').data("kendoComboBox").dataItem().TEL_MOBILE_NO1;
    //                    break;
    //                default:
    //            }

    //        }
    //        $scope.Manual_No_Marriot = $scope.masterModels["MANUAL_NO"];
    //    });
    //    var masterArr = $scope.ChildFormElement[0].element;
    //    var print_master = $.grep(masterArr, function (e) {
    //        return (e['COLUMN_NAME'].indexOf("CALC") === -1 && e['COLUMN_NAME'].indexOf("REMARKS") === -1);
    //    });
    //    var print_child = [];
    //    $.each($scope.ChildFormElement, function (ind, it) {
    //        print_child.push({
    //            element: $.grep(it.element, function (e) {

    //                var switch_on;
    //                switch_on = e['COLUMN_NAME'];
    //                switch (switch_on) {
    //                    case 'ITEM_CODE':
    //                        $scope.childModels[ind]["ITEM_CODE"] = $($(".cproduct_" + ind)[$(".cproduct_" + ind).length - 1]).data("kendoComboBox").dataItem().ItemDescription;
    //                        break;
    //                    case 'PRODUCT_CODE':
    //                        $scope.childModels[ind]["ITEM_CODE"] = $($(".cproduct_" + ind)[$(".cproduct_" + ind).length - 1]).data("kendoComboBox").dataItem().ItemDescription;
    //                        break;
    //                    case 'ACC_CODE':
    //                        $scope.childModels[ind]["ACC_CODE"] = $($(".caccount_" + ind)[$(".caccount_" + ind).length - 1]).data("kendoComboBox").dataItem().ACC_EDESC;
    //                        break;

    //                    case "TO_LOCATION_CODE":
    //                        $scope.childModels[ind]["TO_LOCATION_CODE"] = $($(".ctolocation_" + ind)[$(".ctolocation_" + ind).length - 1]).data("kendoComboBox").dataItem().LocationNames;
    //                        break;
    //                    case "FROM_LOCATION_CODE":
    //                        $scope.childModels[ind]["FROM_LOCATION_CODE"] = $($(".clocation_" + ind)[$(".clocation_" + ind).length - 1]).data("kendoComboBox").dataItem().LocationName;
    //                        break;
    //                    default:
    //                }
    //                return (e['COLUMN_NAME'].indexOf("CALC") === -1 && e['COLUMN_NAME'].indexOf("REMARKS") === -1);
    //            }),
    //            additionalElements: ''
    //        });
    //    });

    //    for (var i = 0; i < $scope.childModels.length; i++) {
    //        if ("FROM_LOCATION_CODE" in $scope.childModels[i]) {
    //            $scope.childModels[i]["FROM_LOCATION_CODE"] = undefined;
    //        }
    //        if ("CALC_QUANTITY" in $scope.childModels[i]) {
    //            $scope.childModels[i]["CALC_QUANTITY"] = undefined;
    //        }
    //        if ("CALC_TOTAL_PRICE" in $scope.childModels[i]) {
    //            $scope.childModels[i]["CALC_TOTAL_PRICE"] = undefined;
    //        }
    //        if ("CALC_UNIT_PRICE" in $scope.childModels[i]) {
    //            $scope.childModels[i]["CALC_UNIT_PRICE"] = undefined;
    //        }
    //        if ("STOCK_BLOCK_FLAG" in $scope.childModels[i]) {
    //            $scope.childModels[i]["STOCK_BLOCK_FLAG"] = undefined;
    //        }
    //        if ("COMPLETED_QUANTITY" in $scope.childModels[i]) {
    //            $scope.childModels[i]["COMPLETED_QUANTITY"] = undefined;
    //        }

    //    };

    //    $scope.childModels = JSON.parse(JSON.stringify($scope.childModels));

    //    $scope.print_header = print_master;
    //    $scope.print_body_col = print_child;
    //    for (var i = 0; i < $scope.print_body_col.length; i++) {

    //        $scope.print_body_col[i].element = $.grep($scope.print_body_col[i].element, function (v) { return ($.inArray(v.COLUMN_NAME, ["FROM_LOCATION_CODE", "STOCK_BLOCK_FLAG", "COMPLETED_QUANTITY"]) == -1) });

    //    }

    //    if ($scope.PrintDiscount > 0) {
    //        $scope.PrintDiscountShow = true;
    //    }

    //    $scope.print_body_col = JSON.parse(JSON.stringify($scope.print_body_col));
    //    var queryString = location.href;
    //    $scope.dzvouchernumber = queryString.split('/')[8];
    //    $scope.dzvoucherdate = moment($scope.dzvouchernumber).format('DD-MMM-YYYY');
    //    $scope.dzformcode = queryString.split('/')[7];
    //    $scope.TodayDate = AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD'))
    //    $scope.amountinword = convertNumberToWords($scope.adtotal);
    //    $("#OperaPrintModal").modal("toggle");


    //}

    //$scope.refresh = function () {
    //    $("#OperaPrintModal").modal("toggle");
    //    // $route.reload();
    //    location.reload();

    //}
    //end//

    $scope.NewPrintDocument = function () {
        debugger;

        var url = '/Print/Home/PreviewPattern?formCode=' + $scope.FormCode + '&companyCode=' + $scope.companycode + '&mainFieldValue=' + encodeURIComponent($scope.OrderNo);
        window.open(url, '_blank');

    }
    $scope.PrintDocument = function () {
        debugger;
        //var guestInfo = "/api/TemplateApi/GetGuestInfoFromMasterTransaction?formCode=" + $scope.FormCode + "&orderno=" + $scope.OrderNo;
        //$http.get(guestInfo).then(function (guestInfodetails) {

        //    $scope.guestDetails = guestInfodetails.data;


        //    $scope.ArrivalDate = $scope.guestDetails.CR_LMT1;
        //    $scope.DepartuteDate = $scope.guestDetails.CR_LMT2;
        //    $scope.RoomNumber = $scope.guestDetails.CR_LMT3;
        //    $scope.BillNumber = $scope.guestDetails.REFERENCE_NO;

        //});
        //var guestInfo = "/api/TemplateApi/GetCompanyInfo";

        $.when(d95).done(function (result) {

            if (result.data != null) {

                $scope.DocumentName = result.data.TABLE_NAME;
                $scope.companycode = result.data.COMPANY_CODE;

                $scope.printCompanyInfo.companyName = result.data.COMPANY_EDESC;
                $scope.printCompanyInfo.address = result.data.ADDRESS;
                //$scope.printCompanyInfo.formName = result.data.FORM_EDESC;
                $scope.printCompanyInfo.phoneNo = rresult.data.TELEPHONE;
                $scope.printCompanyInfo.email = result.data.EMAIL;
                $scope.printCompanyInfo.ComPanNo = result.data.TPIN_VAT_NO;

            }


        });
        debugger;
        var vouch_no = $scope.OrderNo;
        $scope.voucher_no = $scope.OrderNo;
        var getrefnourl = "/api/TemplateApi/getRefNo?orderno=" + vouch_no;
        $http.get(getrefnourl).then(function (response) {
            debugger;
            //if (response.data > 1) {
            if (response.data != null) {
                $scope.refordernoP = response.data.ORDER_NO;
                $scope.reforderdateP = moment(response.data.ORDER_DATE).format('DD-MMM-YYYY');
            }
            //$scope.refordernoP = response.data.ORDER_NO;
            //$scope.reforderdateP = moment(response.data.ORDER_DATE).format('DD-MMM-YYYY');
            //}
        });

        var getcompanyAndBranchDataModel = "/api/TemplateApi/getcompanyAndBranchDataModel";

        $http.get(getcompanyAndBranchDataModel).then(function (response) {
            debugger;
            $scope.companyAndBranchDataModel = response.data;
        });

        var salesInvoiceJewelleryDateTimeModel = "/api/TemplateApi/getSalesInvoiceJewelleryDateTimeModel?salesno=" + vouch_no + "&tableName=" + $scope.DocumentName;

        $http.get(salesInvoiceJewelleryDateTimeModel).then(function (response) {
            debugger;
            $scope.salesInvoiceJewelleryDateTime = response.data;


            var customerCode = $('#customers').data("kendoComboBox").dataItem().CustomerName;
            var customerName = $('#customers').data("kendoComboBox").dataItem().CustomerCode;
            var getCustomerPanNoByCustomerCode = "/api/TemplateApi/getCustomerPanNoByCustomerCode?customerCode=" + customerCode;
            var customerPanNo = "";

            $scope.CheckPrintCount();

            $scope.todayDateOpera = $filter('date')(new Date(), 'yyyy-MM-dd');
            $scope.printTodayDateTime = $filter('date')(new Date(), 'yyyy-MM-dd HH:mm:ss');

            var masterelem = $scope.MasterFormElement;
            $.each($scope.MasterFormElement, function (key, value) {
                debugger;
                if (value['COLUMN_NAME'].indexOf('CODE') > -1) {

                    var switched;
                    switched = value['COLUMN_NAME'];
                    switch (switched) {
                        case 'SUPPLIER_CODE':
                            $scope.masterModels["SUPPLIER_CODE"] = $('#supplier').data("kendoComboBox").dataItem().SUPPLIER_EDESC;
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
                        case "EMPLOYEE_CODE":
                            $scope.masterModels["EMPLOYEE_CODE"] = $('#employee').data("kendoComboBox").dataItem().EMPLOYEE_EDESC;
                            break;
                        case "CUSTOMER_CODE":
                            debugger;
                            $scope.masterModels["CUSTOMER_CODE"] = $('#customers').data("kendoComboBox").dataItem().CustomerName;
                            $scope.masterModels["REGD_OFFICE_EADDRESS"] = $('#customers').data("kendoComboBox").dataItem().REGD_OFFICE_EADDRESS;
                            $scope.masterModels["TPIN_VAT_NO"] = $('#customers').data("kendoComboBox").dataItem().TPIN_VAT_NO;
                            $scope.masterModels["PAN_NO"] = $('#customers').data("kendoComboBox").dataItem().PAN_NO;
                            $scope.masterModels["TEL_MOBILE_NO1"] = $('#customers').data("kendoComboBox").dataItem().TEL_MOBILE_NO1;
                            $scope.masterModels["GuestName"] = $('#customers').data("kendoComboBox").dataItem().CUSTOMER_NDESC;
                            //$scope.TPIN_VAT_NO_customer = $('#customers').data("kendoComboBox").dataItem().TPIN_VAT_NO;
                            $scope.TPIN_VAT_NO = $('#customers').data("kendoComboBox").dataItem().TPIN_VAT_NO;
                            break;

                        default:
                    }

                }
                debugger;
                if (value['COLUMN_NAME'] == "SALES_DATE") {

                    $scope.masterModels["SALES_DATE"] = $scope.salesInvoiceJewelleryDateTime;


                }
                if (value['COLUMN_NAME'] == "RETURN_DATE") {

                    $scope.masterModels["RETURN_DATE"] = $scope.salesInvoiceJewelleryDateTime;


                }
                if (value['COLUMN_NAME'] == "PAYMENT_MODE") {

                    var dropDownList = $("#currency").data("kendoDropDownList");

                    if (dropDownList) {
                        $scope.masterModels["PAYMENT_MODE"] = dropDownList.dataItem().PAYMENT_MODE_EDESC;
                    }


                }
                $scope.Manual_No_Marriot = $scope.masterModels["MANUAL_NO"];
            });

        });


        //$http.get(getCustomerPanNoByCustomerCode).then(function (response) {
        //    debugger;
        //    $('#customers').data("kendoComboBox").dataItem().PAN_NO = response.data;


        //    $scope.CheckPrintCount();

        //    $scope.todayDateOpera = $filter('date')(new Date(), 'yyyy-MM-dd');
        //    $scope.printTodayDateTime = $filter('date')(new Date(), 'yyyy-MM-dd HH:mm:ss');

        //    var masterelem = $scope.MasterFormElement;
        //    $.each($scope.MasterFormElement, function (key, value) {
        //        debugger;
        //        if (value['COLUMN_NAME'].indexOf('CODE') > -1) {

        //            var switched;
        //            switched = value['COLUMN_NAME'];
        //            switch (switched) {
        //                case 'SUPPLIER_CODE':
        //                    $scope.masterModels["SUPPLIER_CODE"] = $('#supplier').data("kendoComboBox").dataItem().SUPPLIER_EDESC;
        //                    break;
        //                case 'ISSUE_TYPE_CODE':
        //                    $scope.masterModels["ISSUE_TYPE_CODE"] = $('#issuetype').data("kendoComboBox").dataItem().ISSUE_TYPE_EDESC;
        //                    break;
        //                case 'TO_BRANCH_CODE':
        //                    $scope.masterModels["TO_BRANCH_CODE"] = $('#branchcode').data("kendoComboBox").dataItem().BRANCH_EDESC;
        //                    break;

        //                case "TO_LOCATION_CODE":
        //                    $scope.masterModels["TO_LOCATION_CODE"] = $('#tolocation').data("kendoComboBox").dataItem().LocationName;
        //                    break;
        //                case "FROM_LOCATION_CODE":
        //                    $scope.masterModels["FROM_LOCATION_CODE"] = $('#location').data("kendoComboBox").dataItem().LocationName;
        //                    break;
        //                case "MASTER_ACC_CODE":
        //                    $scope.masterModels["MASTER_ACC_CODE"] = $('#masteracccode').data("kendoComboBox").dataItem().ACC_EDESC;
        //                    break;
        //                case "EMPLOYEE_CODE":
        //                    $scope.masterModels["EMPLOYEE_CODE"] = $('#employee').data("kendoComboBox").dataItem().EMPLOYEE_EDESC;
        //                    break;
        //                case "CUSTOMER_CODE":
        //                    debugger;
        //                    $scope.masterModels["CUSTOMER_CODE"] = $('#customers').data("kendoComboBox").dataItem().CustomerName;
        //                    $scope.masterModels["REGD_OFFICE_EADDRESS"] = $('#customers').data("kendoComboBox").dataItem().REGD_OFFICE_EADDRESS;
        //                    $scope.masterModels["TPIN_VAT_NO"] = $('#customers').data("kendoComboBox").dataItem().TPIN_VAT_NO;
        //                    $scope.masterModels["PAN_NO"] = $('#customers').data("kendoComboBox").dataItem().PAN_NO;
        //                    $scope.masterModels["TEL_MOBILE_NO1"] = $('#customers').data("kendoComboBox").dataItem().TEL_MOBILE_NO1;
        //                    $scope.masterModels["GuestName"] = $('#customers').data("kendoComboBox").dataItem().CUSTOMER_NDESC;
        //                    //$scope.TPIN_VAT_NO_customer = $('#customers').data("kendoComboBox").dataItem().TPIN_VAT_NO;
        //                    $scope.TPIN_VAT_NO = $('#customers').data("kendoComboBox").dataItem().TPIN_VAT_NO;
        //                    break;

        //                default:
        //            }

        //        }

        //        if (value['COLUMN_NAME'] == "PAYMENT_MODE") {

        //            var dropDownList = $("#currency").data("kendoDropDownList");

        //            if (dropDownList) {
        //                $scope.masterModels["PAYMENT_MODE"] = dropDownList.dataItem().PAYMENT_MODE_EDESC;
        //            }


        //        }
        //        $scope.Manual_No_Marriot = $scope.masterModels["MANUAL_NO"];
        //    });
        //});


        //var form_code = $scope.FormCode;

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
                            $scope.childModels[ind]["TO_LOCATION_CODE"] = $($(".ctolocation_" + ind)[$(".ctolocation_" + ind).length - 1]).data("kendoComboBox").dataItem().LocationNames;
                            break;
                        case "FROM_LOCATION_CODE":
                            $scope.childModels[ind]["FROM_LOCATION_CODE"] = $($(".clocation_" + ind)[$(".clocation_" + ind).length - 1]).data("kendoComboBox").dataItem().LocationName;
                            break;
                        default:
                    }
                    return (e['COLUMN_NAME'].indexOf("CALC") === -1);
                }),
                additionalElements: ''
            });
        });
        if ($scope.DocumentName == "SA_SALES_CHALAN") {
            for (var i = 0; i < $scope.childModels.length; i++) {
                if ("FROM_LOCATION_CODE" in $scope.childModels[i]) {
                    $scope.childModels[i]["FROM_LOCATION_CODE"] = undefined;
                }
                if ("CALC_QUANTITY" in $scope.childModels[i]) {
                    $scope.childModels[i]["CALC_QUANTITY"] = undefined;
                }
                if ("CALC_TOTAL_PRICE" in $scope.childModels[i]) {
                    $scope.childModels[i]["CALC_TOTAL_PRICE"] = undefined;
                }
                if ("CALC_UNIT_PRICE" in $scope.childModels[i]) {
                    $scope.childModels[i]["CALC_UNIT_PRICE"] = undefined;
                }
                if ("STOCK_BLOCK_FLAG" in $scope.childModels[i]) {
                    $scope.childModels[i]["STOCK_BLOCK_FLAG"] = undefined;
                }
                if ("COMPLETED_QUANTITY" in $scope.childModels[i]) {
                    $scope.childModels[i]["COMPLETED_QUANTITY"] = undefined;
                }
                if ("TOTAL_PRICE" in $scope.childModels[i]) {
                    $scope.childModels[i]["TOTAL_PRICE"] = undefined;
                }
                if ("UNIT_PRICE" in $scope.childModels[i]) {
                    $scope.childModels[i]["UNIT_PRICE"] = undefined;
                }

            };
        }
        else {
            for (var i = 0; i < $scope.childModels.length; i++) {
                if ("FROM_LOCATION_CODE" in $scope.childModels[i]) {
                    $scope.childModels[i]["FROM_LOCATION_CODE"] = undefined;
                }
                if ("CALC_QUANTITY" in $scope.childModels[i]) {
                    $scope.childModels[i]["CALC_QUANTITY"] = undefined;
                }
                if ("CALC_TOTAL_PRICE" in $scope.childModels[i]) {
                    $scope.childModels[i]["CALC_TOTAL_PRICE"] = undefined;
                }
                if ("CALC_UNIT_PRICE" in $scope.childModels[i]) {
                    $scope.childModels[i]["CALC_UNIT_PRICE"] = undefined;
                }
                if ("STOCK_BLOCK_FLAG" in $scope.childModels[i]) {
                    $scope.childModels[i]["STOCK_BLOCK_FLAG"] = undefined;
                }
                if ("COMPLETED_QUANTITY" in $scope.childModels[i]) {
                    $scope.childModels[i]["COMPLETED_QUANTITY"] = undefined;
                }
                if ("ALT1_MU_CODE" in $scope.childModels[i]) {
                    $scope.childModels[i]["ALT1_MU_CODE"] = undefined;
                }
                if ("ALT1_QUANTITY" in $scope.childModels[i]) {
                    $scope.childModels[i]["ALT1_QUANTITY"] = undefined;
                }
                //if ("SECOND_QUANTITY" in $scope.childModels[i]) {
                //    $scope.childModels[i]["SECOND_QUANTITY"] = undefined;
                //}


            };
        }

        debugger;
        $scope.childModels = JSON.parse(JSON.stringify($scope.childModels));

        for (var i = 0; i < $scope.childModels.length; i++) {
            $scope.childModels[i]["SN"] = i + 1;
        }
        console.log($scope.childModels);
        $scope.print_header = print_master;
        $scope.print_body_col = print_child;
        if ($scope.DocumentName == "SA_SALES_CHALAN") {
            for (var i = 0; i < $scope.print_body_col.length; i++) {

                $scope.print_body_col[i].element = $.grep($scope.print_body_col[i].element, function (v) { return ($.inArray(v.COLUMN_NAME, ["FROM_LOCATION_CODE", "STOCK_BLOCK_FLAG", "COMPLETED_QUANTITY", "TOTAL_PRICE", "UNIT_PRICE"]) == -1) });

            }
        }
        else {
            for (var i = 0; i < $scope.print_body_col.length; i++) {
                debugger;
                $scope.print_body_col[i].element = $.grep($scope.print_body_col[i].element, function (v) { return ($.inArray(v.COLUMN_NAME, ["FROM_LOCATION_CODE", "STOCK_BLOCK_FLAG", "COMPLETED_QUANTITY", "ALT1_MU_CODE", "ALT1_QUANTITY"]) == -1) });

            }
        }
        debugger;
        if ($scope.PrintDiscount > 0) {
            $scope.PrintDiscountShow = true;
        }
        $scope.TOTAL_QUANTITY = 0;
        $scope.SUB_TOTAL = 0.00;
        $.each($scope.childModels, function (indN, itN) {

            $scope.TOTAL_QUANTITY = $scope.TOTAL_QUANTITY + itN.QUANTITY;
            $scope.SUB_TOTAL = $scope.SUB_TOTAL + itN.TOTAL_PRICE;
        });
        $scope.print_body_col = JSON.parse(JSON.stringify($scope.print_body_col));
        debugger;
        $scope.chunkedProducts = [];
        for (let i = 0; i < $scope.childModels.length; i += chunkSize) {
            let chunk = $scope.childModels.slice(i, i + chunkSize);
            $scope.chunkedProducts.push(chunk);
        }

        console.log($scope.print_body_col);
        console.log($scope.chunkedProducts);

        var queryString = location.href;
        $scope.dzvouchernumber = queryString.split('/')[8];
        $scope.dzvoucherdate = moment($scope.dzvoucherdate).format('DD-MMM-YYYY');
        $scope.dzformcode = queryString.split('/')[7];
        $scope.TodayDate = AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD'))
        $scope.amountinword = convertNumberToWords($scope.adtotal);
        debugger;
        if ($scope.PrintPathUl == "" || $scope.PrintPathUl == undefined || $scope.PrintPathUl == "undefined") {
            displayPopupNotification("Please perform print teplate setup first.", "warning");
        }
        else {
            $("#saveAndPrintSalesModal").modal("toggle");
        }
        //$("#OperaPrintModal").modal("toggle");
    }

    $scope.refresh = function () {
        $("#OperaPrintModal").modal("toggle");
        // $route.reload();
        location.reload();

    }

    //$scope.printDiv1 = function (divName) {
    //    var printContents = document.getElementById(divName).innerHTML;
    //    var popupWin = window.open('', '_blank', 'width=800,height=800', 'orientation = portrait');        
    //    popupWin.document.open();
    //    popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
    //    popupWin.document.close();
    //    $("#OperaPrintModal").modal("toggle");
    //}

    //$scope.printDiv1 = function (divName) {

    //    var vouch_no = $scope.dzvouchernumber.replace(/_/g, '/');
    //    var vouch_no = $scope.OrderNo;
    //    var getprintcounturl = "/api/TemplateApi/GetPrintCountByVoucherNo?voucherno=" + vouch_no + "&formcode=" + $scope.FormCode;;
    //    $http.get(getprintcounturl).then(function (response) {

    //        if (response.data > 1) {
    //            var printContents = document.getElementById(divName).innerHTML;

    //            var popupWin = window.open('', '_blank', 'width=800,height=800', 'orientation = portrait');
    //            popupWin.document.open();
    //            popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
    //            popupWin.document.close();
    //        }
    //        else {

    //            for (var i = 0; i < 3; i++) {
    //                if (i == 0) {
    //                    $("#TaxINvoiceTest").html("TAX INVOICE");
    //                    $("#TaxInvoiceCopy").html("Customer Copy");
    //                }
    //                else if (i == 1) {
    //                    $("#TaxINvoiceTest").html("INVOICE");
    //                    $("#TaxInvoiceCopy").html("Office Copy");
    //                }
    //                else {
    //                    $("#TaxINvoiceTest").html("INVOICE");
    //                    $("#TaxInvoiceCopy").html("Tax Copy");
    //                }
    //                var printContents = document.getElementById(divName).innerHTML;

    //                var popupWin = window.open('', '_blank', 'width=800,height=800', 'orientation = portrait');
    //                popupWin.document.open();

    //                popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
    //                popupWin.document.close();
    //            }
    //        }
    //    });

    //    $("#OperaPrintModal").modal("toggle");
    //}

    $scope.printDiv1 = function (divName) {
        debugger;
        if ($scope.dzvouchernumber != undefined) {
            var vouch_no = $scope.dzvouchernumber.replace(/_/g, '/');
        }
        var vouch_no = $scope.OrderNo;
        var getprintcounturl = "/api/TemplateApi/GetPrintCountByVoucherNo?voucherno=" + vouch_no + "&formcode=" + $scope.FormCode + "&updateCount=true";
        $http.get(getprintcounturl).then(function (response) {
            if ($scope.DocumentName === "SA_SALES_RETURN") {
                $("#TaxINvoiceTest").html("CREDIT NOTE");
                var printContents = document.getElementById(divName).innerHTML;

                var popupWin = window.open('', '_blank', 'width=800,height=800', 'orientation = portrait');
                popupWin.document.open();
                popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
                popupWin.document.close();
            }
            else {
                if (response.data > 1) {
                    var printContents = document.getElementById(divName).innerHTML;

                    var popupWin = window.open('', '_blank', 'width=800,height=800', 'orientation = portrait');
                    popupWin.document.open();
                    popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
                    popupWin.document.close();
                }
                else {
                    //AA

                    for (var i = 0; i < 3; i++) {
                        if (i == 0) {
                            $("#TaxINvoiceTest").html("TAX INVOICE");
                            $("#TaxInvoiceCopy").html("Customer Copy");
                        }
                        else if (i == 1) {
                            $("#TaxINvoiceTest").html("INVOICE");
                            $("#TaxInvoiceCopy").html("Office Copy");
                        }
                        //AA
                        else if (i == 2) {
                            $("#TaxINvoiceTest").html("INVOICE");
                            $("#TaxInvoiceCopy").html("Tax Copy");
                        }
                        var printContents = document.getElementById(divName).innerHTML;

                        var popupWin = window.open('', '_blank', 'width=800,height=800', 'orientation = portrait');
                        popupWin.document.open();

                        popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
                        popupWin.document.close();
                    }
                }
            }
        });

        $("#OperaPrintModal").modal("toggle");
    }

    $scope.printDiv2 = function (divName) {


        var printContents = document.getElementById(divName).innerHTML;

        var popupWin = window.open('', '_blank', 'width=800,height=800', 'orientation = portrait');
        //popupWin.ScreenOrientation = "Portrait";
        popupWin.document.open();
        popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
        popupWin.document.close();
        $("#OperaPrintModal1").modal("toggle");
        //$scope.masterChildData = [];
        //angular.forEach($scope.MasterFormElement, function (value, key) {

        //    if (value.MASTER_CHILD_FLAG == 'M' && value.DISPLAY_FLAG == 'Y') {
        //        $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
        //        $scope.masterModels[value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
        //        if (value['COLUMN_NAME'].indexOf('DATE') > -1) {
        //            $scope.masterModels[value['COLUMN_NAME']] = $filter('date')(new Date(), 'dd-MMM-yyyy');
        //        }
        //    }
        //});

        //var cml = $scope.childModels.length;
        //var sl = parseFloat(cml) - 1;
        //$scope.ChildFormElement.splice(0, sl);
        //$scope.childModels.splice(0, sl);
        //angular.forEach($scope.ChildFormElement[0].element, function (value, key) {

        //    if (value.MASTER_CHILD_FLAG == 'C' && value.DISPLAY_FLAG == 'Y') {
        //        $scope.childModels[0][value.COLUMN_NAME] = value.DEFA_VALUE == null ? "" : value.DEFA_VALUE;
        //    }
        //});

        //$scope.summary.grandTotal = 0;

        //$scope.summary = { 'grandTotal': 0 };
        //$scope.units = [];
        //$scope.totalQty = 0;


    }

    $scope.StandardItemRate = function (customercode, formcode, aeracode, itemcode, key) {

        var sIRUrl = "/api/TemplateApi/GetStarndardRate?customercode=" + customercode + "&&formcode=" + formcode + "&&areacode=" + aeracode + "&&itemcode=" + itemcode;
        $http.get(sIRUrl).then(function (response) {



            var childlenn = $scope.childModels.length;
            for (var i = 0; i < childlenn; i++) {

                if ($scope.childModels[i].hasOwnProperty("UNIT_PRICE")) {

                    $scope.childModels[key]["UNIT_PRICE"] = response.data;
                };
            }



        });


    };

    $scope.PricelistItemRate = function (masterid, itemcode, key) {

        var sIRUrl = "/api/TemplateApi/GetItemRateRateMasterId?masterid=" + masterid + "&&itemcode=" + itemcode;
        $http.get(sIRUrl).then(function (response) {



            var childlenn = $scope.childModels.length;
            for (var i = 0; i < childlenn; i++) {

                if ($scope.childModels[i].hasOwnProperty("UNIT_PRICE")) {

                    $scope.childModels[key]["UNIT_PRICE"] = response.data;

                };
            }



        });


    };

    var ptemplateurl = "/api/TemplateApi/GetPrintTemplateName?formCode=" + $scope.FormCode;
    $http.get(ptemplateurl).then(function (response) {

        $scope.PrintPathUl = "~/Areas/NeoERP.DocumentTemplate/Views/Shared/PrintTemplate/" + response.data + ".cshtml";

    });

    if ($scope.OrderNo != undefined && $scope.OrderNo != "undefined") {
        function BindPriceList(CustomerCode, searchText) {
            var getpricelistByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetPriceListByFilterAndCustomerCode?filter=" + searchText + '&customercode=' + CustomerCode;
            $("#customerpriceid").kendoComboBox({
                optionLabel: "--Select Price List--",
                filter: "contains",
                dataTextField: "PRICE_LIST_NAME",
                dataValueField: "MASTER_ID",

                autobind: true,
                suggest: true,
                dataBound: function (e) {

                    if (this.select() === -1) { //check whether any item is selected
                        this.select(0);
                        this.trigger("change");
                    }
                },
                dataSource: {
                    type: "json",
                    serverFiltering: true,
                    transport: {
                        read: {

                            url: getpricelistByUrl,

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
                select: function (e) {

                    $('#style-switcher').addClass('opened');
                    $('#style-switcher').animate({ 'left': '-241px', 'width': '273px' });

                }

            });
        }

    }

    if ($scope.OrderNo != undefined && $scope.OrderNo != "undefined") {
        debugger;
        $scope.refBGridOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: "/api/TemplateApi/getRefDetails?VoucherNo=" + $scope.OrderNo + '&formcode=' + $scope.FormCode,
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

    //$scope.bindBatchGrid = function (itemcode, locationcode) {
    //    
    //    var checkedItems = [];
    //    var checkedIds = {};
    //    $scope.batchGridOptions = {
    //        dataSource: {
    //            type: "json",

    //            transport: {
    //                read: "/api/TemplateApi/GetDataForBatchModalsales?itemcode=" + itemcode + "&loactioncode=" + locationcode,
    //            },
    //            pageSize: 5,
    //            serverPaging: false,
    //            serverSorting: false
    //        },
    //        sortable: false,
    //        pageable: false,
    //        dataBound: function (e) {
    //            
    //            //$scope.detailExportPromises = [];
    //            $('div').removeClass('.k-header k-grid-toolbar');

    //            $(".checkbox").on("click", selectRow);

    //            var view = this.dataSource.data();
    //            for (var j = 0; j < checkedItems.length; j++) {
    //                for (var i = 0; i < view.length; i++) {
    //                    if (checkedItems[j].TRANSACTION_NO == view[i].TRANSACTION_NO) {
    //                        this.tbody.find("tr[data-uid='" + view[i].uid + "']")
    //                            .addClass("k-state-selected")
    //                            .find(".checkbox")
    //                            .attr("checked", "checked");
    //                    }
    //                }

    //            }

    //            var grid = e.sender;
    //            if (grid.dataSource.total() == 0) {
    //                var colCount = grid.columns.length + 1;
    //                $(e.sender.wrapper)
    //                    .find('tbody')
    //                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, No Data Found For Given Filter. </td></tr>');

    //            }

    //        },
    //        columns: [
    //            {
    //                template: function (dataItem) {
    //                    return "<input type='checkbox' id='${dataItem.TRANSACTION_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.TRANSACTION_NO}'></label>"
    //                },
    //                width: 20
    //            },
    //            {
    //                field: "TRACKING_SERIAL_NO",
    //                title: "Batch NO",
    //                width: "40px"
    //            }
    //        ]
    //    };
    //}

    $scope.SerialTrackingOK = function (key) {

        checkedItems;

        for (var f = 0; f < checkedItems.length; f++) {

            $scope.checkedresult.push({
                //"TRACKING_SERIAL_NO": dataItem.TRACKING_SERIAL_NO,
                "ITEM_CODE": checkedItems[f].ITEM_CODE,
                "MU_CODE": checkedItems[f].MU_CODE,
                "LOCATION_CODE": checkedItems[f].LOCATION_CODE,
                "SERIAL_NO": checkedItems[f].SERIAL_NO,
                "TRACKING_SERIAL_NO": checkedItems[f].TRACKING_SERIAL_NO
            });
        }
        //if ($scope.checkedresult != undefined) {
        //    $scope.childModels[key].QUANTITY = $scope.checkedresult.length;
        //}


    }
    $scope.ShowBatchTransDetail = function ($index) {

        //var a = $scope.dynamicSerialTrackingModalData;
        var urlreference = "";
        if ($scope.OrderNo != "undefined" && $scope.youFromReference == false) {
            urlreference = "/api/TemplateApi/GetDataForBatchModalsalesforedit?itemcode=" + $scope.childModels[$index].ITEM_CODE
                + "&loactioncode=" + $scope.childModels[$index].FROM_LOCATION_CODE + "&voucherno=" + $scope.OrderNo;

        }
        else if ($scope.OrderNo == "undefined" && $scope.youFromReference == true) {
            urlreference = "/api/TemplateApi/GetDataForBatchModalsales?itemcode=" + $scope.childModels[$index].ITEM_CODE
                + "&loactioncode=" + $scope.childModels[$index].FROM_LOCATION_CODE;

        }
        else {
            urlreference = "/api/TemplateApi/GetDataForBatchModalsales?itemcode=" + $scope.childModels[$index].ITEM_CODE
                + "&loactioncode=" + $scope.childModels[$index].FROM_LOCATION_CODE;
        }
        checkedItems = [];
        var checkedIds = {};
        $scope.batchGridOptions = {
            dataSource: {
                type: "json",

                transport: {
                    read: urlreference,
                },
                //pageSize: 5,
                serverPaging: false,
                serverSorting: false
            },
            sortable: false,
            pageable: false,
            dataBound: function (e) {

                //$scope.detailExportPromises = [];
                $('div').removeClass('.k-header k-grid-toolbar');
                //if ($scope.OrderNo != "" && $scope.youFromReference == false)
                //{
                //    var viewfordefcheck = this.dataSource.data();                    
                //    for (var i = 0; i < viewfordefcheck.length; i++) {
                //        if (viewfordefcheck[i].SOURCE_FLAG=="O") {
                //            this.tbody.find("tr[data-uid='" + viewfordefcheck[i].uid + "']")
                //                    .addClass("k-state-selected")
                //                    .find(".checkbox")
                //                    .attr("checked", "checked");
                //            }
                //        }                   
                //}

                $(".checkbox").on("click", selectRow);

                var view = this.dataSource.data();
                for (var j = 0; j < checkedItems.length; j++) {
                    for (var i = 0; i < view.length; i++) {
                        if (checkedItems[j].TRANSACTION_NO == view[i].TRANSACTION_NO) {
                            this.tbody.find("tr[data-uid='" + view[i].uid + "']")
                                .addClass("k-state-selected")
                                .find(".checkbox")
                                .attr("checked", "checked");
                        }
                    }

                }

                var grid = e.sender;
                if (grid.dataSource.total() == 0) {
                    var colCount = grid.columns.length + 1;
                    $(e.sender.wrapper)
                        .find('tbody')
                        .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, No Data Found For Given Filter. </td></tr>');

                }

            },
            columns: [
                {
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.TRANSACTION_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.TRANSACTION_NO}'></label>"
                    },
                    width: 20
                },
                {
                    field: "TRACKING_SERIAL_NO",
                    title: "Serial NO",
                    width: "40px"
                }
            ]
        };
        function selectRow() {

            var checked = this.checked,
                row = $(this).closest("tr"),
                grid = $("#batchGrid_" + $index).data("kendoGrid"),
                dataItem = grid.dataItem(row);

            //if (checked) {
            //    row.addClass("k-state-selected");
            //    $(this).attr('checked', true);
            //    checkedIds[dataItem.TRANSACTION_NO] = checked;
            //    if (checkedItems.length > 0) {
            //        for (var c = 0; c < checkedItems.length; c++) {
            //            if (checkedItems[c].TRACKING_SERIAL_NO == dataItem.TRACKING_SERIAL_NO) {
            //                return;
            //            }
            //            else {

            //            }
            //        }
            //    }
            //    else {
            //        checkedItems.push({
            //            //"TRACKING_SERIAL_NO": dataItem.TRACKING_SERIAL_NO,
            //            "ITEM_CODE": dataItem.ITEM_CODE,
            //            "MU_CODE": dataItem.MU_CODE,
            //            "LOCATION_CODE": dataItem.LOCATION_CODE,
            //            "SERIAL_NO": dataItem.SERIAL_NO,
            //            "TRACKING_SERIAL_NO": dataItem.TRACKING_SERIAL_NO
            //        });
            //    }


            //}
            if (checked) {
                if (row.hasClass("k-state-selected")) {
                    return;

                }
                else {
                    row.addClass("k-state-selected");
                    $(this).attr('checked', true);
                    checkedIds[dataItem.TRANSACTION_NO] = checked;
                    checkedItems.push({
                        //"TRACKING_SERIAL_NO": dataItem.TRACKING_SERIAL_NO,
                        "ITEM_CODE": dataItem.ITEM_CODE,
                        "MU_CODE": dataItem.MU_CODE,
                        "LOCATION_CODE": dataItem.LOCATION_CODE,
                        "SERIAL_NO": dataItem.SERIAL_NO,
                        "TRACKING_SERIAL_NO": dataItem.TRACKING_SERIAL_NO
                    });

                }
            }

            else {
                for (var i = 0; i < checkedItems.length; i++) {
                    if (checkedItems[i].TRANSACTION_NO == dataItem.TRANSACTION_NO && checkedItems[i].TRACKING_SERIAL_NO == dataItem.TRACKING_SERIAL_NO) {
                        checkedItems.splice(i, 1);
                    }
                }
                row.removeClass("k-state-selected");
            }

            //for (var f = 0; f < checkedItems.length; f++) {
            //    $scope.checkedresult.push({
            //        //"TRACKING_SERIAL_NO": dataItem.TRACKING_SERIAL_NO,
            //        "ITEM_CODE": checkedItems[f].ITEM_CODE,
            //        "MU_CODE": checkedItems[f].MU_CODE,
            //        "LOCATION_CODE": checkedItems[f].LOCATION_CODE,
            //        "SERIAL_NO": checkedItems[f].SERIAL_NO,
            //        "TRACKING_SERIAL_NO": checkedItems[f].TRACKING_SERIAL_NO
            //    });
            //}
            $scope.childModels[$index].QUANTITY = Object.keys(checkedIds).length;
        }
        setTimeout(function () {


            $(".serialtrackFlag_" + $index).modal('toggle');
        }, 1500);
    }

    $scope.ShowBatchTran = function ($index, type) {

        if ($scope.refernceForBatch == true) {
            $scope.VoucherNo = $scope.voucherNoFrmRefernce[$index].CHALAN_NO;
        }
        else {
            $scope.VoucherNo = $scope.OrderNo;
        }
        $scope.batchDataGridOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: "/api/TemplateApi/GetbatchTranDataByItemCodeAndLocCode?itemcode=" + $scope.childModels[$index].ITEM_CODE + "&loactioncode=" + $scope.childModels[$index].FROM_LOCATION_CODE + "&refernceNo=" + $scope.VoucherNo
                },
                serverPaging: false,
                serverSorting: false
            },
            sortable: false,
            pageable: false,
            dataBound: function (e) {
                $('div').removeClass('.k-header k-grid-toolbar');
                $(".chkBatchTran").on("click", chkRowFrmBtn);
                var view = this.dataSource.data();
                for (var i = 0; i < view.length; i++) {
                    if (type == "SearchIcon") {
                        $('#' + view[i].BATCH_NO).attr('checked', false);
                        $('#Qty_' + view[i].BATCH_NO).attr('readonly', false);
                        $('#Qty_' + view[i].BATCH_NO).val("");
                    }
                }
                var grid = e.sender;
                if (grid.dataSource.total() == 0) {
                    var colCount = grid.columns.length + 1;
                    $(e.sender.wrapper)
                        .find('tbody')
                        .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, No Data Found For Given Filter. </td></tr>');
                }
            },
            columns: [
                {
                    template: function (dataItem) {
                        if ($scope.refernceForBatch == true) {
                            return "<input type='checkbox' id='" + dataItem.BATCH_NO + "' class='chkBatchTran row-checkbox'>"
                        }
                        else {
                            return "<input type='checkbox' id='" + dataItem.BATCH_NO + "' class='chkBatchTran row-checkbox' checked>"
                        }
                    },
                    width: 20
                },
                {
                    field: "BATCH_NO",
                    title: "Batch NO",
                    width: "40px"
                },
                {
                    field: "EXPIRY_DATE",
                    title: "Expiry Date",
                    template: "#= kendo.toString(kendo.parseDate(EXPIRY_DATE, 'yyyy-MM-dd'), 'dd/MM/yyyy') #",
                    width: "40px"
                },
                {
                    field: "QUANTITY",
                    title: "Quantity",
                    width: "40px"
                },
                {
                    title: "Quantity To Out",
                    template: function (dataItem) {
                        if ($scope.refernceForBatch == true) {
                            return "<input type='number' id='Qty_" + dataItem.BATCH_NO + "' class='row-checkbox form-control' style='height: auto;width: auto;'>"
                        }
                        else {
                            return "<input type='number' id='Qty_" + dataItem.BATCH_NO + "' value = '" + dataItem.QUANTITY + "' readonly class='row-checkbox form-control' style='height: auto;width: auto;'>"
                        }
                    },
                    width: "40px"
                }
            ]
        };
        function chkRowFrmBtn() {
            var checked = this.checked,
                row = $(this).closest("tr"),
                grid = $("#batchTranGrid_" + $index).data("kendoGrid"),
                dataItem = grid.dataItem(row);
            if (checked) {
                if (!$scope.checkedBatchTranResult.find(obj => obj["BATCH_NO"] === dataItem.BATCH_NO)) {
                    var outQty = parseInt($(this).closest('tr').find('#Qty_' + dataItem.BATCH_NO).val());
                    if (outQty == "" || outQty == "undefined" || isNaN(outQty)) {
                        alert('Out Quantity not defined.');
                        $(this).closest('tr').find('#' + dataItem.BATCH_NO).prop('checked', false);
                        $(this).closest('tr').find('#Qty_' + dataItem.BATCH_NO).focus();
                    }
                    else if (dataItem.QUANTITY < outQty) {
                        alert('Out Quantity greater than provided quantity.');
                        $(this).closest('tr').find('#' + dataItem.BATCH_NO).prop('checked', false);
                        $(this).closest('tr').find('#Qty_' + dataItem.BATCH_NO).focus();
                    }
                    else {
                        $scope.checkedBatchTranResult.push({
                            "ITEM_CODE": dataItem.ITEM_CODE,
                            "MU_CODE": dataItem.MU_CODE,
                            "LOCATION_CODE": dataItem.LOCATION_CODE,
                            "SERIAL_NO": dataItem.SERIAL_NO,
                            "SERIAL_TRACKING_FLAG": $scope.serial_tracking_flag,
                            "BATCH_TRACKING_FLAG": $scope.batch_tracking_flag,
                            "QUANTITY": dataItem.QUANTITY,
                            "BATCH_NO": dataItem.BATCH_NO,
                            "EXPIRY_DATE": dataItem.EXPIRY_DATE,
                            "REFERNCE_FROM_BATCH": $scope.refernceForBatch
                        });
                        $(this).closest('tr').find('#Qty_' + dataItem.BATCH_NO).attr('readonly', true);
                        $('.QUANTITY_' + $index).attr('readonly', true);
                        $scope.childModels[$index].QUANTITY = $scope.childModels[$index].QUANTITY + outQty;
                        $scope.childModels[$index].TOTAL = $scope.childModels[$index].QUANTITY * $scope.childModels[$index].UNIT_PRICE;
                    }
                }
            }
            else {
                if ($scope.checkedBatchTranResult.find(obj => obj["BATCH_NO"] === dataItem.BATCH_NO)) {
                    var data = $.grep($scope.checkedBatchTranResult, function (e) {
                        return e.BATCH_NO != dataItem.BATCH_NO;
                    });
                    if (data.length > 0) {
                        $scope.checkedBatchTranResult = [];
                        $scope.checkedBatchTranResult = data;
                    }
                    var outQty = parseInt($(this).closest('tr').find('#Qty_' + dataItem.BATCH_NO).val());
                    $(this).closest('tr').find('#Qty_' + dataItem.BATCH_NO).attr('readonly', false);
                    $(this).closest('tr').find('#Qty_' + dataItem.BATCH_NO).val("");
                    $('.QUANTITY_' + $index).attr('readonly', true);
                    $scope.childModels[$index].QUANTITY = $scope.childModels[$index].QUANTITY - outQty;
                    $scope.childModels[$index].TOTAL = $scope.childModels[$index].QUANTITY * $scope.childModels[$index].UNIT_PRICE;
                }
            }
        }
        setTimeout(function () {
            $(".batchTran_" + $index).modal('toggle');
        }, 1500);
    }
});