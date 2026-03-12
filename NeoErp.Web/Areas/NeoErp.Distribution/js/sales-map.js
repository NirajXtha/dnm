var salesMap = null;
var areaBoundary = null;
var overlay = {polygon: {}, heatmap: null};
var salesMapBounds = null;
var infoWindow = null;
var areaPolygon = {};
var regionPolygon = {};
var regionPolyline = {};
var regionMarker = {};
// District with custom location
var districts = {"Humla":{"coord":"30.0244779586792,81.87944412231445", "population":"55261"},"Darchula":{"coord":"29.876494314317963,80.6965217590332", "population":"139712"},"Bajhang":{"coord":"29.682022001765862,81.04458236694336", "population":"210122"},"Mugu":{"coord":"29.631138259750927,82.42422103881836", "population":"60109"},"Bajura":{"coord":"29.552796812080043,81.55831909179688", "population":"146338"},"Baitadi":{"coord":"29.58334656907578,80.51037216186523", "population":"260015"},"Dolpa":{"coord":"29.212735176086426,83.0395622253418", "population":"39832"},"Jumla":{"coord":"29.27861204480541,82.13730239868164", "population":"117958"},"Kalikot":{"coord":"29.214661598205566,81.74451446533203", "population":"149371"},"Doti":{"coord":"29.18565082550049,80.84156799316406", "population":"213619"},"Dadeldhura":{"coord":"29.208919525146484,80.47037124633789", "population":"151312"},"Achham":{"coord":"29.067813873291016,81.30780029296875", "population":"274505"},"Mustang":{"coord":"29.019266251776244,83.78144454956055", "population":"12477"},"Dailekh":{"coord":"28.883142471313477,81.66998291015625", "population":"281758"},"Jajarkot":{"coord":"28.877865761195096,82.07764434814453", "population":"186375"},"Kanchanpur":{"coord":"28.83077907562256,80.30586242675781", "population":"494553"},"Kailali":{"coord":"28.73830223083496,80.89383697509766", "population":"870771"},"Rukum":{"coord":"28.73126792907715,82.6692008972168", "population":"220092"},"Surkhet":{"coord":"28.649361610412598,81.51066207885742", "population":"387858"},"Manang":{"coord":"28.677886962890625,84.18357849121094", "population":"6444"},"Myagdi":{"coord":"28.593274740425187,83.41275405883789", "population":"112643"},"Gorkha":{"coord":"28.44799988035162,84.77799606323242", "population":"259299"},"Bardiya":{"coord":"28.408295083453623,81.39199829101562", "population":"456547"},"Salyan":{"coord":"28.400267601013184,82.08400344848633", "population":"259309"},"Baglung":{"coord":"28.403896754691477,83.15782165527344", "population":"277582"},"Kaski":{"coord":"28.349750518798828,83.99161911010742", "population":"543767"},"Rolpa":{"coord":"28.3940834510322,82.57798767089844", "population":"232419"},"Lamjung":{"coord":"28.283453941345215,84.44449234008789", "population":"170568"},"Parbat":{"coord":"28.286860912976465,83.69654846191406", "population":"148130"},"Rasuwa":{"coord":"28.207605609702608,85.3270263671875", "population":"44399"},"Pyuthan":{"coord":"28.121519088745117,82.850341796875", "population":"236540"},"Dhading":{"coord":"27.928442808596017,84.89709091186523", "population":"346950"},"Banke":{"coord":"28.092971801757812,81.84926223754883", "population":"554630"},"Gulmi":{"coord":"28.148637714127002,83.29051208496094", "population":"268597"},"Dang":{"coord":"27.96769905090332,82.44760513305664", "population":"605796"},"Syangja":{"coord":"28.018954108651283,83.81455993652344", "population":"270403"},"Sindhupalchok":{"coord":"27.910818190808854,85.66020584106445", "population":"292370"},"Dolakha":{"coord":"27.79402475546042,86.10783386230469", "population":"187584"},"Tanahun":{"coord":"28.01380092468256,84.13196563720703", "population":"336710"},"Arghakhanchi":{"coord":"27.939671718613063,83.11018753051758", "population":"200967"},"Solukhumbu":{"coord":"27.658764994433454,86.65316390991211", "population":"104415"},"Nuwakot":{"coord":"27.894188433385704,85.20988464355469", "population":"283827"},"Sankhuwasabha":{"coord":"27.581351620908357,87.10599136352539", "population":"157854"},"Palpa":{"coord":"27.81869411468506,83.63307571411133", "population":"255386"},"Taplejung":{"coord":"27.609169960021973,87.82640838623047", "population":"129694"},"Chitwan":{"coord":"27.58676356028912,84.32622528076172", "population":"644219"},"Nawalparasi":{"coord":"27.619185191324252,83.81760787963867", "population":"690731"},"Kapilvastu":{"coord":"27.626482009887695,82.9680290222168", "population":"625522"},"Ramechhap":{"coord":"27.43032417775495,86.10038375854492", "population":"206653"},"Kathmandu":{"coord":"27.723996701387527,85.35696029663086", "population":"2011978"},"Rupandehi":{"coord":"27.56171226501465,83.43426513671875", "population":"982851"},"Kavrepalanchok":{"coord":"27.561025619506836,85.63477325439453", "population":"394229"},"Bhaktapur":{"coord":"27.67421817779541,85.44337844848633", "population":"340066"},"Makwanpur":{"coord":"27.43602180480957,85.09759521484375", "population":"443976"},"Lalitpur":{"coord":"27.546823501586914,85.34109115600586", "population":"525211"},"Okhaldhunga":{"coord":"27.33780288696289,86.44639205932617", "population":"150428"},"Bhojpur":{"coord":"27.175825119018555,87.08938217163086", "population":"169139"},"Parsa":{"coord":"27.299473898194783,84.79452133178711", "population":"663559"},"Sindhuli":{"coord":"27.242310796237014,85.90741729736328", "population":"305164"},"Khotang":{"coord":"27.137191875332135,86.774169921875", "population":"190100"},"Panchthar":{"coord":"27.05941541062617,87.6429214477539", "population":"195334"},"Bara":{"coord":"27.10765266418457,85.07121658325195", "population":"765053"},"Tehrathum":{"coord":"27.19576357622211,87.53561782836914", "population":"101546"},"Rautahat":{"coord":"27.041812600652726,85.30918884277344", "population":"772098"},"Dhankuta":{"coord":"27.019126892089844,87.32669067382812", "population":"168131"},"Sarlahi":{"coord":"26.968419075012207,85.57421493530273", "population":"838695"},"Udayapur":{"coord":"26.97299344798355,86.60336303710938", "population":"339267"},"Mahottari":{"coord":"26.919005092132974,85.81463241577148", "population":"673405"},"Illam":{"coord":"26.881919860839844,87.89123153686523", "population":"302791"},"Dhanusa":{"coord":"26.830693244934082,86.05675888061523", "population":"803785"},"Siraha":{"coord":"26.741276741027832,86.336181640625", "population":"674923"},"Saptari":{"coord":"26.59658636181935,86.74429702758789", "population":"679548"},"Morang":{"coord":"26.546639295689598,87.4140396118164", "population":"1036841"},"Sunsari":{"coord":"26.729444221369768,87.13446807861328", "population":"845555"},"Jhapa":{"coord":"26.583044052124023,87.91729354858398", "population":"875828"}};
// District Headquaters Location
// var districts = {"Morang":{"coord":"26.458152770996094,87.27945709228516"},"Khotang":{"coord":"27.222742080688477,86.80105590820312"},"Taplejung":{"coord":"27.357425689697266,87.67997741699219"},"Panchthar":{"coord":"27.145832061767578,87.7756118774414"},"Illam":{"coord":"26.910614013671875,87.93486022949219"},"Jhapa":{"coord":"26.59193992614746,88.06499481201172"},"Sankhuwasabha":{"coord":"27.376585006713867,87.2077407836914"},"Tehrathum":{"coord":"27.127485275268555,87.5396499633789"},"Dhankuta":{"coord":"26.99081802368164,87.34424591064453"},"Bhojpur":{"coord":"27.175823211669922,87.05175018310547"},"Sunsari":{"coord":"26.602121353149414,87.15166473388672"},"Solukhumbu":{"coord":"27.509082794189453,86.5889663696289"},"Okhaldhunga":{"coord":"27.32207489013672,86.50154876708984"},"Udayapur":{"coord":"26.79288673400879,86.7057876586914"},"Saptari":{"coord":"26.545316696166992,86.75047302246094"},"Siraha":{"coord":"26.665773391723633,86.20317840576172"},"Dolakha":{"coord":"27.68475914001465,86.06260681152344"},"Ramechhap":{"coord":"27.33774757385254,86.08182525634766"},"Sindhuli":{"coord":"27.22186851501465,85.9095230102539"},"Dhanusa":{"coord":"26.736465454101562,85.92674255371094"},"Mahottari":{"coord":"26.660377502441406,85.80509185791016"},"Sarlahi":{"coord":"26.86549186706543,85.56040954589844"},"Rasuwa":{"coord":"28.122976303100586,85.29813385009766"},"Sindhupalchok":{"coord":"27.786489486694336,85.71617889404297"},"Nuwakot":{"coord":"27.920480728149414,85.14605712890625"},"Dhading":{"coord":"27.876832962036133,84.85920715332031"},"Kathmandu":{"coord":"27.712886810302734,85.32275390625"},"Bhaktapur":{"coord":"27.685693740844727,85.43383026123047"},"Lalitpur":{"coord":"27.68374252319336,85.32364654541016"},"Kavrepalanchok":{"coord":"27.623132705688477,85.55056762695312"},"Rautahat":{"coord":"26.781095504760742,85.27626037597656"},"Bara":{"coord":"27.04438591003418,85.00658416748047"},"Parsa":{"coord":"27.039817810058594,84.8904800415039"},"Makwanpur":{"coord":"27.43912124633789,85.03254699707031"},"Chitwan":{"coord":"27.68511390686035,84.44576263427734"},"Nawalparasi":{"coord":"27.535600662231445,83.66626739501953"},"Kapilvastu":{"coord":"27.55274200439453,83.04846954345703"},"Palpa":{"coord":"27.871219635009766,83.54866790771484"},"Syangja":{"coord":"28.0865421295166,83.83855438232422"},"Arghakhanchi":{"coord":"27.97964859008789,83.12936401367188"},"Gulmi":{"coord":"28.07402801513672,83.2494125366211"},"Gorkha":{"coord":"28.005746841430664,84.62615966796875"},"Tanahun":{"coord":"27.987340927124023,84.2922134399414"},"Lamjung":{"coord":"28.24101448059082,84.37652587890625"},"Kaski":{"coord":"28.221927642822266,83.9797592163086"},"Manang":{"coord":"28.563058853149414,84.23905944824219"},"Parbat":{"coord":"28.22273826599121,83.67566680908203"},"Baglung":{"coord":"28.273099899291992,83.58971405029297"},"Myagdi":{"coord":"28.35000991821289,83.5588607788086"},"Mustang":{"coord":"28.782564163208008,83.72959899902344"},"Rukum":{"coord":"28.640132904052734,82.51693725585938"},"Salyan":{"coord":"28.385719299316406,82.14995574951172"},"Rolpa":{"coord":"28.315114974975586,82.62963104248047"},"Pyuthan":{"coord":"28.09630584716797,82.87495422363281"},"Dang":{"coord":"28.044925689697266,82.48216247558594"},"Rupandehi":{"coord":"27.51416778564453,83.45418548583984"},"Banke":{"coord":"28.070892333984375,81.62626647949219"},"Bardiya":{"coord":"28.210468292236328,81.34254455566406"},"Surkhet":{"coord":"28.59480857849121,81.62594604492188"},"Dailekh":{"coord":"28.84639549255371,81.70630645751953"},"Jajarkot":{"coord":"28.707923889160156,82.19634246826172"},"Dolpa":{"coord":"28.95292091369629,82.90658569335938"},"Jumla":{"coord":"29.288976669311523,82.18612670898438"},"Kalikot":{"coord":"29.148710250854492,81.60480499267578"},"Mugu":{"coord":"29.55394744873047,82.1537094116211"},"Humla":{"coord":"29.97531509399414,81.8187255859375"},"Bajhang":{"coord":"29.559814453125,81.19532775878906"},"Bajura":{"coord":"29.457284927368164,81.47930908203125"},"Achham":{"coord":"29.135719299316406,81.24185943603516"},"Doti":{"coord":"29.262420654296875,80.96150970458984"},"Kailali":{"coord":"28.70060157775879,80.60131072998047"},"Kanchanpur":{"coord":"28.965801239013672,80.18182373046875"},"Dadeldhura":{"coord":"29.303020477294922,80.58843231201172"},"Baitadi":{"coord":"29.557313919067383,80.42524719238281"},"Darchula":{"coord":"29.847379684448242,80.57302856445312"}};
var sales = null, districtMarker = {};
var locations = [];

