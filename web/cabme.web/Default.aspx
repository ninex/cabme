<%@ Page Title="cabme.co.za - Coming soon" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="cabme.web._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Coming soon
    </h2>
    <p>
        We are currently in dev stages on the android app.
    </p>
    <p>
        Mobile web development has also started.
    </p>
    <% if (IsMobile)
       { %>
    <p>
        Disclaimer: This is a dummy form and doesn't book a taxi.</p>
    <div id="book">
        <h2>
            Book taxi</h2>
        <div id="step1">
            <label>
                Pickup date<input id="pickupDate" type="date" /></label>
            <label>
                Pickup time<input id="pickupTime" type="time" /></label>
            <label>
                Number of people<input type="number" id="number" value="1" /></label>
            <label>
                Address from<input type="text" id="from" /></label>
            <label>
                Address to<input type="text" id="to" /></label>
            <input type="button" id="btnConfirm" value="Confirm" onclick="step1();" />
        </div>
        <div id="step2" style="display: none">
            <label>
                Phone number<input type="tel" id="txtPhone" /></label>
            <label>
                Distance<p id="txtDistance">
                </p>
            </label>
            <label>
                Taxi<select id="ddlTaxi"></select></label>
            <input type="button" id="btnBook" value="Book" onclick="step2();" />
        </div>
        <div id="step3" style="display:none;">
            <p>Booking sent to server for processing.</p>
        </div>
    </div>
    <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?key=AIzaSyB2SQU0TkPbS1-MT4D8fkdvZ4J0JkIeBf8&sensor=false">
    </script>
    <script type="text/javascript">
        var computedDistance = 0;
        $(document).ready(function () {
            $('#step2').hide();
            var now = new Date();
            var today = now.getFullYear() + '-' + (now.getMonth() + 1).padLeft(2, '0') + '-' + now.getDate().padLeft(2, '0');
            var time = now.getHours().padLeft(2, '0') + ':' + now.getMinutes().padLeft(2, '0');
            $('#pickupDate').val(today);
            $('#pickupTime').val(time);
            if (navigator.geolocation) {
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
            //Check if everything valid
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
			  }, callback);
        }

        function step2() {
            $('#step2').slideUp();
            var taxiId = $('#ddlTaxi').attr('selected', true).val();
            var data = {
                "PhoneNumber": $('#txtPhone').val(),
                "NumberOfPeople": $('#number').val(),
                "PickupTime": $('#pickupDate').val() + ' '+ $('#pickupTime').val() + ':00',
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
        function callback(response, status) {
            if (status == google.maps.DistanceMatrixStatus.OK) {
                var origins = response.originAddresses;
                var destinations = response.destinationAddresses;

                for (var i = 0; i < origins.length; i++) {
                    var results = response.rows[i].elements;
                    for (var j = 0; j < results.length; j++) {
                        var element = results[j];
                        var distance = element.distance.text;
                        var duration = element.duration.text;
                        var from = origins[i];
                        var to = destinations[j];
                        computedDistance = element.distance.value;
                        $('#txtDistance').html(distance);

                        $.getJSON('/service/cabmeservice.svc/taxis', function (json) {
                            var options = '';
                            $.each(json, function (index, taxi) {
                                var price = getPriceEstimate(taxi, computedDistance);
                                options += '<option value="' + taxi.Id + '">' + taxi.Name + ' - ' + price + '</option>';
                            });
                            $('#ddlTaxi').html(options);
                            $('#step1').slideUp();
                            $('#step2').show();
                        });
                    }
                }
            }
        }

        function getPriceEstimate(taxi, distance) {
            if (taxi != null && distance) {
                var computedPrice = (taxi.RatePerKm * distance) / 1000;
                if (computedPrice < taxi.MinRate) {
                    computedPrice = taxi.MinRate;
                } else {
                    if (computedPrice % taxi.Units > 0) {
                        computedPrice = computedPrice
								- (computedPrice % taxi.Units)
								+ taxi.Units;
                    }
                }
                //DecimalFormat dec = new DecimalFormat("###.##");
                return "R" + (computedPrice / 100); //+ dec.format((float) computedPrice / 100);
            }
            EstimatedPrice = 0;
            return "No estimate";
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
    </script>
    <%} %>
</asp:Content>
