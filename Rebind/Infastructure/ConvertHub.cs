using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using SignalR.Hubs;

namespace Rebind.Infastructure
{
    [HubName("convertHub")]
    public class ConvertHub : Hub
    {
        public Task Join(String subscriptionId)
        {
            return Groups.Add(Context.ConnectionId, subscriptionId);
        }


        public Task Disconnect()
        {
            return Clients.leave(Context.ConnectionId);
        }
    }
}