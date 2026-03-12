

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
                url: "/api/TemplateApi/GetAllCustomerSetupByFilter",
            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
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
        close: function (e) {

            var dataItem = $("#referenceCustomer").data('kendoComboBox').dataItem();
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
            if (checked) {
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
        else if (tableName == "IP_QUOTATION_INQUIRY") {
            colName = [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='${dataitem.voucher_no}' class='checkbox row-checkbox'></label>"
                        //    return "<input type='checkbox' id='${dataItem.VOUCHER_NO}' class='checkbox row-checkbox'><label class='k-checkbox-label' for='${dataItem.VOUCHER_NO}'></label>"
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

            ];
        }
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


DTModule.controller('refrenceCodeCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter, formtemplateservice) {

    var a = document.docname;


    $scope.refrenceCodeDataSource = {
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
                            FormCode: $scope.FormCode,
                            filter: data.filter.filters[0].value,
                            Table_name: $rootScope.RefTableName,
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            FormCode: $scope.FormCode,
                            filter: "",
                            Table_name: $scope.RefTableName,
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {

                        FormCode: $scope.FormCode,
                        filter: "",
                        Table_name: $scope.RefTableName,
                    };
                    return newParams;
                }
            }
        },
    };
    $scope.refrenceCodeOption = {
        dataSource: $scope.refrenceCodeDataSource,
        //template: '<span>{{dataItem.ORDER_EDESC}}</span>  --- ' +
        //'<span>{{dataItem.Type}}</span>',
        dataTextField: 'ORDER_EDESC',
        dataValueField: 'ORDER_CODE',
        filter: 'contains',
        select: function (e) {

            if (e.dataItem == "undefined" || e.dataItem == "" || e.dataItem == null) {
                return;
            }
            else {

                var orderNo = e.dataItem.ORDER_CODE;
                var defered = $.Deferred();
                var saleOrderformDetail = formtemplateservice.getSalesOrderDetail_ByFormCodeAndOrderNo($scope.FormCode, orderNo, defered);
                $.when(defered).done(function (result) {
                    var response = [];
                    response = result;
                    $scope.refrenceFn(response);

                });
            }
        },


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

});


// hrere
$scope.selectedTemplate = null;

$scope.quotationOptionData = [];
$scope.TEMPLATE = null;
$scope.DocumentReference = {};
$('#FromDateVoucher').on('changeDate', function (e) {
    var scope = angular.element($('#FromDateVoucher')).scope();
    scope.$apply(function () {
        scope.DocumentReference.FROM_DATE = $('#FromDateVoucher').val();
    });
});

$('#ToDateVoucher').on('changeDate', function (e) {
    var scope = angular.element($('#ToDateVoucher')).scope();
    scope.$apply(function () {
        scope.DocumentReference.TO_DATE = $('#ToDateVoucher').val();
    });
});

$http.get('/api/QuotationApi/TemplateOptions')
    .then(function (response) {
        let allData = [];

        if (Array.isArray(response.data)) {
            allData = response.data;
        } else if (response.data && typeof response.data === 'object') {
            allData = [response.data];
        }

        $scope.quotationOptionData = allData;

        // ========== DOCUMENT DROPDOWN ==========
        const quotationDoc = allData.find(item =>
            item.FORM_EDESC && item.FORM_EDESC.toLowerCase().includes('quotation')
        );

        const documentOptions = quotationDoc ? [{
            FORM_CODE: quotationDoc.ID,
            FORM_EDESC: 'Quotation'
        }] : [];

        const $documentDropdown = $("#refrenceTypeMultiSelect");

        if ($documentDropdown.length) {
            if ($documentDropdown.data("kendoDropDownList")) {
                $documentDropdown.data("kendoDropDownList").destroy();
                $documentDropdown.empty();
            }

            $documentDropdown.kendoDropDownList({
                dataSource: documentOptions,
                optionLabel: '-- Select Document --',
                dataTextField: "FORM_EDESC",
                dataValueField: "FORM_CODE",
                autoBind: true
            });
        } else {
            console.warn("#refrenceTypeMultiSelect not found in DOM.");
        }

        // ========== TEMPLATE DROPDOWN ==========
        const parsedTemplates = allData.map(item => ({
            FORM_CODE: `${item.PREFIX || ''};${item.SUFFIX || ''}`,
            FORM_EDESC: item.FORM_EDESC || '(No Name)'
        }));

        // Add "ALL DOCUMENTS" at the top
        parsedTemplates.unshift({
            FORM_CODE: 'all',
            FORM_EDESC: 'ALL DOCUMENTS'
        });

        console.log("Parsed Templates for Dropdown:", parsedTemplates);

        const $templateDropdown = $("#TemplateTypeMultiSelect");

        if ($templateDropdown.length) {
            if ($templateDropdown.data("kendoDropDownList")) {
                $templateDropdown.data("kendoDropDownList").destroy();
                $templateDropdown.empty();
            }

            $templateDropdown.kendoDropDownList({
                dataSource: parsedTemplates,
                optionLabel: '-- Select Template --',
                dataTextField: "FORM_EDESC",
                dataValueField: "FORM_CODE",
                autoBind: true
            });
        } else {
            console.warn("#TemplateTypeMultiSelect not found in DOM.");
        }
    })
    .catch(function (error) {
        console.error('Error fetching template options:', error);
    });


