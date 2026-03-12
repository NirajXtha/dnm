var SubsidiaryLedger = function () {

    var subledgerstaticdata = [
        { text: "Customer", value: "Customer" },
        { text: "Supplier", value: "Supplier" },
        { text: "Employee", value: "Employee" },
        { text: "Misc. Subledger", value: "mSubledger" },
    ],

        init = function () {
            
            var ledgerIds = "";
            BindKendoSplitter();
            LoadSubledgerTypes();
            CustomerMultiSelect();

            //LedgerMultiSelect(ledgerIds);
        },

        BindKendoSplitter = function () {
            $("#splitter").kendoSplitter({
                panes: [
                    { collapsible: true },
                    { collapsible: false },
                    { collapsible: true }
                ]
            });

           
        },

        LoadSubledgerTypes = function () {
            $("#subledgers").kendoDropDownList({
                dataTextField: "text",
                dataValueField: "value",
                dataSource: subledgerstaticdata,
                height: 100,
                change: onChange,
            })

            function onChange(e) {

                var subledgerType = e.sender.dataItem().value;
                
                CustomerMultiSelect(subledgerType);
                $("#ledgerdiv").html('');
                $("#firsttab").siblings().remove();
                $("#firsttab").addClass('active');
                $("#firsttab").trigger("click");
            }
        },

        CustomerMultiSelect = function (subledgerType) {
            
            var autoCompleteurl;
            var autoCompletedataSource = '';
            var multiselect = $('#customerMultiSelect').data("kendoMultiSelect");
            if (subledgerType == "Employee") {

                CustomerTreeView(subledgerType);
                autoCompleteurl = window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetSubsidiaryEmployees";
                funcautoCompleteDataSource(autoCompleteurl, multiselect);
                multiselect.options.placeholder = "Select Employees...";
                multiselect._placeholder();
                $('#headerlabel').text("Employees TreeView");
                $('.caption-subject').text("Employees List");

            }
            else if (subledgerType == "Supplier") {

                CustomerTreeView(subledgerType);
                var autoCompleteurl = window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetSubsidiarySuppliers";
                funcautoCompleteDataSource(autoCompleteurl, multiselect);
                multiselect.options.placeholder = "Select Suppliers...";
                multiselect._placeholder();
                $('#headerlabel').text("Suppliers TreeView");
                $('.caption-subject').text("Suppliers List");
            }
            else if (subledgerType == "Customer") {

                CustomerTreeView(subledgerType);
                autoCompleteurl = window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetSubsidiaryCustomers";
                funcautoCompleteDataSource(autoCompleteurl, multiselect);
                multiselect.options.placeholder = "Select Customers...";
                multiselect._placeholder();
                $('#headerlabel').text("Customers TreeView");
                $('.caption-subject').text("Customers List");
            }
            else if (subledgerType == "mSubledger") {

                CustomerTreeView(subledgerType);
                autoCompleteurl = window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetSubsidiaryMSubLedger";
                funcautoCompleteDataSource(autoCompleteurl, multiselect);
                multiselect.options.placeholder = "Select Misc SubLedger...";
                multiselect._placeholder();
                $('#headerlabel').text("Misc SubLedger TreeView");
                $('.caption-subject').text("Misc SubLedger List");
            }
            else {
                CustomerTreeView();
                autoCompleteurl = window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetSubsidiaryCustomers";
                autoCompletedataSource = new kendo.data.DataSource({
                    transport: {
                        read: {
                            url: autoCompleteurl,
                            dataType: "json"
                        }
                    }
                });
                $("#customerMultiSelect").kendoMultiSelect({
                    dataSource: autoCompletedataSource,
                    dataTextField: "Name",
                    dataValueField: "Code",
                    height: 600,
                    maxSelectedItems: 1,
                    placeholder: "Select Customers....",
                    autoClose: false,
                    select: OnSelect,

                });
                $('#headerlabel').text("Customers TreeView");
                $('#mainheaderlabel').text("Customers List");
            }

            function funcautoCompleteDataSource(autoCompleteurl, multiselect) {

                autoCompletedataSource = new kendo.data.DataSource({
                    transport: {
                        read: {
                            url: autoCompleteurl,
                            dataType: "json"
                        }
                    }
                });
                multiselect.setDataSource(autoCompletedataSource);

            }

            function OnSelect(e) {
                
                
                var dataItem = this.dataItem(e.item.index());
                //alert(dataItem.MasterCode);
                var groupSkuFlag = "G";
                var action = "dblclick";
                loadAccountLedger(dataItem.Code, dataItem.Name, groupSkuFlag, dataItem.MasterCode, action);
                $("#customerMultiSelect").data("kendoMultiSelect").close();
            }


        },

        CustomerTreeView = function (subledgerType) {

            var treeview = $('#CustomerTreeView').data('kendoTreeView');
            if (subledgerType == "Employee") {
                var loadAllEmployeeUrl = window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetAllParentEmployeeNodes";
                var loadEmployeeByNodeUrl = window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetAllEmployeeByEmpId?empId=";
                kendoTreeViewDataSource(loadAllEmployeeUrl, loadEmployeeByNodeUrl, treeview);
            }
            else if (subledgerType == "Customer") {
                var loadAllCustomersUrl = window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetAllParentCustomerNodes";
                var loadCustomerByNodeUrl = window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetAllCustomersByCustId?custId=";
                kendoTreeViewDataSource(loadAllCustomersUrl, loadCustomerByNodeUrl, treeview);
            }
            else if (subledgerType == "Supplier") {
                var loadAllSuppliersUrl = window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetAllParentSupplierNodes";
                var loadSupplierByNodeUrl = window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetAllSupplierBySupId?supId=";
                kendoTreeViewDataSource(loadAllSuppliersUrl, loadSupplierByNodeUrl, treeview);
            }
            else if (subledgerType == "mSubledger") {
                var loadAllMiscSubLedgerUrl = window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetAllParentMiscSubLedgerNodes";
                var loadMiscSubLedgerByNodeUrl = window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetAllMiscSubLedgerBySubId?subId=";
                kendoTreeViewDataSource(loadAllMiscSubLedgerUrl, loadMiscSubLedgerByNodeUrl, treeview);
            }
            else {
                var localData;
                var loadAllCustomersUrl = window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetAllParentCustomerNodes";
                var loadCustomerByNodeUrl = window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetAllCustomersByCustId?custId=";
                var customerTreeDataSource = new kendo.data.HierarchicalDataSource({
                    transport: {
                        read: function (options) {

                            if (typeof options.data.Code != 'undefined') {
                                var id = options.data.Code;
                                $.ajax({
                                    url: loadCustomerByNodeUrl + id,
                                    //data: 'customerId=' + id + '&Level=1' + currentnode.Level,
                                    //  data: currentnode,
                                    type: "GET",
                                    success: function (result) {
                                        var res = result;
                                        // localData = result;
                                        //addToLocalData(localData, res, id);
                                        options.success(res);
                                    },
                                    error: function (result) {
                                        options.error(result);
                                    }
                                });
                                //}
                            }
                            else {
                                $.ajax({
                                    url: loadAllCustomersUrl,
                                    //data: 'customerId=' + null + '&Level=' + null + "&masterCode=" + null,
                                    type: "GET",
                                    success: function (result) {
                                        setTimeout(function () {
                                            options.success(result);
                                        }, 1000);
                                        localData = result;
                                    },
                                    error: function (result) {
                                        options.error(result);
                                    }
                                });
                            }
                        }
                    },
                    schema: {
                        model: {
                            id: "Code",
                            AccountName: "Name",
                            hasChildren: "HasChildren",
                            masterCustCode: "MasterCode"
                        }
                    }
                });

                $("#CustomerTreeView").kendoTreeView({
                    select: onChange,
                    autoScroll: true,
                    autoBind: true,
                    dataSource: customerTreeDataSource,
                    dataTextField: "Name",
                    dataBound: function (e) {
                        accountTreeViewDataBound();
                    },
                    scrollable: {
                        virtual: true
                    },
                    check: onCheck,
                });
            }
            function onChange(e) {
                
                var data = $('#CustomerTreeView').data('kendoTreeView').dataItem(e.node);
                var listType = $('#subledgers').data("kendoDropDownList").value();
                ShowLedgers(data, listType);
            }

            function kendoTreeViewDataSource(loadParentUrl, loadChildUrl, treeview) {
                var customerTreeDataSource = new kendo.data.HierarchicalDataSource({
                    transport: {
                        read: function (options) {

                            if (typeof options.data.Code != 'undefined') {
                                var id = options.data.Code;
                                $.ajax({
                                    url: loadChildUrl + id,
                                    //data: 'customerId=' + id + '&Level=1' + currentnode.Level,
                                    //  data: currentnode,
                                    type: "GET",
                                    success: function (result) {
                                        var res = result;
                                        // localData = result;
                                        //addToLocalData(localData, res, id);
                                        options.success(res);
                                    },
                                    error: function (result) {
                                        options.error(result);
                                    }
                                });
                                //}
                            }
                            else {
                                $.ajax({
                                    url: loadParentUrl,
                                    //data: 'customerId=' + null + '&Level=' + null + "&masterCode=" + null,
                                    type: "GET",
                                    success: function (result) {
                                        setTimeout(function () {
                                            options.success(result);
                                        }, 1000);
                                        localData = result;
                                    },
                                    error: function (result) {
                                        options.error(result);
                                    }
                                });
                            }
                        }
                    },
                    schema: {
                        model: {
                            id: "Code",
                            AccountName: "Name",
                            hasChildren: "HasChildren",
                            masterCustCode: "MasterCode"
                        }
                    }
                });

                treeview.setDataSource(customerTreeDataSource);
            }
        },

        ShowLedgers = function (data, listType) {
            
            var urltest = Arg.url(window.location.protocol + "//" + window.location.host + "/Sales/SubsidiaryLedger/ShowSubLedgersTransaction", { Code: data.MasterCode, accountName: data.Name, listType: listType });
            $.ajax({
                url: urltest,
                type: "GET",
                dataType: "html",
                success: function (result) {

                    $("#ledgerdiv").html(result);
                },
                error: function (result) {
                }
            });
        },

        accountTreeViewDataBound = function () {

            $("#CustomerTreeView").find("[role='treeitem']").each(function () {
                
                var ev = $._data(this, 'events');
                if (ev && ev.dblclick) {
                    return;

                }

                $(this).on("dblclick", function () {
                    
                    event.stopPropagation();
                    var groupSkuFlag = "G";
                    var action = "dblclick";
                    var accountTreeview = $("#CustomerTreeView").data("kendoTreeView");
                    var account = accountTreeview.dataSource.getByUid($(this).attr("data-uid"));
                    
                    loadAccountLedger(account.Code, account.Name, groupSkuFlag,account.MasterCode, action);
                    // SubsidiaryLedger.CheckBoxControl(account.Code, account.Name);
                });

            });

        },

        loadAccountLedger = function (Code, accountName, groupSkuFlag,MasterCode, action) {
            
            var listType = $('#subledgers').data("kendoDropDownList").value();
            ShowLedgetDetails(Code, "account", accountName, listType, Code, groupSkuFlag,MasterCode, action)

            //var urltest = Arg.url(window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetSubLedgerTransactions", { accountCode: masterCode, listType: listType });
            //$.ajax({
            //    url: urltest,
            //    type: "GET",
            //    dataType: "html",
            //    success: function (result) {

            //        var Codes = [];
            //        var LinkSubCodes = [];
            //        var listofIds = JSON.parse(result);
            //        $.each(listofIds, function (key, val) {
            //            Codes.push(val.Code);
            //            LinkSubCodes.push(val.LinkSubCode)
            //        })

            //        if (Codes.length > 0) {

            //            ShowLedgetDetails(Codes, "account", accountName, listType, LinkSubCodes)
            //        }

            //        return;
            //    },
            //    error: function (result) {
            //    }
            //});
        },

        onCheck = function (e) {
            
            // get the ledgerMultiSelect values.
            var customerMultiSelect = $("#ledgerMultiSelect").data("kendoMultiSelect");
            var mvalue = customerMultiSelect.value();
            //get the customerId of the checked node
            var dataItemCustomerID = this.dataItem(e.node).AccountCode;
            if (this.dataItem(e.node).checked == true) {
                //append the selected node value in customerMultiSelect when checked
                var res = [];
                res.push(dataItemCustomerID);
                res.forEach(function (dataItem) {
                    var temp = $.merge($.merge([], mvalue), [dataItem]);
                    temp = $.unique(temp);
                    customerMultiSelect.value(temp);
                    customerMultiSelect._savedOld = temp.slice(0)
                });

            }
            if (this.dataItem(e.node).checked == false) {
                //remove value from customerMultiSelect if the node is unchecked
                mvalue.forEach(function (dataItem) {
                    //var temp = $.grep($.grep(mvalue,[]), [dataItem]);
                    var temp = $.grep(mvalue, function (a) { return a != dataItem });
                    temp = $.unique(temp);
                    customerMultiSelect.value(temp);
                });

            }
        },

        ShowLedgerDetails = function (ledgerId, type, tabName, listType, groupSkuFlag) {

            var accountCodes = ledgerId.toString().split(",");

            for (var i = 0; i < accountCodes.length; i++) {
                var accountName = "";
                if (type == "list" || type == undefined) {
                    accountName = $("a[data-ledgerid='" + accountCodes[i] + "']").attr("data-label");
                }
                else if (type == "multiselect") {
                    var dataitem = $("#ledgerMultiSelect").data("kendoMultiSelect").dataItems();

                    var item = $.grep(dataitem, function (item, index) {
                        return item.AccountCode == accountCodes[i];

                    });
                    if (item != undefined && item != null && item.length > 0) {
                        accountName = item[0].AccountName;
                    }
                }
                else if (type == "account") {
                    accountName = tabName;

                }
                var nextTab = $('#tabs li').size() + 1;
                ShowLedgerTab(accountCodes[i], accountName, nextTab, listType, groupSkuFlag);
            }
        },

        CheckBoxControl = function (checkedValue, label) {
            
            var linkSubCode = "";
            $('.textcheckedforledger').text(label);
            linkSubCode = checkedValue;
            var ledgerIds = [];
            var autoCompletedataSource = '';
            var ledgerIdsWithoutLinkSubCode = [];
            var listType = $("#subledgers").data("kendoDropDownList").value();
            if ($("#list-view-ledgerEvent").find("input:checked").length > 0) {
                $("#list-view-ledgerEvent").find("input:checked").each(function () {
                    var code = $(this).val();
                    if (listType == "Customer") {
                        code = "C" + checkedValue;
                        linkSubCode = "C" + checkedValue;
                    }
                    else if (listType == "Supplier") {
                        code = "S" + checkedValue;
                        linkSubCode = "S" + checkedValue;
                    }
                    else if (listType == "Employee") {
                        code = "E" + checkedValue;
                        linkSubCode = "E" + checkedValue;
                    }
                    else if (listType == "mSubLedger") {
                        code = "M" + checkedValue;
                        linkSubCode = "M" + checkedValue;
                    }

                    ledgerIds.push(code);
                    ledgerIdsWithoutLinkSubCode.push($(this).val());
                });
            }
            else {
                var code = ""
                if (listType == "Customer") {
                    code = "C" + checkedValue;
                    linkSubCode = "C" + checkedValue;
                }
                else if (listType == "Supplier") {
                    code = "S" + checkedValue;
                    linkSubCode = "S" + checkedValue;
                }
                else if (listType == "Employee") {
                    code = "E" + checkedValue;
                    linkSubCode = "E" + checkedValue;
                }
                else if (listType == "mSubLedger") {
                    code = "M" + checkedValue;
                    linkSubCode = "M" + checkedValue;
                }
            }
            if (ledgerIds.length > 1) {

                $("#ledgerEvent_" + checkedValue).attr('checked', true);

                $("#list-view-ledgerEvent").find("input:checked").each(function (key, val) {
                    
                    var code = $(this).val();
                    if (ledgerIdsWithoutLinkSubCode[key] != checkedValue) {
                        $("#ledgerEvent_" + ledgerIdsWithoutLinkSubCode[key]).attr('checked', false);
                    }
                });
            }
            ledgerIds = [];
            ledgerIds.push(linkSubCode);
            
            var autoCompleteurl = window.location.protocol + "//" + window.location.host + "/api/SubsidiaryLedger/GetLedgerAC?ledgerMultiSelects=" + ledgerIds;
            autoCompletedataSource = new kendo.data.DataSource({
                transport: {
                    read: {
                        url: autoCompleteurl,
                        dataType: "json"
                    }
                }
            });
            var multiselect = $('#LedgerMultiSelect').data("kendoMultiSelect");
            multiselect.setDataSource(autoCompletedataSource);
        }

    return {
        init: init,
        ShowLedgerDetails: ShowLedgerDetails,
        CheckBoxControl: CheckBoxControl
    };
}();


$(document).ready(function () {

    SubsidiaryLedger.init();
    DateFilter.init(function () {
        consolidate.init(function () {
          
        });
    });
    $(".listCheckBox").live("change", function () {
        
        var checkedValue = $(this).val();
        var label = $("label[for='ledgerEvent_" + checkedValue + "']").text().trim();
        SubsidiaryLedger.CheckBoxControl(checkedValue, label);
    });


});







