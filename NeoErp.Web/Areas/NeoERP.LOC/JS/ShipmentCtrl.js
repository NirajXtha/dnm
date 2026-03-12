app.controller('ShipmentCtrl', function ($scope, $http, $sce, $filter, $q, ShipmentService) {
    $scope.ItemsOnChange = "";
    $scope.itemlist = [];
    $scope.countrylist = [];
    $scope.hslist = [];
    $scope.beneficiarylist = [];
    $scope.shipmentType = [];
    $scope.shipmentLoad = [];
    $scope.intmbankOptions = [];
    $scope.deliveryPlaceType = [];
    $scope.finalshipmentlist = [];
    $scope.addbutton = true;
    $scope.saveAction = "Save";
    $scope.hidefirstrow = true;
    $scope.noitems = true;
    $scope.additems = false;
    $scope.ItemsShow = true;
    $scope.allitemslist = [];
    $scope.scAction = "Save & Continue";
    $scope.shipmentfieldsdetails = [];

    $scope.si = {
        awb: '',
        grosswhtair: '',
        chargeablewhtair: '',
        fromlocation: '',
        tolocation: '',
        shipmentType: '',
        shipmentload: '',
        shipmentstatus: '',
        containersize: '',
        containernumber: '',
        estimateday: '',
        shippername: '',
        shipperaddress: '',
        consigneename: '',
        consigneeaddress: '',
        issuingcarrername: '',
        issuingcarreraddress: '',
        notifyapplicantname: '',
        notifyapplicantaddress: '',
        remarks: '',
        lading: '',
        grossweightsea: '',
        cbmsea: '',
        vesselno: '',
        road: '',
        truck: '',
        transname: '',
        transaddress: '',
        train: '',

    };

    $scope.cancelsi = function () {
        $scope.si.awbdate = $filter('date')(new Date, "MM/dd/yyyy");
        $scope.si.ladingdate = $filter('date')(new Date, "MM/dd/yyyy");
        $scope.si.roaddate = $filter('date')(new Date, "MM/dd/yyyy");
        $scope.si.traindate = $filter('date')(new Date, "MM/dd/yyyy");
        $scope.lctrackno = 0;
        $scope.sno = 0;
        $scope.si = {};
        $scope.onitemsedit = false;
        $scope.itemlist = [];
    }

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

    $scope.createshipment = function (isValid) {
        $scope.lctrackno;
        $scope.itemlist;
        if (isValid) {
            var sientryurl = window.location.protocol + "//" + window.location.host + "/api/Shipment/CreateShipment";
            var sidetails = {
                INVOICE_NO: $scope.si.perfomainvoice,
                SNO: $scope.sno,
                AWB_NO: $scope.si.awb,
                AWB_DATE: $scope.si.awbdate,
                LC_TRACK_NO: $scope.lctrackno,
                GROSS_WEIGHT_AIR: $scope.si.grosswhtair,
                CHARGEABLE_WEIGHT_AIR: $scope.si.chargeablewhtair,
                FROM_LOCATION: $scope.si.fromlocation,
                TO_LOCATION: $scope.si.tolocation,
                SHIPMENT_TYPE: $scope.si.shipmentType,
                LOAD_TYPE: $scope.si.shipmentload,
                SHIPMENT_STATUS: $scope.si.shipmentstatus,
                CONTAINER_SIZE: $scope.si.containersize,
                CONTAINER_NO: $scope.si.containernumber,
                EST_DAY: $scope.si.estimateday,
                SHIPPER_NAME: $scope.si.shippername,
                SHIPPER_ADDRESS: $scope.si.shipperaddress,
                CONSIGNEE_NAME: $scope.si.consigneename,
                CONSIGNEE_ADDRESS: $scope.si.consigneeaddress,
                ISSUING_CARRIER_NAME: $scope.si.issuingcarrername,
                ISSUING_CARRIER_ADDRESS: $scope.si.issuingcarreraddress,
                NOTIFY_APPLICANT_NAME: $scope.si.notifyapplicantname,
                NOTIFY_APPLICANT_ADDRESS: $scope.si.notifyapplicantaddress,
                REMARKS: $scope.si.remarks,
                BL_NO: $scope.si.lading,
                BL_DATE: $scope.si.ladingdate,
                GROSS_WEIGHT_SEA: $scope.si.grossweightsea,
                CBM_SEA: $scope.si.cbmsea,
                VESSEL_NO: $scope.si.vesselno,
                LR_NO: $scope.si.road,
                LR_DATE: $scope.si.roaddate,
                TRUCK_NO: $scope.si.truck,
                TRANSPORTER_NAME: $scope.si.transname,
                TRANSPORTER_ADDRESS: $scope.si.transaddress,
                RR_NO: $scope.si.train,
                RR_DATE: $scope.si.traindate,
                ITEMLIST: $scope.itemlist,
            };

            var response = $http({
                method: "post",
                url: sientryurl,
                data: sidetails,
                dataType: "json"
            });

            return response.then(function (data) {

                $scope.onitemsedit = false;
                $scope.editbutton = true;
                $scope.addbutton = false;
                displayPopupNotification("Items Shipment Updated Successfully.", "success");

            }, function errorCallback(response) {


                displayPopupNotification(response.data, "error");


            });
        }
        else {
            displayPopupNotification("Fields cannot be empty.", "error");

        }
    }

    $scope.OnChangeShipmentType = function(e)
    {
        var SHIPMENT_TYPE = e.sender.dataItem();
        if (SHIPMENT_TYPE == "AIR") {
            $scope.showforair = true;
            $scope.showforroad = false;
            $scope.showforsea = false;
            $scope.showfortrain = false;
            $scope.showforother = false;
        }
        else if (SHIPMENT_TYPE == "SEA") {
            $scope.showforsea = true;
            $scope.showforair = false;
            $scope.showforroad = true;
            $scope.showfortrain = true;
            $scope.showforother = true;
        }
        else if (SHIPMENT_TYPE == "TRAIN") {
            $scope.showfortrain = true;
            $scope.showforroad = true;
            $scope.showforsea = false;
            $scope.showforair = false;
            $scope.showforother = true;
        }
        else if (SHIPMENT_TYPE == "ROAD") {
            $scope.showforroad = true;
            $scope.showforsea = false;
            $scope.showforair = false;
            $scope.showfortrain = false;
            $scope.showforother = true;
        }

        else {
            $scope.showforother = true;
            $scope.showforroad = false;
            $scope.showforsea = false;
            $scope.showforair = false;
            $scope.showfortrain = false;
        }
    }

    $scope.EditShipmentItems = function (items) {
        $scope.si.awbdate = $filter('date')(new Date(), "MM/dd/yyyy");
        $scope.si.ladingdate = $filter('date')(new Date(), "MM/dd/yyyy");
        $scope.si.roaddate = $filter('date')(new Date(), "MM/dd/yyyy");
        $scope.si.traindate = $filter('date')(new Date(), "MM/dd/yyyy");
        $scope.sno = items.SNO;
        var response = $http({
            method: "get",
            url: "/api/Shipment/GetShipmentDetailsbySno?sno=" + $scope.sno + "&lctrackno=" + $scope.lcetracknumberforlc,
            dataType: "json"
        });
        return response.then(function (data) {

            if (data.data.length == 0) {

            }
            else {
                $scope.si.awb = data.data.AWB_NO;
                $scope.si.awbdate = $filter('date')(data.data.AWB_DATE, "MM/dd/yyyy");
                $scope.si.grosswhtair = data.data.GROSS_WEIGHT_AIR;
                $scope.si.chargeablewhtair = data.data.CHARGEABLE_WEIGHT_AIR;
                $scope.si.fromlocation = data.data.FROM_LOCATION;
                $scope.si.tolocation = data.data.TO_LOCATION;
                $scope.si.shipmentType = data.data.SHIPMENT_TYPE;
                $scope.si.shipmentload = data.data.LOAD_TYPE;
                $scope.si.shipmentstatus = data.data.SHIPMENT_STATUS;
                $scope.si.containersize = data.data.CONTAINER_SIZE;
                $scope.si.containernumber = data.data.CONTAINER_NO;
                $scope.si.estimateday = data.data.EST_DAY;
                $scope.si.shippername = data.data.SHIPPER_NAME;
                $scope.si.shipperaddress = data.data.SHIPPER_ADDRESS;
                $scope.si.consigneename = data.data.CONSIGNEE_NAME;
                $scope.si.consigneeaddress = data.data.CONSIGNEE_ADDRESS;
                $scope.si.issuingcarrername = data.data.ISSUING_CARRIER_NAME;
                $scope.si.issuingcarreraddress = data.data.ISSUING_CARRIER_ADDRESS;
                $scope.si.notifyapplicantname = data.data.NOTIFY_APPLICANT_NAME;
                $scope.si.notifyapplicantaddress = data.data.NOTIFY_APPLICANT_ADDRESS;
                $scope.si.remarks = data.data.REMARKS;
                $scope.si.lading = data.data.BL_NO;
                $scope.si.ladingdate = $filter('date')(data.data.BL_DATE, "MM/dd/yyyy");
                $scope.si.grossweightsea = data.data.GROSS_WEIGHT_SEA;
                $scope.si.cbmsea = data.data.CBM_SEA;
                $scope.si.vesselno = data.data.VESSEL_NO;
                $scope.si.road = data.data.LR_NO;
                $scope.si.roaddate = $filter('date')(data.data.LR_DATE, "MM/dd/yyyy");
                $scope.si.truck = data.data.TRUCK_NO;
                $scope.si.transname = data.data.TRANSPORTER_NAME;
                $scope.si.transaddress = data.data.TRANSPORTER_ADDRESS;
                $scope.si.train = data.data.RR_NO;
                $scope.si.traindate = $filter('date')(data.data.RR_DATE, "MM/dd/yyyy");
                $scope.onitemsedit = true;
                if (items.SHIPMENT_TYPE == "AIR") {
                    $scope.showforair = true;
                    $scope.showforroad = false;
                    $scope.showforsea = false;
                    $scope.showfortrain = false;
                    $scope.showforother = false;
                }
                else if (items.SHIPMENT_TYPE == "SEA") {
                    $scope.showforsea = true;
                    $scope.showforair = false;
                    $scope.showforroad = true;
                    $scope.showfortrain = true;
                    $scope.showforother = true;
                }
                else if (items.SHIPMENT_TYPE == "TRAIN") {
                    $scope.showfortrain = true;
                    $scope.showforroad = true;
                    $scope.showforsea = false;
                    $scope.showforair = false;
                    $scope.showforother = true;
                }
                else if (items.SHIPMENT_TYPE == "ROAD") {
                    $scope.showforroad = true;
                    $scope.showforsea = false;
                    $scope.showforair = false;
                    $scope.showfortrain = false;
                    $scope.showforother = true;
                }

                else {
                    $scope.showforother = true;
                    $scope.showforroad = false;
                    $scope.showforsea = false;
                    $scope.showforair = false;
                    $scope.showfortrain = false;
                }
            }
        }, function errorCallback(response) {
            displayPopupNotification("Error Occured.", "error");
        });

    }

    $scope.AddShipmentItems = function (items) {
        $scope.lctrackno = items.LC_TRACK_NO;
        $scope.onitemsedit = true;
       
        if (items.SHIPMENT_TYPE == "AIR") {
            $scope.showforair = true;
            $scope.showforroad = false;
            $scope.showforsea = false;
            $scope.showfortrain = false;
            $scope.showforother = false;
        }
        else if (items.SHIPMENT_TYPE == "SEA") {
            $scope.showforsea = true;
            $scope.showforair = false;
            $scope.showforroad = true;
            $scope.showfortrain = true;
            $scope.showforother = true;
        }
        else if (items.SHIPMENT_TYPE == "TRAIN") {
            $scope.showfortrain = true;
            $scope.showforroad = true;
            $scope.showforsea = false;
            $scope.showforair = false;
            $scope.showforother = true;
        }
        else if (items.SHIPMENT_TYPE == "ROAD") {
            $scope.showforroad = true;
            $scope.showforsea = false;
            $scope.showforair = false;
            $scope.showfortrain = false;
            $scope.showforother = true;
        }

        else {
            $scope.showforother = true;
            $scope.showforroad = false;
            $scope.showforsea = false;
            $scope.showforair = false;
            $scope.showfortrain = false;
        }
     
    }

    $scope.ItemsOnChange = function (options) {
        $scope.onitemsedit = false;
        var perfomainvoiceno = options.dataItem.INVOICE_CODE;
        perfomainvoiceno = (perfomainvoiceno == undefined) ? null : perfomainvoiceno;
        showloader();
        var response = $http({
            method: "get",
            url: "/api/Shipment/getShipmentDetailsByPinvoice?PinvoiceCode=" + perfomainvoiceno,
            dataType: "json"
        });
        return response.then(function (data) {
            
            if (data.data.length == 0) {

                $scope.itemlist = [];
            }
            else {
                
                if (data.data[0].DETAIL_ENTRY_FLAG == 'Y') {
                    $scope.editbutton = true;
                    $scope.addbutton = false;
                }
                else {
                    $scope.editbutton = false;
                    $scope.addbutton = true;
                }
                $scope.itemlist = data.data;
                $scope.lcetracknumberforlc = data.data[0].LC_TRACK_NO;
                $scope.pinvoicecodeforlc = data.data[0].PINVOICE_CODE;

            }
            hideloader();
        }, function errorCallback(response) {
            displayPopupNotification("Fields should not be empty.", "error");
            hideloader();
        });
    }

    $scope.ipPurchaseOrder = {
        dataType: "json",
        serverFiltering: true,
        filter: "startswith",
        transport: {
            read: function (options) {
                $.ajax({
                    url: "/api/Shipment/GetAllLcIpPurchaseOrder?filter=" + options.data.filter.filters[0].value,
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

    $scope.tableitems = [{
        itemcode: '',
        quantity: '',
        total: '',
        value: '',
        hscode: '',
        country: '',

    }];
    $scope.tablelist = [{
        ITEM_CODE: '',
        CALC_QUANTITY: '',
        CALC_UNIT_PRICE: '',
        VALUE: '',
        HS_CODE: '',
        MU_CODE: '',
        COUNTRY_CODE: '',
    }]
    $scope.issues = [{
        sn: 1,
        name: '',
        quantity: '',
        value: '',
        total: '',
        hscode: '',
        country: ''
    }]


});

app.service('ShipmentService', function ($http) {
    var fac = {};


    return fac;

});



