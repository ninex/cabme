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
        public Taxis GetAllTaxis()
        {
            return Taxi.GetAllTaxis();
        }

        public void MakeBooking(Booking booking)
        {
            try
            {
                Booking.MakeBooking(booking);
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                return;
            }
        }

        public Bookings GetAllBookingsForNumber(string number)
        {
            return Booking.GetAllBookingsByNumber(number);
        }

        public Bookings GetAllBookings()
        {
            return Booking.GetAllBookings();
        }
    }
}
