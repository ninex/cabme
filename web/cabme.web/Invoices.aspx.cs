using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace cabme.web
{
    public partial class Invoices : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.IsInRole("Taxi"))
            {
                Response.Redirect("~/Default.aspx");
            }
        }
    }
}