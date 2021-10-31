using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Packets.ClientPackets
{
    public class AddonOkPacket
    {
        public string AddonId { get; set; }
        public byte[] RemoteConfig { get; set; }
    }
}
