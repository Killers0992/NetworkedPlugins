namespace NetworkedPlugins.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NetworkedPlugins.API.Interfaces;
    using NetworkedPlugins.API.Models;
    using LiteNetLib;
    using LiteNetLib.Utils;
    using NetworkedPlugins.API.Enums;
    using NetworkedPlugins.API.Packets;
    using NetworkedPlugins.API.Models;
    using NetworkedPlugins.API.Events.Player;
    using static NetworkedPlugins.API.Events.NPEventHandler;
    using NetworkedPlugins.API.Extensions;

    /// <summary>
    /// Network Dedicated Addon.
    /// </summary>
    /// <typeparam name="TConfig">The config type.</typeparam>
    public abstract class NPAddonDedicated<TConfig, TRemoteConfig> : IAddonDedicated<TConfig, TRemoteConfig>
        where TConfig : IConfig, new()
        where TRemoteConfig : IConfig, new ()
    {
        /// <inheritdoc/>
        public NPServer Server { get; }

        public IEnumerable<NPServer> GetServers()
        {
            return NPManager.Singleton.Servers.Values.Where(p => p.ServerConfig.LinkToken == Server.ServerConfig.LinkToken);
        }

        /// <inheritdoc/>
        public IAddonHandler<IConfig> Handler { get; }

        /// <inheritdoc/>
        public virtual NPPermissions Permissions { get; } = new NPPermissions()
        {
            ReceivePermissions = new List<AddonSendPermissionTypes>() { AddonSendPermissionTypes.None },
            SendPermissions = new List<AddonReceivePermissionTypes>() { AddonReceivePermissionTypes.None },
        };

        /// <inheritdoc/>
        public Dictionary<CommandType, Dictionary<string, ICommand>> Commands { get; } = new Dictionary<CommandType, Dictionary<string, ICommand>>()
        {
            { CommandType.GameConsole, new Dictionary<string, ICommand>() },
            { CommandType.RemoteAdmin, new Dictionary<string, ICommand>() },
        };

        /// <inheritdoc/>
        public NPManager Manager { get; }

        /// <inheritdoc/>
        public NPLogger Logger { get; }

        /// <inheritdoc/>
        public virtual string AddonName { get; }

        /// <inheritdoc/>
        public virtual Version AddonVersion { get; }

        /// <inheritdoc/>
        public virtual string AddonAuthor { get; }

        /// <inheritdoc/>
        public virtual string AddonId { get; }

        /// <inheritdoc/>
        public string DefaultPath { get; }

        /// <inheritdoc/>
        public string AddonPath { get; }

        /// <inheritdoc/>
        public string ServerPath { get; }

        /// <inheritdoc/>
        public TConfig Config { get; } = new TConfig();

        /// <inheritdoc/>
        public TRemoteConfig RemoteConfig { get; } = new TRemoteConfig();

        /// <summary>
        /// Send data to client.
        /// </summary>
        /// <param name="writer">Data writer.</param>
        public void SendData(NetDataWriter writer)
        {
            Manager.PacketProcessor.Send<AddonDataPacket>(Server.Peer,
                new AddonDataPacket() 
                { 
                    AddonId = AddonId, 
                    Data = writer.Data 
                }, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Check if server running is online.
        /// </summary>
        /// <param name="port">Server Port.</param>
        /// <returns>Is Online.</returns>
        public bool IsServerOnline(ushort port)
        {
            return Manager.Servers.Any(p => p.Value.ServerPort == port);
        }

        /// <inheritdoc/>
        public virtual void OnMessageReceived(NetDataReader reader)
        {
        }

        /// <inheritdoc/>
        public virtual void OnEnable()
        {
            Logger.Info($"[{Server.FullAddress}] Enabled addon \"{AddonName}\" ({AddonVersion}) made by {AddonAuthor}.");
        }

        /// <inheritdoc/>
        public virtual void OnDisable()
        {
            Logger.Info($"[{Server.FullAddress}] Disabled addon \"{AddonName}\" ({AddonVersion}) made by {AddonAuthor}.");
        }

        /// <inheritdoc/>
        public virtual void OnConsoleCommand(string cmd, List<string> arguments)
        {
        }

        /// <inheritdoc/>
        public virtual void OnConsoleResponse(string command, string[] arguments, CommandType type, string response)
        {
        }

        /// <summary>
        /// Compare configs.
        /// </summary>
        /// <param name="other"> Config.</param>
        /// <returns>Int.</returns>
        public int CompareTo(IAddonDedicated<IConfig, IConfig> other) => 0;

        public CustomEventHandler<PlayerJoinedEvent> PlayerJoined { get; set; }
        public void InvokePlayerJoined(PlayerJoinedEvent ev) => PlayerJoined.InvokeSafely(ev);

        public CustomEventHandler<PlayerLeftEvent> PlayerLeft { get; set; }
        public void InvokePlayerLeft(PlayerLeftEvent ev) => PlayerLeft.InvokeSafely(ev);

        public CustomEventHandler<PlayerLocalReportEvent> PlayerLocalReport { get; set; }
        public void InvokePlayerLocalReport(PlayerLocalReportEvent ev) => PlayerLocalReport.InvokeSafely(ev);
    }
}
