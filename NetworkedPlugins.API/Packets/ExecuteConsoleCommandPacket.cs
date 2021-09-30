namespace NetworkedPlugins.API.Packets
{
    /// <summary>
    /// Execute console command packet.
    /// </summary>
    public class ExecuteConsoleCommandPacket
    {
        /// <summary>
        /// Gets or sets addon id.
        /// </summary>
        public string AddonID { get; set; }

        /// <summary>
        /// Gets or sets command name.
        /// </summary>
        public string Command { get; set; }
    }
}
