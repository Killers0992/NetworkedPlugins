namespace NetworkedPlugins.API.Models
{
    /// <summary>
    /// Console response packet.
    /// </summary>
    public class ConsoleResponsePacket
    {
        /// <summary>
        /// Gets or sets addon id.
        /// </summary>
        public string AddonId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating command type.
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Gets or sets command name.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets command args.
        /// </summary>
        public string[] Arguments { get; set; }

        /// <summary>
        /// Gets or sets command response.
        /// </summary>
        public string Response { get; set; }
    }
}
