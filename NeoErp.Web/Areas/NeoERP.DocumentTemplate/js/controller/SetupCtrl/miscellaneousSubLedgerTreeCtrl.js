DTModule.controller('miscellaneousSubLedgerTreeCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter, $compile) {
    $scope.saveupdatebtn = "Save";
    $scope.miscellaneousSubLedgerArr;
    $scope.savegroup = false;
    $scope.groupschilddisabled = true;
    $scope.editFlag = "N";
    $scope.newrootinsertFlag = "Y";
    $scope.treenodeselected = "N";
    $scope.treeSelectedMasterMiscCode = "";
    $scope.miscellaneousSubLedger =
    {
        MISC_CODE: "",
        MISC_EDESC: "",
        MISC_NDESC: "",
        REMARKS: "",
        PRE_MISC_CODE: "",
        GROUP_SKU_FLAG: "G",
        PARENT_MISC_CODE: "",
        REGD_OFFICE_EADDRESS: "",
        TEL_MOBILE_NO1: "",
        VAT_NO: "",
        LINK_SUB_CODE: "",
        ACC_CODE: "",
        BANK_ACCOUNT_NO: "",
        LIMIT: "",
        ACC_OPENING_DATE: "",
        MATURITY_DATE: "",
        PRIOR_ALERT_DAYS: "",
        LOAN_TERMS: "",
        SL_TYPE: "",
        ACC_INT_SETUP: []
    }
    $scope.treeselectedMiscCode = "";
    $scope.miscellaneousSubLedgerArr = $scope.miscellaneousSubLedger;
    $scope.miscellaneousSubLedgerArr.accountInterestSetup = [];
    $scope.masterMiscellaneousSubLedgerCodeDataSource = [
        { text: "<PRIMARY>", value: "" }
    ];
    $scope.treeselectedmiscellaneousSubLedgerCode = "";
    var miscellaneousSubLedgerdataFillDefered = $.Deferred();

    var miscellaneousSubLedgerCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetMiscellaneousSubLedger";

    $scope.miscellaneousSubLedgerGroupDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: miscellaneousSubLedgerCodeUrl,
            }
        }
    });
    $scope.MACDS = [];

    $scope.miscellaneousSubLedgerGroupOptions = {
        dataSource: $scope.miscellaneousSubLedgerGroupDataSource,
        optionLabel: "<PRIMARY>",
        dataTextField: "MISC_EDESC",
        dataValueField: "MISC_CODE",
        change: function (e) {

            var currentItem = e.sender.dataItem(e.node);

        },
        dataBound: function (e) {

        }
    }


    var gettomiscellaneousSubLedgersByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetTreeMiscellaneousSubLedger";
    $scope.miscellaneousSubLedgertreeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: gettomiscellaneousSubLedgersByUrl,
                type: 'GET',
                data: function (data, evt) {
                }
            },

        },
        schema: {
            parse: function (data) {
                return data;
            },
            model: {
                id: "MiscId",
                parentId: "preMiscCode",
                children: "Items",
                fields: {
                    MISC_CODE: { field: "MiscId", type: "string" },
                    MISC_EDESC: { field: "MiscName", type: "string" },
                    parentId: { field: "preMiscCode", type: "string", defaultValue: "00" },
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

            if (newNode.MISC_CODE && newNode.MISC_EDESC) {
                nodeText = (newNode.MISC_EDESC || "").toLowerCase();
                isMatch = nodeText.includes(searchText);
            } else {
                newNode.MISC_CODE = node.MiscId;
                newNode.MISC_EDESC = node.MiscName;
                nodeText = (newNode.MiscName || "").toLowerCase();
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
            $scope.tree.setDataSource($scope.miscellaneousSubLedgertreeData);
            return;
        }

        const pristine = $scope.miscellaneousSubLedgertreeData._pristineData || $scope.miscellaneousSubLedgertreeData.data();
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

    $scope.miscellaneousSubLedgerOnDataBound = function (e) {

        $('#miscellaneousSubLedgertree').data("kendoTreeView").expand('.k-item');
    }
    $scope.getDetailByMiscellaneousSubLedgerCode = function (miscId) {
        var getMiscellaneousSubLedgerdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getMiscellaneousSubLedgerDetailsByMiscellaneousSubLedgerCode?regionCode=" + miscId;
        $http({
            method: 'GET',
            url: getMiscellaneousSubLedgerdetaisByUrl,

        }).then(function successCallback(response) {
            var miscellaneousSubLedgerNature = response.data.DATA.MISC_NATURE;
            $scope.bindNatureByMiscellaneousSubLedgerNature(miscellaneousSubLedgerNature);

        }, function errorCallback(response) {

        });
    }

    $scope.fillMiscellaneousSubLedgerSetupForms = function (miscId) {
        var getMiscellaneousSubLedgerdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getMiscellaneousSubLedgerDetailsByMiscellaneousSubLedgerCode?regionCode=" + miscId;
        $http({
            method: 'GET',
            url: getMiscellaneousSubLedgerdetaisByUrl,

        }).then(function successCallback(response) { 
            
            if (response.data.DATA != null) {
                if (response.data.DATA.GROUP_SKU_FLAG == "I") {
                    var dropdownlist = $("#mastermiscellaneoussubledgercode").data("kendoDropDownList");
                    dropdownlist.value($scope.treeselectedMiscCode);
                }
                $scope.miscellaneousSubLedger = response.data.DATA;
                $scope.miscellaneousSubLedgerArr = response.data.DATA;
                $scope.treeSelectedMasterMiscCode = response.data.DATA.MASTER_MISC_CODE;
                $scope.miscellaneousSubLedgerArr.CHILD_AUTOGENERATED = response.data.DATA.MISC_CODE;

                $scope.miscellaneousSubLedgerArr.accountInterestSetup = angular.copy($scope.miscellaneousSubLedger.ACC_INT_SETUP || []);
                $scope.miscellaneousSubLedgerArr.accountInterestSetup.forEach(function (item, index) {
                    if (item.INT_DATE) {
                        try {
                            const englishDate = $scope.dateToYYYYMMDD(item.INT_DATE);
                            const nepaliDate = AD2BS(englishDate);
                            item.NEP_INT_DATE = nepaliDate;
                            let nepDateSelector = "#nepaliCalendarAccInterest_" + index;
                            $(nepDateSelector).val(nepaliDate)
                        } catch (e) {
                            item.NEP_INT_DATE = "";
                        }
                    } else {
                        item.NEP_INT_DATE = "";
                    }
                });

                if ($scope.miscellaneousSubLedgerArr.MATURITY_DATE && $scope.miscellaneousSubLedgerArr.ACC_OPENING_DATE) {
                    let mDate = moment($scope.miscellaneousSubLedgerArr.MATURITY_DATE)
                    let oDate = moment($scope.miscellaneousSubLedgerArr.ACC_OPENING_DATE)
                    $scope.miscellaneousSubLedgerArr.MATURITY_DAYS = mDate.diff(oDate, 'days')
                }

                setTimeout(function () {
                    $scope.renderDynamicInterestTable();
                }, 0)

                miscellaneousSubLedgerdataFillDefered.resolve(response);

            }
        }, function errorCallback(response) {


        });
    }

    $scope.OnSubLedgerFlagChange = function () {
        setTimeout(function () {
            $scope.renderDynamicInterestTable();
            $(".nepali-calendar5").nepaliDatePicker({
                npdMonth: true,
                onFocus: true,
                npdYear: true,
                npdYearCount: 10,
                altFormat: "dd-MMM-YYYY",
                dateFormat: "dd-MMM-YYYY",
                onChange: function (evt) {
                    
                    var nepalidate = $("#nepaliDate5").val();
                    var Engdate = BS2AD(nepalidate);

                    $("#englishdatedocument5").val(Engdate);

                    var openingDate = $('#nepaliDate5').val();
                    var maturityDate = $('#nepaliDate6').val();
                    var oDate = moment(openingDate).format("MM-DD-YYYY");
                    var mDate = moment(maturityDate).format("MM-DD-YYYY");
                    if (maturityDate != "") {
                        if (mDate < oDate) {
                            $('#nepaliDate5').val("");
                            $('#englishdatedocument5').val("");
                            $("#savedocumentformdata").prop("disabled", true);
                            displayPopupNotification("Start date must be less than End Date!", "warning");
                            return;
                        }
                        else {
                            let maturityMomentDate = moment(maturityDate)
                            let openingMomentDate = moment(openingDate)
                            let dateDifference = maturityMomentDate.diff(openingMomentDate, 'days');
                            $("#totalDays").val(dateDifference)
                            $("#savedocumentformdata").prop("disabled", false);
                        }

                    }
                }
            });
            $(".nepali-calendar6").nepaliDatePicker({
                ndpEnglishInput: 'englishdatedocument6',
                npdMonth: true,
                onFocus: true,
                npdYear: true,
                npdYearCount: 10,
                altFormat: "dd-MMM-YYYY",
                dateFormat: "dd-MMM-YYYY",
                onChange: function (evt) {
                    
                    var nepalidate = $("#nepaliDate6").val();
                    var Engdate = BS2AD(nepalidate);

                    $("#englishdatedocument6").val(Engdate);

                    var openingDate = $('#nepaliDate5').val();
                    var maturityDate = $('#nepaliDate6').val();

                    var oDate = moment(openingDate).format("MM-DD-YYYY");
                    var mDate = moment(maturityDate).format("MM-DD-YYYY");
                    if (mDate < oDate) {
                        $('#nepaliDate6').val("");
                        $('#englishdatedocument6').val("");
                        $("#savedocumentformdata").prop("disabled", true);
                        displayPopupNotification("End  date must be greater than Start Date!", "warning");
                        return;
                    }
                    else {
                        let maturityMomentDate = moment(maturityDate)
                        let openingMomentDate = moment(openingDate)
                        let dateDifference = maturityMomentDate.diff(openingMomentDate, 'days');
                        $("#totalDays").val(dateDifference)
                        $("#savedocumentformdata").prop("disabled", false);
                    }
                }
            });
        }, 0)
    }


    $scope.populateNewMiscellaneousSubLedgerSetupForms = function (miscId) {

        var getMiscellaneousSubLedgerdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getMiscellaneousSubLedgerDetailsByMiscellaneousSubLedgerCode?regionCode=" + miscId;
        $http({
            method: 'GET',
            url: getMiscellaneousSubLedgerdetaisByUrl,
        }).then(function successCallback(response) {
            
            if (response.data.DATA != null) {
                if (response.data.DATA.GROUP_SKU_FLAG == "I") {
                    var dropdownlist = $("#mastermiscellaneoussubledgercode").data("kendoDropDownList");
                    dropdownlist.value($scope.treeselectedMiscCode);
                }
                $scope.miscellaneousSubLedger = response.data.DATA;
                $scope.miscellaneousSubLedgerArr = response.data.DATA;
                $scope.treeSelectedMasterMiscCode = response.data.DATA.MASTER_MISC_CODE;
                $scope.miscellaneousSubLedgerArr.accountInterestSetup = [];
                $scope.miscellaneousSubLedgerArr.SL_TYPE = 'NL';
                $scope.miscellaneousSubLedgerArr.LOAN_TERMS = 'M';
                setTimeout(function () {
                    $scope.renderDynamicInterestTable();
                }, 0)

                miscellaneousSubLedgerdataFillDefered.resolve(response);
            }
        }, function errorCallback(response) {


        });
    }
    $scope.miscellaneousSubLedgeroptions = {
        loadOnDemand: false,
        select: function (e) {
            var currentItem = e.sender.dataItem(e.node);
            $('#miscellaneousSubLedgerGrid').removeClass("show-displaygrid");
            $("#miscellaneousSubLedgerGrid").html("");
            $($(this._current).parents('ul')[$(this._current).parents('ul').length - 1]).find('span').removeClass('hasTreeCustomMenu');
            $(this._current.context).find('span').addClass('hasTreeCustomMenu');
            $scope.miscellaneousSubLedger.MISC_CODE = currentItem.MISC_CODE;
            $scope.miscellaneousSubLedgerArr.MISC_CODE = currentItem.MISC_CODE;
            $scope.miscellaneousSubLedger.MISC_EDESC = currentItem.MISC_EDESC;
            $scope.treenodeselected = "Y";
            $scope.treeselectedMiscCode = currentItem.MISC_CODE;
            $scope.newrootinsertFlag = "N";
            $scope.groupschilddisabled = true;
            $scope.$apply();


        },

    };

    $scope.movescrollbar = function () {
        var element = $(".k-in");
        for (var i = 0; i < element.length; i++) {
            var selectnode = $(element[i]).hasClass("k-state-focused");
            if (selectnode) {
                $("#miscellaneousSubLedgertree").animate({
                    scrollTop: (parseInt(i * 2))
                });
                break;
            }
        }
    }
    $scope.onContextSelect = function (event) {
        if ($scope.miscellaneousSubLedger.MISC_CODE == "")
            return displayPopupNotification("Select Miscellaneous Sub Ledger.", "error");;
        $scope.saveupdatebtn = "Save";
        if (event.item.innerText.trim() == "Delete") {
            $scope.delete($scope.miscellaneousSubLedgerArr.MISC_CODE, "");

        }
        else if (event.item.innerText.trim() == "Update") {
            miscellaneousSubLedgerdataFillDefered = $.Deferred();
            $scope.saveupdatebtn = "Update";
            $scope.fillMiscellaneousSubLedgerSetupForms($scope.treeselectedMiscCode);
            $.when(miscellaneousSubLedgerdataFillDefered).then(function () {

                if ($scope.miscellaneousSubLedger.PARENT_MISC_CODE == null || $scope.miscellaneousSubLedger.PARENT_MISC_CODE == undefined) {
                    var popUpDropdown = $("#mastermiscellaneoussubledgercode").data("kendoDropDownList");
                    popUpDropdown.value('');
                }
                else {
                    var popUpDropdown = $("#mastermiscellaneoussubledgercode").data("kendoDropDownList");
                    popUpDropdown.value($scope.miscellaneousSubLedger.PARENT_MISC_CODE);
                }
                $("#miscellaneousSubLedgerModal").modal();
            })
        }
        else if (event.item.innerText.trim() == "Add") {
            miscellaneousSubLedgerdataFillDefered = $.Deferred();
            $scope.savegroup = true;
            $scope.miscellaneousSubLedgerArr = [];
            $scope.fillMiscellaneousSubLedgerSetupForms($scope.miscellaneousSubLedger.MISC_CODE);
            $.when(miscellaneousSubLedgerdataFillDefered).then(function () {
                $scope.Cleardata();
                var dropdownlist = $("#mastermiscellaneoussubledgercode").data("kendoDropDownList");
                dropdownlist.value($scope.treeselectedMiscCode);
                $scope.miscellaneousSubLedger.GROUP_SKU_FLAG = "G";
                $scope.miscellaneousSubLedgerArr.GROUP_SKU_FLAG = "G";
                $("#miscellaneousSubLedgerModal").modal();

            });


        }
    }
    $scope.saveNewMiscellaneousSubLedger = function (isValid) {
        let masterMiscCodeValue = $scope.treeSelectedMasterMiscCode;
        
        if (isValid == false) {
            displayPopupNotification("Please Select Miscellaneous Sub Ledger English Name", "warning");
            return;
        }

        if ($scope.saveupdatebtn == "Save") {
            var createUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewMiscellaneousSubLedgerHead";
            var model = {
                MISC_CODE: $scope.miscellaneousSubLedgerArr.CHILD_AUTOGENERATED,
                MISC_EDESC: $scope.miscellaneousSubLedgerArr.MISC_EDESC,
                MISC_NDESC: $scope.miscellaneousSubLedgerArr.MISC_NDESC,
                REMARKS: $scope.miscellaneousSubLedgerArr.REMARKS,
                GROUP_SKU_FLAG: $scope.miscellaneousSubLedgerArr.GROUP_SKU_FLAG,
                REGD_OFFICE_EADDRESS: $scope.miscellaneousSubLedgerArr.REGD_OFFICE_EADDRESS,
                TEL_MOBILE_NO1: $scope.miscellaneousSubLedgerArr.TEL_MOBILE_NO1,
                VAT_NO: $scope.miscellaneousSubLedgerArr.VAT_NO,
                LINK_SUB_CODE: $scope.miscellaneousSubLedgerArr.LINK_SUB_CODE,
                MASTER_MISC_CODE: masterMiscCodeValue,
                ACC_CODE: $scope.miscellaneousSubLedgerArr.ACC_CODE,
                BANK_ACCOUNT_NO: $scope.miscellaneousSubLedgerArr.BANK_ACCOUNT_NO,
                LIMIT: $scope.miscellaneousSubLedgerArr.LIMIT,
                ACC_OPENING_DATE: $scope.miscellaneousSubLedgerArr.ACC_OPENING_DATE,
                MATURITY_DATE: $scope.miscellaneousSubLedgerArr.MATURITY_DATE,
                PRIOR_ALERT_DAYS: $scope.miscellaneousSubLedgerArr.PRIOR_ALERT_DAYS,
                LOAN_TERMS: $scope.miscellaneousSubLedgerArr.LOAN_TERMS,
                SL_TYPE: $scope.miscellaneousSubLedgerArr.SL_TYPE
            }

            let accountInterestSetup = $scope.miscellaneousSubLedgerArr.accountInterestSetup || [];
            let accountInterestSetupNew = []
            angular.forEach(accountInterestSetup, function (value, index) {
                let nepDateSelector = "#nepaliCalendarAccInterest_" + index;
                let nepDateValue = $(nepDateSelector).val();

                if (nepDateValue) {
                    value.INT_DATE = BS2AD(nepDateValue)
                    delete value.NEP_INT_DATE;
                    accountInterestSetupNew.push(value);
                }
            });
            model.ACC_INT_SETUP = accountInterestSetupNew;

            $http({
                method: 'POST',
                url: createUrl,
                data: model
            }).then(function successCallback(response) {
                
                if (response.data.MESSAGE == "INSERTED") {
                    $scope.miscellaneousSubLedgerArr = [];
                    if ($scope.miscellaneousSubLedger.GROUP_SKU_FLAG !== "I") {
                        var tree = $("#miscellaneousSubLedgertree").data("kendoTreeView");
                        tree.dataSource.read();
                        $scope.treeSelectedMasterMiscCode = ""
                    }
                    var grid = $('#kGrid').data("kendoGrid");
                    if (grid != undefined) {
                        grid.dataSource.read();
                    }
                    if ($scope.savegroup == true) { $("#miscellaneousSubLedgerModal").modal("toggle"); }
                    else { $("#miscellaneousSubLedgerModal").modal("toggle"); }
                    $("#mastermiscellaneoussubledgercode").data("kendoDropDownList").dataSource.read();
                    $scope.Cleardata();
                    displayPopupNotification("Data successfully saved ", "success");
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Miscellaneous Sub Ledger name already exist please try another Miscellaneous Sub Ledger name.", "error");
                }

                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            }, function errorCallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Miscellaneous Sub Ledger name already exist please try another Miscellaneous Sub Ledger name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            });
            $scope.Cleardata();


        }
        else {
            var updateUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateMiscellaneousSubLedgerByMiscellaneousSubLedgerCode";
            var model = {
                MISC_CODE: $scope.miscellaneousSubLedgerArr.CHILD_AUTOGENERATED,
                MISC_EDESC: $scope.miscellaneousSubLedgerArr.MISC_EDESC,
                MISC_NDESC: $scope.miscellaneousSubLedgerArr.MISC_NDESC,
                PRE_MISC_CODE: $scope.miscellaneousSubLedgerArr.PRE_MISC_CODE,
                MISC_TYPE_FLAG: $scope.miscellaneousSubLedgerArr.MISC_TYPE_FLAG,
                REMARKS: $scope.miscellaneousSubLedgerArr.REMARKS,
                GROUP_SKU_FLAG: $scope.miscellaneousSubLedgerArr.GROUP_SKU_FLAG,
                REGD_OFFICE_EADDRESS: $scope.miscellaneousSubLedgerArr.REGD_OFFICE_EADDRESS,
                TEL_MOBILE_NO1: $scope.miscellaneousSubLedgerArr.TEL_MOBILE_NO1,
                VAT_NO: $scope.miscellaneousSubLedgerArr.VAT_NO,
                LINK_SUB_CODE: $scope.miscellaneousSubLedgerArr.LINK_SUB_CODE,
                MASTER_MISC_CODE: masterMiscCodeValue,
                ACC_CODE: $scope.miscellaneousSubLedgerArr.ACC_CODE,
                BANK_ACCOUNT_NO: $scope.miscellaneousSubLedgerArr.BANK_ACCOUNT_NO,
                LIMIT: $scope.miscellaneousSubLedgerArr.LIMIT,
                ACC_OPENING_DATE: $scope.miscellaneousSubLedgerArr.ACC_OPENING_DATE,
                MATURITY_DATE: $scope.miscellaneousSubLedgerArr.MATURITY_DATE,
                PRIOR_ALERT_DAYS: $scope.miscellaneousSubLedgerArr.PRIOR_ALERT_DAYS,
                LOAN_TERMS: $scope.miscellaneousSubLedgerArr.LOAN_TERMS,
                SL_TYPE: $scope.miscellaneousSubLedgerArr.SL_TYPE,
            }

            let accountInterestSetup = $scope.miscellaneousSubLedgerArr.accountInterestSetup || [];
            let accountInterestSetupNew = []
            angular.forEach(accountInterestSetup, function (value, index) {
                let nepDateSelector = "#nepaliCalendarAccInterest_" + index;
                let nepDateValue = $(nepDateSelector).val();

                if (nepDateValue) {
                    value.INT_DATE = BS2AD(nepDateValue)
                    delete value.NEP_INT_DATE;
                    accountInterestSetupNew.push(value);
                }
            });
            model.ACC_INT_SETUP = accountInterestSetupNew;

            $scope.saveupdatebtn = "Update";
            $http({
                method: 'POST',
                url: updateUrl,
                data: model
            }).then(function successCallback(response) {

                if (response.data.MESSAGE == "UPDATED") {
                    $scope.miscellaneousSubLedgerArr = [];
                    if ($scope.miscellaneousSubLedger.GROUP_SKU_FLAG !== "I") {
                        var tree = $("#miscellaneousSubLedgertree").data("kendoTreeView");
                        tree.dataSource.read();
                        $scope.treeSelectedMasterMiscCode = ""
                    }

                    $scope.Cleardata();
                    var dropdownlist = $("#mastermiscellaneoussubledgercode").data("kendoDropDownList");
                    dropdownlist.dataSource.read();
                    $("#kGrid").data("kendoGrid").dataSource.read();
                    $("#miscellaneousSubLedgerModal").modal("toggle");
                    displayPopupNotification("Data successfully updated ", "success");
                }
                if (response.data.MESSAGE == "ERROR") {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            }, function errorCallback(response) {

                displayPopupNotification("Something went wrong.Please try again later.", "error");
            });
        }
    }


    $scope.MACDSOptions = {
        dataSource: $scope.MACDS,
        dataTextField: "text",
        dataValueField: "value",

    };

    $scope.masterMiscellaneousSubLedgerCodeOptions = {
        dataSource: $scope.masterMiscellaneousSubLedgerCodeDataSource,
        dataTextField: "text",
        dataValueField: "value",
    };


    $scope.BindGrid = function (groupId) {
        $(".topsearch").show();
        var url = null;
        if (groupId == "All") {
            if ($('#regtxtSearchString').val() == null || $('#regtxtSearchString').val() == '' || $('#regtxtSearchString').val() == undefined || $('#regtxtSearchString').val() == 'undefined') {
                alert('Input is empty or undefined.');
                return;
            }
            url = "/api/SetupApi/GetAllMiscellaneousSubLedgerList?searchtext=" + $('#regtxtSearchString').val();
        }
        else {
            $("#regtxtSearchString").val('');
            url = "/api/SetupApi/GetChildOfMiscellaneousSubLedgerByGroup?groupId=" + groupId;
        }

        var reportConfig = GetReportSetting("_miscellaneousSubLedgerSetupPartial");
        $scope.miscellaneousSubLedgerChildGridOptions = {
            dataSource: {
                transport: {
                    read: {
                        url: url,
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        type: "GET"
                    },
                    parameterMap: function (options, type) {
                        var paramMap = JSON.stringify($.extend(options, ReportFilter.filterAdditionalData()));
                        delete paramMap.$inlinecount;
                        delete paramMap.$format;
                        return paramMap;
                    }
                },
                error: function (e) {
                    displayPopupNotification("Sorry error occurred while processing data", "error");
                },
                model: {
                    fields: {
                        MISC_CODE: { type: "string" },
                        MISC_EDESC: { type: "string" },
                        REMARKS: { type: "string" },
                        CREATED_BY: { type: "string" },
                        REGD_OFFICE_EADDRESS: { type: "string" },
                        TEL_MOBILE_NO1: { type: "string" },
                        VAT_NO: { type: "string" },
                        LINK_SUB_CODE: { type: "string" },
                        ACC_CODE: { type: "string" },
                        BANK_ACCOUNT_NO: { type: "string" },
                        LIMIT: { type: "number" },
                        ACC_OPENING_DATE: { type: "date" },
                        MATURITY_DATE: { type: "date" },
                        PRIOR_ALERT_DAYS: { type: "number" },
                        LOAN_TERMS: { type: "string" },
                        SL_TYPE: {type: "string"}
                    }
                },
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
                    SaveReportSetting('_miscellaneousSubLedgerSetupPartial', 'kGrid');
            },
            columnHide: function (e) {
                if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                    SaveReportSetting('_miscellaneousSubLedgerSetupPartial', 'kGrid');
            },
            pageable: {
                refresh: true,
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
                UpdateReportUsingSetting("_miscellaneousSubLedgerSetupPartial", "kGrid");
                $('div').removeClass('.k-header k-grid-toolbar');
            },
            columns: [
                {
                    title: "Code",
                    field: "MISC_CODE",
                    width: "80px"

                },
                {
                    field: "MISC_EDESC",
                    title: "Description",
                    width: "120px"
                },
                {
                    field: "VAT_NO",
                    title: "VAT No.",
                    width: "120px"
                },
                {
                    field: "ACC_CODE",
                    title: "A/C Code",
                    width: "120px"
                },
                {
                    field: "CREATED_BY",
                    title: "Created By",
                    width: "120px"
                },
                {

                    title: "Action",
                    template: '<a class="fa fa-pencil-square-o editAction" title="Edit" ng-click="edit(dataItem.MISC_CODE)"><span class="sr-only"></span> </a><a class="fa fa-trash deleteAction" title="Delete" ng-click="delete(dataItem.MISC_CODE,dataItem.GROUP_SKU_FLAG)"><span class="sr-only"></span> </a>',
                    width: "70px"
                }
            ],
        };

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

    $scope.showModalForNew = function (event) {
        miscellaneousSubLedgerdataFillDefered = $.Deferred();
        $scope.editFlag = "N";
        $scope.saveupdatebtn = "Save";
        $scope.populateNewMiscellaneousSubLedgerSetupForms($scope.treeselectedMiscCode);
        $scope.OnSubLedgerFlagChange();
        $scope.Cleardata();
        $scope.miscellaneousSubLedgerArr.SUB_LEDGER_GROUP = 'N';
        $.when(miscellaneousSubLedgerdataFillDefered).then(function () {
            var dropdownlist = $("#mastermiscellaneoussubledgercode").data("kendoDropDownList");
            dropdownlist.value($scope.treeselectedMiscCode);
            $scope.miscellaneousSubLedger.GROUP_SKU_FLAG = "I";
            $scope.miscellaneousSubLedgerArr.GROUP_SKU_FLAG = "I";
            $scope.getMaxMiscCode();
            $scope.miscellaneousSubLedgerArr.MISC_EDESC = "";
            $("#miscellaneousSubLedgerModal").modal("toggle");
        }, function errorCallback(response) {
            displayPopupNotification(response.data.MISC_CODE, "error");

        });
    }
    $scope.edit = function (miscCode) {
        $scope.editFlag = "Y";
        $scope.saveupdatebtn = "Update";
        $scope.fillMiscellaneousSubLedgerSetupForms(miscCode);
        $scope.OnSubLedgerFlagChange();
        var dropdownlist = $("#mastermiscellaneoussubledgercode").data("kendoDropDownList");
        dropdownlist.value($scope.treeselectedMiscCode);

        $("#miscellaneousSubLedgerModal").modal();
    }
    $scope.delete = function (code, groupskuFlag) {
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
                    
                    if (groupskuFlag === "I")
                        $scope.miscellaneousSubLedger.GROUP_SKU_FLAG = "I";
                    else
                        $scope.miscellaneousSubLedger.GROUP_SKU_FLAG = "G"
                    var delUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteMiscellaneousSubLedgerSetupByMiscellaneousSubLedgerCode?MiscellaneousSubLedgerCode=" + code;
                    $http({
                        method: 'POST',
                        url: delUrl
                    }).then(function successCallback(response) {

                        if (response.data.MESSAGE == "DELETED") {

                            
                            var tree = $("#miscellaneousSubLedgertree").data("kendoTreeView");
                            tree.dataSource.read();
                            $scope.treenodeselected = "N";
                            var grid = $('#kGrid').data("kendoGrid");
                            grid.dataSource.read();
                            $scope.Cleardata();
                            displayPopupNotification("Data successfully deleted ", "success");
                        }
                        else if (response.data.MESSAGE == "HAS_CHILD") {
                            displayPopupNotification("You can not delete. It has child.", "warning");
                        }
                    }, function errorCallback(response) {
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                    });

                }

            }
        });
    }

    $scope.getMaxMiscCode = function () {
        var returnMaxMiscUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getMaxMiscCode";
        $http({
            method: 'GET',
            url: returnMaxMiscUrl,
        }).then(function successCallback(response) {
            $scope.miscellaneousSubLedgerArr.CHILD_AUTOGENERATED = response.data;
        }, function errorCallback(response) {
            displayPopupNotification(response.data.STATUS_CODE, "error");

        });
    }

    $scope.addNewMiscellaneousSubLedger = function () {
        
        $scope.editFlag = "N";
        $scope.savegroup = true;

        $scope.Cleardata();
        $scope.miscellaneousSubLedgerArr = [];
        $scope.MACDS = [];
        $scope.saveupdatebtn = "Save"
        $scope.groupMiscellaneousSubLedgerTypeFlag = "Y";
        $scope.miscellaneousSubLedger.MISC_TYPE_FLAG = "N";
        $scope.treeSelectedMasterMiscCode = "";
        $scope.getMaxMiscCode();
        $scope.MACDS.push({ text: "<PRIMARY>", value: "" });
        $scope.miscellaneousSubLedgerArr.GROUP_SKU_FLAG = 'G';
        $scope.miscellaneousSubLedger.GROUP_SKU_FLAG = 'G';
        $('#mastermiscellaneoussubledgercode').data('kendoDropDownList').value("");

        $scope.miscellaneousSubLedgerArr.MISC_TYPE_FLAG = "N";

        $("#miscellaneousSubLedgerModal").modal("toggle");
    }
    function DisplayNoResultsFound(grid) {

        var dataSource = grid.data("kendoGrid").dataSource;
        var colCount = grid.find('.k-grid-header colgroup > col').length;

        if (dataSource._view.length == 0) {
            grid.find('.k-grid-content tbody')
                .append('<tr class="kendo-data-row"><td colspan="' + colCount + '" style="text-align:center"><b>No Results Found!</b></td></tr>');
        }

        var rowCount = grid.find('.k-grid-content tbody tr').length;

        if (rowCount < dataSource._take) {
            var addRows = dataSource._take - rowCount;
            for (var i = 0; i < addRows; i++) {
            }
        }
    }

    $scope.Cleardata = function () {
        $scope.miscellaneousSubLedgerArr.MISC_EDESC = "";
        $scope.miscellaneousSubLedgerArr.MISC_NDESC = "";
        $scope.miscellaneousSubLedgerArr.REMARKS = "";
        $scope.miscellaneousSubLedgerArr.REGD_OFFICE_EADDRESS = "";
        $scope.miscellaneousSubLedgerArr.TEL_MOBILE_NO1 = "";
        $scope.miscellaneousSubLedgerArr.VAT_NO = "";
        $scope.miscellaneousSubLedgerArr.LINK_SUB_CODE = "";
        $scope.miscellaneousSubLedgerArr.ACC_CODE = "";
        $scope.miscellaneousSubLedgerArr.BANK_ACCOUNT_NO = "";
        $scope.miscellaneousSubLedgerArr.LIMIT = "";
        $scope.miscellaneousSubLedgerArr.ACC_OPENING_DATE = "";
        $scope.miscellaneousSubLedgerArr.MATURITY_DATE = "";
        $scope.miscellaneousSubLedgerArr.PRIOR_ALERT_DAYS = "";
        $scope.miscellaneousSubLedgerArr.LOAN_TERMS = "";
        $scope.miscellaneousSubLedgerArr.SL_TYPE = "";
        $("#nepaliDate5").val("");
        $("#nepaliDate6").val("");
        $scope.miscellaneousSubLedgerArr.accountInterestSetup = [];
    }

    var accTypeUrl123 = window.location.protocol + "//" + window.location.host + "/api/Purchase/GetAccountsList";
    $scope.custaccountOptions = {
        dataTextField: "AccountName",
        dataValueField: "AccountCode",
        maxSelectedItems: 1,
        valuePrive: true,
        autoClose: true,
        //headerTemplate: '<div class="col-md-offset-3"><strong>Select Customer acc...</strong></div>',
        //placeholder: "Select Customer acc...",

        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: accTypeUrl123,
                    dataType: "json"
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
        }
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

        dateInput: true
    };

    $scope.OnOpeningDateChange = function () {
        var openingDate = $('#englishdatedocument5').val();
        var maturityDate = $('#englishdatedocument6').val();
        var oDate = moment(openingDate).format("MM-DD-YYYY");
        var mDate = moment(maturityDate).format("MM-DD-YYYY");
        if (maturityDate != "") {
            if (mDate < oDate) {
                $('#englishdatedocument5').val("");
                $("#savedocumentformdata").prop("disabled", true);
                displayPopupNotification("Start date must be less than End  Date!", "warning");
                $scope.accountsArr.TOTAL_DAYS = "";
                return;
            }
            else {
                $("#savedocumentformdata").prop("disabled", false);
            }
        }
    };

    $scope.OnMaturityDateChange = function () {
        var openingDate = $('#englishdatedocument5').val();
        var maturityDate = $('#englishdatedocument6').val();

        var oDate = moment(openingDate).format("MM-DD-YYYY");
        var mDate = moment(maturityDate).format("MM-DD-YYYY");
        if (mDate < oDate) {
            $('#englishdatedocument6').val("");
            $("#savedocumentformdata").prop("disabled", true);
            displayPopupNotification("End date must be greater than Start Date!", "warning");
            $scope.accountsArr.MATURITY_DAYS = "";
            return;
        }
        else {
            $("#savedocumentformdata").prop("disabled", false);
        }

        if (maturityDate !== "" && openingDate !== "") {
            let mMomentDate = moment(maturityDate);
            let oMomentDate = moment(openingDate)
            let totalDays = mMomentDate.diff(oMomentDate, 'days');
            $scope.miscellaneousSubLedgerArr.MATURITY_DAYS = totalDays;
        }
    };

    $scope.renderDynamicInterestTable = function () {
        
        let tableHtml = `
        <table class="document_child_table dynamic-table table-fixed dynamic_child_table">
            <thead class="child-table-head">
                <tr>
                    <th>S.N.</th>
                    <th>Date</th>
                    <th>Interest Rate</th>
                    <th class="btn-action">&nbsp;</th>
                </tr>
            </thead>
            <tbody class="table_body">`;

        const list = $scope.miscellaneousSubLedgerArr.accountInterestSetup || [];

        if (list.length === 0) {

            tableHtml += `
            <tr>
                <td colspan="3" class="text-center">No interest setup entries.</td>
                <td class="btn-action">
                    <a href="" class="buttonadd" ng-click="add_acc_int_setup_row()"><i class="fa fa-plus"></i></a>
                </td>
            </tr>`;
        } else {
            for (let i = 0; i < list.length; i++) {
                const nepaliId = `nepaliCalendarAccInterest_${i}`;
                const englishId = `englishCalendarAccInterest_${i}`;

                tableHtml += `
                <tr>
                    <td>
                        <div>
                            <input type="text" class="form-control" value="${i + 1}" readonly />
                        </div>
                    </td>
                    <td>
                        <div style="position: relative">
                            <input type="hidden"
                                   id="${englishId}"
                                   name="${englishId}"
                                   style="width:100%;"
                                   class="englishdate" />

                            <div class="nepali_date">
                                <input type="text"
                                       id="${nepaliId}"
                                       data-checkfor="${englishId}"
                                       class="nepaliCalendarAccInterest nepalidate form-control"
                                       />
                                <i style="position: absolute; right: 0;" class="fa fa-calendar calendar-icon-2"></i>
                            </div>
                        </div>
                    </td>
                    <td>
                        <div>
                            <input type="text" ng-model="miscellaneousSubLedgerArr.accountInterestSetup[${i}].INT_RATE" class="form-control" />
                        </div>
                    </td>
                    <td class="btn-action">
                        <a href="" class="buttonadd" ng-click="add_acc_int_setup_row(${i})"><i class="fa fa-plus"></i></a>
                        <a href="javascript:void(0)" class="buttondelete" ng-click="remove_acc_int_setup_row(${i})">
                            <i class="fa fa-trash text-danger" data-tooltip="Remove other info"></i>
                        </a>
                    </td>
                </tr>`;
            }
        }

        tableHtml += `</tbody></table>`;

        const compiled = $compile(tableHtml)($scope);
        $('#accountInterestTableContainer').html(compiled);

        if ($scope.miscellaneousSubLedgerArr.ACC_OPENING_DATE) {
            let englishOpeningDate = $scope.dateToYYYYMMDD($scope.miscellaneousSubLedgerArr.ACC_OPENING_DATE)
            $scope.miscellaneousSubLedgerArr.ACC_OPENING_DATE = englishOpeningDate;
            let nepaliOpeningDate = AD2BS(englishOpeningDate);
            $("#nepaliDate5").val(nepaliOpeningDate);
        } else {
            $("#englishdatedocument5").val();
        }
        if ($scope.miscellaneousSubLedgerArr.MATURITY_DATE) {
            let englishMaturityDate = $scope.dateToYYYYMMDD($scope.miscellaneousSubLedgerArr.MATURITY_DATE)
            $scope.miscellaneousSubLedgerArr.MATURITY_DATE = englishMaturityDate;
            let nepaliMaturityDate = AD2BS(englishMaturityDate);
            $("#nepaliDate6").val(nepaliMaturityDate);
        } else {
            $("#englishdatedocument6").val();
        }

        setTimeout(() => {
            for (let i = 0; i < list.length; i++) {
                const nepaliId = `#nepaliCalendarAccInterest_${i}`;
                const englishId = `#englishCalendarAccInterest_${i}`;

                const storedNepaliDate = $scope.miscellaneousSubLedgerArr.accountInterestSetup[i].NEP_INT_DATE;
                if (storedNepaliDate) {
                    $(nepaliId).val(storedNepaliDate);
                }

                $(nepaliId).nepaliDatePicker({
                    npdMonth: true,
                    onFocus: true,
                    npdYear: true,
                    npdYearCount: 10,
                    dateFormat: "DD-MMM-YYYY",
                    altFormat: "DD-MMM-YYYY",
                    onChange: function () {
                        const nepaliVal = $(nepaliId).val();
                        try {
                            const engDate = BS2AD(nepaliVal);
                            if (engDate) {
                                $(englishId).val(engDate);
                                $scope.miscellaneousSubLedgerArr.accountInterestSetup[i].INT_DATE = engDate;
                                $scope.miscellaneousSubLedgerArr.accountInterestSetup[i].NEP_INT_DATE = nepaliVal;
                            }
                        } catch (e) {
                            $scope.miscellaneousSubLedgerArr.accountInterestSetup[i].INT_DATE = null;
                        }

                        $scope.$applyAsync();
                    }
                });
            }
        }, 100);

        if (!$scope.$$phase) {
            $scope.$apply();
        }

    };

    $scope.createDateFromString = function (dateString) {
        const regex = /^\d{4}-\d{2}-\d{2}$/;
        if (!regex.test(dateString)) {
            console.error("Invalid date format. Expected YYYY-MM-DD.");
            return null;
        }

        const parts = dateString.split('-');
        const year = parseInt(parts[0], 10);
        const month = parseInt(parts[1], 10) - 1;
        const day = parseInt(parts[2], 10);

        const date = new Date(year, month, day);

        if (date.getFullYear() !== year || date.getMonth() !== month || date.getDate() !== day) {
            console.error("Invalid date components. Date created does not match input.");
            return null;
        }

        return date;
    }

    $scope.dateToYYYYMMDD = function (date) {
        date = new Date(date);
        if (!(date instanceof Date) || isNaN(date.getTime())) {
            console.error("Invalid Date object provided.");
            return null;
        }

        const year = date.getFullYear();
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        const day = date.getDate().toString().padStart(2, '0');

        return `${year}-${month}-${day}`;
    }

    $scope.add_acc_int_setup_row = function (index) {
        $scope.miscellaneousSubLedgerArr.accountInterestSetup.splice(index + 1, 0, {
            INT_DATE: "",
            INT_RATE: ""
        });
        $scope.renderDynamicInterestTable();
    }

    $scope.remove_acc_int_setup_row = function (index) {
        $scope.miscellaneousSubLedgerArr.accountInterestSetup.splice(index, 1);
        $scope.renderDynamicInterestTable();
    }

});