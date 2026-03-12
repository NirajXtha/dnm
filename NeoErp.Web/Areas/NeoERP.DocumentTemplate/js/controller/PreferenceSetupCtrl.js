DTModule.controller('PreferenceSetupCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    $scope.saveupdatebtn = "Save";
    $scope.pageName = " Preference Setup";
    $scope.saveupdatebtn = "Save";
    $scope.savegroup = false;
    $scope.editFlag = "N";
    $scope.treenodeselected = "N";
    $scope.treeSelectedDivisionCode = "";
    var companydataFillDefered = $.Deferred();
    $scope.$on('$routeChangeSuccess', function () {
        $scope.fetchEmployee();

        $("#updatedocumentformdata").attr('disabled', 'disabled');
    });
    $scope.showModalForNew = function (event) {

        debugger;
        $scope.saveupdatebtn = "Save"
        $scope.editFlag = "N";
        $scope.clearFields();
        $scope.Removedisableform();
        $("#CompanyModal").modal("toggle");
        //$("#updatedocumentformdata").attr('disabled', 'disabled');
    }

    $scope.companyCenterChildGridOptions = {
        dataSource: {
            type: "json",
            transport: {
                read: "/api/SetupApi/GetPrrffGridData",
            },
            pageSize: 50,
            serverSorting: true
        },
        scrollable: true,
        height: 470,
        sortable: true,
        pageable: true,
        reorderable: true,
        groupable: true,
        resizable: true,
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
            checkboxItem = $(e.container).find('input[type="checkbox"]');
        },
        columnShow: function (e) {
            if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                SaveReportSetting('RouteSetup', 'grid');
        },
        columnHide: function (e) {
            if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                SaveReportSetting('RouteSetup', 'grid');
        },
        dataBound: function (e) {
            debugger;
            $("#kGrid tbody tr").css("cursor", "pointer");
            //DisplayNoResultsFound($('#kGrid'));
            $("#kGrid tbody tr").on('dblclick', function () {
                var resourceCode = $(this).find('td span').html()
                $scope.edit(resourceCode);
                //var tree = $("#divisionRoottree").data("kendoTreeView");
                //tree.dataSource.read();
            })
        },
        columns: [
            {
                field: "COMPANY_EDESC",
                title: "company Name",
                width: "120px",
            },
            {
                field: "BRANCH_EDESC",
                title: "Branch Name",
                width: "120px",

            },
            {
                field: "FY_START_DATE",
                title: "Start Date",
                template: "#= kendo.toString(kendo.parseDate(FY_START_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
                width: "120px",
            },
            {
                field: "FY_END_DATE",
                title: "End Date",
                template: "#= kendo.toString(kendo.parseDate(FY_END_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
                width: "120px",
            },


            {
                title: "Action ",
                template: '<a class="fa fa-pencil-square-o editAction" title="Edit" ng-click="edit(#:COMPANY_CODE#)"><span class="sr-only"></span> </a><a class="fa fa-trash deleteAction" title="Delete" ng-click="delete(#:BRANCH_CODE#)"><span class="sr-only"></span> </a>',
                width: "60px"
            }
        ],


    };


    $scope.fillCompanySetupForms = function (cmpanyId) {
        var getResourcedetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getpreferenceDetailsByCompanyCode?cmpanyId=" + cmpanyId;
        $http({
            method: 'GET',
            url: getResourcedetaisByUrl,
        }).then(function successCallback(response) {
            $scope.preferenceSetup = response.data.DATA;
            $scope.preffArr = response.data.DATA;
            $scope.preffArr.COMPANY_CODE = $scope.preferenceSetup.COMPANY_CODE
            $scope.preffArr.BRANCH_CODE = $scope.preferenceSetup.BRANCH_CODE

            if ($scope.preferenceSetup.EXCHANGE_RATE == "1") {
                $scope.preffArr.EXCHANGE_RATE = "NRS";
            }
            else if ($scope.preferenceSetup.EXCHANGE_RATE == "1.6") {
                $scope.preffArr.EXCHANGE_RATE = "IRS";
            }
            else if ($scope.preferenceSetup.EXCHANGE_RATE == "107") {
                $scope.preffArr.EXCHANGE_RATE = "USD";
            }

            if ($scope.preffArr.FY_START_DATE) {
                let engDate = moment($scope.preffArr.FY_START_DATE).format("YYYY-MM-DD")
                $("#nepaliDate5").val(AD2BS(engDate))
            }

            if ($scope.preffArr.FY_END_DATE) {
                let engDate = moment($scope.preffArr.FY_END_DATE).format("YYYY-MM-DD");
                $("#nepaliDate6").val(AD2BS(engDate))
            }

            if ($scope.preffArr.FREEZE_ENTRY_DATE) {
                let engDate = moment($scope.preffArr.FREEZE_ENTRY_DATE).format("YYYY-MM-DD");
                $("#nepaliDate7").val(AD2BS(engDate))
            }

            $scope.preffArr.FREEZE_FLAG = "Y";

            companydataFillDefered.resolve(response);

        }, function errorCallback(response) {

        });
    }
    //Binding while editing
    $scope.edit = function (cmpanyId) {
        $scope.editFlag = "Y";
        $scope.savegroup = false;
        $scope.saveupdatebtn = "Update";
        $("#updatedocumentformdata").removeAttr('disabled');
        companydataFillDefered = $.Deferred();
        $scope.fillCompanySetupForms(cmpanyId);
        $.when(companydataFillDefered).done(function () {
            $scope.groupCompanyTypeFlag = "N";

            $("#CompanyModal").modal();


        });
        $scope.disableform();


    }

    //Delete function for child
    $scope.delete = function (code) {
        // alert(code);

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

                    //$scope.divisionsetup.GROUP_SKU_FLAG = "I";

                    var delUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeletePreference?companyCode=" + code;
                    $http({
                        method: 'POST',
                        url: delUrl
                    }).then(function successCallback(response) {



                        if (response.data.MESSAGE == "DELETED") {
                            debugger;



                            var grid = $('#kGrid').data("kendoGrid");
                            grid.dataSource.read();
                            bootbox.hideAll();

                            displayPopupNotification("Data succesfully deleted ", "success");
                        }

                    }, function errorCallback(response) {
                        displayPopupNotification(response.data.STATUS_CODE, "error");

                    });

                }
                else if (result == false) {
                    bootbox.hideAll();
                }

            }
        });
    }



    //var enp = [];
    //$scope.init = function () {

    //    debugger;
    //    $http({
    //        method: 'GET',
    //        url: window.location.protocol + "//" + window.location.host + "/api/setupapi/GetPrefffromload",

    //    }).then(function successCallback(response) {
    //        debugger;
    //        $scope.items = response.data;

    //        var emp = angular.copy($scope.items[0]);
    //         $scope.preffArr.COMPANY_CODE = emp.COMPANY_CODE;
    //         $scope.preffArr.BRANCH_CODE = emp.BRANCH_CODE;
    //         $scope.preffArr.DEFAULT_CURRENCY_CODE = emp.DEFAULT_CURRENCY_CODE;
    //         $scope.preffArr.FY_START_DATE = emp.FY_START_DATE.replace('T00:00:00','');
    //         $scope.preffArr.FY_END_DATE = emp.FY_END_DATE.replace('T00:00:00','');
    //         $scope.preffArr.EXCHANGE_RATE = emp.EXCHANGE_RATE;
    //         $scope.preffArr.CASH_ACC_CODE = emp.CASH_ACC_CODE;
    //         $scope.preffArr.METHOD = emp.METHOD;
    //         $scope.preffArr.PRINT_TYPE = emp.PRINT_TYPE;
    //         $scope.preffArr.FREIGHT_NEGATIVE_FLAG = emp.FREIGHT_NEGATIVE_FLAG;
    //         $scope.preffArr.LEDGER_VAT_FLAG = emp.LEDGER_VAT_FLAG;
    //         //$scope.preffArr.COST_CATEGORY_FLAG.emp.COST_CATEGORY_FLAG; 
    //         $scope.preffArr.DOCUMENT_HISTORY_FLAG = emp.DOCUMENT_HISTORY_FLAG;
    //         $scope.preffArr.DIVISION_FLAG = emp.DIVISION_FLAG;
    //         $scope.preffArr.ORDER_TAXABLE_RATE = emp.ORDER_TAXABLE_RATE;
    //         $scope.preffArr.EXCHANGE_RATE_EDITABLE_FLAG = emp.EXCHANGE_RATE_EDITABLE_FLAG;
    //         $scope.preffArr.VNO_AS_SESSION_ID_CONTROL = emp.VNO_AS_SESSION_ID_CONTROL;
    //         $scope.preffArr.DEALER_SYSTEM_FLAG = emp.DEALER_SYSTEM_FLAG;
    //         $scope.preffArr.PAN_VAT_CONTROL_FLAG = emp.PAN_VAT_CONTROL_FLAG;
    //         $scope.preffArr.NEGATIVE_STOCK_FLAG = emp.NEGATIVE_STOCK_FLAG;
    //         $scope.preffArr.SUB_NARRATION_FLAG = emp.SUB_NARRATION_FLAG;
    //         $scope.preffArr.REFERENCE_EDIT_FLAG = emp.REFERENCE_EDIT_FLAG;
    //         $scope.preffArr.CUSTOMER_APPROVAL_FLAG = emp.CUSTOMER_APPROVAL_FLAG;
    //         $scope.preffArr.MASTER_MODIFY = emp.MASTER_MODIFY;
    //         $scope.preffArr.SUPPLIER_APPROVAL_FLAG = emp.SUPPLIER_APPROVAL_FLAG;
    //         $scope.preffArr.CREDIT_CONTROL_FLAG = emp.CREDIT_CONTROL_FLAG;
    //         $scope.preffArr.CREDIT_CONTROL_AS = emp.CREDIT_CONTROL_AS;
    //         $scope.preffArr.CREDIT_CONTROL_AT = emp.CREDIT_CONTROL_AT;
    //         $scope.preffArr.FREQUENT_NO = emp.FREQUENT_NO;
    //         $scope.preffArr.ALERT_REFRESH_RATE = emp.ALERT_REFRESH_RATE;
    //         $scope.preffArr.SECURED_BACK_DAYS = emp.SECURED_BACK_DAYS;
    //         $scope.preffArr.DECIMAL_PLACE = emp.DECIMAL_PLACE;
    //         $scope.preffArr.PASS_EXPIRY_DAYS = emp.PASS_EXPIRY_DAYS
    //         $scope.preffArr.FISCAL_YEAR = emp.FISCAL_YEAR;



    //        $scope.preferenceSetup = angular.copy(emp);
    //    }, function errorCallback(response) {

    //        alert("Error. Try Again!");

    //    });

    //    $scope.disableform();
    //};



    //Company multiselect
    $scope.distCompanySelectOptions = {
        close: function (e) {
            $scope.preferenceSetup.BRANCH_CODE = null;
            var selected = $scope.preferenceSetup.COMPANY_CODE;
            if (typeof (selected) != 'undefined')
                $scope.fetchEmployee(selected.join("','"));
        },
        dataTextField: "COMPANY_EDESC",
        dataValueField: "COMPANY_CODE",
        maxSelectedItems: 1,
        valuePrimitive: true,
        autoClose: true,
        headerTemplate: '<div class="col-md-offset-3"><strong>Select Company...</strong></div>',
        placeholder: "Select Company...",

        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/setupapi/GetCompanyPreff",
                    dataType: "json"
                }
            }
        }
    };

    //Branch multiselect
    $scope.fetchEmployee = function (typeId) {
        var url = '';
        if (typeof (typeId) == 'undefined')
            url = window.location.protocol + "//" + window.location.host + "/api/setupapi/GetBranchPreff";
        else
            url = window.location.protocol + "//" + window.location.host + "/api/setupapi/GetBranchPreff?COMPANY_CODE=" + typeId
        var dataSource = $("#distBranchSelect").data("kendoMultiSelect");
        if (typeof (dataSource) != 'undefined' && dataSource != null) {
            $("#distBranchSelect").data("kendoMultiSelect").dataSource.options.transport.read.url = url;
            $("#distBranchSelect").data("kendoMultiSelect").dataSource.read();
            return;
        }

        $scope.distBranchSelectOptions = {
            dataTextField: "BRANCH_EDESC",
            dataValueField: "BRANCH_CODE",
            maxSelectedItems: 1,
            valuePrimitive: true,
            autoClose: true,
            headerTemplate: '<div class="col-md-offset-3"><strong>Select Branch Name...</strong></div>',
            placeholder: "Select Branch Name...",

            dataBound: function (e) {
                $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
            },
            dataSource: {
                transport: {
                    read: {
                        url: url,
                        dataType: "json"
                    }
                }
            }
        };
    }



    $scope.disableform = function () {
        $("#savedocumentformdata").attr('disabled', 'disabled');
        $("#distCompanySelect").attr('disabled', 'disabled');
        $("#distBranchSelect").attr('disabled', 'disabled');
        $("#currencyMultiSelect").attr('disabled', 'disabled');
        $("#exchangerate").attr('disabled', 'disabled');
        $("#frequently").attr('disabled', 'disabled');
        $("#alterrate").attr('disabled', 'disabled');
        $("#freezabackdays").attr('disabled', 'disabled');
        $("#decimal").attr('disabled', 'disabled');

        $("#passwordexp").attr('disabled', 'disabled');
        $("#englishdatedocument5").attr('disabled', 'disabled');
        $("#englishdatedocument6").attr('disabled', 'disabled');
        $("#nepaliDate5").attr('disabled', 'disabled');
        $("#nepaliDate6").attr('disabled', 'disabled');

        $("#accountMultiSelect").attr('disabled', 'disabled');
        $("#costingmethod").attr('disabled', 'disabled');
        $("#fysicalyear").attr('disabled', 'disabled');

        $("#printType").attr('disabled', 'disabled');
        $("#grniaccountMultiSelect").attr('disabled', 'disabled');
        $("#negativectrl").attr('disabled', 'disabled');
        $("#ledgervat").attr('disabled', 'disabled');
        $("#costcategoryflg").attr('disabled', 'disabled');
        $("#documenthistry").attr('disabled', 'disabled');
        $("#divigionflg").attr('disabled', 'disabled');
        $("#ordertx").attr('disabled', 'disabled');



        //Preference Control
        $("#billwiseadjust").attr('disabled', 'disabled');
        $("#preeditflg").attr('disabled', 'disabled');
        $("#custaprflg").attr('disabled', 'disabled');
        $("#supplierflg").attr('disabled', 'disabled');
        $("#mastermodifyflg").attr('disabled', 'disabled');
        $("#Tbgeneric").attr('disabled', 'disabled');
        $("#versionControlFlag").attr('disabled', 'disabled');
        $("#purchase_inv_form_code").attr('disabled', 'disabled');
        $("#PUR_RETURN_FORM_CODE").attr('disabled', 'disabled');
        $("#PUR_GRN_FORM_CODE").attr('disabled', 'disabled');
        $("#purchase_exp_form_code").attr('disabled', 'disabled');
        $("#sales_inv_form_code").attr('disabled', 'disabled');
        $("#SALES_RETURN_FORM_CODE").attr('disabled', 'disabled');
        $("#gl_reconciliation_form_code").attr('disabled', 'disabled');
        $("#INV_OTHERS_FORM_CODE").attr('disabled', 'disabled');
        $("#CONS_OTHERS_FORM_CODE").attr('disabled', 'disabled');
        $("#asset_depriciation_form_code").attr('disabled', 'disabled');
        $("#WB_AUTO_GRN_FORM_CODE").attr('disabled', 'disabled')
        $("#PUR_RATE_PERCENT").attr('disabled', 'disabled');
        $("#CREDIT_CONTROL_AS").attr('disabled', 'disabled');
        $("#CREDIT_CONTROL_AT").attr('disabled', 'disabled');
        $("#CREDIT_CONTROL_PDC_BG").attr('disabled', 'disabled');
        $("#CREDIT_CONTROL_DAYS").attr('disabled', 'disabled');
        $("#FREIGHT_FORM_CODE").attr('disabled', 'disabled');
        $("#linkDatabase").attr('disabled', 'disabled');
        $("#englishdatedocument7").attr('disabled', 'disabled');
        $("#nepaliDate7").attr('disabled', 'disabled');
        $("#remarks").attr('disabled', 'disabled');
        $("#freezeFlag").attr('disabled', 'disabled');
        $("#bwsmf").attr('disabled', 'disabled');
        $("#sublbr").attr('disabled', 'disabled');
        $("#branchsubledger").attr('disabled', 'disabled');
        $("#bwbmf").attr('disabled', 'disabled');

        $("#pslg").attr('disabled', 'disabled');
        $("#gid").attr('disabled', 'disabled');

        $("#cogsvlaue").attr('disabled', 'disabled');
        $("#negativestock").attr('disabled', 'disabled');
        $("#itemlast3rate").attr('disabled', 'disabled');

        $("#freezecost").attr('disabled', 'disabled');
        $("#subledgernarration").attr('disabled', 'disabled');
        //Master
        $("#generalledger").attr('disabled', 'disabled');

        $("#stockledger").attr('disabled', 'disabled');
        $("#ledger").attr('disabled', 'disabled');
        $("#customer").attr('disabled', 'disabled');
        $("#supplier").attr('disabled', 'disabled');
        $("#item").attr('disabled', 'disabled');
        //ocntrol
        $("#fnc").attr('disabled', 'disabled');
        $("#frc").attr('disabled', 'disabled');
        $("#ccc").attr('disabled', 'disabled');


        $("#cdcc").attr('disabled', 'disabled');
        $("#dcc").attr('disabled', 'disabled');
        $("#none").attr('disabled', 'disabled');
        $("#pdc").attr('disabled', 'disabled');
        $("#csr").attr('disabled', 'disabled');
        $("#dptr").attr('disabled', 'disabled');
        $("#dar").attr('disabled', 'disabled');
        $("#urd").attr('disabled', 'disabled');

        $("#budgetctrl").attr('disabled', 'disabled');
        $("#budgetinfo").attr('disabled', 'disabled');
        $("#ledgerbgtctrl").attr('disabled', 'disabled');
        $("#ledgerbgtinfo").attr('disabled', 'disabled');
        $("#usertrgctrl").attr('disabled', 'disabled');
        $("#tac").attr('disabled', 'disabled');
        $("#spina").attr('disabled', 'disabled');
        $("#elvt").attr('disabled', 'disabled');


        $("#ecct").attr('disabled', 'disabled');
        $("#sobnt").attr('disabled', 'disabled');
        $("#lsag").attr('disabled', 'disabled');
        $("#ssontb").attr('disabled', 'disabled');
        $("#ivvp").attr('disabled', 'disabled');
        $("#pdts").attr('disabled', 'disabled');
        $("#stnu").attr('disabled', 'disabled');
        $("#edcve").attr('disabled', 'disabled');
        $("#eppde").attr('disabled', 'disabled');
        $("#vchi").attr('disabled', 'disabled');


        $("#ded").attr('disabled', 'disabled');
        $("#sibq").attr('disabled', 'disabled');
        $("#erc").attr('disabled', 'disabled');
        $("#otrc").attr('disabled', 'disabled');
        $("#vngasid").attr('disabled', 'disabled');
        $("#eds").attr('disabled', 'disabled');
        $("#dd").attr('disabled', 'disabled');
        $("#dbc").attr('disabled', 'disabled');


        $("#prv").attr('disabled', 'disabled');
        $("#cbnc").attr('disabled', 'disabled');
        $("#salescs").attr('disabled', 'disabled');
        $("#afp").attr('disabled', 'disabled');
        $("#emfm").attr('disabled', 'disabled');
        $("#pandc").attr('disabled', 'disabled');
        $("#ird").attr('disabled', 'disabled');
        $("#loadingslip").attr('disabled', 'disabled');
        $("#dcc").attr('disabled', 'disabled');




    }



    //currenccymultiselect
    var currencyUrl = window.location.protocol + "//" + window.location.host + "/api/setupapi/currencymultiselect";
    $scope.currencyMultiSelect = {
        dataTextField: "CURRENCY_EDESC",
        dataValueField: "CURRENCY_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>...Currency...</strong></div>',
        placeholder: "Select Currency...",
        autoClose: false,
        select: function (e) {
            debugger;
            if (e.dataItem !== undefined) {
                $scope.preffArr.EXCHANGE_RATE = e.dataItem.CURRENCY_CODE;


            }

        },
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: currencyUrl,
                    dataType: "json"
                }
            }
        },


    };

    //accountmultiselct 

    var accountUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAccountCodeWithChild";
    $scope.accountMultiSelect = {
        dataTextField: "ACC_EDESC",
        dataValueField: "ACC_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>...Account...</strong></div>',
        placeholder: "Select Account...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: accountUrl,
                    dataType: "json"
                }
            }
        },

    };

    //GRNI multiselect
    var grniaccountUrl = window.location.protocol + "//" + window.location.host + "/api/Purchase/GetAccountsList";

    $scope.grniAccountDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: grniaccountUrl,
            },
            parameterMap: function (data, action) {
                if (data.filter && data.filter.filters && data.filter.filters.length > 0) {
                    return {
                        filter: data.filter.filters[0].value
                    };
                } else {
                    return {
                        filter: ""
                    };
                }
            }
        },
        schema: {
            data: function (response) {
                return response;
            },
            model: {
                id: "AccountCode",
                fields: {
                    AccountName: { type: "string" },
                    AccountCode: { type: "string" }
                }
            }
        }
    });

    $scope.grniaccountComboBoxOptions = {
        dataTextField: "AccountName",
        dataValueField: "AccountCode",
        placeholder: "Select Account...",
        filter: "contains",
        height: 179,
    };

    $scope.preferenceSetup =
    {
        BRANCH_CODE: '',
        COMPANY_CODE: '',
        FY_START_DATE: '',
        FY_END_DATE: '',
        DEFAULT_CURRENCY_CODE: '',
        EXCHANGE_RATE: '',
        CASH_ACC_CODE: '',
        GRNI_ACC_CODE: '',
        METHOD: '',
        PRINT_TYPE: '',
        PASS_EXPIRY_DAYS: '',
        FISCAL_YEAR: '',
        FREIGHT_NEGATIVE_FLAG: '',
        LEDGER_VAT_FLAG: '',
        COST_CATEGORY_FLAG: '',
        DOCUMENT_HISTORY_FLAG: '',
        DIVISION_FLAG: '',
        ORDER_TAXABLE_RATE: '',
        EXCHANGE_RATE_EDITABLE_FLAG: '',


        PAN_VAT_CONTROL_FLAG: '',
        NEGATIVE_STOCK_FLAG: '',
        SUB_NARRATION_FLAG: '',
        REFERENCE_EDIT_FLAG: '',
        CUSTOMER_APPROVAL_FLAG: '',
        MASTER_MODIFY: '',
        SUPPLIER_APPROVAL_FLAG: '',
        CREDIT_CONTROL_FLAG: '',
        CREDIT_CONTROL_AS: '',
        CREDIT_CONTROL_AT: '',
        FREQUENT_NO: '',
        ALERT_REFRESH_RATE: '',
        SECURED_BACK_DAYS: '',
        DECIMAL_PLACE: '',
        //preference flag
        NEGATIVE_STOCK_FLAG: '',
        RATE3_INFO_FLAG: '',
        FREEZE_PUR_EXP_OPTION: '',
        SUB_NARRATION_FLAG: '',
        SUBLEDGER_BILL_ADJUST_FLAG: '',
        REFERENCE_EDIT_FLAG: '',
        CUSTOMER_APPROVAL_FLAG: '',
        SUPPLIER_APPROVAL_FLAG: '',
        MASTER_MODIFY: '',
        VAT_DR_CR_ENTRY: '',
        REFERENCE_TRAN_FLAG: '',
        BRANCH_BCMAP_FLAG: '',
        STOCK_GENERIC_FLAG: '',
        TABLE_NOT_UNPOST_FLAG: '',
        GB_FLAG: '',
        //master
        GL_FLAG: '',
        SL_FLAG: '',
        AUTO_LEDGER_FLAG: '',
        AUTO_CUTOMER_FLAG: '',
        AUTO_SUPPLIER_FLAG: '',
        AUTO_ITEM_FLAG: '',
        //control
        BUDGET_CONTROL_FLAG: '',
        COST_BUDGET_INFO_FLAG: '',
        COA_BUDGET_CONTROL_FLAG: '',
        COA_BUDGET_INFO_FLAG: '',
        USER_TARGET_CONTROL_FLAG: '',
        TRANS_ALERT_CAPTURE_FLAG: '',
        PARTY_ACTIVE_INFO_FLAG: '',
        LEDGER_VAT_FLAG: '',
        COST_CATEGORY_FLAG: '',
        SUBLEDGER_ORDER_FLAG: '',
        AUTO_GENERATE_LS_FLAG: '',
        TB_SUB_LEDGER_LOAD: '',
        IND_VOUCHER_VERIFY: '',
        PRINT_DATE_TIME: '',
        TABLE_NOT_UNPOST_FLAG: '',
        VAT_DR_CR_ENTRY: '',
        PP_DETAIL_ENTRY: '',
        DOCUMENT_HISTORY_FLAG: '',
        DIVISION_FLAG: '',
        BILL_QTY_FLAG: '',
        EXCHANGE_RATE_EDITABLE_FLAG: '',
        ORDER_TAXABLE_RATE: '',
        VNO_AS_SESSION_ID_CONTROL: '',
        DEALER_SYSTEM_FLAG: '',
        DEFAULT_DIVISION: '',
        DEFAULT_BRANCH_CONSOLE: '',
        PUR_RATE_VERIANCE_FLAG: '',
        CASH_BANK_NEGATIVE_FLAG: '',
        SALES_CUPON_SCHEME_FLAG: '',
        AFP_BUDGET_CONTROL_FLAG: '',
        MASTER_FIELD_MANDATORY_FLAG: '',
        PAN_VAT_CONTROL_FLAG: '',
        SYNC_WITH_IRD: '',
        LS_BACK_ACCESS_DAYS: '',
        RATE_SCHEDULE_TYPE: '',
        DEFAULT_COGS_VALUE: '',
        TB_GENERIC_FLAG: '',
        FREEZE_ENTRY_DATE: '',
        VERSION_CONTROL_FLAG: '',
        REMARKS: '',
        BRANCH_SCMAP_FLAG: '',
        FREIGHT_RATE_FLAG: '',
        AUTO_CUSTOMER_FLAG: '',
        purchase_inv_form_code: '',
        PUR_RETURN_FORM_CODE: '',
        PUR_GRN_FORM_CODE: '',
        purchase_exp_form_code: '',
        sales_inv_form_code: '',
        SALES_RETURN_FORM_CODE: '',
        gl_reconciliation_form_code: '',
        INV_OTHERS_FORM_CODE: '',
        CONS_OTHERS_FORM_CODE: '',
        asset_depriciation_form_code: '',
        WB_AUTO_GRN_FORM_CODE: '',
        PUR_RATE_PERCENT: '',
        CREDIT_CONTROL_AS: '',
        CREDIT_CONTROL_AT: '',
        CREDIT_CONTROL_PDC_BG: '',
        CREDIT_CONTROL_DAYS: '',
        FREIGHT_FORM_CODE: '',
        LINK_DATABASE: '',
    }

    $scope.preffArr = $scope.preferenceSetup;

    //Save and Update Function
    $scope.saveNewPreferencesetup = function (isValid) {

        if (!isValid) {
            displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
            return;
        }
        debugger;

        if ($scope.saveupdatebtn == "Save") {

            if ($scope.preffArr.EXCHANGE_RATE === "NRS") {
                $scope.preffArr.EXCHANGE_RATE = "1";
            }
            else if ($scope.preffArr.EXCHANGE_RATE === "IRS") {
                $scope.preffArr.EXCHANGE_RATE = "1.6";
            }
            else if ($scope.preffArr.EXCHANGE_RATE === "USD") {
                $scope.preffArr.EXCHANGE_RATE = "107";
            }
            var createurl = window.location.protocol + "//" + window.location.host + "/api/setupapi/createPreferenceSetup";
            var datas = {
                COMPANY_CODE: $scope.preffArr.COMPANY_CODE[0],
                BRANCH_CODE: $scope.preffArr.BRANCH_CODE[0],
                FY_START_DATE: $scope.preffArr.FY_START_DATE,
                FY_END_DATE: $scope.preffArr.FY_END_DATE,
                DEFAULT_CURRENCY_CODE: $scope.preffArr.DEFAULT_CURRENCY_CODE,
                EXCHANGE_RATE: $scope.preffArr.EXCHANGE_RATE,
                CASH_ACC_CODE: $scope.preffArr.CASH_ACC_CODE,
                GRNI_ACC_CODE: $scope.preffArr.GRNI_ACC_CODE,
                METHOD: $scope.preffArr.METHOD,
                PRINT_TYPE: $scope.preffArr.PRINT_TYPE,
                PASS_EXPIRY_DAYS: $scope.preffArr.PASS_EXPIRY_DAYS,
                FISCAL_YEAR: $scope.preffArr.FISCAL_YEAR,
                FREIGHT_NEGATIVE_FLAG: $scope.preffArr.FREIGHT_NEGATIVE_FLAG,
                LEDGER_VAT_FLAG: $scope.preffArr.LEDGER_VAT_FLAG,
                COST_CATEGORY_FLAG: $scope.preffArr.COST_CATEGORY_FLAG,
                DOCUMENT_HISTORY_FLAG: $scope.preffArr.DOCUMENT_HISTORY_FLAG,
                DIVISION_FLAG: $scope.preffArr.DIVISION_FLAG,
                ORDER_TAXABLE_RATE: $scope.preffArr.ORDER_TAXABLE_RATE,
                EXCHANGE_RATE_EDITABLE_FLAG: $scope.preffArr.EXCHANGE_RATE_EDITABLE_FLAG,
                VNO_AS_SESSION_ID_CONTROL: $scope.preffArr.VNO_AS_SESSION_ID_CONTROL,
                DEALER_SYSTEM_FLAG: $scope.preffArr.DEALER_SYSTEM_FLAG,
                CREDIT_CONTROL_FLAG: $scope.preffArr.CREDIT_CONTROL_FLAG,
                CREDIT_CONTROL_AS: $scope.preffArr.CREDIT_CONTROL_AS,
                CREDIT_CONTROL_AT: $scope.preffArr.CREDIT_CONTROL_AT,
                FREQUENT_NO: $scope.preffArr.FREQUENT_NO,
                ALERT_REFRESH_RATE: $scope.preffArr.ALERT_REFRESH_RATE,
                SECURED_BACK_DAYS: $scope.preffArr.SECURED_BACK_DAYS,
                DECIMAL_PLACE: $scope.preffArr.DECIMAL_PLACE,
                //prefernce flag  FREIGHT_RATE_FLAG
                NEGATIVE_STOCK_FLAG: $scope.preffArr.NEGATIVE_STOCK_FLAG,
                RATE3_INFO_FLAG: $scope.preffArr.RATE3_INFO_FLAG,
                FREEZE_PUR_EXP_OPTION: $scope.preffArr.FREEZE_PUR_EXP_OPTION,
                SUB_NARRATION_FLAG: $scope.preffArr.SUB_NARRATION_FLAG,
                SUBLEDGER_BILL_ADJUST_FLAG: $scope.preffArr.SUBLEDGER_BILL_ADJUST_FLAG,
                REFERENCE_EDIT_FLAG: $scope.preffArr.REFERENCE_EDIT_FLAG,
                CUSTOMER_APPROVAL_FLAG: $scope.preffArr.CUSTOMER_APPROVAL_FLAG,
                SUPPLIER_APPROVAL_FLAG: $scope.preffArr.SUPPLIER_APPROVAL_FLAG,
                MASTER_MODIFY: $scope.preffArr.MASTER_MODIFY,
                VAT_DR_CR_ENTRY: $scope.preffArr.VAT_DR_CR_ENTRY,
                REFERENCE_TRAN_FLAG: $scope.preffArr.REFERENCE_TRAN_FLAG,
                BRANCH_BCMAP_FLAG: $scope.preffArr.BRANCH_BCMAP_FLAG,
                STOCK_GENERIC_FLAG: $scope.preffArr.STOCK_GENERIC_FLAG,
                TABLE_NOT_UNPOST_FLAG: $scope.preffArr.TABLE_NOT_UNPOST_FLAG,
                DEFAULT_COGS_VALUE: $scope.preffArr.DEFAULT_COGS_VALUE,
                GB_FLAG: $scope.preffArr.GB_FLAG,
                TB_GENERIC_FLAG: $scope.preffArr.TB_GENERIC_FLAG,
                FREEZE_ENTRY_DATE: $scope.preffArr.FREEZE_FLAG === "Y" ? $scope.preffArr.FREEZE_ENTRY_DATE : "",
                VERSION_CONTROL_FLAG: $scope.preffArr.VERSION_CONTROL_FLAG,
                purchase_inv_form_code: $scope.preffArr.purchase_inv_form_code,
                PUR_RETURN_FORM_CODE: $scope.preffArr.PUR_RETURN_FORM_CODE,
                PUR_GRN_FORM_CODE: $scope.preffArr.PUR_GRN_FORM_CODE,
                purchase_exp_form_code: $scope.preffArr.purchase_exp_form_code,
                sales_inv_form_code: $scope.preffArr.sales_inv_form_code,
                SALES_RETURN_FORM_CODE: $scope.preffArr.SALES_RETURN_FORM_CODE,
                gl_reconciliation_form_code: $scope.preffArr.gl_reconciliation_form_code,
                INV_OTHERS_FORM_CODE: $scope.preffArr.INV_OTHERS_FORM_CODE,
                CONS_OTHERS_FORM_CODE: $scope.preffArr.CONS_OTHERS_FORM_CODE,
                asset_depriciation_form_code: $scope.preffArr.asset_depriciation_form_code,
                WB_AUTO_GRN_FORM_CODE: $scope.preffArr.WB_AUTO_GRN_FORM_CODE,
                PUR_RATE_PERCENT: $scope.preffArr.PUR_RATE_PERCENT,
                CREDIT_CONTROL_AS: $scope.preffArr.CREDIT_CONTROL_AS,
                CREDIT_CONTROl_AT: $scope.preffArr.CREDIT_CONTROL_AT,
                CREDIT_CONTROL_PDC_BG: $scope.preffArr.CREDIT_CONTROL_PDC_BG,
                CREDIT_CONTROL_DAYS: $scope.preffArr.CREDIT_CONTROL_DAYS,
                FREIGHT_FORM_CODE: $scope.preffArr.FREIGHT_FORM_CODE,
                LINK_DATABASE: $scopoe.preffArr.LINK_DATABASE,
                REMARKS: $scope.preffArr.REMARKS,
                BRANCH_SCMAP_FLAG: $scope.preffArr.BRANCH_SCMAP_FLAG,
                //master reff  
                GL_FLAG: $scope.preffArr.GL_FLAG,
                SL_FLAG: $scope.preffArr.SL_FLAG,
                AUTO_LEDGER_FLAG: $scope.preffArr.AUTO_LEDGER_FLAG,
                AUTO_CUSTOMER_FLAG: $scope.preffArr.AUTO_CUSTOMER_FLAG,
                AUTO_SUPPLIER_FLAG: $scope.preffArr.AUTO_SUPPLIER_FLAG,
                AUTO_ITEM_FLAG: $scope.preffArr.AUTO_ITEM_FLAG,
                //control
                BUDGET_CONTROL_FLAG: $scope.preffArr.BUDGET_CONTROL_FLAG,
                COST_BUDGET_INFO_FLAG: $scope.preffArr.COST_BUDGET_INFO_FLAG,
                COA_BUDGET_CONTROL_FLAG: $scope.preffArr.COA_BUDGET_CONTROL_FLAG,
                COA_BUDGET_INFO_FLAG: $scope.preffArr.COA_BUDGET_INFO_FLAG,
                USER_TARGET_CONTROL_FLAG: $scope.preffArr.USER_TARGET_CONTROL_FLAG,
                TRANS_ALERT_CAPTURE_FLAG: $scope.preffArr.TRANS_ALERT_CAPTURE_FLAG,
                PARTY_ACTIVE_INFO_FLAG: $scope.preffArr.PARTY_ACTIVE_INFO_FLAG,
                LEDGER_VAT_FLAG: $scope.preffArr.LEDGER_VAT_FLAG,
                COST_CATEGORY_FLAG: $scope.preffArr.COST_CATEGORY_FLAG,
                SUBLEDGER_ORDER_FLAG: $scope.preffArr.SUBLEDGER_ORDER_FLAG,
                AUTO_GENERATE_LS_FLAG: $scope.preffArr.AUTO_GENERATE_LS_FLAG,
                TB_SUB_LEDGER_LOAD: $scope.preffArr.TB_SUB_LEDGER_LOAD,
                IND_VOUCHER_VERIFY: $scope.preffArr.IND_VOUCHER_VERIFY,
                PRINT_DATE_TIME: $scope.preffArr.PRINT_DATE_TIME,
                TABLE_NOT_UNPOST_FLAG: $scope.preffArr.TABLE_NOT_UNPOST_FLAG,
                PP_DETAIL_ENTRY: $scope.preffArr.PP_DETAIL_ENTRY,
                DOCUMENT_HISTORY_FLAG: $scope.preffArr.DOCUMENT_HISTORY_FLAG,
                DIVISION_FLAG: $scope.preffArr.DIVISION_FLAG,
                BILL_QTY_FLAG: $scope.preffArr.BILL_QTY_FLAG,
                EXCHANGE_RATE_EDITABLE_FLAG: $scope.preffArr.EXCHANGE_RATE_EDITABLE_FLAG,
                ORDER_TAXABLE_RATE: $scope.preffArr.ORDER_TAXABLE_RATE,
                VNO_AS_SESSION_ID_CONTROL: $scope.preffArr.VNO_AS_SESSION_ID_CONTROL,
                DEALER_SYSTEM_FLAG: $scope.preffArr.DEALER_SYSTEM_FLAG,
                DEFAULT_DIVISION: $scope.preffArr.DEFAULT_DIVISION,
                DEFAULT_BRANCH_CONSOLE: $scope.preffArr.DEFAULT_BRANCH_CONSOLE,
                PUR_RATE_VERIANCE_FLAG: $scope.preffArr.PUR_RATE_VERIANCE_FLAG,
                CASH_BANK_NEGATIVE_FLAG: $scope.preffArr.CASH_BANK_NEGATIVE_FLAG,
                SALES_CUPON_SCHEME_FLAG: $scope.preffArr.SALES_CUPON_SCHEME_FLAG,
                AFP_BUDGET_CONTROL_FLAG: $scope.preffArr.AFP_BUDGET_CONTROL_FLAG,
                MASTER_FIELD_MANDATORY_FLAG: $scope.preffArr.MASTER_FIELD_MANDATORY_FLAG,
                PAN_VAT_CONTROL_FLAG: $scope.preffArr.PAN_VAT_CONTROL_FLAG,
                SYNC_WITH_IRD: $scope.preffArr.SYNC_WITH_IRD,
                LS_BACK_ACCESS_DAYS: $scope.preffArr.LS_BACK_ACCESS_DAYS,
                RATE_SCHEDULE_TYPE: $scope.preffArr.RATE_SCHEDULE_TYPE,
                FREIGHT_RATE_FLAG: $scope.preffArr.FREIGHT_RATE_FLAG,

            }

            var grid = $("#kGrid").data("kendoGrid").dataSource.data();
            var hasChild = $.grep(grid, function (n) {
                return (n.COMPANY_CODE == datas.COMPANY_CODE);
            });
            if (hasChild.length > 0) {
                displayPopupNotification("The Company already exits in the Preference Setup", "warning");
                return;
            }
            debugger;
            $http({
                method: 'post',
                url: createurl,
                data: JSON.stringify(datas)
            }).then(function successcallback(response) {

                if (response.data.MESSAGE == "INSERTED") {
                    { $("#CompanyModal").modal("toggle"); }

                    var grid = $("#kGrid").data("kendoGrid");
                    if (grid != undefined) {

                        grid.dataSource.read();
                    }


                    $scope.disableform();
                    displayPopupNotification("data succesfully inserted ", "success");
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Company and brach name preference setup already exist please try another company and brach name for preference setup.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            }, function errorcallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Company and brach name preference setup already exist please try another company and brach name for preference setup.", "error");
                }
                displayPopupNotification("Something went wrong.Please try again later.", "error");

            });
        }
        else {

            var updateurl = window.location.protocol + "//" + window.location.host + "/api/setupapi/updatePreference";


            $scope.saveupdatebtn = "Update";
            debugger;
            if ($scope.preffArr.EXCHANGE_RATE === "NRS") {
                $scope.preffArr.EXCHANGE_RATE = "1";
            }
            else if ($scope.preffArr.EXCHANGE_RATE === "IRS") {
                $scope.preffArr.EXCHANGE_RATE = "1.6";
            }
            else if ($scope.preffArr.EXCHANGE_RATE === "USD") {
                $scope.preffArr.EXCHANGE_RATE = "107";
            }
            var model = {
                COMPANY_CODE: $scope.preffArr.COMPANY_CODE[0] == 0 ? $scope.preffArr.COMPANY_CODE : $scope.preffArr.COMPANY_CODE[0],
                BRANCH_CODE: $scope.preffArr.BRANCH_CODE[0] == 0 ? $scope.preffArr.BRANCH_CODE : $scope.preffArr.BRANCH_CODE[0],
                FY_START_DATE: $scope.preffArr.FY_START_DATE,
                FY_END_DATE: $scope.preffArr.FY_END_DATE,
                DEFAULT_CURRENCY_CODE: $scope.preffArr.DEFAULT_CURRENCY_CODE,
                EXCHANGE_RATE: $scope.preffArr.EXCHANGE_RATE,
                CASH_ACC_CODE: $scope.preffArr.CASH_ACC_CODE,
                GRNI_ACC_CODE: $scope.preffArr.GRNI_ACC_CODE,
                METHOD: $scope.preffArr.METHOD,
                PRINT_TYPE: $scope.preffArr.PRINT_TYPE,
                PASS_EXPIRY_DAYS: $scope.preffArr.PASS_EXPIRY_DAYS,
                FISCAL_YEAR: $scope.preffArr.FISCAL_YEAR,
                FREIGHT_NEGATIVE_FLAG: $scope.preffArr.FREIGHT_NEGATIVE_FLAG,
                LEDGER_VAT_FLAG: $scope.preffArr.LEDGER_VAT_FLAG,
                COST_CATEGORY_FLAG: $scope.preffArr.COST_CATEGORY_FLAG,
                DOCUMENT_HISTORY_FLAG: $scope.preffArr.DOCUMENT_HISTORY_FLAG,
                DIVISION_FLAG: $scope.preffArr.DIVISION_FLAG,
                ORDER_TAXABLE_RATE: $scope.preffArr.ORDER_TAXABLE_RATE,
                EXCHANGE_RATE_EDITABLE_FLAG: $scope.preffArr.EXCHANGE_RATE_EDITABLE_FLAG,
                VNO_AS_SESSION_ID_CONTROL: $scope.preffArr.VNO_AS_SESSION_ID_CONTROL,
                DEALER_SYSTEM_FLAG: $scope.preffArr.DEALER_SYSTEM_FLAG,
                CREDIT_CONTROL_FLAG: $scope.preffArr.CREDIT_CONTROL_FLAG,
                CREDIT_CONTROL_AS: $scope.preffArr.CREDIT_CONTROL_AS,
                CREDIT_CONTROL_AT: $scope.preffArr.CREDIT_CONTROL_AT,
                FREQUENT_NO: $scope.preffArr.FREQUENT_NO,
                ALERT_REFRESH_RATE: $scope.preffArr.ALERT_REFRESH_RATE,
                SECURED_BACK_DAYS: $scope.preffArr.SECURED_BACK_DAYS,
                DECIMAL_PLACE: $scope.preffArr.DECIMAL_PLACE,
                //prefernce flag 
                NEGATIVE_STOCK_FLAG: $scope.preffArr.NEGATIVE_STOCK_FLAG,
                RATE3_INFO_FLAG: $scope.preffArr.RATE3_INFO_FLAG,
                FREEZE_PUR_EXP_OPTION: $scope.preffArr.FREEZE_PUR_EXP_OPTION,
                SUB_NARRATION_FLAG: $scope.preffArr.SUB_NARRATION_FLAG,
                SUBLEDGER_BILL_ADJUST_FLAG: $scope.preffArr.SUBLEDGER_BILL_ADJUST_FLAG,
                REFERENCE_EDIT_FLAG: $scope.preffArr.REFERENCE_EDIT_FLAG,
                CUSTOMER_APPROVAL_FLAG: $scope.preffArr.CUSTOMER_APPROVAL_FLAG,
                SUPPLIER_APPROVAL_FLAG: $scope.preffArr.SUPPLIER_APPROVAL_FLAG,
                MASTER_MODIFY: $scope.preffArr.MASTER_MODIFY,
                VAT_DR_CR_ENTRY: $scope.preffArr.VAT_DR_CR_ENTRY,
                REFERENCE_TRAN_FLAG: $scope.preffArr.REFERENCE_TRAN_FLAG,
                BRANCH_BCMAP_FLAG: $scope.preffArr.BRANCH_BCMAP_FLAG,
                STOCK_GENERIC_FLAG: $scope.preffArr.STOCK_GENERIC_FLAG,
                TABLE_NOT_UNPOST_FLAG: $scope.preffArr.TABLE_NOT_UNPOST_FLAG,
                DEFAULT_COGS_VALUE: $scope.preffArr.DEFAULT_COGS_VALUE,
                GB_FLAG: $scope.preffArr.GB_FLAG,
                TB_GENERIC_FLAG: $scope.preffArr.TB_GENERIC_FLAG,
                FREEZE_ENTRY_DATE: $scope.preffArr.FREEZE_FLAG === "Y" ? $scope.preffArr.FREEZE_ENTRY_DATE : "",
                VERSION_CONTROL_FLAG: $scope.preffArr.VERSION_CONTROL_FLAG,
                purchase_inv_form_code: $scope.preffArr.purchase_inv_form_code,
                PUR_RETURN_FORM_CODE: $scope.preffArr.PUR_RETURN_FORM_CODE,
                PUR_GRN_FORM_CODE: $scope.preffArr.PUR_GRN_FORM_CODE,
                purchase_exp_form_code: $scope.preffArr.purchase_exp_form_code,
                sales_inv_form_code: $scope.preffArr.sales_inv_form_code,
                SALES_RETURN_FORM_CODE: $scope.preffArr.SALES_RETURN_FORM_CODE,
                gl_reconciliation_form_code: $scope.preffArr.gl_reconciliation_form_code,
                INV_OTHERS_FORM_CODE: $scope.preffArr.INV_OTHERS_FORM_CODE,
                CONS_OTHERS_FORM_CODE: $scope.preffArr.CONS_OTHERS_FORM_CODE,
                asset_depriciation_form_code: $scope.preffArr.asset_depriciation_form_code,
                WB_AUTO_GRN_FORM_CODE: $scope.preffArr.WB_AUTO_GRN_FORM_CODE,
                PUR_RATE_PERCENT: $scope.preffArr.PUR_RATE_PERCENT,
                CREDIT_CONTROL_AS: $scope.preffArr.CREDIT_CONTROL_AS,
                CREDIT_CONTROl_AT: $scope.preffArr.CREDIT_CONTROL_AT,
                CREDIT_CONTROL_PDC_BG: $scope.preffArr.CREDIT_CONTROL_PDC_BG,
                CREDIT_CONTROL_DAYS: $scope.preffArr.CREDIT_CONTROL_DAYS,
                FREIGHT_FORM_CODE: $scope.preffArr.FREIGHT_FORM_CODE,
                LINK_DATABASE: $scope.preffArr.LINK_DATABASE,
                REMARKS: $scope.preffArr.REMARKS,
                BRANCH_SCMAP_FLAG: $scope.preffArr.BRANCH_SCMAP_FLAG,
                //master reff
                GL_FLAG: $scope.preffArr.GL_FLAG,
                SL_FLAG: $scope.preffArr.SL_FLAG,
                AUTO_LEDGER_FLAG: $scope.preffArr.AUTO_LEDGER_FLAG,
                AUTO_CUSTOMER_FLAG: $scope.preffArr.AUTO_CUSTOMER_FLAG,
                AUTO_SUPPLIER_FLAG: $scope.preffArr.AUTO_SUPPLIER_FLAG,
                AUTO_ITEM_FLAG: $scope.preffArr.AUTO_ITEM_FLAG,
                //control
                BUDGET_CONTROL_FLAG: $scope.preffArr.BUDGET_CONTROL_FLAG,
                COST_BUDGET_INFO_FLAG: $scope.preffArr.COST_BUDGET_INFO_FLAG,
                COA_BUDGET_CONTROL_FLAG: $scope.preffArr.COA_BUDGET_CONTROL_FLAG,
                COA_BUDGET_INFO_FLAG: $scope.preffArr.COA_BUDGET_INFO_FLAG,
                USER_TARGET_CONTROL_FLAG: $scope.preffArr.USER_TARGET_CONTROL_FLAG,
                TRANS_ALERT_CAPTURE_FLAG: $scope.preffArr.TRANS_ALERT_CAPTURE_FLAG,
                PARTY_ACTIVE_INFO_FLAG: $scope.preffArr.PARTY_ACTIVE_INFO_FLAG,
                LEDGER_VAT_FLAG: $scope.preffArr.LEDGER_VAT_FLAG,
                COST_CATEGORY_FLAG: $scope.preffArr.COST_CATEGORY_FLAG,
                SUBLEDGER_ORDER_FLAG: $scope.preffArr.SUBLEDGER_ORDER_FLAG,
                AUTO_GENERATE_LS_FLAG: $scope.preffArr.AUTO_GENERATE_LS_FLAG,
                TB_SUB_LEDGER_LOAD: $scope.preffArr.TB_SUB_LEDGER_LOAD,
                IND_VOUCHER_VERIFY: $scope.preffArr.IND_VOUCHER_VERIFY,
                PRINT_DATE_TIME: $scope.preffArr.PRINT_DATE_TIME,
                TABLE_NOT_UNPOST_FLAG: $scope.preffArr.TABLE_NOT_UNPOST_FLAG,
                PP_DETAIL_ENTRY: $scope.preffArr.PP_DETAIL_ENTRY,
                DOCUMENT_HISTORY_FLAG: $scope.preffArr.DOCUMENT_HISTORY_FLAG,
                DIVISION_FLAG: $scope.preffArr.DIVISION_FLAG,
                BILL_QTY_FLAG: $scope.preffArr.BILL_QTY_FLAG,
                EXCHANGE_RATE_EDITABLE_FLAG: $scope.preffArr.EXCHANGE_RATE_EDITABLE_FLAG,
                ORDER_TAXABLE_RATE: $scope.preffArr.ORDER_TAXABLE_RATE,
                VNO_AS_SESSION_ID_CONTROL: $scope.preffArr.VNO_AS_SESSION_ID_CONTROL,
                DEALER_SYSTEM_FLAG: $scope.preffArr.DEALER_SYSTEM_FLAG,
                DEFAULT_DIVISION: $scope.preffArr.DEFAULT_DIVISION,
                DEFAULT_BRANCH_CONSOLE: $scope.preffArr.DEFAULT_BRANCH_CONSOLE,
                PUR_RATE_VERIANCE_FLAG: $scope.preffArr.PUR_RATE_VERIANCE_FLAG,
                CASH_BANK_NEGATIVE_FLAG: $scope.preffArr.CASH_BANK_NEGATIVE_FLAG,
                SALES_CUPON_SCHEME_FLAG: $scope.preffArr.SALES_CUPON_SCHEME_FLAG,
                AFP_BUDGET_CONTROL_FLAG: $scope.preffArr.AFP_BUDGET_CONTROL_FLAG,
                MASTER_FIELD_MANDATORY_FLAG: $scope.preffArr.MASTER_FIELD_MANDATORY_FLAG,
                PAN_VAT_CONTROL_FLAG: $scope.preffArr.PAN_VAT_CONTROL_FLAG,
                SYNC_WITH_IRD: $scope.preffArr.SYNC_WITH_IRD,
                LS_BACK_ACCESS_DAYS: $scope.preffArr.LS_BACK_ACCESS_DAYS,
                RATE_SCHEDULE_TYPE: $scope.preffArr.RATE_SCHEDULE_TYPE,
                FREIGHT_RATE_FLAG: $scope.preffArr.FREIGHT_RATE_FLAG,
            }

            $http({
                method: 'post',
                url: updateurl,
                data: model
            }).then(function successcallback(response) {



                if (response.data.MESSAGE == "UPDATED") {
                    displayPopupNotification("data succesfully updated ", "success");
                    $("#CompanyModal").modal("toggle");
                    var grid = $("#kGrid").data("kendoGrid");

                    if (grid != undefined) {
                        grid.dataSource.read();
                    }
                    //$scope.init();
                    $scope.disableform();

                    $scope.saveupdatebtn = "Save";
                }
                if (response.data.MESSAGE == "error") {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }

            }, function errorcallback(response) {

                displayPopupNotification("Something went wrong.Please try again later.", "error");

            });




        }


    }

    //disable remove
    $scope.Removedisableform = function () {
        $("#distCompanySelect").removeAttr('disabled');
        $("#distBranchSelect").removeAttr('disabled');
        $("#currencyMultiSelect").removeAttr('disabled');
        $("#exchangerate").removeAttr('disabled');
        $("#frequently").removeAttr('disabled');
        $("#alterrate").removeAttr('disabled');
        $("#freezabackdays").removeAttr('disabled');
        $("#decimal").removeAttr('disabled');

        $("#passwordexp").removeAttr('disabled');
        $("#englishdatedocument5").removeAttr('disabled');
        $("#englishdatedocument6").removeAttr('disabled');
        $("#nepaliDate5").removeAttr('disabled');
        $("#nepaliDate6").removeAttr('disabled');
        $("#accountMultiSelect").removeAttr('disabled');
        $("#costingmethod").removeAttr('disabled');
        $("#fysicalyear").removeAttr('disabled');

        $("#printType").removeAttr('disabled');
        $("#grniaccountMultiSelect").removeAttr('disabled');
        $("#negativectrl").removeAttr('disabled');
        $("#ledgervat").removeAttr('disabled');
        $("#costcategoryflg").removeAttr('disabled');
        $("#documenthistry").removeAttr('disabled');
        $("#divigionflg").removeAttr('disabled');
        $("#ordertx").removeAttr('disabled');


        $("#exchangerateflg").removeAttr('disabled');
        $("#vnoflg").removeAttr('disabled');
        $("#dealersystemflg").removeAttr('disabled');
        $("#panvatflg").removeAttr('disabled');
        $("#negativestkflg").removeAttr('disabled');
        $("#subnarationflg").removeAttr('disabled');
        $("#preeditflg").removeAttr('disabled');
        $("#custaprflg").removeAttr('disabled');


        $("#mastermodifyflg").removeAttr('disabled');
        $("#supplierflg").removeAttr('disabled');
        $("#creditctrlflg").removeAttr('disabled');
        $("#PUR_RATE_PERCENT").removeAttr('disabled');
        $("#CREDIT_CONTROL_AS").removeAttr('disabled');
        $("#CREDIT_CONTROL_AT").removeAttr('disabled');
        $("#CREDIT_CONTROL_PDC_BG").removeAttr('disabled');
        $("#CREDIT_CONTROL_DAYS").removeAttr('disabled');
        $("#creditctrlasflg").removeAttr('disabled');
        $("#creditatflg").removeAttr('disabled');
        $("#savedocumentformdata").removeAttr('disabled');
        //
        //Preference Control
        $("#billwiseadjust").removeAttr('disabled');
        $("#preeditflg").removeAttr('disabled');
        $("#custaprflg").removeAttr('disabled');
        $("#supplierflg").removeAttr('disabled');
        $("#mastermodifyflg").removeAttr('disabled');
        $("#Tbgeneric").removeAttr('disabled');
        $("#versionControlFlag").removeAttr('disabled');
        $("#purchase_inv_form_code").removeAttr('disabled');
        $("#PUR_RETURN_FORM_CODE").removeAttr('disabled');
        $("#PUR_GRN_FORM_CODE").removeAttr('disabled');
        $("#purchase_exp_form_code").removeAttr('disabled');
        $("#sales_inv_form_code").removeAttr('disabled');
        $("#SALES_RETURN_FORM_CODE").removeAttr('disabled');
        $("#gl_reconciliation_form_code").removeAttr('disabled');
        $("#INV_OTHERS_FORM_CODE").removeAttr('disabled');
        $("#CONS_OTHERS_FORM_CODE").removeAttr('disabled');
        $("#asset_depriciation_form_code").removeAttr('disabled');
        $("#WB_AUTO_GRN_FORM_CODE").removeAttr("disabled");
        $("#FREIGHT_FORM_CODE").removeAttr('disabled');
        $("#linkDatabase").removeAttr('disabled')
        $("#englishdatedocument7").removeAttr('disabled');
        $("#nepaliDate7").removeAttr('disabled');
        $("#remarks").removeAttr('disabled');
        $("#freezeFlag").removeAttr('disabled');
        $("#bwsmf").removeAttr('disabled');
        $("#sublbr").removeAttr('disabled');
        $("#branchsubledger").removeAttr('disabled');
        $("#bwbmf").removeAttr('disabled');

        $("#pslg").removeAttr('disabled');
        $("#gid").removeAttr('disabled');

        $("#cogsvlaue").removeAttr('disabled');
        $("#negativestock").removeAttr('disabled');
        $("#itemlast3rate").removeAttr('disabled');

        $("#freezecost").removeAttr('disabled');
        $("#subledgernarration").removeAttr('disabled');
        //Master
        $("#generalledger").removeAttr('disabled');

        $("#stockledger").removeAttr('disabled');
        $("#ledger").removeAttr('disabled');
        $("#customer").removeAttr('disabled');
        $("#supplier").removeAttr('disabled');
        $("#item").removeAttr('disabled');
        //ocntrol
        $("#fnc").removeAttr('disabled');
        $("#frc").removeAttr('disabled');
        $("#ccc").removeAttr('disabled');


        $("#cdcc").removeAttr('disabled');
        $("#dcc").removeAttr('disabled');
        $("#none").removeAttr('disabled');
        $("#pdc").removeAttr('disabled');
        $("#csr").removeAttr('disabled');
        $("#dptr").removeAttr('disabled');
        $("#dar").removeAttr('disabled');
        $("#urd").removeAttr('disabled');

        $("#budgetctrl").removeAttr('disabled');
        $("#budgetinfo").removeAttr('disabled');
        $("#ledgerbgtctrl").removeAttr('disabled');
        $("#ledgerbgtinfo").removeAttr('disabled');
        $("#usertrgctrl").removeAttr('disabled');
        $("#tac").removeAttr('disabled');
        $("#spina").removeAttr('disabled');
        $("#elvt").removeAttr('disabled');


        $("#ecct").removeAttr('disabled');
        $("#sobnt").removeAttr('disabled');
        $("#lsag").removeAttr('disabled');
        $("#ssontb").removeAttr('disabled');
        $("#ivvp").removeAttr('disabled');
        $("#pdts").removeAttr('disabled');
        $("#stnu").removeAttr('disabled');
        $("#edcve").removeAttr('disabled');
        $("#eppde").removeAttr('disabled');
        $("#vchi").removeAttr('disabled');


        $("#ded").removeAttr('disabled');
        $("#sibq").removeAttr('disabled');
        $("#erc").removeAttr('disabled');
        $("#otrc").removeAttr('disabled');
        $("#vngasid").removeAttr('disabled');
        $("#eds").removeAttr('disabled');
        $("#dd").removeAttr('disabled');
        $("#dbc").removeAttr('disabled');


        $("#prv").removeAttr('disabled');
        $("#cbnc").removeAttr('disabled');
        $("#salescs").removeAttr('disabled');
        $("#afp").removeAttr('disabled');
        $("#emfm").removeAttr('disabled');
        $("#pandc").removeAttr('disabled');
        $("#ird").removeAttr('disabled');
        $("#loadingslip").removeAttr('disabled');
        $("#dcc").removeAttr('disabled');

    }
    //Binding while editing
    $scope.editPreferenceSetup = function () {
        $("#updatedocumentformdata").attr('disabled', 'disabled');
        $scope.saveupdatebtn = "Update";
        //$scope.init();
        $scope.Removedisableform();

    }


    $scope.Disable = function () {

        //$scope.saveupdatebtn = "Update";
        //$scope.init();
    }


    //date converter

    $scope.ConvertNepToEng = function ($event) {

        //$event.stopPropagation();
        console.log($(this));
        var date = BS2AD($("#nepaliDate5").val());
        $("#englishdatedocument").val($filter('date')(date, "dd-MMM-yyyy"));
        $('#nepaliDate5').trigger('change');
    };
    $scope.ConvertEngToNepang = function (data, id) {

        var lastChar = id[id.length - 1];
        var ids = "#nepaliDate" + lastChar + "";
        $(ids).val(AD2BS(data));
    };

    $scope.monthSelectorOptions = {
        open: function () {

            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() + 100);
        },
        change: function () {

            var id = this.element.attr('id');
            $scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'), id)
        },
        format: "dd-MMM-yyyy",

        // specifies that DateInput is used for masking the input element
        dateInput: true
    };

    $scope.monthSelectorOptionsSingle = {
        open: function () {
            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",

        // specifies that DateInput is used for masking the input element
        dateInput: true
    };


    var openingDate = $('#englishdatedocument5').val();
    $scope.OnOpeningDateChange = function () {
        debugger
        var openingDate = $('#englishdatedocument5').val();
        var maturityDate = $('#englishdatedocument6').val();
        var oDate = moment(openingDate).format("MM-DD-YYYY");
        var mDate = moment(maturityDate).format("MM-DD-YYYY");
        if (maturityDate != "") {
            if (mDate < oDate) {
                $('#englishdatedocument5').val("");
                $("#savedocumentformdata").prop("disabled", true);
                displayPopupNotification("Start date must be less than End  Date!", "warning");
                return;
            }
            else {
                $("#savedocumentformdata").prop("disabled", false);
            }

        }

    };
    //OnNepaliOpeningDateChange
    $scope.OnMaturityDateChange = function () {
        debugger;
        var openingDate = $('#englishdatedocument5').val();
        var maturityDate = $('#englishdatedocument6').val();
        // var date1 = moment(maturityDate).format("DD-MM-YYYY")


        var oDate = moment(openingDate).format("MM-DD-YYYY");
        var mDate = moment(maturityDate).format("MM-DD-YYYY");
        if (mDate < oDate) {
            $('#englishdatedocument6').val("");
            $("#savedocumentformdata").prop("disabled", true);
            displayPopupNotification("End date must be greater than Start Date!", "warning");
            return;
        }
        else {
            $("#savedocumentformdata").prop("disabled", false);
        }
    };

    $scope.clearFields = function () {

        $scope.preffArr.COMPANY_CODE = "";
        $scope.preffArr.BRANCH_CODE = "";
        $scope.preffArr.FY_START_DATE = "";
        $scope.preffArr.FY_END_DATE = "";
        $scope.preffArr.DEFAULT_CURRENCY_CODE = " ";
        $scope.preffArr.EXCHANGE_RATE = " ";
        $scope.preffArr.CASH_ACC_CODE = "";
        $scope.preffArr.GRNI_ACC_CODE = "";
        $scope.preffArr.METHOD = "";
        $scope.preffArr.PRINT_TYPE = "";
        $scope.preffArr.PASS_EXPIRY_DAYS = "";
        $scope.preffArr.FISCAL_YEAR = " ";
        $scope.preffArr.FREIGHT_NEGATIVE_FLAG = " ";
        $scope.preffArr.LEDGER_VAT_FLAG = " ";
        $scope.preffArr.DOCUMENT_HISTORY_FLAG = " ";
        $scope.preffArr.DIVISION_FLAG = "";
        $scope.preffArr.ORDER_TAXABLE_RATE = "";
        $scope.preffArr.EXCHANGE_RATE_EDITABLE_FLAG = "";
        $scope.preffArr.VNO_AS_SESSION_ID_CONTROL = "";
        $scope.preffArr.DEFAULT_CURRENCY_CODE = " ";
        $scope.preffArr.DEALER_SYSTEM_FLAG = " ";
        $scope.preffArr.PAN_VAT_CONTROL_FLAG = "";
        $scope.preffArr.NEGATIVE_STOCK_FLAG = "";
        $scope.preffArr.SUB_NARRATION_FLAG = "";
        $scope.preffArr.REFERENCE_EDIT_FLAG = "";
        $scope.preffArr.CUSTOMER_APPROVAL_FLAG = " ";
        $scope.preffArr.MASTER_MODIFY = " ";
        $scope.preffArr.SUPPLIER_APPROVAL_FLAG = " ";
        $scope.preffArr.CREDIT_CONTROL_FLAG = " ";
        $scope.preffArr.CREDIT_CONTROL_AS = "";
        $scope.preffArr.CREDIT_CONTROL_AT = "";
        $scope.preffArr.FREQUENT_NO = "";
        $scope.preffArr.ALERT_REFRESH_RATE = "";
        $scope.preffArr.SECURED_BACK_DAYS = " ";
        $scope.preffArr.DECIMAL_PLACE = " ";
        //
        $scope.preffArr.RATE3_INFO_FLAG = "";
        $scope.preffArr.FREEZE_PUR_EXP_OPTION = "";
        $scope.preffArr.SUBLEDGER_BILL_ADJUST_FLAG = "";
        $scope.preffArr.VAT_DR_CR_ENTRY = "";
        $scope.preffArr.REFERENCE_TRAN_FLAG = " ";
        $scope.preffArr.BRANCH_BCMAP_FLAG = " ";
        $scope.preffArr.BRANCH_SCMAP_FLAG = " ";
        $scope.preffArr.STOCK_GENERIC_FLAG = "";
        $scope.preffArr.TABLE_NOT_UNPOST_FLAG = "";
        $scope.preffArr.GL_FLAG = "";
        $scope.preffArr.SL_FLAG = "";
        $scope.preffArr.AUTO_LEDGER_FLAG = " ";
        $scope.preffArr.AUTO_CUTOMER_FLAG = " ";
        $scope.preffArr.AUTO_SUPPLIER_FLAG = " ";
        $scope.preffArr.AUTO_ITEM_FLAG = " ";
        $scope.preffArr.BUDGET_CONTROL_FLAG = "";
        $scope.preffArr.COST_BUDGET_INFO_FLAG = "";
        $scope.preffArr.COA_BUDGET_CONTROL_FLAG = "";
        $scope.preffArr.COA_BUDGET_INFO_FLAG = "";
        $scope.preffArr.USER_TARGET_CONTROL_FLAG = " ";
        $scope.preffArr.TRANS_ALERT_CAPTURE_FLAG = " ";
        $scope.preffArr.PARTY_ACTIVE_INFO_FLAG = "";
        $scope.preffArr.LEDGER_VAT_FLAG = "";
        $scope.preffArr.COST_CATEGORY_FLAG = "";
        $scope.preffArr.SUBLEDGER_ORDER_FLAG = "";
        $scope.preffArr.AUTO_GENERATE_LS_FLAG = " ";
        $scope.preffArr.TB_SUB_LEDGER_LOAD = " ";
        $scope.preffArr.IND_VOUCHER_VERIFY = " ";
        $scope.preffArr.PRINT_DATE_TIME = " ";
        $scope.preffArr.PP_DETAIL_ENTRY = "";
        $scope.preffArr.DOCUMENT_HISTORY_FLAG = "";
        $scope.preffArr.DIVISION_FLAG = "";
        $scope.preffArr.BILL_QTY_FLAG = "";
        $scope.preffArr.EXCHANGE_RATE_EDITABLE_FLAG = " ";
        $scope.preffArr.ORDER_TAXABLE_RATE = ""
        //
        $scope.preffArr.VNO_AS_SESSION_ID_CONTROL = "";
        $scope.preffArr.DEALER_SYSTEM_FLAG = "";
        $scope.preffArr.DEFAULT_DIVISION = "";
        $scope.preffArr.DEFAULT_BRANCH_CONSOLE = " ";
        $scope.preffArr.PUR_RATE_VERIANCE_FLAG = " ";
        $scope.preffArr.CASH_BANK_NEGATIVE_FLAG = " ";
        $scope.preffArr.SALES_CUPON_SCHEME_FLAG = " ";
        $scope.preffArr.AFP_BUDGET_CONTROL_FLAG = "";
        $scope.preffArr.MASTER_FIELD_MANDATORY_FLAG = "";
        $scope.preffArr.PAN_VAT_CONTROL_FLAG = "";
        $scope.preffArr.SYNC_WITH_IRD = "";
        $scope.preffArr.LS_BACK_ACCESS_DAYS = " ";
        $scope.preffArr.RATE_SCHEDULE_TYPE = " ";
        $scope.preffArr.S_BACK_ACCESS_DAYS = "";
        $scope.preffArr.GB_FLAG = "";
        $scope.preffArr.FREIGHT_RATE_FLAG = "";
    }

    var documentSetupOptionsUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getPreferenceDocumentSetupOptions";

    $http.get(documentSetupOptionsUrl)
        .then(function (response) {
            debugger
            $scope.documentSetupOptions = response.data.DATA;
        })
        .catch(function (error) {
            $scope.documentSetupOptions = [];
        });

    var linkDatabaseUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getDatabaseList";

    $scope.linkDatabaseComboDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: linkDatabaseUrl,
                dataType: "json"
            }
        },
        schema: {
            data: "DATA"
        }
    });

});