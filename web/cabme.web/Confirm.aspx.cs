using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Data = cabme.data;
using cabme.web.Service.Entities;

namespace cabme.web
{
    public partial class Confirm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string hash = Request.QueryString["hash"];
                if (!string.IsNullOrEmpty(hash))
                {
                    Booking booking = Booking.Confirm(hash);
                    if (booking == null)
                    {
                        status.InnerHtml = "Booking doesn't exist";
                    }
                    else
                    {
                        status.InnerHtml = "Booking from " + booking.AddrFrom + " confirmed. ";
                        string mailBody = string.Format("Booking received from {0}<br/>People:{4}<br/>Pickup time: {1}<br/>From:{2}<br/>To:{3}<br/>",
                            booking.PhoneNumber, booking.PickupTime, booking.AddrFrom, booking.AddrTo, booking.NumberOfPeople);
                        //Send confirm booking email
                        cabme.web.Service.Mail.SendMail("test@abrie.net", "cabme@abrie.net", "Test booking email", mailBody);
                    }
                }
                else
                {
                    status.InnerHtml = "Booking doesn't exist";
                }
            }
        }
    }
}