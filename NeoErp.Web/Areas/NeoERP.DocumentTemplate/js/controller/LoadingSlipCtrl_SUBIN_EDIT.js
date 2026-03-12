DTModule.controller('LoadingSlipCtrl', function ($scope, $http, $window, $filter, $timeout) {

    $scope.FormName = "Loading Slip Generator";
    $scope.DocumentName = "SR Steel Pvt. Ltd.";
    $scope.lccode = "0";
    $scope.vehicleRegEdit = false;

    $scope.VehicleAndDispatchList = [];
    $scope.loadingSlipDetail = [];

    $scope.loadingVehicleObj = {
        TRANSACTION_NO:"",
        REFERENCE_NO:"" ,
        REFERENCE_FORM_CODE:"",
        VEHICLE_NAME:"",
        TRANSACTION_DATE:"",
        COMPANY_CODE: "",
        BRANCH_CODE: "",
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
        CREATED_BY: "",
        CREATED_DATE: "",
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
        QUANTITY: "",
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

    $scope.TaggedInformation = {

        DISPATCH_NO: "",
        ORDER_NO: "",
        VOUCHER_NO: "",
        ORDER_DATE: "",
        VOUCHER_DATE: "",
        FORM_CODE: "",
        ITEM_CODE: "",
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
        ITEM_EDESC: "",
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
    var checkedItems = [];
    var checkedIds = {};

    $scope.showDispatchGrid = function (event) {
        var checkedItems = [];
        var checkedIds = {};
        $("#dispatchTagModal").modal("toggle");
    };

    function rowSelected() {
      // debugger;

       var  grid = $("#vGrid").data("kendoGrid"),
            selectedItem = grid.dataItem(grid.select());
        $scope.loadingVehicleObj = selectedItem;
        //$scope.VehicleAndDispatchList.push($scope.loadingVehicleObj);
        //console.log("selectedItem======================>>>" + selectedItem);
        $scope.$apply();

        console.log("$scope.loadingVehicleObj===============>>>" + JSON.stringify($scope.loadingVehicleObj));

        $('#acknowledgeFlag').show();
    };


    function onDispatchGridChange() {
        debugger;
        $('#btnConfirmToTag').prop("disabled", false);
         var grid = $("#dGrid").data("kendoGrid"),
            selectedItem = grid.dataItem(grid.select());  

        //$scope.TaggedInformation == $scope.loadingVehicleObj;
        $scope.TaggedInformation = selectedItem;
        $scope.TaggedInformation.REFERENCE_NO = selectedItem.ORDER_NO;
        $scope.TaggedInformation.REFERENCE_FORM_CODE = selectedItem.FORM_CODE;
        $scope.TaggedInformation.TRANSACTION_NO = $scope.loadingVehicleObj.TRANSACTION_NO;
        $scope.TaggedInformation.VEHICLE_NAME = $scope.loadingVehicleObj.VEHICLE_NAME;
        $scope.TaggedInformation.VEHICLE_OWNER_NAME = $scope.loadingVehicleObj.VEHICLE_OWNER_NAME;
        $scope.TaggedInformation.VEHICLE_OWNER_NO = $scope.loadingVehicleObj.VEHICLE_OWNER_NO;
        $scope.TaggedInformation.DRIVER_NAME = $scope.loadingVehicleObj.DRIVER_NAME;
        $scope.TaggedInformation.DRIVER_MOBILE_NO = $scope.loadingVehicleObj.DRIVER_MOBILE_NO;
        $scope.TaggedInformation.TRANSPORT_NAME = $scope.loadingVehicleObj.TRANSPORT_NAME;
        $scope.TaggedInformation.IN_TIME = $scope.loadingVehicleObj.IN_TIME;
        $scope.TaggedInformation.OUT_TIME = $scope.loadingVehicleObj.OUT_TIME;
        $scope.TaggedInformation.LOAD_IN_TIME = $scope.loadingVehicleObj.LOAD_IN_TIME;
        $scope.TaggedInformation.LOAD_OUT_TIME = $scope.loadingVehicleObj.LOAD_OUT_TIME;
        $scope.TaggedInformation.VEHICLE_IN_DATE = $scope.loadingVehicleObj.VEHICLE_IN_DATE;
        $scope.TaggedInformation.VEHICLE_OUT_DATE = $scope.loadingVehicleObj.VEHICLE_OUT_DATE;
        $scope.$apply();
        console.log("$scope.loadingVehicleObj================>>>" + JSON.stringify($scope.loadingVehicleObj));

    };

    $scope.DispatchFilter = "TODAY";

    $scope.FilterDispatch = function (e) {
        showloader();

        $scope.dispatchTagGridOptions = {

            dataSource: {
                type: "json",
                transport: {
                    read: "/api/OrderDispatchApi/GetAllDispatchForLoadingSlip",
                    parameterMap: function (options, type) {
                        if (type === 'read') {
                            return {
                                filter: $scope.DispatchFilter,
                                // ...
                            };
                        }
                    }
                },
                group: {                 
                    field: "CUSTOMER_EDESC",
                },
                pageSize: 10,
                serverSorting: true,
                serverFiltering: true,
            },
            selectable: "multiple",
            scrollable: true,
            height: 350,
            sortable: true,
            pageable: true,
            groupable: true,
            resizable: true,
            //change: onDispatchGridChange,
            dataBound: function (e) {
                debugger;
                $(".checkboxdtgo").on("click", selectRow);


                //$("#dGrid tbody tr").css("cursor", "pointer");

                //$("#dGrid tbody tr").on('dblclick', function () {
                //    var vehicle = $(this).find('td span').html()
                 

                //})
            },
            columns: [
                {                 
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.DISPATCH_NO}' class='checkboxdtgo row-checkbox'><label class='k-checkbox-label' for='${dataItem.DISPATCH_NO}'></label>"
                    },
                    width: 20
                },
                { field: "DISPATCH_NO", title: "Dispatch No", width: 40 },
                { field: "ORDER_NO", title: "Order No", width: 40 },
                { field: "MITI", title: "BS Date", width: 25 },
                { field: "PARTY_TYPE_EDESC", title: "Dealer Name", width: 50 },
                { field: "CUSTOMER_EDESC", title: "Party Name", width: 50 },
                { field: "ADDRESS", title: "Destination", width: 30 },
                { field: "ITEM_EDESC", title: "Product Name", width: 40 },
                { field: "QUANTITY", title: "Order Qty.", width: 25 },
                { field: "UNIT_PRICE", title: "Rate", width: 20 },
            ]

        };

        hideloader();
        //on click of the checkbox:


        function selectRow() {
            debugger;
            var checked = this.checked,
                row = $(this).closest("tr"),
                grid = $("#dGrid").data("kendoGrid"),
                dataItem = grid.dataItem(row);

            if (checked) {
                debugger;
                row.addClass("k-state-selected");
                $(this).attr('checked', true);
                checkedIds[dataItem.DISPATCH_NO] = checked;
                //var CustomerId = "";

                //switch (switch_on) {

                //    case "SA_SALES_ORDER":
                //    case "SA_SALES_INVOICE":
                //    case "SA_SALES_CHALAN":
                        
                //        break;
                //    default:
                //}

                //break;
                //return;
                checkedItems.push({
                    "DISPATCH_NO": dataItem.DISPATCH_NO,
                    //"SERIAL_NO": dataItem.SERIAL_NO,
                    //"TABLE_NAME": $scope.RefTableName,
                    //"ITEM_CODE": dataItem.ITEM_CODE,
                    //"REF_FORM_CODE": dataItem.FORM_CODE
                    "ORDER_NO": dataItem.ORDER_NO,
                    //"FORM_CODE": dataItem.FORM_CODE,
                    //"TRANSACTION_NO" : $scope.loadingVehicleObj.TRANSACTION_NO,
                    //"VEHICLE_NAME": $scope.loadingVehicleObj.VEHICLE_NAME,
                    //"VEHICLE_OWNER_NAME": $scope.loadingVehicleObj.VEHICLE_OWNER_NAME,
                    //"VEHICLE_OWNER_NO":$scope.loadingVehicleObj.VEHICLE_OWNER_NO,
                    //"DRIVER_NAME": $scope.loadingVehicleObj.DRIVER_NAME,
                    //"DRIVER_MOBILE_NO": $scope.loadingVehicleObj.DRIVER_MOBILE_NO,
                    //"TRANSPORT_NAME": $scope.loadingVehicleObj.TRANSPORT_NAME,
                    //"IN_TIME": $scope.loadingVehicleObj.IN_TIME,
                    //"OUT_TIME":  $scope.loadingVehicleObj.OUT_TIME,
                    //"LOAD_IN_TIME":  $scope.loadingVehicleObj.LOAD_IN_TIME,
                    //"LOAD_OUT_TIME":  $scope.loadingVehicleObj.LOAD_OUT_TIME,
                    //"VEHICLE_IN_DATE":  $scope.loadingVehicleObj.VEHICLE_IN_DATE,
                    //"VEHICLE_OUT_DATE":  $scope.loadingVehicleObj.VEHICLE_OUT_DATE
                    "TRANSACTION_NO": $scope.loadingVehicleObj.TRANSACTION_NO,
                    "MITI": dataItem.MITI,
                    "PARTY_TYPE_EDESC": dataItem.PARTY_TYPE_EDESC,
                    "CUSTOMER_EDESC": dataItem.CUSTOMER_EDESC,
                    "ADDRESS": dataItem.ADDRESS,
                    "ITEM_EDESC": dataItem.ITEM_EDESC,
                    "QUANTITY": dataItem.QUANTITY,
                    "UNIT_PRICE": dataItem.UNIT_PRICE

                });
               
                debugger;
            } else {
                for (var i = 0; i < checkedItems.length; i++) {
                    if (checkedItems[i].DISPATCH_NO == dataItem.DISPATCH_NO && checkedItems[i].ORDER_NO == dataItem.ORDER_NO) {
                        checkedItems.splice(i, 1);
                    }
                }
                row.removeClass("k-state-selected");
            }
            if (checkedItems.length > 0) {
                $('#btnConfirmToTag').prop('disabled', false);
            }
           
        }
    };

    $scope.FilterDispatch();

    $scope.vehicleCenterChildGridOptions = {
        dataSource: {
            type: "json",
            transport: {
                read: "/api/OrderDispatchApi/GetRegisteredVehicle",
                parameterMap: function (options, type) {
                    if (type === 'read') {
                        return {
                            from: "LoadingSlip",
                        };
                    }
                }
            },
            pageSize: 20,
            serverSorting: true
        },
        selectable: "single",
        scrollable: true,
        searchable:true,
        height: 300,
        sortable: true,
        pageable: true,
        groupable: true,
        resizable: true,
        change: rowSelected,
        dataBound: function (e) {
         
            $("#kGrid tbody tr").css("cursor", "pointer");

            //$("#kGrid tbody tr").on('dblclick', function () {
            //    var vehicle = $(this).find('td span').html()
            //    $scope.edit(vehicle);

            //})

            //$("#kGrid tbody tr").on('click', function () {
            //    rowSelected(this);

            //});

        },
        columns: [
            {
                field: "TRANSACTION_NO",
                title: " Tran No.",
                width: "80px"
            },
            {
                field: "TRANSACTION_DATE",
                title: "Date",
                template: "#= kendo.toString(kendo.parseDate(TRANSACTION_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
                width: "100px"
            },
            {
                field: "VEHICLE_NAME",
                title: "Vehicle No",
                width: "100px"
            },
            {
                field: "DESTINATION",
                title: "Destination",
                width: "130px"
            },
            {
                field: "ACCESS_FLAG",
                title: "Acknowledge",
                width: "80px"
            },
            {
                field: "REFERENCE_NO",
                title: "Reference/Dispatch No.",
                width: "130px"
            },
            {
                field: "REMARKS",
                title: "Remarks",
                width: "130px"
            },
            {
                field: "DESTINATION",
                title: "Destination",
                width: "130px"
            },
            {
                field: "WB_SLIP_NO",
                title: "L.S. No.",
                width: "100px"
            }
            //{
            //    title: "Action ",
            //    template: '<a class="fa fa-pencil-square-o editAction" title="Edit" ng-click="edit(#:TRANSACTION_NO#)"><span class="sr-only"></span> </a><a class="fa fa-trash deleteAction" title="Delete" ng-click="delete(#:TRANSACTION_NO#)"><span class="sr-only"></span> </a>',
            //    width: "60px"
            //}
        ],


    };


    $scope.ConfirmToTag = function () {
        debugger;
        $scope.TaggedInformation = checkedItems;
        if ($scope.TaggedInformation[0].ORDER_NO) {
            debugger;
            $("#dispatchTagModal").modal('toggle');
            $scope.BindSlipGenerator($scope.TaggedInformation);
        }

        //var grid = $("#lGrid").data("kendoGrid"),
        //    selectedItem = grid.dataItem(grid.select());
        //$scope.loadingVehicleObj = selectedItem;
        //console.log("selectedItem======================>>>" + selectedItem);
        //if (selectedItem.TRANSACTION_NO && selectedItem.REFERENCE_NO) {
        //    $("#dispatchTagModal").modal('toggle');
        //    $scope.BindSlipGenerator();
        //}
        //$scope.$apply();
    };

    $scope.BindSlipGenerator = function (data) {
        debugger;
        dData = [];
        dData.push(data);
        console.log("dataForDispatch===========>> " + JSON.stringify(dData));
        $scope.loadingSlipPrintGridOptions = {
            dataSource: new kendo.data.DataSource({
               //data: dData
                data: data
            }),
            selectable: "single",
            scrollable: true,
            height: 350,
            sortable: true,
            pageable: true,
            groupable: true,
            resizable: true,
            dataBound: function (e) {
                // debugger;
                $("#dGrid tbody tr").css("cursor", "pointer");

               
            },
            columns: [
                {
                    field: "DISPATCH_NO",
                    title: " S.No.",
                    width: "80px"
                },
                {
                    field: "ORDER_NO",
                    title: "Order No.",
                    width: "100px"
                },
                {
                    field: "PARTY_TYPE_EDESC",
                    title: "Party Name",
                    width: "100px"
                },
                {
                    field: "ADDRESS",
                    title: "Destination",
                    width: "130px"
                },
                {
                    field: "ITEM_EDESC",
                    title: "Item Name",
                    width: "80px"
                },
                //{
                //    field: "MU_CODE",
                //    title: "Unit",
                //    width: "130px"
                //},
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
                    template: '<a class="fa fa-check-circle editAction" title="Generate Loading Slip" ng-click="loadingSlipPrint(#:DISPATCH_NO#,dataItem.ORDER_NO)"><span class="sr-only"></span></a>',
                    width: "60px"
                }
            ],


        };
    };


    $scope.generateLoadingSlipPrint = function () {


    };

    $scope.checkSelectedData = function () {
        if (!$scope.loadingVehicleObj.TRANSACTION_NO) {
            displayPopupNotification("Please select the data which you want to edit or modify", "error");
        } else {
            var updateurl = window.location.protocol + "//" + window.location.host + "/api/setupapi/updateVehicleRegistration";

            $scope.saveupdatebtn = "Update";
            var model = {
                TRANSACTION_NO: $scope.loadingVehicleObj.TRANSACTION_NO,
                VEHICLE_NAME: $scope.loadingVehicleObj.VEHICLE_NAME,
                REMARKS: $scope.loadingVehicleObj.REMARKS,
                VEHICLE_OWNER_NAME: $scope.loadingVehicleObj.VEHICLE_OWNER_NAME,
                VEHICLE_OWNER_NO: $scope.loadingVehicleObj.VEHICLE_OWNER_NO,
                DRIVER_NAME: $scope.loadingVehicleObj.DRIVER_NAME,
                DRIVER_LICENCE_NO: $scope.loadingVehicleObj.DRIVER_LICENCE_NO,
                DRIVER_MOBILE_NO: $scope.loadingVehicleObj.DRIVER_MOBILE_NO,
                IN_TIME: $scope.loadingVehicleObj.IN_TIME,
                OUT_TIME: $scope.loadingVehicleObj.OUT_TIME,
                LOAD_IN_TIME: $scope.loadingVehicleObj.LOAD_IN_TIME,
                LOAD_OUT_TIME: $scope.loadingVehicleObj.LOAD_OUT_TIME,
                TEAR_WT: $scope.loadingVehicleObj.TEAR_WT,
                QUANTITY: $scope.loadingVehicleObj.QUANTITY,
                GROSS_WT: $scope.loadingVehicleObj.GROSS_WT,
                NET_WT: $scope.loadingVehicleObj.NET_WT,
                DESTINATION: $scope.loadingVehicleObj.DESTINATION,
                BROKER_NAME: $scope.loadingVehicleObj.BROKER_NAME,
                VEHICLE_IN_DATE: $scope.loadingVehicleObj.VEHICLE_IN_DATE,
                VEHICLE_OUT_DATE: $scope.loadingVehicleObj.VEHICLE_OUT_DATE,
                WB_SLIP_NO: $scope.loadingVehicleObj.WB_SLIP_NO,
                TRANSPORT_NAME: $scope.loadingVehicleObj.TRANSPORT_NAME,
                TRANSACTION_DATE: $scope.loadingVehicleObj.TRANSACTION_DATE,
                TOTAL_VEHICLE_HR: $scope.loadingVehicleObj.TOTAL_VEHICLE_HR,
            }
            $http({
                method: 'post',
                url: updateurl,
                data: model
            }).then(function successcallback(response) {

                
                if (response.data.MESSAGE == "UPDATED") {
                   
                    displayPopupNotification("data succesfully updated ", "success");
                }
                if (response.data.MESSAGE == "error") {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            }, function errorcallback(response) {

                displayPopupNotification("Something went wrong.Please try again later.", "error");

            });
        }
    };

    $scope.loadingSlipPrint = function (dispatchNo, orderNo) {
        debugger;
        var result = $scope.TaggedInformation.filter((x) => { return x.ORDER_NO == orderNo; });
       // var isSlipSave = generateLoadingSlipPrint();
        var model = JSON.stringify(result[0]);
        
        var saveTypeUrl = window.location.protocol + "//" + window.location.host + "/api/OrderDispatchApi/GenerateLoadingSlip";
        $http({
            method: 'POST',
            url: saveTypeUrl,
            //data: $scope.TaggedInformation
            data: model
        }).then(function successCallback(response) {
          
            if (response.data.MESSAGE == "ERROR") {
                displayPopupNotification("Error in save.", "error");

            }
            else {
                DisplayBarNotificationMessage("Loading Slip has been generated successfully");
                console.log("Print loadingSllip", $scope.TaggedInformation);
                $scope.lccode = response.data.MESSAGE;
                $scope.loadingSlipDetail = response.data.data;
                console.log("check data", response.data);
                console.log("check data scope", $scope.loadingSlipDetail);
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
            }

        }, function errorCallback(response) {
            displayPopupNotification("Something went wrong.Please try again later.", "error");

        });


        //if (isSlipSave) {
        //    DisplayBarNotificationMessage("Loasing Slip has been generated successfully");
        //    bootbox.confirm({
        //        title: "Delete",
        //        message: "Do you want to print loading Slip ["+dispatchNo +"]",
        //        buttons: {
        //            confirm: {
        //                label: 'Yes',
        //                className: 'btn-success',
        //                label: '<i class="fa fa-check"></i> Yes',
        //            },
        //            cancel: {
        //                label: 'No',
        //                className: 'btn-danger',
        //                label: '<i class="fa fa-times"></i> No',
        //            }
        //        },
        //        callback: function (result) {

        //            if (result == true) {
        //                displayPopupNotification("you interested", 'success');
        //                //var CITY_EDESC = $("#in_english").val();
        //                //var CITY_CODE = $("#short_cut").val();
        //                //if (CITY_CODE == undefined) {
        //                //    CITY_CODE = $scope.CitySetupObj.SHORT_CUT;
        //                //}
        //                //var deleteTypeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteCitySetup?cityCode=" + CITY_CODE;
        //                //$http({
        //                //    method: 'POST',
        //                //    url: deleteTypeUrl,
        //                //}).then(function successCallback(response) {
        //                //    if (response.data.MESSAGE == "DELETED") {
        //                //        $("#cGrid").data("kendoGrid").dataSource.read();
        //                //        $scope.Cancel();
        //                //        displayPopupNotification("Data succesfully deleted ", "success");
        //                //    }
        //                //    else {
        //                //        displayPopupNotification(response.data.STATUS_CODE, "error");
        //                //    }
        //                //}, function errorCallback(response) {
        //                //    displayPopupNotification(response.data.STATUS_CODE, "error");
        //                //});
        //            } else {
        //                displayPopupNotification("You canceled", 'success');
        //            }
        //            //$scope.Cancel();
        //        }

        //    });

        //}
        //alert("dispatchNo" + dispatchNo);
    }

    $scope.printLoadingSlip = function (divName) {

        var printContents = document.getElementById(divName).innerHTML;

        var popupWin = window.open('', '_blank', 'width=850,height=800', 'orientation = portrait');
        popupWin.document.open();
        popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
        popupWin.document.close();

    };

    $scope.cancelPrint = function () {
        debugger;
        window.location.reload(true);
    };

   

});