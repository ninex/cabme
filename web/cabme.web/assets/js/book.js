var service, geocoder, directionsService;
var bookHub;
var isMapsLoaded = false;
var model;

$(document).ready(function () {
	window.hubReady.done(function () {
		$('#loading').hide();
		$('#step1').fadeIn();
	});
	bookHub = $.connection.bookHub;
	bookHub.showMessage = function (message) {
		$('#msgStatus').append('<p class="status">' + message + '</p>');
	};
	bookHub.confirmBooking = function (time) {
		if (time && time > 0) {
			model.booking().confirmed(true);
			model.booking().waitingTime(time);
		} else {
			$('#msgStatus').append('<p class="status">Booking has been confirmed</p>');
		}
	};
	model = new BookingViewModel();
	ko.applyBindings(model);
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

	self.id = ko.observable(0);
	self.phoneNumber = ko.observable(phoneNumber);
	self.numberOfPeople = ko.observable(1);
	self.pickupDate = ko.observable(today);
	self.pickupTime = ko.observable(time);
	self.city = ko.observable(city);
	self.suburbFrom = ko.observable('');
	self.suburbTo = ko.observable('');
	self.addrFrom = ko.observable(from);
	self.addrTo = ko.observable('');
	self.computedDistance = ko.observable('');
	self.displayDistance = ko.observable('');
	self.confirmed = ko.observable(false);
	self.accepted = ko.observable(false);
	self.switchTaxi = ko.observable(false);
	self.quickTaxi = ko.observable();
	self.taxi = ko.observable();
	self.full = ko.observable(false);
	self.waitingTime = ko.observable(0);
	self.pickup = ko.computed(function () {
		return self.addrFrom() + ', ' + self.suburbFrom();
	}, self);
	self.drop = ko.computed(function () {
		return self.addrTo() + ', ' + self.suburbTo();
	}, self);
	self.expectedArrival = ko.computed(function () {
		var now = new Date();
		now.setMinutes(now.getMinutes() + self.waitingTime());
		return now.getHours().padLeft(2, '0') + ':' + now.getMinutes().padLeft(2, '0');
	}, self);
	self.suburbFrom.subscribe(function (newValue) {
		var city = self.city();
		localStorage[city + 'suburbFrom'] = newValue;
	});
	self.suburbTo.subscribe(function (newValue) {
		var city = self.city();
		localStorage[city + 'suburbTo'] = newValue;
	});
	self.full.subscribe(function (newValue) {
		if (newValue) {
			if (!isMapsLoaded) {
				loadMapScript();
			}
			$('[full]').show();
			$('#lblQuickTaxi').hide();
			$('#btnBookMin').hide();
			$('#btnMakeFull').html('Switch to quick booking.');
		} else {
			$('[full]').hide();
			$('#lblQuickTaxi').show();
			$('#btnBookMin').show();
			$('#btnMakeFull').html('Switch to detailed booking.');
		}
	});

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
				self.taxis.push(new Taxi(taxi.Id, taxi.Name, ''));
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
	        "AddrFrom": self.booking().pickup(),
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
	                if (msg) {
	                    self.booking().id(msg.Id);
	                }
	            },
	            error: function (jqXHR, textStatus, errorThrown) {
	                console.log(errorThrown);
	                popup('Server error', 'The booking can not be created due to a server problem.');
	            }
	        });
	    });
	};
	self.step1 = function () {
		var msg = "";
		var regDate = /^[0-9]{4}-[0-9]{2}-[0-9]{2}$/;
		var regTime = /^[0-9]{2}:[0-9]{2}$/;
		var regNum = /^[0-9]+$/;
		if (!regDate.test(self.booking().pickupDate())) {
			msg += "Invalid date specified.<br/>";
		}
		if (!regTime.test(self.booking().pickupTime())) {
			msg += "Invalid time specified.<br/>";
		}
		if (!regNum.test(self.booking().numberOfPeople())) {
			msg += "Invalid number of people specified.<br/>";
		}
		if (self.booking().addrFrom().length <= 0 || self.booking().addrTo().length <= 0) {
			msg += "Please provide an address from and to.<br/>";
		}
		if (!regNum.test(self.booking().phoneNumber())) {
			msg += 'Invalid phone number.<br/>';
			return;
		}
		if (msg.length > 0) {
			popup('Error', msg);
			return;
		}
		bookHub.announce(self.booking().phoneNumber());
		$('#loading').show();

		service.getDistanceMatrix(
			  {
				  origins: [self.booking().pickup()],
				  destinations: [self.booking().drop()],
				  travelMode: google.maps.TravelMode.DRIVING,
				  avoidHighways: false,
				  avoidTolls: false
			  }, self.distanceResults);
	};
	self.step2 = function () {
	    $('#step2').slideUp();
	    $('#loading').show();
	    if (supports_html5_storage()) {
	        localStorage["from"] = self.booking().addrFrom();
	        localStorage["phone"] = self.booking().phoneNumber();
	    }
	    var suburbFrom = ko.utils.arrayFirst(self.suburbs(), function (suburb) {
	        return suburb.fullAddress === self.booking().suburbFrom();
	    });

	    var data = {
	        "PhoneNumber": self.booking().phoneNumber(),
	        "NumberOfPeople": self.booking().numberOfPeople(),
	        "PickupTime": self.booking().pickupDate() + ' ' + self.booking().pickupTime() + ':00',
	        "AddrFrom": self.booking().pickup(),
	        "AddrTo": self.booking().drop(),
	        "ComputedDistance": self.booking().computedDistance(),
	        "Active": true,
	        "Confirmed": false,
	        "TaxiId": self.booking().taxi().id,
	        "SuburbFromId": suburbFrom != null ? suburbFrom.id : 0
	    };

	    $('#step3').show();
	    $('#loading').hide();

	    $.ajax({
	        type: "POST",
	        contentType: 'application/json',
	        url: '/service/cabmeservice.svc/booking',
	        data: JSON.stringify(data),
	        success: function (msg) {
	            if (msg) {
	                self.booking().id(msg.Id);
	            }
	        },
	        error: function (jqXHR, textStatus, errorThrown) {
	            console.log(errorThrown);
	            popup('Server error', 'The booking can not be created due to a server problem.');
	        }
	    });
	};
	self.accept = function () {
	    $.ajax({
	        type: "POST",
	        contentType: 'application/json',
	        url: '/service/cabmeservice.svc/acceptbooking?id=' + self.booking().id(),
	        data: '',
	        success: function (msg) {
	            self.booking().accepted(true);
	        },
	        error: function (jqXHR, textStatus, errorThrown) {
	            console.log(errorThrown);
	            popup('Server error', 'The booking has not been accepted due to a server problem.');
	        }
	    });
	};
	self.cancel = function () {
	    $.ajax({
	        type: "POST",
	        contentType: 'application/json',
	        url: '/service/cabmeservice.svc/cancelbooking?id=' + self.booking().id(),
	        data: '',
	        success: function (msg) {
	            self.booking().accepted(false);
	            self.booking().confirmed(false);
	            self.booking().switchTaxi(true);
	            $('#step3').hide();
	        },
	        error: function (jqXHR, textStatus, errorThrown) {
	            console.log(errorThrown);
	            popup('Server error', 'The booking has not been cancelled due to a server problem.');
	        }
	    });
	};
	self.changeTaxi = function () {
	    $('#loading').show();

	    self.booking().switchTaxi(false);

	    var suburbFrom = ko.utils.arrayFirst(self.suburbs(), function (suburb) {
	        return suburb.fullAddress === self.booking().suburbFrom();
	    });

	    var data = {
            "Id" : self.booking().id(),
	        "PhoneNumber": self.booking().phoneNumber(),
	        "NumberOfPeople": 1,
	        "AddrFrom": self.booking().pickup(),
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
	                if (msg) {
	                    self.booking().id(msg.Id);
	                }
	            },
	            error: function (jqXHR, textStatus, errorThrown) {
	                console.log(errorThrown);
	                popup('Server error', 'The booking can not be updated due to a server problem.');
	            }
	        });
	    });
	};
	self.distanceResults = function (response, status) {
		if (status == google.maps.DistanceMatrixStatus.OK) {
			var origins = response.originAddresses;
			var destinations = response.destinationAddresses;

			var element = response.rows[0].elements[0];
			if (element.status != 'ZERO_RESULTS') {
				var distance = element.distance.text;
				var duration = element.duration.text;
				var from = origins[0];
				var to = destinations[0];
				self.booking().displayDistance(distance);
				self.booking().computedDistance(element.distance.value);

				$.getJSON('/service/cabmeservice.svc/taxis?distance=' + self.booking().computedDistance(), function (json) {
					self.taxis.removeAll();
					$.each(json, function (index, taxi) {
						self.taxis.push(new Taxi(taxi.Id, taxi.Name, taxi.PriceEstimate));
					});
					$('#step1').slideUp();
					$('#step2').show();
					var request = {
						origin: self.booking().pickup(),
						destination: self.booking().drop(),
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
	};
	self.loadQuickTaxi();
	self.loadSuburbs();
}
function loadMapScript() {
	var script = document.createElement('script');
	script.type = 'text/javascript';
	script.src = 'http://maps.googleapis.com/maps/api/js?key=AIzaSyB2SQU0TkPbS1-MT4D8fkdvZ4J0JkIeBf8&sensor=false&callback=mapsLoaded';
	document.body.appendChild(script);
}
function mapsLoaded() {
	geocoder = new google.maps.Geocoder();
	service = new google.maps.DistanceMatrixService();
	directionsService = new google.maps.DirectionsService();
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