namespace NetworkedPlugins.API.Packets
{
    /// <summary>
    /// Receive addons packet.
    /// </summary>
    public class ReceiveAddonDataPacket
    {
        /// <summary>
        /// Gets or sets addon id.
        /// </summary>
        public string AddonID { get; set; }

        /// <summary>
        /// Gets or sets addon data.
        /// </summary>
        public byte[] Data { get; set; }
    }
}
