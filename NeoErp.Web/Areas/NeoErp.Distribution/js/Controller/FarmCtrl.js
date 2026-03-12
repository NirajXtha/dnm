


distributionModule.controller('FarmCtrl', function ($scope, $http, distributorService, $routeParams) {



    $scope.isViewLoading = false;
    $scope.$on('$routeChangeStart', function () {
        $scope.isViewLoading = true;
    });
    $scope.$on('$routeChangeSuccess', function () {
        $scope.isViewLoading = false;
        $scope.fetchSubdistributors();
    });
    $scope.$on('$routeChangeError', function () {
        $scope.isViewLoading = false;
    });
    $scope.showCustomerSelect = true;



        // Existing code...

        // Hardcoded JSON data for testing
        const hardcodedData = {
            farmingCrops: [
                { CROP_ID: 1, CROP_NAME: "Wheat" },
                { CROP_ID: 2, CROP_NAME: "Rice" },
                { CROP_ID: 3, CROP_NAME: "Corn" }
            ],
            dealers: [
                { DEALER_ID: 1, DEALER_NAME: "Dealer A" },
                { DEALER_ID: 2, DEALER_NAME: "Dealer B" },
                { DEALER_ID: 3, DEALER_NAME: "Dealer C" }
            ],
            subDealers: [
                { SUB_DEALER_ID: 1, SUB_DEALER_NAME: "Sub-Dealer A" },
                { SUB_DEALER_ID: 2, SUB_DEALER_NAME: "Sub-Dealer B" },
                { SUB_DEALER_ID: 3, SUB_DEALER_NAME: "Sub-Dealer C" }
            ],
            //farmDetails: {
            //    latitude: 27.70320076199206,
            //    longitude: 85.31524620117193,
            //    farming_crops: [1, 2], // Example selected crops
            //    dealers: [1], // Example selected dealer
            //    sub_dealers: [2] // Example selected sub-dealer
            //}
        };

        // New MultiSelect options for farming_crops, dealers, and sub_dealers using hardcoded data
        $scope.farmingCropsSelectOptions = {
            dataTextField: "CROP_NAME",
            dataValueField: "CROP_ID",
            height: 600,
            valuePrimitive: true,
            placeholder: "Select Farming Crops...",
            autoClose: false,
            dataSource: []
        };

        $scope.dealersSelectOptions = {
            dataTextField: "DEALER_NAME",
            dataValueField: "DEALER_ID",
            height: 600,
            valuePrimitive: true,
            placeholder: "Select Dealers...",
            autoClose: false,
            dataSource: []
        };

        $scope.subDealersSelectOptions = {
            dataTextField: "SUB_DEALER_NAME",
            dataValueField: "SUB_DEALER_ID",
            height: 600,
            valuePrimitive: true,
            placeholder: "Select Sub-Dealers...",
            autoClose: false,
            dataSource: []
        };

        // Function to fetch data for editing
    $scope.fetchFarmData = function (item) {
        console.log(item);
            $scope.selectedFarmingCrops = item.FARMING_CROPS; 
            $scope.selectedDealers = hardcodedData.dealers; 
            $scope.selectedSubDealers = hardcodedData.sub_dealers; 
            //$('#maplat').html(hardcodedData.farmDetails.latitude); 
            //$('#maplng').html(hardcodedData.farmDetails.longitude); 
            //// Update the map
            //var latlong = [hardcodedData.farmDetails.latitude, hardcodedData.farmDetails.longitude];
            //if (map) {
            //    map.setView(latlong, 15);
            //    marker.setLatLng(latlong);
            //}
        };

        






    $scope.distGroupSelectOptions = {
        //close: function () {

        //    var selected = $("#distGroupSelect").data("kendoMultiSelect").dataItem();
        //    $scope.selectedGroup = typeof (selected) == 'undefined' ? [] : [String(selected.GROUPID)];
        //    //$scope.$apply();
        //},
        dataTextField: "GROUP_EDESC",
        dataValueField: "GROUPID",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Group...</strong></div>',
        placeholder: "Select Group...",
        autoClose: false,
        select: function (e) {

        },
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/getAllResellerGroups",
                    dataType: "json"
                }
            }
        }
    };
    //$scope.dataSourceBrand = [];
    //var productsDataSource = new kendo.data.DataSource({
    //    transport: {
    //        read: {
    //            //url: window.location.protocol + "//" + window.location.host + "/api/Distributor/GetDistributorItems",
    //            url: window.location.protocol + "//" + window.location.host + "/api/DistributionPurchase/GetAllItems",
    //            dataType: "json"
    //        }
    //    }
    //});

