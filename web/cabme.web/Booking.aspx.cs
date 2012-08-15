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
        }
    }
}