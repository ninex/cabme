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
       <p>Disclaimer: This is a dummy form and doesn't book a taxi.</p>
    <div id="book">
        <label>
            Pickup date<input id="pickupDate" type="date" /></label>
        <label>
            Pickup time<input id="pickupTime" type="time" /></label>
        <label>
            Number of people<input type="number" value="1" /></label>
        <label>
            Address from<input type="text" /></label>
        <label>
            Address to<input type="text" /></label>
            <input type="button" id="btnConfirm" value="Confirm" />
    </div>
    <%} %>
    <script>
        $(document).ready(function () {
            var now = new Date();
            var today = now.getFullYear() + '-' + (now.getMonth() + 1).padLeft(2, '0') + '-' + now.getDate().padLeft(2, '0');
            var time = now.getHours().padLeft(2, '0') + ':' + now.getMinutes().padLeft(2, '0');
            $('#pickupDate').val(today);
            $('#pickupTime').val(time);
        });
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
</asp:Content>
