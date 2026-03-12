app.controller('PurchaseOrderController', function ($scope, $compile, $http, $filter, $q, $sce, $timeout, PurchaseOrderService) {
    var d1 = $.Deferred();
    $scope.saveAction = "Save";
    $scope.scAction = "Save & Continue";
    $scope.ContentModal = true;
    $scope.itemname = [];
    $scope.itemlist = [];
    $scope.countrylist = [];
    $scope.hslist = [];
    $scope.beneficiarylist = [];
    $scope.shipmentType = [];
    $scope.shipmentLoad = [];
    $scope.hidefirstrow = true;
    $scope.creditdaysreadonly = false;
    $scope.noitems = true;
    $scope.ItemsShow = true;
    $scope.itemcodeformucode = '';
    $scope.allitemslist = [];
    $scope.allshipmentlist = [];
    $scope.issues = [];
    $scope.allMuCodelist = [];
    $scope.tablelist = [];
    $scope.hideonedit = true;
    $scope.additems = true;
    $scope.ipPurchaseOrder = [];
    $scope.itemListsTemp = null;
    $scope.phoneNumber = /^[0-9-+]+$/;
    $scope.creditdaysNumber = /^-?[0-9]+$/;
    $scope.checkitemlistvalid = 0;

    $scope.filterData = {
        purchaseOrder: "",
        beneficiaryname: "",
        orderdate: $filter('date')(new Date, "MM-dd-yyyy"),
    };


    $scope.resettotal = function () {
        $scope.Total_Quantity = 0;
        $scope.Total_Rate = 0;
        $scope.Total_Amount = 0;
    }

    $scope.monthSelectorOptions = {
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() + 100);
        },
        format: "dd-MMM-yyyy",
        // specifies that DateInput is used for masking the input element
        dateInput: true
    };


    $scope.tsradio = {
        radio: 'Y'
    };

    $scope.lc = {
        purchaseOrder: '',
        number: '',
        beneficiaryname: '',
        weeknumber:'00',
        orderdate: $filter('date')(new Date, "MM/dd/yyyy"),
        opendate: '',
        creditdays: 0,
        remarks: '',
        shipmentfrom: '',
        shipmentTo: '',
        shipmentType: '',
        shipmentload: '',
        duration: '',
        manualno: '',
        deliveryplace: '',
        appname: '',
        appaddress: '',
        billcompname: '',
        billcompadd: '',
        billcompPhone: '',
        shipcompname: '',
        shipcompadd: '',
        shipcompPhone: '',
        contactname: '',
        contactphone: '',
        contactemail: '',
        validitydate: $filter('date')(new Date, "MM/dd/yyyy"),
        estdeliverydate: $filter('date')(new Date, "MM/dd/yyyy"),
        lastshipmentdate: '',
        ppf: '',
        calenderdays: 'DAY'
    };

    ////

    $scope.disableeditcontent = function () {
        $scope.check_incoterms = false;
        $scope.check_deliveryplacetype = false;
        $scope.check_paymentterms = false;
        $scope.check_creditdays = false;
        $scope.check_manualno = false;
        $scope.check_deliveryplace = false;
        $scope.check_appname = false;
        $scope.check_appaddress = false;
        $scope.check_billcompname = false;
        $scope.check_billcompadd = false;
        $scope.check_billcompPhone = false;
        $scope.check_shipcompname = false;
        $scope.shipcompadd = false;
        $scope.check_shipcompPhone = false;
        $scope.check_contactname = false;
        $scope.check_contactemail = false;
        $scope.check_contactphone = false;
        $scope.check_validitydate = false;
        $scope.check_daynumber = false;
        $scope.check_calenderdays = false;
        $scope.check_remarks = false;
        $scope.check_beneficiaryname = false;
        $scope.check_weeknumber = false;
        $scope.check_orderdate = false;
        $scope.check_currency = false;
        //$scope.creditdaysreadonly = true;
        $scope.deliverydropdown = true;
    };


    $scope.cleareditcontent = function () {
        $scope.check_incoterms = true;
        $scope.check_deliveryplacetype = true;
        $scope.check_paymentterms = true;
        $scope.check_creditdays = true;
        $scope.check_manualno = true;
        $scope.check_deliveryplace = true;
        $scope.check_appname = true;
        $scope.check_appaddress = true;
        $scope.check_billcompname = true;
        $scope.check_billcompadd = true;
        $scope.check_billcompPhone = true;
        $scope.check_shipcompname = true;
        $scope.shipcompadd = true;
        $scope.check_shipcompPhone = true;
        $scope.check_contactname = true;
        $scope.check_contactemail = true;
        $scope.check_contactphone = true;
        $scope.check_validitydate = true;
        $scope.check_daynumber = true;
        $scope.check_calenderdays = true;
        $scope.check_remarks = true;
        $scope.check_beneficiaryname = true;
        $scope.check_weeknumber = true;
        $scope.check_orderdate = true;
        $scope.check_currency = true;
        //$scope.creditdaysreadonly = false;
        //$scope.deliverydropdown = false;
        

    };

    $scope.AllcheckboxEnableDisable = function () {
        if ($scope.Allcheckbox) {
            $scope.cleareditcontent();
        } else {
            $scope.disableeditcontent();
        }
    }
    ////

    $scope.itemsDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/PurchaseOrder/getAllItemsList",
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

    $scope.tempItemDataSource = [];

    $scope.itemsOptions = {
        dataSource: $scope.itemsDataSource,
        dataTextField: "ITEM_EDESC",
        dataValueField: "ITEM_CODE",
        dataBound: function () {

            $scope.tempItemDataSource = $("#firstloaditems").data('kendoComboBox').dataSource.data();
        }
    }

    $scope.AddPurchaseOrder = function () {

        $scope.showFormPanel = true;
        $scope.hideonedit = true;
        $scope.cleareditcontent();
        $scope.resettotal();
    }

    $scope.countryListTemp = function () {
        var response = $http({
            method: "get",
            url: "/api/ProformaInvoice/getCountryCodes?filter=" + "",
            dataType: "json",

        });

        return response.then(function (result) {

            $scope.countryListTemp = result.data;
            $scope.countryslist = {
                optionLabel: "--Select Country--",
                filter: "contains",
                dataTextField: "COUNTRY_EDESC",
                dataValueField: "COUNTRY_CODE",
                dataSource: $scope.countryListTemp,

            };


        }, function errorCallback(response) {
            displayPopupNotification("Fields should not be empty.", "error");
        });
    }

    $scope.hscodeListTemp = function () {
        var response = $http({
            method: "get",
            url: "/api/ProformaInvoice/getHsCodes?filter=" + "",
            dataType: "json",

        });

        return response.then(function (result) {

            $scope.hscodeListTemp = result.data;
            $scope.hscodeslist = {
                optionLabel: "--Hs Code--",
                filter: "contains",
                dataTextField: "HS_CODE",
                dataValueField: "HS_CODE",
                dataSource: $scope.hscodeListTemp,

            };


        }, function errorCallback(response) {
            displayPopupNotification("Fields should not be empty.", "error");
        });
    }

    $scope.countryListTemp();

    $scope.hscodeListTemp();

    function getIndexOf(arr, val, prop) {
        var l = arr.length,
            k = 0;
        for (k = 0; k < l; k = k + 1) {
            if (arr[k][prop] === val) {
                return k;
            }
        }
        return false;
    }


    $scope.onItemsChange = function (e, index) {
        var count = 0;
        var selectedItemCode = "";
        if (e.dataItem == undefined) {
            selectedItemCode = e.sender.dataItem().ITEM_CODE;
            $scope.tablelist[index].MU_CODE = e.sender.dataItem().INDEX_MU_CODE;
        }
        else {
            selectedItemCode = e.dataItem.ITEM_CODE;
            $scope.tablelist[index].MU_CODE = e.dataItem.INDEX_MU_CODE;
            $scope.tablelist[index].ITEM_CODE = e.dataItem.ITEM_CODE;
            $scope.tablelist[index].ITEM_EDESC = e.dataItem.ITEM_EDESC;
        }
        for (i = 0; i < $scope.tablelist.length; i++) {

            if (selectedItemCode == $scope.tablelist[i].ITEM_CODE) {
                count++;
                if (count == 2) {
                    $scope.tablelist[i].ITEM_CODE = "";
                    $scope.tablelist[i].ITEM_EDESC = "";
                    displayPopupNotification("Item Already Exist.", "warning");
                    return;
                }
            }
            if ($scope.tablelist[index].ADDED != 'Y') {
                $scope.tablelist[index].EDITED = 'Y';
            }

        }

    }

    $scope.deliveryPlaceTypeDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/LcEntry/GetDeliveryPlaceType",
            }
        },
    };

    $scope.BindPurchaseOrderDataFromExcelFile = function (result, loadmore) {
        $scope.itemlist = [];
        $scope.tablelist = [];
        $scope.itemlist = result;
        $.each($scope.itemlist, function (i, obj) {
            $scope.tablelist.push({
                ITEM_CODE: obj.ITEM_CODE,
                ITEM_EDESC: obj.ITEM_EDESC,
                QUANTITY: obj.QUANTITY,
                AMOUNT: obj.AMOUNT,
                VALUE: '',
                HS_CODE: obj.HS_CODE,
                MU_CODE: obj.MU_CODE,
                COUNTRY_OF_ORIGIN: obj.COUNTRY_OF_ORIGIN,
            });

        });

        $scope.$apply();
    }

    $scope.deliveryPlaceType = {
        optionLabel: "--Select DeliveryPlace Type--",
        dataSource: $scope.deliveryPlaceTypeDatasource,
    };

    $scope.clearonItemChange = function () {
        $scope.tablelist = [];
        $scope.issues = [];
        $scope.itemlist = [];
    }

 
    $scope.ItemsOnChange = function (e) {
        $scope.itemlist = [];
        $scope.clearonItemChange;
       var OrderNumber = e.dataItem.ORDER_NO;
       debugger;
       var exist= false;
        var data = $("#grid").data("kendoGrid").dataSource.data();
        if (data !== undefined) {
            for (var i = 0; i < data.length; i++) {
                var dataItem = data[i];
                if (dataItem.ORDER_NO === OrderNumber) {
                    displayPopupNotification("Order Number Already Exist", "warning");
                    $scope.lc.purchaseOrder = "";
                    exist = true;
                    break;
               }
            }
        };
        if (exist===false) {
            showloader();
            var response = $http({
                method: "get",
                url: "/api/ProformaInvoice/getItemDetailsByOrderCode?OrderCode=" + OrderNumber,
                dataType: "json"
            });
        }
       

        return response.then(function (data) {

            //displayPopupNotification("Purchase Order Already Saved", "error");

            if (data.data.length == 0) {
                $scope.additems = true;

            }
            else {

                if ($scope.issues.length > 0) {
                    $scope.issues = [];
                }
                if (data.data[0].CURRENCY_CODE) {
                    $scope.lc.currency = data.data[0].CURRENCY_CODE;
                    $scope.lc.currencycode = data.data[0].CURRENCY_CODE;
                }
                else {
                    $scope.lc.currency = '';
                    $scope.lc.currencycode = '';
                }

                $scope.additems = true;
                $scope.itemlist = data.data;
                $scope.tablelist = [];
                $scope.Total_Quantity = 0;
                $scope.Total_Amount = 0;

                for (i = 0; i < $scope.itemlist.length; i++) {
                    $scope.tablelist.push({
                        ITEM_CODE: $scope.itemlist[i].ITEM_CODE,
                        ITEM_EDESC: $scope.itemlist[i].ITEM_EDESC,
                        QUANTITY: $scope.itemlist[i].CALC_QUANTITY,
                        AMOUNT: $scope.itemlist[i].CALC_UNIT_PRICE,
                        VALUE: '',
                        HS_CODE: '',
                        MU_CODE: $scope.itemlist[i].MU_CODE,
                        COUNTRY_OF_ORIGIN: '',
                    });

                    $scope.Total_Quantity = parseInt(parseInt($scope.Total_Quantity) + parseInt($scope.itemlist[i].CALC_QUANTITY));
                    $scope.Total_Amount = (parseFloat(parseFloat($scope.Total_Amount) + ($scope.itemlist[i].CALC_QUANTITY * parseFloat($scope.itemlist[i].CALC_UNIT_PRICE)))).toFixed(2);

                }
            } 
            var order_number = e.dataItem.ORDER_NO;
            $.when(d1).done(function (result) {
                debugger;
                $scope.GetPOData(order_number);

            });

            hideloader();
            d1.resolve(data);
        }, function errorCallback(response) {
            displayPopupNotification("Fields should not be empty.", "error");
        });
    }

    $scope.GetPOData = function (order_number) {
        var suppresponse = $http({
            method: "get",
            url: "/api/PurchaseOrder/getPurchaseOrderdateandsupplierByOrderCode?OrderCode=" + order_number,
            dataType: "json"
        });

        return suppresponse.then(function (data) {
            debugger;
            if (data.data != null) {
                if (data.data.POBenificary !== "" || data.data.POBenificary !== undefined) {

                    $('.image-placeholder').html("");
                    $scope.temporarycurrencycode = "";
                    $("#grid").data("kendoGrid").tbody.find("tr").removeClass('highlight');
                    $scope.resettotal();
                    debugger;
                    var kdata = $("#beneficiaryname").data("kendoDropDownList");
                    var karraydata = kdata.dataSource.data();
                    var debeficairycode = parseInt(data.data.POBenificary);
                    $("#beneficiaryname").data('kendoDropDownList').value(debeficairycode);
                    $scope.lc.orderdate = data.data.PODate;
                    $scope.lc.beneficiaryname = data.data.POBenificary;
                    $scope.lc.bnfname = data.data.POBenificary;
                   
                   
                }
                 
                
            }
            else {

                var ddl = $("#beneficiaryname").data("kendoDropDownList");
                ddl.dataSource.read();

                setTimeout(function () { $scope.$apply(); }, 1000);
                $("#beneficiaryname").data("kendoDropDownList").text("--Select Beneficiary--");
            }

        });

    };

    $scope.showtotal = function () {
        $scope.Total_Quantity = 0;
        $scope.Total_Rate = 0;
        $scope.Total_Amount = 0;
        $scope.TotalTablelist = [];
        $scope.TotalTablelist = angular.copy($scope.tablelist);
        for (i = 0; i < $scope.TotalTablelist.length; i++) {
            $scope.TotalTablelist[i].QUANTITY = $scope.TotalTablelist[i].QUANTITY == "" ? 0 : $scope.TotalTablelist[i].QUANTITY;
            $scope.TotalTablelist[i].AMOUNT = $scope.TotalTablelist[i].AMOUNT == "" ? 0 : $scope.TotalTablelist[i].AMOUNT;
            $scope.Total_Quantity = parseInt(parseInt($scope.Total_Quantity) + parseInt($scope.TotalTablelist[i].QUANTITY));
            $scope.Total_Amount = (parseFloat(parseFloat($scope.Total_Amount) + ($scope.TotalTablelist[i].QUANTITY) * parseFloat($scope.TotalTablelist[i].AMOUNT))).toFixed(2);
        }

    };

    $scope.incoTermsDatasource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/LCEntry/GetAllTermsByFilter",
            },
            parameterMap: function (data, action) {
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        var newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        var newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    var newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        },
    };

    $scope.incoTerms = {
        optionLabel: "--Select Terms--",
        filter: "contains",
        dataTextField: "TermName",
        dataValueField: "TermCode",
        dataSource: $scope.incoTermsDatasource,
        change: onTermsChange,
    };

    function onTermsChange() {

        var selectedterms = $('#incoterms').data("kendoDropDownList").text();
        if (selectedterms == "FOB") {
            $scope.lc.deliveryplacetype = "PORT";
            $scope.lc.dplacetype = "PORT";
            $scope.deliverydropdown = true;
            $scope.$apply();
        }
        else {
            $scope.lc.deliveryplacetype = "";
            $scope.lc.dplacetype = "";
            $scope.deliveryPlaceType = [];
            $scope.deliverydropdown = false;
            $scope.$apply();
        }
    }

    $scope.pTermsDatasource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/LCEntry/GetAllPTermsByFilter",
            },
            parameterMap: function (data, action) {
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        var newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        var newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    var newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        },
    };

    $scope.pTerms = {
        optionLabel: "--Select Payment Terms--",
        filter: "contains",
        dataTextField: "PTermName",
        dataValueField: "PTermCode",
        dataSource: $scope.pTermsDatasource,

    };

    $scope.LoadtermsDay = function () {
        var paymentterms = $scope.lc.paymentterms;
        switch (paymentterms) {
            case '121':
                $scope.lc.creditdays = 10;
                break;
            case '122':
                $scope.lc.creditdays = 45;
                break;
            default:
                $scope.lc.creditdays = 0;
        }

    };

    $scope.currencyDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/PurchaseOrder/getCurrency",
            },

        },
    };

    $scope.currencylist = {
        optionLabel: "--Select Currency--",
        dataTextField: "CURRENCY_CODE",
        dataValueField: "CURRENCY_CODE",
        dataSource: $scope.currencyDatasource,

    };

    $scope.BindDataFromExcelFile = function (result, loadmore) {
        $scope.itemlist = [];
        $scope.tablelist = [];
        $scope.itemlist = result;
        $.each($scope.itemlist, function (i, obj) {
            $scope.tablelist.push({
                ITEM_CODE: obj.ITEM_CODE,
                ITEM_EDESC: obj.ITEM_EDESC,
                QUANTITY: obj.QUANTITY,
                AMOUNT: obj.AMOUNT,
                VALUE: '',
                HS_CODE: obj.HS_CODE,
                MU_CODE: obj.MU_CODE,
                COUNTRY_OF_ORIGIN: obj.COUNTRY_OF_ORIGIN,
                ADDED: 'Y',
                EDITED: 'Y',
            });

        });
        $scope.$apply();
    }


    function onPTermsChange() {
        var selectedpterms = $('#pterms').data("kendoDropDownList").text();
        if (selectedpterms == "LC At Sight") {
            $scope.creditdaysreadonly = true;
            $scope.lc.creditdays = 0;
        }
        else {
            $scope.creditdaysreadonly = false;
        }
        $scope.$apply();
    }

    $scope.countrylist = {
        dataType: "json",
        serverFiltering: true,
        filter: "startswith",
        transport: {
            read: function (options) {
                $.ajax({
                    url: "/api/ProformaInvoice/getCountryCodes?filter=" + options.data.filter.filters[0].value,
                    type: "GET",
                    success: function (result) {
                        options.success(result);

                    },
                    error: function (result) {
                        options.error(result);
                    }
                });
            },
        }
    }

    $scope.ipPurchaseOrder = {
        dataType: "json",
        serverFiltering: true,
        filter: "startswith",
        transport: {
            read: function (options) {
                $.ajax({
                    url: "/api/PurchaseOrder/getAllIpPurchaseOrder?filter=" + options.data.filter.filters[0].value,
                    type: "GET",
                    success: function (result) {
                        options.success(result);

                    },
                    error: function (result) {
                        options.error(result);
                    }
                });
            },
        },


    }

    $scope.ipPurchaseOrderfilter = {
        dataType: "json",
        serverFiltering: true,
        filter: "startswith",
        transport: {
            read: function (options) {
                $.ajax({
                    url: "/api/PurchaseOrder/getAllIpPurchaseOrderfilter?filter=" + options.data.filter.filters[0].value,
                    type: "GET",
                    success: function (result) {
                        options.success(result);

                    },
                    error: function (result) {
                        options.error(result);
                    }
                });
            },
        },


    }


    $scope.hslist = {
        dataType: "json",
        serverFiltering: true,
        filter: "startswith",
        transport: {
            read: function (options) {
                $.ajax({
                    url: "/api/ProformaInvoice/getHsCodes?filter=" + options.data.filter.filters[0].value,
                    type: "GET",
                    success: function (result) {
                        options.success(result);

                    },
                    error: function (result) {
                        options.error(result);
                    }
                });
            },
        }
    }

    $scope.bnflistDatasource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/ProformaInvoice/getBeneficiary",
            },
            parameterMap: function (data, action) {
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        var newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        var newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    var newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        },
    };

    $scope.bnflist = {
        optionLabel: "--Select Beneficiary--",
        filter: "contains",
        dataTextField: "BNF_EDESC",
        dataValueField: "BNF_CODE",
        dataSource: $scope.bnflistDatasource,
    };

    $scope.shipmentTypeDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/ProformaInvoice/GetShipmentType",
            }
        },
    };

    $scope.shipmentType = {
        optionLabel: "--Select Shipment Type--",
        dataSource: $scope.shipmentTypeDatasource,
    };
    $scope.shipmentLoadTypeDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/ProformaInvoice/GetLoadShipmentType",
            }
        },
    };

    $scope.shipmentLoad = {
        optionLabel: "--Select Load Type--",
        dataSource: $scope.shipmentLoadTypeDatasource,
    };

    $scope.addIssue = function () {

        $scope.checkitemlistvalid = 1;
        if ($scope.hidefirstrow) { // first run
            $scope.hidefirstrow = false;
            $scope.noitems = false;
        }
        var length = $scope.itemlist.length;
        $scope.tablelist.push({
            ITEM_CODE: '',
            QUANTITY: '',
            AMOUNT: '',
            VALUE: '',
            HS_CODE: '',
            MU_CODE: '',
            COUNTRY_OF_ORIGIN: '',
        });
        var tablelistlength = $scope.tablelist.length;
        if (tablelistlength > 1) {
            $scope.tablelist[tablelistlength - 1].HS_CODE = $scope.tablelist[0].HS_CODE;
            $scope.tablelist[tablelistlength - 1].COUNTRY_OF_ORIGIN = $scope.tablelist[0].COUNTRY_OF_ORIGIN;
        }
        $scope.issues.push({
            sn: length,
            name: '',
            mucode: '',
            quantity: '',
            value: '',
            total: '',
            hscode: '',
            country: '',
        });
    }

    $scope.removeIssue = function (index) {

        var itemLength = $scope.itemlist.length;
        if (itemLength > 0) {
            var currentIndex = index > itemLength ? index - itemLength : itemLength - index;
            $scope.issues.splice(currentIndex, 1);
        }
        else {
            $scope.issues.splice(index, 1);
        }
        $scope.tablelist.splice(index, 1);
        if ($scope.tablelist.length == 0) {
            $scope.noitems = true;
        }
        $scope.showtotal();
    }

    $scope.createpurchaseorder = function (isValid, options) {

        $scope.flag = true;
        var trackno = $scope.lctrack;
        var pocode = $scope.pocode;
        if (isValid) {
            if (trackno == undefined || trackno == 0) {

                if ($scope.itemlist.length == 0) {
                    $.each($scope.tablelist, function (value, index) {

                        if ($scope.tablelist[value].ITEM_CODE === "" || $scope.tablelist[value].QUANTITY === "" || $scope.tablelist[value].AMOUNT === "") {

                            displayPopupNotification("Please choose valid items.", "error");
                            $scope.flag = false;
                            $scope.checkitemlistvalid = 1;
                            return false;

                        }

                    });

                    if ($scope.checkitemlistvalid === 0) {

                        displayPopupNotification("Please Select one item atleast.", "warning");
                        $scope.flag = false;

                    }



                    if ($scope.flag == false) {
                        return;
                    }
                }

                showloader();

            }
            
            var podetails = {
                ORDER_NO: $scope.lc.purchaseOrder,
                PO_CODE: pocode,
                LC_TRACK_NO: trackno,
                BNF_BANK_CODE: $scope.lc.beneficiaryname,
                WEEK_NUMBER: $scope.lc.weeknumber,
                CURRENCY_CODE: $scope.lc.currency,
                TRANSSHIPMENT: $scope.tsradio.radio,
                ORDER_DATE: $scope.lc.orderdate,
                MANUAL_NUMBER: $scope.lc.manualno,
                TERMS_CODE: $scope.lc.incoterms,
                PTERMS_CODE: $scope.lc.pterms,
                CREDIT_DAYS: $scope.lc.creditdays,
                DELIVERY_PLACE_TYPE: $scope.lc.deliveryplacetype,
                DELIVERY_PLACE: $scope.lc.deliveryplace,
                APP_NAME: $scope.lc.appname,
                APP_ADDRESS: $scope.lc.appaddress,
                BILL_COMPANY_NAME: $scope.lc.billcompname,
                BILL_COMPANY_ADD: $scope.lc.billcompadd,
                BILL_COMPANY_PHONE: $scope.lc.billcompPhone,
                SHIP_COMPANY_NAME: $scope.lc.shipcompname,
                SHIP_COMPANY_ADD: $scope.lc.shipcompadd,
                SHIP_COMPANY_PHONE: $scope.lc.shipcompPhone,
                CONTACT_NAME: $scope.lc.contactname,
                CONTACT_PHONE: $scope.lc.contactphone,
                CONTACT_EMAIL: $scope.lc.contactemail,
                VALIDITY_DATE: $scope.lc.validitydate,
                LEAD_TIME: $scope.lc.daynumber + " " + $scope.lc.calenderdays,
                EST_DELIVERY_DATE: $scope.lc.estdeliverydate,
                PPF: $scope.lc.ppf,
                REMARKS: $scope.lc.remarks,
                Itemlist: $scope.tablelist,
            };
            debugger;
            var poentryurl = window.location.protocol + "//" + window.location.host + "/api/PurchaseOrder/CreatePurchaseOrder";
            var response = $http({
                method: "post",
                url: poentryurl,
                data: podetails,
                dataType: "json"
            });

            return response.then(function (data) {

                if (options == "save") {
                    $scope.showFormPanel = false;
                }
                if (data.data != null) {
                    $('#lctrackno').val(data.data.LC_TRACK_NO);
                    $('#purchaseorder').val(data.data.PO_CODE);
                }
                $scope.cancelpo();
                myDropzone.processQueue();
                $('#grid').data("kendoGrid").dataSource.read();
                hideloader();
                displayPopupNotification("Purchase Order Inserted Successfully.", "success");


            }, function errorCallback(response) {

                if (response.status == "304") {
                    displayPopupNotification("Order Number Already Exist.", "warning");
                }
                else {
                    displayPopupNotification("Error Occured", "error");
                }
                hideloader();

            });
        }

        else {


            displayPopupNotification("Fields should not be empty.", "error");
            hideloader();
        }

    };

    $scope.onChangeCountryCode = function (index) {

        $scope.itemlist[index]["HsCode"] = $scope.lc.hs;
    }

    $scope.getImage = function (mylist, ATTACH_DOC) {

        if (mylist != null) {
            var img = "";
            var il = "";
            for (var i = 0; i < mylist.length; i++) {

                img = img + "<img src='" + mylist[i] + "' height=20 />";
                var extension = mylist[i].substr((mylist[i].lastIndexOf('.') + 1));
                extension = extension.toLowerCase();

                switch (extension) {
                    case 'jpg':
                    case 'jpeg':
                        il = il + "<a target='_blank' href='" + mylist[i] + "' data-toggle='tooltip' title='Hooray!'><i class='fa fa-file-image-o image'/></i></a>"
                        break;
                    case 'png':
                        il = il + "<a target='_blank' href='" + mylist[i] + "' ><i class='fa fa-file-image-o image' /></i></a>"
                        break;
                    case 'pdf':
                        il = il + "<a target='_blank' href='" + mylist[i] + "' ><i class='fa fa-file-pdf-o image' /></i></a>"
                        break;
                    case 'docx':
                        il = il + "<a target='_self' href='" + mylist[i] + "' download><i class='fa fa-file-word-o image' /></i></a>"
                        break;
                    case 'xls':
                    case 'xlsx':
                        il = il + "<a target='_self' href='" + mylist[i] + "' download><i class='fa fa-file-excel-o image' /></i></a>"
                        break;
                    default:
                        il = il + "<a target='_self' href='" + mylist[i] + "' data-toggle='tooltip' title='Hooray!' download><i class='fa fa-file-text-o image' /></i></a>"
                }

            }
            return $sce.trustAsHtml(il);
        }
        else if (ATTACH_DOC != null) {

            img = "<img src='" + ATTACH_DOC + "' height=20/>";
            var extension = ATTACH_DOC.substr((ATTACH_DOC.lastIndexOf('.') + 1));
            extension = extension.toLowerCase();
            switch (extension) {
                case 'jpg':
                case 'jpeg':
                    il = "<a target='_blank' href='" + ATTACH_DOC + "' data-toggle='tooltip'  title='Hooray!'><i class='fa fa-file-image-o'/></i></a>"
                    break;
                case 'png':
                    il = "<a target='_blank' href='" + ATTACH_DOC + "' ><i class='fa fa-file-image-o' /></i></a>"
                    break;
                case 'pdf':
                    il = "<a target='_blank' href='" + ATTACH_DOC + "'><i class='fa fa-file-pdf-o' /></i></a>"
                    break;
                case 'docx':
                    il = "<a target='_self' href='" + ATTACH_DOC + "' download><i class='fa fa-file-word-o' /></i></a>"
                    break;
                case 'xls':
                case 'xlsx':
                    il = "<a target='_self' href='" + ATTACH_DOC + "' download><i class='fa fa-file-excel-o' /></i></a>"
                    break;
                default:
                    il = "<a target='_self' href='" + ATTACH_DOC + "' data-toggle='tooltip' title='Hooray!' download><i class='fa fa-file-text-o' /></i></a>"
            }
            return $sce.trustAsHtml(il);
        }
        else {
            il = "";
            return il;
        }
    }

    $scope.cancelpo = function () {

        $scope.lc = {};
        $scope.tablelist = [];
        $scope.lc.orderdate = $filter('date')(new Date, "MM/dd/yyyy");
        $scope.lc.validitydate = $filter('date')(new Date, "MM/dd/yyyy"),
            $scope.lc.estdeliverydate = $filter('date')(new Date, "MM/dd/yyyy"),
            $scope.lctrack = 0;
        $scope.pocode = 0;
        $scope.lc.incoterms = "";
        $scope.lc.terms = "";
        $scope.lc.pterms = "";
        $scope.lc.paymentterms = "";
        $scope.tablelist;
        $scope.lc.deliveryplacetype = "";
        $scope.lc.dplacetype = "";
        $scope.lc.shipmentType = '';
        $scope.lc.shipmentload = '';
        $scope.lc.dplacetype = "";
        $scope.lc.beneficiaryname = "";
        $scope.lc.bnfname = "";
        $scope.saveAction = "Save";
        $scope.scAction = "Save & Continue";
        $scope.lc.daynumber = "";
        $scope.noitems = true;
        $scope.itemlist = [];
        $scope.issues = [];
        $scope.lc.calenderdays = "DAY";
        $scope.hidefirstrow = true;
        $scope.ItemsShow = true;
        $scope.showFormPanel = false;
        $('.image-placeholder').html("");
        $scope.temporarycurrencycode = "";
        $("#grid").data("kendoGrid").tbody.find("tr").removeClass('highlight');
        $scope.resettotal();
        $scope.ContentModal = true;
        $scope.onedit = false;
    }

    $scope.allitemlist = {
        dataType: "json",
        serverFiltering: true,
        filter: "startswith",
        transport: {
            read: function (options) {

                $.ajax({
                    url: "/api/PurchaseOrder/getAllItemsList?filter=" + options.data.filter.filters[0].value,
                    type: "GET",
                    success: function (result) {
                        options.success(result);

                    },
                    error: function (result) {
                        options.error(result);
                    }
                });
            },
        },
        onChange: function (e) {
            $scope.itemname;
        },
        onSelect: function (e) {
            $scope.itemname;
        },
        select: function (e) {
            $scope.itemname;
        }
    }



    $scope.FilterGrid = function (ORDER_NO) {
        $("#grid").data("kendoGrid").tbody.find("tr").css("background-color", "");
        var data = $("#grid").data("kendoGrid").dataSource.data();
        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
            if (dataItem.ORDER_NO == ORDER_NO) {
                $("#grid").data("kendoGrid").tbody.find("tr[data-uid=" + dataItem.uid + "]").css("background-color", "#80ddaa");
            }
        }

    };

    $scope.mainGridOptions = {
        toolbar: ["excel"],

        dataSource: {
            type: "json",
            transport: {

                read: "/api/PurchaseOrder/getAllPurchaseOrders"
            },

            pageSize: 100,
            serverPaging: false,
            serverSorting: false,
            schema: {
                model: {
                    fields: {
                        ORDER_DATE: { type: "date" },
                        LC_TRACK_NO: { type: "number" },
                        CREDIT_DAYS: { type: "number" },
                    }
                },
            },

        },
        toolbar: kendo.template($("#toolbar-template").html()),
        excel: {
            allPages: true
        },
        sortable: true,
        pageable: true,
        groupable: true,
        resizable: true,
        pageable: {
            alwaysVisible: true,
            pageSizes: [5, 10, 20, 100]
        },
        filterable: {
            extra: false,
            operators: {
                number: {
                    eq: "Is equal to",
                    neq: "Is not equal to",
                    gte: "is greater than or equal to	",
                    gt: "is greater than",
                    lte: "is less than or equal",
                    lt: "is less than",
                },
                string: {

                    eq: "Is equal to",
                    neq: "Is not equal to",
                    startswith: "Starts with	",
                    contains: "Contains",
                    doesnotcontain: "Does not contain",
                    endswith: "Ends with",
                },
                date: {
                    eq: "Is equal to",
                    neq: "Is not equal to",
                    gte: "Is after or equal to",
                    gt: "Is after",
                    lte: "Is before or equal to",
                    lt: "Is before",
                }

            }
        },
        columnMenu: true,
        //Wordwrapmenu function calling
        columnMenuInit: function (e) {

            wordwrapmenu(e);
        },
        dataBound: function () {
            //this.expandRow(this.tbody.find("tr.k-master-row").first());
            $scope.detailExportPromises = [];
            $scope.EditPO = function (dataItem) {
                $scope.onedit = true;
                $scope.saveAction = "Update";
                $scope.hideonedit = false;
                $scope.ItemsShow = false;
                $scope.scAction = "Update & Continue";
                $scope.ContentModal = false;
                $scope.disableeditcontent();
                $scope.Allcheckbox = false;
                $scope.showFormPanel = true;
                $scope.pocode = dataItem.PO_CODE;
                $scope.purchaseorder = dataItem.PO_CODE;
                $scope.lc.daynumber = dataItem.LEAD_TIME == null ? "" : parseInt((dataItem.LEAD_TIME).split(" ")[0]);
                $scope.lc.calenderdays = dataItem.LEAD_TIME == null ? "" : (dataItem.LEAD_TIME).split(" ")[1];
                $scope.lc.validitydate = dataItem.VALIDITY_DATE;
                $scope.lctrack = dataItem.LC_TRACK_NO;
                $scope.lc.purchaseOrder = dataItem.ORDER_NO;
                $scope.FilterGrid(dataItem.ORDER_NO);
                $scope.lc.beneficiaryname = dataItem.BNF_BANK_CODE;
                $scope.lc.weeknumber = dataItem.WEEK_NUMBER;
                $scope.lc.bnfname = dataItem.BNF_BANK_CODE;
                $scope.lc.orderdate = $filter('date')(dataItem.ORDER_DATE, "MM/dd/yyyy");
                $scope.lc.manualno = dataItem.MANUAL_NUMBER;
                $scope.lc.currency = dataItem.CURRENCY_CODE;
                $scope.lc.currencycode = dataItem.CURRENCY_CODE;
                $scope.temporarycurrencycode = dataItem.CURRENCY_CODE;
                $scope.lc.incoterms = dataItem.TERMS_CODE;
                $scope.lc.terms = dataItem.TERMS_CODE;
                debugger;
                if (dataItem.TERMS_EDESC == "FOB") {
                    $scope.lc.deliveryplacetype = "PORT";
                    $scope.lc.dplacetype = "PORT";
                    $scope.deliverydropdown = true;
                    //$scope.$apply();
                }
                else {
                    $scope.deliverydropdown = false;
                    $scope.lc.deliveryplacetype = dataItem.DELIVERY_PLACE_TYPE;
                    $scope.lc.dplacetype = dataItem.DELIVERY_PLACE_TYPE;
                    //$scope.$apply();
                }
                $scope.lc.pterms = dataItem.PTERMS_CODE;
                $scope.lc.paymentterms = dataItem.PTERMS_CODE;

                if (dataItem.PTERMS_EDESC == "LC At Sight") {
                    $scope.creditdaysreadonly = true;
                    $scope.lc.creditdays = 0;
                }
                else {
                    $scope.creditdaysreadonly = "";
                }
                $scope.lc.creditdays = dataItem.CREDIT_DAYS;
                $scope.lc.deliveryplace = dataItem.DELIVERY_PLACE;
                $scope.lc.appname = dataItem.APP_NAME;
                $scope.lc.appaddress = dataItem.APP_ADDRESS;
                $scope.lc.billcompname = dataItem.BILL_COMPANY_NAME;
                $scope.lc.billcompadd = dataItem.BILL_COMPANY_ADD;
                $scope.lc.billcompPhone = dataItem.BILL_COMPANY_PHONE;
                $scope.lc.shipcompname = dataItem.SHIP_COMPANY_NAME;
                $scope.lc.shipcompadd = dataItem.SHIP_COMPANY_ADD;
                $scope.lc.shipcompPhone = dataItem.SHIP_COMPANY_PHONE;
                $scope.lc.contactname = dataItem.CONTACT_NAME;
                $scope.lc.contactphone = dataItem.CONTACT_PHONE;
                $scope.lc.contactemail = dataItem.CONTACT_EMAIL;
                $scope.lc.validitydate = $filter('date')(dataItem.VALIDITY_DATE, "MM/dd/yyyy");
                $scope.lc.estdeliverydate = $filter('date')(dataItem.EST_DELIVERY_DATE, "MM/dd/yyyy");
                $scope.lc.remarks = dataItem.REMARKS;
                $scope.tsradio = {
                    radio: dataItem.TRANSSHIPMENT
                };
                //due to shipment validation//
                $scope.lc.shipmentfrom = "val";
                $scope.lc.shipmentTo = "val";
                $scope.lc.shipmentType = "val";
                $scope.lc.shipmentload = "val";
                ////
                $('.image-placeholder').html("");
                var images = "";
                if (dataItem.FILE_DETAIL != null) {
                    var extension = dataItem.FILE_DETAIL.substr((dataItem.FILE_DETAIL.lastIndexOf('.') + 1));
                    extension = extension.toLowerCase();
                    switch (extension) {
                        case 'jpg':
                        case 'jpeg':
                            images += "<div class='col-md-2'><img class='img img-responsive img-thumbnail' style='height:57px' src='" + dataItem.FILE_DETAIL + "'/><input type='button' onclick='RemoveImage(this)' data-src='" + dataItem.FILE_DETAIL + "' class='RemoveImage btn red btn-xs' value='Remove'> </div>"

                            break;
                        case 'pdf':
                            images += "<div class='col-md-2'><i class='fa fa-file-pdf-o fa-4x' style='padding-top:25px' /></i><input type='button' data-src='" + dataItem.FILE_DETAIL + "' onclick='RemoveImage(this)' class='RemoveImage btn red btn-xs' value='Remove'></div>"

                            break;
                        case 'docx':
                            images += "<div class='col-md-2'><i class='fa fa-file-word-o fa-4x' style='padding-top:25px'/></i><input data-src='" + dataItem.FILE_DETAIL + "' type='button' onclick='RemoveImage(this)' class='RemoveImage btn red btn-xs' value='Remove'></div>"

                            break;
                        case 'xls':
                        case 'xlsx':
                            images += "<div class='col-md-2'><i class='fa fa-file-excel-o fa-4x' style='padding-top:25px' /></i><input type='button' data-src='" + dataItem.FILE_DETAIL + "' onclick='RemoveImage(this)' class='RemoveImage btn red btn-xs' value='Remove'></div>"

                            break;
                        default:
                            images += "<div class='col-md-2'><i class='fa fa-file-text-o fa-4x' style='padding-top:25px' /></i><input type='button' data-src='" + dataItem.FILE_DETAIL + "' onclick='RemoveImage(this)' class='RemoveImage btn red btn-xs' value='Remove'></div>"

                            break;
                    }

                }
                else {

                    $.each(dataItem.mylist, function (index, value) {

                        var extension = value.substr((value.lastIndexOf('.') + 1));

                        extension = extension.toLowerCase();
                        switch (extension) {
                            case 'jpg':
                                images += "<div class='col-md-2'><img class='img img-responsive img-thumbnail' style='height:57px' src='" + value + "' /> <input type='button' onclick='RemoveImage(this)' data-src='" + value + "' onclick='RemoveImage()' class='RemoveImage btn red btn-xs' value='Remove'></div>"

                                break;
                            case 'pdf':
                                images += "<div class='col-md-2'><i class='fa fa-file-pdf-o fa-4x' style='padding-top:25px' /></i> <input  data-src='" + value + "' type='button' onclick='RemoveImage(this)' class='RemoveImage btn red btn-xs' value='Remove' ></div>"

                                break;
                            case 'docx':
                                images += "<div class='col-md-2'><i class='fa fa-file-word-o fa-4x' style='padding-top:25px'/></i> <input type='button' data-src='" + value + "' onclick='RemoveImage(this)' class='RemoveImage btn red btn-xs' value='Remove'></div>"

                                break;
                            case 'xls':
                            case 'xlsx':
                                images += "<div class='col-md-2'><i class='fa fa-file-excel-o fa-4x' style='padding-top:25px' /></i> <input type='button' data-src='" + value + "' onclick='RemoveImage(this)' class='RemoveImage btn red btn-xs' value='Remove'></div>"

                                break;
                            default:
                                images += "<div class='col-md-2'><i class='fa fa-file-text-o fa-4x' style='padding-top:25px' /></i> <input type='button' data-src='" + value + "' onclick='RemoveImage(this)' class='RemoveImage btn red btn-xs' value='Remove'></div>"

                        }

                    });
                }

                $('.image-placeholder').html(images);
            }

            $scope.DeletePO = function (dataItem) {

                bootbox.confirm("Are you sure you want to delete?", function (result) {

                    if (result) {

                        dataItem.PO_CODE;
                        var response = $http({
                            method: "get",
                            url: "/api/PurchaseOrder/deletePO?trackno=" + dataItem.LC_TRACK_NO + "&sno=" + dataItem.PO_CODE,
                            dataType: "json"
                        });
                        return response.then(function (data) {

                            $("#grid").data("kendoGrid").dataSource.read();
                            displayPopupNotification("Successfully deleted Purchase Order.", "success");

                        }, function errorCallback(response) {
                            displayPopupNotification("Error Occured.", "error");
                        });

                    }
                });
            }

            $('div').removeClass('.k-header k-grid-toolbar');
        },
        excelExport: function (e) {
            e.preventDefault();

            var workbook = e.workbook;
            $scope.detailExportPromises = [];
            var masterData = e.data;
            for (var rowIndex = 0; rowIndex < masterData.length; rowIndex++) {

                var itemgrid = $($('.itemgrid')[rowIndex]).data("kendoGrid")
                if (itemgrid != undefined) {

                    if (masterData[rowIndex].LC_TRACK_NO != itemgrid.dataSource.data()[rowIndex].LC_TRACK_NO) {

                        isEqual = false;

                    }
                    else {
                        $scope.exportChildData(masterData[rowIndex].LC_TRACK_NO, rowIndex);

                    }
                }
            }
            $.when.apply(null, $scope.detailExportPromises)
                .then(function () {
                    // get the export results
                    var detailExports = $.makeArray(arguments);

                    // sort by masterRowIndex
                    detailExports.sort(function (a, b) {
                        return a.masterRowIndex - b.masterRowIndex;
                    });

                    // add an empty column
                    workbook.sheets[0].columns.unshift({
                        width: 30
                    });

                    // prepend an empty cell to each row
                    for (var i = 0; i < workbook.sheets[0].rows.length; i++) {
                        workbook.sheets[0].rows[i].cells.unshift({});
                    }

                    // merge the detail export sheet rows with the master sheet rows
                    // loop backwards so the masterRowIndex doesn't need to be updated
                    for (var i = detailExports.length - 1; i >= 0; i--) {
                        var masterRowIndex = detailExports[i].masterRowIndex + 1; // compensate for the header row

                        var sheet = detailExports[i].sheet;

                        // prepend an empty cell to each row
                        for (var ci = 0; ci < sheet.rows.length; ci++) {
                            if (sheet.rows[ci].cells[0].value) {
                                sheet.rows[ci].cells.unshift({});
                            }
                        }

                        // insert the detail sheet rows after the master row
                        [].splice.apply(workbook.sheets[0].rows, [masterRowIndex + 1, 0].concat(sheet.rows));
                    }

                    // save the workbook
                    kendo.saveAs({
                        dataURI: new kendo.ooxml.Workbook(workbook).toDataURL(),
                        fileName: "PurchaseOrder.xlsx"
                    });
                });
        },
        columns: [{
            field: "LC_TRACK_NO",
            title: "Track No",
            attributes: { style: "text-align:right;" },
        },
        {
            field: "ORDER_NO",
            title: "Order No",
            width: 130
        },
        {
            field: "BNF_EDESC",
            title: "Beneficiary",

        }, {
            field: "ORDER_DATE",
            title: "Order Date",
            template: "#= kendo.toString(kendo.parseDate(ORDER_DATE),'M/dd/yyyy') #",
        }, {
            field: "TERMS_EDESC",
            title: "Terms Name",
        }, {
            field: "PTERMS_EDESC",
            title: "Payment Terms",

        }, {
            field: "CREDIT_DAYS",
            title: "Credit Days",
            attributes: { style: "text-align:right;" },

        },
        {
            field: "mylist",
            title: "Images",
            template: '<span ng-bind-html= "getImage(dataItem.mylist, dataItem.FILE_DETAIL)"> </span>',
            format: "{0:n0}",

        },
        {
            field: "ID", title: "Action", sortable: false, filterable: false,
            template: '<a class="edit glyphicon glyphicon-edit" title="Edit" ng-click="EditPO(dataItem)" style="color:grey;"><span class="sr-only"></span>'
        }]
    };

    $scope.HistoryPO = function (dataItem) {

        $scope.historyitemGridOptions = function () {


            return {
                dataSource: {
                    type: "json",
                    transport: {
                        read: {
                            url: "/api/PurchaseOrder/getAllPOHistoryItemsList?lctrackno=" + dataItem.LC_TRACK_NO,
                            dataType: "json"
                        },
                    },

                    serverPaging: false,
                    serverSorting: false,
                    serverFiltering: false,
                    pageSize: 100,
                    schema: {
                        model: {
                            id: "ITEM_CODE",
                            fields: {
                                ITEM_EDESC: { validation: { required: true } },
                                MU_CODE: { validation: { required: true } },
                                AMOUNT: { type: "number", validation: { min: 0, required: true } },
                                QUANTITY: { type: "number", validation: { min: 0, required: true } },

                            }
                        }
                    }

                },
                scrollable: false,
                sortable: true,
                pageable: true,
                resizable: true,
                dataBinding: function () {
                    record = (this.dataSource.page() - 1) * this.dataSource.pageSize();
                },
                columns: [
                    { title: "SN", template: "#= ++record #", width: 20, attributes: { style: "text-align:right;" } },
                    { field: "ITEM_EDESC", title: "Item Name", width: "110px", editor: ItemsNameAutoCompleteEditor },
                    { field: "MU_CODE", title: "MU", width: "20px" },
                    { field: "QUANTITY", title: "Qty.", attributes: { style: "text-align:right;" }, width: "25px" },
                    { field: "AMOUNT", title: "Amt.", attributes: { style: "text-align:right;" }, width: "25px" },
                    { field: "CURRENCY_CODE", title: "Currency", width: "25px" },
                    { field: "HS_CODE", title: "Hs Code", width: "30px" },
                    { field: "COUNTRY_OF_ORIGIN", title: "Country", width: "30px" },
                    {
                        field: "CREATED_DATE", title: "Created Date", width: "40px", format: "{0:M/dd/yyyy}",
                        template: "#= CREATED_DATE == null ? '' :kendo.toString(kendo.parseDate(CREATED_DATE),'M/dd/yyyy') #",
                    },
                    { field: "CREATED_BY_EDESC", title: "Created By", width: "50px" },

                    {
                        field: "LAST_MODIFIED_DATE", title: "Updated date", width: "50px", format: "{0:M/dd/yyyy}",
                        template: "#= LAST_MODIFIED_DATE ==null ? '' : kendo.toString(kendo.parseDate(LAST_MODIFIED_DATE),'M/dd/yyyy') #",
                    },
                    { field: "LAST_MODIFIED_BY_EDESC", title: "Updated By", width: "50px" },

                ],


            };

        };

        $scope.historyitemGridOptions();
        $scope.historydocumentGridOptions = function () {
            return {
                dataSource: {
                    type: "json",
                    transport: {
                        read: {
                            url: "/api/PurchaseOrder/getAllPOHistoryDocumentList?lctrackno=" + dataItem.LC_TRACK_NO,
                            dataType: "json"
                        },
                    },

                    serverPaging: false,
                    serverSorting: false,
                    serverFiltering: false,
                    pageSize: 100,
                    schema: {
                        model: {
                            id: "SNO",
                            fields: {


                            }
                        }
                    }

                },
                scrollable: false,
                sortable: true,
                pageable: true,
                resizable: true,
                dataBinding: function () {
                    record = (this.dataSource.page() - 1) * this.dataSource.pageSize();
                },
                columns: [
                    { title: "SN", template: "#= ++record #", width: 20, attributes: { style: "text-align:right;" } },
                    { field: "ORDER_NO", title: "Order Number", width: "80px" },
                    { field: "PINVOICE_NO", title: "Pinvoice Number", width: "80px" },
                    { field: "LC_NUMBER", title: "Lc Number", width: "80px", template: "#= SUPPLIER_EDESC == null?LC_NUMBER:SUPPLIER_EDESC #" },
                    { field: "INVOICE_NUMBER", title: "Invoice Number", width: "80px" },
                    {
                        field: "mylist", title: "Images", template: '<span ng-bind-html= "getImage(dataItem.mylist, dataItem.FILE_DETAIL)"> </span>',
                        width: "50px"
                    },
                    {
                        field: "CREATED_DATE", title: "Created Date", width: "40px", format: "{0:M/dd/yyyy}",
                        template: "#= CREATED_DATE == null ? '' :kendo.toString(kendo.parseDate(CREATED_DATE),'M/dd/yyyy') #",
                    },
                    { field: "CREATED_BY_EDESC", title: "Created By", width: "50px" },

                    {
                        field: "LAST_MODIFIED_DATE", title: "Updated date", width: "50px", format: "{0:M/dd/yyyy}",
                        template: "#= LAST_MODIFIED_DATE ==null ? '' : kendo.toString(kendo.parseDate(LAST_MODIFIED_DATE),'M/dd/yyyy') #",
                    },
                    { field: "LAST_MODIFIED_BY_EDESC", title: "Updated By", width: "50px" },

                ],


            };
        }
        $scope.historydocumentGridOptions();
        $('#historymodal').modal('show');
    }

    var itemcode = "";
    var countrycode = "";
    var mucode = "";
    $scope.itemGridOptions = function (dataItem) {

        return {
            dataSource: {
                type: "json",
                transport: {

                    read: {
                        url: "/api/PurchaseOrder/getAllPOItemsList?filter=" + dataItem.LC_TRACK_NO,
                        dataType: "json"
                    },
                    update: {
                        url: window.location.protocol + "//" + window.location.host + "/api/PurchaseOrder/UpdateItems",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        complete: function (e) {

                            if (e.responseJSON.MESSAGE === "Success") {
                                itemcode = "";
                                displayPopupNotification("Successfully Updated Item.", "success");
                            }
                            else if (e.responseJSON.MESSAGE === "Error") {
                                displayPopupNotification("Item Already Exists.", "warning");

                            }
                            else {
                                displayPopupNotification(e.responseJSON.MESSAGE, "error");
                            }
                            $.each($(".itemgrid"), function (i, value) {
                                $(value).data("kendoGrid").dataSource.read();
                            });
                            $scope.resettotal();
                        }

                    },
                    create: {

                        url: window.location.protocol + "//" + window.location.host + "/api/PurchaseOrder/CreateItems",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        complete: function (e) {

                            if (e.responseJSON.MESSAGE === "Success") {
                                itemcode = "";
                                displayPopupNotification("Successfully Created Item.", "success");
                            }
                            else if (e.responseJSON.MESSAGE === "Error") {
                                displayPopupNotification("Item Already Exists.", "warning");

                            }
                            else {
                                displayPopupNotification(e.responseJSON.MESSAGE, "error");
                            }
                            $.each($(".itemgrid"), function (i, value) {
                                $(value).data("kendoGrid").dataSource.read();
                            });
                            $scope.resettotal();
                        }
                    },
                    parameterMap: function (options, operation) {
                        debugger;

                        if (operation !== "read" && options.models) {

                            var data = options.models;
                            if (data[0].SNO === undefined) {
                                //insert case
                                data[0]["COUNTRY_CODE"] = (data[0].COUNTRY_CODE.COUNTRY_CODE === undefined) ? data[0].COUNTRY_CODE : data[0].COUNTRY_CODE.COUNTRY_CODE;
                                data[0]["HS_CODE"] = data[0].HS_CODE;
                            }
                            else {
                                debugger;
                                //update case
                                if (data[0].COUNTRY_CODE !== null && data[0].HS_CODE === null) {
                                    data[0]["COUNTRY_CODE"] = data[0].COUNTRY_CODE;
                                    data[0]["HS_CODE"] = "";
                                }
                                if (data[0].COUNTRY_CODE === null && data[0].HS_CODE !== null) {
                                    data[0]["COUNTRY_CODE"] = "";
                                    data[0]["HS_CODE"] = data[0].HS_CODE;
                                }
                                if (data[0].COUNTRY_CODE !== null && data[0].HS_CODE !== null) {
                                    if (data[0].COUNTRY_CODE.COUNTRY_CODE !== undefined) {
                                        data[0]["COUNTRY_CODE"] = data[0].COUNTRY_CODE.COUNTRY_CODE;
                                    }
                                    else {
                                        data[0]["COUNTRY_CODE"] = data[0].COUNTRY_CODE;
                                    }
                                    if (data[0].HS_CODE.HS_CODE !== undefined) {
                                        data[0]["HS_CODE"] = data[0].HS_CODE.HS_CODE;
                                    }
                                    else {
                                        data[0]["HS_CODE"] = data[0].HS_CODE;
                                    }
                                }
                                ////if both blank
                                //if (data[0].COUNTRY_CODE.COUNTRY_CODE !== undefined && data[0].HS_CODE.HS_CODE !== undefined) {
                                //    data[0]["COUNTRY_CODE"] = data[0].COUNTRY_CODE.COUNTRY_CODE;
                                //    data[0]["HS_CODE"] = data[0].HS_CODE.HS_CODE;
                                //}

                                ////if both not blank
                                //if (data[0].COUNTRY_CODE !== undefined && data[0].HS_CODE !== undefined) {
                                //    if (!data[0].hasOwnProperty("HS_CODE")) {
                                //        data[0]["COUNTRY_CODE"] = data[0].COUNTRY_CODE;
                                //        data[0]["HS_CODE"] = data[0].HS_CODE;
                                //    }

                                //}

                                ////if both CC blank only
                                //if (data[0].COUNTRY_CODE.COUNTRY_CODE !== undefined && data[0].HS_CODE !== undefined) {
                                //    data[0]["COUNTRY_CODE"] = data[0].COUNTRY_CODE.COUNTRY_CODE;
                                //    data[0]["HS_CODE"] = data[0].HS_CODE;
                                //}

                                ////if both HS blank only
                                //if (data[0].COUNTRY_CODE !== undefined && data[0].HS_CODE.HS_CODE !== undefined) {
                                //    data[0]["COUNTRY_CODE"] = data[0].COUNTRY_CODE;
                                //    data[0]["HS_CODE"] = data[0].HS_CODE.HS_CODE;
                                //}

                            }

                            data[0]["LC_TRACK_NO"] = dataItem.LC_TRACK_NO;
                            if (itemcode == "") {
                                data[0]["ITEM_CODE"] = data[0].ITEM_CODE;
                            }
                            else {
                                data[0]["ITEM_CODE"] = itemcode;
                            }
                            if ($scope.itemcodeformucode == "") {
                                $('#MuCodeAutoComplete').data("kendoAutoComplete").value();
                            }
                            else {
                                data[0]["MU_CODE"] = mucode;
                            }


                            return JSON.stringify(data);
                        }

                    }
                },
                batch: true,
                serverPaging: false,
                serverSorting: false,
                serverFiltering: false,
                pageSize: 50,
                schema: {
                    model: {
                        id: "ITEM_CODE",
                        fields: {
                            LC_TRACK_NO: { type: "number" },
                            ITEM_EDESC: { validation: { required: true } },
                            MU_CODE: { validation: { required: true } },
                            AMOUNT: { type: "number", validation: { min: 1, required: true } },
                            QUANTITY: { type: "number", validation: { min: 1, required: true } },
                            HS_CODE: { validation: { min: 1, required: false } },
                            COUNTRY_CODE: { validation: { min: 1, required: false } },
                        }
                    }
                },

            },
            scrollable: false,
            sortable: true,
            pageable: true,
            dataBinding: function () {
                record = (this.dataSource.page() - 1) * this.dataSource.pageSize();
            },
            toolbar: ["create"],
            excelExport: function (e) {
                // prevent saving the file
                e.preventDefault();

            },
            navigatable: true,
            resizable: true,
            columns: [

                { field: "ITEM_EDESC", title: "Item Name", width: "110px", editor: ItemsNameAutoCompleteEditor },
                { field: "MU_CODE", title: "Master Unit", width: "30px", editor: MUCodeAutoCompleteEditor },
                { field: "QUANTITY", title: "Quantity", attributes: { style: "text-align:right;" }, width: "50px" },
                { field: "AMOUNT", title: "Amount", attributes: { style: "text-align:right;" }, width: "50px" },
                { field: "HS_CODE", title: "Hs Code", width: "50px", editor: HsCodeAutoCompleteEditor },
                { field: "COUNTRY_CODE", title: "Country", width: "80px", editor: CountryAutoCompleteEditor },
                { command: ["edit"], title: "Action", width: "70px" },

            ],
            editable: "inline",

        };
    };

    $scope.exportChildData = function (lctrackno, rowIndex) {

        var deferred = $.Deferred();

        $scope.detailExportPromises.push(deferred);

        var rows = [{
            cells: [
                // First cell

                { value: "Item Name" },
                // Second cell
                { value: "MU_CODE" },
                // Third cell
                { value: "QUANTITY" },
                // Fourth cell
                { value: "AMOUNT" },
                // Fifth cell
                { value: "HS_CODE" },
                { value: "COUNTRY_EDESC" }
            ]
        }];


        $($('.itemgrid')[rowIndex]).data("kendoGrid").dataSource.filter({ field: "LC_TRACK_NO", operator: "eq", value: lctrackno });
        var exporter = new kendo.ExcelExporter({
            columns: [

                {
                    field: "ITEM_EDESC", title: "Item Name",
                }, {
                    field: "MU_CODE", title: "Master Unit",
                }, {
                    field: "QUANTITY", title: "Quantity",
                }, {
                    field: "AMOUNT", title: "Amount",
                }, {
                    field: "HS_CODE", title: "Hs Code",
                }, {
                    field: "COUNTRY_EDESC", title: "Country Name",
                }],
            dataSource: $($('.itemgrid')[rowIndex]).data("kendoGrid").dataSource.data(),
        });
        exporter.workbook().then(function (book, data) {

            deferred.resolve({

                masterRowIndex: rowIndex,
                sheet: book.sheets[0]
            });
        });



    };

    function ItemsNameAutoCompleteEditor(container, options) {

        $('<input id="ItemNameAutoComplete" required data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoAutoComplete({
                dataSource: $scope.allitemlist,
                dataTextField: "ITEM_EDESC",
                dataValueField: "ITEM_CODE",
                filter: "contains",
                change: function (e) {

                    var itemgriddata = $('.itemgrid').data("kendoGrid").dataSource.data();
                    var itemnameautocomplete = $("#ItemNameAutoComplete").data("kendoAutoComplete");
                    var datas = itemnameautocomplete.dataItem();

                    if (datas != undefined) {
                        itemcode = datas.ITEM_CODE;
                        mucode = datas.INDEX_MU_CODE;
                        var mucodeAutoComplete = $('#MuCodeAutoComplete').data("kendoAutoComplete");
                        mucodeAutoComplete.value(mucode);
                        $scope.itemcodeformucode = itemcode;

                    }
                    else {
                        displayPopupNotification("Please Select Items from AutoComplete.", "warning");
                    }


                }

            });

    }
    $scope.allMuCodelist = {
        dataType: "json",
        serverFiltering: true,
        filter: "startswith",
        autoBind: false,
        transport: {
            read: function (options) {
                $.ajax({
                    url: "/api/PurchaseOrder/?filter=" + options.data.filter.filters[0].value + "&itemcode=" + $scope.itemcodeformucode,
                    type: "GET",
                    success: function (result) {

                        options.success(result);

                    },
                    error: function (result) {

                        options.error(result);
                    }
                });
            },
        },
    }
    function MUCodeAutoCompleteEditor(container, options) {

        $('<input id="MuCodeAutoComplete" required data-bind="value:' + options.field + '" disabled/>')
            .appendTo(container)
            .kendoAutoComplete({
                dataSource: $scope.allMuCodelist,
                dataTextField: "MU_CODE",
                dataValueField: "MU_CODE",
                filter: "contains",
                minLength: 1,

            });
    }

    $scope.CalenderTypeDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/LCEntry/GetCalenderDaysType",
            }
        },
    };

    $scope.CalenderDays = {
        dataSource: $scope.CalenderTypeDatasource,
    };

    function HsCodeAutoCompleteEditor(container, options) {
        $('<input data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                dataSource: $scope.hslist,
                dataTextField: "HS_CODE",
                dataValueField: "HS_CODE",
                filter: "contains",
            });
    }
    function CountryAutoCompleteEditor(container, options) {
        $('<input id="CountryNameAutoComplete" data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                dataSource: $scope.countrylist,
                dataTextField: "COUNTRY_CODE",
                dataValueField: "COUNTRY_CODE",
                filter: "contains",

                change: function (e) {
                    var countrynameautocomplete = $("#CountryNameAutoComplete").data("kendoDropDownList");
                    var datas = countrynameautocomplete.dataItem();
                    if (datas != undefined) {
                        countrycode = datas.COUNTRY_CODE;
                    }
                }
            });
    };


});



app.service('PurchaseOrderService', function ($http) {
    var fac = {};


    return fac;

});
app.directive("limitTo", [function () {
    return {
        restrict: "A",
        link: function (scope, elem, attrs) {
            var limit = parseInt(attrs.limitTo);
            angular.element(elem).on("keypress", function (e) {
                if (this.value.length == limit) e.preventDefault();
            });
        }
    }
}]);

