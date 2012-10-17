using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cabme.webmvc.Models
{
    public class Taxi
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int RatePerKm { get; set; }
        public int MinRate { get; set; }
        public int Units { get; set; }
        public DateTime StartOfService { get; set; }
        public DateTime EndOfService { get; set; }
        public bool Is24HService { get; set; }
        public short FleetSize { get; set; }
        public int PriceEstimate { get; set; }

        public static int GetPriceEstimate(int distance, int ratePerKm, int minRate, int units)
        {
            int estimate = 0;
            if (distance > 0)
            {
                var computedPrice = (ratePerKm * distance) / 1000;
                if (computedPrice < minRate)
                {
                    computedPrice = minRate;
                }
                else
                {
                    if (computedPrice % units > 0)
                    {
                        computedPrice = computedPrice - (computedPrice % units) + units;
                    }
                }
                estimate = computedPrice;
            }
            return estimate;
        }
    }
}