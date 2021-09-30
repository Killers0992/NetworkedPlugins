namespace NetworkedPlugins.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LiteNetLib;
    using LiteNetLib.Utils;
    using NetworkedPlugins.API.Interfaces;
    using NetworkedPlugins.API.Models;
    using NetworkedPlugins.API.Packets;

    /// <summary>
    /// Network Host Addon.
    /// </summary>
    /// <typeparam name="TConfig">The config type.</typeparam>
    public abstract class NPAddonHost<TConfig> : IAddon<TConfig>
        where TConfig : IConfig, new()
    {
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
        public TConfig Config { get; } = new TConfig();

        /// <summary>
        /// Send data to client.
        /// </summary>
        /// <param name="serverPort">Server port.</param>
        /// <param name="writer">Data writer.</param>
        public void SendData(ushort serverPort, NetDataWriter writer)
        {
            foreach (var obj in Manager.Servers)
            {
                if (obj.Value.ServerPort == serverPort)
                {
                    Manager.PacketProcessor.Send<ReceiveAddonDataPacket>(obj.Key, new ReceiveAddonDataPacket() { AddonID = AddonId, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                }
            }
        }

        /// <summary>
        /// Send data to client.
        /// </summary>
        /// <param name="server">Server.</param>
        /// <param name="writer">Data writer.</param>
        public void SendData(NPServer server, NetDataWriter writer)
        {
            foreach (var obj in Manager.Servers)
            {
                if (obj.Value.FullAddress == server.FullAddress)
                {
                    Manager.PacketProcessor.Send<ReceiveAddonDataPacket>(obj.Key, new ReceiveAddonDataPacket() { AddonID = AddonId, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                }
            }
        }

        /// <summary>
        /// Send data to client.
        /// </summary>
        /// <param name="serverAddress">Server address.</param>
        /// <param name="writer">Data writer.</param>
        public void SendData(string serverAddress, NetDataWriter writer)
        {
            foreach (var obj in Manager.Servers)
            {
                if (obj.Key.EndPoint.Address.ToString() == serverAddress)
                {
                    Manager.PacketProcessor.Send<ReceiveAddonDataPacket>(obj.Key, new ReceiveAddonDataPacket() { AddonID = AddonId, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                }
            }
        }

        /// <summary>
        /// Send data to client.
        /// </summary>
        /// <param name="serverAddress">Server address.</param>
        /// <param name="serverPort">Server port.</param>
        /// <param name="writer">Data writer.</param>
        public void SendData(string serverAddress, ushort serverPort, NetDataWriter writer)
        {
            foreach (var obj in Manager.Servers)
            {
                if (obj.Key.EndPoint.Address.ToString() == serverAddress)
                {
                    if (obj.Value.ServerPort == serverPort)
                    {
                        Manager.PacketProcessor.Send<ReceiveAddonDataPacket>(obj.Key, new ReceiveAddonDataPacket() { AddonID = AddonId, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                    }
                }
            }
        }

        /// <summary>
        /// Gets all online servers.
        /// </summary>
        /// <returns>List of NPServer.</returns>
        public List<NPServer> GetServers()
        {
            return Manager.Servers.Values.ToList();
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

        /// <summary>
        /// Send data to all servers.
        /// </summary>
        /// <param name="writer">Data writer.</param>
        public void SendData(NetDataWriter writer)
        {
            Manager.PacketProcessor.Send<ReceiveAddonDataPacket>(Manager.NetworkListener, new ReceiveAddonDataPacket() { AddonID = AddonId, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public virtual void OnMessageReceived(NPServer server, NetDataReader reader)
        {
        }

        /// <inheritdoc/>
        public virtual void OnEnable()
        {
        }

        /// <inheritdoc/>
        public virtual void OnReady(NPServer server)
        {
        }

        /// <inheritdoc/>
        public virtual void OnConsoleCommand(string cmd, List<string> arguments)
        {
        }

        /// <inheritdoc/>
        public virtual void OnConsoleResponse(NPServer server, string command, string response, bool isRa)
        {
        }

        /// <summary>
        /// Compare configs.
        /// </summary>
        /// <param name="other"> Config.</param>
        /// <returns>Int.</returns>
        public int CompareTo(IAddon<IConfig> other) => 0;
    }
}
