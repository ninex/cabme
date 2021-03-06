﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using cabme.webmvc.Common;
using cabme.webmvc.Hubs;
using cabme.webmvc.Models;
using Data = cabme.data;

namespace cabme.webmvc.Controllers
{
    public class BookingController : ApiController
    {
        private Data.Interfaces.IRepository<Data.Booking> bookingRepository;
        private Data.Interfaces.IRepository<Data.Suburb> suburbRepository;
        private Data.Interfaces.IRepository<Data.Taxi> taxiRepository;
        private Data.Interfaces.IRepository<Data.User> userRepository;
        private Data.Interfaces.IRepository<Data.UserTaxi> userTaxiRepository;
        private Data.Interfaces.IRepository<Data.ContactDetail> contactDetailRepository;

        //Default constructor initialises with a database context
        public BookingController()
            : this(new Data.Repositories.Repository<Data.Booking>(), new Data.Repositories.Repository<Data.Suburb>(), new Data.Repositories.Repository<Data.Taxi>(),
            new Data.Repositories.Repository<Data.User>(), new Data.Repositories.Repository<Data.UserTaxi>(), new Data.Repositories.Repository<Data.ContactDetail>()) { }

        public BookingController(Data.Interfaces.IRepository<Data.Booking> bookingRepository, Data.Interfaces.IRepository<Data.Suburb> suburbRepository,
            Data.Interfaces.IRepository<Data.Taxi> taxiRepository, Data.Interfaces.IRepository<Data.User> userRepository, Data.Interfaces.IRepository<Data.UserTaxi> userTaxiRepository,
            Data.Interfaces.IRepository<Data.ContactDetail> contactDetailRepository)
        {
            this.bookingRepository = bookingRepository;
            this.suburbRepository = suburbRepository;
            this.taxiRepository = taxiRepository;
            this.userRepository = userRepository;
            this.userTaxiRepository = userTaxiRepository;
            this.contactDetailRepository = contactDetailRepository;
        }
        // GET api/booking
        [Authorize(Roles = "Admin")]
        public IEnumerable<Booking> Get()
        {
            return AllQueryableBookings().ToList();
        }

