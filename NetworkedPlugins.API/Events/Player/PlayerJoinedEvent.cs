using NetworkedPlugins.API.Models;
using System;

namespace NetworkedPlugins.API.Events.Player
{
    public class PlayerJoinedEvent : EventArgs
    {
        public PlayerJoinedEvent(NPPlayer player) => Player = player;

        public NPPlayer Player { get; }
    }
}
