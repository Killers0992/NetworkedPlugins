namespace NetworkedPlugins.API.Interfaces
{
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
        /// Gets a value indicating whether gets if command is on remoteadmin.
        /// </summary>
        bool IsRaCommand { get; }

        /// <summary>
        /// Invoke command.
        /// </summary>
        /// <param name="player">Player functions.</param>
        /// <param name="arguments">Command arguments.</param>
        void Invoke(PlayerFuncs player, List<string> arguments);
    }
}
