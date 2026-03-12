DTModule.controller('schemeSetupTreeCtrl', function ($scope, schemeSetupService, $rootScope, $http, $routeParams, $window, $filter) {
    $scope.saveAction = "Save";
    $scope.btnDelete = false;
    $scope.SchemeHeader = "Add Scheme";
    $scope.SchemeHeaderRun = "Generate";
    $scope.GENERATEDSTATUS = 'A';
    $scope.FROMDATE = '';
    $scope.TODATE = '';
    //$scope.Scheme = {
    //    SCHEME_CODE: "",
    //    SCHEME_EDESC: "",
    //    STATUS: "",
    //    SCHEME_TYPE: "",
    //    TYPE: "",
    //    FORM_CODE:"",
    //    CALCULATION_DAYS: "",
    //    ACCOUNT_CODE: "",
    //    CUSTOMER_CODE: "",
    //    PARTY_TYPE_CODE: "",
    //    ITEM_CODE: "",
    //    FOC_ITEM_CODE: "",
    //    GROUP_CODE: "",
    //    AREA_CODE: "",
    //    CHARGE_CODE: "",
    //    CHARGE_ACCOUNT_CODE: "",
    //    CHARGE_RATE: "",
    //    QUERY_STRING: "",
    //    FROM_QUANTITY: "",
    //    TO_QUANTITY: "",
    //    DISCOUNT_TYPE: "",
    //    DISCOUNT: "",
    //    EFFECTIVE_FROM: "",
    //    EFFECTIVE_TO: "",
    //};


    $scope.Scheme = {
        SCHEME_CODE: "",
        SCHEME_EDESC: "",
        SCHEME_TYPE: "",
        TYPE: "",
        STATUS: "M",
        CALCULATION_DAYS: "",
        FORM_CODE: "",
        ACCOUNT_CODE: "",
        CUSTOMER_CODE: "",
        ITEM_CODE: "",
        PARTY_TYPE_CODE: "",
        AREA_CODE: "",
        BRANCH_CODE: "",
        CHARGE_CODE: "",
        CHARGE_ACCOUNT_CODE: "",
        CHARGE_RATE: "",
        EFFECTIVE_FROM: "",
        EFFECTIVE_TO: "",
        QUERY_STRING: "",
        REMARKS: "",
    };
    $scope.Scheme.EFFECTIVE_FROM = $filter('date')(new Date(), 'dd-MMM-yyyy');
    $scope.Scheme.EFFECTIVE_TO = $filter('date')(new Date(), 'dd-MMM-yyyy');
    $scope.bulkCustomerModel = [];
    $scope.bulkItemModel = [];
    $scope.bulkPartyTypeModel = [];
    $scope.bulkDocumentTypeModel = [];
    $scope.bulkAreaTypeModel = [];
    $scope.bulkBranchTypeModel = [];
    var isUserAdminUrl = "/api/TemplateApi/IsLogedinUserAdmin";
    $http.get(isUserAdminUrl).then(function (response) {
      
        $scope.userRole = response.data;
    });
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
    var dealerUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetAllDealerCode";
    $scope.dealerDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: dealerUrl,
            }
        }
    });
    $scope.dealerOptions = {
        dataSource: $scope.dealerDataSource,
        dataTextField: "PARTY_TYPE_EDESC",
        dataValueField: "PARTY_TYPE_CODE",
        filter: "contains",
        change: function (e) {
            debugger;
            var currentItem = e.sender.dataItem(e.node);
            var datalength = e.sender._old.length;
            if (datalength > 0) {
                $('#customerMultiSelect').attr("disabled", true);
            }
            else {
                $('#customerMultiSelect').attr("disabled", false);
            }
        },
        dataBound: function () {
        }
    };
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
            var currentItem = e.sender.dataItem(e.node);
            
        },
        dataBound: function () {
        }
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
            //var currentItem = e.sender.dataItem(e.node);
            var datalength = e.sender._old.length;
            if (datalength > 0) {
                $('#dealer').attr("disabled", true);
            }
            else {
                $('#dealer').attr("disabled", false);
            }
        },
        dataBound: function () {
        }
    };
    var getCustomersByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetCustomers";
    $scope.treeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: getCustomersByUrl,
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

    $scope.getAllCustomer = function () {
        var allCust = schemeSetupService.getAllCustomer();
        allCust.then(function (custRes) {
            $scope.AllCustomer = custRes.data;
        });
    };

    $scope.getAllCustomer();

    $scope.getAllDealerType = function () {
        var allDealerType = schemeSetupService.getAllDealerType();
        allDealerType.then(function (dealerRes) {
            $scope.AllDealerType = dealerRes.data;
        });
    };

    $scope.getAllDealerType();


    $scope.customerControl = {

        smartButtonMaxItems: 1,
        enableSearch: true,
        smartButtonTextProvider(selectionArray) {
            if (selectionArray.length === 1) {
                return selectionArray[0].label;
            } else {
                return selectionArray.length + ' Selected';
            }
        }
    };
    $scope.multiSelectedCustomer = [];

    $scope.dealerControl = {

        smartButtonMaxItems: 1,
        //checkBoxes: true,
        enableSearch: true,
        smartButtonTextProvider(selectionArray) {
            if (selectionArray.length === 1) {
                return selectionArray[0].label;
            } else {
                return selectionArray.length + ' Selected';
            }
        }
    };
    $scope.multiSelectedDealer = [];



    var itemUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetAllItemForScheme";
    $scope.itemDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: itemUrl,
            }
        }
    });
    $scope.itemOptions = {
        dataSource: $scope.itemDataSource,
        dataTextField: "ITEM_EDESC",
        dataValueField: "ITEM_CODE",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {
        }
    };
    var chargeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetChargeCode";
    $scope.chargeDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: chargeUrl,
            }
        }
    });
    $scope.chargeOptions = {
        dataSource: $scope.chargeDataSource,
        dataTextField: "CHARGE_EDESC",
        dataValueField: "CHARGE_CODE",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {
        }
    };
    $scope.areaSetupDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllAreaSetupByFilter",
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
    //$scope.AreaSetupOption = {
    //    dataSource: $scope.areaSetupDataSource,
    //    template: '<span>{{dataItem.AREA_EDESC}}</span>  ' +
    //        '<span>{{dataItem.Type}}</span>',
    //    dataBound: function (e) {
    //        var issue;
    //        if (this.element[0].attributes['areasetup-index'] == undefined) {
    //            issue = $("#areasetup").data("kendoComboBox");
    //        }
    //        else {
    //            var index = this.element[0].attributes['areasetup-index'].value;
    //            var issueLength = ((parseInt(index) + 1) * 3) - 1;
    //            issue = $($(".areasetup")[issueLength]).data("kendoComboBox");

    //        }
    //        if (issue != undefined) {
    //            issue.setOptions({
    //                template: $.proxy(kendo.template("#= formatValue(AREA_EDESC, '', this.text()) #"), issue)
    //            });
    //        }
    //    }
    //};
    $scope.AreaSetupOption = {
        dataSource: $scope.areaSetupDataSource,
        dataTextField: "AREA_EDESC",
        dataValueField: "AREA_CODE",
        filter: "contains",
        change: function (e) {
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
    $scope.userControl = {
        smartButtonMaxItems: 3,
        enableSearch: true,
        smartButtonTextConverter: function (itemText, originalItem) {
            return itemText;
        }
    };
    function dateSet(date) {
        var month = date.getMonth() + 1;
        var day = date.getDate();
        date = date.getFullYear() + '/' +
            (('' + month).length < 2 ? '0' : '') + month + '/' +
            (('' + day).length < 2 ? '0' : '') + day;
        return date;
    }
    $scope.monthSelectorOptionscalcdays = {
        open: function () {

            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() + 100);
        },
        change: function () {
            debugger;
            //var date = new Date();
            //date.setDate(date.getDate() - $scope.formBackDays);
            //var minDate = dateSet(date);
            var maxDate = dateSet(new Date());
            var selecteddate = dateSet(this.value());
            if ((selecteddate < maxDate)) {
                alert("Selected date not available");
                $("#englishdatecalcdate").focus();
                var months = ["jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec"];
                var curDate = new Date();
                curDate = curDate.getDate() + "-" + months[curDate.getMonth()] + "-" + curDate.getFullYear();
                $("#englishdatecalcdate").val(curDate);
            }

            //var id = this.element.attr('id');
            //$scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'), id)
        },
        format: "dd-MMM-yyyy",

        // specifies that DateInput is used for masking the input element
        dateInput: true
    };
    $scope.monthSelectorOptions = {
        open: function () {

            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() + 100);
        },
        change: function () {

            var id = this.element.attr('id');
            //$scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'), id)
        },
        format: "dd-MMM-yyyy",

        // specifies that DateInput is used for masking the input element
        dateInput: true
    };
    $scope.grid = {
        change: SchemeChange,
        dataSource: {
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getSchemeCodeWithChild",
            },
            pageSize: 20
        },
        dataBound: function (e) {
           
        },
        
        groupable: false,
        sortable: false,
        height: 470,
        selectable: true,
        scrollable: {
            virtual: true
        },
        columns: [
            {
                field: "SCHEME_EDESC",
                title: "Scheme Name",
                width: 150
            }]

    }
    var a = 'A';
    var from = '';
    var to = '';
    $scope.implementgrid = {
        
        dataSource: {
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getManualScheme?status=" + a + "&from=" + from + "&to=" + to,
            },
            //pageSize: 20
        },
        dataBound: function (e) {

        },

        groupable: false,
        pageable: {
            alwaysVisible: false,
            pageSizes: [5, 10, 20]
        },
        sortable: false,
        height: 470,
        selectable: true,
        scrollable: {
            virtual: true
        },
        columns: [
            {
                field: "SCHEME_EDESC",
                title: "Scheme Name",
                width: 150
            },
            {
                field: "EFFECTIVE_FROM",
                title: "Effective From",
                width: 100,
                template: "#=kendo.toString(kendo.parseDate(EFFECTIVE_FROM),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(EFFECTIVE_FROM),'dd MMM yyyy') #",
            }
            ,
            {
            field: "EFFECTIVE_TO",
            title: "Effective To",
                width: 100,
                template: "#=kendo.toString(kendo.parseDate(EFFECTIVE_TO),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(EFFECTIVE_TO),'dd MMM yyyy') #",
            }
            ,
            {
                field: "CALCULATION_DAYS",
                title: "Calculation Days",
                width: 100,
                template: "#=kendo.toString(kendo.parseDate(CALCULATION_DAYS),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(CALCULATION_DAYS),'dd MMM yyyy') #",
            }
            ,
            {
                field: "SCHEME_TYPE",
                title: "Scheme Type",
                width: 120
            }
            ,
            {
                field: "TYPE",
                title: "Type",
                width: 50
            },

            {
                field: "IMPLEMENT_FLAG",
                title: "Implementation",
                width: 120
            }
            ,
            {
                //template: '#if (HasNotes) {# <a href="javascript:void(0)" onclick="openNotes(${CustomerKey}, ${ReservationKey})">View Notes</a> #} else {# N/A #}#'
                //command: [{
                //template: '<a class="edit glyphicon glyphicon-ok" title="Implement" ng-click="doSomething(dataItem.SCHEME_CODE)" style="color:grey;"><span class="sr-only"></span> </a>',
                template: '#if (IMPLEMENT_FLAG==="Yes") {# <a class="edit glyphicon glyphicon-ok" title="Yes" style="color:grey;"><span class="sr-only"></span> </a> #} else {#<a class="edit glyphicon glyphicon-arrow-right" title="No" ng-click="doSomething(dataItem.SCHEME_CODE)" style="color:grey;"><span class="sr-only"></span> </a> #}#',
                //template: '#= redirectEditOrder(ORDER_NO)#',
                //}],
                title: "Implement",
                width: "40px"
            }
        ]

    }

    $scope.bindSchemeimpelmentGrid = function () {
        debugger;
        var fromdate = $('#englishdatedocument5').val();
        var todate = $('#englishdatedocument6').val();
        $scope.implementgrid = {

            dataSource: {
                transport: {
                    read: window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getManualScheme?status=" + $scope.GENERATEDSTATUS + "&from=" + fromdate + "&to=" + todate,
                },
                //pageSize: 20
            },
            dataBound: function (e) {

            },

            groupable: false,
            pageable: {
                alwaysVisible: false,
                pageSizes: [5, 10, 20]
            },
            sortable: false,
            height: 470,
            selectable: true,
            scrollable: {
                virtual: true
            },
            columns: [
                {
                    field: "SCHEME_EDESC",
                    title: "Scheme Name",
                    width: 150
                },
                {
                    field: "EFFECTIVE_FROM",
                    title: "Effective From",
                    width: 100,
                    template: "#=kendo.toString(kendo.parseDate(EFFECTIVE_FROM),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(EFFECTIVE_FROM),'dd MMM yyyy') #",
                }
                ,
                {
                    field: "EFFECTIVE_TO",
                    title: "Effective To",
                    width: 100,
                    template: "#=kendo.toString(kendo.parseDate(EFFECTIVE_TO),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(EFFECTIVE_TO),'dd MMM yyyy') #",
                }
                ,
                {
                    field: "CALCULATION_DAYS",
                    title: "Calculation Days",
                    width: 100,
                    template: "#=kendo.toString(kendo.parseDate(CALCULATION_DAYS),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(CALCULATION_DAYS),'dd MMM yyyy') #",
                }
                ,
                {
                    field: "SCHEME_TYPE",
                    title: "Scheme Type",
                    width: 120
                }
                ,
                {
                    field: "TYPE",
                    title: "Type",
                    width: 50
                },
                {
                    field: "IMPLEMENT_FLAG",
                    title: "Implement",
                    width: 80
                }
                ,
                {
                    //template: '#if (HasNotes) {# <a href="javascript:void(0)" onclick="openNotes(${CustomerKey}, ${ReservationKey})">View Notes</a> #} else {# N/A #}#'
                    //command: [{
                    //template: '<a class="edit glyphicon glyphicon-ok" title="Implement" ng-click="doSomething(dataItem.SCHEME_CODE)" style="color:grey;"><span class="sr-only"></span> </a>',
                    template: '#if (IMPLEMENT_FLAG==="Yes") {# <a class="edit glyphicon glyphicon-ok" title="Implement" style="color:grey;"><span class="sr-only"></span> </a> #} else {#<a class="edit glyphicon glyphicon-remove" title="Implement" ng-click="doSomething(dataItem.SCHEME_CODE)" style="color:grey;"><span class="sr-only"></span> </a> #}#',
                    //template: '#= redirectEditOrder(ORDER_NO)#',
                    //}],
                    title: "Action",
                    width: "40px"
                }
            ]

        }

      

    }
    $scope.Statusclicked = function (e) {
        debugger;
        $scope.FROMDATE = '';
        $scope.TODATE = '';
        $('#englishdatedocument5').val('');
        $('#englishdatedocument6').val('');
    };
    $scope.doSomething = function (schemecode) {
               
            var model = {
                SCHEME_CODE: $scope.Scheme.SCHEME_CODE,
            }
        var implementSchemeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/implementschemeSetup?schemeCode=" + schemecode;
                $http({
                    method: 'POST',
                    url: implementSchemeUrl,
                    //data: model
                }).then(function successCallback(response) {

                    if (response.data.MESSAGE == "INSERTED") {
                        if ($("#kGrid").data("kendoGrid") != null) {
                            $("#kGrid").data("kendoGrid").dataSource.read();
                        }
                        displayPopupNotification("Imlemented Successfully", "Success");

                    }
                    else if (response.data.MESSAGE == "WRONGQUERYFAIL") {
                        displayPopupNotification("Wrong query in setup.Please check field in setup.", "error");
                    }
                    else if (response.data.MESSAGE == "EMPTYDATAFAIL")
                    {
                        displayPopupNotification("Query return empty data.Please review query first.", "error");
                    }
                    else {
                        displayPopupNotification("Something went wrong.Please check input fields in setup", "error");
                    }
                    // this callback will be called asynchronously
                    // when the response is available
                }, function errorCallback(response) {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                    // called asynchronously if an error occurs
                    // or server returns response with an error status.
                });
         
           


     

    }
    function SchemeChange(evt) {
        debugger;
        selectedRow = this.select();
        var item = this.dataItem(selectedRow);

        if (item) {

            $scope.saveAction = "Update";
            $scope.SchemeHeader = "Update Agent";

            var customerbycodeurl = "/api/TemplateApi/GetCustomerForSchemeByCode?code=" + item.CUSTOMER_CODE;
            $http.get(customerbycodeurl).then(function (response) {
                debugger;
                $scope.bulkCustomerModel = response.data;
                $scope.multiSelectedCustomer = response.data;
            });
            var documentbycodeurl = "/api/TemplateApi/GetDocumentCodeForSchemeByCode?code=" + item.FORM_CODE;
            $http.get(documentbycodeurl).then(function (response) {
                debugger;
                $scope.bulkDocumentTypeModel = response.data;
            });
            var areabycodeurl = "/api/TemplateApi/GetAreaCodeForSchemeByCode?code=" + item.AREA_CODE;
            $http.get(areabycodeurl).then(function (response) {

                $scope.bulkAreaTypeModel = response.data;
            });
            var branchbycodeurl = "/api/TemplateApi/GetBranchCodeForSchemeByCode?code=" + item.BRANCH_CODE;
            $http.get(branchbycodeurl).then(function (response) {

                $scope.bulkBranchTypeModel = response.data;
            });
            var itembycodeurl = "/api/TemplateApi/GetItemForSchemeByCode?code=" + item.ITEM_CODE;
            $http.get(itembycodeurl).then(function (response) {
             
                $scope.bulkItemModel = response.data;
            });
            var dealerbycodeurl = "/api/TemplateApi/GetDealerCodeForSchemeByCode?code=" + item.PARTY_TYPE_CODE;
            $http.get(dealerbycodeurl).then(function (response) {
                
                $scope.bulkPartyTypeModel = response.data;
                $scope.multiSelectedDealer = response.data;
            });

            $scope.Scheme.SCHEME_EDESC = item.SCHEME_EDESC;
            $scope.Scheme.SCHEME_CODE = item.SCHEME_CODE;
            $scope.Scheme.STATUS = item.STATUS;
            $scope.Scheme.SCHEME_TYPE = item.SCHEME_TYPE;
            $scope.Scheme.TYPE = item.TYPE;
            $scope.Scheme.CALCULATION_DAYS = item.CALCULATION_DAYS;
            $scope.Scheme.FORM_CODE = item.FORM_CODE;
            $scope.Scheme.ACCOUNT_CODE = item.ACCOUNT_CODE;
            $scope.Scheme.CUSTOMER_CODE = item.CUSTOMER_CODE
            $scope.Scheme.PARTY_TYPE_CODE = item.PARTY_TYPE_CODE;
            $scope.Scheme.ITEM_CODE = item.ITEM_CODE;
            $scope.Scheme.AREA_CODE = item.AREA_CODE;
            $scope.Scheme.CHARGE_CODE = item.CHARGE_CODE;
            $scope.Scheme.QUERY_STRING = item.QUERY_STRING;
            $scope.Scheme.CHARGE_ACCOUNT_CODE = item.CHARGE_ACCOUNT_CODE;
            $scope.Scheme.CHARGE_RATE = item.CHARGE_RATE;
            $scope.Scheme.EFFECTIVE_FROM = item.EFFECTIVE_FROM;
            $scope.Scheme.EFFECTIVE_TO = item.EFFECTIVE_TO;
            $scope.Scheme.BRANCH_CODE = item.BRANCH_CODE;
            $scope.Scheme.REMARKS = item.REMARKS;           
            //$scope.Scheme.FROM_QUANTITY = item.FROM_QUANTITY;
            //$scope.Scheme.TO_QUANTITY = item.TO_QUANTITY;
            //$scope.Scheme.DISCOUNT_TYPE = item.DISCOUNT_TYPE;
            //$scope.Scheme.DISCOUNT = item.DISCOUNT;
            

            //var requiredCustomer = $("#customer").kendoMultiSelect().data("kendoMultiSelect");
            //var cstring = item.CUSTOMER_CODE,
                //strx = cstring.split(',');
            //$scope.bulkCustomerModel = strx;

            //sarray = [];

            //sarray = sarray.concat(strx);
            //equiredCustomer.value(sarray);

     

        
            $scope.btnDelete = true;
            $scope.$apply();
        }
    }
    $scope.getMaxSchemeCode = function () {

        var getMaxCodeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getMaxSchemeCode";
        $http({
            method: 'Get',
            url: getMaxCodeUrl,
        }).then(function successCallback(response) {
            $scope.Scheme.SCHEME_CODE = response.data.DATA;
        }, function errorCallback(response) {
            displayPopupNotification(response.data.STATUS_CODE, "error");
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
    }

    $scope.getMaxSchemeCode();
    $scope.SaveScheme = function (isValid) {
        debugger;
      
        var multipledocumenttext = "";
        var multipledocumentarray = [];
        var multipledocument = _.pluck($scope.bulkDocumentTypeModel, 'FORM_CODE').join(','); 
         multipledocumentarray = $('#document option:selected').toArray().map(item => item.text);
        if (multipledocumentarray.length > 1) {
            multipledocumenttext = multipledocumentarray.join(",");
        }
        else {
            multipledocumenttext = multipledocumentarray;
        }

        var multipleareatext = "";
        var multipleareaarray = [];
        var multiplearea = _.pluck($scope.bulkAreaTypeModel, 'AREA_CODE').join(',');   
        var multipleareaarray = $('#areasetup option:selected').toArray().map(item => item.text);
        if (multipleareaarray.length > 1) {
            multipleareatext = multipleareaarray.join(",");
        }
        else {
            multipleareatext = multipleareaarray;
        }

        var multiplebranchtext = "";
        var multiplebrancharray = [];
        var multiplebranch = _.pluck($scope.bulkBranchTypeModel, 'BRANCH_CODE').join(',');  
        var multiplebrancharray = $('#branch option:selected').toArray().map(item => item.text);
        if (multiplebrancharray.length > 1) {
            multiplebranchtext = multiplebrancharray.join(",");
        }
        else {
            multiplebranchtext = multiplebrancharray;
        }

        //var multipleCustomertext = "";
        //var multipleCustomerarray = [];
        //var multipleCustomer = _.pluck($scope.bulkCustomerModel, 'CUSTOMER_CODE').join(','); 
        //var multipleCustomerarray = $('#customerMultiSelect option:selected').toArray().map(item => item.text);
        //if (multipleCustomerarray.length > 1) {
        //    multipleCustomertext = multipleCustomerarray.join(",");
        //}
        //else {
        //    multipleCustomertext = multipleCustomerarray;
        //}
        var multipleCustomertext = "";
        var multipleCustomerarray = [];
        var multipleCustomer = _.pluck($scope.multiSelectedCustomer, 'TYPE_CODE').join(',');
        var multipleCustomerarray = $('#customerMultiSelect option:selected').toArray().map(item => item.text);
        if (multipleCustomerarray.length > 1) {
            multipleCustomertext = multipleCustomerarray.join(",");
        }
        else {
            multipleCustomertext = multipleCustomerarray;
        }

        var multipleItemtext = "";
        var multipleItemarray = [];
        var multipleItem = _.pluck($scope.bulkItemModel, 'ITEM_CODE').join(',');     
        var multipleItemarray = $('#item option:selected').toArray().map(item => item.text);
        if (multipleItemarray.length > 1) {
            multipleItemtext = multipleItemarray.join(",");
        }
        else {
            multipleItemtext = multipleItemarray;
        }



        //var multipleDealertext = "";
        //var multipleDealerarray = [];
        //var multipleDealer = _.pluck($scope.bulkPartyTypeModel, 'PARTY_TYPE_CODE').join(',');  
        //var multipleDealerarray = $('#dealer option:selected').toArray().map(item => item.text);
        //if (multipleDealerarray.length > 1) {
        //    multipleDealertext = multipleDealerarray.join(",");
        //}
        //else {
        //    multipleDealertext = multipleDealerarray;
        //}

        var multipleDealertext = "";
        var multipleDealerarray = [];
        var multipleDealer = _.pluck($scope.multiSelectedDealer, 'PARTY_TYPE_CODE').join(',');
        var multipleDealerarray = $('#dealer option:selected').toArray().map(item => item.text);
        if (multipleDealerarray.length > 1) {
            multipleDealertext = multipleDealerarray.join(",");
        }
        else {
            multipleDealertext = multipleDealerarray;
        }


        if ($scope.Scheme.SCHEME_EDESC == "") {
            displayPopupNotification("Scheme Name should not be empty.", "error");
            return; 
        }
        if (multipleItem == undefined || multipleItem == "undefined" || multipleItem == null || multipleItem == "") {
            displayPopupNotification("Item should not be empty.", "error");
            return; 
        }
        if (multipledocument == undefined || multipledocument == "undefined" || multipledocument == null || multipledocument == "") {
            displayPopupNotification("Document should not be empty.", "error");
            return;
        }
        if ($scope.Scheme.ACCOUNT_CODE == undefined || $scope.Scheme.ACCOUNT_CODE == "undefined" || $scope.Scheme.ACCOUNT_CODE == null || $scope.Scheme.ACCOUNT_CODE == "") {
            displayPopupNotification("Account should not be empty.", "error");
            return;
        }
        if($scope.Scheme.QUERY_STRING == undefined || $scope.Scheme.QUERY_STRING == "undefined" || $scope.Scheme.QUERY_STRING == null || $scope.Scheme.QUERY_STRING == "") {
            displayPopupNotification("Query should not be empty.", "error");
            return;
        }
        if (multipleCustomer == undefined || multipleCustomer == "undefined" || multipleCustomer == null || multipleCustomer == "") {
            if (multipleDealer == undefined || multipleDealer == "undefined" || multipleDealer == null || multipleDealer == "") {
                displayPopupNotification("Customer or Dealer should not be empty.", "error");
                return;
            }
        }
        if (multipleDealer == undefined || multipleDealer == "undefined" || multipleDealer == null || multipleDealer == "") {
            if (multipleCustomer == undefined || multipleCustomer == "undefined" || multipleCustomer == null || multipleCustomer == "") {
                displayPopupNotification("Customer or Dealer should not be empty.", "error");
                return;
            }
        }
        if ($scope.Scheme.SCHEME_TYPE == undefined || $scope.Scheme.SCHEME_TYPE == "$scope.Scheme.SCHEME_TYPE" || $scope.Scheme.SCHEME_TYPE == null || $scope.Scheme.SCHEME_TYPE == "") {
            displayPopupNotification("Scheme type should not be empty.", "error");
            return;
        }
        if ($scope.Scheme.TYPE == undefined || $scope.Scheme.TYPE == "$scope.Scheme.SCHEME_TYPE" || $scope.Scheme.TYPE == null || $scope.Scheme.TYPE == "") {
            displayPopupNotification("Type should not be empty.", "error");
            return;
        }  
       
        var querystring = $scope.Scheme.QUERY_STRING;
        if (querystring !== undefined && querystring !== "undefined" && querystring !== null && querystring !== "" ){
            if (querystring.toLowerCase().indexOf("insert") >= 0) {
                return displayPopupNotification("Rule Query should not contain insert.", "warning");
            }
            if (querystring.toLowerCase().indexOf("update") >= 0) {
                return displayPopupNotification("Rule Query should not contain update.", "warning");
            }
            if (querystring.toLowerCase().indexOf("delete") >= 0) {
                return displayPopupNotification("Rule Query should not contain delete.", "warning");
            }
            if (querystring.toLowerCase().indexOf("truncate") >= 0) {
                return displayPopupNotification("Rule Query should not contain truncate.", "warning");
            }
            if (querystring.toLowerCase().indexOf("drop") >= 0) {
                return displayPopupNotification("Rule Query should not contain drop.", "warning");
            }
        }
       
        var model = {
            SCHEME_CODE: $scope.Scheme.SCHEME_CODE,
            SCHEME_EDESC: $scope.Scheme.SCHEME_EDESC,
            SCHEME_TYPE: $scope.Scheme.SCHEME_TYPE,
            TYPE: $scope.Scheme.TYPE,
            STATUS: $scope.Scheme.STATUS,
            CALCULATION_DAYS: $scope.Scheme.CALCULATION_DAYS,
            FORM_CODE: multipledocument,
            ACCOUNT_CODE: $scope.Scheme.ACCOUNT_CODE,
            CUSTOMER_CODE: multipleCustomer,
            ITEM_CODE: multipleItem,
            PARTY_TYPE_CODE: multipleDealer,
            AREA_CODE: multiplearea,
            BRANCH_CODE: multiplebranch,
            CHARGE_CODE: $scope.Scheme.CHARGE_CODE,
            CHARGE_ACCOUNT_CODE: $scope.Scheme.CHARGE_ACCOUNT_CODE,
            CHARGE_RATE: $scope.Scheme.CHARGE_RATE,
            EFFECTIVE_FROM: $scope.Scheme.EFFECTIVE_FROM,
            EFFECTIVE_TO: $scope.Scheme.EFFECTIVE_TO,
            QUERY_STRING: $scope.Scheme.QUERY_STRING,
            REMARKS: $scope.Scheme.REMARKS,
            FORM_EDESC :multipledocumenttext,
            AREA_EDESC :multipleareatext,
            BRANCH_EDESC :multiplebranchtext,
            CUSTOMER_EDESC :multipleCustomertext,
            ITEM_EDESC :multipleItemtext,
            PARTY_TYPE_EDESC :multipleDealertext,
           
        }
        
        if (!isValid) {
            displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
            return;
        }
       
        if ($scope.saveAction == "Save") {
           
            var saveSchemeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewSchemeSetup";
            $http({
                method: 'POST',
                url: saveSchemeUrl,
                data: model
            }).then(function successCallback(response) {
            
                if (response.data.MESSAGE == "INSERTED") {
                    debugger;
                    $("#grid").data("kendoGrid").dataSource.read();
                    if ($("#kGrid").data("kendoGrid")!=null) {
                        $("#kGrid").data("kendoGrid").dataSource.read();
                    }
                 
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully saved ", "success");
                    $scope.bulkCustomerModel = [];
                    $scope.bulkDocumentTypeModel = [];
                    $scope.bulkBranchTypeModel = [];
                    $scope.bulkAreaTypeModel = [];
                    $scope.bulkItemModel = [];
                    $scope.bulkPartyTypeModel = [];
                    $scope.Scheme.EFFECTIVE_FROM = $filter('date')(new Date(), 'dd-MMM-yyyy');
                    $scope.Scheme.EFFECTIVE_TO = $filter('date')(new Date(), 'dd-MMM-yyyy');
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
           
            var udpateSchemeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateSchemeSetup";
            $http({
                method: 'POST',
                url: udpateSchemeUrl,
                data: model
            }).then(function successCallback(response) {
               
                if (response.data.MESSAGE == "UPDATED") {

                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                    displayPopupNotification("Data succesfully updated ", "success");
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

        $("#grid").data("kendoGrid").dataSource.read();


    }

    $scope.deleteScheme = function (isValid) {
       
        if (!isValid) {
            displayPopupNotification("There is no any scheme selected.", "warning");
            return;
        }
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
                    var SCHEME_EDESC = $("#SchemeName").val();
                    var SCHEME_CODE = $("#SchemeCode").val();
                    var SCHEMEID;
                    if (SCHEMEID == undefined) {
                        SCHEMEID = $scope.Scheme.SCHEME_CODE;
                    }
                    var deleteSchemetUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/deleteschemeSetup?schemeCode=" + SCHEMEID;
                    $http({
                        method: 'POST',
                        url: deleteSchemetUrl,
                    }).then(function successCallback(response) {
                       
                        if (response.data.MESSAGE == "DELETED") {
                            $("#grid").data("kendoGrid").dataSource.read();
                            $scope.Cancel();
                            displayPopupNotification("Data succesfully deleted ", "success");
                        }
                        else {
                            displayPopupNotification(response.data.STATUS_CODE, "error");
                        }
                        // this callback will be called asynchronously
                        // when the response is available
                    }, function errorCallback(response) {
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                        // called asynchronously if an error occurs
                        // or server returns response with an error status.
                    });
                }

                $("#grid").data("kendoGrid").dataSource.read();
                $scope.Cancel();
            }

        });


    }
    $scope.Cancel = function () {
      
        $scope.saveAction = "Save";
        $scope.SchemeHeader = "Add Scheme";
        $scope.Scheme = {
            SCHEME_CODE: "",
            SCHEME_EDESC: "",
            SCHEME_TYPE: "",
            TYPE: "",
            STATUS: "M",
            CALCULATION_DAYS: "",
            FORM_CODE: "",
            ACCOUNT_CODE: "",
            CUSTOMER_CODE: "",
            ITEM_CODE: "",
            PARTY_TYPE_CODE: "",
            AREA_CODE: "",
            BRANCH_CODE: "",
            CHARGE_CODE: "",
            CHARGE_ACCOUNT_CODE: "",
            CHARGE_RATE: "",
            EFFECTIVE_FROM: "",
            EFFECTIVE_TO: "",
            QUERY_STRING: "",
            REMARKS:"",
        };
        $scope.bulkCustomerModel = [];
        $scope.bulkItemModel = [];
        $scope.bulkPartyTypeModel = [];
        $scope.bulkDocumentTypeModel = [];
  
        $scope.bulkBranchTypeModel = [];
        $scope.bulkAreaTypeModel = [];
        $scope.Scheme.EFFECTIVE_FROM = $filter('date')(new Date(), 'dd-MMM-yyyy');
        $scope.Scheme.EFFECTIVE_TO = $filter('date')(new Date(), 'dd-MMM-yyyy');
   
        $("#grid").data("kendoGrid").clearSelection();
        $scope.getMaxSchemeCode();
        $scope.btnDelete = false;

        $('#dealer').attr("disabled", false);
        $('#customer').attr("disabled", false);
    }

    //var getCustomerForSchemeUrl = window.location.protocol + "//" + window.location.host + "/api/SubLedgerMgmtApi/GetSchemeCustomer";
    //var customerLedgerDataSource = new kendo.data.HierarchicalDataSource({
    //    transport: {
    //        read: {
    //            url: getCustomerForSchemeUrl,
    //            dataType: "json"
    //        }
    //    },
    //    schema: {
    //        parse: function (data) {
    //            return data;
    //        },
    //        model: {
    //            id: "MASTER_CUSTOMER_CODE",
    //            parentId: "PRE_CUSTOMER_CODE",
    //            children: "ITEMS",
    //            items: "ITEMS",
    //            fields: {
    //                TYPE_CODE: { field: "CUSTOMER_CODE", type: "string" },
    //                TYPE_EDESC: { field: "CUSTOMER_EDESC", type: "string" },
    //                parentId: { field: "PRE_CUSTOMER_CODE", type: "string", defaultValue: "00" }
    //            }
    //        }
    //    }
    //});

    //$("#customerTreeView").kendoTreeView({
    //    checkboxes: {
    //        checkChildren: true
    //    },
    //    check: checkedNodeIds,
    //    template: kendo.template($("#treeview-template").html()),
    //    dataTextField: "CUSTOMER_EDESC",
    //    dataSource: getCustomerForSchemeUrl,
    //    dataBound: function (o) {
    //        var grid = o.sender;
    //        if (grid.dataSource.total() === 0) {
    //            $(o.sender.wrapper)
    //                .find('ul')
    //                .append('<li class="kendo-data-row" style="font-size:12px;">Sorry, no data </li>');
    //            displayPopupNotification("No Data Found.", "info");
    //        }
    //    }
    //});

    //function checkedNodeIds(nodes, checkedNodes) {
    //    console.log("nodes========iiiiii===========>>>" + JSON.stringify(nodes));
    //    if (nodes.checked) {

    //        $scope.checkedLedgerObj = {
    //            "CUSTOMER_CODE": nodes.CUSTOMER_CODE,
    //            "CUSTOMER_EDESC": nodes.CUSTOMER_EDESC,
    //            "LINK_SUB_CODE": nodes.LINK_SUB_CODE,
    //            "GROUP_SKU_FLAG": nodes.GROUP_SKU_FLAG,
    //            "ITEMS": []
    //        };
    //        if (nodes.HAS_BRANCH) {

    //            $scope.checkedLedgerObj.ITEMS = nodes.children.options.data.ITEMS;
    //        }

    //        $scope.checkedLedgerList.push($scope.checkedLedgerObj);

    //    } else {
    //        $scope.checkedLedgerList.pop();
    //    }
    //}

    //$scope.checkedLedgerList = [];
    //$scope.checkedLedgerObj = {
    //    "CUSTOMER_CODE": "",
    //    "CUSTOMER_EDESC": "",
    //    "LINK_SUB_CODE": "",
    //    "GROUP_SKU_FLAG": "",
    //    "ITEMS": []
    //};

});

DTModule.service('schemeSetupService', function ($http) {
    this.getAllCustomer = function () {
        var allCust = $http({
            method: "GET",
            url: "/api/CustomFormApi/GetCustomerForPostedDate",
            dataType: "json"
        });
        return allCust;
    };
    this.getAllDealerType = function () {
        var allDlr = $http({
            method: "GET",
            url: "/api/CustomFormApi/GetAllDealerType",
            dataType: "JSON"
        });
        return allDlr;
    };
});