DTModule.controller('postDatedChequeCtrl', function ($scope, postDataChequeService, $http, $routeParams, $window, $filter) {
    $scope.FormName = "Post & Open Dated Cheque";

    $scope.PDCAction = "Ok";
    $scope.IsCheckInTransit = false;
    $scope.IsEncash = false;
    $scope.ReturnToParty = false;
    $scope.DirectBounce = false;
    $scope.IscheckInHand = true;

    $scope.rdoPdcFilter = "ALL";

    $scope.result = false;

    $scope.PDCFilter = {
        DateEnglishFrom: $("#FromDateVoucher").val(),
        DateEnglishTo: $("#ToDateVoucher").val(),
        DateNepaliFrom:new Date(),
        DateNepaliTo:new Date(),
        PdcStatus :"",
        Dealer :"",
        Customer: "",
        PDCType: "",
        PDCAmount:0,
    };

    $scope.TempPDCFormObject = {};
    $scope.PDCFormObject = {
        "RECEIPT_NO": "",
        "RECEIPT_DATE": $filter('date')(new Date(), 'dd-MMM-yyyy'),
        "CHEQUE_DATE": $filter('date')(new Date(), 'dd-MMM-yyyy'),
        "BOUNCE_DATE": "",
        "BOUNCE_DATE_INFO": $filter('date')(new Date(), 'dd-MMM-yyyy'),
        "CUSTOMER_CODE_DDL": "",
        "CUSTOMER_CODE": "",
        "PDC_TYPE": "",
        "PARTY_TYPE_DDL": "",
        "PARTY_TYPE": "",
        "DEALER_TYPE_DDL": "",
        "DEALER_TYPE": "",
        "PDC_AMOUNT": "",
        "PDC_DETAILS": "",
        "PARTY_BANK_NAME": "",
        "CHEQUE_NO": "",
        "REMARKS": "",
        "REMINDER_PRIOR_DAYS": "",
        "MONEY_RECEIPT_ISSUED_BY": "",
        "MONEY_RECEIPT_NO": "",
        "MONEY_RECEIPT_NON": "",
        "CREATED_BY": "",
        "BOUNCE_BY": "",
        "IN_TRANSIT_BY": "",
        "CREATED_DATE": "",
        "IN_TRANSIT_DATE": "",
        "IN_TRANSIT_DATE_INFO": $filter('date')(new Date(), 'dd-MMM-yyyy'),
        "CHEQUE_IN_HAND": false,
        "CHEQUE_IN_TRANSIT": new Boolean(false),
        "DIRECT_BOUNCE": false,
        "CHECK_RETURN": false,
        "CHECK_RETURN_DATE": "",
        "SELECTED_ACCOUNT": "",
        "ACCOUNT_CONFIRM": false,
        "IS_UPDATE": false,
        "ENCASH_DATE": "",
        "DAYS": "",
        "ENCASH_REMARK": "",
        "IS_ODC": "",
        "ACC_CODE": "",
        "MASTER_ACC_CODE":"",
        "MASTER_TRANSACTION_TYPE":"",
        "MASTER_AMOUNT":"",
        "TRANSACTION_TYPE": "",
        "MAPPED_ACC_CODE": "",
        "MAPPED_ACC_EDESC":""
    };

    $scope.UpdateEncashObj = {
        "ENCASH_DATE": "",
        "DAYS": "",
        "ENCASH_REMARK": ""
    };

    $scope.ISODC = false;
    $scope.ISODCChecked = false;
    $scope.toggleChequeDate = function () {
        $scope.ISODC = $scope.ISODCChecked;
        $scope.PDCFormObject.IS_ODC = $scope.ISODCChecked;
    };

    var dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                method: "POST",
                url: "/api/CustomFormApi/GetAllPDCFormDetail",
                dataType: "json",
               // data: { dte: $('#txtEMMDate').val() }
            },
            parameterMap: function (options, operation) {
                if (operation === "read") {
                    $scope.PDCFilter.Customer = _.pluck($scope.multiSelectedCustomer, 'id').join(','); 
                    $scope.PDCFilter.Dealer = _.pluck($scope.multiSelectedDealer, 'id').join(','); 
                    $scope.PDCFilter.PDCType = _.pluck($scope.multiSelectedPDCType, 'id').join(','); 
                    return $scope.PDCFilter;
                }
                return options;
            }

        },
        schema: {
            model: {
                fields: {
                    STATUS: { type: "string" },
                    PDC_AMOUNT: { type: "number" }
                }
            }
        },
        pageSize: 10,
        group: {
            field: "STATUS", aggregates: [
                { field: "PDC_AMOUNT", aggregate: "sum" },
            ]
        },
        //aggregate: [
        //    { field: "PDC_AMOUNT", aggregate: "sum" },
        //]

    });
    var checkedItems = [];

   // $scope.hideVoucherColumn = true;
    $scope.chequeInHandCount = 0;
    $scope.encashCount = 0;
    $scope.bounceCount = 0;

    $scope.bindPDCODCGrid = function () {

        $scope.postDataCheckGridOptions = {
            //toolbar: [{ template: kendo.template($("#template").html())}],
            //toolbar: kendo.template($("#template").html()),
            toolbar: kendo.template($("#template").html()),
            excel: {
                fileName: "PdcOdcExport.xlsx"
            },
           // dataSource: dataSource,
            dataSource: new kendo.data.DataSource({
                transport: {
                    read: {
                        method: "POST",
                        url: "/api/CustomFormApi/GetAllPDCFormDetail",
                        dataType: "json",
                        // data: { dte: $('#txtEMMDate').val() }
                    },
                    parameterMap: function (options, operation) {
                        if (operation === "read") {
                            $scope.PDCFilter.Customer = _.pluck($scope.multiSelectedCustomer, 'id').join(',');
                            $scope.PDCFilter.Dealer = _.pluck($scope.multiSelectedDealer, 'id').join(',');
                            $scope.PDCFilter.PDCType = _.pluck($scope.multiSelectedPDCType, 'id').join(',');
                            //$scope.PDCAmount.PDCAmount =
                            return $scope.PDCFilter;
                        }
                        return options;
                    }

                },
                schema: {
                    model: {
                        fields: {
                            STATUS: { type: "string" },
                            PDC_AMOUNT: { type: "number" }
                        }
                    }
                },
                pageSize: 10,
                group: {
                    field: "STATUS", aggregates: [
                        { field: "PDC_AMOUNT", aggregate: "sum" },
                    ]
                },
        
            }),
            scrollable: true,
            pageable: true,
            selectable: 'row',
            //change: onChange,
            dataBound: function (e) {
                $("#postDataCheckGrid tbody tr").css("cursor", "pointer");
                var grid = e.sender;
                // grid.hideColumn("VOUCHER_NO");
                if (grid.dataSource.total() === 0) {
                    var colCount = grid.columns.length + 1;
                    $(e.sender.wrapper)
                        .find('tbody')
                        .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, No Data Found For Given Filter. </td></tr>');
                }

                //var grid = $("#grid").data("kendoGrid");
                var gridData = grid.dataSource.view();
                for (var i = 0; i < gridData.length; i++) {
                    if (gridData[i].value == "ENCASH") $scope.encashCount++;
                    if (gridData[i].value == "BOUNCE DATE") $scope.bounceCount++;
                    else $scope.chequeInHandCount++;

                    //if (gridData[i].items.VOUCHER_NO) {
                    //    grid.showColumn("VOUCHER_NO");
                    //} else {
                    //    grid.hideColumn("VOUCHER_NO");
                    //}
                }
            },
            columns: [
                {
                    field: "RECEIPT_NO", title: "Recpt. No.", width: 50
                },
                {
                    field: "RECEIPT_DATE", title: "Recpt. Date", width: 70,
                    template: "#= kendo.toString(kendo.parseDate(RECEIPT_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #"
                },
                { field: "CHEQUE_NO", title: "Cheq. No", width: 60 },
                {
                    field: "CHEQUE_DATE", title: "Cheq. Date", width: 60,
                    template: "#= kendo.toString(kendo.parseDate(CHEQUE_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #"
                },
                {
                    field: "ENCASH_DATE", title: "Encash Date", width: 60,
                    template: "#= kendo.toString(kendo.parseDate(ENCASH_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') == null ? 'N/A' : kendo.toString(kendo.parseDate(ENCASH_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #"

                },
                { field: "CUSTOMER", title: "Customer", width: 60 },
                { field: "DEALER", title: "Dealer", width: 80 },
                { field: "PDC_AMOUNT", title: "PDC Amount", width: 80, aggregates: ["sum"], groupFooterTemplate: "Total: #=sum#" },
                { field: "PDC_DETAILS", title: "PDC Detail", width: 80 },
                { field: "BANK_NAME", title: "Bank Name", width: 80 },
                { field: "REMARKS", title: "Remarks", width: 60 },
                //{ field: "IS_ODC", title: "Is ODC", width: 60 },
                {
                    field: "VOUCHER_NO", title: "Voucher No", width: 120
                },
                {
                    title: "Action ",
                    template: '<a class="fa fa-pencil-square-o editAction" title="Edit"  ng-click="EditPDCDetail(dataItem.RECEIPT_NO)"><span class="sr-only"></span> </a><a class="fa fa-trash deleteAction" title="Delete" ng-click="DeletePDCDetail(dataItem.RECEIPT_NO)"><span class="sr-only"></span> </a>',
                    width: "60px"
                }

            ],
        };

    };

    //var grid = $("#postDataCheckGrid").data("kendoGrid");
    //grid.hideColumn("VOUCHER_NO");

    $scope.bindPDCODCGrid();

    $scope.rebindPDCODCGrid = function () {
        var dataSourceL = new kendo.data.DataSource({
            transport: {
                read: {
                    method: "POST",
                    url: "/api/CustomFormApi/GetAllPDCFormDetail",
                    dataType: "json"
                },
                parameterMap: function (options, operation) {
                    if (operation === "read") {
                        $scope.PDCFilter.Customer = _.pluck($scope.multiSelectedCustomer, 'id').join(',');
                        $scope.PDCFilter.Dealer = _.pluck($scope.multiSelectedDealer, 'id').join(',');
                        $scope.PDCFilter.PDCType = _.pluck($scope.multiSelectedPDCType, 'id').join(',');
                        return $scope.PDCFilter;
                    }
                    return options;
                }

            },
            schema: {
                model: {
                    fields: {
                        STATUS: { type: "string" },
                        PDC_AMOUNT: { type: "number" }
                    }
                }
            },
            pageSize: 10,
            group: {
                field: "STATUS", aggregates: [
                    { field: "PDC_AMOUNT", aggregate: "sum" },
                ]
            },
            //aggregate: [
            //    { field: "PDC_AMOUNT", aggregate: "sum" },
            //]

        });
        var grid = $("#postDataCheckGrid").data("kendoGrid");
        grid.setDataSource(dataSourceL);
       // $("#postDataCheckGrid").data("kendoGrid").read();
    };

    $scope.contextSelect= function (e) {
            var row = $(e.target).parent()[0];
            console.log("selected Row============>>>" + JSON.stringify(row));
            var grid = $("#postDataCheckGrid").data("kendoGrid");
            var item = e.item.id;
            console.log("Row Item============>>>" + JSON.stringify(grid.dataItem(grid.select())));
            switch (item) {
                case "new":
                    $scope.createNewPDCForm();
                    break;
                case "edit":
                    $scope.EditPDCDetail(grid.dataItem(grid.select()).RECEIPT_NO);
                    break;
                case "delete":
                    $scope.DeletePDCDetail(grid.dataItem(grid.select()).RECEIPT_NO);
                    break;
                case "duplicate":
                    $scope.duplicateGridRow(grid.dataItem(grid.select()));
                    break;
                case "chequeInTransit":
                    $scope.IsEncash = false;
                    $scope.ReturnToParty = false;
                    $scope.DirectBounce = false;
                    $scope.IscheckInHand = false;
                    $scope.IsCheckInTransit = true;
                    $scope.EditPDCDetail(grid.dataItem(grid.select()).RECEIPT_NO, "intransit");
                    break;
                case "encash":
                    $scope.IsCheckInTransit = false;
                    $scope.IsEncash = true;
                    $scope.ReturnToParty = false;
                    $scope.DirectBounce = false;
                    $scope.IscheckInHand = false;
                    $scope.EditPDCDetail(grid.dataItem(grid.select()).RECEIPT_NO,"encash");
                    break;
                case "generateVoucher":
                    $("#btnGenerateVoucher").removeAttr("disabled");
                    $scope.EditPDCDetail(grid.dataItem(grid.select()).RECEIPT_NO, "generateVoucher");
                   // $("#postDataCheckModal").toggle();
                    break;
                case "bounceEncashPDC":
                    $scope.IsCheckInTransit = false;
                    $scope.IsEncash = false;
                    $scope.ReturnToParty = false;
                    $scope.DirectBounce = true;
                    $scope.IscheckInHand = false;
                    $scope.IsBounce = true;
                    $scope.EditPDCDetail(grid.dataItem(grid.select()).RECEIPT_NO, "bounce");
                    break;
                case "Reject":
                    $scope.IsCheckInTransit = false;
                    $scope.IsEncash = false;
                    $scope.ReturnToParty = false;
                    $scope.IscheckInHand = false;
                    $scope.DirectBounce = true;
                   // $scope.$apply();
                    $("#postDataCheckModal").toggle();
                    $("#bounceDate").removeAttr("disabled");
                    var todayDate = kendo.toString(kendo.parseDate(new Date()), 'MM/dd/yyyy');
                    $scope.PDCFormObject.BOUNCE_DATE = todayDate;
                    $("#bounceDate").data("kendoDatePicker").value(todayDate);
                    $("#bounceDate").data("kendoDatePicker").trigger("change");
                    $scope.EditPDCDetail(grid.dataItem(grid.select()).RECEIPT_NO, "bounce");
                    break;
                case "returnToParty":
                    $scope.IsCheckInTransit = false;
                    $scope.IsEncash = false;
                    $scope.DirectBounce = false;
                    $scope.IscheckInHand = false;
                    $scope.ReturnToParty = true;
                    $scope.EditPDCDetail(grid.dataItem(grid.select()).RECEIPT_NO, "returnToParty");
                    break;
                default:
                    break;
            }
    }

    $scope.clearDropDown= function(){

        $scope.PDCFormObject.CUSTOMER_CODE = "";
        $scope.PDCFormObject.MONEY_RECEIPT_ISSUED_BY = "";
        $scope.PDCFormObject.PARTY_TYPE = "";

    };

    $scope.setPDCOjectFromContext = function (dataItem) {
        $scope.PDCAction = "Update";
        $scope.PDCFormObject = dataItem;
        $scope.PDCFormObject.IS_UPDATE = true;
        $scope.PDCFormObject.CUSTOMER_CODE = { "TYPE_CODE": dataItem.CUSTOMER_CODE };
        $scope.PDCFormObject.MONEY_RECEIPT_ISSUED_BY = { "TYPE_CODE": dataItem.MONEY_RECEIPT_ISSUED_BY };
        $scope.PDCFormObject.PARTY_TYPE = { "PARTY_TYPE_CODE": dataItem.PARTY_TYPE };
        $scope.PDCFormObject.MONEY_RECEIPT_NO = Number(dataItem.MONEY_RECEIPT_NO);
        $scope.PDCFormObject.PARTY_BANK_NAME = dataItem.BANK_NAME;
        $scope.PDCFormObject.REMINDER_PRIOR_DAYS = Number(dataItem.REMINDER_PRIOR_DAYS);
    };
   
    $scope.PdcTypeList = [];
    $scope.PdcTypeList.push(
        
        {
            "id": "02",
            "label": "Cheque In Hand",
            "TypeCode": "02",
            "TypeName": "Cheque In Hand"
        },
        {
            "id": "03",
            "label": "Cheque In Transit",
            "TypeCode": "03",
            "TypeName": "Cheque In Transit"
        },
        {
            "id": "04",
            "label": "Encash",
            "TypeCode": "04",
            "TypeName": "Encash"
        },
        {
            "id": "05",
            "label": "Bounce",
            "TypeCode": "05",
            "TypeName": "Bounce"
        },
        {
            "id": "06",
            "label": "Return",
            "TypeCode": "06",
            "TypeName": "Return"
        });

    $scope.EditPDCDetail = function (pdcId,status='') {
        $scope.PDCAction = "Update";
        var editPdcdUrl = window.location.protocol + "//" + window.location.host + "/api/CustomFormApi/EditPDCFormDetail?pdcId=" + pdcId + "&pdcStatus="+status;
        //var gridObject = $("#postDataCheckGrid");
        //var grid = $("#postDataCheckGrid").data("kendoGrid");
        //var dataItem = grid.dataItem(this.wrapper.parents('tr'))
        //console.log("dataITemWrapper===========>>>" + JSON.stringify(dataItem));
        $http({
            method: 'GET',
            url: editPdcdUrl,

        }).then(function successCallback(response) {

            console.log("Response To Edit===========>>" + JSON.stringify(response));
            $scope.PDCFormObject = response.data;

            if (response.data == null) {
                displayPopupNotification("Please encash check before any transaction", "error");
                return;
            }
            $("#postDataCheckModal").toggle();
            $scope.PDCFormObject.IS_UPDATE = true;
            $scope.PDCFormObject.CUSTOMER_CODE = { "TYPE_CODE": response.data.CUSTOMER_CODE };
            $scope.PDCFormObject.MONEY_RECEIPT_ISSUED_BY = { "TYPE_CODE": response.data.MONEY_RECEIPT_ISSUED_BY };
            $scope.PDCFormObject.PARTY_TYPE = { "PARTY_TYPE_CODE": response.data.PARTY_TYPE };
            $scope.PDCFormObject.MONEY_RECEIPT_NO = Number(response.data.MONEY_RECEIPT_NO);
            $scope.AccMapWithCustomer = response.data.MAPPED_ACC_EDESC;
            $("#custAccount").text($scope.text_truncate($scope.AccMapWithCustomer, 15));
            $("#custAccount").prop('title', $scope.AccMapWithCustomer);

            if (status == "encash") {

                $("#encashDate").removeAttr("disabled");
                $("#btnUpdateEncash").removeClass("disabled");
                var todayDate = kendo.toString(kendo.parseDate(new Date()), 'MM/dd/yyyy');
                $scope.PDCFormObject.ENCASH_DATE = todayDate;
                $("#encashDate").data("kendoDatePicker").value(todayDate);
                $("#encashDate").data("kendoDatePicker").trigger("change");

            }
            else if (status == "intransit") {
                $("#btnUpdateEncash").addClass("disabled")
                $("#inTransitDate").removeAttr("readonly");
                var todayDate = kendo.toString(kendo.parseDate(new Date()), 'MM/dd/yyyy');
                $scope.PDCFormObject.IN_TRANSIT_DATE = todayDate;
                $("#inTransitDate").data("kendoDatePicker").value(todayDate);
                $("#inTransitDate").data("kendoDatePicker").trigger("change");

            }
            else if (status == "bounce") {

                $("#bounceDate").removeAttr("disabled");
                var todayDate = kendo.toString(kendo.parseDate(new Date()), 'MM/dd/yyyy');
                $scope.PDCFormObject.BOUNCE_DATE = todayDate;
                $scope.PDCFormObject.BOUNCE_DATE_INFO = todayDate;
                $("#bounceDate").data("kendoDatePicker").value(todayDate);
                $("#bounceDate").data("kendoDatePicker").trigger("change");
                $("#bounceDateInfo").data("kendoDatePicker").value(todayDate);
                $("#bounceDateInfo").data("kendoDatePicker").trigger("change");

            }
            else if (status == "returnToParty") {

                $("#chequeReturnDate").removeAttr("disabled");
                var todayDate = kendo.toString(kendo.parseDate(new Date()), 'MM/dd/yyyy');
                $scope.PDCFormObject.CHECK_RETURN_DATE = todayDate;
                
                $("#chequeReturnDate").data("kendoDatePicker").value(todayDate);
                $("#chequeReturnDate").data("kendoDatePicker").trigger("change");
                

            }
            else if (status == "generateVoucher") {

            }


        }, function errorCallback(response) {
            displayPopupNotification(response.data.STATUS_CODE, "error");

        });
    };

    $scope.DeletePDCDetail = function (pdcId) {
        bootbox.confirm({
            title: "Delete",
            message: "Are you sure?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success',
                    label: '<i class="fa fa-check"></i> Yes',
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger',
                    label: '<i class="fa fa-times"></i> No',
                }
            },
            callback: function (result) {

                if (result == true) {

                    var delUrl = window.location.protocol + "//" + window.location.host + "/api/CustomFormApi/DeletePDCFormDetail?pdcId=" + pdcId;
                    $http({
                        method: 'POST',
                        url: delUrl
                    }).then(function successCallback(response) {

                        if (response.data == "Successful") {
                            var grid = $("#postDataCheckGrid").data("kendoGrid");
                            grid.dataSource.read();
                            bootbox.hideAll();
                            displayPopupNotification("Data succesfully deleted ", "success");
                        }
                    }, function errorCallback(response) {
                        $scope.refresh();
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                    });

                }
                else if (result == false) {


                    bootbox.hideAll();
                }

            }
        });
    };
    $scope.AccMapWithCustomer = "";

    $scope.UpdateEncash = function () {

        var cheqDate = $filter('date')($scope.PDCFormObject.CHEQUE_DATE,'dd-MMM-yyyy');
        var todays = $filter('date')(new Date(), 'dd-MMM-yyyy');
        console.log("cheqDate====>>" + cheqDate);
        console.log("today====>>" + todays);
        if (cheqDate > todays) {
            displayPopupNotification("Cheque date must not greater then today!!");
            return;
        } else if (cheqDate < todays) {
            displayPopupNotification("Cheque date must not less then today!!");
            return;
        }
        else {
            $scope.PDCFormObject.IS_UPDATE = true;
            if ($scope.PDCFormObject.CUSTOMER_CODE !== null && $scope.PDCFormObject.CUSTOMER_CODE !== "undefined" && $scope.PDCFormObject.CUSTOMER_CODE !== undefined) {

                $scope.PDCFormObject.CUSTOMER_CODE = $scope.PDCFormObject.CUSTOMER_CODE.TYPE_CODE;
                $scope.PDCFormObject.PARTY_TYPE = $scope.PDCFormObject.PARTY_TYPE.PARTY_TYPE_CODE;

            } else {
                displayPopupNotification("Customer & Party type is required : ", "error");
                return;
            }

            if (!$scope.PDCFormObject.hasOwnProperty('PDC_DETAILS')) {
                displayPopupNotification("PDC Detail is required : ", "error");
                return;
            }


            if ($scope.PDCFormObject.MONEY_RECEIPT_ISSUED_BY !== null && $scope.PDCFormObject.MONEY_RECEIPT_ISSUED_BY !== "undefined" && $scope.PDCFormObject.MONEY_RECEIPT_ISSUED_BY !== undefined) $scope.PDCFormObject.MONEY_RECEIPT_ISSUED_BY = $scope.PDCFormObject.MONEY_RECEIPT_ISSUED_BY.TYPE_CODE; 
            $scope.TempPDCFormObject = $scope.PDCFormObject;
            var saveResponse = postDataChequeService.saveNewPDCForm($scope.PDCFormObject);
            saveResponse.then(function (savRes) {
                if (savRes.data === "Successful") {
                    bootbox.confirm({
                        title: "Generate Voucher",
                        message: "Are you sure?",
                        buttons: {
                            confirm: {
                                label: 'Yes',
                                className: 'btn-success',
                                label: '<i class="fa fa-check"></i> Yes',
                            },
                            cancel: {
                                label: 'No',
                                className: 'btn-danger',
                                label: '<i class="fa fa-times"></i> No',
                            }
                        },
                        callback: function (result) {
                            if (result == true) {
                                $("#btnGenerateVoucher").removeAttr("disabled");
                                console.log("selectdaAC====>>" + JSON.stringify($scope. PDCFormObject.SELECTED_ACCOUNT));
                                console.log("accountConf====>>" + JSON.stringify($scope.PDCFormObject.ACCOUNT_CONFIRM));

                                if ($scope.PDCFormObject.SELECTED_ACCOUNT) {

                                    if ($scope.PDCFormObject.ACCOUNT_CONFIRM) {

                                    } else {
                                        displayPopupNotification("Please confirm account", "error");
                                        return;
                                    }

                                } else {
                                    displayPopupNotification("Please select account before voucher generation!!", "error");
                                    return;
                                }

                                //var delUrl = window.location.protocol + "//" + window.location.host + "/api/CustomFormApi/DeletePDCFormDetail?pdcId=" + pdcId;
                                //$http({
                                //    method: 'POST',
                                //    url: delUrl
                                //}).then(function successCallback(response) {

                                //    if (response.data == "Successful") {
                                //        var grid = $("#postDataCheckGrid").data("kendoGrid");
                                //        grid.dataSource.read();
                                //        bootbox.hideAll();
                                //        displayPopupNotification("Data succesfully deleted ", "success");
                                //    }
                                //}, function errorCallback(response) {
                                //    $scope.refresh();
                                //    displayPopupNotification(response.data.STATUS_CODE, "error");
                                //});

                            }
                            else if (result == false) {
                                bootbox.hideAll();
                                displayPopupNotification("Encash updated successfullyl", "success");
                                setTimeout(function () {
                                    location.reload(true);
                                },2000);
                            }

                        }
                    });
                } else {
                    displayPopupNotification("Error while saving PDC/ODC detail: ", "error");
                }
            }); 
        }
    }


    $scope.generatePdcOdcVoucher = function () {
        bootbox.confirm({
            title: "Generate Voucher",
            message: "Are you sure?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success',
                    label: '<i class="fa fa-check"></i> Yes',
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger',
                    label: '<i class="fa fa-times"></i> No',
                }
            },
            callback: function (result) {
                if (result == true) {
                    $("#btnGenerateVoucher").removeAttr("disabled");
                    if ($scope.PDCFormObject.SELECTED_ACCOUNT) {

                        if ($scope.PDCFormObject.ACCOUNT_CONFIRM) {
                            console.log("PDCFORMOBJECTINSIDE====>>" + JSON.stringify($scope.PDCFormObject));
                            console.log("TEMP OBJ====>>" + JSON.stringify($scope.TempPDCFormObject));
                            $scope.PDCFormObject.ACC_CODE = $scope.PDCFormObject.SELECTED_ACCOUNT.ACC_CODE;
                            $scope.PDCFormObject.MASTER_ACC_CODE = $scope.PDCFormObject.SELECTED_ACCOUNT.MASTER_ACC_CODE;

                            var voucherNO = postDataChequeService.generatePdcOdcVoucher($scope.PDCFormObject);
                            voucherNO.then(function (vRes) {
                                console.log("VRS===================>>>" + JSON.stringify(vRes));
                                if (vRes.data.Status == "Successfull") {
                                    DisplayBarNotificationMessage(vRes.data.Message);
                                    setTimeout(function () {
                                        location.reload(true);
                                    }, 2000);
                                } else {
                                    displayPopupNotification(vRes.data.Message, "error");
                                    return;
                                }

                            }, function errorCallback(response) {
                                displayPopupNotification(response.data.ExceptionMessage, "error");
                                setTimeout(function () {
                                    location.reload(true);
                                }, 4000);
                            });
                            


                        } else {
                            displayPopupNotification("Please confirm account", "error");
                            return;
                        }

                    } else {
                        displayPopupNotification("Please select account before voucher generation!!", "error");
                        return;
                    }
                }
                else if (result == false) {
                    bootbox.hideAll();
                    displayPopupNotification("Encash updated successfullyl", "success");
                    setTimeout(function () {
                        location.reload(true);
                    }, 2000);
                }

            }
        });
    };

    $scope.duplicateGridRow = function (dataItem) {
        $("#postDataCheckGrid").data("kendoGrid").dataSource.add(dataItem);
    };
    $scope.customerChanged = function () {
        console.log("customer:============>" + JSON.stringify($scope.PDCFormObject.CUSTOMER_CODE));
        $scope.PDCFormObject.PARTY_TYPE = { "PARTY_TYPE_CODE": $scope.PDCFormObject.CUSTOMER_CODE.PARTY_TYPE_CODE };
        $scope.AccMapWithCustomer = $scope.PDCFormObject.CUSTOMER_CODE.ACC_EDESC;
       
        $("#custAccount").text($scope.text_truncate($scope.AccMapWithCustomer, 15));
        $("#custAccount").prop('title', $scope.AccMapWithCustomer);

        $scope.PDCFilter.Customer = $scope.PDCFormObject.CUSTOMER_CODE.CUSTOMER_CODE;
       // $scope.PDCFilter.Customer = $scope.PDCFormObject.CUSTOMER_CODE.CUSTOMER_CODE;
    };

    $scope.generateNewReceiptNo = function () {
        var newReceiptNo = postDataChequeService.generateNewReceiptNo();
        newReceiptNo.then(function (nrRes) {
            $scope._newReceiptNo = nrRes.data[0];
            $scope.PDCFormObject.RECEIPT_NO = nrRes.data[0];
            $scope.PDCFormObject.CREATED_BY = nrRes.data[1];
            $scope.PDCFormObject.IN_TRANSIT_BY = nrRes.data[1];
            $scope.PDCFormObject.BOUNCE_BY = nrRes.data[1];
            $scope.PDCFormObject.MONEY_RECEIPT_NO = Number(nrRes.data[0]);
        });
    };
    
    $scope.generateReminderDays = function () {
        var reminderDays = postDataChequeService.generateReminderDays();
        reminderDays.then(function (rDays) {
            $scope.ReminderPriorDays = rDays.data;
        });
    };


    $scope.createNewPDCForm = function () {
        $scope.PDCFormObject = {};
        $scope.generateNewReceiptNo();
        $scope.PDCAction = "Ok";
        $scope.IscheckInHand = true;
        $("#postDataCheckModal").toggle();
       
    };

    $scope.generateReminderDays();

    $scope.closeNewPDCFormDialog = function () {
        $scope.clearDropDown();
        $("#postDataCheckModal").toggle();
    };

    $scope.getAllCustomer = function () {
        var allCust = postDataChequeService.getAllCustomer();
        allCust.then(function (custRes) {
            $scope.AllCustomer = custRes.data;
        });
    };

    $scope.getAllCustomer();

    $scope.getAllPartyType = function () {
        var allPartyType = postDataChequeService.getAllPartyType();
        allPartyType.then(function (partyRes) {
            $scope.AllPartyType = partyRes.data;
            $scope.PDCFormObject.CREATED_BY = partyRes.data.CREATED_BY;
        });
    };

    $scope.getAllPartyType();

   $scope.getAllDealerType = function () {
        var allDealerType = postDataChequeService.getAllDealerType();
        allDealerType.then(function (dealerRes) {
            $scope.AllDealerType = dealerRes.data;
        });
    };

    $scope.getAllDealerType();

     $scope.getAllEmpForCheckReceipt = function () {
        var allCheckReceiptEmp = postDataChequeService.getAllEmpForCheckReceipt();
        allCheckReceiptEmp.then(function (empRes) {
            $scope.AllEmpCheckReceipt = empRes.data;
        });
    };

    $scope.getAllEmpForCheckReceipt();

    $scope.getAllChartOfAccount = function () {
        var allAcccount = postDataChequeService.getAllChartOfAccount();
        allAcccount.then(function (accountRes) {
            $scope.AllChartOfAccount = accountRes.data;
        });
    };

    $scope.text_truncate = function (str, length, ending) {
        if (length == null) {
            length = 100;
        }
        if (ending == null) {
            ending = '...';
        }
        if (str.length > length) {
            return str.substring(0, length - ending.length) + ending;
        } else {
            return str;
        }
    };

    $scope.getAllChartOfAccount();

    $scope.dealerControl = {
      
        smartButtonMaxItems: 1,
        //checkBoxes: true,
        enableSearch: true,
        smartButtonTextProvider(selectionArray) {
            if (selectionArray.length === 1) {
                return selectionArray[0].label;
            } else {
                return selectionArray.length + ' Selected';
            }
        }
    };
    $scope.multiSelectedDealer = [];

    $scope.customerControl = {

        smartButtonMaxItems: 1,
        enableSearch: true,
        smartButtonTextProvider(selectionArray) {
            if (selectionArray.length === 1) {
                return selectionArray[0].label;
            } else {
                return selectionArray.length + ' Selected';
            }
        }
    };
    $scope.multiSelectedCustomer = [];
   

    $scope.pdcTypeControl = {
        smartButtonMaxItems: 1,
        enableSearch: true,
        smartButtonTextProvider(selectionArray) {
            if (selectionArray.length === 1) {
                return selectionArray[0].label;
            } else {
                return selectionArray.length + ' Selected';
            }
        }
    };
    $scope.multiSelectedPDCType = [];
         
    $scope.saveNewPDCForm = function () {
        console.log("$scope.PDCFormObject.PDC_DETAILS======>>>>" + JSON.stringify($scope.PDCFormObject.PDC_DETAILS));
        if ($scope.PDCFormObject.CUSTOMER_CODE !== null && $scope.PDCFormObject.CUSTOMER_CODE !== "undefined" && $scope.PDCFormObject.CUSTOMER_CODE !==undefined) {

            $scope.PDCFormObject.CUSTOMER_CODE = $scope.PDCFormObject.CUSTOMER_CODE.TYPE_CODE;
            $scope.PDCFormObject.PARTY_TYPE = $scope.PDCFormObject.PARTY_TYPE.PARTY_TYPE_CODE; 

        } else {
            displayPopupNotification("Customer & Party type is required : ", "error");
            return;
        }

        if (!$scope.PDCFormObject.hasOwnProperty('PDC_DETAILS'))
        {
            displayPopupNotification("PDC Detail is required : ", "error");
            return;
        }
                   
       
        if ($scope.PDCFormObject.MONEY_RECEIPT_ISSUED_BY !== null && $scope.PDCFormObject.MONEY_RECEIPT_ISSUED_BY !== "undefined" && $scope.PDCFormObject.MONEY_RECEIPT_ISSUED_BY !==undefined ) $scope.PDCFormObject.MONEY_RECEIPT_ISSUED_BY = $scope.PDCFormObject.MONEY_RECEIPT_ISSUED_BY.TYPE_CODE; 
       
        console.log("PDC FORM OBJECT ON SAVe=====>>>>" + JSON.stringify($scope.PDCFormObject));
        debugger;
        var saveResponse = postDataChequeService.saveNewPDCForm($scope.PDCFormObject);
       
        saveResponse.then(function (savRes) {
            if (savRes.data === "Successful") {
                if ($scope.PDCAction === "Update") DisplayBarNotificationMessage("PDC entry updated successfully");
                else DisplayBarNotificationMessage("PDC entry saved successfully");
                setTimeout(function () {
                    location.reload(true);
                },3000);
            } else {
                displayPopupNotification("Error while saving PDC/ODC detail: ", "error");
            }
        }); 
    };

    $scope.searchPDFToView = function () {
        var detailToEdit = $("#postDataCheckGrid").data("kendoGrid");
        var filter = $scope.PDCFilter;
        filter.DateEnglishFrom = $("#fromInputDateVoucher").val();
        filter.DateEnglishTo = $("#toInputDateVoucher").val();
        //console.log("Search Modal=======================>>> " + JSON.stringify(filter));
        $scope.$apply();
        var searchResult = postDataChequeService.searchPDCDetail(filter);
        searchResult.then(function (sRes) {
            var newDataSource = new kendo.data.DataSource({
                data: sRes.data
            });
            detailToEdit.setDataSource(newDataSource);
        });

    };

    $scope.FilterPDC = function () {
        $scope.PDCFilter.PdcStatus = $scope.rdoPdcFilter;
    };
});

