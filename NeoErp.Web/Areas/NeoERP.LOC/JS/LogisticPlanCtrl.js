app.controller('LogisticPlanCtrl', function ($scope, $http, $filter, $q, LogisticPlanService) {


    $scope.LC_LOGISTIC_PLAN = [];
    $scope.LC_LOGISTIC_PLAN_CONTAINER = [];
    $scope.Route = [];
    $scope.RouteContainer = [];
    $scope.itemview = false;
    $scope.AirView = false;
    $scope.Hideshipment = false;
    $scope.ReserveTrackno = "";
    $scope.disablesave = false;
    $scope.showcarrier = false;
    $scope.LoadType = false;
    $scope.showcontaineredit = false;
    $scope.ACT_BOOKING_DATE = "";
    $scope.ACT_LOADING_DATE = "";
    $scope.filterData = {
        lcnumber: ""
    };
    $scope.LC_LOGISTIC_PLAN =
        {
            LOGISTIC_PLAN_CODE: "",
            LC_TRACK_NO: "",
            SNO: "",
            CONSIGNEE_NAME: "",
            CONSIGNEE_ADDRESS: "",
            NOTIFY_APPLICANT_NAME: "",
            NOTIFY_APPLICANT_ADDRESS: "",
            REMARKS: "",
            SHIPPING_TYPE: "",
            AIR_PACK: "",
            LC_NUMBER: "",
            LOT_NO: ""
        };
    $scope.LC_LOGISTIC_PLAN_CONTAINER =
        [{
            LC_TRACK_NO: "",
            CONTAINER_CODE: "",
            LOAD_TYPE: "",
            FROM_LOCATION_CODE: "",
            TO_LOCATION_CODE: "",
            EST_BOOKING_DATE: "",
            EST_LOADING_DATE: "",
            ACT_BOOKING_DATE: "",
            ACT_LOADING_DATE: "",
            ROUTE_NO: "",
            SHIPPING_TYPE: "",
            CARRIER_NUMBER: ""
        }];

    $scope.Route = [{
        LC_TRACK_NO: "",
        FROM_LOCATION_CODE: "",
        TO_LOCATION_CODE: "",
        EST_BOOKING_DATE: "",
        EST_LOADING_DATE: "",
        ACT_BOOKING_DATE: "",
        ACT_LOADING_DATE: "",
        ROUTE_NO: "",
        SHIPPING_TYPE: ""
    }];

    $scope.RouteContainer = [{
        CONTAINER_CODE: "",
        LOAD_TYPE: "",
        CARRIER_NUMBER: ""
    }];
    //route 
    $scope.add_childRoute_element = function (index) {
     
        $scope.Route.push({
            LC_TRACK_NO: "",
            FROM_LOCATION_CODE: "",
            TO_LOCATION_CODE: "",
            EST_BOOKING_DATE: "",
            EST_LOADING_DATE: "",
            ACT_BOOKING_DATE: "",
            ACT_LOADING_DATE: "",
            ROUTE_NO: "",
            SHIPPING_TYPE: ""
          
        });

    }
    $scope.remove_childRoute_element = function (index) {
     
        if ($scope.Route.length > 1) {
            $scope.Route.splice(index, 1);
        }


    };
    //
    //container 
    $scope.add_childRouteContainer_element = function (index) {
     
        $scope.RouteContainer.push({
            CONTAINER_CODE: "",
            LOAD_TYPE: "",
            CARRIER_NUMBER:""
        });

    }
    $scope.remove_childRouteContainer_element = function (index) {
     
        if ($scope.RouteContainer.length > 1) {
            $scope.RouteContainer.splice(index, 1);
        }


    };
    //
    $scope.Reset = function () {
        $scope.LC_LOGISTIC_PLAN = [];
        $scope.LC_LOGISTIC_PLAN_CONTAINER = [];
        $scope.LC_LOGISTIC_PLAN =
            {
                LOGISTIC_PLAN_CODE: "",
                LC_TRACK_NO: "",
                SNO: "",
                CONSIGNEE_NAME: "",
                CONSIGNEE_ADDRESS: "",
                NOTIFY_APPLICANT_NAME: "",
                NOTIFY_APPLICANT_ADDRESS: "",
                REMARKS: "",
                Air_Pack: ""

            };

        $scope.LC_LOGISTIC_PLAN_CONTAINER =
            [{
                LC_TRACK_NO: "",
                CONTAINER_CODE: "",
                LOAD_TYPE: "",
                FROM_LOCATION_CODE: "",
                TO_LOCATION_CODE: "",
                EST_BOOKING_DATE: "",
                EST_LOADING_DATE: "",
                ACT_BOOKING_DATE: "",
                ACT_LOADING_DATE: "",
                CARRIER_NUMBER: ""
            }];

    }

    $scope.AddLogistics = function () {
        $scope.saveAction = "Save";
        $scope.showFormPanel = true;
        $scope.showcarrier = false;
        $scope.ContentModal = true;
        $scope.showcontaineredit = false;
        $scope.disablelcnumber = false;
    }

    $scope.cancelLCPlan = function () {
        $scope.showcarrier = false;
        $scope.showFormPanel = false;
        $scope.Reset();
        $scope.lctrackno = 0;
        $scope.sno = 0;
        $scope.itemview = false;
        $scope.containerView = false;
        $scope.showcontaineredit = false;
        $scope.disablelcnumber = false;
       // $scope.ci.lcnumber = "";
        $scope.itemlist = [];
        $scope.Route = [];
        $scope.Route = [{
            LC_TRACK_NO: "",
            FROM_LOCATION_CODE: "",
            TO_LOCATION_CODE: "",
            EST_BOOKING_DATE: "",
            EST_LOADING_DATE: "",
            ACT_BOOKING_DATE: "",
            ACT_LOADING_DATE: "",
            ROUTE_NO: "",
            SHIPPING_TYPE: ""
        }];
        $scope.RouteContainer = [];
        $scope.RouteContainer = [{
            CONTAINER_CODE: "",
            LOAD_TYPE: "",
            CARRIER_NUMBER:""
        }];
     
        $("#lcnumberautocomplete").data("kendoAutoComplete").value("");
        $("#fromlocation").data("kendoDropDownList").value("");
        $("#tolocation").data("kendoDropDownList").value("");
        $scope.cleareditcontent();
        $("#grid").data("kendoGrid").tbody.find("tr").removeClass('highlight');
    }

    $scope.add_childContainer_element = function (index) {
     
        $scope.LC_LOGISTIC_PLAN_CONTAINER.push({
            LC_TRACK_NO: "",
            CONTAINER_CODE: "",
            LOAD_TYPE: "",
            FROM_LOCATION_CODE: "",
            TO_LOCATION_CODE: "",
            EST_BOOKING_DATE: "",
            EST_LOADING_DATE: "",
            ACT_BOOKING_DATE: "",
            ACT_LOADING_DATE: "",
            CARRIER_NUMBER: ""
        });

    }

    $scope.remove_childContainer_element = function (index) {
     
        if ($scope.LC_LOGISTIC_PLAN_CONTAINER.length > 1) {
            $scope.LC_LOGISTIC_PLAN_CONTAINER.splice(index, 1);
        }

    };


    function CommercialInvoice(lcnumber) {
        $scope.itemview = true;
        $scope.AirView = false;
        var response = LogisticPlanService.ItemsOnChange(lcnumber);
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
                    INPUT_QUANTITY: ''
                }];

                $scope.itemlist = data.data;
                $scope.ReserveTrackno = $scope.itemlist[0].LC_TRACK_NO;
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
                        });
                        $scope.tableitems[i].ITEM_CODE = $scope.itemlist[i].ITEM_CODE;
                        $scope.tableitems[i].SNO = $scope.itemlist[i].SNO;
                        $scope.tableitems[i].QUANTITY = $scope.itemlist[i].CALC_QUANTITY;
                        $scope.tableitems[i].AMOUNT = $scope.itemlist[i].CALC_UNIT_PRICE;
                        $scope.tableitems[i].MU_CODE = $scope.itemlist[i].MU_CODE;
                        $scope.tableitems[i].HS_CODE = $scope.itemlist[i].HS_CODE;
                        $scope.tableitems[i].COUNTRY_CODE = $scope.itemlist[i].COUNTRY_CODE;
                        $scope.tableitems[i].RECV_QUANITTY = $scope.itemlist[i].INVOICE_QUANTITY;
                    }
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
        }, function errorCallback(response) {
            displayPopupNotification("Error Occured.", "error");
        });
    };

    $scope.ItemsOnChange = function (e) {
     
        $scope.cleareditcontent();
        var lcnumber = e.dataItem.LC_NUMBER_CODE;
        $scope.ReloadCode = lcnumber;
        lcnumber = (lcnumber == undefined) ? null : lcnumber;
        showloader();
        lcnumber = escape(lcnumber);
        CommercialInvoice(lcnumber);
        hideloader();
    }

    $scope.removeIssue = function (index) {
     
        $scope.itemlist.splice(index, 1);
        $scope.tableitems.splice(index, 1);
        $scope.showreloaditems = true;
    }

    $scope.reloadIssue = function () {
     
        CommercialInvoice($scope.ReloadCode);
        $scope.showreloaditems = false;
    };


    $scope.ipPurchaseOrder = {
        dataType: "json",
        serverFiltering: true,
        filter: "startswith",
        transport: {
            read: function (options) {
                $.ajax({
                    url: "/api/LcLogisticPlan/GetAllLcIpPurchaseOrder?filters=" + options.data.filter.filters[0].value,
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
                    url: "/api/LcLogisticPlan/GetAllLcIpPurchaseOrderfilter?filters=" + options.data.filter.filters[0].value,
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
    



    $scope.shipmentTypeDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/LcLogisticPlan/GetShipmentType",
            }
        },
    };

    $scope.shipmentType = {
        optionLabel: "--Select Shipment Type--",
        dataSource: $scope.shipmentTypeDatasource,
    };

    $scope.containerDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/Logistics/GetAllContainer",
            }
        },
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
        //optionLabel: "--Select Load Type--",
        dataSource: $scope.shipmentLoadTypeDatasource,

       
    };


    $scope.ShippingOnChange = function (e) {
        $scope.showcarrier = true;
     

    }

    $scope.locationDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/Logistics/GetAllLocations",
            }
        },
    };
    function RefreshGrid() {
        $('#grid').data("kendoGrid").dataSource.read();
    };
    $scope.OnInputQuantityChange = function (items, i) {
     
        if (items.INVOICE_QUANTITY != null) {
            var quantity = items.CALC_QUANTITY - items.INVOICE_QUANTITY;
            if (quantity < $scope.itemlist[i].SHIPPMENT_QUANTITY) {
                $scope.itemlist[i].SHIPPMENT_QUANTITY = "";
                displayPopupNotification("Input quantity should be less than remaining quantity.", "warning");

            }
        }
        else {
            if (items.CALC_QUANTITY < $scope.itemlist[i].SHIPPMENT_QUANTITY) {
                $scope.itemlist[i].SHIPPMENT_QUANTITY = "";
                displayPopupNotification("Input quantity should be less than remaining quantity.", "warning");
            }
        }

    }

    $scope.createLCPlan = function (isValid) {

     
        //update data
        if ($scope.saveAction == "Update") {


            var RouteContainerlen = $scope.RouteContainer.length;
            var routelen = $scope.Route.length;
            $scope.LC_LOGISTIC_PLAN_CONTAINER = [];
            for (var i = 0; i < routelen; i++) {

                for (var j = 0; j < RouteContainerlen; j++) {

                    $scope.LC_LOGISTIC_PLAN_CONTAINER.push(
                        {
                            FROM_LOCATION_CODE: $scope.Route[i].FROM_LOCATION_CODE,
                            TO_LOCATION_CODE: $scope.Route[i].TO_LOCATION_CODE,
                            EST_BOOKING_DATE: $scope.Route[i].EST_BOOKING_DATE,
                            EST_LOADING_DATE: $scope.Route[i].EST_LOADING_DATE,
                            ACT_BOOKING_DATE: $scope.Route[i].ACT_BOOKING_DATE,
                            ACT_LOADING_DATE: $scope.Route[i].ACT_LOADING_DATE,
                            SHIPPING_TYPE: $scope.Route[i].SHIPPING_TYPE,
                            PLAN_CONTAINER_CODE: $scope.RouteContainer[j].PLAN_CONTAINER_CODE,
                            CONTAINER_CODE: $scope.RouteContainer[j].CONTAINER_CODE,
                            LOAD_TYPE: $scope.RouteContainer[j].LOAD_TYPE,
                            CARRIER_NUMBER: $scope.RouteContainer[j].CARRIER_NUMBER
                        }
                    )

                }

            }

            var LC_LOGISTIC_PLANVVIEWMODEL = {
                LC_TRACK_NO: $scope.LC_LOGISTIC_PLAN.LC_TRACK_NO,
                LC_NUMBER: $scope.LC_LOGISTIC_PLAN.LC_NUMBER,
                LOT_NO: $scope.LC_LOGISTIC_PLAN.LOT_NO,
                LOGISTIC_PLAN_CODE: $scope.LC_LOGISTIC_PLAN.LOGISTIC_PLAN_CODE,
                CONSIGNEE_NAME: $scope.LC_LOGISTIC_PLAN.CONSIGNEE_NAME,
                CONSIGNEE_ADDRESS: $scope.LC_LOGISTIC_PLAN.CONSIGNEE_ADDRESS,
                NOTIFY_APPLICANT_NAME: $scope.LC_LOGISTIC_PLAN.NOTIFY_APPLICANT_NAME,
                NOTIFY_APPLICANT_ADDRESS: $scope.LC_LOGISTIC_PLAN.NOTIFY_APPLICANT_ADDRESS,
                REMARKS: $scope.LC_LOGISTIC_PLAN.REMARKS,
                LC_LOGISTIC_PLAN_CONTAINER: $scope.LC_LOGISTIC_PLAN_CONTAINER
               
            };
            var url = window.location.protocol + "//" + window.location.host + "/api/LcLogisticPlan/UpdateLogisticPlan";
            var response = $http({
                method: "post",
                url: url,
                data: JSON.stringify(LC_LOGISTIC_PLANVVIEWMODEL),
                dataType: "json"
            });

            return response.then(function (result) {
                if (result.status == "200") {
                    $scope.cancelLCPlan();
                    RefreshGrid();
                     displayPopupNotification("Logistic Plan Updated Successfully.", "success");
                }
                else {
                    displayPopupNotification("Error Occured.", "error");
                }

            });

        }
        else {
        
            //insert data
            if ($scope.Route[0].SHIPPING_TYPE === "--Select Shipment Type--") {
                displayPopupNotification("Select Shippment Type", "warning");
                return;
            }
          if (isValid) {
              var RouteContainerlen = $scope.RouteContainer.length;
              for (var i = 0; i < RouteContainerlen; i++) {
                      if ($scope.RouteContainer[i].CONTAINER_CODE === "" || $scope.RouteContainer[i].CONTAINER_CODE === undefined || $scope.RouteContainer[i].CONTAINER_CODE === null) {
                        displayPopupNotification("Add Carrier Type", "warning");
                        return;
                        break;
                    }
                      if ($scope.RouteContainer[i].LOAD_TYPE === "" || $scope.RouteContainer[i].LOAD_TYPE === undefined || $scope.RouteContainer[i].LOAD_TYPE === null) {
                        displayPopupNotification("Add Load Type Number", "warning");
                        return;
                        break;
                    }
                }
              
               


              
               
               
                var ReserveTrackno = $scope.ReserveTrackno;
                $scope.LC_LOGISTIC_PLAN.LC_TRACK_NO = ReserveTrackno;
             
                var routelen = $scope.Route.length;
                $scope.LC_LOGISTIC_PLAN_CONTAINER = [];
                for (var i = 0; i < routelen; i++) {

                    for (var j = 0; j < RouteContainerlen; j++) {

                        $scope.LC_LOGISTIC_PLAN_CONTAINER.push(
                            {
                                LC_TRACK_NO: ReserveTrackno,
                                FROM_LOCATION_CODE: $scope.Route[i].FROM_LOCATION_CODE,
                                TO_LOCATION_CODE: $scope.Route[i].TO_LOCATION_CODE,
                                EST_BOOKING_DATE: $scope.Route[i].EST_BOOKING_DATE,
                                EST_LOADING_DATE: $scope.Route[i].EST_LOADING_DATE,
                                ACT_BOOKING_DATE: $scope.Route[i].ACT_BOOKING_DATE,
                                ACT_LOADING_DATE: $scope.Route[i].ACT_LOADING_DATE,
                                SHIPPING_TYPE: $scope.Route[i].SHIPPING_TYPE,
                                CONTAINER_CODE: $scope.RouteContainer[j].CONTAINER_CODE,
                                LOAD_TYPE: $scope.RouteContainer[j].LOAD_TYPE,
                                CARRIER_NUMBER: $scope.RouteContainer[j].CARRIER_NUMBER,
                                ROUTE_NO: i + 1
                            }
                        )

                    }

                }


                var LcLogisticPlanModel = {
                    LC_LOGISTIC_PLAN: $scope.LC_LOGISTIC_PLAN,
                    LC_LOGISTIC_PLAN_CONTAINER: $scope.LC_LOGISTIC_PLAN_CONTAINER,
                    LC_LOGISTIC_PLAN_ITEM: $scope.itemlist
                };


                var url = window.location.protocol + "//" + window.location.host + "/api/LcLogisticPlan/CreateLogisticPlan";
                var response = $http({
                    method: "post",
                    url: url,
                    data: JSON.stringify(LcLogisticPlanModel),
                    dataType: "json"
                });

                return response.then(function (data) {
                    $scope.ci.lcnumber = "";
                    $scope.cancelLCPlan();
                    RefreshGrid();

                    displayPopupNotification("Logistic Plan Saved Successfully.", "success");

                }, function errorCallback(response) {
                    displayPopupNotification(response.data, "error");
                });
            }
            else {
                displayPopupNotification("Fields cannot be empty.", "error");

            }
          
        }
       
    };

   
    $scope.FilterGrid = function (LC_NUMBER) {
        $("#grid").data("kendoGrid").tbody.find("tr").css("background-color", "");
        var data = $("#grid").data("kendoGrid").dataSource.data();
        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
            if (dataItem.LC_NUMBER == LC_NUMBER) {
                $("#grid").data("kendoGrid").tbody.find("tr[data-uid=" + dataItem.uid + "]").css("background-color", "#80ddaa");
            }
        }

    };

    $scope.mainGridOptions = {
        toolbar: ["excel"],
        dataSource: {
            type: "json",
            transport: {
              
                read: "/api/LcLogisticPlan/getAllLcLogisticPlan",
            },
            pageSize: 100,
            serverPaging: false,
            serverSorting: false,
            schema: {
                model: {
                    fields: {
                        LC_TRACK_NO: { type: "string" },
                        SUPPLIER_EDESC: { type: "string" },
                        CONSIGNEE_NAME: { type: "string" },
                        CONSIGNEE_ADDRESS: { type: "string" },
                        NOTIFY_APPLICANT_NAME: { type: "string" },
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
            $scope.EditLogisticPlan = function (dataItem) {
                $scope.saveAction = "Update";
                $scope.disablelcnumber = true;
                $scope.showitems = false;
                $scope.showFormPanel = true;
                $scope.showcontaineredit = true;
                $scope.itemview = true;
                $scope.AirView = false;
                $scope.showcarrier = true;
                $scope.disableeditcontent();
                $scope.ContentModal = false;
                $scope.Allcheckbox = false;
                var lc_number = dataItem.SUPPLIER_EDESC;
                $scope.FilterGrid(dataItem.LC_NUMBER);
                $("#lcnumberautocomplete").val(lc_number);
                $scope.LC_LOGISTIC_PLAN.LOGISTIC_PLAN_CODE = dataItem.LOGISTIC_PLAN_CODE;
                $scope.LC_LOGISTIC_PLAN.LOT_NO = dataItem.LOT_NO;
                $scope.LC_LOGISTIC_PLAN.LC_TRACK_NO = dataItem.LC_TRACK_NO;
                $scope.LC_LOGISTIC_PLAN.LC_NUMBER = dataItem.SUPPLIER_EDESC;
                $scope.LC_LOGISTIC_PLAN.CONSIGNEE_NAME = dataItem.CONSIGNEE_NAME;
                $scope.LC_LOGISTIC_PLAN.CONSIGNEE_ADDRESS = dataItem.CONSIGNEE_ADDRESS;
                $scope.LC_LOGISTIC_PLAN.NOTIFY_APPLICANT_NAME = dataItem.NOTIFY_APPLICANT_NAME;
                $scope.LC_LOGISTIC_PLAN.NOTIFY_APPLICANT_ADDRESS = dataItem.NOTIFY_APPLICANT_ADDRESS;
                $scope.LC_LOGISTIC_PLAN.REMARKS = dataItem.REMARKS;
                //GET ITEMS
                var lcnumber = dataItem.LC_NUMBER;
                $scope.ReloadCode = lcnumber;
                lcnumber = (lcnumber == undefined) ? null : lcnumber;
                //showloader();
          
                CommercialInvoice(lcnumber);
              
               

                //GET CONTAINER
                var url = window.location.protocol + "//" + window.location.host + "/api/LcLogisticPlan/GetUpdateShipmentData?LOGISTIC_PLAN_CODE=" + dataItem.LOGISTIC_PLAN_CODE;
                var response = $http({
                    method: "GET",
                    url: url,
                    dataType: "json"
                });

                return response.then(function (result) {
                     
                    $scope.Route[0].SHIPPING_TYPE = result.data[0].SHIPPING_TYPE;
                    $scope.Route[0].EST_BOOKING_DATE = result.data[0].EST_BOOKING_DATE;
                    $scope.Route[0].EST_LOADING_DATE = result.data[0].EST_LOADING_DATE;
                    $scope.Route[0].ACT_BOOKING_DATE = result.data[0].ACT_BOOKING_DATE;
                    $scope.Route[0].ACT_LOADING_DATE = result.data[0].ACT_LOADING_DATE;
                    $scope.Route[0].TO_LOCATION_CODE = result.data[0].TO_LOCATION_CODE;  //TO_LOCATION_EDESC
                    $scope.Route[0].FROM_LOCATION_CODE = result.data[0].FROM_LOCATION_CODE; //FROM_LOCATION_EDESC
                  
                    $scope.RouteContainer = [];
                    angular.forEach(result.data, function (value, index) {
                        $scope.RouteContainer.push({
                            PLAN_CONTAINER_CODE:result.data[index].PLAN_CONTAINER_CODE,
                            CONTAINER_CODE: result.data[index].CONTAINER_CODE,
                            LOAD_TYPE: result.data[index].LOAD_TYPE,
                            CARRIER_NUMBER: result.data[index].CARRIER_NUMBER
                        });
                    });

                 });
             

            }
            $('div').removeClass('.k-header k-grid-toolbar');
            $scope.disablelcnumber = false;
           
        },

        columns: [

            {
                field: "LC_TRACK_NO",
                title: "Track Number",
                attributes: { style: "text-align:right;" },
                width: "120px"
                
            },
            {
                field: "LOGISTIC_PLAN_CODE",
                title: "LOGISTIC_PLAN_CODE",
                hidden: true,

            },
            {
                field: "SUPPLIER_EDESC",
                title: "LC Number",
                width: "210px"
            },


            {
                field: "SNO",
                title: "Plan No",
                width: "90px"
            },
            {
                field: "CARRIER_COUNT",
                title: "No of Carrier",
                width: "120px"
            },
            {
                field: "SHIPPING_TYPE",
                title: "Shipping Type",
                width: "100px"
            },
            {
                field: "CONSIGNEE_NAME",
                title: "Consignee Name",
            },

            {
                field: "CONSIGNEE_ADDRESS",
                title: "Consignee Address",

            },
            {
                field: "NOTIFY_APPLICANT_NAME",
                title: "Notify Applicant Name",
            },
            {
                field: "NOTIFY_APPLICANT_ADDRESS",
                title: "Notify Applicant Address",
            }
            ,

            {
                field: "REMARKS",
                title: "Remarks",
            }
            ,
            {
                field: "ID", title: "Action", sortable: false, filterable: false,
                template: '<a class="edit glyphicon glyphicon-edit" title="Edit" ng-click="EditLogisticPlan(dataItem)" style="color:grey;"><span class="sr-only"></span> </a> '
            }
        ]
    };

 
    $scope.AllcheckboxEnableDisable = function () {

        if ($scope.Allcheckbox) {
            $scope.cleareditcontent();
        } else {
            $scope.disableeditcontent();
        }
    }
    $scope.disableeditcontent = function () {
        $scope.check_SHIPPING_TYPE = false;
        $scope.check_EST_BOOKING_DATE = false;
        $scope.check_EST_LOADING_DATE = false;
        $scope.check_ACT_LOADING_DATE = false;
        $scope.check_ACT_BOOKING_DATE = false;
        $scope.check_FROM_LOCATION_CODE = false;
        $scope.check_TO_LOCATION_CODE = false;
        $scope.editcontainer = false;
                   
    };


    $scope.cleareditcontent = function () {
        $scope.check_SHIPPING_TYPE = true;
        $scope.check_EST_BOOKING_DATE = true;
        $scope.check_EST_LOADING_DATE = true;
        $scope.check_ACT_LOADING_DATE = true;
        $scope.check_ACT_BOOKING_DATE = true;
        $scope.check_FROM_LOCATION_CODE = true;
        $scope.check_TO_LOCATION_CODE = true;
        $scope.editcontainer = true;
     
    };

    $scope.OnEtdBookingDateChange = function (bookingDate) {
     
        var bdate = $('#etdBookingDate_' + bookingDate).val();
        var ldate = $('#etdLoadingingDate_' + bookingDate).val();
        if (ldate != "" && bdate != "") {
            if (ldate < bdate) {
                $('#etdBookingDate_' + bookingDate).val("");
                displayPopupNotification("Loading Date Must Be Greater Than Booking Date!", "warning");
                
                return;
            }
        }


    };

    $scope.OnEtdLoadingDateChange = function (loadingDate) {
     
        var bdate = $('#etdBookingDate_' + loadingDate).val();
        var ldate = $('#etdLoadingingDate_' + loadingDate).val();
        if (ldate != "" && bdate != "") {
            if (ldate < bdate) {
                $('#etdLoadingingDate_' + loadingDate).val("");
                displayPopupNotification("Loading Date Must Be Greater Than Booking Date!", "warning");
                return;
            }
        }
    };

    $scope.OnCarrierNumberChange = function (carrierNumber) {
        var previousCarrierNumber = $('#carrierNumber_' + carrierNumber).val();
        angular.forEach($scope.RouteContainer, function (value, index) {
            if (index != carrierNumber) {
                if (previousCarrierNumber == $scope.RouteContainer[index].CARRIER_NUMBER && $scope.RouteContainer[index].CARRIER_NUMBER!="") {
                    $('#carrierNumber_' + carrierNumber).val("");
                    displayPopupNotification("Two carrier number can not be same!", "warning");
                    return; 
                }                        
            }

        });
    };
   
    $scope.OnCarrierTypeChange = function (e,index) {
        var carrier = e.dataItem.CONTAINER_EDESC;
        if (carrier === "Truck") {
            $(".shipmentload_" + index).attr('disabled', true);
              $scope.RouteContainer[index].LOAD_TYPE = "FCL";
        } else {
            $(".shipmentload_" + index).attr('disabled', false);
            $scope.RouteContainer[index].LOAD_TYPE = "";
        }
     
    };
    $scope.itemGridOptions = function (dataItem) {

        return {
            dataSource: {
                type: "json",
                transport: {

                    read: {
                        url: "/api/LcLogisticPlan/getAllLcLogisticPlanItemListByTrackNumberAndLogisticPlanCode?trackNumber=" + dataItem.LC_TRACK_NO + "&logisticPlanCode=" + dataItem.LOGISTIC_PLAN_CODE,
                        dataType: "json"
                    },
                    update: {
                        url: window.location.protocol + "//" + window.location.host + "/api/LcLogisticPlan/UpdateQuantity",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        complete: function (e) {
                         
                            if (JSON.parse(e.responseText).MESSAGE == "EXCEEDED") {
                                $(".itemgrid").data("kendoGrid").dataSource.read();
                                displayPopupNotification("Entered Quantity Exceeded total quantity.", "warning");
                            }
                            else if (JSON.parse(e.responseText).MESSAGE == "UPDATED") {
                                $(".itemgrid").data("kendoGrid").dataSource.read();
                                displayPopupNotification("Successfully Updated Item Quantity.", "success");
                            }
                            else if (JSON.parse(e.responseText).MESSAGE == "FAILED") {
                                $(".itemgrid").data("kendoGrid").dataSource.read();
                                displayPopupNotification("Failed To Update Item Quantity,quantity is greater than total quantity.", "warning");
                            }
                            else {
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
                            //displayPopupNotification("Quantity is greater than total quantity.", "warning");
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
                            LC_TRACK_NO: { editable: false },
                            ITEM_CODE: { editable: false },
                            ITEM_NAME: { editable: false },
                            ITEM_CODE: { editable: false },
                            QUANTITY: { type: "number", validation: { min: 0, required: true } },
                            TOTAL_QUANTITY: { editable: false },
                            HS_CODE: { editable: false },
                            COUNTRY_EDESC: { editable: false },
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
                //{ title: "SN", template: "#= ++record #", width: "10px", attributes: { style: "text-align:right;" } },
                { field: "LC_TRACK_NO", title: "Track Number", width: "20px", attributes: { style: "text-align:right;" } },
                { field: "ITEM_CODE", title: "Item Code", width: "20px" },
                { field: "ITEM_NAME", title: "Item Name", attributes: { style: "text-align:right;" }, width: "40px" },
                { field: "QUANTITY", title: "Quantity", attributes: { style: "text-align:right;" }, width: "20px" },
                { field: "TOTAL_QUANTITY", title: "Total Quantity", attributes: { style: "text-align:right;" }, width: "20px" },
                { field: "HS_CODE", title: "HS Code", attributes: { style: "text-align:right;" }, width: "20px" },
                { field: "COUNTRY_EDESC", title: "Country", attributes: { style: "text-align:right;" }, width: "20px" },
                { command: ["edit"], title: "Action", width: "40px" },
            ],
            editable: "inline",
        };
    };

    $scope.containerGridOptions = function (dataItem) {
     
        return {
            dataSource: {
                type: "json",
                transport: {

                    read: {
                     
                        url: "/api/LcLogisticPlan/getAllLcLogisticPlanContainerListByTrackNumberAndLogisticPlanCode?trackNumber=" + dataItem.LC_TRACK_NO + "&lotNumber=" + dataItem.LOT_NO,
                        dataType: "json"
                    },
                  
                },
                serverPaging: false,
                serverSorting: false,
                serverFiltering: false,
                pageSize: 50,
            
            },
          
            sortable: true,
            pageable: true,

            dataBound: function (e) {

            },
            dataBinding: function () {
                record = (this.dataSource.page() - 1) * this.dataSource.pageSize();
            },

            columns: [
              
                { field: "SHIPPING_TYPE", title: "Shipping Type", width: "30px" },
                { field: "EST_BOOKING_DATE", title: "EST Booking Date", width: "30px" },
                { field: "EST_LOADING_DATE", title: "EST Loading Date", width: "20px" },
                { field: "ACT_BOOKING_DATE", title: "Actual Booking Date", width: "30px" },
                { field: "ACT_LOADING_DATE", title: "Actual Loading Date", width: "20px" },
                { field: "CONTAINER_EDESC", title: "Carrier", width: "20px" },
                { field: "FROM_LOCATION", title: "From Location", width: "20px" },
                { field: "TO_LOCATION", title: "To Location", width: "20px" },
               
            ],
            
        };
    };
});


app.service('LogisticPlanService', function ($http) {
    var fac = {};
    fac.ItemsOnChange = function (lcnumber) {

        var response = $http({
            method: "get",
            url: "/api/LcLogisticPlan/GetLCLogisticPlanItemsByLCNumber?lcnumber=" + lcnumber,
            dataType: "json"
        });
        return response;
    }

    return fac;

});


