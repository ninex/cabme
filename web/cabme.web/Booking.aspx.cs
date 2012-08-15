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
        }
        /*
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
            }
        }*/

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

        protected void btnReview_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Response.Redirect("~/Review.aspx?hash=" + btn.CommandArgument);
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