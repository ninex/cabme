using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cabme.webmvc.Models
{
    public class Confirmation
    {
        public string Arrival { get; set; }
        public string Hash { get; set; }
        public string PhoneNumber { get; set; }
        public string RefCode { get; set; }
    }
}