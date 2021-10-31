using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Packets.ServerPackets
{
    public class SendAddonsInfoPacket
    {
        public List<AddonInfo> Addons { get; set; }
    }
}
