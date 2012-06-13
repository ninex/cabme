using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Data = cabme.data;
using System.ServiceModel.Web;
using System.Net;

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
            if (booking == null | booking.NumberOfPeople <= 0 || string.IsNullOrEmpty(booking.AddrFrom) || string.IsNullOrEmpty(booking.AddrTo) ||
                booking.TaxiId <= 0 || string.IsNullOrEmpty(booking.PhoneNumber))
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                Data.Booking dataBooking;
                // New booking or update
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
                    if (dataBooking == null || dataBooking.Confirmed)
                    {
                        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                        return null;
                    }
                }
                if (booking.EstimatedPrice == 0 && booking.ComputedDistance > 0)
                {
                    if (booking.SelectedTaxi == null)
                    {
                        booking.SelectedTaxi = Taxi.GetTaxi(booking.TaxiId);
                    }
                    dataBooking.EstimatedPrice = booking.SelectedTaxi.GetPriceEstimate(booking.ComputedDistance);
                }
                else
                {
                    dataBooking.EstimatedPrice = booking.EstimatedPrice;
                }
                dataBooking.Name = booking.Name;
                dataBooking.PhoneNumber = booking.PhoneNumber;
                dataBooking.NumberOfPeople = booking.NumberOfPeople;
                dataBooking.PickupTime = DateTime.ParseExact(booking.PickupTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                dataBooking.AddrFrom = booking.AddrFrom;
                dataBooking.LatitudeFrom = booking.latitudeFrom;
                dataBooking.LongitudeFrom = booking.longitudeFrom;
                dataBooking.AddrTo = booking.AddrTo;
                dataBooking.LatitudeTo = booking.latitudeTo;
                dataBooking.LongitudeTo = booking.longitudeTo;
                dataBooking.ComputedDistance = booking.ComputedDistance;
                dataBooking.Active = booking.Active;
                dataBooking.Confirmed = booking.Confirmed;
                dataBooking.TaxiId = booking.TaxiId;
                dataBooking.LastModified = DateTime.Now;
                if (dataBooking.Id == 0)
                {
                    context.Bookings.InsertOnSubmit(dataBooking);
                }
                //Store booking to database
                context.SubmitChanges();
                booking.Id = dataBooking.Id;
                //Load contact details for taxi
                var contactDetails = context.ContactDetails.Where(p => p.TaxiId == booking.TaxiId).SingleOrDefault();
                //Is the contact details valid
                if (contactDetails != null && (
                    (contactDetails.UseEmail && !string.IsNullOrEmpty(contactDetails.BookingEmail)) ||
                    (!contactDetails.UseEmail && !string.IsNullOrEmpty(contactDetails.BookingSMS))))
                {
                    if (contactDetails.UseEmail && !string.IsNullOrEmpty(contactDetails.BookingEmail))
                    {
                        string mailBody = string.Format("Booking received from {0} for {4} people pickup time: {1}\r\nFrom:{2}\r\nTo:{3}\r\n", booking.PhoneNumber, booking.PickupTime, booking.AddrFrom, booking.AddrTo, booking.NumberOfPeople);
                        //Send confirm booking email
                        Mail.SendMail(contactDetails.BookingEmail, "cabme@abrie.net", "Test booking email", mailBody);
                    }
                    else
                    {
                        //TODO: SMS service
                    }
                }
                else
                {
                    //TODO: Problem that has to be addressed
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("No contact details found for taxi:" + booking.TaxiId)));
                }
            }

            return booking;
        }

        public static Bookings GetAllBookings()
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return new Bookings(AllQueryableBookings(context).ToList());
            }
        }

        public static Bookings GetAllBookingsByNumber(string number, bool? active)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                if (active.HasValue)
                {
                    return new Bookings(AllQueryableBookings(context).Where(p => p.PhoneNumber == number && p.Active == active).ToList());
                }
                else
                {
                    return new Bookings(AllQueryableBookings(context).Where(p => p.PhoneNumber == number).ToList());
                }
            }
        }

        private static IQueryable<Booking> AllQueryableBookings(Data.contentDataContext context)
        {
            return from booking in context.Bookings
                   join taxi in context.Taxis on booking.TaxiId equals taxi.Id into outer
                   from taxi in outer.DefaultIfEmpty()
                   orderby booking.LastModified descending
                   select new Booking
                   {
                       Id = booking.Id,
                       Name = booking.Name,
                       PhoneNumber = booking.PhoneNumber,
                       NumberOfPeople = booking.NumberOfPeople,
                       dPickupTime = booking.PickupTime,
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

