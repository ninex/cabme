using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using cabme.web.Service.Entities;
using System.Net;
using System.Web;

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
                    return Booking.GetAllBookingsByNumber(number, bActive);
                }
                else
                {
                    return Booking.GetAllBookingsByNumber(number, null);
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

        #endregion
    }
}
