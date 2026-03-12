distributionModule.controller("CollectionCtrl", function ($scope, $filter, $timeout, crudAJService) {
    

    $scope.Collection = {
        DIVISIONS: [{
            DIVISION_CODE: "",
            AMOUNT: ""
        }]
    };
    $scope.ShowForm = false;
    $scope.IsUpdate = false;

    $scope.ViewForm = function () {
        $scope.ShowForm = true;
    }

    $scope.SalesPersonSelectOptions = {
        dataTextField: "EMPLOYEE_EDESC",
        dataValueField: "SP_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Sales persons...</strong></div>',
        placeholder: "Sales Person...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/UserSetup/GetSalePerson",
                    dataType: "json"
                }
            },
            schema: {
                parse: function (data) {
                    var SelfItem = { SP_CODE: '0', EMPLOYEE_EDESC: "Self" };
                    data.unshift(SelfItem);
                    return data;
                }
            }
        }
    };

    $scope.DivisionSelectOptions = {
        dataTextField: "DIVISION_EDESC",
        dataValueField: "DIVISION_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Select Division...</strong></div>',
        placeholder: "Division...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        select: function (e) {
            var selected = e.dataItem;
            var PreviouslySelected = [];
            for (var i = 0; i < $scope.Collection.DIVISIONS.length - 1; i++) {
                var val = $scope.Collection.DIVISIONS[i].DIVISION_CODE[0];
                PreviouslySelected.push(val);
            }
            //var PreviouslySelected = $scope.Collection.DIVISIONS.map(function (a) { return a.DIVISION_CODE; });
            if (PreviouslySelected.includes(selected.DIVISION_CODE)) {
                e.preventDefault();
                displayPopupNotification("Item already selected", "warning");
            }
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Distributor/GetDivisions",
                    dataType: "json"
                }
            }
        }
    };

    $scope.PaymentSelectOptions = {
        dataTextField: "ModeName",
        dataValueField: "ModeId",
        height: 50,
        valuePrimitive: true,
        autoBind: false,
        maxSelectedItems: 1,
        placeholder: "Payment Mode...",
        autoClose: false,
        dataSource: [
            { ModeName: "Cash", ModeId: "CASH" },
            { ModeName: "Cheque", ModeId: "CHEQUE" },
        ],
    };

    $scope.mainGridOptions = {
        dataSource: {
            type: "JSON",
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Distributor/GetDistributorCollections", // <-- Get data from here.
                    dataType: "json", // <-- The default was "jsonp".
                    type: "POST",
                    contentType: "application/json; charset=utf-8"
                },
                parameterMap: function (options, type) {
                    var paramMap = JSON.stringify($.extend(options, ReportFilter.filterAdditionalData()));
                    delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                    delete paramMap.$format; // <-- remove format parameter.
                    return paramMap;
                }
            },
            pageSize: 100,
            schema: {
                model: {
                    fields: {
                        CHEQUE_CLEARANCE_DATE: { type: "date" },
                    }
                }
            },
        },
        toolbar: kendo.template($("#toolbar-template").html()),
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
        scrollable: {
            virtual: true
        },
        dataBound: function (o) {
            var grid = this;
            grid.tbody.find("tr").dblclick(function (e) {
                e.preventDefault();
                var item = grid.dataItem(this);
                if (item) {
                    var divisions = [];
                    //get details
                    crudAJService.GetCollectionDetail(item.BILL_NO).then(function (result) {
                        for (let i = 0; i < result.data.length; i++) {
                            divisions.push({
                                AMOUNT: result.data[i].AMOUNT,
                                DIVISION_CODE: [result.data[i].DIVISION_CODE]
                            });
                        }

                        $scope.ShowForm = true;
                        $scope.IsUpdate = true;
                        $scope.PaymentModel = [item.PAYMENT_MODE];
                        $scope.Collection = {
                            SP_CODE: [item.SP_CODE],
                            PAYMENT_MODE: [item.PAYMENT_MODE],
                            AMOUNT: item.AMOUNT,
                            BILL_NO: item.BILL_NO,
                            CHEQUE_NO: item.CHEQUE_NO,
                            CHEQUE_DEPOSIT_BANK: item.CHEQUE_DEPOSIT_BANK,
                            BANK_NAME: item.BANK_NAME,
                            CHEQUE_CLEARANCE_DATE: item.CHEQUE_CLEARANCE_DATE == null ? null : $filter('date')(new Date(item.CHEQUE_CLEARANCE_DATE), 'dd/MM/yyyy'),
                            REMARKS: item.REMARKS,
                            DIVISIONS: divisions
                        };
                    }, function (ex) {
                        displayPopupNotification("Cannot retrieve details", "error");
                    });
                }
            });

            $('div').removeClass('.k-header k-grid-toolbar');
        },
        columns: [{
            field: "SALESPERSON_NAME",
            title: "Salesperson",
        }, {
            field: "ENTITY_NAME",
            title: "Distributor",
        }, {
            field: "PAYMENT_MODE",
            title: "Payment Mode"
        }, {
            field: "AMOUNT",
            title: "Amount",
            attributes: {
                style: "text-align: right;"
            },
        }, {
            field: "BILL_NO",
            title: "Bill Number",
        }, {
            field: "CHEQUE_NO",
            title: "Cheque Number",
        }, {
            field: "BANK_NAME",
            title: "Bank Name",
        }, {
            field: "CHEQUE_DEPOSIT_BANK",
            title: "Cheque Deposit Bank",
        }, {
            field: "CHEQUE_CLEARANCE_DATE",
            title: "Cheque clearence date",
            format: "{0:dd-MMM-yyyy}",
        }, {
            field: "REMARKS",
            title: "Remarks",
        }]
    };

    $scope.ResetForm = function () {
        $scope.PaymentModel = '';
        $scope.Collection = {
            DIVISIONS: [{
                DIVISION_CODE: "",
                AMOUNT: ""
            }]
        };
        $scope.ShowForm = false;
        $scope.IsUpdate = false;
    }

    $scope.SaveCollection = function () {
        var totalDivAmt = 0;
        //if ($scope.IsUpdate) {
        //    var rows = $("#formTable tbody").find("tr"); //all rows in form table

        //    //iterate through each table row, find the multiselect element and find the selected item
        //    //because angular model binding does not work in kendo MultiSelect
        //    for (let i = 0; i < rows.length; i++) {
        //        multiselect = $($(rows[i]).find(".divMultiselect")[1]); //getting the multiselect element
        //        var data = multiselect.data("kendoMultiSelect").value(); //getting the selected value
        //        $scope.Collection.DIVISIONS[i].DIVISION_CODE = data; //setting it to angularmodel
        //    }
        //}
        for (let i = 0; i < $scope.Collection.DIVISIONS.length; i++) {
            if (!$scope.Collection.DIVISIONS[i].AMOUNT || !$scope.Collection.DIVISIONS[i].DIVISION_CODE) {
                displayPopupNotification("Invalid input fields", "warning");
                return;
            }
            var amt = parseFloat($scope.Collection.DIVISIONS[i].AMOUNT);
            totalDivAmt += amt;
        }
        if (totalDivAmt != $scope.Collection.AMOUNT) {
            displayPopupNotification("Total amount do not match", "warning");
            return;
        }

        if ($scope.CollectionForm.$valid) {
            var a = $.extend(true, {}, $scope.Collection);
            a.SP_CODE = a.SP_CODE = $scope.Collection.SP_CODE.length == 1 ? $scope.Collection.SP_CODE[0] : $scope.Collection.SP_CODE;;
            a.PAYMENT_MODE = $scope.Collection.PAYMENT_MODE.length == 1 ? $scope.Collection.PAYMENT_MODE[0] : $scope.Collection.PAYMENT_MODE;
            for (let i = 0; i < $scope.Collection.DIVISIONS.length; i++) {
                a.DIVISIONS[i].DIVISION_CODE = $scope.Collection.DIVISIONS[i].DIVISION_CODE.length == 1 ? $scope.Collection.DIVISIONS[i].DIVISION_CODE[0] : $scope.Collection.DIVISIONS[i].DIVISION_CODE;
            }
            a.SAVE_FLAG = $scope.IsUpdate == false ? "S" : "U";
            crudAJService.SaveCollection(a).then(function (result) {
                displayPopupNotification(result.MESSAGE, result.TYPE);
                $scope.ResetForm();
                $("#grid").data("kendoGrid").dataSource.read();
            }, function (ex) {
                displayPopupNotification("Something went wrong. Please try again.", "error");
            });
        }
        else
            displayPopupNotification("Invalid input fields", "warning");
    }

    $scope.AddDivision = function () {
        var total = $scope.Collection.AMOUNT != '' ? parseFloat($scope.Collection.AMOUNT) : 0;
        var divisionTotal = 0, amt = 0;
        for (var i = 0; i < $scope.Collection.DIVISIONS.length; i++) {
            if ($scope.Collection.DIVISIONS[i].DIVISION_CODE == "" || $scope.Collection.DIVISIONS[i].AMOUNT == "")
                return;
            divisionTotal += parseFloat($scope.Collection.DIVISIONS[i].AMOUNT);
        }
        var amt = total - divisionTotal;
        $scope.Collection.DIVISIONS.push({
            DIVISION_CODE: "",
            AMOUNT: amt,
        });
    }

    $scope.RemoveDivision = function (index) {
        $scope.Collection.DIVISIONS.splice(index, 1);
    }

});