namespace NetworkedPlugins.API.Packets
{
    /// <summary>
    /// Receive addons packet.
    /// </summary>
    public class ReceiveAddonsPacket
    {
        /// <summary>
        /// Gets or sets addon ids.
        /// </summary>
        public string[] AddonIds { get; set; }
    }
}
