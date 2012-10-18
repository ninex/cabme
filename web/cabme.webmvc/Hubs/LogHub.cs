using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;

namespace cabme.webmvc.Hubs
{
    [HubName("logHub")]
    public class LogHub : Hub
    {
        public void Log(string message)
        {
            try
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("Msg:" + message + "\r\nConnection:" + Context.ConnectionId + "\r\nHeaders:" + Context.Headers + "\r\n")));
            }
            catch { }
        }
    }
}