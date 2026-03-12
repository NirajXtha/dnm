DTModule.controller('LoadingSlipCtrl', function ($scope, $http, $window, $filter, $timeout) {

    $scope.lccode = "0";
    $scope.vehicleRegEdit = false;

    $scope.VehicleAndDispatchList = [];
    $scope.loadingSlipDetail = [];

    $scope.loadingVehicleObj = {
        TRANSACTION_NO:"",
        REFERENCE_NO:"" ,
        REFERENCE_FORM_CODE:"",
        VEHICLE_NAME:"",
        TRANSACTION_DATE: new Date(),
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
        CREATED_DATE: new Date(),
        OUT_TIME: "",
        SYN_ROWID: "",
        LOAD_IN_TIME: "",
        LOAD_OUT_TIME: "",
        MODIFY_DATE: new Date(),
        MODIFY_BY: "",
        TOTAL_VEHICLE_HR: "",
        TEAR_WT: "",
        GROSS_WT: "",
        NET_WT: "",
        QUANTITY: "",
        ACCESS_BY: "",
        ACCESS_DATE: new Date(),
        DESTINATION: "",
        BROKER_NAME: "",
        VEHICLE_IN_DATE: new Date(),
        VEHICLE_OUT_DATE: new Date(),
        TRANSPORT_NAME: "",
        WB_SLIP_NO: "",
        TRANSPORTER_CODE: "",
        COMPANY_NAME: "",
        COMPANY_ADDRESS: "",
        COMPANY_TEL_NO: "",
        LS_DATE: "",
        UNIT: ""
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
        TRANSPORTER_CODE: "",
        COMPANY_NAME: "",
        COMPANY_ADDRESS: "",
        COMPANY_TEL_NO: "",
        LS_DATE: "",
        UNIT: ""
    }

    $scope.showDispatchGrid = function (event) {

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
        $('#btnConfirmToTag').prop("disabled", false);
         var grid = $("#dGrid").data("kendoGrid"),
            selectedItem = grid.dataItem(grid.select());  

        //$scope.TaggedInformation == $scope.loadingVehicleObj;
        debugger
        $scope.TaggedInformation = selectedItem;
        $scope.TaggedInformation.REFERENCE_NO = selectedItem.ORDER_NO;
        $scope.TaggedInformation.REFERENCE_FORM_CODE = selectedItem.FORM_CODE;
        $scope.TaggedInformation.TRANSACTION_NO = $scope.loadingVehicleObj.TRANSACTION_NO;
        $scope.TaggedInformation.VEHICLE_NAME = $scope.loadingVehicleObj.VEHICLE_NAME;
        $scope.TaggedInformation.VEHICLE_OWNER_NAME = $scope.loadingVehicleObj.VEHICLE_OWNER_NAME;
        $scope.TaggedInformation.VEHICLE_OWNER_NO = $scope.loadingVehicleObj.VEHICLE_OWNER_NO;
        $scope.TaggedInformation.DRIVER_NAME = $scope.loadingVehicleObj.DRIVER_NAME;
        $scope.TaggedInformation.DRIVER_MOBILE_NO = $scope.loadingVehicleObj.DRIVER_MOBILE_NO;
        $scope.TaggedInformation.DRIVER_LICENCE_NO = $scope.loadingVehicleObj.DRIVER_LICENCE_NO;
        $scope.TaggedInformation.DESTINATION = selectedItem.TO_LOCATION;
        $scope.TaggedInformation.PARTY_TYPE_EDESC = selectedItem.PARTY_TYPE_EDESC;
        $scope.TaggedInformation.PARTY_TYPE_CODE = selectedItem.PARTY_TYPE_CODE;
        $scope.TaggedInformation.ADDRESS = $scope.TaggedInformation.ADDRESS;
        $scope.TaggedInformation.ITEM_EDESC = $scope.TaggedInformation.ITEM_EDESC;
        $scope.TaggedInformation.ITEM_CODE = $scope.TaggedInformation.ITEM_CODE;
        $scope.TaggedInformation.REMARKS = $scope.loadingVehicleObj.REMARKS;
        $scope.TaggedInformation.GEAR_WT = $scope.loadingVehicleObj.GEAR_WT;
        $scope.TaggedInformation.TEAR_WT = $scope.loadingVehicleObj.TEAR_WT;
        $scope.TaggedInformation.NET_WT = $scope.loadingVehicleObj.NET_WT;
        $scope.TaggedInformation.TOTAL_VEHICLE_HR = $scope.loadingVehicleObj.TOTAL_VEHICLE_HR;
        $scope.TaggedInformation.TRANSPORT_NAME = $scope.loadingVehicleObj.TRANSPORT_NAME;
        $scope.TaggedInformation.IN_TIME = $scope.loadingVehicleObj.IN_TIME;
        $scope.TaggedInformation.OUT_TIME = $scope.loadingVehicleObj.OUT_TIME;
        $scope.TaggedInformation.LOAD_IN_TIME = $scope.loadingVehicleObj.LOAD_IN_TIME;
        $scope.TaggedInformation.LOAD_OUT_TIME = $scope.loadingVehicleObj.LOAD_OUT_TIME;
        $scope.TaggedInformation.VEHICLE_IN_DATE = $scope.loadingVehicleObj.VEHICLE_IN_DATE;
        $scope.TaggedInformation.VEHICLE_OUT_DATE = $scope.loadingVehicleObj.VEHICLE_OUT_DATE;
        $scope.TaggedInformation.LS_DATE = $scope.entryDate;
        $scope.TaggedInformation.UNIT = $scope.loadingVehicleObj.UNIT;
        $scope.$apply();
        /*$("#lGrid").show();*/
        console.log("$scope.loadingVehicleObj================>>>" + JSON.stringify($scope.loadingVehicleObj));

    };

    $scope.DispatchFilter = "TODAY";

    $scope.FilterDispatch = function (e) {
        showloader();
        debugger
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
                pageSize: 10,
                serverSorting: true,
                serverFiltering: true,
                sort: {
                    field: "TRANSACTION_NO",
                    dir: "desc"
                }
            },
            selectable: "multiple",
            scrollable: true,
            height: 350,
            sortable: true,
            pageable: true,
            groupable: true,
            resizable: true,
            change: onDispatchGridChange,
            dataBound: function (e) {
                // debugger;
                $("#dGrid tbody tr").css("cursor", "pointer");

                //$("#dGrid tbody tr").on('dblclick', function () {
                //    var vehicle = $(this).find('td span').html()
                 

                //})
            },
            columns: [
                { field: "DISPATCH_NO", title: "Dispatch No", width: 40 },
                { field: "ORDER_NO", title: "Order No", width: 40 },
                { field: "TO_LOCATION", title: "Destination", width: 50 },
                { field: "CUSTOMER_EDESC", title: "Customer Name", width: 50 },
                { field: "QUANTITY", title: "Planning Qty", width: 30 },
                { field: "LS_PENDING_QTY", title: "LS Pending Qty.", width: 30 },
            ]

        };

        hideloader();
    };

    //$scope.FilterDispatch();
    $scope.vehicleCenterChildGridOptions = {
        dataSource: {
            type: "json",
            transport: {
                read: "/api/OrderDispatchApi/GetRegisteredVehicle",
                parameterMap: function (options, type) {
                    if (type === 'read') {
                        var englishDate = $("#englishdatedocument").val();

                        return {
                            ...options, 
                            from: "LoadingSlip",
                            entryDate: englishDate
                        };
                    }
                }
            },
            pageSize: 20,
            autoBind: false,
            sort: {
                field: "TRANSACTION_NO",
                dir: "desc"
            },
            serverSorting: true,
        },
        selectable: "single",
        scrollable: true,
        searchable:true,
        height: 300,
        sortable: true,
        pageable: true,
        groupable: true,
        autoBind: false,
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
                width: "130px",
                template: function (dataItem) {
                    if (dataItem.REFERENCE_NO === null || dataItem.REFERENCE_NO === "") {
                        return "No tag";
                    } else {
                        return dataItem.REFERENCE_NO;
                    }
                }
            },
            {
                field: "REMARKS",
                title: "Remarks",
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
        debugger
        if ($scope.TaggedInformation.PendingToPlanning == 0) {
            displayPopupNotification(`Please select proper Order No. to generate Loading Slip. Because Order No. ${$scope.TaggedInformation.ORDER_NO} has no LS Pending Quantity`, "error")
            return;
        }
        if ($scope.TaggedInformation.ORDER_NO) {
            $("#dispatchTagModal").modal('toggle');
            $scope.BindSlipGenerator($scope.TaggedInformation);
        }

        if (!$("#vehiclePanelBar").find(".row").hasClass("hidePanelContent")) {
            $("#vehiclePanelBar").find(".row").addClass("hidePanelContent");
        }

        var panelBar = $("#panelbar").kendoPanelBar({
            expandMode: "single",
            collapse: function (e) {
                $(e.item).find(".k-content").hide();
            },
            expand: function (e) {
                $(e.item).find(".k-content").show();
            }
        }).data("kendoPanelBar");

        panelBar.collapse($(".k-item", "#panelbar"));

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
        debugger
        dData = [];
        dData.push(data);
        console.log("dataForDispatch===========>> " + JSON.stringify(dData));
        $scope.loadingSlipPrintGridOptions = {
            dataSource: new kendo.data.DataSource({
                data: dData,
                sort: {
                    field: "TRANSACTION_NO",
                    dir: "desc"
                }
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
                    field: "MU_CODE",
                    title: "Unit",
                    width: "100px"
                },
                {
                    field: "QUANTITY",
                    title: "Quantity",
                    width: "80px"
                },
                {
                    field: "UNIT_PRICE",
                    title: "Rate",
                    width: "80px"
                },
                {
                    title: "Action ",
                    template: '<a style="padding: 5px 8px; border-radius: 8px; color: white; background-color: darkgreen;" class="fa fa-check-circle editAction" title="Generate Loading Slip" ng-click="loadingSlipPrint(#:DISPATCH_NO#)"> Generate Loading Slip<span class="sr-only"></span></a>',
                    width: "160px"
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

    $scope.loadingSlipPrint = function (dispatchNo) {
        // var isSlipSave = generateLoadingSlipPrint();
        debugger
        if($scope.loadingVehicleObj.LOAD_IN_TIME) {
            let loadInTime = moment($scope.loadingVehicleObj.LOAD_IN_TIME);
            let loadingSlipDate = moment($scope.entryDate);

            if (loadInTime < loadingSlipDate) {
                displayPopupNotification("Loading Slip Date is not < Vehicle in Date (Transaction Date)", "error")
                return
            }
        }

        var saveTypeUrl = window.location.protocol + "//" + window.location.host + "/api/OrderDispatchApi/GenerateLoadingSlip";
        $http({
            method: 'POST',
            url: saveTypeUrl,
            data: $scope.TaggedInformation
        }).then(function successCallback(response) {
          
            if (response.data.MESSAGE == "ERROR") {
                displayPopupNotification("Error in save.", "error");

            }
            else {
                debugger
                DisplayBarNotificationMessage("Loading Slip has been generated successfully");
                console.log("Print loadingSllip", $scope.TaggedInformation);
                $scope.lccode = response.data.MESSAGE;
                $scope.loadingSlipDetail = response.data.data;
                var vgrid = $("#vGrid").data("kendoGrid");
                if (vgrid) {
                    vgrid.dataSource.read();
                }
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
                        debugger
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
                var lgrid = $("#lGrid").data("kendoGrid");
                if (lgrid) {
                    lgrid.dataSource.read();
                }

                var panelBar = $("#panelbar").kendoPanelBar({
                    expandMode: "single",
                    collapse: function (e) {
                        $(e.item).find(".k-content").hide();
                    },
                    expand: function (e) {
                        $(e.item).find(".k-content").show();
                    }
                }).data("kendoPanelBar");

                panelBar.expand($("#panelbar > .k-item:first"));
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

    $scope.ConvertEngToNep = function () {
        console.log(this);

        var engdate = $("#englishDate5").val();
        var nepalidate = ConvertEngDateToNep(engdate);
        $("#nepaliDate5").val(nepalidate);
    };

    $scope.ConvertNepToEng = function ($event) {

        //$event.stopPropagation();
        console.log($(this));
        var date = BS2AD($("#nepaliDate5").val());
        var date1 = BS2AD($("#nepaliDate51").val());
        $("#englishdatedocument").val($filter('date')(date, "dd-MMM-yyyy"));
        $('#nepaliDate5').trigger('change')
    };

    $scope.ConvertEngToNepang = function (data) {
        $("#nepaliDate5").val(AD2BS(data));
    };

    $scope.someDateFn = function () {

        var engdate = $filter('date')(new Date(new Date().setDate(new Date().getDate() - 1)), 'dd-MMM-yyyy');
        //var engdate1 = $filter('date')(new Date(new Date().setDate(new Date().getDate() - 2)), 'dd-MMM-yyyy');
        var a = ConvertEngDateToNep(engdate);
        //var a1 = ConvertEngDateToNep(engdate1);
        $scope.Dispatch_From = engdate;
        $scope.NepaliDate = a;
        $scope.Dispatch_To = a;
        $scope.PlanningTo = ConvertEngDateToNep($filter('date')(new Date(new Date().setDate(new Date().getDate())), 'dd-MMM-yyyy'));
        //  $scope.PlanningDate = a;

    };

    $scope.someDateFn();
    $scope.monthSelectorOptionsSingle = {
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            let engDate = moment($scope.loadingVehicleObj.TRANSACTION_DATE, "DD-MMM-YYYY").format("YYYY-MM-DD")
            $("#nepaliDate5").val(AD2BS(engDate))
            setTimeout(function () {
                var grid = $("#vGrid").data("kendoGrid");
                if (grid) {
                    grid.dataSource.read();
                }
            }, 100)
        },
        format: "dd-MMM-yyyy",

        dateInput: true
    };
});