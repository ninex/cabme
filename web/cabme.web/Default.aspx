﻿<%@ Page Title="cabme.co.za - Coming soon" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="cabme.web._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <% if (IsMobile)
       { %>
    <p>
        Disclaimer: This is a beta form and doesn't book a taxi.</p>
    <div id="book">
        <h2>
            Book taxi</h2>
        <div id="loading" class="loading">
            <img src="assets/images/loader.gif" alt="loading" />
        </div>
        <div id="step1">
            <label>
                City
                <select id="city" onchange="cityChanged();" >
                    <option selected value="Cape Town">Cape Town</option>
                    <option value="Johannesburg">Johannesburg</option>
                </select></label>
            <label>
                Pickup date<input id="pickupDate" type="date" /></label>
            <label>
                Pickup time<input id="pickupTime" type="time" /></label>
            <label>
                Number of people<input type="number" id="number" value="1" /></label>
            <label>
                Address from<input type="text" id="from" />
                <select id="fromSuburb" onchange="suburbChanged();">
                </select></label>
            <label>
                Address to<input type="text" id="to" />
                <select id="toSuburb" onchange="suburbChanged();">
                </select></label>
            <input type="button" id="btnConfirm" value="Next" onclick="step1();" />
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
            <label>
                Estimated route</label>
            <div id="map_canvas" style="width: 100%; height: 200px">
            </div>
            <input type="button" id="btnBook" value="Book" onclick="step2();" />
        </div>
        <div id="step3" style="display: none;">
            <p>
                Booking sent to server for confirmation.</p>
        </div>
    </div>
    <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?key=AIzaSyB2SQU0TkPbS1-MT4D8fkdvZ4J0JkIeBf8&sensor=false"></script>
    <script type="text/javascript" src="assets/js/book.js"></script>
    <%}
       else
       {  %>
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
