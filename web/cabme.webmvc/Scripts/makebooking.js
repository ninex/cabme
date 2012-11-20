var map, pickupMarker, dropMarker, route;

function loadMapScript() {
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = 'http://maps.googleapis.com/maps/api/js?key=AIzaSyB2SQU0TkPbS1-MT4D8fkdvZ4J0JkIeBf8&sensor=false&callback=APILoaded';
    document.body.appendChild(script);
}
function APILoaded() {
    directionsService = new google.maps.DirectionsService();
    var mapOptions = {
        zoom: 15,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    }
    map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
    google.maps.event.addListener(map, 'click', function (event) {
        mapZoom = map.getZoom();
        clickLocation = event.latLng;
        setTimeout("placeMarker()", 100);
    });

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(geoAllowed, geoNotAllowed);
    } else {
        geoNotAllowed();
    }
}
function geoAllowed(position) {
    var latlng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
    map.setCenter(latlng);
}
function geoNotAllowed() {
    //default to Cape Town
    var latlng = new google.maps.LatLng(-33.9270, 18.4244);
    map.setCenter(latlng);
}
function placeMarker() {
    if (mapZoom == map.getZoom()) {
        if (!pickupMarker) {
            pickupMarker = new google.maps.Marker({
                position: clickLocation,
                map: map,
                draggable: true,
                icon: 'Images/map_pickup.png'
            });
            google.maps.event.addListener(
                pickupMarker,
                'dragend',
                function (event) {
                    recalcRoute();
                }
            );
        } else {
            if (!dropMarker) {
                dropMarker = new google.maps.Marker({
                    position: clickLocation,
                    map: map,
                    draggable: true,
                    icon: 'Images/map_drop.png'
                });
                google.maps.event.addListener(
                    dropMarker,
                    'dragend',
                    function (event) {
                        recalcRoute();
                    }
                );
                recalcRoute();
            }
        }
    }
}
function recalcRoute() {
    if (pickupMarker && dropMarker) {
        var request = {
            origin: pickupMarker.getPosition(),
            destination: dropMarker.getPosition(),
            travelMode: google.maps.TravelMode.DRIVING
        };
        directionsService.route(request, function (result, status) {
            if (route) {
                route.setMap(null);
            }
            if (status == google.maps.DirectionsStatus.OK) {
                route = new google.maps.Polyline({
                    path: result.routes[0].overview_path,
                    map: map,
                    strokeWeight: 3,
                    strokeColor: 'blue'
                });
            }
        });
    }
}
//load the script async
window.onload = loadMapScript;