namespace NetworkedPlugins.API.Packets
{
    using System.Collections.Generic;

    /// <summary>
    /// Receive players packet.
    /// </summary>
    public class ReceivePlayersDataPacket
    {
        /// <summary>
        /// Gets or sets players.
        /// </summary>
        public List<PlayerInfoPacket> Players { get; set; }
    }
}
