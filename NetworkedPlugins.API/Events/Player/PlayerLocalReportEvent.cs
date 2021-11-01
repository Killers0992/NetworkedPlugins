using NetworkedPlugins.API.Models;
using System;

namespace NetworkedPlugins.API.Events.Player
{
    public class PlayerLocalReportEvent : EventArgs
    {
        public PlayerLocalReportEvent(NPPlayer player, NPPlayer targetPlayer, string reason)
        {
            this.Player = player;
            this.TargetPlayer = targetPlayer;
            this.Reason = reason;
        }

        public NPPlayer Player { get; }
        public NPPlayer TargetPlayer { get; }
        public string Reason { get; }
    }
}
