namespace NetworkedPlugins.API.Packets
{
    /// <summary>
    /// Send broadcast packet.
    /// </summary>
    public class SendBroadcastPacket
    {
        /// <summary>
        /// Gets or sets a value indicating whether is admin only.
        /// </summary>
        public bool IsAdminOnly { get; set; }

        /// <summary>
        /// Gets or sets broadcast message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets duration of broadcast.
        /// </summary>
        public ushort Duration { get; set; }
    }
}
