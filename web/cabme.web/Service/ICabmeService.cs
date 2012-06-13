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
           UriTemplate = "bookings?number={number}&active={active}",
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        Bookings GetAllBookingsForNumber(string number, string active);

        [WebInvoke(Method = "POST",
          UriTemplate = "booking",
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        Booking MakeBooking(Booking booking);

        #endregion
    }
}