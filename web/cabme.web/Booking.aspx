<%@ Page Title="View Bookings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Booking.aspx.cs" Inherits="cabme.web.BookingPage" ClientIDMode="Static" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Bookings</h2>
    <div style="float: right">
        <input type="image" class="refresh" src="assets/images/refresh.png" alt="Refresh" data-bind="click: loadData" onclick="btnRefresh();"/>
    </div>
    <article class="tabs">
        <h3 id="htab1">
            Active</h3>
        <h3 id="htab2">
            Completed</h3>
        <h3 id="htab3">
            Incomplete</h3>
        <div id="tab1" style="display: none">
            <div style="height: 100%">
                <div id="pendingBookings" style="display: none">
                    <p>
                        You have new bookings awaiting confirmation. Refresh to view.</p>
                </div>
                <div  data-bind="foreach: bookings">
                <div class="table">
                    <div class="row">
                        <div class="cell">
                            <b>Phone Number: </b>
                        </div>
                        <div class="cell lastCell">
                            <div data-bind="text: phoneNumber, visible: confirmed == true">
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
                            </div></div>
                    </div>
                    <div class="row">
                        <div class="cell">
                            <b>From: </b>
                        </div>
                        <div class="cell lastCell">
                            <div data-bind="text: addrFrom, visible: confirmed == true">
                            </div></div>
                    </div>
                    <div class="row">
                        <div class="cell">
                            <b>To: </b>
                        </div>
                        <div class="cell lastCell">
                            <div data-bind="text: addrTo, visible: confirmed == true">
                            </div></div>
                    </div>
                    <div class="row" data-bind="visible: confirmed == false" >
                        <div class="cell">
                            <b>Driver Code: </b>
                        </div>
                        <div class="cell lastCell">
                            <input type="text" maxlength="8" style="width: 40px;" />
                            <%--<asp:TextBox runat="server" ID="txtRefCode" Columns="2" MaxLength="8"></asp:TextBox>--%>
                        </div>
                    </div>
                    <div class="row" data-bind="visible: confirmed == false" >
                        <div class="cell">
                            <b>Minutes to arrival: </b>
                        </div>
                        <div class="cell lastCell">
                        <input type="text" maxlength="2" style="width: 40px;" />
                            <%--<asp:TextBox runat="server" ID="txtArrival" Columns="2" MaxLength="2"></asp:TextBox>--%>
                        </div>
                    </div>
                    <div class="row" data-bind="visible: confirmed == false" >
                        <div class="cell">
                        </div>
                        <div class="cell lastCell">
                            <asp:Button runat="server" ID="btnConfirm" CssClass="button" Text="Confirm" CommandArgument='<%#Eval("Hash") %>'
                                OnClick="btnConfirm_Click" />
                            <asp:Button runat="server" ID="btnIndependent" CssClass="button" Text="Independent"
                                CommandArgument='<%#Eval("Hash") %>' OnClick="btnIndependent_Click" />
                        </div>
                    </div>
                </div>
            </div>
            </div>
            <%--<asp:Repeater runat="server" ID="ActiveBookings" OnItemDataBound="listBookings_ItemDataBound"
                Visible="false">
                <HeaderTemplate>
                    <div style="height: 100%">
                        <div id="pendingBookings" style="display: none">
                            <p>
                                You have new bookings awaiting confirmation. Refresh to view.</p>
                        </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Panel class="table" runat="server" ID="booking">
                        <div class="row">
                            <div class="cell">
                                <b>Phone Number: </b>
                            </div>
                            <div class="cell lastCell">
                                <%# AllowedToDisplay((string)Eval("PhoneNumber"), (bool)Eval("Confirmed"))%></p></div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>People: </b>
                            </div>
                            <div class="cell lastCell">
                                <%# Eval("NumberOfPeople")%></p></div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>Pickup Time: </b>
                            </div>
                            <div class="cell lastCell">
                                <%# Eval("PickupTime")%></p></div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>Suburb: </b>
                            </div>
                            <div class="cell lastCell">
                                <%# GetSuburb(((string)Eval("AddrFrom")), (cabme.web.Service.Entities.Suburb)Eval("SuburbFrom")) %></p></div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>From: </b>
                            </div>
                            <div class="cell lastCell">
                                <%# AllowedToDisplay(((string)Eval("AddrFrom")).Replace(",", ",<br/>"), (bool)Eval("Confirmed")) %></p></div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>To: </b>
                            </div>
                            <div class="cell lastCell">
                                <%# AllowedToDisplay(((string)Eval("AddrTo")), (bool)Eval("Confirmed")).Replace(",", ",<br/>")%></p></div>
                        </div>
                        <asp:Panel ID="pnlRef" class="row" runat="server" Visible='<%# ShowConfirm((bool)Eval("Confirmed")) %>'>
                            <div class="cell">
                                <b>Driver Code: </b>
                            </div>
                            <div class="cell lastCell">
                                <asp:TextBox runat="server" ID="txtRefCode" Columns="2" MaxLength="8"></asp:TextBox>
                            </div>
                        </asp:Panel>
                        <asp:Panel class="row" runat="server" Visible='<%# ShowConfirm((bool)Eval("Confirmed")) %>'>
                            <div class="cell">
                                <b>Minutes to arrival: </b>
                            </div>
                            <div class="cell lastCell">
                                <asp:TextBox runat="server" ID="txtArrival" Columns="2" MaxLength="2"></asp:TextBox>
                            </div>
                        </asp:Panel>
                        <asp:Panel ID="Panel1" class="row" runat="server" Visible='<%# ShowConfirm((bool)Eval("Confirmed")) %>'>
                            <div class="cell">
                            </div>
                            <div class="cell lastCell">
                                <asp:Button runat="server" ID="btnConfirm" CssClass="button" Text="Confirm" CommandArgument='<%#Eval("Hash") %>'
                                    OnClick="btnConfirm_Click" />
                                <asp:Button runat="server" ID="btnIndependent" CssClass="button" Text="Independent"
                                    CommandArgument='<%#Eval("Hash") %>' OnClick="btnIndependent_Click" />
                            </div>
                        </asp:Panel>
                    </asp:Panel>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Panel runat="server" ID="NoData" Visible="false">
                        <p>
                            No active bookings</p>
                    </asp:Panel>
                    </div>
                </FooterTemplate>
            </asp:Repeater>--%>
        </div>
        <%-- <div id="tab2" style="display: none">
            <asp:Repeater runat="server" ID="CompletedBookings" OnItemDataBound="listBookings_ItemDataBound">
                <HeaderTemplate>
                    <div style="height: 100%">
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Panel class="table" runat="server" ID="booking">
                        <div class="row">
                            <div class="cell">
                                <b>Phone Number: </b>
                            </div>
                            <div class="cell lastCell">
                            </div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>People: </b>
                            </div>
                            <div class="cell lastCell">
                                <%# Eval("NumberOfPeople")%></p></div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>Pickup Time: </b>
                            </div>
                            <div class="cell lastCell">
                                <%# Eval("PickupTime")%></p></div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>From: </b>
                            </div>
                            <div class="cell lastCell">
                                <%# AllowedToDisplay(((string)Eval("AddrFrom")).Replace(",", ",<br/>"), (bool)Eval("Confirmed")) %></p></div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>To: </b>
                            </div>
                            <div class="cell lastCell">
                                <%# AllowedToDisplay(((string)Eval("AddrTo")), (bool)Eval("Confirmed")).Replace(",", ",<br/>")%></p></div>
                        </div>
                        <div class="row">
                            <asp:Button runat="server" ID="btnReview" Text="Review" OnClick="btnReview_Click"
                                CssClass="button" CommandArgument='<% #Eval("Hash") %>' Visible='<%# ShowReview() %>' />
                        </div>
                    </asp:Panel>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div id="tab3" style="display: none">
            <asp:Repeater runat="server" ID="IncompleteBookings" OnItemDataBound="listBookings_ItemDataBound">
                <HeaderTemplate>
                    <div style="height: 100%">
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Panel class="table" runat="server" ID="booking">
                        <div class="row">
                            <div class="cell">
                                <b>Phone Number: </b>
                            </div>
                            <div class="cell lastCell">
                                <%# AllowedToDisplay((string)Eval("PhoneNumber"), (bool)Eval("Confirmed"))%></p></div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>People: </b>
                            </div>
                            <div class="cell lastCell">
                                <%# Eval("NumberOfPeople")%></p></div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>Pickup Time: </b>
                            </div>
                            <div class="cell lastCell">
                                <%# Eval("PickupTime")%></p></div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>From: </b>
                            </div>
                            <div class="cell lastCell">
                                <%# AllowedToDisplay(((string)Eval("AddrFrom")).Replace(",", ",<br/>"), (bool)Eval("Confirmed")) %></p></div>
                        </div>
                        <div class="row">
                            <div class="cell">
                                <b>To: </b>
                            </div>
                            <div class="cell lastCell">
                                <%# AllowedToDisplay(((string)Eval("AddrTo")), (bool)Eval("Confirmed")).Replace(",", ",<br/>")%></p></div>
                        </div>
                    </asp:Panel>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
        </div>--%>
    </article>
</asp:Content>
<asp:Content ContentPlaceHolderID="Scripts" runat="server" ID="Scripts">
    <script type="text/javascript" src="http://cdnjs.cloudflare.com/ajax/libs/knockout/2.1.0/knockout-min.js"></script>
    <script type="text/javascript" src="assets/js/booking.js"></script>
    <script type="text/javascript">    
     <% if (User.IsInRole("Taxi")){ %>
         var taxiHub;
         var taxiId = '<% =Page.User.Identity.Name %>';
         $(document).ready(function () {
             taxiHub = $.connection.taxiHub;
             taxiHub.pendingBooking = function () {
                    $('#pendingBookings').slideDown();
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
