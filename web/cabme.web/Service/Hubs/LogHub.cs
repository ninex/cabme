using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;

namespace cabme.web.Service.Hubs
{
    [HubName("logHub")]
    public class LogHub : Hub
    {
        public void Log(string message)
        {
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("Connection:" + Context.ConnectionId + "\r\nHeaders:" + Context.Headers + "\r\nMsg:" + message)));
        }
    }
}