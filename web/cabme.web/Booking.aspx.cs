using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Service = cabme.web.Service;

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
                return input;
            }
            else
            {
                return "Booking not confirmed";
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (Service.Entities.Booking.Confirm(btn.CommandArgument) != null)
            {
                BookHub.SendClientMessage(booking.PhoneNumber, "Booking confirmed. Thank you for using cabme.");

                BindData();
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