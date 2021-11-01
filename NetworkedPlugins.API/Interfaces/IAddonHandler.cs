using NetworkedPlugins.API.Events.Player;
using NetworkedPlugins.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetworkedPlugins.API.Events.NPEventHandler;

namespace NetworkedPlugins.API.Interfaces
{

    /// <summary>
    /// Network Addon.
    /// </summary>
    /// <typeparam name="TConfig">The config type.</typeparam>
    public interface IAddonHandler<out TConfig> : IComparable<IAddonHandler<IConfig>>
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

        IAddonDedicated<IConfig, IConfig> DefaultAddon { get; }

        Dictionary<NPServer, IAddonDedicated<IConfig, IConfig>> AddonInstances { get; }

        void AddAddon(NPServer targetServer);

        /// <summary>
        /// Gets addon config.
        /// </summary>
        TConfig Config { get; }

        /// <summary>
        /// Called when the addon handler is enabled.
        /// </summary>
        void OnEnable();

        /// <summary>
        /// Called when the addon handler is disabled.
        /// </summary>
        void OnDisable();

        CustomEventHandler<PlayerJoinedEvent> PlayerJoined { get; set; }
        void InvokePlayerJoined(PlayerJoinedEvent ev, NPServer server);

        CustomEventHandler<PlayerLeftEvent> PlayerLeft { get; set; }
        void InvokePlayerLeft(PlayerLeftEvent ev, NPServer server);

        CustomEventHandler<PlayerLocalReportEvent> PlayerLocalReport { get; set; }
        void InvokePlayerLocalReport(PlayerLocalReportEvent ev, NPServer server);
    }
}
