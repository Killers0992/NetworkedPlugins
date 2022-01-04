namespace NetworkedPlugins.API.Interfaces
{
    using System;
    using System.Collections.Generic;
    using NetworkedPlugins.API.Models;
    using LiteNetLib.Utils;
    using NetworkedPlugins.API.Enums;
    using NetworkedPlugins.API.Models;
    using static NetworkedPlugins.API.Events.NPEventHandler;
    using NetworkedPlugins.API.Events.Player;
    using NetworkedPlugins.API.Events.Server;

    /// <summary>
    /// Network Addon.
    /// </summary>
    /// <typeparam name="TConfig">The config type.</typeparam>
    public interface IAddonDedicated<out TConfig, out TRemoteConfig> : IComparable<IAddonDedicated<IConfig, IConfig>>
        where TConfig : IConfig where TRemoteConfig : IConfig
    {
        IAddonHandler<IConfig> Handler { get; }

        NPPermissions Permissions { get; }
        IEnumerable<NPServer> GetServers();
        IEnumerable<NPServer> GetAllServers();

        /// <summary>
        /// Gets server connected.
        /// </summary>
        NPServer Server { get; }

        /// <summary>
        /// Gets network manager.
        /// </summary>
        NPManager Manager { get; }

        /// <summary>
        /// Gets logger.
        /// </summary>
        NPLogger Logger { get; }

        /// <summary>
        /// Gets addon commands.
        /// </summary>
        Dictionary<CommandType, Dictionary<string, ICommand>> Commands { get; }

        /// <summary>
        /// Gets the AddonName.
        /// </summary>
        string AddonName { get; }

        /// <summary>
        /// Gets the AddonVersion.
        /// </summary>
        Version AddonVersion { get; }

        /// <summary>
        /// Gets AddonAuthor.
        /// </summary>
        string AddonAuthor { get; }

        /// <summary>
        /// Gets addon id.
        /// </summary>
        string AddonId { get; }

        /// <summary>
        /// Gets default path.
        /// </summary>
        string DefaultPath { get; }

        /// <summary>
        /// Gets addon path.
        /// </summary>
        string AddonPath { get; }

        /// <summary>
        /// Gets server path.
        /// </summary>
        string ServerPath { get; }

        /// <summary>
        /// Gets addon config.
        /// </summary>
        TConfig Config { get; }

        /// <summary>
        /// Gets addon remote config.
        /// </summary>
        TRemoteConfig RemoteConfig { get; }

        /// <summary>
        /// Called when the addon is enabled.
        /// </summary>
        void OnEnable();

        /// <summary>
        /// Called when the addon is disabled.
        /// </summary>
        void OnDisable();

        /// <summary>
        /// Called when a message is received from a client.
        /// </summary>
        /// <param name="server">Received from.</param>
        /// <param name="reader">Reader data.</param>
        void OnMessageReceived(NetDataReader reader);

        /// <summary>
        /// Called when a console command is received.
        /// </summary>
        /// <param name="cmd"> Command name.</param>
        /// <param name="arguments"> Command arguments.</param>
        void OnConsoleCommand(string cmd, List<string> arguments);

        /// <summary>
        /// Called when a server console response is received.
        /// </summary>
        /// <param name="server"> Received from.</param>
        /// <param name="command"> Command name.</param>
        /// <param name="response"> Response.</param>
        /// <param name="isRa"> Is Ra Command.</param>
        void OnConsoleResponse(string command, string[] arguments, CommandType type, string response);

        CustomEventHandler<PlayerJoinedEvent> PlayerJoined { get; set; }
        void InvokePlayerJoined(PlayerJoinedEvent ev);

        CustomEventHandler<PlayerLeftEvent> PlayerLeft { get; set; }
        void InvokePlayerLeft(PlayerLeftEvent ev);

        CustomEventHandler<PlayerLocalReportEvent> PlayerLocalReport { get; set; }
        void InvokePlayerLocalReport(PlayerLocalReportEvent ev);

        CustomEventHandler<PlayerPreAuthEvent> PlayerPreAuth { get; set; }
        void InvokePlayerPreAuth(PlayerPreAuthEvent ev);

        CustomEventHandler<WaitingForPlayersEvent> WaitingForPlayers { get; set; }
        void InvokeWaitingForPlayers(WaitingForPlayersEvent ev);

        CustomEventHandler<RoundEndedEvent> RoundEnded { get; set; }
        void InvokeRoundEnded(RoundEndedEvent ev);
    }
}
