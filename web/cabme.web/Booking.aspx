<%@ Page Title="View Bookings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Booking.aspx.cs" Inherits="cabme.web.BookingPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Bookings</h2>
    <article class="tabs">
        <h3 id="htab1">
            Active</h3>
        <h3 id="htab2">
            Completed</h3>
        <h3 id="htab3">
            Incomplete</h3>
        <div id="tab1" style="display: none">
            <asp:Repeater runat="server" ID="ActiveBookings" OnItemDataBound="listBookings_ItemDataBound">
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
                        <div class="row">
                            <asp:Button runat="server" ID="btnConfirm" Text="Confirm" CommandArgument='<%#Eval("Hash") %>'
                                Visible='<%# ShowConfirm((bool)Eval("Confirmed")) %>' OnClick="btnConfirm_Click" />
                        </div>
                    </asp:Panel>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Panel runat="server" ID="NoData" Visible="false">
                        <p>
                            No active bookings</p>
                    </asp:Panel>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div id="tab2" style="display: none">
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
                        <div class="row">
                            <asp:Button runat="server" ID="btnReview" Text="Review" OnClick="btnReview_Click"
                                CommandArgument='<% #Eval("Hash") %>' Visible='<%# ShowReview() %>' />
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
        </div>
    </article>
</asp:Content>
<asp:Content ContentPlaceHolderID="Scripts" runat="server" ID="Scripts">
    <script type="text/javascript" src="assets/js/booking.js"></script>
    <script type="text/javascript">
     <% if (User.IsInRole("Taxi")){ %>
         var taxiHub;
         $(document).ready(function () {
             taxiHub = $.connection.taxiHub;
             taxiHub.showMessage = function (message) {
                    
                };
         });
         <%} %>
    </script>
</asp:Content>
