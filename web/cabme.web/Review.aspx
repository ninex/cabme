<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Review.aspx.cs" Inherits="cabme.web.ReviewPage" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Review booking</h2>
    <asp:Panel runat="server" ID="reviewPnl" Style="height: 100%" Visible="false">
        <div class="table" id="booking">
            <div class="row">
                <div class="cell">
                    <b>Pickup Time: </b>
                </div>
                <div class="cell lastCell">
                    <asp:Label ID="lblPickupTime" runat="server" /></div>
            </div>
            <div class="row">
                <div class="cell">
                    <b>From: </b>
                </div>
                <div class="cell lastCell">
                    <asp:Label ID="lblAddrFrom" runat="server" /></div>
            </div>
            <div class="row">
                <div class="cell">
                    <b>To: </b>
                </div>
                <div class="cell lastCell">
                    <asp:Label ID="lblAddrTo" runat="server" /></div>
            </div>
            <br />
            <div class="row">
                <div class="cell">
                    <b>On Time</b>
                </div>
                <div class="cell lastCell">
                    <asp:CheckBox runat="server" ID="chkOnTime" /></div>
            </div>
            <br />
            <div class="row">
                <div class="cell">
                    <b>Friendly</b>
                </div>
                <div class="cell lastCell">
                    <asp:CheckBox runat="server" ID="chkFriendly" /></div>
            </div>
            <br />
            <div class="row">
                <div class="cell">
                    <b>Question 3</b>
                </div>
                <div class="cell lastCell">
                    <asp:CheckBox runat="server" ID="chk3" /></div>
            </div>
            <br />
            <div class="row">
                <div class="cell">
                    <b>Question 4</b>
                </div>
                <div class="cell lastCell">
                    <asp:CheckBox runat="server" ID="chk4" /></div>
            </div>
            <br />
            <div class="row">
                <div class="cell">
                    <b>Question 5</b>
                </div>
                <div class="cell lastCell">
                    <asp:CheckBox runat="server" ID="chk5" /></div>
            </div>
            <br />
            <div class="row">
                <div class="cell">
                    <b>Comment</b>
                </div>
                <div class="cell lastCell">
                </div>
            </div>
            <div class="row">
                <div class="cell">
                </div>
                <div class="cell lastCell">
                    <asp:TextBox TextMode="MultiLine" Rows="4" runat="server" ID="txtComments" /></div>
            </div>
            <br />
            <div class="row">
                <div class="cell">
                </div>
                <div class="cell lastCell">
                    <asp:Button runat="server" ID="btnSubmit" Text="Send" OnClick="btnSubmit_Click"  CssClass="button"/></div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Scripts" ContentPlaceHolderID="Scripts" runat="server">
</asp:Content>
