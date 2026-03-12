

function callWebService(){

            console.log("called web service");
            //fetch the distributors and resellers list from the web server
            $.post("http://192.168.4.102/distribution/MobileService/Service",JSON.stringify({"action":"fetchEntity"}),
                function(response){
                    console.log("got response");
                    //get the response object
                    var responseObject = JSON.parse(response);
                    //check the response object
                    if(responseObject.response == true){
                        //get the list of distributors
                        console.log("response is true");
                        // console.log(responseObject.result.);
                        var distMarkers = new Array();
                        //this j will set the index of the resellers in the distMarkers array   
                        var j = 0;
                        //get the distributors
                        for(var i = 0; i < responseObject.result.distributor.length; i++ ){
                            
                            distMarkers[i] = responseObject.result.distributor[i];
                        
                            j++;
                        }
                        //now we get the resellers
                        for(var i = 0; i < responseObject.result.reseller.length; i++ ){
                            distMarkers[j] = responseObject.result.reseller[i];
                            j++;
                                                
                        }

                        //finally set the distributor markers in the map
                        DistMap.entities = distMarkers;
                        //add the markers to the map
                        DistMap.invalidateMarkers();
                    }
                    else{
                        console.log(responseObject.error);
                    }   
                });
        }


var DistMap = {

    map:"",
    bounds:"",
    mapOptions:{
                mapTypeControlOptions: {
                    mapTypeIds: ['custom', google.maps.MapTypeId.ROADMAP, google.maps.MapTypeId.TERRAIN]
                },
                mapTypeControl: false,
                zoomControl: true,
                streetViewControl: false,
                zoom: 15,
                center: {lat: 27.6737503, lng: 85.314918},
                mapTypeId: google.maps.MapTypeId.ROADMAP,    
                },
    styleOptions:[
                    {
                        featureType: "administrative",
                        elementType: "all",
                        stylers: [
                            { visibility: "off" }
                        ]
                    },{
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
                    },{
                        featureType: "road",
                        elementType: "all",
                        stylers: [
                            { visibility: "off" }
                        ]
                    },{
                        featureType: "transit",
                        elementType: "all",
                        stylers: [
                            { visibility: "off" }
                        ]
                    },{
                        featureType: "water",
                        elementType: "all",
                        stylers: [
                            { visibility: "off" }
                        ]
                    }
                ],           
    markerOptions:{
                fitBounds: true,
                animate:true
                },
    init:function(interval){
        //check for the interval parameter
        interval = typeof interval !== 'undefined' ? interval : 30000;
        //initialize the map
        var mapDiv = document.getElementById('visit-map');
        this.bounds = new google.maps.LatLngBounds();
        console.log("init called for dist map");   

        this.map = new google.maps.Map(mapDiv,this.mapOptions);

        // this.map.mapTypes.set('custom', new google.maps.StyledMapType(this.styleOptions, { name: 'Distribution' }));

        //make initial service call
        callWebService();
        
        //set the interval for the webservice to be called
        console.log(interval);
        var intervals = setInterval(callWebService,interval);                    
    },

    entities:[],
    entityMarkers:[],
    addMarkers:function () {

                    var map = this.map;
                    var infowindow = new google.maps.InfoWindow();
                    
                    var currentClicked = null;

                    //load distributors onto the map
                    for( i = 0; i < this.entities.length; i++ ) {

                        // var title = this.distributors[i][0];
                        // var lats = this.distributors[i][1];
                        // var longs = this.distributors[i][2];
                    
                        var title = this.entities[i].NAME;
                        var lats = this.entities[i].LATITUDE;
                        var longs = this.entities[i].LONGITUDE;
                        var type = this.entities[i].TYPE;
                        var visitBy = this.entities[i].VISIT_BY;
                        var visitDate = this.entities[i].VISIT_DATE;

                        var icon = "asset/img/blue_marker.png";
                        var iconPink = "asset/img/pink_marker.png";
                        var iconGreen = "asset/img/marker_visited.png";

                        var position = new google.maps.LatLng(lats,longs);

                        this.bounds.extend(position);
                        var marker = marker = new google.maps.Marker({
                                map: this.map,
                                title: title,
                                snippet:type, 
                                position: position,
                                content:this.entities[i]
                            });

                        if(this.markerOptions.animate == true){
                            marker.setAnimation(google.maps.Animation.DROP);
                        }
                        
                        if(visitBy != null){
                            marker.setIcon(iconGreen);
                        }   
                        else{
                            if(type ==  "distributor"){    
                                marker.setIcon(icon);
                            }
                            else{
                               marker.setIcon(iconPink);  
                            }
                        } 
                        
                        this.entityMarkers[i] = marker;
                    
                        //set the listener
                        this.entityMarkers[i].addListener('click',function(){

                            var entity = this.content;
                            var contentString = "";
                            if(entity.VISIT_BY != null){
                                contentString = '<p><b>'+entity.NAME+'</b> ('+entity.TYPE+')<br><b>Code : </b>'+entity.CODE+'<br>'+this.position+'<br>Visited by '+entity.VISIT_BY+' on '+entity.VISIT_DATE+'</p>';
                            }    
                            else{
                                contentString = '<p><b>'+entity.NAME+'</b> ('+entity.TYPE+')<br><b>Code : </b>'+entity.CODE+'<br>'+this.position+'</br></p>';    
                            }
                            infowindow.setContent(contentString);

                            infowindow.open(this.map,this);

                            currentClicked = this;

                        });

                    }
                    if(this.markerOptions.fitBounds == true){
                        this.map.fitBounds(this.bounds);
                    }
                    
                },
    removeEntities:function(){
                    this.entities = [];
                },           
    //remove markers from the map but not from the array            
    removeMarkers:function () {
                  for (var i = 0; i < this.entityMarkers.length; i++) {
                        this.entityMarkers[i].setMap(null);
                      }

                      this.entityMarkers =  [];
                },
    invalidateMarkers:function (){
                //remove the previous markers from the map and repopulate with new ones
                this.removeMarkers();
                this.addMarkers();
                this.markerOptions = {fitBounds: false, animate: false };
                }, 

    };
