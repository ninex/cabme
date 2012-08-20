var computedDistance = 0;
var service, geocoder, directionsService;
var bookHub;
var isFull = false;
var lstOfsuburbs = {};

$(document).ready(function () {
    window.hubReady.done(function () {
        $('#loading').hide();
        $('#step1').fadeIn();
    });
    bookHub = $.connection.bookHub;
    bookHub.showMessage = function (message) {
        $('#msgStatus').append('<p class="status">' + message + '</p>');
    };
    $('[full]').hide();
    $('#step2').hide();

    //loadQuickTaxis();
    //loadSuburbs();

    ko.applyBindings(new BookingViewModel());
});
function Booking() {
    var self = this;

    var from = '', phoneNumber = '', city = 'Cape Town';
    if (supports_html5_storage()) {
        from = localStorage["from"];
        phoneNumber = localStorage["phone"];
        if (localStorage["city"] && localStorage["city"] != "null") {
            city = localStorage["city"];
        }
    }
    var now = new Date();
    var today = now.getFullYear() + '-' + (now.getMonth() + 1).padLeft(2, '0') + '-' + now.getDate().padLeft(2, '0');
    var time = now.getHours().padLeft(2, '0') + ':' + now.getMinutes().padLeft(2, '0');

    self.id = 0;
    self.phoneNumber = ko.observable(phoneNumber);
    self.numberOfPeople = ko.observable(1);
    self.pickupDate = ko.observable(today);
    self.pickupTime = ko.observable(time);
    self.city = ko.observable(city);
    self.suburbFrom = ko.observable('');
    self.suburbTo = ko.observable('');
    self.addrFrom = ko.observable(from);
    self.addrTo = ko.observable('');
    self.confirmed = ko.observable(false);
    self.arrival = ko.observable('');
    self.quickTaxi = ko.observable();
    self.suburbFrom.subscribe(function (newValue) {
        var city = self.city();
        localStorage[city + 'suburbFrom'] = newValue;
    });
    self.suburbTo.subscribe(function (newValue) {
        var city = self.city();
        localStorage[city + 'suburbTo'] = newValue;
    });
}
function Taxi(id, name) {
    var self = this;
    self.id = id;
    self.name = name;
}
function Suburb(id, name, city, postalCode) {
    var self = this;
    self.id = id;
    self.name = name;
    self.city = city;
    self.postalCode = postalCode;
    self.fullAddress = name + ', ' + city + ', ' + postalCode;
}
function BookingViewModel() {
    var self = this;
    self.taxis = ko.observableArray();
    self.suburbs = ko.observableArray();
    self.booking = ko.observable(new Booking());
    self.loadQuickTaxi = function () {
        $.getJSON('/service/cabmeservice.svc/taxis', function (json) {
            $.each(json, function (index, taxi) {
                self.taxis.push(new Taxi(taxi.Id, taxi.Name));
            });
        });
    };
    self.loadSuburbs = function () {
        self.suburbs.removeAll();
        var refresh = true;
        var city = self.booking().city();
        if (supports_html5_storage()) {
            localStorage['city'] = city;
            if (localStorage[city + "suburbStamp"] && localStorage[city + "suburbStamp"] != "null" && localStorage[city + "suburbList"] && localStorage[city + "suburbList"] != "null") {
                var orig = new Date().setTime(localStorage[city + "suburbStamp"]);
                var now = +new Date();
                days = Math.round((now - orig) / (1000 * 60 * 60 * 24));
                refresh = days > 30;
            }
        }
        if (refresh) {
            var url = '/service/cabmeservice.svc/suburbs?city=' + city;
            $.getJSON(url, function (json) {
                self.parseSuburbs(self.booking().city(), json);
            });
        } else {
            self.parseSuburbs(city, $.parseJSON(localStorage[city + "suburbList"]));
        }
    };
    self.parseSuburbs = function (city, lst) {
        $.each(lst, function (index, suburb) {
            self.suburbs.push(new Suburb(suburb.Id, suburb.Name, suburb.City, suburb.PostalCode));
        });
        localStorage[city + "suburbList"] = JSON.stringify(lst);
        localStorage[city + "suburbStamp"] = +new Date();
        self.setPrevSuburbs(city);
    };
    self.setPrevSuburbs = function (city) {
        var suburb = localStorage[city + 'suburbFrom'];
        self.booking().suburbFrom(suburb);
        suburb = localStorage[city + 'suburbTo'];
        self.booking().suburbTo(suburb);
    };
    self.booking().city.subscribe(function (newValue) {
        if (supports_html5_storage()) {
            localStorage.removeItem(self.booking().city() + "suburbStamp");
        }
        self.loadSuburbs();
    });
    self.step1Min = function () {
        var msg = "";
        var regNum = /^[0-9]+$/;
        if (!regNum.test(self.booking().phoneNumber())) {
            msg += 'Invalid phone number.<br/>';
            return;
        }
        if (self.booking().addrFrom().length <= 0) {
            msg += "Please provide a pickup address.<br/>";
        }
        if (msg.length > 0) {
            popup('Error', msg);
            return;
        }

        if (supports_html5_storage()) {
            localStorage["from"] = self.booking().addrFrom();
            localStorage["phone"] = self.booking().phoneNumber();
        }
        $('#step1').slideUp();
        $('#loading').show();

        var suburbFrom = ko.utils.arrayFirst(self.suburbs(), function (suburb) {
            return suburb.fullAddress === self.booking().suburbFrom();
        });

        var data = {
            "PhoneNumber": self.booking().phoneNumber(),
            "NumberOfPeople": 1,
            "AddrFrom": self.booking().addrFrom() + ', ' + self.booking().suburbFrom(),
            "ComputedDistance": 0,
            "Active": true,
            "Confirmed": false,
            "TaxiId": self.booking().quickTaxi().id,
            "SuburbFromId": suburbFrom != null ? suburbFrom.id : 0
        };

        $('#step3').show();
        $('#loading').hide();

        bookHub.announce(self.booking().phoneNumber()).done(function () {
            $.ajax({
                type: "POST",
                contentType: 'application/json',
                url: '/service/cabmeservice.svc/booking',
                data: JSON.stringify(data),
                success: function (msg) {
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log(errorThrown);
                    popup('Server error', 'The booking can not be created due to a server problem.');
                }
            });
        });
    };
    self.loadQuickTaxi();
    self.loadSuburbs();
}
/*
function loadMapScript() {
var script = document.createElement('script');
script.type = 'text/javascript';
script.src = 'http://maps.googleapis.com/maps/api/js?key=AIzaSyB2SQU0TkPbS1-MT4D8fkdvZ4J0JkIeBf8&sensor=false&callback=mapsLoaded';
document.body.appendChild(script);
}
window.onload = loadMapScript;

function mapsLoaded() {
geocoder = new google.maps.Geocoder();
service = new google.maps.DistanceMatrixService();
directionsService = new google.maps.DirectionsService();
}*/

