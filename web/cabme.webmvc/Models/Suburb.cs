﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cabme.webmvc.Models
{
    public class Suburb
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }
}