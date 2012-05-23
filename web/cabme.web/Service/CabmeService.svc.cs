using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using cabme.web.Service.Entities;

namespace cabme.web.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CabmeService : ICabmeService
    {       
        public Taxis GetAllTaxis()
        {           
            return Taxi.GetAllTaxis();
        }
		
		public Booking MakeBooking(Booking booking){
			return Booking.MakeBooking(booking);
		}
		
		public Bookings GetAllBookingsForNumber(string number){
			return Booking.GetAllBookingsByNumber(number);
		}
    }
}
