﻿<%@ Page Title="cabme.co.za - Coming soon" Language="C#" MasterPageFile="~/Site.Master"
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
        <div id="step1">
            <p id="btnMakeFull" onclick="makeFull();" class="clickMe">
                Press here to switch to detailed booking.</p>
            <div style="display: table; width: 100%;">
                <div class="row">
                    <div class="cell">
                        <img src="assets/images/city.png" alt="City" />
                    </div>
                    <div class="lastcell">
                        <select id="city" onchange="cityChanged();">
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
                        <input type="tel" id="txtPhone" /></div>
                </div>
                <div class="row" full style="display: none">
                    <div class="cell">
                        <img src="assets/images/calendar.png" alt="Pickup date" />
                    </div>
                    <div class="lastcell">
                        <input id="pickupDate" type="date" /></div>
                </div>
                <div class="row" full style="display: none">
                    <div class="cell">
                    </div>
                    <div class="lastcell">
                        <input id="pickupTime" type="time" /></div>
                </div>
                <div class="row" full style="display: none">
                    <div class="cell">
                        <img src="assets/images/people.png" alt="Number of people" />
                    </div>
                    <div class="lastcell">
                        <input type="number" id="number" value="1" /></div>
                </div>
                <div class="row">
                    <div class="cell">
                        <img src="assets/images/pickup.png" alt="Pickup From" />
                    </div>
                    <div class="lastcell">
                        <input type="text" id="from" />
                    </div>
                </div>
                <div class="row">
                    <div class="cell">
                    </div>
                    <div class="lastcell">
                        <select id="fromSuburb" onchange="suburbChanged();">
                        </select>
                    </div>
                </div>
                <div class="row" full style="display: none">
                    <div class="cell">
                        <img src="assets/images/drop.png" alt="Drop Off" />
                    </div>
                    <div class="lastcell">
                        <input type="text" id="to" />
                    </div>
                </div>
                <div class="row" full style="display: none">
                    <div class="cell">
                    </div>
                    <div class="lastcell">
                        <select id="toSuburb" onchange="suburbChanged();">
                        </select>
                    </div>
                </div>
                <div class="row" id="lblQuickTaxi">
                    <div class="cell">
                        <img src="assets/images/logo.png" alt="Taxi" />
                    </div>
                    <div class="lastcell">
                        <select id="ddlQuickTaxi">
                        </select>
                    </div>
                </div>
            </div>
            <input type="button" id="btnBookMin" value="Book" onclick="step1Min();" />
            <input full style="display: none" type="button" id="btnConfirm" value="Next" onclick="step1();" />
        </div>
        <div id="step2" style="display: none">
            <div style="display: table; width: 100%">
                <div class="row">
                    <div class="cell">
                    </div>
                    <div class="lastCell">
                        <div id="txtDistance">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="cell">
                        <img src="assets/images/logo.png" alt="Taxi" /></div>
                    <div class="lastCell">
                        <select id="ddlTaxi">
                        </select>
                    </div>
                </div>
            </div>
            <p>
                Estimated route
            </p>
            <div id="map" style="overflow: hidden;">
            </div>
            <input type="button" id="btnBook" value="Book" onclick="step2();" />
        </div>
        <div id="step3" style="display: none; border: 1px solid #000; margin: 5px; padding: 5px;">
            <h3>
                Booking status</h3>
            <div id="msgStatus" class="status">
                <p>
                    Booking sent to server.</p>
            </div>
        </div>
    </div>
    <p> Disclaimer: This is a beta form and doesn't book a taxi.</p> <%}
       else
       { %> <h2>
    Coming soon </h2> <p> We are currently in dev stages on the android app. </p> <p>
    Mobile web development has also started. </p> <%} %>
</asp:Content>
<asp:Content ContentPlaceHolderID="Scripts" runat="server" ID="Scripts">
    <script type="text/javascript" src="assets/js/book.js"></script>
</asp:Content>
