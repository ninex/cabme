var model;

$(document).ready(function () {
    model = new BookingViewModel();
    ko.applyBindings(model);
});

function BookingViewModel() {
    var self = this;
    var map, pickupMarker, dropMarker, route;
    self.loadMap = function () {
        var mapOptions = {
            zoom: 15,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        }
        self.map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
        google.maps.event.addListener(self.map, 'click', function (event) {
            mapZoom = self.map.getZoom();
            clickLocation = event.latLng;
            setTimeout(self.placeMarker(), 100);
        });

        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(self.geoAllowed, self.geoNotAllowed);
        } else {
            self.geoNotAllowed();
        }
    };
    self.placeMarker = function () {
        if (mapZoom == self.map.getZoom()) {
            if (!self.pickupMarker) {
                self.pickupMarker = new google.maps.Marker({
                    position: clickLocation,
                    map: self.map,
                    draggable: true,
                    icon: 'Images/map_pickup.png'
                });
                google.maps.event.addListener(
                self.pickupMarker,
                'dragend',
                function (event) {
                    self.recalculateRoute();
                }
            );
            } else {
                if (!self.dropMarker) {
                    self.dropMarker = new google.maps.Marker({
                        position: clickLocation,
                        map: self.map,
                        draggable: true,
                        icon: 'Images/map_drop.png'
                    });
                    google.maps.event.addListener(
                    self.dropMarker,
                    'dragend',
                    function (event) {
                        self.recalculateRoute();
                    }
                );
                    self.recalculateRoute();
                }
            }
        }
    };
    self.recalculateRoute = function () {
        if (self.pickupMarker && self.dropMarker) {
            var request = {
                origin: self.pickupMarker.getPosition(),
                destination: self.dropMarker.getPosition(),
                travelMode: google.maps.TravelMode.DRIVING
            };
            directionsService.route(request, function (result, status) {
                if (route) {
                    route.setMap(null);
                }
                if (status == google.maps.DirectionsStatus.OK) {
                    route = new google.maps.Polyline({
                        path: result.routes[0].overview_path,
                        map: self.map,
                        strokeWeight: 3,
                        strokeColor: 'blue'
                    });
                }
            });
        }
    };
    self.geoAllowed = function(position) {
        var latlng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
        self.map.setCenter(latlng);
    }
    self.geoNotAllowed = function() {
        //default to Cape Town
        var latlng = new google.maps.LatLng(-33.9270, 18.4244);
        self.map.setCenter(latlng);
    }
}

function loadMapScript() {
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = 'http://maps.googleapis.com/maps/api/js?key=AIzaSyB2SQU0TkPbS1-MT4D8fkdvZ4J0JkIeBf8&sensor=false&callback=APILoaded';
    document.body.appendChild(script);
}
function APILoaded() {
    directionsService = new google.maps.DirectionsService();
    model.loadMap();
}

//load the script async
window.onload = loadMapScript;