// Markers to be displayed according to zoom level
var zoomMarkerDistrict = {
    7 : {"Khotang":"Khotang","Taplejung":"Taplejung","Panchthar":"Panchthar","Sankhuwasabha":"Sankhuwasabha","Tehrathum":"Tehrathum","Dhankuta":"Dhankuta","Bhojpur":"Bhojpur","Solukhumbu":"Solukhumbu","Okhaldhunga":"Okhaldhunga","Udayapur":"Udayapur","Saptari":"Saptari","Siraha":"Siraha","Ramechhap":"Ramechhap","Sindhuli":"Sindhuli","Dhanusa":"Dhanusa","Mahottari":"Mahottari","Sarlahi":"Sarlahi","Rasuwa":"Rasuwa","Sindhupalchok":"Sindhupalchok","Nuwakot":"Nuwakot","Dhading":"Dhading","Kathmandu":"Kathmandu","Bhaktapur":"Bhaktapur","Lalitpur":"Lalitpur","Kavrepalanchok":"Kavrepalanchok","Rautahat":"Rautahat","Makwanpur":"Makwanpur","Nawalparasi":"Nawalparasi","Kapilvastu":"Kapilvastu","Palpa":"Palpa","Syangja":"Syangja","Arghakhanchi":"Arghakhanchi","Lamjung":"Lamjung","Parbat":"Parbat","Baglung":"Baglung","Pyuthan":"Pyuthan","Rupandehi":"Rupandehi","Surkhet":"Surkhet","Dailekh":"Dailekh","Kalikot":"Kalikot","Mugu":"Mugu","Bajura":"Bajura","Achham":"Achham","Kanchanpur":"Kanchanpur","Dadeldhura":"Dadeldhura"},
    8 : {"Kathmandu":"Kathmandu", "Lalitpur":"Lalitpur", "Bhaktapur":"Bhaktapur"}
}

