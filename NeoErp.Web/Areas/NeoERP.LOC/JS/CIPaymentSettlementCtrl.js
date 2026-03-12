app.controller('CIPaymentSettlementCtrl', function ($scope, $http, $sce, $filter, $q, ShipmentService) {
    function init() {
        $scope.disable_settlementdate = true;
        $scope.disableinput = true;
        $scope.hide_ppdate = true;
        $scope.showForm = false;
        $scope.showFormPanel = false;
        $scope.disableCI = false;
        $scope.Edit = false;
        $scope.pterm = "";
        $scope.saveAction = "Save";
        $scope.scAction = "Save & Continue";
        $('#invoiceNumber').val("");
        $scope.templctrackno = 0;
        $('.image-placeholder').html("");
        $scope.CIPS = {};
        $scope.CIPS = {
            PS_CODE: "",
            LC_TRACK_NO: "",
            INVOICE_CODE: "",
            CI_AMOUNT: "",
            CURRENCY: "",
            EXCHANGE_RATE_AT_PAYMENT: "",
            DERIVED_TOTAL_AMOUNT: "",
            SETTLEMENT_DATE: "",
        };
    };
    init();

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

    $scope.CIPaymentSettlement = function () {
        $scope.showFormPanel = true;
        $scope.ContentModal = true;
        $scope.check_SETTLEMENT_DATE = false;
        $scope.pterm = "";
        $scope.Edit = false;
    };

    $scope.cancelCIPaymentSettlement = function () {
        init();
        $scope.Allcheckbox = false;
        $("#grid").data("kendoGrid").tbody.find("tr").removeClass('highlight');
    };

    $scope.GETOTALAMOUNT = function () {
        if ($scope.CIPS.EXCHANGE_RATE_AT_PAYMENT === undefined) {
            $scope.CIPS.EXCHANGE_RATE_AT_PAYMENT = "";
        }
        if (isNaN($scope.CIPS.EXCHANGE_RATE_AT_PAYMENT)) {
            $scope.CIPS.EXCHANGE_RATE_AT_PAYMENT = "";
        }

        //$scope.GRN.EXCHANGE_RATE = $scope.GRN.EXCHANGE_RATE === "" ? 1 : $scope.GRN.EXCHANGE_RATE;
        $scope.CIPS.DERIVED_TOTAL_AMOUNT = ($scope.CIPS.CI_AMOUNT * $scope.CIPS.EXCHANGE_RATE_AT_PAYMENT).toFixed(2);
    };

    var INVALID = "";
    $scope.LoadCIInfo = function () {
    
            //if ($scope.pterm === "USANCE") {
            //    return;
        //}
        debugger;
        
        var SettlementDate = $scope.CIPS.SETTLEMENT_DATE;

        if ($('#invoiceNumber').data("kendoAutoComplete").dataItem() != undefined) {
           var InvoiceCode = $('#invoiceNumber').data("kendoAutoComplete").dataItem().INVOICE_CODE;
        };
        var url = window.location.protocol + "//" + window.location.host + "/api/CIPaymentSettlement/LoadCommercialInvoiceInfo?InvoiceCode=" + InvoiceCode;
        $http.get(url).then(function (result) {

            if (result.data.INVALID !== null) {
                $scope.disablesave = true;
                displayPopupNotification("No PPNo/PP Date found. Please Add PPNo And PP Date in GRN.", "warning");
                INVALID = "Invalid";
                return;
            } else {
                $scope.disablesave = false;
                INVALID = "";
            }
            if (INVALID == "") {
                $scope.check_SETTLEMENT_DATE = true;
                $scope.check_EXCHANGE_RATE = true;
                if (result.data.pterm === "USANCE") {
                    $scope.pterm = "USANCE";
                    $scope.CIPS.PP_DATE = result.data.PP_DATE;
                    $scope.CIPS.SETTLEMENT_DATE = "";
                    $scope.hide_ppdate = false;
                    $scope.showForm = true;

                } else {
                    $scope.hide_ppdate = true;
                    $scope.pterm = "ATSIGHT";
                    $scope.CIPS.PP_DATE = "01/01/1001";//not important
                 //   if ($scope.CIPS.SETTLEMENT_DATE !== "") {
                        $scope.showForm = true;
                       
                   // }

                }
                $scope.CIPS.CI_AMOUNT = result.data.CI_AMOUNT;
                $scope.CIPS.CURRENCY = result.data.CURRENCY;
               // $scope.CIPS.EXCHANGE_RATE_AT_PAYMENT = result.data.EXCHANGE_RATE_AT_PAYMENT;
                $scope.CIPS.EXCHANGE_RATE_AT_PAYMENT = 0;
                $scope.GETOTALAMOUNT();
               
            }
            INVALID = "";
            
        });

      

      
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
                read: "/api/CIPaymentSettlement/getAllCISettlement",
            },
         pageSize: 100,
            serverPaging: false,
            serverSorting: false,
            schema: {
                model: {
                    fields: {
                        LC_TRACK_NO: { type: "string" },
                        INVOICE_CODE: { type: "string" },
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
            $scope.EditCIPaymentSettlement = function (dataItem) {
              
                $scope.saveAction = "Update";
                $scope.scAction = "Update & Continue";
                $scope.disableinput = false;
                $scope.showFormPanel = true;
                $scope.showForm = true;
                $scope.disableCI = true;
                $scope.ContentModal = false;
                $scope.disableeditcontent();
                $scope.pterm = dataItem.PTERMS;
                $scope.Allcheckbox = false;
                $scope.CIPS.PS_CODE = dataItem.PS_CODE;
                $scope.CIPS.LC_TRACK_NO = dataItem.LC_TRACK_NO;
                $scope.CIPS.INVOICE_CODE = dataItem.INVOICE_NUMBER;
                $scope.INVOICE_CODE = dataItem.INVOICE_CODE;
                $scope.CIPS.SETTLEMENT_DATE = dataItem.SETTLEMENT_DATE;
                //$scope.LoadCIInfo();
                $scope.CIPS.CI_AMOUNT = dataItem.CI_AMOUNT;
                $scope.CIPS.CURRENCY = dataItem.CURRENCY;
                $scope.CIPS.EXCHANGE_RATE_AT_PAYMENT = dataItem.EXCHANGE_RATE_AT_PAYMENT;
                $scope.CIPS.DERIVED_TOTAL_AMOUNT = dataItem.DERIVED_TOTAL_AMOUNT;
                $scope.dropzonePSCODE = dataItem.PS_CODE;
                $scope.doclctrackno = dataItem.LC_TRACK_NO;
                $scope.FilterGrid(dataItem.INVOICE_NUMBER);
                //$("#invoiceNumber").val(dataItem.INVOICE_NUMBER);
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
                        fileName: "CI Payment Settlement.xlsx"
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
                field: "INVOICE_NUMBER",
                title: "CI",
                width: "100px"
            },
             {
                 field: "PTERMS",
                 title: "TYPE",
                 width: "120px"
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
                field: "EXCHANGE_RATE_AT_PAYMENT",
                title: "Exchange Rate At Payment",
                width: "100px"
            },
            {
                field: "DERIVED_TOTAL_AMOUNT",
                title: "Derived Total Amount",
            },

            {
                field: "SETTLEMENT_DATE",
                title: "Settlemnt Date",

            },
            {
                field: "mylist",
                title: "Images",
                template: '<span ng-bind-html= "getImage(dataItem.mylist, dataItem.FILE_DETAIL)"> </span>',

            },
            {
                field: "ID", title: "Action", sortable: false, filterable: false,
                template: '<a class="edit glyphicon glyphicon-edit" title="Edit" ng-click="EditCIPaymentSettlement(dataItem)" style="color:grey;"><span class="sr-only"></span> </a> <a class="fa fa-history" ng-click="HistoryCIPaymentSettlement(dataItem)" title="History" style="color:grey;"><span class="sr-only"></span> </a>'
            }
        ]
    };
    $scope.HistoryCIPaymentSettlement = function (dataItem) {
      
        $scope.historyitemGridOptions = function () {


            return {
                dataSource: {
                    type: "json",
                    transport: {
                        read: {
                            url: "/api/CIPaymentSettlement/getAllHistoryCIPaymentSettlementList?lctrackno=" + dataItem.LC_TRACK_NO,
                            dataType: "json"
                        },
                    },

                    serverPaging: false,
                    serverSorting: false,
                    serverFiltering: false,
                 pageSize: 100,
                    
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
                    { field: "LC_TRACK_NO", title: "Track Number", width: "110px" },
                    { field: "FILE_DETAIL", title: "File Detail", width: "110px" },
                    { field: "REMARKS", title: "Remarks", width: "110px" },
                  
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
         $('#historymodal').modal('show');
    }

    $scope.InvoiceNoOnChange = function (dataitem) {
        
        if ($scope.saveAction != "Update" || $scope.scAction != "Update & Continue") {
            var griditems = $("#grid").data("kendoGrid").dataSource.data();
            if ($('#invoiceNumber').data("kendoAutoComplete").dataItem() !== undefined) {
                 var INVOICE_NUMBER = $('#invoiceNumber').data("kendoAutoComplete").dataItem().INVOICE_NUMBER;
		if(griditems.length>0)
		{                
		var data = $filter('filter')(griditems, { 'INVOICE_NUMBER': INVOICE_NUMBER }); //selected data filter
                if (data.length > 0) {
                    $scope.CIPS.INVOICE_CODE = "";
                    displayPopupNotification("Commercial Invoice Already Existed.", "warning");
                    return;
                }
		}

            
                var InvoiceCode = $('#invoiceNumber').data("kendoAutoComplete").dataItem().INVOICE_CODE;
                 
                $scope.CIPS.SETTLEMENT_DATE = "";
                $scope.LoadCIInfo();
          
             
               
            }
            
        }
        
       
    };

    $scope.disableeditcontent = function () {
           $scope.check_DERIVED_TOTAL_AMOUNT = false;
           $scope.check_EXCHANGE_RATE = false;
           //$scope.check_CURRENCY = false;
           $scope.check_CI_AMOUNT = false;
           $scope.check_PP_DATE = false;
           $scope.check_SETTLEMENT_DATE = false;
   

    };


    $scope.cleareditcontent = function () {
        $scope.check_DERIVED_TOTAL_AMOUNT = true;
        $scope.check_EXCHANGE_RATE = true;
        //$scope.check_CURRENCY = true;
        $scope.check_CI_AMOUNT = true;
        $scope.check_PP_DATE = true;
        $scope.check_SETTLEMENT_DATE = true;

    };

    $scope.AllcheckboxEnableDisable = function () {
        if ($scope.Allcheckbox) {
            $scope.cleareditcontent();
        } else {
            $scope.disableeditcontent();
        }
    }
    $scope.AddUpdateCIPaymentSettlement = function (isValid) {
        if (isValid) {
            var url = window.location.protocol + "//" + window.location.host + "/api/CIPaymentSettlement/AddUpdateCIPaymentSettlement";
            var INVOICE_CODE = "";
            if ( $scope.saveAction == "Update"||  $scope.scAction == "Update & Continue") {
                INVOICE_CODE = $scope.INVOICE_CODE;
            } else {
                INVOICE_CODE = $('#invoiceNumber').data("kendoAutoComplete").dataItem().INVOICE_CODE;
            }


           
            var CIPaymentSettlementModel = {
                PS_CODE: $scope.CIPS.PS_CODE,
                INVOICE_CODE: INVOICE_CODE,
                CI_AMOUNT:  $scope.CIPS.CI_AMOUNT,
                CURRENCY:  $scope.CIPS.CURRENCY,
                EXCHANGE_RATE_AT_PAYMENT: parseFloat($scope.CIPS.EXCHANGE_RATE_AT_PAYMENT).toFixed(2),
                DERIVED_TOTAL_AMOUNT: parseFloat($scope.CIPS.DERIVED_TOTAL_AMOUNT).toFixed(2),
                SETTLEMENT_DATE:  $scope.CIPS.SETTLEMENT_DATE,
            }


           
            var response = $http({
                method: "post",
                url: url,
                data: CIPaymentSettlementModel,
                dataType: "json"
               });
            return response.then(function (result) {
              
                if (result.data.RESULT === "Updated" || result.data.RESULT === "Inserted") {
                  
                    $('.image-placeholder').html("");
                    if (result.data != null) {
                        $('#dropzonePSCODE').val(result.data.PS_CODE);
                        if (result.data.RESULT === "Inserted") {
                            $("#doclctrackno").val(result.data.LC_TRACK_NO);
                        }
                       
                    }
                  
                    myDropzone.processQueue();
                    $scope.cancelCIPaymentSettlement();
                    init();
                    $("#grid").data("kendoGrid").dataSource.read();
                    displayPopupNotification("Succesfully Saved CI Payment Settlement.", "success");
                }
                else if (result.data.RESULT === "Existed") {
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

   

    app.factory('CIPaymentSettlementService', function ($http) {
        fac = {};
      

        return fac;

    });

});
