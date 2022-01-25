using NetworkedPlugins.API.Events.Player;
using NetworkedPlugins.API.Interfaces;
using NetworkedPlugins.API.Models;
using NetworkedPlugins.API.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using NetworkedPlugins.API.Events;
using static NetworkedPlugins.API.Events.NPEventHandler;
using NetworkedPlugins.API.Events.Server;

namespace NetworkedPlugins.API
{
    public abstract class NPAddonHandler<TConfig> : IAddonHandler<TConfig>
        where TConfig : IConfig, new()
    {
        public NPManager Manager { get; }

        public NPLogger Logger { get; }

        public IAddonDedicated<IConfig, IConfig> DefaultAddon { get; }

        public Dictionary<NPServer, IAddonDedicated<IConfig, IConfig>> AddonInstances { get; } = new Dictionary<NPServer, IAddonDedicated<IConfig, IConfig>>();

        public void AddAddon(NPServer targetServer)
        {
            
            var addon = (IAddonDedicated<IConfig, IConfig>)Activator.CreateInstance(DefaultAddon.GetType());
            var property = addon.GetType().GetProperty("Server", BindingFlags.Public | BindingFlags.Instance);
            var field = property.GetBackingField();
            field.SetValue(addon, targetServer);

            property = addon.GetType().GetProperty("ServerPath", BindingFlags.Public | BindingFlags.Instance);
            field = property.GetBackingField();
            field.SetValue(addon, targetServer.ServerDirectory);

            property = addon.GetType().GetProperty("Handler", BindingFlags.Public | BindingFlags.Instance);
            field = property.GetBackingField();
            field.SetValue(addon, this);

            property = addon.GetType().GetProperty("DefaultPath", BindingFlags.Public | BindingFlags.Instance);
            field = property.GetBackingField();
            field.SetValue(addon, DefaultAddon.DefaultPath);

            property = addon.GetType().GetProperty("AddonPath", BindingFlags.Public | BindingFlags.Instance);
            field = property.GetBackingField();
            field.SetValue(addon, DefaultAddon.AddonPath);

            property = addon.GetType().GetProperty("Manager", BindingFlags.Public | BindingFlags.Instance);
            field = property.GetBackingField();
            field.SetValue(addon, DefaultAddon.Manager);

            property = addon.GetType().GetProperty("Logger", BindingFlags.Public | BindingFlags.Instance);
            field = property.GetBackingField();
            field.SetValue(addon, DefaultAddon.Logger);

            property = addon.GetType().GetProperty("Commands", BindingFlags.Public | BindingFlags.Instance);
            field = property.GetBackingField();
            field.SetValue(addon, DefaultAddon.Commands);

            AddonInstances.Add(targetServer, addon);
        }

        public TConfig Config { get; } = new TConfig();

        public virtual void OnDisable()
        {
            foreach (var addon in AddonInstances.Values)
            {
                if (!addon.RemoteConfig.IsEnabled)
                    continue;

                try
                {
                    addon.OnDisable();
                }
                catch (Exception)
                {
                    Logger.Error($"Error while executing OnDisable event in {addon.AddonName}");
                }
            }
            Logger.Info($"Disabled handler for addon \"{DefaultAddon.AddonName}\" ({DefaultAddon.AddonVersion}) made by \"{DefaultAddon.AddonAuthor}\".");
        }

        public virtual void OnEnable()
        {
            foreach (var addon in AddonInstances.Values)
            {
                if (!addon.RemoteConfig.IsEnabled)
                    continue;

                try
                {
                    addon.OnEnable();
                }
                catch (Exception)
                {
                    Logger.Error($"Error while executing OnDisable event in {addon.AddonName}");
                }
            }
            Logger.Info($"Enabled handler for addon \"{DefaultAddon.AddonName}\" ({DefaultAddon.AddonVersion}) made by \"{DefaultAddon.AddonAuthor}\".");
        }

        public int CompareTo(IAddonHandler<IConfig> other) => 0;

        public CustomEventHandler<PlayerJoinedEvent> PlayerJoined { get; set; }
        public void InvokePlayerJoined(PlayerJoinedEvent ev, NPServer server)
        {
            PlayerJoined.InvokeSafely(ev);

            if (!AddonInstances.TryGetValue(server, out IAddonDedicated<IConfig, IConfig> addon))
                return;

            addon.InvokePlayerJoined(ev);
        }

        public CustomEventHandler<PlayerLeftEvent> PlayerLeft { get; set; }
        public void InvokePlayerLeft(PlayerLeftEvent ev, NPServer server)
        {
            PlayerLeft.InvokeSafely(ev);

            if (!AddonInstances.TryGetValue(server, out IAddonDedicated<IConfig, IConfig> addon))
                return;

            addon.InvokePlayerLeft(ev);
        }

        public CustomEventHandler<PlayerLocalReportEvent> PlayerLocalReport { get; set; }
        public void InvokePlayerLocalReport(PlayerLocalReportEvent ev, NPServer server)
        {
            PlayerLocalReport.InvokeSafely(ev);

            if (!AddonInstances.TryGetValue(server, out IAddonDedicated<IConfig, IConfig> addon))
                return;

            addon.InvokePlayerLocalReport(ev);
        }

        public CustomEventHandler<PlayerPreAuthEvent> PlayerPreAuth { get; set; }
        public void InvokePlayerPreAuth(PlayerPreAuthEvent ev, NPServer server)
        {
            PlayerPreAuth.InvokeSafely(ev);

            if (!AddonInstances.TryGetValue(server, out IAddonDedicated<IConfig, IConfig> addon))
                return;

            addon.InvokePlayerPreAuth(ev);
        }

        public CustomEventHandler<WaitingForPlayersEvent> WaitingForPlayers { get; set; }
        public void InvokeWaitingForPlayers(WaitingForPlayersEvent ev, NPServer server)
        {
            WaitingForPlayers.InvokeSafely(ev);

            if (!AddonInstances.TryGetValue(server, out IAddonDedicated<IConfig, IConfig> addon))
                return;

            addon.InvokeWaitingForPlayers(ev);
        }

        public CustomEventHandler<RoundEndedEvent> RoundEnded { get; set; }
        public void InvokeRoundEnded(RoundEndedEvent ev, NPServer server)
        {
            RoundEnded.InvokeSafely(ev);

            if (!AddonInstances.TryGetValue(server, out IAddonDedicated<IConfig, IConfig> addon))
                return;

            addon.InvokeRoundEnded(ev);
        }
    }
}
