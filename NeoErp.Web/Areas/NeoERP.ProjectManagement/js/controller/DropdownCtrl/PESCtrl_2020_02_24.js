DTModule.controller('PESCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter, formtemplateservice) {
    $scope.dynamicExtraItemChargeModalData = [{       
    }];
    $scope.dynamicExtraCharge = [{       
    }];
    $scope.itemCode = "";
    $scope.quantity = 0;
    $scope.STATICPIModel = {
        ITEM_EDESC: "",
        MU_CODE: "",
        QUANTITY: 0,
        UNIT_PRICE: 0,
        TOTAL_PRICE: 0,
        CALC_UNIT_PRICE: 0,
        CALC_QUANTITY: 0,
        CALC_TOTAL_PRICE: 0,
    };
    $scope.PIChargeList = [];
    $scope.PIChargetxt = [];
    $scope.PICharge = [];
    $scope.PIchildModelTemplate = {};
    $scope.invoiceNo = "";
    $scope.doall = function () {
        $scope.PIChargeListdetails = [];
        $scope.PIChargetxt = [];
        $scope.PICharge = [];
        $scope.PIchildModelTemplate = {};
        var invoiceNo = $("#dropdownInv").val();

        var chargeResponse = window.location.protocol + "//" + window.location.host + '/api/ContraVoucherApi/GetVoucherNoFrmCharge?invoiceNo=' + invoiceNo;  //data bind only acount desc       
        $http.get(chargeResponse).then(function (result) {
            debugger;
            if (result.data.length > 0) {
                $scope.invoiceNo = invoiceNo;
                $scope.DatabindForEdit();
            }
            else {
                var invoiceInfoUrl = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/InvoiceDetails?invoiceNo=" + invoiceNo;
                var documentListUrlData = new kendo.data.DataSource({
                    transport: {
                        read: {
                            url: invoiceInfoUrl,
                            dataType: "json"
                        }
                    }
                    //serverFiltering: true
                });
                documentListUrlData.fetch(function () {
                    var data = documentListUrlData.data();
                    $("#lblSupplierName").text(data[0].SUPPLIER_EDESC);
                    $("#lblInvoiceDate").text(data[0].INVOICE_DATE.replace('T00:00:00', ''));
                    $("#lblCurrency").text(data[0].CURRENCY_CODE);
                    $("#lblExchangeRate").text(data[0].EXCHANGE_RATE);
                    $scope.childPIModels = [];
                    var req = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/InvoiceDetailsForGrid?invoiceNo=" + invoiceNo;
                    $http.get(req).then(function (results) {
                        var rows = results.data;
                        for (var i = 0; i < rows.length; i++) {
                            $scope.dynamicExtraItemChargeModalData = results.data;
                            var tempCopy = angular.copy($scope.STATICPIModel);
                            $scope.childPIModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));
                        }
                        $scope.calculateTotalamt();
                        var re = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/GetIpChargesEdesc?formCode=" + $("#documentTypeMultiSelect").val();
                        $http.get(re).then(function (resu) {

                            $scope.PIChargeListTxt = resu.data;
                        });
                        var reu = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/GetIpChargeCode?formCode=" + $("#documentTypeMultiSelect").val();
                        $http.get(reu).then(function (resue) {

                            $scope.PIChargeList = resue.data;
                        });
                        var reqs = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/GetIpChargeCodedetls?formCode=" + $("#documentTypeMultiSelect").val();
                        $http.get(reqs).then(function (resultss) {

                            $scope.PIChargeListdetails = resultss.data;
                            var count = 0;
                            angular.forEach($scope.dynamicExtraItemChargeModalData, function (va, ke) {

                                angular.forEach($scope.PIChargeListdetails, function (value, key) {

                                    var serial_no = $rootScope.childRowIndexacc + 1;
                                    $scope.PIchildModelTemplate[value.CHARGE_CODE] = 0;
                                    $("#RowWise_TotalCharge_" + ke).text("0");
                                    $("#RowWise_TotalLandedCost_" + ke).text("0");
                                    $scope.dynamicExtraCharge[count] = {
                                        REFERENCE_NO: $("#dropdownInv").val(),
                                        CHARGE_CODE: 0,
                                        SERIAL_NO: serial_no,
                                        CHARGE_TYPE_FLAG: value.CHARGE_TYPE_FLAG,
                                        APPLY_ON: value.APPLY_ON,
                                        GL_FLAG: value.GL_FLAG,
                                        APPORTION_ON: value.APPORTION_ON,
                                        IMPACT_ON: value.IMPACT_ON,
                                        ITEM_CODE: va.ITEM_CODE,
                                        QUANTITY: va.QUANTITY,
                                        CALC_UNIT_PRICE: 0,
                                        CALC_TOTAL_PRICE: 0,
                                        CURRENCY_CODE: "",
                                        EXCHANGE_RATE: 0,
                                        CHARGE_AMOUNT: 0,
                                        FORM_CODE: "",
                                        ACC_CODE: $rootScope.ACC_CODE
                                    };
                                    count++;
                                })
                            });
                            $scope.fillPICharge();
                        });
                        $scope.PIChargetxt.push($scope.PIChargeList);
                        $scope.previousCalTotCost = $scope.PiCalcGrandtotal;
                        $scope.TotalCharge = 0;
                        $scope.TotalLandedCost = 0;
                    });
                });
            }
        });
        //var invoiceInfoUrl = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/InvoiceDetails?invoiceNo=" + invoiceNo;
        //var documentListUrlData = new kendo.data.DataSource({
        //    transport: {
        //        read: {
        //            url: invoiceInfoUrl,
        //            dataType: "json"
        //        }
        //    }
        //    //serverFiltering: true
        //});
        //documentListUrlData.fetch(function () {            
        //    var data = documentListUrlData.data();
        //    $("#lblSupplierName").text(data[0].SUPPLIER_EDESC);
        //    $("#lblInvoiceDate").text(data[0].INVOICE_DATE.replace('T00:00:00', ''));
        //    $("#lblCurrency").text(data[0].CURRENCY_CODE);
        //    $("#lblExchangeRate").text(data[0].EXCHANGE_RATE);
        //    $scope.childPIModels = [];
        //    var req = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/InvoiceDetailsForGrid?invoiceNo=" + invoiceNo;
        //    $http.get(req).then(function (results) {
        //        var rows = results.data;               
        //        for (var i = 0; i < rows.length; i++) {
        //            $scope.dynamicExtraItemChargeModalData = results.data;
        //            var tempCopy = angular.copy($scope.STATICPIModel);
        //            $scope.childPIModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));
        //        }
        //        $scope.calculateTotalamt();
        //        var re = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/GetIpChargesEdesc?formCode=" + $("#documentTypeMultiSelect").val();
        //        $http.get(re).then(function (resu) {
               
        //            $scope.PIChargeListTxt = resu.data;
        //        });
        //        var reu = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/GetIpChargeCode?formCode=" + $("#documentTypeMultiSelect").val();
        //        $http.get(reu).then(function (resue) {
                  
        //            $scope.PIChargeList = resue.data;
        //        });
        //        var reqs = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/GetIpChargeCodedetls?formCode=" + $("#documentTypeMultiSelect").val();
        //        $http.get(reqs).then(function (resultss) {
                 
        //            $scope.PIChargeListdetails = resultss.data;
        //            var count = 0;
        //            angular.forEach($scope.dynamicExtraItemChargeModalData, function (va, ke) {
                     
        //                angular.forEach($scope.PIChargeListdetails, function (value, key) {
                  
        //                    var serial_no = $rootScope.childRowIndexacc + 1;
        //                $scope.PIchildModelTemplate[value.CHARGE_CODE] = 0;                       
        //                    $("#RowWise_TotalCharge_" + ke).text("0");
        //                    $("#RowWise_TotalLandedCost_" + ke).text("0");
        //                    $scope.dynamicExtraCharge[count] = {
        //                        REFERENCE_NO: $("#dropdownInv").val(),
        //                        CHARGE_CODE: 0,
        //                        SERIAL_NO: serial_no,
        //                        CHARGE_TYPE_FLAG: value.CHARGE_TYPE_FLAG,
        //                        APPLY_ON: value.APPLY_ON,
        //                        GL_FLAG: value.GL_FLAG,
        //                        APPORTION_ON: value.APPORTION_ON,
        //                        IMPACT_ON: value.IMPACT_ON,
        //                        ITEM_CODE: va.ITEM_CODE,
        //                        QUANTITY: va.QUANTITY,
        //                        CALC_UNIT_PRICE: 0,
        //                        CALC_TOTAL_PRICE: 0,
        //                        CURRENCY_CODE: "",
        //                        EXCHANGE_RATE: 0,
        //                        CHARGE_AMOUNT: 0,
        //                        FORM_CODE: "",
        //                        ACC_CODE: $rootScope.ACC_CODE
        //                    };
        //                    count++;
        //                })
        //            });
        //            $scope.fillPICharge();
        //        });
        //        $scope.PIChargetxt.push($scope.PIChargeList);
        //        $scope.previousCalTotCost = $scope.PiCalcGrandtotal;
        //        $scope.TotalCharge = 0;
        //        $scope.TotalLandedCost = 0;
        //    });
        //});
    };
    $scope.getObjWithKeysFromOtherObj = function (objKeys, objKeyswithData) {
        var keys = Object.keys(objKeys);
        var result = {};
        for (var i = 0; i < keys.length; i++) {
            result[keys[i]] = objKeyswithData[keys[i]];
        }
        return result;
    };
    $scope.calculateTotalamt = function () {
        var gtp = 0;
        $scope.PiGrandtotal = 0;
        var gctp = 0;
        $scope.PiCalcGrandtotal = 0;
        angular.forEach($scope.childPIModels, function (value, key) {
            if (typeof value.TOTAL_PRICE !== 'undefined' && value.TOTAL_PRICE !== null && value.TOTAL_PRICE !== "") {
                //console.log('value', value);
                gtp = parseFloat(gtp) + (parseFloat(value.TOTAL_PRICE));
            }
            else { return; }
            if (typeof value.CALC_TOTAL_PRICE !== 'undefined' && value.CALC_TOTAL_PRICE !== null && value.CALC_TOTAL_PRICE !== "") {
                //console.log('value', value);
                gctp = parseFloat(gctp) + (parseFloat(value.CALC_TOTAL_PRICE));
            }
            else { return; }
        })
        $scope.PiGrandtotal = gtp;
        $scope.PiCalcGrandtotal = gctp.toFixed(2);
    };
    $scope.ChargeCount = 0;
    $scope.divideamount = function (keys, items) {
        var operation = "";
        var a = $scope.PIChargetxt[keys][items];
        var exchangeRate = parseInt($('#lblExchangeRate').text());
        var totalAmount = $scope.PiGrandtotal;
        angular.forEach($scope.PIChargeListTxt, function (value, key) {           
            if (value.CHARGE_CODE == items) {
                if (value.CHARGE_TYPE_FLAG == "A") {
                    operation = "A";                   
                    return false;
                }
                if (value.CHARGE_TYPE_FLAG == "D") {
                    operation = "D";
                    return false;
                }
            }
        });
        var CalTotAmtWithCharge = 0;
        var TotalCharge = 0;
        angular.forEach($scope.PICharge, function (value, key) {
            var e = Object.keys(value);
            $.each(e, function (i, val) {
                if (val == items) {
                    if (a == null || a == "undefined" || a == "") {
                        a = 0;
                    }
                    var rowAmount = $scope.childPIModels[key].TOTAL_PRICE;
                    var previousCalRate = parseFloat($scope.childPIModels[key].CALC_UNIT_PRICE);
                    var previousCharge = $scope.PICharge[key][items];
                    var presentCharge = ((a / exchangeRate) / totalAmount) * rowAmount;
                    if (previousCharge == null || previousCharge == "undefined" || previousCharge == "") {
                        previousCharge = 0;
                    }
                    $scope.PICharge[key][items] = presentCharge;
                    if (operation == "A") {
                        $scope.childPIModels[key].CALC_UNIT_PRICE = (previousCalRate - previousCharge + presentCharge).toFixed(4);
                    }
                    if (operation == "D") {
                        $scope.childPIModels[key].CALC_UNIT_PRICE = (previousCalRate + previousCharge - presentCharge).toFixed(4);
                    }
                    $scope.childPIModels[key].CALC_TOTAL_PRICE = ($scope.childPIModels[key].CALC_UNIT_PRICE * $scope.childPIModels[key].CALC_QUANTITY).toFixed(4);
                    CalTotAmtWithCharge += parseFloat($scope.childPIModels[key].CALC_TOTAL_PRICE);
                    $("#RowWise_TotalLandedCost_" + key).text($scope.childPIModels[key].CALC_TOTAL_PRICE);
                    var previousRowwiseTotalCharge = parseFloat($("#RowWise_TotalCharge_" + key).text());
                    if (rowwiseTotalCharge == null || rowwiseTotalCharge == "undefined" || rowwiseTotalCharge == NaN || rowwiseTotalCharge == "") {
                        rowwiseTotalCharge = 0;
                    }
                    var rowwiseTotalCharge = previousRowwiseTotalCharge - previousCharge + presentCharge;
                    TotalCharge += rowwiseTotalCharge;
                    $("#RowWise_TotalCharge_" + key).text(rowwiseTotalCharge);
                }
            });
        });
        $scope.PiCalcGrandtotal = CalTotAmtWithCharge.toFixed(2);
        $scope.TotalCharge = TotalCharge.toFixed(2);
        $scope.TotalLandedCost = CalTotAmtWithCharge.toFixed(2);       
    };
    $scope.fillPICharge = function () {
        for (var k = 0; k < $scope.childPIModels.length; k++) {
            var tempCopy = angular.copy($scope.PIchildModelTemplate);
            $scope.PICharge.push(tempCopy);
        }
    };
    $scope.intialCharge = 0;
    $scope.indChargeCal = function (key, item) {
        var operation = "";
        angular.forEach($scope.PIChargeListTxt, function (value, key) {
            if (value.CHARGE_CODE == item) {
                if (value.CHARGE_TYPE_FLAG == "A") {
                    operation = "A";
                    return false;
                }
                if (value.CHARGE_TYPE_FLAG == "D") {
                    operation = "D";
                    return false;
                }
            }
        });
        var exchangeRate = parseInt($('#lblExchangeRate').text());
        var a = $scope.PICharge[key][item];
        var CalTotAmtWithCharge = 0;
        var TotalCharge = 0;
        var CalTotAmtWithCharge = 0;
        if (a == null || a == "undefined" || a == "") {
            a = 0;
        }
        var previousCalRate = parseFloat($scope.childPIModels[key].CALC_UNIT_PRICE);
        var presentCharge = a / exchangeRate;
        if (a > 0) {
            $scope.PICharge[key][item] = presentCharge;
        }
        var previousRowwiseTotalCharge = parseFloat($("#RowWise_TotalCharge_" + key).text());
        if (previousRowwiseTotalCharge == null || previousRowwiseTotalCharge == "undefined" || previousRowwiseTotalCharge == NaN || previousRowwiseTotalCharge == "") {
            previousRowwiseTotalCharge = 0;
        }
        var previousCharge = parseFloat($scope.intialCharge);
        $scope.intialCharge = presentCharge;
       
        if (operation == "A") {
            $scope.childPIModels[key].CALC_UNIT_PRICE = previousCalRate - previousCharge + presentCharge;
        }
        if (operation == "D") {
            $scope.childPIModels[key].CALC_UNIT_PRICE = previousCalRate + previousCharge - presentCharge;
        }
        $scope.childPIModels[key].CALC_TOTAL_PRICE = (parseFloat($scope.childPIModels[key].CALC_UNIT_PRICE) * parseFloat($scope.childPIModels[key].CALC_QUANTITY)).toFixed(4);

        var rowwiseTotalCharge = previousRowwiseTotalCharge - previousCharge + $scope.PICharge[key][item];
        $("#RowWise_TotalCharge_" + key).text(rowwiseTotalCharge);
        angular.forEach($scope.childPIModels, function (value, key) {
            $("#RowWise_TotalLandedCost_" + key).text($scope.childPIModels[key].CALC_TOTAL_PRICE);
            TotalCharge += parseFloat($("#RowWise_TotalCharge_" + key).text());
            CalTotAmtWithCharge += parseFloat($scope.childPIModels[key].CALC_TOTAL_PRICE);
        });
        $scope.PiCalcGrandtotal = parseFloat(CalTotAmtWithCharge.toFixed(2));
        $scope.TotalCharge = TotalCharge;
        $scope.TotalLandedCost = $scope.PiCalcGrandtotal;
    };
    $scope.getval = function (key,item) {
        $scope.intialCharge = $scope.PICharge[key][item];
    };
    $scope.BindDataToTemplate = function () {
     
        var ind = $rootScope.childRowIndexacc;
        //$scope.childModels[ind].AMOUNT = parseFloat($scope.PiCalcGrandtotal);
        $scope.childModels[ind].AMOUNT = parseFloat($scope.TotalCharge);
        //$scope.accsummary.drTotal = parseFloat($scope.PiCalcGrandtotal);
        $scope.accsummary.drTotal = parseFloat($scope.TotalCharge);
        $scope.accsummary.diffAmount = $scope.accsummary.drTotal.toFixed(2) - parseFloat($scope.accsummary.crTotal).toFixed(2);
        var count = 0;
        angular.forEach($scope.childPIModels, function (val1, key1) {
            angular.forEach($scope.PIChargeListTxt, function (val2, key2) {
                var serial_no = ind + 1;
                $scope.dynamicExtraCharge[count].SERIAL_NO = serial_no;
                $scope.dynamicExtraCharge[count].CALC_TOTAL_PRICE = parseFloat($scope.childPIModels[key1].CALC_TOTAL_PRICE);
                $scope.dynamicExtraCharge[count].CALC_UNIT_PRICE = parseFloat($scope.childPIModels[key1].CALC_UNIT_PRICE);
                $scope.dynamicExtraCharge[count].CURRENCY_CODE = $("#lblCurrency").text();
                $scope.dynamicExtraCharge[count].EXCHANGE_RATE = parseFloat($("#lblExchangeRate").text());
                $scope.dynamicExtraCharge[count].CHARGE_AMOUNT = parseFloat($scope.PICharge[key1][val2.CHARGE_CODE]);
                $scope.dynamicExtraCharge[count].CHARGE_CODE = val2.CHARGE_CODE;
                $scope.dynamicExtraCharge[count].CHARGE_TYPE_FLAG = val2.CHARGE_TYPE_FLAG;
                $scope.dynamicExtraCharge[count].FORM_CODE = $("#documentTypeMultiSelect").val();
                count++;
            });
        });
        $rootScope.rootdynamicExtraItemChargeModalData = $scope.dynamicExtraCharge;
       
        //var dtfield = $("#documentTypeMultiSelect").data("kendoDropDownList");
        //var difield = $("#dropdownInv").data("kendoDropDownList");

        //$scope.refpurexp[0].FORM_EDESC = dtfield.text();
        //$scope.refpurexp[0].REFERENCE_NO = difield.text();
        //$scope.refrencePES($scope.refpurexp, function () {
        //});
        $(".AMOUNT_" + ind).attr("readonly", true);
        $("#PurchaseExpSheet").modal('toggle');

        //setTimeout(function () {
     
        //    var dtfield = $("#documentTypeMultiSelect").data("kendoDropDownList");
        //    var difield = $("#dropdownInv").data("kendoDropDownList");

        //    $scope.refpurexp.FORM_EDESC = dtfield.text();
        //    $scope.refpurexp.REFERENCE_NO = difield.text();
        //    $scope.refPESGridOptions = {
        //        dataSource: {
        //            type: "json",
                  
        //            data: $scope.refpurexp,
        //            pageSize: 5,
        //            serverPaging: false,
        //            serverSorting: false
        //        },
        //        sortable: false,
        //        pageable: false,
        //        dataBound: function () {
        //            this.expandRow(this.tbody.find("tr.k-master-row").first());
        //        },
        //        columns: [{
        //            field: "FORM_EDESC",
        //            title: "Document No",
        //            width: "120px"
        //        }, {
        //            field: "REFERENCE_NO",
        //            title: "Voucher No",
        //            width: "120px"
        //        }
        //        ]
        //    };
        //}, 300);
    };
    $scope.BindInvoice = function () {
        var formCode = $("#documentTypeMultiSelect").val();
        var fromdate = $("#FromDateVoucher").val();
        var todate = $("#ToDateVoucher").val();
        var manualNo = this.PES.MANUALNO;
        var ppNo = this.PES.PPNO;
        var supplierCode = this.PES.SUPPLIER;
        var Url = null;
        if ((ppNo == undefined || ppNo == "") && (supplierCode == undefined || supplierCode == "")) {
            Url = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/DocumentListDropDown?tableName=" + null + "&formCode=" + formCode + "&fromdate=" + fromdate + "&todate=" + todate + "&manualNo=" + manualNo;
        }
        if ((manualNo == undefined || manualNo == "") && (supplierCode == undefined || supplierCode == "")) {
            Url = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/DocumentListDropDown?tableName=" + null + "&formCode=" + formCode + "&fromdate=" + fromdate + "&todate=" + todate + "&ppNo=" + ppNo;
        }
        if ((ppNo == undefined || ppNo == "") && (manualNo == undefined || manualNo == "")) {            
            Url = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/DocumentListDropDown?tableName=" + null + "&formCode=" + formCode + "&fromdate=" + fromdate + "&todate=" + todate + "&supplierCode=" + $('#dropdownSupplier').val();
        }
        if (Url == null) {

        }
        else {
            $scope.optionInvoiceDataSource = {
                type: "JSON",
                serverFiltering: true,
                transport: {
                    read: {
                        url: Url,
                    }
                }
            };
            $scope.optionInvoice = {
                dataSource: $scope.optionInvoiceDataSource,
                optionLabel: '-- Select Template --',
                dataTextField: "INVOICE_NO",
                dataValueField: "INVOICE_NO"
            };
        }  
    }  
    $scope.$on('DatabindForEdit', function (event, args) {
        debugger;
        if ($scope.OrderNo != undefined && $scope.OrderNo != "undefined") {
           
                $scope.childPIModels = [];
                $rootScope.childRowIndexacc = args.index;
                $('#idaccount_' + args.index).val($scope.ChildAccCode[args.index]);
        /*var chargeResponse = $http.get(window.location.protocol + "//" + window.location.host + '/api/ContraVoucherApi/GetChargeExpList?voucherNo=' + $scope.OrderNo + '&accCode=' + $scope.ChildAccCode[args.index]);*/  //data bind only acount desc

            var chargeResponse = $http.get(window.location.protocol + "//" + window.location.host + '/api/ContraVoucherApi/GetChargeDtlFrmVoucherNo?orderNo=' + $scope.OrderNo);  //data bind only acount desc

            chargeResponse.then(function (result) {
             
                var data = result.data;
                $scope.ChargeBind = result.data;
                $("#lblSupplierName").text(data[0].SUPPLIER_EDESC);
                $("#lblInvoiceDate").text(data[0].INVOICE_DATE.replace('T00:00:00', ''));
                $("#lblCurrency").text(data[0].CURRENCY_CODE);
                $("#lblExchangeRate").text(data[0].EXCHANGE_RATE);
                $("#documentTypeMultiSelect").val(result.data[0].FORM_CODE);
                var req = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/InvoiceDetailsForGrid?invoiceNo=" + result.data[0].REFERENCE_NO;
                $http.get(req).then(function (results) {   
                
                    var rows = results.data;
                    for (var i = 0; i < rows.length; i++) {
                      
                        $scope.dynamicExtraItemChargeModalData = results.data;
                        var tempCopy = angular.copy($scope.STATICPIModel);
                        $scope.childPIModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));
                        $("#RowWise_TotalCharge_" + i).text("0");
                        $("#RowWise_TotalLandedCost_" + i).text("0");
                    }
                    $scope.calculateTotalamt();
                    var re = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/GetIpChargesEdesc?formCode=" + result.data[0].FORM_CODE;
                    $http.get(re).then(function (resu) {
                        $scope.PIChargeListTxt = resu.data;
                    });
                    var reqs = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/GetIpChargeCode?formCode=" + result.data[0].FORM_CODE;

                    var totalCharge = 0;
                    $http.get(reqs).then(function (resultss) {
                     
                        $scope.PIChargeList = resultss.data;
                        var count = 0;
                        angular.forEach($scope.dynamicExtraItemChargeModalData, function (va, ke) {
                       
                            var rowwiseTotalCharge = 0;
                            angular.forEach($scope.PIChargeList, function (value, key) {
                        
                            $scope.PIchildModelTemplate[value.CHARGE_CODE] = 0;                          
                                //$("#RowWise_TotalCharge_" + ke).text("0");
                                //$("#RowWise_TotalLandedCost_" + ke).text("0");
                                $scope.dynamicExtraCharge[count] = {
                                    REFERENCE_NO: result.data[0].REFERENCE_NO,
                                    CHARGE_CODE: 0,
                                    CHARGE_TYPE_FLAG: "",                                   
                                    ITEM_CODE: va.ITEM_CODE,
                                    QUANTITY: va.QUANTITY,
                                    CALC_UNIT_PRICE: 0,
                                    CALC_TOTAL_PRICE: 0,
                                    CURRENCY_CODE: "",
                                    EXCHANGE_RATE: 0,
                                    CHARGE_AMOUNT: 0,
                                    FORM_CODE: "",
                                    ACC_CODE: $scope.ChildAccCode[args.index]
                                };
                            count++;
                                angular.forEach($scope.ChargeBind, function (val2, key) {
                                 
                                if ($scope.dynamicExtraItemChargeModalData[ke].ITEM_EDESC == val2.ITEM_EDESC) {
                                    if (value.CHARGE_CODE == val2.CHARGE_CODE) {
                                        $scope.PIchildModelTemplate[value.CHARGE_CODE] = val2.CHARGE_AMOUNT;
                                        rowwiseTotalCharge += val2.CHARGE_AMOUNT;
                                        //$scope.PIChargetxt[ke][value.CHARGE_CODE] = val2.CHARGE_AMOUNT;
                                    }
                                } 
                            });
                        });
                            $("#RowWise_TotalCharge_" + ke).text(rowwiseTotalCharge);
                            $("#RowWise_TotalLandedCost_" + ke).text(va.CALC_TOTAL_PRICE);
                            totalCharge += rowwiseTotalCharge;
                            $scope.TotalCharge = totalCharge;
                            var tempCopy = angular.copy($scope.PIchildModelTemplate);
                            $scope.PICharge.push(tempCopy);
                        });                        
                    //$scope.fillPICharge();
                    });                    
                    $scope.PIChargetxt.push($scope.PIChargeList);
                    $scope.previousCalTotCost = parseFloat($scope.PiCalcGrandtotal);
                    $scope.TotalLandedCost = parseFloat($scope.PiCalcGrandtotal);                    
                });
                $("#PurchaseExpSheet").modal('toggle');
            });
        }
        if ($scope.invoiceNo != "" && $scope.invoiceNo != undefined) {
            $scope.childPIModels = [];
            //$rootScope.childRowIndexacc = args;
            $('#idaccount_' + $rootScope.childRowIndexacc).val($rootScope.ACC_CODE);
            var chargeResponse = $http.get(window.location.protocol + "//" + window.location.host + '/api/ContraVoucherApi/GetChargeDtlFrmInvoice?invoiceNo=' + $("#dropdownInv").val());  //data bind only acount desc
            chargeResponse.then(function (result) {

                var data = result.data;
                $scope.ChargeBind = result.data;
                $("#lblSupplierName").text(data[0].SUPPLIER_EDESC);
                $("#lblInvoiceDate").text(data[0].INVOICE_DATE.replace('T00:00:00', ''));
                $("#lblCurrency").text(data[0].CURRENCY_CODE);
                $("#lblExchangeRate").text(data[0].EXCHANGE_RATE);
                $("#documentTypeMultiSelect").val(result.data[0].FORM_CODE);
                var req = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/InvoiceDetailsForGrid?invoiceNo=" + result.data[0].REFERENCE_NO;
                $http.get(req).then(function (results) {

                    var rows = results.data;
                    for (var i = 0; i < rows.length; i++) {

                        $scope.dynamicExtraItemChargeModalData = results.data;
                        var tempCopy = angular.copy($scope.STATICPIModel);
                        $scope.childPIModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));
                        $("#RowWise_TotalCharge_" + i).text("0");
                        $("#RowWise_TotalLandedCost_" + i).text("0");
                    }
                    $scope.calculateTotalamt();
                    var re = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/GetIpChargesEdesc?formCode=" + result.data[0].FORM_CODE;
                    $http.get(re).then(function (resu) {
                        $scope.PIChargeListTxt = resu.data;
                    });
                    var reqs = window.location.protocol + "//" + window.location.host + "/api/ContraVoucherApi/GetIpChargeCode?formCode=" + result.data[0].FORM_CODE;

                    var totalCharge = 0;
                    $http.get(reqs).then(function (resultss) {

                        $scope.PIChargeList = resultss.data;
                        var count = 0;
                        angular.forEach($scope.dynamicExtraItemChargeModalData, function (va, ke) {

                            var rowwiseTotalCharge = 0;
                            angular.forEach($scope.PIChargeList, function (value, key) {

                                $scope.PIchildModelTemplate[value.CHARGE_CODE] = 0;
                                //$("#RowWise_TotalCharge_" + ke).text("0");
                                //$("#RowWise_TotalLandedCost_" + ke).text("0");
                                $scope.dynamicExtraCharge[count] = {
                                    REFERENCE_NO: result.data[0].REFERENCE_NO,
                                    CHARGE_CODE: 0,
                                    CHARGE_TYPE_FLAG: "",
                                    ITEM_CODE: va.ITEM_CODE,
                                    QUANTITY: va.QUANTITY,
                                    CALC_UNIT_PRICE: 0,
                                    CALC_TOTAL_PRICE: 0,
                                    CURRENCY_CODE: "",
                                    EXCHANGE_RATE: 0,
                                    CHARGE_AMOUNT: 0,
                                    FORM_CODE: "",
                                    ACC_CODE: $rootScope.ACC_CODE
                                };
                                count++;
                                angular.forEach($scope.ChargeBind, function (val2, key) {

                                    if ($scope.dynamicExtraItemChargeModalData[ke].ITEM_EDESC == val2.ITEM_EDESC) {
                                        if (value.CHARGE_CODE == val2.CHARGE_CODE) {
                                            $scope.PIchildModelTemplate[value.CHARGE_CODE] = val2.CHARGE_AMOUNT;
                                            rowwiseTotalCharge += val2.CHARGE_AMOUNT;
                                            //$scope.PIChargetxt[ke][value.CHARGE_CODE] = val2.CHARGE_AMOUNT;
                                        }
                                    }
                                });
                            });
                            $("#RowWise_TotalCharge_" + ke).text(rowwiseTotalCharge);
                            $("#RowWise_TotalLandedCost_" + ke).text(va.CALC_TOTAL_PRICE);
                            totalCharge += rowwiseTotalCharge;
                            $scope.TotalCharge = totalCharge;
                            var tempCopy = angular.copy($scope.PIchildModelTemplate);
                            $scope.PICharge.push(tempCopy);
                        });
                        //$scope.fillPICharge();
                    });
                    $scope.PIChargetxt.push($scope.PIChargeList);
                    $scope.previousCalTotCost = parseFloat($scope.PiCalcGrandtotal);
                    $scope.TotalLandedCost = parseFloat($scope.PiCalcGrandtotal);
                });               
            });
        }
    });
}
);