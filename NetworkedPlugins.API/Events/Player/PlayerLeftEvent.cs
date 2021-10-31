using NetworkedPlugins.API.Models;
using System;

namespace NetworkedPlugins.API.Events.Player
{
    public class PlayerLeftEvent : EventArgs
    {
        public PlayerLeftEvent(NPPlayer player) => Player = player;

        public NPPlayer Player { get; }
    }
}
