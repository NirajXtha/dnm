distributionModule.controller('contractSetupCtrl', function ($scope, BrandingService, $routeParams) {
    debugger;
    var bunch = {
        GetContract: window.location.protocol + "//" + window.location.host + "/api/Branding/getAllContractList",
        Supplier: window.location.protocol + "//" + window.location.host + "/api/Branding/GetSupplier",
        Reseller: window.location.protocol + "//" + window.location.host + "/api/Branding/GetBrdReseller",
        GetBrdItem: window.location.protocol + "//" + window.location.host + "/api/DistributionPurchase/GetAllItems?type=B",
        GetDistItem: window.location.protocol + "//" + window.location.host + "/api/DistributionPurchase/GetAllItems?type=D",
        GetCustomer: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetIndividualCustomer",
        GetArea: window.location.protocol + "//" + window.location.host + "/api/Branding/GetArea",
        Question: window.location.protocol + "//" + window.location.host + "/api/Branding/GetQuestionList",
        BrandType: window.location.protocol + "//" + window.location.host + "/api/Branding/GetBrandType",
        GetUnit: window.location.protocol + "//" + window.location.host + "/api/Branding/GetItemUnit",
    }
    // $scope.contractCategory;
    $scope.contractCategory = {
        PRODUCT_ITEMS: [{
            ITEM_CODE: '',
            QUANTITY: ''
        }]
    };

    $scope.add = function () {

        var f = document.getElementById('file').files[0],
            r = new FileReader();

        r.onloadend = function (e) {
            var data = e.target.result;
            //send your binary data via $http or $resource or do anything else with it
        }

        r.readAsBinaryString(f);
    }

    $scope.contractSetupHeader = "Contract Setup";
    $scope.saveAction = "Save";
    $scope.SaveNUpdateIcon = "fa fa-floppy-o";
    $scope.formTitle = "Add Contract";
    $scope.brdIcon = "fa fa-plus-square-o";
    $scope.createPanel = false;
    $scope.ResellerHideandShow = false;
    $scope.ProductHideandShow = false;
    $scope.QuantityHideandShow = false;


    $scope.gridScheme = {
        change: changeContract,
        dataSource: {
            transport: {
                read: bunch.GetContract + "?type=S",
            },
            pageSize: 20
        },
        groupable: true,
        sortable: true,
        height: 400,
        pageable: {
            refresh: true,
            pageSizes: 50,
            buttonCount: 5
        },
        dataBound: function (o) {

            GetSetupSetting("ContractSetup");
            var grid = o.sender;
            if (grid.dataSource.total() == 0) {
                var colCount = grid.columns.length;
                $(o.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                displayPopupNotification("No Data Found.", "info");
            }
        },
        selectable: true,
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
        columns: [
            {
                field: "CONTRACT_EDESC",
                title: "Contract Name",
                width: 250
            },
            {
                field: "CONTRACT_TYPE",
                title: "Type",
                template: function (evt) {
                    if (evt.CONTRACT_TYPE == "VISIT") {
                        return "<span style='color:#00e64d'>" + "Visited" + "</span>";
                    }
                    else {
                        return "<spna style='color:#ff9900'>" + "Not Visited" + "</span>";
                    }

                }

            },
            {
                field: "START_DATE",
                title: " Start Date",
                template: "#= kendo.toString(kendo.parseDate(START_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
            },
            {
                field: "END_DATE",
                title: "End Date",
                template: "#= kendo.toString(kendo.parseDate(END_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
            },
            {
                title: "Action",
                template: " <a class='fa fa-edit editAction' ng-click='UpdateArea($event)' title='Edit'></a>&nbsp &nbsp<a class='fa fa-trash-o deleteAction' ng-click='deleteContract($event)' title='delete'></a>  ",

            }
        ]
    }

    $scope.gridEvent = {
        change: changeContract,
        dataSource: {
            transport: {
                read: bunch.GetContract + "?type=E",
            },
            pageSize: 20
        },
        groupable: true,
        sortable: true,
        height: 400,
        pageable: {
            refresh: true,
            pageSizes: 50,
            buttonCount: 5
        },
        dataBound: function (o) {

            GetSetupSetting("ContractSetup");
            var grid = o.sender;
            if (grid.dataSource.total() == 0) {
                var colCount = grid.columns.length;
                $(o.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                displayPopupNotification("No Data Found.", "info");
            }
        },
        selectable: true,
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
        columns: [
            {
                field: "CONTRACT_EDESC",
                title: "Contract Name",
                width: 250
            },
            {
                field: "CONTRACT_TYPE",
                title: "Type",
                template: function (evt) {
                    if (evt.CONTRACT_TYPE == "VISIT") {
                        return "<span style='color:#00e64d'>" + "Visited" + "</span>";
                    }
                    else {
                        return "<spna style='color:#ff9900'>" + "Not Visited" + "</span>";
                    }

                }

            },
            {
                field: "START_DATE",
                title: " Start Date",
                template: "#= kendo.toString(kendo.parseDate(START_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
            },
            {
                field: "END_DATE",
                title: "End Date",
                template: "#= kendo.toString(kendo.parseDate(END_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
            },
            {
                title: "Action",
                template: " <a class='fa fa-edit editAction' ng-click='UpdateArea($event)' title='Edit'></a>&nbsp &nbsp<a class='fa fa-trash-o deleteAction' ng-click='deleteContract($event)' title='Delete'></a>  ",

            }
        ]
    }

    $scope.gridOther = {
        change: changeContract,
        dataSource: {
            transport: {
                read: bunch.GetContract + "?type=O",
            },
            pageSize: 20
        },
        groupable: true,
        sortable: true,
        height: 400,
        pageable: {
            refresh: true,
            pageSizes: 50,
            buttonCount: 5
        },
        dataBound: function (o) {

            GetSetupSetting("ContractSetup");
            var grid = o.sender;
            if (grid.dataSource.total() == 0) {
                var colCount = grid.columns.length;
                $(o.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                displayPopupNotification("No Data Found.", "info");
            }
        },
        selectable: true,
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
        columns: [
            {
                field: "CONTRACT_EDESC",
                title: "Contract Name",
                width: 250
            },
            {
                field: "CONTRACT_TYPE",
                title: "Type",
                template: function (evt) {
                    if (evt.CONTRACT_TYPE == "VISIT") {
                        return "<span style='color:#00e64d'>" + "Visited" + "</span>";
                    }
                    else {
                        return "<spna style='color:#ff9900'>" + "Not Visited" + "</span>";
                    }

                }

            },
            {
                field: "START_DATE",
                title: " Start Date",
                template: "#= kendo.toString(kendo.parseDate(START_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
            },
            {
                field: "END_DATE",
                title: "End Date",
                template: "#= kendo.toString(kendo.parseDate(END_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
            },
            {
                title: "Action",
                template: " <a class='fa fa-edit editAction' ng-click='UpdateArea($event)' title='Edit'></a>&nbsp &nbsp<a class='fa fa-trash-o deleteAction' ng-click='deleteContract($event)' title='Delete'></a>  ",

            }
        ]
    }

    $scope.UpdateArea = function (evt) {
        debugger;
        var grid = $("#grid").data("kendoGrid");
        var row = evt.target.closest("tr");
        var item = grid.dataItem(row);
        var uniq = "front";
        bindMultipleData(item, uniq);
    }

    function changeContract(evt) {
        debugger;
        selectedRow = this.select();
        var item = this.dataItem(selectedRow);
        bindMultipleData(item);

    }

    function bindMultipleData(item, uniq) {
        debugger;
        $scope.saveAction = "Update"
        $scope.SaveNUpdateIcon = "fa fa-pencil-square-o";
        $scope.formTitle = "Update Contract"
        $scope.brdIcon = "fa fa-pencil-square-o"
        $scope.createPanel = true;
        angular.element('#contractType').data("kendoMultiSelect").value(item.CONTRACT_TYPE);
        angular.element('#amountType').data("kendoMultiSelect").value(item.AMOUNT_TYPE);
        //checkbox 
        if (item.IS_ROUTE_PLAN == "Y") {
            $('input#someSwitchOptionDefaultAsrootplan').prop('checked', true);
        }
        else {
            $('input#someSwitchOptionDefaultAsrootplan').prop('checked', false);
        }
        //chechbox for Gift name
        if (item.HAS_GIFT_NAME == "Y") {
            $('input#someSwitchOptionDefaultAsGiftName').prop('checked', true);
        }
        else {
            $('input#someSwitchOptionDefaultAsGiftName').prop('checked', false);
        }


        // angular.element('#BrandType').data("kendoMultiSelect").value(item.BRANDING_TYPE);
        //store in arr and bind into the field
        var arr = [];
        if (!_.isEmpty(item.ITEM_CODE_STRING))
            $.each(item.ITEM_CODE_STRING.split(','), function (i, obj) {
                arr.push(obj);
            });
        var resellerString = [];
        if (!_.isEmpty(item.RESELLER_CODE_STRING))
            $.each(item.RESELLER_CODE_STRING.split(','), function (i, obj) {
                resellerString.push(obj);
            });
        var customers = [];
        if (!_.isEmpty(item.CUSTOMER_CODE))
            $.each(item.CUSTOMER_CODE.split(','), function (i, obj) {
                customers.push(obj);
            });
        
        var itemsk = [];
        if (!_.isEmpty(item.ITEM_CODE))
            $.each(item.ITEM_CODE.split(','), function (i, obj) {
                itemsk.push(obj);
            });
        var brandArr = [];
        if (!_.isEmpty(item.BRAND_CODE_STRING))
            $.each(item.BRAND_CODE_STRING.split(','), function (i, obj) {
                brandArr.push(obj);
            });
        $scope.contractCategory = {
            areaMultiSelect: arr,
            ID: item.CONTRACT_CODE,
            contractName: item.CONTRACT_EDESC,
            QstType: [item.SET_CODE],
            supplierMultiSelect: [item.SUPPLIER_CODE],
            customerMultiSelect: customers,
            startDatePicker: $.datepicker.formatDate('mm/dd/yy', new Date(item.START_DATE)),
            endDatePicker: $.datepicker.formatDate('mm/dd/yy', new Date(item.END_DATE)),
            selectedBrands: brandArr,
            contractType: [item.CONTRACT_TYPE],
            brdReseller: resellerString, //comma seperated and binded here
            PRODUCT_ITEMS: item.PRODUCT_ITEMS.length == 0 ? [] : item.PRODUCT_ITEMS,
            ProductQuantity: item.PRODUCT_QUANTITY,
            ProductMUCode: [item.MU_CODE],
            brdItems: itemsk,
            amountType: [item.AMOUNT_TYPE],
            BrandType: [item.BRANDING_TYPE],
            Amount: item.AMOUNT,
            paymentDate: $.datepicker.formatDate('mm/dd/yy', new Date(item.PAYMENT_DATE)),
            advanceAmt: item.ADVANCE_AMOUNT,
            contractorName: item.CONTRACTOR_NAME,
            ContractorAddress: item.CONTRACTOR_ADDRESS,
            email: item.OWNER_EMAIL,
            contact: parseInt(item.OWNER_PHONE),
            designation: item.CONTRACTOR_DESIGNATION,
            ownerCompany: item.OWNER_COMPANY_NAME,
            panNo: item.OWNER_PAN_NO,
            vatNo: item.OWNER_VAT_NO,
            ownerName: item.OWNER_NAME,
            ownerAddress: item.OWNER_ADDRESS,
            Discription: item.DESCRIPTION,
            IS_ROUTE_PLAN: item.IS_ROUTE_PLAN,
            HAS_GIFT_NAME: item.HAS_GIFT_NAME
        };

        if (item.CONTRACT_TYPE == "VISIT") {
            $scope.ResellerHideandShow = true;
            if (item.AMOUNT_TYPE == "PRODUCT" || item.AMOUNT_TYPE == "SCHEME_ITEM") {

                $scope.ProductHideandShow = true;
                $scope.QuantityHideandShow = true;


            }

        }

        else {
            $scope.ProductHideandShow = false;
            $scope.QuantityHideandShow = false;
            $scope.ProductHideandShow = false;
        }
        if (uniq != "front")
            $scope.$apply();
    }

    $("#contractType").kendoMultiSelect({
        animation: {
            open: {
                effects: "zoom:in",
                duration: 300
            }
        },
        placeholder: "Please Select...",
        change: resellerchange
    });

    $("#amountType").kendoMultiSelect({
        animation: {
            open: {
                effects: "zoom:in",
                duration: 300,
            }
        },
        placeholder: "Please Select...",
        //change: ChangeItems
    });

    function resellerchange(evt) {
        if (evt.sender.value()[0] == "VISIT") {
            $scope.ResellerHideandShow = true;
        }
        else {
            $scope.ResellerHideandShow = false;
            angular.element('#brdReseller').data("kendoMultiSelect").value("");
        }
    }

    //function ChangeItems(evt) {
    //    if (evt.sender.value()[0] == "PRODUCT" || evt.sender.value()[0] == "SCHEME_ITEM") {
    //        GetItemForProductAndScheme();
    //        $scope.isRequired = "required";
    //        $scope.$apply();
    //    }  
    //    else {
    //        $scope.ProductHideandShow = false;
    //        $scope.QuantityHideandShow = false;
    //        angular.element('#ProductItems').data("kendoMultiSelect").value("");
    //    }
    //}
    //function GetItemForProductAndScheme() {
    //    $scope.ProductHideandShow = true;
    //    $scope.QuantityHideandShow = true;
    //    $scope.ProductItems = {
    //        dataTextField: "ITEM_EDESC",
    //        dataValueField: "ITEM_CODE",
    //        height: 600,
    //        valuePrimitive: true,
    //        maxSelectedItems: 1,
    //        headerTemplate: '<div class="col-md-offset-3"><strong>Items...</strong></div>',
    //        placeholder: "Select Items...",
    //        autoClose: false,
    //        dataBound: function (e) {
    //            //var current = this.value();
    //            //this._savedOld = current.slice(0);
    //            //$("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
    //            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
    //        },
    //        dataSource: {
    //            transport: {
    //                read: {
    //                    url: bunch.GetItem,
    //                    dataType: "json"
    //                }
    //            }
    //        },
    //    };
    //}

    $scope.ProductItems = {
        dataTextField: "ITEM_EDESC",
        dataValueField: "ITEM_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Items...</strong></div>',
        placeholder: "Select Items...",
        autoClose: false,
        dataBound: function (e) {
            //var current = this.value();
            //this._savedOld = current.slice(0);
            //$("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });

            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: bunch.GetBrdItem,
                    dataType: "json"
                }
            }
        },
    };

    $("#contractType").data("kendoMultiSelect").options.maxSelectedItems = 1;
    $("#amountType").data("kendoMultiSelect").options.maxSelectedItems = 1;

    $scope.supplierMultiSelect = {
        dataTextField: "SUPPLIER_EDESC",
        dataValueField: "SUPPLIER_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>...Supplier...</strong></div>',
        placeholder: "Select Supplier...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: bunch.Supplier,
                    dataType: "json"
                }
            }
        },
    };
    //Question Type

    $scope.QstType = {
        dataTextField: "TITLE",
        dataValueField: "SET_CODE",
        height: 600,
        maxSelectedItems: 1,
        valuePrimitive: true,
        headerTemplate: '<div class="col-md-offset-3"><strong>...Type...</strong></div>',
        placeholder: "Select Type...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: bunch.Question,
                    dataType: "json"
                }
            }
        },
    };

    $scope.brdReseller = {
        dataTextField: "RESELLER_NAME",
        dataValueField: "RESELLER_CODE",
        height: 600,
        valuePrimitive: true,
        //  maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>...Reseller...</strong></div>',
        placeholder: "Select Reseller...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: bunch.Reseller,
                    dataType: "json"
                }
            }
        },
    };

    function buildFilters(dataItems) {
        debugger;
        var filters = [],
            length = dataItems.length,
            idx = 0, dataItem;
        if (length == 0) {
            $("#brdItems").data("kendoMultiSelect").value("");
        }
        for (idx = 0; idx < length; idx++) {
            dataItem = dataItems[idx];
            var data = $("#brdItems").data("kendoMultiSelect").dataSource.data();
            var filterdata = _.filter(data, function (da) { return da.BRAND_NAME == dataItem.BRAND_NAME; });
            for (var i = 0; i < filterdata.length; i++) {
                filters.push(filterdata[i].ITEM_CODE);
            }
            //Required field validation for the Multiple Item and only one has been seleted and save into the database

            //$scope.createContractForm.brdItems.$error.required = false;
            //$scope.createContractForm.brdItems.$invalid = false;
            //$("#brdItems").data("kendoMultiSelect").open();

            //$("#brdItems").data("kendoMultiSelect").value(filters);
            $scope.contractCategory.brdItems = filters;
            $scope.$apply();
            $("#brdItems").data("kendoMultiSelect").open();
            //filters.push({
            //    field: "BRAND_NAME",
            //    operator: "eq",
            //    value: parseInt(dataItem.BRAND_NAME)
            //});
        }
    };
    $scope.dataSourceBrand = [];
    var productsDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: bunch.GetDistItem,
                dataType: "json"
            }
        }
    });
    productsDataSource.fetch(function () {
        $scope.distBrandSelectOptions = {
            dataTextField: "BRAND_NAME",
            dataValueField: "BRAND_NAME",
            height: 600,
            //maxSelectedItems: 1,
            valuePrimitive: true,
            headerTemplate: '<div class="col-md-offset-3"><strong>Brands...</strong></div>',
            placeholder: "Select Brands...",
            autoClose: false,
            dataBound: function (e) {
                var current = this.value();
                this._savedOld = current.slice(0);
                $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
            },
            dataSource: new kendo.data.DataSource({
                data: _.uniq(this.data(), "BRAND_NAME"),
            }),
            change: function () {
                buildFilters(this.dataItems());
            }
        };
    });

    $scope.brdItems = {
        dataTextField: "ITEM_EDESC",
        dataValueField: "ITEM_CODE",
        height: 600,
        valuePrimitive: true,
        headerTemplate: '<div class="col-md-offset-3"><strong>Items...</strong></div>',
        placeholder: "Select Items...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: productsDataSource

    };

    $scope.customerMultiSelect = {
        dataTextField: "CUSTOMER_EDESC",
        dataValueField: "DISTRIBUTOR_CODE",
        height: 600,
        valuePrimitive: true,
        //maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>...Customer...</strong></div>',
        placeholder: "Select Customer...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: bunch.GetCustomer,
                    dataType: "json"
                }
            }
        },
    };

    //Branding Type 
    $scope.BrandType = {
        dataTextField: "BRAND_TYPE",
        dataValueField: "BRAND_TYPE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>...Activity...</strong></div>',
        placeholder: "Select Activity...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: bunch.BrandType,
                    dataType: "json"
                }
            }
        },
    };

    $scope.areaMultiSelect = {
        dataTextField: "AREA_NAME",
        dataValueField: "AREA_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems:1,
        headerTemplate: '<div class="col-md-offset-3"><strong>...Area...</strong></div>',
        placeholder: "Select Area...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: bunch.GetArea,
                    dataType: "json"
                }
            }
        },
        change: function (e) {
            var multiSelect = $("#customerMultiSelect").data("kendoMultiSelect");
            var ResellerMultiSelect = $("#brdReseller").data("kendoMultiSelect");
            if (e.sender.value().length > 0) {
                //dist select
                var selectedDistributor = _.pluck(_.filter(multiSelect.dataSource.data(), function (x) {
                    return x.AREA_CODE == e.sender.value()[0];
                }), "DISTRIBUTOR_CODE");

                //reseller select
                var selectedReseller = _.pluck(_.filter(ResellerMultiSelect.dataSource.data(), function (x) {
                    return x.AREA_CODE == e.sender.value()[0];
                }), "RESELLER_CODE");

                $scope.contractCategory.customerMultiSelect = selectedDistributor.length > 0 ? selectedDistributor : null;
                $scope.contractCategory.brdReseller = selectedReseller.length > 0 ? selectedReseller : null;
            } else {
                $scope.contractCategory.customerMultiSelect = null;
                $scope.contractCategory.brdReseller = null;
            }
        }
    };


    //Get MU code from here
    $scope.ProductMUCode = {
        dataTextField: "MU_EDESC",
        dataValueField: "MU_CODE",
        height: 600,
        maxSelectedItems: 1,
        valuePrimitive: true,
        headerTemplate: '<div class="col-md-offset-3"><strong>...unit...</strong></div>',
        placeholder: "Select Unit...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: bunch.GetUnit,
                    dataType: "json"
                }
            }
        },
    }

    $scope.SaveContract = function (isValid, type) {
        debugger;
        if (!isValid) {
            var validation = [
                { ContractName: $scope.createContractForm.contractName.$invalid },
                { supplier: $scope.createContractForm.supplierMultiSelect.$invalid },
                { Customer: $scope.createContractForm.customerMultiSelect.$invalid },
                { startDate: $scope.createContractForm.startDatePicker.$invalid },
                { endDate: $scope.createContractForm.endDatePicker.$invalid },
                { Item: $scope.createContractForm.brdItems.$invalid },
                { area: $scope.createContractForm.areaMultiSelect.$invalid },
                { email: $scope.createContractForm.Email.$invalid },
                { contact: $scope.createContractForm.mobile_number.$error.pattern }
            ]

            if (validation[0].ContractName == true) {

                displayPopupNotification("Plese Select Name", "warning");
                return
            }
            if (validation[1].supplier == true) {

                displayPopupNotification("Plese Select Supplier", "warning");
                $("#supplierMultiSelect").data("kendoMultiSelect").open();
                return
            }
            if (validation[2].Customer == true) {

                displayPopupNotification("Plese Select Customer", "warning");
                $("#customerMultiSelect").data("kendoMultiSelect").open();
                return
            }
            if (validation[3].startDate == true) {

                displayPopupNotification("Plese Select Start Date", "warning");
                return
            }
            if (validation[4].endDate == true) {

                displayPopupNotification("Plese Select End Date", "warning");
                return
            }
            if (validation[5].Item == true) {

                displayPopupNotification("Plese Select Item", "warning");
                $("#brdItems").data("kendoMultiSelect").open();
                return
            }
            if (validation[6].area == true) {
                displayPopupNotification("Plese Select Area", "warning");
                $("#areaMultiSelect").data("kendoMultiSelect").open();
                return
            }
            if (validation[7].email == true) {
                displayPopupNotification("Please Enter Valid Email", "warning");
                return
            }
            if (validation[8].contact == true) {
                displayPopupNotification("Invalid Contact Number!", "warning");
                return
            }
        }



        
        if (type == "S" || type == "E") { //s=> Scheme/Event contract. Only scheme and even contract contains items
            if ($scope.contractCategory.PRODUCT_ITEMS.length == 0) {
                displayPopupNotification("No items selected!", "warning");
                return;
            }
            for (var i = 0; i < $scope.contractCategory.PRODUCT_ITEMS.length; i++) {
                var val = $scope.contractCategory.PRODUCT_ITEMS[i];
                if (val.ITEM_CODE == '' || val.QUANTITY == '') {
                    displayPopupNotification("Item or quantity cannot be empty!", "warning");
                    return;
                }
                else
                    $scope.contractCategory.PRODUCT_ITEMS[i].ITEM_CODE = val.ITEM_CODE.length == 1 ? val.ITEM_CODE.join() : val.ITEM_CODE;
            }
            $scope.contractCategory.contractType = type == "S" ? ["VISIT"] : $scope.contractCategory.contractType;
            $scope.contractCategory.amountType = type == "S" ? ["SCHEME_ITEM"] : ["EVENT"];
        }
        else if (type == "O") {
            if ($scope.contractCategory.PRODUCT_ITEMS.length == 0) {
                displayPopupNotification("No items selected!", "warning");
                return;
            }
            for (var i = 0; i < $scope.contractCategory.PRODUCT_ITEMS.length; i++) {
                var val = $scope.contractCategory.PRODUCT_ITEMS[i];
                if (val.ITEM_CODE == '' || val.QUANTITY == '') {
                    displayPopupNotification("Item or quantity cannot be empty!", "warning");
                    return;
                }
                else
                    $scope.contractCategory.PRODUCT_ITEMS[i].ITEM_CODE = val.ITEM_CODE.length == 1 ? val.ITEM_CODE.join() : val.ITEM_CODE;
            }
            $scope.contractCategory.amountType = $scope.contractCategory.amountType;
        }

        var obj = $scope.contractCategory;
        
        obj.CONTRACT_CODE = $scope.contractCategory.ID;
        obj.CONTRACT_EDESC = $scope.contractCategory.contractName;
        //obj.SUPPLIER_CODE = $scope.contractCategory.supplierMultiSelect[0];
        obj.CUSTOMERS = $scope.contractCategory.customerMultiSelect;
        obj.START_DATE = $scope.contractCategory.startDatePicker;
        obj.END_DATE = $scope.contractCategory.endDatePicker;
        obj.AREA_CODE = $scope.contractCategory.areaMultiSelect;
        obj.ITEM_CODES = $scope.contractCategory.brdItems;
        if ($scope.contractCategory.contractType == undefined) {
            obj.CONTRACT_TYPE = "NON-VISIT";
        }
        else {
            obj.CONTRACT_TYPE = $scope.contractCategory.contractType[0];
        }
        if ($scope.contractCategory.amountType != undefined) {
            obj.AMOUNT_TYPE = $scope.contractCategory.amountType[0];
        }
        if ($scope.contractCategory.BrandType != undefined) {
            obj.BRANDING_TYPE = $scope.contractCategory.BrandType[0]
        }

        if ($scope.contractCategory.brdReseller != undefined) {
            obj.RESELLER_CODE = $scope.contractCategory.brdReseller;
        }

        //if (obj.RESELLER_CODE != null) {
        //    if ($scope.contractCategory.contractType[0] == "NON-VISIT") {
        //        obj.STATUS = "Inactive";
        //    }
        //    else {
        //        obj.STATUS = "active";
        //    }
        //}

        if ($scope.contractCategory.selectedBrands != undefined) {
            obj.BRAND_CODE = $scope.contractCategory.selectedBrands;
        }
        if ($scope.contractCategory.supplierMultiSelect != undefined) {
            obj.SUPPLIER_CODE = $scope.contractCategory.supplierMultiSelect[0];
        }

        obj.ITEM_CODE = $scope.contractCategory.brdItems[0];
        obj.AMOUNT = $scope.contractCategory.Amount;
        obj.PAYMENT_DATE = $scope.contractCategory.paymentDate;
        obj.ADVANCE_AMOUNT = $scope.contractCategory.advanceAmt;
        obj.CONTRACTOR_NAME = $scope.contractCategory.contractorName;
        obj.CONTRACTOR_ADDRESS = $scope.contractCategory.ContractorAddress;
        obj.OWNER_EMAIL = $scope.contractCategory.email;
        obj.OWNER_PHONE = $scope.contractCategory.contact;
        obj.CONTRACTOR_DESIGNATION = $scope.contractCategory.designation;
        obj.OWNER_COMPANY_NAME = $scope.contractCategory.ownerCompany;
        obj.OWNER_PAN_NO = $scope.contractCategory.panNo;
        debugger;
        obj.OWNER_VAT_NO = $scope.contractCategory.vatNo;
        obj.OWNER_NAME = $scope.contractCategory.ownerName;
        obj.OWNER_ADDRESS = $scope.contractCategory.ownerAddress;
        obj.IS_ROUTE_PLAN = $scope.contractCategory.IS_ROUTE_PLAN;
        obj.HAS_GIFT_NAME = $scope.contractCategory.HAS_GIFT_NAME;

        if ($scope.contractCategory.QstType != undefined) {
            obj.SET_CODE = $scope.contractCategory.QstType[0];
        }
        obj.DESCRIPTION = $scope.contractCategory.Discription;

        if ($scope.saveAction == "Save") {
            var saveContract = BrandingService.saveContract(obj);
            saveContract.then(function (response) {
                if (response.data == "success") {
                    displayPopupNotification("success", "success")
                    SeperateFunction.ReadDatabase();
                    $scope.cancelForm();
                }
                else {
                    displayPopupNotification("something went wrong", "error");
                }
            })
        }
        else {
            BrandingService.updateContract(obj).then(function (response) {

                if (response.data == "success") {
                    displayPopupNotification("Update success", "success")
                    SeperateFunction.ReadDatabase();
                    $scope.cancelForm();
                }
                else {
                    displayPopupNotification("something went wrong", "error")
                }
            });
        }
    }

    $scope.cancelForm = function () {
        $scope.saveAction = "Save"
        $scope.SaveNUpdateIcon = "fa fa-floppy-o";
        $scope.formTitle = "Add Contract"
        $scope.brdIcon = "fa fa-plus-square-o"
        $scope.contractCategory = {
            PRODUCT_ITEMS: [{
                ITEM_CODE: '',
                QUANTITY: ''
            }]
        };
        $scope.ResellerHideandShow = false;
        $scope.ProductHideandShow = false;
        $scope.QuantityHideandShow = false;
        angular.element('#contractType').data("kendoMultiSelect").value("");
        angular.element('#amountType').data("kendoMultiSelect").value("");
        angular.element('#brdItems').data("kendoMultiSelect").value("");

        SeperateFunction.ReadDatabase();
        $scope.createPanel = false;
    }

    $scope.ClearForm = function () {
        $scope.saveAction = "Save"
        $scope.SaveNUpdateIcon = "fa fa-floppy-o";
        $scope.formTitle = "Add Contract"
        $scope.brdIcon = "fa fa-plus-square-o"
        $scope.contractCategory = {
            PRODUCT_ITEMS: [{
                ITEM_CODE: '',
                QUANTITY: ''
            }]
        };
        $scope.ResellerHideandShow = false;
        $scope.ProductHideandShow = false;
        $scope.QuantityHideandShow = false;
        angular.element('#contractType').data("kendoMultiSelect").value("");
        angular.element('#amountType').data("kendoMultiSelect").value("");
        angular.element('#brdItems').data("kendoMultiSelect").value("");

        // SeperateFunction.ReadDatabase();
    }
    //
    function pageScroll() {
        window.scrollBy(0, 50); // horizontal and vertical scroll increments
        scrolldelay = setTimeout('pageScroll()', 100); // scrolls every 100 milliseconds
    }


    $scope.AutoScrolling = function () {
        $('.slimScroll').slimScroll({

            start: 'bottom',
            wheelStep: 1,
        });
        var bottomCoord = $('.slimScroll')[0].scrollHeight;
        $('.slimScroll').slimScroll({ scrollTo: bottomCoord });
    }

    $scope.deleteContract = function (evt) {

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
                    var grid = $("#grid").data("kendoGrid");
                    var row = evt.target.closest("tr");
                    var item = grid.dataItem(row);
                    var delOnject = {
                        CONTRACT_CODE: item.CONTRACT_CODE,
                        SUPPLIER_CODE: item.SUPPLIER_CODE,
                        CUSTOMER_CODE: item.CUSTOMER_CODE,
                    }
                    BrandingService.deleteContract(delOnject).then(function (response) {
                        if (response.data == "success") {
                            displayPopupNotification("Deleted success", "Success")
                            SeperateFunction.ReadDatabase();
                            $scope.cancelForm();

                        }
                        else {
                            displayPopupNotification("Something went wrong", "error");
                        }
                    });
                }
            }
        });
    };

    $scope.AddClickEvent = function () {
        $scope.createPanel = true;
    }


    var SeperateFunction = (function ($, w) {
        var ReadDatabase = function () {
            return $("#grid").data("kendoGrid").dataSource.read();
        };

        return {
            ReadDatabase: ReadDatabase,
        };
    }(jQuery, window));

    $scope.AddProductItem = function () {
        debugger;
        if (!$scope.contractCategory.PRODUCT_ITEMS)
            $scope.contractCategory.PRODUCT_ITEMS = [];
        $scope.contractCategory.PRODUCT_ITEMS.unshift({
            ITEM_CODE: '',
            QUANTITY: ''
        });
    }

    $scope.RemoveProductItem = function (index) {
        //$scope.contractCategory.PRODUCT_ITEMS.splice(index, 1);
        //if ($scope.contractCategory.PRODUCT_ITEMS.length == 0) {
        //   // $scope.AddProductItem();
        //}

        for (var i = 0; i < $scope.contractCategory.PRODUCT_ITEMS.length; i++) {
            $scope.contractCategory.PRODUCT_ITEMS[i].SN = $scope.contractCategory.PRODUCT_ITEMS.length - i;

        }
        if ($scope.contractCategory.PRODUCT_ITEMS.length != 0) {
            $scope.contractCategory.PRODUCT_ITEMS.splice(index, 1);
        }
    }

    $scope.ItemChange = function (e, index) {
        debugger;
        var selected = e.dataItem;
        if (!checkSelectedItem(selected.ITEM_CODE, index)) {
            e.preventDefault();
            displayPopupNotification("Item already Selected", "warning");
            return;
        }
        $scope.contractCategory.PRODUCT_ITEMS[index].MU_CODE = selected.UNIT;
    }

    var checkSelectedItem = function (item_code) {
        debugger;
        var PreviouslySelected = [];
        for (var i = 0; i < $scope.contractCategory.PRODUCT_ITEMS.length; i++) {
            var val = $scope.contractCategory.PRODUCT_ITEMS[i].ITEM_CODE[0];
            PreviouslySelected.push(val);
        }
        if (PreviouslySelected.includes(item_code))
            return false;
        else
            return true;
    }

});