function makeFull() {
    if (!isFull) {
        $('[full]').show();
        $('#lblQuickTaxi').hide();
        $('#btnBookMin').hide();
        $('#btnMakeFull').html('Press here to switch to quick booking.');
        isFull = true;
    } else {
        $('[full]').hide();
        $('#lblQuickTaxi').show();
        $('#btnBookMin').show();
        $('#btnMakeFull').html('Press here to switch to detailed booking.');
        isFull = false;
    }
}

function step1() {
    var pickupDate = $('#pickupDate').val();
    var pickupTime = $('#pickupTime').val();
    var numPeople = $('#number').val();
    var origin = $('#from').val();
    var destination = $('#to').val();

    var msg = "";
    var regDate = /^[0-9]{4}-[0-9]{2}-[0-9]{2}$/;
    var regTime = /^[0-9]{2}:[0-9]{2}$/;
    var regNum = /^[0-9]+$/;
    if (!regDate.test(pickupDate)) {
        msg += "Invalid date specified.<br/>";
    }
    if (!regTime.test(pickupTime)) {
        msg += "Invalid time specified.<br/>";
    }
    if (!regNum.test(numPeople)) {
        msg += "Invalid number of people specified.<br/>";
    }
    if (origin.length <= 0 || destination.length <= 0) {
        msg += "Please provide an address from and to.<br/>";
    }
    if (!regNum.test($('#txtPhone').val())) {
        msg += 'Invalid phone number.<br/>';
        return;
    }
    if (msg.length > 0) {
        popup('Error', msg);
        return;
    }

    bookHub.announce($('#txtPhone').val());
    $('#loading').show();

    origin += ', ' + $('#fromSuburb').attr('selected', true).val();
    destination += ', ' + $('#toSuburb').attr('selected', true).val();

    service.getDistanceMatrix(
			  {
			      origins: [origin],
			      destinations: [destination],
			      travelMode: google.maps.TravelMode.DRIVING,
			      avoidHighways: false,
			      avoidTolls: false
			  }, distanceResults);
}
function step2() {

    $('#step2').slideUp();
    if (supports_html5_storage()) {
        localStorage["from"] = $('#from').val();
        localStorage["phone"] = $('#txtPhone').val();
    }

    $('#loading').show();
    var taxiId = $('#ddlTaxi').attr('selected', true).val();
    var data = {
        "PhoneNumber": $('#txtPhone').val(),
        "NumberOfPeople": $('#number').val(),
        "PickupTime": $('#pickupDate').val() + ' ' + $('#pickupTime').val() + ':00',
        "AddrFrom": $('#from').val() + ', ' + $('#fromSuburb').attr('selected', true).val(),
        "AddrTo": $('#to').val() + ', ' + $('#toSuburb').attr('selected', true).val(),
        "ComputedDistance": computedDistance,
        "Active": true,
        "Confirmed": false,
        "TaxiId": taxiId,
        "SuburbFromId": lstOfsuburbs[$('#fromSuburb').attr('selected', true).val()]
    };

    $('#step3').show();
    $('#loading').hide();

    $.ajax({
        type: "POST",
        contentType: 'application/json',
        url: '/service/cabmeservice.svc/booking',
        data: JSON.stringify(data),
        success: function (msg) {
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(errorThrown);
            popup('Server error', 'The booking can not be created due to a server problem.');
        }
    });
}

