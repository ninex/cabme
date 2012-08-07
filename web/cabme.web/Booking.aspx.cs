using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Service = cabme.web.Service;
using cabme.web.Service.Hubs;

namespace cabme.web
{
    public partial class BookingPage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            if (User.Identity.IsAuthenticated)
            {
                Service.Entities.Bookings bookings;
                if (User.IsInRole("Taxi"))
                {
                    bookings = Service.Entities.Booking.GetAllTaxiBookingsForUser(User.Identity.Name);
                }
                else
                {
                    if (User.IsInRole("Admin"))
                    {
                        bookings = Service.Entities.Booking.GetAllBookings();
                    }
                    else
                    {
                        bookings = Service.Entities.Booking.GetAllActiveBookingsForUser(User.Identity.Name);
                    }
                }

                if (bookings != null)
                {
                    ActiveBookings.DataSource = bookings.Where(p => !p.Confirmed && p.dPickupTime.AddMinutes(30) > DateTime.Now);
                    ActiveBookings.DataBind();

                    CompletedBookings.DataSource = bookings.Where(p => p.Confirmed);
                    CompletedBookings.DataBind();

                    IncompleteBookings.DataSource = bookings.Where(p => !p.Confirmed && p.dPickupTime.AddMinutes(30) < DateTime.Now);
                    IncompleteBookings.DataBind();
                }
            }
        }

        protected bool ShowConfirm(bool confirmed)
        {
            return (User.IsInRole("Taxi") && !confirmed);
        }

        protected bool ShowReview()
        {
            return (!User.IsInRole("Taxi"));
        }

        protected string AllowedToDisplay(string input, bool confirmed)
        {
            if (confirmed || !User.IsInRole("Taxi"))
            {
                return string.IsNullOrEmpty(input) ? "" : input;
            }
            else
            {
                return "Booking not confirmed";
            }
        }
        protected string GetSuburb(string AddrFrom, Service.Entities.Suburb suburb)
        {
            if (suburb == null || string.IsNullOrEmpty(suburb.Name))
            {
                return "No suburb information available.";
            }
            else
            {
                return suburb.Name;
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            TextBox txtArrival = btn.Parent.FindControl("txtArrival") as TextBox;
            int minutes = 0;
            string msg;
            if (txtArrival != null && int.TryParse(txtArrival.Text, out minutes))
            {
                msg = "Booking confirmed. Taxi will arrive in " + minutes + " min. Thank you for using cabme.";
            }
            else
            {
                msg = "Booking confirmed. Thank you for using cabme.";
            }
            var booking = Service.Entities.Booking.Confirm(btn.CommandArgument);
            if (booking != null)
            {
                BookHub.SendClientMessage(booking.PhoneNumber, msg);

                BindData();
            }
        }

        protected void btnReview_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Response.Redirect("~/Review.aspx?hash=" + btn.CommandArgument);
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void listBookings_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            var booking = e.Item.DataItem as Service.Entities.Booking;
            Panel ctrl = e.Item.FindControl("booking") as Panel;
            if (ctrl != null)
            {
                if (!booking.Confirmed && booking.dPickupTime.AddMinutes(10) < DateTime.Now)
                {
                    if (booking.dPickupTime.AddMinutes(30) < DateTime.Now)
                    {
                        ctrl.CssClass = "table verylate";
                    }
                    else
                    {
                        ctrl.CssClass = "table late";
                    }
                }
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                if (((Repeater)sender).Items.Count <= 0)
                {
                    Panel pnlNoData = (Panel)e.Item.FindControl("NoData");
                    if (pnlNoData != null)
                    {
                        pnlNoData.Visible = true;
                    }
                }
            }
        }
    }
}