//    productsDataSource.fetch(function () {
//        $scope.distBrandSelectOptions = {
//            dataTextField: "BRAND_NAME",
//            dataValueField: "BRAND_NAME",
//            height: 600,
//            valuePrimitive: true,
//            placeholder: "Select Brands...",
//            autoClose: false,
//            headerTemplate: `
//    <div class="k-header d-flex align-items-center ng-scope" style="
//        display: flex;
//        flex-direction: row;
//        justify-content: space-between;
//        align-items: center;
//    ">
//        <strong>Brands</strong>
//        <div
//            style="font-size: 9px; cursor: pointer;"
//            onmouseover="this.style.textDecoration='underline'"
//            onmouseout="this.style.textDecoration='none'"
//            onclick="angular.element(this).scope().selectAllBrands()"
//        >
//            Select All
//        </div>
//    </div>
//`,
//            dataBound: function (e) {
//                $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
//            },
//            dataSource: new kendo.data.DataSource({
//                data: _.uniq(this.data(), "BRAND_NAME"),
//            }),
//            change: function () {
//                //buildFilters(this.dataItems());
//            }
//        };

        //$scope.selectAllBrands = function () {
        //    var multiselect = $("#distBrandSelect").data("kendoMultiSelect");

        //    if (!multiselect) return;

        //    // Ensure userSetupTree exists
        //    if (!$scope.userSetupTree) {
        //        $scope.userSetupTree = {}; // Initialize the object if not defined
        //    }

        //    // Make sure BrandMultiSelect exists
        //    if (!$scope.userSetupTree.BrandMultiSelect) {
        //        $scope.userSetupTree.BrandMultiSelect = []; // Initialize if not defined
        //    }

        //    multiselect.dataSource.fetch().then(function () {
        //        const allValues = multiselect.dataSource.data().map(item => item[multiselect.options.dataValueField]);
        //        multiselect.value(allValues);
        //        $scope.userSetupTree.BrandMultiSelect = allValues;

        //        buildFilters(multiselect.dataItems());

        //        $scope.$apply();
        //    });
        //};

    //});

    //$scope.distItemsSelectOptions = {
    //    dataTextField: "ITEM_EDESC",
    //    dataValueField: "ITEM_CODE",
    //    height: 600,
    //    valuePrimitive: true,
    //    headerTemplate: '<div class="col-md-offset-3"><strong>Items...</strong></div>',
    //    placeholder: "Select Items...",
    //    autoClose: false,
    //    dataBound: function (e) {
    //        var current = this.value();
    //        this._savedOld = current.slice(0);
    //        $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
    //    },
    //    dataSource: productsDataSource

    //};

    $scope.distDistributorSelectOptions = {
        close: function (e) {
            //clear sub outlet
            $scope.selectedSubDistributor = '';
            $("#distSubDistributorSelect").data("kendoMultiSelect").value([]);

            var selected = $("#distDistributorSelect").data("kendoMultiSelect").dataItem();
            $scope.selectedDistributor = typeof (selected) == 'undefined' ? [] : [String(selected.TYPE_ID)];
            if (typeof (selected) != 'undefined')
                $scope.fetchSubdistributors(selected.TYPE_ID);
            //$scope.$apply();
        },
        dataTextField: "TYPE_EDESC",
        dataValueField: "TYPE_ID",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Distributor...</strong></div>',
        placeholder: "Select Distributor...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/getAllOutletList",
                    dataType: "json"
                }
            }
        }
    };

    $scope.fetchSubdistributors = function (typeId) {
        var url = '';
        if (typeof (typeId) == 'undefined')
            url = window.location.protocol + "//" + window.location.host + "/api/Setup/getAllSubOutletList";
        else
            url = window.location.protocol + "//" + window.location.host + "/api/Setup/getAllSubOutletList?TYPE_ID=" + typeId
        var dataSource = $("#distSubDistributorSelect").data("kendoMultiSelect");
        if (typeof (dataSource) != 'undefined' && dataSource != null) {
            $("#distSubDistributorSelect").data("kendoMultiSelect").dataSource.options.transport.read.url = url;
            $("#distSubDistributorSelect").data("kendoMultiSelect").dataSource.read();
            return;
        }
        $scope.distSubDistributorSelectOptions = {
            dataTextField: "SUBTYPE_EDESC",
            dataValueField: "SUBTYPE_ID",
            height: 600,
            valuePrimitive: true,
            maxSelectedItems: 1,
            headerTemplate: '<div class="col-md-offset-3"><strong>Outlet Category...</strong></div>',
            placeholder: "Select Distributor Category...",
            autoClose: false,
            dataBound: function (e) {
                var current = this.value();
                this._savedOld = current.slice(0);
                $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
            },
            dataSource: {
                transport: {
                    read: {
                        url: url,
                        dataType: "json"
                    }
                }
            }
        };
        //$("#distSubOutletSelect").data("kendoMultiSelect").dataSource.refresh();
    }

    //function buildFilters(dataItems) {
    //    var filters = [],
    //        length = dataItems.length,
    //        idx = 0, dataItem;
    //    if (length == 0) {
    //        $("#distItemsSelect").data("kendoMultiSelect").value("");
    //    }
    //    for (; idx < length; idx++) {
    //        dataItem = dataItems[idx];

    //        var data = $("#distItemsSelect").data("kendoMultiSelect").dataSource.data();
    //        var filterdata = _.filter(data, function (da) { return da.BRAND_NAME == dataItem.BRAND_NAME; });
    //        for (var i = 0; i < filterdata.length; i++) {
    //            filters.push(filterdata[i].ITEM_CODE);
    //        }

    //        $("#distItemsSelect").data("kendoMultiSelect").value(filters);

    //        //filters.push({
    //        //    field: "BRAND_NAME",
    //        //    operator: "eq",
    //        //    value: parseInt(dataItem.BRAND_NAME)
    //        //});
    //    }


    //};
    $scope.pageName = "Add Distributor";
    $scope.saveAction = "Save";
    $scope.createPanel = false;
    reportConfig = GetReportSetting("DistributionSetup");


    async function loadLeaflet() {
        return new Promise((resolve, reject) => {
            // Load Leaflet CSS
            const link = document.createElement("link");
            link.href = "https://unpkg.com/leaflet@1.9.4/dist/leaflet.css";
            link.rel = "stylesheet";
            link.onload = () => {
                // Once the CSS is loaded, load the Leaflet JS
                const script = document.createElement("script");
                script.src = "https://unpkg.com/leaflet@1.9.4/dist/leaflet.js";
                script.type = "text/javascript";
                script.async = true;
                script.onload = resolve;
                script.onerror = reject; // Reject the promise if there's an error
                document.head.appendChild(script);
            };
            link.onerror = reject;
            document.head.appendChild(link);
        });
    }

    //map
    var markersArray = [], map, marker;
    $scope.initialize = async function () {
        await loadLeaflet();
        myLatlng = [27.70320076199206, 85.31524620117193];

        if (map) {
            console.log("Map exists, removing it...");
            map.remove();
            map = null;
            map.removeLayer(marker);
        }

        // Initialize the map
        map = L.map('map-canvas').setView(myLatlng, 10);



        // Add OpenStreetMap tile layer
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '© OpenStreetMap contributors'
        }).addTo(map);

        // Create a draggable marker
        marker = L.marker(myLatlng, { draggable: true }).addTo(map);



        // Update latitude and longitude display
        function updateLatLngDisplay(lat, lng) {
            $('#maplat').html(lat);
            $('#maplng').html(lng);
        }

        // Update display on marker drag
        marker.on('dragend', function (e) {
            var position = marker.getLatLng();
            updateLatLngDisplay(position.lat, position.lng);
        });

        // Update display on map click
        map.on('click', function (e) {
            marker.setLatLng(e.latlng);
            updateLatLngDisplay(e.latlng.lat, e.latlng.lng);
        });

        // Search box functionality
        if (document.getElementById('mapSearchBox') == null) {
            $("#distributor-map-panel").prepend('<div id="distributor-map-fullscreen"></div><input id="mapSearchBox" class="controls" type="text" placeholder="Search Box">');
        }

        var searchBox = document.getElementById('mapSearchBox');
        var searchControl = L.control({ position: 'topleft' });
        searchControl.onAdd = function () {
            return searchBox;
        };
        searchControl.addTo(map);

        // Use a geocoding service (like Nominatim) for searching
        searchBox.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                var query = searchBox.value;
                fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${query}`)
                    .then(response => response.json())
                    .then(data => {
                        if (data.length > 0) {
                            var latlng = [data[0].lat, data[0].lon];
                            marker.setLatLng(latlng);
                            map.setView(latlng, 15);
                            updateLatLngDisplay(data[0].lat, data[0].lon);
                        } else {
                            console.log("No results found");
                        }
                    });
            }
        });

        // Fullscreen toggle functionality
        $('#distributor-map-fullscreen').on('click', function () {
            if (!$(this).is('.map-fullscreen')) {
                $('body').css({ overflow: 'hidden' });
                $('#distributor-map-panel').css({
                    position: "fixed",
                    top: 0,
                    left: 0,
                    height: $(window).height(),
                    width: "100%",
                    zIndex: 9999,
                    margin: "0 auto"
                });
                $(this).addClass('map-fullscreen');
            } else {
                $('#distributor-map-panel').css({
                    position: "relative",
                    top: 'auto',
                    left: 'auto',
                    height: "425px",
                    width: "100%",
                    zIndex: 1,
                    margin: "0 auto"
                });
                $('body').css({ overflow: 'auto' });
                $(this).removeClass('map-fullscreen');
            }
            map.invalidateSize(); // Resize the map
            var latlng = [$('#maplat').html(), $('#maplng').html()];
            map.setView(latlng);
        });
    };

    var map = null;
    var marker;
    $scope.initialize();

    if (map) {
        console.log("working?");
        map.invalidateSize();
    }



    $scope.mapReset = function () {
        var latlong = [27.700769, 85.300140];
        if (map) {

            map.setView(latlong, 10);
            marker.setLatLng(latlong);
        }
        $('#maplat').html(27.700769);
        $('#maplng').html(85.300140);
        map.invalidateSize();
    };

    $scope.distributorCreate = function (isValid) {
        debugger;
        if (!isValid) {
            displayPopupNotification("Invalid Field", "warning");
            return;
        }
        var selectedCustomer = $scope.selecteddistCustomer;
        var selectedCustomerData = _.filter($("#distCustomerMultiSelect").data("kendoMultiSelect").dataSource.data(), function (da) { return da.DISTRIBUTOR_CODE == selectedCustomer; });

        var selectedArea = $scope.selectedArea;
        var selectedGroup = $scope.selectedGroup;
        var ItemCode = $scope.selectedItems;
        if (_.isEmpty(ItemCode)) {
            ItemCode = $("#distItemsSelect").data("kendoMultiSelect").value();
        }
        var selectedCheckBox = $("#someSwitchOptionPrimary").is(":checked");
        if (selectedCheckBox == true) {
            var ACTIVE = "Y";
        }
        else {
            ACTIVE = "N";
        }

        var createCustomerCheckBox = $("#CustomerSwitchOption").is(":checked");
        if (createCustomerCheckBox == true) {
            var CUSTOMERFLAG = "Y";
        }
        else {
            CUSTOMERFLAG = "N";
        }
        var lat = $('#maplat').html();
        var long = $('#maplng').html();
        var data = {
            ACTIVE: ACTIVE,
            CUSTOMERFLAG: CUSTOMERFLAG,
            DISTRIBUTOR_CODE: selectedCustomer[0],
            AREA_CODE: $scope.selectedArea[0],
            ItemCode: ItemCode,
            DISTRIBUTOR_NAME: selectedCustomer == "" ? selectedCustomerData[0].CUSTOMER_EDESC : "",
            //GROUPID: $scope.selectedGroup[0] ? $scope.selectedGroup[0] : "",
            DISTRIBUTOR_SUBTYPE_ID:
                $scope.selectedSubDistributor
                    ? $scope.selectedSubDistributor[0]
                    : '', //$("#distSubOutletSelect").data("kendoMultiSelect").dataItem().SUBTYPE_ID,
            LATITUDE: $('#maplat').html(),
            LONGITUDE: $('#maplng').html(),
        }
        if ($scope.selectedGroup && $scope.selectedGroup[0])
            data.GROUPID = $scope.selectedGroup[0];

        if ($scope.selectedGroup && $scope.selectedGroup[0])
            data.GROUPID = $scope.selectedGroup[0];

        if ($scope.selectedDistributor && $scope.selectedDistributor[0])
            data.DISTRIBUTOR_TYPE_ID = $scope.selectedDistributor[0];

        data.PAN_NO = $('#PanNo').val();
        data.VAT_NO = $('#VatNo').val();

        if ($scope.saveAction == "Update") //update mode
        {
            distributorService.UpdateDistributor(data).then(function (result) {
                if (result.data.STATUS_CODE === 200) {
                    displayPopupNotification(result.data.MESSAGE, "success");
                    $("#grid").data("kendoGrid").dataSource.read();
                    $('#distCustomerMultiSelect').data('kendoMultiSelect').dataSource.read();
                    $scope.createPanel = false;
                }
                else {
                    displayPopupNotification(result.data.MESSAGE, "warning");
                }
            }, function (error) {
                displayPopupNotification("Error", "error");
            });
        }
        else { //add mode
            distributorService.AddDistributor(data).then(function (result) {
                if (result.data.STATUS_CODE === 200) {
                    displayPopupNotification(result.data.MESSAGE, "success");
                    $("#grid").data("kendoGrid").dataSource.read();
                    $('#distCustomerMultiSelect').data('kendoMultiSelect').dataSource.read();
                    $scope.createPanel = false;
                }
                else {
                    displayPopupNotification(result.data.MESSAGE, "warning");
                }
            }, function (error) {
                displayPopupNotification("Error", "error");
            });
        }




    }


    function getDateFormat(date) {
        return kendo.format("{0:" + reportConfig.dateFormat + "}", new Date(date));
    }




    //bind
    $scope.areaSelectOptions = {
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

    };
    function GetIndividualReport(evt) {
        $http({
            method: 'GET',
            url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetIndividualGroup?SingleAreaCode=" + evt.sender.value()[0],
        }).then(function successCallback(response) {
            $scope.selectedGroup = response.data[0] ? [response.data[0].GROUPID] : null;
            response.data[0] ? $("#distGroupSelect").data("kendoMultiSelect").value(response.data[0].GROUPID) : $("#distGroupSelect").data("kendoMultiSelect").value("");
        }, function errorCallback(response) {
        });
    }


    $scope.distCustomerSelectOptions = {
        dataTextField: "CUSTOMER_EDESC",
        dataValueField: "DISTRIBUTOR_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Distributor...</strong></div>',
        placeholder: "Select Distributor...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        filtering: function (e) {

            if (!e.filter)
                e.filter = { value: "" };
            var searchVal = _.filter(this.dataSource.data(), function (da) { return da.DISTRIBUTOR_CODE == ""; });
            if (searchVal.length > 0)
                this.dataSource.remove(searchVal[0]);
            if (e.filter.value.trim() != "") {
                e.filter.value[0] = e.filter.value[0].toUpperCase();
                this.dataSource.add({ CUSTOMER_EDESC: e.filter.value, DISTRIBUTOR_CODE: "" });
            }
        },
        //close: function (e) {
        //    var searchVal = _.filter(this.dataSource.data(), function (da) { return da.DISTRIBUTOR_CODE == null; });
        //    if (searchVal.length > 0)
        //        this.dataSource.remove(searchVal[0]);
        //},
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetIndividualCustomer",
                    dataType: "json"
                }
            }
        }
    };

    if (!$("#grid").data("kendoGrid")) {
        $("#grid").kendoGrid({
            dataSource: {
                type: "json",
                transport: {
                    read: {
                        url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetFarmList", // <-- Get data from here.
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
                error: function (e) {
                    displayPopupNotification("Sorry error occured while processing data", "error");
                },
                model: {
                    fields: {
                        // ASSIGN_DATE: { type: "date" },
                    }
                },
                //group: { field: "GROUP_EDESC" },
                sort: {
                    field: "WEIGHT",
                    dir: "asc"
                },
                pageSize: 500,
            },
            toolbar: kendo.template($("#toolbar-template").html()),
            excel: {
                fileName: "PO Index",
                allPages: true,
            },
            pdf: {
                fileName: "Received Schedule",
                allPages: true,
                avoidLinks: true,
                pageSize: "auto",
                margin: {
                    top: "2m",
                    right: "1m",
                    left: "1m",
                    buttom: "1m",
                },
                landscape: true,
                repeatHeaders: true,
                scale: 0.8,
            },
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
                        gte: "is greater than or equal to   ",
                        gt: "is greater than",
                        lte: "is less than or equal",
                        lt: "is less than",
                    },
                    string: {

                        eq: "Is equal to",
                        neq: "Is not equal to",
                        startswith: "Starts with    ",
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
                    SaveReportSetting('DistributionSetup', 'grid');
            },
            columnHide: function (e) {
                if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                    SaveReportSetting('DistributionSetup', 'grid');
            },
            pageable: {
                refresh: true,
                pageSizes: reportConfig.itemPerPage,
                buttonCount: 5
            },
            scrollable: {
                virtual: true
            },
            dataBound: function (o) {
                GetSetupSetting("DistributorSetup");
                var grid = o.sender;
                if (grid.dataSource.total() == 0) {
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

                UpdateReportUsingSetting("DistributionSetup", "grid");
                $('div').removeClass('.k-header k-grid-toolbar');
            },
            columns: [

                {
                    field: "FARMER_ID",
                    title: "Code",
                    width: "5%"
                },

                {
                    field: "PROFILE_IMG",
                    title: "Farmer Photo",
                    width: "85px",
                    template: function (data) {
                        var imgUrl = window.location.protocol + "//" + window.location.host + '/Areas/NeoErp.Distribution/Images/FarmerImages/' + data.PROFILE_IMG;

                        // Check if SIGNATURE is empty and set to "nophoto" if so
                        if (_.isEmpty(data.PROFILE_IMG)) {
                            imgUrl = window.location.protocol + "//" + window.location.host + "/images/nophoto.png";
                        }
                        return '<a class="fancybox" href="' + imgUrl + '" data-fancybox="group_' + data.ATT_DATE + data.SP_CODE + '" data-caption="' + data.FULL_NAME + '">' +
                            '<img src="' + imgUrl + '" class="img-responsive img-thumbnail" style="width:63px;height:35px;margin: 0 auto;" />' +
                            '</a>';
                    },
                },

                {
                    field: "FARM_EDESC",
                    title: "Farm Name",
                    width: "15%"
                },


                {
                    field: "ADDRESS",
                    title: "Address",
                    width: "20%"
                },

                {
                    field: "FARMER_EDESC",
                    title: "Farmer Name",
                    width: "15%"
                },

                {
                    field: "CONTACT_NO",
                    title: "Contact",
                    width: "10%"
                },

                {
                    field: "EXPERIENCE",
                    title: "Experience",
                    width: "10%"
                },


                {
                    field: "FARM_AREA",
                    title: "Land Size",
                    width: "10%"
                },
                {
                    field: "AREA_CODE",
                    title: "Area Code",
                    width: "8%"
                },

                {
                    field: "REMARKS",
                    title: "Remarks",
                    width: "15%"
                },

                {
                    field: "FARMING_CROPS",
                    title: "Farm Crops",
                    width: "15%"
                }

                //{
                //    title: "Action",
                //    template: " <a class='fa fa-edit editAction' onclick='UpdateClickEvent($(this))' title='Edit'></a>&nbsp &nbsp<a class='fa fa-trash-o deleteAction' onclick='deleteDistributor($(this))' title='delete'></a> ",
                //    width: "7%"
                //}
            ]
        });

        var grid = $("#grid").data("kendoGrid");
        grid.table.kendoSortable({
            filter: ">tbody >tr",
            hint: function (element) { //customize the hint
                var table = $('<table style="width: 900px;" class="k-grid k-widget"></table>'),
                    hint;

                table.append(element.clone()); //append the dragged element
                table.css("opacity", 0.7);

                return table; //return the hint element
            },
            placeholder: function (element) {
                return element.clone().addClass("k-state-hover").css("opacity", 1);
            },
            container: "#grid tbody",
            filter: ">tbody > tr:not('.k-grouping-row')",
            group: "gridGroup",

            change: function (e) {
                var grid = $("#grid").data("kendoGrid"),
                    oldIndex = e.oldIndex,
                    newIndex = e.newIndex,
                    view = grid.dataSource.view(),
                    dataItem = grid.dataSource.getByUid(e.item.data("uid"));
                dataItem.dirty = true;
                if (oldIndex < newIndex) {
                    for (var i = oldIndex + 1; i <= newIndex; i++) {
                        sortAscDescFunction();
                    }
                } else {
                    for (var i = oldIndex - 1; i >= newIndex; i--) {
                        sortAscDescFunction();
                    }
                }

                function sortAscDescFunction() {
                    var distributorCodeArr = _.pluck(_(_.filter(grid.dataSource.data(), function (x) {
                        x["index"] = $("#grid").find("[data-uid='" + x.uid + "']").index();
                        return x.GROUPID == dataItem.GROUPID;
                    })).sortBy("index"), "DISTRIBUTOR_CODE");
                    var obj = {
                        OLD_INDEX: oldIndex,
                        NEW_INDEX: newIndex,
                        GROUPID: dataItem.GROUPID == null ? "''" : dataItem.GROUPID,
                        DISTRIBUTOR_CODE: dataItem.DISTRIBUTOR_CODE,
                        DISTRIBUTOR_LIST: distributorCodeArr
                    }
                    distributorService.UpdateOrder(obj).then(function (result) {

                        $("#grid").data("kendoGrid").dataSource.read();
                        $("#grid").data("kendoGrid").dataSource.sort({
                            field: "WEIGHT",
                            dir: "asc"

                        });
                        if (result.data == "Success") {
                            displayPopupNotification("Order Saved", "success")
                        }
                        else {
                            displayPopupNotification("Something Went Wrong", "error")
                        }

                    }, function (error) {
                        displayPopupNotification("Error Occur on Sorting,try again", "error");
                    });

                    //setTimeout(function () {
                    //    $("#grid").data("kendoGrid").dataSource.pageSize(100);
                    //}, 1e3)
                }

            }
        });
    }

    //var grid = $("#grid").kendoGrid({
    //    dataSource: {
    //        type: "json",
    //        transport: {
    //            read: {
    //                url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetFarmList", // <-- Get data from here.
    //                dataType: "json", // <-- The default was "jsonp".
    //                type: "POST",
    //                contentType: "application/json; charset=utf-8"
    //            },
    //            parameterMap: function (options, type) {
    //                var paramMap = JSON.stringify($.extend(options, ReportFilter.filterAdditionalData()));
    //                delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
    //                delete paramMap.$format; // <-- remove format parameter.
    //                return paramMap;
    //            }
    //        },
    //        error: function (e) {
    //            displayPopupNotification("Sorry error occured while processing data", "error");
    //        },
    //        model: {
    //            fields: {
    //                // ASSIGN_DATE: { type: "date" },
    //            }
    //        },
    //        //group: { field: "GROUP_EDESC" },
    //        sort: {
    //            field: "WEIGHT",
    //            dir: "asc"
    //        },
    //        pageSize: 500,
    //    },
    //    toolbar: kendo.template($("#toolbar-template").html()),
    //    excel: {
    //        fileName: "PO Index",
    //        allPages: true,
    //    },
    //    pdf: {
    //        fileName: "Received Schedule",
    //        allPages: true,
    //        avoidLinks: true,
    //        pageSize: "auto",
    //        margin: {
    //            top: "2m",
    //            right: "1m",
    //            left: "1m",
    //            buttom: "1m",
    //        },
    //        landscape: true,
    //        repeatHeaders: true,
    //        scale: 0.8,
    //    },
    //    height: window.innerHeight - 50,
    //    sortable: true,
    //    reorderable: true,
    //    groupable: true,
    //    resizable: true,
    //    filterable: {
    //        extra: false,
    //        operators: {
    //            number: {
    //                eq: "Is equal to",
    //                neq: "Is not equal to",
    //                gte: "is greater than or equal to   ",
    //                gt: "is greater than",
    //                lte: "is less than or equal",
    //                lt: "is less than",
    //            },
    //            string: {

    //                eq: "Is equal to",
    //                neq: "Is not equal to",
    //                startswith: "Starts with    ",
    //                contains: "Contains",
    //                doesnotcontain: "Does not contain",
    //                endswith: "Ends with",
    //            },
    //            date: {
    //                eq: "Is equal to",
    //                neq: "Is not equal to",
    //                gte: "Is after or equal to",
    //                gt: "Is after",
    //                lte: "Is before or equal to",
    //                lt: "Is before",
    //            }
    //        }
    //    },
    //    columnMenu: true,
    //    columnMenuInit: function (e) {
    //        wordwrapmenu(e);
    //        checkboxItem = $(e.container).find('input[type="checkbox"]');
    //    },
    //    columnShow: function (e) {
    //        if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
    //            SaveReportSetting('DistributionSetup', 'grid');
    //    },
    //    columnHide: function (e) {
    //        if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
    //            SaveReportSetting('DistributionSetup', 'grid');
    //    },
    //    pageable: {
    //        refresh: true,
    //        pageSizes: reportConfig.itemPerPage,
    //        buttonCount: 5
    //    },
    //    scrollable: {
    //        virtual: true
    //    },
    //    dataBound: function (o) {
    //        GetSetupSetting("DistributorSetup");
    //        var grid = o.sender;
    //        if (grid.dataSource.total() == 0) {
    //            var colCount = grid.columns.length;
    //            $(o.sender.wrapper)
    //                .find('tbody')
    //                .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
    //            displayPopupNotification("No Data Found.", "info");
    //        }
    //        else {
    //            var g = $("#grid").data("kendoGrid");
    //            for (var i = 0; i < g.columns.length; i++) {
    //                g.showColumn(i);
    //            }
    //            $("div.k-group-indicator").each(function (i, v) {
    //                g.hideColumn($(v).data("field"));
    //            });
    //        }

    //        UpdateReportUsingSetting("DistributionSetup", "grid");
    //        $('div').removeClass('.k-header k-grid-toolbar');
    //    },
    //    columns: [

    //        {
    //            field: "FARMER_ID",
    //            title: "Code",
    //            width: "5%"
    //        },

    //        {
    //            field: "PROFILE_IMG",
    //            title: "Farmer Photo",
    //            width: "85px",
    //            template: function (data) {
    //                var imgUrl = window.location.protocol + "//" + window.location.host + '/Areas/NeoErp.Distribution/Images/FarmerImages/' + data.PROFILE_IMG;

    //                // Check if SIGNATURE is empty and set to "nophoto" if so
    //                if (_.isEmpty(data.PROFILE_IMG)) {
    //                    imgUrl = window.location.protocol + "//" + window.location.host + "/images/nophoto.png";
    //                }
    //                return '<a class="fancybox" href="' + imgUrl + '" data-fancybox="group_' + data.ATT_DATE + data.SP_CODE + '" data-caption="' + data.FULL_NAME + '">' +
    //                    '<img src="' + imgUrl + '" class="img-responsive img-thumbnail" style="width:63px;height:35px;margin: 0 auto;" />' +
    //                    '</a>';
    //            },
    //        },

    //        {
    //            field: "FARM_EDESC",
    //            title: "Farm Name",
    //            width: "15%"
    //        },


    //        {
    //            field: "ADDRESS",
    //            title: "Address",
    //            width: "20%"
    //        },

    //        {
    //            field: "FARMER_EDESC",
    //            title: "Farmer Name",
    //            width: "15%"
    //        },

    //        {
    //            field: "CONTACT_NO",
    //            title: "Contact",
    //            width: "10%"
    //        },
            
    //        {
    //            field: "EXPERIENCE",
    //            title: "Experience",
    //            width: "10%"
    //        },
            

    //        {
    //            field: "FARM_AREA",
    //            title: "Land Size",
    //            width: "10%"
    //        },
    //        {
    //            field: "AREA_CODE",
    //            title: "Area Code",
    //            width: "8%"
    //        },

    //        {
    //            field: "REMARKS",
    //            title: "Remarks",
    //            width: "15%"
    //        },

    //        {
    //            field: "FARMING_CROPS",
    //            title: "Farm Crops",
    //            width: "15%"
    //        }

    //        //{
    //        //    title: "Action",
    //        //    template: " <a class='fa fa-edit editAction' onclick='UpdateClickEvent($(this))' title='Edit'></a>&nbsp &nbsp<a class='fa fa-trash-o deleteAction' onclick='deleteDistributor($(this))' title='delete'></a> ",
    //        //    width: "7%"
    //        //}
    //    ]

    //}).data("kendoGrid");
    //grid.table.kendoSortable({
    //    // cursor: "move",
    //    filter: ">tbody >tr",
    //    hint: function (element) { //customize the hint
    //        var table = $('<table style="width: 900px;" class="k-grid k-widget"></table>'),
    //            hint;

    //        table.append(element.clone()); //append the dragged element
    //        table.css("opacity", 0.7);

    //        return table; //return the hint element
    //    },
    //    placeholder: function (element) {
    //        return element.clone().addClass("k-state-hover").css("opacity", 1);
    //    },
    //    container: "#grid tbody",
    //    filter: ">tbody > tr:not('.k-grouping-row')",
    //    group: "gridGroup",

    //    change: function (e) {
    //        var grid = $("#grid").data("kendoGrid"),
    //            oldIndex = e.oldIndex,
    //            newIndex = e.newIndex,
    //            view = grid.dataSource.view(),
    //            dataItem = grid.dataSource.getByUid(e.item.data("uid"));
    //        dataItem.dirty = true;
    //        if (oldIndex < newIndex) {
    //            for (var i = oldIndex + 1; i <= newIndex; i++) {
    //                sortAscDescFunction();
    //            }
    //        } else {
    //            for (var i = oldIndex - 1; i >= newIndex; i--) {
    //                sortAscDescFunction();
    //            }
    //        }

    //        function sortAscDescFunction() {
    //            var distributorCodeArr = _.pluck(_(_.filter(grid.dataSource.data(), function (x) {
    //                x["index"] = $("#grid").find("[data-uid='" + x.uid + "']").index();
    //                return x.GROUPID == dataItem.GROUPID;
    //            })).sortBy("index"), "DISTRIBUTOR_CODE");
    //            var obj = {
    //                OLD_INDEX: oldIndex,
    //                NEW_INDEX: newIndex,
    //                GROUPID: dataItem.GROUPID == null ? "''" : dataItem.GROUPID,
    //                DISTRIBUTOR_CODE: dataItem.DISTRIBUTOR_CODE,
    //                DISTRIBUTOR_LIST: distributorCodeArr
    //            }
    //            distributorService.UpdateOrder(obj).then(function (result) {

    //                $("#grid").data("kendoGrid").dataSource.read();
    //                $("#grid").data("kendoGrid").dataSource.sort({
    //                    field: "WEIGHT",
    //                    dir: "asc"

    //                });
    //                if (result.data == "Success") {
    //                    displayPopupNotification("Order Saved", "success")
    //                }
    //                else {
    //                    displayPopupNotification("Something Went Wrong", "error")
    //                }

    //            }, function (error) {
    //                displayPopupNotification("Error Occur on Sorting,try again", "error");
    //            });

    //            //setTimeout(function () {
    //            //    $("#grid").data("kendoGrid").dataSource.pageSize(100);
    //            //}, 1e3)
    //        }

    //    }



    //}),



        //events
        $scope.AddClickEvent = function () {
            debugger;
            $("#distCustomerMultiSelect").data("kendoMultiSelect").value([]);
            $("#distCustomerMultiSelect").data("kendoMultiSelect").enable();
            $("#areaMultiSelect").data("kendoMultiSelect").value([]);
            $("#distGroupSelect").data("kendoMultiSelect").value([]);
            //$("#distItemsSelect").data("kendoMultiSelect").value([]);
            //$("#distBrandSelect").data("kendoMultiSelect").value([]);
            $('input[name=someSwitchOptionPrimary]').prop('checked', false);
            $('input[name=CustomerSwitchOption]').prop('checked', true);
            $('input[name=CustomerSwitchOption]').prop('disabled', false);

            $scope.showCustomerSelect = true;
            $scope.selectedArea = null;
            $scope.selecteddistCustomer = null;

            $scope.pageName = "Add Distributor";
            $scope.saveAction = "Save";
            $scope.createPanel = true;
            //update map   
            var latlong = new google.maps.LatLng(27.700769, 85.300140);

            var latlong = [27.700769, 85.300140];
            if (map) {
                map.removeLayer(marker);
                map.setView(latlong, 10);
                marker.setLatLng(latlong);
            }
            $('#maplat').html(27.700769);
            $('#maplng').html(85.300140);

            setTimeout(function () {
                map.invalidateSize();
                map.setView(latlong, 15);
            });

        }
    //UpdateClickEvent = function (evt) {
    //    debugger;

    //    $scope.showCustomerSelect = false;
    //    $scope.pageName = "Update Farm";
    //    $scope.saveAction = "Update";
    //    $scope.createPanel = true;
    //    var grid = $("#grid").data("kendoGrid");
    //    var row = $(evt).closest("tr");
    //    var item = grid.dataItem(row);
       
    //    //$scope.fetchFarmData(item); 
    //    console.log(item);
    //    $scope.selectedFarmingCrops = item.FARMING_CROPS;
    //    $scope.selectedDealers = hardcodedData.dealers;
    //    $scope.selectedSubDealers = hardcodedData.sub_dealers; 
        
    //    //console.log("i m item", item);
    //    var selectedArea = $.grep($("#areaMultiSelect").data("kendoMultiSelect").dataSource.data(), function (element, index) {
    //        return element.AREA_NAME == item.AREA_CODE;
    //    });
    //    var selectedGroup = $.grep($("#distGroupSelect").data("kendoMultiSelect").dataSource.data(), function (element, index) {
    //        return element.GROUP_EDESC == item.GROUP_EDESC;
    //    });

    //    var selectedDistributor = $.grep($("#distDistributorSelect").data("kendoMultiSelect").dataSource.data(), function (element, index) {
    //        return element.TYPE_EDESC == item.TYPE_EDESC;
    //    });
    //    var selectedSubDistributor = $.grep($("#distSubDistributorSelect").data("kendoMultiSelect").dataSource.data(), function (element, index) {
    //        return element.SUBTYPE_EDESC == item.SUBTYPE_EDESC;
    //    });
    //    // $("#distCustomerMultiSelect").data("kendoMultiSelect").value([item.DISTRIBUTOR_CODE]);
    //    $scope.selecteddistCustomer = [item.DISTRIBUTOR_CODE];
    //    $scope.selectedFarmingCrops = [item.FARMING_CROPS];
    //    //$scope.selectedDealers = hardcodedData.dealers;
    //    //$scope.selectedSubDealers = hardcodedData.sub_dealers; 

    //    $('#PanNo').val(item.PAN_NO);
    //    $('#VatNo').val(item.VAT_NO);

    //    var arr = [];
    //    if (!_.isEmpty(item.ItemCodeString))
    //        $.each(item.ItemCodeString.split(','), function (i, obj) {
    //            arr.push(obj);
    //        });
    //    //$("#distItemsSelect").data("kendoMultiSelect").value(arr);
    //    //var selectedDataSource = $("#distItemsSelect").data("kendoMultiSelect").dataItems();
    //    //var filterBrand = [];
    //    //for (var i = 0; i < selectedDataSource.length; i++) {
    //    //    filterBrand.push(selectedDataSource[i].BRAND_NAME);
    //    //}
    //    //$("#distBrandSelect").data("kendoMultiSelect").value(filterBrand);
    //    //console.log($("#distCustomerMultiSelect").data("kendoMultiSelect"));

    //    $("#distCustomerMultiSelect").data("kendoMultiSelect").readonly();
    //    //$("#areaMultiSelect").data("kendoMultiSelect").value(selectedGroup[0].AREA_CODE);
    //    if (selectedGroup[0]) {
    //        $("#distGroupSelect").data("kendoMultiSelect").value([selectedGroup[0].GROUPID]);
    //        $scope.selectedGroup = [selectedGroup[0].GROUPID];
    //    }
    //    else
    //        $("#distGroupSelect").data("kendoMultiSelect").value([]);


    //    $scope.selecteddistCustomer = [item.DISTRIBUTOR_CODE];
    //    $scope.selectedArea = [item.AREA_CODE];
       
    //    $('input[name=CustomerSwitchOption]').prop('disabled', true);

    //    if (selectedDistributor[0]) {
    //        $("#distDistributorSelect").data("kendoMultiSelect").value([selectedDistributor[0].TYPE_ID]);
    //        $scope.selectedDistributor = [selectedDistributor[0].TYPE_ID];
    //    }
    //    else
    //        $("#distDistributorSelect").data("kendoMultiSelect").value([]);

    //    if (selectedSubDistributor[0]) {
    //        $("#distSubDistributorSelect").data("kendoMultiSelect").value([selectedSubDistributor[0].SUBTYPE_ID]);
    //        $scope.selectedSubDistributor = [selectedSubDistributor[0].SUBTYPE_ID];
    //        console.log(selectedSubDistributor);
    //    }
    //    else
    //        $("#distSubDistributorSelect").data("kendoMultiSelect").value([]);


    //    //$scope.pageName = "Update Distributor";
    //    //$scope.saveAction = "Update";

    //    //update map   
    //    //console.log(item);
    //    var latlong = [item.FARM_LATITUDE, item.FARM_LONGITUDE];
    //    if (map) {
    //        //console.log("inside map");
    //        map.invalidateSize();
    //        map.removeLayer(marker);
    //        marker = L.marker(latlong, { draggable: true }).addTo(map);
    //        map.setView(latlong, 15);
    //    }
    //    $('#maplat').html(item.FARM_LATITUDE);
    //    $('#maplng').html(item.FARM_LONGITUDE);

    //}

    //deleteDistributor = function (evt) {

    //    bootbox.confirm({
    //        title: "Delete",
    //        message: "Are you sure?",
    //        buttons: {
    //            confirm: {
    //                label: 'Yes',
    //                className: 'btn-success',
    //                label: '<i class="fa fa-check"></i> Yes',
    //            },
    //            cancel: {
    //                label: 'No',
    //                className: 'btn-danger',
    //                label: '<i class="fa fa-times"></i> No',
    //            }
    //        },
    //        callback: function (result) {

    //            if (result == true) {

    //                var row = $(evt).closest("tr");
    //                var item = grid.dataItem(row);
    //                var DISTRIBUTOR_CODE = item.DISTRIBUTOR_CODE
    //                data = {
    //                    DISTRIBUTOR_CODE: DISTRIBUTOR_CODE,
    //                }

    //                var deleteDistributor = distributorService.deleteDistributor(data);
    //                deleteDistributor.then(function (response) {

    //                    if (response.data.STATUS_CODE == 300) {
    //                        displayPopupNotification(response.data.MESSAGE, "success")
    //                        $("#grid").data("kendoGrid").dataSource.read();
    //                    }
    //                    else {
    //                        displayPopupNotification(response.data.MESSAGE, "error");
    //                    }
    //                });
    //            }

    //        }
    //    });
    //};

    $scope.cancelClickEvent = function () {
        $scope.createPanel = false;
    }

});