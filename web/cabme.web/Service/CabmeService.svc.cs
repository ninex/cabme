using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using cabme.web.Service.Entities;

namespace cabme.web.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CabmeService : ICabmeService
    {
        #region Taxis

        public Taxis GetAllTaxis(string distance)
        {
            try
            {
                if (string.IsNullOrEmpty(distance))
                {
                    return Taxi.GetAllTaxis();
                }
                else
                {
                    int dist = 0;
                    if (int.TryParse(distance, out dist))
                    {
                        return Taxi.GetAllTaxis(dist);
                    }
                    else
                    {
                        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                return null;
            }
        }

        #endregion

        #region Bookings

        public Bookings GetAllTaxiBookings(string name, string confirmed, string open, string after)
        {
            try
            {
                if (String.IsNullOrEmpty(name))
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                int afterId = 0;
                if (!String.IsNullOrEmpty(after))
                {
                    int.TryParse(after, out afterId);
                }
                bool bConfirmed = false, bOpen = false;
                bool.TryParse(open, out bOpen);
                if (bool.TryParse(confirmed, out bConfirmed))
                {
                    return Booking.GetAllTaxiBookingsForUser(name, bConfirmed, bOpen, afterId);
                }
                else
                {
                    return Booking.GetAllTaxiBookingsForUser(name, null, bOpen, afterId);
                }
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                return null;
            }
        }

        public Bookings GetAllUserBookings(string userName, string confirmed, string open)
        {
            try
            {
                if (String.IsNullOrEmpty(userName))
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                bool bConfirmed = false, bOpen = false;
                if (bool.TryParse(confirmed, out bConfirmed))
                {
                    bool.TryParse(open, out bOpen);
                    return Booking.GetAllBookingsByNumber(userName, true, bConfirmed, bOpen);
                }
                return null;
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                return null;
            }
        }

        public Booking MakeBooking(Booking booking)
        {
            try
            {
                return Booking.MakeBooking(booking);
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                return null;
            }
        }

        public Bookings GetAllBookingsForNumber(string number, string active)
        {
            try
            {
                if (String.IsNullOrEmpty(number))
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                bool bActive = false;
                if (bool.TryParse(active, out bActive))
                {
                    return Booking.GetAllBookingsByNumber(number, bActive, null, null);
                }
                else
                {
                    return Booking.GetAllBookingsByNumber(number, null, null, null);
                }
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                return null;
            }
        }

        public Bookings GetAllBookings()
        {
            try
            {
                return Booking.GetAllBookings();
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                return null;
            }
        }

        public void ConfirmBooking(Confirmation confirmation)
        {
            try
            {
                int minutes = 0;
                if (confirmation != null)
                {
                    int.TryParse(confirmation.Arrival, out minutes);
                    Booking.Confirm(confirmation.Hash, confirmation.RefCode, minutes);
                    if (!string.IsNullOrEmpty(confirmation.PhoneNumber))
                    {
                        cabme.web.Service.Hubs.BookHub.ConfirmBooking(confirmation.PhoneNumber, minutes);
                    }
                }
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
        }

        #endregion

        #region Suburbs

        public Suburbs GetAllSuburbsForCity(string city)
        {
            try
            {
                if (String.IsNullOrEmpty(city))
                {
                    return Suburb.GetAllSuburbs();
                }
                else
                {
                    return Suburb.GetAllByCity(city);
                }
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                return null;
            }
        }

        #endregion

        #region Invoices

        public Invoice GetInvoice(string name, string month, string year)
        {
            try
            {
                int iMonth = 0, iYear = 0;
                if (String.IsNullOrEmpty(name) || !int.TryParse(month, out iMonth) || !int.TryParse(year, out iYear))
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                return Invoice.GetInvoice(name, iMonth, iYear);
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                return null;
            }
        }

        #endregion
    }
}
