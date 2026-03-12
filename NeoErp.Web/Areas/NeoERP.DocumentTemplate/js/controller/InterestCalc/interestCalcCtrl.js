DTModule.controller('interestCalcCtrl', function ($scope, InterestCalcService, $http, $routeParams, $window, $filter) {
    debugger;
    $scope.FormName = "Interest Calculator";
    checkedItems = [];
    $scope.InterestPageStart = "Y";
    $scope.InterestCount = "N";
    $scope.InterestLogCount = "N";
    $scope.bulkGroupAccModel = [];
    $scope.InterestCalculate = {
        RATE: "",
        CUSTOMER_CODE: "",
        FROM_DATE: "",
        TO_DATE: "",
        COMPANY_CODE: "",
        BRANCH_CODE: "",
        GROUP_CODES: null,
        


    };
    $scope.InterestImpactCalc = {
        FORM_CODE: "",
        LEDGER_CODE: "",
        ACCOUNT_CODE: "",
        VOUCHER_DATE: "",
        COMPANY_CODE: "",
        BRANCH_CODE: "",
        CHARGE_ACCOUNT_CODE: ""
       

    };
    var customerUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetAllCustomerForScheme";
    $scope.customerDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: customerUrl,
            }
        }
    });
    $scope.customerOptions = {
        dataSource: $scope.customerDataSource,
        dataTextField: "CUSTOMER_EDESC",
        dataValueField: "CUSTOMER_CODE",
        filter: "contains",
        change: function (e) {
            debugger;
            if ($scope.InterestCalculate.CUSTOMER_CODE.length > 0) {
                //var groupAccCode = _.pluck($scope.InterestCalculate.GROUP_CODES, 'ACC_CODE').join(','); 
                //$('#accIdGroup').attr("disabled", true);
                var multiselect = $("#accIdGroup").data("kendoMultiSelect");
                multiselect.readonly(true);

            }
            else {
                //$('#accIdGroup').attr("disabled", false);
                var multiselect = $("#accIdGroup").data("kendoMultiSelect");
                multiselect.readonly(flase);
            }

        },
        dataBound: function () {
        }
    };
    var cCodes="";
    var interestCalccustomerUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetCustomersForInterestCalc?acccodes=" + cCodes;
    $scope.interestCalccustomerDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: interestCalccustomerUrl,
            }
        }
    });
    $scope.interestCalccustomerOptions = {
        dataSource: $scope.interestCalccustomerDataSource,
        dataTextField: "CUSTOMER_EDESC",
        dataValueField: "CUSTOMER_CODE",
        filter: "contains",
        change: function (e) {
            debugger;

        },
        dataBound: function () {
        }
    };
    var groupAccUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetAllAccountForIntrestCalc";
    $scope.groupAccDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: groupAccUrl,
            }
        }
    });
    $scope.groupAccOptions = {
        dataSource: $scope.groupAccDataSource,
        dataTextField: "ACC_EDESC",
        dataValueField: "ACC_CODE",
        filter: "contains",
        change: function (e) {
            debugger;
            if ($scope.InterestCalculate.GROUP_CODES.length > 0) {
                //var groupAccCode = _.pluck($scope.InterestCalculate.GROUP_CODES, 'ACC_CODE').join(','); 
                $('#customer').attr("disabled", true);

            }
            else {
                $('#customer').attr("disabled", false);
            }
           

        },
        dataBound: function () {
        }
    };
    var documentUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetDocumentCode";
    $scope.documentDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: documentUrl,
            }
        }
    });
    $scope.documentOptions = {
        dataSource: $scope.documentDataSource,
        dataTextField: "FORM_EDESC",
        dataValueField: "FORM_CODE",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {
        }
    };






    $scope.docDataSource = {
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        type: "GET",
        serverFiltering: true,
        suggest: true,
        highlightFirst: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetDocumentByFilter",

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
    };

    $scope.docCodeOption = {
        dataSource: $scope.docDataSource,
        template: '<span>{{dataItem.FORM_EDESC}}</span>',
        dataBound: function (e) {

            //$scope.tempCustomerDataSource = $("#customers").data('kendoComboBox').dataSource.data();
            //if (this.element[0].attributes['customer-index'] == undefined) {
            //    var customer = $("#customers").data("kendoComboBox");
            //}
            //else {
            //    var index = this.element[0].attributes['customer-index'].value;
            //    var customerLength = ((parseInt(index) + 1) * 3) - 1;
            //    var customer = $($(".customer")[customerLength]).data("kendoComboBox");

            //}
            //if (customer != undefined) {
            //    customer.setOptions({
            //        template: $.proxy(kendo.template("#= formatValue(CustomerName, Type, this.text()) #"), customer)
            //    });
            //}
            //$scope.$apply();
        },
        select: function (e) {
            //debugger;
            //var currentCustomer = e.dataItem.CustomerCode;
            //if (currentCustomer !== undefined) {
            //    BindDealer(currentCustomer, "");
            //    BindPriceList(currentCustomer, "");
            //}

        }
    }


    var accUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetAllAccountForBud";
    $scope.accDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: accUrl,
            }
        }
    });
    $scope.accOptions = {
        dataSource: $scope.accDataSource,
        dataTextField: "ACC_EDESC",
        dataValueField: "ACC_CODE",
        filter: "contains",
        change: function (e) {
            debugger;
            var currentItem = e.sender.dataItem(e.node);

        },
        dataBound: function () {
        }
    };
    var branchUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getBranchCodeforScheme";
    $scope.branchDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: branchUrl,
            }
        }
    });
    $scope.branchOptions = {
        dataSource: $scope.branchDataSource,
        dataTextField: "BRANCH_EDESC",
        dataValueField: "BRANCH_CODE",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {
        }
    };
    var accChargeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetAllAccountForBud";
    $scope.accChargeDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: accChargeUrl,
            }
        }
    });
    $scope.ChargeAccOptions = {
        dataSource: $scope.accChargeDataSource,
        dataTextField: "ACC_EDESC",
        dataValueField: "ACC_CODE",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {
        }
    };
    var CompanyUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetCompanyList";
    $scope.companyDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: CompanyUrl,
            }
        }
    });
    $scope.companyOptions = {
        dataSource: $scope.companyDataSource,
        dataTextField: "COMPANY_EDESC",
        dataValueField: "COMPANY_CODE",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {
        }
    };
    $scope.userControl = {
        smartButtonMaxItems: 3,
        enableSearch: true,
        smartButtonTextConverter: function (itemText, originalItem) {
            return itemText;
        }
    };
    $scope.uptodateSelectorOptions = {
        open: function () {

            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() + 100);
        },
        change: function () {

            //var id = this.element.attr('id');
            //$scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'), id)
        },
        format: "dd-MMM-yyyy",

        // specifies that DateInput is used for masking the input element
        dateInput: true
    };
    $scope.voucherdateSelectorOptions = {
        open: function () {

            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() + 100);
        },
        change: function () {
            
            
            //var id = this.element.attr('id');
            //$scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'), id)
        },
        format: "dd-MMM-yyyy",

        // specifies that DateInput is used for masking the input element
        dateInput: true
    };
    $scope.calculateInterest = function () {
        debugger;
        checkedItems = [];
        var groupAccCode = "";
        var uotodate = $scope.InterestCalculate.UPTO_DATE;
        if ($scope.InterestCalculate.GROUP_CODES != null) {
            if ($scope.InterestCalculate.GROUP_CODES.length > 0) {

                 groupAccCode = _.pluck($scope.InterestCalculate.GROUP_CODES, 'ACC_CODE').join(',');
            }
        }
        
        if ($scope.InterestCalculate.RATE == "") {
            displayPopupNotification("Rate is required", "error");
            return;
        }
        //if ($scope.InterestCalculate.CUSTOMER_CODE == "") {
        //    displayPopupNotification("Customer is required", "error");
        //    return;
        //}
        if ($scope.InterestCalculate.UPTO_DATE == "") {
            displayPopupNotification("Upto date is required", "error");
            return;
        }
        if (uotodate!= "") {
            var GivenDate = $scope.InterestCalculate.UPTO_DATE;
            var CurrentDate = new Date();
            GivenDate = new Date(GivenDate);
            if (GivenDate > CurrentDate) {
                displayPopupNotification("Cannot choose future date", "warning");
                return;
            }
            
        }
        $scope.InterestCount = "Y";
        var interestUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/bindCalculateInterestDetails?RATE=" + $scope.InterestCalculate.RATE + "&CUSTOMER_CODE=" + $scope.InterestCalculate.CUSTOMER_CODE + "&GROUP_CODES=" + groupAccCode+ "&UPTO_DATE=" + $scope.InterestCalculate.UPTO_DATE + "&COMPANY_CODE=" + $scope.InterestCalculate.COMPANY_CODE + "&BRANCH_CODE=" + $scope.InterestCalculate.BRANCH_CODE;

        var CustomerinterestDtlsUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/bindCustomerInterestDetails?RATE=" + $scope.InterestCalculate.RATE + "&CUSTOMER_CODE=" + $scope.InterestCalculate.CUSTOMER_CODE + "&UPTO_DATE=" + $scope.InterestCalculate.UPTO_DATE + "&COMPANY_CODE=" + $scope.InterestCalculate.COMPANY_CODE + "&BRANCH_CODE=" + $scope.InterestCalculate.BRANCH_CODE;
        //var interestUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/bindCalculateInterestDetails";

        //var InterestCalculationModel = {
        //    RATE: $scope.InterestCalculate.RATE,
        //    CUSTOMER_CODE: $scope.InterestCalculate.CUSTOMER_CODE,
        //    UPTO_DATE: $scope.InterestCalculate.UPTO_DATE
        //}
        //$scope.schemeGridOptions = {
        //    dataSource: {
        //        transport: {
        //            read: {
        //                type: "POST",
        //                url: interestUrl,
        //                contentType: "application/json; charset=utf-8",
        //                dataType: 'json',

        //            }
        //        },

        //        pageSize: 50,            
        //    },
        //    pageable: true,
        //    columnMenu: true,
        //    scrollable: {
        //        virtual: true
        //    },
        //    dataBound: function (e) {
        //    },
        //    columns: [
        //        {
        //            field: "CUSTOMER_CODE",
        //            title: "Customer Code",
        //            width: 150
        //        },

        //        {
        //            field: "CUSTOMER_EDESC",
        //            title: "Customer",
        //            width: 150
        //        },
        //        {
        //            field: "BALANCE",
        //            title: "Balance",
        //            width: 150
        //        },
        //        {
        //            field: "INTEREST",
        //            title: "Interest",
        //            width: 150
        //        }
        //    ]
        //};
        $scope.InterestGridOptions = {

            dataSource: {
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                type: "GET",
                transport: {
                    read: interestUrl,
                },
                pageSize: 50,
                //serverPaging: true,
                serverSorting: true
            },
            scrollable: true,
            height: 450,
            sortable: true,
            pageable: true,
            dataBound: function (e) {
                var griddata = $("#kGrid").data("kendoGrid");
                var dataItems = griddata.dataSource._pristineData;
                $('#header-chb').on("change", selectallRow);
                $(".row-checkbox").on("click", selectRow);

            },
            columns: [
                {
                    title: 'Select All',
                    headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='row-checkbox_" + dataItem.CUSTOMER_CODE + "' class='row-checkbox'><label class='k-checkbox-label' for='${dataItem.CUSTOMER_CODE}'></label>"
                    },
                    width: 50
                },
                {
                    field: "CUSTOMER_CODE",
                    title: "Customer Code",
                    width: 150
                },

                {
                    field: "CUSTOMER_EDESC",
                    title: "Customer",
                    width: 150
                },
                {
                    field: "CREDIT_DAYS",
                    title: "Credit Days",
                    width: 150
                },
                {
                    field: "BALANCE",
                    title: "Balance",
                    width: 150
                },
                {
                    field: "TOTL_INT_PARENT",
                    title: "Interest",
                    width: 150
                },
                //{
                //    template: '<a class="print glyphicon glyphicon-print" title="Click to Print" ng-dblclick="printdata()"  style="color:grey;"><span class="sr-only"></span> </a>',

                //    title: "Print",
                //    width: "40px"
                //}
                {
                    command: [
                        {
                            "name": "print",
                            "buttonType": "Image",
                            "text": " ",
                            "title": "Click to Print",
                            "imageClass": "fa-print",
                            "iconClass": "fa",
                            "click": function (e) {
                                debugger;
                                var companyCodeV = $("#companyCode").data('kendoComboBox').text();
                                var brnachCodeCodeV = $("#branchCode").data('kendoComboBox').text();
                                var applydate = $("#englishdatedocument6").val();
                                var interestrate = $("#rate").val();
                                var btn = $(e.currentTarget).closest("tr").find("a.k-icon.k-i-expand")[0];
                                $(btn).trigger('click');

                                var data = this.dataItem($(e.currentTarget).closest("tr"));
                                var detailGrid = $(e.currentTarget).closest('tr').next('tr').find('.orders').data("kendoGrid");
                                var items = detailGrid.dataSource.data();
                                data.DetailList = items;
                                e.preventDefault();
                                var template = kendo.template($("#InterestCalcPrintTemplate").html());
                                var result = template(data);

                                $("#SalesPrintFormBody").html(result);
                                $("#strcompanyCode").html(companyCodeV);
                                $("#strbranchCode").html(brnachCodeCodeV);
                               
                                $("#idapplydate").append("Apply Date: " + applydate);
                                $("#idinterestrate").append("Interest Rate: " + interestrate+"%");

                                $("#SalesPrintWindow").modal('show');
                            },
                            
                            visible: function (dataItem) {
                                return true;
                            //if (dataItem.APPROVED_FLAG == "A") return true;
                            //else return false;
                                  }
                        }
                    ]
                },
            ]


        };
        function selectallRow() {
            debugger;

            var checked = this.checked;

            /** Added this block of code **/
            //kendo.ui.progress($("#kimpactGrid"), true);
            //setTimeout(function () {
            //    kendo.ui.progress($("#kimpactGrid"), false);
            //}, 15);
            if (checked) {
                checkedItems = [];
                var grid = $("#kGrid").data("kendoGrid");
                var dataItem = datas = grid.dataSource._pristineData;
                for (var i = 0; i < dataItem.length; i++) {
                    debugger;
                    checkedItems.push({
                        "BALANCE": dataItem[i].BALANCE,
                        "CREDIT_DAYS": dataItem[i].CREDIT_DAYS,
                        "CUSTOMER_CODE": dataItem[i].CUSTOMER_CODE,
                        "CUSTOMER_EDESC": dataItem[i].CUSTOMER_EDESC,
                        "INTEREST": dataItem[i].INTEREST,
                        "UPTO_DATE": $scope.InterestCalculate.UPTO_DATE,
                        "RATE": $scope.InterestCalculate.RATE
                    });
                }
                $('.row-checkbox').attr('checked', true);
            }
            else {
                checkedItems = [];
                $('.row-checkbox').attr('checked', false);
            }
        }

        function selectRow() {
            debugger;
            var checked = this.checked;
            row = $(this).closest("tr"),
                grid = $("#kGrid").data("kendoGrid"),
                dataItem = grid.dataItem(row);
            if (checked) {
                checkedItems.push({
                    "BALANCE": dataItem.BALANCE,
                    "CREDIT_DAYS": dataItem.CREDIT_DAYS,
                    "CUSTOMER_CODE": dataItem.CUSTOMER_CODE,
                    "CUSTOMER_EDESC": dataItem.CUSTOMER_EDESC,
                    "INTEREST": dataItem.INTEREST,
                    "UPTO_DATE": $scope.InterestCalculate.UPTO_DATE,
                    "RATE": $scope.InterestCalculate.RATE
                });
            }
            else {
                for (var i = 0; i < checkedItems.length; i++) {
                    if (checkedItems[i].CUSTOMER_CODE == dataItem.CUSTOMER_CODE) {
                        checkedItems.splice(i, 1);
                    }
                }
                row.removeClass("k-state-selected");
            }
        }
        $scope.detailGridOptions = function (dataItem) {
            debugger;
            return {
                dataSource: {
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    type: "GET",
                    transport: {
                        read: "/api/SetupApi/bindCustomerInterestDetails?RATE=" + $scope.InterestCalculate.RATE + "&CUSTOMER_CODE=" + dataItem.CUSTOMER_CODE + "&UPTO_DATE=" + $scope.InterestCalculate.UPTO_DATE + "&COMPANY_CODE=" + $scope.InterestCalculate.COMPANY_CODE + "&BRANCH_CODE=" + $scope.InterestCalculate.BRANCH_CODE,
                    },
                    serverPaging: true,
                    serverSorting: true,
                    serverFiltering: true,
                    pageSize: 5,

                },
                scrollable: false,
                sortable: true,
                pageable: true,
                columns: [
                    { field: "VOUCHER_NO", title: "Voucher No", width: "70px" },
                    {
                        field: "VOUCHER_DATE", title: "Voucher Date", width: "70px",
                        template: "#=kendo.toString(kendo.parseDate(VOUCHER_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(VOUCHER_DATE),'dd MMM yyyy') #",
                    },
                    { field: "CREDIT_DAYS", title: "Credit Days", width: "40px" },
                    {
                        field: "DUE_DATE", title: "Due Date", width: "70px",
                        template: "#=kendo.toString(kendo.parseDate(DUE_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(DUE_DATE),'dd MMM yyyy') #",
                    },
                    { field: "DUE_DAYS", title: "Due Days", width: "40px" },
                    { field: "BALANCE", title: "Due Amount", width: "70px" },
                    { field: "INTEREST", title: "Interest", width: "70px" },
                   

                ]
            };
        };  
    }
    $scope.printdata = function () {
        debugger;
    }
    $scope.interestImpact = function () {
        debugger;
        if ($scope.InterestImpactCalc.FORM_CODE == "") {
            displayPopupNotification("Document is required", "error");
            return;
        }
        if ($scope.InterestImpactCalc.LEDGER_CODE == "") {
            displayPopupNotification("Ledger is required", "error");
            return;
        }
        if ($scope.InterestImpactCalc.ACCOUNT_CODE == "") {
            displayPopupNotification("Account is required", "error");
            return;
        }
        //if ($scope.InterestImpactCalc.VOUCHER_DATE == "") {
        //    displayPopupNotification("Voucher is required", "error");
        //    return;
        //}
        if ($scope.InterestImpactCalc.COMPANY_CODE == "" || $scope.InterestImpactCalc.COMPANY_CODE == "undefined" || $scope.InterestImpactCalc.COMPANY_CODE == undefined || $scope.InterestImpactCalc.COMPANY_CODE == null) {
            $scope.InterestImpactCalc.COMPANY_CODE = $scope.InterestCalculate.COMPANY_CODE;
            //displayPopupNotification("Company is required", "error");
            //return;
        }
        if ($scope.InterestImpactCalc.BRANCH_CODE == "" || $scope.InterestImpactCalc.BRANCH_CODE == "undefined" || $scope.InterestImpactCalc.BRANCH_CODE == undefined || $scope.InterestImpactCalc.BRANCH_CODE == null) {
            $scope.InterestImpactCalc.BRANCH_CODE = $scope.InterestCalculate.BRANCH_CODE;
            //displayPopupNotification("Branch is required", "error");
            //return;
        }
        datas = checkedItems;
        var model = {
            INTERESET_DATA: JSON.stringify(datas),
            INTEREST_PARAM_DATA: JSON.stringify($scope.InterestImpactCalc),
        }
        if (datas.length > 0) {

            var saveSchemeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/ImpactInterestCalculation";
            $http({
                method: 'POST',
                url: saveSchemeUrl,
                data: model
            }).then(function successCallback(response) {
                debugger;
                if (response.data.MESSAGE == "INSERTED") {

                    displayPopupNotification("Data succesfully saved ", "success");
                    $scope.InterestCount = "N";
                    $scope.InterestLogCount = "Y";
                    $scope.InterestPageStart = "N";

                    //setTimeout(function () {
                    //    location.reload();
                    //}, 1000);

                    //if ($("#kimpactGrid").data("kendoGrid") != null) {
                    //    $("#kimpactGrid").data("kendoGrid").dataSource.read();

                    //}

                }
                else if (response.data.MESSAGE == "CustomerExist") {
                    displayPopupNotification("Particular Customer is already in transaction", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // this callback will be called asynchronously
                // when the response is available
            }, function errorCallback(response) {
                displayPopupNotification("Something went wrong.Please try again later.", "error");
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
        }
        else {
            displayPopupNotification("Please Select data first.", "error");
        }
    }
    var interestlogUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/bindInterestCalcLogDetails";
    $scope.InterestLogGridOptions = {

        dataSource: {
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            type: "GET",
            transport: {
                read: interestlogUrl,
            },
            pageSize: 50,
            //serverPaging: true,
            serverSorting: true
        },
        scrollable: true,
        height: 450,
        sortable: true,
        pageable: true,
        dataBound: function (e) {
            //var griddata = $("#kGrid").data("kendoGrid");
            //var dataItems = griddata.dataSource._pristineData;
            //$('#header-chb').on("change", selectallRow);
            //$(".row-checkbox").on("click", selectRow);

        },
        columns: [
            //{
            //    title: 'Select All',
            //    headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
            //    template: function (dataItem) {
            //        return "<input type='checkbox' id='row-checkbox_" + dataItem.CUSTOMER_CODE + "' class='row-checkbox'><label class='k-checkbox-label' for='${dataItem.CUSTOMER_CODE}'></label>"
            //    },
            //    width: 50
            //},
            {
                hidden: true,
                field: "INTEREST_LOG_CODE",
                title: "Log Code",
                width: 150
            },
            {
                field: "VOUCHER_NO",
                title: "Voucher No",
                width: 150
            },
            {
                field: "VOUCHER_DATE",
                title: "Voucher Date",
                template: "#= kendo.toString(kendo.parseDate(VOUCHER_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
                width: 150
            },
            {
                field: "CUSTOMER_EDESC",
                title: "Customer",
                width: 150
            },
            {
                field: "INTEREST_AMOUNT",
                title: "Interest Amount",
                width: 150
            },
            {
                field: "GENERATED_DATE",
                title: "Generated Date",
                template: "#= kendo.toString(kendo.parseDate(GENERATED_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
                width: 150
            },
            {
                field: "CREATED_DATE",
                title: "Created Date",
                template: "#= kendo.toString(kendo.parseDate(CREATED_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
                width: 150
            }

        ]


    };
    $scope.reloadPage = function () {
        location.reload();
    };
    $scope.interestCalcCustomer = function (cCodes) {
        debugger;
        var interestCalccustomerUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetCustomersForInterestCalc?acccodes=" + cCodes;
        $scope.interestCalccustomerDataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: interestCalccustomerUrl,
                }
            }
        });
        $scope.interestCalccustomerOptions = {
            dataSource: $scope.interestCalccustomerDataSource,
            dataTextField: "CUSTOMER_EDESC",
            dataValueField: "CUSTOMER_CODE",
            filter: "contains",
            change: function (e) {
                debugger;

            },
            dataBound: function () {
            }
        };
    };
});
DTModule.service('InterestCalcService', function ($http) {
    debugger;
});