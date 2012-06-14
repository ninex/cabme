var computedDistance = 0;
$(document).ready(function () {
    $('#step2').hide();
    var now = new Date();
    var today = now.getFullYear() + '-' + (now.getMonth() + 1).padLeft(2, '0') + '-' + now.getDate().padLeft(2, '0');
    var time = now.getHours().padLeft(2, '0') + ':' + now.getMinutes().padLeft(2, '0');
    $('#pickupDate').val(today);
    $('#pickupTime').val(time);
    if (supports_html5_storage()) {
        $('#from').val(localStorage["from"]);
        $('#txtPhone').val(localStorage["phone"]);
    }
    if (navigator.geolocation && $('#from').val().length <= 0) {
        navigator.geolocation.getCurrentPosition(success);
    }
    $('#loading').hide();
});

function success(position) {
    var latlng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
    geocoder = new google.maps.Geocoder();
    geocoder.geocode({ 'latLng': latlng }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            if (results[1]) {
                $('#from').val(results[1].formatted_address);
            }
        } else {
            console.log("Geocoder failed due to: " + status);
        }
    });
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
        msg += "Please provide a address from and to.<br/>";
    }
    if (msg.length > 0) {
        popup('Error', msg);
        return;
    }
    $('#loading').show();
    var service = new google.maps.DistanceMatrixService();
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
    var regNum = /^[0-9]+$/;
    if (!regNum.test($('#txtPhone').val())) {
        popup('Error', 'Invalid phone number.');
        return;
    } 

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
        "AddrFrom": $('#from').val(),
        "AddrTo": $('#to').val(),
        "ComputedDistance": computedDistance,
        "Active": true,
        "Confirmed": false,
        "TaxiId": taxiId
    };
    $.ajax({
        type: "POST",
        contentType: 'application/json',
        url: '/service/cabmeservice.svc/booking',
        data: JSON.stringify(data),
        success: function (msg) {
            $('#step3').show();
            $('#loading').hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(errorThrown);
            $('#loading').hide();
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
                var myOptions = {
                    zoom: 14,
                    mapTypeId: google.maps.MapTypeId.ROADMAP
                }
                map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
                geocoder = new google.maps.Geocoder();
                geocoder.geocode({ 'address': from }, function (results, status) {
                    if (status == google.maps.GeocoderStatus.OK) {
                        map.setCenter(results[0].geometry.location);
                    }
                });
                var directionsService = new google.maps.DirectionsService();
                directionsDisplay = new google.maps.DirectionsRenderer();
                directionsDisplay.setMap(map);
                var request = {
                    origin: from,
                    destination: to,
                    travelMode: google.maps.TravelMode.DRIVING
                };
                directionsService.route(request, function (result, status) {
                    if (status == google.maps.DirectionsStatus.OK) {
                        directionsDisplay.setDirections(result);
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
    $('<div id="pop" class="popContainer"><div class="pop"><h1>' + header + '</h1><p>' + msg + '</p><input type="button" value="OK" onclick="removePop();" /></div></div>').insertAfter('#loading');
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