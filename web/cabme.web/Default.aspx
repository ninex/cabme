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
    <p>
        Disclaimer: This is a dummy form and doesn't book a taxi.</p>
    <div id="book">
        <div id="step1">
            <label>
                Pickup date<input id="pickupDate" type="date" /></label>
            <label>
                Pickup time<input id="pickupTime" type="time" /></label>
            <label>
                Number of people<input type="number" value="1" /></label>
            <label>
                Address from<input type="text" id="from"/></label>
            <label>
                Address to<input type="text" id="to"/></label>
            <input type="button" id="btnConfirm" value="Confirm" onclick="step1();"/>
        </div>
        <div id="step2">
            <label>
                Distance<label runat="server" id="txtDistance" /></label>
            <label>
                Taxi<select></select></label>
        </div>
        <div id="step3">
            <label>
                Price<label runat="server" id="txtPrice" /></label>
            <input type="button" id="btnBook" value="Book" />
        </div>
    </div>
    <script>
        $.cors = true;
              $(document).ready(function () {
                  var now = new Date();
                  var today = now.getFullYear() + '-' + (now.getMonth() + 1).padLeft(2, '0') + '-' + now.getDate().padLeft(2, '0');
                  var time = now.getHours().padLeft(2, '0') + ':' + now.getMinutes().padLeft(2, '0');
                  $('#pickupDate').val(today);
                  $('#pickupTime').val(time);
              });

              function step1() {
                  //Check if everything valid
                  
              }

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
    <%} %>    
</asp:Content>
