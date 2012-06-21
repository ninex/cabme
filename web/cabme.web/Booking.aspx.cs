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
                    listBookings.DataSource = bookings;
                    listBookings.DataBind();
                }
            }
        }

        protected bool ShowConfirm(bool confirmed)
        {
            return (User.IsInRole("Taxi") && !confirmed);
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
                BindData();
            }
        }
    }
}