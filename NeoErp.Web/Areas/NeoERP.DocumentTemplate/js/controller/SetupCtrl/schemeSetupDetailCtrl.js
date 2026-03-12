DTModule.controller('schemeSetupDetailCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    debugger;
    $scope.SchemeDetailHeader = "Reports And Actions";
    $scope.SchemeImpactHeader = "Impact";
    $scope.SchemeDetailCount = "N";
    $scope.SchemeDetail = {
        SCHEME_CODE: "",
        SCHEME_EDESC: "",
        SCHEME_TYPE: "",
        TYPE: "",
        STATUS: "",
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
    };
    window.globalIndex = 0;
    //$scope.dynamicSubLedgerModalData = [{
    //    ACC_CODE: "",
    //    TRANSACTION_TYPE: "",
    //    VOUCHER_AMOUNT: "",
    //    SUBLEDGER_AMOUNT: "",
    //    REMAINING_AMOUNT: "",
    //    SUBLEDGER: [{
    //        SERIAL_NO: 1,
    //        SUB_CODE: "",
    //        SUB_EDESC: "",
    //        PARTY_TYPE_CODE: "",
    //        AMOUNT: "",
    //        PARTICULARS: "",
    //        REFRENCE: "",
    //        Descriptiontest: "",
    //    }]
    //}];
    //$scope.subledgercount = '';
    //$scope.subLedgerDataSource = {
    //    type: "json",
    //    serverFiltering: true,
    //    autoBind: true,

    //    transport: {
    //        read: {
              
    //            url: window.location.protocol + "//" + window.location.host +"/api/TemplateApi/GetAllSubLedgerByFilterPartyType"
    //        },
    //        parameterMap: function (data, action) {
    //            debugger;
    //            var newParams;
    //            if (data.filter != undefined) {
    //                if (data.filter.filters[0] != undefined) {
    //                    newParams = {
    //                        filter: data.filter.filters[0].value,
    //                        partyTypeCode:""
    //                        //accCode: window.accCode
    //                    };
    //                    return newParams;

    //                }
    //                else {
    //                    newParams = {
    //                        filter: "",
    //                        partyTypeCode: ""
    //                    };
    //                    return newParams;
    //                }
    //            }
    //            else {
    //                newParams = {
    //                    filter: "",
    //                    partyTypeCode: ""
    //                };
    //                return newParams;
    //            }
    //        }
    //    },
    //}
    //$scope.subledgerCodeAutocomplete =
    //{
      
    //    dataSource: $scope.subLedgerDataSource,
    //    suggest: false,
    //    highlightFirst: true,
    //    select: function (e) {
    //        debugger;
    //        var description = e.dataItem.SUB_EDESC;
    //        var Code = e.dataItem.SUB_CODE;
    //        var key = this.element[0].attributes['subledger-key'].value;
    //        var index = this.element[0].attributes['subledger-index'].value;
    //        console.log("description", description);

    //        $scope.dynamicSubLedgerModalData[key].SUBLEDGER[index].SUB_EDESC = description;


    //        var sublen = $scope.dynamicSubLedgerModalData[key].SUBLEDGER.length;
    //        //check valildation after select new code  start
    //        for (var j = 0; j < sublen; j++) {
    //            var subcode = $scope.dynamicSubLedgerModalData[key].SUBLEDGER[j].SUB_CODE;
    //            if (index != j) {
    //                if (subcode === Code) {
    //                    $scope.CodeValidation = "You cannot select same Code and Subledger";
    //                    $($(".SUBLEDGERCODE_" + index)[1]).addClass("borderRed");
    //                    $($(".SUBLEDGERDESCRIPTION_" + index)[1]).addClass("borderRed");
    //                    $scope.subledgercount = true;
    //                    return;

    //                }
    //                else {
    //                    $($(".SUBLEDGERCODE_" + index)[1]).removeClass("borderRed");
    //                    $($(".SUBLEDGERDESCRIPTION_" + index)[1]).removeClass("borderRed");
    //                    if (!$(".subledgerfirst").hasClass("borderRed")) {
    //                        $scope.CodeValidation = '';
    //                    }

    //                    $scope.subledgercount = false;
    //                };

    //            }
    //        }
    //        //end

    //    },
    //    dataBound: function (e) {

    //        var index = this.element[0].attributes['subledger-index'].value;
    //        var subledgerLength = ((parseInt(index) + 1) * 3) - 1;
    //        var subledger = $($(".subledgersecond")[subledgerLength]).data("kendoComboBox");
    //        if (subledger != undefined) {
    //            subledger.setOptions({
    //                template: $.proxy(kendo.template("#= formatValue(SUB_EDESC,Type,this.text()) #"), subledger)
    //            });
    //        }
    //    },
    //}


    //$scope.subledgerDescAutocomplete =
    //{
    //    dataSource: $scope.subLedgerDataSource,
    //    suggest: false,
    //    highlightFirst: true,
    //    select: function (e) {
    //        debugger;
    //        var Code = e.dataItem.SUB_CODE;
    //        var key = this.element[0].attributes['subledger-key'].value;
    //        var index = this.element[0].attributes['subledger-index'].value;

    //        $scope.dynamicSubLedgerModalData[key].SUBLEDGER[index].SUB_CODE = Code;


    //        var sublen = $scope.dynamicSubLedgerModalData[key].SUBLEDGER.length;
    //        //check valildation after select new code  start
    //        for (var j = 0; j < sublen; j++) {
    //            var subcode = $scope.dynamicSubLedgerModalData[key].SUBLEDGER[j].SUB_CODE;
    //            if (index != j) {
    //                if (subcode === Code) {
    //                    $scope.CodeValidation = "You cannot select same Code and Subledger";
    //                    $($(".SUBLEDGERCODE_" + index)[1]).addClass("borderRed");
    //                    $($(".SUBLEDGERDESCRIPTION_" + index)[1]).addClass("borderRed");
    //                    $scope.subledgercount = true;
    //                    return;

    //                }
    //                else {
    //                    $($(".SUBLEDGERCODE_" + index)[1]).removeClass("borderRed");
    //                    $($(".SUBLEDGERDESCRIPTION_" + index)[1]).removeClass("borderRed");
    //                    if (!$(".subledgerfirst").hasClass("borderRed")) {
    //                        $scope.CodeValidation = '';
    //                    }
    //                    $scope.subledgercount = false;
    //                };

    //            }
    //        }
    //        end
    //    },
    //    dataBound: function (e) {

    //        var index = this.element[0].attributes['subledger-index'].value;
    //        var subledgerLength = ((parseInt(index) + 1) * 3) - 1;
    //        var subledger = $($(".subledgersecond")[subledgerLength]).data("kendoComboBox");
    //        if (subledger != undefined) {
    //            subledger.setOptions({
    //                template: $.proxy(kendo.template("#= formatValue(SUB_EDESC,Type,this.text()) #"), subledger)
    //            });
    //        }
    //    },
    //};
    //$scope.Changesubledgercode = function (key, index, e) {
    //    //debugger;

    //    $scope.isPresent = false;
    //    var data = $($(".subledgerfirst")[2]).data("kendoComboBox").dataSource.data();
    //    if ($scope.dynamicSubLedgerModalData[key].SUBLEDGER[index].SUB_CODE != "") {                   
    //    }
    //}
    //$scope.popUp = function ($index) { 
    //    debugger;
    //    var ind = $index - 1;
    //    window.globalIndex = ind;
    //   $scope.dynamicSubLedgerModalData[ind].SUBLEDGER_AMOUNT = $("#kimpactGrid").data("kendoGrid")._data[ind].SALES_SCHEME_VALUE;

    //    //$(".dynamicSubLedgerModal_" + $index).modal('show');
    //    //$("#myModal").modal('show');

    //    $(".schemeSubledgerModal_"+ind).toggle();
    //        //$(".dynamicSubLedgerModal_" + $index).modal({
    //        //    show: 'true'
    //        //});

    //        //$(".dynamicSubLedgerModal_" + $index).on('shown.bs.modal', function () {
    //        //    var $this = this;
    //        //    setTimeout(function () {

    //        //        $($($($(".subledger-table")[$index]).find('tr')[1]).find('td :input')[0]).focus();
    //        //    }, 500);
    //        //})
        

    //}
    //$scope.add_childSubledger_element = function (e) {

    //    if ($scope.subledgercount === true) {
    //        displayPopupNotification("Same Code Or Subledger cannot be selected", "warning");
    //        e.preventDefault();
    //        e.stopPropagation();
    //        return false;
    //    }
    //    var subledgerindex = window.globalIndex;
    //    $scope.dynamicSubLedgerModalData[subledgerindex].SUBLEDGER.push({
    //        SERIAL_NO: subledgerindex + 1,
    //        SUB_CODE: "",
    //        SUB_EDESC: "",
    //        PARTY_TYPE_CODE: "",
    //        AMOUNT: "",
    //        PARTICULARS: "",
    //        REFRENCE: ""
    //    });

    //};
    //$scope.remove_childSubledger_element = function (key, index, VOUCHER_AMOUNT) {
    //    if ($scope.dynamicSubLedgerModalData[key].SUBLEDGER.length > 1) {
    //        $scope.dynamicSubLedgerModalData[key].SUBLEDGER.splice(index, 1);
    //        //$scope.Change(key, VOUCHER_AMOUNT);
    //        $scope.CodeValidation = '';
    //        $scope.subledgercount = false;
    //    }
    //};
    checkedItems = [];
    var req = "/api/TemplateApi/GetCompanyInfo";
    $http.get(req).then(function (results) {
        debugger;
        $scope.compname = results.data.COMPANY_EDESC;

    });
    var reqq = "/api/TemplateApi/GetLogedinUser";
    $http.get(reqq).then(function (resultss) {
        debugger;
        $scope.LoginUser = resultss.data;

    });
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
            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {
        }
    };
    var shemeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getSchemeCodeWithChild";
    $scope.schemeDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: shemeUrl,
            }
        }
    });
    $scope.shemeOptions = {
        dataSource: $scope.schemeDataSource,
        dataTextField: "SCHEME_EDESC",
        dataValueField: "SCHEME_CODE",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {
        }
    };
    var shemeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getSchemeCodenotimplemented";
    $scope.schemeDataSourcenoti = new kendo.data.DataSource({
        transport: {
            read: {
                url: shemeUrl,
            }
        }
    });
    $scope.shemeOptionsnoti = {
        dataSource: $scope.schemeDataSourcenoti,
        dataTextField: "SCHEME_EDESC",
        dataValueField: "SCHEME_CODE",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {
        }
    };
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
    $scope.AreaSetupOption = {
        dataSource: $scope.areaSetupDataSource,
        template: '<span>{{dataItem.AREA_EDESC}}</span>  ' +
            '<span>{{dataItem.Type}}</span>',
        dataBound: function (e) {
            var issue;
            if (this.element[0].attributes['areasetup-index'] == undefined) {
                issue = $("#areasetup").data("kendoComboBox");
            }
            else {
                var index = this.element[0].attributes['areasetup-index'].value;
                var issueLength = ((parseInt(index) + 1) * 3) - 1;
                issue = $($(".areasetup")[issueLength]).data("kendoComboBox");

            }
            if (issue != undefined) {
                issue.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(AREA_EDESC, '', this.text()) #"), issue)
                });
            }
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
    $scope.bindSchemeDetailGrid = function () {
        $scope.SchemeDetailCount = "Y";
        var DOCUMENT_SCHEME = {
            SCHEME_CODE: $scope.SchemeDetail.SCHEME_CODE,
            SCHEME_EDESC: $scope.SchemeDetail.SCHEME_EDESC,
            CUSTOMER_CODE: $scope.SchemeDetail.CUSTOMER_CODE,
            SCHEME_TYPE: $scope.SchemeDetail.SCHEME_TYPE,
            TYPE: $scope.SchemeDetail.TYPE,
            ITEM_CODE: $scope.SchemeDetail.ITEM_CODE,
            PARTY_TYPE_CODE: $scope.SchemeDetail.PARTY_TYPE_CODE,
            BRANCH_CODE: $scope.SchemeDetail.BRANCH_CODE,
            AREA_CODE: $scope.SchemeDetail.AREA_CODE,
            EFFECTIVE_FROM: $scope.SchemeDetail.EFFECTIVE_FROM,
            EFFECTIVE_TO: $scope.SchemeDetail.EFFECTIVE_TO,
        }
        $scope.schemeGridOptions = {
            toolbar: ["excel"],
            excel: {
                allPages: true
            },
            excelExport: function (e) {

                ExportToExcel(e);
                e.preventDefault();
            },
            dataSource: {
                transport: {
                    read: {
                        type: "POST",
                        url: "/api/TemplateApi/bindSchemeDetailGrid",
                        contentType: "application/json; charset=utf-8",
                        dataType: 'json',
                        
                    },
                    parameterMap: function (options, type) {
                        debugger;
                        var paramMap = JSON.stringify($.extend(options, { SchemeModel: DOCUMENT_SCHEME }));
                        delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                        delete paramMap.$format; // <-- remove format parameter.
                        return paramMap;
                    },
                },
               
                pageSize: 50,
                //serverPaging: false,
                //serverSorting: false,
               
            },
            
            //scrollable: true,
            //height: 350,
            //sortable: true,
            //resizable: true,
            pageable: true,
            
            columnMenu: true,          
            //pageable: {
            //    refresh: true,
            //    buttonCount: 5
            //},
            //persistSelection: true,
            scrollable: {
                virtual: true
            },
            dataBound: function (e) {

    

            },

            columns: [                             
                {
                    field: "SCHEME_EDESC",
                    title: "Scheme",
                    width: 150
                },

                {
                    field: "CUSTOMER_EDESC",
                    title: "Customer",
                    width: 150
                },
                {
                    field: "CUSTOMER_CODE",
                    title: "Customer Code",
                    width: 150
                },
                {
                    field: "PARTY_TYPE_EDESC",
                    title: "Dealer",
                    width: 150
                },
                {
                    field: "PARTY_TYPE_CODE",
                    title: "Dealer Code",
                    width: 150
                },
                {
                    field: "SALES_SCHEME_VALUE",
                    title: "Bonus Amount",
                    width: 150
                },

                {
                    field: "TOTAL_SALES",
                    title: "Sales Amount",
                    width: 150
                },

                //{

                //    template: '<a class="edit glyphicon glyphicon-edit" title="Implement" ng-click="doManyThings(dataItem.SCHEME_CODE)" style="color:grey;"><span class="sr-only"></span> </a>',

                //    title: " ",
                //    width: "40px"
                //}
            ]
        };
    }
    $scope.bindSchemeDetailGridForImpact = function () {
        checkedItems = [];
        checkedCutomer = [];
        $scope.SchemeDetailCount = "Y";
        var DOCUMENT_SCHEME = {
            SCHEME_CODE: $scope.SchemeDetail.SCHEME_CODE,
            SCHEME_EDESC: $scope.SchemeDetail.SCHEME_EDESC,
            CUSTOMER_CODE: $scope.SchemeDetail.CUSTOMER_CODE,
            SCHEME_TYPE: $scope.SchemeDetail.SCHEME_TYPE,
            TYPE: $scope.SchemeDetail.TYPE,
            ITEM_CODE: $scope.SchemeDetail.ITEM_CODE,
            PARTY_TYPE_CODE: $scope.SchemeDetail.PARTY_TYPE_CODE,
            BRANCH_CODE: $scope.SchemeDetail.BRANCH_CODE,
            AREA_CODE: $scope.SchemeDetail.AREA_CODE,
            EFFECTIVE_FROM: $scope.SchemeDetail.EFFECTIVE_FROM,
            EFFECTIVE_TO: $scope.SchemeDetail.EFFECTIVE_TO,
        }
        $scope.schemeGridOptions = {
            toolbar: ["excel"],
            excel: {
                allPages: true
            },
            excelExport: function (e) {

                ExportToExcel(e);
                e.preventDefault();
            },
            dataSource: {
                transport: {
                    read: {
                        type: "POST",
                        url: "/api/TemplateApi/bindSchemeDetailForImpact",
                        contentType: "application/json; charset=utf-8",
                        dataType: 'json',

                    },
                    parameterMap: function (options, type) {
                        debugger;
                        var paramMap = JSON.stringify($.extend(options, { SchemeModel: DOCUMENT_SCHEME }));
                        delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                        delete paramMap.$format; // <-- remove format parameter.
                        return paramMap;
                    },
                },

                pageSize: 50,
                //serverPaging: false,
                //serverSorting: false,

            },

            //scrollable: true,
            //height: 350,
            //sortable: true,
            //resizable: true,
            pageable: true,

            columnMenu: true,
            //pageable: {
            //    refresh: true,
            //    buttonCount: 5
            //},
            //persistSelection: true,
            scrollable: {
                virtual: true
            },
            dataBound: function (e) {
                debugger;
                var griddata = $("#kimpactGrid").data("kendoGrid");
                var dataItems = griddata.dataSource._pristineData;
                //for (var i = 1; i < dataItems.length; i++) {
                //    var serialno = i;
                //    $scope.dynamicSubLedgerModalData.push({
                //        ACC_CODE: 0,
                //        TRANSACTION_TYPE: "",
                //        VOUCHER_AMOUNT: "",
                //        SUBLEDGER_AMOUNT: "",
                //        REMAINING_AMOUNT: "",
                //        SUBLEDGER: [{
                //            SERIAL_NO: serialno,
                //            SUB_CODE: "",
                //            SUB_EDESC: "",
                //            PARTY_TYPE_CODE: "",
                //            AMOUNT: "",
                //            PARTICULARS: "",
                //            REFRENCE: ""
                //        }]
                //    });
                //}
                $('#header-chb').on("change", selectallRow);
                $(".row-checkbox").on("click", selectRow);
                //$('#header-chb').change(function (ev) {

                //    var checked = ev.target.checked;

                //    /** Added this block of code **/
                //    kendo.ui.progress($("#kimpactGrid"), true);
                //    setTimeout(function () {
                //        kendo.ui.progress($("#kimpactGrid"), false);
                //    }, 15);
                //    /*******************************************/

                //    $('.row-checkbox').each(function (idx, item) {
                //        setTimeout(function () {

                //            if (checked) {
                //                if (!($(item).closest('tr').is('.k-state-selected'))) {
                //                    $(item).click();
                //                }
                //            } else {
                //                if ($(item).closest('tr').is('.k-state-selected')) {
                //                    $(item).click();
                //                }
                //            }
                //        }, 5);
                //    });
                //    //I commented out the below line
                //    //kendo.ui.progress($("#grid"), false);
                //});
                //$(".checkbox").on("click", selectRow);

            },

            columns: [
               
                {
                    title: 'Select All',
                    headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='row-checkbox_" + dataItem.CUSTOMER_CODE + "' class='row-checkbox'><label class='k-checkbox-label' for='${dataItem.SCHEME_CODE}'></label>"
                    },
                    width: 50
                },
                {
                    field: "SCHEME_EDESC",
                    title: "Scheme",
                    width: 150
                },

                {
                    field: "CUSTOMER_EDESC",
                    title: "Customer",
                    width: 150
                },
                {
                    field: "CUSTOMER_CODE",
                    title: "Customer Code",
                    width: 150
                },
                {
                    field: "PARTY_TYPE_EDESC",
                    title: "Dealer",
                    width: 150
                },
                {
                    field: "PARTY_TYPE_CODE",
                    title: "Dealer Code",
                    width: 150
                },
                {
                    field: "SALES_SCHEME_VALUE",
                    title: "Bonus Amount",
                    width: 150
                },

                {
                    field: "TOTAL_SALES",
                    title: "Sales Amount",
                    width: 150
                }

                //{

                //    template: '<a class="edit glyphicon glyphicon-menu-up" title="Implement" ng-click="ShowSubledgermodal(dataItem.SNO)" style="color:grey;"><span class="sr-only"></span> </a>',

                //    title: " ",
                //    width: "40px"
                //}
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
                var grid = $("#kimpactGrid").data("kendoGrid");
                var dataItem = datas = grid.dataSource._pristineData;
                for (var i = 0; i < dataItem.length; i++) {
                    checkedItems.push({
                        "ACCOUNT_CODE": dataItem[i].ACCOUNT_CODE,
                        "ACC_EDESC": dataItem[i].ACC_EDESC,
                        "AREA_CODE": dataItem[i].AREA_CODE,
                        "AREA_EDESC": dataItem[i].AREA_EDESC,
                        "BRANCH_CODE": dataItem[i].BRANCH_CODE,
                        "BRANCH_EDESC": dataItem[i].BRANCH_EDESC,
                        "CHARGE_ACCOUNT_CODE": dataItem[i].CHARGE_ACCOUNT_CODE,
                        "CHARGE_CODE": dataItem[i].CHARGE_CODE,
                        "CHARGE_EDESC": dataItem[i].CHARGE_EDESC,
                        "CHARGE_RATE": dataItem[i].CHARGE_RATE,
                        "CUSTOMER_CODE": dataItem[i].CUSTOMER_CODE,
                        "CUSTOMER_EDESC": dataItem[i].CUSTOMER_EDESC,
                        "EFFECTIVE_FROM": dataItem[i].EFFECTIVE_FROM,
                        "EFFECTIVE_TO": dataItem[i].EFFECTIVE_TO,
                        "ITEM_CODE": dataItem[i].ITEM_CODE,
                        "ITEM_EDESC": dataItem[i].ITEM_EDESC,
                        "PARTY_TYPE_CODE": dataItem[i].PARTY_TYPE_CODE,
                        "PARTY_TYPE_EDESC": dataItem[i].PARTY_TYPE_EDESC,
                        "SALES_DISCOUNT": dataItem[i].SALES_DISCOUNT,
                        "SALES_SCHEME_VALUE": dataItem[i].SALES_SCHEME_VALUE,
                        "SCHEME_CODE": dataItem[i].SCHEME_CODE,
                        "SCHEME_EDESC": dataItem[i].SCHEME_EDESC,
                        "SCHEME_TYPE": dataItem[i].SCHEME_TYPE,
                        "STATUS": dataItem[i].STATUS,
                        "TOTAL_SALES": dataItem[i].TOTAL_SALES,
                        "TYPE": dataItem[i].TYPE,
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
                grid = $("#kimpactGrid").data("kendoGrid"),
                dataItem = grid.dataItem(row);
            if (checked) {              
                     checkedItems.push({
                         "ACCOUNT_CODE": dataItem.ACCOUNT_CODE,
                         "ACC_EDESC": dataItem.ACC_EDESC,
                         "AREA_CODE": $scope.AREA_CODE,
                         "AREA_EDESC": dataItem.AREA_EDESC,
                         "BRANCH_CODE": dataItem.BRANCH_CODE,
                         "BRANCH_EDESC": dataItem.BRANCH_EDESC,
                         "CHARGE_ACCOUNT_CODE": dataItem.CHARGE_ACCOUNT_CODE,
                         "CHARGE_CODE": $scope.CHARGE_CODE,
                         "CHARGE_EDESC": dataItem.CHARGE_EDESC,
                         "CHARGE_RATE": dataItem.CHARGE_RATE,
                         "CUSTOMER_CODE": dataItem.CUSTOMER_CODE,
                         "CUSTOMER_EDESC": dataItem.CUSTOMER_EDESC,
                         "EFFECTIVE_FROM": $scope.EFFECTIVE_FROM,
                         "EFFECTIVE_TO": dataItem.EFFECTIVE_TO,
                         "ITEM_CODE": dataItem.ITEM_CODE,
                         "ITEM_EDESC": dataItem.ITEM_EDESC,
                         "PARTY_TYPE_CODE": dataItem.PARTY_TYPE_CODE,
                         "PARTY_TYPE_EDESC": $scope.PARTY_TYPE_EDESC,
                         "SALES_DISCOUNT": dataItem.SALES_DISCOUNT,
                         "SALES_SCHEME_VALUE": dataItem.SALES_SCHEME_VALUE,
                         "SCHEME_CODE": dataItem.SCHEME_CODE,
                         "SCHEME_EDESC": dataItem.SCHEME_EDESC,
                         "SCHEME_TYPE": dataItem.SCHEME_TYPE,
                         "STATUS": $scope.STATUS,
                         "TOTAL_SALES": dataItem.TOTAL_SALES,
                         "TYPE": dataItem.TYPE,
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
    }
    //$scope.ShowSubledgermodal = function (index) {
    //    debugger;
        
    //    $scope.popUp(index);
        
    //}
    //$scope.SubLedger_Ok = function (index, e) {
    //    debugger;
    //    $(".schemeSubledgerModal_" + index).toggle();
    //};
    //$scope.SubLedger_Cancel = function (index, e) {
    //    debugger;
    //    $scope.dynamicSubLedgerModalData = [{
    //        ACC_CODE: "",
    //        TRANSACTION_TYPE: "",
    //        VOUCHER_AMOUNT: "",
    //        SUBLEDGER_AMOUNT: "",
    //        REMAINING_AMOUNT: "",
    //        SUBLEDGER: [{
    //            SERIAL_NO: 1,
    //            SUB_CODE: "",
    //            SUB_EDESC: "",
    //            PARTY_TYPE_CODE: "",
    //            AMOUNT: "",
    //            PARTICULARS: "",
    //            REFRENCE: "",
    //            Descriptiontest: "",
    //        }]
    //    }];
    //    $(".schemeSubledgerModal_" + index).toggle();
    //};
    //$scope.Change = function (key,index, VOUCHER_AMOUNT) {
    //    debugger;
    //    var SubLength = $scope.dynamicSubLedgerModalData[key].SUBLEDGER.length;
    //    var subledgeramount = 0;
    //    if (VOUCHER_AMOUNT == undefined) {
    //        VOUCHER_AMOUNT = 0;
    //        $scope.dynamicSubLedgerModalData[key].VOUCHER_AMOUNT = 0;
    //    }
    //    else {
    //        $scope.dynamicSubLedgerModalData[key].VOUCHER_AMOUNT = VOUCHER_AMOUNT;
    //    }



    //    for (var i = 0; i < SubLength; i++) {
    //        debugger;
    //        var amt = $scope.dynamicSubLedgerModalData[key].SUBLEDGER[i].AMOUNT;
    //        if (amt != null && amt != "" && amt !== undefined) {
    //            $scope.dynamicSubLedgerModalData[key].SUBLEDGER[i].AMOUNT = parseFloat(amt.toFixed(2));
    //        }


    //        if (amt === undefined) {
    //            $(".SUBLEDGERAMOUNT_" + i).addClass("borderRed");
    //        }
    //        else {
    //            $(".SUBLEDGERAMOUNT_" + i).removeClass("borderRed");

    //        }

    //        if ($(".subledgeramounts").hasClass("borderRed")) {
    //            $(".subledgerok").addClass("disabled");
    //        }
    //        else {
    //            $(".subledgerok").removeClass("disabled");
    //        }

    //        if (amt != "") {
    //            var sa = parseFloat($scope.dynamicSubLedgerModalData[key].SUBLEDGER[i].AMOUNT);
    //            if (isNaN(sa)) {
    //                sa = 0;
    //            }
    //            subledgeramount = parseFloat((subledgeramount + sa).toFixed(2));
    //        }
    //        else {
    //            subledgeramount = subledgeramount;
    //        }
    //        $scope.apply;
    //    }
    //    var bonus_amount = $scope.dynamicSubLedgerModalData[key].SUBLEDGER_AMOUNT;
    //    //$scope.dynamicSubLedgerModalData[key].SUBLEDGER_AMOUNT = (subledgeramount);
    //    $scope.dynamicSubLedgerModalData[key].REMAINING_AMOUNT = parseFloat((bonus_amount - subledgeramount).toFixed(2));

    //    if ($scope.dynamicSubLedgerModalData[key].REMAINING_AMOUNT < 0) {
    //        $('.remainingamt').addClass("borderred");
    //    }
    //    else {
    //        $scope.errorshow = "";
    //        $('.remainingamt').removeClass("borderred");
    //    }
    //}
    function ExportToExcel(e) {
        debugger;
        var companyName = $scope.compname;
        
        var d = new Date();
        var strDate = d.getFullYear() + "/" + (d.getMonth() + 1) + "/" + d.getDate();
        
        var footer = [];
        footer.push({ value: "Date: " + strDate, colSpan: 4, textAlign: "right", bold: true })
        footer.push({ value: "Created By: " + $scope.LoginUser, colSpan: 4, textAlign: "left", bold: true })
        var footerTemp = e.workbook.sheets[0].rows[e.workbook.sheets[0].rows.length - 1].cells;
        for (var i = 0; i < footerTemp.length; i++) {
            //if (typeof (footerTemp[i].value) == "undefined" && i > 13)
            //    footer.push({ value: "", });
            //else if (typeof (footerTemp[i].value) != "undefined" && $(footerTemp[i].value).text() != "Total")
            //    footer.push({ value: $(footerTemp[i].value).text(), textAlign: "right", bold: true });
        }
        var SheetRow = [];
        //Pushing the head row
        SheetRow.push({
            cells: [
                { value: "Scheme", background: "#A9A7A6", },
                { value: "Item Name", background: "#A9A7A6", },
                { value: "Item Code", background: "#A9A7A6", },
                { value: "Customer Name", background: "#A9A7A6", },
                { value: "Customer Code", background: "#A9A7A6", },
                { value: "Dealer", background: "#A9A7A6", },
                { value: "Dealer Code", background: "#A9A7A6", },
                { value: "Earned", background: "#A9A7A6", },
                { value: "Consumed", background: "#A9A7A6", },              
            ],
        });
        WriteData(e.data);
        //recursive function to write the grouped/ungrouped data
        function WriteData(array) {
            array.forEach(function (row, index) {
                if (typeof (row.items) != "undefined") { //if array contains nested items, write a row with group field and enter recursion
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
                            value: row.SCHEME_EDESC,
                        }, {
                                value: row.ITEM_EDESC_MULTI,
                        }, {
                                value: row.ITEM_CODE,
                        }, {
                                value: row.CUSTOMER_EDESC,
                        }, {
                                value: row.CUSTOMER_CODE,
                        }, {
                                value: row.PARTY_TYPE_EDESC,
                        }, {
                                value: row.PARTY_TYPE_CODE,
                        }, {
                                value: row.SALES_SCHEME_VALUE,
                        }, {
                                value: row.TOTAL_SALES,
                        }]
                    });
                }
            });
        }
        //push the footer variable containing total values
        SheetRow.push({
            cells: footer,
        });

        //pushing Report header details
        SheetRow.unshift({
            cells: [{ value: "Scheme Report".split('').join(' '), colSpan: 9, textAlign: "center", bold: true}]
        });
        SheetRow.unshift({
            cells: [{ value: companyName.split('').join(' '), colSpan: 9, textAlign: "center", bold: true}]
        });
        //initializing a workbook
        var Workbook = new kendo.ooxml.Workbook({
            sheets: [{
                columns: [
                    { width: 150 }, { width: 250 }, { width: 75 }, { width: 250 }, { width: 75 }, { width: 200 }, { width: 75 }, { width: 100 }, { width: 100 }
//                        @for (int i = 0; i < 15; i++)
//{
//    <text>{autoWidth: true },</text>
//}
                    ],
rows: SheetRow
                }]
        });
        //finally saving the excel sheet
        kendo.saveAs({
            dataURI: Workbook.toDataURL(),
            fileName: "Scheme Report.xlsx"
        });
    }

    $scope.processAllScheme = function () {
        debugger;
        //$scope.SUB_LEDGER_VALUE_ARRAY = [];
        //var SubLedgerModaldata = $.grep($scope.dynamicSubLedgerModalData, function (e) {

        //    return e.ACC_CODE != 0;
        //});
        //if (SubLedgerModaldata.length > 0) {
        //    $scope.SUB_LEDGER_VALUE_ARRAY = SubLedgerModaldata;
        //}
        //else {
        //    $scope.SUB_LEDGER_VALUE_ARRAY = [];
        //}
        //grid = $("#kimpactGrid").data("kendoGrid");
        //datas = grid.dataSource._pristineData;
        datas = checkedItems;
        //var model = {
        //    SCHEME_IMPLEMENT_VALUE: JSON.stringify(datas),
        //    SUB_LEDGER_VALUE: JSON.stringify($scope.SUB_LEDGER_VALUE_ARRAY),
        //};
        if (datas.length > 0) {
            var saveSchemeUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/ImpactVoucherByScheme";
            $http({
                method: 'POST',
                url: saveSchemeUrl,
                data: datas
            }).then(function successCallback(response) {
                debugger;
                if (response.data.MESSAGE == "INSERTED") {

                    displayPopupNotification("Data succesfully saved ", "success");
                    if ($("#kimpactGrid").data("kendoGrid") != null) {
                        $("#kimpactGrid").data("kendoGrid").dataSource.read();

                    }

                }
                //else if (response.data.MESSAGE == "NOTMAPPED") {
                else if (response.data.MESSAGE.includes("NOTMAPPED")) {
                    debugger;
                    var splitedres = response.data.MESSAGE.split("_");
                    if (splitedres[1] == "C") {
                        displayPopupNotification("Impact not Created.Please map Customer." + "Customer Code:" + splitedres[2] + "and try", "error");
                    }
                    else {
                        displayPopupNotification("Impact not Created.Please map Dealer." + "Party Type Code:" + splitedres[2] + "and try", "error");
                    }
                    //displayPopupNotification("Impact not Created.Please map Dealer or customer and try again.","error");
                    //if (response.data.PARTY_TYPE_CODE != "") {
                    //    displayPopupNotification("Impact not Created.Please map Dealer (" + "Code:" + response.data.PARTY_TYPE_CODE + ",Name:" + response.data.PARTY_TYPE_NAME +") and try again.", "error");
                    //}
                    //if (response.data.CUSTOMER_CODE != "") {
                    //    displayPopupNotification("Impact not Created.Please map Customer (" + "Code:" + response.data.CUSTOMER_CODE + ",Name:" + response.data.CUSTOMER_NAME + ") and try again.", "error");
                    //}

                }
                else if (response.data.MESSAGE == "EMPTYDATAFAIL")
                    {
                    displayPopupNotification("Query resulted empty data.Please try again later.", "error");
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
    };
});