        // POST api/booking
        public Booking Post(Booking booking)
        {
            if (booking == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            BookHub.SendClientMessage(booking.PhoneNumber, "Booking has been received by server.");
            Data.Booking dataBooking;
            // New booking or update
            if (booking.Id == 0)
            {
                dataBooking = bookingRepository.CreateInstance();
                dataBooking.Id = 0;
                dataBooking.Created = DateTime.Now;

                if (dataBooking.UserCancelled || dataBooking.UserAccepted || dataBooking.TaxiCancelled || dataBooking.TaxiAccepted)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                dataBooking = bookingRepository.FindAll(p => p.Id == booking.Id).SingleOrDefault();
                if (dataBooking == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
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
                booking.SuburbFrom = suburbRepository.FindAll(p => p.Id == booking.SuburbFromId).Select(p => new Suburb
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
            dataBooking.UserAccepted = booking.UserAccepted;
            dataBooking.UserCancelled = booking.UserCancelled;
            dataBooking.TaxiAccepted = booking.TaxiAccepted;
            dataBooking.TaxiCancelled = booking.TaxiCancelled;
            dataBooking.WaitingTime = 0;
            dataBooking.TaxiId = booking.TaxiId;
            dataBooking.LastModified = DateTime.Now;
            dataBooking.Hash = Hash.HashPassword(booking.PickupTime + booking.PhoneNumber);
            dataBooking.SuburbFromId = booking.SuburbFromId > 0 ? (int?)booking.SuburbFromId : null;

            //Store booking to database
            bookingRepository.SaveAll();
            booking.Id = dataBooking.Id;

            BookHub.SendClientMessage(booking.PhoneNumber, "Booking is being sent to " + booking.SelectedTaxi.Name + ".");

            string url = "http://www.cabme.co.za/viewbooking/";

            //Load contact details for taxi
            var contactDetails = contactDetailRepository.FindAll(p => p.TaxiId == booking.TaxiId).SingleOrDefault();
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
                        Mail.SendMail(contactDetails.BookingEmail, "cabme@abrie.net", "Test booking email", mailBody);
                        TaxiHub.SendTaxiPendingBooking(booking.SelectedTaxi.Id);
                        BookHub.SendClientMessage(booking.PhoneNumber, "Waiting for " + booking.SelectedTaxi.Name + " to confirm.");
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
            return booking;
        }

        // PUT api/booking/5
        [HttpPut]
        public Booking Put(int id, [FromBody]Booking booking)
        {

            if (booking.UserCancelled || booking.TaxiCancelled)
            {
                var dbBooking = bookingRepository.FindAll(p => p.Id == id).SingleOrDefault();
                if (dbBooking != null)
                {
                    dbBooking.UserCancelled = booking.UserCancelled;
                    dbBooking.TaxiCancelled = booking.TaxiCancelled;
                    dbBooking.Active = false;
                    bookingRepository.SaveAll();
                }
                booking = AllQueryableBookings().Where(p => p.Id == id).SingleOrDefault();

                if (booking != null)
                {
                    if (booking.TaxiCancelled)
                    {
                        if (!string.IsNullOrEmpty(booking.PhoneNumber))
                        {
                            BookHub.CancelBooking(booking.PhoneNumber);
                        }
                        TaxiHub.SendTaxiBookingCancelled(booking.SelectedTaxi.Id, id);
                    }
                    if (booking.UserCancelled)
                    {
                        TaxiHub.SendUserBookingCancelled(booking.SelectedTaxi.Id, id);
                    }
                }

                return booking;
            }
            else
            {
                if (booking.TaxiAccepted || booking.UserAccepted)
                {
                    var dbBooking = bookingRepository.FindAll(p => p.Id == id).SingleOrDefault();
                    if (dbBooking != null)
                    {

                        dbBooking.UserAccepted = booking.UserAccepted;
                        dbBooking.TaxiAccepted = booking.TaxiAccepted;
                        bookingRepository.SaveAll();
                    }
                    booking = AllQueryableBookings().Where(p => p.Id == id).SingleOrDefault();
                    if (booking.UserAccepted)
                    {
                        TaxiHub.SendUserBookingAccepted(booking.SelectedTaxi.Id, id);
                    }
                    if (booking.TaxiAccepted)
                    {
                        TaxiHub.SendTaxiBookingAccepted(booking.SelectedTaxi.Id, id);
                    }
                    return booking;
                }
            }
            return null;
        }

        //DELETE
        public void Delete(string id)
        {

        }

        // Get /api/booking/?userName={userName}&active={active}&open={open}&taxiAccepted={taxiAccepted}&userAccepted={userAccepted}&taxiCancelled={taxiCancelled}&userCancelled={userCancelled}&afterId={afterId}
        [Authorize]
        public IEnumerable<Booking> GetAllBookingsByNumber(string userName, bool? active = null, bool? open = null, bool? taxiAccepted = null, bool? userAccepted = null, bool? taxiCancelled = null, bool? userCancelled = null, int? afterId = 0)
        {
            if (!afterId.HasValue)
            {
                afterId = 0;
            }
            var id = (from user in userRepository.All()
                      join userTaxi in userTaxiRepository.All() on user.Id equals userTaxi.UserId
                      where user.Name == userName
                      select userTaxi.TaxiId).SingleOrDefault();

            IQueryable<Booking> bookings;

            if (open.HasValue)
            {
                if (open.Value)
                {
                    bookings = AllQueryableBookings().Where(p => p.Id > afterId && p.PickupTime.AddMinutes(30) > DateTime.Now).OrderBy(p => p.LastModified);
                }
                else
                {
                    bookings = AllQueryableBookings().Where(p => p.Id > afterId && p.PickupTime.AddMinutes(30) < DateTime.Now).OrderBy(p => p.LastModified);
                }
            }
            else
            {
                bookings = AllQueryableBookings().Where(p => p.Id > afterId).OrderBy(p => p.LastModified);
            }
            if (active.HasValue)
            {
                bookings = bookings.Where(p => p.Active == active);
            }

            if (taxiAccepted.HasValue)
            {
                bookings = bookings.Where(p => p.TaxiAccepted == taxiAccepted);
            }

            if (taxiCancelled.HasValue)
            {
                bookings = bookings.Where(p => p.TaxiCancelled == taxiCancelled);
            }

            if (userAccepted.HasValue)
            {
                bookings = bookings.Where(p => p.UserAccepted == userAccepted);
            }

            if (userCancelled.HasValue)
            {
                bookings = bookings.Where(p => p.UserCancelled == userCancelled);
            }

            if (id == 0)
            {
                var number = (from user in userRepository.All()
                              where user.Name == userName && user.PhoneNumber != null
                              select user.PhoneNumber).SingleOrDefault();
                if (string.IsNullOrEmpty(number))
                {
                    return null;
                }
                return bookings.Where(p => p.PhoneNumber == number).ToList();
            }
            else
            {
                return bookings.Where(p => p.TaxiId == id).ToList();
            }
        }

        //// GET api/booking/{hash}/?referenceCode={referenceCode}&waitingTime={waitingTime}
        [Authorize]
        public Booking Get(string id, string referenceCode = null, int waitingTime = -1)
        {
            string hash = id;
            if (referenceCode == null & waitingTime < 0)
            {
                return AllQueryableBookings().Where(p => p.Hash == hash).SingleOrDefault();
            }
            else
            {
                var booking = AllQueryableBookings().Where(p => p.Hash == hash && !p.TaxiAccepted && p.PickupTime.AddMinutes(30) > DateTime.Now).SingleOrDefault();
                if (booking != null)
                {
                    var dbBooking = bookingRepository.FindAll(p => p.Id == booking.Id).SingleOrDefault();
                    booking.TaxiAccepted = true;
                    booking.ReferenceCode = referenceCode;
                    booking.WaitingTime = waitingTime;
                    dbBooking.TaxiAccepted = true;
                    dbBooking.ReferenceCode = referenceCode;
                    dbBooking.WaitingTime = waitingTime;
                    bookingRepository.SaveAll();
                    if (!string.IsNullOrEmpty(booking.PhoneNumber))
                    {
                        BookHub.ConfirmBooking(booking.PhoneNumber, waitingTime);
                    }
                    TaxiHub.SendTaxiBookingAccepted(booking.SelectedTaxi.Id, booking.Id);
                }
                return booking;
            }
        }

        private IQueryable<Booking> AllQueryableBookings()
        {
            return (from booking in bookingRepository.All()
                    join taxi in taxiRepository.All() on booking.TaxiId equals taxi.Id into outer
                    join suburb in suburbRepository.All() on booking.SuburbFromId equals suburb.Id into outerSuburb
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
                        TaxiAccepted = booking.TaxiAccepted,
                        TaxiCancelled = booking.TaxiCancelled,
                        UserAccepted = booking.UserAccepted,
                        UserCancelled = booking.UserCancelled,
                        WaitingTime = booking.WaitingTime,
                        TaxiId = booking.TaxiId.HasValue ? booking.TaxiId.Value : 0,
                        LastModified = booking.LastModified,
                        Created = booking.Created,
                        Hash = booking.Hash,
                        ReferenceCode = booking.ReferenceCode,
                        SelectedTaxi = taxi == null ? null : new Taxi
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
                        SuburbFrom = suburb == null ? null : new Suburb
                        {
                            City = suburb.City,
                            Name = suburb.Name,
                            PostalCode = suburb.PostalCode
                        }
                    }).AsQueryable();
        }
    }
}
