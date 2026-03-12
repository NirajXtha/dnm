function callMRWebService() {

    //fetch the distributors and resellers list from the web server
    $.post(window.location.protocol + "//" + window.location.host + "/api/Distribution/GetMrVistitedPlain",
        function (response) {
            //get the response object
        
                //get the list of distributors
                // console.log(responseObject.result.);
                var distMarkers = new Array();
                //this j will set the index of the resellers in the distMarkers array
                if ("undefined" != typeof response && "object" == typeof response) {
                    for (var i = 0; i < response.length; i++) {
                        var entity = response[i];
                        var spcode = response[i].SP_CODE;
                        MRMap.entities[spcode] = response[i];

                        if("undefined" != typeof MRMap.mrTraceRoute[spcode] && "undefined" != typeof MRMap.mrTraceRoute[spcode].polyline && MRMap.mrTraceRoute[spcode].polyline instanceof google.maps.Polyline) {
                            var oldPolylineCoords = MRMap.mrTraceRoute[spcode].polyline.getPath().getArray();
                            var oldPolylineCoordsLength = oldPolylineCoords.length;
                            var newPolylineCoords = response[i].TRACE;
                            var newTrace = newPolylineCoords.slice(oldPolylineCoordsLength, newPolylineCoords.length);

                            //var newTrace = onlyInFirstMyObject(a,b).concat(onlyInFirstMyObject(b,a));
                            for(var j in newTrace) {
                                var traceObj = newTrace[j];
                                var mpos = new google.maps.LatLng(traceObj.LATITUDE, traceObj.LONGITUDE);
                                MRMap.mrTraceRoute[spcode].polyline.getPath().push(mpos);

                                var marker = addTraceMarker(MRMap.map, mpos, oldPolylineCoordsLength++);
                                // console.log(marker);
                                entity.ACTIVITY_TIME = entity.SUBMIT_DATE;
                                addTraceMarkerEvent(marker, entity);
                                MRMap.mrTraceRoute[spcode]['marker'].push(marker);
                            }
                        }
                    }
                }
                MRMap.invalidateMarkers();
           
        });
}

