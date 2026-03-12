//function callWebService() {
//    var report=ReportFilter.filterAdditionalData();
//    $.ajax({
//        url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetVistitedPlain",
//        type: "POST",
//        data: JSON.stringify(report),
//        dataType: "json",
//        traditional: true,
//        contentType: "application/json; charset=utf-8",
//        success: function (response) {
//            console.log(response);
//            //get the response object
//            var distMarkers = new Array();
//            //this j will set the index of the resellers in the distMarkers array
//            var j = 0;
//            //get the distributors
//            for (var i = 0; i < response.distributor.length; i++) {
//                distMarkers[i] = response.distributor[i];
//                DistMap.entities[response.distributor[i].CODE] = response.distributor[i];
//                j++;
//            }
//            //now we get the resellers
//            for (var i = 0; i < response.reseller.length; i++) {
//                distMarkers[j] = response.reseller[i];
//                DistMap.entities[response.reseller[i].CODE] = response.reseller[i];

//                j++;
//            }
//            for (var i = 0; i < response.MrVisited.length; i++) {
//                distMarkers[j] = response.MrVisited[i];
//                DistMap.entities[response.MrVisited[i].SP_CODE] = response.MrVisited[i];

//                j++;
//            }
//            //finally set the distributor markers in the map
//            //DistMap.entities = distMarkers;
//            //add the markers to the map
//            DistMap.invalidateMarkers();
//        },
//        error: function () {
          
//        }
//    });
   
//    //fetch the distributors and resellers list from the web server
//    //$.post(window.location.protocol + "//" + window.location.host + "/api/Distribution/GetVistitedPlain", JSON.stringify({ "companyCode": '05' }),
//    //    function (response) {
         
//    //    });
//}

