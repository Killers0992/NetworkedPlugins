namespace NetworkedPlugins.API.Interfaces
{
    using NetworkedPlugins.API.Enums;
    using NetworkedPlugins.API.Models;
    using System.Collections.Generic;

    /// <summary>
    /// Command interface.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets command name.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// Gets command description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets command permission.
        /// </summary>
        string Permission { get; }

        /// <summary>
        /// Gets a value indicating command type.
        /// </summary>
        CommandType Type { get; }

        /// <summary>
        /// Invoke command.
        /// </summary>
        /// <param name="player">Player functions.</param>
        /// <param name="arguments">Command arguments.</param>
        void Invoke(NPPlayer player, string[] arguments);
    }
}
