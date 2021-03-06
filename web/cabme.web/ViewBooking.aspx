﻿<%@ Page Title="View Bookings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ViewBooking.aspx.cs" Inherits="cabme.web.ViewBookingPage" ClientIDMode="Static" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Bookings</h2>
    <div style="float: right">
        <input type="image" class="refresh" src="assets/images/refresh.png" alt="Refresh"
            data-bind="click: loadData" onclick="btnRefresh();" />
    </div>
    <article class="tabs">
        <h3 id="htab1">
            Open</h3>
        <h3 id="htab2">
            Confirmed</h3>
        <h3 id="htab3">
            Missed</h3>
        <div id="tab1" style="display: none">
            <div style="height: 100%">
                <div id="pendingBookings" style="display: none">
                    <p>
                        You have new bookings awaiting confirmation. Refresh to view.</p>
                </div>
                <div data-bind="foreach: {data: openBookings, afterAdd: showNewBooking, beforeRemove: removeBooking}">
                    <div class="table" style="width: 80%;" data-bind="fadeVisible: confirmed, fadeVisible: accepted">
                        <div class="row">
                            <div class="cell">
                                <b>Phone Number: </b>
                            </div>
                            <div class="cell lastCell">
                                <div data-bind="text: phoneNumber, visible: confirmed() == true || isTaxi == false || typeof isTaxi == 'undefined'">
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>People: </b>
                            </div>
                            <div class="cell lastCell">
                                <div data-bind="text: numberOfPeople">
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>Pickup Time: </b>
                            </div>
                            <div class="cell lastCell">
                                <div data-bind="text: pickupTime">
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>Suburb: </b>
                            </div>
                            <div class="cell lastCell">
                                <div data-bind="text: suburb">
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>From: </b>
                            </div>
                            <div class="cell lastCell">
                                <div data-bind="text: addrFrom, visible: confirmed() == true || isTaxi == false || typeof isTaxi == 'undefined'">
                                </div>
                            </div>
                        </div>
                        <div class="row" data-bind="visible: typeof addrTo != 'undefined' && addrTo != '' && addrTo != null">
                            <div class="cell">
                                <b>To: </b>
                            </div>
                            <div class="cell lastCell">
                                <div data-bind="text: addrTo, visible: confirmed() == true || isTaxi == false || typeof isTaxi == 'undefined'">
                                </div>
                            </div>
                        </div>
                        <div class="row" data-bind="visible: confirmed() == false && isTaxi == true">
                            <div class="cell">
                                <b>Driver Code: </b>
                            </div>
                            <div class="cell lastCell">
                                <input type="text" maxlength="8" style="width: 40px;" data-bind="value: refCode" />
                            </div>
                        </div>
                        <div class="row" data-bind="visible: confirmed() == false && isTaxi == true">
                            <div class="cell">
                                <b>Minutes to arrival: </b>
                            </div>
                            <div class="cell lastCell">
                                <input type="text" maxlength="2" style="width: 40px;" data-bind="value: arrival" />
                            </div>
                        </div>
                        <div class="row" data-bind="visible: !accepted() && confirmed() && isTaxi">
                            <div class="cell">
                            </div>
                            <div class="cell lastCell">
                                <div class="status">
                                    Waiting for client to accept</div>
                            </div>
                        </div>
                        <div class="row" data-bind="visible: accepted() == false && isTaxi == true">
                            <div class="cell">
                            </div>
                            <div class="cell lastCell">
                                <input type="button" class="button" value="Confirm" data-bind="click: $parent.confirm, visible: !confirmed()" />
                                <input type="button" class="button" value="Reject" data-bind="click: $parent.reject" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div data-bind="visible: openBookings().length <= 0">
                <p>
                    No active bookings</p>
            </div>
        </div>
        <div id="tab2" style="display: none">
            <div data-bind="foreach: {data: completedBookings}">
                <div class="table" style="width: 80%;">
                    <div class="row">
                        <div class="cell">
                            <b>Phone Number: </b>
                        </div>
                        <div class="cell lastCell">
                            <div data-bind="text: phoneNumber">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cell">
                            <b>People: </b>
                        </div>
                        <div class="cell lastCell">
                            <div data-bind="text: numberOfPeople">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cell">
                            <b>Pickup Time: </b>
                        </div>
                        <div class="cell lastCell">
                            <div data-bind="text: pickupTime">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cell">
                            <b>Suburb: </b>
                        </div>
                        <div class="cell lastCell">
                            <div data-bind="text: suburb">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cell">
                            <b>From: </b>
                        </div>
                        <div class="cell lastCell">
                            <div data-bind="text: addrFrom">
                            </div>
                        </div>
                    </div>
                    <div class="row" data-bind="visible: typeof addrTo != 'undefined' && addrTo != '' && addrTo != null">
                        <div class="cell">
                            <b>To: </b>
                        </div>
                        <div class="cell lastCell">
                            <div data-bind="text: addrTo">
                            </div>
                        </div>
                    </div>
                    <div class="row" data-bind="visible: isTaxi == false">
                        <div class="cell">
                        </div>
                        <div class="cell lastCell">
                            <input type="button" class="button" value="Review" data-bind="click: $parent.review" />
                        </div>
                    </div>
                </div>
            </div>
            <div data-bind="visible: completedBookings().length <= 0">
                <p>
                    No bookings</p>
            </div>
        </div>
        <div id="tab3" style="display: none">
            <div data-bind="foreach: {data: missedBookings}">
                <div class="table" style="width: 80%;">
                    <div class="row">
                        <div class="cell">
                            <b>Phone Number: </b>
                        </div>
                        <div class="cell lastCell">
                            <div data-bind="text: phoneNumber">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cell">
                            <b>People: </b>
                        </div>
                        <div class="cell lastCell">
                            <div data-bind="text: numberOfPeople">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cell">
                            <b>Pickup Time: </b>
                        </div>
                        <div class="cell lastCell">
                            <div data-bind="text: pickupTime">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cell">
                            <b>Suburb: </b>
                        </div>
                        <div class="cell lastCell">
                            <div data-bind="text: suburb">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cell">
                            <b>From: </b>
                        </div>
                        <div class="cell lastCell">
                            <div data-bind="text: addrFrom">
                            </div>
                        </div>
                    </div>
                    <div class="row" data-bind="visible: typeof addrTo != 'undefined' && addrTo != '' && addrTo != null">
                        <div class="cell">
                            <b>To: </b>
                        </div>
                        <div class="cell lastCell">
                            <div data-bind="text: addrTo">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div data-bind="visible: missedBookings().length <= 0">
                <p>
                    No missed bookings</p>
            </div>
        </div>
    </article>
</asp:Content>
<asp:Content ContentPlaceHolderID="Scripts" runat="server" ID="Scripts">
    <script type="text/javascript" src="http://cdnjs.cloudflare.com/ajax/libs/knockout/2.1.0/knockout-min.js"></script>
    <script type="text/javascript" src="assets/js/viewbooking.js"></script>
    <script type="text/javascript">
        var taxiId;
        var userID =  '<% =Page.User.Identity.Name %>';
     <% if (User.IsInRole("Taxi")){ %>
         var taxiHub;
         taxiId = '<% =Page.User.Identity.Name %>';
         $(document).ready(function () {
             taxiHub = $.connection.taxiHub;
             taxiHub.pendingBooking = function () {
                    $('#pendingBookings').slideDown();
                };                
             taxiHub.acceptedBooking = function (id) {
                    acceptedBooking(id);
                };                
             taxiHub.cancelBooking = function (id) {
                    cancelBooking(id);
                };
            window.hubReady.done(function () {
                <% if (Page.User.IsInRole("Taxi")){ %>
                    taxiHub.announce('<% =Page.User.Identity.Name %>');
                <%} %>
            });
         });
         <%} %>
    </script>
</asp:Content>
