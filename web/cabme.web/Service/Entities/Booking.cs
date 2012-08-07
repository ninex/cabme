using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Data = cabme.data;
using System.ServiceModel.Web;
using System.Net;
using System.Web;
using System.Web.Routing;
using System.ServiceModel;
using cabme.web.Service.Hubs;

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

        private DateTime _dPickupTime;
        public DateTime dPickupTime
        {
            get { return _dPickupTime; }
            set
            {
                PickupTime = value.ToString("yyyy-MM-dd hh:mm:ss");
                _dPickupTime = value;
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

        public string Hash { get; set; }

        [DataMember]
        public int SuburbFromId { get; set; }

        [DataMember]
        public Suburb SuburbFrom { get; set; }

        [DataMember]
        public Taxi SelectedTaxi { get; set; }

        public static Booking MakeBooking(Booking booking)
        {
            if (booking == null || booking.NumberOfPeople <= 0 || string.IsNullOrEmpty(booking.AddrFrom) || //string.IsNullOrEmpty(booking.AddrTo) ||
                booking.TaxiId <= 0 || string.IsNullOrEmpty(booking.PhoneNumber))
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }
            BookHub.SendClientMessage(booking.PhoneNumber, "Booking has been received by server.");
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
                if (booking.SelectedTaxi == null)
                {
                    booking.SelectedTaxi = Taxi.GetTaxi(booking.TaxiId);
                }
                if (booking.EstimatedPrice == 0 && booking.ComputedDistance > 0)
                {
                    dataBooking.EstimatedPrice = booking.SelectedTaxi.GetPriceEstimate(booking.ComputedDistance);
                }
                else
                {
                    dataBooking.EstimatedPrice = booking.EstimatedPrice;
                }

                if (string.IsNullOrEmpty(booking.PickupTime))
                {
                    booking.PickupTime = DateTime.Now.AddMinutes(1).ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (booking.SuburbFromId > 0)
                {
                    booking.SuburbFrom = context.Suburbs.Where(p => p.Id == booking.SuburbFromId).Select(p => new Suburb
                    {
                        Id = p.Id,
                        City = p.City,
                        Name = p.Name,
                        PostalCode = p.PostalCode
                    }).SingleOrDefault();
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
                dataBooking.Hash = Account.Hash.HashPassword(booking.PickupTime + booking.PhoneNumber);
                dataBooking.SuburbFromId = booking.SuburbFromId > 0 ? (int?)booking.SuburbFromId : null;
                if (dataBooking.Id == 0)
                {
                    context.Bookings.InsertOnSubmit(dataBooking);
                }
                //Store booking to database
                context.SubmitChanges();
                booking.Id = dataBooking.Id;

                BookHub.SendClientMessage(booking.PhoneNumber, "Booking is being sent to " + booking.SelectedTaxi.Name + ".");
#if DEBUG
                string url = "http://www.cabme.co.za/confirm.aspx?hash=" + dataBooking.Hash;
#else
                string url = "http://" + OperationContext.Current.RequestContext.RequestMessage.Headers.To.Host + "/confirm.aspx?hash=" + dataBooking.Hash;
#endif
                //Load contact details for taxi
                var contactDetails = context.ContactDetails.Where(p => p.TaxiId == booking.TaxiId).SingleOrDefault();
                //Is the contact details valid
                if (contactDetails != null && (
                    (contactDetails.UseEmail && !string.IsNullOrEmpty(contactDetails.BookingEmail)) ||
                    (!contactDetails.UseEmail && !string.IsNullOrEmpty(contactDetails.BookingSMS))))
                {
                    if (contactDetails.UseEmail && !string.IsNullOrEmpty(contactDetails.BookingEmail))
                    {

                        TaxiHub.SendTaxiPendingBooking(booking.SelectedTaxi.Id);
                        BookHub.SendClientMessage(booking.PhoneNumber, "Waiting for " + booking.SelectedTaxi.Name + " to confirm.");
                        string mailBody;
                        if (booking.SuburbFrom != null)
                        {
                            mailBody = string.Format("Booking received from " + booking.SuburbFrom.Name + "<br/><a href=\"{0}\">Click here to confirm</a>", url);
                        }
                        else
                        {
                            mailBody = string.Format("Booking received.<br/><a href=\"{0}\">Click here to confirm</a>", url);
                        }
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

        public static Bookings GetAllTaxiBookingsForUser(string userName)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var id = (from user in context.Users
                          join userTaxi in context.UserTaxis on user.Id equals userTaxi.UserId
                          where user.Name == userName
                          select userTaxi.TaxiId).SingleOrDefault();
                if (id > 0)
                {
                    return new Bookings(AllQueryableBookings(context).Where(p => p.TaxiId == id && p.Active).ToList());
                }
                else
                {
                    return null;
                }
            }
        }
        public static Bookings GetAllActiveBookingsForUser(string userName)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var number = (from user in context.Users
                              where user.Name == userName && user.PhoneNumber != null
                              select user.PhoneNumber).SingleOrDefault();
                if (!string.IsNullOrEmpty(number))
                {
                    return GetAllBookingsByNumber(number, true);
                }
                else
                {
                    return null;
                }
            }
        }
        public static Booking GetBookingByHash(string hash)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var booking = AllQueryableBookings(context).Where(p => p.Hash == hash).SingleOrDefault();
                return booking;
            }
        }

        public static Booking Confirm(string hash)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var booking = AllQueryableBookings(context).Where(p => p.Hash == hash && !p.Confirmed && p.dPickupTime.AddMinutes(30) > DateTime.Now).SingleOrDefault();
                if (booking != null)
                {
                    var dbBooking = context.Bookings.Where(p => p.Id == booking.Id).SingleOrDefault();
                    booking.Confirmed = true;
                    dbBooking.Confirmed = true;
                    context.SubmitChanges();
                }
                return booking;
            }
        }

        private static IQueryable<Booking> AllQueryableBookings(Data.contentDataContext context)
        {
            return from booking in context.Bookings
                   join taxi in context.Taxis on booking.TaxiId equals taxi.Id into outer
                   join suburb in context.Suburbs on booking.SuburbFromId equals suburb.Id into outerSuburb
                   from taxi in outer.DefaultIfEmpty()
                   from suburb in outerSuburb.DefaultIfEmpty()
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
                       Hash = booking.Hash,
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
                       },
                       SuburbFromId = booking.SuburbFromId.HasValue ? booking.SuburbFromId.Value : 0,
                       SuburbFrom = new Suburb
                       {
                           City = suburb.City,
                           Name = suburb.Name,
                           PostalCode = suburb.PostalCode
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