// Item Description
$http.get("/api/QuotationApi/ItemDetails")
    .then(function (response) {
        $scope.itemDescriptionList = response.data;
        $scope.initializeReferenceGrid();
        setTimeout(function () {
            $("#itemDescInput").kendoAutoComplete({
                autoBind: true,
                virtual: {
                    itemHeight: 26,
                    valueMapper: function (options) {
                        options.success();
                    }
                },
                dataSource: $scope.itemDescriptionList,
                dataTextField: "ItemDescription",
                dataValueField: "ItemCode",
                filter: "contains",
                placeholder: "Select Product Code",
                minLength: 1
            });
        }, 100);
    })
    .catch(function (error) {
        displayPopupNotification("Error fetching item details", "error");
    });

// Voucher No
$http.get("/api/QuotationApi/VoucherList")
    .then(function (response) {
        $scope.voucherList = response.data;

        setTimeout(function () {
            $("#voucherInput").kendoAutoComplete({
                autoBind: true,
                virtual: {
                    itemHeight: 26,
                    valueMapper: function (options) {
                        options.success();
                    }
                },
                dataSource: $scope.voucherList,
                dataValueField: "TENDER_NO",
                dataTextField: "TENDER_NO",
                filter: "contains",
                placeholder: "Select Voucher No.",
                minLength: 1
            });
        }, 100);
    })
    .catch(function (error) {
        displayPopupNotification("Error fetching voucher list", "error");
    });

$scope.templateData = [];
// Kendo Grid Options
$scope.quotationGridOptions = {
    autoBind: false,
    dataSource: {
        transport: {
            read: {
                url: '/api/QuotationApi/getTemplateData',
                dataType: 'json',
                data: function () {
                    return $scope.buildSearchParams();
                }
            }
        }
    },
    sortable: true,
    columns: [
        { field: 'TENDOR_NO', title: 'Quotation No.' },
        { field: 'DATE', title: 'Date', template: "#= kendo.toString(kendo.parseDate(date), 'dd/MM/yyyy') #" },
        { field: 'ITEM', title: 'Item Description' },
        { field: 'QUANTITY', title: 'Quantity' },
        { field: 'SPECIFICATION', title: 'Specification' },
        {
            command: [
                {
                    name: 'select',
                    text: 'Select',
                    click: function (e) {
                        e.preventDefault();
                        var dataItem = this.dataItem($(e.currentTarget).closest('tr'));
                        $scope.takeReference(dataItem);
                    }
                }
            ],
            title: 'Actions',
            width: 120
        }
    ]
};

$scope.buildSearchParams = function () {
    return {
        prefix: $scope.selectedTemplate ? $scope.selectedTemplate.ID || $scope.selectedTemplate.id : null,
        Reference: $scope.referenceType,
        Row: $scope.rowType,
        Voucher_no: $scope.voucherNo,
        Item: $scope.itemDesc,
        Name: $scope.consigneeName,
        fromDate: $scope.dateFilter,
        toDate: $scope.adDateStart ? $filter('date')($scope.adDateStart, 'yyyy-MM-dd') : null
    };
};

$scope.searchQuotations = function () {
    console.log('Search clicked');
    var params = $scope.buildSearchParams();
    console.log('Params built:', params);

    var grid = $('#kendoGrid').data('kendoGrid');
    if (grid) {
        grid.dataSource.read();
    } else {
        console.error('Kendo Grid not found.');
        displayPopupNotification('Grid not found. Please try again.', 'error');
    }
};

/*$scope.takeReference = function (dataItem) {
    console.log('Selected Quotation:', dataItem);
    $scope.quotationData = dataItem;
    $('#RefrenceModel').modal('hide');
    console.log("DataItem received in takeReference:", dataItem);

    var grid = $("#referenceGrid").data("kendoGrid");
    var dataSource = grid.dataSource;

    // Remove initial empty row if it exists (assuming empty means no ITEM_CODE or similar check)
    var firstRow = dataSource.at(0);
    if (firstRow && (!firstRow.ITEM_CODE || firstRow.ITEM_CODE === '')) {
        dataSource.remove(firstRow);
    }

    var imageUrl = dataItem.IMAGE_NAME ? (window.location.protocol + "//" + window.location.host + "/Areas/NeoERP.QuotationManagement/Image/Items/" + dataItem.IMAGE_NAME) : '';

    $http.get('/api/QuotationApi/ItemDetails')
        .then(function (response) {
            var itemDetails = response.data;
            var selectedItem = itemDetails.find(item => item.Item_Code === dataItem.ITEM_CODE);

            // If there's at least one existing item in productFormList, update it, else add new
            if ($scope.productFormList && $scope.productFormList.length > 0) {
                // Update the first row data (using Angular's way or just overwrite object)
                var existing = $scope.productFormList[0];
                existing.ItemDescription = dataItem.ITEM_CODE || '';
                existing.SPECIFICATION = dataItem.SPECIFICATION || '';
                existing.IMAGE = dataItem.IMAGE_NAME || '';
                existing.IMAGE_LINK = imageUrl;
                existing.UNIT = dataItem.MU_CODE || '';
                existing.QUANTITY = dataItem.QUANTITY || 0;
                existing.CATEGORY = dataItem.CATEGORY || '';
                existing.BRAND_NAME = dataItem.BRAND_NAME || '';
                existing.INTERFACE = dataItem.INTERFACE || '';
                existing.TYPE = dataItem.TYPE || '';
                existing.LAMINATION = dataItem.LAMINATION || '';
                existing.ITEM_SIZE = dataItem.ITEM_SIZE || '';
                existing.THICKNESS = dataItem.THICKNESS || '';
                existing.COLOR = dataItem.COLOR || '';
                existing.GRADE = dataItem.GRADE || '';
                existing.SIZE_LENGTH = dataItem.SIZE_LENGTH || 0;
                existing.SIZE_WIDTH = dataItem.SIZE_WIDTH || 0;
            } else {
                // Add as new row if empty
                $scope.productFormList.push({
                    ItemDescription: dataItem.ITEM_CODE || '',
                    SPECIFICATION: dataItem.SPECIFICATION || '',
                    IMAGE: dataItem.IMAGE_NAME || '',
                    IMAGE_LINK: imageUrl,
                    UNIT: dataItem.MU_CODE || '',
                    QUANTITY: dataItem.QUANTITY || 0,
                    CATEGORY: dataItem.CATEGORY || '',
                    BRAND_NAME: dataItem.BRAND_NAME || '',
                    INTERFACE: dataItem.INTERFACE || '',
                    TYPE: dataItem.TYPE || '',
                    LAMINATION: dataItem.LAMINATION || '',
                    ITEM_SIZE: dataItem.ITEM_SIZE || '',
                    THICKNESS: dataItem.THICKNESS || '',
                    COLOR: dataItem.COLOR || '',
                    GRADE: dataItem.GRADE || '',
                    SIZE_LENGTH: dataItem.SIZE_LENGTH || 0,
                    SIZE_WIDTH: dataItem.SIZE_WIDTH || 0
                });
            }

            // No need to call addRow if you’re updating an existing row

            // If your grid needs to be refreshed manually, do it here:
            // $scope.$apply(); // or grid.refresh();

        })
        .catch(function (error) {
            console.error('Error fetching item details:', error);
            displayPopupNotification('Error fetching item details', 'error');
        });
};*/

