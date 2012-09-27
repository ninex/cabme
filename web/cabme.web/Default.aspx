<%@ Page Title="cabme.co.za - Coming soon" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="cabme.web._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <% if (IsMobile)
       { %>
    <div id="book">
        <h2>
            Book taxi</h2>
        <div id="loading" class="loading">
            <img src="assets/images/loader.gif" alt="loading" />
        </div>
        <div id="step1" style="display: none">
            <p id="btnMakeFull" class="clickMe" data-bind="click: function(data, event) { booking().full(!booking().full()); }">
                Switch to detailed booking.</p>
            <div style="width: 100%;" class="table">
                <div class="row">
                    <div class="cell">
                        <img src="assets/images/city.png" alt="City" />
                    </div>
                    <div class="lastcell">
                        <select id="city" data-bind="value: booking().city">
                            <option selected value="Cape Town">Cape Town</option>
                            <option value="Johannesburg">Johannesburg</option>
                        </select>
                    </div>
                </div>
                <div class="row">
                    <div class="cell">
                        <img src="assets/images/phone.png" alt="Phone Number" />
                    </div>
                    <div class="lastcell">
                        <input type="tel" id="txtPhone" placeholder="Phone number" data-bind="value: booking().phoneNumber" />
                    </div>
                </div>
                <div class="row" full style="display: none">
                    <div class="cell">
                        <img src="assets/images/calendar.png" alt="Pickup date" />
                    </div>
                    <div class="lastcell">
                        <input id="pickupDate" type="date" placeholder="Pickup date" data-bind="value: booking().pickupDate" />
                    </div>
                </div>
                <div class="row" full style="display: none">
                    <div class="cell">
                    </div>
                    <div class="lastcell">
                        <input id="pickupTime" type="time" placeholder="Pickup time" data-bind="value: booking().pickupTime" />
                    </div>
                </div>
                <div class="row" full style="display: none">
                    <div class="cell">
                        <img src="assets/images/people.png" alt="Number of people" />
                    </div>
                    <div class="lastcell">
                        <input type="number" id="number" placeholder="Number of people" data-bind="value: booking().numberOfPeople" />
                    </div>
                </div>
                <div class="row">
                    <div class="cell">
                        <img src="assets/images/pickup.png" alt="Pickup From" />
                    </div>
                    <div class="lastcell">
                        <input type="text" id="from" placeholder="Pickup from" data-bind="value: booking().addrFrom" />
                    </div>
                </div>
                <div class="row">
                    <div class="cell">
                    </div>
                    <div class="lastcell">
                        <select id="fromSuburb" data-bind="value: booking().suburbFrom, options: suburbs(), optionsText: 'name', optionsValue: 'fullAddress'">
                        </select>
                    </div>
                </div>
                <div class="row" full style="display: none">
                    <div class="cell">
                        <img src="assets/images/drop.png" alt="Drop Off" />
                    </div>
                    <div class="lastcell">
                        <input type="text" id="to" placeholder="Drop off" data-bind="value: booking().addrTo" />
                    </div>
                </div>
                <div class="row" full style="display: none">
                    <div class="cell">
                    </div>
                    <div class="lastcell">
                        <select id="toSuburb" data-bind="value: booking().suburbTo, options: suburbs(), optionsText: 'name', optionsValue: 'fullAddress'">
                        </select>
                    </div>
                </div>
                <div class="row" id="lblQuickTaxi">
                    <div class="cell">
                        <img src="assets/images/logo.png" alt="Taxi" />
                    </div>
                    <div class="lastcell">
                        <select id="ddlQuickTaxi" data-bind="options: taxis, optionsText: 'name',value: booking().quickTaxi">
                        </select>
                    </div>
                </div>
            </div>
            <div style="margin: 10px auto; width: 100%; text-align: center;">
                <input type="button" id="btnBookMin" value="Book" data-bind="click: step1Min" class="button" />
                <input full style="display: none" type="button" id="btnConfirm" value="Next" data-bind="click: step1"
                    class="button" />
            </div>
        </div>
        <div id="step2" style="display: none;">
            <div style="width: 100%; padding: 0px;" class="table">
                <div style="display: table; width: 100%; padding: 15px 15px 0px 15px;">
                    <div class="row">
                        <div class="cell">
                        </div>
                        <div class="lastCell">
                            <div id="txtDistance" data-bind="text: booking().displayDistance">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cell">
                            <img src="assets/images/logo.png" alt="Taxi" /></div>
                        <div class="lastCell">
                            <select id="ddlTaxi" data-bind="options: taxis, optionsText: 'display',value: booking().taxi">
                            </select>
                        </div>
                    </div>
                </div>
                <p style="padding: 0px 15px;">
                    Estimated route
                </p>
                <div id="map" style="overflow: hidden; padding: 0px 15px 15px 15px;">
                </div>
            </div>
            <div style="margin: 10px auto; width: 100%; text-align: center;">
                <input type="button" id="btnBook" value="Book" class="button" data-bind="click: step2" />
            </div>
        </div>
        <div id="step3" style="display: none; width: 100%;" class="table">
            <h3>
                Booking status</h3>
            <div id="msgStatus" class="status" data-bind="visible: !booking().accepted() && !booking().switchTaxi()">
                <p class="status">
                    Booking sent to server.</p>
            </div>
            <div data-bind="visible: !booking().confirmed() && !booking().switchTaxi()">
                <input type="button" id="btnCancel" value="Cancel" class="button" data-bind="click: cancel" />
            </div>
            <div data-bind="visible: booking().confirmed() && !booking().accepted() && !booking().switchTaxi()">
                Booking confirmed. Taxi can arrive in
                <label data-bind="text: booking().waitingTime">
                </label>
                minutes at
                <label data-bind="text: booking().expectedArrival">
                </label>
                <div>
                    <input type="button" id="btnAccept" value="Accept" class="button" data-bind="click: accept" />
                    <input type="button" id="btnDismiss" value="Cancel" class="button" data-bind="click: cancel" />
                </div>
            </div>
            <div data-bind="visible: booking().accepted() && !booking().switchTaxi()">
                Booking accepted. Taxi expected at
                <label data-bind="text: booking().expectedArrival">
                </label>
            </div>
            <div data-bind="visible: booking().switchTaxi()">
                Here you pick a new taxi.
                <input type="button" id="btnNewTaxi" value="Change Taxi" class="button" data-bind="click: changeTaxi" />
            </div>
        </div>
    </div>
    <p>
        Disclaimer: This is a beta form and doesn't book a taxi.</p>
    <%}
       else
       { %>
    <h2>
        Coming soon
    </h2>
    <p>
        We are currently in dev stages on the android app.
    </p>
    <p>
        Mobile web development has also started.
    </p>
    <%} %>
</asp:Content>
<asp:Content ContentPlaceHolderID="Scripts" runat="server" ID="Scripts">
    <script type="text/javascript" src="http://cdnjs.cloudflare.com/ajax/libs/knockout/2.1.0/knockout-min.js"></script>
    <script type="text/javascript" src="assets/js/book.js"></script>
</asp:Content>
