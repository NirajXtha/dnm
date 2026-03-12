DTModule.controller('supplierSetupTreeCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter, $timeout) {

    $scope.saveupdatebtn = "Save";
    $scope.supplierArr;
    $scope.savegroup = false;
    $scope.editFlag = "N";
    $scope.treenodeselected = "N";
    $scope.newrootinsertFlag = "Y";
    $scope.phoneNumbr = /^\+?(\d+(\.(\d+)?)?|\.\d+)$/;
    $scope.accountcode = "";
    $scope.editcode = "";
    $scope.edesc = "";
    $scope.updateSuppliercode = "";
    $scope.AE = true;
    $scope.image = "";
    $scope.treeSelectedSupplierCode = "";
    $scope.treeSelectedMasterSupplierCode = "";
    $scope.treeSelectedMasterPartyTypeCode = "";
    $scope.treeSelectedMasterAccCode = "";
    $scope.imageurledit = "";
    $scope.suppliersetup =
    {
        SHORTCUT: "",
        SUPPLIER_CODE: "",
        PARENT_SUPPLIER_CODE: "",
        SUPPLIER_EDESC: "",
        SUPPLIER_NDESC: "",
        REGD_OFFICE_EADDRESS: "",
        REGD_OFFICE_NADDRESS: "",
        TEL_MOBILE_NO1: "",
        TEL_MOBILE_NO2: "",
        FAX_NO: "",
        EMAIL: "",
        PARTY_TYPE_CODE: "",
        LINK_SUB_CODE: "",
        REMARKS: "",
        ACTIVE_FLAG: "",
        GROUP_SKU_FLAG: "",
        MASTER_SUPPLIER_CODE: "",
        PRE_SUPPLIER_CODE: "",
        COMPANY_CODE: "",
        CREATED_BY: "",
        CREATED_DATE: "",
        DELETED_FLAG: "",
        CREDIT_DAYS: "",
        CURRENT_BALANCE: "",
        CREDIT_ACTION_FLAG: "",
        ACC_CODE: "",
        PR_CODE: "",
        TPIN_VAT_NO: "",
        SYN_ROWID: "",
        DELTA_FLAG: "",
        CREDIT_LIMIT: "",
        MODIFY_DATE: "",
        BRANCH_CODE: "",
        MODIFY_BY: "",
        OPENING_DATE: "",
        M_DAYS: "",
        APPROVED_FLAG: "",
        SUBSTITUTE_NAME: "",
        MATURITY_DATE: "",
        IMAGE_FILE_NAME: "",
        INTEREST_RATE: "",
        CASH_SUPPLIER_FLAG: "",
        SUPPLIER_ID: "",
        GROUP_START_NO: "",
        PREFIX_TEXT: "",
        EXCISE_NO: "",
        PREFIX: "",
        STARTID: "",
        TIN: "",
        ALERT_DAYS: "",
        TDS_CODE: ""
    }
    // Group Supplier modal: Party Type dropdown and Accounts readonly display
    $scope.groupSupplierAccountEdesc = "";
    $scope.groupSupplierAcc_Code = "";
    // Use SetupApi endpoint which requires a 'filter' param (even if empty)
    var groupPartyTypeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getAllPartyTypes";
    $scope.groupSupplierPartyTypeDataSource = new kendo.data.DataSource({
        serverFiltering: true,
        transport: {
            read: { url: groupPartyTypeUrl },
            parameterMap: function (data) {
                var filterValue = "";
                if (data && data.filter && data.filter.filters && data.filter.filters[0]) {
                    filterValue = data.filter.filters[0].value || "";
                }
                return { filter: filterValue };
            }
        },
        schema: {
            data: function (resp) {
                // Support various response shapes
                return Array.isArray(resp) ? resp : (resp.DATA || resp.Data || resp.data || []);
            }
        }
    });
    $scope.groupSupplierPartyTypeOptions = {
        dataSource: $scope.groupSupplierPartyTypeDataSource,
        optionLabel: "-Select Party Type-",
        dataTextField: "PARTY_TYPE_EDESC",
        dataValueField: "PARTY_TYPE_CODE"
    };

    $scope.onChangeGroupSupplierPartyType = function (e) {
        debugger;
        try {
            var item = e.sender.dataItem();
            if (!item) return;
            $scope.$evalAsync(function () {
                $scope.supplierArr.PARTY_TYPE_CODE = item.PARTY_TYPE_CODE;
                $scope.supplierArr.PARTY_TYPE_EDESC = item.PARTY_TYPE_EDESC;
                if (item.ACC_CODE) {
                    $scope.supplierArr.ACC_CODE = item.ACC_CODE;
                    var accUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getAccountDetailsByAccCode?accCode=" + encodeURIComponent(item.ACC_CODE);
                    $http.get(accUrl).then(function (resp) {
                        var data = resp && resp.data ? (resp.data.DATA || resp.data.Data || resp.data) : null;
                        var row = Array.isArray(data) ? (data.length > 0 ? data[0] : null) : data;
                        $scope.groupSupplierAccountEdesc = row && row.ACC_EDESC ? row.ACC_EDESC : (item.ACC_CODE || "");
                        $scope.groupSupplierAcc_Code = row && row.ACC_CODE ? row.ACC_CODE : "";
                    }, function () {
                        $scope.groupSupplierAccountEdesc = item.ACC_CODE || "";
                    });
                } else {
                    $scope.supplierArr.ACC_CODE = '';
                    $scope.groupSupplierAccountEdesc = '';
                }
            });
        } catch (ex) { }
    };

    $scope.isOpenGroupsupplierModal = false;
    // Initialize Party Type and Accounts when group modal opens
    $(document).on('shown.bs.modal', '#groupsupplierModal', function () {
        //   debugger;
        $scope.$evalAsync(function () {
            try {
                //   debugger;
                $scope.isOpenGroupsupplierModal = true;
                var code = $scope.supplierArr && $scope.supplierArr.PARTY_TYPE_CODE;
                var ddl = $("#groupSupplierPartyType").data("kendoDropDownList");
                if (ddl) {
                    var applyValue = function () { if (code) ddl.value(code + ''); };
                    var hasData = (ddl.dataSource.view() || []).length > 0;
                    if (!hasData) { ddl.one('dataBound', function () { $scope.$evalAsync(applyValue); }); ddl.dataSource.read(); }
                    else { $scope.$evalAsync(applyValue); }
                }
                var accCode = $scope.supplierArr && $scope.supplierArr.ACC_CODE;
                if (accCode) {
                    var accUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getAccountDetailsByAccCode?accCode=" + encodeURIComponent(accCode);
                    $http.get(accUrl).then(function (resp) {

                        //debugger;
                        var data = resp && resp.data ? (resp.data.DATA || resp.data.Data || resp.data) : null;
                        var row = Array.isArray(data) ? (data.length > 0 ? data[0] : null) : data;

                        $scope.groupSupplierAccountEdesc = row && row.ACC_EDESC ? row.ACC_EDESC : (accCode || "");

                    }, function () { $scope.groupSupplierAccountEdesc = accCode || ""; });
                } else { $scope.groupSupplierAccountEdesc = ''; }
            } catch (e) { }
        });
    });



    $(document).on('hidden.bs.modal', '#groupsupplierModal', function () {
        $scope.$evalAsync(function () {
            try {
                // modal closed flag
                $scope.isOpenGroupsupplierModal = false;

            } catch (e) { }
        });
    });


    $scope.supplierArr = $scope.suppliersetup;
    $scope.supplierpersonalinfo =
        [{
            OWNER_NAME: '',
            DESIGNATION: '',
            CONTACT_PERSON: '',
            ADDRESS: '',
            TEL_MOBILE_NO: '',
        }]
    $scope.supplierpinfo = $scope.supplierpersonalinfo;
    $scope.suppliersisterconcerns =
        [{
            SISTER_CONCERN_EDESC: '',
            REMARKS: '',
        }]
    $scope.suppliersconcerns = $scope.suppliersisterconcerns;
    $scope.supplierbudgetcenter =
        [{
            BUDGET_CODE: '',
            REMARKS: '',
        }]
    $scope.supplierbcenter = $scope.supplierbudgetcenter;
    $scope.suppliersupplyterms =
        [{
            ITEM_NAME: '',
            MAX_LEAD_TIME: '',
            MIN_LEAD_TIME: '',
            IDEAL_LEAD_TIME: '',
            MIN_ORDER_QTY: '',
            MAX_ORDER_QTY: '',
        }]
    $scope.suppliersterms = $scope.suppliersupplyterms;
    $scope.suppliersupplyotherinfo =
        [{
            FIELD_VALUE: '',
            FIEL_NAME: '',
            REMARKS: '',

        }]
    $scope.suppliersupplyoinfo = $scope.suppliersupplyotherinfo;
    $scope.supplierOtherTermsAndConditions = [{
        FIELD_VALUE: '',
        FIEL_NAME: '',
        REMARKS: '',

    }]
    $scope.suppliersupplyalternativelocinfo =
        [{
            OFFICE_EDESC: '',
            CONTACT_PERSON: '',
            ADDRESS: '',
            TEL_MOBILE_NO: '',
            FAX_NO: '',
            EMAIL: '',
            REMARKS: ''
        }]
    $scope.suppliersupplyalocinfo = $scope.suppliersupplyalternativelocinfo;
    $scope.supplieraccount =
    {
        CREDIT_LIMIT: '',
        CREDIT_DAYS: '',
        CURRENT_BALANCE: '',
        IS_ACTIVE_SUPPLIER: '',

    }
    $scope.supplieracc = $scope.supplieraccount;
    $scope.supplierinvoices =
        [{
            REFERENCE_NO: '',
            INVOICE_DATE: '',
            DUE_DATE: '',
            TRANSACTION_TYPE: '',
            BALANCE: '',
            IVOICE_REMARKS: '',

        }]
    $scope.supplieri = $scope.supplierinvoices;
    $scope.suppliertermsandconditions =
        [{
            COMMENTS: '',
            TERMS_AND_CONDITIONS: '',
            CONDITIONS_VALUE: '',
            CONDITIONS_REMARKS: '',

        }]
    $scope.suppliertandc = $scope.suppliertermsandconditions
    $scope.supplierStockStatus =
        [{
            ITEM_CODE: '',
            STOCK_DATE: '',
            QUANTITY: '',
            REMARKS: '',

        }]
    $scope.supplierdstatus = $scope.supplierStockStatus;
    $scope.budgetcentercount = '';
    var dataFillDefered = "";


    //$scope.Binding = function () {

    //    $('.k-list').slimScroll({
    //        height: '250px'
    //    });
    //};



    $scope.add_Personal_info = function (index) {

        for (var i = 0; i <= $scope.supplierpinfo.length - 1; i++) {

            if ($(".OWNER_" + i).parent().hasClass("borderRed")) {
                displayPopupNotification("Same Name.", "warning");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
        }
        $scope.supplierpinfo.splice(index + 1, 0, {
            'OWNER_NAME': "",
            'DESIGNATION': "",
            'CONTACT_PERSON': "",
            'ADDRESS': "",
            'TEL_MOBILE_NO': "",
        });
        //$scope.PD = {};
    };
    $scope.remove_Personal_info = function (index) {


        if ($scope.supplierpinfo.length > 1) {

            $scope.supplierpinfo.splice(index, 1);

        }
    }

    $scope.add_sister_concern = function (index) {
        for (var i = 0; i <= $scope.suppliersconcerns.length - 1; i++) {

            if ($(".sconcernname_" + i).parent().hasClass("borderRed")) {
                displayPopupNotification("Same  Name.", "warning");
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
        }

        $scope.suppliersconcerns.splice(index + 1, 0, {
            'SISTER_CONCERN_EDESC': "",
            'REMARKS': "",
        });
    };
    $scope.remove_sister_concern = function (index) {


        if ($scope.suppliersconcerns.length > 1) {

            $scope.suppliersconcerns.splice(index, 1);

        }
    }

    $scope.add_budget_center = function (index) {
        debugger
        if ($scope.budgetcentercount === true) {
            displayPopupNotification("Same Code Or Budget Center cannot be selected", "warning");
            e.preventDefault();
            e.stopPropagation();
            return false;
        }
        $scope.supplierbcenter.splice(index + 1, 0, {
            'BUDGET_CODE': "",
            'REMARKS': "",

        });
    };
    $scope.remove_budget_center = function (index) {


        if ($scope.supplierbcenter.length > 1) {

            $scope.supplierbcenter.splice(index, 1);
            $scope.subledgercount = false;
        }
    }
    $scope.checkdupowner = function (key, index) {


        for (var i = 0; i <= $scope.supplierpinfo.length - 1; i++) {
            $(".OWNER_" + i).parent().removeClass("borderRed");
            $("#savedocumentformdata").prop("disabled", false);
        }

        for (var a = 0; a <= $scope.supplierpinfo.length - 1; a++) {
            for (var b = 0; b <= $scope.supplierpinfo.length - 1; b++) {
                if ($scope.supplierpinfo[a] != $scope.supplierpinfo[b]) {
                    if ($scope.supplierpinfo[a].OWNER_NAME === $scope.supplierpinfo[b].OWNER_NAME) {

                        $(".OWNER_" + b).parent().addClass("borderRed");
                        $("#savedocumentformdata").prop("disabled", true);
                    }
                }
            }

        }

    }
    $scope.checkdupsconcern = function (key, index) {


        for (var i = 0; i <= $scope.suppliersconcerns.length - 1; i++) {
            $(".sconcernname_" + i).parent().removeClass("borderRed");
            $("#savedocumentformdata").prop("disabled", false);
        }

        for (var a = 0; a <= $scope.suppliersconcerns.length - 1; a++) {
            for (var b = 0; b <= $scope.suppliersconcerns.length - 1; b++) {
                if ($scope.suppliersconcerns[a] != $scope.suppliersconcerns[b]) {
                    if ($scope.suppliersconcerns[a].SISTER_CONCERN_EDESC === $scope.suppliersconcerns[b].SISTER_CONCERN_EDESC) {

                        $(".sconcernname_" + b).parent().addClass("borderRed");
                        $("#savedocumentformdata").prop("disabled", true);
                    }
                }
            }

        }

    }
    $scope.add_supplier_supply_terms = function (supplyterm) {

        $scope.suppliersupplyterms.push({
            'ITEM_NAME': "",
            'MAX_LEAD_TIME': "",
            'MIN_LEAD_TIME': "",
            'IDEAL_LEAD_TIME': "",
            'MAX_ORDER_QTY': "",
            'MIN_ORDER_QTY': "",

        });
    };
    $scope.remove_supplier_supply_terms = function (index) {


        if ($scope.suppliersupplyterms.length > 1) {

            $scope.suppliersupplyterms.splice(index, 1);

        }
    }
    $scope.add_supplier_supply_terms = function (supplyterm) {

        $scope.suppliersupplyterms.push({
            'ITEM_NAME': "",
            'MAX_LEAD_TIME': "",
            'MIN_LEAD_TIME': "",
            'IDEAL_LEAD_TIME': "",
            'MAX_ORDER_QTY': "",
            'MIN_ORDER_QTY': "",

        });
    };
    $scope.remove_supplier_supply_terms = function (index) {


        if ($scope.suppliersupplyterms.length > 1) {

            $scope.suppliersupplyterms.splice(index, 1);

        }
    }
    $scope.add_supllier_supply_other_info = function (index) {

        $scope.suppliersupplyoinfo.splice(index + 1, 0, {
            'FIELD_NAME': "",
            'FIELD_VALUE': "",
            'REMARKS': "",
        });
    };
    $scope.remove_supllier_supply_other_info = function (index) {


        if ($scope.suppliersupplyoinfo.length > 1) {

            $scope.suppliersupplyoinfo.splice(index, 1);

        }
    }

    $scope.add_supplier_other_terms_and_conditions = function (index) {
        $scope.supplierOtherTermsAndConditions.splice(index + 1, 0, {
            'FIELD_NAME': "",
            'FIELD_VALUE': "",
            'REMARKS': "",
        });
    };
    $scope.remove_supplier_other_terms_and_conditions = function (index) {


        if ($scope.supplierOtherTermsAndConditions.length > 1) {

            $scope.supplierOtherTermsAndConditions.splice(index, 1);

        }
    }
    $scope.add_alternative_location = function (index) {

        $scope.suppliersupplyalternativelocinfo.splice(index + 1, 0, {
            'OFFICE_NAME': "",
            'CONTACT_PERSON': "",
            'ADDRESS': "",
            'TEL_MOBILE_NO': "",
            'EMAIL': "",
            'FAX_NO': "",
            'REMARKS': ""
        });
    };
    $scope.remove_alternative_location = function (index) {


        if ($scope.suppliersupplyalternativelocinfo.length > 1) {

            $scope.suppliersupplyalternativelocinfo.splice(index, 1);

        }
    }
    $scope.add_supplier_invoice = function (index) {
        $scope.supplieri.splice(index + 1, 0, {
            'REFERENCE_NO': "",
            'INVOICE_DATE': "",
            'DUE_DATE': "",
            'TRANSACTION_TYPE': "",
            'BALANCE_AMOUNT': "",
            'REMARKS': "",
        });
    };
    $scope.remove_supplier_invoice = function (index) {


        if ($scope.supplieri.length > 1) {

            $scope.supplieri.splice(index, 1);

        }
    }
    $scope.add_terms = function (term) {

        $scope.suppliertermsandconditions.push({
            'TERMS_AND_CONDITIONS': "",
            'CONDITIONS_VALUE': "",
            'CONDITIONS_REMARKS': "",
        });
    };
    $scope.remove_terms = function (index) {


        if ($scope.suppliertermsandconditions.length > 1) {

            $scope.suppliertermsandconditions.splice(index, 1);

        }
    }
    $scope.add_supplier_stock_status = function (index) {

        $scope.supplierStockStatus.splice(index + 1, 0, {
            'ITEM_CODE': "",
            'AS_ON': "",
            'STOCK_DATE': "",
            'REMARKS': "",
        })
    };
    $scope.remove_supplier_stock_status = function (index) {


        if ($scope.supplierStockStatus.length > 1) {

            $scope.supplierStockStatus.splice(index, 1);

        }
    }
    $scope.mastersupplierCodeDataSource = [
        { text: "/Root", value: "" }
    ];




    var supplierCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getsupplierCodeWithChild";

    $scope.supplierGroupDataSource = {
        transport: {
            read: {
                url: supplierCodeUrl,
            }
        }
    };
    var partyTypeCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetPartyType";

    $scope.partyTypeDataSource = {
        transport: {
            read: {
                url: partyTypeCodeUrl,
            }
        }
    };
    var partyRatingCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetPartyRating";

    $scope.partyRatingDataSource = {
        transport: {
            read: {
                url: partyRatingCodeUrl,
            }
        }
    };

    var budgetCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getBudgetCenterCodeWithChild";


    $scope.budgetGroupDataSource = {
        transport: {
            read: {
                url: budgetCodeUrl,
            }
        }
    };
    $scope.budgetGroupOptions = {
        dataSource: $scope.budgetGroupDataSource,
        optionLabel: "-Select Budget Code-",
        dataTextField: "BUDGET_EDESC",
        dataValueField: "BUDGET_CODE",
        databound: function (e) {

            let selectedValue = this.value();
            console.log(selectedValue);
        }
        //select: function (e) {
        //    
        //    angular.forEach($scope.supplierbcenter, function (value, index) {
        //        $scope.supplierbcenter[index].BUDGET_CODE = $(`budgetcentercode_${index}`).val()
        //    })
        //    var Code = e.dataItem.BUDGET_CODE;
        //    var key = this.element[0].attributes['budgetcenter-key'].value;
        //    var index = this.element[0].attributes['budgetcenter-index'].value;

        //var Code = e.dataItem.BUDGET_CODE;
        //var key = this.element[0].attributes['budgetcenter-key'].value;
        //var index = this.element[0].attributes['budgetcenter-index'].value;

        //$scope.supplierbcenter[index].BUDGET_CODE = Code;


        //var sublen = $scope.supplierbcenter.length;

        //for (var j = 0; j < sublen; j++) {
        //    var subcode = $scope.supplierbcenter[j].BUDGET_CODE;
        //    if (index != j) {
        //        if (subcode === Code) {

        //            $($(".budgetcentercode_" + index)[0]).addClass("borderRed");
        //            $("#savedocumentformdata").prop("disabled", true);
        //            $scope.budgetcentercount = true;
        //            return;

        //        }
        //        else {

        //            $scope.budgetcentercount = false;
        //            $("#savedocumentformdata").prop("disabled", false);
        //        };

        //    }


        //}
        //filter: "contains",
        //},
        //dataBound: function () {

        //    //$scope.Binding();
        //}
    }



    $scope.partytypeOptions = {
        dataSource: $scope.partyTypeDataSource,
        optionLabel: "-Select Party Type-",
        dataTextField: "PARTY_TYPE_EDESC",
        dataValueField: "PARTY_TYPE_CODE",
        change: function (e) {
            $scope.partyTypeCodeOnChange(e);
        },
        //filter: "contains",
    }
    $scope.partyTypeCodeOnChange = function (kendoEvent) {

        var currentItem = kendoEvent.sender.dataItem(kendoEvent.node);

        var tree = $("#accountmap").data("kendoDropDownList");
        tree.value(currentItem.ACC_CODE);

        // $scope.supplierArr.ACC_CODE = currentItem.ACC_CODE;
        $scope.suppliersetup.PARTY_TYPE_CODE = currentItem.PARTY_TYPE_CODE;
        //$("#accountcode").data("kendo").value(currentItem.ACC_CODE);


        //var accountCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetAccCodeByCode?acccode=" + currentItem.ACC_CODE;
        //$scope.accDSource = {
        //    transport: {
        //        read: {
        //            url: accountCodeUrl,
        //        }
        //    }
        //};


        //$scope.accountGOptions = {
        //    dataSource: $scope.accDSource,
        //    optionLabel: "-Select Account-",
        //    dataTextField: "ACC_EDESC",
        //    dataValueField: "ACC_CODE",
        //    change: function (e) {

        //    },
        //    dataBound: function () {

        //    }
        //}
    }
    var accountCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetAllAccountCodesupp";
    $scope.accDSource = {
        transport: {
            read: {
                url: accountCodeUrl,
            }
        }
    };


    $scope.accountGOptions = {
        dataSource: $scope.accDSource,
        optionLabel: "-Select Account-",
        dataTextField: "ACC_EDESC",
        dataValueField: "ACC_CODE",
        change: function (e) {

        },
        dataBound: function () {

        }
    }
    $scope.PartyRatingOptions = {
        dataSource: $scope.partyRatingDataSource,
        optionLabel: "-select  Rating-",
        dataTextField: "PR_EDESC",
        dataValueField: "PR_CODE",
        //filter: "contains",
    }
    $scope.MACDS = [];


    $scope.refresh = function () {

        //var tree = $("#suppliertree").data("kendoTreeView");
        //tree.dataSource.read();
        //if ($scope.suppliersetup.GROUP_SKU_FLAG !== "I") {
        //    var tree = $("#suppliertree").data("kendoTreeView");
        //    tree.dataSource.read();
        //}
        var grid = $("#kGrid").data("kendo-grid");
        if (grid != undefined) {
            grid.dataSource.read();
        }
        var ddl = $("#groupmastersuppliercode").data("kendoDropDownList");
        if (ddl != undefined)
            ddl.dataSource.read();
        if ($scope.groupsupplierTypeFlag == "Y") {
            $("#groupsupplierModal").modal("hide");
        }
        else {
            $("#supplierModal").modal("hide");
        }
        $scope.treenodeselected = 'Y';
        $scope.clearData();
        $scope.reset();
        $('#blah').attr('src', '');
        $($("#myTab").find("li a")[0]).trigger("click");

    }

    $scope.MACDSOptions = {
        dataSource: $scope.supplierGroupDataSource,
        optionLabel: "--\ ROOT",
        dataTextField: "supplier_EDESC",
        dataValueField: "MASTER_SUPPLIER_CODE",
        filter: "contains",
    }

    $scope.Bind = function () {

        $('#groupsuppliercode_listbox').slimScroll({
            height: '250px'
        });
    };

    var getSupplierByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetSuppliers";
    $scope.treeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: getSupplierByUrl,
                type: 'GET',
                data: function (data, evt) {
                }
            },

        },
        schema: {
            model: {
                id: "masterSupplierCode",
                parentId: "preSupplierCode",
                children: "Items",
                fields: {
                    SUPPLIER_CODE: { field: "supplierId", type: "string" },
                    SUPPLIER_EDESC: { field: "supplierName", type: "string" },
                    parentId: { field: "preSupplierCode", type: "string", defaultValue: "00" },
                }
            }
        }
    });

    function deepClone(obj) {
        return JSON.parse(JSON.stringify(obj));
    }

    function filterNodes(nodes, searchText) {
        const result = [];

        angular.forEach(nodes, function (node) {
            const children = node.Items ? (node.Items.data ? node.Items.data() : node.Items) : [];
            const filteredChildren = filterNodes(children, searchText);

            let isMatch = false;
            let nodeText = "";
            let newNode = {};

            newNode = deepClone(node);

            if (newNode.SUPPLIER_CODE && newNode.SUPPLIER_EDESC) {
                nodeText = (newNode.SUPPLIER_EDESC || "").toLowerCase();
                isMatch = nodeText.includes(searchText);
            } else {
                newNode.SUPPLIER_CODE = node.supplierId;
                newNode.SUPPLIER_EDESC = node.supplierName;
                nodeText = (newNode.supplierName || "").toLowerCase();
                isMatch = nodeText.includes(searchText);
            }

            if (isMatch || filteredChildren.length > 0) {
                newNode.Items = filteredChildren;
                result.push(newNode);
            }
        });

        return result;
    }

    $scope.filterTreeView = function () {
        const searchText = ($scope.txtMainGroupSearchString || '').toLowerCase();

        if (!searchText) {
            $scope.tree.setDataSource($scope.treeData);
            return;
        }

        const pristine = $scope.treeData._pristineData || $scope.treeData.data();
        const filteredData = filterNodes(pristine, searchText);

        const filteredDataSource = new kendo.data.HierarchicalDataSource({
            data: filteredData,
            schema: {
                model: {
                    id: "itemCode",
                    children: "Items"
                }
            }
        });

        $scope.tree.setDataSource(filteredDataSource);

        setTimeout(() => {
            $scope.tree.expand(".k-item");
        }, 100);
    };

    //treeview expand on startup
    $scope.supplieronDataBound = function () {
        // $('#suppliertree').data("kendoTreeView").expand('.k-supplier');
    }

    var accountCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAccountCodeWithChild";

    $scope.accountGroupDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: accountCodeUrl,
            }
        }
    });

    $scope.accountGroupOptions = {
        dataSource: $scope.accountGroupDataSource,
        optionLabel: "--select Account Type--",
        dataTextField: "ACC_EDESC",
        dataValueField: "ACC_CODE",
        change: function (e) {

            var currentItem = e.sender.dataItem(e.node);
            var acccode = currentItem.ACC_CODE;
            $scope.supplierArr.ACC_CODE = acccode;
            $scope.suppliersetup.ACC_CODE = acccode;
        },
        dataBound: function () {

            $scope.accountGroupDataSource;
        }
    }
    $scope.fillSupllierSetupForms = function (SupplierCode) {
        var getsupplierdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getsupplierDetailsBysupplierCode?SupplierCode=" + SupplierCode;
        $http({
            method: 'GET',
            url: getsupplierdetaisByUrl,

        }).then(function successCallback(response) {
            //setTimeout(function () {
            $scope.suppliersetup = response.data.DATA;
            $scope.supplierArr = $scope.suppliersetup ?? {};
            $scope.supplierTermsAndConditions = response.data?.DATA?.supplierTermsAndConditions;
            $scope.supplierStockStatus = response.data?.DATA?.supplierStockStatus;
            //$scope.supplierStockStatus = response.data.DATA.supplierStockStatus?.map(function (value) {
            //    return {
            //        ITEM_CODE: value.ITEM_CODE,
            //        QUANTITY: value.QUANTITY,
            //        STOCK_DATE: moment(value.STOCK_DATE).format("YYYY-MM-DD"),
            //        REMARKS: value.REMARKS
            //    }
            //}) ?? [];
            $scope.suppliersupplyoinfo = response.data?.DATA?.supplierOtherInfo;
            $scope.supplierOtherTermsAndConditions = response.data?.DATA?.supplierOtherTermsAndConditions;
            $scope.suppliersupplyalternativelocinfo = response.data?.DATA?.supplierAltLocationInfo;
            $scope.supplierpinfo = response.data?.DATA?.suplierContactmodelList;
            $scope.suppliersconcerns = response.data?.DATA?.supplierSisterConcernmodelList;
            let totalDays = (response.data?.DATA?.OPENING_DATE && response.data?.DATA?.MATURITY_DATE) ? moment(response.data?.DATA?.MATURITY_DATE).diff(moment(response.data?.DATA?.OPENING_DATE), 'days') : "";
            $("#totaldays").val(totalDays)
            $scope.initialBudgetCodes = [];
            debugger
            if (response.data?.DATA?.supplierBudgetCenterInfoList && response.data?.DATA?.supplierBudgetCenterInfoList.length > 0) {
                $scope.supplierbcenter = [];
                angular.forEach(response.data?.DATA?.supplierBudgetCenterInfoList, function (value, index) {
                    let bcenter = {
                        BUDGET_CODE: value.BUDGET_CODE,
                        REMARKS: value.REMARKS
                    }
                    $scope.supplierbcenter.push(bcenter);
                    $scope.initialBudgetCodes.push(value.BUDGET_CODE);
                })
            }
            else {
                $scope.supplierbcenter = [];
            }

            $scope.supplieri = response.data?.DATA?.supplierOpeningBalanceModelList;
            $scope.supplierBankMapping = response.data?.DATA?.supplierBankMapping;
            $scope.supplierItemMapping = response.data?.DATA?.supplierItemMapping;
            $scope.setSelectedAttributes();
            $scope.supplierArr.MASTER_SUPPLIER_CODE = $scope.suppliersetup?.MASTER_SUPPLIER_CODE;
            $scope.supplierArr.GROUP_SKU_FLAG = "G";
            $scope.supplierArr.SUPPLIER_CODE = $scope.suppliersetup?.SUPPLIER_CODE;
            $scope.imageurledit = window.location.protocol + "//" + window.location.host + "/Areas/NeoERP.DocumentTemplate/images/supplier/" + $scope.suppliersetup?.SUPPLIER_CODE + "/" + response.data?.DATA?.IMAGE_FILE_NAME;

            dataFillDefered.resolve(response);
            if ($scope.editFlag == "N") {
                $scope.newSupplierId();
            }
            else {
                var childItemParent = $("#mastersuppliercode").data("kendoDropDownList");
                childItemParent.value($scope.suppliersetup?.PARENT_SUPPLIER_CODE);
                $scope.supplierArr.SHORTCUT = $scope.supplierArr?.SUPPLIER_CODE;
            }


            //}, 100);






            //$scope.mastersupplierCodeDataSource = [];
            //$scope.masterItemCodeDataSource = [];
            //if ($scope.editFlag = "Y") {
            //    $scope.AE = false;
            //    $("#groupsuppliercode").data("kendoComboBox").value($scope.edesc);
            //    $scope.Bind();

            //}




            //if ($scope.editFlag == "Y") {
            //    $scope.mastersupplierCodeDataSource.push({  text: $scope.suppliersetup.SUPPLIER_EDESC, value: $scope.suppliersetup.MASTER_SUPPLIER_CODE});
            //}
            //else {
            //   $scope.mastersupplierCodeDataSource.push({ text: "/Root", value: "" });
            //}
            //var tree = $("#childmastersuppliercode").data("kendoDropDownList");
            //tree.setDataSource($scope.mastersupplierCodeDataSource);
            // this callback will be called asynchronously
            // when the response is available
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
    }

    $scope.supplieroptions = {
        loadOnDemand: false,
        select: function (e) {

            var currentsupplier = e.sender.dataItem(e.node);
            $('#supplierGrid').removeClass("show-displaygrid");
            $("#supplierGrid").html("");
            $($(this._current).parents('ul')[$(this._current).parents('ul').length - 1]).find('span').removeClass('hasTreeCustomMenu');
            $(this._current.context).find('span').addClass('hasTreeCustomMenu');
            $scope.suppliersetup.SUPPLIER_CODE = currentsupplier.SUPPLIER_CODE;
            //$scope.suppliersetup.SUPPLIER_EDESC = currentsupplier.SUPPLIER_EDESC;
            $scope.suppliersetup.MASTER_SUPPLIER_CODE = currentsupplier.masterSupplierCode;
            $scope.supplierArr.MASTER_SUPPLIER_CODE = $scope.suppliersetup.MASTER_SUPPLIER_CODE;
            $scope.editcode = $scope.suppliersetup.MASTER_SUPPLIER_CODE;
            //$scope.edesc = $scope.suppliersetup.SUPPLIER_EDESC;
            $scope.treenodeselected = "Y";
            $scope.newrootinsertFlag = "N";
            $scope.treeSelectedSupplierCode = currentsupplier.SUPPLIER_CODE;
            $scope.treeSelectedMasterSupplierCode = $scope.suppliersetup.MASTER_SUPPLIER_CODE;
            $scope.treeSelectedMasterPartyTypeCode = $scope.suppliersetup.PARTY_TYPE_CODE;
            $scope.treeSelectedMasterAccCode = $scope.suppliersetup.ACC_CODE;
            //$scope.MACDS = [];
            //$scope.MACDS.push({ text: $scope.suppliersetup.SUPPLIER_EDESC, value: $scope.suppliersetup.MASTER_SUPPLIER_CODE })
            //var tree = $("#childmastersuppliercode").data("kendoDropDownList");
            //tree.setDataSource($scope.MACDS);
            //$scope.movescrollbar();
        },

    };


    $scope.movescrollbar = function () {
        var element = $(".k-in");
        for (var i = 0; i < element.length; i++) {
            var selectnode = $(element[i]).hasClass("k-state-focused");
            if (selectnode) {
                $("#suppliertree").animate({
                    scrollTop: (parseInt(i))
                });
                break;
            }
        }
    }


    $scope.onContextSelect = function (event) {

        if ($scope.suppliersetup.supplier_CODE == "")
            return displayPopupNotification("Select supplier.", "error");;
        $scope.saveupdatebtn = "Save";
        if (event.item.innerText.trim() == "Delete") {

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

                        var delUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeletesuppliersetupBysuppliercode?suppliercode=" + $scope.suppliersetup.SUPPLIER_CODE;
                        $http({
                            method: 'POST',
                            url: delUrl
                        }).then(function successCallback(response) {

                            if (response.data.MESSAGE == "DELETED") {
                                $scope.supplierArr = [];
                                $("#supplierModal").modal("hide");
                                $scope.refresh();
                                var tree = $("#suppliertree").data("kendoTreeView");
                                tree.dataSource.read();
                                bootbox.hideAll();
                                displayPopupNotification("Data succesfully deleted ", "success");
                            }
                            if (response.data.MESSAGE == "HAS_CHILD") {

                                $scope.supplierArr = [];
                                //$scope.suppliersetup.MASTER_SUPPLIER_CODE = "";
                                $scope.editcode = "";
                                $scope.edesc = "";
                                $("#supplierModal").modal("hide");
                                bootbox.hideAll();

                                $scope.refresh();
                                displayPopupNotification("Cannot Delete", "warning");
                            }
                            // this callback will be called asynchronously
                            // when the response is available
                        }, function errorCallback(response) {
                            $scope.refresh();
                            displayPopupNotification(response.data.STATUS_CODE, "error");
                            // called asynchronously if an error occurs
                            // or server returns response with an error status.
                        });

                    }
                    else if (result == false) {

                        $scope.refresh();
                        $("#supplierModal").modal("hide");
                        bootbox.hideAll();
                    }

                }
            });
        }
        else if (event.item.innerText.trim() == "Update") {

            $scope.saveupdatebtn = "Update";
            $scope.editFlag = "Y";
            $scope.groupsupplierTypeFlag = "Y";
            dataFillDefered = $.Deferred();
            $scope.fillSupllierSetupForms($scope.treeSelectedSupplierCode);

            $.when(dataFillDefered).done(function () {

                var tree = $("#groupmastersuppliercode").data("kendoDropDownList");
                tree.value($scope.suppliersetup.PARENT_SUPPLIER_CODE);


                $("#groupsupplierModal").modal();
            });


        }
        else if (event.item.innerText.trim() == "Add") {
            $scope.editFlag = "N";
            $scope.groupsupplierTypeFlag = "Y"
            dataFillDefered = $.Deferred();
            $scope.fillSupllierSetupForms($scope.treeSelectedSupplierCode);



            $scope.savegroup = true;

            $.when(dataFillDefered).done(function () {
                $scope.supplierArr.GROUP_SKU_FLAG = "G";
                $scope.suppliersetup.GROUP_SKU_FLAG = "G";
                $scope.suppliersetup.SUPPLIER_EDESC = "";
                $scope.supplierArr.SUPPLIER_EDESC = "";
                $scope.supplierArr.SUPPLIER_NDESC = "";
                $scope.suppliersetup.SUPPLIER_NDESC = "";
                $scope.suppliersetup.PREFIX_TEXT = "";
                $scope.supplierArr.PREFIX_TEXT = "";
                $scope.suppliersetup.GROUP_START_NO = "";
                $scope.supplierArr.REMARKS = "";


                var tree = $("#groupmastersuppliercode").data("kendoDropDownList");
                //tree.value($scope.suppliersetup.SUPPLIER_CODE);
                tree.value($scope.treeSelectedSupplierCode);

                $("#groupsupplierModal").modal();
            });



        }

    }
    $scope.saveNewsupplier = function (isValid) {
        debugger
        //if (!isValid) {
        //    displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
        //    return;
        //}
        if ($scope.suppliersetup.GROUP_SKU_FLAG == 'I' || $scope.supplierArr.GROUP_SKU_FLAG == 'I') {
            var validation = [
                { supplierengname: $scope.suppliersetupform.supplierengname.$invalid },
                { regofficeengname: $scope.suppliersetupform.regofficeengname.$invalid },
                { vatno: $scope.suppliersetupform.vatno.$invalid },
            ]

            if (validation[0].supplierengname == true) {
                displayPopupNotification("Enter english name.", "warning");
                return
            }
            if (validation[1].regofficeengname == true) {
                displayPopupNotification("Enter office address details.", "warning");
                return
            }

            if (validation[2].vatno == true) {
                displayPopupNotification("PAN number should be 9 letter only.", "warning");
                return
            }
        }
        else {
            var validation = [
                { groupsupplierengname: $scope.groupsuppliersetupform.groupsupplierengname.$invalid },
            ]
            if (validation[0].groupsupplierengname == true) {

                displayPopupNotification("Enter english name.", "warning");
                return
            }
        }
        var mastersuppliervalue = $scope.suppliersetup.MASTER_SUPPLIER_CODE;
        //return;
        if ($scope.saveupdatebtn == "Save") {

            //return;
            var createUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewsupplier";
            for (var a = 0; a <= $scope.supplierpinfo.length - 1; a++) {
                for (var b = 0; b <= $scope.supplierpinfo.length - 1; b++) {
                    if ($scope.supplierpinfo[a] != $scope.supplierpinfo[b]) {
                        if ($scope.supplierpinfo[a].OWNER_NAME === $scope.supplierpinfo[b].OWNER_NAME) {
                            $($("#myTab").find("li a")[1]).trigger("click");
                            displayPopupNotification("Validation issue on owner name.", "warning");
                            e.preventDefault();
                            e.stopPropagation();
                            //$('#myTab a[href="#tab-1"]').tab('show');
                            return false;
                        }
                    }
                }
            }
            for (var a = 0; a <= $scope.suppliersconcerns.length - 1; a++) {
                for (var b = 0; b <= $scope.suppliersconcerns.length - 1; b++) {
                    if ($scope.suppliersconcerns[a] != $scope.suppliersconcerns[b]) {
                        if ($scope.suppliersconcerns[a].SISTER_CONCERN_EDESC === $scope.suppliersconcerns[b].SISTER_CONCERN_EDESC) {
                            $($("#myTab").find("li a")[1]).trigger("click");
                            displayPopupNotification("Validation issue on sister concern.", "warning");
                            e.preventDefault();
                            e.stopPropagation();
                            //$('#myTab a[href="#tab-1"]').tab('show');
                            return false;
                        }
                    }
                }
            }
            for (var a = 0; a <= $scope.supplierbcenter.length - 1; a++) {
                for (var b = 0; b <= $scope.supplierbcenter.length - 1; b++) {
                    if ($scope.supplierbcenter[a] != $scope.supplierbcenter[b]) {
                        if ($scope.supplierbcenter[a].BUDGET_CODE === $scope.supplierbcenter[b].BUDGET_CODE) {
                            $($("#myTab").find("li a")[1]).trigger("click");
                            displayPopupNotification("Validation issue on budget center name", "warning");
                            e.preventDefault();
                            e.stopPropagation();
                            //$('#myTab a[href="#tab-1"]').tab('show');
                            return false;
                        }
                    }
                }
            }
            if ($scope.groupsupplierTypeFlag == "N") {
                document.uploadFile();
                if ($('#txtFile')[0].files[0] !== undefined) { $scope.supplierArr.IMAGE_FILE_NAME = $('#txtFile')[0].files[0].name; }
            }
            // For main modal, if controls exist, use their values; otherwise keep values set via group modal
            var partyDdl = $("#partytypecode").data("kendoDropDownList");
            if (partyDdl && partyDdl.value()) { $scope.supplierArr.PARTY_TYPE_CODE = partyDdl.value(); }
            var accDdl = $("#accountmap").data("kendoDropDownList");
            if (accDdl && accDdl.value()) { $scope.supplierArr.ACC_CODE = accDdl.value(); }
            $scope.supplierArr.suplierContactmodelList = $scope.supplierpinfo;
            $scope.supplierArr.supplierTermsAndConditions = $scope.supplierTermsAndConditions;
            $scope.supplierArr.supplierStockStatus = $scope.supplierStockStatus;
            $scope.supplierArr.supplierAltLocationInfo = $scope.suppliersupplyalternativelocinfo;
            $scope.supplierArr.supplierOtherInfo = $scope.suppliersupplyoinfo;
            $scope.supplierArr.supplierOtherTermsAndConditions = $scope.supplierOtherTermsAndConditions;
            $scope.supplierArr.supplierSisterConcernmodelList = $scope.suppliersconcerns;
            $scope.supplierArr.supplierBudgetCenterInfoList = $scope.supplierbcenter;

            $scope.supplierArr.ACC_CODE = $scope.groupSupplierAcc_Code;

            $scope.supplierArr.OPENING_DATE = $("#englishdatedocument5").val();
            $scope.supplierArr.MATURITY_DATE = $("#englishdatedocument6").val();
            $scope.supplierArr.supplierOpeningBalanceModelList = $scope.supplieri;
            $scope.supplierArr.supplierItemMapping = $scope.supplierItemMapping;
            $scope.supplierArr.supplierBankMapping = $scope.supplierBankMapping;
            var suplierSetupModalSet = { suplierSetupModel: $scope.supplierArr };
            $http({
                method: 'POST',
                url: createUrl,
                data: suplierSetupModalSet

            }).then(function successCallback(response) {

                if (response.data.MESSAGE == "INSERTED") {
                    //uploadFile();

                    $scope.supplierArr = [];
                    if ($scope.suppliersetup.GROUP_SKU_FLAG !== "I") {
                        var tree = $("#suppliertree").data("kendoTreeView");
                        tree.dataSource.read();
                    }

                    var grid = $("#kGrid").data("kendo-grid");
                    if (grid != undefined) {
                        grid.dataSource.read();
                    }
                    //var ddl = $("#groupmastersuppliercode").data("kendoDropDownList");
                    var ddl = $("#mastersuppliercode").data("kendoDropDownList");
                    if (ddl != undefined)
                        ddl.dataSource.read();

                    var ddl1 = $("#partytypecode").data("kendoDropDownList");
                    if (ddl1 != undefined)
                        ddl1.dataSource.read();

                    //$scope.supplierpinfo = [];
                    //$scope.suppliersconcerns = [];
                    //$scope.supplierbcenter = [];
                    //$scope.supplieri = [];
                    //$scope.suppliersetup.MASTER_SUPPLIER_CODE = "";
                    $scope.refresh();

                    //$("#groupsupplierModal").modal("toggle");
                    displayPopupNotification("Data succesfully saved ", "success");
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Supplier name already exist please try another Supplier name.", "error");
                }
                else {

                    $scope.refresh();
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // this callback will be called asynchronously
                // when the response is available
            }, function errorCallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Supplier name already exist please try another Supplier name.", "error");
                }
                else {
                    $scope.refresh();
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                    // called asynchronously if an error occurs
                    // or server returns response with an error status.
                }
            });
        }
        else {

            var updateUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updatesupplierbysupplierCode";
            $scope.supplierArr.suplierContactmodelList = $scope.supplierpinfo;
            $scope.supplierArr.supplierTermsAndConditions = $scope.supplierTermsAndConditions;
            $scope.supplierArr.supplierStockStatus = $scope.supplierStockStatus;
            $scope.supplierArr.supplierAltLocationInfo = $scope.suppliersupplyalternativelocinfo;
            $scope.supplierArr.supplierOtherInfo = $scope.suppliersupplyoinfo;
            $scope.supplierArr.supplierOtherTermsAndConditions = $scope.supplierOtherTermsAndConditions;
            $scope.supplierArr.supplierSisterConcernmodelList = $scope.suppliersconcerns;
            $scope.supplierArr.supplierBudgetCenterInfoList = $scope.supplierbcenter;
            $scope.supplierArr.supplierItemMapping = $scope.supplierItemMapping;
            $scope.supplierArr.supplierBankMapping = $scope.supplierBankMapping;
            $scope.supplierArr.OPENING_DATE = $("#englishdatedocument5").val();
            $scope.supplierArr.MATURITY_DATE = $("#englishdatedocument6").val();

            angular.forEach($scope.supplierArr.supplierBudgetCenterInfoList, function (value, index) {
                value.BUDGET_CODE = value.BUDGET_CODE ? value.BUDGET_CODE : $scope.initialBudgetCodes[index];
            })
            $scope.supplierArr.supplierOpeningBalanceModelList = $scope.supplieri;

            $scope.supplierArr.ACC_CODE = $("#accountmap").data("kendoDropDownList").value();

            if ($scope.isOpenGroupsupplierModal) {
                $scope.supplierArr.ACC_CODE = $scope.groupSupplierAcc_Code;
            }

            var suplierSetupModalSet = { suplierSetupModel: $scope.supplierArr };
            $scope.saveupdatebtn = "Update";
            if ($scope.groupsupplierTypeFlag == "N") {
                document.uploadFile();
                if ($('#txtFile')[0].files[0] !== undefined) { $scope.supplierArr.IMAGE_FILE_NAME = $('#txtFile')[0].files[0].name; }
                else { $scope.supplierArr.IMAGE_FILE_NAME = $scope.supplierArr.IMAGE_FILE_NAME; }
            }
            $http({
                method: 'POST',
                url: updateUrl,
                data: suplierSetupModalSet
            }).then(function successCallback(response) {

                if (response.data.MESSAGE == "UPDATED") {
                    $scope.supplierArr = [];
                    //$scope.supplierpinfo = [];
                    //$scope.suppliersconcerns = [];
                    //$scope.supplierbcenter = [];
                    //$scope.supplieri = [];
                    if ($scope.suppliersetup.GROUP_SKU_FLAG !== "I") {
                        var tree = $("#suppliertree").data("kendoTreeView");
                        tree.dataSource.read();
                        $scope.suppliersetup.MASTER_SUPPLIER_CODE = "";
                    }
                    $scope.refresh();
                    displayPopupNotification("Data succesfully updated ", "success");
                }
                if (response.data.MESSAGE == "ERROR") {
                    $scope.refresh();
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // this callback will be called asynchronously
                // when the response is available
            }, function errorCallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Supplier name already exist please try another Supplier name.", "error");
                }
                else {
                    $scope.refresh();
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                    // called asynchronously if an error occurs
                    // or server returns response with an error status.
                }
            });
        }
    }


    $scope.MACDSOptions = {
        dataSource: $scope.MACDS,
        dataTextField: "text",
        dataValueField: "value",
    };

    $scope.supplierGroupOptions = {
        optionLabel: "<PRIMARY>",
        dataSource: $scope.supplierGroupDataSource,
        dataTextField: "SUPPLIER_EDESC",
        dataValueField: "SUPPLIER_CODE",
        select: function (e) {

            $rootScope.quickmastersuppliercode = e.dataItem.MASTER_SUPPLIER_CODE;
        },

        dataBound: function () {

            $scope.supplierGroupDataSource;
        }

    };
    $scope.clearData = function () {

        $scope.supplierArr =
        {
            SHORTCUT: "",
            SUPPLIER_CODE: "",
            SUPPLIER_EDESC: "",
            SUPPLIER_NDESC: "",
            REGD_OFFICE_EADDRESS: "",
            REGD_OFFICE_NADDRESS: "",
            TEL_MOBILE_NO1: "",
            TEL_MOBILE_NO2: "",
            FAX_NO: "",
            EMAIL: "",
            PARTY_TYPE_CODE: "",
            LINK_SUB_CODE: "",
            REMARKS: "",
            ACTIVE_FLAG: "",
            GROUP_SKU_FLAG: "",
            MASTER_SUPPLIER_CODE: "",
            PRE_SUPPLIER_CODE: "",
            COMPANY_CODE: "",
            CREATED_BY: "",
            CREATED_DATE: "",
            DELETED_FLAG: "",
            CREDIT_DAYS: "",
            CURRENT_BALANCE: "",
            CREDIT_ACTION_FLAG: "",
            //ACC_CODE: "",
            PR_CODE: "",
            TPIN_VAT_NO: "",
            SYN_ROWID: "",
            DELTA_FLAG: "",
            CREDIT_LIMIT: "",
            MODIFY_DATE: "",
            BRANCH_CODE: "",
            MODIFY_BY: "",
            OPENING_DATE: "",
            M_DAYS: "",
            APPROVED_FLAG: "",
            SUBSTITUTE_NAME: "",
            MATURITY_DATE: "",
            IMAGE_FILE_NAME: "",
            INTEREST_RATE: "",
            CASH_SUPPLIER_FLAG: "",
            SUPPLIER_ID: "",
            GROUP_START_NO: "",
            PREFIX_TEXT: "",
            EXCISE_NO: "",
            PREFIX: "",
            STARTID: "",
            TIN: "",
            ALERT_DAYS: "",
            TDS_CODE: "",
        }
        $('#masteraccountcode').val("");
        $('#nepaliDate5').val("");
        $('#nepaliDate6').val("");
    }

    $scope.BindGrid = function (groupId) {

        $(".topsearch").show();
        var url = null;
        if (groupId == "All") {
            if ($('#suptxtSearchString').val() == null || $('#suptxtSearchString').val() == '' || $('#suptxtSearchString').val() == undefined || $('#suptxtSearchString').val() == 'undefined') {
                alert('Input is empty or undefined.');
                return;
            }
            url = "/api/SetupApi/GetAllSupplyList?searchtext=" + $('#suptxtSearchString').val();
        }
        else {
            $("#suptxtSearchString").val('');
            url = "/api/SetupApi/GetChildOfsupplierByGroup?groupCode=" + groupId;
        }

        $timeout(function () {
            var $input = $("#suptxtSearchString");
            if ($input.data("bindGlobalSearch")) return;
            $input.data("bindGlobalSearch", true);

            function debounce(fn, delay) {
                var t;
                return function () {
                    var ctx = this, args = arguments;
                    clearTimeout(t);
                    t = setTimeout(function () { fn.apply(ctx, args); }, delay);
                };
            }

            var triggerGlobal = function () {
                var val = $input.val();
                if (val && val.toString().trim().length > 0) {
                    $scope.$evalAsync(function () { $scope.BindGrid('All'); });
                }
            };

            $input.on('keydown.customerGlobal', function (e) {
                if (e.key === 'Enter' || e.keyCode === 13) {
                    e.preventDefault();
                    triggerGlobal();
                }
            });

            $input.on('input.customerGlobal', debounce(triggerGlobal, 400));
        }, 0);

        //grid Bind and sorting and paging Prem Prakash Dhakal
        var reportConfig = GetReportSetting("_supplierSetupPartial");
        $scope.supplierChildGridOptions = {
            dataSource: {
                transport: {
                    read: {
                        url: url,
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        type: "GET"
                    },
                    //parameterMap: function (options, type) {
                    //    var paramMap = JSON.stringify($.extend(options, ReportFilter.filterAdditionalData()));
                    //    delete paramMap.$inlinecount;
                    //    delete paramMap.$format;
                    //    return paramMap;
                    //}
                },
                error: function (e) {
                    displayPopupNotification("Sorry error occured while processing data", "error");
                },
                model: {
                    fields: {
                        SUPPLIER_CODE: { type: "string" },
                        SUPPLIER_EDESC: { type: "string" },
                        REGD_OFFICE_EADDRESS: { type: "string" },
                        TEL_MOBILE_NO1: { type: "string" },
                        EMAIL: { type: "string" },
                        TPIN_VAT_NO: { type: "string" },
                    }
                },
                /*pageSize: reportConfig.defaultPageSize,*/
                pageSize: 50,
            },
            height: 500,
            sortable: true,
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
                    SaveReportSetting('_supplierSetupPartial', 'kGrid');
            },
            columnHide: function (e) {
                if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                    SaveReportSetting('_supplierSetupPartial', 'kGrid');
            },
            pageable: {
                refresh: true,
                //pageSizes: reportConfig.itemPerPage,
                pageSize: 50,
                buttonCount: 5
            },
            pageable: true,
            dataBound: function (o) {
                var grid = o.sender;
                if (grid.dataSource.total() == 0) {
                    var colCount = grid.columns.length;
                    $(o.sender.wrapper)
                        .find('tbody')
                        .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, No data found</td></tr>');
                    /*displayPopupNotification("No Data Found.", "info");*/
                }
                else {
                    var g = $("#kGrid").data("kendoGrid");
                    for (var i = 0; i < g.columns.length; i++) {
                        g.showColumn(i);
                    }
                    $("div.k-group-indicator").each(function (i, v) {
                        g.hideColumn($(v).data("field"));
                    });
                }
                UpdateReportUsingSetting("_supplierSetupPartial", "kGrid");
                $('div').removeClass('.k-header k-grid-toolbar');
            },
            columns: [
                {
                    //hidden: true,
                    field: "SUPPLIER_CODE",
                    title: "CODE",
                    width: "80px"
                },
                {
                    field: "SUPPLIER_EDESC",
                    title: "NAME",
                    width: "120px"
                },
                {
                    field: "REGD_OFFICE_EADDRESS",
                    title: "OFFICE",
                    width: "120px"
                },
                {
                    field: "TEL_MOBILE_NO1",
                    title: "TEL1",
                    width: "80px"
                },
                {
                    field: "EMAIL",
                    title: "EMAIL",
                    width: "120px"
                },
                {
                    field: "TPIN_VAT_NO",
                    title: "TPIN VAT/",
                    width: "60px"
                },
                {
                    title: "Action ",
                    template: '<a class="fa fa-pencil-square-o editAction" title="Edit" ng-click="edit(dataItem.SUPPLIER_CODE)"><span class="sr-only"></span> </a><a class="fa fa-trash deleteAction" title="delete" ng-click="delete(dataItem.SUPPLIER_CODE)"><span class="sr-only"></span> </a>',
                    width: "50px"
                }
            ],
        };



        //$scope.supplierChildGridOptions = {
        //    dataSource: {
        //        transport: {
        //            read: url,
        //            dataType: "json",
        //            contentType: "application/json; charset=utf-8",
        //            type: "GET"
        //        },
        //        pageSize: 50,
        //        //serverPaging: true,
        //        serverSorting: true
        //    },
        //    scrollable: true,

        //    height: 450,
        //    sortable: true,
        //    resizable: true,
        //    pageable: true,
        //    //resizable: true,
        //    dataBound: function (e) {
        //        $("#kGrid tbody tr").css("cursor", "pointer");
        //        DisplayNoResultsFound($('#kGrid'));
        //        $("#kGrid tbody tr").on('dblclick', function () {
        //            var accCode = $(this).find('td span').html()
        //            $scope.edit(accCode);
        //        })

        //    },
        //    columns: [
        //        {
        //            //hidden: true,

        //            field: "SUPPLIER_CODE",
        //            title: "CODE",
        //            width: "80px"

        //        },
        //        {
        //            field: "SUPPLIER_EDESC",
        //            title: "NAME",
        //            width: "120px"
        //        },
        //        {
        //            field: "REGD_OFFICE_EADDRESS",
        //            title: "OFFICE",
        //            width: "120px"
        //        },


        //        {
        //            field: "TEL_MOBILE_NO1",
        //            title: "TEL1",
        //            width: "80px"
        //        },


        //        {
        //            field: "EMAIL",
        //            title: "EMAIL",
        //            width: "120px"
        //        },
        //        {
        //            field: "TPIN_VAT_NO",
        //            title: "TPIN VAT/",
        //            width: "60px"
        //        },

        //        {
        //            title: "Action ",
        //            template: '<a class="fa fa-pencil-square-o editAction" title="Edit" ng-click="edit(dataItem.SUPPLIER_CODE)"><span class="sr-only"></span> </a><a class="fa fa-trash deleteAction" title="delete" ng-click="delete(dataItem.SUPPLIER_CODE)"><span class="sr-only"></span> </a>',

        //            width: "50px"
        //        }
        //    ],


        //};

        $scope.onsiteSearch = function ($this) {

            var q = $("#txtSearchString").val();
            var grid = $("#kGrid").data("kendo-grid");
            grid.dataSource.query({
                page: 1,
                pageSize: 50,
                filter: {
                    logic: "or",
                    filters: [
                        { field: "ORDER_NO", operator: "contains", value: q },
                        { field: "ORDER_DATE", operator: "contains", value: q },
                        { field: "CREATED_BY", operator: "contains", value: q }
                    ]
                }
            });
        }
    }

    $scope.delete = function (code) {
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

                    var delUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeletesuppliersetupBysuppliercode?suppliercode=" + code;
                    $http({
                        method: 'POST',
                        url: delUrl
                    }).then(function successCallback(response) {

                        if (response.data.MESSAGE == "DELETED") {
                            //$scope.supplierArr = [];
                            //$scope.supplierpinfo = [];
                            //$scope.suppliersconcerns = [];
                            //$scope.supplierbcenter = [];
                            //$scope.supplieri = [];
                            $scope.refresh();
                            var grid = $("#kGrid").data("kendo-grid");
                            if (grid != undefined) {
                                grid.dataSource.read();
                            }
                            bootbox.hideAll();
                            displayPopupNotification("Data succesfully deleted ", "success");
                        }
                        if (response.data.MESSAGE == "HAS_CHILD") {

                            //$scope.supplierArr = [];

                            $scope.suppliersetup.MASTER_SUPPLIER_CODE = "";
                            $scope.editcode = "";
                            $scope.edesc = "";
                            $("#supplierModal").modal("hide");
                            bootbox.hideAll();

                            $scope.refresh();
                            displayPopupNotification("Cannot Delete", "warning");
                        }
                        // this callback will be called asynchronously
                        // when the response is available
                    }, function errorCallback(response) {
                        $scope.refresh();
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                        // called asynchronously if an error occurs
                        // or server returns response with an error status.
                    });

                }
                else if (result == false) {

                    $scope.refresh();
                    $("#supplierModal").modal("hide");
                    bootbox.hideAll();
                }

            }
        });
    }
    $scope.reset = function () {


        $scope.supplierpinfo =
            [{
                OWNER_NAME: '',
                DESIGNATION: '',
                CONTACT_PERSON: '',
                ADDRESS: '',
                TEL_MOBILE_NO: '',
            }]

        $scope.suppliersconcerns =
            [{
                SISTER_CONCERN_EDESC: '',
                REMARKS: '',
            }]

        $scope.supplierbcenter =
            [{
                BUDGET_CODE: '',
                REMARKS: '',
            }]

        $scope.suppliersterms =
            [{
                ITEM_NAME: '',
                MAX_LEAD_TIME: '',
                MIN_LEAD_TIME: '',
                IDEAL_LEAD_TIME: '',
                MIN_ORDER_QTY: '',
                MAX_ORDER_QTY: '',
            }]

        $scope.suppliersupplyoinfo =
            [{
                FIELD_VALUE: '',
                FIELD_NAME: '',
                REMARKS: '',

            }]
        $scope.supplierOtherTermsAndConditions =
            [
                {
                    FIELD_VALUE: '',
                    FIELD_NAME: '',
                    REMARKS: '',
                }]

        $scope.suppliersupplyalocinfo =
            [{
                OFFICE_EDESC: '',
                CONTACT_PERSON: '',
                ADDRESS: '',
                TEL_MOBILE_NO: '',
                EMAIL: '',
                FAX_NO: '',
                REMARKS: '',
            }]

        $scope.supplieracc =
        {
            CREDIT_LIMIT: '',
            CREDIT_DAYS: '',
            CURRENT_BALANCE: '',
            IS_ACTIVE_SUPPLIER: '',

        }

        $scope.supplieri =
            [{
                REF_NO: '',
                INVOICE_DATE: '',
                DUE_DATE: '',
                TRANSACTION_TYPE: '',
                BALANCE_AMOUNT: '',
                REMARKS: '',

            }]

        $scope.suppliertandc =
            [{
                COMMENTS: '',
                TERMS_AND_CONDITIONS: '',
                CONDITIONS_VALUE: '',
                CONDITIONS_REMARKS: '',

            }]

        $scope.supplierdstatus =
            [{
                ITEM_NAME: '',
                AS_ON: '',
                QUANTITY: '',
                REMARKS: '',

            }]
    }

    $scope.newSupplierId = function () {
        var delUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getNewSupplierId";
        $http({
            method: 'GET',
            url: delUrl
        }).then(function successCallback(response) {

            if (response.data.MESSAGE == "OK") {

                $scope.suppliersetup.SHORTCUT = response.data.DATA;
                $scope.supplierArr.SHORTCUT = response.data.DATA;

            }

            // this callback will be called asynchronously
            // when the response is available
        }, function errorCallback(response) {
            displayPopupNotification(response.data.STATUS_CODE, "error");
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
    }

    $scope.showModalForNew = function (event) {
        debugger
        $scope.supplierArr = {};
        $scope.editFlag = "N";
        $scope.AE = true;
        //$scope.fillSupllierSetupForms($scope.treeSelectedMasterSupplierCode);
        $scope.saveupdatebtn = "Save"
        $scope.groupsupplierTypeFlag = "N";
        $scope.suppliersetup.GROUP_SKU_FLAG = "I";
        $scope.supplierArr.GROUP_SKU_FLAG = "I";
        $scope.supplierArr.IMAGE_FILE_NAME = "";
        $("a.fileinput-exists").trigger("click");
        $scope.supplierArr.SUPPLIER_CODE = $scope.suppliersetup.SUPPLIER_CODE;
        //$scope.supplierArr.MASTER_SUPPLIER_CODE = $scope.suppliersetup.MASTER_SUPPLIER_CODE;
        var dropdown = $("#mastersuppliercode").data("kendoDropDownList");
        dropdown.dataSource.read();
        $scope.supplierArr.MASTER_SUPPLIER_CODE = $scope.treeSelectedMasterSupplierCode;
        //var tree = $("#mastersuppliercode").data("kendoDropDownList");
        //tree.value($scope.suppliersetup.SUPPLIER_CODE);
        var tree = $("#mastersuppliercode").data("kendoDropDownList");
        tree.value($scope.treeSelectedSupplierCode);

        var masterCode = $scope.treeSelectedMasterSupplierCode;
        if (masterCode) {
            var mapUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetSupplierPartyTypeAndAccountByMasterCode?masterSupplierCode=" + encodeURIComponent(masterCode);
            $http.get(mapUrl).then(function (resp) {
                debugger
                var data = resp && resp.data ? (resp.data.DATA || resp.data.Data || resp.data.data || resp.data) : null;
                if (data) {
                    $scope.suppliersetup = $scope.suppliersetup || {};
                    $scope.suppliersetup.PARTY_TYPE_CODE = data.PARTY_TYPE_CODE || '';
                    $scope.suppliersetup.ACC_CODE = data.ACC_CODE || '';

                    var partyTypeDropDown = $("#partytypecode").data("kendoDropDownList");
                    partyTypeDropDown.value($scope.suppliersetup.PARTY_TYPE_CODE);

                    var accountMapDropDown = $("#accountmap").data("kendoDropDownList");
                    accountMapDropDown.value($scope.suppliersetup.ACC_CODE);
                }
            }, function (err) {
                console.warn('Failed to fetch supplier party/account map', err);
            });
        }

        $scope.newSupplierId();
        $($("#myTab").find("li a")[0]).trigger("click");
        $("#nepaliDate5").val("");
        $("#nepaliDate6").val("");
        $("#totaldays").val("");

        $scope.supplierpinfo = [];
        $scope.suppliersconcerns = [];
        $scope.supplierbcenter = [];
        $scope.supplierTermsAndConditions = [];
        $scope.suppliersupplyoinfo = [];
        $scope.suppliersupplyalternativelocinfo = [];
        $scope.supplieri = [];
        $scope.supplierOtherTermsAndConditions = [];
        $scope.supplierStockStatus = [];

        //$scope.supplierArr.CREATED_DATE = $filter('date')(new Date(), 'dd-MMM-yyyy');
        //$scope.supplierArr.MATURITY_DATE = $filter('date')(new Date(), 'dd-MMM-yyyy');
        //$scope.ConvertEngToNepang(moment($scope.supplierArr.CREATED_DATE).format('YYYY-MM-DD'), "englishdatedocument5");
        $scope.imageurledit = "";
        //$scope.ConvertEngToNepang(moment($scope.supplierArr.MATURITY_DATE).format('YYYY-MM-DD'), "englishdatedocument6");
        $("#supplierModal").modal("toggle");


    }
    $scope.edit = function (Suppliercode) {
        /*  */
        dataFillDefered = $.Deferred();
        $scope.editFlag = "Y";
        $scope.AE = false;
        $scope.saveupdatebtn = "Update"
        $scope.fillSupllierSetupForms(Suppliercode);
        $.when(dataFillDefered).done(function () {

            /* */
            var accd = $("#accountmap").data("kendoDropDownList");
            $scope.suppliersetup = $scope.suppliersetup ?? {};
            accd.value($scope.suppliersetup.ACC_CODE);
            var partytypeid = $("#partytypecode").data("kendoDropDownList");
            partytypeid.value($scope.suppliersetup.PARTY_TYPE_CODE);
            $scope.groupsupplierTypeFlag = "N";
            $scope.suppliersetup.GROUP_SKU_FLAG = "I";
            $scope.supplierArr.GROUP_SKU_FLAG = "I";
            $scope.savegroup = false;
            $scope.ConvertEngToNepang(moment($scope.supplierArr.CREATED_DATE).format('YYYY-MM-DD'), "englishdatedocument5");
            if ($scope.supplierArr.MATURITY_DATE != null) { $scope.ConvertEngToNepang(moment($scope.supplierArr.MATURITY_DATE).format('YYYY-MM-DD'), "englishdatedocument6"); }
            if ($scope.supplierArr.OPENING_DATE != null) { $scope.ConvertEngToNepang(moment($scope.supplierArr.OPENING_DATE).format('YYYY-MM-DD'), "englishdatedocument5"); }
            //$scope.ConvertEngToNepang(moment($scope.supplierArr.CREATED_DATE).format('MM-DD-YYYY'), "englishdatedocument5")
            //$scope.ConvertEngToNepang($scope.supplierArr.MATURITY_DATE.toString('yyyy-MM-dd'), "englishdatedocument6")
            $("#supplierModal").modal();
            setTimeout(function () {
                //$scope.loadimage($scope.supplierArr.IMAGE_FILE_NAME);
            }, 500);


        });
    }
    $scope.addNewsupplier = function () {
        /* */
        $scope.clearData();
        $scope.editFlag = "N";
        $scope.AE = true;
        $scope.saveupdatebtn = "Save"
        $scope.groupsupplierTypeFlag = "Y";
        $scope.suppliersetup.GROUP_SKU_FLAG = "G";
        $scope.supplierArr.GROUP_SKU_FLAG = "G";
        var tree = $("#groupmastersuppliercode").data("kendoDropDownList");
        tree.value("");

        var tree = $("#suppliertree").data("kendoTreeView");
        tree.dataSource.read();
        $scope.newSupplierId();
        $("#groupsupplierModal").modal("toggle");
        $scope.savegroup = true;
    }

    $scope.ConvertNepToEng = function ($event) {

        //$event.stopPropagation();
        console.log($(this));
        var date = BS2AD($("#nepaliDate5").val());
        $("#englishdatedocument").val($filter('date')(date, "dd-MMM-yyyy"));
        $('#nepaliDate5').trigger('change');


        // var date1 = moment(maturityDate).format("DD-MM-YYYY")


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
    $scope.monthSelectorOptionsmaturity = {
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

    $scope.loadimage = function (img) {

        var imgfullpath = window.location.protocol + "//" + window.location.host + "/Areas/NeoERP.DocumentTemplate/images/" + img;

        $('#blah')
            .attr('src', imgfullpath)
            .width(140)
            .height(180);

        $('#txtFile')[0].files[0].name = img;
    };
    function DisplayNoResultsFound(grid) {

        // Get the number of Columns in the grid
        //var grid = $("#kGrid").data("kendo-grid");
        var dataSource = grid.data("kendoGrid").dataSource;
        var colCount = grid.find('.k-grid-header colgroup > col').length;

        // If there are no results place an indicator row
        if (dataSource._view.length == 0) {
            grid.find('.k-grid-content tbody')
                .append('<tr class="kendo-data-row"><td colspan="' + colCount + '" style="text-align:center"><b>No Results Found!</b></td></tr>');
        }

        // Get visible row count
        var rowCount = grid.find('.k-grid-content tbody tr').length;

        // If the row count is less that the page size add in the number of missing rows
        if (rowCount < dataSource._take) {
            var addRows = dataSource._take - rowCount;
            for (var i = 0; i < addRows; i++) {
                //grid.find('.k-grid-content tbody').append('<tr class="kendo-data-row"><td>&nbsp;</td></tr>');
            }
        }
    };
    var openingDate = $('#englishdatedocument5').val();
    $scope.OnOpeningDateChange = function () {
        var openingDate = $('#englishdatedocument5').val();
        var maturityDate = $('#englishdatedocument6').val();
        if (maturityDate != undefined && maturityDate != null && maturityDate != "undefined" && maturityDate != "") {
            let momentODate = moment(openingDate);
            let momentMDate = moment(maturityDate);
            let totalDays = momentMDate.diff(momentODate, 'days');
            $("#totaldays").val(totalDays)
        }
        var oDate = moment(openingDate).format("MM-DD-YYYY");
        var mDate = moment(maturityDate).format("MM-DD-YYYY");
        if (maturityDate != "") {
            if (mDate < oDate) {
                $('#englishdatedocument5').val("");
                $("#savedocumentformdata").prop("disabled", true);
                displayPopupNotification("Opening date must be less than Maturity Date!", "warning");
                return;
            }
            else {
                $("#savedocumentformdata").prop("disabled", false);
            }
        }
    };
    //OnNepaliOpeningDateChange
    $scope.OnMaturityDateChange = function () {

        var openingDate = $('#englishdatedocument5').val();
        var maturityDate = $('#englishdatedocument6').val();
        // var date1 = moment(maturityDate).format("DD-MM-YYYY")
        if (maturityDate != undefined && maturityDate != null && maturityDate != "undefined" && maturityDate != "") {
            var startDay = new Date(openingDate);
            var endDay = new Date(maturityDate);
            var millisBetween = startDay.getTime() - endDay.getTime();
            var days = millisBetween / (1000 * 3600 * 24);
            var finatdaysdiff = Math.round(Math.abs(days));
            $('#totaldays').val(finatdaysdiff);
        }
        var oDate = moment(openingDate).format("MM-DD-YYYY");
        var mDate = moment(maturityDate).format("MM-DD-YYYY");
        if (mDate < oDate) {
            $('#englishdatedocument6').val("");
            $("#savedocumentformdata").prop("disabled", true);
            displayPopupNotification("Maturity date must be greater than Opening Date!", "warning");
            return;
        }
        else {
            $("#savedocumentformdata").prop("disabled", false);
        }
    };

    var TDSCodeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getTDSList";
    $scope.TDSCodeComboDataSource = new kendo.data.DataSource({
        serverFiltering: true,
        transport: {
            read: {
                url: TDSCodeUrl,
            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        }
    });

    $scope.TDSCodeOnChange = function (kendoEvent) {
        var Code = kendoEvent.sender.dataItem().LOCATION_CODE;
        var index = this.$index;
        var sublen = $scope.alternativeTDSInfoList.length;

        for (var j = 0; j < sublen; j++) {
            var subcode = $scope.alternativeTDSInfoList[j].TDS_CODE;
            if (index != j) {
                if (subcode === Code) {

                    $($(".TDSCode_" + index)[0]).addClass("borderRed");
                    $("#savedocumentformdatachild").prop("disabled", true);
                    $scope.TDSCount = true;
                    return;
                }
                else {

                    $($(".TDSCode_" + index)[0]).removeClass("borderRed");
                    $scope.TDSCount = false;
                    $("#savedocumentformdatachild").prop("disabled", false);
                };

            }
        }
    }

    $scope.TDSCodeOptions = {
        dataSource: $scope.TDSCodeComboDataSource,
        optionLabel: "-Select TDS Code-",
        dataTextField: "TDS_EDESC",
        dataValueField: "TDS_CODE",
        change: function (e) {
            $scope.TDSCodeOnChange(e);
        },
        //filter: "contains",
    }

    var customerStockUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getAllItemsForCustomerStock";
    $scope.customerStockDataSource = new kendo.data.DataSource({
        serverFiltering: true,
        transport: {
            read: {
                url: customerStockUrl,
            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        }
    });

    $scope.add_supplier_terms_and_conditions = function (index) {
        $scope.supplierTermsAndConditions.splice(index + 1, 0, {
            ITEM_CODE: "",
            MAX_LEAD_TIME: "",
            MIN_LEAD_TIME: "",
            IDEAL_LEAD_TIME: "",
            MIN_ORDER_QUANTITY: "",
            MAX_ORDER_QUANTITY: "",
            IDEAL_ORDER_QUANTITY: "",
            LAST_SUPPLY_QUANTITY: "",
            LAST_PURCHASE_QUANTITY: "",
            LAST_PURCHASE_ORDER_PRICE: "",
            LAST_LOGISTIC_COST_PERCENT: "",
            REMARKS: ""
        });
    }

    $scope.remove_supplier_terms_and_conditions = function (index) {
        $scope.supplierTermsAndConditions.splice(index, 1);
    }

    $scope.budgetcenterOnChange = function (index, bcenter) {

        $scope.supplierbcenter[index].BUDGET_CODE = bcenter.BUDGET_CODE;
    }

    $scope.initializeKendoMultiSelectForItemMap = function () {

        var selectElementJS = document.getElementById("supplierItemList");
        var selectElement = $(selectElementJS)

        if (selectElement.data("kendoMultiSelect")) {
            selectElement.data("kendoMultiSelect").destroy();
        }

        selectElement.empty();

        $http.get(customerStockUrl + "?filter=")
            .then(function (childResponse) {
                debugger
                var childOptions = childResponse.data;

                angular.forEach(childOptions, function (optionData) {

                    var optionValue = optionData.ITEM_CODE;
                    var optionText = optionData.ITEM_EDESC;

                    selectElement.append($("<option>", {
                        value: optionValue,
                        text: optionText
                    }));
                });

                var kendoOptions = {
                    dataTextField: "ITEM_EDESC",
                    dataValueField: "ITEM_CODE",
                    autoClose: false,
                    change: function () {
                        $scope.supplierItemMapping = this.value();
                        if (!$scope.$$phase) {
                            $scope.$apply();
                        }
                    }
                };
                selectElement.kendoMultiSelect(kendoOptions);
            })
            .catch(function (error) {
                console.error("Error fetching item list for mapping:", error);
            });
    };

    $timeout(function () {
        $scope.initializeKendoMultiSelectForItemMap();
    }, 0);

    $scope.setSelectedAttributes = function () {
        let selectElementJS = document.getElementById("supplierItemList");
        let selectElement = $(selectElementJS);
        var multiSelect = selectElement.data("kendoMultiSelect");

        if (multiSelect) {
            var allAvailableValues = [];
            var dataSource = multiSelect.dataSource;

            dataSource.data().forEach(function (item) {
                allAvailableValues.push(item.ITEM_CODE);
            });

            let valuesToSetAsSelected = [];
            if ($scope.supplierItemMapping) {
                $scope.supplierItemMapping.forEach(function (selectedCodes) {
                    if (allAvailableValues.includes(selectedCodes.toString())) {
                        valuesToSetAsSelected.push(selectedCodes.toString());
                    }
                });
            }

            if (valuesToSetAsSelected.length > 0) {
                multiSelect.value(valuesToSetAsSelected);
            } else {
                multiSelect.value([]);
            }
        }

        if (!$scope.$$phase) {
            $scope.$apply();
        }
    }

    var bankAccountUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getBankAccountsList";

    $scope.bankAccountComboDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: bankAccountUrl,
                dataType: "json"
            }
        },
        schema: {
            data: "DATA"
        }
    });

    $scope.add_bank_mapping = function (index) {
        $scope.supplierBankMapping.splice(index + 1, 0, {
            BANK_NAME: '',
            BANK_BRANCH: '',
            BANK_ACC_NO: '',
            ACC_CODE: '',
            ACC_EDESC: ''
        })
    }

    $scope.remove_bank_mapping = function (index) {
        $scope.supplierBankMapping.splice(index, 1);
    }
});
$(document).ready(function () {
    $(document).off("keydown.bs.dropdown.data-api")
        .on("keydown.bs.dropdown.data-api", "[data-toggle='dropdown'], [role='menu']",
            $.fn.dropdown.Constructor.prototype.keydown);

});




