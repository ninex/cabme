var model, bookHub;

$(document).ready(function () {
    bookHub = $.connection.bookHub;
    bookHub.showMessage = function (message) {
        $('#msgStatus').append('<p>' + message + '</p>');
    };
    bookHub.confirmBooking = function (time) {
        if (time && time > 0) {
            model.booking().taxiAccepted(true);
            model.booking().waitingTime(time);
        } else {
            $('#msgStatus').append('<p>Booking has been confirmed</p>');
        }
    };
    bookHub.cancelBooking = function () {
        model.booking().taxiCancelled(true);
        model.booking().switchTaxi(true);
        model.step2(false);
        model.step1(true);
    }
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
    self.addrFrom = ko.observable('');
    self.addrTo = ko.observable('');
    self.computedDistance = ko.observable('');
    self.displayDistance = ko.observable('');
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
    self.step1 = ko.observable(true);
    self.step2 = ko.observable(false);
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
            distanceService.getDistanceMatrix(
			  {
			      origins: [self.pickupMarker.getPosition()],
			      destinations: [self.dropMarker.getPosition()],
			      travelMode: google.maps.TravelMode.DRIVING,
			      avoidHighways: false,
			      avoidTolls: false
			  }, self.distanceResults);
        }
    };
    self.distanceResults = function (response, status) {
        if (status == google.maps.DistanceMatrixStatus.OK) {
            self.booking().addrFrom(response.originAddresses[0]);
            self.booking().addrTo(response.destinationAddresses[0]);

            var element = response.rows[0].elements[0];
            if (element.status != 'ZERO_RESULTS') {
                var distance = element.distance.text;
                var duration = element.duration.text;
                self.booking().displayDistance(distance);
                self.booking().computedDistance(element.distance.value);

                $.getJSON('/api/taxi/?distance=' + self.booking().computedDistance(), function (json) {
                    self.taxis.removeAll();
                    $.each(json, function (index, taxi) {
                        self.taxis.push(new Taxi(taxi.Id, taxi.Name, taxi.PriceEstimate));
                    });
                });
            }
        }
    };
    self.book = function () {
        var msg = "";
        var regNum = /^[0-9]+$/;
        if (!regNum.test(self.booking().phoneNumber())) {
            msg += 'Invalid phone number.<br/>';
            return;
        }
        if (self.booking().addrFrom().length <= 0 && self.booking().addrTo().length <= 0) {
            msg += "Please select a pickup and drop location.<br/>";
        }
        if (msg.length > 0) {
            popup('Error', msg);
            return;
        }
        if (supports_html5_storage()) {
            localStorage["phone"] = self.booking().phoneNumber();
        }
        var data = {
            "PhoneNumber": self.booking().phoneNumber(),
            "NumberOfPeople": self.booking().numberOfPeople(),
            "PickupTime": self.booking().pickupDate() + ' ' + self.booking().pickupTime() + ':00',
            "AddrFrom": self.booking().addrFrom(),
            "AddrTo": self.booking().addrTo(),
            "ComputedDistance": self.booking().computedDistance(),
            "Active": true,
            "TaxiAccepted": self.booking().taxiAccepted(),
            "UserAccepted": self.booking().userAccepted(),
            "TaxiCancelled": self.booking().taxiCancelled(),
            "UserCancelled": self.booking().userCancelled(),
            "TaxiId": self.booking().taxi().id/*,
            "latitudeFrom": (int)(self.pickupMarker.getPosition().lat() * 1E6),
            "longitudeFrom": (int)(self.pickupMarker.getPosition().lng() * 1E6),
            "latitudeTo": (int)(self.dropMarker.getPosition().lat() * 1E6),
            "longitudeTo": (int)(self.dropMarker.getPosition().lng() * 1E6),*/
        };
        bookHub.announce(self.booking().phoneNumber()).done(function () {
            $.ajax({
                type: "POST",
                contentType: 'application/json',
                url: '/api/booking',
                data: JSON.stringify(data),
                success: function (msg) {
                    if (msg) {
                        self.booking().id(msg.Id);
                        self.step1(false);
                        self.step2(true);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log(errorThrown);
                    popup('Server error', 'The booking can not be created due to a server problem.');
                }
            });
        });
    };
    self.accept = function () {
        var data = {
            "UserAccepted": true,
            "TaxiAccepted": self.booking().taxiAccepted(),
            "Id": self.booking().id(),
            "PhoneNumber": self.booking().phoneNumber(),
            "NumberOfPeople": self.booking().numberOfPeople(),
            "AddrFrom": self.booking().addrFrom(),
            "TaxiId": self.booking().taxi().id
        };
        $.ajax({
            type: "PUT",
            contentType: 'application/json',
            url: '/api/booking/' + self.booking().id(),
            data: JSON.stringify(data),
            success: function (msg) {
                self.booking().userAccepted(true);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(errorThrown);
                popup('Server error', 'The booking has not been accepted due to a server problem.');
            }
        });
    };
    self.cancel = function () {
        var data = {
            "UserCancelled": true,
            "Id": self.booking().id(),
            "PhoneNumber": self.booking().phoneNumber(),
            "NumberOfPeople": self.booking().numberOfPeople(),
            "AddrFrom": self.booking().addrFrom(),
            "TaxiId": self.booking().taxi().id
        };
        $.ajax({
            type: "PUT",
            contentType: 'application/json',
            url: '/api/booking/' + self.booking().id(),
            data: JSON.stringify(data),
            success: function (msg) {
                self.booking().userCancelled(true);
                self.booking().switchTaxi(true);
                self.step2(false);
                self.step1(true);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(errorThrown);
                popup('Server error', 'The booking has not been cancelled due to a server problem.');
            }
        });
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
    distanceService = new google.maps.DistanceMatrixService();
    model.loadMap();
}
function popup(header, msg) {
    $('<div id="pop" class="popContainer"><div class="pop"><h1>' + header + '</h1><p>' + msg + '</p><input type="button" class="button" value="OK" onclick="removePop();" /></div></div>').insertAfter('#loading');
}
function removePop() {
    $('#pop').remove();
}
ko.bindingHandlers.slide = {
    'update': function (element, valueAccessor) {
        if (valueAccessor()) {
            $(element).slideDown();
        } else {
            $(element).slideUp();
        }
    }
};
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