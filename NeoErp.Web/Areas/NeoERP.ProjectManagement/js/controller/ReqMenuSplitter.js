PMModule.controller('ReqMenuSplitter', function ($scope, $http, $routeParams, $window, $filter) {
    $scope.subMenuList = [];
    $scope.orientation = "horizontal";
    $scope.formcode = "";
    $scope.formControlsInfo = [];
    var dataFillDefered = $.Deferred();
    var urlString = window.location.href;
    var parts = urlString.split('/');
    var moduleCode = parts[parts.length - 1]; // Get the last part after splitting by '/'

    $scope.moduleCode = moduleCode;
    //$scope.moduleCode = $routeParams.module;  
    if ($scope.moduleCode == "01") {
        $scope.modulename = "Financial Accounting"
    }
    else if ($scope.moduleCode == "02") {
        $scope.modulename = "Inventory & Procurement"
    }
    else if ($scope.moduleCode == "04") {
        $scope.modulename = "Sales & Revenue"
    }


    d1 = $.Deferred();
    d2 = $.Deferred();
    d3 = $.Deferred();

    var formcode = "";


    $scope.BackFromMenu = function () {
        $window.location.href = '/ProjectManagement/Home/Index#!PM/Dashboard';
    }

    $scope.BindDetailGrid = function (formcode, tableName, formname) {

        $scope.formcode = formcode;

        $scope.formname = formname;
        $("#kGrid").html("");
        BindGrid(formcode, tableName);
        setTimeout(function () {
            $('[data-toggle="tooltip"]').tooltip();
        }, 10)
        // get Splitter object
        //var splitter = $("#splitter").data("kendoSplitter");
        // modify the size of the first pane
        //splitter.options.panes[0].size = "0px";
        // force layout readjustment
        //splitter.resize(true);
    }
    var req = "/api/TemplateApi/GetSubMenuList?moduleCode=" + $scope.moduleCode;
    return $http.get(req).then(function (response) {
        $scope.subMenuList = response.data;
        console.log($scope.subMenuList);
        if ($scope.moduleCode == "01") {
            $scope.modulename = "Financial Accounting"
        }
        else if ($scope.moduleCode == "02") {
            formcode = $scope.subMenuList[0]['FORM_CODE'];
            tableName = $scope.subMenuList[0]['TABLE_NAME'];
            formname = $scope.subMenuList[0]['FORM_EDESC'];
            $scope.BindDetailGrid(formcode, tableName, formname);
        }
        else if ($scope.moduleCode == "04") {
            $scope.modulename = "Sales & Revenue"
        }
    });



    function BindGrid(formCode, tableName) {

        $scope.mainGridOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: "/api/TemplateApi/GetSubMenuDetailList?formcode=" + formCode
                },
                pageSize: 50,
                serverPaging: false,
                serverSorting: true,
                schema: {
                    model: {
                        fields: {
                            VOUCHER_NO: { type: "string" },
                            VOUCHER_DATE: { type: "date" },
                            CREATED_DATE: { type: "date" },
                            CHECKED_DATE: { type: "date" },
                            POSTED_DATE: { type: "date" },
                            MODIFY_DATE: { type: "date" },
                            VOUCHER_AMOUNT: { type: "number" }
                        }
                    }
                },
            },
            toolbar: kendo.template($("#toolbar-template").html()),
            excel: {
                fileName: "Export Report",
                allPages: true,
            },
            scrollable: true,
            height: 427,
            sortable: true,
            resizable: true,
            pageable: true,
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
                        startswith: "Starts with",
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
            },
            dataBound: function (e) {

                $("#kGrid tbody tr").css("cursor", "pointer");
                $("#kGrid tbody tr td:not(:last-child)").on('dblclick', function () {
                    var voucherNo = $(this).closest('tr').find('td span').html()
                    $scope.doSomething(voucherNo);

                })
                if ($scope.moduleCode != "04") {
                    this.hideColumn(13);
                }
            },
            columns: [

                {
                    field: "VOUCHER_NO",
                    title: "Document No.",
                    filterable: true
                    // width: "10"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Date",
                    template: "#=kendo.toString(kendo.parseDate(VOUCHER_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(VOUCHER_DATE),'dd MMM yyyy') #",
                    //width: "10"
                }, {
                    field: "VOUCHER_AMOUNT",
                    title: "Amount",
                    //width: "10",
                    attributes: { style: "text-align:right;" },
                    template: '#= kendo.format("{0:n}",VOUCHER_AMOUNT) #'
                },
                {
                    //field: "SESSION_ROWID",
                    field: "REFERENCE_NO",
                    title: "Manual No.",
                    //template: "#=kendo.toString(SESSION_ROWID) == null ? (kendo.toString(REFERENCE_NO)==null ? '' : kendo.toString(REFERENCE_NO)): kendo.toString(SESSION_ROWID) #",
                    template: "#=kendo.toString(REFERENCE_NO) == null ? (kendo.toString(SYN_ROWID)==null ? '' : kendo.toString(SYN_ROWID)): kendo.toString(REFERENCE_NO) #"
                    //width: "10"
                },
                {
                    field: "CREATED_BY",
                    title: "Prepared By",
                    //width: "10"
                }, {
                    field: "CREATED_DATE",
                    title: "Prepared Date & Time",
                    //template: "#= kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy') #",
                    template: "#= kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy hh:mm:ss') #",
                    //width: "10"
                },
                {
                    field: "CHECKED_BY",
                    title: "Checked By",
                    //width: "10",
                    hidden: true,
                },
                {
                    field: "CHECKED_DATE",
                    title: "Checked Date",
                    //template: "#= kendo.toString(kendo.parseDate(CHECKED_DATE),'dd MMM yyyy') #",
                    template: "#=kendo.toString(kendo.parseDate(CHECKED_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(CHECKED_DATE),'dd MMM yyyy') #",
                    //width: "8",
                    hidden: true,
                },
                {
                    field: "AUTHORISED_BY",
                    title: "Authorised By",
                    //width: "8",
                    hidden: true,
                },
                {
                    field: "POSTED_DATE",
                    title: "Posted Date",
                    //template: "#= kendo.toString(kendo.parseDate(POSTED_DATE),'dd MMM yyyy') #",
                    template: "#=kendo.toString(kendo.parseDate(POSTED_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(POSTED_DATE),'dd MMM yyyy') #",
                    //width: "7",
                    hidden: true,
                },
                {
                    field: "MODIFY_DATE",
                    title: "Modified Date",

                    //template: "#= kendo.toString(kendo.parseDate(MODIFY_DATE),'dd MMM yyyy') #",
                    //template: "#=kendo.toString(kendo.parseDate(MODIFY_DATE),'dd MMM yyyy hh:mm:ss') == null?'':kendo.toString(kendo.parseDate(MODIFY_DATE),'dd MMM yyyy hh:mm:ss') #",
                    template: "#=kendo.toString(kendo.parseDate(MODIFY_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(MODIFY_DATE),'dd MMM yyyy') #",
                    //width: "8"
                },
                {
                    field: "SYN_RowID",
                    title: "SYN ROWID",
                    //width: "5",
                    hidden: true,
                },
                {
                    //command: [{
                    template: '<a class="edit glyphicon glyphicon-edit" title="Edit" ng-click="doSomething(dataItem.VOUCHER_NO)" style="color:grey;"><span class="sr-only"></span> </a>',
                    //template: '#= redirectEditOrder(ORDER_NO)#',
                    //}],
                    title: " ",
                    width: "40px"
                },
                {
                    template: '<a class="print glyphicon glyphicon-print" title="Double click to Print" ng-dblclick="printdata(formcode,dataItem.VOUCHER_NO)"  style="color:grey;"><span class="sr-only"></span> </a>',

                    title: "Print",
                    width: "40px"
                }
            ],
            //dataBound: function (e) {
            //    
            //    var that = this;
            //    $(that.tbody).on("click", "tr", function (e) {
            //        window.location.href = $(this).find('td:first a').attr('href');
            //    });
            //}

        };
        $scope.bindmodel = function (formcode, orderNo) {



            var formDetail = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetFormDetailSetup?formCode=" + formcode;
            $http({
                method: 'GET',
                url: formDetail,

            }).then(function successCallback(response) {

                angular.forEach(response.data, function (value, key) {
                    if (value.MASTER_CHILD_FLAG == 'M' && value.DISPLAY_FLAG == 'Y') {
                        this.push(value);
                        $scope.masterModelTemplate[value['COLUMN_NAME']] = null;
                    }
                }, $scope.MasterFormElement);
                angular.forEach(response.data, function (value, key) {
                    if (value.MASTER_CHILD_FLAG == 'C' && value.DISPLAY_FLAG == 'Y') {
                        this.push(value);
                        $scope.childModelTemplate[value['COLUMN_NAME']] = null;
                    }
                }, $scope.ChildFormElement[0].element);
                //additional child element reservation.

                angular.forEach(response.data, function (value, key) {

                    if (value.MASTER_CHILD_FLAG == 'C' && value.DISPLAY_FLAG == 'Y') {
                        this.push(value);
                        if (value['COLUMN_NAME'].indexOf('FLAG') > -1) {
                            $scope.childModelTemplate[value['COLUMN_NAME']] = "N";
                            value['CHILD_ELEMENT_WIDTH'] = "50";
                        }
                        else if (value['COLUMN_NAME'].indexOf('MU') > -1) {
                            $scope.childModelTemplate[value['COLUMN_NAME']] = "";
                            value['CHILD_ELEMENT_WIDTH'] = "50";
                        }
                        else if (value['COLUMN_NAME'].indexOf('CODE') > -1) {
                            $scope.childModelTemplate[value['COLUMN_NAME']] = "";
                            value['CHILD_ELEMENT_WIDTH'] = "175";
                            if (value['COLUMN_NAME'] == "ITEM_CODE") {
                                $scope.childModelTemplate.ITEM_EDESC = "";
                            }
                        }
                        else if (value['COLUMN_NAME'].indexOf('QUANTITY') > -1) {
                            $scope.childModelTemplate[value['COLUMN_NAME']] = "";
                            value['CHILD_ELEMENT_WIDTH'] = "50";
                        }
                        else if (value['COLUMN_NAME'].indexOf('PRICE') > -1) {
                            $scope.childModelTemplate[value['COLUMN_NAME']] = "";
                            value['CHILD_ELEMENT_WIDTH'] = "75";
                        }

                        else {
                            $scope.childModelTemplate[value['COLUMN_NAME']] = null;
                        }
                        //$scope.childModelTemplate.SERIAL_NO = null;
                    }
                }, $scope.aditionalChildFormElement);
                d1.resolve(response);
            }, function errorCallback(response) {
            });

        };

        $scope.doSomething = function (orderNo) {
            debugger;

            //showloader();
            var voucherno = orderNo.split(new RegExp('/', 'i')).join('_'); 

            if ($scope.moduleCode == "01")
                window.location.href = "/ProjectManagement/Home/Index#!DT/FinanceVoucher/" + formCode + "/" + voucherno + ""
            else if ($scope.moduleCode === "02")
                window.location.href = "/ProjectManagement/Home/Index#!PM/RequisitionEntry/" + formCode + "/" + voucherno + ""
            else if ($scope.moduleCode == "03")
                window.location.href = "/ProjectManagement/Home/Index#!DT/Inventory/" + formCode + "/" + voucherno + ""
            else if ($scope.moduleCode == "04")
                window.location.href = "/ProjectManagement/Home/Index#!DT/formtemplates/" + formCode + "/" + voucherno + ""
            //window.location.href = "/ProjectManagement/Home/Index#!DT/formtemplate/" + formCode + "/" + voucherNo + "/" + tableName + ""
        }
        //AA
        $scope.printdata = function (formcode, orderNo) {
            alert(1);

            var guestInfo = "/api/TemplateApi/GetCrystalReportTransaction?formCode=" + formcode + "&orderno=" + orderNo;

            $scope.MasterFormElement = []; // for elements having master_child_flag='M'
            $scope.ChildFormElement = [{ element: [] }]; // initial child element
            $scope.aditionalChildFormElement = []; // a blank child element model while add button press.
            $scope.masterModels = {}; // for master model
            $scope.childModels = []; // for dynamic
            $scope.masterModelTemplate = {};
            $scope.childModelTemplate = {};
            $scope.bindmodel(formcode, orderNo);
            $.when(d1, d3).done(function () {

                var reqst = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetSalesOrderDetailFormDetailByFormCodeAndOrderNo?formCode=" + formcode + "&orderno=" + orderNo;

                $http.get(reqst).then(function (responsesod) {
                    var rows = responsesod.data;
                    if (rows.length > 0) {
                        for (var i = 0; i < rows.length; i++) {
                            setDataOnModal(rows, i);
                        }
                    }
                    $scope.PrintDocument(orderNo);
                    $scope.CheckPrintCount();

                    if ($scope.DocumentName == "SA_SALES_ORDER") {
                        $("#saveAndPrintOrderModal").modal("toggle");
                    }
                    if ($scope.DocumentName == "SA_SALES_CHALAN") {

                        $("#saveAndPrintChalanModal").modal("toggle");
                    }
                    if ($scope.DocumentName == "SA_SALES_INVOICE" && ($scope.subDocumentName == "Sales TAX Invoice" || $scope.subDocumentName == "Sales Tax Invoice Manual" || $scope.subDocumentName == "Sales TAX Retail" || $scope.subDocumentName == "Sales TAX Cafe" || $scope.subDocumentName == "Sales TAX Invoice - Shiji")) {

                        setTimeout(function () {
                            $("#saveAndPrintInvoiceModal").modal("toggle");
                        }, 50);
                    } if ($scope.DocumentName == "SA_SALES_INVOICE" && ($scope.subDocumentName == "Sales TAX Invoice opera" || $scope.subDocumentName == "Sales TAX Invoice - Opera")) {
                        setTimeout(function () {
                            $("#saveAndPrintInvoiceOperaModal").modal("toggle");
                        }, 50);
                    }
                    setTimeout(function () {
                        //if ($scope.DocumentName == "SA_SALES_RETURN") {
                        //    $("#saveAndPrintReturnModal").modal("toggle");
                        //}
                        if ($scope.DocumentName == "SA_SALES_RETURN" && ($scope.subDocumentName == "Credit Notes" || $scope.subDocumentName == "Sales Return")) {
                            $("#saveAndPrintReturnModal").modal("toggle");
                        }
                        if ($scope.DocumentName == "SA_SALES_RETURN" && $scope.subDocumentName == "Credit Notes Opera") {
                            $("#saveAndPrintReturnOperaModal").modal("toggle");
                        }
                    }, 50);


                });
            })
            function setDataOnModal(rows, i) {

                var tempCopy = angular.copy($scope.childModelTemplate);
                $scope.ChildFormElement.push({ element: $scope.aditionalChildFormElement });
                $scope.childModels.push($scope.getObjWithKeysFromOtherObj(tempCopy, rows[i]));
                var mastertempCopy = angular.copy($scope.masterModelTemplate);
                var mastercopy = $scope.getObjWithKeysFromOtherObj(mastertempCopy, rows[i]);
                $scope.masterModels = angular.copy(mastercopy);
            }
            var guestInfo = "/api/TemplateApi/GetGuestInfoFromMasterTransaction?formCode=" + formcode + "&orderno=" + orderNo;

            $http.get(guestInfo).then(function (guestInfodetails) {
                ;
                $scope.guestDetails = guestInfodetails.data;
                $scope.ArrivalDate = $scope.guestDetails.CR_LMT1;
                $scope.DepartuteDate = $scope.guestDetails.CR_LMT2;
                $scope.RoomNumber = $scope.guestDetails.CR_LMT3;
                $scope.BillNumber = $scope.guestDetails.REFERENCE_NO;
            });
            $scope.printCompanyInfo = {
                companyName: '',
                address: '',
                formName: '',
                phoneNo: '',
                email: '',
                tPinVatNo: '',
            }
            var fdetails = "/api/TemplateApi/GetFormDetailSetup?formCode=" + formcode;
            $http.get(fdetails).then(function (responsefdetails) {

                $scope.formDetails = responsefdetails.data;
                if ($scope.formDetails.length > 0) {

                    $scope.DocumentName = $scope.formDetails[0].TABLE_NAME;
                    $scope.subDocumentName = $scope.formDetails[0].FORM_EDESC;
                    $scope.companycode = $scope.formDetails[0].COMPANY_CODE;
                    $scope.NEGETIVE_STOCK_FLAG = $scope.formDetails[0].NEGATIVE_STOCK_FLAG;
                    $scope.printCompanyInfo.companyName = $scope.formDetails[0].COMPANY_EDESC;
                    $scope.printCompanyInfo.address = $scope.formDetails[0].ADDRESS;
                    $scope.printCompanyInfo.formName = $scope.formDetails[0].FORM_EDESC;
                    $scope.printCompanyInfo.phoneNo = $scope.formDetails[0].TELEPHONE;
                    $scope.printCompanyInfo.email = $scope.formDetails[0].EMAIL;
                    $scope.printCompanyInfo.tPinVatNo = $scope.formDetails[0].TPIN_VAT_NO;

                }
                d3.resolve(responsefdetails);
            });
            $scope.getObjWithKeysFromOtherObj = function (objKeys, objKeyswithData) {

                var keys = Object.keys(objKeys);
                var result = {};
                for (var i = 0; i < keys.length; i++) {

                    //This was checked in order to round off the value to 2 digit Animesh
                    if (keys[i] == "UNIT_PRICE" || keys[i] == "TOTAL_PRICE")
                        result[keys[i]] = objKeyswithData[keys[i]].toFixed(2);
                    else
                        result[keys[i]] = objKeyswithData[keys[i]];

                }
                return result;
            };
            $scope.PrintDocument = function (orderNo) {



                $scope.ChildFormElement.splice(-1);
                $scope.OrderNo = orderNo;
                var vouch_no = orderNo;

                // Bikalp check why we are using this RefNo
                var getrefnourl = "/api/TemplateApi/getRefNo?orderno=" + vouch_no;
                $http.get(getrefnourl).then(function (responserefno) {

                    if (responserefno.data != null) {
                        $scope.refordernoP = responserefno.data.ORDER_NO;
                        $scope.reforderdateP = moment(responserefno.data.ORDER_DATE).format('DD-MMM-YYYY')
                    }
                });

                //var form_code = $scope.FormCode;


                //$scope.CheckPrintCount();

                $scope.todayDateOpera = $filter('date')(new Date(), 'yyyy-MM-dd');
                $scope.printTodayDateTime = $filter('date')(new Date(), 'yyyy-MM-dd HH:mm:ss');

                var masterelem = $scope.MasterFormElement;
                $.each($scope.MasterFormElement, function (key, value) {

                    if (value['COLUMN_NAME'].indexOf('CODE') > -1) {

                        var switched;
                        switched = value['COLUMN_NAME'];
                        switch (switched) {
                            case 'SUPPLIER_CODE':
                                $scope.masterModels["SUPPLIER_CODE"] = $scope.masterModels["SUPPLIER_CODE"];
                                break;
                            case 'ISSUE_TYPE_CODE':
                                $scope.masterModels["ISSUE_TYPE_CODE"] = $scope.masterModels["ISSUE_TYPE_CODE"];
                                break;
                            case 'TO_BRANCH_CODE':
                                $scope.masterModels["TO_BRANCH_CODE"] = $scope.masterModels["TO_BRANCH_CODE"];
                                break;

                            case "TO_LOCATION_CODE":
                                $scope.masterModels["TO_LOCATION_CODE"] = $scope.masterModels["TO_LOCATION_CODE"];
                                break;
                            case "FROM_LOCATION_CODE":
                                $scope.masterModels["FROM_LOCATION_CODE"] = $scope.masterModels["FROM_LOCATION_CODE"];
                                break;
                            case "MASTER_ACC_CODE":
                                $scope.masterModels["MASTER_ACC_CODE"] = $scope.masterModels["MASTER_ACC_CODE"];
                                break;
                            case "CUSTOMER_CODE":
                                var cinfo = "/api/TemplateApi/GetCustomerInfoByCode?filter=" + $scope.masterModels["CUSTOMER_CODE"];
                                $http.get(cinfo).then(function (results) {


                                    $scope.masterModels["CUSTOMER_CODE"] = results.data[0].CustomerName;
                                    $scope.masterModels["REGD_OFFICE_EADDRESS"] = results.data[0].REGD_OFFICE_EADDRESS;
                                    $scope.TPIN_VAT_NO = results.data[0].TPIN_VAT_NO;
                                    $scope.masterModels["TEL_MOBILE_NO1"] = results.data[0].TEL_MOBILE_NO1;
                                    $scope.masterModels["GuestName"] = results.data[0].GuestName;
                                    $scope.masterModels["TPIN_VAT_NO"] = results.data[0].TPIN_VAT_NO;
                                });



                                break;
                            default:
                        }

                    }

                    $scope.Manual_No_Marriot = $scope.masterModels["MANUAL_NO"];
                });
                var masterArr = $scope.ChildFormElement[0].element;
                var print_master = $.grep(masterArr, function (e) {
                    return (e['COLUMN_NAME'].indexOf("CALC") === -1 && e['COLUMN_NAME'].indexOf("REMARKS") === -1);
                });
                var print_child = [];


                $.each($scope.ChildFormElement, function (ind, it) {

                    print_child.push({
                        element: $.grep(it.element, function (e) {

                            var switch_on;
                            switch_on = e['COLUMN_NAME'];
                            switch (switch_on) {
                                case 'ITEM_CODE':
                                    //var Iinfo = "/api/TemplateApi/getItemEdesc?code=" + $scope.childModels[ind]["ITEM_CODE"];
                                    //$http.get(Iinfo).then(function (results) {


                                    //    $scope.childModels[ind]["ITEM_CODE"] = results.data;

                                    //});
                                    $scope.childModels[ind]["ITEM_CODE"] = $scope.childModels[ind]["ITEM_EDESC"];
                                    break;
                                case 'PRODUCT_CODE':
                                    $scope.childModels[ind]["ITEM_CODE"] = $scope.childModels[ind]["ITEM_CODE"];
                                    break;
                                case 'ACC_CODE':
                                    $scope.childModels[ind]["ACC_CODE"] = $scope.childModels[ind]["ACC_CODE"];
                                    break;

                                case "TO_LOCATION_CODE":
                                    $scope.childModels[ind]["TO_LOCATION_CODE"] = $scope.childModels[ind]["TO_LOCATION_CODE"];
                                    break;
                                case "FROM_LOCATION_CODE":
                                    $scope.childModels[ind]["FROM_LOCATION_CODE"] = $scope.childModels[ind]["FROM_LOCATION_CODE"];
                                    break;
                                default:
                            }
                            return (e['COLUMN_NAME'].indexOf("CALC") === -1 && e['COLUMN_NAME'].indexOf("REMARKS") === -1);
                        }),
                        additionalElements: ''
                    });
                });

                if ($scope.DocumentName == "SA_SALES_CHALAN") {

                    for (var i = 0; i < $scope.childModels.length; i++) {
                        if ("FROM_LOCATION_CODE" in $scope.childModels[i]) {
                            $scope.childModels[i]["FROM_LOCATION_CODE"] = undefined;
                        }
                        if ("CALC_QUANTITY" in $scope.childModels[i]) {
                            $scope.childModels[i]["CALC_QUANTITY"] = undefined;
                        }
                        if ("CALC_TOTAL_PRICE" in $scope.childModels[i]) {
                            $scope.childModels[i]["CALC_TOTAL_PRICE"] = undefined;
                        }
                        if ("CALC_UNIT_PRICE" in $scope.childModels[i]) {
                            $scope.childModels[i]["CALC_UNIT_PRICE"] = undefined;
                        }
                        if ("STOCK_BLOCK_FLAG" in $scope.childModels[i]) {
                            $scope.childModels[i]["STOCK_BLOCK_FLAG"] = undefined;
                        }
                        if ("COMPLETED_QUANTITY" in $scope.childModels[i]) {
                            $scope.childModels[i]["COMPLETED_QUANTITY"] = undefined;
                        }
                        if ("TOTAL_PRICE" in $scope.childModels[i]) {
                            $scope.childModels[i]["TOTAL_PRICE"] = undefined;
                        }
                        if ("UNIT_PRICE" in $scope.childModels[i]) {
                            $scope.childModels[i]["UNIT_PRICE"] = undefined;
                        }

                    };
                }
                else {

                    for (var i = 0; i < $scope.childModels.length; i++) {
                        if ("FROM_LOCATION_CODE" in $scope.childModels[i]) {
                            $scope.childModels[i]["FROM_LOCATION_CODE"] = undefined;
                        }
                        if ("CALC_QUANTITY" in $scope.childModels[i]) {
                            $scope.childModels[i]["CALC_QUANTITY"] = undefined;
                        }
                        if ("CALC_TOTAL_PRICE" in $scope.childModels[i]) {
                            $scope.childModels[i]["CALC_TOTAL_PRICE"] = undefined;
                        }
                        if ("CALC_UNIT_PRICE" in $scope.childModels[i]) {
                            $scope.childModels[i]["CALC_UNIT_PRICE"] = undefined;
                        }
                        if ("STOCK_BLOCK_FLAG" in $scope.childModels[i]) {
                            $scope.childModels[i]["STOCK_BLOCK_FLAG"] = undefined;
                        }
                        if ("COMPLETED_QUANTITY" in $scope.childModels[i]) {
                            $scope.childModels[i]["COMPLETED_QUANTITY"] = undefined;
                        }
                        if ("ALT1_MU_CODE" in $scope.childModels[i]) {
                            $scope.childModels[i]["ALT1_MU_CODE"] = undefined;
                        }
                        if ("ALT1_QUANTITY" in $scope.childModels[i]) {
                            $scope.childModels[i]["ALT1_QUANTITY"] = undefined;
                        }
                        if ("SECOND_QUANTITY" in $scope.childModels[i]) {
                            $scope.childModels[i]["SECOND_QUANTITY"] = undefined;
                        }


                    };
                }


                $scope.childModels = JSON.parse(JSON.stringify($scope.childModels));

                $scope.print_header = print_master;
                $scope.print_body_col = print_child;
                if ($scope.DocumentName == "SA_SALES_CHALAN") {

                    for (var i = 0; i < $scope.print_body_col.length; i++) {

                        $scope.print_body_col[i].element = $.grep($scope.print_body_col[i].element, function (v) { return ($.inArray(v.COLUMN_NAME, ["FROM_LOCATION_CODE", "STOCK_BLOCK_FLAG", "COMPLETED_QUANTITY", "TOTAL_PRICE", "UNIT_PRICE"]) == -1) });

                    }
                }
                else {
                    for (var i = 0; i < $scope.print_body_col.length; i++) {

                        $scope.print_body_col[i].element = $.grep($scope.print_body_col[i].element, function (v) { return ($.inArray(v.COLUMN_NAME, ["FROM_LOCATION_CODE", "STOCK_BLOCK_FLAG", "COMPLETED_QUANTITY", "ALT1_MU_CODE", "ALT1_QUANTITY", "SECOND_QUANTITY"]) == -1) });

                    }
                }

                if ($scope.PrintDiscount > 0) {
                    $scope.PrintDiscountShow = true;
                }
                $scope.TOTAL_QUANTITY = 0;
                $scope.SUB_TOTAL = 0.00;
                $.each($scope.childModels, function (indN, itN) {

                    $scope.TOTAL_QUANTITY = $scope.TOTAL_QUANTITY + itN.QUANTITY;
                    //$scope.SUB_TOTAL = $scope.SUB_TOTAL + itN.TOTAL_PRICE; This was previously used Animesh
                    $scope.SUB_TOTAL = $scope.SUB_TOTAL + parseFloat(itN.TOTAL_PRICE);
                });
                debugger;
                $scope.SUB_TOTAL = parseFloat($scope.SUB_TOTAL).toFixed(2);
                //$scope.SUB_TOTAL = parseFloat($scope.SUB_TOTAL.toFixed(2));
                $scope.print_body_col = JSON.parse(JSON.stringify($scope.print_body_col));

                $scope.TodayDate = AD2BS(moment($("#englishdatedocument").val()).format('YYYY-MM-DD'))

                var chargeUrl = "/api/TemplateApi/GetChargeData?formCode=" + $scope.formcode;
                $http.get(chargeUrl).then(function (responsecharge) {

                    $scope.ChargeList = responsecharge.data;
                    var chargeUrlForEdit = "/api/TemplateApi/GetChargeDataForEdit?formCode=" + formcode + "&&voucherNo=" + orderNo;
                    $http.get(chargeUrlForEdit).then(function (reschargeUrlForEdit) {

                        //var var1 = angular.element(document).find('@ConfigurationManager.AppSettings["IsRetailBusiness"]');
                        /*  console.log(var1);*/
                        if (reschargeUrlForEdit.data != undefined) {
                            if (reschargeUrlForEdit.data.length > 0) {
                                $scope.data = reschargeUrlForEdit.data;
                                $.each(reschargeUrlForEdit.data, function (it, val) {
                                    $.each($scope.ChargeList, function (i, v) {

                                        if (val.CHARGE_CODE === v.CHARGE_CODE) {
                                            val.CHARGE_ACTIVE_FLAG = v.CHARGE_ACTIVE_FLAG;
                                            if (v.CHARGE_ACTIVE_FLAG == "Y") {
                                                v.CHARGE_AMOUNT = val.CHARGE_AMOUNT.toFixed(2);
                                            }
                                            else {
                                                v.CHARGE_AMOUNT = 0.00;

                                            }
                                            if (val.VALUE_PERCENT_FLAG == "P") {
                                                val.VALUE_PERCENT_AMOUNT = ((val.CHARGE_AMOUNT * 100) / $scope.SUB_TOTAL).toFixed(2);
                                            }
                                            else {
                                            }
                                            v.ACC_CODE = val.ACC_CODE;
                                        }
                                    });
                                });

                                $scope.calculateChargeAmount1($scope.data, true);



                            }
                        }




                    });

                });





                $scope.calculateChargeAmount1 = function (dataList, bool) {


                    var totalAddition = 0;
                    var totalDeduction = 0;
                    var netTotal = 0;

                    $.each(dataList, function (i, val) {

                        if (val.CHARGE_TYPE_FLAG == "D") {

                            var Deduction = parseFloat(val.CHARGE_AMOUNT);

                            totalDeduction += Deduction;
                        }
                        else {
                            //AA Checking if the flag is active and if the flag is active then the charge is added else not added
                            if (val.CHARGE_ACTIVE_FLAG == "Y") {
                                var Addition = parseFloat(val.CHARGE_AMOUNT);
                                totalAddition += Addition;
                            }
                            //$scope.VALUE_PERCENT_AMOUNT[i] = (val.CHARGE_AMOUNT * 100) / $scope.summary.grandTotal;
                        }
                    });

                    //$scope.deduction = isNaN(totalDeduction) ? 0 : totalDeduction.toFixed(2);
                    //$scope.addition = isNaN(totalAddition) ? 0 : totalAddition.toFixed(2);

                    var tded = isNaN(totalDeduction) ? 0 : totalDeduction.toFixed(2);
                    var tadd = isNaN(totalAddition) ? 0 : totalAddition.toFixed(2);
                    netTotal = parseFloat(parseFloat($scope.SUB_TOTAL) + parseFloat(tadd) - parseFloat(tded)).toFixed(2);

                    $scope.adtotal = isNaN(netTotal) ? 0 : netTotal;
                    $scope.amountinword = convertNumberToWords($scope.adtotal);

                    if (tded > $scope.tadd) {

                        displayPopupNotification("Deduction amount is greater than total amount.", "warning");
                    }
                    else {


                        $scope.deduction = isNaN(totalDeduction) ? 0 : totalDeduction.toFixed(2);
                        $scope.addition = isNaN(totalAddition) ? 0 : totalAddition.toFixed(2);
                    }

                }


                //$("#saveAndPrintModal").modal("toggle");


            }
            $scope.printDiv1 = function (divName) {
                debugger;
                //$scope.CheckPrintCount();
                var vouch_no = orderNo;
                var updateprinturl = "/api/TemplateApi/UpdatePrintCount?VoucherNo=" + vouch_no + "&formcode=" + formcode;
                var response = $http({
                    method: "POST",
                    url: updateprinturl,
                    contentType: "application/json",
                    dataType: "json"
                });
                return response.then(function (data) {
                    debugger;
                    //if (data.data.MESSAGE == "SUCCESS") {
                    $.when(d2).done(function () {
                        debugger;
                        var vouch_no = orderNo;
                        //var vouch_no = $scope.OrderNo;
                        var getprintcounturl = "/api/TemplateApi/GetPrintCountByVoucherNo?voucherno=" + vouch_no + "&formcode=" + formcode;
                        $http.get(getprintcounturl).then(function (responseprintcount) {
                            debugger;
                            if ($scope.DocumentName === "SA_SALES_RETURN") {
                                $("#TaxINvoiceTest").html("CREDIT NOTE");
                                var printContents = document.getElementById(divName).innerHTML;

                                var popupWin = window.open('', '_blank', 'width=800,height=800', 'orientation = portrait');
                                popupWin.document.open();
                                popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
                                popupWin.document.close();
                            }
                            else if ($scope.DocumentName === "SA_SALES_INVOICE") {
                                if (responseprintcount.data > 1) {
                                    var printContents = document.getElementById(divName).innerHTML;

                                    var popupWin = window.open('', '_blank', 'width=800,height=800', 'orientation = portrait');
                                    popupWin.document.open();
                                    popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
                                    popupWin.document.close();
                                }
                                else {

                                    for (var i = 0; i < 3; i++) {
                                        debugger;
                                        if (i == 0) {
                                            var a = document.getElementById(TaxINvoiceTest)
                                            $("h4#TaxINvoiceTest").html("TAX INVOICE");
                                            $("h5#TaxInvoiceCopy").html("Customer Copy");
                                            //$("#TaxINvoiceTest").html("TAX INVOICE");
                                            //$("#TaxInvoiceCopy").html("Customer Copy");
                                        }
                                        else if (i == 1) {
                                            $("h4#TaxINvoiceTest").html("INVOICE");
                                            $("h5#TaxInvoiceCopy").html("Office Copy");

                                            //$("#TaxINvoiceTest").html("INVOICE");
                                            //$("#TaxInvoiceCopy").html("Office Copy");
                                        }
                                        else {
                                            $("h4#TaxINvoiceTest").html("INVOICE");
                                            $("h5#TaxInvoiceCopy").html("Tax Copy");

                                            //$("#TaxINvoiceTest").html("INVOICE");
                                            //$("#TaxInvoiceCopy").html("Tax Copy");
                                        }
                                        var printContents = document.getElementById(divName).innerHTML;

                                        var popupWin = window.open('', '_blank', 'width=800,height=800', 'orientation = portrait');
                                        popupWin.document.open();
                                        debugger;
                                        popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
                                        popupWin.document.close();

                                    }
                                }
                            }
                            else if ($scope.DocumentName === "SA_SALES_ORDER") {
                                $("#TaxINvoiceTest").html("SALES ORDER");
                                var printContents = document.getElementById(divName).innerHTML;

                                var popupWin = window.open('', '_blank', 'width=800,height=800', 'orientation = portrait');
                                popupWin.document.open();
                                popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
                                popupWin.document.close();
                            }
                            else {
                                $("#TaxINvoiceTest").html("SALES CHALAN");
                                var printContents = document.getElementById(divName).innerHTML;

                                var popupWin = window.open('', '_blank', 'width=800,height=800', 'orientation = portrait');
                                popupWin.document.open();
                                popupWin.document.write('<html><body onload="window.print()">' + printContents + '</body></html>');
                                popupWin.document.close();
                            }
                        });
                        debugger;
                        //$("#OperaPrintModal").modal("toggle");
                        if ($scope.DocumentName == "SA_SALES_ORDER") {
                            $("#saveAndPrintOrderModal").modal("toggle");
                        }
                        if ($scope.DocumentName == "SA_SALES_CHALAN") {
                            $("#saveAndPrintChalanModal").modal("toggle");
                        }

                        if ($scope.DocumentName == "SA_SALES_INVOICE" && ($scope.subDocumentName == "Sales TAX Invoice" || $scope.subDocumentName == "Sales TAX Retail")) {

                            if ($scope.DocumentName == "SA_SALES_INVOICE" && ($scope.subDocumentName == "Sales TAX Invoice" || $scope.subDocumentName == "Sales Tax Invoice Manual" || $scope.subDocumentName == "Sales TAX Retail" || $scope.subDocumentName == "Sales TAX Cafe")) {

                                $("#saveAndPrintInvoiceModal").modal("toggle");

                            }
                            if ($scope.DocumentName == "SA_SALES_INVOICE" && $scope.subDocumentName == "Sales TAX Invoice opera") {
                                $("#saveAndPrintInvoiceOperaModal").modal("toggle");

                            }
                            //if ($scope.DocumentName == "SA_SALES_INVOICE") {
                            //    $("#saveAndPrintInvoiceModal").modal("toggle");
                            //}
                            if ($scope.DocumentName == "SA_SALES_RETURN" && ($scope.subDocumentName == "Credit Notes" || $scope.subDocumentName == "Sales Return")) {
                                $("#saveAndPrintReturnModal").modal("toggle");
                            }
                            if ($scope.DocumentName == "SA_SALES_RETURN" && $scope.subDocumentName == "Credit Notes Opera") {
                                $("#saveAndPrintReturnOperaModal").modal("toggle");
                            }
                        };
                        //}
                    });
                })
            };
            //$scope.CheckPrintCount = function () {
            //    debugger;

            //    var vouch_no = orderNo;
            //    var updateprinturl = "/api/TemplateApi/UpdatePrintCount?VoucherNo=" + vouch_no + "&formcode=" + formcode;
            //    var response = $http({
            //        method: "POST",
            //        url: updateprinturl,
            //        contentType: "application/json",
            //        dataType: "json"
            //    });
            //    return response.then(function (data) {
            //        debugger;
            //        if (data.data.MESSAGE == "SUCCESS") {

            //            var getprintcounturl = "/api/TemplateApi/GetPrintCountByVoucherNo?voucherno=" + vouch_no + "&formcode=" + formcode;
            //            $http.get(getprintcounturl).then(function (responseee) {
            //                debugger;

            //                if ($scope.DocumentName === "SA_SALES_RETURN") {
            //                    $scope.taxinvoice = "Credit Note";
            //                }
            //                else {
            //                    if (responseee.data > 1) {
            //                        $scope.printcounttext = "Copy of Original" + "(" + (responseee.data - 1) + ")";
            //                        $scope.taxinvoice = "INVOICE";
            //                    }
            //                    else {
            //                        $scope.taxinvoice = "TAX INVOICE";
            //                        $scope.printcounttext = "";
            //                        $scope.taxinvoice1 = "";
            //                    }
            //                }
            //                d2.resolve(responseee);
            //            });
            //        }
            //    });

            //};

            //AA
            $scope.CheckPrintCount = function () {

                //var vouch_no = $scope.dzvouchernumber.replace(/_/g, '/');
                var vouch_no = orderNo;
                var getprintcounturl = "/api/TemplateApi/GetPrintCountByVoucherNo?voucherno=" + vouch_no + "&formcode=" + formcode;
                $http.get(getprintcounturl).then(function (responseee) {


                    if ($scope.DocumentName === "SA_SALES_RETURN") {
                        $scope.taxinvoice = "Credit Note";
                    }
                    else {
                        if (responseee.data > 0) {
                            $scope.printcounttext = "Copy of Original" + "(" + (responseee.data) + ")";
                            $scope.taxinvoice = "INVOICE";
                            $scope.taxinvoice1 = "";
                        }
                        else {
                            $scope.taxinvoice = "TAX INVOICE";
                            $scope.printcounttext = "";
                            $scope.taxinvoice1 = "";
                        }
                    }
                    d2.resolve(responseee);
                });

            };
            $scope.getNepaliDate = function (date) {
                return AD2BS(moment(date).format('YYYY-MM-DD'));
            };
            $scope.cnlPrint = function () {

            };


        }


    }
    //function convertNumberToWords(amount) {

    //    var words = new Array();
    //    words[0] = '';
    //    words[1] = 'One';
    //    words[2] = 'Two';
    //    words[3] = 'Three';
    //    words[4] = 'Four';
    //    words[5] = 'Five';
    //    words[6] = 'Six';
    //    words[7] = 'Seven';
    //    words[8] = 'Eight';
    //    words[9] = 'Nine';
    //    words[10] = 'Ten';
    //    words[11] = 'Eleven';
    //    words[12] = 'Twelve';
    //    words[13] = 'Thirteen';
    //    words[14] = 'Fourteen';
    //    words[15] = 'Fifteen';
    //    words[16] = 'Sixteen';
    //    words[17] = 'Seventeen';
    //    words[18] = 'Eighteen';
    //    words[19] = 'Nineteen';
    //    words[20] = 'Twenty';
    //    words[30] = 'Thirty';
    //    words[40] = 'Forty';
    //    words[50] = 'Fifty';
    //    words[60] = 'Sixty';
    //    words[70] = 'Seventy';
    //    words[80] = 'Eighty';
    //    words[90] = 'Ninety';
    //    amount = amount.toString();
    //    var atemp = amount.split(".");
    //    var number = atemp[0].split(",").join("");
    //    var n_length = number.length;
    //    var words_string = "";
    //    if (n_length <= 9) {
    //        var n_array = new Array(0, 0, 0, 0, 0, 0, 0, 0, 0);
    //        var received_n_array = new Array();
    //        for (var i = 0; i < n_length; i++) {
    //            received_n_array[i] = number.substr(i, 1);
    //        }
    //        for (var i = 9 - n_length, j = 0; i < 9; i++ , j++) {
    //            n_array[i] = received_n_array[j];
    //        }
    //        for (var i = 0, j = 1; i < 9; i++ , j++) {
    //            if (i == 0 || i == 2 || i == 4 || i == 7) {
    //                if (n_array[i] == 1) {
    //                    n_array[j] = 10 + parseInt(n_array[j]);
    //                    n_array[i] = 0;
    //                }
    //            }
    //        }
    //        value = "";
    //        for (var i = 0; i < 9; i++) {
    //            if (i == 0 || i == 2 || i == 4 || i == 7) {
    //                value = n_array[i] * 10;
    //            } else {
    //                value = n_array[i];
    //            }
    //            if (value != 0) {
    //                words_string += words[value] + " ";
    //            }
    //            if ((i == 1 && value != 0) || (i == 0 && value != 0 && n_array[i + 1] == 0)) {
    //                words_string += "Crores ";
    //            }
    //            if ((i == 3 && value != 0) || (i == 2 && value != 0 && n_array[i + 1] == 0)) {
    //                words_string += "Lakhs ";
    //            }
    //            if ((i == 5 && value != 0) || (i == 4 && value != 0 && n_array[i + 1] == 0)) {
    //                words_string += "Thousand ";
    //            }
    //            if (i == 6 && value != 0 && (n_array[i + 1] != 0 && n_array[i + 2] != 0)) {
    //                words_string += "Hundred and ";
    //            } else if (i == 6 && value != 0) {
    //                words_string += "Hundred ";
    //            }
    //        }
    //        words_string = words_string.split("  ").join(" ");
    //    }
    //    return words_string;
    //}

    function convertNumberToWords(amount) {

        var finalWord1 = test_value(amount);
        var finalWord2 = "";

        var val = amount;
        var actual_val = amount;
        amount = val;

        if (val.indexOf('.') != -1) {
            val = val.substring(val.indexOf('.') + 1, val.length);
            if (val.length == 0 || val == '00') {
                finalWord2 = "paisa zero";
            }
            else {
                amount = val;
                finalWord2 = "paisa" + test_value(amount);
            }
            return finalWord1 + " Rupees and " + finalWord2;
        }
        else {
            //finalWord2 =  " Zero paisa only";
            return finalWord1 + " Rupees";
        }
        amount = actual_val;
    }

    function test_value(amount) {

        var junkVal = amount;
        junkVal = Math.floor(junkVal);
        var obStr = new String(junkVal);
        numReversed = obStr.split("");
        actnumber = numReversed.reverse();


        if (Number(junkVal) == 0) {
            return obStr + '' + 'Rupees Zero';

        }


        var iWords = ["Zero", " One", " Two", " Three", " Four", " Five", " Six", " Seven", " Eight", " Nine"];
        var ePlace = ['Ten', ' Eleven', ' Twelve', ' Thirteen', ' Fourteen', ' Fifteen', ' Sixteen', ' Seventeen', ' Eighteen', 'Nineteen'];
        var tensPlace = ['dummy', ' Ten', ' Twenty', ' Thirty', ' Forty', ' Fifty', ' Sixty', ' Seventy', ' Eighty', ' Ninety'];

        var iWordsLength = numReversed.length;
        var totalWords = "";
        var inWords = new Array();
        var finalWord = "";
        j = 0;
        for (i = 0; i < iWordsLength; i++) {
            switch (i) {
                case 0:
                    if (actnumber[i] == 0 || actnumber[i + 1] == 1) {
                        inWords[j] = '';
                    }
                    else {
                        inWords[j] = iWords[actnumber[i]];
                    }
                    inWords[j] = inWords[j];
                    break;
                case 1:
                    tens_complication();
                    break;
                case 2:
                    if (actnumber[i] == 0) {
                        inWords[j] = '';
                    }
                    else if (actnumber[i - 1] != 0 && actnumber[i - 2] != 0) {
                        inWords[j] = iWords[actnumber[i]] + ' Hundred and';
                    }
                    else {
                        inWords[j] = iWords[actnumber[i]] + ' Hundred';
                    }
                    break;
                case 3:
                    if (actnumber[i] == 0 || actnumber[i + 1] == 1) {
                        inWords[j] = '';
                    }
                    else {
                        inWords[j] = iWords[actnumber[i]];
                    }
                    if (actnumber[i + 1] != 0 || actnumber[i] > 0) { //here
                        inWords[j] = inWords[j] + " Thousand";
                    }
                    break;
                case 4:
                    tens_complication();
                    break;
                case 5:
                    if (actnumber[i] == "0" || actnumber[i + 1] == 1) {
                        inWords[j] = '';
                    }
                    else {
                        inWords[j] = iWords[actnumber[i]];
                    }
                    if (actnumber[i + 1] != 0 || actnumber[i] > 0) {   //here 
                        inWords[j] = inWords[j] + " Lakh";
                    }

                    break;
                case 6:
                    tens_complication();
                    break;
                case 7:
                    if (actnumber[i] == "0" || actnumber[i + 1] == 1) {
                        inWords[j] = '';
                    }
                    else {
                        inWords[j] = iWords[actnumber[i]];
                    }
                    if (actnumber[i + 1] != 0 || actnumber[i] > 0) { // changed here
                        inWords[j] = inWords[j] + " Crore";
                    }
                    break;
                case 8:
                    tens_complication();
                    break;
                default:
                    break;
            }
            j++;
        }
        function tens_complication() {
            if (actnumber[i] == 0) {
                inWords[j] = '';
            }
            else if (actnumber[i] == 1) {
                inWords[j] = ePlace[actnumber[i - 1]];
            }
            else {
                inWords[j] = tensPlace[actnumber[i]];
            }
        }
        inWords.reverse();
        for (i = 0; i < inWords.length; i++) {
            finalWord += inWords[i];
        }
        return finalWord;
    }


    function ExportToExcel(e) {
        debugger;
        var SheetRow = [];

        //Pushing the head row
        SheetRow.push({
            cells: [
                { value: "Document No.", background: "#A9A7A6", },
                { value: "Date", background: "#A9A7A6", },
                { value: "Amount", background: "#A9A7A6", },
                { value: "Manual No.", background: "#A9A7A6", },
                { value: "Prepared By", background: "#A9A7A6", },
                { value: "Prepared Date & Time", background: "#A9A7A6", },
                { value: "Modified Date", background: "#A9A7A6", },
            ],
        });
        WriteData(e.data);
        //recursive function to write the grouped/ungrouped data
        function WriteData(array) {
            debugger;
            array.forEach(function (row, index) {

                if (typeof (row.items) != "undefined") {
                    //if array contains nested items, write a row with group field and enter recursion
                    SheetRow.push({
                        cells: [{
                            value: row.field + " : " + row.value,
                            background: "#E1E1E1",
                            colSpan: 16,//to span the total number of columns
                            fontSize: 12,
                        }]
                    });
                    WriteData(row.items);
                }
                else { //if array contains no nested items write the row to excelsheet
                    SheetRow.push({
                        cells: [{
                            value: row.VOUCHER_NO,
                        }, {
                            value: row.VOUCHER_DATE,
                        }, {
                            value: row.VOUCHER_AMOUNT,
                        }, {
                            value: row.REFERENCE_NO,
                        }, {
                            value: row.CREATED_BY,
                        }, {
                            value: row.CREATED_DATE,
                        }, {
                            value: row.MODIFY_DATE,
                        }]
                    });
                }
            });
        }
        debugger;
        //var Workbook = new kendo.ooxml.Workbook({
        //    sheets: [{
        //        columns: [
        //            { width: 110 }
        //        ],
        //        rows: SheetRow
        //    }]
        //});
        var workbook = new kendo.ooxml.Workbook({
            sheets: [{
                columns: [
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true }
                ],
                rows: SheetRow
            }]
        });
        //finally saving the excel sheet
        kendo.saveAs({

            dataURI: workbook.toDataURL(),
            fileName: "Document Finder.xlsx"
        });

    };
    //$scope.onsiteSearch = function ($this) {
    //    
    //    var q = $("#txtSearchString").val();
    //    var grid = $("#kGrid").data("kendogrid");
    //    grid.dataSource.query({
    //        page: 1,
    //        pageSize: 50,
    //        filter: {
    //            logic: "or",
    //            filters: [
    //              { field: "VOUCHER_NO", operator: "contains", value: q },
    //              { field: "VOUCHER_DATE", operator: "contains", value: q },
    //              { field: "VOUCHER_AMOUNT", operator: "contains", value: q },
    //              { field: "CREATED_BY", operator: "contains", value: q },
    //              { field: "CREATED_DATE", operator: "contains", value: q },
    //              { field: "CHECKED_BY", operator: "contains", value: q },
    //              { field: "CHECKED_DATE", operator: "contains", value: q },
    //              { field: "AUTHORISED_BY", operator: "contains", value: q },
    //              { field: "POSTED_DATE", operator: "contains", value: q },
    //              { field: "MODIFY_DATE", operator: "contains", value: q },
    //              { field: "SYN_RowID", operator: "contains", value: q },




    //            ]
    //        }
    //    });
    //};




    //$scope.setHeight = function () {
    //    kendo.resize($($scope.splitter.wrapper[0]));
    //}
}
);
