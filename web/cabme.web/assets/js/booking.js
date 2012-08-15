
$(document).ready(function () {
    setupTabs();
    ko.applyBindings(new BookingViewModel());
});
function Booking(id, phoneNumber, numberOfPeople, pickupTime, suburb, addrFrom, addrTo, confirmed, hash) {
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
}
function BookingViewModel() {
    var self = this;
    self.bookings = ko.observableArray();
    self.loadData = function () {
        if (taxiId) {
            var params = 'name=' + taxiId + '&confirmed=false&open=true';
            if (self.bookings().length > 0) {
                params += '&after=' + self.bookings()[0].id;
            }
            $.getJSON('/service/cabmeservice.svc/taxibookings?' + params, function (json) {
                $.each(json, function (index, booking) {
                    var suburbName = "N/A";
                    if (booking.SuburbFrom && booking.SuburbFrom != null) {
                        suburbName = booking.SuburbFrom.Name;
                    }
                    var newBooking = new Booking(booking.Id, booking.PhoneNumber, booking.NumberOfPeople, booking.PickupTime, suburbName, booking.AddrFrom, booking.AddrTo, booking.Confirmed, booking.Hash);
                    self.bookings.unshift(newBooking);
                });
            });
        } else {
        }
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
                self.bookings.remove(booking);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
    };
    self.independent = function (booking) {
        alert('Send independent logic');
    };
    self.showNewBooking = function (elem) { $(elem).hide().fadeIn('slow'); };
    self.removeBooking = function (elem) { $(elem).fadeOut(); };
    self.loadData();
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