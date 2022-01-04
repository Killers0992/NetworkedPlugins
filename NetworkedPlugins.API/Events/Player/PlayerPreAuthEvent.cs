using NetworkedPlugins.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Events.Player
{
    public class PlayerPreAuthEvent : EventArgs
    {
        public PlayerPreAuthEvent(NPServer server, string userid, string country, byte flags)
        {
            this.Server = server;
            this.UserID = userid;
            this.Country = country;
            this.Flags = flags;
        }

        public NPServer Server { get; }
        public string UserID { get; }
        public string Country { get; }
        public byte Flags { get; }
    }
}
