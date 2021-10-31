namespace NetworkedPlugins.API.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Receive commands packet.
    /// </summary>
    public class ReceiveCommandsPacket
    {
        /// <summary>
        /// Gets or sets received commands.
        /// </summary>
        public List<CommandInfo> Commands { get; set; }
    }
}
