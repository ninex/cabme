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
    //TODO: Check if everything valid
    var origin1 = $('#from').val();
    var destination1 = $('#to').val();

    var service = new google.maps.DistanceMatrixService();
    service.getDistanceMatrix(
			  {
			      origins: [origin1],
			      destinations: [destination1],
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
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(errorThrown);
        }
    });
}
function distanceResults(response, status) {
    if (status == google.maps.DistanceMatrixStatus.OK) {
        var origins = response.originAddresses;
        var destinations = response.destinationAddresses;

        for (var i = 0; i < origins.length; i++) {
            var results = response.rows[i].elements;
            for (var j = 0; j < results.length; j++) {
                var element = results[j];
                if (element.status != 'ZERO_RESULTS') {
                    var distance = element.distance.text;
                    var duration = element.duration.text;
                    var from = origins[i];
                    var to = destinations[j];
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
                    });
                }
            }
        }
    }
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