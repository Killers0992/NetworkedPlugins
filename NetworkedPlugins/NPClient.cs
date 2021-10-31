namespace NetworkedPlugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using NetworkedPlugins.API;
    using NetworkedPlugins.API.Interfaces;
    using NetworkedPlugins.API.Structs;
    using NetworkedPlugins.API.Extensions;
    using LiteNetLib;
    using LiteNetLib.Utils;
    using MEC;
    using Mirror;

    using static Broadcast;
    using RemoteAdmin;
    using NetworkedPlugins.API.Enums;
    using NetworkedPlugins.API.Packets;
    using NetworkedPlugins.API.Packets.ServerPackets;
    using System.Text;
    using NetworkedPlugins.API.Packets.ClientPackets;

    /// <summary>
    /// Network client.
    /// </summary>
    public class NPClient : NPManager, INetEventListener
    {
        private MainClass plugin;

        private CoroutineHandle refreshPolls;
        private Dictionary<string, NetworkedPlayer> Players = new Dictionary<string, NetworkedPlayer>();
        private NetDataWriter defaultdata;
        private string tokenPath;
        private string remoteConfigsPath;
        private bool isDownloading;
        private DateTime nextUpdate = DateTime.Now;
        private Dictionary<string, AddonInfo> InstalledAddons { get; } = new Dictionary<string, AddonInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NPClient"/> class.
        /// </summary>
        /// <param name="plugin">Plugin class.</param>
        public NPClient(MainClass plugin)
        {
            NPManager.Singleton = this;
            this.plugin = plugin;
            Logger = new PluginLogger();
            string pluginDir = Path.Combine(Paths.Plugins, "NetworkedPlugins");
            tokenPath = Path.Combine(pluginDir, $"NPToken_{Server.Port}.token");
            if (!Directory.Exists(pluginDir))
                Directory.CreateDirectory(pluginDir);
            if (!Directory.Exists(Path.Combine(pluginDir, "addons-" + Server.Port)))
                Directory.CreateDirectory(Path.Combine(pluginDir, "addons-" + Server.Port));
            remoteConfigsPath = Path.Combine(pluginDir, "remoteconfigs-" + Server.Port);
            if (!Directory.Exists(remoteConfigsPath))
                Directory.CreateDirectory(remoteConfigsPath);
            string[] addonsFiles = Directory.GetFiles(Path.Combine(pluginDir, "addons-" + Server.Port), "*.dll");
            Log.Info($"Loading {addonsFiles.Length} addons.");
            foreach (var file in addonsFiles)
            {
                Assembly a = Assembly.LoadFrom(file);
                try
                {
                    foreach (Type t in a.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface))
                    {
                        if (!t.BaseType.IsGenericType || t.BaseType.GetGenericTypeDefinition() != typeof(NPAddonClient<>))
                            continue;

                        IAddonClient<IConfig> addon = null;

                        var constructor = t.GetConstructor(Type.EmptyTypes);
                        if (constructor != null)
                        {
                            addon = constructor.Invoke(null) as IAddonClient<IConfig>;
                        }
                        else
                        {
                            var value = Array.Find(t.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public), property => property.PropertyType == t)?.GetValue(null);

                            if (value != null)
                                addon = value as IAddonClient<IConfig>;
                        }

                        if (addon == null)
                            continue;

                        var addonType = addon.GetType();
                        var prop = addonType.GetProperty("DefaultPath", BindingFlags.Public | BindingFlags.Instance);
                        var field = prop.GetBackingField();
                        field.SetValue(addon, Path.Combine(pluginDir, $"addons-{Server.Port}"));

                        prop = addonType.GetProperty("AddonPath", BindingFlags.Public | BindingFlags.Instance);
                        field = prop.GetBackingField();
                        field.SetValue(addon, Path.Combine(addon.DefaultPath, addon.AddonName));

                        prop = addonType.GetProperty("Manager", BindingFlags.Public | BindingFlags.Instance);
                        field = prop.GetBackingField();
                        field.SetValue(addon, this);

                        prop = addonType.GetProperty("Logger", BindingFlags.Public | BindingFlags.Instance);
                        field = prop.GetBackingField();
                        field.SetValue(addon, Logger);

                        if (ClientAddons.ContainsKey(addon.AddonId))
                        {
                            Logger.Error($"Addon {addon.AddonName} already already registered with id {addon.AddonId}.");
                            break;
                        }

                        ClientAddons.Add(addon.AddonId, addon);
                        LoadAddonConfig(addon);
                        if (!addon.Config.IsEnabled)
                            return;

                        Logger.Info($"Loading addon \"{addon.AddonName}\" ({addon.AddonVersion}) made by {addon.AddonAuthor}.");
                        addon.OnEnable();
                        Logger.Info($"Waiting to client connections..");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed loading addon {Path.GetFileNameWithoutExtension(file)}. {ex.ToString()}");
                }
            }
            Logger.Info($"Starting CLIENT network...");
            Exiled.Events.Handlers.Player.Verified += Player_Verified;
            Exiled.Events.Handlers.Player.Destroying += Player_Destroying;
            Exiled.Events.Handlers.Server.WaitingForPlayers += Server_WaitingForPlayers;
        }

        public void CreateDefaultConnectionData()
        {
            defaultdata = new NetDataWriter();
            defaultdata.Put(plugin.Config.HostConnectionKey);
            defaultdata.Put(plugin.Version.ToString(3));
            defaultdata.Put(Server.Port);
            defaultdata.Put(CustomNetworkManager.slots);
            defaultdata.PutArray(ClientAddons.Select(p => p.Key).ToArray());
            if (File.Exists(tokenPath))
            {
                var bytes = File.ReadAllBytes(tokenPath);
                defaultdata.PutBytesWithLength(bytes, 0, bytes.Length);
            }
            else
            {
                var bytes = Encoding.UTF8.GetBytes(string.Empty);
                defaultdata.PutBytesWithLength(bytes, 0, bytes.Length);
            }
        }

        private void Player_Destroying(DestroyingEventArgs ev)
        {
            if (Players.TryGetValue(ev.Player.UserId, out NetworkedPlayer plr))
            {
                UnityEngine.Object.Destroy(plr);
                Players.Remove(ev.Player.UserId);
            }
        }

        /// <summary>
        /// Unload network client.
        /// </summary>
        public void Unload()
        {
            if (refreshPolls != null)
                Timing.KillCoroutines(refreshPolls);

            Exiled.Events.Handlers.Player.Destroying -= Player_Destroying;
            Exiled.Events.Handlers.Player.Verified -= Player_Verified;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= Server_WaitingForPlayers;
        }

        /// <inheritdoc/>
        public void OnPeerConnected(NetPeer peer)
        {
            Logger.Info("Client connected to host.");
            foreach (var player in Players.Values)
                player.Reset();
        }

        public string GetChangelogs(string ver)
        {
            using (var web = new WebClient())
            {
                try
                {
                    var changelogs = web.DownloadString($"https://raw.githubusercontent.com/Killers0992/NetworkedPlugins/{ver}/NetworkedPlugins.API/changelogs.txt");
                    return changelogs.Contains("404") ? "No Changelogs" : changelogs;
                }
                catch (Exception) { return "";  }
            }
        }

        public void DownloadNewVersion(string ver)
        {
            if (nextUpdate > DateTime.Now)
                return;

            isDownloading = true;
            using(var web = new WebClient())
            {
                Logger.Info($"Downloading \"NetworkedPlugins.dll\" version \"{ver}\"...");
                try
                {
                    web.DownloadFile($"https://github.com/Killers0992/NetworkedPlugins/releases/download/{ver}/NetworkedPlugins.dll", Path.Combine(Paths.Plugins, "NetworkedPlugins.dll"));
                }
                catch (Exception)
                {
                    Logger.Info($"Failed downloading \"NetworkedPlugins.dll\" version \"{ver}\", file not exists! (Next attempt in 15 seconds)");
                    isDownloading = false;
                    nextUpdate = DateTime.Now.AddSeconds(15);
                    return;
                }
                Logger.Info($"Downloaded \"NetworkedPlugins.dll\" version \"{ver}\"...");
                Logger.Info($"Downloading \"NetworkedPlugins.API.dll\" version \"{ver}\"...");
                web.DownloadFile($"https://github.com/Killers0992/NetworkedPlugins/releases/download/{ver}/NetworkedPlugins.API.dll", Path.Combine(Paths.Dependencies, "NetworkedPlugins.API.dll"));
                Logger.Info($"Downloaded \"NetworkedPlugins.API.dll\" version \"{ver}\"...");
                Logger.Info($"Changelogs: \n{GetChangelogs(ver)}");
                switch (plugin.Config.UpdateAction)
                {
                    case UpdateAction.Nothing:
                        Logger.Info($"Downloaded update \"{ver}\" of NetworkedPlugins.");
                        break;
                    case UpdateAction.RestartNextRound:
                        Logger.Info($"Downloaded update \"{ver}\" of NetworkedPlugins, server will be restarted next round.");
                        ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                        break;
                    case UpdateAction.RestartNow:
                        Logger.Info($"Downloaded update \"{ver}\" of NetworkedPlugins, restarting server.");
                        ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                        PlayerStats.StaticChangeLevel(true);
                        break;
                    case UpdateAction.RestartNowIfEmpty:
                        if (Player.List.Count() == 0 || !Round.IsStarted)
                        {
                            Logger.Info($"Downloaded update \"{ver}\" of NetworkedPlugins, restarting server.");
                            ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                            PlayerStats.StaticChangeLevel(true);
                        }
                        else
                        {
                            Logger.Info($"Downloaded update \"{ver}\" of NetworkedPlugins, server will be restarted next round (Server is not empty).");
                            ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                        }
                        break;
                }
            }
            isDownloading = false;
        }

        /// <inheritdoc/>
        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (disconnectInfo.AdditionalData.TryGetByte(out byte rejectType))
            {
                RejectType type = (RejectType)rejectType;
                switch (type)
                {
                    case RejectType.InvalidToken:
                        Logger.Info($"[Connection rejected] Server sended token which not exists on host! (Contact owner of host)");
                        break;
                    case RejectType.OutdatedVersion:
                        if (disconnectInfo.AdditionalData.TryGetString(out string newerVersion) && !isDownloading)
                        {
                            Logger.Info($"[Connection rejected] Server uses older version of NetworkedPlugins (Current: {NPVersion.Version.ToString(3)}, Newer: {newerVersion})");
                            DownloadNewVersion(newerVersion);
                        }
                        break;
                }
            }
            else
                Logger.Info($"[Disconnected] Reason \"{disconnectInfo.Reason}\".");

            foreach(var commandTypes in Commands)
            {
                foreach(var command in commandTypes.Value)
                {
                    switch (commandTypes.Key)
                    {
                        case CommandType.RemoteAdmin:
                            CommandProcessor.RemoteAdminCommandHandler.UnregisterCommand(command.Value);
                            break;
                        case CommandType.GameConsole:
                            QueryProcessor.DotCommandHandler.UnregisterCommand(command.Value);
                            break;
                    }
                    Logger.Info($"Command {command.Value.Command} unregistered from addon.");
                }
            }

            Commands[CommandType.GameConsole].Clear();
            Commands[CommandType.RemoteAdmin].Clear();

            foreach(var player in Players.Values)
                player.NetworkData.IsConnected = false;
            Timing.RunCoroutine(Reconnect());
        }

        /// <inheritdoc/>
        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Logger.Error($"Network error from endpoint {endPoint.Address}, {socketError}");
        }

        /// <inheritdoc/>
        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            try
            {
                PacketProcessor.ReadAllPackets(reader, peer);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error while receiving data from server {ex}");
            }
        }

        /// <inheritdoc/>
        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        /// <inheritdoc/>
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        /// <inheritdoc/>
        public void OnConnectionRequest(ConnectionRequest request)
        {
        }

        private static int GetMethodHash(Type invokeClass, string methodName)
        {
            return (invokeClass.FullName.GetStableHashCode() * 503) + methodName.GetStableHashCode();
        }

        private void Server_WaitingForPlayers()
        {
            if (NetworkListener == null)
                StartNetworkClient();
        }

        private void Player_Verified(VerifiedEventArgs ev)
        {
            Players.Add(ev.Player.UserId, ev.Player.GameObject.AddComponent<NetworkedPlayer>());
        }

        private void StartNetworkClient()
        {
            PacketProcessor.RegisterNestedType<CommandInfo>();
            PacketProcessor.RegisterNestedType<PlayerInfo>();
            PacketProcessor.RegisterNestedType<Position>();
            PacketProcessor.RegisterNestedType<Rotation>();
            PacketProcessor.RegisterNestedType<AddonInfo>();
            PacketProcessor.SubscribeReusable<PlayerInteractPacket, NetPeer>(OnPlayerInteract);
            PacketProcessor.SubscribeReusable<ServerInteractPacket, NetPeer>(OnServerInteract);
            PacketProcessor.SubscribeReusable<ReceiveCommandsPacket, NetPeer>(OnReceiveCommandsData);
            PacketProcessor.SubscribeReusable<SendTokenPacket, NetPeer>(OnReceiveNewToken);
            PacketProcessor.SubscribeReusable<SendAddonsInfoPacket, NetPeer>(OnReceiveAddons);
            PacketProcessor.SubscribeReusable<EventPacket, NetPeer>(OnReceiveEvent);
            PacketProcessor.SubscribeReusable<AddonDataPacket, NetPeer>(OnReceiveAddonData);
            NetworkListener = new NetManager(this);
            NetworkListener.Start();
            CreateDefaultConnectionData();
            NetworkListener.Connect(plugin.Config.HostAddress, plugin.Config.HostPort, defaultdata);
            refreshPolls = Timing.RunCoroutine(RefreshPolls());
        }

        private void OnServerInteract(ServerInteractPacket packet, NetPeer peer)
        {
            ServerInteractionType interactionType = (ServerInteractionType)packet.Type;
            if (!InstalledAddons.TryGetValue(packet.AddonId, out AddonInfo addonInfo))
            {
                Logger.Error($"Addon with id \"{packet.AddonId}\" tried to use interaction \"{interactionType}\" on server but addon is not loaded!");
                return;
            }

            NetDataReader reader = new NetDataReader(packet.Data);

            switch (interactionType)
            {
                case ServerInteractionType.ExecuteCommand:
                    {
                        if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.ConsoleCommandExecution))
                        {
                            Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to execute \"Console Command\" but server dont have required permission!");
                            return;
                        }

                        var command = reader.GetString();
                        var arguments = reader.GetStringArray();

                        var sender = new CustomConsoleExecutor(this, command, arguments, packet.AddonId);
                        GameCore.Console.singleton.TypeCommand($"{command} {string.Join(" ", arguments)}", sender);
                    }
                    break;
                case ServerInteractionType.Broadcast:
                    {
                        if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.Broadcasts))
                        {
                            Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to send \"Broadcast\" but server dont have required permission!");
                            return;
                        }

                        var message = reader.GetString();
                        var duration = reader.GetUShort();
                        var adminOnly = reader.GetBool();

                        if (adminOnly)
                        {
                            foreach (var plr in Player.List)
                                if (plr.ReferenceHub.serverRoles.LocalRemoteAdmin)
                                    plr.Broadcast(duration, message, BroadcastFlags.Normal, false);
                        }
                        else
                            Server.Broadcast.RpcAddElement(message, duration, BroadcastFlags.Normal);
                    }
                    break;
                case ServerInteractionType.ClearBroadcast:
                    {
                        if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.ClearBroadcasts))
                        {
                            Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to \"Clear Broadcasts\" but server dont have required permission!");
                            return;
                        }

                        Server.Broadcast.RpcClearElements();
                    }
                    break;
                case ServerInteractionType.Hint:
                    {
                        if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.HintMessages))
                        {
                            Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to send \"Hint Message\" but server dont have required permission!");
                            return;
                        }

                        var message = reader.GetString();
                        var duration = reader.GetFloat();
                        var adminOnly = reader.GetBool();

                        foreach (var plr in Player.List)
                        {
                            if (plr.ReferenceHub.serverRoles.LocalRemoteAdmin || !adminOnly)
                                plr.ShowHint(message, duration);
                        }
                    }
                    break;
                case ServerInteractionType.Roundrestart:
                    {
                        if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.Roundrestart))
                        {
                            Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to use \"Round Restart\" but server dont have required permission!");
                            return;
                        }

                        var port = reader.GetUShort();

                        if (port != 0)
                            ReferenceHub.HostHub.playerStats.RpcRoundrestartRedirect(0f, port);
                        else
                            ReferenceHub.HostHub.playerStats.Roundrestart();
                    }
                    break;
            }
        }

        private void OnReceiveEvent(EventPacket packet, NetPeer peer)
        {
            NetDataReader data = new NetDataReader(packet.Data);
            switch ((EventType)packet.Type)
            {
                case EventType.PlayerJoined:
                    {
                        string userId = data.GetString();
                        if (Players.TryGetValue(userId, out NetworkedPlayer plr))
                            plr.NetworkData.IsConnected = true;
                    }
                    break;
            }
        }

        private void OnReceiveAddons(SendAddonsInfoPacket packet, NetPeer peer)
        {
            foreach(var addon in packet.Addons)
            {
                if (!InstalledAddons.ContainsKey(addon.AddonId))
                    InstalledAddons.Add(addon.AddonId, addon);

                List<string> MissingInfo = new List<string>();
                if (!plugin.Config.Permissions.ReceivePermissions.Contains(AddonSendPermissionTypes.Everything) && addon.SendPermissions.Any(p => p != (byte)AddonSendPermissionTypes.None))
                    foreach (var sendPerm in addon.SendPermissions)                                                                                   
                        if (!plugin.Config.Permissions.ReceivePermissions.Contains((AddonSendPermissionTypes)sendPerm))
                            MissingInfo.Add($" - Missing SEND permission \"{(AddonSendPermissionTypes)sendPerm}\"");

                if (!plugin.Config.Permissions.SendPermissions.Contains(AddonReceivePermissionTypes.Everything) && addon.ReceivePermissions.Any(p => p != (byte)AddonReceivePermissionTypes.None))
                    foreach (var sendPerm in addon.ReceivePermissions)
                        if (!plugin.Config.Permissions.SendPermissions.Contains((AddonReceivePermissionTypes)sendPerm))
                            MissingInfo.Add($" - Missing RECEIVE permission \"{(AddonReceivePermissionTypes)sendPerm}\"");

                if (MissingInfo.Count != 0)
                    Logger.Info($"Addon \"{addon.AddonName}\" will not work properly, addon requires these permissions\n{string.Join(Environment.NewLine, MissingInfo)}");

                var config = Encoding.UTF8.GetString(addon.RemoteConfig);
                if (!File.Exists(Path.Combine(remoteConfigsPath, $"{addon.AddonName}.yml")))
                {
                    File.WriteAllText(Path.Combine(remoteConfigsPath, $"{addon.AddonName}.yml"), config);
                    Logger.Info($"Added missing config for addon \"{addon.AddonName}\"!");
                }

                var rawConfig = File.ReadAllText(Path.Combine(remoteConfigsPath, $"{addon.AddonName}.yml"));
                var receivedConfig = Deserializer.Deserialize<Dictionary<string, object>>(config);
                var loadedConfig = Deserializer.Deserialize<Dictionary<string, object>>(rawConfig);

                foreach(var missingval in receivedConfig.Except(loadedConfig))
                {
                    loadedConfig.Add(missingval.Key, missingval.Value);
                    Logger.Info($"Added missing config parameter \"{missingval.Key}\" in addon \"{addon.AddonName}\"!");
                }

                foreach (var removeval in loadedConfig.Except(receivedConfig))
                {
                    loadedConfig.Remove(removeval.Key);
                    Logger.Info($"Removed not existing config parameter \"{removeval.Key}\" in addon \"{addon.AddonName}\"!");
                }

                rawConfig = Serializer.Serialize(loadedConfig);
                File.WriteAllText(Path.Combine(remoteConfigsPath, $"{addon.AddonName}.yml"), rawConfig);

                if (ClientAddons.TryGetValue(addon.AddonId, out IAddonClient<IConfig> clientAddon))
                {
                    clientAddon.OnReady();
                }
                else
                {
                    Logger.Info($"RemoteAddon \"{addon.AddonName}\" ({addon.AddonVersion}) made by {addon.AddonAuthor} is ready!");
                }
                PacketProcessor.Send<AddonOkPacket>(NetworkListener, new AddonOkPacket()
                {
                    AddonId = addon.AddonId,
                    RemoteConfig = Encoding.UTF8.GetBytes(rawConfig)
                }, DeliveryMethod.ReliableOrdered);
            }
        }

        private void OnReceiveNewToken(SendTokenPacket packet, NetPeer peer)
        {
            if (!File.Exists(tokenPath))
            {
                File.WriteAllBytes(tokenPath, packet.Token);
                CreateDefaultConnectionData();
                Logger.Info($"Received new token from host and saved.");
            }
            else
            {
                Logger.Error($"Failed while saving new token, token file already exists! ( {tokenPath} )");
            }
        }

        private void OnReceiveAddonData(AddonDataPacket packet, NetPeer peer)
        {
            if (ClientAddons.TryGetValue(packet.AddonId, out IAddonClient<IConfig> addon))
            {
                addon.OnMessageReceived(new NetDataReader(packet.Data));
            }
            else
            {
                Logger.Error($"Failed receiving data while addon with id \"{packet.AddonId}\" is not loaded!");
            }
        }

        private void OnPlayerInteract(PlayerInteractPacket packet, NetPeer peer)
        {
            PlayerInteractionType interactionType = (PlayerInteractionType)packet.Type;
            if (!InstalledAddons.TryGetValue(packet.AddonId, out AddonInfo addonInfo))
            {
                Logger.Error($"Addon with id \"{packet.AddonId}\" tried to use interaction \"{interactionType}\" on player \"{packet.UserID}\" but addon is not loaded!");
                return;
            }

            NetDataReader reader = new NetDataReader(packet.Data);
            Player p = (packet.UserID == "SERVER CONSOLE" || packet.UserID == "GAME CONSOLE") ? Player.Get(PlayerManager.localPlayer) : Player.Get(packet.UserID);
            if (p == null)
            {
                Logger.Info($"Player not found {packet.UserID}, action: {packet.Type}.");
                return;
            }

            switch ((PlayerInteractionType)packet.Type)
            {
                // Kill player
                case PlayerInteractionType.KillPlayer:
                    if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.KillPlayer))
                    {
                        Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to \"{interactionType}\" but server dont have required permission!");
                        break;
                    }

                    p.Kill();
                    break;

                // Report message
                case PlayerInteractionType.ReportMessage:
                    if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.ReportMessages))
                    {
                        Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to send \"{interactionType}\" but server dont have required permission!");
                        break;
                    }

                    p.SendConsoleMessage($"[REPORTING] {reader.GetString()}", "GREEN");
                    break;

                // Remoteadmin message
                case PlayerInteractionType.RemoteAdminMessage:
                    if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.RemoteAdminMessages))
                    {
                        Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to send \"{interactionType}\" but server dont have required permission!");
                        break;
                    }

                    p.RemoteAdminMessage(reader.GetString(), true, reader.GetString());
                    break;

                // Console message
                case PlayerInteractionType.GameConsoleMessage:
                    if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.GameConsoleMessages))
                    {
                        Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to send \"{interactionType}\" but server dont have required permission!");
                        break;
                    }

                    var message = reader.GetString();
                    var pluginName = reader.GetString();

                    p.SendConsoleMessage($"{(!string.IsNullOrEmpty(pluginName) ? $"[{pluginName}] " : string.Empty)}{message}", reader.GetString());
                    break;

                // Redirect
                case PlayerInteractionType.Redirect:
                    if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.RedirectPlayer))
                    {
                        Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to \"{interactionType}\" player but server dont have required permission!");
                        break;
                    }

                    SendClientToServer(p, reader.GetUShort());
                    break;

                // Disconnect
                case PlayerInteractionType.Disconnect:
                    if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.DisconnectPlayer))
                    {
                        Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to \"{interactionType}\" player but server dont have required permission!");
                        break;
                    }

                    ServerConsole.Disconnect(p.GameObject, reader.GetString());
                    break;

                // Hint
                case PlayerInteractionType.Hint:
                    if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.HintMessages))
                    {
                        Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to send \"{interactionType}\" player but server dont have required permission!");
                        break;
                    }

                    p.ShowHint(reader.GetString(), reader.GetFloat());
                    break;

                // Send position to network
                case PlayerInteractionType.SendPosition:
                    bool sendPosition = reader.GetBool();
                    if (!p.SessionVariables.ContainsKey("SP"))
                        p.SessionVariables.Add("SP", sendPosition);
                    p.SessionVariables["SP"] = sendPosition;
                    break;

                // Send rotation to network
                case PlayerInteractionType.SendRotation:
                    bool sendRotation = reader.GetBool();
                    if (!p.SessionVariables.ContainsKey("SR"))
                        p.SessionVariables.Add("SR", sendRotation);
                    p.SessionVariables["SR"] = sendRotation;
                    break;

                // Teleport
                case PlayerInteractionType.Teleport:
                    if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.TeleportPlayer))
                    {
                        Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to \"{interactionType}\" player but server dont have required permission!");
                        break;
                    }

                    p.Position = new UnityEngine.Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
                    break;

                // Godmode
                case PlayerInteractionType.Godmode:
                    if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.GodmodePlayer))
                    {
                        Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to change \"{interactionType}\" player but server dont have required permission!");
                        break;
                    }

                    p.IsGodModeEnabled = reader.GetBool();
                    break;

                // Noclip
                case PlayerInteractionType.Noclip:
                    if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.NoclipPlayer))
                    {
                        Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to change \"{interactionType}\" player but server dont have required permission!");
                        break;
                    }

                    p.NoClipEnabled = reader.GetBool();
                    break;

                // Clear Inv
                case PlayerInteractionType.ClearInventory:
                    if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.ClearInventoryPlayer))
                    {
                        Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to \"{interactionType}\" but server dont have required permission!");
                        break;
                    }

                    p.ClearInventory();
                    break;
            }
        }

        private Dictionary<CommandType, Dictionary<string, CommandSystem.ICommand>> Commands = new Dictionary<CommandType, Dictionary<string, CommandSystem.ICommand>>()
        {
            { CommandType.GameConsole, new Dictionary<string, CommandSystem.ICommand>() },
            { CommandType.RemoteAdmin, new Dictionary<string, CommandSystem.ICommand>() },
        };

        private void OnReceiveCommandsData(ReceiveCommandsPacket packet, NetPeer peer)
        {
            foreach (var pCmd in packet.Commands)
            {
                if (!InstalledAddons.TryGetValue(pCmd.AddonID, out AddonInfo addonInfo))
                {
                    Logger.Error($"Addon with id \"{pCmd.AddonID}\" tried to add command \"{pCmd.CommandName.ToUpper()}\" but addon is not loaded!");
                    continue;
                }
                CommandType commandType = (CommandType)pCmd.Type;

                switch (commandType)
                {
                    case CommandType.GameConsole:
                        if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.GameConsoleNewCommands))
                        {
                            Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to add new \"GameConsole Command\" ({pCmd.CommandName.ToUpper()}) but server dont have required permission!");
                            continue;
                        }
                        break;
                    case CommandType.RemoteAdmin:
                        if (!Extensions.CheckSendPermission(AddonReceivePermissionTypes.RemoteAdminNewCommands))
                        {
                            Logger.Error($"Addon \"{addonInfo.AddonName}\" tried to add new \"RA Command\" ({pCmd.CommandName.ToUpper()}) but server dont have required permission!");
                            continue;
                        }
                        break;
                }

                var command = new TemplateCommand();
                command.AssignedAddonID = pCmd.AddonID;
                command.DummyCommand = pCmd.CommandName;
                command.DummyDescription = pCmd.Description;
                command.Permission = pCmd.Permission;
                command.Type = pCmd.Type;

                Commands[commandType].Add(pCmd.CommandName.ToUpper(), command);

                switch (commandType)
                {
                    case CommandType.RemoteAdmin:
                        CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(command);
                        break;
                    case CommandType.GameConsole:
                        QueryProcessor.DotCommandHandler.RegisterCommand(command);
                        break;
                }

                Logger.Info($"[{commandType}] Command \"{pCmd.CommandName}\" registered in addon \"{addonInfo.AddonName}\".");
            }
        }

        private void SendClientToServer(Player hub, ushort port)
        {
            var serverPS = hub.ReferenceHub.playerStats;
            PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
            writer.WriteSingle(1f);
            writer.WriteUInt16(port);
            RpcMessage msg = new RpcMessage
            {
                netId = serverPS.netId,
                componentIndex = serverPS.ComponentIndex,
                functionHash = GetMethodHash(typeof(PlayerStats), "RpcRoundrestartRedirect"),
                payload = writer.ToArraySegment(),
            };
            hub.Connection.Send<RpcMessage>(msg, 0);
            NetworkWriterPool.Recycle(writer);
        }

        private IEnumerator<float> RefreshPolls()
        {
            while (true)
            {
                yield return Timing.WaitForOneFrame;
                if (NetworkListener != null)
                {
                    if (NetworkListener.IsRunning)
                        NetworkListener.PollEvents();
                }
            }
        }

        private IEnumerator<float> Reconnect()
        {
            yield return Timing.WaitForSeconds(5f);
            Logger.Info($"Reconnecting to {plugin.Config.HostAddress}:{plugin.Config.HostPort}...");
            NetworkListener.Connect(plugin.Config.HostAddress, plugin.Config.HostPort, defaultdata);
        }
    }
}
