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

        public static Taxis GetAllTaxis()
        {
            try
            {
                using (Data.contentDataContext context = new Data.contentDataContext())
                {
                    return new Taxis(context.Taxis.AsEnumerable().Select(taxi => new Taxi
                    {
                        Id = taxi.Id,
                        Name = taxi.Name,
                        PhoneNumber = taxi.PhoneNumber,
                        RatePerKm = taxi.RatePerKm,
                        MinRate = taxi.MinRate,
                        Units = taxi.Units,
                        dStartOfService =taxi.StartOfService,
                        dEndOfService = taxi.EndOfService,
                        Is24HService = taxi.Is24HService,
                        FleetSize = taxi.FleetSize
                    }).ToList());
                }
            }
            catch { return null; }
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