var totalVisit = 0, totalPendingVisit = 0;
var totalCancleVisit = 0;
var totalMrvisit = 0;
var DistMap = {
    salespersonid:0,
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

        //check for the interval parameter
        interval = typeof interval !== 'undefined' ? interval : 150000;
        //initialize the map
        var mapDiv = document.getElementById('map-canvas-visit');
        this.bounds = new google.maps.LatLngBounds();

        this.map = new google.maps.Map(mapDiv, this.mapOptions);

        this.map.mapTypes.set('custom', new google.maps.StyledMapType(this.styleOptions, { name: 'Distribution' }));
        this.map.controls[google.maps.ControlPosition.TOP_LEFT].push(document.getElementById("visit-map-fullscreen"));
        this.map.controls[google.maps.ControlPosition.TOP_RIGHT].push(document.getElementById("visit-map-legend-wrapper"));
        this.map.controls[google.maps.ControlPosition.TOP_CENTER].push(document.getElementById("visit-plan-search"));
        this.map.controls[google.maps.ControlPosition.LEFT_TOP].push(document.getElementById("visit-map-mr-report-filter"));
        //make initial service call
        DistMap.callWebService();

     

        $('#visit-map-fullscreen').on('click', function () {
            $('body').css({
                overflow: 'hidden'
            });

            var mapCenter = DistMap.map.getCenter();
            if (!$(this).is('.map-fullscreen')) {
                $('#visit-plan-search').css({ top: 0, zIndex: 2 });
                $('#visit-map-legend-wrapper').css({ marginTop: 0, zIndex: 2 });

                $('#visit-map-panel').css({
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
                $('#visit-plan-search').css({ zIndex: 1 });
                $('#visit-map-legend-wrapper').css({ zIndex: 1 });
                $('#visit-map-panel').css({
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
            google.maps.event.trigger(DistMap.map, "resize");
            DistMap.map.setCenter(mapCenter);
        });

        DistMap.bindSearchEntityEvent();

    },
    callWebService: function () {
        var report = ReportFilter.filterAdditionalData();

        
        var OutletFilter = $("#outLetMultiSelect").data("kendoMultiSelect").value();
        var DistributorFilter = $("#distributorMultiSelect").data("kendoMultiSelect").value();
        var distModel = {
            OutletFilter : OutletFilter == "" ? [] : OutletFilter,
            DistributorFilter : DistributorFilter == "" ? [] : DistributorFilter
        }
        $.ajax({
            url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetVistitedPlain",
            type: "POST",
            data: JSON.stringify({
                modelPar: report,
                distModel: distModel
            }),
            dataType: "json",
            traditional: true,
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                console.log(response);
                DistMap.bindSearchEntityEvent();
                $("#visit-search-result").html("");
                //get the response object
                var distMarkers = new Array();
                clearMrTrace();
                DistMap.removeMarkers();
                DistMap.entities = [];
                DistMap.entityMarkers = [];

                DistMap.mrTraceRoute = [];
                DistMap.entityMarkerCluster = [];

                //this j will set the index of the resellers in the distMarkers array
                var j = 0;
                //get the distributors
                for (var i = 0; i < response.distributor.length; i++) {
                    distMarkers[i] = response.distributor[i];
                    DistMap.entities[response.distributor[i].CODE] = response.distributor[i];
                    j++;
                }
                //now we get the resellers
                for (var i = 0; i < response.reseller.length; i++) {
                    distMarkers[j] = response.reseller[i];
                    DistMap.entities[response.reseller[i].CODE] = response.reseller[i];

                    j++;
                }
                for (var i = 0; i < response.MrVisited.length; i++) {
                    distMarkers[j] = response.MrVisited[i];
                    DistMap.entities[response.MrVisited[i].SP_CODE] = response.MrVisited[i];

                    j++;
                }
                for (var i = 0; i < response.dealer.length; i++) {
                    distMarkers[j] = response.dealer[i];
                    DistMap.entities[response.dealer[i].CODE] = response.dealer[i];

                    j++;
                }
                
              
                //finally set the distributor markers in the map
                //DistMap.entities = distMarkers;
                //add the markers to the map
                DistMap.invalidateMarkers();
            },
            error: function () {

            }
        });

        //fetch the distributors and resellers list from the web server
        //$.post(window.location.protocol + "//" + window.location.host + "/api/Distribution/GetVistitedPlain", JSON.stringify({ "companyCode": '05' }),
        //    function (response) {

        //    });
    },
    entities: {},
    entityMarkers: {},
    entityMarkerCluster: [],
    mrTraceRoute: {},
  
    bindSearchEntityEvent: function () {
        $('#search-target').on('keyup', function (e) {
            $('#visit-search-result').show();
            var keyCode = e.keyCode || e.which;
            //if ( 13 == keyCode ) {
            //    e.preventDefault();
            //}

            var searchStatus = false;
            $('#visit-search-result').html('');
            var result = {};
            var queryStr = $(this).val();
            for (var eid in DistMap.entities) {
                var isFound = false;
                var searchStr = '';
                var entity = DistMap.entities[eid];
                if (entity.NAME)
                    searchStr = entity.NAME.trim();
                if (entity.ADDRESS && entity.ADDRESS.trim()) {
                    searchStr += ", " + entity.ADDRESS.trim();
                }
                if (entity.TYPE && entity.TYPE.trim()) {
                    searchStr += " (" + (entity.TYPE.charAt(0).toUpperCase() + entity.TYPE.slice(1).toLowerCase()).trim() + ")";
                }
                //console.log(searchStr);
                var reg = new RegExp(queryStr, 'gi');
                var rs = searchStr.replace(reg, function (str) {
                    isFound = true;
                    return '<b>' + str + '</b>';
                });

                if (isFound) { result[eid] = rs; }
            }

            $.each(result, function (k, v) {
                $('#visit-search-result').append('<div id="rs-' + k + '" class="search-result-row" tabindex="-1" style="padding:5px;border-bottom:1px solid #eee;cursor:pointer">' + v + '</div>');
            });

            return true;
        });
        $('#search-target').on('click', function (e) {
            e.stopImmediatePropagation();
            e.stopPropagation();
            $('#visit-search-result').show();
        });
        
        $(".visit-click").on("click", function (e) {
            e.stopImmediatePropagation();
            e.stopPropagation();
            var bounds = new google.maps.LatLngBounds();
            $.each(DistMap.entityMarkers, function (index, node) {

                if (node.content.LAST_VISIT_STATUS == 'Y') {
                    node.setVisible(true);
                    bounds.extend(node.getPosition());
                    DistMap.map.fitBounds(bounds);
                    //DistMap.map.setCenter(node.getPosition());
                    //  google.maps.event.trigger(node, 'click', node);
                }
                else {
                    node.setVisible(false);
                    // DistMap.map.setMap(null)
                }
              
            });
        });
        $(".cancle-visit-click").on("click", function (e) {
            console.log("test");
            e.stopImmediatePropagation();
            e.stopPropagation();
            if (totalCancleVisit <= 0)
            {
                return false;
            }
            var bounds = new google.maps.LatLngBounds();
            $.each(DistMap.entityMarkers, function (index, node) {
                var checkentitylength = 0;
               
                if (node.content.LAST_VISIT_STATUS == 'N') {
                    checkentitylength++;
                    node.setVisible(true);
                    bounds.extend(node.getPosition());
                   

                    //DistMap.map.setCenter(node.getPosition());
                    //DistMap.map.setZoom(10);
                    //DistMap.map.setCenter(node.getPosition());
                    //  google.maps.event.trigger(node, 'click', node);
                }
                else {
                    node.setVisible(false);
                    // DistMap.map.setMap(null)
                }
                if(checkentitylength==1)
                {
                   // DistMap.map.setZoom(bounds[0]);
                    DistMap.map.setCenter(bounds[0]);
                    DistMap.map.setZoom(16);
                }
                else {
                    DistMap.map.fitBounds(bounds);
                }
             
            });
        });
        $(".pending-visit-click").on("click", function (e) {
            e.stopImmediatePropagation();
            e.stopPropagation();
            var bounds = new google.maps.LatLngBounds();
            $.each(DistMap.entityMarkers, function (index, node) {
               
                if(node.content.IS_VISITED==null)
                {
                    node.setVisible(true);
                    bounds.extend(node.getPosition());
                    DistMap.map.fitBounds(bounds);
                    //DistMap.map.setCenter(node.getPosition());
                    //  google.maps.event.trigger(node, 'click', node);
                }
                else {
                    node.setVisible(false);
                    // DistMap.map.setMap(null)
                }
              
            });
        });

        $('#visit-search-result').delegate('.search-result-row', 'click', function (e) {
            e.stopImmediatePropagation();
            e.stopPropagation();
            // Get the corresponding text and set the value
            var selectedValue = "";
            var entity = DistMap.entities[$(this).attr('id').replace('rs-', '')];
            if (entity.TypeMr === "MrType") {
                var selectedValue = entity.EMPLOYEE_EDESC.trim();
                //if (entity.ADDRESS && entity.ADDRESS.trim()) {
                //    selectedValue += ", " + entity.ADDRESS.trim();
                //}
             
                    selectedValue += " (Sales Person)";
                    $('#visit-map-mr-report-filter').show();
                    DistMap.salespersonid = entity.SP_CODE;
                    if ($("#chkgetarea").is(':checked'))
                    {
                        var bounds = new google.maps.LatLngBounds();
                        $.each(DistMap.entityMarkers, function (index, node) {
                            var checkentitylength = 0;

                            if (node.content.LAST_VISIT_BY_CODE == entity.SP_CODE) {
                                checkentitylength++;
                                node.setVisible(true);
                                bounds.extend(node.getPosition());
                                DistMap.map.fitBounds(bounds);

                                //DistMap.map.setCenter(node.getPosition());
                                //DistMap.map.setZoom(10);
                                //DistMap.map.setCenter(node.getPosition());
                                //  google.maps.event.trigger(node, 'click', node);
                            }
                            else {
                                node.setVisible(false);
                                // DistMap.map.setMap(null)
                            }
                      

                        });
                        return false;
                    }
            }
            else {
                var selectedValue = entity.NAME.trim();
                if (entity.ADDRESS && entity.ADDRESS.trim()) {
                    selectedValue += ", " + entity.ADDRESS.trim();
                }
                if (entity.TYPE && entity.TYPE.trim()) {
                    selectedValue += " (" + (entity.TYPE.charAt(0).toUpperCase() + entity.TYPE.slice(1).toLowerCase()).trim() + ")";
                }
            }

            $('#search-target').val(selectedValue);

           // console.log(DistMap.entityMarkers)
            // Display the marker
            // var entityMarker = DistMap.entityMarkers[Number($(this).attr('id').replace('rs-', ''))];
            var entityMarker = DistMap.entityMarkers[$(this).attr('id').replace('rs-', '')];
            //DistMap.map.panTo(entityMarker.getPosition());
            DistMap.map.setCenter(entityMarker.getPosition());
            DistMap.map.setZoom(16);
            google.maps.event.trigger(entityMarker, 'click', entityMarker);
            $('#visit-search-result').hide();
          
        });
        $('#chkgetarea').on("change", function () {


        });
    },
    addMarkers: function () {
        totalMrvisit = 0;
        //reinitialize the visit counts
        var temp_totalVisit = 0; var temp_totalPendingVisit = 0; var temp_totalcancleVisit = 0;

        var currentClicked = null;
        //load distributors onto the map
        //for( i = 0, length=Object.size(this.entities); i < length; i++ ) {
        for (var eid in this.entities) {
            // console.log("addmarker", eid);
            var searchStr = "";
            if (this.entities[eid].TypeMr == "DType")
            {
             searchStr = this.entities[eid].NAME.trim();
            if (this.entities[eid].ADDRESS && this.entities[eid].ADDRESS.trim()) {
                searchStr += ", " + this.entities[eid].ADDRESS.trim();
            }
            if (this.entities[eid].TYPE && this.entities[eid].TYPE.trim()) {
                searchStr += " (" + (this.entities[eid].TYPE.charAt(0).toUpperCase() + this.entities[eid].TYPE.slice(1).toLowerCase()).trim() + ")";
            }
            }
            else if (this.entities[eid].TypeMr == "MrType")
            {
                searchStr = this.entities[eid].EMPLOYEE_EDESC.trim()+"( Sales Person)";

                totalMrvisit++;

            }

            // var title = this.distributors[i][0];
            // var lats = this.distributors[i][1];
            // var longs = this.distributors[i][2];

      
            var title = this.entities[eid].NAME === undefined ? this.entities[eid].EMPLOYEE_EDESC : this.entities[eid].NAME;
            var lats = this.entities[eid].LATITUDE;
            var longs = this.entities[eid].LONGITUDE;
            var type = this.entities[eid].TYPE === undefined ? "Sales Person" : this.entities[eid].TYPE;
            var visitBy = this.entities[eid].LAST_VISIT_BY === undefined ? "Sales Person" : this.entities[eid].LAST_VISIT_BY;;
            var visitDate = this.entities[eid].LAST_VISIT_DATE === undefined ? "Sales Person" : this.entities[eid].LAST_VISIT_DATE;;
            var visitStatus = this.entities[eid].IS_VISITED === undefined ? "Sales Person" : this.entities[eid].IS_VISITED;;
            var lastVisitStatus = this.entities[eid].LAST_VISIT_STATUS === undefined ? "Sales Person" : this.entities[eid].LAST_VISIT_STATUS;;
            var icon = baseUrl + "/blue_marker.png";
            var iconPink = baseUrl + "/pink_marker.png";
            var iconGreen = baseUrl + "/marker_visited.png";

            if ("undefined" == typeof this.entityMarkers[eid]) {

                $('#visit-search-result').append('<div id="rs-' + eid + '" class="search-result-row" tabindex="-1" style="padding:5px;border-bottom:1px solid #eee;cursor:pointer">' + searchStr + '</div>');

                var position = new google.maps.LatLng(lats, longs);

                this.bounds.extend(position);
                var marker = new google.maps.Marker({
                    map: DistMap.map,
                    title: title,
                    snippet: type,
                    position: position,
                    content: this.entities[eid]
                });

                this.entityMarkerCluster.push(marker);

                //if (this.markerOptions.animate == true) {
                //    marker.setAnimation(google.maps.Animation.DROP);
                //}

                var cdate = null;
                var vdate = null;
                if ($('#cur-date').length && $('#cur-date').val()) {
                    cdate = new Date($('#cur-date').val());
                }

                if (this.entities[eid].TypeMr == "MrType")
                {

                }
                else {
                    if ('Y' == visitStatus) {
                        marker.setIcon(iconGreen);
                        temp_totalVisit++;
                        //if ('Y' == lastVisitStatus) {
                        //    marker.setIcon(iconGreen);
                        //    temp_totalVisit++;
                        //}
                        //else {
                        //    marker.setIcon(icon);
                        //    temp_totalcancleVisit++;
                        //}
                    }
                    else if('X' == visitStatus)
                    {
                        marker.setIcon(iconPink);
                        temp_totalPendingVisit++;

                    }
                    else if ('N' == visitStatus) {
                      
                        marker.setIcon(icon);
                        temp_totalcancleVisit++;
                       
                    }
                }

            

                this.entityMarkers[eid] = marker;

                //set the listener
                this.entityMarkers[eid].addListener('click', function () {
                 //   alert("test");
                    var entity = this.content;
                    if (entity.TypeMr == "DType") {

                        $.ajax({
                            url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetPhotoInfo?entityTye=" + entity.TYPE + "&entityCode=" + entity.CODE + "&companyCode=" + entity.COMPANY_CODE,
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
                                $(".custType").html(entity.TYPE);
                                $(".uploderdetails .name").html(entity.NAME);
                                $(".uploderdetails .address").html(entity.ADDRESS);
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
                                    var htmldata = $.parseHTML('<div class="contain-box">'+
                               ' <div class="name-title">'+
                                        '  <h5>Description :' + response.photoInfo[i].DESCRIPTION + '  <span>Uploaded By:' + response.photoInfo[i].USER_NAME + '</span> </h5>' +
                               ' </div>'+
                                        '<div class="contain-image-box"> <img src="' + response.photoInfo[i].FILENAME + '" alt=""> </div></div>');
                                    var distributor = response.photoInfo[i];
                                    $("#loadimage").append(htmldata);
                                   
                                };
                             
                                $("#images").append();
                              
                            },
                            error: function () {

                            }
                        });


                        entity.TYPE = (entity.TYPE.charAt(0).toUpperCase() + entity.TYPE.slice(1).toLowerCase()).trim();
                        var contentString = '<b>' + entity.NAME.trim() + '</b> <br/>(' + entity.TYPE + ')';
                        if ("undefined" != typeof entity.ROUTE_NAME && entity.ROUTE_NAME && entity.ROUTE_NAME.trim() && "undefined" != typeof entity.AREA_NAME && entity.AREA_NAME && entity.AREA_NAME.trim()) {
                            contentString += '<br/>' + entity.ROUTE_NAME.trim() + ', ' + entity.AREA_NAME.trim();
                        }
                        if (entity.visitStatus == 'Y') {
                            contentString += '<br>Visited by ' + entity.LAST_VISIT_BY.trim() + ' on ' + entity.LAST_VISIT_DATE + '</p>';
                        }
                        else if (entity.visitStatus == 'N') {
                            contentString += '<br>Visit Cancelled by ' + entity.LAST_VISIT_BY.trim() + ' on ' + entity.LAST_VISIT_DATE + '</p>';
                        }
                        DistMap.infowindow.setContent('<p>' + contentString + '</p>');

                        DistMap.infowindow.open(DistMap.map, this);

                        currentClicked = this;
                    }
                    else {
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
                        DistMap.infowindow.setContent('<p>' + contentString + '</p>');
                        DistMap.infowindow.entity = entity;

                        DistMap.infowindow.open(DistMap.map, this);

                        currentClicked = this;
                    }

                });
            }
            else {
                /*
                 *the checking of date is causing the condition to be directed to the else case
                 *creating issues in incrementing the total visits
                 */
                //if (visitBy != null && $('#cur-date').val().toLowerCase() == visitDate.toLowerCase()) {
                if (this.entities[eid].TypeMr == "MrType") {

                }
                else {
                    if ('Y' == visitStatus) {
                        this.entityMarkers[eid].setIcon(iconGreen);
                        temp_totalVisit++;
                        //if ('Y' == lastVisitStatus) {
                        //    this.entityMarkers[eid].setIcon(iconGreen);
                        //    temp_totalVisit++;
                        //}
                        //else {
                        //    temp_totalcancleVisit++;
                        //}


                    } else if('X' == visitStatus)
                    {
                        temp_totalPendingVisit++;
                       
                    }
                    else {
                        temp_totalcancleVisit++;
                    }
                }
            }

        }

        if (DistMap.entityMarkerCluster.length > 0 && null == DistMap.markerClusterer) {
            DistMap.markerClusterer = new MarkerClusterer(DistMap.map, DistMap.entityMarkerCluster, {
                maxZoom: 15
            });
        }

      //  console.log("Total Visits : "+temp_totalVisit);
      //  console.log("Total Pending : "+temp_totalPendingVisit);

        //Update the visit total
        $('#visit-count').text(temp_totalVisit);
        $('#pending-visit-count').text(temp_totalPendingVisit);
        totalCancleVisit = temp_totalcancleVisit;
        $('#cancle-visit-count').text(temp_totalcancleVisit);
        $('#total-visit-plan-count').text(temp_totalVisit + temp_totalPendingVisit);
        $("#total-mr-plan-count").text(totalMrvisit);
        //this.evaluateTotalVisits();

        //if (DistMap.markerOptions.fitBounds == true) {
        //    DistMap.map.fitBounds(DistMap.bounds);
        //}

    },
    removeEntities: function () {
        this.entities = [];
    },
    //remove markers from the map but not from the array
    removeMarkers: function () {
        for (var eid in this.entityMarkers)
        {
            this.entityMarkers[eid].setMap(null);
        }
      

        this.entityMarkers = [];
    },
    removeMarkersPoliyline: function () {
        //for (var eid in this.mrTraceRoute) {
        //    this.mrTraceRoute[eid].setMap(null);
        //}


        this.mrTraceRoute = [];
    },
    removeIndivisualMarkers: function (entity) {
      //  this.entity.setMap(null);
        //for (var i = 0; i < this.entityMarkers.length; i++) {
        //    this.entityMarkers[i].setMap(null);
        //}

        //this.entityMarkers = [];
    },
    invalidateMarkers: function () {
        //remove the previous markers from the map and repopulate with new ones
        //this.removeMarkers();
        this.addMarkers();
        this.markerOptions = { fitBounds: false, animate: false };
    },
    evaluateTotalVisits: function () {
        /*
         * this code block need to be somehow merged when adding the marker
         * to the map in the addMarkers() method
         */

        //re-count the total visited and pending visits
        var temp_totalVisit = 0; var temp_totalPendingVisit = 0; var temp_cancleVisit = 0;
        for (var markerId in this.entityMarkers) {

            var marker_visitBy = this.entityMarkers[markerId].content.LAST_VISIT_BY;
            var marker_visitDate = this.entityMarkers[markerId].content.LAST_VISIT_DATE;
            var marker_visitStatus = this.entityMarkers[markerId].content.IS_VISITED;
            var marker_visitCalcleStatus = this.entityMarkers[markerId].content.LAST_VISIT_STATUS;

            if ('Y' == marker_visitStatus) {
                temp_totalVisit++;
            }
            else if('N'==marker_visitCalcleStatus)
            {
                temp_cancleVisit++;
            }
            else {
                temp_totalPendingVisit++;
            }
        }

        //console.log("Total Visits : "+temp_totalVisit);
        //console.log("Total Pending : "+temp_totalPendingVisit);

        // finally Update the visit total
        $('#visit-count').text(temp_totalVisit);
        $('#pending-visit-count').text(temp_totalPendingVisit);
        $('#cancle-visit-count').text(temp_totalPendingVisit);
        $('#total-visit-plan-count').text(temp_totalVisit + temp_totalPendingVisit);
    },
    checkMarker: function (markerName) {
        //console.log("Check for the marker name : "+markerName);
        for (var markerCount in this.entityMarkers) {
            //console.log(this.entityMarkers[markerCount].content.NAME+" Vs "+markerName);
            if (markerName == this.entityMarkers[markerCount].content.NAME) {
                //console.log("return true");
                return true;
            }
        }
    },
};
google.maps.event.addDomListener(window, 'resize', function () {
    //if (DistMap.map == "")
    //    $(window).trigger('load');
    ////    DistMap.init(30000);
    google.maps.event.trigger(DistMap.map, "resize");
    DistMap.map.setCenter(new google.maps.LatLng(DistMap.mapOptions.center.lat, DistMap.mapOptions.center.lng));
});

function initialize() {
    DistMap.init(30000);
    $('#visit-map-mr-legend-wrapper').show();
    $('#visit-map-mr-fullscreen').show();
    $('#visit-plan-mr-search').show();
    $('#visit-map-legend-wrapper').show();
    $('#visit-map-fullscreen').show();
    $('#visit-plan-search').show();
    google.maps.event.addListenerOnce(DistMap.map, 'idle', function () {
        //loaded fully
       

        var oldGoToToday = $.datepicker._gotoToday
        $.datepicker._gotoToday = function (id) {
            oldGoToToday.call(this, id);
            this._selectDate(id);
        }
        $('#visit-map-mr-report-filter-date-inp').datepicker({
            dateFormat: 'dd-mm-yy',
            minDate: '-3D',
            maxDate: 0,
            onSelect: function (dt, inst) {
                // Fetch all the detail path of the selected date and show detail

                // Trigger click of the respective MR
                $('#visit-map-mr-detail-report').trigger('click', { fiterDate: dt });
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
        $('#visit-map-mr-detail-report').on('click', function (e, params) {
            e.preventDefault();
            if ("undefined" != typeof DistMap.salespersonid && DistMap.salespersonid) {
                var spCode = DistMap.salespersonid;
                var mrName = $.trim(DistMap.entityMarkers[DistMap.salespersonid].title);
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
                        DistMap.infowindow.close();
                        // Clear the trace from map
                        clearMrTrace();
                        var traceArr = new google.maps.MVCArray();
                        if ("undefined" != typeof data && "undefined" != typeof data) {
                            if (data.length <= 0) {
                                return false;
                            }

                            if ("undefined" == typeof DistMap.mrTraceRoute[spCode]) {
                                DistMap.mrTraceRoute[spCode] = {
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

                                var marker = addTraceMarker(DistMap.map, position, k);
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
                                DistMap.mrTraceRoute[spCode]['marker'].push(marker);
                                traceArr.push(position);
                                traceBounds.extend(position);
                            });

                            DistMap.mrTraceRoute[spCode]['polyline'] = new google.maps.Polyline({
                                map: DistMap.map,
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
                            google.maps.event.addListener(DistMap.mrTraceRoute[spCode]['polyline'], 'mouseover', function (e) {
                                this.setOptions({
                                    strokeWeight: 5,
                                });
                            });
                            google.maps.event.addListener(DistMap.mrTraceRoute[spCode]['polyline'], 'mouseout', function (e) {
                                this.setOptions({
                                    strokeWeight: 3,
                                });
                            });
                            //animateArrow(MRMap.mrTraceRoute[spCode]['polyline']);

                            /***********************************************************/
                            var numOfPadPoints = 10;
                            var polyPoints = DistMap.mrTraceRoute[spCode]['polyline'].getPath().getArray();
                            var paddedPoints = [];
                            var polylineLength = 0;

                            //Pad the points array
                            $.each(polyPoints, function (key, pt) {
                                var currentPoint = pt;
                                var nextPoint = polyPoints[key + 1];

                                //Check if we're on the last point
                                if (typeof nextPoint !== 'undefined') {
                                    var pathoffirst = new google.maps.LatLng(currentPoint.lat(), currentPoint.lng());
                                    var pathofsecond = new google.maps.LatLng(nextPoint.lat(), nextPoint.lng());
                                    // ..  var     new google.maps.LatLng(39.77745056152344, -86.10900878906250),
                                    var distancebetweentwopoint = google.maps.geometry.spherical.computeDistanceBetween(pathoffirst, pathofsecond);
                                    polylineLength += google.maps.geometry.spherical.computeDistanceBetween(pathoffirst, pathofsecond);
                                    console.log(distancebetweentwopoint, polylineLength);

                                    var totalSlice = Math.round(parseFloat(distancebetweentwopoint / 2));

                                    //var lat = parseFloat(currentPoint.getAttribute("lat"));
                                    //var lng = parseFloat(nextPoint.getAttribute("lng"));
                                    //var pointPath = new google.maps.LatLng(lat, lng);
                                    //path.push(pointPath);
                                    //Get a 10th of the difference in latitude
                                    var latIncr = (nextPoint.lat() - currentPoint.lat()) / totalSlice;

                                    //Get a 10th of the difference in longitude
                                    var lngIncr = (nextPoint.lng() - currentPoint.lng()) / totalSlice;

                                    //Add the current point to the new padded points array
                                    currentPoint.displaypopup = key;

                                    paddedPoints.push(currentPoint);

                                    //Now add 10 points at latIncr & lngIncr intervals between current and next points
                                    //We add this to the new padded points array
                                    for (var i = 1; i < totalSlice; i++) {
                                        var newPt = new google.maps.LatLng(currentPoint.lat() + (i * latIncr), currentPoint.lng() + (i * lngIncr));
                                        paddedPoints.push(newPt);
                                        if (i == totalSlice) {
                                            nextPoint.displaypopup = key;
                                            paddedPoints.push(nextPoint);
                                        }

                                    }

                                }
                            });
                            paddedPoints.push(polyPoints[polyPoints.length - 1]);
                            //  polylineLength += google.maps.geometry.spherical.computeDistanceBetween(paddedPoints[i], paddedPoints[i - 1]);
                            //alert(polylineLength);
                            DistMap.mrTraceRoute[spCode]['polyline']['mmrm'] = new MarkerWithLabel({
                                map: DistMap.map,
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
                                DistMap.mrTraceRoute[spCode]['toids'].push(setTimeout(function (cnt, coords) {
                                    //if (0 == (cnt % 10)) {
                                    //    if (cnt / 10 != 0) {
                                    //        google.maps.event.trigger(DistMap.mrTraceRoute[spCode]['marker'][cnt / 10], "click");
                                    //    }
                                    //}
                                    console.log(coords.displaypopup);
                                    if (undefined !== coords.displaypopup) {
                                        google.maps.event.trigger(DistMap.mrTraceRoute[spCode]['marker'][coords.displaypopup], "click");
                                    }
                                    //if (paddedPoints.displaypopup>0) {
                                    //    google.maps.event.trigger(DistMap.mrTraceRoute[spCode]['marker'][cnt / 10], "click");
                                    //}
                                    var latlng = new google.maps.LatLng(coords.lat(), coords.lng());
                                    DistMap.mrTraceRoute[spCode]['polyline']['mmrm'].setPosition(latlng);
                                }, 800 * i, i, paddedPoints[i])
                                );
                            }

                            var a = window.setInterval(function () {
                                console.log(a);
                                clearInterval(a);
                                $('.trace-marker-label:visible').each(function () {
                                    var w = $(this).outerWidth();
                                    $(this).css({
                                        marginLeft: '-' + (w / 2) + 'px'
                                    });
                                });
                            }, 700);
                            DistMap.map.fitBounds(traceBounds);
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
                $.each(DistMap.entities, function (spc, entity) {

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
                    mliw.open(DistMap.map, DistMap.entityMarkers[spc]);

                    DistMap.mriw[spc] = mliw;
                });
            }
            else {
                // Remove the info windows
                for (var spc in DistMap.mriw) {
                    DistMap.mriw[spc].close();
                }
                DistMap.mriw = {};
            }
        });


    });
}
initialize();
//google.maps.event.addDomListener(window, "load", function () {
//    //initilize map here
//    DistMap.init(30000);
//    google.maps.event.addListenerOnce(DistMap.map, 'idle', function () {
//        //loaded fully
//        $('#visit-map-mr-legend-wrapper').show();
//        $('#visit-map-mr-fullscreen').show();
//        $('#visit-plan-mr-search').show();
//        $('#visit-map-legend-wrapper').show();
//        $('#visit-map-fullscreen').show();
//        $('#visit-plan-search').show();
//        //$('#visit-map-mr-report-filter').show();

//        // var attachHandlers = $.datepicker._attachHandlers;
//        // var generateHTML = $.datepicker._generateHTML;
//        // $.datepicker._attachHandlers = function (inst) {
//        //     // call the cached function in scope of $.datepicker object
//        //     attachHandlers.call($.datepicker, inst);
//        //     // add custom stuff
//        //     inst.dpDiv.find("[data-handler]").map(function () {
//        //         var handler = {
//        //             clear: function () {
//        //                 var id = "#" + inst.id.replace(/\\\\/g, "\\");
//        //                 $.datepicker._clearDate(id);
//        //                 $.datepicker._hideDatepicker();
//        //             }
//        //         };
//        //         if (handler[this.getAttribute("data-handler")]) {
//        //             $(this).bind(this.getAttribute("data-event"), handler[this.getAttribute("data-handler")]);
//        //         }
//        //     });
//        // };
//        // $.datepicker._generateHTML = function (inst) {
//        //     //call the cached function in scope of $.datepicker object
//        //     var html = generateHTML.call($.datepicker, inst);
//        //     var $html = $(html);
//        //     var $buttonPane = $html.filter("div.ui-datepicker-buttonpane.ui-widget-content");
//        //
//        //     $buttonPane.append($("<button />")
//        //         .text("Clear")
//        //         .attr("type", "button")
//        //         .attr("data-handler", "clear")
//        //         .attr("data-event", "click")
//        //         .addClass("ui-datepicker-clear ui-state-default ui-priority-secondary ui-corner-all"));
//        //
//        //     return $html;
//        // };

//        var oldGoToToday = $.datepicker._gotoToday
//        $.datepicker._gotoToday = function (id) {
//            oldGoToToday.call(this, id);
//            this._selectDate(id);
//        }
//        $('#visit-map-mr-report-filter-date-inp').datepicker({
//            dateFormat: 'dd-mm-yy',
//            minDate: '-3D',
//            maxDate: 0,
//            onSelect: function (dt, inst) {
//                // Fetch all the detail path of the selected date and show detail

//                // Trigger click of the respective MR
//                $('#visit-map-mr-detail-report').trigger('click', { fiterDate: dt });
//            },
//            showOn: "button",
//            showButtonPanel: true,
//            beforeShow: function (input, obj) {
//                setTimeout(function () {
//                    var buttonPane = $(input).datepicker("widget").find(".ui-datepicker-buttonpane");
//                    $("#BCL").hide();
//                    $("#BTDY").hide();
//                    $("<button>", {
//                        type: "button",
//                        text: "Clear",
//                        click: function () {
//                            $(input).val('');
//                            // Clear the
//                            clearMrTrace();
//                        }
//                    }).addClass('ui-state-default ui-corner-all').appendTo(buttonPane);
//                }, 1);
//            }
//        });

//        var traceBounds = new google.maps.LatLngBounds();
//        $('#visit-map-mr-detail-report').on('click', function (e, params) {
//            e.preventDefault();
//            if ("undefined" != typeof DistMap.salespersonid && DistMap.salespersonid) {
//                var spCode = DistMap.salespersonid;
//                var mrName = $.trim(DistMap.entityMarkers[DistMap.salespersonid].title);
//                console.log(spCode);

//                var fiterDate = null;
//                if ("undefined" != typeof params) {
//                    if ("undefined" != typeof params.fiterDate) {
//                        fiterDate = params.fiterDate;
//                    }
//                }
//                $.ajax({
//                    url: window.location.protocol + "//" + window.location.host + '/api/Distribution/GetMrVistitedPlainDateWise?date=' + fiterDate + '&spCode=' + spCode,
//                    type: "post",
//                    dataType: "json",
//                    success: function (data) {
//                        console.log(data);
//                        DistMap.infowindow.close();
//                        // Clear the trace from map
//                        clearMrTrace();
//                        var traceArr = new google.maps.MVCArray();
//                        if ("undefined" != typeof data && "undefined" != typeof data) {
//                            if (data.length <= 0) {
//                                return false;
//                            }

//                            if ("undefined" == typeof DistMap.mrTraceRoute[spCode]) {
//                                DistMap.mrTraceRoute[spCode] = {
//                                    "polyline": null,
//                                    "marker": [],   //
//                                    "interval": 0,
//                                    "toids": [], // time out ids
//                                    "mmrm": null,  // moving mr marker
//                                    "iw": null  // infowindow
//                                };
//                            }

//                            var entityTrackJson = data;
//                            $.each(entityTrackJson, function (k, v) {
//                                var position = new google.maps.LatLng(Number(v.LATITUDE), Number(v.LONGITUDE));
//                                // var marker = new google.maps.Marker({
//                                //     map: MRMap.map,
//                                //     position: position,
//                                //     label: (k+1).toString(),
//                                //     icon: {
//                                //         path: google.maps.SymbolPath.CIRCLE,
//                                //         scale: 10,
//                                //         strokeColor: '#008000',
//                                //         strokeOpacity: 1,
//                                //         strokeWeight: 2
//                                //     }
//                                // });

//                                var marker = addTraceMarker(DistMap.map, position, k);
//                                // console.log(marker);
//                                v.EMPLOYEE_EDESC = mrName;
//                                addTraceMarkerEvent(marker, v);
//                                // //add the click listener to the marker
//                                // marker.addListener("click", function () {
//                                //     //display the info window here for the marker
//                                //     var entity = v;
//                                //     entity.EMPLOYEE_EDESC = mrName;
//                                //     var contentString = '<b>' + mrName + '</b><br/>';
//                                //     contentString += '<b><u>Date</u> :   </b>' + entity.ACTIVITY_TIME.trim() + '<br/>';
//                                //     contentString += '<b><u>Remarks</u></b><br/>';
//                                //     var visitRemarks = $.trim(entity.REMARKS);
//                                //     if (visitRemarks == " " || visitRemarks.length < 2) {
//                                //         //no remarks available
//                                //         contentString += "Not Available"
//                                //     }
//                                //     else {
//                                //         contentString += entity.REMARKS;
//                                //     }
//                                //
//                                //     if (null == MRMap.mrTraceRoute[spCode].iw) {
//                                //         MRMap.mrTraceRoute[spCode].iw = new google.maps.InfoWindow({
//                                //             disableAutoPan: true
//                                //         });
//                                //     }
//                                //
//                                //     // MRMap.infowindow.setContent('<p>' + contentString + '</p>');
//                                //     // MRMap.infowindow.entity = entity;
//                                //     // MRMap.infowindow.open(MRMap.map, this);
//                                //
//                                //     MRMap.mrTraceRoute[spCode].iw.setContent('<p>' + contentString + '</p>');
//                                //     MRMap.mrTraceRoute[spCode].iw.open(MRMap.map, this);
//                                //     setTimeout(function() {
//                                //         MRMap.mrTraceRoute[spCode].iw.close();
//                                //     }, 3300);
//                                //
//                                // });
//                                // marker.addListener("mouseover", function() {
//                                //     google.maps.event.trigger(this, "click");
//                                // });
//                                DistMap.mrTraceRoute[spCode]['marker'].push(marker);
//                                traceArr.push(position);
//                                traceBounds.extend(position);
//                            });

//                            DistMap.mrTraceRoute[spCode]['polyline'] = new google.maps.Polyline({
//                                map: DistMap.map,
//                                path: traceArr,
//                                strokeWeight: 3,
//                                strokeColor: '#F75850',
//                                strokeOpacity: 1,
//                                zIndex: 10,
//                                // icons: [{
//                                //     icon: {
//                                //         path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
//                                //         sacle: 8,
//                                //         strokeColor: '#393'
//                                //     },
//                                //     offset: '100%'
//                                // }],
//                                isVisible: true
//                            });
//                            google.maps.event.addListener(DistMap.mrTraceRoute[spCode]['polyline'], 'mouseover', function (e) {
//                                this.setOptions({
//                                    strokeWeight: 5,
//                                });
//                            });
//                            google.maps.event.addListener(DistMap.mrTraceRoute[spCode]['polyline'], 'mouseout', function (e) {
//                                this.setOptions({
//                                    strokeWeight: 3,
//                                });
//                            });
//                            //animateArrow(MRMap.mrTraceRoute[spCode]['polyline']);

//                            /***********************************************************/
//                            var numOfPadPoints = 10;
//                            var polyPoints = DistMap.mrTraceRoute[spCode]['polyline'].getPath().getArray();
//                            var paddedPoints = [];
//                          var  polylineLength = 0;
                           
//                            //Pad the points array
//                            $.each(polyPoints, function (key, pt) {
//                                var currentPoint = pt;
//                                var nextPoint = polyPoints[key + 1];
                              
//                                //Check if we're on the last point
//                                if (typeof nextPoint !== 'undefined') {
//                                    var pathoffirst = new google.maps.LatLng(currentPoint.lat(), currentPoint.lng());
//                                    var pathofsecond = new google.maps.LatLng(nextPoint.lat(), nextPoint.lng());
//                                    // ..  var     new google.maps.LatLng(39.77745056152344, -86.10900878906250),
//                                    var distancebetweentwopoint = google.maps.geometry.spherical.computeDistanceBetween(pathoffirst, pathofsecond);
//                                    polylineLength += google.maps.geometry.spherical.computeDistanceBetween(pathoffirst, pathofsecond);
//                                    console.log(distancebetweentwopoint,polylineLength);
                                   
//                                    var totalSlice = Math.round(parseFloat(distancebetweentwopoint / 2));

//                                    //var lat = parseFloat(currentPoint.getAttribute("lat"));
//                                    //var lng = parseFloat(nextPoint.getAttribute("lng"));
//                                    //var pointPath = new google.maps.LatLng(lat, lng);
//                                    //path.push(pointPath);
//                                    //Get a 10th of the difference in latitude
//                                    var latIncr = (nextPoint.lat() - currentPoint.lat()) / totalSlice;

//                                    //Get a 10th of the difference in longitude
//                                    var lngIncr = (nextPoint.lng() - currentPoint.lng()) / totalSlice;

//                                    //Add the current point to the new padded points array
//                                    currentPoint.displaypopup = key;

//                                    paddedPoints.push(currentPoint);

//                                    //Now add 10 points at latIncr & lngIncr intervals between current and next points
//                                    //We add this to the new padded points array
//                                    for (var i = 1; i < totalSlice; i++) {
//                                        var newPt = new google.maps.LatLng(currentPoint.lat() + (i * latIncr), currentPoint.lng() + (i * lngIncr));
//                                        paddedPoints.push(newPt);
//                                        if (i == totalSlice) {
//                                            nextPoint.displaypopup = key;
//                                            paddedPoints.push(nextPoint);
//                                        }
                                       
//                                    }

//                                }
//                            });
//                            paddedPoints.push(polyPoints[polyPoints.length - 1]);
//                          //  polylineLength += google.maps.geometry.spherical.computeDistanceBetween(paddedPoints[i], paddedPoints[i - 1]);
//                            //alert(polylineLength);
//                            DistMap.mrTraceRoute[spCode]['polyline']['mmrm'] = new MarkerWithLabel({
//                                map: DistMap.map,
//                                position: new google.maps.LatLng(paddedPoints[0].lat(), paddedPoints[0].lng()),
//                                // icon: {
//                                //     path: google.maps.SymbolPath.CIRCLE,
//                                //     scale: 3,
//                                //     strokeColor: '#393'
//                                // },
//                                draggable: false,
//                                raiseOnDrag: false,
//                                labelContent: mrName,
//                                labelAnchor: new google.maps.Point(12, 0),
//                                labelClass: "mr-marker-label", // the CSS class for the label
//                                labelStyle: { opacity: 0.75 },
//                                zIndex: 999
//                            });

//                            for (var i = 0; i < paddedPoints.length; i++) {
//                                DistMap.mrTraceRoute[spCode]['toids'].push(setTimeout(function (cnt, coords) {
//                                    //if (0 == (cnt % 10)) {
//                                    //    if (cnt / 10 != 0) {
//                                    //        google.maps.event.trigger(DistMap.mrTraceRoute[spCode]['marker'][cnt / 10], "click");
//                                    //    }
//                                    //}
//                                    console.log(coords.displaypopup);
//                                    if (undefined !== coords.displaypopup)
//                                    {
//                                        google.maps.event.trigger(DistMap.mrTraceRoute[spCode]['marker'][coords.displaypopup], "click");
//                                    }
//                                    //if (paddedPoints.displaypopup>0) {
//                                    //    google.maps.event.trigger(DistMap.mrTraceRoute[spCode]['marker'][cnt / 10], "click");
//                                    //}
//                                    var latlng = new google.maps.LatLng(coords.lat(), coords.lng());
//                                    DistMap.mrTraceRoute[spCode]['polyline']['mmrm'].setPosition(latlng);
//                                }, 800 * i, i, paddedPoints[i])
//                                );
//                            }

//                            var a = window.setInterval(function () {
//                                console.log(a);
//                                clearInterval(a);
//                                $('.trace-marker-label:visible').each(function () {
//                                    var w = $(this).outerWidth();
//                                    $(this).css({
//                                        marginLeft: '-' + (w / 2) + 'px'
//                                    });
//                                });
//                            }, 700);
//                            DistMap.map.fitBounds(traceBounds);
//                        }
//                    },
//                    error: function (err) {

//                    }
//                });
//            }
//        });

//        $('.visit-map-mr-cbox').on('change', function (e) {
//            e.preventDefault();
//            if (this.checked) {
//                // Display the info windows
//                $.each(DistMap.entities, function (spc, entity) {

//                    var contentString = '<b>' + entity.EMPLOYEE_EDESC.trim() + '</b>';
//                    if (entity.CONTACT_NO && entity.CONTACT_NO.trim()) {
//                        contentString += "<br/>Contact: " + entity.CONTACT_NO.trim();
//                    }
//                    if (entity.REG_OFFICE_ADDRESS && entity.REG_OFFICE_ADDRESS.trim()) {
//                        contentString += "<br/>" + entity.REG_OFFICE_ADDRESS.trim();
//                    }
//                    if (entity.SUBMIT_DATE && entity.SUBMIT_DATE.trim()) {
//                        contentString += "<br/>Last Seen: " + entity.SUBMIT_DATE.trim();
//                    }

//                    var mliw = new google.maps.InfoWindow();
//                    mliw.setContent('<p>' + contentString + '</p>');
//                    mliw.open(DistMap.map, DistMap.entityMarkers[spc]);

//                    DistMap.mriw[spc] = mliw;
//                });
//            }
//            else {
//                // Remove the info windows
//                for (var spc in DistMap.mriw) {
//                    DistMap.mriw[spc].close();
//                }
//                DistMap.mriw = {};
//            }
//        });


//    });
//    //google.maps.event.addListenerOnce(DistMap.map, 'idle', function () {
//    //    //loaded fully
//    //    $('#visit-map-legend-wrapper').show();
//    //    $('#visit-map-fullscreen').show();
//    //    $('#visit-plan-search').show();
//    //});
//});

google.maps.event.addDomListener(window, "click", function () {
    $('#visit-search-result').hide();
});


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
        labelContent: (idx + 1).toString(),
        labelAnchor: new google.maps.Point(0, 0),
        labelClass: "trace-marker-label", // the CSS class for the label
        labelStyle: { opacity: 0.75 }
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

        if (null == DistMap.mrTraceRoute[entity.SP_CODE].iw) {
            DistMap.mrTraceRoute[entity.SP_CODE].iw = new google.maps.InfoWindow({
                disableAutoPan: true
            });
        }

        // MRMap.infowindow.setContent('<p>' + contentString + '</p>');
        // MRMap.infowindow.entity = entity;
        // MRMap.infowindow.open(MRMap.map, this);

        DistMap.mrTraceRoute[entity.SP_CODE].iw.setContent('<p>' + contentString + '</p>');
        DistMap.mrTraceRoute[entity.SP_CODE].iw.open(DistMap.map, this);
        setTimeout(function () {
            DistMap.mrTraceRoute[entity.SP_CODE].iw.close();
        }, 3300);

    });
    marker.addListener("mouseover", function () {
        google.maps.event.trigger(this, "click");
    });
}

function clearMrTrace() {
    var spCode = DistMap.salespersonid;
    if ("undefined" != typeof DistMap.mrTraceRoute[spCode]) {
        window.clearInterval(DistMap.mrTraceRoute[spCode]['polyline']['interval']);
        for (var x in DistMap.mrTraceRoute[spCode]['toids']) {
            var toid = DistMap.mrTraceRoute[spCode]['toids'][x];
            window.clearTimeout(toid);
        }
        if (DistMap.mrTraceRoute[spCode]['polyline']['mmrm'] instanceof google.maps.Marker) {
            DistMap.mrTraceRoute[spCode]['polyline']['mmrm'].setMap(null);
        }
        // Reset the polyline and marker object
        DistMap.mrTraceRoute[spCode]['polyline'].setMap(null);
        $.each(DistMap.mrTraceRoute[spCode]['marker'], function (c, m) {
            m.setMap(null);
        });
    }
}

