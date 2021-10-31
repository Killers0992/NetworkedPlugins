using NetworkedPlugins.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Packets.ClientPackets
{
    public class EventPacket
    {
        public byte Type { get; set; }
        public byte[] Data { get; set; }
    }
}
