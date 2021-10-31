using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Models
{
    public class SendTokenPacket
    {
        public byte[] Token { get; set; }
    }
}
