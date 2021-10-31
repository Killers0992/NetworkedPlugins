namespace NetworkedPlugins
{
    using Exiled.API.Features;
    using LiteNetLib;
    using LiteNetLib.Utils;
    using NetworkedPlugins.API;
    using NetworkedPlugins.API.Enums;
    using NetworkedPlugins.API.Packets.ClientPackets;
    using NetworkedPlugins.API.Structs;
    using UnityEngine;

    public class NetworkedPlayer : MonoBehaviour
    {
        private ReferenceHub _hub;
        private Player _player;

        public bool SendPositionData { get; set; }
        public bool SendRotationData { get; set; }

        public NetworkedPlayerData NetworkData = new NetworkedPlayerData();

        public ReferenceHub Hub
        {
            get
            {
                if (_hub != null)
                    return _hub;
                _hub = GetComponent<ReferenceHub>();
                return _hub;
            }
        }

        public Player Player
        {
            get
            {
                if (_player != null)
                    return _player;
                _player = Player.Get(Hub);
                return _player;
            }
        }

        void Awake()
        {
            Reset();
        }

        public void Reset()
        {
            NetworkData = new NetworkedPlayerData();
            NetDataWriter writer = new NetDataWriter();
            writer.Put(Player.UserId);
            NPManager.Singleton.PacketProcessor.Send<EventPacket>(
                NPManager.Singleton.NetworkListener,
                new EventPacket()
                {
                    Type = (byte)EventType.PlayerJoined,
                    Data = writer.Data
                },
                DeliveryMethod.ReliableOrdered);
        }

        void OnDestroy()
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(Player.UserId);
            NPManager.Singleton.PacketProcessor.Send<EventPacket>(
                NPManager.Singleton.NetworkListener,
                new EventPacket()
                {
                    Type = (byte)EventType.PlayerLeft,
                    Data = writer.Data
                },
                DeliveryMethod.ReliableOrdered);
        }

        void Update()
        {
            if (!NetworkData.IsConnected)
                return;

            NetDataWriter writer = null;
            if (NetworkData.Nickname != Player.Nickname && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerNickname))
            {
                NetworkData.Nickname = Player.Nickname;
                writer = new NetDataWriter();
                writer.Put(NetworkData.Nickname);
                NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                    NPManager.Singleton.NetworkListener, 
                    new UpdatePlayerInfoPacket() 
                    { 
                        UserID = Player.UserId, 
                        Type = (byte)PlayerDataType.Nickname, 
                        Data = writer.Data 
                    }, 
                    DeliveryMethod.ReliableOrdered);
            }

            if (NetworkData.Role != (PlayerRole)Player.Role && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerRole))
            {
                NetworkData.Role = (PlayerRole)Player.Role;
                writer = new NetDataWriter();
                writer.Put((sbyte)NetworkData.Role);
                NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                    NPManager.Singleton.NetworkListener,
                    new UpdatePlayerInfoPacket()
                    {
                        UserID = Player.UserId,
                        Type = (byte)PlayerDataType.Role,
                        Data = writer.Data
                    },
                    DeliveryMethod.ReliableOrdered);
            }

            if (NetworkData.DoNotTrack != Player.DoNotTrack && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerDoNotTrack))
            {
                NetworkData.DoNotTrack = Player.DoNotTrack;
                writer = new NetDataWriter();
                writer.Put(NetworkData.DoNotTrack);
                NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                    NPManager.Singleton.NetworkListener,
                    new UpdatePlayerInfoPacket()
                    {
                        UserID = Player.UserId,
                        Type = (byte)PlayerDataType.DoNotTrack,
                        Data = writer.Data
                    },
                    DeliveryMethod.ReliableOrdered);
            }

            if (NetworkData.RemoteAdminAccess != Player.RemoteAdminAccess && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerRemoteAdminAccess))
            {
                NetworkData.RemoteAdminAccess = Player.RemoteAdminAccess;
                writer = new NetDataWriter();
                writer.Put(NetworkData.RemoteAdminAccess);
                NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                    NPManager.Singleton.NetworkListener,
                    new UpdatePlayerInfoPacket()
                    {
                        UserID = Player.UserId,
                        Type = (byte)PlayerDataType.RemoteAdminAccess,
                        Data = writer.Data
                    },
                    DeliveryMethod.ReliableOrdered);
            }

            if (NetworkData.IsOverwatchEnabled != Player.IsOverwatchEnabled && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerOverwatch))
            {
                NetworkData.IsOverwatchEnabled = Player.IsOverwatchEnabled;
                writer = new NetDataWriter();
                writer.Put(NetworkData.IsOverwatchEnabled);
                NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                    NPManager.Singleton.NetworkListener,
                    new UpdatePlayerInfoPacket()
                    {
                        UserID = Player.UserId,
                        Type = (byte)PlayerDataType.Overwatch,
                        Data = writer.Data
                    },
                    DeliveryMethod.ReliableOrdered);
            }

            if (NetworkData.IPAddress != Player.IPAddress && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerIPAddress))
            {
                NetworkData.IPAddress = Player.IPAddress;
                writer = new NetDataWriter();
                writer.Put(NetworkData.IsOverwatchEnabled);
                NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                    NPManager.Singleton.NetworkListener,
                    new UpdatePlayerInfoPacket()
                    {
                        UserID = Player.UserId,
                        Type = (byte)PlayerDataType.IPAddress,
                        Data = writer.Data
                    },
                    DeliveryMethod.ReliableOrdered);
            }

            if (NetworkData.IsMuted != Player.IsMuted && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerMute))
            {
                NetworkData.IsMuted = Player.IsMuted;
                writer = new NetDataWriter();
                writer.Put(NetworkData.IsMuted);
                NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                    NPManager.Singleton.NetworkListener,
                    new UpdatePlayerInfoPacket()
                    {
                        UserID = Player.UserId,
                        Type = (byte)PlayerDataType.Mute,
                        Data = writer.Data
                    },
                    DeliveryMethod.ReliableOrdered);
            }

            if (NetworkData.IsIntercomMuted != Player.IsIntercomMuted)
            {
                NetworkData.IsIntercomMuted = Player.IsIntercomMuted;
                writer = new NetDataWriter();
                writer.Put(NetworkData.IsIntercomMuted);
                NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                    NPManager.Singleton.NetworkListener,
                    new UpdatePlayerInfoPacket()
                    {
                        UserID = Player.UserId,
                        Type = (byte)PlayerDataType.IntercomMute,
                        Data = writer.Data
                    },
                    DeliveryMethod.ReliableOrdered);
            }

            if (NetworkData.IsGodModeEnabled != Player.IsGodModeEnabled && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerGodmode))
            {
                NetworkData.IsGodModeEnabled = Player.IsGodModeEnabled;
                writer = new NetDataWriter();
                writer.Put(NetworkData.IsGodModeEnabled);
                NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                    NPManager.Singleton.NetworkListener,
                    new UpdatePlayerInfoPacket()
                    {
                        UserID = Player.UserId,
                        Type = (byte)PlayerDataType.Godmode,
                        Data = writer.Data
                    },
                    DeliveryMethod.ReliableOrdered);
            }

            if (NetworkData.Health != Player.Health && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerHealth))
            {
                NetworkData.Health = Player.Health;
                writer = new NetDataWriter();
                writer.Put(NetworkData.Health);
                NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                    NPManager.Singleton.NetworkListener,
                    new UpdatePlayerInfoPacket()
                    {
                        UserID = Player.UserId,
                        Type = (byte)PlayerDataType.Health,
                        Data = writer.Data
                    },
                    DeliveryMethod.ReliableOrdered);
            }

            if (NetworkData.MaxHealth != Player.MaxHealth && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerMaxHealth))
            {
                NetworkData.MaxHealth = Player.MaxHealth;
                writer = new NetDataWriter();
                writer.Put(NetworkData.MaxHealth);
                NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                    NPManager.Singleton.NetworkListener,
                    new UpdatePlayerInfoPacket()
                    {
                        UserID = Player.UserId,
                        Type = (byte)PlayerDataType.MaxHealth,
                        Data = writer.Data
                    },
                    DeliveryMethod.ReliableOrdered);
            }

            if (NetworkData.GroupName != Player.GroupName && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerGroupName))
            {
                NetworkData.GroupName = Player.GroupName;
                writer = new NetDataWriter();
                writer.Put(NetworkData.GroupName);
                NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                    NPManager.Singleton.NetworkListener,
                    new UpdatePlayerInfoPacket()
                    {
                        UserID = Player.UserId,
                        Type = (byte)PlayerDataType.GroupName,
                        Data = writer.Data
                    },
                    DeliveryMethod.ReliableOrdered);
            }

            if (NetworkData.RankColor != Player.RankColor && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerRankColor))
            {
                NetworkData.RankColor = Player.RankColor;
                writer = new NetDataWriter();
                writer.Put(NetworkData.RankColor);
                NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                    NPManager.Singleton.NetworkListener,
                    new UpdatePlayerInfoPacket()
                    {
                        UserID = Player.UserId,
                        Type = (byte)PlayerDataType.RankColor,
                        Data = writer.Data
                    },
                    DeliveryMethod.ReliableOrdered);
            }

            if (NetworkData.RankName != Player.RankName && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerRankColor))
            {
                NetworkData.RankName = Player.RankName;
                writer = new NetDataWriter();
                writer.Put(NetworkData.RankName);
                NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                    NPManager.Singleton.NetworkListener,
                    new UpdatePlayerInfoPacket()
                    {
                        UserID = Player.UserId,
                        Type = (byte)PlayerDataType.RankName,
                        Data = writer.Data
                    },
                    DeliveryMethod.ReliableOrdered);
            }

            if (NetworkData.PlayerID != Player.Id && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerPlayerID))
            {
                NetworkData.PlayerID = Player.Id;
                writer = new NetDataWriter();
                writer.Put(NetworkData.PlayerID);
                NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                    NPManager.Singleton.NetworkListener,
                    new UpdatePlayerInfoPacket()
                    {
                        UserID = Player.UserId,
                        Type = (byte)PlayerDataType.PlayerID,
                        Data = writer.Data
                    },
                    DeliveryMethod.ReliableOrdered);
            }


            if (SendPositionData && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerPosition))
            {
                if (NetworkData.Position.X != Player.Position.x || NetworkData.Position.Y != Player.Position.y || NetworkData.Position.Z != Player.Position.z)
                {
                    writer = new NetDataWriter();
                    NetworkData.Position = new Position()
                    {
                        X = Player.Position.x,
                        Y = Player.Position.y,
                        Z = Player.Position.z,
                    };
                    writer.Put<Position>(NetworkData.Position);
                    NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                        NPManager.Singleton.NetworkListener, 
                        new UpdatePlayerInfoPacket() 
                        { 
                            UserID = Player.UserId,
                            Type = (byte)PlayerDataType.Position,
                            Data = writer.Data
                        }, 
                        DeliveryMethod.ReliableOrdered);
                }
            }

            if (SendRotationData && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerRotation))
            {
                if (NetworkData.Rotation.X != Player.Rotation.x || NetworkData.Rotation.Y != Player.Rotation.y || NetworkData.Rotation.Z != Player.Rotation.z)
                {
                    writer = new NetDataWriter();
                    NetworkData.Rotation = new Rotation()
                    {
                        X = Player.Rotation.x,
                        Y = Player.Rotation.y,
                        Z = Player.Rotation.z,
                    };
                    writer.Put<Rotation>(NetworkData.Rotation);
                    NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                        NPManager.Singleton.NetworkListener,
                        new UpdatePlayerInfoPacket()
                        {
                            UserID = Player.UserId,
                            Type = (byte)PlayerDataType.Rotation,
                            Data = writer.Data
                        },
                        DeliveryMethod.ReliableOrdered);
                }
            }
        }
    }
}
