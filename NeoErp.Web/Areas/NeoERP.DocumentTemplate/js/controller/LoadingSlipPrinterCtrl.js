
DTModule.controller('LoadingSlipPrinterCtrl', function ($scope, $http, $filter) {

    $scope.FormName = "Loading Slip Generator";
    $scope.DocumentName = "SR Steel Pvt. Ltd.";
    $scope.lccode = "0";
    $scope.vehicleRegEdit = false;

    $scope.VehicleAndDispatchList = [];
    $scope.loadingSlipDetail = [];

    $scope.TaggedInformation = {

        DISPATCH_NO: "",
        ORDER_NO: "",
        VOUCHER_NO: "",
        ORDER_DATE: "",
        VOUCHER_DATE: "",
        FORM_CODE: "",
        ITEM_CODE: "",
        ITEM_EDESC:"",
        QUANTITY: "",
        MU_CODE: "",
        UNIT_PRICE: "",
        SERIAL_NO: "",
        MANUAL_NO: "",
        CUSTOMER_CODE: "",
        FROM_LOCATION: "",
        TO_LOCATION: "",
        DISPATCH_FLAG: "",
        COMPANY_CODE: "",
        BRANCH_CODE: "",
        CREATED_BY: "",
        CREATED_DATE: "",
        MITI: "",
        DUE_QTY: "",
        CUSTOMER_EDESC: "",
        PARTY_TYPE_CODE: "",
        ADDRESS: "",
        AGENT_CODE: "",
        PARTY_TYPE_EDESC: "",
        PLANNING_QTY: "",
        PENDING_TO_DISPATCH: "",
        PLANNING_AMOUNT: "",
        EXCISE_AMOUNT: "",
        VAT_AMOUNT: "",
        TRANS_NO: "",
        ACKNOWLEDGE_FLAG: "",
        TRANSACTION_NO: "",
        REFERENCE_NO: "",
        REFERENCE_FORM_CODE: "",
        VEHICLE_NAME: "",
        TRANSACTION_DATE: "",
        DELETED_FLAG: "",
        ACCESS_FLAG: "",
        READ_FLAG: "",
        REMARKS: "",
        VEHICLE_OWNER_NAME: "",
        VEHICLE_OWNER_NO: "",
        DRIVER_NAME: "",
        DRIVER_LICENCE_NO: "",
        DRIVER_MOBILE_NO: "",
        IN_TIME: "",
        OUT_TIME: "",
        SYN_ROWID: "",
        LOAD_IN_TIME: "",
        LOAD_OUT_TIME: "",
        MODIFY_DATE: "",
        MODIFY_BY: "",
        TOTAL_VEHICLE_HR: "",
        TEAR_WT: "",
        GROSS_WT: "",
        NET_WT: "",
        ACCESS_BY: "",
        ACCESS_DATE: "",
        DESTINATION: "",
        BROKER_NAME: "",
        VEHICLE_IN_DATE: "",
        VEHICLE_OUT_DATE: "",
        TRANSPORT_NAME: "",
        WB_SLIP_NO: "",
        TRANSPORTER_CODE: ""

    };


    $scope.loadingSlipPrinterGridOptions = {
        dataSource: {
            type: "json",
            transport: {
                read: "/api/OrderDispatchApi/GetLoadingSlipPrintList",
                parameterMap: function (options, type) {
                    if (type === 'read') {
                        return {
                            from: "LoadingSlip"
                        };
                    }
                }
            },
            pageSize: 20,
            serverSorting: true
        },
        selectable: "single",
        scrollable: true,
        searchable: true,
        height: 300,
        sortable: true,
        pageable: true,
        groupable: true,
        resizable: true,
        dataBound: function (e) {

            $("#loadingSlipPrinterGrid tbody tr").css("cursor", "pointer");

        },
        columns: [
            {
                field: "DISPATCH_NO",
                title: " S.No.",
                width: "80px"
            },
            {
                field: "REFERENCE_NO",
                title: "REFERENCE_NO",
                width: "100px"
            },
            {
                field: "CUSTOMER_EDESC",
                title: "Party Name",
                width: "100px"
            },
            {
                field: "DESTINATION",
                title: "Destination",
                width: "130px"
            },
            {
                field: "ITEM_EDESC",
                title: "Item Name",
                width: "80px"
            },
            {
                field: "VEHICLE_NAME",
                title: "Vehicle",
                width: "100px"
            },
            {
                field: "MU_CODE",
                title: "Unit",
                width: "130px"
            },
            {
                field: "QUANTITY",
                title: "Quantity",
                width: "130px"
            },
            {
                field: "UNIT_PRICE",
                title: "Rate",
                width: "130px"
            },
            {
                title: "Action ",
                template: '<a class="fa fa-check-circle editAction" title="Generate Loading Slip" ng-click="loadingSlipPrint(#:DISPATCH_NO#,this)"><span class="sr-only"></span></a>',
                width: "60px"
            }
        ],
    };


    $scope.loadingSlipPrint = function (dispatchNo, e) {

        //debugger;
        var slipPrintUrl = window.location.protocol + "//" + window.location.host + "/api/OrderDispatchApi/GetLoadingSlipPrintList?dispatchNo=" + e.dataItem.DISPATCH_NO;
        $http({
            method: 'GET',
            url: slipPrintUrl,
            data: dispatchNo
        }).then(function successCallback(response) {
            debugger;
            $scope.loadingSlipDetail = response.data;

            bootbox.confirm({
                title: "Generate Slip",
                message: "Do you want to print loading Slip [" + dispatchNo + "]",
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
                        $('#LoadingSlipPrintModal').modal('toggle');
                        // printLoadingSlip();
                        //  displayPopupNotification("you interested", 'success');

                    } else {
                        $('#LoadingSlipPrintModal').modal('toggle');
                        //displayPopupNotification("You canceled", 'success');
                    }

                }

            });

        }, function errorCallback(response) {
            displayPopupNotification("Something went wrong.Please try again later.", "error");

        });

        var dataItem = e.dataItem;
        //console.log("dataItem=========================>" + JSON.stringify(dataItem));
        $scope.TaggedInformation = dataItem;
        $scope.TaggedInformation.REFERENCE_NO = dataItem.REFERENCE_NO;
        $scope.TaggedInformation.LS_NO = dataItem.LOADING_SLIP_NO;
        $scope.TaggedInformation.VEHICLE_NAME = dataItem.VEHICLE_NAME;
        $scope.TaggedInformation.MITI = $filter('date')(new Date(dataItem.VOUCHER_DATE), 'dd-MMM-yyyy'); //dataItem.VOUCHER_DATE;
        $scope.TaggedInformation.REFERENCE_FORM_CODE = dataItem.FORM_CODE;
        $scope.TaggedInformation.TRANSACTION_NO = dataItem.TRANSACTION_NO;
        $scope.TaggedInformation.ORDER_NO = dataItem.REFERENCE_NO;
        $scope.TaggedInformation.ITEM_EDESC = dataItem.ITEM_EDESC;
       // $scope.TaggedInformation.REFERENCE_NO = dataItem.REFERENCE_NO;

       // $scope.$apply();

        //bootbox.confirm({
        //    title: "Generate Slip",
        //    message: "Do you want to print loading Slip [" + dispatchNo + "]",
        //    buttons: {
        //        confirm: {
        //            label: 'Yes',
        //            className: 'btn-success',
        //            label: '<i class="fa fa-check"></i> Yes'
        //        },
        //        cancel: {
        //            label: 'No',
        //            className: 'btn-danger',
        //            label: '<i class="fa fa-times"></i> No'
        //        }
        //    },
        //    callback: function (result) {

        //        if (result === true) {

        //            $('#LoadingSlipPrintModal').modal('toggle');

        //        } else {

        //           // $('#LoadingSlipPrintModal').modal('toggle');
        //            //this.close();

                  
        //        }

        //    }

        //});
    };

    $scope.printLoadingSlip = function (divName) {

        var printContents = document.getElementById(divName).innerHTML;

        var popupWin = window.open('', '_blank', 'width=850,height=800', 'orientation = portrait');
        popupWin.document.open();
        popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
        popupWin.document.close();

    };

    $scope.cancelPrint = function () {
        window.location.reload(true);
    };

});