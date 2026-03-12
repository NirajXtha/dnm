

DTModule.controller('refrenceCodeCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter, formtemplateservice) {

    $scope.showRefSearch = true;
    var checkedCutomer = [];
    var global_customerCode = '';
    $rootScope.refrenceCodeDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllOrederNoByFlter"
            },
            parameterMap: function (data, action) {
                var newParams;

                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            FormCode: $scope.ref_form_code,
                            filter: data.filter.filters[0].value,
                            Table_name: $scope.RefTableName,
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            FormCode: $scope.ref_form_code,
                            filter: "",
                            Table_name: $scope.RefTableName,
                        };
                        return newParams;
                    }
                }
                else {
                    $scope.RefTableName;
                    newParams = {
                        FormCode: $scope.ref_form_code,
                        filter: "",
                        Table_name: $scope.RefTableName,
                    };
                    return newParams;
                }
            }
        },
    };


    $rootScope.refrenceCodeOption = {
        dataSource: $scope.refrenceCodeDataSource,
        //template: '<span>{{dataItem.ORDER_EDESC}}</span>  --- ' +
        //'<span>{{dataItem.Type}}</span>',
        dataTextField: 'ORDER_EDESC',
        dataValueField: 'ORDER_CODE',
        filter: 'contains',
        close: function (e) {

            var dataItem = $("#refrencetype").data('kendoComboBox').dataItem();
            if (dataItem == "undefined" || dataItem == "" || dataItem == null) {
                return;
            }
            else {
                setTimeout(function () {

                    if ($("#refrencetype").data('kendoComboBox').dataItem() != undefined) {
                        var orderNo = dataItem.ORDER_CODE;
                        var defered = $.Deferred();
                        showloader();
                        var saleOrderformDetail = formtemplateservice.getSalesOrderDetail_ByFormCodeAndOrderNo($scope.ref_form_code, orderNo, defered);
                        $.when(defered).done(function (result) {

                            var response = [];
                            response = result;
                            if ($scope.ModuleCode == "02") {
                                $scope.inventoryRefrenceFn(response, function () {
                                    hideloader();
                                    $(".btn-action a").css('display', 'none')
                                });
                            }
                            else if ($scope.ModuleCode == "04") {
                                $scope.refrenceFn(response, function () {

                                    hideloader();

                                    if ($scope.freeze_master_ref_flag == "Y") {
                                        $(".btn-action a").css('display', 'none')
                                    }
                                    if ($scope.freeze_master_ref_flag == "N") {
                                        $(".btn-action a").css('display', 'inline')
                                    }
                                });
                            }


                        });
                    }
                }, 0);

            }
        },
        dataBound: function (e) {

        },

        change: function (e) {

        }
    }
    $scope.refrenceCodeOnChange = function (kendoEvent) {
        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.refrenceerror = "Please Enter Valid Code."
            $('#refrencetype').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.refrenceerror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    };

    var purl;
    if ($scope.RefTableName == 'SA_SALES_ORDER') {
        purl = "GetAllCustomerSetupByFilter";
    }
    else if ($scope.RefTableName == '') {
        purl = "GetAllSupplierForReferenceByFilter";
    }

    $scope.refCustomerDataSource = {
        type: "json",
        serverFiltering: true,
        suggest: true,
        highlightFirst: true,
        transport: {
            read: {
                //url: "/api/TemplateApi/" + purl,
                url: "/api/TemplateApi/GetAllCustomerSetupByFilter",
            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        //newParams = {
                        //    filter: data.filter.filters[0].value
                        //};
                        //return newParams;
                        if (data.filter.filters[0].value != "") {
                            newParams = {
                                filter: data.filter.filters[0].value
                            };
                            return newParams;
                        }
                        else {
                            newParams = {
                                filter: "!@$"
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
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        },
    };

    $scope.DocumentReference = [];
    $scope.DocumentReference = {
        DOCUMENT: "",
        TEMPLATE: "",
        FROM_DATE: "",
        TO_DATE: "",
        NAME: "",
        ITEM_DESC: "",
        VOUCHER_NO: "",
        SEARCHED_CUSTOMER_CODE: "",
        INCLUDE_CHARGE: "",


    };

    $scope.refCustomerCodeOption = {
        dataSource: $scope.refCustomerDataSource,
        template: '<span>{{dataItem.CustomerName}}</span>  --- ' +
            '<span>{{dataItem.Type}}</span>',
        dataBound: function (e) {

            if (this.element[0].attributes['customer-index'] == undefined) {
                var customer = $("#referenceCustomer").data("kendoComboBox");
            }
            else {
                var index = this.element[0].attributes['customer-index'].value;
                var customerLength = ((parseInt(index) + 1) * 3) - 1;
                var customer = $($("#referenceCustomer")[customerLength]).data("kendoComboBox");

            }
            if (customer != undefined) {
                customer.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(CustomerName, Type, this.text()) #"), customer)
                });
            }
        },
        //change: function (e) {
        //    //$('#RefrenceModel').modal('show');
        //    
        //    $('#customerWiseRefrenceModel').modal('show');
        //    setTimeout(function () {
        //        $("#custGridbindSearch").trigger('click')
        //    }, 0);

        //},
        close: function (e) {

            var dataItem = $("#referenceCustomer").data('kendoComboBox').dataItem();
            //if (this.dataItem(e.item) == undefined)
            //{
            //    return $("#referenceCustomer").data('kendoComboBox').value('');
            //}
            if (dataItem == undefined)
                return $("#referenceCustomer").data('kendoComboBox').value('');
            setTimeout(function () {
                var isUndefined = $("#referenceCustomer").data('kendoComboBox').dataItem();
                if (isUndefined != undefined) {

                    $("input[name=namerow][value='nonereference']").prop("checked", true);
                    $('#customerWiseRefrenceModel').modal('show');
                    $("#custGridbindSearch").trigger('click')
                }
            }, 0);
        }
    }


    var refd = $.Deferred();
    $scope.formDetailData;
    $scope.gridChildColumn = [];

    $scope.dynamicCol = [];

    $scope.Name = "";


    $scope.searchedCustomerCode = '';

    $scope.OrAndList = [{
        andorval: 'and',
        andorname: 'AND',
        status: "Active"
    }, {
        andorval: 'or',
        andorname: 'OR',
        status: "Inactive"
    }]

    $scope.dynamicCol = [
        {
            id: 1,
            //ORAND: '',
            COLUMN_NAME: '',
            COLUMN_VALUE: '',
            readonly: true
        }
    ];

    //Row
    $scope.ROW = {
        radio: 'nonrefrence'
    };
    $scope.ROW = {
        radio: 'incomplete'
    };
    $scope.ROW = {
        radio: 'all'
    };

    //REFERENCE_QUALITY
    $scope.REFERENCE_QUALITY = {
        radio: 'nonrefrence'
    };
    $scope.REFERENCE_QUALITY = {
        radio: 'incomplete'
    };



    var checkedItems = [];
    var checkedIds = {};

    var checkedItemsCount = [];


    $scope.custBindReferenceGrid = function () {

        checkedItems = [];
        var column = generateColumn($scope.RefTableName);
        var groupfield = generateGroupFieldName($scope.RefTableName);

        var DOCUMENT_REFERENCE = {
            FORM_CODE: $scope.DocumentReference.TEMPLATE == "" ? $scope.ref_form_code : $scope.DocumentReference.TEMPLATE,
            TABLE_NAME: $scope.RefTableName,
            NAME: $("#referenceCustomer").data('kendoComboBox').dataItem().CustomerCode,
        };

        $scope.custReferenceGridOptions = {
            dataSource: {
                transport: {
                    read: {
                        type: "POST",
                        url: "/api/TemplateApi/bindReferenceGrid",
                        contentType: "application/json; charset=utf-8",
                        dataType: 'json'
                    },
                    parameterMap: function (options, type) {
                        var paramMap = JSON.stringify($.extend(options, { referenceModel: DOCUMENT_REFERENCE }));
                        delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                        delete paramMap.$format; // <-- remove format parameter.
                        return paramMap;
                    },
                },
                group: {
                    field: groupfield,
                    //template: "<input type='checkbox' class='checkbox' #= Discontinued ? checked='checked' : '' # />",
                    headerTemplate: "<input type='checkbox' id='chkSelectAll' onclick='checkAll(this)'/>",


                },
                pageSize: 50,
                serverPaging: false,
                serverSorting: false,
                schema: {
                    model: {
                        fields: {
                            //CREATED_DATE: { type: "date" },
                            //LC_TRACK_NO: { type: "number" },
                            //EST_DAY: { type: "number" },

                            //ORDER_NO: { type: "string" },
                            //ORDER_DATE: { type: "date" },
                            QUANTITY: { type: "number" },
                            UNIT_PRICE: { type: "number" },
                            TOTAL_PRICE: { type: "number" },
                        }
                    },
                },
            },
            toolbar: kendo.template($("#toolbar-template").html()),
            sortable: true,
            pageable: true,
            height: 500,
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
            },
            persistSelection: true,
            scrollable: {
                virtual: true
            },
            dataBound: function (e) {
                $('div').removeClass('.k-header k-grid-toolbar');

                $(".checkbox").on("click", selectRow);

                var view = this.dataSource.data();
                for (var j = 0; j < checkedItems.length; j++) {
                    for (var i = 0; i < view.length; i++) {
                        if (checkedItems[j].VOUCHER_NO == view[i].VOUCHER_NO && checkedItems[j].SERIAL_NO == view[i].SERIAL_NO) {
                            this.tbody.find("tr[data-uid='" + view[i].uid + "']")
                                .addClass("k-state-selected")
                                .find(".checkbox")
                                .attr("checked", "checked");
                        }
                    }
                }


            },
            columns: column,
        };

        //on click of the checkbox:


        function selectRow() {

            var checked = this.checked,
                row = $(this).closest("tr"),
                grid = $("#custReferenceGrid").data("kendoGrid"),
                dataItem = grid.dataItem(row);

            if (checked) {
                row.addClass("k-state-selected");
                $(this).attr('checked', true);
                checkedIds[dataItem.VOUCHER_NO] = checked;
                var CustomerId = "";

                switch (switch_on) {

                    case "SA_SALES_ORDER":
                    case "SA_SALES_INVOICE":
                    case "SA_SALES_CHALAN":
                        //if (_.contains(checkedCutomer, dataItem.CUSTOMER_CODE)) {
                        //    checkedCutomer.push({ dataItem.CUSTOMER_CODE });
                        //}
                        //else {
                        //    return displayPopupNotification("Please choose Same Customer.", "warning");
                        //    return false;
                        //}
                        break;
                    default:
                }

                //break;
                return;
                checkedItems.push({
                    "VOUCHER_NO": dataItem.VOUCHER_NO,
                    "SERIAL_NO": dataItem.SERIAL_NO,
                    "TABLE_NAME": $scope.RefTableName,
                    "ITEM_CODE": dataItem.ITEM_CODE,
                    "REF_FORM_CODE": dataItem.FORM_CODE


                });
            } else {
                for (var i = 0; i < checkedItems.length; i++) {
                    if (checkedItems[i].VOUCHER_NO == dataItem.ORDER_NO && checkedItems[i].SERIAL_NO == dataItem.SERIAL_NO) {
                        checkedItems.splice(i, 1);
                    }
                }
                row.removeClass("k-state-selected");
            }
        }
    }

    //Grid Binding main Part
    $scope.bindReferenceGrid = function () {

        checkedItems = [];
        checkedCutomer = [];
        //if (valid) {

        var refrencytype = $('#refrenceTypeMultiSelect').val();
        var templatetype = $('#TemplateTypeMultiSelect').val();

        //var document = refrencytype == "null" ? $scope.RefTableName : refrencytype.toString();
        var document = refrencytype == null ? $scope.RefTableName : refrencytype == "null" ? $scope.RefTableName : refrencytype == "0" ? $scope.RefTableName : refrencytype.toString();
        var template = templatetype == null ? 'ALL' : templatetype.toString();//$('#TemplateTypeMultiSelect').val().toString();
        var referencetypebutton = $('input[name=namerefrence]:checked').val();
        var rowbutton = $('input[name=namerow]:checked').val()
        var tableName = $scope.RefTableName;
        var column = generateColumn(tableName);
        var groupfieldn = generateGroupFieldName(tableName);

        if ($scope.DocumentReference.TEMPLATE == "") {
            $scope.DocumentReference.TEMPLATE = "0";
        }
        var col_filter = [];
        angular.forEach($scope.dynamicCol, function (val, key) {
            col_filter.push({
                COLUMN_NAME: val.COLUMN_NAME,
                COLUMN_VALUE: val.COLUMN_VALUE,
                ORAND: val.ORAND
            })
        })

        if (document === 'SA_LOADING_SLIP_DETAIL') {
            $scope.RefTableName = 'SA_LOADING_SLIP_DETAIL';
        }

        var columnsFilter = col_filter;
        var DOCUMENT_REFERENCE = {
            FORM_CODE: $scope.DocumentReference.TEMPLATE == "0" ? $scope.ref_form_code : $scope.DocumentReference.TEMPLATE,
            TABLE_NAME: $scope.RefTableName,
            DOCUMENT: document,
            TEMPLATE: template,
            ROW: rowbutton,
            REFERENCE_QUALITY: referencetypebutton,
            FROM_DATE: moment($("#FromDateVoucher").val()).format('YYYY-MM-DD'),
            TO_DATE: moment($("#ToDateVoucher").val()).format('YYYY-MM-DD'),
            NAME: $scope.DocumentReference.NAME,
            ITEM_DESC: $scope.DocumentReference.ITEM_DESC,
            VOUCHER_NO: $scope.DocumentReference.VOUCHER_NO,
            COLUMNS_FILTER: columnsFilter,



        };
        //when select all in dropdown named template in refernce
        $scope.DocumentReference.TEMPLATE == "0" ? DOCUMENT_REFERENCE.FORM_CODE = null : DOCUMENT_REFERENCE.FORM_CODE = $scope.DocumentReference.TEMPLATE;
        $scope.referenceGridOptions = {

            dataSource: {
                transport: {
                    read: {
                        type: "POST",
                        url: "/api/TemplateApi/bindReferenceGrid",
                        contentType: "application/json; charset=utf-8",
                        dataType: 'json'
                    },
                    parameterMap: function (options, type) {

                        var paramMap = JSON.stringify($.extend(options, { referenceModel: DOCUMENT_REFERENCE }));
                        delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                        delete paramMap.$format; // <-- remove format parameter.
                        return paramMap;
                    },
                },
                group: {
                    //field: "ORDER_NO",
                    //field: "CUSTOMER_CODE",
                    field: groupfieldn,
                },
                pageSize: 50,
                serverPaging: false,
                serverSorting: false,
                schema: {
                    model: {
                        fields: {
                            //CREATED_DATE: { type: "date" },
                            //LC_TRACK_NO: { type: "number" },
                            //EST_DAY: { type: "number" },

                            //ORDER_NO: { type: "string" },
                            //ORDER_DATE: { type: "date" },
                            QUANTITY: { type: "number" },
                            UNIT_PRICE: { type: "number" },
                            TOTAL_PRICE: { type: "number" },
                        }
                    },
                },
            },

            toolbar: kendo.template($("#toolbar-template").html()),

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
            },
            //pageable: {
            //    refresh: true,
            //    buttonCount: 5
            //},
            persistSelection: true,
            scrollable: {
                virtual: true
            },
            dataBound: function (e) {
                $(".k-grouping-row").click(function (e) {
                    debugger;
                    var expanded = $(this)[0].innerText.split(":").pop();
                    var customernameurl = "/api/TemplateApi/getCustomerCode?customeredesc=" + expanded;
                    $http.get(customernameurl).then(function (response) {


                        selectGroupRow(response.data);

                    });
                })
                //$scope.detailExportPromises = [];
                $('div').removeClass('.k-header k-grid-toolbar');

                $(".checkbox").on("click", selectRow);

                var view = this.dataSource.data();
                for (var j = 0; j < checkedItems.length; j++) {
                    for (var i = 0; i < view.length; i++) {
                        if (checkedItems[j].VOUCHER_NO == view[i].VOUCHER_NO && checkedItems[j].SERIAL_NO == view[i].SERIAL_NO) {
                            this.tbody.find("tr[data-uid='" + view[i].uid + "']")
                                .addClass("k-state-selected")
                                .find(".checkbox")
                                .attr("checked", "checked");
                        }
                    }
                    //if (checkedIds[view[i].ORDER_NO]) {
                    //    this.tbody.find("tr[data-uid='" + view[i].uid + "']")
                    //        .addClass("k-state-selected")
                    //        .find(".checkbox")
                    //        .attr("checked", "checked");
                    //}
                }
                var grid = e.sender;
                if (grid.dataSource.total() == 0) {
                    var colCount = grid.columns.length + 1;
                    $(e.sender.wrapper)
                        .find('tbody')
                        .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, No Data Found For Given Filter. </td></tr>');
                    //displayPopupNotification("No Data Found Given Date Filter.", "info");
                }

            },

            columns: column,
        };

        //on click of the checkbox:
        function selectRow() {

            var checked = this.checked,
                row = $(this).closest("tr"),
                grid = $("#referenceGrid").data("kendoGrid"),
                dataItem = grid.dataItem(row);
            //checkedIds[dataItem.ORDER_NO] = checked;
            //checkedItems.push({
            //    "VOUCHER_NO": dataItem.ORDER_NO,
            //    "SERIAL_NO": dataItem.SERIAL_NO,
            //});
            //if (checkedItems.length > 0) {
            //    checkedIds[checkedItems[0].VOUCHER_NO] = false;
            //    checkedItems = [];
            //}
            //else {
            //    checkedItems.push({
            //        "VOUCHER_NO": dataItem.ORDER_NO,
            //        "SERIAL_NO": dataItem.SERIAL_NO,
            //    });
            //}

            if (checked) {
                //-select the row
                //checkedItemsCount.push(dataItem.ORDER_NO);
                //checkedItemsCount = _.uniq(checkedItemsCount);
                //if (checkedItemsCount.length > 1) {
                //    checkedIds = {};
                //    //checkedItems = [];
                //    checkedItemsCount = [];
                //    //$($("#referenceGrid").find('table')[1]).find('tbody tr').removeClass('k-state-selected');
                //    //$('input:checkbox').removeAttr('checked');
                //    checkedItemsCount.push(dataItem.ORDER_NO);
                //}
                if (checkedCutomer.length > 0) {
                    if (jQuery.inArray(dataItem.CUSTOMER_CODE, checkedCutomer) === -1) {
                        checkedCutomer.push(dataItem.CUSTOMER_CODE);
                        return false;
                    }
                    else {

                    }
                }
                else {
                    checkedCutomer.push(dataItem.CUSTOMER_CODE);
                }


                row.addClass("k-state-selected");
                $(this).attr('checked', true);
                checkedIds[dataItem.ORDER_NO] = checked;
                checkedItems.push({
                    "VOUCHER_NO": dataItem.VOUCHER_NO,
                    "SERIAL_NO": dataItem.SERIAL_NO,
                    "TABLE_NAME": $scope.RefTableName,
                    "ITEM_CODE": dataItem.ITEM_CODE,
                    "REF_FORM_CODE": dataItem.FORM_CODE

                });
            } else {
                //-remove selection
                for (var i = 0; i < checkedItems.length; i++) {
                    if (checkedItems[i].VOUCHER_NO == dataItem.VOUCHER_NO && checkedItems[i].SERIAL_NO == dataItem.SERIAL_NO) {
                        checkedItems.splice(i, 1);
                    }
                }
                row.removeClass("k-state-selected");
            }
        }
        function selectGroupRow(id) {
            debugger;
            //row.addClass("k-state-selected");
            $('.row-checkbox_' + id).attr('checked', true);
            grid = $("#referenceGrid").data("kendoGrid");
            var allgroupcostomer = $.grep(grid._data, function (e) {
                return e.CUSTOMER_CODE == id;
            });
            $.each(allgroupcostomer, function (key, value) {
                debugger;
                //checkedIds[value.ORDER_NO] = checked;
                checkedItems.push({
                    "VOUCHER_NO": value.VOUCHER_NO,
                    "SERIAL_NO": value.SERIAL_NO,
                    "TABLE_NAME": $scope.RefTableName,
                    "ITEM_CODE": value.ITEM_CODE,
                    "REF_FORM_CODE": value.FORM_CODE

                });
            });

        }
        //}
        //else {
        //    displayPopupNotification("Please Fill the required fields", "warning")
        //}
    }

    $scope.ReSetCheck = function () {
        checkedItems = [];
        checkedCutomer = [];
    }
    $scope.tableColDataSource = {
        type: "json",
        //serverFiltering: true,
        transport: {
            read: {
                url: "/api/QueryBuilder/GetColumsListByTableName?tablesName=SA_SALES_ORDER",
            },
        }
    };

    $scope.tableColumnList = [];
    $http({
        method: 'GET',
        url: '/api/QueryBuilder/GetColumsListByTableName?tablesName=SA_SALES_ORDER'
    }).then(function successCallback(response) {
        $scope.tableColumnList.push(response.data.DATA);
        $scope.tableColumnOption = {
            dataSource: $scope.tableColumnList[0],
            dataBound: function () {
            },
            change: function (e) {
                var dataType = e.sender.dataItem().dataType;
                var defaultInput = '<input type="text" ng-model="col.COLUMN_VALUE" class="form-control colValue" />';
                $(e.sender.element[0]).closest('td').next('td').find('input').remove();
                $(e.sender.element[0]).closest('td').next('td').append(defaultInput);
                switch (dataType) {
                    case 'NUMBER':
                        $($(e.sender.element[0]).closest('td').next('td').find('input'))[0].type = 'number';
                        break;
                    case 'DATE':
                        $($($(e.sender.element[0]).closest('td').next('td').find('input'))[0]).addClass('maskdate');
                        break;
                    default:
                        $($(e.sender.element[0]).closest('td').next('td').find('input'))[0].type = 'text';
                }
            }
        }
    }, function errorCallback(response) {
    });

    //$scope.tableColumnOption = {
    //    dataSource: $scope.tableColDataSource,
    //    dataBound: function () {
    //        
    //    }
    //}

    $scope.add_col_reference = function (indx, e) {
        var rowCount = $scope.dynamicCol.length;
        $scope.dynamicCol.push({
            id: rowCount + 1,
            ORAND: $scope.OrAndList[0].andorval,
            COLUMN_NAME: '',
            COLUMN_VALUE: '',
            readonly: false
        })
    }
    $scope.remove_col_reference = function (index) {
        if (index === 0)
            return displayPopupNotification("You can not delete first row.", "warning");
        if ($scope.dynamicCol.length > 1) {
            $scope.dynamicCol.splice(index, 1);
        }
    }

    $scope.bindRefrenceDataToTemplate = function () {
        debugger;
        $rootScope.IncludeCharge = $('input[name=IncludeCharge]:checked').val() == undefined ? "False" : "True";
        checkedItems;
        //var serialNo = $.map(checkedItems, function (obj) {
        //    return obj.SERIAL_NO
        //}).join("','")
        //var voucherNo = $.map(checkedItems, function (obj) {
        //    return obj.VOUCHER_NO
        //}).join("','")
        $rootScope.refCheckedItem = checkedItems;

        if (checkedItems.length <= 0)
            return displayPopupNotification("Please choose the item.", "warning");
        showloader();
        getVoucherDetailForReference(checkedItems);
        $("#RefrenceModel").modal('hide');
        $('#customerWiseRefrenceModel').modal('hide');

    }

    $scope.btnRefrenceCancel = function () {

        $("#refrenceTypeMultiSelect").data('kendoDropDownList').value('');
        $('#referenceGrid').empty();
    }

    function getVoucherDetailForReference(checkedItems) {
        debugger;
        var tableName = $scope.RefTableName;
        //var tableName = $("#refrenceTypeMultiSelect").data("kendoDropDownList").value() == "" ? $scope.RefTableName : $("#refrenceTypeMultiSelect").data("kendoDropDownList").text();
        if ($("#refrenceTypeMultiSelect").data("kendoDropDownList").value() == "0")
            tableName = $scope.RefTableName;
        if (tableName == undefined || tableName == "" || tableName == null || tableName == "-- Select Document --")
            tableName == "";
        var formCode = ($scope.DocumentReference.TEMPLATE == "" || $scope.DocumentReference.TEMPLATE == "0") ? $scope.ref_form_code : $scope.DocumentReference.TEMPLATE;
        var rowbutton = "";
        if (!$scope.fromCustomer)
            rowbutton = $('input[name=namerow]:checked').val()
        var model = {
            checkList: checkedItems,
            FormCode: formCode,
            TableName: tableName == "" ? $scope.RefTableName : tableName.toString(),
            ROW: rowbutton,
            //if include charge is set true if also multiple voucher no is selected, single voucher no's transaction with its charge is shown.
            INCLUDE_CHARGE: $('input[name=IncludeCharge]:checked').val() == undefined ? "False" : "True"
            //ModuleCode: $scope.ModuleCode,
            //voucherNo: voucherNo,
            //serialNo: serialNo,
            //formCode: $scope.FormCode,
            //tableName: $("#refrenceTypeMultiSelect").val().toString()
        }
        var url = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetVoucherDetailForReferenceEdit";

        var response = $http({
            method: "POST",
            data: model,
            url: url,
            contentType: "application/json",
            dataType: "json"
        });
        return response.then(function (data) {
            debugger;
            response = data;
            var modulecode = $scope.ModuleCode;
            if (response.data.length <= 0) {
                hideloader();
                return;
            }
            if (modulecode == undefined || modulecode == "" || modulecode == null) {
                hideloader();
                return;

            }
            else {
                if (modulecode == "04") {

                    $scope.refrenceFn(response, function () {
                        //var rowwss=$rootScope.refrencedata;

                        angular.forEach($scope.masterModels,
                            function (mvalue, mkey) {

                                if (mkey === "CUSTOMER_CODE") {
                                    //old code
                                    //var req = "/api/TemplateApi/getCustEdesc?code=" + mvalue;
                                    //$http.get(req).then(function (results) {
                                    //    setTimeout(function () {

                                    //        $("#customers").data('kendoComboBox').dataSource.data([{ CustomerCode: mvalue, CustomerName: results.data, Type: "code" }]);
                                    //    }, 0);
                                    //});
                                    //New change for SRSteel
                                    var req = "/api/TemplateApi/GetCustomerInfoByCode?filter=" + mvalue;
                                    $http.get(req).then(function (results) {
                                        setTimeout(function () {

                                            if (results.data.length > 0) {
                                                $("#customers").data('kendoComboBox').dataSource.data([{ CustomerCode: mvalue, CustomerName: results.data[0].CustomerName, REGD_OFFICE_EADDRESS: results.data[0].REGD_OFFICE_EADDRESS, TEL_MOBILE_NO1: results.data[0].TEL_MOBILE_NO1, TPIN_VAT_NO: results.data[0].TPIN_VAT_NO, Type: "code" }]);
                                            }
                                        }, 0);
                                    });
                                }
                            });
                        angular.forEach($scope.childModels, function (cval, ckey) {

                            if (cval.hasOwnProperty("ITEM_CODE")) {
                                var ireq = "/api/TemplateApi/getItemEdesc?code=" + cval.ITEM_CODE;
                                $http.get(ireq).then(function (results) {
                                    setTimeout(function () {

                                        $("#products_" + ckey).data('kendoComboBox').dataSource.data([{ ItemCode: cval.ITEM_CODE, ItemDescription: results.data, Type: "code" }]);
                                    }, 0);
                                });
                            }


                        });
                        hideloader();
                        if ($scope.freeze_master_ref_flag == "Y") {
                            $(".btn-action a").css('display', 'none')
                        }
                        if ($scope.freeze_master_ref_flag == "N") {
                            $(".btn-action a").css('display', 'inline')
                        }
                    });
                }
                else if (modulecode == "02") {

                    $scope.inventoryRefrenceFn(response, function () {
                        hideloader();
                        $(".btn-action a").css('display', 'none')
                    });


                }
                else if (modulecode == "01") {

                    $scope.refrencefinanceFn(response, function () {
                        hideloader();
                        $(".btn-action a").css('display', 'none')
                    });


                }
            }

        })

        //$http.post(url).then(function (result) {
        //    
        //    response = result;
        //    $scope.refrenceFn(response);
        //});
    };

    function generateColumn(tableName) {

        var colName;
        if (tableName == "SA_SALES_ORDER") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox_" + dataItem.CUSTOMER_CODE + "'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                //{
                //    field: "SERIAL_NO",
                //},
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },
                {
                    field: "ITEM_EDESC",
                    title: "Item"
                }
                ,
                {
                    field: "CUSTOMER_EDESC",
                    title: "Customer"
                }
                ,
                {
                    field: "MU_CODE",
                    title: "Unit"
                }

                ,
                {
                    field: "QUANTITY",
                    title: "Quantity",
                    attributes: { style: "text-align:right;" },
                }
                ,
                {
                    field: "UNIT_PRICE",
                    title: "Unit Price",
                    attributes: { style: "text-align:right;" },
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Total Price",
                    attributes: { style: "text-align:right;" },
                }
                ,
                {
                    field: "REMARKS",
                    title: "REMARK"
                },
                {
                    field: "FORM_CODE",
                    title: "FORM CODE"

                }
            ]
            //colName = "ORDER_NO, ORDER_DATE, ITEM_CODE, CUSTOMER_CODE, MU_CODE, QUANTITY, UNIT_PRICE, TOTAL_PRICE";
        }
        else if (tableName == "SA_SALES_INVOICE") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox_" + dataItem.CUSTOMER_CODE + "'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    //template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },
                {
                    field: "CUSTOMER_EDESC",
                    title: "Customer"
                }
                ,
                {
                    field: "ITEM_EDESC",
                    title: "Item"
                }
                ,
                {
                    field: "MU_CODE",
                    title: "Unit"
                }
                ,
                {
                    field: "QUANTITY",
                    title: "Quantity"
                }
                ,
                {
                    field: "UNIT_PRICE",
                    title: "Unit Price"
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Total Price"
                }
                ,
                {
                    field: "REMARKS",
                    title: "Remarks"
                },

                {
                    field: "FORM_CODE",
                    title: "FORM CODE"

                }

            ];
        }
        //else if (tableName == "SA_SALES_INVOICE") {
        //    colName = [
        //        {
        //            field: "VOUCHER_NO",
        //            title: "Voucher No"
        //        },
        //        {
        //            field: "VOUCHER_DATE",
        //            title: "Voucher Date",
        //            template: "#= SALES_DATE == null ? '' :kendo.toString(kendo.parseDate(SALES_DATE),'M/dd/yyyy') #",
        //        },
        //        {
        //            field: "CUSTOMER_EDESC",
        //            title: "Customer"
        //        }
        //        ,
        //        {
        //            field: "ITEM_EDESC",
        //            title: "Item"
        //        }
        //        ,
        //        {
        //            field: "MU_CODE",
        //            title: "Unit"
        //        }
        //        ,
        //        {
        //            field: "QUANTITY",
        //            title: "Quantity"
        //        }
        //        ,
        //        {
        //            field: "UNIT_PRICE",
        //            title: "Unit Price"
        //        }
        //        ,
        //        {
        //            field: "TOTAL_PRICE",
        //            title: "Total Price"
        //        }
        //        ,
        //        {
        //            field: "REMARKS",
        //            title: "Remarks"
        //        }

        //    ];
        //}
        else if (tableName == "SA_SALES_CHALAN") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox_" + dataItem.CUSTOMER_CODE + "'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },
                {
                    field: "CUSTOMER_CODE",
                    title: "Customer"
                }
                ,
                {
                    field: "FROM_LOCATION_CODE",
                    title: "From Location"
                }
                ,

                {
                    field: "MU_CODE",
                    title: "Unit"
                }
                ,
                {
                    field: "QUANTITY",
                    title: "Quantity"
                }
                ,
                {
                    field: "UNIT_PRICE",
                    title: "Unit Price"
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Total Price"
                }
                ,
                {
                    field: "REMARKS",
                    title: "Remarks"
                },
                {
                    field: "FORM_CODE",
                    title: "FORM CODE"

                }

            ];
        }
        else if (tableName == "SA_SALES_RETURN") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox_" + dataItem.CUSTOMER_CODE + "'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },
                {
                    field: "CUSTOMER_CODE",
                    title: "Customer"
                }
                ,
                {
                    field: "ITEM_CODE",
                    title: "Item Code"
                }
                ,

                {
                    field: "MU_CODE",
                    title: "Unit"
                }
                ,
                {
                    field: "QUANTITY",
                    title: "Quantity"
                }
                ,
                {
                    field: "UNIT_PRICE",
                    title: "Unit Price"
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Total Price"
                }
                ,
                {
                    field: "REMARKS",
                    title: "Remarks"
                },
                {
                    field: "FORM_CODE",
                    title: "FORM CODE"

                }

            ];
        }
        else if (tableName == "IP_PURCHASE_INVOICE") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "MANUAL_NO",
                    title: "Manual No."
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },
                {
                    field: "SUPPLIER_EDESC",
                    title: "Customer"
                }
                ,
                {
                    field: "ITEM_EDESC",
                    title: "Item"
                }
                ,

                {
                    field: "MU_CODE",
                    title: "Unit"
                }
                ,
                {
                    field: "QUANTITY",
                    title: "Quantity"
                }
                ,
                {
                    field: "UNIT_PRICE",
                    title: "Unit Price"
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Total Price"
                }

                ,
                //{
                //    field: "TO_LOCATION_EDESC",
                //    title: "To Location"
                //}
                //,
                //{
                //    field: "DIVISION_CODE",
                //    title: "Division"
                //}
                //,

                {
                    field: "REMARKS",
                    title: "Remarks"
                }

            ];
        }
        else if (tableName == "IP_PURCHASE_ORDER") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                //{
                //    template: function (dataItem) {
                //        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                //    },
                //    width: 50
                //},
                {
                    field: "MANUAL_NO",
                    title: "Manual No."
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },
                {
                    field: "SUPPLIER_EDESC",
                    title: "Supplier"
                }
                ,
                {
                    field: "ITEM_EDESC",
                    title: "Item"
                }
                ,

                {
                    field: "MU_CODE",
                    title: "Unit"
                }
                ,
                {
                    field: "QUANTITY",
                    title: "Quantity"
                }
                ,
                {
                    field: "UNIT_PRICE",
                    title: "Price"
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Total Price"
                }

                //,
                //{
                //    field: "TO_LOCATION_CODE",
                //    title: "To Location"
                //}
                //,
                // {
                //     field: "DIVISION_CODE",
                //     title: "Division"
                // }
                ,

                {
                    field: "REMARKS",
                    title: "Remarks"
                }

            ];
        }
        else if (tableName == "IP_PRODUCTION_ISSUE") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },
                {
                    field: "ISSUE_TYPE_CODE",
                    title: "Issue Type"
                }
                ,
                {
                    field: "FROM_LOCATION_EDESC",
                    title: "From Location"
                }
                ,
                {
                    field: "TO_LOCATION_EDESC",
                    title: "To Location"
                }
                ,


                {
                    field: "ITEM_EDESC",
                    title: "Item Code"
                }
                ,

                {
                    field: "MU_CODE",
                    title: "Unit"
                }
                ,
                {
                    field: "QUANTITY",
                    title: "Quantity"
                }
                ,
                {
                    field: "UNIT_PRICE",
                    title: "Unit Price"
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Total Price"
                }
                ,
                {
                    field: "DIVISION_CODE",
                    title: "Division"
                }
                ,

                {
                    field: "REMARKS",
                    title: "Remarks"
                }

            ];
        }
        else if (tableName == "IP_GOODS_ISSUE") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "MANUAL_NO",
                    title: "Manual No."
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },
                {
                    field: "ISSUE_TYPE_CODE",
                    title: "Issue Type"
                }
                ,
                //{
                //    field: "FROM_LOCATION_EDESC",
                //    title: "From Location"
                //}
                //,
                //{
                //    field: "TO_LOCATION_EDESC",
                //    title: "To Location"
                //}
                //,
                {
                    field: "ITEM_EDESC",
                    title: "Item"
                }
                ,

                {
                    field: "MU_CODE",
                    title: "Unit"
                }
                ,
                {
                    field: "QUANTITY",
                    title: "Quantity"
                }
                ,
                {
                    field: "UNIT_PRICE",
                    title: "Unit Price"
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Total Price"
                }
                ,

                {
                    field: "REMARKS",
                    title: "Remarks"
                }
                //,
                //{
                //    field: "PRODUCT_CODE",
                //    title: "Product"
                //}
                //,
                //{
                //    field: "CUSTOMER_EDESC",
                //    title: "Customer"
                //}
                //,
                //{
                //    field: "EMPLOYEE_EDESC",
                //    title: "Employee"
                //}
                ,
                {
                    field: "SUPPLIER_EDESC",
                    title: "Supplier"
                }
                //,
                //{
                //    field: "DIVISION_CODE",
                //    title: "Division"
                //}
            ];
        }
        else if (tableName == "IP_PURCHASE_INVOICE") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= INVOICE_DATE == null ? '' :kendo.toString(kendo.parseDate(INVOICE_DATE),'M/dd/yyyy') #",
                },
                {
                    field: "SUPPLIER_EDESC",
                    title: "Supplier"
                },
                {
                    field: "ITEM_EDESC",
                    title: "Item Code"
                },
                {
                    field: "MU_CODE",
                    title: "Unit"
                },
                {
                    field: "QUANTITY",
                    title: "Quantity"
                }
                ,
                {
                    field: "UNIT_PRICE",
                    title: "Unit Price"
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Total Price"
                }

                ,
                {
                    field: "TO_LOCATION_EDESC",
                    title: "To Location"
                }
                ,
                {
                    field: "DIVISION_CODE",
                    title: "Division"
                }
                ,

                {
                    field: "REMARKS",
                    title: "Remarks"
                }

            ];
        }
        else if (tableName == "IP_PURCHASE_MRR") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "MANUAL_NO",
                    title: "Manual No."
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    //template: "#= INVOICE_DATE == null ? '' :kendo.toString(kendo.parseDate(INVOICE_DATE),'M/dd/yyyy') #",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },
                {
                    field: "SUPPLIER_EDESC",
                    title: "Supplier"
                },
                {
                    field: "ITEM_EDESC",
                    title: "Item Code"
                },
                {
                    field: "MU_CODE",
                    title: "Unit"
                },
                {
                    field: "QUANTITY",
                    title: "Quantity"
                }
                ,
                {
                    field: "UNIT_PRICE",
                    title: "Price"
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Total Price"
                }

                //,
                //{
                //    field: "TO_LOCATION_EDESC",
                //    title: "To Location"
                //}
                //,

                //{
                //    field: "DIVISION_EDESC",
                //    title: "Division"
                //}
                ,

                {
                    field: "REMARKS",
                    title: "Remarks"
                }

            ];
        }
        else if (tableName == "IP_PURCHASE_RETURN") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= RETURN_DATE == null ? '' :kendo.toString(kendo.parseDate(RETURN_DATE),'M/dd/yyyy') #",
                },
                {
                    field: "FROM_LOCATION_CODE",
                    title: "From Location"
                }
                ,



                {
                    field: "ITEM_EDESC",
                    title: "Item Code"
                }
                ,

                {
                    field: "MU_CODE",
                    title: "Unit"
                }
                ,
                {
                    field: "QUANTITY",
                    title: "Quantity"
                }
                ,
                {
                    field: "UNIT_PRICE",
                    title: "Unit Price"
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Total Price"
                }
                ,
                {
                    field: "BUDGET_CODE",
                    title: "Budget"
                }
                ,

                {
                    field: "REMARKS",
                    title: "Remarks"
                }

            ];
        }
        else if (tableName == "FA_DOUBLE_VOUCHER") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },
                {
                    field: "ACC_CODE",
                    title: "Account"
                }
                ,
                {
                    field: "PARTICULARS",
                    title: "Particular"
                }
                ,
                {
                    field: "TRANSACTION_TYPE",
                    title: "Transaction Type"
                }

                ,

                {
                    field: "AMOUNT",
                    title: "Amount"
                }
                ,
                {
                    field: "REMARKS",
                    title: "Remark"
                }
                ,
                {
                    field: "SUPPLIER_CODE",
                    title: "Supplier"
                }
                ,
                {
                    field: "DIVISION_CODE",
                    title: "Division"
                }

                ,


                {
                    field: "EMPLOYEE_CODE",
                    title: "Employee"
                }

            ];
        }
        else if (tableName == "FA_SINGLE_VOUCHER") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },
                {
                    field: "ACC_CODE",
                    title: "Account"
                }
                ,
                {
                    field: "PARTICULARS",
                    title: "Particular"
                }
                ,
                {
                    field: "TRANSACTION_TYPE",
                    title: "Transaction Type"
                }

                ,

                {
                    field: "AMOUNT",
                    title: "Amount"
                }
                ,
                {
                    field: "REMARKS",
                    title: "Remark"
                }
                ,
                {
                    field: "SUPPLIER_CODE",
                    title: "Supplier"
                }
                ,
                {
                    field: "DIVISION_CODE",
                    title: "Division"
                }

                ,


                {
                    field: "EMPLOYEE_CODE",
                    title: "Employee"
                }

            ];
        }
        else if (tableName == "IP_GOODS_REQUISITION") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "MANUAL_NO",
                    title: "Manual No."
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },
                {
                    field: "ITEM_EDESC",
                    title: "Item Name"
                }
                ,
                {
                    field: "QUANTITY",
                    title: "Quanitity"
                }
                ,
                {
                    field: "MU_CODE",
                    title: "Unit"
                }
                ,
                {
                    field: "UNIT_PRICE",
                    title: "Price"
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Total Amount"
                }
                ,
                {
                    field: "SUPPLIER_EDESC",
                    title: "Supplier"
                }

            ];
        }
        else if (tableName == "IP_PURCHASE_REQUEST") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "MANUAL_NO",
                    title: "Manual No"
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },

                {
                    field: "NepaliVOUCHER_DATE",
                    title: "Miti",

                },

                {
                    field: "ITEM_EDESC",
                    title: "Item Name"
                }
                ,
                {
                    field: "QUANTITY",
                    title: "Quanitity"
                }
                ,
                {
                    field: "MU_CODE",
                    title: "Unit"
                }

                ,

                {
                    field: "UNIT_PRICE",
                    title: "Price"
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Total Amount"
                }
                ,
                {
                    field: "SUPPLIER_EDESC",
                    title: "Supplier"
                }

            ];
        }
        else if (tableName == "IP_GATE_PASS_ENTRY") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "MANUAL_NO",
                    title: "Manual No."
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },

                //{
                //    field: "NepaliVOUCHER_DATE",
                //    title: "Miti",

                //},

                {
                    field: "ITEM_EDESC",
                    title: "Item Name"
                }
                ,
                {
                    field: "QUANTITY",
                    title: "Quanitity"
                }
                ,
                {
                    field: "MU_CODE",
                    title: "Unit"
                }

                ,

                {
                    field: "UNIT_PRICE",
                    title: "Price"
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Total Amount"
                }
                // column supplier_code not in table
                //,
                //{
                //    field: "SUPPLIER_EDESC",
                //    title: "Supplier"
                //}

            ];
        }
        else if (tableName == "IP_RETURNABLE_GOODS_ISSUE") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                //{
                //    field: "MANUAL_NO",
                //    title: "Manual No."
                //},
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },

                //{
                //    field: "NepaliVOUCHER_DATE",
                //    title: "Miti",

                //},

                {
                    field: "ITEM_EDESC",
                    title: "Item Name"
                }
                ,
                {
                    field: "QUANTITY",
                    title: "Quanitity"
                }
                ,
                {
                    field: "MU_CODE",
                    title: "Unit"
                }
                ,
                {
                    field: "UNIT_PRICE",
                    title: "Price"
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Total Amount"
                }
                ,
                {
                    field: "SUPPLIER_EDESC",
                    title: "Supplier"
                }

            ];
        }
        else if (tableName == "IP_TRANSFER_ISSUE") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "MANUAL_NO",
                    title: "Manual No."
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },

                //{
                //    field: "NepaliVOUCHER_DATE",
                //    title: "Miti",

                //},

                {
                    field: "ITEM_EDESC",
                    title: "Item Name"
                }
                ,
                {
                    field: "QUANTITY",
                    title: "Quanitity"
                }
                ,
                {
                    field: "MU_CODE",
                    title: "Unit"
                }
                ,
                {
                    field: "UNIT_PRICE",
                    title: "Price"
                }
                ,
                {
                    field: "TOTAL_PRICE",
                    title: "Total Amount"
                }
                //,
                //{
                //    field: "SUPPLIER_EDESC",
                //    title: "Supplier"
                //}

            ];
        }
        else if (tableName == "FA_JOB_ORDER") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "MANUAL_NO",
                    title: "Manual No."
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },

                //{
                //    field: "NepaliVOUCHER_DATE",
                //    title: "Miti",

                //},

                {
                    field: "SUPPLIER_CODE ",
                    title: "Supplier"
                }
                ,
                {
                    field: "ACC_CODE",
                    title: "Account"
                }
                ,
                {
                    field: "AMOUNT",
                    title: "Ammount"
                }
                ,
                {
                    field: "TRANSACTION_TYPE",
                    title: "Transaction Type"
                }
                ,
                {
                    field: "PAYMENT_MODE",
                    title: "Payment Mode"
                }
                //,
                //{
                //    field: "SUPPLIER_EDESC",
                //    title: "Supplier"
                //}

            ];
        }
        else if (tableName == "FA_ADVICE_VOUCHER") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "MANUAL_NO",
                    title: "Manual No."
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },

                //{
                //    field: "NepaliVOUCHER_DATE",
                //    title: "Miti",

                //},

                //{
                //    field: "SUPPLIER_CODE ",
                //    title: "Supplier"
                //}
                //,
                {
                    field: "ACC_CODE",
                    title: "Account"
                }
                ,
                {
                    field: "AMOUNT",
                    title: "Ammount"
                }
                ,
                {
                    field: "TRANSACTION_TYPE",
                    title: "Transaction Type"
                }
                ,
                {
                    field: "PAYMENT_MODE",
                    title: "Payment Mode"
                }
                //,
                //{
                //    field: "SUPPLIER_EDESC",
                //    title: "Supplier"
                //}

            ];
        }
        else if (tableName == "FA_PAY_ORDER") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
                    },
                    width: 50
                },
                {
                    field: "MANUAL_NO",
                    title: "Manual No."
                },
                {
                    field: "VOUCHER_NO",
                    title: "Voucher No"
                },
                {
                    field: "VOUCHER_DATE",
                    title: "Voucher Date",
                    template: "#= VOUCHER_DATE == null ? '' :kendo.toString(kendo.parseDate(VOUCHER_DATE),'M/dd/yyyy') #",
                },

                //{
                //    field: "NepaliVOUCHER_DATE",
                //    title: "Miti",

                //},

                //{
                //    field: "SUPPLIER_CODE ",
                //    title: "Supplier"
                //}
                //,
                {
                    field: "ACC_CODE",
                    title: "Account"
                }
                ,
                {
                    field: "AMOUNT",
                    title: "Ammount"
                }
                ,
                {
                    field: "TRANSACTION_TYPE",
                    title: "Transaction Type"
                }
                ,
                {
                    field: "PAYMENT_MODE",
                    title: "Payment Mode"
                },
                {
                    field: "CHEQUE_NO",
                    title: "Cheque No"
                }
                //,
                //{
                //    field: "SUPPLIER_EDESC",
                //    title: "Supplier"
                //}

            ];
        }
        return colName;
    }

    function generateGroupFieldName(tableName) {

        var groupField
        var switch_on = tableName;
        switch (switch_on) {
            case "IP_PURCHASE_MRR":
            case "IP_PURCHASE_ORDER":
                groupField = "SUPPLIER_EDESC";
                break;
            case "SA_SALES_ORDER":
            case "SA_SALES_INVOICE":
            case "SA_SALES_CHALAN":
                groupField = "CUSTOMER_EDESC";
                break;
            case "IP_GOODS_REQUISITION":
                groupField = "VOUCHER_NO";
                break;
            default:
                groupField = "VOUCHER_NO";
        }
        return groupField;
    };


    $scope.C_DataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllCustomerSetupByFilter"
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
        },
    }
    $scope.C_Option = {
        dataSource: $scope.C_DataSource,
        dataTextField: 'CustomerName',
        dataValueField: 'CustomerCode',
        filter: "contains",
        select: function (e) {


        },
        dataBound: function (e) {

        },
        change: function (e) {


        }
    }


});

