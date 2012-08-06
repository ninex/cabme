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
        <div id="step1">
            <p id="btnMakeFull" onclick="makeFull();" class="clickMe" >Press here to switch to detailed booking.</p>
            <label>
                City
                <select id="city" onchange="cityChanged();">
                    <option selected value="Cape Town">Cape Town</option>
                    <option value="Johannesburg">Johannesburg</option>
                </select></label>
            <label>
                Phone number<input type="tel" id="txtPhone" /></label>
            <label full>
                Pickup date<input id="pickupDate" type="date" /></label>
            <label full>
                Pickup time<input id="pickupTime" type="time" /></label>
            <label full>
                Number of people<input type="number" id="number" value="1" /></label>
            <label>
                Pickup from<input type="text" id="from" />
                <select id="fromSuburb" onchange="suburbChanged();">
                </select></label>
            <label full>
                Drop off<input type="text" id="to" />
                <select id="toSuburb" onchange="suburbChanged();">
                </select></label>
            <label id="lblQuickTaxi">
                Taxi<select id="ddlQuickTaxi"></select></label>
            <input type="button" id="btnBookMin" value="Book" onclick="step1Min();" />
            <input full type="button" id="btnConfirm" value="Next" onclick="step1();" />
        </div>
        <div id="step2" style="display: none">            
            <label>
                Distance<p id="txtDistance">
                </p>
            </label>
            <label>
                Taxi<select id="ddlTaxi"></select></label>
            <label>
                Estimated route</label>
            <div id="map" style="overflow:hidden">
            </div>            
            <input type="button" id="btnBook" value="Book" onclick="step2();" />
        </div>
        <div id="step3" style="display: none; border: 1px solid #000; margin: 5px; padding: 5px;">
            <h3>Booking status</h3>
            <p id="msgStatus" class="status">
                Booking sent to server.</p>
        </div>
    </div>
    <p>
        Disclaimer: This is a beta form and doesn't book a taxi.</p>
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
<asp:Content ContentPlaceHolderID="Scripts" runat="server" ID="Scripts">
    <script type="text/javascript" src="assets/js/book.js"></script>
</asp:Content>
