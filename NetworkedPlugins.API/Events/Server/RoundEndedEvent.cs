using NetworkedPlugins.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Events.Server
{
    public class RoundEndedEvent : EventArgs
    {
        public RoundEndedEvent(NPServer server)
        {
            Server = server;
        }

        public NPServer Server { get; }
    }
}
