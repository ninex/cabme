using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;

namespace cabme.webmvc.Hubs
{
    [HubName("taxiHub")]
    public class TaxiHub : Hub
    {
        private const string groupPreface = "#taxi";
        //public static Dictionary<int, Dictionary<string, string>> Connections = new Dictionary<int, Dictionary<string, string>>();

        public void Announce(string userName)
        {
            try
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    int taxiId = (new cabme.webmvc.Controllers.TaxiController()).GetTaxiIdForUser(userName);
                    if (taxiId > 0)
                    {
                        Groups.Add(Context.ConnectionId, groupPreface + taxiId);
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
        }
        public static void SendTaxiPendingBooking(int taxiId)
        {
            try
            {
                var hubContext = SignalR.GlobalHost.ConnectionManager.GetHubContext<TaxiHub>();
                hubContext.Clients[groupPreface + taxiId].pendingBooking();
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
        }

        public static void SendUserBookingAccepted(int taxiId, int bookingId)
        {
            try
            {
                var hubContext = SignalR.GlobalHost.ConnectionManager.GetHubContext<TaxiHub>();
                hubContext.Clients[groupPreface + taxiId].acceptedBooking(bookingId, true);
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
        }
        public static void SendTaxiBookingAccepted(int taxiId, int bookingId)
        {
            try
            {
                var hubContext = SignalR.GlobalHost.ConnectionManager.GetHubContext<TaxiHub>();
                hubContext.Clients[groupPreface + taxiId].acceptedBooking(bookingId, false);
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
        }
        public static void SendUserBookingCancelled(int taxiId, int bookingId)
        {
            try
            {
                var hubContext = SignalR.GlobalHost.ConnectionManager.GetHubContext<TaxiHub>();
                hubContext.Clients[groupPreface + taxiId].cancelBooking(bookingId, true);
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
        }
        public static void SendTaxiBookingCancelled(int taxiId, int bookingId)
        {
            try
            {
                var hubContext = SignalR.GlobalHost.ConnectionManager.GetHubContext<TaxiHub>();
                hubContext.Clients[groupPreface + taxiId].cancelBooking(bookingId, false);
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
        }
    }
}