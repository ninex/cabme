var model;

$(document).ready(function () {
    model = new BookingViewModel();
    ko.applyBindings(model);
});

function Booking() {
    var self = this;

    var from = '', phoneNumber = '';
    if (supports_html5_storage()) {
        from = localStorage["from"];
        phoneNumber = localStorage["phone"];        
    }
    var now = new Date();
    var today = now.getFullYear() + '-' + (now.getMonth() + 1).padLeft(2, '0') + '-' + now.getDate().padLeft(2, '0');
    var time = now.getHours().padLeft(2, '0') + ':' + now.getMinutes().padLeft(2, '0');

    self.id = ko.observable(0);
    self.phoneNumber = ko.observable(phoneNumber);
    self.numberOfPeople = ko.observable(1);
    self.pickupDate = ko.observable(today);
    self.pickupTime = ko.observable(time);
    self.computedDistance = ko.observable('');
    self.taxiAccepted = ko.observable(false);
    self.userAccepted = ko.observable(false);
    self.taxiCancelled = ko.observable(false);
    self.userCancelled = ko.observable(false);
    self.switchTaxi = ko.observable(false);
    self.taxi = ko.observable();
    self.waitingTime = ko.observable(0);
    self.expectedArrival = ko.computed(function () {
        var now = new Date();
        now.setMinutes(now.getMinutes() + self.waitingTime());
        return now.getHours().padLeft(2, '0') + ':' + now.getMinutes().padLeft(2, '0');
    }, self);
}
function Taxi(id, name, estimate) {
    var self = this;
    self.id = id;
    self.name = name;
    self.estimated = ko.observable(estimate);
    self.display = ko.computed(function () {
        if (self.estimated() > 0) {
            return self.name + ' - R' + (parseInt(self.estimated()) / 100);
        } else {
            return self.name;
        }
    }, self);
}
function BookingViewModel() {
    var self = this;
    var map, pickupMarker, dropMarker, route;
    self.booking = ko.observable(new Booking());
    self.taxis = ko.observableArray();
    self.loadQuickTaxi = function () {
        $.getJSON('/api/taxi', function (json) {
            $.each(json, function (index, taxi) {
                self.taxis.push(new Taxi(taxi.Id, taxi.Name, ''));
            });
        });
    };
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
    self.book = function () {
    };
    self.geoAllowed = function (position) {
        var latlng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
        self.map.setCenter(latlng);
    };
    self.geoNotAllowed = function () {
        //default to Cape Town
        var latlng = new google.maps.LatLng(-33.9270, 18.4244);
        self.map.setCenter(latlng);
    };
    self.loadQuickTaxi();
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

Number.prototype.padLeft = function (width, char) {
    if (!char) {
        char = " ";
    }

    if (("" + this).length >= width) {
        return "" + this;
    }
    else {
        return arguments.callee.call(char + this, width, char);
    }
};
function supports_html5_storage() {
    try {
        return 'localStorage' in window && window['localStorage'] !== null;
    } catch (e) {
        return false;
    }
}
//load the script async
window.onload = loadMapScript;