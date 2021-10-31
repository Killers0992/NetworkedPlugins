namespace ExampleAddon.Commands
{
    using System.Collections.Generic;
    using NetworkedPlugins.API.Enums;
    using NetworkedPlugins.API.Interfaces;
    using NetworkedPlugins.API.Models;

    /// <inheritdoc/>
    public class TestCommand : ICommand
    {
        /// <inheritdoc/>
        public string CommandName => "test";

        /// <inheritdoc/>
        public string Description { get; } = "Test Command";

        /// <inheritdoc/>
        public string Permission { get; } = string.Empty;

        /// <inheritdoc/>
        public CommandType Type => CommandType.RemoteAdmin;

        /// <inheritdoc/>
        public void Invoke(NPPlayer player, string[] arguments)
        {
            player.SendRAMessage("Test response");
        }
    }
}