$scope.takeReference = function () {
    // Clear existing productFormList
    console.log("hello");
    $scope.productFormList = [];
    angular.forEach($scope.selectedReferenceItems, function (refItem) {
        $scope.addRow();
        var newProduct = $scope.productFormList[$scope.productFormList.length - 1];

        // Safely assign ItemCode even if ItemDescription is null
        newProduct.ItemDescription = refItem.ItemDescription || refItem.ItemCode || '';
        newProduct.ItemCode = refItem.ItemCode || '';  // keep for reference
        newProduct.QUANTITY = refItem.QUANTITY || 0;

        // Defer updateUnit slightly to ensure Kendo data is loaded
        console.log("before update function", newProduct);
        $timeout(function () {
            $scope.updateUnit(newProduct);
        }, 50);
    });
};


$timeout(function () {
    if ($('#bsDateStart').length && $('#bsDateEnd').length) {
        $('#bsDateStart').nepaliDatePicker({
            onChange: function () {
                $scope.bsDateStart = $('#bsDateStart').val();
                $scope.$apply();
            }
        });
        $('#bsDateEnd').nepaliDatePicker({
            onChange: function () {
                $scope.bsDateEnd = $('#bsDateEnd').val();
                $scope.$apply();
            }
        });
    }
}, 0);

$scope.saveQuotation = function () {
    var quotationData = {
        quotation: $scope.quotationData,
        // Add other fields as needed
    };
    $http.post('/api/save-quotation', quotationData).then(function (response) {
        console.log('Quotation Saved:', response.data);
        $location.path('/quotations'); // Redirect after save
    }, function (error) {
        console.error('Error saving quotation:', error);
    });
};

//New Code From Here :>
// Initialize scope variables
$scope.DocumentReference = {
    FORM_CODE: null,
    ITEM_DESC: null,
    TENDER_NO: null,
    FROM_DATE: null,
    TO_DATE: null,
    FROM_DATE_AD: null, // Added for AD date
    TO_DATE_AD: null   // Added for AD date
};
$scope.ROW = 'nonreference';
$scope.dateFilter = 'previous'; // Default to previous
$scope.ref_form_code = ''; // Set dynamically
$scope.RefTableName = 'SA_SALES_ORDER';
$scope.ModuleCode = '04';
$scope.freeze_master_ref_flag = 'N';

var checkedItems = [];

// Update dateFilter to custom when BS or AD dates are changed
$scope.updateDateFilter = function () {
    if ($scope.DocumentReference.FROM_DATE || $scope.DocumentReference.TO_DATE ||
        $scope.DocumentReference.FROM_DATE_AD || $scope.DocumentReference.TO_DATE_AD) {
        $scope.dateFilter = 'custom';
        $("#ddlDateFilterVoucher").data("kendoDropDownList").value('custom');
    }
};
// Voucher ComboBox
$scope.refrenceCodeDataSource = new kendo.data.DataSource({
    transport: {
        read: {
            url: "/api/QuotationApi/VoucherList",
            dataType: "json"
        },
        parameterMap: function (data, type) {
            if (data.filter && data.filter.filters.length > 0) {
                return {
                    filter: data.filter.filters[0].value
                };
            }
            return {};
        }
    }
});

// Item ComboBox
$scope.ItemCodeOption = {
    dataTextField: 'ItemDescription',
    dataValueField: 'ItemCode',
    placeholder: 'Select Product Code',
    filter: 'contains',
    suggest: false,
    minLength: 3,
    dataSource: {
        transport: {
            read: {
                url: '/api/QuotationApi/ItemDetails',
                dataType: 'json'
            }
        }
    }
};