var MRMap = {
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
        center: {lat: 27.6737416, lng: 85.3140083},
        mapTypeId: google.maps.MapTypeId.ROADMAP,
    },
    styleOptions: [
        {
            featureType: "administrative",
            elementType: "all",
            stylers: [
                {visibility: "off"}
            ]
        }, {
            featureType: "landscape",
            elementType: "all",
            stylers: [
                {visibility: "off"}
            ]
        }, {
            featureType: "poi",
            elementType: "all",
            stylers: [
                {visibility: "off"}
            ]
        }, {
            featureType: "road",
            elementType: "all",
            stylers: [
                {visibility: "off"}
            ]
        }, {
            featureType: "transit",
            elementType: "all",
            stylers: [
                {visibility: "off"}
            ]
        }, {
            featureType: "water",
            elementType: "all",
            stylers: [
                {visibility: "off"}
            ]
        }
    ],
    infowindow: new google.maps.InfoWindow(),
    mriw: {},
    markerClusterer: null,
    markerOptions: {
        fitBounds: false,
        animate: true
    },
    init: function (interval) {
        //check for the interval parameter
        interval = typeof interval !== 'undefined' ? interval : 300000;
        //initialize the map
        var mapDiv = document.getElementById('reportmap-mr-canvas-visit');
        this.bounds = new google.maps.LatLngBounds();

        this.map = new google.maps.Map(mapDiv, this.mapOptions);

        //this.map.mapTypes.set('custom', new google.maps.StyledMapType(this.styleOptions, {name: 'Distribution'}));
        this.map.controls[google.maps.ControlPosition.TOP_RIGHT].push(document.getElementById("reportvisit-map-mr-legend-wrapper"));
        this.map.controls[google.maps.ControlPosition.TOP_LEFT].push(document.getElementById("reportvisit-map-mr-fullscreen"));
        this.map.controls[google.maps.ControlPosition.TOP_CENTER].push(document.getElementById("reportvisit-plan-mr-search"));
        this.map.controls[google.maps.ControlPosition.LEFT_TOP].push(document.getElementById("reportvisit-map-mr-report-filter"));

        //make initial service call
       callMRWebService();

        //set the interval for the webservice to be called
        //var intervals = setInterval(callMRWebService, interval);

       $('#reportvisit-map-mr-fullscreen').on('click', function() {
            $('body').css({
                overflow: 'hidden'
            });

            var mapCenter = MRMap.map.getCenter();
            if (! $(this).is('.map-fullscreen') ) {
                $('#reportvisit-plan-mr-search').css({top: 0, zIndex: 2});
                $('#reportvisit-map-mr-legend-wrapper').css({marginTop: 0, zIndex: 2});

                $('#reportvisit-map-mr-panel').css({
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
                $('#reportvisit-plan-mr-search').css({zIndex: 1});
                $('#reportvisit-map-mr-legend-wrapper').css({zIndex: 1});
                $('#reportvisit-map-mr-panel').css({
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
            google.maps.event.trigger(MRMap.map, "resize");
            MRMap.map.setCenter(mapCenter);
        });

        this.bindSalepersonSearchEntityEvent();

    },

    entities: {},
    entityMarkers: {},
    amrid: "",
    mrTraceRoute: {},
    entityMarkerCluster: [],
    bindSalepersonSearchEntityEvent: function() {
        $('#reportmr-search-target').on('keyup', function(e) {
            $('#reportvisit-mr-search-result').show();
            var keyCode = e.keyCode || e.which;
            //if ( 13 == keyCode ) {
            //    e.preventDefault();
            //}

            var searchStatus = false;
            $('#reportvisit-mr-search-result').html('');
            var result = {};
            var queryStr = $(this).val();
            for(var eid in MRMap.entities) {
                var isFound = false;
                var entity = MRMap.entities[eid];
                var searchStr = entity.EMPLOYEE_EDESC.trim();
                if (entity.CONTACT_NO && entity.CONTACT_NO.trim()) {
                    searchStr += ", " + entity.CONTACT_NO.trim();
                }
                if (entity.REG_OFFICE_ADDRESS && entity.REG_OFFICE_ADDRESS.trim()) {
                    searchStr += ", " + entity.REG_OFFICE_ADDRESS.trim();
                }

                //console.log(searchStr);
                var reg = new RegExp(queryStr, 'gi');
                var rs = searchStr.replace(reg, function(str) {
                    isFound = true;
                    return '<b>'+str+'</b>';
                });

                if (isFound) { result[eid] = rs; }
            }

            $.each(result, function(k,v) {
                $('#reportvisit-mr-search-result').append('<div id="rs-' + k + '" class="search-result-row" tabindex="-1" style="padding:5px;border-bottom:1px solid #eee;cursor:pointer">' + v + '</div>');
            });

            return true;
        });
        $('#reportmr-search-target').on('click', function(e) {
            e.stopImmediatePropagation();
            e.stopPropagation();
            $('#reportvisit-mr-search-result').show();
        });

        $('#reportvisit-mr-search-result').delegate('.search-result-row', 'click', function(e) {
            e.stopImmediatePropagation();
            e.stopPropagation();
            // Get the corresponding text and set the value
            var mrid = $(this).attr('id').replace('rs-', '');
            var entity = MRMap.entities[mrid];
            var selectedValue = entity.EMPLOYEE_EDESC.trim();
            $('#reportmr-search-target').val(selectedValue);

            // Display the marker
            var entityMarker = MRMap.entityMarkers[$(this).attr('id').replace('rs-', '')];
            //MRMap.map.panTo(entityMarker.getPosition());
            MRMap.map.setCenter(entityMarker.getPosition());
            MRMap.map.setZoom(16);
            google.maps.event.trigger(entityMarker, 'click', entityMarker);
            $('#reportvisit-mr-search-result').hide();
            $('#reportvisit-map-mr-report-filter').show();

            MRMap.amrid = mrid;
            // If date is already filtered, fetch the trace
            var filterDate = $('#reportvisit-map-mr-report-filter-date-inp').val();
            if (filterDate) {
                $('#reportvisit-map-mr-detail-report').trigger('click', {fiterDate: filterDate});
            }
        });

    },
    addMarkers: function () {
        var currentClicked = null;

        var totalSalesPerson = 0;
        //load distributors onto the map
        //for( i = 0, length=Object.size(this.entities); i < length; i++ ) {
        for (var eid in this.entities) {
            totalSalesPerson++;

            var searchStr = this.entities[eid].EMPLOYEE_EDESC.trim();
            if (this.entities[eid].CONTACT_NO && this.entities[eid].CONTACT_NO.trim()) {
                searchStr += ", " + this.entities[eid].CONTACT_NO.trim();
            }
            if (this.entities[eid].REG_OFFICE_ADDRESS && this.entities[eid].REG_OFFICE_ADDRESS.trim()) {
                searchStr += ", " + this.entities[eid].REG_OFFICE_ADDRESS.trim();
            }

            var title = this.entities[eid].EMPLOYEE_EDESC;
            var lat = Number(this.entities[eid].LATITUDE);
            var lng = Number(this.entities[eid].LONGITUDE);

            var attendaceType = this.entities[eid].Track_type;
            var iconPink = baseUrl + "/man-standing-up.png";
            var iconVisit = baseUrl + "/man-standing-up.png";

            if (attendaceType == "EOD") {
                iconPink = baseUrl + "/man-standing-upatt.png";
                iconVisit = baseUrl + "/man-standing-upatt.png";

            }
            else if (attendaceType == "ATN") {
                iconPink = baseUrl + "/man-standing-upatt.png";
                iconVisit = baseUrl + "/man-standing-upatt.png";
            }

            var position = new google.maps.LatLng(lat, lng);
            var spMarker = null;
            if ("undefined" == typeof this.entityMarkers[eid]) {
                $('#reportvisit-mr-search-result').append('<div id="rs-' + eid + '" class="search-result-row" tabindex="-1" style="padding:5px;border-bottom:1px solid #eee;cursor:pointer">' + searchStr + '</div>');

                this.bounds.extend(position);
                spMarker = new google.maps.Marker({
                    map: MRMap.map,
                    title: title,
                    icon: iconVisit,
                    content: this.entities[eid]
                });

                this.entityMarkerCluster.push(spMarker);

                this.entityMarkers[eid] = spMarker;

                //set the listener
                this.entityMarkers[eid].addListener('click', function () {

                    var entity = this.content;


                    var contentString = '<b>' + entity.EMPLOYEE_EDESC.trim() + '</b>';
                    if (entity.CONTACT_NO && entity.CONTACT_NO.trim()) {
                        contentString += "<br/>Contact: " + entity.CONTACT_NO.trim();
                    }
                    if (entity.REG_OFFICE_ADDRESS && entity.REG_OFFICE_ADDRESS.trim()) {
                        contentString += "<br/>" + entity.REG_OFFICE_ADDRESS.trim();
                    }
                    if (entity.SUBMIT_DATE && entity.SUBMIT_DATE.trim()) {
                        contentString += "<br/>Last Seen: " + entity.SUBMIT_DATE.trim();
                    }
                    if (entity.FILENAME != "" || entity.FILENAME != null) {
                        contentString += "<div class='contain-image-box'> <img src='" + entity.ImageFullPath+"'></div>";
                    }
                    MRMap.infowindow.setContent('<p>' + contentString + '</p>');
                    MRMap.infowindow.entity = entity;

                    MRMap.infowindow.open(MRMap.map, this);

                    currentClicked = this;

                });

                this.entityMarkers[eid].addListener('mouseover', function () {
                    this.setOptions({
                        icon: iconPink
                    });
                    google.maps.event.trigger(this, "click");
                });
                this.entityMarkers[eid].addListener('mouseout', function () {
                    this.setOptions({
                        icon: iconVisit
                    });

                    google.maps.event.trigger(this, "click");
                    setTimeout(function () {
                        if (MRMap.infowindow && MRMap.infowindow instanceof google.maps.InfoWindow) {
                            MRMap.infowindow.close();
                        }
                    }, 3000);
                });
            }
            else {
                spMarker = this.entityMarkers[eid];
            }
            spMarker.setPosition(position);

        }

        //if (MRMap.entityMarkerCluster.length > 0 && null == MRMap.markerClusterer) {
        //    MRMap.markerClusterer = new MarkerClusterer(MRMap.map, MRMap.entityMarkerCluster, {
        //        maxZoom: 15
        //    });
        //}

        $('#reportmr-count').text(totalSalesPerson);

        //if (MRMap.markerOptions.fitBounds == true) {
        //    MRMap.map.fitBounds(MRMap.bounds);
        //}

    },
    removeEntities: function () {
        this.entities = [];
    },
    //remove markers from the map but not from the array
    removeMarkers: function () {
        for (var i = 0; i < this.entityMarkers.length; i++) {
            this.entityMarkers[i].setMap(null);
        }

        this.entityMarkers = [];
    },
    invalidateMarkers: function () {
        //remove the previous markers from the map and repopulate with new ones
        //this.removeMarkers();
        this.addMarkers();
        this.markerOptions = {fitBounds: false, animate: false};
    },

};

google.maps.event.addDomListener(window, 'resize', function() {
    google.maps.event.trigger(MRMap.map, "resize");
    MRMap.map.setCenter(new google.maps.LatLng(MRMap.mapOptions.center.lat, MRMap.mapOptions.center.lng));
});

// var mrTrace = {};
google.maps.event.addDomListener(window, "load", function () {
    //initilize map here
    //MRMap.init(300000);

    //google.maps.event.addListener(MRMap.map, 'bounds_changed', function() {
    //    $('.trace-marker-label:visible').each(function() {
    //        var w = $(this).outerWidth();
    //        $(this).css({
    //            marginLeft: '-' + (w/2) + 'px'
    //        });
    //    });
    //});

   

});

function initializeMrMap() {
    MRMap.init(300000);
    google.maps.event.addListener(MRMap.map, 'bounds_changed', function() {
        $('.trace-marker-label:visible').each(function() {
            var w = $(this).outerWidth();
            $(this).css({
                marginLeft: '-' + (w/2) + 'px'
            });
        });
    });
    google.maps.event.addListenerOnce(MRMap.map, 'idle', function () {
        //loaded fully
        $('#reportvisit-map-legend-wrapper').show();
        $('#reportvisit-map-fullscreen').show();
        $('#reportvisit-plan-search').show();
        //$('#visit-map-mr-report-filter').show();

        // var attachHandlers = $.datepicker._attachHandlers;
        // var generateHTML = $.datepicker._generateHTML;
        // $.datepicker._attachHandlers = function (inst) {
        //     // call the cached function in scope of $.datepicker object
        //     attachHandlers.call($.datepicker, inst);
        //     // add custom stuff
        //     inst.dpDiv.find("[data-handler]").map(function () {
        //         var handler = {
        //             clear: function () {
        //                 var id = "#" + inst.id.replace(/\\\\/g, "\\");
        //                 $.datepicker._clearDate(id);
        //                 $.datepicker._hideDatepicker();
        //             }
        //         };
        //         if (handler[this.getAttribute("data-handler")]) {
        //             $(this).bind(this.getAttribute("data-event"), handler[this.getAttribute("data-handler")]);
        //         }
        //     });
        // };
        // $.datepicker._generateHTML = function (inst) {
        //     //call the cached function in scope of $.datepicker object
        //     var html = generateHTML.call($.datepicker, inst);
        //     var $html = $(html);
        //     var $buttonPane = $html.filter("div.ui-datepicker-buttonpane.ui-widget-content");
        //
        //     $buttonPane.append($("<button />")
        //         .text("Clear")
        //         .attr("type", "button")
        //         .attr("data-handler", "clear")
        //         .attr("data-event", "click")
        //         .addClass("ui-datepicker-clear ui-state-default ui-priority-secondary ui-corner-all"));
        //
        //     return $html;
        // };

        var oldGoToToday = $.datepicker._gotoToday
        $.datepicker._gotoToday = function (id) {
            oldGoToToday.call(this, id);
            this._selectDate(id);
        }
        $('#reportvisit-map-mr-report-filter-date-inp').datepicker({
            dateFormat: 'dd-mm-yy',
            minDate: '-3D',
            maxDate: 0,
            onSelect: function (dt, inst) {
                // Fetch all the detail path of the selected date and show detail

                // Trigger click of the respective MR
                $('#reportvisit-map-mr-detail-report').trigger('click', { fiterDate: dt });
            },
            showOn: "button",
            showButtonPanel: true,
            beforeShow: function (input, obj) {
                setTimeout(function () {
                    var buttonPane = $(input).datepicker("widget").find(".ui-datepicker-buttonpane");
                    $("#BCL").hide();
                    $("#BTDY").hide();
                    $("<button>", {
                        type: "button",
                        text: "Clear",
                        click: function () {
                            $(input).val('');
                            // Clear the
                            clearMrTrace();
                        }
                    }).addClass('ui-state-default ui-corner-all').appendTo(buttonPane);
                }, 1);
            }
        });

        var traceBounds = new google.maps.LatLngBounds();
        $('#reportvisit-map-mr-detail-report').on('click', function (e, params) {
            e.preventDefault();
            if ("undefined" != typeof MRMap.amrid && MRMap.amrid) {
                var spCode = MRMap.amrid;
                var mrName = $.trim(MRMap.entityMarkers[MRMap.amrid].title);
                console.log(spCode);

                var fiterDate = null;
                if ("undefined" != typeof params) {
                    if ("undefined" != typeof params.fiterDate) {
                        fiterDate = params.fiterDate;
                    }
                }
                $.ajax({
                    url: window.location.protocol + "//" + window.location.host + '/api/Distribution/GetMrVistitedPlainDateWise?date=' + fiterDate + '&spCode=' + spCode,
                    type: "post",
                    dataType: "json",
                    success: function (data) {
                        console.log(data);
                        MRMap.infowindow.close();
                        // Clear the trace from map
                        clearMrTrace();
                        var traceArr = new google.maps.MVCArray();
                        if ("undefined" != typeof data && "undefined" != typeof data) {
                            if (data.length <= 0) {
                                return false;
                            }

                            if ("undefined" == typeof MRMap.mrTraceRoute[spCode]) {
                                MRMap.mrTraceRoute[spCode] = {
                                    "polyline": null,
                                    "marker": [],   //
                                    "interval": 0,
                                    "toids": [], // time out ids
                                    "mmrm": null,  // moving mr marker
                                    "iw": null  // infowindow
                                };
                            }

                            var entityTrackJson = data;
                            $.each(entityTrackJson, function (k, v) {
                                var position = new google.maps.LatLng(Number(v.LATITUDE), Number(v.LONGITUDE));
                                // var marker = new google.maps.Marker({
                                //     map: MRMap.map,
                                //     position: position,
                                //     label: (k+1).toString(),
                                //     icon: {
                                //         path: google.maps.SymbolPath.CIRCLE,
                                //         scale: 10,
                                //         strokeColor: '#008000',
                                //         strokeOpacity: 1,
                                //         strokeWeight: 2
                                //     }
                                // });

                                var marker = addTraceMarker(MRMap.map, position, k);
                                // console.log(marker);
                                v.EMPLOYEE_EDESC = mrName;
                                addTraceMarkerEvent(marker, v);
                                // //add the click listener to the marker
                                // marker.addListener("click", function () {
                                //     //display the info window here for the marker
                                //     var entity = v;
                                //     entity.EMPLOYEE_EDESC = mrName;
                                //     var contentString = '<b>' + mrName + '</b><br/>';
                                //     contentString += '<b><u>Date</u> :   </b>' + entity.ACTIVITY_TIME.trim() + '<br/>';
                                //     contentString += '<b><u>Remarks</u></b><br/>';
                                //     var visitRemarks = $.trim(entity.REMARKS);
                                //     if (visitRemarks == " " || visitRemarks.length < 2) {
                                //         //no remarks available
                                //         contentString += "Not Available"
                                //     }
                                //     else {
                                //         contentString += entity.REMARKS;
                                //     }
                                //
                                //     if (null == MRMap.mrTraceRoute[spCode].iw) {
                                //         MRMap.mrTraceRoute[spCode].iw = new google.maps.InfoWindow({
                                //             disableAutoPan: true
                                //         });
                                //     }
                                //
                                //     // MRMap.infowindow.setContent('<p>' + contentString + '</p>');
                                //     // MRMap.infowindow.entity = entity;
                                //     // MRMap.infowindow.open(MRMap.map, this);
                                //
                                //     MRMap.mrTraceRoute[spCode].iw.setContent('<p>' + contentString + '</p>');
                                //     MRMap.mrTraceRoute[spCode].iw.open(MRMap.map, this);
                                //     setTimeout(function() {
                                //         MRMap.mrTraceRoute[spCode].iw.close();
                                //     }, 3300);
                                //
                                // });
                                // marker.addListener("mouseover", function() {
                                //     google.maps.event.trigger(this, "click");
                                // });
                                MRMap.mrTraceRoute[spCode]['marker'].push(marker);
                                traceArr.push(position);
                                traceBounds.extend(position);
                            });

                            MRMap.mrTraceRoute[spCode]['polyline'] = new google.maps.Polyline({
                                map: MRMap.map,
                                path: traceArr,
                                strokeWeight: 3,
                                strokeColor: '#F75850',
                                strokeOpacity: 1,
                                zIndex: 10,
                                // icons: [{
                                //     icon: {
                                //         path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                                //         sacle: 8,
                                //         strokeColor: '#393'
                                //     },
                                //     offset: '100%'
                                // }],
                                isVisible: true
                            });
                            google.maps.event.addListener(MRMap.mrTraceRoute[spCode]['polyline'], 'mouseover', function (e) {
                                this.setOptions({
                                    strokeWeight: 5,
                                });
                            });
                            google.maps.event.addListener(MRMap.mrTraceRoute[spCode]['polyline'], 'mouseout', function (e) {
                                this.setOptions({
                                    strokeWeight: 3,
                                });
                            });
                            //animateArrow(MRMap.mrTraceRoute[spCode]['polyline']);

                            /***********************************************************/
                            var numOfPadPoints = 10;
                            var polyPoints = MRMap.mrTraceRoute[spCode]['polyline'].getPath().getArray();
                            var paddedPoints = [];
                            //Pad the points array
                            $.each(polyPoints, function (key, pt) {
                                var currentPoint = pt;
                                var nextPoint = polyPoints[key + 1];

                                //Check if we're on the last point
                                if (typeof nextPoint !== 'undefined') {

                                    //Get a 10th of the difference in latitude
                                    var latIncr = (nextPoint.lat() - currentPoint.lat()) / 10;

                                    //Get a 10th of the difference in longitude
                                    var lngIncr = (nextPoint.lng() - currentPoint.lng()) / 10;

                                    //Add the current point to the new padded points array
                                    paddedPoints.push(currentPoint);

                                    //Now add 10 points at latIncr & lngIncr intervals between current and next points
                                    //We add this to the new padded points array
                                    for (var i = 1; i < numOfPadPoints; i++) {
                                        var newPt = new google.maps.LatLng(currentPoint.lat() + (i * latIncr), currentPoint.lng() + (i * lngIncr));
                                        paddedPoints.push(newPt);
                                        if (i == numOfPadPoints) {
                                            paddedPoints.push(nextPoint);
                                        }
                                    }
                                }
                            });
                            paddedPoints.push(polyPoints[polyPoints.length - 1]);

                            MRMap.mrTraceRoute[spCode]['polyline']['mmrm'] = new MarkerWithLabel({
                                map: MRMap.map,
                                position: new google.maps.LatLng(paddedPoints[0].lat(), paddedPoints[0].lng()),
                                // icon: {
                                //     path: google.maps.SymbolPath.CIRCLE,
                                //     scale: 3,
                                //     strokeColor: '#393'
                                // },
                                draggable: false,
                                raiseOnDrag: false,
                                labelContent: mrName,
                                labelAnchor: new google.maps.Point(12, 0),
                                labelClass: "mr-marker-label", // the CSS class for the label
                                labelStyle: { opacity: 0.75 },
                                zIndex: 999
                            });

                            for (var i = 0; i < paddedPoints.length; i++) {
                                MRMap.mrTraceRoute[spCode]['toids'].push(setTimeout(function (cnt, coords) {
                                    if (0 == (cnt % 10)) {
                                        if (cnt / 10 != 0) {
                                            google.maps.event.trigger(MRMap.mrTraceRoute[spCode]['marker'][cnt / 10], "click");
                                        }
                                    }
                                    var latlng = new google.maps.LatLng(coords.lat(), coords.lng());
                                    MRMap.mrTraceRoute[spCode]['polyline']['mmrm'].setPosition(latlng);
                                }, 800 * i, i, paddedPoints[i])
                                );
                            }

                            var a = window.setInterval(function () {
                                clearInterval(a);
                                $('.trace-marker-label:visible').each(function () {
                                    var w = $(this).outerWidth();
                                    $(this).css({
                                        marginLeft: '-' + (w / 2) + 'px'
                                    });
                                });
                            }, 700);
                            MRMap.map.fitBounds(traceBounds);
                        }
                    },
                    error: function (err) {

                    }
                });
            }
        });

        $('.visit-map-mr-cbox').on('change', function (e) {
            e.preventDefault();
            if (this.checked) {
                // Display the info windows
                $.each(MRMap.entities, function (spc, entity) {

                    var contentString = '<b>' + entity.EMPLOYEE_EDESC.trim() + '</b>';
                    if (entity.CONTACT_NO && entity.CONTACT_NO.trim()) {
                        contentString += "<br/>Contact: " + entity.CONTACT_NO.trim();
                    }
                    if (entity.REG_OFFICE_ADDRESS && entity.REG_OFFICE_ADDRESS.trim()) {
                        contentString += "<br/>" + entity.REG_OFFICE_ADDRESS.trim();
                    }
                    if (entity.SUBMIT_DATE && entity.SUBMIT_DATE.trim()) {
                        contentString += "<br/>Last Seen: " + entity.SUBMIT_DATE.trim();
                    }

                    var mliw = new google.maps.InfoWindow();
                    mliw.setContent('<p>' + contentString + '</p>');
                    mliw.open(MRMap.map, MRMap.entityMarkers[spc]);

                    MRMap.mriw[spc] = mliw;
                });
            }
            else {
                // Remove the info windows
                for (var spc in MRMap.mriw) {
                    MRMap.mriw[spc].close();
                }
                MRMap.mriw = {};
            }
        });
    });
}
google.maps.event.addDomListener(window, "click", function () {
    $('#reportvisit-mr-search-result').hide();
});

// Use the DOM setInterval() function to change the offset of the symbol
// at fixed intervals.
function animateArrow(polyline) {
    var count = 0;
    window.setInterval(function() {
        count = (count + 1) % 200;

        var icons = polyline.get('icons');
        icons[0].offset = (count / 2) + '%';
        polyline.set('icons', icons);
    }, 200);
}

function moveMarker(map, marker, latlng) {
    marker.setPosition(latlng);
    map.panTo(latlng);
}

function autoRefresh(map) {
    var i, route, marker;

    route = new google.maps.Polyline({
        path: [],
        geodesic : true,
        strokeColor: '#FF0000',
        strokeOpacity: 1.0,
        strokeWeight: 2,
        editable: false,
        map:map
    });

    marker=new google.maps.Marker({map:map,icon:"http://maps.google.com/mapfiles/ms/micons/blue.png"});

    for (i = 0; i < pathCoords.length; i++) {
        setTimeout(function (coords)
        {
            var latlng = new google.maps.LatLng(coords.lat, coords.lng);
            route.getPath().push(latlng);
            moveMarker(map, marker, latlng);
        }, 200 * i, pathCoords[i]);
    }
}

var onlyInFirst = function(equal, a, b){
    return a.filter(function(current){
        return b.filter(equal(current)).length == 0
    });
}
var onlyInFirstMyObject = onlyInFirst.bind(0, function equal(a){
    return function(b){
        return a.LATITUDE == b.LATITUDE &&
            a.LONGITUDE == b.LONGITUDE
    }
});
//var result = onlyInFirstMyObject(a,b).concat(onlyInFirstMyObject(b,a));

function addTraceMarker(map, position, idx) {
    var marker = new MarkerWithLabel({
        map: map,
        position: position,
        icon: {
            path: google.maps.SymbolPath.CIRCLE,
            scale: 12,
            strokeColor: '#393',
            strokeOpacity: 1,
            strokeWeight: 2
        },
        draggable: false,
        raiseOnDrag: false,
        labelContent: (idx+1).toString(),
        labelAnchor: new google.maps.Point(0, 0),
        labelClass: "trace-marker-label", // the CSS class for the label
        labelStyle: {opacity: 0.75}
    });
    
    return marker;
}

function addTraceMarkerEvent(marker, entity) {
    //add the click listener to the marker
    marker.addListener("click", function () {
        //display the info window here for the marker
        var contentString = '<b>' + entity.EMPLOYEE_EDESC + '</b><br/>';
        contentString += '<b><u>Date</u> :   </b>' + entity.ACTIVITY_TIME.trim() + '<br/>';
        contentString += '<b><u>Remarks</u></b><br/>';
        var visitRemarks = $.trim(entity.REMARKS);
        if (visitRemarks == " " || visitRemarks.length < 2) {
            //no remarks available
            contentString += "Not Available"
        }
        else {
            contentString += entity.REMARKS;
        }

        if (null == MRMap.mrTraceRoute[entity.SP_CODE].iw) {
            MRMap.mrTraceRoute[entity.SP_CODE].iw = new google.maps.InfoWindow({
                disableAutoPan: true
            });
        }

        // MRMap.infowindow.setContent('<p>' + contentString + '</p>');
        // MRMap.infowindow.entity = entity;
        // MRMap.infowindow.open(MRMap.map, this);

        MRMap.mrTraceRoute[entity.SP_CODE].iw.setContent('<p>' + contentString + '</p>');
        MRMap.mrTraceRoute[entity.SP_CODE].iw.open(MRMap.map, this);
        setTimeout(function() {
            MRMap.mrTraceRoute[entity.SP_CODE].iw.close();
        }, 3300);

    });
    marker.addListener("mouseover", function() {
        google.maps.event.trigger(this, "click");
    });
}

function clearMrTrace() {
    var spCode = MRMap.amrid;
    if ("undefined" != typeof MRMap.mrTraceRoute[spCode]) {
        window.clearInterval(MRMap.mrTraceRoute[spCode]['polyline']['interval']);
        for (var x in MRMap.mrTraceRoute[spCode]['toids']) {
            var toid = MRMap.mrTraceRoute[spCode]['toids'][x];
            window.clearTimeout(toid);
        }
        if (MRMap.mrTraceRoute[spCode]['polyline']['mmrm'] instanceof google.maps.Marker) {
            MRMap.mrTraceRoute[spCode]['polyline']['mmrm'].setMap(null);
        }
        // Reset the polyline and marker object
        MRMap.mrTraceRoute[spCode]['polyline'].setMap(null);
        $.each(MRMap.mrTraceRoute[spCode]['marker'], function (c, m) {
            m.setMap(null);
        });
    }
}
