app.controller('LogisticsCtrl', function ($scope, $http, $sce, $filter, $q, ShipmentService) {
    $scope.docAction = null;
    $scope.docName = null;
    $scope.docNameList = [];
    $scope.docActionList = [];
    $scope.uploadfields = [];
    $scope.shipmentdetails = [];
    $scope.ItemsOnChange = "";
    $scope.itemlist = [];
    $scope.perfomainvoiceno = "";
    $scope.items = [];
    $scope.ShowContainer = false;
    $scope.HideShippment = false;
    $scope.editcontent = false;
    $scope.countrylist = [];
    $scope.hslist = [];
    $scope.beneficiarylist = [];
    $scope.shipmentType = [];
    $scope.shipmentLoad = [];
    $scope.intmbankOptions = [];
    $scope.deliveryPlaceType = [];
    $scope.finalshipmentlist = [];
    $scope.showforcommon = false;
    $scope.logisticCode = 0;
    $scope.saveAction = "Save";
    $scope.allitemslist = [];
    $scope.shipmentfieldsdetails = [];
    $scope.ContentModal = true;
    $scope.Allcheckbox = false;
    $scope.DisableShipmentType = false;
  
    $scope.DOC = {
        DOC_REC_SUP_DATE: "",
        DOC_END_BANK_DATE: "",
        DOC_SEND_TOAGENT_DATE: "",
        POSTAGE_CN: "",
        POST_CLEAR_DATE: ""
    };
    $scope.LC_LOGISTIC_CONTAINER =
        [{
            LC_LOGISTIC_CON_CODE: "",
            LC_TRACK_NO: "",
            INVOICE_NO: "",
            LOGISTIC_PLAN_CODE: "",
            CONTAINER_CODE: "",
            LOAD_TYPE: "",
            DETENTION_DAYS: "",
            DEFAULT_AMOUNT: "",
            PER_DAY_CHRAGE: ""
        }];
    $scope.filterData = {
        InvoiceNumber: ""
    };
  

    $scope.add_childRouteContainer_element = function (index) {

        $scope.LC_LOGISTIC_CONTAINER.push({
            LC_LOGISTIC_CON_CODE: "",
            CONTAINER_CODE: "",
            LOAD_TYPE: "",
            LOGISTIC_PLAN_CODE: $scope.LC_LOGISTIC_CONTAINER[0].LOGISTIC_PLAN_CODE,
            LC_TRACK_NO: $scope.LC_LOGISTIC_CONTAINER[0].LC_TRACK_NO,
            DETENTION_DAYS: "",
            DEFAULT_AMOUNT: "",
            PER_DAY_CHRAGE: ""
        });

    }



    $scope.remove_childRouteContainer_element = function (index) {

        if ($scope.LC_LOGISTIC_CONTAINER.length > 1) {
            $scope.LC_LOGISTIC_CONTAINER.splice(index, 1);
        }


    };

    $scope.containerDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/Logistics/GetAllContainer",
            }
        },
    };




    $scope.shipmentLoad = {
        //optionLabel: "--Select Load Type--",
        dataSource: $scope.shipmentLoadTypeDatasource,
    };
    //vend

    $scope.commonColumns = function () {
        $scope.fromLocation = "";
        $scope.toLocation = "";
       
        $scope.SRC_ETA = "";
        $scope.SRC_ETD = "";
        $scope.SRC_ATA = "";
        $scope.SRC_ATD = "";
        $scope.SRC_ETD_DES = "";

        $scope.SRC_ETD_DES = "";
        $scope.DES_ETD = "";
        $scope.DES_ATA = "";
        $scope.DES_ATD = "";
        $scope.DES_EDD = "";

        $scope.contractorName = "";
        $scope.contractorAddress = "";
        $scope.contractAmt = "";
        $scope.contractDate = "";
        $scope.jobOrderNo = "";
        $scope.shipperName = "";
        $scope.shipperAddress = "";

        $scope.DOC.DOC_REC_SUP_DATE = "";
        $scope.DOC.DOC_END_BANK_DATE = "";
        $scope.DOC.DOC_SEND_TOAGENT_DATE = "";
        $scope.DOC.POSTAGE_CN = "";
        $scope.DOC.PORT_CLEAR_DATE = "";

    }
    var serialno = 1;
    $scope.addIssue = function () {
        $scope.uploadfields;
        $scope.uploadfields.push({
            SN: serialno,
            DOC_NAME: '',
            DOC_DATE: '',
            DOC_ACTION: 'PREPARE',
            DOC_PREPARED_DATE: '',
            DOC_EST_RECEIVED_DATE: '',
            DOC_RECEIVED_DATE: '',
            DOC_EST_SUBMITTED_DATE: '',
            DOC_SUBMITTED_DATE: '',
        });
        serialno++;
        $scope.$apply();
    }
    $scope.AddLogistics = function () {

        $scope.oneditshow = false;
        $scope.saveAction = "Save";
        $scope.showFormPanel = true;
        $scope.ShowContainer = false;
        $scope.HideShippment = false;
        $scope.ContentModal = true;
        $("#shipmentType").data("kendoDropDownList").enable(true);
        $scope.cleareditcontent();

    }
    $scope.RemoveIssue = function (index, totallengths) {

        if (totallengths == 0) {
            $scope.uploadfields = [];
        }
        $scope.uploadfields;
        var indexes = index - 1;
        $scope.uploadfields.splice(indexes, 1);
        $scope.$apply();

    }


    $scope.si = {
        awb: '',
        grosswhtair: '',
        chargeablewhtair: '',
        fromlocation: '',
        tolocation: '',
        shipmentType: '',
        shipmentload: "LCL",
        load: "LCL",
        shipmentstatus: '',
        container: '',
        estimateday: '',
        shippername: '',
        shipperaddress: '',
        consigneename: '',
        consigneeaddress: '',
        issuingcarrer: '',
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
        airpack: ''

    };

    $scope.cancelsi = function () {
        $scope.commonColumns();
        $scope.si.awbdate = '';
        $scope.si.ladingdate = '';
        $scope.si.roaddate = '';
        $scope.si.traindate = '';
        $scope.contractorCode = '';
        $scope.clearingAgent = "";
        $scope.si.shipmentload = "LCL";
        $scope.si.load = "LCL";
        $scope.lctrackno = 0;
        $scope.sno = 0;
        $scope.si = {};
        $scope.onitemsedit = false;
        $scope.itemlist = [];
        $scope.documentupload = false;
        $scope.showFormPanel = false;
        $scope.editextradocument = true;
        $scope.cleareditcontent();
        $scope.LogisticContainerPlan = [];
        $scope.uploadfields = [];
        $('.dropzone')[0].dropzone.files.forEach(function (file) {
            file.previewElement.remove();
        });
        $("#fromlocation").data("kendoDropDownList").value("");
        $("#tolocation").data("kendoDropDownList").value("");
        $('.dropzone').removeClass('dz-started');
        $("#grid").data("kendoGrid").tbody.find("tr").removeClass('highlight');
    }



    $scope.disableeditcontent = function () {
        $scope.si_check_awb = false;
        $scope.si_check_awbdate = false;
        $scope.si_check_grosswhtair = false;
        $scope.si_check_chargeablewhtair = false;
        $scope.si_check_airpack = false;
        $scope.si_check_lading = false;
        $scope.si_check_ladingdate = false;
        $scope.si_check_grossweightsea = false;
        $scope.si_check_cbmsea = false;
        $scope.si_check_vesselno = false;
        $scope.si_check_road = false;
        $scope.si_check_roaddate = false;
        $scope.si_check_truck = false;
        $scope.si_check_transname = false;
        $scope.si_check_transaddress = false;
        $scope.si_check_train = false;
        $scope.si_check_traindate = false;
        $scope.si_check_fromLocation = false;
        $scope.si_check_toLocation = false;

        $scope.si_check_SRC_ETA = false;
        $scope.si_check_SRC_ATA = false;
        $scope.si_check_SRC_ETD_DES = false;
        $scope.si_check_SRC_ETD = false;
        $scope.si_check_SRC_ATD = false;

        $scope.si_check_DES_ETA = false;
        $scope.si_check_DES_ETD = false;
        $scope.si_check_DES_ATA = false;
        $scope.si_check_DES_ATD = false;
        $scope.si_check_DES_ETD_NEXT_DES = false;

        $scope.editextradocument = false;

        $scope.si_check_contractorCode = false;
        $scope.si_check_contractorAddress = false;
        $scope.si_check_contractAmt = false;
        $scope.si_check_contractDate = false;
        $scope.si_check_jobOrderNo = false;
        $scope.si_check_shipperName = false;
        $scope.si_check_shipperAddress = false;
        $scope.si_check_issuingcarrer = false;
        $scope.si_check_clearingAgent = false;
        $scope.si_check_remarks = false;
        $scope.editcontainer = false;
        $scope.editdocument = false;
    };


    $scope.cleareditcontent = function () {
        $scope.si_check_awb = true;
        $scope.si_check_awbdate = true;
        $scope.si_check_grosswhtair = true;
        $scope.si_check_chargeablewhtair = true;
        $scope.si_check_airpack = true;
        $scope.si_check_lading = true;
        $scope.si_check_ladingdate = true;
        $scope.si_check_grossweightsea = true;
        $scope.si_check_cbmsea = true;
        $scope.si_check_vesselno = true;
        $scope.si_check_road = true;
        $scope.si_check_roaddate = true;
        $scope.si_check_truck = true;
        $scope.si_check_transname = true;
        $scope.si_check_transaddress = true;
        $scope.si_check_train = true;
        $scope.si_check_traindate = true;
        $scope.si_check_fromLocation = true;
        $scope.si_check_toLocation = true;
       
        $scope.si_check_SRC_ETA = true;
        $scope.si_check_SRC_ATA = true;
        $scope.si_check_SRC_ETD_DES = true;
        $scope.si_check_SRC_ETD = true;
        $scope.si_check_SRC_ATD = true;

        $scope.si_check_DES_ETA = true;
        $scope.si_check_DES_ETD = true;
        $scope.si_check_DES_ATA = true;
        $scope.si_check_DES_ATD = true;
        $scope.si_check_DES_ETD_NEXT_DES = true;

        $scope.editextradocument = true;

        $scope.si_check_contractorCode = true;
        $scope.si_check_contractorAddress = true;
        $scope.si_check_contractAmt = true;
        $scope.si_check_contractDate = true;
        $scope.si_check_jobOrderNo = true;
        $scope.si_check_shipperName = true;
        $scope.si_check_shipperAddress = true;
        $scope.si_check_issuingcarrer = true;
        $scope.si_check_clearingAgent = true;
        $scope.si_check_remarks = true;
        $scope.editcontainer = true;
        $scope.editdocument = true;
    };


    $scope.ClearOnItemChange = function () {
        $scope.commonColumns();
        $scope.si.awbdate = '';
        $scope.si.ladingdate = '';
        $scope.si.roaddate = '';
        $scope.si.traindate = '';
        $scope.contractorCode = '';
        $scope.clearingAgent = "";
        $scope.si.shipmentload = "LCL";
        $scope.si.load = "LCL";
        $scope.lctrackno = 0;
        $scope.sno = 0;
        var perfomainvoice = $scope.si.perfomainvoice;
        var lctrackno = $scope.lctrackno;
        $scope.si = {};
        $scope.si.perfomainvoice = perfomainvoice;
        $scope.lctrackno = lctrackno;
        $scope.onitemsedit = false;
        $scope.itemlist = [];
        $scope.documentupload = false;
        $scope.showFormPanel = true;
        $scope.editextradocument = true;
        $scope.LogisticContainerPlan = [];
        $scope.uploadfields = [];
        $('.dropzone')[0].dropzone.files.forEach(function (file) {
            file.previewElement.remove();
        });

        $('.dropzone').removeClass('dz-started');
    }

    $scope.shipmentTypeDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/Logistics/GetShipmentType",
            }
        },
    };

    $scope.shipmentType = {
        optionLabel: "--Select Shipment Type--",
        dataSource: $scope.shipmentTypeDatasource,


    };

    $scope.docActionDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/Logistics/GetDocActionType",

            }
        },
    };


    $scope.documentDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/Logistics/GetAllDocuments",
            }
        },
    };

 

    $scope.locationDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/Logistics/GetAllLocations",
            }
        },
    };

    //vstart
    $scope.contractorNameDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/Logistics/GetAllContractor",
            }
        },
    };

    $scope.clearingAgentDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/Logistics/GetAllClearingAgent",
            }
        },
    };
   

    $scope.issuingCarrierDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/Logistics/GetAllIssuingCarrier",
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

    function RefreshGrid() {
        $("#grid").data("kendoGrid").dataSource.read();
    }

    $scope.LoadETA = function () {

        var locationvalue = $("#fromlocation").val();
        var invoiceno = $scope.perfomainvoiceno;
        var response = $http({
            method: "get",
            url: "/api/Logistics/GetLogisticETAByInvLocationCode?invoiceno=" + invoiceno + "&locationcode=" + locationvalue,
            dataType: "json"
        });
        return response.then(function (data) {
            if (data.data.length != 0) {

                if (data.data.Result == "Exist") {
                    $scope.SRC_ETA = "";
                    $("#fromlocation").data("kendoDropDownList").value("");
                    displayPopupNotification("Cannot Select Same Source Location", "warning");
                    return;
                }

             

                $scope.SRC_ETA = data.data.DES_ETA;
                $scope.SRC_ETD = data.data.DES_ETD;
                $scope.SRC_ATA = data.data.DES_ATA;
                $scope.SRC_ATD = data.data.DES_ATD;
                $scope.SRC_ETD_DES = data.data.DES_ETD_NEXT_DES;
           
            } else {

                $scope.SRC_ETA = "";
                $scope.SRC_ETD = "";
                $scope.SRC_ATA = "";
                $scope.SRC_ATD = "";
                $scope.SRC_ETD_DES = "";
              
            }
        });
    };


    $scope.CheckLogisticTolocation = function () {

        var locationvalue = $("#tolocation").val();
        var invoiceno = $scope.perfomainvoiceno;
        var response = $http({
            method: "get",
            url: "/api/Logistics/CheckLogisticTolocation?invoiceno=" + invoiceno + "&locationcode=" + locationvalue,
            dataType: "json"
        });
        return response.then(function (data) {

            if (data.data.length != 0) {
                if (data.data == "Exist") {
                    $("#tolocation").data("kendoDropDownList").value("");
                    displayPopupNotification("Cannot Select Same Destination Location", "warning");
                    return;
                }
            }
        });
    };

    $scope.createshipment = function (isValid) {
        debugger;
        var flag = true;
        if ($scope.uploadfields.length != 0) {
            $.each($scope.uploadfields, function (key, val) {
                if (val.DOC_NAME == "") {
                    displayPopupNotification("Document name cannot be empty.", "error");
                    flag = false;
                    return;
                }
            })
            if (flag == false) {
                return false;
            }
        }
        if ($scope.saveAction !== "Update") {
            $scope.logisticCode = 0;
        }
        if (isValid) {
            var sientryurl = window.location.protocol + "//" + window.location.host + "/api/Logistics/CreateLogistics";

            var CONTRACTER_CODE = $("#contractorName").val();
            var SHIPMENT_TYPE = $("#shipmentType").val();
            $scope.LC_LOGISTIC_CONTAINER;
          
            var sidetails = {
                LOGISTIC_CODE: $scope.logisticCode,
                INVOICE_NO: $scope.si.perfomainvoice,
                SNO: $scope.sno,
                          
                SRC_ETA: $scope.SRC_ETA,
                SRC_ETD: $scope.SRC_ETD,
                SRC_ATA: $scope.SRC_ATA,
                SRC_ATD: $scope.SRC_ATD,
                SRC_ETD_DES: $scope.SRC_ETD_DES,

                DES_ETA: $scope.SRC_ETD_DES,
                DES_ETD: $scope.DES_ETD,
                DES_ATA: $scope.DES_ATA,
                DES_ATD: $scope.DES_ATD,
                DES_ETD_NEXT_DES: $scope.DES_EDD,


                //EXTRA DOCUMENT DATA
                DOC_REC_SUP_DATE: $scope.DOC.DOC_REC_SUP_DATE,
                DOC_END_BANK_DATE: $scope.DOC.DOC_END_BANK_DATE,
                DOC_SEND_TOAGENT_DATE: $scope.DOC.DOC_SEND_TOAGENT_DATE,
                POSTAGE_CN: $scope.DOC.POSTAGE_CN,
                PORT_CLEAR_DATE: $scope.DOC.PORT_CLEAR_DATE,


                CONTRACTER_NAME: $scope.contractorName,
                CONTRACTER_CODE: CONTRACTER_CODE,
                AGENT_CODE: $scope.clearingAgent,
                CONTRACTER_ADDRESS: $scope.contractorAddress,
                CONTRACT_AMOUNT: $scope.contractAmt,
                CONTRACT_DATE: $scope.contractDate,
                JOB_ORDER_NO: $scope.jobOrderNo,
                FROM_LOCATION: $scope.fromLocation,
                TO_LOCATION: $scope.toLocation,
                SHIPPER_NAME: $scope.shipperName,
                SHIPPER_ADDRESS: $scope.shipperAddress,
                AIR_AWB_NO: $scope.si.awb,
                AIR_AWB_DATE: $scope.si.awbdate,
                LC_TRACK_NO: $scope.lctrackno,
                AIR_GROSS_WEIGHT: $scope.si.grosswhtair,
                AIR_CHARGEABLE_WEIGHT: $scope.si.chargeablewhtair,
             
                SHIPMENT_STATUS: $scope.si.shipmentstatus,
                SHIPMENT_TYPE: SHIPMENT_TYPE,
               
                EST_DAY: $scope.si.estimateday,
                CONSIGNEE_NAME: $scope.si.consigneename,
                CONSIGNEE_ADDRESS: $scope.si.consigneeaddress,
                ISSUING_CARRIER_CODE: $scope.si.issuingcarrer,
                NOTIFY_APPLICANT_NAME: $scope.si.notifyapplicantname,
                NOTIFY_APPLICANT_ADDRESS: $scope.si.notifyapplicantaddress,
                REMARKS: $scope.si.remarks,
                SEA_BL_NO: $scope.si.lading,
                SEA_BL_DATE: $scope.si.ladingdate,
                SEA_GROSS_WEIGHT: $scope.si.grossweightsea,
                SEA_CBM: $scope.si.cbmsea,
                SEA_VESSEL_NO: $scope.si.vesselno,
                ROAD_LR_NO: $scope.si.road,
                ROAD_LR_DATE: $scope.si.roaddate,
                ROAD_TRUCK_NO: $scope.si.truck,
                ROAD_TRANSPORTER_NAME: $scope.si.transname,
                ROAD_TRANSPORTER_ADDRESS: $scope.si.transaddress,
                TRAIN_RR_NO: $scope.si.train,
                TRAIN_RR_DATE: $scope.si.traindate,
                FILE_DATA: $scope.uploadfields,
                AIR_PACK: $scope.si.airpack,
                LC_LOGISTIC_CONTAINER: $scope.LC_LOGISTIC_CONTAINER
            };

            var response = $http({
                method: "POST",
                url: sientryurl,
                data: sidetails,
                dataType: "json"
            });

            return response.then(function (data) {

                $("#SerialNo").val(data.data.SerialNo);
                $("#lctrackno").val($scope.lctrackno);
                $("#cinumber").val(data.data.InvoiceNo);
                $scope.onitemsedit = false;
                $scope.editbutton = true;
                $scope.addbutton = false;
                HideShippment = true;
                documentupload = false;
                $scope.cancelsi();
                $scope.commonColumns();
                $scope.invoiceno = "";
                $scope.commercialnumber = false;
                RefreshGrid();
                $scope.uploadfields = [];
                $('.dropzone')[0].dropzone.files.forEach(function (file) {
                    file.previewElement.remove();
                });
                $('.dropzone').removeClass('dz-started');
                myDropzone.processQueue();

                if ($scope.saveAction === "Update") {
                    displayPopupNotification("Logistics Updated Successfully.", "success");
                } else if ($scope.saveAction === "Save") {
                    displayPopupNotification("Logistics Saved Successfully.", "success");
                }

            }, function errorCallback(response) {
                displayPopupNotification(response.data, "error");
            });
        }
        else {
            displayPopupNotification("Fields cannot be empty.", "error");

        }
    }

    $scope.SelectShipmentType = function (index) {

        $(".go").css('background-color', 'white');
        $($(".go_" + index)[0]).css('background-color', '#93e9c4');
        var SHIPPING_TYPE = $scope.LogisticContainerPlan[index].SHIPPING_TYPE;
        $scope.fromLocation = $scope.LogisticContainerPlan[index].FROM_LOCATION_CODE;
        $scope.toLocation = $scope.LogisticContainerPlan[index].TO_LOCATION_CODE;
        $scope.etdDate = $scope.LogisticContainerPlan[index].EST_LOADING_DATE;
        var e = undefined;
        var Logistic_Detail_Code = "";
        var create_edit = "create";
        var perfomainvoiceno = $scope.perfomainvoiceno;
        $scope.OnChangeShipmentType(e, SHIPPING_TYPE);
    }

    $scope.ChangeOnShipmentType = function (SHIPMENT_TYPE) {
        if (SHIPMENT_TYPE == "AIR") {
            $scope.showforair = true;
            $scope.showforroad = false;
            $scope.showforsea = false;
            $scope.showfortrain = false;
            $scope.showforother = false;
            $scope.showforcommon = true;
            $scope.ShowContainer = false;
            $scope.DisableShipmentType = true;
        }
        else if (SHIPMENT_TYPE == "SEA") {
            $scope.showforsea = true;
            $scope.showforair = false;
            $scope.showforroad = false;
            $scope.showfortrain = false;
            $scope.showforother = false;
            $scope.showforcommon = true;
            $scope.ShowContainer = true;
            $scope.DisableShipmentType = false;
        }
        else if (SHIPMENT_TYPE == "TRAIN") {
            $scope.showfortrain = true;
            $scope.showforroad = false;
            $scope.showforsea = false;
            $scope.showforair = false;
            $scope.showforother = false;
            $scope.showforcommon = true;
            $scope.ShowContainer = true;
            $scope.DisableShipmentType = false;
        }
        else if (SHIPMENT_TYPE == "ROAD") {
            $scope.showforroad = true;
            $scope.showforsea = false;
            $scope.showforair = false;
            $scope.showfortrain = false;
            $scope.showforother = false;
            $scope.showforcommon = true;
            $scope.ShowContainer = true;
            $scope.DisableShipmentType = false;
        }

        else {
            $scope.showforother = true;
            $scope.showforroad = false;
            $scope.showforsea = false;
            $scope.showforair = false;
            $scope.showfortrain = false;
            $scope.showforcommon = true;
            $scope.ShowContainer = false;
            $scope.DisableShipmentType = false;
        }
    }

    $scope.OnChangeShipmentType = function (e, type) {
        $(".go").css('background-color', 'white');
        var SHIPMENT_TYPE = "";
        if (type !== undefined) {
            SHIPMENT_TYPE = type;
        } else {
            SHIPMENT_TYPE = e.sender.dataItem();
        }
        $scope.ChangeOnShipmentType(SHIPMENT_TYPE);
        if (SHIPMENT_TYPE === "--Select Shipment Type--") {
            $scope.documentupload = false;
            $scope.showforcommon = false;

        }
        else {
            $scope.documentupload = true;
            $scope.showforcommon = true;
        }





    }

    $scope.AddShipmentItems = function (items) {
        $scope.si.shipmentload = "LCL";
        $scope.si.load = "LCL";
        $scope.lctrackno = items.LC_TRACK_NO;
        $scope.onitemsedit = true;
        $scope.si.shipmentType = items.SHIPMENT_TYPE;
        $scope.ChangeOnShipmentType(items.SHIPMENT_TYPE);
        $scope.showforcommon = false;
        hideloader();
    };

    //new code
    function GetLogisticPlanbyperfomainvoiceno(perfomainvoiceno) {
        var response = $http({
            method: "get",
            url: "/api/LcLogisticPlan/GetLogisticPlanbyperformainvoice?PinvoiceCode=" + perfomainvoiceno,
            dataType: "json"
        });
        return response.then(function (data) {

            if (data.data.length == 0) {
                $scope.LogisticContainerPlan = [];

            }
            else {

                $scope.LogisticContainerPlan =
                    [{
                        LOT_NO: "",
                        SHIPPING_TYPE: "",
                        FROM_LOCATION_CODE: "",
                        TO_LOCATION_CODE: "",
                        EST_BOOKING_DATE: "",
                        EST_LOADING_DATE: ""
                    }];
                $scope.LogisticContainerPlan = data.data;


         
            }
            d1.resolve(response);
        }, function errorCallback(response) {
            displayPopupNotification("Error Occured.", "error");
        });

    };

    function getShipmentDetailsByPinvoice(perfomainvoiceno) {
        var response = $http({
            method: "get",
            url: "/api/Logistics/getShipmentDetailsByPinvoice?PinvoiceCode=" + perfomainvoiceno,
            dataType: "json"
        });
        return response.then(function (data) {
            debugger;
            if (data.data.length == 0) {

                $scope.itemlist = [];
                $scope.onitemsedit = false;
            }
            else {
                $scope.onitemsedit = true;
                if (data.data[0].DETAIL_ENTRY_FLAG == 'Y') {
                    $scope.editbutton = true;
                    $scope.addbutton = false;
                }
                else {
                    $scope.editbutton = false;
                    $scope.addbutton = true;
                }
                debugger;
                $scope.itemlist = data.data;


                $scope.Total_Quantity = 0;
                $scope.Total_Amount = 0.00;

                for (i = 0; i < $scope.itemlist.length; i++) {
                    $scope.Total_Quantity = parseInt(parseInt($scope.Total_Quantity) + parseInt($scope.itemlist[i].QUANTITY));
                    $scope.Total_Amount = (parseFloat(parseFloat($scope.Total_Amount) + ($scope.itemlist[i].QUANTITY * parseFloat($scope.itemlist[i].AMOUNT)))).toFixed(2);

                };

                $scope.lcetracknumberforlc = data.data[0].LC_TRACK_NO;
                $scope.lctrackno = data.data[0].LC_TRACK_NO;
                $scope.pinvoicecodeforlc = data.data[0].PINVOICE_CODE;

            }

        }, function errorCallback(response) {
            displayPopupNotification("Fields should not be empty.", "error");

        });
    }

    function IsAir(perfomainvoiceno) {
        var response = $http({
            method: "get",
            url: "/api/Logistics/IsAir?PinvoiceCode=" + perfomainvoiceno,
            dataType: "json"
        });
        return response.then(function (data) {
            if (data.data) {
                $scope.DisableShipmentType = true;
            } else
            {
                $scope.DisableShipmentType = false;
             }
            d3.resolve(response);
        }, function errorCallback(response) {
            displayPopupNotification("Error Occured.", "error");
        });

    };
    //
    $scope.ItemsOnChange = function (options) {
        debugger;
        $scope.ClearOnItemChange();
        $(".go").css('background-color', 'white');
        var perfomainvoiceno = options.dataItem.INVOICE_CODE;
        perfomainvoiceno = (perfomainvoiceno == undefined) ? null : perfomainvoiceno;
        $scope.perfomainvoiceno = perfomainvoiceno;
        showloader();
        d1 = $.Deferred();
        getShipmentDetailsByPinvoice(perfomainvoiceno);  //FETCH ITEM LIST
        perfomainvoiceno=escape(perfomainvoiceno);
        GetLogisticPlanbyperfomainvoiceno(perfomainvoiceno); //SHIPPING DETAIL   -- SEA/ROAD/TRAIN
        $.when(d1).done(function () {
            $scope.LogisticContainerPlan;
            if ($scope.LogisticContainerPlan.length > 0) {
                var SHIPMENT_TYPE = $scope.LogisticContainerPlan[0].SHIPPING_TYPE;
                var Logistic_Detail_Code = "";
                var create_edit = "create"
                Logistics(SHIPMENT_TYPE, perfomainvoiceno, create_edit, Logistic_Detail_Code);  //CONTAINER LIST
                $scope.ShowContainer = false;
            }
         
            $scope.HideShippment = true;
            $scope.showforair = false;
            $scope.showforroad = false;
            $scope.showforsea = false;
            $scope.showfortrain = false;
            $scope.showforother = false;
            $scope.showforcommon = false;
            $scope.commercialnumber = false;
            $scope.invoiceno = false;
          
            $("#shipmentType").data("kendoDropDownList").value("--Select Shipment Type--");
            d3 = $.Deferred();
            IsAir(perfomainvoiceno);
          
            $.when(d3).done(function () {
                if ($scope.DisableShipmentType) {
                    $("#shipmentType").data("kendoDropDownList").value("AIR");
                    $scope.ChangeOnShipmentType("AIR");
                    $("#shipmentType").attr('disabled', 'disabled');
                }
                hideloader();
            });
        });

       
    }
    $scope.ipPurchaseOrder = {
        dataType: "json",
        serverFiltering: true,
        filter: "startswith",
        transport: {
            read: function (options) {
                $.ajax({
                    url: "/api/Logistics/GetAllLcIpPurchaseOrder?filter=" + options.data.filter.filters[0].value,
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
                    url: "/api/Logistics/GetAllLcIpPurchaseOrderfilter?filter=" + options.data.filter.filters[0].value,
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

    $scope.getImage = function (mylist, ATTACH_DOC) {
        if (mylist != null) {
            var img = "";
            var il = "";
            for (var i = 0; i < mylist.length; i++) {

                img = img + "<img src='" + mylist[i].FILE_URL + "' height=20 />";
                var extension = mylist[i].FILE_URL.substr((mylist[i].FILE_URL.lastIndexOf('.') + 1));
                extension = extension.toLowerCase();

                //$('.image').data('src')
                switch (extension) {
                    case 'jpg':
                    case 'jpeg':
                        il = il + "<a target='_blank' href='" + mylist[i].FILE_URL + "' data-toggle='tooltip' title='Hooray!'><i class='fa fa-file-image-o image'/></i></a>"
                        break;
                    case 'png':
                        il = il + "<a target='_blank' href='" + mylist[i].FILE_URL + "' ><i class='fa fa-file-image-o image' /></i></a>"
                        break;
                    case 'pdf':
                        il = il + "<a target='_blank' href='" + mylist[i].FILE_URL + "' ><i class='fa fa-file-pdf-o image' /></i></a>"
                        break;
                    case 'docx':
                        il = il + "<a target='_self' href='" + mylist[i].FILE_URL + "' download><i class='fa fa-file-word-o image' /></i></a>"
                        break;
                    case 'xls':
                    case 'xlsx':
                        il = il + "<a target='_self' href='" + mylist[i].FILE_URL + "' download><i class='fa fa-file-excel-o image' /></i></a>"
                        break;
                    default:
                        il = il + "<a target='_self' href='" + mylist[i].FILE_URL + "' data-toggle='tooltip' title='Hooray!' download><i class='fa fa-file-text-o image' /></i></a>"
                }

            }
            return $sce.trustAsHtml(il);
        }

        else {
            il = "";
            return il;
        }
    }


    $scope.AllcheckboxEnableDisable = function () {

        if ($scope.Allcheckbox) {
            $scope.cleareditcontent();
        } else {
            $scope.disableeditcontent();
        }
    }


    function Logistics(SHIPMENT_TYPE, perfomainvoiceno, create_edit, Logistic_Detail_Code) {

        var response = $http({
            method: "get",
            url: "/api/Logistics/GetLogisticPlanContainerDetailByShipmentType?shipmentType=" + SHIPMENT_TYPE + "&InvoiceNo=" + perfomainvoiceno + "&create_edit=" + create_edit + "&Logistic_Detail_Code=" + Logistic_Detail_Code,
            dataType: "json"
        });

        return response.then(function (data) {

            if (data.data.length == 0) {
                $scope.LC_LOGISTIC_CONTAINER = [];

            }
            else {
                $scope.LC_LOGISTIC_CONTAINER = [];
                $scope.LC_LOGISTIC_CONTAINER = data.data;
                var SHIPMENT_TYPE = $scope.LC_LOGISTIC_CONTAINER[0].SHIPMENT_TYPE;
               


            }
        }, function errorCallback(response) {
            displayPopupNotification("Error Occured.", "error");
        });
    };

  



    $scope.mainGridOptions = {
        toolbar: ["excel"],
        dataSource: {
            type: "json",
            transport: {
              read: "/api/Logistics/getAllLogistic",
            },
            pageSize: 100,
            serverPaging: false,
            serverSorting: false,
            schema: {
                model: {
                    fields: {
                        CREATED_DATE: { type: "date" },
                        LC_TRACK_NO: { type: "number" },
                        EST_DAY: { type: "number" },
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
                        fileName: "Logistics.xlsx"
                    });
                });
        },
        columns: [
            {
                field: "LC_TRACK_NO",
                title: "Track No",
                attributes: { style: "text-align:right;" }
            },
            {
                field: "INVOICE_NO",
                title: "Invoice No"
            },
            {
                field: "CREATED_BY",
                title: "Created by",

            },

            {
                field: "CREATED_DATE",
                title: "Created date",
                template: "#= CREATED_DATE == null ? '' :kendo.toString(kendo.parseDate(CREATED_DATE),'M/dd/yyyy') #",
            },

        ]
    };

    $scope.exportChildData = function (lctrackno, rowIndex) {

        var deferred = $.Deferred();

        $scope.detailExportPromises.push(deferred);

        var rows = [{
            cells: [
                // First cell
                { value: "SHIPMENT_TYPE" },
                { value: "FROM_LOCATION" },
                // Second cell
                { value: "TO_LOCATION" },
                // Third cell
                { value: "CONTRACTER_NAME" },
                // Fourth cell
                { value: "CONTRACTER_ADDRESS" },
                // Fifth cell
                { value: "CONTRACT_AMOUNT" },
                { value: "CONTRACT_DATE" }
            ]
        }];

        $($('.itemgrid')[rowIndex]).data("kendoGrid").dataSource.filter({ field: "LC_TRACK_NO", operator: "eq", value: lctrackno });
        var exporter = new kendo.ExcelExporter({
            columns: [
                {
                    field: "SHIPMENT_TYPE"
                },
                {
                    field: "FROM_LOCATION", title: "From Location",
                }, {
                    field: "TO_LOCATION", title: "To Location",
                }, {
                    field: "CONTRACTER_NAME", title: "Contracter Name",
                }, {
                    field: "CONTRACTER_ADDRESS", title: "Contracter Address",
                }, {
                    field: "CONTRACT_AMOUNT", title: "Contract Amount",
                }, {
                    field: "CONTRACT_DATE", title: "Contract Date",
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

    $scope.shipmentDetailitemGridOptions = function (dataItem) {

        return {
            dataSource: {
                type: "json",
                transport: {

                    read: {
                        url: "/api/Logistics/getAllLogisticShipmentList?lctrack=" + dataItem.LC_TRACK_NO + "&invoice=" + dataItem.INVOICE_NO,
                        dataType: "json"
                    },


                },
                batch: true,
                serverPaging: false,
                serverSorting: false,
                serverFiltering: false,
                pageSize: 20,
                group: { field: "SHIPMENT_TYPE", title: "Shipment Type" },
                schema: {
                    model: {
                        fields: {
                            CONTRACT_DATE: { type: "date" },
                        }
                    },
                },
                filter: {
                    field: "LC_TRACK_NO",
                    operator: "eq",
                    value: dataItem.LC_TRACK_NO
                }
            },

            scrollable: false,
            excelExport: function (e) {
                // prevent saving the file
                e.preventDefault();

              
            },
            sortable: true,
            pageable: true,
            navigatable: true,
            resizable: true,
            dataBound: function (o) {
               
            },
            columns: [
                {
                    field: "FROM_LOCATION",
                    title: "From",
                },
                {
                    field: "TO_LOCATION",
                    title: "To",

                },
                {
                    field: "CONTRACTER_NAME",
                    title: "Contract Name",

                },
                {
                    field: "CONTRACTER_ADDRESS",
                    title: "Contract Address",

                },
                {
                    field: "CONTRACT_AMOUNT",
                    title: "Contract Amt",
                    format: "{0:n2}",
                    attributes: { style: "text-align:right;" }

                },
                {
                    field: "CONTRACT_DATE",
                    title: "Contract Date",
                    template: "#= CONTRACT_DATE == null ? '' :kendo.toString(kendo.parseDate(CONTRACT_DATE),'M/dd/yyyy') #",
                },

                {
                    field: "mylist", title: "Images", template: '<span ng-bind-html= "getImage(dataItem.FILE_DATA, dataItem.FILE_DATA)"> </span>',

                },
                {
                    field: "Action",
                    width: "50px",
                    template: '<a class="edit glyphicon glyphicon-edit" ng-click="EditLD(dataItem)" style="color:grey;"><span class="sr-only"></span> </a><a class="fa fa-history" ng-click="HistoryLogistics(dataItem)" title="History" style="color:grey;"><span class="sr-only"></span> </a>'
                },

            ],
        };
    };


    $scope.FilterGrid = function (LOGISTIC_CODE) {
        $("#grid").data("kendoGrid").tbody.find("tr").css("background-color", "");
        var data = $("#grid").data("kendoGrid").dataSource.data();
        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
            if (dataItem.LOGISTIC_CODE == LOGISTIC_CODE) {
                $("#grid").data("kendoGrid").tbody.find("tr[data-uid=" + dataItem.uid + "]").css("background-color", "#80ddaa");
            }
        }

    };


    $scope.EditLD = function (items) {
        debugger;
        $scope.saveAction = "Update";
        $scope.editcontent = true;
        $scope.documentupload = true;
        $scope.profomainvoicedisabled = true;
        $scope.ContentModal = false;
        $("#shipmentType").data("kendoDropDownList").enable(false);
        $scope.lctrackno = items.LC_TRACK_NO;
        $scope.invoiceno = items.INVOICE_NO;
        $scope.logicticcode = items.LOGISTIC_CODE;
        $scope.FilterGrid(items.LOGISTIC_CODE);
        $scope.Allcheckbox = false;
        $scope.commercialnumber = true;
        $scope.$apply;
        var response = $http({
            method: "get",
            url: "/api/Logistics/GetLogisticShipmentDetailsByLCode?lctrackno=" + $scope.lctrackno + "&invoiceno=" + $scope.invoiceno + "&logicticcode=" + $scope.logicticcode,
            dataType: "json"
        });
        return response.then(function (data) {

            if (data.data.length == 0) {

            }
            else {

                $scope.showFormPanel = true;
                $scope.oneditshow = false;
                $scope.HideShippment = true;
                $scope.logisticCode = data.data.LOGISTIC_CODE;
                $scope.si.awb = data.data.AIR_AWB_NO;
                $scope.si.awbdate = $filter('date')(data.data.AIR_AWB_DATE, "MM/dd/yyyy");
             

                $scope.SRC_ETA = $filter('date')(data.data.SRC_ETA, "MM/dd/yyyy");
                $scope.SRC_ETD = $filter('date')(data.data.SRC_ETD, "MM/dd/yyyy");
                $scope.SRC_ATA = $filter('date')(data.data.SRC_ATA, "MM/dd/yyyy");
                $scope.SRC_ATD = $filter('date')(data.data.SRC_ATD, "MM/dd/yyyy");
                $scope.SRC_ETD_DES = $filter('date')(data.data.SRC_ETD_DES, "MM/dd/yyyy");
                $scope.DES_ETA = $filter('date')(data.data.SRC_ETD_DES, "MM/dd/yyyy");
                $scope.DES_ETD = $filter('date')(data.data.DES_ETD, "MM/dd/yyyy");
                $scope.DES_ATA = $filter('date')(data.data.DES_ATA, "MM/dd/yyyy");
                $scope.DES_ATD = $filter('date')(data.data.DES_ATD, "MM/dd/yyyy");
                $scope.DES_ETD_NEXT_DES = $filter('date')(data.data.DES_ETD_NEXT_DES, "MM/dd/yyyy");


                $scope.DOC.DOC_REC_SUP_DATE = data.data.DOC_REC_SUP_DATE;
                $scope.DOC.DOC_END_BANK_DATE = data.data.DOC_END_BANK_DATE;
                $scope.DOC.DOC_SEND_TOAGENT_DATE = data.data.DOC_SEND_TOAGENT_DATE;
                $scope.DOC.POSTAGE_CN = data.data.POSTAGE_CN;
                $scope.DOC.PORT_CLEAR_DATE = data.data.PORT_CLEAR_DATE;


                $scope.contractorName = data.data.CONTRACTER_NAME;
                $scope.contractorAddress = data.data.CONTRACTER_ADDRESS;
                $scope.contractAmt = data.data.CONTRACT_AMOUNT;
                $scope.contractDate = $filter('date')(data.data.CONTRACT_DATE, "MM/dd/yyyy");
                $scope.jobOrderNo = data.data.JOB_ORDER_NO;
                $scope.shipperName = data.data.SHIPPER_NAME;
                $scope.shipperAddress = data.data.SHIPPER_ADDRESS;
                $scope.si.remarks = data.data.REMARKS
                $scope.si.lading = data.data.SEA_BL_NO;
                $scope.si.ladingdate = $filter('date')(data.data.SEA_BL_DATE, "MM/dd/yyyy");
                $scope.si.grossweightsea = data.data.SEA_GROSS_WEIGHT;
                $scope.si.grossweightsea = data.data.SEA_GROSS_WEIGHT;
                $scope.si.cbmsea = data.data.SEA_CBM;
                $scope.si.vesselno = data.data.SEA_VESSEL_NO;
                $scope.si.road = data.data.ROAD_LR_NO;
                $scope.si.roaddate = $filter('date')(data.data.ROAD_LR_DATE, "MM/dd/yyyy");
                $scope.si.truck = data.data.ROAD_TRUCK_NO;
                $scope.si.transname = data.data.ROAD_TRANSPORTER_NAME;
                $scope.si.transaddress = data.data.ROAD_TRANSPORTER_ADDRESS;
                $scope.si.train = data.data.TRAIN_RR_NO;
                $scope.si.traindate = $filter('date')(data.data.TRAIN_RR_DATE, "MM/dd/yyyy");
                $scope.si.grosswhtair = data.data.AIR_GROSS_WEIGHT;
                $scope.si.chargeablewhtair = data.data.AIR_CHARGEABLE_WEIGHT;
                $scope.fromLocation = data.data.FROM_LOCATION_CODE;
                $scope.toLocation = data.data.TO_LOCATION_CODE;
                $scope.clearingAgent = data.data.AGENT_CODE;
                $scope.contractorCode = data.data.CONTRACTER_CODE;
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
                $scope.si.container = data.data.CONTAINER_CODE;
                $scope.si.issuingcarrer = data.data.ISSUING_CARRIER_CODE;
                $("#AIR_PACK").val(data.data.AIR_PACK);
                $scope.onitemsedit = true;

                d1 = $.Deferred();
                d2 = $.Deferred();
                $scope.si.perfomainvoice = $scope.invoiceno;
                $scope.perfomainvoiceno = data.data.PINVOICE_NO;
                var perfomainvoiceno = $scope.perfomainvoiceno;
                getShipmentDetailsByPinvoice(perfomainvoiceno);
                GetLogisticPlanbyperfomainvoiceno(perfomainvoiceno);

                var SHIPMENT_TYPE = data.data.SHIPMENT_TYPE;
                var Logistic_Detail_Code = $scope.logisticCode;
                $.when(d1).done(function () {
                    var create_edit = "edit";

                    if (SHIPMENT_TYPE !== "AIR") {

                        setTimeout(function () { Logistics(SHIPMENT_TYPE, perfomainvoiceno, create_edit, Logistic_Detail_Code); }, 100);
                        $scope.ShowContainer = true;
                    } else {
                        $scope.ShowContainer = false;
                    }

                });

                if (data.data.SHIPMENT_TYPE == "AIR") {
                    $scope.showforair = true;
                    $scope.showforroad = false;
                    $scope.showforsea = false;
                    $scope.showfortrain = false;
                    $scope.showforother = false;
                    $scope.showforcommon = true;
                }
                else if (data.data.SHIPMENT_TYPE == "SEA") {
                    $scope.showforsea = true;
                    $scope.showforair = false;
                    $scope.showforroad = false;
                    $scope.showfortrain = false;
                    $scope.showforother = false;
                    $scope.showforcommon = true;
                }
                else if (data.data.SHIPMENT_TYPE == "TRAIN") {
                    $scope.showfortrain = true;
                    $scope.showforroad = false;
                    $scope.showforsea = false;
                    $scope.showforair = false;
                    $scope.showforother = false;
                    $scope.showforcommon = true;
                }
                else if (data.data.SHIPMENT_TYPE == "ROAD") {
                    $scope.showforroad = true;
                    $scope.showforsea = false;
                    $scope.showforair = false;
                    $scope.showfortrain = false;
                    $scope.showforother = false;
                    $scope.showforcommon = true;
                    $scope.hideCarrier = true;
                }

                else {
                    $scope.showforother = true;
                    $scope.showforroad = false;
                    $scope.showforsea = false;
                    $scope.showforair = false;
                    $scope.showfortrain = false;
                    $scope.showforcommon = true;
                }


                //disble input on edit start

                $scope.disableeditcontent();
                //end
                var imageurl = [];

                if (data.data.FILE_DATA.length > 0) {
                    $.each(data.data.FILE_DATA, function (key, value) {

                        var filepath = value.FILE_URL;
                        var path = filepath.replace(/[/]/g, '_');
                        var filename = path.split('_')[2];
                        $scope.uploadfields.push({
                            SNO: value.SNO,
                            DOC_NAME: value.DOCUMENT_CODE,
                            DOC_DATE: $filter('date')(value.DOC_DATE, "MM/dd/yyyy"),
                            DOC_ACTION: value.DOC_ACTION,
                            DOC_PREPARED_DATE: $filter('date')(value.DOC_PREPARED_DATE, "MM/dd/yyyy"),
                            DOC_EST_RECEIVED_DATE: $filter('date')(value.DOC_EST_RECEIVED_DATE, "MM/dd/yyyy"),
                            DOC_RECEIVED_DATE: $filter('date')(value.DOC_RECEIVED_DATE, "MM/dd/yyyy"),
                            DOC_EST_SUBMITTED_DATE: $filter('date')(value.DOC_EST_SUBMITTED_DATE, "MM/dd/yyyy"),
                            DOC_SUBMITTED_DATE: $filter('date')(value.DOC_SUBMITTED_DATE, "MM/dd/yyyy"),
                        });

                        var mockFile = {
                            name: filename,
                            size: 12345,
                            type: 'image/jpeg',
                            url: value.FILE_URL,
                            accepted: true,
                        };
                        if (key == 0) {
                            myDropzone.on("addedfile", function (file) {
                                caption = file.caption == undefined ? "" : file.caption;
                                file._captionLabel = Dropzone.createElement("<a class='fa fa-download dropzone-download' href='" + value.FILE_URL + "' name='Download' class='dropzone_caption' download></a>");
                                file.previewElement.appendChild(file._captionLabel);
                                file["SNO"] = value.SNO;
                                file["INVOICENO"] = items.INVOICE_NO;
                                file["LC_TRACK_NO"] = items.LC_TRACK_NO;
                                value.SNO++;
                            });
                        }
                        myDropzone.emit("addedfile", mockFile, "edit");
                        myDropzone.files.push(mockFile);

                    });

                }
            }





        },
            function errorCallback(response) {
                displayPopupNotification("Error Occured.", "error");
            });
    };
    $scope.HistoryLogistics = function (dataItem) {
        $scope.historyitemGridOptions = function () {


            return {
                dataSource: {
                    type: "json",
                    transport: {
                        read: {
                            url: "/api/Logistics/getAllLogisticsHistoryList?lctrackno=" + dataItem.LC_TRACK_NO,
                            dataType: "json"
                        },
                    },

                    serverPaging: false,
                    serverSorting: false,
                    serverFiltering: false,
                    pageSize: 10,
                 
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
                   
                    {
                        field: "SRC_ETA", title: "Estimated Time of Arrival(ETA)", width: "50px", format: "{0:M/dd/yyyy}",
                        template: "#= SRC_ETA == null ? '' :kendo.toString(kendo.parseDate(SRC_ETA),'M/dd/yyyy') #",
                    },
                    {
                        field: "SRC_ETD", title: "Estimated Time of Departure (ETD)", width: "50px", format: "{0:M/dd/yyyy}",
                        template: "#= SRC_ETD == null ? '' :kendo.toString(kendo.parseDate(SRC_ETD),'M/dd/yyyy') #",
                    },
                    {
                        field: "SRC_ATA", title: "Actual Time of Arrival (ATA)", width: "50px", format: "{0:M/dd/yyyy}",
                        template: "#= SRC_ATA == null ? '' :kendo.toString(kendo.parseDate(SRC_ATA),'M/dd/yyyy') #",
                    },
                    {
                        field: "SRC_ATD", title: "Actual Time of Departure (ATD)", width: "50px", format: "{0:M/dd/yyyy}",
                        template: "#= SRC_ATD == null ? '' :kendo.toString(kendo.parseDate(SRC_ATD),'M/dd/yyyy') #",
                    },
                    {
                        field: "SRC_ETD_DES", title: "Estimated Time of Delivery (EDD)", width: "50px", format: "{0:M/dd/yyyy}",
                        template: "#= SRC_ETD_DES == null ? '' :kendo.toString(kendo.parseDate(SRC_ETD_DES),'M/dd/yyyy') #",
                    },
                    {
                        field: "DES_ETA", title: "Estimated Time of Arrival (ETA)", width: "50px", format: "{0:M/dd/yyyy}",
                        template: "#= DES_ETA == null ? '' :kendo.toString(kendo.parseDate(DES_ETA),'M/dd/yyyy') #",
                    },
                    {
                        field: "DES_ETD", title: "Estimated Time of Departure (ETD)", width: "50px", format: "{0:M/dd/yyyy}",
                        template: "#= DES_ETD == null ? '' :kendo.toString(kendo.parseDate(DES_ETD),'M/dd/yyyy') #",
                    },
                    {
                        field: "DES_ATA", title: "Actual Time of Arrival (ATA)", width: "50px", format: "{0:M/dd/yyyy}",
                        template: "#= DES_ATA == null ? '' :kendo.toString(kendo.parseDate(DES_ATA),'M/dd/yyyy') #",
                    },
                    {
                        field: "DES_ATD", title: "Actual Time of Departure (ATD)", width: "50px", format: "{0:M/dd/yyyy}",
                        template: "#= DES_ATD == null ? '' :kendo.toString(kendo.parseDate(DES_ATD),'M/dd/yyyy') #",
                    },
                    {
                        field: "DES_ETD_NEXT_DES", title: "Estimated Time of Delivery (EDD)", width: "50px", format: "{0:M/dd/yyyy}",
                        template: "#= DES_ETD_NEXT_DES == null ? '' :kendo.toString(kendo.parseDate(DES_ETD_NEXT_DES),'M/dd/yyyy') #",
                    },
                   
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
       
        $scope.historyitemGridOptions();

        $scope.historyLogisticContainerGridOptions = function () {
            return {
                dataSource: {
                    type: "json",
                    transport: {
                        read: {
                            url: "/api/Logistics/getAllLogisticsContainerHistoryList?lctrackno=" + dataItem.LC_TRACK_NO,
                            dataType: "json"
                        },
                    },

                    serverPaging: false,
                    serverSorting: false,
                    serverFiltering: false,
                    pageSize: 10,
                    
                },
                scrollable: false,
                sortable: true,
                pageable: true,

                dataBinding: function () {
                    record = (this.dataSource.page() - 1) * this.dataSource.pageSize();
                },
                columns: [
                    { title: "SN", template: "#= ++record #", width: 20, attributes: { style: "text-align:right;" } },
                    { field: "LC_TRACK_NO", title: "Track Number", width: "110px" },
                    { field: "INVOICE_NO", title: "Invoice Number", width: "110px" },

                   
                    {
                        field: "CREATED_DATE", title: "Created Date", width: "40px", format: "{0:M/dd/yyyy}",
                        template: "#= CREATED_DATE == null ? '' :kendo.toString(kendo.parseDate(CREATED_DATE),'M/dd/yyyy') #",
                    },
                    { field: "CREATED_BY_EDESC", title: "Created By", width: "50px" },

                   

                ],

               
            };
        }
        $scope.historyLogisticContainerGridOptions();

        $scope.historydocumentGridOptions = function () {
            return {
                dataSource: {
                    type: "json",
                    transport: {
                        read: {
                            url: "/api/Logistics/getAllLogisticsDocumentHistoryList?lctrackno=" + dataItem.LC_TRACK_NO,
                            dataType: "json"
                        },
                    },

                    serverPaging: false,
                    serverSorting: false,
                    serverFiltering: false,
                    pageSize: 10,
                   
                },
                scrollable: false,
                sortable: true,
                pageable: true,

                dataBinding: function () {
                    record = (this.dataSource.page() - 1) * this.dataSource.pageSize();
                },
                columns: [
                    { title: "SN", template: "#= ++record #", width: 20, attributes: { style: "text-align:right;" } },
                    { field: "LC_TRACK_NO", title: "Track Number", width: "110px" },
                    { field: "INVOICE_NO", title: "Invoice Number", width: "110px" },
                   
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
        }
        $scope.historydocumentGridOptions();
        $('#historymodal').modal('show');
    }

});


app.service('ShipmentService', function ($http) {
    var fac = {};
    return fac;

});


