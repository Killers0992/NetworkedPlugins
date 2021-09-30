namespace NetworkedPlugins.API.Packets
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
        public List<CommandInfoPacket> Commands { get; set; }
    }
}
