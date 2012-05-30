using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Data = cabme.data;

namespace cabme.web.Service.Entities
{
    [DataContract(Namespace = "http://cabme.co.za/booking")]
    public class Booking
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public short NumberOfPeople { get; set; }

        [DataMember]
        public string PickupTime { get; set; }
        public DateTime dPickupTime
        {
            set
            {
                PickupTime = value.ToString("yyyy-MM-dd hh:mm:ss");
            }
        }

        [DataMember]
        public string AddrFrom { get; set; }

        [DataMember]
        public int latitudeFrom { get; set; }

        [DataMember]
        public int longitudeFrom { get; set; }

        [DataMember]
        public string AddrTo { get; set; }

        [DataMember]
        public int latitudeTo { get; set; }

        [DataMember]
        public int longitudeTo { get; set; }

        [DataMember]
        public int ComputedDistance { get; set; }

        [DataMember]
        public int EstimatedPrice { get; set; }

        [DataMember]
        public bool Active { get; set; }

        [DataMember]
        public bool Confirmed { get; set; }

        [DataMember]
        public int TaxiId { get; set; }

        public DateTime LastModified { get; set; }

        public DateTime Created { get; set; }

        [DataMember]
        public Taxi SelectedTaxi { get; set; }

        public static Booking MakeBooking(Booking booking)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                Data.Booking dataBooking;
                if (booking.Id == 0)
                {
                    dataBooking = new Data.Booking()
                    {
                        Id = 0,
                        Created = DateTime.Now
                    };
                }
                else
                {
                    dataBooking = context.Bookings.Where(p => p.Id == booking.Id).SingleOrDefault();
                    if (dataBooking == null)
                    {
                        return null;
                    }
                }
                dataBooking.Name = booking.Name;
                dataBooking.PhoneNumber = booking.PhoneNumber;
                dataBooking.NumberOfPeople = booking.NumberOfPeople;
                dataBooking.PickupTime = DateTime.ParseExact(booking.PickupTime, "yyyy-MM-dd hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                dataBooking.AddrFrom = booking.AddrFrom;
                dataBooking.LatitudeFrom = booking.latitudeFrom;
                dataBooking.LongitudeFrom = booking.longitudeFrom;
                dataBooking.AddrTo = booking.AddrTo;
                dataBooking.LatitudeTo = booking.latitudeTo;
                dataBooking.LongitudeTo = booking.longitudeTo;
                dataBooking.ComputedDistance = booking.ComputedDistance;
                dataBooking.EstimatedPrice = booking.EstimatedPrice;
                dataBooking.Active = booking.Active;
                dataBooking.Confirmed = booking.Confirmed;
                dataBooking.TaxiId = booking.TaxiId;
                dataBooking.LastModified = DateTime.Now;
                if (dataBooking.Id == 0)
                {
                    context.Bookings.InsertOnSubmit(dataBooking);
                }
                context.SubmitChanges();
                booking.Id = dataBooking.Id;
            }
            return booking;
        }

        public static Bookings GetAllBookings()
        {
            try
            {
                using (Data.contentDataContext context = new Data.contentDataContext())
                {
                    return new Bookings(AllQueryableBookings(context).ToList());
                }
            }
            catch { return null; }
        }

        public static Bookings GetAllBookingsByNumber(string number)
        {
            try
            {
                using (Data.contentDataContext context = new Data.contentDataContext())
                {
                    return new Bookings(AllQueryableBookings(context).Where(p => p.PhoneNumber == number).ToList());
                }
            }
            catch { return null; }
        }

        private static IQueryable<Booking> AllQueryableBookings(Data.contentDataContext context)
        {
            return from booking in context.Bookings
                   join taxi in context.Taxis on booking.TaxiId equals taxi.Id into outer
                   from taxi in outer.DefaultIfEmpty()
                   select new Booking
                   {
                       Id = booking.Id,
                       Name = booking.Name,
                       PhoneNumber = booking.PhoneNumber,
                       NumberOfPeople = booking.NumberOfPeople,
                       dPickupTime =booking.PickupTime,
                       AddrFrom = booking.AddrFrom,
                       latitudeFrom = booking.LatitudeFrom.HasValue ? booking.LatitudeFrom.Value : 0,
                       longitudeFrom = booking.LongitudeFrom.HasValue ? booking.LongitudeFrom.Value : 0,
                       AddrTo = booking.AddrTo,
                       latitudeTo = booking.LatitudeTo.HasValue ? booking.LatitudeTo.Value : 0,
                       longitudeTo = booking.LongitudeTo.HasValue ? booking.LongitudeTo.Value : 0,
                       ComputedDistance = booking.ComputedDistance,
                       EstimatedPrice = booking.EstimatedPrice,
                       Active = booking.Active,
                       Confirmed = booking.Confirmed,
                       TaxiId = booking.TaxiId.HasValue ? booking.TaxiId.Value : 0,
                       LastModified = booking.LastModified,
                       Created = booking.Created,
                       SelectedTaxi = new Taxi
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
                       }
                   };
        }
    }

    [CollectionDataContract(Namespace = "http://cabme.co.za/bookings")]
    public class Bookings : List<Booking>
    {
        public Bookings()
        {
        }

        public Bookings(List<Booking> bookings)
            : base(bookings)
        {
        }
    }
}

