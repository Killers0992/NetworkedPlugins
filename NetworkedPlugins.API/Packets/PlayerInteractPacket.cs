namespace NetworkedPlugins.API.Packets
{
    /// <summary>
    /// Player interact packet.
    /// </summary>
    public class PlayerInteractPacket
    {
        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        public string UserID { get; set; }

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
