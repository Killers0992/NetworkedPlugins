namespace NetworkedPlugins.API.Packets
{
    /// <summary>
    /// Update player info packet.
    /// </summary>
    public class UpdatePlayerInfoPacket
    {
        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// Gets or sets player info type.
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Gets or sets player info data.
        /// </summary>
        public byte[] Data { get; set; }
    }
}
