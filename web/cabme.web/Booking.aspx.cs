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
                if (User.IsInRole("Taxi") && User.Identity.IsAuthenticated)
                {
                    var bookings = Service.Entities.Booking.GetAllTaxiBookingsForUser(User.Identity.Name);
                    if (bookings != null)
                    {
                        pl.InnerHtml = "<p>Number of bookings: " + bookings.Count() + "</p>";
                    }
                    else
                    {
                        pl.InnerHtml = "No bookings";
                    }
                }
            }
        }
    }
}