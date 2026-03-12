app.controller('LCEntryController', function ($scope, $http, $sce, $filter, $q, LCEntryService) {

    $scope.saveAction = "Save";
    $scope.scAction = "Save & Continue";
    $scope.itemlist = [];
    $scope.beneficiarylist = [];
    $scope.lcstatus = [];
    $scope.shipmentType = [];
    $scope.shipmentLoad = [];
    $scope.allitemslist = [];
    $scope.hslist = [];
    $scope.creditdaysreadonly = false;
    $scope.showcreditdays = true;
    $scope.ItemsShow = true;
    $scope.tablelist = [];
    $scope.ContentModal = true;
    $scope.amendmentTextbox = true;
    $scope.isApproved = false;
    var temporarytableitems = [];
    $scope.filterData = {
        perfomainvoice: "",
        number: ""
    };



    $scope.chargeradio = {
        radio: 'A'
    };
    $scope.confirmradio = {
        radio: 'B'
    };
    $scope.psradio = {
        radio: 'Y'
    };
    $scope.confirmationradio = {
        radio: 'N'
    };
    $scope.transferradio = {
        radio: 'Y'
    };
    $scope.insuranceradio = {
        radio: 'N'
    };

    $scope.LCperfomainvoiceOnchange = function (kendoEvent) {
        if (kendoEvent.sender.dataItem() == undefined) {
            $('#perfomainvoiceautocomplete').data("kendoAutoComplete").value([]);
        }
    }

    $scope.allitemslist = {
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

    $scope.onItemsChange = function (e, index) {

        var count = 0;
        var selectedItemCode = "";
        selectedItemCode = e.dataItem.ITEM_CODE;
        $scope.tablelist[index].MU_CODE = e.dataItem.INDEX_MU_CODE;
        if ($scope.tablelist.length == 1) {
            if (selectedItemCode == $scope.tablelist[0].ITEM_CODE) {

            }
            else {
                $($('.amendment')[0]).prop("disabled", false).addClass("showborder");
                $scope.tablelist[0].EDITED = 'Y';
            }
        }
        else if ($scope.tablelist.length > 1) {
            for (i = 0; i < $scope.tablelist.length; i++) {
                if (selectedItemCode == $scope.tablelist[i].ITEM_CODE) {
                    count++;
                    if (count == 1) {
                        $($('.amendment')[index]).prop("disabled", true).removeClass("showborder");
                        $scope.tablelist[i].ITEM_CODE = "";
                        $scope.tablelist[index].ITEM_EDESC = "";
                        displayPopupNotification("Item Already Exist.", "warning");
                        return;
                    }
                }
                else {
                    $($('.amendment')[index]).prop("disabled", false).addClass("showborder");
                }
                $scope.tablelist[index].EDITED = 'Y';
            }

        }


        //$scope.$apply();
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

    $scope.issues = [{
        sn: 1,
        name: '',
        quantity: '',
        value: '',
        total: '',
        hscode: '',
        country: ''
    }]

    $scope.AddLc = function () {
        $scope.showFormPanel = true;
        $scope.lc.calenderdays = 'DAY';
        $scope.cleareditcontent();
        $scope.resettotal();
    }

    $scope.OnQuantityChange = function (i) {

        if (temporarytableitems[i].quantity != $scope.tablelist[i].QUANTITY) {
            if (i < $scope.itemlist.length) {
                $($('.amendment')[i]).prop("disabled", false).addClass("showborder");
                $scope.tablelist[i].AMTEDITED = 'Y';
            }
        }

    }

    $scope.OnAmountChange = function (i) {

        if (temporarytableitems[i].value != $scope.tablelist[i].AMOUNT) {
            if (i < $scope.itemlist.length) {
                $($('.amendment')[i]).prop("disabled", false).addClass("showborder");
                $scope.tablelist[i].AMTEDITED = 'Y';
            }
        }
    }

    $scope.createlc = function (isValid, options) {

        $scope.flag = true;
        var trackno = $scope.lctrack;
        var lccode = $scope.lccode;


        if (isValid) {

            if (trackno == undefined || trackno == 0) {
                if ($scope.itemlist.length == 0) {

                    $.each($scope.tablelist, function (value, index) {

                        if ($scope.tablelist[value].ITEM_CODE.length == 0 || $scope.tablelist[value].QUANTITY == "" || $scope.tablelist[value].AMOUNT == "") {
                            displayPopupNotification("Please choose valid items.", "error");
                            $scope.flag = false;
                            return false;
                        }

                    });
                    if ($scope.flag == false) {
                        return;
                    }
                }

                $.each($scope.tablelist, function (key, val) {
                    if (val.EDITED == "Y") {
                        val.ITEM_CODE = val.ITEM_EDESC;
                    }
                });
                showloader();

            }

            var pientryurl = window.location.protocol + "//" + window.location.host + "/api/LCEntry/createLcEntry";
            var beneficiarynames = $('#beneficiaryname').data("kendoDropDownList").text();
            if (beneficiarynames == "--Select Beneficiary--") {
                beneficiarynames = "";
            }

            var pidetails = {
                PINVOICE_CODE: $scope.pinvoicecodeforlc,
                LC_NUMBER: $scope.lc.number,
                LC_TRACK_NO: trackno,
                LOC_CODE: lccode,
                LEAD_TIME: $scope.lc.daynumber + " " + $scope.lc.calenderdays,
                LC_TRACK: $scope.lcetracknumberforlc,
                OPEN_DATE: $scope.lc.opendate,
                EXPIRY_DATE: $scope.lc.expirydate,
                EXPIRY_PLACE: $scope.lc.expiryplace,
                STATUS_CODE: $scope.lc.lcstatus,
                LAST_SHIPMENT_DATE: $scope.lc.lastshipmentdate,
                TOLERANCE_PER: $scope.lc.toleranceper,
                TERMS_CODE: $scope.lc.incoterms,
                ADVISING_BANK_CODE: $scope.lc.advising,
                CONFIRM_BANK_CODE: $scope.lc.confirming,
                ISSUING_BANK_CODE: $scope.lc.issuing,
                PTERMS_CODE: $scope.lc.pterms,
                CREDIT_DAYS: $scope.lc.creditdays,
                PARTIAL_SHIPMENT: $scope.psradio.radio,
                CONFIRMATION_REQ: $scope.confirmationradio.radio,
                TRANSFERABLE: $scope.transferradio.radio,
                INSURANCE_FLAG: $scope.insuranceradio.radio,
                BEF_OUT_CHARGE: $scope.chargeradio.radio,
                APP_OUT_CHARGE: $scope.chargeradio.radio,
                BNF_CONFIRM_CHARGE: $scope.confirmradio.radio,
                APP_CONFIRM_CHARGE: $scope.confirmradio.radio,
                DOC_REQ_DAYS: $scope.lc.docreqdays,
                ORIGIN_COUNTRY_CODE: $scope.lc.origincountrycode,
                REMARKS: $scope.lc.remarks,
                Itemlist: $scope.tablelist
            };

            var response = $http({
                method: "post",
                url: pientryurl,
                data: pidetails,
                dataType: "json"
            });

            return response.then(function (data) {
                if (data.data.PINVOICE_CODE != undefined) {
                    if (options == "save") {
                        $scope.showFormPanel = false;
                    }
                    if (data.data != null) {

                        $('#lctrackno').val(data.data.LC_TRACK_NO);
                        $('#loccode').val(data.data.LOC_CODE);
                    }
                    $scope.cancellc();
                    myDropzone.processQueue();
                    $('#grid').data("kendoGrid").dataSource.read();
                    hideloader();
                    displayPopupNotification("Letter Of Credit Updated Successfully.", "success");

                }
                else {
                    displayPopupNotification(data.data, "warning");
                    hideloader();
                }


            }, function errorCallback(response) {
                if (response.status == "304") {
                    displayPopupNotification("Lc Number Already Exist.", "warning");
                }
                else {
                    displayPopupNotification("Error Occured", "error");
                }
                hideloader();
            });
        }
        else {
            displayPopupNotification("Fields cannot be empty.", "error");

        }


    }

    $scope.lc = {
        perfomainvoice: '',
        purchaseorder: '',
        number: '',
        opendate: '',
        expirydate: '',
        validitydate: '',
        estdeliverydate: '',
        lastshipmentdate: '',
        expiryplace: '',
        advising: '',
        creditdays: 0,
        confirming: '',
        toleranceper: '',
        docreqdays: '',
        remarks: '',
        calenderdays: 'DAY',
        leadtime: '',
    };

    $scope.cancellc = function () {

        $scope.lc = {};
        $scope.chargeradio = {
            radio: 'A'
        };
        $scope.confirmradio = {
            radio: 'B'
        };
        $scope.psradio = {
            radio: 'Y'
        };
        $scope.confirmationradio = {
            radio: 'N'
        };
        $scope.transferradio = {
            radio: 'Y'
        };
        $scope.profomainvoicedisabled = false;
        $scope.lctrack = 0;
        $scope.loccode = 0;
        $('#beneficiaryname').data("kendoDropDownList").value("");
        $('#incoterms').data("kendoDropDownList").value("");
        $('#pterms').data("kendoDropDownList").value("");
        $('#lcstatus').data("kendoDropDownList").value("");
        $('#issuing').data("kendoDropDownList").value("");
        $('#origincountrycode').data("kendoDropDownList").value("");
        $scope.saveAction = "Save";
        $scope.scAction = "Save & Continue";
        $scope.ShowShipmentForm = true;
        $scope.noitems = true;
        $scope.itemlist = [];
        $scope.issues[0] = {};
        $scope.hidefirstrow = true;
        $scope.ItemsShow = true;
        $scope.showFormPanel = false;
        $scope.lc.opendate = '';
        $scope.lc.expirydate = '';
        //$scope.lc.validitydate = '';
        $scope.lc.estdeliverydate = '';
        $scope.lc.lastshipmentdate = '';
        $('.image-placeholder').html("");
        $scope.lccode = 0;
        $("#grid").data("kendoGrid").tbody.find("tr").removeClass('highlight');
        $scope.ContentModal = true;
        $scope.disablelcnumber = false;
    }

    $scope.ipPurchaseOrder = {
        dataType: "json",
        serverFiltering: true,
        filter: "startswith",
        transport: {
            read: function (options) {

                $.ajax({
                    url: "/api/LCEntry/getAllLcIpPurchaseOrder?filter=" + options.data.filter.filters[0].value,
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

    $scope.lcfromsynergy = {
        type: "json",
        serverFiltering: true,
        suggest: true,
        highlightFirst: true,
        transport: {
            read: {
                url: "/api/LCEntry/GetAllSupplierByFilter",

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

    $scope.addIssue = function () {
        var data = $scope.allitemslist;
        if ($scope.hidefirstrow) {
            $scope.hidefirstrow = false;
            $scope.noitems = false;
        } else {
            var i = $scope.issues.length;
            var available = $scope.issues;
        }
        $scope.issues = [];

        $scope.issues.push({
            sn: i + 1,
            name: '',
            quantity: '',
            value: '',
            total: '',
            hscode: $scope.hslist,
            country: '',
        });
        for (var i = 0; i < available.length; i++) {
            var item = available[i];
            $scope.issues.push(item);
        }
    }

    $scope.removeIssue = function (index) {
        if (index == 0 && $scope.issues.length == 1) {
            $scope.noitems = true;
            $scope.hidefirstrow = true;
            $scope.issues[0] = {};
        }
        else {
            $scope.issues.splice(index, 1);
            for (var i = 0; i < $scope.issues.length; i++) {
                $scope.issues[i].sn = $scope.issues.length - i;
            }
        }
        $scope.tablelist.splice(index, 1);
        $scope.resettotal();
    }

    $scope.LoadtermsDay = function () {
        var paymentterms = $scope.lc.paymentterm;
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


    $scope.ProformaInvoiceOnChange = function (e) {


        $scope.lc.purchaseorder = "";
        showloader();
        var response = $http({
            method: "get",
            url: "/api/LCEntry/getItemDetailsByOrderCode?PinvoiceCode=" + e.dataItem.PINVOICE_NO,
            dataType: "json"
        });
        return response.then(function (data) {

            if (data.data.length == 0) {

                $scope.itemlist = [];
            }
            else {
                $scope.itemlist = [];
                $scope.lc.validitydate = "";

                if (data.data.ORDER_NO != null) {
                    $scope.lc.purchaseorder = data.data.ORDER_NO;
                    var values = data.data.LEAD_TIME.split(" ");
                    $scope.lc.daynumber = parseInt(values[0]);
                    $scope.lc.calenderdays = values[1];

                }
                if (data.data.BNF_NAME != null) {
                    $('#beneficiaryname').data("kendoDropDownList").text(data.data.BNF_NAME);
                    $scope.lc.beneficiaryname = data.data.BNF_CODE;;
                }
                else {
                    var beneficiarydatasource = $('#beneficiaryname').data("kendoDropDownList");
                    beneficiarydatasource.value("");
                    beneficiarydatasource.dataSource.read();
                    $scope.lc.beneficiaryname = [];
                }
                if (data.data.TERMS_CODE != null) {
                    $scope.lc.incoterms = data.data.TERMS_CODE;
                    $scope.lc.terms = data.data.TERMS_CODE;
                }
                else {
                    $scope.lc.incoterms = '';
                    $scope.lc.terms = '';
                }

                if (data.data.PTERMS_CODE != null) {

                    $scope.lc.pterms = data.data.PTERMS_CODE;
                    $scope.lc.paymentterm = data.data.PTERMS_CODE;
                    $('#pterms').data("kendoDropDownList").value(data.data.PTERMS_CODE);
                    var selectedpterms = $('#pterms').data("kendoDropDownList").text();
                    if (selectedpterms == "LC At Sight") {
                        $scope.creditdaysreadonly = true;
                        $scope.lc.creditdays = 0;

                    }
                    else {
                        $scope.creditdaysreadonly = false;
                    }

                }
                else {
                    $scope.lc.pterms = '';
                    $scope.lc.paymentterm = '';
                    $scope.creditdaysreadonly = false;
                }

                $scope.lc.creditdays = data.data.CREDIT_DAYS;
                if (data.data.VALIDITY_DATE != null) {
                    $scope.lc.validitydate = $filter('date')(data.data.VALIDITY_DATE, "MM/dd/yyyy");
                }

                if (data.data.EST_DELIVERY_DATE != null) {
                    $scope.lc.estdeliverydate = $filter('date')(data.data.EST_DELIVERY_DATE, "MM/dd/yyyy");
                }


                $scope.itemlist = data.data.Itemlist;
                $scope.Total_Quantity = 0;
                $scope.Total_Amount = 0;
                $scope.tablelist = [];
                for (i = 0; i < $scope.itemlist.length; i++) {
                    $scope.tablelist.push({
                        ITEM_CODE: $scope.itemlist[i].ITEM_CODE,
                        ITEM_EDESC: $scope.itemlist[i].ITEM_EDESC,
                        QUANTITY: $scope.itemlist[i].QUANTITY,
                        AMOUNT: $scope.itemlist[i].AMOUNT,
                        VALUE: '',
                        HS_CODE: $scope.itemlist[i].HS_CODE,
                        MU_CODE: $scope.itemlist[i].MU_CODE,
                        COUNTRY_OF_ORIGIN: $scope.itemlist[i].COUNTRY_OF_ORIGIN,
                        REMARKS: '',
                        ITEMEDITED: '',
                        AMTEDITED: '',
                        AMENDMENT: '',
                        SNO: $scope.itemlist[i].SNO
                    });

                    $scope.Total_Quantity = parseInt(parseInt($scope.Total_Quantity) + parseInt($scope.itemlist[i].QUANTITY));
                    $scope.Total_Amount = (parseFloat(parseFloat($scope.Total_Amount) + ($scope.itemlist[i].QUANTITY * parseFloat($scope.itemlist[i].AMOUNT)))).toFixed(2);




                }
                $scope.lcetracknumberforlc = data.data.LC_TRACK_NO;
                $scope.pinvoicecodeforlc = data.data.PINVOICE_CODE;
                temporarytableitems = [];
                temporarytableitems = $.extend(true, {}, $scope.tablelist);

            }
            hideloader();
        }, function errorCallback(response) {
            displayPopupNotification("Error Occured.", "error");
        });
    }



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

    $scope.resettotal = function () {
        $scope.Total_Quantity = 0;
        $scope.Total_Rate = 0;
        $scope.Total_Amount = 0;
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

    };

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
        change: onPTermsChange,
    };

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

    $scope.lcStatusDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/LCEntry/GetAllStatusByFilter?filter" + "",
            },

        },
    };

    $scope.lcstatus = {
        optionLabel: "--Select Status--",

        dataTextField: "StatusName",
        dataValueField: "StatusCode",
        dataSource: $scope.lcStatusDatasource,

    };

    $scope.picountrylist = {
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

    $scope.hscodelist = {
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

    $scope.countryDatasource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/ProformaInvoice/getCountryCodes",
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

    $scope.countrylist = {
        optionLabel: "--Select Country--",
        filter: "contains",
        dataTextField: "COUNTRY_EDESC",
        dataValueField: "COUNTRY_CODE",
        dataSource: $scope.countryDatasource,
    };

    $scope.bankDatasource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/LCEntry/GetAllBanksByFilter",
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

    $scope.bankOptions = {
        optionLabel: "--Select Banks--",
        filter: "contains",
        dataTextField: "BANK_NAME",
        dataValueField: "BANK_CODE",
        dataSource: $scope.bankDatasource,

    };

    $scope.supplierbankDatasource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/LCEntry/GetAllSupplierBanksByFilter",
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

    $scope.supplierbankOptions = {
        optionLabel: "--Select Supplier Banks--",
        filter: "contains",
        dataTextField: "BANK_NAME",
        dataValueField: "BANK_CODE",
        dataSource: $scope.supplierbankDatasource,

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


    $scope.getImage = function (mylist, ATTACH_DOC) {

        if (mylist != null) {
            var img = "";
            var il = "";
            for (var i = 0; i < mylist.length; i++) {

                img = img + "<img src='" + mylist[i] + "' height=20 />";
                var extension = mylist[i].substr((mylist[i].lastIndexOf('.') + 1));
                extension = extension.toLowerCase();

                //$('.image').data('src')
                switch (extension) {
                    case 'jpg':
                    case 'jpeg':
                        il = il + "<a target='_blank' href='" + mylist[i] + "'><i class='fa fa-file-image-o image'/></i></a>"
                        break;
                    case 'png':
                        il = il + "<a target='_blank' href='" + mylist[i] + "' ><i class='fa fa-file-image-o image' /></i></a>"
                        break;
                    case 'pdf':
                        il = il + "<a target='_blank' href='" + mylist[i] + "'><i class='fa fa-file-pdf-o image' /></i></a>"
                        break;
                    case 'docx':
                        il = il + "<a target='_self' href='" + mylist[i] + "' download><i class='fa fa-file-word-o image' /></i></a>"
                        break;
                    case 'xls':
                    case 'xlsx':
                        il = il + "<a target='_self' href='" + mylist[i] + "' download><i class='fa fa-file-excel-o image' /></i></a>"
                        break;
                    default:
                        il = il + "<a target='_self' href='" + mylist[i] + "' download><i class='fa fa-file-text-o image' /></i></a>"
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
                    il = "<a target='_blank' href='" + ATTACH_DOC + "'><i class='fa fa-file-image-o'/></i></a>"
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
                    il = "<a target='_self' href='" + ATTACH_DOC + "' download><i class='fa fa-file-text-o' /></i></a>"
            }
            return $sce.trustAsHtml(il);
        }
        else {
            il = "";
            return il;
        }
    }

    $scope.FilterGrid = function (LOC_CODE) {
        $("#grid").data("kendoGrid").tbody.find("tr").css("background-color", "");
        var data = $("#grid").data("kendoGrid").dataSource.data();
        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
            if (dataItem.LOC_CODE == LOC_CODE) {
                $("#grid").data("kendoGrid").tbody.find("tr[data-uid=" + dataItem.uid + "]").css("background-color", "#80ddaa");
            }
        }

    };



    $scope.disableeditcontent = function () {
        $scope.check_lcstatus = false;
        $scope.check_opendate = false;
        $scope.check_expirydate = false;
        $scope.check_lastshipmentdate = false;
        $scope.check_terms = false;
        $scope.check_paymentterm = false;
        $scope.check_creditdays = false;


        $scope.check_leadtime = false;
        $scope.check_expiryplace = false;
        $scope.check_toleranceper = false;
        $scope.check_docreqdays = false;

        $scope.check_advising = false;
        $scope.check_confirming = false;
        $scope.check_issuing = false;

        $scope.check_remarks = false;
        $scope.check_origincountrycode = false;
        $scope.check_validitydate = false;

    };


    $scope.cleareditcontent = function () {
        $scope.check_lcstatus = true;
        $scope.check_opendate = true;
        $scope.check_expirydate = true;
        $scope.check_lastshipmentdate = true;
        $scope.check_terms = true;
        $scope.check_paymentterm = true;
        $scope.check_creditdays = true;


        $scope.check_leadtime = true;
        $scope.check_expiryplace = true;
        $scope.check_toleranceper = true;
        $scope.check_docreqdays = true;

        $scope.check_advising = true;
        $scope.check_confirming = true;
        $scope.check_issuing = true;

        $scope.check_remarks = true;
        $scope.check_origincountrycode = true;
        $scope.check_validitydate = true;


    };

    $scope.AllcheckboxEnableDisable = function () {
        if ($scope.Allcheckbox) {
            $scope.cleareditcontent();
        } else {
            $scope.disableeditcontent();
        }
    }

    $scope.mainGridOptions = {
        toolbar: ["excel"],
        dataSource: {
            type: "json",
            transport: {
                read: "/api/LCEntry/GetAllLcList",
            },
            pageSize: 100,
            serverPaging: false,
            serverSorting: false,
            schema: {
                model: {
                    fields: {
                        LC_TRACK_NO: { type: "number" },
                        LC_NUMBER: { type: "string" },
                        EXPIRY_DATE: { type: "date" },
                        LAST_SHIPMENT_DATE: { type: "date" },
                        OPEN_DATE: { type: "date" },
                        CREDIT_DAYS: { type: "number" },
                        SUPPLIER_EDESC: { type: "string" },
                        mylist: { type: "" },
                        APPROVED_BY: { type: "string" }
                    }
                }
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
        columnMenuInit: function (e) {
            wordwrapmenu(e);
        },
        dataBound: function () {
            //this.expandRow(this.tbody.find("tr.k-master-row").first());
            $scope.detailExportPromises = [];
            $scope.EditPO = function (dataItem) {
                $scope.saveAction = "Update";
                $scope.profomainvoicedisabled = true;
                $scope.ItemsShow = false;
                $('#lccode').val(dataItem.LOC_CODE);
                $scope.scAction = "Update & Continue";
                $scope.showFormPanel = true;
                $scope.ContentModal = false;
                //$scope.disablelcnumber = true;
                $scope.disablelcnumber = false;
                $scope.disableeditcontent();
                $scope.Allcheckbox = false;
                $scope.lc.perfomainvoice = dataItem.PINVOICE_NO;
                $scope.lc.purchaseorder = dataItem.ORDER_NO;
                $scope.lctrack = dataItem.LC_TRACK_NO;
                $scope.lccode = dataItem.LOC_CODE;
                $scope.FilterGrid(dataItem.LOC_CODE);
                $scope.lc.daynumber = dataItem.LEAD_TIME == null ? "" : parseInt((dataItem.LEAD_TIME).split(" ")[0]);
                $scope.lc.calenderdays = dataItem.LEAD_TIME == null ? "" : (dataItem.LEAD_TIME).split(" ")[1];
                $scope.lc.number = dataItem.LC_NUMBER;
                $('#beneficiaryname').data("kendoDropDownList").text(dataItem.BNF_NAME)
                $scope.lc.opendate = $filter('date')(dataItem.OPEN_DATE, "MM/dd/yyyy");
                $scope.lc.expirydate = $filter('date')(dataItem.EXPIRY_DATE, "MM/dd/yyyy");
                $scope.lc.incoterms = dataItem.TERMS_CODE;
                $scope.lc.terms = dataItem.TERMS_CODE;
                $scope.lc.pterms = dataItem.PTERMS_CODE;
                $scope.lc.paymentterm = dataItem.PTERMS_CODE;
                if (dataItem.PTERMS_EDESC == "LC At Sight") {
                    $scope.creditdaysreadonly = true;
                    $scope.lc.creditdays = 0;
                }
                else {
                    $scope.creditdaysreadonly = "";
                }
                $scope.lc.creditdays = dataItem.CREDIT_DAYS;
                $scope.lc.expiryplace = dataItem.EXPIRY_PLACE;
                $scope.lc.advising = dataItem.ADVISING_BANK_CODE;
                $scope.lc.confirming = dataItem.CONFIRM_BANK_CODE;
                $scope.lc.issuing = dataItem.ISSUING_BANK_CODE;
                $scope.lc.lcstatus = dataItem.STATUS_CODE;
                $scope.lc.statuses = dataItem.STATUS_CODE;
                $scope.lc.lastshipmentdate = $filter('date')(dataItem.LAST_SHIPMENT_DATE, "MM/dd/yyyy");
                $scope.lc.toleranceper = dataItem.TOLERANCE_PER;
                $scope.lc.origincountrycode = dataItem.ORIGIN_COUNTRY_CODE;
                $scope.lc.issuing = dataItem.ISSUING_BANK_CODE;
                $scope.lc.docreqdays = dataItem.DOC_REQ_DAYS;
                $scope.lc.remarks = dataItem.REMARKS;

                $scope.confirmradio = {
                    radio: dataItem.APP_CONFIRM_CHARGE
                };
                $scope.psradio = {
                    radio: dataItem.PARTIAL_SHIPMENT
                };
                $scope.confirmationradio = {
                    radio: dataItem.CONFIRMATION_REQ
                };
                $scope.transferradio = {
                    radio: dataItem.TRANSFERABLE
                };
                $scope.insuranceradio = {
                    radio: dataItem.INSURANCE_FLAG
                };

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
                            case 'jpeg':
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
                        fileName: "Letter Of Credit.xlsx"
                    });
                });
        },
        columns: [{ field: "LC_TRACK_NO", title: "Track No", attributes: { style: "text-align:right;" }, width: 110 },
        { field: "LC_NUMBER", title: "Lc Number", template: "#= SUPPLIER_EDESC == null?LC_NUMBER:SUPPLIER_EDESC #", width: 120 },

        {
            field: "OPEN_DATE",
            title: "Open Date",
            template: "#= OPEN_DATE == null ? '' :kendo.toString(kendo.parseDate(OPEN_DATE),'M/dd/yyyy') #",
            width: 110
        },

        {
            field: "EXPIRY_DATE",
            title: "Expiry Date",
            template: "#= EXPIRY_DATE == null ? '' :kendo.toString(kendo.parseDate(EXPIRY_DATE),'M/dd/yyyy') #",
            width: 110
        },

        {
            field: "LAST_SHIPMENT_DATE",
            title: "Last Shipment Date",
            template: "#= LAST_SHIPMENT_DATE == null ? '' :kendo.toString(kendo.parseDate(LAST_SHIPMENT_DATE),'M/dd/yyyy') #",

        },
        {
            field: "TERMS_EDESC",
            title: "Terms Name",

        }, {
            field: "PTERMS_EDESC",
            title: "Payment Terms",
        },

        {
            field: "CREDIT_DAYS",
            title: "Credit Days",

            attributes: { style: "text-align:right;" }
        },
        {
            field: "STATUS_EDESC",
            title: "Status",


        },
        {
            field: "APPROVED_BY",
            title: "Approve Status",


        },

        {
            field: "mylist",
            title: "Images",
            template: '<span ng-bind-html= "getImage(dataItem.mylist, dataItem.FILE_DETAIL)"> </span>',


        },
        {
            field: "ID", title: "Action", sortable: false, filterable: false,
            template: '<a class="approved glyphicon glyphicon-ok" title="Approve" ng-click="Approve(dataItem)" style="color:grey;"><span class="sr-only"></span> </a><a class="edit glyphicon glyphicon-edit" title="Edit" ng-click="EditPO(dataItem)" style="color:grey;"><span class="sr-only"></span> </a> <a class="status glyphicon glyphicon-check" title="Check Status" ng-click="CheckStatus(dataItem)" style="color:grey;"><span class="sr-only"></span> </a><a class="pring glyphicon glyphicon-print" title="Print Preview" ng-click="PrintPreview(dataItem)" style="color:grey;"><span class="sr-only"></span> </a> <a class="fa fa-history" ng-click="HistoryPO(dataItem)" title="History" style="color:grey;"><span class="sr-only"></span> </a>'
        }
        ]
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
                //{
                //field: "LC_TRACK_NO"
                //},
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
                    { field: "QUANTITY", title: "Quantity", attributes: { style: "text-align:right;" }, width: "20px" },
                    { field: "AMOUNT", title: "Rate", attributes: { style: "text-align:right;" }, width: "20px" },
                    { field: "", title: "Total Amount", template: '#= QUANTITY * AMOUNT #', attributes: { style: "text-align:right;" }, width: "20px" },
                    { field: "CURRENCY_CODE", title: "Currency", width: "25px" },

                    { field: "COUNTRY_OF_ORIGIN", title: "Country", width: "30px" },
                    {
                        field: "CREATED_DATE", title: "Created Date", width: "35px", format: "{0:M/dd/yyyy}",
                        template: "#= CREATED_DATE == null ? '' :kendo.toString(kendo.parseDate(CREATED_DATE),'M/dd/yyyy') #",
                    },
                    { field: "CREATED_BY_EDESC", title: "Created By", width: "40px" },

                    {
                        field: "LAST_MODIFIED_DATE", title: "Updated date", width: "35px", format: "{0:M/dd/yyyy}",
                        template: "#= LAST_MODIFIED_DATE ==null ? '' : kendo.toString(kendo.parseDate(LAST_MODIFIED_DATE),'M/dd/yyyy') #",
                    },
                    { field: "LAST_MODIFIED_BY_EDESC", title: "Updated By", width: "40px" },
                    { field: "REMARKS", title: "Amendment Reason", width: "60px" },
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

                // editor: MUCodeAutoCompleteEditor
            };
        }
        $scope.historydocumentGridOptions();
        $('#historymodal').modal('show');
    }



    $scope.Approve = function (dataItem) {
        $('#approvemodal').modal('show');
        $scope.saveApproval = function () {

            var isApproved = $scope.isApproved;
            var lcNumber = dataItem.LC_NUMBER;
            var trackNumber = dataItem.LC_TRACK_NO;
            // var amount = dataItem.AMOUNT;
            if (isApproved === true) {
                var response = $http({
                    method: "post",
                    url: "/api/LCEntry/insertIntoMasterTransaction?lcNumber=" + lcNumber + "&trackNumber=" + trackNumber,
                    dataType: "json"
                });
                return response.then(function (data) {
                    $('#grid').data("kendoGrid").dataSource.read();
                    $('#approvemodal').modal('hide');
                    $scope.isApproved = false;
                    displayPopupNotification("Successfully Approved.", "success");
                }, function errorCallback(response) {
                    displayPopupNotification("Error Occured.", "error");
                });
            }

        }

    }
    $scope.CancelApproval = function () {
        $('#approvemodal').modal('hide');
        $scope.isApproved = false;
    };


    $scope.CheckStatus = function (dataItem) {

        $('#statusmodal').modal('show');
        $scope.lctracknumberforstatus = dataItem.LC_TRACK_NO;
        $scope.status = dataItem.STATUS_CODE;
    }
    $scope.CancelStatus = function () {
        $('#statusmodal').modal('hide');
    }

    $scope.saveStatus = function () {

        var status = $scope.status;
        $scope.lctracknumberforstatus;
        var response = $http({
            method: "post",
            url: "/api/LCEntry/saveStatus?status=" + status + "&lctrack=" + $scope.lctracknumberforstatus,
            dataType: "json"
        });
        return response.then(function (data) {
            $('#grid').data("kendoGrid").dataSource.read();
            $('#statusmodal').modal('hide');
            $scope.status = '';
            displayPopupNotification("Successfully Updated Status.", "success");
        }, function errorCallback(response) {
            displayPopupNotification("Error Occured.", "error");
        });

    }

    $scope.PrintPreview = function (dataItem) {

        var lctrack = dataItem.LC_TRACK_NO;

        var response = $http({
            method: "get",
            url: "/api/LCEntry/lcDetailsList?tracknumber=" + lctrack,
            dataType: "json"
        });
        return response.then(function (data) {

            $('#printmodal').modal('show');
            $scope.LC_TRACK_NO = data.data.LC_TRACK_NO;
            $scope.ORDER_NO = data.data.ORDER_NO;
            $scope.CURRENCY_CODE = data.data.CURRENCY_CODE;
            $scope.MANUAL_NUMBER = data.data.MANUAL_NUMBER;
            $scope.VALIDITY_DATE = $filter('date')(data.data.VALIDITY_DATE, "MM/dd/yyyy");
            $scope.EST_DELIVERY_DATE = $filter('date')(data.data.EST_DELIVERY_DATE, "MM/dd/yyyy");
            $scope.DELIVERY_PLACE_TYPE = data.data.DELIVERY_PLACE_TYPE;
            $scope.DELIVERY_PLACE = data.data.DELIVERY_PLACE;
            $scope.APP_NAME = data.data.APP_NAME;
            $scope.APP_ADDRESS = data.data.APP_ADDRESS;
            $scope.BILL_COMP_NAME = data.data.BILL_COMPANY_NAME;
            $scope.BILL_COMP_ADD = data.data.BILL_COMPANY_ADD;
            $scope.BILL_COMPANY_PHONE = data.data.APP_ADDRESS;
            $scope.SHIP_COMPANY_NAME = data.data.SHIP_COMPANY_NAME;
            $scope.SHIP_COMPANY_ADD = data.data.SHIP_COMPANY_ADD;
            $scope.SHIP_COMPANY_PHONE = data.data.SHIP_COMPANY_PHONE;
            $scope.TRANSSHIPMENT = data.data.TRANSSHIPMENT;
            $scope.CONTACT_NAME = data.data.CONTACT_NAME;
            $scope.CONTACT_PHONE = data.data.CONTACT_PHONE;
            $scope.CONTACT_EMAIL = data.data.CONTACT_EMAIL;
            $scope.TERMS_EDESC = data.data.TERMS_EDESC;
            $scope.PTERMS_EDESC = data.data.PTERMS_EDESC;
            $scope.BNF_EDESC = data.data.BNF_EDESC;
            $scope.BNF_ADDRESS = data.data.ADDRESS;
            $scope.PINVOICE_NO = data.data.PINVOICE_NO;
            $scope.BANK_NAME = data.data.BANK_NAME;
            $scope.BANK_BRANCH = data.data.BANK_BRANCH;
            $scope.SWIFT_CODE = data.data.SWIFT_CODE;
            $scope.INTM_BANK_CODE = data.data.INTM_BANK_EDESC;
            $scope.INTM_SWIFT_CODE = data.data.INTM_SWIFT_CODE;
            $scope.ACCEPTED_DOC_DATE = $filter('date')(data.data.ACCEPTED_DOC_DATE, "MM/dd/yyyy");
            $scope.REMARKS = data.data.REMARKS;
            $scope.LC_NUMBER = data.data.LC_NUMBER;
            $scope.OPEN_DATE = $filter('date')(data.data.OPEN_DATE, "MM/dd/yyyy");
            $scope.EXPIRY_DATE = $filter('date')(data.data.EXPIRY_DATE, "MM/dd/yyyy");
            $scope.EXPIRY_PLACE = data.data.EXPIRY_PLACE;
            $scope.STATUS_EDESC = data.data.STATUS_EDESC;
            $scope.LAST_SHIPMENT_DATE = $filter('date')(data.data.LAST_SHIPMENT_DATE, "MM/dd/yyyy");
            $scope.TOLERANCE_PER = data.data.TOLERANCE_PER;
            $scope.ADVISING_BANK_EDESC = data.data.ADVISING_BANK_EDESC;
            $scope.CONFIRM_BANK_EDESC = data.data.CONFIRM_BANK_EDESC;
            $scope.ISSUING_BANK_EDESC = data.data.ISSUING_BANK_EDESC;
            $scope.LC_TERMS_EDESC = data.data.LC_TERMS_EDESC;
            $scope.LC_PTERMS_EDESC = data.data.LC_PTERMS_EDESC;
            $scope.PARTIALSHIPMENT = data.data.PARTIAL_SHIPMENT;
            $scope.CONFIRM_REQ = data.data.CONFIRMATION_REQ;
            $scope.TRANSFERABLE = data.data.TRANSFERABLE;
            $scope.INSURANCE_FLAG = data.data.INSURANCE_FLAG;
            if (data.data.APP_OUT_CHARGE == "A") {
                $scope.OUT_CHARGE = "Applicant";
            }
            else {
                $scope.OUT_CHARGE = "Beneficiary";
            }
            if (data.data.BNF_CONFIRM_CHARGE == "A") {
                $scope.CONFIRM_CHARGE = "Applicant";
            }
            else {
                $scope.CONFIRM_CHARGE = "Beneficiary";
            }
            $scope.DOC_REQ_DAYS = data.data.DOC_REQ_DAYS;
            $scope.ORIGIN_COUNTRY = data.data.ORIGIN_COUNTRY_CODE;

            $scope.FROM_LOCATION = data.data.FROM_LOCATION;
            $scope.TO_LOCATION = data.data.TO_LOCATION;
            $scope.SHIPMENT_TYPE = data.data.SHIPMENT_TYPE;
            $scope.LOAD_TYPE = data.data.LOAD_TYPE;

        }, function errorCallback(response) {
            displayPopupNotification("Error Occured.", "error");
        });
    }

    $scope.printDiv = function (divName) {

        var printContents = document.getElementById(divName).innerHTML;

        var popupWin = window.open('', '_blank', 'width=800,height=800', 'orientation = landscape');
        popupWin.ScreenOrientatio = "Landscape";
        popupWin.document.open();
        popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="style.css" /></head><body onload="window.print()">' + printContents + '</body></html>');
        popupWin.document.close();
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
                        }
                    },
                    parameterMap: function (options, operation) {


                        if (operation !== "read" && options.models) {

                            var data = options.models;
                            //if (options.models[0].COUNTRY_EDESC != null) {
                            //    if (options.models[0].COUNTRY_EDESC.length > 3) {
                            //        options.models[0].COUNTRY_EDESC = options.models[0].COUNTRY_OF_ORIGIN;
                            //    }
                            //    else {
                            //        data[0]["COUNTRY_OF_ORIGIN"] = countrycode;
                            //    }
                            //}

                            if (data[0].SNO === undefined) {
                                //insert case
                                data[0]["COUNTRY_CODE"] = (data[0].COUNTRY_CODE.COUNTRY_CODE === undefined) ? data[0].COUNTRY_CODE : data[0].COUNTRY_CODE.COUNTRY_CODE;
                                data[0]["HS_CODE"] = data[0].HS_CODE;
                            }
                            else {
                                //update case

                                //if both blank
                                if (data[0].COUNTRY_CODE.COUNTRY_CODE !== undefined && data[0].HS_CODE.HS_CODE !== undefined) {
                                    data[0]["COUNTRY_CODE"] = data[0].COUNTRY_CODE.COUNTRY_CODE;
                                    data[0]["HS_CODE"] = data[0].HS_CODE.HS_CODE;
                                }

                                //if both not blank
                                if (data[0].COUNTRY_CODE !== undefined && data[0].HS_CODE !== undefined) {
                                    if (!data[0].hasOwnProperty("HS_CODE")) {
                                        data[0]["COUNTRY_CODE"] = data[0].COUNTRY_CODE;
                                        data[0]["HS_CODE"] = data[0].HS_CODE;
                                    }

                                }

                                //if both CC blank only
                                if (data[0].COUNTRY_CODE.COUNTRY_CODE !== undefined && data[0].HS_CODE !== undefined) {
                                    data[0]["COUNTRY_CODE"] = data[0].COUNTRY_CODE.COUNTRY_CODE;
                                    data[0]["HS_CODE"] = data[0].HS_CODE;
                                }

                                //if both HS blank only
                                if (data[0].COUNTRY_CODE !== undefined && data[0].HS_CODE.HS_CODE !== undefined) {
                                    data[0]["COUNTRY_CODE"] = data[0].COUNTRY_CODE;
                                    data[0]["HS_CODE"] = data[0].HS_CODE.HS_CODE;
                                }

                            }


                            data[0]["LC_TRACK_NO"] = dataItem.LC_TRACK_NO;
                            if (itemcode == "") {
                                data[0]["ITEM_CODE"] = data[0].ITEM_CODE;
                            }
                            else {
                                data[0]["ITEM_CODE"] = itemcode;
                            }
                            $scope.itemcodeformucode = ($scope.itemcodeformucode == undefined || $scope.itemcodeformucode == "") ? null : $scope.itemcodeformucode
                            if ($scope.itemcodeformucode == null) {
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
                            ITEM_EDESC: { validation: { required: true } },
                            MU_CODE: { validation: { required: true } },
                            AMOUNT: { type: "number", validation: { min: 1, required: true } },
                            QUANTITY: { type: "number", validation: { min: 1, required: true } },
                            HS_CODE: { validation: { required: true } },
                            COUNTRY_CODE: { validation: { required: true } },
                        }
                    }
                },
                //filter: {
                //    field: "LC_TRACK_NO",
                //    operator: "eq",
                //    value: dataItem.LC_TRACK_NO
                //}
                //filter: { field: "LC_TRACK_NO", operator: "eq", value: dataItem.LC_TRACK_NO }
            },
            scrollable: false,
            sortable: true,
            pageable: true,
            resizable: true,
            dataBinding: function () {
                record = (this.dataSource.page() - 1) * this.dataSource.pageSize();
            },
            toolbar: ["create"],

            navigatable: true,
            columns: [
                //{ title: "SN", template: "#= ++record #", width: 30, attributes: { style: "text-align:right;" } },
                { field: "ITEM_EDESC", title: "Item Name", width: "110px", editor: ItemsNameAutoCompleteEditor },
                { field: "MU_CODE", title: "Master Unit", width: "30px", editor: MUCodeAutoCompleteEditor },
                { field: "QUANTITY", title: "Quantity", attributes: { style: "text-align:right;" }, width: "50px" },
                { field: "AMOUNT", title: "Rate", attributes: { style: "text-align:right;" }, width: "50px" },
                { field: "", title: "Total Amount", template: '#= QUANTITY * AMOUNT #', attributes: { style: "text-align:right;" }, width: "50px" },
                { field: "HS_CODE", title: "Hs Code", width: "50px", editor: HsCodeAutoCompleteEditor },
                { field: "COUNTRY_CODE", title: "Country", width: "80px", editor: CountryAutoCompleteEditor },
                { command: ["edit"], title: "Action", width: "70px" },

            ],
            editable: "inline",
            // editor: MUCodeAutoCompleteEditor
        };
    };

    function ItemsNameAutoCompleteEditor(container, options) {
        $('<input id="ItemNameAutoComplete" required data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoAutoComplete({
                dataSource: $scope.allitemslist,
                dataTextField: "ITEM_EDESC",
                dataValueField: "ITEM_CODE",
                filter: "contains",
                change: function (e) {

                    var itemnameautocomplete = $("#ItemNameAutoComplete").data("kendoAutoComplete");
                    var datas = itemnameautocomplete.dataItem();
                    itemcode = datas.ITEM_CODE;
                    mucode = datas.INDEX_MU_CODE;
                    var mucodeAutoComplete = $('#MuCodeAutoComplete').data("kendoAutoComplete");
                    mucodeAutoComplete.value(mucode);
                    $scope.itemcodeformucode = itemcode;
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

        $('<input id="MuCodeAutoComplete" disabled required data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoAutoComplete({
                dataSource: $scope.allMuCodelist,
                dataTextField: "MU_CODE",
                dataValueField: "MU_CODE",
                filter: "contains",
                minLength: 1,

            });
    }


    function HsCodeAutoCompleteEditor(container, options) {
        $('<input  required data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                dataSource: $scope.hslist,
                dataTextField: "HS_CODE",
                dataValueField: "HS_CODE",
                filter: "contains",
                minLength: 1,
            });
    }

    function CountryAutoCompleteEditor(container, options) {
        $('<input required id="CountryNameAutoComplete" data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                dataSource: $scope.picountrylist,
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
    }


});

app.service('LCEntryService', function ($http) {
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

