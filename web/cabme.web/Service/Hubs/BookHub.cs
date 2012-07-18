using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;

namespace cabme.web.Service.Hubs
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

        public void Ping(string number, string message)
        {
            SendClientMessage(number, message);
        }
    }
}