function numberFormat(number,decimals,dec_point,thousands_sep){var n=!isFinite(+number)?0:+number,prec=!isFinite(+decimals)?0:Math.abs(decimals),sep=(typeof thousands_sep==='undefined')?',':thousands_sep,dec=(typeof dec_point==='undefined')?'.':dec_point,s='',toFixedFix=function(n,prec){var k=Math.pow(10,prec);return''+Math.round(n*k)/k;};s=(prec?toFixedFix(n,prec):''+Math.round(n)).split('.');if(s[0].length>3){s[0]=s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g,sep);}
    if((s[1]||'').length<prec){s[1]=s[1]||'';s[1]+=new Array(prec-s[1].length+1).join('0');}
    return s.join(dec);}

//////function fetchBoundaryAndSalesData() {
//////    $.ajax({
//////        url: 'http://localhost:22395/api/Distribution/GetSalesByDistrictAndAreaBoundary?companyCode=01',
//////        type: "post",
//////       // data: JSON.stringify({action:"getSalesByDistrictAndAreaBoundary",COMPANY_CODE:COMPANY_CODE,BRANCH_CODE:BRANCH_CODE}),
//////        dataType: "json",
//////        success: function(data) {
//////            console.log('sales', data);
//////            try {
//////                if (data.error) {
//////                    throw data.error;
//////                }
//////                if (! data) {
//////                    throw "District sales data not found!!!";
//////                }