// Reference Grid Options
$scope.initializeReferenceGrid = function () {
    $scope.referenceGridOptions = {
        dataSource: {
            transport: {
                read: {
                    type: "POST",
                    url: "/api/QuotationApi/getTemplateData",
                    contentType: "application/json; charset=utf-8",
                    dataType: 'json'
                },
                parameterMap: function (options, type) {
                    const prefixSuffix = $("#TemplateTypeMultiSelect").val() || 'all';
                    const reference = $("#refrenceTypeMultiSelect").val() || '';
                    const [prefix, suffix] = prefixSuffix.split(";");

                    const payload = {
                        prefix: prefixSuffix || null,
                        Reference: reference,
                        Row: $scope.ROW || null,
                        Voucher_no: $scope.voucher_no || null,
                        Item: $scope.DocumentReference.ITEM_DESC || null,
                        Name: $scope.DocumentReference.TEMPLATE || null,
                        fromDate: $scope.DocumentReference.FROM_DATE || null,
                        toDate: $scope.DocumentReference.TO_DATE || null
                    };

                    return JSON.stringify(payload);
                }

            },
            pageSize: 50,
            serverPaging: false,
            serverSorting: false,
            schema: {
                model: {
                    fields: {
                        TENDER_NO: { type: "string", defaultValue: "" },
                        ITEM_EDESC: { type: "string", defaultValue: "" },
                        MU_CODE: { type: "string", defaultValue: "" },
                        QUANTITY: { type: "number", defaultValue: 0 },
                        UNIT_PRICE: { type: "number", defaultValue: 0 },
                        TOTAL_PRICE: { type: "number", defaultValue: 0 },
                        REMARKS: { type: "string", defaultValue: "" },
                        SERIAL_NO: { type: "string", defaultValue: "" },
                        ITEM_CODE: { type: "string", defaultValue: "" }
                    }
                },
                parse: function (data) {
                    let flattened = [];

                    // Build a quick lookup for ITEM_CODE to ITEM_EDESC
                    const itemLookup = {};
                    ($scope.itemDescriptionList || []).forEach(item => {
                        itemLookup[item.ItemCode] = item.ItemDescription;
                    });

                    data.forEach(parent => {
                        if (parent.Items && parent.Items.length) {
                            parent.Items.forEach(item => {
                                const itemCode = item.ITEM_CODE || '';
                                const itemEDESC = itemLookup[itemCode] || item.SPECIFICATION || '';

                                flattened.push({
                                    TENDER_NO: parent.TENDER_NO || '',
                                    ITEM_EDESC: itemEDESC,
                                    MU_CODE: item.UNIT || '',
                                    QUANTITY: item.QUANTITY || 0,
                                    UNIT_PRICE: item.PRICE || 0,
                                    TOTAL_PRICE: (item.QUANTITY || 0) * (item.PRICE || 0),
                                    REMARKS: parent.REMARKS || '',
                                    SERIAL_NO: item.ID || '',
                                    ITEM_CODE: itemCode,
                                    SPECIFICATION: item.SPECIFICATION || '',
                                    IMAGE_NAME: item.IMAGE_NAME || '',
                                    CATEGORY: item.CATEGORY || '',
                                    BRAND_NAME: item.BRAND_NAME || '',
                                    INTERFACE: item.INTERFACE || '',
                                    TYPE: item.TYPE || '',
                                    LAMINATION: item.LAMINATION || '',
                                    ITEM_SIZE: item.ITEM_SIZE || '',
                                    THICKNESS: item.THICKNESS || '',
                                    COLOR: item.COLOR || '',
                                    GRADE: item.GRADE || '',
                                    SIZE_LENGTH: item.SIZE_LENGTH || 0,
                                    SIZE_WIDTH: item.SIZE_WIDTH || 0,
                                    PRICE: item.PRICE || 0
                                });
                            });
                        }
                    });

                    const seen = new Set();
                    const unique = [];
                    flattened.forEach(row => {
                        const key = row.TENDER_NO + "_" + row.SERIAL_NO;
                        if (!seen.has(key)) {
                            seen.add(key);
                            unique.push(row);
                        }
                    });

                    return unique;
                }
            }
        },
        sortable: true,
        pageable: true,
        reorderable: true,
        groupable: true,
        resizable: true,
        filterable: {
            extra: false,
            operators: {
                number: { eq: "Is equal to", neq: "Is not equal to", gte: "Is greater than or equal to", gt: "Is greater than", lte: "Is less than or equal", lt: "Is less than" },
                string: { eq: "Is equal to", neq: "Is not equal to", startswith: "Starts with", contains: "Contains", doesnotcontain: "Does not contain", endswith: "Ends with" },
                date: { eq: "Is equal to", neq: "Is not equal to", gte: "Is after or equal to", gt: "Is after", lte: "Is before or equal to", lt: "Is before" }
            }
        },
        columnMenu: true,
        persistSelection: true,
        scrollable: { virtual: true },
        dataBound: function (e) {
            var grid = e.sender;
            grid.tbody.find(".select-checkbox").off("change").on("change", function () {
                var $row = $(this).closest("tr");
                var dataItem = grid.dataItem($row);

                if (this.checked) {
                    // Add to selectedReferences if not already added
                    if (!$scope.selectedReferences.find(r => r.TENDER_NO === dataItem.TENDER_NO && r.SERIAL_NO === dataItem.SERIAL_NO)) {
                        $scope.selectedReferences.push(dataItem);
                    }
                } else {
                    // Remove from selectedReferences
                    $scope.selectedReferences = $scope.selectedReferences.filter(r => !(r.TENDER_NO === dataItem.TENDER_NO && r.SERIAL_NO === dataItem.SERIAL_NO));
                }
                $scope.$apply();  // Trigger Angular digest for UI update if needed
            });

            // Preserve checkbox states on paging/filtering etc
            var view = this.dataSource.view();
            for (var i = 0; i < view.length; i++) {
                var item = view[i];
                var isSelected = $scope.selectedReferences.some(r => r.TENDER_NO === item.TENDER_NO && r.SERIAL_NO === item.SERIAL_NO);
                var row = this.tbody.find("tr[data-uid='" + item.uid + "']");
                row.find(".select-checkbox").prop("checked", isSelected);
                if (isSelected) {
                    row.addClass("k-state-selected");
                } else {
                    row.removeClass("k-state-selected");
                }
            }
        }
        ,
        columns: [
            {
                title: "Select",
                width: 80,
                template: "<input type='checkbox' class='select-checkbox' />"
            },
            { field: "TENDER_NO", title: "Voucher No" },
            { field: "ITEM_EDESC", title: "Item" },
            { field: "MU_CODE", title: "Unit" },
            { field: "QUANTITY", title: "Quantity", attributes: { style: "text-align:right;" } },
            { field: "REMARKS", title: "Remark" },
            { field: "SERIAL_NO", title: "Serial No" },
            { field: "ITEM_CODE", title: "Item Code" },
            {
                field: "IMAGE_NAME",
                hidden: true
            },
            {
                field: "CATEGORY",
                hidden: true
            },
            {
                field: "BRAND_NAME",
                hidden: true
            },
            {
                field: "INTERFACE",
                hidden: true
            },
            {
                field: "TYPE",
                hidden: true
            },
            {
                field: "LAMINATION",
                hidden: true
            },
            {
                field: "ITEM_SIZE",
                hidden: true
            },
            {
                field: "THICKNESS",
                hidden: true
            },
            {
                field: "COLOR",
                hidden: true
            },
            {
                field: "GRADE",
                hidden: true
            },
            {
                field: "SIZE_LENGTH",
                hidden: true
            },
            {
                field: "SIZE_WIDTH",
                hidden: true
            },
            {
                field: "PRICE",
                hidden: true
            }
        ]
    }
};
$scope.dateFilterOptions = [
    { value: 'today', label: 'Today' },
    { value: 'yesterday', label: 'Yesterday' },
    { value: 'this_week', label: 'This Week' },
    { value: 'this_month', label: 'This Month' },
    { value: 'this_year', label: 'This Year' }
];

