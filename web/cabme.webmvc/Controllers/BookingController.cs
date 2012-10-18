﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Data = cabme.data;
using cabme.webmvc.Models;
using System.Threading.Tasks;

namespace cabme.webmvc.Controllers
{
    public class BookingController : ApiController
    {
        // GET api/confirmation
        public IEnumerable<Booking> Get()
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return AllQueryableBookings(context).ToList();
            }
        }

        // GET api/confirmation/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/confirmation
        public Booking Post(Booking booking)
        {
            /*if (booking == null || booking.NumberOfPeople <= 0 || string.IsNullOrEmpty(booking.AddrFrom) || //string.IsNullOrEmpty(booking.AddrTo) ||
                booking.TaxiId <= 0 || string.IsNullOrEmpty(booking.PhoneNumber))
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }*/
            //BookHub.SendClientMessage(booking.PhoneNumber, "Booking has been received by server.");
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
                        /*WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                        return null;*/
                    }
                }
                if (booking.SelectedTaxi == null)
                {
                    booking.SelectedTaxi = new TaxiController().Get(booking.TaxiId);
                }
                if (booking.EstimatedPrice == 0 && booking.ComputedDistance > 0)
                {
                    booking.EstimatedPrice = Taxi.GetPriceEstimate(booking.ComputedDistance, booking.SelectedTaxi.RatePerKm, booking.SelectedTaxi.MinRate, booking.SelectedTaxi.Units);
                }
                dataBooking.EstimatedPrice = booking.EstimatedPrice;
                
                if (booking.PickupTime.CompareTo(DateTime.MinValue) == 0)
                {
                    booking.PickupTime = DateTime.Now.AddMinutes(1);
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
                dataBooking.PickupTime = booking.PickupTime;
                dataBooking.AddrFrom = booking.AddrFrom;
                dataBooking.LatitudeFrom = booking.latitudeFrom;
                dataBooking.LongitudeFrom = booking.longitudeFrom;
                dataBooking.AddrTo = booking.AddrTo;
                dataBooking.LatitudeTo = booking.latitudeTo;
                dataBooking.LongitudeTo = booking.longitudeTo;
                dataBooking.ComputedDistance = booking.ComputedDistance;
                dataBooking.Active = booking.Active;
                dataBooking.Confirmed = booking.Confirmed;
                dataBooking.Accepted = booking.Accepted;
                dataBooking.WaitingTime = 0;
                dataBooking.TaxiId = booking.TaxiId;
                dataBooking.LastModified = DateTime.Now;
                dataBooking.Hash = "1";//Account.Hash.HashPassword(booking.PickupTime + booking.PhoneNumber);
                dataBooking.SuburbFromId = booking.SuburbFromId > 0 ? (int?)booking.SuburbFromId : null;
                if (dataBooking.Id == 0)
                {
                    context.Bookings.InsertOnSubmit(dataBooking);
                }
                //Store booking to database
                context.SubmitChanges();
                booking.Id = dataBooking.Id;

                //BookHub.SendClientMessage(booking.PhoneNumber, "Booking is being sent to " + booking.SelectedTaxi.Name + ".");
#if DEBUG
                string url = "http://www.cabme.co.za/confirm.aspx?hash=" + dataBooking.Hash;
#else
                string url = "http://" + OperationContext.Current.RequestContext.RequestMessage.Headers.To.Host + "/confirm.aspx?hash=" + dataBooking.Hash;
#endif

                //Load contact details for taxi
                var contactDetails = context.ContactDetails.Where(p => p.TaxiId == booking.TaxiId).SingleOrDefault();
                Task t = new Task(() =>
                {
                    //Is the contact details valid
                    if (contactDetails != null && (
                        (contactDetails.UseEmail && !string.IsNullOrEmpty(contactDetails.BookingEmail)) ||
                        (!contactDetails.UseEmail && !string.IsNullOrEmpty(contactDetails.BookingSMS))))
                    {
                        if (contactDetails.UseEmail && !string.IsNullOrEmpty(contactDetails.BookingEmail))
                        {
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
                            /*Mail.SendMail(contactDetails.BookingEmail, "cabme@abrie.net", "Test booking email", mailBody);
                            TaxiHub.SendTaxiPendingBooking(booking.SelectedTaxi.Id);
                            BookHub.SendClientMessage(booking.PhoneNumber, "Waiting for " + booking.SelectedTaxi.Name + " to confirm.");*/
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
                });
                try
                {
                    t.Start();
                }
                catch (AggregateException aggEx)
                {
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(aggEx));
                }
            }
            return booking;
        }

        // PUT api/confirmation/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/confirmation/5
        public void Delete(int id)
        {
        }
        /*
        public static IEnumerable<Booking> GetAllBookingsByNumber(string userName, bool? active, bool? confirmed, bool? open)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var number = (from user in context.Users
                              where user.Name == userName && user.PhoneNumber != null
                              select user.PhoneNumber).SingleOrDefault();
                if (!string.IsNullOrEmpty(number))
                {
                    if (active.HasValue)
                    {
                        if (confirmed.HasValue && confirmed.Value)
                        {
                            return new Bookings(AllQueryableBookings(context).Where(p => p.PhoneNumber == number && p.Active == active && p.Confirmed).OrderBy(p => p.LastModified).ToList());
                        }
                        else
                        {
                            if (open.HasValue && open.Value)
                            {
                                return new Bookings(AllQueryableBookings(context).Where(p => p.PhoneNumber == number && p.Active == active && !p.Confirmed && p.dPickupTime.AddMinutes(30) > DateTime.Now).OrderBy(p => p.LastModified).ToList());
                            }
                            else
                            {
                                return new Bookings(AllQueryableBookings(context).Where(p => p.PhoneNumber == number && p.Active == active && !p.Confirmed && p.dPickupTime.AddMinutes(30) < DateTime.Now).OrderBy(p => p.LastModified).ToList());
                            }
                        }
                    }
                    else
                    {
                        return new Bookings(AllQueryableBookings(context).Where(p => p.PhoneNumber == number).ToList());
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public static IEnumerable<Booking> GetAllTaxiBookingsForUser(string userName, bool? confirmed, bool open, int afterId)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var id = (from user in context.Users
                          join userTaxi in context.UserTaxis on user.Id equals userTaxi.UserId
                          where user.Name == userName
                          select userTaxi.TaxiId).SingleOrDefault();
                if (id > 0)
                {
                    if (confirmed.HasValue)
                    {
                        if (confirmed.Value)
                        {
                            return new Bookings(AllQueryableBookings(context).Where(p => p.TaxiId == id && p.Active && p.Confirmed && p.Id > afterId).OrderBy(p => p.LastModified).ToList());
                        }
                        else
                        {
                            if (open)
                            {
                                return new Bookings(AllQueryableBookings(context).Where(p => p.TaxiId == id && p.Active && !p.Confirmed && p.Id > afterId && p.dPickupTime.AddMinutes(30) > DateTime.Now).OrderBy(p => p.LastModified).ToList());
                            }
                            else
                            {
                                return new Bookings(AllQueryableBookings(context).Where(p => p.TaxiId == id && p.Active && !p.Confirmed && p.Id > afterId && p.dPickupTime.AddMinutes(30) < DateTime.Now).OrderBy(p => p.LastModified).ToList());
                            }
                        }
                    }
                    else
                    {
                        if (open)
                        {
                            return new Bookings(AllQueryableBookings(context).Where(p => p.TaxiId == id && p.Active && p.Id > afterId && p.dPickupTime.AddMinutes(30) > DateTime.Now).OrderBy(p => p.LastModified).ToList());
                        }
                        else
                        {
                            return new Bookings(AllQueryableBookings(context).Where(p => p.TaxiId == id && p.Active && p.Id > afterId && p.dPickupTime.AddMinutes(30) < DateTime.Now).OrderBy(p => p.LastModified).ToList());
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public static IEnumerable<Booking> GetAllActiveBookingsForUser(string userName)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var number = (from user in context.Users
                              where user.Name == userName && user.PhoneNumber != null
                              select user.PhoneNumber).SingleOrDefault();
                if (!string.IsNullOrEmpty(number))
                {
                    return GetAllBookingsByNumber(number, true, null, null);
                }
                else
                {
                    return null;
                }
            }
        }*/

        //// GET api/confirmation/?hash=
        public static Booking GetBookingByHash(string hash)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var booking = AllQueryableBookings(context).Where(p => p.Hash == hash).SingleOrDefault();
                return booking;
            }
        }

        public static Booking Confirm(string hash, string referenceCode, int waitingTime)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var booking = AllQueryableBookings(context).Where(p => p.Hash == hash && !p.Confirmed && p.PickupTime.AddMinutes(30) > DateTime.Now).SingleOrDefault();
                if (booking != null)
                {
                    var dbBooking = context.Bookings.Where(p => p.Id == booking.Id).SingleOrDefault();
                    booking.Confirmed = true;
                    booking.ReferenceCode = referenceCode;
                    booking.WaitingTime = waitingTime;
                    dbBooking.Confirmed = true;
                    dbBooking.ReferenceCode = referenceCode;
                    dbBooking.WaitingTime = waitingTime;
                    context.SubmitChanges();
                }
                return booking;
            }
        }
        /*
        public static Booking AcceptBooking(int id)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var dbBooking = context.Bookings.Where(p => p.Id == id).SingleOrDefault();
                if (dbBooking != null)
                {
                    dbBooking.Accepted = true;
                    context.SubmitChanges();
                }
                var booking = AllQueryableBookings(context).Where(p => p.Id == id).SingleOrDefault();
                return booking;
            }
        }

        public static Booking CancelBooking(int id)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var dbBooking = context.Bookings.Where(p => p.Id == id).SingleOrDefault();
                if (dbBooking != null)
                {
                    dbBooking.Accepted = false;
                    dbBooking.Confirmed = false;
                    dbBooking.Active = false;
                    context.SubmitChanges();
                }
                var booking = AllQueryableBookings(context).Where(p => p.Id == id).SingleOrDefault();
                return booking;
            }
        }*/

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
                       PickupTime = booking.PickupTime,
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
                       Accepted = booking.Accepted,
                       WaitingTime = booking.WaitingTime,
                       TaxiId = booking.TaxiId.HasValue ? booking.TaxiId.Value : 0,
                       LastModified = booking.LastModified,
                       Created = booking.Created,
                       Hash = booking.Hash,
                       ReferenceCode = booking.ReferenceCode,
                       SelectedTaxi = new Taxi
                       {
                           Id = taxi.Id,
                           Name = taxi.Name,
                           PhoneNumber = taxi.PhoneNumber,
                           RatePerKm = taxi.RatePerKm,
                           MinRate = taxi.MinRate,
                           Units = taxi.Units,
                           StartOfService = taxi.StartOfService,
                           EndOfService = taxi.EndOfService,
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
}