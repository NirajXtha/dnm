//LOC Commercial Invoice Entry 
app.controller('CommercialInvoiceCtrl', function ($scope, $http, $compile, $filter, $q, $sce, $timeout, CommercialInvoiceService) {
    $scope.ReloadCode = "";
    $scope.tempinvoicecode = 0;
    $scope.items = [];
    $scope.lcnumber = "";
    $scope.lctrackno = "";
    $scope.containeritem = false;
    $scope.editon = false;
    $scope.LogisticContainerPlan = [];
    $scope.Lots_LogisticContainerPlan = [];
    $scope.CommercialInvoiceArray = [];
    $scope.disableci = true;
    $scope.ShowCarrierPanel = false;
    $scope.filterData = {
        lcnumber: ""
    };


    var d1 = "";

    function init() {
        $scope.showFormPanel = false;
        $scope.ShowOnEditForm = false;
        $scope.saveAction = "Save";
        $scope.scAction = "Save & Continue";
        $scope.itemlist = [];
        $scope.showitems = false;
        $scope.showreloaditems = false;
        $scope.disableinvoicedate = false;
        $scope.disableppdate = false;
        $scope.disablecppnumber = false;
        $scope.disableexchangerate = false;
        $scope.disablecurrency = false;
        $scope.disableairwaybilldate = false;
        $scope.disableairwaybillno = false;
        $('#invoiceNumber').val("");
        $scope.disablelcnumber = false;
        $scope.templctrackno = 0;
        $('.image-placeholder').html("");


        $scope.LogisticContainerPlan
        [{
            SNO: "",
            SHIPPING_TYPE: "",
            FROM_LOCATION_CODE: "",
            TO_LOCATION_CODE: "",
            EST_BOOKING_DATE: "",
            EST_LOADING_DATE: "",
            ACT_BOOKING_DATE: "",
            ACT_LOADING_DATE: "",
            TO_LOCATION_EDESC: "",
            FROM_LOCATION_EDESC: ""
        }];
        $scope.tableitems = [{
            ITEM_CODE: '',
            SNO: '',
            QUANTITY: 0,
            AMOUNT: 0,
            VALUE: 0,
            HS_CODE: '',
            COUNTRY_CODE: '',
            MU_CODE: '',
            RECV_QUANTITY: '',
            INPUT_QUANTITY: '',
            LOT_NO: ''
        }];


        $scope.ci = [{
            lcnumber: '',
            cinvoicedate: '',
            cinvoiceno: '',
            cppdate: '',
            cppnumber: '',
            airwaybilldate: '',
            airwaybillno: '',
            lotno: ''
        }];
        $scope.ci.isair = false;

        $scope.DO = [];

        $scope.DO = [{
            LC_DO_CODE:"",
            LC_TRACK_NO: "",
            INVOICE_CODE: "",
            ITEM_CODE: "",
            DO_NUMBER: "",
            QUANTITY: "",
            REMARKS: ""
        }];

        $scope.DODataItem = {
            LC_TRACK_NO: "",
            INVOICE_CODE: "",
            ITEM_CODE: "",
        };
    }

    $scope.Lots_LogisticContainerPlan =
        {
            lcnumber: '',
            cinvoicedate: '',
            cinvoiceno: '',
            cppdate: '',
            cppnumber: '',
            airwaybilldate: '',
            airwaybillno: ''
        };
    init();



    //Group LogisticContainerPlan
    var indexedLot = [];
    $scope.done = false;
    $scope.LotToFilter = function () {

        indexedLot = [];
        if ($scope.LogisticContainerPlan.length > 0) {
            $scope.Lot = _.uniq($scope.LogisticContainerPlan, "LOT_NO");
            var len = $scope.Lot.length;
         
            if (!$scope.done) {
                for (var i = 0; i < len; i++) {
                    $scope.CommercialInvoiceArray.push({ "item": $scope.Lots_LogisticContainerPlan }, { "LOT_NO": $scope.Lot[i].LOT_NO });
                };
            }

            $scope.done = true;
            $scope.$apply;
        }
        return $scope.LogisticContainerPlan;
    };

    $scope.filterLot = function (player) {
        var IsNew = indexedLot.indexOf(player.LOT_NO) == -1;
        if (IsNew) {
            indexedLot.push(player.LOT_NO);
        }
        return IsNew;
    }
    //end



    $scope.AddCommercialInvoice = function () {
        $scope.showFormPanel = true;
        $scope.disableinvoicedate = false;
        $scope.disableppdate = false;
        $scope.disablecppnumber = false;
        $scope.disableexchangerate = false;
        $scope.disablecurrency = false;
        $scope.disableairwaybilldate = false;
        $scope.disableairwaybillno = false;
        $scope.disablelcnumber = false;
        $scope.resettotal();
    };

    function CommercialInvoice(lcnumber) {
       
        var response = CommercialInvoiceService.ItemsOnChange(lcnumber);
        return response.then(function (data) {
         
            if (data.data.length == 0) {
                $scope.itemlist = [];

            }
            else {
                $scope.tableitems = [{
                    ITEM_CODE: '',
                    SNO: '',
                    QUANTITY: '',
                    AMOUNT: '',
                    VALUE: '',
                    HS_CODE: '',
                    COUNTRY_CODE: '',
                    MU_CODE: '',
                    RECV_QUANTITY: '',
                    INPUT_QUANTITY: '',
                    LOT_NO: ''
                }];
             
                $scope.itemlist = data.data;
                $scope.items = angular.copy($scope.itemlist);

                $scope.lctracknocommercial = data.data[0].LC_TRACK_NO;

                for (i = 0; i < $scope.itemlist.length; i++) {

                    if (i == 0) {
                        $scope.tableitems[i].ITEM_CODE = $scope.itemlist[i].ITEM_CODE;
                        $scope.tableitems[i].SNO = $scope.itemlist[i].SNO;
                        $scope.tableitems[i].QUANTITY = $scope.itemlist[i].CALC_QUANTITY;
                        $scope.tableitems[i].AMOUNT = $scope.itemlist[i].CALC_UNIT_PRICE;
                        $scope.tableitems[i].MU_CODE = $scope.itemlist[i].MU_CODE;
                        $scope.tableitems[i].HS_CODE = $scope.itemlist[i].HS_CODE;
                        $scope.tableitems[i].COUNTRY_CODE = $scope.itemlist[i].COUNTRY_CODE;
                        $scope.tableitems[i].RECV_QUANITTY = $scope.itemlist[i].INVOICE_QUANTITY;
                        $scope.tableitems[i].LOT_NO = $scope.itemlist[i].LOT_NO;
                    }
                    else {
                        $scope.tableitems.push({
                            ITEM_CODE: $scope.itemlist[i].ITEM_CODE,
                            SNO: $scope.itemlist[i].SNO,
                            QUANTITY: $scope.itemlist[i].CALC_QUANTITY,
                            AMOUNT: $scope.itemlist[i].CALC_UNIT_PRICE,
                            MU_CODE: $scope.itemlist[i].MU_CODE,
                            HS_CODE: $scope.itemlist[i].HS_CODE,
                            COUNTRY_CODE: $scope.itemlist[i].COUNTRY_CODE,
                            RECV_QUANITTY: $scope.itemlist[i].INVOICE_QUANTITY,
                            LOT_NO: $scope.itemlist[i].LOT_NO,
                        });
                        $scope.tableitems[i].ITEM_CODE = $scope.itemlist[i].ITEM_CODE;
                        $scope.tableitems[i].SNO = $scope.itemlist[i].SNO;
                        $scope.tableitems[i].QUANTITY = $scope.itemlist[i].CALC_QUANTITY;
                        $scope.tableitems[i].AMOUNT = $scope.itemlist[i].CALC_UNIT_PRICE;
                        $scope.tableitems[i].MU_CODE = $scope.itemlist[i].MU_CODE;
                        $scope.tableitems[i].HS_CODE = $scope.itemlist[i].HS_CODE;
                        $scope.tableitems[i].COUNTRY_CODE = $scope.itemlist[i].COUNTRY_CODE;
                        $scope.tableitems[i].RECV_QUANITTY = $scope.itemlist[i].INVOICE_QUANTITY;
                        $scope.tableitems[i].LOT_NO = $scope.itemlist[i].LOT_NO;
                    }
                    var savecount = 0;
                    var tableitemlength = $scope.tableitems.length;
                    angular.forEach($scope.tableitems, function (value, index) {


                        if (value.QUANTITY == value.RECV_QUANITTY) {
                            ++savecount;
                        }

                    });
                    if (savecount == tableitemlength) {

                        $scope.disablesave = true;
                    } else {
                        $scope.disablesave = false;
                    }
                }

            }
        }, function errorCallback(response) {
            displayPopupNotification("Error Occured.", "error");
        });
    };
    $scope.ItemsOnChange = function (e) {
        debugger;
        $scope.showitems = true;
        $scope.ShowCarrierPanel = true;
        $scope.ShowOnEditForm = false;
        $scope.disableci = false;
        var lcnumber = e.dataItem.LC_NUMBER_CODE;
        var LC_NUMBER_DESC = e.dataItem.LC_NUMBER;
        $scope.ReloadCode = lcnumber;
        lcnumber = (lcnumber == undefined) ? null : lcnumber;
        showloader();
        $scope.lcnumber = lcnumber;
        $scope.LC_NUMBER_DESC = LC_NUMBER_DESC;
        lcnumber = escape(lcnumber);
        //CommercialInvoice(lcnumber);
        GetLogisticPlan(lcnumber); //lot_no
    
    
        hideloader();
        //  $(".go").css('background-color', 'white');
    }


    function LoadCIBylcnumber(lcnumber) {
        var response = CommercialInvoiceService.LoadCIBylcnumber(lcnumber);
        return response.then(function (data) {
            if (data.data.length > 0) {
                $scope.Lots_LogisticContainerPlan =
                    {
                        lcnumber: '',
                        cinvoicedate: '',
                        cinvoiceno: '',
                        cppdate: '',
                        cppnumber: '',
                        airwaybilldate: '',
                        airwaybillno: ''
                    };
            }
        });
    };


    $scope.removeContainer = function (index) {
        if ($scope.LC_PLAN_CONTAINER_LIST.length>1) {
            $scope.LC_PLAN_CONTAINER_LIST.splice(index, 1);
        }
        
         
    }


    function GetLogisticPlan(lcnumber) {
        debugger;
        var response = CommercialInvoiceService.GetLogisticPlan(lcnumber);
        return response.then(function (data) {
            if (data.data.LC_PLAN_CONTAINER_LIST.length == 0) {
                $scope.LC_PLAN_CONTAINER_LIST = [];
                $scope.showitems = false;
               //   displayPopupNotification("No Carrrier Data Found.", "warning");
               // $scope.disablesave = true;
            }
            //else {
            //    $scope.disablesave = false;
            //}
            $scope.LogisticContainerPlan = [];
            if (data.data.LC_LOGISTIC_PLAN_CONTAINER.length != 0) {
                $scope.LogisticContainerPlan = data.data.LC_LOGISTIC_PLAN_CONTAINER;
            } 
               
            $scope.LC_PLAN_CONTAINER_LIST = data.data.LC_PLAN_CONTAINER_LIST;
                
            $scope.tableitems = [{
                ITEM_CODE: '',
                SNO: '',
                QUANTITY: '',
                AMOUNT: '',
                VALUE: '',
                HS_CODE: '',
                COUNTRY_CODE: '',
                MU_CODE: '',
                RECV_QUANTITY: '',
                INPUT_QUANTITY: '',
                SHIPPMENT_QUANTITY: '',
                EXCHANGE_RATE: '',
                SALES_EXG_RATE: '',
                PAYMENT_DATE:'',
                LOT_NO: ''
            }];

            $scope.itemlist = data.data.ItemDetails;
            $scope.items = angular.copy($scope.itemlist);
            if (data.data.ItemDetails.length>0) {
                $scope.lctracknocommercial = data.data.ItemDetails[0].LC_TRACK_NO;
            }
            debugger;
            $scope.Total_Quantity = 0;
            $scope.Total_Amount = 0;
            $scope.Shippment_Total_Amount = 0;
            for (i = 0; i < $scope.itemlist.length; i++) {

                if (i == 0) {
                    $scope.tableitems[i].ITEM_CODE = $scope.itemlist[i].ITEM_CODE;
                    $scope.tableitems[i].SNO = $scope.itemlist[i].SNO;
                    $scope.tableitems[i].QUANTITY = $scope.itemlist[i].CALC_QUANTITY;
                    $scope.tableitems[i].AMOUNT = $scope.itemlist[i].CALC_UNIT_PRICE;
                    $scope.tableitems[i].MU_CODE = $scope.itemlist[i].MU_CODE;
                    $scope.tableitems[i].HS_CODE = $scope.itemlist[i].HS_CODE;
                    $scope.tableitems[i].COUNTRY_CODE = $scope.itemlist[i].COUNTRY_CODE;
                    $scope.tableitems[i].RECV_QUANITTY = $scope.itemlist[i].INVOICE_QUANTITY;
                    $scope.tableitems[i].SHIPPMENT_QUANTITY = $scope.itemlist[i].SHIPPMENT_QUANTITY;
                    $scope.tableitems[i].EXCHANGE_RATE = $scope.itemlist[i].EXCHANGE_RATE;
                    $scope.tableitems[i].SALES_EXG_RATE = $scope.itemlist[i].SALES_EXG_RATE;
                    $scope.tableitems[i].PAYMENT_DATE = $scope.itemlist[i].PAYMENT_DATE;
                    
                    $scope.tableitems[i].LOT_NO = $scope.itemlist[i].LOT_NO;
               
                }
                else {
                    $scope.tableitems.push({
                        ITEM_CODE: $scope.itemlist[i].ITEM_CODE,
                        SNO: $scope.itemlist[i].SNO,
                        QUANTITY: $scope.itemlist[i].CALC_QUANTITY,
                        AMOUNT: $scope.itemlist[i].CALC_UNIT_PRICE,
                        MU_CODE: $scope.itemlist[i].MU_CODE,
                        HS_CODE: $scope.itemlist[i].HS_CODE,
                        COUNTRY_CODE: $scope.itemlist[i].COUNTRY_CODE,
                        RECV_QUANITTY: $scope.itemlist[i].INVOICE_QUANTITY,
                        SHIPPMENT_QUANTITY: $scope.itemlist[i].SHIPPMENT_QUANTITY,
                        EXCHANGE_RATE: $scope.itemlist[i].EXCHANGE_RATE,
                        SALES_EXG_RATE: $scope.itemlist[i].SALES_EXG_RATE,
                        PAYMENT_DATE: $scope.itemlist[i].PAYMENT_DATE,
                        
                        LOT_NO: $scope.itemlist[i].LOT_NO
                    });
                    $scope.tableitems[i].ITEM_CODE = $scope.itemlist[i].ITEM_CODE;
                    $scope.tableitems[i].SNO = $scope.itemlist[i].SNO;
                    $scope.tableitems[i].QUANTITY = $scope.itemlist[i].CALC_QUANTITY;
                    $scope.tableitems[i].AMOUNT = $scope.itemlist[i].CALC_UNIT_PRICE;
                    $scope.tableitems[i].MU_CODE = $scope.itemlist[i].MU_CODE;
                    $scope.tableitems[i].HS_CODE = $scope.itemlist[i].HS_CODE;
                    $scope.tableitems[i].COUNTRY_CODE = $scope.itemlist[i].COUNTRY_CODE;
                    $scope.tableitems[i].RECV_QUANITTY = $scope.itemlist[i].INVOICE_QUANTITY;
                    $scope.tableitems[i].SHIPPMENT_QUANTITY = $scope.itemlist[i].SHIPPMENT_QUANTITY;
                    $scope.tableitems[i].EXCHANGE_RATE = $scope.itemlist[i].EXCHANGE_RATE;
                    $scope.tableitems[i].SALES_EXG_RATE = $scope.itemlist[i].SALES_EXG_RATE;
                    $scope.tableitems[i].PAYMENT_DATE = $scope.itemlist[i].PAYMENT_DATE;

                    
                    $scope.tableitems[i].LOT_NO = $scope.itemlist[i].LOT_NO;
                   
                   

                }
                $scope.Total_Quantity = parseInt(parseInt($scope.Total_Quantity) + parseInt($scope.itemlist[i].CALC_QUANTITY));
                $scope.Total_Amount = (parseFloat(parseFloat($scope.Total_Amount) + ($scope.itemlist[i].CALC_QUANTITY * parseFloat($scope.itemlist[i].CALC_UNIT_PRICE)))).toFixed(2);
                var shippment_quantity = $scope.tableitems[i].SHIPPMENT_QUANTITY == null ? 0 : $scope.tableitems[i].SHIPPMENT_QUANTITY;
                $scope.Shippment_Total_Amount = (parseFloat(parseFloat($scope.Shippment_Total_Amount) + (shippment_quantity * parseFloat($scope.itemlist[i].CALC_UNIT_PRICE)))).toFixed(2);
             

            }
            $scope.containeritem = true;
            $scope.lctrackno = data.data.ItemDetails[0].LC_TRACK_NO;
            $scope.ci.currency = data.data.LC_ENTRY_CURRENCY.CURRENCY_CODE;
            $scope.ci.exchangerate=data.data.LC_ENTRY_CURRENCY.EXCHANGE_RATE;
            $scope.$apply;
            debugger;
            var data = $("#grid").data("kendoGrid").dataSource.data();
            for (var i = 0; i < data.length; i++) {
                var dataItem = data[i];
                if (dataItem.IS_AIR == "Y") {
                    $scope.ci.isair = true;
                }
            }

      
        }, function errorCallback(response) {
            displayPopupNotification("Error Occured.", "error");
        });

    };


    $scope.Shippment_total = function () {
        debugger;
        $scope.Shippment_Total_Amount = 0;
        $scope.Total_Quantity = 0;
        $scope.Total_Amount = 0;
        for (i = 0; i < $scope.itemlist.length; i++) {
            var shippment_quantity = $scope.tableitems[i].SHIPPMENT_QUANTITY == null ? 0 : $scope.tableitems[i].SHIPPMENT_QUANTITY;
            $scope.Shippment_Total_Amount = (parseFloat(parseFloat($scope.Shippment_Total_Amount) + (shippment_quantity * parseFloat($scope.itemlist[i].CALC_UNIT_PRICE)))).toFixed(2);

            $scope.Total_Quantity = parseInt(parseInt($scope.Total_Quantity) + parseInt($scope.tableitems[i].QUANTITY));
            $scope.Total_Amount = (parseFloat(parseFloat($scope.Total_Amount) + ($scope.tableitems[i].QUANTITY * parseFloat($scope.tableitems[i].AMOUNT)))).toFixed(2);
        }
    }

  


    $scope.resettotal = function () {
        $scope.Total_Quantity = 0;
        $scope.Total_Rate = 0;
        $scope.Total_Amount = 0;
    }


    $scope.ipPurchaseOrder = {
        dataType: "json",
        serverFiltering: true,
        filter: "startswith",
        transport: {
            read: function (options) {
                $.ajax({
                    url: "/api/CommercialInvoice/GetAllLcIpPurchaseOrder?filters=" + options.data.filter.filters[0].value,
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
                    url: "/api/CommercialInvoice/GetAllLcIpPurchaseOrderfilter?filters=" + options.data.filter.filters[0].value,
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

    $scope.allInvoiceNumbers = {
        dataType: "json",
        serverFiltering: true,
        filter: "contains",
        transport: {
            read: function (options) {
                $.ajax({
                    url: "/api/CommercialInvoice/GetAllInvoiceNumbers?filters=" + options.data.filter.filters[0].value + "&lcnumber=" + $scope.ci.lcnumber,
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

    $scope.removeIssue = function (index) {
        debugger;
        $scope.itemlist.splice(index, 1);
        $scope.tableitems.splice(index, 1);
        $scope.showreloaditems = true;
        var savecount = 0;
        var tableitemlength = $scope.tableitems.length;
        angular.forEach($scope.tableitems, function (value, index) {


            if (value.QUANTITY == value.RECV_QUANITTY) {
                ++savecount;
            }

        });
        if (savecount == tableitemlength) {

            $scope.disablesave = true;
        } else {
            $scope.disablesave = false;
        }
        $scope.Shippment_total();
    }

    $scope.CheckShippmentIsolation = function (i) {
       $scope.SELETED_DATA = $filter('filter')($scope.LC_PLAN_CONTAINER_LIST, { 'Selected': true }); //selected data filter
   };
    

    function GetLogisticItems(lcnumber) {
        debugger;
        var response = CommercialInvoiceService.GetLogisticItems(lcnumber);
        return response.then(function (data) {
           
            if (data.data.ItemDetails.length == 0) {
                displayPopupNotification("No Data Found.", "warning");
                return;
            }
            else {
                $scope.tableitems = [{
                    ITEM_CODE: '',
                    SNO: '',
                    QUANTITY: '',
                    AMOUNT: '',
                    VALUE: '',
                    HS_CODE: '',
                    COUNTRY_CODE: '',
                    MU_CODE: '',
                    RECV_QUANTITY: '',
                    INPUT_QUANTITY: '',
                    SHIPPMENT_QUANTITY: '',
                    EXCHANGE_RATE: '',
                    SALES_EXG_RATE: '',
                    PAYMENT_DATE:'',
                    LOT_NO: ''
                }];

                $scope.itemlist = data.data.ItemDetails;
                $scope.items = angular.copy($scope.itemlist);
                if (data.data.ItemDetails.length > 0) {
                    $scope.lctracknocommercial = data.data.ItemDetails[0].LC_TRACK_NO;
                }


                for (i = 0; i < $scope.itemlist.length; i++) {

                    if (i == 0) {
                        $scope.tableitems[i].ITEM_CODE = $scope.itemlist[i].ITEM_CODE;
                        $scope.tableitems[i].SNO = $scope.itemlist[i].SNO;
                        $scope.tableitems[i].QUANTITY = $scope.itemlist[i].CALC_QUANTITY;
                        $scope.tableitems[i].AMOUNT = $scope.itemlist[i].CALC_UNIT_PRICE;
                        $scope.tableitems[i].MU_CODE = $scope.itemlist[i].MU_CODE;
                        $scope.tableitems[i].HS_CODE = $scope.itemlist[i].HS_CODE;
                        $scope.tableitems[i].COUNTRY_CODE = $scope.itemlist[i].COUNTRY_CODE;
                        $scope.tableitems[i].RECV_QUANITTY = $scope.itemlist[i].INVOICE_QUANTITY;
                        $scope.tableitems[i].SHIPPMENT_QUANTITY = $scope.itemlist[i].SHIPPMENT_QUANTITY;
                        $scope.tableitems[i].EXCHANGE_RATE = $scope.itemlist[i].EXCHANGE_RATE;
                        $scope.tableitems[i].SALES_EXG_RATE = $scope.itemlist[i].SALES_EXG_RATE;
                        $scope.tableitems[i].PAYMENT_DATE = $scope.itemlist[i].PAYMENT_DATE;

                        
                        $scope.tableitems[i].LOT_NO = $scope.itemlist[i].LOT_NO;
                    }
                    else {
                        $scope.tableitems.push({
                            ITEM_CODE: $scope.itemlist[i].ITEM_CODE,
                            SNO: $scope.itemlist[i].SNO,
                            QUANTITY: $scope.itemlist[i].CALC_QUANTITY,
                            AMOUNT: $scope.itemlist[i].CALC_UNIT_PRICE,
                            MU_CODE: $scope.itemlist[i].MU_CODE,
                            HS_CODE: $scope.itemlist[i].HS_CODE,
                            COUNTRY_CODE: $scope.itemlist[i].COUNTRY_CODE,
                            RECV_QUANITTY: $scope.itemlist[i].INVOICE_QUANTITY,
                            SHIPPMENT_QUANTITY: $scope.itemlist[i].SHIPPMENT_QUANTITY,
                            EXCHANGE_RATE: $scope.itemlist[i].EXCHANGE_RATE,
                            SALES_EXG_RATE: $scope.itemlist[i].SALES_EXG_RATE,
                            PAYMENT_DATE: $scope.itemlist[i].PAYMENT_DATE,

                            
                            LOT_NO: $scope.itemlist[i].LOT_NO
                        });
                        $scope.tableitems[i].ITEM_CODE = $scope.itemlist[i].ITEM_CODE;
                        $scope.tableitems[i].SNO = $scope.itemlist[i].SNO;
                        $scope.tableitems[i].QUANTITY = $scope.itemlist[i].CALC_QUANTITY;
                        $scope.tableitems[i].AMOUNT = $scope.itemlist[i].CALC_UNIT_PRICE;
                        $scope.tableitems[i].MU_CODE = $scope.itemlist[i].MU_CODE;
                        $scope.tableitems[i].HS_CODE = $scope.itemlist[i].HS_CODE;
                        $scope.tableitems[i].COUNTRY_CODE = $scope.itemlist[i].COUNTRY_CODE;
                        $scope.tableitems[i].RECV_QUANITTY = $scope.itemlist[i].INVOICE_QUANTITY;
                        $scope.tableitems[i].SHIPPMENT_QUANTITY = $scope.itemlist[i].SHIPPMENT_QUANTITY;
                        $scope.tableitems[i].EXCHANGE_RATE = $scope.itemlist[i].EXCHANGE_RATE;
                        $scope.tableitems[i].SALES_EXG_RATE = $scope.itemlist[i].SALES_EXG_RATE;
                        $scope.tableitems[i].PAYMENT_DATE = $scope.itemlist[i].PAYMENT_DATE;

                        
                        $scope.tableitems[i].LOT_NO = $scope.itemlist[i].LOT_NO;
                    }
                }
                $scope.Shippment_total();
                $scope.containeritem = true;
                $scope.lctrackno = data.data.ItemDetails[0].LC_TRACK_NO;
                $scope.$apply;
            }
        }, function errorCallback(response) {
            displayPopupNotification("Error Occured.", "error");
        });

    };

    $scope.reloadIssue = function () {
        debugger;
        GetLogisticItems($scope.ReloadCode);
        $scope.showreloaditems = false;
        $scope.Shippment_Total_Amount = 0;
       
    };

  

    $scope.cancelCInvoice = function () {
        $scope.tempinvoicecode = 0;
        $('#invoicecode').val("");
        $scope.disablelcnumber = false;
        $scope.containeritem = false;
        $scope.disablesave = false;
        init();
        $("#grid").data("kendoGrid").tbody.find("tr").removeClass('highlight');
    }

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

    var cidetails;
  
  
  
    $scope.createCommercialInvoice = function (isValid, options) {
        debugger;
       
        var isair = $scope.ci.isair;
        showloader();
        var invoicecode = $scope.ci.cinvoiceno;


        if ($scope.ci.lcnumber === undefined || $scope.ci.lcnumber === "") {
            hideloader();
            displayPopupNotification("Lc Number is Required", "warning");
            return;
        }

        if ($scope.ci.cinvoiceno === undefined || $scope.ci.cinvoiceno === "") {
            hideloader();
            displayPopupNotification("Invoice Number is Required", "warning");
            return;
        }
        if ($scope.ci.cinvoicedate === undefined || $scope.ci.cinvoicedate === "") {
            hideloader();
            displayPopupNotification("Invoice Date is Required", "warning");
            return;
        }


        if ($scope.saveAction != "Update" || $scope.scAction != "Update & Continue") {
           
                var itemslist = $.extend(true, [], $scope.tableitems),
                    length = itemslist.length;
                var reserveItemList = [];
                    for (i = length - 1; i >= 0; i--) {

                    if (itemslist[i].SHIPPMENT_QUANTITY == null || itemslist[i].SHIPPMENT_QUANTITY == undefined || itemslist[i].SHIPPMENT_QUANTITY == "") {
                        itemslist.splice(i, 1);
                    }
                }

          //  }
            if (itemslist != undefined) {
                if (itemslist.length == 0) {
                    displayPopupNotification("Please Input at least one quantity.", "warning");
                    hideloader();
                    return;
                }
            }
            if (!isair) {
            $scope.CHECK_LC_PLAN_CONTAINER_LIST = [];
            $scope.CHECK_LC_PLAN_CONTAINER_LIST = $filter('filter')($scope.LC_PLAN_CONTAINER_LIST, { 'Selected': true }); //selected data filter
            if ($scope.CHECK_LC_PLAN_CONTAINER_LIST.length === 0) {
                $scope.CHECK_LC_PLAN_CONTAINER_LIST = [];
                displayPopupNotification("Please Select Carrier List.", "warning");
                hideloader();
                return;
            }
            }

            $scope.tableitems;
            $scope.lcnumber;
            var tempinvoice;
            var templctrackno;
            tempinvoice = $scope.tempinvoicecode == 0 ? 0 : $scope.tempinvoicecode;
            templctrackno = $scope.templctrackno == 0 ? $scope.lctracknocommercial : $scope.templctrackno;

           
            // $scope.LC_PLAN_CONTAINER_LIST_CHECK = $filter('filter')($scope.LC_PLAN_CONTAINER_LIST, { 'Selected': true }); //selected data filter
            // if ($scope.LC_PLAN_CONTAINER_LIST_CHECK.length === 0) {
                // displayPopupNotification("Please Select Carrier List.", "warning");
                // hideloader();
                // return;
            // }
            if (!isair) {
                $scope.LC_PLAN_CONTAINER_LIST = $filter('filter')($scope.LC_PLAN_CONTAINER_LIST, { 'Selected': true }); //selected data filter
                if ($scope.LC_PLAN_CONTAINER_LIST.length === 0) {
                    displayPopupNotification("Please Select Carrier List.", "warning");
                    hideloader();
                    return;
                }
            }
            $scope.CommercialInvoiceData = [{
                "INVOICE_DATE": $scope.ci.cinvoicedate,
                "INVOICE_NUMBER": $scope.ci.cinvoiceno,
                "INVOICE_CODE": $scope.editinvoicecode,
                "PP_DATE": $scope.ci.cppdate,
                "AWB_NUMBER": $scope.ci.airwaybillno,
                "AWB_DATE": $scope.ci.airwaybilldate,
                "PP_NO": $scope.ci.cppnumber,
                "LOT_NO": $scope.ci.lotno,
                "CURRENCY_CODE": $scope.ci.currency,
                "EXCHANGE_RATE": $scope.ci.exchangerate,
            }];




            var MultiCommercialInvoiceModel = {
                LC_NUMBER: $scope.LC_NUMBER_DESC,
                LC_TRACK_NO: $scope.lctrackno,
                TEMP_INVOICE_CODE: tempinvoice,
                TEMP_LC_TRACK_NO: templctrackno,
                CommercialInvoiceData: $scope.CommercialInvoiceData,
               // Itemlist: $scope.tableitems,
                Itemlist: itemslist,
                ContainerList: $scope.LC_PLAN_CONTAINER_LIST,
                IS_AIR: $scope.ci.isair
            };


        }
        else {
         
            $scope.CommercialInvoiceData = [];
            $scope.CommercialInvoiceData = [{
                "INVOICE_DATE": $scope.ci.cinvoicedate,
                "INVOICE_NUMBER": $scope.ci.cinvoiceno,
                "INVOICE_CODE": $scope.editinvoicecode,
                "PP_DATE": $scope.ci.cppdate,
                "AWB_NUMBER": $scope.ci.airwaybillno,
                "AWB_DATE": $scope.ci.airwaybilldate,
                "PP_NO": $scope.ci.cppnumber,
                "LOT_NO": $scope.ci.lotno
            }];


            var MultiCommercialInvoiceModel = {
                LC_NUMBER: $scope.ci.lcnumber,
                //LC_TRACK_NO: $scope.lctrackno,
                LC_TRACK_NO: $scope.lctracknocommercial,
                TEMP_INVOICE_CODE: tempinvoice,
                TEMP_LC_TRACK_NO: templctrackno,
                CommercialInvoiceData: $scope.CommercialInvoiceData,
                Itemlist: $scope.tableitems,
                CURRENCY_CODE: $scope.ci.currency,
                EXCHANGE_RATE: $scope.ci.exchangerate,
                IS_AIR: $scope.ci.isair
            };






        }


        

        var ciurl = window.location.protocol + "//" + window.location.host + "/api/CommercialInvoice/CreateCommercialInvoice";

        var response = $http({
            method: "post",
            url: ciurl,
            data: MultiCommercialInvoiceModel,
            dataType: "json"
        });
      
        return response.then(function (data) {
         
            if (options == "save") {
                $scope.showFormPanel = false;
                $scope.containeritem = false;
            }
            $('.image-placeholder').html("");
            if (data.data != null) {

                $('#lctrackno').val(data.data.LC_TRACK_NO);
                $('#dropzoneinvoicecode').val(data.data.INVOICE_CODE);
            }

            myDropzone.processQueue();
            $('#grid').data("kendoGrid").dataSource.read();
            hideloader();
            displayPopupNotification("Invoice Created Successfully.", "success");
            init();

        }, function errorCallback(response) {

            if (response.status == "304") {
                displayPopupNotification("Invoice Number Already Exist.", "warning");
            }
            else {
                displayPopupNotification("Error Occured", "error");
            }
            hideloader();
        });

       
    }

    $scope.InvoiceNoOnSelect = function (e) {
       
        if ($scope.saveAction != "Update" || $scope.scAction != "Update & Continue") {

            var griditems = $("#grid").data("kendoGrid").dataSource.data();
            if ($('#invoiceNumber').data("kendoAutoComplete").dataItem() != undefined) {
                var INVOICE_NUMBER = $('#invoiceNumber').data("kendoAutoComplete").dataItem().INVOICE_NUMBER;
                if (INVOICE_NUMBER !== undefined) {
                    var data = $filter('filter')(griditems, { 'INVOICE_NUMBER': INVOICE_NUMBER }); //selected data filter
                    if (data.length > 0) {
                        $scope.ci.cinvoiceno = "";
                        displayPopupNotification("Commercial Invoice Already Existed.", "warning");
                        return;
                    }
                }
            }
        }
    };

    $scope.InvoiceNoOnChange = function (e) {
     
       
        if ($scope.saveAction != "Update" || $scope.scAction != "Update & Continue") {
            $scope.ci.cinvoiceno;
            var response = CommercialInvoiceService.InvoiceOnChange($scope.ci.cinvoiceno);
            return response.then(function (data) {
             
                if (data.data != null || data.data != undefined) {
                    $scope.tempinvoicecode = data.data.INVOICE_CODE;
                    $scope.templctrackno = data.data.LC_TRACK_NO;
                    if (data.data.INVOICE_DATE != null || data.data.INVOICE_DATE != undefined || data.data.INVOICE_DATE != "") {

                        $scope.ci.cinvoicedate = $filter('date')(data.data.INVOICE_DATE, "MM/dd/yyyy");
                        $scope.disableinvoicedate = true;

                    }
                    if (data.data.PP_DATE != null || data.data.PP_DATE != undefined || data.data.PP_DATE != "") {
                        $scope.ci.cppdate = $filter('date')(data.data.PP_DATE, "MM/dd/yyyy");
                        $scope.disableppdate = true;
                    }
                    if (data.data.PP_NO != null || data.data.PP_NO != undefined || data.data.PP_NO != "") {
                        $scope.ci.cppnumber = data.data.PP_NO;
                        $scope.disablecppnumber = true;
                    }
                    if (data.data.EXCHANGE_RATE != null || data.data.EXCHANGE_RATE != undefined || data.data.EXCHANGE_RATE != "") {
                        $scope.ci.exchangerate = data.data.EXCHANGE_RATE;
                        $scope.disableexchangerate = true;
                    }
                    if (data.data.INVOICE_CURRENCY != null || data.data.INVOICE_CURRENCY != undefined || data.data.INVOICE_CURRENCY != "") {
                        $scope.ci.currency = data.data.INVOICE_CURRENCY;
                        $scope.ci.currencycode = data.data.INVOICE_CURRENCY;
                        $scope.disablecurrency = true;
                    }
                    if (data.data.AWB_DATE != null || data.data.AWB_DATE != undefined || data.data.AWB_DATE != "") {
                        $scope.ci.airwaybilldate = $filter('date')(data.data.AWB_DATE, "MM/dd/yyyy");
                        $scope.disableairwaybilldate = true;
                    }
                    if (data.data.AWB_NUMBER != null || data.data.AWB_NUMBER != undefined || data.data.AWB_NUMBER != "") {
                        $scope.ci.airwaybillno = data.data.AWB_NUMBER;
                        $scope.disableairwaybillno = true;
                    }
                    if (data.data.LC_NUMBER != null || data.data.LC_NUMBER != undefined || data.data.LC_NUMBER != "") {
                        $scope.ci.lcnumber = data.data.LC_NUMBER;

                    }

                 
                }
                else {
                    $scope.disableinvoicedate = false;
                    $scope.disableppdate = false;
                    $scope.disablecppnumber = false;
                    $scope.disableexchangerate = false;
                    $scope.disablecurrency = false;
                    $scope.disableairwaybilldate = false;
                    $scope.disableairwaybillno = false;
                }

            }, function errorCallback(response) {
                displayPopupNotification("Error Occured.", "error");

            });
        }
    }

  
    $scope.OnInputQuantityChange = function (tableitems, i) {
        debugger;
        if (tableitems[i].SHIPPMENT_QUANTITY != null) {
            var quantity = tableitems[i].QUANTITY - tableitems[i].SHIPPMENT_QUANTITY;
            if (quantity < $scope.tableitems[i].RECV_QUANITTY) {
                $scope.tableitems[i].SHIPPMENT_QUANTITY = "";
                displayPopupNotification("Input quantity should be less than remaining quantity.", "warning");
            }
        }
        else {
            if (tableitems[i].QUANTITY < $scope.tableitems[i].SHIPPMENT_QUANTITY) {
                $scope.tableitems[i].SHIPPMENT_QUANTITY = "";
                displayPopupNotification("Input quantity should be less than remaining quantity.", "warning");
            }
        }
        $scope.Shippment_total();
    }

    $scope.removeAllIssue = function (tableitems) {
        debugger;
        var count = 0;
        var length = tableitems.length;
        for (j = 0; j < length; j++)
        {
            debugger;
            if (tableitems[count].SHIPPMENT_QUANTITY == null) {
                $scope.removeIssue(count);
            }
            else {
                count++;
            }
        }

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


  
    $scope.FilterGrid = function (INVOICE_NUMBER) {
        $("#grid").data("kendoGrid").tbody.find("tr").css("background-color", "");
        var data = $("#grid").data("kendoGrid").dataSource.data();
        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
            if (dataItem.INVOICE_NUMBER == INVOICE_NUMBER) {
                $("#grid").data("kendoGrid").tbody.find("tr[data-uid=" + dataItem.uid + "]").css("background-color", "#80ddaa");
            }
        }

    };


    $scope.mainGridOptions = {

        toolbar: ["excel"],
        dataSource: {
            type: "json",
            transport: {
               // read: "/api/CommercialInvoice/getAllCommercialInvoiceFilter?lcnumber="+"",
                read: "/api/CommercialInvoice/getAllCommercialInvoice",
            },
           pageSize: 100,
            serverPaging: false,
            serverSorting: false,
            schema: {
                model: {
                    fields: {
                        LC_TRACK_NO: { type: "number" },
                        LC_NUMBER: { type: "string" },
                        INVOICE_NUMBER: { type: "string" },
                        mylist: { type: "" }
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
                    startswith: "Starts with",
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
            $scope.EditCommercialInvoice = function (dataItem) {
                debugger;
                $scope.saveAction = "Update";
                $scope.scAction = "Update & Continue";
                $scope.disablelcnumber = true;
                $scope.showitems = false;
                $scope.showFormPanel = true;
                $scope.ShowOnEditForm = true;
                $scope.lctracknocommercial = dataItem.LC_TRACK_NO;
                $('#invoiceNumber').val(dataItem.INVOICE_CODE);
                $scope.dropzoneinvoicecode = dataItem.INVOICE_CODE;
                $scope.editinvoicecode = dataItem.INVOICE_CODE;
                $scope.ci.lcnumber = dataItem.LC_NUMBER;
                $scope.ci.cinvoicedate = $filter('date')(dataItem.INVOICE_DATE, "MM/dd/yyyy");
                $scope.ci.cinvoiceno = dataItem.INVOICE_NUMBER;
                $scope.FilterGrid(dataItem.INVOICE_NUMBER);
                $scope.ci.cppdate = $filter('date')(dataItem.PP_DATE, "MM/dd/yyyy");
                $scope.ci.cppnumber = dataItem.PP_NO;
                $scope.ci.currency = dataItem.INVOICE_CURRENCY;
                $scope.ci.currencycode = dataItem.INVOICE_CURRENCY;
                $scope.ci.exchangerate = dataItem.EXCHANGE_RATE;
                $scope.ci.airwaybilldate = $filter('date')(dataItem.AWB_DATE, "MM/dd/yyyy");
                $scope.ci.airwaybillno = dataItem.AWB_NUMBER;
                $scope.ci.lotno = dataItem.LOT_NO;
                var IsAir = dataItem.IS_AIR;
                if (IsAir === "Y") {
                    $scope.ci.isair = true;
                } else
                {
                    $scope.ci.isair = false;
                }
                
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
                        fileName: "Commercial Invoice.xlsx"
                    });
                });
        },
        columns: [
            {
                field: "LC_TRACK_NO",
                title: "Track Number",
                attributes: { style: "text-align:right;" },
                width: "120px"
            },
            {
                field: "LC_NUMBER",
                title: "Lc Number",
                width:"120px"
            }, 
                
            {
                field: "INVOICE_NUMBER",
                title: "Invoice Number",
                width: "125px"
            },
        

            {
                field: "INVOICE_DATE",
                title: "Invoice Date",
                template: "#= kendo.toString(kendo.parseDate(INVOICE_DATE),'M/dd/yyyy') #",
                width: "120px"
            },
            {
                field: "PP_NO",
                title: "Pragyapan No",

            },
            {
                field: "PP_DATE",
                title: "Pragyapan Date",
                template: "#= PP_DATE ==null ? '' : kendo.toString(kendo.parseDate(PP_DATE),'M/dd/yyyy') #",
            },

       

            {
                field: "AWB_NUMBER",
                title: "Airway Bill No",
            },
            {
                field: "AWB_DATE",
                title: "Airway Bill Date",
                template: "#= AWB_DATE ==null ? '' : kendo.toString(kendo.parseDate(AWB_DATE),'M/dd/yyyy') #",
            },
            {
                field: "IS_AIR",
                title: "Is Air",
            },
             {
                 field: "mylist",
                 title: "Images",
                 template: '<span ng-bind-html= "getImage(dataItem.mylist, dataItem.FILE_DETAIL)"> </span>',

             },
            
            {
                field: "ID", title: "Action", sortable: false, filterable: false,
                template: '<a class="edit glyphicon glyphicon-edit" title="Edit" ng-click="EditCommercialInvoice(dataItem)" style="color:grey;"><span class="sr-only"></span> </a>  <a class="fa fa-history" ng-click="HistoryCommercialInvoice(dataItem)" title="History" style="color:grey;"><span class="sr-only"></span> </a>'
            }

        ]
    };

    


    $scope.itemGridOptions = function (dataItem) {

        return {
            dataSource: {
                type: "json",
                transport: {

                    read: {
                        url: "/api/CommercialInvoice/getAllCommercialItemsList?filter=" + dataItem.INVOICE_NUMBER,
                        dataType: "json"
                    },
                    update: {
                        url: window.location.protocol + "//" + window.location.host + "/api/CommercialInvoice/UpdateQuantity",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        complete: function (e) {
                         
                            if (JSON.parse(e.responseText).MESSAGE == "EXCEEDED") {
                                $('.itemgrid').data("kendoGrid").dataSource.read();
                                displayPopupNotification("Entered Quantity Exceeded total quantity.", "warning");
                            }
                            else if (JSON.parse(e.responseText).MESSAGE == "UPDATED") {
                                $('.itemgrid').data("kendoGrid").dataSource.read();
                                displayPopupNotification("Successfully Updated Item Quantity.", "success");
                            }
                            else if (JSON.parse(e.responseText).MESSAGE == "FAILED") {
                                $('.itemgrid').data("kendoGrid").dataSource.read();
                                displayPopupNotification("Failed To Update Item Quantity,quantity is greater than total quantity.", "warning");
                            }
                            else{
                                if (e.status == 200) {
                                    $.each($(".itemgrid"), function (i, value) {

                                        $(value).data("kendoGrid").dataSource.read();
                                    });
                                    displayPopupNotification("Successfully Updated Item Quantity.", "success");
                                }
                                $scope.Greater = false;
                            }

                        }

                    },
                    parameterMap: function (options, operation) {
                     
                        if (options.QUANTITY > options.TOTAL_QUANTITY) {
                            $.each($(".itemgrid"), function (i, value) {

                                $(value).data("kendoGrid").dataSource.read();
                            });
                           // displayPopupNotification("Quantity is greater than total quantity.", "warning");
                            return false;
                            $scope.Greater = false;
                        }
                        if (operation !== "read" && options) {

                            var data = options;

                            return JSON.stringify(data);
                        }

                    }
                },
                serverPaging: false,
                serverSorting: false,
                serverFiltering: false,
                pageSize: 50,
                schema: {
                    model: {
                        id: "ITEM_CODE",
                        fields: {
                            ITEM_EDESC: { editable: false },
                            MU_CODE: { editable: false },
                            AMOUNT: { type: "number", editable: false },
                            QUANTITY: { type: "number", validation: { min: 0, required: true } },
                            EXCHANGE_RATE: { type: "number", validation: { min: 0, required: true } },
                            SALES_EXG_RATE: { type: "number", validation: { min: 0, required: true } },
                            PAYMENT_DATE: { type: "date", editable: true },
                            TOTAL_QUANTITY: { type: "number", editable: false }
                        }
                    }
                },
                filter: {
                    field: "LC_TRACK_NO",
                    operator: "eq",
                    value: dataItem.LC_TRACK_NO
                }
               
            },
            scrollable: false,
            sortable: true,
            pageable: true,
            resizable: true,
            excelExport: function (e) {
                // prevent saving the file
                e.preventDefault();

               
            },
            dataBound: function (e) {

            },
            dataBinding: function () {
                record = (this.dataSource.page() - 1) * this.dataSource.pageSize();
            },

            columns: [
            
                { field: "ITEM_EDESC", title: "Item Name", width: "90px" },
                { field: "MU_CODE", title: "Master Unit", width: "30px" },
                { field: "QUANTITY", title: "Received Quantity", attributes: { style: "text-align:right;" }, width: "20px" },
                { field: "TOTAL_QUANTITY", title: "Total Quantity", attributes: { style: "text-align:right;" }, width: "20px" },
                { field: "AMOUNT", title: "Rate", attributes: { style: "text-align:right;" }, width: "20px" },
                { field: "EXCHANGE_RATE", title: "Payment Exg Rate", attributes: { style: "text-align:right;" }, width: "30px" },
                { field: "SALES_EXG_RATE", title: "Sales Exg Rate", attributes: { style: "text-align:right;" }, width: "30px" },
                { field: "PAYMENT_DATE", title: "Payment Date", attributes: { style: "text-align:right;" }, width: "30px" },
                { template: "<a class='k-button' ng-click='ShowItemDo(dataItem)'>View DO</a>", title: "Item DO", width: "22px" },
                { command: ["edit"], title: "Action", width: "22px"}
               
            ],
            editable: "inline",
        };
    };


    $scope.OnDOChange = function (DO_index) {
        debugger;
        var previousDONumber = $('#DO_NUMBER_' + DO_index).val();
        angular.forEach($scope.DO, function (value, index) {
            if (index != DO_index) {
                if (previousDONumber == $scope.DO[index].DO_NUMBER && $scope.DO[index].DO_NUMBER != "") {
                    $('#DO_NUMBER_' + DO_index).val("");
                    displayPopupNotification("Two DO Number can not be same!", "warning");
                    return;
                }
            }

        });
    };


    $scope.ShowItemDo = function (dataItem) {
        var LC_TRACK_NO = dataItem.LC_TRACK_NO;
        var INVOICE_CODE= dataItem.INVOICE_CODE;
        var ITEM_CODE= dataItem.ITEM_CODE;
        $scope.DODataItem = {
            LC_TRACK_NO: LC_TRACK_NO,
            INVOICE_CODE: INVOICE_CODE,
            ITEM_CODE: ITEM_CODE,
        };
        $scope.DO_ITEM_EDESC = dataItem.ITEM_EDESC;
        var CIDOModel = $scope.DODataItem;
        var url = window.location.protocol + "//" + window.location.host + "/api/CommercialInvoice/GetCIItemDOByItemCode";
        var response = $http({
            method: "post",
            url: url,
            data: JSON.stringify(cIDOModel=CIDOModel),
            dataType: "json"
        });

        return response.then(function (data) {
             if (data.data.length > 0)
            {
                $scope.DO = data.data;
            }
            
            $("#DOModal").modal('show');
        });

      
    };


    $scope.Add_DO_List = function (index) {

        $scope.DO.push({
            LC_DO_CODE:"",
            LC_TRACK_NO: "",
            INVOICE_CODE: "",
            ITEM_CODE: "",
            DO_NUMBER: "",
            QUANTITY: "",
            REMARKS: ""
        });

    }
    $scope.Remove_DO_List = function (index) {
        if ($scope.DO.length > 1) {
            $scope.DO.splice(index, 1);
        }
    };


    $scope.ResetDo = function () {
        $scope.DO = [{
            LC_DO_CODE: "",
            LC_TRACK_NO: "",
            INVOICE_CODE: "",
            ITEM_CODE: "",
            DO_NUMBER: "",
            QUANTITY: "",
            REMARKS: ""
        }];


    };

    

    $scope.SaveDO = function () {
    
        $scope.DODataItem;
        angular.forEach($scope.DO, function (value, index) {
            $scope.DO[index].LC_TRACK_NO = $scope.DODataItem.LC_TRACK_NO;
            $scope.DO[index].INVOICE_CODE = $scope.DODataItem.INVOICE_CODE;
            $scope.DO[index].ITEM_CODE = $scope.DODataItem.ITEM_CODE;
        });

        $scope.DO;
        var CIDOModel = $scope.DO;

        var url = window.location.protocol + "//" + window.location.host + "/api/CommercialInvoice/CreateCIItemDO";
        var response = $http({
            method: "post",
            url: url,
            data: JSON.stringify(cIDOModel=CIDOModel),
            dataType: "json"
        });

        return response.then(function (data) {
            $scope.ResetDo();
            displayPopupNotification("CI Item DO Saved Successfully.", "success");

        }, function errorCallback(response) {
            displayPopupNotification(response.data, "error");
        });

    };


    $scope.HistoryCommercialInvoice = function (dataItem) {
       
        $scope.historyitemGridOptions = function () {


            return {
                dataSource: {
                    type: "json",
                    transport: {
                        read: {
                            url: "/api/CommercialInvoice/getAllHistoryCommercialInvoiceList?lctrackno=" + dataItem.LC_TRACK_NO,
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
                                LC_TRACK_NO: { validation: { required: true } },
                                LC_NUMBER: { validation: { required: true } },
                                INVOICE_NUMBER: { validation: { required: true } },
                               
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
                    { field: "ITEM_EDESC", title: "Item Name", width: "110px" },
                    { field: "MU_CODE", title: "MU", width: "20px" },
                    { field: "QUANTITY", title: "Qty.", attributes: { style: "text-align:right;" }, width: "20px" },
                    { field: "AMOUNT", title: "Rate", attributes: { style: "text-align:right;" }, width: "20px" },
                  
                    {
                        field: "CREATED_DATE", title: "Created Date", width: "50px", format: "{0:M/dd/yyyy}",
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
        //$(".historyitemgrid").data("kendoGrid").destroy();
        $scope.historyitemGridOptions();

        
        $('#historymodal').modal('show');
    }
    

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

    $scope.monthSelectorOptions = {
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() + 100);
        },
        format: "dd-MMM-yyyy",
        // specifies that DateInput is used for masking the input element
        dateInput: true
    };



});


app.service('CommercialInvoiceService', function ($http, $q, $timeout) {

    this.ItemsOnChange = function (lcnumber) {
       
        var response = $http({
            method: "get",
            url: "/api/LcLogisticPlan/GetLCLogisticPlanItemsByLCNumber?lcnumber=" + lcnumber,
            dataType: "json"
        });
        return response;
    }
    this.GetLogisticPlan = function (lcnumber) {

        var response = $http({
            method: "get",
            url: "/api/CommercialInvoice/GetLogisticPlan?lcnumber=" + lcnumber,
            dataType: "json"
        });
        return response;
    }

    this.GetLogisticItems = function (lcnumber) {

        var response = $http({
            method: "get",
            url: "/api/CommercialInvoice/GetLogisticItems?lcnumber=" + lcnumber,
            dataType: "json"
        });
        return response;
    }
    
    this.LoadCIBylcnumber = function (lcnumber) {

        var response = $http({
            method: "get",
            url: "/api/CommercialInvoice/LoadCIBylcnumber?lcnumber=" + lcnumber,
            dataType: "json"
        });
        return response;
    }


    



    this.InvoiceOnChange = function (invoiceno) {
        var response = $http({
            method: "get",
            url: "/api/CommercialInvoice/getDetailByInvoiceNo?invoiceno=" + invoiceno,
            dataType: "json"
        });
        return response;
    }
    this.loadLC = function () {
        return $http.get("/api/CommercialInvoice/getLOCType");
    }
    this.loadCategory = function () {
        return $http.get("/api/CommercialInvoice/getCategoryType");
    }
    this.loadUnit = function () {
        return $http.get("/api/CommercialInvoice/getUnitType");
    }
    this.saveCommercialInvoice = function (invoice) {
        var response = $http({
            method: "post",
            url: "/Api/CommercialInvoice/saveCommercialInvoice",
            data: JSON.stringify(invoice),
            dataType: "json"
        });
        return response;
    }
    this.updateCommercialInvoice = function (invoice) {
        var response = $http({
            method: "post",
            url: "/Api/CommercialInvoice/UpdateCommercialInvoice",
            data: JSON.stringify(invoice),
            dataType: "json"
        });
        return response;
    }
    this.deleteCommercialInvoice = function (pfinum) {
        var response = $http({
            method: "post",
            url: "/Api/CommercialInvoice/DeleteCommercialInvoice",
            params: {
                pfiCode: JSON.stringify(pfinum)
            },
            dataType: "json"
        });
        return response;
    }


});