function generateNepaliMonths() {
    const nepaliMonths = ['Baishakh', 'Jestha', 'Ashadh', 'Shrawan', 'Bhadra', 'Ashwin', 'Kartik', 'Mangsir', 'Poush', 'Magh', 'Falgun', 'Chaitra'];

    try {
        const todayAD = new Date();
        const adString = todayAD.getFullYear() + '-' +
            String(todayAD.getMonth() + 1).padStart(2, '0') + '-' +
            String(todayAD.getDate()).padStart(2, '0');

        const todayBS = AD2BS(adString); // returns "2081-04-01"
        const bsMonth = parseInt(todayBS.split('-')[1], 10); // 04 -> Shrawan (index 3)

        const nepaliMonthOptions = [];
        for (let i = 0; i < bsMonth; i++) {
            nepaliMonthOptions.push({
                value: 'nepali_month_' + i,
                label: nepaliMonths[i]
            });
        }

        // Run within digest cycle to update view
        $scope.$applyAsync(() => {
            $scope.dateFilterOptions = $scope.dateFilterOptions.concat(nepaliMonthOptions);
            $scope.dateFilterOptions.push({ value: 'custom', label: 'Custom' });
        });
    } catch (e) {
        console.error('Nepali date conversion failed:', e);
    }
}
$scope.bindRefrenceDataToTemplate = function () {
    if (!$scope.selectedReferences || $scope.selectedReferences.length === 0) {
        alert("Please select at least one item.");
        return;
    }
    console.log($scope.selectedReferences);
    $http.get('/api/QuotationApi/ItemDetails').then(function (response) {
        var itemDetails = response.data;

        $scope.selectedReferences.forEach(function (dataItem) {
            var imageUrl = dataItem.IMAGE_NAME ? (window.location.protocol + "//" + window.location.host + "/Areas/NeoERP.QuotationManagement/Image/Items/" + dataItem.IMAGE_NAME) : '';
            var maxId = Math.max(...$scope.productFormList.map(product => product.ID));
            $scope.counterProduct = maxId !== -Infinity ? maxId + 1 : 1;
            // Check if productFormList already has this item (by unique key like TENDER_NO + SERIAL_NO)
            var existingIndex = $scope.productFormList.findIndex(function (p) {
                return p.TENDER_NO === dataItem.TENDER_NO && p.SERIAL_NO === dataItem.SERIAL_NO;
            });

            var productData = {
                ID: $scope.counterProduct,
                TENDER_NO: dataItem.TENDER_NO || '',
                SERIAL_NO: dataItem.SERIAL_NO || '',
                ItemDescription: dataItem.ITEM_CODE || '',
                SPECIFICATION: dataItem.SPECIFICATION || '',
                IMAGE: dataItem.IMAGE_NAME || '',
                IMAGE_LINK: imageUrl,
                UNIT: dataItem.MU_CODE || '',
                QUANTITY: dataItem.QUANTITY || 0,
                CATEGORY: dataItem.CATEGORY || '',
                BRAND_NAME: dataItem.BRAND_NAME || '',
                INTERFACE: dataItem.INTERFACE || '',
                TYPE: dataItem.TYPE || '',
                LAMINATION: dataItem.LAMINATION || '',
                ITEM_SIZE: dataItem.ITEM_SIZE || '',
                THICKNESS: dataItem.THICKNESS || '',
                COLOR: dataItem.COLOR || '',
                GRADE: dataItem.GRADE || '',
                SIZE_LENGTH: dataItem.SIZE_LENGTH || 0,
                SIZE_WIDTH: dataItem.SIZE_WIDTH || 0,
                PRICE: dataItem.PRICE || 0
            };
            $scope.productFormList.push(productData);
            console.log("Inside", $scope.productFormList);
        });

        // Clear selected references after adding/updating
        $scope.selectedReferences = [];

        // Optionally refresh UI or close modal
        $('#RefrenceModel').modal('hide');
        // Apply scope if needed
        $scope.$applyAsync();

    }).catch(function (error) {
        console.error('Error fetching item details:', error);
        alert('Error fetching item details');
    });
};

