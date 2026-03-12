distributionModule.controller('SchemeApprovalCtrl', function ($scope, $http, $routeParams, $filter) {
    debugger;
    $scope.showGiftoption = false;
    $scope.showQtyAmtoptions = false;
    $scope.showOptions = false;
    $scope.createPanel = false;
    $scope.prop = "disabled";
    $scope.approveAction = "Approve";
    $scope.rows = [{
        MAX_VALUE: '',
        MIN_VALUE: '',
        Discount: '',
        DiscountType: '',
        GiftQty: '',
        GiftItemCode: []
    }];
    $scope.AddProductItem = function () {
        debugger;
        if (!$scope.rows)
            $scope.rows = [];
        $scope.rows.unshift({
            MAX_VALUE: '',
            MIN_VALUE: '',
            Discount: '',
            DiscountType: '',
            GiftQty: '',
            GiftItemCode: []
        });
    }

    $scope.RemoveProductItem = function (index) {

        debugger;
        for (var i = 0; i < $scope.rows.length; i++) {
            $scope.rows[i].SN = $scope.rows.length - i;

        }
        if ($scope.rows.length != 0) {
            $scope.rows.splice(index, 1);
        }
    }

    

    $scope.Cancel = function () {
        $scope.createPanel = false;
        $("input").prop('disabled', true);
        $("select").prop('disabled', true);
        

    };
    $scope.employeeOptions = {
        filter: "contains",
        dataTextField: "EMPLOYEE_EDESC",
        dataValueField: "EMPLOYEE_CODE",
        autoBind: true,
        placeholder: "Select Employee...",
        maxSelectedItems: 1,
        dataSource: {
            type: "json",
            serverFiltering: true,
            transport: {
                read: {
                    url: "/api/Setup/GetEmployeesandRoute",
                },
                parameterMap: function (data, action) {
                    debugger;
                    if (data.filter != undefined) {
                        if (data.filter.filters[0] != undefined) {
                            var newParams = {
                                filter: data.filter.filters[0].value,
                                empGroup: ""
                            };
                            return newParams;
                        }
                        else {
                            var newParams = {
                                filter: "",
                                empGroup: ""
                            };
                            return newParams;
                        }
                    }
                    else {
                        var newParams = {
                            filter: "",//planCode,
                            empGroup: ""//$("#groups").data("kendoDropDownList").dataItem().GROUPID
                        };
                        return newParams;
                    }
                }
            }
        },

        change: function (e) {
            debugger;
            var areamultiSelect = $("#areaMultiSelect").data("kendoMultiSelect");
            var multiSelect = $("#customerMultiSelect").data("kendoMultiSelect");
            var ResellerMultiSelect = $("#brdReseller").data("kendoMultiSelect");
            if (e.sender.value().length > 0) {
                var data = e.sender.dataSource.data();
                var areacode = _.pluck(_.filter(data, function (x) {

                    return x.EMPLOYEE_CODE == e.sender.value()[0];
                }), "AREA_CODE");



                //area select 
                var selectedAreaCode = [];
                _.filter(areamultiSelect.dataSource.data(), function (x) {
                    /*debugger;*/
                    angular.forEach(areacode[0], function (code) {

                        if (x.AREA_CODE == code) {
                            selectedAreaCode.push(code);

                        };

                    });



                });




                //dist select
                var selectedDistributor = [];
                _.filter(multiSelect.dataSource.data(), function (x) {
                    angular.forEach(areacode[0], function (code) {

                        if (x.AREA_CODE == code) {
                            selectedDistributor.push(x.DISTRIBUTOR_CODE);

                        };

                    });
                });

                //reseller select
                var selectedReseller = [];
                _.filter(ResellerMultiSelect.dataSource.data(), function (x) {
                    angular.forEach(areacode[0], function (code) {

                        if (x.AREA_CODE == code) {
                            selectedReseller.push(x.RESELLER_CODE);

                        };

                    });
                });

                $("#areaMultiSelect").data("kendoMultiSelect").value(selectedAreaCode);
                $("#customerMultiSelect").data("kendoMultiSelect").value(selectedDistributor);
                $("#brdReseller").data("kendoMultiSelect").value(selectedReseller);

                //$scope.customerMultiSelect = selectedDistributor.length > 0 ? selectedDistributor : null;
                //$scope.brdReseller = selectedReseller.length > 0 ? selectedReseller : null;
            } else {
                $scope.selectedemployee = null;
                $scope.customerMultiSelect = null;
                $scope.brdReseller = null;
            }

        }

    };
    $scope.areaSelectOptions = {
        dataTextField: "AREA_NAME",
        dataValueField: "AREA_CODE",
        height: 600,
        change: GetIndividualReport,
        valuePrimitive: true,
        /*maxSelectedItems: 1,*/
        headerTemplate: '<div class="col-md-offset-3"><strong>Area...</strong></div>',
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
                    url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetDistArea",
                    dataType: "json"
                }
            }
        },
        change: function (e) {
            debugger;
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

                $("#customerMultiSelect").data("kendoMultiSelect").value(selectedDistributor);
                $("#brdReseller").data("kendoMultiSelect").value(selectedReseller);

                //$scope.customerMultiSelect = selectedDistributor.length > 0 ? selectedDistributor : null;
                //$scope.brdReseller = selectedReseller.length > 0 ? selectedReseller : null;
            } else {
                $scope.customerMultiSelect = null;
                $scope.brdReseller = null;
            }
        }

    };
    $scope.userSelectOptions = {
        dataTextField: "AREA_NAME",
        dataValueField: "AREA_CODE",
        height: 600,
        change: GetIndividualReport,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Area...</strong></div>',
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
                    url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetDistArea",
                    dataType: "json"
                }
            }
        },
        change: function (e) {
            debugger;
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

                $("#customerMultiSelect").data("kendoMultiSelect").value(selectedDistributor);
                $("#brdReseller").data("kendoMultiSelect").value(selectedReseller);

                //$scope.customerMultiSelect = selectedDistributor.length > 0 ? selectedDistributor : null;
                //$scope.brdReseller = selectedReseller.length > 0 ? selectedReseller : null;
            } else {
                $scope.customerMultiSelect = null;
                $scope.brdReseller = null;
            }
        }

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
                    url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetIndividualCustomer",
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
                    url: window.location.protocol + "//" + window.location.host + "/api/Branding/GetBrdReseller",
                    dataType: "json"
                }
            }
        },
    };


    function GetIndividualReport(evt) {
        $http({
            method: 'GET',
            url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetIndividualGroup?SingleAreaCode=" + evt.sender.value()[0],
        }).then(function successCallback(response) {
            $scope.selectedGroup = response.data[0] ? [response.data[0].GROUPID] : null;
            //response.data[0] ? $("#distGroupSelect").data("kendoMultiSelect").value(response.data[0].GROUPID) : $("#distGroupSelect").data("kendoMultiSelect").value("");
        }, function errorCallback(response) {
        });
    }
    var productsDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                //url: window.location.protocol + "//" + window.location.host + "/api/Distributor/GetDistributorItems",
                url: window.location.protocol + "//" + window.location.host + "/api/DistributionPurchase/GetAllItems?type=D",
                dataType: "json"
            }
        }
    });

    productsDataSource.fetch(function () {
        $scope.distBrandSelectOptions = {
            dataTextField: "BRAND_NAME",
            dataValueField: "BRAND_NAME",
            height: 600,
            valuePrimitive: true,
            headerTemplate: '<div class="col-md-offset-3"><strong>Brands...</strong></div>',
            placeholder: "Select Brands...",
            autoClose: false,
            dataBound: function (e) {
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

    $scope.distItemsSelectOptions = {
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

    $scope.giftItemsSelectOptions = {
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
        //dataSource: productsDataSource
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/DistributionPurchase/GetAllItems?type=B",
                    dataType: "json"
                }
            }
        },
    };


    function buildFilters(dataItems) {
        var filters = [],
            length = dataItems.length,
            idx = 0, dataItem;
        if (length == 0) {
            $("#distItemsSelect").data("kendoMultiSelect").value("");
        }
        for (; idx < length; idx++) {
            dataItem = dataItems[idx];

            var data = $("#distItemsSelect").data("kendoMultiSelect").dataSource.data();
            var filterdata = _.filter(data, function (da) { return da.BRAND_NAME == dataItem.BRAND_NAME; });
            for (var i = 0; i < filterdata.length; i++) {
                filters.push(filterdata[i].ITEM_CODE);
            }

            $("#distItemsSelect").data("kendoMultiSelect").value(filters);

            //filters.push({
            //    field: "BRAND_NAME",
            //    operator: "eq",
            //    value: parseInt(dataItem.BRAND_NAME)
            //});
        }


    };

    $scope.SchemeOptions = {
        optionLabel: "--Select--",
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: [
            { Text: "Quantity", Value: "QTY" },
            { Text: "Amount", Value: "AMT" },
            { Text: "Gift", Value: "GIFT" },
        ],

    };

    $scope.showOtherOptions = function (option) {
        debugger;

        if (option == "") {
            $scope.showQtyAmtoptions = false;
            $scope.showGiftoption = false;
            $scope.showOptions = false;

        }
        if (option == "QTY") {
            debugger;
            $scope.SchemeTypeName = "Quantity";
            $scope.showQtyAmtoptions = true;
            $scope.showGiftoption = false;
            $scope.showOptions = true;


        }
        if (option == "AMT") {

            $scope.SchemeTypeName = "Amount";
            $scope.showQtyAmtoptions = true;
            $scope.showGiftoption = false;
            $scope.showOptions = true;


        }
        if (option == "GIFT") {
            $scope.SchemeTypeName = "Gift";
            $scope.showQtyAmtoptions = false;
            $scope.showGiftoption = true;
            $scope.showOptions = true;


        }

    };

    $scope.DiscountOptions = {
        optionLabel: "--Select--",
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: [
            { Text: "%", Value: "PERCENT" },
            { Text: "Value", Value: "VALUE" }
        ],

    };

    $scope.schemeReject = function (isValid) {


        $http({
            method: 'POST',
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/ApproveScheme?SchemeId=" + $scope.SchemeID+"&Action=R",
        }).then(function successCallback(response) {
            debugger;
            if (response.data == true) {
                $("#grid").data("kendoGrid").dataSource.read();
                $scope.createPanel = false;
                displayPopupNotification("Scheme approved successfully", "success");
                $scope.showOtherOptions("");

            }
            else {
                displayPopupNotification(response.data, "error");
            }

        }, function errorCallback(response) {
        });

    };

    $scope.schemeEdit = function (isValid) {
        debugger;

        if ($scope.saveAction == "Edit") {
            $scope.saveAction = "Update";
            $scope.approveAction = "Approve and Save";
            $("input").prop('disabled', false);
            $("select").prop('disabled', false);
            $("button").prop('disabled', false);
        }
        else {
            
                schemeInsert();
            
        
        }
    };
    $scope.schemeApprove = function (isValid) {
        debugger;

        var actionname = $scope.approveAction;
        if (actionname == "Approve") {
            $http({
                method: 'POST',
                url: window.location.protocol + "//" + window.location.host + "/api/Setup/ApproveScheme?SchemeId=" + $scope.SchemeID + "&Action=" + "A",
            }).then(function successCallback(response) {
                debugger;
                if (response.data == true) {
                    $scope.createPanel = false;
                    $("#grid").data("kendoGrid").dataSource.read();

                    displayPopupNotification("Scheme approved successfully", "success");
                    $scope.showOtherOptions("");

                }
                else {
                    displayPopupNotification("Something went wrong", "error");
                }

            }, function errorCallback(response) {
            });
        }
        else {
            var schemename = $scope.SchemeName;
            var selectedEmployeeCode = $("#employeeMultiSelect").data("kendoMultiSelect").value()[0];
            var selectedAreaCode = $("#areaMultiSelect").data("kendoMultiSelect").value();
            var selectedCustomer = $("#customerMultiSelect").data("kendoMultiSelect").value();
            var selectedReseller = $("#brdReseller").data("kendoMultiSelect").value();
            var startDate = $scope.startDatePicker;
            var endDate = $scope.endDatePicker;
            var selectedBrands = $("#distBrandSelect").data("kendoMultiSelect").value();
            var selectedItems = $("#distItemsSelect").data("kendoMultiSelect").value();
            if (selectedEmployeeCode == undefined || selectedAreaCode.length == 0 || selectedCustomer.length == 0 || selectedReseller.length == 0 || startDate == undefined || endDate == undefined || selectedBrands.length == 0 || selectedItems.length == 0) {
                displayPopupNotification("Field Empty! ", "warning");
                return;
            }
            var schemeType = $("#SchemeType").data("kendo-drop-down-list").value();
            if (schemeType == undefined) {
                displayPopupNotification("Select Scheme Type ", "warning");
                return;
            }


            if (schemeType == "GIFT") {
                debugger;
                var errorcount = 0;
                var quantityerror = 0;

                angular.forEach($scope.rows, function (row) {
                    debugger;
                    if (row.GiftQty == "" || row.GiftItemCode == null) {
                        errorcount++;
                    }
                    else {
                        var maxqty = parseInt(row.MAX_VALUE);
                        var minqty = parseInt(row.MIN_VALUE);
                        if (maxqty <= minqty) {
                            quantityerror++;

                        }

                    }
                });

                if (errorcount > 0) {
                    displayPopupNotification("Missing field", "warning");
                    return;
                }
                if (quantityerror > 0) {
                    displayPopupNotification("Min Quantity cannot be greater  than or equal to Max Quantity", "warning");
                    return;
                }
                var schemeData = {
                    Action: actionname,
                    SchemeID: $scope.SchemeID,
                    SchemeName: schemename,
                    SP_CODE: selectedEmployeeCode,
                    AreaCode: selectedAreaCode,
                    CustomerCode: selectedCustomer,
                    ResellerCode: selectedReseller,
                    StartDate: startDate,
                    EndDate: endDate,
                    ItemCode: selectedItems,
                    OfferType: schemeType,
                    SchemeDetails: $scope.rows
                };
                $http({
                    method: 'POST',
                    data: schemeData,
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/AddScheme",
                }).then(function successCallback(response) {
                    debugger;
                    if (response.data == "Success") {
                        $("#grid").data("kendoGrid").dataSource.read();
                        $scope.createPanel = false;
                        displayPopupNotification("Scheme saved successfully", "success");
                        $scope.showOtherOptions("");

                    }
                    else {
                        displayPopupNotification(response.data, "error");
                    }

                }, function errorCallback(response) {
                });

            }
            else {
                debugger;
                var errorcount = 0;
                var quantityerror = 0;
                angular.forEach($scope.rows, function (row) {
                    if (row.Discount == "" || row.DiscountType == "") {
                        errorcount++;
                    }
                    else {
                        var maxqty = parseInt(row.MAX_VALUE);
                        var minqty = parseInt(row.MIN_VALUE);
                        if (maxqty <= minqty) {
                            quantityerror++;
                        }

                    }
                });
                if (errorcount > 0) {
                    displayPopupNotification("Missing field", "warning");
                    return;
                }
                if (quantityerror > 0) {
                    displayPopupNotification("Min Quantity cannot be greater  than or equal to Max Quantity", "warning");
                    return;
                }

                var schemeData = {
                    Action: actionname,
                    SchemeID: $scope.SchemeID,
                    SchemeName: schemename,
                    SP_CODE: selectedEmployeeCode,
                    AreaCode: selectedAreaCode,
                    CustomerCode: selectedCustomer,
                    ResellerCode: selectedReseller,
                    StartDate: startDate,
                    EndDate: endDate,
                    ItemCode: selectedItems,
                    OfferType: schemeType,
                    SchemeDetails: $scope.rows
                };
                $http({
                    method: 'POST',
                    data: schemeData,
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/AddScheme",
                }).then(function successCallback(response) {
                    debugger;
                    if (response.data == "Success") {
                        $scope.createPanel = false;
                        $("#grid").data("kendoGrid").dataSource.read();

                        displayPopupNotification("Scheme saved successfully", "success");
                        $scope.showOtherOptions("");

                    }
                    else {
                        displayPopupNotification("Something went wrong", "error");
                    }

                }, function errorCallback(response) {
                });

                
            }
        }

    };
    $scope.grid = {
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetAllScheme",
                    dataType: "json",
                    type: "GET",
                    contentType: "application/json; charset=utf-8"
                },
                parameterMap: function (options, type) {
                    var paramMap = JSON.stringify($.extend(options, ReportFilter.filterAdditionalData()));
                    delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                    delete paramMap.$format; // <-- remove format parameter.
                    return paramMap;
                }
            },
            schema: {
                //data: "data",
                parse: function (data) {
                    debugger;
                    //var result = $filter('filter')(data, { OfferType: "GIFT" });
                    //angular.forEach(result, function (res) {
                    //    $scope.SchemeID.push(res.SchemeID);
                    //});

                    //for (let i = 0; i < data.data.length; i++) {
                    //    data.data[i].Location = (data.data[i].LATITUDE && data.data[i].LONGITUDE) ? (data.data[i].LATITUDE + "," + data.data[i].LONGITUDE) : "Missing"
                    //}
                    //if (data.inActive > 0) {
                    //    Metronic.alert({
                    //        container: $('.content').val(), // alerts parent container
                    //        place: 'append', // append or prepent in container
                    //        type: 'success', // alert's type
                    //        message: data.inActive + " new outlets are waiting for activation.", // alert's message
                    //        close: true, // make alert closable
                    //        reset: true, // close all previouse alerts first
                    //        focus: true, // auto scroll to the alert after shown
                    //        closeInSeconds: 10, // auto close after defined seconds
                    //        icon: 'fa fa-user' // put icon class before the message
                    //    });
                    //}
                    return data;
                },
                model: {
                    fields: {
                        StartDate: { type: "date" },
                        EndDate: { type: "date" },
                    }
                },
            },
            error: function (e) {
                displayPopupNotification("Sorry error occured while processing data", "error");
            },
            //pageSize: reportConfig.defaultPageSize,
        },
        toolbar: kendo.template($("#toolbar-template").html()),
        //excel: {
        //    fileName: "Reseller Setup",
        //    allPages: true,
        //},
        //excelExport: function (e) {
        //    ExportToExcel(e);
        //    e.preventDefault();
        //},
        //pdf: {
        //    fileName: "Received Schedule",
        //    allPages: true,
        //    avoidLinks: true,
        //    pageSize: "auto",
        //    margin: {
        //        top: "2m",
        //        right: "1m",
        //        left: "1m",
        //        buttom: "1m",
        //    },
        //    landscape: true,
        //    repeatHeaders: true,
        //    scale: 0.8,
        //},
        height: window.innerHeight - 50,
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
                SaveReportSetting('ResellerSetup', 'grid');
        },
        columnHide: function (e) {
            if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                SaveReportSetting('ResellerSetup', 'grid');
        },
        pageable: {
            refresh: true,
            //pageSizes: reportConfig.itemPerPage,
            buttonCount: 5
        },
        //scrollable: {
        //    virtual: true
        //},
        pageable: true,
        dataBound: function (o) {
            debugger;
            //GetSetupSetting("ResellerSetup");
            var grid = o.sender;
            if (grid.dataSource.data().length == 0) {
                var colCount = grid.columns.length;
                $(o.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                displayPopupNotification("No Data Found.", "info");
            }
            else {
                var g = $("#grid").data("kendoGrid");
                for (var i = 0; i < g.columns.length; i++) {
                    g.showColumn(i);
                }
                $("div.k-group-indicator").each(function (i, v) {
                    g.hideColumn($(v).data("field"));
                });
            }

            //UpdateReportUsingSetting("ResellerSetup", "grid");
            if (grid.dataSource.data().length == 0) {
                var colCount = grid.columns.length;
                $(o.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                displayPopupNotification("No Data Found.", "info");
            }
            else {
                var g = $("#grid").data("kendoGrid");
                for (var i = 0; i < g.columns.length; i++) {
                    g.showColumn(i);
                }
                $("div.k-group-indicator").each(function (i, v) {
                    g.hideColumn($(v).data("field"));
                });
            }

            //UpdateReportUsingSetting("ResellerSetup", "grid");
            $('div').removeClass('.k-header k-grid-toolbar');
        },
        columns: [{
            field: "SchemeName",
            title: "Scheme Name",
            width: "120px"
        },
        {
            field: "StartDate",
            title: "Start Date",
            format: "{0:MM/dd/yyyy}",
            width: "120px"
        },
        {
            field: "EndDate",
            title: "End Date",
            format: "{0:MM/dd/yyyy}",
            width: "120px"
        },
        {
            field: "Employee_Name",
            title: "Employee Name",
            width: "120px"
        },
        {
            field: "OfferType",
            title: "Scheme Type",
            width: "120px"
            },
            {
                field: "CheckedStatus",
                title: "Checked",
                template: "# if(CheckedStatus == 'Y') {"
                    + "# <span style='color:green;'>Yes</span> #"
                    + "} else if(CheckedStatus == 'N') { "
                    + "# <span style='color:red;'>No</span> #"
                    + "} else { "
                    + "# <span>Pending</span> #"
                    + "} #",
                width: "120px"
            },

            {
                field: "Status",
                title: "Approval Status",
                template: "# if(Status == 'Y') {"
                    + "# <span style='color:green;'>Approved</span> #"
                    + "} else if(Status == 'N') { "
                    + "# <span style='color:red;'>Rejected</span> #"
                    + "} else { "
                    + "# <span>Pending</span> #"
                    + "} #",
                width: "120px"
            },

            {
                title: "Action",
                template: " # if(Status != 'N' && Status != 'Y'){ #<a class='fa fa-edit editAction' ng-click='editScheme($event)' title='Edit'></a>&nbsp &nbsp#}#<a class='fa fa-trash-o deleteAction' ng-click='deleteScheme($event)' title='delete'></a> ",
                width: "8%"
            }
        ]
    };

 

    debugger;

    $scope.schemeItems = function (dataItem) {
        debugger;
        if (dataItem.OfferType == "GIFT") {
            $scope.showOthers = false;
        }
        else {
            $scope.showOthers = true;

        }
        var id = dataItem.SchemeID;
        return {
            dataSource: {
                type: "json",
                transport: {
                    read: {
                        url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetSchemeItem?SchemeID=" + id,
                        dataType: "json",
                        type: "GET",
                        contentType: "application/json; charset=utf-8"
                    },
                },
                schema: {
                    parse: function (data) {
                        debugger;
                        //getgiftItems(id);
                        return data.DATA;
                    }
                },
                error: function (e) {
                    displayPopupNotification("Sorry error occured while processing data", "error");
                },
            },
            scrollable: false,
            sortable: false,
            pageable: false,
            columns: [
                { field: "ITEM_EDESC", title: "Items", width: "100%" },
            ]
        };
    }

    $scope.schemeAreas = function (dataItem) {
        debugger;

        var id = dataItem.SchemeID;
        return {
            dataSource: {
                type: "json",
                transport: {
                    read: {
                        url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetSchemeArea?SchemeID=" + id,
                        dataType: "json",
                        type: "GET",
                        contentType: "application/json; charset=utf-8"
                    },
                },

                error: function (e) {
                    displayPopupNotification("Sorry error occured while processing data", "error");
                },
            },
            scrollable: false,
            sortable: false,
            pageable: false,
            columns: [
                { field: "AREA_NAME", title: "Areas", width: "100%" },
            ]
        };
    };


    $scope.getGiftItem = function (dataItem) {

        debugger;
        var id = dataItem.SchemeID;
        debugger;
        return {
            dataSource: {
                type: "json",
                transport: {
                    read: {
                        url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetGiftItem?SchemeID=" + id,
                        dataType: "json",
                        type: "GET",
                        contentType: "application/json; charset=utf-8"
                    },
                },
                schema: {
                    parse: function (data) {
                        debugger;
                        return data.DATA;
                    }
                },
                error: function (e) {
                    displayPopupNotification("Sorry error occured while processing data", "error");
                },
            },
            scrollable: false,
            sortable: false,
            pageable: false,
            columns: [
                {
                    field: "Min_Value",
                    template: '<input style="width:120px" type="text" value="#= Min_Value #" disabled/>',
                    //title: "Min Quantity",
                    width: "100%"
                },
                {
                    field: "Max_Value",
                    template: '<input style="width:120px" type="text" value="#= Max_Value #" disabled/>',
                    title: "Max Quantity",
                    width: "100%"
                },
                {
                    field: "ItemName",
                    //template:' <input style="width:120px" type="text" value="#= ItemName #"/> ',
                    template: ' <textarea  rows="4" cols="35">#= ItemName #</textarea> ',
                    title: "Items",
                    width: "100%"
                },
                {
                    field: "Gift_QTY",
                    template: '<input style="width:120px" type="text" value="#= Gift_QTY #" disabled/>',
                    title: "Gift Quantity",
                    width: "100%"
                },
            ]
        };
        //getgiftItems(schemeid);
    };


    $scope.others = function (dataItem) {
        debugger;
        var id = dataItem.SchemeID;
        return {
            dataSource: {
                type: "json",
                transport: {
                    read: {
                        url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetOtherItem?SchemeID=" + id,
                        dataType: "json",
                        type: "GET",
                        contentType: "application/json; charset=utf-8"
                    },
                },
                schema: {
                    parse: function (data) {
                        debugger;
                        return data.DATA;
                    }
                },
                error: function (e) {
                    displayPopupNotification("Sorry error occured while processing data", "error");
                },
            },
            scrollable: false,
            sortable: false,
            pageable: false,
            columns: [
                { field: "Min_Value", title: "Min Quantity", width: "100%" },
                { field: "Max_Value", title: "Max Quantity", width: "100%" },
                { field: "Discount", title: "Discount", width: "100%" },
                { field: "DiscountType", title: "Discount Type", width: "100%" },
            ]
        };
    };
    

    debugger;

    $scope.editScheme = function (evt) {
        debugger;
        $scope.createPanel = true;
        $("input").prop('disabled', true);
        $("select").prop('disabled', true);
        $scope.saveAction = "Edit";
        $scope.approveAction = "Approve";
        var grid = $("#grid").data("kendoGrid");
        var row = evt.target.closest("tr");
        var item = grid.dataItem(row);
        $scope.SchemeID = item.SchemeID;
        $scope.SchemeName = item.SchemeName;
        var EmpMultiSelect = $("#employeeMultiSelect").data("kendoMultiSelect");
        var multiSelect = $("#customerMultiSelect").data("kendoMultiSelect");
        var ResellerMultiSelect = $("#brdReseller").data("kendoMultiSelect");
        $("#employeeMultiSelect").data("kendoMultiSelect").value(item.SP_CODE);
        if (item.SP_CODE != null) {


            //area select 
            var selectedArea = _.pluck(_.filter(EmpMultiSelect.dataSource.data(), function (x) {
                return x.EMPLOYEE_CODE == item.SP_CODE;
            }), "AREA_CODE");


            //dist select
            var selectedDistributor = [];
            _.filter(multiSelect.dataSource.data(), function (x) {
                angular.forEach(selectedArea[0], function (code) {

                    if (x.AREA_CODE == code) {
                        selectedDistributor.push(x.DISTRIBUTOR_CODE);

                    };

                });
            });

            //reseller select
            var selectedReseller = [];
            _.filter(ResellerMultiSelect.dataSource.data(), function (x) {
                angular.forEach(selectedArea[0], function (code) {

                    if (x.AREA_CODE == code) {
                        selectedReseller.push(x.RESELLER_CODE);

                    };

                });
            });
                        
            $("#areaMultiSelect").data("kendoMultiSelect").value(selectedArea[0]);
            $("#customerMultiSelect").data("kendoMultiSelect").value(selectedDistributor);
            $("#brdReseller").data("kendoMultiSelect").value(selectedReseller);

        }
        else {
            $scope.customerMultiSelect = null;
            $scope.brdReseller = null;
        }

        var b = $scope.distBrandSelectOptions;
        var a = productsDataSource;
        $scope.startDatePicker = $.datepicker.formatDate('mm/dd/yy', new Date(item.StartDate));
        $scope.endDatePicker = $.datepicker.formatDate('mm/dd/yy', new Date(item.EndDate));

        var id = item.SchemeID;
        var response = $http({
            method: "get",
            url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetSchemeItem?SchemeID=" + id,
            dataType: "json"
        }).then(function (response) {
            debugger;
            var data = response.data.DATA;
            var scitem = _.pluck(data, "ITEM_CODE");
            $("#distItemsSelect").data("kendoMultiSelect").value(scitem);
            var selectedDataSource = $("#distItemsSelect").data("kendoMultiSelect").dataItems();
            var filterBrand = [];
            for (var i = 0; i < selectedDataSource.length; i++) {
                debugger;
                filterBrand.push(selectedDataSource[i].BRAND_NAME);
            }
            $("#distBrandSelect").data("kendoMultiSelect").value(filterBrand);
            $("#SchemeType").data("kendo-drop-down-list").value(item.OfferType);
            $scope.showOtherOptions(item.OfferType);
            $scope.MaxQty = item.Max_Value;
            $scope.MinQty = item.Min_Value;
            if (item.OfferType == "GIFT") {
                var response = $http({
                    method: "get",
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetGiftItem?SchemeID=" + id,
                    dataType: "json"
                }).then(function (response) {
                    debugger;
                    var items = response.data.DATA;
                    $scope.rows = [];
                    angular.forEach(items, function (item) {
                        debugger;
                        var row = {
                            MAX_VALUE: item.Max_Value,
                            MIN_VALUE: item.Min_Value,
                            Discount: '',
                            DiscountType: '',
                            GiftQty: item.Gift_QTY,
                            GiftItemCode: item.ItemCode
                        };
                        $scope.rows.push(row);
                    });

                    //var giftitem = _.pluck(item, "ITEM_CODE");
                    //$("#distGiftItemsSelect").data("kendoMultiSelect").value(giftitem);
                });
            }
            else {
                debugger;
                var response = $http({
                    method: "get",
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetOtherItem?SchemeID=" + id,
                    dataType: "json"
                }).then(function (response) {
                    debugger;
                    var items = response.data.DATA;
                    $scope.rows = [];
                    angular.forEach(items, function (item) {
                        debugger;
                        if (item.DiscountType != "PERCENT") {
                            item.DiscountType = "VALUE";
                        }
                        var row = {
                            MAX_VALUE: item.Max_Value,
                            MIN_VALUE: item.Min_Value,
                            Discount: item.Discount,
                            DiscountType: item.DiscountType,
                            GiftQty: '',
                            GiftItemCode: []
                        };
                        $scope.rows.push(row);
                    });


                });



            }

        });

        debugger;
        $("#distBrandSelect").data("kendoMultiSelect").value([item.BRAND_NAME]);
    };

    

    var schemeInsert = function () {
        debugger;
        
        var actionname = $scope.saveAction;
        var schemename = $scope.SchemeName;
        var selectedEmployeeCode = $("#employeeMultiSelect").data("kendoMultiSelect").value()[0];
        var selectedAreaCode = $("#areaMultiSelect").data("kendoMultiSelect").value();
        var selectedCustomer = $("#customerMultiSelect").data("kendoMultiSelect").value();
        var selectedReseller = $("#brdReseller").data("kendoMultiSelect").value();
        var startDate = $scope.startDatePicker;
        var endDate = $scope.endDatePicker;
        var selectedBrands = $("#distBrandSelect").data("kendoMultiSelect").value();
        var selectedItems = $("#distItemsSelect").data("kendoMultiSelect").value();
        if (selectedEmployeeCode == undefined || selectedAreaCode.length == 0 || selectedCustomer.length == 0 || selectedReseller.length == 0 || startDate == undefined || endDate == undefined || selectedBrands.length == 0 || selectedItems.length == 0) {
            displayPopupNotification("Field Empty! ", "warning");
            return;
        }
        var schemeType = $("#SchemeType").data("kendo-drop-down-list").value();
        if (schemeType == undefined) {
            displayPopupNotification("Select Scheme Type ", "warning");
            return;
        }
        

        if (schemeType == "GIFT") {
            debugger;
            var errorcount = 0;
            var quantityerror = 0;

            angular.forEach($scope.rows, function (row) {
                debugger;
                if (row.GiftQty == "" || row.GiftItemCode == null) {
                    errorcount++;
                }
                else {
                    var maxqty = parseInt(row.MAX_VALUE);
                    var minqty = parseInt(row.MIN_VALUE);
                    if (maxqty <= minqty) {
                        quantityerror++;

                    }

                }
            });

            if (errorcount > 0) {
                displayPopupNotification("Missing field", "warning");
                return;
            }
            if (quantityerror > 0) {
                displayPopupNotification("Min Quantity cannot be greater  than or equal to Max Quantity", "warning");
                return;
            }
            var schemeData = {
                Action: actionname,
                SchemeID: $scope.SchemeID,
                SchemeName: schemename,
                SP_CODE: selectedEmployeeCode,
                AreaCode: selectedAreaCode,
                CustomerCode: selectedCustomer,
                ResellerCode: selectedReseller,
                StartDate: startDate,
                EndDate: endDate,
                ItemCode: selectedItems,
                OfferType: schemeType,
                SchemeDetails: $scope.rows
            };
            $http({
                method: 'POST',
                data: schemeData,
                url: window.location.protocol + "//" + window.location.host + "/api/Setup/AddScheme",
            }).then(function successCallback(response) {
                debugger;
                if (response.data == "Success") {
                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.createPanel = false;
                    displayPopupNotification("Scheme saved successfully", "success");
                    $scope.showOtherOptions("");

                }
                else {
                    displayPopupNotification(response.data, "error");
                }

            }, function errorCallback(response) {
            });

        }
        else {
            debugger;
            var errorcount = 0;
            var quantityerror = 0;
            angular.forEach($scope.rows, function (row) {
                if (row.Discount == "" || row.DiscountType == "") {
                    errorcount++;
                }
                else {
                    var maxqty = parseInt(row.MAX_VALUE);
                    var minqty = parseInt(row.MIN_VALUE);
                    if (maxqty <= minqty) {
                        quantityerror++;
                    }

                }
            });
            if (errorcount > 0) {
                displayPopupNotification("Missing field", "warning");
                return;
            }
            if (quantityerror > 0) {
                displayPopupNotification("Min Quantity cannot be greater  than or equal to Max Quantity", "warning");
                return;
            }
           
            var schemeData = {
                Action: actionname,
                SchemeID: $scope.SchemeID,
                SchemeName: schemename,
                SP_CODE: selectedEmployeeCode,
                AreaCode: selectedAreaCode,
                CustomerCode: selectedCustomer,
                ResellerCode: selectedReseller,
                StartDate: startDate,
                EndDate: endDate,
                ItemCode: selectedItems,
                OfferType: schemeType,
                SchemeDetails: $scope.rows
            };
            $http({
                method: 'POST',
                data: schemeData,
                url: window.location.protocol + "//" + window.location.host + "/api/Setup/AddScheme",
            }).then(function successCallback(response) {
                debugger;
                if (response.data == "Success") {
                    $scope.createPanel = false;
                    $("#grid").data("kendoGrid").dataSource.read();

                    displayPopupNotification("Scheme saved successfully", "success");
                    $scope.showOtherOptions("");

                }
                else {
                    displayPopupNotification("Something went wrong", "error");
                }

            }, function errorCallback(response) {
            });

            //if ($scope.D.Type == undefined) {
            //    displayPopupNotification("Invalid Field", "warning");
            //    return;
            //}
        }

    };

    $scope.clear = function () {
        debugger;
        $scope.SchemeName = "";
        $("#areaMultiSelect").data("kendoMultiSelect").value([]);
        $("#customerMultiSelect").data("kendoMultiSelect").value([]);
        $("#brdReseller").data("kendoMultiSelect").value([]);
        $("#distBrandSelect").data("kendoMultiSelect").value([]);
        $("#distItemsSelect").data("kendoMultiSelect").value([]);
        $("#SchemeType").data("kendo-drop-down-list").value([]);
        $("#distGiftItemsSelect").data("kendoMultiSelect").value([]);
        //$("#DiscountType").data("kendo-drop-down-list").value([]);
        $scope.rows = [{
            MAX_VALUE: '',
            MIN_VALUE: '',
            Discount: '',
            DiscountType: '',
            GiftQty: '',
            GiftItemCode: []
        }];
        $scope.startDatePicker = "";
        $scope.endDatePicker = "";

    };

    $scope.deleteScheme = function (evt) {
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

                    var schemeid = item.SchemeID;

                    $http({
                        method: "get",
                        url: window.location.protocol + "//" + window.location.host + "/api/Setup/DeleteScheme?SchemeID=" + schemeid,
                        dataType: "json"
                    }).then(function (response) {
                        debugger;
                        if (response.status == 200) {
                            displayPopupNotification(response.data.MESSAGE, "Success");
                            $("#grid").data("kendoGrid").dataSource.read();
                            $scope.clear();
                        }
                        else {
                            displayPopupNotification(response.data.MESSAGE, "error");
                        }

                    });
                }

            }
        });
    };
});