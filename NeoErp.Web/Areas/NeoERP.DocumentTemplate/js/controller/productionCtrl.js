

DTModule.controller('productionCtrl', function ($scope, $rootScope, $http, $routeParams, productionservice, productionfactory, $window, $filter, hotkeys) {

    const ACC_CODE = "ACC_CODE";
    const BUDGET_FLAG = "BUDGET_FLAG";
    const BUDGET_CODE = "BUDGET_CODE";
    const TRANSACTION_TYPE = "TRANSACTION_TYPE";
    const AMOUNT = "AMOUNT";
    $scope.units = [];
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
            BUDGET_VAL: "",
            QUANTITY: "",
            ACCOUNT_ALLOCATION: "",
        }]

    }];

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
            $scope.SaveDocumentFormData(param);
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

    //$scope.OrderNo = $routeParams.orderNo1 + "/" + $routeParams.orderNo2 + "/" + $routeParams.orderNo3;
    if ($routeParams.orderno != undefined) {
        $scope.OrderNo = $routeParams.orderno.split(new RegExp('_', 'i')).join('/');

    }
    else
    { $scope.OrderNo = "undefined"; }

    $scope.formcode = $routeParams.formcode;

    $scope.FormCode = $routeParams.formcode;
    document.formCode = $scope.FormCode;



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

    $scope.MasterFormElement = []; // for elements having master_child_flag='M'
    $scope.ChildFormElement = [{ element: [], additionalElements: $scope.BUDGET_CENTER }]; // initial child element

    $scope.accsummary = { 'drTotal': 0.00, 'crTotal': 0.00, 'diffAmount': 0.00 }

    $scope.aditionalChildFormElement = []; // a blank child element model while add button press.
    $scope.formDetail = ""; // all form_dtl elements retrived from service (contains master_child 'M' && 'C' ).
    $scope.formSetup = "";
    $scope.formCustomSetup = "";
    $scope.CustomFormElement = [];

    $scope.ModuleCode = "";
    //$scope.ModuleCode = '01';
    $scope.MasterFormElementValue = [];
    $scope.SalesOrderformDetail = "";
    $scope.datePattern = /^[01][0-9]-(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)-\d{4}$/
    $scope.companycode = "";
    //^[01][0-9]-(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)-\d{4}$

    $scope.BUDGET_MODAL = [];
    $scope.BUDGET_CHILD_MODAL = [];


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

    $scope.havRefrence = 'Y';

    $scope.printCompanyInfo = {
        companyName: '',
        address: '',
        form_edesc: ''
    }


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

    var formUrl = "/api/SetupApi/GetFormControlByFormCode?formcode=" + $scope.FormCode;
    $http.get(formUrl).then(function (response) {
     
        if (response.data.length != 0) {
            $scope.formControlModelData = response.data;
         
        }

    });
    // vend

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
        else if (tablename == "IP_TRANSFER_ISSUE" || tablename == "IP_GOODS_ISSUE" || tablename == "IP_GATE_PASS_ENTRY") {
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
            $scope.havRefrence = response[0].REFERENCE_FLAG;
            $scope.RefTableName = response[0].REF_TABLE_NAME;
            $scope.ref_form_code = response[0].REF_FORM_CODE;
        });
    }

    $scope.inventoryRefrenceFn = function (response, callback) {
        var rows = response.data;
        $scope.BUDGET_MODAL.push({ ACC_CODE: "", BUDGET_FLAG: "", BUDGET: [] });
        $scope.BUDGET_CHILD_MODAL.push({ BUDGET_VAL: "", AMOUNT: "", NARRATION: "" });
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
                    $('.dz-remove').css("display", "none");
                }
            }
            $scope.masterModels = {};
            var masterModel = angular.copy($scope.masterModelTemplate);
            var savedData = $scope.getObjWithKeysFromOtherObj(masterModel, response.data[0]);
            $scope.masterModels = savedData;

            //to solve problem in suppliercode binding for update purpose
            suppliercodeforupdate = $scope.masterModels.SUPPLIER_CODE;

            $scope.ChildFormElement = [];
            $scope.childModels = [];

            for (var i = 0; i < rows.length; i++) {
                setDataOnModal(rows, i);
            }
            $scope.ChildSumOperations(0);

        }

        function setDataOnModal(rows, i) {
            var tempCopy = angular.copy($scope.childModelTemplate);
            $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
            $scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));

            var rowsObj = rows[i];

            var budgetModel = angular.copy($scope.BUDGET_MODAL[0]);
            var budgetChildModel = angular.copy($scope.BUDGET_CHILD_MODAL[0]);

            $scope.dynamicModalData.push($scope.getObjWithKeysFromOtherObj(budgetModel, rowsObj));
            if ($scope.dynamicModalData[i].BUDGET == undefined) {
                $scope.dynamicModalData[i].BUDGET = [];
                $scope.dynamicModalData[i].BUDGET.push(budgetChildModel);
            }
            var config = {
                async: false
            };
            var voucherNo = PrimaryColumnForTable($scope.DocumentName);
            var response = $http.get('/api/InventoryApi/GetDataForBudgetModal?VOUCHER_NO=' + rows[i][voucherNo], config);
            response.then(function (budgetResult) {
                if (budgetResult.data != "") {
                    for (var a = 0; a < budgetResult.data.length; a++) {
                        ;
                        for (var b = 0; b < $scope.dynamicSubLedgerModalData.length; b++) {
                            if (budgetResult.data[a].SERIAL_NO == b) {
                                $scope.dynamicModalData[budgetResult.data[b].SERIAL_NO].BUDGET = [];
                                $scope.dynamicModalData[budgetResult.data[b].SERIAL_NO].BUDGET.push($scope.getObjWithKeysFromOtherObj(budgetChildModel, budgetResult.data[a]));
                            }

                        }
                    }

                }

            });
        }
        callback();
    };

    var formDetail = productionservice.getFormDetail_ByFormCode($scope.formcode, d1);
    $.when(d1).done(function (result) {
       
        $scope.formDetail = result.data;
        if ($scope.formDetail.length > 0) {
            $scope.DocumentName = $scope.formDetail[0].TABLE_NAME;
            $scope.companycode = $scope.formDetail[0].COMPANY_CODE;
            $scope.printCompanyInfo.companyName = $scope.formDetail[0].COMPANY_EDESC;
            $scope.printCompanyInfo.address = $scope.formDetail[0].ADDRESS;
            $scope.printCompanyInfo.form_edesc = $scope.formDetail[0].FORM_EDESC;
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
                }
            },
            $scope.MasterFormElement);
      
       


        //collection of child elements.
        angular.forEach(values,
            function (value, key) {
                if (value.MASTER_CHILD_FLAG == 'C' && value.DISPLAY_FLAG == 'Y') {
                    this.push(value);
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
            $scope.BUDGET_CHILD_MODAL.push({ BUDGET_VAL: "", AMOUNT: "", NARRATION: "" });

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
                        $('.dz-remove').css("display", "none");
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
                    $scope.newgenorderno = "";
                    $scope.save = "Update";
                    $scope.savecontinue = "Update & Continue";
                }
                for (var i = 0; i < rows.length; i++) {
                    setDataOnModal(rows, i);
                }
                $scope.ChildSumOperations(0);

            }
            else {
            }

            function setDataOnModal(rows, i) {
                var tempCopy = angular.copy($scope.childModelTemplate);
                $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
                $scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));

                var rowsObj = rows[i];

                var budgetModel = angular.copy($scope.BUDGET_MODAL[0]);
                var budgetChildModel = angular.copy($scope.BUDGET_CHILD_MODAL[0]);

                $scope.dynamicModalData.push($scope.getObjWithKeysFromOtherObj(budgetModel, rowsObj));
                if ($scope.dynamicModalData[i].BUDGET == undefined) {
                    $scope.dynamicModalData[i].BUDGET = [];
                    $scope.dynamicModalData[i].BUDGET.push(budgetChildModel);
                }
                var config = {
                    async: false
                };

                var voucherNo = PrimaryColumnForTable($scope.DocumentName);

                var response = $http.get('/api/InventoryApi/GetDataForBudgetModal?VOUCHER_NO=' + rows[i][voucherNo], config);
                response.then(function (budgetResult) {
                    if (budgetResult.data != "") {
                        for (var a = 0; a < budgetResult.data.length; a++) {
                            ;
                            for (var b = 0; b < $scope.dynamicSubLedgerModalData.length; b++) {
                                if (budgetResult.data[a].SERIAL_NO == b) {
                                    $scope.dynamicModalData[budgetResult.data[b].SERIAL_NO].BUDGET = [];
                                    $scope.dynamicModalData[budgetResult.data[b].SERIAL_NO].BUDGET.push($scope.getObjWithKeysFromOtherObj(budgetChildModel, budgetResult.data[a]));
                                }

                            }
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
        dt.resolve($scope.MasterFormElement);

    });

    $scope.ChildCalcSum = function (key) {

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
        if (calc_rate === undefined || calc_quantity === undefined || calc_rate === 0 || calc_rate === "" || calc_quantity === 0 || calc_quantity === "" || calc_rate === null || calc_quantity === null) {
            $scope.childModels[key].CALC_TOTAL_PRICE = 0;
        }
        else {
            var total_price = calc_rate * calc_quantity;
            $scope.childModels[key].CALC_TOTAL_PRICE = parseFloat(total_price.toFixed(2));
        }

        var sum = 0;
        $.each($scope.childModels, function (key, value) {
            var calc_total_amount = value.CALC_UNIT_PRICE * value.CALC_QUANTITY;
            sum += calc_total_amount;
        });
    }

    $scope.ChildSumOperations = function (keys) {

        var child_rate = $scope.childModels[keys].UNIT_PRICE;
        var quantity = $scope.childModels[keys].QUANTITY;
        if (child_rate != null && child_rate != "" && child_rate !== undefined) {
            $scope.childModels[keys].UNIT_PRICE = parseFloat(child_rate.toFixed(2));
        }

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

        if (quantity !== undefined || quantity !== null || quantity !== "") {
            $scope.childModels[keys].CALC_QUANTITY = quantity;
        }
        if (child_rate !== undefined || child_rate !== null || child_rate !== "") {
            $scope.childModels[keys].CALC_UNIT_PRICE = child_rate;
        }

        var sum = 0;
        if ($scope.childModels[keys].hasOwnProperty("TOTAL_PRICE")) {
            $.each($scope.childModels, function (key, value) {
                $scope.dynamicModalData[key].BUDGET[0].QUANTITY = value.QUANTITY;
                value.TOTAL_PRICE = parseFloat((value.QUANTITY * value.UNIT_PRICE).toFixed(2));
                value.CALC_TOTAL_PRICE = parseFloat((value.QUANTITY * value.UNIT_PRICE).toFixed(2));
                var total_price = value.TOTAL_PRICE;
                if (isNaN(total_price) || total_price === undefined && total_price === null) {
                    $scope.summary.grandTotal = parseFloat(sum.toFixed(2));
                    $scope.setTotal();

                }
                else {
                    sum += total_price;
                    $scope.summary.grandTotal = parseFloat(sum.toFixed(2));
                    $scope.setTotal();
                }

            });
        }
        else {
            $scope.summary.grandTotal = 0;
            $scope.setTotal();
        }
    }


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
                var primarycolumn = PrimaryColumnForTable($scope.DocumentName);
                //$scope.masterModels[primarycolumn] = res.data;
                $.each($scope.MasterFormElement, function (key, value) {
                    if (value['COLUMN_NAME'].indexOf('DATE') > -1) {
                        $scope.masterModels[value.COLUMN_NAME] = $filter('date')(new Date(), 'dd-MMM-yyyy');
                        if (value['COLUMN_HEADER'].indexOf('Miti') > -1) {
                            if (value.DEFA_VALUE == null) {
                                ;
                                var englishdate = $filter('date')(new Date(), 'yyyy-MM-dd');
                                var nepalidate = AD2BS(englishdate);
                                $("#nepaliDate5").val(nepalidate);
                            }
                            else {
                                ;
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
            $scope.units.push({ mu_name: item.MU_CODE, mu_code_value: parseFloat(qtySum.toFixed(4)) });
            if (item.QUANTITY !== undefined) {
                totalQty += item.QUANTITY;
                $scope.totalQty = parseFloat(totalQty.toFixed(4));
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

        }
        //$scope.muwiseQty();
    }

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
        $scope.dynamicModalData.push({
            LOCATION_CODE: "",
            BUDGET_FLAG: "",
            BUDGET: [{
                BUDGET_VAL: "",
                QUANTITY: "",
                ACCOUNT_ALLOCATION: ""
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

    $scope.add_childbudgetflag_element = function (e) {
        var totalQtyOFBudgetFlag = 0;
        $.each($scope.dynamicModalData[index].BUDGET, function (it, val) {
            totalQtyOFBudgetFlag += val.QUANTITY;
        })
        if (totalQtyOFBudgetFlag < $scope.childModels[index].CALC_QUANTITY) {
            $scope.dynamicModalData[index].BUDGET.push({
                BUDGET_VAL: "",
                QUANTITY: $scope.childModels[index].CALC_QUANTITY - totalQtyOFBudgetFlag,
                ACCOUNT_ALLOCATION: ""
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


    // remove child row.
    $scope.remove_child_element = function (index) {

        if ($scope.ChildFormElement.length > 1) {
            $scope.ChildFormElement.splice(index, 1);
            $scope.childModels.splice(index, 1); // }
            $scope.dynamicModalData.splice(index, 1);
            $scope.dynamicSubLedgerModalData.splice(index, 1);
            var sum = 0;
            $.each($scope.childModels, function (key, value) {
                value.TOTAL_PRICE = parseFloat((value.QUANTITY * value.UNIT_PRICE).toFixed(2));
                sum += value.TOTAL_PRICE;
                $scope.summary.grandTotal = parseFloat(sum.toFixed(2));
                $scope.setTotal();

            });
        }
    }

    var formSetup = productionservice.getFormSetup_ByFormCode($scope.formcode, d2);

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
    //            ACCOUNT_ALLOCATION: ""
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
    //                ACCOUNT_ALLOCATION: ""
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
                        BUDGET_VAL: "",
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
        if (event.keyCode == 13) {
            accCode = $scope.childModels[$index][ACC_CODE];
            $scope.dynamicModalData[$index].BUDGET_FLAG = $scope.childModels[$index].TO_BUDGET_FLAG == undefined ? "" : $scope.childModels[$index].TO_BUDGET_FLAG;
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
            $scope.childModels[$index].BUDGET_FLAG = $scope.childModels[$index].BUDGET_FLAG == 'L' ? 'L' : 'E';
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
        var formDetail = productionservice.getDraftFormDetail_ByFormCode(templateCode, d7);
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
        debugger;
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
        debugger;
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
            //newWin.document.write('<html><head><link rel="stylesheet" type="text/css" media="print" href="~/Areas/NeoERP.DocumentTemplate/Content/Site.css" /></head><body onload="window.print()">' + divToPrint.innerHTML + '</body></html>');
            ////newWin.document.write('<html><body onload="window.print()">' + divToPrint.innerHTML + '</body></html>');
            //newWin.document.close();
            //setTimeout(function () { newWin.close(); }, 10);
        }
        $scope.someFn();
        $scope.dynamicModalData = [{
            LOCATION_CODE: "",
            BUDGET_FLAG: "",
            BUDGET: [{
                BUDGET_VAL: "",
                QUANTITY: "",
                ACCOUNT_ALLOCATION: "",
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

     
        //check master model validation

        $scope.loadingBtn();
        //check master customer validation
        if ($scope.masterModels.hasOwnProperty("CUSTOMER_CODE")) {
            var master_customer_code = $scope.masterModels.CUSTOMER_CODE;
            if ($(".mcustomer").hasClass("borderRed") || master_customer_code == null || master_customer_code == "" || master_customer_code == undefined) {
                displayPopupNotification("Customer Code is required", "warning");
                return $scope.loadingBtnReset;
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
                displayPopupNotification("Division Code is required", "warning");
                return $scope.loadingBtnReset();
            }
        };

        //check master employee code / marketing person validation
        if ($scope.masterModels.hasOwnProperty("EMPLOYEE_CODE")) {
            var master_employee_code = $scope.masterModels.EMPLOYEE_CODE;
            if ($(".memployee").hasClass("borderRed") || master_employee_code == null || master_employee_code == "" || master_employee_code == undefined) {
                displayPopupNotification("Employee Code is required", "warning");
                return $scope.loadingBtnReset();
            }
        };

        //check master sales type validation
        if ($scope.masterModels.hasOwnProperty("SALES_TYPE_CODE")) {
            var master_salestype = $scope.masterModels.SALES_TYPE_CODE;
            if ($(".msalestype").hasClass("borderRed") || master_salestype == null || master_salestype == "" || master_salestype == undefined) {
                displayPopupNotification("Sales Type Code is required", "warning");
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
                if (child_rate == null || child_rate == "") {
                    displayPopupNotification("Rate/Unit is required", "warning");
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
                if (calc_rate == null || calc_rate == "") {
                    displayPopupNotification("Calcutaled Rate/Unit is required", "warning");
                    return $scope.loadingBtnReset();
                }
                if (calc_rate === undefined) {
                    displayPopupNotification("Enter Calcutaled Amount Correctly", "error");
                    return $scope.loadingBtnReset();
                }
            };

        };

        //showloader();
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
            Master_COLUMN_VALUE: JSON.stringify($scope.masterModels),
            Child_COLUMNS: childcolumnkeys,
            Child_COLUMN_VALUE: JSON.stringify($scope.childModels),
            Grand_Total: grandtotal,
            Custom_COLUMN_VALUE: JSON.stringify($scope.customModels),
            Order_No: orderno,
            //BUDGET_TRANS_VALUE: JSON.stringify($scope.dynamicModalData),
            BUDGET_TRANS_VALUE: angular.toJson($scope.dynamicModalData),
            //SUB_LEDGER_VALUE: JSON.stringify($scope.dynamicSubLedgerModalData),
            //SUB_LEDGER_VALUE: angular.toJson($scope.dynamicSubLedgerModalData),
            //DR_TOTAL_VALUE: angular.toJson($scope.accsummary.drTotal),
            //CR_TOTAL_VALUE: angular.toJson($scope.accsummary.crTotal),

        };
       
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

                hideloader();
                $scope.dzvouchernumber = data.data.VoucherNo;
                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;
                myInventoryDropzone.processQueue();
                var host = $window.location.host;
                var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Template/SplitterIndex#!DT/MenuSplitter/02";
                setTimeout(function () {
                    $window.location.href = landingUrl;
                }, 1000);
            }
            else if (data.data.MESSAGE == "SAVENPRINT") {
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
                                case "TO_LOCATION_CODE":
                                    $scope.childModels[ind]["TO_LOCATION_CODE"] = $($(".ctolocation_" + ind)[$(".ctolocation_" + ind).length - 1]).data("kendoComboBox").dataItem().LocationName;
                                    break;
                                case "FROM_LOCATION_CODE":
                                    $scope.childModels[ind]["FROM_LOCATION_CODE"] = $($(".ctolocation_" + ind)[$(".ctolocation_" + ind).length - 1]).data("kendoComboBox").dataItem().LocationName;
                                    break;
                                default:
                            }
                            //if (e['COLUMN_NAME'] == "ITEM_CODE") {
                            //    $scope.childModels[ind]["ITEM_CODE"] = $($(".cproduct_" + ind)[$(".cproduct_" + ind).length - 1]).data("kendoComboBox").dataItem().ItemDescription;
                            //}
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
                displayPopupNotification("Data succesfully Saved.", "success");
                $scope.TodayDate = AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD'))
                $("#printTemplateModal").toggle('modal');
            }
            else if (data.data.MESSAGE == "SAVEANDCONTINUE") {
                displayPopupNotification("Data succesfully Saved.", "success");
                $scope.someFn();
                $scope.dynamicModalData = [{
                    LOCATION_CODE: "",
                    BUDGET_FLAG: "",
                    BUDGET: [{
                        BUDGET_VAL: "",
                        QUANTITY: "",
                        ACCOUNT_ALLOCATION: "",
                    }]

                }];
                $scope.summary.grandTotal = 0;
                $scope.summary = { 'grandTotal': 0 };
                $scope.units = [];
                $scope.totalQty = 0;
                $scope.dzvouchernumber = data.data.VoucherNo;
                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;
                myInventoryDropzone.processQueue();

            }
            else if (data.data.MESSAGE == "UPDATED") {
                DisplayBarNotificationMessage("Data succesfully updated.", "success");
                $scope.dzvouchernumber = data.data.VoucherNo;
                $scope.dzvoucherdate = moment(data.data.VoucherDate).format('DD-MMM-YYYY');
                $scope.dzformcode = data.data.FormCode;
                myInventoryDropzone.processQueue();
                var landingUrl = window.location.protocol + "//" + window.location.host + "/DocumentTemplate/Template/SplitterIndex#!DT/MenuSplitter/02";
                //$window.location.href = landingUrl;
                setTimeout(function () {
                    $window.location.href = landingUrl;
                }, 1000);
            }
            else {
                hideloader();
                displayPopupNotification(response.data.MESSAGE, "error");
            }
            $scope.loadingBtnReset();
        },
            function errorCallback(response) {
                hideloader();
                displayPopupNotification(response.data.MESSAGE, "error");
                $scope.loadingBtnReset();
            });

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
            BUDGET_VAL: "",
            AMOUNT: "",
            NARRATION: ""
        });
        $scope.dynamicModalData = [{
            LOCATION_CODE: "",
            BUDGET_FLAG: "",
            BUDGET: [{
                BUDGET_VAL: "",
                QUANTITY: "",
                ACCOUNT_ALLOCATION: "",
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
    };

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

            angular.forEach(customvalues, function (value, key) {

                this.push(value);
                if ($scope.OrderNo == "undefined")
                    $scope.customModels[value['FIELD_NAME']] = value['DEFA_FIELD_VALUE'];
                else
                    $scope.customModels[value['FIELD_NAME']] = value['FIELD_VALUE'];
            }, $scope.CustomFormElement);
        }
    })

    $scope.masterModelDataFn = function (fn) {
        var saleOrderformDetail = productionservice.getSalesOrderDetail_ByFormCodeAndOrderNo($scope.formcode, $scope.OrderNo, d4);
        $.when(d4).done(function (result) {
            fn(result);
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


    $scope.someDateFn = function () {

        var engdate = $filter('date')(new Date(), 'dd-MMM-yyyy');
        var a = ConvertEngDateToNep(engdate);
        $scope.NepaliDate = a;
        $("#nepaliDate5").val(a);

    };

    $scope.ReferenceList = [];
    var req = "/api/InventoryApi/GetReferenceList?formcode=" + $scope.formcode;
    $http.get(req).then(function (response) {
        $scope.ReferenceList = response.data;
    });

    $scope.ShowInventoryRefrence = function () {
        if ($scope.havRefrence == 'Y') {
            $('#RefrenceModel').modal('show');

        }
    }

    function getVoucherDetailForReferenceProduct() {
        alert("s");
     
        var model = {
            checkList: 'test',
            FormCode: '415',
            TableName:'sales_order',
            ROW: 'rr',
            //if include charge is set true if also multiple voucher no is selected, single voucher no's transaction with its charge is shown.
            INCLUDE_CHARGE: "True"
            //ModuleCode: $scope.ModuleCode,
            //voucherNo: voucherNo,
            //serialNo: serialNo,
            //formCode: $scope.FormCode,
            //tableName: $("#refrenceTypeMultiSelect").val().toString()
        }
        var url = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetVoucherDetailForReferenceEdit";

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
                else if (modulecode == "02") {

                    $scope.inventoryRefrenceFn(response, function () {
                        hideloader();
                        $(".btn-action a").css('display', 'none')
                    });


                }
            }

        })

        //$http.post(url).then(function (result) {
        //    
        //    response = result;
        //    $scope.refrenceFn(response);
        //});
    };
});


