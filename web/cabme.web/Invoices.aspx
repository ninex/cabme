<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Invoices.aspx.cs" Inherits="cabme.web.Invoices" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" media="screen" href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/themes/base/jquery-ui.css">
    <style>
        .ui-datepicker-calendar
        {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <label for="startDate">
            Date :</label>
        <input name="startDate" id="startDate" class="date-picker" readonly="true" data-bind="datepicker: monthYear,datepickerOptions:
          {
            changeMonth: true,
            changeYear: true,
            showButtonPanel: true,
            dateFormat: 'MM yy',
            onClose: function(dateText, inst) {
                var month = $('#ui-datepicker-div .ui-datepicker-month :selected').val();
                var year = $('#ui-datepicker-div .ui-datepicker-year :selected').val();                
                $root.monthYear(new Date(year, month, 1));
                $root.loadData(year, parseInt(month) + 1);
                $(this).blur();
            }
          }" />
        <label>Detailed
            <input type="checkbox" data-bind="checked: detailed" /></label>
        <label data-bind="visible: detailed()">
            Reference Code :</label>
        <input type="text" data-bind="value: filter, valueUpdate: 'afterkeydown', visible: detailed()" />
    </div>
    <div data-bind="foreach: invoice">
        <div class="table" style="width: 100%" data-bind="visible: $root.detailed()">
            <div style="display: table-header-group">
                <div class="cell">
                    <b>Suburb From</b>
                </div>
                <div class="cell">
                    <b>Reference Code</b>
                </div>
                <div class="cell">
                    <b>Time</b>
                </div>
            </div>
            <!-- ko foreach: itemsToShow -->
            <div class="row" style="width: 100%">
                <div class="cell" style="margin: 10px; border-bottom: 1px solid #eee;">
                    <label data-bind="text: suburbFrom">
                    </label>
                </div>
                <div class="cell" style="margin: 10px; border-bottom: 1px solid #eee;">
                    <label data-bind="text: refCode">
                    </label>
                </div>
                <div class="cell" style="margin: 10px; border-bottom: 1px solid #eee;">
                    <label data-bind="text: time">
                    </label>
                </div>
            </div>
            <!-- /ko -->
            <div class="row">
                <div class="cell">
                </div>
                <div class="cell">
                </div>
                <div class="cell" style="text-align: right">
                    <b>Total:&nbsp;</b><label data-bind="text: total">
                    </label>
                </div>
            </div>
        </div>
        <div class="table" style="width: 100%" data-bind="visible: !$root.detailed()">
            <div class="row">
                <div class="cell">
                </div>
                <div class="cell">
                </div>
                <div class="cell">
                </div>
            </div>
            <div class="row">
                <div class="cell">
                </div>
                <div class="cell">
                </div>
                <div class="cell" style="text-align: right">
                    <b>Total:&nbsp;</b><label data-bind="text: total">
                    </label>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Scripts" ContentPlaceHolderID="Scripts" runat="server">
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/jquery-ui.min.js"></script>
    <script type="text/javascript" src="http://cdnjs.cloudflare.com/ajax/libs/knockout/2.1.0/knockout-min.js"></script>
    <script type="text/javascript" src="assets/js/invoices.js"></script>
    <script type="text/javascript">
        var taxiId;
         <% if (User.IsInRole("Taxi")){ %>
                taxiId = '<% =Page.User.Identity.Name %>';
         <% } %>
        $(function () {
            $('.date-picker').datepicker();
        });
    </script>
</asp:Content>
