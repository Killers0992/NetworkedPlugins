namespace NetworkedPlugins.API.Packets
{
    /// <summary>
    /// Send hint packet.
    /// </summary>
    public class SendHintPacket
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
        public float Duration { get; set; }
    }
}
