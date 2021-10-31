﻿using NetworkedPlugins.API.Events.Player;
using NetworkedPlugins.API.Interfaces;
using NetworkedPlugins.API.Structs;
using NetworkedPlugins.API.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using NetworkedPlugins.API.Events;
using static NetworkedPlugins.API.Events.NPEventHandler;

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
            AddonInstances.Add(targetServer, DefaultAddon);

            if (!AddonInstances.TryGetValue(targetServer, out IAddonDedicated<IConfig, IConfig> addon))
                return;

            var property = addon.GetType().GetProperty("Server", BindingFlags.Public | BindingFlags.Instance);
            var field = property.GetBackingField();
            field.SetValue(addon, targetServer);

            property = addon.GetType().GetProperty("ServerPath", BindingFlags.Public | BindingFlags.Instance);
            field = property.GetBackingField();
            field.SetValue(addon, targetServer.ServerDirectory);

            property = addon.GetType().GetProperty("Handler", BindingFlags.Public | BindingFlags.Instance);
            field = property.GetBackingField();
            field.SetValue(addon, this);
        }

        public TConfig Config { get; }

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
    }
}