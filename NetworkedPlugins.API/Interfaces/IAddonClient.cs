namespace NetworkedPlugins.API.Interfaces
{
    using System;
    using System.Collections.Generic;
    using NetworkedPlugins.API.Structs;
    using LiteNetLib.Utils;

    /// <summary>
    /// Network Addon.
    /// </summary>
    /// <typeparam name="TConfig">The config type.</typeparam>
    public interface IAddonClient<out TConfig> : IComparable<IAddonClient<IConfig>>
        where TConfig : IConfig
    {
        /// <summary>
        /// Gets network manager.
        /// </summary>
        NPManager Manager { get; }

        /// <summary>
        /// Gets logger.
        /// </summary>
        NPLogger Logger { get; }

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
        /// Gets addon config.
        /// </summary>
        TConfig Config { get; }

        /// <summary>
        /// Called when the addon is enabled.
        /// </summary>
        void OnEnable();

        /// <summary>
        /// Called when the addon is disabled.
        /// </summary>
        void OnDisable();

        /// <summary>
        /// On addon is ready.
        /// </summary>
        void OnReady();

        /// <summary>
        /// Called when a message is received from a server.
        /// </summary>
        /// <param name="server">Received from.</param>
        /// <param name="reader">Reader data.</param>
        void OnMessageReceived(NetDataReader reader);
    }
}