generateNepaliMonths();
$scope.bindReferenceGrid = function (isValid) {
    checkedItems = [];

    if (!isValid) {
        console.log('Form is invalid');
        return;
    }

    let fromDate = null, toDate = null;
    let fromDateAD = null, toDateAD = null;

    const parseDate = (date) => !isNaN(new Date(date).getTime()) ? $filter('date')(date, 'yyyy-MM-dd') : null;

    // Parse manually entered dates
    fromDate = parseDate($scope.DocumentReference.FROM_DATE);
    toDate = parseDate($scope.DocumentReference.TO_DATE);
    fromDateAD = parseDate($scope.DocumentReference.FROM_DATE_AD);
    toDateAD = parseDate($scope.DocumentReference.TO_DATE_AD);

    const today = new Date();

    switch ($scope.dateFilter) {
        case 'previous':
            const previous = new Date();
            previous.setDate(today.getDate() - 30);
            fromDate = toDate = fromDateAD = toDateAD = $filter('date')(previous, 'yyyy-MM-dd');
            toDate = toDateAD = $filter('date')(today, 'yyyy-MM-dd');
            break;

        case 'today':
            fromDate = toDate = fromDateAD = toDateAD = $filter('date')(today, 'yyyy-MM-dd');
            break;

        case 'yesterday':
            const yesterday = new Date();
            yesterday.setDate(today.getDate() - 1);
            fromDate = toDate = fromDateAD = toDateAD = $filter('date')(yesterday, 'yyyy-MM-dd');
            break;

        case 'this_week':
            const firstDayOfWeek = new Date(today);
            firstDayOfWeek.setDate(today.getDate() - today.getDay());
            fromDate = fromDateAD = $filter('date')(firstDayOfWeek, 'yyyy-MM-dd');
            toDate = toDateAD = $filter('date')(today, 'yyyy-MM-dd');
            break;

        case 'last_week':
            const lastWeekStart = new Date(today);
            lastWeekStart.setDate(today.getDate() - 7 - today.getDay());
            const lastWeekEnd = new Date(lastWeekStart);
            lastWeekEnd.setDate(lastWeekStart.getDate() + 6);
            fromDate = fromDateAD = $filter('date')(lastWeekStart, 'yyyy-MM-dd');
            toDate = toDateAD = $filter('date')(lastWeekEnd, 'yyyy-MM-dd');
            break;

        case 'this_month':
            const firstDayOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
            fromDate = fromDateAD = $filter('date')(firstDayOfMonth, 'yyyy-MM-dd');
            toDate = toDateAD = $filter('date')(today, 'yyyy-MM-dd');
            break;

        case 'custom':
            if (!fromDate && fromDateAD) fromDate = fromDateAD;
            if (!toDate && toDateAD) toDate = toDateAD;
            if (!fromDateAD && fromDate) fromDateAD = fromDate;
            if (!toDateAD && toDate) toDateAD = toDate;
            break;

    }

    // Assign resolved dates to scope
    $scope.DocumentReference.FROM_DATE = fromDate;
    $scope.DocumentReference.TO_DATE = toDate;
    $scope.DocumentReference.FROM_DATE_AD = fromDateAD;
    $scope.DocumentReference.TO_DATE_AD = toDateAD;

    // Refresh grid
    const grid = $('#referenceGrid').data('kendoGrid');
    if (grid) {
        grid.dataSource.read();
    } else {
        console.error('Reference Grid not found.');
    }
};

