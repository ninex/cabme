﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace cabme.webmvc.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [Required(AllowEmptyStrings=false)]
        public string PhoneNumber { get; set; }

        [Range(1,16)]
        public short NumberOfPeople { get; set; }

        public DateTime PickupTime { get; set; }

        [Required(AllowEmptyStrings=false)]
        public string AddrFrom { get; set; }

        public int latitudeFrom { get; set; }

        public int longitudeFrom { get; set; }

        public string AddrTo { get; set; }

        public int latitudeTo { get; set; }

        public int longitudeTo { get; set; }

        public int ComputedDistance { get; set; }

        public int EstimatedPrice { get; set; }

        public bool Active { get; set; }

        public bool TaxiCancelled { get; set; }

        public bool UserCancelled { get; set; }

        public bool TaxiAccepted { get; set; }

        public bool UserAccepted { get; set; }

        public int WaitingTime { get; set; }

        [Range(1, int.MaxValue)]
        public int TaxiId { get; set; }

        public DateTime LastModified { get; set; }

        public DateTime Created { get; set; }

        public string Hash { get; set; }

        public int SuburbFromId { get; set; }

        public Suburb SuburbFrom { get; set; }

        public Taxi SelectedTaxi { get; set; }

        public string ReferenceCode { get; set; }
    }
}