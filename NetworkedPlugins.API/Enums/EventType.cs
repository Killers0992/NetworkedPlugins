using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Enums
{
    public enum EventType : byte
    {
        PlayerJoined,
        PlayerLeft,
        PlayerLocalReport,
        PreAuth,
        WaitingForPlayers,
        RoundEnded
    }
}
