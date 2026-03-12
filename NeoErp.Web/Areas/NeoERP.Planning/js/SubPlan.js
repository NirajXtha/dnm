var subPlanModule = (function () {

    var frequenciesbyplan = [];
    var itemsbyplan = [];
    var plancode, headerwisecolumn;
    var loadCus = false, loadDivision = false, loadEmployee = false;
    var finalData = [];

    function init() {
        $('#GenerateSubPlan').hide();
        $('#radiobutton').hide();
        getPlan();
        BindPlanName();
        BindSubGroup();
        BindFrequency();
        if (code != '') {
            $("#planName").data("kendoDropDownList").value(code);
            BindFrequencyByPlan(code);
        }
    }

    //start init functions
    function getPlan() {

        var param = {
            plancode: code,
        }
        $.ajax({
            type: "POST",
            url: window.location.protocol + "//" + window.location.host + "/api/SubPlanApi/GetPlan?plancode=" + code,
        }).done(function (result) {

        }).error(function (result) {

        })
    }
    function BindPlanName() {
        var mainurl = window.location.protocol + "//" + window.location.host + "/api/SubPlanApi/GetAllPlanNames";
        $("#planName").kendoDropDownList({
            optionLabel: "--Select Product Plan Name--",
            filter: "contains",
            dataTextField: "ITEM_EDESC",
            dataValueField: "PLAN_CODE",
            suggest: true,
            dataSource: {
                type: "json",
                serverFiltering: true,
                transport: {
                    read: {
                        url: mainurl,
                    },
                    parameterMap: function (data, action) {

                        if (data.filter != undefined) {
                            if (data.filter.filters[0] != undefined) {
                                var newParams = {
                                    filter: data.filter.filters[0].value
                                };
                                return newParams;
                            }
                            else {
                                var newParams = {
                                    filter: ""
                                };
                                return newParams;
                            }
                        }
                        else {
                            var newParams = {
                                filter: ""
                            };
                            return newParams;
                        }

                    }

                }
            }
        });

    }
    function BindSubGroup() {
        var mainurl = window.location.protocol + "//" + window.location.host + "/api/SubPlanApi/GetSubGroup";
        $("#subgroup").kendoDropDownList({
            optionLabel: "--Select SubGroup--",
            dataSource: {
                type: "json",
                transport: {
                    read: {
                        url: mainurl,
                    }

                }
            }
        });
    }
    function BindFrequency() {
        $("#frequency").kendoDropDownList({
            optionLabel: "--Select Frequency--",
            dataTextField: "TIME_FRAME_EDESC",
            dataValueField: "TIME_FRAME_CODE",

        });


    }
    //end init functions

    function BindFrequencyByPlan(PLAN_CODE) {
        var mainurl = window.location.protocol + "//" + window.location.host + "/api/SubPlanApi/GetTimeFrameForSubPlan?planCode=" + PLAN_CODE;
        return $.ajax({
            type: "GET",
            url: mainurl,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        });
    }

    function BindYearMonthColumnHeader(PLAN_CODE) {
        var mainurl = window.location.protocol + "//" + window.location.host + "/api/SubPlanApi/GetDynamicMultiHeader?plancode=" + PLAN_CODE;
        return $.ajax({
            type: "GET",
            url: mainurl,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        });
    }

    function BindItemsByPlan(PLAN_CODE) {

        var data = {
            planCode: PLAN_CODE,
            itemCode: "",
        }
        var mainurl = window.location.protocol + "//" + window.location.host + "/api/PlanSetupApi/getItemByCode";
        return $.ajax({
            type: "GET",
            url: mainurl,
            data: data,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        });
    }

    function SubGroupOnChange() {
        var employeeTree, divisionTree, customerTree;
        employeeTree = $("#employeeTreeView").data("kendoTreeView");
        divisionTree = $("#divisionTreeView").data("kendoTreeView");
        customerTree = $("#customerTreeView").data("kendoTreeView");

        $('#radiobutton').show();
        var groupname = $('#subgroup').data("kendoDropDownList").value();
        if (groupname == "CUSTOMER") {
            $(".slimScrollBar,.slimScrollRail").remove();
            $(".slimScrollDiv").contents().unwrap();
            $("#divisionTreeView").hide();
            $('#employeeTreeView').hide();
            if (loadCus == false) {

                $('#customerTreeView').show();
                if (employeeTree != undefined) {
                    employeeTree.dataSource.read();
                }
                else if (divisionTree != undefined) {
                    divisionTree.dataSource.read();
                }

                BindCustomerTreeView();
                loadCus = true;
            }
            else {
                if (employeeTree != undefined) {
                    employeeTree.dataSource.read();
                }
                else if (divisionTree != undefined) {
                    divisionTree.dataSource.read();
                }
                $('#customerTreeView').show();
            }
            $('#customerTreeView').slimScroll({
                height: '150px'
            });
        }
        if (groupname == "DIVISION") {

            $(".slimScrollBar,.slimScrollRail").remove();
            $(".slimScrollDiv").contents().unwrap();
            $('#customerTreeView').hide();
            $('#employeeTreeView').hide();
            if (loadDivision == false) {
                $("#divisionTreeView").show();
                if (employeeTree != undefined) {
                    employeeTree.dataSource.read();
                }
                else if (customerTree != undefined) {
                    customerTree.dataSource.read();
                }
                BindDivisionTreeView();
                loadDivision = true;
            }
            else {
                if (employeeTree != undefined) {
                    employeeTree.dataSource.read();
                }
                else if (customerTree != undefined) {
                    customerTree.dataSource.read();
                }
                $("#divisionTreeView").show();
            }
            $('#divisionTreeView').slimScroll({
                height: '150px'
            });
        }

        if (groupname == "EMPLOYEE") {

            $(".slimScrollBar,.slimScrollRail").remove();
            $(".slimScrollDiv").contents().unwrap();
            $('#customerTreeView').hide();
            $("#divisionTreeView").hide();
            if (loadEmployee == false) {
                $("#employeeTreeView").show();
                if (divisionTree != undefined) {
                    divisionTree.dataSource.read();
                }
                else if (customerTree != undefined) {
                    customerTree.dataSource.read();
                }
                BindEmployeeTreeView();
                loadEmployee = true;
            }
            else {
                if (divisionTree != undefined) {
                    divisionTree.dataSource.read();
                }
                else if (customerTree != undefined) {
                    customerTree.dataSource.read();
                }
                $("#employeeTreeView").show();
            }
            $('#employeeTreeView').slimScroll({
                height: '150px'
            });
        }

    }

    function BindCustomerTreeView() {


        var getCustomersByUrl = window.location.protocol + "//" + window.location.host + "/api/SubPlanApi/GetCustomers";
        var localData;
        var customerTreeDataSource = new kendo.data.HierarchicalDataSource({
            transport: {
                read: {
                    url: getCustomersByUrl,
                    type: 'GET',
                    data: function (data, evt) {
                    }
                },

            },
            schema: {
                model: {
                    id: "MASTER_CUSTOMER_CODE",
                    parentId: "PRE_CUSTOMER_CODE",
                    children: "Items",
                    fields: {
                        CUSTOMER_CODE: { field: "CUSTOMER_CODE", type: "string" },
                        CUSTOMER_EDESC: { field: "CUSTOMER_EDESC", type: "string" },
                        parentId: { field: "PRE_CUSTOMER_CODE", type: "string", defaultValue: "00" },
                    }
                }
            }
        });

        $("#customerTreeView").kendoTreeView({
            checkboxes: {
                checkChildren: true
            },
            loadOnDemand: false,
            autoScroll: true,
            autoBind: true,
            dataSource: customerTreeDataSource,
            dataTextField: "customerName",
            height: 400,
            scrollable: {
                virtual: true
            },
            check: onCheck,

        });

        var myArray = [];

        var pic = null, parentkey = 0;
        function onCheck(e) {

            finalData = [];
            treeView = $("#customerTreeView").data("kendoTreeView");
            customerDataSource = treeView.dataSource.view();
            SelectedItemTree(customerDataSource, finalData);


            function SelectedItemTree(customerDataSource, finalData) {

                $.each(customerDataSource, function (i, e) {

                    var hasChecked = true;
                    if (e.dirty && e.hasChildren) {
                        var status;
                        hasChecked = checkMyNodeIsChecked(e, status);
                        if (hasChecked == 'checked')
                            hasChecked = true;
                    }
                    else if (!e.checked) {
                        hasChecked = false;
                    }

                    if (hasChecked) {

                        if (customerDataSource[i].dirty == true || customerDataSource[i].checked == true) {

                            var parentId = customerDataSource[i].preCustomerCode == "00" ? null : customerDataSource[i].preCustomerCode;

                            if (customerDataSource[i].Items == null || customerDataSource[i].Items.length != 0) {
                                finalData.push({ "id": customerDataSource[i].masterCustomerCode, "preCustomerCode": customerDataSource[i].preCustomerCode, "masterCustomerCode": customerDataSource[i].masterCustomerCode, "customerName": customerDataSource[i].customerName, "parentId": parentId, "hasChildren": customerDataSource[i].hasChildren, "customerId": customerDataSource[i].customerId, "groupSkuFlag": customerDataSource[i].groupSkuFlag })
                            }
                            else {
                                finalData.push({ "id": customerDataSource[i].masterCustomerCode, "preCustomerCode": customerDataSource[i].preCustomerCode, "masterCustomerCode": customerDataSource[i].masterCustomerCode, "customerName": customerDataSource[i].customerName, "parentId": parentId, "hasChildren": false, "customerId": customerDataSource[i].customerId, "groupSkuFlag": customerDataSource[i].groupSkuFlag })
                            }
                            SelectedItemTree(customerDataSource[i].Items, finalData);
                        }
                    }

                });


            }
        }
    }

    function BindDivisionTreeView() {
        var loadAllDivisionsUrl = window.location.protocol + "//" + window.location.host + "/api/SubPlanApi/GetDivisions";
        var localData;
        var divisionTreeDataSource = new kendo.data.HierarchicalDataSource({
            transport: {
                read: {
                    url: loadAllDivisionsUrl,
                    type: 'GET',
                    data: function (data, evt) {

                    }
                },
            },
            schema: {
                model: {
                    id: "MASTER_DIVISION_CODE",
                    parentId: "PRE_DIVISION_CODE",
                    children: "Items",
                    fields: {
                        DIVISION_CODE: { field: "DIVISION_CODE", type: "string" },
                        DIVISION_EDESC: { field: "DIVISION_EDESC", type: "string" },
                        parentId: { field: "PRE_DIVISION_CODE", type: "string", defaultValue: "00" },
                    }
                }
            }
        });


        $("#divisionTreeView").kendoTreeView({
            checkboxes: {
                checkChildren: true
            },
            autoScroll: true,
            loadOnDemand: false,
            autoBind: true,
            dataSource: divisionTreeDataSource,
            dataTextField: "divisionName",
            height: 400,
            scrollable: {
                virtual: true
            },
            check: onCheck,
        });

        function onCheck(e) {
            finalData = [];
            treeView = $("#divisionTreeView").data("kendoTreeView");
            divisionDataSource = treeView.dataSource.view();
            SelectedItemTree(divisionDataSource, finalData);

            function SelectedItemTree(divisionDataSource, finalData) {

                $.each(divisionDataSource, function (i, e) {

                    var hasChecked = true;
                    if (e.dirty && e.hasChildren) {
                        var status;
                        hasChecked = checkMyNodeIsChecked(e, status);
                        if (hasChecked == 'checked')
                            hasChecked = true;
                    }
                    else if (!e.checked) {
                        hasChecked = false;
                    }

                    if (hasChecked) {

                        if (divisionDataSource[i].dirty == true || divisionDataSource[i].checked == true) {

                            var parentId = divisionDataSource[i].preDivisionCode == "00" ? null : divisionDataSource[i].preDivisionCode;
                            if (divisionDataSource[i].Items == null || divisionDataSource[i].Items.length != 0) {
                                finalData.push({ "id": divisionDataSource[i].masterDivisionCode, "preDivisionCode": divisionDataSource[i].preDivisionCode, "masterDivisionCode": divisionDataSource[i].masterDivisionCode, "divisionName": divisionDataSource[i].divisionName, "parentId": parentId, "hasChildren": divisionDataSource[i].hasChildren, "divisionId": divisionDataSource[i].divisionId, "groupSkuFlag": divisionDataSource[i].groupSkuFlag })
                            }
                            else {
                                finalData.push({ "id": divisionDataSource[i].masterDivisionCode, "preDivisionCode": divisionDataSource[i].preDivisionCode, "masterDivisionCode": divisionDataSource[i].masterDivisionCode, "divisionName": divisionDataSource[i].divisionName, "parentId": parentId, "hasChildren": false, "divisionId": divisionDataSource[i].divisionId, "groupSkuFlag": divisionDataSource[i].groupSkuFlag })
                            }
                            SelectedItemTree(divisionDataSource[i].Items, finalData);
                        }
                    }

                });


            }
        }

    }

    function BindEmployeeTreeView() {
        var loadAllEmployeesUrl = window.location.protocol + "//" + window.location.host + "/api/SubPlanApi/GetEmployees";
        var localData;
        var employeeTreeDataSource = new kendo.data.HierarchicalDataSource({
            transport: {
                read: {
                    url: loadAllEmployeesUrl,
                    type: 'GET',
                    data: function (data, evt) {

                    }
                },
            },
            schema: {
                model: {
                    id: "MASTER_EMPLOYEE_CODE",
                    parentId: "PRE_EMPLOYEE_CODE",
                    children: "Items",
                    fields: {
                        EMPLOYEE_CODE: { field: "EMPLOYEE_CODE", type: "string" },
                        EMPLOYEE_EDESC: { field: "EMPLOYEE_EDESC", type: "string" },
                        parentId: { field: "PRE_EMPLOYEE_CODE", type: "string", defaultValue: "00" },
                    }
                }
            }
        });

        $("#employeeTreeView").kendoTreeView({
            checkboxes: {
                checkChildren: true
            },
            loadOnDemand: false,
            autoScroll: true,
            autoBind: true,
            dataSource: employeeTreeDataSource,
            dataTextField: "employeeName",
            height: 400,
            scrollable: {
                virtual: true
            },
            check: onCheck,
        });

        function onCheck(e) {
            finalData = [];
            treeView = $("#employeeTreeView").data("kendoTreeView");
            employeeDataSource = treeView.dataSource.view();
            SelectedItemTree(employeeDataSource, finalData);

            function SelectedItemTree(employeeDataSource, finalData) {

                $.each(employeeDataSource, function (i, e) {

                    var hasChecked = true;
                    if (e.dirty && e.hasChildren) {
                        var status;
                        hasChecked = checkMyNodeIsChecked(e, status);
                        if (hasChecked == 'checked')
                            hasChecked = true;
                    }
                    else if (!e.checked) {
                        hasChecked = false;
                    }

                    if (hasChecked) {

                        if (employeeDataSource[i].dirty == true || employeeDataSource[i].checked == true) {

                            var parentId = employeeDataSource[i].preEmployeeCode == "00" ? null : employeeDataSource[i].preEmployeeCode;
                            if (employeeDataSource[i].Items == null || employeeDataSource[i].Items.length != 0) {
                                finalData.push({ "id": employeeDataSource[i].masterEmployeeCode, "preEmployeeCode": employeeDataSource[i].preEmployeeCode, "masterEmployeeCode": employeeDataSource[i].masterEmployeeCode, "employeeName": employeeDataSource[i].employeeName, "parentId": parentId, "hasChildren": employeeDataSource[i].hasChildren, "employeeId": employeeDataSource[i].employeeId, "groupSkuFlag": employeeDataSource[i].groupSkuFlag })
                            }
                            else {
                                finalData.push({ "id": employeeDataSource[i].masterEmployeeCode, "preEmployeeCode": employeeDataSource[i].preEmployeeCode, "masterEmployeeCode": employeeDataSource[i].masterEmployeeCode, "employeeName": employeeDataSource[i].employeeName, "parentId": parentId, "hasChildren": false, "employeeId": employeeDataSource[i].employeeId, "groupSkuFlag": employeeDataSource[i].groupSkuFlag })
                            }
                            SelectedItemTree(employeeDataSource[i].Items, finalData);
                        }
                    }

                });

            }
        }
    }

    // removing parent
    function checkMyNodeIsChecked(nodeData, status) {
        if (nodeData.checked == undefined && nodeData.hasChildren) {
            for (var i = 0; i < nodeData.Items.length; i++) {
                if (status == 'checked') {
                    return status;
                    break;
                }

                if (nodeData.Items[i].checked == undefined && nodeData.Items[i].hasChildren) {
                    if (nodeData.Items[i].dirty && nodeData.Items[i].hasChildren) {
                        status = check_on_child(nodeData.Items[i].Items, nodeData.Items[i], status);
                    }
                }
                else if (nodeData.Items[i].checked) {
                    status = 'checked';
                    return status;
                    break;
                }
            }
        }
        else if (nodeData.checked) {
            status = 'checked';
            return status;
        }
    }

    // remove child of parent
    function check_on_child(item, parent, status) {
        for (var i = 0; i < item.length; i++) {
            if (item[i].dirty && item[i].hasChildren && !item[i].checked) {
                check_on_child(item[i].Items, item[i], status);
            } else {
                status = 'checked';
                return status;
                break;
            }
        }
    }

    function ViewGrid() {

        if ($('#subplanName').val() == "" || $('#planName').data("kendoDropDownList").value() == "" || $('#subgroup').data("kendoDropDownList").value() == "") {
            displayPopupNotification("Fields should not be empty.", "error");
            return;
        }

        var PLAN_CODE = $("#planName").data("kendoDropDownList").value();
        showloader();
        BindYearMonthColumnHeader(PLAN_CODE).then(function (result) {

            headerwisecolumn = result;
            $('#ChoosedSubGroup').val($('#subgroup').data("kendoDropDownList").value());
            $('#GenerateSubPlan').show();
            if ($("#SubPlanTreeView").data("kendoTreeList") != undefined) {
                $("#SubPlanTreeView").html("");
                $("#SubPlanTreeView").data("kendoTreeList").destroy();
            }
            var header = "";
            if ($('#frequencyradio').is(':checked')) {
                header = "frequency";
                $('#headertype').val(header);
                BindFrequencyByPlan(PLAN_CODE).then(function (resultdata) {
                    $('#FrequencyLabel').val(resultdata[0].FrequencyName);
                    frequenciesbyplan = resultdata;
                    BindTree(header);
                });

            }
            else {
                header = "productname";
                $('#headertype').val(header);

                BindItemsByPlan(PLAN_CODE).then(function (itemsdata) {
                    itemsbyplan = itemsdata;
                    BindTree(header);
                });
            }
        });
    }

    function BindTree(header) {
        var months = [{ name: 'JAN', value: 1 }, { name: 'FEB', value: 2 }, { name: 'MAR', value: 3 }, { name: 'APR', value: 4 }
                , { name: 'MAY', value: 5 }, { name: 'JUN', value: 6 }, { name: 'JUL', value: 7 }, { name: 'AUG', value: 8 }, { name: 'SEP', value: 9 }
                , { name: 'OCT', value: 10 }, { name: 'NOV', value: 11 }, { name: 'DEC', value: 12 }];
        var percentresult = 0;
        var length = 0;
        var result = 0;
        var columns = [];

        if (finalData[0].customerName != undefined && finalData[0].customerName != 0) {
            finalData;
            columns = [{
                field: "customerName",
                title: "Customer Name",
                columns: [{
                    title: "Contact Title",
                    width: 200,
                    template: "#= <input type='text'/>#"
                }, ],
                width: "200px",
                locked: true
            },
            ];
            if (header == "frequency") {
                length = frequenciesbyplan.length;
                $.each(frequenciesbyplan, function (i, value) {
                    
                    var title = '';
                    var time_frame_value = "";
                    if (value.FrequencyName == "MONTH") {
                        title = title = frequenciesbyplan[i].Title;
                        time_frame_value = months[_.findIndex(months, function (voteItem) { return voteItem.name == value.getPeriod[0].MONTH })].value;
                    }
                    else if (value.FrequencyName == "WEEK") {
                        title = frequenciesbyplan[i].Title;
                        time_frame_value = frequenciesbyplan[i].getPeriod[0].WEEK;
                    }
                    columns.push({
                        title: title,
                        editable: true,
                        attributes: { style: "text-align: right;" },
                        format: "{0:n}",
                        width: 100,
                        template: "#= subPlanModule.SetPercentTextField('" + length + "',customerId,preCustomerCode,masterCustomerCode,groupSkuFlag,'" + time_frame_value + "_" + frequenciesbyplan[i].getPeriod[0].YEAR + "') #"
                    })
                });
            }
            else {
                length = itemsbyplan.length;
                $.each(itemsbyplan, function (i, value) {
                    columns.push({
                        field: value.ITEM_CODE,
                        title: itemsbyplan[i].ITEM_EDESC,
                        editable: true,
                        attributes: { style: "text-align: right;" },
                        format: "{0:n}",
                        width: 100,
                        //template: "#= subPlanModule.SetPercentTextField('" + length + "',customerId,preCustomerCode,masterCustomerCode,groupSkuFlag) #",
                        template: "#= subPlanModule.SetPercentTextFieldForProduct('" + length + "',customerId,preCustomerCode,masterCustomerCode,groupSkuFlag,'" + value.ITEM_CODE + "') #",
                    })
                });
            }
        }
        else if (finalData[0].divisionName != undefined && finalData[0].divisionName != 0) {
            columns = [{ field: "divisionName", title: "Division Name", width: "200px", locked: true },
                   //{ field: "percentValue", title: "Totals (in %)", width: "100px", locked: true, template: "#= subPlanModule.Total('" + length + "',divisionId,preDivisionCode,masterDivisionCode,groupSkuFlag) #" }
            ];

            if (header == "frequency") {
                length = frequenciesbyplan.length;
                $.each(frequenciesbyplan, function (i, value) {

                    var title = '';
                    var time_frame_value = "";
                    if (value.FrequencyName == "MONTH") {
                        //title = months[_.findIndex(months, function (voteItem) { return voteItem.value == value.TIME_FRAME_VALUE })].name;
                        title = frequenciesbyplan[i].Title;
                        time_frame_value = months[_.findIndex(months, function (voteItem) { return voteItem.name == value.getPeriod[0].MONTH })].value;
                    }
                    else if (value.FrequencyName == "WEEK") {
                        //title = frequenciesbyplan[i].TIME_FRAME_EDESC;
                        title = frequenciesbyplan[i].Title;
                        time_frame_value = frequenciesbyplan[i].getPeriod[0].WEEK;
                    }
                    columns.push({
                        field: value.id,
                        title: title,
                        editable: true,
                        attributes: { style: "text-align: right;" },
                        format: "{0:n}",
                        width: 100,
                        //template: "#= subPlanModule.SetPercentTextField('" + length + "',divisionId,preDivisionCode,masterDivisionCode,groupSkuFlag,'" + frequenciesbyplan[i].TIME_FRAME_VALUE + "') #",
                        template: "#= subPlanModule.SetPercentTextField('" + length + "',divisionId,preDivisionCode,masterDivisionCode,groupSkuFlag,'" + time_frame_value + "_" + frequenciesbyplan[i].getPeriod[0].YEAR + "') #",
                    })
                });
            }
            else {
                length = itemsbyplan.length;
                $.each(itemsbyplan, function (i, value) {
                    columns.push({
                        field: value.ITEM_CODE,
                        title: itemsbyplan[i].ITEM_EDESC,
                        editable: true,
                        attributes: { style: "text-align: right;" },
                        format: "{0:n}",
                        width: 100,
                        //template: "#= subPlanModule.SetPercentTextField('" + length + "',divisionId,preDivisionCode,masterDivisionCode,groupSkuFlag) #",
                        template: "#= subPlanModule.SetPercentTextFieldForProduct('" + length + "',divisionId,preDivisionCode,masterDivisionCode,groupSkuFlag,'" + value.ITEM_CODE + "') #",
                    })
                });
            }
        }
        else if (finalData[0].employeeName != undefined && finalData[0].employeeName != 0) {
            columns = [{ field: "employeeName", title: "Employee Name", width: "200px", locked: true },
                   //{ field: "percentValue", title: "Totals (in %)", width: "100px", locked: true, template: "#= subPlanModule.Total('" + length + "',employeeId,preEmployeeCode,masterEmployeeCode,groupSkuFlag) #" }
            ];

            if (header == "frequency") {
                length = frequenciesbyplan.length;
                $.each(frequenciesbyplan, function (i, value) {
                    // column generate
                    var title = '';
                    var time_frame_value = "";
                    if (value.FrequencyName == "MONTH") {
                        title = frequenciesbyplan[i].Title;
                        time_frame_value = months[_.findIndex(months, function (voteItem) { return voteItem.name == value.getPeriod[0].MONTH })].value;
                    }
                    else if (value.FrequencyName == "WEEK") {
                        title = frequenciesbyplan[i].Title;
                        time_frame_value = frequenciesbyplan[i].getPeriod[0].WEEK;
                    }
                    columns.push({
                        field: value.id,
                        title: title,
                        editable: true,
                        attributes: { style: "text-align: right;" },
                        format: "{0:n}",
                        width: 100,
                        template: "#= subPlanModule.SetPercentTextField('" + length + "',employeeId,preEmployeeCode,masterEmployeeCode,groupSkuFlag,'" + time_frame_value + "_" + frequenciesbyplan[i].getPeriod[0].YEAR + "') #",
                    })
                });
            }
            else {
                length = itemsbyplan.length;
                $.each(itemsbyplan, function (i, value) {
                    columns.push({
                        field: value.ITEM_CODE,
                        title: itemsbyplan[i].ITEM_EDESC,
                        editable: true,
                        attributes: { style: "text-align: right;" },
                        format: "{0:n}",
                        width: 100,
                        //template: "#= subPlanModule.SetPercentTextField('" + length + "',employeeId,preEmployeeCode,masterEmployeeCode,groupSkuFlag) #",
                        template: "#= subPlanModule.SetPercentTextFieldForProduct('" + length + "',employeeId,preEmployeeCode,masterEmployeeCode,groupSkuFlag,'" + value.ITEM_CODE + "') #",
                    })
                });
            }
        }

        var dataSource = new kendo.data.TreeListDataSource({
            data: (jQuery.makeArray(finalData)),
            schema: {
                model: {
                    id: "id",
                    fields: {
                        id: { type: "string" },
                        parentId: { field: "parentId", nullable: true },
                        percentValue: { field: "percentValue", type: "string" }
                    },
                }
            }
        });

        $("#SubPlanTreeView").kendoTreeList({
            dataSource: dataSource,
            columns: columns,
            scrollable: true,
            resizable: true,
            dataBound: function (e) {
                if (header == "frequency") {// adding month/year row on header
                    var html = "<tr style='height: 35px;'>";
                    var yearHtml = "<tr style='height: 35px;'>";
                    var monthHtml = "<tr style='height: 35px;'>";
                    var weekHeader = "";
                    var prevYear = "";
                    var prevMonth = "";
                    var yearmonth = "";

                    $.each(headerwisecolumn, function (index, it) {
                        if (it.getPeriod.length > 0 && (it.FrequencyName.indexOf("MONTH") != -1)) {
                            if (prevYear != it.getPeriod[0].YEAR) {
                                html += "<th class='k-header' style='text-align: center;font-weight:600' colspan='" + it.getPeriod[0].YEARCOUNT + "' >" + it.getPeriod[0].YEAR + "</th>";
                            }
                            prevYear = it.getPeriod[0].YEAR;
                        }
                        else if (it.getPeriod.length > 0 && (it.FrequencyName.indexOf("WEEK") != -1)) {
                            var year = it.getPeriod[0].YEAR;
                            var month = it.getPeriod[0].MONTH;
                            var week = it.getPeriod[0].WEEK;
                            var yearCount = it.getPeriod[0].YEARCOUNT;
                            var monthCount = it.getPeriod[0].MONTHCOUNT;

                            if (prevYear != year) {
                                yearHtml += "<th class='k-header' style='text-align: center;font-weight:600' colspan='" + yearCount + "' >" + it.getPeriod[0].YEAR + "</th>";
                                weekHeader = "week";
                                prevYear = year;
                            }
                            if (prevMonth != month) {
                                monthHtml += "<th class='k-header' style='text-align: center;font-weight:600' colspan='" + monthCount + "' >" + it.getPeriod[0].MONTH + "</th>";
                                prevMonth = month;
                            }
                        }
                    });

                    var blankRow = "<tr class='amountRow'><th></th><th></th></tr>", blankTr = '';
                    blankTr = blankRow;
                    if (html != "")
                        html += "</tr>";

                    if (weekHeader != "") {
                        html = yearHtml + "</tr>" + monthHtml + "</tr>";
                        blankTr = blankRow + blankRow;
                    }
                    $("#SubPlanTreeView").find("thead").last().prepend(html);
                    $("#SubPlanTreeView").find('thead').first().append(blankTr);
                }// adding month/year row on header end

                $("#SubPlanTreeView").data("kendoTreeList").autoFitColumn();
                var childInputArr = $('input[class*=total_]');


                $("input[type='number']").on("change", function () {
                    
                    var value = $(this).val();
                    var className = $(this)[0].className;
                    var allElementList = $("input[class*=frequency_I_]");
                    var columnElementList = $.grep(allElementList, function (v, i) {
                        return v.className.split('_')[v.className.split('_').length - 2] == className.split('_')[v.className.split('_').length - 2];
                    })

                    var count = parseFloat(0);
                    $.each(columnElementList, function (i, v) {
                        count = (parseFloat(count) + parseFloat(v.value));
                    })

                    if (count !== parseInt(100)) {
                        var valuedifference = (parseFloat(100) - parseFloat(count));
                        $(this).attr('title', 'The value difference to 100 is ' + valuedifference);
                        $(this).addClass('deactive');
                    }
                    else {
                        $.each(columnElementList, function (i, v) {
                            $(this).removeClass('deactive');
                        })
                    }
                });

                // column wise percent calculation.
                // subplan grid data.

                // comment area
                var treeview = $("#SubPlanTreeView").data("kendoTreeList").dataSource.data();
                var iLevelAllChilds = $.grep(treeview, function (value, index) { return value.groupSkuFlag == 'I'; }); // Ilevel Items
                var iLevelAllGroups = $.grep(treeview, function (value, index) { return value.groupSkuFlag == 'G'; }); // G level Items

                var iLevelFrequencyField = $('input[class*=frequency_I_]');
                var itemLength = iLevelAllChilds.length;
                $.each(iLevelFrequencyField, function (index, value) {
                    $this = value;
                    $this.value = Number(((1 / itemLength) * 100)).toFixed(2);
                });

                if (finalData[0].customerName != undefined && finalData[0].customerName != 0) {
                    // customer treelist logic
                    $.each(iLevelAllGroups, function (index, value) {
                        $this = value;
                        var masterCode = $this.masterCustomerCode;
                        var elements = [];
                        $.each(iLevelFrequencyField, function (i, v) { // getting all chield of each group
                            var cls = v.className.split('_')[3];
                            if (cls == masterCode) {
                                elements.push(v);
                            }
                        });

                        $.each(elements, function (i, v) { // getting each frequency withing the group and sum and place
                            var freq = v.className.split('_')[5];
                            var cols = $.grep(elements, function (v, i) {
                                return v.className.split('_')[v.className.split('_').length - 1] == freq;
                            })
                            var groupSpanList = $('span[class*=frequency_G_]');
                            $.each(groupSpanList, function (i, v) {
                                var inputMasterCode = v.className.split('_')[4];
                                var inputFreqValue = v.className.split('_')[v.className.split('_').length - 1];
                                if (inputFreqValue == freq) {
                                    var sum = 0;
                                    $.each(cols, function (i, v) { sum = parseFloat(sum) + parseFloat(v.value); });
                                    this.innerText = Number(sum).toFixed(2);
                                }
                            })
                        })
                    })

                    //if (header == "frequency") {
                    //    var freq = frequenciesbyplan;
                    //    var colmElements = $("input[class*=frequency_I_"); // all I Level frequencyItems
                    //    var colmSpanElements = $("span[class*=frequency_G_"); // all G Level frequencyItems
                    //    $.each(freq, function (i, v) {
                    //        
                    //        var freqVal = ''; // frequencyValue week/month
                    //        if (v.FrequencyName == "WEEK") { freqVal = v.getPeriod[0].WEEK; }
                    //        else if (v.FrequencyName == "MONTH") { freqVal = v.getPeriod[0].MONTH; }
                    //        var year = v.getPeriod[0].YEAR; // year

                    //        var colmsItems = $.grep(colmElements, function (val, i) {
                    //            var takeYear = val.className.split('_')[val.className.split('_').length - 1];
                    //            var takefreqVal = val.className.split('_')[val.className.split('_').length - 2];
                    //            if (takeYear == year && takefreqVal == freqVal) {
                    //                return val;
                    //            }
                    //        });
                    //        var uniqueItems = _.uniq(colmsItems, false, function (a) {
                    //            return a.className.split('_')[a.className.split('_').length - 3];
                    //        });
                    //        $.each(uniqueItems, function (i, val) {
                    //            var valueSplitted = val.className.split('_');
                    //            var preCode = valueSplitted[valueSplitted.length - 4];
                    //            var masterCode = valueSplitted[valueSplitted.length - 3];
                    //            var s_year = valueSplitted[valueSplitted.length - 1];
                    //            var s_month = valueSplitted[valueSplitted.length - 2];
                    //            var takespan = _.uniq(colmSpanElements, false, function (a) {
                    //                if (a.className.split('_')[a.className.split('_').length - 3] == preCode &&
                    //                    a.className.split('_')[a.className.split('_').length - 1] == s_year &&
                    //                    a.className.split('_')[a.className.split('_').length - 2] == s_month) {
                    //                    return a;
                    //                }
                    //            });

                    //            // save to takespan
                    //            //var sumFreqItemsOfUnique = 
                    //            var takenSpanClass = takespan[takespan.length - 1].className;
                    //            var el = colmElements;
                    //            
                    //            var elementsToSum = $.grep(el, function (a) {
                    //                return ((a.className.split('_')[a.className.split('_').length - 4] == preCode) &&
                    //                    (a.className.split('_')[a.className.split('_').length - 2] == s_month) &&
                    //                   (a.className.split('_')[a.className.split('_').length - 1] == s_year));
                    //            });

                    //            var sum = parseFloat(0);
                    //            $.each(elementsToSum, function (i, v) {
                    //                sum = sum + parseFloat(v.value);
                    //            })
                    //            takespan[takespan.length - 1].innerText = Number(sum).toFixed(2);
                    //        })
                    //        
                    //    });
                    //}
                }
                else if (finalData[0].divisionName != undefined && finalData[0].divisionName != 0) {
                    // division treelist logic
                    $.each(iLevelAllGroups, function (index, value) {
                        $this = value;
                        var masterCode = $this.masterDivisionCode;
                        var elements = [];
                        $.each(iLevelFrequencyField, function (i, v) { // getting all chield of each group
                            var cls = v.className.split('_')[3];
                            if (cls == masterCode) {
                                elements.push(v);
                            }
                        });

                        $.each(elements, function (i, v) { // getting each frequency withing the group and sum and place
                            var freq = v.className.split('_')[5];
                            var cols = $.grep(elements, function (v, i) {
                                return v.className.split('_')[v.className.split('_').length - 1] == freq;
                            })

                            var groupSpanList = $('span[class*=frequency_G_]');
                            $.each(groupSpanList, function (i, v) {
                                var inputMasterCode = v.className.split('_')[4];
                                var inputFreqValue = v.className.split('_')[v.className.split('_').length - 1];
                                if (inputFreqValue == freq) {
                                    var sum = 0;
                                    $.each(cols, function (i, v) { sum = parseFloat(sum) + parseFloat(v.value); });
                                    this.innerText = Number(sum).toFixed(2);
                                }
                            })
                        })
                    })
                }
                else if (finalData[0].employeeName != undefined && finalData[0].employeeName != 0) {
                    // employee treelist logic
                    $.each(iLevelAllGroups, function (index, value) {
                        $this = value;
                        var masterCode = $this.masterEmployeeCode;
                        var elements = [];
                        $.each(iLevelFrequencyField, function (i, v) { // getting all chield of each group
                            var cls = v.className.split('_')[3];
                            if (cls == masterCode) {
                                elements.push(v);
                            }
                        });

                        $.each(elements, function (i, v) { // getting each frequency withing the group and sum and place
                            var freq = v.className.split('_')[5];
                            var cols = $.grep(elements, function (v, i) {
                                return v.className.split('_')[v.className.split('_').length - 1] == freq;
                            })

                            var groupSpanList = $('span[class*=frequency_G_]');
                            $.each(groupSpanList, function (i, v) {
                                var inputMasterCode = v.className.split('_')[4];
                                var inputFreqValue = v.className.split('_')[v.className.split('_').length - 1];
                                if (inputFreqValue == freq) {
                                    var sum = 0;
                                    $.each(cols, function (i, v) { sum = parseFloat(sum) + parseFloat(v.value); });
                                    //this.innerText = Number(sum).toFixed(2);
                                }
                            })
                        })
                    })
                }
                
                if (header == "frequency") {
                        var freq = frequenciesbyplan;
                        var colmElements = $("input[class*=frequency_I_"); // all I Level frequencyItems
                        var colmSpanElements = $("span[class*=frequency_G_"); // all G Level frequencyItems
                        $.each(freq, function (i, v) {
                            
                            var freqVal = ''; // frequencyValue week/month
                            if (v.FrequencyName == "WEEK") { freqVal = v.getPeriod[0].WEEK; }
                            else if (v.FrequencyName == "MONTH") { freqVal = v.getPeriod[0].MONTH; }
                            var year = v.getPeriod[0].YEAR; // year

                            var colmsItems = $.grep(colmElements, function (val, i) {
                                var takeYear = val.className.split('_')[val.className.split('_').length - 1];
                                var takefreqVal = val.className.split('_')[val.className.split('_').length - 2];
                                if (takeYear == year && takefreqVal == freqVal) {
                                    return val;
                                }
                            });
                            var uniqueItems = _.uniq(colmsItems, false, function (a) {
                                return a.className.split('_')[a.className.split('_').length - 3];
                            });
                            $.each(uniqueItems, function (i, val) {
                                var valueSplitted = val.className.split('_');
                                var preCode = valueSplitted[valueSplitted.length - 4];
                                var masterCode = valueSplitted[valueSplitted.length - 3];
                                var s_year = valueSplitted[valueSplitted.length - 1];
                                var s_month = valueSplitted[valueSplitted.length - 2];
                                var takespan = _.uniq(colmSpanElements, false, function (a) {
                                    if (a.className.split('_')[a.className.split('_').length - 3] == preCode &&
                                        a.className.split('_')[a.className.split('_').length - 1] == s_year &&
                                        a.className.split('_')[a.className.split('_').length - 2] == s_month) {
                                        return a;
                                    }
                                });

                                // save to takespan
                                //var sumFreqItemsOfUnique = 
                                var takenSpanClass = takespan[takespan.length - 1].className;
                                var el = colmElements;
                                
                                var elementsToSum = $.grep(el, function (a) {
                                    return ((a.className.split('_')[a.className.split('_').length - 4] == preCode) &&
                                        (a.className.split('_')[a.className.split('_').length - 2] == s_month) &&
                                       (a.className.split('_')[a.className.split('_').length - 1] == s_year));
                                });

                                var sum = parseFloat(0);
                                $.each(elementsToSum, function (i, v) {
                                    sum = sum + parseFloat(v.value);
                                })
                                takespan[takespan.length - 1].innerText = Number(sum).toFixed(2);
                            })
                            
                        });
                    }
                // comment area end
                hideloader();
            }
        });


    }

    function SetPercentTextField(length, id, preCode, masterCode, groupSkuFlag, timeFrameValue) {
        if (timeFrameValue == undefined)
            timeFrameValue = 0;
        result = 100 / length;
        percentresult = Number(Math.round(result + 'e' + 2) + 'e-' + 2);
        if (groupSkuFlag == "I") {
            var html = "<input style='float:left; text-align:right;' min='0' class='frequency_" + groupSkuFlag + "_" + id + "_" + preCode + "_" + masterCode + "_" + timeFrameValue + "' name='frequency_" + groupSkuFlag + "_" + id + "_" + preCode + "_" + masterCode + "_" + timeFrameValue + "' value='" + percentresult + "' type='number'/>";
            return html;
        }
        else if (groupSkuFlag == 'G') {
            return "<span class='frequency_" + groupSkuFlag + "_" + id + "_" + preCode + "_" + masterCode + "_" + timeFrameValue + "'><span>";
        }
    }

    function SetPercentTextFieldForProduct(length, id, preCode, masterCode, groupSkuFlag, itemcode) {
        result = 100 / length;
        percentresult = Number(Math.round(result + 'e' + 2) + 'e-' + 2);

        if (groupSkuFlag == "I") {
            var html = "<input style='float:left; text-align:right;' min='0' class='frequency_" + groupSkuFlag + "_" + id + "_" + preCode + "_" + masterCode + "_" + itemcode + "' name='frequency_" + groupSkuFlag + "_" + id + "_" + preCode + "_" + masterCode + "_" + itemcode + "' value='" + percentresult + "' type='number'/>";
            return html;
        }
        else {
            return "";
        }
    }

    function Total(length, id, preCode, masterCode, groupSkuFlag) {

        if (groupSkuFlag == "I") {
            var result = 100;//parseInt(percentresult * length);
            var html = "<input style='float:left; text-align:right;' min='0' class='total_" + groupSkuFlag + "_" + id + "_" + preCode + "_" + masterCode + "' name='total_" + groupSkuFlag + "_" + id + "_" + preCode + "_" + masterCode + "' value='" + result + "' type='number' disabled/>";
            return html;
        }
        else {
            return "";
        }
    }

    function GenerateSubPlan() {

        var count = 0;
        var value = $('.deactive').length;
        if (value > 0) {
            if (count == 0) {
                displayPopupNotification("Error in % calculations. Please check the red dots.", "error");
            }
            count++;
            return;
        }
        showloader();
        var hello = $("#planName").data("kendoDropDownList").text();
        var data = $('#productPlanName').val($("#planName").data("kendoDropDownList").text());
        var subPlanUrl = window.location.protocol + "//" + window.location.host + "/SubPlan/SaveSubPlan";
        var subPlanDetail = $('#generateplan').serialize();
        $.ajax({
            type: 'POST',
            url: subPlanUrl,
            data: JSON.stringify(subPlanDetail),
            dataType: 'json',
            success: function (data) {
                window.location = "/Planning/Home/Setup#!Planning/SubplanList";
                hideloader();
                displayPopupNotification("Succesfully Saved.", "success");
            },
            error: function (error, status) {
                displayPopupNotification("Error Occured.", "error");
                hideloader();
            }
        });
    }
    return {
        init: init,
        ViewGrid: ViewGrid,
        SubGroupOnChange: SubGroupOnChange,
        SetPercentTextField: SetPercentTextField,//this is called by template so it should be made global
        SetPercentTextFieldForProduct: SetPercentTextFieldForProduct,
        Total: Total,//this is called by template so it should be made global
        GenerateSubPlan: GenerateSubPlan,

    };

})();

$(document).ready(function () { subPlanModule.init() });
$("#ViewGrid").on('click', function () { subPlanModule.ViewGrid(); });
$('#subgroup').on('change', function () { subPlanModule.SubGroupOnChange(); });
$('#GenerateSubPlan').on('click', function () { subPlanModule.GenerateSubPlan(); });




