
var OutletMap = {
    map: "",
    bounds: "",
    mapOptions: {
        mapTypeControlOptions: {
            mapTypeIds: ['custom', google.maps.MapTypeId.ROADMAP, google.maps.MapTypeId.TERRAIN]
        },
        mapTypeControl: false,
        zoomControl: true,
        zoomControlOptions: {
            style: google.maps.ZoomControlStyle.SMALL,
            position: google.maps.ControlPosition.TOP_LEFT
        },
        streetViewControl: false,
        zoom: 7,
        //minZoom: 7,
        center: { lat: 28.43443095421775, lng: 84.20351049414055 },
        mapTypeId: google.maps.MapTypeId.ROADMAP,
    },
    styleOptions: [
        {
            featureType: "administrative",
            elementType: "all",
            stylers: [
                { visibility: "off" }
            ]
        }, {
            featureType: "landscape",
            elementType: "all",
            stylers: [
                { visibility: "off" }
            ]
        }, {
            featureType: "poi",
            elementType: "all",
            stylers: [
                { visibility: "off" }
            ]
        }, {
            featureType: "road",
            elementType: "all",
            stylers: [
                { visibility: "off" }
            ]
        }, {
            featureType: "transit",
            elementType: "all",
            stylers: [
                { visibility: "off" }
            ]
        }, {
            featureType: "water",
            elementType: "all",
            stylers: [
                { visibility: "off" }
            ]
        }
    ],
    infowindow: new google.maps.InfoWindow(),
    markerClusterer: null,
    markerOptions: {
        fitBounds: false,
        animate: true
    },
    init: function (interval) {

        //map = new google.maps.Map(document.getElementById('Area-map-canvas-visit'), {
        //    center: { lat: -34.397, lng: 150.644 },
        //    zoom: 8
        //});
        //check for the interval parameter
        interval = typeof interval !== 'undefined' ? interval : 150000;
        //initialize the map
        var mapDiv = document.getElementById('outlet-map-canvas');
        this.bounds = new google.maps.LatLngBounds();

        this.map = new google.maps.Map(mapDiv, this.mapOptions);

        this.map.mapTypes.set('custom', new google.maps.StyledMapType(this.styleOptions, { name: 'Distribution1' }));
        this.map.controls[google.maps.ControlPosition.TOP_LEFT].push(document.getElementById("outlet-visit-map-fullscreen"));


        //make initial service call
        OutletMap.callWebService();



        $('#outlet-visit-map-fullscreen').on('click', function () {
            $('body').css({
                overflow: 'hidden'
            });

            var mapCenter = OutletMap.map.getCenter();
            if (!$(this).is('.map-fullscreen')) {

                $('#outlet-map-console').css({
                    position: "fixed",
                    //top: $('.navbar-static-top').height() + 'px',
                    top: 0,
                    left: 0,
                    height: $(window).height(),// - $('.navbar-static-top').height() + 'px', //"100%",
                    width: "100%",
                    zIndex: 9999,
                    margin: "0 auto"
                });
                $(this).addClass('map-fullscreen');
            }
            else {

                $('#outlet-map-console').css({
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
            google.maps.event.trigger(OutletMap.map, "resize");
            OutletMap.map.setCenter(mapCenter);
        });

        //  OutletMap.bindSearchEntityEvent();

    },
    callWebService: function () {

        var type = "";
        var SelectedRes = $("#ResellerMultiSelect").data("kendoMultiSelect").value();
        var SelectedDist = $("#DistributorMultiSelect").data("kendoMultiSelect").value();

        if (SelectedRes.length > 0 && SelectedDist.length > 0)
            type = "RD";
        else if (SelectedRes.length > 0)
            type = "R";
        else if (SelectedDist.length > 0)
            type = "D";

        var report = ReportFilter.filterAdditionalData();
        report.ReportFilters.CustomerFilter = $("#SalesMultiSelect").data("kendoMultiSelect").value();
        console.log(report);
        $.ajax({
            url: window.location.protocol + "//" + window.location.host + "/api/Report/GetAllOutleet?type=" + type,
            type: "POST",
            data: JSON.stringify(report),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                console.log(response);
                // var  = new MarkerClusterer(OutletMap.map, OutletMap.AreaentityMarkerCluster);
                /// ... later on
                if (OutletMap.AreaentityMarkerClusterNew instanceof MarkerClusterer) {
                    OutletMap.AreaentityMarkerClusterNew.clearMarkers();
                }
                OutletMap.AreaentityMarkerClusterNew = [];
                for (var eid in OutletMap.OutletMapentities) {
                    OutletMap.OutletMapentities[eid].setMap(null);
                }
                OutletMap.AreaentityMarkerCluster = [];
                OutletMap.OutletMapentities = [];
                OutletMap.distAreaMarkers = [];
                // OutletMap.entityMarkers = [];

                for (var i = 0; i < response.length; i++) {
                    OutletMap.distAreaMarkers[i] = response[i];
                    //DistMap.OutletMapentities[response[i].CODE] = response[i];

                }

                //finally set the distributor markers in the map
                //DistMap.entities = distMarkers;
                //add the markers to the map
                OutletMap.invalidateMarkers();
            },
            error: function () {

            }
        });

        //fetch the distributors and resellers list from the web server
        //$.post(window.location.protocol + "//" + window.location.host + "/api/Distribution/GetVistitedPlain", JSON.stringify({ "companyCode": '05' }),
        //    function (response) {

        //    });
    },
    distAreaMarkers: [],
    OutletMapentities: [],
    AreaentityMarkers: {},
    AreaentityMarkerCluster: [],
    AreaentityMarkerClusterNew: [],
    mrTraceRoute: {},


    addMarkers: function () {


        //var icon = baseUrl + "/blue_marker.png";
        //var iconPink = baseUrl + "/pink_marker.png";
        //var ActiveIcon = baseUrl + "/marker_visited.png";
        var icon = baseUrl + "/5-01-big.png";
        var iconPink = baseUrl + "/7-01-big.png";
        var ActiveIcon = baseUrl + "/6-01-big.png";


        for (var eid in this.distAreaMarkers) {
            var position = new google.maps.LatLng(this.distAreaMarkers[eid].Latitude, this.distAreaMarkers[eid].Longitude);

            this.bounds.extend(position);
            var marker = new google.maps.Marker({
                map: OutletMap.map,
                title: this.distAreaMarkers[eid].Name,
                snippet: this.distAreaMarkers[eid].Type,
                position: position,
                content: this.distAreaMarkers[eid]
            });
            if (this.distAreaMarkers[eid].Type == "R") {
                if (this.distAreaMarkers[eid].ACTIVE == "Y") {
                    marker.setIcon(ActiveIcon);
                }
                else if (this.distAreaMarkers[eid].ACTIVE == "N") {
                    marker.setIcon(iconPink);
                }
                else {
                    marker.setIcon(iconPink);
                }

            }
            else {
                marker.setIcon(icon);
            }
            this.OutletMapentities.push(marker);
            this.OutletMapentities[eid].addListener('click', function () {

                var entity = this.content;
                var contentString = "";
                contentString = contentString += 'Name: ' + entity.Name + ' <br> Address: ' + entity.Address + '<br> Area Name: ' + entity.AreaName + '<br> Created By' + entity.CREATED_BY + '<br> Created Date' + entity.CREATED_DATE + '<br> Active:- ' + entity.ACTIVE + '<br> Entity Type:' + entity.Type;
                OutletMap.infowindow.setContent('<p>' + contentString + '</p>');

                OutletMap.infowindow.open(OutletMap.map, this);
                $.ajax({
                    url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetPhotoInfo?entityTye=" + entity.Type + "&entityCode=" + entity.code + "&companyCode=06",
                    type: "POST",
                    //data: JSON.stringify(report),
                    dataType: "json",
                    traditional: true,
                    contentType: "application/json; charset=utf-8",
                    success: function (response) {

                        $("#loadimage").html('');
                        $(".gride-2").show();
                        if ($("#toggle-switcher").hasClass('opened')) {
                            $(".gride-2").hide();
                            $("#toggle-switcher").removeClass('opened');
                            $('#style-switcher').animate({ 'left': '-40px', 'width': '37px' });
                        } else {
                            $("#toggle-switcher").addClass('opened');
                            $(".gride-2").show();
                            $('#style-switcher').animate({ 'left': '-331px', 'width': '350px' });
                        }



                        //for detail info
                        $(".custType").html(entity.Type);
                        $(".uploderdetails .name").html(entity.Name);
                        $(".uploderdetails .address").html(entity.Address);
                        $(".uploderdetails .contact").html(entity.CONTACT);
                        $(".uploderdetails .pan").html(entity.PAN);

                        //for tile
                        $("#totalSalesCounter").attr("data-value", response.distCustomerInfo.TOTAL_SALES).html(response.distCustomerInfo.TOTAL_SALES);
                        $("#monthlySalesCounter").attr("data-value", response.distCustomerInfo.MONTH_SALES).html(response.distCustomerInfo.MONTH_SALES);
                        $("#totalCollectionCounter").attr("data-value", response.distCustomerInfo.TOTAL_COLLECTION).html(response.distCustomerInfo.TOTAL_COLLECTION);
                        $("#remainingPoCounter").attr("data-value", response.distCustomerInfo.REMAINING_PO).html(response.distCustomerInfo.REMAINING_PO);
                        //$(".salesCounterUpProgress > span").css({ "width": "80%" });
                        //$(".salesCounterUpProgress").next().find("status-number").html(" 80% ");

                        //for table
                        $("#itemsTable tbody").find("tr").remove();
                        for (let i = 0; i < response.distCustomerInfo.ITEMS.length; i++) {
                            var Data = response.distCustomerInfo.ITEMS[i];
                            var markup = "<tr><td>" + Data.ORDER_DATE + "</td><td>" + Data.ITEM_EDESC + "</td><td>" + Data.QUANTITY + "</td><td>" + Data.TOTAL_PRICE + "</td></tr>";
                            $("#itemsTable tbody").append(markup);
                        }

                        //for image
                        for (var i = 0; i < response.photoInfo.length; i++) {
                            var htmldata = $.parseHTML('<div class="contain-box">' +
                                ' <div class="name-title">' +
                                '  <h5>Description :' + response.photoInfo[i].DESCRIPTION + '  <span>Uploaded By:' + response.photoInfo[i].USER_NAME + '</span> </h5>' +
                                ' </div>' +
                                '<div class="contain-image-box"> <img src="' + response.photoInfo[i].FILENAME + '" alt=""> </div></div>');
                            var distributor = response.photoInfo[i];
                            $("#loadimage").append(htmldata);

                        };

                        $("#images").append();

                    },
                    error: function () {

                    }
                });
            });

            this.AreaentityMarkerCluster.push(marker);
        }
        if (OutletMap.AreaentityMarkerCluster.length > 0) {
            OutletMap.AreaentityMarkerClusterNew = new MarkerClusterer(OutletMap.map, OutletMap.AreaentityMarkerCluster, {
                maxZoom: 12
            });
        }


    },

    //remove markers from the map but not from the array


    invalidateMarkers: function () {
        //remove the previous markers from the map and repopulate with new ones
        //this.removeMarkers();

        this.addMarkers();
        this.markerOptions = { fitBounds: false, animate: false };
    },


};
//google.maps.event.addDomListener(window, 'resize', function () {
//    //if (DistMap.map == "")
//    //    $(window).trigger('load');
//    ////    DistMap.init(30000);
//    google.maps.event.trigger(DistMap.map, "resize");
//    OutletMap.map.setCenter(new google.maps.LatLng(DistMap.mapOptions.center.lat, DistMap.mapOptions.center.lng));
//});

function initialize() {
    console.log("abc");
    OutletMap.init(30000);

    //google.maps.event.addListenerOnce(OutletMap.map, 'idle', function () {
    //    //loaded fully




    //});
}
initialize();





