app.controller('GrnCtrl', function ($scope, $http, $sce, $filter, $q, ShipmentService) {

    function init() {
        $scope.showFormPanel = false;
        $scope.showForm = false;
        $scope.disableinput = true;
        $scope.disable_ppdate = true;
        $scope.disableCI = false;
        $scope.ContentModal = true;
        $scope.saveAction = "Save";
        $scope.scAction = "Save & Continue";
        $('#invoiceNumber').val("");
        $scope.templctrackno = 0;
        $('.image-placeholder').html("");
        $scope.GRN = {};
        $scope.GRN = {
            LC_TRACK_NO: "",
            GRN_CODE:"",
            GRN_NO: "",
            INVOICE_CODE: "",
            GRN_DATE: "",
            PP_NO:"",
            PP_RECEIEVE_DATE: "",
            CI_AMOUNT: "",
            CURRENCY: "",
            EXCHANGE_RATE: "",
            DERIVED_TOTAL_AMOUNT: ""

        };
    };
    init();

    $scope.ipPurchaseOrder = {
        //ip
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

    $scope.ADDGRN = function () {
        $scope.showFormPanel = true;
        $scope.check_GRN_NO = true;
        $scope.check_GRN_DATE = true;
        $scope.check_EXCHANGE_RATE = true;
        $scope.check_PP_NO = true;
        $scope.check_PP_RECEIEVE_DATE = true;
        $scope.check_EXCHANGE_RATE = true;
    };

    $scope.cancelGRN = function () {
        init();
        $("#grid").data("kendoGrid").tbody.find("tr").removeClass('highlight');
    };

    $scope.GETOTALAMOUNT = function () {
        $scope.GRN;
        if ($scope.GRN.EXCHANGE_RATE === undefined) {
            $scope.GRN.EXCHANGE_RATE = "";
        }
        if (isNaN($scope.GRN.EXCHANGE_RATE)) {
            $scope.GRN.EXCHANGE_RATE = "";
        }
        //$scope.GRN.EXCHANGE_RATE = $scope.GRN.EXCHANGE_RATE === "" ? 1 : $scope.GRN.EXCHANGE_RATE;
        $scope.GRN.DERIVED_TOTAL_AMOUNT = ($scope.GRN.CI_AMOUNT * $scope.GRN.EXCHANGE_RATE).toFixed(2);
    };




    var INVALID = "";
    $scope.LoadCIInfo = function () {

        if ($('#invoiceNumber').data("kendoAutoComplete").dataItem() != undefined) {
            var InvoiceCode = $('#invoiceNumber').data("kendoAutoComplete").dataItem().INVOICE_CODE;
            };
              var url = window.location.protocol + "//" + window.location.host + "/api/GRN/LoadCommercialInvoiceInfo?InvoiceCode=" + InvoiceCode;
        $http.get(url).then(function (result) {

            if (result.data.INVALID !== null) {
                $scope.disablesave = true;
                displayPopupNotification("No PP Date found. Please update PP Date in Commercial Invoice.", "warning");
                INVALID = "Invalid";
                return;
            } else {
                $scope.disablesave = false;
                INVALID = "";
            }
            if (INVALID == "") {
                $scope.showForm = true;
                $scope.GRN.CI_AMOUNT = result.data.CI_AMOUNT;
                $scope.GRN.CURRENCY = result.data.CURRENCY;
                $scope.GRN.EXCHANGE_RATE = result.data.EXCHANGE_RATE;
                $scope.GRN.EXCHANGE_RATE = 0;
                $scope.GETOTALAMOUNT();

            }
            INVALID = "";

        });




    };

    
    $scope.disableeditcontent = function () {
        $scope.check_PP_RECEIEVE_DATE = false;
        $scope.check_PP_NO = false;
        $scope.check_GRN_NO=false;
        $scope.check_GRN_DATE=false;
        $scope.check_CI_AMOUNT=false;
        $scope.check_CURRENCY=false;
        $scope.check_EXCHANGE_RATE=false ;
        $scope.check_DERIVED_TOTAL_AMOUNT=false;
                   
    };


    $scope.cleareditcontent = function () {
        $scope.check_PP_RECEIEVE_DATE = true;
        $scope.check_PP_NO = true;
        $scope.check_GRN_NO = true;
        $scope.check_GRN_DATE = true;
        $scope.check_CI_AMOUNT = true;
        $scope.check_CURRENCY = true;
        $scope.check_EXCHANGE_RATE = true;
        $scope.check_DERIVED_TOTAL_AMOUNT = true;
     
    };
  
    $scope.AllcheckboxEnableDisable = function () {
        if ($scope.Allcheckbox) {
            $scope.cleareditcontent();
        } else {
            $scope.disableeditcontent();
        }
    }

   
    $scope.FilterGrid = function (GRN_CODE) {
        $("#grid").data("kendoGrid").tbody.find("tr").css("background-color", "");
        var data = $("#grid").data("kendoGrid").dataSource.data();
        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
          if (dataItem.GRN_CODE == GRN_CODE) {
          $("#grid").data("kendoGrid").tbody.find("tr[data-uid=" + dataItem.uid + "]").css("background-color", "#80ddaa");
         }
        }
      };
 
   

    $scope.mainGridOptions = {
        toolbar: ["excel"],
        dataSource: {
            type: "json",
            transport: {
                read: "/api/GRN/getAllGRN",
            },
          pageSize: 100,
            serverPaging: false,
            serverSorting: false,
            schema: {
                model: {
                    fields: {
                        LC_TRACK_NO: { type: "string" },
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

            $scope.EditGRN = function (dataItem) {

                $scope.saveAction = "Update";
                $scope.scAction = "Update & Continue";
                $scope.showFormPanel = true;
                $scope.showForm = true;
                $scope.disableeditcontent();
                $scope.ContentModal = false;
                $scope.disableCI = true;
                $scope.Allcheckbox = false;
                $scope.GRN.GRN_CODE = dataItem.GRN_CODE;
                $scope.GRN.GRN_NO = dataItem.GRN_NO;
                $scope.FilterGrid(dataItem.GRN_CODE);
                $scope.GRN.LC_TRACK_NO = dataItem.LC_TRACK_NO;
                $scope.GRN.INVOICE_CODE = dataItem.INVOICE_NUMBER;
                $scope.INVOICE_CODE = dataItem.INVOICE_CODE;
                $scope.GRN.GRN_DATE = dataItem.GRN_DATE;
                $scope.GRN.PP_RECEIEVE_DATE = dataItem.PP_RECEIEVE_DATE;
                $scope.GRN.PP_NO = dataItem.PP_NO;
                $scope.GRN.CI_AMOUNT = dataItem.CI_AMOUNT;
                $scope.GRN.CURRENCY = dataItem.CURRENCY;
                $scope.GRN.EXCHANGE_RATE = dataItem.EXCHANGE_RATE;
                $scope.GRN.DERIVED_TOTAL_AMOUNT = dataItem.DERIVED_TOTAL_AMOUNT;
            }
            $('div').removeClass('.k-header k-grid-toolbar');
           

        },
        excelExport: function (e) {
            e.preventDefault();

            var workbook = e.workbook;
            $scope.detailExportPromises = [];
            // save the workbook
            kendo.saveAs({
                dataURI: new kendo.ooxml.Workbook(workbook).toDataURL(),
                fileName: "Grn.xlsx"
            });

                  
              
        },
        columns: [


            {
                field: "LC_TRACK_NO",
                title: "Track Number",
                attributes: { style: "text-align:right;" },
                width: "100px",

            },
            {
                field: "INVOICE_NUMBER",
                title: "Commercial Invoice",
                width: "100px",
            },
            {
                field: "GRN_NO",
                title: "GRN No.",
                width: "100px",
                attributes: { style: "text-align:right;" },
            },
            {
                field: "GRN_DATE",
                title: "GRN Date",
                width: "100px",
            },
            {
                field: "PP_NO",
                title: "PP No.",
                width: "100px",
                attributes: { style: "text-align:right;" },
            },
            {
                field: "PP_RECEIEVE_DATE",
                title: "PP Date",
                width: "100px",
            },
         
            {
                field: "CI_AMOUNT",
                title: "CI Amount",
                width: "120px"
            },


            {
                field: "CURRENCY",
                title: "Currency",
                width: "90px"
            },

            {
                field: "EXCHANGE_RATE",
                title: "Exchange Rate At Payment",
                width: "100px"
            },
            {
                field: "DERIVED_TOTAL_AMOUNT",
                title: "Derived Total Amount",
            },
            
            {
                field: "ID", title: "Action", sortable: false, filterable: false,
                template: '<a class="edit glyphicon glyphicon-edit" title="Edit" ng-click="EditGRN(dataItem)" style="color:grey;"><span class="sr-only"></span> </a> '
            }
        ]
    };
  
    $scope.InvoiceNoOnChange = function (dataitem) {
        if ($scope.saveAction != "Update" || $scope.scAction != "Update & Continue") {
            var griditems = $("#grid").data("kendoGrid").dataSource.data();
            var INVOICE_NUMBER = $('#invoiceNumber').data("kendoAutoComplete").dataItem().INVOICE_NUMBER;
		if(griditems.length>0)
		{             
 		var data = $filter('filter')(griditems, { 'INVOICE_NUMBER': INVOICE_NUMBER }); //selected data filter
            if (data.length > 0) {
                $scope.GRN.INVOICE_CODE = "";
                displayPopupNotification("Commercial Invoice Already Existed.", "warning");
                return;
            }
	 }
		
        }
        $scope.LoadCIInfo();

    };


    $scope.AddUpdateGRN = function (isValid) {

        if (isValid) {
            var url = window.location.protocol + "//" + window.location.host + "/api/GRN/AddUpdateGRN";
            var INVOICE_CODE = "";
            if ($scope.saveAction == "Update" || $scope.scAction == "Update & Continue") {
                INVOICE_CODE = $scope.INVOICE_CODE;
            } else {
                INVOICE_CODE = $('#invoiceNumber').data("kendoAutoComplete").dataItem().INVOICE_CODE;
            }
            var GRN = {
                GRN_CODE: $scope.GRN.GRN_CODE,
                GRN_NO: $scope.GRN.GRN_NO,
                LC_TRACK_NO: $scope.GRN.LC_TRACK_NO,
                GRN_DATE: $scope.GRN.GRN_DATE,
                PP_RECEIEVE_DATE: $scope.GRN.PP_RECEIEVE_DATE,
                PP_NO: $scope.GRN.PP_NO,
                INVOICE_CODE: INVOICE_CODE,
                CI_AMOUNT: $scope.GRN.CI_AMOUNT,
                CURRENCY: $scope.GRN.CURRENCY,
                EXCHANGE_RATE: parseFloat($scope.GRN.EXCHANGE_RATE).toFixed(2),
                DERIVED_TOTAL_AMOUNT: parseFloat($scope.GRN.DERIVED_TOTAL_AMOUNT).toFixed(2)
            };

            var response = $http({
                method: "post",
                url: url,
                data: GRN,
                dataType: "json"
             
            });
            return response.then(function (result) {
                if (result.data === "Updated" || result.data === "Inserted") {
                    $scope.cancelGRN();
                    $("#grid").data("kendoGrid").dataSource.read();
                    displayPopupNotification("Succesfully Saved GRN.", "success");
                }
                else if (result.data === "Existed") {
                    displayPopupNotification("Invoice Number Already Existed.", "warning");
                } else {
                    displayPopupNotification("Error Occured.", "error");
                }

            });
        }
        else {
            displayPopupNotification("Fields cannot be empty.", "warning");
        }
    };

    app.factory('GRNService', function ($http) {
        fac = {};
        fac.GetAllList = function () {
            return $http.get("/api/GRN/getAllGRN");
        }

        return fac;

    });

});
