var model;
$(document).ready(function () {
    setupTabs();
    model = new BookingViewModel();
    ko.applyBindings(model);
});
function Booking(id, phoneNumber, numberOfPeople, pickupTime, suburb, addrFrom, addrTo, userAccepted, taxiAccepted, userCancelled, taxiCancelled, hash, taxiId) {
    var self = this;
    self.id = id;
    self.phoneNumber = phoneNumber;
    self.numberOfPeople = numberOfPeople;
    self.pickupTime = pickupTime;
    self.suburb = suburb;
    self.addrFrom = addrFrom;
    self.addrTo = addrTo;
    self.userAccepted = ko.observable(userAccepted);
    self.taxiAccepted = ko.observable(taxiAccepted);
    self.userCancelled = ko.observable(userCancelled);
    self.taxiCancelled = ko.observable(taxiCancelled);
    self.hash = hash;
    self.refCode = ko.observable('');
    self.arrival = ko.observable(1);
    self.isTaxi = taxiId && taxiId != null;
    self.expectedArrival = ko.computed(function () {
        var now = new Date(self.pickupTime);
        now.setMinutes(now.getMinutes() + self.arrival());
        return now.getUTCHours().padLeft(2, '0') + ':' + now.getMinutes().padLeft(2, '0');
    }, self);
}
function BookingViewModel() {
    var self = this;
    var url;
    self.openBookings = ko.observableArray();
    self.acceptedBookings = ko.observableArray();
    self.completedBookings = ko.observableArray();
    self.missedBookings = ko.observableArray();
    self.cancelledBookings = ko.observableArray();
    self.loadData = function () {
        self.openBookings.removeAll();
        self.acceptedBookings.removeAll();
        self.completedBookings.removeAll();
        self.missedBookings.removeAll();
        self.cancelledBookings.removeAll();
        self.loadOpenData();
        self.loadAcceptedData();
        self.loadCompletedData();
        self.loadMissedData();
        self.loadCancelledData();
    };
    self.loadOpenData = function () {
        if (taxiId) {
            var params = 'userName=' + taxiId + '&active=true&open=true&userAccepted=false&taxiCancelled=false&userCancelled=false';
            if (self.openBookings().length > 0) {
                params += '&after=' + self.openBookings()[0].id;
            }
            url = '/api/booking/?' + params;
        } else {
            url = '/api/booking/?userName=' + userID + '&active=true&open=true&userAccepted=false&taxiCancelled=false&userCancelled=false';
        }
        $.getJSON(url, function (json) {
            $.each(json, function (index, booking) {
                var suburbName = "N/A";
                if (booking.SuburbFrom && booking.SuburbFrom != null) {
                    suburbName = booking.SuburbFrom.Name;
                }
                var newBooking = new Booking(booking.Id, booking.PhoneNumber, booking.NumberOfPeople, booking.PickupTime, suburbName, booking.AddrFrom, booking.AddrTo, booking.UserAccepted, booking.TaxiAccepted, booking.UserCancelled, booking.TaxiCancelled, booking.Hash, taxiId);
                self.openBookings.unshift(newBooking);
                if (newBooking.taxiAccepted()) {
                    setTimeout(function () {
                        self.openBookings.remove(newBooking);
                    }, 300000);
                }
            });
        });
    };
    self.loadAcceptedData = function () {
        if (taxiId) {
            url = '/api/booking/?userName=' + taxiId + '&active=true&open=true&taxiAccepted=true&userAccepted=true&taxiCancelled=false&userCancelled=false';
        } else {
            url = '/api/booking/?userName=' + userID + '&active=true&open=true&taxiAccepted=true&userAccepted=true&taxiCancelled=false&userCancelled=false';
        }
        $.getJSON(url, function (json) {
            $.each(json, function (index, booking) {
                var suburbName = "N/A";
                if (booking.SuburbFrom && booking.SuburbFrom != null) {
                    suburbName = booking.SuburbFrom.Name;
                }
                self.acceptedBookings.unshift(new Booking(booking.Id, booking.PhoneNumber, booking.NumberOfPeople, booking.PickupTime, suburbName, booking.AddrFrom, booking.AddrTo, booking.Confirmed, booking.Accepted, booking.Hash, taxiId));
            });
        });
    };
    self.loadCompletedData = function () {
        if (taxiId) {
            url = '/api/booking/?userName=' + taxiId + '&active=true&open=false&taxiAccepted=true&userAccepted=true&taxiCancelled=false&userCancelled=false';
        } else {
            url = '/api/booking/?userName=' + userID + '&active=true&open=false&taxiAccepted=true&userAccepted=true&taxiCancelled=false&userCancelled=false';
        }
        $.getJSON(url, function (json) {
            $.each(json, function (index, booking) {
                var suburbName = "N/A";
                if (booking.SuburbFrom && booking.SuburbFrom != null) {
                    suburbName = booking.SuburbFrom.Name;
                }
                self.completedBookings.unshift(new Booking(booking.Id, booking.PhoneNumber, booking.NumberOfPeople, booking.PickupTime, suburbName, booking.AddrFrom, booking.AddrTo, booking.Confirmed, booking.Accepted, booking.Hash, taxiId));
            });
        });
    };
    self.loadMissedData = function () {
        if (taxiId) {
            url = '/api/booking/?userName=' + taxiId + '&active=true&open=false&taxiAccepted=false&taxiCancelled=false&userCancelled=false';
        } else {
            url = '/api/booking/?userName=' + userID + '&active=true&open=false&taxiAccepted=false&taxiCancelled=false&userCancelled=false';
        }
        $.getJSON(url, function (json) {
            $.each(json, function (index, booking) {
                var suburbName = "N/A";
                if (booking.SuburbFrom && booking.SuburbFrom != null) {
                    suburbName = booking.SuburbFrom.Name;
                }
                self.missedBookings.unshift(new Booking(booking.Id, booking.PhoneNumber, booking.NumberOfPeople, booking.PickupTime, suburbName, booking.AddrFrom, booking.AddrTo, booking.Confirmed, booking.Accepted, booking.Hash, taxiId));
            });
        });
    };
    self.loadCancelledData = function () {
        if (taxiId) {
            url = '/api/booking/?userName=' + taxiId + '&taxiCancelled=true&userCancelled=false';
        } else {
            url = '/api/booking/?userName=' + userID + '&taxiCancelled=true&userCancelled=false';
        }
        $.getJSON(url, function (json) {
            $.each(json, function (index, booking) {
                var suburbName = "N/A";
                if (booking.SuburbFrom && booking.SuburbFrom != null) {
                    suburbName = booking.SuburbFrom.Name;
                }
                self.cancelledBookings.unshift(new Booking(booking.Id, booking.PhoneNumber, booking.NumberOfPeople, booking.PickupTime, suburbName, booking.AddrFrom, booking.AddrTo, booking.Confirmed, booking.Accepted, booking.Hash, taxiId));
            });
        });

        if (taxiId) {
            url = '/api/booking/?userName=' + taxiId + '&taxiCancelled=false&userCancelled=true';
        } else {
            url = '/api/booking/?userName=' + userID + '&taxiCancelled=false&userCancelled=true';
        }
        $.getJSON(url, function (json) {
            $.each(json, function (index, booking) {
                var suburbName = "N/A";
                if (booking.SuburbFrom && booking.SuburbFrom != null) {
                    suburbName = booking.SuburbFrom.Name;
                }
                self.cancelledBookings.unshift(new Booking(booking.Id, booking.PhoneNumber, booking.NumberOfPeople, booking.PickupTime, suburbName, booking.AddrFrom, booking.AddrTo, booking.Confirmed, booking.Accepted, booking.Hash, taxiId));
            });
        });
    };
    self.confirm = function (booking) {
        var params = 'waitingTime=' + booking.arrival();
        if (booking.refCode().length > 0) {
            params += '&referenceCode=' + booking.refCode();
        }
        $.ajax({
            type: "GET",
            contentType: 'application/json',
            url: '/api/booking/' + booking.hash + '/?' + params,
            data: '',
            success: function (msg) {
                booking.taxiAccepted(true);
                self.completedBookings.unshift(booking);
                setTimeout(function () {
                    self.openBookings.remove(booking);
                }, 300000);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
    };
    self.independent = function (booking) {
        alert('Send independent logic');
    };
    self.acceptedBooking = function (id, userAccepted) {
        var booking = ko.utils.arrayFirst(self.openBookings(), function (booking) {
            return booking.id === id;
        });
        if (userAccepted) {
            booking.userAccepted(true);
        } else {
            booking.taxiAccepted(true);
        }
    };
    self.reject = function (booking) {
        var data = {
            "TaxiCancelled": true,
            "Id": booking.id,
            "PhoneNumber": booking.phoneNumber,
            "NumberOfPeople": booking.numberOfPeople,
            "AddrFrom": booking.addrFrom,
            "TaxiId": 1
        };
        $.ajax({
            type: "PUT",
            contentType: 'application/json',
            url: '/api/booking/' + booking.id,
            data: JSON.stringify(data),
            success: function (msg) {
                booking.taxiCancelled(true);
                self.openBookings.remove(booking);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(errorThrown);
                popup('Server error', 'The booking has not been cancelled due to a server problem.');
            }
        });
    };
    self.cancelBooking = function (id, userCancelled) {
        var booking = ko.utils.arrayFirst(self.openBookings(), function (booking) {
            return booking.id === id;
        });
        if (userCancelled) {
            booking.userCancelled(true);
        } else {
            booking.taxiCancelled(true);
        }
        self.openBookings.remove(booking);
    }
    self.review = function (booking) {
        alert('Review logic');
    };
    self.showNewBooking = function (elem) { $(elem).hide().fadeIn('slow'); };
    self.removeBooking = function (elem) { $(elem).fadeOut(); };
    self.loadData();
}
function acceptedBooking(id, userAccepted) {
    model.acceptedBooking(id, userAccepted);
}
function cancelBooking(id, userCancelled) {
    model.cancelBooking(id, userCancelled);
}
function btnRefresh() {
    $('#pendingBookings').slideUp();
    //so that the image button doesn't postback
    return false;
}
ko.bindingHandlers.fadeVisible = {
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        if (value) {
            $(element).shake(3, 5, 500);
        }
    }
};
jQuery.fn.shake = function (intShakes, intDistance, intDuration) {
    this.each(function () {
                                $(this).css("position", "relative");
                                for (var x = 1; x <= intShakes; x++) {
                                    $(this).animate({ left: (intDistance * -1) }, (((intDuration / intShakes) / 4)))
                                        .animate({ left: intDistance }, ((intDuration / intShakes) / 2))
                                        .animate({ left: 0 }, (((intDuration / intShakes) / 4)));
                                }
    });
    return this;
};
function setupTabs() {
    $('#tab1').show();
    $('#tab2').hide();
    $('#tab3').hide();
    $('#tab4').hide();
    $('#tab5').hide();
    $('#htab1').addClass('current');

    $('#htab1').click(function () {
        $('h3').removeClass('current');
        $('#tab2').hide();
        $('#tab3').hide();
        $('#tab4').hide();
        $('#tab5').hide();
        $('#tab1').fadeIn();
        $('#htab1').addClass('current');
    });
    $('#htab2').click(function () {
        $('h3').removeClass('current');
        $('#tab1').hide();
        $('#tab3').hide();
        $('#tab4').hide();
        $('#tab5').hide();
        $('#tab2').fadeIn();
        $('#htab2').addClass('current');
    });
    $('#htab3').click(function () {
        $('h3').removeClass('current');
        $('#tab1').hide();
        $('#tab2').hide();
        $('#tab4').hide();
        $('#tab5').hide();
        $('#tab3').fadeIn();
        $('#htab3').addClass('current');
    });
    $('#htab4').click(function () {
        $('h3').removeClass('current');
        $('#tab1').hide();
        $('#tab2').hide();
        $('#tab3').hide();
        $('#tab5').hide();
        $('#tab4').fadeIn();
        $('#htab4').addClass('current');
    });
    $('#htab5').click(function () {
        $('h3').removeClass('current');
        $('#tab1').hide();
        $('#tab2').hide();
        $('#tab3').hide();
        $('#tab4').hide();
        $('#tab5').fadeIn();
        $('#htab5').addClass('current');
    });
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