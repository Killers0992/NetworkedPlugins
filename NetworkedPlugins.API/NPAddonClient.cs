namespace NetworkedPlugins.API
{
    using System;
    using System.Collections.Generic;
    using NetworkedPlugins.API.Interfaces;
    using NetworkedPlugins.API.Models;
    using NetworkedPlugins.API.Models;
    using LiteNetLib;
    using LiteNetLib.Utils;
    using NetworkedPlugins.API.Packets;

    /// <summary>
    /// Network Client Addon.
    /// </summary>
    /// <typeparam name="TConfig">The config type.</typeparam>
    public abstract class NPAddonClient<TConfig> : IAddonClient<TConfig>
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
        /// Send data to server.
        /// </summary>
        /// <param name="writer">Data writer.</param>
        public void SendData(NetDataWriter writer)
        {
            Manager.PacketProcessor.Send<AddonDataPacket>(Manager.NetworkListener,
                new AddonDataPacket()
                {
                    AddonId = AddonId,
                    Data = writer.Data
                }, DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public virtual void OnMessageReceived(NetDataReader reader)
        {
        }

        /// <inheritdoc/>
        public virtual void OnEnable()
        {
            Logger.Info($"Enabled addon \"{AddonName}\" ({AddonVersion}) made by {AddonAuthor}.");
        }


        /// <inheritdoc/>
        public virtual void OnDisable()
        {
            Logger.Info($"Disabled addon \"{AddonName}\" ({AddonVersion}) made by {AddonAuthor}.");
        }

        /// <inheritdoc/>
        public virtual void OnReady()
        {
            Logger.Info($"Addon \"{AddonName}\" ({AddonVersion}) made by {AddonAuthor} is ready!");
        }

        /// <summary>
        /// Compare configs.
        /// </summary>
        /// <param name="other"> Config.</param>
        /// <returns>Int.</returns>
        public int CompareTo(IAddonClient<IConfig> other) => 0;
    }
}
