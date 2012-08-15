
$(document).ready(function () {
    setupTabs();
    ko.applyBindings(new BookingViewModel());
});
function Booking(id, phoneNumber, numberOfPeople, pickupTime, suburb, addrFrom, addrTo, confirmed, hash, taxiId) {
    var self = this;
    self.id = id;
    self.phoneNumber = phoneNumber;
    self.numberOfPeople = numberOfPeople;
    self.pickupTime = pickupTime;
    self.suburb = suburb;
    self.addrFrom = addrFrom;
    self.addrTo = addrTo;
    self.confirmed = confirmed;
    self.hash = hash;
    self.refCode = ko.observable('');
    self.arrival = ko.observable('');
    self.isTaxi = taxiId && taxiId != null;
}
function BookingViewModel() {
    var self = this;
    var url;
    self.openBookings = ko.observableArray();
    self.completedBookings = ko.observableArray();
    self.missedBookings = ko.observableArray();
    self.loadData = function () {
        if (taxiId) {
            var params = 'name=' + taxiId + '&confirmed=false&open=true';
            if (self.openBookings().length > 0) {
                params += '&after=' + self.openBookings()[0].id;
            }
            url = '/service/cabmeservice.svc/taxibookings?' + params;
        } else {
            url = '/service/cabmeservice.svc/userbookings?user=' + userID + '&confirmed=false&open=true';
        }
        $.getJSON(url, function (json) {
            $.each(json, function (index, booking) {
                var suburbName = "N/A";
                if (booking.SuburbFrom && booking.SuburbFrom != null) {
                    suburbName = booking.SuburbFrom.Name;
                }
                var newBooking = new Booking(booking.Id, booking.PhoneNumber, booking.NumberOfPeople, booking.PickupTime, suburbName, booking.AddrFrom, booking.AddrTo, booking.Confirmed, booking.Hash, taxiId);
                self.openBookings.unshift(newBooking);
            });
        });
    };
    self.loadCompletedData = function () {
        if (taxiId) {
            url = '/service/cabmeservice.svc/taxibookings?name=' + taxiId + '&confirmed=true';
        } else {
            url = '/service/cabmeservice.svc/userbookings?user=' + userID + '&confirmed=true';
        }
        $.getJSON(url, function (json) {
            $.each(json, function (index, booking) {
                var suburbName = "N/A";
                if (booking.SuburbFrom && booking.SuburbFrom != null) {
                    suburbName = booking.SuburbFrom.Name;
                }
                self.completedBookings.unshift(new Booking(booking.Id, booking.PhoneNumber, booking.NumberOfPeople, booking.PickupTime, suburbName, booking.AddrFrom, booking.AddrTo, booking.Confirmed, booking.Hash, taxiId));
            });
        });
    };
    self.loadMissedData = function () {
        if (taxiId) {
            url = '/service/cabmeservice.svc/taxibookings?name=' + taxiId + '&confirmed=false&open=false';
        } else {
            url = '/service/cabmeservice.svc/userbookings?user=' + userID + '&confirmed=false&open=false';
        }
        $.getJSON(url, function (json) {
            $.each(json, function (index, booking) {
                var suburbName = "N/A";
                if (booking.SuburbFrom && booking.SuburbFrom != null) {
                    suburbName = booking.SuburbFrom.Name;
                }
                self.missedBookings.unshift(new Booking(booking.Id, booking.PhoneNumber, booking.NumberOfPeople, booking.PickupTime, suburbName, booking.AddrFrom, booking.AddrTo, booking.Confirmed, booking.Hash, taxiId));
            });
        });
    };
    self.confirm = function (booking) {
        var data = {
            "Arrival": booking.arrival(),
            "RefCode": booking.refCode(),
            "PhoneNumber": booking.phoneNumber,
            "Hash": booking.hash
        };
        $.ajax({
            type: "POST",
            contentType: 'application/json',
            url: '/service/cabmeservice.svc/confirmbooking',
            data: JSON.stringify(data),
            success: function (msg) {
                self.openBookings.remove(booking);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
    };
    self.independent = function (booking) {
        alert('Send independent logic');
    };
    self.review = function (booking) {
        alert('Review logic');
    };
    self.showNewBooking = function (elem) { $(elem).hide().fadeIn('slow'); };
    self.removeBooking = function (elem) { $(elem).fadeOut(); };
    self.loadData();
    self.loadCompletedData();
    self.loadMissedData();
}
function btnRefresh() {
    $('#pendingBookings').slideUp();
    //so that the image button doesn't postback
    return false;
}

function setupTabs() {
    $('#tab1').show();
    $('#tab2').hide();
    $('#tab3').hide();
    $('#htab1').addClass('current');

    $('#htab1').click(function () {
        $('h3').removeClass('current');
        $('#tab2').hide();
        $('#tab3').hide();
        $('#tab1').fadeIn();
        $('#htab1').addClass('current');
    });
    $('#htab2').click(function () {
        $('h3').removeClass('current');
        $('#tab1').hide();
        $('#tab3').hide();
        $('#tab2').fadeIn();
        $('#htab2').addClass('current');
    });
    $('#htab3').click(function () {
        $('h3').removeClass('current');
        $('#tab1').hide();
        $('#tab2').hide();
        $('#tab3').fadeIn();
        $('#htab3').addClass('current');
    });
}