DTModule.service('postDataChequeService', function ($http) {

    this.getAllCustomer = function () {
        var allCust = $http({
            method: "GET",
            url: "/api/CustomFormApi/GetCustomerForPostedDate",
            dataType: "json"
        });
        return allCust;
    };

    this.getAllPartyType = function () {
        var allPT = $http({
            method: "GET",
            url: "/api/CustomFormApi/GetAllPartyType",
            dataType: "JSON"
        });
        return allPT;
    };

  this.getAllDealerType = function () {
        var allDlr = $http({
            method: "GET",
            url: "/api/CustomFormApi/GetAllDealerType",
            dataType: "JSON"
        });
        return allDlr;
    };

    this.getAllChartOfAccount = function () {
        var allCOA = $http({
            method: "GET",
            url: "/api/SubLedgerMgmtApi/GetCharOfAccountTree",
            dataType: "JSON"
        });
        return allCOA;
    };

    this.saveNewPDCForm = function (saveParam) {
        var saveRes = $http({
            method: "POST",
            url: "/api/CustomFormApi/SaveNewPDCForm",
            data: JSON.stringify(saveParam),
            dataType: "JSON"
        });
        return saveRes;
    };

    this.searchPDCDetail = function (searchParam) {
        var searchResult = $http({
            method: "POST",
            url: "/api/CustomFormApi/SearchPDCDetail",
            data: JSON.stringify(searchParam),
            dataType: "JSON"

        });
        return searchResult;
    };

    this.generateNewReceiptNo = function () {
        var allCOA = $http({
            method: "GET",
            url: "/api/CustomFormApi/GenerateNewReceipt",
            dataType: "JSON"
        });
        return allCOA;
    };

    this.generateReminderDays = function () {
        var rDays = $http({
            method: "GET",
            url: "/api/CustomFormApi/GenerateReminderPriorDays",
            dataType: "JSON"
        });
        return rDays;
    };

     this.getAllEmpForCheckReceipt = function () {
            var allEmp = $http({
                method: "GET",
                url: "/api/SubLedgerMgmtApi/GetEmployeeSubLedger",
                dataType: "json"
            });
            return allEmp;
        };

     this.UpdateEncash = function () {
         var updatedInfo = $http({
             method: "POST",
             url: "/api/CustomFormApi/SaveNewPDCForm",
             dataType: "JSON"
         });
     };


     this.generatePdcOdcVoucher = function (parameter) {
         var searchResult = $http({
             method: "POST",
             url: "/api/CustomFormApi/GeneratePdcOdcVoucher",
             data: JSON.stringify(parameter),
             dataType: "JSON"

         });
         return searchResult;
     }
});