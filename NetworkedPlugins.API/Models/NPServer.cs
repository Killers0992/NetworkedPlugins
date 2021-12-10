namespace NetworkedPlugins.API.Models
{
    using System.Collections.Generic;

    using NetworkedPlugins.API.Interfaces;
    using LiteNetLib;
    using LiteNetLib.Utils;
    using NetworkedPlugins.API.Packets.ServerPackets;
    using System.Linq;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using NetworkedPlugins.API.Extensions;
    using NetworkedPlugins.API.Enums;

    /// <summary>
    /// Server.
    /// </summary>
    public class NPServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NPServer"/> class.
        /// </summary>
        /// <param name="server">Server.</param>
        /// <param name="processor">Packet Processor.</param>
        /// <param name="serverAddress">Server Address.</param>
        /// <param name="port">Server Port.</param>
        /// <param name="maxPlayers">Max Players.</param>
        public NPServer(NetPacketProcessor processor, string serverAddress, ushort port, int maxPlayers)
        {
            this.PacketProcessor = processor;
            this.ServerAddress = serverAddress;
            this.ServerPort = port;
            this.MaxPlayers = maxPlayers;
        }

        public NetworkServerConfig ServerConfig { get; set; }
        public string ServerDirectory { get; set; }

        public IEnumerable<IAddonDedicated<IConfig, IConfig>> AddonInstances
        {
            get
            {
                List<IAddonDedicated<IConfig, IConfig>> addons = new List<IAddonDedicated<IConfig, IConfig>>();
                foreach(var handler in NPManager.Singleton.DedicatedAddonHandlers.Values)
                {
                    if (handler.AddonInstances.TryGetValue(this, out IAddonDedicated<IConfig, IConfig> addon))
                        addons.Add(addon);
                }
                return addons;
            }
        }

        public IAddonDedicated<IConfig, IConfig> GetAddon(string addonId)
        {
            if (NPManager.Singleton.DedicatedAddonHandlers.TryGetValue(addonId, out IAddonHandler<IConfig> handler))
                if (handler.AddonInstances.TryGetValue(this, out IAddonDedicated<IConfig, IConfig> addon))
                    return addon;

            return null;
        }

        public T GetAddon<T>(string addonId) where T : IAddonDedicated<IConfig, IConfig>
        {
            if (NPManager.Singleton.DedicatedAddonHandlers.TryGetValue(addonId, out IAddonHandler<IConfig> handler))
                if (handler.AddonInstances.TryGetValue(this, out IAddonDedicated<IConfig, IConfig> addon))
                    return (T)addon;       

            return default(T);
        }

        public void LoadServerConfig()
        {
            if (!Directory.Exists(Path.Combine(ServerDirectory)))
                Directory.CreateDirectory(Path.Combine(ServerDirectory));

            if (!File.Exists(Path.Combine(ServerDirectory, "config.yml")))
                File.WriteAllText(Path.Combine(ServerDirectory, "config.yml"), NPManager.Serializer.Serialize(new NetworkServerConfig()));

            ServerConfig = NPManager.Deserializer.Deserialize<NetworkServerConfig>(File.ReadAllText(Path.Combine(ServerDirectory, "config.yml")));
            SaveServerConfig();
        }

        public void SaveServerConfig()
        {
            File.WriteAllText(Path.Combine(ServerDirectory, "config.yml"), NPManager.Serializer.Serialize(ServerConfig));
        }

        public byte[] GenNewToken()
        {
            var guid = Guid.NewGuid();
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(guid.ToString()));
            ServerConfig.Token = token;

            SaveServerConfig();
            return Encoding.UTF8.GetBytes(token);
        }

        public void UpdateName()
        {
            if (string.IsNullOrEmpty(ServerConfig.ServerName))
                return;

            var property = this.GetType().GetProperty("ServerName", BindingFlags.Public | BindingFlags.Instance);
            var field = property.GetBackingField();
            field.SetValue(this, ServerConfig.ServerName);
        }


        public bool InitServer(ConnectionRequest request, string receivedToken, List<string> addons)
        {
            ServerDirectory = Path.Combine("servers", $"{ServerAddress}_{ServerPort}");

            LoadServerConfig();
            UpdateName();

            if (ServerConfig.InstalledAddons == null)
                ServerConfig.InstalledAddons = new List<string>();

            foreach (var addon in addons)
                if (!ServerConfig.InstalledAddons.Contains(addon))
                    ServerConfig.InstalledAddons.Add(addon);

            if (string.IsNullOrEmpty(ServerConfig.Token))
            {
                Peer = request.Accept();

                PacketProcessor.Send<SendTokenPacket>(Peer,
                    new SendTokenPacket()
                    {
                        Token = GenNewToken()
                    }, DeliveryMethod.ReliableOrdered);
                return true;
            }

            if (receivedToken != ServerConfig.Token)
            {
                NPManager.Singleton.Logger.Error($"Received invalid token from server {FullAddress}.");
                NetDataWriter writer = new NetDataWriter();
                writer.Put((byte)RejectType.InvalidToken);
                request.Reject(writer);
                return false;
            }

            Peer = request.Accept();
            return true;
        }

        public void UninitServer()
        {
            foreach (var addon in AddonInstances)
            {
                try
                {
                    addon.OnDisable();
                }
                catch (Exception ex)
                {
                    NPManager.Singleton.Logger.Error($"Error while executing OnDisable event in {addon.AddonName}\n{ex}");
                }
            }
        }

        public void LoadInstalledAddons()
        {
            foreach (var addon in NPManager.Singleton.DedicatedAddonHandlers.Where(p => ServerConfig.InstalledAddons.Contains(p.Key)))
            {
                addon.Value.AddAddon(this);
            }

            PacketProcessor.Send<SendAddonsInfoPacket>(Peer, new SendAddonsInfoPacket()
            {
                Addons = AddonInstances.Select(p => new Packets.AddonInfo()
                {
                    AddonId = p.AddonId,
                    AddonAuthor = p.AddonAuthor,
                    AddonName = p.AddonName,
                    AddonVersion = p.AddonVersion.ToString(3),
                    ReceivePermissions = p.Permissions.SendPermissions.Select(p2 => (byte)p2).ToArray(),
                    SendPermissions = p.Permissions.ReceivePermissions.Select(p2 => (byte)p2).ToArray(),
                    RemoteConfig = Encoding.UTF8.GetBytes(NPManager.Serializer.Serialize(p.RemoteConfig))
                }).ToList()
            }, DeliveryMethod.ReliableOrdered);
        }

        public void LoadAddon(string id)
        {
            var addon = NPManager.Singleton.DedicatedAddonHandlers.Values.FirstOrDefault(p => p.DefaultAddon.AddonId == id);
            if (addon == null)
                return;

            addon.AddAddon(this);

            PacketProcessor.Send<SendAddonsInfoPacket>(Peer, new SendAddonsInfoPacket()
            {
                Addons = new List<Packets.AddonInfo> 
                { 
                    new Packets.AddonInfo()
                    {
                        AddonId = addon.DefaultAddon.AddonId,
                        AddonAuthor = addon.DefaultAddon.AddonAuthor,
                        AddonName = addon.DefaultAddon.AddonName,
                        AddonVersion = addon.DefaultAddon.AddonVersion.ToString(3),
                        ReceivePermissions = addon.DefaultAddon.Permissions.SendPermissions.Select(p2 => (byte)p2).ToArray(),
                        SendPermissions = addon.DefaultAddon.Permissions.ReceivePermissions.Select(p2 => (byte)p2).ToArray(),
                        RemoteConfig = Encoding.UTF8.GetBytes(NPManager.Serializer.Serialize(addon.DefaultAddon.RemoteConfig))
                    }
                }
            }, DeliveryMethod.ReliableOrdered);
        }

        public void UnloadAddon(string id)
        {
            if (!ServerConfig.InstalledAddons.Contains(id))
                return;

            if (NPManager.Singleton.DedicatedAddonHandlers.TryGetValue(id, out IAddonHandler<IConfig> handler))
            {
                if (handler.AddonInstances.ContainsKey(this))
                {
                    handler.AddonInstances[this].OnDisable();
                    handler.AddonInstances.Remove(this);
                }
            }

            PacketProcessor.Send<UnloadAddonPacket>(Peer, new UnloadAddonPacket()
            {
                AddonIds = new string[] { id }
            }, DeliveryMethod.ReliableOrdered);
        }



        /// <summary>
        /// Gets or sets Packet processor.
        /// </summary>
        public NetPacketProcessor PacketProcessor { get; set; }

        /// <summary>
        /// Gets or sets Server peer.
        /// </summary>
        public NetPeer Peer { get; set; }

        /// <summary>
        /// Gets server name.
        /// </summary>
        public string ServerName { get; } = "Default Name";

        /// <summary>
        /// Gets server address.
        /// </summary>
        public string ServerAddress { get; } = "localhost";

        /// <summary>
        /// Gets server port.
        /// </summary>
        public ushort ServerPort { get; } = 7777;

        /// <summary>
        /// Gets server max players.
        /// </summary>
        public int MaxPlayers { get; } = 25;

        /// <summary>
        /// Gets dictionary of online players.
        /// </summary>
        public Dictionary<string, NPPlayer> PlayersDictionary { get; } = new Dictionary<string, NPPlayer>();

        /// <summary>
        /// Gets current online players.
        /// </summary>
        public IEnumerable<NPPlayer> Players => PlayersDictionary.Values;

        /// <summary>
        /// Gets server full address.
        /// </summary>
        public string FullAddress => $"{ServerAddress}:{ServerPort}";

        /// <summary>
        /// Gets player via userid.
        /// </summary>
        /// <param name="userId">Player UserID.</param>
        /// <returns>Player.</returns>
        public NPPlayer GetPlayer(string userId)
        {
            if (PlayersDictionary.ContainsKey(userId))
            {
                return PlayersDictionary[userId];
            }

            return null;
        }

        /// <summary>
        /// If player is online.
        /// </summary>
        /// <param name="userId">Player UserID.</param>
        /// <returns>Boolean.</returns>
        public bool IsPlayerOnline(string userId)
        {
            return PlayersDictionary.ContainsKey(userId);
        }

        /// <summary>
        /// Execute command on server.
        /// </summary>
        /// <param name="command">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        public void ExecuteCommand(string command, List<string> arguments)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(command);
            writer.PutArray(arguments.ToArray());
            PacketProcessor.Send<ServerInteractPacket>(Peer, 
                new ServerInteractPacket()
                { 
                    AddonId = Assembly.GetCallingAssembly().GetAddonId(),
                    Type = (byte)ServerInteractionType.ExecuteCommand,
                    Data = writer.Data,
                }, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Display broadcast on server.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="duration">Duration of broadcast.</param>
        /// <param name="isAdminOnly">Is displayed only for admins.</param>
        public void SendBroadcast(string message, ushort duration, bool isAdminOnly = false)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(message);
            writer.Put(duration);
            writer.Put(isAdminOnly);
            PacketProcessor.Send<ServerInteractPacket>(Peer,
                new ServerInteractPacket()
                {
                    AddonId = Assembly.GetCallingAssembly().GetAddonId(),
                    Type = (byte)ServerInteractionType.Broadcast,
                    Data = writer.Data,
                }, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Display hint on server.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="duration">Duration of hint.</param>
        /// <param name="isAdminOnly">Is displayed only for admins.</param>
        public void SendHint(string message, float duration, bool isAdminOnly = false)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(message);
            writer.Put(duration);
            writer.Put(isAdminOnly);
            PacketProcessor.Send<ServerInteractPacket>(Peer,
                new ServerInteractPacket()
                {
                    AddonId = Assembly.GetCallingAssembly().GetAddonId(),
                    Type = (byte)ServerInteractionType.Hint,
                    Data = writer.Data,
                }, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Clear all broadcasts on server.
        /// </summary>
        public void ClearBroadcast()
        {
            PacketProcessor.Send<ServerInteractPacket>(Peer,
                new ServerInteractPacket()
                {
                    AddonId = Assembly.GetCallingAssembly().GetAddonId(),
                    Type = (byte)ServerInteractionType.ClearBroadcast,
                    Data = new byte[0],
                }, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Roundrestart server.
        /// </summary>
        /// <param name="port">Optional for redirecting everyone to other server..</param>
        public void RoundRestart(ushort port = 0)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(port);
            PacketProcessor.Send<ServerInteractPacket>(Peer,
                new ServerInteractPacket()
                {
                    AddonId = Assembly.GetCallingAssembly().GetAddonId(),
                    Type = (byte)ServerInteractionType.Hint,
                    Data = writer.Data,
                }, DeliveryMethod.ReliableOrdered);
        }
    }
}
