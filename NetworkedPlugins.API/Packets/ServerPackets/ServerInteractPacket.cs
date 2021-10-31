namespace NetworkedPlugins.API.Packets.ServerPackets
{
    /// <summary>
    /// Server interact packet.
    /// </summary>
    public class ServerInteractPacket
    {                   
        /// <summary>
        /// Gets or sets addon id.
        /// </summary>
        public string AddonId { get; set; }

        /// <summary>
        /// Gets or sets type of interaction.
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Gets or sets interaction data.
        /// </summary>
        public byte[] Data { get; set; }
    }
}
