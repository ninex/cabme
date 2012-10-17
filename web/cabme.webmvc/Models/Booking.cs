using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cabme.webmvc.Models
{
    public class Booking
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public short NumberOfPeople { get; set; }
        public DateTime PickupTime { get; set; }
        public string AddrFrom { get; set; }
        public int latitudeFrom { get; set; }
        public int longitudeFrom { get; set; }
        public string AddrTo { get; set; }
        public int latitudeTo { get; set; }
        public int longitudeTo { get; set; }
        public int ComputedDistance { get; set; }
        public int EstimatedPrice { get; set; }
        public bool Active { get; set; }
        public bool Confirmed { get; set; }
        public bool Accepted { get; set; }
        public int WaitingTime { get; set; }
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