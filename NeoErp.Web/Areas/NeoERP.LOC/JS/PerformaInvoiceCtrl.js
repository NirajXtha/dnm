app.controller('PerformaInvoiceCtrl', function ($scope, $compile, $http, $filter, $q, $sce, PerformaInvoiceService) {
    $scope.ItemsOnChange = "";
    $scope.itemlist = [];
    $scope.currencycode = "";
    $scope.countrylist = [];
    $scope.hslist = [];
    $scope.beneficiarylist = [];
    $scope.shipmentType = [];
    $scope.shipmentLoad = [];
    $scope.intmbankOptions = [];
    $scope.deliveryPlaceType = [];
    $scope.issues = [];
    $scope.saveAction = "Save";
    $scope.hidefirstrow = true;
    $scope.noitems = true;
    $scope.ItemsShow = true;
    $scope.allitemslist = [];
    $scope.scAction = "Save & Continue";
    $scope.ContentModal = true;
    var temporarytableitems = [];
    $scope.tablelist = [];
    $scope.totalItems = [];
    $scope.hideonedit = true;
    $scope.checkitemlistvalid = 0;
    $scope.filterData = {
        purchaseOrder:"",
        pinvoiceno: "",
        pinvoicedate: $filter('date')(new Date, "MM-dd-yyyy"),
    };



    $scope.disableeditcontent = function () {
        $scope.check_pinvoiceno = false;
        $scope.check_pinvoicedate = false;
        $scope.check_currencycode = false;
        $scope.check_docdate = false;
        $scope.check_beneficiaryname = false;
        $scope.check_beneficiaryaddress = false;
        $scope.check_intmbankcode = false;
        $scope.check_intmswiftcode = false;

        $scope.check_bankcode = false;
        $scope.check_bankbranch = false;
        $scope.check_swiftcode = false;

        $scope.check_remarks = false;
    };


    $scope.cleareditcontent = function () {
        $scope.check_pinvoiceno = true;
        $scope.check_pinvoicedate = true;
        $scope.check_currencycode = true;
        $scope.check_docdate = true;
        $scope.check_beneficiaryname = true;
        $scope.check_beneficiaryaddress = true;
        $scope.check_intmbankcode = true;
        $scope.check_intmswiftcode = true;

        $scope.check_bankcode = true;
        $scope.check_bankbranch = true;
        $scope.check_swiftcode = true;

        $scope.check_remarks = true;

    };

    $scope.AllcheckboxEnableDisable = function () {
        if ($scope.Allcheckbox) {
            $scope.cleareditcontent();
        } else {
            $scope.disableeditcontent();
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

    $scope.pi = {
        purchaseOrder: '',
        pinvoiceno: '',
        pinvoicedate:'',
        docdate: '',
        beneficiaryname: 0,
        beneficiaryaddress: '',
        intmbankcode: '',
        intmswiftcode: '',
        bankcode: '',
        bankbranch: '',
        swiftcode: '',
        remarks: '',
    };

    $scope.AddPerformaInvoice = function () {
        $scope.showFormPanel = true;
        $scope.onedit = false;
        $scope.hideonedit = true;
        $scope.cleareditcontent();
        $scope.resettotal();
    }

    $scope.cancelpi = function () {
        $scope.pi = {};
        $scope.tablelist = [];
        $scope.pi.pinvoicedate = '',
            $scope.pi.docdate = '',
            $scope.lctrack = 0;
        $scope.pocode = 0;

        $scope.saveAction = "Save";
        $scope.scAction = "Save & Continue";
        $scope.noitems = true;
        $scope.itemlist = [];

        $scope.issues = [];
        $scope.hidefirstrow = true;
        $scope.ItemsShow = true;
        $scope.showFormPanel = false;
        $('.image-placeholder').html("");
        $("#grid").data("kendoGrid").tbody.find("tr").removeClass('highlight');
        $scope.ContentModal = true;
    }

    $scope.clearonItemChange = function () {
        $scope.tablelist = [];
        $scope.issues = [];
        $scope.itemlist = [];
    }

    $scope.createperfomainvoice = function (isValid, options) {
     
        $scope.flag = true;
        var trackno = $scope.lctrack;
        var pinvoice = $scope.pincode;
        if (isValid) {

            if (trackno == undefined || trackno == 0) {
                if ($scope.itemlist.length == 0) {

                    $.each($scope.tablelist, function (value, index) {

                        if ($scope.tablelist[value].ITEM_CODE.length == 0 || $scope.tablelist[value].QUANTITY == "" || $scope.tablelist[value].AMOUNT == "") {
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
          
            var pientryurl = window.location.protocol + "//" + window.location.host + "/api/ProformaInvoice/CreatePerfomaInvoice";
            var pidetails = {
                PINVOICE_NO: $scope.pi.pinvoiceno,
                PINVOICE_DATE: $scope.pi.pinvoicedate,
                ORDER_NO: $scope.pi.purchaseOrder,
                LC_TRACK_NO: trackno,
                LC_TRACK: $scope.lctracknumber,
                CURRENCY_CODE: $scope.pi.currencycode,
                PINVOICE_CODE: pinvoice,
                ACCEPTED_DOC_DATE: $scope.pi.docdate,
                BNF_CODE: $scope.pi.beneficiaryname,
                BNF_ADDRESS: $scope.pi.beneficiaryaddress,
                INTM_BANK_CODE: $scope.pi.intmbankcode,
                INTM_SWIFT_CODE: $scope.pi.intmswiftcode,
                BANK_CODE: $scope.pi.bankcode,
                BANK_BRANCH: $scope.pi.bankbranch,
                SWIFT_CODE: $scope.pi.swiftcode,
                REMARKS: $scope.pi.remarks,
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
                        $('#pinvoicecode').val(data.data.PINVOICE_CODE);
                    }

                    $scope.cancelpi();
                    myDropzone.processQueue();
                    $('#grid').data("kendoGrid").dataSource.read();
                    displayPopupNotification("Perfoma Invoice Updated Successfully.", "success");
                    hideloader();
                }
                else {
                    displayPopupNotification(data.data, "warning");
                    hideloader();
                }
            }, function errorCallback(response) {


                if (response.status == '500') {
                    displayPopupNotification(response.data, "warning");
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

    $scope.ipPurchaseOrder = {
        dataType: "json",
        serverFiltering: true,
        filter: "startswith",
        transport: {
            read: function (options) {

                $.ajax({
                    url: "/api/ProformaInvoice/getAllLcIpPurchaseOrder?filter=" + options.data.filter.filters[0].value,
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
                    url: "/api/ProformaInvoice/getAllIpPurchaseOrderfilter?filter=" + options.data.filter.filters[0].value,
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

   $scope.ipPurchaseInvoicefilter = {
        dataType: "json",
        serverFiltering: true,
        filter: "startswith",
        transport: {
            read: function (options) {
                $.ajax({
                    url: "/api/ProformaInvoice/getAllIpipPurchaseInvoicefilter?filter=" + options.data.filter.filters[0].value,
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

    $scope.hscodeListTemp = function () {
        var response = $http({
            method: "get",
            url: "/api/ProformaInvoice/getHsCodes?filter=" + "",
            dataType: "json",

        });

        return response.then(function (result) {

            $scope.hscodeListTemp = result.data;
            $scope.hscodeslist = {
                optionLabel: "--Select Hs Code--",
                filter: "contains",
                dataTextField: "HS_CODE",
                dataValueField: "HS_CODE",
                dataSource: $scope.hscodeListTemp,
               
            };


        }, function errorCallback(response) {
            displayPopupNotification("Fields should not be empty.", "error");
        });
    }

    $scope.countryListTemp = function () {
        var response = $http({
            method: "get",
            url: "/api/ProformaInvoice/getCountryCodes?filter=" + "",
            dataType: "json",

        });

        return response.then(function (result) {

            $scope.countryListTemp = result.data;
            $scope.countrylist = {
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

    $scope.countryListTemp();
    $scope.hscodeListTemp();

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
                EDITED: 'N',
            });

        });
        $scope.$apply();
    }

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

    $scope.ItemsOnChange = function (e) {

        $scope.itemlist = [];
        $scope.clearonItemChange();
        showloader();

        var response = $http({
            method: "get",
            url: "/api/ProformaInvoice/getItemDetailsByTrackOrderNo?OrderCode=" + e.dataItem.ORDER_NO,
            dataType: "json"
        });
        return response.then(function (data) {
         
            if (data.data.length == 0) {
               
                $scope.itemlist = [];
            }
            else {

                if (data.data[0].LC_TRACK_NO != null) {
                    $scope.lctracknumber = data.data[0].LC_TRACK_NO;
                }
                if (data.data[0].CURRENCY_CODE != null) {
                    $scope.pi.currencycode = data.data[0].CURRENCY_CODE;
                    $scope.currencycode = data.data[0].CURRENCY_CODE;
                }
                if (data.data[0].BNF_BANK_CODE != null) {
                    $scope.pi.beneficiaryname = data.data[0].BNF_BANK_CODE;
                    $scope.pi.beneficiaryaddress = data.data[0].ADDRESS;
                }
                if ($scope.issues.length > 0) {
                    $scope.issues = {};
                }

                $scope.itemlist = data.data;
                $scope.Total_Quantity = 0;
                $scope.Total_Amount = 0;
                for (i = 0; i < $scope.itemlist.length; i++) {
                    $scope.tablelist.push({
                        ITEM_CODE: $scope.itemlist[i].ITEM_CODE,
                        ITEM_EDESC: $scope.itemlist[i].ITEM_EDESC,
                        QUANTITY: $scope.itemlist[i].CALC_QUANTITY,
                        AMOUNT: $scope.itemlist[i].CALC_UNIT_PRICE,
                        VALUE: '',
                        HS_CODE: $scope.itemlist[i].HS_CODE,
                        MU_CODE: $scope.itemlist[i].MU_CODE,
                        COUNTRY_OF_ORIGIN: $scope.itemlist[i].COUNTRY_CODE,
                        EDITED: '',
                        ADDED: '',
                        CURRENCY_CODE: $scope.currencycode,
                    });
                  
                    $scope.Total_Quantity = parseInt(parseInt($scope.Total_Quantity) + parseInt($scope.itemlist[i].CALC_QUANTITY));
                    $scope.Total_Amount = (parseFloat(parseFloat($scope.Total_Amount) + ($scope.itemlist[i].CALC_QUANTITY * parseFloat($scope.itemlist[i].CALC_UNIT_PRICE)))).toFixed(2);

                }

            }
            temporarytableitems = [];
            temporarytableitems = $.extend(true, {}, $scope.tablelist);
            hideloader();
        }, function errorCallback(response) {
            displayPopupNotification("Fields should not be empty.", "error");
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

    $scope.deliveryPlaceTypeDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/LcEntry/GetDeliveryPlaceType",
            }
        },
    };

    $scope.deliveryPlaceType = {
        optionLabel: "--Select DeliveryPlace Type--",
        dataSource: $scope.deliveryPlaceTypeDatasource,
    };

    $scope.intmbankDatasource = {
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

    $scope.intmbankOptions = {
        optionLabel: "--Select Banks--",
        filter: "contains",
        dataTextField: "BANK_NAME",
        dataValueField: "BANK_CODE",
        dataSource: $scope.intmbankDatasource,
        change: onIntmBankChange,
    };

    function onIntmBankChange(e) {

        var IntmBankSwiftCode = "";
        var intmbankcode = $('#intmbanks').data("kendoDropDownList").dataItem().BANK_CODE;
        var datasource = $('#intmbanks').data("kendoDropDownList").dataSource.data();
        $.each(datasource, function (i, e) {
            if (e.BANK_CODE == intmbankcode) {
                IntmBankSwiftCode = e.SWIFT_CODE;
                $scope.pi.intmswiftcode = IntmBankSwiftCode;
            }
        })
        $('#intmswiftcode').val(IntmBankSwiftCode);
    }

    $scope.bankDatasource = {
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

    $scope.bankOptions = {
        optionLabel: "--Select Banks--",
        filter: "contains",
        dataTextField: "BANK_NAME",
        dataValueField: "BANK_CODE",
        dataSource: $scope.bankDatasource,
        change: onBankChange,
    };

    function onBankChange(e) {

        var Bankbranch = "";
        var BankSwiftCode = "";
        var bankcode = $('#banks').data("kendoDropDownList").dataItem().BANK_CODE;
        var datasource = $('#banks').data("kendoDropDownList").dataSource.data();
        $.each(datasource, function (i, e) {
            if (e.BANK_CODE == bankcode) {
                Bankbranch = e.BRANCH;
                BankSwiftCode = e.SWIFT_CODE;
                $scope.pi.bankbranch = Bankbranch;
                $scope.pi.swiftcode = BankSwiftCode;
            }
        })
        $('#bankbranch').val(Bankbranch);
        $('#swiftcode').val(BankSwiftCode);
        //$('#beneficiaryaddress').val(Address);
    }


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
        change: $scope.onChange,
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
        change: $scope.onChange,
    };

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
        dataTextField: "COUNTRY_CODE",
        dataValueField: "COUNTRY_CODE",
        dataSource: $scope.countryDatasource,
    };

    $scope.hslistDatasource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/ProformaInvoice/getHsCodes",
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

    $scope.hslist = {
        optionLabel: "--Select HS Code--",
        filter: "contains",
        dataTextField: "HS_CODE",
        dataValueField: "HS_CODE",
        dataSource: $scope.hslistDatasource,
    };

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
        change: onbnfChange,
    };

    function onbnfChange(e) {

        var Address = "";
        var benefcode = $('#benefeciaryname').data("kendoDropDownList").dataItem().BNF_CODE;
        var datasource = $('#benefeciaryname').data("kendoDropDownList").dataSource.data();
        $.each(datasource, function (i, e) {
            if (e.BNF_CODE == benefcode) {
                Address = e.ADDRESS;
                $scope.pi.beneficiaryaddress = Address;
            }
        })
        $('#beneficiaryaddress').val(Address);

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

    $scope.addIssue = function () {
        $scope.checkitemlistvalid = 1;
        $scope.issues;
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
            ADDED: 'Y',
            EDITED: '',
            CURRENCY_CODE: $scope.currencycode,

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
            $scope.issues.splice(currentIndex, 1);
        }
        $scope.tablelist.splice(index, 1);
        if ($scope.tablelist.length == 0) {
            $scope.noitems = true;
        }
        $scope.showtotal();
    }

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


   
    $scope.FilterGrid = function (PINVOICE_CODE) {
        $("#grid").data("kendoGrid").tbody.find("tr").css("background-color", "");
        var data = $("#grid").data("kendoGrid").dataSource.data();
        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
            if (dataItem.PINVOICE_CODE == PINVOICE_CODE) {
                $("#grid").data("kendoGrid").tbody.find("tr[data-uid=" + dataItem.uid + "]").css("background-color", "#80ddaa");
            }
        }

    };


    $scope.mainGridOptions = {
        toolbar: ["excel"],
        dataSource: {
            type: "json",
            transport: {
                read: "/api/ProformaInvoice/getAllPerfomaInvoice",
            },
          pageSize: 100,
            serverPaging: false,
            serverSorting: false,
            schema: {
                model: {
                    fields: {
                        PINVOICE_DATE: { type: "date" },
                        EST_DELIVERY_DATE: { type: "date" },
                        LC_TRACK_NO: { type: "number" },
                        ACCEPTED_DOC_DATE: { type: "date" },
                        mylist: {type:""}
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
        columnMenuInit: function (e) {
            wordwrapmenu(e);
        },
        dataBound: function () {
            $scope.detailExportPromises = [];
            $scope.EditPI = function (dataItem) {
               
                $scope.hideonedit = false;
                $scope.onedit = true;
                $scope.saveAction = "Update";
                $scope.ItemsShow = false;
                $scope.scAction = "Update & Continue";
                $scope.disableeditcontent();
                $scope.Allcheckbox = false;
                $scope.ContentModal = false;
                $scope.showFormPanel = true;
                $scope.pinvoicecode = dataItem.PINVOICE_CODE;
                $scope.pincode = dataItem.PINVOICE_CODE;
                $scope.FilterGrid(dataItem.PINVOICE_CODE);
                $scope.pi.currencycode = dataItem.CURRENCY_CODE;
                $scope.lctrack = dataItem.LC_TRACK_NO;
                $scope.pi.suppliercode = dataItem.SUPPLIER_CODE;
                $scope.pi.purchaseOrder = dataItem.ORDER_NO;
                $scope.pi.pinvoiceno = dataItem.PINVOICE_NO;
                $scope.pi.pinvoicedate = $filter('date')(dataItem.PINVOICE_DATE, "MM/dd/yyyy");
                $scope.pi.docdate = $filter('date')(dataItem.ACCEPTED_DOC_DATE, "MM/dd/yyyy");
                $scope.pi.beneficiaryname = dataItem.BNF_CODE;
                $scope.pi.beneficiaryaddress = dataItem.BNF_ADDRESS;
                $scope.pi.intmbankcode = dataItem.INTM_BANK_CODE;
                $scope.pi.intmswiftcode = dataItem.INTM_SWIFT_CODE;
                $scope.pi.bankcode = dataItem.BANK_CODE;
                $scope.pi.bankbranch = dataItem.BANK_BRANCH;
                $scope.pi.swiftcode = dataItem.SWIFT_CODE;
                $scope.pi.remarks = dataItem.REMARKS;
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
                        fileName: "PerfomaInvoice.xlsx"
                    });
                });
        },
        columns: [{
            field: "LC_TRACK_NO",
            title: "Track No",
            attributes: { style: "text-align:right;" },
        },
        {
            field: "PINVOICE_NO",
            title: "Pinvoice No",
            width: 120
        },
        {
            field: "ORDER_NO",
            title: "Order No",
            width: 150
        }
            , {
            field: "PINVOICE_DATE",
            title: "Pinvoice Date",

            template: "#= kendo.toString(kendo.parseDate(PINVOICE_DATE),'M/dd/yyyy') #",

        },
        {
            field: "BNF_NAME",
            title: "Beneficiary Name",

        },
        {
            field: "BANK_NAME",
            title: "Bank Name",

        },
        {
            field: "INTM_BANK_EDESC",
            title: "Intermediate Bank",

        },
        {
            field: "ACCEPTED_DOC_DATE",
            title: "Accepted Date",

            template: "#= ACCEPTED_DOC_DATE == null ? '' :kendo.toString(kendo.parseDate(ACCEPTED_DOC_DATE),'M/dd/yyyy') #",

        },
        {
            field: "mylist",
            title: "Images",
            template: '<span ng-bind-html= "getImage(dataItem.mylist, dataItem.FILE_DETAIL)"> </span>',

        },
        {
            field: "ID", title: "Action", sortable: false, filterable: false,
            template: '<a class="edit glyphicon glyphicon-edit" ng-click="EditPI(dataItem)" style="color:grey;"><span class="sr-only"></span> </a> <a class="fa fa-history" ng-click="HistoryPO(dataItem)" title="History" style="color:grey;"><span class="sr-only"></span> </a>'
        }
        ]
    };

    $scope.exportChildData = function (lctrackno, rowIndex) {
     
        var deferred = $.Deferred();

        $scope.detailExportPromises.push(deferred);

        var rows = [{
            cells: [
                // First cell
                //{ value: "LC_TRACK_NO" },
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
                    field: "AMOUNT", title: "Rate",
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
                  //  { field: "HS_CODE", title: "Hs Code", width: "30px" },
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
                            $.each($(".shipmentgrid"), function (i, value) {

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
                            $.each($(".shipmentgrid"), function (i, value) {

                                $(value).data("kendoGrid").dataSource.read();
                            });
                        }

                    },
                    parameterMap: function (options, operation) {


                        if (operation !== "read" && options.models) {
                            var data = options.models;
                         

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
                            if ($scope.itemcodeformucode == undefined || $scope.itemcodeformucode == "") {
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
               
            },
            scrollable: false,
            sortable: true,
            pageable: true,
            resizable: true,
            dataBinding: function () {
                record = (this.dataSource.page() - 1) * this.dataSource.pageSize();
            },
            toolbar: ["create"],
            excelExport: function (e) {
                // prevent saving the file
                e.preventDefault();

                
            },
            navigatable: true,
            columns: [
               
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

        };
    };

    function MUCodeAutoCompleteEditor(container, options) {

        $('<input id="MuCodeAutoComplete" required disabled data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoAutoComplete({
                dataSource: $scope.allMuCodelist,
                dataTextField: "MU_CODE",
                dataValueField: "MU_CODE",
                filter: "contains",
                minLength: 1,

            });
    }

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

    function HsCodeAutoCompleteEditor(container, options) {
        $('<input required data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                dataSource: $scope.hscodelist,
                dataTextField: "HS_CODE",
                dataValueField: "HS_CODE",
                filter: "contains",
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


app.service('PerformaInvoiceService', ['$http', '$q', '$timeout', function ($http, $q, $timeout) {
    var fac = {};

}]);

