using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Data = cabme.data;

namespace cabme.web.Service.Entities
{
    [DataContract(Namespace = "http://cabme.co.za/taxi")]
    public class Taxi
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public int RatePerKm { get; set; }

        [DataMember]
        public int MinRate { get; set; }

        [DataMember]
        public int Units { get; set; }

        [DataMember]
        public string StartOfService { get; set; }
        public DateTime dStartOfService
        {
            set
            {
                StartOfService = value.ToString("hh:mm:ss");
            }
        }

        [DataMember]
        public string EndOfService { get; set; }
        public DateTime dEndOfService
        {
            set
            {
                EndOfService = value.ToString("hh:mm:ss");
            }
        }

        [DataMember]
        public bool Is24HService { get; set; }

        [DataMember]
        public short FleetSize { get; set; }

        [DataMember]
        public int PriceEstimate { get; set; }

        public static Taxis GetAllTaxis()
        {
            return GetAllTaxis(0);
        }
        public static Taxis GetAllTaxis(int distance)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var taxis = new Taxis(context.Taxis.AsEnumerable().Select(taxi => new Taxi
                {
                    Id = taxi.Id,
                    Name = taxi.Name,
                    PhoneNumber = taxi.PhoneNumber,
                    RatePerKm = taxi.RatePerKm,
                    MinRate = taxi.MinRate,
                    Units = taxi.Units,
                    dStartOfService = taxi.StartOfService,
                    dEndOfService = taxi.EndOfService,
                    Is24HService = taxi.Is24HService,
                    FleetSize = taxi.FleetSize
                }).ToList());
                if (distance > 0)
                {
                    foreach (var taxi in taxis)
                    {
                        taxi.GetPriceEstimate(distance);
                    }
                }
                return taxis;
            }
        }

        public static Taxi GetTaxi(int taxiId)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return context.Taxis.Where(p => p.Id == taxiId).Select(taxi => new Taxi
                {
                    Id = taxi.Id,
                    Name = taxi.Name,
                    PhoneNumber = taxi.PhoneNumber,
                    RatePerKm = taxi.RatePerKm,
                    MinRate = taxi.MinRate,
                    Units = taxi.Units,
                    dStartOfService = taxi.StartOfService,
                    dEndOfService = taxi.EndOfService,
                    Is24HService = taxi.Is24HService,
                    FleetSize = taxi.FleetSize
                }).SingleOrDefault();
            }
        }

        public static int GetTaxiIdForUser(string userName)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return (from user in context.Users
                        join userTaxi in context.UserTaxis on user.Id equals userTaxi.UserId
                        where user.Name == userName
                        select userTaxi.TaxiId).SingleOrDefault();
            }
        }

        public int GetPriceEstimate(int distance)
        {
            if (distance > 0)
            {
                var computedPrice = (RatePerKm * distance) / 1000;
                if (computedPrice < MinRate)
                {
                    computedPrice = MinRate;
                }
                else
                {
                    if (computedPrice % Units > 0)
                    {
                        computedPrice = computedPrice - (computedPrice % Units) + Units;
                    }
                }
                this.PriceEstimate = computedPrice;
            }
            else
            {
                this.PriceEstimate = 0;
            }
            return this.PriceEstimate;
        }
    }

    [CollectionDataContract(Namespace = "http://cabme.co.za/taxis")]
    public class Taxis : List<Taxi>
    {
        public Taxis()
        {
        }

        public Taxis(List<Taxi> taxis)
            : base(taxis)
        {
        }
    }
}