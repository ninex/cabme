using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using cabme.web.Service.Entities;

namespace cabme.web.Service
{

    [ServiceContract(Namespace = "http://cabme.co.za")]
    public interface ICabmeService
    {
        #region Taxis

        [WebInvoke(Method = "GET",
           UriTemplate = "taxis?distance={distance}",
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        Taxis GetAllTaxis(string distance);

        #endregion

        #region Bookings

        [WebInvoke(Method = "GET",
           UriTemplate = "taxibookings?name={name}&confirmed={confirmed}&open={open}&after={after}",
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        Bookings GetAllTaxiBookings(string name, string confirmed, string open, string after);

        [WebInvoke(Method = "GET",
           UriTemplate = "bookings?number={number}&active={active}",
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        Bookings GetAllBookingsForNumber(string number, string active);

        [WebInvoke(Method = "GET",
           UriTemplate = "userbookings?user={user}&confirmed={confirmed}&open={open}",
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        Bookings GetAllUserBookings(string user, string confirmed, string open);

        [WebInvoke(Method = "POST",
          UriTemplate = "booking",
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        Booking MakeBooking(Booking booking);

        [WebInvoke(Method = "POST",
          UriTemplate = "confirmbooking",
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void ConfirmBooking(Confirmation confirmation);

        #endregion

        #region Suburbs

        [WebInvoke(Method = "GET",
           UriTemplate = "suburbs?city={city}",
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        Suburbs GetAllSuburbsForCity(string city);

        #endregion
    }
}