function distanceResults(response, status) {
    if (status == google.maps.DistanceMatrixStatus.OK) {
        var origins = response.originAddresses;
        var destinations = response.destinationAddresses;

        var element = response.rows[0].elements[0];
        if (element.status != 'ZERO_RESULTS') {
            var distance = element.distance.text;
            var duration = element.duration.text;
            var from = origins[0];
            var to = destinations[0];
            computedDistance = element.distance.value;
            $('#txtDistance').html(distance);

            $.getJSON('/service/cabmeservice.svc/taxis?distance=' + computedDistance, function (json) {
                var options = '';
                $.each(json, function (index, taxi) {
                    options += '<option value="' + taxi.Id + '">' + taxi.Name + ' - R' + (taxi.PriceEstimate / 100) + '</option>';
                });
                $('#ddlTaxi').html(options);
                $('#step1').slideUp();
                $('#step2').show();
                var request = {
                    origin: $('#from').val() + ', ' + $('#fromSuburb').attr('selected', true).val(),
                    destination: $('#to').val() + ', ' + $('#toSuburb').attr('selected', true).val(),
                    travelMode: google.maps.TravelMode.DRIVING
                };
                directionsService.route(request, function (result, status) {
                    if (status == google.maps.DirectionsStatus.OK) {
                        var width = $(document).width();
                        if (width > 480) {
                            width = 480;
                        }
                        $('#map').html('<img style="max-width:100%;max-height:100%;" src="http://maps.googleapis.com/maps/api/staticmap?size=' + width + 'x400&sensor=false&path=weight:5|color:blue|enc:' + result.routes[0].overview_polyline.points + '&markers=label:A|' + from + '&markers=label:B|' + to + '" />');
                    }
                });
                $('#loading').hide();
            });
        } else {
            $('#loading').hide();
            popup('Check Address', 'Can not find both addresses. Please double check.');
        }
    } else {
        $('#loading').hide();
        popup('Server problem', 'We are having problems accessing the Google Maps service');
    }

}
function popup(header, msg) {
    $('<div id="pop" class="popContainer"><div class="pop"><h1>' + header + '</h1><p>' + msg + '</p><input type="button" class="button" value="OK" onclick="removePop();" /></div></div>').insertAfter('#loading');
}
function removePop() {
    $('#pop').remove();
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