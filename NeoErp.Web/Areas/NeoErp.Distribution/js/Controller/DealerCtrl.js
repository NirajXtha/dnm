
distributionModule.controller('DealerCtrl', function ($scope, DistSetupService, $routeParams) {
    $scope.isViewLoading = false;
    $scope.$on('$routeChangeStart', function () {
        $scope.isViewLoading = true;
    });
    $scope.$on('$routeChangeSuccess', function () {
        $scope.isViewLoading = false;
    });
    $scope.$on('$routeChangeError', function () {
        $scope.isViewLoading = false;
    });

    $scope.param = $routeParams.param;
    $scope.pageName = "Add Dealer";
    $scope.saveAction = "Save";
    reportConfig = GetReportSetting("DealerSetup");


   


    //map
    var markersArray = [], map, marker;
    $scope.initialize = function () {
        var myLatlng = new google.maps.LatLng(27.700769, 85.300140);

        var mapOptions = {
            zoom: 10,
            center: myLatlng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);

        marker = new google.maps.Marker({
            map: map,
            position: myLatlng,
            title: 'Center',
            draggable: true
        });



        /***************** search box function *****************/
        // Create the search box and link it to the UI element.


        if (document.getElementById('mapSearchBox') == null)
            $("#dealer-map-panel").prepend('<div id="dealer-map-fullscreen"></div><input id="mapSearchBox" class="controls" type="text" placeholder="Search Box">');
        var input = document.getElementById('mapSearchBox');
        var searchBox = new google.maps.places.SearchBox(input);
        map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);
        // Bias the SearchBox results towards current map's viewport.
        map.addListener('bounds_changed', function () {
            searchBox.setBounds(map.getBounds());
        });

        // Listen for the event fired when the user selects a prediction and retrieve
        // more details for that place.
        searchBox.addListener('places_changed', function () {
            var places = searchBox.getPlaces();
            // console.log(places);

            if (places.length == 0) {
                return;
            }

            if (!places[0].geometry) {
                console.log("Returned place contains no geometry");
                return;
            }

            var latlng = new google.maps.LatLng(places[0].geometry.location.lat(), places[0].geometry.location.lng());
            marker.setPosition(latlng);

            var bounds = new google.maps.LatLngBounds();

            if (places[0].geometry.viewport) {
                bounds.union(places[0].geometry.viewport);
            } else {
                bounds.extend(places[0].geometry.location);
            }
            map.fitBounds(bounds);
            $('#maplat').html(places[0].geometry.location.lat());
            $('#maplng').html(places[0].geometry.location.lng());
        });


        /***************** search box function *****************/


        google.maps.event.addListener(map, 'click', function (e) {
            marker.setPosition(e.latLng);
            $('#maplat').html(e.latLng.lat());
            $('#maplng').html(e.latLng.lng());
        });

        google.maps.event.addListener(marker, 'drag', function (e) {
            $('#maplat').html(e.latLng.lat());
            $('#maplng').html(e.latLng.lng());
        });


        /***************** map full screen button function *****************/
        map.controls[google.maps.ControlPosition.TOP_LEFT].push(document.getElementById('dealer-map-fullscreen'));
        document.getElementById('dealer-map-fullscreen').style.display = 'block';

        $('#dealer-map-fullscreen').on('click', function () {
            var mapCenter = map.getCenter();
            if (!$(this).is('.map-fullscreen')) {
                $('body').css({
                    overflow: 'hidden'
                });

                $('#dealer-map-panel').css({
                    position: "fixed",

                    top: 0,
                    left: 0,
                    height: $(window).height(),
                    width: "100%",
                    zIndex: 9999,
                    margin: "0 auto"
                });
                $(this).addClass('map-fullscreen');
            }
            else {

                $('#dealer-map-panel').css({
                    position: "relative",
                    top: 'auto',
                    left: 'auto',
                    height: "425px",
                    width: "100%",
                    zIndex: 1,
                    margin: "0 auto"
                });
                $('body').css({
                    overflow: 'auto'
                });
                $(this).removeClass('map-fullscreen');
            }
            google.maps.event.trigger(map, "resize");
            var latlng = new google.maps.LatLng(document.getElementById('maplat').value, document.getElementById('maplng').value);
            map.setCenter(latlng);
        });
        /***************** map full screen button function *****************/
    }
    $scope.initialize();


    $scope.mapReset = function () {
        var latlong = new google.maps.LatLng(27.700769, 85.300140);
        map.setCenter(latlong);
        map.setZoom(10);
        marker.setPosition(latlong);
        $('#maplat').html(27.700769);
        $('#maplng').html(85.300140);
        google.maps.event.trigger(document.getElementById('map-canvas'), 'resize');
    }

    //add/update
    $scope.dealerCreate = function (isValid) {
        if (!isValid) {
            displayPopupNotification("Invalid Field", "warning");
            return;
        }
        var selectedCustomer = $("#PartyTypeMultiSelect").data("kendoMultiSelect").value();
        var selectedArea = $scope.selectedArea;
        var lat = $('#maplat').html();
        var long = $('#maplng').html();
        var data = {
            DEALER_CODE: selectedCustomer[0],
            AREA_Code: $scope.selectedArea[0],
            ADDRESS: $scope.officeAddress,
            CONTACT_NO: $scope.contactNo,
            EMAIL: $scope.email,
            PAN_NO: $scope.panNo,
            VAT_NO: $scope.vatNo,
            LATITUDE: $('#maplat').html(),
            LONGITUDE: $('#maplng').html(),
        }

        if ($scope.saveAction == "Update") //update mode
        {
            DistSetupService.UpdateDealer(data).then(function (result) {
                if (result.data.STATUS_CODE === 200) {
                    displayPopupNotification(result.data.MESSAGE, "success");
                    $("#grid").data("kendoGrid").dataSource.read();
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
            DistSetupService.AddDealer(data).then(function (result) {
                if (result.data.STATUS_CODE === 200) {
                    displayPopupNotification(result.data.MESSAGE, "success");
                    $("#grid").data("kendoGrid").dataSource.read();
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


    //bind
    $scope.areaSelectOptions = {
        dataTextField: "AREA_NAME",
        dataValueField: "AREA_CODE",
        height: 600,
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
        }
    };
    $scope.partyTypeSelectOptions = {
        dataTextField: "PARTY_TYPE_EDESC",
        dataValueField: "PARTY_TYPE_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Dealer...</strong></div>',
        placeholder: "Select Dealer...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/UserSetup/GetPartyType",
                    dataType: "json"
                }
            }
        }
    };
    $scope.grid = {
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetDealerList",
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
            //serverFiltering: false,
            //serverAggregates: true,
            //schema: {
            //    data: "VisitSummaryViewModel", // records are returned in the "data" field of the response
            //    total: "total", // total number of records is in the "total" field of the response
            //    aggregates: "AggregationResult",
            //},
            model: {
                fields: {
                    // ASSIGN_DATE: { type: "date" },
                }
            },
            //serverPaging: false,
            //serverSorting: false,
            pageSize: reportConfig.defaultPageSize,
        },
        toolbar: kendo.template($("#toolbar-template").html()),
        excel: {
            fileName: "Dealer Setup",
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
                SaveReportSetting('DealerSetup', 'grid');
        },
        columnHide: function (e) {
            if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                SaveReportSetting('DealerSetup', 'grid');
        },
        pageable: {
            refresh: true,
            pageSizes: reportConfig.itemPerPage,
            buttonCount: 5
        },
        pageable:true,
        scrollable: {
            virtual: true
        },
        dataBound: function (o) {
            GetSetupSetting("DealerSetup");
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

            UpdateReportUsingSetting("DealerSetup", "grid");
            $('div').removeClass('.k-header k-grid-toolbar');
        },
        columns: [
             {
                 field: "DEALER_CODE",
                 title: "Code",
                 width: "8%"
             },
             {
                 field: "DEALER_NAME",
                 title: "Dealer Name",
             },
             {
                 field: "ACC_CODE",
                 title:"Acc Code"
             },
               {
                   field: "ADDRESS",
                   title: "Address",
               },
             {
                 field: "CONTACT_NO",
                 title: "Contact No",
             },
              {
                  field: "EMAIL",
                  title: "Email",
              },
               {
                   field: "PAN_NO",
                   title: "Pan No",
               },
                {
                    field: "VAT_NO",
                    title: "Vat No",
                },
              {
                  field: "AREA_Name",
                  title: "Area",
              },
               {
                   field: "ISACTIVE",
                   title: "Status",
                   template: "#= ISACTIVE == 'Y' ? 'Active' : 'InActive' # ",
                   width: "9%"
               },
               {
                   title: "Action",
                   template: " <a class='fa fa-edit editAction' ng-click='UpdateClickEvent($event)' title='Edit'></a>&nbsp &nbsp<a class='fa fa-trash-o deleteAction' ng-click='deleteDealer($event)' title='delete'></a> ",
                   width:"9%"
               }



        ]

    }


    //events
    $scope.AddClickEvent = function () {
        $("#PartyTypeMultiSelect").data("kendoMultiSelect").value([]);
        $("#PartyTypeMultiSelect").data("kendoMultiSelect").enable();
        $("#areaMultiSelect").data("kendoMultiSelect").value([]);

        $scope.pageName = "Add Dealer";
        $scope.saveAction = "Save";

        $scope.createPanel = true;
        //update map   
        var latlong = new google.maps.LatLng(27.700769, 85.300140);
        map.setCenter(latlong);
        marker.setPosition(latlong);
        $('#maplat').html(27.700769);
        $('#maplng').html(85.300140);
       
        setTimeout(function () {
            google.maps.event.trigger(document.getElementById('map-canvas'), 'resize');
            map.setCenter(latlong);
        });

    }
    $scope.UpdateClickEvent = function (evt) {
        
        var grid = $("#grid").data("kendoGrid");
        var row = $(evt.target).closest("tr");
        var item = grid.dataItem(row);
        
        var selectedArea = $.grep($("#areaMultiSelect").data("kendoMultiSelect").dataSource.data(), function (element, index) {
            return element.AREA_NAME == item.AREA_Name;
        });


        $("#PartyTypeMultiSelect").data("kendoMultiSelect").value([item.DEALER_CODE]);
        $("#PartyTypeMultiSelect").data("kendoMultiSelect").readonly();
        $("#areaMultiSelect").data("kendoMultiSelect").value([selectedArea[0].AREA_CODE]);
        $scope.officeAddress = item.ADDRESS;
        $scope.contactNo = parseInt(item.CONTACT_NO);
        $scope.email = item.EMAIL;
        $scope.panNo = item.PAN_NO;
        $scope.vatNo = item.VAT_NO;


        $scope.selectedArea = [selectedArea[0].AREA_CODE];
        $scope.selectedDealer = [item.DEALER_CODE];
        $scope.pageName = "Update Dealer";
        $scope.saveAction = "Update";

        //update map   
        var latlong = new google.maps.LatLng(item.LATITUDE, item.LONGITUDE);
        map.setCenter(latlong);
        marker.setPosition(latlong);
        $('#maplat').html(item.LATITUDE);
        $('#maplng').html(item.LONGITUDE);

        $scope.createPanel = true;

    }

    $scope.deleteDealer = function (evt) {
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
                    var row = $(evt.target).closest("tr");
                    var item = grid.dataItem(row);

                    var DEALER_CODE = item.DEALER_CODE;
                    var DEALER_NAME = item.DEALER_NAME;
                    var data = {
                        DEALER_CODE: DEALER_CODE,
                        DEALER_NAME: DEALER_NAME
                    }

                    var deleteDealer = DistSetupService.deleteDealer(data);
                    deleteDealer.then(function (response) {
                        
                        if (response.data.STATUS_CODE == 300) {
                            displayPopupNotification(response.data.MESSAGE, "Success")
                            $("#grid").data("kendoGrid").dataSource.read();
                        }
                        else {
                            displayPopupNotification(response.data.MESSAGE, "error")
                        }
                    });
                }
            }
        });
    }

    $scope.cancelClickEvent = function () {
        $scope.createPanel = false;
        $scope.officeAddress = "";
        $scope.contactNo = "";
        $scope.email = "";
        $scope.panNo = "";
        $scope.vatNo = "";
        $scope.selectedArea = "";
        $scope.selectedDealer = "";
    }

});
