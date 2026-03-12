DTModule.controller('GateEntryCtrl', function ($scope, $filter, $window, $timeout, $http, $q) {
    $scope.FormName = "Gate Entry";
    $scope.FROM_DATE = "";
    $scope.TO_DATE = "";
    $scope.PRODUCT_FILTER = "";

    $scope.ConvertEngToNep = function () {
        console.log(this);

        var engdate = $("#englishDate5").val();
        var nepalidate = ConvertEngDateToNep(engdate);
        $("#nepaliDate5").val(nepalidate);
        $("#nepaliDate51").val(nepalidate);
    };

    $scope.ConvertNepToEng = function ($event) {

        //$event.stopPropagation();
        console.log($(this));
        var date = BS2AD($("#nepaliDate5").val());
        var date1 = BS2AD($("#nepaliDate51").val());
        $("#englishdatedocument").val($filter('date')(date, "dd-MMM-yyyy"));
        $("#englishdatedocument1").val($filter('date')(date1, "dd-MMM-yyyy"));
        $('#nepaliDate5').trigger('change')
        $('#nepaliDate51').trigger('change')
    };

    $scope.ConvertEngToNepang = function (data) {
        $("#nepaliDate5").val(AD2BS(data));
    };
    $scope.ConvertEngToNepang1 = function (data) {

        $("#nepaliDate51").val(AD2BS(data));
    }
    $scope.ConvertGateEngToNepang = function (data) {
        $("#gateNepaliDate").val(AD2BS(data))
    }
    $scope.ConvertBillEngToNepang = function (data) {
        $("#billNepaliDate").val(AD2BS(data))
    }

    $scope.someDateFn = function () {

        var engdate = $filter('date')(new Date(new Date().setDate(new Date().getDate() - 1)), 'dd-MMM-yyyy');
        //var engdate1 = $filter('date')(new Date(new Date().setDate(new Date().getDate() - 2)), 'dd-MMM-yyyy');
        var a = ConvertEngDateToNep(engdate);
        //var a1 = ConvertEngDateToNep(engdate1);
        $scope.Dispatch_From = engdate;
        $scope.NepaliDate = a;
        $scope.Dispatch_To = a;
        $scope.PlanningTo = ConvertEngDateToNep($filter('date')(new Date(new Date().setDate(new Date().getDate())), 'dd-MMM-yyyy'));
        //  $scope.PlanningDate = a;

    };

    $scope.gateEntry = {};
    $scope.vehicleList = [];
    $scope.transporterList = [];
    $scope.referenceList = [];
    $scope.selectedVehicle = null;
    $scope.selectedTransporter = null;
    $scope.selectedReference = null;
    $scope.vehicleTypeList = [];
    $scope.selectedVehicleType = null;
    $scope.locationList = [];
    $scope.selectedLocation = null;
    $scope.referenceItems = [];

    function getCurrentTimeAsDate() {
        var now = new Date();
        return new Date(1970, 0, 1, now.getHours(), now.getMinutes());
    }


    $scope.clearForm = function () {
        $scope.gateEntry = {
            GATE_NO: '',
            GATE_DATE: new Date(),
            MANUAL_NO: '',
            VEHICLE_NAME: '',
            IN_TIME: '',
            OUT_TIME: '',
            BILL_NO: '',
            BILL_DATE: new Date(),
            TRANSPORT_NAME: '',
            LOCATION_CODE: '',
            SUPPLIER_CODE: '',
            GROSS_WT: null,
            TEAR_WT: null,
            NET_WT: null,
            DRIVER_NAME: '',
            PERSON: '',
            RECEIVED_BY: '',
            REMARKS: '',
            GATE_IN_FLAG: false,
            GATE_IN_BY: '',
            INWARD_TYPE: '',
            REFERENCE_NO: '',
            VEHICLE_CODE: '',
            PARTY_WEIGHT: null,
            NO_OF_PACKET: null,
            VEHICLE_TYPE: '',
            TRANSPORT_CODE: '',
            BILL_RATE: '',
            FORM_CODE: ''
        };
        $scope.saveupdatebtn = "Save";
        $scope.editFlag = "N";

        $scope.gateEntry.IN_TIME = getCurrentTimeAsDate();

        // Clear selections
        $scope.selectedVehicle = null;
        $scope.selectedTransporter = null;
        $scope.selectedReference = null;
        $scope.selectedVehicleType = null;
        $scope.selectedLocation = null;
        $scope.referenceItems = [];

        // Clear grid
        var grid = $("#inwardGrid").data("kendoGrid");
        if (grid) {
            grid.dataSource.data([]);
            grid.refresh();
        }
        $scope.selectedReferences = [];
    };

    $scope.calculateNetWeight = function () {
        var gross = parseFloat($scope.gateEntry.GROSS_WT || 0);
        var tare = parseFloat($scope.gateEntry.TEAR_WT || 0);
        var net = gross - tare;
        $scope.gateEntry.NET_WT = isNaN(net) ? null : net;
    };

    $scope.someDateFn();
    $scope.monthSelectorOptionsSingle = {
        value: new Date(),
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {

            $scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",


        dateInput: true
    };

    $scope.monthSelectorOptionsSingle1 = {
        open: function () {

            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {

            $scope.ConvertEngToNepang1(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",


        dateInput: true
    };

    $scope.monthGateSelectorOptionsSingle = {
        open: function () {

            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {

            $scope.ConvertGateEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",


        dateInput: true
    };

    $scope.monthBillSelectorOptionsSingle = {
        open: function () {

            var calendar = this.dateView.calendar;

            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {

            $scope.ConvertBillEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",


        dateInput: true
    };


    $scope.BindGateEntryGrid = function () {
        var grid = $("#gateEntryGrid").data("kendoGrid");

        if (grid) {
            grid.dataSource.read();
        }
    };

    $timeout(function () {
        let startDate, endDate;
        let today = moment();
        endDate = today.toDate();
        startDate = moment().subtract(1, 'days').toDate();
        $("#englishdatedocument").data("kendoDatePicker").value(startDate);
        $("#englishdatedocument1").data("kendoDatePicker").value(endDate);

        let nepaliFromDate = ConvertEngDateToNep(moment(startDate).format('DD-MMM-YYYY'));
        let nepaliToDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));

        $("#nepaliDate5").val(nepaliFromDate);
        $("#nepaliDate51").val(nepaliToDate);
        $scope.BindGateEntryGrid();

        $scope.initializeInwardGrid();
    }, 200)

    $scope.initializeInwardGrid = function () {
        $("#inwardGrid").kendoGrid({
            dataSource: {
                data: []
            },
            height: 180,
            scrollable: true,
            sortable: true,
            autobind: false,
            columns: [
                { field: "SERIAL_NO", title: "S.No", width: "60px" },
                { field: "ITEM_EDESC", title: "Product Description" },
                { field: "MU_CODE", title: "Unit", width: "150px" },
                {
                    field: "BILL_QTY",
                    title: "Quantity",
                    width: "250px",
                    template: function (dataItem) {
                        debugger
                        return '<input type="number" class="k-input k-textbox grid-qty" style="width:100%" value="' + dataItem.BILL_QTY + '" data-uid="' + dataItem.uid + '" />';
                    }
                },
                { field: "AVAILABLE_QTY", title: "Available Qty", width: "250px", hidden: true }
            ],
            dataBound: function () {
                var grid = this;
                var dataSource = grid.dataSource;

                grid.tbody.find(".grid-qty").off('change').on('change', function () {
                    var input = $(this);
                    var uid = input.data("uid");
                    var model = dataSource.getByUid(uid);

                    var newQty = parseFloat(input.val()) || 0;

                    if (newQty > (model.AVAILABLE_QTY || 0)) {
                        displayPopupNotification('Quantity cannot exceed available quantity: ' + model.AVAILABLE_QTY, 'info');
                        newQty = model.AVAILABLE_QTY;
                        input.val(newQty);
                    }

                    model.set("BILL_QTY", newQty);
                });
            }
        });
    }

    $scope.GateEntryGridOptions = {
        dataSource: new kendo.data.DataSource({
            transport: {
                read: {
                    url: "/api/SetupApi/GetGateEntryList",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json"
                },
                parameterMap: function (options, type) {
                    if (type === "read") {
                        let fromDate = $(".fromDate input").val();
                        let toDate = $(".toDate input").val();
                        let productFilter = $(".productFilter").val();

                        var combinedOptions = angular.extend({}, options, {
                            FROM_DATE: $scope.toDate(fromDate),
                            TO_DATE: $scope.toDate(toDate),
                            PRODUCT_FILTER: productFilter
                        });

                        return JSON.stringify(combinedOptions);
                    }
                    return options;
                }
            },
            autoBind: false,
            schema: {
                data: "DATA",
            },
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            // Disable server grouping to allow client-side grouping
            serverGrouping: false,
            pageSize: 10,
            sort: {
                field: "GATE_NO",
                dir: "desc"
            },
            // Add the group configuration here
            group: {
                field: "REFERENCE_NO",
                dir: "asc" // or "desc"
            }
        }),
        autoBind: false,
        selectable: "single",
        scrollable: true,
        height: 350,
        sortable: true,
        pageable: true,
        groupable: true,
        resizable: true,
        dataBound: function (e) {
            $("#gateEntryGrid tbody tr").css("cursor", "pointer");
            var grid = this;
            grid.tbody.find('tr').off('dblclick').on('dblclick', function () {
                var dataItem = grid.dataItem(this);
                if (dataItem && dataItem.GATE_NO) {
                    $scope.editGateEntry(dataItem.GATE_NO);
                }
            });
            // bind action buttons
            grid.tbody.find('.btn-ge-edit').off('click').on('click', function (ev) {
                var gateNo = $(this).data('id');
                $scope.editGateEntry(gateNo);
            });
            grid.tbody.find('.btn-ge-delete').off('click').on('click', function (ev) {
                var gateNo = $(this).data('id');
                $scope.deleteGateEntry(gateNo);
            });
        },
        columns: [
            {
                field: "REFERENCE_NO",
                title: "Ref. Voucher No",
                width: "120px",
                hidden: true, // You can hide the column if you only want to group by it
                groupHeaderTemplate: "Reference No: #= kendo.htmlEncode(value) #"
            },
            {
                field: "GATE_NO",
                title: "Gate No",
                width: "80px"
            },
            {
                field: "GATE_MITI",
                title: "Gate Miti",
                width: "100px",
            },
            {
                field: "BILL_NO",
                title: "Bill No",
                width: "100px"
            },
            {
                field: "GATE_MITI",
                title: "Bill Miti",
                width: "100px",
            },
            {
                field: "SUPPLIER_EDESC",
                title: "Party Name",
                width: "150px"
            },
            {
                field: "ITEM_EDESC",
                title: "Item Name",
                width: "150px"
            },
            {
                field: "Detail_BILL_QTY",
                title: "Quantity",
                width: "100px"
            },
            {
                field: "LOCATION_EDESC",
                title: "Location",
                width: "120px"
            },
            {
                field: "TRANSPORT_NAME",
                title: "Transport Name",
                width: "150px"
            },
            {
                field: "VEHICLE_NAME",
                title: "Vehicle No",
                width: "120px"
            },
            {
                field: "IN_TIME",
                title: "In Time",
                width: "120px"
            },
            {
                field: "OUT_TIME",
                title: "Out Time",
                width: "120px"
            },
            {
                field: "GROSS_WT",
                title: "Gross Wt",
                width: "120px"
            },
            {
                field: "TEAR_WT",
                title: "Tear Wt",
                width: "120px"
            },
            {
                field: "NET_WT",
                title: "Net Wt",
                width: "120px"
            },
            {
                field: "RECEIVED_BY",
                title: "Received By",
                width: "120px"
            },
            {
                field: "REMARKS",
                title: "REMARKS",
                width: "120px"
            },
            {
                field: "STATUS",
                title: "STATUS",
                width: "120px",
                template: function (dataItem) {
                    if (dataItem.GATE_IN_BY) {
                        return 'Gate In';
                    } else {
                        return '';
                    }
                }
            },
            {
                field: "GATE_IN_BY",
                title: "Proceed By",
                width: "120px"
            },
            {
                field: "REFERENCE_NO",
                title: "Ref. Voucher No",
                width: "120px"
            },
            {
                title: "Actions",
                width: "140px",
                template: function (dataItem) {
                    var id = kendo.htmlEncode(dataItem.GATE_NO || '');
                    return '<button class="btn btn-xs btn-primary btn-ge-edit" data-id="' + id + '"><i class="fa fa-pencil"></i> Edit</button> ' +
                        '<button class="btn btn-xs btn-danger btn-ge-delete" data-id="' + id + '"><i class="fa fa-trash"></i> Delete</button>';
                }
            }
        ]
    };

    $scope.initializeDates = function () {
        let today = moment();
        endDate = today.toDate();

        $scope.gateEntry.GATE_DATE = moment(endDate).format("DD-MMM-YYYY");
        $scope.gateEntry.BILL_DATE = moment(endDate).format("DD-MMM-YYYY");
        $("#gateEnglishDate").data("kendoDatePicker").value(moment(endDate).format("DD-MMM-YYYY"));
        $("#billEnglishDate").data("kendoDatePicker").value(moment(endDate).format("DD-MMM-YYYY"));

        let nepaliToDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));

        $("#gateNepaliDate").val(nepaliToDate);
        $("#billNepaliDate").val(nepaliToDate);
    }

    $scope.showModalForNew = function (event) {
        $scope.selectedReferences = [];
        $scope.saveupdatebtn = "Save"
        $scope.editFlag = "N";
        $scope.clearForm();
        $scope.resetFormValidation();
        $http.get('/api/SetupApi/GetMaxGateEntryNo').then(function (res) {
            if (res.data && res.data.DATA) {
                $scope.gateEntry.GATE_NO = res.data.DATA;
            }
        });
        $scope.selectedReferences = "";
        $("#inwardTransactionModal").modal("toggle");
        $scope.initializeDates();
    }

    function loadVehicles(filter) {
        return $http.get('/api/SetupApi/GetVehicles', { params: { filter: filter || '' } }).then(function (res) {
            $scope.vehicleList = (res.data && res.data.DATA) || [];
            return $scope.vehicleList;
        });
    }
    function loadTransporters(filter) {
        return $http.get('/api/SetupApi/GetTransporters', { params: { filter: filter || '' } }).then(function (res) {
            $scope.transporterList = (res.data && res.data.DATA) || [];
            return $scope.transporterList;
        });
    }
    function loadReferences(filter) {
        var gd = $scope.gateEntry && $scope.gateEntry.GATE_DATE ? moment($scope.gateEntry.GATE_DATE).format('YYYY-MM-DD') : '';
        return $http.get('/api/SetupApi/GetReferences', { params: { filter: filter || '', gateDate: gd } }).then(function (res) {
            $scope.referenceList = (res.data && res.data.DATA) || [];
            angular.forEach($scope.referenceList, function (item) {
                item.displayText = item.ORDER_NO + ' - ' + item.SUPPLIER_EDESC;
            });
            return $scope.referenceList;
        });
    }
    function getSelectedReference(orderNo) {
        return $http.get('/api/SetupApi/GetSelectedReference', { params: { orderNo: orderNo } }).then(function (res) {
            let selectedReference = (res.data && res.data.DATA) || [];
            return selectedReference;
        })
    }
    function loadVehicleTypes(filter) {
        return $http.get('/api/SetupApi/GetVehicleTypes', { params: { filter: filter || '' } }).then(function (res) {
            $scope.vehicleTypeList = (res.data && res.data.DATA) || [];
            return $scope.vehicleTypeList;
        });
    }

    function loadLocations() {
        return $http.get('/api/SetupApi/GetLocations').then(function (res) {
            $scope.locationList = (res.data && res.data.DATA) || [];
            return $scope.locationList;
        });
    }

    $scope.onVehicleChange = function () {
        if (!$scope.selectedVehicle);
        let selectedVehicle = $scope.vehicleList.find(a => a.NAME.includes($scope.selectedVehicle.trim()))
        if (!selectedVehicle) {
            $scope.gateEntry.DRIVER_NAME = "";
        }
        $scope.gateEntry.VEHICLE_NAME = selectedVehicle.NAME;
        $scope.gateEntry.VEHICLE_CODE = selectedVehicle.CODE;
        $http.get('/api/SetupApi/GetDriverByVehicleEdesc', { params: { vehicleEdesc: $scope.selectedVehicle } }).then(function (res) {
            var name = res.data && res.data.DATA;
            if (name) $scope.gateEntry.DRIVER_NAME = name;
        });
    };

    // Open shared selector modals and consume selections
    $scope.openSupplierSelector = function () {
        if (!window.PartyItemSelector) { console.warn('PartyItemSelector not loaded'); return; }
        PartyItemSelector.openPartyModal('supplier').then(function (res) {
            var list = (res && res.data) || [];
            if (list.length > 0) {
                var s = list[0];
                $scope.$applyAsync(function () {
                    $scope.gateEntry.SUPPLIER_CODE = s.SUPPLIER_CODE || s.CODE || s.Id || s.id;
                    $scope.gateEntry.SUPPLIER_EDESC = s.SUPPLIER_EDESC || s.NAME || s.Name || '';
                });
            }
        });
    };

    $scope.openItemSelector = function () {
        if (!window.PartyItemSelector) { console.warn('PartyItemSelector not loaded'); return; }
        PartyItemSelector.openItemModal().then(function (res) {
            var items = (res && res.data) || [];
            if (items.length === 0) return;
            var grid = $("#inwardGrid").data("kendoGrid");
            if (!grid) return;
            var ds = grid.dataSource;
            var existing = ds.data().toJSON ? ds.data().toJSON() : ds.data();
            items.forEach(function (it, idx) {
                // Avoid duplicates by ITEM_CODE
                var code = it.ITEM_CODE || it.CODE || it.Id || it.id;
                if (!code) return;
                var already = existing.some(function (e) { return (e.ITEM_CODE || e.CODE) === code; });
                if (!already) {
                    ds.add({
                        SERIAL_NO: existing.length + idx + 1,
                        ITEM_CODE: code,
                        ITEM_EDESC: it.ITEM_EDESC || it.NAME || it.Name || '',
                        MU_CODE: it.MU_CODE || '',
                        AVAILABLE_QTY: it.AVAILABLE_QTY || 0,
                        BILL_QTY: it.AVAILABLE_QTY || 0,
                        FORM_CODE: it.FORM_CODE || $scope.gateEntry.FORM_CODE || ''
                    });
                }
            });
            grid.refresh();
        });
    };

    $scope.onTransporterChange = function () {
        debugger
        if (!$scope.selectedTransporter) return;
        let selectedTransporter = $scope.transporterList.find(a => a.NAME.includes($scope.selectedTransporter.trim()))
        $scope.gateEntry.TRANSPORT_NAME = selectedTransporter.NAME;
        $scope.gateEntry.TRANSPORT_CODE = selectedTransporter.CODE;
    };
    $scope.onVehicleTypeChange = function () {
        if (!$scope.selectedVehicleType) return;
        $scope.gateEntry.VEHICLE_TYPE = $scope.selectedVehicleType.NAME;
    };

    $scope.onLocationChange = function () {
        if (!$scope.selectedLocation) return;
        let selectedLocation = $scope.locationList.find(a => a.NAME.includes($scope.selectedLocation.trim()))
        $scope.gateEntry.LOCATION_CODE = selectedLocation.CODE;
    };

    $scope.onReferencesChange = function () {
        let multiSelectWidget = $("#referenceMultiSelect").data("kendoMultiSelect");

        if (!multiSelectWidget) {
            console.error("Kendo MultiSelect widget not found.");
            return;
        }

        let references = multiSelectWidget.dataItems();

        if (!references || references.length === 0) {
            $scope.gateEntry.SUPPLIER_EDESC = "";
            $scope.gateEntry.SUPPLIER_CODE = "";
            $scope.selectedOrderNo = "";

            let grid = $("#inwardGrid").data("kendoGrid");
            if (grid) {
                grid.dataSource.data([]);
                grid.refresh();
            }
            return;
        }

        $scope.handleMultiReferencePopulation(references);
    };
   
    $scope.handleMultiReferencePopulation = function (selectedReferences) {
        if (!selectedReferences || selectedReferences.length === 0) return;

        let referencePairs = selectedReferences.map(function (ref) {
            return {
                ReferenceNo: ref.ORDER_NO,
                FormCode: ref.FORM_CODE
            };
        });

        let multiReferenceRequest = {
            References: referencePairs,
            GateDate: $scope.gateEntry.GATE_DATE
        };

        $scope.selectedOrderNo = selectedReferences.map(r => r.ORDER_NO).join(', ');

        $http.post('/api/SetupApi/GetPartyNameByReferences', referencePairs)
            .then(function (response) {
                if (response.data && response.data.DATA && response.data.DATA.length > 0) {
                    let suppliers = response.data.DATA;

                    let combinedNames = suppliers.map(s => s.SUPPLIER_EDESC).join(', ');

                    $scope.gateEntry.SUPPLIER_EDESC = combinedNames;

                    $scope.gateEntry.SUPPLIER_CODE = suppliers[0].SUPPLIER_CODE;
                } else {
                    $scope.gateEntry.SUPPLIER_EDESC = "";
                    $scope.gateEntry.SUPPLIER_CODE = "";
                }
            });

        $http.post('/api/SetupApi/GetMultiReferenceItems', multiReferenceRequest)
            .then(function (response) {
                if (response.data && response.data.DATA) {
                    let combinedItems = response.data.DATA;

                    $scope.referenceItems = combinedItems.map(function (item) {
                        return {
                            REFERENCE_NO: item.REFERENCE_NO,
                            SERIAL_NO: item.SERIAL_NO,
                            ITEM_EDESC: item.ITEM_EDESC,
                            MU_CODE: item.MU_CODE,
                            AVAILABLE_QTY: item.AVAILABLE_QTY,
                            BILL_QTY: item.AVAILABLE_QTY
                        };
                    });

                    $scope.initializeInwardGrid();
                    let grid = $("#inwardGrid").data("kendoGrid");
                    if (grid) {
                        grid.dataSource.data([]);
                        grid.dataSource.data($scope.referenceItems);
                        grid.refresh();
                    }
                } else {
                    let grid = $("#inwardGrid").data("kendoGrid");
                    if (grid) {
                        grid.dataSource.data([]);
                        grid.refresh();
                    }
                }
            });
    };

    $scope.populateOthersThroughReferences = function (selectedReference) {
        referenceNo = selectedReference.ORDER_NO;
        $scope.selectedOrderNo = referenceNo;
        let formCode = selectedReference.FORM_CODE;

        $http.get('/api/SetupApi/GetPartyNameByReference', {
            params: {
                referenceNo: referenceNo,
                formCode: formCode
            }
        }).then(function (response) {

            if (response.data && response.data.DATA) {
                $scope.gateEntry.SUPPLIER_EDESC = response.data.DATA.SUPPLIER_EDESC;
                $scope.gateEntry.SUPPLIER_CODE = response.data.DATA.SUPPLIER_CODE;
            }
        });

        $http.get('/api/SetupApi/GetReferenceItems', {
            params: {
                referenceNo: referenceNo,
                formCode: formCode,
                gateDate: $scope.gateEntry.GATE_DATE
            }
        }).then(function (response) {

            if (response.data && response.data.DATA) {
                $scope.referenceItems = response.data.DATA.map(function (item) {
                    return {
                        SERIAL_NO: item.SERIAL_NO,
                        ITEM_EDESC: item.ITEM_EDESC,
                        MU_CODE: item.MU_CODE,
                        AVAILABLE_QTY: item.AVAILABLE_QTY,
                        BILL_QTY: item.AVAILABLE_QTY
                    };
                });

                $scope.initializeInwardGrid();
                grid = $("#inwardGrid").data("kendoGrid");
                grid.dataSource.data([])
                grid.dataSource.data($scope.referenceItems);
                grid.refresh();
            }
        });
    }

    $scope.onInwardTypeChange = function () {
        if ($scope.gateEntry.INWARD_TYPE === 'Referencial Inward') {
            loadReferences('');
        } else {
            $scope.gateEntry.REFERENCE_NO = '';
            $scope.selectedReference = '';
            $scope.referenceItems = [];
            $("#inwardGrid").kendoGrid().data([]);
            $scope.gateEntry.SUPPLIER_EDESC = ''
        }
    };

    $q.all([loadVehicles(''), loadTransporters(''), loadVehicleTypes(''), loadLocations()]);

    function parseTimeString(timeStr) {
        if (!timeStr) return null;
        var parts = timeStr.split(":");
        return new Date(1970, 0, 1, parseInt(parts[0], 10), parseInt(parts[1], 10));
    }

    $scope.editGateEntry = function (gateNo) {
        if (!gateNo) return;
        $http.get('/api/SetupApi/GetGateEntryById', { params: { gateNo: gateNo } }).then(function (res) {
            if (res.data && res.data.DATA) {
                var d = res.data.DATA;
                d.GATE_IN_FLAG = d.GATE_IN_FLAG === 'Y' || d.GATE_IN_FLAG === 'y' || d.GATE_IN_FLAG === true;
                $scope.gateEntry = d;
                $scope.gateEntry.IN_TIME = parseTimeString(d.IN_TIME)
                $scope.gateEntry.OUT_TIME = parseTimeString(d.OUT_TIME)
                $scope.saveupdatebtn = "Update";
                $scope.editFlag = "Y";
                $("#gateNepaliDate").val(AD2BS(moment($scope.gateEntry.GATE_DATE).format("YYYY-MM-DD")));
                $("#billNepaliDate").val(AD2BS(moment($scope.gateEntry.BILL_DATE).format("YYYY-MM-DD")))
                $q.all([loadVehicles(''), loadTransporters(''), loadVehicleTypes(''), loadLocations()]).then(function () {

                    $scope.selectedVehicle = ($scope.vehicleList || []).find(function (x) { return x.NAME === d.VEHICLE_NAME; }).NAME || null;
                    $scope.selectedTransporter = ($scope.transporterList || []).find(function (x) { return x.NAME === d.TRANSPORT_NAME; }).NAME || null;
                    //$scope.selectedVehicleType = ($scope.vehicleTypeList || []).find(function (x) { return x.NAME === d.VEHICLE_TYPE; }).CODE || null;
                    $scope.selectedLocation = ($scope.locationList || []).find(function (x) { return x.CODE === d.LOCATION_CODE; }).NAME || null;
                });
                if ($scope.gateEntry.INWARD_TYPE === 'Referencial Inward') {
                    loadReferences('').then(function () {
                        debugger
                        $scope.onReferenceChange(d.REFERENCE_NO);
                    });
                }
                $("#inwardTransactionModal").modal("show");
            } else {
                if (typeof displayPopupNotification === 'function') displayPopupNotification('Not found', 'warning');
            }
        }, function (err) {
            if (typeof displayPopupNotification === 'function') displayPopupNotification('Error loading record', 'error');
        });
    };
    $scope.onReferenceChange = function (refnum) {

        if (!refnum) {
            $scope.selectedReferences = [];
            return;
        }

        // Filter matching records
        var filtered = ($scope.referenceList || []).filter(function (x) {
            return x.ORDER_NO === refnum;
        });

        // Assign ORDER_NO values to MultiSelect model
        $scope.selectedReferences = filtered.map(function (x) {
            return x.ORDER_NO;
        });
    };


    $scope.deleteGateEntry = function (gateNo) {
        if (!gateNo) return;
        if (!confirm('Delete Gate Entry ' + gateNo + '?')) return;
        $http.delete('/api/SetupApi/DeleteGateEntry', { params: { gateNo: gateNo } }).then(function (res) {
            if (typeof displayPopupNotification === 'function') displayPopupNotification('Deleted', 'success');
            $scope.BindGateEntryGrid();
        }, function () {
            if (typeof displayPopupNotification === 'function') displayPopupNotification('Delete failed', 'error');
        });
    };
    function formatTimeToString(dateObj) {
        if (!dateObj) return null;
        var hours = ("0" + dateObj.getHours()).slice(-2);
        var minutes = ("0" + dateObj.getMinutes()).slice(-2);
        return hours + ":" + minutes;
    }
    $scope.toDate = function (val) {
        if (!val) return null;
        var m = moment(val);
        return m.isValid() ? m.format("YYYY-MM-DD") : null;
    }


    $scope.resetFormValidation = function () {
        if ($scope.inwardForm) {
            // Reset form to pristine state
            $scope.inwardForm.$setPristine();
            $scope.inwardForm.$setUntouched();
        }
    };

    $scope.saveGateEntry = function () {
 
        if ($scope.inwardForm.$invalid) {
            angular.forEach($scope.inwardForm.$error, function (field) {
                angular.forEach(field, function (errorField) {
                    errorField.$setTouched();
                });
            });
            return;
        }
        if (!$scope.selectedOrderNo) {
            displayPopupNotification('Please select Reference Number', 'warning');
            return;
        }

        let dataToSave = [];
        var grid = $("#inwardGrid").data("kendoGrid");
        var gridData = grid ? grid.dataSource.data() : [];
        gridData.forEach(function (item) {
            var payload = angular.copy($scope.gateEntry || {});
            if ($scope.selectedOrderNo) payload.REFERENCE_NO = $scope.selectedOrderNo;
            payload.GATE_DATE = $scope.toDate(payload.GATE_DATE);
            payload.BILL_DATE = $scope.toDate(payload.BILL_DATE);
            payload.CREATED_DATE = $scope.toDate(payload.CREATED_DATE) || new Date().toISOString();
            payload.GATE_IN_FLAG = payload.GATE_IN_FLAG ? 'Y' : 'N';
            payload.DELETED_FLAG = payload.DELETED_FLAG || 'N';
            payload.IN_TIME = formatTimeToString(payload.IN_TIME);
            payload.OUT_TIME = formatTimeToString(payload.OUT_TIME);
            payload.ITEM_CODE = item.ITEM_CODE;
            payload.MU_CODE = item.MU_CODE;
            payload.Detail_BILL_QTY = item.AVAILABLE_QTY;
            payload.BILL_VALUE = item.TOTAL_PRICE;
            payload.BILL_RATE = item.UNIT_PRICE;
            payload.FORM_CODE = item.FORM_CODE;
            dataToSave.push(payload);
        });

        $http.post('/api/SetupApi/SaveGateEntry', dataToSave).then(function (res) {
            var msg = (res.data && res.data.MESSAGE) || 'Saved';
            if (typeof displayPopupNotification === 'function') displayPopupNotification(msg, 'success');
            $scope.resetFormValidation();
            $("#inwardTransactionModal").modal("hide");
            $scope.BindGateEntryGrid();
            $scope.clearForm();
        }, function (err) {
            var emsg = (err.data && err.data.MESSAGE) || 'Save failed';
            if (typeof displayPopupNotification === 'function') displayPopupNotification(emsg, 'error');
        });
    };

    $scope.printGateEntry = function () {
        var grid = $("#gateEntryGrid").data("kendoGrid");
        if (!grid) {
            console.error("Kendo Grid instance not found. Make sure the grid has id='gateEntryGrid'.");
            return;
        }

        var data = grid.dataSource.data();

        if (data.length === 0) {
            displayPopupNotification("No data available to print.", "info");
            return;
        }

        var allColumns = [
            { field: "GATE_NO", title: "Gate No" },
            { field: "GATE_MITI", title: "Gate Miti" },
            { field: "BILL_NO", title: "Bill No" },
            { field: "BILL_MITI", title: "Bill Miti" },
            { field: "SUPPLIER_EDESC", title: "Party Name" },
            { field: "ITEM_EDESC", title: "Item Name" },
            { field: "Detail_BILL_QTY", title: "Quantity" },
            { field: "LOCATION", title: "Location" },
            { field: "TRANSPORT_NAME", title: "Transport Name" },
            { field: "VEHICLE_NAME", title: "Vehicle No" },
            { field: "IN_TIME", title: "In Time" },
            { field: "OUT_TIME", title: "Out Time" },
            { field: "GROSS_WT", title: "Gross Wt" },
            { field: "TEAR_WT", title: "Tear Wt" },
            { field: "NET_WT", title: "Net Wt" },
            { field: "RECEIVED_BY", title: "Received By" },
            { field: "REMARKS", title: "Remarks" },
            { field: "STATUS", title: "Status" }
        ];

        var proceedByColumn = { field: "GATE_IN_BY", title: "Proceed By" };

        var printContent = `
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Gate Entry Report</title>
                    <style>
                        body { font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; font-size: 10px; }
                        table { width: 100%; border-collapse: collapse; }
                        th, td { border: 1px solid #ccc; padding: 6px; text-align: left; }
                        th { background-color: #f2f2f2; }
                        .page-break { page-break-before: always; }
                        .report-header { text-align: center; }
                        .footer { display: flex; justify-content: space-between; position: fixed; bottom: 0; width: 100%; font-size: 8px; }
                        .proceed-by { text-align: center; margin-top: 20px; font-size: 12px; }
                        .page-number { text-align: right; font-size: 8px; }
                    </style>
                </head>
                <body>
            `;

            printContent += `
            <div class='page-content'>
                <h2 class='report-header'>Gate Entry Report</h2>
                <table>
                    <thead>
                        <tr>
                            ${allColumns.map(col => `<th>${kendo.htmlEncode(col.title)}</th>`).join('')}
                        </tr>
                    </thead>
                    <tbody>
                        ${data.map(item => `
                            <tr>
                                ${allColumns.map(col => `<td>${kendo.htmlEncode(item[col.field] || '')}</td>`).join('')}
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
                <div class='proceed-by'>
                    <strong>Proceed By:</strong> ${data.length > 0 ? kendo.htmlEncode(data[0].GATE_IN_BY || '') : 'N/A'}
                </div>
                <div class='footer'>
                    <span>Printed on ${new Date().toLocaleDateString()} at ${new Date().toLocaleTimeString()}</span>
                    <span>Printed by ADMIN</span>
                </div>
            </div>
        `;

        printContent += `</body></html>`;

        var printWindow = window.open("", "_blank");

        if (!printWindow) {
            alert("Pop-up window blocked. Please enable pop-ups for this site.");
            return;
        }

        printWindow.document.open();
        printWindow.document.write(printContent);
        printWindow.document.close();

        printWindow.onafterprint = function () {
            printWindow.close();
        };

        printWindow.focus();
        printWindow.print();
    };

});

