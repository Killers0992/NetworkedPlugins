namespace NetworkedPlugins.Dedicated
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Threading.Tasks;

    using NetworkedPlugins.API;
    using NetworkedPlugins.API.Interfaces;
    using NetworkedPlugins.API.Models;

    using LiteNetLib;
    using LiteNetLib.Utils;
    using NetworkedPlugins.API.Enums;
    using System.Text;
    using NetworkedPlugins.API.Packets;
    using NetworkedPlugins.API.Packets.ClientPackets;
    using NetworkedPlugins.API.Events.Player;
    using NetworkedPlugins.API.Extensions;

    /// <summary>
    /// Dedicated host.
    /// </summary>
    public class Host : NPManager, INetEventListener
    {
        private DedicatedConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="Host"/> class.
        /// </summary>
        public Host()
        {
            Logger = new ConsoleLogger();
            Singleton = this;

            NetDebug.Logger = new NetworkLogger();

            if (!File.Exists("config.yml"))
                File.WriteAllText("config.yml", Serializer.Serialize(new DedicatedConfig()));

            config = Deserializer.Deserialize<DedicatedConfig>(File.ReadAllText("config.yml"));

            if (!Directory.Exists("addons"))
                Directory.CreateDirectory("addons");

            if (!Directory.Exists("servers"))
                Directory.CreateDirectory("servers");

            string[] addonsFiles = Directory.GetFiles("addons", "*.dll");
            Logger.Info($"Loading {addonsFiles.Length} addons.");
            foreach (var file in addonsFiles)
            {
                Assembly a = Assembly.LoadFrom(file);
                try
                {
                    string addonID = string.Empty;
                    foreach (Type t in a.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface))
                    {
                        if (!t.BaseType.IsGenericType || t.BaseType.GetGenericTypeDefinition() != typeof(NPAddonDedicated<,>))
                            continue;

                        IAddonDedicated<IConfig, IConfig> addon = null;
                        var constructor = t.GetConstructor(Type.EmptyTypes);
                        if (constructor != null)
                        {
                            addon = constructor.Invoke(null) as IAddonDedicated<IConfig, IConfig>;
                        }
                        else
                        {
                            var value = Array.Find(t.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public), property => property.PropertyType == t)?.GetValue(null);

                            if (value != null)
                                addon = value as IAddonDedicated<IConfig, IConfig>;
                        }

                        if (addon == null)
                            continue;

                        var addonType = addon.GetType();
                        var property = addonType.GetProperty("DefaultPath", BindingFlags.Public | BindingFlags.Instance);
                        var field = property.GetBackingField();
                        field.SetValue(addon, Path.Combine("addons"));

                        property = addonType.GetProperty("AddonPath", BindingFlags.Public | BindingFlags.Instance);
                        field = property.GetBackingField();
                        field.SetValue(addon, Path.Combine(addon.DefaultPath, addon.AddonName));

                        property = addonType.GetProperty("Manager", BindingFlags.Public | BindingFlags.Instance);
                        field = property.GetBackingField();
                        field.SetValue(addon, this);

                        property = addonType.GetProperty("Logger", BindingFlags.Public | BindingFlags.Instance);
                        field = property.GetBackingField();
                        field.SetValue(addon, Logger);

                        LoadAddonConfig(addon);

                        if (!addon.Config.IsEnabled)
                            return;

                        foreach(var type in a.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface))
                        {
                            if (!type.BaseType.IsGenericType || type.BaseType.GetGenericTypeDefinition() != typeof(NPAddonHandler<>))
                                continue;

                            IAddonHandler<IConfig> addonHandler = null;
                            var constructor2 = type.GetConstructor(Type.EmptyTypes);
                            if (constructor != null)
                            {
                                addonHandler = constructor2.Invoke(null) as IAddonHandler<IConfig>;
                            }
                            else
                            {
                                var value = Array.Find(type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public), property => property.PropertyType == type)?.GetValue(null);

                                if (value != null)
                                    addonHandler = value as IAddonHandler<IConfig>;
                            }

                            if (addonHandler == null)
                                continue;

                            var addonType2 = addonHandler.GetType();

                            var property2 = addonType2.GetProperty("Manager", BindingFlags.Public | BindingFlags.Instance);
                            var field2 = property2.GetBackingField();
                            field2.SetValue(addonHandler, this);

                            property2 = addonType2.GetProperty("Logger", BindingFlags.Public | BindingFlags.Instance);
                            field2 = property2.GetBackingField();
                            field2.SetValue(addonHandler, Logger);

                            property2 = addonType2.GetProperty("DefaultAddon", BindingFlags.Public | BindingFlags.Instance);
                            field2 = property2.GetBackingField();
                            field2.SetValue(addonHandler, addon);

                            try
                            {
                                addonHandler.OnEnable();
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"Failed executing OnEnable in addon handler {addon.AddonName}. {ex.ToString()}");
                            }

                            if (DedicatedAddonHandlers.ContainsKey(addon.AddonId))
                            {
                                Logger.Error($"Addon with id \"{addon.AddonId}\" ({addon.AddonName}) is already registered!");
                                break;
                            }

                            foreach (var Cmdtype in a.GetTypes())
                            {
                                if (typeof(ICommand).IsAssignableFrom(Cmdtype))
                                {
                                    ICommand cmd = (ICommand)Activator.CreateInstance(Cmdtype);
                                    RegisterCommand(addon, cmd);
                                }
                            }

                            AddonAssemblies.Add(a, addon.AddonId);
                            DedicatedAddonHandlers.Add(addon.AddonId, addonHandler);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed loading addon {Path.GetFileNameWithoutExtension(file)}. {ex.ToString()}");
                }
            }

            Logger.Info($"Starting HOST network...");
            StartNetworkHost();
        }

        /// <inheritdoc/>
        public void OnPeerConnected(NetPeer peer) { }

        /// <inheritdoc/>
        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (Servers.TryGetValue(peer, out NPServer server))
            {
                server.UninitServer();
                Logger.Info($"Server \"{server.FullAddress}\" disconnected from host. (Info: {disconnectInfo.Reason.ToString()})");
                Servers.Remove(peer);
            }
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
                Logger.Error($"Failed in {peer.EndPoint.Address}, {ex}");
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
            if (!request.Data.TryGetString(out string key))
                return;

            if (key != config.HostConnectionKey && !string.IsNullOrEmpty(config.HostConnectionKey))
                return;

            if (!request.Data.TryGetString(out string version))
                return;

            if (Version.TryParse(version, out Version receivedVersion))
            {
                if (receivedVersion.CompareTo(NPVersion.Version) < 0)
                {
                    NetDataWriter writer = new NetDataWriter();
                    writer.Put((byte)RejectType.OutdatedVersion);
                    writer.Put(NPVersion.Version.ToString(3));
                    request.Reject(writer);
                    return;
                }
            }

            if (!request.Data.TryGetUShort(out ushort port))
                return;

            if (!request.Data.TryGetInt(out int maxplayers))
                return;

            if (!request.Data.TryGetStringArray(out string[] addons))
                return;

            if (!request.Data.TryGetBytesWithLength(out byte[] token))
                return;

            if (Servers.Values.FirstOrDefault(p => p.ServerAddress == request.RemoteEndPoint.Address.ToString() && p.ServerPort == port) != null)
            {
                Logger.Error($"Server with ip \"{request.RemoteEndPoint.Address.ToString()}\" and port \"{port}\" is already connected. (Multiple ips on machine)");
                request.Reject();
                return;
            }

            var server = new NPServer(PacketProcessor, request.RemoteEndPoint.Address.ToString(), port, maxplayers);

            var Addons = addons.ToList();
            Addons.AddRange(config.DefaultAddons);

            if (!server.InitServer(request, Encoding.UTF8.GetString(token), Addons))
                return;

            try
            {
                server.LoadInstalledAddons();
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed loading addons for server \"{server.FullAddress}\".\n{ex}");
            }


            Servers.Add(server.Peer, server);
            Logger.Info($"New server connected \"{server.FullAddress}\".");
        }

        private void StartNetworkHost()
        {
            PacketProcessor.RegisterNestedType<CommandInfo>();
            PacketProcessor.RegisterNestedType<PlayerInfo>();
            PacketProcessor.RegisterNestedType<Position>();
            PacketProcessor.RegisterNestedType<Rotation>();
            PacketProcessor.RegisterNestedType<AddonInfo>();
            PacketProcessor.SubscribeReusable<AddonDataPacket, NetPeer>(OnReceiveAddonData);
            PacketProcessor.SubscribeReusable<ExecuteCommandPacket, NetPeer>(OnExecuteCommand);
            PacketProcessor.SubscribeReusable<UpdatePlayerInfoPacket, NetPeer>(OnUpdatePlayerInfo);
            PacketProcessor.SubscribeReusable<ConsoleResponsePacket, NetPeer>(OnConsoleResponse);
            PacketProcessor.SubscribeReusable<AddonOkPacket, NetPeer>(OnAddonOk);
            PacketProcessor.SubscribeReusable<EventPacket, NetPeer>(OnReceiveEvent);
            NetworkListener = new NetManager(this);
            Logger.Info($"IP: {config.HostAddress}");
            Logger.Info($"Port: {config.HostPort}");
            NetworkListener.Start(config.HostPort);
            Task.Factory.StartNew(async () =>
            {
                await RefreshPolls();
            });
        }

        private void OnReceiveEvent(EventPacket packet, NetPeer peer)
        {
            if (!Servers.TryGetValue(peer, out NPServer server))
                return;

            NetDataReader data = new NetDataReader(packet.Data); 
            switch ((EventType)packet.Type)
            {
                case EventType.PlayerJoined:
                    {
                        string userId = data.GetString();
                        string nickname = data.GetString();
                        var newPlayer = new NPPlayer(server, userId);

                        if (server.PlayersDictionary.ContainsKey(userId))
                            break;

                        server.PlayersDictionary.Add(userId, newPlayer);

                        foreach (var handler in DedicatedAddonHandlers.Values)
                            handler.InvokePlayerJoined(new PlayerJoinedEvent(newPlayer), server);

                        NetDataWriter writer = new NetDataWriter();
                        writer.Put(userId);
                        PacketProcessor.Send<EventPacket>(peer, new EventPacket()
                        {
                            Type = (byte)EventType.PlayerJoined,
                            Data = writer.Data
                        }, DeliveryMethod.ReliableOrdered);
                    }
                    break;
                case EventType.PlayerLeft:
                    {
                        string userId = data.GetString();

                        if (!server.PlayersDictionary.TryGetValue(userId, out NPPlayer plr))
                            break;

                        foreach (var handler in DedicatedAddonHandlers.Values)
                            handler.InvokePlayerLeft(new PlayerLeftEvent(plr), server);

                        server.PlayersDictionary.Remove(userId);
                    }
                    break;
                case EventType.PlayerLocalReport:
                    {
                        string userId = data.GetString();
                        string targetUserId = data.GetString();
                        string reason = data.GetString();

                        if (!server.PlayersDictionary.TryGetValue(userId, out NPPlayer plr))
                            break;

                        if (!server.PlayersDictionary.TryGetValue(targetUserId, out NPPlayer target))
                            break;

                        foreach (var handler in DedicatedAddonHandlers.Values)
                            handler.InvokePlayerLocalReport(new PlayerLocalReportEvent(plr, target, reason), server);
                    }
                    break;
            }
        }

        private void OnAddonOk(AddonOkPacket packet, NetPeer peer)
        {
            if (!Servers.TryGetValue(peer, out NPServer server))
                return;

            var addon = server.GetAddon(packet.AddonId);
            if (addon == null)
                return;

            addon.RemoteConfig.CopyProperties((IConfig)Deserializer.Deserialize(Encoding.UTF8.GetString(packet.RemoteConfig), addon.RemoteConfig.GetType()));
                
            if (addon.RemoteConfig.IsEnabled)
            {
                try
                {
                    addon.OnEnable();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error while executin OnEnable event in \"{addon.AddonName}\"\n{ex}");
                }
            }
            else
            {
                Logger.Info($"[{server.FullAddress}] Addon \"{addon.AddonName}\" ({addon.AddonVersion}) made by {addon.AddonAuthor} is disabled!");
            }

            List<CommandInfo> commands = new List<CommandInfo>();
            commands.AddRange(addon.Commands[CommandType.GameConsole].Select(p => new CommandInfo()
            {
                AddonID = addon.AddonId,
                Description = p.Value.Description,
                CommandName = p.Value.CommandName,
                Permission = p.Value.Permission,
                Type = (byte)CommandType.GameConsole
            }).ToList());

            commands.AddRange(addon.Commands[CommandType.RemoteAdmin].Select(p => new CommandInfo()
            {
                AddonID = addon.AddonId,
                Description = p.Value.Description,
                CommandName = p.Value.CommandName,
                Permission = p.Value.Permission,
                Type = (byte)CommandType.RemoteAdmin
            }).ToList());

            PacketProcessor.Send<ReceiveCommandsPacket>(peer, new ReceiveCommandsPacket()
            {
                Commands = commands
            }, DeliveryMethod.ReliableOrdered);
        }

        private void OnConsoleResponse(ConsoleResponsePacket packet, NetPeer peer)
        {
            if (!Servers.TryGetValue(peer, out NPServer server))
                return;

            var addon = server.GetAddon(packet.AddonId);
            if (addon == null)
            {
                Logger.Error($"Failed receiving console response while addon with id \"{packet.AddonId}\" is not loaded on that server!");
                return;
            }

            addon.OnConsoleResponse(packet.Command, packet.Arguments, (CommandType)packet.Type, packet.Response);
        }

        private void OnUpdatePlayerInfo(UpdatePlayerInfoPacket packet, NetPeer peer)
        {
            if (!Servers.TryGetValue(peer, out NPServer server))
                return;

            if (!server.PlayersDictionary.ContainsKey(packet.UserID))
                server.PlayersDictionary.Add(packet.UserID, new NPPlayer(server, packet.UserID));

            if (!server.PlayersDictionary.TryGetValue(packet.UserID, out NPPlayer player))
                return;

            NetDataReader reader = new NetDataReader(packet.Data);

            switch ((PlayerDataType)packet.Type)
            {
                case PlayerDataType.Nickname:
                    player.Nickname = reader.GetString();
                    break;
                case PlayerDataType.Role:
                    player.Role = (PlayerRole)reader.GetUShort();
                    break;
                case PlayerDataType.DoNotTrack:
                    player.DoNotTrack = reader.GetBool();
                    break;
                case PlayerDataType.RemoteAdminAccess:
                    player.RemoteAdminAccess = reader.GetBool();
                    break;
                case PlayerDataType.Overwatch:
                    player.IsOverwatchEnabled = reader.GetBool();
                    break;
                case PlayerDataType.IPAddress:
                    player.IPAddress = reader.GetString();
                    break;
                case PlayerDataType.Mute:
                    player.IsMuted = reader.GetBool();
                    break;
                case PlayerDataType.IntercomMute:
                    player.IsIntercomMuted = reader.GetBool();
                    break;
                case PlayerDataType.Godmode:
                    player.IsGodModeEnabled = reader.GetBool();
                    break;
                case PlayerDataType.Health:
                    player.Health = reader.GetFloat();
                    break;
                case PlayerDataType.MaxHealth:
                    player.MaxHealth = reader.GetInt();
                    break;
                case PlayerDataType.GroupName:
                    player.GroupName = reader.GetString();
                    break;
                case PlayerDataType.RankColor:
                    player.RankColor = reader.GetString();
                    break;
                case PlayerDataType.RankName:
                    player.RankName = reader.GetString();
                    break;
                case PlayerDataType.Position:
                    player.Position = reader.Get<Position>();
                    break;
                case PlayerDataType.Rotation:
                    player.Rotation = reader.Get<Rotation>();
                    break;
                case PlayerDataType.PlayerID:
                    player.PlayerID = reader.GetInt();
                    break;
            }
        }

        private void OnExecuteCommand(ExecuteCommandPacket packet, NetPeer peer)
        {
            CommandType commandType = (CommandType)packet.Type; 
            if (!Servers.TryGetValue(peer, out NPServer server))
                return;

            if (!server.PlayersDictionary.TryGetValue(packet.UserID, out NPPlayer player))
            {
                Logger.Error($"Player \"{packet.UserID}\" tried executing command \"{packet.CommandName.ToUpper()}\" but its offline.");
                return;
            }

            var addon = server.GetAddon(packet.AddonID);
            if (addon == null)
            {
                Logger.Error($"Player \"{packet.UserID}\" tried executing command \"{packet.CommandName.ToUpper()}\" but addon id is invalid!");
                return;
            }

            if (addon.Commands[commandType].TryGetValue(packet.CommandName.ToUpper(), out ICommand cmd))
            {
                Logger.Info($"[{addon.AddonName}] [{commandType}] Player \"{packet.UserID}\" executed command \"{packet.CommandName.ToUpper()}\" with arguments \"{string.Join(" ", packet.Arguments)}\".");
                try
                {
                    cmd.Invoke(player, packet.Arguments);
                }
                catch(Exception ex)
                {
                    Logger.Error($"[{addon.AddonName}] Player \"{packet.UserID}\" failed to execute command \"{packet.CommandName.ToUpper()}\",\n{ex}");
                }
            }
            else
            {
                Logger.Error($"[{addon.AddonName}] Player \"{packet.UserID}\" tried executing command \"{packet.CommandName.ToUpper()}\" but command not exists in targeted addon!");
            }

        }

        private void OnReceiveAddonData(AddonDataPacket packet, NetPeer peer)
        {
            if (!Servers.TryGetValue(peer, out NPServer server))
                return;

            var addon = server.GetAddon(packet.AddonId);
            if (addon == null)
            {
                Logger.Error($"Failed receiving data while addon with id \"{packet.AddonId}\" is not loaded on that server!");
                return;
            }

            addon.OnMessageReceived(new NetDataReader(packet.Data));
        }

        private async Task RefreshPolls()
        {
            while (true)
            {
                await Task.Delay(15);
                if (NetworkListener != null)
                {
                    if (NetworkListener.IsRunning)
                        NetworkListener.PollEvents();
                }
            }
        }
    }
}