//////                // Initialize the variable
//////                sales = {total:0, ds:{}};
////////data.result.sales = {"Kathmandu":"5001","Lalitpur":"4000","Bhaktapur":"1500","Chitwan":"2000","Kavrepalanchok":"1000","Dhading":"300","Makwanpur":"500","Tanahun":"800","Kaski":"2500"};
//////                for(var d in data) {
//////                    sales.total += Number(data[d]);
//////                }
//////                sales.ds = data;
//////                areaBoundary = data.boundary;
//////            }
//////            catch(err) {
//////                //alert(err);
//////            }
//////        },
//////        error: function(e) {

//////        }
//////    });
//////}
function fetchBoundaryAndSalesData() {
    $.ajax({
        url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetSalesByDistrictAndAreaBoundary?companyCode=02",
        type: "post",
        //data: JSON.stringify({ action: "getSalesByDistrictAndAreaBoundary", COMPANY_CODE: COMPANY_CODE, BRANCH_CODE: BRANCH_CODE }),
        dataType: "json",
        success: function (data) {
            console.log('sales', data);
            try {
                if (data.error) {
                    throw data.error;
                }
                if (!data) {
                    throw "District sales data not found!!!";
                }

                // Initialize the variable
                sales = { total: 0, ds: {} };
                //data.result.sales = {"Kathmandu":"5001","Lalitpur":"4000","Bhaktapur":"1500","Chitwan":"2000","Kavrepalanchok":"1000","Dhading":"300","Makwanpur":"500","Tanahun":"800","Kaski":"2500"};
                for (var d in data.sales) {
                    sales.total += Number(data.sales[d]);
                }
                sales.ds = data.sales;
                areaBoundary = data.boundary;
            }
            catch (err) {
                //alert(err);
            }
        },
        error: function (e) {

        }
    });
}
fetchBoundaryAndSalesData();

