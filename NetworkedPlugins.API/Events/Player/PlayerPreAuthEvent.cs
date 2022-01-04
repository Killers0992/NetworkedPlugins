using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Events.Player
{
    public class PlayerPreAuthEvent : EventArgs
    {
        public PlayerPreAuthEvent(string userid, string country, byte flags)
        {
            this.UserID = userid;
            this.Country = country;
            this.Flags = flags;
        }

        public string UserID { get; set; }
        public string Country { get; set; }
        public byte Flags { get; set; }
    }
}
