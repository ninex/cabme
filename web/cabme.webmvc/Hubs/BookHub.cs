using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;

namespace cabme.webmvc.Hubs
{
    [HubName("bookHub")]
    public class BookHub : Hub
    {
        public static Dictionary<string, string> Connections = new Dictionary<string, string>();

        public void Announce(string number)
        {
            if (!string.IsNullOrEmpty(number))
            {
                Connections[number] = Context.ConnectionId;
            }
        }

        public static void SendClientMessage(string number, string message)
        {
            try
            {
                var hubContext = SignalR.GlobalHost.ConnectionManager.GetHubContext<BookHub>();
                if (Connections.ContainsKey(number))
                {
                    string id = Connections[number];
                    if (!string.IsNullOrEmpty(id))
                    {
                        hubContext.Clients[id].showMessage(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
        }

        public static void ConfirmBooking(string number, int waitingTime)
        {
            try
            {
                var hubContext = SignalR.GlobalHost.ConnectionManager.GetHubContext<BookHub>();
                if (Connections.ContainsKey(number))
                {
                    string id = Connections[number];
                    if (!string.IsNullOrEmpty(id))
                    {
                        hubContext.Clients[id].confirmBooking(waitingTime);
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
        }
        public static void CancelBooking(string number)
        {
            try
            {
                var hubContext = SignalR.GlobalHost.ConnectionManager.GetHubContext<BookHub>();
                if (Connections.ContainsKey(number))
                {
                    string id = Connections[number];
                    if (!string.IsNullOrEmpty(id))
                    {
                        hubContext.Clients[id].cancelBooking();
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
        }
    }
}