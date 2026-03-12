DTModule.controller('ThirdPartyPreferenceSetupCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    $scope.saveupdatebtn = "Save";
    $scope.showModalForNew = function (event) {
        $scope.saveupdatebtn = "Save"
        $scope.editFlag = "N";
        $scope.OSArr = [];
        $("#preffModal").modal("toggle");
    }
    //Grid
    $scope.PreffdGridOptions = {
        dataSource: {
            type: "json",
            transport: {
                read: "/api/OSApi/GetPreferenceSetup",
            },
            pageSize: 50,
            serverSorting: true
        },
        scrollable: true,
        height: 450,
        sortable: true,
        pageable: true,
        dataBound: function (e) {
            $("#kGrid tbody tr").css("cursor", "pointer");

            $("#kGrid tbody tr").on('dblclick', function () {
                var vehicle = $(this).find('td span').html()
                $scope.edit(vehicle);
            })
        },
        columns: [
             {
                 field: "OS_ID",
                 title: "OS Id",
                 width: "80px"
             },
            {
                field: "SFORM_CODE",
                title: " Sform Code",
                width: "80px"
            },
            {
                field: "STABLE_NAME",
                title: "Stable Name",
                width: "130px"
            },
            {
                field: "TYPE",
                title: "Type",
                width: "130px"
            },
            {
                field: "SITEM_ACC_CODE",
                title: "Sitem Acc",

                width: "130px"
            },

            {
                title: "Action ",
                template: '<a class="fa fa-pencil-square-o editAction" title="Edit" ng-click="edit(#:OS_ID#)"><span class="sr-only"></span> </a>',
                width: "60px"
            }
        ],

    };

    $scope.OSArr = {
        OS_ID: "",
        SFORM_CODE: "",
        STABLE_NAME: "",
        SCUS_ACC_CODE: "",
        SITEM_ACC_CODE: "",
        SMERGE_ITEM_FLAG: "",
        S_ISPARENTCUSTOMER_CREATED: "",
        S_ISPARENTITEM_CREATED: "",
        S_DISPLAY_FLAG: "",
        O_DISPLAY_FLAG: "",
        SVAT_CHARGE_ACC_CODE: "",
        SDISCOUNT_ACC_CODE: '',
        SSERCIVE_ACC_CODE: "",
        //
        OFORM_CODE: "",
        OTABLE_NAME: "",
        OCUS_ACC_CODE: "",
        OITEM_ACC_CODE: "",
        OMERGE_ITEM_FLAG: "",
        OVAT_CHARGE_ACC_CODE: "",
        ODISCOUNT_ACC_CODE: "",
        OSERCIVE_ACC_CODE: "",
        //
        IRD_URL: "",
        IRD_USER: "",
        IRD_PASSWORD: "",
        IRD_PAN_NO: "",
        ENABLE_IRD: "",
        ENABLE_REALTIME: "",
        ENABLE_SYN: ""
       
    };
    //account
    var accTypeUrl123 = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAccountCodeWithChild";
    $scope.custaccountOptions = {
        dataTextField: "ACC_EDESC",
        dataValueField: "ACC_CODE",
        maxSelectedItems: 1,
        valuePrimitive: true,
        autoClose: true,
        headerTemplate: '<div class="col-md-offset-3"><strong>Select Customer acc...</strong></div>',
        placeholder: "Select Customer acc...",

        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: accTypeUrl123,
                    dataType: "json"
                }
            }
        }
    };


    //item
    var accTypeUr = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAccountCodeWithChild";
    $scope.itemaccountOptions = {
        dataTextField: "ACC_EDESC",
        dataValueField: "ACC_CODE",
        maxSelectedItems: 1,
        valuePrimitive: true,
        autoClose: true,
        headerTemplate: '<div class="col-md-offset-3"><strong>Select item acc...</strong></div>',
        placeholder: "Select Item acc...",

        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: accTypeUr,
                    dataType: "json"
                }
            }
        }
    };

    //Symphiny from code
    var fromcode = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAllfrcd";
    $scope.SformCodeOptions = {
        dataTextField: "FORM_CODE",
        dataValueField: "FORM_CODE",
        maxSelectedItems: 1,
        valuePrimitive: true,
        autoClose: true,
        headerTemplate: '<div class="col-md-offset-3"><strong>Select form code...</strong></div>',
        placeholder: "Select form code...",
        select: function (e) {
            debugger;
            if (e.dataItem !== undefined) {
                $scope.OSArr.STABLE_NAME = e.dataItem.REF_TABLE_NAME;
            }
        },
        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: fromcode,
                    dataType: "json"
                }
            }
        }
    };

    //Opera form code
    var fromcode = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAllfrcd";
    $scope.OformCodeOptions = {
        dataTextField: "FORM_CODE",
        dataValueField: "FORM_CODE",
        maxSelectedItems: 1,
        valuePrimitive: true,
        autoClose: true,
        headerTemplate: '<div class="col-md-offset-3"><strong>Select form code...</strong></div>',
        placeholder: "Select form code...",
        select: function (e) {
            debugger;
            if (e.dataItem !== undefined) {
                $scope.OSArr.OTABLE_NAME = e.dataItem.REF_TABLE_NAME;
            }
        },
        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: fromcode,
                    dataType: "json"
                }
            }
        }
    };


    //Save 
    $scope.saveNewOS = function () {
        if ($scope.saveupdatebtn == "Save") {
            var model = {
                OS_ID: $scope.OSArr.OS_ID,
                SFORM_CODE: $scope.OSArr.SFORM_CODE,
                STABLE_NAME: $scope.OSArr.STABLE_NAME,
                SCUS_ACC_CODE: $scope.OSArr.SCUS_ACC_CODE,
                SITEM_ACC_CODE: $scope.OSArr.SITEM_ACC_CODE,
                SMERGE_ITEM_FLAG: $scope.OSArr.SMERGE_ITEM_FLAG,
                S_ISPARENTCUSTOMER_CREATED: $scope.OSArr.S_ISPARENTCUSTOMER_CREATED,
                S_ISPARENTITEM_CREATED: $scope.OSArr.S_ISPARENTITEM_CREATED,
                S_DISPLAY_FLAG: $scope.OSArr.S_DISPLAY_FLAG,
                O_DISPLAY_FLAG: $scope.OSArr.O_DISPLAY_FLAG,
                SVAT_CHARGE_ACC_CODE: $scope.OSArr.SVAT_CHARGE_ACC_CODE,
                SDISCOUNT_ACC_CODE: $scope.OSArr.SDISCOUNT_ACC_CODE,
                SSERCIVE_ACC_CODE: $scope.OSArr.SSERCIVE_ACC_CODE,
                //OPERA
                OFORM_CODE: $scope.OSArr.OFORM_CODE,
                OTABLE_NAME: $scope.OSArr.OTABLE_NAME,
                OCUS_ACC_CODE: $scope.OSArr.OCUS_ACC_CODE,
                OITEM_ACC_CODE: $scope.OSArr.SITEM_ACC_CODE,
                OMERGE_ITEM_FLAG: $scope.OSArr.OMERGE_ITEM_FLAG,
                OVAT_CHARGE_ACC_CODE: $scope.OSArr.OVAT_CHARGE_ACC_CODE,
                ODISCOUNT_ACC_CODE: $scope.OSArr.ODISCOUNT_ACC_CODE,
                OSERCIVE_ACC_CODE: $scope.OSArr.OSERCIVE_ACC_CODE,
                //
                IRD_URL: $scope.OSArr.IRD_URL,
                IRD_USER: $scope.OSArr.IRD_USER,
                IRD_PASSWORD: $scope.OSArr.IRD_PASSWORD,
                IRD_PAN_NO: $scope.OSArr.IRD_PAN_NO,
                ENABLE_IRD: $scope.OSArr.ENABLE_IRD,
                ENABLE_REALTIME: $scope.OSArr.ENABLE_REALTIME,
                ENABLE_SYN: $scope.OSArr.ENABLE_SYN,
           


            }
            var saveOSUrl = window.location.protocol + "//" + window.location.host + "/api/OSApi/SavePreferenceSetupOS";
            $http({
                method: 'POST',
                url: saveOSUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "INSERTED") {
                    $("#kGrid").data("kendoGrid").dataSource.read();
                    displayPopupNotification("Data succesfully saved ", "success");
                    $("#preffModal").modal("toggle");
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Third Party name already exist please try another Third Party name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            }, function errorCallback(response) {
                 if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Third Party name already exist please try another Third Party name.", "error");
                }
                else {
                displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            });
        }
        else if ($scope.saveupdatebtn == "Update") {
            var udpateVehicleUrl = window.location.protocol + "//" + window.location.host + "/api/OSApi/UpdatePreferenceSetupOS";
            var model = {
                OS_ID: $scope.OSArr.OS_ID,
                SFORM_CODE: $scope.OSArr.SFORM_CODE,
                STABLE_NAME: $scope.OSArr.STABLE_NAME,
                SCUS_ACC_CODE: $scope.OSArr.SCUS_ACC_CODE,
                SITEM_ACC_CODE: $scope.OSArr.SITEM_ACC_CODE,
                SMERGE_ITEM_FLAG: $scope.OSArr.SMERGE_ITEM_FLAG,
                S_ISPARENTCUSTOMER_CREATED: $scope.OSArr.S_ISPARENTCUSTOMER_CREATED,
                S_ISPARENTITEM_CREATED: $scope.OSArr.S_ISPARENTITEM_CREATED,
                S_DISPLAY_FLAG: $scope.OSArr.S_DISPLAY_FLAG,
                O_DISPLAY_FLAG: $scope.OSArr.O_DISPLAY_FLAG,
                SVAT_CHARGE_ACC_CODE: $scope.OSArr.SVAT_CHARGE_ACC_CODE,
                SDISCOUNT_ACC_CODE: $scope.OSArr.SDISCOUNT_ACC_CODE,
                SSERCIVE_ACC_CODE: $scope.OSArr.SSERCIVE_ACC_CODE,
                SPARTY_TYPE_CODE: $scope.OSArr.SPARTY_TYPE_CODE,
                //OPERA
                OFORM_CODE: $scope.OSArr.OFORM_CODE,
                OTABLE_NAME: $scope.OSArr.OTABLE_NAME,
                OCUS_ACC_CODE: $scope.OSArr.OCUS_ACC_CODE,
                OITEM_ACC_CODE: $scope.OSArr.SITEM_ACC_CODE,
                OMERGE_ITEM_FLAG: $scope.OSArr.OMERGE_ITEM_FLAG,
                OVAT_CHARGE_ACC_CODE: $scope.OSArr.OVAT_CHARGE_ACC_CODE,
                ODISCOUNT_ACC_CODE: $scope.OSArr.ODISCOUNT_ACC_CODE,
                OSERCIVE_ACC_CODE: $scope.OSArr.OSERCIVE_ACC_CODE,
                OPARTY_TYPE_CODE: $scope.OSArr.OPARTY_TYPE_CODE,
                //
                IRD_URL: $scope.OSArr.IRD_URL,
                IRD_USER: $scope.OSArr.IRD_USER,
                IRD_PASSWORD: $scope.OSArr.IRD_PASSWORD,
                IRD_PAN_NO: $scope.OSArr.IRD_PAN_NO,
                ENABLE_IRD: $scope.OSArr.ENABLE_IRD,
                ENABLE_REALTIME: $scope.OSArr.ENABLE_REALTIME,
                ENABLE_SYN: $scope.OSArr.ENABLE_SYN,
            }
            $http({
                method: 'POST',
                url: udpateVehicleUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "UPDATED") {
                    $("#kGrid").data("kendoGrid").dataSource.read();

                    displayPopupNotification("Data succesfully updated ", "success");
                    $("#preffModal").modal("toggle");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            }, function errorCallback(response) {
                displayPopupNotification("Something went wrong.Please try again later.", "error");

            });
        }
    }


    $scope.fillCompanySetupForms = function (prefId) {
        var getResourcedetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/OSApi/getPreffitem?prefId=" + prefId;
        $http({
            method: 'GET',
            url: getResourcedetaisByUrl,

        }).then(function successCallback(response) {
            $scope.OSArr = response.data.DATA;
            $scope.OSArr.OS_ID = $scope.OSArr.OS_ID;
            if ($scope.OSArr.OMERGE_ITEM_FLAG == "Y") {
                $scope.OSArr.OMERGE_ITEM_FLAG = true;
            }
            else {
                $scope.OSArr.OMERGE_ITEM_FLAG = false;
            }
            if ($scope.OSArr.SMERGE_ITEM_FLAG == "Y") {
                $scope.OSArr.SMERGE_ITEM_FLAG = true;
            }
            else {
                $scope.OSArr.SMERGE_ITEM_FLAG = false;
            }
            if ($scope.OSArr.S_DISPLAY_FLAG == "Y") {
                $scope.OSArr.S_DISPLAY_FLAG = true;
            }
            else {
                $scope.OSArr.S_DISPLAY_FLAG = false;
            }
            if ($scope.OSArr.O_DISPLAY_FLAG == "Y") {
                $scope.OSArr.O_DISPLAY_FLAG = true;
            }
            else {
                $scope.OSArr.O_DISPLAY_FLAG = false;
            }
            
            //
            if ($scope.OSArr.ENABLE_IRD == "Y") {
                $scope.OSArr.ENABLE_IRD = true;
            }
            else {
                $scope.OSArr.ENABLE_IRD = false;
            }
            if ($scope.OSArr.ENABLE_REALTIME == "Y") {
                $scope.OSArr.ENABLE_REALTIME = true;
            }
            else {
                $scope.OSArr.ENABLE_REALTIME = false;
            }
            if ($scope.OSArr.ENABLE_SYN == "Y") {
                $scope.OSArr.ENABLE_SYN = true;
            }
            else {
                $scope.OSArr.ENABLE_SYN = false;
            }

            companydataFillDefered.resolve(response);


        }, function errorCallback(response) {

        });
    }

    $scope.edit = function (prefId) {
        $scope.editFlag = "Y";
        $scope.saveupdatebtn = "Update";
        companydataFillDefered = $.Deferred();
        $scope.fillCompanySetupForms(prefId);
        $.when(companydataFillDefered).done(function () {
            $("#preffModal").modal("toggle");
        });


    }

    ////for customer
    $scope.customersArr =
      {
          TRANSACTION_TYPE: "",
          PREFIX_TEXT: "",
          CUSTOMER_CODE: "",
          CUSTOMER_EDESC: "",
          CUSTOMER_NDESC: "",
          CUSTOMER_ACCOUNT: "",
          CUSTOMER_TYPE: "",
          MASTER_CUSTOMER_CODE: "",
          PRE_CUSTOMER_CODE: "",
          COMPANY_CODE: "",
          CREATED_BY: "",
          CREATED_DATE: "",
          DELETED_FLAG: "",
          SYN_ROWID: "",
          CUSTOMER_PREFIX: "",
          CUSTOMER_STARTID: "",
          REMARKS: "",
          TEL_MOBILE_NO1: "",
          CREDIT_ACTION_FLAG: "",
          APPROVED_FLAG: "",
          CUSTOMER_ID: "",
          REGD_OFFICE_EADDRESS: "",
          ACC_CODE: "",

      }

    $scope.saveNewCustomer = function () {
        var createUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewCustomerGroup1";
        var model = {
            CUSTOMER_CODE: $scope.customersArr.CUSTOMER_CODE,
            CUSTOMER_EDESC: $scope.customersArr.CUSTOMER_EDESC,
            CUSTOMER_NDESC: $scope.customersArr.CUSTOMER_NDESC,
            MASTER_CUSTOMER_CODE: $scope.customersArr.MASTER_CUSTOMER_CODE,
            PRE_CUSTOMER_CODE: $scope.customersArr.PRE_CUSTOMER_CODE,
            CUSTOMER_ACCOUNT: $scope.customersArr.CUSTOMER_ACCOUNT,
            CUSTOMER_TYPE: $scope.customersArr.CUSTOMER_TYPE,
            CUSTOMER_PREFIX: $scope.customersArr.CUSTOMER_PREFIX,
            CUSTOMER_STARTID: $scope.customersArr.CUSTOMER_STARTID,
            REMARKS: $scope.customersArr.REMARKS,
            GROUP_SKU_FLAG: "G",
            TEL_MOBILE_NO1: $scope.customersArr.TEL_MOBILE_NO1,
            CREDIT_ACTION_FLAG: $scope.customersArr.CREDIT_ACTION_FLAG,
            APPROVED_FLAG: $scope.customersArr.APPROVED_FLAG,
            CUSTOMERCUSTOMER_ID_STARTID: $scope.customersArr.CUSTOMER_ID,
            REGD_OFFICE_EADDRESS: $scope.customersArr.REGD_OFFICE_EADDRESS,
            ACC_CODE: $scope.customersArr.ACC_CODE
        }
        $http({
            method: 'POST',
            url: createUrl,
            data: model
        }).then(function successCallback(response) {

            if (response.data.MESSAGE == "INSERTED") {
                displayPopupNotification("Customer Created Successfully ", "success");
                $scope.OSArr.S_ISPARENTCUSTOMER_CREATED = "Y";
            }
            else {
                displayPopupNotification("Something went wrong.Please try again later.", "error");
                $scope.OSArr.S_ISPARENTCUSTOMER_CREATED = "N";
            }

        }, function errorCallback(response) {
            displayPopupNotification("Something went wrong.Please try again later.", "error");
            $scope.OSArr.S_ISPARENTCUSTOMER_CREATED = "N";

        });



    }


    //For item
    $scope.itemsetup =
       {
           AVG_RATE: "",
           BATCH_FLAG: "N",
           BATCH_SERIAL_FLAG: "N",
           BRANCH_CODE: "",
           CATEGORY_CODE: "",
           COMPANY_CODE: "",
           COSTING_METHOD_FLAG: "",
           CREATED_BY: "",
           CREATED_DATE: "",
           CURRENT_STOCK: "",
           DANGER_LEVEL: "",
           DEFAULT_WIP_STOCK: "",
           DELETED_FLAG: "",
           DELTA_FLAG: "",
           DIMENSION: "",
           ECO_ORDER_QUANTITY: "",
           FRACTION_VALUE: "",
           GROUP_SKU_FLAG: "",
           HS_CODE: "",
           IMAGE_FILE_NAME: "",
           INDEX_MU_CODE: "",
           ITEM_CODE: "",
           ITEM_EDESC: "",
           ITEM_NDESC: "",
           LEAD_TIME: "",
           LINK_SUB_CODE: "",
           MASTER_ITEM_CODE: "",
           MAX_LEVEL: "",
           MAX_USAGE: "",
           MAX_VALUE: "",
           MIN_LEVEL: "",
           MIN_USAGE: "",
           MIN_VALUE: "",
           MODIFY_BY: "",
           MODIFY_DATE: "",
           MULTI_MU_CODE: "",
           NORMAL_USAGE: "",
           PREFERRED_LEVEL: "",
           PREFERRED_SUPPLIER_CODE: "",
           PRE_ITEM_CODE: "",
           PRODUCT_CODE: "",
           PURCHASE_PRICE: "",
           REEM_WEIGHT_KG: "",
           REMARKS: "",
           REMARKS2ND: "",
           REORDER_LEVEL: "",
           SALES_PRICE: "",
           SERIAL_FLAG: "N",
           SERIAL_PREFIX_LENGTH: "",
           SERIAL_PREFIX_TEXT: "",
           SERVICE_ITEM_FLAG: "N",
           SHELF_LIFE_DAYS: "",
           SYN_ROWID: "",
           VALUATION_FLAG: "",

       }

    $scope.itemArr = $scope.itemsetup;


    $scope.saveNewitem = function () {
        var createUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewitem1";
        var model = { model: $scope.itemArr };
        $http({
            method: 'POST',
            url: createUrl,
            data: model
        }).then(function successCallback(response) {
            if (response.data.MESSAGE == "INSERTED") {
                $scope.OSArr.S_ISPARENTITEM_CREATED = "Y";
                displayPopupNotification("Item Create Successfully ", "success");
            }
            else {

                displayPopupNotification("Something went wrong.Please try again later.", "error");
                $scope.OSArr.S_ISPARENTITEM_CREATED = "N";
            }
        }, function errorCallback(response) {
            $scope.OSArr.S_ISPARENTITEM_CREATED = "N";
            displayPopupNotification("Something went wrong.Please try again later.", "error");
        });


    }
});



