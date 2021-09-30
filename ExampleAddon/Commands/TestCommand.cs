namespace ExampleAddon.Commands
{
    using System.Collections.Generic;
    using NetworkedPlugins.API.Interfaces;

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
        public bool IsRaCommand => true;

        /// <inheritdoc/>
        public void Invoke(PlayerFuncs player, List<string> arguments)
        {
            player.SendRAMessage("Test response");
        }
    }
}