function initialize() {

    var gradient = [
        [ 0, [238, 238, 238] ],
        [ 28, [244, 238, 25] ],
        [ 72, [255, 131, 44] ],
        [ 100, [255, 0, 0] ]
    ];

    var sliderWidth = 75;
    //var count = 1;
    $('#slider').append('<div class="my-handle ui-slider-handle"><svg height="18" width="14"><path d="M0 0 L7 14 L14 0 Z"/></svg></div>');
    $("#slider").slider({
        min: 1,
        step: 1.333,
        max: 100,
        slide: function (event, ui) {

            var colorRange = []
            $.each(gradient, function (index, value) {
                if (ui.value <= value[0]) {
                    colorRange = [index - 1, index]
                    return false;
                }
            });

            //Get the two closest colors
            var firstcolor = gradient[colorRange[0]][1];
            var secondcolor = gradient[colorRange[1]][1];

            //Calculate ratio between the two closest colors
            var firstcolor_x = sliderWidth * (gradient[colorRange[0]][0] / 100);
            var secondcolor_x = sliderWidth * (gradient[colorRange[1]][0] / 100) - firstcolor_x;
            var slider_x = sliderWidth * (ui.value / 100) - firstcolor_x;
            var ratio = slider_x / secondcolor_x

            //Get the color with pickHex(thx, less.js's mix function!)
            var result = pickHex(secondcolor, firstcolor, ratio);

            //$('#result').css("background-color", 'rgb(' + result.join() + ')');

        }
    });

    function pickHex(color1, color2, weight) {
        var p = weight;
        var w = p * 2 - 1;
        var w1 = (w / 1 + 1) / 2;
        var w2 = 1 - w1;
        var rgb = [Math.round(color1[0] * w1 + color2[0] * w2),
            Math.round(color1[1] * w1 + color2[1] * w2),
            Math.round(color1[2] * w1 + color2[2] * w2)];
        return rgb;
    }

    var myStyle = [
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
    ];

    salesMap = new google.maps.Map(document.getElementById('map-canvas-sales'), {
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
        panControl: false,
        center: new google.maps.LatLng(28.43443095421775, 84.20351049414055),
        zoom: 7,
        minZoom: 7,
        maxZoom: 13,
        mapTypeId: 'custom'
    });

    salesMap.mapTypes.set('custom', new google.maps.StyledMapType(myStyle, { name: 'Distribution' }));

    var geoJsonFiles = ['nepal-development-region.json', 'nepal-district.json', 'nepal-district-headquarters-modified.json'];
    var json = null;

    salesMap.controls[google.maps.ControlPosition.TOP_LEFT].push(document.getElementById('sales-map-fullscreen'));
    salesMap.controls[google.maps.ControlPosition.TOP_RIGHT].push(document.getElementById('sales-district-wrapper'));

    salesMapBounds = new google.maps.LatLngBounds();
    $.getJSON(baseUrl1 + "js/nepal-development-region.json", "", function (res) {
        var cnt = 0;
        var regionFillColor = ['#5EE8A2', '#1A8BEE', '#10FCE2', '#91C917', '#8A2BF9'];
        for(var x in res.features) {
            var regionBounds = new google.maps.LatLngBounds();
            var regionCoords = new google.maps.MVCArray();
            for(var i= 0, coords=res.features[x].geometry.coordinates[0]; i<coords.length; i++) {
                var rcoord = new google.maps.LatLng(coords[i][1], coords[i][0]);
                regionCoords.push(rcoord);
                regionBounds.extend(rcoord);
            }
            var region = res.features[x].properties.REGION;
            // Construct region polygon
            //var rfcolor = regionFillColor[cnt++];
            var rpolygon = new google.maps.Polygon({
                map: salesMap,
                paths: regionCoords,
                strokeColor: '#afafaf',
                strokeOpacity: 1,
                strokeWeight: 1,
                fillColor: regionFillColor[cnt++], //getRandomColor(),
                fillOpacity: 0.2,
                zIndex: 1,
                region: region
            });

            google.maps.event.addListener(rpolygon, 'click', function (e) {
                var z = parseInt(salesMap.getZoom());
                salesMap.setZoom(++z);
                salesMap.setCenter(e.latLng);
            });

            regionPolygon[region] = rpolygon;

            regionPolyline[region] = new google.maps.Polyline({
                map: null,
                path: regionCoords,
                strokeWeight: 2,
                strokeColor: '#afafaf',
                strokeOpacity: 0.4,
                zIndex: 10,
            });

            regionMarker[region] = new MarkerWithLabel({
                map: salesMap,
                icon: {
                    path: google.maps.SymbolPath.CIRCLE,
                    scale: 0
                },
                position: regionBounds.getCenter(),
                draggable: false,
                raiseOnDrag: true,
                labelContent: region,
                labelAnchor: new google.maps.Point(12, 0),
                labelClass: "region-marker-label", // the CSS class for the label
                labelStyle: {opacity: 0.75}
            });
        }
    });
    $.ajax({
        url: baseUrl1 + "js/" + geoJsonFiles[1],
        dataType: 'json',
        success: function(data) {
            json = data;

            var districtWithSales = {}, // Sale percent key and value is marker with label
                districtWithNoSales = {}; // District name key and value is marker with label
            var cnt = 1;
            var heatMapData = [];
            var totalPopulation = 0;

            var dpmInterval = setInterval(function() {
                if (null != sales) {
                    clearInterval(dpmInterval);
                    $.each(json.features, function (key, data) {
                        var districtName = data.properties.DISTRICT.charAt(0).toUpperCase() + data.properties.DISTRICT.slice(1).toLowerCase();
                        locations.push(districtName + ", Nepal");

                        var districtPosition = null;
                        if ("undefined" != typeof districts[districtName]) {
                            totalPopulation += Number(districts[districtName].population);
                            //console.log(key+1, districtName, districts[districtName].population);
                            var coordinate = districts[districtName].coord.split(',');
                            districtPosition = new google.maps.LatLng(Number(coordinate[0]), Number(coordinate[1]));
                            //heatMapData.push(new google.maps.LatLng(Number(coordinate[0]), Number(coordinate[1])));
                            heatMapData.push({
                                location: new google.maps.LatLng(Number(coordinate[0]), Number(coordinate[1])),
                                weight: (100 + cnt++)
                            });
                        }

                        var regionCoords = [];
                        var coord = data.geometry.coordinates;

                        // Define the LatLng coordinates for the polygon.
                        var districtBounds = new google.maps.LatLngBounds();
                        $.each(coord[0], function (k, v) {
                            var lng = v[0];
                            var lat = v[1];
                            regionCoords.push(new google.maps.LatLng(lat, lng));
                            // Region bounds
                            districtBounds.extend(new google.maps.LatLng(lat, lng));
                            salesMapBounds.extend(new google.maps.LatLng(lat, lng));
                        });
                        // Add Label
                        if (!districtBounds.isEmpty()) {
                            districtMarker[districtName] = new MarkerWithLabel({
                                map: null,//salesMap,
                                icon: {
                                    path: google.maps.SymbolPath.CIRCLE,
                                    scale: 1
                                },
                                position: districtPosition, //districtBounds.getCenter(),
                                draggable: false,
                                raiseOnDrag: true,
                                districtName: districtName,
                                labelContent: ("undefined" != typeof zoomMarkerDistrict[salesMap.getZoom()] && "undefined" == typeof zoomMarkerDistrict[salesMap.getZoom()][districtName]) ? districtName : "",
                                labelAnchor: new google.maps.Point(12, 0),
                                labelClass: "marker-label", // the CSS class for the label
                                labelStyle: {opacity: 0.75}
                            });
                            //if ("Kathmandu" == districtName) {
                            //    new MarkerWithLabel({
                            //        map: salesMap,
                            //        icon: {
                            //            path: 'M 271.37385,298.62593 C 0,-23.85937 -19.56797,-43.20121 -43.70628,-43.20121 -24.13831,0 -43.70629,19.34184 -43.70629,43.20121 0,23.85937 19.56798,43.20121 43.70629,43.20121 24.13831,0 43.70628,-19.34184 43.70628,-43.20121 Z',
                            //            scale: 2,
                            //            strokeWeight: 1,
                            //            strokeColor: "#F00",
                            //            fillOpacity: 0.2
                            //        },
                            //        position: districtPosition, //districtBounds.getCenter(),
                            //        draggable: false,
                            //        raiseOnDrag: true,
                            //        districtName: districtName
                            //    });
                            //}

                            google.maps.event.addListener(districtMarker[districtName], 'click', function (e) {

                                if (salesMap.getZoom() < 9) {
                                    salesMap.setZoom(9);
                                }
                                salesMap.setCenter(this.getPosition());
                                if ("undefined" != typeof e && "undefined" != typeof e.isTrigger) {
                                    $('.district-sales-sl').removeClass('district-sales-sl');
                                    var $districtRow = $('#sales-district .district-row span:contains("' + this.districtName + '")').closest('div.district-row');
                                    $districtRow.addClass('district-sales-sl');
                                    $('#sales-district-wrapper').scrollTop(
                                        $districtRow.offset().top - $('#sales-district').offset().top
                                    );
                                }

                                $("#slider").slider('value', overlay.polygon[this.districtName].dps);

                                var dsale = ("undefined" != typeof sales.ds[districtName]) ? sales.ds[districtName] : 0;
                                var dpsale = dsale ? numberFormat(dsale / sales.total * 100, 2) : 0;
                                var dpconsumption = dsale ? dsale / districts[districtName].population * 100 : 0;
                                var content = "<b>" + this.districtName + "</b>"
                                    + "<br/>Population : " + numberFormat(districts[districtName].population) + " (" + numberFormat(districts[districtName].population / totalPopulation * 100, 2) + "%)"
                                    + "<br/>Sales : " + dsale + " Qty. (" + dpsale + "%)";
                                //+ "<br/>Consumption : " + dpconsumption + "%";
                                if (infoWindow instanceof google.maps.InfoWindow) {
                                    infoWindow.setOptions({
                                        map: salesMap,
                                        content: content,
                                        position: this.getPosition()
                                    })
                                }
                                else {
                                    infoWindow = new google.maps.InfoWindow({
                                        position: this.getPosition(),
                                        maxWidth: 700,
                                        content: content
                                    });
                                }
                                infoWindow.open(salesMap);
                            });
                        }

                        var light = {r: 255, g: 237, b: 160};
                        var dark = {r: 252, g: 78, b: 42};
                        var dsale = ("undefined" != typeof sales.ds[districtName]) ? sales.ds[districtName] : 0;
                        var dpsale = dsale ? numberFormat(dsale / sales.total * 100, 2) : 0;
                        var percent = dsale ? dsale / sales.total * 100 : 0; //(key + 1.333);

                        //var fillColor = makeGradientColor(light, dark, dsale).hex;
                        var fillColor = ["#eeeedc", "#efeed2", "#efeec8", "#efeebe", "#f0eeb4", "#f0eeaa", "#f0ee9f", "#f0ee95", "#f1ee8b", "#f1ee81", "#f1ee77", "#f2ee6d", "#f2ee63", "#f2ee58", "#f2ee4e", "#f3ee44", "#f3ee3a", "#f3ee30", "#f4ee26", "#f4ee1c", "#f4ec19", "#f5e81a", "#f5e51b", "#f5e21b", "#f6df1c", "#f6db1c", "#f6d81d", "#f7d51d", "#f7d21e", "#f7ce1f", "#f8cb1f", "#f8c820", "#f8c520", "#f9c121", "#f9be21", "#f9bb22", "#fab823", "#fab423", "#fab124", "#fbae24", "#fbab25", "#fba826", "#fca426", "#fca127", "#fc9e27", "#fd9b28", "#fd9728", "#fd9429", "#fe912a", "#fe8e2a", "#fe8a2b", "#ff872b", "#ff842c", "#ff7e2a", "#ff7828", "#ff7226", "#ff6c24", "#ff6522", "#ff5f20", "#ff591e", "#ff531c", "#ff4d1a", "#ff4618", "#ff4016", "#ff3a13", "#ff3411", "#ff2d0f", "#ff270d", "#ff210b", "#ff1b09", "#ff1407", "#ff0e05", "#ff0803", "#ff0201", "#ff0000"];
                        // Construct the district polygon.
                        var cidx = dsale ? Math.round((dsale / sales.total * 100) / 100 * 75) : 0;
                        var dpolygon = new google.maps.Polygon({
                            map: salesMap,
                            paths: regionCoords,
                            strokeColor: '#afafaf',
                            strokeOpacity: 1,
                            strokeWeight: 1,
                            fillColor: fillColor[cidx], //getRandomColor(),
                            fillOpacity: 1,
                            zIndex: 1,
                            district: districtName,
                            cidx: cidx,
                            dps: (dsale / sales.total * 100)
                        });

                        overlay.polygon[districtName] = dpolygon;
                        google.maps.event.addListener(dpolygon, 'click', function (e) {
                            e.isTrigger = true;
                            e.cidx = this.cidx;
                            google.maps.event.trigger(districtMarker[this.district], 'click', e);
                        });

                        // Districts with no sales
                        if (0 != dsale) {
                            districtWithSales[dpsale] = {
                                districtName: districtName,
                                dsale: dsale,
                                dpsale: dpsale,
                                cidx: cidx,
                                fillColor: fillColor[cidx]
                            }
                        }
                        // Disricts with sales
                        if (0 == dsale) {
                            districtWithNoSales[districtName] = {
                                districtName: districtName,
                                dsale: dsale,
                                dpsale: dpsale,
                                cidx: cidx,
                                fillColor: fillColor[cidx]
                            }
                        }

                        /*********************************************/
                        if ("undefined" != typeof DistMap && "undefined" != typeof DistMap.map) {
                            new google.maps.Polygon({
                                map: DistMap.map,
                                paths: regionCoords,
                                strokeColor: '#524E4E',
                                strokeOpacity: 1,
                                strokeWeight: 1,
                                fillColor: '#eeeeee',
                                fillOpacity: 0
                            });
                        }
                        /*********************************************/
                        // $('#sales-district').append('<div class="district-row" style="padding:3px 5px;border-bottom:1px solid #eee;"><div class="dname" style="float:left;"><span>' + districtName + '</span>' + '&nbsp;' + dpsale + '%' + '</div><div class="dscolor" style="float:right;height:10px;width:10px;background:' + fillColor[cidx] + '"></div><div class="clear" style="clear:both;"></div></div></div>');
                    });

                    var sortedDistrictWithSalesKeys = Object.keys(districtWithSales).sort(function(a, b) {
                        return parseFloat(b) - parseFloat(a);
                    });
                    console.log(sortedDistrictWithSalesKeys);
                    var sortedDistrictWithWithNoSalesKeys = Object.keys(districtWithNoSales).sort();
                    // Add the sorted the legends
                    for(var i= 0; i<sortedDistrictWithSalesKeys.length; i++) {
                        var key = sortedDistrictWithSalesKeys[i];
                        $('#sales-district').append('<div class="district-row" style="padding:3px 5px;border-bottom:1px solid #eee;"><div class="dname" style="float:left;"><span>' + districtWithSales[key].districtName + '</span></div><div style="float:right;"><span>' + districtWithSales[key].dpsale + '%' + '</span>&nbsp;<span class="dscolor" style="display:inline-block;height:10px;width:10px;background:' + districtWithSales[key].fillColor + '"></span></div><div class="clear" style="clear:both;"></div></div></div>');
                    }
                    for(var i= 0; i<sortedDistrictWithWithNoSalesKeys.length; i++) {
                        var key = sortedDistrictWithWithNoSalesKeys[i];
                        $('#sales-district').append('<div class="district-row" style="padding:3px 5px;border-bottom:1px solid #eee;"><div class="dname" style="float:left;"><span>' + districtWithNoSales[key].districtName + '</span></div><div style="float:right;"><span>' + districtWithNoSales[key].dpsale + '%' + '</span>&nbsp;<span class="dscolor" style="display:inline-block;height:10px;width:10px;background:' + districtWithNoSales[key].fillColor + '"></span></div><div class="clear" style="clear:both;"></div></div></div>');
                    }
                    
                    $('.district-row').on('click', function(e) {
                        var $this = $(this);
                        google.maps.event.trigger(districtMarker[$(this).find('.dname span').text()], 'click');
                    });
                }
            }, 500);

            var abInterval = setInterval(function() {
                if (null != areaBoundary) {
                    // Clear the interval
                    clearInterval(abInterval);

                    for (var area in areaBoundary) {
                        // DRAW AREA BOUNDARY
                        if (areaBoundary[area].data) {
                            var parr = [];
                            $.each(areaBoundary[area].data.split(':'), function (k, v) {
                                var coord = v.split(',');
                                var lng = Number(coord[0]);
                                var lat = Number(coord[1]);
                                parr.push(new google.maps.LatLng(lat, lng));
                            });
                            parr.push(parr[0]);
                            var ap = new google.maps.Polygon({
                                map: salesMap,
                                paths: parr,
                                strokeColor: '#524E4E',
                                strokeOpacity: 1,
                                strokeWeight: 1,
                                fillColor: getRandomColor(), //getRandomColor(),
                                fillOpacity: 1,
                                area: area,
                                zIndex: 9,
                                district: areaBoundary[area].district
                            });
                            areaPolygon[area] = ap;

                            google.maps.event.addListener(ap, 'click', function(e) {
                                var content = "<b>" + this.area + "</b>"
                                    + "<br/>" + this.district + " District";
                                if (infoWindow instanceof google.maps.InfoWindow) {
                                    infoWindow.setOptions({
                                        map: salesMap,
                                        content: content,
                                        position: e.latLng
                                    })
                                }
                                else {
                                    infoWindow = new google.maps.InfoWindow({
                                        position: e.latLng,
                                        maxWidth: 700,
                                        content: content
                                    });
                                }
                                infoWindow.open(salesMap);
                            });

                        }
                    }
                }
            }, 1000);

            // Zoom change event listener
            google.maps.event.addListener(salesMap, 'zoom_changed', function() {
                var z = salesMap.getZoom();
                var districtLabelToHide = {};
                var hideDistrictMarker = true;
                switch ( true ) {
                    case ( 8 == z ) :
                        for(var zlevel in zoomMarkerDistrict) {
                            if(zlevel >= z) {
                                for(var district in zoomMarkerDistrict[zlevel]) {
                                    districtLabelToHide[district] = zoomMarkerDistrict[zlevel][district];
                                }
                            }
                        }
                        hideDistrictMarker = true;
                        break;
                    case (7 >= z) :
                        for(var zlevel in zoomMarkerDistrict) {
                            if(zlevel >= z) {
                                for(var district in zoomMarkerDistrict[zlevel]) {
                                    districtLabelToHide[district] = zoomMarkerDistrict[zlevel][district];
                                }
                            }
                        }
                        hideDistrictMarker = true;
                        break;
                    case (9 <= z) :
                        // Display the remaining label
                        for(var zlevel in zoomMarkerDistrict) {
                            if(zlevel >= z) {
                                for(var district in zoomMarkerDistrict[zlevel]) {
                                    districtLabelToHide[district] = zoomMarkerDistrict[zlevel][district];
                                }
                            }
                        }
                        hideDistrictMarker = false;
                        break;
                    default :
                        hideDistrictMarker = true;
                        break;
                }

                for(var district in districtMarker) {
                    if ("undefined" != typeof districtLabelToHide[district]) {
                        districtMarker[district].labelContent = "";
                    }
                    else {
                        districtMarker[district].labelContent = district;
                    }

                    /*********************************************/
                    // Display the district marker accordingly
                    if (hideDistrictMarker) {
                        districtMarker[district].setOptions({
                            map: null
                        });
                        // District polygon
                        overlay.polygon[district].setOptions({
                            zIndex: 1
                        });
                    }
                    else {
                        districtMarker[district].setOptions({
                            map: salesMap
                        });
                        // District polygon
                        overlay.polygon[district].setOptions({
                            zIndex: 2
                        });
                    }
                    /*********************************************/
                }

                for(var region in regionMarker) {
                    if(hideDistrictMarker) {
                        // Display the region label
                        regionMarker[region].setOptions({
                            map: salesMap
                        });

                        regionPolygon[region].setOptions({
                            zIndex: 2
                        });
                        regionPolyline[region].setOptions({
                            map: null
                        });
                    }
                    else {
                        regionMarker[region].setMap(null);
                        regionPolygon[region].setOptions({
                            //map: null
                            zIndex: 0
                        });
                        // Draw region polyline
                        regionPolyline[region].setOptions({
                            map: salesMap
                        });
                    }
                }

            });
        }
    });

    $('#sales-map-fullscreen').on('click', function() {
        var mapCenter = DistMap.map.getCenter();
        if (! $(this).is('.map-fullscreen') ) {
            $('body').css({
                overflow: 'hidden'
            });

            $('#slider').css({zIndex: 2});
            $('#sales-district-wrapper').css({marginTop: 0, zIndex: 2});

            $('#sales-map-panel').css({
                position: "fixed",
                //top: $('.navbar-static-top').height() + 'px',
                top: 0,
                left: 0,
                //height: $(window).height() - $('.navbar-static-top').height() + 'px', //"100%",
                height: $(window).height(),
                width: "100%",
                zIndex: 9999,
                margin: "0 auto"
            });
            $(this).addClass('map-fullscreen');
        }
        else {
            $('#slider').css({zIndex: 2});
            $('#sales-district-wrapper').css({zIndex: 2});

            $('#sales-map-panel').css({
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
        google.maps.event.trigger(salesMap, "resize");
        salesMap.setCenter(mapCenter);
    });

}

function getRandomColor() {
    var letters = '0123456789ABCDEF'.split('');
    var color = '#';
    for (var i = 0; i < 6; i++ ) {
        color += letters[Math.round(Math.random() * 15)];
    }
    return color;
}


$(document).ready(function () {    
    initialize()
    google.maps.event.addListenerOnce(salesMap, 'idle', function () {
        //loaded fully        
        document.getElementById('sales-map-fullscreen').style.display = 'block';
        document.getElementById('sales-district-wrapper').style.display = 'block';
    });   
    google.maps.event.addDomListener(window, "resize", function () {       
        google.maps.event.trigger(salesMap, "resize");
        salesMap.setCenter(salesMap.getCenter());
        if (null != salesMapBounds) {
            salesMap.fitBounds(salesMapBounds);
        }
    });
});


function makeGradientColor(color1, color2, percent) {
    var newColor = {};

    function makeChannel(a, b) {
        return (a + Math.round((b - a) * (percent / 100)));
    }

    function makeColorPiece(num) {
        num = Math.min(num, 255);   // not more than 255
        num = Math.max(num, 0);     // not less than 0
        var str = num.toString(16);
        if (str.length < 2) {
            str = "0" + str;
        }
        return (str);
    }

    newColor.r = makeChannel(color1.r, color2.r);
    newColor.g = makeChannel(color1.g, color2.g);
    newColor.b = makeChannel(color1.b, color2.b);
    newColor.hex = "#" +
        makeColorPiece(newColor.r) +
        makeColorPiece(newColor.g) +
        makeColorPiece(newColor.b);

    return (newColor);
}
