namespace NetworkedPlugins.API.Packets
{
    /// <summary>
    /// Excute command packet.
    /// </summary>
    public class ExecuteCommandPacket
    {
        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// Gets or sets addon id.
        /// </summary>
        public string AddonID { get; set; }

        /// <summary>
        /// Gets or sets command name.
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// Gets or sets arguments.
        /// </summary>
        public string[] Arguments { get; set; }
    }
}
