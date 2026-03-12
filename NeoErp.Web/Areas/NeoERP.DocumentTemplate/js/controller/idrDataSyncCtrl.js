DTModule.controller('idrDataSyncCtrl', function ($scope, $http, $q, $timeout) {

    $scope.Page = "IRD Data Sync";
    $scope.startedMessage = "";
    $scope.logFromDate = "";
    $scope.logToDate = "";
    $scope.logVoucherNo = "";
    $scope.mainGridOptionsIRDList = {
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetIRDNotSyncDataList",
                    dataType: "json",
                    type: "GET",
                    contentType: "application/json; charset=utf-8"

                },
                parameterMap: function (options, type) {
                    if (type === "read") {
                        return options;
                    }
                }
            },
            schema: {
                model: {
                    fields: {
                        IS_SELECTED: { type: "boolean" },
                        Form_CODE: { type: "string" },
                        VOUCHER_NO: { type: "string" },
                        VOUCHER_DATE: { type: "date" },
                        VOUCHER_AMOUNT: { type: "number" },
                        REFERENCE_NO: { type: "string" },
                        CREATED_BY: { type: "string" },
                        CREATED_DATE: { type: "date" },
                        MODIFY_DATE: { type: "date" },
                        CHECKED_BY: { type: "string" },
                        CHECKED_DATE: { type: "date" },
                        AUTHORISED_BY: { type: "string" },
                        POSTED_DATE: { type: "date" }
                    }
                }
            },
            pageSize: 10,
            serverPaging: false,
            serverFiltering: false,
            serverSorting: false
        },
        toolbar: kendo.template($("#toolbar-template").html()),
        //height: window.innerHeight - 50,
        sortable: true,
        reorderable: true,
        groupable: false,
        resizable: true,
        filterable: {
            extra: false,
            operators: {
                string: {
                    contains: "Contains",
                    startswith: "Starts with",
                    eq: "Is equal to",
                    neq: "Is not equal to"
                }
            }
        },
        pageable: {
            refresh: true,
            pageSizes: [5, 10],
            buttonCount: 5
        },
        columns: [
            {
                template: "<input type='checkbox' class='checkbox' ng-model='dataItem.IS_SELECTED' ng-change='onRowSelect(dataItem)' />",
                // headerTemplate: "<input type='checkbox' id='selectAllCheckbox' ng-model='state.selectAll' ng-change='toggleAll()' />",
                width: "30px"
            },
            {
                hidden: true,
                field: "Form_CODE",
            },
            {
                field: "VOUCHER_NO",
                title: "Document No.",
                width: "120px"
            },
            {
                field: "VOUCHER_DATE",
                title: "Date",
                template: "#= kendo.toString(kendo.parseDate(VOUCHER_DATE),'dd MMM yyyy') #",
                width: "100px"
            },
            {
                field: "VOUCHER_AMOUNT",
                title: "Amount",
                attributes: { style: "text-align:right;" },
                format: "{0:n2}",
                width: "100px"
            },
            //{
            //    field: "REFERENCE_NO",
            //    title: "Manual No.",
            //    width: "100px"
            //},
            {
                field: "CREATED_BY",
                title: "Prepared By",
                width: "100px"
            },
            {
                field: "CREATED_DATE",
                title: "Prepared Date & Time",
                template: "#= kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy hh:mm:ss') #",
                width: "150px"
            },
            {
                field: "MODIFY_DATE",
                title: "Modified Date",
                template: "#= kendo.toString(kendo.parseDate(MODIFY_DATE),'dd MMM yyyy') #",
                width: "100px"
            },
            //{
            //    title: "Bill Create",
            //    template: "<div style='text-align:center'><i class='fa fa-check-square-o'></i></div>",
            //    width: "80px"
            //}
        ],
        columnMenu: true,
        dataBound: function () {
            var grid = this;
            $timeout(function () {
                var data = grid.dataSource.view();
                var allSelected = true;
                if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        if (!data[i].IS_SELECTED) {
                            allSelected = false;
                            break;
                        }
                    }
                } else {
                    allSelected = false;
                }
                $scope.state.selectAll = allSelected;
            });
        }
    };

    $scope.state = { selectAll: false };

    $scope.toggleAll = function () {
        var grid = $("#kGridIRDList").data("kendoGrid");
        var data = grid.dataSource.view();
        angular.forEach(data, function (item) {
            item.IS_SELECTED = $scope.state.selectAll;
        });
    };

    $scope.onRowSelect = function (dataItem) {
        var grid = $("#kGridIRDList").data("kendoGrid");
        var data = grid.dataSource.view();
        if (!dataItem.IS_SELECTED) {
            $scope.state.selectAll = false;
        } else {
            var allSelected = true;
            for (var i = 0; i < data.length; i++) {
                if (!data[i].IS_SELECTED) {
                    allSelected = false;
                    break;
                }
            }
            $scope.state.selectAll = allSelected;
        }
    };

    $scope.SyncData = function () {
        debugger;
        var selectedItems = [];
        var grid = $("#kGridIRDList").data("kendoGrid");
        var data = grid.dataSource.data();

        angular.forEach(data, function (item) {
            if (item.IS_SELECTED) {
                selectedItems.push(item);
            }
        });

        if (selectedItems.length === 0) {
            displayPopupNotification("Please select at least one item to sync.", "warning");
            return;
        }

        //$http.post(window.location.protocol + "//" + window.location.host + "/api/TemplateApi/SyncIRDData", selectedItems).then(function (response) {
        //    displayPopupNotification("Synced Successfully", "success");
        //    //$("#kGridIRDList").data("kendoGrid").dataSource.data();
        //    //$scope.selectAll = false;
        //}, function (error) {
        //    displayPopupNotification("Error: " + error.data.Message, "error");
        //});

        $http.post(window.location.protocol + "//" + window.location.host + "/api/TemplateApi/SyncIRDData", selectedItems)
            .then(function (response) {
                $scope.startedMessage = "Syncing started...\nPlease refresh list to verify";
                displayPopupNotification("Syncing started.Please check later", "success");
                // $scope.startedMessage = "Syncing started...Please Refresh List to verify";
                // $scope.startedMessage = "Syncing started...\nPlease refresh list to verify";



                $timeout(function () {
                    var grid = $("#kGridIRDList").data("kendoGrid");
                    grid.dataSource.read();
                    grid.refresh();
                    //$scope.startedMessage = "";
                }, 3000);



                $scope.state.selectAll = false;
            }, function (error) {

            });

    };

    $scope.RefreshList = function () {
        var grid = $("#kGridIRDList").data("kendoGrid");
        grid.dataSource.read();
        grid.refresh();
        $scope.startedMessage = "";
    };


    $scope.FilterLogData = function () {
        var grid = $("#kGridLogList").data("kendoGrid");
        if (grid) {
            grid.dataSource.read();
        }
    };

    $scope.ResetLogData = function () {
        $scope.logFromDate = "";
        $scope.logToDate = "";
        $scope.logVoucherNo = "";
        var grid = $("#kGridLogList").data("kendoGrid");
        if (grid) {
            grid.dataSource.read();
        }
    };

    $scope.ShowLogList = function () {
        $("#logModal").modal("show");
        var grid = $("#kGridLogList").data("kendoGrid");
        if (grid) {
            grid.dataSource.read();
        }
    };

    $scope.copyToClipboard = function (item) {







        var text = "";
        if (typeof item === 'object' && item !== null && item.REQUEST_JSON) {
            text = item.REQUEST_JSON;
        } else if (typeof item === 'string') {
            text = item;
        }

        if (!text) {
            displayPopupNotification("No content to copy.", "warning");
            return;
        }


        navigator.clipboard.writeText(text)
            .then(function () {
                //alert("Copied!");
                displayPopupNotification("Request JSON Copied to clipboard", "success");
            })
            .catch(function (err) {
                console.log("Error copying", err);
            });




        //var textarea = document.createElement("textarea");
        //textarea.value = text;
        //textarea.style.position = "fixed"; // Prevent scrolling to bottom of page in MS Edge.
        //document.body.appendChild(textarea);
        //textarea.select();
        //try {
        //    var successful = document.execCommand("copy");
        //    if (successful) {
        //        displayPopupNotification("Request JSON Copied to clipboard", "success");
        //    } else {
        //        displayPopupNotification("Failed to copy", "error");
        //    }
        //} catch (ex) {
        //    console.warn("Copy to clipboard failed.", ex);
        //    displayPopupNotification("Failed to copy", "error");
        //} finally {
        //    document.body.removeChild(textarea);
        //}
    };

    $scope.logGridOptions = {
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetIRDDataSyncLogList",
                    dataType: "json",
                    type: "GET",
                    contentType: "application/json; charset=utf-8",

                },
                parameterMap: function (options, type) {
                    if (type === "read") {
                        var fromDate = $scope.logFromDate ? kendo.toString($scope.logFromDate, "yyyy-MM-dd") : "";
                        var toDate = $scope.logToDate ? kendo.toString($scope.logToDate, "yyyy-MM-dd") : "";
                        var voucherNo = $scope.logVoucherNo;
                        return {
                            fromDate: fromDate,
                            toDate: toDate,
                            voucherNo: voucherNo,
                            skip: options.skip,
                            take: options.take,
                            page: options.page,
                            pageSize: options.pageSize
                        };
                    }
                }
            },
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            schema: {
                data: "data",
                total: "total",
                model: {
                    fields: {
                        VOUCHER_NO: { type: "string" },
                        FORM_CODE: { type: "string" },
                        MESSAGE: { type: "string" },
                        CREATED_DATE: { type: "date" },
                        AMOUNT: { type: "string" },
                        VAT: { type: "string" },
                        TAX_AMOUNT: { type: "string" },
                        REQUEST_JSON: { type: "string" },
                        RESPONSE_JSON: { type: "string" }
                    }
                }
            },
            pageSize: 10,
        },
        height: 350,
        scrollable: true,
        sortable: true,
        pageable: {
            refresh: true,
            pageSizes: true,
            buttonCount: 5
        },
        columns: [
            {
                field: "CREATED_DATE",
                title: "Date",
                template: "#= kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy hh:mm:ss') #",
                width: "150px"
            },
            {
                field: "VOUCHER_NO",
                title: "Voucher No",
                width: "120px"
            },
            {
                field: "FORM_CODE",
                title: "Form Code",
                width: "100px"
            },
            {
                field: "MESSAGE",
                title: "Message",
                width: "300px"
            },
            {
                field: "AMOUNT",
                title: "Amount",
                width: "100px"
            },
            {
                field: "VAT",
                title: "Vat",
                width: "100px"
            },
            {
                field: "TAX_AMOUNT",
                title: "Tax Amount",
                width: "100px"
            },
            {
                field: "REQUEST_JSON",
                title: "Request",
                width: "300px",
                template: "<a class='btn btn-xs default' ng-click='copyToClipboard(dataItem)' title='Copy Content'><i class='fa fa-copy'></i></a> <span>#= REQUEST_JSON #</span>"
            },
            {
                field: "RESPONSE_JSON",
                title: "Response",
                width: "300px"
            }
        ]
    };

    $scope.applySearch = function () {
        var grid = $("#kGridIRDList").data("kendoGrid");
        if ($scope.searchText) {
            grid.dataSource.filter({
                field: "VOUCHER_NO",
                operator: "contains",
                value: $scope.searchText
            });
        } else {
            grid.dataSource.filter({});
        }
    };

    function displayPopupNotification(message, type) {
        // Assuming global notification function or standard alert if not present
        if (window.displayPopupNotification) {
            window.displayPopupNotification(message, type);
        } else {
            alert(message);
        }
    }

});
