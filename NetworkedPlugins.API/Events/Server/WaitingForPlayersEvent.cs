using NetworkedPlugins.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Events.Server
{
    public class WaitingForPlayersEvent : EventArgs
    {
        public WaitingForPlayersEvent(NPServer server)
        {
            Server = server;
        }

        public NPServer Server { get; }
    }
}
