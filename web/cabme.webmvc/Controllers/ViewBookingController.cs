using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cabme.webmvc.Hubs;
using cabme.webmvc.Common;

namespace cabme.webmvc.Controllers
{
    public class ViewBookingController : Controller
    {
        //
        // GET: /ViewBooking/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}