// Search function
$scope.bindReferenceGrid = function (isValid) {
    checkedItems = [];
    if (!isValid) {
        console.log('Form is invalid');
        return;
    }

    let fromDate = null, toDate = null;
    let fromDateAD = null, toDateAD = null;

    const today = new Date();
    const formatDate = date => $filter('date')(date, 'yyyy-MM-dd');

    const parseDate = date => !isNaN(new Date(date).getTime()) ? formatDate(new Date(date)) : null;

    // Manual BS and AD parsing
    fromDate = parseDate($scope.DocumentReference.FROM_DATE);
    toDate = parseDate($scope.DocumentReference.TO_DATE);
    fromDateAD = parseDate($scope.DocumentReference.FROM_DATE_AD);
    toDateAD = parseDate($scope.DocumentReference.TO_DATE_AD);

    // Apply dateFilter logic
    switch ($scope.dateFilter) {
        case 'today':
            fromDate = toDate = fromDateAD = toDateAD = formatDate(today);
            break;

        case 'yesterday':
            const yesterday = new Date(today);
            yesterday.setDate(today.getDate() - 1);
            fromDate = toDate = fromDateAD = toDateAD = formatDate(yesterday);
            break;

        case 'this_week':
            const firstDayOfWeek = new Date(today);
            firstDayOfWeek.setDate(today.getDate() - today.getDay());
            fromDate = fromDateAD = formatDate(firstDayOfWeek);
            toDate = toDateAD = formatDate(today);
            break;

        case 'last_week':
            const lastWeekStart = new Date(today);
            lastWeekStart.setDate(today.getDate() - 7 - today.getDay());
            const lastWeekEnd = new Date(lastWeekStart);
            lastWeekEnd.setDate(lastWeekStart.getDate() + 6);
            fromDate = fromDateAD = formatDate(lastWeekStart);
            toDate = toDateAD = formatDate(lastWeekEnd);
            break;

        case 'this_month':
            const firstDayOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
            fromDate = fromDateAD = formatDate(firstDayOfMonth);
            toDate = toDateAD = formatDate(today);
            break;

        case 'this_year':
            const firstDayOfYear = new Date(today.getFullYear(), 0, 1);
            fromDate = fromDateAD = formatDate(firstDayOfYear);
            toDate = toDateAD = formatDate(today);
            break;

        case 'custom':
            if (!fromDate && fromDateAD) fromDate = fromDateAD;
            if (!toDate && toDateAD) toDate = toDateAD;
            if (!fromDateAD && fromDate) fromDateAD = fromDate;
            if (!toDateAD && toDate) toDateAD = toDate;
            break;

        default:
            break;
    }

    // Assign resolved dates to scope
    $scope.DocumentReference.FROM_DATE = fromDate;
    $scope.DocumentReference.TO_DATE = toDate;
    $scope.DocumentReference.FROM_DATE_AD = fromDateAD;
    $scope.DocumentReference.TO_DATE_AD = toDateAD;

    // Refresh the grid
    const grid = $('#referenceGrid').data('kendoGrid');
    if (grid) {
        grid.dataSource.read();
    } else {
        console.error('Reference Grid not found.');
    }
};

// Take reference
/*$scope.bindRefrenceDataToTemplate = function () {
    if (checkedItems.length === 0) {
        alert("Please choose at least one item.");
        return;
    }

    // Save checked items to global itemListData
    $scope.itemListData = angular.copy(checkedItems); // Deep copy if needed

    $rootScope.refCheckedItem = checkedItems;
    console.log($scope.itemListData);
    showloader(); // Assume defined globally
    getVoucherDetailForReference(checkedItems).then(function () {
        $('#RefrenceModel').modal('hide');
    });
};*/

function getVoucherDetailForReference(checkedItems) {
    var tableName = $("#refrenceTypeMultiSelect").data("kendoDropDownList").value() === "0" ? $scope.RefTableName : $scope.RefTableName;
    var formCode = !$scope.DocumentReference.TEMPLATE || $scope.DocumentReference.TEMPLATE === "0" ? $scope.ref_form_code : $scope.DocumentReference.TEMPLATE;
    var model = {
        checkList: checkedItems,
        FormCode: formCode,
        TableName: tableName,
        ROW: $scope.ROW
    };

    return $http.post('/api/TemplateApi/GetVoucherDetailForReferenceEdit', model)
        .then(function (response) {
            if (response.data.length === 0) {
                hideloader();
                return;
            }

            if ($scope.ModuleCode === "04") {
                $scope.refrenceFn(response, function () {
                    angular.forEach($scope.masterModels || {}, function (mvalue, mkey) {
                        if (mkey === "CUSTOMER_CODE") {
                            var req = "/api/TemplateApi/GetCustomerInfoByCode?filter=" + mvalue;
                            $http.get(req).then(function (results) {
                                if (results.data.length > 0) {
                                    $("#customers").data('kendoComboBox').dataSource.data([{
                                        CustomerCode: mvalue,
                                        CustomerName: results.data[0].CustomerName,
                                        REGD_OFFICE_EADDRESS: results.data[0].REGD_OFFICE_EADDRESS,
                                        TEL_MOBILE_NO1: results.data[0].TEL_MOBILE_NO1,
                                        TPIN_VAT_NO: results.data[0].TPIN_VAT_NO,
                                        Type: "code"
                                    }]);
                                }
                            });
                        }
                    });

                    angular.forEach($scope.childModels || {}, function (cval, ckey) {
                        if (cval.hasOwnProperty("ITEM_CODE")) {
                            var ireq = "/api/TemplateApi/getItemEdesc?code=" + cval.ITEM_CODE;
                            $http.get(ireq).then(function (results) {
                                $("#products_" + ckey).data('kendoComboBox').dataSource.data([{
                                    ItemCode: cval.ITEM_CODE,
                                    ItemDescription: results.data,
                                    Type: "code"
                                }]);
                            });
                        }
                    });

                    hideloader();
                    if ($scope.freeze_master_ref_flag === "Y") {
                        $(".btn-action a").css('display', 'none');
                    } else {
                        $(".btn-action a").css('display', 'inline');
                    }
                });
            } else {
                hideloader();
            }
        });
}

// Placeholder for refrenceFn
$scope.refrenceFn = function (response, callback) {
    $scope.quotationData = response.data[0];
    $scope.productFormList = response.data.map(function (item, index) {
        return {
            TID: item.TENDER_NO || '',
            ID: index + 1,
            ItemDescription: item.ITEM_EDESC || '',
            QUANTITY: item.QUANTITY || 0,
            UNIT: item.MU_CODE || '',
            UNIT_PRICE: item.UNIT_PRICE || 0,
            TOTAL_PRICE: item.TOTAL_PRICE || 0
        };
    });
    $scope.createPanel = true;
    $scope.tablePanel = false;
    $scope.$applyAsync();
    callback();
};

