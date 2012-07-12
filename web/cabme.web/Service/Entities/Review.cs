using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Data = cabme.data;
using System.ServiceModel.Web;
using System.Net;

namespace cabme.web.Service.Entities
{
    [DataContract(Namespace = "http://cabme.co.za/review")]
    public class Review
    {
        [DataMember]
        public int Id { get; set; }


        [DataMember]
        public string UserName { get; set; }
        public int UserId { get; set; }

        [DataMember]
        public int BookingId { get; set; }

        [DataMember]
        public int TaxiId { get; set; }

        [DataMember]
        public byte Rating { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public string DateCreated { get; set; }

        private DateTime _dDateCreated;
        public DateTime dDateCreated
        {
            get { return _dDateCreated; }
            set
            {
                DateCreated = value.ToString("yyyy-MM-dd hh:mm:ss");
                _dDateCreated = value;
            }
        }

        [DataMember]
        public string DateLastModified { get; set; }

        private DateTime _dDateLastModified;
        public DateTime dDateLastModified
        {
            get { return _dDateLastModified; }
            set
            {
                DateLastModified = value.ToString("yyyy-MM-dd hh:mm:ss");
                _dDateLastModified = value;
            }
        }

        [DataMember]
        public bool Active { get; set; }

        public Taxi ReviewedTaxi { get; set; }

        public Booking ReviewedBooking { get; set; }

        public static Review MakeReview(Review review)
        {
            if (review == null || string.IsNullOrEmpty(review.UserName) || review.TaxiId <= 0 || review.BookingId <= 0 || review.Rating < 0 || string.IsNullOrEmpty(review.Comment))
            {
                if (WebOperationContext.Current != null)
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                }
                return null;
            }
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                review.UserId = context.Users.Where(p => p.Name == review.UserName).Select(p => p.Id).SingleOrDefault();
                if (review.UserId <= 0)
                {
                    if (WebOperationContext.Current != null)
                    {
                        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    }
                    return null;
                }
                Data.Review dataReview;
                DateTime timestamp = DateTime.Now;
                // New review or update
                if (review.Id == 0)
                {
                    dataReview = new Data.Review()
                    {
                        Id = 0,
                        DateCreated = timestamp,
                        DateLastModified = timestamp,
                        UserId = review.UserId,
                        BookingId = review.BookingId,
                        TaxiId = review.TaxiId
                    };
                }
                else
                {
                    dataReview = context.Reviews.Where(p => p.Id == review.Id).SingleOrDefault();
                    if (dataReview == null || !dataReview.Active)
                    {
                        if (WebOperationContext.Current != null)
                        {
                            WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                        }
                        return null;
                    }
                    dataReview.DateLastModified = timestamp;
                }
                dataReview.Rating = review.Rating;
                dataReview.Comment = review.Comment;
                dataReview.Active = true;
                if (dataReview.Id == 0)
                {
                    context.Reviews.InsertOnSubmit(dataReview);
                }
                //Store review to database
                context.SubmitChanges();
                review.Id = dataReview.Id;
            }

            return review;
        }

        public static Reviews GetAllReviews()
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return new Reviews(AllQueryableReviews(context).ToList());
            }
        }

        public static Review GetReview(string userName, int bookingId, int taxiId)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return AllQueryableReviews(context).Where(p => p.UserName == userName && p.BookingId == bookingId && p.TaxiId == taxiId).SingleOrDefault();
            }
        }

        public static Reviews GetAllUserReviews(string userName)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                return new Reviews(AllQueryableReviews(context).Where(p => p.UserName == userName && p.Active).ToList());
            }
        }

        public static Reviews GetAllTaxiReviewsForUser(string userName)
        {
            using (Data.contentDataContext context = new Data.contentDataContext())
            {
                var id = (from user in context.Users
                          join userTaxi in context.UserTaxis on user.Id equals userTaxi.UserId
                          where user.Name == userName
                          select userTaxi.TaxiId).SingleOrDefault();
                if (id > 0)
                {
                    return new Reviews(AllQueryableReviews(context).Where(p => p.TaxiId == id && p.Active).ToList());
                }
                else
                {
                    return null;
                }
            }
        }

        private static IQueryable<Review> AllQueryableReviews(Data.contentDataContext context)
        {
            return from review in context.Reviews
                   join taxi in context.Taxis on review.TaxiId equals taxi.Id
                   join user in context.Users on review.UserId equals user.Id
                   join booking in context.Bookings on review.BookingId equals booking.Id
                   orderby review.DateLastModified descending
                   select new Review
                   {
                       Id = review.Id,
                       UserId = review.UserId,
                       UserName = user.Name,
                       BookingId = review.BookingId,
                       TaxiId = review.TaxiId,
                       Rating = review.Rating,
                       Comment = review.Comment,
                       dDateCreated = review.DateCreated,
                       dDateLastModified = review.DateLastModified,
                       Active = review.Active,
                       ReviewedTaxi = new Taxi
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
                       ReviewedBooking = new Booking
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
                       }
                   };
        }

    }
    [CollectionDataContract(Namespace = "http://cabme.co.za/reviews")]
    public class Reviews : List<Review>
    {
        public Reviews()
        {
        }

        public Reviews(List<Review> reviews)
            : base(reviews)
        {
        }
    }
}