﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;

namespace cabme.web.Service.Hubs
{
    [HubName("taxiHub")]
    public class TaxiHub : Hub
    {
        public static Dictionary<int, Dictionary<string, string>> Connections = new Dictionary<int, Dictionary<string, string>>();

        public void Announce(string userName)
        {
            try
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    int taxiId = Entities.Taxi.GetTaxiIdForUser(userName);
                    if (taxiId > 0)
                    {
                        if (!Connections.ContainsKey(taxiId) || Connections[taxiId] == null)
                        {
                            Connections[taxiId] = new Dictionary<string, string>();
                        }
                        Connections[taxiId][userName] = Context.ConnectionId;
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
                if (Connections.ContainsKey(taxiId))
                {
                    Dictionary<string, string> users = Connections[taxiId];
                    if (users != null)
                    {
                        foreach (string id in users.Values)
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                hubContext.Clients[id].pendingBooking();
                            }
                        }
                    }
                }
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
                if (Connections.ContainsKey(taxiId))
                {
                    Dictionary<string, string> users = Connections[taxiId];
                    if (users != null)
                    {
                        foreach (string id in users.Values)
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                hubContext.Clients[id].acceptedBooking(bookingId);
                            }
                        }
                    }
                }
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
                if (Connections.ContainsKey(taxiId))
                {
                    Dictionary<string, string> users = Connections[taxiId];
                    if (users != null)
                    {
                        foreach (string id in users.Values)
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                hubContext.Clients[id].cancelBooking(bookingId);
                            }
                        }
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