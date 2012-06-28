<%@ Page Title="View Bookings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Booking.aspx.cs" Inherits="cabme.web.BookingPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Active Bookings</h2>
    <asp:Repeater runat="server" ID="listBookings">
        <HeaderTemplate>
            <div style="height: 100%">
        </HeaderTemplate>
        <ItemTemplate>
            <div class="table" runat="server">
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
                        <%# AllowedToDisplay(((string)Eval("AddrTo")).Replace(",", ",<br/>"), (bool)Eval("Confirmed")) %></p></div>
                </div>
                <div class="row">
                    <asp:Button runat="server" ID="btnConfirm" Text="Confirm" CommandArgument='<%#Eval("Hash") %>'
                        Visible='<%# ShowConfirm((bool)Eval("Confirmed")) %>' OnClick="btnConfirm_Click" />
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>
</asp:Content>