// Cancel function
$scope.btnRefrenceCancel = function () {
    $scope.DocumentReference = {
        FORM_CODE: null,
        ITEM_DESC: null,
        TENDER_NO: null,
        FROM_DATE: null,
        TO_DATE: null,
        FROM_DATE_AD: null,
        TO_DATE_AD: null
    };
    $scope.TEMPLATE = null;
    $scope.ROW = 'nonreference';
    $scope.dateFilter = 'previous';
    checkedItems = [];
    $("#refrenceTypeMultiSelect").data('kendoDropDownList').value('');
    $('#referenceGrid').data('kendoGrid').dataSource.data([]);
    $('#RefrenceModel').modal('hide');
};

// Initialize date pickers
$timeout(function () {
    // BS Date Pickers
    $("#fromInputDateVoucher").datepicker({
        format: 'mm/dd/yyyy',
        autoclose: true
    }).on('changeDate', function (e) {
        $scope.$apply(function () {
            $scope.DocumentReference.FROM_DATE = e.date || null;
            $scope.updateDateFilter();
        });
    });

    $("#toInputDateVoucher").datepicker({
        format: 'mm/dd/yyyy',
        autoclose: true
    }).on('changeDate', function (e) {
        $scope.$apply(function () {
            $scope.DocumentReference.TO_DATE = e.date || null;
            $scope.updateDateFilter();
        });
    });

    // AD Date Pickers
    $("#fromInputDateVoucherAD").datepicker({
        format: 'mm/dd/yyyy',
        autoclose: true
    }).on('changeDate', function (e) {
        $scope.$apply(function () {
            $scope.DocumentReference.FROM_DATE_AD = e.date || null;
            $scope.updateDateFilter();
        });
    });

    $("#toInputDateVoucherAD").datepicker({
        format: 'mm/dd/yyyy',
        autoclose: true
    }).on('changeDate', function (e) {
        $scope.$apply(function () {
            $scope.DocumentReference.TO_DATE_AD = e.date || null;
            $scope.updateDateFilter();
        });
    });
}, 0);


$scope.ItemSelect = {
    autoBind: true,
    virtual: {
        itemHeight: 26,
        valueMapper: function (options) {
            options.success();
        }
    },
    dataSource: {
        transport: {
            read: {
                url: window.location.protocol + "//" + window.location.host + "/api/QuotationApi/ItemDetails",
                dataType: "json"
            }
        }
    },
    dataTextField: "ItemDescription",
    dataValueField: "ItemCode",
    filter: "contains",
    autoClose: true,
    change: function (e) {
        var selectedItem = this.dataItem();
    }
};



QMModule.controller('valueSetup', function ($scope, $rootScope, $http, $filter, $timeout) {
    $scope.initGrid = function () {
        $("#kGrid").kendoGrid({
            dataSource: {
                transport: {
                    read: {
                        url: "/api/QuotationApi/getUserValue",
                        dataType: "json"
                    }
                },
                schema: {
                    model: {
                        id: "ID",
                        fields: {
                            ID: { type: "number", editable: false },
                            Employee_Name: { type: "string", editable: false },
                            EMPLOYEE_EDESC: { type: "string", editable: false },
                            QUOTATION_APPROVAL_LIMIT: { type: "number", editable: true, nullable: true }
                        }
                    }
                }
            },
            height: 400,
            editable: "inline",
            sortable: true,
            pageable: false,
            columns: [
                {
                    title: "S.No",
                    width: "60px"
                },
                {
                    field: "ID",
                    hidden: true
                },
                {
                    field: "EMPLOYEE_EDESC",
                    title: "Employee Name",
                    width: "250px"
                },
                {
                    field: "QUOTATION_APPROVAL_LIMIT",
                    title: "Amount",
                    width: "150px",
                    editor: numericEditor,
                    template: "#= QUOTATION_APPROVAL_LIMIT != null ? QUOTATION_APPROVAL_LIMIT : '' #"
                },
                {
                    command: [
                        {
                            name: "edit",
                            text: { edit: "Edit", update: "Update", cancel: "Cancel" }
                        },
                        {
                            name: "saveCustom",
                            text: "Save",
                            click: function (e) {
                                e.preventDefault();
                                var tr = $(e.target).closest("tr");
                                var grid = $("#kGrid").data("kendoGrid");
                                var dataItem = grid.dataItem(tr);

                                var payload = {
                                    ID: dataItem.ID,
                                    Employee_Name: dataItem.Employee_Name,
                                    QUOTATION_APPROVAL_LIMIT: dataItem.QUOTATION_APPROVAL_LIMIT
                                };

                                $http.post("/api/QuotationApi/setUserValue", payload).then(function () {
                                    alert("Successfully saved.");
                                    grid.dataSource.read(); // refresh
                                }, function (err) {
                                    alert("Save failed.");
                                    console.error(err);
                                });
                            }
                        }
                    ],
                    title: "Action",
                    width: "180px"
                }
            ],
            dataBound: function () {
                var grid = this;
                var items = grid.items();
                var rowNumber = 1;

                $(items).each(function () {
                    var row = $(this);
                    row.find("td:first").html(rowNumber);
                    rowNumber++;
                });
            }


        });
    };

    let rowNumber = 0;

    // Custom numeric editor to handle nulls
    function numericEditor(container, options) {
        $('<input name="' + options.field + '"/>')
            .appendTo(container)
            .kendoNumericTextBox({
                format: "n2",
                decimals: 2,
                min: 0
            });
    }

    $scope.initGrid();
});