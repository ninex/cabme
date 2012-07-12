using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cabme.web.Service.Entities;

namespace cabme.web
{
    public partial class ReviewPage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string hash = Request.QueryString["hash"];
                if (!string.IsNullOrEmpty(hash))
                {
                    if (!User.IsInRole("Taxi") && User.Identity.IsAuthenticated)
                    {
                        var booking = Booking.GetBookingByHash(hash);
                        var review = Review.GetReview(User.Identity.Name, booking.Id, booking.TaxiId);
                        if (booking != null)
                        {
                            reviewPnl.Visible = true;
                            lblPickupTime.Text = booking.PickupTime;
                            lblAddrFrom.Text = booking.AddrFrom.Replace(",", ",<br/>");
                            lblAddrTo.Text = booking.AddrTo.Replace(",", ",<br/>");
                        }
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string hash = Request.QueryString["hash"];
            var booking = Booking.GetBookingByHash(hash);
            var preview = Review.GetReview(User.Identity.Name, booking.Id, booking.TaxiId);

            int rating = 0;
            rating = (chkOnTime.Checked ? 1 : 0) + (chkFriendly.Checked ? 1 : 0 )+ (chk3.Checked ? 1 : 0 )+ (chk4.Checked ? 1 : 0 )+ (chk5.Checked ? 1 : 0);

            Review review = new Review()
            {
                Id = preview != null ? preview.Id : 0,
                Active = true,
                BookingId = booking.Id,
                TaxiId = booking.TaxiId,
                UserName = User.Identity.Name,
                Comment = txtComments.Text,
                Rating = (byte)rating
            };
            Review.MakeReview(review);

            Response.Redirect("Reviews.aspx");
        }
    }
}